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
    public partial class frmEditAddress : Form
    {
        public frmEditAddress()
        {
            InitializeComponent();
        }

        public string Result = "";

        public void ParseAddress(string OldAddr)
        {
            if (OldAddr.Length > 0)
            {
                string[] Parts = OldAddr.Split(':');
                txtAddress.Text = Parts[0].Replace("#", "");
                txtAddress.Text = txtAddress.Text.Replace("@", "");
                if (Parts[0].StartsWith("#"))
                {
                    if (Parts[0].EndsWith("@"))
                        radioEndSegment.Checked = true;
                    else
                        radioRelative.Checked = true;
                }
                else
                {
                    if (Parts[0].EndsWith("@"))
                        radioEndFile.Checked = true;
                    else
                        radioAbsolute.Checked = true;
                }
                if (Parts.Length > 1)
                {
                    ushort x;
                    if (HexToUshort(Parts[1], out x))
                        numBytes.Value = x;
                }
                if (Parts.Length > 2)
                {
                    if (Parts[2].ToLower().Contains("hex"))
                        radioHEX.Checked = true;
                    else if (Parts[2].ToLower().Contains("text") || Parts[2].ToLower().Contains("txt"))
                        radioText.Checked = true;
                }

            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text.Length == 0)
                return;
            Result = "";
            if (radioRelative.Checked || radioEndSegment.Checked)
                Result += "#";
            if (radioEndSegment.Checked || radioEndFile.Checked)
                txtAddress.Text += "@";
            Result += txtAddress.Text + ":" + numBytes.Value.ToString() + ":";

            if (radioHEX.Checked)
                Result += "hex";
            else if (radioText.Checked)
                Result += "text";
            else
                Result += "int";

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
