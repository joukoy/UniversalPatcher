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
    public partial class frmExtractSegments : Form
    {
        public frmExtractSegments()
        {
            InitializeComponent();
        }

        private string SegmentFileName(string FnameStart, string Extension)
        {
            string FileName = FnameStart + Extension;
            FileName = FileName.Replace("?", "_");
            if (radioReplace.Checked)
                return FileName;

            if (!Directory.Exists(Path.GetDirectoryName(FnameStart)))
                Directory.CreateDirectory(Path.GetDirectoryName(FnameStart));

            if (!File.Exists(FileName))
            {
                return FileName;
            }

            if (radioSkip.Checked)
            {
                Logger("Skipping duplicate file: " + FileName);
                return "";
            }

            //radioRename checked
            uint Fnr = 0;
            while (File.Exists(FileName))
            {
                Fnr++;
                FileName = FnameStart + "(" + Fnr.ToString() + ")" + Extension;
            }
            return FileName;
        }

        private void ExtractSegments(string FileName)
        {
            PcmFile PCM = new PcmFile(FileName);
            GetFileInfo(FileName, ref PCM, true, true);
            string Descr = FileName.Replace(".bin", "");
            if (PCM.segmentinfos == null)
            {
                Logger("no segments defined");
                return;
            }
            try
            {
                for (int s = 0; s < PCM.segmentinfos.Length; s++)
                {
                    if (labelCustomFolder.Text.Length > 1)
                    {
                        string FnameStart = Path.Combine(labelCustomFolder.Text, PCM.segmentinfos[s].PN.PadLeft(8, '0'));
                        FileName = SegmentFileName(FnameStart, ".bin");
                    }
                    else
                    {
                        string FnameStart = Path.Combine(Application.StartupPath, "Segments", PCM.OS, PCM.segmentinfos[s].SegNr, PCM.segmentinfos[s].Name + "-" + PCM.segmentinfos[s].PN + PCM.segmentinfos[s].Ver);
                        FileName = SegmentFileName(FnameStart, ".binsegment");
                    }
                    if (FileName.Length > 0)
                    {
                        Logger("Writing " + PCM.segmentinfos[s].Name + " to file: " + FileName + ", size: " + PCM.segmentinfos[s].Size + " (0x" + PCM.segmentinfos[s].Size + ")");
                        WriteSegmentToFile(FileName, PCM.binfile[s].SegmentBlocks, PCM.buf);
                        StreamWriter sw = new StreamWriter(FileName + ".txt");
                        sw.WriteLine(Descr);
                        sw.Close();
                        Logger("[OK]");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnSelectfiles_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            //frmF.MdiParent = FormMain;
            frmF.btnOK.Text = "Extract!"; 
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    ExtractSegments(FileName);
                }
            }

        }

        private void btnCustomFolder_Click(object sender, EventArgs e)
        {
            string Folder = SelectFolder("Select destination folder");
            if (Folder.Length == 0)
                return;
            labelCustomFolder.Text = Folder;
        }
    }
}
