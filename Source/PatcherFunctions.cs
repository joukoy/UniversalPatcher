using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class upatcher
{
    public struct DetectRule
    {
        public string xml;
        public ushort group;
        public string grouplogic;   //and, or, xor
        public string address;
        public UInt64 data;
        public string compare;        //==, <, >, !=      
    }

    public struct Patch
    {
        public string Name;
        public string FileName;
        public string Description;
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

    public struct SegmentConfig
    {
        public string Name;     
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
        public bool Eeprom;         //Special case: P01 or P59 Eeprom segment
        public string PNAddr;
        public string VerAddr;
        public string SegNrAddr;
        public string ExtraInfo;
        public string Comment;
        public string CheckWords;
        public string SearchAddresses;  //Possible start addresses for Eepom segment
        public string Searchfor;  //search if this found/not found in segment
        public string Searchfrom; //Search above in these addresses
        public bool SearchNot;     //Search where NOT found
    }

    public const short CSMethod_None = 0;
    public const short CSMethod_crc16 = 1;
    public const short CSMethod_crc32 = 2;
    public const short CSMethod_Bytesum = 3;
    public const short CSMethod_Wordsum = 4;
    public const short CSMethod_Dwordsum = 5;

    public static List<SegmentConfig> Segments = new List<SegmentConfig>();
    public static List<DetectRule> DetectRules;

    public static string XMLFile;

    public struct AddressData
    {
        public string Name;
        public uint Address;
        public ushort Bytes;
        public ushort Type;
    }

    public const ushort TypeText = 0;
    public const ushort TypeHex = 1;
    public const ushort TypeInt = 2;


    public static CheckWord GetCheckwordAddress(BinFile binfile, string AddrLine)
    {
        string[] LineParts;
        if (AddrLine.Contains("+"))
            LineParts = AddrLine.Split('+');
        else if (AddrLine.Contains("-"))
            LineParts = AddrLine.Split('-');
        else
        {
            throw new Exception("No + or - after Checkword! (" + AddrLine +")");
        }
        foreach (CheckWord CW in binfile.Checkwords)
        {
            if (LineParts[0] == (CW.Key))
            {
                Debug.WriteLine("Checkword: " + CW.Key + " => " + CW.Address.ToString("X"));
                return CW;
            }
        }
        CheckWord checkw = new CheckWord();
        checkw.Address = 0;
        checkw.Key = "";
        return checkw;
    }

    public static AddressData GMV6(string Line, uint SegmentAddress, byte[] buf, ref BinFile binfileSegment)
    {
        uint BufSize = (uint)buf.Length;
        uint GMOS = 0;
        binfileSegment.PNaddr.Address = 0;
        AddressData AD = new AddressData();

        for (int i = 2; i < 20; i++)
        {
            if (BEToUint16(buf, (uint)(BufSize - i)) == 0xA55A) //Read OS version from end of file, before bytes A5 5A
            {
                binfileSegment.PNaddr.Address = (uint)(BufSize - (i + 4));
                Debug.WriteLine("V6: Found PN address from: " + binfileSegment.PNaddr.Address.ToString("X"));
            }
        }
        if (binfileSegment.PNaddr.Address == 0)
            throw new Exception("OS id missing");
        GMOS = BEToUint32(buf, binfileSegment.PNaddr.Address);
        binfileSegment.PNaddr.Bytes = 4;
        binfileSegment.PNaddr.Type = TypeInt;
        Block B = new Block();
        B.Start = binfileSegment.PNaddr.Address;
        B.End = binfileSegment.PNaddr.Address + 3;
        if (binfileSegment.ExcludeBlocks == null)
            binfileSegment.ExcludeBlocks = new List<Block>();
        binfileSegment.ExcludeBlocks.Add(B);
        string FileName = Path.Combine(Application.StartupPath, "XML", Line);
        StreamReader sr = new StreamReader(FileName);
        string OsLine;
        while ((OsLine = sr.ReadLine()) != null)
        {
            //Custom handling: read OS:Segmentaddress pairs from file
            string[] OsLineparts = OsLine.Split(':');
            if (OsLineparts.Length == 2)
            {
                if (OsLineparts[0] == GMOS.ToString())
                {
                    if (HexToUint(OsLineparts[1], out AD.Address))
                    {
                        Debug.WriteLine("Address: " + AD.Address.ToString("X") + ", Bytes: 4, Type: HEX");
                        sr.Close();
                        AD.Bytes = 4;
                        AD.Type = TypeHex;
                        return AD;
                    }
                }
            }
        }
        sr.Close();
        throw new Exception("Unsupported OS:  " + GMOS.ToString());

    }

    public static AddressData ParseAddress (string Line, uint SegmentAddress, byte[] buf, ref BinFile binfileSegment)
    {

        Debug.WriteLine("Addressline: " + Line);
        if (Line.StartsWith("GM-V6"))
        {
            //Custom handling: read OS:Segmentaddress pairs from file
            Debug.WriteLine("V6");
            return GMV6(Line, SegmentAddress, buf, ref binfileSegment);
        }

        AddressData AD = new AddressData();

        if (Line.Length == 0) 
            return AD;
        string[] Lineparts = Line.Split(':');
        CheckWord CWAddr;
        CWAddr.Address = 0;
        CWAddr.Key = "";
        bool Negative = false;
        if (Lineparts[0].Contains("CW"))
        { 
            CWAddr = GetCheckwordAddress(binfileSegment, Lineparts[0]);
            if (CWAddr.Key != "")
                Lineparts[0] = Lineparts[0].Replace(CWAddr.Key, "");
        }
        if (!HexToUint(Lineparts[0].Replace("#", ""), out AD.Address))
            throw new Exception("Can't convert from HEX: " + Lineparts[0].Replace("#", "") + " (" + Line +")");
            //return AD;

        if (Line.StartsWith("#"))
        {
            AD.Address += SegmentAddress;
        }
        if (Negative)
            AD.Address = CWAddr.Address - AD.Address;
        else
            AD.Address += CWAddr.Address;


        if (Lineparts.Length > 1)
            UInt16.TryParse(Lineparts[1], out AD.Bytes);
        AD.Type = TypeInt;
        if (Lineparts.Length > 2)
        {
            if (Lineparts[2].ToLower() == "hex")
                AD.Type = TypeHex;
            else if (Lineparts[2].ToLower() == "text")
                AD.Type = TypeText;
        }
        Debug.WriteLine("Name: " + AD.Name + ", Address: " + AD.Address.ToString("X") + ", Bytes: " + AD.Bytes.ToString() + ", Type: " + AD.Type.ToString());
        return AD;
    }


    public static bool ParseSegmentAddresses(string Line, SegmentConfig S, byte[] buf, out List<Block> Blocks)
    {
        Debug.WriteLine("Segment address line: " + Line);
        Blocks = new List<Block>();

        if (Line == null || Line == "")
        {
            //It is ok to have empty address (for CS, not for segment)
            Block B = new Block();
            B.End = 0;
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
            bool isWord = false;
            ushort Multiple = 1;

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


            if (StartEnd[0].Contains("*"))
            {
                string[] SM = StartEnd[0].Split('*');
                StartEnd[0] = SM[0];
                UInt16.TryParse(SM[1],out Multiple);
            }

            if (StartEnd[0].Contains(":2"))
            {
                string[] SW = StartEnd[0].Split(':');
                StartEnd[0] = SW[0];
                isWord = true;
            }

            if (StartEnd.Length > 1 && StartEnd[1].Contains(":2"))
            {
                string[] EW = StartEnd[1].Split(':');
                StartEnd[1] = EW[0];
                isWord = true;
            }


            if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
            {
                throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")"); 
            }
            if (StartEnd[0].StartsWith("@"))
            {
                uint tmpStart = B.Start;
                for (int m=1; m<=Multiple; m++)
                {                
                    //Read address from bin at this address
                    
                    if (isWord)
                    { 
                        B.Start = BEToUint16(buf, tmpStart);
                        B.End = BEToUint16(buf, tmpStart + 2);
                        tmpStart += 4;
                    }
                    else
                    {
                        B.Start = BEToUint32(buf, tmpStart);
                        B.End = BEToUint32(buf, tmpStart + 4);
                        tmpStart += 8;
                    }
                    if (Multiple>1)
                    {
                        // Have multiple start-end pairs
                        B.Start += (uint)Offset;
                        B.End += (uint)Offset;
                        Blocks.Add(B);
                    }
                }
            }
            else
            {
                if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                    throw new Exception("Can't decode from HEX: " + StartEnd[1].Replace("@", "") + " (" + Line + ")");
                if (B.End >= buf.Length)    //Make 1MB config work with 512kB bin
                    B.End = (uint)buf.Length - 1;
            }
            if (Multiple < 2)
            { 
                if (StartEnd.Length>1 && StartEnd[1].StartsWith("@"))
                {
                    //Read End address from bin at this address
                    B.End = BEToUint32(buf, B.End);
                }
                if (StartEnd.Length > 1 && StartEnd[1].EndsWith("@"))
                {
                    //Address is relative to end of bin
                    uint end;
                    if (HexToUint(StartEnd[1].Replace("@",""), out end) )
                        B.End = (uint)buf.Length - end - 1;
                }
                B.Start += (uint)Offset;
                B.End += (uint)Offset;
                Blocks.Add(B);
            }
            i++;
        }
        foreach (Block B in Blocks)
            Debug.WriteLine("Address block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
        return true;
    }

    public static List<AddressData> ParseExtraInfo(byte[] buf, string Line, uint SegmentAddress, BinFile binfile)
    {
        Debug.WriteLine("Extrainfo: " + Line);
        List<AddressData> LEX = new List<AddressData>();
        if (Line == null || Line.Length == 0 || !Line.Contains(":"))
            return LEX;

        string[] LineParts = Line.Split(',');
        foreach (string LinePart in LineParts)
        {
            AddressData E = new AddressData();
            string[] AddrParts = LinePart.Split(':');
            if (AddrParts.Length < 3)
                return LEX;

            E.Name = AddrParts[0];

            CheckWord CWAddr;
            CWAddr.Key = "";
            CWAddr.Address = 0;
            bool Negative = false;
            if (AddrParts[1].Contains("CW"))
            {
                CWAddr = GetCheckwordAddress(binfile, AddrParts[1]);
                if (CWAddr.Key != "")
                    AddrParts[1] = AddrParts[1].Replace(CWAddr.Key, "");
            }
            if (AddrParts[1].Contains("-"))
                Negative = true;
            AddrParts[1] = AddrParts[1].Replace("-", "");
            AddrParts[1] = AddrParts[1].Replace("+", "");
            if (!HexToUint(AddrParts[1].Replace("#", ""), out E.Address))
                return LEX;

            if (Negative)
                E.Address = CWAddr.Address - E.Address;
            else
                E.Address += CWAddr.Address;

            if (AddrParts[1].StartsWith("#"))
            {
                E.Address += SegmentAddress;
            }

            if (AddrParts.Length > 2)
                UInt16.TryParse(AddrParts[2], out E.Bytes);
            E.Type = TypeInt;
            if (AddrParts.Length > 3)
            {
                if (AddrParts[3].ToLower() == "hex")
                    E.Type = TypeHex;
                else if (AddrParts[3].ToLower() == "text")
                    E.Type = TypeText;
            }
            LEX.Add(E);
        }
        for (int l = 0; l < LEX.Count; l++)
            Debug.WriteLine("Extrainfo name: " + LEX[l].Name + ", Address: " + LEX[l].Address.ToString("X") + ", Bytes: " + LEX[l].Bytes + ", Type: " + LEX[l].Type);
        return LEX;
    }

    public static  bool FindSegment(byte[] buf, SegmentConfig S, ref BinFile binfile)
    {
        Debug.WriteLine("Searching segment");
        if (S.Searchfor.Length == 0)
            return false;
        ushort Bytes = (ushort)(S.Searchfor.Length / 2);
        if (Bytes == 1)
            Bytes = 2;
        if (Bytes == 3)
            Bytes = 4;
        if (Bytes > 4 && Bytes < 8)
            Bytes = 8;
        UInt64 SearchFor;
        if (!HexToUint64(S.Searchfor, out SearchFor))
            return false;
        uint SearchFrom;
        if (!HexToUint(S.Searchfrom, out SearchFrom))
            return false;

        List<Block> Blocks;
        binfile.SegmentBlocks = new List<Block>();
        if (!ParseSegmentAddresses(S.SearchAddresses, S, buf, out Blocks))
            return false;
        foreach (Block B in Blocks)
        {

            uint Addr = B.Start + SearchFrom;
            if (!S.SearchNot)
            {

                if (Bytes == 8)
                    if (BEToUint64(buf, Addr) == SearchFor)
                    { 
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
                if (Bytes == 4)
                    if (BEToUint32(buf, Addr) == SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
                if (Bytes == 2)
                    if (BEToUint16(buf, Addr) == SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
                if (Bytes == 1)
                    if (buf[Addr] == SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
            }
            else
            {
                if (Bytes == 8)
                    if (BEToUint64(buf, Addr) != SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
                if (Bytes == 4)
                    if (BEToUint32(buf, Addr) != SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
                if (Bytes == 2)
                    if (BEToUint16(buf, Addr) != SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
                if (Bytes == 1)
                    if (buf[Addr] != SearchFor)
                    {
                        binfile.SegmentBlocks.Add(B);
                        Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                        return true;
                    }
            }
        }
        Debug.WriteLine("Not found");
        return false;
    }

    public static bool FindCheckwordData(byte[] buf, SegmentConfig S, ref BinFile binfile)
    {
        Debug.WriteLine("Checkwords: " + S.CheckWords);
        if (S.CheckWords == null)
            return false;
        binfile.Checkwords = new List<CheckWord>();
        string[] CWlist = S.CheckWords.Split(',');
        foreach(string Row in CWlist)
        {
            string[] Parts = Row.Split(':');
            if (Parts.Length == 4)
            {
                Debug.WriteLine(Parts[3] + ": " + Parts[0] + " in " + Parts[1] + " ?");
                CheckWord checkw = new CheckWord();
                UInt64 CW;
                uint Location;
                if (!HexToUint64(Parts[0], out CW))
                    return false;
                if (!HexToUint(Parts[1], out Location))
                    return false;
                if (!HexToUint(Parts[2], out checkw.Address))
                    return false;
                checkw.Key = Parts[3];
                checkw.Address += binfile.SegmentBlocks[0].Start;

                Location += binfile.SegmentBlocks[0].Start;
                ushort Bytes = (ushort)(Parts[0].Length / 2);
                if (Bytes == 1)
                    Bytes = 2;
                if (Bytes == 3)
                    Bytes = 4;
                if (Bytes > 4 && Bytes < 8)
                    Bytes = 8;

                if (Bytes == 1)
                    if (buf[Location] == CW)
                    {
                        Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X"));
                        binfile.Checkwords.Add(checkw);
                    }
                if (Bytes == 2)
                    if (BEToUint16(buf,Location) == CW)
                    {
                        Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X2"));
                        binfile.Checkwords.Add(checkw);
                    }
                if (Bytes == 4)
                    if (BEToUint32(buf, Location) == CW)
                    {
                        Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X4"));
                        binfile.Checkwords.Add(checkw);
                    }
                if (Bytes == 8)
                    if (BEToUint64(buf, Location) == CW)
                    {
                        Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X8"));
                        binfile.Checkwords.Add(checkw);
                    }
            }
        }
        return true;
    }

    public static void GetSegmentAddresses(byte[] buf, out BinFile[] binfile)
    {
        binfile = new BinFile[Segments.Count];
        for (int i=0; i< Segments.Count;i++)
        {
            SegmentConfig S = Segments[i];
            List<Block> B = new List<Block>();
            binfile[i].ExcludeBlocks = B;
            if (S.SearchAddresses != null)
            {                
                if (!FindSegment(buf, S, ref binfile[i]))
                    return;
            }
            else { 
                if (!ParseSegmentAddresses(S.Addresses, S, buf, out B))
                    return;
                binfile[i].SegmentBlocks = B;
            }
            if (!ParseSegmentAddresses(S.CS1Blocks, S, buf, out B))
                return;
            binfile[i].CS1Blocks = B;
            if (!ParseSegmentAddresses(S.CS2Blocks, S, buf, out B))
                return;
            binfile[i].CS2Blocks = B;
            binfile[i].CS1Address = ParseAddress(S.CS1Address, binfile[i].SegmentBlocks[0].Start,buf, ref binfile[i]);
            binfile[i].CS2Address = ParseAddress(S.CS2Address, binfile[i].SegmentBlocks[0].Start, buf, ref binfile[i]);
            if (S.CheckWords != null && S.CheckWords != "")
                FindCheckwordData(buf, S, ref binfile[i]);
                    
            if (binfile[i].PNaddr.Bytes == 0)  //if not searched
                binfile[i].PNaddr = ParseAddress(S.PNAddr, binfile[i].SegmentBlocks[0].Start, buf, ref binfile[i]);
            binfile[i].VerAddr = ParseAddress(S.VerAddr, binfile[i].SegmentBlocks[0].Start, buf, ref binfile[i]);
            binfile[i].SegNrAddr = ParseAddress(S.SegNrAddr, binfile[i].SegmentBlocks[0].Start, buf, ref binfile[i]);
            binfile[i].ExtraInfo = ParseExtraInfo(buf, S.ExtraInfo, binfile[i].SegmentBlocks[0].Start, binfile[i]);
        }
    }

    public static string ReadInfo(byte[] buf, AddressData AD)
    {
        Debug.WriteLine("Reading address: " + AD.Address.ToString("X") + ", bytes: " + AD.Bytes.ToString() + ", Type: " + AD.Type); 
        string Result = "";
        if (AD.Bytes == 1)
        {
            if (AD.Type == TypeHex)
                Result = buf[AD.Address].ToString("X2");
            else if (AD.Type == TypeText)
                Result = ReadTextBlock(buf, (int)AD.Address, AD.Bytes);
            else
                Result = buf[AD.Address].ToString();
        }
        else if (AD.Bytes == 2)
        {
            if (AD.Type == TypeHex)
                Result = BEToUint16(buf, AD.Address).ToString("X4");
            else if (AD.Type == TypeText)
                Result = ReadTextBlock(buf, (int)AD.Address, AD.Bytes);
            else
                Result = BEToUint16(buf, AD.Address).ToString();
        }
        else if (AD.Bytes == 8)
        {
            if (AD.Type == TypeHex)
                Result = BEToUint64(buf, AD.Address).ToString("X4");
            else if (AD.Type == TypeText)
                Result = ReadTextBlock(buf, (int)AD.Address, AD.Bytes);
            else
                Result = BEToUint64(buf, AD.Address).ToString();
        }
        else //Default is 4 bytes
        {
            if (AD.Type == TypeHex)
                Result = BEToUint32(buf, AD.Address).ToString("X4");
            else if (AD.Type == TypeText)
                Result = ReadTextBlock(buf, (int)AD.Address, AD.Bytes);
            else
                Result = BEToUint32(buf, AD.Address).ToString();
        }
        Debug.WriteLine("Result: " + Result);
        return Result;
    }

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


    public static void WriteSegmentToFile(string FileName, uint StartAddr, uint Length, byte[] Buf)
    {

        using (FileStream stream = new FileStream(FileName, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Buf, (int)StartAddr, (int)Length);
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
        if (Bytes ==  2 || Bytes == 0) //Bytes = 0 if not saving
        { 
            sum = (sum & 0xFFFF);
        }
        if (SwapB)
        {
            if (Bytes == 2)
                sum = (ushort)SwapBytes((ushort)sum);
            else
                sum = SwapBytes(sum);

        }
        Debug.WriteLine("Result: " + sum.ToString("X"));
        return sum;
    }

    public static string SelectFile(string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*")
    {

        OpenFileDialog fdlg = new OpenFileDialog();
        fdlg.Title = "Select file";
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;
        if (Filter.Contains("PATCH"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        if (Filter.Contains("XML"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
        else if (Filter.Contains("BIN"))
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;

        if (fdlg.ShowDialog() == DialogResult.OK)
        {
            if (Filter.Contains("XML"))
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
        if (Filter.Contains("XML"))
            saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
        else if (Filter.Contains("BIN"))
            saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;

        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            if (Filter.Contains("XML"))
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

    public class Crc16
    {
        const ushort polynomial = 0xA001;
        ushort[] table = new ushort[256];

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }

    }

    public class Crc32
        {
            uint[] table;

            public uint ComputeChecksum(byte[] bytes)
            {
                uint crc = 0xffffffff;
                for (int i = 0; i < bytes.Length; ++i)
                {
                    byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                    crc = (uint)((crc >> 8) ^ table[index]);
                }
                return ~crc;
            }

            public byte[] ComputeChecksumBytes(byte[] bytes)
            {
                return BitConverter.GetBytes(ComputeChecksum(bytes));
            }

            public Crc32()
            {
                uint poly = 0xedb88320;
                table = new uint[256];
                uint temp = 0;
                for (uint i = 0; i < table.Length; ++i)
                {
                    temp = i;
                    for (int j = 8; j > 0; --j)
                    {
                        if ((temp & 1) == 1)
                        {
                            temp = (uint)((temp >> 1) ^ poly);
                        }
                        else
                        {
                            temp >>= 1;
                        }
                    }
                    table[i] = temp;
                }
            }
     }
}