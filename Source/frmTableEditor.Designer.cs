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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableEditor));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteSpecialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchCodeFromGoogleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFromCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTableFromCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smoothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interpolateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtMath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.labelUnits = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOBD2DescriptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHistogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoResizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRawHEXValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.binaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableTooltipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rememberCompareSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.conditionalFormattingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGraphicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTableVisualizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offsetVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHEXWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fwdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkSwapXY = new System.Windows.Forms.CheckBox();
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.contextMenuHexWindowSettings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setExtraoffsetToPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrollToTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highlightBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAsciiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHeadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOffsetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherDataColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifiedColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectionColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numExtraOffset = new System.Windows.Forms.NumericUpDown();
            this.groupExtraOffset = new System.Windows.Forms.GroupBox();
            this.btnApplyExtraOffset = new System.Windows.Forms.Button();
            this.btnToggleHexview = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupSelectCompare.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTuneValue)).BeginInit();
            this.groupDifference.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuHexWindowSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset)).BeginInit();
            this.groupExtraOffset.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(652, 355);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
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
            this.copyTableFromCompareToolStripMenuItem,
            this.smoothToolStripMenuItem,
            this.interpolateToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 224);
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
            // smoothToolStripMenuItem
            // 
            this.smoothToolStripMenuItem.Name = "smoothToolStripMenuItem";
            this.smoothToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.smoothToolStripMenuItem.Text = "Smooth";
            this.smoothToolStripMenuItem.Click += new System.EventHandler(this.smoothToolStripMenuItem_Click);
            // 
            // interpolateToolStripMenuItem
            // 
            this.interpolateToolStripMenuItem.Name = "interpolateToolStripMenuItem";
            this.interpolateToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.interpolateToolStripMenuItem.Text = "Interpolate";
            this.interpolateToolStripMenuItem.Click += new System.EventHandler(this.interpolateToolStripMenuItem_Click);
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
            this.btnExecute.ImageKey = "Apply.png";
            this.btnExecute.ImageList = this.imageList1;
            this.btnExecute.Location = new System.Drawing.Point(123, 46);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(34, 29);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Apply.png");
            this.imageList1.Images.SetKeyName(1, "collapse.png");
            this.imageList1.Images.SetKeyName(2, "expand.png");
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
            this.showRawHEXValuesToolStripMenuItem,
            this.disableTooltipsToolStripMenuItem,
            this.dataFontToolStripMenuItem,
            this.rememberCompareSelectionToolStripMenuItem,
            this.conditionalFormattingToolStripMenuItem});
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
            this.addressToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addressToolStripMenuItem.Text = "Address";
            this.addressToolStripMenuItem.Click += new System.EventHandler(this.addressToolStripMenuItem_Click);
            // 
            // binaryToolStripMenuItem
            // 
            this.binaryToolStripMenuItem.Name = "binaryToolStripMenuItem";
            this.binaryToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.binaryToolStripMenuItem.Text = "Binary";
            this.binaryToolStripMenuItem.Click += new System.EventHandler(this.binaryToolStripMenuItem_Click);
            // 
            // decimalToolStripMenuItem
            // 
            this.decimalToolStripMenuItem.Name = "decimalToolStripMenuItem";
            this.decimalToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
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
            // conditionalFormattingToolStripMenuItem
            // 
            this.conditionalFormattingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.offToolStripMenuItem,
            this.tableSettingsToolStripMenuItem,
            this.tableValuesToolStripMenuItem});
            this.conditionalFormattingToolStripMenuItem.Name = "conditionalFormattingToolStripMenuItem";
            this.conditionalFormattingToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.conditionalFormattingToolStripMenuItem.Text = "Conditional formatting";
            // 
            // offToolStripMenuItem
            // 
            this.offToolStripMenuItem.Name = "offToolStripMenuItem";
            this.offToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.offToolStripMenuItem.Text = "Off";
            this.offToolStripMenuItem.Click += new System.EventHandler(this.offToolStripMenuItem_Click);
            // 
            // tableSettingsToolStripMenuItem
            // 
            this.tableSettingsToolStripMenuItem.Name = "tableSettingsToolStripMenuItem";
            this.tableSettingsToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.tableSettingsToolStripMenuItem.Text = "Table settings";
            this.tableSettingsToolStripMenuItem.Click += new System.EventHandler(this.tableSettingsToolStripMenuItem_Click);
            // 
            // tableValuesToolStripMenuItem
            // 
            this.tableValuesToolStripMenuItem.Name = "tableValuesToolStripMenuItem";
            this.tableValuesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.tableValuesToolStripMenuItem.Text = "Table values";
            this.tableValuesToolStripMenuItem.Click += new System.EventHandler(this.tableValuesToolStripMenuItem_Click);
            // 
            // graphToolStripMenuItem
            // 
            this.graphToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGraphicToolStripMenuItem,
            this.showTableVisualizationToolStripMenuItem,
            this.offsetVisualizerToolStripMenuItem,
            this.showHEXWindowToolStripMenuItem});
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
            // showHEXWindowToolStripMenuItem
            // 
            this.showHEXWindowToolStripMenuItem.Name = "showHEXWindowToolStripMenuItem";
            this.showHEXWindowToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.showHEXWindowToolStripMenuItem.Text = "HEX window";
            this.showHEXWindowToolStripMenuItem.Click += new System.EventHandler(this.showHEXWindowToolStripMenuItem_Click);
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
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 76);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(895, 355);
            this.splitContainer1.SplitterDistance = 652;
            this.splitContainer1.TabIndex = 19;
            // 
            // contextMenuHexWindowSettings
            // 
            this.contextMenuHexWindowSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyEditToolStripMenuItem,
            this.cancelEditToolStripMenuItem,
            this.setExtraoffsetToPositionToolStripMenuItem,
            this.scrollToTableToolStripMenuItem,
            this.toolStripSeparator3,
            this.fontToolStripMenuItem,
            this.columnsToolStripMenuItem,
            this.highlightBackgroundToolStripMenuItem,
            this.showAsciiToolStripMenuItem,
            this.showHeadersToolStripMenuItem,
            this.showOffsetsToolStripMenuItem,
            this.toolStripSeparator1,
            this.backgroundColorToolStripMenuItem,
            this.textColorToolStripMenuItem,
            this.otherDataColorToolStripMenuItem,
            this.modifiedColorToolStripMenuItem,
            this.selectionColorToolStripMenuItem,
            this.resetColorsToolStripMenuItem});
            this.contextMenuHexWindowSettings.Name = "contextMenuHexWindowSettings";
            this.contextMenuHexWindowSettings.Size = new System.Drawing.Size(210, 368);
            // 
            // applyEditToolStripMenuItem
            // 
            this.applyEditToolStripMenuItem.Name = "applyEditToolStripMenuItem";
            this.applyEditToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.applyEditToolStripMenuItem.Text = "Apply edit";
            this.applyEditToolStripMenuItem.Click += new System.EventHandler(this.applyEditToolStripMenuItem_Click);
            // 
            // cancelEditToolStripMenuItem
            // 
            this.cancelEditToolStripMenuItem.Name = "cancelEditToolStripMenuItem";
            this.cancelEditToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.cancelEditToolStripMenuItem.Text = "Cancel Edit";
            this.cancelEditToolStripMenuItem.Click += new System.EventHandler(this.cancelEditToolStripMenuItem_Click);
            // 
            // setExtraoffsetToPositionToolStripMenuItem
            // 
            this.setExtraoffsetToPositionToolStripMenuItem.Name = "setExtraoffsetToPositionToolStripMenuItem";
            this.setExtraoffsetToPositionToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.setExtraoffsetToPositionToolStripMenuItem.Text = "Set as extraoffset position";
            this.setExtraoffsetToPositionToolStripMenuItem.Click += new System.EventHandler(this.setExtraoffsetToPositionToolStripMenuItem_Click);
            // 
            // scrollToTableToolStripMenuItem
            // 
            this.scrollToTableToolStripMenuItem.Name = "scrollToTableToolStripMenuItem";
            this.scrollToTableToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.scrollToTableToolStripMenuItem.Text = "Scroll to table";
            this.scrollToTableToolStripMenuItem.Click += new System.EventHandler(this.scrollToTableToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(206, 6);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.fontToolStripMenuItem.Text = "Font...";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // columnsToolStripMenuItem
            // 
            this.columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
            this.columnsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.columnsToolStripMenuItem.Text = "Columns";
            // 
            // highlightBackgroundToolStripMenuItem
            // 
            this.highlightBackgroundToolStripMenuItem.Enabled = false;
            this.highlightBackgroundToolStripMenuItem.Name = "highlightBackgroundToolStripMenuItem";
            this.highlightBackgroundToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.highlightBackgroundToolStripMenuItem.Text = "Highlight background";
            this.highlightBackgroundToolStripMenuItem.Visible = false;
            this.highlightBackgroundToolStripMenuItem.Click += new System.EventHandler(this.highlightBackgroundToolStripMenuItem_Click);
            // 
            // showAsciiToolStripMenuItem
            // 
            this.showAsciiToolStripMenuItem.Name = "showAsciiToolStripMenuItem";
            this.showAsciiToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.showAsciiToolStripMenuItem.Text = "Show ascii";
            this.showAsciiToolStripMenuItem.Click += new System.EventHandler(this.showAsciiToolStripMenuItem_Click);
            // 
            // showHeadersToolStripMenuItem
            // 
            this.showHeadersToolStripMenuItem.Name = "showHeadersToolStripMenuItem";
            this.showHeadersToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.showHeadersToolStripMenuItem.Text = "Show headers";
            this.showHeadersToolStripMenuItem.Click += new System.EventHandler(this.showHeadersToolStripMenuItem_Click);
            // 
            // showOffsetsToolStripMenuItem
            // 
            this.showOffsetsToolStripMenuItem.Name = "showOffsetsToolStripMenuItem";
            this.showOffsetsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.showOffsetsToolStripMenuItem.Text = "Show offsets";
            this.showOffsetsToolStripMenuItem.Click += new System.EventHandler(this.showOffsetsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(206, 6);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // textColorToolStripMenuItem
            // 
            this.textColorToolStripMenuItem.Name = "textColorToolStripMenuItem";
            this.textColorToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.textColorToolStripMenuItem.Text = "Data Color";
            this.textColorToolStripMenuItem.Click += new System.EventHandler(this.textColorToolStripMenuItem_Click);
            // 
            // otherDataColorToolStripMenuItem
            // 
            this.otherDataColorToolStripMenuItem.Name = "otherDataColorToolStripMenuItem";
            this.otherDataColorToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.otherDataColorToolStripMenuItem.Text = "Other Data Color";
            this.otherDataColorToolStripMenuItem.Click += new System.EventHandler(this.otherDataColorToolStripMenuItem_Click);
            // 
            // modifiedColorToolStripMenuItem
            // 
            this.modifiedColorToolStripMenuItem.Name = "modifiedColorToolStripMenuItem";
            this.modifiedColorToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.modifiedColorToolStripMenuItem.Text = "Modified Color";
            this.modifiedColorToolStripMenuItem.Click += new System.EventHandler(this.modifiedColorToolStripMenuItem_Click);
            // 
            // selectionColorToolStripMenuItem
            // 
            this.selectionColorToolStripMenuItem.Name = "selectionColorToolStripMenuItem";
            this.selectionColorToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.selectionColorToolStripMenuItem.Text = "Selection Color";
            this.selectionColorToolStripMenuItem.Click += new System.EventHandler(this.selectionColorToolStripMenuItem_Click);
            // 
            // resetColorsToolStripMenuItem
            // 
            this.resetColorsToolStripMenuItem.Name = "resetColorsToolStripMenuItem";
            this.resetColorsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.resetColorsToolStripMenuItem.Text = "Reset colors";
            this.resetColorsToolStripMenuItem.Click += new System.EventHandler(this.resetColorsToolStripMenuItem_Click);
            // 
            // numExtraOffset
            // 
            this.numExtraOffset.Location = new System.Drawing.Point(6, 13);
            this.numExtraOffset.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numExtraOffset.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numExtraOffset.Name = "numExtraOffset";
            this.numExtraOffset.Size = new System.Drawing.Size(83, 20);
            this.numExtraOffset.TabIndex = 21;
            // 
            // groupExtraOffset
            // 
            this.groupExtraOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupExtraOffset.Controls.Add(this.btnApplyExtraOffset);
            this.groupExtraOffset.Controls.Add(this.numExtraOffset);
            this.groupExtraOffset.Location = new System.Drawing.Point(716, 31);
            this.groupExtraOffset.Name = "groupExtraOffset";
            this.groupExtraOffset.Size = new System.Drawing.Size(134, 39);
            this.groupExtraOffset.TabIndex = 22;
            this.groupExtraOffset.TabStop = false;
            this.groupExtraOffset.Text = "Extra offset";
            // 
            // btnApplyExtraOffset
            // 
            this.btnApplyExtraOffset.ImageKey = "Apply.png";
            this.btnApplyExtraOffset.ImageList = this.imageList1;
            this.btnApplyExtraOffset.Location = new System.Drawing.Point(95, 8);
            this.btnApplyExtraOffset.Name = "btnApplyExtraOffset";
            this.btnApplyExtraOffset.Size = new System.Drawing.Size(34, 29);
            this.btnApplyExtraOffset.TabIndex = 22;
            this.btnApplyExtraOffset.UseVisualStyleBackColor = true;
            this.btnApplyExtraOffset.Click += new System.EventHandler(this.btnApplyExtraOffset_Click);
            // 
            // btnToggleHexview
            // 
            this.btnToggleHexview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleHexview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleHexview.ImageKey = "expand.png";
            this.btnToggleHexview.ImageList = this.imageList1;
            this.btnToggleHexview.Location = new System.Drawing.Point(855, 39);
            this.btnToggleHexview.Name = "btnToggleHexview";
            this.btnToggleHexview.Size = new System.Drawing.Size(34, 29);
            this.btnToggleHexview.TabIndex = 23;
            this.btnToggleHexview.UseVisualStyleBackColor = true;
            this.btnToggleHexview.Click += new System.EventHandler(this.btnToggleHexview_Click);
            // 
            // frmTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 450);
            this.Controls.Add(this.btnToggleHexview);
            this.Controls.Add(this.groupExtraOffset);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.chkRawHex);
            this.Controls.Add(this.groupDifference);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.numTuneValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numDecimals);
            this.Controls.Add(this.groupSelectCompare);
            this.Controls.Add(this.chkSwapXY);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.labelUnits);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtMath);
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
            this.groupSelectCompare.ResumeLayout(false);
            this.groupSelectCompare.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTuneValue)).EndInit();
            this.groupDifference.ResumeLayout(false);
            this.groupDifference.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuHexWindowSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset)).EndInit();
            this.groupExtraOffset.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox chkSwapXY;
        private System.Windows.Forms.ToolStripMenuItem showRawHEXValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableTooltipsToolStripMenuItem;
        private ToolStripMenuItem graphToolStripMenuItem;
        private ToolStripMenuItem showGraphicToolStripMenuItem;
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
        private ToolStripMenuItem smoothToolStripMenuItem;
        private ToolStripMenuItem interpolateToolStripMenuItem;
        private SplitContainer splitContainer1;
        private ToolStripMenuItem showHEXWindowToolStripMenuItem;
        private ToolStripMenuItem conditionalFormattingToolStripMenuItem;
        private ToolStripMenuItem tableSettingsToolStripMenuItem;
        private ToolStripMenuItem tableValuesToolStripMenuItem;
        private ToolStripMenuItem offToolStripMenuItem;
        private ContextMenuStrip contextMenuHexWindowSettings;
        private ToolStripMenuItem fontToolStripMenuItem;
        private ToolStripMenuItem columnsToolStripMenuItem;
        private ToolStripMenuItem backgroundColorToolStripMenuItem;
        private ToolStripMenuItem modifiedColorToolStripMenuItem;
        private ToolStripMenuItem selectionColorToolStripMenuItem;
        private ToolStripMenuItem textColorToolStripMenuItem;
        private ToolStripMenuItem highlightBackgroundToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem showHeadersToolStripMenuItem;
        private ToolStripMenuItem showOffsetsToolStripMenuItem;
        private ToolStripMenuItem showAsciiToolStripMenuItem;
        private ToolStripMenuItem resetColorsToolStripMenuItem;
        private ToolStripMenuItem applyEditToolStripMenuItem;
        private ToolStripMenuItem cancelEditToolStripMenuItem;
        private ToolStripMenuItem otherDataColorToolStripMenuItem;
        private NumericUpDown numExtraOffset;
        private GroupBox groupExtraOffset;
        private Button btnApplyExtraOffset;
        private ToolStripMenuItem setExtraoffsetToPositionToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem scrollToTableToolStripMenuItem;
        private ImageList imageList1;
        private Button btnToggleHexview;
    }
}