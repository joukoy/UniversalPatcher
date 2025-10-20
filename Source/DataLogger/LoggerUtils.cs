using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static UniversalPatcher.DataLogger;
using UniversalPatcher;
using System.Threading.Tasks;
using System.Diagnostics;
using J2534DotNet;
using System.IO;
using System.Threading;
using System.Drawing;
using static UniversalPatcher.PidConfig;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public static class LoggerUtils
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    [DllImport("kernel32.dll")]
    public static extern bool FreeLibrary(IntPtr hModule);

    public static List<OBDMessage> analyzerData { get; set; }

    public static Dictionary<byte, string> PcmResponses;
    public static List<RealTimeControl> RealTimeControls { get; set; }

    public static readonly string savedFiltersFile = Path.Combine(Application.StartupPath, "Logger", "savedfilters.xml");


    public enum ControlType
    {
        Slider,
        Set
    }

    public class RealTimeControl
    {
        public string Control { get; set; }
        public ControlType Controltype { get; set; }
        //public string Command { get; set; }
        public string CommandSet { get; set; }
        public string CommandClear { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string EnumValues { get; set; }
        public int Interval { get; set; }
    }


    public class Parameter
    {
        public Parameter()
        {
            Conversions = new List<Conversion>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Conversion> Conversions { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public int index { get; set; }
    }

    public class MathParameter
    {
        public MathParameter()
        {
            Conversions = new List<Conversion>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Conversion> Conversions { get; set; }
        public string Description { get; set; }
        public string xParameterId { get; set; }
        public string yParameterId { get; set; }
        public string xDataType { get; set; }
        public string yDataType { get; set; }
        public int index { get; set; }
    }

    public class RamParameter
    {
        public RamParameter()
        {
            Conversions = new List<Conversion>();
            Locations = new List<Location>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Conversion> Conversions { get; set; }
        public List<Location> Locations { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public int index { get; set; }
    }

    public class Location
    {
        public string os { get; set; }
        public string address { get; set; }
    }


    public enum FilterMode
    {
        None,
        Analyzer,
        Logging,
        Custom
    }

    public static readonly List<string> SupportedBaudRates = new List<string>
    {
        "300",
        "600",
        "1200",
        "2400",
        "4800",
        "9600",
        "19200",
        "38400",
        "57600",
        "115200",
        "230400",
        "460800",
        "500000",
        "921600",
        "1000000"
    };

    public class MsgFilter
    {
        private ProtocolID proto;
        public MsgFilter()
        {
            this.FilterName = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");
            FilterType = FilterType.PASS_FILTER;
            Mask = new PassThruMsg();
            Pattern = new PassThruMsg();
            FlowControl = new PassThruMsg();
        }
        public MsgFilter(JFilter jf)
        {
            FromString(jf.Filter);
            this.FilterName = jf.FilterName;
        }
        public MsgFilter(string Filters, ProtocolID Proto)
        {
            proto = Proto;
            FromString(Filters);
        }

        public string FilterName { get; set; }
        public FilterType FilterType { get; set; }
        public PassThruMsg Mask { get; set; }
        public PassThruMsg Pattern { get; set; }
        public PassThruMsg FlowControl { get; set; }

        private void FromString(string Filters)
        {
            Mask = new PassThruMsg();
            Pattern = new PassThruMsg();
            FlowControl = new PassThruMsg();
            PassThruMsg pMsg;
            string[] rows = Filters.Split(new char[] { '+', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                pMsg = new PassThruMsg();
                pMsg.ProtocolID = proto;
                string[] rowParts = row.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string setting in rowParts)
                {
                    string[] msgParts = setting.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (msgParts.Length == 2)
                    {
                        string prop = msgParts[0].ToLower().Trim();
                        string val = msgParts[1].ToUpper().Trim();
                        switch (prop)
                        {
                            case "type":
                                if (Enum.TryParse<FilterType>(val, out FilterType f))
                                {
                                    FilterType = f;
                                }
                                break;
                            case "name":
                                FilterName = val;
                                break;
                            case "mask":
                                pMsg.Data = val.Replace(" ", "").ToBytes();
                                Mask = pMsg;
                                break;
                            case "pattern":
                                pMsg.Data = val.Replace(" ", "").ToBytes();
                                Pattern = pMsg;
                                break;
                            case "flowcontrol":
                                pMsg.Data = val.Replace(" ", "").ToBytes();
                                FlowControl = pMsg;
                                break;
                            case "rxstatus":
                                pMsg.RxStatus = ParseRxStatus(val);
                                break;
                            case "txflags":
                                pMsg.TxFlags = ParseTxFLags(val);
                                break;
                        }
                    }
                }
            }
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Type:" + FilterType.ToString());
            if (FilterName != null)
                sb.Append(",Name:" + FilterName);
            sb.Append(Environment.NewLine);
            if (Mask.Data == null || Mask.Data.Length == 0)
                sb.Append("Mask:,");
            else
                sb.Append("Mask:" + BitConverter.ToString(Mask.Data).Replace("-", "") + ",");
            sb.Append("RxStatus:" + Mask.RxStatus.ToString().Replace(",", "|") + ",");
            sb.Append("TxFlags:" + Mask.TxFlags.ToString().Replace(",", "|") + Environment.NewLine);
            if (Pattern.Data == null || Pattern.Data.Length == 0)
                sb.Append("Pattern:,");
            else
                sb.Append("Pattern:" + BitConverter.ToString(Pattern.Data).Replace("-", "") + ",");
            sb.Append("RxStatus:" + Pattern.RxStatus.ToString().Replace(",", "|") + ",");
            sb.Append("TxFlags:" + Pattern.TxFlags.ToString().Replace(",", "|") + Environment.NewLine);
            if (FilterType == FilterType.FLOW_CONTROL_FILTER && FlowControl.Data != null && FlowControl.Data.Length > 0)
            {
                sb.Append("FlowControl:" + BitConverter.ToString(FlowControl.Data).Replace("-", "") + ",");
                sb.Append("RxStatus:" + FlowControl.RxStatus.ToString().Replace(",", "|") + ",");
                sb.Append("TxFlags:" + FlowControl.TxFlags.ToString().Replace(",", "|") + Environment.NewLine);
            }
            return sb.ToString();
        }

    }
    public class JFilter
    {
        public JFilter()
        {
            this.FilterName = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");
        }
        public JFilter(string Filter)
        {
            MsgFilter mf = new MsgFilter(Filter, ProtocolID.J1850VPW);
            this.FilterName = mf.FilterName;
            this.Filter = mf.ToString();
        }
        public JFilter(string FilterName, string Filter)
        {
            this.FilterName = FilterName;
            this.Filter = Filter;
        }
        public string FilterName { get; set; }
        public string Filter { get; set; }
    }

    public class JFilters
    {
        public List<MsgFilter> MessageFilters { get; set; }

        public JFilters()
        {
            MessageFilters = new List<MsgFilter>();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (MsgFilter mf in MessageFilters)
            {
                sb.Append(mf.ToString() + Environment.NewLine);
            }
            return sb.ToString();
        }

        public JFilters(string Filters, ProtocolID Proto)
        {
            MessageFilters = new List<MsgFilter>();
            PassThruMsg pMsg;
            MsgFilter msgfilter = new MsgFilter();
            string[] rows = Filters.Split(new char[] { '+','\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string row in rows)
            {
                pMsg = new PassThruMsg();
                pMsg.ProtocolID = Proto;
                string[] rowParts = row.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string setting in rowParts)
                {
                    string[] msgParts = setting.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (msgParts.Length == 2)
                    {
                        string prop = msgParts[0].ToLower().Trim();
                        string val = msgParts[1].ToUpper().Trim();
                        switch (prop)
                        {
                            case "type":
                                if (Enum.TryParse<FilterType>(val, out FilterType f))
                                {
                                    msgfilter = new MsgFilter();
                                    msgfilter.FilterType = f;
                                    MessageFilters.Add(msgfilter);
                                }
                                break;
                            case "name":
                                msgfilter.FilterName = val;
                                break;
                            case "mask":
                                pMsg.Data = val.Replace(" ", "").ToBytes();
                                msgfilter.Mask = pMsg;
                                break;
                            case "pattern":
                                pMsg.Data = val.Replace(" ", "").ToBytes();
                                msgfilter.Pattern = pMsg;
                                break;
                            case "flowcontrol":
                                pMsg.Data = val.Replace(" ", "").ToBytes();
                                msgfilter.FlowControl = pMsg;
                                break;
                            case "rxstatus":
                                pMsg.RxStatus = ParseRxStatus(val);
                                break;
                            case "txflags":
                                pMsg.TxFlags = ParseTxFLags(val);
                                break;
                        }
                    }
                }
            }
        }
    }

    public static TxFlag ParseTxFLags(string EnumString)
    {
        TxFlag retVal = TxFlag.NONE;
        string[] txs = EnumString.Split('|');
        foreach (string tx in txs)
        {
            if (Enum.TryParse<TxFlag>(tx, out TxFlag t))
            {
                retVal = retVal | t;
            }
        }
        return retVal;
    }

    public static RxStatus ParseRxStatus(string EnumString)
    {
        RxStatus retVal = RxStatus.NONE;
        string[] rxs = EnumString.Split('|');
        foreach (string rx in rxs)
        {
            if (Enum.TryParse<RxStatus>(rx, out RxStatus r))
            {
                retVal = retVal | r;
            }
        }
        return retVal;
    }

    public class Conversion
    {
        public string Units { get { return units; } set { if (string.IsNullOrEmpty(value)) units = "Raw"; else units = value; } }

        public string Expression { get; set; }

        public string Format { get; set; }

        public bool IsBitMapped { get; set; }

        public int BitIndex { get; set; }

        public string TrueValue { get; set; }

        public string FalseValue { get; set; }

        private string units;
        public Conversion()
        {
            this.IsBitMapped = false;
            this.BitIndex = -1;
            this.TrueValue = null;
            this.FalseValue = null;
        }
        public Conversion(string units, string expression, string format)
        {
            this.Units = units;
            this.Expression = Sanitize(expression);
            this.Format = format;
            this.IsBitMapped = false;
            this.BitIndex = -1;
            this.TrueValue = null;
            this.FalseValue = null;
        }

        public Conversion(string units, int bitIndex, string trueValue, string falseValue)
        {
            this.Units = units;
            this.Expression = "x";
            this.Format = "";
            this.IsBitMapped = true;
            this.BitIndex = bitIndex;
            this.TrueValue = trueValue;
            this.FalseValue = falseValue;
        }

        public override string ToString()
        {
            return this.Units;
        }

        /// <summary>
        /// The expression parser doesn't support bit-shift operators.
        /// So we hack them into division operators here.
        /// It's not pretty, but it's less ugly than changing the
        /// expressions in the XML file.
        /// </summary>
        private string Sanitize(string input)
        {
            int startIndex = input.IndexOf(">>");
            if (startIndex == -1)
            {
                return input;
            }

            int endIndex = startIndex;
            char shiftChar = ' ';
            for (int index = startIndex + 2; index < input.Length; index++)
            {
                endIndex = index;
                shiftChar = input[index];
                if (shiftChar == ' ')
                {
                    continue;
                }
                else
                {
                    endIndex++;
                    break;
                }
            }

            int shiftCount = shiftChar - '0';
            if (shiftCount < 0 || shiftCount > 15)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to parse >> operator in \"{0}\"", input));
            }

            string oldText = input.Substring(startIndex, endIndex - startIndex);
            string newText = string.Format("/{0}", Math.Pow(2, shiftCount));
            return input.Replace(oldText, newText);
        }
    }


        public static ProfileDataType ConvertToDataType(PidDataType pid)
        {
            switch (pid)
            {
                case PidDataType.uint8:
                    return ProfileDataType.UBYTE;
                case PidDataType.int8:
                    return ProfileDataType.SBYTE;
                case PidDataType.uint16:
                    return ProfileDataType.UWORD;
                case PidDataType.int16:
                    return ProfileDataType.SWORD;
                case PidDataType.uint32:
                    return ProfileDataType.UINT32;
                case PidDataType.int32:
                    return ProfileDataType.INT32;
            default:
                    return ProfileDataType.UBYTE;
            }
        }

    public static byte[] GetByteParameters(string byteStr)
    {
        List<byte> retVal = new List<byte>();
        try
        {
            string[] bParts = byteStr.Split(',');
            foreach (string bStr in bParts)
            {
                if (bStr.Contains("-"))
                {
                    string[] fParts = bStr.Split('-');
                    if (fParts.Length == 2)
                    {
                        if (HexToByte(fParts[0], out byte start) && HexToByte(fParts[1], out byte end))
                        {
                            for (int f = start; f <= end; f++)
                            {
                                retVal.Add((byte)f);
                            }
                        }
                        else
                        {
                            LoggerBold("Unknown address range: " + byteStr);
                            return null;
                        }
                    }
                    else
                    {
                        LoggerBold("Unknown address range: " + byteStr);
                        return null;
                    }
                }
                else
                {
                    retVal.AddRange(bStr.Replace(" ", "").ToBytes());
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
            LoggerBold("Error, LoggerUtils line " + line + ": " + ex.Message);
        }
        return retVal.ToArray();
    }

    public static byte CreateConfigByte(byte position, byte bytes, byte defineBy)
    {
        byte dBy = defineBy;
        if (dBy == 4) //Math
            dBy = 1;
        //byte pidbyte = 0b01000000; // 01xxxxxx = Pid
        //byte pos = (byte)(0xFF & (position << 3)); // xxXXXxxx Position of pid in response
        //return (byte)(pidbyte | pos | bytes);      //last 3 bits: data size (bytes)
        return (byte)((dBy << 6) | (position << 3) | bytes);
    }

    public static void LogExceptions(this Task task)
    {
        task.ContinueWith(t =>
        {
            var aggException = t.Exception.Flatten();
            foreach (var exception in aggException.InnerExceptions)
                Debug.WriteLine(exception);
        },
        TaskContinuationOptions.OnlyOnFaulted);
    }

    public static J2534InitParameters LoadJ2534Settings(string FileName)
    {
        try
        {
            Logger("Loading J2534 settings from file: " + FileName);
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(J2534InitParameters));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            J2534InitParameters JSettings = (J2534InitParameters)reader.Deserialize(file);
            file.Close();
            Logger("[OK]");
            return JSettings;
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, LoggerUtils line " + line + ": " + ex.Message);
            return null;
        }
    }

    public static List<J2534InitParameters> LoadJ2534SettingsList(string FileName)
    {
        try
        {
            Logger("Loading combined J2534 settings from file: " + FileName);
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<J2534InitParameters>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            List<J2534InitParameters> JSettings = (List<J2534InitParameters>)reader.Deserialize(file);
            file.Close();
            Logger("[OK]");
            return JSettings;
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, LoggerUtils line " + line + ": " + ex.Message);
            return null;
        }
    }

    public static void SaveJ2534Settings(string FileName, J2534InitParameters JSettings)
    {
        try
        {
            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(J2534InitParameters));
                writer.Serialize(stream, JSettings);
                stream.Close();
            }
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, SaveProfile line " + line + ": " + ex.Message +", " + ex.InnerException);
        }
    }

    public static void SaveJ2534SettingsList(string FileName, List<J2534InitParameters> JSettings)
    {
        try
        {
            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<J2534InitParameters>));
                writer.Serialize(stream, JSettings);
                stream.Close();
            }
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, SaveProfile line " + line + ": " + ex.Message + ", " + ex.InnerException);
        }
    }

    public static List<RealTimeControl> LoadRealTimeControls()
    {
        try
        {
            List<RealTimeControl> rtcs = new List<RealTimeControl>();
            if (File.Exists(AppSettings.ControlCommandsFile))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<RealTimeControl>));
                System.IO.StreamReader file = new System.IO.StreamReader(AppSettings.ControlCommandsFile);
                rtcs = (List<RealTimeControl>)reader.Deserialize(file);
                file.Close();
            }
            return rtcs;
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, LoggerUtils line " + line + ": " + ex.Message + ", " + ex.InnerException);
        }
        return new List<RealTimeControl>();
    }
    public static void initPcmResponses()
    {
        PcmResponses = new Dictionary<byte, string>();
        PcmResponses.Add(0x00, "Affirmative Response");
        PcmResponses.Add(0x10, "General Reject");
        PcmResponses.Add(0x11, "Mode Not Supported");
        PcmResponses.Add(0x12, "Sub-Function Not Supported or Invalid format");
        PcmResponses.Add(0x21, "Busy - Repeat Request");
        PcmResponses.Add(0x22, "Conditions Not Correct or Request Sequence Error");
        PcmResponses.Add(0x23, "Routine Not Complete");
        PcmResponses.Add(0x31, "Request Out of Range");
        PcmResponses.Add(0x33, "Security Access Denied");
        PcmResponses.Add(0x34, "Security Access Allowed");
        PcmResponses.Add(0x35, "Invalid Key");
        PcmResponses.Add(0x36, "Exceed Number of Attempts");
        PcmResponses.Add(0x37, "Required Time Delay Not Expired");
        PcmResponses.Add(0x40, "Download Not Accepted");
        PcmResponses.Add(0x41, "Improper Download Type");
        PcmResponses.Add(0x42, "Can't Download to Specified Address");
        PcmResponses.Add(0x43, "Can't Download Number of Bytes Requested");
        PcmResponses.Add(0x44, "Ready for Download");
        PcmResponses.Add(0x50, "Upload not Accepted");
        PcmResponses.Add(0x51, "Improper Upload Type");
        PcmResponses.Add(0x52, "Can't Upload from Specified Address");
        PcmResponses.Add(0x53, "Can't Upload Number of Bytes Requested");
        PcmResponses.Add(0x54, "Ready for Upload");
        PcmResponses.Add(0x61, "Normal Exit with Results Available");
        PcmResponses.Add(0x62, "Normal Exit without Results Available");
        PcmResponses.Add(0x63, "Abnormal Exit with Results");
        PcmResponses.Add(0x64, "Abnormal Exit without Results");
        PcmResponses.Add(0x71, "Transfer Suspended");
        PcmResponses.Add(0x72, "Transfer Aborted");
        PcmResponses.Add(0x73, "Block Transfer Complete/Next BLock");
        PcmResponses.Add(0x74, "Illegal Address in Block Transfer");
        PcmResponses.Add(0x75, "Illegal Byte Count in Block Transfer");
        PcmResponses.Add(0x76, "Illegal Block Tranfser Type");
        PcmResponses.Add(0x77, "Block Transfer Data Checksum Error");
        PcmResponses.Add(0x78, "Block Transfer Message Correctly Received");
        PcmResponses.Add(0x79, "Incorrect Byte Count During Block Transfer");
    }

    public static bool IsRunningUnderWine()
    {
        try
        {
            IntPtr m_pDll = LoadLibrary("ntdll.dll");
            IntPtr FuncAddr = GetProcAddress(m_pDll, "wine_get_version");
            if (FuncAddr != IntPtr.Zero)
            {
                //wine_get_version available only in Wine
                Debug.WriteLine("Running in Wine");
                FreeLibrary(m_pDll);
                return true;
            }
            else
            {
                Debug.WriteLine("Running in Windows");
                FreeLibrary(m_pDll);
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("IsRunningUnderWine Error " + ex.Message);
            return false;
        }
    }

    public static List<CANDevice> SearchAllDevicesOnBus(Device LogDevice)
    {
        List<CANDevice> retVal = new List<CANDevice>();
        List<CANDevice> CanNodes = new List<CANDevice>();
        Logger("Query devices on bus, High Speed CAN");
        CanNodes = QueryConnectedCanDevices(false, LoggingProtocol.HSCAN, LogDevice);
        if (CanNodes.Count > 0)
        {
            retVal.AddRange(CanNodes);
            Logger("Found " + CanNodes.Count.ToString() + " nodes (HSCAN)");
        }
        Logger("Query devices on bus, Low Speed CAN");
        //DisconnectCan();
        CanNodes = QueryConnectedCanDevices(false, LoggingProtocol.LSCAN, LogDevice);
        if (CanNodes.Count > 0)
        {
            retVal.AddRange(CanNodes);
            Logger("Found " + CanNodes.Count.ToString() + " nodes (LSCAN)");
        }

        Logger("Query devices on bus, VPW");
        CanNodes = QueryConnectedCanDevices(false, LoggingProtocol.VPW, LogDevice);
        if (CanNodes.Count > 0)
        {
            retVal.AddRange(CanNodes);
            Logger("Found " + CanNodes.Count.ToString() + " nodes (VPW)");
        }


        Logger("Found " + retVal.Count.ToString() + " nodes (total)");

        return retVal;
    }

    public static List<CANDevice> QueryConnectedCanDevices(bool RunExtra, LoggingProtocol protocol, Device LogDevice)
    {
        int[] ExpectedMsg;
        List<CANDevice> retVal = new List<CANDevice>();
        try
        {
            Logger("Query CAN devices on bus");
            if (LogDevice != null && LogDevice.Connected)
            {
                //LogDevice.Disconnect();
                Logger("Disconnecting previous connection");
                LogDevice.PassthruDisconnect();
            }
            if (protocol == LoggingProtocol.HSCAN)
            {
                J2534InitParameters jParms = new J2534InitParameters();
                jParms.Protocol = ProtocolID.CAN;
                jParms.Baudrate = BaudRate.ISO15765_500000;
                jParms.Connectflag = ConnectFlag.NONE;
                jParms.PassFilters = "Type:PASS_FILTER,Name:PreUtil1" + Environment.NewLine;
                jParms.PassFilters += "Mask: 0000FFE0,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                jParms.PassFilters += "Pattern:00000640" + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                if (!LogDevice.PassthruConnect(jParms))
                {
                    LoggerBold("Error connecting");
                    return retVal;
                }
                Application.DoEvents();
                //Use manual keepalive in preutil
                //string pMsg = "00 00 01 01 FE 3E:1600:ISO15765_FRAME_PAD|ISO15765_ADDR_TYPE";
                //LogDevice.StartPeriodicMsg(pMsg, false);
                //LogDevice.SetWriteTimeout(AppSettings.TimeoutJ2534Write);
                //Filter2
                string filter2 = "Type:PASS_FILTER,Name:PreUtil1" + Environment.NewLine +
                    "Mask: 0000FFF8,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine +
                    "Pattern:0000" + datalogger.CanDevice.ResID.ToString("X4") + ",RxStatus: NONE,TxFlags: NONE";
                int[] fltrs = LogDevice.SetupFilters(filter2, false, false);
                if (fltrs == null || fltrs.Length == 0)
                {
                    return retVal;
                }
            }
            else if (protocol == LoggingProtocol.LSCAN)
            {
                J2534InitParameters jParms = new J2534InitParameters();
                jParms.Protocol = ProtocolID.SW_CAN_PS;
                jParms.Baudrate = BaudRate.ISO15765_33K3;
                jParms.Connectflag = ConnectFlag.NONE;
                jParms.Sconfigs = "J1962_PINS=$00000100";
                jParms.PassFilters = "Type:PASS_FILTER,Name:PreUtil1" + Environment.NewLine;
                jParms.PassFilters += "Mask: 0000FFE0,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                jParms.PassFilters += "Pattern:00000640" + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                if (!LogDevice.PassthruConnect(jParms))
                {
                    LoggerBold("Error connecting");
                    return retVal;
                }
                //LogDevice.SetJ2534Configs("J1962_PINS=$00000100", false);
                //NO mixed format for SW_CAN_PS
                //LogDevice.SetJ2534Configs("CAN_MIXED_FORMAT=1", false);
                /*
                string PassFilters = "Type:PASS_FILTER,Name:PreUtil1" + Environment.NewLine;
                PassFilters += "Mask: 0000FFE0,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                PassFilters += "Pattern:00000640" + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                int[] fltrs = LogDevice.SetupFilters(PassFilters, false, true);
                if (fltrs == null || fltrs.Length == 0)
                {
                    return retVal;
                }
                */
                //LogDevice.SetJ2534Configs("CAN_MIXED_FORMAT=1", false);
                byte[] preMsg = new byte[] { 0x00, 0x00, 0x01, 0x00 };
                OBDMessage preObd = new OBDMessage(preMsg);
                preObd.Txflag = TxFlag.SW_CAN_HV_TX;
                if (LogDevice.SendMessage(preObd, 0))
                {
                    Thread.Sleep(500);
                    preMsg = new byte[] { 0x00, 0x00, 0x01, 0x01, 0xFD, 0x02, 0x10, 0x04, 0x00, 0x00, 0x00, 0x00 };
                    preObd = new OBDMessage(preMsg);

                    LogDevice.SendMessage(preObd, 1);
                    ExpectedMsg = new int[] { -1 };
                    byte[] response = ReceiveResponse(LogDevice,ExpectedMsg, ": ", true, true, false, true);
                    if (response == null)
                    {
                        Logger("No response with SW_CAN_HV_RX");
                        return retVal;
                    }
                }
                else
                {
                    return retVal;
                }
            }
            else if (protocol == LoggingProtocol.VPW)
            {
                J2534InitParameters jParms = new J2534InitParameters();
                jParms.Protocol = ProtocolID.J1850VPW;
                jParms.Baudrate = BaudRate.J1850VPW;
                jParms.Connectflag = ConnectFlag.NONE;
                if (!LogDevice.PassthruConnect(jParms))
                {
                    LoggerBold("Error connecting");
                    return retVal;
                }
                LogDevice.SetAnalyzerFilter();
                Response<List<byte>> modules = datalogger.QueryDevicesOnBus(true);
                for (int m = 0; m < modules.Value.Count; m++)
                {
                    byte b = modules.Value[m];
                    CANDevice vpwDev = new CANDevice();
                    vpwDev.ModuleID = b;
                    vpwDev.Subnet = LoggingProtocol.VPW;
                    if (analyzer.PhysAddresses.ContainsKey(b))
                    {
                        vpwDev.ModuleName = analyzer.PhysAddresses[b];
                    }
                    else
                    {
                        vpwDev.ModuleName = b.ToString("X2");
                    }
                    retVal.Add(vpwDev);
                }
                return retVal;

            }

            Logger("Sending module iD command 1a B0");
            byte[] Msg = new byte[] { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x02, 0x1A, 0xB0, 0x00, 0x00, 0x00, 0x00 };
            OBDMessage oMsg = new OBDMessage(Msg);
            if (!LogDevice.SendMessage(oMsg, 3))
            {
                return retVal;
            }
            ExpectedMsg = new int[] { 0x00, 0x00, 0x07, 0xE8, 0x03, 0x5A, 0xB0, -1, 0xAA, 0xAA, 0xAA, 0xAA };
            while (true)
            {
                byte[] response = ReceiveResponse(LogDevice,ExpectedMsg, "Query CAN devices on bus: ", true, true);
                if (response != null && response.Length > 7 && response[5] == 0x5A)
                {
                    //CANDevice device = CanModules.Where(X => X.ModuleID == response[7]).FirstOrDefault();
                    CANDevice device = CanModules.Where(X => X.ResID == (response[2] << 8 | response[3])).FirstOrDefault();
                    if (device != null)
                    {
                        CANDevice newDev = device.ShallowCopy();
                        newDev.Subnet = protocol;
                        CANDevice prevDev = retVal.Where(X => X.ModuleID == response[7] && X.Subnet == newDev.Subnet).FirstOrDefault();
                        if (prevDev == null)
                        {
                            retVal.Add(newDev);
                            Logger("Response from: " + device.ModuleName + " [" + device.ModuleID.ToString("X2") + "][" + device.ResID.ToString("X4") + "]");
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            if (retVal.Count == 0 && protocol == LoggingProtocol.LSCAN)
            {
                Logger("No answer, setting new filter");
                string PassFilters = "Type:PASS_FILTER,Name:PreUtil1" + Environment.NewLine;
                PassFilters += "Mask: 00000700,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                PassFilters += "Pattern:00000300" + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                LogDevice.SetupFilters(PassFilters, false, true);
                Msg = new byte[] { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x01, 0xA2, 0x00, 0x00, 0x00, 0x00, 0x00 };
                oMsg = new OBDMessage(Msg);
                LogDevice.SendMessage(oMsg, 3);
                ExpectedMsg = new int[] { 0x00, 0x00, 0x03, 0x60, 0x02, 0xE2, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };
                byte[] response = ReceiveResponse(LogDevice, ExpectedMsg, "Query CAN devices on bus: ", true, true);
                if (response != null && response.Length > 7 && response[5] == 0xE2)
                {
                    Logger("Answer ok");
                }
                else
                {
                    Logger("Unknown answer, continue anyway??");
                }

                Logger("Sending module iD command 1a B0 (Second try)");
                Msg = new byte[] { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x02, 0x1A, 0xB0, 0x00, 0x00, 0x00, 0x00 };
                oMsg = new OBDMessage(Msg);
                if (LogDevice.SendMessage(oMsg, 0x03))
                {
                    ExpectedMsg = new int[] { 0x00, 0x00, 0x07, 0xE8, 0x03, 0x5A, 0xB0, -1, 0xAA, 0xAA, 0xAA, 0xAA };
                    while (true)
                    {
                        byte[] resp = ReceiveResponse(LogDevice, ExpectedMsg, "Query CAN devices on bus: ", true, true);
                        if (resp != null && resp.Length > 7 && resp[5] == 0x5A)
                        {
                            //CANDevice device = CanModules.Where(X => X.ModuleID == response[7]).FirstOrDefault();
                            CANDevice device = CanModules.Where(X => X.ResID == (resp[2] << 8 | resp[3])).FirstOrDefault();
                            if (device != null)
                            {
                                CANDevice newDev = device.ShallowCopy();
                                newDev.Subnet = LoggingProtocol.LSCAN;
                                CANDevice prevDev = retVal.Where(X => X.ModuleID == resp[7] && X.Subnet == newDev.Subnet).FirstOrDefault();
                                if (prevDev == null)
                                {
                                    retVal.Add(newDev);
                                    Logger("Response from: " + device.ModuleName + " [" + device.ModuleID.ToString("X2") + "][" + device.ResID.ToString("X4") + "]");
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    return retVal;
                }

            }
            return retVal;
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("LoggerUtils, line " + line + ": " + ex.Message);
        }
        return retVal;
    }

    public static  byte[] ReceiveResponse(Device LogDevice, int[] ExpectedMsg, string question, bool Preutil, bool BlockMsg, bool StopOn78 = false, bool waifor_HV_RX = false)
    {
        byte[] retVal = null;
        try
        {
            int ReadTimeout = 300;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ExpectedMsg.Length; i++)
            {
                if (ExpectedMsg[i] == -1)
                {
                    sb.Append("XX");
                }
                else if (ExpectedMsg[i] == -2)
                {
                    sb.Append("(XX)");
                }
                else if (ExpectedMsg[i] == -3)
                {
                    sb.Append("ID" + i.ToString());
                }
                else
                {
                    sb.Append(ExpectedMsg[i].ToString("X2"));
                }
                sb.Append(" ");
            }
            Logger("Expecting value: " + sb.ToString().Trim());
            DateTime startTime = DateTime.Now;
            bool done = false;
            retVal = new byte[0];
            byte statusPos = 4;
            if (Preutil)
            {
                statusPos = 5;
            }
            do
            {
                OBDMessage oMsg = LogDevice.ReceiveMessage(true);
                if (oMsg != null && oMsg.Length > statusPos)
                {
                    byte statusByte = (byte)ExpectedMsg[statusPos];
                    byte[] resp = oMsg.GetBytes();
                    retVal = resp;
                    if (waifor_HV_RX)
                    {
                        if (oMsg.Rxstatus == J2534DotNet.RxStatus.SW_CAN_HV_RX || AppSettings.LoggerJ2534Device.ToLower().Contains("sim"))
                        {
                            done = true;
                            break;
                        }
                    }
                    else if (resp[statusPos] == statusByte)
                    {
                        BlockMsg = false;
                        if (statusByte == 0x76 && resp.Length > statusPos + 2)
                        {
                            if (resp[statusPos + 2] == 0x78 || resp[statusPos + 2] == 0x6F)
                            {
                                if (StopOn78)
                                {
                                    done = true;
                                    break;
                                }
                                else
                                {
                                    Debug.WriteLine("Device busy, waiting for more messages");
                                    startTime = DateTime.Now;
                                    Thread.Sleep(100);
                                }
                            }
                            else if (resp[statusPos + 2] == 0x65 || resp[statusPos + 2] == 0x73 || resp[statusPos + 2] == 0x86)
                            {
                                Debug.WriteLine("Receive success (" + resp[statusPos + 2].ToString("X2") + ")");
                                done = true;
                                break;
                            }
                            else
                            {
                                Debug.WriteLine("Receive error (" + resp[statusPos + 2].ToString("X2") + ")");
                                done = true;
                                break;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Receive success (" + resp[statusPos].ToString("X2") + ")");
                            done = true;
                            break;
                        }
                    }
                    else if (resp[statusPos] == 0x7F)
                    {
                        BlockMsg = false;
                        if (resp.Length > statusPos + 2 && (resp[statusPos + 2] == 0x78 || resp[statusPos + 2] == 0x23))
                        {
                            Debug.WriteLine("Device busy, waiting for more messages (" + resp[statusPos + 2].ToString("X2") + ")");
                            startTime = DateTime.Now;
                            Thread.Sleep(100);
                        }
                        else if (resp.Length > statusPos + 2)
                        {
                            byte errByte = resp[statusPos + 2];
                            Debug.WriteLine("Device returned error " + errByte.ToString("X2") + ": " + PcmResponses[errByte]);
                        }
                        else
                        {
                            Debug.WriteLine("Device returned error, stop listening");
                            done = true;
                            break;
                        }
                    }
                }
                if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(ReadTimeout))
                {
                    Logger("Timeout [" + ReadTimeout.ToString() + "]  waiting for: " + sb.ToString());
                    break;
                }
            } while (!done);
            return retVal;
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Logger("Error, LoggerUtils line " + line + ": " + ex.Message);
            return null;
        }
    }

}
