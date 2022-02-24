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
//using PcmHacking;

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

        public PcmFile PCM;
        //private List<TableData> tableDataList;
        private string sortBy = "TableName";
        private int sortIndex = 0;
        private bool columnsModified = false;
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        private BindingList<TableData> filteredTableDatas = new BindingList<TableData>();
        SortOrder strSortOrder = SortOrder.Ascending;
        private TableData lastSelectTd;        
        int keyDelayCounter = 0;
        private SplitContainer splitTree;
        private TreeViewMS treeDimensions;
        private TreeViewMS treeValueType;
        private TreeViewMS treeCategory;
        private TreeViewMS treeSegments;
        private string[] GalleryArray;
        string currentTab;
        int iconSize;
        bool treeMode;
        string currentBin = "A";

        private void frmTuner_Load(object sender, EventArgs e)
        {
            
            selectedCompareBin = "";
            SetWorkingMode();
            disableConfigAutoloadToolStripMenuItem.Checked = Properties.Settings.Default.disableTunerAutoloadSettings;
            chkShowCategorySubfolder.Checked = Properties.Settings.Default.TableExplorerUseCategorySubfolder;
            chkAutoMulti1d.Checked = Properties.Settings.Default.TunerAutomulti1d;

            radioTreeMode.Checked = Properties.Settings.Default.TunerTreeMode;
            radioListMode.Checked = !Properties.Settings.Default.TunerTreeMode;
            SelectDispMode();

            LogReceivers.Add(txtResult);

            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.TunerWindowSize.Width > 0 || Properties.Settings.Default.TunerWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.TunerWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.TunerWindowLocation;
                    this.Size = Properties.Settings.Default.TunerWindowSize;
                }
                if (Properties.Settings.Default.TunerLogWindowSize.Width > 0 || Properties.Settings.Default.TunerLogWindowSize.Height > 0)
                {
                    this.splitContainer2.SplitterDistance = Properties.Settings.Default.TunerLogWindowSize.Width;
                    this.splitContainer1.SplitterDistance = Properties.Settings.Default.TunerLogWindowSize.Height;
                }
                if (Properties.Settings.Default.TunerListModeTreeWidth > 0)
                    splitContainerListMode.SplitterDistance = Properties.Settings.Default.TunerListModeTreeWidth;

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
            string tunerCols = Properties.Settings.Default.TunerModeColumns;
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
            dataGridView1.DataSource = bindingsource;
            dataGridView1.AllowUserToAddRows = false;
            FilterTables();

            this.AllowDrop = true;
            this.DragEnter += FrmTuner_DragEnter;
            this.DragDrop += FrmTuner_DragDrop;
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
            FilterTables();
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

        private void frmTuner_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.TunerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.TunerWindowLocation = this.Location;
                    Properties.Settings.Default.TunerWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.TunerWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.TunerWindowSize = this.RestoreBounds.Size;
                }
                Size logSize = new Size();
                logSize.Width = this.splitContainer2.SplitterDistance;
                logSize.Height = this.splitContainer1.SplitterDistance;
                Properties.Settings.Default.TunerLogWindowSize = logSize;
                Properties.Settings.Default.TunerListModeTreeWidth = splitContainerListMode.SplitterDistance;
            }
            Properties.Settings.Default.Save();
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

        public void SelectPCM()
        {
            try
            {
                this.Text = "Tuner " + PCM.FileName + " [" + PCM.tunerFile + "]";
                PCM.SelectTableDatas(0, PCM.FileName);
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
                FilterTables();
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                    mi.Checked = false;
                foreach (ToolStripMenuItem mi in findDifferencesToolStripMenuItem.DropDownItems)
                    mi.Enabled = true;
                foreach (ToolStripMenuItem mi in findDifferencesHEXToolStripMenuItem.DropDownItems)
                    mi.Enabled = true;

                findDifferencesToolStripMenuItem.DropDownItems[PCM.FileName].Enabled = false;
                findDifferencesHEXToolStripMenuItem.DropDownItems[PCM.FileName].Enabled = false;

                ToolStripMenuItem mitem = (ToolStripMenuItem)currentFileToolStripMenuItem.DropDownItems[PCM.FileName];
                mitem.Checked = true;
                string tmpFL = currentBin;
                currentBin = GetFileLetter(mitem.Text);
                if (currentBin == selectedCompareBin)
                    selectedCompareBin = tmpFL;

                FilterTables();
                if (treeView1.Visible)
                {
                    treeView1.SelectedNodes.Clear();
                    TreeParts.AddNodes(treeView1.Nodes, PCM);
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

        public void LoadConfigforPCM(ref PcmFile newPCM)
        {
            try
            {
                if (!Properties.Settings.Default.disableTunerAutoloadSettings)
                {
                    newPCM.AutoLoadTunerConfig();
                    if (newPCM.tunerFile.Length == 0)
                    {
                        ImportTableSeek(ref newPCM);
                        if (newPCM.Segments.Count > 0 && newPCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                            ImportTinyTunerDB(ref newPCM);
                    }
                    ImportDTC(ref newPCM);

                    RefreshTablelist();
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
                TableData td = tableTds[0];
                if (td.Values.StartsWith("Patch:"))
                {
                    if (MessageBox.Show(td.TableDescription, "Apply patch?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ApplyTdPatch(td, ref PCM);
                    }
                    return;
                }
                else if (td.Values.StartsWith("TablePatch:"))
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

                if (td.OS != PCM.OS && !td.CompatibleOS.Contains("," + PCM.OS + ","))
                {
                    LoggerBold("WARING! OS Mismatch, File OS: " + PCM.OS + ", config OS: " + td.OS);
                }
                frmTableEditor frmT = new frmTableEditor();
                frmT.tuner = this;
                if (treeMode && !newWindow)
                {
                    frmT.Dock = DockStyle.Fill;
                    frmT.FormBorderStyle = FormBorderStyle.None;
                    frmT.TopLevel = false;
                    splitTree.Panel2.Controls.Add(frmT);
                }
                frmT.disableMultiTable = disableMultitableToolStripMenuItem.Checked;
                frmT.PrepareTable(PCM, td, tableTds, currentBin);
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile comparePCM = (PcmFile)mi.Tag;
                    if (PCM.FileName != comparePCM.FileName)
                    {
                        Logger("Adding file: " + Path.GetFileName(comparePCM.FileName) + " to compare menu... ", false);
                        if (PCM.OS == comparePCM.OS)
                        {
                            PcmFile tmpPcm = comparePCM.ShallowCopy(); //Don't mess with original
                            tmpPcm.tableDatas = PCM.tableDatas;
                            frmT.AddCompareFiletoMenu(tmpPcm, null, mi.Text, selectedCompareBin);
                            Logger("[OK]");
                        }
                        else
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
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
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
                Logger("Error: " + ex.Message);
            }

        }

        private void btnSaveBin_Click(object sender, EventArgs e)
        {
            SaveBin();
        }

        public void RefreshTablelist()
        {
            try
            {
                this.dataGridView1.SelectionChanged -= new System.EventHandler(this.DataGridView1_SelectionChanged);

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                //Don't fire events when adding data to combobox!
                this.comboTableCategory.SelectedIndexChanged -= new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);
                comboTableCategory.DataSource = null;
                categoryBindingSource.DataSource = null;
                if (!PCM.tableCategories.Contains("_All"))
                    PCM.tableCategories.Add("_All");
                PCM.tableCategories.Sort();
                categoryBindingSource.DataSource = PCM.tableCategories;
                comboTableCategory.DataSource = categoryBindingSource;
                comboTableCategory.Refresh();
                this.comboTableCategory.SelectedIndexChanged += new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);

                Application.DoEvents();
                FilterTables();
                if (treeView1.Visible)
                {
                    treeView1.SelectedNodes.Clear();
                    TreeParts.AddNodes(treeView1.Nodes, PCM);
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

        private void ImportTableSeek(ref PcmFile _PCM)
        {
            Logger("Importing tableseek...", false);
            Application.DoEvents();
            _PCM.ImportSeekTables();
            RefreshTablelist();
            Logger(" [OK]");
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            PCM.LoadTableList();
            RefreshTablelist();
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
            PCM.tableDatas = new List<TableData>();
            RefreshTablelist();
        }

        private void ReorderColumns()
        {
            try
            {
                string[] tunerColumns = Properties.Settings.Default.TunerModeColumns.ToLower().Split(',');
                string[] configColumns = Properties.Settings.Default.ConfigModeColumnOrder.ToLower().Split(',');
                string[] configWidth = Properties.Settings.Default.ConfigModeColumnWidth.Split(',');
                string[] tunerWidth = Properties.Settings.Default.TunerModeColumnWidth.Split(',');
                Debug.WriteLine("Tuner order: " + Properties.Settings.Default.TunerModeColumns);
                Debug.WriteLine("Config order: " + Properties.Settings.Default.ConfigModeColumnOrder);

                int invisibleIndex = tunerColumns.Length;
                for (int c = 0; c < dataGridView1.Columns.Count && c < configWidth.Length; c++)
                {
                    if (Properties.Settings.Default.WorkingMode == 2) //advanced
                    {
                        dataGridView1.Columns[c].ReadOnly = false;
                        dataGridView1.Columns[c].Visible = true;
                        int order = Array.IndexOf(configColumns, dataGridView1.Columns[c].HeaderText.ToLower());
                        if (order > -1 && order < dataGridView1.Columns.Count)
                            dataGridView1.Columns[c].DisplayIndex = order;
                        dataGridView1.Columns[c].Width = Convert.ToInt32(configWidth[c]);
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
                            dataGridView1.Columns[c].Width = Convert.ToInt32(tunerWidth[c]);
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
                if (sortIndex > dataGridView1.Columns.Count)
                {
                    dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }
        private void FilterTables()
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
                if (!Properties.Settings.Default.TunerShowUnitsUndefined || !Properties.Settings.Default.TunerShowUnitsMetric || !Properties.Settings.Default.TunerShowUnitsImperial)
                {
                    List<TableData> newTDList = new List<TableData>();
                    if (Properties.Settings.Default.TunerShowUnitsUndefined)
                        foreach (TableData tmpTd in PCM.tableDatas.Where(x => x.DispUnits == DisplayUnits.Undefined))
                            newTDList.Add(tmpTd);
                    if (Properties.Settings.Default.TunerShowUnitsImperial)
                        foreach (TableData tmpTd in PCM.tableDatas.Where(x => x.DispUnits == DisplayUnits.Imperial))
                            newTDList.Add(tmpTd);
                    if (Properties.Settings.Default.TunerShowUnitsMetric)
                        foreach (TableData tmpTd in PCM.tableDatas.Where(x => x.DispUnits == DisplayUnits.Metric))
                            newTDList.Add(tmpTd);
                    compareList = newTDList;
                }

                if (strSortOrder == SortOrder.Ascending)
                    compareList = compareList.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    compareList = compareList.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();

                IEnumerable<TableData> results = compareList;

                if (txtSearchTableSeek.Text.Length > 0)
                {
                    string newStr = txtSearchTableSeek.Text.Replace("OR", "|");
                    if (newStr.Contains("|"))
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
                        newStr = txtSearchTableSeek.Text.Replace("AND", "&");
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
                    results = results.Where(t => t.addrInt < uint.MaxValue);

                string cat = comboTableCategory.Text;
                if (cat != "_All" && cat != "")
                {
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
                        results = results.Where(t => t.Category.ToLower().Contains(comboTableCategory.Text.ToLower()));
                    }
                }

                if (!treeMode && !treeView1.Nodes["All"].IsSelected && !treeView1.Nodes["Patches"].IsSelected && treeView1.SelectedNodes.Count > 0)
                {
                    List<string> selectedSegs = new List<string>();
                    List<string> selectedCats = new List<string>();
                    List<string> selectedValTypes = new List<string>();
                    List<string> selectedDimensions = new List<string>();
                    foreach (TreeNode tn in treeView1.SelectedNodes)
                    {
                        switch (tn.Parent.Name)
                        {
                            case "Segments":
                                selectedSegs.Add(tn.Name);
                                break;
                            case "Categories":
                                selectedCats.Add(tn.Name);
                                break;
                            case "Dimensions":
                                selectedDimensions.Add(tn.Name);
                                break;
                            case "ValueTypes":
                                selectedValTypes.Add(tn.Name);
                                break;
                        }
                        TreeNode tnParent = tn.Parent;
                        while (tnParent.Parent != null)
                        {
                            switch (tnParent.Parent.Name)
                            {
                                case "Segments":
                                    selectedSegs.Add(tnParent.Name);
                                    break;
                                case "Categories":
                                    selectedCats.Add(tnParent.Name);
                                    break;
                                case "Dimensions":
                                    selectedDimensions.Add(tnParent.Name);
                                    break;
                                case "ValueTypes":
                                    selectedValTypes.Add(tnParent.Name);
                                    break;
                            }
                            tnParent = tnParent.Parent;
                        }
                    }

                    if (selectedSegs.Count > 0)
                    {
                        List<TableData> newTDList = new List<TableData>();
                        foreach (string seg in selectedSegs)
                        {
                            int segNr = 0;
                            for (int s = 0; s < PCM.segmentinfos.Length; s++)
                                if (PCM.segmentinfos[s].Name == seg)
                                    segNr = s;
                            uint addrStart = PCM.segmentAddressDatas[segNr].SegmentBlocks[0].Start;
                            uint addrEnd = PCM.segmentAddressDatas[segNr].SegmentBlocks[PCM.segmentAddressDatas[segNr].SegmentBlocks.Count - 1].End;
                            var newResults = results.Where(t => t.addrInt >= addrStart && t.addrInt <= addrEnd);
                            foreach (TableData nTd in newResults)
                                newTDList.Add(nTd);
                        }
                        results = newTDList;
                    }

                    if (selectedCats.Count > 0)
                    {
                        List<TableData> newTDList = new List<TableData>();
                        foreach (TableData td in results)
                        {
                            if (selectedCats.Contains(td.Category))
                                newTDList.Add(td);
                        }
                        results = newTDList;
                    }

                    if (selectedValTypes.Count > 0)
                    {
                        List<TableData> newTDList = new List<TableData>();
                        foreach (string valT in selectedValTypes)
                        {
                            if (valT == "mask")
                            {
                                foreach (TableData td in results)
                                {
                                    if (td.BitMask != null && td.BitMask.Length > 0)
                                        newTDList.Add(td);
                                }

                            }
                            else
                            {
                                foreach (TableData td in results)
                                {
                                    string tdValT = GetTableValueType(td).ToString();
                                    if (tdValT == valT)
                                        newTDList.Add(td);
                                }
                            }
                        }
                        results = newTDList;
                    }

                    if (selectedDimensions.Count > 0)
                    {
                        List<TableData> newTDList = new List<TableData>();
                        foreach (TableData td in results)
                        {
                            if (selectedDimensions.Contains(td.Dimensions().ToString() + "D"))
                                newTDList.Add(td);
                        }
                        results = newTDList;
                    }
                }

                filteredTableDatas = new BindingList<TableData>(results.ToList());
                bindingsource.DataSource = filteredTableDatas;
                ReorderColumns();
                txtDescription.Text = "";
                FilterTree();
                this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
                dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
                DrawingControl.ResumeDrawing(dataGridView1);
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
        }

        private void FilterTree()
        {
            ClearPanel2();
            if (tabControl1.SelectedTab.Name == "tabDimensions")
                LoadDimensions();
            else if (tabControl1.SelectedTab.Name == "tabValueType")
                LoadValueTypes();
            else if (tabControl1.SelectedTab.Name == "tabCategory")
                LoadCategories();
            else if (tabControl1.SelectedTab.Name == "tabSegments")
                LoadSegments();
        }
        private void comboTableCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("comboTableCategory_SelectedIndexChanged");
            FilterTables();
        }

        private void btnSearchTableSeek_Click(object sender, EventArgs e)
        {
            try
            {
                int rowindex = dataGridView1.CurrentCell.RowIndex;
                for (int i = rowindex + 1; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["TableName"].Value != null && dataGridView1.Rows[i].Cells["TableName"].Value.ToString().ToLower().Contains(txtSearchTableSeek.Text.ToLower()))
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
            RefreshTablelist();

        }

        private void btnReadTinyTunerDB_Click(object sender, EventArgs e)
        {
            ImportTinyTunerDB(ref PCM);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.LoadTableList();
            comboTableCategory.Text = "_All";
            RefreshTablelist();

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
                for (int t = 0; t < _PCM.tableDatas.Count; t++)
                {
                    if (_PCM.tableDatas[t].TableName == "DTC" || _PCM.tableDatas[t].TableName == "DTC.Codes")
                    {
                        haveDTC = true;
                        Logger(" DTC codes already defined");
                        break;
                    }
                }
                if (!haveDTC)
                {
                    TableData tdTmp = new TableData();
                    tdTmp.ImportDTC(_PCM, ref _PCM.tableDatas,true);
                    tdTmp.ImportDTC(_PCM, ref _PCM.tableDatas, false);
                    Logger(" [OK]");
                    FilterTables();
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
            PCM.tableDatas = new List<TableData>();
            PCM.tableCategories = new List<string>();
            RefreshTablelist();
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
                if (Properties.Settings.Default.WorkingMode > 0 && dataGridView1.SelectedCells.Count > 0 && e.Button == MouseButtons.Right)
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                            if (PCM.tableDatas[i].Category.ToLower() == cat.ToLower() && PCM.tableDatas[i].TableName.ToLower() == name.ToLower())
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
                RefreshTablelist();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void txtSearchTableSeek_TextChanged(object sender, EventArgs e)
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
            FilterTables();
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
                            if (PCM.tableDatas[i].Category.ToLower() == cat.ToLower() && PCM.tableDatas[i].TableName.ToLower().StartsWith(name.ToLower()))
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
                RefreshTablelist();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
            FilterTables();
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
                FilterTables();
            }
            else if (Properties.Settings.Default.WorkingMode != 2)
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
                Debug.WriteLine(ex.Message);
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
            RefreshTablelist();
        }

        private void DataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.WorkingMode != 2)
                {
                    e.Cancel = true;
                    return;
                }
                for (int r = 0; r < dataGridView1.SelectedRows.Count; r++)
                {
                    int row = dataGridView1.SelectedRows[r].Index;
                    int id = Convert.ToInt32(dataGridView1.Rows[row].Cells["id"].Value);
                    PCM.tableDatas.RemoveAt(id);
                    FilterTables();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void PeekTableValuesWithCompare(TableData shTd)
        {
            PeekTableValues(shTd, PCM); //Show values from current file 
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
            {
                PcmFile peekPCM = (PcmFile)mi.Tag;
                if (peekPCM.FileName != PCM.FileName)
                {
                    TableData compTd = FindTableData(shTd, peekPCM.tableDatas);
                    if (compTd != null)
                    {
                        txtDescription.AppendText(peekPCM.FileName + ": [" + shTd.TableName + "]" + Environment.NewLine);
                        PeekTableValues(compTd, peekPCM);
                    }
                }
            }
        }

        private void PeekTableValues(TableData shTd, PcmFile peekPCM)
        {
            try
            {
                if (shTd.addrInt >= peekPCM.fsize)
                {
                    Debug.WriteLine("No address defined");
                    return;
                }
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
                txtDescription.SelectionColor = Color.Blue;
                string minMax = " [";
                if (shTd.Min > double.MinValue)
                    minMax += " Min: " + shTd.Min.ToString();
                if (shTd.Max < double.MaxValue)
                    minMax += " Max: " + shTd.Max.ToString();
                if (minMax == " [")
                    minMax = "";
                else
                    minMax += "] ";
                if (shTd.Dimensions() == 1)
                {
                    double curVal = GetValue(peekPCM.buf, (uint)(shTd.addrInt + shTd.Offset), shTd, 0, peekPCM);
                    UInt64 rawVal = (UInt64)GetRawValue(peekPCM.buf, (uint)(shTd.addrInt + shTd.Offset), shTd, 0, peekPCM.platformConfig.MSB);
                    string valTxt = curVal.ToString();
                    string unitTxt = " " + shTd.Units;
                    string maskTxt = "";
                    TableValueType vt = GetTableValueType(shTd);
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
                    uint addr = (uint)(shTd.addrInt + shTd.Offset);
                    if (shTd.RowMajor)
                    {
                        for (int r = 0; r < shTd.Rows; r++)
                        {
                            for (int c = 0; c < shTd.Columns; c++)
                            {
                                double curVal = GetValue(peekPCM.buf, addr, shTd, 0, peekPCM);
                                addr += (uint)GetElementSize(shTd.DataType);
                                tblData.Append("[" + curVal.ToString("#0.0") + "]");
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
                                addr += (uint)GetElementSize(shTd.DataType);
                                tblRows[r] += "[" + curVal.ToString("#0.0") + "]";
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
                LoggerBold(ex.Message);
            }
        }

        public void ShowTableDescription(PcmFile PCM, TableData shTd)
        {
            try
            {
                if (PCM.buf == null || PCM.buf.Length == 0)
                    return;
                txtDescription.Text = "";
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Bold);
                txtDescription.AppendText(shTd.TableName + Environment.NewLine);
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
                if (shTd.TableDescription != null)
                    txtDescription.AppendText(shTd.TableDescription + Environment.NewLine);
                if (shTd.ExtraDescription != null)
                    txtDescription.AppendText(shTd.ExtraDescription + Environment.NewLine);

                PeekTableValuesWithCompare(shTd);
                labelTableName.Text = shTd.TableName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode != null)
                {
                    if (treeView1.SelectedNode.Name == "Patches")
                        return;
                    if (treeView1.SelectedNode.Parent != null && treeView1.SelectedNode.Parent.Name == "Patches")
                        return;
                }
                TableData selTd = (TableData)dataGridView1.CurrentRow.DataBoundItem;
                ShowTableDescription(PCM,selTd);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void tunerModeColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmData frmd = new frmData();
            frmd.txtData.Text = Properties.Settings.Default.TunerModeColumns;
            frmd.Text = "Columns visible in tuner mode:";
            if (frmd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.TunerModeColumns = frmd.txtData.Text;
            }
            Properties.Settings.Default.Save();
            frmd.Dispose();
            FilterTables();
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
                LoggerBold(ex.Message);
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
                LoggerBold(ex.Message);
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
                Debug.WriteLine(ex.Message);
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
                Debug.WriteLine(ex.Message);
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
                if (Properties.Settings.Default.WorkingMode == 2)
                {
                    //Advanced mode
                    Properties.Settings.Default.ConfigModeColumnOrder = order.ToString().Trim(',');
                    Properties.Settings.Default.ConfigModeColumnWidth = columnWidth.ToString().Trim(',');
                }
                else
                {
                    //Tuner mode
                    Properties.Settings.Default.TunerModeColumns = order.ToString().Trim(',');
                    Properties.Settings.Default.TunerModeColumnWidth = columnWidth.ToString().Trim(',');
                }
                Properties.Settings.Default.Save();
                columnsModified = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }
        }


        void Cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string[] tunerModCols = Properties.Settings.Default.TunerModeColumns.Split(',');

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
            }
        }

        void columnSelection_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            menuItem.Checked = !menuItem.Checked;
            Debug.WriteLine(menuItem.Name);
            dataGridView1.Columns[menuItem.Name].Visible = menuItem.Checked;
            columnsModified = true;
            if (Properties.Settings.Default.WorkingMode != 2)
                FilterTables();
        }

        private void resetTunerModeColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TunerModeColumns = "id,TableName,Category,Units,Columns,Rows,TableDescription";
            Properties.Settings.Default.TunerModeColumnWidth = "35,100,237,135,100,100,100,100,100,114,100,100,100,100,60,46,100,100,100,100,100,493,100,";
            Properties.Settings.Default.Save();
            FilterTables();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }
        private void loadBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            OpenNewBinFile(false);
            timer.Stop();
            Debug.WriteLine("Open new file time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
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
                tdMenuItem.Tag = newPCM.tableDataIndex;
                tableListToolStripMenuItem.DropDownItems.Add(tdMenuItem);
                tdMenuItem.Click += tablelistSelect_Click;

                UpdateFileInfoTab();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                    double orgVal = GetValue(pcm1.buf, td1.addrInt, td1, (uint)td1.Offset, pcm1);
                    double compVal = GetValue(pcm2.buf, td2.addrInt, td2, (uint)td2.Offset, pcm2);
                    if (orgVal == compVal)
                        return null;
                    else
                        return cmpTd;
                }
                byte[] buff1 = new byte[tbSize];
                byte[] buff2 = new byte[tbSize];
                Array.Copy(pcm1.buf, td1.addrInt + td1.Offset, buff1, 0, tbSize);
                Array.Copy(pcm2.buf, td2.addrInt + td2.Offset, buff2, 0, tbSize);
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
                frmHexDiff fhd = new frmHexDiff(PCM, cmpWithPcm, diffTableDatas, cmpTableDatas);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                        uint end = (uint)(start + GetElementSize(tData.DataType) * tData.Rows * tData.Columns + tData.Offset);
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
                frmHexDiff fhd = new frmHexDiff(PCM, cmpWithPcm, diffTableDatas, cmpTableDatas);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
            FilterTables();
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
        }

        private void tableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTableSeek(ref PCM);
        }

        private void ImportXDF()
        {
            XDF xdf = new XDF();
            xdf.ImportXdf(PCM, PCM.tableDatas);
            Debug.WriteLine("Categories: " + PCM.tableCategories.Count);
            //LoggerBold("Note: Only basic XDF conversions are supported, check Math and SavingMath values");
            RefreshTablelist();
            comboTableCategory.Text = "_All";

        }
        private void xDFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ImportXDF();
        }

        private void tinyTunerDBV6OnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTinyTunerDB(ref PCM);
        }


        private void cSVexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportxperimentalCSV();
        }

        private void cSV2ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportXperimentalCsv2();
        }

        private void AddNewTableList()
        {
            frmData frmD = new frmData();
            frmD.Text = "Tablelist name:";
            if (frmD.ShowDialog() == DialogResult.OK)
            {
                foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
                {
                    //Reset all to uncheck
                    mi.Checked = false;
                }
                ToolStripMenuItem mItem = new ToolStripMenuItem(frmD.txtData.Text);
                mItem.Name = frmD.txtData.Text;
                PCM.SelectTableDatas(PCM.altTableDatas.Count, mItem.Name);
                mItem.Tag = PCM.altTableDatas.Count - 1;
                mItem.Checked = true;
                tableListToolStripMenuItem.DropDownItems.Add(mItem);
                mItem.Click += tablelistSelect_Click;
                FilterTables();
            }


        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTableList();
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
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas.Insert(id, fte.td);
                            FilterTables();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }
        }

        private void editRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData newTd = lastSelectTd.ShallowCopy(false);
                frmTdEditor fte = new frmTdEditor();
                fte.td = newTd;
                fte.LoadTd();
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    lastSelectTd = fte.td;
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas[id] = fte.td;
                            FilterTables();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.tableDatas.Remove(lastSelectTd);
            FilterTables();
        }

        private void duplicateTableConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData newTd = lastSelectTd.ShallowCopy(true);
            for (int id = 0; id < PCM.tableDatas.Count; id++)
            {
                if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                {
                    PCM.tableDatas.Insert(id, newTd);
                    FilterTables();
                    break;
                }
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
            RefreshTablelist();
        }

        private void loadTablelistnewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewTableList();
            PCM.LoadTableList();
            comboTableCategory.Text = "_All";
            RefreshTablelist();
        }

        private void openMultipleBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Open files";
                frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
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
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void disableConfigAutoloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disableConfigAutoloadToolStripMenuItem.Checked = !disableConfigAutoloadToolStripMenuItem.Checked;

            Properties.Settings.Default.disableTunerAutoloadSettings = disableConfigAutoloadToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();

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
                LoggerBold(ex.Message);
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
                Logger(ex.Message);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void moreSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMoreSettings frmTS = new frmMoreSettings();
            frmTS.ShowDialog();
            FilterTables();
        }

        private void timerFilter_Tick(object sender, EventArgs e)
        {
            keyDelayCounter++;
            if (keyDelayCounter > Properties.Settings.Default.keyPressWait100ms)
            {
                timerFilter.Enabled = false;
                keyDelayCounter = 0;
                FilterTables();
            }
        }

        private void caseSensitiveFilteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caseSensitiveFilteringToolStripMenuItem.Checked = !caseSensitiveFilteringToolStripMenuItem.Checked;
            FilterTables();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                if (treeMode)
                {
                    TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                    //foreach (TreeNode tn in tv.Nodes)
                    //  findCheckdNodes(tn, ref tableIds);
                    foreach (TreeNode tn in tv.SelectedNodes)
                        if (tn.Tag != null)
                            tableTds.Add((TableData)tn.Tag);
                    if (tableTds.Count == 0)
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                    uint addr = (uint)(pTd.addrInt + pTd.Offset);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                    if (srcTd.Category.Length > 1)
                        patchTd.Category += "." + srcTd.Category;
                    patchTd.OS = srcTd.OS;
                    patchTd.CompatibleOS = "Table:" + srcTd.TableName + ",columns:" + srcTd.Columns.ToString() + ",rows:" + srcTd.Rows.ToString();
                    frmTableEditor frmTE = new frmTableEditor();
                    frmTE.PrepareTable(PCM, srcTd, null, "A");
                    frmTE.LoadTable();
                    uint step = (uint)GetElementSize(srcTd.DataType);
                    uint addr = (uint)(srcTd.addrInt + srcTd.Offset);
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
                FilterTables();
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void createPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateTablePatch(true);
        }

        private void SetWorkingMode()
        {
            int workingMode = Properties.Settings.Default.WorkingMode;
            if (workingMode == 0)   //Tourist mode
            {
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
            }
            else
            {
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

            }
            if (workingMode == 2) //advanced
            {
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
                unitsToolStripMenuItem.Visible = true;
                resetTunerModeColumnsToolStripMenuItem.Visible = false;
                btnFlash.Visible = true;

            }
            else
            {
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
                unitsToolStripMenuItem.Visible = false;
                resetTunerModeColumnsToolStripMenuItem.Visible = false;
                btnFlash.Visible = false;
            }
        }

        private void SelectTreemode()
        {
            treeMode = true;
            dataGridView1.Visible = false;
            treeView1.Visible = false;
            treeView1.SelectedNodes.Clear();
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
            }
            splitTree.Visible = true;
            splitContainerListMode.Visible = false;
            tabControl1.Visible = true;
            currentTab = "Dimensions";
            LoadDimensions();
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
        }

        private void UpdateFileInfoTab()
        {
            try
            {
                if (currentTab != "FileInfo")
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }
        }

        private void TabFileInfo_Enter(object sender, EventArgs e)
        {
            if (currentTab == "FileInfo")
                return;
            currentTab = "FileInfo";
            UpdateFileInfoTab();
        }

        private void SelectListMode()
        {
            ClearPanel2();
            treeMode = false;
            if (splitTree != null)
                splitTree.Visible = false;
            splitContainerListMode.Visible = true;
            dataGridView1.Visible = true;
            dataGridView1.DataSource = bindingsource;
            treeView1.Visible = true;
            if (treeView1.Nodes.Count == 0)
            {
                treeView1.SelectedNodes.Clear();
                TreeParts.AddNodes(treeView1.Nodes, PCM);
            }
            treeView1.AfterSelect += TreeView1_AfterSelect;
            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
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
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && treeView1.SelectedNode.Parent.Name == "Patches")
            {
                contextMenuStripPatch.Show(Cursor.Position.X, Cursor.Position.Y);
            }
            ContextMenuStrip cxMenu = new ContextMenuStrip();
            MenuItem mi = new MenuItem("Expand all");
        }

        private void ShowPatch(int ind)
        {
            try
            {
                dataGridView1.DataSource = patches[ind].patches;
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

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (e.Node.Name == "Patches")
                {
                    TreeParts.AddPatchNodes(e.Node, PCM);
                    treeView1.ContextMenuStrip = contextMenuStripPatch;
                    return;
                }
                if (e.Node.Parent != null && e.Node.Parent.Name == "Patches")
                {
                    ShowPatch(Convert.ToInt32(e.Node.Tag));
                    treeView1.ContextMenuStrip = contextMenuStripPatch;
                    return;
                }
                dataGridView1.DataSource = bindingsource;
                treeView1.ContextMenuStrip = contextMenuStripListTree;
                FilterTables();
                if (e.Node.Nodes.Count == 0 && e.Node.Name != "All" && e.Node.Parent != null)
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
                        lastSelectTd = (TableData)tv.SelectedNode.Tag;
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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

        private void AutoMultiTable()
        {
            try
            {
                if (!chkAutoMulti1d.Checked)
                    return;
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                if (tv.SelectedNode == null)
                    return;
                if (tv.SelectedNode.Tag != null)
                    return;
                List<TableData> tableTds = new List<TableData>();
                foreach (TreeNode tn in tv.SelectedNode.Nodes)
                {
                    TableData sTd = (TableData)tn.Tag;
                    if (sTd.Dimensions() == 1 && !sTd.TableName.Contains("[") && !sTd.TableName.Contains("."))
                        tableTds.Add(sTd);
                }
                if (tableTds.Count > 0)
                {
                    ClearPanel2();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "tabSettings" || tabControl1.SelectedTab.Name == "tabPatches")
                return;
            TreeViewMS tms = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
            if (e.Node.Tag == null)
            {
                AutoMultiTable();
                return;
            }
            ClearPanel2();
            List<TableData> tableTds = new List<TableData>();
            //int tbId = Convert.ToInt32(e.Node.Tag);
            foreach (TreeNode tn in tms.SelectedNodes)
                if (tn.Tag != null)
                    tableTds.Add((TableData)tn.Tag);
           // showTableDescription(PCM, tableIds[0]);
            OpenTableEditor(tableTds);
        }


        private void TabSegments_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Segments")
                return;
            currentTab = "Segments";
            ClearPanel2();
            LoadSegments();
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
            if (currentTab == "Patches")
                return;
            if (PCM == null || PCM.configFileFullName == null || PCM.configFileFullName.Length == 0)
                return;
            currentTab = "Patches";
            ClearPanel2();
            ShowPatchSelector();
        }

        private void TabValueType_Enter(object sender, EventArgs e)
        {
            if (currentTab == "ValueType")
                return;
            currentTab = "ValueType";
            ClearPanel2();
            LoadValueTypes();
        }

        private void TabDimensions_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Dimensions")
                return;
            currentTab = "Dimensions";
            ClearPanel2();
            LoadDimensions();
        }

        private void TabCategory_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Category")
                return;
            currentTab = "Category";
            ClearPanel2();
            LoadCategories();
        }

        private void TabSettings_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            SetIconSize();
        }

        private void TabSettings_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Settings")
                return;
            currentTab = "Settings";
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

                            imageList1.Images.Add(Path.GetFileName(GalleryArray[i]), Icon.ExtractAssociatedIcon(GalleryArray[i]));
                        }
                    }
                }
                if (treeDimensions != null)
                {
                    treeDimensions.ItemHeight = iconSize + 2;
                    treeDimensions.Indent = iconSize + 4;
                    treeDimensions.Font = Properties.Settings.Default.TableExplorerFont;
                }
                if (treeCategory != null)
                {
                    treeCategory.ItemHeight = iconSize + 2;
                    treeCategory.Indent = iconSize + 4;
                    treeCategory.Font = Properties.Settings.Default.TableExplorerFont;
                }
                if (treeSegments != null)
                {
                    treeSegments.ItemHeight = iconSize + 2;
                    treeSegments.Indent = iconSize + 4;
                    treeSegments.Font = Properties.Settings.Default.TableExplorerFont;
                }
                if (treeValueType != null)
                {
                    treeValueType.ItemHeight = iconSize + 2;
                    treeValueType.Indent = iconSize + 4;
                    treeValueType.Font = Properties.Settings.Default.TableExplorerFont;
                }
                treeView1.ItemHeight = iconSize + 2;
                treeView1.Indent = iconSize + 4;
                treeView1.Font = Properties.Settings.Default.TableExplorerFont;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }
        }

        private TreeNode CreateTreeNode(string txt)
        {
            TreeNode tn = new TreeNode();
            tn.Name = txt;
            tn.ImageKey = txt + ".ico";
            tn.SelectedImageKey = txt + ".ico";
            return tn;
        }

        private void LoadDimensions()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                if (tabDimensions.Controls.Contains(treeDimensions))
                {
                    treeDimensions.SelectedNodes.Clear();
                    foreach (TreeNode tn in treeDimensions.Nodes)
                        tn.Nodes.Clear();
                }
                else
                {
                    treeDimensions = new TreeViewMS();
                    SetIconSize();
                    treeDimensions.ImageList = imageList1;
                    treeDimensions.CheckBoxes = false;
                    treeDimensions.Dock = DockStyle.Fill;
                    treeDimensions.HideSelection = false;
                    tabDimensions.Controls.Add(treeDimensions);
                    //treeDimensions.AfterCheck += Tree_AfterCheck;
                    treeDimensions.AfterSelect += Tree_AfterSelect;
                    treeDimensions.NodeMouseClick += Tree_NodeMouseClick;
                    treeDimensions.Nodes.Add(CreateTreeNode("1D"));
                    treeDimensions.Nodes.Add(CreateTreeNode("2D"));
                    treeDimensions.Nodes.Add(CreateTreeNode("3D"));
                }

                for (int i = 0; i < filteredTableDatas.Count; i++)
                {
                    //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                    {
                        TreeNode tnChild = new TreeNode(filteredTableDatas[i].TableName);
                        tnChild.Tag = (TableData)filteredTableDatas[i];

                        TableValueType vt = GetTableValueType(filteredTableDatas[i]);
                        if (vt == TableValueType.bitmask)
                        {
                            tnChild.ImageKey = "bitmask.ico";
                            tnChild.SelectedImageKey = "bitmask.ico";
                        }
                        else if (vt == TableValueType.boolean)
                        {
                            tnChild.ImageKey = "boolean.ico";
                            tnChild.SelectedImageKey = "boolean.ico";
                        }
                        else if (vt == TableValueType.selection)
                        {
                            tnChild.ImageKey = "enum.ico";
                            tnChild.SelectedImageKey = "enum.ico";
                        }
                        else if (vt == TableValueType.patch)
                        {
                            tnChild.ImageKey = "patch.ico";
                            tnChild.SelectedImageKey = "patch.ico";
                        }
                        else
                        {
                            tnChild.ImageKey = "number.ico";
                            tnChild.SelectedImageKey = "number.ico";
                        }

                        string nodeKey = "";
                        switch (filteredTableDatas[i].Dimensions())
                        {
                            case 1:
                                nodeKey = "1D";
                                break;
                            case 2:
                                nodeKey = "2D";
                                break;
                            case 3:
                                nodeKey = "3D";
                                break;
                        }

                        if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                        {
                            treeDimensions.Nodes[nodeKey].Nodes.Add(tnChild);
                        }
                        else
                        {
                            string cat = filteredTableDatas[i].Category;
                            if (cat == "")
                                cat = "(Empty)";
                            if (!treeDimensions.Nodes[nodeKey].Nodes.ContainsKey(cat))
                            {
                                TreeNode dimCatTn = new TreeNode(cat);
                                dimCatTn.Name = cat;
                                dimCatTn.ImageKey = "category.ico";
                                dimCatTn.SelectedImageKey = "category.ico";
                                treeDimensions.Nodes[nodeKey].Nodes.Add(dimCatTn);
                            }
                            treeDimensions.Nodes[nodeKey].Nodes[cat].Nodes.Add(tnChild);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                    foreach (TreeNode tn in treeValueType.Nodes)
                        tn.Nodes.Clear();
                }
                else
                {
                    treeValueType = new TreeViewMS();
                    SetIconSize();
                    treeValueType.ImageList = imageList1;
                    treeValueType.CheckBoxes = false;
                    treeValueType.Dock = DockStyle.Fill;
                    treeValueType.HideSelection = false;
                    tabValueType.Controls.Add(treeValueType);
                    treeValueType.AfterSelect += Tree_AfterSelect;
                    treeValueType.AfterCheck += Tree_AfterCheck;
                    treeValueType.NodeMouseClick += Tree_NodeMouseClick;
                    treeValueType.Nodes.Add(CreateTreeNode("number"));
                    treeValueType.Nodes.Add(CreateTreeNode("enum"));
                    treeValueType.Nodes.Add(CreateTreeNode("bitmask"));
                    treeValueType.Nodes.Add(CreateTreeNode("boolean"));
                    treeValueType.Nodes.Add(CreateTreeNode("patch"));
                }

                for (int i = 0; i < filteredTableDatas.Count; i++)
                {
                    //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                    {
                        TreeNode tnChild = new TreeNode(filteredTableDatas[i].TableName);
                        tnChild.Tag = (TableData)filteredTableDatas[i];

                        switch (filteredTableDatas[i].Dimensions())
                        {
                            case 1:
                                tnChild.ImageKey = "1d.ico";
                                tnChild.SelectedImageKey = "1d.ico";
                                break;
                            case 2:
                                tnChild.ImageKey = "2d.ico";
                                tnChild.SelectedImageKey = "2d.ico";
                                break;
                            case 3:
                                tnChild.ImageKey = "3d.ico";
                                tnChild.SelectedImageKey = "3d.ico";
                                break;
                        }

                        TableValueType vt = GetTableValueType(filteredTableDatas[i]);
                        string nodeKey = "";
                        if (vt == TableValueType.bitmask)
                            nodeKey = "bitmask";
                        else if (vt == TableValueType.boolean)
                            nodeKey = "boolean";
                        else if (vt == TableValueType.selection)
                            nodeKey = "enum";
                        else if (vt == TableValueType.patch)
                            nodeKey = "patch";
                        else
                            nodeKey = "number";

                        if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                        {
                            treeValueType.Nodes[nodeKey].Nodes.Add(tnChild);
                        }
                        else
                        {
                            string cat = filteredTableDatas[i].Category;
                            if (cat == "")
                                cat = "(Empty)";
                            if (!treeValueType.Nodes[nodeKey].Nodes.ContainsKey(cat))
                            {
                                TreeNode vtCatTn = new TreeNode(cat);
                                vtCatTn.Name = cat;
                                vtCatTn.ImageKey = "category.ico";
                                vtCatTn.SelectedImageKey = "category.ico";
                                treeValueType.Nodes[nodeKey].Nodes.Add(vtCatTn);
                            }
                            treeValueType.Nodes[nodeKey].Nodes[cat].Nodes.Add(tnChild);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                    SetIconSize();
                    treeCategory.ImageList = imageList1;
                    treeCategory.CheckBoxes = false;
                    treeCategory.HideSelection = false;
                    treeCategory.Dock = DockStyle.Fill;
                    tabCategory.Controls.Add(treeCategory);
                    treeCategory.AfterSelect += Tree_AfterSelect;
                    treeCategory.AfterCheck += Tree_AfterCheck;
                    treeCategory.NodeMouseClick += Tree_NodeMouseClick;
                }
                for (int i = 0; i < filteredTableDatas.Count; i++)
                {
                    //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                    {

                        TreeNode tnChild = new TreeNode(filteredTableDatas[i].TableName);
                        tnChild.Tag = (TableData)filteredTableDatas[i];
                        string ico = "";
                        TableValueType vt = GetTableValueType(filteredTableDatas[i]);
                        if (vt == TableValueType.bitmask)
                        {
                            ico = "mask";
                        }
                        else if (vt == TableValueType.boolean)
                        {
                            ico = "flag";
                        }
                        else if (vt == TableValueType.patch)
                        {
                            ico = "patch";
                        }
                        else if (vt == TableValueType.selection)
                        {
                            ico = "enum";
                        }

                        switch (filteredTableDatas[i].Dimensions())
                        {
                            case 1:
                                ico += "1d.ico";
                                break;
                            case 2:
                                ico += "2d.ico";
                                break;
                            case 3:
                                ico += "3d.ico";
                                break;
                        }
                        tnChild.ImageKey = ico;
                        tnChild.SelectedImageKey = ico;

                        string cat = filteredTableDatas[i].Category;
                        if (cat == "")
                            cat = "(Empty)";
                        if (!treeCategory.Nodes.ContainsKey(cat))
                        {
                            TreeNode cTnChild = new TreeNode(cat);
                            cTnChild.Name = cat;
                            cTnChild.ImageKey = "category.ico";
                            cTnChild.SelectedImageKey = "category.ico";
                            treeCategory.Nodes.Add(cTnChild);
                        }
                        treeCategory.Nodes[cat].Nodes.Add(tnChild);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                    SetIconSize();
                    treeSegments.ImageList = imageList1;
                    treeSegments.CheckBoxes = false;
                    treeSegments.HideSelection = false;
                    treeSegments.Dock = DockStyle.Fill;
                    tabSegments.Controls.Add(treeSegments);
                    treeSegments.AfterSelect += Tree_AfterSelect;
                    treeSegments.AfterCheck += Tree_AfterCheck;
                    treeSegments.NodeMouseClick += Tree_NodeMouseClick;
                }
                TreeNode segTn;
                for (int i = 0; i < PCM.Segments.Count; i++)
                {
                    if (PCM.Segments[i].Missing)
                        continue;
                    segTn = new TreeNode(PCM.Segments[i].Name);
                    segTn.Name = PCM.Segments[i].Name;
                    segTn.ImageKey = "segments.ico";
                    segTn.SelectedImageKey = "segments.ico";

                    bool found = false;
                    foreach (string icofile in GalleryArray)
                    {

                        double percentage = ComputeSimilarity.CalculateSimilarity(Path.GetFileNameWithoutExtension(icofile).ToLower(), segTn.Name.ToLower());
                        if ((int)(percentage * 100) >= 80)
                        {
                            segTn.ImageKey = Path.GetFileName(icofile);
                            segTn.SelectedImageKey = Path.GetFileName(icofile);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (string icofile in GalleryArray)
                        {
                            if (segTn.Name.ToLower().Contains(Path.GetFileNameWithoutExtension(icofile)))
                            {
                                segTn.ImageKey = Path.GetFileName(icofile);
                                segTn.SelectedImageKey = Path.GetFileName(icofile);
                                found = true;
                                break;
                            }
                        }
                    }
                    treeSegments.Nodes.Add(segTn);
                }

                for (int i = 0; i < filteredTableDatas.Count; i++)
                {
                    //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                    {

                        TreeNode tnChild = new TreeNode(filteredTableDatas[i].TableName);
                        tnChild.Tag = (TableData)filteredTableDatas[i];
                        string ico = "";
                        TableValueType vt = GetTableValueType(filteredTableDatas[i]);
                        if (vt == TableValueType.bitmask)
                        {
                            ico = "mask";
                        }
                        else if (vt == TableValueType.boolean)
                        {
                            ico = "flag";
                        }
                        else if (vt == TableValueType.patch)
                        {
                            ico = "patch";
                        }
                        else if (vt == TableValueType.selection)
                        {
                            ico = "enum";
                        }

                        switch (filteredTableDatas[i].Dimensions())
                        {
                            case 1:
                                ico += "1d.ico";
                                break;
                            case 2:
                                ico += "2d.ico";
                                break;
                            case 3:
                                ico += "3d.ico";
                                break;
                        }

                        tnChild.ImageKey = ico;
                        tnChild.SelectedImageKey = ico;

                        string cat = filteredTableDatas[i].Category;
                        if (cat == "")
                            cat = "(Empty)";

                        int seg = PCM.GetSegmentNumber(filteredTableDatas[i].addrInt);
                        if (seg > -1)
                        {
                            if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                            {
                                treeSegments.Nodes[seg].Nodes.Add(tnChild);
                            }
                            else
                            {
                                if (!treeSegments.Nodes[seg].Nodes.ContainsKey(cat))
                                {
                                    TreeNode tnNew = new TreeNode(cat);
                                    tnNew.Name = cat;
                                    tnNew.ImageKey = "category.ico";
                                    tnNew.SelectedImageKey = "category.ico";
                                    treeSegments.Nodes[seg].Nodes.Add(tnNew);
                                }
                                treeSegments.Nodes[seg].Nodes[cat].Nodes.Add(tnChild);
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                Properties.Settings.Default.TunerTreeMode = radioTreeMode.Checked;
                Properties.Settings.Default.Save();
                FilterTables();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }


        private void radioTreeMode_CheckedChanged(object sender, EventArgs e)
        {
            radioListMode.Checked = !radioTreeMode.Checked;
            SelectDispMode();
        }

        private void radioListMode_CheckedChanged(object sender, EventArgs e)
        {
            radioTreeMode.Checked = !radioListMode.Checked;
            SelectDispMode();
        }

        private void chkShowCategorySubfolder_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TableExplorerUseCategorySubfolder = chkShowCategorySubfolder.Checked;
            Properties.Settings.Default.Save();
            FilterTree();
        }

        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TableData> tableTds = GetSelectedTableTds();
            OpenTableEditor(tableTds, true);
        }

        private void chkAutoMulti1d_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TunerAutomulti1d = chkAutoMulti1d.Checked;
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
            catch (Exception ex) { LoggerBold(ex.Message); }

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
                LoggerBold(ex.Message);
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
                if (fif.ShowDialog() == DialogResult.OK && Properties.Settings.Default.AutomaticOpenImportedFile)
                {
                    List<string> fList = new List<string>();
                    fList.Add(fif.outFileName);
                    OpenNewBinFile(false, fList);
                }
                return;

                //Old code:
                Logger("Reading file: " + fileName, false);
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
                if (Properties.Settings.Default.AutomaticOpenImportedFile)
                {
                    List<string> fList = new List<string>();
                    fList.Add(fileName);
                    OpenNewBinFile(false, fList);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
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
                if (fif.ShowDialog() == DialogResult.OK && Properties.Settings.Default.AutomaticOpenImportedFile)
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
                if (fif.ShowDialog() == DialogResult.OK && Properties.Settings.Default.AutomaticOpenImportedFile)
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
            OpenTableEditor(hTds, true);
        }

        private void EditAxisTable(string header)
        {
            try
            {
                TableData newTd = PCM.GetTdbyHeader(header).ShallowCopy(false);
                frmTdEditor fte = new frmTdEditor();
                fte.td = newTd;
                fte.LoadTd();
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    lastSelectTd = fte.td;
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas[id] = fte.td;
                            FilterTables();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                if (fte.ShowDialog() == DialogResult.OK)
                {
                    for (int id = 0; id < PCM.tableDatas.Count; id++)
                    {
                        if (PCM.tableDatas[id].guid == lastSelectTd.guid)
                        {
                            PCM.tableDatas[id] = fte.td;
                            FilterTables();
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                return;
            frmHistogram fh = new frmHistogram();
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
        }

        private void touristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkingMode = 0;
            Properties.Settings.Default.Save();
            SetWorkingMode();
        }

        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkingMode = 1;
            Properties.Settings.Default.Save();
            SetWorkingMode();
        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkingMode = 2;
            Properties.Settings.Default.Save();
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
    }
}

