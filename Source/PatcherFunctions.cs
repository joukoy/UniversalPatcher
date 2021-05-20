using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UniversalPatcher;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using UniversalPatcher.Properties;
using System.Linq;
using System.Drawing;
using System.Xml.Serialization;
using MathParserTK;

public class upatcher
{
    public class DetectRule
    {
        public DetectRule() { }

        public string xml { get; set; }
        public ushort group { get; set; }
        public string grouplogic { get; set; }   //and, or, xor
        public string address { get; set; }
        public UInt64 data { get; set; }
        public string compare { get; set; }        //==, <, >, !=      
    }


    public class XmlPatch
    {
        public XmlPatch() { }
        public string Description { get; set; }
        public string XmlFile { get; set; }
        public string Segment { get; set; }
        public string CompatibleOS { get; set; }
        public string Data { get; set; }
        public string Rule { get; set; }
        public string HelpFile { get; set; }
        public string PostMessage { get; set; }
    }

    public class CVN
    {
        public CVN() { }
        public string XmlFile { get; set; }
        public string AlternateXML { get; set; }
        public string SegmentNr { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string cvn { get; set; }
        public string ReferenceCvn { get; set; }
    }
    public struct Block
    {
        public uint Start;
        public uint End;
    }

    public struct CheckWord
    {
        public string Key;
        public uint Address;
    }

    /*
     File information is read in 3 phases:
     1. XML-file have definitions, how info is stored (SegmentConfig)
     2. Addresses for information is collected from file (SegmentAddressData). 
        For example (OS 12579405): read EngineCal segment address from address 514 => SegmentBlocks => Block1 = 8000 - 15DFFF
        PNAddr is segment address +4 => PNaddr = 8004
     3. Read information from file using collected addresses (SegmentInfo)     
     */
    public struct SegmentAddressData
    {
        public List<Block> SegmentBlocks;
        public List<Block> SwapBlocks;
        public List<Block> CS1Blocks;
        public List<Block> CS2Blocks;
        public List<Block> ExcludeBlocks;
        public AddressData CS1Address;
        public AddressData CS2Address;
        public AddressData PNaddr;
        public AddressData VerAddr;
        public AddressData SegNrAddr;
        public List<CheckWord> Checkwords;
        public List<AddressData> ExtraInfo;
    }

    public class SegmentInfo
    {
        public SegmentInfo()
        {
            Name = "";
            FileName = "";
            XmlFile = "";
            Address = "";
            SwapAddress = "";
            Size = "";
            SwapSize = "";
            CS1 = "";
            CS2 = "";
            CS1Calc = "";
            CS2Calc = "";
            cvn = "";
            Stock = "";
            PN = "";
            Ver = "";
            SegNr = "";
            ExtraInfo = "";
        }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string XmlFile { get; set; }
        public string Address { get; set; }
        public string SwapAddress { get; set; }
        public string Size { get; set; }
        public string SwapSize { get; set; }
        public string CS1 { get; set; }
        public string CS2 { get; set; }
        public string CS1Calc { get; set; }
        public string CS2Calc { get; set; }
        public string cvn { get; set; }
        public string Stock { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string SegNr { get; set; }
        public string ExtraInfo { get; set; }

    }

    public class dtcCode
    {
        public dtcCode()
        {
            codeInt = 0;
            codeAddrInt = uint.MaxValue;
            //CodeAddr = "";
            statusAddrInt = uint.MaxValue;
            //StatusAddr = "";
            Description = "";
            Status = 99;
            MilStatus = 99;
            MilAddr = "";
            milAddrInt = 0;
            StatusTxt = "";
        }
        public UInt16 codeInt;
        public string Code { get; set; }
        public byte Status { get; set; }
        public string StatusTxt { get; set; }
        public byte MilStatus { get; set; }
        public string Description { get; set; }
        public uint codeAddrInt;
        public string CodeAddr
        {
            get
            {
                if (codeAddrInt == uint.MaxValue)
                    return "";
                else
                    return codeAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = codeAddrInt;
                    if (!HexToUint(value, out codeAddrInt))
                        codeAddrInt = prevVal;
                }
                else
                {
                    codeAddrInt = uint.MaxValue;
                }
            }
        }

        public uint statusAddrInt;
        public string StatusAddr
        {
            get
            {
                if (statusAddrInt == uint.MaxValue)
                    return "";
                else
                    return statusAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = statusAddrInt;
                    if (!HexToUint(value, out statusAddrInt))
                        statusAddrInt = prevVal;
                }
                else
                {
                    statusAddrInt = uint.MaxValue;
                }
            }
        }
        public uint milAddrInt;
        public string MilAddr { get; set; }
    }

    public class OBD2Code
    {
        public OBD2Code()
        {

        }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TableView
    {
        public TableView()
        { }
        public uint Row { get; set; }
        public uint addrInt;
        public string Address { get; set; }
        public byte dataInt;
        public string Data { get; set; }
    }

    public struct SegmentConfig
    {
        public string Name;
        public string Version;
        public string Addresses;    //Segment addresses, can be multiple parts
        public string SwapAddress;  //Segment addresses, can be multiple parts, used for segment swapping
        public string CS1Address;           //Checksum 1 Address
        public string CS2Address;           //Checksum 2 Address
        public short CS1Method;     //Checksum 1 calculation method
        public short CS2Method;     //Checksum 2 calculation method
        public string CS1Blocks;       //Calculate checksum 1 from these addresses
        public string CS2Blocks;       //Calculate checksum 2 from these addresses
        public short CS1Complement;   //Calculate 1's or 2's Complement from Checksum?
        public short CS2Complement;   //Calculate 1's or 2's Complement from Checksum?
        public bool CS1SwapBytes;
        public bool CS2SwapBytes;
        public int CVN;             //0=None, 1=Checksum 1, 2=Checksum 2
        public bool Eeprom;         //Special case: P01 or P59 Eeprom segment
        public string PNAddr;
        public string VerAddr;
        public string SegNrAddr;
        public string ExtraInfo;
        public string Comment;
        public string CheckWords;
        public string SearchAddresses;  //Possible start addresses for searched segment
        public string Searchfor;  //search if this found/not found in segment
        //public string Searchfrom; //Search above in these addresses OBSOLETE
        //public bool SearchNot;     //Search where NOT found OBSOLETE
    }

