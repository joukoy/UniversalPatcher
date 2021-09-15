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
//using System.Threading;
//using System.Threading.Tasks;
using System.Diagnostics;
using UniversalPatcher.Properties;
using System.Drawing.Text;
using System.ComponentModel.Design;
using MathParserTK;
using System.Configuration;
using System.Xml.Linq;
using System.Globalization;
using System.Xml;

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

        private frmSegmenList frmSL;
        private PcmFile basefile;
        private PcmFile modfile;
        private CheckBox[] chkSegments;
        private CheckBox[] chkExtractSegments;
        //public string pcmConfigFile;
        private string LastXML = "";
        private BindingSource bindingSource = new BindingSource();
        private BindingSource CvnSource = new BindingSource();
        private BindingSource badCvnSource = new BindingSource();
        private BindingSource Finfosource = new BindingSource();
        private BindingSource badchkfilesource = new BindingSource();
        private BindingSource searchedTablesBindingSource = new BindingSource();
        private BindingSource pidListBindingSource = new BindingSource();
        private BindingSource dtcBindingSource = new BindingSource();
        private BindingSource tableSeekBindingSource = new BindingSource();
        private BindingSource patchesBindingSource = new BindingSource();
        private List<PidSearch.PID> pidList = new List<PidSearch.PID>();
        private BindingList<FoundTable> filteredCategories = new BindingList<FoundTable>();
        private BindingSource categoryBindingSource = new BindingSource();


        private uint lastCustomSearchResult = 0;
        private string logFile;
        StreamWriter logwriter;

        private string cvnSortBy = "";
        private int cvnSortIndex = 0;
        SortOrder cvnStrSortOrder = SortOrder.Ascending;

        private void FrmPatcher_Load(object sender, EventArgs e)
        {
            basefile = new PcmFile();

            if (Properties.Settings.Default.MainWindowPersistence)
            {
                rememberWindowSizeToolStripMenuItem.Checked = true;
                if (Properties.Settings.Default.MainWindowSize.Width > 0 || Properties.Settings.Default.MainWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.MainWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.MainWindowLocation;
                    this.Size = Properties.Settings.Default.MainWindowSize;
                }
                if (Properties.Settings.Default.PatcherSplitterDistance > 0)
                    splitPatcher.SplitterDistance = Properties.Settings.Default.PatcherSplitterDistance;
                else
                    splitPatcher.SplitterDistance = 138;
            }
            else
            {
                rememberWindowSizeToolStripMenuItem.Checked = false;
            }

            //Set default values for Tuner datagrid, if not set previously:
            TableData tdTmp = new TableData();
            if (Properties.Settings.Default.ConfigModeColumnOrder == null || Properties.Settings.Default.ConfigModeColumnOrder.Length == 0)
            {
                string cOrder = "";
                foreach (var prop in tdTmp.GetType().GetProperties())
                {
                    cOrder += prop.Name + ",";
                }
                Properties.Settings.Default.ConfigModeColumnOrder = cOrder.Trim(',');
            }

            if (Properties.Settings.Default.TunerModeColumns == null || Properties.Settings.Default.TunerModeColumns.Length == 0)
            {
                Properties.Settings.Default.ConfigModeColumnOrder = "id,TableName,Category,Units,Columns,Rows,TableDescription";
            }

            if (Properties.Settings.Default.ConfigModeColumnWidth == null || Properties.Settings.Default.ConfigModeColumnWidth.Length == 0)
            {
                Properties.Settings.Default.ConfigModeColumnWidth = "35,200,234,135,100,100,100,100,100,114,100,100,100,100,60,46,100,100,100,100,100,100,100";
            }
            if (Properties.Settings.Default.TunerModeColumnWidth == null || Properties.Settings.Default.TunerModeColumnWidth.Length == 0)
            {
                Properties.Settings.Default.TunerModeColumnWidth = "35,100,237,135,100,100,100,100,100,114,100,100,100,100,60,46,100,100,100,100,100,493,100,";

            }

            Application.DoEvents();

            addCheckBoxes();
            numSuppress.Value = Properties.Settings.Default.SuppressAfter;
            if (numSuppress.Value == 0)
                numSuppress.Value = 10;
            logFile = Path.Combine(Application.StartupPath, "Log", "universalpatcher" + DateTime.Now.ToString("_yyyyMMdd_hhmm") + ".rtf");

            chkDebug.Checked = Properties.Settings.Default.DebugOn;
            checkAutorefreshCVNlist.Checked = Properties.Settings.Default.AutorefreshCVNlist;
            checkAutorefreshFileinfo.Checked = Properties.Settings.Default.AutorefreshFileinfo;
            disableTunerAutloadConfigToolStripMenuItem.Checked = Properties.Settings.Default.disableTunerAutoloadSettings;

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

            setWorkingMode();
            dataCVN.ColumnHeaderMouseClick += DataCVN_ColumnHeaderMouseClick;
            tabFakeCvn.Enter += TabFakeCvn_Enter;
            numFakeCvnBytes.KeyPress += numFakeCvnBytesFromEnd_ValueChanged;
        }

        private void TabFakeCvn_Enter(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.CvnPopupAccepted)
            {
                string msg = "Avoiding emission checks can be illegal at some areas." + Environment.NewLine;
                msg += "Use for educational and research purposes only, solely at your own risk." + Environment.NewLine;
                msg += "By accepting this disclaimer you agree to take full reponsibility for" + Environment.NewLine;
                msg += "any damage or legal actions that can result by using this program" + Environment.NewLine;
                msg += "outside of its educational purpose." + Environment.NewLine + Environment.NewLine;
                msg += "Do you agree?";
                DialogResult dialogResult = MessageBox.Show(msg, "Disclaimer", MessageBoxButtons.YesNo);
                if  (dialogResult == DialogResult.No)
                {
                    tabFunction.TabPages.Remove(tabFakeCvn);
                }
                else
                {
                    Properties.Settings.Default.CvnPopupAccepted = true;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void DataCVN_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cvnSortBy = dataCVN.Columns[e.ColumnIndex].Name;
                cvnSortIndex = e.ColumnIndex;
                cvnStrSortOrder = getSortOrder(cvnSortIndex);
                filterCVN();
            }

        }

        private void filterCVN()
        {
            List<CVN> compareList = new List<CVN>();
            if (cvnStrSortOrder == SortOrder.Ascending)
                compareList = ListCVN.OrderBy(x => typeof(CVN).GetProperty(cvnSortBy).GetValue(x, null)).ToList();
            else
                compareList = ListCVN.OrderByDescending(x => typeof(CVN).GetProperty(cvnSortBy).GetValue(x, null)).ToList();
            CvnSource.DataSource = compareList;
            dataCVN.Columns[cvnSortIndex].HeaderCell.SortGlyphDirection = cvnStrSortOrder;
            //RefreshCVNlist();
        }

        private SortOrder getSortOrder(int columnIndex)
        {
            try
            {
                if (dataCVN.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                    || dataCVN.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                {
                    dataCVN.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    return SortOrder.Ascending;
                }
                else
                {
                    dataCVN.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    return SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return SortOrder.Ascending;
        }

        private void setWorkingMode()
        {
            try
            {
                int patcherMode = Properties.Settings.Default.WorkingMode;
                switch (patcherMode)
                {
                    case 0:
                        touristToolStripMenuItem.Checked = true;
                        basicToolStripMenuItem.Checked = false;
                        advancedToolStripMenuItem.Checked = false;
                        break;
                    case 1:
                        touristToolStripMenuItem.Checked = false;
                        basicToolStripMenuItem.Checked = true;
                        advancedToolStripMenuItem.Checked = false;
                        break;
                    case 2:
                        touristToolStripMenuItem.Checked = false;
                        basicToolStripMenuItem.Checked = false;
                        advancedToolStripMenuItem.Checked = true;
                        break;

                }
                if (patcherMode == 0)   //Tourist mode
                {
                    tabControl1.TabPages.Remove(tabFinfo);
                    tabFunction.Visible = false;
                    //tabControl1.Location = new Point(0,60);
                    //tabControl1.Height = this.Height - 100;
                    splitPatcher.Panel1Collapsed = true;
                    splitPatcher.Panel1.Hide();
                    chkLogtodisplay.Visible = false;
                    chkLogtoFile.Visible = false;
                    labelShowMax.Visible = false;
                    labelPatchRows.Visible = false;
                    numSuppress.Visible = false;
                    toolStripMenuItem1.Visible = false;

                    btnSearch.Visible = false;
                    btnSaveFileInfo.Visible = false;
                }
                else
                {
                    if (!tabControl1.TabPages.Contains(tabFinfo))
                        tabControl1.TabPages.Add(tabFinfo);
                    tabFunction.Visible = true;
                    //tabControl1.Location = new Point(0, 191);
                    //tabControl1.Height = this.Height - 230;
                    splitPatcher.Panel1Collapsed = false;
                    splitPatcher.Panel1.Show();
                    chkLogtodisplay.Visible = true;
                    chkLogtoFile.Visible = true;
                    labelShowMax.Visible = true;
                    labelPatchRows.Visible = true;
                    numSuppress.Visible = true;
                    toolStripMenuItem1.Visible = true;
                    btnSearch.Visible = true;
                    btnSaveFileInfo.Visible = true;
                }
                if (patcherMode == 2)   //Advanced
                {
                    if (!tabFunction.TabPages.Contains(tabCreate))
                        tabFunction.TabPages.Add(tabCreate);
                    if (!tabFunction.TabPages.Contains(tabExtract))
                        tabFunction.TabPages.Add(tabExtract);
                    if (!tabFunction.TabPages.Contains(tabExtractSegments))
                        tabFunction.TabPages.Add(tabExtractSegments);
                    if (!tabFunction.TabPages.Contains(tabChecksumUtil))
                        tabFunction.TabPages.Add(tabChecksumUtil);

                    string[] args = Environment.GetCommandLineArgs();
                    if (args.Length > 3 && args[3].Contains("fakecvn"))
                    {
                        if (!tabFunction.TabPages.Contains(tabFakeCvn))
                            tabFunction.TabPages.Add(tabFakeCvn);
                    }
                    else
                    {
                        tabFunction.TabPages.Remove(tabFakeCvn);
                    }


                    if (!tabControl1.TabPages.Contains(tabDebug))
                        tabControl1.TabPages.Add(tabDebug);
                    if (!tabControl1.TabPages.Contains(tabPatch))
                        tabControl1.TabPages.Add(tabPatch);
                    if (!tabControl1.TabPages.Contains(tabCVN))
                        tabControl1.TabPages.Add(tabCVN);
                    if (!tabControl1.TabPages.Contains(tabBadCvn))
                        tabControl1.TabPages.Add(tabBadCvn);
                    if (!tabControl1.TabPages.Contains(tabCsAddress))
                        tabControl1.TabPages.Add(tabCsAddress);
                    if (!tabControl1.TabPages.Contains(tabBadChkFile))
                        tabControl1.TabPages.Add(tabBadChkFile);
                    if (!tabControl1.TabPages.Contains(tabSearchedTables))
                        tabControl1.TabPages.Add(tabSearchedTables);
                    if (!tabControl1.TabPages.Contains(tabPIDList))
                        tabControl1.TabPages.Add(tabPIDList);
                    if (!tabControl1.TabPages.Contains(tabDTC))
                        tabControl1.TabPages.Add(tabDTC);
                    if (!tabControl1.TabPages.Contains(tabTableSeek))
                        tabControl1.TabPages.Add(tabTableSeek);

                    groupSearch.Visible = true;

                    setupSegmentsToolStripMenuItem.Visible = true;
                    autodetectToolStripMenuItem.Visible = true;
                    stockCVNToolStripMenuItem.Visible = true;
                    editTableSearchToolStripMenuItem.Visible = true;
                    fileTypesToolStripMenuItem.Visible = true;
                    dTCSearchToolStripMenuItem.Visible = true;
                    pIDSearchToolStripMenuItem.Visible = true;
                    tableSeekToolStripMenuItem.Visible = true;
                    segmentSeekToolStripMenuItem.Visible = true;
                    oBD2CodesToolStripMenuItem.Visible = true;
                    rememberWindowSizeToolStripMenuItem.Visible = true;
                    disableTunerAutloadConfigToolStripMenuItem.Visible = true;

                } 
                else //Tourist or Basic
                {
                    tabFunction.TabPages.Remove(tabCreate);
                    tabFunction.TabPages.Remove(tabExtract);
                    tabFunction.TabPages.Remove(tabExtractSegments);
                    tabFunction.TabPages.Remove(tabChecksumUtil);

                    tabFunction.TabPages.Remove(tabFakeCvn);


                    tabControl1.TabPages.Remove(tabDebug);
                    tabControl1.TabPages.Remove(tabPatch);
                    tabControl1.TabPages.Remove(tabCVN);
                    tabControl1.TabPages.Remove(tabBadCvn);
                    tabControl1.TabPages.Remove(tabCsAddress);
                    tabControl1.TabPages.Remove(tabBadChkFile);
                    tabControl1.TabPages.Remove(tabSearchedTables);
                    tabControl1.TabPages.Remove(tabPIDList);
                    tabControl1.TabPages.Remove(tabDTC);
                    tabControl1.TabPages.Remove(tabTableSeek);

                    groupSearch.Visible = false;

                    setupSegmentsToolStripMenuItem.Visible = false;
                    autodetectToolStripMenuItem.Visible = false;
                    stockCVNToolStripMenuItem.Visible = false;
                    editTableSearchToolStripMenuItem.Visible = false;
                    fileTypesToolStripMenuItem.Visible = false;
                    dTCSearchToolStripMenuItem.Visible = false;
                    pIDSearchToolStripMenuItem.Visible = false;
                    tableSeekToolStripMenuItem.Visible = false;
                    segmentSeekToolStripMenuItem.Visible = false;
                    oBD2CodesToolStripMenuItem.Visible = false;
                    rememberWindowSizeToolStripMenuItem.Visible = false;
                    disableTunerAutloadConfigToolStripMenuItem.Visible = false;
                   
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
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
            dataGridDTC.DataSource = null;
            dtcBindingSource.DataSource = basefile.dtcCodes;
            if (basefile.dtcCodes.Count == 0)
                tabDTC.Text = "DTC";
            else
                tabDTC.Text = "DTC (" + basefile.dtcCodes.Count.ToString() + ")";
            dataGridDTC.DataSource = dtcBindingSource;
            dataGridDTC.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }
        public void refreshTableSeek()
        {
            tableSeekBindingSource.DataSource = null;
            dataGridTableSeek.DataSource = null;
            tableSeekBindingSource.DataSource = basefile.foundTables;
            if (basefile.foundTables.Count == 0)
                tabTableSeek.Text = "Table Seek";
            else
                tabTableSeek.Text = "Table Seek (" + basefile.foundTables.Count.ToString() + ")";
            dataGridTableSeek.DataSource = tableSeekBindingSource;
            //dataGridTableSeek.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            comboTableCategory.DataSource = null;
            categoryBindingSource.DataSource = null;
            basefile.tableCategories.Sort();
            categoryBindingSource.DataSource = basefile.tableCategories;
            comboTableCategory.DataSource = categoryBindingSource;
            dataGridTableSeek.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void FrmPatcher_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (chkLogtoFile.Checked)
            {
                chkLogtoFile.Checked = false;
                Application.DoEvents();
            }
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.MainWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.MainWindowLocation = this.Location;
                    Properties.Settings.Default.MainWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.MainWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.MainWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.PatcherSplitterDistance = splitPatcher.SplitterDistance;
                Properties.Settings.Default.Save();
            }
        }
        public void addCheckBoxes()
        {
            if (LastXML == basefile.configFileFullName && chkSegments != null && chkSegments.Length == basefile.Segments.Count)
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
            chkSegments = new CheckBox[basefile.Segments.Count];
            chkExtractSegments = new CheckBox[basefile.Segments.Count];
            for (int s = 0; s < basefile.Segments.Count; s++)
            {
                CheckBox chk = new CheckBox();
                tabCreate.Controls.Add(chk);
                chk.Location = new Point(Left, 80);
                chk.Text = basefile.Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                if (!chk.Text.ToLower().Contains("eeprom"))
                    chk.Checked = true;
                chkSegments[s] = chk;

                chk = new CheckBox();
                tabExtractSegments.Controls.Add(chk);
                chk.Location = new Point(Left, 80);
                chk.Text = basefile.Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                chk.Checked = true;
                chkExtractSegments[s] = chk;

                Left += chk.Width + 5;

            }
            LastXML = basefile.configFileFullName;

        }

        private void CheckSegmentCompatibility()
        {
            if ( txtBaseFile.Text == "" || txtModifierFile.Text == "")
                return;

            labelXML.Text = Path.GetFileName(basefile.configFileFullName) + " (v " + basefile.Segments[0].Version + ")";
            for (int s = 0; s < basefile.Segments.Count; s++)
            {
                string BasePN = basefile.ReadInfo(basefile.segmentAddressDatas[s].PNaddr);
                string ModPN = modfile.ReadInfo(modfile.segmentAddressDatas[s].PNaddr);
                string BaseVer = basefile.ReadInfo(basefile.segmentAddressDatas[s].VerAddr);
                string ModVer = modfile.ReadInfo(modfile.segmentAddressDatas[s].VerAddr);

                if (chkForceCompare.Checked)
                {
                    if (basefile.segmentinfos[s].Size == modfile.segmentinfos[s].Size)
                        chkSegments[s].Enabled = true;
                    else
                        chkSegments[s].Enabled = false;
                }
                else
                {
                    if (basefile.Segments[s].Missing || BasePN != ModPN || BaseVer != ModVer)
                    {
                        Logger(basefile.Segments[s].Name.PadLeft(11) + " differ: " + BasePN.ToString().PadRight(8) + " " + BaseVer + " <> " + ModPN.ToString().PadRight(8) + " " + ModVer);
                        chkSegments[s].Enabled = false;
                    }
                    else
                    {
                        chkSegments[s].Enabled = true;
                    }
                }
            }
        }
        private void getPidList()
        {
            try
            {
                pidList = new List<PidSearch.PID>();
                if (chkSearchPids.Checked)
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
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
            refreshPidList();
        }

        private void ShowFileInfo(PcmFile PCM, bool InfoOnly)
        {
            try
            {
                if (PCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                { 
                    var item = new ListViewItem(PCM.OS);
                    if (PCM.segmentAddressDatas[0].CS1Address.Address == uint.MaxValue)
                        item.SubItems.Add("");
                    else
                        item.SubItems.Add(PCM.segmentAddressDatas[0].CS1Address.Address.ToString("X"));
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
                for (int i = 0; i < PCM.Segments.Count; i++)
                {
                    SegmentConfig S = PCM.Segments[i];
                    if (S.Hidden || S.Missing)
                        continue;
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
                    for (int i = 0; i < PCM.Segments.Count; i++)
                    {
                        SegmentConfig S = PCM.Segments[i];
                        if (S.Missing)
                            continue;
                        Logger(" " + PCM.segmentinfos[i].Name.PadRight(11), false);
                        if (S.CS1Method != CSMethod_None && chkCS1.Checked)
                        {
                            if (PCM.segmentAddressDatas[i].CS1Address.Address == uint.MaxValue)
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
                            if (PCM.segmentAddressDatas[i].CS2Address.Address == uint.MaxValue)
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

        private void GetFileInfo(string FileName, ref PcmFile PCM, bool InfoOnly, bool Show = true)
        {
            try
            {
                if (PCM.Segments == null || PCM.Segments.Count == 0)
                {
                    labelXML.Text = "";
                    Logger(Environment.NewLine + Path.GetFileName(FileName));
                    addCheckBoxes();
                    return;
                }
                labelXML.Text = PCM.configFile + " (v " + PCM.Segments[0].Version + ")";
                Logger(Environment.NewLine + Path.GetFileName(FileName) + " (" + labelXML.Text + ")" + Environment.NewLine);
                if (PCM.Segments.Count > 0)
                    Logger("Segments:");
                if (chkSearchTables.Checked)
                {
                    TableFinder tableFinder = new TableFinder();
                    tableFinder.searchTables(PCM,false);
                }

                if (PCM.OS == null || PCM.OS == "")
                    LoggerBold("Warning: No OS defined, limiting functions");
                if (checkAutorefreshCVNlist.Checked)
                    RefreshCVNlist();
                if (Show)
                    ShowFileInfo(PCM, InfoOnly);
                if (!chkLogtodisplay.Checked)
                {
                    txtResult.AppendText(".");
                }
                RefreshBadCVNlist();

                basefile.dtcCodes = new List<dtcCode>();
                DtcSearch DS = new DtcSearch();
                string dtcSearchResult = "";
                if (chkSearchDTC.Checked)
                {
                    dtcSearchResult = DS.searchDtc(PCM);
                    Logger(dtcSearchResult);
                }
                refreshDtcList();

                TableSeek TS = new TableSeek();
                if (chkTableSeek.Checked)
                {
                    Logger("Seeking tables...",false);
                    Logger(TS.seekTables(PCM));
                }
                refreshTableSeek();
                                    
                getPidList();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void openBaseFile()
        {
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                string fileName = SelectFile();
                if (fileName.Length > 1)
                {
                    basefile.tableCategories = new List<string>(); //Clear list
                    txtBaseFile.Text = fileName;
                    basefile = new PcmFile(fileName, chkAutodetect.Checked, basefile.configFileFullName);
                    labelBinSize.Text = basefile.fsize.ToString();
                    GetFileInfo(txtBaseFile.Text, ref basefile, false);
                    this.Text = "Universal Patcher - " + Path.GetFileName(fileName);
                    txtOS.Text = basefile.OS;
                    clearFakeCVN();
                }
                timer.Stop();
                Debug.WriteLine("Time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }
        private void btnOrgFile_Click(object sender, EventArgs e)
        {
            openBaseFile();
        }

        private void btnModFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile();
            if (FileName.Length > 1)
            {
                txtModifierFile.Text = FileName;
                modfile = new PcmFile(FileName, chkAutodetect.Checked,basefile.configFileFullName);
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


        private void CompareBlock(byte[] OrgFile, byte[] ModFile, uint Start, uint End, string CurrentSegment, List<AddressData> SkipList)
        {
            Logger(" [" + Start.ToString("X") + " - " + End.ToString("X") + "] ");
            uint ModCount = 0;
            XmlPatch xpatch = new XmlPatch();
            bool BlockStarted = false;
            for (uint addr = Start; addr < End; addr++)
            {
                if (OrgFile[addr] != ModFile[addr])
                {
                    bool SkipAddr = false;
                    for (int s=0; s<SkipList.Count; s++)
                    {
                        if (SkipList[s].Bytes > 0 && addr >= SkipList[s].Address && addr <= (uint)(SkipList[s].Address + SkipList[s].Bytes - 1))
                        {
                            SkipAddr = true;
                        }
                    }
                    if (SkipAddr)
                    {
                        Debug.WriteLine("Skipping: " + addr.ToString("X") + "(" + CurrentSegment +")");
                    }
                    else
                    { 
                        if (!BlockStarted)
                        {
                            //Start new block 
                            xpatch = new XmlPatch();
                            xpatch.XmlFile = Path.GetFileName(basefile.configFileFullName);
                            xpatch.Data = "";
                            xpatch.Description = "";
                            xpatch.Segment = CurrentSegment;
                            xpatch.Data = "";
                            xpatch.CompatibleOS = txtOS.Text + ":" + addr.ToString("X");
                            BlockStarted = true;
                        }
                        else
                            xpatch.Data += " ";

                        xpatch.Data += ModFile[addr].ToString("X2");
                        ModCount++;
                        if (ModCount <= numSuppress.Value)
                            Logger(addr.ToString("X6") + ": " + OrgFile[addr].ToString("X2") + " => " + ModFile[addr].ToString("X2"));
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
                basefile = new PcmFile(txtBaseFile.Text,chkAutodetect.Checked,basefile.configFileFullName);
                modfile = new PcmFile(txtModifierFile.Text, chkAutodetect.Checked, basefile.configFileFullName);
                if (!checkAppendPatch.Checked || PatchList == null)
                    PatchList = new List<XmlPatch>();
                GetFileInfo(txtBaseFile.Text, ref basefile, false, false);
                GetFileInfo(txtModifierFile.Text, ref modfile, false, false);

                List<AddressData> SkipList = new List<AddressData>();

                labelBinSize.Text = fsize.ToString();
                if (basefile.Segments.Count == 0)
                {
                    Logger("No segments defined, comparing complete file");
                    CompareBlock(basefile.buf, modfile.buf, 0, (uint)fsize, "", SkipList);
                }
                else if (chkCompareAll.Checked)
                {
                    Logger("Comparing complete file");
                    CompareBlock(basefile.buf, modfile.buf, 0, (uint)fsize, "", SkipList);
                }
                else
                {

                    for (int Snr = 0; Snr < basefile.Segments.Count; Snr++)
                    {
                        if (!basefile.Segments[Snr].Missing)
                        {
                            for (int p = 0; p < basefile.segmentAddressDatas[Snr].SegmentBlocks.Count; p++)
                            {
                                if (basefile.Segments[Snr].CS1Address != null && basefile.Segments[Snr].CS1Address != "")
                                    SkipList.Add(basefile.segmentAddressDatas[Snr].CS1Address);
                                if (basefile.Segments[Snr].CS2Address != null && basefile.Segments[Snr].CS2Address != "")
                                    SkipList.Add(basefile.segmentAddressDatas[Snr].CS2Address);
                            }
                        }
                    }

                    for (int Snr = 0; Snr < basefile.Segments.Count; Snr++)
                    {
                        if (chkSegments[Snr].Enabled && chkSegments[Snr].Checked)
                        {
                            Logger("Comparing segment " + basefile.Segments[Snr].Name, false);
                            for (int p = 0; p < basefile.segmentAddressDatas[Snr].SegmentBlocks.Count; p++)
                            {
                                uint Start = basefile.segmentAddressDatas[Snr].SegmentBlocks[p].Start;
                                uint End = basefile.segmentAddressDatas[Snr].SegmentBlocks[p].End;
                                CompareBlock(basefile.buf, modfile.buf, Start, End, basefile.Segments[Snr].Name, SkipList);
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
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold( "frmPatcher, line " + line + ": " + ex.Message);
            }
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SuppressAfter = (uint)numSuppress.Value;
            Properties.Settings.Default.Save();
            if (txtBaseFile.Text.Length == 0 || txtModifierFile.Text.Length == 0)
                return;
            if (basefile.Segments != null && basefile.Segments.Count > 0)
            { 
                labelXML.Text = basefile.configFile + " (v " + basefile.Segments[0].Version + ")";
            }
            if (txtOS.Text.Length == 0)
            {
                txtOS.Text = "ALL";
            }
            txtResult.Text = "";

            CompareBins();
            RefreshPatch();
        }

        private void SavePatch(string Description, string fileName = "")
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
                if (fileName == "")
                    fileName = SelectSaveFile("XMLPATCH files (*.xmlpatch)|*.xmlpatch|All files (*.*)|*.*");
                if (fileName.Length < 1)
                    return;
                Logger("Saving to file: " + Path.GetFileName(fileName), false);
                if (PatchList[0].Description == null)
                {
                    XmlPatch xpatch = PatchList[0];
                    xpatch.Description = Description;
                    PatchList[0] = xpatch;
                }

                using (FileStream stream = new FileStream(fileName, FileMode.Create))
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
                string fileName = SelectSaveFile("BIN files (*.bin)|*.bin|ALL files(*.*)|*.*",Path.GetFileName(txtBaseFile.Text));
                if (fileName.Length == 0)
                    return;

                Logger("Saving to file: " + fileName);
                basefile.saveBin(fileName);
                this.Text = "Universal Patcher - " + Path.GetFileName(fileName);
                txtBaseFile.Text = fileName;
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
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
                txtResult.AppendText(LogText);
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
                if (NewLine)
                    txtResult.AppendText(Environment.NewLine);
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
                txtResult.AppendText(LogText);
                if (NewLine)
                    txtResult.AppendText(Environment.NewLine);
            }
            Application.DoEvents();
        }

        private void btnCheckSums_Click(object sender, EventArgs e)
        {
            if (basefile.Segments != null && basefile.Segments.Count > 0)
            { 
                basefile.FixCheckSums();
            }
        }



        private void btnLoadFolder_Click(object sender, EventArgs e)
        {
            try
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
                        PcmFile PCM = new PcmFile(FileName, chkAutodetect.Checked, basefile.configFileFullName);
                        GetFileInfo(FileName, ref PCM, true);
                    }
                    if (!chkLogtodisplay.Checked)
                        txtResult.AppendText(Environment.NewLine + "[Done]" + Environment.NewLine);
                    else
                        Logger("[Done]");
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
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
            try
            {
                if (basefile == null || PatchList == null)
                {
                    Logger("Nothing to do");
                    return;
                }
                ApplyXMLPatch(ref basefile);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void ExtractTable(uint Start, uint End, string[] OSlist, string MaskText)
        {
            try
            {
                XmlPatch xpatch = new XmlPatch();
                xpatch.CompatibleOS = OSlist[0] + ":" + Start.ToString("X"); 
                for (int i=1;i < OSlist.Length; i++)
                    xpatch.CompatibleOS += "," + OSlist[i] + ":" + Start.ToString("X");
                xpatch.XmlFile = Path.GetFileName(basefile.configFileFullName);
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
                RefreshPatch();

            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnExtract_Click(object sender, EventArgs e)
        {
            try
            {
                uint Start;
                uint End;
                string MaskText = "";
                if (txtBaseFile.Text.Length == 0)
                    return;
                basefile = new PcmFile(txtBaseFile.Text, chkAutodetect.Checked, basefile.configFileFullName);
                GetFileInfo(txtBaseFile.Text, ref basefile, true, false);
                if (txtCompatibleOS.Text.Length == 0)
                    txtCompatibleOS.Text = basefile.OS;
                if (txtCompatibleOS.Text.Length == 0)
                    txtCompatibleOS.Text = "ALL";
                string[] OSlist = txtCompatibleOS.Text.Split(',');
                string[] blocks = txtExtractRange.Text.Split(',');
                for (int b = 0; b < blocks.Length; b++)
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void RefreshPatch()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = PatchList;
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

        public void RefreshPatchList()
        {
            patchesBindingSource.DataSource = null;
            listPatches.DataSource = null;
            patchesBindingSource.DataSource = patches;
            listPatches.DataSource = patchesBindingSource;
            listPatches.DisplayMember = "Name";
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
                if (basefile.configFileFullName != null && basefile.configFileFullName.Length > 0)
                    frmM.txtXML.Text = Path.GetFileName(basefile.configFileFullName);
            }
            
            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                if (PatchList == null)
                    PatchList = new List<XmlPatch>();
                XmlPatch XP = new XmlPatch();
                XP.Name = frmM.txtName.Text;
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
                RefreshPatch();
            }
        }

        public void openPatchSelector()
        {
            try
            {
                frmPatchSelector frmPS = new frmPatchSelector();
                frmPS.basefile = basefile;
                frmPS.Show();
                Application.DoEvents();
                frmPS.loadPatches();
                RefreshPatch();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnBinLoadPatch_Click(object sender, EventArgs e)
        {
            //LoadPatch();
            openPatchSelector();
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
            try
            {
                if (PatchList == null ||  PatchList.Count == 0)
                    return;
                if (dataPatch.SelectedRows.Count == 0 && dataPatch.SelectedCells.Count == 0)
                    return;
                if (dataPatch.SelectedRows.Count == 0)
                    dataPatch.Rows[dataPatch.SelectedCells[0].RowIndex].Selected = true;
                frmManualPatch frmM = new frmManualPatch();
                if (dataPatch.CurrentRow.Cells["Name"].Value != null)
                    frmM.txtName.Text = dataPatch.CurrentRow.Cells["Name"].Value.ToString();
                if (dataPatch.CurrentRow.Cells["Description"].Value != null)
                    frmM.txtDescription.Text = dataPatch.CurrentRow.Cells["Description"].Value.ToString();
                if (dataPatch.CurrentRow.Cells["XmlFile"].Value != null)
                    frmM.txtXML.Text = dataPatch.CurrentRow.Cells["XmlFile"].Value.ToString();
                if (dataPatch.CurrentRow.Cells["Segment"].Value != null)
                    frmM.txtSegment.Text = dataPatch.CurrentRow.Cells["Segment"].Value.ToString();
                if (dataPatch.CurrentRow.Cells["CompatibleOS"].Value != null)
                    frmM.txtCompOS.Text = dataPatch.CurrentRow.Cells["CompatibleOS"].Value.ToString();
                if (dataPatch.CurrentRow.Cells["Data"].Value != null)
                    frmM.txtData.Text = dataPatch.CurrentRow.Cells["Data"].Value.ToString();
                if (dataPatch.CurrentRow.Cells["Rule"].Value != null && dataPatch.CurrentRow.Cells["Rule"].Value.ToString().Contains(":"))
                {
                    string[] Parts = dataPatch.CurrentRow.Cells["Rule"].Value.ToString().Split(':');
                    if (Parts.Length == 3)
                    {
                        frmM.txtReadAddr.Text = Parts[0];
                        frmM.txtMask.Text = Parts[1];
                        frmM.txtValue.Text = Parts[2];
                    }
                }
                if (dataPatch.CurrentRow.Cells["HelpFile"].Value != null)
                    frmM.txtHelpFile.Text = dataPatch.CurrentRow.Cells["HelpFile"].Value.ToString();

                if (frmM.ShowDialog(this) == DialogResult.OK)
                {
                    dataPatch.CurrentRow.Cells["Name"].Value = frmM.txtName.Text;
                    dataPatch.CurrentRow.Cells["Description"].Value = frmM.txtDescription.Text;
                    dataPatch.CurrentRow.Cells["XmlFile"].Value = frmM.txtXML.Text;
                    dataPatch.CurrentRow.Cells["Segment"].Value = frmM.txtSegment.Text;
                    dataPatch.CurrentRow.Cells["CompatibleOS"].Value = frmM.txtCompOS.Text;
                    dataPatch.CurrentRow.Cells["Data"].Value = frmM.txtData.Text;
                    if (frmM.txtReadAddr.Text.Length > 0)
                    {
                        dataPatch.CurrentRow.Cells["Rule"].Value = frmM.txtReadAddr.Text + ":" + frmM.txtMask.Text + ":" + frmM.txtValue.Text;
                    }
                    dataPatch.CurrentRow.Cells["HelpFile"].Value = frmM.txtHelpFile.Text;
                }
                frmM.Dispose();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

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
            RefreshPatch();
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
            RefreshPatch();
            dataPatch.CurrentCell = dataPatch.Rows[row + 1].Cells[0];
            dataPatch.Rows[row + 1].Selected = true;
        }

        private void loadXMLfile()
        {
            try
            {
                string fileName = SelectFile("Select XML file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
                if (fileName.Length < 1)
                    return;
                basefile.loadConfigFile(fileName);
                labelXML.Text = Path.GetFileName(fileName) + " (v " + basefile.Segments[0].Version + ")";
                addCheckBoxes();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
        private void loadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadXMLfile();
        }

        private void setupSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmSL != null && frmSL.Visible)
                {
                    frmSL.BringToFront();
                    return;
                }
                frmSL = new frmSegmenList();
                frmSL.PCM = basefile;
                frmSL.InitMe();
                if (basefile.configFileFullName.Length > 0)
                    frmSL.LoadFile(basefile.configFileFullName);
                if (frmSL.ShowDialog() == DialogResult.OK)
                {
                    //addCheckBoxes();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void autodetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmAutodetect frmAD = new frmAutodetect();
                frmAD.Show();
                frmAD.InitMe();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshPatch();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshPatch();

        }

        public void LoadPatch(string fileName)
        {
            try
            {
                //fileName = SelectFile("Select patch file", "XML patch files (*.xmlpatch)|*.xmlpatch|PATCH files (*.patch)|*.patch|ALL files(*.*)|*.*");
                if (fileName.Length > 1)
                {
                    labelPatchname.Text = fileName;
                    Logger("Loading file: " + fileName);
                    PatchList = loadPatchFile(fileName);
                    if (PatchList.Count > 0)
                    {
                        Logger("Description: " + PatchList[0].Description);
                        if (!PatchList[0].CompatibleOS.ToLower().StartsWith("table:") && !PatchList[0].CompatibleOS.ToLower().StartsWith("search:"))
                        {
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
                    }
                    bool isCompatible = false;
                    for (int x=0; x<PatchList.Count; x++)
                    {
                        if (checkPatchCompatibility(PatchList[x],basefile) < uint.MaxValue)
                            isCompatible = true;
                    }
                    if (isCompatible)
                    {
                        btnApplypatch.Enabled = true;
                        Logger("Patch is compatible, you can apply it");
                    }
                    RefreshPatch();
                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

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
                    if (CheckStockCVN(stock.PN,stock.Ver,stock.SegmentNr,stock.cvn , false, basefile.configFileFullName) != "[stock]")
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
            try
            {
                frmEditXML frmE = new frmEditXML();
                frmE.LoadStockCVN();
                frmE.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePatch(txtPatchDescription.Text);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshPatch();

        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            RefreshPatch();

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
            try
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
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

        private void ExtractSegments(PcmFile PCM, string Descr, bool AllSegments, string dstFolder, string prefix="", string suffix="")
        {            
            if (PCM.segmentinfos == null)
            {
                Logger("no segments defined");
                return;
            }
            try
            {
                string scriptFName = prefix.Replace("$nr", "").Replace("$ver", "").Replace("$cvn", "").Replace("-", "").Replace("#", "") + "-";
                scriptFName += Path.GetFileNameWithoutExtension(PCM.FileName) + "-";
                scriptFName += suffix.Replace("$ver", "").Replace("$cvn", "").Replace("-","").Replace("#", "") + ".csv";
                scriptFName = Path.Combine(dstFolder, scriptFName);
                string scriptContent = "Filename;Part1";
                int maxBlocks = 1;
                for (int s = 0; s < PCM.segmentinfos.Length; s++)
                {
                    if (PCM.segmentAddressDatas[s].SegmentBlocks.Count > maxBlocks)
                        maxBlocks = PCM.segmentAddressDatas[s].SegmentBlocks.Count;
                }
                for (int b=1; b<maxBlocks;b++)
                {
                    scriptContent += ";Part" + (b + 1).ToString();
                }
                scriptContent += Environment.NewLine + "Fill:4A FC;" + PCM.fsize.ToString("X") + Environment.NewLine;

                for (int s=0;s<PCM.segmentinfos.Length;s++)
                {
                    if (AllSegments || chkExtractSegments[s].Checked)
                    {
                        string FileName;
                        if (dstFolder.Length > 0)
                        {
                            string FnameStart = PCM.segmentinfos[s].PN.PadLeft(8,'0');
                            string tmpPrefix = prefix.Replace("$nr", PCM.segmentinfos[s].SegNr).Replace("$ver", PCM.segmentinfos[s].Ver).Replace("$cvn", PCM.segmentinfos[s].cvn);
                            string tmpSuffix = suffix.Replace("$nr", PCM.segmentinfos[s].SegNr).Replace("$ver", PCM.segmentinfos[s].Ver).Replace("$cvn", PCM.segmentinfos[s].cvn);
                            FnameStart = tmpPrefix + FnameStart + tmpSuffix;
                            FnameStart = Path.Combine(dstFolder, FnameStart);
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
                                WriteSegmentToFile(FileName, PCM.segmentAddressDatas[s].SwapBlocks, PCM.buf);
                                scriptContent += FileName + ";" + PCM.segmentAddressDatas[s].SwapBlocks[0].Start.ToString("X") + "-" + PCM.segmentAddressDatas[s].SwapBlocks[0].End.ToString("X");
                                for (int b=1; b< PCM.segmentAddressDatas[s].SwapBlocks.Count; b++)
                                    scriptContent += ";" + PCM.segmentAddressDatas[s].SwapBlocks[b].Start.ToString("X") + "-" + PCM.segmentAddressDatas[s].SwapBlocks[b].End.ToString("X");
                            }
                            else
                            { 
                                Logger("Writing " + PCM.segmentinfos[s].Name + " to file: " + FileName + ", size: " + PCM.segmentinfos[s].Size);
                                WriteSegmentToFile(FileName, PCM.segmentAddressDatas[s].SegmentBlocks, PCM.buf);
                                scriptContent += FileName + ";" + PCM.segmentAddressDatas[s].SegmentBlocks[0].Start.ToString("X") + "-" + PCM.segmentAddressDatas[s].SegmentBlocks[0].End.ToString("X");
                                for (int b = 1; b < PCM.segmentAddressDatas[s].SegmentBlocks.Count; b++)
                                    scriptContent += ";" + PCM.segmentAddressDatas[s].SegmentBlocks[b].Start.ToString("X") + "-" + PCM.segmentAddressDatas[s].SegmentBlocks[b].End.ToString("X");
                            }
                            scriptContent += Environment.NewLine;
                            StreamWriter sw = new StreamWriter(FileName + ".txt");
                            sw.WriteLine(Descr);
                            sw.Close();
                            Logger("[OK]");
                        }
                    }
                }

                if (dstFolder.Length > 0) //Custom destination
                {
                    Logger("Writing rebuild script: " + scriptFName + "... ");
                    WriteTextFile(scriptFName, scriptContent);
                    Logger("[OK]");
                }

            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
        private void btnExtractSegments_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBaseFile.Text.Length == 0)
                {
                    Logger("No file loaded");
                    return;
                }
                if (txtSegmentDescription.Text.Length == 0)
                    txtSegmentDescription.Text = Path.GetFileName(basefile.FileName).Replace(".bin", "");
                ExtractSegments(basefile, txtExtractDescription.Text, false, "");
                SaveSegmentList();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnExtractSegmentsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                frmExtractSegments frmES = new frmExtractSegments();
                frmES.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
                if (frmES.ShowDialog(this) == DialogResult.OK)
                {
                    if (!chkLogtodisplay.Checked)
                        txtResult.AppendText("Extracting...");
                    string dstFolder = frmES.labelCustomdst.Text;
                    for (int i = 0; i < frmES.listFiles.CheckedItems.Count; i++)
                    {
                        string fileName = frmES.listFiles.CheckedItems[i].Tag.ToString();
                        PcmFile PCM = new PcmFile(fileName, chkAutodetect.Checked, basefile.configFileFullName);
                        GetFileInfo(fileName, ref PCM, true, checkExtractShowinfo.Checked);
                        string descr = Path.GetFileName(fileName).Replace(".bin", "");
                        if (frmES.txtDescription.Text.Length > 0)
                            descr = frmES.txtDescription.Text;
                        string prefix = frmES.txtFileNamePrefix.Text.ToLower();
                        string suffix = frmES.txtFilenameSuffix.Text.ToLower();
                        ExtractSegments(PCM, descr, true, dstFolder,prefix,suffix);
                    }
                    if (!chkLogtodisplay.Checked)
                        txtResult.AppendText(Environment.NewLine + "Segments extracted" + Environment.NewLine);
                    else
                        Logger("Segments extracted");
                    SaveSegmentList();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        private void btnSwapSegments_Click(object sender, EventArgs e)
        {
            try
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
                List<string> currentSegements = new List<string>();
                for (int s = 0; s < basefile.Segments.Count; s++)
                {
                    if (!basefile.Segments[s].Missing)
                    {
                        string seg = basefile.segmentinfos[s].Name.PadRight(15) + basefile.segmentinfos[s].PN + basefile.segmentinfos[s].Ver;
                        currentSegements.Add(seg);
                    }
                }
                frmSwapSegmentList frmSw = new frmSwapSegmentList();
                frmSw.LoadSegmentList(ref basefile);
                if (frmSw.ShowDialog(this) == DialogResult.OK)
                {
                    basefile = frmSw.PCM;
                    basefile.GetInfo();
                    basefile.FixCheckSums();
                    LoggerBold(Environment.NewLine + "Swapped segments:");
                    for (int s = 0; s < basefile.Segments.Count; s++)
                    {
                        if (!basefile.Segments[s].Missing)
                        {
                            string newPN = basefile.segmentinfos[s].PN + basefile.segmentinfos[s].Ver;
                            if (!currentSegements[s].EndsWith(newPN))
                                Logger(currentSegements[s] + " => " + basefile.segmentinfos[s].PN + basefile.segmentinfos[s].Ver);
                        }
                    }
                    Logger("Segment(s) swapped and checksums fixed (you can save BIN now)");
                }
                frmSw.Dispose();
            }
            catch (Exception ex) { LoggerBold(ex.Message); }
        }

        private void FixFileChecksum(string fileName)
        {
            try
            {
                basefile = new PcmFile(fileName,chkAutodetect.Checked, basefile.configFileFullName);
                GetFileInfo(fileName, ref basefile, true, false);                
                if (basefile.FixCheckSums())  //Returns true, if need fix for checksum
                {  
                    Logger("Saving file: " + fileName);
                    basefile.saveBin(fileName);
                    Logger("[OK]");
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnFixFilesChecksum_Click(object sender, EventArgs e)
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
            try
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

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
            try { 
                frmSearchText frmS = new frmSearchText();
                frmS.Show();
                frmS.initMe(txtResult);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
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
            try
            {
            frmSearchTables frmST = new frmSearchTables();
            frmST.Show(this);
            if (tableSearchFile != null && tableSearchFile.Length > 0)
                frmST.LoadFile(tableSearchFile);
            else
                frmST.LoadConfig();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnSaveSearchedTables_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
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
            getPidList();
        }

        private void btnClearPidList_Click(object sender, EventArgs e)
        {
            pidList = new List<PidSearch.PID>();
            refreshPidList();
        }

        private void btnSavePidList_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

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
            try
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnClearDTC_Click(object sender, EventArgs e)
        {
            refreshDtcList();
        }

        private void btnSaveCsvDTC_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void modifyDtc()
        {
            try
            { 
                int codeIndex = dataGridDTC.SelectedCells[0].RowIndex;
                frmSetDTC frmS = new frmSetDTC();
                frmS.startMe(codeIndex, basefile);
                if (frmS.ShowDialog() == DialogResult.OK)
                {
                    basefile.dtcCodes[codeIndex].Status = (byte)frmS.comboDtcStatus.SelectedIndex;

                    basefile.buf[basefile.dtcCodes[codeIndex].statusAddrInt] = basefile.dtcCodes[codeIndex].Status;
                    if (basefile.dtcCombined)
                    {
                        basefile.dtcCodes[codeIndex].StatusTxt = dtcStatusCombined[basefile.dtcCodes[codeIndex].Status];
                        dataGridDTC.Rows[codeIndex].Cells["StatusTxt"].Value = basefile.dtcCodes[codeIndex].StatusTxt;

                        if (basefile.dtcCodes[codeIndex].Status > 3)
                            basefile.dtcCodes[codeIndex].MilStatus = 1;
                        else
                            basefile.dtcCodes[codeIndex].MilStatus = 0;
                    }
                    else
                    {
                        basefile.dtcCodes[codeIndex].MilStatus = (byte)frmS.comboMIL.SelectedIndex;
                        basefile.dtcCodes[codeIndex].StatusTxt = dtcStatus[basefile.dtcCodes[codeIndex].Status];
                        basefile.buf[basefile.dtcCodes[codeIndex].milAddrInt] = basefile.dtcCodes[codeIndex].MilStatus;
                        dataGridDTC.Rows[codeIndex].Cells["StatusTxt"].Value = basefile.dtcCodes[codeIndex].StatusTxt;
                    }
                    dataGridDTC.Rows[codeIndex].Cells["Status"].Value = basefile.dtcCodes[codeIndex].Status;
                

                    tabFunction.SelectedTab = tabApply;
                    Logger("DTC modified, you can now save bin");
                }
                frmS.Dispose();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
        private void dataGridDTC_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            modifyDtc();
        }

        private void btnSetDTC_Click(object sender, EventArgs e)
        {
            modifyDtc();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void dTCSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try 
            { 
                frmEditXML frmE = new frmEditXML();
                frmE.LoadDTCSearchConfig();
                frmE.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnShowTableData_Click(object sender, EventArgs e)
        {
            try 
            { 
                tableViews = new List<TableView>();
                int dataIndex = dataIndex = dataGridSearchedTables.SelectedCells[0].RowIndex;
                uint StartAddr;
                uint rows;
                if (chkTableSearchNoFilters.Checked)
                {
                    StartAddr = tableSearchResultNoFilters[dataIndex].AddressInt;
                    rows = tableSearchResultNoFilters[dataIndex].Rows;
                }
                else
                {
                    StartAddr = tableSearchResult[dataIndex].AddressInt;
                    rows = tableSearchResult[dataIndex].Rows;
                }

                if (rows == 0)
                {
                    TableView dt = new TableView();
                    dt.Row = 0;
                    dt.Address = StartAddr.ToString("X8");
                    dt.addrInt = StartAddr;
                    dt.dataInt = basefile.buf[StartAddr];
                    dt.Data = dt.dataInt.ToString("X2");
                    tableViews.Add(dt);
                }
                else
                {
                    uint row = 0;
                    for (uint addr = StartAddr; addr < StartAddr + rows; addr++)
                    {
                        TableView dt = new TableView();
                        dt.Row = row;
                        dt.addrInt = addr;
                        dt.Address = addr.ToString("X8");
                        dt.dataInt = basefile.buf[addr];
                        dt.Data = dt.dataInt.ToString("X2");
                        tableViews.Add(dt);
                        row++;
                    }
                }
                frmEditXML frmEX = new frmEditXML();
                frmEX.LoadTableData();
                frmEX.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        private void tableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (basefile.configFileFullName == null)
                {
                    Logger("No file/XML selected");
                    return;
                }
                frmEditXML frmE = new frmEditXML();
                frmE.LoadTableSeek(basefile.tableSeekFile);
                frmE.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnClearTableSeek_Click(object sender, EventArgs e)
        {
            tableSeeks = new List<TableSeek>();
            refreshTableSeek();
        }

        private void openTableEditor()
        {
            try
            {

                int rowindex = dataGridTableSeek.CurrentCell.RowIndex;
                int columnindex = dataGridTableSeek.CurrentCell.ColumnIndex;
                int codeIndex = Convert.ToInt32(dataGridTableSeek.Rows[rowindex].Cells["id"].Value);
                TableSeek ts = tableSeeks[basefile.foundTables[codeIndex].configId];
                if (basefile.foundTables[codeIndex].Address == null || basefile.foundTables[codeIndex].Address == "")
                {
                    Logger("No address defined!");
                    return;
                }

                frmTableEditor frmT = new frmTableEditor();
                frmT.loadSeekTable(codeIndex, basefile);
                frmT.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            openTableEditor();
        }

        private void dataGridTableSeek_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            openTableEditor();
        }

        private void rememberWindowSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rememberWindowSizeToolStripMenuItem.Checked)
            {
                rememberWindowSizeToolStripMenuItem.Checked = false;
                Properties.Settings.Default.MainWindowPersistence = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                rememberWindowSizeToolStripMenuItem.Checked = true;
                Properties.Settings.Default.MainWindowPersistence = true;
                Properties.Settings.Default.Save();
            }
        }

        private void btnReadTinyTunerDB_Click(object sender, EventArgs e)
        {
            TableSeek TS = new TableSeek();
            if (chkTableSeek.Checked)
            {
                Logger("Seeking tables...", false);
                Logger(TS.seekTables(basefile));
            }

            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDB(basefile));
            refreshTableSeek();
        }

        private void comboTableCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterTableSeek();
        }

        private void chkForceCompare_CheckedChanged(object sender, EventArgs e)
        {
            CheckSegmentCompatibility();
        }

        private void btnSearchTableSeek_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridTableSeek.CurrentCell.RowIndex;
            for (int i = rowindex +1 ; i < dataGridTableSeek.RowCount; i++)
            {
                if (dataGridTableSeek.Rows[i].Cells["Name"].Value.ToString().ToLower().Contains(txtSearchTableSeek.Text.ToLower()))
                {
                    dataGridTableSeek.ClearSelection();
                    dataGridTableSeek.CurrentCell = dataGridTableSeek.Rows[i].Cells[0];
                    dataGridTableSeek.CurrentCell.Selected = true;
                    break;
                }
            }

        }

        private void btnTuner_Click(object sender, EventArgs e)
        {
            try
            {
                if (basefile == null)
                {
                    LoggerBold("No file loaded");
                    //return;
                }
                //basefile.tableDatas = new List<TableData>();
                FrmTuner ft = new FrmTuner(basefile);
                ft.Show();
            }
            catch(Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void filterTableSeek()
        {
            try
            {
                string cat = comboTableCategory.Text;
                var results = basefile.foundTables.Where(t => t.Name.Length > 0); //How should I define empty variable??
                if (cat != "_All" && cat != "")
                    results = results.Where(t => t.Category.ToLower().Contains(comboTableCategory.Text.ToLower()));
                if (txtSearchTableSeek.Text.Length > 0)
                    results = results.Where(t => t.Name.ToLower().Contains(txtSearchTableSeek.Text.ToLower()));
                filteredCategories = new BindingList<FoundTable>(results.ToList());
                tableSeekBindingSource.DataSource = filteredCategories;
            }
            catch { }

        }
        private void txtSearchTableSeek_TextChanged(object sender, EventArgs e)
        {
            filterTableSeek();
        }


        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openBaseFile();
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {            
            Logger("Saving file: " + basefile.FileName);
            basefile.saveBin(basefile.FileName);
            Logger("OK");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBin();
        }

        private void disableTunerAutoloadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disableTunerAutloadConfigToolStripMenuItem.Checked = !disableTunerAutloadConfigToolStripMenuItem.Checked;
            Properties.Settings.Default.disableTunerAutoloadSettings = disableTunerAutloadConfigToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void moreSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { 
                frmMoreSettings frmTS = new frmMoreSettings();
                frmTS.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnLoadPatch_Click(object sender, EventArgs e)
        {
            openPatchSelector();
        }

        private void oBD2CodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML frmEX = new frmEditXML();
            frmEX.loadOBD2CodeList();
            frmEX.Show();
        }

        private void segmentSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (basefile.configFileFullName == null)
                {
                    Logger("No file/XML selected");
                    return;
                }
                frmEditXML frmE = new frmEditXML();
                frmE.Show();
                frmE.LoadSegmentSeek(basefile.segmentSeekFile);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void pIDSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmEditXML frmE = new frmEditXML();
                frmE.LoadPIDSearchConfig();
                frmE.Show();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private uint csUtilCalcCS(bool calcOnly, uint oldVal)
        {
            List<Block> blocks;
            ParseAddress(txtChecksumRange.Text, basefile, out blocks);
            List<Block> excludes;
            ParseAddress(txtExclude.Text, basefile, out excludes);
            AddressData csAddr;
            csAddr.Address = 0;
            HexToUint(txtCSAddr.Text, out csAddr.Address);
            csAddr.Bytes = (ushort)numCSBytes.Value;
            csAddr.Name = "CS";
            csAddr.Type = 0;
            short method = CSMethod_Bytesum;
            short complement = 0;

            if (chkCSUtilTryAll.Checked && calcOnly)
            {
                for (complement = 0; complement <= 2; complement++)
                {                                        
                    uint csCalc = CalculateChecksum(basefile.buf, csAddr, blocks, excludes, CSMethod_crc16, complement, (ushort)numCSBytes.Value, chkCsUtilSwapBytes.Checked);
                    if (csCalc == oldVal)
                        LoggerBold("Method: CRC16,    Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                    else
                        Logger("Method: CRC16,    Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));

                    csCalc = CalculateChecksum(basefile.buf, csAddr, blocks, excludes, CSMethod_crc32, complement, (ushort)numCSBytes.Value, chkCsUtilSwapBytes.Checked);
                    if (csCalc == oldVal)
                        LoggerBold("Method: CRC32,    Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                    else
                        Logger("Method: CRC32,    Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));

                    csCalc = CalculateChecksum(basefile.buf, csAddr, blocks, excludes, CSMethod_Bytesum, complement, (ushort)numCSBytes.Value, chkCsUtilSwapBytes.Checked);
                    if (csCalc == oldVal)
                        LoggerBold("Method: Bytesum,  Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                    else
                        Logger("Method: Bytesum,  Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));

                    csCalc = CalculateChecksum(basefile.buf, csAddr, blocks, excludes, CSMethod_Wordsum, complement, (ushort)numCSBytes.Value, chkCsUtilSwapBytes.Checked);
                    if (csCalc == oldVal)
                        LoggerBold("Method: WordSum,  Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                    else
                        Logger("Method: WordSum,  Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));

                    csCalc = CalculateChecksum(basefile.buf, csAddr, blocks, excludes, CSMethod_Dwordsum, complement, (ushort)numCSBytes.Value, chkCsUtilSwapBytes.Checked);
                    if (csCalc == oldVal)
                        LoggerBold("Method: DwordSum, Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                    else
                        Logger("Method: DwordSum, Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                }
                return 0;
            }
            else
            {

                if (radioCSUtilCrc16.Checked)
                    method = CSMethod_crc16;
                if (radioCSUtilCrc32.Checked)
                    method = CSMethod_crc32;
                if (radioCSUtilDwordSum.Checked)
                    method = CSMethod_Dwordsum;
                if (radioCSUtilSUM.Checked)
                    method = CSMethod_Bytesum;
                if (radioCSUtilWordSum.Checked)
                    method = CSMethod_Wordsum;
                if (radioCSUtilComplement1.Checked)
                    complement = 1;
                if (radioCSUtilComplement2.Checked)
                    complement = 2;
                return CalculateChecksum(basefile.buf, csAddr, blocks, excludes, method, complement, (ushort)numCSBytes.Value, chkCsUtilSwapBytes.Checked);
            }
        }

        private void btnTestChecksum_Click(object sender, EventArgs e)
        {
            try
            {
                Logger("Checksum research:");

                uint oldVal = 0;
                uint csAddr;
                if (HexToUint(txtCSAddr.Text, out csAddr))
                {
                    if (numCSBytes.Value == 1)
                        oldVal = basefile.buf[csAddr];
                    else if (numCSBytes.Value == 2)
                        oldVal = BEToUint16(basefile.buf, csAddr);
                    else if (numCSBytes.Value == 4)
                        oldVal = BEToUint32(basefile.buf, csAddr);

                    Logger("Saved value: " + oldVal.ToString("X"));
                }

                if (chkCSUtilTryAll.Checked)
                {
                    csUtilCalcCS(true, oldVal);
                }
                else
                {
                    Logger("Result: ", false);
                    Logger(csUtilCalcCS(true, oldVal).ToString("X"));
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnCsUtilFix_Click(object sender, EventArgs e)
        {
            try
            {
                uint CS1Calc = csUtilCalcCS(false, 0);
                uint csAddr;
                HexToUint(txtCSAddr.Text, out csAddr);

                uint oldVal = 0;
                if (numCSBytes.Value == 1)
                {
                    oldVal = basefile.buf[csAddr];
                    basefile.buf[csAddr] = (byte)CS1Calc;
                }
                else if (numCSBytes.Value == 2)
                {
                    oldVal = BEToUint16(basefile.buf, csAddr);
                    SaveUshort(basefile.buf, csAddr, (ushort)CS1Calc);
                }
                else if (numCSBytes.Value == 4)
                {
                    oldVal = BEToUint32(basefile.buf, csAddr);
                    SaveUint32(basefile.buf, csAddr, CS1Calc);
                }
                Logger("Checksum: " + oldVal.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                Logger("You can save BIN file now");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void listPatches_SelectedIndexChanged(object sender, EventArgs e)
        {
            PatchList = patches[listPatches.SelectedIndex].patches;
            RefreshPatch();
        }

        private void btnSaveAllPatches_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Patch pa in patches)
                {
                    PatchList = pa.patches;
                    string fileName = Path.Combine(Application.StartupPath, "Patches", pa.Name + ".xmlpatch");
                    Logger("Saving patch: " + fileName, false);
                    SavePatch(pa.Name, fileName);
                    Logger(" [OK]");
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnAddPatch_Click(object sender, EventArgs e)
        {
            try
            {
                frmData frmD = new frmData();
                frmD.Text = "Patch Name:";
                if (frmD.ShowDialog() == DialogResult.OK)
                {
                    Patch patch = new Patch();
                    patch.Name = frmD.txtData.Text;
                    patches.Add(patch);
                    RefreshPatchList();
                }
                frmD.Dispose();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnDelPatch_Click(object sender, EventArgs e)
        {
            try
            {
                if (listPatches.SelectedIndex < 0)
                    return;
                patches.RemoveAt(listPatches.SelectedIndex);
                RefreshPatchList();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void touristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkingMode = 0;
            Properties.Settings.Default.Save();
            setWorkingMode();
        }

        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkingMode = 1;
            Properties.Settings.Default.Save();
            setWorkingMode();
        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkingMode = 2;
            Properties.Settings.Default.Save();
            setWorkingMode();
        }

        private void fakeCvn(int seg)
        {
            uint freeAddr = uint.MaxValue;

            Logger(basefile.Segments[seg].Name.PadRight(20), false);
            if (basefile.Segments[seg].CVN == 0)
            {
                Logger("No CVN defined");
            }
            if (txtTargetCVN.Text.Length == 0)
            {
                Logger("No target CVN");
                return;
            }

            if (radioFakeCvnAddr.Checked)
            {
                if (!HexToUint(txtFreeAddress.Text, out freeAddr))
                {
                    LoggerBold("Can't convert from HEX: " + txtFreeAddress.Text);
                    return;
                }
            }
            else if (radioFakeCVNRelativeAddr.Checked)
            {
                freeAddr = basefile.segmentAddressDatas[seg].SegmentBlocks[basefile.segmentAddressDatas[seg].SegmentBlocks.Count - 1].End - (uint)numFakeCvnBytesFromEnd.Value;
            }
            else //Ver
            {
                freeAddr = basefile.segmentAddressDatas[seg].VerAddr.Address;
                numFakeCvnBytes.Value = basefile.segmentAddressDatas[seg].VerAddr.Bytes;
            }

            uint targetCvn = uint.MaxValue;
            if (!HexToUint(txtTargetCVN.Text, out targetCvn))
            {
                LoggerBold("Can't convert from HEX: " + txtTargetCVN.Text);
                return;
            }

            uint orgValue = BEToUint32(basefile.buf, freeAddr);

            uint cvn;
            if (basefile.Segments[seg].CVN == 1)
                cvn = basefile.calculateCS1(seg, false);
            else
                cvn = basefile.calculateCS2(seg, false);

            if (cvn == targetCvn)
            {
                Logger("[OK]");
                return;
            }
            uint maxVal = byte.MaxValue;
            switch ((int)numFakeCvnBytes.Value)
            {
                case 2:
                    maxVal = ushort.MaxValue;
                    break;
                case 3:
                    maxVal = 0xFFFFFF;
                    break;
                case 4:
                    maxVal = uint.MaxValue;
                    break;
            }

            bool found = false;
            Stopwatch timer = new Stopwatch();
            timer.Start();

            for (uint testVal = 0; testVal < maxVal; testVal++)
            {
                if (btnFakeCVN.Text == "Go")    //Stop pressed
                {
                    SaveUint32(basefile.buf, freeAddr, orgValue); //Restore original data
                    return;
                }
                labelFakeCVNTestVal.Text = testVal.ToString("X");
                if ((int)(testVal % 100) == 0)
                    Application.DoEvents();
                switch ((int)numFakeCvnBytes.Value)
                {
                    case 2:
                        SaveUshort(basefile.buf, freeAddr, (ushort)testVal);
                        break;
                    case 3:
                        Save3Bytes(basefile.buf, freeAddr, testVal);
                        break;
                    case 4:
                        SaveUint32(basefile.buf, freeAddr, testVal);
                        break;
                    default:
                        basefile.buf[freeAddr] = (byte)testVal;
                        break;
                }
                uint calcCVN = 0;

                uint CS1Calc = basefile.calculateCS1(seg,false);
                if (basefile.Segments[seg].CVN == 1)
                    calcCVN = CS1Calc;
                else
                {
                    //Fix CS1 first
                    basefile.saveCS1(seg, CS1Calc);
                    calcCVN = basefile.calculateCS2(seg, false);
                }
                if (calcCVN == targetCvn)
                {
                    found = true;
                    break;
                }
            }
            timer.Stop();

            if (found)
            {
                Logger("[fixed]", false);
                Logger(" (" + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'") + ")");
            }
            else
            {
                LoggerBold("Can't fix CVN!");
                SaveUint32(basefile.buf,freeAddr, orgValue); //Restore original data
            }

        }

        private void btnFakeCVN_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnFakeCVN.Text == "Go")
                {
                    btnFakeCVN.Text = "Stop";
                    LoggerBold(Environment.NewLine + "Searching CVN");
                    if (radioFakeCvnSingleSegment.Checked)
                    {
                        fakeCvn(comboFakeCvnSegment.SelectedIndex);
                    }
                    else
                    {
                        for (int i=0; i<comboFakeCvnSegment.Items.Count;i++)
                        {
                            if (btnFakeCVN.Text == "Go")
                                break;
                            getTargetCvn(i);
                            Application.DoEvents();
                            if (!basefile.Segments[i].Name.ToLower().Contains("eeprom"))
                            {
                                comboFakeCvnSegment.Text = basefile.Segments[i].Name;
                                Application.DoEvents();
                                fakeCvn(i);

                            }
                        }
                    }
                    Logger("Done");
                    basefile.FixCheckSums();
                    basefile.GetInfo();
                    ShowFileInfo(basefile,true);
                    showFakeCvnSelectedBytes();
                    //GetFileInfo(txtBaseFile.Text, ref basefile, false);
                }
                else //stop
                {
                    btnFakeCVN.Text = "Go";
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            btnFakeCVN.Text = "Go";
        }

        private void numFakeCvnBytes_ValueChanged(object sender, EventArgs e)
        {
            showFakeCvnSelectedBytes();
        }

        private void getTargetCvn(int seg)
        {
            txtTargetCVN.Text = "";
            labelFakeCvnPn.Text = "P/N: " + basefile.segmentinfos[seg].PN;
            for (int c = 0; c < StockCVN.Count; c++)
            {
                if (StockCVN[c].PN == basefile.segmentinfos[seg].PN && StockCVN[c].Ver == basefile.segmentinfos[seg].Ver && StockCVN[c].SegmentNr == basefile.segmentinfos[seg].SegNr)
                {
                    txtTargetCVN.Text = StockCVN[c].cvn;
                    break;
                }
            }
            if (txtTargetCVN.Text == "")
            {
                for (int r = 0; r < referenceCvnList.Count; r++)
                {
                    if (basefile.segmentinfos[seg].PN == referenceCvnList[r].PN)
                    {
                        txtTargetCVN.Text = referenceCvnList[r].CVN;
                    }
                }
            }

        }

        private void showFakeCvnSelectedBytes()
        {
            uint freeAddr = 0;
            if (!HexToUint(txtFreeAddress.Text, out freeAddr))
                return;

            try
            {
                int seg = comboFakeCvnSegment.SelectedIndex;
                richEndOfSegment.Text = "";
                uint segStartAddr = basefile.segmentAddressDatas[seg].SegmentBlocks[0].Start;
                uint segEndAddr = basefile.segmentAddressDatas[seg].SegmentBlocks[basefile.segmentAddressDatas[seg].SegmentBlocks.Count - 1].End;
                uint start = segStartAddr;
                if ((int)(freeAddr - 15) > 0)
                    start = freeAddr - 15;
                uint otherSegAddr = uint.MaxValue;
                for (uint a = start; a < basefile.fsize && a < (freeAddr + 15); a++)
                {
                    if (a >= freeAddr && a < (freeAddr + numFakeCvnBytes.Value))
                    {
                        richEndOfSegment.SelectionColor = Color.Red;
                    }
                    else if (a >= segStartAddr && a <= segEndAddr)
                    {
                        richEndOfSegment.SelectionColor = Color.Black;
                    }
                    else
                    {
                        richEndOfSegment.SelectionColor = Color.LightBlue;
                        otherSegAddr = a;
                    }
                    if (a==segEndAddr || a == (segStartAddr-1))
                        richEndOfSegment.AppendText(basefile.buf[a].ToString("X2") + "|");
                    else
                        richEndOfSegment.AppendText(basefile.buf[a].ToString("X2") + " ");
                }
                richEndOfSegment.SelectionColor = Color.Black;
                richEndOfSegment.AppendText(Environment.NewLine + "Black:" + basefile.Segments[seg].Name);
                string otherSegment = basefile.GetSegmentName(otherSegAddr);
                if (otherSegment != "")
                {
                    richEndOfSegment.SelectionColor = Color.LightBlue;
                    richEndOfSegment.AppendText(Environment.NewLine + "Lightblue:" + otherSegment);
                }
                richEndOfSegment.SelectionColor = Color.Red;
                richEndOfSegment.AppendText(Environment.NewLine + "Red:Selected bytes");
            }
            catch { };
        }

        private void comboFakeCvnSegment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int seg = comboFakeCvnSegment.SelectedIndex;

                if (radioFakeCVNRelativeAddr.Checked)
                {
                    txtFreeAddress.Text = (basefile.segmentAddressDatas[seg].SegmentBlocks[basefile.segmentAddressDatas[seg].SegmentBlocks.Count - 1].End - (int)numFakeCvnBytesFromEnd.Value).ToString("X");
                }
                else if (radioFakeCvnUseVer.Checked)
                {
                    txtFreeAddress.Text = basefile.segmentAddressDatas[seg].VerAddr.Address.ToString("X");
                    numFakeCvnBytes.Value = basefile.segmentAddressDatas[seg].VerAddr.Bytes;
                }
                getTargetCvn(seg);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void radioFakeCVNRelativeAddr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboFakeCvnSegment.Text == "")
                    return;
                int seg = comboFakeCvnSegment.SelectedIndex;
                txtFreeAddress.Text = (basefile.segmentAddressDatas[seg].SegmentBlocks[basefile.segmentAddressDatas[seg].SegmentBlocks.Count - 1].End - (int)numFakeCvnBytesFromEnd.Value).ToString("X");
                if (radioFakeCVNRelativeAddr.Checked)
                    numFakeCvnBytesFromEnd.Enabled = true;
                else
                    numFakeCvnBytesFromEnd.Enabled = false;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void numFakeCvnBytesFromEnd_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboFakeCvnSegment.Text == "" || !radioFakeCVNRelativeAddr.Checked)
                    return;
                int seg = comboFakeCvnSegment.SelectedIndex;   
                txtFreeAddress.Text = (basefile.segmentAddressDatas[seg].SegmentBlocks[basefile.segmentAddressDatas[seg].SegmentBlocks.Count - 1].End - (int)numFakeCvnBytesFromEnd.Value).ToString("X");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void radioFakeCvnUseVer_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioFakeCvnUseVer.Checked)
                {
                    txtFreeAddress.Enabled = false;
                    numFakeCvnBytes.Enabled = false;
                    if (comboFakeCvnSegment.Text.Length > 1)
                    {
                        txtFreeAddress.Text = basefile.segmentAddressDatas[comboFakeCvnSegment.SelectedIndex].VerAddr.Address.ToString("X");
                        numFakeCvnBytes.Value = basefile.segmentAddressDatas[comboFakeCvnSegment.SelectedIndex].VerAddr.Bytes;
                    }
                }
                else
                {
                    txtFreeAddress.Enabled = true;
                    numFakeCvnBytes.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void radioFakeCvnAddr_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFakeCvnAddr.Checked)
                txtFreeAddress.Enabled = true;
            else
                txtFreeAddress.Enabled = false;
        }

        private void txtTargetCVN_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioFakeCvnAllSegments_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFakeCvnAllSegments.Checked)
            {
                if (radioFakeCvnAddr.Checked)
                    radioFakeCVNRelativeAddr.Checked = true;
                radioFakeCvnAddr.Enabled = false;
                txtTargetCVN.Enabled = false;
            }
            else
            {
                radioFakeCvnAddr.Enabled = true;
                txtTargetCVN.Enabled = true;
            }

        }

        private void clearFakeCVN()
        {
            comboFakeCvnSegment.Items.Clear();
            comboFakeCvnSegment.Text = "";
            for (int s = 0; s < basefile.Segments.Count; s++)
                comboFakeCvnSegment.Items.Add(basefile.Segments[s].Name);
            txtTargetCVN.Text = "";
            txtFreeAddress.Text = "";
            numFakeCvnBytes.Value = 4;
            radioFakeCVNRelativeAddr.Checked = true;
            radioFakeCvnSingleSegment.Checked = true;
        }
        private void btnOpenBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "http://tis2web.service.gm.com/";
                Clipboard.SetText(labelFakeCvnPn.Text.Replace("P/N: ", ""));
                Logger("Added " + labelFakeCvnPn.Text + " to clipboard");
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void radioFakeCvnSingleSegment_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnFakeCvnAddtoStock_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboFakeCvnSegment.Text.Length == 0 || txtTargetCVN.Text.Length == 0)
                    return;
                int seg = comboFakeCvnSegment.SelectedIndex;
                CVN stock = new CVN();
                stock.cvn = txtTargetCVN.Text;
                stock.PN = basefile.segmentinfos[seg].PN;
                stock.SegmentNr = basefile.segmentinfos[seg].SegNr;
                stock.Ver = basefile.segmentinfos[seg].Ver;
                stock.XmlFile = basefile.configFile + ".xml";

                if (CheckStockCVN(stock.PN, stock.Ver, stock.SegmentNr, stock.cvn, false, basefile.configFileFullName) != "[stock]")
                {
                    //Add if not already in list
                    StockCVN.Add(stock);
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
                else
                {
                    Logger("Already in list: " + stock.cvn);
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] buf = new byte[1];

                string scriptFname = SelectFile("Select script file", "CSV Files (*.csv)|*.csv|All FIles (*.*)|*.*");
                if (scriptFname.Length == 0)
                    return;

                Logger("Rebuilding...");
                string line = "";
                StreamReader sr = new StreamReader(scriptFname);
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.StartsWith("Filename") && !line.StartsWith("#"))
                    {
                        string[] lParts = line.Split(';');
                        if (lParts[0].StartsWith("Fill:")) //Create and fill filebuffer
                        {
                            string[] fillParts = lParts[0].Replace("Fill:", "").Split(' ');
                            byte[] fillBytes = new byte[fillParts.Length];
                            for (int x = 0; x < fillParts.Length; x++)
                            {
                                HexToByte(fillParts[x], out fillBytes[x]);
                            }

                            if (lParts.Length < 2)
                            {
                                LoggerBold("Missing file size: " + line);
                                return;
                            }
                            uint fsize;
                            if (!HexToUint(lParts[1], out fsize))
                            {
                                LoggerBold("Can't convert file size form HEX: " + lParts[1]);
                                return;
                            }

                            buf = new byte[fsize];

                            uint addr = 0;
                            for (;addr <= (fsize - fillBytes.Length);)
                            {
                                for (int f=0; f < fillBytes.Length ; f++)
                                {
                                    buf[addr] = fillBytes[f];
                                    addr++;
                                }
                            }
                            
                        }
                        else
                        {
                            string[] fParts = line.Split(';');
                            if (!File.Exists(fParts[0]))
                            {
                                LoggerBold("File not found: " + fParts[0]);
                                return;
                            }
                            byte[] segment = ReadBin(fParts[0]);
                            uint offset = 0;
                            for (int x=1; x < fParts.Length;x++) //Part1, Part2...
                            {
                                string[] pParts = fParts[x].Split('-');
                                if (pParts.Length != 2)
                                {
                                    LoggerBold("Unknown segment Start - End: " + fParts[x]);
                                    return;
                                }

                                uint start;
                                uint end;
                                if (!HexToUint(pParts[0], out start))
                                {
                                    LoggerBold("Unknown segment Start - End: " + fParts[x]);
                                    return;
                                }
                                if (!HexToUint(pParts[1], out end))
                                {
                                    LoggerBold("Unknown segment Start - End: " + fParts[x]);
                                    return;
                                }
                                Array.Copy(segment, offset, buf, start, end -start + 1);
                                offset += end - start + 1;
                            }
                        }
                    }
                }
                string defName = Path.GetFileNameWithoutExtension(scriptFname) + "-rebuild.bin";
                string fName = SelectSaveFile("",defName);
                if (fName.Length == 0)
                    return;
                Logger("Writing to file: " + fName);
                WriteBinToFile(fName, buf);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void txtFreeAddress_TextChanged(object sender, EventArgs e)
        {
            showFakeCvnSelectedBytes();
        }


        private void btnVisualSegments_Click_1(object sender, EventArgs e)
        {
            frmVisualSegments fvs = new frmVisualSegments();
            fvs.Show();
            fvs.showChart(basefile);

        }

        private void createProgramShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCreateShortcuts fcs = new frmCreateShortcuts();
            fcs.Show();
        }
    }
}



