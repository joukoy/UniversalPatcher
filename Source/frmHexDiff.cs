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
    public partial class frmHexDiff : Form
    {
        public frmHexDiff(PcmFile _pcm1, PcmFile _pcm2, List<int> _tdList)
        {
            InitializeComponent();
            pcm1 = _pcm1;
            pcm2 = _pcm2;
            tdList = _tdList;
            findDifferences();
        }
        private void frmHexDiff_Load(object sender, EventArgs e)
        {

        }
        private class TableDiff
        {
            public int id { get; set; }
            public string TableName { get; set; }
            public string File1 { get; set; }
            public string Data1 { get; set; }
            public string File2 { get; set; }
            public string Data2 { get; set; }
            public TableData td;
        }
        private List<TableDiff> tdiffList;
        private PcmFile pcm1;
        private PcmFile pcm2;
        private List<int> tdList;

        private void findDifferences()
        {
            tdiffList = new List<TableDiff>();

            for (int t = 0; t < tdList.Count; t++)
            {
                TableData td = pcm1.tableDatas[tdList[t]];
                uint step = (uint)getElementSize(td.DataType);
                int count = td.Rows * td.Columns;
                uint addr = (uint)(td.addrInt + td.Offset);
                List<int> tableIds = new List<int>();
                tableIds.Add((int)td.id);
                string data1 = "";
                string data2 = "";
                string formatStr = "X" + (step * 2).ToString();
                for (int a = 0; a < count; a++)
                {
                    data1 += getRawValue(pcm1.buf,addr, td,0).ToString(formatStr) + " ";
                    data2 += getRawValue(pcm2.buf,addr, td,0).ToString(formatStr) + " ";
                    addr += step; 
                }
                TableDiff tDiff = new TableDiff();
                tDiff.Data1 = data1.Trim();
                tDiff.Data2 = data2.Trim();
                tDiff.File1 = pcm1.FileName;
                tDiff.File2 = pcm2.FileName;
                tDiff.id = tdList[t];
                tDiff.TableName = td.TableName;
                tDiff.td = td;
                tdiffList.Add(tDiff);
            }
            dataGridView1.DataSource = tdiffList;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.CellMouseDoubleClick += DataGridView1_CellMouseDoubleClick;

        }


        private void DataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
            TableData td = pcm1.tableDatas[id];
            frmTableEditor frmT = new frmTableEditor(pcm1,pcm2);
            List<int> tableIds = new List<int>();
            tableIds.Add(id);
            frmT.prepareTable(td, tableIds);
            frmT.Show();
            frmT.loadTable(true);
        }


    }
}
