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
    public partial class frmSetDTC : Form
    {
        public frmSetDTC()
        {
            InitializeComponent();
        }

        private void frmSetDTC_Load(object sender, EventArgs e)
        {

        }

        public int codeIndex;

        public void startMe(int code, string pcmType)
        {
            codeIndex = code;
            comboDtcStatus.Items.Clear();

            if (pcmType == "e38" || pcmType == "e67")
            {
                for (int i = 0; i < dtcStatusE38.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatusE38[i]);
                comboDtcStatus.Text = dtcStatusE38[dtcCodesE38[code].Status];
                labelCode.Text = dtcCodesE38[code].Code;
                labelDescription.Text = dtcCodesE38[code].Description;
                comboMIL.Visible = false;
                labelMil.Visible = false;
            }
            else if (pcmType == "p01-p59")
            {
                for (int i = 0; i < dtcStatusP59.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatusP59[i]);
                comboMIL.Items.Add("Disabled");
                comboMIL.Items.Add("Enabled");
                comboMIL.SelectedIndex = dtcCodesP59[code].MilStatus;
                comboDtcStatus.Text = dtcStatusP59[dtcCodesP59[code].Status];
                labelCode.Text = dtcCodesP59[code].Code;
                labelDescription.Text = dtcCodesP59[code].Description;

            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
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
