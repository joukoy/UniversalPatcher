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

namespace UniversalPatcher
{
    public partial class frmEditXML : Form
    {
        public frmEditXML()
        {
            InitializeComponent();
        }

        private BindingSource bindingSource = new BindingSource();

        string fileName = "";
        public void LoadRules()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = DetectRules;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }

        public void LoadStockCVN()
        {
            this.Text = "Edit stock CVN list";
            bindingSource.DataSource = null;
            bindingSource.DataSource = StockCVN;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }
        public void LoadDTCSearchConfig()
        {
            this.Text = "Edit DTC Search config";
            bindingSource.DataSource = null;
            bindingSource.DataSource = dtcSearchConfigs;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }
        public void LoadTableSeek(string fname)
        {
            fileName = fname;
            this.Text = "Edit Table Seek config";
            if (tableSeeks == null) tableSeeks = new List<TableSeek>();
            bindingSource.DataSource = null;
            bindingSource.DataSource = tableSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            dataGridView1.Columns["ConditionalOffset"].ToolTipText = "If set, and Opcode Address last 2 bytes > 0x5000, Offset = -10000";            
        }

        public void LoadTableData()
        {
            this.Text = "Table data";
            bindingSource.DataSource = null;
            bindingSource.DataSource = tableDatas;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            btnSave.Visible = false;
        }

        public void LoadFileTypes()
        {
            this.Text = "File Types";
            bindingSource.DataSource = null;
            bindingSource.DataSource = fileTypeList;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == this.dataGridView1.Columns["Rating"].Index) && e.Value != null)
            {
                DataGridViewCell cell =
                    this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (e.Value.Equals("DataType"))
                {
                    cell.ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (this.Text.Contains("CVN"))
                {
                    Logger("Saving file stockcvn.xml",false);
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
        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        private void btnSaveCSV_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i= 0;i< dataGridView1.Columns.Count; i++)
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

        private void frmEditXML_Load(object sender, EventArgs e)
        {

        }

        private void btnImportCSV_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            StreamReader sr = new StreamReader(FileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] lineparts = line.Split(';');
                if (lineparts.Length == 2)
                {
                    CVN C1 = new CVN();
                    C1.PN = lineparts[0];
                    C1.cvn = lineparts[1];
                    bool isinlist = false;
                    for (int i = 0; i < StockCVN.Count; i++)
                    {
                        if (StockCVN[i].PN == C1.PN)
                        {
                            isinlist= true;
                            if (StockCVN[i].cvn != C1.cvn)
                            {
                                //Update, is not correct CVN
                                Logger("Updating PN: " + C1.PN + " Old: " + StockCVN[i].cvn + ", new: " + C1.cvn);
                                C1.SegmentNr = StockCVN[i].SegmentNr;
                                C1.Ver = StockCVN[i].Ver;
                                C1.XmlFile = StockCVN[i].XmlFile;
                                StockCVN.RemoveAt(i);
                                StockCVN.Insert(i, C1);
                            }
                        }
                    }
                    if (!isinlist)
                        StockCVN.Add(C1);
                }
            }
            sr.Close();
            LoadStockCVN();
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == this.dataGridView1.Columns["DataType"].Index) )
            {
                DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                 cell.ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            }

        }
    }
}
