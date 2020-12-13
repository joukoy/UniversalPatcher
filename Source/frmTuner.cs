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

namespace UniversalPatcher
{
    public partial class frmTuner : Form
    {
        public frmTuner(PcmFile PCM1)
        {
            InitializeComponent();
            PCM = PCM1;
            if (upatcher.Segments[0].CS1Address.StartsWith("GM-V6"))
                importTinyTunerDBV6OnlyToolStripMenuItem.Enabled = true;
            else
                importTinyTunerDBV6OnlyToolStripMenuItem.Enabled = false;
        }

        private PcmFile PCM;
        
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        private BindingList<TableData> filteredCategories = new BindingList<TableData>();

        private void openTableEditor()
        {
            try
            {

                int rowindex = dataGridView1.CurrentCell.RowIndex;
                int columnindex = dataGridView1.CurrentCell.ColumnIndex;
                int codeIndex = Convert.ToInt32(dataGridView1.Rows[rowindex].Cells["id"].Value);
                frmTableEditor frmT = new frmTableEditor();
                TableData td = tableDatas[codeIndex];
                if (td.Address == null || td.Address == "")
                {
                    Logger("No address defined!");
                    return;
                }

                if (td.OS != PCM.OS)
                {
                    LoggerBold("WARING! OS Mismatch, File OS: " + PCM.OS + ", config OS: " + td.OS);
                }
                if (td.OutputType == DataType.Flag && td.BitMask != null && td.BitMask.Length > 0)
                {
                    frmEditFlag ff = new frmEditFlag();
                    ff.loadFlag(PCM, td);
                    ff.Show();
                }
                else
                {
                    frmT.loadTable(td, PCM);
                    frmT.Show();
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
                        string Ret = GmEeprom.FixEepromKey(PCM.buf);
                        if (Ret.Contains("Fixed"))
                            NeedFix = true;
                        Logger(Ret);
                    }
                    else
                    {
                        if (S.CS1Method != CSMethod_None)
                        {
                            uint CS1 = 0;
                            uint CS1Calc = CalculateChecksum(PCM.buf, PCM.segmentAddressDatas[i].CS1Address, PCM.segmentAddressDatas[i].CS1Blocks, PCM.segmentAddressDatas[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, PCM.segmentAddressDatas[i].CS1Address.Bytes, S.CS1SwapBytes);
                            if (PCM.segmentAddressDatas[i].CS1Address.Address < uint.MaxValue)
                            {
                                if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 1)
                                {
                                    CS1 = PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address];
                                }
                                else if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 2)
                                {
                                    CS1 = BEToUint16(PCM.buf, PCM.segmentAddressDatas[i].CS1Address.Address);
                                }
                                else if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 4)
                                {
                                    CS1 = BEToUint32(PCM.buf, PCM.segmentAddressDatas[i].CS1Address.Address);
                                }
                            }
                            if (CS1 == CS1Calc)
                                Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                            else
                            {
                                if (PCM.segmentAddressDatas[i].CS1Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (PCM.segmentAddressDatas[i].CS1Address.Bytes * 2).ToString();
                                    Logger(" Checksum 1: " + CS1Calc.ToString(hexdigits) + " [Not saved]");
                                }
                                else
                                {
                                    if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 1)
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address] = (byte)CS1Calc;
                                    else if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 2)
                                    {
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address + 1] = (byte)(CS1Calc & 0xFF);
                                    }
                                    else if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 4)
                                    {
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF000000) >> 24);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address + 1] = (byte)((CS1Calc & 0xFF0000) >> 16);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address + 2] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS1Address.Address + 3] = (byte)(CS1Calc & 0xFF);

                                    }
                                    Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                    NeedFix = true;
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None)
                        {
                            uint CS2 = 0;
                            uint CS2Calc = CalculateChecksum(PCM.buf, PCM.segmentAddressDatas[i].CS2Address, PCM.segmentAddressDatas[i].CS2Blocks, PCM.segmentAddressDatas[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, PCM.segmentAddressDatas[i].CS2Address.Bytes, S.CS2SwapBytes);
                            if (PCM.segmentAddressDatas[i].CS2Address.Address < uint.MaxValue)
                            {
                                if (PCM.segmentAddressDatas[i].CS2Address.Bytes == 1)
                                {
                                    CS2 = PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address];
                                }
                                else if (PCM.segmentAddressDatas[i].CS2Address.Bytes == 2)
                                {
                                    CS2 = BEToUint16(PCM.buf, PCM.segmentAddressDatas[i].CS2Address.Address);
                                }
                                else if (PCM.segmentAddressDatas[i].CS2Address.Bytes == 4)
                                {
                                    CS2 = BEToUint32(PCM.buf, PCM.segmentAddressDatas[i].CS2Address.Address);
                                }
                            }
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                            else
                            {
                                if (PCM.segmentAddressDatas[i].CS2Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (PCM.segmentAddressDatas[i].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (PCM.segmentAddressDatas[i].CS2Address.Bytes * 2).ToString();
                                    Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    if (PCM.segmentAddressDatas[i].CS2Address.Bytes == 1)
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address] = (byte)CS2Calc;
                                    else if (PCM.segmentAddressDatas[i].CS2Address.Bytes == 2)
                                    {
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address + 1] = (byte)(CS2Calc & 0xFF);
                                    }
                                    else if (PCM.segmentAddressDatas[i].CS2Address.Bytes == 4)
                                    {
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF000000) >> 24);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address + 1] = (byte)((CS2Calc & 0xFF0000) >> 16);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address + 2] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.segmentAddressDatas[i].CS2Address.Address + 3] = (byte)(CS2Calc & 0xFF);

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
        private void SaveBin()
        {
            try
            {
                if (PCM == null || PCM.buf == null | PCM.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string FileName = SelectSaveFile("BIN files (*.bin)|*.bin|ALL files(*.*)|*.*");
                if (FileName.Length == 0)
                    return;

                FixCheckSums();
                Logger("Saving to file: " + FileName);
                WriteBinToFile(FileName, PCM.buf);
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
            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            dataGridView1.Columns["OutputType"].ToolTipText = "1=Float, 2=Int, 3=Hex, 4=Text, 5=Flag";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            for (int i=0; i< dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["OutputType"].ToolTipText = "1=Float, 2=Int, 3=Hex, 4=Text, 5=Flag";
                if (dataGridView1.Rows[i].Cells["TableDescription"].Value != null)
                    dataGridView1.Rows[i].Cells["TableName"].ToolTipText = dataGridView1.Rows[i].Cells["TableDescription"].Value.ToString();
            }
            Application.DoEvents();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

            comboTableCategory.DataSource = null;
            categoryBindingSource.DataSource = null;
            tableCategories.Sort();
            categoryBindingSource.DataSource = tableCategories;
            comboTableCategory.DataSource = categoryBindingSource;
            comboTableCategory.Refresh();

        }

        private void importTableSeek()
        {
            for (int i = 0; i < foundTables.Count; i++)
            {
                TableData tableData = new TableData();
                tableData.importFoundTable(i, PCM);
                tableDatas.Add(tableData);
            }
            refreshTablelist();

        }

        private void LoadXML()
        {
            try
            {

                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                //string defName = PCM.OS + ".xml";
                string fName = SelectFile("Select XML File", "XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
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
            TableData td = new TableData();
            dtcCode dtc = dtcCodes[0];
            td.Address = dtc.StatusAddr;
            td.AddrInt = dtc.statusAddrInt;
            td.Category = "DTC";
            td.ColumnHeaders = "Status";
            td.Columns = 1;
            td.Floating = false;
            td.OutputType = DataType.Int;
            td.Decimals = 0;
            td.ElementSize = 1; // (byte)(dtcCodes[1].codeAddrInt - dtcCodes[0].codeAddrInt);
            td.Math = "X";
            td.OS = PCM.OS;
            for (int i = 0; i < dtcCodes.Count; i++)
            {
                td.RowHeaders += dtcCodes[i].Code + ",";
            }
            td.RowHeaders = td.RowHeaders.Trim(',');
            td.Rows = (ushort)dtcCodes.Count;
            td.SavingMath = "X";
            td.Signed = false;
            if (dtcCombined)
                td.TableDescription = "00 MIL and reporting off, 01 type A/no mil, 02 type B/no mil, 03 type C/no mil, 04 not reported/mil, 05 type A/mil, 06 type B/mil, 07 type c/mil";
            else
                td.TableDescription = "0 = 1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY), 1 = 2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles), 2 = Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC), 3 = Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
            td.TableName = "DTC";

            tableDatas.Add(td);

            if (!dtcCombined)
            {
                td = new TableData();
                td.TableName = "DTC MIL";
                td.Address = dtc.MilAddr;
                td.AddrInt = dtc.milAddrInt;
                td.Category = "DTC";
                td.ColumnHeaders = "MIL";
                td.Columns = 1;
                td.Floating = false;
                td.OutputType = DataType.Int;
                td.Decimals = 0;
                td.ElementSize = 1; // (byte)(dtcCodes[1].milAddrInt - dtcCodes[0].milAddrInt);
                td.Math = "X";
                td.OS = PCM.OS;
                for (int i = 0; i < dtcCodes.Count; i++)
                {
                    td.RowHeaders += dtcCodes[i].Code + ",";
                }
                td.Rows = (ushort)dtcCodes.Count;
                td.SavingMath = "X";
                td.Signed = false;
                td.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                tableDatas.Add(td);
            }
            refreshTablelist();

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
                string cat = comboTableCategory.Text;
                var results = tableDatas.Where(t => t.TableName.Length > 0); //How should I define empty variable??
                if (!showTablesWithEmptyAddressToolStripMenuItem.Checked)
                    results = results.Where(t => t.Address.Length > 0);
                if (cat != "_All" && cat != "")
                    results = results.Where(t => t.Category.ToLower().Contains(comboTableCategory.Text.ToLower()));
                if (txtSearchTableSeek.Text.Length > 0)
                    results = results.Where(t => t.TableName.ToLower().Contains(txtSearchTableSeek.Text.ToLower()));
                filteredCategories = new BindingList<TableData>(results.ToList());
                bindingsource.DataSource = filteredCategories;
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
            SaveBin();
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
                            tableDatas[i].AddrInt = Convert.ToUInt32(addr, 16);
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
                                tableDatas[i].AddrInt = Convert.ToUInt32(addr, 16);
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
    }
}
