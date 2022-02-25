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
    /// This class encapsulates all code that is unique to the Legacy Elm USB interface.
    /// </summary>
    public class LegacyElmDeviceImplementation : ElmDeviceImplementation
    {
        /// <summary>
        /// Device type for use in the Device Picker dialog box, and for internal comparisons.
        /// </summary>
        public const string DeviceType = "Elm327";

        /// <summary>
        /// The device can cache the message header to speed up serial communications. 
        /// To use that properly, we need to keep track of the cached header.
        /// </summary>
        private string currentHeader = "unset";

        /// <summary>
        /// Constructor.
        /// </summary>
        public LegacyElmDeviceImplementation(
            Action<OBDMessage> enqueue,
            Func<int> getRecievedMessageCount,
            IPort port) :
            base(enqueue, getRecievedMessageCount, port)
        {
            // Please keep the left side easy to read in hex. Then add 12 bytes for VPW overhead.
            this.MaxSendSize = 1024 + 12;
            this.MaxReceiveSize = 1024 + 12;
            this.Supports4X = true;
            this.LogDeviceType = DataLogger.LoggingDevType.Elm;
        }

        /// <summary>
        /// This string is what will appear in the drop-down list in the UI.
        /// </summary>
        public override string GetDeviceType()
        {
            return DeviceType;
        }

        /// <summary>
        /// Configure the device for use - and also confirm that the device is what we think it is.
        /// </summary>
        public override bool Initialize()
        {
            Debug.WriteLine("Initializing " + this.ToString());

            // We're going to reset the interface device, which means that it's going
            // to forgot what header the app previously told it to use. That requires
            // the app to forget what header the interface was told to use - that will
            // cause the app to send another set-header command later on.
            this.currentHeader = "header not yet set";

            try
            {
                //Logger("Elm self test result: " + this.SendRequest("AT #3"));  // self test
                Logger("Elm firmware: " + this.SendRequest("AT @1").Data);          // firmware check
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

        /// <summary>
        /// Send a message, do not expect a response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            byte[] messageBytes = message.GetBytes();
            string header;
            string payload;
            this.ParseMessage(messageBytes, out header, out payload);
            bool getResponse = true;
            if (responses < 1)
                getResponse = false;

            Port.DiscardBuffers();
            datalogger.LogDevice.MessageSent(message);
            if (header != this.currentHeader)
            {
                SerialString setHeaderResponse = this.SendRequest("AT SH " + header, getResponse);
                Debug.WriteLine("Set header response: " + setHeaderResponse);

/*                if (setHeaderResponse.Data != "OK")
                {
                    // Does it help to retry once?
                    setHeaderResponse = this.SendRequest("AT SH " + header);
                    Debug.WriteLine("Set header response: " + setHeaderResponse);
                }
*/
                if (!this.ProcessResponse(setHeaderResponse, "set-header command", !getResponse))
                {
                    return false;
                }

                this.currentHeader = header;
            }

            payload = payload.Replace(" ", "");
            //Debug.WriteLine("TX: " + payload);
            SerialString sendMessageResponse = this.SendRequest(payload + " ", getResponse);
            if (!this.ProcessResponse(sendMessageResponse, "message content", !getResponse))
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Separate the message into header and payload.
        /// </summary>
        private void ParseMessage(byte[] messageBytes, out string header, out string payload)
        {
            // The incoming byte array needs to separated into header and payload portions,
            // which are sent separately.
            string hexRequest = messageBytes.ToHex();
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

        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        /// <returns></returns>
        public override void Receive()
        {
            try
            {
                SerialString response = this.ReadELMLine(false);
                //Debug.WriteLine("Elm line: " + response + ", Prompt: " + Prompt);
                this.ProcessResponse(response, "receive");
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("Timeout during receive.");
            }
        }

    }
}