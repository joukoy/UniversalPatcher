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
    public partial class frmEditXML : Form
    {
        public frmEditXML()
        {
            InitializeComponent();
        }

        private BindingSource bindingSource = new BindingSource();

        public void LoadRules()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = DetectRules;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
