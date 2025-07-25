using J2534DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LoggerUtils;

namespace UniversalPatcher
{
    /// <summary>
    /// This class provides a way to test most of the app without any interface hardware.
    /// </summary>
    public class MockDevice : Device
    {
        /// <summary>
        /// Device ID string to use in the Device Picker form, and in interal device-type comparisons.
        /// </summary>
        public const string DeviceType = "Mock Serial Device";

        /// <summary>
        /// The mock port.
        /// </summary>
        private IPort port;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public MockDevice(IPort port) : base()
        {
            this.port = port;
        }

        /// <summary>
        /// Not actually necessary for this device type, but since we need to implement IDisposable...
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            this.Connected = false;
        }
        
        /// <summary>
        /// Initialize the device. It's just a no-op for this device type.
        /// </summary>
        public override bool Initialize(int Baudrate, J2534InitParameters j2534Init)
        {
            this.Connected = true;
            this.Protocol = j2534Init.Protocol;
            return true;
        }
        public override bool SetProgramminVoltage(PinNumber pinNumber, uint voltage)
        {
            Debug.WriteLine("SetProgramminVoltage not implemented for " + DeviceType);
            return false;
        }
        public override bool AddToFunctMsgLookupTable(byte[] FuncAddr, bool secondary)
        {
            return false;
        }
        public override bool DeleteFromFunctMsgLookupTable(byte[] FuncAddr, bool secondary)
        {
            return false;
        }
        public override bool ClearFunctMsgLookupTable(bool secondary)
        {
            return false;
        }
        public override bool SetJ2534Configs(string Configs, bool secondary)
        {
            Debug.WriteLine("SetJ2534Configs not implemented for " + DeviceType);
            return false;
        }
        public override bool StartPeriodicMsg(string PeriodicMsg, bool secondary)
        {
            Debug.WriteLine("StartPeriodicMsg not implemented for " + DeviceType);
            return false;
        }
        public override bool StopPeriodicMsg(string PeriodicMsg, bool secondary)
        {
            Debug.WriteLine("StopPeriodicMsg not implemented for " + DeviceType);
            return false;
        }
        public override bool ClearPeriodicMsg(bool secondary)
        {
            Debug.WriteLine("ClearPeriodicMsg not implemented for " + DeviceType);
            return false;
        }


        /// <summary>
        /// Not needed.
        /// </summary>
        public override TimeoutScenario SetTimeout(TimeoutScenario scenario)
        {
            return this.currentTimeoutScenario;
        }

        public override void SetWriteTimeout(int timeout)
        {
        }

        public override void SetReadTimeout(int timeout)
        {
        }

        /// <summary>
        /// Send a message, do not expect a response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            StringBuilder builder = new StringBuilder();
            Debug.WriteLine("Sending message " + message.GetBytes().ToHex());
            this.port.Send(message.GetBytes());
            return true;
        }

        public override void ReceiveBufferedMessages()
        {
            return;
        }

        /// <summary>
        /// Try to read an incoming message from the device.
        /// </summary>
        /// <returns></returns>
        public override void Receive(int NumMessages, bool WaitForTimeout)
        {
            return;
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
                Debug.WriteLine("Setting VPW 1X");
            }
            else
            {
                Debug.WriteLine("Setting VPW 4X");
            }

            return true;
        }

        /// <summary>
        /// Purse any messages in the incoming-message buffer.
        /// </summary>
        public override void ClearMessageBuffer()
        {
            this.port.DiscardBuffers();
        }

        public override bool SetLoggingFilter()
        {
            return true;
        }
        public override bool SetAnalyzerFilter()
        {
            return true;
        }
        public override bool RemoveFilters(int[] filterIds)
        {
            return true;
        }
        public override bool RemoveFilters2(int[] filterIds)
        {
            return true;
        }

    }
}
