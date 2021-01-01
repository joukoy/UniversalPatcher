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
            PCM = PCM1;
            this.Text = "Tuner " + Path.GetFileName(PCM.FileName);

            if (upatcher.Segments[0].CS1Address.StartsWith("GM-V6"))
                importTinyTunerDBV6OnlyToolStripMenuItem.Enabled = true;
            else
                importTinyTunerDBV6OnlyToolStripMenuItem.Enabled = false;

            if (!Properties.Settings.Default.disableTunerAutoloadSettings)
            {
                string defaultXml = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                if (File.Exists(defaultXml))
                {
                    LoadXML(defaultXml);
                }
                else
                {
                    Logger("File not found: " + defaultXml);
                    if (dtcCodes.Count > 0)
                        importDTC();
                    if (tableSeeks.Count > 0)
                        importTableSeek();
                }
            }
        }

        private PcmFile PCM;
        private string sortBy = "id";
        private int sortIndex = 0;
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        private BindingList<TableData> filteredCategories = new BindingList<TableData>();

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
                    if (tableDatas[tableIds[i]].Rows != tableDatas[tableIds[0]].Rows || tableDatas[tableIds[i]].Columns != tableDatas[tableIds[0]].Columns)
                    {
                        LoggerBold("Can't load multible tables with different size");
                        return;
                    }
                }
                TableData td = tableDatas[tableIds[0]];
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
                    frmT.Show();
                    frmT.PCM = PCM;
                    frmT.tableIds = tableIds;
                    frmT.disableMultiTable = disableMultitableToolStripMenuItem.Checked;
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
                Logger(PCM.saveBin(fileName));
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
            bindingsource.DataSource = null;
            dataGridView1.DataSource = null;
            bindingsource.DataSource = tableDatas;
            dataGridView1.DataSource = bindingsource;
            UseComboBoxForEnums(dataGridView1);
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
            Application.DoEvents();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

            comboTableCategory.DataSource = null;
            categoryBindingSource.DataSource = null;
            tableCategories.Sort();
            categoryBindingSource.DataSource = tableCategories;
            comboTableCategory.DataSource = categoryBindingSource;
            comboTableCategory.Refresh();

            comboFilterBy.Items.Clear();
            for (int col=0;col<dataGridView1.Columns.Count;col++)
            {
                comboFilterBy.Items.Add(dataGridView1.Columns[col].HeaderText);
            }
            comboFilterBy.Text = "TableName";
        }

        private void importTableSeek()
        {
            Logger("Importing TableSeek tables... ", false);
            for (int i = 0; i < foundTables.Count; i++)
            {
                TableData tableData = new TableData();
                tableData.importFoundTable(i, PCM);
                tableDatas.Add(tableData);
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
                Logger("Loading file: " + fName);
                if (fName.Length == 0)
                    return;
                if (File.Exists(fName))
                {
                    Debug.WriteLine("Loading " + fName + "...");
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    System.IO.StreamReader file = new System.IO.StreamReader(fName);
                    tableDatas = (List<TableData>)reader.Deserialize(file);
                    file.Close();
                }
                for (int t = 0; t < tableDatas.Count; t++)
                {
                    string category = tableDatas[t].Category;
                    if (!tableCategories.Contains(category))
                        tableCategories.Add(category);
                }
                Logger("OK");
                refreshTablelist();
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

        private void SaveXML()
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                string fName = SelectSaveFile("XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
                if (fName.Length == 0)
                    return;

                Logger("Saving file " + fName + "...", false);
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, tableDatas);
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
            Logger("Importing DTC codes... ", false);
            TableData td = new TableData();
            dtcCode dtc = dtcCodes[0];
            //td.Address = dtc.StatusAddr;
            td.addrInt = dtc.statusAddrInt;
            td.Category = "DTC";
            //td.ColumnHeaders = "Status";
            td.Columns = 1;
            //td.Floating = false;
            td.OutputType = OutDataType.Int;
            td.Decimals = 0;
            //td.ElementSize = 1; // (byte)(dtcCodes[1].codeAddrInt - dtcCodes[0].codeAddrInt);
            td.DataType = InDataType.UBYTE;
            td.Math = "X";
            td.OS = PCM.OS;
            for (int i = 0; i < dtcCodes.Count; i++)
            {
                td.RowHeaders += dtcCodes[i].Code + ",";
            }
            td.RowHeaders = td.RowHeaders.Trim(',');
            td.Rows = (ushort)dtcCodes.Count;
            td.SavingMath = "X";
            //td.Signed = false;
            if (dtcCombined)
            {
                //td.TableDescription = "00 MIL and reporting off, 01 type A/no mil, 02 type B/no mil, 03 type C/no mil, 04 not reported/mil, 05 type A/mil, 06 type B/mil, 07 type c/mil";
                td.Values = "Enum: 00:MIL and reporting off,01:type A/no mil,02:type B/no mil,03:type C/no mil, 04:not reported/mil,05:type A/mil,06:type B/mil,07:type c/mil";
                td.TableName = "DTC";
            }
            else
            {
                //td.TableDescription = "0 = 1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY), 1 = 2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles), 2 = Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC), 3 = Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                td.Values = "Enum: 0:1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY),1:2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles),2:Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC),3:Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                td.TableName = "DTC.Codes";
            }

            tableDatas.Add(td);

            if (!dtcCombined)
            {
                td = new TableData();
                td.TableName = "DTC.MIL_Enable";
                //td.Address = dtc.MilAddr;
                td.addrInt = dtc.milAddrInt;
                td.Category = "DTC";
                //td.ColumnHeaders = "MIL";
                td.Columns = 1;
                //td.Floating = false;
                td.OutputType = OutDataType.Flag;
                td.Decimals = 0;
                //td.ElementSize = 1; // (byte)(dtcCodes[1].milAddrInt - dtcCodes[0].milAddrInt);
                td.DataType = InDataType.UBYTE;
                td.Math = "X";
                td.OS = PCM.OS;
                for (int i = 0; i < dtcCodes.Count; i++)
                {
                    td.RowHeaders += dtcCodes[i].Code + ",";
                }
                td.Rows = (ushort)dtcCodes.Count;
                td.SavingMath = "X";
                //td.Signed = false;
                td.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                //td.Values = "Enum: 0:No MIL (Lamp always off),1:MIL (Lamp may be commanded on by PCM)";
                tableDatas.Add(td);
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
            }

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
                Properties.Settings.Default.Save();
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tableDatas = new List<TableData>();
            refreshTablelist();
        }

        private void filterTables()
        {
            try
            {

                string strColumnName = sortBy;
                SortOrder strSortOrder = getSortOrder(sortIndex);
                List<TableData> compareList = new List<TableData>();
                if (strSortOrder == SortOrder.Ascending)
                {
                    compareList = tableDatas.OrderBy(x => typeof(TableData).GetProperty(strColumnName).GetValue(x, null)).ToList();
                }
                else
                {
                    compareList = tableDatas.OrderByDescending(x => typeof(TableData).GetProperty(strColumnName).GetValue(x, null)).ToList();
                }

                string cat = comboTableCategory.Text;
                var results = compareList.Where(t => t.TableName.Length > 0); //How should I define empty variable??
                if (txtSearchTableSeek.Text.Length > 0)
                    results = results.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t,null).ToString().ToLower().Contains(txtSearchTableSeek.Text.ToLower()));
                if (!showTablesWithEmptyAddressToolStripMenuItem.Checked)
                    results = results.Where(t => t.addrInt < uint.MaxValue);
                if (cat != "_All" && cat != "")
                    results = results.Where(t => t.Category.ToLower().Contains(comboTableCategory.Text.ToLower()));
                filteredCategories = new BindingList<TableData>(results.ToList());
                bindingsource.DataSource = filteredCategories;
                dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
            }
            catch { }

        }

        private void comboTableCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                        //dataGridTableSeek.Rows[i].Cells[0].Selected = true;                    
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
            SaveXML();
        }

        private void saveBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            Logger(PCM.saveBin(PCM.FileName));
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
            tableDatas = new List<TableData>();
            refreshTablelist();
        }


        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
                dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            openTableEditor();
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
            string FileName = SelectFile("Select CSV File","CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Loading file: " + FileName, false);
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
                    for (int i=0; i< tableDatas.Count;i++)
                    {
                        if (tableDatas[i].Category.ToLower() == cat.ToLower() && tableDatas[i].TableName.ToLower() == name.ToLower())
                        {
                            tableDatas[i].Address = addr;
                            //tableDatas[i].AddrInt = Convert.ToUInt32(addr, 16);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Debug.WriteLine(name + ": not found");
                        for (int i = 0; i < tableDatas.Count; i++)
                        {
                            if (cat.ToLower() == "protected" && tableDatas[i].TableName.ToLower() == name.ToLower())
                            {
                                tableDatas[i].Address = addr;
                                //tableDatas[i].AddrInt = Convert.ToUInt32(addr, 16);
                                found = true;
                                Debug.WriteLine(name + ": PROTECTED");
                                break;
                            }
                        }
                    }
                }
            }
            Logger(" [OK]");
            refreshTablelist();
        }

        private void exportXDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    uint lastmask = uint.MaxValue;
                    uint addrInt = Convert.ToUInt32(addr, 16);
                    for (int i = 0; i < tableDatas.Count; i++)
                    {
                        if (tableDatas[i].Category.ToLower() == cat.ToLower() && tableDatas[i].TableName.ToLower().StartsWith(name.ToLower()))
                        {
                            if (name == "K_DYNA_AIR_COEFFICIENT")
                            {
                                //tableDatas[i].Address = addrInt.ToString("X8");
                                tableDatas[i].addrInt = addrInt;
                                Debug.WriteLine(tableDatas[i].TableName + ": " + addrInt.ToString("X"));
                                addrInt += 2;
                            }
                            else
                            {
                                uint mask = Convert.ToUInt32(tableDatas[i].BitMask, 16);
                                if (lastmask == uint.MaxValue)
                                    lastmask = mask;
                                if (mask > lastmask)
                                {
                                    addrInt++;
                                }
                                lastmask = mask;
                                //tableDatas[i].Address = addrInt.ToString("X8");
                                tableDatas[i].addrInt = addrInt;
                                Debug.WriteLine(tableDatas[i].TableName + ": " + addrInt.ToString("X") + " mask: " + mask.ToString("X"));
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
            for (int i=0; i< tableDatas.Count; i++)
            {
                TableData t = tableDatas[i];
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
            sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
            sortIndex = e.ColumnIndex;
            filterTables();
        }

        private SortOrder getSortOrder(int columnIndex)
        {
            if (dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending 
                ||dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
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
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewColumn column in ((DataGridView)sender).Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

    }
}
