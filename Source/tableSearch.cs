using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using static upatcher;
using System.Windows.Forms;

namespace UniversalPatcher
{
    public class TableSearchConfig
    {
        public TableSearchConfig()
        {
            //ID = "";
            searchData = "";
            Items = "";
            Name = "";
            searchRange = "";
            tableRange = "";
            //start = "";
            //distanceMin = 0;
            //distanceMax = int.MaxValue;
            //location = 0;
        }

        public class ParsedTableSearchConfig
        {
            public ParsedTableSearchConfig()
            {
                searchParts = new List<string>();
                searchValues = new List<int>();
                searchString = "";
            }
            public void addPart(string part)
            {
                searchString += part + " ";
                searchParts.Add(part);
                int partVal;
                if (part == "*" || part.ToLower() == "x" || !HexToInt(part, out partVal))
                {
                    searchValues.Add(-1);
                }
                else
                {
                    searchValues.Add(partVal);
                }
            }
            public List<string> searchParts;
            public List<int> searchValues;
            public string searchString;
        }
        public void parseAddresses(PcmFile PCM)
        {
            ParseAddress(searchRange, PCM, out searchBlocks);
            ParseAddress(tableRange, PCM, out tableBlocks);

        }
        //public string ID { get; set; }
        public string searchData { get; set; }
        public string Items { get; set; }
        public string Name { get; set; }
        public string searchRange { get; set; }
        public string tableRange { get; set; }

        public List<Block> searchBlocks;
        public List<Block> tableBlocks;

        //public string start { get; set; }
        //public int distanceMin { get; set; }
        //public int distanceMax { get; set; }
        //public int location { get; set; }

        private bool ParseAddress(string Line, PcmFile PCM, out List<Block> Blocks)
        {
            Debug.WriteLine("Segment address line: " + Line);
            Blocks = new List<Block>();

            if (Line == null || Line == "")
            {
                //It is ok to have empty address (for CS, not for segment)
                Block B = new Block();
                B.End = PCM.fsize;
                B.Start = 0;
                Blocks.Add(B);
                return true;
            }

            string[] Parts = Line.Split(',');
            int i = 0;

            foreach (string Part in Parts)
            {
                string[] StartEnd = Part.Split('-');
                Block B = new Block();
                int Offset = 0;

                if (StartEnd[0].Contains(">"))
                {
                    string[] SO = StartEnd[0].Split('>');
                    StartEnd[0] = SO[0];
                    uint x;
                    if (!HexToUint(SO[1], out x))
                        throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                    Offset = (int)x;
                }
                if (StartEnd[0].Contains("<"))
                {
                    string[] SO = StartEnd[0].Split('<');
                    StartEnd[0] = SO[0];
                    uint x;
                    if (!HexToUint(SO[1], out x))
                        throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                    Offset = ~(int)x;
                }


                if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
                {
                    throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")");
                }
                if (StartEnd[0].StartsWith("@"))
                {
                    uint tmpStart = B.Start;
                    B.Start = BEToUint32(PCM.buf, tmpStart);
                    B.End = BEToUint32(PCM.buf, tmpStart + 4);
                    tmpStart += 8;
                }
                else
                {
                    if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                        throw new Exception("Can't decode from HEX: " + StartEnd[1].Replace("@", "") + " (" + Line + ")");
                    if (B.End >= PCM.buf.Length)    //Make 1MB config work with 512kB bin
                        B.End = (uint)PCM.buf.Length - 1;
                }
                if (StartEnd.Length > 1 && StartEnd[1].StartsWith("@"))
                {
                    //Read End address from bin at this address
                    B.End = BEToUint32(PCM.buf, B.End);
                }
                if (StartEnd.Length > 1 && StartEnd[1].EndsWith("@"))
                {
                    //Address is relative to end of bin
                    uint end;
                    if (HexToUint(StartEnd[1].Replace("@", ""), out end))
                        B.End = (uint)PCM.buf.Length - end - 1;
                }
                B.Start += (uint)Offset;
                B.End += (uint)Offset;
                Blocks.Add(B);
                i++;
            }
            foreach (Block B in Blocks)
                Debug.WriteLine("Address block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
            return true;
        }


    }
    public class TableSearchResult
    {
        public TableSearchResult()
        {
            hitCount = 1;
        }
        public string OS { get; set; }
        public string File { get; set; }
        public string Name { get; set; }
        public string Segment { get; set; }
        public int hitCount { get; set; }
        public string Search { get; set; }
        public string Found { get; set; }
        public uint AddressInt;
        public string Address { get; set; }
        public string Data { get; set; }
    }

