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

    public struct ExcludeBlock
    {
        public uint Start;
        public uint End;
    }

    public class Segment
    {
        public string Name;     
        public string Addresses; //Segment addresses, can be multiple parts
        public uint CSA1;       //Checksum 1 Address
        public uint CSA2;       //Checksum 2 Address
        public short CSMethod1; //Checksum 1 calculation method
        public short CSMethod2; //Checksum 2 calculation method
        public short CSSize1;   //Checksum 1 size: byte, word or dword (1,2,4)
        public short CSSize2;   //Checksum 2 size: byte, word or dword (1,2,4)
    }

    public const short CSMethod_None = 0;
    public const short CSMethod_crc16 = 1;
    public const short CSMethod_crc32 = 2;
    public const short CSMethod_sum = 3;
    public const short CSMethod_invsum = 4;

    public static List<Segment> Segments = new List<Segment>();
    public static List<ExcludeBlock> Exclude;

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

    public static uint CalculateChecksum(string Address, byte[] Data)
    {
        string[] Parts = Address.Split(',');
        uint sum = 0;

        for (int p = 0; p < Parts.Length; p++)
        {
            string[] StartEnd = Parts[p].Split('-');
            for (int se = 0; se < StartEnd.Length; se++)
            {
                uint StartAddr;
                uint EndAddr;
                UInt32.TryParse(StartEnd[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out StartAddr);
                UInt32.TryParse(StartEnd[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out EndAddr);

                for (uint i = StartAddr + 2; i < EndAddr; i += 2)
                {
                    sum += BEToUint16(Data, i);
                }
            }
        }
        sum = (sum & 0xFFFF);
        return (65536 - sum) & 0xFFFF;
    }

    public static uint CalculateInvChecksum(string Address, byte[] Data)
    {
        string[] Parts = Address.Split(',');
        uint sum = 0;

        for (int p = 0; p < Parts.Length; p++)
        {
            string[] StartEnd = Parts[p].Split('-');
            for (int se = 0; se < StartEnd.Length; se++)
            {
                uint StartAddr;
                uint EndAddr;
                UInt32.TryParse(StartEnd[0], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out StartAddr);
                UInt32.TryParse(StartEnd[1], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out EndAddr);

                for (uint i = StartAddr + 2; i < EndAddr; i += 1)
                {
                    sum += (uint)(Data[i]*256) + Data[i+1];
                }
            }
        }
        sum = (sum & 0xFFFF);
        return (65536 - sum) & 0xFFFF;
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