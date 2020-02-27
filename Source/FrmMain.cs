using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;

namespace UniversalPatcher
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private frmSegmentSettings frmSS;
        private FrmPatcher frmP;

        private void btnSegments_Click(object sender, EventArgs e)
        {

            if (frmSS != null && frmSS.Visible)
            {
                frmSS.BringToFront();
                return;
            }
            frmSS = new frmSegmentSettings();
            frmSS.Show(this);
            frmSS.InitMe();

        }

        private void btnPatcher_Click(object sender, EventArgs e)
        {
            if (frmP != null && frmP.Visible)
            {
                frmP.BringToFront();
                return;
            }
            frmP = new FrmPatcher();
            frmP.Show(this);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(Application.StartupPath, "Patches")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Patches"));
            if (!File.Exists(Path.Combine(Application.StartupPath, "BinFiles")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "BinFIles"));
            if (!File.Exists(Path.Combine(Application.StartupPath, "XML")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "XML"));

            LastBinfolder = Path.Combine(Application.StartupPath, "BinFiles");
            LastPatchfolder = Path.Combine(Application.StartupPath, "Patches");
            LastXMLfolder = Path.Combine(Application.StartupPath, "XML");
        }
    }
}