    public class SearchVariable
    {
        public SearchVariable()
        {
            Name = "";
            Data = "";
        }
        public string Name { get; set; }
        public string Data { get; set; }
    }

    public class TableFinder
    {

        public TableFinder()
        {

        }
        private List<SearchVariable> searchVariables = new List<SearchVariable>(); //"Global" list
        private List<TableSearchConfig.ParsedTableSearchConfig> parsedConfigList = new List<TableSearchConfig.ParsedTableSearchConfig>();
        private int parseTableSearchString(string searchString, int start, TableSearchConfig.ParsedTableSearchConfig prevConfig)
        {
            int commonParts = 0;    //How many parts from beginning of searchstring, before first range etc 
            string[] search1Parts = searchString.Trim().Split(' ');
            TableSearchConfig.ParsedTableSearchConfig parsedConfig = new TableSearchConfig.ParsedTableSearchConfig();

            try
            {

                uint bytecount = 0;

                parsedConfig.searchString = prevConfig.searchString;
                for (int v = 0; v < prevConfig.searchParts.Count; v++)
                {
                    //add already handled part to string
                    parsedConfig.searchParts.Add(prevConfig.searchParts[v]);
                    parsedConfig.searchValues.Add(prevConfig.searchValues[v]);
                }

                for (int pos = start; pos < search1Parts.Length; pos++)
                {
                    if (search1Parts[pos].StartsWith("$"))
                    {
                        //is variable
                        for (int var = 0; var < searchVariables.Count; var++)
                        {
                            if (searchVariables[var].Name == search1Parts[pos].Replace("$", ""))
                            {
                                //replace variable text with variable value
                                string[] varParts = searchVariables[var].Data.Split(' ');
                                for (int vp = 0; vp < varParts.Length; vp++)
                                {
                                    parsedConfig.addPart(varParts[vp]);
                                }
                                bytecount += (uint)varParts.Length;
                                break;
                            }
                        }

                    }
                    else if (search1Parts[pos].Contains(":"))
                    {
                        //Have item
                        string[] varParts = search1Parts[pos].Split(':');
                        if (varParts.Length == 2)
                        {
                            byte varSize = 0;
                            if (!HexToByte(varParts[1], out varSize))
                                throw new Exception("Variable size missing! Example: variable1:4");
                            parsedConfig.addPart(varParts[1]);
                            bytecount++;
                            for (uint p = 1; p < varSize; p++)
                            {
                                parsedConfig.addPart("*");
                                bytecount++;
                            }
                        }
                        else
                        {
                            byte itemSize = 0;
                            if (!HexToByte(varParts[1], out itemSize))
                                throw new Exception("Item size missing! Example: rows:4:int");
                            //uint position = bytecount;
                            parsedConfig.addPart(search1Parts[pos]);
                            for (int x = 1; x < itemSize; x++)
                            {
                                parsedConfig.addPart("*");
                                bytecount++;
                            }
                        }
                    }
                    else if (search1Parts[pos].Contains("-")) //Range
                    {
                        //Create all possible combinations for this range
                        if (commonParts == 0)
                            commonParts = pos;
                        string part = search1Parts[pos];
                        string[] parts = part.Split('-');
                        int from = int.Parse(parts[0]);
                        int to = int.Parse(parts[1]);
                        for (int wildcards = from; wildcards < to; wildcards++)
                        {
                            string tmpString = parsedConfig.searchString;
                            TableSearchConfig.ParsedTableSearchConfig tmpConfig = new TableSearchConfig.ParsedTableSearchConfig();
                            tmpConfig.searchString = parsedConfig.searchString;
                            for (int w = 0; w < parsedConfig.searchParts.Count; w++)
                            {
                                tmpConfig.searchParts.Add(parsedConfig.searchParts[w]);
                                tmpConfig.searchValues.Add(parsedConfig.searchValues[w]);
                            }

                            for (int wildcard = 0; wildcard < wildcards; wildcard++)
                            {
                                tmpString += "* ";
                                tmpConfig.addPart("*");
                                //tmpConfigList.addPart("*");
                            }
                            //Add rest of searchstring:
                            for (int s = pos + 1; s < search1Parts.Length; s++)
                            {
                                tmpString += search1Parts[s] + " ";
                                //tmpConfigList.addPart(search1Parts[s]);
                            }
                            //Parse all (not last possible) combinations and add to list:
                            parseTableSearchString(tmpString, pos + wildcards + 1, tmpConfig);
                        }
                        //Add last combination to current searchstring
                        //newSearchString += "* ";
                        for (int f = 0; f < to; f++)
                        {
                            parsedConfig.addPart("*");
                            bytecount++;
                        }
                    }
                    else if (search1Parts[pos].Contains("/")) //For example: 3/6/9 = 3 or 6 or 9 bytes
                    {
                        if (commonParts == 0)
                            commonParts = pos;
                        //List<string> tmpList;
                        string[] parts = search1Parts[pos].Split('/');
                        for (int c = 0; c < parts.Length; c++)
                        {
                            int count = int.Parse(parts[c]);
                            //Generate all combinations
                            if (c < parts.Length - 1)
                            {
                                string tmpString = parsedConfig.searchString;
                                TableSearchConfig.ParsedTableSearchConfig tmpConfig = new TableSearchConfig.ParsedTableSearchConfig();
                                tmpConfig.searchString = parsedConfig.searchString;
                                for (int w = 0; w < parsedConfig.searchParts.Count; w++)
                                {
                                    tmpConfig.searchParts.Add(parsedConfig.searchParts[w]);
                                    tmpConfig.searchValues.Add(parsedConfig.searchValues[w]);
                                }
                                for (int wildcards = 0; wildcards < count; wildcards++)
                                {
                                    //Add reuired wildcards
                                    tmpConfig.addPart("*");
                                    tmpString += "* ";
                                }
                                for (int s = pos + 1; s < search1Parts.Length; s++)
                                {
                                    //Add rest of searchstring:
                                    tmpString += search1Parts[s] + " ";
                                }
                                tmpString = tmpString.Trim();
                                //Parse all (not last possible) combinations and add to list:
                                parseTableSearchString(tmpString, pos + count + 1, tmpConfig);
                            }
                            else
                            {
                                //Add last combination to current searchstring
                                for (uint cc = 0; cc < count; cc++)
                                {
                                    //newSearchString += "* ";
                                    parsedConfig.addPart("*");
                                    bytecount++;
                                }
                                //Add rest of searchstring:
                                for (int s = pos + 1; s < search1Parts.Length; s++)
                                {
                                    //newSearchString += search1Parts[s] + " ";
                                    parsedConfig.addPart(search1Parts[s]);
                                    bytecount++;
                                }
                            }
                        }
                    }
                    else
                    {
                        //searchPartList.Add(search1Parts[v]);
                        if (search1Parts[pos] != " ")
                        {
                            //newSearchString += search1Parts[v] + " ";
                            parsedConfig.addPart(search1Parts[pos]);
                            bytecount++;
                        }
                    }
                }
                //searchStrings.Add(newSearchString.Trim());
                parsedConfigList.Add(parsedConfig);
                Debug.WriteLine("Added searchstring: " + parsedConfig.searchString);
                if (parsedConfigList.Count == 1)   //If only one string, no need to check other strings
                    commonParts = (int)bytecount;
                return commonParts;
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("ParseTablesearch: " + line + ": " + ex.Message);
                return -1;
            }
        }