    public class SwapSegment
    {
        //For storing information about extracted calibration segments
        public SwapSegment()
        {

        }
        public string FileName { get; set; }
        public string XmlFile { get; set; }
        public string OS { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string SegNr { get; set; }
        public int SegIndex { get; set; }
        public string Size { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Stock { get; set; }
        public string Cvn { get; set; }
        public string SegmentSizes { get; set; }     //For OS compatibility
        public string SegmentAddresses { get; set; } //For OS compatibility

    }
    public struct AddressData
    {
        public string Name;
        public uint Address;
        public ushort Bytes;
        public ushort Type;
    }

    public struct referenceCvn
    {
        public string PN;
        public string CVN;
    }

    public class FileType
    {
        public FileType() { }

        public string Description { get; set; }
        public string Extension { get; set; }
    }

    public struct SearchedAddress
    {
        public uint Addr;
        public ushort Rows;
        public ushort Columns;
    }

    public const short CSMethod_None = 0;
    public const short CSMethod_crc16 = 1;
    public const short CSMethod_crc32 = 2;
    public const short CSMethod_Bytesum = 3;
    public const short CSMethod_Wordsum = 4;
    public const short CSMethod_Dwordsum = 5;

    public static List<DetectRule> DetectRules;
    public static List<XmlPatch> PatchList;
    public static List<CVN> StockCVN;
    public static List<CVN> ListCVN;
    public static List<CVN> BadCvnList;
    public static List<SegmentInfo> SegmentList;
    public static List<SegmentInfo> BadChkFileList = new List<SegmentInfo>();
    public static List<SwapSegment> SwapSegments;
    //public static List<TableSearchConfig> tableSearchConfig;
    public static List<TableSearchResult> tableSearchResult;
    public static List<TableSearchResult> tableSearchResultNoFilters;
    public static List<TableView> tableViews;
    public static List<referenceCvn> referenceCvnList;
    public static List<FileType> fileTypeList;
    public static List<OBD2Code> OBD2Codes;
    public static List<DtcSearchConfig> dtcSearchConfigs;
    public static List<TableSeek> tableSeeks;
    public static List<UniversalPatcher.TableData> XdfElements;
    public static List<Units> unitList;
    public static List<RichTextBox> LogReceivers;

    public static string tableSearchFile;
    public static string tableSeekFile = "";
    public static MathParser parser = new MathParser();
    public static SavingMath savingMath = new SavingMath();

    public static FrmPatcher frmpatcher;

    public static string[] dtcStatusCombined = { "MIL and reporting off", "Type A/no MIL", "Type B/no MIL", "Type C/no MIL", "Not reported/MIL", "Type A/MIL", "Type B/MIL", "Type C/MIL" };
    public static string[] dtcStatus = { "1 Trip/immediately", "2 Trips", "Store only", "Disabled" };

    public const ushort TypeFloat = 1;
    public const ushort TypeInt = 2;
    public const ushort TypeHex = 3;
    public const ushort TypeText = 4;
    public const ushort TypeFlag = 5;
    public const ushort TypeFilename = 10;
    
    public enum OutDataType
    {
        Float = 1,
        Int = 2,
        Hex = 3,
        Text = 4,
        Flag = 5,
        Filename = 10
    }

    public enum InDataType
    {
        UBYTE,              //UNSIGNED INTEGER - 8 BIT
        SBYTE,              //SIGNED INTEGER - 8 BIT
        UWORD,              //UNSIGNED INTEGER - 16 BIT
        SWORD,              //SIGNED INTEGER - 16 BIT
        UINT32,              //UNSIGNED INTEGER - 32 BIT
        INT32,              //SIGNED INTEGER - 32 BIT
        UINT64,           //UNSIGNED INTEGER - 64 BIT
        INT64,            //SIGNED INTEGER - 64 BIT
        FLOAT32,       //SINGLE PRECISION FLOAT - 32 BIT
        FLOAT64,        //DOUBLE PRECISION FLOAT - 64 BIT
        UNKNOWN
    }

    public enum TableValueType
    {
        boolean,
        selection,
        bitmask,
        number
    }

    public static TableValueType getValueType(TableData td)
    {
        TableValueType retVal;

        if (td.Units == null)
            td.Units = "";
        if (td.BitMask != null && td.BitMask.Length > 0)
        {
            retVal = TableValueType.bitmask;
        }
        else if (td.Units.ToLower().Contains("boolean") || td.Units.ToLower().Contains("t/f"))
        {
            retVal = TableValueType.boolean;
        }
        else if (td.Units.ToLower().Contains("true") && td.Units.ToLower().Contains("false"))
        {
            retVal = TableValueType.boolean;
        }
        else if (td.Values.StartsWith("Enum: "))
        {
            retVal = TableValueType.selection;
        }
        else
        {
            retVal = TableValueType.number;
        }
        return retVal;
    }

    public static int getBits(InDataType dataType)
    {
        int bits = 8; // Assume one byte if not defined. OK?
        if (dataType == InDataType.SBYTE || dataType == InDataType.UBYTE)
            bits = 8;
        if (dataType == InDataType.SWORD || dataType == InDataType.UWORD)
            bits = 16;
        if (dataType == InDataType.INT32 || dataType == InDataType.UINT32 || dataType == InDataType.FLOAT32)
            bits = 32;
        if (dataType == InDataType.INT64 || dataType == InDataType.UINT64 || dataType == InDataType.FLOAT64)
            bits = 64;
        if (dataType == InDataType.UNKNOWN)
            Logger("Warning, unknown data type. Assuming UBYTE");

        return bits;
    }
    public static int getElementSize(InDataType dataType)
    {
        return getBits(dataType) / 8;
    }
    public static bool getSigned(InDataType dataType)
    {
        bool signed = false;
        if (dataType == InDataType.INT32 || dataType == InDataType.INT64 || dataType == InDataType.SBYTE || dataType == InDataType.SWORD)
            signed = true;
        return signed;
    }

    public static InDataType convertToDataType(string bitStr, bool signed, bool floating)
    {
        InDataType retVal = InDataType.UNKNOWN;
        int bits = Convert.ToInt32(bitStr);
        retVal = convertToDataType(bits / 8, signed, floating);
        return retVal;
    }

    public static InDataType convertToDataType(int elementSize, bool Signed, bool floating)
    {
        InDataType DataType = InDataType.UNKNOWN; 
        if (elementSize == 1)
        {
            if (Signed == true)
            {
                DataType = InDataType.SBYTE;
            }
            else
            {
                DataType = InDataType.UBYTE;
            }

        }
        else if (elementSize == 2)
        {
            if (Signed == true)
            {
                DataType = InDataType.SWORD;
            }
            else
            {
                DataType = InDataType.UWORD;
            }

        }
        else if (elementSize == 4)
        {
            if (floating)
            {
                DataType = InDataType.FLOAT32;
            }
            else
            {
                if (Signed == true)
                {
                    DataType = InDataType.UINT32;
                }
                else
                {
                    DataType = InDataType.INT32;
                }
            }
        }
        else if (elementSize == 8)
        {
            if (floating)
            {
                DataType = InDataType.FLOAT64;
            }
            else
            {
                if (Signed == true)
                {
                    DataType = InDataType.INT64;
                }
                else
                {
                    DataType = InDataType.UINT64;
                }
            }

        }
        return DataType;
    }

    public static double getMaxValue (InDataType dType)
    {
        if (dType == InDataType.FLOAT32)
            return float.MaxValue;
        else if (dType == InDataType.FLOAT64)
            return double.MaxValue;
        else if (dType == InDataType.INT32)
            return Int32.MaxValue;
        else if (dType == InDataType.INT64)
            return Int64.MaxValue;
        else if (dType == InDataType.SBYTE)
            return sbyte.MaxValue;
        else if (dType == InDataType.SWORD)
            return Int16.MaxValue;
        else if (dType == InDataType.UBYTE)
            return byte.MaxValue;
        else if (dType == InDataType.UINT32)
            return UInt32.MaxValue;
        else if (dType == InDataType.UINT64)
            return UInt64.MaxValue;
        else if (dType == InDataType.UWORD)
            return UInt16.MaxValue;
        else 
            return byte.MaxValue;

    }

    public static double getMinValue(InDataType dType)
    {
        if (dType == InDataType.FLOAT32)
            return float.MinValue;
        else if (dType == InDataType.FLOAT64)
            return double.MinValue;
        else if (dType == InDataType.INT32)
            return Int32.MinValue;
        else if (dType == InDataType.INT64)
            return Int64.MinValue;
        else if (dType == InDataType.SBYTE)
            return sbyte.MinValue;
        else if (dType == InDataType.SWORD)
            return Int16.MinValue;
        else if (dType == InDataType.UBYTE)
            return byte.MinValue;
        else if (dType == InDataType.UINT32)
            return UInt32.MinValue;
        else if (dType == InDataType.UINT64)
            return UInt64.MinValue;
        else if (dType == InDataType.UWORD)
            return UInt16.MinValue;
        else
            return byte.MinValue;

    }

    public static string readConversionTable(string mathStr, PcmFile PCM)
    {
        //Example: TABLE:'MAF Scalar #1'
        string retVal = mathStr;
        int start = mathStr.IndexOf("table:") + 6;
        int mid = mathStr.IndexOf("'", start + 7);
        string conversionTable = mathStr.Substring(start, mid - start + 1);
        TableData tmpTd = new TableData();
        tmpTd.TableName = conversionTable.Replace("'", "");
        int targetId = findTableDataId(tmpTd, PCM.tableDatas);
        if (targetId > -1)
        {
            TableData conversionTd = PCM.tableDatas[targetId];
            double conversionVal = getValue(PCM.buf, (uint)(conversionTd.addrInt + conversionTd.Offset), conversionTd, 0, PCM);
            retVal = mathStr.Replace("table:" + conversionTable, conversionVal.ToString());
            Debug.WriteLine("Using conversion table: " + conversionTd.TableName);
        }

        return retVal;
    }

    public static string readConversionRaw(string mathStr, PcmFile PCM)
    {
        // Example: RAW:0x321:SWORD:MSB
        string retVal = mathStr;
        int start = mathStr.IndexOf("raw:");
        int mid = mathStr.IndexOf(" ", start + 3);
        string rawStr = mathStr.Substring(start, mid - start + 1);
        string[] rawParts = rawStr.Split(':');
        if (rawParts.Length < 3)
        {
            throw new Exception("Unknown RAW definition in Math: " + mathStr);
        }
        InDataType idt =(InDataType) Enum.Parse(typeof(InDataType), rawParts[2].ToUpper());
        TableData tmpTd = new TableData();
        tmpTd.Address = rawParts[1];
        tmpTd.Offset = 0;
        tmpTd.DataType = idt;
        double rawVal = (double)getRawValue(PCM.buf, tmpTd.addrInt, tmpTd, 0);
        if (rawParts.Length > 3 && rawParts[3].StartsWith("lsb"))
        {
            int eSize = getElementSize(idt);
            if (eSize == 2)
                rawVal = SwapBytes((ushort)rawVal);
            else if (eSize == 4)
                rawVal = SwapBytes((UInt32)rawVal);
            else if (eSize == 8)
                rawVal = SwapBytes((UInt64)rawVal);
        }
        retVal = mathStr.Replace(rawStr, rawVal.ToString());
        return retVal;
    }

    //
    //Get value from defined table, using defined math functions.
    //
    public static double getValue(byte[] myBuffer, uint addr, TableData mathTd, uint offset, PcmFile PCM)
    {
        double retVal = 0;
        try
        {

            if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0)
            {
                UInt64 rawVal = (UInt64)getRawValue(myBuffer, addr, mathTd, offset);
                UInt64 mask = Convert.ToUInt64(mathTd.BitMask.Replace("0x", ""), 16);
                if (((UInt64) rawVal & mask) == mask)
                    return 1;
                else
                    return 0;
            }

            UInt32 bufAddr = addr - offset;

            if (mathTd.DataType == InDataType.SBYTE)
                retVal = (sbyte)myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.UBYTE)
                retVal = myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.SWORD)
                retVal = BEToInt16(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UWORD)
                retVal = BEToUint16(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT32)
                retVal = BEToInt32(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UINT32)
                retVal = BEToUint32(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT64)
                retVal = BEToInt64(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UINT64)
                retVal = BEToUint64(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.FLOAT32)
                retVal = BEToFloat32(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.FLOAT64)
                retVal = BEToFloat64(myBuffer, bufAddr);

