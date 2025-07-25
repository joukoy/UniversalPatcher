using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    public class WideBand
    {
        public enum WBType
        {
            None,
            AEM,
            PLX,
            Innovate,
            Elm327_CAN,
            Test
        }
        private enum PlxParserState
        {
            EXPECTING_START,
            EXPECTING_FIRST_HALF_OF_SENSOR_TYPE,
            EXPECTING_SECOND_HALF_OF_SENSOR_TYPE,
            EXPECTING_INSTANCE,
            EXPECTING_FIRST_HALF_OF_VALUE,
            EXPECTING_SECOND_HALF_OF_VALUE,

        }
        public double RAW { get { return raw; } }
        private double raw;
        private ushort CanId;
        private SerialPort port;
        private PlxParserState plxState;
        private int plxPartialValue = 0;
        private int plxSensorType = 0;
        private byte plxInstance = 0;
        private int innovatePos = 0;
        ushort innovateLambda = 0;
        //private List<DateTime> ValSetTimes = new List<DateTime>();
        Hertz hertz = new Hertz();

        public double AFR 
        { 
            get 
            {
                switch (AppSettings.Wbtype)
                {
                    case WBType.None:
                        return 0;
                    case WBType.AEM:
                        return raw;
                    case WBType.PLX:
                        return raw / 25.5 + 10;
                    case WBType.Innovate:
                        return (raw / 1000 + 0.5) * 14.7;
                    case WBType.Elm327_CAN:
                        return raw * AppSettings.WBCanAfrFactor;
                    case WBType.Test:
                        hertz.AddTime();
                        return raw;
                    default:
                        return 0;
                }
            }
        }

        public WideBand()
        {
            try
            {
                switch (AppSettings.Wbtype)
                {
                    case WBType.None:
                        break;
                    case WBType.AEM:
                        StartAem();
                        break;
                    case WBType.PLX:
                        StartPlx();
                        break;
                    case WBType.Innovate:
                        StartInnovate();
                        break;
                    case WBType.Elm327_CAN:
                        Task.Factory.StartNew(() => StartCAN());
                        break;
                    case WBType.Test:
                        raw = 14.1;
                        break;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, WideBand line " + line + ": " + ex.Message);
            }
        }

        public void Discard()
        {
            try
            {
                if (port != null )
                {
                    if (port.IsOpen)
                        port.Close();
                    port.Dispose();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, WideBand line " + line + ": " + ex.Message);
            }
        }
        public double Herz()
        {
            return hertz.GetHertz();
        }
        private bool OpenSerialPort(int BaudRate)
        {
            try
            {
                string[] pParts = AppSettings.WBSerial.Split(':');
                string PortName = pParts[0];
                this.port = new SerialPort(PortName);
                this.port.BaudRate = BaudRate;
                this.port.DataBits = 8;
                this.port.Parity = Parity.None;
                this.port.StopBits = StopBits.One;
                this.port.ReadTimeout = 1000;
                //For event handling:
                this.port.ReceivedBytesThreshold = 1;
                this.port.RtsEnable = true;
                this.port.Open();
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, WideBand line " + line + ": " + ex.Message);
                return false;
            }

        }
        private void StartAem()
        {
            if (string.IsNullOrEmpty(AppSettings.WBSerial))
            {
                return;
            }
            if (OpenSerialPort(9600))
            {
                this.port.ErrorReceived += Port_ErrorReceived;
                this.port.DataReceived += AemDataReceived;
            }
        }

        private void AemDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string aemStr = port.ReadLine();
                if (AppSettings.WBDebug)
                {
                    Debug.WriteLine("AEM: " + aemStr);
                }
                raw = Convert.ToDouble(aemStr, System.Globalization.CultureInfo.InvariantCulture);
                hertz.AddTime();
                //ValSetTimes.Add(DateTime.Now);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, WideBand line " + line + ": " + ex.Message);
            }
        }

        private void StartPlx()
        {
            if (string.IsNullOrEmpty(AppSettings.WBSerial))
            {
                return;
            }
            plxState = PlxParserState.EXPECTING_START;
            if (OpenSerialPort(19200))
            {
                this.port.ErrorReceived += Port_ErrorReceived;
                this.port.DataReceived += PlxDataReceived;
            }
        }

        private void PlxDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (port.BytesToRead > 0)
                {
                    int data = port.ReadByte();

                    if (data < 0)
                    {
                        Debug.WriteLine("PLX: Error reading byte from buffer");
                        return;
                    }
                    if (AppSettings.WBDebug)
                    {
                        Debug.WriteLine("PLX: State: " + plxState.ToString() + " received: " + data.ToString("X2"));
                    }
                    byte b = (byte)data;

                    if (b == 0x80)
                    {
                        plxState = PlxParserState.EXPECTING_FIRST_HALF_OF_SENSOR_TYPE;
                    }

                    if (b == 0x40)
                    {
                        plxState = PlxParserState.EXPECTING_START;
                    }

                    switch (plxState)
                    {
                        case PlxParserState.EXPECTING_FIRST_HALF_OF_SENSOR_TYPE:
                            plxState = PlxParserState.EXPECTING_SECOND_HALF_OF_SENSOR_TYPE;
                            plxPartialValue = b;
                            break;

                        case PlxParserState.EXPECTING_SECOND_HALF_OF_SENSOR_TYPE:
                            plxState = PlxParserState.EXPECTING_INSTANCE;
                            plxSensorType = (plxPartialValue << 6) | b;
                            break;

                        case PlxParserState.EXPECTING_INSTANCE:
                            plxState = PlxParserState.EXPECTING_FIRST_HALF_OF_VALUE;
                            plxInstance = b;
                            break;

                        case PlxParserState.EXPECTING_FIRST_HALF_OF_VALUE:
                            plxState = PlxParserState.EXPECTING_SECOND_HALF_OF_VALUE;
                            plxPartialValue = b;
                            break;

                        case PlxParserState.EXPECTING_SECOND_HALF_OF_VALUE:
                            plxState = PlxParserState.EXPECTING_FIRST_HALF_OF_SENSOR_TYPE;
                            raw = (plxPartialValue << 6) | b;
                            //ValSetTimes.Add(DateTime.Now);
                            hertz.AddTime();
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("PLX sensor : {0} instance : {1} value : {2} " + plxSensorType.ToString(), plxInstance.ToString(), raw.ToString());
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, WideBand line " + line + ": " + ex.Message);
            }
        }

        private void StartInnovate()
        {
            if (string.IsNullOrEmpty(AppSettings.WBSerial))
            {
                return;
            }
            if (OpenSerialPort(19200))
            {
                this.port.ErrorReceived += Port_ErrorReceived;
                this.port.DataReceived += InnovateDataReceived;
            }
        }

        private void InnovateDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (port.BytesToRead > 0)
                {
                    int data = port.ReadByte();

                    if (data < 0)
                    {
                        Debug.WriteLine("Innovate: Error reading byte from buffer");
                        return;
                    }
                    byte b0 = (byte)data;
                    if (innovatePos == 0)
                    {
                        if ((b0 & 178) == 178)
                        {
                            //Header high byte
                            innovatePos++;
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("Innovate: header high byte");
                            }
                        }
                    }
                    else if (innovatePos == 1)
                    {
                        if ((b0 & 128) == 128)
                        {
                            //Header low byte
                            innovatePos++;
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("Innovate: header low byte");
                            }
                        }
                        else
                        {
                            innovatePos = 0;
                        }

                    }
                    else if (innovatePos == 2 || innovatePos == 3)
                    {
                        if ((b0 & 128) == 128) innovatePos = 0; //Function/status word
                        else innovatePos++;
                    }
                    else if (innovatePos == 4)
                    {
                        //Data high byte
                        if ((b0 & 128) == 128) innovatePos = 0; //Function/status word
                        else
                        {

                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("Lambda byte0 " + b0.ToString("X2"));
                            }
                            innovateLambda = (ushort)((b0 & 63) << 7);
                            innovatePos++;
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("Innovate: data high byte\n");
                            }
                        }
                    }
                    else if (innovatePos == 5)
                    {
                        //Data low byte
                        if ((b0 & 128) == 128) innovatePos = 0;  //Function/status word
                        else
                        {
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("Lambda byte1 " + b0.ToString("X2"));
                            }
                            //lambda = lambda + b0;
                            innovateLambda = (ushort)(innovateLambda | b0);
                            raw = innovateLambda;
                            //ValSetTimes.Add(DateTime.Now);
                            hertz.AddTime();
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine("Innovate: data low byte");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, WideBand line " + line + ": " + ex.Message);
            }
        }

        static void SendCommand(SerialPort port, string command)
        {
            port.Write(command + "\r");
            while (port.BytesToRead == 0)
            {
                System.Threading.Thread.Sleep(10);
            }
            while (port.BytesToRead > 0)
            {
                string portdata = port.ReadExisting();
                if (AppSettings.WBDebug)
                {
                    Debug.Write(portdata);
                }

            }
        }
        private void StartCAN()
        {
            if (string.IsNullOrEmpty(AppSettings.WBSerial))
            {
                return;
            }
            if (port != null &&  port.IsOpen)
            {
                port.Close();
            }
            if (OpenSerialPort(AppSettings.WBCanBaudrate))
            {
                this.CanId = AppSettings.WBCanID;
                SendCommand(port, "ATZ");     // Reset
                SendCommand(port, "ATE0");    // Echo off
                SendCommand(port, "ATL1");    // Linefeed on
                SendCommand(port, "ATSP6");   // Set protocol to ISO 15765-4 (500 kbps)
                SendCommand(port, "ATSTFF");   // Set timeout to maximum
                SendCommand(port, "ATCRA" + CanId.ToString("X"));  //Receive only CanID frames
                SendCommand(port, "00");    // Read data
                this.port.ErrorReceived += Port_ErrorReceived;
                this.port.DataReceived += CAN_DataReceived; ;
            }
        }

        private void CAN_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = port.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (AppSettings.WBDebug)
                    {
                        Debug.WriteLine("CANWB Raw: " + line.Trim());
                    }
                    // Parse lines like: "810 04 0F A0 0F F8 ..." (hex values)
                    if (line.StartsWith(">"))
                    {
                        port.Write("00\r");
                    }
                    else 
                    {
                        //Example: 0: 74 0F 78 67 8F 35 (Not valid values)
                        int bytepos = AppSettings.WBCanBytePosition + 1; //Skip 0:
                        string[] parts = line.Trim().Split(' ');
                        if (parts.Length >= bytepos + 1)
                        {
                            raw = Convert.ToUInt16(parts[bytepos] + parts[bytepos+1], 16);
                            hertz.AddTime();
                            if (AppSettings.WBDebug)
                            {
                                Debug.WriteLine($"CANWB AFR: {AFR:F2}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, WideBand line " + line + ": " + ex.Message);
            }
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

    }
}
