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
                switch(AppSettings.WorkingMode)
                {
                    case 1:
                        radioBasic.Checked = true;
                        break;
                    case 2:
                        radioAdvanced.Checked = true;
                        break;
                    default:
                        radioTourist.Checked = true;
                        break;
                }
                switch(AppSettings.OpenInStartup)
                {
                    case UpatcherSettings.StartupUtil.Launcher:
                        radioLauncher.Checked = true;
                        break;
                    case UpatcherSettings.StartupUtil.Tuner:
                        radioTuner.Checked = true;
                        break;
                    case UpatcherSettings.StartupUtil.Logger:
                        radioLogger.Checked = true;
                        break;
                    case UpatcherSettings.StartupUtil.Patcher:
                        radioLogger.Checked = true;
                        break;
                }
                ToolTip t1 = new ToolTip();
                t1.SetToolTip(radioTourist, "Very limited funcionality, only basic Open/Save is available");
                t1.SetToolTip(radioBasic, "Common functions available");
                t1.SetToolTip(radioAdvanced, "For professional users. All functions available");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        private void btnTuner_Click(object sender, EventArgs e)
        {
            PcmFile PCM = new PcmFile();
            if (AppSettings.TunerUseSessionTabs)
            {
                frmTunerMain ftm = new frmTunerMain(PCM);
                ftm.Show();
            }
            else
            {
                FrmTuner ft = new FrmTuner(PCM);
                ft.Show();
            }

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

        private void setWorkingMode()
        {
            if (radioTourist.Checked)
                AppSettings.WorkingMode = 0;
            else if (radioBasic.Checked)
                AppSettings.WorkingMode = 1;
            else
                AppSettings.WorkingMode = 2;

            AppSettings.Save();
        }
        private void radioTourist_CheckedChanged(object sender, EventArgs e)
        {
            setWorkingMode();
        }

        private void radioBasic_CheckedChanged(object sender, EventArgs e)
        {
            setWorkingMode();
        }

        private void radioAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            setWorkingMode();
        }

        private void btnLogger_Click(object sender, EventArgs e)
        {
            StartLogger(null);
        }

        private void radioLauncher_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLauncher.Checked)
            {
                AppSettings.OpenInStartup = UpatcherSettings.StartupUtil.Launcher;
                AppSettings.Save();
            }
        }

        private void radioLogger_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLogger.Checked)
            {
                AppSettings.OpenInStartup = UpatcherSettings.StartupUtil.Logger;
                AppSettings.Save();
            }
        }

        private void radioTuner_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTuner.Checked)
            {
                AppSettings.OpenInStartup = UpatcherSettings.StartupUtil.Tuner;
                AppSettings.Save();
            }

        }

        private void radioPatcher_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPatcher.Checked)
            {
                AppSettings.OpenInStartup = UpatcherSettings.StartupUtil.Patcher;
                AppSettings.Save();
            }

        }

        private void createShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCreateShortcuts fcs = new frmCreateShortcuts();
            fcs.Show();
        }

        private void serialportServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSerialPortServer fsps = new frmSerialPortServer();
            fsps.Show();
        }
    }
}
