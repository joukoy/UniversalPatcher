using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;
namespace UniversalPatcher
{
    public partial class frmSelectDBC : Form
    {
        public frmSelectDBC(List<DBC.DBCMsg> MsgList)
        {
            InitializeComponent();
            Msgs = MsgList;
        }
        public DBC.DBCMsg SelectedMsg;
        public List<DBC.DBCMsg> SelectedMsgs;
        public List<DBC.DBCSignal> SelectedSignals;
        private List<DBC.DBCMsg> Msgs;
        private void frmSelectDBC_Load(object sender, EventArgs e)
        {
            labelByteNumber.Text = "B\ny\nt\ne\n\nn\nu\nm\nb\ne\nr";
            for (int b=7;b>=0;b--)
            {
                dataGridBitView.Columns.Add(b.ToString(), b.ToString());
                //dataGridBitView.Columns[b.ToString()].Width = 20;
            }
            dataGridMsg.SelectionChanged += DataGridMsg_SelectionChanged;
            dataGridMsg.DataBindingComplete += DataGridMsg_DataBindingComplete;
            dataGridSignal.DataBindingComplete += DataGridSignal_DataBindingComplete;
            dataGridSignal.SelectionChanged += DataGridSignal_SelectionChanged;
            if (Msgs == null)
            {
                if (datalogger.dbcMessages == null || datalogger.dbcMessages.Count == 0)
                {
                    LoadDBCFile();
                }
                Msgs = datalogger.dbcMessages;
            }
            BindingList<DBC.DBCMsg> blMsg = new BindingList<DBC.DBCMsg>(Msgs);
            dataGridMsg.DataSource = blMsg;
            if (Msgs.Count > 0)
            {
                dataGridMsg.CurrentCell = dataGridMsg.Rows[0].Cells[0];
                //LoadSelectedMsg();
            }
            SelectedMsgs = new List<DBC.DBCMsg>();
            SelectedSignals = new List<DBC.DBCSignal>();
        }

        private void DataGridSignal_SelectionChanged(object sender, EventArgs e)
        {
            dataGridBitView.Rows.Clear();
            if (dataGridSignal.SelectedCells.Count == 0 || dataGridMsg.SelectedCells.Count == 0)
            {
                return;
            }
            DBC.DBCMsg msg = dataGridMsg.Rows[dataGridMsg.SelectedCells[0].RowIndex].DataBoundItem as DBC.DBCMsg;
            DBC.DBCSignal SelectedSignal = dataGridSignal.Rows[dataGridSignal.SelectedCells[0].RowIndex].DataBoundItem as DBC.DBCSignal;
            for (int row = 0; row < msg.DataLength;row++)
            {
                dataGridBitView.Rows.Add();
                int byteNum = row;
                if (!SelectedSignal.MSB)
                {
                    byteNum = msg.DataLength - row -1;
                }
                dataGridBitView.Rows[row].HeaderCell.Value = byteNum.ToString();
            }
            if (SelectedSignal.MSB)
            {
                int bitNumber = 7;
                int selectedbitcount = 0;
                for (int row = 0; row < msg.DataLength ; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        dataGridBitView.Rows[row].Cells[col].Value = bitNumber;
                        if (bitNumber == SelectedSignal.BitStartPos)
                        {
                            selectedbitcount = 1;
                            dataGridBitView.CurrentCell = dataGridBitView.Rows[row].Cells[col];
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightSteelBlue;
                        }
                        else if (selectedbitcount > 0 && selectedbitcount< SelectedSignal.NumBits)
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
                for (int row = msg.DataLength-1; row >= 0 ; row--)
                {
                    for (int col = 7; col >= 0; col--)
                    {
                        dataGridBitView.Rows[row].Cells[col].Value = bitNumber;
                        if (bitNumber == SelectedSignal.BitStartPos)
                        {
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightSteelBlue;
                            dataGridBitView.CurrentCell = dataGridBitView.Rows[row].Cells[col];
                        }
                        else if (bitNumber > SelectedSignal.BitStartPos && bitNumber < (SelectedSignal.BitStartPos + SelectedSignal.NumBits))
                        {
                            dataGridBitView.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                        }
                        bitNumber++;
                    }
                }
            }
            dataGridBitView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridBitView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }

        private void DataGridSignal_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridSignal.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void DataGridMsg_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridMsg.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void LoadSelectedMsg()
        {
            if (dataGridMsg.SelectedCells.Count == 0)
            {
                return;
            }
            DBC.DBCMsg msg = dataGridMsg.Rows[dataGridMsg.SelectedCells[0].RowIndex].DataBoundItem as DBC.DBCMsg;
            BindingList<DBC.DBCSignal> blSignal = new BindingList<DBC.DBCSignal>(msg.Signals);
            dataGridSignal.DataSource = blSignal;
        }
        private void DataGridMsg_SelectionChanged(object sender, EventArgs e)
        {
            LoadSelectedMsg();
        }

        private void LoadDBCFile()
        {
            string fName = SelectFile("Select DBC file", DbcFilter);
            if (fName.Length == 0)
            {
                return;
            }
            labelFileName.Text = fName;
            datalogger.dbcMessages = DBC.ReadDbcFile(fName);
            BindingList<DBC.DBCMsg> blMsg = new BindingList<DBC.DBCMsg>(datalogger.dbcMessages);
            dataGridMsg.DataSource = blMsg;

        }
        private void loadDBCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDBCFile();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dataGridMsg.SelectedCells.Count == 0)
            {
                return;
            }
            SelectedMsg = dataGridMsg.Rows[dataGridMsg.SelectedCells[0].RowIndex].DataBoundItem as DBC.DBCMsg;
            if (dataGridSignal.SelectedCells.Count == 0)
            {
                return;
            }
            List<int> rows = new List<int>();
            for (int c=0;c< dataGridSignal.SelectedCells.Count;c++)
            {
                if (!rows.Contains(dataGridSignal.SelectedCells[c].RowIndex))
                {
                    rows.Add(dataGridSignal.SelectedCells[c].RowIndex);
                }
            }
            foreach (int row in rows)
            {
                DBC.DBCSignal SelectedSignal = dataGridSignal.Rows[row].DataBoundItem as DBC.DBCSignal;
                SelectedSignals.Add(SelectedSignal);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnImportAllMsgs_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            if (dataGridMsg.SelectedCells.Count == 0)
            {
                return;
            }
            List<int> rows = new List<int>();
            for (int c = 0; c < dataGridMsg.SelectedCells.Count; c++)
            {
                if (!rows.Contains(dataGridMsg.SelectedCells[c].RowIndex))
                {
                    rows.Add(dataGridMsg.SelectedCells[c].RowIndex);
                }
            }
            foreach (int row in rows)
            {
                DBC.DBCMsg selectedmsg = dataGridMsg.Rows[row].DataBoundItem as DBC.DBCMsg;
                SelectedMsgs.Add(selectedmsg);
            }
            this.Close();
        }
    }
}
