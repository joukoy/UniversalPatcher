using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Upatcher;
using static Helpers;
using System.Diagnostics;

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

        public TimeoutScenario TimeoutScenario { get; set; }

        protected readonly Action<OBDMessage> enqueue;

        protected readonly Func<int> getRecievedMessageCount;

        public DataLogger.LoggingDevType LogDeviceType { get; protected set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public ElmDeviceImplementation(
            Action<OBDMessage> enqueue,
            Func<int> getRecievedMessageCount,
            IPort port)
        {
            this.enqueue = enqueue;
            this.getRecievedMessageCount = getRecievedMessageCount;
            this.Port = port;

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
            Debug.WriteLine(response = this.SendRequest("AT Z"));  // reset

            if(string.IsNullOrWhiteSpace(response))
            {
                LoggerBold($"No device found on {this.Port.ToString()}");
                return false;
            }

            Debug.WriteLine(this.SendRequest("AT E0")); // disable echo
            Debug.WriteLine(this.SendRequest("AT S0")); // no spaces on responses

            string voltage = this.SendRequest("AT RV");             // Get Voltage
            Logger("Voltage: " + voltage);

            // First we check for known-bad ELM clones.
            string elmID = this.SendRequest("AT I");                // Identify (ELM)
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
        public string SendRequest(string request, bool getresponse = true)
        {
            Debug.WriteLine("TX: " + request);
            try
            {
                Port.DiscardBuffers();
                this.Port.Send(Encoding.ASCII.GetBytes(request + " \r"));
                if (getresponse)
                {
                    string response = ReadELMLine(true).Data;
                    return response;
                }
                else
                {
                    return "";
                }
            }
            catch (TimeoutException)
            {
                return "";
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
            string actualResponse = this.SendRequest(message);

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
        protected SerialString ReadELMLine(bool MultiLine)
        {
            // (MaxReceiveSize * 2) + 2 for Checksum + longest possible prompt. Minimum prompt 2 CR + 1 Prompt, +2 extra
            int maxPayload = (MaxReceiveSize * 2) + 7;

            // A buffer to receive a single byte.
            SerialByte buffer = new SerialByte(1);

            bool Prompt = false;

            // Use StringBuilder to collect the bytes.
            StringBuilder builtString = new StringBuilder();

            for (int i = 0; i < maxPayload; i++)
            {
                // Receive a single byte.
                this.Port.Receive(buffer, 0, 1);

                // Is it the prompt '>'.
                if (buffer.Data[0] == '>')
                {
                    // Prompt found, we're done.
                    //Debug.WriteLine("Elm prompt: " + builtString.ToString());
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
                    }
                    else if (!string.IsNullOrWhiteSpace(builtString.ToString()))
                    {
                        //Receiving multiple messages at once, message/row without prompt
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
            rMsg.ElmPrompt = response.Prompt;
            return Response.Create(ResponseStatus.Success, rMsg);

            //return Response.Create(ResponseStatus.Success, new OBDMessage(message));
        }

        /// <summary>
        /// Process responses from the ELM/ST devices.
        /// </summary>
        protected bool ProcessResponse(SerialString rawResponse, string context, bool allowEmpty = false)
        {
            if (rawResponse.Prompt || rawResponse.Data.Contains("STOPPED"))
            {
                //We have received prompt
                //Debug.WriteLine("Processresponse: " + rawResponse.Data);
                OBDMessage response = new OBDMessage(Encoding.ASCII.GetBytes(rawResponse.Data));
                response.TimeStamp = (ulong)rawResponse.TimeStamp;
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

            string[] segments = rawResponse.Data.Split('<');
            foreach (string segment in segments)
            {
                if (segment.IsHex())
                {
                    string[] hexResponses = segment.Split(' ');
                    foreach (string singleHexResponse in hexResponses)
                    {
                        byte[] deviceResponseBytes = singleHexResponse.ToBytes();
                        if (deviceResponseBytes.Length > 0)
                        {
                            Array.Resize(ref deviceResponseBytes, deviceResponseBytes.Length - 1); // remove checksum byte
                        }

                        //Debug.WriteLine("RX: " + deviceResponseBytes.ToHex());

                        OBDMessage response = new OBDMessage(deviceResponseBytes);
                        response.TimeStamp = (ulong)rawResponse.TimeStamp;
                        this.enqueue(response);
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