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
            public string fullFileName;
            public string FileName  {  get { return Path.GetFileName(fullFileName); }}
            public string Description { get; set; }
            public string CompatibleOS { get; set; }
            public string Platform { get; set; }
        }

        private List<PatchFile> patchFileList;
        public PcmFile basefile;
        public FrmTuner tunerForm;
        private BindingSource bindingSource;

        private void frmPatchSelector_Load(object sender, EventArgs e)
        {
            bindingSource = new BindingSource();
            dataGridView1.DataSource = bindingSource;
            dataGridView1.CellContentDoubleClick += DataGridView1_CellContentDoubleClick;
            if (tunerForm != null)
                btnSelect.Text = "Apply selected patch";
            LogReceivers.Add(txtResult);
        }

        private void DataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPatch();
        }

        private void RefreshFileList()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = patchFileList;
            Application.DoEvents();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }

        private void LoadPatch(string fileName)
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
                    if (!newPatchList[0].CompatibleOS.ToLower().StartsWith("table:") && !newPatchList[0].CompatibleOS.ToLower().StartsWith("search:"))
                        Logger("For OS: " + CompOS);
                }
                bool isCompatible = false;
                for (int x = 0; x < newPatchList.Count; x++)
                {
                    if (CheckPatchCompatibility(newPatchList[x], basefile) < uint.MaxValue)
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
                    pFile.fullFileName = fileName;
                    pFile.Platform = newPatchList[0].XmlFile;
                    patchFileList.Add(pFile);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void LoadPatches()
        {
            patchFileList = new List<PatchFile>();

            string folder = Path.Combine(Application.StartupPath, "Patches");
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo file in Files)
            {
                LoadPatch(file.FullName);
            }
            RefreshFileList();
        }

        public void ConvertToXmlPatch(TableData td, PcmFile PCM)    //Not in use??
        {
            patchFileList = new List<PatchFile>();
            PatchFile pFile = new PatchFile();
            pFile.CompatibleOS = td.OS;
            pFile.Description = td.TableDescription;
            pFile.fullFileName = td.TableName;
            pFile.Platform = PCM.configFile ;
            patchFileList.Add(pFile);
            RefreshFileList();
        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadPatches();
        }

        private void SelectPatch()
        {
            if (patchFileList == null || patchFileList.Count == 0)
                return;
            int row = dataGridView1.SelectedCells[0].RowIndex;
            PatchFile pFile = (PatchFile)dataGridView1.Rows[row].DataBoundItem;
            string fName = pFile.fullFileName;
            if (tunerForm == null)
            {
                this.Hide();
                frmpatcher.LoadPatch(fName);
                this.Close();
            }
            else
            {
                tunerForm.ApplyPatch(fName);
            }

        }
        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectPatch();
        }
    }
}
