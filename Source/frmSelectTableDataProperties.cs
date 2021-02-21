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
        public void loadProperties(TableData td, bool showValues=true)
        {
            int top = 19;
            foreach (var prop in td.GetType().GetProperties())
            {
                if (prop.Name == "id")
                    continue;
                CheckBox chk = new CheckBox();
                chk.Name = prop.Name;
                chk.Text = prop.Name;
                if (showValues)
                {
                    chk.Text += ": " + prop.GetValue(td, null);
                    chk.Tag = prop.GetValue(td, null);
                }
                if (prop.Name == "TableName")
                    chk.Checked = true;
                chk.AutoSize = true;
                chkBoxes.Add(chk);
                groupBox1.Controls.Add(chk);
                chk.Location = new Point(11, top);
                top += 23;
            }
            this.Height = top + 50;
        }

        public void loadFiles(List<frmMassModifyTableData.TunerFile> tunerFiles, List<string> tableNames)
        {
            //This is not in use, filelist needs scrollbars...
            chkBoxes = new List<CheckBox>();
            int top = 19;
            for (int f = 0; f < tunerFiles.Count; f++)
            {
                for (int t = 0; t < tunerFiles[f].tableDatas.Count; t++)
                {
                    if (tableNames.Contains(tunerFiles[f].tableDatas[t].TableName) )
                    {
                        CheckBox chk = new CheckBox();
                        chk.Name = chk + f.ToString();
                        chk.Text = tunerFiles[f].FileName;
                        chk.Checked = true;
                        chk.Tag = f;
                        chkBoxes.Add(chk);
                        groupBox1.Controls.Add(chk);
                        chk.Location = new Point(11, top);
                        top += 23;
                        break;
                    }
                }
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
