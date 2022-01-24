using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates all code that is unique to the DVI interface.
    /// </summary>
    /// 
    public class OBDXProDevice
    {
        /// <summary>
        /// VPW can operate at two speeds. It is generally in standard (low speed) mode, but can switch to 4X (high speed).
        /// </summary>
        /// <remarks>
        /// High speed is better whend reading the entire contents of the PCM.
        /// Transitions to high speed must be negotiated, and any module that doesn't
        /// want to switch can force the bus to stay at standard speed. Annoying.
        /// </remarks>
        public enum VpwSpeed
        {
            /// <summary>
            /// 10.4 kpbs. This is the standard VPW speed.
            /// </summary>
            Standard,

            /// <summary>
            /// 41.2 kbps. This is the high VPW speed.
            /// </summary>
            FourX,
        }

        public const string DeviceType = "OBDX Pro";
        public bool TimeStampsEnabled = false;
        public bool CRCInReceivedFrame = false;
        private SerialPort Port;
        private int Timeout;

        /// <summary>
        /// Queue of messages received from the VPW bus.
        /// </summary>
        private Queue<byte[]> queue = new Queue<byte[]>();

        // This default is probably excessive but it should always be
        // overwritten by a call to SetTimeout before use anyhow.
        private VpwSpeed vpwSpeed = VpwSpeed.Standard;

        //ELM Command Set
        //To be put in, not really needed if using DVI command set

        //DVI (Direct Vehicle Interface) Command Set
        public static readonly byte[]DVI_BOARD_HARDWARE_VERSION = new byte[] { 0x22, 0x1, 0x0, 0 };
        public static readonly byte[]DVI_BOARD_FIRMWARE_VERSION = new byte[] { 0x22, 0x1, 0x1, 0 };
        public static readonly byte[]DVI_BOARD_MODEL = new byte[] { 0x22, 0x1, 0x2, 0 };
        public static readonly byte[]DVI_BOARD_NAME = new byte[] { 0x22, 0x1, 0x3, 0 };
        public static readonly byte[]DVI_UniqueSerial = new byte[] { 0x22, 0x1, 0x4, 0 };
        public static readonly byte[] DVI_Supported_OBD_Protocols = new byte[] { 0x22, 0x1, 0x5, 0 };
        public static readonly byte[]DVI_Supported_PC_Protocols = new byte[] { 0x22, 0x1, 0x6, 0 };

        public static readonly byte[]DVI_Req_NewtorkWriteStatus = new byte[] { 0x24, 0x1, 0x1, 0 };
        public static readonly byte[]DVI_Set_NewtorkWriteStatus = new byte[] { 0x24, 0x2, 0x1, 0, 0 };
        public static readonly byte[]DVI_Req_ConfigRespStatus = new byte[] { 0x24, 0x1, 0x2, 0 };
        public static readonly byte[] DVI_Set_CofigRespStatus = new byte[] { 0x24, 0x2, 0x2, 0, 0 };

        public static readonly byte[]DVI_Req_TimeStampOnRxNetwork = new byte[] { 0x24, 0x1, 0x3, 0 };
        public static readonly byte[]DVI_Set_TimeStampOnRxNetwork = new byte[] { 0x24, 0x2, 0x3, 0, 0 };

        public static readonly byte[]DVI_RESET = new byte[] { 0x25, 0x0, 0 };

        public static readonly byte[]DVI_Req_OBD_Protocol = new byte[] { 0x31, 0x1, 0x1, 0 };
        public static readonly byte[]DVI_Set_OBD_Protocol = new byte[] { 0x31, 0x2, 0x1, 0, 0 };
        public static readonly byte[]DVI_Req_NewtorkEnable = new byte[] { 0x31, 0x1, 0x2, 0 };
        public static readonly byte[]DVI_Set_NewtorkEnable = new byte[] { 0x31, 0x2, 0x2, 0, 0 };
        public static readonly byte[]DVI_Set_API_Protocol = new byte[] { 0x31, 0x2, 0x6, 0, 0 };

        public static readonly byte[]DVI_Req_To_Filter = new byte[] { 0x33, 0x1, 0x0, 0 };
        public static readonly byte[]DVI_Set_To_Filter = new byte[] { 0x33, 0x3, 0x0, 0, 0, 0 };
        public static readonly byte[]DVI_Req_From_Filter = new byte[] { 0x33, 0x1, 0x1, 0 };
        public static readonly byte[]DVI_Set_From_Filter = new byte[] { 0x33, 0x3, 0x1, 0, 0, 0 };
        public static readonly byte[]DVI_Req_RangeTo_Filter = new byte[] { 0x33, 0x1, 0x2, 0 };
        public static readonly byte[]DVI_Set_RangeTo_Filter = new byte[] { 0x33, 0x3, 0x2, 0, 0, 0, 0 };
        public static readonly byte[]DVI_Req_RangeFrom_Filter = new byte[] { 0x33, 0x1, 0x3, 0 };
        public static readonly byte[]DVI_Set_RangeFrom_Filter = new byte[] { 0x33, 0x4, 0x3, 0, 0, 0, 0 };
        public static readonly byte[]DVI_Req_Mask = new byte[] { 0x33, 0x1, 0x4, 0 };
        public static readonly byte[]DVI_Set_Mask = new byte[] { 0x33, 0x5, 0x4, 0, 0, 0, 0, 0 };
        public static readonly byte[]DVI_Req_Speed = new byte[] { 0x33, 0x1, 0x6, 0 };
        public static readonly byte[]DVI_Set_Speed = new byte[] { 0x33, 0x2, 0x6, 0, 0 };
        public static readonly byte[]DVI_Req_ValidateCRC_onRX = new byte[] { 0x33, 0x1, 0x7, 0 };
        public static readonly byte[]DVI_Set_ValidateCRC_onRX = new byte[] { 0x33, 0x2, 0x7, 0, 0 };
        public static readonly byte[]DVI_Req_Show_CRC_OnNetwork = new byte[] { 0x33, 0x1, 0x8, 0 };
        public static readonly byte[] DVI_Set_Show_CRC_OnNetwork = new byte[] { 0x33, 0x2, 0x8, 0, 0 };
        public static readonly byte[]DVI_Req_Write_Idle_Timeout = new byte[] { 0x33, 0x1, 0x9, 0 };
        public static readonly byte[]DVI_Set_Write_Idle_Timeout = new byte[] { 0x33, 0x3, 0x9, 0, 0, 0 };
        public static readonly byte[]DVI_Req_1x_Timings = new byte[] { 0x33, 0x2, 0xA, 0, 0 };
        public static readonly byte[]DVI_Set_1x_Timings = new byte[] { 0x33, 0x4, 0xA, 0, 0, 0, 0 };
        public static readonly byte[]DVI_Req_4x_Timings = new byte[] { 0x33, 0x2, 0xB, 0, 0 };
        public static readonly byte[]DVI_Set_4x_Timings = new byte[] { 0x33, 0x4, 0xB, 0, 0, 0, 0 };
        public static readonly byte[]DVI_Req_ResetTimings = new byte[] { 0x33, 0x2, 0xC, 0, 0 };
        public static readonly byte[]DVI_Req_ErrorBits = new byte[] { 0x33, 0x1, 0xD, 0 };
        public static readonly byte[]DVI_Set_ErrorBits = new byte[] { 0x33, 0x3, 0xD, 0, 0, 0 };
        public static readonly byte[]DVI_Req_ErrorCount = new byte[] { 0x33, 0x2, 0xE, 0, 0 };
        public static readonly byte[]DVI_Req_DefaultSettings = new byte[] { 0x33, 0x1, 0xF, 0 };

        public static readonly byte[]DVI_Req_ADC_SingleChannel = new byte[] { 0x35, 0x2, 0x0, 0, 0 };
        public static readonly byte[]DVI_Req_ADC_MultipleChannels = new byte[] { 0x35, 0x2, 0x1, 0, 0 };


        public OBDXProDevice(SerialPort port)
        {
            Port = port;
        }

        public void SendData(byte[] data)
        {
            Port.Write(data, 0, data.Length);
        }

        public string GetDeviceType()
        {
            return DeviceType;
        }

        public bool Initialize()
        {
            Logger("Initializing " + this.ToString());


            Port.BaudRate = 115200;
            Timeout = 1000;

            Port.Open();

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
            if (!Status)
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


            Logger("Device Successfully Initialized and Ready");
            
            return true;
        }

        public void Set4xSpeed()
        {
            if (SetVpwSpeedInternal(VpwSpeed.FourX))
            {
                Logger("Speed set to 4x");
            }
            else
            {
                Logger("Unable to set speed 4x");
            }

        }

        public void StartLogging()
        {
            Port.DataReceived += Port_DataReceived;
        }
        public void StopLogging()
        {
            Port.DataReceived -= Port_DataReceived;
            if (Port.IsOpen)
                Port.Close();
        }


        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            /*
            int bufLen = Port.BytesToRead;
            byte[] buf = new byte[bufLen];
            Port.Read(buf, 0, bufLen);
            Enqueue(buf);
            Debug.WriteLine("Read: " + BitConverter.ToString(buf));
            */
            ReadDVIPacket();
        }

        /// <summary>
        /// This will process incoming messages for up to 250ms looking for a message
        /// </summary>

        public Response<byte[]> FindResponseFromTool(byte[] expected)
        {
            //this.Logger.AddDebugMessage("FindResponse called");
            for (int iterations = 0; iterations < 5; iterations++)
            {
                Response <byte[]> response = ReadDVIPacket(Timeout);
                if (response != null)  // Hack to silence error - See: https://pcmhacking.net/forums/viewtopic.php?f=42&t=6730&start=110#p101790
                    if (response.Status == ResponseStatus.Success)
                        if (response.Value.SequenceEqual(expected))
                            return Response.Create(ResponseStatus.Success, response.Value);
                Thread.Sleep(50);
            }

            return Response.Create(ResponseStatus.Timeout,new byte[1]);
        }

        /// <summary>
        /// Wait for serial byte to be availble. False if timeout.
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
                if (Port.BytesToRead > TempCount)
                {
                    TempCount = Port.BytesToRead;
                    sw.Restart();
                }
                if (Port.BytesToRead >= NumBytes) { return true; }
                Thread.Sleep(10);
            }
            return false;
        }

        public byte[] ReadData_Old()
        {
            if (this.queue.Count == 0)
                return null;
            List<byte> retVal = new List<byte>();
            while(this.queue.Count > 0)
            {
                lock (this.queue)
                {
                    retVal.AddRange(this.queue.Dequeue());
                }
            }
            return retVal.ToArray();
        }

        public byte[] ReadData()
        {
            if (this.queue.Count == 0)
                return null;
            lock (this.queue)
            {
                return this.queue.Dequeue();
            }
        }

        protected void Enqueue(byte[] message)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(message);
            }
        }

        /// <summary>
        /// Read an DVI formatted packet from the interface.
        /// If it recevies a Network message, in enqueues it and returns null;
        /// If it receives a Device message, it returns the message.
        /// </summary>
        public Response<byte[]> ReadDVIPacket(int timeout = 0)
        {
            UInt16 Length = 0;

            byte offset = 0;
            byte[] rx = new byte[3]; // we dont read more than 3 bytes at a time
            byte[] timestampbuf = new byte[3];
            ulong timestampmicro = 0;
            // First Byte is command
            //Second is length, third also for long frame
            //Data
            //Checksum
            bool Chk = false;
            try
            {
                Chk = WaitForSerial(1, timeout);
                if (Chk == false)
                {
                    Logger("Timeout.. no data present A");
                    return Response.Create(ResponseStatus.Timeout, new byte[1]);
                }

                //get first byte for command
                Port.Read(rx, 0, 1);
            }
            catch (Exception) // timeout exception - log no data, return error.
            {
                Logger("No Data");
                return Response.Create(ResponseStatus.Timeout, new byte[1]);
            }


            if (rx[0] == 0x8 || rx[0] == 0x9) //for network frames
            {
                //check if timestamps enabled
                if (TimeStampsEnabled)
                {
                    //next 4 bytes will be timestamp in microseconds
                    for (byte i = 0; i < 4; i++)
                    {
                        Chk = WaitForSerial(1);
                        if (Chk == false)
                        {
                            Logger("Timeout.. no data present B");
                            return Response.Create(ResponseStatus.Timeout, new byte[1]);
                        }
                        Port.Read(timestampbuf, i, 1);
                    }
                    timestampmicro = (ulong)((ulong)timestampbuf[0] * 0x100 ^ 3) + (ulong)((ulong)timestampbuf[1] * 0x100 ^ 2) + (ulong)((ulong)timestampbuf[0] * 0x100) + (ulong)timestampbuf[0];
                }
                if (rx[0] == 0x8) //if short, only get one byte for length
                {
                    Chk = WaitForSerial(1);
                    if (Chk == false)
                    {
                        Logger("Timeout.. no data present C");
                        return Response.Create(ResponseStatus.Timeout, new byte[1]);
                    }
                    Port.Read(rx, 1, 1);
                    Length = rx[1];
                }
                else //if long, get two bytes for length
                {
                    offset += 1;
                    Chk = WaitForSerial(2);
                    if (Chk == false)
                    {
                        Logger("Timeout.. no data present D");
                        return Response.Create(ResponseStatus.Timeout,new byte[1]);
                    }
                    Port.Read(rx, 1, 2);
                    Length = (ushort)((ushort)(rx[1] * 0x100) + rx[2]);
                }

            }
            else //for all other received frames
            {
                Chk = WaitForSerial(1);
                if (Chk == false)
                {
                    Debug.WriteLine("Timeout.. no data present E");
                    return Response.Create(ResponseStatus.Timeout, new byte[1]);
                }
                Port.Read(rx, 1, 1);
                Length = rx[1];
            }

            byte[] receive = new byte[Length + 3 + offset];
            Chk =  WaitForSerial((ushort)(Length + 1));
            if (Chk == false)
            {
                Debug.WriteLine("Timeout.. no data present F");
                return Response.Create(ResponseStatus.Timeout, new byte[1]);
            }

            int bytes;
            receive[0] = rx[0];//Command
            receive[1] = rx[1];//length
            if (rx[0] == 0x09) receive[2] = rx[2];//length long frame
            bytes = Port.Read(receive, 2 + offset, Length + 1);//get rest of frame
            if (bytes <= 0)
            {
                Debug.WriteLine("Failed reading " + Length + " byte packet");
                return Response.Create(ResponseStatus.Error, new byte[1]);
            }
            //should have entire frame now
            //verify checksum correct
            byte CalcChksm = 0;
            for (ushort i = 0; i < (receive.Length - 1); i++) CalcChksm += receive[i];
            if (rx[0] == 0x08 || rx[0] == 0x09)
            {
                if (TimeStampsEnabled)
                {
                    CalcChksm += timestampbuf[0];
                    CalcChksm += timestampbuf[1];
                    CalcChksm += timestampbuf[2];
                    CalcChksm += timestampbuf[3];
                }
            }
            CalcChksm = (byte)~CalcChksm;

            if (receive[receive.Length - 1] != CalcChksm)
            {
                //Debug.WriteLine("Total Length Data=" + Length + " RX: " + receive.ToHex());
                Debug.WriteLine("Checksum error on received message.");
                return null;
            }

            // this.Logger.AddDebugMessage("Total Length Data=" + Length + " RX: " + receive.ToHex());

            if (receive[0] == 0x8 || receive[0] == 0x9)
            {
                //network frames //Strip header and checksum
                byte[] StrippedFrame = new byte[Length];
                Buffer.BlockCopy(receive, 2 + offset, StrippedFrame, 0, Length);
                Debug.WriteLine("RX: " + BitConverter.ToString(StrippedFrame));
                this.Enqueue(StrippedFrame);
                return null;
            }
            else if (receive[0] == 0x7F)
            {
                // Error from the device
                byte[] result = receive;
                Debug.WriteLine("XPro Error: " + result.ToString());
                return Response.Create(ResponseStatus.Error, result);
            }
            else
            {
                // Valid message from the device
                //this.Logger.AddDebugMessage("XPro: " + receive.ToHex());
                return Response.Create(ResponseStatus.Success, receive);
            }
        }


        private Response<String> ReadELMPacket(String SentFrame)
        {
            // UInt16 Counter = 0;
            bool framefound = false;
            bool Chk = false;

            string StrResp = "";
            byte[] rx = { 0 };
            try
            {
                while (framefound == false)
                {
                    Chk = WaitForSerial(1);
                    if (Chk == false)
                    {
                        Debug.WriteLine("Timeout.. no data present");
                        return Response.Create(ResponseStatus.Timeout, "");
                    }

                    Port.Read(rx, 0, 1);
                    if (rx[0] == 0xD) //carriage return
                    {
                        if (StrResp != SentFrame)
                        {
                            framefound = true;
                            break;
                        }
                        StrResp = "";
                        continue;
                    }
                    else if (rx[0] == 0xA) continue;//newline
                    StrResp += Convert.ToChar(rx[0]);
                }

                //Find Idle frame
                framefound = false;
                while (framefound == false)
                {
                    Chk =  WaitForSerial(1);
                    if (Chk == false)
                    {
                        Debug.WriteLine("ELM Idle frame not detected");
                        return Response.Create(ResponseStatus.Timeout, "");
                    }
                    Port.Read(rx, 0, 1);
                    if (rx[0] == '>')
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
        public byte CalcChecksum(byte[] MyArr)
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
        public Response<byte[]> SendDVIPacket(byte[] message, bool checkResponse = true)
        {
            int length = message.Length;
            byte[] RawPacket = message;
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
            SendData(SendPacket);

            if (!checkResponse)
                return Response.Create(ResponseStatus.Success, new byte[0]);

            // Wait for confirmation of successful send
            Response<byte[]> m = null;

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
                return Response.Create(ResponseStatus.Error, new byte[0]);
            }

            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value;
                if (Val[0] == 0x20 && Val[2] == 0x00)
                {
                    Debug.WriteLine("TX: " + BitConverter.ToString(message));
                    return Response.Create(ResponseStatus.Success, message);
                }
                else if (Val[0] == 0x21 && Val[2] == 0x00)
                {
                    Debug.WriteLine("TX: " + BitConverter.ToString(message));
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
            //Set filter
            bool Status = SetToFilter(DeviceId.Tool);
            if (Status == false) return Response.Create(ResponseStatus.Error, false);

            //Enable network rx/tx for protocol
            Status = EnableProtocolNetwork();
            if (Status == false) return Response.Create(ResponseStatus.Error, false);

            return Response.Create(ResponseStatus.Success, true);
        }


        private bool ResetDevice()
        {
            //Send DVI reset
            byte[] Msg = DVI_RESET;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);
            Application.DoEvents();
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            //Send ELM reset
            byte[] MsgATZ = { (byte)'A', (byte)'T', (byte)'Z', 0xD };
            SendData(MsgATZ);
            System.Threading.Thread.Sleep(100);
            Port.DiscardInBuffer();
            Port.DiscardOutBuffer();

            //AT@1 will return OBDX Pro VT - will then need to change its API to DVI bytes.
            byte[] MsgAT1 = { (byte)'A', (byte)'T', (byte)'@', (byte)'1', 0xD };
            SendData(MsgAT1);
            Response<String> m = ReadELMPacket("AT@1");
            if (m.Status == ResponseStatus.Success)
            {
                Logger("Device Found: " + m.Value);
            }
            else
            { 
                Logger("OBDX Pro device not found or failed response"); 
                return false; 
            }

            //Change to DVI protocol DX 
            byte[] MsgDXDP = { (byte)'D', (byte)'X', (byte)'D', (byte)'P', (byte)'1', 0xD };
            SendData(MsgDXDP);
            m = ReadELMPacket("DXDP1");
            if (m.Status == ResponseStatus.Success && m.Value == "OK")
            {
                Debug.Write("Switched to DVI protocol");
            }
            else 
            { 
                Logger("Failed to switch to DVI protocol"); 
                return false; 
            }
            return true;
        }

        private Response<string> GetBoardDetails()
        {
            string Details = "";
            byte[] Msg = OBDXProDevice.DVI_BOARD_NAME;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);

            Response<byte[]> m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value;
                Details = System.Text.Encoding.ASCII.GetString(Val, 3, Val[1] - 1);
                //  this.Logger.AddUserMessage("Device Found: " + name);
                // return new Response<String>(ResponseStatus.Success, name);
            }
            else
            {
                Logger("OBDX Pro device not found or failed response");
                return new Response<String>(ResponseStatus.Error, null);
            }


            //Firmware version
            Msg = OBDXProDevice.DVI_BOARD_FIRMWARE_VERSION;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);
            m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value;
                string Firmware = ((float)(Val[3] * 0x100 + Val[4]) / 100).ToString("n2");
                Logger("Firmware version: v" + Firmware);
                Details += " - Firmware: v" + Firmware;
            }
            else
            {
                Logger("Unable to read firmware version");
                return new Response<String>(ResponseStatus.Error, null);
            }

            //Hardware version
            Msg = OBDXProDevice.DVI_BOARD_HARDWARE_VERSION;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);
            m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value;
                string Hardware = ((float)(Val[3] * 0x100 + Val[4]) / 100).ToString("n2");
                Logger("Hardware version: v" + Hardware);
                Details += " - Hardware: v" + Hardware;
            }
            else
            {
                Logger("Unable to read hardware version");
                return new Response<String>(ResponseStatus.Error, null);
            }


            //Unique Serial
            Msg = OBDXProDevice.DVI_UniqueSerial;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);
            m = ReadDVIPacket();
            if (m.Status == ResponseStatus.Success)
            {
                byte[] Val = m.Value;
                byte[] serial = new byte[12];
                Array.Copy(Val, 3, serial, 0, 12);
                String Serial = string.Join("", Array.ConvertAll(serial, b => b.ToString("X2")));
                Logger("Unique Serial: " + Serial);
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
            byte[] Msg = OBDXProDevice.DVI_Set_OBD_Protocol;
            Msg[3] = (byte)val;
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);

            //get response
            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<byte[]> m = FindResponseFromTool(RespBytes);
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
            byte[] Msg = OBDXProDevice.DVI_Set_To_Filter;
            Msg[3] = Val; // DeviceId.Tool;
            Msg[4] = 1; //on
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<byte[]> response = ReadDVIPacket();
            if (response.Status == ResponseStatus.Success & response.Value.SequenceEqual(RespBytes))
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

        private bool EnableProtocolNetwork()
        {
            byte[] Msg = OBDXProDevice.DVI_Set_NewtorkEnable;
            Msg[3] = 1; //on
            Msg[Msg.Length - 1] = CalcChecksum(Msg);
            SendData(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<byte[]> response = ReadDVIPacket();
            if (response.Status == ResponseStatus.Success & response.Value.SequenceEqual(RespBytes))
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
        private bool SetVpwSpeedInternal(VpwSpeed newSpeed)
        {

            byte[] Msg = OBDXProDevice.DVI_Set_Speed;

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
            SendData(Msg);

            byte[] RespBytes = new byte[Msg.Length];
            Array.Copy(Msg, RespBytes, Msg.Length);
            RespBytes[0] += (byte)0x10;
            RespBytes[RespBytes.Length - 1] = CalcChecksum(RespBytes);
            Response<byte[]> m = FindResponseFromTool(RespBytes);
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

    }
}