            if (mathTd.Math == null || mathTd.Math.Length == 0)
                mathTd.Math = "X";
            string mathStr = mathTd.Math.ToLower().Replace("x", retVal.ToString());
            if (mathStr.Contains("table:"))
            {
                mathStr = readConversionTable(mathStr, PCM);
            }
            if (mathStr.Contains("raw:"))
            {
                mathStr = readConversionRaw(mathStr, PCM);
            }
            retVal = parser.Parse(mathStr, false);
            //Debug.WriteLine(mathStr);
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Patcherfunctions error, line " + line + ": " + ex.Message);
        }

        return retVal;
    }

    public static double getRawValue(byte[] myBuffer, UInt32 addr, TableData mathTd, uint offset)
    {
        UInt32 bufAddr = addr - offset;
        double retVal = 0;
        try
        {
            switch (mathTd.DataType)
            {
                case InDataType.SBYTE:
                    return (sbyte)myBuffer[bufAddr];
                case InDataType.UBYTE:
                    return (byte)myBuffer[bufAddr];
                case InDataType.SWORD:
                    return (Int16)BEToInt16(myBuffer, bufAddr);
                case InDataType.UWORD:
                    return (UInt16)BEToUint16(myBuffer, bufAddr);
                case InDataType.INT32:
                    return (Int32)BEToInt32(myBuffer, bufAddr);
                case InDataType.UINT32:
                    return (UInt32)BEToUint32(myBuffer, bufAddr);
                case InDataType.INT64:
                    return (Int64)BEToInt64(myBuffer, bufAddr);
                case InDataType.UINT64:
                    return (UInt64)BEToInt64(myBuffer, bufAddr);
                case InDataType.FLOAT32:
                    return (float)BEToFloat32(myBuffer, bufAddr);
                case InDataType.FLOAT64:
                    return BEToFloat64(myBuffer, bufAddr);
            }

        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Patcherfunctions error, line " + line + ": " + ex.Message);
        }

        return retVal;
    }


    public static  Dictionary<double, string> parseEnumHeaders(string eVals)
    {
        Dictionary<double, string> retVal = new Dictionary<double, string>();
        string[] posVals = eVals.Split(',');
        for (int r = 0; r < posVals.Length; r++)
        {
            string[] parts = posVals[r].Split(':');
            double val = 0;
            double.TryParse(parts[0], out val);
            string txt = posVals[r];
            if (!retVal.ContainsKey(val))
                retVal.Add(val, txt);
        }
        retVal.Add(double.MaxValue, "------------");
        return retVal;
    }

    public static Dictionary<int, string> parseIntEnumHeaders(string eVals)
    {
        Dictionary<int, string> retVal = new Dictionary<int, string>();
        string[] posVals = eVals.Split(',');
        for (int r = 0; r < posVals.Length; r++)
        {
            string[] parts = posVals[r].Split(':');
            int val = 0;
            int.TryParse(parts[0], out val);
            string txt = posVals[r];
            if (!retVal.ContainsKey(val))
                retVal.Add(val, txt);
        }
        retVal.Add(int.MaxValue, "------------");
        return retVal;
    }

    public static uint checkPatchCompatibility(XmlPatch xpatch, PcmFile basefile)
    {
        uint retVal = uint.MaxValue;
        bool isCompatible = false;
        string[] Parts = xpatch.XmlFile.Split(',');
        foreach (string Part in Parts)
        {
            if (Part.ToLower().Replace(".xml","") == basefile.configFile)
                isCompatible = true;
        }
        if (!isCompatible)
        {
            Logger("Incompatible patch, current configfile: " + basefile.configFile + ", patch configile: " + xpatch.XmlFile.Replace(".xml", ""));
            return retVal;
        }

        if (xpatch.CompatibleOS.ToLower().StartsWith("search:"))
        {
            string searchStr = xpatch.CompatibleOS.Substring(7);
            for (int seg = 0; seg < basefile.segmentinfos.Length; seg++)
            {
                if (basefile.segmentinfos[seg].Name.ToLower() == xpatch.Segment.ToLower())
                {
                    Debug.WriteLine("Searching only segment: " + basefile.segmentinfos[seg].Name);
                    for (int b = 0; b < basefile.segmentAddressDatas[seg].SegmentBlocks.Count; b++)
                    {
                        retVal = searchBytes(basefile, searchStr, basefile.segmentAddressDatas[seg].SegmentBlocks[b].Start, basefile.segmentAddressDatas[seg].SegmentBlocks[b].End);
                        if (retVal < uint.MaxValue)
                            break;
                    }
                }
            }
            if (retVal == uint.MaxValue) //Search whole bin
                retVal = searchBytes(basefile, searchStr, 0, basefile.fsize);
            if (retVal < uint.MaxValue)
            {
                Logger("Data found at address: " + retVal.ToString("X8"));
                isCompatible = true;
            }
            else
            {
                uint tmpVal = searchBytes(basefile, xpatch.Data, 0, basefile.fsize);
                if (tmpVal < uint.MaxValue)
                    Logger("Patch is already applied, data found at: " + tmpVal.ToString("X8"));
                else
                    Logger("Data not found. Incompatible patch");
            }
        }
        else if (xpatch.CompatibleOS.ToLower().StartsWith("table:"))
        {
            if (basefile.tableDatas.Count < 3)
                basefile.loadTunerConfig();
            basefile.importDTC();
            basefile.importSeekTables();
            string[] tableParts = xpatch.CompatibleOS.Split(',');
            if (tableParts.Length < 3)
            {
                Logger("Incomplete table definition:" + xpatch.CompatibleOS);
            }
            else
            {
                string tbName = "";
                int rows = 1;
                int columns = 1;
                for (int i = 0; i < tableParts.Length; i++)
                {
                    string[] xParts = tableParts[i].Split(':');
                    if (xParts[0].ToLower() == "table")
                        tbName = xParts[1];
                    if (xParts[0].ToLower() == "columns")
                        columns = Convert.ToInt32(xParts[1]);
                    if (xParts[0].ToLower() == "rows")
                        rows = Convert.ToInt32(xParts[1]);
                }
                TableData tmpTd = new TableData();
                tmpTd.TableName = tbName;
                Logger("Table: " + tbName);
                int id = findTableDataId(tmpTd, basefile.tableDatas);
                if (id > -1)
                {
                    tmpTd = basefile.tableDatas[id];
                    if (tmpTd.Rows == rows && tmpTd.Columns == columns)
                    {
                        isCompatible = true;
                        retVal = (uint)id;
                    }
                }
            }
        }
        else
        {
            string[] OSlist = xpatch.CompatibleOS.Split(',');
            string BinPN = "";
            foreach (string OS in OSlist)
            {
                Parts = OS.Split(':');
                if (Parts[0] == "ALL")
                {
                    isCompatible = true;
                    if (!HexToUint(Parts[1], out retVal))
                        throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                    Debug.WriteLine("ALL, Addr: " + Parts[1]);
                }
                else
                {
                    if (BinPN == "")
                    {
                        //Search OS once
                        for (int s = 0; s < basefile.Segments.Count; s++)
                        {
                            string PN = basefile.ReadInfo(basefile.segmentAddressDatas[s].PNaddr);
                            if (Parts[0] == PN)
                            {
                                isCompatible = true;
                                BinPN = PN;
                            }
                        }
                    }
                    if (Parts[0] == BinPN)
                    {
                        isCompatible = true;
                        if (!HexToUint(Parts[1], out retVal))
                            throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                        Debug.WriteLine("OS: " + BinPN + ", Addr: " + Parts[1]);
                    }
                }
            }
        }
        return retVal;
    }

