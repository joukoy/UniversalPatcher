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

        private class Tagi
        {
            public uint addr { get; set;}
            public int id { get; set; }
        }
        private TableData td;
        private PcmFile PCM;
        //private bool tableModified = false;
        private bool commaDecimal = true;
        private byte[] dataBuffer;
        private uint bufSize = 0;
        MathParser parser = new MathParser();
        Dictionary<double, string> possibleVals = new Dictionary<double, string>();

        List<TableData> filteredTables;
        bool combo = false;

        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

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
            //tableModified = false;
            disableTooltipsToolStripMenuItem.Checked = false;
        }
        public void loadSeekTable(int tId, PcmFile PCM1)
        {

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            NumberFormatInfo nfi = new CultureInfo(currentCulture, false).NumberFormat;
            if (nfi.NumberDecimalSeparator == ",") commaDecimal = true;
            else commaDecimal = false;

            PCM = PCM1;
            TableSeek tSeek = tableSeeks[foundTables[tId].configId];
            this.Text = "Table Editor: " + foundTables[tId].Name;
            //if (foundTables[tId].Description.Length > 0)
            //    this.Text += " - " + foundTables[tId].Description;

            FoundTable ft = foundTables[tId];

            td = new TableData();
            td.addrInt = ft.addrInt;
            //td.Address = ft.Address;
            td.Category = ft.Category;
            //td.Floating = tSeek.Floating;
            //td.ElementSize = (byte)(tSeek.Bits / 8);
            //td.Signed = tSeek.Signed;
            td.DataType = tSeek.DataType;
            td.Math = tSeek.Math;
            td.SavingMath = tSeek.SavingMath;
            td.OS = PCM.OS;
            td.RowMajor = tSeek.RowMajor;
            td.Rows = ft.Rows;
            td.Columns = ft.Columns;
            td.ColumnHeaders = tSeek.ColHeaders;
            td.RowHeaders = tSeek.RowHeaders;
            td.Decimals = tSeek.Decimals;            
            td.TableDescription = tSeek.Description;
            td.TableName = ft.Name;
            td.Units = tSeek.Units;

            loadTable(td, PCM);
        }


        private double getValue(uint addr, TableData mathTd)
        {
            double value = 0;
            UInt32 bufAddr = addr - td.addrInt;
            if (mathTd.DataType == InDataType.SBYTE)
                value = (sbyte)dataBuffer[bufAddr];
            if (mathTd.DataType == InDataType.UBYTE)
                value = dataBuffer[bufAddr];
            if (mathTd.DataType == InDataType.SWORD)
                value = BEToInt16(dataBuffer,bufAddr);
            if (mathTd.DataType == InDataType.UWORD)
                value = BEToUint16(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT32)
                value = BEToInt32(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UINT32)
                value = BEToUint32(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT64)
                value = BEToInt64(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UINT64)
                value = BEToUint64(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.FLOAT32)
                value = BEToFloat32(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.FLOAT64)
                value = BEToFloat64(dataBuffer, bufAddr);

            double multiplier = 1;
            if (td.TableName.EndsWith(".Data") && !mathTd.TableName.EndsWith(".xVal") && !mathTd.TableName.EndsWith(".yVal"))
            {
                string xTbName = td.TableName.Replace(".Data", ".xVal");
                for (int x = 0; x < tableDatas.Count; x++)
                {
                    if (tableDatas[x].TableName == xTbName)
                    {
                        TableData xtb = tableDatas[x];
                        uint xaddr = (uint)(xtb.addrInt + xtb.Offset);
                        multiplier = getHeaderValue(xaddr, xtb);    //GetHeaderValue gets from full buffer, not from tablebuffer
                        break;
                    }
                }
            }


            string mathStr = mathTd.Math.ToLower().Replace("x", value.ToString());
            if (commaDecimal) mathStr = mathStr.Replace(".", ",");
            value = parser.Parse(mathStr, false);
            value = value * multiplier;
            return value;
        }

        private UInt64 getRawValue(UInt32 addr, TableData mathTd)
        {
            UInt32 bufAddr = addr - td.addrInt; 
            if (mathTd.DataType == InDataType.UWORD || mathTd.DataType == InDataType.SWORD)
                return BEToUint16(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT32 || mathTd.DataType == InDataType.UINT32 || mathTd.DataType == InDataType.FLOAT32)
                return BEToUint32(dataBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT64 || mathTd.DataType == InDataType.UINT64 || mathTd.DataType == InDataType.FLOAT64)
                return BEToUint64(dataBuffer, bufAddr);
            return dataBuffer[bufAddr];
        }

        public void setCellValue(uint addr, int row, int col, TableData mathTd)
        {
            if (showRawHEXValuesToolStripMenuItem.Checked)
                dataGridView1.Rows[row].Cells[col].Value = getRawValue(addr, mathTd);
            else if (td.OutputType == OutDataType.Text)
                dataGridView1.Rows[row].Cells[col].Value = Convert.ToChar((ushort)getValue(addr, mathTd));
            else if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0)
            {
                UInt64 mask = Convert.ToUInt64(mathTd.BitMask.Replace("0x", ""),16);
                if ((getRawValue(addr, mathTd) & mask) == mask)
                {
                    Debug.WriteLine(mathTd.TableName + ": " + mathTd.BitMask + ", mask: " + mask.ToString("X")+ ", Data: " + getRawValue(addr, mathTd).ToString("X") + " Row: " + row + ", Col: " + col + ", true");
                    dataGridView1.Rows[row].Cells[col].Value = 1 ;
                }
                else
                {
                    Debug.WriteLine(mathTd.TableName + ": " + mathTd.BitMask + " mask: " + mask.ToString("X") +  ", Data: " + getRawValue(addr, mathTd).ToString("X") + " Row: " + row + ", Col: " + col + ", false");
                    dataGridView1.Rows[row].Cells[col].Value = 0;
                }
            }
            else if (combo)
            {
                double val = getValue(addr, mathTd);
                if (!possibleVals.ContainsKey(val))
                    val = double.MaxValue;
                dataGridView1.Rows[row].Cells[col].Value = val;
            }
            else
                dataGridView1.Rows[row].Cells[col].Value = getValue(addr, mathTd);

            Tagi t = new Tagi();
            t.addr = addr;
            t.id = (int)mathTd.id;
            dataGridView1.Rows[row].Cells[col].Tag = t;

            if (!disableTooltipsToolStripMenuItem.Checked && mathTd.TableDescription != null)
            {
                if (mathTd.TableDescription.Length > 200)
                    dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription.Substring(0, 200);
                else
                    dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription;
            }
        }

        private double getHeaderValue(uint addr, TableData mathTd, bool useMultiplier = false)
        {
            double value = 0;
            if (mathTd.DataType == InDataType.SBYTE)
                value = (sbyte)PCM.buf[addr];
            if (mathTd.DataType == InDataType.UBYTE)
                value = PCM.buf[addr];
            if (mathTd.DataType == InDataType.SWORD)
                value = BEToInt16(PCM.buf, addr);
            if (mathTd.DataType == InDataType.UWORD)
                value = BEToUint16(PCM.buf, addr);
            if (mathTd.DataType == InDataType.INT32)
                value = BEToInt32(PCM.buf, addr);
            if (mathTd.DataType == InDataType.UINT32)
                value = BEToUint32(PCM.buf, addr);
            if (mathTd.DataType == InDataType.INT64)
                value = BEToInt64(PCM.buf, addr);
            if (mathTd.DataType == InDataType.UINT64)
                value = BEToUint64(PCM.buf, addr);
            if (mathTd.DataType == InDataType.FLOAT32)
                value = BEToFloat32(PCM.buf, addr);
            if (mathTd.DataType == InDataType.FLOAT64)
                value = BEToFloat64(PCM.buf, addr);

            double multiplier = 1;
            if (useMultiplier && td.TableName.EndsWith(".Data") && !mathTd.TableName.EndsWith(".xVal") && !mathTd.TableName.EndsWith(".yVal"))
            {
                string yTbName = td.TableName.Replace(".Data", ".yVal");
                for (int y=0; y<tableDatas.Count; y++)
                {
                    if (tableDatas[y].TableName == yTbName)
                    {
                        TableData ytb = tableDatas[y];
                        uint xaddr = (uint)(ytb.addrInt + ytb.Offset);
                        multiplier = getHeaderValue(xaddr, ytb);
                        break;
                    }
                }                
            }

            string mathStr = mathTd.Math.ToLower().Replace("x", value.ToString());
            if (commaDecimal) mathStr = mathStr.Replace(".", ",");
            value = parser.Parse(mathStr, false);
            value = value * multiplier;
            return value;
        }

        private string loadHeaderFromTable(string tableName, int count, bool useMultiplier = false)
        {
            string headers = "" ;
            for (int i=0; i < tableDatas.Count; i++)
            {
                TableData t = tableDatas[i];
                if (t.TableName == tableName)
                {
                    uint step = (uint)(getBits(t.DataType) / 8);
                    uint addr = (uint)(t.addrInt + t.Offset);
                    for (int a = 0; a < count; a++ )
                    {
                        headers += t.Units.Trim() + " " +  getHeaderValue(addr,t,useMultiplier).ToString().Replace(",",".") + ",";
                        addr += step;
                    }
                    headers = headers.Trim(',');
                    break;
                }
            }
            return headers;
        }

        private void parseEnumHeaders(TableData hTd)
        {
            if (possibleVals.Count > 0) return;
            string[] posVals = hTd.RowHeaders.Split(',');
            for (int r = 0; r < posVals.Length; r++)
            {
                string[] parts = posVals[r].Split(':');
                double val = 0;
                double.TryParse(parts[0], out val);
                string txt = posVals[r];
                if (!possibleVals.ContainsKey(val))
                    possibleVals.Add(val, txt);
            }
            possibleVals.Add(double.MaxValue, "------------");
        }

        private int getColumnByTableData(TableData cTd,int col)
        {
            int ind = int.MinValue;
            string colName = "";

            if (cTd.Columns == dataGridView1.Columns.Count)
                return col;

            if (cTd.Columns == 1)
            {
                string[] tParts = cTd.TableName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                colName = tParts[tParts.Length - 1].Trim();
                if (cTd.ColumnHeaders.Length > 0)
                    colName += " " + cTd.ColumnHeaders.Trim(); 
            }
            else if (cTd.ColumnHeaders.Contains(','))
            {
                string[] tParts = cTd.ColumnHeaders.Split(',');
                if (tParts.Length >= (col -1))
                    colName = tParts[col].Trim();
                if (cTd.ColumnHeaders.Length > 0)
                    colName += " " + cTd.ColumnHeaders.Trim();
            }
            if (colName != "")
            {
                for (int c = 0; c < dataGridView1.Columns.Count; c++)
                {
                    if (dataGridView1.Columns[c].HeaderText == colName)
                    ind = c;
                }
                
            }
            return ind;
        }

        private int getRowByTableData(TableData cTd, int row)
        {
            int ind = int.MinValue;
            string rowName = "";

            if (cTd.Rows == dataGridView1.Rows.Count)
            {
                Debug.WriteLine("getRowByTableData: cTd.Rows == dataGridView1.Rows.Count");
                return row;
            }

            if (cTd.Rows == 1)
            {
                string[] tParts = cTd.TableName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (tParts.Length > 2)
                {
                    rowName = tParts[tParts.Length - 2].Trim();
                    Debug.WriteLine("getRowByTableData: By tablename: " + rowName);
                }
            }
            if (cTd.RowHeaders.Contains(','))
            {
                string[] tParts = cTd.RowHeaders.Split(',');
                if (tParts.Length >= (row - 1))
                {
                    rowName = tParts[row].Trim();
                    Debug.WriteLine("getRowByTableData: By current TD " + rowName);
                }
            }
            else if (td.RowHeaders.Contains(','))
            {
                string[] tParts = td.RowHeaders.Split(',');
                if (tParts.Length >= (row - 1))
                {
                    rowName = tParts[row].Trim();
                    Debug.WriteLine("getRowByTableData: By main TD " + rowName);
                }
            }
            if (rowName != "")
            {
                for (int c = 0; c < dataGridView1.Rows.Count; c++)
                {
                    if (dataGridView1.Rows[c].HeaderCell.Value.ToString() == rowName)
                        ind = c;
                }
            }
            return ind;
        }


        private void loadMultiTable(string tableName)
        {
            this.Text = "Table Editor: " + tableName;
            if (td.Units != null)
                labelUnits.Text = "Units: " + td.Units;


            swapXyToolStripMenuItem.Enabled = false;
            chkSwapXY.Enabled = false;

            var results = tableDatas.Where(t => t.TableName.StartsWith(tableName)); 
            filteredTables = new List<TableData>(results.ToList());
            filteredTables = filteredTables.OrderBy(o => o.addrInt).ToList();

            td = filteredTables[0];

            if (bufSize == 0)
            {
                int elementSize = getBits(td.DataType) / 8;
                int singleTableSize = td.Rows * td.Columns * elementSize;
                bufSize = (uint)(filteredTables[filteredTables.Count - 1].addrInt - filteredTables[0].addrInt + td.Offset + singleTableSize);
                dataBuffer = new byte[bufSize];
                Array.Copy(PCM.buf, td.addrInt, dataBuffer, 0, bufSize);
            }

            List<string> colHeaders = new List<string>();
            List<string> rowHeaders = new List<string>();
            List<bool> comboCols = new List<bool>();
            List<bool> flagCols = new List<bool>();

            for (int t = 0; t < filteredTables.Count; t++)
            {
                //Collect all different row & column labels from table names

                bool comboTb = false;
                bool flagCol = false;

                string[] tParts = filteredTables[t].TableName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (tParts.Length > 2)
                {
                    if (!rowHeaders.Contains(tParts[1]))
                        rowHeaders.Add(tParts[1].Trim());
                    string colHdr = tParts[2].Trim();
                    if (filteredTables[t].ColumnHeaders != "")
                        colHdr += " " + filteredTables[t].ColumnHeaders.Trim();
                    if (!colHeaders.Contains(colHdr))
                    {
                        colHeaders.Add(colHdr);
                        if (filteredTables[t].Rows < 2 && filteredTables[t].RowHeaders.Contains(","))
                        {
                            comboTb = true;
                            parseEnumHeaders(filteredTables[t]);
                        }
                        comboCols.Add(comboTb);
                        if (filteredTables[t].OutputType == OutDataType.Flag)
                            flagCol = true;
                        flagCols.Add(flagCol);
                    }
                }
                else
                {
                    string colHdr = tParts[1].Trim();
                    if (filteredTables[t].ColumnHeaders != "")
                        colHdr += " " + filteredTables[t].ColumnHeaders.Trim();
                    if (!colHeaders.Contains(colHdr))
                    {
                        colHeaders.Add(colHdr);
                        if (filteredTables[t].Rows < 2 && filteredTables[t].RowHeaders.Contains(","))
                        {
                            comboTb = true;
                            parseEnumHeaders(filteredTables[t]);
                        }
                        comboCols.Add(comboTb);
                        if (filteredTables[t].OutputType == OutDataType.Flag)
                            flagCol = true;
                        flagCols.Add(flagCol);
                    }
                }
            }


            if (rowHeaders.Count < 2 && td.Rows > 1)
            {
                if (filteredTables[0].RowHeaders.Contains(','))
                {
                    rowHeaders.Clear();
                    string[] rParts = filteredTables[0].RowHeaders.Split(',');
                    for (int r = 0; r < rParts.Length; r++)
                        rowHeaders.Add(rParts[r].Trim());
                }
            }


            if (td.ColumnHeaders.StartsWith("Table: "))
            {
                colHeaders.Clear();
                string[] parts = td.ColumnHeaders.Split(' ');
                string[] colHeaderStr = loadHeaderFromTable(parts[1], td.Columns).Split(',');
                bool comboTb = false;
                bool flagCol = false;
                if (td.OutputType == OutDataType.Flag)
                    flagCol = true;
                if (td.Rows < 2 && td.RowHeaders.Contains(","))
                    comboTb = true;
                for (int p = 0; p < colHeaderStr.Length; p++)
                {
                    colHeaders.Add(colHeaderStr[p]);
                    comboCols.Add(comboTb);
                    flagCols.Add(flagCol);
                }
            }

            if (td.RowHeaders.StartsWith("Table: "))
            {
                rowHeaders.Clear();
                string[] parts = td.RowHeaders.Split(' ');
                string[] rowHeaderStr = loadHeaderFromTable(parts[1], td.Rows).Split(',');
                for (int p = 0; p < rowHeaderStr.Length; p++)
                    rowHeaders.Add(rowHeaderStr[p]);
            }

            if (rowHeaders.Count == 0)
                rowHeaders.Add("");

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            for (int c = 0; c < colHeaders.Count; c++)
            {
                string hdrTxt = colHeaders[c];
                if (colHeaders.Count == 1)
                    dataGridView1.Columns[c].HeaderText = td.Units;

                if (flagCols[c])
                {
                    DataGridViewCheckBoxColumn col_chkbox = new DataGridViewCheckBoxColumn();
                    dataGridView1.Columns.Add(col_chkbox);
                }
                else if (comboCols[c])
                {
                    DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
                    comboCol.DataSource = new BindingSource(possibleVals, null);
                    comboCol.DisplayMember = "Value";
                    comboCol.ValueMember = "Key";
                    dataGridView1.Columns.Add(comboCol);
                }
                else
                {
                    dataGridView1.Columns.Add(hdrTxt, hdrTxt);
                }
                dataGridView1.Columns[c].HeaderText = hdrTxt;

            }

            for (int r = 0; r < rowHeaders.Count; r++)
            {
                dataGridView1.Rows.Add();
                if (rowHeaders.Count == 1)
                    dataGridView1.Rows[r].HeaderCell.Value = td.Units;
                else
                    dataGridView1.Rows[r].HeaderCell.Value = rowHeaders[r];
            }

            int gridRow = 0;
            for (int tbl = 0; tbl<filteredTables.Count; tbl++)
            {
                TableData ft = filteredTables[tbl];
                int elementsize = getElementSize(ft.DataType);
                int gridCol = 0;
                uint addr = (uint)(ft.addrInt + ft.Offset);
                if (ft.RowMajor)
                {
                    for (int r = 0; r < ft.Rows; r++)
                    {
                        if (dataGridView1.Rows.Count > 1)
                            gridRow = getRowByTableData(ft, r);
                        for (int c = 0; c < ft.Columns; c++)
                        {
                            if (dataGridView1.Columns.Count > 1)
                                gridCol = getColumnByTableData(ft, c);
                            setCellValue(addr, gridRow, gridCol, ft);
                            addr += (uint)elementsize;
                        }
                    }
                }
                else
                {
                    for (int c = 0; c < ft.Columns; c++)
                    {
                        for (int r = 0; r < ft.Rows; r++)
                        {
                            if (dataGridView1.Columns.Count > 1)
                                gridCol = getColumnByTableData(ft, c);
                            if (dataGridView1.Rows.Count > 1)
                                gridRow = getRowByTableData(ft, r);
                            setCellValue(addr, gridRow, gridCol, ft);
                            addr += (uint)elementsize;
                        }
                    }

                }
            }
            setDataGridLayout();
        }

        public int getRowCountFromTable()
        {
            int rows = td.Rows;

            for (int x=0; x< tableDatas.Count; x++)
            {
                if (tableDatas[x].TableName == td.TableName.Replace(".Data", ".Size"))
                {
                    uint addr = (uint)(tableDatas[x].addrInt + tableDatas[x].Offset);
                    rows = (int)getHeaderValue(addr, tableDatas[x]);
                    break;
                }
            }

            return rows;
        }
        public void loadTable(TableData td1, PcmFile PCM1)
        {
            try
            {
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                NumberFormatInfo nfi = new CultureInfo(currentCulture, false).NumberFormat;
                if (nfi.NumberDecimalSeparator == ",") commaDecimal = true;
                else commaDecimal = false;

                PCM = PCM1;
                td = td1;

                if (showRawHEXValuesToolStripMenuItem.Checked)
                    combo = false;

                if (td.TableName.Contains("[") || td.TableName.Contains("."))
                {
                    if (td.TableName.EndsWith(".Data") || td.TableName.EndsWith(".xVal") || td.TableName.EndsWith(".yVal") || td.TableName.EndsWith(".Size"))
                    {
                        //Special case, "Normal" table, but header values from tables, WITH different table as multiplier
                        Debug.WriteLine("Special case, not real multitable");
                    }
                    else
                    {
                        int location = 0;
                        if (td.TableName.Contains("["))
                            location = td.TableName.IndexOf('[');
                        else
                            location = td.TableName.LastIndexOf('.');
                        string tbName = td.TableName.Substring(0, location);
                        for (int t = 0; t < tableDatas.Count; t++)
                        {
                            if (tableDatas[t].TableName.StartsWith(tbName) && tableDatas[t].TableName != td.TableName)
                            {
                                //It is multitable
                                loadMultiTable(tbName);
                                return;
                            }
                        }
                    }
                }

                //uint addr = (uint)(td.addrInt + td.Offset);

                this.Text = "Table Editor: " + td.TableName;
                //if (td.TableDescription != null && td.TableDescription.Length > 0)
                //  this.Text += " - " + td.TableDescription;

                if (td.Units != null)
                    labelUnits.Text = "Units: " + td.Units;

                if (bufSize == 0)
                {
                    int elementSize = getBits(td.DataType) / 8;
                    bufSize = (uint)(td.Rows * td.Columns * elementSize + td.Offset);
                    dataBuffer = new byte[bufSize];
                    Array.Copy(PCM.buf, td.addrInt, dataBuffer, 0, bufSize);
                }

                int rowCount = td.Rows;
                int colCount = td.Columns;

                if (td.TableName.ToLower().EndsWith(".data"))
                {
                    rowCount = getRowCountFromTable();
                }

                string[] colHeaders = td.ColumnHeaders.Split(',');
                if (td.ColumnHeaders.StartsWith("Table: "))
                {
                    string[] parts = td.ColumnHeaders.Split(' ');
                    colHeaders = loadHeaderFromTable(parts[1], td.Columns).Split(',');
                }

                string[] rowHeaders = td.RowHeaders.Split(',');
                if (td.RowHeaders.StartsWith("Table: "))
                {
                    string[] parts = td.RowHeaders.Split(' ');
                    rowHeaders = loadHeaderFromTable(parts[1], td.Rows, true).Split(',');
                }


                if (swapXyToolStripMenuItem.Checked)
                {
                    //Swap col/row
                    rowCount = td.Columns;
                    colCount = td.Rows;
                    string[] tmp = rowHeaders;
                    rowHeaders = colHeaders;
                    colHeaders = tmp;
                    //colHeaders = td.RowHeaders.Split(',');
                    //rowHeaders = td.ColumnHeaders.Split(',');
                }

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                if (td.OutputType == OutDataType.Flag || (td.Units != null && td.Units.ToLower().Contains("boolean")))
                {
                    for (int c = 0; c < colCount; c++)
                    {
                        DataGridViewCheckBoxColumn col_chkbox = new DataGridViewCheckBoxColumn();
                        dataGridView1.Columns.Add(col_chkbox);
                    }
                }
                else if (rowCount < 2 && colCount < 2 && td.RowHeaders.Contains(",") && showRawHEXValuesToolStripMenuItem.Checked == false)
                {
                    //Special case, possible values in rowheader
                    combo = true;
                    chkSwapXY.Enabled = false;
                    swapXyToolStripMenuItem.Enabled = false;
                    //showRawHEXValuesToolStripMenuItem.Enabled = false;
                    txtMath.Enabled = false;
                    btnExecute.Enabled = false;
                    exportCSVToolStripMenuItem1.Enabled = false;

                    parseEnumHeaders(td);

                    DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();

                    comboCol.DataSource = new BindingSource(possibleVals, null);
                    comboCol.DisplayMember = "Value";
                    comboCol.ValueMember = "Key";
                    dataGridView1.Columns.Add(comboCol);
                    Array.Clear(rowHeaders, 0, rowHeaders.Length);

                }
                else
                {
                    dataGridView1.ColumnCount = colCount;
                }

                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader; // .AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);
                int step = getBits(td.DataType) / 8;

                uint addr = (uint)(td.addrInt + td.Offset);
                if (swapXyToolStripMenuItem.Checked ^ td.RowMajor == false)
                {
                    for (int r = 0; r < rowCount; r++)
                        dataGridView1.Rows.Add();
                    for (int c = 0; c < colCount; c++)
                    {
                        for (int r = 0; r < rowCount; r++)
                        {
                            setCellValue(addr, r, c, td);
                            addr += (uint)step;
                        }
                    }

                }
                else
                {
                    for (int r = 0; r < rowCount; r++)
                    {
                        var index = dataGridView1.Rows.Add();
                        for (int c = 0; c < colCount; c++)
                        {
                            setCellValue(addr, r, c, td);
                            addr += (uint)step;
                        }
                    }
                }
                for (int c = 0; c < colCount; c++)
                {
                    string headerTxt = "";
                    if (c > colHeaders.Length - 1 || colHeaders[0].Length == 0)
                        headerTxt = "";
                    else
                        headerTxt = colHeaders[c];
                    dataGridView1.Columns[c].HeaderText = headerTxt;
                }

                for (int r = 0; r < rowHeaders.Length; r++)
                {
                    if (r < rowCount)
                        dataGridView1.Rows[r].HeaderCell.Value = rowHeaders[r];
                }
                setDataGridLayout();
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

        private void setDataGridLayout()
        {
            try
            {
                string formatStr = "0";
                if (showRawHEXValuesToolStripMenuItem.Checked || td.OutputType == OutDataType.Hex)
                {
                    formatStr = "X" + (getBits(td.DataType)/4).ToString();
                }
                else if (td.OutputType == OutDataType.Text || td.OutputType == OutDataType.Flag)
                {
                    formatStr = "";
                }
                else
                {
                    for (int f = 1; f <= td.Decimals; f++)
                    {
                        if (f == 1) formatStr += ".";
                        formatStr += "0";
                    }
                    formatStr += "#";
                }
                foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
                {
                    dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (formatStr != "")
                        dgvc.DefaultCellStyle.Format = formatStr;
                }
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                if (autoResizeToolStripMenuItem.Checked) AutoResize();
                dataGridView1.RefreshEdit();
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
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1)
                {
                    Tagi t = (Tagi)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                    SaveValue(t.addr, e.RowIndex, e.ColumnIndex, tableDatas[t.id]);
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

        private void saveFlag(uint bufAddr, bool flag, TableData mathTd)
        {
            string maskStr = mathTd.BitMask.Replace("0x", "");
            if (mathTd.DataType == InDataType.UBYTE || mathTd.DataType == InDataType.SBYTE)
            {
                byte mask = Convert.ToByte(maskStr, 16);
                if (flag)
                {
                    dataBuffer[bufAddr] = (byte)(dataBuffer[bufAddr] | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    dataBuffer[bufAddr] = (byte)(dataBuffer[bufAddr] & mask);
                }
            }
            else if (mathTd.DataType == InDataType.SWORD || mathTd.DataType == InDataType.UWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = BEToUint16(dataBuffer, bufAddr);
                ushort newVal;
                if (flag)
                {
                    newVal = (ushort)(curVal | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    newVal = (ushort)(curVal & mask);
                }
                SaveUshort(dataBuffer,bufAddr, newVal);
            }
            else if (mathTd.DataType == InDataType.INT32 || mathTd.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = BEToUint32(dataBuffer,bufAddr);
                UInt32 newVal;
                if (flag)
                {
                    newVal = (UInt32)(curVal | mask);
                }
                else
                {
                    mask = ~mask;
                    newVal = (UInt32)(curVal & mask);
                }
                SaveUint32(dataBuffer, bufAddr, newVal);
            }
            else if (mathTd.DataType == InDataType.INT64 || mathTd.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                UInt64 curVal = BEToUint64(dataBuffer,bufAddr);
                UInt64 newVal;
                if (flag)
                {
                    newVal = (UInt64)(curVal | mask);
                }
                else
                {
                    mask = ~mask;
                    newVal = (UInt64)(curVal & mask);
                }
                SaveUint64(dataBuffer,bufAddr, newVal);
            }

        }
        private void SaveValue(uint addr,int r, int c, TableData mathTd)
        {
            MathParser parser = new MathParser();
            UInt32 bufAddr = addr - td.addrInt;

            if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != "")
            {
                bool flag = Convert.ToBoolean(dataGridView1.Rows[r].Cells[c].Value);
                saveFlag(bufAddr, flag, mathTd);
                return;
            }

            double value = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
            if (value == double.MaxValue) return;
            if (!showRawHEXValuesToolStripMenuItem.Checked)
            {
                if (!combo)
                {
                    if (value > mathTd.Max)
                    {
                        value = mathTd.Max;
                        dataGridView1.Rows[r].Cells[c].Value = value;
                    }
                    if (value < mathTd.Min)
                    {
                        value = mathTd.Min;
                        dataGridView1.Rows[r].Cells[c].Value = value;
                    }
                }
                string mathStr = mathTd.SavingMath.ToLower().Replace("x", value.ToString());
                if (commaDecimal) mathStr = mathStr.Replace(".", ",");
                value = parser.Parse(mathStr, true);
            }
            if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
                dataBuffer[bufAddr] = (byte)value;
            if (td.DataType == InDataType.SWORD)
                SaveShort(dataBuffer, bufAddr, (short)value);
            if (td.DataType == InDataType.UWORD)
                SaveUshort(dataBuffer, bufAddr, (ushort)value);
            if (td.DataType == InDataType.FLOAT32)
                SaveFloat32(dataBuffer, bufAddr, (Single)value);
            if (td.DataType == InDataType.INT32)
                SaveInt32(dataBuffer, bufAddr, (Int32)value);
            if (td.DataType == InDataType.UINT32)
                SaveUint32(dataBuffer, bufAddr, (UInt32)value);
            if (td.DataType == InDataType.FLOAT64)
                SaveFloat64(dataBuffer, bufAddr, value);
            if (td.DataType == InDataType.INT64)
                SaveInt64(dataBuffer, bufAddr, (Int64)value);
            if (td.DataType == InDataType.UINT64)
                SaveUint64(dataBuffer, bufAddr, (UInt64)value);

        }
        private void saveTable()
        {
            try
            {
                Array.Copy(dataBuffer, 0, PCM.buf, td.addrInt, bufSize);
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
                MathParser parser = new MathParser();

                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    string mathStr = txtMath.Text.ToLower().Replace("x", cell.Value.ToString());
                    if (commaDecimal) mathStr = mathStr.Replace(".", ",");
                    double newvalue = parser.Parse(mathStr);
                    cell.Value = newvalue;
                    uint addr = 0;
                    int id = 0;
                    Tagi t = (Tagi)dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Tag;
                    SaveValue(addr, cell.RowIndex, cell.ColumnIndex, tableDatas[t.id]);

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
            if (dgv_width < 175) dgv_width = 175;
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
                uint addr = td.addrInt;
                for (int a=0;a<bufSize; a++)
                {
                    if (PCM.buf[addr + a] != dataBuffer[a])
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
            loadTable(td, PCM);
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
            loadTable(td, PCM);

        }

        private void chkSwapXY_CheckedChanged(object sender, EventArgs e)
        {
            swapXyToolStripMenuItem.Checked = chkSwapXY.Checked;
            loadTable(td, PCM);
        }

        private void showRawHEXValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showRawHEXValuesToolStripMenuItem.Checked)
                showRawHEXValuesToolStripMenuItem.Checked = false;
            else
                showRawHEXValuesToolStripMenuItem.Checked = true;
            loadTable(td, PCM);
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
                        if (td.TableDescription != null)
                            dataGridView1.Rows[r].Cells[c].ToolTipText = td.TableDescription;
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

        private void showTableDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTableDescription ft = new frmTableDescription();
            ft.textBox1.Text = td.TableDescription;
            ft.Text = td.TableName;
            ft.ShowDialog(this);
        }

        private void showGraphicToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
    }
}