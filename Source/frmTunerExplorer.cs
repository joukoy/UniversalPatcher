using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;
namespace UniversalPatcher
{
    public partial class frmTunerExplorer : Form
    {
        public frmTunerExplorer()
        {
            InitializeComponent();
        }

        public PcmFile PCM;
        frmTuner tuner;
        frmTableTree tableTree;

        private void openTree()
        {
            if (tuner == null)
                tuner = new frmTuner(PCM);

            if (tableTree != null)
                tableTree.Dispose();

            tableTree = new frmTableTree();
            tableTree.TopLevel = false;
            tableTree.Dock = DockStyle.Fill;
            tableTree.tunerExplorer = this;
            splitContainer1.Panel1.Controls.Add(tableTree);
            tableTree.FormBorderStyle = FormBorderStyle.None;
            tableTree.loadTree(PCM.tableDatas, tuner);            
            tableTree.Show();
        }

        private void frmTunerExplorer_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.TunerExplorerWindowSize.Width > 0 || Properties.Settings.Default.TunerExplorerWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.TunerExplorerWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.TunerExplorerWindowLocation;
                    this.Size = Properties.Settings.Default.TunerExplorerWindowSize;
                }
                if (Properties.Settings.Default.TunerExplorerWindowSplitterDistance > 0)
                    splitContainer1.SplitterDistance = Properties.Settings.Default.TunerExplorerWindowSplitterDistance;
                if (Properties.Settings.Default.TunerExplorerWindowSplitter2Distance > 0)
                    splitContainer2.SplitterDistance = Properties.Settings.Default.TunerExplorerWindowSplitter2Distance;
            }
            this.FormClosing += FrmTunerExplorer_FormClosing;
            LogReceivers.Add(txtResult);
            openTree();
        }

        private void FrmTunerExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.TunerExplorerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.TunerExplorerWindowLocation = this.Location;
                    Properties.Settings.Default.TunerExplorerWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.TunerExplorerWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.TunerExplorerWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.TunerExplorerWindowSplitterDistance = splitContainer1.SplitterDistance;
                Properties.Settings.Default.TunerExplorerWindowSplitter2Distance = splitContainer2.SplitterDistance;
            }
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

        private void importTableSeek()
        {
            PCM.importSeekTables();
            Logger("OK");
        }

        private void importTinyTunerDB()
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDBtoTableData(PCM, PCM.tableDatas));

        }

        private void importDTC()
        {
            Logger("Importing DTC codes... ", false);
            bool haveDTC = false;
            for (int t = 0; t < PCM.tableDatas.Count; t++)
            {
                if (PCM.tableDatas[t].TableName == "DTC" || PCM.tableDatas[t].TableName == "DTC.Codes")
                {
                    haveDTC = true;
                    Logger(" DTC codes already defined");
                    break;
                }
            }
            if (!haveDTC)
            {
                TableData tdTmp = new TableData();
                tdTmp.importDTC(PCM, ref PCM.tableDatas);
                Logger(" [OK]");
            }
        }

        public void loadConfigforPCM(ref PcmFile newPCM)
        {
            string defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", newPCM.OS + ".xml");
            string compXml = "";
            if (File.Exists(defaultTunerFile))
            {
                long conFileSize = new FileInfo(defaultTunerFile).Length;
                if (conFileSize < 255)
                {
                    compXml = ReadTextFile(defaultTunerFile);
                    defaultTunerFile = Path.Combine(Application.StartupPath, "Tuner", compXml);
                    Logger("Using compatible file: " + compXml);
                }
            }
            if (File.Exists(defaultTunerFile))
            {
                Logger(newPCM.LoadTableList(defaultTunerFile));
                importDTC();
            }
            else
            {
                Logger("File not found: " + defaultTunerFile);
                importDTC();
                importTableSeek();
                if (newPCM.Segments.Count > 0 && newPCM.Segments[0].CS1Address.StartsWith("GM-V6"))
                    importTinyTunerDB();
            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) 
                return;
            PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
            PCM = newPCM;
            loadConfigforPCM(ref PCM);
            tuner.PCM = PCM;
            openTree();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            PCM.saveBin(PCM.FileName);
            Logger("Done.");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PCM == null || PCM.buf == null | PCM.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string fileName = SelectSaveFile("BIN files (*.bin)|*.bin|ALL files(*.*)|*.*", PCM.FileName);
                if (fileName.Length == 0)
                    return;

                Logger("Saving to file: " + fileName);
                PCM.saveBin(fileName);
                this.Text = "Tuner " + Path.GetFileName(fileName);
                Logger("Done.");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
