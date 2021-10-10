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
    public partial class frmEditDetectAddress : Form
    {
        public frmEditDetectAddress()
        {
            InitializeComponent();
        }

        public string Result;

        public void ParseAddress(string OldAddr)
        {

            if (OldAddr.Length == 0)
                return;
            if (OldAddr.StartsWith("@"))
                radioStartRead.Checked = true;
            else
                radioStartAbsolute.Checked = true;

            if (OldAddr.Contains(":"))
            {
                string[] Parts = OldAddr.Split(':');
                OldAddr = Parts[0];
                ushort x;
                if (UInt16.TryParse(Parts[1], out x))
                    numBytes.Value = x;
            }
            if (OldAddr.Contains("@:") || OldAddr.EndsWith("@"))
                chkEndofFile.Checked = true;
            txtStart.Text = OldAddr.Replace("@", "");

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = "";
            if (txtStart.Text.Length == 0)
                return;
            if (radioStartAbsolute.Checked)
            {
                 Result = txtStart.Text;
            }
            else
            {
                Result = "@" + txtStart.Text;
            }
            if (chkEndofFile.Checked)
                Result += "@";
            Result += ":" + numBytes.Value.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