        public void searchTables(string FileName, PcmFile PCM)
        {

            try
            {
                if (tableSearchResult == null)
                    tableSearchResult = new List<TableSearchResult>();
                if (tableSearchResultNoFilters == null)
                    tableSearchResultNoFilters = new List<TableSearchResult>();
                searchVariables = new List<SearchVariable>();
                List<TableSearchResult> thisFileTables = new List<TableSearchResult>();

                string searchXMLFile = Path.Combine(Application.StartupPath, "XML", "SearchTables-" + Path.GetFileName(XMLFile));

                if (File.Exists(searchXMLFile))
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSearchConfig>));
                    System.IO.StreamReader file = new System.IO.StreamReader(searchXMLFile);
                    List<TableSearchConfig> tableSearchConfig = (List<TableSearchConfig>)reader.Deserialize(file);
                    file.Close();
                    tableSearchFile = searchXMLFile;
                    TableSearchResult tsr;
                    for (int i = 0; i < tableSearchConfig.Count; i++)
                    {
                        string searchTxt = tableSearchConfig[i].searchData.Replace("[", "");
                        searchTxt = searchTxt.Replace("]", "");
                        //List<string> searchStrings;
                        //List<string[]> searchPartList = new List<string[]>();

                        parsedConfigList = new List<TableSearchConfig.ParsedTableSearchConfig>();
                        TableSearchConfig.ParsedTableSearchConfig parsedConfig = new TableSearchConfig.ParsedTableSearchConfig();

                        TableSearchConfig tsc = tableSearchConfig[i];
                        tsc.parseAddresses(PCM);

                        Debug.WriteLine("Original searchstring: " + searchTxt);
                        int commonParts = parseTableSearchString(searchTxt, 0, parsedConfig);


                        for (int block = 0; block < tsc.searchBlocks.Count; block++)
                        {
                            uint addr = tsc.searchBlocks[block].Start;
                            while (addr < tsc.searchBlocks[block].End)
                            {
                                bool match = false;
                                for (int ss = 0; ss < parsedConfigList.Count; ss++)
                                {
                                    int hits = 0;
                                    for (int j = 0; j < parsedConfigList[ss].searchParts.Count; j++)
                                    {
                                        if (parsedConfigList[ss].searchValues[j] == -1)
                                        {
                                            //Count as hit (wildcard, ignore)
                                            hits++;
                                        }
                                        else
                                        {
                                            if (PCM.buf[addr + j] == parsedConfigList[ss].searchValues[j])
                                            {
                                                hits++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (hits == parsedConfigList[ss].searchValues.Count)
                                    {
                                        match = true;
                                        searchTxt = parsedConfigList[ss].searchString;
                                        parsedConfig = parsedConfigList[ss];
                                        break;
                                    }
                                    if (hits < parsedConfigList[ss].searchValues.Count && hits < commonParts)
                                    {
                                        match = false;
                                        break;
                                    }
                                }
                                if (match)
                                {

                                    tsr = new TableSearchResult();
                                    tsr.OS = PCM.OS;
                                    tsr.File = PCM.FileName;
                                    tsr.Search = searchTxt;
                                    tsr.Name = tableSearchConfig[i].Name;
                                    tsr.Found = addr.ToString("X8") + ":";
                                    for (uint t = 0; t < parsedConfig.searchParts.Count; t++)
                                    {
                                        if (t > 0)
                                            tsr.Found += " ";
                                        tsr.Found += PCM.buf[addr + t].ToString("X2");
                                    }
                                    if (tableSearchConfig[i].Items.Length > 0)
                                    {
                                        string[] items = tableSearchConfig[i].Items.Split(',');
                                        for (int p = 0; p < items.Length; p++)
                                        {
                                            string[] itemParts = items[p].Split(':');
                                            uint location = (uint)(addr + int.Parse(itemParts[1]));
                                            if (itemParts.Length == 3)
                                            {
                                                SearchVariable SV = new SearchVariable();
                                                SV.Name = itemParts[0];
                                                if (itemParts[2].ToLower() == "e")
                                                    location += (uint)parsedConfig.searchParts.Count;
                                                tsr.AddressInt = location;
                                                tsr.Data = SV.Name;
                                                SV.Data = ((location & 0xFF000000) >> 24).ToString("X2") + " " + ((location & 0xFF0000) >> 16).ToString("X2") + " " + ((location & 0xFF00) >> 8).ToString("X2") + " " + (location & 0xFF).ToString("X2");
                                                searchVariables.Add(SV);
                                            }
                                            else
                                            {
                                                if (itemParts.Length != 4)
                                                    throw new Exception("item needs 4 parts: name:location:bytes:type, for example: rows:1:2:int");

                                                if (itemParts[0].StartsWith("@"))
                                                {
                                                    tsr.AddressInt = BEToUint32(PCM.buf, location);
                                                }
                                                if (tsr.Data != null && tsr.Data.Length > 1)
                                                    tsr.Data += "; ";
                                                tsr.Data += itemParts[0] + ":";   //Item name

                                                string formatString = "";
                                                if (itemParts[3].ToLower() == "hex")
                                                    formatString = "X";
                                                if (itemParts[2] == "1")
                                                    tsr.Data += PCM.buf[location].ToString(formatString);
                                                if (itemParts[2] == "2")
                                                    tsr.Data += BEToUint16(PCM.buf, location).ToString(formatString);
                                                if (itemParts[2] == "4")
                                                    tsr.Data += BEToUint32(PCM.buf, location).ToString(formatString);
                                                if (itemParts[2] == "8")
                                                    tsr.Data += BEToUint16(PCM.buf, location).ToString(formatString);

                                            }
                                            for (int seg = 0; seg < Segments.Count; seg++)
                                            {
                                                for (int b = 0; b < PCM.binfile[seg].SegmentBlocks.Count; b++)
                                                {
                                                    if (tsr.AddressInt >= PCM.binfile[seg].SegmentBlocks[b].Start && tsr.AddressInt <= PCM.binfile[seg].SegmentBlocks[b].End)
                                                        tsr.Segment = PCM.segmentinfos[seg].Name;
                                                }
                                            }
                                        }
                                    }
                                    if (searchTxt.Contains(":"))
                                    {
                                        int k = 0;
                                        while (k < parsedConfig.searchParts.Count)
                                        {
                                            if (parsedConfig.searchParts[k].Contains(":"))
                                            {
                                                string[] varParts = parsedConfig.searchParts[k].Split(':');
                                                if (varParts.Length == 2)
                                                {
                                                    //Set variable
                                                    SearchVariable SV = new SearchVariable();
                                                    SV.Name = varParts[0];
                                                    tsr.Data += SV.Name;
                                                    int bytes = int.Parse(varParts[1]);
                                                    uint location = (uint)(addr + k);
                                                    tsr.AddressInt = location;
                                                    for (uint l = 0; l < bytes; l++)
                                                    {
                                                        SV.Data += PCM.buf[location + l].ToString("X2") + " ";
                                                    }
                                                    SV.Data = SV.Data.Trim();
                                                    searchVariables.Add(SV);
                                                }
                                                else
                                                {
                                                    //Its's item, show it.
                                                    uint location = (uint)(addr + k);
                                                    if (varParts[0].ToLower().StartsWith("@"))
                                                    {
                                                        tsr.AddressInt = BEToUint32(PCM.buf, location);
                                                    }
                                                    if (tsr.Data != null && tsr.Data.Length > 1)
                                                        tsr.Data += "; ";
                                                    tsr.Data += varParts[0] + ":";
                                                    string formatString = "";
                                                    if (varParts[2].ToLower() == "hex")
                                                        formatString = "X";
                                                    if (varParts[1] == "1")
                                                        tsr.Data += PCM.buf[location].ToString(formatString);
                                                    if (varParts[1] == "2")
                                                        tsr.Data += BEToUint16(PCM.buf, location).ToString(formatString);
                                                    if (varParts[1] == "4")
                                                        tsr.Data += BEToUint32(PCM.buf, location).ToString(formatString);
                                                    if (varParts[1] == "8")
                                                        tsr.Data += BEToUint64(PCM.buf, location).ToString(formatString);


                                                }
                                                for (int seg = 0; seg < Segments.Count; seg++)
                                                {
                                                    for (int b = 0; b < PCM.binfile[seg].SegmentBlocks.Count; b++)
                                                    {
                                                        if (tsr.AddressInt >= PCM.binfile[seg].SegmentBlocks[b].Start && tsr.AddressInt <= PCM.binfile[seg].SegmentBlocks[b].End)
                                                            tsr.Segment = PCM.segmentinfos[seg].Name;
                                                    }
                                                }
                                                k += int.Parse(varParts[1]) - 1;
                                            }
                                            k++;
                                        }
                                    }
                                    TableSearchResult tsrNoFilter = new TableSearchResult();
                                    tsrNoFilter.Address = tsr.Address;
                                    tsrNoFilter.AddressInt = tsr.AddressInt;
                                    tsrNoFilter.Data = tsr.Data;
                                    tsrNoFilter.File = tsr.File;
                                    tsrNoFilter.Found = tsr.Found;
                                    tsrNoFilter.hitCount = 1;
                                    tsrNoFilter.Name = tsr.Name;
                                    tsrNoFilter.OS = tsr.OS;
                                    tsrNoFilter.Search = tsr.Search;
                                    tsrNoFilter.Segment = tsr.Segment;

                                    tableSearchResultNoFilters.Add(tsrNoFilter);
                                    for (int tblock = 0; tblock < tsc.tableBlocks.Count; tblock++)
                                    {
                                        if (tsr.AddressInt >= tsc.tableBlocks[tblock].Start && tsr.AddressInt <= tsc.tableBlocks[tblock].End)
                                        {
                                            bool duplicate = false;
                                            for (int ts = 0; ts < thisFileTables.Count; ts++)
                                            {
                                                if (thisFileTables[ts].Data == tsr.Data && thisFileTables[ts].Name == tsr.Name)
                                                {
                                                    thisFileTables[ts].hitCount++;
                                                    duplicate = true;
                                                    break;
                                                }
                                            }
                                            tsr.Address = tsr.AddressInt.ToString("X8");
                                            if (!duplicate)
                                                thisFileTables.Add(tsr);
                                        }
                                    }
                                }
                                addr++;
                            }

                        }

                    }
                    for (int t = 0; t < thisFileTables.Count; t++)
                    {
                        tableSearchResult.Add(thisFileTables[t]);
                    }
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Tablesearch: " + line + ": " + ex.Message);
            }
        }
    }
}
