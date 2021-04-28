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
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmHexDiff : Form
    {
        public frmHexDiff(PcmFile _pcm1, PcmFile _pcm2, List<int> _tdList, List<int> _tdList2)
        {
            InitializeComponent();
            pcm1 = _pcm1;
            pcm2 = _pcm2;
            tdList = _tdList;
            tdList2 = _tdList2;
        }
        private void frmHexDiff_Load(object sender, EventArgs e)
        {

        }
        private class TableDiff
        {
            public int id { get; set; }
            public int id2 { get; set; }
            public string addr1 { get; set; }
            public string addr2 { get; set; }
            public string TableName { get; set; }
            public string Data1 { get; set; }
            public string Data2 { get; set; }
            public TableData td;
        }
        private List<TableDiff> tdiffList;
        private PcmFile pcm1;
        private PcmFile pcm2;
        private List<int> tdList;
        private List<int> tdList2;

        SortOrder strSortOrder = SortOrder.Ascending;
        private int sortIndex = 0;
        private string sortBy = "id";
        BindingSource bindingSource = new BindingSource();

        public void findDifferences(bool showAsHex)
        {
            if (!showAsHex)
                this.Text = "File differences";
            tdiffList = new List<TableDiff>();
            labelFileNames.Text = pcm1.FileName + " <> " + pcm2.FileName;

            for (int t = 0; t < tdList.Count; t++)
            {
                TableData td = pcm1.tableDatas[tdList[t]];
                TableData td2 = pcm2.tableDatas[tdList2[t]];
                uint step = (uint)getElementSize(td.DataType);
                uint step2 = (uint)getElementSize(td2.DataType);
                int count = td.Rows * td.Columns;
                uint addr = (uint)(td.addrInt + td.Offset);
                uint addr2 = (uint)(td2.addrInt + td2.Offset);
                string data1 = "";
                string data2 = "";
                string formatStr = "";
                if (showAsHex)
                    formatStr = "X" + (step * 2).ToString();
                for (int a = 0; a < count; a++)
                {
                    if (showAsHex)
                    {
                        data1 += ((double)getRawValue(pcm1.buf, addr, td, 0)).ToString(formatStr) + " ";
                        data2 += ((double)getRawValue(pcm2.buf, addr2, td2, 0)).ToString(formatStr) + " ";
                    }
                    else
                    {
                        data1 += getValue(pcm1.buf, addr, td, 0,pcm1).ToString(formatStr) + " ";
                        data2 += getValue(pcm2.buf, addr2, td2, 0,pcm2).ToString(formatStr) + " ";
                    }
                    addr += step;
                    addr2 += step2;
                }

                TableDiff tDiff = new TableDiff();
                tDiff.Data1 = data1.Trim();
                tDiff.Data2 = data2.Trim();
                tDiff.id = tdList[t];
                tDiff.TableName = td.TableName;
                tDiff.td = td;
                tDiff.id2 = tdList2[t];
                tDiff.addr1 = td.Address;
                tDiff.addr2 = td2.Address;
                tdiffList.Add(tDiff);
            }
            dataGridView1.DataSource = bindingSource;
            bindingSource.DataSource = tdiffList;
            dataGridView1.CellMouseDoubleClick += DataGridView1_CellMouseDoubleClick;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
        }

        private void filterTables()
        {
            List<TableDiff> compareList = new List<TableDiff>();
            if (strSortOrder == SortOrder.Ascending)
                compareList = tdiffList.OrderBy(x => typeof(TableDiff).GetProperty(sortBy).GetValue(x, null)).ToList();
            else
                compareList = tdiffList.OrderByDescending(x => typeof(TableDiff).GetProperty(sortBy).GetValue(x, null)).ToList();
            var results = compareList.Where(t=> t.TableName.ToString().ToLower().Contains(txtFilter.Text.Trim()));
            bindingSource.DataSource = results;
            Application.DoEvents();
            if (dataGridView1.Columns.Count > sortIndex)
                dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //saveGridLayout(); //Save before reorder!
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = getSortOrder(sortIndex);
                filterTables();
            }
        }

        private SortOrder getSortOrder(int columnIndex)
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
                Debug.WriteLine(ex.Message);
            }
            return SortOrder.Ascending;
        }

        private void DataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                int id2 = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id2"].Value);
                TableData td = pcm1.tableDatas[id];
                frmTableEditor frmT = new frmTableEditor();
                List<int> tableIds = new List<int>();
                tableIds.Add(id);
                frmT.prepareTable(pcm1, td, tableIds,"A");
                frmT.addCompareFiletoMenu(pcm2, pcm2.tableDatas[id2],"B:" + pcm2.FileName);
                frmT.Show();
                frmT.loadTable();
                frmT.radioSideBySide.Checked = true;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void saveCSV()
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataGridView1.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataGridView1.Rows[r].Cells[i].Value != null)
                            row += dataGridView1.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }


        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            filterTables();
        }

        private void btnSaveCsv_Click(object sender, EventArgs e)
        {
            saveCSV();
        }
    }
}
