using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
public class upatcher
{
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

    public struct BinFile
    {
        public Block[] SegmentBlocks;
        public Block[] CS1Blocks;
        public Block[] CS2Blocks;
        public AddressData CS1Address;
        public AddressData CS2Address;
        public AddressData PNaddr;
        public AddressData VerAddr;
        public AddressData SegNrAddr;
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
        public string PNAddr;
        public string VerAddr;
        public string SegNrAddr;
    }

    public const short CSMethod_None = 0;
    public const short CSMethod_crc16 = 1;
    public const short CSMethod_crc32 = 2;
    public const short CSMethod_Bytesum = 3;
    public const short CSMethod_Wordsum = 4;
    public const short CSMethod_Dwordsum = 5;

    public static List<SegmentConfig> Segments = new List<SegmentConfig>();

    public struct  AddressData
    {
        public uint Address;
        public ushort Bytes;
        public ushort Type;
    }


    public const ushort TypeText = 0;
    public const ushort TypeHex = 1;
    public const ushort TypeInt = 2;

    public static string SelectFile(string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*")
    {

        OpenFileDialog fdlg = new OpenFileDialog();
        fdlg.Title = "Select file";
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;
        if (fdlg.ShowDialog() == DialogResult.OK)
        {
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

        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            return saveFileDialog.FileName;
        }
        else
            return "";

    }

    public static bool HexToUint(string Hex, out uint x)
    {
        x=0;        
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

    public static AddressData ParseAddress (string Line, uint SegmentAddress)
    {
        AddressData AD = new AddressData();

        string[] Lineparts = Line.Split(':');
        if  (!HexToUint(Lineparts[0].Replace("#", ""), out AD.Address))
            return AD;

        if (Line.StartsWith("#"))
        {
            AD.Address += SegmentAddress;
        }

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

        return AD;
    }

    public static string ReadInfo( byte[] buf, AddressData AD )
    {

        string Result = "";
        if (AD.Bytes == 1)
        { 
            if (AD.Type == TypeHex)
                Result = buf[AD.Address].ToString("X2");
            else if (AD.Type == TypeText)
                Result = System.Text.Encoding.ASCII.GetString(buf, (int)AD.Address, AD.Bytes);
            else
                Result = buf[AD.Address].ToString();
        }
        else if (AD.Bytes == 2)
            if (AD.Type == TypeHex)
                Result = BEToUint16(buf, AD.Address).ToString("X4");
            else if (AD.Type == TypeText)
                Result = System.Text.Encoding.ASCII.GetString(buf, (int)AD.Address, AD.Bytes);
            else
                Result = BEToUint16(buf, AD.Address).ToString();
        else
            if (AD.Type == TypeHex)
                Result = BEToUint32(buf, AD.Address).ToString("X4");
        else if (AD.Type == TypeText)
            Result = System.Text.Encoding.ASCII.GetString(buf, (int)AD.Address, AD.Bytes);
        else
            Result = BEToUint32(buf, AD.Address).ToString();

        return Result;
    }

    public static bool ParseSegmentAddresses(string Line, SegmentConfig S, byte[] buf, out Block[] Blocks)
    {
        if (Line == null || Line == "")
        {
            //It is ok to have empty address (for CS, not for segment)
            Blocks = new Block[1];
            Blocks[0].End = 0;
            Blocks[0].Start = 0;
            return true;
        }
        string[] Parts = Line.Split(',');
        Blocks = new Block[Parts.Length];
        int i = 0;
        
        foreach (string Part in Parts)
        {
            string[] StartEnd = Part.Split('-');
            Block B = new Block();
            if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
            {
                Blocks[i].Start = 0;
                Blocks[i].End = 0;
                return false;
            }
            if (StartEnd[0].StartsWith("@"))
            {
                //Read address from bin at this address
                uint tmp = B.Start;
                B.Start = BEToUint32(buf, B.Start);
                B.End = BEToUint32(buf, tmp + 4);
            }
            else
            {
                if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                    return false;
                if (B.End >= buf.Length)    //Make 1MB config work with 512kB bin
                    B.End = (uint)buf.Length - 1;
            }
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
                B.End = (uint)buf.Length - end;
            }
            Blocks[i] = B;
            i++;
        }
        return true;
    }

    public static void GetSegmentAddresses(byte[] buf, out BinFile[] binfile)
    {
        binfile = new BinFile[Segments.Count];
        for (int i=0; i< Segments.Count;i++)
        {
            SegmentConfig S = Segments[i];
            Block[] B = new Block[1];
            B[0].Start = 0;
            B[0].End = 0;
            if (!ParseSegmentAddresses(S.Addresses, S, buf, out B))
                return;
            binfile[i].SegmentBlocks = B ;
            if (!ParseSegmentAddresses(S.CS1Blocks, S, buf, out B))
                return;
            binfile[i].CS1Blocks = B;
            if (!ParseSegmentAddresses(S.CS2Blocks, S, buf, out B))
                return;
            binfile[i].CS2Blocks = B;
            binfile[i].CS1Address = ParseAddress(S.CS1Address, binfile[i].SegmentBlocks[0].Start);
            binfile[i].CS2Address = ParseAddress(S.CS2Address, binfile[i].SegmentBlocks[0].Start);
            binfile[i].PNaddr = ParseAddress(S.PNAddr, binfile[i].SegmentBlocks[0].Start);
            binfile[i].VerAddr = ParseAddress(S.VerAddr, binfile[i].SegmentBlocks[0].Start);
            binfile[i].SegNrAddr = ParseAddress(S.SegNrAddr, binfile[i].SegmentBlocks[0].Start);
        }
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

    public static uint CalculateChecksum(byte[] Data, AddressData CSAddress, Block[] CSBlocks, short Method, short Complement, ushort Bytes)
    {
        uint sum = 0;
        uint BufSize = 0;
        List<Block> Blocks = new List<Block>();
        
        for (int p = 0; p < CSBlocks.Length; p++)
        {
            Block B = new Block();
            B.Start = CSBlocks[p].Start;
            B.End = CSBlocks[p].End;
            if (CSAddress.Address >= B.Start && CSAddress.Address <= B.End)
            {
                //Checksum  is located in this block
                if (CSAddress.Address == B.Start)    //At beginning of segment
                {
                    B.Start += CSAddress.Bytes;
                }
                else
                {
                    //Located at middle of block, create new block C, ending before checksum
                    Block C = new Block();
                    C.Start = B.Start;
                    C.End = CSAddress.Address - 1;
                    Blocks.Add(C);
                    BufSize += C.End - C.Start + 1;
                    B.Start = CSAddress.Address + CSAddress.Bytes; //Move block B to start after Checksum
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
        if (Bytes == 2)
        { 
            sum = (sum & 0xFFFF);
            if (Method == CSMethod_crc16 || Method == CSMethod_crc32)
                sum = (ushort)SwapBytes((ushort)sum);
        }
        return sum;
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