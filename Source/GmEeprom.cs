﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Upatcher;
using static Helpers;

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

            tmpBuf = ReadBin(FileName);

            return GetVIN(tmpBuf);
        }

        public static EepromKey GetEepromKey(byte[] buf)
        {
            uint VINAddr = GetVINAddr(buf);
            EepromKey tmpKey;

            //Calculate key
            tmpKey.Seed = ReadUint16(buf, VINAddr,true);
            tmpKey.Key = ReadUint16(buf, VINAddr + 2,true);

            tmpKey.NewKey = (UInt16)(tmpKey.Seed + 0x5201);
            tmpKey.NewKey = (UInt16)(SwapBytes(tmpKey.NewKey,2) + 0x9738);
            tmpKey.NewKey = (UInt16)(0xffff - tmpKey.NewKey - 0xd428);

            return tmpKey;
        }

        public static bool FixEepromKey(byte[] buf)
        {
            bool needFix = false;
            EepromKey Key;
            Key = GetEepromKey(buf);
            Logger("Seed: ".PadRight(16) + Key.Seed.ToString("X4") + Environment.NewLine + "Bin Key: ".PadRight(16) + Key.Key.ToString("X4"),false);
            if (Key.Key != Key.NewKey)
            {
                needFix = true;
                uint VINAddr = GetVINAddr(buf);

                buf[VINAddr + 2] = (byte)((Key.NewKey & 0xFF00) >> 8);
                buf[VINAddr + 3] = (byte)(Key.NewKey & 0xFF);
                Logger("  *  Calculated: ".PadRight(16) + Key.NewKey.ToString("X4") + " [Fixed]");
            }
            else
                Logger(" [OK]");
            return needFix;
        }
        public static string GetKeyStatus(byte[]buf)
        {
            EepromKey Key;
            Key = GetEepromKey(buf);

            string Ret = " Seed: " + Key.Seed.ToString("X4");
            Ret += ", Bin Key " + Key.Key.ToString("X4");
            if (Key.Key == Key.NewKey)
                Ret += " [OK]";
            else
                Ret += " * Calculated: " + Key.NewKey.ToString("X4") + " [Fail]" ;
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

            string PN = ReadUint32(buf, VINAddr + 4,true).ToString();
            Ver = ReadTextBlock(buf, (int)VINAddr + 0x1C, 4);

            string Ret = " Hardware ".PadRight(20) + ReadUint32(buf, VINAddr + 4,true).ToString() + Environment.NewLine;
            Ret += " Serial ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 8, 12,true) + Environment.NewLine;
            Ret += " Id ".PadRight(20) + ReadUint32(buf, VINAddr + 0x14,true).ToString() + Environment.NewLine;
            Ret += " Id2 ".PadRight(20) + ReadUint32(buf, VINAddr + 0x18,true).ToString() + Environment.NewLine;
            Ret += " Broadcast ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 0x1C, 4) + Environment.NewLine;
            Ret += " PN ".PadRight(20) + PN + Environment.NewLine;
            Ret += " Ver ".PadRight(20) + Ver + Environment.NewLine;
            Ret += " VIN ".PadRight(20) + GetVIN(buf) + Environment.NewLine;
            return Ret;
        }

        public static string GetVer(byte[] buf)
        {
            uint VINAddr = GetVINAddr(buf);
            if (VINAddr == 1) //Check word not found
                return "?";
            return ReadTextBlock(buf, (int)VINAddr + 0x1C, 4);
        }

        public static string GetPN(byte[] buf)
        {
            uint VINAddr = GetVINAddr(buf);
            if (VINAddr == 1) //Check word not found
                return "?";
            return ReadUint32(buf, VINAddr + 4,true).ToString();
        }

        public static string GetExtraInfo(byte[] buf)
        {
            uint VINAddr = GetVINAddr(buf);

            string Ret = " Hardware ".PadRight(20) + ReadUint32(buf, VINAddr + 4,true).ToString() + Environment.NewLine;
            Ret += " Serial ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 8, 12) + Environment.NewLine;
            Ret += " Id ".PadRight(20) + ReadUint32(buf, VINAddr + 0x14,true).ToString() + Environment.NewLine;
            Ret += " Id2 ".PadRight(20) + ReadUint32(buf, VINAddr + 0x18,true).ToString() + Environment.NewLine;
            Ret += " Broadcast ".PadRight(20) + ReadTextBlock(buf, (int)VINAddr + 0x1C, 4) + Environment.NewLine;
            Ret += " VIN ".PadRight(20) + GetVIN(buf) + Environment.NewLine;
            return Ret;
        }

    }
}
