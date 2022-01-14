using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmSearchTables : Form
    {
        public frmSearchTables()
        {
            InitializeComponent();
        }
        private BindingSource configBindingSource = new BindingSource();
        private static List<TableSearchConfig> tableSearchConfig = new List<TableSearchConfig>();
        public void LoadConfig()
        {
            configBindingSource.DataSource = null;
            configBindingSource.DataSource = tableSearchConfig;
            dataGridConfig.DataSource = null;
            dataGridConfig.DataSource = configBindingSource;
            Application.DoEvents();
            dataGridConfig.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            labelConfigFile.Text = Path.GetFileName(tableSearchFile);
        }

        private void SaveFile(string FileName)
        {
            try
            {
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSearchConfig>));
                    writer.Serialize(stream, tableSearchConfig);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile(tableSearchFile);
        }

        public void LoadFile(string FileName)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSearchConfig>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            tableSearchConfig = (List<TableSearchConfig>)reader.Deserialize(file);
            file.Close();
            tableSearchFile = FileName;
            LoadConfig();
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select XML file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            LoadFile(FileName);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            SaveFile(FileName);
            labelConfigFile.Text = Path.GetFileName(FileName);
            tableSearchFile = FileName;
        }
    }

}
