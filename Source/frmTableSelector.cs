using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmTableSelector : Form
    {
        public frmTableSelector()
        {
            InitializeComponent();
        }

        private List<TableData> tableList;
        private IEnumerable<TableData> sortedList;
        private BindingSource bindingSource = new BindingSource();
        public TableData selectedTd;
        public PcmFile PCM = null;
        SortOrder strSortOrder = SortOrder.Ascending;
        private string sortBy = "TableName";
        private int sortIndex = 1;

        private void frmTableSelector_Load(object sender, EventArgs e)
        {
            dataGridView1.CellMouseDoubleClick += DataGridView1_CellMouseDoubleClick;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
        }

        private void SortResults()
        {
            try
            {
                if (strSortOrder == SortOrder.Ascending)
                    sortedList = sortedList.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    sortedList = sortedList.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                bindingSource.DataSource = sortedList;
                bindingSource.ResetBindings(false);
                dataGridView1.Update();
                this.Refresh();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableselector , line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = GetSortOrder(sortIndex);
                SortResults();
            }
        }

        private void DataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SelectAndClose();
        }

        private void LoadXML(string fName)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
            System.IO.StreamReader file = new System.IO.StreamReader(fName);
            tableList= (List<TableData>)reader.Deserialize(file);
            sortedList = tableList;
            file.Close();
        }
        private void AddDataSource()
        {
            bindingSource.DataSource = sortedList;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Update();
            Application.DoEvents();
            TableData tdTmp = new TableData();
            string[] selected = new string[] { "TableName", "Rows", "Columns", "TableDescription", "RowHeaders","ColumnHeaders" };
            foreach (var prop in tdTmp.GetType().GetProperties())
            {
                if (!selected.Contains(prop.Name))
                {
                    dataGridView1.Columns[prop.Name].Visible = false;
                }
            }
            dataGridView1.Columns["TableName"].Width = 200;
            dataGridView1.Columns["Rows"].Width = 70;
            dataGridView1.Columns["Columns"].Width = 70;
            dataGridView1.Columns["TableDescription"].Width = 1000;
        }
        private void btnLoadXML_Click(object sender, EventArgs e)
        {
            string defFile = Path.Combine(Application.StartupPath, "Tuner","*.xml");
            string fName = SelectFile("Select tablelist XML", TablelistFilter, defFile);
            if (fName == "")
                return;

            LoadXML(fName);
            AddDataSource();
        }

        private void SelectAndClose()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            selectedTd = (TableData)dataGridView1.SelectedRows[0].DataBoundItem;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            SelectAndClose();
        }

        private void btnLoadBin_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select BIN file", BinFilter);
            if (fName == "")
                return;
            PCM = new PcmFile(fName, true, "");
            PCM.AutoLoadTunerConfig();
            tableList = PCM.tableDatas;
            sortedList = tableList;
            AddDataSource();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilter.Text))
            {
                sortedList = tableList;
            }
            else
            {
                sortedList = tableList.Where(X => X.TableName.ToLower().Contains(txtFilter.Text.ToLower()));
            }
            bindingSource.DataSource = sortedList;
            bindingSource.ResetBindings(false);
            dataGridView1.Update();
            this.Refresh();
        }
        private SortOrder GetSortOrder(int columnIndex)
        {
            try
            {
                if (dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                    || dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    return SortOrder.Ascending;
                }
                else
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    return SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableselector , line " + line + ": " + ex.Message);
            }
            return SortOrder.Ascending;
        }

    }
}
