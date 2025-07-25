using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UniversalPatcher
{
    public partial class frmSignalView : Form
    {
        public frmSignalView()
        {
            InitializeComponent();
        }

        private void frmSignalView_Load(object sender, EventArgs e)
        {
            dataGridCanData.Columns.Add("DataHex", "Data (HEX)");
            dataGridCanData.Columns.Add("DataBin", "Data (BIN)");
            for (int b = 7; b >= 0; b--)
            {
                dataGridBitView.Columns.Add(b.ToString(), b.ToString());
                dataGridMask.Columns.Add(b.ToString(), b.ToString());
                dataGridCanData.Columns.Add(b.ToString(), b.ToString());
                dataGridBitView.Columns[b.ToString()].Width = 20;
                dataGridMask.Columns[b.ToString()].Width = 20;
                dataGridCanData.Columns[b.ToString()].Width = 20;
            }
        }
        public void LoadSignal(byte[] data, LogParam.PidParameter parm)
        {
            this.Text = "Signal view: " + parm.Name;
            dataGridBitView.Rows.Clear();
            for (int row = 0; row < parm.PassivePid.MsgLength; row++)
            {
                dataGridBitView.Rows.Add();
                int byteNum = row;
                if (!parm.PassivePid.MSB)
                {
                    byteNum = parm.PassivePid.MsgLength - row - 1;
                }
                dataGridBitView.Rows[row].HeaderCell.Value = byteNum.ToString();
            }
            if (parm.PassivePid.MSB)
            {
                int bitNumber = 7;
                int selectedbitcount = 0;
                for (int row = 0; row < parm.PassivePid.MsgLength; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        dataGridBitView.Rows[row].Cells[col].Value = bitNumber;
                        if (bitNumber == parm.PassivePid.BitStartPos)
                        {
                            selectedbitcount = 1;
                            dataGridBitView.CurrentCell = dataGridBitView.Rows[row].Cells[col];
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightSteelBlue;
                        }
                        else if (selectedbitcount > 0 && selectedbitcount < parm.PassivePid.NumBits)
                        {
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                            selectedbitcount++;
                        }
                        bitNumber--;
                    }
                    bitNumber += 16;
                }
            }
            else
            {
                int bitNumber = 0;
                for (int row = parm.PassivePid.MsgLength - 1; row >= 0; row--)
                {
                    for (int col = 7; col >= 0; col--)
                    {
                        dataGridBitView.Rows[row].Cells[col].Value = bitNumber;
                        if (bitNumber == parm.PassivePid.BitStartPos)
                        {
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightSteelBlue;
                            dataGridBitView.CurrentCell = dataGridBitView.Rows[row].Cells[col];
                        }
                        else if (bitNumber > parm.PassivePid.BitStartPos && bitNumber < (parm.PassivePid.BitStartPos + parm.PassivePid.NumBits))
                        {
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                        }
                        bitNumber++;
                    }
                }
            }
            //dataGridBitView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            //dataGridBitView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            //Data view:
            for (int row = 0; row < data.Length; row++)
            {
                dataGridCanData.Rows.Add();
                dataGridCanData.Rows[row].Cells["DataHex"].Value = data[row].ToString("X2");
                dataGridCanData.Rows[row].Cells["DataBin"].Value = Convert.ToString(data[row], 2).PadLeft(8, '0');
                int mask = 1;
                for (int x = 0; x <8;x++)
                {
                    if ((data[row] & mask) == mask)
                    {
                        dataGridCanData.Rows[row].Cells[x.ToString()].Value = 1;
                    }
                    else
                    {
                        dataGridCanData.Rows[row].Cells[x.ToString()].Value = 0;
                    }
                    mask <<= 1;
                }
            }
            //dataGridCanData.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            //Mask:
            for (int row = 0; row < parm.PassivePid.MsgLength; row++)
            {
                dataGridMask.Rows.Add();
                int byteNum = row;
                if (!parm.PassivePid.MSB)
                {
                    byteNum = parm.PassivePid.MsgLength - row - 1;
                }
                dataGridMask.Rows[row].HeaderCell.Value = byteNum.ToString();
            }
            if (parm.PassivePid.MSB)
            {
                int bitNumber = 7;
                int selectedbitcount = 0;
                for (int row = 0; row < parm.PassivePid.MsgLength; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if (bitNumber == parm.PassivePid.BitStartPos)
                        {
                            selectedbitcount = 1;
                            dataGridMask.CurrentCell = dataGridMask.Rows[row].Cells[col];
                            dataGridMask.Rows[row].Cells[col].Value = 1;
                        }
                        else if (selectedbitcount > 0 && selectedbitcount < parm.PassivePid.NumBits)
                        {
                            dataGridMask.Rows[row].Cells[col].Value = 1;
                            selectedbitcount++;
                        }
                        else
                        {
                            dataGridMask.Rows[row].Cells[col].Value = 0;
                        }
                        bitNumber--;
                    }
                    bitNumber += 16;
                }
            }
            else
            {
                int bitNumber = 0;
                for (int row = parm.PassivePid.MsgLength - 1; row >= 0; row--)
                {
                    for (int col = 7; col >= 0; col--)
                    {
                        if (bitNumber == parm.PassivePid.BitStartPos)
                        {
                            dataGridMask.Rows[row].Cells[col].Value = 1;
                            dataGridMask.CurrentCell = dataGridMask.Rows[row].Cells[col];
                        }
                        else if (bitNumber > parm.PassivePid.BitStartPos && bitNumber < (parm.PassivePid.BitStartPos + parm.PassivePid.NumBits))
                        {
                            dataGridMask.Rows[row].Cells[col].Value = 1;
                        }
                        else
                        {
                            dataGridMask.Rows[row].Cells[col].Value = 0;
                        }
                        bitNumber++;
                    }
                }
            }
            //dataGridMask.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

        }

    }
}
