using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static UniversalPatcher.DataLogger;
using UniversalPatcher;
using static LoggerUtils;
using System.Xml.Serialization;

namespace UniversalPatcher
{
    public class PidConfig
    {
        public PidConfig()
        {
            DataType = ProfileDataType.UBYTE;
            addr2 = -1;
            Math = "X";
            Type = DefineBy.Pid;
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
        public enum DefineBy
        {
            //Offset = 0,
            Pid = 1,
            Address = 2,
            Proprietary = 3,
            Math = 4
        }


        public string PidName { get; set; }
        public DefineBy Type { get; set; }

        [XmlIgnoreAttribute]
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
        [XmlIgnoreAttribute]
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
                int pos2 = Math.IndexOf(")", pos1);
                string onOff = Math.Substring(pos1, pos2 - pos1);
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
            string[] bitVals = Math.Substring(5).Trim().ToUpper().Split(new string[] { "BIT " }, StringSplitOptions.RemoveEmptyEntries);
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

        private string ParseBytesValue(double value1)
        {
            string retVal = "";
            for (int byteNr = 1; byteNr <= 4; byteNr++)
            {
                if (Math.ToLower().Contains("byte " + byteNr.ToString()))
                {

                    byte tmpVal = (byte)((UInt32)value1 >> (4 - byteNr) * 8);
                    retVal += tmpVal.ToString("X2") + "-";
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
                Debug.WriteLine("Error, LoggerUtils line " + line + ": " + ex.Message);
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
                Debug.WriteLine("Error, LoggerUtils line " + line + ": " + ex.Message);
            }
            return double.MinValue;
        }
    }

}
