﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static UniversalPatcher.ExtensionMethods;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmMassCopyTables : Form
    {
        public frmMassCopyTables()
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
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
        public List<TableData> tableTds;
        private List<SelectPCM> pcmList;
        private BindingSource bindingSource;

        private void frmMassCopyTables_Load(object sender, EventArgs e)
        {
            bindingSource = new BindingSource();
            if (AppSettings.MainWindowPersistence)
            {
                if (AppSettings.MassCopyTableWindowSize.Width > 0 || AppSettings.MassCopyTableWindowSize.Height > 0)
                {
                    this.WindowState = AppSettings.MassCopyTableWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = AppSettings.MassCopyTableWindowLocation;
                    this.Size = AppSettings.MassCopyTableWindowSize;
                }
                if (AppSettings.MassCopyTableWindowSize.Width > 0 || AppSettings.MassCopyTableWindowSize.Height > 0)
                {
                    this.splitContainer1.SplitterDistance = AppSettings.MassCopyTableSplitHeight;
                }
            }
            txtFnameExtension.Text = AppSettings.MassCopyTableFilenameExtra;
            this.FormClosing += FrmMassCopyTables_FormClosing;
        }

        private void FrmMassCopyTables_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                AppSettings.MassCopyTableWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    AppSettings.MassCopyTableWindowLocation = this.Location;
                    AppSettings.MassCopyTableWindowSize = this.Size;
                }
                else
                {
                    AppSettings.MassCopyTableWindowLocation = this.RestoreBounds.Location;
                    AppSettings.MassCopyTableWindowSize = this.RestoreBounds.Size;
                }
                AppSettings.MassCopyTableSplitHeight = this.splitContainer1.SplitterDistance;
            }
            AppSettings.MassCopyTableFilenameExtra = txtFnameExtension.Text;
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
                    SearchTargetTables(sPCM.pcmFile, tableTds, true);                    
                }
            }

            if (selCount == 0)
            {
                Logger("No files selected");
            }
            else
            {
                RefreshPcmList();
                LoggerBold("Done, You can now save files");
            }
        }

        private void RefreshPcmList()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = pcmList;
            dataGridView1.DataSource = bindingSource;
            //dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
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

        private void CopyTableData(TableData srcTd, TableData dstTd, ref PcmFile dstPCM)
        {
            frmTableEditor srcTE = new frmTableEditor();
            srcTE.PrepareTable(PCM, srcTd, null,"A");
            //srcTE.loadTable();
            frmTableEditor dstTE = new frmTableEditor();
            dstTE.PrepareTable(dstPCM, dstTd, null,"A");
            //dstTE.loadTable();

            for (int cell = 0; cell < srcTE.compareFiles[0].tableInfos[0].tableCells.Count;cell++)
            {
                TableCell srcTc = srcTE.compareFiles[0].tableInfos[0].tableCells[cell];
                TableCell dstTc = dstTE.compareFiles[0].tableInfos[0].tableCells[cell];
                dstTc.SaveValue(Convert.ToDouble(srcTc.lastValue));
            }

            dstTE.SaveTable(false);
            srcTE.Dispose();
            dstTE.Dispose();
        }

        private void SearchTargetTables(PcmFile dstPCM, List<TableData> tableTds, bool execNow)
        {
            for (int x = 0; x < tableTds.Count; x++)
            {
                TableData sourceTd = tableTds[x];
                TableData dstTd = FindTableData(sourceTd, dstPCM.tableDatas);
                if (dstTd == null)
                {
                    Logger("Table missing: " + sourceTd.TableName);
                }
                else
                {
                    if (sourceTd.Rows != dstTd.Rows || sourceTd.Columns != dstTd.Columns || sourceTd.RowMajor != dstTd.RowMajor)
                    {
                        Logger("Table size not match: " + sourceTd.TableName);
                    }
                    else
                    {
                        if (execNow)
                        {
                            Logger("Copying table: " + sourceTd.TableName + "...", false);
                            CopyTableData(sourceTd, dstTd, ref dstPCM);
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

        public void StartTableCopy()
        {
            try 
            { 
                frmFileSelection frmF = new frmFileSelection();
                frmF.btnOK.Text = "Select files";
                frmF.Text = "Select target for tables";
                frmF.LoadFiles(GetLastFolder(BinFilter));
                if (frmF.ShowDialog(this) == DialogResult.OK)
                {
                    SetLastFolder(BinFilter, frmF.txtFolder.Text);
                    pcmList = new List<SelectPCM>();
                    for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                    {
                        string fileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                        PcmFile newPCM = new PcmFile(fileName, true, "");
                        LoggerBold(fileName);
                        newPCM.AutoLoadTunerConfig();
                        if (PCM.seekTablesImported && !newPCM.seekTablesImported)
                            newPCM.ImportSeekTables();
                        SelectPCM sPCM = new SelectPCM();
                        sPCM.pcmFile = newPCM;
                        SearchTargetTables(sPCM.pcmFile, tableTds, false);
                        pcmList.Add(sPCM);
                    }
                    LoggerBold("Select destination files and Press Apply to copy tables");
                    RefreshPcmList();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }


        private int CountSelections()
        {
            int selCount = 0;
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                    selCount++;
            }
            return selCount;
        }

        private string GenerateFname(string origFname, string fPath="")
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
            if (CountSelections() == 0)
            {
                Logger("No files selected");
                return;
            }
            for (int i=0; i< pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    string origFname = pcmList[i].pcmFile.FileName;
                    string newFname = GenerateFname(origFname);
                    Logger("Saving file: " + newFname, false);
                    pcmList[i].pcmFile.SaveBin(newFname);
                    Logger(" [OK]");
                }
            }
            Logger("Done");
        }

        private void btnSaveTo_Click(object sender, EventArgs e)
        {
            if (CountSelections() == 0)
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
                    string newFname = GenerateFname(origFName, savePath);
                    Logger("Saving file: " + newFname, false);
                    pcmList[i].pcmFile.SaveBin(newFname);
                    Logger(" [OK]");
                }
            }
            Logger("Done");
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (CountSelections() == 0)
            {
                Logger("No files selected");
                return;
            }
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    string origFName = Path.GetFileName(pcmList[i].pcmFile.FileName);
                    string newFname = SelectSaveFile(BinFilter, origFName);
                    if (newFname.Length > 0)
                    {
                        Logger("Saving file: " + newFname, false);
                        pcmList[i].pcmFile.SaveBin(newFname);
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
            RefreshPcmList();
        }

        private void unSelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < pcmList.Count; i++)
                pcmList[i].Selected = false;
            RefreshPcmList();
        }

        private void btnTuner_Click(object sender, EventArgs e)
        {
            if (CountSelections() == 0)
            {
                Logger("No files selected");
                return;
            }
            FrmTuner frmT = new FrmTuner(null);
            frmT.Show();
            for (int i = 0; i < pcmList.Count; i++)
            {
                if (pcmList[i].Selected)
                {
                    PcmFile newPCM = pcmList[i].pcmFile;
                    frmT.AddtoCurrentFileMenu(newPCM);
                    frmT.PCM = newPCM;
                    frmT.LoadConfigforPCM(ref newPCM);
                }
            }
            frmT.SelectPCM();

        }
    }
}
