using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UniversalPatcher;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UniversalPatcher.Properties;

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
     2. Addresses for information is collected from file (BinFile). 
        For example (OS 12579405): read EngineCal segment address from address 514 => SegmentBlocks => Block1 = 8000 - 15DFFF
        PNAddr is segment address +4 => PNaddr = 8004
     3. Read information from file using collected addresses (SegmentInfo)     
     */
    public struct BinFile
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
        public string FileName  { get; set; }
        public string XmlFile  { get; set; }
        public string Address { get; set; }
        public string SwapAddress { get; set; }
        public string Size  { get; set; }
        public string SwapSize { get; set; }
        public string CS1  { get; set; }
        public string CS2  { get; set; }
        public string CS1Calc { get; set; }
        public string CS2Calc  { get; set; }
        public string cvn { get; set; }
        public string Stock  { get; set; }
        public string PN  { get; set; }
        public string Ver  { get; set; }
        public string SegNr  { get; set; }
        public string ExtraInfo  { get; set; }

    }

    public class dtcCode
    {
        public dtcCode()
        {
            codeInt = 0;
            codeAddrInt = 0;
            CodeAddr = "";
            statusAddrInt = 0;
            StatusAddr = "";
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
        public string CodeAddr { get; set; }
        public uint statusAddrInt;
        public string StatusAddr { get; set; }
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

    public class TableData
    {
        public TableData()
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
        public string SegNr {get;set; }
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


public const short CSMethod_None = 0;
    public const short CSMethod_crc16 = 1;
    public const short CSMethod_crc32 = 2;
    public const short CSMethod_Bytesum = 3;
    public const short CSMethod_Wordsum = 4;
    public const short CSMethod_Dwordsum = 5;

    public static List<SegmentConfig> Segments = new List<SegmentConfig>();
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
    public static List<TableData> tableDatas;
    public static List<referenceCvn> referenceCvnList;
    public static List<FileType> fileTypeList;
    public static List<dtcCode> dtcCodes;
    public static List<OBD2Code> OBD2Codes;
    public static List<DtcSearchConfig> dtcSearchConfigs;
    public static bool dtcCombined = false;

    public static string XMLFile;
    public static string tableSearchFile;

    public static FrmPatcher frmpatcher;

    public static string[] dtcStatusCombined = { "MIL and reporting off", "Type A/no MIL", "Type B/no MIL", "Type C/no MIL", "Not reported/MIL", "Type A/MIL", "Type B/MIL", "Type C/MIL" };
    public static string[] dtcStatus = { "1 Trip/immediately", "2 Trips", "Store only", "Disabled" };

    public const ushort TypeText = 0;
    public const ushort TypeHex = 1;
    public const ushort TypeInt = 2;
    public const ushort TypeFilename = 3;

    public static T DeepClone<T>(T obj)
    {
        T objResult;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Position = 0;
            objResult = (T)bf.Deserialize(ms);
        }
        return objResult;
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

    public static string SelectFile(string Title = "Select file", string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*")
    {
        OpenFileDialog fdlg = new OpenFileDialog();
        if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
        if (Filter.Contains("PATCH") || Filter.Contains("TXT"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        else if (Filter.Contains("BIN"))
        {
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            Filter = "BIN files (*.bin)|*.bin";
            for (int f=0;f<fileTypeList.Count;f++)
            {
                string newFilter = "|" + fileTypeList[f].Description + "|" + "*." + fileTypeList[f].Extension;
                Filter += newFilter;
            }
            Filter += "|All files (*.*)|*.*";
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
        saveFileDialog.FileName = defaultFileName;
        if (Filter.Contains("PATCH"))
            saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
            saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
        else if (Filter.Contains("BIN"))
            saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;

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

    public static string SelectFolder(string Title)
    {
        string folderPath = "";
        OpenFileDialog folderBrowser = new OpenFileDialog();
        // Set validate names and check file exists to false otherwise windows will
        // not let you select "Folder Selection."
        folderBrowser.ValidateNames = false;
        folderBrowser.CheckFileExists = false;
        folderBrowser.CheckPathExists = true;
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
    public static string CheckStockCVN(string PN, string Ver, string SegNr, string cvn, bool AddToList)
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
        string[] searchParts = searchString.Split(' ');

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
                    byte searchval;
                    HexToByte(searchParts[part], out searchval);
                    if (PCM.buf[addr + part] != searchval)
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
        return uint.MaxValue;
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

    public static uint BEToUint32(byte[] buf, uint offset)
    {
        //Shift first byte 24 bits left, second 16bits left...
        return (uint)((buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
    }

    public static UInt16 BEToUint16(byte[] buf, uint offset)
    {
        return (UInt16)((buf[offset] << 8) | buf[offset + 1]);
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

    
}