using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmTuner : Form
    {
        public frmTuner(PcmFile PCM1)
        {
            InitializeComponent();

            contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);

            enableConfigModeToolStripMenuItem.Checked = Properties.Settings.Default.TunerConfigMode;

            PCM = PCM1;
            //tableDataList = PCM.tableDatas;
            if (PCM == null || PCM1.fsize == 0) return; //No file selected
            addtoCurrentFileMenu(PCM);
            loadConfigforPCM(ref PCM);
            selectPCM();
        }

        public PcmFile PCM;
        //private List<TableData> tableDataList;
        private string sortBy = "id";
        private int sortIndex = 0;
        private bool columnsModified = false;
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        private BindingList<TableData> filteredCategories = new BindingList<TableData>();
        SortOrder strSortOrder = SortOrder.Ascending;
        private int lastSelectedId;
        string compXml = "";
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
            enableConfigModeToolStripMenuItem.Checked = Properties.Settings.Default.TunerConfigMode;
            insertRowToolStripMenuItem.Enabled = Properties.Settings.Default.TunerConfigMode;
            deleteRowToolStripMenuItem.Enabled = Properties.Settings.Default.TunerConfigMode;
            editRowToolStripMenuItem.Enabled = Properties.Settings.Default.TunerConfigMode;
            duplicateTableConfigToolStripMenuItem.Enabled = Properties.Settings.Default.TunerConfigMode;
            disableConfigAutoloadToolStripMenuItem.Checked = Properties.Settings.Default.disableTunerAutoloadSettings;
            chkShowCategorySubfolder.Checked = Properties.Settings.Default.TableExplorerUseCategorySubfolder;
            chkAutoMulti1d.Checked = Properties.Settings.Default.TunerAutomulti1d;

            radioTreeMode.Checked = Properties.Settings.Default.TunerTreeMode;
            radioListMode.Checked = !Properties.Settings.Default.TunerTreeMode;
            selectDispMode();

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

            }

            comboFilterBy.Items.Clear();
            TableData tdTmp = new TableData();
            foreach (var prop in tdTmp.GetType().GetProperties())
            {
                //Add to filter by-combo
                comboFilterBy.Items.Add(prop.Name);
                if (prop.Name != "id")
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem(prop.Name);
                    menuItem.Name = prop.Name;
                    contextMenuStrip2.Items.Add(menuItem);
                    menuItem.Click += new EventHandler(columnSelection_Click);
                }
            }
            comboFilterBy.Text = "TableName";
            labelTableName.Text = "";
            lastSelectedId = -1;
            dataGridView1.DataSource = bindingsource;
            filterTables();
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
            }
            Size logSize = new Size();
            logSize.Width = this.splitContainer2.SplitterDistance;
            logSize.Height = this.splitContainer1.SplitterDistance;
            Properties.Settings.Default.TunerLogWindowSize = logSize;
            Properties.Settings.Default.TunerConfigMode = enableConfigModeToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        public void selectPCM()
        {
            this.Text = "Tuner " + PCM.FileName + " [" + PCM.tunerFile + "]";
            PCM.selectTableDatas(0, PCM.FileName);
            //tableDataList = PCM.tableDatas;
            for (int m= tableListToolStripMenuItem.DropDownItems.Count - 1; m >=0; m-- )
            { 

                if (tableListToolStripMenuItem.DropDownItems[m].Tag != null)
                    tableListToolStripMenuItem.DropDownItems.RemoveAt(m);
            }
            for (int i=0; i< PCM.altTableDatas.Count; i++)
            {
                ToolStripMenuItem miNew = new ToolStripMenuItem(PCM.altTableDatas[i].Name);
                miNew.Name = PCM.altTableDatas[i].Name;
                miNew.Tag = i;
                if (i == 0)
                    miNew.Checked = true;
                tableListToolStripMenuItem.DropDownItems.Add(miNew);
                miNew.Click += tablelistSelect_Click;
            }
            if (PCM.Segments.Count > 0 &&  PCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                tinyTunerDBV6OnlyToolStripMenuItem.Enabled = true;
            else
                tinyTunerDBV6OnlyToolStripMenuItem.Enabled = false;
            filterTables();
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
            currentBin = mitem.Text.Substring(0, 1);
        }

        public void loadConfigforPCM(ref PcmFile newPCM)
        {
            if (!Properties.Settings.Default.disableTunerAutoloadSettings)
            {
                string defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", newPCM.OS + ".xml");
                if (newPCM.OS.Length == 0)
                    defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", newPCM.configFile + "-def.xml");
                compXml = "";
                if (File.Exists(defaultTunerFile))
                {
                    long conFileSize = new FileInfo(defaultTunerFile).Length;
                    if (conFileSize < 255)
                    {
                        compXml = ReadTextFile(defaultTunerFile);
                        defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", compXml);
                        Logger("Using compatible file: " + compXml);
                    }
                }
                if (File.Exists(defaultTunerFile))
                {
                    Logger(newPCM.LoadTableList(defaultTunerFile));
                    importDTC(ref newPCM);
                    refreshTablelist();
                }
                else
                {
                    defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", newPCM.configFile + "-def.xml");
                    if (File.Exists(defaultTunerFile))
                    {
                        Logger(newPCM.LoadTableList(defaultTunerFile));
                        importDTC(ref newPCM);
                        refreshTablelist();
                    }
                    else
                    {
                        Logger("File not found: " + defaultTunerFile);
                        importDTC(ref newPCM);
                        importTableSeek(ref newPCM);
                        if (newPCM.Segments.Count > 0 && newPCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                            importTinyTunerDB(ref newPCM);
                    }
                }
                this.Text = "Tuner: " + newPCM.FileName + " [" + newPCM.tunerFile + "]";

            }

        }

        public void openTableEditor(List<int> tableIds = null, bool newWindow = false)
        {
            try
            {
                if (tableIds == null)
                    tableIds = new List<int>();
                if (tableIds.Count == 0)
                {
                    tableIds = getSelectedTableIds();
                }
                TableData td = PCM.tableDatas[tableIds[0]];
                if (td.addrInt == uint.MaxValue)
                {
                    Logger("No address defined!");
                    return;
                }

                if (td.OS != PCM.OS && !td.CompatibleOS.Contains("," + PCM.OS +","))
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
                frmT.prepareTable(PCM, td, tableIds, currentBin);
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                {
                    PcmFile comparePCM = (PcmFile)mi.Tag;
                    if (PCM.FileName != comparePCM.FileName)
                    {
                        Logger("Adding file: " + Path.GetFileName(comparePCM.FileName) + " to compare menu... ", false);
                        if (PCM.OS == comparePCM.OS)
                        {
                            frmT.addCompareFiletoMenu(comparePCM, td, mi.Text);
                        }
                        else
                        {
                            int x = -1;
                            for (int y = 0; y < tableIds.Count; y++)
                            {
                                TableData tmpTd = PCM.tableDatas[tableIds[y]];
                                x = findTableDataId(tmpTd, comparePCM.tableDatas);
                                if (x > -1)
                                    break;
                            }
                            if (x < 0)
                            {
                                LoggerBold("Table not found");
                            }
                            else
                            {
                                frmT.addCompareFiletoMenu(comparePCM, comparePCM.tableDatas[x], mi.Text);
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
                frmT.loadTable();                
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
                string fileName = SelectSaveFile("BIN files (*.bin)|*.bin|ALL files(*.*)|*.*",PCM.FileName);
                if (fileName.Length == 0)
                    return;

                Logger("Saving to file: " + fileName);
                PCM.saveBin(fileName);
                this.Text = "Tuner " + Path.GetFileName(fileName) + " [" + PCM.tunerFile +"]";
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
        public void refreshTablelist()
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
            filterTables();
        }

        private void importTableSeek(ref PcmFile _PCM)
        {
            Logger("Importing tableseek...");
            Application.DoEvents();
            _PCM.importSeekTables();
            refreshTablelist();
            Logger("OK");
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            Logger(PCM.LoadTableList());
            refreshTablelist();
        }

        private void SaveTableList(string fName ="")
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                if (PCM.OS.Length == 0)
                    defName = Path.Combine(Application.StartupPath, "Tuner", PCM.configFile + "-def.xml");
                if (compXml.Length > 0)
                    defName = Path.Combine(Application.StartupPath, "Tuner", compXml);
                if (fName.Length == 0)
                    fName = SelectSaveFile("XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
                if (fName.Length == 0)
                    return;
                dataGridView1.EndEdit();
                Logger("Saving file " + fName + "...", false);
                PCM.SaveTableList(fName);
                Logger(" [OK]");
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

        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            SaveTableList();
        }

        private void btnImportDTC_Click(object sender, EventArgs e)
        {
            importDTC(ref PCM);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            PCM.tableDatas = new List<TableData>();
            refreshTablelist();
        }

        private void reorderColumns()
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
                for (int c = 0; c < dataGridView1.Columns.Count; c++)
                {
                    if (enableConfigModeToolStripMenuItem.Checked)
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
                dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;

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
        private void filterTables()
        {
            try
            {
                this.dataGridView1.SelectionChanged -= new System.EventHandler(this.DataGridView1_SelectionChanged);

                //if (PCM == null || PCM.fsize == 0) return;
                if (PCM == null || PCM.tableDatas == null)
                    return;
                //Save settings before reordering
                saveGridLayout();
                //Fix table-ID's
                for (int tbId = 0; tbId < PCM.tableDatas.Count; tbId++)
                    PCM.tableDatas[tbId].id = (uint)tbId;

                List<TableData> compareList = new List<TableData>();
                if (strSortOrder == SortOrder.Ascending)
                    compareList = PCM.tableDatas.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    compareList = PCM.tableDatas.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();

                var results = compareList.Where(t => t.id < uint.MaxValue); //How should I define empty variable??

                if (txtSearchTableSeek.Text.Length > 0)
                {
                    string newStr = txtSearchTableSeek.Text.Replace("OR", "|");
                    if (newStr.Contains("|"))
                    {
                        if (caseSensitiveFilteringToolStripMenuItem.Checked)
                        {
                            string[] orStr = newStr.Split('|');
                            if (orStr.Length == 2)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[0].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[1].Trim()));
                            if (orStr.Length == 3)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[0].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[1].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[2].Trim()));
                            if (orStr.Length == 4)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[0].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[1].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[2].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(orStr[3].Trim()));
                        }
                        else
                        {
                            string[] orStr = newStr.ToLower().Split('|');
                            if (orStr.Length == 2)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[0].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[1].Trim()));
                            if (orStr.Length == 3)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[0].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[1].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[2].Trim()));
                            if (orStr.Length == 4)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[0].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[1].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[2].Trim()) ||
                                typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[3].Trim()));

                        }
                    }
                    else
                    {
                        newStr = txtSearchTableSeek.Text.Replace("AND", "&");
                        string[] andStr = newStr.Split('&');
                        foreach (string sStr in andStr)
                        {
                            if (caseSensitiveFilteringToolStripMenuItem.Checked)
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().Contains(sStr.Trim()));
                            else
                                results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(sStr.ToLower().Trim()));
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
                filteredCategories = new BindingList<TableData>(results.ToList());
                bindingsource.DataSource = filteredCategories;
                reorderColumns();
                txtDescription.Text = "";
                filterTree();
                this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void filterTree()
        {
            clearPanel2();
            if (tabControl1.SelectedTab.Name == "tabDimensions")
                loadDimensions();
            else if (tabControl1.SelectedTab.Name == "tabValueType")
                loadValueTypes();
            else if (tabControl1.SelectedTab.Name == "tabCategory")
                loadCategories();
            else if (tabControl1.SelectedTab.Name == "tabSegments")
                loadSegments();
        }
        private void comboTableCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("comboTableCategory_SelectedIndexChanged");
            filterTables();
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

        private void importTinyTunerDB(ref PcmFile _PCM)
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDBtoTableData(_PCM, _PCM.tableDatas));
            refreshTablelist();

        }

        private void btnReadTinyTunerDB_Click(object sender, EventArgs e)
        {
            importTinyTunerDB(ref PCM);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger(PCM.LoadTableList());
            comboTableCategory.Text = "_All";
            refreshTablelist();
            //currentXmlFile = PCM.configFileFullName;
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTableList(PCM.tunerFile);
        }

        private void saveBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            PCM.saveBin(PCM.FileName);
            Logger("Done.");

        }

        private void importDTC(ref PcmFile _PCM)
        {
            Logger("Importing DTC codes... ", false);
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
                tdTmp.importDTC(_PCM, ref _PCM.tableDatas);
                Logger(" [OK]");
                filterTables();
            }
        }

        private void clearTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.tableDatas = new List<TableData>();
            PCM.tableCategories = new List<string>();
            refreshTablelist();
        }


        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {

                if (dataGridView1.SelectedCells.Count > 0 && e.Button == MouseButtons.Right)
                {
                    lastSelectedId = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
                    contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
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
                DataGridView.HitTestInfo  hti = g.HitTest(p.X, p.Y);
                if (hti.Type != DataGridViewHitTestType.ColumnHeader && hti.Type != DataGridViewHitTestType.RowHeader)
                    openTableEditor();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Copy to clipboard
            CopyToClipboard();

            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                dgvCell.Value = string.Empty;
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
            List<int> tableIds = getSelectedTableIds();            
            openTableEditor(tableIds,true);
        }

        private void exportCSV()
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
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
        private void exportCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void importxperimentalCSV()
        {
            for (int i = 0; i < PCM.tableDatas.Count; i++)
                PCM.tableDatas[i].addrInt = uint.MaxValue;
            string fileName = SelectFile("Select CSV File", "CSV files (*.csv)|*.csv|All files (*.*)|*.*");
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
            refreshTablelist();

        }
        private void importCSVexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exportXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            filterTables();
        }

        private void showFileInfo(PcmFile pcm, RichTextBox txtBox)
        {
            txtBox.SelectionFont = new Font(txtBox.Font, FontStyle.Bold);
            txtBox.AppendText(pcm.FileName + Environment.NewLine);
            txtBox.SelectionFont = new Font(txtBox.Font, FontStyle.Regular);
            for (int i = 0; i < pcm.Segments.Count; i++)
            {
                SegmentConfig S = pcm.Segments[i];
                txtBox.AppendText( " " + pcm.segmentinfos[i].Name.PadRight(11));
                if (pcm.segmentinfos[i].PN.Length > 1)
                {
                    if (pcm.segmentinfos[i].Stock == "[stock]")
                        txtBox.AppendText(" PN: " + pcm.segmentinfos[i].PN.PadRight(9));
                    else
                        txtBox.AppendText(" PN: " + pcm.segmentinfos[i].PN.PadRight(9));
                }
                if (pcm.segmentinfos[i].Ver.Length > 1)
                    txtBox.AppendText(", Ver: " + pcm.segmentinfos[i].Ver);

                if (pcm.segmentinfos[i].SegNr.Length > 0)
                    txtBox.AppendText(", Nr: " + pcm.segmentinfos[i].SegNr.PadRight(3));
                    txtBox.AppendText("[" + pcm.segmentinfos[i].Address + "]");
                    txtBox.AppendText(", Size: " + pcm.segmentinfos[i].Size.ToString());
                if (pcm.segmentinfos[i].ExtraInfo != null && pcm.segmentinfos[i].ExtraInfo.Length > 0)
                    txtBox.AppendText(Environment.NewLine + pcm.segmentinfos[i].ExtraInfo);
                txtBox.AppendText(Environment.NewLine);
            }

        }

        private void importXperimentalCsv2()
        {
            string FileName = SelectFile("Select CSV File", "CSV files (*.csv)|*.csv|All files (*.*)|*.*");
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
            refreshTablelist();

        }
        private void importCSV2ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!e.Exception.Message.Contains("DataGridViewComboBoxCell"))
                Debug.WriteLine(e.Exception);
        }

        private void comboFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("comboFilterBy_SelectedIndexChanged");
            filterTables();
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
                //saveGridLayout(); //Save before reorder!
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = getSortOrder(sortIndex);
                filterTables();
            }
            else if (!enableConfigModeToolStripMenuItem.Checked)
            {
                contextMenuStrip2.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private SortOrder getSortOrder(int columnIndex)
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
            for (int i=0;i<PCM.tableDatas.Count; i++)
            {
                if (PCM.tableDatas[i].TableName.ToLower().StartsWith("ka_") || PCM.tableDatas[i].TableName.ToLower().StartsWith("ke_") || PCM.tableDatas[i].TableName.ToLower().StartsWith("kv_"))
                    PCM.tableDatas[i].TableName = PCM.tableDatas[i].TableName.Substring(3);
            }
            refreshTablelist();
        }

        private void DataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!enableConfigModeToolStripMenuItem.Checked)
            {
                e.Cancel = true;
                return;
            }
            for (int r=0; r< dataGridView1.SelectedRows.Count; r++)
            {
                int row = dataGridView1.SelectedRows[r].Index;
                int id = Convert.ToInt32(dataGridView1.Rows[row].Cells["id"].Value);
                PCM.tableDatas.RemoveAt(id);
                filterTables();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void peekTableValuesWithCompare(int ind)
        {
            int myInd = findTableDataId(PCM.tableDatas[ind], PCM.tableDatas);
            if (myInd == -1)
            {
                LoggerBold("Table missing: " + PCM.tableDatas[ind].TableName);
                return;
            }
            peekTableValues(myInd, PCM); //Show values from current file 
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
            {
                PcmFile peekPCM = (PcmFile)mi.Tag;
                if (peekPCM.FileName != PCM.FileName)
                {
                    myInd = findTableDataId(PCM.tableDatas[ind], peekPCM.tableDatas);
                    if (myInd > -1)
                    {
                        txtDescription.AppendText(peekPCM.FileName + ": [" + peekPCM.tableDatas[myInd].TableName + "]" + Environment.NewLine);
                        peekTableValues(myInd, peekPCM);
                    }
                }
            }
        }

        private void peekTableValues(int ind, PcmFile peekPCM)
        {
            try
            {
                if (peekPCM.tableDatas[ind].addrInt >= peekPCM.fsize)
                {
                    Debug.WriteLine("No address defined");
                    return;
                }
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
                txtDescription.SelectionColor = Color.Blue;
                string minMax = " [";
                if (peekPCM.tableDatas[ind].Min > double.MinValue)
                    minMax += " Min: " + peekPCM.tableDatas[ind].Min.ToString();
                if (peekPCM.tableDatas[ind].Max < double.MaxValue)
                    minMax += " Max: " + peekPCM.tableDatas[ind].Max.ToString();
                if (minMax == " [")
                    minMax = "";
                else
                    minMax += "] ";
                if (peekPCM.tableDatas[ind].Dimensions() == 1)
                {
                    double curVal = getValue(peekPCM.buf, (uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset), peekPCM.tableDatas[ind],0, peekPCM);
                    UInt64 rawVal = (UInt64) getRawValue(peekPCM.buf, (uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset), peekPCM.tableDatas[ind],0);
                    string valTxt = curVal.ToString();
                    string unitTxt = " " + peekPCM.tableDatas[ind].Units;
                    string maskTxt = "";
                    TableValueType vt = getValueType(peekPCM.tableDatas[ind]);
                    if (vt == TableValueType.boolean)
                    {
                        if (peekPCM.tableDatas[ind].BitMask != null && peekPCM.tableDatas[ind].BitMask.Length > 0)
                        {
                            unitTxt = "";
                            UInt64 maskVal = Convert.ToUInt64(peekPCM.tableDatas[ind].BitMask.Replace("0x", ""), 16);
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
                                rawBinVal = rawBinVal.PadLeft(getBits(peekPCM.tableDatas[ind].DataType), '0');
                                maskTxt = " [" + rawBinVal + "], bit $" + bit.ToString();
                            }
                        }
                        else
                        {
                            unitTxt = ", Unset/Set";
                            if (curVal > 0)
                                valTxt = "Set, " + valTxt;
                            else
                                valTxt = "Unset, " + valTxt;
                        }
                    }
                    else if (vt == TableValueType.selection)
                    {
                        Dictionary<double, string> possibleVals = parseEnumHeaders(peekPCM.tableDatas[ind].Values.Replace("Enum: ", ""));
                        if (possibleVals.ContainsKey(curVal))
                            unitTxt = " (" + possibleVals[curVal] + ")";
                        else
                            unitTxt = " (Out of range)";
                    }
                    string formatStr = "X" + (getElementSize(peekPCM.tableDatas[ind].DataType) * 2).ToString();
                    string rawTxt = "";
                    switch (peekPCM.tableDatas[ind].DataType)
                    {
                        case InDataType.FLOAT32:
                            rawTxt = ((Single)rawVal).ToString(formatStr);
                            break;
                        case InDataType.FLOAT64:
                            rawTxt = ((double)rawVal).ToString(formatStr);
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
                    string tblData = "Current values: " + minMax + Environment.NewLine;
                    uint addr = (uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset);
                    if (peekPCM.tableDatas[ind].RowMajor)
                    {
                        for (int r = 0; r < peekPCM.tableDatas[ind].Rows; r++)
                        {
                            for (int c = 0; c < peekPCM.tableDatas[ind].Columns; c++)
                            {
                                double curVal = getValue(peekPCM.buf, addr, peekPCM.tableDatas[ind],0, peekPCM);
                                addr += (uint)getElementSize(peekPCM.tableDatas[ind].DataType);
                                tblData += "[" + curVal.ToString("#0.0") + "]";
                            }
                            tblData += Environment.NewLine;
                        }
                    }
                    else
                    {
                        List<string> tblRows = new List<string>();
                        for (int r = 0; r < peekPCM.tableDatas[ind].Rows; r++)
                            tblRows.Add("");
                        for (int c = 0; c < peekPCM.tableDatas[ind].Columns; c++)
                        {

                            for (int r = 0; r < peekPCM.tableDatas[ind].Rows; r++)
                            {
                                double curVal = getValue(peekPCM.buf, addr, peekPCM.tableDatas[ind],0, peekPCM);
                                addr += (uint)getElementSize(peekPCM.tableDatas[ind].DataType);
                                tblRows[r] += "[" + curVal.ToString("#0.0") + "]";
                            }
                        }
                        for (int r = 0; r < peekPCM.tableDatas[ind].Rows; r++)
                            tblData += tblRows[r] + Environment.NewLine;
                    }
                    txtDescription.AppendText(tblData);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void showTableDescription(PcmFile PCM, int ind = -1)
        {
            txtDescription.Text = "";
            if ((ind == -1 && dataGridView1.SelectedCells.Count < 1) || PCM.tableDatas.Count == 0)
            {
                return;
            }
            if (ind < 0)
                ind = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Bold);
            txtDescription.AppendText(PCM.tableDatas[ind].TableName + Environment.NewLine);
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
            if (PCM.tableDatas[ind].TableDescription != null)
                txtDescription.AppendText(PCM.tableDatas[ind].TableDescription + Environment.NewLine);
            if (PCM.tableDatas[ind].ExtraDescription != null)
                txtDescription.AppendText(PCM.tableDatas[ind].ExtraDescription + Environment.NewLine);

            peekTableValuesWithCompare(ind);
            labelTableName.Text = PCM.tableDatas[ind].TableName;
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            showTableDescription(PCM);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void setConfigMode()
        {
            cSVexperimentalToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;
            cSV2ExperimentalToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;
            xMLGeneratorExportToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;
            xMlgeneratorImportCSVToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;     
        }
        private void enableConfigModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableConfigModeToolStripMenuItem.Checked = !enableConfigModeToolStripMenuItem.Checked;
            insertRowToolStripMenuItem.Enabled = enableConfigModeToolStripMenuItem.Checked;
            deleteRowToolStripMenuItem.Enabled = enableConfigModeToolStripMenuItem.Checked;
            editRowToolStripMenuItem.Enabled = enableConfigModeToolStripMenuItem.Checked;
            duplicateTableConfigToolStripMenuItem.Enabled = enableConfigModeToolStripMenuItem.Checked;
            filterTables();
            setConfigMode();
            Application.DoEvents();
            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
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
            filterTables();
        }

        private void exportXMLgeneratorCSV()
        {
            try
            {
                string fName = SelectSaveFile("CSV files (*.csv)|*.csv|ALL files|*.*");
                if (fName.Length == 0) return;

                Logger("Generating CSV...");

                string csvData = "Category;Tablename;Size;" + PCM.tableDatas[0].OS + ";" + Environment.NewLine;
                for (int row = 0; row < PCM.tableDatas.Count; row++)
                {
                    int tbSize = PCM.tableDatas[row].Rows * PCM.tableDatas[row].Columns * getElementSize(PCM.tableDatas[row].DataType);
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

        private void importXMLGeneratorCsv()
        {
            try
            {
                List<OsAddrStruct> osAddrList = new List<OsAddrStruct>();
                List<string> osList = new List<string>();

                LoggerBold("Supply CSV file in format: Category;Tablename;Size;OS1Address1;OS2Address2;...");
                LoggerBold("OS versions in header, for example: Category;Tablename;Size;12587603;12582405;12587656;");
                string fName = SelectFile("Select CSV file for XML generator", "CSV files (*.csv)|*.csv|ALL files|*.*");
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
                                    TableData newTd = PCM.tableDatas[t].ShallowCopy();
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
            SaveTableList();
        }

        private void configModeColumnOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmData frmd = new frmData();
            frmd.Text = "Column order in config mode:";
            frmd.txtData.Text = Properties.Settings.Default.ConfigModeColumnOrder;
            if (frmd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.ConfigModeColumnOrder = frmd.txtData.Text;
            }
            Properties.Settings.Default.Save();
            frmd.Dispose();
            filterTables();
        }
        
        private struct DisplayOrder
        {
            public int index;
            public string columnName;
        }
        private void DataGridView1_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            Debug.WriteLine("Displayindex: " + e.Column.HeaderText);
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
                Debug.WriteLine("Disabling event");
                this.dataGridView1.ColumnDisplayIndexChanged -= new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView1_ColumnDisplayIndexChanged);
                this.dataGridView1.ColumnWidthChanged -= new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGridView1_ColumnWidthChanged);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void saveGridLayout()
        {
            if (!columnsModified) return;

            if (dataGridView1.Columns.Count < 2) return;
            string columnWidth = "";
            int maxDispIndex = 0;
            List<DisplayOrder> displayOrder = new List<DisplayOrder>();
            for (int c = 0; c < dataGridView1.Columns.Count; c++)
            {
                columnWidth += dataGridView1.Columns[c].Width.ToString() + ","; 
                if (dataGridView1.Columns[c].Visible && dataGridView1.Columns[c].Name != "id")
                {
                    DisplayOrder dispO = new DisplayOrder();
                    dispO.columnName = dataGridView1.Columns[c].Name;
                    dispO.index = dataGridView1.Columns[c].DisplayIndex;
                    displayOrder.Add(dispO);
                    if (dispO.index > maxDispIndex)
                        maxDispIndex = dispO.index;
                }
            }


            string order = "id,";
            for (int i = 0; i <= maxDispIndex; i++)
            {
                for (int j = 0; j < displayOrder.Count; j++)
                {
                    if (displayOrder[j].index == i)
                    {
                        order += displayOrder[j].columnName + ",";
                        break;
                    }
                }
            }
            Debug.WriteLine("Display order: " + order);
            Debug.WriteLine("Column width: " + columnWidth);
            if (enableConfigModeToolStripMenuItem.Checked)
            {
                //Config mode
                Properties.Settings.Default.ConfigModeColumnOrder = order.Trim(',');
                Properties.Settings.Default.ConfigModeColumnWidth = columnWidth.Trim(',');
            }
            else
            {
                //Tuner mode
                Properties.Settings.Default.TunerModeColumns = order.Trim(',');
                Properties.Settings.Default.TunerModeColumnWidth = columnWidth.Trim(',');
            }
            Properties.Settings.Default.Save();
            columnsModified = false;
        }

       
        void cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
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
            if (!enableConfigModeToolStripMenuItem.Checked)
                filterTables();
        }

        private void resetTunerModeColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TunerModeColumns = "id,TableName,Category,Units,Columns,Rows,TableDescription";
            Properties.Settings.Default.TunerModeColumnWidth = "35,100,237,135,100,100,100,100,100,114,100,100,100,100,60,46,100,100,100,100,100,493,100,";           
            Properties.Settings.Default.Save();
            filterTables();
        }

        private void openNewBinFile()
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) return;
            PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
            addtoCurrentFileMenu(newPCM);
            PCM = newPCM;
            loadConfigforPCM(ref PCM);
            selectPCM();
        }
        private void loadBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openNewBinFile();
        }

        public void addtoCurrentFileMenu(PcmFile newPCM, bool setdefault = true)
        {
            if (setdefault)
                foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                    mi.Checked = false;

            char lastFile = 'A';
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                lastFile++;

            ToolStripMenuItem menuitem = new ToolStripMenuItem(newPCM.FileName);
            menuitem.Text = lastFile.ToString() + ": " + newPCM.FileName;
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

            updateFileInfoTab();
        }

        private void CmpHexMenuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            PcmFile cmpWithPcm = (PcmFile)menuitem.Tag;
            findTableDifferencesHEX(cmpWithPcm);
        }

        private void Menuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            bool isChecked = menuitem.Checked;