    public static uint applyTablePatch(PcmFile basefile, XmlPatch xpatch, int tdId)
    {
        int diffCount = 0;
        frmTableEditor frmTE = new frmTableEditor();
        TableData pTd = basefile.tableDatas[tdId];
        frmTE.prepareTable(basefile, pTd, null, "A");
        //frmTE.loadTable();
        uint addr = (uint)(pTd.addrInt + pTd.Offset);
        uint step = (uint)getElementSize(pTd.DataType);
        try
        {
            string[] dataParts = xpatch.Data.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (int cell = 0; cell < frmTE.compareFiles[0].tableInfos[0].tableCells.Count; cell++)
            {
                TableCell tCell = frmTE.compareFiles[0].tableInfos[0].tableCells[cell];
                double val = Convert.ToDouble(dataParts[diffCount].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                if (val != Convert.ToDouble(tCell.origValue))
                    diffCount++;
                tCell.saveValue(val);
            }
            frmTE.saveTable();
            frmTE.Dispose();
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, patcherfunctions line " + line + ": " + ex.Message);

        }
        return (uint)(diffCount * step);
    }

    public static bool ApplyXMLPatch(PcmFile basefile)
    {
        try
        {
            string PrevSegment = "";
            uint ByteCount = 0;
            string[] Parts;
            string prevDescr = "";

            Logger("Applying patch:");
            foreach (XmlPatch xpatch in PatchList)
            {
                if (xpatch.Description != null && xpatch.Description != "" && xpatch.Description != prevDescr)
                    Logger(xpatch.Description);
                prevDescr = xpatch.Description;
                uint Addr = checkPatchCompatibility(xpatch, basefile);
                if (Addr < uint.MaxValue)
                {
                    if (xpatch.Segment != null && xpatch.Segment.Length > 0 && PrevSegment != xpatch.Segment)
                    {
                        PrevSegment = xpatch.Segment;
                        Logger("Segment: " + xpatch.Segment);
                    }
                    bool PatchRule = true; //If there is no rule, apply patch
                    if (xpatch.Rule != null && (xpatch.Rule.Contains(':') || xpatch.Rule.Contains('[')))
                    {
                        Parts = xpatch.Rule.Split(new char[] { ']', '[', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (Parts.Length == 3)
                        {
                            uint RuleAddr;
                            if (!HexToUint(Parts[0], out RuleAddr))
                                throw new Exception("Can't decode from HEX: " + Parts[0] + " (" + xpatch.Rule + ")");
                            ushort RuleMask;
                            if (!HexToUshort(Parts[1], out RuleMask))
                                throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.Rule + ")");
                            ushort RuleValue;
                            if (!HexToUshort(Parts[2].Replace("!", ""), out RuleValue))
                                throw new Exception("Can't decode from HEX: " + Parts[2] + " (" + xpatch.Rule + ")");

                            if (Parts[2].Contains("!"))
                            {
                                if ((basefile.buf[RuleAddr] & RuleMask) != RuleValue)
                                {
                                    PatchRule = true;
                                    Logger("Rule match, applying patch");
                                }
                                else
                                {
                                    PatchRule = false;
                                    Logger("Rule doesn't match, skipping patch");
                                }
                            }
                            else
                            {
                                if ((basefile.buf[RuleAddr] & RuleMask) == RuleValue)
                                {
                                    PatchRule = true;
                                    Logger("Rule match, applying patch");
                                }
                                else
                                {
                                    PatchRule = false;
                                    Logger("Rule doesn't match, skipping patch");
                                }
                            }

                        }
                    }
                    if (PatchRule)
                    {
                        if (xpatch.CompatibleOS.ToLower().StartsWith("table:"))
                        {
                            ByteCount += applyTablePatch(basefile, xpatch, (int)Addr);
                        }
                        else
                        {
                            Debug.WriteLine(Addr.ToString("X") + ":" + xpatch.Data);
                            Parts = xpatch.Data.Split(' ');
                            foreach (string Part in Parts)
                            {
                                //Actually add patch data:
                                if (Part.Contains("[") || Part.Contains(":"))
                                {
                                    //Set bits / use Mask
                                    byte Origdata = basefile.buf[Addr];
                                    Debug.WriteLine("Set address: " + Addr.ToString("X") + ", old data: " + Origdata.ToString("X"));
                                    string[] dataparts;
                                    dataparts = Part.Split(new char[] { ']', '[', ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    byte Setdata = byte.Parse(dataparts[0], System.Globalization.NumberStyles.HexNumber);
                                    byte Mask = byte.Parse(dataparts[1].Replace("]", ""), System.Globalization.NumberStyles.HexNumber);

                                    // Set 1
                                    byte tmpMask = (byte)(Mask & Setdata);
                                    byte Newdata = (byte)(Origdata | tmpMask);

                                    // Set 0
                                    tmpMask = (byte)(Mask & ~Setdata);
                                    Newdata = (byte)(Newdata & ~tmpMask);

                                    Debug.WriteLine("New data: " + Newdata.ToString("X"));
                                    basefile.buf[Addr] = Newdata;
                                }
                                else
                                {
                                    //Set byte
                                    if (Part != "*") //Skip wildcards
                                        basefile.buf[Addr] = Byte.Parse(Part, System.Globalization.NumberStyles.HexNumber);
                                }
                                Addr++;
                                ByteCount++;
                            }
                        }
                    }
                    if (xpatch.PostMessage != null && xpatch.PostMessage.Length > 1)
                        LoggerBold(xpatch.PostMessage);
                }
                else
                {
                    Logger("Patch is not compatible");
                    return false;
                }
            }
            Logger("Applied: " + ByteCount.ToString() + " Bytes");
            Logger("You can save BIN file now");
        }
        catch (Exception ex)
        {
            Logger("Error: " + ex.Message);
            return false;
        }
        return true;
    }

    public static bool compareTables(int id1, int id2, PcmFile pcm1, PcmFile pcm2)
    {
        bool match = true;

        TableData td1 = pcm1.tableDatas[id1];
        TableData td2 = pcm2.tableDatas[id2];

        if ((td1.Rows * td1.Columns) != (td2.Rows * td2.Columns))
            return false;
        List<double> tableValues = new List<double>();
        uint addr = (uint)(td1.addrInt + td1.Offset);
        uint step = (uint)getElementSize(td1.DataType);
        if (td1.RowMajor)
        {
            for (int r = 0; r < td1.Rows; r++)
            {
                for (int c = 0; c < td1.Columns; c++)
                {
                    double val = getValue(pcm1.buf,addr,td1,0,pcm1);
                    tableValues.Add(val);
                    addr += step;
                }
            }
        }
        else
        {
            for (int c = 0; c < td1.Columns; c++)
            {
                for (int r = 0; r < td1.Rows; r++)
                {
                    double val = getValue(pcm1.buf, addr, td1, 0, pcm1);
                    tableValues.Add(val);
                    addr += step;
                }
            }
        }


        addr = (uint)(td2.addrInt + td2.Offset);
        step = (uint)getElementSize(td2.DataType);
        int i = 0;
        if (td2.RowMajor)
        {
            for (int r = 0; r < td2.Rows; r++)
            {
                for (int c = 0; c < td2.Columns; c++)
                {
                    double val = getValue(pcm2.buf, addr, td2, 0,pcm2);
                    if (val != tableValues[i])
                    {
                        match = false;
                        break;
                    }
                    addr += step;
                    i++;
                }
            }
        }
        else
        {
            for (int c = 0; c < td2.Columns; c++)
            {
                for (int r = 0; r < td2.Rows; r++)
                {
                    double val = getValue(pcm2.buf, addr, td2, 0, pcm2);
                    if (val != tableValues[i])
                    {
                        match = false;
                        break;
                    }
                    addr += step;
                    i++;
                }
            }
        }

        return match;
    }

    public static byte[] ReadBin(string FileName, uint FileOffset, uint Length)
    {

        byte[] buf = new byte[Length];

        long offset = 0;
        long remaining = Length;

        using (BinaryReader freader = new BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
            freader.BaseStream.Seek(FileOffset, 0);
            while (remaining > 0)
            {
                int read = freader.Read(buf, (int)offset, (int)remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
            freader.Close();
        }
        return buf;
    }


    public static void WriteBinToFile(string FileName, byte[] Buf)
    {

        using (FileStream stream = new FileStream(FileName, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Buf);
                writer.Close();
            }
        }
    }

    public static void WriteSegmentToFile(string FileName, List<Block> Addr, byte[] Buf)
    {

        using (FileStream stream = new FileStream(FileName, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                for (int b=0;b<Addr.Count;b++)
                {
                    uint StartAddr = Addr[b].Start;
                    uint Length = Addr[b].End - Addr[b].Start + 1;
                    writer.Write(Buf, (int)StartAddr, (int)Length);
                }
                writer.Close();
            }
        }

    }

    public static string ReadTextFile(string fileName)
    {
        StreamReader sr = new StreamReader(fileName);
        string fileContent = sr.ReadToEnd();
        sr.Close();
        return fileContent;
    }

    public static void WriteTextFile(string fileName, string fileContent)
    {
        using (StreamWriter writetext = new StreamWriter(fileName))
        {
            writetext.Write(fileContent);
        }

    }
    public static void saveOBD2Codes()
    {
        string OBD2CodeFile = Path.Combine(Application.StartupPath, "XML", "OBD2Codes.xml");
        Logger("Saving file " + OBD2CodeFile + "...", false);
        using (FileStream stream = new FileStream(OBD2CodeFile, FileMode.Create))
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<OBD2Code>));
            writer.Serialize(stream, OBD2Codes);
            stream.Close();
        }
        Logger(" [OK]");

    }
    public static void loadOBD2Codes()
    {
        string OBD2CodeFile = Path.Combine(Application.StartupPath, "XML", "OBD2Codes.xml");
        if (File.Exists(OBD2CodeFile))
        {
            Debug.WriteLine("Loading OBD2Codes.xml");
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<OBD2Code>));
            System.IO.StreamReader file = new System.IO.StreamReader(OBD2CodeFile);
            OBD2Codes = (List<OBD2Code>)reader.Deserialize(file);
            file.Close();
        }
        else
        {
            OBD2Codes = new List<OBD2Code>();
        }

    }

