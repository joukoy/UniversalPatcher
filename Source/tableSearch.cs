﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using static Upatcher;
using System.Windows.Forms;
using static Helpers;

namespace UniversalPatcher
{
    public class TableSearchConfig
    {
        public TableSearchConfig()
        {
            //ID = "";
            searchData = "";
            Items = "";
            Variables = "";
            Name = "";
            searchRange = "";
            tableRange = "";
            //start = "";
            //distanceMin = 0;
            //distanceMax = int.MaxValue;
            //location = 0;
        }
        //public string ID { get; set; }
        public string searchData { get; set; }
        public string Items { get; set; }
        public string Variables { get; set; }
        public string Name { get; set; }
        public string searchRange { get; set; }
        public string tableRange { get; set; }

        public List<Block> searchBlocks;
        public List<Block> tableBlocks;

        //public string start { get; set; }
        //public int distanceMin { get; set; }
        //public int distanceMax { get; set; }
        //public int location { get; set; }

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

        public void ParseAddresses(PcmFile PCM)
        {
            ParseAddress(searchRange, PCM, out searchBlocks);
            ParseAddress(tableRange, PCM, out tableBlocks);
        }

        public List<SearchVariable> ParseVariables(PcmFile PCM)
        {
            List<SearchVariable> searchVariables = new List<SearchVariable>();
            string[] varlist = Variables.Split(',');
            for (int v = 0; v < varlist.Length; v++)
            {
                string[] varParts = varlist[v].Split('=');
                if (varParts.Length < 2)
                {
                    Debug.WriteLine("Unknown variable: " + varlist[v]);
                }
                else
                {
                    if (varParts[1].StartsWith("@"))
                    {
                        SearchVariable SV = new SearchVariable();
                        SV.Name = varParts[0].Trim();
                        uint location;
                        int bytes;
                        string[] svDataParts = varParts[1].Split(':');

                        if (!HexToUint(svDataParts[0].Replace("@", ""), out location))
                            throw new Exception("Can't decode variable location: " + varlist[v]);
                        if (!HexToInt(svDataParts[1].Replace("@", ""), out bytes))
                            throw new Exception("Can't decode variable size: " + varlist[v]);

                        for (int i = 0; i < bytes; i++)
                        {
                            SV.Data += PCM.buf[location + i].ToString("X2") + " ";
                        }
                        SV.Data = SV.Data.Trim();
                        searchVariables.Add(SV);
                        Debug.WriteLine("New variable: " + SV.Name + "=" + SV.Data);
                    }
                    else
                    {
                        string[] svDataParts = varParts[1].Split(';');
                        foreach (string svDataPart in svDataParts)
                        {
                            SearchVariable SV = new SearchVariable();
                            SV.Name = varParts[0].Trim(); ;
                            SV.Data = svDataPart.Trim();
                            searchVariables.Add(SV);
                            Debug.WriteLine("New variable: " + SV.Name + "=" + SV.Data);
                        }
                    }
                }
            }
            return searchVariables;
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
        public string Category { get; set; }
        public string Label { get; set; }
        public string Data { get; set; }
        public uint Rows { get; set; }
        public uint Size { get; set; }
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

            private int parseTableSearchString(string searchString, TableSearchConfig.ParsedTableSearchConfig parsedConfig)
            {
                int commonParts = 0;    //How many parts from beginning of searchstring, before first range etc 
                string[] search1Parts = searchString.Trim().Split(' ');

                try
                {
                    for (int pos = parsedConfig.searchParts.Count; pos < search1Parts.Length; pos++)
                    {
                        if (search1Parts[pos].StartsWith("$"))
                        {
                            List<int> varAddresses = new List<int>();
                            //is variable
                            for (int var = 0; var < searchVariables.Count; var++)
                            {
                                if (searchVariables[var].Name == search1Parts[pos].Replace("$", ""))
                                {
                                    varAddresses.Add(var);
                                }
                            }
                            for (int v = 0; v < varAddresses.Count; v++)
                            {
                                int var = varAddresses[v];
                                //replace variable with variable value
                                string[] varParts = searchVariables[var].Data.Split(' ');
                                if (var < varAddresses.Count - 1)
                                {
                                    TableSearchConfig.ParsedTableSearchConfig tmpConfig = new TableSearchConfig.ParsedTableSearchConfig();
                                    tmpConfig.searchString = parsedConfig.searchString;
                                    string tmpString = parsedConfig.searchString + searchVariables[var].Data + " ";
                                    for (int w = 0; w < parsedConfig.searchParts.Count; w++)
                                    {
                                        //Copy parsed data to tmpConfig
                                        tmpConfig.searchParts.Add(parsedConfig.searchParts[w]);
                                        tmpConfig.searchValues.Add(parsedConfig.searchValues[w]);
                                    }
                                    for (int vp = 0; vp < varParts.Length; vp++)
                                    {
                                        //Copy variable values to tmpConfig
                                        tmpConfig.addPart(varParts[vp]);
                                    }
                                    for (int s = pos + 1; s < search1Parts.Length; s++)
                                    {
                                        //Add rest of searchstring:
                                        tmpString += search1Parts[s] + " ";
                                    }
                                    //Parse all other combinations and add to list:
                                    parseTableSearchString(tmpString, tmpConfig);
                                }
                                else
                                {
                                    //Last combination added to current searchstring
                                    for (int vp = 0; vp < varParts.Length; vp++)
                                    {
                                        parsedConfig.addPart(varParts[vp]);
                                    }
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
                                for (uint p = 1; p < varSize; p++)
                                {
                                    parsedConfig.addPart("*");
                                }
                            }
                            else
                            {
                                byte itemSize = 0;
                                if (!HexToByte(varParts[1], out itemSize))
                                    throw new Exception("Item size missing! Example: rows:4:int");
                                parsedConfig.addPart(search1Parts[pos]);
                                for (int x = 1; x < itemSize; x++)
                                {
                                    parsedConfig.addPart("*");
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
                                parseTableSearchString(tmpString, tmpConfig);
                            }
                            //Add last combination to current searchstring
                            //newSearchString += "* ";
                            for (int f = 0; f < to; f++)
                            {
                                parsedConfig.addPart("*");
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
                                    parseTableSearchString(tmpString, tmpConfig);
                                }
                                else
                                {
                                    //Add last combination to current searchstring
                                    for (uint cc = 0; cc < count; cc++)
                                    {
                                        //newSearchString += "* ";
                                        parsedConfig.addPart("*");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (search1Parts[pos] != " ")
                            {
                                parsedConfig.addPart(search1Parts[pos]);
                            }
                        }
                    }
                    parsedConfigList.Add(parsedConfig);
                    Debug.WriteLine("Added searchstring: " + parsedConfig.searchString);
                    if (parsedConfigList.Count == 1)   //If only one string, no need to check other strings
                        commonParts = parsedConfig.searchParts.Count;
                    return commonParts;
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    Debug.WriteLine("ParseTablesearch: " + line + ": " + ex.Message);
                    return -1;
                }
            }
        

            private TableSearchResult ParseMatch(PcmFile PCM, TableSearchConfig.ParsedTableSearchConfig parsedConfig, TableSearchConfig tableSearchConfig, uint addr)
            {
                TableSearchResult tsr = new TableSearchResult();
                tsr.OS = PCM.OS;
                tsr.File = PCM.FileName;
                tsr.Search = parsedConfig.searchString;
                tsr.Name = tableSearchConfig.Name;
                tsr.Found = addr.ToString("X8") + ":";
                for (uint t = 0; t < parsedConfig.searchParts.Count; t++)
                {
                    if (t > 0)
                        tsr.Found += " ";
                    tsr.Found += PCM.buf[addr + t].ToString("X2");
                }
                if (tableSearchConfig.Items.Length > 0)
                {
                    string[] items = tableSearchConfig.Items.Split(',');
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
                                tsr.AddressInt = PCM.ReadUInt32(location);
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
                                tsr.Data += PCM.ReadUInt16(location).ToString(formatString);
                            if (itemParts[2] == "4")
                                tsr.Data += PCM.ReadUInt32(location).ToString(formatString);
                            if (itemParts[2] == "8")
                                tsr.Data += PCM.ReadUInt64(location).ToString(formatString);

                        }
                        tsr.Segment = PCM.GetSegmentName(tsr.AddressInt);
                    }
                }
                if (parsedConfig.searchString.Contains(":"))
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
                                    tsr.AddressInt = PCM.ReadUInt32(location);
                                }
                                if (varParts[0].ToLower().StartsWith("row"))
                                {
                                    tsr.Rows = PCM.buf[location];
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
                                    tsr.Data += PCM.ReadUInt16(location).ToString(formatString);
                                if (varParts[1] == "4")
                                    tsr.Data += PCM.ReadUInt32(location).ToString(formatString);
                                if (varParts[1] == "8")
                                    tsr.Data += PCM.ReadUInt64(location).ToString(formatString);


                            }
                            for (int seg = 0; seg < PCM.Segments.Count; seg++)
                            {
                                if (!PCM.Segments[seg].Missing)
                                {
                                    for (int b = 0; b < PCM.segmentAddressDatas[seg].SegmentBlocks.Count; b++)
                                    {
                                        if (tsr.AddressInt >= PCM.segmentAddressDatas[seg].SegmentBlocks[b].Start && tsr.AddressInt <= PCM.segmentAddressDatas[seg].SegmentBlocks[b].End)
                                            tsr.Segment = PCM.segmentinfos[seg].Name;
                                    }
                                }
                            }
                            k += int.Parse(varParts[1]) - 1;
                        }
                        k++;
                    }
                }
                for (int o = 0; o < PCM.osAddressList.Count; o++)
                {
                    if (PCM.osAddressList[o].address == tsr.AddressInt)
                    {
                        tsr.Category = PCM.osAddressList[o].category;
                        tsr.Label = PCM.osAddressList[o].label;
                        tsr.Size = PCM.osAddressList[o].size;
                        break;
                    }
                }

