using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public class SegmentInfo
    {
        public SegmentInfo(PcmFile PCM, int seg)
        {
            this.PCM = PCM;
            this.seg = seg;
        }
        private PcmFile PCM;
        private int seg;

        public string Name { get { return PCM.Segments[seg].Name; } }
        public string FileName { get { return PCM.FileName; } }
        public string XmlFile { get { return PCM.configFile + ".xml"; } }
        public string Address
        {
            get
            {
                string tmp = "";
                for (int s = 0; s < PCM.segmentAddressDatas[seg].SegmentBlocks.Count; s++)
                {
                    if (s > 0)
                        tmp += ", ";
                    tmp += PCM.segmentAddressDatas[seg].SegmentBlocks[s].Start.ToString("X4") + " - " + PCM.segmentAddressDatas[seg].SegmentBlocks[s].End.ToString("X4");
                }
                return tmp;

            }
        }

        public string SwapAddress
        {
            get
            {
                string tmp = "";
                if (PCM.Segments[seg].SwapAddress != null && PCM.Segments[seg].SwapAddress.Length > 1)
                {
                    for (int s = 0; s < PCM.segmentAddressDatas[seg].SwapBlocks.Count; s++)
                    {
                        if (s > 0)
                            tmp += ", ";
                        tmp += PCM.segmentAddressDatas[seg].SwapBlocks[s].Start.ToString("X4") + " - " + PCM.segmentAddressDatas[seg].SwapBlocks[s].End.ToString("X4");
                    }
                    return tmp;
                }
                else
                    return "";
            }
        }

        public string Size { get { return GetSize().ToString("X"); } }
        public string SwapSize { get { return GetSwapSize().ToString("X"); } }

        public uint GetSwapSize()
        {
            uint SSize = 0;
            try
            {
                for (int s = 0; s < PCM.segmentAddressDatas[seg].SwapBlocks.Count; s++)
                {
                    SSize += PCM.segmentAddressDatas[seg].SwapBlocks[s].End - PCM.segmentAddressDatas[seg].SwapBlocks[s].Start + 1;
                }
            }
            catch { }
            return SSize;
        }

        public string CS1
        {
            get
            {
                if (PCM.Segments[seg].Eeprom)
                    return GmEeprom.GetKeyStatus(PCM.buf);
                else
                    return CsToString(GetCS1(), PCM.segmentAddressDatas[seg].CS1Address.Bytes, PCM.Segments[seg].Checksum1Method, PCM.platformConfig.MSB);
            }
        }
        public string CS2 { get { return CsToString(GetCS2(), PCM.segmentAddressDatas[seg].CS2Address.Bytes, PCM.Segments[seg].Checksum2Method, PCM.platformConfig.MSB); } }
        public string CS1Calc { get { return CsToString(GetCS1Calc(), PCM.segmentAddressDatas[seg].CS1Address.Bytes, PCM.Segments[seg].Checksum1Method, PCM.platformConfig.MSB); } }
        public string CS2Calc { get { return CsToString(GetCS2Calc(), PCM.segmentAddressDatas[seg].CS1Address.Bytes, PCM.Segments[seg].Checksum1Method, PCM.platformConfig.MSB); } }

        public string cvn
        {
            get
            {
                switch (PCM.Segments[seg].CVN)
                {
                    case 1:
                        return CS1Calc;
                    case 2:
                        return CS2Calc;
                    default:
                        return "";
                }
            }
        }

        public string Stock
        {
            get
            {
                if (PCM.Segments[seg].CVN == 0)
                    return "";
                return CheckStockCVN(PN, Ver, SegNr, GetCvn(), true, PCM.configFile + ".xml");
            }
        }

        public string PN
        {
            get
            {
                if (PCM.Segments[seg].Eeprom)
                    return GmEeprom.GetPN(PCM.buf);
                else
                    return PCM.ReadInfo(PCM.segmentAddressDatas[seg].PNaddr);
            }
        }

        public string Ver
        {
            get
            {
                if (PCM.Segments[seg].Eeprom)
                    return GmEeprom.GetVer(PCM.buf);
                else
                    return PCM.ReadInfo(PCM.segmentAddressDatas[seg].VerAddr);
            }
        }

        public UInt64 GetCS1Calc() { return (UInt64)(PCM.CalculateCS1(seg)); }
        public UInt64 GetCS2Calc() { return (UInt64)(PCM.CalculateCS2(seg)); }
        public UInt64 GetCS1() { return (UInt64)(PCM.ReadValue(PCM.segmentAddressDatas[seg].CS1Address)); }
        public UInt64 GetCS2() { return (UInt64)(PCM.ReadValue(PCM.segmentAddressDatas[seg].CS2Address)); }
        public string SegNr { get { return PCM.ReadInfo(PCM.segmentAddressDatas[seg].SegNrAddr); } }

        public string ExtraInfo 
        { 
            get
            {
                if (PCM.Segments[seg].Eeprom)
                {
                    return GmEeprom.GetExtraInfo(PCM.buf);
                }
                if (PCM.segmentAddressDatas[seg].ExtraInfo != null && PCM.segmentAddressDatas[seg].ExtraInfo.Count > 0)
                {
                    string ExtraI = "";
                    for (int e = 0; e < PCM.segmentAddressDatas[seg].ExtraInfo.Count; e++)
                    {
                        if (e > 0)
                            ExtraI += Environment.NewLine;
                        ExtraI += " " + PCM.segmentAddressDatas[seg].ExtraInfo[e].Name + ": " + PCM.ReadInfo(PCM.segmentAddressDatas[seg].ExtraInfo[e]);
                    }
                    return ExtraI;
                }
                else
                {
                    return "";
                }
            }
        }


        public string GetExtraData(int ind)
        {
            string retVal = "";
            if (PCM.segmentAddressDatas[seg].ExtraInfo != null && PCM.segmentAddressDatas[seg].ExtraInfo.Count > 0)
            {
                retVal = PCM.ReadInfo(PCM.segmentAddressDatas[seg].ExtraInfo[ind]);
            }
            return retVal;
        }

        public void SetExtraData(int ind, string Data)
        {
            uint addr = PCM.segmentAddressDatas[seg].ExtraInfo[ind].Address;
            for (int i=0; i< Data.Length; i++)
            {
                byte b = (byte)Data[i];
                PCM.buf[addr + i] = b;
            }
        }

        public uint GetSize()
        {
            try
            {
                if (PCM.Segments[seg].Eeprom)
                {
                    return 0x4000;
                }
                else
                {
                    uint SSize = 0;
                    for (int s = 0; s < PCM.segmentAddressDatas[seg].SegmentBlocks.Count; s++)
                    {
                        SSize += PCM.segmentAddressDatas[seg].SegmentBlocks[s].End - PCM.segmentAddressDatas[seg].SegmentBlocks[s].Start + 1;
                    }
                    return SSize;
                }
            }
            catch
            {
                return 0;
            }
        }

        public UInt64 GetCvn()
        {
            switch (PCM.Segments[seg].CVN)
            {
                case 1:
                    return GetCS1Calc();
                case 2:
                    return GetCS2Calc();
                default:
                    return UInt64.MaxValue; ;
            }
        }

        public static string CsToString(UInt64 cs, int csBytes, CSMethod csMethod, bool MSB)
        {
            string HexLength="X4";
            if (cs == UInt64.MaxValue)
                return "";

            if (csBytes == 0)
            {
                HexLength = "X4";
                if (csMethod == CSMethod.crc32 || csMethod == CSMethod.Dwordsum)
                    HexLength = "X8";
                if (!MSB)
                    cs = SwapBytes(cs, 4);
            }
            else
            {
                HexLength = "X" + (csBytes * 2).ToString();
                if (!MSB)
                    cs = SwapBytes(cs, csBytes);
            }
            return cs.ToString(HexLength);
        }

        public uint GetStartAddr()
        {
            return PCM.segmentAddressDatas[seg].SegmentBlocks[0].Start;
        }

        public uint GetEndAddr()
        {
            return PCM.segmentAddressDatas[seg].SegmentBlocks[PCM.segmentAddressDatas[seg].SegmentBlocks.Count-1].End;
        }
    }
}
