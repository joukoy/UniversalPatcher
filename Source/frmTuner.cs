using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using static UniversalPatcher.ExtensionMethods;
using static Upatcher;
using static Helpers;
using static UniversalPatcher.TreeParts;
using System.Threading.Tasks;
using System.Reflection;

namespace UniversalPatcher
{
    public partial class FrmTuner : Form
    {
        public FrmTuner(PcmFile PCM1, bool loadTableList = true)
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
            contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(Cms_Opening);

            PCM = PCM1;
            //tableDataList = PCM.tableDatas;
            if (PCM == null || PCM1.fsize == 0) return; //No file selected
            AddtoCurrentFileMenu(PCM);
            if (loadTableList)
                LoadConfigforPCM(ref PCM);
            SelectPCM();
        }

        public FrmTuner(PcmFile PCM1, frmHistogram frmH)
        {
            hstForm = frmH;
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
            contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(Cms_Opening);

            PCM = PCM1;
            //tableDataList = PCM.tableDatas;
            if (PCM == null || PCM1.fsize == 0) return; //No file selected
            AddtoCurrentFileMenu(PCM);
            LoadConfigforPCM(ref PCM);
            SelectPCM();
        }

        public enum DispMode
        {
            None,
            Tree,
            List
        }

        private class Rename
        {
            public Rename(int indx,string newName)
            {
                this.Indx = indx;
                this.Newname = newName;
            }
            public int Indx { get; set; }
            public string Newname { get; set; }
        }

        public class NaviSetting
        {
            public Guid TableSelection { get; set; }
            public string Filter { get; set; }
            public string FilterBy { get; set; }
            public List<string> NavPath { get; set; }
            public string Tab { get; set; }
        }
        public class SessionPcm
        {
            public SessionPcm()
            {
                NaviSettings = new List<NaviSetting>();
                TdListNames = new List<string>();
                TdListFiles = new List<string>();
            }
            public List<NaviSetting> NaviSettings { get; set; }
            public string BinFile { get; set; }
            public int NaviCurrent { get; set; }
            public int CurrentTdList { get; set; }
            public List<string> TdListNames { get; set; }
            public List<string> TdListFiles { get; set; }
        }

        public class SessionSettings
        {
            public SessionSettings()
            {
                Pcms = new List<SessionPcm>();
            }
            public string SessionName { get; set; }
            public bool MapSession { get; set; }
            public string SortBy { get; set; }
            public int SortIndex { get; set; }
            public SortOrder StrSortOrder { get; set; }
            public string CurrentBin { get; set; }
            public List<SessionPcm> Pcms { get; set; }
        }
        public PcmFile PCM;
        //private List<TableData> tableDataList;
        private string sortBy = "TableName";
        private int sortIndex = 0;
        private bool columnsModified = false;
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        //private BindingList<TableData> bindingsource = new BindingList<TableData>();
        private BindingList<TableData> filteredTableDatas = new BindingList<TableData>();
        SortOrder strSortOrder = SortOrder.Ascending;
        private TableData lastSelectTd;        
        int keyDelayCounter = 0;
        private SplitContainer splitTree;
        private TreeViewMS treeDimensions;
        private TreeViewMS treeMulti;
        private TreeViewMS treeValueType;
        private TreeViewMS treeCategory;
        private TreeViewMS treeSegments;
        private string[] GalleryArray;
        TabPage currentTab;
        int iconSize;
        public string currentBin = "A";
        bool ExtraOffsetFirstTry = true;
        private frmTableVisDouble ftvd;
        public int CompareSelection = 0;
        public int CompareType = 0;
        public bool ShowAsHex = false;
        public bool SwapXy = false;
        private DispMode DisplayMode = DispMode.None;
        bool Navigating = false;
        ToolTip NaviTip = new ToolTip();
        private string sessionname;
        private frmHistogram hstForm;
        public bool histogramTableSelectionEnabled;
        private FileTraceListener DebugFileListener;

        public String SessionName
        {
            get { return sessionname; }
            set
            {
                sessionname = value;
                if (TunerMain != null && myTab != null)
                {
                    myTab.Text = sessionname;
                }
            }
        }
        private bool mapSession = false;
        public frmTunerMain TunerMain;
        public TabPage myTab;
        private void frmTuner_Load(object sender, EventArgs e)
        {
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            selectedCompareBin = "";
            SetWorkingMode();
            disableConfigAutoloadToolStripMenuItem.Checked = AppSettings.disableTunerAutoloadSettings;
            chkShowCategorySubfolder.Checked = AppSettings.TableExplorerUseCategorySubfolder;
            chkAutoMulti1d.Checked = AppSettings.TunerAutomulti1d;
            numIconSize.Value = AppSettings.TableExplorerIconSize;
            numNaviMaxTablesPerNode.Value = AppSettings.NavigatorMaxTablesPerNode;
            numNaviMaxTablesTotal.Value = AppSettings.NavigatorMaxTablesTotal;
            chkShowTableCount.Checked = AppSettings.TunerShowTableCount;
            radioTreeMode.Checked = AppSettings.TunerTreeMode;
            radioListMode.Checked = !AppSettings.TunerTreeMode;
            SelectDispMode();

            //LogReceivers.Add(txtResult);

            if (AppSettings.MainWindowPersistence && hstForm != null)
            {
                if (AppSettings.TunerWindowSize.Width > 0 || AppSettings.TunerWindowSize.Height > 0)
                {
                    this.WindowState = AppSettings.TunerWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = AppSettings.TunerWindowLocation;
                    this.Size = AppSettings.TunerWindowSize;
                }
                if (AppSettings.TunerLogWindowSize.Width > 0 || AppSettings.TunerLogWindowSize.Height > 0)
                {
                    this.splitContainer2.SplitterDistance = AppSettings.TunerLogWindowSize.Width;
                    this.splitContainer1.SplitterDistance = AppSettings.TunerLogWindowSize.Height;
                }
                if (AppSettings.TunerListModeTreeWidth > 0)
                    splitContainerListMode.SplitterDistance = AppSettings.TunerListModeTreeWidth;

            }

            if (frmpatcher == null)
                patcherToolStripMenuItem.Visible = true;
            else
                patcherToolStripMenuItem.Visible = false;

            comboFilterBy.Items.Clear();
            TableData tdTmp = new TableData();
            comboFilterBy.Items.Add("All");
            foreach (var prop in tdTmp.GetType().GetProperties())
            {
                //Add to filter by-combo
                comboFilterBy.Items.Add(prop.Name);
                ToolStripMenuItem menuItem = new ToolStripMenuItem(prop.Name);
                menuItem.Name = prop.Name;
                contextMenuStrip2.Items.Add(menuItem);
                menuItem.Click += new EventHandler(columnSelection_Click);

            }
            string tunerCols = AppSettings.TunerModeColumns;
            if (!tunerCols.Contains("Address"))
                tunerCols = tunerCols.Trim(',') + ",Address";
            string[] sortStrs = tunerCols.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sortStr in sortStrs )
            {
                ToolStripMenuItem sortItem = new ToolStripMenuItem(sortStr);
                sortItem.Name = sortStr;
                if (sortBy == sortStr)
                    sortItem.Checked = true;
                sortByToolStripMenuItem.DropDownItems.Add(sortItem);
                sortItem.Click += SortItem_Click;
            }

            comboFilterBy.Text = "TableName";
            labelTableName.Text = "";
            //lastSelectedId = -1;
            lastSelectTd = null;
            filteredTableDatas = new BindingList<TableData>(PCM.tableDatas);
            bindingsource = new BindingSource(typeof(BindingList<TableData>), null);
            bindingsource.DataSource = filteredTableDatas;
            dataGridView1.DataSource = bindingsource;
            dataGridView1.AllowUserToAddRows = false;
            //FilterTables(false);

