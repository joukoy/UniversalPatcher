using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Upatcher;
using static Helpers;
using J2534DotNet;
using static LoggerUtils;
using System.Threading;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates all code that is unique to the JET 14005 interface which is simiar to JET 838.
    /// </summary>
    /// 
    public class JetDevice : SerialDevice
    {
        public int ReadTimeout = 1000;
        public int WriteTimeout = 500;

        public const string DeviceType = "JET 14005";
        public short Model = 0; // 0 = unknown
        
        // interface commands (tx)
        public static readonly OBDMessage JET_RESET                = new OBDMessage(new byte[] { 0xF1, 0xA5 });
        // jet cable only capable of VPW
        //public static readonly OBDMessage JET_ENTER_VPW_MODE       = new OBDMessage(new byte[] { 0xE1, 0x33 });
        public static readonly OBDMessage JET_REQUEST_MODEL        = new OBDMessage(new byte[] { 0xF0 });
        public static readonly OBDMessage JET_REQUEST_FIRMWARE     = new OBDMessage(new byte[] { 0xB0 });
        public static readonly OBDMessage JET_DISABLE_TX_ACK       = new OBDMessage(new byte[] { 0x52, 0x40, 0x00 });
        public static readonly OBDMessage JET_EEPROM_REPORT_LO     = new OBDMessage(new byte[] { 0x52, 0x54, 0x01 });
        public static readonly OBDMessage JET_EEPROM_REPORT_HI     = new OBDMessage(new byte[] { 0x52, 0x54, 0x02 });
        public static readonly OBDMessage JET_FILTER_DEST          = new OBDMessage(new byte[] { 0x52, 0x5B, DeviceId.Tool });
        public static readonly OBDMessage JET_1X_SPEED             = new OBDMessage(new byte[] { 0xC1, 0x00 });
        public static readonly OBDMessage JET_4X_SPEED             = new OBDMessage(new byte[] { 0xC1, 0x01 });
        
        // interface response (rx)
        // JET reader strips the header
        public static readonly OBDMessage JET_VPW                  = new OBDMessage(new byte[] { 0x07 });       // 91 07
        public static readonly OBDMessage JET_FIRMWARE             = new OBDMessage(new byte[] { 0x04 });       // 92 04 15 (firmware 1.5)
        public static readonly OBDMessage JET_TX_ACK               = new OBDMessage(new byte[] { 0x60 });       // 01 60
        public static readonly OBDMessage JET_FILTER_DEST_OK       = new OBDMessage(new byte[] { 0x5B, DeviceId.Tool });// 62 5B F0
        public static readonly OBDMessage JET_DISABLE_TX_ACK_OK    = new OBDMessage(new byte[] { 0x40, 0x00 }); // 62 40 00
        public static readonly OBDMessage JET_BLOCK_TX_ACK         = new OBDMessage(new byte[] { 0xF3, 0x60 }); // F3 60
        public static readonly OBDMessage JET_EEPROM_ACK           = new OBDMessage(new byte[] { 0x54 }); // 62 54 [01/02] [64 bytes]

        public JetDevice(IPort port) : base(port)
        {
            this.MaxSendSize = 2048+10+2;    // packets up to 4112 but we want 4096 byte data blocks
            this.MaxReceiveSize = 2048+10+2; // with 10 byte header and 2 byte block checksum
            this.Supports4X = true;
            //this.SupportsSingleDpidLogging = true;
            //this.SupportsStreamLogging = true;
        }

        public override string GetDeviceType()
        {
            return DeviceType;
        }

        public override bool Initialize(int BaudRate, J2534InitParameters j2534Init)
        {
            Logger("Initializing " + this.ToString());

            Response<OBDMessage> m;

            SerialPortConfiguration configuration = new SerialPortConfiguration();
            configuration.BaudRate = 57600; // default RS232 speed for 838
            this.Port.OpenAsync(configuration);
            this.Port.DiscardBuffers();

            Debug.WriteLine("Sending 'reset' message.");
            this.Port.Send(JetDevice.JET_RESET.GetBytes());
            m = ReadJETPacket();
            if (m.Status == ResponseStatus.Success)
            {
                switch (m.Value.GetBytes()[0])
                {
                    case 0x07:
                        Logger("JET 14005 Reset OK");
                        this.Model = 14005;
                        break;
                    default:
                        Logger("Unknown and unsupported JET device detected. Please add support and submit a patch!");
                        return false;
                }
            }
            else
            {
                Logger("JET device not found or failed reset");
                return false;
            }

            Debug.WriteLine("Looking for Firmware message");
            this.Port.Send(JetDevice.JET_REQUEST_FIRMWARE.GetBytes());

            m = this.FindResponse(JET_FIRMWARE);
            if ( m.Status == ResponseStatus.Success )
            {
                byte firmware = m.Value.GetBytes()[1];
                int major = firmware >> 4;
                int minor = firmware & 0x0F;
                Logger("JET Firmware " + major + "." + minor);
            }
            else
            {
                Logger("Firmware not found or failed reset");
                Debug.WriteLine("Expected " + JET_FIRMWARE.GetBytes());
                return false;
            }

            JETSetup();
            this.Connected = true;
            return true;
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

        public override void SetWriteTimeout(int timeout)
        {
            Debug.WriteLine("Setting write timeout to: " + timeout.ToString());
            WriteTimeout = timeout;
        }

        public override void SetReadTimeout(int timeout)
        {
            ReadTimeout = timeout;
            //ReadTimeout = AppSettings.TimeoutJ2534DeviceRead;
            Debug.WriteLine("Setting read timeout to: " + ReadTimeout);
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        public override TimeoutScenario SetTimeout(TimeoutScenario scenario)
        {
            return this.currentTimeoutScenario;
        }

        /// <summary>
        /// This will process incoming messages for up to 500ms looking for a message
        /// </summary>
        public Response<OBDMessage> FindResponse(OBDMessage expected)
        {
            //Debug.WriteLine("FindResponse called");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while(stopwatch.ElapsedMilliseconds < 3000)
            {
                Response<OBDMessage> response = ReadJETPacket();
                if (response.Status == ResponseStatus.Success) 
                    if (Utility.CompareArraysPart(response.Value.GetBytes(), expected.GetBytes()))
                        return Response.Create(ResponseStatus.Success, (OBDMessage) response.Value);
                Thread.Sleep(100);
            }

            return Response.Create(ResponseStatus.Timeout, (OBDMessage) null);
        }

        /// <summary>
        /// Read an JET formatted packet from the interface, and return a Response/Message
        /// </summary>
        private Response<OBDMessage> ReadJETPacket()
        {

            //Debug.WriteLine("Trace: ReadJETPacket");
            int length = 0;
            bool status = true; // do we have a status byte? (we dont for some 9x init commands)
            SerialByte rx = new SerialByte(2); // we dont read more than 2 bytes at a time

            // Get the first packet byte.
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (sw.ElapsedMilliseconds < ReadTimeout)
                {
                    if (this.Port.GetReceiveQueueSize() > 0) 
                    {
                        break;
                    }
                    Thread.Sleep(10);
                }
                if (this.Port.GetReceiveQueueSize() > 0)
                {
                    this.Port.Receive(rx, 0, 1);
                }
                else
                {
                    Debug.WriteLine("Waited " + ReadTimeout.ToString() + " ms.. no data present");
                    return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                }
            }
            catch (Exception) // timeout exception - log no data, return error.
            {
                Debug.WriteLine("No Data");
                return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
            }

            // read a JET format length
            switch (rx.Data[0])
            {
                case 0x11:
                    this.Port.Receive(rx, 0, 1);
                    length = rx.Data[0];
                    break;
                case 0x12:
                    this.Port.Receive(rx, 0, 1);
                    length = rx.Data[0] << 8;
                    this.Port.Receive(rx, 0, 1);
                    length += rx.Data[0];
                    break;
                default:
                    //Debug.WriteLine("RX: Header " + rx[0].ToString("X2"));
                    int type = rx.Data[0] >> 4;
                    switch (type) {
                        case 0xF: // standard < 16 byte data packet (JET 838)
                        case 0x0: // standard < 16 byte data packet (JET 842/852)
                            length = rx.Data[0] & 0x0F;
                            break;
                        case 0x2:
                            length = rx.Data[0] & 0x0F;
                            status = false;
                            break;
                        case 0x3: // Invalid Command
                            length = rx.Data[0] & 0x0F;
                            SerialByte r = new SerialByte(length);
                            this.Port.Receive(r, 0, 1);
                            Debug.WriteLine("RX: Invalid command. Packet that began with  " + r.Data.ToHex() + " was rejected by the JET");
                            return Response.Create(ResponseStatus.Error, new OBDMessage(r.Data));
                        case 0x6: // avt filter / eeprom report
                            length = rx.Data[0] & 0x0F;
                            status = false;
                            break;
                        case 0x8: // high speed notifications
                            length = rx.Data[0] & 0x0F;
                            length--;
                            status = false;
                            break;
                        case 0x9: // init and version
                            length = rx.Data[0] & 0x0F;
                            status = false;
                            break;
                        case 0xC: // C1 01 for 4x OK
                            length = rx.Data[0] & 0x0F;
                            status = false;
                            break;
                        default:
                            Debug.WriteLine("RX: Unhandled packet type " + type + ". Add support to ReadJETPacket()");
                            status = false; // all non-zero high nibble type bytes have no status
                            break;
                    }
                    break;
            }

            // if we need to get check and discard the status byte
            if (status == true)
            {
                length--;
                this.Port.Receive(rx, 0, 1);
                if (rx.Data[0] != 0) 
                    Debug.WriteLine("RX: bad packet status: " + rx.Data[0].ToString("X2"));
            }

            if (length <= 0) {
                Debug.WriteLine("Not reading " + length + " byte packet");
                return Response.Create(ResponseStatus.Error, (OBDMessage)null);
            }

            // build a complete packet
            SerialByte receive = new SerialByte(length);
            byte[] packet = new byte[length];
            int bytes;
            DateTime start = DateTime.Now;
            DateTime stop = start + TimeSpan.FromMilliseconds(ReadTimeout);
            for (int i = 0; i < length; )
            {
                if (DateTime.Now > stop) return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                bytes = this.Port.Receive(receive, 0, length);
                Buffer.BlockCopy(receive.Data, 0, packet, i, bytes);
                i += bytes;
            }

            if (packet[0] == 0x54)
            { // eeprom report, dump addl 64 bytes
                length = 64;
                bytes = 0;
                SerialByte rcv = new SerialByte(length);
                for (int i = 0; i < length;)
                {
                    if (DateTime.Now > stop) return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                    bytes = this.Port.Receive(rcv, 0, length);
                    i += bytes;
                }
            }

            //Debug.WriteLine("Total Length=" + length + " RX: " + packet.ToHex());
            return Response.Create(ResponseStatus.Success, new OBDMessage(packet));
        }

        /// <summary>
        /// Convert a Message to an JET formatted transmit, and send to the interface
        /// </summary>
        private Response<OBDMessage> SendJETPacket(OBDMessage message)
        {
            //Debug.WriteLine("Trace: SendJETPacket");

            byte[] txb = { 0x12 };
            int length = message.GetBytes().Length;

            if (length > 0xFF)
            {
                this.Port.Send(txb);
                txb[0] = unchecked((byte)(length >> 8));
                this.Port.Send(txb);
                txb[0] = unchecked((byte)(length & 0xFF));
                this.Port.Send(txb);
            }
            else if (length > 0x0F)
            {
                txb[0] = (byte)(0x11);
                this.Port.Send(txb);
                txb[0] = unchecked((byte)(length & 0xFF));
                this.Port.Send(txb);
            }
            else
            {
                txb[0] = unchecked((byte)(length & 0x0F));
                this.Port.Send(txb);
            }

            //Debug.WriteLine("send: " + message.GetBytes().ToHex());
            this.Port.Send(message.GetBytes());
            
            return Response.Create(ResponseStatus.Success, message);
        }

        /// <summary>
        /// Configure JET to return only packets targeted to the tool (Device ID F0), and disable transmit acks
        /// </summary>
        private Response<Boolean> JETSetup()
        {
            //Debug.WriteLine("JETSetup called");

            Debug.WriteLine("Disable JET Acks");
            this.Port.Send(JET_DISABLE_TX_ACK.GetBytes());
            Response<OBDMessage> m = this.FindResponse(JET_DISABLE_TX_ACK_OK);
            if (m.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("JET Acks disabled");
            }
            else
            {
                Logger("Could not disable ACKs");
                Debug.WriteLine("Expected " + JET_DISABLE_TX_ACK_OK.ToString());
                return Response.Create(ResponseStatus.Error, false);
            }

            SetLoggingFilter();
            
            Debug.WriteLine("Dump JET eeprom");
            this.Port.Send(JET_EEPROM_REPORT_LO.GetBytes());
            m = this.FindResponse(JET_EEPROM_ACK);
            if (m.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("JET eeprom read low");
                this.Port.Send(JET_EEPROM_REPORT_HI.GetBytes());
                m = this.FindResponse(JET_EEPROM_ACK);
                if (m.Status == ResponseStatus.Success)
                {
                    Debug.WriteLine("JET eeprom read high");
                }
                else
                {
                    Logger("Could not dump JET eeprom high");
                    Debug.WriteLine("Expected " + JET_EEPROM_ACK.ToString());
                    return Response.Create(ResponseStatus.Error, false);
                }

            }
            else
            {
                Logger("Could not dump JET eeprom low");
                Debug.WriteLine("Expected " + JET_EEPROM_ACK.ToString());
                return Response.Create(ResponseStatus.Error, false);
            }

            return Response.Create(ResponseStatus.Success, true);
        }

        /// <summary>
        /// Send a message, wait for a response, return the response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            //Debug.WriteLine("Sendrequest called");
            Debug.WriteLine("TX: " + message.GetBytes().ToHex());
            SendJETPacket(message);
            message.TimeStamp = DateTime.Now.Ticks;
            this.MessageSent(message);
            return true;
        }

        public override void ReceiveBufferedMessages()
        {
        }

        public override void Receive(bool WaitForTimeout)
        {
            if (WaitForTimeout || Port.GetReceiveQueueSize() > 3)
            {
                Response<OBDMessage> response = ReadJETPacket();
                if (response.Status == ResponseStatus.Success)
                {
                    Debug.WriteLine("RX: " + response.Value.GetBytes().ToHex());
                    this.Enqueue(response.Value, true);
                    return;
                }
            }
            Debug.WriteLine("JET: no message waiting.");            
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
                Debug.WriteLine("JET setting VPW 1X");
                this.Port.Send(JetDevice.JET_1X_SPEED.GetBytes());
                ReadJETPacket(); // C1 00 (switched to 1x)
            }
            else
            {
                ReadJETPacket(); // 23 83 00 20 JET generated response from generic PCM switch high speed command in Vehicle.cs
                Debug.WriteLine("JET setting VPW 4X");
                this.Port.Send(JetDevice.JET_4X_SPEED.GetBytes());
                ReadJETPacket(); // C1 01 (switched to 4x)
            }

            return true;
        }

        public override void ClearMessageBuffer()
        {
            this.Port.DiscardBuffers();
            System.Threading.Thread.Sleep(50);
        }

        public override bool SetLoggingFilter()
        {
            Debug.WriteLine("Configure JET filter");
            this.Port.Send(JET_FILTER_DEST.GetBytes());
            Response<OBDMessage> m = this.FindResponse(JET_FILTER_DEST_OK);
            if (m.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("JET filter configured");
            }
            else
            {
                Logger("Could not configure JET filter");
                Debug.WriteLine("Expected " + JET_FILTER_DEST_OK.ToString());
                return false;
            }
            return true;
        }
        public override bool SetAnalyzerFilter()
        {
            Debug.WriteLine("SetAnalyzerFilter not implemented");
            return true;
        }
        public override bool RemoveFilters(int[] filterIds)
        {
            Debug.WriteLine("RemoveFilters not implemented");
            return true;
        }

    }
}
