using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class TableSeek
    {
        public TableSeek()
        {
            Name = "";
            Description = "";
            SearchStr = "";
            Rows = 0;
            Columns = 0;
            RowHeaders = "";
            ColHeaders = "";
            Math = "X";
            SavingMath = "X";
            Offset = 0;
            ConditionalOffset = false;
            Bits = 16;
            Decimals = 2;
            DataType = 1;
            UseHit = "1";
            Range = "";
            Segments = "";
        }

        public string Name { get; set; }
        public string SearchStr { get; set; }
        public byte Rows { get; set; }
        public byte Columns { get; set; }
        public string RowHeaders { get; set; }
        public string ColHeaders { get; set; }
        public string Math { get; set; }
        public string SavingMath { get; set; }
        public int Offset { get; set; }
        public bool ConditionalOffset { get; set; }
        public ushort Bits { get; set; }
        public bool Signed { get; set; }
        public ushort Decimals { get; set; }
        public ushort DataType { get; set; }
        public string UseHit { get; set; }
        public string Range { get; set; }
        public string Segments { get; set; }
        public string ValidationSearchStr { get; set; }
        public string Description { get; set; }

        public string seekTables(PcmFile PCM)
        {
            string retVal = "Seeking tables..." + Environment.NewLine;
            try
            {
                string fileName = Path.Combine(Application.StartupPath, "XML", "TableSeek-" + PCM.xmlFile + ".xml");
                if (fileName != tableSeekFile)
                {
                    if (File.Exists(fileName))
                    {
                        tableSeekFile = fileName;
                        Debug.WriteLine("Loading " + fileName);
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSeek>));
                        System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                        tableSeeks = (List<TableSeek>)reader.Deserialize(file);
                        file.Close();

                    }
                    else
                    {
                        tableSeeks = new List<TableSeek>();
                        return retVal;
                    }
                }
                for (int s = 0; s < tableSeeks.Count; s++)
                {
                    uint startAddr = 0;
                    uint endAddr = PCM.fsize;
                    List<Block> addrList = new List<Block>();
                    SearchedAddress sAddr;
                    sAddr.Addr = uint.MaxValue;
                    sAddr.Rows = 0;
                    sAddr.Columns = 0;
                    Block block = new Block();
                    if (tableSeeks[s].Range != null && tableSeeks[s].Range.Length > 0)
                    {
                        string[] rangeS = tableSeeks[s].Range.Split(',');
                        for (int r=0; r<rangeS.Length; r++)
                        {
                            string[] range = rangeS[r].Split('-');
                            if (range.Length != 2) throw new Exception("Unknown address range:" + rangeS[r]);
                            if (HexToUint(range[0],out block.Start) == false) throw new Exception("Unknown HEX code:" + range[0]);
                            if (HexToUint(range[1], out block.End) == false) throw new Exception("Unknown HEX code:" + range[1]);
                            addrList.Add(block);
                        }
                    }
                    else if (tableSeeks[s].Segments != null && tableSeeks[s].Segments.Length > 0)
                    {
                        string[] segStrings = tableSeeks[s].Segments.Split(',');
                        for (int y=0; y< segStrings.Length; y++)
                        {
                            int segNr = 0;
                            if (int.TryParse( segStrings[y], out segNr) == false) throw new Exception("Unknown segment: " + segStrings[y]);
                            for (int b=0; b< PCM.binfile[segNr].SegmentBlocks.Count; b++)
                                addrList.Add(PCM.binfile[segNr].SegmentBlocks[b]);
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
                    string[] hParts = tableSeeks[s].UseHit.Split(',');
                    for (int h=0; h< hParts.Length; h++)
                    {
                        if (hParts[h].Contains("-"))
                        {
                            string[] hParts2 = hParts[h].Split('-');
                            //It's range, loop through all values
                            ushort hStart = 0;
                            ushort hEnd = 1;
                            ushort.TryParse(hParts2[0], out hStart);
                            ushort.TryParse(hParts2[1], out hEnd);
                            for (ushort x=hStart; x <= hEnd; x++)
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
                            Debug.WriteLine("TableSeek: Searching: " + tableSeeks[s].SearchStr + ", Start: " + startAddr.ToString("X") + ", end: " + endAddr.ToString("X"));
                            sAddr = getAddrbySearchString(PCM, tableSeeks[s].SearchStr, ref startAddr, endAddr, tableSeeks[s].ConditionalOffset, tableSeeks[s].ValidationSearchStr);
                            if (sAddr.Addr < PCM.fsize)
                            {
                                hit++;
                                Debug.WriteLine("Found: " + sAddr.Addr.ToString("X") + ", Hit: " + hit.ToString() + " of " + wantedHit);
                            }
                            if (hit == wantedHit && sAddr.Addr < PCM.fsize)
                            {
                                FoundTable ft = new FoundTable();
                                ft.configId = s;
                                ft.Name = tableSeeks[s].Name.Replace("£", (wHit +1).ToString());
                                ft.Description = tableSeeks[s].Description.Replace("£", (wHit + 1).ToString());
                                ft.addrInt = (uint)(sAddr.Addr + tableSeeks[s].Offset);
                                ft.Address = ft.addrInt.ToString("X8");
                                if (tableSeeks[s].Rows > 0)
                                    ft.Rows = tableSeeks[s].Rows;
                                else
                                    ft.Rows = sAddr.Rows;
                                if (tableSeeks[s].Columns > 0)
                                    ft.Columns = tableSeeks[s].Columns;
                                else
                                    ft.Columns = sAddr.Columns;
                                foundTables.Add(ft);
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
                return "Table seek, line " + line + ": " + ex.Message;
            }
            retVal += "Done";
            return retVal;
        }
    }

    public class FoundTable
    {
        public FoundTable()
        {
            Name = "";
            addrInt = uint.MaxValue;
            Address = "";
            configId = -1;
            Description = "";
        }
        public string Name { get; set; }

        public uint addrInt;
        public string Address { get; set; }
        public byte Rows { get; set; }
        public byte Columns { get; set; }
        public int configId;
        public string Description { get; set; }
    }


}
