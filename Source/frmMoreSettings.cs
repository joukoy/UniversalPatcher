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
            UniversalPatcher.Properties.Settings.Default.TunerMinTableEquivalency = (int)numTunerTableMinEquivalency.Value;
            UniversalPatcher.Properties.Settings.Default.keyPressWait100ms = (int)numKeypressWait.Value;
            UniversalPatcher.Properties.Settings.Default.TableEditorMinOtherEquivalency = (int)numTunerMinEqOther.Value;
            UniversalPatcher.Properties.Settings.Default.MulitableChars = txtMultitableChars.Text;
            UniversalPatcher.Properties.Settings.Default.DisableAutoFixChecksum = chkDisableAutoCS.Checked;
            UniversalPatcher.Properties.Settings.Default.RequireValidVerForStock = chkRequireValidVerForStock.Checked;
            UniversalPatcher.Properties.Settings.Default.AutomaticOpenImportedFile = chkAutoOpenImportedFile.Checked;
            UniversalPatcher.Properties.Settings.Default.TunerShowUnitsImperial = chkDisplayImperial.Checked;
            UniversalPatcher.Properties.Settings.Default.TunerShowUnitsMetric = chkDisplayMetric.Checked;
            UniversalPatcher.Properties.Settings.Default.TunerShowUnitsUndefined = chkDisplayUndefined.Checked;
            UniversalPatcher.Properties.Settings.Default.SplashShowTime = (int)numSplashTime.Value;
            UniversalPatcher.Properties.Settings.Default.xdfImportUseTableName = chkXdfUseTableName.Checked;
            UniversalPatcher.Properties.Settings.Default.FlashApp = txtFlashApp.Text;
            UniversalPatcher.Properties.Settings.Default.FLashParams = txtFlashParams.Text;
            UniversalPatcher.Properties.Settings.Default.LoggerUseIntegrated = chkUseIntegratedLogger.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerExternalApp = txtExternalLogger.Text;
            //UniversalPatcher.Properties.Settings.Default.startPatcher = chkStartPatcher.Checked;
            UniversalPatcher.Properties.Settings.Default.Save();
            this.Close();
        }

        private void frmTunerSettings_Load(object sender, EventArgs e)
        {
            numTunerTableMinEquivalency.Value = UniversalPatcher.Properties.Settings.Default.TunerMinTableEquivalency;
            numKeypressWait.Value = UniversalPatcher.Properties.Settings.Default.keyPressWait100ms;
            numTunerMinEqOther.Value = UniversalPatcher.Properties.Settings.Default.TableEditorMinOtherEquivalency;
            txtMultitableChars.Text = UniversalPatcher.Properties.Settings.Default.MulitableChars;
            chkDisableAutoCS.Checked = UniversalPatcher.Properties.Settings.Default.DisableAutoFixChecksum;
            chkRequireValidVerForStock.Checked = UniversalPatcher.Properties.Settings.Default.RequireValidVerForStock;
            chkAutoOpenImportedFile.Checked = UniversalPatcher.Properties.Settings.Default.AutomaticOpenImportedFile;
            chkDisplayImperial.Checked = UniversalPatcher.Properties.Settings.Default.TunerShowUnitsImperial;
            chkDisplayMetric.Checked = UniversalPatcher.Properties.Settings.Default.TunerShowUnitsMetric;
            chkDisplayUndefined.Checked = UniversalPatcher.Properties.Settings.Default.TunerShowUnitsUndefined;
            numSplashTime.Value = UniversalPatcher.Properties.Settings.Default.SplashShowTime;
            chkXdfUseTableName.Checked = UniversalPatcher.Properties.Settings.Default.xdfImportUseTableName;
            txtFlashApp.Text = UniversalPatcher.Properties.Settings.Default.FlashApp;
            txtFlashParams.Text = UniversalPatcher.Properties.Settings.Default.FLashParams;
            txtExternalLogger.Text = UniversalPatcher.Properties.Settings.Default.LoggerExternalApp;
            //chkStartPatcher.Checked = UniversalPatcher.Properties.Settings.Default.startPatcher;
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
