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
            AppSettings.TunerMinTableEquivalency = (int)numTunerTableMinEquivalency.Value;
            AppSettings.keyPressWait100ms = (int)numKeypressWait.Value;
            AppSettings.TableEditorMinOtherEquivalency = (int)numTunerMinEqOther.Value;
            AppSettings.MultitableChars = txtMultitableChars.Text;
            AppSettings.DisableAutoFixChecksum = chkDisableAutoCS.Checked;
            AppSettings.RequireValidVerForStock = chkRequireValidVerForStock.Checked;
            AppSettings.AutomaticOpenImportedFile = chkAutoOpenImportedFile.Checked;
            AppSettings.TunerShowUnitsImperial = chkDisplayImperial.Checked;
            AppSettings.TunerShowUnitsMetric = chkDisplayMetric.Checked;
            AppSettings.TunerShowUnitsUndefined = chkDisplayUndefined.Checked;
            AppSettings.SplashShowTime = (int)numSplashTime.Value;
            AppSettings.xdfImportUseTableName = chkXdfUseTableName.Checked;
            AppSettings.FlashApp = txtFlashApp.Text;
            AppSettings.FLashParams = txtFlashParams.Text;
            AppSettings.LoggerUseIntegrated = chkUseIntegratedLogger.Checked;
            AppSettings.LoggerExternalApp = txtExternalLogger.Text;
            AppSettings.ConfirmProgramExit = chkConfirmExit.Checked;
            AppSettings.TunerUseSessionTabs = chkUseTunerMain.Checked;
            //AppSettings.startPatcher = chkStartPatcher.Checked;
            AppSettings.Save();
            this.Close();
        }

        private void frmTunerSettings_Load(object sender, EventArgs e)
        {
            numTunerTableMinEquivalency.Value = AppSettings.TunerMinTableEquivalency;
            numKeypressWait.Value = AppSettings.keyPressWait100ms;
            numTunerMinEqOther.Value = AppSettings.TableEditorMinOtherEquivalency;
            txtMultitableChars.Text = AppSettings.MultitableChars;
            chkDisableAutoCS.Checked = AppSettings.DisableAutoFixChecksum;
            chkRequireValidVerForStock.Checked = AppSettings.RequireValidVerForStock;
            chkAutoOpenImportedFile.Checked = AppSettings.AutomaticOpenImportedFile;
            chkDisplayImperial.Checked = AppSettings.TunerShowUnitsImperial;
            chkDisplayMetric.Checked = AppSettings.TunerShowUnitsMetric;
            chkDisplayUndefined.Checked = AppSettings.TunerShowUnitsUndefined;
            numSplashTime.Value = AppSettings.SplashShowTime;
            chkXdfUseTableName.Checked = AppSettings.xdfImportUseTableName;
            txtFlashApp.Text = AppSettings.FlashApp;
            txtFlashParams.Text = AppSettings.FLashParams;
            txtExternalLogger.Text = AppSettings.LoggerExternalApp;
            chkConfirmExit.Checked = AppSettings.ConfirmProgramExit;
            chkUseTunerMain.Checked = AppSettings.TunerUseSessionTabs;
            //chkStartPatcher.Checked = AppSettings.startPatcher;
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
