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
    public partial class frmPatchSelector : Form
    {
        public frmPatchSelector()
        {
            InitializeComponent();
        }

        private class PatchFile
        {
            public string FileName { get; set; }
            public string Description { get; set; }
            public string CompatibleOS { get; set; }
            public string Platform { get; set; }
        }

        private List<PatchFile> patchFileList;
        public PcmFile basefile;
        public frmTuner tunerForm;
        private BindingSource bindingSource;

        private void frmPatchSelector_Load(object sender, EventArgs e)
        {
            bindingSource = new BindingSource();
            dataGridView1.DataSource = bindingSource;
            dataGridView1.CellContentDoubleClick += DataGridView1_CellContentDoubleClick;
            if (tunerForm != null)
                btnSelect.Text = "Apply selected patch";
        }

        private void DataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            selectPatch();
        }

        private void refreshFileList()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = patchFileList;
            Application.DoEvents();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            txtResult.AppendText(LogText);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        private void loadPatch(string fileName)
        {
            try
            {
                Logger("Loading file: " + fileName);
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                List<XmlPatch> newPatchList = (List<XmlPatch>)reader.Deserialize(file);
                file.Close();
                string CompOS = "";
                if (newPatchList.Count > 0)
                {
                    Logger("Description: " + newPatchList[0].Description);
                    string[] OsList = newPatchList[0].CompatibleOS.Split(',');
                    foreach (string OS in OsList)
                    {
                        if (CompOS != "")
                            CompOS += ",";
                        string[] Parts = OS.Split(':');
                        CompOS += Parts[0];
                    }
                    Logger("For OS: " + CompOS);
                }
                bool isCompatible = false;
                for (int x = 0; x < newPatchList.Count; x++)
                {
                    if (checkPatchCompatibility(newPatchList[x], basefile) < uint.MaxValue)
                    {
                        isCompatible = true;
                        break;
                    }
                }
                if (isCompatible || chkShowAll.Checked)
                {
                    Logger("Patch is compatible");
                    PatchFile pFile = new PatchFile();
                    pFile.CompatibleOS = CompOS;
                    pFile.Description = newPatchList[0].Description;
                    pFile.FileName = fileName;
                    pFile.Platform = newPatchList[0].XmlFile;
                    patchFileList.Add(pFile);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
        public void loadPatches()
        {
            patchFileList = new List<PatchFile>();

            string folder = Path.Combine(Application.StartupPath, "Patches");
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo file in Files)
            {
                loadPatch(file.FullName);
            }
            refreshFileList();
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            loadPatches();
        }

        private void selectPatch()
        {
            if (patchFileList == null || patchFileList.Count == 0)
                return;
            int row = dataGridView1.SelectedCells[0].RowIndex;
            string fName = dataGridView1.Rows[row].Cells["FileName"].Value.ToString();
            this.Hide();
            if (tunerForm == null)
                frmpatcher.LoadPatch(fName);
            else
                tunerForm.applyPatch(fName);
            this.Close();

        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            selectPatch();
        }


    }
}
