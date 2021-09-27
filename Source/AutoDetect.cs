using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static upatcher;

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
                    if (DR.hexdata != null && DR.hexdata.Length > 0)
                    {
                        uint bytes = 1;
                        uint.TryParse(Parts[1], out bytes);
                        StringBuilder readBytes = new StringBuilder();
                        for (int a = 0; a < bytes; a++)
                        {
                            readBytes.Append(PCM.buf[Addr + a].ToString("X2"));
                        }
                        if (readBytes.ToString() == DR.hexdata)
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
                            Data = BEToUint16(PCM.buf, Addr);
                        else
                        {
                            switch (Parts[1])
                            {
                                case "1":
                                    Data = BEToUint16(PCM.buf, Addr);
                                    break;
                                case "2":
                                    Data = (uint)BEToUint16(PCM.buf, Addr);
                                    break;
                                case "4":
                                    Data = BEToUint32(PCM.buf, Addr);
                                    break;
                                case "8":
                                    Data = BEToUint64(PCM.buf, Addr);
                                    break;
                                default:
                                        Data = BEToUint32(PCM.buf, Addr);
                                    break;
                            }

                        }
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
        public string autoDetect(PcmFile PCM)
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
