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

public static class LoggerUtils
{
    public static List<OBDMessage> analyzerData { get; set; }
    public static Dictionary<byte, string> PcmResponses;

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

    public enum DefineBy
    {
        //Offset = 0,
        Pid = 1,
        Address = 2,
        Proprietary = 3,
        Math = 4
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
        public string Units { get; private set; }

        public string Expression { get; private set; }

        public string Format { get; private set; }

        public bool IsBitMapped { get; private set; }

        public int BitIndex { get; private set; }

        public string TrueValue { get; private set; }

        public string FalseValue { get; private set; }

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


        public enum ProfileDataType
        {
            UBYTE = 0,              //UNSIGNED INTEGER - 8 BIT
            SBYTE = 1,              //SIGNED INTEGER - 8 BIT
            UWORD = 2,              //UNSIGNED INTEGER - 16 BIT
            SWORD = 3,              //SIGNED INTEGER - 16 BIT
            INT32 = 4,              //SIGNED DWORD - 32 bit
            UINT32 = 5,             //UNSIGNED DWORD - 32 bit
            //THREEBYTES = 6,
            UNKNOWN
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

    public class PidConfig
    {
        public PidConfig()
        {
            DataType = ProfileDataType.UBYTE;
            addr2 = -1;
            Math = "X";
        }
        public string PidName { get; set; }
        public DefineBy Type { get; set; }
        public string Address
        {
            get
            {
                if (addr < 0)
                {
                    return "-" + (-1 * addr).ToString("X4");
                }
                else
                {
                    return addr.ToString("X4");
                }
            }
            set
            {
                HexToInt(value.Replace("0x", "").Replace("-", ""), out addr);
                if (value.StartsWith("-"))
                {
                    addr = -1 * addr;
                }
                else
                {
                    if (Type == DefineBy.Address)
                    {
                        addr |= 0xFF0000;
                    }
                }
            }
        }
        public string Address2
        {
            get
            {
                if (addr2 < 0)
                    return "-" + (-1 * addr2).ToString("X4");
                else
                    return addr2.ToString("X4");
            }
            set
            {
                HexToInt(value.Replace("0x", "").Replace("-", ""), out addr2);
                if (value.StartsWith("-"))
                    addr2 = -1 * addr2;
            }
        }
        public ProfileDataType DataType { get; set; }
        public ProfileDataType Pid2DataType { get; set; }
        //public OutDataType OutDataType { get; set; }
        public bool HighPriority { get; set; }
        public string Math { get; set; }
        public string Units { get; set; }
        //public int index { get; set; }
        public bool IsBitMapped { get; set; }
        public int BitIndex { get; set; }
        public int addr;
        public int addr2;

        public string GetBitmappedValue(double value)
        {
            string trueVal = "On";
            string falseVal = "Off";
            string[] vals = Math.Split(',');
            if (vals.Length > 1)
            {
                trueVal = vals[0];
                falseVal = vals[1];
            }
            int bits = (int)value;
            bits = bits >> BitIndex;
            bool flag = (bits & 1) != 0;
            string retVal = falseVal;
            if (flag)
                retVal = trueVal;
            //Debug.WriteLine("Bitmapped: " + retVal);
            return retVal;
        }
    
        public bool GetBitmappedBoolValue(double value)
        {
            //string[] vals = Math.Split(',');
            int bits = (int)value;
            bits = bits >> BitIndex;
            bool flag = (bits & 1) != 0;
            return flag;
        }

        public static string ParseEnumValue(string eVals, double value1)
        {
            if (eVals.ToLower().StartsWith("enum:"))
                eVals = eVals.Substring(5).Trim();
            List<double> enumVals = new List<double>();
            string[] posVals = eVals.Split(',');
            bool hexVals = false;
            for (int r = 0; r < posVals.Length; r++)
            {
                string[] parts = posVals[r].Split(':');
                if (parts[0].StartsWith("$") || (!double.TryParse(parts[0], out double _d) && HexToUint64(parts[0], out UInt64 _u)))
                {
                    hexVals = true;
                    break;
                }
            }

            for (int r = 0; r < posVals.Length; r++)
            {
                string[] parts = posVals[r].Split(':');
                double val = 0;
                if (parts[0].StartsWith("$") || hexVals)
                {
                    if (HexToUint64(parts[0].Replace("$", ""), out UInt64 uVal))
                        val = (double)uVal;
                }
                else
                {
                    double.TryParse(parts[0], out val);
                }
                string txt = posVals[r];
                if (!enumVals.Contains(val))
                    enumVals.Add(val);
                if (val == value1)
                    return txt;
            }
            return "";
        }

