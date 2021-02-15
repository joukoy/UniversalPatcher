using System;
using System.Windows.Forms;

namespace UniversalPatcher
{
    partial class frmTuner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnEditTable = new System.Windows.Forms.Button();
            this.txtSearchTableSeek = new System.Windows.Forms.TextBox();
            this.comboTableCategory = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBinAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMultipleBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllBINFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTablesWithEmptyAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableMultitableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableConfigModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTunerModeColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableConfigAutloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xDFToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tinyTunerDBV6OnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMlgeneratorImportCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVexperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSV2ExperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLGeneratorExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDifferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTablelistxmlTableseekImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTablelistnewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchAndCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchAndCompareAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareSelectedTablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateTableConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.comboFilterBy = new System.Windows.Forms.ComboBox();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtDescription = new System.Windows.Forms.RichTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.massModifyTableListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(850, 409);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView1_DataError);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.DataGridView1_SelectionChanged);
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DataGridView1_UserDeletingRow);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataGridView1_MouseDown);
            this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DataGridView1_MouseUp);
            // 
            // btnEditTable
            // 
            this.btnEditTable.Location = new System.Drawing.Point(539, 27);
            this.btnEditTable.Name = "btnEditTable";
            this.btnEditTable.Size = new System.Drawing.Size(120, 23);
            this.btnEditTable.TabIndex = 6;
            this.btnEditTable.Text = "Edit Table";
            this.btnEditTable.UseVisualStyleBackColor = true;
            this.btnEditTable.Click += new System.EventHandler(this.btnEditTable_Click);
            // 
            // txtSearchTableSeek
            // 
            this.txtSearchTableSeek.Location = new System.Drawing.Point(196, 30);
            this.txtSearchTableSeek.Name = "txtSearchTableSeek";
            this.txtSearchTableSeek.Size = new System.Drawing.Size(131, 20);
            this.txtSearchTableSeek.TabIndex = 14;
            this.txtSearchTableSeek.TextChanged += new System.EventHandler(this.txtSearchTableSeek_TextChanged);
            // 
            // comboTableCategory
            // 
            this.comboTableCategory.FormattingEnabled = true;
            this.comboTableCategory.Location = new System.Drawing.Point(391, 28);
            this.comboTableCategory.Name = "comboTableCategory";
            this.comboTableCategory.Size = new System.Drawing.Size(142, 21);
            this.comboTableCategory.TabIndex = 13;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(333, 33);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Category:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.currentFileToolStripMenuItem,
            this.tableListToolStripMenuItem,
            this.utilitiesToolStripMenuItem,
            this.xmlToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(850, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadBINToolStripMenuItem,
            this.saveBINToolStripMenuItem,
            this.saveBinAsToolStripMenuItem,
            this.openMultipleBINToolStripMenuItem,
            this.saveAllBINFilesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadBINToolStripMenuItem
            // 
            this.loadBINToolStripMenuItem.Name = "loadBINToolStripMenuItem";
            this.loadBINToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.loadBINToolStripMenuItem.Text = "Open BIN";
            this.loadBINToolStripMenuItem.Click += new System.EventHandler(this.loadBINToolStripMenuItem_Click);
            // 
            // saveBINToolStripMenuItem
            // 
            this.saveBINToolStripMenuItem.Name = "saveBINToolStripMenuItem";
            this.saveBINToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.saveBINToolStripMenuItem.Text = "&Save BIN";
            this.saveBINToolStripMenuItem.Click += new System.EventHandler(this.saveBINToolStripMenuItem_Click);
            // 
            // saveBinAsToolStripMenuItem
            // 
            this.saveBinAsToolStripMenuItem.Name = "saveBinAsToolStripMenuItem";
            this.saveBinAsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.saveBinAsToolStripMenuItem.Text = "Save BIN &As...";
            this.saveBinAsToolStripMenuItem.Click += new System.EventHandler(this.saveBinAsToolStripMenuItem_Click);
            // 
            // openMultipleBINToolStripMenuItem
            // 
            this.openMultipleBINToolStripMenuItem.Name = "openMultipleBINToolStripMenuItem";
            this.openMultipleBINToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openMultipleBINToolStripMenuItem.Text = "Open multiple BIN";
            this.openMultipleBINToolStripMenuItem.Click += new System.EventHandler(this.openMultipleBINToolStripMenuItem_Click);
            // 
            // saveAllBINFilesToolStripMenuItem
            // 
            this.saveAllBINFilesToolStripMenuItem.Name = "saveAllBINFilesToolStripMenuItem";
            this.saveAllBINFilesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.saveAllBINFilesToolStripMenuItem.Text = "Save All BIN files";
            this.saveAllBINFilesToolStripMenuItem.Click += new System.EventHandler(this.saveAllBINFilesToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTablesWithEmptyAddressToolStripMenuItem,
            this.disableMultitableToolStripMenuItem,
            this.unitsToolStripMenuItem,
            this.enableConfigModeToolStripMenuItem,
            this.resetTunerModeColumnsToolStripMenuItem,
            this.disableConfigAutloadToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // showTablesWithEmptyAddressToolStripMenuItem
            // 
            this.showTablesWithEmptyAddressToolStripMenuItem.Name = "showTablesWithEmptyAddressToolStripMenuItem";
            this.showTablesWithEmptyAddressToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.showTablesWithEmptyAddressToolStripMenuItem.Text = "Show tables with empty address";
            this.showTablesWithEmptyAddressToolStripMenuItem.Click += new System.EventHandler(this.showTablesWithEmptyAddressToolStripMenuItem_Click);
            // 
            // disableMultitableToolStripMenuItem
            // 
            this.disableMultitableToolStripMenuItem.Name = "disableMultitableToolStripMenuItem";
            this.disableMultitableToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.disableMultitableToolStripMenuItem.Text = "Disable multitable";
            this.disableMultitableToolStripMenuItem.Click += new System.EventHandler(this.disableMultitableToolStripMenuItem_Click);
            // 
            // unitsToolStripMenuItem
            // 
            this.unitsToolStripMenuItem.Name = "unitsToolStripMenuItem";
            this.unitsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.unitsToolStripMenuItem.Text = "Units";
            this.unitsToolStripMenuItem.Click += new System.EventHandler(this.unitsToolStripMenuItem_Click);
            // 
            // enableConfigModeToolStripMenuItem
            // 
            this.enableConfigModeToolStripMenuItem.Name = "enableConfigModeToolStripMenuItem";
            this.enableConfigModeToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.enableConfigModeToolStripMenuItem.Text = "Enable config mode";
            this.enableConfigModeToolStripMenuItem.Click += new System.EventHandler(this.enableConfigModeToolStripMenuItem_Click);
            // 
            // resetTunerModeColumnsToolStripMenuItem
            // 
            this.resetTunerModeColumnsToolStripMenuItem.Name = "resetTunerModeColumnsToolStripMenuItem";
            this.resetTunerModeColumnsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.resetTunerModeColumnsToolStripMenuItem.Text = "Reset tuner mode columns";
            this.resetTunerModeColumnsToolStripMenuItem.Click += new System.EventHandler(this.resetTunerModeColumnsToolStripMenuItem_Click);
            // 
            // disableConfigAutloadToolStripMenuItem
            // 
            this.disableConfigAutloadToolStripMenuItem.Name = "disableConfigAutloadToolStripMenuItem";
            this.disableConfigAutloadToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.disableConfigAutloadToolStripMenuItem.Text = "Disable config autload";
            this.disableConfigAutloadToolStripMenuItem.Click += new System.EventHandler(this.disableConfigAutloadToolStripMenuItem_Click);
            // 
            // currentFileToolStripMenuItem
            // 
            this.currentFileToolStripMenuItem.Name = "currentFileToolStripMenuItem";
            this.currentFileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.currentFileToolStripMenuItem.Text = "BIN file";
            // 
            // tableListToolStripMenuItem
            // 
            this.tableListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem});
            this.tableListToolStripMenuItem.Name = "tableListToolStripMenuItem";
            this.tableListToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.tableListToolStripMenuItem.Text = "Tablelist";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // utilitiesToolStripMenuItem
            // 
            this.utilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.exportToolStripMenuItem1,
            this.findDifferencesToolStripMenuItem,
            this.massModifyTableListsToolStripMenuItem});
            this.utilitiesToolStripMenuItem.Name = "utilitiesToolStripMenuItem";
            this.utilitiesToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.utilitiesToolStripMenuItem.Text = "Utilities";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dTCToolStripMenuItem,
            this.tableSeekToolStripMenuItem,
            this.xDFToolStripMenuItem1,
            this.tinyTunerDBV6OnlyToolStripMenuItem,
            this.xMlgeneratorImportCSVToolStripMenuItem,
            this.cSVexperimentalToolStripMenuItem,
            this.cSV2ExperimentalToolStripMenuItem});
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.importToolStripMenuItem1.Text = "Import";
            // 
            // dTCToolStripMenuItem
            // 
            this.dTCToolStripMenuItem.Name = "dTCToolStripMenuItem";
            this.dTCToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.dTCToolStripMenuItem.Text = "DTC";
            this.dTCToolStripMenuItem.Click += new System.EventHandler(this.dTCToolStripMenuItem_Click);
            // 
            // tableSeekToolStripMenuItem
            // 
            this.tableSeekToolStripMenuItem.Name = "tableSeekToolStripMenuItem";
            this.tableSeekToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.tableSeekToolStripMenuItem.Text = "TableSeek";
            this.tableSeekToolStripMenuItem.Click += new System.EventHandler(this.tableSeekToolStripMenuItem_Click);
            // 
            // xDFToolStripMenuItem1
            // 
            this.xDFToolStripMenuItem1.Name = "xDFToolStripMenuItem1";
            this.xDFToolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.xDFToolStripMenuItem1.Text = "XDF";
            this.xDFToolStripMenuItem1.Click += new System.EventHandler(this.xDFToolStripMenuItem1_Click);
            // 
            // tinyTunerDBV6OnlyToolStripMenuItem
            // 
            this.tinyTunerDBV6OnlyToolStripMenuItem.Name = "tinyTunerDBV6OnlyToolStripMenuItem";
            this.tinyTunerDBV6OnlyToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.tinyTunerDBV6OnlyToolStripMenuItem.Text = "TinyTuner DB (V6 Only)";
            this.tinyTunerDBV6OnlyToolStripMenuItem.Click += new System.EventHandler(this.tinyTunerDBV6OnlyToolStripMenuItem_Click);
            // 
            // xMlgeneratorImportCSVToolStripMenuItem
            // 
            this.xMlgeneratorImportCSVToolStripMenuItem.Name = "xMlgeneratorImportCSVToolStripMenuItem";
            this.xMlgeneratorImportCSVToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.xMlgeneratorImportCSVToolStripMenuItem.Text = "XML-generator CSV";
            this.xMlgeneratorImportCSVToolStripMenuItem.Click += new System.EventHandler(this.xMlgeneratorImportCSVToolStripMenuItem_Click);
            // 
            // cSVexperimentalToolStripMenuItem
            // 
            this.cSVexperimentalToolStripMenuItem.Name = "cSVexperimentalToolStripMenuItem";
            this.cSVexperimentalToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.cSVexperimentalToolStripMenuItem.Text = "CSV (experimental)";
            this.cSVexperimentalToolStripMenuItem.Click += new System.EventHandler(this.cSVexperimentalToolStripMenuItem_Click);
            // 
            // cSV2ExperimentalToolStripMenuItem
            // 
            this.cSV2ExperimentalToolStripMenuItem.Name = "cSV2ExperimentalToolStripMenuItem";
            this.cSV2ExperimentalToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.cSV2ExperimentalToolStripMenuItem.Text = "CSV2 (Experimental)";
            this.cSV2ExperimentalToolStripMenuItem.Click += new System.EventHandler(this.cSV2ExperimentalToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cSVToolStripMenuItem,
            this.xDFToolStripMenuItem,
            this.xMLGeneratorExportToolStripMenuItem});
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.exportToolStripMenuItem1.Text = "Export";
            // 
            // cSVToolStripMenuItem
            // 
            this.cSVToolStripMenuItem.Name = "cSVToolStripMenuItem";
            this.cSVToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.cSVToolStripMenuItem.Text = "CSV";
            this.cSVToolStripMenuItem.Click += new System.EventHandler(this.cSVToolStripMenuItem_Click);
            // 
            // xDFToolStripMenuItem
            // 
            this.xDFToolStripMenuItem.Name = "xDFToolStripMenuItem";
            this.xDFToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.xDFToolStripMenuItem.Text = "XDF";
            this.xDFToolStripMenuItem.Click += new System.EventHandler(this.xDFToolStripMenuItem_Click);
            // 
            // xMLGeneratorExportToolStripMenuItem
            // 
            this.xMLGeneratorExportToolStripMenuItem.Name = "xMLGeneratorExportToolStripMenuItem";
            this.xMLGeneratorExportToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.xMLGeneratorExportToolStripMenuItem.Text = "XML Generator CSV";
            this.xMLGeneratorExportToolStripMenuItem.Click += new System.EventHandler(this.xMLGeneratorExportToolStripMenuItem_Click);
            // 
            // findDifferencesToolStripMenuItem
            // 
            this.findDifferencesToolStripMenuItem.Name = "findDifferencesToolStripMenuItem";
            this.findDifferencesToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.findDifferencesToolStripMenuItem.Text = "Find differences";
            this.findDifferencesToolStripMenuItem.Click += new System.EventHandler(this.findDifferencesToolStripMenuItem_Click);
            // 
            // xmlToolStripMenuItem
            // 
            this.xmlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadXMLToolStripMenuItem,
            this.loadTablelistxmlTableseekImportToolStripMenuItem,
            this.loadTablelistnewToolStripMenuItem,
            this.saveXMLToolStripMenuItem,
            this.saveXMLAsToolStripMenuItem,
            this.clearTableToolStripMenuItem});
            this.xmlToolStripMenuItem.Name = "xmlToolStripMenuItem";
            this.xmlToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.xmlToolStripMenuItem.Text = "Xml";
            // 
            // loadXMLToolStripMenuItem
            // 
            this.loadXMLToolStripMenuItem.Name = "loadXMLToolStripMenuItem";
            this.loadXMLToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.loadXMLToolStripMenuItem.Text = "Load Tablelist";
            this.loadXMLToolStripMenuItem.Click += new System.EventHandler(this.loadXMLToolStripMenuItem_Click);
            // 
            // loadTablelistxmlTableseekImportToolStripMenuItem
            // 
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Name = "loadTablelistxmlTableseekImportToolStripMenuItem";
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Text = "Load tablelist(xml+Tableseek import)";
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Click += new System.EventHandler(this.loadTablelistxmlTableseekImportToolStripMenuItem_Click);
            // 
            // loadTablelistnewToolStripMenuItem
            // 
            this.loadTablelistnewToolStripMenuItem.Name = "loadTablelistnewToolStripMenuItem";
            this.loadTablelistnewToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.loadTablelistnewToolStripMenuItem.Text = "Load tablelist (new)";
            this.loadTablelistnewToolStripMenuItem.Click += new System.EventHandler(this.loadTablelistnewToolStripMenuItem_Click);
            // 
            // saveXMLToolStripMenuItem
            // 
            this.saveXMLToolStripMenuItem.Name = "saveXMLToolStripMenuItem";
            this.saveXMLToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.saveXMLToolStripMenuItem.Text = "Save Tablelist";
            this.saveXMLToolStripMenuItem.Click += new System.EventHandler(this.saveXMLToolStripMenuItem_Click);
            // 
            // saveXMLAsToolStripMenuItem
            // 
            this.saveXMLAsToolStripMenuItem.Name = "saveXMLAsToolStripMenuItem";
            this.saveXMLAsToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.saveXMLAsToolStripMenuItem.Text = "Save Tablelist As...";
            this.saveXMLAsToolStripMenuItem.Click += new System.EventHandler(this.saveXMLAsToolStripMenuItem_Click);
            // 
            // clearTableToolStripMenuItem
            // 
            this.clearTableToolStripMenuItem.Name = "clearTableToolStripMenuItem";
            this.clearTableToolStripMenuItem.Size = new System.Drawing.Size(269, 22);
            this.clearTableToolStripMenuItem.Text = "&Clear Tablelist";
            this.clearTableToolStripMenuItem.Click += new System.EventHandler(this.clearTableToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.editTableToolStripMenuItem,
            this.searchAndCompareToolStripMenuItem,
            this.searchAndCompareAllToolStripMenuItem,
            this.compareSelectedTablesToolStripMenuItem,
            this.toolStripSeparator1,
            this.editRowToolStripMenuItem,
            this.insertRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.duplicateTableConfigToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(204, 252);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // editTableToolStripMenuItem
            // 
            this.editTableToolStripMenuItem.Name = "editTableToolStripMenuItem";
            this.editTableToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.editTableToolStripMenuItem.Text = "Edit table";
            this.editTableToolStripMenuItem.Click += new System.EventHandler(this.editTableToolStripMenuItem_Click);
            // 
            // searchAndCompareToolStripMenuItem
            // 
            this.searchAndCompareToolStripMenuItem.Name = "searchAndCompareToolStripMenuItem";
            this.searchAndCompareToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.searchAndCompareToolStripMenuItem.Text = "Search and compare";
            this.searchAndCompareToolStripMenuItem.Click += new System.EventHandler(this.searchAndCompareToolStripMenuItem_Click);
            // 
            // searchAndCompareAllToolStripMenuItem
            // 
            this.searchAndCompareAllToolStripMenuItem.Name = "searchAndCompareAllToolStripMenuItem";
            this.searchAndCompareAllToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.searchAndCompareAllToolStripMenuItem.Text = "Search and compare All";
            this.searchAndCompareAllToolStripMenuItem.Click += new System.EventHandler(this.searchAndCompareAllToolStripMenuItem_Click);
            // 
            // compareSelectedTablesToolStripMenuItem
            // 
            this.compareSelectedTablesToolStripMenuItem.Name = "compareSelectedTablesToolStripMenuItem";
            this.compareSelectedTablesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.compareSelectedTablesToolStripMenuItem.Text = "Compare selected tables";
            this.compareSelectedTablesToolStripMenuItem.Click += new System.EventHandler(this.compareSelectedTablesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // editRowToolStripMenuItem
            // 
            this.editRowToolStripMenuItem.Enabled = false;
            this.editRowToolStripMenuItem.Name = "editRowToolStripMenuItem";
            this.editRowToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.editRowToolStripMenuItem.Text = "Edit table config";
            this.editRowToolStripMenuItem.Click += new System.EventHandler(this.editRowToolStripMenuItem_Click);
            // 
            // insertRowToolStripMenuItem
            // 
            this.insertRowToolStripMenuItem.Enabled = false;
            this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
            this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.insertRowToolStripMenuItem.Text = "Insert table config";
            this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Enabled = false;
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.deleteRowToolStripMenuItem.Text = "Delete table config";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // duplicateTableConfigToolStripMenuItem
            // 
            this.duplicateTableConfigToolStripMenuItem.Enabled = false;
            this.duplicateTableConfigToolStripMenuItem.Name = "duplicateTableConfigToolStripMenuItem";
            this.duplicateTableConfigToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.duplicateTableConfigToolStripMenuItem.Text = "Duplicate table config";
            this.duplicateTableConfigToolStripMenuItem.Click += new System.EventHandler(this.duplicateTableConfigToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Filter by:";
            // 
            // comboFilterBy
            // 
            this.comboFilterBy.FormattingEnabled = true;
            this.comboFilterBy.Location = new System.Drawing.Point(69, 30);
            this.comboFilterBy.Name = "comboFilterBy";
            this.comboFilterBy.Size = new System.Drawing.Size(121, 21);
            this.comboFilterBy.TabIndex = 18;
            this.comboFilterBy.SelectedIndexChanged += new System.EventHandler(this.comboFilterBy_SelectedIndexChanged);
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(424, 89);
            this.txtResult.TabIndex = 19;
            this.txtResult.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtResult);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtDescription);
            this.splitContainer1.Size = new System.Drawing.Size(850, 89);
            this.splitContainer1.SplitterDistance = 424;
            this.splitContainer1.TabIndex = 21;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(0, 0);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(422, 89);
            this.txtDescription.TabIndex = 0;
            this.txtDescription.Text = "";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 57);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(850, 502);
            this.splitContainer2.SplitterDistance = 409;
            this.splitContainer2.TabIndex = 22;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // massModifyTableListsToolStripMenuItem
            // 
            this.massModifyTableListsToolStripMenuItem.Name = "massModifyTableListsToolStripMenuItem";
            this.massModifyTableListsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.massModifyTableListsToolStripMenuItem.Text = "Mass modify TableLists";
            this.massModifyTableListsToolStripMenuItem.Click += new System.EventHandler(this.massModifyTableListsToolStripMenuItem_Click);
            // 
            // frmTuner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 559);
            this.Controls.Add(this.comboFilterBy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSearchTableSeek);
            this.Controls.Add(this.comboTableCategory);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btnEditTable);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer2);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmTuner";
            this.Text = "Tuner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTuner_FormClosing);
            this.Load += new System.EventHandler(this.frmTuner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnEditTable;
        private System.Windows.Forms.TextBox txtSearchTableSeek;
        private System.Windows.Forms.ComboBox comboTableCategory;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBINToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTableToolStripMenuItem;
        private Label label1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem showTablesWithEmptyAddressToolStripMenuItem;
        private ComboBox comboFilterBy;
        private ToolStripMenuItem disableMultitableToolStripMenuItem;
        private ToolStripMenuItem saveBinAsToolStripMenuItem;
        private ToolStripMenuItem unitsToolStripMenuItem;
        private RichTextBox txtResult;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ToolStripMenuItem enableConfigModeToolStripMenuItem;
        private RichTextBox txtDescription;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem resetTunerModeColumnsToolStripMenuItem;
        private ToolStripMenuItem loadBINToolStripMenuItem;
        private ToolStripMenuItem currentFileToolStripMenuItem;
        private ToolStripMenuItem tableListToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem insertRowToolStripMenuItem;
        private ToolStripMenuItem deleteRowToolStripMenuItem;
        private ToolStripMenuItem editRowToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem duplicateTableConfigToolStripMenuItem;
        private ToolStripMenuItem searchAndCompareToolStripMenuItem;
        private ToolStripMenuItem utilitiesToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem1;
        private ToolStripMenuItem dTCToolStripMenuItem;
        private ToolStripMenuItem tableSeekToolStripMenuItem;
        private ToolStripMenuItem xDFToolStripMenuItem1;
        private ToolStripMenuItem tinyTunerDBV6OnlyToolStripMenuItem;
        private ToolStripMenuItem xMlgeneratorImportCSVToolStripMenuItem;
        private ToolStripMenuItem cSVexperimentalToolStripMenuItem;
        private ToolStripMenuItem cSV2ExperimentalToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem1;
        private ToolStripMenuItem cSVToolStripMenuItem;
        private ToolStripMenuItem xDFToolStripMenuItem;
        private ToolStripMenuItem xMLGeneratorExportToolStripMenuItem;
        private ToolStripMenuItem xmlToolStripMenuItem;
        private ToolStripMenuItem loadXMLToolStripMenuItem;
        private ToolStripMenuItem saveXMLToolStripMenuItem;
        private ToolStripMenuItem saveXMLAsToolStripMenuItem;
        private ToolStripMenuItem clearTableToolStripMenuItem;
        private ToolStripMenuItem loadTablelistxmlTableseekImportToolStripMenuItem;
        private ToolStripMenuItem loadTablelistnewToolStripMenuItem;
        private ToolStripMenuItem openMultipleBINToolStripMenuItem;
        private ToolStripMenuItem saveAllBINFilesToolStripMenuItem;
        private ToolStripMenuItem disableConfigAutloadToolStripMenuItem;
        private ToolStripMenuItem findDifferencesToolStripMenuItem;
        private ToolStripMenuItem searchAndCompareAllToolStripMenuItem;
        private ToolStripMenuItem compareSelectedTablesToolStripMenuItem;
        private ToolStripMenuItem massModifyTableListsToolStripMenuItem;
    }
}