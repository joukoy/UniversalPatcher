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
    public partial class frmSelectTableDataProperties : Form
    {
        public frmSelectTableDataProperties()
        {
            InitializeComponent();
        }

        public List<CheckBox> chkBoxes = new List<CheckBox>();
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public void loadProperties(TableData td)
        {
            int top = 19;
            foreach (var prop in td.GetType().GetProperties())
            {
                if (prop.Name == "id")
                    continue;
                CheckBox chk = new CheckBox();
                chk.Name = prop.Name;
                chk.Text = prop.Name + ": " + prop.GetValue(td, null);
                chk.Tag = prop.GetValue(td, null);
                chk.AutoSize = true;
                chkBoxes.Add(chk);
                groupBox1.Controls.Add(chk);
                chk.Location = new Point(11, top);
                top += 23;
            }
            this.Height = top + 50;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                for (int c = 0; c < chkBoxes.Count; c++)
                    chkBoxes[c].Checked = true;
            }
            else
            {
                for (int c = 0; c < chkBoxes.Count; c++)
                    chkBoxes[c].Checked = false;
            }
        }
    }
}
