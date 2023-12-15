using System;
using System.Windows.Forms;

namespace UniversalPatcher
{
    partial class frmTableEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableEditor));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtMath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.labelUnits = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteSpecialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchCodeFromGoogleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFromCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTableFromCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOBD2DescriptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHistogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoResizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapXyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRawHEXValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableTooltipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rememberCompareSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGraphicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTableVisualizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offsetVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fwdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkSwapXY = new System.Windows.Forms.CheckBox();
            this.numColumn = new System.Windows.Forms.NumericUpDown();
            this.labelColumn = new System.Windows.Forms.Label();
            this.groupSelectCompare = new System.Windows.Forms.GroupBox();
            this.radioDifference2 = new System.Windows.Forms.RadioButton();
            this.radioCompareAll = new System.Windows.Forms.RadioButton();
            this.radioSideBySideText = new System.Windows.Forms.RadioButton();
            this.radioSideBySide = new System.Windows.Forms.RadioButton();
            this.radioDifference = new System.Windows.Forms.RadioButton();
            this.radioCompareFile = new System.Windows.Forms.RadioButton();
            this.radioOriginal = new System.Windows.Forms.RadioButton();
            this.numDecimals = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numTuneValue = new System.Windows.Forms.NumericUpDown();
            this.labelInfo = new System.Windows.Forms.Label();
            this.groupDifference = new System.Windows.Forms.GroupBox();
            this.radioPercent = new System.Windows.Forms.RadioButton();
            this.radioMultiplier = new System.Windows.Forms.RadioButton();
            this.radioAbsolute = new System.Windows.Forms.RadioButton();
            this.chkRawHex = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColumn)).BeginInit();
            this.groupSelectCompare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTuneValue)).BeginInit();
            this.groupDifference.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(2, 72);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(891, 359);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // txtMath
            // 
            this.txtMath.Location = new System.Drawing.Point(35, 50);
            this.txtMath.Name = "txtMath";
            this.txtMath.Size = new System.Drawing.Size(82, 20);
            this.txtMath.TabIndex = 1;
            this.txtMath.Text = "X*1";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(123, 50);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(54, 21);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // labelUnits
            // 
            this.labelUnits.AutoSize = true;
            this.labelUnits.Location = new System.Drawing.Point(183, 53);
            this.labelUnits.Name = "labelUnits";
            this.labelUnits.Size = new System.Drawing.Size(10, 13);
            this.labelUnits.TabIndex = 5;
            this.labelUnits.Text = "-";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.pasteSpecialToolStripMenuItem,
            this.exportCsvToolStripMenuItem,
            this.searchCodeFromGoogleToolStripMenuItem,
            this.copyFromCompareToolStripMenuItem,
            this.copyTableFromCompareToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 180);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // pasteSpecialToolStripMenuItem
            // 
            this.pasteSpecialToolStripMenuItem.Name = "pasteSpecialToolStripMenuItem";
            this.pasteSpecialToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.pasteSpecialToolStripMenuItem.Text = "Paste Special...";
            this.pasteSpecialToolStripMenuItem.Click += new System.EventHandler(this.pasteSpecialToolStripMenuItem_Click);
            // 
            // exportCsvToolStripMenuItem
            // 
            this.exportCsvToolStripMenuItem.Name = "exportCsvToolStripMenuItem";
            this.exportCsvToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.exportCsvToolStripMenuItem.Text = "Export csv";
            this.exportCsvToolStripMenuItem.Click += new System.EventHandler(this.exportCsvToolStripMenuItem_Click);
            // 
            // searchCodeFromGoogleToolStripMenuItem
            // 
            this.searchCodeFromGoogleToolStripMenuItem.Name = "searchCodeFromGoogleToolStripMenuItem";
            this.searchCodeFromGoogleToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.searchCodeFromGoogleToolStripMenuItem.Text = "Search code by Google";
            this.searchCodeFromGoogleToolStripMenuItem.Visible = false;
            this.searchCodeFromGoogleToolStripMenuItem.Click += new System.EventHandler(this.searchCodeFromGoogleToolStripMenuItem_Click);
            // 
            // copyFromCompareToolStripMenuItem
            // 
            this.copyFromCompareToolStripMenuItem.Name = "copyFromCompareToolStripMenuItem";
            this.copyFromCompareToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.copyFromCompareToolStripMenuItem.Text = "Copy from compare";
            this.copyFromCompareToolStripMenuItem.Click += new System.EventHandler(this.copyFromCompareToolStripMenuItem_Click);
            // 
            // copyTableFromCompareToolStripMenuItem
            // 
            this.copyTableFromCompareToolStripMenuItem.Name = "copyTableFromCompareToolStripMenuItem";
            this.copyTableFromCompareToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.copyTableFromCompareToolStripMenuItem.Text = "Copy table from compare";
            this.copyTableFromCompareToolStripMenuItem.Click += new System.EventHandler(this.copyTableFromCompareToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.graphToolStripMenuItem,
            this.compareToolStripMenuItem,
            this.rewToolStripMenuItem,
            this.fwdToolStripMenuItem,
            this.upToolStripMenuItem,
            this.downToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(895, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exportCSVToolStripMenuItem1,
            this.saveOBD2DescriptionsToolStripMenuItem,
            this.showHistogramToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exportCSVToolStripMenuItem1
            // 
            this.exportCSVToolStripMenuItem1.Name = "exportCSVToolStripMenuItem1";
            this.exportCSVToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.exportCSVToolStripMenuItem1.Text = "Export CSV";
            this.exportCSVToolStripMenuItem1.Click += new System.EventHandler(this.exportCSVToolStripMenuItem1_Click);
            // 
            // saveOBD2DescriptionsToolStripMenuItem
            // 
            this.saveOBD2DescriptionsToolStripMenuItem.Name = "saveOBD2DescriptionsToolStripMenuItem";
            this.saveOBD2DescriptionsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.saveOBD2DescriptionsToolStripMenuItem.Text = "Save OBD2 Descriptions";
            this.saveOBD2DescriptionsToolStripMenuItem.Click += new System.EventHandler(this.saveOBD2DescriptionsToolStripMenuItem_Click);
            // 
            // showHistogramToolStripMenuItem
            // 
            this.showHistogramToolStripMenuItem.Name = "showHistogramToolStripMenuItem";
            this.showHistogramToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showHistogramToolStripMenuItem.Text = "Show Histogram";
            this.showHistogramToolStripMenuItem.Click += new System.EventHandler(this.showHistogramToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoResizeToolStripMenuItem,
            this.swapXyToolStripMenuItem,
            this.showRawHEXValuesToolStripMenuItem,
            this.disableTooltipsToolStripMenuItem,
            this.dataFontToolStripMenuItem,
            this.rememberCompareSelectionToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // autoResizeToolStripMenuItem
            // 
            this.autoResizeToolStripMenuItem.Name = "autoResizeToolStripMenuItem";
            this.autoResizeToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.autoResizeToolStripMenuItem.Text = "Auto Resize";
            this.autoResizeToolStripMenuItem.Click += new System.EventHandler(this.autoResizeToolStripMenuItem_Click);
            // 
            // swapXyToolStripMenuItem
            // 
            this.swapXyToolStripMenuItem.Name = "swapXyToolStripMenuItem";
            this.swapXyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.swapXyToolStripMenuItem.Text = "Swap x/y";
            this.swapXyToolStripMenuItem.Click += new System.EventHandler(this.swapXyToolStripMenuItem_Click);
            // 
            // showRawHEXValuesToolStripMenuItem
            // 
            this.showRawHEXValuesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addressToolStripMenuItem,
            this.binaryToolStripMenuItem,
            this.decimalToolStripMenuItem});
            this.showRawHEXValuesToolStripMenuItem.Name = "showRawHEXValuesToolStripMenuItem";
            this.showRawHEXValuesToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.showRawHEXValuesToolStripMenuItem.Text = "Show Raw HEX values";
            this.showRawHEXValuesToolStripMenuItem.Click += new System.EventHandler(this.showRawHEXValuesToolStripMenuItem_Click);
            // 
            // addressToolStripMenuItem
            // 
            this.addressToolStripMenuItem.Name = "addressToolStripMenuItem";
            this.addressToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addressToolStripMenuItem.Text = "Address";
            this.addressToolStripMenuItem.Click += new System.EventHandler(this.addressToolStripMenuItem_Click);
            // 
            // binaryToolStripMenuItem
            // 
            this.binaryToolStripMenuItem.Name = "binaryToolStripMenuItem";
            this.binaryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.binaryToolStripMenuItem.Text = "Binary";
            this.binaryToolStripMenuItem.Click += new System.EventHandler(this.binaryToolStripMenuItem_Click);
            // 
            // decimalToolStripMenuItem
            // 
            this.decimalToolStripMenuItem.Name = "decimalToolStripMenuItem";
            this.decimalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.decimalToolStripMenuItem.Text = "Decimal";
            this.decimalToolStripMenuItem.Click += new System.EventHandler(this.decimalToolStripMenuItem_Click);
            // 
            // disableTooltipsToolStripMenuItem
            // 
            this.disableTooltipsToolStripMenuItem.Name = "disableTooltipsToolStripMenuItem";
            this.disableTooltipsToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.disableTooltipsToolStripMenuItem.Text = "Disable Tooltips";
            this.disableTooltipsToolStripMenuItem.Click += new System.EventHandler(this.disableTooltipsToolStripMenuItem_Click);
            // 
            // dataFontToolStripMenuItem
            // 
            this.dataFontToolStripMenuItem.Name = "dataFontToolStripMenuItem";
            this.dataFontToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.dataFontToolStripMenuItem.Text = "Data font...";
            this.dataFontToolStripMenuItem.Click += new System.EventHandler(this.dataFontToolStripMenuItem_Click);
            // 
            // rememberCompareSelectionToolStripMenuItem
            // 
            this.rememberCompareSelectionToolStripMenuItem.Name = "rememberCompareSelectionToolStripMenuItem";
            this.rememberCompareSelectionToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.rememberCompareSelectionToolStripMenuItem.Text = "Remember compare selection";
            this.rememberCompareSelectionToolStripMenuItem.Click += new System.EventHandler(this.rememberCompareSelectionToolStripMenuItem_Click);
            // 
            // graphToolStripMenuItem
            // 
            this.graphToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGraphicToolStripMenuItem,
            this.showTableVisualizationToolStripMenuItem,
            this.offsetVisualizerToolStripMenuItem});
            this.graphToolStripMenuItem.Name = "graphToolStripMenuItem";
            this.graphToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.graphToolStripMenuItem.Text = "View";
            // 
            // showGraphicToolStripMenuItem
            // 
            this.showGraphicToolStripMenuItem.Name = "showGraphicToolStripMenuItem";
            this.showGraphicToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.showGraphicToolStripMenuItem.Text = "Show graphic";
            this.showGraphicToolStripMenuItem.Click += new System.EventHandler(this.showGraphicToolStripMenuItem_Click);
            // 
            // showTableVisualizationToolStripMenuItem
            // 
            this.showTableVisualizationToolStripMenuItem.Name = "showTableVisualizationToolStripMenuItem";
            this.showTableVisualizationToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.showTableVisualizationToolStripMenuItem.Text = "Table visualizer";
            this.showTableVisualizationToolStripMenuItem.Click += new System.EventHandler(this.showTableVisualizationToolStripMenuItem_Click);
            // 
            // offsetVisualizerToolStripMenuItem
            // 
            this.offsetVisualizerToolStripMenuItem.Name = "offsetVisualizerToolStripMenuItem";
            this.offsetVisualizerToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.offsetVisualizerToolStripMenuItem.Text = "Offset visualizer";
            this.offsetVisualizerToolStripMenuItem.Click += new System.EventHandler(this.offsetVisualizerToolStripMenuItem_Click);
            // 
            // compareToolStripMenuItem
            // 
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            this.compareToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.compareToolStripMenuItem.Text = "Compare file";
            // 
            // rewToolStripMenuItem
            // 
            this.rewToolStripMenuItem.Name = "rewToolStripMenuItem";
            this.rewToolStripMenuItem.Size = new System.Drawing.Size(27, 20);
            this.rewToolStripMenuItem.Text = "<";
            this.rewToolStripMenuItem.Click += new System.EventHandler(this.rewToolStripMenuItem_Click);
            // 
            // fwdToolStripMenuItem
            // 
            this.fwdToolStripMenuItem.Name = "fwdToolStripMenuItem";
            this.fwdToolStripMenuItem.Size = new System.Drawing.Size(27, 20);
            this.fwdToolStripMenuItem.Text = ">";
            this.fwdToolStripMenuItem.Click += new System.EventHandler(this.fwdToolStripMenuItem_Click);
            // 
            // upToolStripMenuItem
            // 
            this.upToolStripMenuItem.Name = "upToolStripMenuItem";
            this.upToolStripMenuItem.Size = new System.Drawing.Size(26, 20);
            this.upToolStripMenuItem.Text = "˄";
            this.upToolStripMenuItem.Click += new System.EventHandler(this.upToolStripMenuItem_Click);
            // 
            // downToolStripMenuItem
            // 
            this.downToolStripMenuItem.Name = "downToolStripMenuItem";
            this.downToolStripMenuItem.Size = new System.Drawing.Size(26, 20);
            this.downToolStripMenuItem.Text = "˅";
            this.downToolStripMenuItem.Click += new System.EventHandler(this.downToolStripMenuItem_Click);
            // 
            // chkSwapXY
            // 
            this.chkSwapXY.AutoSize = true;
            this.chkSwapXY.Location = new System.Drawing.Point(347, 4);
            this.chkSwapXY.Name = "chkSwapXY";
            this.chkSwapXY.Size = new System.Drawing.Size(75, 17);
            this.chkSwapXY.TabIndex = 7;
            this.chkSwapXY.Text = "Swap X/Y";
            this.chkSwapXY.UseVisualStyleBackColor = true;
            this.chkSwapXY.CheckedChanged += new System.EventHandler(this.chkSwapXY_CheckedChanged);
            // 
            // numColumn
            // 
            this.numColumn.Enabled = false;
            this.numColumn.Location = new System.Drawing.Point(636, 3);
            this.numColumn.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numColumn.Name = "numColumn";
            this.numColumn.Size = new System.Drawing.Size(38, 20);
            this.numColumn.TabIndex = 8;
            this.numColumn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numColumn.Visible = false;
            this.numColumn.ValueChanged += new System.EventHandler(this.numColumn_ValueChanged);
            // 
            // labelColumn
            // 
            this.labelColumn.AutoSize = true;
            this.labelColumn.Location = new System.Drawing.Point(585, 5);
            this.labelColumn.Name = "labelColumn";
            this.labelColumn.Size = new System.Drawing.Size(45, 13);
            this.labelColumn.TabIndex = 9;
            this.labelColumn.Text = "Column:";
            this.labelColumn.Visible = false;
            // 
            // groupSelectCompare
            // 
            this.groupSelectCompare.Controls.Add(this.radioDifference2);
            this.groupSelectCompare.Controls.Add(this.radioCompareAll);
            this.groupSelectCompare.Controls.Add(this.radioSideBySideText);
            this.groupSelectCompare.Controls.Add(this.radioSideBySide);
            this.groupSelectCompare.Controls.Add(this.radioDifference);
            this.groupSelectCompare.Controls.Add(this.radioCompareFile);
            this.groupSelectCompare.Controls.Add(this.radioOriginal);
            this.groupSelectCompare.Enabled = false;
            this.groupSelectCompare.Location = new System.Drawing.Point(9, 27);
            this.groupSelectCompare.Name = "groupSelectCompare";
            this.groupSelectCompare.Size = new System.Drawing.Size(434, 20);
            this.groupSelectCompare.TabIndex = 10;
            this.groupSelectCompare.TabStop = false;
            this.groupSelectCompare.Text = "Show";
            // 
            // radioDifference2
            // 
            this.radioDifference2.AutoSize = true;
            this.radioDifference2.Enabled = false;
            this.radioDifference2.Location = new System.Drawing.Point(362, 0);
            this.radioDifference2.Name = "radioDifference2";
            this.radioDifference2.Size = new System.Drawing.Size(51, 17);
            this.radioDifference2.TabIndex = 6;
            this.radioDifference2.TabStop = true;
            this.radioDifference2.Text = "A < B";
            this.radioDifference2.UseVisualStyleBackColor = true;
            this.radioDifference2.CheckedChanged += new System.EventHandler(this.radioDifference2_CheckedChanged);
            // 
            // radioCompareAll
            // 
            this.radioCompareAll.AutoSize = true;
            this.radioCompareAll.Location = new System.Drawing.Point(246, 0);
            this.radioCompareAll.Name = "radioCompareAll";
            this.radioCompareAll.Size = new System.Drawing.Size(44, 17);
            this.radioCompareAll.TabIndex = 5;
            this.radioCompareAll.TabStop = true;
            this.radioCompareAll.Text = "A | *";
            this.radioCompareAll.UseVisualStyleBackColor = true;
            this.radioCompareAll.CheckedChanged += new System.EventHandler(this.radioCompareAll_CheckedChanged);
            // 
            // radioSideBySideText
            // 
            this.radioSideBySideText.AutoSize = true;
            this.radioSideBySideText.Location = new System.Drawing.Point(186, 0);
            this.radioSideBySideText.Name = "radioSideBySideText";
            this.radioSideBySideText.Size = new System.Drawing.Size(48, 17);
            this.radioSideBySideText.TabIndex = 4;
            this.radioSideBySideText.TabStop = true;
            this.radioSideBySideText.Text = "A [B]";
            this.radioSideBySideText.UseVisualStyleBackColor = true;
            this.radioSideBySideText.CheckedChanged += new System.EventHandler(this.radioSideBySideText_CheckedChanged);
            // 
            // radioSideBySide
            // 
            this.radioSideBySide.AutoSize = true;
            this.radioSideBySide.Location = new System.Drawing.Point(126, 0);
            this.radioSideBySide.Name = "radioSideBySide";
            this.radioSideBySide.Size = new System.Drawing.Size(47, 17);
            this.radioSideBySide.TabIndex = 3;
            this.radioSideBySide.TabStop = true;
            this.radioSideBySide.Text = "A | B";
            this.radioSideBySide.UseVisualStyleBackColor = true;
            this.radioSideBySide.CheckedChanged += new System.EventHandler(this.radioSideBySide_CheckedChanged);
            // 
            // radioDifference
            // 
            this.radioDifference.AutoSize = true;
            this.radioDifference.Enabled = false;
            this.radioDifference.Location = new System.Drawing.Point(299, 0);
            this.radioDifference.Name = "radioDifference";
            this.radioDifference.Size = new System.Drawing.Size(51, 17);
            this.radioDifference.TabIndex = 2;
            this.radioDifference.Text = "A > B";
            this.radioDifference.UseVisualStyleBackColor = true;
            this.radioDifference.CheckedChanged += new System.EventHandler(this.radioDifference_CheckedChanged);
            // 
            // radioCompareFile
            // 
            this.radioCompareFile.AutoSize = true;
            this.radioCompareFile.Location = new System.Drawing.Point(88, 0);
            this.radioCompareFile.Name = "radioCompareFile";
            this.radioCompareFile.Size = new System.Drawing.Size(32, 17);
            this.radioCompareFile.TabIndex = 1;
            this.radioCompareFile.Text = "B";
            this.radioCompareFile.UseVisualStyleBackColor = true;
            this.radioCompareFile.CheckedChanged += new System.EventHandler(this.radioCompareFile_CheckedChanged);
            // 
            // radioOriginal
            // 
            this.radioOriginal.AutoSize = true;
            this.radioOriginal.Checked = true;
            this.radioOriginal.Location = new System.Drawing.Point(52, 0);
            this.radioOriginal.Name = "radioOriginal";
            this.radioOriginal.Size = new System.Drawing.Size(32, 17);
            this.radioOriginal.TabIndex = 0;
            this.radioOriginal.TabStop = true;
            this.radioOriginal.Text = "A";
            this.radioOriginal.UseVisualStyleBackColor = true;
            this.radioOriginal.CheckedChanged += new System.EventHandler(this.radioOriginal_CheckedChanged);
            // 
            // numDecimals
            // 
            this.numDecimals.Location = new System.Drawing.Point(541, 1);
            this.numDecimals.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numDecimals.Name = "numDecimals";
            this.numDecimals.Size = new System.Drawing.Size(38, 20);
            this.numDecimals.TabIndex = 11;
            this.numDecimals.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numDecimals.ValueChanged += new System.EventHandler(this.numDecimals_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(482, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Decimals:";
            // 
            // numTuneValue
            // 
            this.numTuneValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numTuneValue.Location = new System.Drawing.Point(9, 49);
            this.numTuneValue.Maximum = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.numTuneValue.Minimum = new decimal(new int[] {
            -727379969,
            232,
            0,
            -2147483648});
            this.numTuneValue.Name = "numTuneValue";
            this.numTuneValue.Size = new System.Drawing.Size(17, 20);
            this.numTuneValue.TabIndex = 15;
            this.numTuneValue.ValueChanged += new System.EventHandler(this.numTuneValue_ValueChanged);
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInfo.AutoSize = true;
            this.labelInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelInfo.Location = new System.Drawing.Point(2, 434);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(30, 15);
            this.labelInfo.TabIndex = 16;
            this.labelInfo.Text = "Info:";
            // 
            // groupDifference
            // 
            this.groupDifference.Controls.Add(this.radioPercent);
            this.groupDifference.Controls.Add(this.radioMultiplier);
            this.groupDifference.Controls.Add(this.radioAbsolute);
            this.groupDifference.Location = new System.Drawing.Point(442, 27);
            this.groupDifference.Name = "groupDifference";
            this.groupDifference.Size = new System.Drawing.Size(127, 20);
            this.groupDifference.TabIndex = 17;
            this.groupDifference.TabStop = false;
            this.groupDifference.Visible = false;
            // 
            // radioPercent
            // 
            this.radioPercent.AutoSize = true;
            this.radioPercent.Location = new System.Drawing.Point(91, 0);
            this.radioPercent.Name = "radioPercent";
            this.radioPercent.Size = new System.Drawing.Size(33, 17);
            this.radioPercent.TabIndex = 2;
            this.radioPercent.Text = "%";
            this.radioPercent.UseVisualStyleBackColor = true;
            this.radioPercent.CheckedChanged += new System.EventHandler(this.radioPercent_CheckedChanged);
            // 
            // radioMultiplier
            // 
            this.radioMultiplier.AutoSize = true;
            this.radioMultiplier.Location = new System.Drawing.Point(52, 0);
            this.radioMultiplier.Name = "radioMultiplier";
            this.radioMultiplier.Size = new System.Drawing.Size(30, 17);
            this.radioMultiplier.TabIndex = 1;
            this.radioMultiplier.Text = "x";
            this.radioMultiplier.UseVisualStyleBackColor = true;
            this.radioMultiplier.CheckedChanged += new System.EventHandler(this.radioMultiplier_CheckedChanged);
            // 
            // radioAbsolute
            // 
            this.radioAbsolute.AutoSize = true;
            this.radioAbsolute.Checked = true;
            this.radioAbsolute.Location = new System.Drawing.Point(7, 0);
            this.radioAbsolute.Name = "radioAbsolute";
            this.radioAbsolute.Size = new System.Drawing.Size(39, 17);
            this.radioAbsolute.TabIndex = 0;
            this.radioAbsolute.TabStop = true;
            this.radioAbsolute.Text = "+/-";
            this.radioAbsolute.UseVisualStyleBackColor = true;
            this.radioAbsolute.CheckedChanged += new System.EventHandler(this.radioAbsolute_CheckedChanged);
            // 
            // chkRawHex
            // 
            this.chkRawHex.AutoSize = true;
            this.chkRawHex.Location = new System.Drawing.Point(428, 4);
            this.chkRawHex.Name = "chkRawHex";
            this.chkRawHex.Size = new System.Drawing.Size(48, 17);
            this.chkRawHex.TabIndex = 18;
            this.chkRawHex.Text = "HEX";
            this.chkRawHex.UseVisualStyleBackColor = true;
            this.chkRawHex.CheckedChanged += new System.EventHandler(this.chkRawHex_CheckedChanged);
            // 
            // frmTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 450);
            this.Controls.Add(this.chkRawHex);
            this.Controls.Add(this.groupDifference);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.numTuneValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numDecimals);
            this.Controls.Add(this.groupSelectCompare);
            this.Controls.Add(this.labelColumn);
            this.Controls.Add(this.numColumn);
            this.Controls.Add(this.chkSwapXY);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.labelUnits);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtMath);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmTableEditor";
            this.Text = "Table Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTableEditor_FormClosing);
            this.Load += new System.EventHandler(this.frmTableEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColumn)).EndInit();
            this.groupSelectCompare.ResumeLayout(false);
            this.groupSelectCompare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTuneValue)).EndInit();
            this.groupDifference.ResumeLayout(false);
            this.groupDifference.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtMath;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label labelUnits;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCsvToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCSVToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoResizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem swapXyToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSwapXY;
        private System.Windows.Forms.ToolStripMenuItem showRawHEXValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableTooltipsToolStripMenuItem;
        private ToolStripMenuItem graphToolStripMenuItem;
        private ToolStripMenuItem showGraphicToolStripMenuItem;
        private Label labelColumn;
        public NumericUpDown numColumn;
        private ToolStripMenuItem compareToolStripMenuItem;
        private GroupBox groupSelectCompare;
        private RadioButton radioDifference;
        private RadioButton radioCompareFile;
        private NumericUpDown numDecimals;
        private Label label1;
        private ToolStripMenuItem dataFontToolStripMenuItem;
        private ToolStripMenuItem saveOBD2DescriptionsToolStripMenuItem;
        private ToolStripMenuItem searchCodeFromGoogleToolStripMenuItem;
        private ToolStripMenuItem copyFromCompareToolStripMenuItem;
        private RadioButton radioSideBySideText;
        private RadioButton radioOriginal;
        private RadioButton radioCompareAll;
        private NumericUpDown numTuneValue;
        public RadioButton radioSideBySide;
        private Label labelInfo;
        private GroupBox groupDifference;
        private RadioButton radioPercent;
        private RadioButton radioMultiplier;
        private RadioButton radioAbsolute;
        private RadioButton radioDifference2;
        private ToolStripMenuItem copyTableFromCompareToolStripMenuItem;
        private CheckBox chkRawHex;
        private ToolStripMenuItem pasteSpecialToolStripMenuItem;
        private ToolStripMenuItem showTableVisualizationToolStripMenuItem;
        private ToolStripMenuItem showHistogramToolStripMenuItem;
        private ToolStripMenuItem offsetVisualizerToolStripMenuItem;
        private ToolStripMenuItem rememberCompareSelectionToolStripMenuItem;
        private ToolStripMenuItem rewToolStripMenuItem;
        private ToolStripMenuItem fwdToolStripMenuItem;
        private ToolStripMenuItem downToolStripMenuItem;
        private ToolStripMenuItem upToolStripMenuItem;
        private ToolStripMenuItem addressToolStripMenuItem;
        private ToolStripMenuItem binaryToolStripMenuItem;
        private ToolStripMenuItem decimalToolStripMenuItem;
    }
}