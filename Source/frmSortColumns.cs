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
    public partial class frmSortColumns : Form
    {
        public frmSortColumns(string Cols)
        {
            InitializeComponent();
            this.Cols = Cols;
        }

        private string Cols;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int c=0;c<listBox1.Items.Count;c++)
            {
                string col = listBox1.Items[c].ToString();
                sb.Append(col + ",");
            }            
            if (AppSettings.WorkingMode == 2)
            {
                AppSettings.ConfigModeColumnOrder = sb.ToString().Trim(',');
            }
            else
            {
                AppSettings.TunerModeColumns = sb.ToString().Trim(',');
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (listBox1.SelectedItems.Count == 0)
                {
                    return;
                }
                this.listBox1.SelectedIndexChanged -= new System.EventHandler(this.listBox1_SelectedIndexChanged);
                string tmp = listBox1.SelectedItem.ToString();
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                int pos = vScrollBar1.Value;
                if (pos > listBox1.Items.Count)
                {
                    pos = listBox1.Items.Count ;
                }
                listBox1.Items.Insert(pos, tmp);
                listBox1.SelectedIndex = pos;
                this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmSortColumns , line " + line + ": " + ex.Message);
            }
        }

        private void frmSortColumns_Load(object sender, EventArgs e)
        {
            string[] columns = Cols.Split(',');
            foreach (string col in columns)
            {
                listBox1.Items.Add(col);
            }
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = listBox1.Items.Count - 1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vScrollBar1.Value = listBox1.SelectedIndex;
        }
    }
}
