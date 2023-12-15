using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmEditPairs : Form
    {
        public frmEditPairs()
        {
            InitializeComponent();
        }
        public string pairStr { get; set; }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            StringBuilder sb = new StringBuilder();
            for(int r=0;r<dataGridView1.Rows.Count;r++)
            {
                if (!string.IsNullOrEmpty(dataGridView1.Rows[r].Cells[0].Value as string) && !string.IsNullOrEmpty(dataGridView1.Rows[r].Cells[1].Value as string))
                {
                    sb.Append(dataGridView1.Rows[r].Cells[0].Value.ToString() + ":" + dataGridView1.Rows[r].Cells[1].Value.ToString() + ",");
                }
            }
            pairStr = sb.ToString().Trim(',');
            this.Close();
        }

        private void ParsePairs()
        {
            try
            {
                dataGridView1.Columns.Add("OS", "OS");
                dataGridView1.Columns.Add("Address", "Address");
                if (string.IsNullOrEmpty(pairStr) || !pairStr.Contains(":"))
                {
                    return;
                }
                string[] rows = pairStr.Split(',');
                foreach (string row in rows)
                {
                    string[] parts = row.Split(':');
                    int r = dataGridView1.Rows.Add();
                    dataGridView1.Rows[r].Cells[0].Value = parts[0];
                    dataGridView1.Rows[r].Cells[1].Value = parts[1];
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmEditPairs , line " + line + ": " + ex.Message);
            }

        }
        private void frmEditPairs_Load(object sender, EventArgs e)
        {
            ParsePairs();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
