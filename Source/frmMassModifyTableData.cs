using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmMassModifyTableData : Form
    {
        public frmMassModifyTableData()
        {
            InitializeComponent();
        }

        public class TunerFile
        {
            public List<TableData> tableDatas = new List<TableData>();
            public string fileName { get; set; }
        }

        public class MassModProperties
        {
            public string TableName { get; set; }
            public string Category { get; set; }
            public string TableDescription { get; set; }
            public string ExtraDescription { get; set; }
            public string ColumnHeaders { get; set; }
            public string RowHeaders { get; set; }
            public string Units { get; set; }
            public string Values { get; set; }
        }
        private List<TunerFile> tunerFiles = new List<TunerFile>();
        private List<string> modifiedFiles = new List<string>();
        private List<string> tableRows = new List<string>();
        private List<MassModProperties> displayDatas = new List<MassModProperties>();
        private BindingSource bindingSource = new BindingSource();
        SortOrder strSortOrder = SortOrder.Ascending;
        private string sortBy = "TableName";
        private int sortIndex = 0;

        private void frmMassModifyTableData_Load(object sender, EventArgs e)
        {
        }

        public void loadData()
        {
            dataGridView2.Columns.Add("TableName", "TableName");
            dataGridView2.Columns.Add("Property", "Property");
            dataGridView2.Columns.Add("OldValue", "OldValue");
            dataGridView2.Columns.Add("NewValue", "NewValue");

            MassModProperties tmpMmp = new MassModProperties();
            foreach (var prop in tmpMmp.GetType().GetProperties())
            {
                //Add to filter by-combo
                comboFilterBy.Items.Add(prop.Name);
            }
            comboFilterBy.Text = "TableName";

            string folder = Path.Combine(Application.StartupPath, "Tuner");
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.xml", SearchOption.AllDirectories);

            foreach (FileInfo file in Files)
            {
                long conFileSize = new FileInfo(file.FullName).Length;
                if (conFileSize < 255 || file.Name.ToLower() == "units.xml")
                    continue;
                TunerFile tf = new TunerFile();
                tf.tableDatas = loadTableDataFile(file.FullName);
                tf.fileName = file.FullName;
                tunerFiles.Add(tf);
                addTableListTodgrid(tf.tableDatas);
            }
            dataGridView1.DataSource = bindingSource;
            filterData();
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            Application.DoEvents();
            
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            Logger("Files loaded");
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.FormattedValue == dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                return;
            int row = dataGridView2.Rows.Add();
            dataGridView2.Rows[row].Cells["Property"].Value = dataGridView1.Columns[e.ColumnIndex].Name;
            dataGridView2.Rows[row].Cells["OldValue"].Value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            dataGridView2.Rows[row].Cells["TableName"].Value = dataGridView1.Rows[e.RowIndex].Cells["TableName"].Value;
            dataGridView2.Rows[row].Cells["NewValue"].Value = e.FormattedValue;

        }


        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //saveGridLayout(); //Save before reorder!
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = getSortOrder(sortIndex);
                filterData();
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

        private void filterData()
        {
            List<MassModProperties> compareList = new List<MassModProperties>();
            if (strSortOrder == SortOrder.Ascending)
                compareList = displayDatas.OrderBy(x => typeof(MassModProperties).GetProperty(sortBy).GetValue(x, null)).ToList();
            else
                compareList = displayDatas.OrderByDescending(x => typeof(MassModProperties).GetProperty(sortBy).GetValue(x, null)).ToList();

            var results = compareList.Where(t => t.TableName != ""); //How should I define empty variable??

            if (txtSearch.Text.Length > 0)
            {
                string newStr = txtSearch.Text.Replace("OR", "|");
                if (newStr.Contains("|"))
                {
                    string[] orStr = newStr.Split('|');
                    if (orStr.Length == 2)
                        results = results.Where(t => typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[0].Trim()) ||
                        typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[1].Trim()));
                    if (orStr.Length == 3)
                        results = results.Where(t => typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[0].Trim()) ||
                        typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[1].Trim()) ||
                        typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[2].Trim()));
                    if (orStr.Length == 4)
                        results = results.Where(t => typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[0].Trim()) ||
                        typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[1].Trim()) ||
                        typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[2].Trim()) ||
                        typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(orStr[3].Trim()));
                }
                else
                {
                    newStr = txtSearch.Text.Replace("AND", "&");
                    string[] andStr = newStr.Split('&');
                    foreach (string sStr in andStr)
                        results = results.Where(t => typeof(MassModProperties).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(sStr.Trim()));
                }
            }
            bindingSource.DataSource = results;
            dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
        }
        private void addTableListTodgrid(List<TableData> tdList)
        {
            for (int i=0; i< tdList.Count; i++)
            {
                MassModProperties mmp = new MassModProperties();
                mmp.Category = tdList[i].Category;
                mmp.ColumnHeaders = tdList[i].ColumnHeaders;
                mmp.ExtraDescription = tdList[i].ExtraDescription;
                mmp.RowHeaders = tdList[i].RowHeaders;
                mmp.TableDescription = tdList[i].TableDescription;
                mmp.TableName = tdList[i].TableName;
                mmp.Units = tdList[i].Units;
                mmp.Values = tdList[i].Values; 
                //string ser = upatcher.SerializeObject<MassModProperties>(mmp);
                string ser = "";
                foreach (var prop in mmp.GetType().GetProperties())
                {
                    ser += prop.GetValue(mmp,null);
                }

                if (!tableRows.Contains(ser))
                {
                    tableRows.Add(ser);
                    displayDatas.Add(mmp);
                }
            }
        }

        private void addToGrid(string Property, string oldVal, string newVal)
        {
            int row = dataGridView2.Rows.Add();
            dataGridView2.Rows[row].Cells["Property"].Value = Property;
            dataGridView2.Rows[row].Cells["OldValue"].Value = oldVal;
            dataGridView2.Rows[row].Cells["NewValue"].Value = newVal;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            filterData();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            dataGridView2.EndEdit();
            for (int row = 0; row < dataGridView2.Rows.Count ; row++)
            {
                if (dataGridView2.Rows[row].Cells["TableName"].Value == null)
                    break;
                string tableName = dataGridView2.Rows[row].Cells["TableName"].Value.ToString();
                string Property = dataGridView2.Rows[row].Cells["Property"].Value.ToString();
                string oldVal = dataGridView2.Rows[row].Cells["OldValue"].Value.ToString();
                string newVal = dataGridView2.Rows[row].Cells["NewValue"].Value.ToString();
                for (int t = 0; t < tunerFiles.Count; t++ )
                {
                    for (int i=0; i< tunerFiles[t].tableDatas.Count; i++)
                    {
                        if (tunerFiles[t].tableDatas[i].TableName == tableName)
                        {
                            Logger("File: " + tunerFiles[t].fileName);
                            Logger("Modifying table: " + tableName + ", " + Property + ": " + oldVal + " => " + newVal);
                            if (!modifiedFiles.Contains(tunerFiles[t].fileName))
                                modifiedFiles.Add(tunerFiles[t].fileName);

                            Type type = tunerFiles[t].tableDatas[i].GetType();
                            PropertyInfo prop = type.GetProperty(Property);
                            prop.SetValue(tunerFiles[t].tableDatas[i], newVal, null);
                        }
                    }
                }
            }
            dataGridView2.Rows.Clear();
            Logger("Done");

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int modF = 0; modF < modifiedFiles.Count; modF++)
            {
                Logger("Saving file: " + modifiedFiles[modF], false);
                for (int tunerF=0; tunerF < tunerFiles.Count; tunerF++)
                {
                    if (tunerFiles[tunerF].fileName == modifiedFiles[modF])
                    {
                        using (FileStream stream = new FileStream(tunerFiles[tunerF].fileName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                            writer.Serialize(stream, tunerFiles[tunerF].tableDatas);
                            stream.Close();
                        }
                        Logger(" [OK]");
                        break;
                    }
                }
            }
            Logger("Files saved");
        }
        private void Logger(string LogText, Boolean NewLine = true)
        {
            try
            {
                txtResult.AppendText(LogText);
                if (NewLine)
                    txtResult.AppendText(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }
        }
        private void LoggerBold(string LogText, Boolean NewLine = true)
        {
            try
            {
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
                txtResult.AppendText(LogText);
                txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
                if (NewLine)
                    txtResult.AppendText(Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
            }
        }
        private List<TableData> loadTableDataFile(string fName)
        {
            List<TableData> tmpTableDatas = new List<TableData>();
            try
            {
                if (File.Exists(fName))
                {
                    Logger("Loading " + fName + "...", false);
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    System.IO.StreamReader file = new System.IO.StreamReader(fName);
                    tmpTableDatas = (List<TableData>)reader.Deserialize(file);
                    file.Close();
                    Logger(" [OK]");
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return tmpTableDatas;
        }

    }
}
