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

        private DataTable dt;
        private int tId;
        private TableSeek tSeek;
        private PcmFile PCM;
        private bool tableModified = false;
        private bool commaDecimal = true;

        public void loadTable(int tableId, PcmFile PCM1)
        {

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            NumberFormatInfo nfi = new CultureInfo(currentCulture, false).NumberFormat;
            if (nfi.NumberDecimalSeparator == ",") commaDecimal = true;
            else commaDecimal = false;

            PCM = PCM1;
            tId = tableId;
            MathParser parser = new MathParser();
            dt = new DataTable("data");
            tSeek = tableSeeks[foundTables[tableId].configId];
            this.Text = "Table Editor: " + tSeek.Name;
            string[] colHeaders = tSeek.ColHeaders.Split(',');
            for (int c = 0; c < tSeek.Columns; c++)
            {
                string headerTxt = "";
                if (c > colHeaders.Length || colHeaders[0].Length == 0)
                    headerTxt = "";
                else
                    headerTxt = colHeaders[c];
                if (tSeek.DataType == 1 )
                    dt.Columns.Add(headerTxt, typeof(double)); //System.Type.GetType("System.Decimal"));
                if (tSeek.DataType == 2)
                    dt.Columns.Add(headerTxt, typeof(int));  //System.Type.GetType("System.Int32"));
                if (tSeek.DataType == 3 || tSeek.DataType == 4)
                    dt.Columns.Add(headerTxt, typeof(string));
            }
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader; // .AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);
            uint addr = foundTables[tableId].addrInt;
            int step = (int)(tSeek.Bits / 8);
            double value = 0;
            for (int r = 0; r < tSeek.Rows; r++)
            {
                var dRow = new object[tSeek.Columns];
                for (int c = 0; c < tSeek.Columns; c++)
                {
                    if (tSeek.Bits == 8)
                    {
                        if (tSeek.Signed)
                            value = unchecked((sbyte)PCM.buf[addr]);
                        else
                            value = PCM.buf[addr];
                    }
                    if (tSeek.Bits == 16)
                    {
                        if (tSeek.Signed)
                            value = BEToInt16(PCM.buf, addr);
                        else
                            value = BEToUint16(PCM.buf, addr);
                    }
                    if (tSeek.Bits == 32)
                    {
                        if (tSeek.Signed)
                            value = BEToInt32(PCM.buf, addr);
                        else
                            value = BEToUint32(PCM.buf, addr);
                    }
                    string mathStr = tSeek.Math.ToLower().Replace("x", value.ToString());
                    if (commaDecimal) mathStr = mathStr.Replace(".", ",");
                    value = parser.Parse(mathStr, false);
                    dRow[c] = value;

                    addr += (uint)step;
                }
                dt.Rows.Add(dRow);
            }
            dataGridView1.DataSource = dt;
            tableModified = false;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            string[] rowHeaders = tSeek.RowHeaders.Split(',');
            for (int r = 0; r < tSeek.Rows; r++)
            {
                if (r < rowHeaders.Length)
                    dataGridView1.Rows[r].HeaderCell.Value = rowHeaders[r];
            }
            string formatStr = "0";
            for (int f=0; f< tSeek.Decimals; f++)
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
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1) tableModified = true;
        }

        private void saveTable()
        {
            try
            {
                uint addr = foundTables[tId].addrInt;
                int step = (int)(tSeek.Bits / 8);
                double value = 0;
                MathParser parser = new MathParser();
                for (int r = 0; r < tSeek.Rows; r++)
                {
                    for (int c = 0; c < tSeek.Columns; c++)
                    {
                        value = Convert.ToDouble(dt.Rows[r].ItemArray[c]);
                        string mathStr = tSeek.SavingMath.ToLower().Replace("x", value.ToString());
                        if (commaDecimal) mathStr = mathStr.Replace(".", ",");
                        value = parser.Parse(mathStr, true);
                        if (tSeek.Bits == 8)
                        {
                            PCM.buf[addr] = (byte)value;
                        }
                        if (tSeek.Bits == 16)
                        {
                            if (tSeek.Signed)
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
                        if (tSeek.Bits == 32)
                        {
                            if (tSeek.DataType == 1)
                            {
                                byte[] buffer = BitConverter.GetBytes((float)value);
                                PCM.buf[addr] = buffer[0];
                                PCM.buf[addr + 1] = buffer[1];
                                PCM.buf[addr + 2] = buffer[2];
                                PCM.buf[addr + 3] = buffer[3];
                            }
                            else
                            {
                                if (tSeek.Signed)
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
                        addr += (uint)step;
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
        private void frmTableEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
    }
}