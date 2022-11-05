
namespace UniversalPatcher
{
    partial class frmHistogram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHistogram));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabHistogram = new System.Windows.Forms.TabPage();
            this.labelSelectTable = new System.Windows.Forms.Label();
            this.labelCellinfo = new System.Windows.Forms.Label();
            this.tabSelectTable = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnColorLow = new System.Windows.Forms.Button();
            this.btnColorHigh = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnColorMid = new System.Windows.Forms.Button();
            this.numRangeMax = new System.Windows.Forms.NumericUpDown();
            this.numRangeMin = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numSkipValue = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.comboSkipParam = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboValueparam = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboYparam = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboXparam = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReadCsv = new System.Windows.Forms.Button();
            this.txtColumnSeparator = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseCsv = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGetLiveData = new System.Windows.Forms.CheckBox();
            this.groupTemplate = new System.Windows.Forms.GroupBox();
            this.radioTemplateUseTable = new System.Windows.Forms.RadioButton();
            this.radioTemplateManual = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtColHeaders = new System.Windows.Forms.TextBox();
            this.txtRowHeaders = new System.Windows.Forms.TextBox();
            this.btnApplyTemplate = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.numDecimals = new System.Windows.Forms.NumericUpDown();
            this.btnGenColHeaders = new System.Windows.Forms.Button();
            this.btnGenRowHeaders = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabHistogram.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSkipValue)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(786, 377);
            this.dataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabHistogram);
            this.tabControl1.Controls.Add(this.tabSelectTable);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 426);
            this.tabControl1.TabIndex = 1;
            // 
            // tabHistogram
            // 
            this.tabHistogram.Controls.Add(this.labelSelectTable);
            this.tabHistogram.Controls.Add(this.labelCellinfo);
            this.tabHistogram.Controls.Add(this.dataGridView1);
            this.tabHistogram.Location = new System.Drawing.Point(4, 22);
            this.tabHistogram.Name = "tabHistogram";
            this.tabHistogram.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistogram.Size = new System.Drawing.Size(792, 400);
            this.tabHistogram.TabIndex = 0;
            this.tabHistogram.Text = "Histogram";
            this.tabHistogram.UseVisualStyleBackColor = true;
            // 
            // labelSelectTable
            // 
            this.labelSelectTable.AutoSize = true;
            this.labelSelectTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectTable.Location = new System.Drawing.Point(99, 156);
            this.labelSelectTable.Name = "labelSelectTable";
            this.labelSelectTable.Size = new System.Drawing.Size(524, 24);
            this.labelSelectTable.TabIndex = 2;
            this.labelSelectTable.Text = "Select table as template, or setup columns and rows manually";
            // 
            // labelCellinfo
            // 
            this.labelCellinfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCellinfo.AutoSize = true;
            this.labelCellinfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelCellinfo.Location = new System.Drawing.Point(0, 383);
            this.labelCellinfo.Name = "labelCellinfo";
            this.labelCellinfo.Size = new System.Drawing.Size(43, 15);
            this.labelCellinfo.TabIndex = 1;
            this.labelCellinfo.Text = "Cellinfo";
            // 
            // tabSelectTable
            // 
            this.tabSelectTable.Location = new System.Drawing.Point(4, 22);
            this.tabSelectTable.Name = "tabSelectTable";
            this.tabSelectTable.Size = new System.Drawing.Size(792, 400);
            this.tabSelectTable.TabIndex = 2;
            this.tabSelectTable.Text = "Table selection";
            this.tabSelectTable.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupTemplate);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Controls.Add(this.numSkipValue);
            this.tabSettings.Controls.Add(this.label7);
            this.tabSettings.Controls.Add(this.comboSkipParam);
            this.tabSettings.Controls.Add(this.label6);
            this.tabSettings.Controls.Add(this.comboValueparam);
            this.tabSettings.Controls.Add(this.label5);
            this.tabSettings.Controls.Add(this.comboYparam);
            this.tabSettings.Controls.Add(this.label4);
            this.tabSettings.Controls.Add(this.comboXparam);
            this.tabSettings.Controls.Add(this.label3);
            this.tabSettings.Controls.Add(this.btnReadCsv);
            this.tabSettings.Controls.Add(this.txtColumnSeparator);
            this.tabSettings.Controls.Add(this.label2);
            this.tabSettings.Controls.Add(this.btnBrowseCsv);
            this.tabSettings.Controls.Add(this.label1);
            this.tabSettings.Controls.Add(this.txtLogFile);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(792, 400);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnColorLow);
            this.groupBox1.Controls.Add(this.btnColorHigh);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.btnColorMid);
            this.groupBox1.Controls.Add(this.numRangeMax);
            this.groupBox1.Controls.Add(this.numRangeMin);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 126);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(221, 137);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shading";
            // 
            // btnColorLow
            // 
            this.btnColorLow.BackColor = System.Drawing.Color.Lime;
            this.btnColorLow.Location = new System.Drawing.Point(144, 81);
            this.btnColorLow.Name = "btnColorLow";
            this.btnColorLow.Size = new System.Drawing.Size(59, 20);
            this.btnColorLow.TabIndex = 8;
            this.btnColorLow.Text = "Color";
            this.btnColorLow.UseVisualStyleBackColor = false;
            this.btnColorLow.Click += new System.EventHandler(this.btnColorLow_Click);
            // 
            // btnColorHigh
            // 
            this.btnColorHigh.BackColor = System.Drawing.Color.LightCoral;
            this.btnColorHigh.Location = new System.Drawing.Point(144, 19);
            this.btnColorHigh.Name = "btnColorHigh";
            this.btnColorHigh.Size = new System.Drawing.Size(59, 20);
            this.btnColorHigh.TabIndex = 7;
            this.btnColorHigh.Text = "Color";
            this.btnColorHigh.UseVisualStyleBackColor = false;
            this.btnColorHigh.Click += new System.EventHandler(this.btnColorHigh_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Low value";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 52);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Mid value";
            // 
            // btnColorMid
            // 
            this.btnColorMid.BackColor = System.Drawing.Color.White;
            this.btnColorMid.Location = new System.Drawing.Point(144, 52);
            this.btnColorMid.Name = "btnColorMid";
            this.btnColorMid.Size = new System.Drawing.Size(59, 20);
            this.btnColorMid.TabIndex = 4;
            this.btnColorMid.Text = "Color";
            this.btnColorMid.UseVisualStyleBackColor = false;
            this.btnColorMid.Click += new System.EventHandler(this.btnColorMid_Click);
            // 
            // numRangeMax
            // 
            this.numRangeMax.DecimalPlaces = 2;
            this.numRangeMax.Location = new System.Drawing.Point(78, 19);
            this.numRangeMax.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numRangeMax.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numRangeMax.Name = "numRangeMax";
            this.numRangeMax.Size = new System.Drawing.Size(62, 20);
            this.numRangeMax.TabIndex = 3;
            // 
            // numRangeMin
            // 
            this.numRangeMin.DecimalPlaces = 2;
            this.numRangeMin.Location = new System.Drawing.Point(78, 81);
            this.numRangeMin.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numRangeMin.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numRangeMin.Name = "numRangeMin";
            this.numRangeMin.Size = new System.Drawing.Size(62, 20);
            this.numRangeMin.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "High value";
            // 
            // numSkipValue
            // 
            this.numSkipValue.Location = new System.Drawing.Point(484, 95);
            this.numSkipValue.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numSkipValue.Name = "numSkipValue";
            this.numSkipValue.Size = new System.Drawing.Size(51, 20);
            this.numSkipValue.TabIndex = 15;
            this.numSkipValue.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(456, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "<";
            // 
            // comboSkipParam
            // 
            this.comboSkipParam.FormattingEnabled = true;
            this.comboSkipParam.Location = new System.Drawing.Point(323, 94);
            this.comboSkipParam.Name = "comboSkipParam";
            this.comboSkipParam.Size = new System.Drawing.Size(121, 21);
            this.comboSkipParam.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(221, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Skip if parameter";
            // 
            // comboValueparam
            // 
            this.comboValueparam.FormattingEnabled = true;
            this.comboValueparam.Location = new System.Drawing.Point(323, 64);
            this.comboValueparam.Name = "comboValueparam";
            this.comboValueparam.Size = new System.Drawing.Size(121, 21);
            this.comboValueparam.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Value parameter";
            // 
            // comboYparam
            // 
            this.comboYparam.FormattingEnabled = true;
            this.comboYparam.Location = new System.Drawing.Point(78, 94);
            this.comboYparam.Name = "comboYparam";
            this.comboYparam.Size = new System.Drawing.Size(121, 21);
            this.comboYparam.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Y parameter";
            // 
            // comboXparam
            // 
            this.comboXparam.FormattingEnabled = true;
            this.comboXparam.Location = new System.Drawing.Point(78, 64);
            this.comboXparam.Name = "comboXparam";
            this.comboXparam.Size = new System.Drawing.Size(121, 21);
            this.comboXparam.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "X parameter";
            // 
            // btnReadCsv
            // 
            this.btnReadCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadCsv.Location = new System.Drawing.Point(714, 58);
            this.btnReadCsv.Name = "btnReadCsv";
            this.btnReadCsv.Size = new System.Drawing.Size(72, 27);
            this.btnReadCsv.TabIndex = 5;
            this.btnReadCsv.Text = "Go";
            this.btnReadCsv.UseVisualStyleBackColor = true;
            this.btnReadCsv.Click += new System.EventHandler(this.btnReadCsv_Click);
            // 
            // txtColumnSeparator
            // 
            this.txtColumnSeparator.Location = new System.Drawing.Point(111, 6);
            this.txtColumnSeparator.Name = "txtColumnSeparator";
            this.txtColumnSeparator.Size = new System.Drawing.Size(41, 20);
            this.txtColumnSeparator.TabIndex = 4;
            this.txtColumnSeparator.Text = ",";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Column separator:";
            // 
            // btnBrowseCsv
            // 
            this.btnBrowseCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseCsv.Location = new System.Drawing.Point(744, 30);
            this.btnBrowseCsv.Name = "btnBrowseCsv";
            this.btnBrowseCsv.Size = new System.Drawing.Size(41, 22);
            this.btnBrowseCsv.TabIndex = 2;
            this.btnBrowseCsv.Text = "...";
            this.btnBrowseCsv.UseVisualStyleBackColor = true;
            this.btnBrowseCsv.Click += new System.EventHandler(this.btnBrowseCsv_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log file:";
            // 
            // txtLogFile
            // 
            this.txtLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFile.Location = new System.Drawing.Point(78, 32);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(660, 20);
            this.txtLogFile.TabIndex = 0;
            this.txtLogFile.TextChanged += new System.EventHandler(this.txtCsvFile_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSettingsToolStripMenuItem,
            this.saveSettingsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadSettingsToolStripMenuItem.Text = "Load settings";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveSettingsToolStripMenuItem.Text = "Save Settings";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // chkGetLiveData
            // 
            this.chkGetLiveData.AutoSize = true;
            this.chkGetLiveData.Location = new System.Drawing.Point(144, 5);
            this.chkGetLiveData.Name = "chkGetLiveData";
            this.chkGetLiveData.Size = new System.Drawing.Size(149, 17);
            this.chkGetLiveData.TabIndex = 17;
            this.chkGetLiveData.Text = "Receive data from Logger";
            this.chkGetLiveData.UseVisualStyleBackColor = true;
            this.chkGetLiveData.CheckedChanged += new System.EventHandler(this.chkGetLiveData_CheckedChanged);
            // 
            // groupTemplate
            // 
            this.groupTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupTemplate.Controls.Add(this.btnGenRowHeaders);
            this.groupTemplate.Controls.Add(this.btnGenColHeaders);
            this.groupTemplate.Controls.Add(this.numDecimals);
            this.groupTemplate.Controls.Add(this.label13);
            this.groupTemplate.Controls.Add(this.btnApplyTemplate);
            this.groupTemplate.Controls.Add(this.txtRowHeaders);
            this.groupTemplate.Controls.Add(this.txtColHeaders);
            this.groupTemplate.Controls.Add(this.label12);
            this.groupTemplate.Controls.Add(this.label11);
            this.groupTemplate.Controls.Add(this.radioTemplateManual);
            this.groupTemplate.Controls.Add(this.radioTemplateUseTable);
            this.groupTemplate.Location = new System.Drawing.Point(260, 135);
            this.groupTemplate.Name = "groupTemplate";
            this.groupTemplate.Size = new System.Drawing.Size(524, 128);
            this.groupTemplate.TabIndex = 17;
            this.groupTemplate.TabStop = false;
            this.groupTemplate.Text = "Template";
            // 
            // radioTemplateUseTable
            // 
            this.radioTemplateUseTable.AutoSize = true;
            this.radioTemplateUseTable.Checked = true;
            this.radioTemplateUseTable.Location = new System.Drawing.Point(19, 18);
            this.radioTemplateUseTable.Name = "radioTemplateUseTable";
            this.radioTemplateUseTable.Size = new System.Drawing.Size(74, 17);
            this.radioTemplateUseTable.TabIndex = 0;
            this.radioTemplateUseTable.TabStop = true;
            this.radioTemplateUseTable.Text = "Use Table";
            this.radioTemplateUseTable.UseVisualStyleBackColor = true;
            // 
            // radioTemplateManual
            // 
            this.radioTemplateManual.AutoSize = true;
            this.radioTemplateManual.Location = new System.Drawing.Point(111, 19);
            this.radioTemplateManual.Name = "radioTemplateManual";
            this.radioTemplateManual.Size = new System.Drawing.Size(122, 17);
            this.radioTemplateManual.TabIndex = 1;
            this.radioTemplateManual.TabStop = true;
            this.radioTemplateManual.Text = "Use manual headers";
            this.radioTemplateManual.UseVisualStyleBackColor = true;
            this.radioTemplateManual.CheckedChanged += new System.EventHandler(this.radioTemplateManual_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Column headers:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Row headers:";
            // 
            // txtColHeaders
            // 
            this.txtColHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColHeaders.Location = new System.Drawing.Point(124, 40);
            this.txtColHeaders.Name = "txtColHeaders";
            this.txtColHeaders.Size = new System.Drawing.Size(369, 20);
            this.txtColHeaders.TabIndex = 4;
            // 
            // txtRowHeaders
            // 
            this.txtRowHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRowHeaders.Location = new System.Drawing.Point(124, 71);
            this.txtRowHeaders.Name = "txtRowHeaders";
            this.txtRowHeaders.Size = new System.Drawing.Size(369, 20);
            this.txtRowHeaders.TabIndex = 5;
            // 
            // btnApplyTemplate
            // 
            this.btnApplyTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyTemplate.Enabled = false;
            this.btnApplyTemplate.Location = new System.Drawing.Point(432, 97);
            this.btnApplyTemplate.Name = "btnApplyTemplate";
            this.btnApplyTemplate.Size = new System.Drawing.Size(86, 23);
            this.btnApplyTemplate.TabIndex = 6;
            this.btnApplyTemplate.Text = "Apply";
            this.btnApplyTemplate.UseVisualStyleBackColor = true;
            this.btnApplyTemplate.Click += new System.EventHandler(this.btnApplyTemplate_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 97);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "Decimals";
            // 
            // numDecimals
            // 
            this.numDecimals.Location = new System.Drawing.Point(124, 95);
            this.numDecimals.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDecimals.Name = "numDecimals";
            this.numDecimals.Size = new System.Drawing.Size(57, 20);
            this.numDecimals.TabIndex = 8;
            this.numDecimals.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // btnGenColHeaders
            // 
            this.btnGenColHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenColHeaders.Location = new System.Drawing.Point(497, 38);
            this.btnGenColHeaders.Name = "btnGenColHeaders";
            this.btnGenColHeaders.Size = new System.Drawing.Size(20, 24);
            this.btnGenColHeaders.TabIndex = 9;
            this.btnGenColHeaders.Text = ">";
            this.btnGenColHeaders.UseVisualStyleBackColor = true;
            this.btnGenColHeaders.Click += new System.EventHandler(this.btnGenColHeaders_Click);
            // 
            // btnGenRowHeaders
            // 
            this.btnGenRowHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenRowHeaders.Location = new System.Drawing.Point(497, 68);
            this.btnGenRowHeaders.Name = "btnGenRowHeaders";
            this.btnGenRowHeaders.Size = new System.Drawing.Size(20, 24);
            this.btnGenRowHeaders.TabIndex = 10;
            this.btnGenRowHeaders.Text = ">";
            this.btnGenRowHeaders.UseVisualStyleBackColor = true;
            this.btnGenRowHeaders.Click += new System.EventHandler(this.btnGenRowHeaders_Click);
            // 
            // frmHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkGetLiveData);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmHistogram";
            this.Text = "Histogram";
            this.Load += new System.EventHandler(this.frmHistogram_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabHistogram.ResumeLayout(false);
            this.tabHistogram.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSkipValue)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupTemplate.ResumeLayout(false);
            this.groupTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabHistogram;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Button btnBrowseCsv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLogFile;
        private System.Windows.Forms.Button btnReadCsv;
        private System.Windows.Forms.TextBox txtColumnSeparator;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboYparam;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboXparam;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numSkipValue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboSkipParam;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboValueparam;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnColorLow;
        private System.Windows.Forms.Button btnColorHigh;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnColorMid;
        private System.Windows.Forms.NumericUpDown numRangeMax;
        private System.Windows.Forms.NumericUpDown numRangeMin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Label labelCellinfo;
        private System.Windows.Forms.TabPage tabSelectTable;
        private System.Windows.Forms.Label labelSelectTable;
        private System.Windows.Forms.CheckBox chkGetLiveData;
        private System.Windows.Forms.GroupBox groupTemplate;
        private System.Windows.Forms.TextBox txtRowHeaders;
        private System.Windows.Forms.TextBox txtColHeaders;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton radioTemplateManual;
        private System.Windows.Forms.RadioButton radioTemplateUseTable;
        private System.Windows.Forms.Button btnApplyTemplate;
        private System.Windows.Forms.NumericUpDown numDecimals;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnGenRowHeaders;
        private System.Windows.Forms.Button btnGenColHeaders;
    }
}