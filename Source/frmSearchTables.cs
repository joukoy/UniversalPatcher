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
    public partial class frmSearchTables : Form
    {
        public frmSearchTables()
        {
            InitializeComponent();
        }
        private BindingSource configBindingSource = new BindingSource();

        public void LoadConfig()
        {
            configBindingSource.DataSource = null;
            configBindingSource.DataSource = tablesearchconfig;
            dataGridConfig.DataSource = null;
            dataGridConfig.DataSource = configBindingSource;
            dataGridConfig.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            labelConfigFile.Text = Path.GetFileName(tableSearchFile);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
                if (FileName.Length == 0)
                    return;
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSearchConfig>));
                    writer.Serialize(stream, tablesearchconfig);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select XML file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSearchConfig>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            tablesearchconfig = (List<TableSearchConfig>)reader.Deserialize(file);
            file.Close();
            LoadConfig();
        }
    }
}
