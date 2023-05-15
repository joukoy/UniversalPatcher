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
    /// This class encapsulates all code that is unique to the ScanTool MX interface.
    /// </summary>
    public class ScanToolDeviceImplementation : ElmDeviceImplementation
    {
        /// <summary>
        /// Device type for use in the Device Picker dialog box, and for internal comparisons.
        /// </summary>
        public const string DeviceType = "ObdLink ScanTool";

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScanToolDeviceImplementation(
            Action<OBDMessage, bool> enqueue,
            Func<int> getReceivedMessageCount,
            IPort port,
            Action<OBDMessage> MessageSent) : 
            base(enqueue, getReceivedMessageCount, port, MessageSent)
        {
            // Both of these numbers could be slightly larger, but round numbers are easier to work with,
            // and these are only used with the Scantool SX interface anyhow. If we detect an AllPro
            // adapter we'll overwrite these values, see the Initialize method below.

            // Please keep the left side easy to read in hex. Then add 12 bytes for VPW overhead.
            // The STPX approach to sending messages should work with larger buffers, but when I tried
            // with my SX, it didn't work. That might only work with the MX (bluetooth version).
            this.MaxSendSize = 192 + 12;

            // The ScanTool SX will download 512kb in roughly 30 minutes at 500 bytes per read.
            // ScanTool reliability suffers at 508 bytes or more, so we're going with a number
            // that's round in base 10 rather than in base 2.
            this.MaxReceiveSize = 500 + 12;

            // This would need a firmware upgrade at the very least, and likely isn't even possible 
            // with current hardware.
            this.Supports4X = false;

            this.LogDeviceType = DataLogger.LoggingDevType.Obdlink;
        }

        /// <summary>
        /// This string is what will appear in the drop-down list in the UI.
        /// </summary>
        public override string GetDeviceType()
        {
            return DeviceType;
        }

        /// <summary>
        /// Confirm that we're actually connected to the right device, and initialize it.
        /// </summary>
        public override bool Initialize()
        {
            Debug.WriteLine("Determining whether " + this.ToString() + " is connected.");
            
            try
            {
                string stID = this.SendRequest("ST I").Data;                 // Identify (ScanTool.net)
                if (stID == "?" || string.IsNullOrEmpty(stID))
                {
                    Debug.WriteLine("This is not a ScanTool device.");
                    return false;
                }

                Logger("ScanTool device ID: " + stID);

                // The following table was provided by ScanTool.net Support - ticket #33419
                // Device                     Max Msg Size    Max Tested Baudrate
                // STN1110                    2k              2 Mbps *
                // STN1130 (OBDLink SX)       2k              2 Mbps *
                // STN1150 (OBDLink MX v1)    2k              N/A
                // STN1151 (OBDLink MX v2)    2k              N/A
                // STN1155 (OBDLink LX)       2k              N/A
                // STN1170                    2k              2 Mbps *
                // STN2100                    4k              2 Mbps
                // STN2120                    4k              2 Mbps
                // STN2230 (OBDLink EX)       4k              N/A
                // STN2255 (OBDLink MX+)      4k              N/A
                //
                // * With character echo off (ATE 0), 1 Mbps with character echo on (ATE 1)

                // Here 1024 bytes = 2048 ASCII hex bytes, without spaces
                if (stID.Contains("STN1100") || // SparkFun OBD-II UART
                    stID.Contains("STN1110") || // SX
                    stID.Contains("STN1130") || // SX
                    stID.Contains("STN1150") || // MX v1
                    stID.Contains("STN1151") || // MX v2
                    stID.Contains("STN1155") || // LX
                    stID.Contains("STN1170") || //
                    stID.Contains("STN2100") || //
                    stID.Contains("STN2120") || //
                    stID.Contains("STN2230") || // EX
                    stID.Contains("STN2255"))   // MX+
                {
                    // Testing shows larger packet sizes do not equate to faster transfers
                    this.MaxSendSize = 1024 + 12;
                    this.MaxReceiveSize = 1024 + 12;
                }
                else
                {
                    Logger("This ScanTool device is not supported.");
                    Logger("Please check universalpatcher.net to ensure that you have the latest release.");
                    Logger("We're going to default to very small packet sizes, which will make everything slow, but at least it'll probably work.");
                    this.MaxSendSize = 128 + 12;
                    this.MaxReceiveSize = 128 + 12;
                }

                // Setting timeout to a large value. Since we use STPX commands,
                // the device will stop listening when it receives the expected
                // number of responses, rather than waiting for the timeout.
                Debug.WriteLine(this.SendRequest("STPTO 3000"));

            }
            catch (Exception exception)
            {
                Debug.WriteLine("Unable to initalize " + this.ToString());
                Debug.WriteLine(exception.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the time required for the given scenario.
        /// </summary>
        public override int GetTimeoutMilliseconds(TimeoutScenario scenario, VpwSpeed speed)
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
                        milliseconds = 250;
                        break;

                    case TimeoutScenario.EraseMemoryBlock:
                        milliseconds = 3000;
                        break;

                    case TimeoutScenario.WriteMemoryBlock:
                        milliseconds = 140; // 125 works, added some for safety
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
                        milliseconds = 4500;
                        break;

                    case TimeoutScenario.Maximum:
                        return 1020;

                    default:
                        Debug.WriteLine("Unknown timeout scenario " + scenario);
                        return 1020;
                        //throw new NotImplementedException("Unknown timeout scenario " + scenario);
                }
            }
            else
            {
                throw new NotImplementedException("Since when did ScanTool devices support 4x?");
            }

            return milliseconds;
        }

        /// <summary>
        /// Set the timeout to the device. If this is set too low, the device
        /// will return 'No Data'. The ST Equivalent timeout command doesn't have
        /// the same 1020 millisecond limit since it takes an integer milliseconds
        /// as a paramter.
        /// </summary>
        public override bool SetTimeoutMilliseconds(int milliseconds)
        {
            return this.SendAndVerify("STPTO " + milliseconds, "OK");
        }


        /// <summary>
        /// Send a message, do not expect a response.
        /// </summary>
        /// <remarks>
        /// This initially used standard ELM commands, however the ScanTool family
        /// of devices supports an "STPX" command that simplifies things a lot.
        /// Timeout adjustements are no longer needed, and longer packets are supported.
        /// </remarks>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            try
            {
                Port.DiscardBuffers();

                byte[] messageBytes = message.GetBytes();

                StringBuilder builder = new StringBuilder();
                builder.Append("STPX H:");
                builder.Append(messageBytes[0].ToString("X2"));
                builder.Append(messageBytes[1].ToString("X2"));
                builder.Append(messageBytes[2].ToString("X2"));

                /*                int responses;
                                switch (this.TimeoutScenario)
                                {
                                    case TimeoutScenario.DataLogging4:
                                        responses = 4;
                                        break;

                                    case TimeoutScenario.DataLogging3:
                                        responses = 3;
                                        break;

                                    case TimeoutScenario.DataLogging2:
                                        responses = 2;
                                        break;

                                    default:
                                        responses = 1;
                                        break;
                                }
                */
                // Special case for tool-present broadcast messages.
                // TODO: Create a new TimeoutScenario value, maybe call it "TransmitOnly" or something like that.
                /*                if (Utility.CompareArrays(messageBytes, PcmLogger.priority, 0xFE, 0xF0, 0x3F))
                                {
                                    responses = 0;
                                    Debug.WriteLine("ELM tester present");
                                }
                */

                builder.AppendFormat(", T:{0}", WriteTimeout.ToString());

                bool getResponse = true;
                if (responses < 1)
                {
                    getResponse = false;
                    builder.AppendFormat(", R:{0}", Math.Abs(responses));
                    responses = 0;
                }
                else
                {
                    builder.AppendFormat(", R:{0}", responses);
                }
                //builder.AppendFormat(", R:50");

                this.MessageSent(message);

                if (messageBytes.Length < 200)
                {
                    // Short messages can be sent with a single write to the ScanTool.
                    builder.Append(", D:");
                    for (int index = 3; index < messageBytes.Length; index++)
                    {
                        builder.Append(messageBytes[index].ToString("X2"));
                    }
                    bool sendOK = false;
                    for (int retry = 1; retry <= 5; retry++)
                    {
                        SerialString dataResponse = this.SendRequest(builder.ToString(), getResponse);
                        Debug.WriteLine("Dataresponse: " + dataResponse.Data);
                        if (this.ProcessResponse(dataResponse, "STPX with data", allowEmpty: responses == 0))
                        {
                            sendOK = true;
                            break;
                        }
                        else
                        {
                            Debug.WriteLine("Unexpected response to STPX with data: " + dataResponse.Data + ", Retry: " + retry.ToString());
                        }
                    }
                    if (!sendOK)
                    {
                        Logger("Unexpected response to STPX with data");
                        return false;
                    }
                }
                else
                {
                    // Long messages need to be sent in two steps: first the STPX command, then the data payload.
                    builder.Append(", L:");
                    int dataLength = messageBytes.Length - 3;
                    builder.Append(dataLength.ToString());
                    string header = builder.ToString();
                    for (int attempt = 1; attempt <= 5; attempt++)
                    {
                        SerialString headerResponse = this.SendRequest(header);
                        if (headerResponse.Data != "DATA")
                        {
                            Logger("Unexpected response to STPX header: " + headerResponse);
                            continue;
                        }

                        break;
                    }

                    builder = new StringBuilder();
                    for (int index = 3; index < messageBytes.Length; index++)
                    {
                        builder.Append(messageBytes[index].ToString("X2"));
                    }

                    string data = builder.ToString();
                    SerialString dataResponse = this.SendRequest(data);

                    if (!this.ProcessResponse(dataResponse, "STPX payload", responses == 0))
                    {
                        Logger("Unexpected response to STPX payload: " + dataResponse.Data);
                        return false;
                    }
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// Borrowed from the AllPro class just for testing. Should be removed after STPX is working.
        /// </summary>
        private void ParseMessage(byte[] messageBytes, out string header, out string payload)
        {
            // The incoming byte array needs to separated into header and payload portions,
            // which are sent separately.
            string hexRequest = messageBytes.ToHex();
            header = hexRequest.Substring(0, 9);
            payload = hexRequest.Substring(9);
        }

        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        public override void Receive(bool WaitForTimeout)
        {
            try
            {
                if (WaitForTimeout || Port.GetReceiveQueueSize() > 3)
                {
                    SerialString response = this.ReadELMLine(false);
                    //Debug.WriteLine("Elm: " + response.Data);
                    this.ProcessResponse(response, "receive");
                }
                if (this.getReceivedMessageCount() == 0)
                {
                   // this.ReceiveViaMonitorMode();
                }
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("Timeout during receive.");
                // this.ReceiveViaMonitorMode();
            }
        }

        /// <summary>
        /// This doesn't actually work yet, but I like the idea...
        /// </summary>
        private void ReceiveViaMonitorMode()
        {
            try
            {
                SerialString monitorResponse = this.SendRequest("AT MA");
                Debug.WriteLine("Response to AT MA 1: " + monitorResponse);

                if (monitorResponse.Data != ">?")
                {
                    SerialString response = this.ReadELMLine(true);
                    this.ProcessResponse(monitorResponse, "receive via monitor");
                }
            }
            catch(TimeoutException)
            {
                Debug.WriteLine("Timeout during receive via monitor mode.");
            }
            finally
            { 
                SerialString stopMonitorResponse = this.SendRequest("AT MA");
                Debug.WriteLine("Response to AT MA 2: " + stopMonitorResponse.Data);
            }
        }
    }
}