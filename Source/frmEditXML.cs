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

    }
}
