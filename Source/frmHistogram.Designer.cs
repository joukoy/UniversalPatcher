
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
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.groupParams = new System.Windows.Forms.GroupBox();
            this.btnRefreshParams = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboXparam = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboYparam = new System.Windows.Forms.ComboBox();
            this.btnReadCsv = new System.Windows.Forms.Button();
            this.numSkipValue = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboValueparam = new System.Windows.Forms.ComboBox();
            this.comboSkipParam = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupLogfile = new System.Windows.Forms.GroupBox();
            this.txtColumnSeparator = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.btnBrowseCsv = new System.Windows.Forms.Button();
            this.groupTemplate = new System.Windows.Forms.GroupBox();
            this.btnLoadTable = new System.Windows.Forms.Button();
            this.btnGenRowHeaders = new System.Windows.Forms.Button();
            this.btnGenColHeaders = new System.Windows.Forms.Button();
            this.numDecimals = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.btnApplyTemplate = new System.Windows.Forms.Button();
            this.txtRowHeaders = new System.Windows.Forms.TextBox();
            this.txtColHeaders = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnColorLow = new System.Windows.Forms.Button();
            this.btnColorHigh = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnColorMid = new System.Windows.Forms.Button();
            this.numRangeMax = new System.Windows.Forms.NumericUpDown();
            this.numRangeMin = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.tabTemplateBin = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGetLiveData = new System.Windows.Forms.CheckBox();
            this.timerLiveData = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.labelCellinfo = new System.Windows.Forms.Label();
            this.chkShowSettings = new System.Windows.Forms.CheckBox();
            this.tabSavedSettings = new System.Windows.Forms.TabPage();
            this.dataGridViewSavedSettings = new System.Windows.Forms.DataGridView();
            this.btnSaveCurrent = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSkipValue)).BeginInit();
            this.groupLogfile.SuspendLayout();
            this.groupTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMin)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabSavedSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSavedSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Location = new System.Drawing.Point(4, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(901, 257);
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
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabTemplateBin);
            this.tabControl1.Controls.Add(this.tabSavedSettings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(909, 357);
            this.tabControl1.TabIndex = 1;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.groupParams);
            this.tabSettings.Controls.Add(this.groupLogfile);
            this.tabSettings.Controls.Add(this.groupTemplate);
            this.tabSettings.Controls.Add(this.groupBox1);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(901, 331);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupParams
            // 
            this.groupParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupParams.Controls.Add(this.btnRefreshParams);
            this.groupParams.Controls.Add(this.label3);
            this.groupParams.Controls.Add(this.comboXparam);
            this.groupParams.Controls.Add(this.label4);
            this.groupParams.Controls.Add(this.comboYparam);
            this.groupParams.Controls.Add(this.btnReadCsv);
            this.groupParams.Controls.Add(this.numSkipValue);
            this.groupParams.Controls.Add(this.label5);
            this.groupParams.Controls.Add(this.label7);
            this.groupParams.Controls.Add(this.comboValueparam);
            this.groupParams.Controls.Add(this.comboSkipParam);
            this.groupParams.Controls.Add(this.label6);
            this.groupParams.Location = new System.Drawing.Point(8, 91);
            this.groupParams.Name = "groupParams";
            this.groupParams.Size = new System.Drawing.Size(885, 95);
            this.groupParams.TabIndex = 19;
            this.groupParams.TabStop = false;
            this.groupParams.Text = "Parameters";
            // 
            // btnRefreshParams
            // 
            this.btnRefreshParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshParams.Location = new System.Drawing.Point(690, 59);
            this.btnRefreshParams.Name = "btnRefreshParams";
            this.btnRefreshParams.Size = new System.Drawing.Size(93, 23);
            this.btnRefreshParams.TabIndex = 16;
            this.btnRefreshParams.Text = "Refresh";
            this.btnRefreshParams.UseVisualStyleBackColor = true;
            this.btnRefreshParams.Click += new System.EventHandler(this.btnRefreshParams_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "X parameter";
            // 
            // comboXparam
            // 
            this.comboXparam.FormattingEnabled = true;
            this.comboXparam.Location = new System.Drawing.Point(85, 25);
            this.comboXparam.Name = "comboXparam";
            this.comboXparam.Size = new System.Drawing.Size(121, 21);
            this.comboXparam.TabIndex = 7;
            this.comboXparam.SelectedIndexChanged += new System.EventHandler(this.comboXparam_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Y parameter";
            // 
            // comboYparam
            // 
            this.comboYparam.FormattingEnabled = true;
            this.comboYparam.Location = new System.Drawing.Point(85, 55);
            this.comboYparam.Name = "comboYparam";
            this.comboYparam.Size = new System.Drawing.Size(121, 21);
            this.comboYparam.TabIndex = 9;
            // 
            // btnReadCsv
            // 
            this.btnReadCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadCsv.Location = new System.Drawing.Point(789, 59);
            this.btnReadCsv.Name = "btnReadCsv";
            this.btnReadCsv.Size = new System.Drawing.Size(88, 23);
            this.btnReadCsv.TabIndex = 5;
            this.btnReadCsv.Text = "Apply";
            this.btnReadCsv.UseVisualStyleBackColor = true;
            this.btnReadCsv.Click += new System.EventHandler(this.btnReadCsv_Click);
            // 
            // numSkipValue
            // 
            this.numSkipValue.Location = new System.Drawing.Point(476, 56);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(228, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Value parameter";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(457, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(13, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "<";
            // 
            // comboValueparam
            // 
            this.comboValueparam.FormattingEnabled = true;
            this.comboValueparam.Location = new System.Drawing.Point(330, 25);
            this.comboValueparam.Name = "comboValueparam";
            this.comboValueparam.Size = new System.Drawing.Size(121, 21);
            this.comboValueparam.TabIndex = 11;
            // 
            // comboSkipParam
            // 
            this.comboSkipParam.FormattingEnabled = true;
            this.comboSkipParam.Location = new System.Drawing.Point(330, 55);
            this.comboSkipParam.Name = "comboSkipParam";
            this.comboSkipParam.Size = new System.Drawing.Size(121, 21);
            this.comboSkipParam.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(228, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Skip if parameter";
            // 
            // groupLogfile
            // 
            this.groupLogfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLogfile.Controls.Add(this.txtColumnSeparator);
            this.groupLogfile.Controls.Add(this.label2);
            this.groupLogfile.Controls.Add(this.label1);
            this.groupLogfile.Controls.Add(this.txtLogFile);
            this.groupLogfile.Controls.Add(this.btnBrowseCsv);
            this.groupLogfile.Location = new System.Drawing.Point(8, 6);
            this.groupLogfile.Name = "groupLogfile";
            this.groupLogfile.Size = new System.Drawing.Size(884, 83);
            this.groupLogfile.TabIndex = 18;
            this.groupLogfile.TabStop = false;
            this.groupLogfile.Text = "Logfile";
            // 
            // txtColumnSeparator
            // 
            this.txtColumnSeparator.Location = new System.Drawing.Point(121, 19);
            this.txtColumnSeparator.Name = "txtColumnSeparator";
            this.txtColumnSeparator.Size = new System.Drawing.Size(41, 20);
            this.txtColumnSeparator.TabIndex = 4;
            this.txtColumnSeparator.Text = ",";
            this.txtColumnSeparator.TextChanged += new System.EventHandler(this.txtColumnSeparator_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Column separator:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log file:";
            // 
            // txtLogFile
            // 
            this.txtLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFile.Location = new System.Drawing.Point(59, 42);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(772, 20);
            this.txtLogFile.TabIndex = 0;
            this.txtLogFile.TextChanged += new System.EventHandler(this.txtCsvFile_TextChanged);
            // 
            // btnBrowseCsv
            // 
            this.btnBrowseCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseCsv.Location = new System.Drawing.Point(837, 40);
            this.btnBrowseCsv.Name = "btnBrowseCsv";
            this.btnBrowseCsv.Size = new System.Drawing.Size(41, 22);
            this.btnBrowseCsv.TabIndex = 2;
            this.btnBrowseCsv.Text = "...";
            this.btnBrowseCsv.UseVisualStyleBackColor = true;
            this.btnBrowseCsv.Click += new System.EventHandler(this.btnBrowseCsv_Click);
            // 
            // groupTemplate
            // 
            this.groupTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupTemplate.Controls.Add(this.btnLoadTable);
            this.groupTemplate.Controls.Add(this.btnGenRowHeaders);
            this.groupTemplate.Controls.Add(this.btnGenColHeaders);
            this.groupTemplate.Controls.Add(this.numDecimals);
            this.groupTemplate.Controls.Add(this.label13);
            this.groupTemplate.Controls.Add(this.btnApplyTemplate);
            this.groupTemplate.Controls.Add(this.txtRowHeaders);
            this.groupTemplate.Controls.Add(this.txtColHeaders);
            this.groupTemplate.Controls.Add(this.label12);
            this.groupTemplate.Controls.Add(this.label11);
            this.groupTemplate.Location = new System.Drawing.Point(235, 192);
            this.groupTemplate.Name = "groupTemplate";
            this.groupTemplate.Size = new System.Drawing.Size(658, 137);
            this.groupTemplate.TabIndex = 17;
            this.groupTemplate.TabStop = false;
            this.groupTemplate.Text = "Template";
            // 
            // btnLoadTable
            // 
            this.btnLoadTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadTable.Location = new System.Drawing.Point(459, 108);
            this.btnLoadTable.Name = "btnLoadTable";
            this.btnLoadTable.Size = new System.Drawing.Size(97, 23);
            this.btnLoadTable.TabIndex = 11;
            this.btnLoadTable.Text = "Load from bin";
            this.btnLoadTable.UseVisualStyleBackColor = true;
            this.btnLoadTable.Click += new System.EventHandler(this.btnLoadTable_Click);
            // 
            // btnGenRowHeaders
            // 
            this.btnGenRowHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenRowHeaders.Location = new System.Drawing.Point(581, 50);
            this.btnGenRowHeaders.Name = "btnGenRowHeaders";
            this.btnGenRowHeaders.Size = new System.Drawing.Size(67, 24);
            this.btnGenRowHeaders.TabIndex = 10;
            this.btnGenRowHeaders.Text = "Generate";
            this.btnGenRowHeaders.UseVisualStyleBackColor = true;
            this.btnGenRowHeaders.Click += new System.EventHandler(this.btnGenRowHeaders_Click);
            // 
            // btnGenColHeaders
            // 
            this.btnGenColHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenColHeaders.Location = new System.Drawing.Point(581, 20);
            this.btnGenColHeaders.Name = "btnGenColHeaders";
            this.btnGenColHeaders.Size = new System.Drawing.Size(67, 24);
            this.btnGenColHeaders.TabIndex = 9;
            this.btnGenColHeaders.Text = "Generate";
            this.btnGenColHeaders.UseVisualStyleBackColor = true;
            this.btnGenColHeaders.Click += new System.EventHandler(this.btnGenColHeaders_Click);
            // 
            // numDecimals
            // 
            this.numDecimals.Location = new System.Drawing.Point(121, 77);
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
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 79);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "Decimals";
            // 
            // btnApplyTemplate
            // 
            this.btnApplyTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyTemplate.Location = new System.Drawing.Point(562, 108);
            this.btnApplyTemplate.Name = "btnApplyTemplate";
            this.btnApplyTemplate.Size = new System.Drawing.Size(86, 23);
            this.btnApplyTemplate.TabIndex = 6;
            this.btnApplyTemplate.Text = "Apply";
            this.btnApplyTemplate.UseVisualStyleBackColor = true;
            this.btnApplyTemplate.Click += new System.EventHandler(this.btnApplyTemplate_Click);
            // 
            // txtRowHeaders
            // 
            this.txtRowHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRowHeaders.Location = new System.Drawing.Point(121, 53);
            this.txtRowHeaders.Name = "txtRowHeaders";
            this.txtRowHeaders.Size = new System.Drawing.Size(454, 20);
            this.txtRowHeaders.TabIndex = 5;
            // 
            // txtColHeaders
            // 
            this.txtColHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtColHeaders.Location = new System.Drawing.Point(121, 22);
            this.txtColHeaders.Name = "txtColHeaders";
            this.txtColHeaders.Size = new System.Drawing.Size(454, 20);
            this.txtColHeaders.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Row headers:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Column headers:";
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
            this.groupBox1.Location = new System.Drawing.Point(8, 192);
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
            // tabTemplateBin
            // 
            this.tabTemplateBin.Location = new System.Drawing.Point(4, 22);
            this.tabTemplateBin.Name = "tabTemplateBin";
            this.tabTemplateBin.Size = new System.Drawing.Size(901, 331);
            this.tabTemplateBin.TabIndex = 2;
            this.tabTemplateBin.Text = "Template bin";
            this.tabTemplateBin.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(909, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSettingsToolStripMenuItem,
            this.loadRecentToolStripMenuItem,
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
            // loadRecentToolStripMenuItem
            // 
            this.loadRecentToolStripMenuItem.Name = "loadRecentToolStripMenuItem";
            this.loadRecentToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadRecentToolStripMenuItem.Text = "Load Recent";
            this.loadRecentToolStripMenuItem.Click += new System.EventHandler(this.loadRecentToolStripMenuItem_Click);
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
            this.chkGetLiveData.Enabled = false;
            this.chkGetLiveData.Location = new System.Drawing.Point(144, 5);
            this.chkGetLiveData.Name = "chkGetLiveData";
            this.chkGetLiveData.Size = new System.Drawing.Size(149, 17);
            this.chkGetLiveData.TabIndex = 17;
            this.chkGetLiveData.Text = "Receive data from Logger";
            this.chkGetLiveData.UseVisualStyleBackColor = true;
            this.chkGetLiveData.CheckedChanged += new System.EventHandler(this.chkGetLiveData_CheckedChanged);
            // 
            // timerLiveData
            // 
            this.timerLiveData.Enabled = true;
            this.timerLiveData.Interval = 250;
            this.timerLiveData.Tick += new System.EventHandler(this.timerLiveData_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel1.Controls.Add(this.labelCellinfo);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(909, 641);
            this.splitContainer1.SplitterDistance = 280;
            this.splitContainer1.TabIndex = 18;
            // 
            // labelCellinfo
            // 
            this.labelCellinfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelCellinfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelCellinfo.Location = new System.Drawing.Point(0, 263);
            this.labelCellinfo.Name = "labelCellinfo";
            this.labelCellinfo.Size = new System.Drawing.Size(909, 17);
            this.labelCellinfo.TabIndex = 19;
            this.labelCellinfo.Text = "Cell info";
            // 
            // chkShowSettings
            // 
            this.chkShowSettings.AutoSize = true;
            this.chkShowSettings.Checked = true;
            this.chkShowSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowSettings.Location = new System.Drawing.Point(448, 6);
            this.chkShowSettings.Name = "chkShowSettings";
            this.chkShowSettings.Size = new System.Drawing.Size(92, 17);
            this.chkShowSettings.TabIndex = 20;
            this.chkShowSettings.Text = "Show settings";
            this.chkShowSettings.UseVisualStyleBackColor = true;
            this.chkShowSettings.CheckedChanged += new System.EventHandler(this.chkShowSettings_CheckedChanged);
            // 
            // tabSavedSettings
            // 
            this.tabSavedSettings.Controls.Add(this.btnLoad);
            this.tabSavedSettings.Controls.Add(this.btnSaveCurrent);
            this.tabSavedSettings.Controls.Add(this.dataGridViewSavedSettings);
            this.tabSavedSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSavedSettings.Name = "tabSavedSettings";
            this.tabSavedSettings.Size = new System.Drawing.Size(901, 331);
            this.tabSavedSettings.TabIndex = 3;
            this.tabSavedSettings.Text = "Saved settings";
            this.tabSavedSettings.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSavedSettings
            // 
            this.dataGridViewSavedSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewSavedSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSavedSettings.Location = new System.Drawing.Point(3, 33);
            this.dataGridViewSavedSettings.Name = "dataGridViewSavedSettings";
            this.dataGridViewSavedSettings.Size = new System.Drawing.Size(895, 298);
            this.dataGridViewSavedSettings.TabIndex = 0;
            // 
            // btnSaveCurrent
            // 
            this.btnSaveCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCurrent.Location = new System.Drawing.Point(721, 4);
            this.btnSaveCurrent.Name = "btnSaveCurrent";
            this.btnSaveCurrent.Size = new System.Drawing.Size(79, 28);
            this.btnSaveCurrent.TabIndex = 1;
            this.btnSaveCurrent.Text = "Add current";
            this.btnSaveCurrent.UseVisualStyleBackColor = true;
            this.btnSaveCurrent.Click += new System.EventHandler(this.btnSaveCurrent_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(806, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(87, 27);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // frmHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 665);
            this.Controls.Add(this.chkShowSettings);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.chkGetLiveData);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmHistogram";
            this.Text = "Histogram";
            this.Load += new System.EventHandler(this.frmHistogram_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.groupParams.ResumeLayout(false);
            this.groupParams.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSkipValue)).EndInit();
            this.groupLogfile.ResumeLayout(false);
            this.groupLogfile.PerformLayout();
            this.groupTemplate.ResumeLayout(false);
            this.groupTemplate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDecimals)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRangeMin)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabSavedSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSavedSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabControl tabControl1;
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
        private System.Windows.Forms.CheckBox chkGetLiveData;
        private System.Windows.Forms.GroupBox groupTemplate;
        private System.Windows.Forms.TextBox txtRowHeaders;
        private System.Windows.Forms.TextBox txtColHeaders;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnApplyTemplate;
        private System.Windows.Forms.NumericUpDown numDecimals;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnGenRowHeaders;
        private System.Windows.Forms.Button btnGenColHeaders;
        private System.Windows.Forms.Timer timerLiveData;
        private System.Windows.Forms.GroupBox groupParams;
        private System.Windows.Forms.GroupBox groupLogfile;
        private System.Windows.Forms.Button btnLoadTable;
        private System.Windows.Forms.Button btnRefreshParams;
        private System.Windows.Forms.ToolStripMenuItem loadRecentToolStripMenuItem;
        private System.Windows.Forms.TabPage tabTemplateBin;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label labelCellinfo;
        private System.Windows.Forms.CheckBox chkShowSettings;
        private System.Windows.Forms.TabPage tabSavedSettings;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSaveCurrent;
        private System.Windows.Forms.DataGridView dataGridViewSavedSettings;
    }
}