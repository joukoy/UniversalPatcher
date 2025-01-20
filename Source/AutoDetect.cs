using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    class AutoDetect
    {
        private struct DetectGroup
        {
            public string Logic;
            public uint Hits;
            public uint Miss;
        }

        private bool CheckRule(DetectRule DR, PcmFile PCM)
        {
            try
            {
                UInt64 data = 0;
                uint addr = 0;
                if (DR.address == "filesize")
                {
                    data = (UInt64)new FileInfo(PCM.FileName).Length;
                }
                else
                {
                    string[] Parts = DR.address.Split(':');
                    HexToUint(Parts[0].Replace("@", ""), out addr);
                    if (DR.address.StartsWith("@"))
                        addr = PCM.ReadUInt32(addr);
                    if (Parts[0].EndsWith("@"))
                        addr = (uint)PCM.buf.Length - addr;
                    if (DR.hexdata != null && DR.hexdata.Length > 0)
                    {
                        uint bytes = 1;
                        uint.TryParse(Parts[1], out bytes);
                        if ((addr + bytes) > PCM.buf.Length)
                        {
                            return false;
                        }
                        StringBuilder readBytes = new StringBuilder();
                        for (int a = 0; a < bytes; a++)
                        {
                            readBytes.Append(PCM.buf[addr + a].ToString("X2"));
                        }
                        if (readBytes.ToString() == DR.hexdata.ToUpper())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Parts.Length == 1)
                        {
                            if (PCM.buf.Length > addr)
                            {
                                data = PCM.ReadUInt16(addr);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (byte.TryParse(Parts[1], out byte bLen))
                            {
                                if ((addr + bLen) > PCM.buf.Length)
                                {
                                    return false;
                                }
                            }
                            switch (Parts[1])
                            {
                                case "1":
                                    data = PCM.ReadByte(addr);
                                    break;
                                case "2":
                                    data = (uint)PCM.ReadUInt16(addr);
                                    break;
                                case "4":
                                    data = PCM.ReadUInt32(addr);
                                    break;
                                case "8":
                                    data = PCM.ReadUInt64(addr);
                                    break;
                                default:
                                    data = PCM.ReadUInt32(addr);
                                    break;
                            }

                        }
                    }
                }

                //Logger(DR.xml + ": " + DR.address + ": " + DR.data.ToString("X") + DR.compare + "(" + DR.grouplogic + ") " + " [" + Addr.ToString("X") + ": " + Data.ToString("X") + "]");

                if (DR.compare == "==")
                {
                    if (data == DR.data)
                        return true;
                }
                if (DR.compare == "<")
                {
                    if (data < DR.data)
                        return true;
                }
                if (DR.compare == ">")
                {
                    if (data > DR.data)
                        return true;
                }
                if (DR.compare == "!=")
                {
                    if (data != DR.data)
                        return true;
                }
                //Logger("Not match");
                return false;
            }
            catch (Exception ex)
            {
                //Something wrong, just skip this part and continue
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(st.FrameCount - 1);
                int line = frame.GetFileLineNumber();
                LoggerBold("Autodetect, line " + line + ": " + ex.Message);
                return false;
            }
        }
        public string DetectPlatform(PcmFile PCM)
        {
            string Result = "";

            List<string> XmlList = new List<string>();
            XmlList.Add(DetectRules[0].xml.ToLower());
            for (int s = 0; s < DetectRules.Count; s++)
            {
                //Create list of XML files we know:
                if (!XmlList.Contains(DetectRules[s].xml.ToLower()))
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

    }
}
