using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

using UniversalPatcher;

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
        public string SegmentNr { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string cvn { get; set; }
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

    public struct BinFile
    {
        public List<Block> SegmentBlocks;
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
            Size = "";
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
        public string Size  { get; set; }
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
    public struct SegmentConfig
    {
        public string Name;
        public string Version;
        public string Addresses;    //Segment addresses, can be multiple parts
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
    public static List<SegmentInfo> ListSegment;

    public static string XMLFile;
    public static string LogText = "";

    public static frmLog FormLog;
    public static frmCVN FormCVN;
    public static frmFinfo FormFinfo;
    public static frmPatcheditor FormPatcheditor;
    public static FrmMain FormMain;
    public struct AddressData
    {
        public string Name;
        public uint Address;
        public ushort Bytes;
        public ushort Type;
    }
    private struct DetectGroup
    {
        public string Logic;
        public uint Hits;
        public uint Miss;
    }

    public const ushort TypeText = 0;
    public const ushort TypeHex = 1;
    public const ushort TypeInt = 2;

    public static byte[] ReadBin(string FileName, uint FileOffset, uint Length)
    {

        byte[] buf = new byte[Length];

        long offset = 0;
        long remaining = Length;

        using (BinaryReader freader = new BinaryReader(File.Open(FileName, FileMode.Open)))
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
        Debug.WriteLine("Calculating hecksum, method: " + Method);
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
        fdlg.Title = Title;
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;
        if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
        if (Filter.Contains("PATCH") || Filter.Contains("TXT"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        else if (Filter.Contains("BIN"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;

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
    public static string SelectSaveFile(string Filter = "BIN files (*.bin)|*.bin")
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        //saveFileDialog.Filter = "BIN files (*.bin)|*.bin";
        saveFileDialog.Filter = Filter;
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.Title = "Save to file";
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
        // Always default to Folder Selection.
        folderBrowser.Title = Title;
        folderBrowser.FileName = "Folder Selection";
        if (folderBrowser.ShowDialog() == DialogResult.OK)
        {
            folderPath = Path.GetDirectoryName(folderBrowser.FileName);            
        }
        return folderPath;
    }
    public static bool CheckStockCVN(string PN, string Ver, string SegNr, string cvn, bool AddToList)
    {
        for (int c = 0; c < StockCVN.Count; c++)
        {
                //if (StockCVN[c].XmlFile == Path.GetFileName(XMLFile) && StockCVN[c].PN == PN && StockCVN[c].Ver == Ver && StockCVN[c].SegmentNr == SegNr && StockCVN[c].cvn == cvn)
                if (StockCVN[c].PN == PN && StockCVN[c].Ver == Ver && StockCVN[c].SegmentNr == SegNr && StockCVN[c].cvn == cvn)
                {
                    return true;
                }
        }
        if (AddToList)
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
                ListCVN.Add(C1);                
            }
        }
        return false;
    }
    public static void ShowFileInfo(PcmFile PCM, bool InfoOnly)
    {
        try
        {
            for (int i = 0; i < Segments.Count; i++)
            {
                SegmentConfig S = Segments[i];
                Logger(PCM.segmentinfos[i].Name.PadRight(11), false);
                if (PCM.segmentinfos[i].PN.Length > 1)
                {
                    if (PCM.segmentinfos[i].Stock == "True")
                        LoggerBold(" PN: " + PCM.segmentinfos[i].PN.PadRight(9), false);
                    else
                        Logger(" PN: " + PCM.segmentinfos[i].PN.PadRight(9), false);
                }
                if (PCM.segmentinfos[i].Ver.Length > 1)
                    Logger(", Ver: " + PCM.segmentinfos[i].Ver, false);

                if (PCM.segmentinfos[i].SegNr.Length > 0)
                    Logger(", Nr: " + PCM.segmentinfos[i].SegNr.PadRight(3), false);
                    Logger("[" + PCM.segmentinfos[i].Address + "]", false);
                    Logger(", Size: " + PCM.segmentinfos[i].Size.ToString(), false);
                if (PCM.segmentinfos[i].ExtraInfo != null && PCM.segmentinfos[i].ExtraInfo.Length > 0)
                    Logger(Environment.NewLine + PCM.segmentinfos[i].ExtraInfo, false);

                    Logger("");
            }
            Logger("Checksums:");
            for (int i = 0; i < Segments.Count; i++)
            {
                SegmentConfig S = Segments[i];
                if (S.CS1Method != CSMethod_None)
                {
                    if (PCM.binfile[i].CS1Address.Bytes == 0)
                    {
                        Logger(" Checksum1: " + PCM.segmentinfos[i].CS1Calc, false);
                    }
                    else
                    {
                        if (PCM.segmentinfos[i].CS1 == PCM.segmentinfos[i].CS1Calc)
                        {
                            Logger(" Checksum 1: " + PCM.segmentinfos[i].CS1 + " [OK]", false);
                        }
                        else
                        {
                            Logger(" Checksum 1: " + PCM.segmentinfos[i].CS1 + ", Calculated: " + PCM.segmentinfos[i].CS1Calc + " [Fail]", false);
                        }
                    }
                }

                if (S.CS2Method != CSMethod_None)
                {
                    if (PCM.binfile[i].CS2Address.Bytes == 0)
                    {
                        Logger(" Checksum2: " + PCM.segmentinfos[i].CS2Calc, false);
                    }
                    else
                    {
                        if (PCM.segmentinfos[i].CS2 == PCM.segmentinfos[i].CS2Calc)
                        {
                            Logger(" Checksum 2: " + PCM.segmentinfos[i].CS2 + " [OK]", false);
                        }
                        else
                        {
                            Logger(" Checksum 2:" + PCM.segmentinfos[i].CS2 + ", Calculated: " + PCM.segmentinfos[i].CS2Calc + " [Fail]", false);
                        }
                    }
                }
                if (PCM.segmentinfos[i].Stock == "True")
                    LoggerBold("[Stock]", true);
                Logger("");
            }
            if (FormCVN != null && FormCVN.Visible)
            {
                FormCVN.RefreshCVNlist();
            }
            if (FormFinfo != null && FormFinfo.Visible)
            {
                FormFinfo.RefreshFileInfo();
            }
        }
        catch (Exception ex)
        {
            Logger("Error: " + ex.Message);
        }

    }

    private static bool CheckRule(DetectRule DR, PcmFile PCM)
    {
        try
        {

            UInt64 Data = 0;
            uint Addr = 0;
            if (DR.address == "filesize")
            {
                Data = (UInt64)new FileInfo(PCM.FileName).Length;
            }
            else
            {
                string[] Parts = DR.address.Split(':');
                HexToUint(Parts[0].Replace("@", ""), out Addr);
                if (DR.address.StartsWith("@"))
                    Addr = BEToUint32(PCM.buf, Addr);
                if (Parts[0].EndsWith("@"))
                    Addr = (uint)PCM.buf.Length - Addr;
                if (Parts.Length == 1)
                    Data = BEToUint16(PCM.buf, Addr);
                else
                {
                    if (Parts[1] == "1")
                        Data = (uint)PCM.buf[Addr];
                    if (Parts[1] == "2")
                        Data = (uint)BEToUint16(PCM.buf, Addr);
                    if (Parts[1] == "4")
                        Data = BEToUint32(PCM.buf, Addr);
                    if (Parts[1] == "8")
                        Data = BEToUint64(PCM.buf, Addr);

                }
            }

            //Logger(DR.xml + ": " + DR.address + ": " + DR.data.ToString("X") + DR.compare + "(" + DR.grouplogic + ") " + " [" + Addr.ToString("X") + ": " + Data.ToString("X") + "]");

            if (DR.compare == "==")
            {
                if (Data == DR.data)
                    return true;
            }
            if (DR.compare == "<")
            {
                if (Data < DR.data)
                    return true;
            }
            if (DR.compare == ">")
            {
                if (Data > DR.data)
                    return true;
            }
            if (DR.compare == "!=")
            {
                if (Data != DR.data)
                    return true;
            }
            //Logger("Not match");
            return false;
        }
        catch (Exception ex)
        {
            //Something wrong, just skip this part and continue
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public static string Autodetect(PcmFile PCM)
    {
        string Result = "";

        List<string> XmlList = new List<string>();
        XmlList.Add(DetectRules[0].xml.ToLower());
        for (int s = 0; s < DetectRules.Count; s++)
        {
            //Create list of XML files we know:
            bool Found = false;
            for (int x = 0; x < XmlList.Count; x++)
            {
                if (XmlList[x] == DetectRules[s].xml.ToLower())
                    Found = true;
            }
            if (!Found)
                XmlList.Add(DetectRules[s].xml.ToLower());
        }
        for (int x = 0; x < XmlList.Count; x++)
        {
            uint MaxGroup = 0;

            //Check if compatible with THIS xml
            List<DetectRule> DRL = new List<DetectRule>();
            for (int s = 0; s < DetectRules.Count; s++)
            {
                if (XmlList[x] == DetectRules[s].xml.ToLower())
                {
                    DRL.Add(DetectRules[s]);
                    if (DetectRules[s].group > MaxGroup)
                        MaxGroup = DetectRules[s].group;
                }
            }
            //Now all rules for this XML are in DRL (DetectRuleList)
            DetectGroup[] DG = new DetectGroup[MaxGroup + 1];
            for (int d = 1; d <= MaxGroup; d++)
            {
                //Clear DG (needed?)
                DG[d].Hits = 0;
                DG[d].Miss = 0;
            }
            for (int d = 0; d < DRL.Count; d++)
            {
                //This list have only rules for one XML, lets go thru them
                DG[DRL[d].group].Logic = DRL[d].grouplogic;
                if (CheckRule(DRL[d], PCM))
                    //This check matches
                    DG[DRL[d].group].Hits++;
                else
                    DG[DRL[d].group].Miss++;
            }
            //Now we have array DG, where hits & misses are counted per group, for this XML
            bool Detection = true;
            for (int g = 1; g <= MaxGroup; g++)
            {
                //If all groups match, then this XML, match.
                if (DG[g].Logic == "And")
                {
                    //Logic = and => if any Miss, not detection
                    if (DG[g].Miss > 0)
                        Detection = false;
                }
                if (DG[g].Logic == "Or")
                {
                    if (DG[g].Hits == 0)
                        Detection = false;
                }
                if (DG[g].Logic == "Xor")
                {
                    if (DG[g].Hits != 1)
                        Detection = false;
                }
            }
            if (Detection)
            {
                //All groups have hit (if grouplogic = or, only one hit per group is a hit)
                if (Result != "")
                    Result += Environment.NewLine;
                Result += XmlList[x];
                Debug.WriteLine("Autodetect: " + XmlList[x]);
            }
        }
        return Result.ToLower();
    }

    public static bool FixCheckSums(ref PcmFile PCM)
    {
        bool NeedFix = false;
        try
        {
            Logger("Fixing Checksums:");
            for (int i = 0; i < Segments.Count; i++)
            {
                SegmentConfig S = Segments[i];
                Logger(S.Name);
                if (S.Eeprom)
                {
                    string Ret = GmEeprom.FixEepromKey(PCM.buf);
                    if (Ret.Contains("Fixed"))
                        NeedFix = true;
                    Logger(Ret);
                }
                else
                {
                    if (S.CS1Method != CSMethod_None)
                    {
                        uint CS1 = 0;
                        uint CS1Calc = CalculateChecksum(PCM.buf, PCM.binfile[i].CS1Address, PCM.binfile[i].CS1Blocks, PCM.binfile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, PCM.binfile[i].CS1Address.Bytes, S.CS1SwapBytes);
                        if (PCM.binfile[i].CS1Address.Bytes == 1)
                        {
                            CS1 = PCM.buf[PCM.binfile[i].CS1Address.Address];
                        }
                        else if (PCM.binfile[i].CS1Address.Bytes == 2)
                        {
                            CS1 = BEToUint16(PCM.buf, PCM.binfile[i].CS1Address.Address);
                        }
                        else if (PCM.binfile[i].CS1Address.Bytes == 4)
                        {
                            CS1 = BEToUint32(PCM.buf, PCM.binfile[i].CS1Address.Address);
                        }
                        if (CS1 == CS1Calc)
                            Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                        else
                        {
                            if (PCM.binfile[i].CS1Address.Bytes == 0)
                            {
                                Logger(" Checksum 1: " + CS1Calc.ToString("X4") + " [Not saved]");
                            }
                            else
                            {
                                if (PCM.binfile[i].CS1Address.Bytes == 1)
                                    PCM.buf[PCM.binfile[i].CS1Address.Address] = (byte)CS1Calc;
                                else if (PCM.binfile[i].CS1Address.Bytes == 2)
                                {
                                    PCM.buf[PCM.binfile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF00) >> 8);
                                    PCM.buf[PCM.binfile[i].CS1Address.Address + 1] = (byte)(CS1Calc & 0xFF);
                                }
                                else if (PCM.binfile[i].CS1Address.Bytes == 4)
                                {
                                    PCM.buf[PCM.binfile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF000000) >> 24);
                                    PCM.buf[PCM.binfile[i].CS1Address.Address + 1] = (byte)((CS1Calc & 0xFF0000) >> 16);
                                    PCM.buf[PCM.binfile[i].CS1Address.Address + 2] = (byte)((CS1Calc & 0xFF00) >> 8);
                                    PCM.buf[PCM.binfile[i].CS1Address.Address + 3] = (byte)(CS1Calc & 0xFF);

                                }
                                Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                NeedFix = true;
                            }
                        }
                    }

                    if (S.CS2Method != CSMethod_None)
                    {
                        uint CS2 = 0;
                        uint CS2Calc = CalculateChecksum(PCM.buf, PCM.binfile[i].CS2Address, PCM.binfile[i].CS2Blocks, PCM.binfile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, PCM.binfile[i].CS2Address.Bytes, S.CS2SwapBytes);
                        if (PCM.binfile[i].CS2Address.Bytes == 1)
                        {
                            CS2 = PCM.buf[PCM.binfile[i].CS2Address.Address];
                        }
                        else if (PCM.binfile[i].CS2Address.Bytes == 2)
                        {
                            CS2 = BEToUint16(PCM.buf, PCM.binfile[i].CS2Address.Address);
                        }
                        else if (PCM.binfile[i].CS2Address.Bytes == 4)
                        {
                            CS2 = BEToUint32(PCM.buf, PCM.binfile[i].CS2Address.Address);
                        }
                        if (CS2 == CS2Calc)
                            Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                        else
                        {
                            if (PCM.binfile[i].CS2Address.Bytes == 0)
                            {
                                Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                            }
                            else
                            {
                                if (PCM.binfile[i].CS2Address.Bytes == 1)
                                    PCM.buf[PCM.binfile[i].CS2Address.Address] = (byte)CS2Calc;
                                else if (PCM.binfile[i].CS2Address.Bytes == 2)
                                {
                                    PCM.buf[PCM.binfile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF00) >> 8);
                                    PCM.buf[PCM.binfile[i].CS2Address.Address + 1] = (byte)(CS2Calc & 0xFF);
                                }
                                else if (PCM.binfile[i].CS2Address.Bytes == 4)
                                {
                                    PCM.buf[PCM.binfile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF000000) >> 24);
                                    PCM.buf[PCM.binfile[i].CS2Address.Address + 1] = (byte)((CS2Calc & 0xFF0000) >> 16);
                                    PCM.buf[PCM.binfile[i].CS2Address.Address + 2] = (byte)((CS2Calc & 0xFF00) >> 8);
                                    PCM.buf[PCM.binfile[i].CS2Address.Address + 3] = (byte)(CS2Calc & 0xFF);

                                }
                                Logger(" Checksum 2: " + CS2.ToString("X") + " => " + CS2Calc.ToString("X4") + " [Fixed]");
                                NeedFix = true;
                            }
                        }
                    }

                }

            }
        }
        catch (Exception ex)
        {
            Logger("Error: " + ex.Message);
        }
        return NeedFix;
    }

    public static void GetFileInfo(string FileName, ref PcmFile PCM, bool InfoOnly, bool autodetect, bool Show = true)
    {
        try
        {
            if (autodetect)
            {
                string ConfFile = Autodetect(PCM);
                Logger("Autodetect: " + ConfFile);
                if (ConfFile == "" || ConfFile.Contains(Environment.NewLine))
                {
                    XMLFile = "";
                    Segments.Clear();
                }
                else
                {
                    ConfFile = Path.Combine(Application.StartupPath, "XML", ConfFile);
                    if (File.Exists(ConfFile))
                    {
                        frmSegmenList frmSL = new frmSegmenList();
                        frmSL.LoadFile(ConfFile);
                    }
                    else
                    {
                        Logger("XML File not found");
                        XMLFile = "";
                        Segments.Clear();
                        Logger(Environment.NewLine + Path.GetFileName(FileName));
                        return;
                    }
                }
            }
            if (Segments == null || Segments.Count == 0)
            {
                Logger(Environment.NewLine + Path.GetFileName(FileName));
                return;
            }
            Logger(Environment.NewLine + Path.GetFileName(FileName) + Environment.NewLine);
            PCM.GetSegmentAddresses();
            if (Segments.Count > 0)
                Logger("Segments:");
            PCM.GetInfo();
            if (FormCVN != null && FormCVN.Visible)
                FormCVN.RefreshCVNlist();
            if (PCM.OS == null || PCM.OS == "")
                LoggerBold("Warning: No OS segment defined, limiting functions");
            if (Show)
                ShowFileInfo(PCM, InfoOnly);
        }
        catch (Exception ex)
        {
            Logger(ex.Message);
        }
    }

    public static void FixFileChecksum(string FileName)
    {
        try
        {
            PcmFile PCM = new PcmFile(FileName);
            GetFileInfo(FileName, ref PCM, true, true);
            if (FixCheckSums(ref PCM))
            {
                Logger("Saving file: " + FileName);
                WriteBinToFile(FileName, PCM.buf);
                Logger("[OK]");
            }
        }
        catch (Exception ex)
        {
            Logger(ex.Message);
        }

    }
    public static bool ApplyXMLPatch(ref PcmFile PCM)
    {
        try
        {
            bool isCompatible = false;
            string BinPN = "";
            string PrevSegment = "";
            uint ByteCount = 0;
            string[] Parts;
            if (PatchList[0].XmlFile != null)
            {
                Parts = PatchList[0].XmlFile.Split(',');
                foreach (string Part in Parts)
                {
                    if (Part == Path.GetFileName(XMLFile))
                        isCompatible = true;
                }
                if (!isCompatible)
                {
                    Logger("Incompatible patch");
                    return false;
                }
            }
            Logger("Applying patch:");
            foreach (XmlPatch xpatch in PatchList)
            {
                isCompatible = false;
                uint Addr = 0;
                string[] OSlist = xpatch.CompatibleOS.Split(',');
                foreach (string OS in OSlist)
                {
                    Parts = OS.Split(':');
                    if (Parts[0] == "ALL")
                    {
                        isCompatible = true;
                        if (!HexToUint(Parts[1], out Addr))
                            throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                        Debug.WriteLine("ALL, Addr: " + Parts[1]);
                    }
                    else
                    {
                        if (BinPN == "")
                        {
                            //Search OS once
                            for (int s = 0; s < Segments.Count; s++)
                            {
                                string PN = PCM.ReadInfo(PCM.binfile[s].PNaddr);
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
                            if (!HexToUint(Parts[1], out Addr))
                                throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                            Debug.WriteLine("OS: " + BinPN + ", Addr: " + Parts[1]);
                        }
                    }
                }
                if (isCompatible)
                {
                    if (xpatch.Description != null && xpatch.Description != "")
                        Logger(xpatch.Description);
                    if (xpatch.Segment != null && xpatch.Segment.Length > 0 && PrevSegment != xpatch.Segment)
                    {
                        PrevSegment = xpatch.Segment;
                        Logger("Segment: " + xpatch.Segment);
                    }
                    bool PatchRule = true; //If there is no rule, apply patch
                    if (xpatch.Rule != null && (xpatch.Rule.Contains(":") || xpatch.Rule.Contains("[")))
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
                                if ((PCM.buf[RuleAddr] & RuleMask) != RuleValue)
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
                                if ((PCM.buf[RuleAddr] & RuleMask) == RuleValue)
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
                        Debug.WriteLine(Addr.ToString("X") + ":" + xpatch.Data);
                        Parts = xpatch.Data.Split(' ');
                        foreach (string Part in Parts)
                        {
                            //Actually add patch data:
                            if (Part.Contains("[") || Part.Contains(":"))
                            {
                                //Set bits / use Mask
                                byte Origdata = PCM.buf[Addr];
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
                                PCM.buf[Addr] = Newdata;
                            }
                            else
                            {
                                //Set byte
                                PCM.buf[Addr] = Byte.Parse(Part, System.Globalization.NumberStyles.HexNumber);
                            }
                            Addr++;
                            ByteCount++;
                        }
                    }
                }
                else
                {
                    Logger("Patch is not compatible");
                    return false;
                }
            }
            Logger("Applied: " + ByteCount.ToString() + " Bytes");
        }
        catch (Exception ex)
        {
            Logger("Error: " + ex.Message);
            return false;
        }
        return true;
    }
    public static void ExtractTable(uint Start, uint End, string[] OSlist, string MaskText, PcmFile PCM, string Description)
    {
        try
        {
            XmlPatch xpatch = new XmlPatch();
            xpatch.CompatibleOS = OSlist[0] + ":" + Start.ToString("X");
            for (int i = 1; i < OSlist.Length; i++)
                xpatch.CompatibleOS += "," + OSlist[i] + ":" + Start.ToString("X");
            xpatch.XmlFile = Path.GetFileName(XMLFile);
            xpatch.Description = Description;
            Logger("Extracting " + Start.ToString("X") + " - " + End.ToString("X"));
            for (uint i = Start; i <= End; i++)
            {
                if (i > Start)
                    xpatch.Data += " ";
                if (MaskText.ToLower() == "ff" || MaskText == "")
                {
                    xpatch.Data += PCM.buf[i].ToString("X2");
                }
                else
                {
                    byte Mask = byte.Parse(MaskText, System.Globalization.NumberStyles.HexNumber);
                    xpatch.Data += (PCM.buf[i] & Mask).ToString("X2") + "[" + MaskText + "]";
                }
            }
            if (PatchList == null)
                PatchList = new List<XmlPatch>();
            Logger("[OK]");
            PatchList.Add(xpatch);
            if (FormPatcheditor == null || !FormPatcheditor.Visible)
            { 
                FormPatcheditor = new frmPatcheditor();
                FormPatcheditor.MdiParent = FormMain;
                FormPatcheditor.Show();
            }
            FormPatcheditor.RefreshDatagrid();

        }
        catch (Exception ex)
        {
            Logger(ex.Message);
        }

    }

    public static void Logger(string LogText, bool newline = true)
    {
        if (FormLog == null)
            return;
        if (newline)
            LogText += Environment.NewLine;
        FormLog.ShowLog(LogText);
        Application.DoEvents();
    }
    public static void LoggerBold(string LogText, bool newline = true)
    {
        if (FormLog == null)
            return;
        if (newline)
            LogText += Environment.NewLine;
        FormLog.ShowLogBold(LogText);
        Application.DoEvents();
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

    public static bool HexToUshort(string Hex, out ushort x)
    {
        x = 0;
        if (!UInt16.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
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