//            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
//                mi.Checked = false;
            menuitem.Checked = !isChecked;
            PCM = (PcmFile)menuitem.Tag;
            selectPCM();
        }

        private void compareWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void compareMenuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            PcmFile cmpWithPcm = (PcmFile)menuitem.Tag;
            findTableDifferences(cmpWithPcm);
        }

        int diffMissingTables;
        private int compareTableHEX(int tInd,PcmFile pcm1, PcmFile pcm2)
        {
            TableData td1 = pcm1.tableDatas[tInd];
            TableData td2 = td1.ShallowCopy();
            int tbSize = td1.Rows * td1.Columns * getElementSize(td1.DataType);
            int cmpId = tInd;
            if (pcm1.OS != pcm2.OS && !td1.CompatibleOS.Contains("," + pcm2.OS + ","))
            {
                //Not 100% compatible file, find table by name & category
                cmpId = findTableDataId(td1, pcm2.tableDatas);
                if (cmpId < 0)
                {
                    //Logger("Table not found: " + td1.TableName + "[" + pcm2.FileName + "]");
                    diffMissingTables++;
                    return -1;    //Don't add to list if not in both files
                }
                td2 = pcm2.tableDatas[cmpId];
                int tb2size = td2.Rows * td2.Columns * getElementSize(td2.DataType);
                if (tbSize != tb2size)
                    return -1;
            }
            if ((td1.addrInt + tbSize) > pcm1.fsize || (td2.addrInt + tbSize) > pcm2.fsize)
            {
                LoggerBold("Table address out of range: " + td1.TableName);
                return -1;
            }

            if (td1.BitMask != null && td1.BitMask.Length > 0)
            {
                //Check only bit
                double orgVal = getValue(pcm1.buf,td1.addrInt,td1 ,(uint)td1.Offset,pcm1);
                double compVal = getValue(pcm2.buf, td2.addrInt,td2,(uint)td2.Offset, pcm2);
                if (orgVal == compVal)
                    return -1;
                else
                    return cmpId;
            }
            byte[] buff1 = new byte[tbSize];
            byte[] buff2 = new byte[tbSize];
            Array.Copy(pcm1.buf, td1.addrInt + td1.Offset, buff1, 0, tbSize);
            Array.Copy(pcm2.buf, td2.addrInt + td2.Offset, buff2, 0, tbSize);
            if (buff1.SequenceEqual(buff2))
                return -1;
            else
                return cmpId;   //Found table with different data

        }

        private void findTableDifferences(PcmFile cmpWithPcm)
        {
            Logger("Finding tables with different data");
            cmpWithPcm.selectTableDatas(0, "");
            List<int> diffTableDatas = new List<int>();
            List<int> cmpTableDatas = new List<int>();
            for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
            {
                if (PCM.tableDatas[t1].addrInt < PCM.fsize)
                {
                    int cmpId = findTableDataId(PCM.tableDatas[t1], cmpWithPcm.tableDatas);
                    if (cmpId > -1)
                    {
                        if (!compareTables(t1, cmpId, PCM, cmpWithPcm))
                        {
                            diffTableDatas.Add(t1);
                            cmpTableDatas.Add(cmpId);
                        }
                    }
                }
            }
            Logger(" [OK]");
            frmHexDiff fhd = new frmHexDiff(PCM, cmpWithPcm, diffTableDatas, cmpTableDatas);
            fhd.Show();
            fhd.findDifferences(false);
        }

        private void findTableDifferences_old(PcmFile cmpWithPcm)
        {
            Logger("Finding tables with different data");
            string newMenuTxt = PCM.FileName + " <> " + cmpWithPcm.FileName;
            Logger(newMenuTxt);
            bool menuExist = false;
            int cmpTdList = 0;
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                if (mi.Name == newMenuTxt)
                {
                    menuExist = true;
                    mi.Checked = true;
                    cmpTdList = (int)mi.Tag;
                }
                else
                {
                    mi.Checked = false;
                }
            }
            if (!menuExist)
            {
                PCM.addTableDatas(newMenuTxt); 
                ToolStripMenuItem miNew = new ToolStripMenuItem(newMenuTxt);
                miNew.Name = newMenuTxt;
                miNew.Checked = true;
                miNew.Tag = PCM.altTableDatas.Count - 1;
                cmpTdList = PCM.altTableDatas.Count - 1;
                tableListToolStripMenuItem.DropDownItems.Add(miNew);
                miNew.Click += tablelistSelect_Click; 
            }

            cmpWithPcm.selectTableDatas(0, "");
            List<TableData> diffTableDatas = new List<TableData>();
            for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
            {
                int cmpId = compareTableHEX(t1, PCM, cmpWithPcm);
                if (cmpId > -1)
                { 
                    PCM.altTableDatas[cmpTdList].tableDatas.Add(PCM.tableDatas[t1]);
                }
            }
            PCM.selectTableDatas(cmpTdList,newMenuTxt);
            if (diffMissingTables > 0)
                Logger(diffMissingTables.ToString() + " Tables not found");
            filterTables();
            Logger(" [OK]");

        }

        private void findTableDifferencesHEX(PcmFile cmpWithPcm)
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
                    uint end = (uint)(start + getElementSize(tData.DataType) * tData.Rows * tData.Columns + tData.Offset);
                    for (uint b = start; b < end; b++)
                        buf[b] = 1;
                }
                for (int s = 0; s < PCM.Segments.Count; s++)
                {
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
                                undefTd.id = (uint)PCM.tableDatas.Count;
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
            cmpWithPcm.selectTableDatas(0, "");
            List<int> diffTableDatas = new List<int>();
            List<int> cmpTableDatas = new List<int>();
            for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
            {
                if (PCM.tableDatas[t1].addrInt < PCM.fsize)
                {
                    int cmpId = compareTableHEX(t1, PCM, cmpWithPcm);
                    if (cmpId > -1)
                    {
                        diffTableDatas.Add(t1);
                        cmpTableDatas.Add(cmpId);
                    }
                }
            }
            Logger(" [OK]");
            frmHexDiff fhd = new frmHexDiff(PCM, cmpWithPcm, diffTableDatas,cmpTableDatas);
            fhd.Show();
            fhd.findDifferences(true);
        }

        private void tablelistSelect_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                //Reset all to uncheck
                mi.Checked = false;
            }

            ToolStripMenuItem mItem = (ToolStripMenuItem)sender;
            PCM.selectTableDatas((int)mItem.Tag,mItem.Name);
            mItem.Checked = true;
            filterTables();
        }


        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportCSV();
        }

        private void xDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Generating xdf...");
            XDF xdf = new XDF();
            Logger(xdf.exportXdf(PCM, PCM.tableDatas));

        }

        private void xMLGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportXMLgeneratorCSV();
        }

        private void dTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importDTC(ref PCM);
        }

        private void tableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTableSeek(ref PCM);
        }

        private void importXDF()
        {
            XDF xdf = new XDF();
            Logger(xdf.importXdf(PCM, PCM.tableDatas));
            Debug.WriteLine("Categories: " + PCM.tableCategories.Count);
            //LoggerBold("Note: Only basic XDF conversions are supported, check Math and SavingMath values");
            refreshTablelist();
            comboTableCategory.Text = "_All";

        }
        private void xDFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            importXDF();
        }

        private void tinyTunerDBV6OnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTinyTunerDB(ref PCM);
        }


        private void cSVexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importxperimentalCSV();
        }

        private void cSV2ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXperimentalCsv2();
        }

        private void addNewTableList()
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
                PCM.selectTableDatas(PCM.altTableDatas.Count, mItem.Name);
                mItem.Tag = PCM.altTableDatas.Count - 1;
                mItem.Checked = true;
                tableListToolStripMenuItem.DropDownItems.Add(mItem);
                mItem.Click += tablelistSelect_Click;
                filterTables();
            }


        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewTableList();
        }

        private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData newTd = new TableData();
            newTd.id = (uint)lastSelectedId;
            newTd.OS = PCM.OS;
            frmTdEditor fte = new frmTdEditor();
            fte.td = newTd;
            fte.loadTd();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                PCM.tableDatas.Insert(lastSelectedId, fte.td);
                filterTables();
            }
            fte.Dispose();
        }

        private void editRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData newTd = PCM.tableDatas[lastSelectedId].ShallowCopy();
            frmTdEditor fte = new frmTdEditor();
            fte.td = newTd;
            fte.loadTd();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                PCM.tableDatas[lastSelectedId] = fte.td.ShallowCopy();
                filterTables();
            }
            fte.Dispose();
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.tableDatas.RemoveAt(lastSelectedId);
            filterTables();
        }

        private void duplicateTableConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData newTd = PCM.tableDatas[lastSelectedId].ShallowCopy();
            PCM.tableDatas.Insert(lastSelectedId, newTd);
            filterTables();
        }

        private void searchAndCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMassCompare fmc = new frmMassCompare();
            fmc.PCM = PCM;
            //int id = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
            fmc.td = PCM.tableDatas[lastSelectedId];
            fmc.Text = "Search and Compare: " + fmc.td.TableName;
            fmc.Show();
            fmc.selectCmpFiles();
        }


        private void xMlgeneratorImportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXMLGeneratorCsv();
        }

        private void xMLGeneratorExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportXMLgeneratorCSV();
        }

        private void loadTablelistxmlTableseekImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.LoadTableList();
            importDTC(ref PCM);            
            importTableSeek(ref PCM);
            comboTableCategory.Text = "_All";
            filterTables();
        }

        private void loadTablelistnewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addNewTableList();
            PCM.LoadTableList();
            comboTableCategory.Text = "_All";
            filterTables();
        }

        private void openMultipleBINToolStripMenuItem_Click(object sender, EventArgs e)
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
                    addtoCurrentFileMenu(newPCM);
                    PCM = newPCM;
                    loadConfigforPCM(ref PCM);
                }
                selectPCM();
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
                pcmFile.saveBin(pcmFile.FileName);
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
                fmc.td = PCM.tableDatas[lastSelectedId];
                fmc.Text = "Search and Compare: " + fmc.td.TableName;
                fmc.Show();
                fmc.selectCmpFiles();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void compareSelectedTables()
        {
            try
            {
                List<int> ids = getSelectedTableIds();
                if (ids.Count != 2)
                {
                    Logger("Select 2 tables!");
                    return;
                }
                int id1 = ids[0];
                int id2 = ids[1];

                TableData td1 = PCM.tableDatas[id1];
                TableData td2 = PCM.tableDatas[id2];
                if (td1.Rows != td2.Rows || td1.Columns != td2.Columns)
                {
                    Logger("Select 2 tables with equal size!");
                    return;
                }
                Logger("Comparing....");
                frmTableEditor frmT = new frmTableEditor();
                frmT.disableMultiTable = disableMultitableToolStripMenuItem.Checked;
                PcmFile comparePCM = PCM.ShallowCopy();
                comparePCM.FileName = td2.TableName;
                frmT.addCompareFiletoMenu(comparePCM, td2, "");
                frmT.Show();
                frmT.prepareTable(PCM, td1, null, "A");
                frmT.loadTable();

            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void compareSelectedTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compareSelectedTables();
        }

        private void massModifyTableListsToolStripMenuItem_Click(object sender, EventArgs e)
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
            fmm.loadData(tunerFiles);
        }

        private void massModifyTableListsSelectFilesToolStripMenuItem_Click(object sender, EventArgs e)
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
            fmm.loadData(tunerFiles);
        }

        private void moreSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMoreSettings frmTS = new frmMoreSettings();
            frmTS.Show();
        }

        private void timerFilter_Tick(object sender, EventArgs e)
        {
            keyDelayCounter++;
            if (keyDelayCounter > Properties.Settings.Default.keyPressWait100ms)
            {
                timerFilter.Enabled = false;
                keyDelayCounter = 0;
                filterTables();
            }
        }

        private void caseSensitiveFilteringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            caseSensitiveFilteringToolStripMenuItem.Checked = !caseSensitiveFilteringToolStripMenuItem.Checked;
            filterTables();
        }


        private void copySelectedTablesToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> tableIds = getSelectedTableIds();
            frmMassCopyTables fls = new frmMassCopyTables();
            fls.PCM = PCM;
            fls.tableIds = tableIds;
            fls.Show();
            fls.startTableCopy();

        }

        private void openPatchSelector()
        {
            frmPatchSelector frmPS = new frmPatchSelector();
            frmPS.basefile = PCM;
            frmPS.tunerForm = this;
            frmPS.Show();
            Application.DoEvents();
            frmPS.loadPatches();
        }

        public void applyPatch(string fileName)
        {
            Logger("Loading file: " + fileName);
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            PatchList = (List<XmlPatch>)reader.Deserialize(file);
            file.Close();
            ApplyXMLPatch(PCM);
        }

        private void applyPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openPatchSelector();
        }

        private List<int> getSelectedTableIds()
        {
            List<int> tableIds = new List<int>();
            if (treeMode)
            {
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                //foreach (TreeNode tn in tv.Nodes)
                //  findCheckdNodes(tn, ref tableIds);
                foreach (TreeNode tn in tv.SelectedNodes)
                    if (tn.Tag != null)
                        tableIds.Add((int)tn.Tag);
                if (tableIds.Count == 0)
                    tableIds.Add(lastSelectedId);
            }
            else
            {
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int row = dataGridView1.SelectedCells[i].RowIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[row].Cells["id"].Value);
                    if (!tableIds.Contains(id))
                        tableIds.Add(id);
                }
            }
            return tableIds;
        }

        private void generateTablePatch()
        {
            List<int> tableIds = getSelectedTableIds();
            if (tableIds.Count == 0)
            {
                Logger("No tables selected");
                return;
            }
            string defName = Path.Combine(Application.StartupPath, "Patches", "newpatch.xmlpatch");
            string patchFname = SelectSaveFile("PATCH files (*.xmlpatch)|*.xmlpatch|ALL files (*.*)|*.*", defName);
            if (patchFname.Length == 0)
                return;
            string Description = "";
            frmData frmD = new frmData();
            frmD.Text = "Patch Description";
            if (frmD.ShowDialog() == DialogResult.OK)
                Description = frmD.txtData.Text;
            frmD.Dispose();
            List<XmlPatch> newPatch = new List<XmlPatch>();
            for (int i=0; i < tableIds.Count; i++)
            {
                int id = tableIds[i];
                TableData pTd = PCM.tableDatas[id];
                XmlPatch xpatch = new XmlPatch();
                xpatch.CompatibleOS = "Table:" + pTd.TableName + ",columns:" + pTd.Columns.ToString() + ",rows:" + pTd.Rows.ToString();
                xpatch.XmlFile = PCM.configFile;
                xpatch.Segment = PCM.GetSegmentName(pTd.addrInt);
                xpatch.Description = Description;
                frmTableEditor frmTE = new frmTableEditor();
                frmTE.prepareTable(PCM, pTd, null, "A");
                frmTE.loadTable();
                uint step = (uint)getElementSize(pTd.DataType);
                uint addr = (uint)(pTd.addrInt + pTd.Offset);
                if (pTd.RowMajor)
                {
                    for (int r=0; r<pTd.Rows; r++)
                    {
                        for (int c=0; c<pTd.Columns; c++)
                        {
                            xpatch.Data += getValue(PCM.buf, addr, pTd,0, PCM).ToString().Replace(",", ".") + " ";
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
                            xpatch.Data += getValue(PCM.buf, addr, pTd,0, PCM).ToString().Replace(",", ".") + " ";
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

        private void createPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generateTablePatch();
        }

        private void selectTreemode()
        {
            treeMode = true;
            dataGridView1.Visible = false;
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

            tabControl1.Visible = true;
            currentTab = "Dimensions";
            loadDimensions();
            btnCollapse.Visible = true;
            btnExpand.Visible = true;
            //numIconSize.Visible = true;
            //labelIconSize.Visible = true;
            cutToolStripMenuItem.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            selectToolStripMenuItem.Enabled = true;
            labelBy.Visible = false;
            comboFilterBy.Visible = false;
            comboFilterBy.Text = "TableName";
            labelCategory.Visible = false;
            comboTableCategory.Visible = false;
            comboTableCategory.Text = "_All";
            settingsToolStripMenuItem.Visible = false;
            utilitiesToolStripMenuItem.Visible = false;
            showTablesWithEmptyAddressToolStripMenuItem.Visible = false;
            unitsToolStripMenuItem.Visible = false;
            enableConfigModeToolStripMenuItem.Visible = false;
            resetTunerModeColumnsToolStripMenuItem.Visible = false;
            importXDFToolStripMenuItem.Visible = true;
        }

        private void updateFileInfoTab()
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
                    rBox.Dock = DockStyle.Fill;
                    showFileInfo(infoPcm, rBox);
                }
            }

        }

        private void TabFileInfo_Enter(object sender, EventArgs e)
        {
            if (currentTab == "FileInfo")
                return;
            currentTab = "FileInfo";
            updateFileInfoTab();
        }

        private void selectListMode()
        {
            clearPanel2();
            treeMode = false;
            if (splitTree != null)
                splitTree.Visible = false;
            dataGridView1.Visible = true;
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
            settingsToolStripMenuItem.Visible = true;
            utilitiesToolStripMenuItem.Visible = true;

            showTablesWithEmptyAddressToolStripMenuItem.Visible = true;
            unitsToolStripMenuItem.Visible = true;
            enableConfigModeToolStripMenuItem.Visible = true;
            resetTunerModeColumnsToolStripMenuItem.Visible = false;
            importXDFToolStripMenuItem.Visible = false;
        }


        private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
                if (tv.SelectedNode != null && tv.SelectedNode.Tag != null)
                    lastSelectedId = Convert.ToInt32(tv.SelectedNode.Tag);
                else
                    lastSelectedId = -1;
                contextMenuStripTree.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void findCheckdNodes(TreeNode tn, ref List<int> tableIds)
        {
            if (tn.Nodes.Count > 0)
            {
                foreach (TreeNode tnChild in tn.Nodes)
                    findCheckdNodes(tnChild, ref tableIds);
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

        private void autoMultiTable()
        {
            if (!chkAutoMulti1d.Checked)
                return;
            TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
            if (tv.SelectedNode == null)
                return;
            if (tv.SelectedNode.Tag != null)
                return;
            List<int> tableIds = new List<int>();
            foreach (TreeNode tn in tv.SelectedNode.Nodes)
            {
                int id = (int)tn.Tag;
                if (PCM.tableDatas[id].Dimensions() == 1 && !PCM.tableDatas[id].TableName.Contains("[") && !PCM.tableDatas[id].TableName.Contains("."))
                    tableIds.Add(id);
            }
            if (tableIds.Count > 0)
            {
                clearPanel2();
                openTableEditor(tableIds);
            }

        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tabControl1.SelectedTab.Name == "tabSettings" || tabControl1.SelectedTab.Name == "tabPatches")
                return;
            TreeViewMS tms = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
            if (e.Node.Tag == null)
            {
                autoMultiTable();
                return;
            }
            clearPanel2();
            List<int> tableIds = new List<int>();
            //int tbId = Convert.ToInt32(e.Node.Tag);
            foreach(TreeNode tn in tms.SelectedNodes)
                if (tn.Tag != null)
                    tableIds.Add((int)tn.Tag);
            showTableDescription(PCM, tableIds[0]);
            openTableEditor(tableIds);
        }


        private void TabSegments_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Segments")
                return;
            currentTab = "Segments";
            clearPanel2();
            loadSegments();
        }


        private void showPatchSelector()
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
            frmP.loadPatches();
        }

        private void TabPatches_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Patches")
                return;
            if (PCM == null || PCM.configFileFullName == null || PCM.configFileFullName.Length == 0)
                return;
            currentTab = "Patches";
            clearPanel2();
            showPatchSelector();
        }

        private void TabValueType_Enter(object sender, EventArgs e)
        {
            if (currentTab == "ValueType")
                return;
            currentTab = "ValueType";
            clearPanel2();
            loadValueTypes();
        }

        private void TabDimensions_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Dimensions")
                return;
            currentTab = "Dimensions";
            clearPanel2();
            loadDimensions();
        }

        private void TabCategory_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Category")
                return;
            currentTab = "Category";
            clearPanel2();
            loadCategories();
        }

        private void TabSettings_Leave(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            setIconSize();
        }

        private void TabSettings_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Settings")
                return;
            currentTab = "Settings";
        }

        private void setIconSize()
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
        }
        private TreeNode createTreeNode(string txt)
        {
            TreeNode tn = new TreeNode(txt);
            tn.Name = txt;
            tn.ImageKey = txt + ".ico";
            tn.SelectedImageKey = txt + ".ico";
            return tn;
        }

        private void loadDimensions()
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
                setIconSize();
                treeDimensions.ImageList = imageList1;
                treeDimensions.CheckBoxes = false;
                treeDimensions.Dock = DockStyle.Fill;
                tabDimensions.Controls.Add(treeDimensions);
                //treeDimensions.AfterCheck += Tree_AfterCheck;
                treeDimensions.AfterSelect += Tree_AfterSelect;
                treeDimensions.NodeMouseClick += Tree_NodeMouseClick;
                treeDimensions.Nodes.Add(createTreeNode("1D"));
                treeDimensions.Nodes.Add(createTreeNode("2D"));
                treeDimensions.Nodes.Add(createTreeNode("3D"));
            }

            for (int i = 0; i < filteredCategories.Count; i++)
            {
                //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {
                    TreeNode tnChild = new TreeNode(filteredCategories[i].TableName);
                    tnChild.Tag = (int)filteredCategories[i].id;

                    TableValueType vt = getValueType(filteredCategories[i]);
                    if (filteredCategories[i].BitMask != null && filteredCategories[i].BitMask.Length > 0)
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
                    else
                    {
                        tnChild.ImageKey = "number.ico";
                        tnChild.SelectedImageKey = "number.ico";
                    }

                    string nodeKey = "";
                    switch (filteredCategories[i].Dimensions())
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
                        string cat = filteredCategories[i].Category;
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
        private void loadValueTypes()
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
                setIconSize();
                treeValueType.ImageList = imageList1;
                treeValueType.CheckBoxes = false;
                treeValueType.Dock = DockStyle.Fill;
                tabValueType.Controls.Add(treeValueType);
                treeValueType.AfterSelect += Tree_AfterSelect;
                treeValueType.AfterCheck += Tree_AfterCheck;
                treeValueType.NodeMouseClick += Tree_NodeMouseClick;
                treeValueType.Nodes.Add(createTreeNode("number"));
                treeValueType.Nodes.Add(createTreeNode("enum"));
                treeValueType.Nodes.Add(createTreeNode("bitmask"));
                treeValueType.Nodes.Add(createTreeNode("boolean"));
            }

            for (int i = 0; i < filteredCategories.Count; i++)
            {
                //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {
                    TreeNode tnChild = new TreeNode(filteredCategories[i].TableName);
                    tnChild.Tag = (int)filteredCategories[i].id;

                    switch(filteredCategories[i].Dimensions())
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

                    TableValueType vt = getValueType(filteredCategories[i]);
                    string nodeKey = "";
                    if (filteredCategories[i].BitMask != null && filteredCategories[i].BitMask.Length > 0)
                        nodeKey = "bitmask";
                    else if (vt == TableValueType.boolean)
                        nodeKey = "boolean";
                    else if (vt == TableValueType.selection)
                        nodeKey = "enum";
                    else
                        nodeKey = "number";

                    if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                    {
                        treeValueType.Nodes[nodeKey].Nodes.Add(tnChild);
                    }
                    else
                    {
                        string cat = filteredCategories[i].Category;
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

        private void loadCategories()
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
                setIconSize();
                treeCategory.ImageList = imageList1;
                treeCategory.CheckBoxes = false;

                treeCategory.Dock = DockStyle.Fill;
                tabCategory.Controls.Add(treeCategory);
                treeCategory.AfterSelect += Tree_AfterSelect;
                treeCategory.AfterCheck += Tree_AfterCheck;
                treeCategory.NodeMouseClick += Tree_NodeMouseClick;
            }
            for (int i = 0; i < filteredCategories.Count; i++)
            {
                //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {

                    TreeNode tnChild = new TreeNode(filteredCategories[i].TableName);
                    tnChild.Tag = (int)filteredCategories[i].id;
                    string ico = "";
                    TableValueType vt = getValueType(filteredCategories[i]);
                    if (filteredCategories[i].BitMask != null && filteredCategories[i].BitMask.Length > 0)
                    {
                        ico = "mask";
                    }
                    else if (vt == TableValueType.boolean)
                    {
                        ico = "flag";
                    }
                    else if (vt == TableValueType.selection)
                    {
                        ico = "enum";
                    }

                    switch (filteredCategories[i].Dimensions())
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

                    string cat = filteredCategories[i].Category;
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

        private void loadSegments()
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
                setIconSize();
                treeSegments.ImageList = imageList1;
                treeSegments.CheckBoxes = false;

                treeSegments.Dock = DockStyle.Fill;
                tabSegments.Controls.Add(treeSegments);
                treeSegments.AfterSelect += Tree_AfterSelect;
                treeSegments.AfterCheck += Tree_AfterCheck;
                treeSegments.NodeMouseClick += Tree_NodeMouseClick;
            }
            TreeNode segTn;
            for (int i = 0; i < PCM.Segments.Count; i++)
            {
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

            for (int i = 0; i < filteredCategories.Count; i++)
            {
                //if (filteredCategories[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {

                    TreeNode tnChild = new TreeNode(filteredCategories[i].TableName);
                    tnChild.Tag = (int)filteredCategories[i].id;
                    string ico = "";
                    TableValueType vt = getValueType(filteredCategories[i]);
                    if (filteredCategories[i].BitMask != null && filteredCategories[i].BitMask.Length > 0)
                    {
                        ico = "mask";
                    }
                    else if (vt == TableValueType.boolean)
                    {
                        ico = "flag";
                    }
                    else if (vt == TableValueType.selection)
                    {
                        ico = "enum";
                    }

                    switch (filteredCategories[i].Dimensions())
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

                    string cat = filteredCategories[i].Category;
                    if (cat == "")
                        cat = "(Empty)";

                    int seg = PCM.GetSegmentNumber(filteredCategories[i].addrInt);
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

        private void clearPanel2()
        {
            if (splitTree == null)
                return;
            foreach (var x in splitTree.Panel2.Controls.OfType<Form>())
            {
                x.Close();
            }
            labelTableName.Text = "";
            txtDescription.Text = "";
        }

        private void selectDispMode()
        {
            if (radioTreeMode.Checked)
                selectTreemode();
            else
                selectListMode();
            Properties.Settings.Default.TunerTreeMode = radioTreeMode.Checked;
            Properties.Settings.Default.Save();

        }
        private void radioTreeMode_CheckedChanged(object sender, EventArgs e)
        {
            radioListMode.Checked = !radioTreeMode.Checked;
            selectDispMode();
        }

        private void radioListMode_CheckedChanged(object sender, EventArgs e)
        {
            radioTreeMode.Checked = !radioListMode.Checked;
            selectDispMode();
        }

        private void showCategorySubfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void btnMultitable_Click(object sender, EventArgs e)
        {
            TreeViewMS tv = (TreeViewMS)tabControl1.SelectedTab.Controls[0];
            if (tv.SelectedNode == null)
                return;
            if (tv.SelectedNode.Tag != null)
                return;
            List<int> tableIds = new List<int>();
            foreach (TreeNode tn in tv.SelectedNode.Nodes)
            {
                int id = (int)tn.Tag;
                if (PCM.tableDatas[id].Dimensions() == 1 && !PCM.tableDatas[id].TableName.Contains("[") && !PCM.tableDatas[id].TableName.Contains("."))
                    tableIds.Add(id);                    
            }
            if (tableIds.Count > 0)
            {
                clearPanel2();
                openTableEditor(tableIds);
            }
        }

        private void chkShowCategorySubfolder_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TableExplorerUseCategorySubfolder = chkShowCategorySubfolder.Checked;
            Properties.Settings.Default.Save();
            filterTree();
        }

        private void numIconSize_ValueChanged(object sender, EventArgs e)
        {

        }

        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> tableIds = getSelectedTableIds();
            openTableEditor(tableIds, true);
        }

        private void chkAutoMulti1d_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TunerAutomulti1d = chkAutoMulti1d.Checked;
        }

        private void selectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) return;
            PcmFile cmpWithPcm = new PcmFile(newFile, true, PCM.configFileFullName);
            loadConfigforPCM(ref cmpWithPcm);
            addtoCurrentFileMenu(cmpWithPcm, false);
            findTableDifferences(cmpWithPcm);
        }

        private void selectFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) return;
            PcmFile cmpWithPcm = new PcmFile(newFile, true, PCM.configFileFullName);
            loadConfigforPCM(ref cmpWithPcm);
            addtoCurrentFileMenu(cmpWithPcm, false);
            findTableDifferencesHEX(cmpWithPcm);
        }

        private void compareSelectedTablesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            compareSelectedTables();
        }

        private void importXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXDF();
        }
    }
}
