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

        private enum ColType
        {
            Flag,
            Combo,
            Value
        }

        private class ColumnInfo
        {
            public ColType  columnType { get; set; }
            public Dictionary<double, string> enumVals { get; set; }
            public Dictionary<int, string> enumIntVals { get; set; }
            public string enumStr { get; set; }
            public OutDataType outputType { get; set; }
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
        private TableData td;
        public PcmFile PCM;
        string tableName = "";
        private frmTableEditor compareEditor;
        private bool disableSaving = false;
        private bool commaDecimal = true;
        private byte[] tableBuffer;
        private uint bufSize = 0;
        MathParser parser = new MathParser();
        Font dataFont;
        
        public bool disableMultiTable = false;
        public List<int> tableIds = new List<int>();

        List<TableData> filteredTables;

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
        }

        public void addCompareFiletoMenu(PcmFile cmpPCM)
        {
            ToolStripMenuItem menuitem = new ToolStripMenuItem(cmpPCM.FileName);
            menuitem.Tag = cmpPCM;
            menuitem.Name = Path.GetFileName(cmpPCM.FileName);
            menuitem.Click += compareSelection_Click;
            if (compareToolStripMenuItem.DropDownItems.Count == 0)
            {
                //First file selected by default
                menuitem.Checked = true;
                loadCompareTable(cmpPCM);
                groupSelectCompare.Enabled = true;
            }
            compareToolStripMenuItem.DropDownItems.Add(menuitem);
        }
        public void loadCompareTable(PcmFile cmpPCM)
        {
            compareEditor = new frmTableEditor();
            compareEditor.PCM = cmpPCM;
            compareEditor.tableIds = tableIds;
            compareEditor.disableMultiTable = disableMultiTable;
            compareEditor.loadTable(cmpPCM.selectedTable);
        }

        private void compareSelection_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in compareToolStripMenuItem.DropDownItems)
                mi.Checked = false;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            menuitem.Checked = true;
            PcmFile cmpPCM = (PcmFile)menuitem.Tag;
            loadCompareTable(cmpPCM);
            loadTable(td);
        }

        public void loadSeekTable(int tId, PcmFile PCM1)
        {

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            NumberFormatInfo nfi = new CultureInfo(currentCulture, false).NumberFormat;
            if (nfi.NumberDecimalSeparator == ",") commaDecimal = true;
            else commaDecimal = false;

            PCM = PCM1;
            TableSeek tSeek = tableSeeks[PCM.foundTables[tId].configId];
            this.Text = "Table Editor: " + PCM.foundTables[tId].Name;

            FoundTable ft = PCM.foundTables[tId];

            td = new TableData();
            td.importFoundTable(tId, PCM);

            loadTable(td);
        }


        public double getValue(uint addr, TableData mathTd, bool getCompare = false)
        {
            double retVal = 0;

            Byte[] myBuffer = tableBuffer;
            UInt32 bufAddr = addr - td.addrInt;

            if ((int)(addr - td.addrInt) < 0 || (int)(addr - td.addrInt) > bufSize)
            {
                //Read data from other table
                myBuffer = PCM.buf;
                bufAddr = addr;
            }

            if (mathTd.DataType == InDataType.SBYTE)
                retVal = (sbyte)myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.UBYTE)
                retVal = myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.SWORD)
                retVal = BEToInt16(myBuffer,bufAddr);
            if (mathTd.DataType == InDataType.UWORD)
                retVal = BEToUint16(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT32)
                retVal = BEToInt32(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UINT32)
                retVal = BEToUint32(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT64)
                retVal = BEToInt64(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UINT64)
                retVal = BEToUint64(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.FLOAT32)
                retVal = BEToFloat32(myBuffer, bufAddr);
            if (mathTd.DataType == InDataType.FLOAT64)
                retVal = BEToFloat64(myBuffer, bufAddr);

            string mathStr = mathTd.Math.ToLower().Replace("x", retVal.ToString());
            if (commaDecimal) mathStr = mathStr.Replace(".", ",");
            retVal = parser.Parse(mathStr, false);
            if (compareEditor != null)
            {
                int currentOffset = (int)(addr - (td.addrInt + td.Offset) ); //Bytes from (table start + offset)
                int currentSteps = (int)currentOffset / getElementSize(td.DataType); 
                uint cmpAddr = (uint)(currentSteps * getElementSize(compareEditor.td.DataType) + compareEditor.td.addrInt + compareEditor.td.Offset);
                if (radioDifference.Checked)
                {
                    double cmpVal = compareEditor.getValue(cmpAddr, mathTd);
                    return retVal - cmpVal;
                }
                if (radioOriginal.Checked)
                {
                    return retVal;
                }
                if (radioCompareFile.Checked || getCompare)
                {
                    return compareEditor.getValue(cmpAddr, mathTd);
                }

            }
            return retVal;
        }

        public UInt64 getRawValue(UInt32 addr, TableData mathTd, bool getCompare = false)
        {
            UInt32 bufAddr = addr - td.addrInt;
            UInt64 retVal = 0;
            if (mathTd.DataType == InDataType.UWORD || mathTd.DataType == InDataType.SWORD)
                retVal = BEToUint16(tableBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT32 || mathTd.DataType == InDataType.UINT32 || mathTd.DataType == InDataType.FLOAT32)
                retVal = BEToUint32(tableBuffer, bufAddr);
            if (mathTd.DataType == InDataType.INT64 || mathTd.DataType == InDataType.UINT64 || mathTd.DataType == InDataType.FLOAT64)
                retVal = BEToUint64(tableBuffer, bufAddr);
            if (mathTd.DataType == InDataType.UBYTE || mathTd.DataType == InDataType.SBYTE)
                retVal =  tableBuffer[bufAddr];
            if (compareEditor != null)
            {
                int currentOffset = (int)(addr - (td.addrInt + td.Offset)); //Bytes from (table start + offset)
                int currentSteps = (int)currentOffset / getElementSize(td.DataType);
                uint cmpAddr = (uint)(currentSteps * getElementSize(compareEditor.td.DataType) + compareEditor.td.addrInt + compareEditor.td.Offset);
                if (radioDifference.Checked)
                {
                    UInt64 cmpVal = compareEditor.getRawValue(cmpAddr, mathTd);
                    return retVal - cmpVal;
                }
                if (radioOriginal.Checked)
                {
                    return retVal;
                }
                if (radioCompareFile.Checked || getCompare)
                {
                    return compareEditor.getRawValue(cmpAddr, mathTd);
                }

            }
            return retVal;
        }

        public void setCellValue(uint addr, int row, int col, TableData mathTd)
        {
            double val = 0;
            if (radioSideBySide.Checked)
            {
                double orgVal;
                double cmpVal;
                string orgTxt = "";
                string cmpTxt = "";
                string formatStr = "0";
                if (showRawHEXValuesToolStripMenuItem.Checked)
                {
                    orgVal = getRawValue(addr, mathTd);
                    cmpVal = getRawValue(addr, mathTd, true);
                    formatStr = "X" + (getElementSize(mathTd.DataType) * 2).ToString();
                    orgTxt = ((uint)orgVal).ToString(formatStr);
                    cmpTxt = ((uint)cmpVal).ToString(formatStr);
                }
                else
                {
                    orgVal = getValue(addr, mathTd);
                    cmpVal = getValue(addr, mathTd, true);
                    if (td.OutputType == OutDataType.Text)
                    {
                        orgTxt = Convert.ToChar((ushort)orgVal).ToString();
                        cmpTxt = Convert.ToChar((ushort)cmpVal).ToString();
                    }
                    else if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0)
                    {
                        UInt64 mask = Convert.ToUInt64(mathTd.BitMask.Replace("0x", ""), 16);
                        if ((getRawValue(addr, mathTd) & mask) == mask)
                            orgTxt = "1";
                        else
                            orgTxt = "0";
                        if ((getRawValue(addr, mathTd, true) & mask) == mask)
                            cmpTxt = "1";
                        else
                            cmpTxt = "0";
                    }
                    else if (mathTd.OutputType == OutDataType.Hex)
                    {
                        val = getValue(addr, mathTd);
                        dataGridView1.Rows[row].Cells[col].Value = (uint)val;
                    }
                    else if (mathTd.OutputType == OutDataType.Int)
                    {
                        orgTxt = ((int)orgVal).ToString();
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
                        orgTxt = orgVal.ToString(formatStr);
                        cmpTxt = cmpVal.ToString(formatStr);
                    }
                }
                dataGridView1.Rows[row].Cells[col].Value = orgTxt + " [" + cmpTxt + "]";
                Tagi ta = new Tagi();
                ta.addr = addr;
                ta.id = (int)mathTd.id;
                dataGridView1.Rows[row].Cells[col].Tag = ta;
                if (orgVal == cmpVal)
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                else
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                return;
            }
            if (showRawHEXValuesToolStripMenuItem.Checked)
                dataGridView1.Rows[row].Cells[col].Value = getRawValue(addr, mathTd);
            else if (td.OutputType == OutDataType.Text)
                dataGridView1.Rows[row].Cells[col].Value = Convert.ToChar((ushort)getValue(addr, mathTd));
            else if (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0)
            {
                UInt64 mask = Convert.ToUInt64(mathTd.BitMask.Replace("0x", ""), 16);
                if ((getRawValue(addr, mathTd) & mask) == mask)
                {
                    Debug.WriteLine(mathTd.TableName + ": " + mathTd.BitMask + ", mask: " + mask.ToString("X") + ", Data: " + getRawValue(addr, mathTd).ToString("X") + " Row: " + row + ", Col: " + col + ", true");
                    dataGridView1.Rows[row].Cells[col].Value = 1;
                }
                else
                {
                    Debug.WriteLine(mathTd.TableName + ": " + mathTd.BitMask + " mask: " + mask.ToString("X") + ", Data: " + getRawValue(addr, mathTd).ToString("X") + " Row: " + row + ", Col: " + col + ", false");
                    dataGridView1.Rows[row].Cells[col].Value = 0;
                }
            }
            else if (mathTd.OutputType == OutDataType.Hex)
            {
                val = getValue(addr, mathTd);
                dataGridView1.Rows[row].Cells[col].Value = (uint)val;
            }
            else if (mathTd.OutputType == OutDataType.Int)
            {
                val = getValue(addr, mathTd);
                dataGridView1.Rows[row].Cells[col].Value = (int)val;
            }
            else
            {
                val = getValue(addr, mathTd);
                dataGridView1.Rows[row].Cells[col].Value = val;
            }

            if (dataGridView1.Columns[col].GetType() != typeof(DataGridViewComboBoxColumn) && dataGridView1.Columns[col].GetType() != typeof(DataGridViewCheckBoxColumn))
            {
                if (val < (mathTd.Max * 0.9) && val > (mathTd.Min * 1.1))
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.White;
                else if (val > mathTd.Max)
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.Pink;
                else if (val > (0.9 * mathTd.Max))
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                else if (val < mathTd.Min)
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.AliceBlue;
                else if (val < (1.1 * mathTd.Min))
                    dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
            }
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

        private string loadHeaderFromTable(string tableName, int count)
        {
            string headers = "" ;
            for (int i=0; i < PCM.tableDatas.Count; i++)
            {
                TableData t = PCM.tableDatas[i];
                if (t.TableName == tableName)
                {
                    uint step = (uint)(getBits(t.DataType) / 8);
                    uint addr = (uint)(t.addrInt + t.Offset);
                    for (int a = 0; a < count; a++ )
                    {
                        string formatStr = "0.####";
                        if (t.Units.Contains("%"))
                            formatStr = "0";
                        headers += t.Units.Trim() + " " + getValue(addr, t).ToString(formatStr).Replace(",", ".") + ",";
                        addr += step;
                    }
                    headers = headers.Trim(',');
                    break;
                }
            }
            return headers;
        }

        public Dictionary<double, string> parseEnumHeaders(string  eVals)
        {
            Dictionary<double, string> retVal = new Dictionary<double, string>();
            string[] posVals = eVals.Split(',');
            for (int r = 0; r < posVals.Length; r++)
            {
                string[] parts = posVals[r].Split(':');
                double val = 0;
                double.TryParse(parts[0], out val);
                string txt = posVals[r];
                if (!retVal.ContainsKey(val))
                    retVal.Add(val, txt);
            }
            retVal.Add(double.MaxValue, "------------");
            return retVal;
        }
        public Dictionary<int, string> parseIntEnumHeaders(string eVals)
        {
            Dictionary<int, string> retVal = new Dictionary<int, string>();
            string[] posVals = eVals.Split(',');
            for (int r = 0; r < posVals.Length; r++)
            {
                string[] parts = posVals[r].Split(':');
                int val = 0;
                int.TryParse(parts[0], out val);
                string txt = posVals[r];
                if (!retVal.ContainsKey(val))
                    retVal.Add(val, txt);
            }
            retVal.Add(int.MaxValue, "------------");
            return retVal;
        }

        private int getColumnByTableData(TableData cTd,int col)
        {
            int ind = int.MinValue;
            string colName = "";

            if (cTd.Columns == dataGridView1.Columns.Count)
                return col;

            if (cTd.Columns == 1)
            {
                if (tableIds.Count > 1)
                {
                    colName = cTd.TableName;
                    if (cTd.ColumnHeaders != null && cTd.ColumnHeaders != "" && !cTd.ColumnHeaders.Contains(","))
                        colName += " " + cTd.ColumnHeaders.Trim();
                }
                else
                {
                    MultiTableName mtn = new MultiTableName(cTd.TableName, (int)numColumn.Value);
                    colName = mtn.columnName;
                    if (cTd.ColumnHeaders != null && cTd.ColumnHeaders != "" && !cTd.ColumnHeaders.Contains(","))
                        colName += " " + cTd.ColumnHeaders.Trim();
                }
            }
            if (cTd.ColumnHeaders.Contains(','))
            {
                string[] tParts = cTd.ColumnHeaders.Split(',');
                if (tParts.Length >= (col -1))
                    colName = tParts[col].Trim();
                if (cTd.ColumnHeaders != null && cTd.ColumnHeaders != "" && !cTd.ColumnHeaders.Contains(","))
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
        private int getColumnByTableData_XySwap(TableData cTd, int col)
        {
            int ind = int.MinValue;
            string colName = "";

            if (cTd.Rows == dataGridView1.Columns.Count)
                return col;

            if (cTd.Rows == 1)
            {
                MultiTableName mtn = new MultiTableName(cTd.TableName, (int)numColumn.Value);
                colName = mtn.RowName;
            }
            else if (cTd.RowHeaders.Contains(','))
            {
                string[] tParts = cTd.RowHeaders.Split(',');
                if (tParts.Length >= (col - 1))
                    colName = tParts[col].Trim();
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
                MultiTableName mtn = new MultiTableName(cTd.TableName, (int)numColumn.Value);
                rowName = mtn.RowName;
                Debug.WriteLine("getRowByTableData: By tablename: " + rowName);
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

        private int getRowByTableData_XySwap(TableData cTd, int row)
        {
            int ind = int.MinValue;
            string rowName = "";

            if (cTd.Columns == dataGridView1.Rows.Count)
            {
                Debug.WriteLine("getRowByTableData_XySwap: cTd.Columns == dataGridView1.Rows.Count");
                return row;
            }

            if (cTd.Columns == 1)
            {
                if (tableIds.Count > 1)
                {
                    rowName = cTd.TableName;
                    if (cTd.ColumnHeaders != null && cTd.ColumnHeaders != "" && !cTd.ColumnHeaders.Contains(","))
                        rowName += " " + cTd.ColumnHeaders.Trim();
                    Debug.WriteLine("getRowByTableData_XySwap: By tablename: " + rowName);
                }
                else
                {
                    MultiTableName mtn = new MultiTableName(cTd.TableName, (int)numColumn.Value);
                    rowName = mtn.columnName;
                    if (cTd.ColumnHeaders != null && cTd.ColumnHeaders != "" && !cTd.ColumnHeaders.Contains(","))
                        rowName += " " + cTd.ColumnHeaders.Trim();
                    Debug.WriteLine("getRowByTableData_XySwap: By tablename: " + rowName);
                }
            }
            if (cTd.ColumnHeaders.Contains(','))
            {
                string[] tParts = cTd.ColumnHeaders.Split(',');
                if (tParts.Length >= (row - 1))
                {
                    rowName = tParts[row].Trim();
                    Debug.WriteLine("getRowByTableData_XySwap: By current TD " + rowName);
                }
            }
            else if (td.ColumnHeaders.Contains(','))
            {
                string[] tParts = td.ColumnHeaders.Split(',');
                if (tParts.Length >= (row - 1))
                {
                    rowName = tParts[row].Trim();
                    Debug.WriteLine("getRowByTableData_XySwap: By main TD " + rowName);
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

        private ColumnInfo getColinfoByTableData (TableData td1)
        {
            ColumnInfo colInfo = new ColumnInfo();
            if (showRawHEXValuesToolStripMenuItem.Checked || radioSideBySide.Checked)
            {
                colInfo.columnType = ColType.Value;
            }
            else if (td1.Values != null && td1.Values.StartsWith("Enum: "))
            {
                colInfo.columnType = ColType.Combo;
                string enumStr = td1.Values.Replace("Enum: ", "");
                colInfo.enumVals = parseEnumHeaders(enumStr);
                colInfo.enumIntVals = parseIntEnumHeaders(enumStr);
                //colInfo.enumVals = parseEnumHeadersToList(enumStr);
                colInfo.enumStr = enumStr;
            }
            else if (td1.OutputType == OutDataType.Flag)
            {
                colInfo.columnType = ColType.Flag;
            }
            else
            {
                colInfo.columnType = ColType.Value;
            }
            colInfo.outputType = td1.OutputType;
            return colInfo;
        }

        public void loadMultiTable(string tableName)
        {
            List<string> colHeaders = new List<string>();
            List<string> rowHeaders = new List<string>();
            List<ColumnInfo> coliInfos = new List<ColumnInfo>();

            this.tableName = tableName;

            if (td.Units.ToLower().Contains("bitmask"))
                labelUnits.Text = "Units: Boolean";
            else
                labelUnits.Text = "Units: " + getUnitFromTableData(td);
            if (td.Values != null && !td.Values.StartsWith("Enum:"))
                labelUnits.Text += ", Values: " +  td.Values;

            if (tableIds.Count > 1)
            {
                //Manually selected multiple tables
                filteredTables = new List<TableData>();
                tableIds.Sort();
                for (int i = 0; i < tableIds.Count; i++)
                {
                    ColumnInfo colInfo = getColinfoByTableData(PCM.tableDatas[tableIds[i]]);
                    coliInfos.Add(colInfo);
                    string colHdr = PCM.tableDatas[tableIds[i]].TableName;
                    if (PCM.tableDatas[tableIds[i]].ColumnHeaders != "" && !PCM.tableDatas[tableIds[i]].ColumnHeaders.Contains(","))
                        colHdr += " " + PCM.tableDatas[tableIds[i]].ColumnHeaders.Trim();
                    colHeaders.Add(colHdr);
                    filteredTables.Add(PCM.tableDatas[tableIds[i]]);
                }
                if (td.RowHeaders == null || td.RowHeaders.Length == 0)
                    for (int r = 0; r < td.Rows; r++)
                        rowHeaders.Add("");
            }
            else
            {
                //Multible tables which are meant to be linked together
                string filterName = td.TableName.Substring(0, tableName.Length + 1);
                var results = PCM.tableDatas.Where(t => t.TableName.StartsWith(filterName));
                filteredTables = new List<TableData>(results.ToList());
                filteredTables = filteredTables.OrderBy(o => o.addrInt).ToList();
            }
            td = filteredTables[0];

            if (bufSize == 0)
            {
                List<TableData> sizeList = new List<TableData>(filteredTables.OrderBy(o => o.addrInt).ToList());
                int elementSize = getElementSize(sizeList[sizeList.Count - 1].DataType); //Last table in list
                int singleTableSize = td.Rows * td.Columns * elementSize;
                bufSize = (uint)(sizeList[sizeList.Count - 1].addrInt - sizeList[0].addrInt + sizeList[sizeList.Count - 1].Offset + singleTableSize);
                tableBuffer = new byte[bufSize];
                Array.Copy(PCM.buf, td.addrInt, tableBuffer, 0, bufSize);
            }


            if (tableIds.Count < 2)
            {
                for (int t = 0; t < filteredTables.Count; t++)
                {
                    //Collect all different row & column labels from table names

                    ColumnInfo colInfo = new ColumnInfo();
                    MultiTableName mtn = new MultiTableName(filteredTables[t].TableName, (int)numColumn.Value);
                    if (!rowHeaders.Contains(mtn.RowName))
                        rowHeaders.Add(mtn.RowName);
                    string colHdr = mtn.columnName;
                    if (filteredTables[t].ColumnHeaders != null &&  filteredTables[t].ColumnHeaders != "" && !filteredTables[t].ColumnHeaders.Contains(","))
                        colHdr += " " + filteredTables[t].ColumnHeaders.Trim();
                    if (!colHeaders.Contains(colHdr))
                    {
                        colHeaders.Add(colHdr);
                        colInfo = getColinfoByTableData(filteredTables[t]);
                        coliInfos.Add(colInfo);
                    }
                }
/*                if (filteredTables[0].TableName.StartsWith("DTC") )
                {
                    colHeaders.Add("Description");
                    ColumnInfo colInfo = new ColumnInfo();
                    colInfo.columnType = ColType.Value;
                    colInfo.outputType = OutDataType.Text;
                    coliInfos.Add(colInfo);
                }
*/
            }
            if (rowHeaders.Count < 2 && td.Rows > 1)
            {
                if (filteredTables[0].RowHeaders!= null && filteredTables[0].RowHeaders.Contains(','))
                {
                    rowHeaders.Clear();
                    string[] rParts = filteredTables[0].RowHeaders.Split(',');
                    for (int r = 0; r < rParts.Length; r++)
                        rowHeaders.Add(rParts[r].Trim());
                }
            }
            if (rowHeaders[0].Contains("]["))
            { //Tablename Have [][][]
                numColumn.Enabled = true;
                numColumn.Visible = true;
                labelColumn.Visible = true;
            }

            if (td.ColumnHeaders.StartsWith("Table: "))
            {
                colHeaders.Clear();
                string[] parts = td.ColumnHeaders.Split(' ');
                string[] colHeaderStr = loadHeaderFromTable(parts[1], td.Columns).Split(',');
                ColumnInfo colInfo = new ColumnInfo();
                colInfo = getColinfoByTableData(td);
                for (int p = 0; p < colHeaderStr.Length; p++)
                {
                    coliInfos.Add(colInfo);
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

            bool xySwapped = chkSwapXY.Checked;
 
            //If there is different kind of columns, X/Y can't be swapped
            int boolCols = 0;
            int combCols = 0;
            int regCols = 0;
            for (int c = 0; c < coliInfos.Count; c++)
            {
                if (coliInfos[c].columnType == ColType.Combo)
                {
                    if (c > 0 && coliInfos[c].enumStr != coliInfos[c-1].enumStr)
                    {   //Diffrent values in comboboxes, not possible to put in one column                       
                        combCols = 10;
                        break;
                    }
                    combCols = 1;
                }
                else if (coliInfos[c].columnType == ColType.Flag) boolCols = 1;
                else regCols = 1;
            }
            if ((boolCols + combCols + regCols) == 1)
            {
                //Only regular, combo, or Flag columns used, can swap
                if (rowHeaders.Count == 1 && colHeaders.Count > 1)                    
                    xySwapped = !chkSwapXY.Checked; //Invert checkbox meaning, swap by default
            }
            else
            {
                chkSwapXY.Enabled = false;
                swapXyToolStripMenuItem.Enabled = false;
            }

            if (xySwapped)
            {
                //Swapped, put ROWheaders to COLUMNS
                for (int r = 0; r < rowHeaders.Count; r++)
                {
                    string hdrTxt = rowHeaders[r];
                    if (rowHeaders.Count == 1)
                    {
                        if (td.Units.ToLower().Contains("bitmask"))
                            hdrTxt = "Boolean";
                        else
                            hdrTxt = td.Units;
                    }
                    //Setup Checkbox & Combo -columns
                    if (coliInfos[0].columnType == ColType.Flag)
                    {
                        DataGridViewCheckBoxColumn col_chkbox = new DataGridViewCheckBoxColumn();
                        dataGridView1.Columns.Add(col_chkbox);
                    }
                    else if (coliInfos[0].columnType == ColType.Combo)
                    {
                        DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
                        if (coliInfos[0].outputType == OutDataType.Float)
                            comboCol.DataSource = new BindingSource(coliInfos[0].enumVals, null);
                        else
                            comboCol.DataSource = new BindingSource(coliInfos[0].enumIntVals, null);
                        comboCol.DisplayMember = "Value";
                        comboCol.ValueMember = "Key";
                        dataGridView1.Columns.Add(comboCol);
                    }
                    else
                    {
                        dataGridView1.Columns.Add(hdrTxt, hdrTxt);
                    }
                    dataGridView1.Columns[r].HeaderText = hdrTxt;
                }

                //Put ColHeaders to ROWLables
                for (int c = 0; c < colHeaders.Count; c++)
                {
                    dataGridView1.Rows.Add();
                    if (colHeaders.Count == 1)
                    {
                        if (td.Units.ToLower().Contains("bitmask"))
                            dataGridView1.Rows[c].HeaderCell.Value = "Boolean";
                        else
                            dataGridView1.Rows[c].HeaderCell.Value = td.Units;
                    }
                    else
                    {
                        dataGridView1.Rows[c].HeaderCell.Value = colHeaders[c];
                    }
                }


                int gridRow = 0;
                for (int tbl = 0; tbl < filteredTables.Count; tbl++) ///Go thru all filtered tables
                {
                    int gridCol = 0;
                    TableData ft = filteredTables[tbl];
                    int elementsize = getElementSize(ft.DataType);
                    uint addr = (uint)(ft.addrInt + ft.Offset);
                    if (!ft.RowMajor) 
                    {
                        for (int r = 0; r < ft.Rows; r++) //All rows from table
                        {
                            if (dataGridView1.Columns.Count > 1)
                                gridCol = getColumnByTableData_XySwap(ft, r);
                            for (int c = 0; c < ft.Columns; c++) //All columns from table
                            {
                                if (dataGridView1.Rows.Count > 1)
                                    gridRow = getRowByTableData_XySwap(ft, c);
                                setCellValue(addr, gridRow, gridCol, ft);
                                addr += (uint)elementsize;
                            }
                        }
                    }
                    else //Rowmajor
                    {
                        for (int c = 0; c < ft.Columns; c++)
                        {
                            if (dataGridView1.Rows.Count > 1)
                                gridRow = getRowByTableData_XySwap(ft, c);
                            for (int r = 0; r < ft.Rows; r++)
                            {
                                if (dataGridView1.Columns.Count > 1)
                                    gridCol = getColumnByTableData_XySwap(ft, r);
                                setCellValue(addr, gridRow, gridCol, ft);
                                addr += (uint)elementsize;
                            }
                        }

                    }

                }


            }
            else //Not xyswapped
            {
                for (int c = 0; c < colHeaders.Count; c++)
                {
                    string hdrTxt = colHeaders[c];
                    if (colHeaders.Count == 1)
                    {
                        if (td.Units.ToLower().Contains("bitmask"))
                            dataGridView1.Columns[c].HeaderText = "Boolean";
                        else
                            dataGridView1.Columns[c].HeaderText = td.Units;
                    }
                    //Setup comboboxes & checkboxes
                    if (coliInfos[c].columnType == ColType.Flag)
                    {
                        DataGridViewCheckBoxColumn col_chkbox = new DataGridViewCheckBoxColumn();
                        dataGridView1.Columns.Add(col_chkbox);
                    }
                    else if (coliInfos[c].columnType == ColType.Combo)
                    {
                        DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
                        if (coliInfos[c].outputType == OutDataType.Float)
                            comboCol.DataSource = new BindingSource(coliInfos[c].enumVals, null);
                        else
                            comboCol.DataSource = new BindingSource(coliInfos[c].enumIntVals, null);
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
                    {
                        if (td.Units.ToLower().Contains("bitmask"))
                            dataGridView1.Rows[r].HeaderCell.Value = "Boolean";
                        else
                            dataGridView1.Rows[r].HeaderCell.Value = td.Units;
                    }
                    else
                        dataGridView1.Rows[r].HeaderCell.Value = rowHeaders[r];
                }

                int gridRow = 0;
                for (int tbl = 0; tbl < filteredTables.Count; tbl++)
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
                if (td.TableName.StartsWith("DTC"))
                {
                    showDtdDescriptions();
                }

            }
            for (int r=0; r< dataGridView1.Rows.Count; r++)
            {
                for (int c=0; c<dataGridView1.Columns.Count; c++)
                {
                    if (dataGridView1.Rows[r].Cells[c].Tag == null)
                    {
                        dataGridView1.Rows[r].Cells[c].ReadOnly = true;
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.DarkGray;
                    }
                }
            }
            setDataGridLayout();
        }

        private void showDtdDescriptions()
        {
            DtcSearch ds = new DtcSearch();
            if (OBD2Codes == null || OBD2Codes.Count == 0)
                ds.loadOBD2Codes();
            if (OBD2Codes.Count == 0)
                return;
            chkSwapXY.Enabled = false;
            swapXyToolStripMenuItem.Enabled = false;
            int col = dataGridView1.Columns.Add("Description", "Description");
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                string descr = ds.getDtcDescription(dataGridView1.Rows[r].HeaderCell.Value.ToString());
                dataGridView1.Rows[r].Cells["Description"].Value = descr;
                Tagi tg = new Tagi();
                tg.addr = uint.MaxValue - 1;
                tg.id = r;
                dataGridView1.Rows[r].Cells["Description"].Tag = tg;
            }
        }
        public int getColumnsFromTable()
        {
            int cols = td.Columns;

            string yTbName = td.TableName.Replace(".Data", ".xVal");
            for (int y = 0; y < PCM.tableDatas.Count; y++)
            {
                if (PCM.tableDatas[y].TableName == yTbName)
                {
                    TableData ytb = PCM.tableDatas[y];
                    uint xaddr = (uint)(ytb.addrInt + ytb.Offset);
                    cols = (int)getValue(xaddr, ytb);
                    break;
                }
            }

            return cols;

        }
        public int getRowCountFromTable()
        {
            int rows = td.Rows;

            for (int x=0; x< PCM.tableDatas.Count; x++)
            {
                if (PCM.tableDatas[x].TableName == td.TableName.Replace(".Data", ".Size") || PCM.tableDatas[x].TableName == td.TableName.Replace(".Data", ".yVal"))
                {
                    uint addr = (uint)(PCM.tableDatas[x].addrInt + PCM.tableDatas[x].Offset);
                    rows = (int)getValue(addr, PCM.tableDatas[x]);
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
        public void loadTable(TableData td1)
        {
            try
            {
                var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                NumberFormatInfo nfi = new CultureInfo(currentCulture, false).NumberFormat;
                if (nfi.NumberDecimalSeparator == ",") commaDecimal = true;
                else commaDecimal = false;

                td = td1;
                if (td.OutputType == OutDataType.Flag || td.Units.ToLower().StartsWith("boolean") || td.Values.StartsWith("Enum: "))
                    radioDifference.Enabled = false;

                if (tableIds.Count > 1)
                {
                    loadMultiTable(td.TableName);
                    return;
                }
                if (!disableMultiTable && (td.TableName.Contains("[") || td.TableName.Contains(".")))
                {
                    if (td.TableName.StartsWith("Header.") || td.TableName.EndsWith(".Data") || td.TableName.EndsWith(".xVal") || td.TableName.EndsWith(".yVal") || td.TableName.EndsWith(".Size"))
                    {
                        //Special case, "Normal" table, but header values from tables, WITH different table as multiplier
                        Debug.WriteLine("Special case, not real multitable");
                    }
                    else
                    {
                        MultiTableName mtn = new MultiTableName(td.TableName, (int)numColumn.Value);
                        for (int t = 0; t < PCM.tableDatas.Count; t++)
                        {
                            if (PCM.tableDatas[t].Category == td.Category && PCM.tableDatas[t].TableName.StartsWith(mtn.TableName) && PCM.tableDatas[t].TableName != td.TableName)
                            {
                                //It is multitable
                                loadMultiTable(mtn.TableName);
                                return;
                            }
                        }
                    }
                }

                tableName = td.TableName;
                setMyText();
                
                labelUnits.Text = "Units: " + getUnitFromTableData(td);
                if (td.Values != null && !td.Values.StartsWith("Enum:"))
                    labelUnits.Text += ", Values: " +  td.Values;

                if (bufSize == 0)
                {
                    int elementSize = getBits(td.DataType) / 8;
                    bufSize = (uint)(td.Rows * td.Columns * elementSize + td.Offset);
                    tableBuffer = new byte[bufSize];
                    Array.Copy(PCM.buf, td.addrInt, tableBuffer, 0, bufSize);
                }

                int rowCount = td.Rows;
                int colCount = td.Columns;

                if (td.TableName.ToLower().EndsWith(".data"))
                {
                    rowCount = getRowCountFromTable();
                    colCount = getColumnsFromTable();
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
                    rowHeaders = loadHeaderFromTable(parts[1], td.Rows).Split(',');
                }


                if (swapXyToolStripMenuItem.Checked)
                {
                    //Swap col/row
                    rowCount = td.Columns;
                    colCount = td.Rows;
                    string[] tmp = rowHeaders;
                    rowHeaders = colHeaders;
                    colHeaders = tmp;
                }

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                if (radioSideBySide.Checked)
                {
                    dataGridView1.ColumnCount = colCount;
                }
                else if (td.OutputType == OutDataType.Flag || (td.Units != null && td.Units.ToLower().Contains("boolean")))
                {
                    for (int c = 0; c < colCount; c++)
                    {
                        DataGridViewCheckBoxColumn col_chkbox = new DataGridViewCheckBoxColumn();
                        dataGridView1.Columns.Add(col_chkbox);
                    }
                }
                else if (td.Values.Contains(",") && showRawHEXValuesToolStripMenuItem.Checked == false)
                {
                    //Special case, possible values in rowheader
                    txtMath.Enabled = false;
                    btnExecute.Enabled = false;
                    exportCSVToolStripMenuItem1.Enabled = false;

                    Dictionary<double, string> possibleVals = parseEnumHeaders(td.Values.Replace("Enum: ",""));
                    Dictionary<int, string> possibleIntVals = parseIntEnumHeaders(td.Values.Replace("Enum: ", ""));
                    //List <comboValues> possibleVals = parseEnumHeadersToList(td.Values.Replace("Enum: ", ""));
                    for (int c = 0; c < colCount; c++)
                    {
                        DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
                        if (td.OutputType == OutDataType.Float)
                            comboCol.DataSource = new BindingSource(possibleVals,null);
                        else
                            comboCol.DataSource = new BindingSource(possibleIntVals, null);
                        //comboCol.DataSource = possibleVals;
                        //comboCol.DataPropertyName = "value";
                        comboCol.DisplayMember = "Value";
                        comboCol.ValueMember = "Key";
                        dataGridView1.Columns.Add(comboCol);
                    }
                    //Array.Clear(rowHeaders, 0, rowHeaders.Length);

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
                if (td.TableName == "DTC" && this.Visible)
                {
                    showDtdDescriptions();
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
                if (numDecimals.Value < 0 && td != null)
                    numDecimals.Value = td.Decimals;
                string formatStr = "0";
                if (showRawHEXValuesToolStripMenuItem.Checked || td.OutputType == OutDataType.Hex)
                {
                    formatStr = "X" + ((int)numDecimals.Value).ToString() ;
                }
                else if (td.OutputType == OutDataType.Text || td.OutputType == OutDataType.Flag || radioSideBySide.Checked)
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
                if (e.RowIndex > -1)
                {
                    Tagi t = (Tagi)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                    SaveValue(t.addr, e.RowIndex, e.ColumnIndex, PCM.tableDatas[t.id]);
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
            string maskStr = "FF";
            if (mathTd.BitMask != null)
                maskStr = mathTd.BitMask.Replace("0x", "");
            if (mathTd.DataType == InDataType.UBYTE || mathTd.DataType == InDataType.SBYTE)
            {
                byte mask = Convert.ToByte(maskStr, 16);
                if (flag)
                {
                    tableBuffer[bufAddr] = (byte)(tableBuffer[bufAddr] | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    tableBuffer[bufAddr] = (byte)(tableBuffer[bufAddr] & mask);
                }
            }
            else if (mathTd.DataType == InDataType.SWORD || mathTd.DataType == InDataType.UWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = BEToUint16(tableBuffer, bufAddr);
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
                SaveUshort(tableBuffer,bufAddr, newVal);
            }
            else if (mathTd.DataType == InDataType.INT32 || mathTd.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = BEToUint32(tableBuffer,bufAddr);
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
                SaveUint32(tableBuffer, bufAddr, newVal);
            }
            else if (mathTd.DataType == InDataType.INT64 || mathTd.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                UInt64 curVal = BEToUint64(tableBuffer,bufAddr);
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
                SaveUint64(tableBuffer,bufAddr, newVal);
            }

        }
        private void SaveValue(uint addr,int r, int c, TableData mathTd)
        {
            MathParser parser = new MathParser();
            UInt32 bufAddr = addr - td.addrInt;

            if (td.TableName.StartsWith("DTC") && addr == (uint.MaxValue - 1))
            {
                //OBD2 Description
                OBD2Code oc = new OBD2Code();
                oc.Code = dataGridView1.Rows[r].HeaderCell.Value.ToString();
                oc.Description = dataGridView1.Rows[r].Cells[c].Value.ToString();
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
                return;
            }
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
                if (dataGridView1.Columns[c].GetType() != typeof(DataGridViewComboBoxColumn))
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
                tableBuffer[bufAddr] = (byte)value;
            if (td.DataType == InDataType.SWORD)
                SaveShort(tableBuffer, bufAddr, (short)value);
            if (td.DataType == InDataType.UWORD)
                SaveUshort(tableBuffer, bufAddr, (ushort)value);
            if (td.DataType == InDataType.FLOAT32)
                SaveFloat32(tableBuffer, bufAddr, (Single)value);
            if (td.DataType == InDataType.INT32)
                SaveInt32(tableBuffer, bufAddr, (Int32)value);
            if (td.DataType == InDataType.UINT32)
                SaveUint32(tableBuffer, bufAddr, (UInt32)value);
            if (td.DataType == InDataType.FLOAT64)
                SaveFloat64(tableBuffer, bufAddr, value);
            if (td.DataType == InDataType.INT64)
                SaveInt64(tableBuffer, bufAddr, (Int64)value);
            if (td.DataType == InDataType.UINT64)
                SaveUint64(tableBuffer, bufAddr, (UInt64)value);

        }
        private void saveTable()
        {
            try
            {
                Array.Copy(tableBuffer, 0, PCM.buf, td.addrInt, bufSize);
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
                    SaveValue(addr, cell.RowIndex, cell.ColumnIndex, PCM.tableDatas[t.id]);

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
                    if (PCM.buf[addr + a] != tableBuffer[a])
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
            loadTable(td);
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
            loadTable(td);

        }

        private void chkSwapXY_CheckedChanged(object sender, EventArgs e)
        {
            swapXyToolStripMenuItem.Checked = chkSwapXY.Checked;
            loadTable(td);
        }

        private void showRawHEXValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showRawHEXValuesToolStripMenuItem.Checked)
                showRawHEXValuesToolStripMenuItem.Checked = false;
            else
                showRawHEXValuesToolStripMenuItem.Checked = true;
            loadTable(td);
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
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!e.Exception.Message.Contains("DataGridViewComboBoxCell"))
                Debug.WriteLine(e.Exception);
        }

        private void numColumn_ValueChanged(object sender, EventArgs e)
        {
            MultiTableName mtn = new MultiTableName(td.TableName, (int)numColumn.Value);
            loadMultiTable(mtn.TableName);
        }

        private void radioCompareFile_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCompareFile.Checked)
            {
                dataGridView1.BackgroundColor = Color.Red;
                disableSaving = true;
                setMyText();
            }
            loadTable(td);
        }

        private void radioDifference_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDifference.Checked)
            {
                dataGridView1.BackgroundColor = Color.Red;
                disableSaving = true;
                setMyText();
            }
            loadTable(td);
        }
        private void radioSideBySide_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSideBySide.Checked)
            {
                dataGridView1.BackgroundColor = Color.Red;
                disableSaving = true;
                setMyText();
            }
            graphToolStripMenuItem.Enabled = !radioSideBySide.Checked;            

            loadTable(td);
        }

        private void radioOriginal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioOriginal.Checked)
            {
                dataGridView1.BackgroundColor = Color.Gray;
                disableSaving = false;
                setMyText();
            }
            loadTable(td);
        }

        private void setMyText()
        {
            this.Text = "Tuner: " + tableName + " [";
            if (radioOriginal.Checked)
                this.Text += PCM.FileName + "]";
            if (radioDifference.Checked || radioSideBySide.Checked)
                this.Text += PCM.FileName + " <> " + compareEditor.PCM.FileName + "]";
            if (radioCompareFile.Checked)
                this.Text += compareEditor.PCM.FileName + "]";
        }

        private void numDecimals_ValueChanged(object sender, EventArgs e)
        {
            loadTable(td);
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
            loadTable(td);
        }

        private void saveOBD2DescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveOBD2Codes();
        }
    }
}