    public static string autoDetect(PcmFile PCM)
    {
        AutoDetect autod = new AutoDetect();
        return autod.autoDetect(PCM);
    }
    public static uint CalculateChecksum(byte[] Data, AddressData CSAddress, List<Block> CSBlocks,List<Block> ExcludeBlocks, short Method, short Complement, ushort Bytes, Boolean SwapB)
    {
        Debug.WriteLine("Calculating checksum, method: " + Method);
        uint sum = 0;
        uint BufSize = 0;
        List<Block> Blocks = new List<Block>();
        
        for (int p = 0; p < CSBlocks.Count; p++)
        {
            Block B = new Block();
            B.Start = CSBlocks[p].Start;
            B.End = CSBlocks[p].End;
            if (CSAddress.Address >= B.Start && CSAddress.Address <= B.End)
            {
                //Checksum  is located in this block
                if (CSAddress.Address == B.Start)    //At beginning of segment
                {
                    //At beginning of segment
                    Debug.WriteLine("Checksum is at start of block, skipping");
                    B.Start += CSAddress.Bytes;
                }
                else
                {
                    //Located at middle of block, create new block C, ending before checksum
                    Debug.WriteLine("Checksum is at middle of block, skipping");
                    Block C = new Block();
                    C.Start = B.Start;
                    C.End = CSAddress.Address - 1;
                    Blocks.Add(C);
                    BufSize += C.End - C.Start + 1;
                    B.Start = CSAddress.Address + CSAddress.Bytes; //Move block B to start after Checksum
                }
            }
            foreach (Block ExcBlock in ExcludeBlocks)
            {
                if (ExcBlock.Start >= B.Start && ExcBlock.End <= B.End)
                {
                    //Excluded block in this block
                    if (ExcBlock.Start == B.Start)    //At beginning of segment, move start of block
                    {
                        B.Start = ExcBlock.End + 1;
                    }
                    else
                    {
                        if (ExcBlock.End < B.End -1)
                        {
                            //Located at middle of block, create new block C, ending before excluded block
                            Block C = new Block();
                            C.Start = B.Start;
                            C.End = ExcBlock.Start - 1;
                            Blocks.Add(C);
                            BufSize += C.End - C.Start + 1;
                            B.Start = ExcBlock.End + 1; //Move block B to start after excluded block
                        }
                        else
                        {
                            //Exclude block at end of block, move end of block backwards
                            B.End = ExcBlock.Start - 1;
                        }
                    }
                }
            }
            Blocks.Add(B);
            BufSize += B.End - B.Start + 1;
        }

        byte[] tmp = new byte[BufSize];
        uint Offset = 0;
        foreach (Block B in Blocks)
        {
            //Copy blocks to tmp array for calculation
            Debug.WriteLine("Block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
            uint BlockSize = B.End - B.Start + 1;
            Array.Copy(Data, B.Start, tmp, Offset, BlockSize);
            Offset += BlockSize;
        }

        if (Method == CSMethod_Bytesum)
        {
            for (uint i = 0; i < tmp.Length; i++)
            {
                sum += tmp[i];
            }
        }

        if (Method == CSMethod_Wordsum)
        {
            for (uint i = 0; i < tmp.Length - 1; i += 2)
            {
                sum += BEToUint16(tmp, i);
            }
        }

        if (Method == CSMethod_Dwordsum)
        { 
            for (uint i = 0; i < tmp.Length - 3; i += 4)
            {
                sum += BEToUint32(tmp, i);
            }
        }

        if (Method == CSMethod_crc16)
        { 
            Crc16 C16 = new Crc16();
            sum = C16.ComputeChecksum(tmp);            
        }

        if (Method == CSMethod_crc32)
        { 
            Crc32 C32 = new Crc32();
            sum = C32.ComputeChecksum(tmp);
        }


        if (Complement == 1)
        {
            sum = ~sum;
        }
        if (Complement == 2)
        {
            sum = ~sum + 1;
        }

        if (Bytes == 1)
            sum = (sum & 0xFF);
        //if (Bytes == 2 || Bytes == 0) //Bytes = 0 if not saving
        if (Bytes == 2) //Bytes = 0 if not saving
        {
            sum = (sum & 0xFFFF);
        }
        if (SwapB)
        {
            if (Method == CSMethod_Wordsum || Method == CSMethod_crc16)
                sum = (ushort)SwapBytes((ushort)sum);
            else
                sum = SwapBytes(sum);

        }
        Debug.WriteLine("Result: " + sum.ToString("X"));
        return sum;
    }

    public static bool ParseAddress(string Line, PcmFile PCM, out List<Block> Blocks)
    {
        Debug.WriteLine("Segment address line: " + Line);
        Blocks = new List<Block>();

        if (Line == null || Line == "")
        {
            Block B = new Block();
            B.End = PCM.fsize;
            B.Start = 0;
            Blocks.Add(B);
            return true;
        }

        string[] Parts = Line.Split(',');
        int i = 0;

        foreach (string Part in Parts)
        {
            string[] StartEnd = Part.Split('-');
            Block B = new Block();
            int Offset = 0;

            if (StartEnd[0].Contains(">"))
            {
                string[] SO = StartEnd[0].Split('>');
                StartEnd[0] = SO[0];
                uint x;
                if (!HexToUint(SO[1], out x))
                    throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                Offset = (int)x;
            }
            if (StartEnd[0].Contains("<"))
            {
                string[] SO = StartEnd[0].Split('<');
                StartEnd[0] = SO[0];
                uint x;
                if (!HexToUint(SO[1], out x))
                    throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                Offset = ~(int)x;
            }


            if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
            {
                throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")");
            }
            if (StartEnd[0].StartsWith("@"))
            {
                uint tmpStart = B.Start;
                B.Start = BEToUint32(PCM.buf, tmpStart);
                B.End = BEToUint32(PCM.buf, tmpStart + 4);
                tmpStart += 8;
            }
            else
            {
                if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                    throw new Exception("Can't decode from HEX: " + StartEnd[1].Replace("@", "") + " (" + Line + ")");
                if (B.End >= PCM.buf.Length)    //Make 1MB config work with 512kB bin
                    B.End = (uint)PCM.buf.Length - 1;
            }
            if (StartEnd.Length > 1 && StartEnd[1].StartsWith("@"))
            {
                //Read End address from bin at this address
                B.End = BEToUint32(PCM.buf, B.End);
            }
            if (StartEnd.Length > 1 && StartEnd[1].EndsWith("@"))
            {
                //Address is relative to end of bin
                uint end;
                if (HexToUint(StartEnd[1].Replace("@", ""), out end))
                    B.End = (uint)PCM.buf.Length - end - 1;
            }
            B.Start += (uint)Offset;
            B.End += (uint)Offset;
            Blocks.Add(B);
            i++;
        }
        foreach (Block B in Blocks)
            Debug.WriteLine("Address block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
        return true;
    }


    public static List<string> SelectMultipleFiles(string Title = "Select files", string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*", string defaultFile = "")
    {
        List<string> fileList = new List<string>();

        OpenFileDialog fdlg = new OpenFileDialog();
        if (Filter.Contains("BIN"))
        {
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            Filter = "BIN files (*.bin)|*.bin";
            for (int f = 0; f < fileTypeList.Count; f++)
            {
                string newFilter = "|" + fileTypeList[f].Description + "|" + "*." + fileTypeList[f].Extension;
                Filter += newFilter;
            }
            Filter += "|All files (*.*)|*.*";
        }
        else if (Filter.ToLower().Contains("xdf"))
        {
            fdlg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions");
        }
        else if (defaultFile.Length > 0)
        {

            fdlg.FileName = Path.GetFileName(defaultFile);
            fdlg.InitialDirectory = Path.GetDirectoryName(defaultFile);
        }
        else
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
            if (Filter.Contains("PATCH") || Filter.Contains("TXT"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        }

        fdlg.Title = Title;
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;
        fdlg.Multiselect = true;
        if (fdlg.ShowDialog() == DialogResult.OK)
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastXMLfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("BIN"))
                UniversalPatcher.Properties.Settings.Default.LastBINfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastPATCHfolder = Path.GetDirectoryName(fdlg.FileName);
            UniversalPatcher.Properties.Settings.Default.Save();
            foreach (string fName in fdlg.FileNames)
                fileList.Add(fName);
        }
        return fileList;

    }


    public static string SelectFile(string Title = "Select file", string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*", string defaultFile = "")
    {
        OpenFileDialog fdlg = new OpenFileDialog();
        if (Filter.Contains("BIN"))
        {
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            Filter = "BIN files (*.bin)|*.bin";
            for (int f = 0; f < fileTypeList.Count; f++)
            {
                string newFilter = "|" + fileTypeList[f].Description + "|" + "*." + fileTypeList[f].Extension;
                Filter += newFilter;
            }
            Filter += "|All files (*.*)|*.*";
        }
        else if (Filter.ToLower().Contains("xdf"))
        {
            fdlg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions");
        }
        else if (defaultFile.Length > 0)
        {

            fdlg.FileName = Path.GetFileName(defaultFile);
            fdlg.InitialDirectory = Path.GetDirectoryName(defaultFile);
        }
        else
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
            if (Filter.Contains("PATCH") || Filter.Contains("TXT"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        }

        fdlg.Title = Title;
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;

        if (fdlg.ShowDialog() == DialogResult.OK)
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastXMLfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("BIN"))
                UniversalPatcher.Properties.Settings.Default.LastBINfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastPATCHfolder = Path.GetDirectoryName(fdlg.FileName);
            UniversalPatcher.Properties.Settings.Default.Save();
            return fdlg.FileName;
        }
        return "";

    }
    public static string SelectSaveFile(string Filter = "BIN files (*.bin)|*.bin", string defaultFileName = "")
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        //saveFileDialog.Filter = "BIN files (*.bin)|*.bin";
        saveFileDialog.Filter = Filter;
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.Title = "Save to file";
        if (defaultFileName.Length > 0)
        {
            saveFileDialog.FileName = Path.GetFileName(defaultFileName);
            string defPath = Path.GetDirectoryName(defaultFileName);
            if (defPath != "")
            {
                saveFileDialog.InitialDirectory = defPath;
            }
        }
        else
        {
            if (Filter.Contains("PATCH"))
                saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
            else if (Filter.Contains("BIN"))
                saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            else if (Filter.Contains("XDF"))
                saveFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions");
        }

        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastXMLfolder = Path.GetDirectoryName(saveFileDialog.FileName);
            else if (Filter.Contains("BIN"))
                UniversalPatcher.Properties.Settings.Default.LastBINfolder = Path.GetDirectoryName(saveFileDialog.FileName);
            else if (Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastPATCHfolder = Path.GetDirectoryName(saveFileDialog.FileName);
            UniversalPatcher.Properties.Settings.Default.Save();
            return saveFileDialog.FileName;
        }
        else
            return "";

    }

