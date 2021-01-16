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

        public void startMe(int code, PcmFile PCM)
        {
            codeIndex = code;
            comboDtcStatus.Items.Clear();

            if (PCM.dtcCombined)
            {
                for (int i = 0; i < dtcStatusCombined.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatusCombined[i]);
                comboMIL.Visible = false;
                labelMil.Visible = false;
                comboDtcStatus.Text = dtcStatusCombined[PCM.dtcCodes[code].Status];
                labelCode.Text = PCM.dtcCodes[code].Code;
                labelDescription.Text = PCM.dtcCodes[code].Description;
            }
            else
            {
                for (int i = 0; i < dtcStatus.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatus[i]);
                comboMIL.Items.Add("Disabled");
                comboMIL.Items.Add("Enabled");
                comboMIL.SelectedIndex = PCM.dtcCodes[code].MilStatus;
                comboDtcStatus.Text = dtcStatus[PCM.dtcCodes[code].Status];
                labelCode.Text = PCM.dtcCodes[code].Code;
                labelDescription.Text = PCM.dtcCodes[code].Description;
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
