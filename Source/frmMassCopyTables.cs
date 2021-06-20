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
    public partial class frmMassCopyTables : Form
    {
        public frmMassCopyTables()
        {
            InitializeComponent();
        }


        private class SelectPCM
        {
            public SelectPCM()
            {
                Selected = true;
            }
            public bool Selected { get; set; }
            public PcmFile pcmFile { get; set; }
            public string FileName
            {
                get
                {
                    return pcmFile.FileName;
                }
            }
        }

        public PcmFile PCM;
        public List<int> tableIds;
        private List<SelectPCM> pcmList;
        private BindingSource bindingSource;

        private void frmMassCopyTables_Load(object sender, EventArgs e)
        {
            bindingSource = new BindingSource();
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.MassCopyTableWindowSize.Width > 0 || Properties.Settings.Default.MassCopyTableWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.MassCopyTableWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.MassCopyTableWindowLocation;
                    this.Size = Properties.Settings.Default.MassCopyTableWindowSize;
                }
                if (Properties.Settings.Default.MassCopyTableWindowSize.Width > 0 || Properties.Settings.Default.MassCopyTableWindowSize.Height > 0)
                {
                    this.splitContainer1.SplitterDistance = Properties.Settings.Default.MassCopyTableSplitHeight;
                }
            }
            txtFnameExtension.Text = Properties.Settings.Default.MassCopyTableFilenameExtra;
            this.FormClosing += FrmMassCopyTables_FormClosing;
        }

        private void FrmMassCopyTables_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.MassCopyTableWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.MassCopyTableWindowLocation = this.Location;
                    Properties.Settings.Default.MassCopyTableWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.MassCopyTableWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.MassCopyTableWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.MassCopyTableSplitHeight = this.splitContainer1.SplitterDistance;
            }
            Properties.Settings.Default.MassCopyTableFilenameExtra = txtFnameExtension.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int selCount = 0;
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    selCount++;
                    string fileName = pcmList[i].pcmFile.FileName;
                    SelectPCM sPCM = pcmList[i];
                    LoggerBold(fileName);
                    searchTargetTables(sPCM.pcmFile, tableIds, true);                    
                }
            }

            if (selCount == 0)
            {
                Logger("No files selected");
            }
            else
            {
                refreshPcmList();
                LoggerBold("Done, You can now save files");
            }
        }

        private void refreshPcmList()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = pcmList;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
            dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
            Application.DoEvents();
            
            dataGridView1.Columns["pcmFile"].Visible = false;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            //dataGridView1.Columns[0].Width = 50;
            //dataGridView1.Columns[1].Width = 1000;
        }

        private void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0 && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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



        private void copyTableData(TableData srcTd, TableData dstTd, ref PcmFile dstPCM)
        {
            frmTableEditor srcTE = new frmTableEditor();
            srcTE.prepareTable(PCM, srcTd, null,"A");
            //srcTE.loadTable();
            frmTableEditor dstTE = new frmTableEditor();
            dstTE.prepareTable(dstPCM, dstTd, null,"A");
            //dstTE.loadTable();

            for (int cell = 0; cell < srcTE.compareFiles[0].tableInfos[0].tableCells.Count;cell++)
            {
                TableCell srcTc = srcTE.compareFiles[0].tableInfos[0].tableCells[cell];
                TableCell dstTc = dstTE.compareFiles[0].tableInfos[0].tableCells[cell];
                dstTc.saveValue(Convert.ToDouble(srcTc.lastValue));
            }

            dstTE.saveTable(false);
            srcTE.Dispose();
            dstTE.Dispose();
        }

        private void searchTargetTables(PcmFile dstPCM, List<int> tableIds, bool execNow)
        {
            for (int x = 0; x < tableIds.Count; x++)
            {
                TableData sourceTd = PCM.tableDatas[tableIds[x]];
                int targetId = findTableDataId(sourceTd, dstPCM.tableDatas);
                if (targetId < 0)
                {
                    Logger("Table missing: " + sourceTd.TableName);
                }
                else
                {
                    TableData dstTd = dstPCM.tableDatas[targetId];
                    if (sourceTd.Rows != dstTd.Rows || sourceTd.Columns != dstTd.Columns || sourceTd.RowMajor != dstTd.RowMajor)
                    {
                        Logger("Table size not match: " + sourceTd.TableName);
                    }
                    else
                    {
                        if (execNow)
                        {
                            Logger("Copying table: " + sourceTd.TableName + "...", false);
                            copyTableData(sourceTd, dstTd, ref dstPCM);
                            Logger(" [OK]");
                        }
                        else
                        {
                            Logger("Table found: " + sourceTd.TableName);
                        }
                    }
                }
            }
        }

        public void startTableCopy()
        {
            try 
            { 
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Select files";
                frmF.Text = "Select target for tables";
                frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
                if (frmF.ShowDialog(this) == DialogResult.OK)
                {
                    pcmList = new List<SelectPCM>();
                    for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                    {
                        string fileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                        PcmFile newPCM = new PcmFile(fileName, true, "");
                        LoggerBold(fileName);
                        newPCM.loadTunerConfig();
                        if (PCM.seekTablesImported && !newPCM.seekTablesImported)
                            newPCM.importSeekTables();
                        SelectPCM sPCM = new SelectPCM();
                        sPCM.pcmFile = newPCM;
                        searchTargetTables(sPCM.pcmFile, tableIds, false);
                        pcmList.Add(sPCM);
                    }
                    LoggerBold("Select destination files and Press Apply to copy tables");
                    refreshPcmList();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }


        private int countSelections()
        {
            int selCount = 0;
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                    selCount++;
            }
            return selCount;
        }

        private string generateFname(string origFname, string fPath="")
        {
            string retVal = origFname;
            if (txtFnameExtension.Text.Length > 0)
            {
                string origFnameWithouthExt = Path.GetFileNameWithoutExtension(origFname);
                if (fPath.Length == 0)
                    fPath = Path.GetDirectoryName(origFname);
                string ext = Path.GetExtension(origFname);
                retVal = Path.Combine(fPath, origFnameWithouthExt + txtFnameExtension.Text + ext);
            }
            return retVal;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (countSelections() == 0)
            {
                Logger("No files selected");
                return;
            }
            for (int i=0; i< pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    string origFname = pcmList[i].pcmFile.FileName;
                    string newFname = generateFname(origFname);
                    Logger("Saving file: " + newFname, false);
                    pcmList[i].pcmFile.saveBin(newFname);
                    Logger(" [OK]");
                }
            }
            Logger("Done");
        }

        private void btnSaveTo_Click(object sender, EventArgs e)
        {
            if (countSelections() == 0)
            {
                Logger("No files selected");
                return;
            }

            string savePath = SelectFolder("Select target folder");
            if (savePath.Length == 0)
                return;

            for (int i=0; i< pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    string origFName = Path.GetFileName(pcmList[i].pcmFile.FileName);
                    string newFname = generateFname(origFName, savePath);
                    Logger("Saving file: " + newFname, false);
                    pcmList[i].pcmFile.saveBin(newFname);
                    Logger(" [OK]");
                }
            }
            Logger("Done");
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (countSelections() == 0)
            {
                Logger("No files selected");
                return;
            }
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    string origFName = Path.GetFileName(pcmList[i].pcmFile.FileName);
                    string newFname = SelectSaveFile("BIN files (*.bin)|*.bin", origFName);
                    if (newFname.Length > 0)
                    {
                        Logger("Saving file: " + newFname, false);
                        pcmList[i].pcmFile.saveBin(newFname);
                        Logger(" [OK]");
                    }
                }
            }
            Logger("Done");
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < pcmList.Count; i++)
                pcmList[i].Selected = true;
            refreshPcmList();
        }

        private void unSelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < pcmList.Count; i++)
                pcmList[i].Selected = false;
            refreshPcmList();
        }

        private void btnTuner_Click(object sender, EventArgs e)
        {
            if (countSelections() == 0)
            {
                Logger("No files selected");
                return;
            }
            frmTuner frmT = new frmTuner(null);
            frmT.Show();
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    PcmFile newPCM = pcmList[i].pcmFile;
                    frmT.addtoCurrentFileMenu(newPCM);
                    frmT.PCM = newPCM;
                    frmT.loadConfigforPCM(ref newPCM);
                }
            }
            frmT.selectPCM();

        }
    }
}
