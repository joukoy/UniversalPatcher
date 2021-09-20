using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;
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

        public string Size { get { return getSize().ToString("X"); } }
        public string SwapSize { get { return getSwapSize().ToString("X"); } }

        public uint getSwapSize()
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
                    return csToString(getCS1(), PCM.segmentAddressDatas[seg].CS1Address.Bytes, PCM.Segments[seg].CS1Method);
            }
        }
        public string CS2 { get { return csToString(getCS2(), PCM.segmentAddressDatas[seg].CS2Address.Bytes, PCM.Segments[seg].CS2Method); } }
        public string CS1Calc { get { return csToString(getCS1Calc(), PCM.segmentAddressDatas[seg].CS1Address.Bytes, PCM.Segments[seg].CS1Method); } }
        public string CS2Calc { get { return csToString(getCS2Calc(), PCM.segmentAddressDatas[seg].CS1Address.Bytes, PCM.Segments[seg].CS1Method); } }

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
                return CheckStockCVN(PN, Ver, SegNr, getCvn(), true, PCM.configFile + ".xml");
            }
        }

        public string PN
        {
            get
            {
                if (PCM.Segments[seg].Eeprom)
                    return GmEeprom.getPN(PCM.buf);
                else
                    return PCM.ReadInfo(PCM.segmentAddressDatas[seg].PNaddr);
            }
        }

        public string Ver
        {
            get
            {
                if (PCM.Segments[seg].Eeprom)
                    return GmEeprom.getVer(PCM.buf);
                else
                    return PCM.ReadInfo(PCM.segmentAddressDatas[seg].VerAddr);
            }
        }

        public UInt64 getCS1Calc() { return (UInt64)(PCM.calculateCS1(seg)); }
        public UInt64 getCS2Calc() { return (UInt64)(PCM.calculateCS2(seg)); }
        public UInt64 getCS1() { return (UInt64)(PCM.ReadValue(PCM.segmentAddressDatas[seg].CS1Address)); }
        public UInt64 getCS2() { return (UInt64)(PCM.ReadValue(PCM.segmentAddressDatas[seg].CS2Address)); }
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

        public uint getSize()
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

        public UInt64 getCvn()
        {
            switch (PCM.Segments[seg].CVN)
            {
                case 1:
                    return getCS1Calc();
                case 2:
                    return getCS2Calc();
                default:
                    return UInt64.MaxValue; ;
            }
        }

        public static string csToString(UInt64 cs, int csBytes, short csMethod)
        {
            string HexLength;
            if (cs == UInt64.MaxValue)
                return "";
            if (csBytes == 0)
            {
                HexLength = "X4";
                if (csMethod == CSMethod_crc32 || csMethod == CSMethod_Dwordsum)
                    HexLength = "X8";
            }
            else
            {
                HexLength = "X" + (csBytes * 2).ToString();
            }
            return cs.ToString(HexLength);
        }

    }
}
