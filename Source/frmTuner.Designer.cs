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
            this.loadXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDifferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTableSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importXDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTinyTunerDBV6OnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCSVexperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCSV2ExperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importXMLgeneratorCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportXDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportXMLgeneratorCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTablesWithEmptyAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableMultitableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableConfigModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTunerModeColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.comboFilterBy = new System.Windows.Forms.ComboBox();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtDescription = new System.Windows.Forms.RichTextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showTablelistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.currentFileToolStripMenuItem});
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
            this.loadXMLToolStripMenuItem,
            this.saveXMLToolStripMenuItem,
            this.saveXMLAsToolStripMenuItem,
            this.clearTableToolStripMenuItem,
            this.findDifferencesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadBINToolStripMenuItem
            // 
            this.loadBINToolStripMenuItem.Name = "loadBINToolStripMenuItem";
            this.loadBINToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadBINToolStripMenuItem.Text = "Open BIN";
            this.loadBINToolStripMenuItem.Click += new System.EventHandler(this.loadBINToolStripMenuItem_Click);
            // 
            // saveBINToolStripMenuItem
            // 
            this.saveBINToolStripMenuItem.Name = "saveBINToolStripMenuItem";
            this.saveBINToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveBINToolStripMenuItem.Text = "&Save BIN";
            this.saveBINToolStripMenuItem.Click += new System.EventHandler(this.saveBINToolStripMenuItem_Click);
            // 
            // saveBinAsToolStripMenuItem
            // 
            this.saveBinAsToolStripMenuItem.Name = "saveBinAsToolStripMenuItem";
            this.saveBinAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveBinAsToolStripMenuItem.Text = "Save BIN &As...";
            this.saveBinAsToolStripMenuItem.Click += new System.EventHandler(this.saveBinAsToolStripMenuItem_Click);
            // 
            // loadXMLToolStripMenuItem
            // 
            this.loadXMLToolStripMenuItem.Name = "loadXMLToolStripMenuItem";
            this.loadXMLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadXMLToolStripMenuItem.Text = "Load XML";
            this.loadXMLToolStripMenuItem.Click += new System.EventHandler(this.loadXMLToolStripMenuItem_Click);
            // 
            // saveXMLToolStripMenuItem
            // 
            this.saveXMLToolStripMenuItem.Name = "saveXMLToolStripMenuItem";
            this.saveXMLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveXMLToolStripMenuItem.Text = "Save XML";
            this.saveXMLToolStripMenuItem.Click += new System.EventHandler(this.saveXMLToolStripMenuItem_Click);
            // 
            // saveXMLAsToolStripMenuItem
            // 
            this.saveXMLAsToolStripMenuItem.Name = "saveXMLAsToolStripMenuItem";
            this.saveXMLAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveXMLAsToolStripMenuItem.Text = "Save XML As...";
            this.saveXMLAsToolStripMenuItem.Click += new System.EventHandler(this.saveXMLAsToolStripMenuItem_Click);
            // 
            // clearTableToolStripMenuItem
            // 
            this.clearTableToolStripMenuItem.Name = "clearTableToolStripMenuItem";
            this.clearTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clearTableToolStripMenuItem.Text = "&Clear Tablelist";
            this.clearTableToolStripMenuItem.Click += new System.EventHandler(this.clearTableToolStripMenuItem_Click);
            // 
            // findDifferencesToolStripMenuItem
            // 
            this.findDifferencesToolStripMenuItem.Name = "findDifferencesToolStripMenuItem";
            this.findDifferencesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.findDifferencesToolStripMenuItem.Text = "Find differences";
            this.findDifferencesToolStripMenuItem.Click += new System.EventHandler(this.findDifferencesToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importDTCToolStripMenuItem,
            this.importTableSeekToolStripMenuItem,
            this.importXDFToolStripMenuItem,
            this.importTinyTunerDBV6OnlyToolStripMenuItem,
            this.importCSVexperimentalToolStripMenuItem,
            this.importCSV2ExperimentalToolStripMenuItem,
            this.importXMLgeneratorCSVToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.importToolStripMenuItem.Text = "&Import";
            // 
            // importDTCToolStripMenuItem
            // 
            this.importDTCToolStripMenuItem.Name = "importDTCToolStripMenuItem";
            this.importDTCToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importDTCToolStripMenuItem.Text = "Import DTC";
            this.importDTCToolStripMenuItem.Click += new System.EventHandler(this.importDTCToolStripMenuItem_Click);
            // 
            // importTableSeekToolStripMenuItem
            // 
            this.importTableSeekToolStripMenuItem.Name = "importTableSeekToolStripMenuItem";
            this.importTableSeekToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importTableSeekToolStripMenuItem.Text = "Import TableSeek";
            this.importTableSeekToolStripMenuItem.Click += new System.EventHandler(this.importTableSeekToolStripMenuItem_Click);
            // 
            // importXDFToolStripMenuItem
            // 
            this.importXDFToolStripMenuItem.Name = "importXDFToolStripMenuItem";
            this.importXDFToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importXDFToolStripMenuItem.Text = "Import XDF";
            this.importXDFToolStripMenuItem.Click += new System.EventHandler(this.importXDFToolStripMenuItem_Click);
            // 
            // importTinyTunerDBV6OnlyToolStripMenuItem
            // 
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Name = "importTinyTunerDBV6OnlyToolStripMenuItem";
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Text = "Import TinyTuner DB (V6 only)";
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Click += new System.EventHandler(this.importTinyTunerDBV6OnlyToolStripMenuItem_Click);
            // 
            // importCSVexperimentalToolStripMenuItem
            // 
            this.importCSVexperimentalToolStripMenuItem.Name = "importCSVexperimentalToolStripMenuItem";
            this.importCSVexperimentalToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importCSVexperimentalToolStripMenuItem.Text = "Import CSV (experimental)";
            this.importCSVexperimentalToolStripMenuItem.Visible = false;
            this.importCSVexperimentalToolStripMenuItem.Click += new System.EventHandler(this.importCSVexperimentalToolStripMenuItem_Click);
            // 
            // importCSV2ExperimentalToolStripMenuItem
            // 
            this.importCSV2ExperimentalToolStripMenuItem.Name = "importCSV2ExperimentalToolStripMenuItem";
            this.importCSV2ExperimentalToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importCSV2ExperimentalToolStripMenuItem.Text = "Import CSV 2 (Experimental)";
            this.importCSV2ExperimentalToolStripMenuItem.Visible = false;
            this.importCSV2ExperimentalToolStripMenuItem.Click += new System.EventHandler(this.importCSV2ExperimentalToolStripMenuItem_Click);
            // 
            // importXMLgeneratorCSVToolStripMenuItem
            // 
            this.importXMLgeneratorCSVToolStripMenuItem.Name = "importXMLgeneratorCSVToolStripMenuItem";
            this.importXMLgeneratorCSVToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importXMLgeneratorCSVToolStripMenuItem.Text = "Import XML-generator CSV";
            this.importXMLgeneratorCSVToolStripMenuItem.Click += new System.EventHandler(this.importXMLgeneratorCSVToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportCSVToolStripMenuItem,
            this.exportXDFToolStripMenuItem,
            this.exportXMLgeneratorCSVToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.exportToolStripMenuItem.Text = "&Export";
            // 
            // exportCSVToolStripMenuItem
            // 
            this.exportCSVToolStripMenuItem.Name = "exportCSVToolStripMenuItem";
            this.exportCSVToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.exportCSVToolStripMenuItem.Text = "Export CSV";
            this.exportCSVToolStripMenuItem.Click += new System.EventHandler(this.exportCsvToolStripMenuItem_Click);
            // 
            // exportXDFToolStripMenuItem
            // 
            this.exportXDFToolStripMenuItem.Name = "exportXDFToolStripMenuItem";
            this.exportXDFToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.exportXDFToolStripMenuItem.Text = "Export XDF";
            this.exportXDFToolStripMenuItem.Click += new System.EventHandler(this.exportXDFToolStripMenuItem_Click);
            // 
            // exportXMLgeneratorCSVToolStripMenuItem
            // 
            this.exportXMLgeneratorCSVToolStripMenuItem.Name = "exportXMLgeneratorCSVToolStripMenuItem";
            this.exportXMLgeneratorCSVToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.exportXMLgeneratorCSVToolStripMenuItem.Text = "Export XML-generator CSV";
            this.exportXMLgeneratorCSVToolStripMenuItem.Click += new System.EventHandler(this.exportXMLgeneratorCSVToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTablesWithEmptyAddressToolStripMenuItem,
            this.disableMultitableToolStripMenuItem,
            this.unitsToolStripMenuItem,
            this.enableConfigModeToolStripMenuItem,
            this.resetTunerModeColumnsToolStripMenuItem,
            this.showTablelistToolStripMenuItem});
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
            // currentFileToolStripMenuItem
            // 
            this.currentFileToolStripMenuItem.Name = "currentFileToolStripMenuItem";
            this.currentFileToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.currentFileToolStripMenuItem.Text = "Current BIN file";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.editTableToolStripMenuItem,
            this.compareWithToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(150, 114);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // editTableToolStripMenuItem
            // 
            this.editTableToolStripMenuItem.Name = "editTableToolStripMenuItem";
            this.editTableToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.editTableToolStripMenuItem.Text = "Edit table";
            this.editTableToolStripMenuItem.Click += new System.EventHandler(this.editTableToolStripMenuItem_Click);
            // 
            // compareWithToolStripMenuItem
            // 
            this.compareWithToolStripMenuItem.Enabled = false;
            this.compareWithToolStripMenuItem.Name = "compareWithToolStripMenuItem";
            this.compareWithToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.compareWithToolStripMenuItem.Text = "Compare with";
            this.compareWithToolStripMenuItem.Visible = false;
            this.compareWithToolStripMenuItem.Click += new System.EventHandler(this.compareWithToolStripMenuItem_Click);
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
            // showTablelistToolStripMenuItem
            // 
            this.showTablelistToolStripMenuItem.Name = "showTablelistToolStripMenuItem";
            this.showTablelistToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.showTablelistToolStripMenuItem.Text = "Show tablelist";
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
        private System.Windows.Forms.ToolStripMenuItem loadXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDTCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTableSeekToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importXDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTinyTunerDBV6OnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearTableToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCSVToolStripMenuItem;
        private ToolStripMenuItem importCSVexperimentalToolStripMenuItem;
        private ToolStripMenuItem exportXDFToolStripMenuItem;
        private Label label1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem showTablesWithEmptyAddressToolStripMenuItem;
        private ToolStripMenuItem importCSV2ExperimentalToolStripMenuItem;
        private ComboBox comboFilterBy;
        private ToolStripMenuItem disableMultitableToolStripMenuItem;
        private ToolStripMenuItem saveBinAsToolStripMenuItem;
        private ToolStripMenuItem unitsToolStripMenuItem;
        private RichTextBox txtResult;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ToolStripMenuItem enableConfigModeToolStripMenuItem;
        private ToolStripMenuItem importXMLgeneratorCSVToolStripMenuItem;
        private ToolStripMenuItem exportXMLgeneratorCSVToolStripMenuItem;
        private ToolStripMenuItem saveXMLAsToolStripMenuItem;
        private RichTextBox txtDescription;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem resetTunerModeColumnsToolStripMenuItem;
        private ToolStripMenuItem loadBINToolStripMenuItem;
        private ToolStripMenuItem currentFileToolStripMenuItem;
        private ToolStripMenuItem compareWithToolStripMenuItem;
        private ToolStripMenuItem findDifferencesToolStripMenuItem;
        private ToolStripMenuItem showTablelistToolStripMenuItem;
    }
}