            this.AllowDrop = true;
            this.DragEnter += FrmTuner_DragEnter;
            this.DragDrop += FrmTuner_DragDrop;

            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView1.KeyUp += DataGridView1_KeyUp;
            dataGridView1.UserAddedRow += DataGridView1_UserAddedRow;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            revToolStripMenuItem.MouseHover += RevToolStripMenuItem_MouseHover;
            fwdToolStripMenuItem.MouseHover += RevToolStripMenuItem_MouseHover;
            revToolStripMenuItem.MouseDown += NaviMenuItem_MouseDown; 
            fwdToolStripMenuItem.MouseDown += NaviMenuItem_MouseDown;
            txtFilter.TextChanged += txFilter_TextChanged;
            if (hstForm != null)
            {
                splitContainerListMode.SplitterDistance = 200;
                this.splitContainer1.SplitterDistance = (int)(0.7 * splitContainer1.Width);
                histogramTableSelectionEnabled = true;
            }
        }


        private bool IsSessionModified()
        {
            bool modified = false;
            try
            {
                if (currentFileToolStripMenuItem.DropDownItems.Count == 0)
                {
                    return false;
                }
                if (string.IsNullOrEmpty(SessionName))
                {
                    return true;
                }
                string originalSesname = SessionName;
                string tmpsession = SessionName + "-tmp-" + DateTime.Now.ToString("HH-MM-dd-HH-ss-ffff");
                string tmpFolder = Path.Combine(Application.StartupPath, "TunerSessions", tmpsession);
                string sesFodler = Path.Combine(Application.StartupPath, "TunerSessions", SessionName);
                SaveSession(tmpsession, false);
                SessionName = originalSesname; //Return original session name
                DirectoryInfo d = new DirectoryInfo(sesFodler);
                FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);
                DirectoryInfo d2 = new DirectoryInfo(tmpFolder);
                FileInfo[] tmpFiles = d2.GetFiles("*.*", SearchOption.AllDirectories);

                if (Files.Length != tmpFiles.Length)
                {
                    modified = true;
                }
                else
                {
                    for (int f = 0; f < Files.Length; f++)
                    {
                        if (Files[f].Length != tmpFiles[f].Length)
                        {
                            modified = true;
                            break;
                        }
                        byte[] content1 = ReadBin(Files[f].FullName);
                        byte[] content2 = ReadBin(tmpFiles[f].FullName);
                        if (!content1.SequenceEqual(content2))
                        {
                            modified = true;
                            break;
                        }
                    }
                }
                foreach (FileInfo f in tmpFiles)
                {
                    File.Delete(f.FullName);
                }
                Directory.Delete(tmpFolder);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner line " + line + ": " + ex.Message);
            }
            return modified;
        }
        private void frmTuner_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (AppSettings.ConfirmProgramExit)
            {

                if (IsSessionModified())
                {
                    string title;
                    if (string.IsNullOrEmpty(SessionName))
                        title = "Save session?";
                    else
                        title = "Save session: " + SessionName + " ?";
                    DialogResult result = MessageBox.Show("Save session before closing?\nCancel=Don't close", title, MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        SaveSession(SessionName, true);
                    }
                }
                else if (currentFileToolStripMenuItem.DropDownItems.Count > 0)
                {
                    if (MessageBox.Show("Close Tuner?", "Exit now?", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            if (AppSettings.MainWindowPersistence && hstForm != null)
            {
                AppSettings.TunerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    AppSettings.TunerWindowLocation = this.Location;
                    AppSettings.TunerWindowSize = this.Size;
                }
                else
                {
                    AppSettings.TunerWindowLocation = this.RestoreBounds.Location;
                    AppSettings.TunerWindowSize = this.RestoreBounds.Size;
                }
                Size logSize = new Size();
                logSize.Width = this.splitContainer2.SplitterDistance;
                logSize.Height = this.splitContainer1.SplitterDistance;
                AppSettings.TunerLogWindowSize = logSize;
                AppSettings.TunerListModeTreeWidth = splitContainerListMode.SplitterDistance;
            }
            AppSettings.Save();
            if (PCM.BufModified())
            {
                DialogResult res = MessageBox.Show("File modified.\n\rSave modifications before closing?", "Save changes?", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                if (res == DialogResult.Yes)
                {
                    Logger("Saving to file: " + PCM.FileName);
                    PCM.SaveBin(PCM.FileName);
                    Logger("File saved");
                }
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
        }

        private void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            int col = dataGridView1.SelectedCells[0].ColumnIndex;
            if (dataGridView1.Columns[col].Name == "ExtraOffset") 
            {
                if (e.KeyChar == (char)Keys.Space)
                {
                    dataGridView1.EndEdit();
                    TableData selTd = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                    ShowTableDescription(PCM, selTd);
                    numExtraOffset.Value = selTd.extraoffset;
                    e.Handled = true;
                }
            }
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, txtResult);
        }
        
        private void SortItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem sortItem = (ToolStripMenuItem)sender;
            sortItem.Checked = true;
            sortBy = sortItem.Name;
            foreach (ToolStripMenuItem mi in sortByToolStripMenuItem.DropDownItems)
            {
                if (mi.Name != sortItem.Name)
                    mi.Checked = false;
            }
            SortTables();
        }

        private void FrmTuner_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> fileList = new List<string>();
            foreach (string file in files)
                fileList.Add(file);
            OpenNewBinFile(false, fileList);
        }

        private void FrmTuner_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }


        public void SelectPCM()
        {
            try
            {
                this.Text = "Tuner " + PCM.FileName + " [" + PCM.tunerFile + "]";
                PCM.SelectTableDatas(0, PCM.FileName);
                if (TunerMain != null)
                {
                    TunerMain.Text = "Tuner " + PCM.FileName + " [" + PCM.tunerFile + "]";
                }
/*                frmNavigator fn = new frmNavigator();
                fn.Show();
                fn.PCM = PCM;
                fn.dataGridView2.DataSource = fn.PCM.NaviGator;
*/
                //tableDataList = PCM.tableDatas;
                for (int m = tableListToolStripMenuItem.DropDownItems.Count - 1; m >= 0; m--)
                {

                    if (tableListToolStripMenuItem.DropDownItems[m].Tag != null)
                        tableListToolStripMenuItem.DropDownItems.RemoveAt(m);
                }
                for (int i = 0; i < PCM.altTableDatas.Count; i++)
                {
                    ToolStripMenuItem miNew = new ToolStripMenuItem(PCM.altTableDatas[i].Name);
                    miNew.Name = PCM.altTableDatas[i].Name;
                    miNew.Tag = i;
                    if (i == 0)
                        miNew.Checked = true;
                    tableListToolStripMenuItem.DropDownItems.Add(miNew);
                    miNew.Click += tablelistSelect_Click;
                }
                if (PCM.Segments.Count > 0 && PCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                    tinyTunerDBV6OnlyToolStripMenuItem.Enabled = true;
                else
                    tinyTunerDBV6OnlyToolStripMenuItem.Enabled = false;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                    mi.Checked = false;
                foreach (ToolStripMenuItem mi in findDifferencesToolStripMenuItem.DropDownItems)
                    mi.Enabled = true;
                foreach (ToolStripMenuItem mi in findDifferencesHEXToolStripMenuItem.DropDownItems)
                    mi.Enabled = true;

                foreach (ToolStripMenuItem fmi in addSegmentOffsetNodeToolStripMenuItem.DropDownItems)
                {
                    if (fmi.Text == PCM.FileName)
                        fmi.Enabled = false;
                    else
                        fmi.Enabled = true;
                }
                findDifferencesToolStripMenuItem.DropDownItems[PCM.FileName].Enabled = false;
                findDifferencesHEXToolStripMenuItem.DropDownItems[PCM.FileName].Enabled = false;

                ToolStripMenuItem mitem = (ToolStripMenuItem)currentFileToolStripMenuItem.DropDownItems[PCM.FileName];
                mitem.Checked = true;
                string tmpFL = currentBin;
                currentBin = GetFileLetter(mitem.Text);
                if (currentBin == selectedCompareBin)
                    selectedCompareBin = tmpFL;
                RefreshListModeTree();
                Navigate(0);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner line " + line + ": " + ex.Message);
            }
        }

        private void RefreshListModeTree()
        {
            treeView1.AfterSelect -= TreeView1_AfterSelect;
            FilterTables(false);
            treeView1.SelectedNodes.Clear();
            treeView1.Nodes.Clear();
            AddNodes(treeView1.Nodes, PCM, PCM.tableDatas, true);
            if (PCM.SelectedNode.ContainsKey(treeView1.Name))
            {
                List<string> path = PCM.SelectedNode[treeView1.Name];
                RestoreNodePath(treeView1, path, PCM);
            }
            treeView1.AfterSelect += TreeView1_AfterSelect;

        }

        public void LoadConfigforPCM(ref PcmFile newPCM)
        {
            try
            {
                if (!AppSettings.disableTunerAutoloadSettings)
                {
                    newPCM.AutoLoadTunerConfig();
                    if (newPCM.tunerFile.Length == 0)
                    {
                        ImportTableSeek(ref newPCM);
                        if (newPCM.Segments.Count > 0 && newPCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                            ImportTinyTunerDB(ref newPCM);
                    }
                    ImportDTC(ref newPCM);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner line " + line + ": " + ex.Message);
            }
        }

        public void OpenTableEditor(List<TableData> tableTds = null, bool newWindow = false)
        {
            try
            {
                if (PCM.buf == null || PCM.buf.Length == 0)
                {
                    Logger("No file loaded");
                    return;
                }
                if (tableTds == null)
                    tableTds = new List<TableData>();
                if (tableTds.Count == 0)
                {
                    tableTds = GetSelectedTableTds();
                }
                if (tableTds == null || tableTds.Count == 0 || tableTds[0] == null)
                    return;
                TableData td = tableTds[0];
                if (!string.IsNullOrEmpty(td.Values) && td.Values.StartsWith("Patch:"))
                {
                    if (MessageBox.Show(td.TableDescription, "Apply patch?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ApplyTdPatch(td, ref PCM);
                    }
                    return;
                }
                else if (!string.IsNullOrEmpty(td.Values) && td.Values.StartsWith("TablePatch:"))
                {
                    if (MessageBox.Show(td.TableDescription, "Apply patch?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ApplyTdTablePatch(ref PCM, td);
                    }
                    return;
                }

                if (td.addrInt == uint.MaxValue)
                {
                    Logger("No address defined!");
                    return;
                }

                if (!string.IsNullOrEmpty(td.OS) && !td.OS.Contains(PCM.OS) && !td.CompatibleOS.Contains("," + PCM.OS + ","))
                {
                    LoggerBold("WARING! OS Mismatch, File OS: " + PCM.OS + ", config OS: " + td.OS);                
                }

                frmTableEditor frmT;
                if (DisplayMode == DispMode.Tree && !newWindow && splitTree.Panel2.Controls.Count > 0 && splitTree.Panel2.Controls[0].Name =="frmTableEditor")
                {
                    frmT = (frmTableEditor)splitTree.Panel2.Controls[0];
                    frmT.SaveOnExit();
                }
                else
                {
                    ClearPanel2();
                    frmT = new frmTableEditor();
                    frmT.tuner = this;
                    if (DisplayMode == DispMode.Tree && !newWindow)
                    {
                        frmT.Dock = DockStyle.Fill;
                        frmT.FormBorderStyle = FormBorderStyle.None;
                        frmT.TopLevel = false;
                        splitTree.Panel2.Controls.Add(frmT);
                    }
                }
                if (DisplayMode == DispMode.List)
                {
                    frmT.tunerSelectedTables = filteredTableDatas.ToList();
                }
                else
                {
                    TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                    Tnode tnode;
                    if (tv.SelectedNode.Parent == null)
                    {
                        tnode = (Tnode)tv.SelectedNode.Tag;
                    }
                    else
                    {
                        tnode = (Tnode)tv.SelectedNode.Parent.Tag;
                    }
                    frmT.tunerSelectedTables = tnode.filteredTds;
                }
                frmT.disableMultiTable = disableMultitableToolStripMenuItem.Checked;
                frmT.CleanUp();
                frmT.PrepareTable(PCM, td, tableTds, currentBin);
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile comparePCM = (PcmFile)mi.Tag;
                    if (PCM.FileName != comparePCM.FileName)
                    {
                        Logger("Adding file: " + Path.GetFileName(comparePCM.FileName) + " to compare menu... ", false);
                        {
                            TableData xTd = null;
                            for (int y = 0; y < tableTds.Count; y++)
                            {
                                TableData tmpTd = tableTds[y];
                                xTd = FindTableData(tmpTd, comparePCM.tableDatas);
                                if (xTd != null)
                                    break;
                            }
                            if (xTd == null)
                            {
                                LoggerBold("Table not found");
                            }
                            else
                            {
                                frmT.AddCompareFiletoMenu(comparePCM, null, mi.Text, selectedCompareBin);
                                if (PCM.configFile != comparePCM.configFile)
                                {
                                    LoggerBold(Environment.NewLine + "Warning: file type different, results undefined!");
                                }
                                else
                                {
                                    Logger("[OK]");
                                }
                            }
                        }
                    }
                }
                frmT.Show();
                frmT.LoadTable();
                if (DisplayMode == DispMode.List)
                {
                    dataGridView1.Update();
                    dataGridView1.Refresh();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            OpenTableEditor();
        }

        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            txtResult.AppendText(LogText);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        private void SaveBin()
        {
            try
            {
                if (PCM == null || PCM.buf == null | PCM.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string fileName = SelectSaveFile(BinFilter, PCM.FileName);
                if (fileName.Length == 0)
                    return;

                Logger("Saving to file: " + fileName);
                ClearPanel2();
                PcmFile newPCM = PCM.ShallowCopy();
                newPCM.SaveBin(fileName);
                AddtoCurrentFileMenu(newPCM);
                PCM.ReloadBinFile();
                PCM = newPCM;
                SelectPCM();
                Logger("Done.");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void btnSaveBin_Click(object sender, EventArgs e)
        {
            SaveBin();
        }

        public void RefreshFast()
        {
            //bindingsource = filteredTableDatas;
            this.Invoke((MethodInvoker)delegate ()
            {
                dataGridView1.Update();
                dataGridView1.Refresh();
            });
        }

        private void RefreshCategories()
        {
            //Don't fire events when adding data to combobox!
            this.comboTableCategory.SelectedIndexChanged -= new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);
            string selectedCat = comboTableCategory.Text;
            comboTableCategory.DataSource = null;
            categoryBindingSource.DataSource = null;
            categoryBindingSource.DataSource = PCM.tableCategories.OrderBy(x => x);
            comboTableCategory.DataSource = categoryBindingSource;
            comboTableCategory.Refresh();
            if (comboTableCategory.Items.Contains(selectedCat))
                comboTableCategory.Text = selectedCat;
            this.comboTableCategory.SelectedIndexChanged += new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);
        }

        public void RefreshTablelist(bool RestorePath)
        {
            try
            {
                this.dataGridView1.SelectionChanged -= new System.EventHandler(this.DataGridView1_SelectionChanged);

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                RefreshCategories();
                Application.DoEvents();
                FilterTables(RestorePath);
                treeView1.SelectedNodes.Clear();
                treeView1.Nodes.Clear();
                DrawingControl.SuspendDrawing(treeView1);
                TreeParts.AddNodes(treeView1.Nodes, PCM, filteredTableDatas.ToList(), true);
                DrawingControl.ResumeDrawing(treeView1);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner line " + line + ": " + ex.Message);
            }
        }

        private void ImportTableSeek(ref PcmFile _PCM)
        {
            Logger("Importing tableseek...", false);
            Application.DoEvents();
            _PCM.ImportSeekTables();
            Logger(" [OK]");
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            PCM.LoadTableList();
            RefreshTablelist(false);
        }


        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            PCM.SaveTableList("");
        }

        private void btnImportDTC_Click(object sender, EventArgs e)
        {
            ImportDTC(ref PCM);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            PCM.ClearTableList();
            RefreshTablelist(false);
        }

        private void ReorderColumns()
        {
            try
            {
                string[] tunerColumns = AppSettings.TunerModeColumns.ToLower().Split(',');
                string[] configColumns = AppSettings.ConfigModeColumnOrder.ToLower().Split(',');
                string[] configWidth = AppSettings.ConfigModeColumnWidth.Split(',');
                string[] tunerWidth = AppSettings.TunerModeColumnWidth.Split(',');
                Debug.WriteLine("Tuner order: " + AppSettings.TunerModeColumns);
                Debug.WriteLine("Config order: " + AppSettings.ConfigModeColumnOrder);

                int invisibleIndex = tunerColumns.Length;
                for (int c = 0; c < dataGridView1.Columns.Count && c < configWidth.Length; c++)
                {
                    if (AppSettings.WorkingMode == 2) //advanced
                    {
                        dataGridView1.Columns[c].ReadOnly = false;
                        dataGridView1.Columns[c].Visible = true;
                        int order = Array.IndexOf(configColumns, dataGridView1.Columns[c].HeaderText.ToLower());
                        if (order > -1 && order < dataGridView1.Columns.Count)
                        {
                            dataGridView1.Columns[c].DisplayIndex = order;
                            if (c < configWidth.Length)
                            {
                                dataGridView1.Columns[c].Width = Convert.ToInt32(configWidth[c]);
                            }
                        }
                    }
                    else
                    {
                        dataGridView1.Columns[c].ReadOnly = true;
                        if (dataGridView1.Columns[c].HeaderText.ToLower() == "id")
                        {
                            dataGridView1.Columns[c].DisplayIndex = 0;
                            dataGridView1.Columns[c].Visible = true;
                            dataGridView1.Columns[c].Width = Convert.ToInt32(tunerWidth[c]);
                        }
                        else if (tunerColumns.Contains(dataGridView1.Columns[c].HeaderText.ToLower()))
                        {
                            dataGridView1.Columns[c].Visible = true;
                            if (c < tunerWidth.Length)
                            {
                                dataGridView1.Columns[c].Width = Convert.ToInt32(tunerWidth[c]);
                            }
                            int order = Array.IndexOf(tunerColumns, dataGridView1.Columns[c].HeaderText.ToLower());
                            if (order > -1 && order < dataGridView1.Columns.Count)
                                dataGridView1.Columns[c].DisplayIndex = order;
                        }
                        else
                        {
                            dataGridView1.Columns[c].DisplayIndex = invisibleIndex;
                            invisibleIndex++;
                            dataGridView1.Columns[c].Visible = false;
                        }
                    }
                    //Debug.WriteLine("Column: " + c + ":, " + dataGridView1.Columns[c].HeaderText + ", index: ", dataGridView1.Columns[c].DisplayIndex.ToString());
                }
                if (sortIndex < dataGridView1.Columns.Count)
                {
                    dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
                }
                Debug.WriteLine("Reorder done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        private void SortTables()
        {
            this.dataGridView1.SelectionChanged -= new System.EventHandler(this.DataGridView1_SelectionChanged);
            DrawingControl.SuspendDrawing(dataGridView1);
            List<TableData> results = filteredTableDatas.ToList();
            if (strSortOrder == SortOrder.Ascending)
                results = results.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
            else
                results = results.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();

            filteredTableDatas = new BindingList<TableData>(results.ToList());

            dataGridView1.DataSource = filteredTableDatas;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
            DrawingControl.ResumeDrawing(dataGridView1);
            dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
            FilterTree(true);
        }
        private void FilterTables(bool RestorePath)
        {
            try
            {
                this.dataGridView1.SelectionChanged -= new System.EventHandler(this.DataGridView1_SelectionChanged);
                DrawingControl.SuspendDrawing(dataGridView1);

                //if (PCM == null || PCM.fsize == 0) return;
                if (PCM == null || PCM.tableDatas == null)
                    return;
                //Save settings before reordering
                SaveGridLayout();

                List<TableData> compareList = PCM.tableDatas;
                Debug.WriteLine("Total table count: " + compareList.Count.ToString());
                if (showOnlyMappedTablesToolStripMenuItem.Checked)
                {
                    compareList = compareList.Where(X => X.ExtraOffset != "0" ).ToList();
                    Debug.WriteLine("Tables with Extra offset: " + compareList.Count.ToString());
                }
                if (!AppSettings.TunerShowUnitsUndefined || !AppSettings.TunerShowUnitsMetric || !AppSettings.TunerShowUnitsImperial)
                {
                    List<TableData> newTDList = new List<TableData>();
                    if (AppSettings.TunerShowUnitsUndefined)
                    {
                        newTDList.AddRange(compareList.Where(x => x.DispUnits == DisplayUnits.Undefined).ToArray());
                    }
                    if (AppSettings.TunerShowUnitsImperial)
                    {
                        newTDList.AddRange(compareList.Where(x => x.DispUnits == DisplayUnits.Imperial).ToArray());
                    }
                    if (AppSettings.TunerShowUnitsMetric)
                    {
                        newTDList.AddRange(compareList.Where(x => x.DispUnits == DisplayUnits.Metric).ToArray());
                    }
                    compareList = newTDList;
                }

                if (strSortOrder == SortOrder.Ascending)
                    compareList = compareList.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    compareList = compareList.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();

                IEnumerable<TableData> results = compareList;

                if (txtFilter.Text.Length > 0)
                {
                    string newStr = txtFilter.Text.Replace("OR", "|");
                    if (newStr.StartsWith("%difference:"))
                    {
                        string[] dparts = newStr.Split(':');
                        if (dparts.Length == 3)
                        {
                            PcmFile cmpPcm1 = null;
                            PcmFile cmpPcm2 = null;
                            for (int m = 0; m < currentFileToolStripMenuItem.DropDownItems.Count; m++)
                            {
                                ToolStripItem ti = currentFileToolStripMenuItem.DropDownItems[m];
                                if (ti.Text.StartsWith(dparts[1]))
                                {
                                    cmpPcm1 = (PcmFile)ti.Tag;
                                }
                                if (ti.Text.StartsWith(dparts[2]))
                                {
                                    cmpPcm2 = (PcmFile)ti.Tag;
                                }
                            }
                            if (cmpPcm1 == null)
                            {
                                Debug.WriteLine("Bin " + dparts[1] + " missing");
                                return;
                            }
                            if (cmpPcm2 == null)
                            {
                                Debug.WriteLine("Bin " + dparts[2] + " missing");
                                return;
                            }
                            List<TableData> newTDList = new List<TableData>();
                            for (int t=0;t< compareList.Count();t++)
                            {
                                TableData cmpTd2 = FindTableData(compareList[t], cmpPcm2.tableDatas);
                                if (!CompareTables(compareList[t],cmpTd2,cmpPcm1,cmpPcm2))
                                {
                                    newTDList.Add(compareList[t]);
                                }
                            }
                            compareList = newTDList;
                            results = compareList;
                        }
                    }
                    else if (newStr.Contains("|"))
                    {
                        string[] orStr = newStr.Split('|');
                        List<TableData> newTDList = new List<TableData>();
                        TableData tdTmp = new TableData();
                        if (comboFilterBy.Text == "All")
                        {
                            foreach (var prop in tdTmp.GetType().GetProperties())
                            {
                                foreach (string orS in orStr)
                                {
                                    List<TableData> tmpRes = FilterTdList(results, orS.Trim(), prop.Name, caseSensitiveFilteringToolStripMenuItem.Checked);
                                    foreach (TableData td in tmpRes)
                                    {
                                        bool isInList = false;
                                        foreach (TableData nTd in newTDList)
                                        {
                                            if (td.guid == nTd.guid)
                                            {
                                                isInList = true;
                                                break;
                                            }
                                        }
                                        if (!isInList)
                                            newTDList.Add(td);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (string orS in orStr)
                            {
                                List<TableData> tmpRes = FilterTdList(results, orS.Trim(), comboFilterBy.Text, caseSensitiveFilteringToolStripMenuItem.Checked);
                                foreach (TableData td in tmpRes)
                                    newTDList.Add(td);
                            }
                        }
                        results = newTDList;
                    }
                    else
                    {
                        newStr = txtFilter.Text.Replace("AND", "&");
                        string[] andStr = newStr.Split('&');
                        if (comboFilterBy.Text == "All")
                        {
                            List<TableData> newTDList = new List<TableData>();
                            TableData tdTmp = new TableData();
                            foreach (var prop in tdTmp.GetType().GetProperties())
                            {
                                Debug.WriteLine(prop.Name);
                                List<TableData> tmpRes = results.ToList();
                                foreach (string sStr in andStr)
                                {
                                    tmpRes = FilterTdList(tmpRes, sStr, prop.Name, caseSensitiveFilteringToolStripMenuItem.Checked);
                                }
                                foreach (TableData td in tmpRes)
                                {
                                    bool isInList = false;
                                    foreach (TableData nTd in newTDList)
                                    {
                                        if (td.guid == nTd.guid)
                                        {
                                            isInList = true;
                                            break;
                                        }
                                    }
                                    if (!isInList)
                                        newTDList.Add(td);
                                }
                            }
                            results = newTDList;
                        }
                        else
                        {
                            foreach (string sStr in andStr)
                            {
                                results = FilterTdList(results, sStr, comboFilterBy.Text, caseSensitiveFilteringToolStripMenuItem.Checked);
                            }
                        }
                    }
                }

                if (!showTablesWithEmptyAddressToolStripMenuItem.Checked)
                {
                    Debug.WriteLine("Filtering tables with empty address");
                    results = results.Where(t => t.addrInt < uint.MaxValue);
                    Debug.WriteLine("OK");
                }

                string cat = comboTableCategory.Text;
                if (cat != "_All" && cat != "")
                {
                    Debug.WriteLine("Filtering tablesby category");
                    if (cat.StartsWith("Seg-"))
                    {
                        string seg = cat.Substring(4, cat.Length - 4);
                        int segNr = 0;
                        for (int s = 0; s < PCM.segmentinfos.Length; s++)
                            if (PCM.segmentinfos[s].Name == seg)
                                segNr = s;
                        uint addrStart = PCM.segmentAddressDatas[segNr].SegmentBlocks[0].Start;
                        uint addrEnd = PCM.segmentAddressDatas[segNr].SegmentBlocks[PCM.segmentAddressDatas[segNr].SegmentBlocks.Count - 1].End;
                        results = results.Where(t => t.addrInt >= addrStart && t.addrInt <= addrEnd);
                    }
                    else
                    {
                        results = results.Where(t => t.Category.ToLower() == comboTableCategory.Text.ToLower());
                    }
                    Debug.WriteLine("OK");
                }

/*                if (DisplayMode == DispMode.List && !treeView1.Nodes["All"].IsSelected && !treeView1.Nodes["Patches"].IsSelected && treeView1.SelectedNodes.Count > 0)
                {
                    Debug.WriteLine("Filtering by tree");
                    List<TableData> newTDList = new List<TableData>();
                    foreach (TreeNode tn in treeView1.SelectedNodes)
                    {
                        TreeParts.Tnode tnode = (TreeParts.Tnode)tn.Tag;
                        newTDList.AddRange(tnode.filteredTds);
                    }
                    results = newTDList;
                    Debug.WriteLine("OK");
                    Debug.WriteLine("Total table count, filtered by selected tree node: " + compareList.Count.ToString());
                }
*/
                filteredTableDatas = new BindingList<TableData>(results.ToList());

                dataGridView1.DataSource = filteredTableDatas;
                //bindingsource = filteredTableDatas;
                ReorderColumns();
                txtDescription.Text = "";
                FilterTree(RestorePath);
                this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
                Debug.WriteLine("Enable drawings");
                DrawingControl.ResumeDrawing(dataGridView1);
                Debug.WriteLine("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                Debug.WriteLine("frmTuner, line: " + line + ", " + ex.Message);
            }

        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //Debug.WriteLine("End edit");
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Category" || dataGridView1.Columns[e.ColumnIndex].Name == "ExtraCatefories")
            {
                Debug.WriteLine("Refresh cats");
                RefreshCategories();
            }
            this.Refresh();
        }

        private void FilterTree(bool RestorePath)
        {
            try
            {
                Debug.WriteLine("Filter tree, RestorePath: " + RestorePath.ToString());
                if (DisplayMode != DispMode.Tree)
                {
                    return;
                }
                if (tabControl1.SelectedTab == null)
                    tabControl1.SelectedTab = tabMultiTree;
                DrawingControl.SuspendDrawing(treeView1);
                if (tabControl1.SelectedTab == tabDimensions)
                    LoadDimensions();
                else if (tabControl1.SelectedTab == tabValueType)
                    LoadValueTypes();
                else if (tabControl1.SelectedTab == tabCategory)
                    LoadCategories();
                else if (tabControl1.SelectedTab == tabSegments)
                    LoadSegments();
                else if (tabControl1.SelectedTab == tabMultiTree)
                    LoadMultiTree();
                DrawingControl.ResumeDrawing(treeView1);
                if (tabControl1.SelectedTab.Controls[0].GetType() != typeof(TreeViewMS))
                {
                    Debug.WriteLine("control 0 is not TreeViewMS (" + tabControl1.SelectedTab.Controls[0].GetType().ToString() + ")");
                    return;
                }
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                if (PCM.SelectedNode.ContainsKey(tv.Name) && RestorePath && !Navigating)
                {
                    Debug.WriteLine("FilterTree, restoring path");
                    List<string> path = PCM.SelectedNode[tv.Name];
                    if (path != null && tabControl1.SelectedTab != tabSettings && tabControl1.SelectedTab != tabFileInfo && tabControl1.SelectedTab != tabPatches)
                        RestoreNodePath(tv, path, PCM);
                }
                //tv.AfterSelect += Tree_AfterSelect;
                //tv.NodeMouseClick += Tree_NodeMouseClick;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                Debug.WriteLine("frmTuner, line: " + line + ", " + ex.Message);
            }
        }
        private void comboTableCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("comboTableCategory_SelectedIndexChanged");
            //FilterTables(true);
            RefreshTablelist(true);
        }

        private void btnSearchTableSeek_Click(object sender, EventArgs e)
        {
            try
            {
                int rowindex = dataGridView1.CurrentCell.RowIndex;
                for (int i = rowindex + 1; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["TableName"].Value != null && dataGridView1.Rows[i].Cells["TableName"].Value.ToString().ToLower().Contains(txtFilter.Text.ToLower()))
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                        dataGridView1.CurrentCell.Selected = true;
                        break;
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
                LoggerBold("Error, frmTuner line " + line + ": " + ex.Message);
            }
        }

        private void ImportTinyTunerDB(ref PcmFile _PCM)
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.ReadTinyDBtoTableData(_PCM, _PCM.tableDatas));

        }

        private void btnReadTinyTunerDB_Click(object sender, EventArgs e)
        {
            ImportTinyTunerDB(ref PCM);
            RefreshTablelist(false);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.LoadTableList();
            comboTableCategory.Text = "_All";
            RefreshTablelist(false);

            //currentXmlFile = PCM.configFileFullName;
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            PCM.SaveTableList("");
        }

        private void saveBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            ClearPanel2();
            PCM.SaveBin(PCM.FileName);
            Logger("Done.");
        }

        private void ImportDTC(ref PcmFile _PCM)
        {
            try
            {
                Logger("Importing DTC codes...", false);
                bool haveDTC = false;
/*              for (int t = 0; t < _PCM.tableDatas.Count; t++)
                {
                    if (_PCM.tableDatas[t].TableName == "DTC" || _PCM.tableDatas[t].TableName == "DTC.Codes")
                    {
                        haveDTC = true;
                        Logger(" DTC codes already defined");
                        break;
                    }
                }
*/
                if (!haveDTC)
                {
                    TableData tdTmp = new TableData();
                    tdTmp.ImportDTC(_PCM, ref _PCM.tableDatas,true);
                    tdTmp.ImportDTC(_PCM, ref _PCM.tableDatas, false);
                    Logger(" [OK]");
                    //FilterTables(true);
                    RefreshTablelist(true);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner importDTC, line " + line + ": " + ex.Message);
            }
        }

        private void clearTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.ClearTableList();
            //PCM.tableCategories = new List<string>();
            RefreshTablelist(false);
        }

        private void EnableTreeModeAxisMenus(TableData selTd)
        {
            if (selTd.ColumnHeaders.ToLower().StartsWith("table:") || selTd.ColumnHeaders.ToLower().StartsWith("guid:"))
            {
                xAxisToolStripMenuItem.Enabled = true;
            }
            else
            {
                xAxisToolStripMenuItem.Enabled = false;
            }
            if (selTd.RowHeaders.ToLower().StartsWith("table:") || selTd.RowHeaders.ToLower().StartsWith("guid:"))
            {
                yAxisToolStripMenuItem.Enabled = true;
            }
            else
            {
                yAxisToolStripMenuItem.Enabled = false;
            }
            if (selTd.Math.ToLower().Contains("table:"))
            {
                mathToolStripMenuItem.Enabled = true;
            }
            else
            {
                mathToolStripMenuItem.Enabled = false;
            }
        }

        private void EnableAxisTableMenus(TableData selTd)
        {
            if (selTd.ColumnHeaders.ToLower().StartsWith("table:") || selTd.ColumnHeaders.ToLower().StartsWith("guid:"))
            {
                editXaxisTableToolStripMenuItem.Enabled = true;
                openXaxisTableToolStripMenuItem.Enabled = true;
            }
            else
            {
                editXaxisTableToolStripMenuItem.Enabled = false;
                openXaxisTableToolStripMenuItem.Enabled = false;
            }

            if (selTd.RowHeaders.ToLower().StartsWith("table:") || selTd.RowHeaders.ToLower().StartsWith("guid:"))
            {
                editYaxisTableToolStripMenuItem.Enabled = true;
                openYaxisTableToolStripMenuItem.Enabled = true;
            }
            else
            {
                editYaxisTableToolStripMenuItem.Enabled = false;
                openYaxisTableToolStripMenuItem.Enabled = false;
            }
            if (selTd.Math.ToLower().Contains("table:"))
            {
                editMathtableToolStripMenuItem.Enabled = true;
                openMathtableToolStripMenuItem.Enabled = true;
            }
            else
            {
                editMathtableToolStripMenuItem.Enabled = false;
                openMathtableToolStripMenuItem.Enabled = false;
            }

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
/*                if (e.RowIndex == -1 && e.Button == MouseButtons.Right)
                {
                    contextMenuStrip2.Show(Cursor.Position.X, Cursor.Position.Y);
                }
                else 
*/
                if (e.Button == MouseButtons.Left)
                {
                    SaveCurrentPath(treeView1);
                }
                if (AppSettings.WorkingMode > 0 && dataGridView1.SelectedCells.Count > 0 && e.Button == MouseButtons.Right)
                {
                    lastSelectTd = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                    contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
                    EnableAxisTableMenus(lastSelectTd);
                }
            }
            catch { }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView g = sender as DataGridView;
            if (g != null)
            {
                Point p = g.PointToClient(MousePosition);
                DataGridView.HitTestInfo hti = g.HitTest(p.X, p.Y);
                if (hti.Type != DataGridViewHitTestType.ColumnHeader && hti.Type != DataGridViewHitTestType.RowHeader)
                    OpenTableEditor();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Copy to clipboard
                CopyToClipboard();

                //Clear selected cells
                foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                    dgvCell.Value = string.Empty;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Perform paste Operation
            PasteClipboardValue();
        }

        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void PasteClipboardValue()
        {
            try
            {
                //Show Error if no cell is selected
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please select a cell", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Get the starting Cell
                DataGridViewCell startCell = GetStartCell(dataGridView1);
                //Get the clipboard value in a dictionary
                Dictionary<int, Dictionary<int, string>> cbValue =
                        ClipBoardValues(Clipboard.GetText());

                int iRowIndex = startCell.RowIndex;
                foreach (int rowKey in cbValue.Keys)
                {
                    int iColIndex = startCell.ColumnIndex;
                    foreach (int cellKey in cbValue[rowKey].Keys)
                    {
                        //Check if the index is within the limit
                        if (iColIndex <= dataGridView1.Columns.Count - 1
                        && iRowIndex <= dataGridView1.Rows.Count - 1)
                        {
                            DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                            //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                            /*if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                                (!chkPasteToSelectedCells.Checked))*/
                            cell.Value = cbValue[rowKey][cellKey];
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionary with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }

        private void editTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TableData> tableTds = GetSelectedTableTds();
            OpenTableEditor(tableTds, true);
        }

        private void ExportCSV()
        {
            try
            {
                string FileName = SelectSaveFile(CsvFilter);
                if (FileName.Length == 0)
                    return;
                Logger("Writing to file: " + Path.GetFileName(FileName), false);
                using (StreamWriter writetext = new StreamWriter(FileName))
                {
                    string row = "";
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        row += dataGridView1.Columns[i].HeaderText;
                    }
                    writetext.WriteLine(row);
                    for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                    {
                        row = "";
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            if (i > 0)
                                row += ";";
                            if (dataGridView1.Rows[r].Cells[i].Value != null)
                                row += dataGridView1.Rows[r].Cells[i].Value.ToString();
                        }
                        writetext.WriteLine(row);
                    }
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        private void exportCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ImportxperimentalCSV()
        {
            try
            {
                for (int i = 0; i < PCM.tableDatas.Count; i++)
                    PCM.tableDatas[i].addrInt = uint.MaxValue;
                string fileName = SelectFile("Select CSV File", CsvFilter);
                if (fileName.Length == 0)
                    return;
                Logger("Loading file: " + fileName, false);
                //string osNew = Path.GetFileName(FileName).Replace("Addresses-", "").Replace(".csv","");
                string osNew = "12587603";
                StreamReader sr = new StreamReader(fileName);
                string csvLine;
                while ((csvLine = sr.ReadLine()) != null)
                {
                    string[] cParts = csvLine.Split(',');
                    if (cParts.Length > 2)
                    {
                        string cat = cParts[0];
                        string name = cParts[1];
                        string addr = cParts[2];
                        bool found = false;
                        for (int i = 0; i < PCM.tableDatas.Count; i++)
                        {
                            if (PCM.tableDatas[i].Category == cat && PCM.tableDatas[i].TableName.ToLower() == name.ToLower())
                            {
                                PCM.tableDatas[i].Address = addr;
                                PCM.tableDatas[i].OS = osNew;
                                Debug.WriteLine(PCM.tableDatas[i].TableName);
                                //PCM.tableDatas[i].AddrInt = Convert.ToUInt32(addr, 16);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Debug.WriteLine(name + ": not found");
                            for (int i = 0; i < PCM.tableDatas.Count; i++)
                            {
                                if (cat.ToLower() == "protected" && PCM.tableDatas[i].TableName.ToLower() == name.ToLower())
                                {
                                    PCM.tableDatas[i].Address = addr;
                                    PCM.tableDatas[i].OS = osNew;
                                    //PCM.tableDatas[i].AddrInt = Convert.ToUInt32(addr, 16);
                                    found = true;
                                    Debug.WriteLine(name + ": PROTECTED");
                                    break;
                                }
                            }
                        }
                    }
                }
                /*            for (int i = PCM.tableDatas.Count -1; i >= 0; i--)
                            {
                                if (PCM.tableDatas[i].addrInt == uint.MaxValue)
                                    PCM.tableDatas.RemoveAt(i);
                            }*/
                //Fix table names:
                for (int i = 0; i < PCM.tableDatas.Count; i++)
                {
                    PCM.tableDatas[i].OS = osNew;
                    if (PCM.tableDatas[i].TableName.ToLower().StartsWith("ka_") || PCM.tableDatas[i].TableName.ToLower().StartsWith("ke_") || PCM.tableDatas[i].TableName.ToLower().StartsWith("kv_"))
                        PCM.tableDatas[i].TableName = PCM.tableDatas[i].TableName.Substring(3);
                }
                Logger(" [OK]");
                RefreshTablelist(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void txFilter_TextChanged(object sender, EventArgs e)
        {
            //filterTables();
            keyDelayCounter = 0;
            timerFilter.Enabled = true;
        }

        private void showTablesWithEmptyAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showTablesWithEmptyAddressToolStripMenuItem.Checked)
                showTablesWithEmptyAddressToolStripMenuItem.Checked = false;
            else
                showTablesWithEmptyAddressToolStripMenuItem.Checked = true;
            //FilterTables(true);
            RefreshTablelist(true);
        }

        private void ShowFileInfo(PcmFile pcm, RichTextBox txtBox)
        {
            txtBox.SelectionFont = new Font(txtBox.Font, FontStyle.Bold);
            txtBox.AppendText(pcm.FileName + Environment.NewLine);
            txtBox.SelectionFont = new Font(txtBox.Font, FontStyle.Regular);
            for (int i = 0; i < pcm.Segments.Count; i++)
            {
                SegmentConfig S = pcm.Segments[i];
                if (S.Hidden)
                    continue;
                txtBox.AppendText(" " + pcm.segmentinfos[i].Name.PadRight(11));
                if (pcm.segmentinfos[i].PN.Length > 1)
                {
                    if (pcm.segmentinfos[i].Stock == "[stock]")
                        txtBox.AppendText(" PN: " + pcm.segmentinfos[i].PN.PadLeft(9));
                    else
                        txtBox.AppendText(" PN: " + pcm.segmentinfos[i].PN.PadLeft(9));
                }
                if (pcm.segmentinfos[i].Ver.Length > 1)
                    txtBox.AppendText(", Ver: " + pcm.segmentinfos[i].Ver.PadLeft(4));
                else
                    txtBox.AppendText(" ".PadLeft(11));
                if (pcm.segmentinfos[i].SegNr.Length > 0)
                    txtBox.AppendText(", Nr: " + pcm.segmentinfos[i].SegNr.PadLeft(3));
                else
                    txtBox.AppendText(" ".PadLeft(9));
                txtBox.AppendText("[" + pcm.segmentinfos[i].Address + "]");

                txtBox.AppendText(", Size: " + pcm.segmentinfos[i].Size.ToString());
                if (pcm.segmentinfos[i].ExtraInfo != null && pcm.segmentinfos[i].ExtraInfo.Length > 0)
                    txtBox.AppendText(Environment.NewLine + pcm.segmentinfos[i].ExtraInfo);
                txtBox.AppendText(Environment.NewLine);
            }
        }

        private void ImportXperimentalCsv2()
        {
            try
            {
                string FileName = SelectFile("Select CSV File", CsvFilter);
                if (FileName.Length == 0)
                    return;

                Logger("Loading file: " + FileName, false);
                StreamReader sr = new StreamReader(FileName);
                string csvLine;
                while ((csvLine = sr.ReadLine()) != null)
                {
                    string[] cParts = csvLine.Split(';');
                    if (cParts.Length > 2)
                    {
                        string cat = cParts[0];
                        string name = cParts[1];
                        string addr = cParts[2];
                        if (name.ToLower().StartsWith("ka_") || name.ToLower().StartsWith("ke_") || name.ToLower().StartsWith("kv_"))
                            name = name.Substring(3);

                        uint lastmask = uint.MaxValue;
                        uint addrInt = Convert.ToUInt32(addr, 16);
                        for (int i = 0; i < PCM.tableDatas.Count; i++)
                        {
                            if (PCM.tableDatas[i].Category == cat && PCM.tableDatas[i].TableName.ToLower().StartsWith(name.ToLower()))
                            {
                                if (name == "K_DYNA_AIR_COEFFICIENT")
                                {
                                    //PCM.tableDatas[i].Address = addrInt.ToString("X8");
                                    PCM.tableDatas[i].addrInt = addrInt;
                                    Debug.WriteLine(PCM.tableDatas[i].TableName + ": " + addrInt.ToString("X"));
                                    addrInt += 2;
                                }
                                else
                                {
                                    uint mask = Convert.ToUInt32(PCM.tableDatas[i].BitMask, 16);
                                    if (lastmask == uint.MaxValue)
                                        lastmask = mask;
                                    if (mask > lastmask)
                                    {
                                        addrInt++;
                                    }
                                    lastmask = mask;
                                    //PCM.tableDatas[i].Address = addrInt.ToString("X8");
                                    PCM.tableDatas[i].addrInt = addrInt;
                                    Debug.WriteLine(PCM.tableDatas[i].TableName + ": " + addrInt.ToString("X") + " mask: " + mask.ToString("X"));
                                }
                            }
                        }
                    }
                }
                Logger(" [OK]");
                RefreshTablelist(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!e.Exception.Message.Contains("DataGridViewComboBoxCell"))
                Debug.WriteLine(e.Exception);
        }

        private void comboFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("comboFilterBy_SelectedIndexChanged");
            if (!string.IsNullOrEmpty(txtFilter.Text))
                RefreshTablelist(true);
            //FilterTables(true);
        }

        private void disableMultitableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disableMultitableToolStripMenuItem.Checked = !disableMultitableToolStripMenuItem.Checked;
        }

        private void saveBinAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveBin();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = GetSortOrder(sortIndex);
                SortTables();
            }
            else if (AppSettings.WorkingMode != 2)
            {
                contextMenuStrip2.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private SortOrder GetSortOrder(int columnIndex)
        {
            try
            {
                if (dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                    || dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    return SortOrder.Ascending;
                }
                else
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    return SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
            return SortOrder.Ascending;
        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            Debug.WriteLine("Databindingcomplete");
            UseComboBoxForEnums(dataGridView1);
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.DataSourceNullValue = string.Empty;
            }
        }

        private void unitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML fe = new frmEditXML();
            fe.Show();
            fe.LoadUnits();
        }

        private void fixTableNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PCM.tableDatas.Count; i++)
            {
                if (PCM.tableDatas[i].TableName.ToLower().StartsWith("ka_") || PCM.tableDatas[i].TableName.ToLower().StartsWith("ke_") || PCM.tableDatas[i].TableName.ToLower().StartsWith("kv_"))
                    PCM.tableDatas[i].TableName = PCM.tableDatas[i].TableName.Substring(3);
            }
            RefreshTablelist(true);
        }

        private void DataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                TableData rTd = (TableData)dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                AddToRedoLog(rTd, PCM.tableDatas,"TableData",rTd.TableName, "", ReDo.RedoAction.Add, "", "", dataGridView1.CurrentCell.RowIndex);
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var oldValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            var newValue = e.FormattedValue;
            TableData rTd = (TableData)dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
            string prop = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            AddToRedoLog(rTd, PCM.tableDatas, "TableData", rTd.TableName, prop, ReDo.RedoAction.Edit, oldValue, newValue);
        }


        private void DataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if (AppSettings.WorkingMode != 2)
                {
                    e.Cancel = true;
                    return;
                }
                for (int r = 0; r < dataGridView1.SelectedRows.Count; r++)
                {
                    TableData rTd = (TableData)dataGridView1.SelectedRows[r].DataBoundItem;
                    AddToRedoLog(rTd, PCM.tableDatas, "TableData", rTd.TableName, "", ReDo.RedoAction.Delete, "", "", r);
                    PCM.tableDatas.Remove(rTd);
                    //FilterTables();
                }
                dataGridView1.Update();
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        private void PeekTableValuesWithCompare(TableData shTd)
        {
            PeekTableValues(shTd, PCM,true, Color.Blue); //Show values from current file 
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
            {
                PcmFile peekPCM = (PcmFile)mi.Tag;
                if (peekPCM.FileName != PCM.FileName)
                {
                    TableData compTd = FindTableData(shTd, peekPCM.tableDatas);
                    if (compTd != null)
                    {
                        txtDescription.AppendText(peekPCM.FileName + ": [" + shTd.TableName + "]" + Environment.NewLine);
                        PeekTableValues(compTd, peekPCM,true, Color.Blue);
                    }
                }
            }
            if (ftvd != null && ftvd.Visible)
            {
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile peekPCM = (PcmFile)mi.Tag;
                    if (peekPCM.FileName != PCM.FileName)
                    {
                        TableData compTd = FindTableData(shTd, peekPCM.tableDatas);
                        if (compTd != null)
                        {
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                ftvd.vis1.ChangeTd(PCM, shTd);
                                ftvd.vis2.ChangeTd(peekPCM, compTd);
                                ftvd.ShowTables(0);
                            });
                            break;
                        }
                    }
                }
            }

        }

        private void PeekTableValues(TableData shTd, PcmFile peekPCM, bool ShowMinMax, Color color)
        {
            try
            {
                if (shTd.addrInt >= peekPCM.fsize)
                {
                    Debug.WriteLine("No address defined");
                    return;
                }
                if (shTd.OutputType == OutDataType.Bitmap)
                {
                    Debug.WriteLine("Bitmap peek not implemented");
                    return;
                }
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
                txtDescription.SelectionColor = color;
                string minMax = " [";
                if (shTd.Min > double.MinValue)
                    minMax += " Min: " + shTd.Min.ToString();
                if (shTd.Max < double.MaxValue)
                    minMax += " Max: " + shTd.Max.ToString();
                if (minMax == " [")
                    minMax = "";
                else
                    minMax += "] ";
                if (!ShowMinMax)
                {
                    minMax = "";
                }
                if (shTd.Dimensions() == 1)
                {
                    double curVal = GetValue(peekPCM.buf, (uint)(shTd.addrInt + shTd.Offset + shTd.extraoffset), shTd, 0, peekPCM);
                    UInt64 rawVal = (UInt64)GetRawValue(peekPCM.buf, shTd.StartAddress(), shTd, 0, peekPCM.platformConfig.MSB);
                    string valTxt = curVal.ToString();
                    string unitTxt = " " + shTd.Units;
                    string maskTxt = "";
                    TableValueType vt = shTd.ValueType();
                    if (vt == TableValueType.bitmask)
                    {
                        unitTxt = "";
                        UInt64 maskVal = Convert.ToUInt64(shTd.BitMask.Replace("0x", ""), 16);
                        if ((rawVal & maskVal) == maskVal)
                            valTxt = "Set";
                        else
                            valTxt = "Unset";
                        string maskBits = Convert.ToString((Int64)maskVal, 2);
                        int bit = -1;
                        for (int i = 0; 1 <= maskBits.Length; i++)
                        {
                            if (((maskVal & (UInt64)(1 << i)) != 0))
                            {
                                bit = i + 1;
                                break;
                            }
                        }
                        if (bit > -1)
                        {
                            string rawBinVal = Convert.ToString((Int64)rawVal, 2);
                            rawBinVal = rawBinVal.PadLeft(GetBits(shTd.DataType), '0');
                            maskTxt = " [" + rawBinVal + "], bit $" + bit.ToString();
                        }
                    }
                    else if (vt == TableValueType.boolean)
                    {
                        unitTxt = ", Unset/Set";
                        if (curVal > 0)
                            valTxt = "Set, " + valTxt;
                        else
                            valTxt = "Unset, " + valTxt;
                    }
                    else if (vt == TableValueType.selection)
                    {
                        Dictionary<double, string> possibleVals = ParseEnumHeaders(shTd.Values);
                        if (possibleVals.ContainsKey(curVal))
                            unitTxt = " (" + possibleVals[curVal] + ")";
                        else
                            unitTxt = " (Out of range)";
                    }
                    string formatStr = "X" + (GetElementSize(shTd.DataType) * 2).ToString();
                    string rawTxt = "";
                    switch (shTd.DataType)
                    {
                        case InDataType.FLOAT32:
                            rawTxt = ((Int32)rawVal).ToString(formatStr);
                            break;
                        case InDataType.FLOAT64:
                            rawTxt = ((Int64)rawVal).ToString(formatStr);
                            break;
                        case InDataType.INT64:
                            rawTxt = ((Int64)rawVal).ToString(formatStr);
                            break;
                        case InDataType.INT32:
                            rawTxt = ((Int32)rawVal).ToString(formatStr);
                            break;
                        case InDataType.UINT64:
                            rawTxt = ((UInt64)rawVal).ToString(formatStr);
                            break;
                        case InDataType.UINT32:
                            rawTxt = ((UInt32)rawVal).ToString(formatStr);
                            break;
                        case InDataType.SWORD:
                            rawTxt = ((Int16)rawVal).ToString(formatStr);
                            break;
                        case InDataType.UWORD:
                            rawTxt = ((UInt16)rawVal).ToString(formatStr);
                            break;
                        case InDataType.SBYTE:
                            rawTxt = ((sbyte)rawVal).ToString(formatStr);
                            break;
                        case InDataType.UBYTE:
                            rawTxt = ((byte)rawVal).ToString(formatStr);
                            break;
                        default:
                            rawTxt = ((Int32)rawVal).ToString(formatStr);
                            break;
                    }

                    txtDescription.AppendText("Current value: " + valTxt + unitTxt + " [" + rawTxt + "]" + minMax + maskTxt);
                    txtDescription.AppendText(Environment.NewLine);
                }
                else //Not 1D
                {
                    //string tblData = "Current values: " + minMax + Environment.NewLine;
                    StringBuilder tblData = new StringBuilder("Current values: " + minMax + Environment.NewLine);
                    uint addr = shTd.StartAddress();
                    double firstVal = GetValue(peekPCM.buf, addr, shTd, 0, peekPCM);
                    uint elemSize = (uint)shTd.ElementSize();
                    if (shTd.RowMajor)
                    {
                        for (int r = 0; r < shTd.Rows; r++)
                        {
                            for (int c = 0; c < shTd.Columns; c++)
                            {
                                double curVal = GetValue(peekPCM.buf, addr, shTd, 0, peekPCM);
                                addr += elemSize;
                                tblData.Append("[" + curVal.ToString("#0.0") + "]");
                                if (!ShowMinMax)
                                {
                                    if (curVal > shTd.Max || curVal < shTd.Min)
                                    {
                                        Debug.WriteLine("Value out of range: " + curVal.ToString());
                                        return;
                                    }
                                }
                            }
                            tblData.Append(Environment.NewLine);
                        }
                    }
                    else
                    {
                        List<string> tblRows = new List<string>();
                        for (int r = 0; r < shTd.Rows; r++)
                            tblRows.Add("");
                        for (int c = 0; c < shTd.Columns; c++)
                        {
                            for (int r = 0; r < shTd.Rows; r++)
                            {
                                double curVal = GetValue(peekPCM.buf, addr, shTd, 0, peekPCM);
                                addr += elemSize;
                                tblRows[r] += "[" + curVal.ToString("#0.0") + "]";
                                if (!ShowMinMax)
                                {
                                    if (curVal > shTd.Max || curVal < shTd.Min)
                                    {
                                        Debug.WriteLine("Value out of range: " + curVal.ToString());
                                        return;
                                    }
                                }
                            }
                        }
                        for (int r = 0; r < shTd.Rows; r++)
                            tblData.Append(tblRows[r] + Environment.NewLine);
                    }
                    txtDescription.AppendText(tblData.ToString());
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private int GetSelectionCount()
        {
            List<int> selectedRows = new List<int>();
            foreach (DataGridViewCell dgc in dataGridView1.SelectedCells)
            {
                if (!selectedRows.Contains(dgc.RowIndex))
                {
                    selectedRows.Add(dgc.RowIndex);
                }
            }
            //string message = "Selected " + selectedRows.Count.ToString() + " tables";
            return selectedRows.Count;
        }
        public void ShowTableDescription(PcmFile PCM, TableData shTd)
        {
            try
            {
                if (PCM.buf == null || PCM.buf.Length == 0)
                    return;
                txtDescription.Text = "";
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Bold);
                txtDescription.AppendText(shTd.TableName);
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
                if (DisplayMode == DispMode.List)
                {
                    txtDescription.AppendText(" (" + GetSelectionCount().ToString() + " tables selected)");
                }
                txtDescription.AppendText(Environment.NewLine);
                if (shTd.TableDescription != null)
                    txtDescription.AppendText(shTd.TableDescription + Environment.NewLine);
                if (shTd.ExtraDescription != null)
                    txtDescription.AppendText(shTd.ExtraDescription + Environment.NewLine);

                PeekTableValuesWithCompare(shTd);
                labelTableName.Text = shTd.TableName;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine("Datagridviev selection changed");
                TreeViewMS tv = treeView1;
                if (DisplayMode == DispMode.Tree)
                {
                    tv = treeMulti;
                }
                if (tv.SelectedNode != null)
                {
                    if (tv.SelectedNode.Name == "Patches")
                        return;
                    if (tv.SelectedNode.Parent != null && tv.SelectedNode.Parent.Name == "Patches")
                        return;
                }
                TableData selTd = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                ShowTableDescription(PCM, selTd);
                numExtraOffset.Value = selTd.extraoffset;
                ExtraOffsetFirstTry = true;
                if (histogramTableSelectionEnabled)
                {
                    hstForm.SetupTable(PCM, selTd);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }
        private void ExportXMLgeneratorCSV()
        {
            try
            {
                string fName = SelectSaveFile(CsvFilter);
                if (fName.Length == 0) return;

                Logger("Generating CSV...");

                string csvData = "Category;Tablename;Size;" + PCM.tableDatas[0].OS + ";" + Environment.NewLine;
                for (int row = 0; row < PCM.tableDatas.Count; row++)
                {
                    int tbSize = PCM.tableDatas[row].Rows * PCM.tableDatas[row].Columns * GetElementSize(PCM.tableDatas[row].DataType);
                    csvData += PCM.tableDatas[row].Category + ";";
                    csvData += PCM.tableDatas[row].TableName + ";";
                    csvData += tbSize.ToString() + ";";
                    //csvData += PCM.tableDatas[row].OS + ";";
                    csvData += PCM.tableDatas[row].Address;
                    csvData += Environment.NewLine;
                }
                Logger("Writing to file: " + fName, false);
                WriteTextFile(fName, csvData);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        private void exportXMLgeneratorCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        struct OsAddrStruct
        {
            public string tableName;
            public string category;
            public string OS;
            public string addr;
        }

        private void ImportXMLGeneratorCsv()
        {
            try
            {
                List<OsAddrStruct> osAddrList = new List<OsAddrStruct>();
                List<string> osList = new List<string>();

                LoggerBold("Supply CSV file in format: Category;Tablename;Size;OS1Address1;OS2Address2;...");
                LoggerBold("OS versions in header, for example: Category;Tablename;Size;12587603;12582405;12587656;");
                string fName = SelectFile("Select CSV file for XML generator", CsvFilter);
                if (fName.Length == 0) return;
                Logger("Using file: " + PCM.tunerFile + " as template");
                Logger("Reading file: " + fName, false);

                StreamReader sr = new StreamReader(fName);
                string csvLine;
                if ((csvLine = sr.ReadLine()) == null)
                    throw new Exception("Empty file");

                string[] hdrParts = csvLine.Split(';');
                if (hdrParts.Length < 4)
                    throw new Exception("Header too sort");

                for (int h = 4; h < hdrParts.Length; h++)
                    if (hdrParts[h].Length > 2)
                        osList.Add(hdrParts[h]);

                while ((csvLine = sr.ReadLine()) != null)
                {
                    string[] lParts = csvLine.Split(';');
                    if (lParts.Length > 4)
                    {
                        for (int x = 4; x < lParts.Length; x++)
                        {
                            OsAddrStruct oa;
                            oa.tableName = lParts[1];
                            oa.category = lParts[0];
                            oa.OS = osList[x - 4];
                            oa.addr = lParts[x];
                            osAddrList.Add(oa);
                        }
                    }
                }
                Logger(" [OK]");
                for (int o = 0; o < osList.Count; o++)
                {
                    List<TableData> newTds = new List<TableData>();
                    for (int x = 0; x < osAddrList.Count; x++)
                    {
                        if (osAddrList[x].OS == osList[o])
                        {
                            for (int t = 0; t < PCM.tableDatas.Count; t++)
                            {
                                if (PCM.tableDatas[t].TableName == osAddrList[x].tableName && PCM.tableDatas[t].Category == osAddrList[x].category)
                                {
                                    TableData newTd = PCM.tableDatas[t].ShallowCopy(true);
                                    newTd.OS = osList[o];
                                    newTd.Address = osAddrList[x].addr;
                                    newTds.Add(newTd);
                                    Debug.WriteLine(PCM.tableDatas[t].TableName + ", addr:" + osAddrList[x].addr);
                                }
                            }
                        }
                    }
                    fName = Path.Combine(Application.StartupPath, "Tuner", osList[o] + "-export.xml");
                    Logger("Saving file " + fName + "...", false);
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                        writer.Serialize(stream, newTds);
                        stream.Close();
                    }
                    Logger(" [OK]");

                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void saveXMLAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
            //string defName = PCM.OS + ".xml";
            string fName = SelectSaveFile(XmlFilter, defName);
            if (string.IsNullOrEmpty(fName))
                return;
            PCM.SaveTableList(fName);
        }

        private struct DisplayOrder
        {
            public int index;
            public string columnName;
        }
        private void DataGridView1_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //Debug.WriteLine("Displayindex: " + e.Column.HeaderText);
            columnsModified = true;
        }
        private void DataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            columnsModified = true;
            //saveGridLayout();
            Debug.WriteLine("Columnwidth: " + e.Column.HeaderText);
        }

        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var info = dataGridView1.HitTest(e.X, e.Y);
                if (e.Button == MouseButtons.Left && info.RowIndex == -1)
                {
                    this.dataGridView1.ColumnDisplayIndexChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView1_ColumnDisplayIndexChanged);
                    this.dataGridView1.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView1_ColumnWidthChanged);
                    Debug.WriteLine("Enabling event");
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                //Debug.WriteLine("Disabling event");
                this.dataGridView1.ColumnDisplayIndexChanged -= new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView1_ColumnDisplayIndexChanged);
                this.dataGridView1.ColumnWidthChanged -= new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView1_ColumnWidthChanged);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void SaveGridLayout()
        {
            try
            {
                if (!columnsModified) return;

                if (dataGridView1.Columns.Count < 2) return;
                StringBuilder columnWidth = new StringBuilder();
                int maxDispIndex = 0;
                List<DisplayOrder> displayOrder = new List<DisplayOrder>();
                for (int c = 0; c < dataGridView1.Columns.Count; c++)
                {
                    columnWidth.Append(dataGridView1.Columns[c].Width.ToString() + ",");
                    if (dataGridView1.Columns[c].Visible)
                    {
                        DisplayOrder dispO = new DisplayOrder();
                        dispO.columnName = dataGridView1.Columns[c].Name;
                        dispO.index = dataGridView1.Columns[c].DisplayIndex;
                        displayOrder.Add(dispO);
                        if (dispO.index > maxDispIndex)
                            maxDispIndex = dispO.index;
                    }
                }


                StringBuilder order = new StringBuilder();
                for (int i = 0; i <= maxDispIndex; i++)
                {
                    for (int j = 0; j < displayOrder.Count; j++)
                    {
                        if (displayOrder[j].index == i)
                        {
                            order.Append(displayOrder[j].columnName + ",");
                            break;
                        }
                    }
                }
                Debug.WriteLine("Display order: " + order.ToString());
                Debug.WriteLine("Column width: " + columnWidth);
                if (AppSettings.WorkingMode == 2)
                {
                    //Advanced mode
                    AppSettings.ConfigModeColumnOrder = order.ToString().Trim(',');
                    AppSettings.ConfigModeColumnWidth = columnWidth.ToString().Trim(',');
                }
                else
                {
                    //Tuner mode
                    AppSettings.TunerModeColumns = order.ToString().Trim(',');
                    AppSettings.TunerModeColumnWidth = columnWidth.ToString().Trim(',');
                }
                AppSettings.Save();
                columnsModified = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        void EnableTunerModeColumns()
        {
            string[] tunerModCols = AppSettings.TunerModeColumns.Split(',');

            //Mark checked all visible columns
            foreach (ToolStripMenuItem menuItem in contextMenuStrip2.Items)
            {
                menuItem.Checked = false;
                for (int i = 0; i < tunerModCols.Length; i++)
                {
                    if (menuItem.Name == tunerModCols[i])
                    {
                        menuItem.Checked = true;
                        break;
                    }
                }
                dataGridView1.Columns[menuItem.Name].Visible = menuItem.Checked;
            }
        }

        void Cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            EnableTunerModeColumns();
        }

        private void RemoveTunerModColumn(string col)
        {
            string[] tunerModCols = AppSettings.TunerModeColumns.Split(',');
            StringBuilder sb = new StringBuilder();
            foreach (ToolStripMenuItem menuItem in contextMenuStrip2.Items)
            {
                if (tunerModCols.Contains(menuItem.Name) && menuItem.Name != col)
                {
                    sb.Append(menuItem.Name + ",");
                    menuItem.Checked = true;
                }
                else
                {
                    menuItem.Checked = false;
                }
            }
            AppSettings.TunerModeColumns = sb.ToString().Trim(',');
            AppSettings.Save();
        }

        void columnSelection_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                menuItem.Checked = !menuItem.Checked;
                Debug.WriteLine(menuItem.Name);
                //dataGridView1.Columns[menuItem.Name].Visible = menuItem.Checked;
                if (AppSettings.WorkingMode != 2)
                {
                    RemoveTunerModColumn(menuItem.Name);
                    columnsModified = true;
                    dataGridView1.Columns[menuItem.Name].Visible = !dataGridView1.Columns[menuItem.Name].Visible;
                    dataGridView1.Update();
                    dataGridView1.Refresh();
                    //FilterTables();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner, line " + line + ": " + ex.Message);
            }
        }

        private void resetTunerModeColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.TunerModeColumns = "id,TableName,Category,Units,Columns,Rows,TableDescription";
            AppSettings.TunerModeColumnWidth = "35,100,237,135,100,100,100,100,100,114,100,100,100,100,60,46,100,100,100,100,100,493,100,";
            AppSettings.Save();
            dataGridView1.Update();
            dataGridView1.Refresh();
            //FilterTables();
        }

        private void OpenNewBinFile(bool asCompare, List<string> fileList = null)
        {
            try
            {
                //string newFile = SelectFile();
                if (fileList == null)
                    fileList = SelectMultipleFiles();
                foreach (string newFile in fileList)
                {
                    if (newFile.Length == 0) return;
                    PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
                    AddtoCurrentFileMenu(newPCM, false);
                    LoadConfigforPCM(ref newPCM);
                    if (!asCompare)
                    {
                        PCM = newPCM;
                        SelectPCM();
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
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        private void loadBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            OpenNewBinFile(false);
            RefreshTablelist(false);
            timer.Stop();
            Debug.WriteLine("Open new file time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
            if (tabControl1.SelectedTab == tabCategory && treeCategory != null && treeCategory.Nodes != null && treeCategory.Nodes.Count > 0)
            {
                tabCategory.Focus();
                treeCategory.Nodes[0].Expand();
            }
        }

        private string GetFileLetter(string menuTxt)
        {
            string retVal = "";

            int pos = menuTxt.IndexOf(':');
            if (pos > -1)
                retVal = menuTxt.Substring(0, pos);
            return retVal;
        }

        public void AddtoCurrentFileMenu(PcmFile newPCM, bool setdefault = true)
        {
            try
            {
                if (setdefault)
                    foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                        mi.Checked = false;

                int lastFile = 0;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                    lastFile++;
                string fLetter = Base26Encode(lastFile);

                ToolStripMenuItem menuitem = new ToolStripMenuItem(newPCM.FileName);
                menuitem.Text = fLetter + ": " + newPCM.FileName;
                menuitem.Name = newPCM.FileName;
                menuitem.Tag = newPCM;
                if (setdefault)
                    menuitem.Checked = true;
                currentFileToolStripMenuItem.DropDownItems.Add(menuitem);
                menuitem.Click += Menuitem_Click;

                ToolStripMenuItem cmpMenuitem = new ToolStripMenuItem(menuitem.Name);
                cmpMenuitem.Name = menuitem.Name;
                cmpMenuitem.Tag = newPCM;
                findDifferencesToolStripMenuItem.DropDownItems.Add(cmpMenuitem);
                cmpMenuitem.Click += compareMenuitem_Click;

                ToolStripMenuItem cmpHexMenuitem = new ToolStripMenuItem(menuitem.Name);
                cmpHexMenuitem.Name = menuitem.Name;
                cmpHexMenuitem.Tag = newPCM;
                findDifferencesHEXToolStripMenuItem.DropDownItems.Add(cmpHexMenuitem);
                cmpHexMenuitem.Click += CmpHexMenuitem_Click;


                ToolStripMenuItem tdMenuItem = new ToolStripMenuItem(newPCM.FileName);
                tdMenuItem.Name = newPCM.FileName;
                tdMenuItem.Tag = newPCM.currentTableDatasList;
                tableListToolStripMenuItem.DropDownItems.Add(tdMenuItem);
                tdMenuItem.Click += tablelistSelect_Click;

                ToolStripMenuItem offsetMenuItem = new ToolStripMenuItem(newPCM.FileName);
                offsetMenuItem.Name = newPCM.FileName;
                offsetMenuItem.Tag = newPCM;
                addSegmentOffsetNodeToolStripMenuItem.DropDownItems.Add(offsetMenuItem);
                offsetMenuItem.Click += Fmi_Click;

                UpdateFileInfoTab();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void CmpHexMenuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            PcmFile cmpWithPcm = (PcmFile)menuitem.Tag;
            FindTableDifferencesHEX(cmpWithPcm);
        }

        private void Menuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            bool isChecked = menuitem.Checked;
            //            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
            //                mi.Checked = false;
            menuitem.Checked = !isChecked;
            PCM = (PcmFile)menuitem.Tag;
            SelectPCM();
        }

        private void compareWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void compareMenuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            PcmFile cmpWithPcm = (PcmFile)menuitem.Tag;
            FindTableDifferences(cmpWithPcm);
        }

        int diffMissingTables;
        private TableData CompareTableHEX(TableData td1, PcmFile pcm1, PcmFile pcm2)
        {
            try
            {
                TableData td2 = td1.ShallowCopy(false);
                int tbSize = td1.Rows * td1.Columns * GetElementSize(td1.DataType);
                TableData cmpTd = td1;
                if (pcm1.OS != pcm2.OS && !td1.CompatibleOS.Contains("," + pcm2.OS + ","))
                {
                    //Not 100% compatible file, find table by name & category
                    cmpTd = FindTableData(td1, pcm2.tableDatas);
                    if (cmpTd == null)
                    {
                        //Logger("Table not found: " + td1.TableName + "[" + pcm2.FileName + "]");
                        diffMissingTables++;
                        return null;    //Don't add to list if not in both files
                    }
                    td2 = cmpTd;
                    int tb2size = td2.Rows * td2.Columns * GetElementSize(td2.DataType);
                    if (tbSize != tb2size)
                        return null;
                }
                if ((td1.addrInt + tbSize) > pcm1.fsize || (td2.addrInt + tbSize) > pcm2.fsize)
                {
                    LoggerBold("Table address out of range: " + td1.TableName);
                    return null;
                }

                if (td1.BitMask != null && td1.BitMask.Length > 0)
                {
                    //Check only bit
                    double orgVal = GetValue(pcm1.buf, td1.addrInt, td1, (int)(td1.Offset + td1.extraoffset), pcm1);
                    double compVal = GetValue(pcm2.buf, td2.addrInt, td2, (int)(td2.Offset + td2.extraoffset), pcm2);
                    if (orgVal == compVal)
                        return null;
                    else
                        return cmpTd;
                }
                byte[] buff1 = new byte[tbSize];
                byte[] buff2 = new byte[tbSize];
                Array.Copy(pcm1.buf, td1.StartAddress(), buff1, 0, tbSize);
                Array.Copy(pcm2.buf, td2.StartAddress(), buff2, 0, tbSize);
                if (buff1.SequenceEqual(buff2))
                    return null;
                else
                    return cmpTd;   //Found table with different data
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner compareTableHEX, line " + line + ": " + ex.Message);
            }
            return null;
        }

        private void FindTableDifferences(PcmFile cmpWithPcm)
        {
            try
            {
                Logger("Finding tables with different data");
                cmpWithPcm.SelectTableDatas(0, "");
                List<TableData> diffTableDatas = new List<TableData>();
                List<TableData> cmpTableDatas = new List<TableData>();
                for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
                {
                    if (PCM.tableDatas[t1].addrInt < PCM.fsize)
                    {
                        TableData cmpId;
                        if (PCM.OS == cmpWithPcm.OS)
                            cmpId = PCM.tableDatas[t1];
                        else
                            cmpId = FindTableData(PCM.tableDatas[t1], cmpWithPcm.tableDatas);
                        if (cmpId != null)
                        {
                            if (!CompareTables(PCM.tableDatas[t1], cmpId, PCM, cmpWithPcm))
                            {
                                diffTableDatas.Add(PCM.tableDatas[t1]);
                                cmpTableDatas.Add(cmpId);
                            }
                        }
                    }
                }
                Logger(" [OK]");
                frmHexDiff fhd = new frmHexDiff(PCM, cmpWithPcm, diffTableDatas, cmpTableDatas, this);
                fhd.Show();
                fhd.FindDifferences(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }


        private void FindTableDifferencesHEX(PcmFile cmpWithPcm)
        {
            try
            {
                Logger("Finding tables with different data");
                if (PCM.configFile != cmpWithPcm.configFile)
                    LoggerBold("WARING! OS mismatch!");
                else
                {
                    //Check undefined areas, too
                    int udCount = 0;
                    byte[] buf = new byte[PCM.buf.Length];
                    for (int b = 0; b < buf.Length; b++)
                        buf[b] = 0;
                    for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
                    {
                        TableData tData = PCM.tableDatas[t1];
                        uint start = tData.addrInt;
                        uint end = (uint)(start + GetElementSize(tData.DataType) * tData.Rows * tData.Columns + tData.Offset + tData.extraoffset);
                        for (uint b = start; b < end; b++)
                            buf[b] = 1;
                    }
                    for (int s = 0; s < PCM.Segments.Count; s++)
                    {
                        if (PCM.Segments[s].Missing)
                            continue;
                        for (int sb = 0; sb < PCM.segmentAddressDatas[s].SegmentBlocks.Count; sb++)
                        {
                            uint b = PCM.segmentAddressDatas[s].SegmentBlocks[sb].Start;
                            for (; b < PCM.segmentAddressDatas[s].SegmentBlocks[sb].End; b++)
                            {
                                if (buf[b] == 0)    //undefined area
                                {
                                    TableData undefTd = new TableData();
                                    undefTd.addrInt = b;
                                    undefTd.Columns = 1;
                                    undefTd.DataType = InDataType.UBYTE;
                                    undefTd.OS = PCM.OS;
                                    undefTd.OutputType = OutDataType.Hex;
                                    undefTd.TableName = "Undefined " + udCount.ToString();
                                    for (; b < PCM.segmentAddressDatas[s].SegmentBlocks[sb].End; b++)
                                    {
                                        if (buf[b] == 1)
                                        {
                                            if (b - undefTd.addrInt > 1)
                                            {
                                                undefTd.Rows = (ushort)(b - undefTd.addrInt - 1);
                                                PCM.tableDatas.Add(undefTd);
                                                cmpWithPcm.tableDatas.Add(undefTd);
                                                udCount++;
                                            }
                                            break;
                                        }
                                    }
                                    if (b == PCM.segmentAddressDatas[s].SegmentBlocks[sb].End && undefTd.Rows > 0)
                                    {
                                        PCM.tableDatas.Add(undefTd);
                                        cmpWithPcm.tableDatas.Add(undefTd);
                                        udCount++;
                                    }
                                }
                            }
                        }
                    }

                }
                cmpWithPcm.SelectTableDatas(0, "");
                List<TableData> diffTableDatas = new List<TableData>();
                List<TableData> cmpTableDatas = new List<TableData>();
                for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
                {
                    if (PCM.tableDatas[t1].addrInt < PCM.fsize)
                    {
                        TableData cmpTd = CompareTableHEX(PCM.tableDatas[t1], PCM, cmpWithPcm);
                        if (cmpTd != null)
                        {
                            diffTableDatas.Add(PCM.tableDatas[t1]);
                            cmpTableDatas.Add(cmpTd);
                        }
                    }
                }
                Logger(" [OK]");
                if (diffTableDatas.Count == 0)
                {
                    Logger("All tables are identical");
                    return;
                }
                frmHexDiff fhd = new frmHexDiff(PCM, cmpWithPcm, diffTableDatas, cmpTableDatas, this);
                fhd.Show();
                fhd.FindDifferences(true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void tablelistSelect_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                //Reset all to uncheck
                mi.Checked = false;
            }

            ToolStripMenuItem mItem = (ToolStripMenuItem)sender;
            PCM.SelectTableDatas((int)mItem.Tag, mItem.Name);
            mItem.Checked = true;
            //FilterTables(false);
            RefreshTablelist(false);
            Navigate(0);
        }


        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCSV();
        }

        private void xDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Generating xdf...");
            XDF xdf = new XDF();
            Logger(xdf.ExportXdf(PCM, PCM.tableDatas));

        }

        private void xMLGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportXMLgeneratorCSV();
        }

        private void dTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportDTC(ref PCM);
            RefreshTablelist(false);
        }

        private void tableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTableSeek(ref PCM);
            RefreshTablelist(false);
        }

        private void ImportXDF()
        {
            XDF xdf = new XDF();
            xdf.ImportXdf(PCM, PCM.tableDatas);
            Debug.WriteLine("Categories: " + PCM.tableCategories.Count);
            //LoggerBold("Note: Only basic XDF conversions are supported, check Math and SavingMath values");
            RefreshTablelist(false);
            comboTableCategory.Text = "_All";
        }
        private void xDFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportXDF();
            RefreshTablelist(false);
        }

        private void tinyTunerDBV6OnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTinyTunerDB(ref PCM);
            RefreshTablelist(false);
        }


        private void cSVexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportxperimentalCSV();
        }

        private void cSV2ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportXperimentalCsv2();
        }

        private void AddNewTableList(string ListName)
        {
            int l = 0;
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                //Reset all to uncheck
                mi.Checked = false;
                l++;
            }
            string listName;
            if (string.IsNullOrEmpty(ListName))
            {
                listName = "List" + l.ToString();
                frmData frmD = new frmData();
                frmD.Text = "Tablelist name (optional)";
                if (frmD.ShowDialog() == DialogResult.OK)
                {
                    listName = frmD.txtData.Text;
                }
            }
            else
            {
                listName = ListName;
            }
            ToolStripMenuItem mItem = new ToolStripMenuItem(listName);
            mItem.Name = listName;
            PCM.SelectTableDatas(PCM.altTableDatas.Count, mItem.Name);
            mItem.Tag = PCM.altTableDatas.Count - 1;
            mItem.Checked = true;
            tableListToolStripMenuItem.DropDownItems.Add(mItem);
            mItem.Click += tablelistSelect_Click;
            //FilterTables(true);
            RefreshTablelist(true);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTableList("");
        }

        private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData newTd = lastSelectTd.ShallowCopy(true);
                newTd.OS = PCM.OS;
                frmTdEditor fte = new frmTdEditor();
                fte.td = newTd;
                fte.LoadTd();
                int c = dataGridView1.CurrentCell.ColumnIndex;
                int r = dataGridView1.CurrentCell.RowIndex;
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas.Insert(id, fte.td);
                            AddToRedoLog(fte.td, PCM.tableDatas, "TableData", fte.td.TableName, "", ReDo.RedoAction.Add, "", "", id);
                            FilterTables(true);
                            dataGridView1.ClearSelection();
                            dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[c];
                            break;
                        }
                    }
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void editRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmTdEditor fte = new frmTdEditor();
                fte.td = (TableData)dataGridView1.CurrentCell.OwningRow.DataBoundItem;
                fte.LoadTd();
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.Update();
                    this.Refresh();
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int c = dataGridView1.CurrentCell.ColumnIndex;
                int r = dataGridView1.CurrentCell.RowIndex;
                AddToRedoLog(lastSelectTd, PCM.tableDatas, "TableData", lastSelectTd.TableName, "", ReDo.RedoAction.Delete, "", "", r);
                PCM.tableDatas.Remove(lastSelectTd);
                FilterTables(true);
                dataGridView1.ClearSelection();
                if (r < dataGridView1.Rows.Count)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[c];
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void duplicateTableConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData newTd = lastSelectTd.ShallowCopy(true);
                int c = dataGridView1.CurrentCell.ColumnIndex;
                int r = dataGridView1.CurrentCell.RowIndex;
                for (int id = 0; id < PCM.tableDatas.Count; id++)
                {
                    if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                    {
                        PCM.tableDatas.Insert(id, newTd);
                        AddToRedoLog(newTd, PCM.tableDatas, "TableData", newTd.TableName, "", ReDo.RedoAction.Add, "", "", id);
                        FilterTables(true);
                        dataGridView1.ClearSelection();
                        dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[c];
                        break;
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
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void searchAndCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmMassCompare fmc = new frmMassCompare();
                fmc.PCM = PCM;
                //int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
                fmc.td = lastSelectTd;
                fmc.Text = "Search and Compare: " + fmc.td.TableName;
                fmc.Show();
                fmc.SelectCmpFiles();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }


        private void xMlgeneratorImportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportXMLGeneratorCsv();
        }

        private void xMLGeneratorExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportXMLgeneratorCSV();
        }

        private void loadTablelistxmlTableseekImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.LoadTableList();
            ImportDTC(ref PCM);
            ImportTableSeek(ref PCM);
            comboTableCategory.Text = "_All";
            RefreshTablelist(false);
        }

        private void loadTablelistnewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTableList("");
            PCM.LoadTableList();
            comboTableCategory.Text = "_All";
            RefreshTablelist(false);
        }

        private void openMultipleBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Open files";
                frmF.LoadFiles(AppSettings.LastBINfolder);
                if (frmF.ShowDialog(this) == DialogResult.OK)
                {
                    for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                    {
                        string newFile = frmF.listFiles.CheckedItems[i].Tag.ToString();
                        Logger("Opening file: " + newFile);
                        PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
                        AddtoCurrentFileMenu(newPCM);
                        PCM = newPCM;
                        LoadConfigforPCM(ref PCM);
                    }
                    SelectPCM();
                    RefreshTablelist(true);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void disableConfigAutoloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disableConfigAutoloadToolStripMenuItem.Checked = !disableConfigAutoloadToolStripMenuItem.Checked;

            AppSettings.disableTunerAutoloadSettings = disableConfigAutoloadToolStripMenuItem.Checked;
            AppSettings.Save();

        }

        private void saveAllBINFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
            {
                PcmFile pcmFile = (PcmFile)mi.Tag;
                Logger("Saving file:" + pcmFile.FileName);
                pcmFile.SaveBin(pcmFile.FileName);
            }
            Logger("OK");
        }


        private void searchAndCompareAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmMassCompare fmc = new frmMassCompare();
                fmc.PCM = PCM;
                fmc.compareAll = true;
                //int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
                fmc.td = lastSelectTd;
                fmc.Text = "Search and Compare: " + fmc.td.TableName;
                fmc.Show();
                fmc.SelectCmpFiles();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void CompareSelectedTables()
        {
            try
            {
                List<TableData> tds = GetSelectedTableTds();
                if (tds.Count != 2)
                {
                    Logger("Select 2 tables!");
                    return;
                }

                TableData td1 = tds[0];
                TableData td2 = tds[1];
                if (td1.Rows != td2.Rows || td1.Columns != td2.Columns)
                {
                    Logger("Select 2 tables with equal size!");
                    return;
                }
                Logger("Comparing....");
                frmTableEditor frmT = new frmTableEditor();
                frmT.disableMultiTable = true;
                PcmFile pcm1 = PCM.ShallowCopy();
                pcm1.FileName = td1.TableName;
                PcmFile pcm2 = PCM.ShallowCopy();
                pcm2.FileName = td2.TableName;
                List<TableData> tds2 = new List<TableData>();
                tds2.Add(td1);
                frmT.PrepareTable(pcm1, td1, tds2, "A");
                frmT.AddCompareFiletoMenu(pcm2, td2, "B: " + td2.TableName, "B");
                frmT.Show();
                frmT.radioSideBySide.Checked = true;
                frmT.LoadTable();

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        private void compareSelectedTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompareSelectedTables();
        }

        private void massModifyTableListsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string folder = Path.Combine(Application.StartupPath, "Tuner"); ;
                DirectoryInfo d = new DirectoryInfo(folder);
                FileInfo[] Files = d.GetFiles("*.xml", SearchOption.AllDirectories);
                List<string> tunerFiles = new List<string>();
                foreach (FileInfo file in Files)
                {
                    tunerFiles.Add(file.FullName);
                }
                frmMassModifyTableData fmm = new frmMassModifyTableData();
                fmm.Show();
                fmm.LoadData(tunerFiles);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void massModifyTableListsSelectFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Open files";
                frmF.filter = ".xml";
                frmF.LoadFiles(Path.Combine(Application.StartupPath, "Tuner"));
                List<string> tunerFiles = new List<string>();
                if (frmF.ShowDialog(this) == DialogResult.OK)
                {
                    for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                    {
                        string newFile = frmF.listFiles.CheckedItems[i].Tag.ToString();
                        tunerFiles.Add(newFile);
                    }
                }
                frmF.Dispose();
                frmMassModifyTableData fmm = new frmMassModifyTableData();
                fmm.Show();
                fmm.LoadData(tunerFiles);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void moreSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMoreSettings frmTS = new frmMoreSettings();
            frmTS.ShowDialog();
            //FilterTables(true);
            RefreshTablelist(true);
        }

        private void timerFilter_Tick(object sender, EventArgs e)
        {
            keyDelayCounter++;
            if (keyDelayCounter > AppSettings.keyPressWait100ms)
            {
                timerFilter.Enabled = false;
                Debug.WriteLine("Filter timer");
                keyDelayCounter = 0;
                Navigating = true;
                //FilterTables(true);
                RefreshTablelist(true);
                Navigating = false;
            }
        }

        private void caseSensitiveFilteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caseSensitiveFilteringToolStripMenuItem.Checked = !caseSensitiveFilteringToolStripMenuItem.Checked;
            FilterTables(true);
        }


        private void copySelectedTablesToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TableData> tableTds = GetSelectedTableTds();
            frmMassCopyTables fls = new frmMassCopyTables();
            fls.PCM = PCM;
            fls.tableTds = tableTds;
            fls.Show();
            fls.StartTableCopy();
        }

        private void OpenPatchSelector()
        {
            frmPatchSelector frmPS = new frmPatchSelector();
            frmPS.basefile = PCM;
            frmPS.tunerForm = this;
            frmPS.Show();
            Application.DoEvents();
            frmPS.LoadPatches();
        }

        public void ApplyPatch(string fileName)
        {
            try
            {
                Logger("Loading file: " + fileName);
                PatchList = LoadPatchFile(fileName);
                ApplyXMLPatch(ref PCM);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void applyPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPatchSelector();
        }

        private List<TableData> GetSelectedTableTds()
        {
            List<TableData> tableTds = new List<TableData>();
            try
            {
                if (DisplayMode == DispMode.Tree)
                {
                    TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                    //foreach (TreeNode tn in tv.Nodes)
                    //  findCheckdNodes(tn, ref tableIds);
                    foreach (TreeNode tn in tv.SelectedNodes)
                    {
                        Tnode tnode = (Tnode)tn.Tag;
                        if (tnode.NodeType == NType.Table)
                            tableTds.Add(tnode.Td);
                    }
                    if (tableTds.Count == 0 && lastSelectTd != null)
                        tableTds.Add(lastSelectTd);
                }
                else
                {
                    for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                    {
                        int row = dataGridView1.SelectedCells[i].RowIndex;
                        //int id = Convert.ToInt32(dataGridView1.Rows[row].Cells["id"].Value);
                        TableData xTd = (TableData)dataGridView1.Rows[row].DataBoundItem;
                        if (!tableTds.Contains(xTd))
                            tableTds.Add(xTd);
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
                LoggerBold("Error, frmTuner, line " + line + ": " + ex.Message);
            }
            return tableTds;
        }

        private void GenerateTablePatch(bool createNew)
        {
            try
            {
                List<TableData> tableTds = GetSelectedTableTds();
                if (tableTds.Count == 0)
                {
                    Logger("No tables selected");
                    return;
                }
                string Description = "";
                List<XmlPatch> newPatch;
                string patchFname;
                if (createNew)
                {
                    string defName = Path.Combine(Application.StartupPath, "Patches", "newpatch.xmlpatch");
                    patchFname = SelectSaveFile("PATCH files (*.xmlpatch)|*.xmlpatch|ALL files (*.*)|*.*", defName);
                    if (string.IsNullOrEmpty(patchFname))
                        return;
                    frmData frmD = new frmData();
                    frmD.Text = "Patch Description";
                    if (frmD.ShowDialog() == DialogResult.OK)
                        Description = frmD.txtData.Text;
                    frmD.Dispose();
                    newPatch = new List<XmlPatch>();
                }
                else
                {
                    patchFname = SelectFile("Select patch", "PATCH files (*.xmlpatch)|*.xmlpatch|ALL files (*.*)|*.*");
                    if (patchFname.Length == 0)
                        return;
                    newPatch = LoadPatchFile(patchFname);
                    if (newPatch.Count > 0)
                        Description = newPatch[0].Description;
                }
                for (int i = 0; i < tableTds.Count; i++)
                {
                    TableData pTd = tableTds[i];
                    XmlPatch xpatch = new XmlPatch();
                    xpatch.CompatibleOS = "Table:" + pTd.TableName + ",columns:" + pTd.Columns.ToString() + ",rows:" + pTd.Rows.ToString();
                    xpatch.XmlFile = PCM.configFile;
                    xpatch.Segment = PCM.GetSegmentName(pTd.addrInt);
                    xpatch.Description = Description;
                    frmTableEditor frmTE = new frmTableEditor();
                    frmTE.PrepareTable(PCM, pTd, null, "A");
                    frmTE.LoadTable();
                    uint step = (uint)GetElementSize(pTd.DataType);
                    uint addr = pTd.StartAddress();
                    if (pTd.RowMajor)
                    {
                        for (int r = 0; r < pTd.Rows; r++)
                        {
                            for (int c = 0; c < pTd.Columns; c++)
                            {
                                xpatch.Data += GetValue(PCM.buf, addr, pTd, 0, PCM).ToString().Replace(",", ".") + " ";
                                addr += step;
                            }
                        }
                    }
                    else
                    {
                        for (int c = 0; c < pTd.Columns; c++)
                        {
                            for (int r = 0; r < pTd.Rows; r++)
                            {
                                xpatch.Data += GetValue(PCM.buf, addr, pTd, 0, PCM).ToString().Replace(",", ".") + " ";
                                addr += step;
                            }
                        }
                    }
                    newPatch.Add(xpatch);
                }
                Logger("Saving to file: " + Path.GetFileName(patchFname), false);

                using (FileStream stream = new FileStream(patchFname, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    writer.Serialize(stream, newPatch);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void GenerateTablePatchTableData()
        {
            try
            {
                List<TableData> tableTds = GetSelectedTableTds();
                if (tableTds.Count == 0)
                {
                    Logger("No tables selected");
                    return;
                }
                Logger("Creating patches...");
                for (int i = 0; i < tableTds.Count; i++)
                {
                    TableData srcTd = tableTds[i];
                    TableData patchTd = new TableData();
                    patchTd.addrInt = 0;
                    patchTd.DataType = InDataType.UBYTE;
                    patchTd.TableDescription = srcTd.TableDescription;
                    patchTd.TableName = "Patch: " + srcTd.TableName;
                    patchTd.Category = "Patch";
                    patchTd.AddCategory(srcTd.ExtraCategories);
                    patchTd.OS = srcTd.OS;
                    patchTd.CompatibleOS = "Table:" + srcTd.TableName + ",columns:" + srcTd.Columns.ToString() + ",rows:" + srcTd.Rows.ToString();
                    frmTableEditor frmTE = new frmTableEditor();
                    frmTE.PrepareTable(PCM, srcTd, null, "A");
                    frmTE.LoadTable();
                    uint step = (uint)GetElementSize(srcTd.DataType);
                    uint addr = srcTd.StartAddress();
                    patchTd.Values = "TablePatch: ";
                    if (srcTd.RowMajor)
                    {
                        for (int r = 0; r < srcTd.Rows; r++)
                        {
                            for (int c = 0; c < srcTd.Columns; c++)
                            {
                                patchTd.Values += GetValue(PCM.buf, addr, srcTd, 0, PCM).ToString().Replace(",", ".") + " ";
                                addr += step;
                            }
                        }
                    }
                    else
                    {
                        for (int c = 0; c < srcTd.Columns; c++)
                        {
                            for (int r = 0; r < srcTd.Rows; r++)
                            {
                                patchTd.Values += GetValue(PCM.buf, addr, srcTd, 0, PCM).ToString().Replace(",", ".") + " ";
                                addr += step;
                            }
                        }
                    }
                    PCM.tableDatas.Add(patchTd);
                }
                FilterTables(true);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void createPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateTablePatch(true);
        }

        private void SetWorkingMode()
        {
            int workingMode = AppSettings.WorkingMode;

            switch(workingMode)
            {
                case 0: //Tourist
                    touristToolStripMenuItem.Checked = true;
                    advancedToolStripMenuItem.Checked = false;
                    basicToolStripMenuItem.Checked = false;
                    settingsToolStripMenuItem.Visible = false;
                    utilitiesToolStripMenuItem.Visible = false;

                    xmlToolStripMenuItem.Visible = false;
                    tableListToolStripMenuItem.Visible = false;
                    findDifferencesHEXToolStripMenuItem.Visible = false;
                    findDifferencesToolStripMenuItem.Visible = false;
                    applyPatchToolStripMenuItem.Visible = false;

                    saveAllBINFilesToolStripMenuItem.Visible = false;
                    openMultipleBINToolStripMenuItem.Visible = false;
                    reloadFileFromDiskToolStripMenuItem.Visible = false;
                    openCompareBINToolStripMenuItem.Visible = false;

                    cSVexperimentalToolStripMenuItem.Visible = false;
                    cSV2ExperimentalToolStripMenuItem.Visible = false;
                    xMLGeneratorExportToolStripMenuItem.Visible = false;
                    xMlgeneratorImportCSVToolStripMenuItem.Visible = false;
                    insertRowToolStripMenuItem.Enabled = false;
                    deleteRowToolStripMenuItem.Enabled = false;
                    editRowToolStripMenuItem.Enabled = false;
                    duplicateTableConfigToolStripMenuItem.Enabled = false;

                    dTCToolStripMenuItem.Visible = false;
                    tableSeekToolStripMenuItem.Visible = false;
                    tinyTunerDBV6OnlyToolStripMenuItem.Visible = false;
                    xMLGeneratorExportToolStripMenuItem.Visible = false;
                    xMlgeneratorImportCSVToolStripMenuItem.Visible = false;
                    cSVexperimentalToolStripMenuItem.Visible = false;
                    cSV2ExperimentalToolStripMenuItem.Visible = false;
                    massModifyTableListsToolStripMenuItem.Visible = false;
                    massModifyTableListsSelectFilesToolStripMenuItem.Visible = false;
                    swapSegmentsToolStripMenuItem.Visible = false;

                    showTablesWithEmptyAddressToolStripMenuItem.Visible = false;
                    showTablesWithEmptyAddressToolStripMenuItem.Checked = false;
                    unitsToolStripMenuItem.Visible = false;
                    resetTunerModeColumnsToolStripMenuItem.Visible = false;
                    groupExtraOffset.Visible = false;
                    btnFlash.Visible = false;

                    break;
                case 1: //Basic
                    basicToolStripMenuItem.Checked = true;
                    touristToolStripMenuItem.Checked = false;
                    advancedToolStripMenuItem.Checked = false;

                    settingsToolStripMenuItem.Visible = true;
                    utilitiesToolStripMenuItem.Visible = true;
                    xmlToolStripMenuItem.Visible = true;
                    tableListToolStripMenuItem.Visible = true;
                    findDifferencesHEXToolStripMenuItem.Visible = true;
                    findDifferencesToolStripMenuItem.Visible = true;
                    applyPatchToolStripMenuItem.Visible = true;
                    swapSegmentsToolStripMenuItem.Visible = true;

                    saveAllBINFilesToolStripMenuItem.Visible = true;
                    openMultipleBINToolStripMenuItem.Visible = true;
                    reloadFileFromDiskToolStripMenuItem.Visible = true;
                    openCompareBINToolStripMenuItem.Visible = true;

                    cSVexperimentalToolStripMenuItem.Visible = false;
                    cSV2ExperimentalToolStripMenuItem.Visible = false;
                    xMLGeneratorExportToolStripMenuItem.Visible = false;
                    xMlgeneratorImportCSVToolStripMenuItem.Visible = false;
                    insertRowToolStripMenuItem.Enabled = false;
                    deleteRowToolStripMenuItem.Enabled = false;
                    editRowToolStripMenuItem.Enabled = false;
                    duplicateTableConfigToolStripMenuItem.Enabled = false;

                    dTCToolStripMenuItem.Visible = false;
                    tableSeekToolStripMenuItem.Visible = false;
                    tinyTunerDBV6OnlyToolStripMenuItem.Visible = false;
                    xMLGeneratorExportToolStripMenuItem.Visible = false;
                    xMlgeneratorImportCSVToolStripMenuItem.Visible = false;
                    cSVexperimentalToolStripMenuItem.Visible = false;
                    cSV2ExperimentalToolStripMenuItem.Visible = false;
                    massModifyTableListsToolStripMenuItem.Visible = false;
                    massModifyTableListsSelectFilesToolStripMenuItem.Visible = false;
                    swapSegmentsToolStripMenuItem.Visible = false;

                    showTablesWithEmptyAddressToolStripMenuItem.Visible = false;
                    showTablesWithEmptyAddressToolStripMenuItem.Checked = false;
                    unitsToolStripMenuItem.Visible = false;
                    resetTunerModeColumnsToolStripMenuItem.Visible = false;
                    groupExtraOffset.Visible = false;
                    btnFlash.Visible = false;

                    EnableTunerModeColumns();
                    break;
                case 2: //Advanced
                    basicToolStripMenuItem.Checked = false;
                    touristToolStripMenuItem.Checked = false;
                    advancedToolStripMenuItem.Checked = true;
                    settingsToolStripMenuItem.Visible = true;

                    utilitiesToolStripMenuItem.Visible = true;
                    xmlToolStripMenuItem.Visible = true;
                    tableListToolStripMenuItem.Visible = true;
                    findDifferencesHEXToolStripMenuItem.Visible = true;
                    findDifferencesToolStripMenuItem.Visible = true;
                    applyPatchToolStripMenuItem.Visible = true;
                    swapSegmentsToolStripMenuItem.Visible = true;

                    saveAllBINFilesToolStripMenuItem.Visible = true;
                    openMultipleBINToolStripMenuItem.Visible = true;
                    reloadFileFromDiskToolStripMenuItem.Visible = true;
                    openCompareBINToolStripMenuItem.Visible = true;

                    advancedToolStripMenuItem.Checked = true;
                    cSVexperimentalToolStripMenuItem.Visible = true;
                    cSV2ExperimentalToolStripMenuItem.Visible = true;
                    xMLGeneratorExportToolStripMenuItem.Visible = true;
                    xMlgeneratorImportCSVToolStripMenuItem.Visible = true;
                    insertRowToolStripMenuItem.Enabled = true;
                    deleteRowToolStripMenuItem.Enabled = true;
                    editRowToolStripMenuItem.Enabled = true;
                    duplicateTableConfigToolStripMenuItem.Enabled = true;

                    dTCToolStripMenuItem.Visible = true;
                    tableSeekToolStripMenuItem.Visible = true;
                    tinyTunerDBV6OnlyToolStripMenuItem.Visible = true;
                    xMLGeneratorExportToolStripMenuItem.Visible = true;
                    xMlgeneratorImportCSVToolStripMenuItem.Visible = true;
                    cSVexperimentalToolStripMenuItem.Visible = true;
                    cSV2ExperimentalToolStripMenuItem.Visible = true;
                    massModifyTableListsToolStripMenuItem.Visible = true;
                    massModifyTableListsSelectFilesToolStripMenuItem.Visible = true;

                    showTablesWithEmptyAddressToolStripMenuItem.Visible = true;
                    showTablesWithEmptyAddressToolStripMenuItem.Checked = true;
                    unitsToolStripMenuItem.Visible = true;
                    resetTunerModeColumnsToolStripMenuItem.Visible = false;
                    groupExtraOffset.Visible = true;
                    btnFlash.Visible = true;

                    break;
            }
            ReorderColumns();
        }

        private void SelectTreemode()
        {
            if (DisplayMode == DispMode.Tree)
                return;
            DisplayMode = DispMode.Tree;
            dataGridView1.Visible = false;
            treeView1.Visible = false;
                //treeView1.SelectedNodes.Clear();
            dataGridView1.DataSource = null;
            if (splitTree == null)
            {
                splitTree = new SplitContainer();
                splitTree.Orientation = Orientation.Vertical;
                splitTree.Panel1.Controls.Add(tabControl1);
                splitContainer2.Panel1.Controls.Add(splitTree);
                splitTree.Dock = DockStyle.Fill;
                tabControl1.Dock = DockStyle.Fill;
                tabDimensions.Enter += TabDimensions_Enter;
                tabValueType.Enter += TabValueType_Enter;
                tabPatches.Enter += TabPatches_Enter;
                tabCategory.Enter += TabCategory_Enter;
                tabSegments.Enter += TabSegments_Enter;
                tabSettings.Enter += TabSettings_Enter;
                tabSettings.Leave += TabSettings_Leave;
                tabFileInfo.Enter += TabFileInfo_Enter;
                tabMultiTree.Enter += TabMultiTree_Enter;
            }
            splitTree.Visible = true;
            splitContainerListMode.Visible = false;
            tabControl1.Visible = true;
            //currentTab = "Dimensions";
            //LoadDimensions();
            btnCollapse.Visible = true;
            btnExpand.Visible = true;
            //numIconSize.Visible = true;
            //labelIconSize.Visible = true;
            cutToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            selectToolStripMenuItem.Enabled = true;
            //labelBy.Visible = false;
            //comboFilterBy.Visible = false;
            //comboFilterBy.Text = "TableName";
            labelCategory.Visible = false;
            comboTableCategory.Visible = false;
            comboTableCategory.Text = "_All";
            btnExecute.Visible = false;
            txtMath.Visible = false;
        }

        private void TabMultiTree_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabMultiTree)
                return;
            currentTab = tabMultiTree;
            Debug.WriteLine("TabMultiTree_Enter");
            FilterTree(true);
        }

        private void LoadMultiTree()
        {
            try
            {

                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                if (tabMultiTree.Controls.Contains(treeMulti))
                {
                    treeMulti.SelectedNodes.Clear();
                    treeMulti.Nodes.Clear();
                }
                else
                {
                    treeMulti = new TreeViewMS();
                    treeMulti.Name = "TreeMulti";
                    SetIconSize();
                    treeMulti.ImageList = imageList1;
                    treeMulti.CheckBoxes = false;
                    treeMulti.Dock = DockStyle.Fill;
                    treeMulti.HideSelection = false;
                    tabMultiTree.Controls.Add(treeMulti);
                    treeMulti.AfterSelect += Tree_AfterSelect;
                    treeMulti.NodeMouseClick += Tree_NodeMouseClick;
                    treeMulti.AfterExpand += TreeMulti_AfterExpand;
                }
                AddNodes(treeMulti.Nodes, PCM,filteredTableDatas.ToList(), true);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner, line " + line + ": " + ex.Message);
            }

        }

        private void TreeMulti_AfterExpand(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                if (AppSettings.TableExplorerUseCategorySubfolder && e.Node.Parent == null)
                {
                    {
                        Tnode tnode1 = (Tnode)node.Tag;
                        if (!IncludesCollection(node, NType.Category, false))
                            AddCategories(node.Nodes, tnode1.filteredTds, false);
                    }
                }
                else
                {
                    AddTablesToTree(node);
                }
            }
        }

        private void Tree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (AppSettings.TableExplorerUseCategorySubfolder && e.Node.Parent == null )
            {
                Tnode tnode1 = (Tnode)e.Node.Tag;
                if (!IncludesCollection(e.Node, NType.Category, false))
                    AddCategories(e.Node.Nodes, tnode1.filteredTds, false);
            }
            AddTablesToTree(e.Node);
            foreach(TreeNode node in e.Node.Nodes)
            {
                AddTablesToTree(node);
            }
        }

        private void UpdateFileInfoTab()
        {
            try
            {
                if (currentTab != tabFileInfo)
                    return;
                tabControlFileInfo.Dock = DockStyle.Fill;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    string tabName = "tab" + mi.Text.Substring(0, 1);
                    if (tabControlFileInfo.TabPages[tabName] == null)
                    {
                        TabPage newTab = new TabPage(mi.Text.Substring(0, 1));
                        newTab.Name = tabName;
                        tabControlFileInfo.TabPages.Add(newTab);
                        PcmFile infoPcm = (PcmFile)mi.Tag;
                        RichTextBox rBox = new RichTextBox();
                        tabControlFileInfo.TabPages[tabName].Controls.Add(rBox);
                        rBox.Font = new Font("Consolas", 8);
                        rBox.Dock = DockStyle.Fill;
                        ShowFileInfo(infoPcm, rBox);
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
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void TabFileInfo_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabFileInfo)
                return;
            currentTab = tabFileInfo;
            UpdateFileInfoTab();
        }

        private void SelectListMode()
        {
            if (DisplayMode == DispMode.List)
                return;

            DisplayMode = DispMode.List;
            ClearPanel2();
            if (splitTree != null)
                splitTree.Visible = false;
            splitContainerListMode.Visible = true;
            if (hstForm != null || AppSettings.TunerListModeTreeWidth == 0 || AppSettings.TunerListModeTreeWidth > this.Width/2)
            {
                splitContainerListMode.SplitterDistance = 200;
            }
            else if (AppSettings.TunerListModeTreeWidth > 0)
            {
                splitContainerListMode.SplitterDistance = AppSettings.TunerListModeTreeWidth;
            }
                
            dataGridView1.Visible = true;
            dataGridView1.DataSource = filteredTableDatas;
            treeView1.Visible = true;
            //treeView1.SelectedNodes.Clear();
            //treeView1.Nodes.Clear();
            if (treeView1.Nodes.Count == 0)
            {
                AddNodes(treeView1.Nodes, PCM, PCM.tableDatas, true);
                treeView1.AfterSelect += TreeView1_AfterSelect;
                treeView1.AfterExpand += TreeView1_AfterExpand;
                treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            }
            btnCollapse.Visible = false;
            btnExpand.Visible = false;
            //numIconSize.Visible = false;
            //labelIconSize.Visible = false;
            cutToolStripMenuItem.Enabled = true;
            copyToolStripMenuItem.Enabled = true;
            pasteToolStripMenuItem.Enabled = true;
            selectToolStripMenuItem.Enabled = false;

            labelBy.Visible = true;
            comboFilterBy.Visible = true;
            labelCategory.Visible = true;
            comboTableCategory.Visible = true;
            btnExecute.Visible = true;
            txtMath.Visible = true;
        }


        private void TreeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            Tnode tnode1 = (Tnode)e.Node.Tag;
            if (e.Node.Name == "All")
            {
                TreeParts.AddTablesToTree(e.Node);
            }
            if (e.Node.Name != "All")
            { 
                TreeParts.AddChildNodes(e.Node, PCM);
                foreach (TreeNode node in e.Node.Nodes)
                {
                    TreeParts.AddChildNodes(node, PCM);
                }
            }            
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && treeView1.SelectedNode.Parent.Name == "Patches")
                {
                    contextMenuStripPatch.Show(Cursor.Position.X, Cursor.Position.Y);
                }
                ContextMenuStrip cxMenu = new ContextMenuStrip();
                MenuItem mi = new MenuItem("Expand all");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner, line " + line + ": " + ex.Message);
            }
        }

        private void ShowPatch(Patch Patch)
        {
            try
            {
                ClearPanel2();
                dataGridView1.Visible = true;
                dataGridView1.DataSource = Patch.patches;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner showPatch, line " + line + ": " + ex.Message);
            }
        }

        private void FilterByTreeview1()
        {
            if (!treeView1.Nodes["All"].IsSelected && !treeView1.Nodes["Patches"].IsSelected && treeView1.SelectedNodes.Count > 0)
            {
                Debug.WriteLine("Filtering by tree");
                DrawingControl.SuspendDrawing(dataGridView1);
                this.dataGridView1.SelectionChanged -= new System.EventHandler(this.DataGridView1_SelectionChanged);
                List<TableData> newTDList = new List<TableData>();
                foreach (TreeNode tn in treeView1.SelectedNodes)
                {
                    TreeParts.Tnode tnode = (TreeParts.Tnode)tn.Tag;
                    newTDList.AddRange(tnode.filteredTds);
                }
                filteredTableDatas = new BindingList<TableData>(newTDList);
                dataGridView1.DataSource = filteredTableDatas;
                this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
                DrawingControl.ResumeDrawing(dataGridView1);
                Debug.WriteLine("OK");
                Debug.WriteLine("Total table count, filtered by selected tree node: " + newTDList.Count.ToString());
            }

        }
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                Debug.WriteLine("Treeview1 afterselect");
                TreeParts.Tnode tnode = (TreeParts.Tnode)e.Node.Tag;
                if (e.Node.Name == "Patches")
                {
                    TreeParts.AddPatchNodes(e.Node, PCM);
                    treeView1.ContextMenuStrip = contextMenuStripPatch;
                    return;
                }
                if (tnode.NodeType == TreeParts.NType.Patch)
                {
                    ShowPatch(tnode.Patch);
                    treeView1.ContextMenuStrip = contextMenuStripPatch;
                    return;
                }
                /*                if (AppSettings.TableExplorerUseCategorySubfolder)
                                {
                                    if (tnode.NodeType != TreeParts.NType.Category)
                                    {
                                        AddCategories(e.Node.Nodes, tnode.filteredTds,true);
                                    }
                                }
                */
                //dataGridView1.DataSource = filteredTableDatas;
                treeView1.ContextMenuStrip = contextMenuStripListTree;
                if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
                {
                    SaveCurrentPath(treeView1);
                    //FilterTables(true);
                    FilterByTreeview1();
                }
                else
                {
                    //FilterTables(false);
                    FilterByTreeview1();
                }
                //if (e.Node.Nodes.Count == 0 && e.Node.Name != "All" && e.Node.Parent != null)
                if (e.Node.Name != "All" && e.Node.Parent != null)
                    TreeParts.AddChildNodes(e.Node, PCM);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner TreeView1_AfterSelect, line " + line + ": " + ex.Message);
            }
        }

        private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                    if (tv.SelectedNode != null && tv.SelectedNode.Tag != null)
                    {
                        //lastSelectedId = Convert.ToInt32(tv.SelectedNode.Tag);
                        Tnode tnode = (Tnode)tv.SelectedNode.Tag;
                        lastSelectTd = tnode.Td;
                    }
                    else
                    {
                        lastSelectTd = null;
                    }
                    contextMenuStripTree.Show(Cursor.Position.X, Cursor.Position.Y);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void FindCheckedNodes(TreeNode tn, ref List<int> tableIds)
        {
            if (tn.Nodes.Count > 0)
            {
                foreach (TreeNode tnChild in tn.Nodes)
                    FindCheckedNodes(tnChild, ref tableIds);
            }
            else
            {
                if (tn.Checked)
                    tableIds.Add((int)tn.Tag);
            }
        }

        private void Tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.Checked)
                {
                    foreach (TreeNode tn in e.Node.Nodes)
                        tn.Checked = true;
                }
                else
                {
                    foreach (TreeNode tn in e.Node.Nodes)
                        tn.Checked = false;
                }
            }
        }

        private bool AutoMultiTable()
        {
            try
            {
                if (!chkAutoMulti1d.Checked)
                    return false;
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                if (tv.SelectedNode == null)
                    return false;
                //if (tv.SelectedNode.Tag != null)
                  //  return;
                List<TableData> tableTds = new List<TableData>();
                foreach (TreeNode tn in tv.SelectedNode.Nodes)
                {
                    Tnode tnode = (Tnode)tn.Tag;
                    TableData sTd = tnode.Td;
                    if (sTd != null && sTd.Dimensions() == 1 && !sTd.TableName.Contains("[") && !sTd.TableName.Contains("."))
                        tableTds.Add(sTd);
                }
                if (tableTds.Count > 0)
                {
                    //ClearPanel2();
                    OpenTableEditor(tableTds);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
            return false;
        }

        private void SaveCurrentPath(TreeViewMS tv)
        {
            try
            {
                if (Navigating)
                {
                    Debug.WriteLine("Navigating, saving disabled");
                    //return;
                }
                List<string> path = GetCurrentNodePath(tv);
                if (path == null || path.Count ==0 )
                {
                    Debug.WriteLine("Empty path, not saving");
                    return;
                }
                TabPage tab = tabControl1.SelectedTab;
                Tnode tnode = (Tnode)tv.SelectedNode.Tag;
                TableData td = null;
                if (DisplayMode == DispMode.List)
                {
                    tab = null;
                    if (filteredTableDatas.Count > 0)
                    {
                        td = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                        path.Insert(0, td.TableName);
                    }
                }
                else
                {
                    td = tnode.Td;
                }
                Navi navi = new Navi(tab, path, txtFilter.Text, comboFilterBy.Text, td);
                Navi naviPrev = PCM.Navigator.LastOrDefault();
                if (naviPrev != null && naviPrev.Path != null && naviPrev.Path.Count > 0 && navi.Tab != null && navi.Tab.Name != null)
                {
                    if (navi.NodeSerial() == naviPrev.NodeSerial())
                    {
                        Debug.WriteLine("Duplicate Navi, not saving");
                        return;
                    }

                    int count = 0;
                    for (int n= PCM.Navigator.Count-1; n>= 0; n--)
                    {
                        naviPrev = PCM.Navigator[n];
                        if (navi.PathSerial() == naviPrev.PathSerial())
                        {
                            count++;
                            if (count >= AppSettings.NavigatorMaxTablesPerNode)
                            {
                                Debug.WriteLine( AppSettings.NavigatorMaxTablesPerNode.ToString() + " nodes in same path. removing oldest");
                                PCM.Navigator.RemoveAt(n);
                                break;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("After " + count.ToString() + " navi's, found navi with different path. Saving");
                            break;
                        }
                    }
                }
                PCM.Navigator.Add(navi);
                Debug.WriteLine("Adding navi: " + navi.NodeSerial());
                while (PCM.Navigator.Count > AppSettings.NavigatorMaxTablesTotal)
                {
                    Debug.WriteLine("Removing items from navigator, currentcount: " + PCM.Navigator.Count);
                    PCM.Navigator.RemoveAt(0);
                }
                PCM.NaviCurrent = PCM.Navigator.Count - 1;
                if (path == null || path.Count == 0)
                {
                    return;
                }
                if (PCM.SelectedNode.ContainsKey(tv.Name))
                {
                    PCM.SelectedNode[tv.Name].Clear();
                    PCM.SelectedNode[tv.Name].AddRange(path);
                }
                else
                {
                    PCM.SelectedNode.Add(tv.Name, path);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                //Logger(DateTime.Now.ToString() + " After Select");
                Debug.WriteLine("Tree_AfterSelect");
                Tnode tnode1 = (Tnode)e.Node.Tag;
                if (e.Node.Name == "Patches")
                {
                    ClearPanel2();
                    ShowPatchSelector();
                    return;
                }
                if (tnode1.NodeType == NType.Patch)
                {
                    return;
                }
                if (tabControl1.SelectedTab.Name == "tabSettings" || tabControl1.SelectedTab.Name == "tabPatches")
                    return;
                TreeViewMS tms = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
                {
                    SaveCurrentPath(tms);
                }
                if (AppSettings.TableExplorerUseCategorySubfolder && 
                    (e.Node.Parent == null || tnode1.ParentTnode == null|| tnode1.ParentTnode.Isroot))
                {
                    if (!IncludesCollection(e.Node,NType.Category,false))
                        AddCategories(e.Node.Nodes, tnode1.filteredTds,false);
                }
                AddTablesToTree(e.Node);

                if (tnode1.NodeType != NType.Table)
                {
                    if (AutoMultiTable())
                        return; 
                    //If multitable fail, we may have other tasks to do
                }
                if (!splitTree.Panel2.Controls.ContainsKey("frmTableEditor"))
                    ClearPanel2();
                List<TableData> tableTds = new List<TableData>();
                foreach (TreeNode tn in tms.SelectedNodes)
                {
                    TreeParts.Tnode tnode = (TreeParts.Tnode)tn.Tag;
                    if (tnode.NodeType == TreeParts.NType.Table)
                        tableTds.Add(((TreeParts.Tnode)tn.Tag).Td);
                }
                // showTableDescription(PCM, tableIds[0]);
                if (histogramTableSelectionEnabled)
                {
                    hstForm.SetupTable(PCM, tableTds[0]);
                }
                if (tableTds == null || tableTds.Count == 0)
                {
                    ClearPanel2();
                }
                else
                {
                    lastSelectTd = tableTds.LastOrDefault();
                    EnableTreeModeAxisMenus(lastSelectTd);
                    OpenTableEditor(tableTds);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }


        private void TabSegments_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabSegments)
                return;
            Debug.WriteLine("TabSegments_Enter");
            currentTab = tabSegments;
            FilterTree(true);
        }


        private void ShowPatchSelector()
        {
            frmPatchSelector frmP = new frmPatchSelector();
            frmP.tunerForm = this;
            frmP.basefile = PCM;
            frmP.TopLevel = false;
            frmP.Dock = DockStyle.Fill;
            frmP.FormBorderStyle = FormBorderStyle.None;
            //frmP.splitContainer1.SplitterWidth = frmP.splitContainer1.Height;
            frmP.splitContainer1.Panel2Collapsed = true;
            frmP.splitContainer1.Panel2.Hide();
            splitTree.Panel2.Controls.Add(frmP);
            frmP.Show();
            Application.DoEvents();
            frmP.LoadPatches();
        }

        private void TabPatches_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabPatches)
                return;
            if (PCM == null || PCM.configFileFullName == null || PCM.configFileFullName.Length == 0)
                return;
            currentTab = tabPatches;
            ClearPanel2();
            ShowPatchSelector();
        }

        private void TabValueType_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabValueType)
                return;
            currentTab = tabValueType;
            Debug.WriteLine("TabValueType_Enter");
            FilterTree(true);
        }

        private void TabDimensions_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabDimensions)
                return;
            currentTab = tabDimensions;
            Debug.WriteLine("TabDimensions_Enter");
            FilterTree(true);
        }

        private void TabCategory_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabCategory)
                return;
            currentTab = tabCategory;
            Debug.WriteLine("TabCategory_Enter");
            FilterTree(true);
        }

        private void TabSettings_Leave(object sender, EventArgs e)
        {
            AppSettings.TableExplorerIconSize = (int)numIconSize.Value;
            AppSettings.NavigatorMaxTablesPerNode = (int)numNaviMaxTablesPerNode.Value;
            AppSettings.NavigatorMaxTablesTotal = (int)numNaviMaxTablesTotal.Value;
            while (PCM.Navigator.Count > AppSettings.NavigatorMaxTablesTotal)
            {
                Debug.WriteLine("Removing items from navigator, currentcount: " + PCM.Navigator.Count);
                PCM.Navigator.RemoveAt(PCM.Navigator.Count - 1);
            }
            AppSettings.TunerShowTableCount = chkShowTableCount.Checked;
            AppSettings.Save();
            SetIconSize();
            RefreshListModeTree();
        }

        private void TabSettings_Enter(object sender, EventArgs e)
        {
            if (currentTab == tabSettings)
                return;
            currentTab = tabSettings;
        }

        private void SetIconSize()
        {
            try
            {
                if (iconSize != (int)numIconSize.Value)
                {
                    //Size modified since last call
                    iconSize = (int)numIconSize.Value;
                    imageList1.ImageSize = new Size(iconSize, iconSize);
                    imageList1.Images.Clear();
                    string folderIcon = Path.Combine(Application.StartupPath, "Icons", "explorer.ico");
                    imageList1.Images.Add(Image.FromFile(folderIcon));
                    string iconFolder = Path.Combine(Application.StartupPath, "Icons");
                    GalleryArray = System.IO.Directory.GetFiles(iconFolder);
                    for (int i = 0; i < GalleryArray.Length; i++)
                    {
                        if (GalleryArray[i].ToLower().EndsWith(".ico"))
                        {
                            //Debug.WriteLine("Icon: " + GalleryArray[i]);
                            imageList1.Images.Add(Path.GetFileName(GalleryArray[i]), Icon.ExtractAssociatedIcon(GalleryArray[i]));
                        }
                    }
                }
                if (treeDimensions != null)
                {
                    treeDimensions.ItemHeight = iconSize + 2;
                    treeDimensions.Indent = iconSize + 4;
                    treeDimensions.Font = AppSettings.TableExplorerFont.ToFont();
                }
                if (treeCategory != null)
                {
                    treeCategory.ItemHeight = iconSize + 2;
                    treeCategory.Indent = iconSize + 4;
                    treeCategory.Font = AppSettings.TableExplorerFont.ToFont();
                }
                if (treeSegments != null)
                {
                    treeSegments.ItemHeight = iconSize + 2;
                    treeSegments.Indent = iconSize + 4;
                    treeSegments.Font = AppSettings.TableExplorerFont.ToFont();
                }
                if (treeValueType != null)
                {
                    treeValueType.ItemHeight = iconSize + 2;
                    treeValueType.Indent = iconSize + 4;
                    treeValueType.Font = AppSettings.TableExplorerFont.ToFont();
                }
                if (treeMulti != null)
                {
                    treeMulti.ItemHeight = iconSize + 2;
                    treeMulti.Indent = iconSize + 4;
                    treeMulti.Font = AppSettings.TableExplorerFont.ToFont();
                }

                treeView1.ItemHeight = iconSize + 2;
                treeView1.Indent = iconSize + 4;
                treeView1.Font = AppSettings.TableExplorerFont.ToFont();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void LoadDimensions()
        {
            try
            {
                if (tabDimensions.Controls.Contains(treeDimensions))
                {
                    treeDimensions.Nodes.Clear();
                    treeDimensions.SelectedNodes.Clear();
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Application.DoEvents();
                    treeDimensions = new TreeViewMS();
                    treeDimensions.Name = "treeDimensions";
                    SetIconSize();
                    treeDimensions.ImageList = imageList1;
                    treeDimensions.CheckBoxes = false;
                    treeDimensions.Dock = DockStyle.Fill;
                    treeDimensions.HideSelection = false;
                    tabDimensions.Controls.Add(treeDimensions);
                    //treeDimensions.AfterCheck += Tree_AfterCheck;
                    treeDimensions.AfterSelect += Tree_AfterSelect;
                    treeDimensions.NodeMouseClick += Tree_NodeMouseClick;
                    treeDimensions.AfterExpand += Tree_AfterExpand;
                }
                AddDimensions(treeDimensions.Nodes, filteredTableDatas.ToList(),false);

                if (AppSettings.TableExplorerUseCategorySubfolder)
                {
                    Debug.WriteLine("Using subcats");
                    foreach (TreeNode node in treeDimensions.Nodes)
                    {
                        Tnode tnode = (Tnode)node.Tag;
                        if (!IncludesCollection(node, NType.Category, false))
                            AddCategories(node.Nodes, tnode.filteredTds, true);
                    }

                }
                else
                {
                    foreach (TreeNode dNode in treeDimensions.Nodes)
                    {
                        TreeParts.Tnode tnode = (TreeParts.Tnode)dNode.Tag;
                        for (int i = 0; i < tnode.filteredTds.Count; i++)
                        {
                            //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                            {
                                TableData fTd = tnode.filteredTds[i];
                                string ico = fTd.ValueType().ToString() +".ico";
                                Tnode tChild = new Tnode(fTd.TableName, fTd.TableName, NType.Table, ico, dNode);
                                tChild.Td = fTd;
                            }
                        }
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner, line " + line + ": " + ex.Message);
            }

        }

        private void LoadValueTypes()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                if (tabValueType.Controls.Contains(treeValueType))
                {
                    treeValueType.SelectedNodes.Clear();
                    treeValueType.Nodes.Clear();
                }
                else
                {
                    treeValueType = new TreeViewMS();
                    treeValueType.Name = "treeValueType";
                    SetIconSize();
                    treeValueType.ImageList = imageList1;
                    treeValueType.CheckBoxes = false;
                    treeValueType.Dock = DockStyle.Fill;
                    treeValueType.HideSelection = false;
                    tabValueType.Controls.Add(treeValueType);
                    treeValueType.AfterSelect += Tree_AfterSelect;
                    treeValueType.AfterCheck += Tree_AfterCheck;
                    treeValueType.NodeMouseClick += Tree_NodeMouseClick;
                    treeValueType.AfterExpand += Tree_AfterExpand;
                }

                AddValueTypes(treeValueType.Nodes, filteredTableDatas.ToList(),false);

                if (AppSettings.TableExplorerUseCategorySubfolder)
                {
                    Debug.WriteLine("Using subcats");
                    foreach (TreeNode node in treeValueType.Nodes)
                    {
                        Tnode tnode = (Tnode)node.Tag;
                        if (!IncludesCollection(node, NType.Category, false))
                            AddCategories(node.Nodes,tnode.filteredTds,true);
                    }
                }
                else
                {
                    foreach (TreeNode dNode in treeValueType.Nodes)
                    {
                        Tnode tnode = (Tnode)dNode.Tag;
                        for (int i = 0; i < tnode.filteredTds.Count; i++)
                        {
                            TableData fTd = tnode.filteredTds[i];
                            string ico = fTd.Dimensions().ToString() + "d.ico";
                            Tnode tChild = new Tnode(fTd.TableName, fTd.TableName, NType.Table, ico, dNode);
                            tChild.Td = fTd;
                        }
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void LoadCategories()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                if (tabCategory.Controls.Contains(treeCategory))
                {
                    treeCategory.SelectedNodes.Clear();
                    treeCategory.Nodes.Clear();
                }
                else
                {
                    treeCategory = new TreeViewMS();
                    treeCategory.Name = "treeCategory";
                    SetIconSize();
                    treeCategory.ImageList = imageList1;
                    treeCategory.CheckBoxes = false;
                    treeCategory.HideSelection = false;
                    treeCategory.Dock = DockStyle.Fill;
                    tabCategory.Controls.Add(treeCategory);
                    treeCategory.AfterSelect += Tree_AfterSelect;
                    treeCategory.AfterCheck += Tree_AfterCheck;
                    treeCategory.NodeMouseClick += Tree_NodeMouseClick;
                    treeCategory.AfterExpand += Tree_AfterExpand;
                }
                AddCategories(treeCategory.Nodes, filteredTableDatas.ToList(),false);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner, line " + line + ": " + ex.Message);
            }
        }

        private void LoadSegments()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                if (tabSegments.Controls.Contains(treeSegments))
                {
                    treeSegments.SelectedNodes.Clear();
                    treeSegments.Nodes.Clear();
                }
                else
                {
                    treeSegments = new TreeViewMS();
                    treeSegments.Name = "treeSegments";
                    SetIconSize();
                    treeSegments.ImageList = imageList1;
                    treeSegments.CheckBoxes = false;
                    treeSegments.HideSelection = false;
                    treeSegments.Dock = DockStyle.Fill;
                    tabSegments.Controls.Add(treeSegments);
                    treeSegments.AfterSelect += Tree_AfterSelect;
                    treeSegments.AfterCheck += Tree_AfterCheck;
                    treeSegments.NodeMouseClick += Tree_NodeMouseClick;
                    treeSegments.AfterExpand += Tree_AfterExpand;
                }
                AddSegments(treeSegments.Nodes,PCM, filteredTableDatas.ToList(),false);

                if (AppSettings.TableExplorerUseCategorySubfolder)
                {
                    Debug.WriteLine("Using subcats");
                    foreach (TreeNode node in treeSegments.Nodes)
                    {
                        Tnode tnode = (Tnode)node.Tag;
                        if (!IncludesCollection(node, NType.Category, false))
                            AddCategories(node.Nodes, tnode.filteredTds, true);
                    }
                }
                else
                {
                    foreach (TreeNode dNode in treeSegments.Nodes)
                    {
                        TreeParts.Tnode tnode = (TreeParts.Tnode)dNode.Tag;
                        for (int i = 0; i < tnode.filteredTds.Count; i++)
                        {
                            TableData fTd = tnode.filteredTds[i];
                            string ico = fTd.ValueType().ToString().Replace("number", "") + fTd.Dimensions() + "d.ico";
                            Tnode tChild = new Tnode(fTd.TableName, fTd.TableName, NType.Table, ico, dNode);
                            tChild.Td = fTd;
                        }
                    }
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            try
            {
                TreeViewMS tree = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                tree.ExpandAll();
            }
            catch { };
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            try
            {
                TreeViewMS tree = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                tree.CollapseAll();
            }
            catch { };
        }

        private void ClearPanel2(bool force = false)
        {
            if (splitTree == null)
                return;
            foreach (var x in splitTree.Panel2.Controls.OfType<Form>())
            {
                if (force)
                    x.Dispose();
                else
                    x.Close();
            }
            labelTableName.Text = "";
            txtDescription.Text = "";
        }

        private void SelectDispMode()
        {
            try
            {
                if (radioTreeMode.Checked)
                    SelectTreemode();
                else
                    SelectListMode();
                AppSettings.TunerTreeMode = radioTreeMode.Checked;
                AppSettings.Save();
                FilterTables(true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }


        private void radioTreeMode_CheckedChanged(object sender, EventArgs e)
        {
            radioListMode.Checked = !radioTreeMode.Checked;
            SelectDispMode();
        }

        private void radioListMode_CheckedChanged(object sender, EventArgs e)
        {
            //radioTreeMode.Checked = !radioListMode.Checked;
            //SelectDispMode();
        }

        private void chkShowCategorySubfolder_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.TableExplorerUseCategorySubfolder = chkShowCategorySubfolder.Checked;
            AppSettings.Save();
            FilterTree(true);
        }

        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TableData> tableTds = GetSelectedTableTds();
            OpenTableEditor(tableTds, true);
        }

        private void chkAutoMulti1d_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.TunerAutomulti1d = chkAutoMulti1d.Checked;
        }

        private void SelectDiffFile(bool hexMode)
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) return;
            PcmFile cmpWithPcm = new PcmFile(newFile, true, PCM.configFileFullName);
            LoadConfigforPCM(ref cmpWithPcm);
            AddtoCurrentFileMenu(cmpWithPcm, false);
            if (hexMode)
                FindTableDifferencesHEX(cmpWithPcm);
            else
                FindTableDifferences(cmpWithPcm);

        }
        private void selectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectDiffFile(false);
        }

        private void selectFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectDiffFile(true);
        }

        private void compareSelectedTablesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CompareSelectedTables();
        }

        private void importXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportXDF();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            tn.ExpandAll();
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            tn.Collapse();
        }

        private void expand2LevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            foreach (TreeNode childTn in tn.Nodes)
                TreeParts.AddChildNodes(childTn, PCM);
            tn.ExpandAll();

        }

        private void expand3LevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeView1.SelectedNode;
            foreach (TreeNode childTn in tn.Nodes)
            {
                TreeParts.AddChildNodes(childTn, PCM);
                foreach (TreeNode grandChild in childTn.Nodes)
                    TreeParts.AddChildNodes(grandChild, PCM);
            }
            tn.ExpandAll();

        }

        private void reloadFileFromDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.ReloadBinFile();
            ClearPanel2(true);
        }

        private void selectFileToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectDiffFile(false);
        }

        private void selectFileToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectDiffFile(true);
        }

        private void addTablesToExistingPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateTablePatch(false);
        }

        private void applyPatchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PatchList = patches[Convert.ToInt32(treeView1.SelectedNode.Tag)].patches;
            ApplyXMLPatch(ref PCM);
        }

        private void swapSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PCM == null)
                {
                    Logger("No file loaded");
                    return;
                }
                if (PCM.OS == null || PCM.OS == "")
                {
                    Logger("No OS segment defined");
                    return;
                }
                List<string> currentSegements = new List<string>();
                for (int s = 0; s < PCM.Segments.Count; s++)
                {
                    if (!PCM.Segments[s].Missing)
                    {
                        string seg = PCM.segmentinfos[s].Name.PadRight(15) + PCM.segmentinfos[s].PN + PCM.segmentinfos[s].Ver;
                        currentSegements.Add(seg);
                    }
                }
                frmSwapSegmentList frmSw = new frmSwapSegmentList();
                frmSw.LoadSegmentList(ref PCM);
                if (frmSw.ShowDialog(this) == DialogResult.OK)
                {
                    PCM = frmSw.PCM;
                    PCM.FixCheckSums();
                    LoggerBold(Environment.NewLine + "Swapped segments:");
                    for (int s = 0; s < PCM.Segments.Count; s++)
                    {
                        if (!PCM.Segments[s].Missing)
                        {
                            string newPN = PCM.segmentinfos[s].PN + PCM.segmentinfos[s].Ver;
                            if (!currentSegements[s].EndsWith(newPN))
                                Logger(currentSegements[s] + " => " + PCM.segmentinfos[s].PN + PCM.segmentinfos[s].Ver);
                        }
                    }
                    Logger("Segment(s) swapped and checksums fixed (you can save BIN now)");
                }
                frmSw.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void patcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmpatcher == null)
            {
                frmpatcher = new FrmPatcher();
                frmpatcher.Show();
            }
            else
                frmpatcher.BringToFront();

        }

        private void openCompareBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenNewBinFile(true);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void homepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "https://universalpatcher.net/";
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void sGMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = SelectFile("Select SGM File", "sgm files (*.sgm)|*.sgm|All files (*.*)|*.*");
                if (fileName.Length == 0)
                    return;

                frmImportFile fif = new frmImportFile();
                fif.ImportSGM(fileName);
                if (fif.ShowDialog() == DialogResult.OK && AppSettings.AutomaticOpenImportedFile)
                {
                    List<string> fList = new List<string>();
                    fList.Add(fif.outFileName);
                    OpenNewBinFile(false, fList);
                }
                return;

                //Old code:
