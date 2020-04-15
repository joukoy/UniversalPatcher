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

        private frmSegmenList frmSL;
        private FrmPatcher frmP;

        private void btnSegments_Click(object sender, EventArgs e)
        {

            if (frmSL != null && frmSL.Visible)
            {
                frmSL.BringToFront();
                return;
            }
            frmSL = new frmSegmenList();
            frmSL.Show(this);
            frmSL.InitMe();

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
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Patches")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Patches"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "XML")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "XML"));

            Properties.Settings.Default.LastBINfolder = Properties.Settings.Default.LastBINfolder;
            Properties.Settings.Default.LastPATCHfolder = Properties.Settings.Default.LastPATCHfolder;
            Properties.Settings.Default.LastXMLfolder = Properties.Settings.Default.LastXMLfolder;

            if (Properties.Settings.Default.LastXMLfolder == "")
                Properties.Settings.Default.LastXMLfolder = Path.Combine(Application.StartupPath, "XML");
            if (Properties.Settings.Default.LastPATCHfolder == "")
                Properties.Settings.Default.LastPATCHfolder = Path.Combine(Application.StartupPath, "Patches");
        }
    }
}
