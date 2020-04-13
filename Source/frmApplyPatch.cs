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
    public partial class frmApplyPatch : Form
    {
        public frmApplyPatch()
        {
            InitializeComponent();
        }

        private PcmFile PCM;
        private void btnBinFile_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile();
            if (fileName.Length == 0)
                return;
            labelBinFile.Text = fileName;
            PCM = new PcmFile(fileName);
            GetFileInfo(fileName, ref PCM, false, true);

        }

        private void LoadPatch()
        {
            if (FormPatcheditor == null)
            {
                FormPatcheditor = new frmPatcheditor();
                FormPatcheditor.MdiParent = FormMain;
                FormPatcheditor.Show();
            }
            if (labelPatchfile.Text != FormPatcheditor.labelPatchname.Text)
                FormPatcheditor.LoadPatch(labelPatchfile.Text);
            this.BringToFront();
        }
        private void btnPatchFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select patch file", "XML patch files (*.xmlpatch)|*.xmlpatch|PATCH files (*.patch)|*.patch|ALL files(*.*)|*.*");
            if (FileName.Length == 0)
                return;
            labelPatchfile.Text = FileName;
            LoadPatch();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            LoadPatch();
            ApplyXMLPatch(ref PCM);
            FixCheckSums(ref PCM);
        }

        private void btnSaveBIN_Click(object sender, EventArgs e)
        {
            string fileName = SelectSaveFile();
            if (fileName.Length == 0)
                return;
            Logger("Saving to file: " + fileName);
            WriteBinToFile(fileName, PCM.buf);
            Logger("OK");

        }
    }
}