/*                Logger("Reading file: " + fileName, false);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(SWCNT));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                SWCNT swcnt = (SWCNT)reader.Deserialize(file);
                file.Close();
                Logger(" [OK], converting...");

                SWCNTDATEN data = (SWCNTDATEN)swcnt.Items[3];
                SWCNTDATENDATENBLOECKEDATENBLOCK[] segments = data.DATENBLOECKE;

                int blockCount = segments.Length;
                List<byte[]> binBlocks = new List<byte[]>();
                int size = 0;
                for (int i = 0; i < blockCount; i++)
                {
                    string b64Data = segments[i].DATENBLOCKDATEN;
                    b64Data = b64Data.Substring(b64Data.IndexOf("base64") + 6);
                    byte[] b = System.Convert.FromBase64String(b64Data);
                    binBlocks.Add(b);
                    int start;
                    HexToInt(segments[i].STARTADR.Replace("0x", ""), out start);
                    int bSize = 0;
                    HexToInt(segments[i].GROESSEDEKOMPRIMIERT.Replace("0x", ""), out bSize);
                    if (size < (start + bSize))
                        size = start + bSize;
                    Logger("Segment: " + segments[i].DATENBLOCKNAME + " ", false);
                    SWCNTDATENDATENBLOECKEDATENBLOCKDATENBLOCKCHECK[] cs = segments[i].DATENBLOCKCHECK;
                    SWCNTDATENDATENBLOECKEDATENBLOCKLOESCHBEREICH[] segBlocks = segments[i].LOESCHBEREICH;
                    string sbStr = "";
                    for (int sb = 0; sb < segBlocks.Length; sb++)
                    {
                        sbStr += segBlocks[sb].STARTADR.Replace("0x", "") + " - " + segBlocks[sb].ENDADR.Replace("0x", "") + ", ";
                    }
                    //sbStr = sbStr.Trim(',');
                    Logger(sbStr, false);

                    for (int x = 0; x < cs.Length; x++)
                    {
                        Logger("Checksum: " + cs[x].CHECKSUMME.Replace("0x", "") + " (" + cs[x].STARTADR.Replace("0x", "") + " - " + cs[x].ENDADR.Replace("0x", "") + ")");
                        int csend = 0;
                        HexToInt(cs[x].ENDADR.Replace("0x", ""), out csend);
                        if (csend > size)
                            size = csend;
                    }
                }

                byte[] binbuf = new byte[size];

                for (int i = 0; i < blockCount; i++)
                {
                    byte[] b = binBlocks[i];
                    int start;
                    HexToInt(segments[i].STARTADR.Replace("0x", ""), out start);
                    Array.Copy(b, 0, binbuf, start, b.Length);
                }

                Logger("Done");

                fileName = SelectSaveFile(BinFilter, "imported-file.bin");
                if (fileName.Length == 0)
                    return;
                Logger("Saving to file: " + fileName, false);
                WriteBinToFile(fileName, binbuf);
                Logger(" [OK]");
                if (AppSettings.AutomaticOpenImportedFile)
                {
                    List<string> fList = new List<string>();
                    fList.Add(fileName);
                    OpenNewBinFile(false, fList);
                }
*/
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void intelHEXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = SelectFile("Select HEX File", "hex files (*.hex)|*.hex|All files (*.*)|*.*");
                if (fileName.Length == 0)
                    return;

                Logger("Importing file: " + fileName);
                frmImportFile fif = new frmImportFile();
                fif.ImportIntel(fileName);
                if (fif.ShowDialog() == DialogResult.OK && AppSettings.AutomaticOpenImportedFile)
                {
                    List<string> fList = new List<string>();
                    fList.Add(fif.outFileName);
                    OpenNewBinFile(false, fList);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner intelHEXToolStripMenuItem_Click, line " + line + ": " + ex.Message);
            }
        }

        private void motorolaSrecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = SelectFile("Select S19 File", "S19 files (*.s19)|*.s19|All files (*.*)|*.*");
                if (fileName.Length == 0)
                    return;
                Logger("Importing file: " + fileName);

                frmImportFile fif = new frmImportFile();
                fif.ImportMotorola(fileName);
                if (fif.ShowDialog() == DialogResult.OK && AppSettings.AutomaticOpenImportedFile)
                {
                    List<string> fList = new List<string>();
                    fList.Add(fif.outFileName);
                    OpenNewBinFile(false, fList);
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner motorolaSrecordToolStripMenuItem_Click, line " + line + ": " + ex.Message);
            }
        }

        private void createPatchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GenerateTablePatchTableData();
        }

        private void OpenAxisTable(string header)
        {
            TableData hTd = PCM.GetTdbyHeader(header);
            List<TableData> hTds = new List<TableData>();
            hTds.Add(hTd);
            OpenTableEditor(hTds, DisplayMode == DispMode.List);
        }

        private void EditAxisTable(string header)
        {
            try
            {
                TableData newTd = PCM.GetTdbyHeader(header).ShallowCopy(false);
                frmTdEditor fte = new frmTdEditor();
                fte.td = newTd;
                fte.LoadTd();
                int c = dataGridView1.CurrentCell.ColumnIndex;
                int r = dataGridView1.CurrentCell.RowIndex;
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    lastSelectTd = fte.td;
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas[id] = fte.td;
                            FilterTables(true);
                            dataGridView1.ClearSelection();
                            dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[c];
                            break;
                        }
                    }
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }

        private void editXaxisTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditAxisTable(lastSelectTd.ColumnHeaders);
        }

        private void editYaxisTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditAxisTable(lastSelectTd.RowHeaders);
        }

        private void openXaxisTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenAxisTable(lastSelectTd.ColumnHeaders);
        }

        private void openYaxisTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenAxisTable(lastSelectTd.RowHeaders);
        }

        private void editMathtableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData newTd = PCM.GetConversiotableByMath(lastSelectTd.Math);
                frmTdEditor fte = new frmTdEditor();
                fte.td = newTd;
                fte.LoadTd();
                int c = dataGridView1.CurrentCell.ColumnIndex;
                int r = dataGridView1.CurrentCell.RowIndex;
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas[id] = fte.td;
                            FilterTables(true);
                            dataGridView1.ClearSelection();
                            dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[c];
                            break;
                        }
                    }
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void openMathtableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData hTd = PCM.GetConversiotableByMath(lastSelectTd.Math);
            List<TableData> hTds = new List<TableData>();
            hTds.Add(hTd);
            OpenTableEditor(hTds, true);

        }

        private void btnFlash_Click(object sender, EventArgs e)
        {
            StartFlashApp(PCM, true);
        }

        private void ShowHistogram()
        {
            List<TableData> tds = GetSelectedTableTds();
            if (tds == null || tds.Count == 0)
            {
                Logger("Select table as template for histogram");
                return;
            }
            frmHistogram fh = new frmHistogram(false);
            fh.Show();
            fh.SetupTable(PCM, tds[0]);
        }
        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHistogram();
        }

        private void showHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHistogram();
        }

        private void loggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartLogger(PCM);
        }

        private void readWritePCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartFlashApp(null, false);
        }

        private void patcherToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmpatcher = new FrmPatcher();
            frmpatcher.Show();
            if (PCM != null && PCM.FileName != null)
                frmpatcher.OpenBaseFile(PCM.FileName);
        }

        private void touristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.WorkingMode = 0;
            AppSettings.Save();
            SetWorkingMode();
        }

        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.WorkingMode = 1;
            AppSettings.Save();
            SetWorkingMode();
        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.WorkingMode = 2;
            AppSettings.Save();
            SetWorkingMode();
        }

        private void createProgramShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCreateShortcuts fcs = new frmCreateShortcuts();
            fcs.Show();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCredits fc = new frmCredits();
            fc.Show();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    string mathStr = txtMath.Text.ToLower().Replace("x", cell.Value.ToString());
                    double newvalue = parser.Parse(mathStr);
                    cell.Value = newvalue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void TestExtraOffset(string Range, bool FilterOutOfRange, bool ColorCoding, bool OffsetBytes)
        {
            try
            {
                string[] parts = Range.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    LoggerBold("Supply range in format: Start - End");
                    return;
                }
                int Start;
                int End;
                if (!int.TryParse(parts[0], out Start))
                {
                    LoggerBold("Can't convert from hex: " + parts[0]);
                    return;
                }
                if (!int.TryParse(parts[1], out End))
                {
                    LoggerBold("Can't convert to integer: " + parts[1]);
                    return;
                }

                TableData tdTmp = ((TableData)dataGridView1.CurrentRow.DataBoundItem).ShallowCopy(false);

                List<byte> targetBytes = new List<byte>();
                PcmFile peekPCM = null;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    peekPCM = (PcmFile)mi.Tag;
                    if (peekPCM.FileName != PCM.FileName)
                    {
                        if (tdTmp.Offset > 0)
                        {
                            for (int o=0;o<tdTmp.Offset;o++)
                            {
                                targetBytes.Add(peekPCM.buf[tdTmp.addrInt + o]);
                            }
                        }
                        break;
                    }
                    else
                    {
                        peekPCM = null;
                    }
                }

                if (peekPCM == null)
                {
                    ColorCoding = false;
                }
                txtDescription.AppendText(Environment.NewLine);
                int OrigEO = tdTmp.extraoffset;
                for (int eo = Start; eo<=End; eo++)
                {
                    tdTmp.extraoffset = OrigEO + eo;
                    uint addr = tdTmp.StartAddress();
                    Color color = Color.Blue;
                    //double val = GetValue(PCM.buf, addr, tdTmp, 0, PCM);
                    bool offRange = false;
                    if (targetBytes.Count > 0 && OffsetBytes)
                    {
                        for (int o=0; o< tdTmp.Offset;o++)
                        {
                            if (PCM.buf[tdTmp.addrInt + tdTmp.extraoffset + o] != targetBytes[o])
                            {
                                offRange = true;    //Display only matches
                                break;
                            }
                        }
                    }
                    if (!offRange)
                    {
                        if (ColorCoding || FilterOutOfRange)
                        {
                            uint addrA = tdTmp.StartAddress();
                            uint addrB = (uint)(tdTmp.addrInt + tdTmp.Offset);
                            double maxVariation = 0;
                            for (int i = 0; i < tdTmp.Elements(); i++)
                            {
                                double valA = GetValue(PCM.buf, addrA, tdTmp, 0, PCM);
                                double valB = GetValue(peekPCM.buf, addrB, tdTmp, 0, PCM);
                                if (FilterOutOfRange && (valB > tdTmp.Max || valB < tdTmp.Min))
                                {
                                    Debug.WriteLine("Value out of range: " + valB.ToString());
                                    offRange = true;
                                    break;
                                }
                                double variation = Math.Abs((valA - valB) / valB);
                                if (variation > maxVariation)
                                {
                                    maxVariation = variation;
                                }
                                addrA += (uint)tdTmp.ElementSize();
                                addrB += (uint)tdTmp.ElementSize();
                            }
                            if (maxVariation == 0)
                            {
                                color = Color.Red;
                            }
                            else if (maxVariation <= 0.1)
                            {
                                color = Color.Green;
                            }
                            else
                            {
                                color = Color.Blue;
                            }
                        }
                    }
                    if (!offRange)
                    {
                        txtDescription.AppendText("Extra Offset: " + tdTmp.extraoffset.ToString() + " [" + addr.ToString("X") + "]" + Environment.NewLine);
                        PeekTableValues(tdTmp, PCM, false, color);
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
                LoggerBold("Error, frmTuner testExtraOffset, line " + line + ": " + ex.Message);
            }
        }

        private void testExtraOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmExtraOffset feo = new frmExtraOffset();
            feo.Text = "Offset range";
            feo.TextBox1.Text = "-10,10";
            feo.chkFilterOutOfRange.Visible = true;
            if (feo.ShowDialog() == DialogResult.OK)
            {
                TestExtraOffset(feo.TextBox1.Text, feo.chkFilterOutOfRange.Checked, feo.chkColorCoding.Checked, feo.chkOffsetBytes.Checked);
            }
            feo.Dispose();
        }

        private void FindExtraOffset(bool Fwd)
        {
            try
            {
                TableData tdTmp = ((TableData)dataGridView1.CurrentRow.DataBoundItem).ShallowCopy(false);

                double target = double.MaxValue;
                UInt64 Target3D = UInt64.MaxValue;
                PcmFile peekPCM = null;
                TableData compTd = null;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    peekPCM = (PcmFile)mi.Tag;
                    if (peekPCM.FileName != PCM.FileName)
                    {
                        //compTd = FindTableData(tdTmp, peekPCM.tableDatas);
                        compTd = tdTmp.ShallowCopy(true);
                        if (compTd != null)
                        {
                            target = GetValue(peekPCM.buf, compTd.addrInt, compTd, compTd.Offset, peekPCM);
                            if (compTd.Offset == 4)
                            {
                                //Special case
                                Target3D = ReadUint32(peekPCM.buf, compTd.addrInt, peekPCM.platformConfig.MSB);
                            }
                            break;
                        }
                    }
                    else
                    {
                        peekPCM = null;
                    }
                }
                if (peekPCM == null)
                {
                    LoggerBold("Compare file not loaded");
                    return;
                }

                txtDescription.AppendText(Environment.NewLine);
                int step = 1;
                if (!Fwd)
                {
                    step = -1;
                }
                bool found = false;
                Logger("Searching... ", false);
                if (ExtraOffsetFirstTry)
                {
                    ExtraOffsetFirstTry = false;
                }
                else
                {
                    tdTmp.extraoffset += step;
                }                
                for (int eo = tdTmp.extraoffset; (tdTmp.addrInt + tdTmp.Offset + eo) < PCM.fsize && (tdTmp.addrInt + tdTmp.Offset + eo) > 0; eo += step)
                {
                    int hits = 0;
                    if ((tdTmp.addrInt + tdTmp.Offset + tdTmp.extraoffset + tdTmp.Size()) > PCM.fsize)
                    {
                        hits = 0;
                        break;
                    }
                    tdTmp.extraoffset = eo;
                    uint cmpAddr = (uint)(compTd.addrInt + compTd.Offset);
                    uint addr = (uint)(tdTmp.addrInt + tdTmp.Offset + eo);
                    for (int r=0; r<tdTmp.Elements(); r++)
                    {
                        if ((addr + tdTmp.ElementSize()) > PCM.fsize || addr <= 0)
                        {
                            hits=0;
                            break;
                        }
                        if (GetRawValue(peekPCM.buf, cmpAddr, compTd, 0, peekPCM.platformConfig.MSB) == GetRawValue(PCM.buf, addr, tdTmp, 0, PCM.platformConfig.MSB))
                        {
                            hits++;
                        }
                        else
                        {
                            hits = 0;
                            break;
                        }
                        addr += (uint)tdTmp.ElementSize();
                        cmpAddr += (uint)compTd.ElementSize();
                    }
                    if (hits == tdTmp.Elements())
                    {
                        txtDescription.AppendText("Extra Offset: " + eo.ToString() + " [" + (tdTmp.addrInt + tdTmp.Offset + tdTmp.extraoffset).ToString("X") + "]" + Environment.NewLine);
                        PeekTableValues(tdTmp, PCM, false, Color.Red);
                        dataGridView1.CurrentRow.Cells["ExtraOffset"].Value = eo;
                        numExtraOffset.Value = eo;
                        dataGridView1.EndEdit();
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Logger(" Match not found");
                }
                else
                {
                    Logger(" [OK]");
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner testExtraOffset, line " + line + ": " + ex.Message);
            }
        }

        private void btnExtraOffsetNext_Click(object sender, EventArgs e)
        {
            FindExtraOffset(true);
        }

        private void radioExtraOffsetMinus_CheckedChanged(object sender, EventArgs e)
        {
            ExtraOffsetFirstTry = true;
        }

        private void numExtraOffset_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                TableData tdTmp = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                tdTmp.extraoffset = (int)numExtraOffset.Value;
                dataGridView1.CurrentRow.Cells["ExtraOffset"].Value = tdTmp.extraoffset;
                //dataGridView1.EndEdit();
                if (clearPreviewToolStripMenuItem.Checked)
                {
                    TableData selTd = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                    ShowTableDescription(PCM, selTd);
                }
                else
                {
                    PeekTableValues(tdTmp, PCM, false, Color.Blue);
                    if (appendFocusToolStripMenuItem.Checked)
                    {
                        txtDescription.SelectionStart = txtDescription.Text.Length;
                        txtDescription.ScrollToCaret();
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
                Debug.WriteLine("Error, frmTuner numExtraOffset_ValueChanged, line " + line + ": " + ex.Message);
            }
        }

        private void btnExtraOffsetPrev_Click(object sender, EventArgs e)
        {
            FindExtraOffset(false);
        }

        private void appendToPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            appendToPreviewToolStripMenuItem.Checked = true;
            clearPreviewToolStripMenuItem.Checked = false;
            appendFocusToolStripMenuItem.Checked = false;
        }

        private void clearPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            appendToPreviewToolStripMenuItem.Checked = false;
            clearPreviewToolStripMenuItem.Checked = true;
            appendFocusToolStripMenuItem.Checked = false;

        }

        private void appendFocusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            appendToPreviewToolStripMenuItem.Checked = false;
            clearPreviewToolStripMenuItem.Checked = false;
            appendFocusToolStripMenuItem.Checked = true;
        }

        private void addressRelativeDiffToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                TableData selTd = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile peekPCM = (PcmFile)mi.Tag;
                    if (peekPCM.FileName != PCM.FileName)
                    {
                        int seg = peekPCM.GetSegmentNumber((uint)(selTd.addrInt + selTd.Offset));
                        int diff = (int)PCM.segmentAddressDatas[seg].SegmentBlocks[0].Start - (int)peekPCM.segmentAddressDatas[seg].SegmentBlocks[0].Start;
                        Logger("Segment address difference: " + diff.ToString());
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
                Debug.WriteLine("Error, frmTuner addressRelativeDiffToolStripMenuItem1_Click, line " + line + ": " + ex.Message);
            }
        }

        private void btnExtraOffsetTest_Click(object sender, EventArgs e)
        {
            try
            {
                TableData tdTmp = ((TableData)dataGridView1.CurrentRow.DataBoundItem).ShallowCopy(true);
                tdTmp.extraoffset = (int)numExtraOffsetTest.Value;
                ShowTableDescription(PCM, tdTmp);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner btnExtraOffsetTest_Click, line " + line + ": " + ex.Message);
            }
        }

        private void btnExtraOffsetTestApply_Click(object sender, EventArgs e)
        {
            try
            {
                TableData tdTmp = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                tdTmp.extraoffset = (int)numExtraOffsetTest.Value;
                dataGridView1.CurrentRow.Cells["ExtraOffset"].Value = tdTmp.extraoffset;
                ShowTableDescription(PCM, tdTmp);
                numExtraOffset.Value = tdTmp.extraoffset;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner btnExtraOffsetTestApply_Click, line " + line + ": " + ex.Message);
            }
        }

        private void addNewTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int c = 0;
                int r = 0;
                TableData newTd = new TableData();
                frmTdEditor fte = new frmTdEditor();
                fte.td = newTd;
                fte.LoadTd();
                if (dataGridView1.Rows.Count > 0)
                {
                    c = dataGridView1.CurrentCell.ColumnIndex;
                    r = dataGridView1.CurrentCell.RowIndex;
                }
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    PCM.tableDatas.Add(fte.td);
                    FilterTables(true);
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells[c];
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void showOffsetVisualizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData compTd = null;
                PcmFile peekPCM = null;
                List<TableData> tds = GetSelectedTableTds();
                if (tds.Count == 0)
                {
                    Logger("No table selected");
                    return;
                }
                TableData selTd = tds[0];
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    peekPCM = (PcmFile)mi.Tag;
                    if (peekPCM.FileName != PCM.FileName)
                    {
                        compTd = FindTableData(selTd, peekPCM.tableDatas);
                        if (compTd != null)
                        {
                            break;
                        }
                    }
                }

