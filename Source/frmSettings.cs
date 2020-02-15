using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;
using System.IO;

namespace UniversalPatcher
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }



        private void frmSettings_Load(object sender, EventArgs e)
        {
            listView1.Enabled = true;
            listView1.Clear();
            listView1.View = View.Details;
            listView1.Columns.Add("Start");
            listView1.Columns.Add("End");
            listView1.Columns[0].Width = 250;
            listView1.Columns[1].Width = 250;
            listView1.FullRowSelect = true;
            if (Exclude != null)
            {
                foreach(ExcludeBlock EB in Exclude)
                {
                    var item = new ListViewItem(EB.Start.ToString("X"));
                    item.SubItems.Add(EB.End.ToString("X"));
                    listView1.Items.Add(item);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                txtStart.Text = listView1.SelectedItems[0].Text;
                txtEnd.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var item = new ListViewItem(txtStart.Text);
            item.SubItems.Add(txtEnd.Text);
            listView1.Items.Add(item);

        }

        private void bntDel_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].Remove();
            }

        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].Text = txtStart.Text;
                listView1.SelectedItems[0].SubItems[1].Text = txtEnd.Text;
            }

        }

        private void SortList()
        {
            //Sort:
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                for (int j = i + 1; j < listView1.Items.Count; j++)
                {
                    uint x = 0;
                    uint y = 0;
                    UInt32.TryParse(listView1.Items[i].Text, out x);
                    UInt32.TryParse(listView1.Items[j].Text, out y);
                    if (x > y)
                    {
                        string tmp1 = listView1.Items[j].Text;
                        string tmp2 = listView1.Items[j].SubItems[1].Text;
                        listView1.Items[j].Text = listView1.Items[i].Text;
                        listView1.Items[j].SubItems[1].Text = listView1.Items[i].SubItems[1].Text;
                        listView1.Items[i].Text = tmp1;
                        listView1.Items[i].SubItems[1].Text = tmp2;
                    }
                }
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("INI files (*.ini)|*.ini|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            SortList();

            StreamWriter sw = new StreamWriter(FileName);
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                sw.WriteLine(listView1.Items[i].Text + "-" + listView1.Items[i].SubItems[1].Text);
            }
            sw.Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("INI files (*.ini)|*.ini|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            StreamReader sr = new StreamReader(FileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] LineParts = line.Split('-');
                if (LineParts.Length > 1)
                {
                    var item = new ListViewItem(LineParts[0]);
                    item.SubItems.Add(LineParts[1]);
                    listView1.Items.Add(item);
                }
            }
            sr.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SortList();
            Exclude = new List<ExcludeBlock>();
            ExcludeBlock EB = new ExcludeBlock();

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                UInt32.TryParse(listView1.Items[i].Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out EB.Start);
                UInt32.TryParse(listView1.Items[i].SubItems[1].Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out EB.End);

                Exclude.Add(EB);                
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
