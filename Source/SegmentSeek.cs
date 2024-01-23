using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public class SegmentSeek
    {
        public SegmentSeek()
        {
            Name = "";
            SearchStr = "";
            UseHit = "";
            Range = "";
            Segments = "";
            ValidationSearchStr = "";
            Description = "";
            ConditionalOffset = false;
            SignedOffset = false;
            MSB = true;
        }
        public string Name { get; set; }
        public string SearchStr { get; set; }
        public Int64 Offset { get; set; }
        public bool ConditionalOffset { get; set; }
        public bool SignedOffset { get; set; }
        public string UseHit { get; set; }
        public string Range { get; set; }
        public string Segments { get; set; }
        public string ValidationSearchStr { get; set; }
        public string Description { get; set; }
        public bool MSB { get; set; }

        private PcmFile PCM;


        private struct Starter
        {
            public byte StartByte;
            public List<uint> addresses;
        }
        private List<Starter> starters;
        private List<byte> startBytes;

        public SegmentSeek ShallowCopy()
        {
            return (SegmentSeek)this.MemberwiseClone();
        }

        private uint SearchByteSequence(PcmFile PCM, string searchString, uint Start, uint End)
        {
            uint addr;
            try
            {
                string[] searchParts = searchString.Trim().Split(' ');
                byte[] bytes = new byte[searchParts.Length];

                int startByteInd = int.MaxValue;

                for (int b = 0; b < searchParts.Length; b++)
                {
                    byte searchval = 0;
                    if (searchParts[b] != "*")
                    {
                        HexToByte(searchParts[b], out searchval);
                        if (startByteInd == int.MaxValue)
                            startByteInd = b;
                    }
                    bytes[b] = searchval;
                }

                int ind = startBytes.IndexOf(bytes[startByteInd]);
                if (ind < 0)
                {
                    //This byte is not in list, use slower method:
                    return Upatcher.SearchBytes(PCM, searchString, Start, End);
                }

                foreach (uint a in starters[ind].addresses)
                {
                    if ((a - startByteInd) >= Start && (a - startByteInd) < End)
                    {
                        addr = (uint)(a - startByteInd);
                        bool match = true;
                        if ((addr + searchParts.Length) > PCM.fsize)
                            return uint.MaxValue;
                        for (uint part = 0; part < searchParts.Length; part++)
                        {
                            if (searchParts[part] != "*")
                            {
                                if (PCM.buf[addr + part] != bytes[part])
                                {
                                    match = false;
                                    break;
                                }
                            }
                        }
                        if (match)
                        {
                            return addr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error searchBytes, line " + line + ": " + ex.Message);
            }
            return uint.MaxValue;
        }

        private string GenerateValidationSearchString(uint addr, string origStr)
        {
            string retVal = "";
            string validationAddr = (addr).ToString("X8");
            string[] vParts = origStr.Split(' ');
            int vpartNr = 0;
            for (int v = 0; v < vParts.Length; v++)
            {
                if (vParts[v] == "@")
                {
                    retVal += validationAddr.Substring(vpartNr * 2, 2) + " ";
                    vpartNr++;
                }
                else
                {
                    retVal += vParts[v] + " ";
                }
            }

            return retVal.Trim();
        }

        private SearchedAddress SearchAddrBySearchString(PcmFile PCM, string searchStr, ref uint startAddr, uint endAddr, SegmentSeek tSeek)
        {
            SearchedAddress retVal;
            retVal.Addr = uint.MaxValue;
            retVal.Columns = 0;
            retVal.Rows = 0;
            try
            {
                string modStr = searchStr.Replace("r", "");
                modStr = modStr.Replace("k", "");
                modStr = modStr.Replace("x", "");
                modStr = modStr.Replace("y", "");
                modStr = modStr.Replace("@", "*");
                modStr = modStr.Replace("# ", "* "); //# alone at beginning or middle
                if (modStr.EndsWith("#"))
                    modStr = modStr.Replace(" #", " *"); //# alone at end
                modStr = modStr.Replace("#", ""); //For example: #21 00 21
                uint addr = SearchByteSequence(PCM, modStr, startAddr, endAddr);
                if (addr == uint.MaxValue)
                {
                    //Not found
                    startAddr = uint.MaxValue;
                    return retVal;
                }

                string[] sParts = searchStr.Trim().Split(' ');
                startAddr = addr + (uint)sParts.Length;

                if (tSeek.ValidationSearchStr != null && tSeek.ValidationSearchStr != "")
                {
                    bool validated = false;
                    string[] validationList = tSeek.ValidationSearchStr.Split(',');
                    for (int v = 0; v < validationList.Length; v++)
                    {
                        string vStr = validationList[v];
                        Debug.WriteLine("Validation string: " + vStr + ", Address: " + addr.ToString("X8"));
                        int vLen = vStr.Split('@').Length - 1;
                        if (vLen != 4) throw new Exception("Validation search needs four '@'");
                        string newStr = GenerateValidationSearchString(addr, vStr);
                        Debug.WriteLine("Searching validationstring: " + newStr);
                        if (SearchByteSequence(PCM, newStr, 0, PCM.fsize) < uint.MaxValue)
                        {
                            validated = true;
                            break;
                        }
                        //Try with 0x10k offset:
                        newStr = GenerateValidationSearchString(addr + 0x10000, vStr);
                        Debug.WriteLine("Not found, using 10k offset and searching validationstring: " + newStr);
                        if (SearchByteSequence(PCM, newStr, 0, PCM.fsize) < uint.MaxValue)
                        {
                            validated = true;
                            break;
                        }
                    }
                    if (validated)
                    {
                        Debug.WriteLine("Found, validated");
                    }
                    else
                    {
                        Debug.WriteLine("Not found");
                        retVal.Addr = uint.MaxValue;
                        return retVal;
                    }
                }


                int[] locations = new int[4];
                int l = 0;
                string addrStr = "*";
                if (searchStr.Contains("@")) addrStr = "@";
                else if (searchStr.Contains("*") || searchStr.Contains("#")) addrStr = "*";
                else
                {
                    //Address is AFTER searchstring
                    retVal.Addr = PCM.ReadUInt32(addr + (uint)sParts.Length);
                }
                for (int p = 0; p < sParts.Length; p++)
                {
                    if (sParts[p].Contains(addrStr) && l < 4)
                    {
                        locations[l] = p;
                        l++;
                    }
                    if (sParts[p].Contains("r") || sParts[p].Contains("x"))
                    {
                        retVal.Rows = (ushort)PCM.buf[(uint)(addr + p)];
                    }
                    if (sParts[p].Contains("k") || sParts[p].Contains("y"))
                    {
                        retVal.Columns = (ushort)PCM.buf[(uint)(addr + p)];
                    }
                    if (sParts[p].Contains("#"))
                    {
                        retVal.Addr = (uint)(addr + p);
                    }

                }
                if (retVal.Addr < uint.MaxValue)
                {
                    return retVal;
                }

                //We are here, so we must have @ @ @ @ or @ @ in searchsting
                if (l == 4)
                {
                    if (tSeek.MSB)
                        retVal.Addr = (uint)(PCM.buf[addr + locations[0]] << 24 | PCM.buf[addr + locations[1]] << 16 | PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                    else
                        retVal.Addr = (uint)(PCM.buf[addr + locations[3]] << 24 | PCM.buf[addr + locations[2]] << 16 | PCM.buf[addr + locations[1]] << 8 | PCM.buf[addr + locations[0]]);
                }
                else if (l == 2)
                {
                    if (tSeek.MSB)
                        retVal.Addr = (uint)(PCM.buf[addr + locations[0]] << 8 | PCM.buf[addr + locations[1]]);
                    else
                        retVal.Addr = (uint)(PCM.buf[addr + locations[1]] << 8 | PCM.buf[addr + locations[0]]);
                }
                else
                {
                    LoggerBold("Unknown address definition, must be @ @ or @ @ @ @: " + searchStr);
                    retVal.Addr = uint.MaxValue;
                }

                if (tSeek.ConditionalOffset)
                {
                    ushort addrWord = (ushort)(PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                    if (addrWord > 0x5000)
                        retVal.Addr -= 0x10000;
                }
                if (tSeek.SignedOffset)
                {
                    ushort addrWord = (ushort)(PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                    if (addrWord > 0x8000)
                        retVal.Addr -= 0x10000;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("getAddrbySearchString, line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        public static List<SegmentSeek> LoadSegmentSeekFile(string fName)
        {
            Logger(" (" + Path.GetFileName(fName) + ") ", false);
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentSeek>));
            System.IO.StreamReader file = new System.IO.StreamReader(fName);
            List <SegmentSeek> sSeeks = (List<SegmentSeek>)reader.Deserialize(file);
            file.Close();
            return sSeeks;
        }

        public void SeekSegments(PcmFile PCM1)
        {
            PCM = PCM1;
            try
            {

                segmentSeeks = new List<SegmentSeek>();

                string fileName = PCM.SegmentSeekFile;
                if (File.Exists(fileName))
                {
                    segmentSeeks = LoadSegmentSeekFile(fileName);
                }
                else
                {
                    Debug.WriteLine("Configuration not found: " + Path.GetFileName(fileName));
                    return;
                }

                startBytes = new List<byte>();
                starters = new List<Starter>();
                for (int s = 0; s < segmentSeeks.Count; s++)
                {
                    if (segmentSeeks[s].SearchStr != null && segmentSeeks[s].SearchStr.Length > 0)
                    {
                        byte b = 0;
                        string modStr = Regex.Replace(segmentSeeks[s].SearchStr, @"[kxy@#*]", "");
                        modStr = modStr.Trim();

                        string[] bytelist = modStr.Split(' ');
                        if (HexToByte(bytelist[0], out b))
                        {
                            if (!startBytes.Contains(b))
                            {
                                startBytes.Add(b);
                                Starter st = new Starter();
                                st.addresses = new List<uint>();
                                st.StartByte = b;
                                starters.Add(st);
                            }
                        }
                    }
                    if (segmentSeeks[s].ValidationSearchStr != null && segmentSeeks[s].ValidationSearchStr.Length > 0)
                    {
                        string[] valStrings = segmentSeeks[s].ValidationSearchStr.Split(',');
                        foreach (string valStr in valStrings)
                        {
                            byte b = 0;
                            string modStr = Regex.Replace(valStr, @"[kxy@#*]", "");
                            modStr = modStr.Trim();

                            string[] bytelist = modStr.Split(' ');
                            if (HexToByte(bytelist[0], out b))
                            {
                                if (!startBytes.Contains(b))
                                {
                                    startBytes.Add(b);
                                    Starter st = new Starter();
                                    st.addresses = new List<uint>();
                                    st.StartByte = b;
                                    starters.Add(st);
                                }
                            }
                        }
                    }
                }

                for (uint addr = 0; addr < PCM.buf.Length; addr++)
                {
                    byte b = PCM.buf[addr];
                    int ind = startBytes.IndexOf(b);
                    if (ind > -1)
                        starters[ind].addresses.Add(addr);
                }


                for (int s = 0; s < segmentSeeks.Count; s++)
                {
                    //Logger(".", false);
                    Application.DoEvents();
                    Debug.WriteLine(segmentSeeks[s].Name + " Usehit: " + segmentSeeks[s].UseHit);
                    if (segmentSeeks[s].SearchStr.Length == 0)
                        continue;   //Can't search if string is empty!
                    uint startAddr = 0;
                    uint endAddr = PCM.fsize;
                    List<Block> addrList = new List<Block>();
                    SearchedAddress sAddr;
                    sAddr.Addr = uint.MaxValue;
                    sAddr.Rows = 0;
                    sAddr.Columns = 0;
                    Block block = new Block();
                    if (segmentSeeks[s].Range != null && segmentSeeks[s].Range.Length > 0)
                    {
                        string[] rangeS = segmentSeeks[s].Range.Split(',');
                        for (int r = 0; r < rangeS.Length; r++)
                        {
                            string[] range = rangeS[r].Split('-');
                            if (range.Length != 2) throw new Exception("Unknown address range:" + rangeS[r]);
                            if (HexToUint(range[0], out block.Start) == false) throw new Exception("Unknown HEX code:" + range[0]);
                            if (HexToUint(range[1], out block.End) == false) throw new Exception("Unknown HEX code:" + range[1]);
                            addrList.Add(block);
                        }
                    }
                    else if (segmentSeeks[s].Segments != null && segmentSeeks[s].Segments.Length > 0)
                    {
                        string[] segStrings = segmentSeeks[s].Segments.Split(',');
                        for (int y = 0; y < segStrings.Length; y++)
                        {
                            int segNr = 0;
                            if (int.TryParse(segStrings[y], out segNr))
                            {
                                if (PCM.Segments[segNr].Addresses.Contains("seek:"))
                                {
                                    Debug.WriteLine("Segmentseek: Parsing address for segment " + segNr.ToString() + " contains seek:, can't use it");
                                    block.Start = 0;
                                    block.End = PCM.fsize;
                                    addrList.Add(block);
                                }
                                else if (PCM.ParseSegmentAddresses(PCM.Segments[segNr].Addresses, PCM.Segments[segNr], out List<Block> sBlocks))
                                {
                                    Debug.WriteLine("Segmentseek: Parsing address for segment " + segNr.ToString());
                                    addrList.AddRange(sBlocks);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Segmentseek: unknown segment number: " + segStrings[y] );
                                block.Start = 0;
                                block.End = PCM.fsize;
                                addrList.Add(block);
                            }
                        }
                    }
                    else
                    {
                        block.Start = 0;
                        block.End = PCM.fsize;
                        addrList.Add(block);
                    }
                    int hit = 0;
                    ushort wantedHit = 1;
                    List<ushort> wantedHitList = new List<ushort>();
                    string[] hParts = segmentSeeks[s].UseHit.Split(',');
                    for (int h = 0; h < hParts.Length; h++)
                    {
                        if (hParts[h].Contains("-"))
                        {
                            string[] hParts2 = hParts[h].Split('-');
                            //It's range, loop through all values
                            ushort hStart = 0;
                            ushort hEnd = 1;
                            ushort.TryParse(hParts2[0], out hStart);
                            ushort.TryParse(hParts2[1], out hEnd);
                            for (ushort x = hStart; x <= hEnd; x++)
                                wantedHitList.Add(x);
                        }
                        else
                        {
                            //Single value
                            if (ushort.TryParse(hParts[h], out wantedHit))
                            {
                                wantedHitList.Add(wantedHit);
                            }
                        }
                    }

                    int wHit = 0;
                    for (int b = 0; b < addrList.Count; b++)
                    {
                        startAddr = addrList[b].Start;
                        endAddr = addrList[b].End;
                        while (startAddr < PCM.fsize)
                        {
                            wantedHit = wantedHitList[wHit];
                            string[] ssParts = segmentSeeks[s].SearchStr.Split('+');     //At end of string can be +D4 +1W6 etc, for reading next address from found addr
                            Debug.WriteLine("SegmentSeek: Searching: " + segmentSeeks[s].SearchStr + ", Start: " + startAddr.ToString("X") + ", end: " + endAddr.ToString("X"));
                            sAddr = SearchAddrBySearchString(PCM, ssParts[0], ref startAddr, endAddr, segmentSeeks[s]);
                            for (int jump = 1; jump < ssParts.Length && sAddr.Addr < PCM.fsize; jump++)
                            {
                                //Read table address from address we found by searchstring
                                string numOnly = ssParts[jump].Replace("+", "").Replace("D", "").Replace("W", "");
                                int offset = Convert.ToInt32(numOnly);  //For first jump, use SegmentSeek offset, for other jumps use searchstring offset
                                uint currentAddr = (uint)(sAddr.Addr + offset);
                                Debug.WriteLine("SegmentSeek: Reading new address from:" + currentAddr.ToString("X"));
                                if (ssParts[jump].Contains("D"))
                                    sAddr.Addr = (uint)(PCM.ReadUInt32(currentAddr));
                                else
                                    sAddr.Addr = (uint)(PCM.ReadUInt16(currentAddr));
                                Debug.WriteLine("SegmentSeek: New address:" + sAddr.Addr.ToString("X"));
                            }

                            if ((sAddr.Addr + segmentSeeks[s].Offset) < PCM.fsize)
                            {
                                hit++;
                                Debug.WriteLine("Found: " + sAddr.Addr.ToString("X") + ", Hit: " + hit.ToString() + " of " + wantedHit);
                            }
                            if (hit == wantedHit && (sAddr.Addr + segmentSeeks[s].Offset) < PCM.fsize)
                            {
                                FoundSegment fs = new FoundSegment(PCM);
                                fs.Name = segmentSeeks[s].Name.Replace("£", (wHit + 1).ToString());
                                fs.Description = segmentSeeks[s].Description.Replace("£", (wHit + 1).ToString());
                                fs.addrInt = (uint)(sAddr.Addr + segmentSeeks[s].Offset);
                                //fs.Address = fs.addrInt.ToString("X8");
                                PCM.foundSegments.Add(fs);
                                wHit++;
                            }
                            if (wHit >= wantedHitList.Count) break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold ("Segment seek, line " + line + ": " + ex.Message);
            }
            return;
        }
    }

    public class FoundSegment
    {
        public FoundSegment(PcmFile PCM)
        {
            Name = "";
            addrInt = uint.MaxValue;
            Description = "";
            id = PCM.foundSegments.Count;
        }
        public int id { get; set; }
        public string Name { get; set; }

        public uint addrInt;
        public string Address
        {
            get
            {
                if (addrInt == uint.MaxValue)
                    return "";
                else
                    return addrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = addrInt;
                    if (!HexToUint(value, out addrInt))
                        addrInt = prevVal;
                }
                else
                {
                    addrInt = uint.MaxValue;
                }
            }
        }
        public string Description { get; set; }
    }

}