/*                if (compTd == null)
                {
                    Logger("Please open another file");
                    return;
                }
*/
                if (ftvd == null || !ftvd.Visible)
                {
                    StartVisualizer(PCM, selTd, peekPCM, compTd, 0);
                    //Task.Factory.StartNew(() => StartVisualizer(PCM, selTd, peekPCM, compTd, 0));
                    //ftvd = new frmTableVisDouble(PCM, selTd, peekPCM, compTd, 0);
                    //ftvd.tuner = this;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }

        }
        //[STAThread]
        private void StartVisualizer(PcmFile PCM1, TableData td1, PcmFile PCM2, TableData td2, uint SelectedByte)
        {
            ftvd = new frmTableVisDouble(PCM1, PCM2,td1,td2);
            ftvd.tuner = this;
            ftvd.ShowTables(SelectedByte);
            ftvd.Show();
            //Application.Run(ftvd);
        }
        private void mirrorSegmentsFromCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mirrorSegmentsFromCompareToolStripMenuItem.Checked = !mirrorSegmentsFromCompareToolStripMenuItem.Checked;
            FilterTables(true);
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }

        public class Roska
        {
            public Roska()
            {
                Components = new List<string>();
            }
            public string eka { get; set; }
            [XmlArray]
            [XmlArrayItem("Component")]
            public List<string> Components { get; set; }
            public string Categories
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i=0; i<Components.Count;i++)
                    {
                        sb.Append(Components[i]);
                        if (i < Components.Count - 1)
                            sb.Append(" - ");
                    }
                    return sb.ToString();
                }
                set
                {
                    string[] parts = value.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    Components.AddRange(parts);
                }
            }

        }

        private void SelectExplorerFont()
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.Font = AppSettings.TableExplorerFont.ToFont();
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                AppSettings.TableExplorerFont = SerializableFont.FromFont(fontDlg.Font);
                AppSettings.Save();
                SetIconSize();
            }
            fontDlg.Dispose();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectExplorerFont();
        }


        private void btnExplorerFont_Click(object sender, EventArgs e)
        {
            SelectExplorerFont();
        }

        private void treeviewSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            radioTreeMode.Checked = true;
            tabControl1.SelectedTab = tabSettings;
        }

        private void Fmi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem fmi = (ToolStripMenuItem)sender;
            PcmFile cmpPcm = (PcmFile)fmi.Tag;
            if (cmpPcm == null)
            {
                return;
            }
            //AddSegments(treeView1.Nodes, cmpPcm, filteredTableDatas.ToList(), true, true);
            AddSegments(treeView1.Nodes, cmpPcm, cmpPcm.tableDatas, true, true);

        }

        private void renameDuplicateTablenamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Rename> renames = new List<Rename>();
            for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
            {
                TableData td1 = PCM.tableDatas[t1];
                string tableName = td1.TableName;
                int counter = 1;
                for (int t2 = t1 + 1; t2<PCM.tableDatas.Count; t2++)
                {
                    TableData td2 = PCM.tableDatas[t2];
                    if (td2.TableName == tableName)
                    {
                        string newName = tableName + "-" + counter.ToString("00");
                        if (td1.TableName == tableName)
                        {
                            Logger("Renaming: " + tableName + " (guid: " + td1.guid.ToString() + ") => " + newName);
                            //td1.TableName = newName;
                            renames.Add(new Rename(t1, newName));
                            counter++;
                        }
                        newName = tableName + "-" + counter.ToString("00");
                        //td2.TableName = newName;
                        renames.Add(new Rename(t2, newName));
                        Logger("Renaming: " + tableName + " (guid: " + td2.guid.ToString() + ") => " + newName);
                        counter++;
                    }
                }
            }
            if (renames.Count == 0)
            {
                Logger("No duplicates found");
            }
            else
            {
                if (MessageBox.Show("Apply new names?", "Rename tables?",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach(Rename rename in renames)
                    {
                        PCM.tableDatas[rename.Indx].TableName = rename.Newname;
                    }
                    Logger("Done");
                }
            }
        }

        private void ShowNaviTip()
        {
            if (PCM.Navigator.Count == 0)
                return;

            string message = "Navigator: " + (PCM.NaviCurrent + 1).ToString() + "/" + PCM.Navigator.Count.ToString();
            NaviTip.Show(message, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 30, 2000);
        }

        private void Navigate(int step)
        {
            try
            {
                if (PCM.Navigator.Count > 0)
                {
                    Navigating = true;
                    Navi naviOld = PCM.Navigator[PCM.NaviCurrent];
                    PCM.NaviCurrent += step;

                    ShowNaviTip();

                    TreeViewMS tv;
                    Navi navi = PCM.Navigator[PCM.NaviCurrent];
                    if (navi.Tab == null)
                    {
                        radioListMode.Checked = true;
                        tv = treeView1;
                    }
                    else
                    {
                        radioTreeMode.Checked = true;
                        tabControl1.SelectedTab = navi.Tab;
                        tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                    }
/*                    if (step != 0 && tv.SelectedNode != null && tv.SelectedNode.Parent != null &&
                        tv.SelectedNode.Parent.Nodes.ContainsKey(navi.Path[0]) && navi != null &&
                        navi.PathSerial() == naviOld.PathSerial())
                    {
                        Debug.WriteLine("Navigating directly in same path, different node");
                        tv.SelectedNode = tv.SelectedNode.Parent.Nodes[navi.Path[0]];
                    }
                    else
*/
                    {
                        txtFilter.TextChanged -= txFilter_TextChanged;
                        txtFilter.Text = "";
                        comboFilterBy.Text = navi.FilterBy;
                        txtFilter.Text = navi.Filter;
                        FilterTables(false);
                        RestoreNodePath(tv, navi.Path, PCM);
                        if (DisplayMode == DispMode.List)
                        {
                            for (int r = 0; r < dataGridView1.Rows.Count; r++)
                            {
                                TableData cTd = (TableData)dataGridView1.Rows[r].DataBoundItem;
                                if (cTd.guid == navi.Td.guid)
                                {
                                    dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells["TableName"];
                                    break;
                                }
                            }
                        }
                        Application.DoEvents();
                        txtFilter.TextChanged += txFilter_TextChanged;
                    }
                }
                Navigating = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
                Navigating = false;
            }
        }

        private void revToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PCM.NaviCurrent > 0)
            {
                Navigate(-1);
            }
        }

        private void fwdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PCM.NaviCurrent < (PCM.Navigator.Count -1))
            {
                Navigate(1);
            }

        }

        private void openInListModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                List<string> path = TreeParts.GetCurrentNodePath(tv);
                TableData rTd = GetSelectedTableTds().First();
                switch (tabControl1.SelectedTab.Name)
                {
                    case "Patches":
                        path.Add("Patches");
                        break;
                    case "tabSegments":
                        path.Add("Segments");
                        break;
                    case "tabDimensions":
                        path.Add("Dimensions");
                        break;
                    case "tabValueType":
                        path.Add("ValueTypes");
                        break;
                    case "tabCategory":
                        path.Add("Categories");
                        break;
                }
                radioListMode.Checked = true;
                Navigating = true;
                RestoreNodePath(treeView1, path, PCM);                
                Navigating = false;
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    TableData cTd = (TableData)dataGridView1.Rows[r].DataBoundItem;
                    if (cTd.guid == rTd.guid)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[r].Cells["TableName"];
                        break;
                    }
                }
                Navigating = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
                Navigating = false;
            }
        }

        private void FindAxisTable(string header)
        {
            try
            {
                Debug.WriteLine("Axis table, Navigator: " + PCM.NaviCurrent.ToString() + "/" + (PCM.Navigator.Count - 1).ToString());
                TableData hTd = PCM.GetTdbyHeader(header);
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                List<string> path = GetCurrentNodePath(tv);
                path[0] = hTd.TableName;    //Replace last node with axis-table
                txtFilter.TextChanged -= txFilter_TextChanged;
                txtFilter.Text = hTd.TableName + " | " + lastSelectTd.TableName;
                comboFilterBy.Text = "TableName";
                FilterTables(false);
                txtFilter.TextChanged += txFilter_TextChanged;
                Debug.WriteLine("Restoring node");
                StringBuilder dbgStr = new StringBuilder("Restoring " + tv.Name + ": ");
                for (int i = 0; i < path.Count; i++)
                {
                    dbgStr.Append(path[i] + ", ");
                }
                Debug.WriteLine(dbgStr.ToString());
                if (!tv.Nodes.ContainsKey(path.Last()))
                    return;
                TreeNode node = tv.Nodes[path.Last()];
                for (int i = path.Count - 2; i >= 0; i--)
                {
                    Tnode tnode1 = (Tnode)node.Tag;
                    if (tv.Name == "treeView1")
                    {
                        if (node.Name != "All" && node.Parent != null)
                            TreeParts.AddChildNodes(node, PCM);
                    }
                    else
                    {
                        if (AppSettings.TableExplorerUseCategorySubfolder &&
                            (node.Parent == null || tnode1.ParentTnode == null || tnode1.ParentTnode.Isroot))
                        {
                            if (!IncludesCollection(node, NType.Category, false))
                                AddCategories(node.Nodes, tnode1.filteredTds, false);
                        }
                        AddTablesToTree(node);
                    }
                    if (!node.Nodes.ContainsKey(path[i]))
                    {
                        Debug.WriteLine("Node " + node.Name + " not contains key: " + path[i]);
                        foreach(TreeNode child in node.Parent.Nodes)
                        {
                            Tnode tnode2 = (Tnode)child.Tag;
                            if (AppSettings.TableExplorerUseCategorySubfolder &&
                                (child.Parent == null || tnode2.ParentTnode == null || tnode2.ParentTnode.Isroot))
                            {
                                if (!IncludesCollection(child, NType.Category, false))
                                    AddCategories(child.Nodes, tnode2.filteredTds, false);
                            }
                            AddTablesToTree(child);
                            if (child.Nodes.ContainsKey(path[i]))
                            {
                                node = child;
                                Debug.WriteLine("Node " + child.Name + " contains key: " + path[i]);
                                break;
                            }
                        }
                    }
                    node = node.Nodes[path[i]];
                }
                tv.SelectedNode = node;
                Debug.WriteLine("Findaxistable Selected node: " + node.Name);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        void GotoAxisTable(string header)
        {
            try
            {
                Navigating = true;
                FindAxisTable(lastSelectTd.ColumnHeaders);
                Debug.WriteLine("Axis table, Navigator: " + (PCM.NaviCurrent + 1).ToString() + "/" + PCM.Navigator.Count.ToString());
                Application.DoEvents();
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                SaveCurrentPath(tv);
                Navigating = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
                Navigating = false;
            }
        }

        private void xAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GotoAxisTable(lastSelectTd.ColumnHeaders);
        }

        private void yAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GotoAxisTable(lastSelectTd.RowHeaders);
        }

        private void mathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GotoAxisTable(lastSelectTd.Math);
        }

        private void ShowNaviInfo(string NextPrev, int ind)
        {
            string message = NextPrev + Environment.NewLine + PCM.Navigator[ind].NaviInfo();
            NaviTip.Show(message, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y, 7000);
        }

        private void ShowNaviSelection()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            for (int i=0; i< PCM.Navigator.Count; i++)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(PCM.Navigator[i].PathStr());
                if (i == PCM.NaviCurrent)
                    mi.Checked = true;
                mi.Click += Mi_Click;
                mi.Tag = i;
                cms.Items.Add(mi);
            }
            cms.Show(Cursor.Position);
          
        }

        private void Mi_Click(object sender, EventArgs e)
        {
            int step = (int)((ToolStripMenuItem)sender).Tag - PCM.NaviCurrent;
            Navigate(step);
        }

        private void NaviMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ShowNaviSelection();
                //if (PCM.NaviCurrent < PCM.NaviGator.Count - 1)
                //{
                //    ShowNaviInfo("Next: ", PCM.NaviCurrent + 1);
                //}
            }
        }

        private void RevToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ShowNaviTip();
        }

        private void DataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                SaveCurrentPath(treeView1);
            }
        }

        private void groupNavigator_Enter(object sender, EventArgs e)
        {

        }

        private void copyRowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                for (int c = 0; c < dataGridView1.SelectedCells.Count; c++)
                    dataGridView1.Rows[dataGridView1.SelectedCells[c].RowIndex].Selected = true;
            }
            ClipBrd = new List<object>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                object obj = row.DataBoundItem;
                ClipBrd.Add(obj);
            }
        }

        private void pasteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                int row = dataGridView1.SelectedCells[0].RowIndex;
                for (int i = 0; i < ClipBrd.Count; i++)
                {
                    TableData newTd = ((TableData)ClipBrd[i]).ShallowCopy(true);
                    PCM.tableDatas.Add(newTd);
                }
                RefreshTablelist(true);
                dataGridView1.CurrentCell = dataGridView1.Rows[row].Cells[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void tabCategory_Click(object sender, EventArgs e)
        {

        }

        private void updateAddressesByOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PCM == null || string.IsNullOrEmpty(PCM.OS))
                return;
            PCM.UpdateAddressesByOS();
            RefreshTablelist(false);
        }

        private void editOSAddressPairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmEditPairs fep = new frmEditPairs();
                fep.pairStr = lastSelectTd.OS_Address;
                fep.OS = PCM.OS;
                if (fep.ShowDialog() == DialogResult.OK)
                {
                    lastSelectTd.OS_Address = fep.pairStr;
                    this.Refresh();
                }
                fep.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        public string CreateMapSession()
        {
            try
            {
                frmMapSession fms = new frmMapSession();
                if (fms.ShowDialog() != DialogResult.OK)
                {
                    return "";
                }
                SessionName = fms.txtSessionName.Text;
                mapSession = true;
                string sesPath = Path.Combine(Application.StartupPath, "TunerSessions", SessionName);
                Directory.CreateDirectory(sesPath);

                string newMapBin = Path.Combine(sesPath, Path.GetFileName(fms.txtMapPin.Text));
                Logger("Copying " + fms.txtMapPin.Text + " => " + newMapBin);
                File.Copy(fms.txtMapPin.Text, newMapBin, true);
                PcmFile mapPCM = new PcmFile(newMapBin, true, "");
                string mapOS = mapPCM.OS;
                if (string.IsNullOrEmpty(mapOS))
                {
                    mapOS = Path.GetFileNameWithoutExtension(fms.txtMapPin.Text);
                }
                string newMapDefFile = Path.Combine(sesPath, mapOS + "-map.XML");
                string newRefDefFile;

                AddtoCurrentFileMenu(mapPCM, false);

                string newRefBin = Path.Combine(sesPath, Path.GetFileName(fms.txtRefBin.Text));
                Logger("Copying " + fms.txtRefBin.Text + " => " + newRefBin);
                File.Copy(fms.txtRefBin.Text, newRefBin, true);
                PcmFile refPCM = new PcmFile(newRefBin, true, "");
                AddtoCurrentFileMenu(refPCM, true);
                if (fms.chkUseAutoloadTables.Checked)
                {
                    LoadConfigforPCM(ref refPCM);
                    newRefDefFile = Path.Combine(sesPath, Path.GetFileNameWithoutExtension(refPCM.FileName) + ".XML");
                    refPCM.SaveTableList(newRefDefFile);
                }
                else
                {
                    newRefDefFile = Path.Combine(sesPath, Path.GetFileName(fms.txtRefTableList.Text));
                    Logger("Copying " + fms.txtRefTableList.Text + " => " + newRefDefFile);
                    File.Copy(fms.txtRefTableList.Text, newRefDefFile, true);
                    refPCM.LoadTableList(newRefDefFile);
                }
                Logger("Copying " + newRefDefFile + " => " + newMapDefFile);
                File.Copy(newRefDefFile, newMapDefFile, true);
                mapPCM.ClearTableList();
                mapPCM.LoadTableList(newMapDefFile);
                mapPCM.RenameTableDatas(newMapDefFile);

                PCM = refPCM;
                SelectPCM();
                /*
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile mPCM = (PcmFile)mi.Tag;
                    if (mPCM.FileName == mapPCM.FileName)
                    {
                        mPCM.ClearTableList();
                        mPCM.LoadTableList(newRefDefFile);
                    }
                }
                */
                SaveSession(SessionName,true);
                return SessionName;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
                return "";
            }

        }
        private void openMapSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TunerMain != null)
            {
                TunerMain.AddSession(frmTunerMain.SessionType.Map);
            }
            else
            {
                CreateMapSession();
            }
        }

        public string SaveSession(string SessionName, bool Verbose)
        {
            try
            {
                if (string.IsNullOrEmpty(SessionName))
                {
                    FrmAsk fa = new FrmAsk();
                    fa.Text = "Session name?";
                    fa.labelAsk.Text = "Session name:";
                    if (string.IsNullOrEmpty(this.sessionname))
                        fa.TextBox1.Text = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
                    else
                        fa.TextBox1.Text = sessionname;
                    if (fa.ShowDialog() == DialogResult.OK)
                    {
                        SessionName = fa.TextBox1.Text;
                    }
                    else
                    {
                        return "";
                    }
                }
                if (string.IsNullOrEmpty(SessionName))
                {
                    return "";
                }
                this.SessionName = SessionName;
                string sesPath = Path.Combine(Application.StartupPath, "TunerSessions", SessionName);
                if (!Directory.Exists(sesPath))
                    Directory.CreateDirectory(sesPath);
                string fName = Path.Combine(sesPath, "Session.xml");
                SessionSettings sessionsettings = new SessionSettings();
                sessionsettings.SessionName = SessionName;
                sessionsettings.MapSession = mapSession;
                if (mapSession)
                    sessionsettings.CurrentBin = Path.GetFileName(PCM.FileName);
                else
                    sessionsettings.CurrentBin = PCM.FileName;
                sessionsettings.SortBy = sortBy;
                sessionsettings.SortIndex = sortIndex;
                sessionsettings.StrSortOrder = strSortOrder;
                int fNr = 0;
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    fNr++;
                    PcmFile sPCM = (PcmFile)mi.Tag;
                    SessionPcm sessPcm = new SessionPcm();
                    if (mapSession)
                    {
                        string newBinFile = Path.Combine(sesPath, Path.GetFileName(sPCM.FileName));
                        if (Verbose)
                            Logger("Saving " + newBinFile);
                        WriteBinToFile(newBinFile, sPCM.buf);
                        sessPcm.BinFile = newBinFile;
                    }
                    else
                    {
                        sessPcm.BinFile = sPCM.FileName;
                    }
                    sessPcm.NaviCurrent = sPCM.NaviCurrent;
                    sessPcm.CurrentTdList = sPCM.currentTableDatasList;
                    foreach (Navi navi in sPCM.Navigator)
                    {
                        NaviSetting ns = new NaviSetting();
                        if (navi.Td != null)
                        {
                            ns.TableSelection = navi.Td.guid;
                        }
                        ns.NavPath = navi.Path;
                        ns.Filter = navi.Filter;
                        ns.FilterBy = navi.FilterBy;
                        ns.Tab = navi.TabName;
                        sessPcm.NaviSettings.Add(ns);
                    }
                    sessionsettings.Pcms.Add(sessPcm);
                    for (int d=0;d<sPCM.altTableDatas.Count;d++)
                    {
                        if (sPCM.altTableDatas[d].tableDatas.Count > 0)
                        {
                            //string dFile = Path.Combine(sesPath, "tablelist-" + fNr.ToString() + "-" + d.ToString() + ".xml");
                            //if (mapSession && sPCM.altTableDatas[d].Name.StartsWith(sPCM.OS + "-map"))
                              //  dFile = Path.Combine(sesPath, sPCM.OS + "-map.xml");
                            string dFile = Path.Combine(sesPath, ReplaceFileNameInvalidChars(Path.GetFileNameWithoutExtension(sPCM.altTableDatas[d].Name)) + ".XML");
                            if (Verbose)
                                Logger("Saving file " + dFile);
                            using (FileStream stream = new FileStream(dFile, FileMode.Create))
                            {
                                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                                writer.Serialize(stream, sPCM.altTableDatas[d].tableDatas);
                                stream.Close();
                            }
                            sessPcm.TdListNames.Add(Path.GetFileName(sPCM.altTableDatas[d].Name));
                            sessPcm.TdListFiles.Add(Path.GetFileName(dFile));
                        }
                    }
                }
                if (Verbose)
                    Logger("Saving to file " + fName);
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(SessionSettings));
                    writer.Serialize(stream, sessionsettings);
                    stream.Close();
                }
                if (Verbose)
                    Logger("Done");
                return SessionName;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
                return "";
            }

        }
        public string LoadSession()
        {
            try
            {
                string sesPath = Path.Combine(Application.StartupPath, "TunerSessions");
                string defFile = Path.Combine(sesPath, "Session.xml");
                string fName = SelectFile("Select session file", XmlFilter,defFile);
                if (string.IsNullOrEmpty(fName))
                {
                    return "";
                }
                sesPath = Path.GetDirectoryName(fName);
                fName = Path.Combine(sesPath, "Session.xml");
                Debug.WriteLine("Loading file " + fName); 
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(SessionSettings));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                SessionSettings sessionsettings = (SessionSettings)reader.Deserialize(file);
                file.Close();
                this.SessionName = sessionsettings.SessionName;
                this.mapSession = sessionsettings.MapSession;
                string selectedBin = "";
                foreach (SessionPcm sessPcm in sessionsettings.Pcms)
                {
                    string binfile = sessPcm.BinFile;
                    if (sessionsettings.MapSession)
                    {
                        binfile = Path.Combine(sesPath, sessPcm.BinFile);
                    }
                    PcmFile newPCM = new PcmFile(Path.Combine(sesPath, sessPcm.BinFile), true, "");
                    AddtoCurrentFileMenu(newPCM, true);
                    PCM = newPCM;
                    SelectPCM();
                    PCM.ClearTableList();
                    for (int d=0;d<sessPcm.TdListNames.Count;d++)
                    {
                        string dFile = Path.Combine(sesPath, sessPcm.TdListFiles[d]);
                        if (File.Exists(dFile))
                        {
                            if (d > 0)
                            {
                                //First tablelist replace original tablelist, other added
                                AddNewTableList(sessPcm.TdListNames[d]);
                            }
                            PCM.LoadTableList(dFile);
                            comboTableCategory.Text = "_All";
                            RefreshTablelist(false);
                        }
                    }
                    foreach (NaviSetting ns in sessPcm.NaviSettings)
                    {
                        TableData tdN = PCM.tableDatas.Where(X => X.guid.Equals(ns.TableSelection)).FirstOrDefault();
                        TabPage tabN = null;
                        foreach (TabPage tab in tabControl1.TabPages)
                        {
                            if (tab.Name == ns.Tab)
                            {
                                tabN = tab;
                                break;
                            }
                        }
                        //Navi navi = new Navi(tabN, ns.NavPath.Replace("||","->"), ns.Filter, ns.FilterBy, tdN);
                        Navi navi = new Navi(tabN, ns.NavPath, ns.Filter, ns.FilterBy, tdN);
                        PCM.Navigator.Add(navi);
                    }
                    PCM.NaviCurrent = sessPcm.NaviCurrent;
                    PCM.currentTableDatasList = sessPcm.CurrentTdList;
                }
                selectedBin = Path.Combine(sesPath, sessionsettings.CurrentBin);
                sortBy = sessionsettings.SortBy;
                sortIndex = sessionsettings.SortIndex;
                strSortOrder = sessionsettings.StrSortOrder;                    
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile sPCM = (PcmFile)mi.Tag;
                    if (sPCM.FileName == selectedBin)
                    {
                        PCM = sPCM;
                        SelectPCM();
                        break;
                    }

                }
                Navigate(0);
                Logger("Done");
                return SessionName;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
                return "";
            }

        }
        private void saveMapSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSession(SessionName,true);
        }

        private void loadMapSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TunerMain != null)
            {
                TunerMain.AddSession(frmTunerMain.SessionType.Load);
            }
            else
            {
                LoadSession();
            }
        }

        private void saveSessionAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSession("",true);
        }

        private void renameTablelistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmData frmD = new frmData();
            frmD.Text = "New Tablelist name";
            string listName = "";
            if (frmD.ShowDialog() == DialogResult.OK)
            {
                listName = frmD.txtData.Text;
                PCM.RenameTableDatas(listName);
                for (int m = tableListToolStripMenuItem.DropDownItems.Count - 1; m >= 0; m--)
                {

                    if (tableListToolStripMenuItem.DropDownItems[m].Tag != null)
                        tableListToolStripMenuItem.DropDownItems.RemoveAt(m);
                }
                for (int i = 0; i < PCM.altTableDatas.Count; i++)
                {
                    ToolStripMenuItem miNew = new ToolStripMenuItem(PCM.altTableDatas[i].Name);
                    miNew.Name = PCM.altTableDatas[i].Name;
                    miNew.Tag = i;
                    if (i == PCM.currentTableDatasList)
                        miNew.Checked = true;
                    tableListToolStripMenuItem.DropDownItems.Add(miNew);
                    miNew.Click += tablelistSelect_Click;
                }
            }
        }

        private void closeSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TunerMain != null)
            {
                TunerMain.CloseSession(myTab);
            }
            else
            {
                this.Close();
            }
        }

        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TunerMain != null)
            {
                TunerMain.AddSession(frmTunerMain.SessionType.Empty);
            }
            else
            {
                SaveSession("",true);
            }
        }

        private void showOnlyMappedTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showOnlyMappedTablesToolStripMenuItem.Checked = !showOnlyMappedTablesToolStripMenuItem.Checked;
            //FilterTables(false);
            RefreshTablelist(false);
        }

        private void saveMappedTablesAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> extraOffsets = PCM.tableDatas.Where(X => X.ExtraOffset != "0").ToList();
                List<TableData> savingTds = new List<TableData>();
                foreach (TableData td in extraOffsets)
                {
                    TableData newTd = td.ShallowCopy(true); //Dont modify original
                    newTd.addrInt = (uint)(td.addrInt + td.extraoffset);
                    newTd.extraoffset = 0;
                    savingTds.Add(newTd);
                }
                dataGridView1.EndEdit();
                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + "-mapped.xml");
                string fName = SelectSaveFile(XmlFilter, defName);
                if (string.IsNullOrEmpty(fName))
                    return;
                Logger("Saving file " + fName + "...", false);

                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, savingTds);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void copyToTableseekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> tableTds = GetSelectedTableTds().OrderBy(X=>X.addrInt).ToList();
                foreach (TableData td in tableTds)
                {
                    Logger("Adding to tableseek: " + td.TableName);
                    TableSeek ts = new TableSeek();
                    ts.ImportTableData(td);
                    tableSeeks.Add(ts);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void editTableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmEditXML frmE = new frmEditXML();
                string fName = PCM.TableSeekFile;
                frmE.EditCurrentTableSeek(fName);
                frmE.Show();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }

        private void sortColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cols = AppSettings.TunerModeColumns;
            if (AppSettings.WorkingMode > 1)
            {
                cols = AppSettings.ConfigModeColumnOrder;
            }
            frmSortColumns frmsc = new frmSortColumns(cols);
            if (frmsc.ShowDialog() == DialogResult.OK)
            {
                ReorderColumns();
            }
        }

        private void undoRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRedo frmR = new frmRedo();
            frmR.ShowDialog();
            this.Refresh();
        }

        private void xDFnewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Generating xdf...");
            XDF xdf = new XDF();
            xdf.ExportXdf2(PCM, PCM.tableDatas);
        }

        private void launcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMain fMain = new FrmMain();
            fMain.Show();

        }

        private void createDebugLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createDebugLogToolStripMenuItem.Checked = !createDebugLogToolStripMenuItem.Checked;

            if (createDebugLogToolStripMenuItem.Checked)
            {
                string fName = SelectSaveFile(TxtFilter);
                DebugFileListener = new FileTraceListener(fName);
                Debug.Listeners.Add(DebugFileListener);
            }
            else
            {
                Debug.Listeners.Remove(DebugFileListener);
                DebugFileListener.CloseLog();
            }
        }
    }
}

