using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;
using System.Text.RegularExpressions;

using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using UniversalPatcher.Properties;
using System.Drawing.Text;
using System.ComponentModel.Design;

namespace UniversalPatcher
{
    public partial class FrmPatcher : Form
    {
        public FrmPatcher()
        {
            InitializeComponent();
            txtResult.EnableContextMenu();
            txtDebug.EnableContextMenu();
            frmpatcher = this;
        }
        private struct DetectGroup
        {
            public string Logic;
            public uint Hits;
            public uint Miss;
        }

        private frmSegmenList frmSL;
        private PcmFile basefile;
        private PcmFile modfile;
        private CheckBox[] chkSegments;
        private CheckBox[] chkExtractSegments;
        private string LastXML = "";
        private BindingSource bindingSource = new BindingSource();
        private BindingSource CvnSource = new BindingSource();
        private BindingSource badCvnSource = new BindingSource();
        private BindingSource Finfosource = new BindingSource();
        private BindingSource badchkfilesource = new BindingSource();
        private BindingSource searchedTablesBindingSource = new BindingSource();
        private BindingSource pidListBindingSource = new BindingSource();
        private BindingSource dtcBindingSource = new BindingSource();
        private List<PidSearch.PID> pidList = new List<PidSearch.PID>();
        private uint lastCustomSearchResult = 0;
        private string logFile;
        StreamWriter logwriter;
        private void FrmPatcher_Load(object sender, EventArgs e)
        {
            this.Show();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                Logger(args[1]);
                frmSegmenList frmSL = new frmSegmenList();
                frmSL.LoadFile(args[1]);
            }
            addCheckBoxes();
            numSuppress.Value = Properties.Settings.Default.SuppressAfter;
            if (numSuppress.Value == 0)
                numSuppress.Value = 10;

            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Patches")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Patches"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "XML")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "XML"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Segments")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Segments"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Log")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Log"));
            logFile = Path.Combine(Application.StartupPath, "Log", "universalpatcher" + DateTime.Now.ToString("_yyyyMMdd_hhmm") + ".rtf");


            if (Properties.Settings.Default.LastXMLfolder == "")
                Properties.Settings.Default.LastXMLfolder = Path.Combine(Application.StartupPath, "XML");
            if (Properties.Settings.Default.LastPATCHfolder == "")
                Properties.Settings.Default.LastPATCHfolder = Path.Combine(Application.StartupPath, "Patches");
            chkDebug.Checked = Properties.Settings.Default.DebugOn;
            checkAutorefreshCVNlist.Checked = Properties.Settings.Default.AutorefreshCVNlist;
            checkAutorefreshFileinfo.Checked = Properties.Settings.Default.AutorefreshFileinfo;

            DetectRules = new List<DetectRule>();
            string AutoDetectFile = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
            if (File.Exists(AutoDetectFile))
            {
                Debug.WriteLine("Loading autodetect.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                System.IO.StreamReader file = new System.IO.StreamReader(AutoDetectFile);
                DetectRules = (List<DetectRule>)reader.Deserialize(file);
                file.Close();
            }

            StockCVN = new List<CVN>();
            string StockCVNFile = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
            if (File.Exists(StockCVNFile))
            {
                Debug.WriteLine("Loading stockcvn.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                System.IO.StreamReader file = new System.IO.StreamReader(StockCVNFile);
                StockCVN = (List<CVN>)reader.Deserialize(file);
                file.Close();
            }
            else
            {
                //Dirty fix to make system work without stockcvn.xml
                CVN ctmp = new CVN();
                ctmp.cvn = "";
                ctmp.PN = "";
                StockCVN.Add(ctmp);
            }
            loadReferenceCvn();

            string SwapSegmentListFile = Path.Combine(Application.StartupPath, "Segments", "extractedsegments.xml");
            if (File.Exists(SwapSegmentListFile))
            {
                Debug.WriteLine("Loading extractedsegments.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<SwapSegment>));
                System.IO.StreamReader file = new System.IO.StreamReader(SwapSegmentListFile);
                SwapSegments = (List<SwapSegment>)reader.Deserialize(file);
                file.Close();

            }
            else
            {
                SwapSegments = new List<SwapSegment>();
            }


            string FileTypeListFile = Path.Combine(Application.StartupPath, "XML", "filetypes.xml");
            if (File.Exists(FileTypeListFile))
            {
                Debug.WriteLine("Loading filetypes.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<FileType>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileTypeListFile);
                fileTypeList = (List<FileType>)reader.Deserialize(file);
                file.Close();

            }
            else
            {
                fileTypeList = new List<FileType>();
            }

            listCSAddresses.Enabled = true;
            listCSAddresses.Clear();
            listCSAddresses.View = View.Details;
            listCSAddresses.FullRowSelect = true;
            listCSAddresses.Columns.Add("OS");
            listCSAddresses.Columns.Add("CS1 Address");
            listCSAddresses.Columns.Add("OS Store Address");
            listCSAddresses.Columns.Add("MAF Address");
            listCSAddresses.Columns.Add("VE table");
            listCSAddresses.Columns.Add("3d tables");
            //listCSAddresses.Columns[0].Width = 100;
            //listCSAddresses.Columns[1].Width = 100;
            //listCSAddresses.Columns[2].Width = 100;
            //listCSAddresses.Columns[2].Width = 100;
        }

        public void refreshSearchedTables()
        {
            int count = 0;
            if (tableSearchResult == null)
                return;
            searchedTablesBindingSource.DataSource = null;
            if (chkTableSearchNoFilters.Checked)
            {
                searchedTablesBindingSource.DataSource = tableSearchResultNoFilters;
                count = tableSearchResultNoFilters.Count;
            }
            else
            {
                searchedTablesBindingSource.DataSource = tableSearchResult;
                count = tableSearchResult.Count;
            }
            dataGridSearchedTables.DataSource = null;
            dataGridSearchedTables.DataSource = searchedTablesBindingSource;
            dataGridSearchedTables.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            if (tableSearchResult.Count == 0)
                tabSearchedTables.Text = "Searched Tables";
            else
                tabSearchedTables.Text = "Searched Tables (" + count.ToString() + ")";
        }
        public void refreshPidList()
        {
            pidListBindingSource.DataSource = null;
            pidListBindingSource.DataSource = pidList;
            dataGridPIDlist.DataSource = null;
            dataGridPIDlist.DataSource = pidListBindingSource;
            dataGridPIDlist.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            if (pidList.Count == 0)
                tabPIDList.Text = "PIDs";
            else
                tabPIDList.Text = "PIDs (" + pidList.Count.ToString() + ")";
        }

        public void refreshDtcList()
        {
            dtcBindingSource.DataSource = null;
            dtcBindingSource.DataSource = dtcCodes;
            dataGridDTC.DataSource = null;
            dataGridDTC.DataSource = dtcBindingSource;
            dataGridDTC.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            if (dtcCodes.Count == 0)
                tabDTC.Text = "DTC";
            else
                tabDTC.Text = "DTC (" + dtcCodes.Count.ToString() + ")";
        }

        private void FrmPatcher_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (chkLogtoFile.Checked)
            {
                chkLogtoFile.Checked = false;
                Application.DoEvents();
            }    
        }
        public void addCheckBoxes()
        {
            if (LastXML == XMLFile && chkSegments != null && chkSegments.Length == Segments.Count)
                return;
            if (chkSegments != null)
            {
                for (int s = 0; s < chkSegments.Length; s++)
                {
                    chkSegments[s].Dispose();
                    chkExtractSegments[s].Dispose();
                }
            }
            int Left = 6;
            chkSegments = new CheckBox[Segments.Count];
            chkExtractSegments = new CheckBox[Segments.Count];
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = new CheckBox();
                tabCreate.Controls.Add(chk);
                chk.Location = new Point(Left, 80);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                if (!chk.Text.ToLower().Contains("eeprom"))
                    chk.Checked = true;
                chkSegments[s] = chk;

                chk = new CheckBox();
                tabExtractSegments.Controls.Add(chk);
                chk.Location = new Point(Left, 80);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                chk.Checked = true;
                chkExtractSegments[s] = chk;

                Left += chk.Width + 5;

            }
            LastXML = XMLFile;

        }

        private void CheckSegmentCompatibility()
        {
            if ( txtBaseFile.Text == "" || txtModifierFile.Text == "")
                return;

            labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            for (int s = 0; s < Segments.Count; s++)
            {
                string BasePN = basefile.ReadInfo(basefile.binfile[s].PNaddr);
                string ModPN = modfile.ReadInfo(modfile.binfile[s].PNaddr);
                string BaseVer = basefile.ReadInfo(basefile.binfile[s].VerAddr);
                string ModVer = modfile.ReadInfo(modfile.binfile[s].VerAddr);

                if (BasePN != ModPN || BaseVer != ModVer)
                {
                    Logger(Segments[s].Name.PadLeft(11) + " differ: " + BasePN.ToString().PadRight(8) + " " + BaseVer + " <> " + ModPN.ToString().PadRight(8) + " " + ModVer);
                    chkSegments[s].Enabled = false;
                }
                else
                {
                    chkSegments[s].Enabled = true;
                }
            }
        }

        private void ShowFileInfo(PcmFile PCM, bool InfoOnly)
        {
            try
            {
                if (Segments[0].CS1Address.StartsWith("GM-V6"))
                { 
                    var item = new ListViewItem(PCM.OS);
                    if (PCM.binfile[0].CS1Address.Address == uint.MaxValue)
                        item.SubItems.Add("");
                    else
                        item.SubItems.Add(PCM.binfile[0].CS1Address.Address.ToString("X"));
                    if (PCM.osStoreAddress == uint.MaxValue)
                        item.SubItems.Add("");
                    else
                        item.SubItems.Add(PCM.osStoreAddress.ToString("X"));
                    item.SubItems.Add(PCM.mafAddress);
                    if (PCM.v6VeTable.address == uint.MaxValue)
                        item.SubItems.Add("");
                    else
                        item.SubItems.Add(PCM.v6VeTable.address.ToString("X") + ":" + PCM.v6VeTable.rows.ToString());
                    string v6tablelist = "";
                    for (int i=0; i< PCM.v6tables.Count; i++)
                    {
                        if (i > 0)
                            v6tablelist += ",";
                        v6tablelist += PCM.v6tables[i].address.ToString("X") + ":" + PCM.v6tables[i].rows.ToString();
                    }
                    item.SubItems.Add(v6tablelist);
                    listCSAddresses.Items.Add(item);
                    listCSAddresses.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    tabCsAddress.Text = "Gm-v6 info (" + listCSAddresses.Items.Count.ToString() + ")";
                }
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(" " + PCM.segmentinfos[i].Name.PadRight(11), false);
                    if (PCM.segmentinfos[i].PN.Length > 1)
                    {
                        if (PCM.segmentinfos[i].Stock == "[stock]")
                            LoggerBold(" PN: " + PCM.segmentinfos[i].PN.PadRight(9), false);
                        else
                            Logger(" PN: " + PCM.segmentinfos[i].PN.PadRight(9), false);
                    }
                    if (PCM.segmentinfos[i].Ver.Length > 1)
                        Logger(", Ver: " + PCM.segmentinfos[i].Ver, false);

                    if (PCM.segmentinfos[i].SegNr.Length > 0)
                        Logger(", Nr: " + PCM.segmentinfos[i].SegNr.PadRight(3), false);
                    if (chkRange.Checked)
                        Logger("[" + PCM.segmentinfos[i].Address + "]", false);
                    if (chkSize.Checked)
                        Logger(", Size: " + PCM.segmentinfos[i].Size.ToString(), false);
                    if (PCM.segmentinfos[i].ExtraInfo != null && PCM.segmentinfos[i].ExtraInfo.Length > 0)
                        Logger(Environment.NewLine + PCM.segmentinfos[i].ExtraInfo, false);

                    if (!txtResult.Text.EndsWith(Environment.NewLine) || chkLogtoFile.Checked)
                        Logger("");
                }
                if (chkCS1.Checked || chkCS2.Checked)
                {
                    Logger("Checksums:");
                    for (int i = 0; i < Segments.Count; i++)
                    {
                        SegmentConfig S = Segments[i];
                        Logger(" " + PCM.segmentinfos[i].Name.PadRight(11), false);
                        if (S.CS1Method != CSMethod_None && chkCS1.Checked)
                        {
                            if (PCM.binfile[i].CS1Address.Address == uint.MaxValue)
                            {
                                Logger(" Checksum1: " + PCM.segmentinfos[i].CS1Calc, false);
                            }
                            else
                            {
                                if (PCM.segmentinfos[i].CS1 == PCM.segmentinfos[i].CS1Calc)
                                {
                                    Logger(" Checksum 1: " + PCM.segmentinfos[i].CS1 + " [OK]", false);
                                }
                                else
                                {
                                    Logger(" Checksum 1: " + PCM.segmentinfos[i].CS1 + ", Calculated: " + PCM.segmentinfos[i].CS1Calc + " [Fail]", false);
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None && chkCS2.Checked)
                        {
                            if (PCM.binfile[i].CS2Address.Address == uint.MaxValue)
                            {
                                Logger(" Checksum2: " + PCM.segmentinfos[i].CS2Calc, false);
                            }
                            else
                            {
                                if (PCM.segmentinfos[i].CS2 == PCM.segmentinfos[i].CS2Calc)
                                {
                                    Logger(" Checksum 2: " + PCM.segmentinfos[i].CS2 + " [OK]", false);
                                }
                                else
                                {
                                    Logger(" Checksum 2:" + PCM.segmentinfos[i].CS2 + ", Calculated: " + PCM.segmentinfos[i].CS2Calc + " [Fail]", false);
                                }
                            }
                        }
                        if (PCM.segmentinfos[i].Stock == "[stock]")
                            LoggerBold(" [stock]", false);
                        else
                            Logger(" " + PCM.segmentinfos[i].Stock, false);
                        if (!txtResult.Text.EndsWith(Environment.NewLine) || chkLogtoFile.Checked)
                            Logger("");
                    }
                }

                refreshSearchedTables();

                if (!InfoOnly)
                {
                    addCheckBoxes();
                    CheckSegmentCompatibility();
                }
                if (checkAutorefreshFileinfo.Checked)
                    RefreshFileInfo();
                if (chkAutoRefreshBadChkFile.Checked)
                    RefreshBadChkFile();
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void GetFileInfo(string FileName, ref PcmFile PCM, bool InfoOnly, bool Show = true)
        {
            try
            {
                if (chkAutodetect.Checked)
                {
                    string ConfFile = Autodetect(PCM);
                    Logger("Autodetect: " + ConfFile);
                    if (ConfFile == "" || ConfFile.Contains(Environment.NewLine))
                    {
                        labelXML.Text = "";
                        XMLFile = "";
                        Segments.Clear();
                    }
                    else
                    {
                        ConfFile = Path.Combine(Application.StartupPath, "XML", ConfFile);
                        if (File.Exists(ConfFile))
                        {
                            frmSegmenList frmSL = new frmSegmenList();
                            frmSL.LoadFile(ConfFile);
                        }
                        else
                        {
                            Logger("XML File not found");
                            labelXML.Text = "";
                            XMLFile = "";
                            Segments.Clear();
                            Logger(Environment.NewLine + Path.GetFileName(FileName));
                            return;
                        }
                    }
                }
                if (Segments == null || Segments.Count == 0)
                {
                    labelXML.Text = "";
                    Logger(Environment.NewLine + Path.GetFileName(FileName));
                    addCheckBoxes();
                    return;
                }
                labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
                Logger(Environment.NewLine + Path.GetFileName(FileName) + " (" + labelXML.Text + ")" + Environment.NewLine);
                PCM.GetSegmentAddresses();
                if (Segments.Count > 0)
                    Logger("Segments:");
                PCM.GetInfo();
                if (chkSearchTables.Checked)
                {
                    TableFinder tableFinder = new TableFinder();
                    tableFinder.searchTables(PCM,false);
                }

                if (Path.GetFileName(XMLFile).StartsWith("e38") || Path.GetFileName(XMLFile).StartsWith("e67"))
                {
                    dtcCodes = new List<dtcCode>();
                    SearchDtc(PCM);
                    refreshDtcList();
                }
                    
                if (PCM.OS == null || PCM.OS == "")
                    LoggerBold("Warning: No OS segment defined, limiting functions");
                if (checkAutorefreshCVNlist.Checked)
                    RefreshCVNlist();
                if (Show)
                    ShowFileInfo(PCM, InfoOnly);
                if (!chkLogtodisplay.Checked)
                {
                    txtResult.AppendText(".");
                }
                RefreshBadCVNlist();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
        private void btnOrgFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile();
            if (FileName.Length > 1)
            {
                txtBaseFile.Text = FileName;
                basefile = new PcmFile(FileName);
                labelBinSize.Text = basefile.fsize.ToString();
                GetFileInfo(txtBaseFile.Text, ref basefile, false);
                txtOS.Text = basefile.OS;
            }
        }

        private void btnModFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile();
            if (FileName.Length > 1)
            {
                txtModifierFile.Text = FileName;
                modfile = new PcmFile(FileName);
                GetFileInfo(txtModifierFile.Text, ref modfile, false);
            }

        }

        private void RefreshFileInfo() 
        { 
            dataFileInfo.DataSource = null;
            Finfosource.DataSource = null;
            Finfosource.DataSource = SegmentList;
            dataFileInfo.DataSource = Finfosource;
            if (SegmentList == null || SegmentList.Count == 0)
                tabFinfo.Text = "File info";
            else
                tabFinfo.Text = "File info (" + SegmentList.Count.ToString() + ")";
        }
        private void RefreshBadChkFile()
        {
            dataBadChkFile.DataSource = null;
            badchkfilesource.DataSource = null;
            badchkfilesource.DataSource = BadChkFileList;
            dataBadChkFile.DataSource = badchkfilesource;
            if (BadChkFileList == null || BadChkFileList.Count == 0)
                tabBadChkFile.Text = "bad chk file";
            else
                tabBadChkFile.Text = "bad chk file (" + BadChkFileList.Count.ToString() + ")";
        }
        private bool ApplyXMLPatch()
        {
            try
            {
                bool isCompatible = false;
                string BinPN="";
                string PrevSegment = "";
                uint ByteCount = 0;
                string[] Parts;
                if (PatchList[0].XmlFile != null)
                {
                    Parts = PatchList[0].XmlFile.Split(',');
                    foreach (string Part in Parts)
                    {
                        if (Part == Path.GetFileName(XMLFile))
                            isCompatible = true;
                    }
                    if (!isCompatible)
                    { 
                        Logger("Incompatible patch");
                        return false;
                    }
                }
                Logger("Applying patch:");
                foreach (XmlPatch xpatch in PatchList)
                {
                    isCompatible = false;
                    uint Addr = 0;
                    string[] OSlist = xpatch.CompatibleOS.Split(',');
                    foreach (string OS in OSlist)
                    {
                        Parts = OS.Split(':');
                        if(Parts[0] == "ALL")
                        {
                            isCompatible = true;
                            if (!HexToUint(Parts[1], out Addr))
                                throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                            Debug.WriteLine("ALL, Addr: " + Parts[1]);
                        }
                        else
                        {
                            if (BinPN == "")
                            { 
                                //Search OS once
                                for (int s = 0; s < Segments.Count; s++)
                                {
                                    string PN = basefile.ReadInfo(basefile.binfile[s].PNaddr);
                                    if (Parts[0] == PN)
                                    {                                        
                                        isCompatible = true;
                                        BinPN = PN;
                                    }
                                }
                            }
                            if (Parts[0] == BinPN)
                            {
                                isCompatible = true;
                                if (!HexToUint(Parts[1], out Addr))
                                    throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                                Debug.WriteLine("OS: " + BinPN + ", Addr: " + Parts[1]);
                            }
                        }
                    }
                    if (isCompatible)
                    {
                        if (xpatch.Description != null && xpatch.Description != "")
                            Logger(xpatch.Description);
                        if (xpatch.Segment != null && xpatch.Segment.Length > 0 && PrevSegment != xpatch.Segment)
                        {
                            PrevSegment = xpatch.Segment;
                            Logger("Segment: " + xpatch.Segment);
                        }
                        bool PatchRule = true; //If there is no rule, apply patch
                        if (xpatch.Rule != null && (xpatch.Rule.Contains(':') || xpatch.Rule.Contains('[')))
                        {
                            Parts = xpatch.Rule.Split(new char[] { ']', '[', ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (Parts.Length == 3)
                            {
                                uint RuleAddr;
                                if (!HexToUint(Parts[0], out RuleAddr))
                                    throw new Exception("Can't decode from HEX: " + Parts[0] + " (" + xpatch.Rule + ")");
                                ushort RuleMask;
                                if (!HexToUshort(Parts[1], out RuleMask))
                                    throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.Rule + ")");
                                ushort RuleValue;
                                if (!HexToUshort(Parts[2].Replace("!",""), out RuleValue))
                                    throw new Exception("Can't decode from HEX: " + Parts[2] + " (" + xpatch.Rule + ")");

                                if (Parts[2].Contains("!"))
                                {
                                    if ((basefile.buf[RuleAddr] & RuleMask) != RuleValue)
                                    {
                                        PatchRule = true;
                                        Logger("Rule match, applying patch");
                                    }
                                    else
                                    {
                                        PatchRule = false;
                                        Logger("Rule doesn't match, skipping patch");
                                    }
                                }
                                else
                                {
                                    if ((basefile.buf[RuleAddr] & RuleMask) == RuleValue)
                                    {
                                        PatchRule = true;
                                        Logger("Rule match, applying patch");
                                    }
                                    else
                                    {
                                        PatchRule = false;
                                        Logger("Rule doesn't match, skipping patch");
                                    }
                                }

                            }
                        }
                        if (PatchRule) { 
                            Debug.WriteLine(Addr.ToString("X") + ":" + xpatch.Data);
                            Parts = xpatch.Data.Split(' ');                        
                            foreach(string Part in Parts)
                            {                            
                                //Actually add patch data:
                                if (Part.Contains("[") || Part.Contains(":"))
                                {
                                    //Set bits / use Mask
                                    byte Origdata = basefile.buf[Addr];
                                    Debug.WriteLine("Set address: " + Addr.ToString("X") + ", old data: " + Origdata.ToString("X"));
                                    string[] dataparts;
                                    dataparts = Part.Split(new char[] { ']', '[', ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    byte Setdata = byte.Parse(dataparts[0], System.Globalization.NumberStyles.HexNumber);
                                    byte Mask = byte.Parse(dataparts[1].Replace("]",""), System.Globalization.NumberStyles.HexNumber);

                                    // Set 1
                                    byte tmpMask = (byte)(Mask & Setdata);
                                    byte Newdata = (byte)(Origdata | tmpMask);

                                    // Set 0
                                    tmpMask = (byte)(Mask & ~Setdata);
                                    Newdata = (byte)(Newdata & ~tmpMask);

                                    Debug.WriteLine("New data: " + Newdata.ToString("X"));
                                    basefile.buf[Addr] = Newdata;
                                }
                                else 
                                { 
                                    //Set byte
                                    basefile.buf[Addr] = Byte.Parse(Part,System.Globalization.NumberStyles.HexNumber);
                                }
                                Addr++;
                                ByteCount++;
                            }
                        }
                    }
                    else
                    {
                        Logger("Patch is not compatible");
                        return false;
                    }
                }
                Logger("Applied: " + ByteCount.ToString() + " Bytes");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
                return false;
            }
            return true;
        }

        private void CompareBlock(byte[] OrgFile, byte[] ModFile, uint Start, uint End, string CurrentSegment, AddressData[] SkipList)
        {
            Logger(" [" + Start.ToString("X") + " - " + End.ToString("X") + "] ");
            uint ModCount = 0;
            XmlPatch xpatch = new XmlPatch();
            bool BlockStarted = false;
            for (uint i = Start; i < End; i++)
            {
                if (OrgFile[i] != ModFile[i])
                {
                    bool SkipAddr = false;
                    for (int s=0; s<SkipList.Length; s++)
                    {
                        if (SkipList[s].Bytes > 0 && i >= SkipList[s].Address && i <= (uint)(SkipList[s].Address + SkipList[s].Bytes - 1))
                        {
                            SkipAddr = true;
                        }
                    }
                    if (SkipAddr)
                    {
                        Debug.WriteLine("Skipping: " + i.ToString("X") + "(" + CurrentSegment +")");
                    }
                    else
                    { 
                        if (!BlockStarted)
                        {
                            //Start new block 
                            xpatch = new XmlPatch();
                            xpatch.XmlFile = Path.GetFileName(XMLFile);
                            xpatch.Data = "";
                            xpatch.Description = "";
                            xpatch.Segment = CurrentSegment;
                            xpatch.Data = "";
                            xpatch.CompatibleOS = txtOS.Text + ":" + i.ToString("X");
                            BlockStarted = true;
                        }
                        else
                            xpatch.Data += " ";

                        xpatch.Data += ModFile[i].ToString("X2");
                        ModCount++;
                        if (ModCount <= numSuppress.Value)
                            Logger(i.ToString("X6") + ": " + OrgFile[i].ToString("X2") + " => " + ModFile[i].ToString("X2"));
                    }

                }
                else if (BlockStarted)
                {
                    //No more differences in this block
                    PatchList.Add(xpatch);
                    BlockStarted = false;
                }

            }
            if (BlockStarted)
            {
                PatchList.Add(xpatch);
            }
            if (ModCount > numSuppress.Value)
            {
                Logger("(Suppressing output)");
                Logger("Total: " + ModCount.ToString() + " differences");
            }

        }
        public void CompareBins()
        {
            try
            {
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                uint fsize2 = (uint)new System.IO.FileInfo(txtModifierFile.Text).Length;
                if (fsize != fsize2)
                {
                    Logger("Files are different size, will not compare!");
                    return;
                }
                basefile = new PcmFile(txtBaseFile.Text);
                modfile = new PcmFile(txtModifierFile.Text);
                if (!checkAppendPatch.Checked || PatchList == null)
                    PatchList = new List<XmlPatch>();
                GetFileInfo(txtBaseFile.Text, ref basefile, false, false);
                GetFileInfo(txtModifierFile.Text, ref modfile, false, false);

                labelBinSize.Text = fsize.ToString();
                if (Segments.Count == 0)
                {
                    Logger("No segments defined, comparing complete file");
                    AddressData[] SkipList = new AddressData[0];
                    CompareBlock(basefile.buf, modfile.buf, 0, (uint)fsize, "", SkipList);
                }
                else if (chkCompareAll.Checked)
                {
                    Logger("Comparing complete file");
                    AddressData[] SkipList = new AddressData[0];
                    CompareBlock(basefile.buf, modfile.buf, 0, (uint)fsize, "", SkipList);
                }
                else
                {
                    for (int Snr = 0; Snr < Segments.Count; Snr++)
                    {
                        if (chkSegments[Snr].Enabled && chkSegments[Snr].Checked)
                        {
                            Logger("Comparing segment " + Segments[Snr].Name, false);
                            for (int p = 0; p < basefile.binfile[Snr].SegmentBlocks.Count; p++)
                            {
                                uint Start = basefile.binfile[Snr].SegmentBlocks[p].Start;
                                uint End = basefile.binfile[Snr].SegmentBlocks[p].End;
                                AddressData[] SkipList = new AddressData[2];
                                if (Segments[Snr].CS1Address != null && Segments[Snr].CS1Address != "")
                                    SkipList[0] = basefile.binfile[Snr].CS1Address;
                                if (Segments[Snr].CS2Address != null && Segments[Snr].CS2Address != "")
                                    SkipList[1] = basefile.binfile[Snr].CS2Address;
                                CompareBlock(basefile.buf, modfile.buf, Start, End, Segments[Snr].Name, SkipList);
                            }
                            Logger("");
                        }
                    }
                }
                if (PatchList.Count > 0)
                    Logger("Created patch for OS: " + txtOS.Text);
                else
                    Logger("No differences found");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SuppressAfter = (uint)numSuppress.Value;
            Properties.Settings.Default.Save();
            if (txtBaseFile.Text.Length == 0 || txtModifierFile.Text.Length == 0)
                return;
            if (Segments != null && Segments.Count > 0)
            { 
                labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            }
            if (txtOS.Text.Length == 0)
            {
                txtOS.Text = "ALL";
            }
            txtResult.Text = "";

            CompareBins();
            RefreshDatagrid();
        }

        private void SavePatch(string Description)
        {
            try
            {
                if (PatchList == null || PatchList.Count == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                /*if (Description.Length < 1 && PatchList[0].Description == null)
                {
                    Logger("Supply patch description");
                    return;
                }*/
                string FileName = SelectSaveFile("XMLPATCH files (*.xmlpatch)|*.xmlpatch|All files (*.*)|*.*");
                if (FileName.Length < 1)
                    return;
                Logger("Saving to file: " + Path.GetFileName(FileName), false);
                if (PatchList[0].Description == null)
                {
                    XmlPatch xpatch = PatchList[0];
                    xpatch.Description = Description;
                    PatchList[0] = xpatch;
                }

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    writer.Serialize(stream, PatchList);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void SaveBin()
        {
            try
            {
                if (basefile == null || basefile.buf == null | basefile.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string FileName = SelectSaveFile();
                if (FileName.Length == 0)
                    return;
                Logger("Saving to file: " + FileName);
                WriteBinToFile(FileName, basefile.buf);
                Logger("Done.");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }

        private void btnSaveBin_Click(object sender, EventArgs e)
        {
            SaveBin();
        }

        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            if (chkLogtoFile.Checked)
            {
                logwriter.Write("\\b " + LogText +" \\b0");
                if (NewLine)
                    logwriter.Write("\\par" + Environment.NewLine);
            }
            if (chkLogtodisplay.Checked)
            {
                //txtResult.Focus();
                //int Start = txtResult.Text.Length;
                //txtResult.AppendText(LogText);
                //txtResult.Select(Start, LogText.Length);
                /*txtResult.Select(Start, 1);*/
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
                //txtResult.SelectedRtf = @"{\rtf1\ansi \b " + LogText + "\b0  ";
                txtResult.AppendText(LogText);
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
                if (NewLine)
                    txtResult.AppendText(Environment.NewLine);
                //Application.DoEvents();
            }
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            if (chkLogtoFile.Checked)
            {
                logwriter.Write(LogText.Replace("\\","\\\\"));
                if (NewLine)
                    logwriter.Write("\\par" + Environment.NewLine);
            }
            if (chkLogtodisplay.Checked)
            { 
                //txtResult.Focus();
                int Start = txtResult.Text.Length;
                //txtResult.AppendText(LogText);
                //txtResult.Select(Start, LogText.Length);
                /*txtResult.Select(Start  , 1);
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);*/
                txtResult.AppendText(LogText);
                if (NewLine)
                    txtResult.AppendText(Environment.NewLine);
                //Application.DoEvents();
            }
            Application.DoEvents();
        }

        private void btnCheckSums_Click(object sender, EventArgs e)
        {
            if (Segments != null && Segments.Count > 0)
            { 
                FixCheckSums();
            }
        }
        private bool FixCheckSums()
        {
            bool NeedFix = false;
            try
            {
                Logger("Fixing Checksums:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(S.Name);
                    if (S.Eeprom)
                    {
                        string Ret = GmEeprom.FixEepromKey(basefile.buf);
                        if (Ret.Contains("Fixed"))
                            NeedFix = true;
                        Logger(Ret);
                    }
                    else
                    {
                        if (S.CS1Method != CSMethod_None)
                        {
                            uint CS1 = 0;
                            uint CS1Calc = CalculateChecksum(basefile.buf, basefile.binfile[i].CS1Address, basefile.binfile[i].CS1Blocks, basefile.binfile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, basefile.binfile[i].CS1Address.Bytes, S.CS1SwapBytes);
                            if (basefile.binfile[i].CS1Address.Address < uint.MaxValue)
                            { 
                                if (basefile.binfile[i].CS1Address.Bytes == 1)
                                {
                                    CS1 = basefile.buf[basefile.binfile[i].CS1Address.Address];
                                }
                                else if (basefile.binfile[i].CS1Address.Bytes == 2)
                                {
                                    CS1 = BEToUint16(basefile.buf, basefile.binfile[i].CS1Address.Address);
                                }
                                else if (basefile.binfile[i].CS1Address.Bytes == 4)
                                {
                                    CS1 = BEToUint32(basefile.buf, basefile.binfile[i].CS1Address.Address);
                                }
                            }
                            if (CS1 == CS1Calc)
                                Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                            else
                            {
                                if (basefile.binfile[i].CS1Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (basefile.binfile[i].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (basefile.binfile[i].CS1Address.Bytes * 2).ToString();
                                    Logger(" Checksum 1: " + CS1Calc.ToString(hexdigits) + " [Not saved]");
                                }
                                else
                                {
                                    if (basefile.binfile[i].CS1Address.Bytes == 1)
                                        basefile.buf[basefile.binfile[i].CS1Address.Address] = (byte)CS1Calc;
                                    else if (basefile.binfile[i].CS1Address.Bytes == 2)
                                    {
                                        basefile.buf[basefile.binfile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        basefile.buf[basefile.binfile[i].CS1Address.Address + 1] = (byte)(CS1Calc & 0xFF);
                                    }
                                    else if (basefile.binfile[i].CS1Address.Bytes == 4)
                                    {
                                        basefile.buf[basefile.binfile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF000000) >> 24);
                                        basefile.buf[basefile.binfile[i].CS1Address.Address + 1] = (byte)((CS1Calc & 0xFF0000) >> 16);
                                        basefile.buf[basefile.binfile[i].CS1Address.Address + 2] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        basefile.buf[basefile.binfile[i].CS1Address.Address + 3] = (byte)(CS1Calc & 0xFF);

                                    }
                                    Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                    NeedFix = true;
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None)
                        {
                            uint CS2 = 0;
                            uint CS2Calc = CalculateChecksum(basefile.buf, basefile.binfile[i].CS2Address, basefile.binfile[i].CS2Blocks, basefile.binfile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, basefile.binfile[i].CS2Address.Bytes, S.CS2SwapBytes);
                            if (basefile.binfile[i].CS2Address.Address < uint.MaxValue)
                            { 
                                if (basefile.binfile[i].CS2Address.Bytes == 1)
                                {
                                    CS2 = basefile.buf[basefile.binfile[i].CS2Address.Address];
                                }
                                else if (basefile.binfile[i].CS2Address.Bytes == 2)
                                {
                                    CS2 = BEToUint16(basefile.buf, basefile.binfile[i].CS2Address.Address);
                                }
                                else if (basefile.binfile[i].CS2Address.Bytes == 4)
                                {
                                    CS2 = BEToUint32(basefile.buf, basefile.binfile[i].CS2Address.Address);
                                }
                            }
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                            else
                            {
                                if (basefile.binfile[i].CS2Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (basefile.binfile[i].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (basefile.binfile[i].CS2Address.Bytes * 2).ToString();
                                    Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    if (basefile.binfile[i].CS2Address.Bytes == 1)
                                        basefile.buf[basefile.binfile[i].CS2Address.Address] = (byte)CS2Calc;
                                    else if (basefile.binfile[i].CS2Address.Bytes == 2)
                                    {
                                        basefile.buf[basefile.binfile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        basefile.buf[basefile.binfile[i].CS2Address.Address + 1] = (byte)(CS2Calc & 0xFF);
                                    }
                                    else if (basefile.binfile[i].CS2Address.Bytes == 4)
                                    {
                                        basefile.buf[basefile.binfile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF000000) >> 24);
                                        basefile.buf[basefile.binfile[i].CS2Address.Address + 1] = (byte)((CS2Calc & 0xFF0000) >> 16);
                                        basefile.buf[basefile.binfile[i].CS2Address.Address + 2] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        basefile.buf[basefile.binfile[i].CS2Address.Address + 3] = (byte)(CS2Calc & 0xFF);

                                    }
                                    Logger(" Checksum 2: " + CS2.ToString("X") + " => " + CS2Calc.ToString("X4") + " [Fixed]");
                                    NeedFix = true;
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
            return NeedFix;
        }

        private bool CheckRule(DetectRule DR, PcmFile PCM)
        {
            try { 
            
                UInt64 Data = 0;
                uint Addr = 0;
                if (DR.address == "filesize")
                {
                    Data = (UInt64)new FileInfo(PCM.FileName).Length;
                }
                else
                {
                    string[] Parts = DR.address.Split(':');
                    HexToUint(Parts[0].Replace("@", ""),out Addr);
                    if (DR.address.StartsWith("@"))
                        Addr = BEToUint32(PCM.buf, Addr);
                    if (Parts[0].EndsWith("@"))
                        Addr = (uint)PCM.buf.Length - Addr;
                    if (Parts.Length == 1)
                        Data = BEToUint16(PCM.buf, Addr);
                    else
                    {
                        if (Parts[1] == "1")
                            Data = (uint)PCM.buf[Addr];
                        if (Parts[1] == "2")
                            Data = (uint)BEToUint16(PCM.buf, Addr);
                        if (Parts[1] == "4")
                            Data = BEToUint32(PCM.buf, Addr);
                        if (Parts[1] == "8")
                            Data = BEToUint64(PCM.buf, Addr);

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

        private string Autodetect(PcmFile PCM)
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
            for (int x=0; x < XmlList.Count;x++)
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
                for (int d=0; d < DRL.Count; d++)
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

        private void btnLoadFolder_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            frmF.btnOK.Text = "OK";
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                if (!chkLogtodisplay.Checked)
                    txtResult.AppendText("Writing file info to logfile");
                string dstFolder = frmF.labelCustomdst.Text;
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    PcmFile PCM = new PcmFile(FileName);
                    GetFileInfo(FileName, ref PCM, true);
                }
                if (!chkLogtodisplay.Checked)
                    txtResult.AppendText(Environment.NewLine + "[Done]" + Environment.NewLine);
                else
                    Logger("[Done]");
            }

        }

        private void btnSaveFileInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("RTF files (*.rtf)|*.rtf|All files (*.*)|*.*");
                if (FileName.Length > 1)
                {
                    StreamWriter sw = new StreamWriter(FileName);
                    sw.WriteLine(txtResult.Rtf);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }

        }

        private void btnApplypatch_Click(object sender, EventArgs e)
        {
            if (basefile == null || PatchList == null)
            {
                Logger("Nothing to do");
                return;
            }
            ApplyXMLPatch();
            btnCheckSums_Click(sender, e);
        }

        private void ExtractTable(uint Start, uint End, string[] OSlist, string MaskText)
        {
            try
            {
                XmlPatch xpatch = new XmlPatch();
                xpatch.CompatibleOS = OSlist[0] + ":" + Start.ToString("X"); 
                for (int i=1;i < OSlist.Length; i++)
                    xpatch.CompatibleOS += "," + OSlist[i] + ":" + Start.ToString("X");
                xpatch.XmlFile = Path.GetFileName(XMLFile);
                xpatch.Description = txtExtractDescription.Text;
                Logger("Extracting " + Start.ToString("X") + " - " + End.ToString("X"));
                for (uint i = Start; i <= End; i++)
                {
                    if (i > Start)
                        xpatch.Data += " ";
                    if (MaskText.ToLower() == "ff" || MaskText == "") 
                    { 
                        xpatch.Data += basefile.buf[i].ToString("X2");
                    }
                    else
                    {
                        byte Mask = byte.Parse(MaskText, System.Globalization.NumberStyles.HexNumber);
                        xpatch.Data += (basefile.buf[i] & Mask).ToString("X2") + "[" + MaskText +"]";
                    }
                }
                if (PatchList == null)
                    PatchList = new List<XmlPatch>();
                Logger("[OK]");
                PatchList.Add(xpatch);
                RefreshDatagrid();

            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnExtract_Click(object sender, EventArgs e)
        {
            uint Start;
            uint End;
            string MaskText = "";
            if (txtBaseFile.Text.Length == 0)
                return;
            basefile = new PcmFile(txtBaseFile.Text);
            GetFileInfo(txtBaseFile.Text, ref basefile, true, false);
            if (txtCompatibleOS.Text.Length == 0)
                txtCompatibleOS.Text = basefile.OS;
            if (txtCompatibleOS.Text.Length == 0)
                txtCompatibleOS.Text = "ALL";
            string[] OSlist = txtCompatibleOS.Text.Split(',');
            string[] blocks = txtExtractRange.Text.Split(',');
            for (int b=0; b< blocks.Length; b++) 
            {
                MaskText = "";
                string block = blocks[b];
                if (block.Contains("["))
                {
                    string[] AddrMask = block.Split('[');
                    block = AddrMask[0];
                    MaskText = AddrMask[1].Replace("]", "");
                }
                if (block.Contains(":"))
                {
                    string[] StartEnd = block.Split(':');
                    if (!HexToUint(StartEnd[0], out Start))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[0]);
                        return;
                    }
                    if (!HexToUint(StartEnd[1], out End))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[1]);
                        return;
                    }
                    End += Start - 1;
                }
                else
                {
                    if (!block.Contains("-"))
                    {
                        Logger("Supply address range, for example 200-220 or 200:4");
                        return;
                    }
                    string[] StartEnd = block.Split('-');
                    if (!HexToUint(StartEnd[0], out Start))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[0]);
                        return;
                    }
                    if (!HexToUint(StartEnd[1], out End))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[1]);
                        return;
                    }
                }
                ExtractTable(Start, End, OSlist, MaskText);
            }

        }

        private void RefreshDatagrid()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = PatchList;
            dataPatch.DataSource = null;
            dataPatch.DataSource = bindingSource;
            if (PatchList == null || PatchList.Count == 0)
            { 
                tabPatch.Text = "Patch editor";
            }
            else 
            { 
                tabPatch.Text = "Patch editor (" + PatchList.Count.ToString() +")";
            }
        }

        private void btnManualPatch_Click(object sender, EventArgs e)
        {
            frmManualPatch frmM = new frmManualPatch();
            if (PatchList != null && PatchList.Count > 0)
            {
                string[] Oslist = PatchList[0].CompatibleOS.Split(',');
                foreach (string os in Oslist)
                {
                    if (frmM.txtCompOS.Text.Length > 0)
                        frmM.txtCompOS.Text += ",";
                    string[] Parts = os.Split(':');
                    frmM.txtCompOS.Text += Parts[0] + ":";
                    frmM.txtXML.Text = PatchList[0].XmlFile;
                    frmM.txtDescription.Text = PatchList[0].Description;
                }
            }
            else
            {
                if (txtOS.Text.Length > 0)
                    frmM.txtCompOS.Text = txtOS.Text + ":";
                else
                    frmM.txtCompOS.Text = "ALL:";
                if (XMLFile != null && XMLFile.Length > 0)
                    frmM.txtXML.Text = Path.GetFileName(XMLFile);
            }
            
            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                if (PatchList == null)
                    PatchList = new List<XmlPatch>();
                XmlPatch XP = new XmlPatch();
                XP.Description = frmM.txtDescription.Text;
                XP.Segment = frmM.txtSegment.Text;
                XP.XmlFile = frmM.txtXML.Text;
                XP.CompatibleOS = frmM.txtCompOS.Text;
                XP.Data = frmM.txtData.Text;
                if (frmM.txtReadAddr.Text.Length > 0)
                {
                    XP.Rule = frmM.txtReadAddr.Text + "[" + frmM.txtMask.Text + "]";
                    if (frmM.chkNOT.Checked)
                        XP.Rule += "!";
                    XP.Rule += frmM.txtValue.Text;
                }
                PatchList.Add(XP);
                RefreshDatagrid();
            }
        }

        private void btnBinLoadPatch_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DebugOn = chkDebug.Checked;
            Properties.Settings.Default.Save();
            if (chkDebug.Checked)
            {
                RichTextBoxTraceListener tbtl = new RichTextBoxTraceListener(txtDebug);
                Debug.Listeners.Add(tbtl);
            }
            else
            {
                Debug.Listeners.Clear();
            }

        }

        public void EditPatchRow()
        {
            if (dataPatch.SelectedRows.Count == 0 && dataPatch.SelectedCells.Count == 0)
                return;
            if (dataPatch.SelectedRows.Count == 0)
                dataPatch.Rows[dataPatch.SelectedCells[0].RowIndex].Selected = true;
            frmManualPatch frmM = new frmManualPatch();
            if (dataPatch.CurrentRow.Cells[0].Value != null)
                frmM.txtDescription.Text = dataPatch.CurrentRow.Cells[0].Value.ToString();
            if (dataPatch.CurrentRow.Cells[1].Value != null)
                frmM.txtXML.Text = dataPatch.CurrentRow.Cells[1].Value.ToString();
            if (dataPatch.CurrentRow.Cells[2].Value != null)
                frmM.txtSegment.Text = dataPatch.CurrentRow.Cells[2].Value.ToString();
            if (dataPatch.CurrentRow.Cells[3].Value != null)
                frmM.txtCompOS.Text = dataPatch.CurrentRow.Cells[3].Value.ToString();
            if (dataPatch.CurrentRow.Cells[4].Value != null)
                frmM.txtData.Text = dataPatch.CurrentRow.Cells[4].Value.ToString();
            if (dataPatch.CurrentRow.Cells[5].Value != null && dataPatch.CurrentRow.Cells[5].Value.ToString().Contains(":"))
            {
                string[] Parts = dataPatch.CurrentRow.Cells[5].Value.ToString().Split(':');
                if (Parts.Length == 3)
                {
                    frmM.txtReadAddr.Text = Parts[0];
                    frmM.txtMask.Text = Parts[1];
                    frmM.txtValue.Text = Parts[2];
                }
            }
            if (dataPatch.CurrentRow.Cells[6].Value != null)
                frmM.txtHelpFile.Text = dataPatch.CurrentRow.Cells[6].Value.ToString();

            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                dataPatch.CurrentRow.Cells[0].Value = frmM.txtDescription.Text;
                dataPatch.CurrentRow.Cells[1].Value = frmM.txtXML.Text;
                dataPatch.CurrentRow.Cells[2].Value = frmM.txtSegment.Text;
                dataPatch.CurrentRow.Cells[3].Value = frmM.txtCompOS.Text;
                dataPatch.CurrentRow.Cells[4].Value = frmM.txtData.Text;
                if (frmM.txtReadAddr.Text.Length > 0)
                {
                    dataPatch.CurrentRow.Cells[5].Value = frmM.txtReadAddr.Text + ":" + frmM.txtMask.Text + ":" + frmM.txtValue.Text;
                }
                dataPatch.CurrentRow.Cells[6].Value = frmM.txtHelpFile.Text;
            }
            frmM.Dispose();

        }

        private void dataPatch_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditPatchRow();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditPatchRow();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataPatch.SelectedRows.Count == 0)
                return;
            dataPatch.Rows.Remove(dataPatch.CurrentRow);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int row = dataPatch.CurrentRow.Index; 
            if (row == 0)
                return;
            XmlPatch CurrentP = PatchList[row];
            PatchList.RemoveAt(row);
            PatchList.Insert(row - 1, CurrentP);
            RefreshDatagrid();
            dataPatch.CurrentCell = dataPatch.Rows[row - 1].Cells[0];
            dataPatch.Rows[row - 1].Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int row = dataPatch.CurrentRow.Index;
            if (row >= dataPatch.Rows.Count - 2)
                return;
            XmlPatch CurrentP = PatchList[row];
            PatchList.RemoveAt(row);
            PatchList.Insert(row + 1, CurrentP);
            RefreshDatagrid();
            dataPatch.CurrentCell = dataPatch.Rows[row + 1].Cells[0];
            dataPatch.Rows[row + 1].Selected = true;
        }

        private void loadXMLfile()
        {
            string FileName = SelectFile("Select XML file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            frmSegmenList frmSL = new frmSegmenList();
            frmSL.LoadFile(FileName);
            frmSL.Dispose();
            labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            addCheckBoxes();

        }
        private void loadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadXMLfile();
        }

        private void setupSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSL != null && frmSL.Visible)
            {
                frmSL.BringToFront();
                return;
            }
            frmSL = new frmSegmenList();
            frmSL.InitMe();
            if (frmSL.ShowDialog() == DialogResult.OK)
            {
                //addCheckBoxes();
            }

        }

        private void autodetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAutodetect frmAD = new frmAutodetect();
            frmAD.Show();
            frmAD.InitMe();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDatagrid();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshDatagrid();

        }

        public void LoadPatch()
        {
            try
            {
                string FileName = SelectFile("Select patch file", "XML patch files (*.xmlpatch)|*.xmlpatch|PATCH files (*.patch)|*.patch|ALL files(*.*)|*.*");
                if (FileName.Length > 1)
                {
                    labelPatchname.Text = FileName;
                    Logger("Loading file: " + FileName);
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                    PatchList = (List<XmlPatch>)reader.Deserialize(file);
                    file.Close();
                    if (PatchList.Count > 0)
                    {
                        Logger("Description: " + PatchList[0].Description);
                        string[] OsList = PatchList[0].CompatibleOS.Split(',');
                        string CompOS = "";
                        foreach (string OS in OsList)
                        {
                            if (CompOS != "")
                                CompOS += ",";
                            string[] Parts = OS.Split(':');
                            CompOS += Parts[0];
                        }
                        Logger("For OS: " + CompOS);
                    }
                    btnApplypatch.Enabled = true;
                    RefreshDatagrid();
                    Logger("[OK]");
                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePatch(txtPatchDescription.Text);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try 
            {
                if (dataPatch.Rows[dataPatch.CurrentRow.Index].Cells[6].Value == null)
                    return;
                string FileName = dataPatch.Rows[dataPatch.CurrentRow.Index].Cells[6].Value.ToString();
                FileName = Path.Combine(Application.StartupPath, "Patches" , FileName);
                Process.Start(FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void buttonAddtoStock_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = false;
                int counter = 0;
                for (int i=0;i < ListCVN.Count; i++)
                {
                    CVN stock = ListCVN[i];
                    counter++;
                    if (CheckStockCVN(stock.PN,stock.Ver,stock.SegmentNr,stock.cvn , false) != "[stock]")
                    {
                        //Add if not already in list
                        StockCVN.Add(stock);
                        isNew = true;
                        Debug.WriteLine(stock.PN + " " + stock.Ver + " cvn: " + stock.cvn + " added");
                    }
                    else
                    {
                        Debug.WriteLine(stock.PN + " " + stock.Ver + " cvn: " + stock.cvn + " Already in list");
                    }
                }
                if (counter == 0)
                {
                    Logger("No CVN defined");
                    return;
                }
                if (!isNew)
                {
                    Logger("All segments already in stock list");
                    return;
                }
                Logger("Saving file stockcvn.xml");
                string FileName = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                    writer.Serialize(stream, StockCVN);
                    stream.Close();
                }
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void stockCVNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML frmE = new frmEditXML();
            frmE.LoadStockCVN();
            frmE.Show();
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePatch(txtPatchDescription.Text);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshDatagrid();

        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            RefreshDatagrid();

        }
        private void RefreshBadCVNlist()
        {
            dataGridBadCvn.DataSource = null;
            badCvnSource.DataSource = null;
            badCvnSource.DataSource = BadCvnList;
            dataGridBadCvn.DataSource = badCvnSource;
            if (BadCvnList != null && BadCvnList.Count > 0)
                tabBadCvn.Text = "Mismatch CVN (" + BadCvnList.Count.ToString() + ")";
            else
                tabBadCvn.Text = "Mismatch CVN";
        }

        private void ClearBadCVNlist()
        {
            BadCvnList = new List<CVN>();
            RefreshBadCVNlist();
        }

        private void RefreshCVNlist()
        {
            dataCVN.DataSource = null;
            CvnSource.DataSource = null;
            CvnSource.DataSource = ListCVN;
            dataCVN.DataSource = CvnSource;
            if (ListCVN != null && ListCVN.Count > 0)
                tabCVN.Text = "CVN (" + ListCVN.Count.ToString() + ")";
            else
                tabCVN.Text = "CVN";
        }

        private void ClearCVNlist()
        {
            ListCVN = new List<CVN>();
            RefreshCVNlist();
        }

        private void btnClearCVN_Click(object sender, EventArgs e)
        {
            ClearCVNlist();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SegmentList.Clear();
            RefreshFileInfo();
        }

        private void btnSaveCSV_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataFileInfo.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataFileInfo.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataFileInfo.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataFileInfo.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataFileInfo.Rows[r].Cells[i].Value != null)
                            row += dataFileInfo.Rows[r].Cells[i].Value.ToString();
                    }
                    row = row.Replace(Environment.NewLine, ":");
                    row = row.Replace(",", " ");
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private void SaveSegmentList()
        {
            Logger("Saving file extractedsegments.xml", false);
            string FileName = Path.Combine(Application.StartupPath, "Segments", "extractedsegments.xml");
            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SwapSegment>));
                writer.Serialize(stream, SwapSegments);
                stream.Close();
            }
            Logger(" [OK]");

        }
        private string SegmentFileName(string FnameStart, string Extension)
        {
            string FileName = FnameStart + Extension;
            FileName = FileName.Replace("?", "_");
            if (!Directory.Exists(Path.GetDirectoryName(FnameStart)))
                Directory.CreateDirectory(Path.GetDirectoryName(FnameStart));
            if (radioReplace.Checked)
            {
                for (int x=0;x<SwapSegments.Count;x++)
                {
                    if (FileName == Application.StartupPath + SwapSegments[x].FileName)
                    {                        
                        SwapSegments.RemoveAt(x);
                    }
                }
                return FileName;
            }

            if (!File.Exists(FileName))
            {
                return FileName;
            }

            if (radioSkip.Checked)
            {
                Logger("Skipping duplicate file: " + FileName);
                return "";
            }

            //radioRename checked
            uint Fnr = 0;
            while (File.Exists(FileName))
            {
                Fnr++;
                FileName = FnameStart + "(" + Fnr.ToString() + ")" + Extension;
            }
            return FileName;
        }

        private void ExtractSegments(PcmFile PCM, string Descr, bool AllSegments, string dstFolder)
        {            
            if (PCM.segmentinfos == null)
            {
                Logger("no segments defined");
                return;
            }
            try
            {
                for (int s=0;s<PCM.segmentinfos.Length;s++)
                {
                    if (AllSegments || chkExtractSegments[s].Checked)
                    {
                        string FileName;
                        if (dstFolder.Length > 0)
                        {
                            string FnameStart = Path.Combine(dstFolder, PCM.segmentinfos[s].PN.PadLeft(8,'0'));
                            FileName = SegmentFileName(FnameStart, ".bin");
                        }
                        else
                        {
                            string FnameStart = Path.Combine(Application.StartupPath, "Segments", PCM.OS, PCM.segmentinfos[s].SegNr, PCM.segmentinfos[s].Name + "-" + PCM.segmentinfos[s].PN + PCM.segmentinfos[s].Ver);
                            FileName = SegmentFileName(FnameStart, ".binsegment");
                            if (FileName.Length > 0)
                            { 
                                SwapSegment swapsegment = new SwapSegment();
                                swapsegment.Description = Descr;
                                swapsegment.FileName = FileName.Replace(Application.StartupPath,"");
                                swapsegment.OS = PCM.OS;
                                swapsegment.PN = PCM.segmentinfos[s].PN;
                                swapsegment.Ver = PCM.segmentinfos[s].Ver;
                                swapsegment.SegIndex = s;
                                swapsegment.SegNr = PCM.segmentinfos[s].SegNr;
                                swapsegment.Cvn = PCM.segmentinfos[s].cvn;
                                if (PCM.segmentinfos[s].SwapAddress != "")
                                {
                                    swapsegment.Address = PCM.segmentinfos[s].SwapAddress;
                                    swapsegment.Size = PCM.segmentinfos[s].SwapSize;
                                }
                                else
                                {
                                    swapsegment.Address = PCM.segmentinfos[s].Address;
                                    swapsegment.Size = PCM.segmentinfos[s].Size;
                                }
                                swapsegment.Stock = PCM.segmentinfos[s].Stock;
                                swapsegment.XmlFile = PCM.segmentinfos[s].XmlFile;
                                if (PCM.segmentinfos[s].Name == "OS")
                                {
                                    for (int x=0;x< PCM.segmentinfos.Length;x++)
                                    {
                                        if (x>0)
                                        {
                                            swapsegment.SegmentSizes += ";";
                                            swapsegment.SegmentAddresses += ";";
                                        }
                                        if (PCM.segmentinfos[x].SwapAddress != "")
                                        {
                                            swapsegment.SegmentSizes += PCM.segmentinfos[x].Name + ":" + PCM.segmentinfos[x].SwapSize;
                                            swapsegment.SegmentAddresses += PCM.segmentinfos[x].Name + ":" + PCM.segmentinfos[x].SwapAddress;
                                        }
                                        else
                                        { 
                                            swapsegment.SegmentSizes += PCM.segmentinfos[x].Name + ":" + PCM.segmentinfos[x].Size;
                                            swapsegment.SegmentAddresses += PCM.segmentinfos[x].Name + ":" + PCM.segmentinfos[x].Address;
                                        }
                                    }
                                }
                                SwapSegments.Add(swapsegment);
                            }
                        }
                        if (FileName.Length > 0) 
                        {
                            
                            if (PCM.segmentinfos[s].SwapAddress != "")
                            {
                                Logger("Writing " + PCM.segmentinfos[s].Name + " to file: " + FileName + ", size: " + PCM.segmentinfos[s].SwapSize);
                                WriteSegmentToFile(FileName, PCM.binfile[s].SwapBlocks, PCM.buf);
                            }
                            else
                            { 
                                Logger("Writing " + PCM.segmentinfos[s].Name + " to file: " + FileName + ", size: " + PCM.segmentinfos[s].Size);
                                WriteSegmentToFile(FileName, PCM.binfile[s].SegmentBlocks, PCM.buf);
                            }
                            StreamWriter sw = new StreamWriter(FileName + ".txt");
                            sw.WriteLine(Descr);
                            sw.Close();
                            Logger("[OK]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
        private void btnExtractSegments_Click(object sender, EventArgs e)
        {
            if (txtBaseFile.Text.Length == 0)
            {
                Logger("No file loaded");
                return;
            }
            if (txtSegmentDescription.Text.Length == 0)
                txtSegmentDescription.Text = Path.GetFileName(basefile.FileName).Replace(".bin", "");
            ExtractSegments(basefile, txtExtractDescription.Text, false,"");
            SaveSegmentList();
        }

        private void btnExtractSegmentsFolder_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            frmF.labelCustomdst.Visible = true;
            frmF.btnCustomdst.Visible = true;
            frmF.btnOK.Text = "Extract!";
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                if (!chkLogtodisplay.Checked)
                    txtResult.AppendText("Extracting...");
                string dstFolder = frmF.labelCustomdst.Text;
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    PcmFile PCM = new PcmFile(FileName);
                    GetFileInfo(FileName, ref PCM, true,checkExtractShowinfo.Checked);
                    ExtractSegments(PCM, Path.GetFileName(FileName).Replace(".bin", ""), true, dstFolder);
                }
                if (!chkLogtodisplay.Checked)
                    txtResult.AppendText(Environment.NewLine + "Segments extracted" + Environment.NewLine);
                else
                    Logger("Segments extracted");
                SaveSegmentList();
            }
        }

        private void btnSwapSegments_Click(object sender, EventArgs e)
        {
            if (basefile == null)
            {
                Logger("No file loaded");
                return;
            }
            if (basefile.OS == null || basefile.OS == "")
            {
                Logger("No OS segment defined");
                return;
            }
            frmSwapSegmentList frmSw = new frmSwapSegmentList();
            frmSw.LoadSegmentList(ref basefile);
            if (frmSw.ShowDialog(this) == DialogResult.OK)
            {
                basefile = frmSw.PCM;
                FixCheckSums();
                Logger("");
                Logger("Segment(s) swapped and checksums fixed (you can save BIN now)");
            }
            frmSw.Dispose();
        }

        private void FixFileChecksum(string FileName)
        {
            try
            {
                basefile = new PcmFile(FileName);
                GetFileInfo(FileName, ref basefile, true, false);
                if (FixCheckSums())
                {
                    Logger("Saving file: " + FileName);
                    WriteBinToFile(FileName, basefile.buf);
                    Logger("[OK]");
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                if (!chkLogtodisplay.Checked)
                    txtResult.AppendText("Fixing checksums...");
                for (int i= 0; i< frmF.listFiles.CheckedItems.Count; i++)
                {
                    string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    FixFileChecksum(FileName);
                }
                if (!chkLogtodisplay.Checked)
                    txtResult.AppendText(Environment.NewLine +  "[Checksums fixed]" + Environment.NewLine);
                else
                    Logger("[Checksums fixed]");
            }
        }

        private void btnRefreshFileinfo_Click(object sender, EventArgs e)
        {
            RefreshFileInfo();
        }

        private void btnRefreshCvnList_Click(object sender, EventArgs e)
        {
            RefreshCVNlist();
        }

        private void checkAutorefreshFileinfo_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutorefreshFileinfo = checkAutorefreshFileinfo.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkAutorefreshCVNlist_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutorefreshCVNlist = checkAutorefreshCVNlist.Checked;
            Properties.Settings.Default.Save();
        }

         private void chkLogtoFile_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogtoFile.Checked)
            {
                if (!File.Exists(logFile))
                {
                    logwriter = new StreamWriter(logFile);
                    logwriter.WriteLine(@"{\rtf1\ansi\deff0\nouicompat{\fonttbl{\f0\fnil\fcharset0 Lucida Console;}{\f1\fnil\fcharset0 Lucida Console;}}" + Environment.NewLine);
                    logwriter.WriteLine(@"{\*\generator Riched20 10.0.18362}\viewkind4\uc1" + Environment.NewLine);
                    logwriter.WriteLine(@"\pard\sa200\sl276\slmult1\f0\fs16\lang11" + Environment.NewLine);
                }
                else
                {
                    logwriter = new StreamWriter(logFile, true);
                }
                txtResult.AppendText("Logging to file: " + logFile + Environment.NewLine);
            }
            else
            {
                logwriter.WriteLine("}");
                logwriter.Close();
                txtResult.AppendText("Logfile closed" + Environment.NewLine);
            }
        }

        private void btnSaveCSaddresses_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            try 
            { 
                using (StreamWriter writetext = new StreamWriter(FileName))
                {
                    string row = "";
                    for (int i = 0; i < listCSAddresses.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        row += listCSAddresses.Columns[i].Text;
                    }
                    writetext.WriteLine(row);
                    for (int r = 0; r < listCSAddresses.Items.Count; r++)
                    {
                        row = "";
                        for (int i = 0; i < listCSAddresses.Columns.Count; i++)
                        {
                            if (i > 0)
                                row += ";";
                            if (listCSAddresses.Items[r].SubItems[i].Text != null)
                                row += listCSAddresses.Items[r].SubItems[i].Text;
                        }
                        writetext.WriteLine(row);
                    }
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnClearCSAddresses_Click(object sender, EventArgs e)
        {
            listCSAddresses.Items.Clear();
            tabCsAddress.Text = "Gm-v6 info";
        }

        private void chkAutodetect_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkAutodetect.Checked)
            {
                loadXMLfile();
            }
        }

        private void dataFileInfo_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try 
            {    
                string folder = Path.GetDirectoryName(dataFileInfo.Rows[e.RowIndex].Cells[1].Value.ToString());
                string FileName = Path.GetFileName(dataFileInfo.Rows[e.RowIndex].Cells[1].Value.ToString());
                OpenFolderAndSelectItem(folder, FileName);
                //Process.Start(folder);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnSaveCsvBadChkFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataBadChkFile.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataBadChkFile.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataBadChkFile.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataBadChkFile.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataBadChkFile.Rows[r].Cells[i].Value != null)
                            row += dataBadChkFile.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private void btnRefreshBadChkFile_Click(object sender, EventArgs e)
        {
            RefreshBadChkFile();
        }


        private void dataBadChkFile_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string folder = Path.GetDirectoryName(dataBadChkFile.Rows[e.RowIndex].Cells[1].Value.ToString());
                string FileName = Path.GetFileName(dataBadChkFile.Rows[e.RowIndex].Cells[1].Value.ToString());
                OpenFolderAndSelectItem(folder, FileName);
                //Process.Start(folder);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmSearchText frmS = new frmSearchText();
            frmS.Show();
            frmS.initMe(txtResult);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmSearchText frmS = new frmSearchText();
            frmS.Show();
            frmS.initMe(txtDebug);
        }

        private void btnSaveDebug_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("RTF files (*.rtf)|*.rtf|All files (*.*)|*.*");
                if (FileName.Length > 1)
                {
                    StreamWriter sw = new StreamWriter(FileName);
                    sw.WriteLine(txtDebug.Rtf);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }

        }

        private void btnClearBadchkFile_Click(object sender, EventArgs e)
        {
            BadChkFileList = new List<SegmentInfo>();
            RefreshBadChkFile();
        }

        private void btnSearchTables_Click(object sender, EventArgs e)
        {
        }

        private void editTableSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSearchTables frmST = new frmSearchTables();
            frmST.Show(this);
            if (tableSearchFile != null && tableSearchFile.Length > 0)
                frmST.LoadFile(tableSearchFile);
            else
                frmST.LoadConfig();

        }

        private void btnSaveSearchedTables_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataGridSearchedTables.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataGridSearchedTables.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataGridSearchedTables.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataGridSearchedTables.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataGridSearchedTables.Rows[r].Cells[i].Value != null)
                            row += dataGridSearchedTables.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private void btnClearSearchedTables_Click(object sender, EventArgs e)
        {
            tableSearchResult = new List<TableSearchResult>();
            tableSearchResultNoFilters = new List<TableSearchResult>();
            refreshSearchedTables();
        }

        private void dataGridSearchedTables_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string folder = Path.GetDirectoryName(dataGridSearchedTables.Rows[e.RowIndex].Cells[1].Value.ToString());
                string FileName = Path.GetFileName(dataGridSearchedTables.Rows[e.RowIndex].Cells[1].Value.ToString());
                OpenFolderAndSelectItem(folder, FileName);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnGetPidList_Click(object sender, EventArgs e)
        {
            try
            {
                PidSearch pidSearch = new PidSearch(basefile);
                if (pidSearch.pidList == null)
                {
                    Logger("PIDs not found");
                    return;
                }
                for (int i = 0; i < pidSearch.pidList.Count; i++)
                {
                    pidList.Add(pidSearch.pidList[i]);
                }
                refreshPidList();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnClearPidList_Click(object sender, EventArgs e)
        {
            pidList = new List<PidSearch.PID>();
            refreshPidList();
        }

        private void btnSavePidList_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataGridPIDlist.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataGridPIDlist.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataGridPIDlist.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataGridPIDlist.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataGridPIDlist.Rows[r].Cells[i].Value != null)
                            row += dataGridPIDlist.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private void chkTableSearchNoFilters_CheckedChanged(object sender, EventArgs e)
        {
            refreshSearchedTables();
        }

        private void btnClearBadCvn_Click(object sender, EventArgs e)
        {
            ClearBadCVNlist();
        }

        private void BtnRefreshBadCvn_Click(object sender, EventArgs e)
        {
            RefreshBadCVNlist();
        }

        private void ShowCustomSearchResult(uint addr)
        {
            uint startAddr;
            string[] searhParts = txtCustomSearchString.Text.Split(' ');

            Logger("Found at address: " + addr.ToString("X"));
            lastCustomSearchResult = addr;
            string dataBuf = "";
            if (addr > 5)
                startAddr = addr - 5;
            else
                startAddr = 0;

            for (uint a = startAddr; a < addr; a++)
            {
                dataBuf += basefile.buf[a].ToString("X2") + " ";
            }
            Logger(dataBuf, false);
            dataBuf = "";
            for (uint a = addr; a < addr + searhParts.Length; a++)
            {
                dataBuf += basefile.buf[a].ToString("X2") + " ";
            }
            LoggerBold(dataBuf, false);
            dataBuf = "";
            uint endAddr = 0;
            if ((addr + searhParts.Length + 5) > basefile.fsize)
                endAddr = basefile.fsize;
            else
                endAddr = (uint)(addr + searhParts.Length + 5);
            for (uint a = (uint)(addr + searhParts.Length); a < endAddr; a++)
            {
                dataBuf += basefile.buf[a].ToString("X2") + " ";
            }
            Logger(dataBuf);
        }
        private void btnCustomSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkCustomTableSearch.Checked)
                {
                    TableFinder tableFinder = new TableFinder();
                    tableFinder.searchTables(basefile,false, txtCustomSearchString.Text);
                    refreshSearchedTables();
                    return;
                }
                uint startAddr = 0;
                if (txtCustomSearchStartAddress.Text.Length == 0 || !HexToUint(txtCustomSearchStartAddress.Text, out startAddr))
                    startAddr = 0;
                uint addr = searchBytes(basefile, txtCustomSearchString.Text, startAddr, basefile.fsize);
                if (addr == uint.MaxValue)
                    Logger("Not found");
                else
                {
                    ShowCustomSearchResult(addr);
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnCustomSearchNext_Click(object sender, EventArgs e)
        {
            try
            {
                uint addr = searchBytes(basefile, txtCustomSearchString.Text, lastCustomSearchResult + 1, basefile.fsize);
                if (addr == uint.MaxValue)
                    Logger("Not found");
                else
                {
                    ShowCustomSearchResult(addr);
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnCustomFindAll_Click(object sender, EventArgs e)
        {
            try
            {
                uint startAddr = 0;
                if (txtCustomSearchStartAddress.Text.Length == 0 || !HexToUint(txtCustomSearchStartAddress.Text, out startAddr))
                    startAddr = 0;
                while (startAddr < uint.MaxValue)
                {
                    uint addr = searchBytes(basefile, txtCustomSearchString.Text, startAddr, basefile.fsize);
                    if (addr < uint.MaxValue)
                    {
                        ShowCustomSearchResult(addr);
                        startAddr = lastCustomSearchResult + 1;
                    }
                    else
                    {
                        Logger("Done");
                        return;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void chkCustomTableSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustomTableSearch.Checked)
            {
                txtCustomSearchStartAddress.Enabled = false;
                btnCustomFindAll.Enabled = false;
                btnCustomSearchNext.Enabled = false;
            }
            else
            {
                txtCustomSearchStartAddress.Enabled = true;
                btnCustomFindAll.Enabled = true;
                btnCustomSearchNext.Enabled = true;
            }
        }

        private void fileTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML frmEX = new frmEditXML();
            frmEX.LoadFileTypes();
            frmEX.Show();

        }

        private void btnCrossTableSearch_Click(object sender, EventArgs e)
        {
            if (basefile == null || modfile == null)
            {
                Logger("No files selected");
                return;
            }
            Logger("Cross table search...");
            tableSearchResult = new List<TableSearchResult>();
            TableFinder tableFinder = new TableFinder();
            tableFinder.searchTables(basefile,false);
            tableFinder.searchTables(modfile, true,"",(int)numCrossVariation.Value);
            refreshSearchedTables();
            Logger("Done");
        }

        private void btnSaveDecCsv_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataFileInfo.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataFileInfo.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataFileInfo.Rows.Count - 1); r++)
                {
                    string val = "";
                    uint valDec = 0;
                    row = "";
                    for (int i = 0; i < dataFileInfo.Columns.Count; i++)
                    {
                        if (dataFileInfo.Rows[r].Cells[i].Value != null)
                            val = dataFileInfo.Rows[r].Cells[i].Value.ToString();
                        if (i > 0)
                            row += ";";
                        if (i == 3 || i == 4)  //Address (block)
                        {
                            string[] aparts = val.Split(',');
                            val = "";
                            for (int a= 0; a<aparts.Length; a++)
                            {
                                if (a > 0)
                                    val += ":";
                                string[] addresses = aparts[a].Split('-');
                                if (addresses.Length > 1)
                                {
                                    if (!HexToUint(addresses[0],out valDec))
                                    {
                                        Debug.WriteLine("Can't convert from hex: " + addresses[0]);
                                        break;
                                    }
                                    val += valDec.ToString() + " - ";
                                    if (!HexToUint(addresses[1], out valDec))
                                    {
                                        Debug.WriteLine("Can't convert from hex: " + addresses[1]);
                                        break;
                                    }
                                    val += valDec.ToString();
                                }
                            }

                        }
                        else if (i> 4 && i < 12)
                        {
                            if (HexToUint(val, out valDec))
                                val = valDec.ToString();
                        }
                        row += val;
                    }
                    row = row.Replace(",", " ");
                    row = row.Replace(Environment.NewLine, ":");

                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private void btnClearDTC_Click(object sender, EventArgs e)
        {
            refreshDtcList();
        }

        private void btnSaveCsvDTC_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataGridDTC.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataGridDTC.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataGridDTC.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataGridDTC.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataGridDTC.Rows[r].Cells[i].Value != null)
                            row += dataGridDTC.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        //Search GM e38/e67 DTC codes
        private void SearchDtc(PcmFile PCM)
        {
            try
            {
                if (PCM.OSSegment == -1)
                {
                    Logger("DTC search: No OS segment??");
                    return;
                }
                if (PCM.diagSegment == -1)
                {
                    Logger("DTC search: No Diagnostic segment??");
                    return;
                }

                //Get codes from OS segment:
                string searchStr = "00 00 00 10 00 11";
                for (int b = 0; b < PCM.binfile[PCM.OSSegment].SegmentBlocks.Count; b++)
                {
                    uint startAddr = searchBytes(PCM,searchStr , PCM.binfile[PCM.OSSegment].SegmentBlocks[b].Start, PCM.binfile[PCM.OSSegment].SegmentBlocks[b].End);
                    if (startAddr < uint.MaxValue)
                    {
                        searchStr = "94 21 ff";
                        uint endAddr = searchBytes(PCM, searchStr, startAddr + 6, PCM.binfile[PCM.OSSegment].SegmentBlocks[b].End);
                        if (endAddr == uint.MaxValue)
                        {
                            LoggerBold("DTC Search: SearchString: " + searchStr + " not found");
                            return;
                        }
                        for (uint addr = startAddr + 2; addr < endAddr; addr += 2)
                        {
                            dtcCode dtc = new dtcCode();
                            dtc.codeAddrInt = addr;
                            dtc.CodeAddr = addr.ToString("X8");
                            dtc.codeInt = BEToUint16(PCM.buf, addr);
                            
                            string codeTmp = dtc.codeInt.ToString("X4");
                            if (codeTmp.StartsWith("5"))
                            {
                                dtc.Code = "C1" + codeTmp.Substring(1);
                            }
                            else if (codeTmp.StartsWith("C"))
                            {
                                dtc.Code = "U0" + codeTmp.Substring(1);
                            }
                            else if (codeTmp.StartsWith("D"))
                            {
                                dtc.Code = codeTmp;
                            }
                            else
                            {
                                dtc.Code = "P" + codeTmp;
                            }
                            dtcCodes.Add(dtc);
                        }
                        break;
                    }
                }

                int dtcCount = dtcCodes.Count;
                for (int b = 0; b < PCM.binfile[PCM.diagSegment].SegmentBlocks.Count; b++)
                {
                    for (uint addr = PCM.binfile[PCM.diagSegment].SegmentBlocks[b].Start; addr < PCM.binfile[PCM.diagSegment].SegmentBlocks[b].End; addr++)
                    {
                        bool valuesOK = true; 
                        if (PCM.buf[addr] <= 7 && PCM.buf[addr + dtcCount] == 0xFF) //DTC code status is 0-7, FF after table 
                        {
                            valuesOK = true;
                            for (uint a=addr; a < addr + dtcCount-1;a++)
                            {
                                if (PCM.buf[a] > 7 )
                                {
                                    //This is not DTC table, it can only have values 0-7
                                    valuesOK = false;
                                    break;
                                }
                            }
                            if (valuesOK)
                            {
                                //We found the table!
                                int dtcNr = 0;
                                for (uint addr2 = addr; addr2 < addr + dtcCount; addr2++)
                                {
                                    if (BEToUint16(PCM.buf, addr2) == 0xFF)
                                    {
                                        return;
                                    }
                                    dtcCode dtc = dtcCodes[dtcNr];
                                    dtc.statusAddrInt = addr2;
                                    dtc.StatusAddr = addr2.ToString("X8");
                                    dtc.Status = PCM.buf[addr2];

                                    if (dtc.Status == 0) dtc.StatusTxt = "MIL and reporting off";
                                    if (dtc.Status == 1) dtc.StatusTxt = "Type A/no MIL";
                                    if (dtc.Status == 2) dtc.StatusTxt = "Type B/no MIL";
                                    if (dtc.Status == 3) dtc.StatusTxt = "Type C/no MIL";
                                    if (dtc.Status == 4) dtc.StatusTxt = "Not reported/no MIL";
                                    if (dtc.Status == 5) dtc.StatusTxt = "Type A/MIL";
                                    if (dtc.Status == 6) dtc.StatusTxt = "Type B/MIL";
                                    if (dtc.Status == 7) dtc.StatusTxt = "Type C/MIL";

                                    dtcCodes.RemoveAt(dtcNr);
                                    dtcCodes.Insert(dtcNr, dtc);
                                    dtcNr++;
                                }
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
                var line = frame.GetFileLineNumber(); 
                LoggerBold("DTC search, line " + line + ": " +  ex.Message);
                return;
            }
        }
    }
}


