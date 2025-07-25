using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmPidSelector : Form
    {
        public frmPidSelector()
        {
            InitializeComponent();
        }

        public string Id;
        public string Conversion;
        private void frmPidSelector_Load(object sender, EventArgs e)
        {
            dataGridView1.DataError += DataGridView1_DataError;
            SetupDg();
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void SetupDg()
        {
            try
            {
                dataGridView1.Columns.Add("Id", "Id");
                dataGridView1.Columns.Add("Name", "Name");
                dataGridView1.Columns.Add("Conversion", "Conversion");
                dataGridView1.Columns.Add("Description", "Description");

                for (int p = 0; p < datalogger.PidParams.Count; p++)
                {
                    int row = dataGridView1.Rows.Add();
                    LogParam.PidParameter parm = datalogger.PidParams[p];
                    dataGridView1.Rows[row].Cells["Id"].Value = parm.Id;
                    dataGridView1.Rows[row].Cells["Name"].Value = parm.Name;
                    dataGridView1.Rows[row].Cells["Description"].Value = parm.Description;
                    DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                    dataGridView1.Rows[row].Cells["Conversion"].Tag = parm.Conversions;
                    dgc.DataSource = parm.Conversions;
                    dgc.ValueMember = "units";
                    dgc.DisplayMember = "units";
                    dataGridView1.Rows[row].Cells["Conversion"] = dgc;
                    Application.DoEvents();
                    if (parm.Conversions.Count > 0)
                    {
                        dataGridView1.Rows[row].Cells["Conversion"].Value = parm.Conversions[0].Units;
                    }
                }
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                datalogger.Receiver.SetReceiverPaused(false);
                LoggerBold("Error, frmPidSelector line " + line + ": " + ex.Message);
            }

        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            int row = dataGridView1.SelectedCells[0].RowIndex;
            Id = dataGridView1.Rows[row].Cells["Id"].Value.ToString();
            Conversion = dataGridView1.Rows[row].Cells["Conversion"].Value.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
