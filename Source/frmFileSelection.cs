using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmFileSelection : Form
    {
        public frmFileSelection()
        {
            InitializeComponent();
        }

        public void LoadFiles(string Folder)
        {
            listFiles.Enabled = true;
            listFiles.Items.Clear();
            listFiles.View = View.Details;
            listFiles.Columns.Add("Files");
            listFiles.Columns[0].Width = 1000;

            listFiles.CheckBoxes = true;
            DirectoryInfo d = new DirectoryInfo(Folder);
            FileInfo[] Files = d.GetFiles("*.bin");
            foreach (FileInfo file in Files)
            {
                var item = new ListViewItem(file.Name);
                item.Tag = file.FullName;
                listFiles.Items.Add(item);
            }
            txtFolder.Text = Folder;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select one file from folder");
            if (FileName.Length > 0)
                LoadFiles(Path.GetDirectoryName(FileName));
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i=0; i< listFiles.Items.Count; i++)
            {
                listFiles.Items[i].Checked = true;
            }
        }

        private void txtFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoadFiles(txtFolder.Text);

        }
    }
}
