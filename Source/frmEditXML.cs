using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;
using UniversalPatcher.Properties;
using System.Diagnostics;

namespace UniversalPatcher
{
    public partial class frmEditXML : Form
    {
        public frmEditXML()
        {
            InitializeComponent();
        }

        private BindingSource bindingSource = new BindingSource();
        private bool starting = true;
        private string sortBy = "";
        private int sortIndex = 0;
        SortOrder strSortOrder = SortOrder.Ascending;

        string fileName = "";
        public void LoadRules()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = DetectRules;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }

        public void LoadStockCVN()
        {
            this.Text = "Edit stock CVN list";
            bindingSource.DataSource = null;
            bindingSource.DataSource = StockCVN;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }
        public void LoadDTCSearchConfig()
        {
            this.Text = "Edit DTC Search config";
            bindingSource.DataSource = null;
            bindingSource.DataSource = dtcSearchConfigs;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Columns["ConditionalOffset"].ToolTipText = "Possible values:code,status,mil (Multiple values allowed)";
            UseComboBoxForEnums(dataGridView1);
        }
        public void LoadTableSeek(string fname)
        {
            fileName = fname;
            this.Text = "Edit Table Seek config";
            if (tableSeeks == null) tableSeeks = new List<TableSeek>();
            refreshTableSeek();
        }

        public void LoadTableData()
        {
            this.Text = "Table data";
            bindingSource.DataSource = null;
            bindingSource.DataSource = tableViews;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            btnSave.Visible = false;
            dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            UseComboBoxForEnums(dataGridView1);
        }

        public void LoadFileTypes()
        {
            this.Text = "File Types";
            bindingSource.DataSource = null;
            bindingSource.DataSource = fileTypeList;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }
        public void LoadUnits()
        {
            this.Text = "Units";
            bindingSource.DataSource = null;
            bindingSource.DataSource = unitList;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }

        public void loadOBD2CodeList()
        {
            this.Text = "Edit OBD2 Codes";
            loadOBD2Codes();
            refreshOBD2Codes();
        }

