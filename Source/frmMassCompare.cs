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
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmMassCompare : Form
    {
        public frmMassCompare()
        {
            InitializeComponent();
        }

        public PcmFile PCM;
        public TableData td;
        public bool compareAll = false;

        public class DgvRow
        {
            public int id;
            public List<DataGridViewRow> gdvrows = new List<DataGridViewRow>();
        }
        private List<DgvRow> tableDgvRows = new List<DgvRow>();
        private void frmMassCompare_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.MassCompareWindowSize.Width > 0 || Properties.Settings.Default.MassCompareWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.MassCompareWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.MassCompareWindowLocation;
                    this.Size = Properties.Settings.Default.MassCompareWindowSize;
                }
                if (Properties.Settings.Default.MassCompareSplitWidth > 0)
                    splitContainer1.SplitterDistance = Properties.Settings.Default.MassCompareSplitWidth;
            }

            setupDataGrid();
        }
        private void FrmMassCompare_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.MassCompareWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.MassCompareWindowLocation = this.Location;
                    Properties.Settings.Default.MassCompareWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.MassCompareWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.MassCompareWindowSize = this.RestoreBounds.Size;
                }
                if (!compareAll)
                    Properties.Settings.Default.MassCompareSplitWidth = splitContainer1.SplitterDistance;
            }

        }

        private string peekTableValues(int ind, PcmFile peekPCM)
        {
            string retVal = "";
            try
            {
                frmTableEditor frmT = new frmTableEditor();
                frmT.PCM = peekPCM;
                frmT.disableMultiTable = true;
                frmT.loadTable(peekPCM.tableDatas[ind]);
                //txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
                //txtResult.SelectionColor = Color.Blue;
                if (peekPCM.tableDatas[ind].Rows == 1 && peekPCM.tableDatas[ind].Columns == 1)
                {
                    double curVal = frmT.getValue((uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset), peekPCM.tableDatas[ind]);
                    UInt64 rawVal = frmT.getRawValue((uint)(peekPCM.tableDatas[ind].addrInt + peekPCM.tableDatas[ind].Offset), peekPCM.tableDatas[ind]);
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
                        Dictionary<double, string> possibleVals = frmT.parseEnumHeaders(peekPCM.tableDatas[ind].Values.Replace("Enum: ", ""));
                        if (possibleVals.ContainsKey(curVal))
                            unitTxt = " (" + possibleVals[curVal] + ")";
                        else
                            unitTxt = " (Out of range)";
                    }
                    string formatStr = "X" + (getElementSize(peekPCM.tableDatas[ind].DataType) * 2).ToString();
                    retVal = valTxt + unitTxt + " [" + rawVal.ToString(formatStr) + "]" + maskTxt;
                    //txtResult.AppendText(Environment.NewLine);
                }
                else
                {
                    string tblData = ""; //"Current values: " + Environment.NewLine;
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
                    retVal = tblData;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return retVal;
        }

        private void setupDataGrid()
        {
            //dataGridView1.Columns.Add("File", "File");
            dataGridView1.Columns.Add("OS", "OS");
            dataGridView1.Columns.Add("Segment", "Segment");
            dataGridView1.Columns.Add("PN", "PN");
            dataGridView1.Columns.Add("Table Name", "Table Name");
            dataGridView1.Columns.Add("Table Address", "Table Address");
            dataGridView1.Columns.Add("Stock", "Stcok");
            dataGridView1.Columns.Add("Current Value", "Current Value");
            dataGridView1.Columns["Current Value"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.RowHeadersWidth = 200;
        }

        private void DataGridViewTableList_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewTableList.SelectedCells.Count == 0)
                    return;
                if (dataGridViewTableList.Rows[dataGridViewTableList.SelectedCells[0].RowIndex].Tag == null)
                    return;
                dataGridView1.Rows.Clear();
                int id = (int)dataGridViewTableList.Rows[dataGridViewTableList.SelectedCells[0].RowIndex].Tag;
                for (int r = 0; r < tableDgvRows[id].gdvrows.Count; r++)
                    dataGridView1.Rows.Add(tableDgvRows[id].gdvrows[r]);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void compareTable(PcmFile cmpPCM)
        {
            int id = findTableDataId(td, cmpPCM.tableDatas);
            if (id < 0)
            {
                Logger("Table not found");
                return;
            }
            TableData cmpTd = cmpPCM.tableDatas[id];
            int row = dataGridView1.Rows.Add();
            dataGridView1.Rows[row].HeaderCell.Value = cmpPCM.FileName;
            dataGridView1.Rows[row].Cells["OS"].Value = cmpPCM.OS;
            dataGridView1.Rows[row].Cells["Segment"].Value = cmpPCM.GetSegmentName(cmpTd.addrInt);
            int segNr = cmpPCM.GetSegmentNumber(cmpTd.addrInt);
            if (segNr > -1)
            {
                dataGridView1.Rows[row].Cells["PN"].Value = cmpPCM.segmentinfos[segNr].PN;
                dataGridView1.Rows[row].Cells["Stock"].Value = cmpPCM.segmentinfos[segNr].Stock;
            }
            dataGridView1.Rows[row].Cells["Table Name"].Value = cmpTd.TableName;
            dataGridView1.Rows[row].Cells["Table Address"].Value = cmpTd.Address;
            dataGridView1.Rows[row].Cells["Current Value"].Value = peekTableValues(id, cmpPCM);
            //dataGridView1.Rows[row].Height = 50;
        }


        private void loadConfigforPCM(PcmFile cmpPCM)
        {
            if (!Properties.Settings.Default.disableTunerAutoloadSettings)
            {
                string defaultXml = Path.Combine(Application.StartupPath, "Tuner", cmpPCM.OS + ".xml");
                if (File.Exists(defaultXml))
                {
                    cmpPCM.LoadTableList(defaultXml);
                    bool haveDTC = false;
                    for (int t = 0; t < cmpPCM.tableDatas.Count; t++)
                    {
                        if (cmpPCM.tableDatas[t].TableName.StartsWith("DTC"))
                        {
                            haveDTC = true;
                            break;
                        }
                    }
                    if (!haveDTC)
                        importDTC(cmpPCM);
                }
                else
                {
                    Logger("File not found: " + defaultXml);
                    importDTC(cmpPCM);
                    importTableSeek(cmpPCM);
                }
            }

        }
        private void importTableSeek(PcmFile cmpPCM)
        {
            if (cmpPCM.foundTables.Count == 0)
            {
                TableSeek TS = new TableSeek();
                Logger("Seeking tables...", false);
                Logger(TS.seekTables(cmpPCM));
            }
            Logger("Importing TableSeek tables... ", false);
            for (int i = 0; i < cmpPCM.foundTables.Count; i++)
            {
                TableData tableData = new TableData();
                tableData.importFoundTable(i, cmpPCM);
                cmpPCM.tableDatas.Add(tableData);
            }
            Logger("OK");
        }

        private void importDTC(PcmFile cmpPCM)
        {
            Logger("Importing DTC codes... ", false);
            TableData tdTmp = new TableData();
            tdTmp.importDTC(cmpPCM, ref cmpPCM.tableDatas);
            Logger(" [OK]");
        }

        private void compareAllTables(List<string> files)
        {
            dataGridViewTableList.SelectionChanged -= DataGridViewTableList_SelectionChanged;
            List<PcmFile> pcmfiles = new List<PcmFile>();
            //Load all files:
            for (int i=0; i < files.Count; i++)
            {
                string fName = files[i];
                LoggerBold(fName);
                PcmFile cmpPCM = new PcmFile(fName, true, "");
                loadConfigforPCM(cmpPCM);
                pcmfiles.Add(cmpPCM);
            }
            Logger("Reading tables ", false);
            for (int i = 0; i < PCM.tableDatas.Count; i++)
            {
                td = PCM.tableDatas[i];
                int row = dataGridViewTableList.Rows.Add();
                dataGridViewTableList.Rows[row].Cells[0].Value = td.TableName;
                dataGridViewTableList.Rows[row].Cells[1].Value = td.Category;
                dataGridView1.Rows.Clear();
                for (int p = 0; p < pcmfiles.Count; p++)
                {
                    PcmFile cmpPCM = pcmfiles[p];
                    int id = findTableDataId(td, cmpPCM.tableDatas);
                    if (id < 0)
                    {
                        continue;
                    }
                    compareTable(cmpPCM);
                    dataGridViewTableList.Rows[row].Cells[2].Value = Convert.ToInt32(dataGridViewTableList.Rows[row].Cells[2].Value) + 1;
                }

                DgvRow dgvRow = new DgvRow();
                for (int r=0; r < dataGridView1.Rows.Count; r++)
                {
                    dgvRow.id = r;
                    dgvRow.gdvrows.Add(dataGridView1.Rows[r]);
                }
                tableDgvRows.Add(dgvRow);
                dataGridViewTableList.Rows[row].Tag = tableDgvRows.Count - 1;
                if ((i % 10) == 0)
                    Logger(".", false);
            }
            dataGridViewTableList.SelectionChanged += DataGridViewTableList_SelectionChanged;
        }

        private void initCompareAll()
        {
            dataGridViewTableList.Rows.Clear();
            dataGridViewTableList.Columns.Clear();
            dataGridViewTableList.Columns.Add("Table Name", "Table Name");
            dataGridViewTableList.Columns.Add("Category", "Category");
            dataGridViewTableList.Columns.Add("Hits", "Hits");
        }
        public void selectCmpFiles()
        {
            if (!compareAll)
            {
                splitContainer1.Panel1Collapsed = true;
                splitContainer1.Panel1.Hide();
            }
            frmFileSelection frmF = new frmFileSelection();
            frmF.btnOK.Text = "Compare files";
            frmF.Text = "Search and Compare: " + td.TableName;
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                if (compareAll)
                {
                    List<string> files = new List<string>();
                    for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                    {
                        string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                        files.Add(FileName);
                    }
                    initCompareAll();
                    compareAllTables(files);
                }
                else
                {
                    for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                    {
                        string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                        PcmFile cmpPcm = new PcmFile(FileName,true,"");
                        LoggerBold(FileName);
                        loadConfigforPCM(cmpPcm);
                        compareTable(cmpPcm);
                    }
                }

            }
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            Logger("Done");
        }
        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            selectCmpFiles();
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
        private void saveCSV()
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "File;";
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    row += dataGridView1.Columns[i].HeaderText + ";";
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                {
                    row = dataGridView1.Rows[r].HeaderCell.Value.ToString();
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (dataGridView1.Rows[r].Cells[i].Value != null)
                            row += dataGridView1.Rows[r].Cells[i].Value.ToString();
                        row += ";";
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private void btnSaveCsv_Click(object sender, EventArgs e)
        {
            saveCSV();
        }

        private void dataGridViewTableList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView1.Rows.Clear();
                int id = (int)dataGridViewTableList.Rows[e.RowIndex].Tag;
                for (int r = 0; r < tableDgvRows[id].gdvrows.Count; r++)
                    dataGridView1.Rows.Add(tableDgvRows[id].gdvrows[r]);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
    }
}