        private bool GetBitStatus(double value, int bitposition)
        {
            int bits = (int)value;
            bits = bits >> bitposition;
            bool flag = (bits & 1) != 0;
            return flag;
        }

        public string ParseBitsValue(double value1)
        {
            string retVal = "";
            string trueVal = "On";
            string falseVal = "Off";
            string[] doubleBitVals = { "fail", "indeterminate", "indeterminate", "pass" };

            if (Math.Contains("(") && Math.Contains(")"))
            {
                int pos1 = Math.IndexOf("(") + 1;
                int pos2 = Math.IndexOf(")",pos1);
                string onOff = Math.Substring(pos1, pos2 - pos1);
                string[] onOffParts = onOff.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (onOffParts.Length < 2)
                    onOffParts = onOff.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (onOffParts.Length > 1)
                {
                    foreach(string s1 in onOffParts)
                    {
                        string s = s1.Trim();
                        if (s.StartsWith("1="))
                            trueVal = s.Substring(2);
                        else if (s.StartsWith("0="))
                            falseVal = s.Substring(2);
                    }
                }
            }
            if (Math.Contains("[") && Math.Contains("]"))
            {
                int pos1 = Math.IndexOf("[");
                int pos2 = Math.IndexOf("]", pos1);
                string onOff = Math.ToUpper().Substring(pos1, pos2 - pos1);
                string[] onOffParts = onOff.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (onOffParts.Length > 1)
                {
                    foreach (string s in onOffParts)
                    {
                        if (s.StartsWith("10 or 01") || s.StartsWith("01 or 10"))
                        {
                            doubleBitVals[0b10] = s.Substring(8);
                            doubleBitVals[0b01] = s.Substring(8);
                        }
                        else if (s.StartsWith("00"))
                        {
                            doubleBitVals[0b00] = s.Substring(8);
                        }
                        else if (s.StartsWith("01"))
                        {
                            doubleBitVals[0b01] = s.Substring(8);
                        }
                        else if (s.StartsWith("10"))
                        {
                            doubleBitVals[0b10] = s.Substring(8);
                        }
                        else if (s.StartsWith("11"))
                        {
                            doubleBitVals[0b11] = s.Substring(8);
                        }
                    }
                }
            }
            string[] bitVals = Math.Substring(5).Trim().ToUpper().Split(new string[] {"BIT " }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string b in bitVals)
            {
                string[] bitPosStr = b.Split(new char[] { ':', '=', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int bitPos = 0;
                if (!string.IsNullOrWhiteSpace(bitPosStr[0]) && (bitPosStr[0].Contains("-") || int.TryParse(bitPosStr[0], out bitPos)))
                {
                    string nextChar = b.Substring(1, 1);
                    if (nextChar == "-")
                    {
                        string[] pairParts = b.Substring(0,3).Split('-');
                        if (pairParts.Length == 2)
                        {
                            if (int.TryParse(pairParts[0], out int bit1Pos) && int.TryParse(pairParts[1], out int bit2Pos))
                            {
                                int bits = (int)value1;
                                bits = bits >> bit1Pos;
                                byte flag1 = (byte)(bits & 1);
                                bits = (int)value1;
                                bits = bits >> bit2Pos;
                                byte flag2 = (byte)(bits & 1);
                                int dVal = (flag1 << 1) | flag2;
                                retVal += doubleBitVals[dVal] +"-";
                            }
                        }
                    }
                    else 
                    {
                        if (b.Contains("(") && b.Contains(")"))
                        {
                            int pos1 = b.IndexOf("(");
                            int pos2 = b.IndexOf(")", pos1);
                            string onOff = b.Substring(pos1, pos2 - pos1);
                            string[] onOffParts = onOff.Split(new char[] { ',', '(',')' }, StringSplitOptions.RemoveEmptyEntries);
                            if (onOffParts.Length < 2)
                                onOffParts = onOff.Split(new char[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                            if (onOffParts.Length > 1)
                            {
                                foreach (string s1 in onOffParts)
                                {
                                    string s = s1.Trim();
                                    if (s.StartsWith("1="))
                                        trueVal = s.Substring(2);
                                    else if (s.StartsWith("0="))
                                        falseVal = s.Substring(2);
                                    else if (s.StartsWith("1 ="))
                                        trueVal = s.Substring(3);
                                    else if (s.StartsWith("0 ="))
                                        falseVal = s.Substring(3);
                                }
                            }
                        }

                        if (GetBitStatus(value1,bitPos))
                        {
                            retVal += trueVal + "-";
                        }
                        else
                        {
                            retVal += falseVal +"-";
                        }
                    }
                }
            }
            return retVal.Trim('-');
        }

        private string ParseBytesValue(double value1)
        {
            string retVal = "";
            for (int byteNr=1; byteNr<=4; byteNr++)
            {
                if (Math.ToLower().Contains("byte " + byteNr.ToString()))
                {

                    byte tmpVal = (byte)((UInt32)value1 >> (4-byteNr) * 8);
                    retVal += tmpVal.ToString("X2") +"-";
                }
            }
            return retVal.Trim('-');
        }

        public string GetCalculatedValue(double value1, double value2)
        {
            try
            {
                if (Math.ToLower().StartsWith("circuitstatus"))
                {
                    //BYTES 1-2 ARE THE DTC ENCODED AS BCD (PO443 = $04 $43) BYTE 3 BIT 7 PCM CONTROLLED STATE OF THE OUTPUT (1=ON 2=OFF)
                    byte b1 = (byte)((UInt32)value1 >> 16);
                    byte b2 = (byte)((UInt32)value1 >> 8);
                    byte b3 = (byte)((UInt32)value1);
                    string retVal = "P" + b1.ToString("X2") + b2.ToString("X2") + ":";
                    if ((b3 & 0x80) == 0)
                        retVal += "OFF";
                    else
                        retVal += "ON";
                    return retVal;
                }
                else if (Math.ToLower().StartsWith("enum:"))
                {
                    return ParseEnumValue(Math, value1);
                }
                else if (Math.ToLower().StartsWith("bits:"))
                {
                    return ParseBitsValue(value1);
                }
                else if (Math.ToLower().StartsWith("bytes:"))
                {
                    return ParseBytesValue(value1);
                }
                else if (IsBitMapped)
                {
                    return GetBitmappedValue(value1);
                }
                else
                {
                    string assignedMath = Math.ToLower().Replace("x", value1.ToString());
                    if (Math.Contains("y") && value2 > double.MinValue)
                        assignedMath = assignedMath.ToLower().Replace("y", value2.ToString());
                    double calcVal = parser.Parse(assignedMath);
                    return calcVal.ToString();
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
            return "";
        }

        public double GetCalculatedDoubleValue(double value1, double value2)
        {
            try
            {
                if (Math.ToLower().StartsWith("enum:") || Math.ToLower().StartsWith("bits:"))
                {
                    return value1;
                }
                else if (IsBitMapped)
                {
                    bool val = GetBitmappedBoolValue(value1);
                    if (val)
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    string assignedMath = Math.ToLower().Replace("x", value1.ToString());
                    if (Math.Contains("y") && value2 > double.MinValue)
                        assignedMath = assignedMath.ToLower().Replace("y", value2.ToString());
                    double calcVal = parser.Parse(assignedMath);
                    return calcVal;
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
            return double.MinValue;
        }
    }

    public static byte[] GetByteParameters(string byteStr)
    {
        List<byte> retVal = new List<byte>();
        try
        {
            if (byteStr.Contains("-"))
            {
                string[] fParts = byteStr.Split('-');
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
                return byteStr.Replace(" ", "").ToBytes();
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
