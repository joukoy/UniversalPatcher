using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using static Upatcher;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using static Helpers;

namespace UniversalPatcher
{
    public class PcmFile
    {
        public PcmFile()
        {
            SetDefaultValues();
        }
        public PcmFile(string Fname, bool autodetect, string cnfFile)
        {
            try
            {
                SetDefaultValues();
                FileName = Fname;
                altTableDatas[0].Name = Fname;
                fsize = (uint)new FileInfo(FileName).Length;
                buf = ReadBin(FileName);
                osStoreAddress = uint.MaxValue;
                if (autodetect)
                {
                    string cfgFile = AutoDetect(this);
                    if (cfgFile.Length > 0)
                    {
                        _configFileFullName = Path.Combine(Application.StartupPath, "XML", cfgFile);
                    }
                }
                else
                {
                    _configFileFullName = cnfFile;
                }
                if (segmentFile.Length > 0)
                {
                    LoadPlatformConfig(PlatformConfigFile);
                    LoadConfigFile(segmentFile);
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
            public AltTableData()
            {
                tableDatas = new List<TableData>();
                Navigator = new List<TreeParts.Navi>();
                tunerFile = "";
            }
            public List<TableData> tableDatas { get; set; }
            public string Name { get; set; }
            public List<TreeParts.Navi> Navigator { get; set; }
            public int NaviCurrent { get; set; }
            public string tunerFile { get; set; }

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
        public string v6CalStart;
        public string v6OSCrc;
        public List<osAddresses> osAddressList;
        public List<SegmentConfig> Segments;
        public List<FoundTable> foundTables;
        public List<FoundSegment> foundSegments;
        public List<DtcCode> dtcCodes;
        public List<DtcCode> dtcCodes2;
        public Dictionary<byte, string> dtcValues;
/*        public List<string> SelectedNodeCategory;
        public List<string> SelectedNodeDimension;
        public List<string> SelectedNodeMulti;
        public List<string> SelectedNodeSegment;
        public List<string> SelectedNodeValueType;
        public List<string> SelectedNodeList;
*/
        public Dictionary<string, List<String>> SelectedNode = new Dictionary<string, List<string>>();
        public List<TreeParts.Navi> Navigator = new List<TreeParts.Navi>();
        public int NaviCurrent;

        public List<string> tableCategories
        {
            get 
            {
                List<string> cats = new List<string>();
                cats.Add("_All");
                if (segmentinfos != null)
                {
                    for (int c = 0; c < segmentinfos.Length; c++)
                    {
                        if (!Segments[c].Missing)
                            cats.Add("Seg-" + segmentinfos[c].Name);
                    }
                }
                for (int t=0; t<tableDatas.Count; t++)
                {
                    if (!cats.Contains(tableDatas[t].Category))
                    {
                        cats.Add(tableDatas[t].Category);
                    }
                }
                return cats;
            }
        }
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
        public string compXml = "";
        public bool seekTablesImported;
        public PcmPlatform platformConfig = new PcmPlatform();

        public string segmentFile
        {
            get
            {
                if (!string.IsNullOrEmpty(platformConfig.SegmentFile))
                    return Path.Combine(Application.StartupPath, "XML", platformConfig.SegmentFile);
                else if (configFile.Length > 0)
                    return Path.Combine(Application.StartupPath, "XML", configFile + ".xml");
                else
                    return "";
            }
        }
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

        public int DiagSegment
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

        public string TableSeekFile
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
                if (!string.IsNullOrEmpty(platformConfig.TableSeekFile))
                    tSeekFile = Path.Combine(Application.StartupPath, "XML", platformConfig.TableSeekFile);

                if (File.Exists(tSeekFile))
                {
                    long conFileSize = new FileInfo(tSeekFile).Length;
                    if (conFileSize < 255)
                    {
                        string compXml = ReadTextFile(tSeekFile).Split(new[] { '\r', '\n' }).FirstOrDefault();
                        compXml = Path.Combine(Application.StartupPath, "XML", compXml);
                        if (File.Exists(compXml))
                        {
                            tSeekFile = compXml;
                            Logger("Using compatible file: " + Path.GetFileName(compXml));
                        }
                    }
                }

                return tSeekFile;
            }
        }


        public string SegmentSeekFile
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
                if (!string.IsNullOrEmpty(platformConfig.SegmentSeekFile))
                    ssFile = Path.Combine(Application.StartupPath, "XML", platformConfig.SegmentSeekFile);

                if (File.Exists(ssFile))
                {
                    long conFileSize = new FileInfo(ssFile).Length;
                    if (conFileSize < 255)
                    {
                        string compXml = ReadTextFile(ssFile).Split(new[] { '\r', '\n' }).FirstOrDefault();
                        compXml = Path.Combine(Application.StartupPath, "XML", compXml);
                        if (File.Exists(compXml))
                        {
                            ssFile = compXml;
                            Logger("Using compatible file: " + Path.GetFileName(compXml));
                        }
                    }
                }

                return ssFile;

            }
        }

