using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static upatcher;

namespace UniversalPatcher
{
    class GmEeprom
    {
        public struct EepromKey
        {
            public UInt16 Seed;
            public UInt16 Key;
            public UInt16 NewKey;
        }

        public static uint GetVINAddr(byte[] buf)
        {
            uint offset = 0;

            if (buf.Length >= (512 * 1024))
            {
                //Full binary
                offset = 0x4000;
            }

            //Find check-word from Eeprom_data:
            uint CheckAddr = offset + 0x88;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return offset;
            CheckAddr = offset + 0x56;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return offset;
            CheckAddr = offset + 0x2088;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return offset + 0x2000;
            CheckAddr = offset + 0x2056;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return offset + 0x2000;
            else
                return 1;
        }

        public static uint GetModelFromEeprom(byte[] buf)
        {
            uint offset = 0;

            if (buf.Length >= (512 * 1024))
            {
                //Full binary
                offset = 0x4000;
            }

            //Find check-word from Eeprom_data:
            uint CheckAddr = offset + 0x88;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return 2001;
            CheckAddr = offset + 0x56;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return 1999;
            CheckAddr = offset + 0x2088;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return 2001;
            CheckAddr = offset + 0x2056;
            if (BitConverter.ToUInt16(buf, (int)CheckAddr) == 0xA0A5)
                return 1999;
            else
                return 1; //Unknown
        }

        public static string ValidateVIN(string VINcode)
        {
            VINcode = Regex.Replace(VINcode, "[^a-zA-Z0-9]", "");   //Replace all special chars with ""
            return VINcode.ToUpper();
        }

        public static string GetVIN(byte[] buf)
        {
            uint VINoffset = GetVINAddr(buf);
            if (VINoffset == 1) //Error
            {
                return "?";
            }
            return ValidateVIN(System.Text.Encoding.ASCII.GetString(buf, (int)VINoffset + 0x21, 17));
        }

        public static string ReadVIN(string FileName)
        {
            long fsize = new System.IO.FileInfo(FileName).Length;
            byte[] tmpBuf = new byte[fsize];

            tmpBuf = ReadBin(FileName, 0, (uint)fsize);

            return GetVIN(tmpBuf);
        }

        public static EepromKey GetEepromKey(byte[] buf)
        {
            uint VINAddr = GetVINAddr(buf);
            EepromKey tmpKey;

            //Calculate key
            tmpKey.Seed = BEToUint16(buf, VINAddr);
            tmpKey.Key = BEToUint16(buf, VINAddr + 2);

            tmpKey.NewKey = (UInt16)(tmpKey.Seed + 0x5201);
            tmpKey.NewKey = (UInt16)(SwapBytes(tmpKey.NewKey) + 0x9738);
            tmpKey.NewKey = (UInt16)(0xffff - tmpKey.NewKey - 0xd428);

            return tmpKey;
        }

        public static string FixEepromKey(byte[] buf)
        {
            EepromKey Key;
            Key = GetEepromKey(buf);
            string Ret = "Seed: ".PadRight(16) + Key.Seed.ToString("X4") + Environment.NewLine + "Bin Key: ".PadRight(16) + Key.Key.ToString("X4");
            if (Key.Key != Key.NewKey)
            {
                uint VINAddr = GetVINAddr(buf);

                buf[VINAddr + 2] = (byte)((Key.NewKey & 0xFF00) >> 8);
                buf[VINAddr + 3] = (byte)(Key.NewKey & 0xFF);
                Ret += "  *  Calculated: ".PadRight(16) + Key.NewKey.ToString("X4") + " [Fixed]";
            }
            else
                Ret += " [OK]";
            return Ret;
        }
        public static string GetKeyStatus(byte[]buf)
        {
            EepromKey Key;
            Key = GetEepromKey(buf);

            string Ret = " Seed: " + Key.Seed.ToString("X4");
            Ret += ", Bin Key " + Key.Key.ToString("X4");
            if (Key.Key == Key.NewKey)
                Ret += " [OK]" + Environment.NewLine;
            else
                Ret += " * Calculated: " + Key.NewKey.ToString("X4") + " [Fail]" + Environment.NewLine;
            return Ret;
        }
        public static string GetEEpromInfoText(byte[] buf)
        {
            uint VINAddr = GetVINAddr(buf);
            string Ver;

            if (VINAddr == 1) //Check word not found
            {
                return "Eeprom_data unreadable" + Environment.NewLine;
            }

            string PN = BEToUint32(buf, VINAddr + 4).ToString();
            Ver = ReadTextBlock(buf, (int)VINAddr + 0x1C, 4);

            string Ret = " Hardware ".PadRight(20) + BEToUint32(buf, VINAddr + 4).ToString() + Environment.NewLine;
            Ret += " Serial ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 8, 12) + Environment.NewLine;
            Ret += " Id ".PadRight(20) + BEToUint32(buf, VINAddr + 0x14).ToString() + Environment.NewLine;
            Ret += " Id2 ".PadRight(20) + BEToUint32(buf, VINAddr + 0x18).ToString() + Environment.NewLine;
            Ret += " Broadcast ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 0x1C, 4) + Environment.NewLine;
            Ret += " PN ".PadRight(20) + PN + Environment.NewLine;
            Ret += " Ver ".PadRight(20) + Ver + Environment.NewLine;
            Ret += " VIN ".PadRight(20) + GetVIN(buf) + Environment.NewLine;
            return Ret;
        }

        public static void GetEEpromInfo(byte[] buf, ref SegmentInfo sinfo)
        {
            uint VINAddr = GetVINAddr(buf);

            if (VINAddr == 1) //Check word not found
            {
                sinfo.PN = "Eeprom_data unreadable";
                return;
            }

            sinfo.Address = VINAddr.ToString("X") + "-" + (VINAddr + 0x1FFF).ToString("X");
            sinfo.Size = "2000";
            sinfo.PN = BEToUint32(buf, VINAddr + 4).ToString();
            sinfo.Ver = ReadTextBlock(buf, (int)VINAddr + 0x1C, 4);

            string Ret = " Hardware ".PadRight(20) + BEToUint32(buf, VINAddr + 4).ToString() + Environment.NewLine;
            Ret += " Serial ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 8, 12) + Environment.NewLine;
            Ret += " Id ".PadRight(20) + BEToUint32(buf, VINAddr + 0x14).ToString() + Environment.NewLine;
            Ret += " Id2 ".PadRight(20) + BEToUint32(buf, VINAddr + 0x18).ToString() + Environment.NewLine;
            Ret += " Broadcast ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 0x1C, 4) + Environment.NewLine;
            Ret += " VIN ".PadRight(20) + GetVIN(buf) + Environment.NewLine;
            sinfo.ExtraInfo = Ret;
        }

    }
}
