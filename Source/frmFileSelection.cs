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
            listFiles.Columns.Add("File");
            listFiles.Columns.Add("Folder");
            listFiles.Columns[0].Width = 300;
            listFiles.Columns[1].Width = 500;

            listFiles.CheckBoxes = true;
            if (Folder == "")
                Folder = Application.StartupPath;
            DirectoryInfo d = new DirectoryInfo(Folder);
            FileInfo[] Files;
            if (chkSubfolders.Checked)
                Files = d.GetFiles("*.*", SearchOption.AllDirectories);
            else
                Files = d.GetFiles("*.*");

            string[] filters;
            if (fileTypeList != null && fileTypeList.Count > 0 && chkIncludeCustomFileTypes.Checked)
            {
                filters = new string[fileTypeList.Count + 1];
                filters[0] = ".bin";
                for (int f = 0; f < fileTypeList.Count; f++)
                    filters[f + 1] = "." + fileTypeList[f].Extension;
            }
            else
            {
                filters = new string[1];
                filters[0] = ".bin";
            }
            foreach (FileInfo file in Files)
            {
                for (int f = 0; f < filters.Length; f++)
                {
                    if (file.Extension.ToLower() == filters[f].ToLower())
                    {
                        var item = new ListViewItem(file.Name);
                        item.Tag = file.FullName;
                        item.SubItems.Add(file.DirectoryName);
                        listFiles.Items.Add(item);
                        break;
                    }
                }
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
            string Folder = SelectFolder("Select folder");
            if (Folder.Length > 0)
                LoadFiles(Folder);
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

        private void btnCustomdst_Click(object sender, EventArgs e)
        {
            string Folder = SelectFolder("Select destination folder");
            if (Folder.Length > 0)
                labelCustomdst.Text = Folder;
        }

        private void chkSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            LoadFiles(txtFolder.Text);
        }

        private void chkIncludeCustomFileTypes_CheckedChanged(object sender, EventArgs e)
        {
            LoadFiles(txtFolder.Text);
        }
    }
}
