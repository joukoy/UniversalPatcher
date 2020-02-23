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
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnSegments_Click(object sender, EventArgs e)
        {
            frmSegmentSettings frmSS = new frmSegmentSettings();
            frmSS.InitMe();
            frmSS.Show();
        }

        private void btnPatcher_Click(object sender, EventArgs e)
        {
            FrmPatcher FrmP = new FrmPatcher();
            FrmP.Show();
        }
    }
}
