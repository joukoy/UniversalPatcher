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
    public partial class frmTableHeaders : Form
    {
        public frmTableHeaders()
        {
            InitializeComponent();
        }

        public string HeaderStr { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            StringBuilder sb = new StringBuilder();
            for (decimal d=numMin.Value; d<=numMax.Value; d+= numStep.Value)
            {
                sb.Append(d.ToString("0.#####").Replace(",",".") + ",");
            }
            HeaderStr = sb.ToString().Trim(',');
            this.Close();
        }
    }
}
