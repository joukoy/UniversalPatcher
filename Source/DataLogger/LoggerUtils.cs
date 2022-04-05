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

    public enum KInit
    {
        None,
        FastInit_GMDelco,
        FastInit_ME7_5,
        FastInit_J1979,
        FiveBaudInit_J1979
    }

    [Serializable]
    public class J2534InitParameters
    {
        public J2534InitParameters()
        {
            this.VPWLogger = false;
            this.Kinit = KInit.None;
        }
        public J2534InitParameters(bool VpwLogger)
        {
            this.VPWLogger = VpwLogger;
        }
        public bool VPWLogger { get; set; }
        public ProtocolID Protocol { get; set; }
        public string Baudrate { get; set; }
        public string Sconfigs { get; set; }
        public KInit Kinit { get; set; }
        public string InitBytes { get; set; }
        public ConnectFlag Connectflag { get; set; }
        public string PerodicMsg { get; set; }
        public int PriodicInterval { get; set; }
        public string PassFilters { get; set; }
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

            public string GetCalculatedValue(double value1, double value2)
            {
                if (IsBitMapped)
                {
                    return GetBitmappedValue(value1);
                }
                else
                {
                    string math = Math.ToLower().Replace("x", value1.ToString());
                    if (Math.Contains("y") && value2 > double.MinValue)
                        math = Math.ToLower().Replace("y", value2.ToString());
                    double calcVal = parser.Parse(math);
                    return calcVal.ToString();
                }
            }
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
            LoggerBold("Error, LoadProfile line " + line + ": " + ex.Message);
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
