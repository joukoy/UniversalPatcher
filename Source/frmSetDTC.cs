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

        public void startMe(int code, string xmlFile)
        {
            codeIndex = code;
            comboDtcStatus.Items.Clear();

            if (xmlFile == "e38" || xmlFile == "e67")
            {
                for (int i = 0; i < dtcStatusE38.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatusE38[i]);
                comboDtcStatus.Text = dtcStatusE38[dtcCodesE38[code].Status];
                labelCode.Text = dtcCodesE38[code].Code;
                labelDescription.Text = dtcCodesE38[code].Description;
                comboMIL.Visible = false;
                labelMil.Visible = false;
            }
            else 
            {
                for (int i = 0; i < dtcStatus.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatus[i]);
                comboMIL.Items.Add("Disabled");
                comboMIL.Items.Add("Enabled");
                comboMIL.SelectedIndex = dtcCodes[code].MilStatus;
                comboDtcStatus.Text = dtcStatus[dtcCodes[code].Status];
                labelCode.Text = dtcCodes[code].Code;
                labelDescription.Text = dtcCodes[code].Description;

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
