using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static Helpers;

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
            //SavingMath = "X";
            Offset = 0;
            ConditionalOffset = false;
            SignedOffset = false;
            Decimals = 2;
            Min = 0;
            Max = 255;
            OutputType = OutDataType.Float;
            UseHit = "1";
            Range = "";
            Segments = "";
            Units = "";
            Values = "";
            RowMajor = true;
            MSB = true;
        }

        public string Name { get; set; }
        public string SearchStr { get; set; }
        public ushort Rows { get; set; }
        public ushort Columns { get; set; }
        public string RowHeaders { get; set; }
        public string ColHeaders { get; set; }
        public string Math { get; set; }
        //public string SavingMath { get; set; }
        public Int64 Offset { get; set; }
        public bool ConditionalOffset { get; set; }
        public bool SignedOffset { get; set; }
        public InDataType DataType { get; set; }
        public ushort Decimals { get; set; }
        public OutDataType OutputType { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public string UseHit { get; set; }
        public string Range { get; set; }
        public string Segments { get; set; }
        public string ValidationSearchStr { get; set; }
        public List<string> Categories;
        public string Category { get; set; }
        public string Units { get; set; }
        public string Values { get; set; }
        public string BitMask { get; set; }
        public bool RowMajor { get; set; }
        public string Description { get; set; }
        public bool MSB { get; set; }
        public DisplayUnits DispUnits { get; set; }
        public string RefAddress { get; set; }
        public string ExtraTableName { get; set; }
        public string ExtraDescription { get; set; }
        public string ExtraCategories { get; set; }
        public string ApplySearchForCount { get; set; }

        private PcmFile PCM;

        private struct Starter
        {
            public byte StartByte;
            public List<uint> addresses;
        }
        private List<Starter> starters;
        private List<byte> startBytes;

        public TableSeek ShallowCopy()
        {
            return (TableSeek)this.MemberwiseClone();
        }

        public void ImportTableData(TableData td)
        {
            BitMask = td.BitMask;
            Category = td.Category;
            ColHeaders = td.ColumnHeaders;
            Columns = td.Columns;
            Decimals = td.Decimals;
            Description = td.TableDescription;
            ExtraDescription = td.ExtraDescription;
            ExtraCategories = td.ExtraCategories;
            Math = td.Math;
            Max = td.Max;
            Min = td.Min;
            Name = td.TableName;
            Offset = td.Offset;
            OutputType = td.OutputType;
            RowHeaders = td.RowHeaders;
            RowMajor = td.RowMajor;
            Rows = td.Rows;
            Values = td.Values;
            Units = td.Units;
            DataType = td.DataType;
            RefAddress = td.Address;
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

        private  string GenerateValidationSearchString(uint addr, string origStr)
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

        private SearchedAddress SearchAddrBySearchString(PcmFile PCM, string searchStr, ref uint startAddr, uint endAddr, TableSeek tSeek)
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

                if (!string.IsNullOrEmpty(tSeek.ValidationSearchStr))
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

        public static List<TableSeek> LoadTableSeekFile(string fName)
        {
            Logger(" (" + Path.GetFileName(fName) + ") ", false);
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSeek>));
            System.IO.StreamReader file = new System.IO.StreamReader(fName);
            List<TableSeek> tSeeks = (List<TableSeek>)reader.Deserialize(file);
            file.Close();
            return tSeeks;
        }

        public string SeekTables(PcmFile PCM1)
        {
            PCM = PCM1;
            string retVal = "";
            try
            {

                PCM.foundTables = new List<FoundTable>();

                //PCM.tableCategories = new List<string>();
                //PCM.tableCategories.Add("_All");
                if (PCM.segmentinfos == null)
                    return "";
                for (int c = 0; c < PCM.segmentinfos.Length; c++)
                {
                    if (!PCM.Segments[c].Missing)
                        PCM.tableCategories.Add("Seg-" + PCM.segmentinfos[c].Name);
                }

                string fileName = PCM.TableSeekFile;
                if (File.Exists(fileName))
                {
                    tableSeeks = LoadTableSeekFile(fileName);
                }
                else
                {
                    if (PCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                    {
                        //if (!PCM.tableCategories.Contains("Fuel")) PCM.tableCategories.Add("Fuel");
                        tableSeeks = new List<TableSeek>();
                        TableSeek ts = new TableSeek();
                        FoundTable ft = new FoundTable(PCM);

                        ft.Address = PCM.v6VeTable.address.ToString("X8");
                        ft.addrInt = PCM.v6VeTable.address;
                        ft.Rows = (byte)PCM.v6VeTable.rows;
                        ft.Columns = 17;
                        ft.configId = 0;
                        ft.Name = "VE";
                        ft.Category = "Fuel";
                        ft.Description = "Volumetric Efficiency";
                        PCM.foundTables.Add(ft);

                        ts.Name = "VE";
                        ts.Description = "Volumetric Efficiency";
                        ts.DataType = InDataType.UWORD;
                        //ts.Bits = 16;
                        //ts.Floating = true;
                        ts.OutputType = OutDataType.Float;
                        ts.Decimals = 6;
                        ts.Math = "X*0.0002441406";
                        ts.Offset = 0;
                        //ts.SavingMath = "X/0.0002441406";
                        //ts.Signed = false;
                        ts.Category = "Fuel";                            
                        ts.ColHeaders = "RPM 0,400,800,1200,1600,2000,2400,2800,3200,3600,4000,4400,4800,5200,5600,6000,6400, 6800";
                        if (ft.Rows == 15)
                            ts.RowHeaders = "kpa 0,10,20,30,40,50,60,70,80,90,100,110,120,130,140";
                        else
                            ts.RowHeaders = "kpa 20,30,40,50,60,70,80,90,100,110,120,130,140";
                        ts.RowMajor = false;
                        tableSeeks.Add(ts);

                        ft = new FoundTable(PCM);
                        HexToUint(PCM.mafAddress, out ft.addrInt);
                        ft.Address = ft.addrInt.ToString("X8");
                        ft.Rows = 81;
                        ft.Columns = 1;
                        ft.configId = 1;
                        ft.Name = "MAF";
                        ft.Category = "Fuel";
                        ft.Description = "Grams Per Second";
                        PCM.foundTables.Add(ft);

                        ts = new TableSeek();
                        ts.DataType = InDataType.UWORD;
                        //ts.Bits = 16;
                        ts.Name = "MAF";
                        ts.Math = "X*0.0078125";
                        //ts.SavingMath = "X/0.0078125";
                        //ts.Floating = true;
                        ts.OutputType = OutDataType.Float;
                        ts.Decimals = 4;
                        //ts.Signed = false;
                        ts.Category = "Fuel";
                        ts.Units = "grams/s";
                        ts.RowHeaders = "1500,";
                        for (int rh = 1; rh < 82; rh++)
                        {
                            ts.RowHeaders += (1500 + rh * 125).ToString();
                            if (rh < 81) ts.RowHeaders += ",";
                        }
                        ts.ColHeaders = "g/s";
                        ts.Description = "Grams Per Second";
                        ts.RowMajor = false;
                        tableSeeks.Add(ts);

                        retVal += "OK";
                        return retVal;
                    }
                    else
                    {
                        tableSeeks = new List<TableSeek>();
                        retVal += "Configuration not found: " + Path.GetFileName(fileName) + Environment.NewLine;
                        return retVal;
                    }
                }
                Stopwatch timer = new Stopwatch();
                timer.Start();

                startBytes = new List<byte>();
                starters = new List<Starter>();
                for (int s = 0; s < tableSeeks.Count; s++)
                {
                    TableSeek ts = tableSeeks[s];
                    if (ts.SearchStr != null && ts.SearchStr.Length > 0)
                    {
                        byte b = 0;
                        string modStr = Regex.Replace(ts.SearchStr, @"[kxy@#*]", "");
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
                    if (ts.ValidationSearchStr != null && ts.ValidationSearchStr.Length > 0)
                    {
                        string[] valStrings = ts.ValidationSearchStr.Split(',');
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

                for (uint addr=0; addr< PCM.buf.Length; addr++)
                {
                    byte b = PCM.buf[addr];
                    int ind = startBytes.IndexOf(b);
                    if (ind > -1)
                        starters[ind].addresses.Add(addr);
                }
                timer.Stop();
                Debug.WriteLine("Starters search Time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));


                for (int s = 0; s < tableSeeks.Count; s++)
                {
                    TableSeek ts = tableSeeks[s];
                    Application.DoEvents();
                    Debug.WriteLine(ts.Name + " Usehit: " + ts.UseHit);
                    if (ts.SearchStr.Length == 0)
                        continue;   //Can't search if string is empty!
                    for (int c = 0; c < ts.Categories.Count; c++)
                    {
                        //if (!PCM.tableCategories.Contains(ts.Categories[c]))
                          //  PCM.tableCategories.Add(ts.Categories[c]);
                    }
                    uint startAddr = 0;
                    uint endAddr = PCM.fsize;
                    List<Block> addrList = new List<Block>();
                    SearchedAddress sAddr;
                    sAddr.Addr = uint.MaxValue;
                    sAddr.Rows = 0;
                    sAddr.Columns = 0;
                    Block block = new Block();
                    if (ts.Range != null && ts.Range.Length > 0)
                    {
                        string[] rangeS = ts.Range.Split(',');
                        for (int r=0; r<rangeS.Length; r++)
                        {
                            string[] range = rangeS[r].Split('-');
                            if (range.Length != 2) throw new Exception("Unknown address range:" + rangeS[r]);
                            if (HexToUint(range[0],out block.Start) == false) throw new Exception("Unknown HEX code:" + range[0]);
                            if (HexToUint(range[1], out block.End) == false) throw new Exception("Unknown HEX code:" + range[1]);
                            addrList.Add(block);
                        }
                    }
                    else if (ts.Segments != null && ts.Segments.Length > 0)
                    {
                        string[] segStrings = ts.Segments.Split(',');
                        for (int y=0; y< segStrings.Length; y++)
                        {
                            int segNr = 0;
                            segNr = PCM.GetSegmentByNr(segStrings[y]);
                            for (int b=0; b< PCM.segmentAddressDatas[segNr].SegmentBlocks.Count; b++)
                                addrList.Add(PCM.segmentAddressDatas[segNr].SegmentBlocks[b]);
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
                    string[] hParts = ts.UseHit.Split(',');
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
                        while (startAddr < PCM.fsize && wHit < wantedHitList.Count) 
                        {
                            wantedHit = wantedHitList[wHit];
                            char splitChr = '+';
                            if (!ts.SearchStr.Contains("+") && ts.SearchStr.Contains("-"))
                            {
                                splitChr = '-';
                            }
                            string[] ssParts = ts.SearchStr.Split(splitChr);     //At end of string can be +D4 +1W6 etc, for reading next address from found addr
                            Debug.WriteLine("TableSeek: Searching: " + ts.SearchStr + ", Start: " + startAddr.ToString("X") + ", end: " + endAddr.ToString("X"));                            
                            sAddr = SearchAddrBySearchString(PCM, ssParts[0], ref startAddr, endAddr, ts);
                            for (int jump = 1; jump < ssParts.Length && (sAddr.Addr + ts.Offset) < PCM.fsize; jump++)
                            {
                                //Read table address from address we found by searchstring
                                string numOnly = ssParts[jump].Replace("+", "").Replace("D", "").Replace("W", "");
                                int offset = Convert.ToInt32(numOnly);  //For first jump, use tableseek offset, for other jumps use searchstring offset
                                if (splitChr == '-')
                                {
                                    offset = offset * -1;
                                }
                                uint currentAddr = (uint)(sAddr.Addr + offset);
                                Debug.WriteLine("seekTables: Reading new address from:" + currentAddr.ToString("X"));
                                if (ssParts[jump].Contains("D"))
                                    sAddr.Addr = (uint)(PCM.ReadUInt32(currentAddr));
                                else
                                    sAddr.Addr = (uint)(PCM.ReadUInt16(currentAddr));
                                Debug.WriteLine("seekTables: New address:" + sAddr.Addr.ToString("X"));
                            }

                            if ((sAddr.Addr + ts.Offset) < PCM.fsize)
                            {
                                hit++;
                                Debug.WriteLine("Found: " + sAddr.Addr.ToString("X") + ", Hit: " + hit.ToString() + " of " + wantedHit);
                            }
                            if (hit == wantedHit && (sAddr.Addr + ts.Offset) < PCM.fsize)
                            {
                                FoundTable ft = new FoundTable(PCM);
                                ft.configId = s;
                                ft.Name = ts.Name.Replace("£", (wHit +1).ToString());
                                ft.Description = ts.Description.Replace("£", (wHit + 1).ToString());
                                ft.addrInt = (uint)(sAddr.Addr + ts.Offset);
                                if (ts.Category != null && ts.Category != "")
                                {
                                    ft.Category = ts.Category;
                                }
                                else
                                {
                                    ft.Category = "Seg-" + PCM.GetSegmentName(ft.addrInt);
                                }
                                ft.Address = ft.addrInt.ToString("X8");
                                if (ts.Rows > 0)
                                    ft.Rows = ts.Rows;
                                else
                                    ft.Rows = sAddr.Rows;
                                if (ts.Columns > 0)
                                    ft.Columns = ts.Columns;
                                else
                                    ft.Columns = sAddr.Columns;
                                PCM.foundTables.Add(ft);
                                wHit++;
                            }
                        }
                    }

                }
                Debug.WriteLine("Applying search for next XX tables");
                for (int seekId = 0; seekId < tableSeeks.Count; seekId++)
                {
                    TableSeek ts1 = tableSeeks[seekId];
                    string applyStr = ts1.ApplySearchForCount;
                    if (!string.IsNullOrEmpty(ts1.ApplySearchForCount) && ts1.ApplySearchForCount != "0")
                    {
                        FoundTable ft = PCM.foundTables.Where(X => X.configId == seekId).FirstOrDefault();
                        if (ft == null)
                        {
                            Debug.WriteLine("Error, Table not found from FoundTables: " + ts1.Name);
                            continue;
                        }
                        Debug.WriteLine("Table: " + ts1.Name + ", count: " + ts1.ApplySearchForCount);
                        int applyStart = seekId;
                        int applyEnd = seekId;
                        bool neg = false;
                        if (applyStr.StartsWith("-"))
                        {
                            applyStr = applyStr.Trim('-');
                            neg = true;
                        }
                        if (applyStr.Contains("-"))
                        {
                            //Range. Example -3-5, tables s-3 ... s+5
                            string[] aParts = applyStr.Split('-');
                            if (aParts.Length == 2 && int.TryParse(aParts[0], out int s1) && int.TryParse(aParts[1], out int s2))
                            {
                                applyStart = seekId - s1;
                                applyEnd = seekId + s2;
                                if (neg)
                                {
                                    applyStart = -1 * applyStart;
                                }
                            }
                        }
                        else
                        {
                            //Example: 5
                            //Example: -3
                            if (int.TryParse(ts1.ApplySearchForCount, out int a1))
                            {
                                if (a1 < 0)
                                {
                                    applyStart = seekId + a1;
                                    applyEnd = seekId - 1;
                                }
                                else
                                {
                                    applyStart = seekId + 1;
                                    applyEnd = seekId + a1;
                                }
                            }
                        }
                        if (applyStart < 0)
                        {
                            Debug.WriteLine("Can't start from " + applyStart.ToString() + ", starting from 0");
                            applyStart = 0;
                        }
                        Debug.WriteLine("Applying to tableseeks: " + applyStart.ToString()  + " - " + applyEnd.ToString());

                        for (int applyId = applyStart; applyId <= applyEnd && applyId < tableSeeks.Count; applyId++)
                        {
                            if (applyId == seekId)
                            {
                                Debug.WriteLine("Skpping original tableseek");
                                continue;
                            }
                            TableSeek ts2 = tableSeeks[applyId];
                            if (HexToUint(ts1.RefAddress, out uint ref1) && HexToUint(ts2.RefAddress, out uint ref2))
                            {
                                int diff = (int)(ref2 - ref1);
                                FoundTable ft2 = new FoundTable(PCM);
                                ft2.configId = applyId;
                                ft2.addrInt = (uint)(ft.addrInt + diff);
                                ft2.Name = ts2.Name;
                                ft2.Description = ts2.Description;
                                if (!string.IsNullOrEmpty(ts2.Category))
                                {
                                    ft2.Category = ts2.Category;
                                }
                                else
                                {
                                    ft2.Category = "Seg-" + PCM.GetSegmentName(ft2.addrInt);
                                }
                                ft2.Address = ft2.addrInt.ToString("X8");
                                if (ts2.Rows > 0)
                                    ft2.Rows = ts2.Rows;
                                else
                                    ft2.Rows = ft.Rows;
                                if (ts2.Columns > 0)
                                    ft2.Columns = ts2.Columns;
                                else
                                    ft2.Columns = ft.Columns;

                                PCM.foundTables.Add(ft2);
                                Debug.WriteLine("Added table: " + ts2.Name + ", Address: " + ft2.Address);
                            }
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
        public FoundTable(PcmFile PCM)
        {
            Name = "";
            addrInt = uint.MaxValue;
            //Address = "";
            configId = -1;
            Description = "";
            id = PCM.foundTables.Count;
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
        public ushort Rows { get; set; }
        public ushort Columns { get; set; }
        public int configId;
        public string Category { get; set; }
        public string Description { get; set; }
        public FoundTable ShallowCopy()
        {
            FoundTable newFt = (FoundTable)this.MemberwiseClone();
            return newFt;
        }

    }


}
