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

namespace UniversalPatcher
{
    public partial class frmTableEditor : Form
    {
        public frmTableEditor()
        {
            InitializeComponent();
        }

        private TableData td;
        private PcmFile PCM;
        private bool tableModified = false;
        private bool commaDecimal = true;
        MathParser parser = new MathParser();

        private double getValue(uint addr)
        {
            double value = 0;
            if (td.ElementSize == 1)
            {
                if (td.Signed)
                    value = unchecked((sbyte)PCM.buf[addr]);
                else
                    value = PCM.buf[addr];
            }
            if (td.ElementSize == 2)
            {
                if (td.Signed)
                    value = BEToInt16(PCM.buf, addr);
                else
                    value = BEToUint16(PCM.buf, addr);
            }
            if (td.ElementSize == 4)
            {
                if (td.Signed)
                    value = BEToInt32(PCM.buf, addr);
                else
                    value = BEToUint32(PCM.buf, addr);
            }
            string mathStr = td.Math.ToLower().Replace("x", value.ToString());
            if (commaDecimal) mathStr = mathStr.Replace(".", ",");
            value = parser.Parse(mathStr, false);
            return value;
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
            if (foundTables[tId].Description.Length > 0)
                this.Text += " - " + foundTables[tId].Description;

            FoundTable ft = foundTables[tId];

            td = new TableData();
            td.AddrInt = ft.addrInt;
            td.Address = ft.Address;
            td.Category = ft.Category;
            td.DataType = tSeek.DataType;
            td.ElementSize = (byte)(tSeek.Bits / 8);
            td.Math = tSeek.Math;
            td.SavingMath = tSeek.SavingMath;
            td.OS = PCM.OS;
            td.RowMajor = tSeek.RowMajor;
            td.Rows = ft.Rows;
            td.Columns = ft.Columns;
            td.ColumnHeaders = tSeek.ColHeaders;
            td.RowHeaders = tSeek.RowHeaders;
            td.Decimals = tSeek.Decimals;
            td.Signed = tSeek.Signed;
            td.TableDescription = tSeek.Description;
            td.TableName = ft.Name;
            td.Units = tSeek.Units;

            loadTable(td, PCM);
        }


