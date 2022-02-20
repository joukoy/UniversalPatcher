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
    /// This class encapsulates all code that is unique to the DVI interface.
    /// </summary>
    /// 
    public class OBDXProDevice : SerialDevice
    {
        public const string DeviceType = "OBDX Pro";
        public bool TimeStampsEnabled = false;
        public bool CRCInReceivedFrame = false;
        private int BaudRate;

        // This default is probably excessive but it should always be
        // overwritten by a call to SetTimeout before use anyhow.
        private VpwSpeed vpwSpeed = VpwSpeed.Standard;

        //ELM Command Set
        //To be put in, not really needed if using DVI command set

        //DVI (Direct Vehicle Interface) Command Set
        public static readonly OBDMessage DVI_BOARD_HARDWARE_VERSION = new OBDMessage(new byte[] { 0x22, 0x1, 0x0, 0 });
        public static readonly OBDMessage DVI_BOARD_FIRMWARE_VERSION = new OBDMessage(new byte[] { 0x22, 0x1, 0x1, 0 });
        public static readonly OBDMessage DVI_BOARD_MODEL = new OBDMessage(new byte[] { 0x22, 0x1, 0x2, 0 });
        public static readonly OBDMessage DVI_BOARD_NAME = new OBDMessage(new byte[] { 0x22, 0x1, 0x3, 0 });
        public static readonly OBDMessage DVI_UniqueSerial = new OBDMessage(new byte[] { 0x22, 0x1, 0x4, 0 });
        public static readonly OBDMessage DVI_Supported_OBD_Protocols = new OBDMessage(new byte[] { 0x22, 0x1, 0x5, 0 });
        public static readonly OBDMessage DVI_Supported_PC_Protocols = new OBDMessage(new byte[] { 0x22, 0x1, 0x6, 0 });

        public static readonly OBDMessage DVI_Req_NewtorkWriteStatus = new OBDMessage(new byte[] { 0x24, 0x1, 0x1, 0 });
        public static readonly OBDMessage DVI_Set_NewtorkWriteStatus = new OBDMessage(new byte[] { 0x24, 0x2, 0x1, 0, 0 });
        public static readonly OBDMessage DVI_Req_ConfigRespStatus = new OBDMessage(new byte[] { 0x24, 0x1, 0x2, 0 });
        public static readonly OBDMessage DVI_Set_CofigRespStatus = new OBDMessage(new byte[] { 0x24, 0x2, 0x2, 0, 0 });

        public static readonly OBDMessage DVI_Req_TimeStampOnRxNetwork = new OBDMessage(new byte[] { 0x24, 0x1, 0x3, 0 });
        public static readonly OBDMessage DVI_Set_TimeStampOnRxNetwork = new OBDMessage(new byte[] { 0x24, 0x2, 0x3, 0, 0 });

        public static readonly OBDMessage DVI_RESET = new OBDMessage(new byte[] { 0x25, 0x0, 0 });

        public static readonly OBDMessage DVI_Req_OBD_Protocol = new OBDMessage(new byte[] { 0x31, 0x1, 0x1, 0 });
        public static readonly OBDMessage DVI_Set_OBD_Protocol = new OBDMessage(new byte[] { 0x31, 0x2, 0x1, 0, 0 });
        public static readonly OBDMessage DVI_Req_NewtorkEnable = new OBDMessage(new byte[] { 0x31, 0x1, 0x2, 0 });
        public static readonly OBDMessage DVI_Set_NewtorkEnable = new OBDMessage(new byte[] { 0x31, 0x2, 0x2, 0, 0 });
        public static readonly OBDMessage DVI_Set_API_Protocol = new OBDMessage(new byte[] { 0x31, 0x2, 0x6, 0, 0 });

        public static readonly OBDMessage DVI_Req_To_Filter = new OBDMessage(new byte[] { 0x33, 0x1, 0x0, 0 });
        public static readonly OBDMessage DVI_Set_To_Filter = new OBDMessage(new byte[] { 0x33, 0x3, 0x0, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_From_Filter = new OBDMessage(new byte[] { 0x33, 0x1, 0x1, 0 });
        public static readonly OBDMessage DVI_Set_From_Filter = new OBDMessage(new byte[] { 0x33, 0x3, 0x1, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_RangeTo_Filter = new OBDMessage(new byte[] { 0x33, 0x1, 0x2, 0 });
        public static readonly OBDMessage DVI_Set_RangeTo_Filter = new OBDMessage(new byte[] { 0x33, 0x3, 0x2, 0, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_RangeFrom_Filter = new OBDMessage(new byte[] { 0x33, 0x1, 0x3, 0 });
        public static readonly OBDMessage DVI_Set_RangeFrom_Filter = new OBDMessage(new byte[] { 0x33, 0x4, 0x3, 0, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_Mask = new OBDMessage(new byte[] { 0x33, 0x1, 0x4, 0 });
        public static readonly OBDMessage DVI_Set_Mask = new OBDMessage(new byte[] { 0x33, 0x5, 0x4, 0, 0, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_Speed = new OBDMessage(new byte[] { 0x33, 0x1, 0x6, 0 });
        public static readonly OBDMessage DVI_Set_Speed = new OBDMessage(new byte[] { 0x33, 0x2, 0x6, 0, 0 });
        public static readonly OBDMessage DVI_Req_ValidateCRC_onRX = new OBDMessage(new byte[] { 0x33, 0x1, 0x7, 0 });
        public static readonly OBDMessage DVI_Set_ValidateCRC_onRX = new OBDMessage(new byte[] { 0x33, 0x2, 0x7, 0, 0 });
        public static readonly OBDMessage DVI_Req_Show_CRC_OnNetwork = new OBDMessage(new byte[] { 0x33, 0x1, 0x8, 0 });
        public static readonly OBDMessage DVI_Set_Show_CRC_OnNetwork = new OBDMessage(new byte[] { 0x33, 0x2, 0x8, 0, 0 });
        public static readonly OBDMessage DVI_Req_Write_Idle_Timeout = new OBDMessage(new byte[] { 0x33, 0x1, 0x9, 0 });
        public static readonly OBDMessage DVI_Set_Write_Idle_Timeout = new OBDMessage(new byte[] { 0x33, 0x3, 0x9, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_1x_Timings = new OBDMessage(new byte[] { 0x33, 0x2, 0xA, 0, 0 });
        public static readonly OBDMessage DVI_Set_1x_Timings = new OBDMessage(new byte[] { 0x33, 0x4, 0xA, 0, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_4x_Timings = new OBDMessage(new byte[] { 0x33, 0x2, 0xB, 0, 0 });
        public static readonly OBDMessage DVI_Set_4x_Timings = new OBDMessage(new byte[] { 0x33, 0x4, 0xB, 0, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_ResetTimings = new OBDMessage(new byte[] { 0x33, 0x2, 0xC, 0, 0 });
        public static readonly OBDMessage DVI_Req_ErrorBits = new OBDMessage(new byte[] { 0x33, 0x1, 0xD, 0 });
        public static readonly OBDMessage DVI_Set_ErrorBits = new OBDMessage(new byte[] { 0x33, 0x3, 0xD, 0, 0, 0 });
        public static readonly OBDMessage DVI_Req_ErrorCount = new OBDMessage(new byte[] { 0x33, 0x2, 0xE, 0, 0 });
        public static readonly OBDMessage DVI_Req_DefaultSettings = new OBDMessage(new byte[] { 0x33, 0x1, 0xF, 0 });

        public static readonly OBDMessage DVI_Req_ADC_SingleChannel = new OBDMessage(new byte[] { 0x35, 0x2, 0x0, 0, 0 });
        public static readonly OBDMessage DVI_Req_ADC_MultipleChannels = new OBDMessage(new byte[] { 0x35, 0x2, 0x1, 0, 0 });

        public OBDXProDevice(IPort port) : base(port)
        {
            this.MaxSendSize = 4096 + 10 + 2;    // packets up to 4112 but we want 4096 byte data blocks
            this.MaxReceiveSize = 4096 + 10 + 2; // with 10 byte header and 2 byte block checksum
            this.Supports4X = true;

            // This will be used during device initialization.
            this.currentTimeoutScenario = TimeoutScenario.ReadProperty;
            this.LogDeviceType = DataLogger.LoggingDevType.Other;
        }

        public override string GetDeviceType()
        {
            return DeviceType;
        }

        public override bool Initialize(int BaudRate)
        {
            Debug.WriteLine("Initializing " + this.ToString());

            //For further inits:
            this.BaudRate = BaudRate;
            SerialPortConfiguration configuration = new SerialPortConfiguration();
            configuration.BaudRate = BaudRate;
            configuration.Timeout = 1000;
            this.Port.OpenAsync(configuration);
            System.Threading.Thread.Sleep(100);

            ////Reset scantool - ensures starts at ELM protocol
            bool Status = ResetDevice();
            if (Status == false)
            {
                Logger("Unable to reset DVI device.");
                return false;
            }

            //Request Board information
            Response<string> BoardName = GetBoardDetails();
            if (BoardName.Status != ResponseStatus.Success)
            {
                Logger("Unable to get DVI device details.");
                return false;
            }

            //Set protocol to VPW mode
            Status = SetProtocol(OBDProtocols.VPW);
            if (Status == false)
            {
                Logger("Unable to set DVI device protocol to VPW.");
                return false;
            }

            Response<bool> SetupStatus = DVISetup();
            if (SetupStatus.Status != ResponseStatus.Success)
            {
                Logger("DVI device initialization failed.");
                return false;
            }
            
            if (!SetToFilter(DeviceId.Tool))
            {
                Logger("Filter setup failed.");
                return false;
            }

            Logger("Device Successfully Initialized and Ready");
            return true;
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        public override TimeoutScenario SetTimeout(TimeoutScenario scenario)
        {
            TimeoutScenario previousScenario = this.currentTimeoutScenario;
            this.currentTimeoutScenario = scenario;
            return previousScenario;
        }

        /// <summary>
        /// This will process incoming messages for up to 250ms looking for a message
        /// </summary>

        public Response<OBDMessage> FindResponseFromTool(byte[] expected)
        {
            //Debug.WriteLine("FindResponse called");
            for (int iterations = 0; iterations < 5; iterations++)
            {
                Response<OBDMessage> response = this.ReadDVIPacket(this.GetReceiveTimeout());
                if (response != null)  // Hack to silence error - See: https://pcmhacking.net/forums/viewtopic.php?f=42&t=6730&start=110#p101790
                    if (response.Status == ResponseStatus.Success)
                        if (Utility.CompareArraysPart(response.Value.GetBytes(), expected))
                            return Response.Create(ResponseStatus.Success, (OBDMessage)response.Value);
                Thread.Sleep(50);
            }

            return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
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
        /// Read an DVI formatted packet from the interface.
        /// If it recevies a Network message, in enqueues it and returns null;
        /// If it receives a Device message, it returns the message.
        /// </summary>
        private Response<OBDMessage> ReadDVIPacket(int timeout = 0)
        {
            UInt16 Length = 0;

            byte offset = 0;
            SerialByte rx = new SerialByte(3); // we dont read more than 3 bytes at a time
            SerialByte timestampbuf = new SerialByte(3);
            ulong timestampmicro = 0;
            // First Byte is command
            //Second is length, third also for long frame
            //Data
            //Checksum
            bool Chk = false;
            try
            {
                Chk = (WaitForSerial(1, timeout));
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


            if (rx.Data[0] == 0x8 || rx.Data[0] == 0x9) //for network frames
            {
                //check if timestamps enabled
                if (TimeStampsEnabled)
                {
                    //next 4 bytes will be timestamp in microseconds
                    for (byte i = 0; i < 4; i++)
                    {
                        Chk = (WaitForSerial(1));
                        if (Chk == false)
                        {
                            Debug.WriteLine("Timeout.. no data present B");
                            return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                        }
                        this.Port.Receive(timestampbuf, i, 1);
                    }
                    timestampmicro = (ulong)((ulong)timestampbuf.Data[0] * 0x100 ^ 3) + (ulong)((ulong)timestampbuf.Data[1] * 0x100 ^ 2) + (ulong)((ulong)timestampbuf.Data[0] * 0x100) + (ulong)timestampbuf.Data[0];
                }
                if (rx.Data[0] == 0x8) //if short, only get one byte for length
                {
                    Chk = (WaitForSerial(1));
                    if (Chk == false)
                    {
                        Debug.WriteLine("Timeout.. no data present C");
                        return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                    }
                    this.Port.Receive(rx, 1, 1);
                    Length = rx.Data[1];
                }
                else //if long, get two bytes for length
                {
                    offset += 1;
                    Chk = (WaitForSerial(2));
                    if (Chk == false)
                    {
                        Debug.WriteLine("Timeout.. no data present D");
                        return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                    }
                    this.Port.Receive(rx, 1, 2);
                    Length = (ushort)((ushort)(rx.Data[1] * 0x100) + rx.Data[2]);
                }

            }
            else //for all other received frames
            {
                Chk = (WaitForSerial(1));
                if (Chk == false)
                {
                    Debug.WriteLine("Timeout.. no data present E");
                    return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
                }
                this.Port.Receive(rx, 1, 1);
                Length = rx.Data[1];
            }

            SerialByte receive = new SerialByte(Length + 3 + offset);
            Chk = (WaitForSerial((ushort)(Length + 1)));
            if (Chk == false)
            {
                Debug.WriteLine("Timeout.. no data present F");
                return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
            }

            int bytes;
            receive.Data[0] = rx.Data[0];//Command
            receive.Data[1] = rx.Data[1];//length
            if (rx.Data[0] == 0x09) receive.Data[2] = rx.Data[2];//length long frame
            bytes = this.Port.Receive(receive, 2 + offset, Length + 1);//get rest of frame
            if (bytes <= 0)
            {
                Debug.WriteLine("Failed reading " + Length + " byte packet");
                return Response.Create(ResponseStatus.Error, (OBDMessage)null);
            }
            //should have entire frame now
            //verify checksum correct
            byte CalcChksm = 0;
            for (ushort i = 0; i < (receive.Data.Length - 1); i++) CalcChksm += receive.Data[i];
            if (rx.Data[0] == 0x08 || rx.Data[0] == 0x09)
            {
                if (TimeStampsEnabled)
                {
                    CalcChksm += timestampbuf.Data[0];
                    CalcChksm += timestampbuf.Data[1];
                    CalcChksm += timestampbuf.Data[2];
                    CalcChksm += timestampbuf.Data[3];
                }
            }
            CalcChksm = (byte)~CalcChksm;

            if (receive.Data[receive.Data.Length - 1] != CalcChksm)
            {
                Debug.WriteLine("Total Length Data=" + Length + " RX: " + receive.Data.ToHex());
                Debug.WriteLine("Checksum error on received message.");
                return null;
            }

            //Debug.WriteLine("Total Length Data=" + Length + " RX: " + receive.ToHex());

           /* if (receive.Data.Length > 5 && receive.Data[5] == 0x7F)
            {
                // Error from the device
                OBDMessage result = new OBDMessage(receive.Data);
                Debug.WriteLine("XPro Error: " + result.ToString());
                return Response.Create(ResponseStatus.Error, result);
            }
            else*/
           if (receive.Data[0] == 0x8 || receive.Data[0] == 0x9)
            {
                //network frames //Strip header and checksum
                byte[] StrippedFrame = new byte[Length];
                Buffer.BlockCopy(receive.Data, 2 + offset, StrippedFrame, 0, Length);
                //Debug.WriteLine("RX: " + StrippedFrame.ToHex());
                if (!TimeStampsEnabled)
                    timestampmicro = (ulong)rx.TimeStamp;
                this.Enqueue(new OBDMessage(StrippedFrame, timestampmicro, 0));
                return null;
            }
            else
            {
                // Valid message from the device
                //Debug.WriteLine("XPro: " + receive.ToHex());
                OBDMessage rMsg = new OBDMessage(receive.Data);
                rMsg.TimeStamp = (ulong)rx.TimeStamp;
                return Response.Create(ResponseStatus.Success, rMsg);
            }
        }


        private Response<String> ReadELMPacket(String SentFrame)
        {
            // UInt16 Counter = 0;
            bool framefound = false;
            bool Chk = false;

            string StrResp = "";
            SerialByte rx = new SerialByte(1);
            rx.Data[0] = 0;
            try
            {
                while (framefound == false)
                {
                    Chk = (WaitForSerial(1));
                    if (Chk == false)
                    {
                        Debug.WriteLine("Timeout.. no data present");
                        return Response.Create(ResponseStatus.Timeout, "");
                    }

                    this.Port.Receive(rx, 0, 1);
                    if (rx.Data[0] == 0xD) //carriage return
                    {
                        if (StrResp != SentFrame)
                        {
                            framefound = true;
                            break;
                        }
                        StrResp = "";
                        continue;
                    }
                    else if (rx.Data[0] == 0xA) continue;//newline
                    StrResp += Convert.ToChar(rx.Data[0]);
                }

                //Find Idle frame
                framefound = false;
                while (framefound == false)
                {
                    Chk = (WaitForSerial(1));
                    if (Chk == false)
                    {
                        Debug.WriteLine("ELM Idle frame not detected");
                        return Response.Create(ResponseStatus.Timeout, "");
                    }
                    this.Port.Receive(rx, 0, 1);
                    if (rx.Data[0] == '>')
                    {
                        framefound = true;
                        break;
                    }
                }
                return Response.Create(ResponseStatus.Success, StrResp);

            }
            catch (Exception) // timeout exception - log no data, return error.
            {
                Debug.WriteLine("No Data");
                return Response.Create(ResponseStatus.Timeout, (""));
            }

        }

        /// <summary>
        /// Calc checksum for byte array for all messages to/from device
        /// </summary>
        private byte CalcChecksum(byte[] MyArr)
        {
            byte CalcChksm = 0;
            for (ushort i = 0; i < (MyArr.Length - 1); i++)
            {
                CalcChksm += MyArr[i];
            }
            return ((byte)~CalcChksm);
        }


        /// <summary>
        /// Convert a Message to an DVI formatted transmit, and send to the interface
        /// </summary>
        private Response<OBDMessage> SendDVIPacket(OBDMessage message, int responses)
        {
            int length = message.GetBytes().Length;
            byte[] RawPacket = message.GetBytes();
            byte[] SendPacket = new byte[length + 3];

            if (length > 0xFF)
            {
                System.Array.Resize(ref SendPacket, SendPacket.Length + 1);
                SendPacket[0] = 0x11;
                SendPacket[1] = (byte)(length >> 8);
                SendPacket[2] = (byte)length;
                Buffer.BlockCopy(RawPacket, 0, SendPacket, 3, length);
            }
            else
            {
                SendPacket[0] = 0x10;
                SendPacket[1] = (byte)length;
                Buffer.BlockCopy(RawPacket, 0, SendPacket, 2, length);
            }

            //Add checksum
            SendPacket[SendPacket.Length - 1] = CalcChecksum(SendPacket);

            //Send frame
            this.Port.Send(SendPacket);
            Debug.WriteLine("TX: " + message.ToString());

            // Wait for confirmation of successful send
            Response<OBDMessage> m = null;

            if (responses < 1)
            {
                return Response.Create(ResponseStatus.Success, new OBDMessage(new byte[0]));
            }
            for (int attempt = 0; attempt < 10; attempt++)
            {
                m = ReadDVIPacket(500);
                if (m != null)
                {
                    if (m.Status == ResponseStatus.Timeout)
                    {
                        continue;
                    }
                    break;
                }
            }            

            if (m == null)
            {
                // This should never happen, but just in case...
                Logger("No response to send attempt. " + message.ToString());
                return Response.Create(ResponseStatus.Error, new OBDMessage(new byte[0]));
            }

            Debug.WriteLine("RX: " + m.ToString());
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value.GetBytes();
                if (Val[0] == 0x20 && Val[2] == 0x00)
                {
                    //Debug.WriteLine("TX: " + message.ToString());
                    return Response.Create(ResponseStatus.Success, message);
                }
                else if (Val[0] == 0x21 && Val[2] == 0x00)
                {
                    //Debug.WriteLine("TX: " + message.ToString());
                    return Response.Create(ResponseStatus.Success, message);
                }
                else
                {
                    Logger("Unable to transmit, odd response from device: " + message.ToString());
                    return Response.Create(ResponseStatus.Error, message);
                }
            }
            else
            {
                Logger("Unable to transmit, " + m.Status + ": " + message.ToString());
                return Response.Create(ResponseStatus.Error, message);
            }
        }

        /// <summary>
        /// Configure DVI to return only packets targeted to the tool (Device ID F0), and disable transmit acks
        /// </summary>
        private Response<Boolean> DVISetup()
        {
            //Enable network rx/tx for protocol
            bool Status = EnableProtocolNetwork();
            if (Status == false) return Response.Create(ResponseStatus.Error, false);

            return Response.Create(ResponseStatus.Success, true);
        }

        /// <summary>
        /// Send a message, wait for a response, return the response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            //Debug.WriteLine("Sendrequest called");
            //  Debug.WriteLine("TX: " + message.GetBytes().ToHex());
            Response<OBDMessage> m = SendDVIPacket(message, responses);
            if (m.Status != ResponseStatus.Success)
            {
                Debug.WriteLine(m.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Receive a message from the network - or at least try to.
        /// </summary>
        /// <remarks>
        /// Messages are placed into the queue by the code in ReadDvIPacket.
        /// Retry loops and message processing are in the application layer.
        /// </remarks>
        public override void Receive()
        {
            ReadDVIPacket();
        }

        private bool ResetDevice()
        {
            //Send DVI reset
            byte[] Msg = OBDXProDevice.DVI_RESET.GetBytes();
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);
            System.Threading.Thread.Sleep(100);
            this.Port.DiscardBuffers();

            //Send ELM reset
            byte[] MsgATZ = { (byte)'A', (byte)'T', (byte)'Z', 0xD };
            this.Port.Send(MsgATZ);
            System.Threading.Thread.Sleep(100);
            this.Port.DiscardBuffers();

            //AT@1 will return OBDX Pro VT - will then need to change its API to DVI bytes.
            byte[] MsgAT1 = { (byte)'A', (byte)'T', (byte)'@', (byte)'1', 0xD };
            this.Port.Send(MsgAT1);
            Response<String> m = ReadELMPacket("AT@1");
            if (m.Status == ResponseStatus.Success) Logger("Device Found: " + m.Value);
            else { Logger("OBDX Pro device not found or failed response"); return false; }

            //Change to DVI protocol DX 
            byte[] MsgDXDP = { (byte)'D', (byte)'X', (byte)'D', (byte)'P', (byte)'1', 0xD };
            this.Port.Send(MsgDXDP);
            m = ReadELMPacket("DXDP1");
            if (m.Status == ResponseStatus.Success && m.Value == "OK") Debug.WriteLine("Switched to DVI protocol");
            else { Logger("Failed to switch to DVI protocol"); return false; }
            return true;
        }

        private Response<string> GetBoardDetails()
        {
            string Details = "";
            byte[] Msg = OBDXProDevice.DVI_BOARD_NAME.GetBytes();
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);

            Response<OBDMessage> m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value.GetBytes();
                Details = System.Text.Encoding.ASCII.GetString(Val, 3, Val[1] - 1);
                //  Logger("Device Found: " + name);
                // return new Response<String>(ResponseStatus.Success, name);
            }
            else
            {
                Logger("OBDX Pro device not found or failed response");
                return new Response<String>(ResponseStatus.Error, null);
            }


            //Firmware version
            Msg = OBDXProDevice.DVI_BOARD_FIRMWARE_VERSION.GetBytes();
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);
            m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value.GetBytes();
                string Firmware = ((float)(Val[3] * 0x100 + Val[4]) / 100).ToString("n2");
                Debug.WriteLine("Firmware version: v" + Firmware);
                Details += " - Firmware: v" + Firmware;
            }
            else
            {
                Logger("Unable to read firmware version");
                return new Response<String>(ResponseStatus.Error, null);
            }

            //Hardware version
            Msg = OBDXProDevice.DVI_BOARD_HARDWARE_VERSION.GetBytes();
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);
            m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value.GetBytes();
                string Hardware = ((float)(Val[3] * 0x100 + Val[4]) / 100).ToString("n2");
                Debug.WriteLine("Hardware version: v" + Hardware);
                Details += " - Hardware: v" + Hardware;
            }
            else
            {
                Logger("Unable to read hardware version");
                return new Response<String>(ResponseStatus.Error, null);
            }


            //Unique Serial
            Msg = OBDXProDevice.DVI_UniqueSerial.GetBytes();
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);
            m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value.GetBytes();
                byte[] serial = new byte[12];
                Array.Copy(Val, 3, serial, 0, 12);
                String Serial = string.Join("", Array.ConvertAll(serial, b => b.ToString("X2")));
                Debug.WriteLine("Unique Serial: " + Serial);
                Details += " - Unique Serial: " + Serial;
                return new Response<String>(ResponseStatus.Success, Details);
            }
            else
            {
                Logger("Unable to read unique Serial");
                return new Response<String>(ResponseStatus.Error, null);
            }
        }

        enum OBDProtocols : UInt16
        {
            VPW = 1
        }
        private bool SetProtocol(OBDProtocols val)
        {
            byte[] Msg = OBDXProDevice.DVI_Set_OBD_Protocol.GetBytes();
            Msg[3] = (byte)val;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);

            //get response
            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<OBDMessage> m = FindResponseFromTool(RespBytes);
            if (m.Status == ResponseStatus.Success)
            {
                Debug.WriteLine("OBD Protocol Set to VPW");
            }
            else
            {
                Logger("Unable to set OBDX Pro device to VPW mode");
                Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(Msg, b => b.ToString("X2"))));
                return false;
            }
            return true;
        }

        private bool SetToFilter(byte Val)
        {
            if (DataLogger.useBusFilters == false)
            {
                return true;
            }
            byte[] Msg = OBDXProDevice.DVI_Set_To_Filter.GetBytes();
            Msg[3] = Val; // DeviceId.Tool;
            Msg[4] = 1; //on
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<OBDMessage> response = ReadDVIPacket();
            if (response == null)
            {
                Debug.WriteLine("Null response to set filter, ignoring...");
                return true;
            }
            if (response.Status == ResponseStatus.Success & Utility.CompareArraysPart(response.Value.GetBytes(), RespBytes))
            {
                Debug.WriteLine("Filter set and enabled");
                return true;
            }
            else
            {

                Debug.WriteLine("Failed to set filter");
                return false;
            }
        }

        private bool RemoveToFilter()
        {
            byte[] Msg = OBDXProDevice.DVI_Set_RangeTo_Filter.GetBytes();
            Msg[4] = 0; //Range start
            Msg[5] = 0xFF; //Range end
            Msg[6] = 0; //0 = Off
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<OBDMessage> response = ReadDVIPacket();
            if (response.Status == ResponseStatus.Success )
            {
                Debug.WriteLine("Filters removed");
                return true;
            }
            else
            {

                Debug.WriteLine("Failed to remove filter");
                return false;
            }
        }

        private bool EnableProtocolNetwork()
        {
            byte[] Msg = OBDXProDevice.DVI_Set_NewtorkEnable.GetBytes();
            Msg[3] = 1; //on
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<OBDMessage> response = ReadDVIPacket();
            if (response.Status == ResponseStatus.Success & Utility.CompareArraysPart(response.Value.GetBytes(), RespBytes))
            {
                Debug.WriteLine("Network enabled");
                return true;
            }
            else
            {
                Debug.WriteLine("Failed to enable network");
                return false;
            }
        }

        /// <summary>
        /// Set the interface to 1x or 4x speed
        /// </summary>
        /// <remarks>
        /// The caller must also tell the PCM to switch speeds
        /// </remarks>
        protected override bool SetVpwSpeedInternal(VpwSpeed newSpeed)
        {

            byte[] Msg = OBDXProDevice.DVI_Set_Speed.GetBytes();

            if (newSpeed == VpwSpeed.Standard)
            {
                Debug.WriteLine("DVI setting VPW 1X");
                Msg[3] = 0;
                this.vpwSpeed = VpwSpeed.Standard;

            }
            else
            {
                Debug.WriteLine("DVI setting VPW 4X");
                Msg[3] = 1;
                this.vpwSpeed = VpwSpeed.FourX;
            }

            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<OBDMessage> m = FindResponseFromTool(RespBytes);
            if (m.Status != ResponseStatus.Success) return false;

            return true;
        }


        enum ConfigurationErrors : byte
        {
            InvalidCommand = 1,
            RecvTooLong = 2,
            ByteWaitTimeout = 3,
            InvalidSerialChksum = 4,
            SubCommandIncorrectSize = 5,
            InvalidSubCommand = 6,
            SubCommandInvalidData = 7
        }
        enum NetworkErrors : byte
        {
            ReadBusInactive = 0,
            ReadSOFLongerThenMax = 1,
            ReadSOFShorterThenMin = 2,
            Read2BytesOrLess = 3,
            ReadCRCIncorrect = 4,
            ReadFilterTOdoesNotMatch = 5,
            ReadFilterFROMdoesNotMatch = 6,
            ReadRangeFilterTOdoesNotMatch = 7,
            ReadRangeFilterFROMdoesNotMatch = 8,
            WriteFrameIdleFindTimeout = 9,
            NotEnabled = 10,
            ReadOnlyMode = 11,
            ReadFrameAwaitingSendPC = 12
        }
        enum ErrorType : byte
        {
            ConfigOrTxNetwork = 0,
            RxNetwork = 1
        }


        private void ProcessError(ErrorType Type, byte code)
        {
            string ErrVal = "";
            if (Type == ErrorType.ConfigOrTxNetwork)
            {
                switch (code)
                {
                    case (byte)ConfigurationErrors.InvalidCommand:
                        ErrVal = "Invalid command byte received";
                        break;
                    case (byte)ConfigurationErrors.RecvTooLong:
                        ErrVal = "Sent frame is larger then max allowed frame (4200)";
                        break;
                    case (byte)ConfigurationErrors.ByteWaitTimeout:
                        ErrVal = "Timeout occured waiting for byte to be received from PC";
                        break;
                    case (byte)ConfigurationErrors.InvalidSerialChksum:
                        ErrVal = "Invalid frame checksum sent to scantool";
                        break;
                    case (byte)ConfigurationErrors.SubCommandIncorrectSize:
                        ErrVal = "Sent command had incorrect length";
                        break;
                    case (byte)ConfigurationErrors.InvalidSubCommand:
                        ErrVal = "Invalid sub command detected";
                        break;
                    case (byte)ConfigurationErrors.SubCommandInvalidData:
                        ErrVal = "Invalid data detected for sub command";
                        break;
                }
            }
            else if (Type == ErrorType.RxNetwork)
            {

            }
            Debug.WriteLine("Fault reported from scantool: " + ErrVal);
        }

        public override void ClearMessageBuffer()
        {
            this.Port.DiscardBuffers();
        }

        /// <summary>
        /// This is based on the timeouts used by the AllPro, so it could probably be optimized further.
        /// </summary>
        private int GetReceiveTimeout()
        {
            int result;
            if (this.vpwSpeed == VpwSpeed.Standard)
            {
                switch (this.currentTimeoutScenario)
                {
                    case TimeoutScenario.Minimum:
                        result = 50;
                        break;

                    case TimeoutScenario.ReadProperty:
                        result = 50;
                        break;

                    case TimeoutScenario.ReadCrc:
                        result = 250;
                        break;

                    case TimeoutScenario.ReadMemoryBlock:
                        result = 250;
                        break;

                    case TimeoutScenario.EraseMemoryBlock:
                        result = 1000;
                        break;

                    case TimeoutScenario.WriteMemoryBlock:
                        result = 1200;
                        break;

                    case TimeoutScenario.SendKernel:
                        result = 4000;
                        break;

                    case TimeoutScenario.DataLogging1:
                        result = 25;
                        break;

                    case TimeoutScenario.DataLogging2:
                        result = 40;
                        break;

                    case TimeoutScenario.DataLogging3:
                        result = 60;
                        break;

                    case TimeoutScenario.DataLogging4:
                        result = 4500;
                        break;

                    case TimeoutScenario.Maximum:
                        result = 1020;
                        break;

                    default:
                        throw new NotImplementedException("Unknown timeout scenario " + this.currentTimeoutScenario);
                }
            }
            else
            {
                switch (this.currentTimeoutScenario)
                {
                    case TimeoutScenario.Minimum:
                        result = 50;
                        break;

                    case TimeoutScenario.ReadProperty:
                        result = 50;
                        break;

                    case TimeoutScenario.ReadCrc:
                        result = 250;
                        break;

                    case TimeoutScenario.ReadMemoryBlock:
                        result = 250;
                        break;

                    case TimeoutScenario.EraseMemoryBlock:
                        result = 1000;
                        break;

                    case TimeoutScenario.WriteMemoryBlock:
                        result = 600;
                        break;

                    case TimeoutScenario.SendKernel:
                        result = 2000;
                        break;

                    case TimeoutScenario.DataLogging1:
                        result = 7;
                        break;

                    case TimeoutScenario.DataLogging2:
                        result = 10;
                        break;

                    case TimeoutScenario.DataLogging3:
                        result = 15;
                        break;

                    case TimeoutScenario.DataLogging4:
                        result = 20;
                        break;

                    case TimeoutScenario.Maximum:
                        result = 1020;
                        break;

                    default:
                        throw new NotImplementedException("Unknown timeout scenario " + this.currentTimeoutScenario);
                }
            }

            return result;
        }

        /// <summary>
        /// Reads and filters a line from the device, returns it as a string
        /// </summary>
        /// <remarks>
        /// Strips Non Printable, >, and Line Feeds. Converts Carriage Return to Space. Strips leading and trailing whitespace.
        /// </remarks>
        private string ReadELMLine()
        {
            // (MaxReceiveSize * 2) + 2 for Checksum + longest possible prompt. Minimum prompt 2 CR + 1 Prompt, +2 extra
            int maxPayload = (MaxReceiveSize * 2) + 7;

            // A buffer to receive a single byte.
            SerialByte b = new SerialByte(1);

            // Use StringBuilder to collect the bytes.
            StringBuilder builtString = new StringBuilder();
            for (int i = 0; i < maxPayload; i++)
            {
                // Receive a single byte.
                Port.Receive(b, 0, 1);

                // Is it the prompt '>'.
                if (b.Data[0] == '>')
                {
                    // Prompt found, we're done.
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
            return builtString.ToString().Trim();
        }

        /// <summary>
        /// Send a request in string form, wait for a response (for init)
        /// </summary>
        /// <remarks>
        /// The API for this method (sending a string, returning a string) matches
        /// the way that we need to communicate with ELM and STN devices for setup
        /// </remarks>
        public string SendRequest(string request)
        {
            Debug.WriteLine("TX: " + request);

            try
            {
                DataLogger.port.Send(Encoding.ASCII.GetBytes(request + " \r"));
                Thread.Sleep(100);
                string response = ReadELMLine();
                return response;
            }
            catch (TimeoutException)
            {
                return string.Empty;
            }
        }

        public override bool SetLoggingFilter()
        {
            if (this.CurrentFilter == "logging")
                return true;
            Debug.WriteLine("Set logging filter");

            Initialize(BaudRate);

            this.CurrentFilter = "logging";
            return true;
        }

        public override bool SetAnalyzerFilter()
        {
            if (this.CurrentFilter == "analyzer")
                return true;
            Debug.WriteLine("Setting analyzer filter");
            byte[] Msg = new byte[] { 37, 0x0, 218 }; //Reset back to ELM protocol
            //byte[] Msg = OBDXProDevice.DVI_RESET.GetBytes();
            //Msg[Msg.Length - 1] = CalcChecksum(Msg);
            this.Port.Send(Msg);
            Thread.Sleep(300);
            //ClearMessageBuffer();
            //ClearMessageQueue();
                        //Debug.WriteLine(SendRequest("AT Z"));
                       //Thread.Sleep(100);
            Debug.WriteLine(SendRequest("AT L1")); //Enable new line characters between commands/messages
            Debug.WriteLine(SendRequest("AT SP2")); //Set protocol to VPW J1850
            Debug.WriteLine(SendRequest("AT H1")); //Enable headers
            Debug.WriteLine(SendRequest("AT AR")); //Clear filters
            ClearMessageBuffer();
            ClearMessageQueue();
            Debug.WriteLine(SendRequest("ATMA")); //Begin monitoring bus traffic
            this.CurrentFilter = "analyzer";
            return true;
        }

        public override bool RemoveFilters()
        {
            Debug.WriteLine("Removing filters");
            if (this.CurrentFilter == "analyzer")
            {
                Debug.WriteLine(SendRequest("AT L0")); //Disable new line characters between commands/messages
                byte[] MsgDXDP = { (byte)'D', (byte)'X', (byte)'D', (byte)'P', (byte)'1', 0xD };
                this.Port.Send(MsgDXDP);
                Response<string> m = ReadELMPacket("DXDP1");
                if (m.Status == ResponseStatus.Success && m.Value == "OK") Debug.WriteLine("Switched to DVI protocol");
                else { Logger("Failed to switch to DVI protocol"); return false; }
            }
            RemoveToFilter();
            return true;
        }

    }
}
