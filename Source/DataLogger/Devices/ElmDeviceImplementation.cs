using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.Threading;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates code that is shared by AllProDevice and ScanToolDevice.
    /// </summary>
    public class ElmDeviceImplementation
    {
        protected IPort Port { get; private set; }

        public int MaxSendSize { get; protected set; }

        public int MaxReceiveSize { get; protected set; }

        public bool Supports4X { get; protected set; }

        public double Voltage { get; protected set; }

        public TimeoutScenario TimeoutScenario { get; set; }

        protected readonly Action<OBDMessage> enqueue;

        protected readonly Action<OBDMessage> MessageSent;

        protected readonly Func<int> getReceivedMessageCount;

        public DataLogger.LoggingDevType LogDeviceType { get; protected set; }

        public int ReadTimeout = 1000;

        public int WriteTimeout = 1000;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ElmDeviceImplementation(
            Action<OBDMessage> enqueue,
            Func<int> getReceivedMessageCount,
            IPort port,
            Action<OBDMessage> MessageSent)
        {
            this.enqueue = enqueue;
            this.getReceivedMessageCount = getReceivedMessageCount;
            this.Port = port;
            this.MessageSent = MessageSent;
            // These are only relevant for device initialization.
            // After that, configuration from the derived classes will be used instead.
            this.MaxReceiveSize = 200;
            this.MaxSendSize = 200;
            this.Supports4X = false;            
        }

        /// <summary>
        /// This string is what will appear in the drop-down list in the UI.
        /// </summary>
        public virtual string GetDeviceType()
        {
            throw new NotImplementedException("This is only implemented by derived classes.");
        }

        /// <summary>
        /// Confirm that we're actually connected to the right device, and initialize it.
        /// </summary>
        public virtual bool Initialize()
        {
            string response;
            // This is common across all ELM-based devices.
            this.SendRequest(""); // send a cr/lf to prevent the ATZ failing.
            Debug.WriteLine(response = this.SendRequest("AT Z").Data);  // reset
            if(string.IsNullOrWhiteSpace(response))
            {
                LoggerBold($"No device found on {this.Port.ToString()}");
                return false;
            }

            Debug.WriteLine(this.SendRequest("AT E0").Data); // disable echo
            Debug.WriteLine(this.SendRequest("AT S0").Data); // no spaces on responses

            string voltage = this.SendRequest("AT RV").Data;             // Get Voltage
            Logger("Voltage: " + voltage);
            if (double.TryParse(voltage, out double volts))
            {
                this.Voltage = volts;
            }

            // First we check for known-bad ELM clones.
            string elmID = this.SendRequest("AT I").Data;                // Identify (ELM)
            if (elmID != "?")
            {
                Logger("Elm ID: " + elmID);
                if (elmID.Contains("ELM327 v1.5"))
                {
                    // TODO: Add a URL to a web page with a list of supported devices.
                    // No such web page exists yet, but I'm sure we'll create one some day...
                    Logger("ERROR: This OBD2 interface is not supported.");
                    //return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the time required for the given scenario.
        /// </summary>
        public virtual int GetTimeoutMilliseconds(TimeoutScenario scenario, VpwSpeed speed)
        {
            // This base class is only instantiated for device-independent initialization.
            return 250;
        }

        /// <summary>
        /// Set the timeout to the device. If this is set too low, the device
        /// will return 'No Data'. ELM327 is limited to 1020 milliseconds maximum.
        /// </summary>
        public virtual bool SetTimeoutMilliseconds(int milliseconds)
        {
           int parameter = Math.Min(Math.Max(1, (milliseconds / 4)), 255);
           string value = parameter.ToString("X2");
           return this.SendAndVerify("AT ST " + value, "OK");
        }

        /// <summary>
        /// Set the write timeout to the device. 
        /// </summary>
        public virtual void SetWriteTimeoutMilliseconds(int milliseconds)
        {
            WriteTimeout = milliseconds;
        }

        /// <summary>
        /// Set the Read timeout to the device. 
        /// </summary>
        public virtual void SetReadTimeoutMilliseconds(int milliseconds)
        {
            ReadTimeout = milliseconds;
        }

        /// <summary>
        /// Send a message, do not expect a response.
        /// </summary>
        public virtual bool SendMessage(OBDMessage message, int responses)
        {
            throw new NotImplementedException("This is only implemented by derived classes.");
        }

        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        public virtual void Receive()
        {
            throw new NotImplementedException("This is only implemented by derived classes.");
        }

        /// <summary>
        /// Send a request in string form, wait for a response (for init)
        /// </summary>
        /// <remarks>
        /// The API for this method (sending a string, returning a string) matches
        /// the way that we need to communicate with ELM and STN devices for setup
        /// </remarks>
        public SerialString SendRequest(string request, bool getresponse = true)
        {
            Debug.WriteLine("TX: " + request);
            try
            {
                Port.DiscardBuffers();
                this.Port.Send(Encoding.ASCII.GetBytes(request + " \r"));
                if (getresponse)
                {
                    return ReadELMLine(true);
                }
                else
                {
                    return new SerialString("",0,false);
                }
            }
            catch (TimeoutException)
            {
                return new SerialString("", 0, false);
            }
        }

        /// <summary>
        /// Send a command to the device, confirm that we got the response we expect. 
        /// </summary>
        /// <remarks>
        /// This is primarily for use in the Initialize method, where we're talking to the 
        /// interface device rather than the PCM.
        /// </remarks>
        public  bool SendAndVerify(string message, string expectedResponse)
        {
            string actualResponse = this.SendRequest(message).Data;

            if (string.Equals(actualResponse, expectedResponse))
            {
                Debug.WriteLine(actualResponse);
                return true;
            }

            Debug.WriteLine("Did not recieve expected response. " + actualResponse + " does not equal " + expectedResponse);
            return false;
        }


        /// <summary>
        /// Reads and filters a line from the device, returns it as a string
        /// </summary>
        /// <remarks>
        /// Strips Non Printable, >, and Line Feeds. Converts Carriage Return to Space. Strips leading and trailing whitespace.
        /// </remarks>
        public SerialString ReadELMLine(bool MultiLine)
        {
            // (MaxReceiveSize * 2) + 2 for Checksum + longest possible prompt. Minimum prompt 2 CR + 1 Prompt, +2 extra
            int maxPayload = (MaxReceiveSize * 2) + 7;

            // A buffer to receive a single byte.
            SerialByte buffer = new SerialByte(1);

            bool Prompt = false;

            // Use StringBuilder to collect the bytes.
            StringBuilder builtString = new StringBuilder();

            //Build list of timestamps, one for each row/message
            List<long> tStamps = new List<long>();
            bool newRow = true;

            //for (int i = 0; i < maxPayload; i++)
            while (true)
            {
                // Receive a single byte.
                this.Port.Receive(buffer, 0, 1);

                if (newRow)
                {
                    newRow = false;
                    tStamps.Add(buffer.TimeStamp);
                }
                // Is it the prompt '>'.
                if (buffer.Data[0] == '>') // || buffer.Data[0] == '?')
                {
                    // Prompt found, we're done.
                    //Debug.WriteLine("Elm prompt: " + builtString.ToString());
                    Debug.WriteLine("Elm promt received at " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                    Prompt = true;
                    break;
                }

                // Is it a CR
                if (buffer.Data[0] == 13)
                {
                    if (MultiLine)
                    {
                        //Handle multiple lines as one message
                        // CR found, replace with space.
                        buffer.Data[0] = 32;
                        newRow = true;
                    }
                    else if (!string.IsNullOrEmpty(builtString.ToString()) && !string.IsNullOrWhiteSpace(builtString.ToString()))
                    {                        
                        break;
                    }

                }

                // Printable characters only.
                if (buffer.Data[0] >= 32 && buffer.Data[0] <= 126)
                {
                    // Append it to builtString.
                    builtString.Append((char)buffer.Data[0]);
                }
            }

            // Convert to string, trim and return
            SerialString serialStr = new SerialString(builtString.ToString().Trim(), buffer.TimeStamp, Prompt);
            serialStr.TimeStamps = tStamps;
            //Debug.WriteLine("Elm line: " + serialStr.Data);
            return serialStr;
        }

        /// <summary>
        /// Collects a line with ReadELMLine() and converts it to a Message
        /// </summary>
        protected Response<OBDMessage> ReadELMPacket()
        {
            //Debug.WriteLine("Trace: ReadELMPacket");
            SerialString response = ReadELMLine(false);
            byte[] message = response.Data.ToBytes();
            OBDMessage rMsg = new OBDMessage(message);
            rMsg.TimeStamp = (ulong)response.TimeStamp;
            rMsg.DevTimeStamp = (ulong)response.TimeStamp;
            rMsg.ElmPrompt = response.Prompt;
            return Response.Create(ResponseStatus.Success, rMsg);

            //return Response.Create(ResponseStatus.Success, new OBDMessage(message));
        }

        /// <summary>
        /// Process responses from the ELM/ST devices.
        /// </summary>
        public bool ProcessResponse(SerialString rawResponse, string context, bool allowEmpty = false)
        {
            if (rawResponse.Prompt && (rawResponse.Data.Length < 5 || rawResponse.Data.Contains("STOPPED")) ) 
            {
                //We have received prompt
                Debug.WriteLine("Processresponse with prompt: " + rawResponse.Data);
                OBDMessage response = new OBDMessage(new byte[0]);
                //response.ElmLine = rawResponse.Data;
                //if (rawResponse.Data.StartsWith("6C") || rawResponse.Data.StartsWith("8C"))
                  //  response = new OBDMessage(rawResponse.Data.ToBytes());
                response.TimeStamp = (ulong)rawResponse.TimeStamp;
                response.DevTimeStamp = (ulong)rawResponse.TimeStamp;
                response.ElmPrompt = true;
                this.enqueue(response);
                return true;
            }

            if (string.IsNullOrWhiteSpace(rawResponse.Data))
            {
                if (allowEmpty)
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine("Empty response to " + context + ". That's not OK.");
                    return false;
                }
            }


            if (rawResponse.Data == "OK")
            {
                return true;
            }

            // We sent successfully, but the PCM didn't reply immediately.
            if (rawResponse.Data == "NO DATA")
            {
                return true;
            }

            string[] segments = rawResponse.Data.Replace("BUFFER FULL", "").Replace("OUT OF MEMORY", "").Split('<');
            foreach (string segment in segments)
            {
                if (segment.IsHex())
                {
                    int s = 0;
                    string[] hexResponses = segment.Split(' ');
                    foreach (string singleHexResponse in hexResponses)
                    {
                        byte[] deviceResponseBytes = singleHexResponse.ToBytes();
                        if (deviceResponseBytes.Length > 0)
                        {
                            Array.Resize(ref deviceResponseBytes, deviceResponseBytes.Length - 1); // remove checksum byte
                        }
                        Debug.WriteLine("RX: " + deviceResponseBytes.ToHex());
                        //Debug.WriteLine("Timestamp " + s.ToString() +": "+ rawResponse.TimeStamps[s].ToString());
                        OBDMessage response = new OBDMessage(deviceResponseBytes);
                        response.DevTimeStamp = (ulong)rawResponse.TimeStamps[s];
                        response.TimeStamp = (ulong)rawResponse.TimeStamps[s];
                        response.ElmPrompt = rawResponse.Prompt;
                        this.enqueue(response);
                        s++;
                    }
                    return true;
                }

                if (segment.EndsWith("OK"))
                {
                    Debug.WriteLine("WTF: Response not valid, but ends with OK.");
                    return true;
                }

                Debug.WriteLine(
                    string.Format(
                        "Unexpected response to {0}: {1}",
                        context,
                        segment));
            }

            return false;
        }


    }
}