                TableSearchResult tsrNoFilter = new TableSearchResult();
                tsrNoFilter.AddressInt = tsr.AddressInt;
                tsrNoFilter.Address = tsr.AddressInt.ToString("X8");
                tsrNoFilter.Data = tsr.Data;
                tsrNoFilter.File = tsr.File;
                tsrNoFilter.Found = tsr.Found;
                tsrNoFilter.hitCount = 1;
                tsrNoFilter.Name = tsr.Name;
                tsrNoFilter.OS = tsr.OS;
                tsrNoFilter.Search = tsr.Search;
                tsrNoFilter.Segment = tsr.Segment;
                tsrNoFilter.Rows = tsr.Rows;
                tsrNoFilter.Category = tsr.Category;
                tsrNoFilter.Label = tsr.Label;
                tsrNoFilter.Size = tsr.Size;

                tableSearchResultNoFilters.Add(tsrNoFilter);
                return tsr;

            }
        
        public void SearchTables(PcmFile PCM, bool crossSearch, string CustomSearch = "",int crossVariation=3)
        {

            try
            {
                if (tableSearchResult == null)
                    tableSearchResult = new List<TableSearchResult>();
                if (tableSearchResultNoFilters == null)
                    tableSearchResultNoFilters = new List<TableSearchResult>();
                searchVariables = new List<SearchVariable>();
                TableSearchResult tsr = new TableSearchResult();
                List<TableSearchResult> thisFileTables = new List<TableSearchResult>();
                List<TableSearchConfig> tableSearchConfig;

                if (CustomSearch == "")
                {
                    string searchXMLFile = Path.Combine(Application.StartupPath, "XML", "SearchTables-" + PCM.configFile + ".xml");

                    if (!File.Exists(searchXMLFile))
                        return;

                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSearchConfig>));
                    System.IO.StreamReader file = new System.IO.StreamReader(searchXMLFile);
                    tableSearchConfig = (List<TableSearchConfig>)reader.Deserialize(file);
                    file.Close();
                    tableSearchFile = searchXMLFile;
                }
                else
                {
                    tableSearchConfig = new List<TableSearchConfig>();
                    TableSearchConfig tsc = new TableSearchConfig();
                    tsc.searchData = CustomSearch;
                    tableSearchConfig.Add(tsc);
                }
            