    public static string SelectFolder(string Title, string defaultFolder = "")
    {
        string folderPath = "";
        OpenFileDialog folderBrowser = new OpenFileDialog();
        // Set validate names and check file exists to false otherwise windows will
        // not let you select "Folder Selection."
        folderBrowser.ValidateNames = false;
        folderBrowser.CheckFileExists = false;
        folderBrowser.CheckPathExists = true;
        if (defaultFolder.Length > 0)
            folderBrowser.InitialDirectory = defaultFolder;
        else
            folderBrowser.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
        // Always default to Folder Selection.
        folderBrowser.Title = Title;
        folderBrowser.FileName = "Folder Selection";
        if (folderBrowser.ShowDialog() == DialogResult.OK)
        {
            folderPath = Path.GetDirectoryName(folderBrowser.FileName);
            UniversalPatcher.Properties.Settings.Default.LastBINfolder = folderPath;
            UniversalPatcher.Properties.Settings.Default.Save();
        }
        return folderPath;
    }

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

    public static void OpenFolderAndSelectItem(string folderPath, string file)
    {
        IntPtr nativeFolder;
        uint psfgaoOut;
        SHParseDisplayName(folderPath, IntPtr.Zero, out nativeFolder, 0, out psfgaoOut);

        if (nativeFolder == IntPtr.Zero)
        {
            // Log error, can't find folder
            return;
        }

        IntPtr nativeFile;
        SHParseDisplayName(Path.Combine(folderPath, file), IntPtr.Zero, out nativeFile, 0, out psfgaoOut);

        IntPtr[] fileArray;
        if (nativeFile == IntPtr.Zero)
        {
            // Open the folder without the file selected if we can't find the file
            fileArray = new IntPtr[0];
        }
        else
        {
            fileArray = new IntPtr[] { nativeFile };
        }

        SHOpenFolderAndSelectItems(nativeFolder, (uint)fileArray.Length, fileArray, 0);

        Marshal.FreeCoTaskMem(nativeFolder);
        if (nativeFile != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(nativeFile);
        }
    }

