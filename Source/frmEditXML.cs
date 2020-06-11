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

namespace UniversalPatcher
{
    public partial class frmEditXML : Form
    {
        public frmEditXML()
        {
            InitializeComponent();
        }

        private BindingSource bindingSource = new BindingSource();

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

 
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
    }
}