                for (int i = 0; i < tableSearchConfig.Count; i++)
                {                        
                    string searchTxt = tableSearchConfig[i].searchData.Replace("[", "");
                    searchTxt = searchTxt.Replace("]", "");

                    if (searchTxt.StartsWith("//") || searchTxt.Length < 2)
                    {
                        Debug.WriteLine("Skipping disabled line: " + searchTxt);
                    }
                    else
                    {
                        parsedConfigList = new List<TableSearchConfig.ParsedTableSearchConfig>();
                        TableSearchConfig.ParsedTableSearchConfig parsedConfig = new TableSearchConfig.ParsedTableSearchConfig();

                        TableSearchConfig tsc = tableSearchConfig[i];
                        tsc.ParseAddresses(PCM);
                        List <SearchVariable> tmpVariables = tsc.ParseVariables(PCM);
                        for (int var = 0; var < tmpVariables.Count; var++)
                            searchVariables.Add(tmpVariables[var]);
                        Debug.WriteLine("Original searchstring: " + searchTxt);
                        int commonParts = parseTableSearchString(searchTxt, parsedConfig);
                        Debug.WriteLine("Searchstrings generated: " + parsedConfigList.Count);

                        for (int block = 0; block < tsc.searchBlocks.Count; block++)
                        {
                            uint addr = tsc.searchBlocks[block].Start;
                            while (addr < tsc.searchBlocks[block].End)
                            {
                                int ss = 0;
                                while (ss < parsedConfigList.Count)
                                {
                                    bool match = false;
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
                                    }
                                    if (hits < parsedConfigList[ss].searchValues.Count && hits < commonParts)
                                    {
                                        //Optimization: beginning of all parsed searchstrings until "commonParts" are identical. 
                                        //If this string doesn't match, others in list doesn't match either
                                        match = false;
                                        ss = parsedConfigList.Count;
                                    }
                                    if (match)
                                    {
                                        tsr = ParseMatch(PCM, parsedConfigList[ss], tsc, addr);
                                        for (int tblock = 0; tblock < tsc.tableBlocks.Count; tblock++)
                                        {
                                            if (tsr.AddressInt >= tsc.tableBlocks[tblock].Start && tsr.AddressInt <= tsc.tableBlocks[tblock].End)
                                            {
                                                bool duplicate = false;
                                                for (int ts = 0; ts < thisFileTables.Count; ts++)
                                                {
                                                    //if (thisFileTables[ts].Data == tsr.Data && thisFileTables[ts].Name == tsr.Name && thisFileTables[ts].Search == tsr.Search)
                                                    if (thisFileTables[ts].AddressInt == tsr.AddressInt && thisFileTables[ts].Rows == tsr.Rows)
                                                    {
                                                        thisFileTables[ts].hitCount++;
                                                        duplicate = true;
                                                        //thisFileTables[ts].Address += ";" + tsr.AddressInt.ToString("X8");
                                                        thisFileTables[ts].Found += ";" + tsr.Found;
                                                        break;
                                                    }
                                                }

                                                if (!duplicate)
                                                {
                                                    tsr.Address = tsr.AddressInt.ToString("X8");
                                                    thisFileTables.Add(tsr);
                                                }
                                            }
                                        }
                                    }
                                    ss++;
                                }
                                addr++;
                            }

                        }
                    }


                }

                if (crossSearch)
                {
                    CrossSearchTables(thisFileTables,PCM, crossVariation);
                }
                else
                {
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
                var frame = st.GetFrame(st.FrameCount-1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Tablesearch: " + line + ": " + ex.Message);
            }
        }

        private void CrossSearchTables(List<TableSearchResult> thisFileTables, PcmFile PCM, int crossVariation)
        {
            try
            {
                bool[] usedRow = new bool[thisFileTables.Count];
                for (int u = 0; u < usedRow.Length; u++)
                    usedRow[u] = false;
                int t = 0;
                int lastHit = 0;
                int missCount = 0;
                int t3 = 0;

                for (int pos = 0; pos < tableSearchResult.Count; pos++)
                {
                    for (int pos2 = pos + 1; pos2 < tableSearchResult.Count; pos2++)
                    {
                        if (tableSearchResult[pos].AddressInt > tableSearchResult[pos2].AddressInt)
                        {
                            //Oraganize by Address:
                            TableSearchResult tmpTsr = tableSearchResult[pos];
                            tableSearchResult[pos] = tableSearchResult[pos2];
                            tableSearchResult[pos2] = tmpTsr;
                        }
                    }
                }

                for (int pos = 0; pos < thisFileTables.Count; pos++)
                {
                    for (int pos2 = pos + 1; pos2 < thisFileTables.Count; pos2++)
                    {
                        if (thisFileTables[pos].AddressInt > thisFileTables[pos2].AddressInt)
                        {
                            //Oraganize by Address:
                            TableSearchResult tmpTsr = thisFileTables[pos];
                            thisFileTables[pos] = thisFileTables[pos2];
                            thisFileTables[pos2] = tmpTsr;
                        }
                    }
                }

                while (t < tableSearchResult.Count)
                {
                    bool hit = false;
                    int start = t3 - missCount - crossVariation;
                    if (start < 0)
                        start = 0;
                    for (int maxOffset = 0; maxOffset < 100000 && !hit; maxOffset++)
                    {
                        for (int t2 = start; t2 < (t3 + crossVariation) && t2 < thisFileTables.Count; t2++)
                        {
                            if (!usedRow[t2])
                            {
                                uint offset = (uint)Math.Abs(tableSearchResult[t].AddressInt - thisFileTables[t2].AddressInt);
                                if (tableSearchResult[t].Name == thisFileTables[t2].Name
                                    && tableSearchResult[t].Segment == thisFileTables[t2].Segment
                                    && tableSearchResult[t].Search == thisFileTables[t2].Search
                                    && tableSearchResult[t].Rows == thisFileTables[t2].Rows
                                    && offset <= maxOffset
                                    )
                                {
                                    tableSearchResult.Insert(t + 1, thisFileTables[t2]);
                                    usedRow[t2] = true;
                                    lastHit = t2;
                                    hit = true;
                                    Debug.WriteLine("BinA: " + t3 + ", BinB: " + t2 + ", Addr offset: " + offset.ToString());
                                    break;
                                }
                            }
                        }
                    }
                    if (!hit)
                    {
                        TableSearchResult crossTSR = new TableSearchResult();
                        crossTSR.OS = PCM.OS;
                        tableSearchResult.Insert(t + 1, crossTSR);
                        missCount++;
                        Debug.WriteLine("BinA: " + t3 + ", BinB: missing");
                    }
                    t += 2;
                    t3++;
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount-1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("crossSearchTables: " + line + ": " + ex.Message);
            }

        }
    }
}
