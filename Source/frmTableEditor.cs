using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;
using MathParserTK;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace UniversalPatcher
{
    public partial class frmTableEditor : Form
    {
        public frmTableEditor()
        {
            InitializeComponent();
        }


        private enum ColType
        {
            Flag,
            Combo,
            Value
        }


        private class MultiTableName
        {
            public MultiTableName(string fullName, int columnPos)
            {
                RowName = "";
                string[] separators = Properties.Settings.Default.MulitableChars.Split(' ');
                string[] nParts = fullName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                //string[] nParts = fullName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                TableName = nParts[0];
                if (nParts.Length == 2)
                {
                    columnName = nParts[1].Trim();
                }
                if (nParts.Length == 3)
                {
                    columnName = nParts[1].Trim();
                    RowName = nParts[2].Trim();
                }
                if (nParts.Length > 3)
                {                    
                    columnName = nParts[columnPos].Trim();
                    for (int i = 1; i < 4; i++)
                        if (i != columnPos)
                        RowName += "[" + nParts[i].Trim() + "]";
                }

            }
            public string TableName { get; set;}
            public string columnName { get; set; }
            public string RowName { get; set; }
        }

/*        public struct TableId
        {
            public int id;
            public int cmpId;
        }*/


        //List of loaded files (for compare) File 0 is always "master" or A
        public List<CompareFile> compareFiles = new List<CompareFile>();

        public string tableName = "";
        private bool disableSaving = false;
        Font dataFont;

        private bool only1d = false;    //Show multiple 1D tables as one multirow table
        public bool disableMultiTable = false;
        public bool multiSelect = false;    //Manually selected multiple files
        private bool duplicateTableName = false;    // Multiple tables wit equal name, but some other setting may differ
        public int currentFile = 0;
        public int currentCmpFile = 1;

        public frmTuner tuner;
        private string lastTable = "";
        List<CheckBox> fileCheckBoxes;

        int multiplierDecimals = 3;
        int decimals = 0;

        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            if (Properties.Settings.Default.TableEditorFont == null)
                dataFont = new Font("Consolas", 9);
            else
                dataFont = Properties.Settings.Default.TableEditorFont;

            numTuneValue.Tag = numTuneValue.Value;
            autoResizeToolStripMenuItem.Checked = Properties.Settings.Default.TableEditorAutoResize;
            if (Properties.Settings.Default.TableEditorAutoResize)
            {
                AutoResize();
            }
            else if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.TableEditorWindowSize.Width > 0 || Properties.Settings.Default.TableEditorWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.TableEditorWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.TableEditorWindowLocation;
                    this.Size = Properties.Settings.Default.TableEditorWindowSize;
                }
            }
            disableTooltipsToolStripMenuItem.Checked = false;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView1.RowHeaderMouseClick += DataGridView1_RowHeaderMouseClick;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellClick += DataGridView1_SelectionChanged;
        }

        private void frmTableEditor_FormClosing(object sender, EventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.MainWindowPersistence)
                {
                    Properties.Settings.Default.TableEditorWindowState = this.WindowState;
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        Properties.Settings.Default.TableEditorWindowLocation = this.Location;
                        Properties.Settings.Default.TableEditorWindowSize = this.Size;
                    }
                    else
                    {
                        Properties.Settings.Default.TableEditorWindowLocation = this.RestoreBounds.Location;
                        Properties.Settings.Default.TableEditorWindowSize = this.RestoreBounds.Size;
                    }
                    Properties.Settings.Default.Save();
                }

                bool tableModified = false;
                uint addr = compareFiles[0].tableBufferOffset;
                for (int a = 0; a < compareFiles[0].buf.Length; a++)
                {
                    if (compareFiles[0].pcm.buf[addr + a] != compareFiles[0].buf[a])
                    {
                        tableModified = true;
                        break;
                    }
                }

                if (tableModified)
                {
                    DialogResult dialogResult = MessageBox.Show("Apply modifications?", "Apply modifications?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        saveTable();
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        this.DialogResult = DialogResult.Cancel;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void showCellInfo()
        {
            try
            {
                if (dataGridView1.SelectedCells.Count == 0 || dataGridView1.SelectedCells[0].Tag == null)
                    return;
                TableCell tCell = (TableCell)dataGridView1.SelectedCells[0].Tag;
                if (tCell.addr == (uint.MaxValue - 1))
                {
                    labelInfo.Text = "";
                    return; //OBD2 Description
                }
                string thisTable = tCell.td.TableName;
                if (thisTable != lastTable && tuner != null)
                {
                    lastTable = thisTable;
                    tuner.showTableDescription(tCell.tableInfo.compareFile.pcm, (int)tCell.td.id);
                }
                string minMaxTxt = "";
                TableData tData = tCell.td;
                double minRaw = getMinValue(tData.DataType);
                double maxRaw = getMaxValue(tData.DataType);
                double min = tCell.calculatedValue(minRaw);
                if (minRaw == 0 && double.IsNaN(min))
                {
                    minRaw = 1;
                    min = tCell.calculatedValue(minRaw);
                }
                double max = tCell.calculatedValue(maxRaw);

                string formatStr = "0";
                for (int f = 1; f <= (int)numDecimals.Value; f++)
                {
                    if (f == 1) formatStr += ".";
                    formatStr += "0";
                }

                if (min > max)
                {
                    //swap
                    double tmp = max;
                    max = min;
                    min = tmp;
                }

                if (min < tData.Min || max > tData.Max)
                {
                    minMaxTxt = "Soft limits: Min " + tData.Min.ToString(formatStr) + " Max " + tData.Max.ToString(formatStr) +
                        " Hard limits: Min " + min.ToString(formatStr) + " Max " + max.ToString(formatStr);

                }
                else
                {
                    minMaxTxt = "Min " + min.ToString(formatStr) + " Max " + max.ToString(formatStr);
                }
                string valTxt = " Last value " + Convert.ToDouble(tCell.lastValue).ToString(formatStr) + " Saved value " + Convert.ToDouble(tCell.origValue).ToString(formatStr);
                labelInfo.Text = minMaxTxt + valTxt + " Address: " + tCell.addr.ToString("X");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            showCellInfo();
        }

        private void parseTableInfo(CompareFile cmpFile)
        {
            try
            {
                PcmFile pcm = cmpFile.pcm;
                List<TableData> sizeList = new List<TableData>(cmpFile.filteredTables.OrderBy(o => o.addrInt).ToList());
                TableData first = sizeList[0];
                TableData last = sizeList[sizeList.Count - 1];
                int elementSize = getElementSize(last.DataType);
                int singleTableSize = last.Rows * last.Columns * elementSize;
                uint bufSize = (uint)(last.addrInt - first.addrInt + last.Offset + singleTableSize);
                cmpFile.buf = new byte[bufSize];
                Array.Copy(pcm.buf, first.addrInt, cmpFile.buf, 0, bufSize);
                cmpFile.tableBufferOffset = first.addrInt;

                for (int tId = 0; tId < cmpFile.tableIds.Count; tId++)
                {
                    TableData tData = pcm.tableDatas[cmpFile.tableIds[tId]];
                    TableInfo tInfo = new TableInfo(pcm, tData);
                    tInfo.compareFile = cmpFile;

                    int rowCount = tData.Rows;
                    int colCount = tData.Columns;

                    /*                int elementSize = getBits(tData.DataType) / 8;
                                    int bufSize = (int)(tData.Rows * tData.Columns * elementSize + tData.Offset);
                                    cmpFile.buf = new byte[bufSize];
                                    Array.Copy(pcm.buf, tData.addrInt, cmpFile.buf, 0, bufSize);
                                    cmpFile.tableBufferOffset = tData.addrInt;
                    */
                    if (tData.TableName.ToLower().EndsWith(".data"))
                    {
                        rowCount = getRowCountFromTable(tData, pcm);
                        colCount = getColumnsFromTable(tData, pcm);
                    }

                    string[] cHeaders = tData.ColumnHeaders.Split(',');
                    if (tData.ColumnHeaders.StartsWith("Table: "))
                    {
                        cHeaders = loadHeaderFromTable(tData.ColumnHeaders.Substring(7), tData.Columns, pcm).Split(',');
                    }

                    string[] rHeaders = tData.RowHeaders.Split(',');
                    if (tData.RowHeaders.StartsWith("Table: "))
                    {
                        rHeaders = loadHeaderFromTable(tData.RowHeaders.Substring(7), tData.Rows, pcm).Split(',');
                    }

                    string RowPrefix = "";
                    string colPrefix = "";
                    if (!disableMultiTable)
                    {
                        string[] nParts = tData.TableName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nParts.Length > 1)
                        {
                            //"Real" multitable
                            string TableName = nParts[0];
                            if (nParts.Length == 2)
                            {
                                colPrefix = nParts[1].Trim();
                            }
                            if (nParts.Length == 3)
                            {
                                colPrefix = nParts[1].Trim();
                                RowPrefix = nParts[2].Trim();
                            }
                            if (nParts.Length > 3)
                            {
                                int columnPos = (int)numColumn.Value;
                                colPrefix = nParts[columnPos].Trim();
                                for (int i = 1; i < 4; i++)
                                    if (i != columnPos)
                                        RowPrefix += "[" + nParts[i].Trim() + "]";
                            }
                            colPrefix += " ";
                            RowPrefix += " ";
                        }
                    }

                    List<string> colHeaders = new List<string>();
                    List<string> rowHeaders = new List<string>();
                    for (int c = 0; c < tData.Columns; c++)
                    {
                        string cHdr = "";
                        if (cHeaders.Length >= c + 1 && cHeaders[c].Length > 0)
                            cHdr = cHeaders[c];
                        else
                            cHdr = tData.Units + " " + c.ToString();
                        if (duplicateTableName)
                            cHdr += " [" + tData.id + "]";
                        if (colHeaders.Contains(colPrefix + cHdr))
                            colHeaders.Add(colPrefix + cHdr + c.ToString());
                        else
                            colHeaders.Add(colPrefix + cHdr);
                    }
                    for (int r = 0; r < tData.Rows; r++)
                    {
                        string rHdr = "";
                        if (rHeaders.Length >= r + 1 && rHeaders[r].Length > 0)
                            rHdr = rHeaders[r];
                        else
                            rHdr = "(" + r.ToString() + ")";
                        if (rowHeaders.Contains(RowPrefix + rHdr))
                            rowHeaders.Add(RowPrefix + rHdr + r.ToString());
                        else
                            rowHeaders.Add(RowPrefix + rHdr);
                    }

                    uint addr = (uint)(tData.addrInt + tData.Offset);
                    int step = getElementSize(tData.DataType);
                    if (tData.RowMajor)
                    {
                        for (int r = 0; r < tData.Rows; r++)
                        {
                            for (int c = 0; c < tData.Columns; c++)
                            {
                                TableCell tc = new TableCell();
                                tc.tableInfo = tInfo;
                                tc.td = tData;
                                tc.Column = c;
                                tc.Row = r;
                                tc.ColHeader = colHeaders[c];
                                tc.RowhHeader = rowHeaders[r];
                                tc.addr = addr;
                                tc.lastValue = getValue(pcm.buf, addr, tData, 0, pcm);
                                tc.lastRawValue = getRawValue(pcm.buf, addr, tData, 0);
                                addr += (uint)step;
                                tInfo.tableCells.Add(tc);
                            }

                        }
                    }
                    else
                    {
                        // not rowmajor
                        for (int c = 0; c < tData.Columns; c++)
                        {
                            for (int r = 0; r < tData.Rows; r++)
                            {
                                TableCell tc = new TableCell();
                                tc.tableInfo = tInfo;
                                tc.td = tData;
                                tc.Column = c;
                                tc.Row = r;
                                tc.ColHeader = colHeaders[c];
                                tc.RowhHeader = rowHeaders[r];
                                tc.addr = addr;
                                tc.lastValue = getValue(pcm.buf, addr, tData, 0, pcm);
                                tc.lastRawValue = getRawValue(pcm.buf, addr, tData, 0);
                                addr += (uint)step;
                                tInfo.tableCells.Add(tc);
                            }

                        }

                    }
                    cmpFile.tableInfos.Add(tInfo);
                }
                compareFiles.Add(cmpFile);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Form.ModifierKeys != Keys.Control )
                dataGridView1.ClearSelection();
            for (int c = 0; c < dataGridView1.Columns.Count; c++)
                dataGridView1.Rows[e.RowIndex].Cells[c].Selected = true;
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Form.ModifierKeys != Keys.Control)
                dataGridView1.ClearSelection();
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
                dataGridView1.Rows[r].Cells[e.ColumnIndex].Selected = true;
        }

        public void prepareTable(PcmFile pcm, TableData td,List<int> tableIds, string fileLetter)
        {
            try
            {
                TableInfo tInfo = new TableInfo(pcm, td);
                CompareFile orgFile = new CompareFile(pcm);
                orgFile.fileLetter = fileLetter;
                radioOriginal.Text = fileLetter;
                tableName = td.TableName;

                if (tableIds == null)
                {
                    tableIds = new List<int>();
                    tableIds.Add((int)td.id);
                }

                if (tableIds.Count > 1)
                {
                    multiSelect = true;
                    prepareMultiTable(orgFile, td, tableIds);
                    return;
                }
                if (!disableMultiTable)
                {
                    if (td.TableName.ToLower().EndsWith(".xval") || td.TableName.ToLower().EndsWith(".yval"))
                    {
                        for (int x = 0; x < pcm.tableDatas.Count; x++)
                        {
                            if (pcm.tableDatas[x].TableName.ToLower() == td.TableName.ToLower().Replace(".yval", ".data").Replace(".xval", ".data"))
                            {
                                td = pcm.tableDatas[x];
                                prepareMultiTable(orgFile, td, null);
                                return;
                            }
                        }
                    }
                    string[] separators = Properties.Settings.Default.MulitableChars.Split(' ');
                    //if (td.TableName.Contains("[") || td.TableName.Contains("."))
                    if (separators.Any(td.TableName.Contains))
                    {
                        //if (td.TableName.ToLower().Contains(" vs.") || td.TableName.StartsWith("Header.") || td.TableName.EndsWith(".Data") || td.TableName.EndsWith(".xVal") || td.TableName.EndsWith(".yVal") || td.TableName.EndsWith(".Size"))
                        if (td.TableName.ToLower().Contains(" vs.") || td.TableName.StartsWith("Header.") || td.TableName.EndsWith(".Data") || td.TableName.EndsWith(".Size"))
                        {
                            //Special case, "Normal" table, but header values from tables, WITH different table as multiplier
                            Debug.WriteLine("Special case, not real multitable");
                        }
                        else
                        {
                            MultiTableName mtn = new MultiTableName(td.TableName, (int)numColumn.Value);
                            tableName = mtn.TableName;
                            for (int t = 0; t < pcm.tableDatas.Count; t++)
                            {
                                if (pcm.tableDatas[t].Category == td.Category && pcm.tableDatas[t].TableName.StartsWith(mtn.TableName) && pcm.tableDatas[t].TableName != td.TableName)
                                {
                                    //It is multitable
                                    prepareMultiTable(orgFile, pcm.tableDatas[t], null);
                                    return;
                                }
                            }
                        }
                    }
                }
                orgFile.tableIds = tableIds;
                orgFile.filteredTables = new List<TableData>();
                orgFile.filteredTables.Add(pcm.tableDatas[tableIds[0]]);
                parseTableInfo(orgFile);
                setMyText();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }


        private void prepareMultiTable(CompareFile cmpFile, TableData tData, List<int> tableIds)
        {
            try
            {
                int maxCols = 0;
                int maxRows = 0;
                int cols = 0;
                int rows = 0;

                if (tableIds != null && tableIds.Count > 1)
                {
                    //Manually selected multiple tables
                    cmpFile.filteredTables = new List<TableData>();
                    tableIds.Sort();

                    List<string> tableNameList = new List<string>();
                    for (int i = 0; i < tableIds.Count; i++)
                    {
                        TableData mTd = cmpFile.pcm.tableDatas[tableIds[i]];
                        if (tableNameList.Contains(mTd.TableName))
                        {
                            duplicateTableName = true;
                        }
                        else
                        {
                            tableNameList.Add(mTd.TableName);
                        }
                        if (mTd.Columns > maxCols)
                            maxCols = mTd.Columns;
                        if (mTd.Rows > maxRows)
                            maxRows = mTd.Rows;
                    }
                    for (int i = 0; i < tableIds.Count; i++)
                    {
                        cmpFile.filteredTables.Add(cmpFile.pcm.tableDatas[tableIds[i]]);
                    }
                    rows = tableIds.Count;
                    cols = maxCols;
                    if (maxCols < 2 && maxRows < 2)
                        only1d = true;
                }
                else
                {
                    //Multible tables which are meant to be linked together
                    string filterName = tData.TableName.Substring(0, tableName.Length + 1);
                    var results = cmpFile.pcm.tableDatas.Where(t => t.TableName.StartsWith(filterName));
                    cmpFile.filteredTables = new List<TableData>(results.ToList());
                    cmpFile.filteredTables = cmpFile.filteredTables.OrderBy(o => o.addrInt).ToList();
                    cols = cmpFile.filteredTables.Count;
                    rows = cmpFile.filteredTables[0].Rows;
                    tableIds = new List<int>();
                    for (int i = 0; i < cmpFile.filteredTables.Count; i++)
                        tableIds.Add((int)cmpFile.filteredTables[i].id);
                }

                cmpFile.tableIds = tableIds;
                cmpFile.Rows = rows;
                cmpFile.Cols = cols;
                parseTableInfo(cmpFile);
                setMyText();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void modifyRadioText(string menuTxt)
        {
            if (menuTxt == null || menuTxt.Length == 0)
                return;
            string selectedBin = getFileLetter(menuTxt);
            radioCompareFile.Text = selectedBin;
            radioDifference.Text = radioOriginal.Text + " > " + selectedBin;
            radioDifference2.Text = radioOriginal.Text + " < " + selectedBin;
            radioSideBySide.Text = radioOriginal.Text + " | " + selectedBin;
            radioSideBySideText.Text = radioOriginal.Text + " [" + selectedBin + "]";
            radioCompareAll.Text = radioOriginal.Text + " | *";
        }

        private string getFileLetter(string menuTxt)
        {
            string retVal = "";

            int pos = menuTxt.IndexOf(':');
            if (pos > -1)
                retVal = menuTxt.Substring(0, pos);
            return retVal;
        }

        public void addCompareFiletoMenu(PcmFile cmpPCM, TableData _compareTd, string menuTxt)
        {
            try
            {
                CompareFile cmpFile = new CompareFile(cmpPCM);
                ToolStripMenuItem menuitem = new ToolStripMenuItem(cmpPCM.FileName);
                menuitem.Tag = cmpFile;
                menuitem.Name = Path.GetFileName(cmpPCM.FileName);
                if (menuTxt.Length > 0)
                {
                    menuitem.Text = menuTxt;
                    cmpFile.fileLetter = getFileLetter(menuTxt);
                }
                else
                {
                    int lastFile = 0;
                    foreach (ToolStripMenuItem mi in compareToolStripMenuItem.DropDownItems)
                        lastFile++;
                    string fLetter = Base26Encode(lastFile);
                    menuitem.Text = fLetter + ": " + cmpPCM.FileName;
                    cmpFile.fileLetter = fLetter;
                }
                prepareCompareTable(cmpFile);
                menuitem.Click += compareSelection_Click;
                if (compareToolStripMenuItem.DropDownItems.Count == 0)
                {
                    //First file selected by default
                    menuitem.Checked = true;
                    groupSelectCompare.Enabled = true;
                    modifyRadioText(menuTxt);
                    currentCmpFile = 1;
                }
                compareToolStripMenuItem.DropDownItems.Add(menuitem);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }

        public void prepareCompareTable(CompareFile cmpFile)
        {
            try
            {
                for (int i = 0; i < compareFiles[0].tableIds.Count; i++)
                {
                    int id = compareFiles[0].tableIds[i];
                    int cmpId = id;
                    if (cmpFile.pcm.OS == compareFiles[0].pcm.OS)
                    {
                        cmpFile.tableIds.Add(id);
                        cmpFile.filteredTables.Add(cmpFile.pcm.tableDatas[id]);
                    }
                    else
                    {
                        cmpId = findTableDataId(compareFiles[0].pcm.tableDatas[compareFiles[0].tableIds[i]], cmpFile.pcm.tableDatas);
                        if (cmpId > -1)
                        {
                            cmpFile.tableIds.Add(cmpId);
                            cmpFile.filteredTables.Add(cmpFile.pcm.tableDatas[cmpId]);
                        }
                    }
                    cmpFile.refTableIds.Add(id, cmpId);
                }
                parseTableInfo(cmpFile);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void compareSelection_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in compareToolStripMenuItem.DropDownItems)
                mi.Checked = false;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            modifyRadioText(menuitem.Text);
            menuitem.Checked = true;
            CompareFile cmpFile = (CompareFile)menuitem.Tag;
            currentCmpFile = findFile(cmpFile.fileLetter);
            if (radioCompareFile.Checked)
                selectFile(cmpFile.fileLetter);
            //prepareCompareTable(cmpFile); //Not again
            loadTable();
            setMyText();
        }

        public void loadSeekTable(int tId, PcmFile pcm)
        {
            try
            {
                CompareFile cmpFile = new CompareFile(pcm);
                if (!pcm.seekTablesImported)
                    pcm.importSeekTables();
                TableSeek tSeek = tableSeeks[pcm.foundTables[tId].configId];
                this.Text = "Table Editor: " + pcm.foundTables[tId].Name;

                FoundTable ft = pcm.foundTables[tId];
                for (int f = 0; f < pcm.tableDatas.Count; f++)
                {
                    if (pcm.tableDatas[f].TableName == tSeek.Name && pcm.tableDatas[f].addrInt == ft.addrInt)
                    {
                        prepareTable(pcm, pcm.tableDatas[f], null, "A");
                        loadTable();
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private int findCurrentRow(TableData mathTd,TableData cmpTd, int currentRow)
        {
            string[] headersA = mathTd.RowHeaders.Split(',');
            string[] headersB = cmpTd.RowHeaders.Split(',');

            for (int i=0; i< headersB.Length; i++)
            {
                if (headersB[i] == headersA[currentRow])
                    return i;
            }
            return -1;
        }

        public void setCellValue(int row, int col, TableCell tCell, TableCell cmpTCell)
        {
            try
            {
                TableData mathTd = tCell.td;
                double curVal = Convert.ToDouble(tCell.lastValue);
                double origVal = Convert.ToDouble(tCell.origValue);
                double curRawValue = tCell.lastRawValue;
                double cmpRawValue = UInt64.MaxValue;
                double cmpVal = double.MinValue;
                bool showSidebySide = radioSideBySide.Checked;
                if (cmpTCell == null)
                    showSidebySide = false;
                if (cmpTCell != null && !radioOriginal.Checked)
                {
                    cmpVal = Convert.ToDouble(cmpTCell.lastValue);
                    cmpRawValue = (double)cmpTCell.lastRawValue;
                    tCell.cmpValue = cmpVal;
                }
                //if (radioSideBySide.Checked && !disableSideBySide)
                if (radioSideBySideText.Checked)
                {
                    string curTxt = "";
                    string cmpTxt = "";
                    string formatStr = "0";
                    if (showRawHEXValuesToolStripMenuItem.Checked)
                    {
                        formatStr = "X" + (getElementSize(mathTd.DataType) * 2).ToString();
                        curTxt = curRawValue.ToString(formatStr);
                        if (cmpRawValue < UInt64.MaxValue)
                            cmpTxt = ((uint)cmpRawValue).ToString(formatStr);
                    }
                    else
                    {
                        if (mathTd.OutputType == OutDataType.Text)
                        {
                            //curTxt = ReadTextBlock(tCell.tableInfo.compareFile.buf, (int)(tCell.addr - tCell.tableInfo.compareFile.tableBufferOffset), tCell.td.Columns);
                            curTxt = Convert.ToChar((ushort)curVal).ToString();
                            if (cmpVal > double.MinValue)
                            {
                                cmpTxt = Convert.ToChar((ushort)cmpVal).ToString();
                                //cmpTxt = ReadTextBlock(cmpTCell.tableInfo.compareFile.buf, (int)(cmpTCell.addr - cmpTCell.tableInfo.compareFile.tableBufferOffset), cmpTCell.td.Columns);
                            }
                        }
                        else if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0)
                        {
                            curTxt = curVal.ToString();
                            if (cmpRawValue < UInt64.MaxValue)
                            {
                                cmpTxt = cmpVal.ToString();
                            }
                        }
                        else if (mathTd.OutputType == OutDataType.Hex)
                        {
                            formatStr = "X" + (getElementSize(mathTd.DataType) * 2).ToString();
                            curTxt = curRawValue.ToString(formatStr);
                            if (cmpRawValue < UInt64.MaxValue)
                                cmpTxt = cmpRawValue.ToString(formatStr);
                        }
                        else if (mathTd.OutputType == OutDataType.Int)
                        {
                            curTxt = ((int)curVal).ToString();
                            if (cmpVal > double.MinValue)
                                cmpTxt = ((int)cmpVal).ToString();
                        }
                        else
                        {

                            for (int f = 1; f <= (int)numDecimals.Value; f++)
                            {
                                if (f == 1) formatStr += ".";
                                formatStr += "0";
                            }
                            //formatStr += "#";
                            curTxt = curVal.ToString(formatStr);
                            if (cmpVal > double.MinValue)
                                cmpTxt = cmpVal.ToString(formatStr);
                        }
                    }
                    if (cmpTCell == null)
                        cmpTxt = "";
                    dataGridView1.Rows[row].Cells[col].Value = curTxt + " [" + cmpTxt + "]";
                    dataGridView1.Rows[row].Cells[col].Tag = tCell;
                    if (curVal == cmpVal)
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                    else
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                    return;
                }
                //Not side by side text mode, continue...

                double showVal = curVal;
                double showRawVal = curRawValue;
                if (radioDifference.Checked)
                {
                    if (radioMultiplier.Checked)
                        showVal = curVal / cmpVal;
                    else if (radioPercent.Checked)
                        showVal = curVal / cmpVal * 100 - 100;
                    else
                        showVal = curVal - cmpVal;
                    showRawVal = curRawValue - cmpRawValue;
                }    
                else if (radioDifference2.Checked)
                {
                    if (radioMultiplier.Checked)
                        showVal = cmpVal / curVal;
                    else if (radioPercent.Checked)
                        showVal = cmpVal / curVal * 100 - 100;
                    else
                        showVal = cmpVal - curVal;
                    showRawVal = cmpRawValue - curRawValue;

                }

                if (showRawHEXValuesToolStripMenuItem.Checked)
                {
                    switch (mathTd.DataType)
                    {
                        case InDataType.FLOAT32:
                            dataGridView1.Rows[row].Cells[col].Value = (Int32)showRawVal;
                            break;
                        case InDataType.FLOAT64:
                            dataGridView1.Rows[row].Cells[col].Value = (Int64)showRawVal;
                            break;
                        case InDataType.INT64:
                            dataGridView1.Rows[row].Cells[col].Value = (Int64)showRawVal;
                            break;
                        case InDataType.INT32:
                            dataGridView1.Rows[row].Cells[col].Value = (Int32)showRawVal;
                            break;
                        case InDataType.UINT64:
                            dataGridView1.Rows[row].Cells[col].Value = (UInt64)showRawVal;
                            break;
                        case InDataType.UINT32:
                            dataGridView1.Rows[row].Cells[col].Value = (UInt32)showRawVal;
                            break;
                        case InDataType.SWORD:
                            dataGridView1.Rows[row].Cells[col].Value = (Int16)showRawVal;
                            break;
                        case InDataType.UWORD:
                            dataGridView1.Rows[row].Cells[col].Value = (UInt16)showRawVal;
                            break;
                        case InDataType.SBYTE:
                            dataGridView1.Rows[row].Cells[col].Value = (sbyte)showRawVal;
                            break;
                        case InDataType.UBYTE:
                            dataGridView1.Rows[row].Cells[col].Value = (byte)showRawVal;
                            break;
                        default:
                            dataGridView1.Rows[row].Cells[col].Value = (Int32)showRawVal;
                            break;
                    }
                    //dataGridView1.Rows[row].Cells[col].Value = Convert.ToInt64(showRawVal);
                }
                else if (mathTd.OutputType == OutDataType.Text)
                {
                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToChar((ushort)showRawVal);
                    //dataGridView1.Rows[row].Cells[col].Value = ReadTextBlock(tCell.tableInfo.compareFile.buf, (int)(tCell.addr - tCell.tableInfo.compareFile.tableBufferOffset), tCell.td.Columns);
                }
                else if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0)
                {
                    dataGridView1.Rows[row].Cells[col].Value = (int)showVal;
                }
                else if (mathTd.OutputType == OutDataType.Hex)
                {
                    dataGridView1.Rows[row].Cells[col].Value = (uint)showVal;
                }
                else if (mathTd.OutputType == OutDataType.Int)
                {
                    dataGridView1.Rows[row].Cells[col].Value = (int)showVal;
                }
                else
                {
                    dataGridView1.Rows[row].Cells[col].Value = showVal;
                }

                dataGridView1.Rows[row].Cells[col].Tag = tCell;
                setCellColor(row, col,tCell);
                if (!disableTooltipsToolStripMenuItem.Checked && mathTd.TableDescription != null)
                {
                    if (mathTd.TableDescription.Length > 200)
                        dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription.Substring(0, 200);
                    else
                        dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }

        private void setCellColor(int row, int col, TableCell tCell)
        {
            try
            {
                TableData mathTd = tCell.td;
                double curVal = Convert.ToDouble(tCell.lastValue);
                double origVal = Convert.ToDouble(tCell.origValue);
                Color[] colors =
                     {
                        //Color.FromArgb(255, 255, 192, 192), //Pink?
                        Color.FromArgb(255, 255, 224, 192),
                        Color.FromArgb(255, 255, 255, 192),
                        Color.FromArgb(255, 192, 255, 192),
                        Color.FromArgb(255, 192, 255, 255),
                        Color.FromArgb(255, 192, 192, 255),
                        Color.FromArgb(255, 255, 192, 255),
                        Color.FromArgb(255, 224, 224, 224),
                        Color.FromArgb(255, 255, 128, 128),
                        Color.FromArgb(255, 255, 192, 128),
                        Color.FromArgb(255, 255, 255, 128),
                        Color.FromArgb(255, 128, 255, 128),
                        Color.FromArgb(255, 128, 255, 255),
                        Color.FromArgb(255, 128, 128, 255),
                        Color.FromArgb(255, 255, 128, 255),
                        Color.Silver,
                        Color.Red,
                        Color.FromArgb(255, 255, 128, 0),
                        Color.Yellow,
                        Color.Lime,
                        Color.Cyan,
                        Color.Blue,
                        Color.Fuchsia,
                        Color.Gray,
                        Color.FromArgb(255, 192, 0, 0),
                        Color.FromArgb(255, 192, 64, 0),
                        Color.FromArgb(255, 192, 192, 0),
                        Color.FromArgb(255, 0, 192, 0),
                        Color.FromArgb(255, 0, 192, 192),
                        Color.FromArgb(255, 0, 0, 192),
                        Color.FromArgb(255, 192, 0, 192),
                        Color.FromArgb(255, 64, 64, 64),
                        Color.Maroon,
                        Color.FromArgb(255, 128, 64, 0),
                        Color.Olive,
                        Color.Green,
                        Color.Teal,
                        Color.Navy,
                        Color.Purple,
                        Color.Black,
                        Color.FromArgb(255, 64, 0, 0),
                        Color.FromArgb(255, 128, 64, 64),
                        Color.FromArgb(255, 64, 64, 0),
                        Color.FromArgb(255, 0, 64, 0),
                        Color.FromArgb(255, 0, 64, 64),
                        Color.FromArgb(255, 0, 0, 64),
                        Color.FromArgb(255, 64, 0, 64),
                    };

                if (radioSideBySide.Checked || radioCompareAll.Checked)
                {
                    if (tCell.tableInfo.compareFile.pcm.FileName != compareFiles[0].pcm.FileName)
                    {
                        //Compare Cell
                        string colTxt = "[" + compareFiles[0].fileLetter + "]" + dataGridView1.Columns[col].HeaderText.Substring(3);
                        string rowTxt = dataGridView1.Rows[row].HeaderCell.Value.ToString();
                        int orgCol = getColumnByHeader(colTxt);
                        int orgRow = getRowByHeader(rowTxt);
                        if (dataGridView1.Rows[orgRow].Cells[orgCol].Tag != null)
                        {
                            TableCell tOrigCell = (TableCell)dataGridView1.Rows[orgRow].Cells[orgCol].Tag;
                            if (Convert.ToDouble(tOrigCell.lastValue) != Convert.ToDouble(tCell.lastValue))
                            {
                                dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                            }
                            else
                            {
                                string fLetter = dataGridView1.Columns[col].HeaderText.Substring(1, 1);
                                char fl = fLetter[0];
                                int nr = fl - 'A';
                                if (nr > colors.Length - 1)
                                    nr = colors.Length - 1;
                                dataGridView1.Rows[row].Cells[col].Style.BackColor = colors[nr];
                            }
                        }
                        return;
                    }
                }
                if (dataGridView1.Columns[col].GetType() != typeof(DataGridViewComboBoxColumn) &&
                    dataGridView1.Rows[row].Cells[col].GetType() != typeof(DataGridViewComboBoxCell))
                {
                    if (curVal != origVal)
                    {
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.Yellow;
                        if (!disableTooltipsToolStripMenuItem.Checked)
                            dataGridView1.Rows[row].Cells[col].ToolTipText = "Original value: " + origVal.ToString();
                    }
                    else
                    {
                        if (curVal < (mathTd.Max * 0.9) && curVal > (mathTd.Min * 1.1))
                            dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.White;
                        else if (curVal > mathTd.Max)
                            dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.Pink;
                        else if (curVal > (0.9 * mathTd.Max))
                            dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                        else if (curVal < mathTd.Min)
                            dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.AliceBlue;
                        else if (curVal < (1.1 * mathTd.Min))
                            dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                        if (!disableTooltipsToolStripMenuItem.Checked)
                            dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription;
                    }
                }
                else if (dataGridView1.Columns[col].GetType() == typeof(DataGridViewComboBoxColumn) || dataGridView1.Rows[row].Cells[col].GetType() == typeof(DataGridViewComboBoxCell))
                {
                    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[row].Cells[col];
                    if (curVal != origVal)
                    {
                        cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Italic);
                        if (!disableTooltipsToolStripMenuItem.Checked)
                            cell.ToolTipText = "Original value: " + origVal.ToString();
                    }
                    else
                    {
                        cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        if (!disableTooltipsToolStripMenuItem.Checked)
                            cell.ToolTipText = mathTd.TableDescription;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private string loadHeaderFromTable(string tableName, int count, PcmFile pcm)
        {
            string headers = "" ;
            for (int i=0; i < pcm.tableDatas.Count; i++)
            {
                TableData headerTd = pcm.tableDatas[i];
                if (headerTd.TableName == tableName)
                {
                    uint step = (uint)(getBits(headerTd.DataType) / 8);
                    uint addr = (uint)(headerTd.addrInt + headerTd.Offset);
                    for (int a = 0; a < count; a++ )
                    {
                        string formatStr = "0.####";
                        if (headerTd.Units.Contains("%"))
                            formatStr = "0";
                        headers += headerTd.Units.Trim() + " " + getValue(pcm.buf, addr, headerTd, 0, pcm).ToString(formatStr).Replace(",", ".") + ",";
                        addr += step;
                    }
                    headers = headers.Trim(',');
                    break;
                }
            }
            return headers;
        }

        private int getColumnByHeader(string hdrTxt)
        {
            int ind = int.MinValue;
            hdrTxt = hdrTxt.Trim();
            for (int c = 0; c < dataGridView1.Columns.Count; c++)
            {
                if (dataGridView1.Columns[c].HeaderText == hdrTxt)
                    ind = c;
            }
                
            if (ind < 0)
            {
                ind = dataGridView1.Columns.Add(hdrTxt, hdrTxt);
            }
            return ind;
        }

        private int getRowByHeader(string hdrTxt)
        {
            int ind = int.MinValue;
            hdrTxt = hdrTxt.Trim();
            for (int c = 0; c < dataGridView1.Rows.Count; c++)
            {
                if (dataGridView1.Rows[c].HeaderCell.Value.ToString() == hdrTxt)
                    ind = c;
            }
            if (ind < 0)
            {
                ind = dataGridView1.Rows.Add();
                dataGridView1.Rows[ind].HeaderCell.Value = hdrTxt;
            }
            return ind;
        }

        private void addCellByType(TableData ft, int gridRow, int gridCol)
        {
            if (radioSideBySideText.Checked || showRawHEXValuesToolStripMenuItem.Checked)
                return;
            try
            {
                TableValueType vt = getValueType(ft);
                if (vt == TableValueType.boolean || vt == TableValueType.bitmask)
                {
                    DataGridViewCheckBoxCell dgc = new DataGridViewCheckBoxCell();
                    dgc.Style.NullValue = false;
                    dataGridView1.Rows[gridRow].Cells[gridCol] = dgc;
                }
                else if (vt == TableValueType.selection)
                {
                    DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                    if (ft.OutputType == OutDataType.Float)
                    {
                        Dictionary<double, string> possibleVals = parseEnumHeaders(ft.Values.Replace("Enum: ", ""));
                        dgc.DataSource = new BindingSource(possibleVals, null);
                    }
                    else
                    {
                        Dictionary<int, string> possibleVals = parseIntEnumHeaders(ft.Values.Replace("Enum: ", ""));
                        dgc.DataSource = new BindingSource(possibleVals, null);
                    }
                    dgc.ValueMember = "key";
                    dgc.DisplayMember = "value";
                    dataGridView1.Rows[gridRow].Cells[gridCol] = dgc;
                }
                else
                {
                    //at least one table which difference can be shown
                    radioDifference.Enabled = true;
                    radioDifference2.Enabled = true;
                }

            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);

            }
        }

        public void loadTable()
        {
            try
            {
                this.dataGridView1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);

                CompareFile selectedFile = compareFiles[currentFile];
                PcmFile PCM = selectedFile.pcm;
                TableData td = selectedFile.tableInfos[0].td;

                if (td.Units.ToLower().Contains("bitmask"))
                    labelUnits.Text = "Units: Boolean";
                else
                    labelUnits.Text = "Units: " + getUnitFromTableData(td);
                if (getValueType(td) == TableValueType.selection)
                    labelUnits.Text += ", Values: " + td.Values;

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                bool xySwapped = chkSwapXY.Checked;

                if (selectedFile.Cols > 5 && selectedFile.Rows < 5)
                    xySwapped = !chkSwapXY.Checked;

                List<CompareFile> cmpFiles = new List<CompareFile>();
                CompareFile diffFile = null;
                if (radioDifference.Checked || radioDifference2.Checked)
                {
                    diffFile = compareFiles[currentCmpFile];
                }
                if (radioSideBySideText.Checked)
                {
                    diffFile = compareFiles[currentCmpFile];
                }
                if (radioSideBySide.Checked)
                {
                    diffFile = compareFiles[currentCmpFile];
                    cmpFiles.Add(diffFile);
                }
                if (radioCompareAll.Checked)
                {
                    for (int i = 1; i < compareFiles.Count; i++)
                    {
                        cmpFiles.Add(compareFiles[i]);
                    }
                }

                CompareFile sFile = compareFiles[currentFile];

                for (int tbl = 0; tbl < sFile.tableInfos.Count; tbl++)
                {
                    TableData ft = sFile.tableInfos[tbl].td;
                    TableInfo cmpTinfo = null;

                    int gridCol = 0;
                    int gridRow = 0;

                    //Find maximum cell count from all comparefiles:
                    int cellCount = sFile.tableInfos[tbl].tableCells.Count;
                    for (int d = 0; d < cmpFiles.Count; d++)
                    {
                        int cmpId = -1;
                        if (cmpFiles[d].refTableIds.ContainsKey(sFile.tableIds[tbl]))
                            cmpId = cmpFiles[d].refTableIds[sFile.tableIds[tbl]];
                        if (cmpId > -1)
                        {
                            int pos = cmpFiles[d].tableIds.IndexOf(cmpId);
                            cmpTinfo = cmpFiles[d].tableInfos[pos];
                            if (cmpTinfo.tableCells.Count > cellCount)
                                cellCount = cmpTinfo.tableCells.Count;
                        }
                    }

                    for (int cell = 0; cell < cellCount; cell++)
                    {
                        TableCell cmpCell = null;
                        if (sFile.tableInfos[tbl].tableCells.Count > cell)
                        {
                            //Original file may have less cells than some of compare files?
                            TableCell tcell = sFile.tableInfos[tbl].tableCells[cell];
                            if (diffFile != null)   //RadioDifference checked
                            {
                                int cmpId = -1;
                                if (diffFile.refTableIds.ContainsKey(sFile.tableIds[tbl]))
                                    cmpId = diffFile.refTableIds[sFile.tableIds[tbl]];
                                if (cmpId > -1)
                                {
                                    int pos = diffFile.tableIds.IndexOf(cmpId);
                                    cmpTinfo = diffFile.tableInfos[pos];
                                    TableData cmpTd = diffFile.pcm.tableDatas[cmpId];
                                    if (cmpTinfo.tableCells.Count > cell)
                                        cmpCell = cmpTinfo.tableCells[cell];
                                }
                            }
                            string colHdr;
                            string rowHdr;
                            if (!xySwapped)
                            {
                                colHdr = tcell.ColHeader;
                                rowHdr = tcell.RowhHeader;
                                if (only1d)
                                {
                                    colHdr = "[" + selectedFile.fileLetter + "] "; //Show only [A]
                                    rowHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                                else if (radioSideBySide.Checked || radioCompareAll.Checked)
                                {
                                    if (multiSelect)
                                        colHdr = "[" + selectedFile.fileLetter + "] " + "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                    else
                                        colHdr = "[" + selectedFile.fileLetter + "] " + tcell.ColHeader;
                                }
                                else if (multiSelect)
                                {
                                    colHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                            }
                            else
                            {
                                colHdr = tcell.RowhHeader;
                                rowHdr = tcell.ColHeader;
                                if (only1d)
                                {
                                    rowHdr = "[" + selectedFile.fileLetter + "] "; //Show only [A]
                                    colHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                                else if (radioSideBySide.Checked || radioCompareAll.Checked)
                                {
                                    if (multiSelect)
                                    {
                                        rowHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                        colHdr = "[" + selectedFile.fileLetter + "] " + tcell.RowhHeader;
                                    }
                                    else
                                    {
                                        rowHdr = tcell.ColHeader;
                                        colHdr = "[" + selectedFile.fileLetter + "] " + tcell.RowhHeader;
                                    }
                                }
                                else if (multiSelect)
                                {
                                    rowHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                            }
                            gridCol = getColumnByHeader(colHdr);
                            gridRow = getRowByHeader(rowHdr);
                            addCellByType(ft, gridRow, gridCol);
                            setCellValue(gridRow, gridCol, tcell, cmpCell);
                        }
                        for (int d = 0; d < cmpFiles.Count; d++)
                        {

                            if (radioCompareAll.Checked)
                            {
                                bool selected = false;
                                for (int x = 0; x < fileCheckBoxes.Count; x++)
                                    if (fileCheckBoxes[x].Text == cmpFiles[d].fileLetter && fileCheckBoxes[x].Checked)
                                        selected = true;
                                if (!selected)
                                    continue;   
                            }
                            int cmpId = -1;
                            if (cmpFiles[d].refTableIds.ContainsKey(sFile.tableIds[tbl]))
                                cmpId = cmpFiles[d].refTableIds[sFile.tableIds[tbl]];
                            if (cmpId > -1)
                            {
                                int pos = cmpFiles[d].tableIds.IndexOf(cmpId);
                                cmpTinfo = cmpFiles[d].tableInfos[pos];
                                TableData cmpTd = cmpFiles[d].pcm.tableDatas[cmpId];
                                if (cmpTinfo.tableCells.Count > cell)
                                {
                                    cmpCell = cmpTinfo.tableCells[cell];
                                    string cmpColHdr = "";
                                    string cmpRowHdr = "";
                                    if (!xySwapped)
                                    {
                                        cmpColHdr = cmpCell.ColHeader;
                                        cmpRowHdr = cmpCell.RowhHeader;
                                        if (only1d)
                                        {
                                            cmpColHdr = "[" + cmpFiles[d].fileLetter + "] ";
                                            cmpRowHdr = "[" + cmpTd.TableName + "] " + cmpCell.ColHeader;
                                        }
                                        else
                                        {
                                            if (multiSelect)
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + "[" + cmpTd.TableName + "] " + cmpCell.ColHeader;
                                            else
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + cmpCell.ColHeader;
                                        }
                                    }
                                    else
                                    {
                                        cmpRowHdr = cmpCell.ColHeader;
                                        cmpColHdr = cmpCell.RowhHeader;
                                        if (only1d)
                                        {
                                            cmpRowHdr = "[" + cmpFiles[d].fileLetter + "] ";
                                            cmpColHdr = "[" + cmpTd.TableName + "] " + cmpCell.ColHeader;
                                        }
                                        else
                                        {
                                            if (multiSelect)
                                            {
                                                cmpRowHdr = "[" + cmpTd.TableName + "] " + cmpCell.ColHeader;
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + cmpCell.RowhHeader;
                                            }
                                            else
                                            {
                                                cmpRowHdr = cmpCell.ColHeader;
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + cmpCell.RowhHeader;
                                            }
                                        }
                                    }
                                    gridCol = getColumnByHeader(cmpColHdr);
                                    gridRow = getRowByHeader(cmpRowHdr);
                                    addCellByType(cmpCell.td, gridRow, gridCol);
                                    setCellValue(gridRow, gridCol, cmpCell, null);
                                }
                            }

                        }

                    }

                }
                
                if (td.TableName.StartsWith("DTC"))
                {
                    showDtcDescriptions();
                }


                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        if (dataGridView1.Rows[r].Cells[c].Tag == null)
                        {
                            dataGridView1.Rows[r].Cells[c].ReadOnly = true;
                            if (radioSideBySide.Checked)
                            {
                                if (dataGridView1.Rows[r].Cells[c].Value == null)
                                    dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.DarkGray;
                            }
                            else
                            {
                                dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.DarkGray;
                            }
                        }
                    }
                }
                setDataGridLayout(td);
                dataGridView1.EndEdit();
                this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                showCellInfo();

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }


        private void showDtcDescriptions()
        {
            try
            {
                DtcSearch ds = new DtcSearch();
                if (OBD2Codes == null || OBD2Codes.Count == 0)
                    loadOBD2Codes();
                if (OBD2Codes.Count == 0)
                    return;
                chkSwapXY.Enabled = false;
                swapXyToolStripMenuItem.Enabled = false;
                searchCodeFromGoogleToolStripMenuItem.Visible = true;
                DataGridViewColumn dgc = new DataGridViewColumn();
                dgc.Name = "Description";
                dgc.HeaderText = "Description";
                dgc.CellTemplate = new DataGridViewTextBoxCell();
                dataGridView1.Columns.Insert(0, dgc);
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    string descr = ds.getDtcDescription(dataGridView1.Rows[r].HeaderCell.Value.ToString());
                    dataGridView1.Rows[r].Cells["Description"].Value = descr;
                    if (dataGridView1.Rows[r].Cells[1].Tag != null)
                    {
                        TableCell tc = (TableCell)dataGridView1.Rows[r].Cells[1].Tag;
                        TableCell tcDescr = tc.ShallowCopy();
                        tcDescr.addr = uint.MaxValue - 1;
                        dataGridView1.Rows[r].Cells["Description"].Tag = tcDescr;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        public int getColumnsFromTable(TableData tData, PcmFile pcm)
        {
            int cols = tData.Columns;
            try
            {

                string yTbName = tData.TableName.Replace(".Data", ".xVal");
                for (int y = 0; y < pcm.tableDatas.Count; y++)
                {
                    if (pcm.tableDatas[y].TableName == yTbName)
                    {
                        TableData ytb = pcm.tableDatas[y];
                        uint xaddr = (uint)(ytb.addrInt + ytb.Offset);
                        cols = (int)getValue(pcm.buf, xaddr, ytb, 0, pcm);
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            return cols;
        }

        public int getRowCountFromTable(TableData tData, PcmFile pcm)
        {
            int rows = tData.Rows;
            try
            {

                for (int x = 0; x < pcm.tableDatas.Count; x++)
                {
                    if (pcm.tableDatas[x].TableName == tData.TableName.Replace(".Data", ".Size") || pcm.tableDatas[x].TableName == tData.TableName.Replace(".Data", ".yVal"))
                    {
                        uint addr = (uint)(pcm.tableDatas[x].addrInt + pcm.tableDatas[x].Offset);
                        rows = (int)getValue(pcm.buf, addr, pcm.tableDatas[x], 0, pcm);
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            return rows;
        }

        private string getUnitFromTableData(TableData tData)
        {
            string retVal = "";

            /*for (int i = 0; i < unitList.Count; i++)
                if (tData.TableName.Contains(unitList[i].Abbreviation) && unitList[i].Unit != null && unitList[i].Unit.Length > 0)
                    return unitList[i].Unit;*/

            if (tData.Units != null)
                retVal = tData.Units;
            
            return retVal;
        }


        private void setDataGridLayout(TableData td)
        {
            try
            {
                if (numDecimals.Value < 0 && td != null)
                {
                    this.numDecimals.ValueChanged -= new System.EventHandler(this.numDecimals_ValueChanged);
                    numDecimals.Value = td.Decimals;
                    decimals = td.Decimals;
                    this.numDecimals.ValueChanged += new System.EventHandler(this.numDecimals_ValueChanged);
                }
                string formatStr = "0";
                if (showRawHEXValuesToolStripMenuItem.Checked || td.OutputType == OutDataType.Hex)
                {
                    //formatStr = "X" + ((int)numDecimals.Value).ToString();
                    formatStr = "X" + (getElementSize(td.DataType) *2).ToString();
                }
                else if (td.OutputType == OutDataType.Text || td.OutputType == OutDataType.Flag )
                {
                    formatStr = "";
                }
                else
                {
                    for (int f = 1; f <= (int)numDecimals.Value ; f++)
                    {
                        if (f == 1) formatStr += ".";
                        formatStr += "0";
                    }
                    //formatStr += "#";
                }
                foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
                {
                    dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (dgvc.HeaderText != "Description")
                        dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvc.DefaultCellStyle.Font = dataFont;
                    if (formatStr != "" && dgvc.GetType() != typeof(DataGridViewComboBoxColumn) )
                        dgvc.DefaultCellStyle.Format = formatStr;
                }
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                if (autoResizeToolStripMenuItem.Checked) AutoResize();
                //dataGridView1.RefreshEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                MessageBox.Show("Error, frmTableEditor line " + line + ": " + ex.Message, "Error");
            }

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (disableSaving)
            {
                e.Cancel = true;
                return;
            }
        }


        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                TableCell tc = new TableCell(); 
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                {
                    tc = (TableCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                }
                if ( dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null || String.IsNullOrWhiteSpace(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                {
                    if (tc.lastValue != null)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = tc.lastValue;
                        return;
                    }
                }

                TableData td = tc.td;
                if (e.RowIndex > -1)
                {
                    if (td.TableName.StartsWith("DTC") && tc.addr == (uint.MaxValue - 1))
                    {
                        //OBD2 Description
                        OBD2Code oc = new OBD2Code();
                        oc.Code = dataGridView1.Rows[e.RowIndex].HeaderCell.Value.ToString();
                        oc.Description = dataGridView1.Rows[e.RowIndex].Cells["Description"].Value.ToString();
                        bool codeFound = false;
                        for (int o = 0; o < OBD2Codes.Count; o++)
                        {
                            if (OBD2Codes[o].Code == oc.Code)
                            {
                                OBD2Codes[o].Description = oc.Description;
                                codeFound = true;
                                break;
                            }
                        }
                        if (!codeFound)
                        {
                            OBD2Codes.Add(oc);
                        }
                    }
                    else
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != tc.lastValue)
                            SaveValue(e.RowIndex, e.ColumnIndex, tc);
                        setCellColor(e.RowIndex, e.ColumnIndex, tc);
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
                MessageBox.Show("Error, frmTableEditor line " + line + ": " + ex.Message, "Error");
            }
        }




        public void SaveValue(int r, int c, TableCell tCell, double value = double.MinValue)
        {            
            double newValue = value;
            TableData mathTd = tCell.td;
            try
            {
                if (value == double.MinValue)
                {
                    if (dataGridView1.Rows[r].Cells[c].GetType() == typeof(DataGridViewComboBoxCell))
                    {
                        DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView1.Rows[r].Cells[c];
                        newValue = Convert.ToDouble(cb.Value);
                    }
                    else if (showRawHEXValuesToolStripMenuItem.Checked)
                    {
                        newValue = (double)Convert.ToInt64(dataGridView1.Rows[r].Cells[c].Value.ToString(), 16);
                    }
                    else
                    {
                        newValue = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
                        if (radioDifference.Checked)
                        {
                            if (radioAbsolute.Checked)
                                newValue = (double)tCell.lastValue - newValue;
                            else if (radioMultiplier.Checked)
                                newValue = (double)tCell.cmpValue * newValue;
                            else if (radioPercent.Checked)
                                newValue =  (100 + newValue) / 100 * (double)tCell.cmpValue;
                        }
                        else if (radioDifference2.Checked)
                        {
                            if (radioAbsolute.Checked)
                                newValue =  newValue + (double)tCell.lastValue;
                            else if (radioMultiplier.Checked)
                                newValue = (double)tCell.cmpValue / newValue;
                            else if(radioPercent.Checked)
                                newValue = (100 - newValue) / 100 * (double)tCell.cmpValue;
                        }
                    }
                }
                if (newValue == double.MaxValue) return;

                if (showRawHEXValuesToolStripMenuItem.Checked)
                {
                    tCell.saveValue(newValue, true);
                }
                else
                { 
                    if (dataGridView1.Columns[c].GetType() != typeof(DataGridViewComboBoxColumn)
                        && dataGridView1.Rows[r].Cells[c].GetType() != typeof(DataGridViewComboBoxCell))
                    {
                        if (newValue > mathTd.Max)
                            newValue = mathTd.Max;
                        if (newValue < mathTd.Min)
                            newValue = mathTd.Min;
                    }
                    //string mathStr = mathTd.SavingMath.ToLower();
                    //newValue = parser.Parse(mathStr, true);
                    tCell.saveValue(newValue);
                    //string mathStr = mathTd.Math.ToLower();
                    //double calcValue = (double)tCell.lastValue;
                    if (radioDifference.Checked || radioDifference2.Checked)
                        loadTable();
                    else
                        dataGridView1.Rows[r].Cells[c].Value = tCell.lastValue;
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
                dataGridView1.Rows[r].Cells[c].Value = tCell.lastValue;
            }
        }

        public void saveTable()
        {
            try
            {
                dataGridView1.EndEdit();
                byte[] tableBuffer = compareFiles[0].buf;
                Array.Copy(tableBuffer, 0, compareFiles[0].pcm.buf,compareFiles[0].tableBufferOffset, tableBuffer.Length);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                MessageBox.Show("Error, frmTableEditor line " + line + ": " + ex.Message, "Error");
            }

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
                    TableCell tc = (TableCell)dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Tag;
                    SaveValue(cell.RowIndex, cell.ColumnIndex,tc);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void AutoResize()
        {
            try
            {
                int dgv_width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + dataGridView1.RowHeadersWidth;
                if (dgv_width < 550) dgv_width = 550;
                int dgv_height = dataGridView1.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridView1.ColumnHeadersHeight;
                Screen myScreen = Screen.FromPoint(MousePosition);
                System.Drawing.Rectangle area = myScreen.WorkingArea;
                if ((dgv_width + 150) > area.Width)
                    this.Width = area.Width - 50;
                else
                    this.Width = dgv_width + 50; //150
                if ((dgv_height + 100) > area.Height)
                    this.Height = area.Height - 50;
                else
                    this.Height = dgv_height + 150; //175
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void chkAutoResize_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TableEditorAutoResize = autoResizeToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            if (autoResizeToolStripMenuItem.Checked)
            {
                AutoResize();
            }
        }

        private void chkTranspose_CheckedChanged(object sender, EventArgs e)
        {
            loadTable();
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            if (autoResizeToolStripMenuItem.Checked) AutoResize();
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
            try
            {
                //Show Error if no cell is selected
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please select a cell", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                dataGridView1.BeginEdit(true);

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
                            //if ((chkPasteToSelectedCells.Checked && cell.Selected) || (!chkPasteToSelectedCells.Checked))
                            cell.Value = cbValue[rowKey][cellKey];
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
                dataGridView1.EndEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
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

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
                dataGridView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void exportCsv()
        {
            try
            {

                string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
                if (FileName.Length == 0)
                    return;
                using (StreamWriter writetext = new StreamWriter(FileName))
                {
                    string row = ";";
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        row += dataGridView1.Columns[i].HeaderText;
                    }
                    writetext.WriteLine(row);
                    for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                    {
                        row = dataGridView1.Rows[r].HeaderCell.Value.ToString() + ";";
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
                MessageBox.Show(FileName, "CSV Export done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void exportCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exportCsv();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveTable();
        }

        private void exportCSVToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exportCsv();
        }

        private void autoResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoResizeToolStripMenuItem.Checked)
                autoResizeToolStripMenuItem.Checked = false;
            else
                autoResizeToolStripMenuItem.Checked = true;
            Properties.Settings.Default.TableEditorAutoResize = autoResizeToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
            if (autoResizeToolStripMenuItem.Checked)
            {
                AutoResize();
            }

        }

        private void swapXyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (swapXyToolStripMenuItem.Checked)
                swapXyToolStripMenuItem.Checked = false;
            else
                swapXyToolStripMenuItem.Checked = true;
            chkSwapXY.Checked = swapXyToolStripMenuItem.Checked;
            loadTable();

        }

        private void chkSwapXY_CheckedChanged(object sender, EventArgs e)
        {
            swapXyToolStripMenuItem.Checked = chkSwapXY.Checked;
            loadTable();
        }

        private void showRawHEXValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showRawHEXValuesToolStripMenuItem.Checked = !showRawHEXValuesToolStripMenuItem.Checked;
            chkRawHex.Checked = showRawHEXValuesToolStripMenuItem.Checked;
            loadTable();
        }

        private void disableTooltipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disableTooltipsToolStripMenuItem.Checked)
            {
                disableTooltipsToolStripMenuItem.Checked = false;
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        TableCell tc = (TableCell)dataGridView1.Rows[r].Cells[c].Tag;
                        if (tc.td.TableDescription != null && dataGridView1.Rows[r].Cells[c].ToolTipText == null)
                            dataGridView1.Rows[r].Cells[c].ToolTipText = tc.td.TableDescription;
                    }
                }
            }
            else
            {
                disableTooltipsToolStripMenuItem.Checked = true;
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        dataGridView1.Rows[r].Cells[c].ToolTipText = null;
                    }
                }
            }
        }


        private void showGraphicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData td = compareFiles[currentFile].tableInfos[0].td;
                frmGraphics fg = new frmGraphics();
                fg.Text = td.TableName;
                fg.Show();
                fg.chart1.Series.Clear();
                double minVal = double.MaxValue;
                double maxVal = double.MinValue;

                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    fg.chart1.Series.Add(new Series());
                    fg.chart1.Series[r].ChartType = SeriesChartType.Line;
                    if (dataGridView1.Rows[r].HeaderCell.Value != null)
                        fg.chart1.Series[r].Name = dataGridView1.Rows[r].HeaderCell.Value.ToString();
                    fg.chart1.Series[r].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                    int point = 0;
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        double val = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
                        if (val > maxVal) maxVal = val;
                        if (val < minVal) minVal = val;
                        fg.chart1.Series[r].Points.AddXY(dataGridView1.Columns[c].HeaderCell.Value, val);
                        fg.chart1.Series[r].Points[point].MarkerStyle = MarkerStyle.Circle;
                        fg.chart1.Series[r].Points[point].MarkerSize = 5;
                        point++;
                    }
                }
                //fg.chart1.ChartAreas[0].AxisY.Interval = 10;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!e.Exception.Message.Contains("DataGridViewComboBoxCell"))
                Debug.WriteLine(e.Exception);
        }

        private void numColumn_ValueChanged(object sender, EventArgs e)
        {
            TableData td = compareFiles[currentFile].tableInfos[0].td;
            MultiTableName mtn = new MultiTableName(td.TableName, (int)numColumn.Value);
            //loadMultiTable(mtn.TableName,compareFiles[currentFile].pcm);
        }

        private void selectFile(string letter)
        {
            for (int l = 0; l < compareFiles.Count; l++)
            {
                if (compareFiles[l].fileLetter == letter)
                {
                    currentFile = l;
                    break;
                }
            }

        }

        private int  findFile(string letter)
        {
            for (int l = 0; l < compareFiles.Count; l++)
            {
                if (compareFiles[l].fileLetter == letter)
                {
                    return l;
                }
            }
            return -1;
        }


        private void radioCompareFile_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCompareFile.Checked)
            {
                selectFile(radioCompareFile.Text);
                dataGridView1.BackgroundColor = Color.Red;
                setMyText();
                loadTable();
            }

        }

        private void radioDifference_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDifference.Checked || radioDifference2.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                disableSaving = false;
                setMyText();
                groupDifference.Visible = true;
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = multiplierDecimals;
                }
                loadTable();
            }
            else
            {
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = decimals;
                }

                groupDifference.Visible = false;
            }
        }

        private void radioSideBySide_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSideBySide.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                //disableSaving = true;
                setMyText();
                loadTable();
            }
            graphToolStripMenuItem.Enabled = !radioSideBySide.Checked;
            copyFromCompareToolStripMenuItem.Enabled = radioSideBySide.Checked;
        }

        private void radioOriginal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioOriginal.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Gray;
                disableSaving = false;
                setMyText();
                loadTable();
            }
        }

        private void setMyText()
        {
            this.Text = "Tuner: " + compareFiles[currentFile].tableInfos[0].td.TableName + " [";
            if (radioOriginal.Checked)
                this.Text += compareFiles[currentFile].pcm.FileName + "]";
            if (radioDifference.Checked || radioSideBySide.Checked || radioSideBySideText.Checked)
                this.Text += compareFiles[currentFile].pcm.FileName + " - " + compareFiles[currentCmpFile].pcm.FileName + "]";
            if (radioCompareFile.Checked)
                this.Text += compareFiles[currentCmpFile].pcm.FileName + "]";
            if (radioCompareAll.Checked)
                this.Text += compareFiles[currentFile].pcm.FileName + " - * ]";
        }

        private void numDecimals_ValueChanged(object sender, EventArgs e)
        {
            if ((radioDifference.Checked || radioDifference2.Checked) && radioMultiplier.Checked)
                multiplierDecimals = (int)numDecimals.Value;
            else
                decimals = (int)numDecimals.Value;
            loadTable();
        }

        private void dataFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.Font = dataFont;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                dataFont = fontDlg.Font;
                Properties.Settings.Default.TableEditorFont = dataFont;
                Properties.Settings.Default.Save();
            }
            fontDlg.Dispose();
            loadTable();
        }

        private void saveOBD2DescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            saveOBD2Codes();
        }
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void searchCodeFromGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData td = compareFiles[currentFile].tableInfos[0].td;
            if (!td.TableName.StartsWith("DTC"))
                return;
            string url = "https://www.google.com/search?q=Chevrolet+" + dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].HeaderCell.Value.ToString();
            System.Diagnostics.Process.Start(url);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void copyFromCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> rows = new List<int>();
                List<int> cols = new List<int>();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    rows.Add(dataGridView1.SelectedCells[i].RowIndex);
                    cols.Add(dataGridView1.SelectedCells[i].ColumnIndex);

                }
                dataGridView1.BeginEdit(true);
                for (int i = 0; i < rows.Count; i++)
                {
                    int row = rows[i];
                    int col = cols[i];
                    if (dataGridView1.Rows[row].Cells[col].Tag != null)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[row].Cells[col];
                        var val = dataGridView1.Rows[row].Cells[col + 1].Value;
                        dataGridView1.Rows[row].Cells[col].Value = val;
                    }
                }
                dataGridView1.EndEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }

        private void radioSideBySideText_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSideBySideText.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                loadTable();
                setMyText();
            }


        }

        private void addFileCheckBoxes()
        {
            if (fileCheckBoxes != null && fileCheckBoxes.Count == compareFiles.Count - 1)
            {
                for (int i = 0; i < fileCheckBoxes.Count; i++)
                    fileCheckBoxes[i].Visible = true;
                return;
            }
            int left = 186;
            fileCheckBoxes = new List<CheckBox>();
            for (int i=1; i < compareFiles.Count; i++)
            {
                CheckBox cBox = new CheckBox();
                cBox.Text = compareFiles[i].fileLetter;
                cBox.Checked = true;
                this.Controls.Add(cBox);
                fileCheckBoxes.Add(cBox);
                cBox.Location = new Point(left, 50);
                cBox.BringToFront();
                cBox.CheckedChanged += CBox_CheckedChanged;
                if (cBox.Text.Length == 1)
                    left += 30;
                else
                    left += 40;
            }
        }

        private void CBox_CheckedChanged(object sender, EventArgs e)
        {
            loadTable();
        }

        private void radioCompareAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCompareAll.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                addFileCheckBoxes();
                loadTable();
                setMyText();
            }
            else
            {
                for (int i = 0; i < fileCheckBoxes.Count; i++)
                    fileCheckBoxes[i].Visible = false;
            }
        }

        private void tuneCellValues(double step)
        {
            try
            {
                dataGridView1.BeginEdit(true);
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    TableCell tCell = (TableCell)dataGridView1.SelectedCells[i].Tag;
                    TableData mathTd = tCell.td;
                    double rawVal = (double)tCell.lastRawValue;
                    double newRawVal = rawVal + step;
                    Debug.WriteLine("Row: " + dataGridView1.SelectedCells[i].RowIndex + ", col: " + dataGridView1.SelectedCells[i].ColumnIndex + ", Old raw: " + tCell.lastRawValue + ", new raw: " + newRawVal);
                    tCell.saveValue(newRawVal, true);
                    double val = Convert.ToDouble(tCell.lastValue);

                    dataGridView1.SelectedCells[i].Value = val;
                    setCellColor(dataGridView1.SelectedCells[i].RowIndex, dataGridView1.SelectedCells[i].ColumnIndex, tCell);
                }
                this.dataGridView1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                dataGridView1.EndEdit();
                this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void numTuneValue_ValueChanged(object sender, EventArgs e)
        {
            if (radioDifference.Checked || radioDifference2.Checked)
                return;
            decimal oldVal = (decimal)numTuneValue.Tag;
            decimal newVal = numTuneValue.Value;
            Debug.WriteLine("Old:" + oldVal + ", new: " + newVal);
            if (newVal > oldVal) 
                tuneCellValues(1);
            else
                tuneCellValues(-1);
            numTuneValue.Tag = newVal;
        }

        private void radioAbsolute_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAbsolute.Checked)
            {
                disableSaving = false;
                loadTable();
            }
        }

        private void radioMultiplier_CheckedChanged(object sender, EventArgs e)
        {
            if (radioMultiplier.Checked)
            {
                numDecimals.Value = multiplierDecimals;
                disableSaving = false;
                loadTable();
            }
            else
            {
                numDecimals.Value = decimals;
            }
        }

        private void radioPercent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPercent.Checked)
            {
                //disableSaving = true;
                loadTable();
            }
        }

        private void radioDifference2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDifference.Checked || radioDifference2.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                disableSaving = false;
                setMyText();
                groupDifference.Visible = true;
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = multiplierDecimals;
                }
                loadTable();
            }
            else
            {
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = decimals;
                }

                groupDifference.Visible = false;
            }

        }

        private void copyTableFromCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                for (int id = 0; id < compareFiles[0].tableInfos.Count; id++)
                {
                    TableInfo ti = compareFiles[0].tableInfos[id];
                    TableInfo cmpTi;
                    int cmpId = compareFiles[currentCmpFile].refTableIds[(int)ti.td.id];
                    if (cmpId > -1)
                    {
                        int pos = compareFiles[currentCmpFile].tableIds.IndexOf(cmpId);
                        cmpTi = compareFiles[currentCmpFile].tableInfos[pos];
                        for (int cell = 0; cell < ti.tableCells.Count; cell++)
                        {
                            ti.tableCells[cell].saveValue(Convert.ToDouble(cmpTi.tableCells[cell].lastValue));
                        }
                    }
                }
                loadTable();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void chkRawHex_CheckedChanged(object sender, EventArgs e)
        {
            showRawHEXValuesToolStripMenuItem.Checked = chkRawHex.Checked;
            loadTable();
        }

        private void pasteSpecialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSpecial();
        }

        private void PasteSpecial()
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

                frmPasteSpecial fps = new frmPasteSpecial();
                if (fps.ShowDialog() != DialogResult.OK)
                    return;

                string cellPosMath = "X";
                string cellNegMath = "X";
                if (fps.radioAdd.Checked)
                {
                    cellPosMath = "X+C";
                    cellNegMath = "X+C";
                }
                else if (fps.radioMultiply.Checked)
                {
                    cellPosMath = "C*X";
                    cellNegMath = "C*X";
                }
                else if (fps.radioPercent.Checked)
                {
                    cellPosMath = "(100+C)/100*X";
                    cellNegMath = "(100+C)/100*X";
                }
                else if (fps.radioTarget.Checked)
                {
                    double target = Convert.ToDouble(fps.txtTarget.Text, System.Globalization.CultureInfo.InvariantCulture);
                    cellPosMath = "C/" + target.ToString()+"*X";
                    cellNegMath = "C/" + target.ToString() + "*X"; 
                }
                else if (fps.radioCustom.Checked)
                {
                    cellPosMath = fps.txtCustomPositive.Text;
                    cellNegMath = fps.txtCustomNegative.Text;
                }

                dataGridView1.BeginEdit(true);

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
                            //if ((chkPasteToSelectedCells.Checked && cell.Selected) || (!chkPasteToSelectedCells.Checked))
                            double cbVal;
                            if (Double.TryParse(cbValue[rowKey][cellKey].ToString(), out cbVal))
                            {
                                //double cbVal = Convert.ToDouble(cbValue[rowKey][cellKey], System.Globalization.CultureInfo.InvariantCulture, out cbVal);
                                string mathTxt = "X";
                                if (cbVal >= 0)
                                    mathTxt = cellPosMath.Replace("X", cell.Value.ToString());
                                else
                                    mathTxt = cellNegMath.Replace("X", cell.Value.ToString());
                                mathTxt = mathTxt.Replace("C", cbVal.ToString());
                                Debug.WriteLine(mathTxt);
                                cell.Value = parser.Parse(mathTxt);
                            }
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
                dataGridView1.EndEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }


        /*        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
if (dataGridView1.IsCurrentCellInEditMode && dataGridView1.SelectedCells[0].GetType() != typeof(DataGridViewComboBoxCell))
{
TableCell tc = (TableCell)dataGridView1.SelectedCells[0].Tag;
double newValue = tc.lastRawValue;

if (keyData == Keys.Up)
 newValue++;
else if (keyData == Keys.Down)
 newValue--;
else if (keyData == Keys.PageUp)
 newValue += 10;
else if (keyData == Keys.PageDown)
 newValue -= 10;
else
 return base.ProcessCmdKey(ref msg, keyData);

string mathStr = tc.td.Math.ToLower();
double newVal = safeCalc(mathStr, newValue, tc.td);
dataGridView1.SelectedCells[0].Value = newVal;
Debug.WriteLine(newVal);
return true;
}
return base.ProcessCmdKey(ref msg, keyData);
}
*/
    }
}