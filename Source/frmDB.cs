using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmDB : Form
    {
        public frmDB()
        {
            InitializeComponent();
        }

        private DataTable cvnTable;
        OleDbCommandBuilder builder;
        private string sortBy = "pn";
        private int sortIndex = 0;
        SortOrder strSortOrder = SortOrder.Ascending;
        bool dataModified = false;
        bool useStockDB = true;

        private void frmDB_Load(object sender, EventArgs e)
        {
            cvnTable = cvnDB.stockCvn;
            this.FormClosing += FrmDB_FormClosing;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            txtFilter.KeyUp += TxtFilter_KeyUp;
            builder = new OleDbCommandBuilder();
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataModified = true;
        }

        private void fillFilterBy()
        {
            comboFilterBy.Items.Clear();
            comboFilterBy.Text = "pn";
            if (useStockDB)
            {
                CVN tmpCvn = new CVN();
                foreach (var prop in tmpCvn.GetType().GetProperties())
                    comboFilterBy.Items.Add(prop.Name.ToLower());
            }
            else
            {
                comboFilterBy.Items.Add("pn");
                comboFilterBy.Items.Add("cvn");
            }
        }

        private void TxtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            filterRows();
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = getSortOrder(sortIndex);
                filterRows();
            }
        }
        private SortOrder getSortOrder(int columnIndex)
        {
            try
            {
                /*                if (dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                                    || dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                                {
                                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                                    return SortOrder.Ascending;
                                }
                                else
                                {
                                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                                    return SortOrder.Descending;
                                }*/
                if (strSortOrder == SortOrder.Ascending)
                    return SortOrder.Descending;
                else
                    return SortOrder.Ascending;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return SortOrder.Ascending;
        }

        private bool askCommit()
        {
            if (dataModified)
            {
                DialogResult res = MessageBox.Show("Commit changes?\n\r If no, previous modifications will be lost", "Commit changes?", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                {
                    commitDB();
                }
                else if (res == DialogResult.Cancel)
                    return false;
            }
            return true;

        }
        private void FrmDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dataModified)
            {
                DialogResult res = MessageBox.Show("Save?", "Save?", MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                {
                    commitDB();
                }
                else if (res == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void filterRows()
        {
            try
            {
                DataTable dt = new DataTable();
                if (useStockDB)
                    dt = cvnDB.stockCvn;
                else
                    dt = cvnDB.refCvn;
                string expression = "";
                if (txtFilter.Text.Length > 0)
                    expression = "[" + comboFilterBy.Text + "] LIKE '%" + txtFilter.Text + "%'";
                string sortorder = "[cvn] ASC";
                if (strSortOrder == SortOrder.Ascending)
                    sortorder = "[" + sortBy + "] ASC";
                else
                    sortorder = "[" + sortBy + "] DESC";
                DataView dv = new DataView(dt, expression, sortorder, DataViewRowState.CurrentRows);

                cvnTable = dv.ToTable();
                dataGridView1.DataSource = cvnTable;
                dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void loadDB(string[] tableNames)
        {
            try
            {
                dataGridView1.DataSource = cvnTable;
                comboTable.DataSource = tableNames;
                filterRows();
                labelStatus.Text = cvnTable.Rows.Count.ToString() + " Records";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }
        }

        private void reloadDB()
        {
            try
            {
                dataGridView1.DataSource = null;
                askCommit();
                if (comboTable.Text == "cvn")
                {
                    cvnTable = cvnDB.stockCvn;
                    builder = new OleDbCommandBuilder(cvnDB.stockAdapter);
                    btnImportCSV.Enabled = false;
                    useStockDB = true;
                }
                else
                {
                    btnImportCSV.Enabled = true;
                    useStockDB = false;
                    cvnTable = cvnDB.refCvn;
                    builder = new OleDbCommandBuilder(cvnDB.refAdapter);
                }
                dataGridView1.DataSource = cvnTable;
                fillFilterBy();
                filterRows();
                labelStatus.Text = cvnTable.Rows.Count.ToString() + " Records";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }

        }

        private void comboTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            reloadDB();
        }

        private void commitDB()
        {
            try
            {
                if (dataModified)
                {
                    labelStatus.Text = "Updating database...";
                    Application.DoEvents();
                    if (useStockDB)
                    {
                        cvnDB.stockAdapter.InsertCommand = builder.GetInsertCommand();
                        cvnDB.stockAdapter.DeleteCommand = builder.GetDeleteCommand();
                        cvnDB.stockAdapter.UpdateCommand = builder.GetDeleteCommand();
                    }
                    else
                    {
                        cvnDB.refAdapter.InsertCommand = builder.GetInsertCommand();
                        cvnDB.refAdapter.DeleteCommand = builder.GetDeleteCommand();
                        cvnDB.refAdapter.UpdateCommand = builder.GetDeleteCommand();
                    }
                    cvnDB.stockAdapter.Update(cvnTable);
                    labelStatus.Text = cvnTable.Rows.Count + " Records";
                    if (useStockDB)
                        builder = new OleDbCommandBuilder(cvnDB.stockAdapter);
                    else
                        builder = new OleDbCommandBuilder(cvnDB.refAdapter);

                    dataModified = false;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }

        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            commitDB();
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            try
            {
                if (builder != null)
                    cvnDB.stockAdapter.Dispose();
                builder = null;
                reloadDB();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }
        }

        private void btnImportXml_Click(object sender, EventArgs e)
        {
            try
            {
                labelStatus.Text = "Importing xml...";
                Application.DoEvents();
                Application.DoEvents();

                dataModified = true;
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                //builder = new OleDbCommandBuilder(cvnDB.stockAdapter);
                if (comboTable.Text == "cvn")
                {
                    List<CVN> stockList = new List<CVN>(); ;
                    string defFile = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                    string StockCVNFile = SelectFile("Select Stock CVN file", "XML (*.xml)|*.xml|ALL (*.*)|*.*", defFile);
                    if (StockCVNFile.Length > 0 && File.Exists(StockCVNFile))
                    {
                        Debug.WriteLine("Loading stockcvn.xml");
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                        System.IO.StreamReader file = new System.IO.StreamReader(StockCVNFile);
                        stockList = (List<CVN>)reader.Deserialize(file);
                        file.Close();
                    }
                    for (int i=0;  i<stockList.Count;i++)
                    {

                        CVN cvn = stockList[i];

                        DataRow newRow = cvnTable.NewRow();
                        foreach (var prop in cvn.GetType().GetProperties())
                        {
                            var val = prop.GetValue(cvn, null);
                            if (val != null && val.ToString().Length > 0)
                            {
                                if (prop.Name.ToLower() == "segmentnr")
                                {
                                    int cVal;
                                    if (!HexToInt(prop.GetValue(cvn, null).ToString(), out cVal))
                                        continue;
                                    newRow[prop.Name.ToLower()] = cVal;
                                }
                                else
                                {
                                    newRow[prop.Name.ToLower()] = prop.GetValue(cvn, null);
                                }
                            }
                        }
                        cvnTable.Rows.Add(newRow);
                        if (i % 100 == 0)
                        {
                            labelStatus.Text = "Importing xml... (" + i + " records)";
                            Application.DoEvents();
                        }

                    }
                    cvnDB.stockAdapter.InsertCommand = builder.GetInsertCommand();
                }
                else
                {
                    List<referenceCvn> refList = new List<referenceCvn>(); ;
                    string defFile = Path.Combine(Application.StartupPath, "XML", "referencecvn.xml");
                    string refCVNFile = SelectFile("Select refrence CVN file", "XML (*.xml)|*.xml|ALL (*.*)|*.*", defFile);
                    if (refCVNFile.Length > 0 && File.Exists(refCVNFile))
                    {
                        Debug.WriteLine("Loading stockcvn.xml");
                        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<referenceCvn>));
                        System.IO.StreamReader file = new System.IO.StreamReader(refCVNFile);
                        refList = (List<referenceCvn>)reader.Deserialize(file);
                        file.Close();
                    }

                    for (int i = 0; i < refList.Count; i++)
                    {

                        referenceCvn cvn = refList[i];
                        DataRow newRow = cvnTable.NewRow();
                        newRow["cvn"] = cvn.PN;
                        newRow["PN"] = cvn.PN;
                        cvnTable.Rows.Add(newRow);
                        if (i % 100 == 0)
                        {
                            labelStatus.Text = "Importing xml... (" + i + " records)";
                            Application.DoEvents();
                        }
                    }
                    //cvnDB.refAdapter.InsertCommand = builder.GetInsertCommand();
                }
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = cvnTable;
                labelStatus.Text = cvnTable.Rows.Count.ToString() + " Records";

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }
        }

        private void btnRemoveDuplicates_Click(object sender, EventArgs e)
        {
            try
            {
                dataModified = true;
                labelStatus.Text = "Removing duplicates...";
                Application.DoEvents();
                List<string> rowStr = new List<string>();
                StringBuilder sb = new StringBuilder();
                if (comboTable.Text == "cvn")
                    builder = new OleDbCommandBuilder(cvnDB.stockAdapter);
                else
                    builder = new OleDbCommandBuilder(cvnDB.refAdapter);
                int counter = 0;
                for (int i = cvnTable.Rows.Count-1;i>=0 ; i--)
                {
                    for (int c = 0; c < cvnTable.Columns.Count; c++)
                        if (cvnTable.Columns[c].ColumnName != "id")
                            sb.Append(cvnTable.Rows[i][c].ToString());
                    if (rowStr.Contains(sb.ToString()))
                    {
                        cvnTable.Rows.RemoveAt(i);
                        counter++;
                        if (counter %100==0)
                        {
                            labelStatus.Text = "Removing duplicates... (" + counter.ToString() +" removed)";
                            Application.DoEvents();
                        }
                    }
                    else
                    {
                        rowStr.Add(sb.ToString());
                    }
                    sb = new StringBuilder();
                }
                cvnDB.stockAdapter.DeleteCommand = builder.GetDeleteCommand();
                labelStatus.Text = cvnTable.Rows.Count.ToString() + " Records";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }
        }

        private void importCSV(string fileName)
        {
            try
            {
                labelStatus.Text = "Importing file: " + fileName;
                Application.DoEvents();
                dataModified = true;
                int counter = 0;
                StreamReader sr = new StreamReader(fileName);
                int PnPos = int.MaxValue;
                int cvnPos = int.MaxValue;
                string line = sr.ReadLine();
                int x = line.Split('\'').Length - 1;
                if (x < 4)
                {
                    //Example: 1024	'4E5973E5'
                    PnPos = 0;
                    cvnPos = 1;
                }
                else
                {
                    // Example: 5505901	33	'28010723'	'7ADD    '	5504160
                    // PN & CVN are in ''
                    string[] lineparts = line.Split(new char[] { '\t', ' ',',',';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int y = 0; y < lineparts.Length; y++)
                    {
                        if (lineparts[y].StartsWith("'"))
                        {
                            if (PnPos == int.MaxValue)
                            {
                                PnPos = y;
                            }
                            else
                            {
                                cvnPos = y;
                                break;
                            }
                        }
                    }

                }
                
                sr.Close();
                sr = new StreamReader(fileName);
                while ((line = sr.ReadLine()) != null)
                {

                    string[] lineparts = line.Split(new char[] { '\t', ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineparts.Length > cvnPos)
                    {
                        DataRow newRow = cvnTable.NewRow();
                        newRow["PN"] = lineparts[PnPos].Replace("'","");
                        newRow["cvn"] = lineparts[cvnPos].Replace("'", "");
                        cvnTable.Rows.Add(newRow);
                        counter++;
                        if (counter % 100 == 0)
                        {
                            labelStatus.Text = "Importing file: " + fileName + " (" + counter + " records)";
                            Application.DoEvents();
                        }

                    }
                }


                cvnDB.stockAdapter.InsertCommand = builder.GetInsertCommand();
                labelStatus.Text = cvnTable.Rows.Count.ToString() + " Records";

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDB line " + line + ": " + ex.Message);

            }

        }

        private void btnImportCSV_Click(object sender, EventArgs e)
        {
            if (comboTable.Text == "cvn")
                builder = new OleDbCommandBuilder(cvnDB.stockAdapter);
            else
                builder = new OleDbCommandBuilder(cvnDB.refAdapter);
            List<string> fileList = SelectMultipleFiles("Select files", "ALL (*.*)|*.*");
            foreach (string newFile in fileList)
            {
                if (newFile.Length == 0) return;
                importCSV(newFile);
            }


        }

        private void CopyToClipboard()
        {
            try
            {
                //Copy to clipboard
                DataObject dataObj = dataGridView1.GetClipboardContent();
                if (dataObj != null)
                    Clipboard.SetDataObject(dataObj);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
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
                            dataGridView1.BeginEdit(true);
                            dataGridView1.NotifyCurrentCellDirty(true);
                            dataGridView1.EndEdit();
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

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            try
            {
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
            }
            catch { }
            return copyValues;
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
            PasteClipboardValue();
        }
    }
}
