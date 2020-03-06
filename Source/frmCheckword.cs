using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmCheckword : Form
    {
        public frmCheckword()
        {
            InitializeComponent();
        }

        public int CurrentSegment;
        public void InitMe(int SegmentNr)
        {
            CurrentSegment = SegmentNr;
            listCheckwords.Clear();
            listCheckwords.View = View.Details;
            listCheckwords.Columns.Add("Checkword");
            listCheckwords.Columns.Add("Location");
            listCheckwords.Columns.Add("Data Location");
            listCheckwords.Columns[0].Width = 70;
            listCheckwords.Columns[1].Width = 70;
            listCheckwords.Columns[2].Width = 70;
            listCheckwords.MultiSelect = false;
            listCheckwords.CheckBoxes = false;
            listCheckwords.FullRowSelect = true;

            SegmentConfig S = Segments[SegmentNr];

            string[] Rows = S.CheckWords.Split(',');
            foreach (string Row in Rows)
            {
                string[] Parts = Row.Split(':');
                if (Parts.Length == 3)
                {
                    var item = new ListViewItem(Parts[0]);
                    item.SubItems.Add(Parts[1]);
                    item.SubItems.Add(Parts[2]);
                    listCheckwords.Items.Add(item);
                }
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var item = new ListViewItem(txtCheckword.Text);
            item.SubItems.Add(txtCheckwordLocation.Text);
            item.SubItems.Add(txtDatalocation.Text);
            listCheckwords.Items.Add(item);
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (listCheckwords.SelectedItems.Count == 0)
                return;
            listCheckwords.SelectedItems[0].SubItems[0].Text = txtCheckword.Text;
            listCheckwords.SelectedItems[0].SubItems[1].Text = txtCheckwordLocation.Text;
            listCheckwords.SelectedItems[0].SubItems[2].Text = txtDatalocation.Text;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (listCheckwords.SelectedItems.Count == 0)
                return;
            listCheckwords.SelectedItems[0].Remove();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SegmentConfig S = Segments[CurrentSegment];
            S.CheckWords = "";
            for (int s = 0; s < listCheckwords.Items.Count; s++)
            {
                if (s > 0)
                    S.CheckWords += ",";
                S.CheckWords += listCheckwords.Items[s].SubItems[0].Text + ":" + listCheckwords.Items[s].SubItems[1].Text + ":" + listCheckwords.Items[s].SubItems[2].Text;
            }
            Segments[CurrentSegment] = S;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
