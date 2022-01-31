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
    /// This class encapsulates common code for ELM-derived devices, and also handles detecting the
    /// specific ELM device that is attached. After detecting the device is acts as a facade, with
    /// a device-specific class implementing device-specific functionality.
    /// </summary>
    public class ElmDevice : SerialDevice
    {
        /// <summary>
        /// Device type for use in the Device Picker dialog box, and for internal comparisons.
        /// </summary>
        public const string DeviceType = "ObdLink or AllPro";

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
        public override bool Initialize()
        {
            try
            {
                Debug.WriteLine("ElmDevice initialization starting.");

                SerialPortConfiguration configuration = new SerialPortConfiguration();
                configuration.BaudRate = 115200;
                configuration.Timeout = 1200;

                this.Port.OpenAsync(configuration);
                this.Port.DiscardBuffers();

                if (!this.SharedInitialization())
                {
                    return false;
                }

                AllProDeviceImplementation allProDevice = new AllProDeviceImplementation(
                    this.Enqueue, 
                    () => this.ReceivedMessageCount,
                    this.Port);

                if (allProDevice.Initialize())
                {
                    this.implementation = allProDevice;
                }
                else
                {
                    ScanToolDeviceImplementation scanToolDevice = new ScanToolDeviceImplementation(
                        this.Enqueue,
                        () => this.ReceivedMessageCount,
                        this.Port);

                    if (scanToolDevice.Initialize())
                    {
                        this.implementation = scanToolDevice;
                    }
                }

                // These are shared by all ELM-based devices.
                if (!this.implementation.SendAndVerify("AT AL", "OK") ||               // Allow Long packets
                    !this.implementation.SendAndVerify("AT SP2", "OK") ||              // Set Protocol 2 (VPW)
                    !this.implementation.SendAndVerify("AT DP", "SAE J1850 VPW") ||    // Get Protocol (Verify VPW)
                    !this.implementation.SendAndVerify("AT AR", "OK") ||               // Turn Auto Receive on (default should be on anyway)
                    !this.implementation.SendAndVerify("AT AT0", "OK") ||              // Disable adaptive timeouts
                    !this.implementation.SendAndVerify("AT SR " + DeviceId.Tool.ToString("X2"), "OK") || // Set receive filter to this tool ID
                    !this.implementation.SendAndVerify("AT H1", "OK") ||               // Send headers
                    !this.implementation.SendAndVerify("AT ST 20", "OK")               // Set timeout (will be adjusted later, too)                 
                    )
                {
                    return false;
                }

                this.MaxSendSize = this.implementation.MaxSendSize;
                this.MaxReceiveSize = this.implementation.MaxReceiveSize;
                this.Supports4X = this.implementation.Supports4X;
                return true;
            }
            catch (Exception exception)
            {
                Logger("Unable to initalize " + this.ToString());
                Debug.WriteLine(exception.ToString());
                return false;
            }
        }

        private bool SharedInitialization()
        {
            // This will only be used for device-independent initialization.
            ElmDeviceImplementation sharedImplementation = new ElmDeviceImplementation(null, null, this.Port);
            return sharedImplementation.Initialize();
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
            this.Port.SetTimeout(milliseconds + 1000);

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
        public override bool SendMessage(OBDMessage message)
        {
            return this.implementation.SendMessage(message);
        }

        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        /// <returns></returns>
        public override void Receive()
        {
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
    }
}