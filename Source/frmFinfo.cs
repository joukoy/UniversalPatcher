using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{

    public partial class frmFinfo : Form
    {
        public frmFinfo()
        {
            InitializeComponent();
        }

        private BindingSource Finfosource = new BindingSource();

        public void RefreshFileInfo()
        {
            dataFileInfo.DataSource = null;
            Finfosource.DataSource = null;
            Finfosource.DataSource = ListSegment;
            dataFileInfo.DataSource = Finfosource;
        }

        private void frmFinfo_Load(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshFileInfo();
        }
    }
}
