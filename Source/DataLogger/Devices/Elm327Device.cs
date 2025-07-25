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
using J2534DotNet;
using static LoggerUtils;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates common code for ELM-derived devices, and also handles detecting the
    /// specific ELM device that is attached. After detecting the device is acts as a facade, with
    /// a device-specific class implementing device-specific functionality.
    /// </summary>
    public class Elm327Device : SerialDevice
    {
        /// <summary>
        /// Device type for use in the Device Picker dialog box, and for internal comparisons.
        /// </summary>
        public const string DeviceType = "Elm327 (CAN + VPW)";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Elm327Device(IPort port) : base(port)
        {
            this.Supports4X = false;

        }
        public double Voltage { get; protected set; }

        public int MaxSendSize { get; protected set; }

        public int MaxReceiveSize { get; protected set; }

        public bool Supports4X { get; protected set; }
        public J2534InitParameters CanParameters { get; set; }
        public TimeoutScenario TimeoutScenario { get; set; }

        public int ReadTimeout = 1000;

        public int WriteTimeout = 1000;
        private string currentHeader = "unset";
        private bool Elm21 = true;

        protected readonly Func<int> getReceivedMessageCount;
        /// <summary>
        /// This string is what will appear in the drop-down list in the UI.
        /// </summary>
        public override string GetDeviceType()
        {
            return DeviceType;
        }

        protected override void Dispose(bool disposing)
        {
            this.Port.ClosePort();
        }

        /// <summary>
        /// Use the related classes to discover which type of device is currently connected.
        /// </summary>
        public override bool Initialize(int BaudRate, J2534InitParameters j2534Init)
        {
            try
            {
                Debug.WriteLine("Elm327Device initialization starting.");

                this.Protocol = j2534Init.Protocol;
                this.CanParameters = j2534Init;
                SerialPortConfiguration configuration = new SerialPortConfiguration();
                configuration.BaudRate = BaudRate;
                //configuration.Timeout = 1200;
                configuration.Timeout = 2000;

                this.Port.OpenAsync(configuration);
                this.Port.DiscardBuffers();

                string response;
                // This is common across all ELM-based devices.
                this.SendRequest(""); // send a cr/lf to prevent the ATZ failing.
                Debug.WriteLine(response = this.SendRequest("AT Z").Data);  // reset
                if (string.IsNullOrWhiteSpace(response))
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

                Logger("Elm firmware: " + this.SendRequest("AT @1").Data);          // firmware check
                if (this.Protocol == ProtocolID.ISO15765)
                {
                    if (!SendAndVerify("AT AL", "OK") ||               // Allow Long packets
                        !SendAndVerify("AT SP6", "OK") ||              // Set Protocol 6 (CAN)
                        !SendAndVerify("AT DP", "ISO 15765-4 (CAN 11/500)") ||    // Get Protocol (Verify 15765-4)
                        !SendAndVerify("AT AR", "OK") ||               // Turn Auto Receive on (default should be on anyway)
                        !SendAndVerify("AT AT0", "OK") ||              // Disable adaptive timeouts
                        !SendAndVerify("AT SH " + j2534Init.CanPCM.RequestID.ToString("X") , "OK") || // Set header
                        !SendAndVerify("AT H1", "OK") ||               // Send headers
                        !SendAndVerify("AT ST 20", "OK")             // Set timeout (will be adjusted later, too)                 
                                                                                         //|| !SendAndVerify("STPX SH=7E0,SR=7E8,DATA=2CFE000B0010000C", "OK")               // Set timeout (will be adjusted later, too)                 
                        )
                    {
                        LoggerBold("Elm327 init failed");
                        return false;
                    }
                    if (!elmID.Contains("2.1"))
                    {
                        Elm21 = false;
                        if (!SendAndVerify("AT CF" + j2534Init.CanPCM.DiagID.ToString("X"), "OK") ||     //Set filter id
                            !SendAndVerify("AT CM5FF", "OK") ||     //Set filter mask
                            !SendAndVerify("AT CAF0", "OK") ||               // Don't format isotp messages automatically                       
                            !SendAndVerify("AT FCSM0", "OK") ||         //Automatically send flow control messages
                            !SendAndVerify("AT CFC1", "OK")           //Automatically send flow control messages
                        )
                        {
                            LoggerBold("Elm327 init failed");
                            return false;
                        }
                    }

                }
                else if (this.Protocol == ProtocolID.J1850VPW)
                {
                    // These are shared by all ELM-based devices.
                    if (!SendAndVerify("AT AL", "OK") ||               // Allow Long packets
                        !SendAndVerify("AT SP2", "OK") ||              // Set Protocol 2 (VPW)
                        !SendAndVerify("AT DP", "SAE J1850 VPW") ||    // Get Protocol (Verify VPW)
                        !SendAndVerify("AT AR", "OK") ||               // Turn Auto Receive on (default should be on anyway)
                        !SendAndVerify("AT AT0", "OK") ||              // Disable adaptive timeouts
                      //!SendAndVerify("AT SR " + DeviceId.Tool.ToString("X2"), "OK") || // Set receive filter to this tool ID
                        !SendAndVerify("AT H1", "OK") ||               // Send headers
                        !SendAndVerify("AT ST 20", "OK")               // Set timeout (will be adjusted later, too)                 
                        )
                    {
                        LoggerBold("Initialization error, see debug log for details");
                        return false;
                    }
                }
                else
                {
                    Logger("Unsupported protocol: " + CanParameters.Protocol.ToString());
                    return false;
                }
                this.MaxSendSize = MaxSendSize;
                this.MaxReceiveSize = MaxReceiveSize;
                this.Supports4X = Supports4X;
                this.LogDeviceType = LogDeviceType;

                this.Port.SetReadTimeout(AppSettings.TimeoutSerialPortRead);
                //this.Port.SetWriteTimeout(AppSettings.LoggerPortWriteTimeout);
                CanParameters = j2534Init;

                SetLoggingFilter();
                this.Connected = true;
                return true;
            }
            catch (Exception exception)
            {
                Logger("Unable to initalize " + this.ToString());
                Debug.WriteLine(exception.ToString());
                return false;
            }
        }

        /// <summary>
        /// Send a command to the device, confirm that we got the response we expect. 
        /// </summary>
        /// <remarks>
        /// This is primarily for use in the Initialize method, where we're talking to the 
        /// interface device rather than the PCM.
        /// </remarks>
        public bool SendAndVerify(string message, string expectedResponse)
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
            Debug.WriteLine("Elm line: " + serialStr.Data);
            return serialStr;
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
            Debug.WriteLine("TX: " + request + ", time: " + DateTime.Now.ToString("HH.mm.ss.ffff"));
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
                    return new SerialString("", 0, false);
                }
            }
            catch (TimeoutException)
            {
                return new SerialString("", 0, false);
            }
        }
        /// <summary>
        /// Process responses from the ELM/ST devices.
        /// </summary>
        public bool ProcessResponse(SerialString rawResponse, string context, bool allowEmpty = false)
        {
            if (rawResponse.Data == "OK")
            {
                return true;
            }

            if (rawResponse.Prompt && (rawResponse.Data.Length < 3 || rawResponse.Data.Contains("STOPPED")))
            {
                //We have received prompt
                Debug.WriteLine("Processresponse with prompt: " + rawResponse.Data);
                OBDMessage response = new OBDMessage(new byte[0]);
                //response.ElmLine = rawResponse.Data;
                //if (rawResponse.Data.StartsWith("6C") || rawResponse.Data.StartsWith("8C"))
                //  response = new OBDMessage(rawResponse.Data.ToBytes());
                response.TimeStamp = rawResponse.TimeStamp;
                response.ElmPrompt = true;
                this.Enqueue(response, true);
                return false;
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



            // We sent successfully, but the PCM didn't reply immediately.
            if (rawResponse.Data == "NO DATA")
            {
                this.SendRequest("00"); //Ask for data with dummy request
                return true;
            }

            string[] segments = rawResponse.Data.Replace("BUFFER FULL", "").Replace("OUT OF MEMORY", "").Split('<');
            foreach (string segment in segments)
            {
                if (segment.IsHex())
                {
                    int s = 0;
                    int totalLength = int.MaxValue;
                    List<byte> isotpData = new List<byte>();
                    isotpData.AddRange(new byte[] { 0, 0, 0, 0 }); //Reserve space for canid
                    string[] hexResponses = segment.Split(' ');
                    foreach (string singleHexResponse in hexResponses)
                    {
                        byte[] deviceResponseBytes = singleHexResponse.ToBytes();
                        if (this.Protocol == J2534DotNet.ProtocolID.ISO15765)
                        {
                            deviceResponseBytes = ("0" + singleHexResponse).ToBytes();
                            uint canid = (uint)((deviceResponseBytes[0] << 8) | deviceResponseBytes[1]);
                            //if (singleHexResponse.StartsWith(ecuStr) || deviceResponseBytes.Length > 12)
                            if (IsoTP.isISO15765Frame(canid, deviceResponseBytes.Skip(2).ToArray(), (byte)(deviceResponseBytes.Length - 2)))
                            {
                                //Parse ISOTP message, first add CanId:
                                if (deviceResponseBytes.Length < 3) continue;
                                byte pci = deviceResponseBytes[2];
                                if ((pci & 0xF0) == 0x10)  // First frame
                                {
                                    totalLength = deviceResponseBytes[3];
                                    Debug.WriteLine($"[First Frame]: {singleHexResponse}, Total data length: {totalLength} ");
                                    for (int i = 4; i < deviceResponseBytes.Length; i++) isotpData.Add(deviceResponseBytes[i]);
                                    continue;
                                }
                                else if ((pci & 0xF0) == 0x20)  // Consecutive frame
                                {
                                    Debug.WriteLine("[Next Frame]: " + singleHexResponse);
                                    for (int i = 3; i < deviceResponseBytes.Length && (isotpData.Count - 4) < totalLength; i++) isotpData.Add(deviceResponseBytes[i]);
                                    if ((isotpData.Count - 4) < totalLength)
                                    {
                                        //More data coming
                                        continue;
                                    }
                                }
                                else if ((pci & 0xF0) == 0) // Single frame
                                {
                                    Debug.WriteLine("[Single Frame]: " + singleHexResponse);
                                    totalLength = pci & 0x0F;
                                    for (int i = 3; i < deviceResponseBytes.Length && (isotpData.Count - 4) < totalLength; i++) isotpData.Add(deviceResponseBytes[i]);
                                }
                                isotpData[2] = deviceResponseBytes[0];
                                isotpData[3] = deviceResponseBytes[1];
                                deviceResponseBytes = isotpData.ToArray();
                            }
                            else
                            {
                                //Single frame/full isotp message
                                deviceResponseBytes = ("00000" + singleHexResponse).ToBytes();
                            }
                        }
                        if (deviceResponseBytes.Length > 0 && CanParameters.Protocol == J2534DotNet.ProtocolID.J1850VPW)
                        {
                            Array.Resize(ref deviceResponseBytes, deviceResponseBytes.Length - 1); // remove checksum byte
                        }
                        Debug.WriteLine("RX: " + deviceResponseBytes.ToHex());
                        //Debug.WriteLine("Timestamp " + s.ToString() +": "+ rawResponse.TimeStamps[s].ToString());
                        OBDMessage response = new OBDMessage(deviceResponseBytes);
                        response.TimeStamp = rawResponse.TimeStamps[s];
                        response.ElmPrompt = rawResponse.Prompt;
                        this.Enqueue(response, true);
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
        /// <summary>
        /// Separate the message into header and payload.
        /// </summary>
        private void ParseMessage(byte[] messageBytes, out string header, out string payload)
        {
            // The incoming byte array needs to separated into header and payload portions,
            // which are sent separately.
            string hexRequest = messageBytes.ToHex();
            if (CanParameters.Protocol == J2534DotNet.ProtocolID.J1850VPW)
            {
                if (hexRequest.Length > 9)
                {
                    header = hexRequest.Substring(0, 9);
                    payload = hexRequest.Substring(9);
                }
                else
                {
                    header = "";
                    payload = "";
                }
            }
            else
            {
                ushort devid = (ushort)(messageBytes[2] << 8 | messageBytes[3]);
                header = devid.ToString("X");
                payload = hexRequest.Substring(12);
            }
        }

        public static List<string> BuildIsoTpFrames(byte[] fullMessage)
        {
            List<string> frames = new List<string>();
            byte[] message = fullMessage.Skip(4).ToArray();
            Debug.WriteLine("IsoTP message, length: " + message.Length.ToString());
            if (message.Length <= 7)
            {
                // Single Frame
                byte[] frame = new byte[8];
                frame[0] = (byte)(0x00 | message.Length); // SF | length
                Array.Copy(message, 0, frame, 1, message.Length);
                for (int i = 1 + message.Length; i < 8; i++)
                    frame[i] = 0x0; // AA padding?
                string frameStr = frame.ToHex();
                frames.Add(frameStr);
            }
            else
            {
                // Multi-Frame
                Debug.WriteLine("Multi-frame");
                int totalLength = message.Length;
                int numConsecutiveFrames = (totalLength - 6 + 6) / 7;

                // First Frame
                byte[] firstFrame = new byte[8];
                firstFrame[0] = (byte)(0x10 | ((totalLength >> 8) & 0x0F)); // FF | high nibble of length
                firstFrame[1] = (byte)(totalLength & 0xFF); // low byte of length
                Array.Copy(message, 0, firstFrame, 2, 6);
                string frameStr = firstFrame.ToHex();
                frames.Add(frameStr);

                // Consecutive Frames
                int offset = 6;
                for (int frameNumber = 1; offset < totalLength; frameNumber++)
                {
                    int chunkSize = Math.Min(7, totalLength - offset);
                    //byte[] cf = new byte[chunkSize +1];
                    byte[] cf = new byte[8];
                    cf[0] = (byte)(0x20 | (frameNumber & 0x0F)); // CF | sequence number
                    Array.Copy(message, offset, cf, 1, chunkSize);
                    for (int j = 1 + chunkSize; j < 8; j++)
                        cf[j] = 0x0; // AA padding ??
                    string cfStr = cf.ToHex();
                    frames.Add(cfStr);
                    offset += chunkSize;
                }
            }

            return frames;
        }

        /// <summary>
        /// Send a message, do not expect a response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            string header;
            string payload;
            byte[] messageBytes = message.GetBytes();
            this.ParseMessage(messageBytes, out header, out payload);
            bool getResponse = true;
            if (responses < 1)
                getResponse = false;

            Port.DiscardBuffers();
            this.MessageSent(message);
            if (header != currentHeader)
            {
                SerialString setHeaderResponse = this.SendRequest("AT SH " + header, getResponse);
                Debug.WriteLine("Set header response (1): " + setHeaderResponse.Data + ", time: " + DateTime.Now.ToString("HH.mm.ss.ffff"));

                for (int retry = 0; retry < 5; retry++)
                {
                    if (setHeaderResponse.Data == "OK")
                    {
                        break;
                    }
                    else
                    {
                        // Does it help to retry once?
                        setHeaderResponse = this.SendRequest("AT SH " + header);
                        Debug.WriteLine("Set header response (" + (retry + 2).ToString() + "): " + setHeaderResponse.Data + ", time: " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                    }
                }
                if (!this.ProcessResponse(setHeaderResponse, "set-header command", !getResponse))
                {
                    return false;
                }
                this.currentHeader = header;
            }

            if (CanParameters.Protocol == ProtocolID.J1850VPW)
            {
                payload = payload.Replace(" ", "");
                Debug.WriteLine("TX: " + payload + ", time: " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                SerialString sendMessageResponse = this.SendRequest(payload + " ", getResponse);
                if (!this.ProcessResponse(sendMessageResponse, "message content", !getResponse))
                {
                    return false;
                }
            }
            else if (Elm21 && CanParameters.Protocol == J2534DotNet.ProtocolID.ISO15765)
            {
                payload = payload.Replace(" ", "");
                Debug.WriteLine("TX: " + payload + ", time: " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                SerialString sendMessageResponse = this.SendRequest(payload + " ", getResponse);
                if (!this.ProcessResponse(sendMessageResponse, "message content", !getResponse))
                {
                    return false;
                }
            }
            else if (CanParameters.Protocol == J2534DotNet.ProtocolID.ISO15765)
            {
                List<string> frames = BuildIsoTpFrames(messageBytes);

                SerialString sendMessageResponse = new SerialString("", DateTime.Now.Ticks, false);
                foreach (string msg in frames)
                {
                    payload = msg.Replace(" ", "");
                    //Debug.WriteLine("TX: " + payload + ", time: " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                    sendMessageResponse = this.SendRequest(payload, true);
                }
                if (!this.ProcessResponse(sendMessageResponse, "message content", !getResponse))
                {
                    return false;
                }
            }
            else
            {
                Logger("Unsupported protocol: " + CanParameters.Protocol.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the time required for the given scenario.
        /// </summary>
        public int GetTimeoutMilliseconds(TimeoutScenario scenario, VpwSpeed speed)
        {
            int milliseconds;

            if (speed == VpwSpeed.Standard)
            {
                switch (scenario)
                {
                    case TimeoutScenario.Minimum:
                        milliseconds = 0;
                        break;

                    case TimeoutScenario.ReadProperty:
                        milliseconds = 25;
                        break;

                    case TimeoutScenario.ReadCrc:
                        milliseconds = 100;
                        break;

                    case TimeoutScenario.ReadMemoryBlock:
                        milliseconds = 300;
                        break;

                    case TimeoutScenario.EraseMemoryBlock:
                        milliseconds = 1000;
                        break;

                    case TimeoutScenario.WriteMemoryBlock:
                        milliseconds = 250;
                        break;

                    case TimeoutScenario.SendKernel:
                        milliseconds = 50;
                        break;

                    case TimeoutScenario.DataLogging1:
                        milliseconds = 25;
                        break;

                    case TimeoutScenario.DataLogging2:
                        milliseconds = 40;
                        break;

                    case TimeoutScenario.DataLogging3:
                        milliseconds = 60;
                        break;

                    case TimeoutScenario.DataLogging4:
                        milliseconds = 2000;
                        break;

                    case TimeoutScenario.Maximum:
                        return 1020;

                    default:
                        throw new NotImplementedException("Unknown timeout scenario " + scenario);
                }
            }
            else
            {
                switch (scenario)
                {
                    case TimeoutScenario.Minimum:
                        milliseconds = 0;
                        break;

                    // The app doesn't currently do this in 4X mode, so this is only a guess.
                    case TimeoutScenario.ReadProperty:
                        milliseconds = 12;
                        break;

                    case TimeoutScenario.ReadCrc:
                        milliseconds = 100;
                        break;

                    case TimeoutScenario.ReadMemoryBlock:
                        milliseconds = 50;
                        break;

                    case TimeoutScenario.EraseMemoryBlock:
                        milliseconds = 1000;
                        break;

                    case TimeoutScenario.WriteMemoryBlock:
                        milliseconds = 170;
                        break;

                    case TimeoutScenario.SendKernel:
                        milliseconds = 10;
                        break;

                    case TimeoutScenario.DataLogging1:
                        milliseconds = 7;
                        break;

                    case TimeoutScenario.DataLogging2:
                        milliseconds = 10;
                        break;

                    case TimeoutScenario.DataLogging3:
                        milliseconds = 15;
                        break;

                    case TimeoutScenario.DataLogging4:
                        milliseconds = 2000;
                        break;

                    case TimeoutScenario.Maximum:
                        return 1020;

                    default:
                        throw new NotImplementedException("Unknown timeout scenario " + scenario);
                }
            }

            return milliseconds;
        }

        public override bool SetProgramminVoltage(PinNumber pinNumber, uint voltage)
        {
            LoggerBold("SetProgramminVoltage not implemented for " + DeviceType);
            return false;
        }
        public override bool AddToFunctMsgLookupTable(byte[] FuncAddr, bool secondary)
        {
            LoggerBold("AddToFunctMsgLookupTable not implemented for " + DeviceType);
            return false;
        }
        public override bool DeleteFromFunctMsgLookupTable(byte[] FuncAddr, bool secondary)
        {
            LoggerBold("DeleteFromFunctMsgLookupTable not implemented for " + DeviceType);
            return false;
        }
        public override bool ClearFunctMsgLookupTable(bool secondary)
        {
            LoggerBold("ClearFunctMsgLookupTable not implemented for " + DeviceType);
            return false;
        }
        public override bool SetJ2534Configs(string Configs, bool secondary)
        {
            LoggerBold("SetJ2534Configs not implemented for " + DeviceType);
            return false;
        }
        public override bool StartPeriodicMsg(string PeriodicMsg, bool secondary)
        {
            LoggerBold("StartPeriodicMsg not implemented for " + DeviceType);
            return false;
        }
        public override bool StopPeriodicMsg(string PeriodicMsg, bool secondary)
        {
            LoggerBold("StopPeriodicMsg not implemented for " + DeviceType);
            return false;
        }
        public override bool ClearPeriodicMsg(bool secondary)
        {
            LoggerBold("ClearPeriodicMsg not implemented for " + DeviceType);
            return false;
        }

        private bool SharedInitialization(J2534InitParameters j2534Init)
        {
            // This will only be used for device-independent initialization.
            ElmDeviceImplementation sharedImplementation = new ElmDeviceImplementation(null, null, this.Port, this.MessageSent);
            return sharedImplementation.Initialize();
        }

        public override void SetWriteTimeout(int timeout)
        {
            //Port.SetWriteTimeout(timeout + AppSettings.LoggerPortWriteTimeout);
            //Port.SetWriteTimeout(timeout + 1000);
            //Port.SetTimeout(timeout + 1000);
            SetWriteTimeoutMilliseconds(timeout);
        }

        public override void SetReadTimeout(int timeout)
        {
            //Port.SetTimeout(timeout + AppSettings.LoggerPortReadTimeout);
            //Port.SetTimeout(timeout + 1000);
            SetTimeoutMilliseconds(timeout);
            //SetReadTimeoutMilliseconds(timeout);
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
        /// Set the amount of time that we'll wait for a message to arrive.
        /// </summary>
        public override TimeoutScenario SetTimeout(TimeoutScenario scenario)
        {
            if (this.currentTimeoutScenario == scenario)
            {
                return this.currentTimeoutScenario;
            }

            int milliseconds = GetTimeoutMilliseconds(scenario, this.Speed);
            
            Debug.WriteLine("Setting timeout for " + scenario + ", " + milliseconds.ToString() + " ms.");

            // The port timeout needs to be considerably longer than the device timeout,
            // otherwise you get "STOPPED" or "NO DATA" somewhat randomly. (I mostly saw
            // this when sending the tool-present messages, but that might be coincidence.)
            //
            // Consider increasing if STOPPED / NO DATA is still a problem. 
            this.Port.SetTimeout(milliseconds + AppSettings.TimeoutSerialPortRead);

            // This code is so problematic that I've left it here as a warning. The app is
            // unable to receive the response to the erase command if this code is enabled.
            // 
            //if (this.implementation is ScanToolDeviceImplementation)
            //{
            //    TimeoutScenario old = this.currentTimeoutScenario;
            //    this.currentTimeoutScenario = scenario;
            //    return old;
            //}

            // I briefly tried hard-coding timeout values for the AT ST command,
            // but that's a recipe for failure. If the port timeout is shorter
            // than the device timeout, reads will consistently fail.
            SetTimeoutMilliseconds(milliseconds);
            
            TimeoutScenario result = this.currentTimeoutScenario;
            this.currentTimeoutScenario = scenario;
            TimeoutScenario = scenario;
            return result;
        }

        public override void ReceiveBufferedMessages()
        {
        }

        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        /// <returns></returns>
        public override void Receive(int NumMessages, bool WaitForTimeout)
        {
            try
            {
                for (int m = 0; m < NumMessages; m++)
                {
                    if (WaitForTimeout || Port.GetReceiveQueueSize() > 3)
                    {
                        SerialString response = this.ReadELMLine(false);
                        Debug.WriteLine("Elm line: " + response.Data);
                        this.ProcessResponse(response, "receive");
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("Timeout during receive.");
            }
        }

        /// <summary>
        /// Set the interface to low (false) or high (true) speed
        /// </summary>
        /// <remarks>
        /// The caller must also tell the PCM to switch speeds
        /// </remarks>
        protected override bool SetVpwSpeedInternal(VpwSpeed newSpeed)
        {
            if (newSpeed == VpwSpeed.Standard)
            {
                Debug.WriteLine("AllPro setting VPW 1X");
                if (!SendAndVerify("AT VPW1", "OK"))
                    return false;
            }
            else
            {
                Debug.WriteLine("AllPro setting VPW 4X");
                if (!SendAndVerify("AT VPW4", "OK"))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Discard any messages in the recevied-message queue.
        /// </summary>
        public override void ClearMessageBuffer()
        {
            this.Port.DiscardBuffers();
        }

        /// <summary>
        /// Reads and filters a line from the device, returns it as a string
        /// </summary>
        /// <remarks>
        /// Strips Non Printable, >, and Line Feeds. Converts Carriage Return to Space. Strips leading and trailing whitespace.
        /// </remarks>
        private SerialString ReadELMLine()
        {           
            // (MaxReceiveSize * 2) + 2 for Checksum + longest possible prompt. Minimum prompt 2 CR + 1 Prompt, +2 extra
            int maxPayload = (MaxReceiveSize * 2) + 7;

            // A buffer to receive a single byte.
            SerialByte b = new SerialByte(1);

            bool Prompt = false;

            // Use StringBuilder to collect the bytes.
            StringBuilder builtString = new StringBuilder();
            //for (int i = 0; i < maxPayload; i++)
            while (true)
            {
                // Receive a single byte.
                Port.Receive(b, 0, 1);

                // Is it the prompt '>'.
                if (b.Data[0] == '>' || b.Data[0] == '?')
                {
                    // Prompt found, we're done.
                    Prompt = true;
                    break;
                }

                // Is it a CR
                if (b.Data[0] == 13)
                {
                    // CR found, replace with space.
                    break;
                    //b.Data[0] = 32;
                }

                // Printable characters only.
                if (b.Data[0] > 32 && b.Data[0] <= 126)
                {
                    // Append it to builtString.
                    builtString.Append((char)b.Data[0]);
                }
            }

            // Convert to string, trim and return
            SerialString serialStr = new SerialString(builtString.ToString().Trim(), b.TimeStamp,Prompt);
            return serialStr;
        }

        /// <summary>
        /// Send a request in string form, wait for a response (for init)
        /// </summary>
        /// <remarks>
        /// The API for this method (sending a string, returning a string) matches
        /// the way that we need to communicate with ELM and STN devices for setup
        /// </remarks>
        public string SendRequest(string request, out bool Prompt)
        {
            Debug.WriteLine("TX: " + request);
            Prompt = false;
            try
            {
                Port.Send(Encoding.ASCII.GetBytes(request + " \r"));
                Thread.Sleep(100);                
                SerialString response = ReadELMLine();
                Debug.WriteLine("Elm response: " + response.Data);
                return response.Data;
            }
            catch (TimeoutException)
            {
                return string.Empty;
            }
        }

        public override bool SetLoggingFilter()
        {
            if (this.CurrentFilter == FilterMode.Logging)
                return true;
            Debug.WriteLine("Set logging filter");
            //Initialize();
            SendAndVerify("AT L0", "OK"); //Disable new line characters between commands/messages
            this.CurrentFilter = FilterMode.Logging;
            if (datalogger.useVPWFilters && Protocol == ProtocolID.J1850VPW)
            {
                return SendAndVerify("AT SR " + DeviceId.Tool.ToString("X2"), "OK");
            }
            else
            {
                return true;
            }
        }
        public override bool SetAnalyzerFilter()
        {
            if (this.Protocol == ProtocolID.ISO15765)
            {
                Debug.WriteLine("Keep filter, avoid BUFFER FULL");
                //SendAndVerify("AT CRA1BA", "OK"); //Receive only passive pid 0x1BA
                //SendAndVerify("AT ST FF", "OK");
                //Port.Send(Encoding.ASCII.GetBytes("00\r")); //Begin monitoring bus traffic 
            }
            else if (this.Protocol == ProtocolID.J1850VPW)
            {
                Debug.WriteLine("Setting analyzer filter");
                SendAndVerify("AT L1", "OK"); //Enable new line characters between commands/messages
                SendAndVerify("AT H1", "OK");
                SendAndVerify("AT AR", "OK"); // Set receive filter Off
                Thread.Sleep(200);
                Port.Send(Encoding.ASCII.GetBytes("ATMA \r")); //Begin monitoring bus traffic 
            }
            else
            {
                Logger("Unsupported protocol: " + CanParameters.Protocol.ToString());
                return false;
            }

            this.CurrentFilter = FilterMode.Analyzer;
            return true;
        }
        public override bool RemoveFilters(int[] filterIds)
        {
            Debug.WriteLine("Removing filters");
            this.CurrentFilter = FilterMode.None;
            return SendAndVerify("AT AR", "OK"); // Set receive filter Off
        }
        public override bool RemoveFilters2(int[] filterIds)
        {
            Debug.WriteLine("RemoveFilters2 not implemented");
            return true;
        }

    }
}