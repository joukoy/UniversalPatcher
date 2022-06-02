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
    public partial class frmPasteSpecial : Form
    {
        public frmPasteSpecial()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (radioAdd.Checked)
                Properties.Settings.Default.PasteSpecialMode = 0;
            else if (radioMultiply.Checked)
                Properties.Settings.Default.PasteSpecialMode = 1;
            else if (radioPercent.Checked)
                Properties.Settings.Default.PasteSpecialMode = 2;
            else if (radioTarget.Checked)
                Properties.Settings.Default.PasteSpecialMode = 3;
            else if (radioCustom.Checked)
                Properties.Settings.Default.PasteSpecialMode = 4;
            Properties.Settings.Default.PasteSpecialPositiveFormula = txtCustomPositive.Text;
            Properties.Settings.Default.PasteSpecialNegativeFormula = txtCustomNegative.Text;
            Properties.Settings.Default.PasteSpecialTarget = txtTarget.Text;
            Properties.Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmPasteSpecial_Load(object sender, EventArgs e)
        {
            switch(Properties.Settings.Default.PasteSpecialMode)
            {
                case 0:
                    radioAdd.Checked = true;
                    break;
                case 1:
                    radioMultiply.Checked = true;
                    break;
                case 2:
                    radioPercent.Checked = true;
                    break;
                case 3:
                    radioTarget.Checked = true;
                    break;
                case 4:
                    radioCustom.Checked = true;
                    break;
            }
            txtCustomPositive.Text = Properties.Settings.Default.PasteSpecialPositiveFormula;
            txtCustomNegative.Text = Properties.Settings.Default.PasteSpecialNegativeFormula;
            txtTarget.Text = Properties.Settings.Default.PasteSpecialTarget;
        }
    }

}
