using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using static upatcher;

namespace UniversalPatcher
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private frmLog frmDebug;
        private void FrmMain_Load(object sender, EventArgs e)
        {
            FormMain = this;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
            {
                Logger(args[1]);
                frmSegmenList frmSL = new frmSegmenList();
                frmSL.LoadFile(args[1]);
            }

            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Patches")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Patches"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "XML")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "XML"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Segments")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Segments"));


            if (Properties.Settings.Default.LastXMLfolder == "")
                Properties.Settings.Default.LastXMLfolder = Path.Combine(Application.StartupPath, "XML");
            if (Properties.Settings.Default.LastPATCHfolder == "")
                Properties.Settings.Default.LastPATCHfolder = Path.Combine(Application.StartupPath, "Patches");

            DetectRules = new List<DetectRule>();
            string AutoDetectFile = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
            if (File.Exists(AutoDetectFile))
            {
                Debug.WriteLine("Loading autodetect.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                System.IO.StreamReader file = new System.IO.StreamReader(AutoDetectFile);
                DetectRules = (List<DetectRule>)reader.Deserialize(file);
                file.Close();
            }

            StockCVN = new List<CVN>();
            string StockCVNFile = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
            if (File.Exists(StockCVNFile))
            {
                Debug.WriteLine("Loading stockcvn.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                System.IO.StreamReader file = new System.IO.StreamReader(StockCVNFile);
                StockCVN = (List<CVN>)reader.Deserialize(file);
                file.Close();
            }
            ShowLog();
        }

        private void ShowLog()
        {
            if (FormLog == null)
                FormLog = new frmLog();
            FormLog.MdiParent = this;
            FormLog.Location = new Point(0, (this.Height - FormLog.Height - 80));
            FormLog.Show();
        }
        private void ShowCVN()
        {
            FormCVN = new frmCVN();
            FormCVN.MdiParent = this;
            //frmCvn.Location = new Point(0, (this.Height - frmL.Height));
            FormCVN.Show();
        }

        private void ShowFileInfo()
        {
            FormFinfo = new frmFinfo();
            FormFinfo.MdiParent = this;
            //frmFi.Location = new Point(0, (this.Height - frmFi.Height));
            FormFinfo.Show();
            FormFinfo.RefreshFileInfo();
        }

        private void ShowPatchEditor()
        {
            FormPatcheditor = new frmPatcheditor();
            FormPatcheditor.MdiParent = this;
            //frmFi.Location = new Point(0, (this.Height - frmFi.Height));
            FormPatcheditor.Show();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowLog();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmDebug == null)
            {
                frmDebug = new frmLog();
                frmDebug.MdiParent = this;
                frmDebug.StartDebug();
                frmDebug.Show();
            }
        }

        private void createPatchByCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCompare frmC = new frmCompare();
            frmC.MdiParent = this;
            frmC.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.MdiParent = this;
            aboutBox.Show();
        }

        private void segmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSegmenList frmSL = new frmSegmenList();
            frmSL.InitMe();
            frmSL.MdiParent = this;
            frmSL.Show();
        }

        private void autodetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAutodetect frmAD = new frmAutodetect();
            frmAD.MdiParent = this;
            frmAD.Show();
            frmAD.InitMe();
        }

        private void stockCVNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML frmE = new frmEditXML();
            frmE.MdiParent = this;
            frmE.LoadStockCVN();
            frmE.Show();
        }

        private void getFileInfoFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Fldr = SelectFolder("Select folder");
            if (Fldr.Length == 0)
                return;
            DirectoryInfo d = new DirectoryInfo(Fldr);
            FileInfo[] Files = d.GetFiles("*.bin");
            foreach (FileInfo file in Files)
            {
                PcmFile binfile = new PcmFile(file.FullName);
                GetFileInfo(file.FullName, ref binfile, true, true);
            }

        }

        private void fixChecksumOfFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    FixFileChecksum(FileName);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void cVNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowCVN();
        }

        private void fileInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFileInfo();
        }

        private void patchEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPatchEditor();
        }

        private void swapSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile();
            if (fileName.Length == 0)
                return;
            frmSwapSegmentList frmSw = new frmSwapSegmentList();
            frmSw.LoadSegmentList(fileName);
            frmSw.MdiParent = this;
            frmSw.Show();
        }

        private void applyPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmApplyPatch formA = new frmApplyPatch();
            formA.MdiParent = this;
            formA.Show();
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {

        }

        private void extractTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile();
            if (fileName.Length == 0)
                return;
            frmExtractTable frmE = new frmExtractTable();
            frmE.MdiParent = this;
            frmE.initMe(fileName);
            frmE.Show();
        }

        private void extractSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmExtractSegments frmE = new frmExtractSegments();
            frmE.MdiParent = this;
            frmE.Show();
        }
    }
}
