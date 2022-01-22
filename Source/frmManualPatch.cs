using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmManualPatch : Form
    {
        public frmManualPatch()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select help-file", TxtFilter);
            if (FileName.Length > 0)
                txtHelpFile.Text = Path.GetFileName(FileName);
        }
    }

}