    public static string CheckStockCVN(string PN, string Ver, string SegNr, string cvn, bool AddToList, string XMLFile)
    {
        string retVal = "[n/a]";
        for (int c = 0; c < StockCVN.Count; c++)
        {
            //if (StockCVN[c].XmlFile == Path.GetFileName(XMLFile) && StockCVN[c].PN == PN && StockCVN[c].Ver == Ver && StockCVN[c].SegmentNr == SegNr && StockCVN[c].cvn == cvn)
            if (StockCVN[c].PN == PN && StockCVN[c].Ver == Ver && StockCVN[c].SegmentNr == SegNr)
            {
                if (Path.GetFileName(XMLFile) != StockCVN[c].XmlFile && StockCVN[c].AlternateXML == null)
                {
                    CVN c1 = StockCVN[c];
                    c1.AlternateXML = Path.GetFileName(XMLFile);
                    StockCVN.RemoveAt(c);
                    StockCVN.Insert(c, c1);
                }
                if (StockCVN[c].cvn == cvn)
                {
                    retVal = "[stock]";
                    break;
                }
                else
                {
                    retVal = "[modded]";
                    break;
                    //return "[modded]";
                }
            }
        }


        if (retVal == "[n/a]")
        {
            //Check if it's in referencelist
            bool cvnMismatch = false;
            uint refC = 0;
            uint cvnInt = 0;
            if (!HexToUint(cvn, out cvnInt))
            {
                throw new Exception("Can't convert from HEX: " + cvn);
            }
            string refString = "";
            if (referenceCvnList == null) return "";
            for (int r = 0; r < referenceCvnList.Count; r++)
            {
                if (PN == referenceCvnList[r].PN)
                {
                    refString = referenceCvnList[r].CVN;
                    cvnMismatch = true;    //Found from referencelist, match not found YET
                    if (!HexToUint(referenceCvnList[r].CVN, out refC))
                    {
                        throw new Exception("Can't convert from HEX: " + referenceCvnList[r].CVN);
                    }
                    if (refC == cvnInt)
                    {
                        Debug.WriteLine("PN: " + PN + " CVN found from Referencelist: " + referenceCvnList[r].CVN);
                        cvnMismatch = false; //Found from referencelist, no mismatch
						retVal = "[stock]";

                    }
                    ushort refShort;
                    if (!HexToUshort(referenceCvnList[r].CVN, out refShort))
                    {
                        throw new Exception("Can't convert from HEX: " + referenceCvnList[r].CVN);
                    }
                    if (SwapBytes(refShort) == cvnInt)
                    {
                        Debug.WriteLine("PN: " + PN + " byteswapped CVN found from Referencelist: " + referenceCvnList[r].CVN);
                        cvnMismatch = false; //Found from referencelist, no mismatch
						retVal = "[stock]";
                    }
                    else
                    {
                        Debug.WriteLine("Byte swapped CVN doesn't match: " + SwapBytes(refShort).ToString("X") + " <> " + cvnInt.ToString("X"));
                    }
                    break;
                }
            }

            if (cvnMismatch) //Found from referencelist, have mismatch
            {
                retVal = "[modded/R]";
                bool isInBadCvnList = false;
                AddToList = false;  //Don't add to CVN list if add to mismatch CVN
                if (BadCvnList == null)
                    BadCvnList = new List<CVN>();
                for (int i = 0; i < BadCvnList.Count; i++)
                {
                    if (BadCvnList[i].PN == PN && BadCvnList[i].cvn == cvn)
                    {
                        isInBadCvnList = true;
                        Debug.WriteLine("PN: " + PN + ", CVN: " + cvn + " is already in badCvnList");
                        break;
                    }
                }
                if (!isInBadCvnList)
                {
                    Debug.WriteLine("Adding PN: " + PN + ", CVN: " + cvn + " to badCvnList");
                    CVN C1 = new CVN();
                    C1.cvn = cvn;
                    C1.PN = PN;
                    C1.SegmentNr = SegNr;
                    C1.Ver = Ver;
                    C1.XmlFile = Path.GetFileName(XMLFile);
                    C1.ReferenceCvn = refString.TrimStart('0');
                    BadCvnList.Add(C1);

                }
            }
        }

        if (AddToList && retVal != "[stock]")
        {
            bool IsinCVNlist = false;
            if (ListCVN == null)
                ListCVN = new List<CVN>();
            for (int c = 0; c < ListCVN.Count; c++)
            {
                //if (ListCVN[c].XmlFile == Path.GetFileName(XMLFile) && ListCVN[c].PN == PN && ListCVN[c].Ver == Ver && ListCVN[c].SegmentNr == SegNr && ListCVN[c].cvn == cvn)
                if (ListCVN[c].PN == PN && ListCVN[c].Ver == Ver && ListCVN[c].SegmentNr == SegNr && ListCVN[c].cvn == cvn)
                {
                    Debug.WriteLine("Already in CVN list: " + cvn);
                    IsinCVNlist = true;
                    break;
                }
            }
            if (!IsinCVNlist)
            {
                CVN C1 = new CVN();
                C1.cvn = cvn;
                C1.PN = PN;
                C1.SegmentNr = SegNr;
                C1.Ver = Ver;
                C1.XmlFile = Path.GetFileName(XMLFile);
                for (int r = 0; r < referenceCvnList.Count; r++)
                {
                    if (referenceCvnList[r].PN == C1.PN)
                    {
                        C1.ReferenceCvn = referenceCvnList[r].CVN.TrimStart('0');
                        break;
                    }
                }
                ListCVN.Add(C1);
            }
        }

        return retVal;
    }

