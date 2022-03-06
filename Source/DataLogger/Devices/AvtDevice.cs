using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates all code that is unique to the AVT 852 interface.
    /// </summary>
    /// 
    public class AvtDevice : SerialDevice
    {
        public const string DeviceType = "AVT (838/842/852)";
        public short Model = 0; // 0 = unknown or 838, 842, 852

        public static readonly OBDMessage AVT_RESET                = new OBDMessage(new byte[] { 0xF1, 0xA5 });
        public static readonly OBDMessage AVT_ENTER_VPW_MODE       = new OBDMessage(new byte[] { 0xE1, 0x33 });
        public static readonly OBDMessage AVT_REQUEST_MODEL        = new OBDMessage(new byte[] { 0xF0 });
        public static readonly OBDMessage AVT_REQUEST_FIRMWARE     = new OBDMessage(new byte[] { 0xB0 });
        public static readonly OBDMessage AVT_DISABLE_TX_ACK       = new OBDMessage(new byte[] { 0x52, 0x40, 0x00 });
        public static readonly OBDMessage AVT_FILTER_DEST          = new OBDMessage(new byte[] { 0x52, 0x5B, DeviceId.Tool });
        public static readonly OBDMessage AVT_1X_SPEED             = new OBDMessage(new byte[] { 0xC1, 0x00 });
        public static readonly OBDMessage AVT_4X_SPEED             = new OBDMessage(new byte[] { 0xC1, 0x01 });

        // AVT reader strips the header
        public static readonly OBDMessage AVT_VPW                  = new OBDMessage(new byte[] { 0x07 });       // 91 07
        public static readonly OBDMessage AVT_852_IDLE             = new OBDMessage(new byte[] { 0x27 });       // 91 27
        public static readonly OBDMessage AVT_842_IDLE             = new OBDMessage(new byte[] { 0x12 });       // 91 12
        public static readonly OBDMessage AVT_FIRMWARE             = new OBDMessage(new byte[] { 0x04 });       // 92 04 15 (firmware 1.5)
        public static readonly OBDMessage AVT_TX_ACK               = new OBDMessage(new byte[] { 0x60 });       // 01 60
        public static readonly OBDMessage AVT_FILTER_DEST_OK       = new OBDMessage(new byte[] { 0x5B, DeviceId.Tool });// 62 5B F0
        public static readonly OBDMessage AVT_DISABLE_TX_ACK_OK    = new OBDMessage(new byte[] { 0x40, 0x00 }); // 62 40 00
        public static readonly OBDMessage AVT_BLOCK_TX_ACK         = new OBDMessage(new byte[] { 0xF3, 0x60 }); // F3 60

        public AvtDevice(IPort port) : base(port)
        {
            this.MaxSendSize = 4096+10+2;    // packets up to 4112 but we want 4096 byte data blocks
            this.MaxReceiveSize = 4096+10+2; // with 10 byte header and 2 byte block checksum
            this.Supports4X = true;
            this.LogDeviceType = DataLogger.LoggingDevType.Other;
        }

        public override string GetDeviceType()
        {
            return DeviceType;
        }

        public override bool Initialize(int BaudRate, LoggerUtils.J2534InitParameters j2534Init)
        {
            Debug.WriteLine("Initializing " + this.ToString());

            Response<OBDMessage> m;

            SerialPortConfiguration configuration = new SerialPortConfiguration();
            configuration.BaudRate = BaudRate;
            this.Port.OpenAsync(configuration);
            this.Port.DiscardBuffers();

            Debug.WriteLine("Sending 'reset' message.");
            this.Port.Send(AvtDevice.AVT_RESET.GetBytes());
            m = ReadAVTPacket();
            if (m.Status == ResponseStatus.Success)
            {
                switch (m.Value.GetBytes()[0])
                {
                    case 0x27:
                        Logger("AVT 852 Reset OK");
                        this.Model = 852;
                        break;
                    case 0x12:
                        Logger("AVT 842 Reset OK");
                        this.Model = 842;
                        break;
                    case 0x07:
                        Logger("AVT 838 Reset OK");
                        this.Model = 838;
                        this.MaxSendSize = 2048 + 10 + 2;
                        this.MaxReceiveSize = 2048 + 10 + 2;
                        break;
                    default:
                        Logger("Unknown and unsupported AVT device detected. Please add support and submit a patch!");
                        return false;
                }
            }
            else
            {
                Logger("AVT device not found or failed reset");
                return false;
            }

            if (this.Model == 838)
            {
                Debug.WriteLine("Sending 'Firmware' message.");
                this.Port.Send(AvtDevice.AVT_REQUEST_FIRMWARE.GetBytes());// we need to request this on 838 but the 852 sends it without being asked. 842 needs testing.
            }
            Debug.WriteLine("Looking for Firmware message");
            m = this.FindResponse(AVT_FIRMWARE);
            if ( m.Status == ResponseStatus.Success )
            {
                byte firmware = m.Value.GetBytes()[1];
                int major = firmware >> 4;
                int minor = firmware & 0x0F;
                Logger("AVT Firmware " + major + "." + minor);
            }
            else
            {
                Logger("Firmware not found or failed reset");
                Debug.WriteLine("Expected " + AVT_FIRMWARE.GetBytes());
                return false;
            }

            // 838 defaults to vpw mode, so dont set it on that device.
            if (this.Model != 838)
            {
                this.Port.Send(AvtDevice.AVT_ENTER_VPW_MODE.GetBytes());
                m = FindResponse(AVT_VPW);
                if (m.Status == ResponseStatus.Success)
                {
                    Debug.WriteLine("Set VPW Mode");
                }
                else
                {
                    Logger("Unable to set AVT device to VPW mode");
                    Debug.WriteLine("Expected " + AvtDevice.AVT_VPW.ToString());
                    return false;
                }
            }
            AVTSetup();
            SetLoggingFilter();
            return true;
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
                Response<OBDMessage> response = this.ReadAVTPacket();
                if (response.Status == ResponseStatus.Success) 
                    if (Utility.CompareArraysPart(response.Value.GetBytes(), expected.GetBytes()))
                        return Response.Create(ResponseStatus.Success, (OBDMessage) response.Value);
                Thread.Sleep(100);
            }

            return Response.Create(ResponseStatus.Timeout, (OBDMessage) null);
        }

        /// <summary>
        /// Read an AVT formatted packet from the interface, and return a Response/Message
        /// </summary>
        private Response<OBDMessage> ReadAVTPacket()
        {

            //Debug.WriteLine("Trace: ReadAVTPacket");
            int length = 0;
            bool status = true; // do we have a status byte? (we dont for some 9x init commands)
            SerialByte rx = new SerialByte(2); // we dont read more than 2 bytes at a time
            // Get the first packet byte.

            bool Chk = false;
            try
            {
                Chk = (WaitForSerial(1, 1000));
                if (Chk == false)
                {
                    Debug.WriteLine("Timeout.. no data present A");
                    return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                }

                //get first byte for command
                this.Port.Receive(rx, 0, 1);
            }
            catch (Exception) // timeout exception - log no data, return error.
            {
                Debug.WriteLine("No Data");
                return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
            }

            // read an AVT format length
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
                        case 0x0: // standard < 16 byte data packet
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
                            Debug.WriteLine("RX: Invalid command. Packet that began with  " + r.Data.ToHex() + " was rejected by the AVT");
                            return Response.Create(ResponseStatus.Error, new OBDMessage(r.Data));
                        case 0x6: // avt filter
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
                            Debug.WriteLine("RX: Unhandled packet type " + type + ". Add support to ReadAVTPacket()");
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
                if (rx.Data[0] != 0) Debug.WriteLine("RX: bad packet status: " + rx.Data[0].ToString("X2"));
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
            DateTime stop = start + TimeSpan.FromSeconds(2);
            for (int i = 0; i < length; )
            {
                if (DateTime.Now > stop) return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                bytes = this.Port.Receive(receive, 0, length);
                Buffer.BlockCopy(receive.Data, 0, packet, i, bytes);
                i += bytes;
            }

            //Debug.WriteLine("Total Length=" + length + " RX: " + packet.ToHex());
            OBDMessage rMsg = new OBDMessage(packet);
            rMsg.TimeStamp = (ulong)rx.TimeStamp;
            rMsg.SysTimeStamp = rMsg.TimeStamp;
            return Response.Create(ResponseStatus.Success, rMsg);

            //return Response.Create(ResponseStatus.Success, new OBDMessage(packet));
        }

        /// <summary>
        /// Wait for serial bytes to be availble. False if timeout.
        /// </summary>
        private bool WaitForSerial(ushort NumBytes, int timeout = 0)
        {
            if (timeout == 0)
            {
                timeout = 500;
            }

            int TempCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Wait for bytes to arrive...
            while (sw.ElapsedMilliseconds < timeout)
            {
                if (this.Port.GetReceiveQueueSize() > TempCount)
                {
                    TempCount = this.Port.GetReceiveQueueSize();
                    sw.Restart();
                }
                if (this.Port.GetReceiveQueueSize() >= NumBytes) { return true; }
                Thread.Sleep(10);
            }
            return false;
        }

        /// <summary>
        /// Convert a Message to an AVT formatted transmit, and send to the interface
        /// </summary>
        private Response<OBDMessage> SendAVTPacket(OBDMessage message, int responses)
        {
            //Debug.WriteLine("Trace: SendAVTPacket");

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

            Debug.WriteLine("send: " + message.GetBytes().ToHex());
            this.Port.Send(message.GetBytes());

            return Response.Create(ResponseStatus.Success, message);
        }

        /// <summary>
        /// Configure AVT to return only packets targeted to the tool (Device ID F0), and disable transmit acks
        /// </summary>
        private Response<Boolean> AVTSetup()
        {
            //Debug.WriteLine("AVTSetup called");

            Debug.WriteLine("Disable AVT Acks");
            this.Port.Send(AVT_DISABLE_TX_ACK.GetBytes());
            Response<OBDMessage> m = this.FindResponse(AVT_DISABLE_TX_ACK_OK);
            if (m.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("AVT Acks disabled");
            }
            else
            {
                Logger("Could not disable ACKs");
                Debug.WriteLine("Expected " + AVT_DISABLE_TX_ACK_OK.ToString());
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
            this.MessageSent(message);
            SendAVTPacket(message, responses);
            return true;
        }

        public override void Receive()
        {
           
            Response<OBDMessage> response = ReadAVTPacket();
            if (response.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("RX: " + response.Value.GetBytes().ToHex());
                this.Enqueue(response.Value);
                return;
            }

            Debug.WriteLine("AVT: no message waiting.");            
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
                Debug.WriteLine("AVT setting VPW 1X");
                this.Port.Send(AvtDevice.AVT_1X_SPEED.GetBytes());
                ReadAVTPacket(); // C1 00 (switched to 1x)
            }
            else
            {
                ReadAVTPacket(); // 23 83 00 20 AVT generated response from generic PCM switch high speed command in Vehicle.cs
                Debug.WriteLine("AVT setting VPW 4X");
                this.Port.Send(AvtDevice.AVT_4X_SPEED.GetBytes());
                ReadAVTPacket(); // C1 01 (switched to 4x)
            }

            return true;
        }

        public override void ClearMessageBuffer()
        {
            this.Port.DiscardBuffers();
            System.Threading.Thread.Sleep(50);
        }

        public override bool SetProtocol(int Protocol, int BaudRate, int ConnectFlag)
        {
            return false;
        }

        public override bool SetConfig(J2534DotNet.SConfig[] sc)
        {
            return false;
        }

        public override bool SetLoggingFilter()
        {
            if (this.CurrentFilter == "logging" || datalogger.useVPWFilters == false)
                return true;
            Debug.WriteLine("Configure AVT filter");
            this.Port.Send(AVT_FILTER_DEST.GetBytes());
            Response<OBDMessage> m = this.FindResponse(AVT_FILTER_DEST_OK);
            if (m.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("AVT filter configured");
            }
            else
            {
                Logger("Could not configure AVT filter");
                Debug.WriteLine("Expected " + AVT_FILTER_DEST_OK.ToString());
                return false;
            }
            this.CurrentFilter = "logging";
            return true;
        }
        public override bool SetAnalyzerFilter()
        {
            if (this.CurrentFilter == "analyzer")
                return true;
            Debug.WriteLine("Setting analyzer filter");
            byte[] msg = new byte[] { 0x52, 0x5b, 0x00 };
            Port.Send(msg); //Clear filter, listen for all devices
            msg = new byte[] { 0xe1, 0x33 };
            Port.Send(msg); //Begin monitoring bus traffic
            this.CurrentFilter = "analyzer";
            return true;
        }
        public override bool RemoveFilters()
        {
            Debug.WriteLine("Removing filters");
            byte[] msg = new byte[] { 0x52, 0x5b, 0x00 };
            Port.Send(msg); //Clear filter, listen for all devices
            return true;
        }

    }
}
