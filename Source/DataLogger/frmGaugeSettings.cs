using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmGaugeSettings : Form
    {
        public UpGauge gauge;
        public frmGaugeSettings(UpGauge gauge)
        {
            InitializeComponent();
            this.gauge = gauge;
        }

        private void frmGaugeSettings_Load(object sender, EventArgs e)
        {
            foreach (LogParam.PidSettings pidProfile in datalogger.SelectedPids)
            {

                comboPidName.Items.Add(pidProfile.Parameter.Name);
            }
           
            comboGaugetype.ValueMember = "Value";
            comboGaugetype.DisplayMember = "Name";
            comboGaugetype.DataSource = Enum.GetValues(typeof(UpGauge.GaugeType)).Cast<object>().Select(v => new
            {
                Value = (int)v,
                Name =  v.ToString() 
            }).ToList();

            LoadPidSettings();
        }

        private void LoadPidSettings()
        {
            if (string.IsNullOrEmpty(gauge.PidName))
            {
                txtCaption.Enabled = false;
            }
            else
            {
                txtCaption.Enabled = true;
                comboPidName.Text = gauge.PidName;
                txtCaption.Text = comboPidName.Text;
                comboGaugetype.Text = gauge.Type.ToString();
            }
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            gauge.CapText = txtCaption.Text;
            gauge.MinValue = (double)numMin.Value;
            gauge.MaxValue = (double)numMax.Value;
            gauge.Type = (UpGauge.GaugeType)comboGaugetype.SelectedValue;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            fpe.LoadObject(gauge.Gauge, "Gauges");
            fpe.Show();
        }

        private void comboPidName_SelectedIndexChanged(object sender, EventArgs e)
        {
            gauge.PidName = comboPidName.Text;
            LoadPidSettings();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
