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
            tableDataList = PCM.tableDatas;
            if (PCM == null || PCM1.fsize == 0) return; //No file selected
            addtoCurrentFileMenu(PCM);
            loadConfigforPCM();
            selectPCM();
        }

        private PcmFile PCM;
        private List<TableData> tableDataList;
        private string sortBy = "id";
        private int sortIndex = 0;
        private bool columnsModified = false;
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        private BindingList<TableData> filteredCategories = new BindingList<TableData>();
        SortOrder strSortOrder = SortOrder.Ascending;
        private string currentXmlFile;

        private void selectPCM()
        {
            this.Text = "Tuner " + PCM.FileName;
            tableDataList = PCM.tableDatas;
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                if (mi.Name == PCM.FileName)
                {
                    mi.Checked = true;
                    mi.Tag = PCM.tableDatas;
                }
                else
                {
                    mi.Checked = false;
                }
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
                
            findDifferencesToolStripMenuItem.DropDownItems[PCM.FileName].Enabled = false;

            ToolStripMenuItem mitem = (ToolStripMenuItem)currentFileToolStripMenuItem.DropDownItems[PCM.FileName];
            mitem.Checked = true;

        }

        private void loadConfigforPCM()
        {
            tableDataList = PCM.tableDatas;
            if (!Properties.Settings.Default.disableTunerAutoloadSettings)
            {
                string defaultXml = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                if (File.Exists(defaultXml))
                {
                    LoadTableList(defaultXml);
                    bool haveDTC = false;
                    for (int t = 0; t < PCM.tableDatas.Count; t++)
                    {
                        if (PCM.tableDatas[t].TableName.StartsWith("DTC"))
                        {
                            haveDTC = true;
                            break;
                        }
                    }
                    if (!haveDTC)
                        importDTC();
                }
                else
                {
                    Logger("File not found: " + defaultXml);
                    importDTC();
                    importTableSeek();
                }
            }

        }
        private void openTableEditor()
        {
            try
            {
                List<int> tableIds = new List<int>();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int row = dataGridView1.SelectedCells[i].RowIndex;
                    int id = Convert.ToInt32(dataGridView1.Rows[row].Cells["id"].Value);
                    if (!tableIds.Contains(id))
                        tableIds.Add(id);
                }
                for (int i=1; i< tableIds.Count;i++)
                {
                    if (tableDataList[tableIds[i]].Rows != tableDataList[tableIds[0]].Rows || tableDataList[tableIds[i]].Columns != tableDataList[tableIds[0]].Columns)
                    {
                        LoggerBold("Can't load multible tables with different size");
                        return;
                    }
                }
                TableData td = tableDataList[tableIds[0]];
                if (td.addrInt == uint.MaxValue)
                {
                    Logger("No address defined!");
                    return;
                }

                if (td.OS != PCM.OS)
                {
                    LoggerBold("WARING! OS Mismatch, File OS: " + PCM.OS + ", config OS: " + td.OS);
                }
/*                if (td.OutputType == OutDataType.Flag && td.BitMask != null && td.BitMask.Length > 0)
                {
                    frmEditFlag ff = new frmEditFlag();
                    ff.loadFlag(PCM, td);
                    ff.Show();
                }
                else*/
                {
                    frmTableEditor frmT = new frmTableEditor();
                    frmT.PCM = PCM;
                    frmT.tableIds = tableIds;
                    frmT.disableMultiTable = disableMultitableToolStripMenuItem.Checked;
                    foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                    {
                        PcmFile comparePCM = (PcmFile)mi.Tag;
                        if (PCM.FileName != comparePCM.FileName)
                        {
                            Logger("Adding file: " + Path.GetFileName(comparePCM.FileName) + " to compare menu... ", false);
                            bool tblFound = false;
                            string tbName = td.TableName;
                            if (td.TableName.Contains("*"))
                            {
                                tbName = td.TableName.Substring(0, td.TableName.IndexOf('*') );
                            }
                            for (int x = 0; x < comparePCM.tableDatas.Count; x++)
                            {
                                if (comparePCM.tableDatas[x].Category == td.Category && comparePCM.tableDatas[x].TableName.StartsWith(tbName))
                                {
                                    if (comparePCM.tableDatas[x].Rows != td.Rows || comparePCM.tableDatas[x].Columns != td.Columns || comparePCM.tableDatas[x].RowMajor != td.RowMajor)
                                    {
                                        Logger("Table size not match!");
                                    }
                                    else
                                    {
                                        tblFound = true;
                                        comparePCM.selectedTable = comparePCM.tableDatas[x];
                                        frmT.addCompareFiletoMenu(comparePCM);
                                        if (PCM.configFile != comparePCM.configFile)
                                        {
                                            LoggerBold(Environment.NewLine + "Warning: file type different, results undefined!");
                                        }
                                        else
                                        {
                                            Logger("[OK]");
                                        }
                                        break;
                                    }
                                }
                            }                            
                            if (!tblFound)
                            {
                                LoggerBold("Table not found" );
                            }
                        }
                    }
                    frmT.Show();
                    frmT.loadTable(td);
                }
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
            int Start = txtResult.Text.Length;
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
                this.Text = "Tuner " + Path.GetFileName(fileName);
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
            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            //dataGridView1.Columns["OutputType"].ToolTipText = "1=Float, 2=Int, 3=Hex, 4=Text, 5=Flag";
            //dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            /*for (int i=0; i< dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["OutputType"].ToolTipText = "1=Float, 2=Int, 3=Hex, 4=Text, 5=Flag";
                dataGridView1.Rows[i].Cells["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
                if (dataGridView1.Rows[i].Cells["TableDescription"].Value != null)
                    dataGridView1.Rows[i].Cells["TableName"].ToolTipText = dataGridView1.Rows[i].Cells["TableDescription"].Value.ToString();
            }*/

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

        private void importTableSeek()
        {
            if (PCM.foundTables.Count == 0)
            {
                TableSeek TS = new TableSeek();
                Logger("Seeking tables...", false);
                Logger(TS.seekTables(PCM));
            }
            Logger("Importing TableSeek tables... ", false);
            for (int i = 0; i < PCM.foundTables.Count; i++)
            {
                TableData tableData = new TableData();
                tableData.importFoundTable(i, PCM);
                PCM.tableDatas.Add(tableData);
            }
            refreshTablelist();
            Logger("OK");
        }

        private void LoadTableList(string fName="")
        {
            try
            {

                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                //string defName = PCM.OS + ".xml";
                if (fName == "")
                    fName = SelectFile("Select XML File", "XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
                Logger("Loading file: " + fName, false);
                if (fName.Length == 0)
                    return;
                if (File.Exists(fName))
                {
                    currentXmlFile = fName;
                    Debug.WriteLine("Loading " + fName + "...");
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    System.IO.StreamReader file = new System.IO.StreamReader(fName);
                    PCM.tableDatas = (List<TableData>)reader.Deserialize(file);
                    file.Close();
                }
                for (int t = 0; t < PCM.tableDatas.Count; t++)
                {
                    string category = PCM.tableDatas[t].Category;
                    if (!PCM.tableCategories.Contains(category))
                        PCM.tableCategories.Add(category);
                }
                tableListToolStripMenuItem.DropDownItems[PCM.FileName].Tag = PCM.tableDatas;
                tableDataList = PCM.tableDatas;
                Logger(" [OK]");
                refreshTablelist();
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
                LoggerBold("Error, line " + line + ": " + ex.Message);
            }
        }
        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            LoadTableList();
        }

        private void SaveTableList(string fName ="")
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                if (fName.Length == 0)
                    fName = SelectSaveFile("XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
                if (fName.Length == 0)
                    return;
                currentXmlFile = fName;
                Logger("Saving file " + fName + "...", false);
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, tableDataList);
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
                LoggerBold("Error, line " + line + ": " + ex.Message);
            }

        }
        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            SaveTableList();
        }

        private void btnImportDTC_Click(object sender, EventArgs e)
        {
            importDTC();
        }

        private void frmTuner_Load(object sender, EventArgs e)
        {
            enableConfigModeToolStripMenuItem.Checked = Properties.Settings.Default.TunerConfigMode;

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
            tableDataList = PCM.tableDatas;
            //ToolStripMenuItem miNormal = new ToolStripMenuItem("Normal tablelist");
            //miNormal.Name = "Normal tablelist";
            //miNormal.Tag = PCM.tableDatas;
            //miNormal.Checked = true;
            //tableListToolStripMenuItem.DropDownItems.Add(miNormal);
            //miNormal.Click += tablelistSelect_Click;

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
                Debug.WriteLine("reorderColumns, line " + line + ": " + ex.Message);
            }

        }
        private void filterTables()
        {
            try
            {
                //if (PCM == null || PCM.fsize == 0) return;
                if (tableDataList == null)
                    return;
                //Save settings before reordering
                saveGridLayout();
                //Fix table-ID's
                for (int tbId = 0; tbId < tableDataList.Count; tbId++)
                    tableDataList[tbId].id = (uint)tbId;

                List<TableData> compareList = new List<TableData>();
                if (strSortOrder == SortOrder.Ascending)
                    compareList = tableDataList.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    compareList = tableDataList.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();

                string cat = comboTableCategory.Text;
                var results = compareList.Where(t => t.TableName.Length > 0); //How should I define empty variable??
                if (txtSearchTableSeek.Text.Length > 0)
                    results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearchTableSeek.Text.ToLower()));
                if (!showTablesWithEmptyAddressToolStripMenuItem.Checked)
                    results = results.Where(t => t.addrInt < uint.MaxValue);
                if (cat != "_All" && cat != "")
                    results = results.Where(t => t.Category.ToLower().Contains(comboTableCategory.Text.ToLower()));
                filteredCategories = new BindingList<TableData>(results.ToList());
                bindingsource.DataSource = filteredCategories;
                reorderColumns();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

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
                LoggerBold("Error, line " + line + ": " + ex.Message);
            }
        }

        private void importTinyTunerDB()
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDBtoTableData(PCM,tableDataList));
            refreshTablelist();

        }
        private void btnReadTinyTunerDB_Click(object sender, EventArgs e)
        {
            importTinyTunerDB();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTableList();
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTableList(currentXmlFile);
        }

        private void saveBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            PCM.saveBin(PCM.FileName);
            Logger("Done.");

        }

        private void importDTC()
        {
            Logger("Importing DTC codes... ", false);
            TableData tdTmp = new TableData();
            tdTmp.importDTC(PCM, ref tableDataList);
            Logger(" [OK]");
            filterTables();
        }
        private void importDTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void importTableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void importXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void importTinyTunerDBV6OnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void clearTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableDataList = new List<TableData>();
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
                if (mi.Checked)
                    mi.Tag = tableDataList;
            refreshTablelist();
        }


        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0 && e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
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
            openTableEditor();
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
            for (int i = 0; i < tableDataList.Count; i++)
                tableDataList[i].addrInt = uint.MaxValue;
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
                    for (int i = 0; i < tableDataList.Count; i++)
                    {
                        if (tableDataList[i].Category.ToLower() == cat.ToLower() && tableDataList[i].TableName.ToLower() == name.ToLower())
                        {
                            tableDataList[i].Address = addr;
                            tableDataList[i].OS = osNew;
                            Debug.WriteLine(tableDataList[i].TableName);
                            //tableDataList[i].AddrInt = Convert.ToUInt32(addr, 16);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Debug.WriteLine(name + ": not found");
                        for (int i = 0; i < tableDataList.Count; i++)
                        {
                            if (cat.ToLower() == "protected" && tableDataList[i].TableName.ToLower() == name.ToLower())
                            {
                                tableDataList[i].Address = addr;
                                tableDataList[i].OS = osNew;
                                //tableDataList[i].AddrInt = Convert.ToUInt32(addr, 16);
                                found = true;
                                Debug.WriteLine(name + ": PROTECTED");
                                break;
                            }
                        }
                    }
                }
            }
            /*            for (int i = tableDataList.Count -1; i >= 0; i--)
                        {
                            if (tableDataList[i].addrInt == uint.MaxValue)
                                tableDataList.RemoveAt(i);
                        }*/
            //Fix table names:
            for (int i = 0; i < tableDataList.Count; i++)
            {
                tableDataList[i].OS = osNew;
                if (tableDataList[i].TableName.ToLower().StartsWith("ka_") || tableDataList[i].TableName.ToLower().StartsWith("ke_") || tableDataList[i].TableName.ToLower().StartsWith("kv_"))
                    tableDataList[i].TableName = tableDataList[i].TableName.Substring(3);
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
            filterTables();
        }

        private void showTablesWithEmptyAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showTablesWithEmptyAddressToolStripMenuItem.Checked)
                showTablesWithEmptyAddressToolStripMenuItem.Checked = false;
            else
                showTablesWithEmptyAddressToolStripMenuItem.Checked = true;
            filterTables();
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
                    for (int i = 0; i < tableDataList.Count; i++)
                    {
                        if (tableDataList[i].Category.ToLower() == cat.ToLower() && tableDataList[i].TableName.ToLower().StartsWith(name.ToLower()))
                        {
                            if (name == "K_DYNA_AIR_COEFFICIENT")
                            {
                                //tableDataList[i].Address = addrInt.ToString("X8");
                                tableDataList[i].addrInt = addrInt;
                                Debug.WriteLine(tableDataList[i].TableName + ": " + addrInt.ToString("X"));
                                addrInt += 2;
                            }
                            else
                            {
                                uint mask = Convert.ToUInt32(tableDataList[i].BitMask, 16);
                                if (lastmask == uint.MaxValue)
                                    lastmask = mask;
                                if (mask > lastmask)
                                {
                                    addrInt++;
                                }
                                lastmask = mask;
                                //tableDataList[i].Address = addrInt.ToString("X8");
                                tableDataList[i].addrInt = addrInt;
                                Debug.WriteLine(tableDataList[i].TableName + ": " + addrInt.ToString("X") + " mask: " + mask.ToString("X"));
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

        private void convertToDataTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i=0; i< tableDataList.Count; i++)
            {
                TableData t = tableDataList[i];
                t.DataType = convertToDataType(t.ElementSize, t.Signed, t.Floating);
            }
            refreshTablelist();
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

        }

        private void unitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML fe = new frmEditXML();
            fe.Show();
            fe.LoadUnits();
        }

        private void fixTableNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i=0;i<tableDataList.Count; i++)
            {
                if (tableDataList[i].TableName.ToLower().StartsWith("ka_") || tableDataList[i].TableName.ToLower().StartsWith("ke_") || tableDataList[i].TableName.ToLower().StartsWith("kv_"))
                    tableDataList[i].TableName = tableDataList[i].TableName.Substring(3);
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
                tableDataList.RemoveAt(id);
                filterTables();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void peekTableValuesWithCompare(int ind)
        {
            int myInd = findTableDataId(tableDataList[ind], PCM);
            if (myInd == -1)
            {
                LoggerBold("Table missing: " + tableDataList[ind].TableName);
                return;
            }
            peekTableValues(myInd, PCM);
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
            {
                PcmFile peekPCM = (PcmFile)mi.Tag;
                if (peekPCM.FileName != PCM.FileName)
                {
                    myInd = findTableDataId(tableDataList[ind], peekPCM);
                    if (myInd > 0)
                    {
                        txtDescription.AppendText(peekPCM.FileName + ": " + Environment.NewLine);
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
                frmTableEditor frmT = new frmTableEditor();
                frmT.PCM = peekPCM;
                frmT.disableMultiTable = true;
                frmT.loadTable(peekPCM.tableDatas[ind]);
                txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
                txtDescription.SelectionColor = Color.Blue;
                if (peekPCM.tableDatas[ind].Rows == 1 && peekPCM.tableDatas[ind].Columns == 1)
                {
                    double curVal = frmT.getValue((uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset), peekPCM.tableDatas[ind]);
                    UInt64 rawVal = frmT.getRawValue((uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset), peekPCM.tableDatas[ind]);
                    string valTxt = curVal.ToString();
                    string unitTxt = " " + peekPCM.tableDatas[ind].Units;
                    string maskTxt = "";
                    if (peekPCM.tableDatas[ind].OutputType == OutDataType.Flag || peekPCM.tableDatas[ind].Units.ToLower().StartsWith("boolean"))
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
                    else if (peekPCM.tableDatas[ind].Values.StartsWith("Enum: "))
                    {
                        Dictionary<double, string> possibleVals = frmT.parseEnumHeaders(peekPCM.tableDatas[ind].Values.Replace("Enum: ", ""));
                        if (possibleVals.ContainsKey(curVal))
                            unitTxt = " (" + possibleVals[curVal] + ")";
                        else
                            unitTxt = " (Out of range)";
                    }
                    string formatStr = "X" + (getElementSize(peekPCM.tableDatas[ind].DataType) * 2).ToString();
                    txtDescription.AppendText("Current value: " + valTxt + unitTxt + " [" + rawVal.ToString(formatStr) + "]" + maskTxt);
                    txtDescription.AppendText(Environment.NewLine);
                }
                else
                {
                    string tblData = "Current values: " + Environment.NewLine;
                    uint addr = (uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset);
                    if (peekPCM.tableDatas[ind].RowMajor)
                    {
                        for (int r = 0; r < peekPCM.tableDatas[ind].Rows; r++)
                        {
                            for (int c = 0; c < peekPCM.tableDatas[ind].Columns; c++)
                            {
                                double curVal = frmT.getValue(addr, peekPCM.tableDatas[ind]);
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
                                double curVal = frmT.getValue(addr, peekPCM.tableDatas[ind]);
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

        private int findTableDataId(TableData refTd, PcmFile pcm1)
        {
            for (int t = 0; t < pcm1.tableDatas.Count; t++)
            {
                if (pcm1.tableDatas[t].TableName == refTd.TableName && pcm1.tableDatas[t].Category == refTd.Category)
                {
                    return t;
                }
            }
            return -1;
        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            txtDescription.Text = "";
            if (dataGridView1.SelectedCells.Count < 1 || tableDataList.Count == 0)
            {
                return;
            }
            int ind = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Bold);
            txtDescription.AppendText(tableDataList[ind].TableName + Environment.NewLine); 
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
            if (tableDataList[ind].TableDescription != null)
                txtDescription.AppendText(tableDataList[ind].TableDescription + Environment.NewLine);
            if (tableDataList[ind].ExtraDescription != null)
                txtDescription.AppendText(tableDataList[ind].ExtraDescription + Environment.NewLine);

            peekTableValuesWithCompare(ind);
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

                string csvData = "Category;Tablename;Size;" + tableDataList[0].OS + ";" + Environment.NewLine;
                for (int row = 0; row < tableDataList.Count; row++)
                {
                    int tbSize = tableDataList[row].Rows * tableDataList[row].Columns * getElementSize(tableDataList[row].DataType);
                    csvData += tableDataList[row].Category + ";";
                    csvData += tableDataList[row].TableName + ";";
                    csvData += tbSize.ToString() + ";";
                    //csvData += tableDataList[row].OS + ";";
                    csvData += tableDataList[row].Address;
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
                Logger("Using file: " + currentXmlFile + " as template");
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
                            for (int t = 0; t < tableDataList.Count; t++)
                            {
                                if (tableDataList[t].TableName == osAddrList[x].tableName && tableDataList[t].Category == osAddrList[x].category)
                                {
                                    TableData newTd = tableDataList[t].ShallowCopy();
                                    newTd.OS = osList[o];
                                    newTd.Address = osAddrList[x].addr;
                                    newTds.Add(newTd);
                                    Debug.WriteLine(tableDataList[t].TableName + ", addr:" + osAddrList[x].addr);
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
        private void importXMLgeneratorCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        private void loadBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) return;
            PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName, null);
            addtoCurrentFileMenu(newPCM);
            PCM = newPCM;
            loadConfigforPCM();
            selectPCM();
        }

        private void addtoCurrentFileMenu(PcmFile newPCM)
        {
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                mi.Checked = false;
            ToolStripMenuItem menuitem = new ToolStripMenuItem(newPCM.FileName);
            menuitem.Name = newPCM.FileName;
            menuitem.Tag = newPCM;
            menuitem.Checked = true;
            currentFileToolStripMenuItem.DropDownItems.Add(menuitem);
            menuitem.Click += Menuitem_Click;

            ToolStripMenuItem cmpMenuitem = new ToolStripMenuItem(menuitem.Name);
            cmpMenuitem.Name = menuitem.Name;
            cmpMenuitem.Tag = newPCM;
            findDifferencesToolStripMenuItem.DropDownItems.Add(cmpMenuitem);
            cmpMenuitem.Click += compareMenuitem_Click;

            ToolStripMenuItem tdMenuItem = new ToolStripMenuItem(newPCM.FileName);
            tdMenuItem.Name = newPCM.FileName;
            tdMenuItem.Tag = newPCM.tableDatas;
            tableListToolStripMenuItem.DropDownItems.Add(tdMenuItem);
            tdMenuItem.Click += tablelistSelect_Click;
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
        private bool compareTable(int tInd,PcmFile pcm1, PcmFile pcm2)
        {
            TableData td1 = pcm1.tableDatas[tInd];
            TableData td2 = td1.ShallowCopy();
            int tbSize = td1.Rows * td1.Columns * getElementSize(td1.DataType);
            if (pcm1.OS != pcm2.OS)
            {
                bool found = false;
                //Not 100% compatible file, find table by name & category
                for (int t = 0; t < pcm2.tableDatas.Count; t++)
                {
                    if (pcm2.tableDatas[t].TableName == td1.TableName && pcm2.tableDatas[t].Category == td1.Category)
                    {
                        td2 = pcm2.tableDatas[t];
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    //Logger("Table not found: " + td1.TableName + "[" + pcm2.FileName + "]");
                    diffMissingTables++;
                    return false;
                }
                int tb2size = td2.Rows * td2.Columns * getElementSize(td2.DataType);
                if (tbSize != tb2size)
                    return false;
            }
            if ((td1.addrInt + tbSize) > pcm1.fsize || (td2.addrInt + tbSize) > pcm2.fsize)
            {
                LoggerBold("Table address out of range: " + td1.TableName);
                return false;
            }

            if (td1.BitMask != null && td1.BitMask.Length > 0)
            {
                //Check only bit
                UInt64 mask = Convert.ToUInt64(td1.BitMask.Replace("0x", ""), 16);
                UInt64 orgVal = (readTableData(pcm1.buf, td1) & mask);
                UInt64 compVal = (readTableData(pcm2.buf, td2) & mask);
                if (orgVal == compVal)
                    return true;
                else
                    return false;
            }
            byte[] buff1 = new byte[tbSize];
            byte[] buff2 = new byte[tbSize];
            Array.Copy(pcm1.buf, td1.addrInt + td1.Offset, buff1, 0, tbSize);
            Array.Copy(pcm2.buf, td2.addrInt + td2.Offset, buff2, 0, tbSize);
            if (buff1.SequenceEqual(buff2))
                return true;
            else
                return false;

        }
        private void findTableDifferences(PcmFile cmpWithPcm)
        {
            Logger("Finding tables with different data");
            string newMenuTxt = PCM.FileName + " <> " + cmpWithPcm.FileName;
            Logger(newMenuTxt);
            bool menuExist = false;
            List<TableData> newTableDatas = new List<TableData>();
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                if (mi.Name == newMenuTxt)
                {
                    menuExist = true;
                    mi.Checked = true;
                    mi.Tag = newTableDatas;
                }
                else
                {
                    mi.Checked = false;
                }
            }
            if (!menuExist)
            {
                ToolStripMenuItem miNew = new ToolStripMenuItem(newMenuTxt);
                miNew.Name = newMenuTxt;
                miNew.Checked = true;
                miNew.Tag = newTableDatas;
                tableListToolStripMenuItem.DropDownItems.Add(miNew);
                miNew.Click += tablelistSelect_Click; 
            }

            List<TableData> diffTableDatas = new List<TableData>();
            for (int t1 = 0; t1 < PCM.tableDatas.Count; t1++)
            {
                if (!compareTable(t1, PCM, cmpWithPcm))
                {
                    bool found = false;
                    for (int t2 = 0; t2 < newTableDatas.Count; t2++)
                    {
                        if (newTableDatas[t2].TableName == PCM.tableDatas[t1].TableName && newTableDatas[t2].Category == PCM.tableDatas[t1].Category)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found) //Not previously added
                        newTableDatas.Add(PCM.tableDatas[t1]);
                }
            }
            if (diffMissingTables > 0)
                Logger(diffMissingTables.ToString() + " Tables not found");
            tableDataList = newTableDatas;
            filterTables();
            Logger(" [OK]");

        }

        private void tablelistSelect_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
            {
                //Save current list:
                if (mi.Checked)
                    mi.Tag = tableDataList;
                //Reset all to uncheck
                mi.Checked = false;
            }

            ToolStripMenuItem mItem = (ToolStripMenuItem)sender;
            tableDataList = (List<TableData>)mItem.Tag;
            mItem.Checked = true;
            filterTables();
        }

        private void findDifferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportCSV();
        }

        private void xDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Generating xdf...");
            XDF xdf = new XDF();
            Logger(xdf.exportXdf(PCM, tableDataList));

        }

        private void xMLGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportXMLgeneratorCSV();
        }

        private void dTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importDTC();
        }

        private void tableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTableSeek();
        }

        private void xDFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            XDF xdf = new XDF();
            Logger(xdf.importXdf(PCM, tableDataList));
            Debug.WriteLine("Categories: " + PCM.tableCategories.Count);
            LoggerBold("Note: Only basic XDF conversions are supported, check Math and SavingMath values");
            refreshTablelist();
        }

        private void tinyTunerDBV6OnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTinyTunerDB();
        }

        private void xMgeneratorCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXMLGeneratorCsv();
        }

        private void cSVexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importxperimentalCSV();
        }

        private void cSV2ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importXperimentalCsv2();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmData frmD = new frmData();
            frmD.Text = "Tablelist name:";
            if (frmD.ShowDialog() == DialogResult.OK)
            {
                foreach (ToolStripMenuItem mi in tableListToolStripMenuItem.DropDownItems)
                {
                    //Save current list:
                    if (mi.Checked)
                        mi.Tag = tableDataList;
                    //Reset all to uncheck
                    mi.Checked = false;
                }
                ToolStripMenuItem mItem = new ToolStripMenuItem(frmD.txtData.Text);
                mItem.Name = frmD.txtData.Text;
                tableDataList = new List<TableData>();
                TableData tdTmp = new TableData();
                tableDataList.Add(tdTmp);
                mItem.Tag = tableDataList;
                mItem.Checked = true;
                tableListToolStripMenuItem.DropDownItems.Add(mItem);
                mItem.Click += tablelistSelect_Click;
                filterTables();
            }

        }
    }
}
