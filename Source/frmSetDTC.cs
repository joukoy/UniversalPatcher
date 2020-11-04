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
            comboDtcStatus.Items.Clear();

            for (int i = 0; i < dtcStatus.Length; i++)
                comboDtcStatus.Items.Add(dtcStatus[i]);

        }

        public int codeIndex;

        public void startMe(int code)
        {
            codeIndex = code;
            comboDtcStatus.Text = dtcStatus[dtcCodes[code].Status];
            labelCode.Text = dtcCodes[code].Code;
            labelDescription.Text = dtcCodes[code].Description;
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
