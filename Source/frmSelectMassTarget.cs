using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static UniversalPatcher.ExtensionMethods;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmSelectMassTarget : Form
    {
        public frmSelectMassTarget()
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);

        }

        public void LoadData(List<frmMassModifyTableData.TableDataExtended> tdeList)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.DataSource = tdeList;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
            dataGridView1.DataError += DataGridView1_DataError;
        }


        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UseComboBoxForEnums(dataGridView1);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmSelectMassTarget_Load(object sender, EventArgs e)
        {
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                if (chkSelectAll.Checked)
                    dataGridView1.Rows[r].Cells["Select"].Value = true;
                else
                    dataGridView1.Rows[r].Cells["Select"].Value = false;
            }

        }
    }
}
