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
                AppSettings.PasteSpecialMode = 0;
            else if (radioMultiply.Checked)
                AppSettings.PasteSpecialMode = 1;
            else if (radioPercent.Checked)
                AppSettings.PasteSpecialMode = 2;
            else if (radioTarget.Checked)
                AppSettings.PasteSpecialMode = 3;
            else if (radioCustom.Checked)
                AppSettings.PasteSpecialMode = 4;
            AppSettings.PasteSpecialPositiveFormula = txtCustomPositive.Text;
            AppSettings.PasteSpecialNegativeFormula = txtCustomNegative.Text;
            AppSettings.PasteSpecialTarget = txtTarget.Text;
            AppSettings.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmPasteSpecial_Load(object sender, EventArgs e)
        {
            switch(AppSettings.PasteSpecialMode)
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
            txtCustomPositive.Text = AppSettings.PasteSpecialPositiveFormula;
            txtCustomNegative.Text = AppSettings.PasteSpecialNegativeFormula;
            txtTarget.Text = AppSettings.PasteSpecialTarget;
        }
    }

}
