using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static LoggerUtils;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class LogParam
    {
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
        public enum DefineBy
        {
            //Note: value used in pidsetup as byte
            //Offset = 0,
            Pid =1,
            Address =2,
            Proprietary = 3,
            Math = 4,
            Passive =5,
            WB_Afr = 6,
            WB_Raw =7
        }

        public class LogPid
        {
            public PidParameter Parameter { get; set; }
            public string Variable { get; set; }
            public Conversion Units { get; set; }
            public Location Os { get; set; }
        }
        public class PidProfile
        {
            public string Id { get; set; }
            public string Conversion { get; set; }
            public string Os { get; set; }
        }

        public class Location
        {
            public int addr;
            private string searchstr;
            public string Os { get; set; }
            public string Address
            {
                get 
                {
                    if (string.IsNullOrEmpty(searchstr))
                    {
                        return addr.ToString("X");
                    }
                    else
                    {
                        return searchstr;
                    }
                }
                set 
                {
                    if (Os == "search" || !value.IsHex())
                    {
                        searchstr = value;
                        Os = value.ToLower();
                    }
                    else
                    {
                        searchstr = null;
                        HexToInt(value.Replace("0x", ""), out addr);
                    }
                }
            }
        }
        public class PassivePidSettings
        {
            public int MsgLength { get; set; }
            public int BitStartPos { get; set; }
            public int NumBits { get; set; }
            public bool MSB { get; set; }
            public string Origin { get; set; }
        }
        public class PidSettings
        {
            public PidSettings() { }
            public PidSettings(PidParameter parm) 
            {
                this.Parameter = parm;
                if (parm != null && parm.Conversions != null && parm.Conversions.Count > 0)
                {
                    Units = parm.Conversions[0];
                }
            }
            public PidParameter Parameter { get; set; }
            public Conversion Units { get; set; }
            public Location Os { get; set; }
            private Hertz hertz = new Hertz();
            private UInt64 lastpassiverawvalue;
            public UInt64 LastPassiveRawValue
            {
                get { return lastpassiverawvalue; }
                set
                {
                    lastpassiverawvalue = value;
                    hertz.AddTime();
                }
            }
            private string lastpassivetextvalue;
            public string LastPassiveTextValue
            {
                get { return lastpassivetextvalue; }
                set
                {
                    lastpassivetextvalue = value;
                    hertz.AddTime();
                }
            }
            public override string ToString()
            {
                return this.Parameter.Name;
            }

            public double Herz()
            {
                if (Parameter.Type == DefineBy.Pid)
                {
                    return datalogger.slothandler.GetPidHerz(Parameter.Address);
                }
                else if (Parameter.Type == DefineBy.Math)
                {
                    return datalogger.slothandler.GetPidHerz(Parameter.Pids[0].Parameter.Address);
                }
                else if (Parameter.Type == DefineBy.WB_Afr || Parameter.Type == DefineBy.WB_Raw)
                {
                    return WB.Herz();
                }
                else if (Parameter.Type == DefineBy.Passive)
                {
                    return hertz.GetHertz();
                }
                return -1;
            }
        }

        public class PidParameter
        {
            public PidParameter()
            {
                Pids = new List<LogPid>();
                Conversions = new List<Conversion>();
                Locations = new List<Location>();
                PassivePid = new PassivePidSettings();
                PassivePid.MSB = true;
                Custom = false;
                //Settings = new PidSettings();
                //Settings.Parameter = this;
                Type = DefineBy.Pid;
                VarStrings = new List<string>();
                VarStrings.AddRange(new string[] { "x","y","z"});
                for (char v='w'; v>'f';v--)
                {
                    VarStrings.Add(v.ToString());
                }
            }
            public int Address;
            private string PidId;

            private List<string> VarStrings;
            //public bool Enabled { get; set; }
            public string Id 
            {
                get
                {
                    if (Type == DefineBy.Pid )
                    {
                        return Address.ToString("X4");
                    }
                    else if (Type == DefineBy.Passive)
                    {
                        return Address.ToString("X8");
                    }
                    else
                    {
                        return PidId;
                    }
                }
                set
                {
                    PidId = value;
                    if (value != null)
                    {
                        if (Type == DefineBy.Pid || Type == DefineBy.Passive)
                        {
                            HexToInt(value.Replace("0x", ""), out Address);
                        }
                    }
                }
            }
            public string Name { get; set; }
            public DefineBy Type { get; set; }
            public ProfileDataType DataType;
            public List<LogPid> Pids;
            public List<Conversion> Conversions;
            public List<Location> Locations;
            public string Description { get; set; }
            public bool Custom { get; set; }
            //For passive mode:
            public PassivePidSettings PassivePid;
            public PidParameter ShallowCopy()
            {
                return (PidParameter)this.MemberwiseClone();
            }
            public List<PidParameter> GetLinkedPids()
            {
                List<PidParameter> retVal = new List<PidParameter>();
                if (Pids != null && Pids.Count > 0)
                {
                    foreach(LogPid lPid in Pids)
                    {
                        retVal.Add(lPid.Parameter);
                        retVal.AddRange(lPid.Parameter.GetLinkedPids());
                    }
                }
                return retVal;
            }
            public LogPid AddPid(string Id, string Conversion)
            {
                LogPid pid = new LogPid();
                pid.Parameter = datalogger.PidParams.Where(X=>X.Id == Id).FirstOrDefault();
                pid.Units = pid.Parameter.Conversions.Where(X=>X.Units == Conversion).FirstOrDefault();
                pid.Variable = VarStrings[Pids.Count];
                Pids.Add(pid);
                return pid;
            }
            public LogPid AddPid(string Id, string Conversion, string Variable)
            {
                LogPid pid = new LogPid();
                pid.Parameter = datalogger.PidParams.Where(X => X.Id == Id).FirstOrDefault();
                if (pid.Parameter == null)
                {
                    Logger("PID not found: " + Id);
                    return null;
                }
                pid.Units = pid.Parameter.Conversions.Where(X => X.Units == Conversion).FirstOrDefault();
                pid.Variable = Variable;
                Pids.Add(pid);
                return pid;
            }
            public int GetElementSize()
            {
                switch(DataType)
                {
                    case ProfileDataType.UBYTE:
                        return 1;
                    case ProfileDataType.SBYTE:
                        return 1;
                    case ProfileDataType.UWORD:
                        return 2;
                    case ProfileDataType.SWORD:
                        return 2;
                    case ProfileDataType.INT32:
                        return 4;
                    case ProfileDataType.UINT32:
                        return 4;
                    case ProfileDataType.UNKNOWN:
                        return 1;
                    default:
                        return 1;
                }
            }
            public string GetBitmappedValue(double value, PidSettings pidProfile)
            {
                string trueVal = pidProfile.Units.TrueValue;
                string falseVal = pidProfile.Units.FalseValue;
                int bits = (int)value;
                bits = bits >> pidProfile.Units.BitIndex;
                bool flag = (bits & 1) != 0;
                string retVal = falseVal;
                if (flag)
                    retVal = trueVal;
                //Debug.WriteLine("Bitmapped: " + retVal);
                return retVal;
            }
            public bool GetBitmappedBoolValue(double value, PidSettings pidProfile)
            {
                int bits = (int)value;
                bits = bits >> pidProfile.Units.BitIndex;
                bool flag = (bits & 1) != 0;
                bool retVal = false;
                if (flag)
                    retVal = true;
                return retVal;
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
            public string ParseBitsValue(string math, double value1)
            {
                string retVal = "";
                string trueVal = "On";
                string falseVal = "Off";
                string[] doubleBitVals = { "fail", "indeterminate", "indeterminate", "pass" };

                if (math.Contains("(") && math.Contains(")"))
                {
                    int pos1 = math.IndexOf("(") + 1;
                    int pos2 = math.IndexOf(")", pos1);
                    string onOff = math.Substring(pos1, pos2 - pos1);
                    string[] onOffParts = onOff.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (onOffParts.Length < 2)
                        onOffParts = onOff.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (onOffParts.Length > 1)
                    {
                        foreach (string s1 in onOffParts)
                        {
                            string s = s1.Trim();
                            if (s.StartsWith("1="))
                                trueVal = s.Substring(2);
                            else if (s.StartsWith("0="))
                                falseVal = s.Substring(2);
                        }
                    }
                }
                if (math.Contains("[") && math.Contains("]"))
                {
                    int pos1 = math.IndexOf("[");
                    int pos2 = math.IndexOf("]", pos1);
                    string onOff = math.ToUpper().Substring(pos1, pos2 - pos1);
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
                string[] bitVals = math.Substring(5).Trim().ToUpper().Split(new string[] { "BIT " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string b in bitVals)
                {
                    string[] bitPosStr = b.Split(new char[] { ':', '=', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int bitPos = 0;
                    if (!string.IsNullOrWhiteSpace(bitPosStr[0]) && (bitPosStr[0].Contains("-") || int.TryParse(bitPosStr[0], out bitPos)))
                    {
                        string nextChar = b.Substring(1, 1);
                        if (nextChar == "-")
                        {
                            string[] pairParts = b.Substring(0, 3).Split('-');
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
                                    retVal += doubleBitVals[dVal] + "-";
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
                                string[] onOffParts = onOff.Split(new char[] { ',', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
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

                            if (GetBitStatus(value1, bitPos))
                            {
                                retVal += trueVal + "-";
                            }
                            else
                            {
                                retVal += falseVal + "-";
                            }
                        }
                    }
                }
                return retVal.Trim('-');
            }
            private bool GetBitStatus(double value, int bitposition)
            {
                int bits = (int)value;
                bits = bits >> bitposition;
                bool flag = (bits & 1) != 0;
                return flag;
            }
            private string ParseBytesValue(string math, double value1)
            {
                string retVal = "";
                for (int byteNr = 1; byteNr <= 4; byteNr++)
                {
                    if (math.ToLower().Contains("byte " + byteNr.ToString()))
                    {

                        byte tmpVal = (byte)((UInt32)value1 >> (4 - byteNr) * 8);
                        retVal += tmpVal.ToString("X2") + "-";
                    }
                }
                return retVal.Trim('-');
            }


            public double GetCalculatedValue(PidSettings pidProfile, bool UseQuery)
            {
                try
                {
                    if (pidProfile.Units == null)
                    {
                        return double.MinValue;
                    }
                    string math = pidProfile.Units.Expression;
                    if (Type == DefineBy.Math)
                    {
                        foreach (LogPid mPid in Pids)
                        {
                            PidParameter mParm = mPid.Parameter;
                            if (mParm != null)
                            {
                                PidSettings mProfile = new PidSettings();
                                mProfile.Parameter = mParm;
                                mProfile.Units = mPid.Units;
                                mProfile.Os = mPid.Os;
                                double mPidVal = mParm.GetCalculatedValue(mProfile, UseQuery);
                                if (mPidVal == double.MinValue)
                                {
                                    return double.MinValue;
                                }
                                math = math.Replace(mPid.Variable, mPidVal.ToString());
                            }
                        }
                    }
                    else
                    {
                        double pidVal;
                        if (Type == DefineBy.WB_Afr)
                        {
                            pidVal = WB.AFR;
                        }
                        else if (Type == DefineBy.WB_Raw)
                        {
                            pidVal = WB.RAW;
                        }
                        else if (Type == DefineBy.Passive)
                        {
                            pidVal = pidProfile.LastPassiveRawValue;
                        }
                        else
                        {
                            int addr;
                            if (Type == DefineBy.Address)
                            {
                                addr = pidProfile.Os.addr;
                                if (addr == int.MinValue)
                                {
                                    return double.MinValue;
                                }
                            }
                            else
                            {
                                addr = Address;
                            }
                            if (UseQuery)
                            {
                                DataLogger.ReadValue rv = datalogger.QuerySinglePidValue(this);
                                if (rv.FailureCode > 0)
                                {
                                    return double.MinValue;
                                }
                                pidVal = rv.PidValue;
                            }
                            else
                            {
                                pidVal = datalogger.slothandler.GetLastPidValue(addr);
                            }
                        }
                        if (pidVal == double.MinValue)
                        {
                            return double.MinValue;
                        }

                        if (math.ToLower().StartsWith("circuitstatus"))
                        {
                            //BYTES 1-2 ARE THE DTC ENCODED AS BCD (PO443 = $04 $43) BYTE 3 BIT 7 PCM CONTROLLED STATE OF THE OUTPUT (1=ON 2=OFF)
                            byte b1 = (byte)((UInt32)pidVal >> 16);
                            byte b2 = (byte)((UInt32)pidVal >> 8);
                            byte b3 = (byte)((UInt32)pidVal);
                            if ((b3 & 0x80) == 0)
                                return 0;
                            else
                                return 1;
                        }
                        else if (math.ToLower().StartsWith("enum:"))
                        {
                            return pidVal;
                        }
                        else if (math.ToLower().StartsWith("bits:"))
                        {
                            return pidVal;
                        }
                        else if (math.ToLower().StartsWith("bytes:"))
                        {
                            return pidVal;
                        }

                        if (Conversions[0].IsBitMapped)
                        {
                            bool bitVal = GetBitmappedBoolValue(pidVal, pidProfile);
                            if (bitVal)
                                return 1;
                            else
                                return 0;
                        }
                        math = math.Replace("x", pidVal.ToString());
                    }
                    Debug.WriteLine("Math: " + math);
                    return parser.Parse(math);
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    LoggerBold("Error, LogParam line " + line + ": " + ex.Message);
                    return double.MinValue;
                }

            }

            public string GetCalculatedStringValue(PidSettings pidProfile, bool useQuery)
            {
                try
                {
                    if (pidProfile.Units == null )
                    {
                        return "";
                    }
                    string math = pidProfile.Units.Expression;
                    if (Type == DefineBy.Math)
                    {
                        foreach (LogPid mPid in Pids)
                        {

                            PidParameter mParm = mPid.Parameter;
                            if (mParm != null)
                            {
                                PidSettings mProfile = new PidSettings();
                                mProfile.Parameter = mParm;
                                mProfile.Units = mPid.Units;
                                mProfile.Os = mPid.Os;
                                double mPidVal = mParm.GetCalculatedValue(mProfile,useQuery);
                                if (mPidVal == double.MinValue)
                                {
                                    return "";
                                }
                                math = math.Replace(mPid.Variable, mPidVal.ToString());
                            }
                        }
                    }
                    else
                    {
                        double pidVal;
                        if (Type == DefineBy.WB_Afr)
                        {
                            pidVal = WB.AFR;
                        }
                        else if (Type == DefineBy.WB_Raw)
                        {
                            pidVal = WB.RAW;
                        }
                        else if (Type == DefineBy.Passive)
                        {
                            if (string.IsNullOrEmpty(pidProfile.LastPassiveTextValue))
                            {
                                pidVal = pidProfile.LastPassiveRawValue;
                            }
                            else
                            {
                                return pidProfile.LastPassiveTextValue;
                            }
                        }
                        else
                        {
                            int addr;
                            if (Type == DefineBy.Address)
                            {
                                addr = pidProfile.Os.addr;
                                if (addr == int.MinValue)
                                {
                                    return "";
                                }
                                Address = addr;
                            }
                            else
                            {
                                addr = Address;
                            }
                            if (useQuery)
                            {
                                DataLogger.ReadValue rv = datalogger.QuerySinglePidValue(this);
                                if (rv.FailureCode > 0)
                                {
                                    return "";
                                }
                                pidVal = rv.PidValue;
                            }
                            else
                            {
                                pidVal = datalogger.slothandler.GetLastPidValue(addr);
                            }
                        }
                        if (pidVal == double.MinValue)
                        {
                            return "";
                        }

                        if (math.ToLower().StartsWith("circuitstatus"))
                        {
                            //BYTES 1-2 ARE THE DTC ENCODED AS BCD (PO443 = $04 $43) BYTE 3 BIT 7 PCM CONTROLLED STATE OF THE OUTPUT (1=ON 2=OFF)
                            byte b1 = (byte)((UInt32)pidVal >> 16);
                            byte b2 = (byte)((UInt32)pidVal >> 8);
                            byte b3 = (byte)((UInt32)pidVal);
                            string retVal = "P" + b1.ToString("X2") + b2.ToString("X2") + ":";
                            if ((b3 & 0x80) == 0)
                                retVal += "OFF";
                            else
                                retVal += "ON";
                            return retVal;
                        }
                        else if (math.ToLower().StartsWith("enum:"))
                        {
                            return ParseEnumValue(math, pidVal);
                        }
                        else if (math.ToLower().StartsWith("bits:"))
                        {
                            return ParseBitsValue(math,pidVal);
                        }
                        else if (math.ToLower().StartsWith("bytes:"))
                        {
                            return ParseBytesValue(math,pidVal);
                        }

                        if (Conversions[0].IsBitMapped)
                        {
                            return GetBitmappedValue(pidVal, pidProfile);
                        }
                        math = math.Replace("x", pidVal.ToString());
                    }
                    //Debug.WriteLine("Math: " + math);
                    return parser.Parse(math).ToString(pidProfile.Units.Format);
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    LoggerBold("Error, LogParam line " + line + ": " + ex.Message);
                    return "";
                }

            }

        }

 
    }
}
