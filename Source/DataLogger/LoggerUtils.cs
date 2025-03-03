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

public static class LoggerUtils
{
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
        "921600"
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
        public string Units { get; set; }

        public string Expression { get; set; }

        public string Format { get; set; }

        public bool IsBitMapped { get; set; }

        public int BitIndex { get; set; }

        public string TrueValue { get; set; }

        public string FalseValue { get; set; }

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


}
