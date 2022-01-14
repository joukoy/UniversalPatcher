using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmMoreSettings : Form
    {
        public frmMoreSettings()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TunerMinTableEquivalency = (int)numTunerTableMinEquivalency.Value;
            Properties.Settings.Default.keyPressWait100ms = (int)numKeypressWait.Value;
            Properties.Settings.Default.TableEditorMinOtherEquivalency = (int)numTunerMinEqOther.Value;
            Properties.Settings.Default.MulitableChars = txtMultitableChars.Text;
            Properties.Settings.Default.DisableAutoFixChecksum = chkDisableAutoCS.Checked;
            Properties.Settings.Default.RequireValidVerForStock = chkRequireValidVerForStock.Checked;
            Properties.Settings.Default.AutomaticOpenImportedFile = chkAutoOpenImportedFile.Checked;
            Properties.Settings.Default.TunerShowUnitsImperial = chkDisplayImperial.Checked;
            Properties.Settings.Default.TunerShowUnitsMetric = chkDisplayMetric.Checked;
            Properties.Settings.Default.TunerShowUnitsUndefined = chkDisplayUndefined.Checked;
            Properties.Settings.Default.SplashShowTime = (int)numSplashTime.Value;
            Properties.Settings.Default.xdfImportUseTableName = chkXdfUseTableName.Checked;
            Properties.Settings.Default.FlashApp = txtFlashApp.Text;
            Properties.Settings.Default.FLashParams = txtFlashParams.Text;
            //Properties.Settings.Default.startPatcher = chkStartPatcher.Checked;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void frmTunerSettings_Load(object sender, EventArgs e)
        {
            numTunerTableMinEquivalency.Value = Properties.Settings.Default.TunerMinTableEquivalency;
            numKeypressWait.Value = Properties.Settings.Default.keyPressWait100ms;
            numTunerMinEqOther.Value = Properties.Settings.Default.TableEditorMinOtherEquivalency;
            txtMultitableChars.Text = Properties.Settings.Default.MulitableChars;
            chkDisableAutoCS.Checked = Properties.Settings.Default.DisableAutoFixChecksum;
            chkRequireValidVerForStock.Checked = Properties.Settings.Default.RequireValidVerForStock;
            chkAutoOpenImportedFile.Checked = Properties.Settings.Default.AutomaticOpenImportedFile;
            chkDisplayImperial.Checked = Properties.Settings.Default.TunerShowUnitsImperial;
            chkDisplayMetric.Checked = Properties.Settings.Default.TunerShowUnitsMetric;
            chkDisplayUndefined.Checked = Properties.Settings.Default.TunerShowUnitsUndefined;
            numSplashTime.Value = Properties.Settings.Default.SplashShowTime;
            chkXdfUseTableName.Checked = Properties.Settings.Default.xdfImportUseTableName;
            txtFlashApp.Text = Properties.Settings.Default.FlashApp;
            txtFlashParams.Text = Properties.Settings.Default.FLashParams;
            //chkStartPatcher.Checked = Properties.Settings.Default.startPatcher;
        }

        private void numKeypressWait_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowseApp_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select fash application", "EXE (*.exe)|*.exe|All (*.*)|*.*");
            if (fName.Length == 0)
                return;
            txtFlashApp.Text = fName;
        }
    }
}
