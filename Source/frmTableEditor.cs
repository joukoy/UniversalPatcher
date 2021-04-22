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
                string[] nParts = fullName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
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
        private List<CompareFile> compareFiles = new List<CompareFile>();

        public string tableName = "";
        private bool disableSaving = false;
        Font dataFont;

        private bool only1d = false;
        public bool disableMultiTable = false;
        public bool multiSelect = false;
        private bool duplicateTableName = false;
        List<TableData> filteredTables;
        public int currentFile = 0;
        public int currentCmpFile = 1;

        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            if (Properties.Settings.Default.TableEditorFont == null)
                dataFont = new Font("Consolas", 9);
            else
                dataFont = Properties.Settings.Default.TableEditorFont;

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
                MessageBox.Show("Error, line " + line + ": " + ex.Message, "Error");
            }
        }

        private void parseTableInfo(CompareFile cmpFile)
        {
            PcmFile pcm = cmpFile.pcm;
            for (int tId =0; tId < cmpFile.tableIds.Count; tId++)
            {
                TableData tData = pcm.tableDatas[cmpFile.tableIds[tId]];
                TableInfo tInfo = new TableInfo(pcm, tData);
                tInfo.compareFile = cmpFile;

                int rowCount = tData.Rows;
                int colCount = tData.Columns;

                int elementSize = getBits(tData.DataType) / 8;
                int bufSize = (int)(tData.Rows * tData.Columns * elementSize + tData.Offset);
                cmpFile.buf = new byte[bufSize];
                Array.Copy(pcm.buf, tData.addrInt, cmpFile.buf, 0, bufSize);
                cmpFile.tableBufferOffset = tData.addrInt;

                if (tData.TableName.ToLower().EndsWith(".data"))
                {
                    rowCount = getRowCountFromTable(tData, pcm);
                    colCount = getColumnsFromTable(tData, pcm);
                }

                string[] cHeaders = tData.ColumnHeaders.Split(',');
                if (tData.ColumnHeaders.StartsWith("Table: "))
                {
                    //string[] parts = tData.ColumnHeaders.Split(' ');
                    cHeaders = loadHeaderFromTable(tData.ColumnHeaders.Substring(7), tData.Columns, pcm).Split(',');
                }

                string[] rHeaders = tData.RowHeaders.Split(',');
                if (tData.RowHeaders.StartsWith("Table: "))
                {
                    //string[] parts = tData.RowHeaders.Split(' ');
                    rHeaders = loadHeaderFromTable(tData.RowHeaders.Substring(7), tData.Rows, pcm).Split(',');
                }

                string RowPrefix = "";
                string colPrefix = "";
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


                List<string> colHeaders = new List<string>();
                List<string> rowHeaders = new List<string>();
                for (int c=0; c<tData.Columns; c++)
                {
                    string cHdr = "";
                    if (cHeaders.Length >= c + 1 && cHeaders[c].Length > 0)
                        cHdr = cHeaders[c];
                    else
                        cHdr = tData.Units + " " + c.ToString();
                    if (duplicateTableName)
                        cHdr += " [" + tData.id + "]";
                    colHeaders.Add(colPrefix + cHdr);
                }
                for (int r = 0; r < tData.Rows; r++)
                {
                    string rHdr = "";
                    if (rHeaders.Length >= r + 1 && rHeaders[r].Length > 0)
                        rHdr = rHeaders[r];
                    else
                        rHdr = "(" + r.ToString() + ")";
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
                            tc.lastValue = getValue(pcm.buf, addr, tData, 0, pcm);
                            tc.lastRawValue = getRawValue(pcm.buf, addr, tData, 0);
                            tc.origValue = tc.lastValue;
                            tc.addr = addr;
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
                            tc.lastValue = getValue(pcm.buf, addr, tData, 0, pcm);
                            tc.lastRawValue = getRawValue(pcm.buf, addr, tData, 0);
                            tc.origValue = tc.lastValue;
                            tc.addr = addr;
                            addr += (uint)step;
                            tInfo.tableCells.Add(tc);
                        }

                    }

                }                
                cmpFile.tableInfos.Add(tInfo);
            }
            compareFiles.Add(cmpFile);
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
            TableInfo tInfo = new TableInfo(pcm, td);
            CompareFile orgFile = new CompareFile(pcm);
            orgFile.fileLetter = fileLetter;
            radioOriginal.Text = fileLetter;

            if (tableIds == null)
            {
                tableIds = new List<int>();
                tableIds.Add((int)td.id);
            }

            if (tableIds.Count > 1)
            {
                multiSelect = true;
                prepareMultiTable(orgFile,td,tableIds);
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
                if (td.TableName.Contains("[") || td.TableName.Contains("."))
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
                                prepareMultiTable(orgFile,pcm.tableDatas[t],null);
                                return;
                            }
                        }
                    }
                }
            }
            orgFile.tableIds = tableIds;
            parseTableInfo(orgFile);
        }


        private void prepareMultiTable(CompareFile cmpFile, TableData tData, List<int> tableIds)
        {
            int maxCols = 0;
            int maxRows = 0;
            int cols = 0;
            int rows = 0;

            if (tableIds != null && tableIds.Count > 1)
            {
                //Manually selected multiple tables
                filteredTables = new List<TableData>();
                tableIds.Sort();

                List<string> tableNameList = new List<string>();
                for (int i = 0; i < tableIds.Count; i++)
                {
                    TableData mTd =cmpFile.pcm.tableDatas[tableIds[i]];
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
                    filteredTables.Add(cmpFile.pcm.tableDatas[tableIds[i]]);
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
                filteredTables = new List<TableData>(results.ToList());
                filteredTables = filteredTables.OrderBy(o => o.addrInt).ToList();
                cols = filteredTables.Count;
                rows = filteredTables[0].Rows;
                tableIds = new List<int>();
                for (int i = 0; i < filteredTables.Count; i++)
                    tableIds.Add((int)filteredTables[i].id);
            }

            cmpFile.tableIds = tableIds;
            cmpFile.Rows = rows;
            cmpFile.Cols = cols;
            parseTableInfo(cmpFile);
        }

        private void modifyRadioText(string menuTxt)
        {
            string selectedBin = menuTxt.Substring(0, 1);
            radioCompareFile.Text = selectedBin;
            radioDifference.Text = radioOriginal.Text + " > " + selectedBin;
            radioSideBySide.Text = radioOriginal.Text + " | " + selectedBin;
            radioSideBySideText.Text = radioOriginal.Text + " [" + selectedBin + "]";
        }

        public void addCompareFiletoMenu(PcmFile cmpPCM, TableData _compareTd, string menuTxt)
        {
            CompareFile cmpFile = new CompareFile(cmpPCM);
            ToolStripMenuItem menuitem = new ToolStripMenuItem(cmpPCM.FileName);
            menuitem.Tag = cmpFile;
            menuitem.Name = Path.GetFileName(cmpPCM.FileName);
            if (menuTxt.Length > 0)
            {
                menuitem.Text = menuTxt;
                cmpFile.fileLetter = menuitem.Text.Substring(0, 1);
            }
            else
            {
                char lastFile = 'A';
                foreach (ToolStripMenuItem mi in compareToolStripMenuItem.DropDownItems)
                    lastFile++;
                menuitem.Text = lastFile.ToString() +": "+ cmpPCM.FileName;
                cmpFile.fileLetter = lastFile.ToString();
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

        public void prepareCompareTable(CompareFile cmpFile)
        {
            for (int i = 0; i < compareFiles[0].tableIds.Count; i++)
            {
                int id = compareFiles[0].tableIds[i];
                int cmpId = id;
                if (cmpFile.pcm.OS == compareFiles[0].pcm.OS)
                {
                    cmpFile.tableIds.Add(id);
                }
                else
                {
                    cmpId = findTableDataId(compareFiles[0].pcm.tableDatas[compareFiles[0].tableIds[i]], cmpFile.pcm.tableDatas);
                    if (cmpId > -1)
                        cmpFile.tableIds.Add(cmpId);
                }
                cmpFile.refTableIds.Add(id,cmpId);
            }
            parseTableInfo(cmpFile);
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
            //prepareCompareTable(cmpFile); //Not again
            loadTable();
            setMyText();
        }

        public void loadSeekTable(int tId, PcmFile pcm)
        {


            CompareFile cmpFile = new CompareFile(pcm);

            if (!compareFiles[0].pcm.seekTablesImported)
                compareFiles[0].pcm.importSeekTables();
            TableSeek tSeek = tableSeeks[compareFiles[0].pcm.foundTables[tId].configId];
            this.Text = "Table Editor: " + compareFiles[0].pcm.foundTables[tId].Name;

            FoundTable ft = compareFiles[0].pcm.foundTables[tId];
            for (int f=0; f< compareFiles[0].pcm.tableDatas.Count; f++)
            {
                if (pcm.tableDatas[f].TableName == tSeek.Name && pcm.tableDatas[f].addrInt == ft.addrInt)
                {
                    prepareTable(pcm, pcm.tableDatas[f], null,"A");
                    loadTable();
                    break;
                }
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
                double curVal = (double)tCell.lastValue;
                double origVal = (double)tCell.origValue;
                double curRawValue = (UInt64)tCell.lastRawValue;
                double cmpRawValue = UInt64.MaxValue;
                double cmpVal = double.MinValue;
                bool showSidebySide = radioSideBySide.Checked;
                if (cmpTCell == null)
                    showSidebySide = false;
                if (cmpTCell != null && !radioOriginal.Checked)
                {
                    cmpVal = (double) cmpTCell.lastValue;
                    cmpRawValue = cmpTCell.lastRawValue;
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
                            curTxt = Convert.ToChar((ushort)curVal).ToString();
                            if (cmpVal > double.MinValue)
                                cmpTxt = Convert.ToChar((ushort)cmpVal).ToString();
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
                    showVal = curVal - cmpVal;
                    showRawVal = curRawValue - cmpRawValue;
                }    

                if (showRawHEXValuesToolStripMenuItem.Checked)
                {
                    dataGridView1.Rows[row].Cells[col].Value = (uint)showRawVal;
                }
                else if (mathTd.OutputType == OutDataType.Text)
                {
                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToChar((ushort)showRawVal);
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
                LoggerBold("frmTableEditor error, line " + line + ": " + ex.Message);
            }

        }

        private void setCellColor(int row, int col, TableCell tCell)
        {
            TableData mathTd = tCell.td;
            double curVal = Convert.ToDouble(tCell.lastValue);
            double origVal = Convert.ToDouble(tCell.origValue);
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

            if (only1d)
                hdrTxt = "-";
            for (int c = 0; c < dataGridView1.Columns.Count; c++)
            {
                if (dataGridView1.Columns[c].HeaderText == hdrTxt)
                    ind = c;
            }
                
            if (ind < 0)
            {
                ind = dataGridView1.Columns.Add(hdrTxt, hdrTxt);
/*                if (radioSideBySide.Checked)
                {
                    hdrTxt = "(" + hdrTxt + ")";
                    int cmpCol = dataGridView1.Columns.Add(hdrTxt, hdrTxt);
                    dataGridView1.Columns[cmpCol].HeaderCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                }
*/
            }
            return ind;
        }

        private int getRowByHeader(string hdrTxt)
        {
            int ind = int.MinValue;

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
            if (radioSideBySideText.Checked)
                return;
            try
            {
                TableValueType vt = getValueType(ft);
                if (vt == TableValueType.boolean)
                {
                    DataGridViewCheckBoxCell dgc = new DataGridViewCheckBoxCell();
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
                }

            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, line " + line + ": " + ex.Message);

            }
        }

        public void loadTable()
        {
            try
            {

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

                string prefix = "";
                string tblNamePrefix = "";
                string cmpPrefix = "";
                string rowPrefix = "";

                List<CompareFile> cmpFiles = new List<CompareFile>();
                CompareFile diffFile = null;
                if (radioDifference.Checked)
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
                    prefix = "[" +selectedFile.fileLetter + "] ";
                    cmpFiles.Add(diffFile);
                }
                if (radioCompareAll.Checked)
                {
                    for (int i=1; i< compareFiles.Count; i++)
                    {
                        cmpFiles.Add(compareFiles[i]);
                    }
                    prefix = "[" + selectedFile.fileLetter + "] ";
                }

                CompareFile sFile = compareFiles[currentFile];

                for (int tbl = 0; tbl < sFile.tableInfos.Count; tbl++)
                {
                    TableData ft = sFile.tableInfos[tbl].td;
                    TableInfo cmpTinfo = null;
                    if (only1d)
                    {
                        rowPrefix = "[" + ft.TableName + "] ";
                    }

                    int gridCol = 0;
                    int gridRow = 0;

                    for (int cell = 0; cell < sFile.tableInfos[tbl].tableCells.Count; cell++)
                    {
                        TableCell tcell = sFile.tableInfos[tbl].tableCells[cell];
                        TableCell cmpCell = null;
                        if (multiSelect)
                        {
                            tblNamePrefix = "[" + tcell.td.TableName + "] ";
                        }
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
                                cmpCell = cmpTinfo.tableCells[cell];
                            }
                        }
                        if (!xySwapped)
                        {
                            gridCol = getColumnByHeader(prefix + tblNamePrefix +  tcell.ColHeader);
                            gridRow = getRowByHeader(rowPrefix + tcell.RowhHeader);
                        }
                        else
                        {
                            gridCol = getColumnByHeader(rowPrefix + prefix + tblNamePrefix + tcell.RowhHeader);
                            gridRow = getRowByHeader(tcell.ColHeader);
                        }
                        addCellByType(ft, gridRow, gridCol);
                        setCellValue(gridRow, gridCol, tcell, cmpCell);

                        for (int d=0; d < cmpFiles.Count; d++)
                        {
                            int cmpId = -1;
                            if (cmpFiles[d].refTableIds.ContainsKey(sFile.tableIds[tbl]))
                                cmpId = cmpFiles[d].refTableIds[sFile.tableIds[tbl]];
                            if (cmpId > -1)
                            {
                                int pos = cmpFiles[d].tableIds.IndexOf(cmpId);
                                cmpTinfo = cmpFiles[d].tableInfos[pos];
                                TableData cmpTd = cmpFiles[d].pcm.tableDatas[cmpId];
                                cmpCell = cmpTinfo.tableCells[cell];
                                cmpPrefix = "[" + cmpFiles[d].fileLetter + "] ";
                                if (multiSelect)
                                    tblNamePrefix = "[" + cmpTd.TableName + "] ";

                                if (!xySwapped)
                                {
                                    gridCol = getColumnByHeader(cmpPrefix + tblNamePrefix + cmpCell.ColHeader);
                                    gridRow = getRowByHeader(cmpCell.RowhHeader);
                                }
                                else
                                {
                                    gridCol = getColumnByHeader(cmpPrefix + tblNamePrefix + cmpCell.RowhHeader);
                                    gridRow = getRowByHeader(cmpCell.ColHeader);
                                }
                                addCellByType(cmpCell.td, gridRow, gridCol);
                                setCellValue(gridRow, gridCol, cmpCell, null);
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

        private void showDtcDescriptions()
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
            dataGridView1.Columns.Insert(0,dgc);
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                string descr = ds.getDtcDescription(dataGridView1.Rows[r].HeaderCell.Value.ToString());
                dataGridView1.Rows[r].Cells["Description"].Value = descr;
                TableCell tc = (TableCell)dataGridView1.Rows[r].Cells[1].Tag ;
                tc.addr = uint.MaxValue - 1;
                dataGridView1.Rows[r].Cells["Description"].Tag = tc;
            }
        }

        public int getColumnsFromTable(TableData tData, PcmFile pcm)
        {
            int cols = tData.Columns;

            string yTbName = tData.TableName.Replace(".Data", ".xVal");
            for (int y = 0; y < pcm.tableDatas.Count; y++)
            {
                if (pcm.tableDatas[y].TableName == yTbName)
                {
                    TableData ytb = pcm.tableDatas[y];
                    uint xaddr = (uint)(ytb.addrInt + ytb.Offset);
                    cols = (int)getValue(pcm.buf, xaddr, ytb,0, pcm);
                    break;
                }
            }

            return cols;

        }
        public int getRowCountFromTable(TableData tData, PcmFile pcm)
        {
            int rows = tData.Rows;

            for (int x=0; x< pcm.tableDatas.Count; x++)
            {
                if (pcm.tableDatas[x].TableName == tData.TableName.Replace(".Data", ".Size") || pcm.tableDatas[x].TableName == tData.TableName.Replace(".Data", ".yVal"))
                {
                    uint addr = (uint)(pcm.tableDatas[x].addrInt + pcm.tableDatas[x].Offset);
                    rows = (int)getValue(pcm.buf, addr, pcm.tableDatas[x],0, pcm);
                    break;
                }
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
                    this.numDecimals.ValueChanged += new System.EventHandler(this.numDecimals_ValueChanged);
                }
                string formatStr = "0";
                if (showRawHEXValuesToolStripMenuItem.Checked || td.OutputType == OutDataType.Hex)
                {
                    formatStr = "X" + ((int)numDecimals.Value).ToString() ;
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
                MessageBox.Show("Error, line " + line + ": " + ex.Message, "Error");
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


        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
                        bool codeFoumd = false;
                        for (int o = 0; o < OBD2Codes.Count; o++)
                        {
                            if (OBD2Codes[o].Code == oc.Code)
                            {
                                OBD2Codes[o].Description = oc.Description;
                                codeFoumd = true;
                                break;
                            }
                        }
                        if (!codeFoumd)
                        {
                            OBD2Codes.Add(oc);
                        }
                    }
                    else
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != tc.lastValue)
                            SaveValue(e.RowIndex, e.ColumnIndex, tc);
                    }
                    setCellColor(e.RowIndex, e.ColumnIndex, tc);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                MessageBox.Show("Error, line " + line + ": " + ex.Message, "Error");
            }
        }


        public double safeCalc(string mathStr, double X, TableData mathTd)
        {
            double retVal = double.MinValue;
            try
            {
                if (mathTd.DataType == InDataType.INT32)
                    X = (Int32)X;
                else if (mathTd.DataType == InDataType.INT64)
                    X = (Int64)X;
                else if (mathTd.DataType == InDataType.SBYTE)
                    X = (sbyte)X;
                else if (mathTd.DataType == InDataType.SWORD)
                    X = (Int16)X;
                else if (mathTd.DataType == InDataType.UBYTE)
                    X = (byte)X;
                else if (mathTd.DataType == InDataType.UINT32)
                    X = (UInt32)X;
                else if (mathTd.DataType == InDataType.UINT64)
                    X = (UInt64)X;
                else if (mathTd.DataType == InDataType.UWORD)
                    X = (UInt16)X;
                string newMathStr = mathStr.ToLower().Replace("x", X.ToString());
                retVal = parser.Parse(newMathStr, false);

                if (mathTd.OutputType == OutDataType.Hex || mathTd.OutputType == OutDataType.Int)
                    retVal = (Int64)retVal;
                else if (mathTd.Decimals > 0)
                {
                    retVal = Math.Round(retVal, mathTd.Decimals);
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

            return retVal;
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
                    else
                    {
                        newValue = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
                    }
                }
                if (newValue == double.MaxValue) return;

                if (!showRawHEXValuesToolStripMenuItem.Checked)
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
                    string mathStr = mathTd.Math.ToLower();
                    double calcValue = safeCalc(mathStr, tCell.lastRawValue, mathTd);
                    dataGridView1.Rows[r].Cells[c].Value = calcValue;
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
                MessageBox.Show("Error, line " + line + ": " + ex.Message, "Error");
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
                //tableModified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void AutoResize()
        {
            int dgv_width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + dataGridView1.RowHeadersWidth;
            if (dgv_width < 500) dgv_width = 500;
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
                        //if ((chkPasteToSelectedCells.Checked && cell.Selected) || (!chkPasteToSelectedCells.Checked))
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
                MessageBox.Show("Error, line " + line + ": " + ex.Message, "Error");
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
            if (showRawHEXValuesToolStripMenuItem.Checked)
                showRawHEXValuesToolStripMenuItem.Checked = false;
            else
                showRawHEXValuesToolStripMenuItem.Checked = true;
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
            TableData td = compareFiles[currentFile].tableInfos[0].td;
            frmGraphics fg = new frmGraphics();
            fg.Text = td.TableName;
            fg.Show();
            fg.chart1.Series.Clear();
            double minVal = double.MaxValue;
            double maxVal = double.MinValue;

            for (int r=0; r<dataGridView1.Rows.Count; r++)
            {
                fg.chart1.Series.Add(new Series());
                fg.chart1.Series[r].ChartType = SeriesChartType.Line;                
                if (dataGridView1.Rows[r].HeaderCell.Value != null)
                    fg.chart1.Series[r].Name = dataGridView1.Rows[r].HeaderCell.Value.ToString();
                fg.chart1.Series[r].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                int point = 0;
                for (int c=0; c< dataGridView1.Columns.Count; c++)
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
            if (radioDifference.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                disableSaving = true;
                setMyText();
                loadTable();
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
            this.Text = "Tuner: " + tableName + " [";
            if (radioOriginal.Checked)
                this.Text += compareFiles[currentFile].pcm.FileName + "]";
            if (radioDifference.Checked || radioSideBySide.Checked)
                this.Text += compareFiles[currentFile].pcm.FileName;
            if (radioCompareFile.Checked)
            {
                int id = findFile(radioCompareFile.Text);
                this.Text += compareFiles[id].pcm.FileName + "]";
            }
        }

        private void numDecimals_ValueChanged(object sender, EventArgs e)
        {
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
            List<int> rows = new List<int>();
            List<int> cols = new List<int>();
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                rows.Add(dataGridView1.SelectedCells[i].RowIndex);
                cols.Add(dataGridView1.SelectedCells[i].ColumnIndex);

            }
            for (int i=0; i< rows.Count; i++)
            { 
                int row = rows[i];
                int col = cols[i];
                if (dataGridView1.Rows[row].Cells[col].Tag != null)  
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[row].Cells[col];
                    dataGridView1.BeginEdit(true);
                    var val = dataGridView1.Rows[row].Cells[col + 1].Value;
                    dataGridView1.Rows[row].Cells[col].Value = val;
                    dataGridView1.EndEdit();
                }
            }
        }

        private void radioSideBySideText_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSideBySideText.Checked)
            {
                dataGridView1.BackgroundColor = Color.Red;
                loadTable();
            }


        }

        private void radioCompareAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCompareAll.Checked)
            {
                dataGridView1.BackgroundColor = Color.Red;
                loadTable();
            }
        }

        private void tuneCellValues(int step)
        {
            dataGridView1.BeginEdit(true);
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                TableCell tCell = (TableCell)dataGridView1.SelectedCells[i].Tag;
                TableData mathTd = tCell.td;
                double rawVal = tCell.lastRawValue;
                double minRawVal = getMinValue(mathTd.DataType);
                double maxRawVal = getMaxValue(mathTd.DataType);
                if (step > 0 && rawVal == maxRawVal)
                    return;
                if (step < 0 && rawVal == minRawVal)
                    return;
                string mathStr = tCell.td.Math;
                if (mathStr.Contains("table:"))
                {
                    mathStr = readConversionTable(mathStr, tCell.tableInfo.pcm);
                }
                if (mathStr.Contains("raw:"))
                {
                    mathStr = readConversionRaw(mathStr, tCell.tableInfo.pcm);
                }
                rawVal += step;
                tCell.saveValue(rawVal, true);
                double val = safeCalc(mathStr, tCell.lastRawValue, tCell.td);

                dataGridView1.SelectedCells[i].Value = val;   
                setCellColor(dataGridView1.SelectedCells[i].RowIndex, dataGridView1.SelectedCells[i].ColumnIndex, tCell);
            }
            dataGridView1.EndEdit();
        }
        private void btnIncrease_Click(object sender, EventArgs e)
        {
            tuneCellValues(1);
        }

        private void btnDecrease_Click(object sender, EventArgs e)
        {
            tuneCellValues(-1);
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