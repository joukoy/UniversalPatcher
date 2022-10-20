using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;

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

        public void StartMe(int code, PcmFile PCM, List<DtcCode> dtcCodes)
        {
            codeIndex = code;
            comboDtcStatus.Items.Clear();

            comboDtcStatus.DataSource = PCM.dtcValues.ToList();
            comboDtcStatus.DisplayMember = "Value";
            comboDtcStatus.ValueMember = "Key";
            if (PCM.dtcCombined)
            {
                /*for (int i = 0; i < dtcStatusCombined.Length; i++)
                    comboDtcStatus.Items.Add(dtcStatusCombined[i]);*/
                comboMIL.Visible = false;
                labelMil.Visible = false;
                comboDtcStatus.Text = PCM.dtcValues[dtcCodes[code].Status].ToString();
                labelCode.Text = dtcCodes[code].Code;
                labelDescription.Text = dtcCodes[code].Description;
            }
            else
            {
                comboMIL.Items.Add("Disabled");
                comboMIL.Items.Add("Enabled");
                comboMIL.SelectedIndex = dtcCodes[code].MilStatus;
                comboDtcStatus.Text = PCM.dtcValues[dtcCodes[code].Status].ToString();
                labelCode.Text = dtcCodes[code].Code;
                labelDescription.Text = dtcCodes[code].Description;
            }
            if (dtcCodes[code].TypeTxt == "")
            {
                labelType.Visible = false;
                comboType.Visible = false;
            }
            else
            {
                comboType.Items.Add("TypeA");
                comboType.Items.Add("TypeB");
                comboType.Text = dtcCodes[code].TypeTxt.ToString();
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