        private void refreshOBD2Codes()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = OBD2Codes;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;

        }

        private void refreshTableSeek()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = tableSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;

            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            //dataGridView1.Columns["ConditionalOffset"].ToolTipText = "If set, and Opcode Address last 2 bytes > 0x5000, Offset = -10000";
            //dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            UseComboBoxForEnums(dataGridView1);
        }
        private void saveThis()
        {
            try
            {
                dataGridView1.EndEdit();
                if (this.Text.Contains("CVN"))
                {
                    Logger("Saving file stockcvn.xml", false);
                    string FileName = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                    using (FileStream stream = new FileStream(FileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                        writer.Serialize(stream, StockCVN);
                        stream.Close();
                    }
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("File Types"))
                {
                    Logger("Saving file filetypes.xml", false);
                    string FileName = Path.Combine(Application.StartupPath, "XML", "filetypes.xml");
                    using (FileStream stream = new FileStream(FileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<FileType>));
                        writer.Serialize(stream, fileTypeList);
                        stream.Close();
                    }
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("DTC"))
                {
                    Logger("Saving file DtcSearch.xml", false);
                    string FileName = Path.Combine(Application.StartupPath, "XML", "DtcSearch.xml");
                    using (FileStream stream = new FileStream(FileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DtcSearchConfig>));
                        writer.Serialize(stream, dtcSearchConfigs);
                        stream.Close();
                    }
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("OBD2"))
                {
                    saveOBD2Codes();
                }
                else if (this.Text.Contains("Seek"))
                {
                    Logger("Saving file " + fileName, false);
                    using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSeek>));
                        writer.Serialize(stream, tableSeeks);
                        stream.Close();
                    }
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("Units"))
                {
                    fileName = Path.Combine(Application.StartupPath, "Tuner", "units.xml");
                    Logger("Saving file " + fileName, false);
                    using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Units>));
                        writer.Serialize(stream, unitList);
                        stream.Close();
                    }
                    Logger(" [OK]");
                }
                else
                {
                    Logger("Saving file autodetect.xml", false);
                    string FileName = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
                    using (FileStream stream = new FileStream(FileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                        writer.Serialize(stream, DetectRules);
                        stream.Close();
                    }
                    Logger(" [OK]");

                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            saveThis();
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
        private void btnSaveCSV_Click(object sender, EventArgs e)
        {
            saveCSV();
        }

        private void frmEditXML_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {                
                if (Properties.Settings.Default.EditXMLWindowSize.Width > 0 || Properties.Settings.Default.EditXMLWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.EditXMLWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.EditXMLWindowLocation;
                    this.Size = Properties.Settings.Default.EditXMLWindowSize;
                }
            }

        }
        private void frmEditXML_FormClosing(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.EditXMLWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.EditXMLWindowLocation = this.Location;
                    Properties.Settings.Default.EditXMLWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.EditXMLWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.EditXMLWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.Save();
            }
        }

        private void importCSV()
        {
            //Example:
            //SPARK_ADVANCE;KA_PISTON_SLAP_SPARK_RETARD;00013D8E;81294;330;3C 0A 00 74 0B 20 7C @ @ @ @ 4E B9;2;;-05;80 FC 00 14 74 0B 20 7C @ @ @ @ 4E B9;1;;06-07
            //06-07 is "extension", add *extension to tablename
            try
            {
                string FileName = SelectFile("Select CSV file", "CSV files (*.csv)|*.csv|All files (*.*)|*.*");
                if (FileName.Length == 0)
                    return;
                StreamReader sr = new StreamReader(FileName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] lineparts = line.Split(';');
                    if (lineparts.Length > 5)
                    {
                        Debug.WriteLine(line);
                        for (int i = 5; i < lineparts.Length; i += 4)
                        {
                            TableSeek ts = new TableSeek();
                            ts.Category = lineparts[0];
                            ts.Name = lineparts[1];
                            ts.SearchStr = lineparts[i];
                            string xtension = "";
                            if (lineparts.Length >= i + 1)
                                ts.UseHit = lineparts[i + 1];
                            if (lineparts.Length >= i + 2 && lineparts[i + 2].Length > 0)
                                ts.Offset = Convert.ToInt32(lineparts[i + 2]);
                            if (lineparts.Length >= i + 3 && lineparts[i + 3].Length > 0)
                                xtension = lineparts[i + 3];
                            // ts.Name += "*" + lineparts[i + 3];

                            for (int s = 0; s < tableSeeks.Count; s++)
                            {
                                if (tableSeeks[s].Name != null && tableSeeks[s].Category != null && tableSeeks[s].Name.ToLower() == lineparts[1].ToLower() && tableSeeks[s].Category.ToLower() == ts.Category.ToLower())
                                {
                                    TableSeek tsNew = new TableSeek();
                                    tsNew = tableSeeks[s].ShallowCopy();
                                    tsNew.SearchStr = ts.SearchStr;
                                    tsNew.UseHit = ts.UseHit;
                                    tsNew.Offset = ts.Offset;
                                    if (xtension.Length > 0)
                                        tsNew.Name += "*" + xtension;
                                    tableSeeks.Add(tsNew);
                                    Debug.WriteLine(tsNew.Name);
                                    break;
                                }
                            }

                        }
                    }
                }
                sr.Close();

                for (int s = tableSeeks.Count - 1; s >= 0; s--)
                {
                    if (tableSeeks[s].Name.ToLower().StartsWith("ka_") || tableSeeks[s].Name.ToLower().StartsWith("ke_") || tableSeeks[s].Name.ToLower().StartsWith("kv_"))
                        tableSeeks[s].Name = tableSeeks[s].Name.Substring(3);
                    if (tableSeeks[s].SearchStr.Length == 0)
                        tableSeeks.RemoveAt(s);
                }
                bindingSource.DataSource = null;
                bindingSource.DataSource = tableSeeks;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = bindingSource;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            importCSV();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (this.Text.Contains("Seek") && starting)
            {
                dataGridView1.AutoResizeColumns();
                dataGridView1.Columns["SearchStr"].Width = 100;
                dataGridView1.Columns["RowHeaders"].Width = 100;
                dataGridView1.Columns["Colheaders"].Width = 100;
                starting = false;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveThis();
        }

        private void saveCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCSV();
        }

        private void importOBD2()
        {
            string FileName = SelectFile("Select CSV file", "CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            StreamReader sr = new StreamReader(FileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                int pos = line.IndexOf(' ');
                if (pos > -1)
                {
                    OBD2Code oc = new OBD2Code();
                    oc.Code = line.Substring(0,pos).Trim();
                    oc.Description = line.Substring(pos).Trim();
                    bool exist = false;
                    for (int i = 0; i < OBD2Codes.Count; i++)
                    {
                        if (OBD2Codes[i].Code == oc.Code)
                        {
                            exist = true;
                            OBD2Codes[i].Description = oc.Description;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        OBD2Codes.Add(oc);
                    }
                }
            }
            sr.Close();
            
        }
        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Text.Contains("OBD2"))
            {
                importOBD2();
            }
            else
            {
                importCSV();
            }
        }

        private void convertToDataTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tableSeeks.Count; i++)
            {
                TableSeek t = tableSeeks[i];
                t.DataType = convertToDataType(t.Bits / 8, t.Signed, t.Floating);
            }

            refreshTableSeek();
        }
        private void dataGridView1_Dataerror(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        public void filterTables()
        {
            try
            {
                if (this.Text.Contains("Table Seek"))
                {
                    List<TableSeek> compareList = new List<TableSeek>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = tableSeeks.OrderBy(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = tableSeeks.OrderByDescending(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                else if (this.Text.Contains("Autodetect"))
                {
                    List<DetectRule> compareList = new List<DetectRule>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = DetectRules.OrderBy(x => typeof(DetectRule).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = DetectRules.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                else if (this.Text.Contains("stock CVN"))
                {
                    List<CVN> compareList = new List<CVN>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = StockCVN.OrderBy(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = StockCVN.OrderByDescending(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                else if (this.Text.Contains("DTC Search"))
                {
                    List<DtcSearchConfig> compareList = new List<DtcSearchConfig>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = dtcSearchConfigs.OrderBy(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = dtcSearchConfigs.OrderByDescending(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
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

    }
}
