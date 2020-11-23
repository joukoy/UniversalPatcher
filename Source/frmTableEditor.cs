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
            for (int c = 0; c < colHeaders.Length; c++)
            {
                dt.Columns.Add(colHeaders[c], System.Type.GetType("System.Decimal"));
            }

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
                        value = PCM.buf[addr];
                    }
                    if (tSeek.Bits == 16)
                    {
                        value = BEToUint16(PCM.buf, addr);
                    }
                    if (tSeek.Bits == 32)
                    {
                        value = BEToUint32(PCM.buf, addr);
                    }
                    string mathStr = tSeek.Math.ToLower().Replace("x", value.ToString());
                    if (commaDecimal) mathStr = mathStr.Replace(".", ",");
                    dRow[c] = parser.Parse(mathStr, false);
                    addr += (uint)step;
                }
                dt.Rows.Add(dRow);
            }
            dataGridView1.DataSource = dt;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            Application.DoEvents();
            tableModified = false;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            string[] rowHeaders = tSeek.RowHeaders.Split(',');
            for (int r = 0; r < rowHeaders.Length; r++)
            {
                dataGridView1.Rows[r].HeaderCell.Value = rowHeaders[r];
            }
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
                        int intValue = (int)parser.Parse(mathStr, true);
                        if (tSeek.Bits == 8)
                        {
                            PCM.buf[addr] = (byte)value;
                        }
                        if (tSeek.Bits == 16)
                        {
                            PCM.buf[addr] = (byte)((intValue & 0xFF00) >> 8);
                            PCM.buf[addr + 1] = (byte)(intValue & 0xFF);
                        }
                        if (tSeek.Bits == 32)
                        {
                            PCM.buf[addr] = (byte)((intValue & 0xFF000000) >> 24);
                            PCM.buf[addr + 1] = (byte)((intValue & 0xFF0000) >> 16);
                            PCM.buf[addr + 2] = (byte)((intValue & 0xFF00) >> 8);
                            PCM.buf[addr + 3] = (byte)(intValue & 0xFF);

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
    }
}