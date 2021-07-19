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
            PcmFile PCM = new PcmFile();
            frmSL.InitMe();

        }

        private void btnPatcher_Click(object sender, EventArgs e)
        {
            try
            {
                if (frmP != null && frmP.Visible)
                {
                    frmP.BringToFront();
                    return;
                }
                frmP = new FrmPatcher();
                frmP.Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnTuner_Click(object sender, EventArgs e)
        {
            PcmFile PCM = new PcmFile();
            frmTuner frmT = new frmTuner(PCM);
            frmT.Show();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                frmMoreSettings frmTS = new frmMoreSettings();
                frmTS.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }
    }
}
