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
        private PcmFile PCM;
        private List<TableSeek> tSeeks;
        private List<SegmentSeek> sSeeks;
        private List<CVN> sCVN;
        private List<DtcSearchConfig> dtcSC;
        private List<PidSearchConfig> pidSC;
        private List<Units> units;
        private List<FileType> fileTypes;
        private List<DetectRule> detectrules;

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
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;            
            //dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
        }

        private void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

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

        public void LoadRules()
        {
            detectrules = new List<DetectRule>();
            for (int i=0;i<DetectRules.Count;i++)
            {
                DetectRule dr = DetectRules[i].ShallowCopy();
                detectrules.Add(dr);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = detectrules;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }

        public void LoadStockCVN()
        {
            this.Text = "Edit stock CVN list";
            sCVN = new List<CVN>();
            for (int i=0; i<StockCVN.Count;i++)
            {
                CVN cvn = StockCVN[i].ShallowCopy();
                sCVN.Add(cvn);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = sCVN;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }
        public void LoadDTCSearchConfig()
        {
            this.Text = "Edit DTC Search config";
            dtcSC = new List<DtcSearchConfig>();
            for (int i=0; i< dtcSearchConfigs.Count;i++)
            {
                DtcSearchConfig dsc = dtcSearchConfigs[i].ShallowCopy();
                dtcSC.Add(dsc);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = dtcSC;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Columns["ConditionalOffset"].ToolTipText = "Possible values:code,status,mil (Multiple values allowed)";
            UseComboBoxForEnums(dataGridView1);
        }
        public void LoadPIDSearchConfig()
        {
            this.Text = "Edit PID Search config";
            pidSC = new List<PidSearchConfig>();
            for (int i=0; i< pidSearchConfigs.Count; i++)
            {
                PidSearchConfig psc = pidSearchConfigs[i].ShallowCopy();
                pidSC.Add(psc);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = pidSC;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }
       
        public void LoadTableSeek(string fname)
        {
            fileName = fname;
            this.Text = "Edit Table Seek config";

            if (fname.Length > 0 &&  File.Exists(fname))
                tSeeks = TableSeek.loadtTableSeekFile(fname);
            else
                tSeeks = new List<TableSeek>();
            refreshTableSeek();
        }

        public void LoadSegmentSeek(string fname)
        {
            fileName = fname;
            this.Text = "Edit Segment Seek config";
            if (fname.Length > 0 &&  File.Exists(fname))
                sSeeks = SegmentSeek.loadSegmentSeekFile(fname);
            else
                sSeeks = new List<SegmentSeek>();
            refreshSegmentSeek();
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
            fileTypes = new List<FileType>();
            for (int i=0; i<fileTypeList.Count;i++)
            {
                FileType ft = fileTypeList[i].ShallowCopy();
                fileTypes.Add(ft);
            }
            labelHelp.Text = "Select ONE as default. Example: Description = BIN files, Extension = *.bin;*.dat";
            if (fileTypes.Count == 0)
            {
                FileType ft = new FileType();
                ft.Default = true;
                ft.Description = "BIN files";
                ft.Extension = "*.bin";
                fileTypes.Add(ft);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = fileTypes;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }
        public void LoadUnits()
        {
            this.Text = "Units";
            units = new List<Units>();
            for (int i=0; i<unitList.Count;i++)
            {
                Units u = unitList[i].ShallowCopy();
                units.Add(u);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = units;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }

        public void loadOBD2CodeList()
        {
            this.Text = "Edit OBD2 Codes";
            loadOBD2Codes();
            refreshOBD2Codes();
        }

        private void refreshdgrid()
        {
            object tmp = bindingSource.DataSource;
            bindingSource.DataSource = null;
            dataGridView1.DataSource = null;
            bindingSource.DataSource = tmp;
            dataGridView1.DataSource = bindingSource;
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
            bindingSource.DataSource = tSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;

            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            //dataGridView1.Columns["ConditionalOffset"].ToolTipText = "If set, and Opcode Address last 2 bytes > 0x5000, Offset = -10000";
            //dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            UseComboBoxForEnums(dataGridView1);
        }

        private void refreshSegmentSeek()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = sSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;

            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            //dataGridView1.Columns["ConditionalOffset"].ToolTipText = "If set, and Opcode Address last 2 bytes > 0x5000, Offset = -10000";
            //dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            UseComboBoxForEnums(dataGridView1);
        }

        private void saveThis(string fName)
        {
            try
            {
                dataGridView1.EndEdit();
                if (this.Text.Contains("CVN"))
                {
                    Logger("Saving file stockcvn.xml", false);
                    if (fName.Length == 0)
                        fName = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                        writer.Serialize(stream, sCVN);
                        stream.Close();
                    }
                    StockCVN = sCVN;
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("File Types"))
                {
                    Logger("Saving file filetypes.xml", false);
                    if (fName.Length == 0)
                        fName = Path.Combine(Application.StartupPath, "XML", "filetypes.xml");
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<FileType>));
                        writer.Serialize(stream, fileTypes);
                        stream.Close();
                    }
                    fileTypeList = fileTypes;
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("DTC"))
                {
                    Logger("Saving file DtcSearch.xml", false);
                    if (fName.Length == 0)
                        fName = Path.Combine(Application.StartupPath, "XML", "DtcSearch.xml");
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DtcSearchConfig>));
                        writer.Serialize(stream, dtcSC);
                        stream.Close();
                    }
                    dtcSearchConfigs = dtcSC;
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("PID"))
                {
                    Logger("Saving file PidSearch.xml", false);
                    if (fName.Length == 0)
                        fName = Path.Combine(Application.StartupPath, "XML", "PidSearch.xml");
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidSearchConfig>));
                        writer.Serialize(stream, pidSC);
                        stream.Close();
                    }
                    pidSearchConfigs = pidSC;
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("OBD2"))
                {
                    saveOBD2Codes();
                }
                else if (this.Text.ToLower().Contains("table seek"))
                {
                    if (fName.Length == 0)
                        fName = SelectSaveFile("XML (*.xml)|*.xml|All (*.*)|*.*","new-tableseek.xml");
                    if (fName.Length == 0)
                        return;
                    Logger("Saving file " + fName, false);
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSeek>));
                        writer.Serialize(stream, tSeeks);
                        stream.Close();
                    }
                    tableSeeks = tSeeks;
                    Logger(" [OK]");
                }
                else if (this.Text.ToLower().Contains("segment seek"))
                {
                    if (fName.Length == 0)
                        fName = SelectSaveFile("XML (*.xml)|*.xml|All (*.*)|*.*", "new-segmentseek.xml");
                    if (fName.Length == 0)
                        return;
                    Logger("Saving file " + fName, false);
                    using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentSeek>));
                        writer.Serialize(stream, sSeeks);
                        stream.Close();
                    }
                    segmentSeeks = sSeeks;
                    Logger(" [OK]");
                }
                else if (this.Text.Contains("Units"))
                {
                    if (fName.Length == 0)
                        fName = Path.Combine(Application.StartupPath, "Tuner", "units.xml");
                    Logger("Saving file " + fName, false);
                    using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Units>));
                        writer.Serialize(stream, units);
                        stream.Close();
                    }
                    unitList = units;
                    Logger(" [OK]");
                }
                else
                {
                    Logger("Saving file autodetect.xml", false);
                    if (fName.Length == 0)
                        fName = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
                    using (FileStream stream = new FileStream(fName, FileMode.Create))
                    {
                        System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                        writer.Serialize(stream, detectrules);
                        stream.Close();
                    }
                    DetectRules = detectrules;
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
            saveThis(fileName);
            this.Close();
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
                                ts.Offset = Convert.ToInt64(lineparts[i + 2]);
                            if (lineparts.Length >= i + 3 && lineparts[i + 3].Length > 0)
                                xtension = lineparts[i + 3];
                            // ts.Name += "*" + lineparts[i + 3];

                            for (int s = 0; s < tSeeks.Count; s++)
                            {
                                if (tSeeks[s].Name != null && tSeeks[s].Category != null && tSeeks[s].Name.ToLower() == lineparts[1].ToLower() && tSeeks[s].Category.ToLower() == ts.Category.ToLower())
                                {
                                    TableSeek tsNew = new TableSeek();
                                    tsNew = tSeeks[s].ShallowCopy();
                                    tsNew.SearchStr = ts.SearchStr;
                                    tsNew.UseHit = ts.UseHit;
                                    tsNew.Offset = ts.Offset;
                                    if (xtension.Length > 0)
                                        tsNew.Name += "*" + xtension;
                                    tSeeks.Add(tsNew);
                                    Debug.WriteLine(tsNew.Name);
                                    break;
                                }
                            }

                        }
                    }
                }
                sr.Close();

                for (int s = tSeeks.Count - 1; s >= 0; s--)
                {
                    if (tSeeks[s].Name.ToLower().StartsWith("ka_") || tSeeks[s].Name.ToLower().StartsWith("ke_") || tSeeks[s].Name.ToLower().StartsWith("kv_"))
                        tSeeks[s].Name = tSeeks[s].Name.Substring(3);
                    if (tSeeks[s].SearchStr.Length == 0)
                        tSeeks.RemoveAt(s);
                }
                bindingSource.DataSource = null;
                bindingSource.DataSource = tSeeks;
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
            if (this.Text.Contains("Table Seek") && starting)
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
            saveThis(fileName);
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
                        compareList = tSeeks.OrderBy(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = tSeeks.OrderByDescending(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                else if (this.Text.Contains("Autodetect"))
                {
                    List<DetectRule> compareList = new List<DetectRule>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = detectrules.OrderBy(x => typeof(DetectRule).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = detectrules.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                else if (this.Text.Contains("stock CVN"))
                {
                    List<CVN> compareList = new List<CVN>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = sCVN.OrderBy(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = sCVN.OrderByDescending(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareList;
                }
                else if (this.Text.Contains("DTC Search"))
                {
                    List<DtcSearchConfig> compareList = new List<DtcSearchConfig>();
                    if (strSortOrder == SortOrder.Ascending)
                        compareList = dtcSC.OrderBy(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareList = dtcSC.OrderByDescending(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
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

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectSaveFile("XML (*.xml)|*.xml|All (*.*)|*.*",fileName);
            if (fName.Length == 0)
                return;
            saveThis(fName);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            object myObj = dataGridView1.CurrentRow.DataBoundItem;
            fpe.loadTd(myObj);
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                refreshdgrid();
            }
        }
    }
}