        public string PlatformConfigFile { get {return Path.Combine(Application.StartupPath, "XML", configFile + "-platform.xml"); } }

        public void SetDefaultValues()
        {            
            _configFileFullName = "";
            Segments = new List<SegmentConfig>();
            SwapSegments = new List<SwapSegment>();
            //dtcCodes = new List<DtcCode>();
            //dtcCodes2 = new List<DtcCode>();
            tableDatas = new List<TableData>();
            foundTables = new List<FoundTable>();
            foundSegments = null;
            //tableCategories = new List<string>();
            altTableDatas = new List<AltTableData>();
            currentTableDatasList = 0;
            SelectTableDatas(0, "");
            seekTablesImported = false;
        }
        public PcmFile ShallowCopy()
        {
            return (PcmFile)this.MemberwiseClone();
        }

        public void LoadPlatformConfig(string fName)
        {
            if (fName.Length == 0)
                fName = PlatformConfigFile;
            if (File.Exists(fName))
            {
                Logger("Reading Platform config: " + Path.GetFileName(fName), false);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PcmPlatform));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                platformConfig = (PcmPlatform)reader.Deserialize(file);
                file.Close();
                Logger(" [OK]");
            }
            else
            {
                //Create file && do conversion
                Logger("Creating Platform config: " + Path.GetFileName(fName), false);
                platformConfig.MSB = true;
                platformConfig.SegmentFile = configFile + ".xml";
                platformConfig.SegmentSeekFile = Path.GetFileName(SegmentSeekFile);
                platformConfig.TableSeekFile = Path.GetFileName(TableSeekFile);
                for (int i=0; i< pidSearchConfigs.Count; i++)
                {
                    if (pidSearchConfigs[i].XMLFile == configFile)
                    {
                        platformConfig.PidSearchString = pidSearchConfigs[i].SearchString;
                        platformConfig.PidSearchStep = (uint)pidSearchConfigs[i].Step;
                    }
                    SavePlatformConfig(PlatformConfigFile);
                }
                Logger(" [OK]");
            }
        }

        public void SavePlatformConfig(string fName)
        {
            try
            {
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PcmPlatform));
                    writer.Serialize(stream, platformConfig);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void ReloadBinFile()
        {
            buf = ReadBin(FileName);
        }

        /// <summary>
        /// This function checks if file is modified after loading
        /// </summary>
        /// 
        public bool BufModified()
        {
            if (FileName == null || buf.Length == 0)
                return false;
            byte[] compareBuf = ReadBin(FileName);
            if (buf.SequenceEqual(compareBuf))
                return false;
            else
                return true;
        }

        public void ImportSeekTables()
        {
            try
            {
                if (seekTablesImported)
                    return;
                if (foundTables.Count == 0)
                {
                    TableSeek TS = new TableSeek();
                    Logger("Seeking tables...", false);
                    Logger(TS.SeekTables(this));
                }
                Logger("Importing TableSeek tables... ", false);
                for (int i = 0; i < foundTables.Count; i++)
                {
                    TableData tableData = new TableData();
                    tableData.ImportFoundTable(i, this);
                    tableDatas.Add(tableData);
                }
                //if (!tableCategories.Contains("DTC"))
                  //  tableCategories.Add("DTC");
                seekTablesImported = true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void ImportDTC()
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
                tdTmp.ImportDTC(this, ref tableDatas,true);
                tdTmp.ImportDTC(this, ref tableDatas, false);
                Logger(" [OK]");
            }
        }

        public void ImportTinyTunerDB()
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.ReadTinyDBtoTableData(this, tableDatas));
        }

        public void UpdateAddressesByOS()
        {
            Logger("Updating addresses", false);
            int i = 0;
            if (string.IsNullOrEmpty(OS))
            {
                Logger(" [No OS]");
                return;
            }
            foreach (TableData td in tableDatas)
            {
                td.UpdateAddressByOS(OS);
                i++;
                if (i % 300 == 0)
                {
                    Logger(".", false);
                    Application.DoEvents();
                }
            }
            Logger(" [Done]");
        }
        public void AutoLoadTunerConfig()
        {
            if (!AppSettings.disableTunerAutoloadSettings)
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(Application.StartupPath, "Tuner"));
               // FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);

                List<FileInfo> filterdFiles = new List<FileInfo>();
                FileInfo[] Files = d.GetFiles("*.*");
                filterdFiles.AddRange(Files);
                DirectoryInfo[] dirs = d.GetDirectories();
                for (int di=0; di< dirs.Length;di++)
                {
                    string fldr = Path.GetFileName(dirs[di].Name);
                    if (!fldr.StartsWith(".") && !fldr.StartsWith(".") && !fldr.StartsWith("~"))
                        filterdFiles.AddRange(dirs[di].GetFiles("*.*", SearchOption.AllDirectories));
                }

                string defaultTunerFile = OS;
                if (OS.Length == 0)
                    defaultTunerFile = configFile + "-def";
                else if (OSSegment < segmentinfos.Length)
                    defaultTunerFile = OS + "-" + segmentinfos[OSSegment].Ver;
                defaultTunerFile += ".xml";
                FileInfo tFile = null;
                tFile = filterdFiles.Where(x => x.Name == defaultTunerFile).FirstOrDefault();
                if (tFile == null)
                {
                    defaultTunerFile = OS + ".xml";
                    tFile = filterdFiles.Where(x => x.Name == defaultTunerFile).FirstOrDefault();
                }
                if (tFile == null)
                {
                    tFile = filterdFiles.Where(x => x.Name.Contains(OS)).FirstOrDefault();
                }
                compXml = "";
                if (tFile != null)
                {

                    if (tFile.Length < 255)
                    {
                        string tmpXml= ReadTextFile(tFile.FullName).Split(new[] { '\r', '\n' }).FirstOrDefault(); ;
                        if (File.Exists(tmpXml))
                        {
                            compXml = tmpXml;
                            tFile = filterdFiles.Where(x => x.Name == compXml).FirstOrDefault();
                            Logger("Using compatible file: " + compXml);
                        }
                    }
                }
                if (tFile == null)
                {
                    defaultTunerFile = configFile + "-def.xml";
                    tFile = filterdFiles.Where(x => x.Name == defaultTunerFile).FirstOrDefault();
                }
                if (tFile != null)
                {
                    LoadTableList(tFile.FullName);
                    tunerFile = tFile.FullName;
                }
            }

        }

        public void ClearTableList()
        {
            tableDatas = new List<TableData>();
            Navigator = new List<TreeParts.Navi>();
            NaviCurrent = 0;
        }

        public void LoadTableList(string fName = "")
        {
            try
            {

                string defName = Path.Combine(Application.StartupPath, "Tuner", OS + ".xml");
                //string defName = PCM.OS + ".xml";
                if (fName == "")
                    fName = SelectFile("Select XML File", XmlFilter, defName);
                if (fName.Length == 0)
                    return ;
                List<TableData> tmpTableDatas = new List<TableData>();
                long conFileSize = new FileInfo(fName).Length;
                if (conFileSize < 255)
                {
                    string compXml = ReadTextFile(fName).Split(new[] { '\r', '\n' }).FirstOrDefault();
                    compXml = Path.Combine(Path.GetDirectoryName(fName),compXml);
                    if (File.Exists(compXml))
                    {
                        Logger(Path.GetFileName(fName) + " => " + Path.GetFileName(compXml));
                        fName = compXml;
                    }
                }
                Logger( "Loading tuner file: " + Path.GetFileName(fName), false);
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
/*                    for (int c = 0; c < tableDatas[t].Categories.Count; c++)
                    {
                        string category = tableDatas[t].Categories[c];
                        if (!tableCategories.Contains(category))
                            tableCategories.Add(category);
                    }
*/
                }
                //if (!tableCategories.Contains("DTC"))
                  //  tableCategories.Add("DTC");

                Logger(" [OK]");
                UpdateAddressesByOS();
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
                LoggerBold(" Error, pcmfile line " + line + ": " + ex.Message);
            }
        }

        public void SaveTableList(string fName)
        {
            try
            {
                if (fName.Length == 0)
                    fName = tunerFile;
                if (tunerFile.Length == 0)
                    tunerFile = Path.Combine(Application.StartupPath, "Tuner", OS + ".xml");
                Logger("Saving file " + tunerFile + "...", false);

                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, tableDatas);
                    stream.Close();
                }
                tunerFile = fName;
                Logger(" [OK]");
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

        public void AddTableDatas(string name)
        {
            AltTableData tdl = new AltTableData();
            tdl.Name = name;
            altTableDatas.Add(tdl);
            tunerFile = "";
        }

        public void SelectTableDatas(int listNumber, string name)
        {
            if (altTableDatas.Count < (listNumber + 1))
            {
                AddTableDatas(name);
            }
            if (listNumber != currentTableDatasList)
            {
                //Store old list:
                altTableDatas[currentTableDatasList].tableDatas = tableDatas;
                altTableDatas[currentTableDatasList].Navigator = Navigator;
                altTableDatas[currentTableDatasList].NaviCurrent = NaviCurrent;
                altTableDatas[currentTableDatasList].tunerFile = tunerFile;
                //Select new:
                currentTableDatasList = listNumber;
                tableDatas = altTableDatas[currentTableDatasList].tableDatas;
                tunerFile = altTableDatas[currentTableDatasList].tunerFile;
                NaviCurrent = altTableDatas[currentTableDatasList].NaviCurrent;
                Navigator = altTableDatas[currentTableDatasList].Navigator;
            }
        }

        public void LoadConfigFile(string fileName)
        {
            try
            {
                Segments.Clear();
                Logger("Loading file: " + Path.GetFileName(fileName), false);
                long conFileSize = new FileInfo(fileName).Length;
                if (conFileSize < 255)
                {
                    string compXml = ReadTextFile(fileName).Split(new[] { '\r', '\n' }).FirstOrDefault();
                    compXml = Path.Combine(Path.GetDirectoryName(fileName), compXml);
                    if (File.Exists(compXml))
                    {
                        fileName = compXml;
                        Logger(", Using compatible file: " + Path.GetFileName(compXml));
                    }
                }

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

        public void SaveConfigFile(string fileName)
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

        public void SaveBin(string fName)
        {
            try
            {
                if (AppSettings.DisableAutoFixChecksum)
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

        public void SaveCS1(int seg, UInt64 CS1Calc)
        {
            if (segmentAddressDatas[seg].CS1Address.Bytes == 1)
                buf[segmentAddressDatas[seg].CS1Address.Address] = (byte)CS1Calc;
            else if (segmentAddressDatas[seg].CS1Address.Bytes == 2)
                 SaveUshort(segmentAddressDatas[seg].CS1Address.Address, (ushort)CS1Calc);
            else if (segmentAddressDatas[seg].CS1Address.Bytes == 4)
                SaveUint32(segmentAddressDatas[seg].CS1Address.Address, (uint)CS1Calc);
            else if (segmentAddressDatas[seg].CS1Address.Bytes == 8)
                SaveUint64(segmentAddressDatas[seg].CS1Address.Address, CS1Calc);
        }

        public void SaveCS2(int seg, UInt64 CS2Calc)
        {
            if (segmentAddressDatas[seg].CS2Address.Bytes == 1)
                buf[segmentAddressDatas[seg].CS2Address.Address] = (byte)CS2Calc;
            else if (segmentAddressDatas[seg].CS2Address.Bytes == 2)
                SaveUshort(segmentAddressDatas[seg].CS2Address.Address, (ushort)CS2Calc);
            else if (segmentAddressDatas[seg].CS2Address.Bytes == 4)
                SaveUint32(segmentAddressDatas[seg].CS2Address.Address, (uint)CS2Calc);
            else if (segmentAddressDatas[seg].CS2Address.Bytes == 8)
                SaveUint64(segmentAddressDatas[seg].CS2Address.Address, CS2Calc);

        }

        public UInt64 CalculateCS1(int seg, bool dbg = true)
        {
            SegmentConfig S = Segments[seg];
            if (Segments[seg].Checksum1Method == CSMethod.None)
                return uint.MaxValue;
            return CalculateChecksum(platformConfig.MSB, buf, segmentAddressDatas[seg].CS1Address, segmentAddressDatas[seg].CS1Blocks, segmentAddressDatas[seg].ExcludeBlocks, S.Checksum1Method, S.CS1Complement, segmentAddressDatas[seg].CS1Address.Bytes, S.CS1SwapBytes,dbg);
        }

        public UInt64 CalculateCS2(int seg, bool dbg = true)
        {
            SegmentConfig S = Segments[seg];
            if (Segments[seg].Checksum2Method == CSMethod.None)
                return uint.MaxValue;
            return CalculateChecksum(platformConfig.MSB, buf, segmentAddressDatas[seg].CS2Address, segmentAddressDatas[seg].CS2Blocks, segmentAddressDatas[seg].ExcludeBlocks, S.Checksum2Method, S.CS2Complement, segmentAddressDatas[seg].CS2Address.Bytes, S.CS2SwapBytes,dbg);
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
                        if (S.Checksum1Method != CSMethod.None)
                        {
                            uint CS1 = 0;
                            UInt64 CS1Calc = CalculateCS1(seg);
                            if (segmentAddressDatas[seg].CS1Address.Address < uint.MaxValue)
                            {
                                CS1 = (uint)ReadValue(segmentAddressDatas[seg].CS1Address);
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
                                    SaveCS1(seg, CS1Calc);
                                    Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                }
                            }
                        }

                        if (S.Checksum2Method != CSMethod.None)
                        {
                            uint CS2 = 0;
                            UInt64 CS2Calc = CalculateCS2(seg);
                            if (segmentAddressDatas[seg].CS2Address.Address < uint.MaxValue)
                            {
                                CS2 = (uint)ReadValue(segmentAddressDatas[seg].CS2Address);
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
                                    SaveCS2(seg, CS2Calc);
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

        public void LoadAddresses()
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
            SeekSegments();

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
                else if (!string.IsNullOrEmpty(S.SearchAddresses))
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
                if (!string.IsNullOrEmpty(S.SwapAddress) && S.SwapAddress.Length > 1)
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
                    if (!string.IsNullOrEmpty(S.CheckWords))
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
            LoadAddresses();
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

                if (S.Checksum1Method != CSMethod.None)
                {
                    if (segmentAddressDatas[i].CS1Address.Address != uint.MaxValue)
                    {
                        if (segmentinfos[i].GetCS1() != segmentinfos[i].GetCS1Calc())
                        { 
                            checksumOK = false;
                        }
                    }
                        
                }

                if (S.Checksum2Method != CSMethod.None)
                {
                    if (segmentAddressDatas[i].CS2Address.Address != uint.MaxValue)
                    {
                        if (segmentinfos[i].GetCS2() != segmentinfos[i].GetCS2Calc())
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
                            if (ReadUInt64(Addr) == SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 4)
                            if (ReadUInt32(Addr) == SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 2)
                            if (ReadUInt16(Addr) == SearchFor)
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
                            if (ReadUInt64(Addr) != SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 4)
                            if (ReadUInt32(Addr) != SearchFor)
                            {
                                segmentAddressDatas[SegNr].SegmentBlocks.Add(B);
                                Debug.WriteLine("Found: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                                return true;
                            }
                        if (Bytes == 2)
                            if (ReadUInt16(Addr) != SearchFor)
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
                        if (ReadUInt16(Location) == CW)
                        {
                            Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X2"));
                            binfile.Checkwords.Add(checkw);
                        }
                    if (Bytes == 4)
                        if (ReadUInt32(Location) == CW)
                        {
                            Debug.WriteLine("Checkword: " + checkw.Key + " Found in: " + Location.ToString("X") + ", Data location: " + checkw.Address.ToString("X4"));
                            binfile.Checkwords.Add(checkw);
                        }
                    if (Bytes == 8)
                        if (ReadUInt64(Location) == CW)
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

            if (AD.Type == AddressDataType.Hex)
            {
                for (int a = 0; a < AD.Bytes; a++)
                    Result += buf[AD.Address + a].ToString("X2");
            }
            else if (AD.Type == AddressDataType.Text)
            {
                Result = ReadTextBlock(buf, (int)AD.Address, AD.Bytes,false);
            }
            else if (AD.Type == AddressDataType.Filename)
            {
                Result = AD.Address.ToString();
            }
            else
            {
                Result = ReadValue(AD).ToString();
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
                Result = ReadUInt16(AD.Address);
            }
            else if (AD.Bytes == 8)
            {
                Result = (UInt64)ReadUInt64(AD.Address);
            }
            else //Default is 4 bytes
            {
                Result = ReadUInt32(AD.Address);
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
                    return ReadUInt32(i + 2);
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
                    uint mafAddr = ReadUInt32(i + 6);
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
                        v6.address = ReadUInt32(i + length + 4);
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
                            uint addr = ReadUInt32(i + 2);
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

        private void FindV6CalStart_OSCrc()
        {
            string searchStr;
            if (fsize == (512 * 1024))
            {
                searchStr = "B0 B9 * * * * 65 08 B0 B9 * * * * 63 10 B0 B9 @ @ @ @ 65 2C";
            }
            else if (fsize == (1024 * 1024))
            {
                searchStr = "65 0A 22 0A B2 B9 * * * * 63 14 22 0A B2 B9 @ @ @ @ 65";
            }
            else
            {
                return;
            }
            uint start = 0x8000;
            uint addr1 = GetAddrbySearchString(this,searchStr,ref start,fsize).Addr;
            if (addr1 < uint.MaxValue)
            {
                uint calStart = ReadUInt32(addr1);
                v6CalStart = calStart.ToString("X4");
                Debug.WriteLine("Found V6 Cal start: " + v6CalStart);
                uint crcEnd = calStart - 1; //6e2ff
                uint calcSize = 0x4000 + crcEnd - 0x8000 +1;
                byte[] calcBuf = new byte[calcSize];
                Array.Copy(buf, 0, calcBuf, 0, 0x4000);
                Array.Copy(buf, 0x8000, calcBuf, 0x4000, crcEnd - 0x8000 + 1);
                Debug.WriteLine("V6 OS CRC: 0 - 3FFF, 8000 - " + crcEnd.ToString("X4") +", " + calcSize.ToString("X4") + " bytes");                
                Crc32 crc = new Crc32();
                uint cksum = crc.ComputeChecksum(calcBuf);
                v6OSCrc = cksum.ToString("X8");
                Debug.WriteLine("V6 OS CRC: " + v6OSCrc);
            }
            else
            {
                Debug.WriteLine("V6 Cal start not found");
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
                if (ReadUInt16((uint)(BufSize - i)) == 0xA55A) //Read OS version from end of file, before bytes A5 5A
                {
                    segmentAddressDatas[SegNr].PNaddr.Address = (uint)(BufSize - (i + 4));
                    Debug.WriteLine("V6: Found PN address from: " + segmentAddressDatas[SegNr].PNaddr.Address.ToString("X"));
                }
            }
            if (segmentAddressDatas[SegNr].PNaddr.Address == 0)
                throw new Exception("OS id missing");
            GMOS = ReadUInt32(segmentAddressDatas[SegNr].PNaddr.Address);
            segmentAddressDatas[SegNr].PNaddr.Bytes = 4;
            segmentAddressDatas[SegNr].PNaddr.Type = AddressDataType.Int;
            Block B = new Block();
            B.Start = segmentAddressDatas[SegNr].PNaddr.Address;
            B.End = segmentAddressDatas[SegNr].PNaddr.Address + 3;
            if (segmentAddressDatas[SegNr].ExcludeBlocks == null)
                segmentAddressDatas[SegNr].ExcludeBlocks = new List<Block>();
            segmentAddressDatas[SegNr].ExcludeBlocks.Add(B);

            FindV6MAFAddress();
            FindV6VeTable();
            FindV6OtherTables();
            FindV6CalStart_OSCrc();

            AD.Address = FindV6checksumAddress();
            if (AD.Address < uint.MaxValue)
            {
                Debug.WriteLine("Find V6 checksum address: " + AD.Address.ToString("X"));
                AD.Bytes = 4;
                AD.Type = AddressDataType.Hex;
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
                            AD.Type = AddressDataType.Hex;
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

        private void SeekSegments()
        {
            if (foundSegments == null)
            {
                foundSegments = new List<FoundSegment>();
                SegmentSeek sSeek = new SegmentSeek();
                sSeek.SeekSegments(this);
            }
        }

        private uint SeekAddress(string line)
        {
            uint retVal = uint.MaxValue;
            try
            {
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
            AD.Type = AddressDataType.Int;
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
                    string[] lineParts = Line.Split(',');   
                    foreach (string linePart in lineParts)
                    {
                        //If there is multiple seek's return first we can find
                        AD.Address = SeekAddress(linePart);
                        if (AD.Address < uint.MaxValue)
                        {
                            //Address handled, handle bytes & type:
                            string[] lParts = linePart.Split(':');
                            if (lParts.Length > 2)
                                UInt16.TryParse(lParts[2], out AD.Bytes);
                            if (lParts.Length > 3)
                            {
                                if (lParts[3].ToLower() == "hex")
                                    AD.Type = AddressDataType.Hex;
                                else if (lParts[3].ToLower() == "text")
                                    AD.Type = AddressDataType.Text;
                            }
                            return AD;
                        }
                    }
                    //Not found?
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
                                AD.Type = AddressDataType.Filename;
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
                        AD.Type = AddressDataType.Hex;
                    else if (Lineparts[2].ToLower() == "text")
                        AD.Type = AddressDataType.Text;
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
                    int EndOffset = 0;
                    bool isWord = false;
                    ushort Multiple = 1;

                    bool useLength = false;
                    if (StartEnd.Length > 1 && StartEnd[1].StartsWith("L"))
                    {
                        useLength = true;
                        StartEnd[1] = StartEnd[1].Replace("L", "");
                    }

                    if (StartEnd[0].ToLower().StartsWith("seek:"))
                    {
                        B.Start = SeekAddress(StartEnd[0]);
                    }
                    if (StartEnd.Length > 1 && StartEnd[1].ToLower().StartsWith("seek:"))
                    {
                        B.End = SeekAddress(StartEnd[1]);
                    }
/*                    if (Part.Contains("seek:"))
                    {
                        if (B.Start < uint.MaxValue && B.End < uint.MaxValue)
                        {
                            Blocks.Add(B);
                           
                        }
                    }
*/
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
                        Offset = (int)(-1 * x);
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

                    if (!StartEnd[0].Contains("seek"))
                    {
                        if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
                        {
                            throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")");
                        }
                    }

                    if (StartEnd[0].StartsWith("@"))
                    {
                        uint tmpStart = B.Start;
                        for (int m = 1; m <= Multiple; m++)
                        {
                            //Read address from bin at this address

                            if (isWord)
                            {
                                B.Start = ReadUInt16(tmpStart);
                                B.End = ReadUInt16(tmpStart + 2);
                                tmpStart += 4;
                            }
                            else
                            {
                                B.Start = ReadUInt32(tmpStart);
                                B.End = ReadUInt32(tmpStart + 4);
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
                    
                    if (StartEnd.Length > 1 && !StartEnd[1].Contains("seek"))
                    {
                        if (StartEnd[1].Contains(">"))
                        {
                            string[] SO = StartEnd[1].Split('>');
                            StartEnd[1] = SO[0];
                            uint x;
                            if (!HexToUint(SO[1], out x))
                                throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                            EndOffset = (int)x;
                        }
                        if (StartEnd[1].Contains("<"))
                        {
                            string[] SO = StartEnd[1].Split('<');
                            StartEnd[1] = SO[0];
                            uint x;
                            if (!HexToUint(SO[1], out x))
                                throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                            EndOffset = (int)(-1 * x);
                        }

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
                            B.End = ReadUInt32(B.End);
                        }
                        if (StartEnd.Length > 1 && StartEnd[1].EndsWith("@"))
                        {
                            //Address is relative to end of bin
                            uint end;
                            if (HexToUint(StartEnd[1].Replace("@", ""), out end))
                                B.End = (uint)buf.Length - end - 1;
                        }
                        B.Start = (uint)(B.Start + Offset);
                        if (EndOffset != 0)
                            B.End = (uint)(B.End + EndOffset);
                        else
                            B.End = (uint)(B.End + Offset + EndOffset);
                        if (useLength)
                        {
                            B.End = B.Start -1 + B.End;
                        }
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
                for  (int p=0; p < LineParts.Length; p++)
                {
                    AddressData E = new AddressData();

                    string LinePart = LineParts[p].Trim();
                    string[] AddrParts = LinePart.Split(':');
                    if (AddrParts.Length < 3)
                        return LEX;

                    if (AddrParts[0] == ("seek"))
                    {
                        E.Address = SeekAddress(LinePart);
                        E.Name = AddrParts[1];
                    }
                    else
                    {
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
                    }

                    if (AddrParts.Length > 2)
                        UInt16.TryParse(AddrParts[2], out E.Bytes);
                    E.Type = AddressDataType.Int;
                    if (AddrParts.Length > 3)
                    {
                        if (AddrParts[3].ToLower() == "hex")
                            E.Type = AddressDataType.Hex;
                        else if (AddrParts[3].ToLower() == "text")
                            E.Type = AddressDataType.Text;
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
            try
            {
                if (segmentinfos != null)
                {
                    for (int s = 0; s < segmentinfos.Length; s++)
                    {
                        for (int b = 0; b < segmentAddressDatas[s].SegmentBlocks.Count; b++)
                        {
                            if (addr >= segmentAddressDatas[s].SegmentBlocks[b].Start && addr <= segmentAddressDatas[s].SegmentBlocks[b].End)
                            {
                                retVal = segmentinfos[s].Name;
                                return retVal;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return retVal;
        }

        public int GetSegmentNumber(uint addr)
        {
            int retVal = -1;
            try
            {
                if (segmentinfos == null)
                    return -1;
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
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return retVal;
        }

        // If segment number can be read from bin, use that number
        // Othwerwise use ordernumber
        public int GetSegmentByNr(string nr)
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

        public TableData GetTdbyHeader(string header)
        {
            TableData headerTd = null;
            try
            {
                if (header.ToLower().StartsWith("guid:"))
                {
                    Guid tdGuid = Guid.Parse(header.Substring(5).TrimStart());
                    headerTd = tableDatas.Where(x => x.guid == tdGuid).First();
                }
                else if (header.ToLower().StartsWith("table:"))
                {
                    string tbName = header.Substring(6).Trim();
                    headerTd = tableDatas.Where(x => x.TableName == tbName).FirstOrDefault();
                    if (headerTd == null)
                    {
                        foreach (TableData td in tableDatas)
                        {
                            if (td.TableName.StartsWith(tbName +"*"))
                            {
                                headerTd = td;
                                break;
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
                var errline = frame.GetFileLineNumber();
                Debug.WriteLine("Error, PcmFile line " + errline + ": " + ex.Message);
            }
            return headerTd;
        }

        public TableData GetConversiotableByMath(string mathStr)
        {
            TableData retVal = null;
            try
            {
                //Example: TABLE:'MAF Scalar #1'
                int start = mathStr.ToLower().IndexOf("table:") + 6;
                int mid = mathStr.IndexOf("'", start + 2);
                string conversionTable = mathStr.Substring(start, mid - start + 1);
                string tbName = conversionTable.Replace("'", "");
                retVal = tableDatas.Where(x => x.TableName == tbName).First();
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

        public byte ReadByte(uint addr)
        {
            return buf[addr];
        }

        public ushort ReadUInt16(uint addr)
        {
            if (platformConfig.MSB)
                return (UInt16)((buf[addr] << 8) | buf[addr + 1]);
            else
                return (UInt16)((buf[addr + 1] << 8) | buf[addr]);
        }

        public Int16 ReadInt16(uint addr)
        {
            if (platformConfig.MSB)
                return (Int16)((buf[addr] << 8) | buf[addr + 1]);
            else
                return (Int16)((buf[addr + 1] << 8) | buf[addr]);
        }

        public UInt32 ReadUInt32(uint addr)
        {
            byte[] tmp = new byte[4];
            Array.Copy(buf, addr, tmp, 0, 4);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            return BitConverter.ToUInt32(tmp, 0);
        }

        public Int32 ReadInt32(uint addr)
        {
            byte[] tmp = new byte[4];
            Array.Copy(buf, addr, tmp, 0, 4);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }

        public Double ReadFloat32(uint addr)
        {
            byte[] tmp = new byte[4];
            Array.Copy(buf, addr, tmp, 0, 4);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            return BitConverter.ToSingle(tmp, 0);
        }

        public UInt64 ReadUInt64(uint addr)
        {
            byte[] tmp = new byte[8];
            Array.Copy(buf, addr, tmp, 0, 8);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            return BitConverter.ToUInt64(tmp, 0);
        }

        public Int64 ReadInt64(uint addr)
        {
            byte[] tmp = new byte[8];
            Array.Copy(buf, addr, tmp, 0, 8);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }

        public Double ReadFloat64(uint addr)
        {
            byte[] tmp = new byte[8];
            Array.Copy(buf, addr, tmp, 0, 8);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            return BitConverter.ToDouble(tmp, 0);
        }


        public void SaveFloat32(uint offset, Single data)
        {
            byte[] tmp = new byte[4];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 4);
        }

        public void SaveFloat64(uint offset, double data)
        {
            byte[] tmp = new byte[8];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 8);
        }

        public void SaveUint64(uint offset, UInt64 data)
        {
            byte[] tmp = new byte[8];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 8);
        }

        public void SaveInt64(uint offset, Int64 data)
        {
            byte[] tmp = new byte[8];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 8);
        }
        public void SaveUint32(uint offset, UInt32 data)
        {
            byte[] tmp = new byte[4];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 4);
        }
        public void SaveInt32(uint offset, Int32 data)
        {
            byte[] tmp = new byte[4];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 4);
        }

        public void Save3Bytes(uint offset, UInt32 data)
        {
            if (platformConfig.MSB)
            {
                buf[offset] = (byte)(data & 0xff);
                buf[offset + 1] = (byte)((data >> 8) & 0xff);
                buf[offset + 2] = (byte)((data >> 16) & 0xff);
            }
            else
            {
                buf[offset + 2] = (byte)(data & 0xff);
                buf[offset + 1] = (byte)((data >> 8) & 0xff);
                buf[offset] = (byte)((data >> 16) & 0xff);
            }
        }

        public void SaveUshort(uint offset, ushort data)
        {
            byte[] tmp = new byte[2];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 2);
        }

        public void SaveShort(uint offset, short data)
        {
            byte[] tmp = new byte[2];
            tmp = BitConverter.GetBytes(data);
            if (platformConfig.MSB)
                Array.Reverse(tmp);
            Array.Copy(tmp, 0, buf, offset, 2);
        }


    }
}
