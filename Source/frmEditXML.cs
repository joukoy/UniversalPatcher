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
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void saveThis()
        {
            try
            {
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
        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
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

        }

        private void btnImportCSV_Click(object sender, EventArgs e)
        {
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
                        for (int i = 5; i < lineparts.Length; i += 4)
                        {
                            TableSeek ts = new TableSeek();
                            ts.Category = lineparts[0];
                            ts.Name = lineparts[1];
                            ts.SearchStr = lineparts[i];
                            if (lineparts.Length >= i + 1)
                                ts.UseHit = lineparts[i + 1];
                            if (lineparts.Length >= i + 2 && lineparts[i + 2].Length > 0)
                                ts.Offset = Convert.ToInt32(lineparts[i + 2]);
                            if (lineparts.Length >= i + 3 && lineparts[i + 3].Length > 0)
                                ts.Name += "_" + lineparts[i + 3];

                            for (int s = 0; s < tableSeeks.Count; s++)
                            {
                                if (tableSeeks[s].Name.ToLower() == lineparts[1].ToLower() && tableSeeks[s].Category.ToLower() == ts.Category.ToLower())
                                {
                                    if (tableSeeks[s].SearchStr.Length > 0)
                                    {
                                        if (tableSeeks[s].SearchStr != ts.SearchStr && tableSeeks[s].UseHit != ts.UseHit && tableSeeks[s].Offset != ts.Offset)
                                        {
                                            TableSeek tsNew = new TableSeek();
                                            tsNew.BitMask = tableSeeks[s].BitMask;
                                            tsNew.DataType = tableSeeks[s].DataType;
                                            //tsNew.Bits = tableSeeks[s].Bits;
                                            //tsNew.Floating = tableSeeks[s].Floating;
                                            //tsNew.Signed = tableSeeks[s].Signed;
                                            tsNew.ColHeaders = tableSeeks[s].ColHeaders;
                                            tsNew.Columns = tableSeeks[s].Columns;
                                            tsNew.ConditionalOffset = tableSeeks[s].ConditionalOffset;
                                            tsNew.Decimals = tableSeeks[s].Decimals;
                                            tsNew.Description = tableSeeks[s].Description;
                                            tsNew.Math = tableSeeks[s].Math;
                                            tsNew.Max = tableSeeks[s].Max;
                                            tsNew.Min = tableSeeks[s].Min;
                                            tsNew.OutputType = tableSeeks[s].OutputType;
                                            tsNew.Range = tableSeeks[s].Range;
                                            tsNew.RowHeaders = tableSeeks[s].RowHeaders;
                                            tsNew.RowMajor = tableSeeks[s].RowMajor;
                                            tsNew.Rows = tableSeeks[s].Rows;
                                            tsNew.SavingMath = tableSeeks[s].SavingMath;
                                            tsNew.Segments = tableSeeks[s].Segments;
                                            tsNew.Units = tableSeeks[s].Units;
                                            tsNew.ValidationSearchStr = tableSeeks[s].ValidationSearchStr;
                                            tsNew.Name = ts.Name;
                                            tsNew.SearchStr = ts.SearchStr;
                                            tsNew.UseHit = ts.UseHit;
                                            tsNew.Offset = ts.Offset;
                                            tableSeeks.Add(tsNew);
                                        }
                                    }
                                    else
                                    {
                                        tableSeeks[s].SearchStr = ts.SearchStr;
                                        tableSeeks[s].UseHit = ts.UseHit;
                                        tableSeeks[s].Offset = ts.Offset;
                                        tableSeeks[s].Name = ts.Name;
                                    }
                                    break;
                                }
                            }

                        }
                    }
                }
                sr.Close();

                for (int s = tableSeeks.Count - 1; s >= 0; s--)
                {
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

        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            importCSV();
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

    }
}