        public void loadTable(TableData td1, PcmFile PCM1)
        {

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            NumberFormatInfo nfi = new CultureInfo(currentCulture, false).NumberFormat;
            if (nfi.NumberDecimalSeparator == ",") commaDecimal = true;
            else commaDecimal = false;

            PCM = PCM1;
            td = td1;

            this.Text = "Table Editor: " + td.TableName;
            if (td.TableDescription != null && td.TableDescription.Length > 0)
                this.Text += " - " + td.TableDescription;

            if (td.Units != null)
                labelUnits.Text = "units: " + td.Units;

            int rowCount = td.Rows;
            int colCount = td.Columns;

            string[] colHeaders = td.ColumnHeaders.Split(',');
            string[] rowHeaders = td.RowHeaders.Split(',');
            if (chkTranspose.Checked)
            {
                //Swap col/row
                rowCount = td.Columns;
                colCount = td.Rows;
                colHeaders = td.RowHeaders.Split(',');
                rowHeaders = td.ColumnHeaders.Split(',');
            }
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = colCount;

            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader; // .AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);
            uint addr = td.AddrInt;
            int step = (int)(td.ElementSize);
            double value = 0;

            if (chkTranspose.Checked ^ td.RowMajor == false)
            {
                for (int r=0; r < rowCount; r++)
                    dataGridView1.Rows.Add();
                for (int c = 0; c < colCount; c++)
                {
                    for (int r = 0; r < rowCount; r++)
                    {
                        value = getValue(addr);
                        dataGridView1.Rows[r].Cells[c].Value = value;
                        dataGridView1.Rows[r].Cells[c].Tag = addr;
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
                        value = getValue(addr);
                        dataGridView1.Rows[index].Cells[c].Value = value;
                        dataGridView1.Rows[index].Cells[c].Tag = addr;
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
            string formatStr = "0";
            for (int f = 1; f <= td.Decimals; f++)
            {
                if (f > 0) formatStr += ".";
                formatStr += "0";
            }
            formatStr += "#";
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvc.DefaultCellStyle.Format = formatStr;
            }
            tableModified = false;
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                tableModified = true;
                chkTranspose.Enabled = false;
            }
        }

        private void SaveValue(uint addr,int r, int c)
        {
            MathParser parser = new MathParser();

            double value = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
            string mathStr = td.SavingMath.ToLower().Replace("x", value.ToString());
            if (commaDecimal) mathStr = mathStr.Replace(".", ",");
            value = parser.Parse(mathStr, true);
            if (td.ElementSize == 1)
            {
                PCM.buf[addr] = (byte)value;
            }
            if (td.ElementSize == 2)
            {
                if (td.Signed)
                {
                    short newValue = (short)value;
                    PCM.buf[addr] = (byte)((newValue & 0xFF00) >> 8);
                    PCM.buf[addr + 1] = (byte)(newValue & 0xFF);
                }
                else
                {
                    ushort newValue = (ushort)value;
                    PCM.buf[addr] = (byte)((newValue & 0xFF00) >> 8);
                    PCM.buf[addr + 1] = (byte)(newValue & 0xFF);
                }

            }
            if (td.ElementSize == 4)
            {
                if (td.DataType == 1)
                {
                    byte[] buffer = BitConverter.GetBytes((float)value);
                    PCM.buf[addr] = buffer[0];
                    PCM.buf[addr + 1] = buffer[1];
                    PCM.buf[addr + 2] = buffer[2];
                    PCM.buf[addr + 3] = buffer[3];
                }
                else
                {
                    if (td.Signed)
                    {
                        Int32 newValue = (Int32)value;
                        PCM.buf[addr] = (byte)((newValue & 0xFF000000) >> 24);
                        PCM.buf[addr + 1] = (byte)((newValue & 0xFF0000) >> 16);
                        PCM.buf[addr + 2] = (byte)((newValue & 0xFF00) >> 8);
                        PCM.buf[addr + 3] = (byte)(newValue & 0xFF);
                    }
                    else
                    {
                        UInt32 newValue = (UInt32)value;
                        PCM.buf[addr] = (byte)((newValue & 0xFF000000) >> 24);
                        PCM.buf[addr + 1] = (byte)((newValue & 0xFF0000) >> 16);
                        PCM.buf[addr + 2] = (byte)((newValue & 0xFF00) >> 8);
                        PCM.buf[addr + 3] = (byte)(newValue & 0xFF);
                    }
                }
            }

        }
        private void saveTable()
        {
            try
            {

                for (int r=0; r< dataGridView1.Rows.Count; r++)
                {
                    for (int c=0; c< dataGridView1.Columns.Count; c++)
                    {
                        SaveValue(Convert.ToUInt32(dataGridView1.Rows[r].Cells[c].Tag), r, c);
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
                }
                tableModified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void AutoResize()
        {
            int dgv_width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
            if (dgv_width < 175) dgv_width = 175;
            int dgv_height = dataGridView1.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            Screen myScreen = Screen.FromPoint(Cursor.Position);
            System.Drawing.Rectangle area = myScreen.WorkingArea;
            if ((dgv_width + 125) > area.Width)
                this.Width = area.Width - 50;
            else
                this.Width = dgv_width + 125;
            if ((dgv_height + 100) > area.Height)
                this.Height = area.Height - 50;
            else
                this.Height = dgv_height + 130;

        }
        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            chkAutoResize.Checked = Properties.Settings.Default.TableEditorAutoResize;
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
            Properties.Settings.Default.TableEditorAutoResize = chkAutoResize.Checked;
            Properties.Settings.Default.Save();
            if (chkAutoResize.Checked)
            {
                AutoResize();
            }
        }

        private void chkTranspose_CheckedChanged(object sender, EventArgs e)
        {
            loadTable(td, PCM);
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            if (chkAutoResize.Checked) AutoResize();
        }
    }
}