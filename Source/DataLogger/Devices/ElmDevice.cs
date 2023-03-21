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
    public class ElmDevice : SerialDevice
    {
        /// <summary>
        /// Device type for use in the Device Picker dialog box, and for internal comparisons.
        /// </summary>
        public const string DeviceType = "Elm327, ObdLink or AllPro";

        /// <summary>
        /// This will be initalized after discovering which device is actually connected at the moment.
        /// </summary>
        private ElmDeviceImplementation implementation = null;


        /// <summary>
        /// Constructor.
        /// </summary>
        public ElmDevice(IPort port) : base(port)
        {
        }


        /// <summary>
        /// This string is what will appear in the drop-down list in the UI.
        /// </summary>
        public override string GetDeviceType()
        {
            if (this.implementation == null)
            {
                return DeviceType;
            }
            return this.implementation.GetDeviceType();
        }

        /// <summary>
        /// Use the related classes to discover which type of device is currently connected.
        /// </summary>
        public override bool Initialize(int BaudRate, J2534InitParameters j2534Init)
        {
            try
            {
                Debug.WriteLine("ElmDevice initialization starting.");

                SerialPortConfiguration configuration = new SerialPortConfiguration();
                configuration.BaudRate = BaudRate;
                //configuration.Timeout = 1200;
                configuration.Timeout = 2000;

                this.Port.OpenAsync(configuration);
                this.Port.DiscardBuffers();

                if (!this.SharedInitialization())
                {
                    return false;
                }

                AllProDeviceImplementation allProDevice = new AllProDeviceImplementation(
                    this.Enqueue, 
                    () => this.ReceivedMessageCount,
                    this.Port, this.MessageSent);

                if (allProDevice.Initialize())
                {
                    this.implementation = allProDevice;
                }
                else
                {
                    ScanToolDeviceImplementation scanToolDevice = new ScanToolDeviceImplementation(
                        this.Enqueue,
                        () => this.ReceivedMessageCount,
                        this.Port,
                        this.MessageSent);

                    if (scanToolDevice.Initialize())
                    {
                        this.implementation = scanToolDevice;
                    }
                    else
                    {
                        LegacyElmDeviceImplementation legacyElmDevice = new LegacyElmDeviceImplementation(
                        this.Enqueue,
                        () => this.ReceivedMessageCount,
                        this.Port,
                        this.MessageSent);
                        if (legacyElmDevice.Initialize())
                        {
                            this.implementation = legacyElmDevice;
                            Thread.Sleep(100); //Old, slow hw?
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                // These are shared by all ELM-based devices.
                if (!this.implementation.SendAndVerify("AT AL", "OK") ||               // Allow Long packets
                    !this.implementation.SendAndVerify("AT SP2", "OK") ||              // Set Protocol 2 (VPW)
                    !this.implementation.SendAndVerify("AT DP", "SAE J1850 VPW") ||    // Get Protocol (Verify VPW)
                    !this.implementation.SendAndVerify("AT AR", "OK") ||               // Turn Auto Receive on (default should be on anyway)
                    !this.implementation.SendAndVerify("AT AT0", "OK") ||              // Disable adaptive timeouts
                    //!this.implementation.SendAndVerify("AT SR " + DeviceId.Tool.ToString("X2"), "OK") || // Set receive filter to this tool ID
                    !this.implementation.SendAndVerify("AT H1", "OK") ||               // Send headers
                    !this.implementation.SendAndVerify("AT ST 20", "OK")               // Set timeout (will be adjusted later, too)                 
                    )
                {
                    return false;
                }

                this.MaxSendSize = this.implementation.MaxSendSize;
                this.MaxReceiveSize = this.implementation.MaxReceiveSize;
                this.Supports4X = this.implementation.Supports4X;
                this.LogDeviceType = this.implementation.LogDeviceType;

                //this.Port.SetReadTimeout(AppSettings.LoggerPortReadTimeout);
                //this.Port.SetWriteTimeout(AppSettings.LoggerPortWriteTimeout);

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

        private bool SharedInitialization()
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
            this.implementation.SetWriteTimeoutMilliseconds(timeout);
        }

        public override void SetReadTimeout(int timeout)
        {
            //Port.SetTimeout(timeout + AppSettings.LoggerPortReadTimeout);
            //Port.SetTimeout(timeout + 1000);
            this.implementation.SetTimeoutMilliseconds(timeout);
            //this.implementation.SetReadTimeoutMilliseconds(timeout);
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

            int milliseconds = this.implementation.GetTimeoutMilliseconds(scenario, this.Speed);
            
            Debug.WriteLine("Setting timeout for " + scenario + ", " + milliseconds.ToString() + " ms.");

            // The port timeout needs to be considerably longer than the device timeout,
            // otherwise you get "STOPPED" or "NO DATA" somewhat randomly. (I mostly saw
            // this when sending the tool-present messages, but that might be coincidence.)
            //
            // Consider increasing if STOPPED / NO DATA is still a problem. 
            this.Port.SetTimeout(milliseconds + AppSettings.TimeoutPortRead);

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
            this.implementation.SetTimeoutMilliseconds(milliseconds);
            
            TimeoutScenario result = this.currentTimeoutScenario;
            this.currentTimeoutScenario = scenario;
            this.implementation.TimeoutScenario = scenario;
            return result;
        }

        /// <summary>
        /// Send a message, do not expect a response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            return this.implementation.SendMessage(message, responses);
        }


        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        /// <returns></returns>
        public override void Receive()
        {
            if (this.Port == null || !this.Port.PortOpen())
            {
                Debug.WriteLine("Port closed, disposing ELM device");
                OBDMessage oMsg = new OBDMessage(new byte[] { 0 });
                oMsg.Error = 0x99;
                this.Enqueue(oMsg);                
                return;
            }
            this.implementation.Receive();
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
                if (!this.implementation.SendAndVerify("AT VPW1", "OK"))
                    return false;
            }
            else
            {
                Debug.WriteLine("AllPro setting VPW 4X");
                if (!this.implementation.SendAndVerify("AT VPW4", "OK"))
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
            //this.implementation.Initialize();
            this.implementation.SendAndVerify("AT L0", "OK"); //Disable new line characters between commands/messages
            this.CurrentFilter = FilterMode.Logging;
            if (datalogger.useVPWFilters)
            {
                return this.implementation.SendAndVerify("AT SR " + DeviceId.Tool.ToString("X2"), "OK");
            }
            else
            {
                return true;
            }
        }
        public override bool SetAnalyzerFilter()
        {
            Debug.WriteLine("Setting analyzer filter");

            this.implementation.SendAndVerify("AT L1", "OK"); //Enable new line characters between commands/messages
            this.implementation.SendAndVerify("AT AR", "OK"); // Set receive filter Off
            this.implementation.SendAndVerify("AT H1", "OK"); 
            Thread.Sleep(100);

            //ClearMessageBuffer();
            //ClearMessageQueue();
            Thread.Sleep(100);
            Port.Send(Encoding.ASCII.GetBytes("ATMA \r")); //Begin monitoring bus traffic 

            this.CurrentFilter = FilterMode.Analyzer;
            return true;
        }
        public override bool RemoveFilters()
        {
            Debug.WriteLine("Removing filters");
            this.CurrentFilter = FilterMode.None;
            return this.implementation.SendAndVerify("AT AR", "OK"); // Set receive filter Off
        }

    }
}