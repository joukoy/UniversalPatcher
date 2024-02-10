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
    public partial class frmMapSession : Form
    {
        public frmMapSession()
        {
            InitializeComponent();
        }

        private void frmMapSession_Load(object sender, EventArgs e)
        {
            txtMapPin.Text = AppSettings.MapSessionMapBin;
            txtRefBin.Text = AppSettings.MapSessionRefBin;
            txtRefTableList.Text = AppSettings.MapSessionRefTableList;
            txtSessionName.Text = "map-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            chkUseAutoloadTables.Checked = AppSettings.MapSessionUseAutoLoadTableList;
        }

        private void btnMapPin_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select MAP bin");
            if (!string.IsNullOrEmpty(fName))
            {
                txtMapPin.Text = fName;
            }
        }

        private void btnRefBin_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select Reference bin");
            if (!string.IsNullOrEmpty(fName))
            {
                txtRefBin.Text = fName;
            }
        }

        private void btnRefDef_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select Reference definition file", XmlFilter);
            if (!string.IsNullOrEmpty(fName))
            {
                txtRefTableList.Text = fName;
            }
        }

        private void btnSessionFolder_Click(object sender, EventArgs e)
        {
            string fName = SelectFolder("Select session folder");
            if (!string.IsNullOrEmpty(fName))
            {
                txtSessionName.Text = fName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AppSettings.MapSessionMapBin = txtMapPin.Text;
            AppSettings.MapSessionRefBin = txtRefBin.Text;
            AppSettings.MapSessionRefTableList = txtRefTableList.Text;
            AppSettings.MapSessionUseAutoLoadTableList = chkUseAutoloadTables.Checked;
            AppSettings.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