    public static void loadReferenceCvn()
    {
        string FileName = Path.Combine(Application.StartupPath, "XML", "Reference-CVN.xml");
        if (!File.Exists(FileName))
            return;
        referenceCvnList = new List<referenceCvn>();
        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<referenceCvn>));
        System.IO.StreamReader file = new System.IO.StreamReader(FileName);
        referenceCvnList = (List<referenceCvn>)reader.Deserialize(file);
        file.Close();
        foreach (referenceCvn refCvn in referenceCvnList)
        {
            for (int i = 0; i < StockCVN.Count; i++)
            {
                if (StockCVN[i].PN == refCvn.PN)
                {
                    CVN C1 = StockCVN[i];
                    C1.ReferenceCvn = refCvn.CVN;
                    StockCVN.RemoveAt(i);
                    StockCVN.Insert(i, C1);
                }
            }
        }
    }

    public static uint searchBytes(PcmFile PCM, string searchString, uint Start, uint End, ushort stopVal = 0)
    {
        uint addr;
        try
        {
            string[] searchParts = searchString.Trim().Split(' ');
            byte[] bytes = new byte[searchParts.Length];

            for (int b = 0; b < searchParts.Length; b++)
            {
                byte searchval = 0;
                if (searchParts[b] != "*")
                    HexToByte(searchParts[b], out searchval);
                bytes[b] = searchval;
            }

            for (addr = Start; addr < End; addr++)
            {
                bool match = true;
                if (stopVal != 0 && BEToUint16(PCM.buf, addr) == stopVal)
                {
                    return uint.MaxValue;
                }
                if ((addr + searchParts.Length) > PCM.fsize)
                    return uint.MaxValue;
                for (uint part = 0; part < searchParts.Length; part++)
                {
                    if (searchParts[part] != "*")
                    {
                        if (PCM.buf[addr + part] != bytes[part])
                        {
                            match = false;
                            break;
                        }
                    }
                }
                if (match)
                {
                    return addr;
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
            Debug.WriteLine("Error searchBytes, line " + line + ": " + ex.Message);
        }
        return uint.MaxValue;
    }


    public static SearchedAddress getAddrbySearchString(PcmFile PCM, string searchStr, ref uint startAddr, uint endAddr, bool conditionalOffset = false)
    {
        SearchedAddress retVal;
        retVal.Addr = uint.MaxValue;
        retVal.Columns = 0;
        retVal.Rows = 0;
        try
        {
            string modStr = searchStr.Replace("r", "");
            modStr = modStr.Replace("k", "");
            modStr = modStr.Replace("x", "");
            modStr = modStr.Replace("y", "");
            modStr = modStr.Replace("@", "*");
            modStr = modStr.Replace("# ", "* "); //# alone at beginning or middle
            if (modStr.EndsWith("#"))
                modStr = modStr.Replace(" #", " *"); //# alone at end
            modStr = modStr.Replace("#", ""); //For example: #21 00 21
            uint addr = searchBytes(PCM, modStr, startAddr, endAddr);
            if (addr == uint.MaxValue)
            {
                //Not found
                startAddr = uint.MaxValue;
                return retVal;
            }

            string[] sParts = searchStr.Trim().Split(' ');
            startAddr = addr + (uint)sParts.Length;

            int[] locations = new int[4];
            int l = 0;
            string addrStr = "*";
            if (searchStr.Contains("@")) addrStr = "@";
            else if (searchStr.Contains("*") || searchStr.Contains("#")) addrStr = "*";
            else
            {
                //Address is AFTER searchstring
                retVal.Addr = BEToUint32(PCM.buf, addr + (uint)sParts.Length);
            }
            for (int p = 0; p < sParts.Length; p++)
            {
                if (sParts[p].Contains(addrStr) && l < 4)
                {
                    locations[l] = p;
                    l++;
                }
                if (sParts[p].Contains("r") || sParts[p].Contains("x"))
                {
                    retVal.Rows = (ushort)PCM.buf[(uint)(addr + p)];
                }
                if (sParts[p].Contains("k") || sParts[p].Contains("y"))
                {
                    retVal.Columns = (ushort) PCM.buf[(uint)(addr + p)];
                }
                if (sParts[p].Contains("#"))
                {
                    retVal.Addr = (uint)(addr + p);
                }

            }
            if (retVal.Addr < uint.MaxValue)
            {
                return retVal;
            }

            //We are here, so we must have @ @ @ @  in searchsting
            if (l < 4)
            {
                Logger("Less than 4 @ in searchstring, address need 4 bytes! (" + searchStr + ")");
                retVal.Addr = uint.MaxValue;
            }

            retVal.Addr = (uint)(PCM.buf[addr + locations[0]] << 24 | PCM.buf[addr + locations[1]] << 16 | PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
            if (conditionalOffset)
            {
                ushort addrWord = (ushort)(PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                if (addrWord > 0x5000)
                    retVal.Addr -= 0x10000;
            }
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.WriteLine ("getAddrbySearchString, line " + line + ": " + ex.Message);
        }
        return retVal;
    }


    public static uint searchWord(PcmFile PCM, ushort sWord, uint Start, uint End, ushort stopVal = 0)
    {
        for (uint addr = Start; addr < End; addr++)
        {
            if (stopVal != 0 && BEToUint16(PCM.buf, addr) == stopVal)
            {
                return uint.MaxValue;
            }
            if (BEToUint16(PCM.buf, addr) == sWord)
            { 
                return addr;
            }
        }
        return uint.MaxValue;
    }


    public static bool HexToUint64(string Hex, out UInt64 x)
    {
        x = 0;
        if (!UInt64.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }

    public static bool HexToUint(string Hex, out uint x)
    {
        x = 0;
        if (!UInt32.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }
    public static bool HexToInt(string Hex, out int x)
    {
        x = 0;
        if (!int.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }

    public static bool HexToUshort(string Hex, out ushort x)
    {
        x = 0;
        if (!UInt16.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }
    public static bool HexToByte(string Hex, out byte x)
    {
        x = 0;
        if (!byte.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }

    public static string ReadTextBlock(byte[] buf, int Address, int Bytes)
    {
        string Result = System.Text.Encoding.ASCII.GetString(buf, (int)Address, Bytes);
        Result = Regex.Replace(Result, "[^a-zA-Z0-9]", "?");
        return Result;
    }

    public static UInt64 BEToUint64(byte[] buf, uint offset)
    {
        byte[] tmp = new byte[8];
        Array.Copy(buf, offset, tmp, 0, 8);
        Array.Reverse(tmp);
        return BitConverter.ToUInt64(tmp,0);
    }

    public static Int64 BEToInt64(byte[] buf, uint offset)
    {
        byte[] tmp = new byte[8];
        Array.Copy(buf, offset, tmp, 0, 8);
        Array.Reverse(tmp);
        return BitConverter.ToInt64(tmp, 0);
    }
    public static Double BEToFloat64(byte[] buf, uint offset)
    {
        byte[] tmp = new byte[8];
        Array.Copy(buf, offset, tmp, 0, 8);
        Array.Reverse(tmp);
        return BitConverter.ToDouble(tmp, 0);
    }
    public static float BEToFloat32(byte[] buf, uint offset)
    {
        byte[] tmp = new byte[4];
        Array.Copy(buf, offset, tmp, 0, 4);
        Array.Reverse(tmp);
        return BitConverter.ToSingle(tmp, 0);
    }

    public static uint BEToUint32(byte[] buf, uint offset)
    {
        //Shift first byte 24 bits left, second 16bits left...
        return (uint)((buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
    }

    public static UInt16 BEToUint16(byte[] buf, uint offset)
    {
        return (UInt16)((buf[offset] << 8) | buf[offset + 1]);
    }

    public static int BEToInt32(byte[] buf, uint offset)
    {
        //Shift first byte 24 bits left, second 16bits left...
        return (int)((buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
    }

    public static Int16 BEToInt16(byte[] buf, uint offset)
    {
        return (Int16)((buf[offset] << 8) | buf[offset + 1]);
    }
    public static void SaveFloat32(byte[] buf, uint offset, Single data)
    {
        byte[] tmp = new byte[4];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 4);
    }
    public static void SaveFloat64(byte[] buf, uint offset, double data)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 8);
    }

    public static void SaveUint64(byte[] buf, uint offset, UInt64 data)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp,0,buf,offset,8);
    }

    public static void SaveInt64(byte[] buf, uint offset, Int64 data)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 8);
    }
    public static void SaveUint32(byte[] buf, uint offset, UInt32 data)
    {
        byte[] tmp = new byte[4];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 4);
    }
    public static void SaveInt32(byte[] buf, uint offset, Int32 data)
    {
        byte[] tmp = new byte[4];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 4);
    }
    public static void SaveUshort(byte[] buf, uint offset, ushort data)
    {
        byte[] tmp = new byte[2];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 2);
    }
    public static void SaveShort(byte[] buf, uint offset, short data)
    {
        byte[] tmp = new byte[2];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 2);
    }

    public static ushort SwapBytes(ushort x)
    {
        return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
    }

    public static uint SwapBytes(uint x)
    {
        return ((x & 0x000000ff) << 24) +
               ((x & 0x0000ff00) << 8) +
               ((x & 0x00ff0000) >> 8) +
               ((x & 0xff000000) >> 24);
    }

    public static UInt64 SwapBytes(UInt64 data)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        Array.Reverse(tmp);
        return BitConverter.ToUInt64(tmp, 0);
    }

    public static void UseComboBoxForEnums(DataGridView g)
    {
        try
        {
            g.Columns.Cast<DataGridViewColumn>()
             .Where(x => x.ValueType.IsEnum && x.GetType() != typeof(DataGridViewComboBoxColumn))
             .ToList().ForEach(x =>
             {
                 var index = x.Index;
                 g.Columns.RemoveAt(index);
                 var c = new DataGridViewComboBoxColumn();
                 c.ValueType = x.ValueType;
                 c.ValueMember = "Value";
                 c.DisplayMember = "Name";
                 c.DataPropertyName = x.DataPropertyName;
                 c.HeaderText = x.HeaderText;
                 c.Name = x.Name;
                 if (x.ValueType.IsEnum)
                 {
                     c.DataSource = Enum.GetValues(x.ValueType).Cast<object>().Select(v => new
                     {
                         Value = (int)v,
                         Name = Enum.GetName(x.ValueType, v) /* or any other logic to get text */
                     }).ToList();
                 }

                 g.Columns.Insert(index, c);
             });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }


    public static int findTableDataId(TableData refTd, List<TableData> tdList)
    {
        int pos1 = refTd.TableName.IndexOf("*");
        if (pos1 < 0)
            pos1 = refTd.TableName.Length;

        string refTableName = refTd.TableName.ToLower().Substring(0, pos1).Replace(" ", "_");
        for (int t = 0; t < tdList.Count; t++)
        {
            int pos2 = tdList[t].TableName.IndexOf("*");
            if (pos2 < 0)
                pos2 = tdList[t].TableName.Length;
            //if (pcm1.tableDatas[t].TableName.ToLower().Substring(0, pos2) == refTd.TableName.ToLower().Substring(0, pos1) && pcm1.tableDatas[t].Category.ToLower() == refTd.Category.ToLower())
            if (tdList[t].TableName.ToLower().Substring(0, pos2).Replace(" ","_") == refTableName)
            {
                return t;
            }
        }
        //Not found (exact match) maybe close enough?
        int required = UniversalPatcher.Properties.Settings.Default.TunerMinTableEquivalency;
        if (required == 100)
            return -1;  //already searched for 100% match
        for (int t = 0; t < tdList.Count; t++)
        {
            int pos2 = tdList[t].TableName.IndexOf("*");
            if (pos2 < 0)
                pos2 = tdList[t].TableName.Length;
            double percentage = ComputeSimilarity.CalculateSimilarity(tdList[t].TableName.ToLower().Substring(0, pos2).Replace(" ", "_"), refTableName);
            if ((int)(percentage * 100) >= required )
            {
                Debug.WriteLine(refTd.TableName + " <=> " + tdList[t].TableName + "; Equivalency: " + (percentage * 100).ToString() + "%");
                return t;
            }
        }

        return -1;
    }

    public static void Logger(string LogText, Boolean NewLine = true)
    {
        try
        {
            frmpatcher.Logger(LogText, NewLine);
            for (int l = LogReceivers.Count - 1; l >= 0;  l--)
            {
                if (LogReceivers[l].IsDisposed)
                    LogReceivers.RemoveAt(l);
            }
            for (int l=0; l< LogReceivers.Count; l++)
            {
                LogReceivers[l].AppendText(LogText);
                if (NewLine)
                    LogReceivers[l].AppendText(Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
    }
    public static void LoggerBold(string LogText, Boolean NewLine = true)
    {
        try
        {
            frmpatcher.LoggerBold(LogText, NewLine);
            for (int l = LogReceivers.Count - 1; l >= 0; l--)
            {
                if (LogReceivers[l].IsDisposed)
                    LogReceivers.RemoveAt(l);
            }
            for (int l = 0; l < LogReceivers.Count; l++)
            {
                LogReceivers[l].SelectionFont = new Font(LogReceivers[l].Font, FontStyle.Bold);
                LogReceivers[l].AppendText(LogText);
                LogReceivers[l].SelectionFont = new Font(LogReceivers[l].Font, FontStyle.Regular);
                if (NewLine)
                    LogReceivers[l].AppendText(Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
    }


}