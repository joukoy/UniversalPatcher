using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using static upatcher;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;

namespace UniversalPatcher
{
    public class PcmFile
    {
        public PcmFile()
        {
            setDefaultValues();
        }
        public PcmFile(string Fname, bool autodetect, string cnfFile)
        {
            try
            {
                setDefaultValues();
                FileName = Fname;
                altTableDatas[0].Name = Fname;
                fsize = (uint)new FileInfo(FileName).Length;
                buf = ReadBin(FileName);
                osStoreAddress = uint.MaxValue;
                if (autodetect)
                {
                    string cfgFile = autoDetect(this);
                    if (cfgFile.Length > 0)
                    {
                        _configFileFullName = Path.Combine(Application.StartupPath, "XML", cfgFile);
                    }
                }
                else
                {
                    _configFileFullName = cnfFile;
                }
                if (configFileFullName.Length > 0)
                {
                    loadConfigFile(configFileFullName);
                    GetSegmentAddresses();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }
        public struct V6Table
        {
            public uint address;
            public ushort rows; 
        }
        public class osAddresses
        {
            public osAddresses()
            {

            }
            public string category { get; set; }
            public string label { get; set; }
            public uint address { get; set; }
            public uint size { get; set; }            
        }

        public class AltTableData
        {
            public List<TableData> tableDatas { get; set; }
            public string Name { get; set; }
        }
        public byte[] buf;
        public string FileName;
        public SegmentAddressData[] segmentAddressDatas;
        public SegmentInfo[] segmentinfos;
        public uint fsize;
        public uint osStoreAddress;
        public string mafAddress;
        public List<V6Table> v6tables;
        public V6Table v6VeTable;
        public List<osAddresses> osAddressList;
        public List<SegmentConfig> Segments;
        public List<FoundTable> foundTables;
        public List<FoundSegment> foundSegments;
        public List<dtcCode> dtcCodes;
        public List<string> tableCategories;
        public List<TableData> tableDatas;
        public bool dtcCombined = false;
        //public TableData selectedTable; //Required for Tuner/Compare
        public int tableDataIndex; //Tuner tabledatalist id
        public List<AltTableData> altTableDatas;
        private int currentTableDatasList;
        private string _configFileFullName;
        public string configFileFullName { get { return _configFileFullName; } }
        public string configFile { get { return Path.GetFileNameWithoutExtension(configFileFullName); } }
        public string tunerFile { get; set; }
        public List<string> tunerFileList { get; set; }
        public bool seekTablesImported;

        public string OS
        {
            get
            {
                for (int s=0; s < Segments.Count; s++)
                { 
                    if (Segments[s].Name == "OS")
                        return segmentinfos[s].PN;
                }
                return "";
            }
        }

        public int OSSegment
        {
            get
            {
                for (int s = 0; s < Segments.Count; s++)
                {
                    if (Segments[s].Name == "OS")
                        return s;
                }
                return int.MaxValue;
            }
        }

        public int diagSegment
        {
            get
            {
                for (int s=0; s<Segments.Count; s++)
                if (Segments[s].Name == "EngineDiag")
                {
                        return s;
                }
                return int.MaxValue;
            }
        }

        public string tableSeekFile
        {
            get
            {
                string cnfFile = configFile;
                if (configFile.Contains("."))
                {
                    int pos = configFile.IndexOf(".");
                    cnfFile = cnfFile.Substring(0, pos);
                }

                string tSeekFile = Path.Combine(Application.StartupPath, "XML", "TableSeek-" + cnfFile + ".xml");

                if (File.Exists(tSeekFile))
                {
                    long conFileSize = new FileInfo(tSeekFile).Length;
                    if (conFileSize < 255)
                    {
                        string compXml = ReadTextFile(tSeekFile).Split(new[] { '\r', '\n' }).FirstOrDefault();
                        tSeekFile = Path.Combine(Application.StartupPath, "XML", compXml);
                        Logger("Using compatible file: " + compXml);
                    }
                }

                return tSeekFile;
            }
        }

        public string segmentSeekFile
        {
            get
            {
                string cnfFile = configFile;
                if (configFile.Contains("."))
                {
                    int pos = configFile.IndexOf(".");
                    cnfFile = cnfFile.Substring(0, pos);
                }

                string ssFile = Path.Combine(Application.StartupPath, "XML", "SegmentSeek-" + cnfFile + ".xml");
                if (File.Exists(ssFile))
                {
                    long conFileSize = new FileInfo(ssFile).Length;
                    if (conFileSize < 255)
                    {
                        string compXml = ReadTextFile(ssFile).Split(new[] { '\r', '\n' }).FirstOrDefault();
                        ssFile = Path.Combine(Application.StartupPath, "XML", compXml);
                        Logger("Using compatible file: " + compXml);
                    }
                }
                return ssFile;

            }
        }

        public void setDefaultValues()
        {            
            _configFileFullName = "";
            Segments = new List<SegmentConfig>();
            dtcCodes = new List<dtcCode>();
            tableDatas = new List<TableData>();
            foundTables = new List<FoundTable>();
            foundSegments = new List<FoundSegment>();
            tableCategories = new List<string>();
            altTableDatas = new List<AltTableData>();
            tunerFileList = new List<string>();
            currentTableDatasList = 0;
            selectTableDatas(0, "");
            seekTablesImported = false;
        }
        public PcmFile ShallowCopy()
        {
            return (PcmFile)this.MemberwiseClone();
        }

        public void reloadBinFile()
        {
            buf = ReadBin(FileName);
        }

        public void importSeekTables()
        {
            try
            {
                if (seekTablesImported)
                    return;
                if (foundTables.Count == 0)
                {
                    TableSeek TS = new TableSeek();
                    Logger("Seeking tables...", false);
                    Logger(TS.seekTables(this));
                }
                Logger("Importing TableSeek tables... ", false);
                for (int i = 0; i < foundTables.Count; i++)
                {
                    TableData tableData = new TableData();
                    tableData.importFoundTable(i, this);
                    tableDatas.Add(tableData);
                }
                if (!tableCategories.Contains("DTC"))
                    tableCategories.Add("DTC");
                //Fix table id's
                for (int i = 0; i < tableDatas.Count; i++)
                    tableDatas[i].id = (uint)i;
                seekTablesImported = true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void importDTC()
        {
            bool haveDTC = false;
            for (int t = 0; t < tableDatas.Count; t++)
            {
                if (tableDatas[t].TableName == "DTC" || tableDatas[t].TableName == "DTC.Codes")
                {
                    haveDTC = true;
                    break;
                }
            }
            if (!haveDTC)
            {
                Logger("Importing DTC codes... ", false);
                TableData tdTmp = new TableData();
                tdTmp.importDTC(this, ref tableDatas);
                Logger(" [OK]");
            }
        }

        public void importTinyTunerDB()
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDBtoTableData(this, tableDatas));
        }

        public void loadTunerConfig()
        {
            if (!Properties.Settings.Default.disableTunerAutoloadSettings)
            {
                string defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", OS + ".xml");
                string compXml = "";
                if (File.Exists(defaultTunerFile))
                {
                    long conFileSize = new FileInfo(defaultTunerFile).Length;
                    if (conFileSize < 255)
                    {
                        compXml = ReadTextFile(defaultTunerFile).Split(new[] { '\r', '\n' }).FirstOrDefault();
                        defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", compXml);
                        Logger("Using compatible file: " + compXml);
                    }
                }
                if (File.Exists(defaultTunerFile))
                {
                    Logger(LoadTableList(defaultTunerFile));
                    importDTC();
                }
                else
                {
                    Logger("File not found: " + defaultTunerFile);
                    importDTC();
                    importSeekTables();
                    if (Segments.Count > 0 && Segments[0].CS1Address.StartsWith("GM-V6"))
                        importTinyTunerDB();
                }
            }

        }

        public string LoadTableList(string fName = "")
        {
            string retVal = "";
            try
            {

                string defName = Path.Combine(Application.StartupPath, "Tuner", OS + ".xml");
                //string defName = PCM.OS + ".xml";
                if (fName == "")
                    fName = SelectFile("Select XML File", "XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
                if (fName.Length == 0)
                    return retVal;
                List<TableData> tmpTableDatas = new List<TableData>();
                long conFileSize = new FileInfo(fName).Length;
                if (conFileSize < 255)
                {
                    string compXml = ReadTextFile(fName);
                    retVal += Path.GetFileName(fName) + " => " + compXml + Environment.NewLine;
                    fName = Path.Combine(Path.GetDirectoryName(fName),compXml);
                }
                retVal += "Loading file: " + fName;
                if (File.Exists(fName))
                {
                    Debug.WriteLine("Loading " + fName + "...");
                    tunerFile = fName;
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    System.IO.StreamReader file = new System.IO.StreamReader(fName);
                    tmpTableDatas = (List<TableData>)reader.Deserialize(file);
                    file.Close();
                }
                for (int t = 0; t < tmpTableDatas.Count; t++)
                {
                    tmpTableDatas[t].Origin = "xml";
                    tableDatas.Add(tmpTableDatas[t]);
                    string category = tableDatas[t].Category;
                    if (!tableCategories.Contains(category))
                        tableCategories.Add(category);
                }
                if (!tableCategories.Contains("DTC"))
                    tableCategories.Add("DTC");

                retVal += " [OK]";
                Application.DoEvents();
                //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, pcmfile pcmfile line " + line + ": " + ex.Message);
                retVal += "Error, pcmfile line " + line + ": " + ex.Message;
            }
            return retVal;
        }

        public void SaveTableList(string fName)
        {
            try
            {
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, tableDatas);
                    stream.Close();
                }
                tunerFile = fName;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, pcmfile line " + line + ": " + ex.Message);
            }

        }

        public void addTableDatas(string name)
        {
            AltTableData tdl = new AltTableData();
            tdl.Name = name;
            tdl.tableDatas = new List<TableData>();
            altTableDatas.Add(tdl);
            tunerFile = "";
            tunerFileList.Add("");
        }

        public void selectTableDatas(int listNumber, string name)
        {
            if (altTableDatas.Count < (listNumber + 1))
            {
                addTableDatas(name);
            }
            if (listNumber != currentTableDatasList)
            {
                //Store old list:
                altTableDatas[currentTableDatasList].tableDatas = tableDatas;
                tunerFileList[currentTableDatasList] = tunerFile;
                //Select new:
                currentTableDatasList = listNumber;
                tableDatas = altTableDatas[currentTableDatasList].tableDatas;
                tunerFile = tunerFileList[listNumber];
            }
        }

        public void loadConfigFile(string fileName)
        {
            try
            {
                Segments.Clear();
                Logger("Loading file: " + Path.GetFileName(fileName), false);
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                Segments = (List<SegmentConfig>)reader.Deserialize(file);
                file.Close();
                _configFileFullName = fileName;
                Logger(" [OK]");
                if (Segments[0].Version == null || Segments[0].Version == "")
                {
                    SegmentConfig S = Segments[0];
                    S.Version = "1";
                    Segments[0] = S;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        public void saveConfigFile(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
                    writer.Serialize(stream, Segments);
                    stream.Close();
                }
                _configFileFullName = fileName;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void saveBin(string fName)
        {
            try
            {
                if (Properties.Settings.Default.DisableAutoFixChecksum)
                    LoggerBold("Warning! Automatic checksum fix is disabled");
                else
                    FixCheckSums();
                WriteBinToFile(fName, buf);
                FileName = fName;
            }
            catch (Exception ex)
            {
                LoggerBold(Environment.NewLine + ex.Message);
            }
        }

        public void saveCS1(int seg, uint CS1Calc)
        {
            if (segmentAddressDatas[seg].CS1Address.Bytes == 1)
                buf[segmentAddressDatas[seg].CS1Address.Address] = (byte)CS1Calc;
            else if (segmentAddressDatas[seg].CS1Address.Bytes == 2)
                SaveUshort(buf, segmentAddressDatas[seg].CS1Address.Address, (ushort)CS1Calc);
            else if (segmentAddressDatas[seg].CS1Address.Bytes == 4)
                SaveUint32(buf, segmentAddressDatas[seg].CS1Address.Address, CS1Calc);
        }

        public void saveCS2(int seg, uint CS2Calc)
        {
            if (segmentAddressDatas[seg].CS2Address.Bytes == 1)
                buf[segmentAddressDatas[seg].CS2Address.Address] = (byte)CS2Calc;
            else if (segmentAddressDatas[seg].CS2Address.Bytes == 2)
                SaveUshort(buf, segmentAddressDatas[seg].CS2Address.Address, (ushort)CS2Calc);
            else if (segmentAddressDatas[seg].CS2Address.Bytes == 4)
                SaveUint32(buf, segmentAddressDatas[seg].CS2Address.Address, CS2Calc);

        }

        public UInt64 calculateCS1(int seg, bool dbg = true)
        {
            SegmentConfig S = Segments[seg];
            return CalculateChecksum(buf, segmentAddressDatas[seg].CS1Address, segmentAddressDatas[seg].CS1Blocks, segmentAddressDatas[seg].ExcludeBlocks, S.CS1Method, S.CS1Complement, segmentAddressDatas[seg].CS1Address.Bytes, S.CS1SwapBytes,dbg);
        }

        public UInt64 calculateCS2(int seg, bool dbg = true)
        {
            SegmentConfig S = Segments[seg];
            return CalculateChecksum(buf, segmentAddressDatas[seg].CS2Address, segmentAddressDatas[seg].CS2Blocks, segmentAddressDatas[seg].ExcludeBlocks, S.CS2Method, S.CS2Complement, segmentAddressDatas[seg].CS2Address.Bytes, S.CS2SwapBytes,dbg);
        }
        public bool FixCheckSums()
        {
            bool needFix = false;
            try
            {
                Logger("Fixing Checksums:");
                for (int seg = 0; seg < Segments.Count; seg++)
                {
                    if (Segments[seg].CS1Blocks.Length == 0)
                        continue;
                    SegmentConfig S = Segments[seg];
                    Logger(S.Name);
                    if (S.Eeprom)
                    {
                        if (GmEeprom.FixEepromKey(buf))
                            needFix = true;
                    }
                    else
                    {
                        if (S.CS1Method != CSMethod_None)
                        {
                            uint CS1 = 0;
                            UInt64 CS1Calc = calculateCS1(seg);
                            if (segmentAddressDatas[seg].CS1Address.Address < uint.MaxValue)
                            {
                                if (segmentAddressDatas[seg].CS1Address.Bytes == 1)
                                {
                                    CS1 = buf[segmentAddressDatas[seg].CS1Address.Address];
                                }
                                else if (segmentAddressDatas[seg].CS1Address.Bytes == 2)
                                {
                                    CS1 = BEToUint16(buf, segmentAddressDatas[seg].CS1Address.Address);
                                }
                                else if (segmentAddressDatas[seg].CS1Address.Bytes == 4)
                                {
                                    CS1 = BEToUint32(buf, segmentAddressDatas[seg].CS1Address.Address);
                                }
                            }
                            if (CS1 == CS1Calc)
                                Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                            else
                            {
                                if (segmentAddressDatas[seg].CS1Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (segmentAddressDatas[seg].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (segmentAddressDatas[seg].CS1Address.Bytes * 2).ToString();
                                    Logger("Checksum 1: " + CS1Calc.ToString(hexdigits) + " [Not saved]");
                                }
                                else
                                {
                                    needFix = true;
                                    saveCS1(seg, (uint)CS1Calc);
                                    Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None)
                        {
                            uint CS2 = 0;
                            UInt64 CS2Calc = calculateCS2(seg);
                            if (segmentAddressDatas[seg].CS2Address.Address < uint.MaxValue)
                            {
                                if (segmentAddressDatas[seg].CS2Address.Bytes == 1)
                                {
                                    CS2 = buf[segmentAddressDatas[seg].CS2Address.Address];
                                }
                                else if (segmentAddressDatas[seg].CS2Address.Bytes == 2)
                                {
                                    CS2 = BEToUint16(buf, segmentAddressDatas[seg].CS2Address.Address);
                                }
                                else if (segmentAddressDatas[seg].CS2Address.Bytes == 4)
                                {
                                    CS2 = BEToUint32(buf, segmentAddressDatas[seg].CS2Address.Address);
                                }
                            }
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                            else
                            {
                                if (segmentAddressDatas[seg].CS2Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (segmentAddressDatas[seg].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (segmentAddressDatas[seg].CS2Address.Bytes * 2).ToString();
                                    Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    needFix = true;
                                    saveCS2(seg, (uint)CS2Calc);
                                    Logger(" Checksum 2: " + CS2.ToString("X") + " => " + CS2Calc.ToString("X4") + " [Fixed]");
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                LoggerBold("Error: " + ex.Message);
            }
            return needFix;
        }

        public void loadAddresses()
        {
            string FileName = Path.Combine(Application.StartupPath, "XML", "addresses-" + OS + ".xml");
            if (!File.Exists(FileName))
                return;
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<osAddresses>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            osAddressList = (List<osAddresses>)reader.Deserialize(file);
            file.Close();
        }

        private void GetSegmentAddresses()
        {
            segmentAddressDatas = new SegmentAddressData[Segments.Count];
            segmentinfos = new SegmentInfo[Segments.Count];
            for (int i = 0; i < Segments.Count; i++)
            {
                segmentinfos[i] = new SegmentInfo(this, i);
                SegmentConfig S = Segments[i];
                S.Missing = false;
                List<Block> B = new List<Block>();
                segmentAddressDatas[i].ExcludeBlocks = B;
                if (S.Eeprom)
                {
                    //Special case for GM eeprom segment
                    Block eeblock;
                    eeblock.Start = 0x4000;
                    eeblock.End = 0x7fff;
                    B.Add(eeblock);
                    segmentAddressDatas[i].SegmentBlocks = B;
                }
                else if (S.SearchAddresses != null)
                {
                    if (!FindSegment(S, i))
                        return;
                }
                else
                {
                    if (!ParseSegmentAddresses(S.Addresses, S, out B))
                    {
                        S.Missing = true;
                        return;
                    }
                    segmentAddressDatas[i].SegmentBlocks = B;
                }
                if (S.SwapAddress != null && S.SwapAddress.Length > 1)
                { 
                    if (!ParseSegmentAddresses(S.SwapAddress, S, out B))
                        return;
                    segmentAddressDatas[i].SwapBlocks = B;
                }
                if (ParseSegmentAddresses(S.CS1Blocks, S, out B))
                {
                    segmentAddressDatas[i].CS1Blocks = B;
                    if (!ParseSegmentAddresses(S.CS2Blocks, S, out B))
                        return;
                    segmentAddressDatas[i].CS2Blocks = B;
                    segmentAddressDatas[i].CS1Address = ParseAddress(S.CS1Address, i);
                    segmentAddressDatas[i].CS2Address = ParseAddress(S.CS2Address, i);
                    if (S.CheckWords != null && S.CheckWords != "")
                        FindCheckwordData(buf, S, ref segmentAddressDatas[i]);

                    if (segmentAddressDatas[i].PNaddr.Bytes == 0)  //if not searched
                        segmentAddressDatas[i].PNaddr = ParseAddress(S.PNAddr, i);
                    segmentAddressDatas[i].VerAddr = ParseAddress(S.VerAddr, i);
                    segmentAddressDatas[i].SegNrAddr = ParseAddress(S.SegNrAddr, i);
                    segmentAddressDatas[i].ExtraInfo = ParseExtraInfo(S.ExtraInfo, i);
                }
                else if (S.CS1Address.Contains("seek:"))
                {
                    S.Missing = true;
                    Segments.RemoveAt(i);
                    Segments.Insert(i, S);
                }
            }
            osAddressList = new List<osAddresses>();
            loadAddresses();
            GetInfo();
        }

        public void GetInfo()
        {
                    
            bool checksumOK = true;

            if (SegmentList == null)
                SegmentList = new List<StaticSegmentInfo>();
            for (int i = 0; i < Segments.Count; i++)
            {
                SegmentConfig S = Segments[i];
                if (S.Missing)
                    continue;

                if (S.CS1Method != CSMethod_None)
                {
                    if (segmentAddressDatas[i].CS1Address.Address != uint.MaxValue)
                    {
                        if (segmentinfos[i].getCS1() != segmentinfos[i].getCS1Calc())
                        { 
                            checksumOK = false;
                        }
                    }
                        
                }

                if (S.CS2Method != CSMethod_None)
                {
                    if (segmentAddressDatas[i].CS2Address.Address != uint.MaxValue)
                    {
                        if (segmentinfos[i].getCS2() != segmentinfos[i].getCS2Calc())
                        {
                            checksumOK = false;
                        }
                    }
                }

                StaticSegmentInfo ssi = new StaticSegmentInfo(segmentinfos[i]);
                SegmentList.Add(ssi);
            }
            if (!checksumOK)
            {
                for (int i = 0; i< Segments.Count; i++)
                {
                    if (!Segments[i].Missing)
                    {
                        StaticSegmentInfo ssi = new StaticSegmentInfo(segmentinfos[i]);
                        BadChkFileList.Add(ssi);
                    }
                }
            }
        }

        public bool FindSegment(SegmentConfig S, int SegNr)
        {
            if (!S.Searchfor.Contains(":"))
                throw new Exception("Segment search need 3 parameters: Serch for: search from: Y/N (" + S.Searchfor + ")");
            Debug.WriteLine("Searching segment");

            string[] Parts = S.Searchfor.Split(',');
            foreach (string Part in Parts)
            {
                string[] ForFrom = Part.Split(':');
                if (ForFrom.Length != 3)
                    throw new Exception("Segment search need 3 parameters: Serch for: search from: Y/N (" + Part + ")");
                Debug.WriteLine("Searching for: " + ForFrom[0] + " From: " + ForFrom[1] + " " + ForFrom[2]);

                if (ForFrom[0].Length == 0)
                    return false;
                ushort Bytes = (ushort)(ForFrom[0].Length / 2);
                if (Bytes == 1)
                    Bytes = 2;
                if (Bytes == 3)
                    Bytes = 4;
                if (Bytes > 4 && Bytes < 8)
                    Bytes = 8;
                UInt64 SearchFor;
                if (!HexToUint64(ForFrom[0], out SearchFor))
                    return false;
                uint SearchFrom;
                if (!HexToUint(ForFrom[1], out SearchFrom))
                    return false;
                string SearchNot = ForFrom[2].ToLower();

                List<Block> Blocks;
                segmentAddressDatas[SegNr].SegmentBlocks = new List<Block>();
                if (!ParseSegmentAddresses(S.SearchAddresses, S, out Blocks))
                    return false;
                foreach (Block B in Blocks)
                {

                    uint Addr = B.Start + SearchFrom;
                    if (!SearchNot.StartsWith("n"))
                    {

                        if (Bytes == 8)
                            if (BEToUint64(buf, Addr) == SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 4)
                            if (BEToUint32(buf, Addr) == SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 2)
                            if (BEToUint16(buf, Addr) == SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 1)
                            if (buf[Addr] == SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                    }
                    else
                    {
                        if (Bytes == 8)
                            if (BEToUint64(buf, Addr) != SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 4)
                            if (BEToUint32(buf, Addr) != SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 2)
                            if (BEToUint16(buf, Addr) != SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 1)
                            if (buf[Addr] != SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                    }
                }
            }
            Debug.WriteLine("Not found");
            return false;
        }

        public bool FindCheckwordData(byte[] buf, SegmentConfig S, ref SegmentAddressData binfile)
        {
            Debug.WriteLine("Checkwords: " + S.CheckWords);
            if (S.CheckWords == null)
                return false;
            binfile.Checkwords = new List<CheckWord>();
            string[] CWlist = S.CheckWords.Split(',');
            foreach (string Row in CWlist)
            {
                string[] Parts = Row.Split(':');
                if (Parts.Length == 4)
                {
                    Debug.WriteLine(Parts[3] + ": " + Parts[0] + " in " + Parts[1] + " ?");
                    CheckWord checkw = new CheckWord();
                    UInt64 CW;
                    uint Location;
                    if (!HexToUint64(Parts[0], out CW))
                        return false;
                    if (!HexToUint(Parts[1], out Location))
                        return false;
                    if (!HexToUint(Parts[2], out checkw.Address))
                        return false;
                    checkw.Key = Parts[3];
                    checkw.Address += binfile.SegmentBlocks[0].Start;

                    Location += binfile.SegmentBlocks[0].Start;
                    ushort Bytes = (ushort)(Parts[0].Length / 2);
                    if (Bytes == 1)
                        Bytes = 2;
                    if (Bytes == 3)
                        Bytes = 4;
                    if (Bytes > 4 && Bytes < 8)
                        Bytes = 8;

                    if (Bytes == 1)
                        if (buf[Location] == CW)
                        {
                            Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X"));
                            binfile.Checkwords.Add(checkw);
                        }
                    if (Bytes == 2)
                        if (BEToUint16(buf, Location) == CW)
                        {
                            Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X2"));
                            binfile.Checkwords.Add(checkw);
                        }
                    if (Bytes == 4)
                        if (BEToUint32(buf, Location) == CW)
                        {
                            Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X4"));
                            binfile.Checkwords.Add(checkw);
                        }
                    if (Bytes == 8)
                        if (BEToUint64(buf, Location) == CW)
                        {
                            Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X8"));
                            binfile.Checkwords.Add(checkw);
                        }
                }
            }
            return true;
        }


        public string ReadInfo(AddressData AD)
        {
            Debug.WriteLine("Reading address: " + AD.Address.ToString("X") + ", bytes: " + AD.Bytes.ToString() + ", Type: " + AD.Type);
            if (AD.Address == uint.MaxValue)
            {
                Debug.WriteLine("Address not defined");
                return "";
            }
            string Result = "";

            if (AD.Type == TypeHex)
            {
                for (int a = 0; a < AD.Bytes; a++)
                    Result += buf[AD.Address + a].ToString("X2");
            }
            else if (AD.Type == TypeText)
            {
                Result = ReadTextBlock(buf, (int)AD.Address, AD.Bytes);
            }
            else if (AD.Type == TypeFilename)
            {
                Result = AD.Address.ToString();
            }
            else if (AD.Bytes == 1)
            {
                    Result = buf[AD.Address].ToString();
            }
            else if (AD.Bytes == 2)
            {
                    Result = BEToUint16(buf, AD.Address).ToString();
            }
            else if (AD.Bytes == 8)
            {
                    Result = BEToUint64(buf, AD.Address).ToString();
            }
            else //Default is 4 bytes
            {
                    Result = BEToUint32(buf, AD.Address).ToString();
            }

            Debug.WriteLine("Result: " + Result);
            return Result;
        }

        public UInt64 ReadValue(AddressData AD)
        {
            Debug.WriteLine("Reading address: " + AD.Address.ToString("X") + ", bytes: " + AD.Bytes.ToString() + ", Type: " + AD.Type);
            if (AD.Address == uint.MaxValue)
            {
                Debug.WriteLine("Address not defined");
                return UInt64.MaxValue;
            }
            UInt64 Result = UInt64.MaxValue;

            if (AD.Bytes == 1)
            {
                Result = (uint)buf[AD.Address];
            }
            else if (AD.Bytes == 2)
            {
                Result = BEToUint16(buf, AD.Address);
            }
            else if (AD.Bytes == 8)
            {
                Result = (UInt64)BEToUint64(buf, AD.Address);
            }
            else //Default is 4 bytes
            {
                Result = BEToUint32(buf, AD.Address);
            }

            Debug.WriteLine("Result: " + Result.ToString("X"));
            return Result;
        }

        private uint FindV6OSAddress(byte[] searchfor)
        {
            uint osStoreAddr = uint.MaxValue;
            for (uint i=0; i < fsize - 6; i++)
            {
                bool match = true;
                for (uint j=0; j<6; j++)
                {
                    if (buf[i + j] != searchfor[j])
                    { 
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    Debug.WriteLine("Found OS Store address from: " + i.ToString("X"));
                    osStoreAddr = i;
                    //Check if this ONLY match
                    for (uint k=i+6; k < fsize - 6; k++)
                    {
                        bool secondmatch = true;
                        for (uint j = 0; j < 6; j++)
                        {
                            if (buf[k + j] != searchfor[j])
                            { 
                                secondmatch = false;
                                break;
                            }
                        }
                        if (secondmatch)
                        {
                            //Found other match, dont know which is correct
                            Debug.WriteLine("Found other match from: " + k.ToString("X") + ", dont know which is correct");
                            return uint.MaxValue;
                        }
                    }
                }
            }
            return osStoreAddr;
        }
        private uint FindV6checksumAddress()
        {
            byte[] searchfor = new byte[6];
            osStoreAddress = uint.MaxValue;

            if (fsize == 256 * 1024)
            {
                searchfor = new byte[] {0x20,0x39,0x0,0x03,0xff,0xfa};
                osStoreAddress = FindV6OSAddress(searchfor);
            }
            if (fsize == 512 * 1024)
            {
                searchfor = new byte[] { 0x26, 0x39, 0x0, 0x07, 0xff, 0xfa };
                osStoreAddress = FindV6OSAddress(searchfor);
                if (osStoreAddress == uint.MaxValue)
                {
                    searchfor = new byte[] { 0x20, 0x39, 0x0, 0x07, 0xff, 0xfa };
                    osStoreAddress = FindV6OSAddress(searchfor);
                }
                if (osStoreAddress == uint.MaxValue)
                {
                    searchfor = new byte[] { 0x20, 0x39, 0x0, 0x07, 0xff, 0xf8 };
                    osStoreAddress = FindV6OSAddress(searchfor);
                }
                if (osStoreAddress == uint.MaxValue)
                {
                    searchfor = new byte[] { 0x26, 0x39, 0x0, 0x07, 0xff, 0xf8 };
                    osStoreAddress = FindV6OSAddress(searchfor);
                }
            }
            if (fsize == 1024 * 1024)
            {
                searchfor = new byte[] { 0x26, 0x39, 0x0, 0x0f, 0xff, 0xfa };
                osStoreAddress = FindV6OSAddress(searchfor);
            }

            if (osStoreAddress == uint.MaxValue)
                return osStoreAddress;

            for (uint i=osStoreAddress + 16; i< osStoreAddress + 32 && i< fsize; i++)
            {
                if (buf[i] == searchfor[0] && buf[i+1] == searchfor[1])
                {
                    return BEToUint32(buf, i + 2);
                }
            }
            //Not found?
            return uint.MaxValue;
        }

        private string FindMafCodes(byte[] searchfor)
        {
            string res = "";
            uint prevMafAddr = uint.MaxValue;
            for (uint i = 0; i < fsize - 6; i++)
            {
                bool match = true;
                for (uint j = 0; j < 6; j++)
                {
                    if (buf[i + j] != searchfor[j])
                    { 
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    Debug.WriteLine("Found MAF address from: " + i.ToString("X"));
                    uint mafAddr = BEToUint32(buf, i + 6);
                    if (mafAddr != prevMafAddr)
                    { 
                        if (res.Length > 0)
                            res += ",";
                        res += mafAddr.ToString("X");
                        prevMafAddr = mafAddr;
                    }
                }
            }
            return res;
        }

        private void FindV6MAFAddress()
        {
            byte[] searchfor = new byte[6];
            mafAddress = "";

            searchfor = new byte[] { 0x30, 0x3C, 0x50, 0x0, 0x20, 0x7c };
            mafAddress = FindMafCodes(searchfor);
            if (mafAddress.Length == 0)
            {
                searchfor = new byte[] { 0x36, 0x3C, 0x50, 0x0, 0x24, 0x7c };
                mafAddress = FindMafCodes(searchfor);
            }

        }
        private V6Table FindVEAddr(byte[] searchfor, ushort length)
        {
            V6Table v6;
            v6.address = uint.MaxValue;
            v6.rows = ushort.MaxValue;
            
            for (uint i = 0; i < fsize - length; i++)
            {
                bool match = true;
                for (uint j = 0; j < length; j++)
                {
                    if (buf[i + j] != searchfor[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    Debug.WriteLine("Found VE search sequence from: " + i.ToString("X"));
                    if (buf[i + length] == 0x74 && buf[i+length+2] == 0x20 &&  buf[i + length + 3] == 0x7C)
                    {
                        Debug.WriteLine("Found V6 VE table from: " + v6.address.ToString("X"));
                        v6.address = BEToUint32(buf, i + length + 4);
                        v6.rows = buf[i + length + 1];
                    }
                }
            }
            return v6;
        }

        private void FindV6VeTable()
        {
            byte[] searchfor = new byte[] { 0x0C, 0x40, 0x1C, 0x00, 0x64, 0x04, 0xE2, 0x48, 0x60, 0x04, 0x30, 0x3C, 0x0E, 0x00 };
            v6VeTable = FindVEAddr(searchfor, 14);
            if (v6VeTable.address == uint.MaxValue)
            {
                searchfor = new byte[] { 0x0C, 0x40, 0x1C, 0x00, 0x64, 0x08, 0xE2, 0x48, 0x04, 0x40, 0x02, 0x00, 0x60, 0x04, 0X30, 0X3C, 0X0C, 0X00 };
                v6VeTable = FindVEAddr(searchfor, 18);
            }
            if (v6VeTable.address == uint.MaxValue)
            {
                searchfor = new byte[] { 0x0C, 0x46, 0x1C, 0x00, 0x64, 0x0A, 0x20, 0x06, 0xE2, 0x48, 0x04, 0x40, 0x02, 0x00, 0X60, 0X04, 0X30, 0X3C, 0x0C, 0x00 };
                v6VeTable = FindVEAddr(searchfor, 20);
            }
        }

        private void FindV6OtherTables()
        {
            v6tables = new List<V6Table>();
            uint calStartAddr = 0;
            if (fsize == 256 * 1024)
                calStartAddr = 0x36000;
            if (fsize == 512 * 1024)
                calStartAddr = 0x64000;
            if (fsize == 1024 * 1024)
                calStartAddr = 0xCF000;

            for (uint i=0;i< fsize-10; i++)
            {
                if (buf[i] == 0x20 && buf[i + 1] == 0x7C && buf[i + 6] == 0x4E && buf[i + 7] == 0xB9)
                {
                    for (uint j=0; j < 18 && i - j > 0; j++)
                    {
                        if (buf[i-j] == 0x74)
                        {
                            uint addr = BEToUint32(buf,i + 2);
                            if (addr < fsize && addr > calStartAddr)
                            { 
                                Debug.WriteLine("Found V6 table address from address: " + (i + 3).ToString("X"));
                                V6Table v6 = new V6Table();
                                v6.address = addr;
                                v6.rows = buf[i - j + 1];
                                v6tables.Add(v6);
                            }
                        }
                    }
                }
            }
        }
        private AddressData GMV6(string Line, int SegNr)
        {
            uint BufSize = (uint)buf.Length;
            uint GMOS = 0;
            segmentAddressDatas[SegNr].PNaddr.Address = 0;
            AddressData AD = new AddressData();

            for (int i = 2; i < 20; i++)
            {
                if (BEToUint16(buf, (uint)(BufSize - i)) == 0xA55A) //Read OS version from end of file, before bytes A5 5A
                {
                    segmentAddressDatas[SegNr].PNaddr.Address = (uint)(BufSize - (i + 4));
                    Debug.WriteLine("V6: Found PN address from: " + segmentAddressDatas[SegNr].PNaddr.Address.ToString("X"));
                }
            }
            if (segmentAddressDatas[SegNr].PNaddr.Address == 0)
                throw new Exception("OS id missing");
            GMOS = BEToUint32(buf, segmentAddressDatas[SegNr].PNaddr.Address);
            segmentAddressDatas[SegNr].PNaddr.Bytes = 4;
            segmentAddressDatas[SegNr].PNaddr.Type = TypeInt;
            Block B = new Block();
            B.Start = segmentAddressDatas[SegNr].PNaddr.Address;
            B.End = segmentAddressDatas[SegNr].PNaddr.Address + 3;
            if (segmentAddressDatas[SegNr].ExcludeBlocks == null)
                segmentAddressDatas[SegNr].ExcludeBlocks = new List<Block>();
            segmentAddressDatas[SegNr].ExcludeBlocks.Add(B);

            FindV6MAFAddress();
            FindV6VeTable();
            FindV6OtherTables();

            AD.Address = FindV6checksumAddress();
            if (AD.Address < uint.MaxValue)
            {
                Debug.WriteLine("Find V6 checksum address: " + AD.Address.ToString("X"));
                AD.Bytes = 4;
                AD.Type = TypeHex;
                return AD;
            }

            Debug.WriteLine("Checksum address not found, using old file-method");

            string FileName = Path.Combine(Application.StartupPath, "XML", Line);
            StreamReader sr = new StreamReader(FileName);
            string OsLine;
            string LastLine = "";
            bool isinfile = false;
            while ((OsLine = sr.ReadLine()) != null)
            {
                //Custom handling: read OS:Segmentaddress pairs from file
                string[] OsLineparts = OsLine.Split(':');
                if (OsLineparts.Length == 2)
                {
                    if (OsLineparts[0] == GMOS.ToString())
                    {
                        isinfile = true;
                        if (HexToUint(OsLineparts[1], out AD.Address))
                        {
                            Debug.WriteLine("Address: " + AD.Address.ToString("X") + ", Bytes: 4, Type: HEX");
                            sr.Close();
                            AD.Bytes = 4;
                            AD.Type = TypeHex;
                            return AD;
                        }
                    }
                    LastLine = OsLine;
                }
            }
            sr.Close();

            if (!isinfile)
            {
                //OS not in file, add it: (OS may be in file, but without address)
                string NewOS = "";
                if (!LastLine.Contains(Environment.NewLine))
                    NewOS = Environment.NewLine;
                NewOS += GMOS.ToString() + ":";
                StreamWriter sw = new StreamWriter(FileName,true);
                sw.WriteLine(NewOS);
                sw.Close();
                throw new Exception("Unsupported OS:  " + GMOS.ToString() + ", adding to file: " + FileName);

            }
            throw new Exception("Unsupported OS:  " + GMOS.ToString());

        }

        private uint seekAddress(string line)
        {
            uint retVal = uint.MaxValue;
            try
            {
                if (foundSegments.Count == 0)
                {
                    SegmentSeek sSeek = new SegmentSeek();
                    sSeek.seekSegments(this);
                    if (foundSegments.Count == 0)
                    {
                        //Stop seeking again if nothing is found
                        FoundSegment fs = new FoundSegment(this);
                        foundSegments.Add(fs);
                    }
                }

                string[] parts = line.Split(':');
                if (parts.Length < 2)
                    throw new Exception("Name missing: " + line);

                foreach (FoundSegment fs in foundSegments)
                {
                    if (fs.Name == parts[1])
                    {
                        Debug.WriteLine("SegmentSeek: " + fs.Name + ", Address: " + fs.Address);
                        return fs.addrInt;
                    }
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var errline = frame.GetFileLineNumber();
                Debug.WriteLine("Error, PcmFile line " + errline + ": " + ex.Message);
            }
            return retVal;

        }

        public AddressData ParseAddress(string Line, int SegNr)
        {
            AddressData AD = new AddressData();
            AD.Address = uint.MaxValue;
            AD.Bytes = 2;
            AD.Type = TypeInt;
            try
            {
                Debug.WriteLine("Addressline: " + Line);
                //Set defaults:

                if (Line.Length == 0)
                {
                    return AD;
                }

                if (Line.StartsWith("GM-V6"))
                {
                    //Custom handling: read OS:Segmentaddress pairs from file
                    Debug.WriteLine("V6");
                    return GMV6(Line, SegNr);
                }

                //Special handling, seek address
                if (Line.ToLower().StartsWith("seek:"))
                {
                    AD.Address = seekAddress(Line);
                    //Address handled, handle bytes & type:
                    string[] lParts = Line.Split(':');
                    if (lParts.Length > 2)
                        UInt16.TryParse(lParts[2], out AD.Bytes);
                    if (lParts.Length > 3)
                    {
                        if (lParts[3].ToLower() == "hex")
                            AD.Type = TypeHex;
                        else if (lParts[2].ToLower() == "text")
                            AD.Type = TypeText;
                    }

                    return AD;
                }

                //Special handling, get info from filename:
                if (Line.StartsWith("filename"))
                {
                    if (!Line.Contains(":"))
                        throw new Exception("usage: filename:digits, or filename:digitsmax-digitsmin");
                    string[] parts = Line.Split(':');
                    ushort digitsmax;
                    ushort digitsmin;
                    if (parts[1].Contains("-"))
                    {
                        string[] digitparts = parts[1].Split('-');
                        if (!ushort.TryParse(digitparts[0], out digitsmax))
                            throw new Exception("usage: filename:digits or filename:digitsmax-digitsmin");
                        if (!ushort.TryParse(digitparts[1], out digitsmin))
                            throw new Exception("usage: filename:digits or filename:digitsmax-digitsmin");
                    }
                    else
                    {
                        ushort x;
                        if (!ushort.TryParse(parts[1], out x))
                            throw new Exception("usage: filename:digits or filename:digitsmax-digitsmin");
                        digitsmin = x;
                        digitsmax = x;
                    }
                    for (ushort digits = digitsmax; digits >= digitsmin; digits--)
                    {
                        string[] numbers = Regex.Split(FileName, @"\D+");
                        for (int v = numbers.Length - 1; v >= 0; v--)
                        {
                            string value = numbers[v];
                            if (!string.IsNullOrEmpty(value) && value.Length == digits)
                            {
                                AD.Address = uint.Parse(value);
                                Debug.WriteLine("PN from filename: {0}", AD.Address);
                                AD.Bytes = digits;
                                AD.Type = TypeFilename;
                                return AD;
                            }
                        }
                    }
                    //Not found?
                    return AD;
                }

                string[] Lineparts = Line.Split(':');
                CheckWord CWAddr;
                CWAddr.Address = 0;
                CWAddr.Key = "";
                bool Negative = false;
                if (Lineparts[0].Contains("CW"))
                {
                    CWAddr = GetCheckwordAddress(Lineparts[0], SegNr);
                    if (CWAddr.Key != "")
                        Lineparts[0] = Lineparts[0].Replace(CWAddr.Key, "");
                }

                if (Lineparts[0].Replace("#", "") == "")
                {
                    //If address is not defined: (For checksum, display-only)
                    AD.Address = uint.MaxValue;
                }
                else
                {
                    bool relativetoend = false;
                    if (Lineparts[0].EndsWith("@"))
                    {
                        Lineparts[0] = Lineparts[0].Replace("@", "");
                        relativetoend = true;
                    }

                    if (!HexToUint(Lineparts[0].Replace("#", ""), out AD.Address))
                        throw new Exception("Can't convert from HEX: " + Lineparts[0].Replace("#", "") + " (" + Line + ")");

                    if (relativetoend)
                    {
                        if (Line.StartsWith("#"))
                        {
                            AD.Address = segmentAddressDatas[SegNr].SegmentBlocks[(segmentAddressDatas[SegNr].SegmentBlocks.Count - 1)].End - AD.Address;
                        }
                        else
                        {
                            AD.Address = fsize - AD.Address;
                        }
                    }
                    else
                    {
                        if (Line.StartsWith("#"))
                        {
                            AD.Address += segmentAddressDatas[SegNr].SegmentBlocks[0].Start;
                        }
                        if (Negative)
                            AD.Address = CWAddr.Address - AD.Address;
                        else
                            AD.Address += CWAddr.Address;
                    }
                }
                //Address handled, handle bytes & type:
                if (Lineparts.Length > 1)
                    UInt16.TryParse(Lineparts[1], out AD.Bytes);
                if (Lineparts.Length > 2)
                {
                    if (Lineparts[2].ToLower() == "hex")
                        AD.Type = TypeHex;
                    else if (Lineparts[2].ToLower() == "text")
                        AD.Type = TypeText;
                }
                Debug.WriteLine("Name: " + AD.Name + ", Address: " + AD.Address.ToString("X") + ", Bytes: " + AD.Bytes.ToString() + ", Type: " + AD.Type.ToString());
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var errline = frame.GetFileLineNumber();
                Debug.WriteLine("Error, PcmFile line " + errline + ": " + ex.Message);
            }
            return AD;
        }


        public CheckWord GetCheckwordAddress(string AddrLine, int SegNr)
        {
            string[] LineParts;
            if (AddrLine.Contains("+"))
                LineParts = AddrLine.Split('+');
            else if (AddrLine.Contains("-"))
                LineParts = AddrLine.Split('-');
            else
            {
                throw new Exception("No + or - after Checkword! (" + AddrLine + ")");
            }
            foreach (CheckWord CW in segmentAddressDatas[SegNr].Checkwords)
            {
                if (LineParts[0] == (CW.Key))
                {
                    Debug.WriteLine("Checkword: " + CW.Key + " => " + CW.Address.ToString("X"));
                    return CW;
                }
            }
            CheckWord checkw = new CheckWord();
            checkw.Address = 0;
            checkw.Key = "";
            return checkw;
        }

        public bool ParseSegmentAddresses(string Line, SegmentConfig S, out List<Block> Blocks)
        {
            Blocks = new List<Block>();
            try
            {
                Debug.WriteLine("Segment address line: " + Line);

                if (Line == null || Line == "")
                {
                    //It is ok to have empty address (for CS, not for segment)
                    Block B = new Block();
                    B.End = 0;
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
                    bool isWord = false;
                    ushort Multiple = 1;

                    if (StartEnd[0].ToLower().StartsWith("seek:"))
                    {
                        B.Start = seekAddress(StartEnd[0]);
                    }
                    if (StartEnd.Length > 1 && StartEnd[1].ToLower().StartsWith("seek:"))
                    {
                        B.End = seekAddress(StartEnd[1]);
                    }
                    if (Part.Contains("seek:"))
                    {
                        if (B.Start < uint.MaxValue && B.End < uint.MaxValue)
                        {
                            Blocks.Add(B);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

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


                    if (StartEnd[0].Contains("*"))
                    {
                        string[] SM = StartEnd[0].Split('*');
                        StartEnd[0] = SM[0];
                        UInt16.TryParse(SM[1], out Multiple);
                    }

                    if (StartEnd[0].Contains(":2"))
                    {
                        string[] SW = StartEnd[0].Split(':');
                        StartEnd[0] = SW[0];
                        isWord = true;
                    }

                    if (StartEnd.Length > 1 && StartEnd[1].Contains(":2"))
                    {
                        string[] EW = StartEnd[1].Split(':');
                        StartEnd[1] = EW[0];
                        isWord = true;
                    }


                    if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
                    {
                        throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")");
                    }
                    if (StartEnd[0].StartsWith("@"))
                    {
                        uint tmpStart = B.Start;
                        for (int m = 1; m <= Multiple; m++)
                        {
                            //Read address from bin at this address

                            if (isWord)
                            {
                                B.Start = BEToUint16(buf, tmpStart);
                                B.End = BEToUint16(buf, tmpStart + 2);
                                tmpStart += 4;
                            }
                            else
                            {
                                B.Start = BEToUint32(buf, tmpStart);
                                B.End = BEToUint32(buf, tmpStart + 4);
                                tmpStart += 8;
                            }
                            if (Multiple > 1)
                            {
                                // Have multiple start-end pairs
                                B.Start += (uint)Offset;
                                B.End += (uint)Offset;
                                Blocks.Add(B);
                            }
                        }
                    }
                    else
                    {
                        if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                            throw new Exception("Can't decode from HEX: " + StartEnd[1].Replace("@", "") + " (" + Line + ")");
                        if (B.End >= buf.Length)    //Make 1MB config work with 512kB bin
                            B.End = (uint)buf.Length - 1;
                    }
                    if (Multiple < 2)
                    {
                        if (StartEnd.Length > 1 && StartEnd[1].StartsWith("@"))
                        {
                            //Read End address from bin at this address
                            B.End = BEToUint32(buf, B.End);
                        }
                        if (StartEnd.Length > 1 && StartEnd[1].EndsWith("@"))
                        {
                            //Address is relative to end of bin
                            uint end;
                            if (HexToUint(StartEnd[1].Replace("@", ""), out end))
                                B.End = (uint)buf.Length - end - 1;
                        }
                        B.Start += (uint)Offset;
                        B.End += (uint)Offset;
                        Blocks.Add(B);
                    }
                    i++;
                }
                foreach (Block B in Blocks)
                    Debug.WriteLine("Address block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var errline = frame.GetFileLineNumber();
                Debug.WriteLine("Error, PcmFile line " + errline + ": " + ex.Message);
            }
            return true;
        }

        public List<AddressData> ParseExtraInfo(string Line, int SegNr)
        {
            List<AddressData> LEX = new List<AddressData>();
            try
            {
                Debug.WriteLine("Extrainfo: " + Line);
                if (Line == null || Line.Length == 0 || !Line.Contains(":"))
                    return LEX;

                string[] LineParts = Line.Split(',');
                foreach (string LinePart in LineParts)
                {
                    AddressData E = new AddressData();
                    string[] AddrParts = LinePart.Split(':');
                    if (AddrParts.Length < 3)
                        return LEX;

                    E.Name = AddrParts[0];

                    CheckWord CWAddr;
                    CWAddr.Key = "";
                    CWAddr.Address = 0;
                    bool Negative = false;
                    if (AddrParts[1].Contains("CW"))
                    {
                        CWAddr = GetCheckwordAddress(AddrParts[1], SegNr);
                        if (CWAddr.Key != "")
                            AddrParts[1] = AddrParts[1].Replace(CWAddr.Key, "");
                    }
                    if (AddrParts[1].Contains("-"))
                        Negative = true;
                    AddrParts[1] = AddrParts[1].Replace("-", "");
                    AddrParts[1] = AddrParts[1].Replace("+", "");
                    if (!HexToUint(AddrParts[1].Replace("#", ""), out E.Address))
                        return LEX;

                    if (Negative)
                        E.Address = CWAddr.Address - E.Address;
                    else
                        E.Address += CWAddr.Address;

                    if (AddrParts[1].StartsWith("#"))
                    {
                        E.Address += segmentAddressDatas[SegNr].SegmentBlocks[0].Start;
                    }

                    if (AddrParts.Length > 2)
                        UInt16.TryParse(AddrParts[2], out E.Bytes);
                    E.Type = TypeInt;
                    if (AddrParts.Length > 3)
                    {
                        if (AddrParts[3].ToLower() == "hex")
                            E.Type = TypeHex;
                        else if (AddrParts[3].ToLower() == "text")
                            E.Type = TypeText;
                    }
                    LEX.Add(E);
                }
                for (int l = 0; l < LEX.Count; l++)
                    Debug.WriteLine("Extrainfo name: " + LEX[l].Name + ", Address: " + LEX[l].Address.ToString("X") + ", Bytes: " + LEX[l].Bytes + ", Type: " + LEX[l].Type);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var errline = frame.GetFileLineNumber();
                Debug.WriteLine("Error, PcmFile line " + errline + ": " + ex.Message);
            }
            return LEX;
        }

        public string GetSegmentName(uint addr)
        {
            string retVal = "";
            for (int s = 0; s<segmentinfos.Length; s++)
            {
                for (int b=0; b< segmentAddressDatas[s].SegmentBlocks.Count; b++)
                {
                    if (addr >= segmentAddressDatas[s].SegmentBlocks[b].Start && addr <= segmentAddressDatas[s].SegmentBlocks[b].End)
                    {
                        retVal = segmentinfos[s].Name;
                        return retVal;
                    }
                }
            }
            return retVal;
        }

        public int GetSegmentNumber(uint addr)
        {
            int retVal = -1;
            for (int s = 0; s < segmentinfos.Length; s++)
            {
                for (int b = 0; b < segmentAddressDatas[s].SegmentBlocks.Count; b++)
                {
                    if (addr >= segmentAddressDatas[s].SegmentBlocks[b].Start && addr <= segmentAddressDatas[s].SegmentBlocks[b].End)
                    {
                        retVal = s;
                        return retVal;
                    }
                }
            }
            return retVal;
        }

        // If segment number can be read from bin, use that number
        // Othwerwise use ordernumber
        public int getSegmentByNr(string nr)
        {
            for (int i=0; i< segmentinfos.Length; i++)
            {
                if (!Segments[i].Missing && segmentinfos[i].SegNr == nr.Trim())
                    return i;
            }

            int retVal;
            if (int.TryParse(nr, out retVal) == false)
                throw new Exception("Unknown segment: " + nr);

            return retVal;
        }
    }
}
