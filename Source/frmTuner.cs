using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using static upatcher;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Web.UI.WebControls;

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
            if (PCM == null || PCM1.fsize == 0) return; //No file selected
            addtoCurrentFileMenu(PCM);
            loadConfigforPCM();
            selectPCM();
        }

        private PcmFile PCM;
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
            this.Text = "Tuner " + Path.GetFileName(PCM.FileName);
            if (PCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                importTinyTunerDBV6OnlyToolStripMenuItem.Enabled = true;
            else
                importTinyTunerDBV6OnlyToolStripMenuItem.Enabled = false;
            filterTables();
            foreach (ToolStripMenuItem mi in compareWithToolStripMenuItem.DropDownItems)
                mi.Enabled = true;
            compareWithToolStripMenuItem.DropDownItems[Path.GetFileName(PCM.FileName)].Enabled = false;
        }

        private void loadConfigforPCM()
        {
            if (!Properties.Settings.Default.disableTunerAutoloadSettings)
            {
                string defaultXml = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                if (File.Exists(defaultXml))
                {
                    LoadXML(defaultXml);
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
                        if (PCM.dtcCodes.Count > 0)
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
        private void openTableEditor(PcmFile comparePCM)
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
                    if (PCM.tableDatas[tableIds[i]].Rows != PCM.tableDatas[tableIds[0]].Rows || PCM.tableDatas[tableIds[i]].Columns != PCM.tableDatas[tableIds[0]].Columns)
                    {
                        LoggerBold("Can't load multible tables with different size");
                        return;
                    }
                }
                TableData td = PCM.tableDatas[tableIds[0]];
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
                    if (comparePCM != null)
                    {
                        bool tblFound = false;
                        for (int x=0; x < comparePCM.tableDatas.Count; x++)
                        {
                            if (comparePCM.tableDatas[x].TableName == td.TableName && comparePCM.tableDatas[x].Category == td.Category)
                            {
                                tblFound = true;
                                frmT.loadCompareTable(comparePCM, comparePCM.tableDatas[x]);
                                break;
                            }
                        }
                        if (!tblFound)
                        {
                            LoggerBold("Table not found from: " + Path.GetFileName(comparePCM.FileName));
                            return;
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
            openTableEditor(null);
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

        private void LoadXML(string fName="")
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
            LoadXML();
        }

        private void SaveXML(string fName ="")
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
                    writer.Serialize(stream, PCM.tableDatas);
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
            SaveXML();
        }

        private void importDTC()
        {
            if (PCM.dtcCodes.Count == 0)
            {
                DtcSearch DS = new DtcSearch();
                Logger(DS.searchDtc(PCM));
            }
            Logger("Importing DTC codes... ", false);
            TableData dtcTd = new TableData();
            dtcCode dtc = PCM.dtcCodes[0];
            dtcTd.addrInt = dtc.statusAddrInt;
            dtcTd.Category = "DTC";
            dtcTd.Columns = 1;
            //td.Floating = false;
            dtcTd.OutputType = OutDataType.Int;
            dtcTd.Decimals = 0;
            dtcTd.DataType = InDataType.UBYTE;
            dtcTd.Math = "X";
            dtcTd.OS = PCM.OS;
            for (int i = 0; i < PCM.dtcCodes.Count; i++)
            {
                dtcTd.RowHeaders += PCM.dtcCodes[i].Code + ",";
            }
            dtcTd.RowHeaders = dtcTd.RowHeaders.Trim(',');
            dtcTd.Rows = (ushort)PCM.dtcCodes.Count;
            dtcTd.SavingMath = "X";
            if (PCM.dtcCombined)
            {
                //td.TableDescription = "00 MIL and reporting off, 01 type A/no mil, 02 type B/no mil, 03 type C/no mil, 04 not reported/mil, 05 type A/mil, 06 type B/mil, 07 type c/mil";
                dtcTd.Values = "Enum: 00:MIL and reporting off,01:type A/no mil,02:type B/no mil,03:type C/no mil, 04:not reported/mil,05:type A/mil,06:type B/mil,07:type c/mil";
                dtcTd.TableName = "DTC";
            }
            else
            {
                //td.TableDescription = "0 = 1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY), 1 = 2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles), 2 = Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC), 3 = Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                dtcTd.Values = "Enum: 0:1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY),1:2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles),2:Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC),3:Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                dtcTd.TableName = "DTC.Codes";
            }

            PCM.tableDatas.Insert(0,dtcTd);

            if (!PCM.dtcCombined)
            {
                dtcTd = new TableData();
                dtcTd.TableName = "DTC.MIL_Enable";
                dtcTd.addrInt = dtc.milAddrInt;
                dtcTd.Category = "DTC";
                //td.ColumnHeaders = "MIL";
                dtcTd.Columns = 1;
                dtcTd.OutputType = OutDataType.Flag;
                dtcTd.Decimals = 0;
                dtcTd.DataType = InDataType.UBYTE;
                dtcTd.Math = "X";
                dtcTd.OS = PCM.OS;
                for (int i = 0; i < PCM.dtcCodes.Count; i++)
                {
                    dtcTd.RowHeaders += PCM.dtcCodes[i].Code + ",";
                }
                dtcTd.Rows = (ushort)PCM.dtcCodes.Count;
                dtcTd.SavingMath = "X";
                //td.Signed = false;
                dtcTd.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                //td.Values = "Enum: 0:No MIL (Lamp always off),1:MIL (Lamp may be commanded on by PCM)";
                PCM.tableDatas.Insert(1,dtcTd);
            }
            refreshTablelist();
            Logger("OK");
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
                if (PCM == null || PCM.fsize == 0) return;
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
            Logger(tt.readTinyDBtoTableData(PCM));
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
            LoadXML();
        }

        private void saveXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveXML(currentXmlFile);
        }

        private void saveBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            PCM.saveBin(PCM.FileName);
            Logger("Done.");

        }

        private void importDTCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importDTC();
        }

        private void importTableSeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTableSeek();
        }

        private void importXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XDF xdf = new XDF();
            Logger(xdf.importXdf(PCM));
            LoggerBold("Note: Only basic XDF conversions are supported, check Math and SavingMath values");
            refreshTablelist();
        }

        private void importTinyTunerDBV6OnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importTinyTunerDB();
        }

        private void clearTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PCM.tableDatas = new List<TableData>();
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
                    openTableEditor(null);
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
            openTableEditor(null);
        }

        private void exportCsvToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void importCSVexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PCM.tableDatas.Count; i++)
                PCM.tableDatas[i].addrInt = uint.MaxValue;
            string FileName = SelectFile("Select CSV File","CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Loading file: " + FileName, false);
            //string osNew = Path.GetFileName(FileName).Replace("Addresses-", "").Replace(".csv","");
            string osNew = "12587603";
            StreamReader sr = new StreamReader(FileName);
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
                    for (int i=0; i< PCM.tableDatas.Count;i++)
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

        private void exportXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Generating xdf...");
            XDF xdf = new XDF();
            Logger(xdf.exportXdf(PCM));
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

        private void importCSV2ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void convertToDataTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i=0; i< PCM.tableDatas.Count; i++)
            {
                TableData t = PCM.tableDatas[i];
                t.DataType = convertToDataType(t.ElementSize, t.Signed, t.Floating);
            }
            refreshTablelist();
        }
        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
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

        private void peekTableValues(int ind)
        {
            frmTableEditor frmT = new frmTableEditor();
            frmT.PCM = PCM;
            frmT.disableMultiTable = true;
            frmT.loadTable(PCM.tableDatas[ind]);
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
            txtDescription.SelectionColor = Color.Blue;
            if (PCM.tableDatas[ind].Rows == 1 && PCM.tableDatas[ind].Columns == 1)
            {
                double curVal = frmT.getValue((uint)(PCM.tableDatas[ind].addrInt + PCM.tableDatas[ind].Offset), PCM.tableDatas[ind]);
                UInt64 rawVal = frmT.getRawValue((uint)(PCM.tableDatas[ind].addrInt + PCM.tableDatas[ind].Offset), PCM.tableDatas[ind]);
                string valTxt = curVal.ToString();
                string unitTxt = " " + PCM.tableDatas[ind].Units;
                string maskTxt = "";
                if (PCM.tableDatas[ind].OutputType == OutDataType.Flag || PCM.tableDatas[ind].Units.ToLower().StartsWith("boolean"))
                {
                    unitTxt = ", Unset/Set";
                    if (curVal > 0)
                        valTxt = "Set, " + valTxt;
                    else
                        valTxt = "Unset, " + valTxt;
                    if (PCM.tableDatas[ind].BitMask != null && PCM.tableDatas[ind].BitMask.Length > 0)
                    {
                        unitTxt = "";
                        long maskVal = Convert.ToInt64(PCM.tableDatas[ind].BitMask.Replace("0x", ""), 16);
                        string maskBits = Convert.ToString(maskVal, 2);
                        int bit = -1;
                        for (int i = 0; 1 <= maskBits.Length; i++)
                        {
                            if (((maskVal & (1 << i)) != 0))
                            {
                                bit = i + 1;
                                break;
                            }
                        }
                        if (bit > -1)
                        {
                            string rawBinVal = Convert.ToString((Int64)rawVal, 2);
                            rawBinVal = rawBinVal.PadLeft(getBits(PCM.tableDatas[ind].DataType), '0');
                            maskTxt = " [" + rawBinVal + "], bit $" + bit.ToString();
                        }
                    }
                }
                else if (PCM.tableDatas[ind].Values.StartsWith("Enum: "))
                {
                    Dictionary<double, string> possibleVals = frmT.parseEnumHeaders(PCM.tableDatas[ind].Values.Replace("Enum: ", ""));
                    unitTxt = " (" + possibleVals[curVal] + ")";
                }
                string formatStr = "X" + (getElementSize(PCM.tableDatas[ind].DataType) * 2).ToString();
                txtDescription.AppendText("Current value: " + valTxt + unitTxt + " [" + rawVal.ToString(formatStr) + "]" + maskTxt);
                txtDescription.AppendText(Environment.NewLine);
            }
            else
            {
                string tblData = "Current values: " + Environment.NewLine ;
                uint addr = (uint)(PCM.tableDatas[ind].addrInt + PCM.tableDatas[ind].Offset);
                if (PCM.tableDatas[ind].RowMajor)
                {
                    for (int r=0; r< PCM.tableDatas[ind].Rows; r++)
                    {
                        for (int c=0; c<PCM.tableDatas[ind].Columns; c++)
                        {
                            double curVal = frmT.getValue(addr, PCM.tableDatas[ind]);
                            addr += (uint)getElementSize(PCM.tableDatas[ind].DataType);
                            tblData += "[" + curVal.ToString("#0.0") + "]";
                        }
                        tblData += Environment.NewLine;
                    }
                }
                else
                {
                    List<string> tblRows = new List<string>();
                    for (int r = 0; r < PCM.tableDatas[ind].Rows; r++)
                        tblRows.Add("");
                    for (int c = 0; c < PCM.tableDatas[ind].Columns; c++)
                    {
                        
                        for (int r = 0; r < PCM.tableDatas[ind].Rows; r++)
                        {
                            double curVal = frmT.getValue(addr, PCM.tableDatas[ind]);
                            addr += (uint)getElementSize(PCM.tableDatas[ind].DataType);
                            tblRows[r] += "[" + curVal.ToString("#0.0") + "]";
                        }
                    }
                    for (int r = 0; r < PCM.tableDatas[ind].Rows; r++)
                        tblData += tblRows[r] + Environment.NewLine;
                }
                txtDescription.AppendText(tblData);
            }

        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            txtDescription.Text = "";
            if (dataGridView1.SelectedCells.Count < 1)
            {
                return;
            }
            int ind = Convert.ToInt32(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["id"].Value);
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Bold);
            txtDescription.AppendText(PCM.tableDatas[ind].TableName + Environment.NewLine); 
            txtDescription.SelectionFont = new Font(txtDescription.Font, FontStyle.Regular);
            if (PCM.tableDatas[ind].TableDescription != null)
                txtDescription.AppendText(PCM.tableDatas[ind].TableDescription + Environment.NewLine);
            if (PCM.tableDatas[ind].ExtraDescription != null)
                txtDescription.AppendText(PCM.tableDatas[ind].ExtraDescription + Environment.NewLine);

            peekTableValues(ind);
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void setConfigMode()
        {
            importCSV2ExperimentalToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;
            importCSVexperimentalToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;
            importXMLgeneratorCSVToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;
            exportXMLgeneratorCSVToolStripMenuItem.Visible = enableConfigModeToolStripMenuItem.Checked;     
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

        private void exportXMLgeneratorCSVToolStripMenuItem_Click(object sender, EventArgs e)
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
        struct OsAddrStruct
        {
            public string tableName;
            public string category;
            public string OS;
            public string addr;
        }

        private void importXMLgeneratorCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try 
            {
                List<OsAddrStruct> osAddrList = new List<OsAddrStruct>();
                List<string> osList = new List<string>();

                LoggerBold("Supply CSV file in format: Category;Tablename;Size;OS1Address1;OS2Address2;...");
                LoggerBold("OS versions in header, for example: Category;Tablename;Size;12587603;12582405;12587656;");
                string fName = SelectFile("Select CSV file for XML generator","CSV files (*.csv)|*.csv|ALL files|*.*");
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

                for (int h=4; h < hdrParts.Length; h++)
                    if (hdrParts[h].Length > 2)
                        osList.Add(hdrParts[h]);

                while ((csvLine = sr.ReadLine()) != null )
                {
                    string[] lParts = csvLine.Split(';');
                    if (lParts.Length > 4)
                    {
                        for (int x = 4; x < lParts.Length; x++)
                        {
                            OsAddrStruct oa;
                            oa.tableName = lParts[1];
                            oa.category = lParts[0];
                            oa.OS = osList[x-4];
                            oa.addr = lParts[x];
                            osAddrList.Add(oa);
                        }
                    }
                }
                Logger(" [OK]");
                for (int o=0; o<osList.Count; o++)
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
            SaveXML();
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
            PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
            addtoCurrentFileMenu(newPCM);
            PCM = newPCM;
            loadConfigforPCM();
            selectPCM();
        }

        private void addtoCurrentFileMenu(PcmFile newPCM)
        {
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                mi.Checked = false;
            ToolStripMenuItem menuitem = new ToolStripMenuItem(Path.GetFileName(newPCM.FileName));
            menuitem.Name = Path.GetFileName(newPCM.FileName);
            menuitem.Tag = newPCM;
            menuitem.Checked = true;
            currentFileToolStripMenuItem.DropDownItems.Add(menuitem);
            menuitem.Click += Menuitem_Click;

            ToolStripMenuItem cmpMenuitem = new ToolStripMenuItem(menuitem.Name);
            cmpMenuitem.Name = menuitem.Name;
            cmpMenuitem.Tag = newPCM;
            compareWithToolStripMenuItem.DropDownItems.Add(cmpMenuitem);
            cmpMenuitem.Click += compareMenuitem_Click;

        }
        private void Menuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            bool isChecked = menuitem.Checked;
            foreach (ToolStripMenuItem mi in currentFileToolStripMenuItem.DropDownItems)
                mi.Checked = false;
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
            PcmFile cmpPCM = (PcmFile)menuitem.Tag;
            openTableEditor(cmpPCM);
        }
    }
}
