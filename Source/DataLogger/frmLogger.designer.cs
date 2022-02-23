
namespace UniversalPatcher
{
    partial class frmLogger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogger));
            this.timerShowData = new System.Windows.Forms.Timer(this.components);
            this.comboSerialPort = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.groupLogSettings = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkWriteLog = new System.Windows.Forms.CheckBox();
            this.btnBrowsLogFolder = new System.Windows.Forms.Button();
            this.txtLogSeparator = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelSeparator = new System.Windows.Forms.Label();
            this.txtLogFolder = new System.Windows.Forms.TextBox();
            this.listProfiles = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridLogData = new System.Windows.Forms.DataGridView();
            this.tabProfile = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtParamSearch = new System.Windows.Forms.TextBox();
            this.radioParamMath = new System.Windows.Forms.RadioButton();
            this.radioParamRam = new System.Windows.Forms.RadioButton();
            this.radioParamStd = new System.Windows.Forms.RadioButton();
            this.dataGridPidNames = new System.Windows.Forms.DataGridView();
            this.btnQueryPid = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dataGridLogProfile = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemove = new System.Windows.Forms.Button();
            this.tabAnalyzer = new System.Windows.Forms.TabPage();
            this.btnAnalyzerSaveCsv = new System.Windows.Forms.Button();
            this.btnClearAnalyzerGrid = new System.Windows.Forms.Button();
            this.chkHideHeartBeat = new System.Windows.Forms.CheckBox();
            this.dataGridAnalyzer = new System.Windows.Forms.DataGridView();
            this.tabDTC = new System.Windows.Forms.TabPage();
            this.groupDTC = new System.Windows.Forms.GroupBox();
            this.btnHistoryDTC = new System.Windows.Forms.Button();
            this.btnGetVINCode = new System.Windows.Forms.Button();
            this.btnCurrentDTC = new System.Windows.Forms.Button();
            this.comboModule = new System.Windows.Forms.ComboBox();
            this.chkDtcAllModules = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnClearCodes = new System.Windows.Forms.Button();
            this.dataGridDtcCodes = new System.Windows.Forms.DataGridView();
            this.Module = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupAdvanced = new System.Windows.Forms.GroupBox();
            this.chkFilterParamsByOS = new System.Windows.Forms.CheckBox();
            this.chkBusFilters = new System.Windows.Forms.CheckBox();
            this.chkPriority = new System.Windows.Forms.CheckBox();
            this.chkRawValues = new System.Windows.Forms.CheckBox();
            this.chkReverseSlotNumbers = new System.Windows.Forms.CheckBox();
            this.labelResponseMode = new System.Windows.Forms.Label();
            this.comboResponseMode = new System.Windows.Forms.ComboBox();
            this.groupHWSettings = new System.Windows.Forms.GroupBox();
            this.categories = new System.Windows.Forms.GroupBox();
            this.j2534RadioButton = new System.Windows.Forms.RadioButton();
            this.serialRadioButton = new System.Windows.Forms.RadioButton();
            this.j2534OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.j2534DeviceList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.serialOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.comboBaudRate = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkFTDI = new System.Windows.Forms.CheckBox();
            this.comboSerialDeviceType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSendBus = new System.Windows.Forms.TextBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterSupportedPidsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectDisconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.timerAnalyzer = new System.Windows.Forms.Timer(this.components);
            this.labelConnected = new System.Windows.Forms.Label();
            this.timerSearchParams = new System.Windows.Forms.Timer(this.components);
            this.txtVPWmessages = new System.Windows.Forms.RichTextBox();
            this.chkEnableConsole = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.groupLogSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogData)).BeginInit();
            this.tabProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPidNames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogProfile)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabAnalyzer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAnalyzer)).BeginInit();
            this.tabDTC.SuspendLayout();
            this.groupDTC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDtcCodes)).BeginInit();
            this.tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupAdvanced.SuspendLayout();
            this.groupHWSettings.SuspendLayout();
            this.categories.SuspendLayout();
            this.j2534OptionsGroupBox.SuspendLayout();
            this.serialOptionsGroupBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerShowData
            // 
            this.timerShowData.Interval = 300;
            this.timerShowData.Tick += new System.EventHandler(this.timerShowData_Tick);
            // 
            // comboSerialPort
            // 
            this.comboSerialPort.FormattingEnabled = true;
            this.comboSerialPort.Location = new System.Drawing.Point(81, 50);
            this.comboSerialPort.Name = "comboSerialPort";
            this.comboSerialPort.Size = new System.Drawing.Size(351, 21);
            this.comboSerialPort.TabIndex = 2;
            this.comboSerialPort.SelectedIndexChanged += new System.EventHandler(this.comboSerialPort_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Controls.Add(this.tabProfile);
            this.tabControl1.Controls.Add(this.tabAnalyzer);
            this.tabControl1.Controls.Add(this.tabDTC);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(912, 423);
            this.tabControl1.TabIndex = 3;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.groupLogSettings);
            this.tabLog.Controls.Add(this.dataGridLogData);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(904, 397);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // groupLogSettings
            // 
            this.groupLogSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupLogSettings.Controls.Add(this.groupBox1);
            this.groupLogSettings.Controls.Add(this.listProfiles);
            this.groupLogSettings.Controls.Add(this.label3);
            this.groupLogSettings.Location = new System.Drawing.Point(-1, 0);
            this.groupLogSettings.Name = "groupLogSettings";
            this.groupLogSettings.Size = new System.Drawing.Size(438, 401);
            this.groupLogSettings.TabIndex = 31;
            this.groupLogSettings.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkWriteLog);
            this.groupBox1.Controls.Add(this.btnBrowsLogFolder);
            this.groupBox1.Controls.Add(this.txtLogSeparator);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.labelSeparator);
            this.groupBox1.Controls.Add(this.txtLogFolder);
            this.groupBox1.Location = new System.Drawing.Point(9, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(421, 102);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logfile";
            // 
            // chkWriteLog
            // 
            this.chkWriteLog.AutoSize = true;
            this.chkWriteLog.Checked = true;
            this.chkWriteLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriteLog.Location = new System.Drawing.Point(9, 73);
            this.chkWriteLog.Name = "chkWriteLog";
            this.chkWriteLog.Size = new System.Drawing.Size(68, 17);
            this.chkWriteLog.TabIndex = 9;
            this.chkWriteLog.Text = "Write log";
            this.chkWriteLog.UseVisualStyleBackColor = true;
            // 
            // btnBrowsLogFolder
            // 
            this.btnBrowsLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsLogFolder.Location = new System.Drawing.Point(382, 40);
            this.btnBrowsLogFolder.Name = "btnBrowsLogFolder";
            this.btnBrowsLogFolder.Size = new System.Drawing.Size(33, 23);
            this.btnBrowsLogFolder.TabIndex = 12;
            this.btnBrowsLogFolder.Text = "...";
            this.btnBrowsLogFolder.UseVisualStyleBackColor = true;
            this.btnBrowsLogFolder.Click += new System.EventHandler(this.btnBrowsLogFolder_Click);
            // 
            // txtLogSeparator
            // 
            this.txtLogSeparator.Location = new System.Drawing.Point(144, 71);
            this.txtLogSeparator.Name = "txtLogSeparator";
            this.txtLogSeparator.Size = new System.Drawing.Size(30, 20);
            this.txtLogSeparator.TabIndex = 14;
            this.txtLogSeparator.Text = ",";
            this.txtLogSeparator.TextChanged += new System.EventHandler(this.txtLogSeparator_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Logfolder:";
            // 
            // labelSeparator
            // 
            this.labelSeparator.AutoSize = true;
            this.labelSeparator.Location = new System.Drawing.Point(82, 74);
            this.labelSeparator.Name = "labelSeparator";
            this.labelSeparator.Size = new System.Drawing.Size(56, 13);
            this.labelSeparator.TabIndex = 13;
            this.labelSeparator.Text = "Separator:";
            // 
            // txtLogFolder
            // 
            this.txtLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFolder.Location = new System.Drawing.Point(6, 42);
            this.txtLogFolder.Name = "txtLogFolder";
            this.txtLogFolder.Size = new System.Drawing.Size(370, 20);
            this.txtLogFolder.TabIndex = 10;
            // 
            // listProfiles
            // 
            this.listProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listProfiles.FormattingEnabled = true;
            this.listProfiles.Location = new System.Drawing.Point(9, 136);
            this.listProfiles.Name = "listProfiles";
            this.listProfiles.Size = new System.Drawing.Size(423, 251);
            this.listProfiles.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Profile:";
            // 
            // dataGridLogData
            // 
            this.dataGridLogData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridLogData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLogData.Location = new System.Drawing.Point(443, 3);
            this.dataGridLogData.Name = "dataGridLogData";
            this.dataGridLogData.Size = new System.Drawing.Size(458, 391);
            this.dataGridLogData.TabIndex = 8;
            // 
            // tabProfile
            // 
            this.tabProfile.Controls.Add(this.splitContainer1);
            this.tabProfile.Location = new System.Drawing.Point(4, 22);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Size = new System.Drawing.Size(904, 397);
            this.tabProfile.TabIndex = 2;
            this.tabProfile.Text = "Profile";
            this.tabProfile.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridPidNames);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnQueryPid);
            this.splitContainer1.Panel2.Controls.Add(this.btnAdd);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridLogProfile);
            this.splitContainer1.Panel2.Controls.Add(this.btnRemove);
            this.splitContainer1.Size = new System.Drawing.Size(904, 397);
            this.splitContainer1.SplitterDistance = 368;
            this.splitContainer1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtParamSearch);
            this.groupBox2.Controls.Add(this.radioParamMath);
            this.groupBox2.Controls.Add(this.radioParamRam);
            this.groupBox2.Controls.Add(this.radioParamStd);
            this.groupBox2.Location = new System.Drawing.Point(4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(363, 31);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // txtParamSearch
            // 
            this.txtParamSearch.Location = new System.Drawing.Point(203, 8);
            this.txtParamSearch.Name = "txtParamSearch";
            this.txtParamSearch.Size = new System.Drawing.Size(154, 20);
            this.txtParamSearch.TabIndex = 3;
            this.txtParamSearch.Text = "Search...";
            // 
            // radioParamMath
            // 
            this.radioParamMath.AutoSize = true;
            this.radioParamMath.Location = new System.Drawing.Point(135, 9);
            this.radioParamMath.Name = "radioParamMath";
            this.radioParamMath.Size = new System.Drawing.Size(49, 17);
            this.radioParamMath.TabIndex = 2;
            this.radioParamMath.TabStop = true;
            this.radioParamMath.Text = "Math";
            this.radioParamMath.UseVisualStyleBackColor = true;
            this.radioParamMath.CheckedChanged += new System.EventHandler(this.radioParamMath_CheckedChanged);
            // 
            // radioParamRam
            // 
            this.radioParamRam.AutoSize = true;
            this.radioParamRam.Location = new System.Drawing.Point(80, 9);
            this.radioParamRam.Name = "radioParamRam";
            this.radioParamRam.Size = new System.Drawing.Size(49, 17);
            this.radioParamRam.TabIndex = 1;
            this.radioParamRam.Text = "RAM";
            this.radioParamRam.UseVisualStyleBackColor = true;
            this.radioParamRam.CheckedChanged += new System.EventHandler(this.radioParamRam_CheckedChanged);
            // 
            // radioParamStd
            // 
            this.radioParamStd.AutoSize = true;
            this.radioParamStd.Checked = true;
            this.radioParamStd.Location = new System.Drawing.Point(6, 9);
            this.radioParamStd.Name = "radioParamStd";
            this.radioParamStd.Size = new System.Drawing.Size(68, 17);
            this.radioParamStd.TabIndex = 0;
            this.radioParamStd.TabStop = true;
            this.radioParamStd.Text = "Standard";
            this.radioParamStd.UseVisualStyleBackColor = true;
            this.radioParamStd.CheckedChanged += new System.EventHandler(this.radioParamStd_CheckedChanged);
            // 
            // dataGridPidNames
            // 
            this.dataGridPidNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridPidNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPidNames.Location = new System.Drawing.Point(0, 35);
            this.dataGridPidNames.Name = "dataGridPidNames";
            this.dataGridPidNames.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPidNames.Size = new System.Drawing.Size(368, 363);
            this.dataGridPidNames.TabIndex = 1;
            // 
            // btnQueryPid
            // 
            this.btnQueryPid.Location = new System.Drawing.Point(3, 316);
            this.btnQueryPid.Name = "btnQueryPid";
            this.btnQueryPid.Size = new System.Drawing.Size(28, 27);
            this.btnQueryPid.TabIndex = 4;
            this.btnQueryPid.Text = "?";
            this.btnQueryPid.UseVisualStyleBackColor = true;
            this.btnQueryPid.Click += new System.EventHandler(this.btnQueryPid_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 76);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(28, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dataGridLogProfile
            // 
            this.dataGridLogProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridLogProfile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLogProfile.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridLogProfile.Location = new System.Drawing.Point(37, 0);
            this.dataGridLogProfile.Name = "dataGridLogProfile";
            this.dataGridLogProfile.Size = new System.Drawing.Size(492, 394);
            this.dataGridLogProfile.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.cutRowToolStripMenuItem,
            this.copyRowToolStripMenuItem,
            this.pasteRowToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(126, 92);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // cutRowToolStripMenuItem
            // 
            this.cutRowToolStripMenuItem.Name = "cutRowToolStripMenuItem";
            this.cutRowToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.cutRowToolStripMenuItem.Text = "Cut row";
            this.cutRowToolStripMenuItem.Click += new System.EventHandler(this.cutRowToolStripMenuItem_Click);
            // 
            // copyRowToolStripMenuItem
            // 
            this.copyRowToolStripMenuItem.Name = "copyRowToolStripMenuItem";
            this.copyRowToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.copyRowToolStripMenuItem.Text = "Copy row";
            this.copyRowToolStripMenuItem.Click += new System.EventHandler(this.copyRowToolStripMenuItem_Click);
            // 
            // pasteRowToolStripMenuItem
            // 
            this.pasteRowToolStripMenuItem.Name = "pasteRowToolStripMenuItem";
            this.pasteRowToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.pasteRowToolStripMenuItem.Text = "Paste row";
            this.pasteRowToolStripMenuItem.Click += new System.EventHandler(this.pasteRowToolStripMenuItem_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(3, 114);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(28, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // tabAnalyzer
            // 
            this.tabAnalyzer.Controls.Add(this.btnAnalyzerSaveCsv);
            this.tabAnalyzer.Controls.Add(this.btnClearAnalyzerGrid);
            this.tabAnalyzer.Controls.Add(this.chkHideHeartBeat);
            this.tabAnalyzer.Controls.Add(this.dataGridAnalyzer);
            this.tabAnalyzer.Location = new System.Drawing.Point(4, 22);
            this.tabAnalyzer.Name = "tabAnalyzer";
            this.tabAnalyzer.Size = new System.Drawing.Size(904, 397);
            this.tabAnalyzer.TabIndex = 4;
            this.tabAnalyzer.Text = "Analyzer";
            this.tabAnalyzer.UseVisualStyleBackColor = true;
            // 
            // btnAnalyzerSaveCsv
            // 
            this.btnAnalyzerSaveCsv.Location = new System.Drawing.Point(198, 2);
            this.btnAnalyzerSaveCsv.Name = "btnAnalyzerSaveCsv";
            this.btnAnalyzerSaveCsv.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyzerSaveCsv.TabIndex = 6;
            this.btnAnalyzerSaveCsv.Text = "Save CSV";
            this.btnAnalyzerSaveCsv.UseVisualStyleBackColor = true;
            this.btnAnalyzerSaveCsv.Click += new System.EventHandler(this.btnAnalyzerSaveCsv_Click);
            // 
            // btnClearAnalyzerGrid
            // 
            this.btnClearAnalyzerGrid.Location = new System.Drawing.Point(117, 3);
            this.btnClearAnalyzerGrid.Name = "btnClearAnalyzerGrid";
            this.btnClearAnalyzerGrid.Size = new System.Drawing.Size(75, 21);
            this.btnClearAnalyzerGrid.TabIndex = 5;
            this.btnClearAnalyzerGrid.Text = "Clear";
            this.btnClearAnalyzerGrid.UseVisualStyleBackColor = true;
            this.btnClearAnalyzerGrid.Click += new System.EventHandler(this.btnClearAnalyzerGrid_Click);
            // 
            // chkHideHeartBeat
            // 
            this.chkHideHeartBeat.AutoSize = true;
            this.chkHideHeartBeat.Checked = true;
            this.chkHideHeartBeat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHideHeartBeat.Location = new System.Drawing.Point(8, 8);
            this.chkHideHeartBeat.Name = "chkHideHeartBeat";
            this.chkHideHeartBeat.Size = new System.Drawing.Size(98, 17);
            this.chkHideHeartBeat.TabIndex = 4;
            this.chkHideHeartBeat.Text = "Hide Heartbeat";
            this.chkHideHeartBeat.UseVisualStyleBackColor = true;
            // 
            // dataGridAnalyzer
            // 
            this.dataGridAnalyzer.AllowUserToAddRows = false;
            this.dataGridAnalyzer.AllowUserToDeleteRows = false;
            this.dataGridAnalyzer.AllowUserToOrderColumns = true;
            this.dataGridAnalyzer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridAnalyzer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridAnalyzer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridAnalyzer.Location = new System.Drawing.Point(0, 28);
            this.dataGridAnalyzer.Name = "dataGridAnalyzer";
            this.dataGridAnalyzer.Size = new System.Drawing.Size(902, 369);
            this.dataGridAnalyzer.TabIndex = 3;
            // 
            // tabDTC
            // 
            this.tabDTC.Controls.Add(this.groupDTC);
            this.tabDTC.Controls.Add(this.dataGridDtcCodes);
            this.tabDTC.Location = new System.Drawing.Point(4, 22);
            this.tabDTC.Name = "tabDTC";
            this.tabDTC.Size = new System.Drawing.Size(904, 397);
            this.tabDTC.TabIndex = 5;
            this.tabDTC.Text = "DTC";
            this.tabDTC.UseVisualStyleBackColor = true;
            // 
            // groupDTC
            // 
            this.groupDTC.Controls.Add(this.btnHistoryDTC);
            this.groupDTC.Controls.Add(this.btnGetVINCode);
            this.groupDTC.Controls.Add(this.btnCurrentDTC);
            this.groupDTC.Controls.Add(this.comboModule);
            this.groupDTC.Controls.Add(this.chkDtcAllModules);
            this.groupDTC.Controls.Add(this.label8);
            this.groupDTC.Controls.Add(this.btnClearCodes);
            this.groupDTC.Location = new System.Drawing.Point(3, 3);
            this.groupDTC.Name = "groupDTC";
            this.groupDTC.Size = new System.Drawing.Size(173, 202);
            this.groupDTC.TabIndex = 9;
            this.groupDTC.TabStop = false;
            // 
            // btnHistoryDTC
            // 
            this.btnHistoryDTC.Location = new System.Drawing.Point(6, 96);
            this.btnHistoryDTC.Name = "btnHistoryDTC";
            this.btnHistoryDTC.Size = new System.Drawing.Size(162, 29);
            this.btnHistoryDTC.TabIndex = 1;
            this.btnHistoryDTC.Text = "Get History DTC codes";
            this.btnHistoryDTC.UseVisualStyleBackColor = true;
            this.btnHistoryDTC.Click += new System.EventHandler(this.btnHistoryDTC_Click);
            // 
            // btnGetVINCode
            // 
            this.btnGetVINCode.Location = new System.Drawing.Point(6, 166);
            this.btnGetVINCode.Name = "btnGetVINCode";
            this.btnGetVINCode.Size = new System.Drawing.Size(162, 29);
            this.btnGetVINCode.TabIndex = 8;
            this.btnGetVINCode.Text = "Get VIN Code";
            this.btnGetVINCode.UseVisualStyleBackColor = true;
            this.btnGetVINCode.Click += new System.EventHandler(this.btnGetVINCode_Click);
            // 
            // btnCurrentDTC
            // 
            this.btnCurrentDTC.Location = new System.Drawing.Point(6, 61);
            this.btnCurrentDTC.Name = "btnCurrentDTC";
            this.btnCurrentDTC.Size = new System.Drawing.Size(162, 29);
            this.btnCurrentDTC.TabIndex = 0;
            this.btnCurrentDTC.Text = "Get current DTC codes";
            this.btnCurrentDTC.UseVisualStyleBackColor = true;
            this.btnCurrentDTC.Click += new System.EventHandler(this.btnCurrentDTC_Click);
            // 
            // comboModule
            // 
            this.comboModule.FormattingEnabled = true;
            this.comboModule.Location = new System.Drawing.Point(63, 34);
            this.comboModule.Name = "comboModule";
            this.comboModule.Size = new System.Drawing.Size(105, 21);
            this.comboModule.TabIndex = 2;
            // 
            // chkDtcAllModules
            // 
            this.chkDtcAllModules.AutoSize = true;
            this.chkDtcAllModules.Location = new System.Drawing.Point(63, 12);
            this.chkDtcAllModules.Name = "chkDtcAllModules";
            this.chkDtcAllModules.Size = new System.Drawing.Size(79, 17);
            this.chkDtcAllModules.TabIndex = 6;
            this.chkDtcAllModules.Text = "All modules";
            this.chkDtcAllModules.UseVisualStyleBackColor = true;
            this.chkDtcAllModules.CheckedChanged += new System.EventHandler(this.chkDtcAllModules_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Module:";
            // 
            // btnClearCodes
            // 
            this.btnClearCodes.Location = new System.Drawing.Point(6, 131);
            this.btnClearCodes.Name = "btnClearCodes";
            this.btnClearCodes.Size = new System.Drawing.Size(162, 29);
            this.btnClearCodes.TabIndex = 5;
            this.btnClearCodes.Text = "Clear DTC codes";
            this.btnClearCodes.UseVisualStyleBackColor = true;
            this.btnClearCodes.Click += new System.EventHandler(this.btnClearCodes_Click);
            // 
            // dataGridDtcCodes
            // 
            this.dataGridDtcCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridDtcCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridDtcCodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Module,
            this.Code,
            this.Description,
            this.Status});
            this.dataGridDtcCodes.Location = new System.Drawing.Point(176, 0);
            this.dataGridDtcCodes.Name = "dataGridDtcCodes";
            this.dataGridDtcCodes.Size = new System.Drawing.Size(724, 396);
            this.dataGridDtcCodes.TabIndex = 7;
            // 
            // Module
            // 
            this.Module.HeaderText = "Module";
            this.Module.Name = "Module";
            // 
            // Code
            // 
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.Width = 300;
            // 
            // Status
            // 
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.Width = 300;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.splitContainer3);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(904, 397);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.btnConnect);
            this.splitContainer3.Panel1.Controls.Add(this.groupAdvanced);
            this.splitContainer3.Panel1.Controls.Add(this.groupHWSettings);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtVPWmessages);
            this.splitContainer3.Panel2.Controls.Add(this.txtSendBus);
            this.splitContainer3.Size = new System.Drawing.Size(904, 397);
            this.splitContainer3.SplitterDistance = 457;
            this.splitContainer3.TabIndex = 35;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(354, 262);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 56);
            this.btnConnect.TabIndex = 21;
            this.btnConnect.Text = "Connect/ Disconnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupAdvanced
            // 
            this.groupAdvanced.Controls.Add(this.chkEnableConsole);
            this.groupAdvanced.Controls.Add(this.chkFilterParamsByOS);
            this.groupAdvanced.Controls.Add(this.chkBusFilters);
            this.groupAdvanced.Controls.Add(this.chkPriority);
            this.groupAdvanced.Controls.Add(this.chkRawValues);
            this.groupAdvanced.Controls.Add(this.chkReverseSlotNumbers);
            this.groupAdvanced.Controls.Add(this.labelResponseMode);
            this.groupAdvanced.Controls.Add(this.comboResponseMode);
            this.groupAdvanced.Location = new System.Drawing.Point(3, 242);
            this.groupAdvanced.Name = "groupAdvanced";
            this.groupAdvanced.Size = new System.Drawing.Size(345, 102);
            this.groupAdvanced.TabIndex = 34;
            this.groupAdvanced.TabStop = false;
            this.groupAdvanced.Text = "Advanced";
            // 
            // chkFilterParamsByOS
            // 
            this.chkFilterParamsByOS.AutoSize = true;
            this.chkFilterParamsByOS.Checked = true;
            this.chkFilterParamsByOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilterParamsByOS.Location = new System.Drawing.Point(9, 79);
            this.chkFilterParamsByOS.Name = "chkFilterParamsByOS";
            this.chkFilterParamsByOS.Size = new System.Drawing.Size(135, 17);
            this.chkFilterParamsByOS.TabIndex = 36;
            this.chkFilterParamsByOS.Text = "Filter parameters by OS";
            this.chkFilterParamsByOS.UseVisualStyleBackColor = true;
            // 
            // chkBusFilters
            // 
            this.chkBusFilters.AutoSize = true;
            this.chkBusFilters.Location = new System.Drawing.Point(9, 59);
            this.chkBusFilters.Name = "chkBusFilters";
            this.chkBusFilters.Size = new System.Drawing.Size(100, 17);
            this.chkBusFilters.TabIndex = 35;
            this.chkBusFilters.Text = "Use BUS Filters";
            this.chkBusFilters.UseVisualStyleBackColor = true;
            this.chkBusFilters.CheckedChanged += new System.EventHandler(this.chkBusFilters_CheckedChanged);
            // 
            // chkPriority
            // 
            this.chkPriority.AutoSize = true;
            this.chkPriority.Location = new System.Drawing.Point(191, 56);
            this.chkPriority.Name = "chkPriority";
            this.chkPriority.Size = new System.Drawing.Size(79, 17);
            this.chkPriority.TabIndex = 34;
            this.chkPriority.Text = "Use Priority";
            this.chkPriority.UseVisualStyleBackColor = true;
            this.chkPriority.CheckedChanged += new System.EventHandler(this.chkPriority_CheckedChanged);
            // 
            // chkRawValues
            // 
            this.chkRawValues.AutoSize = true;
            this.chkRawValues.Location = new System.Drawing.Point(9, 19);
            this.chkRawValues.Name = "chkRawValues";
            this.chkRawValues.Size = new System.Drawing.Size(82, 17);
            this.chkRawValues.TabIndex = 26;
            this.chkRawValues.Text = "Raw values";
            this.chkRawValues.UseVisualStyleBackColor = true;
            // 
            // chkReverseSlotNumbers
            // 
            this.chkReverseSlotNumbers.AutoSize = true;
            this.chkReverseSlotNumbers.Checked = true;
            this.chkReverseSlotNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReverseSlotNumbers.Location = new System.Drawing.Point(9, 39);
            this.chkReverseSlotNumbers.Name = "chkReverseSlotNumbers";
            this.chkReverseSlotNumbers.Size = new System.Drawing.Size(149, 17);
            this.chkReverseSlotNumbers.TabIndex = 27;
            this.chkReverseSlotNumbers.Text = "Slot numbers start from FE";
            this.chkReverseSlotNumbers.UseVisualStyleBackColor = true;
            // 
            // labelResponseMode
            // 
            this.labelResponseMode.AutoSize = true;
            this.labelResponseMode.Location = new System.Drawing.Point(191, 15);
            this.labelResponseMode.Name = "labelResponseMode";
            this.labelResponseMode.Size = new System.Drawing.Size(88, 13);
            this.labelResponseMode.TabIndex = 24;
            this.labelResponseMode.Text = "Response Mode:";
            // 
            // comboResponseMode
            // 
            this.comboResponseMode.FormattingEnabled = true;
            this.comboResponseMode.Location = new System.Drawing.Point(191, 29);
            this.comboResponseMode.Name = "comboResponseMode";
            this.comboResponseMode.Size = new System.Drawing.Size(149, 21);
            this.comboResponseMode.TabIndex = 23;
            this.comboResponseMode.SelectedIndexChanged += new System.EventHandler(this.comboResponseMode_SelectedIndexChanged);
            // 
            // groupHWSettings
            // 
            this.groupHWSettings.Controls.Add(this.categories);
            this.groupHWSettings.Controls.Add(this.j2534OptionsGroupBox);
            this.groupHWSettings.Controls.Add(this.serialOptionsGroupBox);
            this.groupHWSettings.Location = new System.Drawing.Point(3, 3);
            this.groupHWSettings.Name = "groupHWSettings";
            this.groupHWSettings.Size = new System.Drawing.Size(449, 233);
            this.groupHWSettings.TabIndex = 22;
            this.groupHWSettings.TabStop = false;
            // 
            // categories
            // 
            this.categories.Controls.Add(this.j2534RadioButton);
            this.categories.Controls.Add(this.serialRadioButton);
            this.categories.Location = new System.Drawing.Point(6, 6);
            this.categories.Name = "categories";
            this.categories.Size = new System.Drawing.Size(174, 71);
            this.categories.TabIndex = 18;
            this.categories.TabStop = false;
            this.categories.Text = "Device &Category";
            // 
            // j2534RadioButton
            // 
            this.j2534RadioButton.AutoSize = true;
            this.j2534RadioButton.Location = new System.Drawing.Point(7, 46);
            this.j2534RadioButton.Name = "j2534RadioButton";
            this.j2534RadioButton.Size = new System.Drawing.Size(91, 17);
            this.j2534RadioButton.TabIndex = 1;
            this.j2534RadioButton.Text = "&J2534 Device";
            this.j2534RadioButton.UseVisualStyleBackColor = true;
            this.j2534RadioButton.CheckedChanged += new System.EventHandler(this.j2534RadioButton_CheckedChanged);
            // 
            // serialRadioButton
            // 
            this.serialRadioButton.AutoSize = true;
            this.serialRadioButton.Checked = true;
            this.serialRadioButton.Location = new System.Drawing.Point(7, 20);
            this.serialRadioButton.Name = "serialRadioButton";
            this.serialRadioButton.Size = new System.Drawing.Size(110, 17);
            this.serialRadioButton.TabIndex = 0;
            this.serialRadioButton.TabStop = true;
            this.serialRadioButton.Text = "&Serial Port Device";
            this.serialRadioButton.UseVisualStyleBackColor = true;
            this.serialRadioButton.CheckedChanged += new System.EventHandler(this.serialRadioButton_CheckedChanged);
            // 
            // j2534OptionsGroupBox
            // 
            this.j2534OptionsGroupBox.Controls.Add(this.j2534DeviceList);
            this.j2534OptionsGroupBox.Controls.Add(this.label5);
            this.j2534OptionsGroupBox.Location = new System.Drawing.Point(185, 6);
            this.j2534OptionsGroupBox.Name = "j2534OptionsGroupBox";
            this.j2534OptionsGroupBox.Size = new System.Drawing.Size(258, 72);
            this.j2534OptionsGroupBox.TabIndex = 19;
            this.j2534OptionsGroupBox.TabStop = false;
            this.j2534OptionsGroupBox.Text = "J2534 Device Options";
            // 
            // j2534DeviceList
            // 
            this.j2534DeviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.j2534DeviceList.FormattingEnabled = true;
            this.j2534DeviceList.Location = new System.Drawing.Point(7, 37);
            this.j2534DeviceList.Name = "j2534DeviceList";
            this.j2534DeviceList.Size = new System.Drawing.Size(245, 21);
            this.j2534DeviceList.TabIndex = 1;
            this.j2534DeviceList.SelectedIndexChanged += new System.EventHandler(this.j2534DeviceList_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "&Device Type";
            // 
            // serialOptionsGroupBox
            // 
            this.serialOptionsGroupBox.Controls.Add(this.comboBaudRate);
            this.serialOptionsGroupBox.Controls.Add(this.label7);
            this.serialOptionsGroupBox.Controls.Add(this.chkFTDI);
            this.serialOptionsGroupBox.Controls.Add(this.comboSerialDeviceType);
            this.serialOptionsGroupBox.Controls.Add(this.label1);
            this.serialOptionsGroupBox.Controls.Add(this.comboSerialPort);
            this.serialOptionsGroupBox.Controls.Add(this.label4);
            this.serialOptionsGroupBox.Location = new System.Drawing.Point(5, 83);
            this.serialOptionsGroupBox.Name = "serialOptionsGroupBox";
            this.serialOptionsGroupBox.Size = new System.Drawing.Size(438, 141);
            this.serialOptionsGroupBox.TabIndex = 20;
            this.serialOptionsGroupBox.TabStop = false;
            this.serialOptionsGroupBox.Text = "Serialport options";
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.FormattingEnabled = true;
            this.comboBaudRate.Location = new System.Drawing.Point(80, 107);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(351, 21);
            this.comboBaudRate.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Baudrate:";
            // 
            // chkFTDI
            // 
            this.chkFTDI.AutoSize = true;
            this.chkFTDI.Location = new System.Drawing.Point(10, 19);
            this.chkFTDI.Name = "chkFTDI";
            this.chkFTDI.Size = new System.Drawing.Size(72, 17);
            this.chkFTDI.TabIndex = 18;
            this.chkFTDI.Text = "Use FTDI";
            this.chkFTDI.UseVisualStyleBackColor = true;
            this.chkFTDI.CheckedChanged += new System.EventHandler(this.chkFTDI_CheckedChanged);
            // 
            // comboSerialDeviceType
            // 
            this.comboSerialDeviceType.FormattingEnabled = true;
            this.comboSerialDeviceType.Location = new System.Drawing.Point(81, 78);
            this.comboSerialDeviceType.Name = "comboSerialDeviceType";
            this.comboSerialDeviceType.Size = new System.Drawing.Size(351, 21);
            this.comboSerialDeviceType.TabIndex = 17;
            this.comboSerialDeviceType.SelectedIndexChanged += new System.EventHandler(this.comboSerialDeviceType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Device Type";
            // 
            // txtSendBus
            // 
            this.txtSendBus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSendBus.Location = new System.Drawing.Point(2, 374);
            this.txtSendBus.Name = "txtSendBus";
            this.txtSendBus.Size = new System.Drawing.Size(436, 20);
            this.txtSendBus.TabIndex = 1;
            // 
            // labelProgress
            // 
            this.labelProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelProgress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.Location = new System.Drawing.Point(437, 26);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(468, 57);
            this.labelProgress.TabIndex = 22;
            this.labelProgress.Text = "idle";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(4, 26);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(421, 32);
            this.btnStartStop.TabIndex = 5;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(912, 107);
            this.txtResult.TabIndex = 6;
            this.txtResult.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(912, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadProfileToolStripMenuItem,
            this.saveProfileToolStripMenuItem,
            this.saveProfileAsToolStripMenuItem,
            this.newProfileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadProfileToolStripMenuItem
            // 
            this.loadProfileToolStripMenuItem.Name = "loadProfileToolStripMenuItem";
            this.loadProfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.loadProfileToolStripMenuItem.Text = "Load profile";
            this.loadProfileToolStripMenuItem.Click += new System.EventHandler(this.loadProfileToolStripMenuItem_Click);
            // 
            // saveProfileToolStripMenuItem
            // 
            this.saveProfileToolStripMenuItem.Name = "saveProfileToolStripMenuItem";
            this.saveProfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveProfileToolStripMenuItem.Text = "Save profile";
            this.saveProfileToolStripMenuItem.Click += new System.EventHandler(this.saveProfileToolStripMenuItem_Click);
            // 
            // saveProfileAsToolStripMenuItem
            // 
            this.saveProfileAsToolStripMenuItem.Name = "saveProfileAsToolStripMenuItem";
            this.saveProfileAsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveProfileAsToolStripMenuItem.Text = "Save profile as...";
            this.saveProfileAsToolStripMenuItem.Click += new System.EventHandler(this.saveProfileAsToolStripMenuItem_Click);
            // 
            // newProfileToolStripMenuItem
            // 
            this.newProfileToolStripMenuItem.Name = "newProfileToolStripMenuItem";
            this.newProfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.newProfileToolStripMenuItem.Text = "New profile";
            this.newProfileToolStripMenuItem.Click += new System.EventHandler(this.newProfileToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterSupportedPidsToolStripMenuItem,
            this.connectDisconnectToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionToolStripMenuItem.Text = "Action";
            // 
            // filterSupportedPidsToolStripMenuItem
            // 
            this.filterSupportedPidsToolStripMenuItem.Name = "filterSupportedPidsToolStripMenuItem";
            this.filterSupportedPidsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.filterSupportedPidsToolStripMenuItem.Text = "Filter pids by BIN file";
            this.filterSupportedPidsToolStripMenuItem.Click += new System.EventHandler(this.filterSupportedPidsToolStripMenuItem_Click);
            // 
            // connectDisconnectToolStripMenuItem
            // 
            this.connectDisconnectToolStripMenuItem.Name = "connectDisconnectToolStripMenuItem";
            this.connectDisconnectToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.connectDisconnectToolStripMenuItem.Text = "Connect/Disconnect";
            this.connectDisconnectToolStripMenuItem.Click += new System.EventHandler(this.connectDisconnectToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 64);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtResult);
            this.splitContainer2.Size = new System.Drawing.Size(912, 534);
            this.splitContainer2.SplitterDistance = 423;
            this.splitContainer2.TabIndex = 5;
            // 
            // timerAnalyzer
            // 
            this.timerAnalyzer.Tick += new System.EventHandler(this.timerAnalyzer_Tick);
            // 
            // labelConnected
            // 
            this.labelConnected.AutoSize = true;
            this.labelConnected.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelConnected.Location = new System.Drawing.Point(196, 6);
            this.labelConnected.Name = "labelConnected";
            this.labelConnected.Size = new System.Drawing.Size(75, 15);
            this.labelConnected.TabIndex = 24;
            this.labelConnected.Text = "Disconnected";
            // 
            // timerSearchParams
            // 
            this.timerSearchParams.Tick += new System.EventHandler(this.timerSearchParams_Tick);
            // 
            // txtVPWmessages
            // 
            this.txtVPWmessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVPWmessages.HideSelection = false;
            this.txtVPWmessages.Location = new System.Drawing.Point(2, 3);
            this.txtVPWmessages.Name = "txtVPWmessages";
            this.txtVPWmessages.Size = new System.Drawing.Size(435, 371);
            this.txtVPWmessages.TabIndex = 2;
            this.txtVPWmessages.Text = "";
            // 
            // chkEnableConsole
            // 
            this.chkEnableConsole.AutoSize = true;
            this.chkEnableConsole.Location = new System.Drawing.Point(191, 79);
            this.chkEnableConsole.Name = "chkEnableConsole";
            this.chkEnableConsole.Size = new System.Drawing.Size(99, 17);
            this.chkEnableConsole.TabIndex = 37;
            this.chkEnableConsole.Text = "Enable console";
            this.chkEnableConsole.UseVisualStyleBackColor = true;
            this.chkEnableConsole.CheckedChanged += new System.EventHandler(this.chkEnableConsole_CheckedChanged);
            // 
            // frmLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 598);
            this.Controls.Add(this.labelConnected);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnStartStop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLogger";
            this.Text = "Logger";
            this.Load += new System.EventHandler(this.frmLogger_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.groupLogSettings.ResumeLayout(false);
            this.groupLogSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogData)).EndInit();
            this.tabProfile.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPidNames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogProfile)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabAnalyzer.ResumeLayout(false);
            this.tabAnalyzer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAnalyzer)).EndInit();
            this.tabDTC.ResumeLayout(false);
            this.groupDTC.ResumeLayout(false);
            this.groupDTC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDtcCodes)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupAdvanced.ResumeLayout(false);
            this.groupAdvanced.PerformLayout();
            this.groupHWSettings.ResumeLayout(false);
            this.categories.ResumeLayout(false);
            this.categories.PerformLayout();
            this.j2534OptionsGroupBox.ResumeLayout(false);
            this.j2534OptionsGroupBox.PerformLayout();
            this.serialOptionsGroupBox.ResumeLayout(false);
            this.serialOptionsGroupBox.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerShowData;
        private System.Windows.Forms.ComboBox comboSerialPort;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TabPage tabProfile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridLogProfile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileToolStripMenuItem;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dataGridPidNames;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.DataGridView dataGridLogData;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox chkWriteLog;
        private System.Windows.Forms.Button btnBrowsLogFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLogFolder;
        private System.Windows.Forms.TextBox txtLogSeparator;
        private System.Windows.Forms.Label labelSeparator;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboSerialDeviceType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox j2534OptionsGroupBox;
        private System.Windows.Forms.ComboBox j2534DeviceList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox categories;
        private System.Windows.Forms.RadioButton j2534RadioButton;
        private System.Windows.Forms.RadioButton serialRadioButton;
        private System.Windows.Forms.GroupBox serialOptionsGroupBox;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label labelResponseMode;
        private System.Windows.Forms.ComboBox comboResponseMode;
        private System.Windows.Forms.CheckBox chkFTDI;
        private System.Windows.Forms.CheckBox chkRawValues;
        private System.Windows.Forms.CheckBox chkReverseSlotNumbers;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listProfiles;
        private System.Windows.Forms.GroupBox groupLogSettings;
        private System.Windows.Forms.GroupBox groupAdvanced;
        private System.Windows.Forms.TabPage tabAnalyzer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timerAnalyzer;
        private System.Windows.Forms.DataGridView dataGridAnalyzer;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterSupportedPidsToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkHideHeartBeat;
        private System.Windows.Forms.CheckBox chkPriority;
        private System.Windows.Forms.ComboBox comboBaudRate;
        private System.Windows.Forms.TabPage tabDTC;
        private System.Windows.Forms.Button btnCurrentDTC;
        private System.Windows.Forms.Button btnHistoryDTC;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboModule;
        private System.Windows.Forms.Button btnClearCodes;
        private System.Windows.Forms.Button btnQueryPid;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox chkDtcAllModules;
        private System.Windows.Forms.Label labelConnected;
        private System.Windows.Forms.ToolStripMenuItem connectDisconnectToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridDtcCodes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Module;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.Button btnGetVINCode;
        private System.Windows.Forms.Button btnClearAnalyzerGrid;
        private System.Windows.Forms.Button btnAnalyzerSaveCsv;
        private System.Windows.Forms.GroupBox groupHWSettings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioParamMath;
        private System.Windows.Forms.RadioButton radioParamRam;
        private System.Windows.Forms.RadioButton radioParamStd;
        private System.Windows.Forms.CheckBox chkBusFilters;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyRowToolStripMenuItem;
        private System.Windows.Forms.TextBox txtParamSearch;
        private System.Windows.Forms.Timer timerSearchParams;
        private System.Windows.Forms.ToolStripMenuItem newProfileToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupDTC;
        private System.Windows.Forms.CheckBox chkFilterParamsByOS;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox txtSendBus;
        private System.Windows.Forms.RichTextBox txtVPWmessages;
        private System.Windows.Forms.CheckBox chkEnableConsole;
    }
}

