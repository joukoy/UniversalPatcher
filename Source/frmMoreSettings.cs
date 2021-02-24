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
            this.Close();
        }

        private void frmTunerSettings_Load(object sender, EventArgs e)
        {
            numTunerTableMinEquivalency.Value = Properties.Settings.Default.TunerMinTableEquivalency;
            numKeypressWait.Value = Properties.Settings.Default.keyPressWait100ms;
            numTunerMinEqOther.Value = Properties.Settings.Default.TableEditorMinOtherEquivalency;
        }

        private void numKeypressWait_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
