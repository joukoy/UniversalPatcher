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
    public partial class frmCreateShortcuts : Form
    {
        public frmCreateShortcuts()
        {
            InitializeComponent();
        }

        private void radioCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCustom.Checked)
            {
                string dst = SelectFolder("Select location for shortcuts");
                if (dst.Length == 0)
                    return;
                radioCustom.Text = dst;
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                Logger("Generating shortcuts...",false);
                string dst = "";
                if (radioCustom.Checked)
                    dst = radioCustom.Text;
                if (chkPatcherTourist.Checked)
                    CreateShortcut(dst, "Patcher-Tourist", "tourist patcher");
                if (chkPatcherBasic.Checked)
                    CreateShortcut(dst, "Patcher-Basic", "basic patcher");
                if (chkPatcherAdvanced.Checked)
                    CreateShortcut(dst, "Patcher-Advanced", "advanced patcher");

                if (chkTunerTourist.Checked)
                    CreateShortcut(dst, "Tuner-Tourist", "tourist tuner");
                if (chkTunerBasic.Checked)
                    CreateShortcut(dst, "Tuner-Basic", "basic tuner");
                if (chkTunerAdvanced.Checked)
                    CreateShortcut(dst, "Tuner-Advanced", "advanced patcher");

                Logger(" [OK]");
                this.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
