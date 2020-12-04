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
    public partial class frmDbEditor : Form
    {
        public frmDbEditor()
        {
            InitializeComponent();
        }

        private void frmDbEditor_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'tinyTunerDataSet.TableData' table. You can move, or remove it, as needed.
            this.tableDataTableAdapter.Fill(this.tinyTunerDataSet.TableData);
            //dataGridView1.DataSource = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=TinyTuner.mdb";
        }
    }
}
