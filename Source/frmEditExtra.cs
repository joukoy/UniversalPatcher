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
    public partial class frmEditExtra : Form
    {
        public frmEditExtra()
        {
            InitializeComponent();
        }

        public string Result = "";

        public void InitMe(string OldAddr, SegmentConfig S)
        {
            if (S.CheckWords != null)
            {
                string[] CWS = S.CheckWords.Split(',');
                foreach (string CW in CWS)
                {
                    string[] Parts = CW.Split(':');
                    if (Parts.Length == 4)
                    {
                        if (!comboCheckword.Items.Contains(Parts[3]))
                            comboCheckword.Items.Add(Parts[3]);
                    }
                }
            }
            if (OldAddr.Length > 0)
            {
                string[] Extras = OldAddr.Split(',');
                foreach (string Extra in Extras) 
                {
                    listExtra.Items.Add(Extra);
                }
            }

        }

        private string CreateRow()
        {
            string Row = txtName.Text + ":";
            if (radioRelative.Checked)
                Row += "#";
            if (radioCheckword.Checked)
                Row += comboCheckword.Text;
            if (radioCheckword.Checked && !txtAddress.Text.StartsWith("-") && !txtAddress.Text.StartsWith("+"))
                Row += "+";
            Row += txtAddress.Text + ":" + numBytes.Value.ToString() + ":";

            if (radioHEX.Checked)
                Row += "hex";
            else if (radioText.Checked)
                Row += "text";
            else
                Row += "int";
            return Row;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text.Length == 0)
                return;
            if (txtName.Text.Length == 0)
                return;
            string Row = CreateRow();
            listExtra.Items.Add(Row);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioRelative_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioAbsolute_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioCheckword_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCheckword.Checked)
                comboCheckword.Enabled = true;
            else
                comboCheckword.Enabled = false;
        }

        private void listExtra_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] Parts = listExtra.Text.Split(':');
            if (Parts.Length < 4)
                return;
            txtName.Text = Parts[0];
            if (Parts[1].StartsWith("#"))
                radioRelative.Checked = true;
            else
                radioAbsolute.Checked = true;
            if (Parts[1].Contains("CW"))
            {
                string[] CWParts = Parts[1].Split('W');  //To make compiler happy
                if (Parts[1].Contains("-"))
                { 
                    CWParts = Parts[1].Split('-');
                    CWParts[1] = "-" + CWParts[1];
                }
                if (Parts[1].Contains("+"))
                    CWParts = Parts[1].Split('+');
                radioCheckword.Checked = true;
                comboCheckword.Text = CWParts[0];
                Parts[1] = CWParts[1];
            }
            txtAddress.Text = Parts[1].Replace("#", "");
            ushort x;
            if (HexToUshort(Parts[2], out x))
            numBytes.Value = x;
            if (Parts[3].ToLower().Contains("hex"))
                radioHEX.Checked = true;
            else if (Parts[3].ToLower().Contains("text") || Parts[2].ToLower().Contains("txt"))
                radioText.Checked = true;

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = "";
            for  (int i=0;i<listExtra.Items.Count;i++)
            {
                if (i > 0)
                    Result += ",";
                Result += listExtra.Items[i].ToString();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listExtra.SelectedItems.Count == 0)
                return;
            listExtra.Items.RemoveAt(listExtra.SelectedIndex);
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text.Length == 0)
                return;
            if (txtName.Text.Length == 0)
                return;
            string Row = CreateRow();
            int pos = listExtra.SelectedIndex;
            listExtra.Items.RemoveAt(pos);
            listExtra.Items.Insert(pos,Row);
        }
    }
}
