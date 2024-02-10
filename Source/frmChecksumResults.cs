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
using static UniversalPatcher.ChecksumSearcher;

namespace UniversalPatcher
{
    public partial class frmChecksumResults : Form
    {
        public frmChecksumResults()
        {
            InitializeComponent();
        }
        private List<CkSearchResult> SearchResults;
        private BindingSource SortedResults = new BindingSource();
        private string OS;
        SortOrder strSortOrder = SortOrder.Ascending;
        private string sortBy = "Start";
        private int sortIndex = 1;
        public List<CkSearchResult> SelectedResults = new List<CkSearchResult>();

        private void frmChecksumResults_Load(object sender, EventArgs e)
        {
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
        }

        public void LoadResults(List<CkSearchResult> SearchResults, string OS)
        {
            this.SearchResults = SearchResults;
            this.OS = OS;
            if (SearchResults != null)
            {
                SortedResults.DataSource = SearchResults.OrderBy(x => typeof(CkSearchResult).GetProperty(sortBy).GetValue(x, null));
                dataGridView1.DataSource = SortedResults; //.OrderBy(x => typeof(CkSearchResult).GetProperty(sortBy).GetValue(x, null));
                for (int c = 1; c < 5; c++)
                {
                    dataGridView1.Columns[c].DefaultCellStyle.Format = "X";
                }
            }
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
                Debug.WriteLine("Error, frmCheksumResults , line " + line + ": " + ex.Message);
            }
            return SortOrder.Ascending;
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

        private void SortResults()
        {
            try
            {
                if (strSortOrder == SortOrder.Ascending)
                    SortedResults.DataSource = SearchResults.OrderBy(x => typeof(CkSearchResult).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    SortedResults.DataSource = SearchResults.OrderByDescending(x => typeof(CkSearchResult).GetProperty(sortBy).GetValue(x, null)).ToList();
                //dataGridView1.DataSource = SortedResults;
                dataGridView1.Refresh();
                this.Refresh();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTuner , line " + line + ": " + ex.Message);
            }
        }
        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<CkSearchResult> newResults = new List<CkSearchResult>();
                List<CkSearchResult> sortedList;
                if (strSortOrder == SortOrder.Ascending)
                    sortedList = SearchResults.OrderBy(x => typeof(CkSearchResult).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    sortedList = SearchResults.OrderByDescending(x => typeof(CkSearchResult).GetProperty(sortBy).GetValue(x, null)).ToList();

                foreach (CkSearchResult result in sortedList)
                {
                    if (result.Select)
                    {
                        newResults.Add(result);
                    }
                }
                if (newResults.Count == 0)
                {
                    if (MessageBox.Show("Nothing selected, save all?","Save results",MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        newResults = SearchResults;
                    }
                }
                if (newResults.Count > 0)
                {
                    string defFile = Path.Combine(Application.StartupPath, "ChecksumSearch", OS + ".XML");
                    string fName = SelectSaveFile(XmlFilter, defFile);
                    if (string.IsNullOrEmpty(fName))
                        return;
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CkSearchResult>));
                        writer.Serialize(stream, newResults);
                        stream.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResults, line " + line + ": " + ex.Message);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string defFile = Path.Combine(Application.StartupPath, "ChecksumSearch", OS + ".XML");
                string fName = SelectFile("Select config file", XmlFilter, defFile);
                if (string.IsNullOrEmpty(fName))
                    return;
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CkSearchResult>));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                SearchResults = (List<CkSearchResult>)reader.Deserialize(file);
                file.Close();
                LoadResults(SearchResults, OS);

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResults, line " + line + ": " + ex.Message);
            }

        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach(CkSearchResult cr in SearchResults)
            {
                cr.Select = chkSelectAll.Checked;
            }
            dataGridView1.Refresh();
            this.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            SelectedResults = SearchResults.OrderBy(x => x.Start).Where(Y => Y.Select).ToList();
            this.Close();
        }
    }
}
