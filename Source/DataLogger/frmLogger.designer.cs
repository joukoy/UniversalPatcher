
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
            this.btnStartStopAnalyzer = new System.Windows.Forms.Button();
            this.btnSaveAnalyzerMsgs = new System.Windows.Forms.Button();
            this.btnAnalyzerSaveCsv = new System.Windows.Forms.Button();
            this.btnClearAnalyzerGrid = new System.Windows.Forms.Button();
            this.chkHideHeartBeat = new System.Windows.Forms.CheckBox();
            this.dataGridAnalyzer = new System.Windows.Forms.DataGridView();
            this.tabDTC = new System.Windows.Forms.TabPage();
            this.groupDTC = new System.Windows.Forms.GroupBox();
            this.txtDtcCustomModule = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDtcCustomMode = new System.Windows.Forms.TextBox();
            this.btnDtcCustom = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
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
            this.btnConnect2 = new System.Windows.Forms.Button();
            this.groupAdvanced = new System.Windows.Forms.GroupBox();
            this.chkFilterParamsByOS = new System.Windows.Forms.CheckBox();
            this.chkVPWFilters = new System.Windows.Forms.CheckBox();
            this.chkPriority = new System.Windows.Forms.CheckBox();
            this.chkRawValues = new System.Windows.Forms.CheckBox();
            this.chkReverseSlotNumbers = new System.Windows.Forms.CheckBox();
            this.labelResponseMode = new System.Windows.Forms.Label();
            this.comboResponseMode = new System.Windows.Forms.ComboBox();
            this.tabVPWConsole = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtEmulatorId = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.btnEmulatorEditResponses = new System.Windows.Forms.Button();
            this.btnEmulatorLoadResponses = new System.Windows.Forms.Button();
            this.ChkEmulatorResponseMode = new System.Windows.Forms.CheckBox();
            this.txtSendBus = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnStopScript = new System.Windows.Forms.Button();
            this.btnConsoleRefresh = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnConsoleLoadScript = new System.Windows.Forms.Button();
            this.chkConsoleAutorefresh = new System.Windows.Forms.CheckBox();
            this.numConsoleScriptDelay = new System.Windows.Forms.NumericUpDown();
            this.chkConsole4x = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkEnableConsole = new System.Windows.Forms.CheckBox();
            this.chkConsoleTimestamps = new System.Windows.Forms.CheckBox();
            this.richVPWmessages = new System.Windows.Forms.RichTextBox();
            this.tabJConsole = new System.Windows.Forms.TabPage();
            this.txtJConsoleSend = new System.Windows.Forms.TextBox();
            this.richJConsole = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnJconsoleStopScript = new System.Windows.Forms.Button();
            this.labelJconsoleConnected = new System.Windows.Forms.Label();
            this.btnJConsoleUploadScript = new System.Windows.Forms.Button();
            this.numJConsoleScriptDelay = new System.Windows.Forms.NumericUpDown();
            this.chkJConsole4x = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.chkJConsoleTimestamps = new System.Windows.Forms.CheckBox();
            this.btnJConsoleConnect = new System.Windows.Forms.Button();
            this.groupJ2534Options = new System.Windows.Forms.GroupBox();
            this.numJ2534PeriodicMsgInterval = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.txtJ2534PeriodicMsg = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnJ2534SettingsLoad = new System.Windows.Forms.Button();
            this.btnJ2534SettingsSaveAs = new System.Windows.Forms.Button();
            this.txtJ2534InitBytes = new System.Windows.Forms.TextBox();
            this.chkConsoleUseJ2534Timestamps = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.comboJ2534Init = new System.Windows.Forms.ComboBox();
            this.txtJ2534SetPins = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.comboJ2534DLL = new System.Windows.Forms.ComboBox();
            this.comboJ2534Connectflag = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboJ2534Baudrate = new System.Windows.Forms.ComboBox();
            this.comboJ2534Protocol = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tabAlgoTest = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioFindAlgo = new System.Windows.Forms.RadioButton();
            this.btnAlgoTest = new System.Windows.Forms.Button();
            this.txtAlgo = new System.Windows.Forms.TextBox();
            this.labelAlgo = new System.Windows.Forms.Label();
            this.radioFindAllKeys = new System.Windows.Forms.RadioButton();
            this.txtSeed = new System.Windows.Forms.TextBox();
            this.radioFindKey = new System.Windows.Forms.RadioButton();
            this.labelSeed = new System.Windows.Forms.Label();
            this.txtAlgoTest = new System.Windows.Forms.TextBox();
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
            this.parseLogfileToBinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labelConnected = new System.Windows.Forms.Label();
            this.timerSearchParams = new System.Windows.Forms.Timer(this.components);
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
            this.groupHWSettings.SuspendLayout();
            this.categories.SuspendLayout();
            this.j2534OptionsGroupBox.SuspendLayout();
            this.serialOptionsGroupBox.SuspendLayout();
            this.groupAdvanced.SuspendLayout();
            this.tabVPWConsole.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConsoleScriptDelay)).BeginInit();
            this.tabJConsole.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJConsoleScriptDelay)).BeginInit();
            this.groupJ2534Options.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJ2534PeriodicMsgInterval)).BeginInit();
            this.tabAlgoTest.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.comboSerialPort.Location = new System.Drawing.Point(80, 37);
            this.comboSerialPort.Name = "comboSerialPort";
            this.comboSerialPort.Size = new System.Drawing.Size(255, 21);
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
            this.tabControl1.Controls.Add(this.tabVPWConsole);
            this.tabControl1.Controls.Add(this.tabJConsole);
            this.tabControl1.Controls.Add(this.tabAlgoTest);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(906, 422);
            this.tabControl1.TabIndex = 3;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.groupLogSettings);
            this.tabLog.Controls.Add(this.dataGridLogData);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(898, 396);
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
            this.groupLogSettings.Size = new System.Drawing.Size(438, 400);
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
            this.listProfiles.Size = new System.Drawing.Size(423, 199);
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
            this.dataGridLogData.Size = new System.Drawing.Size(452, 390);
            this.dataGridLogData.TabIndex = 8;
            // 
            // tabProfile
            // 
            this.tabProfile.Controls.Add(this.splitContainer1);
            this.tabProfile.Location = new System.Drawing.Point(4, 22);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Size = new System.Drawing.Size(898, 396);
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
            this.splitContainer1.Size = new System.Drawing.Size(898, 396);
            this.splitContainer1.SplitterDistance = 364;
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
            this.dataGridPidNames.Size = new System.Drawing.Size(364, 362);
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
            this.dataGridLogProfile.Size = new System.Drawing.Size(488, 393);
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
            this.tabAnalyzer.Controls.Add(this.btnStartStopAnalyzer);
            this.tabAnalyzer.Controls.Add(this.btnSaveAnalyzerMsgs);
            this.tabAnalyzer.Controls.Add(this.btnAnalyzerSaveCsv);
            this.tabAnalyzer.Controls.Add(this.btnClearAnalyzerGrid);
            this.tabAnalyzer.Controls.Add(this.chkHideHeartBeat);
            this.tabAnalyzer.Controls.Add(this.dataGridAnalyzer);
            this.tabAnalyzer.Location = new System.Drawing.Point(4, 22);
            this.tabAnalyzer.Name = "tabAnalyzer";
            this.tabAnalyzer.Size = new System.Drawing.Size(898, 396);
            this.tabAnalyzer.TabIndex = 4;
            this.tabAnalyzer.Text = "Analyzer";
            this.tabAnalyzer.UseVisualStyleBackColor = true;
            // 
            // btnStartStopAnalyzer
            // 
            this.btnStartStopAnalyzer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStopAnalyzer.Location = new System.Drawing.Point(763, 3);
            this.btnStartStopAnalyzer.Name = "btnStartStopAnalyzer";
            this.btnStartStopAnalyzer.Size = new System.Drawing.Size(127, 23);
            this.btnStartStopAnalyzer.TabIndex = 8;
            this.btnStartStopAnalyzer.Text = "Start Analyzer";
            this.btnStartStopAnalyzer.UseVisualStyleBackColor = true;
            this.btnStartStopAnalyzer.Click += new System.EventHandler(this.btnStartStopAnalyzer_Click);
            // 
            // btnSaveAnalyzerMsgs
            // 
            this.btnSaveAnalyzerMsgs.Location = new System.Drawing.Point(279, 3);
            this.btnSaveAnalyzerMsgs.Name = "btnSaveAnalyzerMsgs";
            this.btnSaveAnalyzerMsgs.Size = new System.Drawing.Size(106, 22);
            this.btnSaveAnalyzerMsgs.TabIndex = 7;
            this.btnSaveAnalyzerMsgs.Text = "Save messages";
            this.btnSaveAnalyzerMsgs.UseVisualStyleBackColor = true;
            this.btnSaveAnalyzerMsgs.Click += new System.EventHandler(this.btnSaveAnalyzerMsgs_Click);
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
            this.dataGridAnalyzer.Size = new System.Drawing.Size(896, 367);
            this.dataGridAnalyzer.TabIndex = 3;
            // 
            // tabDTC
            // 
            this.tabDTC.Controls.Add(this.groupDTC);
            this.tabDTC.Controls.Add(this.dataGridDtcCodes);
            this.tabDTC.Location = new System.Drawing.Point(4, 22);
            this.tabDTC.Name = "tabDTC";
            this.tabDTC.Size = new System.Drawing.Size(898, 396);
            this.tabDTC.TabIndex = 5;
            this.tabDTC.Text = "DTC";
            this.tabDTC.UseVisualStyleBackColor = true;
            // 
            // groupDTC
            // 
            this.groupDTC.Controls.Add(this.txtDtcCustomModule);
            this.groupDTC.Controls.Add(this.label10);
            this.groupDTC.Controls.Add(this.txtDtcCustomMode);
            this.groupDTC.Controls.Add(this.btnDtcCustom);
            this.groupDTC.Controls.Add(this.label6);
            this.groupDTC.Controls.Add(this.btnHistoryDTC);
            this.groupDTC.Controls.Add(this.btnGetVINCode);
            this.groupDTC.Controls.Add(this.btnCurrentDTC);
            this.groupDTC.Controls.Add(this.comboModule);
            this.groupDTC.Controls.Add(this.chkDtcAllModules);
            this.groupDTC.Controls.Add(this.label8);
            this.groupDTC.Controls.Add(this.btnClearCodes);
            this.groupDTC.Location = new System.Drawing.Point(3, 3);
            this.groupDTC.Name = "groupDTC";
            this.groupDTC.Size = new System.Drawing.Size(199, 391);
            this.groupDTC.TabIndex = 9;
            this.groupDTC.TabStop = false;
            // 
            // txtDtcCustomModule
            // 
            this.txtDtcCustomModule.Location = new System.Drawing.Point(143, 314);
            this.txtDtcCustomModule.Name = "txtDtcCustomModule";
            this.txtDtcCustomModule.Size = new System.Drawing.Size(39, 20);
            this.txtDtcCustomModule.TabIndex = 14;
            this.txtDtcCustomModule.Text = "10";
            this.txtDtcCustomModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(23, 314);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Custom module:";
            // 
            // txtDtcCustomMode
            // 
            this.txtDtcCustomMode.Location = new System.Drawing.Point(143, 290);
            this.txtDtcCustomMode.Name = "txtDtcCustomMode";
            this.txtDtcCustomMode.Size = new System.Drawing.Size(39, 20);
            this.txtDtcCustomMode.TabIndex = 12;
            this.txtDtcCustomMode.Text = "10";
            this.txtDtcCustomMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnDtcCustom
            // 
            this.btnDtcCustom.Location = new System.Drawing.Point(20, 340);
            this.btnDtcCustom.Name = "btnDtcCustom";
            this.btnDtcCustom.Size = new System.Drawing.Size(162, 29);
            this.btnDtcCustom.TabIndex = 11;
            this.btnDtcCustom.Text = "Get custom DTC codes";
            this.btnDtcCustom.UseVisualStyleBackColor = true;
            this.btnDtcCustom.Click += new System.EventHandler(this.btnDtcCustom_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 293);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Custom dtc:";
            // 
            // btnHistoryDTC
            // 
            this.btnHistoryDTC.Location = new System.Drawing.Point(20, 96);
            this.btnHistoryDTC.Name = "btnHistoryDTC";
            this.btnHistoryDTC.Size = new System.Drawing.Size(162, 29);
            this.btnHistoryDTC.TabIndex = 1;
            this.btnHistoryDTC.Text = "Get History DTC codes";
            this.btnHistoryDTC.UseVisualStyleBackColor = true;
            this.btnHistoryDTC.Click += new System.EventHandler(this.btnHistoryDTC_Click);
            // 
            // btnGetVINCode
            // 
            this.btnGetVINCode.Location = new System.Drawing.Point(20, 166);
            this.btnGetVINCode.Name = "btnGetVINCode";
            this.btnGetVINCode.Size = new System.Drawing.Size(162, 29);
            this.btnGetVINCode.TabIndex = 8;
            this.btnGetVINCode.Text = "Get VIN Code";
            this.btnGetVINCode.UseVisualStyleBackColor = true;
            this.btnGetVINCode.Click += new System.EventHandler(this.btnGetVINCode_Click);
            // 
            // btnCurrentDTC
            // 
            this.btnCurrentDTC.Location = new System.Drawing.Point(20, 61);
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
            this.comboModule.Location = new System.Drawing.Point(77, 34);
            this.comboModule.Name = "comboModule";
            this.comboModule.Size = new System.Drawing.Size(105, 21);
            this.comboModule.TabIndex = 2;
            // 
            // chkDtcAllModules
            // 
            this.chkDtcAllModules.AutoSize = true;
            this.chkDtcAllModules.Location = new System.Drawing.Point(88, 11);
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
            this.label8.Location = new System.Drawing.Point(23, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Module:";
            // 
            // btnClearCodes
            // 
            this.btnClearCodes.Location = new System.Drawing.Point(20, 131);
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
            this.dataGridDtcCodes.Location = new System.Drawing.Point(208, 0);
            this.dataGridDtcCodes.Name = "dataGridDtcCodes";
            this.dataGridDtcCodes.Size = new System.Drawing.Size(686, 395);
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
            this.tabSettings.Controls.Add(this.groupHWSettings);
            this.tabSettings.Controls.Add(this.btnConnect2);
            this.tabSettings.Controls.Add(this.groupAdvanced);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(898, 396);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupHWSettings
            // 
            this.groupHWSettings.Controls.Add(this.categories);
            this.groupHWSettings.Controls.Add(this.j2534OptionsGroupBox);
            this.groupHWSettings.Controls.Add(this.serialOptionsGroupBox);
            this.groupHWSettings.Location = new System.Drawing.Point(8, 3);
            this.groupHWSettings.Name = "groupHWSettings";
            this.groupHWSettings.Size = new System.Drawing.Size(361, 224);
            this.groupHWSettings.TabIndex = 22;
            this.groupHWSettings.TabStop = false;
            // 
            // categories
            // 
            this.categories.Controls.Add(this.j2534RadioButton);
            this.categories.Controls.Add(this.serialRadioButton);
            this.categories.Location = new System.Drawing.Point(6, 19);
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
            this.j2534OptionsGroupBox.Enabled = false;
            this.j2534OptionsGroupBox.Location = new System.Drawing.Point(189, 19);
            this.j2534OptionsGroupBox.Name = "j2534OptionsGroupBox";
            this.j2534OptionsGroupBox.Size = new System.Drawing.Size(165, 72);
            this.j2534OptionsGroupBox.TabIndex = 19;
            this.j2534OptionsGroupBox.TabStop = false;
            this.j2534OptionsGroupBox.Text = "J2534 Device Options";
            // 
            // j2534DeviceList
            // 
            this.j2534DeviceList.FormattingEnabled = true;
            this.j2534DeviceList.Location = new System.Drawing.Point(7, 37);
            this.j2534DeviceList.Name = "j2534DeviceList";
            this.j2534DeviceList.Size = new System.Drawing.Size(146, 21);
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
            this.serialOptionsGroupBox.Location = new System.Drawing.Point(6, 96);
            this.serialOptionsGroupBox.Name = "serialOptionsGroupBox";
            this.serialOptionsGroupBox.Size = new System.Drawing.Size(348, 120);
            this.serialOptionsGroupBox.TabIndex = 20;
            this.serialOptionsGroupBox.TabStop = false;
            this.serialOptionsGroupBox.Text = "Serialport options";
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.FormattingEnabled = true;
            this.comboBaudRate.Location = new System.Drawing.Point(80, 89);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(256, 21);
            this.comboBaudRate.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 92);
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
            this.comboSerialDeviceType.Location = new System.Drawing.Point(81, 63);
            this.comboSerialDeviceType.Name = "comboSerialDeviceType";
            this.comboSerialDeviceType.Size = new System.Drawing.Size(255, 21);
            this.comboSerialDeviceType.TabIndex = 17;
            this.comboSerialDeviceType.SelectedIndexChanged += new System.EventHandler(this.comboSerialDeviceType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Device Type";
            // 
            // btnConnect2
            // 
            this.btnConnect2.Location = new System.Drawing.Point(121, 234);
            this.btnConnect2.Name = "btnConnect2";
            this.btnConnect2.Size = new System.Drawing.Size(142, 29);
            this.btnConnect2.TabIndex = 23;
            this.btnConnect2.Text = "Connect/ Disconnect";
            this.btnConnect2.UseVisualStyleBackColor = true;
            this.btnConnect2.Click += new System.EventHandler(this.btnConnect2_Click);
            // 
            // groupAdvanced
            // 
            this.groupAdvanced.Controls.Add(this.chkFilterParamsByOS);
            this.groupAdvanced.Controls.Add(this.chkVPWFilters);
            this.groupAdvanced.Controls.Add(this.chkPriority);
            this.groupAdvanced.Controls.Add(this.chkRawValues);
            this.groupAdvanced.Controls.Add(this.chkReverseSlotNumbers);
            this.groupAdvanced.Controls.Add(this.labelResponseMode);
            this.groupAdvanced.Controls.Add(this.comboResponseMode);
            this.groupAdvanced.Location = new System.Drawing.Point(375, 4);
            this.groupAdvanced.Name = "groupAdvanced";
            this.groupAdvanced.Size = new System.Drawing.Size(348, 115);
            this.groupAdvanced.TabIndex = 34;
            this.groupAdvanced.TabStop = false;
            this.groupAdvanced.Text = "Advanced settings";
            // 
            // chkFilterParamsByOS
            // 
            this.chkFilterParamsByOS.AutoSize = true;
            this.chkFilterParamsByOS.Checked = true;
            this.chkFilterParamsByOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilterParamsByOS.Location = new System.Drawing.Point(166, 19);
            this.chkFilterParamsByOS.Name = "chkFilterParamsByOS";
            this.chkFilterParamsByOS.Size = new System.Drawing.Size(135, 17);
            this.chkFilterParamsByOS.TabIndex = 36;
            this.chkFilterParamsByOS.Text = "Filter parameters by OS";
            this.chkFilterParamsByOS.UseVisualStyleBackColor = true;
            // 
            // chkVPWFilters
            // 
            this.chkVPWFilters.AutoSize = true;
            this.chkVPWFilters.Location = new System.Drawing.Point(9, 53);
            this.chkVPWFilters.Name = "chkVPWFilters";
            this.chkVPWFilters.Size = new System.Drawing.Size(103, 17);
            this.chkVPWFilters.TabIndex = 35;
            this.chkVPWFilters.Text = "Use VPW Filters";
            this.chkVPWFilters.UseVisualStyleBackColor = true;
            this.chkVPWFilters.CheckedChanged += new System.EventHandler(this.chkBusFilters_CheckedChanged);
            // 
            // chkPriority
            // 
            this.chkPriority.AutoSize = true;
            this.chkPriority.Location = new System.Drawing.Point(166, 37);
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
            this.chkReverseSlotNumbers.Location = new System.Drawing.Point(9, 36);
            this.chkReverseSlotNumbers.Name = "chkReverseSlotNumbers";
            this.chkReverseSlotNumbers.Size = new System.Drawing.Size(149, 17);
            this.chkReverseSlotNumbers.TabIndex = 27;
            this.chkReverseSlotNumbers.Text = "Slot numbers start from FE";
            this.chkReverseSlotNumbers.UseVisualStyleBackColor = true;
            // 
            // labelResponseMode
            // 
            this.labelResponseMode.AutoSize = true;
            this.labelResponseMode.Location = new System.Drawing.Point(7, 72);
            this.labelResponseMode.Name = "labelResponseMode";
            this.labelResponseMode.Size = new System.Drawing.Size(88, 13);
            this.labelResponseMode.TabIndex = 24;
            this.labelResponseMode.Text = "Response Mode:";
            // 
            // comboResponseMode
            // 
            this.comboResponseMode.FormattingEnabled = true;
            this.comboResponseMode.Location = new System.Drawing.Point(7, 89);
            this.comboResponseMode.Name = "comboResponseMode";
            this.comboResponseMode.Size = new System.Drawing.Size(167, 21);
            this.comboResponseMode.TabIndex = 23;
            this.comboResponseMode.SelectedIndexChanged += new System.EventHandler(this.comboResponseMode_SelectedIndexChanged);
            // 
            // tabVPWConsole
            // 
            this.tabVPWConsole.Controls.Add(this.groupBox6);
            this.tabVPWConsole.Controls.Add(this.txtSendBus);
            this.tabVPWConsole.Controls.Add(this.groupBox3);
            this.tabVPWConsole.Controls.Add(this.richVPWmessages);
            this.tabVPWConsole.Location = new System.Drawing.Point(4, 22);
            this.tabVPWConsole.Name = "tabVPWConsole";
            this.tabVPWConsole.Size = new System.Drawing.Size(898, 396);
            this.tabVPWConsole.TabIndex = 6;
            this.tabVPWConsole.Text = "VPW Console";
            this.tabVPWConsole.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtEmulatorId);
            this.groupBox6.Controls.Add(this.label21);
            this.groupBox6.Controls.Add(this.btnEmulatorEditResponses);
            this.groupBox6.Controls.Add(this.btnEmulatorLoadResponses);
            this.groupBox6.Controls.Add(this.ChkEmulatorResponseMode);
            this.groupBox6.Location = new System.Drawing.Point(150, 8);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(140, 381);
            this.groupBox6.TabIndex = 36;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Emulator";
            this.groupBox6.Visible = false;
            // 
            // txtEmulatorId
            // 
            this.txtEmulatorId.Location = new System.Drawing.Point(38, 41);
            this.txtEmulatorId.Name = "txtEmulatorId";
            this.txtEmulatorId.Size = new System.Drawing.Size(92, 20);
            this.txtEmulatorId.TabIndex = 52;
            this.txtEmulatorId.Text = "10";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 44);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(21, 13);
            this.label21.TabIndex = 51;
            this.label21.Text = "ID:";
            // 
            // btnEmulatorEditResponses
            // 
            this.btnEmulatorEditResponses.Location = new System.Drawing.Point(6, 222);
            this.btnEmulatorEditResponses.Name = "btnEmulatorEditResponses";
            this.btnEmulatorEditResponses.Size = new System.Drawing.Size(124, 24);
            this.btnEmulatorEditResponses.TabIndex = 50;
            this.btnEmulatorEditResponses.Text = "Edit Responses";
            this.btnEmulatorEditResponses.UseVisualStyleBackColor = true;
            this.btnEmulatorEditResponses.Click += new System.EventHandler(this.btnConsoleEditResponses_Click);
            // 
            // btnEmulatorLoadResponses
            // 
            this.btnEmulatorLoadResponses.Location = new System.Drawing.Point(6, 196);
            this.btnEmulatorLoadResponses.Name = "btnEmulatorLoadResponses";
            this.btnEmulatorLoadResponses.Size = new System.Drawing.Size(125, 25);
            this.btnEmulatorLoadResponses.TabIndex = 49;
            this.btnEmulatorLoadResponses.Text = "Load Responses";
            this.btnEmulatorLoadResponses.UseVisualStyleBackColor = true;
            this.btnEmulatorLoadResponses.Click += new System.EventHandler(this.btnConsoleLoadResponses_Click);
            // 
            // ChkEmulatorResponseMode
            // 
            this.ChkEmulatorResponseMode.AutoSize = true;
            this.ChkEmulatorResponseMode.Location = new System.Drawing.Point(6, 20);
            this.ChkEmulatorResponseMode.Name = "ChkEmulatorResponseMode";
            this.ChkEmulatorResponseMode.Size = new System.Drawing.Size(103, 17);
            this.ChkEmulatorResponseMode.TabIndex = 48;
            this.ChkEmulatorResponseMode.Text = "Response mode";
            this.ChkEmulatorResponseMode.UseVisualStyleBackColor = true;
            this.ChkEmulatorResponseMode.CheckedChanged += new System.EventHandler(this.ChkConsoleResponseMode_CheckedChanged);
            // 
            // txtSendBus
            // 
            this.txtSendBus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSendBus.Location = new System.Drawing.Point(304, 370);
            this.txtSendBus.Name = "txtSendBus";
            this.txtSendBus.Size = new System.Drawing.Size(589, 20);
            this.txtSendBus.TabIndex = 1;
            this.txtSendBus.TextChanged += new System.EventHandler(this.txtSendBus_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnStopScript);
            this.groupBox3.Controls.Add(this.btnConsoleRefresh);
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Controls.Add(this.btnConsoleLoadScript);
            this.groupBox3.Controls.Add(this.chkConsoleAutorefresh);
            this.groupBox3.Controls.Add(this.numConsoleScriptDelay);
            this.groupBox3.Controls.Add(this.chkConsole4x);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.chkEnableConsole);
            this.groupBox3.Controls.Add(this.chkConsoleTimestamps);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(140, 387);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "VPW Console";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // btnStopScript
            // 
            this.btnStopScript.Enabled = false;
            this.btnStopScript.Location = new System.Drawing.Point(6, 201);
            this.btnStopScript.Name = "btnStopScript";
            this.btnStopScript.Size = new System.Drawing.Size(90, 23);
            this.btnStopScript.TabIndex = 47;
            this.btnStopScript.Text = "Stop Script";
            this.btnStopScript.UseVisualStyleBackColor = true;
            this.btnStopScript.Click += new System.EventHandler(this.btnStopScript_Click_1);
            // 
            // btnConsoleRefresh
            // 
            this.btnConsoleRefresh.Location = new System.Drawing.Point(2, 102);
            this.btnConsoleRefresh.Name = "btnConsoleRefresh";
            this.btnConsoleRefresh.Size = new System.Drawing.Size(90, 21);
            this.btnConsoleRefresh.TabIndex = 43;
            this.btnConsoleRefresh.Text = "Refresh";
            this.btnConsoleRefresh.UseVisualStyleBackColor = true;
            this.btnConsoleRefresh.Click += new System.EventHandler(this.btnConsoleRefresh_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(6, 322);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(125, 24);
            this.btnConnect.TabIndex = 21;
            this.btnConnect.Text = "Connect/ Disconnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnConsoleLoadScript
            // 
            this.btnConsoleLoadScript.Location = new System.Drawing.Point(6, 171);
            this.btnConsoleLoadScript.Name = "btnConsoleLoadScript";
            this.btnConsoleLoadScript.Size = new System.Drawing.Size(90, 24);
            this.btnConsoleLoadScript.TabIndex = 0;
            this.btnConsoleLoadScript.Text = "Upload script";
            this.btnConsoleLoadScript.UseVisualStyleBackColor = true;
            this.btnConsoleLoadScript.Click += new System.EventHandler(this.btnConsoleLoadScript_Click);
            // 
            // chkConsoleAutorefresh
            // 
            this.chkConsoleAutorefresh.AutoSize = true;
            this.chkConsoleAutorefresh.Location = new System.Drawing.Point(6, 65);
            this.chkConsoleAutorefresh.Name = "chkConsoleAutorefresh";
            this.chkConsoleAutorefresh.Size = new System.Drawing.Size(83, 17);
            this.chkConsoleAutorefresh.TabIndex = 44;
            this.chkConsoleAutorefresh.Text = "Auto refresh";
            this.chkConsoleAutorefresh.UseVisualStyleBackColor = true;
            this.chkConsoleAutorefresh.CheckedChanged += new System.EventHandler(this.chkConsoleAutorefresh_CheckedChanged);
            // 
            // numConsoleScriptDelay
            // 
            this.numConsoleScriptDelay.Location = new System.Drawing.Point(6, 146);
            this.numConsoleScriptDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numConsoleScriptDelay.Name = "numConsoleScriptDelay";
            this.numConsoleScriptDelay.Size = new System.Drawing.Size(51, 20);
            this.numConsoleScriptDelay.TabIndex = 40;
            this.numConsoleScriptDelay.ValueChanged += new System.EventHandler(this.numConsoleScriptDelay_ValueChanged);
            // 
            // chkConsole4x
            // 
            this.chkConsole4x.AutoSize = true;
            this.chkConsole4x.Checked = true;
            this.chkConsole4x.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkConsole4x.Location = new System.Drawing.Point(6, 48);
            this.chkConsole4x.Name = "chkConsole4x";
            this.chkConsole4x.Size = new System.Drawing.Size(73, 17);
            this.chkConsole4x.TabIndex = 42;
            this.chkConsole4x.Text = "Enable 4x";
            this.chkConsole4x.UseVisualStyleBackColor = true;
            this.chkConsole4x.CheckedChanged += new System.EventHandler(this.chkConsole4x_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "Script delay (ms)";
            // 
            // chkEnableConsole
            // 
            this.chkEnableConsole.AutoSize = true;
            this.chkEnableConsole.Location = new System.Drawing.Point(6, 14);
            this.chkEnableConsole.Name = "chkEnableConsole";
            this.chkEnableConsole.Size = new System.Drawing.Size(127, 17);
            this.chkEnableConsole.TabIndex = 37;
            this.chkEnableConsole.Text = "Enable VPW console";
            this.chkEnableConsole.UseVisualStyleBackColor = true;
            this.chkEnableConsole.CheckedChanged += new System.EventHandler(this.chkEnableConsole_CheckedChanged);
            // 
            // chkConsoleTimestamps
            // 
            this.chkConsoleTimestamps.AutoSize = true;
            this.chkConsoleTimestamps.Location = new System.Drawing.Point(6, 30);
            this.chkConsoleTimestamps.Name = "chkConsoleTimestamps";
            this.chkConsoleTimestamps.Size = new System.Drawing.Size(82, 17);
            this.chkConsoleTimestamps.TabIndex = 38;
            this.chkConsoleTimestamps.Text = "Timestamps";
            this.chkConsoleTimestamps.UseVisualStyleBackColor = true;
            // 
            // richVPWmessages
            // 
            this.richVPWmessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richVPWmessages.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richVPWmessages.HideSelection = false;
            this.richVPWmessages.Location = new System.Drawing.Point(304, 5);
            this.richVPWmessages.Name = "richVPWmessages";
            this.richVPWmessages.Size = new System.Drawing.Size(589, 360);
            this.richVPWmessages.TabIndex = 2;
            this.richVPWmessages.Text = "";
            this.richVPWmessages.WordWrap = false;
            // 
            // tabJConsole
            // 
            this.tabJConsole.Controls.Add(this.txtJConsoleSend);
            this.tabJConsole.Controls.Add(this.richJConsole);
            this.tabJConsole.Controls.Add(this.groupBox4);
            this.tabJConsole.Controls.Add(this.btnJConsoleConnect);
            this.tabJConsole.Controls.Add(this.groupJ2534Options);
            this.tabJConsole.Location = new System.Drawing.Point(4, 22);
            this.tabJConsole.Margin = new System.Windows.Forms.Padding(2);
            this.tabJConsole.Name = "tabJConsole";
            this.tabJConsole.Size = new System.Drawing.Size(898, 396);
            this.tabJConsole.TabIndex = 7;
            this.tabJConsole.Text = "J-Console";
            this.tabJConsole.UseVisualStyleBackColor = true;
            // 
            // txtJConsoleSend
            // 
            this.txtJConsoleSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJConsoleSend.Location = new System.Drawing.Point(328, 371);
            this.txtJConsoleSend.Margin = new System.Windows.Forms.Padding(2);
            this.txtJConsoleSend.Name = "txtJConsoleSend";
            this.txtJConsoleSend.Size = new System.Drawing.Size(570, 20);
            this.txtJConsoleSend.TabIndex = 0;
            this.txtJConsoleSend.TextChanged += new System.EventHandler(this.txtJConsoleSend_TextChanged);
            // 
            // richJConsole
            // 
            this.richJConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richJConsole.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richJConsole.HideSelection = false;
            this.richJConsole.Location = new System.Drawing.Point(328, 5);
            this.richJConsole.Name = "richJConsole";
            this.richJConsole.Size = new System.Drawing.Size(570, 361);
            this.richJConsole.TabIndex = 3;
            this.richJConsole.Text = "";
            this.richJConsole.WordWrap = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnJconsoleStopScript);
            this.groupBox4.Controls.Add(this.labelJconsoleConnected);
            this.groupBox4.Controls.Add(this.btnJConsoleUploadScript);
            this.groupBox4.Controls.Add(this.numJConsoleScriptDelay);
            this.groupBox4.Controls.Add(this.chkJConsole4x);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.chkJConsoleTimestamps);
            this.groupBox4.Location = new System.Drawing.Point(219, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(106, 288);
            this.groupBox4.TabIndex = 51;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "J-Console";
            // 
            // btnJconsoleStopScript
            // 
            this.btnJconsoleStopScript.Enabled = false;
            this.btnJconsoleStopScript.Location = new System.Drawing.Point(7, 240);
            this.btnJconsoleStopScript.Name = "btnJconsoleStopScript";
            this.btnJconsoleStopScript.Size = new System.Drawing.Size(89, 23);
            this.btnJconsoleStopScript.TabIndex = 46;
            this.btnJconsoleStopScript.Text = "Stop Script";
            this.btnJconsoleStopScript.UseVisualStyleBackColor = true;
            this.btnJconsoleStopScript.Click += new System.EventHandler(this.btnStopJScript_Click);
            // 
            // labelJconsoleConnected
            // 
            this.labelJconsoleConnected.AutoSize = true;
            this.labelJconsoleConnected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelJconsoleConnected.Location = new System.Drawing.Point(12, 25);
            this.labelJconsoleConnected.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelJconsoleConnected.Name = "labelJconsoleConnected";
            this.labelJconsoleConnected.Size = new System.Drawing.Size(75, 15);
            this.labelJconsoleConnected.TabIndex = 45;
            this.labelJconsoleConnected.Text = "Disconnected";
            // 
            // btnJConsoleUploadScript
            // 
            this.btnJConsoleUploadScript.Location = new System.Drawing.Point(6, 209);
            this.btnJConsoleUploadScript.Name = "btnJConsoleUploadScript";
            this.btnJConsoleUploadScript.Size = new System.Drawing.Size(90, 24);
            this.btnJConsoleUploadScript.TabIndex = 0;
            this.btnJConsoleUploadScript.Text = "Upload script";
            this.btnJConsoleUploadScript.UseVisualStyleBackColor = true;
            this.btnJConsoleUploadScript.Click += new System.EventHandler(this.btnJConsoleUploadScript_Click);
            // 
            // numJConsoleScriptDelay
            // 
            this.numJConsoleScriptDelay.Location = new System.Drawing.Point(6, 184);
            this.numJConsoleScriptDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numJConsoleScriptDelay.Name = "numJConsoleScriptDelay";
            this.numJConsoleScriptDelay.Size = new System.Drawing.Size(51, 20);
            this.numJConsoleScriptDelay.TabIndex = 40;
            this.numJConsoleScriptDelay.ValueChanged += new System.EventHandler(this.numJConsoleScriptDelay_ValueChanged);
            // 
            // chkJConsole4x
            // 
            this.chkJConsole4x.AutoSize = true;
            this.chkJConsole4x.Checked = true;
            this.chkJConsole4x.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkJConsole4x.Location = new System.Drawing.Point(6, 72);
            this.chkJConsole4x.Name = "chkJConsole4x";
            this.chkJConsole4x.Size = new System.Drawing.Size(73, 17);
            this.chkJConsole4x.TabIndex = 42;
            this.chkJConsole4x.Text = "Enable 4x";
            this.chkJConsole4x.UseVisualStyleBackColor = true;
            this.chkJConsole4x.CheckedChanged += new System.EventHandler(this.chkJConsole4x_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(4, 168);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(84, 13);
            this.label20.TabIndex = 41;
            this.label20.Text = "Script delay (ms)";
            // 
            // chkJConsoleTimestamps
            // 
            this.chkJConsoleTimestamps.AutoSize = true;
            this.chkJConsoleTimestamps.Location = new System.Drawing.Point(6, 54);
            this.chkJConsoleTimestamps.Name = "chkJConsoleTimestamps";
            this.chkJConsoleTimestamps.Size = new System.Drawing.Size(82, 17);
            this.chkJConsoleTimestamps.TabIndex = 38;
            this.chkJConsoleTimestamps.Text = "Timestamps";
            this.chkJConsoleTimestamps.UseVisualStyleBackColor = true;
            // 
            // btnJConsoleConnect
            // 
            this.btnJConsoleConnect.Location = new System.Drawing.Point(30, 321);
            this.btnJConsoleConnect.Name = "btnJConsoleConnect";
            this.btnJConsoleConnect.Size = new System.Drawing.Size(125, 24);
            this.btnJConsoleConnect.TabIndex = 21;
            this.btnJConsoleConnect.Text = "Connect/ Disconnect";
            this.btnJConsoleConnect.UseVisualStyleBackColor = true;
            this.btnJConsoleConnect.Click += new System.EventHandler(this.btnJConsoleConnect_Click);
            // 
            // groupJ2534Options
            // 
            this.groupJ2534Options.Controls.Add(this.numJ2534PeriodicMsgInterval);
            this.groupJ2534Options.Controls.Add(this.label19);
            this.groupJ2534Options.Controls.Add(this.txtJ2534PeriodicMsg);
            this.groupJ2534Options.Controls.Add(this.label18);
            this.groupJ2534Options.Controls.Add(this.btnJ2534SettingsLoad);
            this.groupJ2534Options.Controls.Add(this.btnJ2534SettingsSaveAs);
            this.groupJ2534Options.Controls.Add(this.txtJ2534InitBytes);
            this.groupJ2534Options.Controls.Add(this.chkConsoleUseJ2534Timestamps);
            this.groupJ2534Options.Controls.Add(this.label17);
            this.groupJ2534Options.Controls.Add(this.label16);
            this.groupJ2534Options.Controls.Add(this.comboJ2534Init);
            this.groupJ2534Options.Controls.Add(this.txtJ2534SetPins);
            this.groupJ2534Options.Controls.Add(this.label15);
            this.groupJ2534Options.Controls.Add(this.comboJ2534DLL);
            this.groupJ2534Options.Controls.Add(this.comboJ2534Connectflag);
            this.groupJ2534Options.Controls.Add(this.label14);
            this.groupJ2534Options.Controls.Add(this.label13);
            this.groupJ2534Options.Controls.Add(this.label11);
            this.groupJ2534Options.Controls.Add(this.comboJ2534Baudrate);
            this.groupJ2534Options.Controls.Add(this.comboJ2534Protocol);
            this.groupJ2534Options.Controls.Add(this.label12);
            this.groupJ2534Options.Location = new System.Drawing.Point(7, 3);
            this.groupJ2534Options.Name = "groupJ2534Options";
            this.groupJ2534Options.Size = new System.Drawing.Size(206, 288);
            this.groupJ2534Options.TabIndex = 50;
            this.groupJ2534Options.TabStop = false;
            this.groupJ2534Options.Text = "J2534 options";
            // 
            // numJ2534PeriodicMsgInterval
            // 
            this.numJ2534PeriodicMsgInterval.Location = new System.Drawing.Point(83, 212);
            this.numJ2534PeriodicMsgInterval.Margin = new System.Windows.Forms.Padding(2);
            this.numJ2534PeriodicMsgInterval.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.numJ2534PeriodicMsgInterval.Name = "numJ2534PeriodicMsgInterval";
            this.numJ2534PeriodicMsgInterval.Size = new System.Drawing.Size(113, 20);
            this.numJ2534PeriodicMsgInterval.TabIndex = 64;
            this.numJ2534PeriodicMsgInterval.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(8, 214);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(67, 13);
            this.label19.TabIndex = 63;
            this.label19.Text = "Interval: (ms)";
            // 
            // txtJ2534PeriodicMsg
            // 
            this.txtJ2534PeriodicMsg.Location = new System.Drawing.Point(82, 189);
            this.txtJ2534PeriodicMsg.Margin = new System.Windows.Forms.Padding(2);
            this.txtJ2534PeriodicMsg.Name = "txtJ2534PeriodicMsg";
            this.txtJ2534PeriodicMsg.Size = new System.Drawing.Size(115, 20);
            this.txtJ2534PeriodicMsg.TabIndex = 62;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(7, 189);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 13);
            this.label18.TabIndex = 61;
            this.label18.Text = "Periodic msg:";
            // 
            // btnJ2534SettingsLoad
            // 
            this.btnJ2534SettingsLoad.Location = new System.Drawing.Point(118, 262);
            this.btnJ2534SettingsLoad.Margin = new System.Windows.Forms.Padding(2);
            this.btnJ2534SettingsLoad.Name = "btnJ2534SettingsLoad";
            this.btnJ2534SettingsLoad.Size = new System.Drawing.Size(79, 21);
            this.btnJ2534SettingsLoad.TabIndex = 60;
            this.btnJ2534SettingsLoad.Text = "Load...";
            this.btnJ2534SettingsLoad.UseVisualStyleBackColor = true;
            this.btnJ2534SettingsLoad.Click += new System.EventHandler(this.btnJ2534SettingsLoad_Click);
            // 
            // btnJ2534SettingsSaveAs
            // 
            this.btnJ2534SettingsSaveAs.Location = new System.Drawing.Point(5, 261);
            this.btnJ2534SettingsSaveAs.Margin = new System.Windows.Forms.Padding(2);
            this.btnJ2534SettingsSaveAs.Name = "btnJ2534SettingsSaveAs";
            this.btnJ2534SettingsSaveAs.Size = new System.Drawing.Size(79, 21);
            this.btnJ2534SettingsSaveAs.TabIndex = 59;
            this.btnJ2534SettingsSaveAs.Text = "Save as...";
            this.btnJ2534SettingsSaveAs.UseVisualStyleBackColor = true;
            this.btnJ2534SettingsSaveAs.Click += new System.EventHandler(this.btnJ2534SettingsSaveAs_Click);
            // 
            // txtJ2534InitBytes
            // 
            this.txtJ2534InitBytes.Location = new System.Drawing.Point(82, 166);
            this.txtJ2534InitBytes.Name = "txtJ2534InitBytes";
            this.txtJ2534InitBytes.Size = new System.Drawing.Size(115, 20);
            this.txtJ2534InitBytes.TabIndex = 58;
            // 
            // chkConsoleUseJ2534Timestamps
            // 
            this.chkConsoleUseJ2534Timestamps.AutoSize = true;
            this.chkConsoleUseJ2534Timestamps.Location = new System.Drawing.Point(92, 236);
            this.chkConsoleUseJ2534Timestamps.Name = "chkConsoleUseJ2534Timestamps";
            this.chkConsoleUseJ2534Timestamps.Size = new System.Drawing.Size(114, 17);
            this.chkConsoleUseJ2534Timestamps.TabIndex = 39;
            this.chkConsoleUseJ2534Timestamps.Text = "J2534 Timestamps";
            this.chkConsoleUseJ2534Timestamps.UseVisualStyleBackColor = true;
            this.chkConsoleUseJ2534Timestamps.CheckedChanged += new System.EventHandler(this.chkConsoleUseJ2534Timestamps_CheckedChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 168);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(52, 13);
            this.label17.TabIndex = 57;
            this.label17.Text = "Init bytes:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 144);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(24, 13);
            this.label16.TabIndex = 56;
            this.label16.Text = "Init:";
            // 
            // comboJ2534Init
            // 
            this.comboJ2534Init.FormattingEnabled = true;
            this.comboJ2534Init.Location = new System.Drawing.Point(83, 141);
            this.comboJ2534Init.Name = "comboJ2534Init";
            this.comboJ2534Init.Size = new System.Drawing.Size(115, 21);
            this.comboJ2534Init.TabIndex = 55;
            this.comboJ2534Init.SelectedIndexChanged += new System.EventHandler(this.comboJ2534Init_SelectedIndexChanged);
            // 
            // txtJ2534SetPins
            // 
            this.txtJ2534SetPins.Location = new System.Drawing.Point(82, 117);
            this.txtJ2534SetPins.Name = "txtJ2534SetPins";
            this.txtJ2534SetPins.Size = new System.Drawing.Size(116, 20);
            this.txtJ2534SetPins.TabIndex = 54;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 119);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 13);
            this.label15.TabIndex = 53;
            this.label15.Text = "Set pins:";
            // 
            // comboJ2534DLL
            // 
            this.comboJ2534DLL.FormattingEnabled = true;
            this.comboJ2534DLL.Location = new System.Drawing.Point(83, 25);
            this.comboJ2534DLL.Name = "comboJ2534DLL";
            this.comboJ2534DLL.Size = new System.Drawing.Size(116, 21);
            this.comboJ2534DLL.TabIndex = 52;
            this.comboJ2534DLL.SelectedIndexChanged += new System.EventHandler(this.comboJ2534DLL_SelectedIndexChanged);
            // 
            // comboJ2534Connectflag
            // 
            this.comboJ2534Connectflag.FormattingEnabled = true;
            this.comboJ2534Connectflag.Location = new System.Drawing.Point(82, 93);
            this.comboJ2534Connectflag.Name = "comboJ2534Connectflag";
            this.comboJ2534Connectflag.Size = new System.Drawing.Size(116, 21);
            this.comboJ2534Connectflag.TabIndex = 50;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 28);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(68, 13);
            this.label14.TabIndex = 51;
            this.label14.Text = "&Device Type";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 95);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 13);
            this.label13.TabIndex = 49;
            this.label13.Text = "Connectflag:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 50);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(49, 13);
            this.label11.TabIndex = 46;
            this.label11.Text = "Protocol:";
            // 
            // comboJ2534Baudrate
            // 
            this.comboJ2534Baudrate.FormattingEnabled = true;
            this.comboJ2534Baudrate.Location = new System.Drawing.Point(82, 69);
            this.comboJ2534Baudrate.Name = "comboJ2534Baudrate";
            this.comboJ2534Baudrate.Size = new System.Drawing.Size(116, 21);
            this.comboJ2534Baudrate.TabIndex = 48;
            // 
            // comboJ2534Protocol
            // 
            this.comboJ2534Protocol.FormattingEnabled = true;
            this.comboJ2534Protocol.Location = new System.Drawing.Point(83, 47);
            this.comboJ2534Protocol.Name = "comboJ2534Protocol";
            this.comboJ2534Protocol.Size = new System.Drawing.Size(116, 21);
            this.comboJ2534Protocol.TabIndex = 45;
            this.comboJ2534Protocol.SelectedIndexChanged += new System.EventHandler(this.comboJ2534Protocol_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 47;
            this.label12.Text = "Baudrate:";
            // 
            // tabAlgoTest
            // 
            this.tabAlgoTest.Controls.Add(this.groupBox5);
            this.tabAlgoTest.Controls.Add(this.txtAlgoTest);
            this.tabAlgoTest.Location = new System.Drawing.Point(4, 22);
            this.tabAlgoTest.Name = "tabAlgoTest";
            this.tabAlgoTest.Size = new System.Drawing.Size(898, 396);
            this.tabAlgoTest.TabIndex = 8;
            this.tabAlgoTest.Text = "AlgoTest";
            this.tabAlgoTest.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioFindAlgo);
            this.groupBox5.Controls.Add(this.btnAlgoTest);
            this.groupBox5.Controls.Add(this.txtAlgo);
            this.groupBox5.Controls.Add(this.labelAlgo);
            this.groupBox5.Controls.Add(this.radioFindAllKeys);
            this.groupBox5.Controls.Add(this.txtSeed);
            this.groupBox5.Controls.Add(this.radioFindKey);
            this.groupBox5.Controls.Add(this.labelSeed);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(167, 179);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            // 
            // radioFindAlgo
            // 
            this.radioFindAlgo.AutoSize = true;
            this.radioFindAlgo.Location = new System.Drawing.Point(6, 65);
            this.radioFindAlgo.Name = "radioFindAlgo";
            this.radioFindAlgo.Size = new System.Drawing.Size(68, 17);
            this.radioFindAlgo.TabIndex = 2;
            this.radioFindAlgo.Text = "Find algo";
            this.radioFindAlgo.UseVisualStyleBackColor = true;
            this.radioFindAlgo.CheckedChanged += new System.EventHandler(this.radioFindAlgo_CheckedChanged);
            // 
            // btnAlgoTest
            // 
            this.btnAlgoTest.Location = new System.Drawing.Point(52, 139);
            this.btnAlgoTest.Name = "btnAlgoTest";
            this.btnAlgoTest.Size = new System.Drawing.Size(101, 24);
            this.btnAlgoTest.TabIndex = 5;
            this.btnAlgoTest.Text = "Go";
            this.btnAlgoTest.UseVisualStyleBackColor = true;
            this.btnAlgoTest.Click += new System.EventHandler(this.btnAlgoTest_Click);
            // 
            // txtAlgo
            // 
            this.txtAlgo.Location = new System.Drawing.Point(53, 114);
            this.txtAlgo.Name = "txtAlgo";
            this.txtAlgo.Size = new System.Drawing.Size(100, 20);
            this.txtAlgo.TabIndex = 2;
            // 
            // labelAlgo
            // 
            this.labelAlgo.AutoSize = true;
            this.labelAlgo.Location = new System.Drawing.Point(3, 117);
            this.labelAlgo.Name = "labelAlgo";
            this.labelAlgo.Size = new System.Drawing.Size(31, 13);
            this.labelAlgo.TabIndex = 0;
            this.labelAlgo.Text = "Algo:";
            // 
            // radioFindAllKeys
            // 
            this.radioFindAllKeys.AutoSize = true;
            this.radioFindAllKeys.Location = new System.Drawing.Point(6, 42);
            this.radioFindAllKeys.Name = "radioFindAllKeys";
            this.radioFindAllKeys.Size = new System.Drawing.Size(83, 17);
            this.radioFindAllKeys.TabIndex = 1;
            this.radioFindAllKeys.Text = "Find all keys";
            this.radioFindAllKeys.UseVisualStyleBackColor = true;
            this.radioFindAllKeys.CheckedChanged += new System.EventHandler(this.radioFindAllKeys_CheckedChanged);
            // 
            // txtSeed
            // 
            this.txtSeed.Location = new System.Drawing.Point(53, 88);
            this.txtSeed.Name = "txtSeed";
            this.txtSeed.Size = new System.Drawing.Size(100, 20);
            this.txtSeed.TabIndex = 3;
            // 
            // radioFindKey
            // 
            this.radioFindKey.AutoSize = true;
            this.radioFindKey.Checked = true;
            this.radioFindKey.Location = new System.Drawing.Point(6, 19);
            this.radioFindKey.Name = "radioFindKey";
            this.radioFindKey.Size = new System.Drawing.Size(66, 17);
            this.radioFindKey.TabIndex = 0;
            this.radioFindKey.TabStop = true;
            this.radioFindKey.Text = "Find Key";
            this.radioFindKey.UseVisualStyleBackColor = true;
            this.radioFindKey.CheckedChanged += new System.EventHandler(this.radioFindKey_CheckedChanged);
            // 
            // labelSeed
            // 
            this.labelSeed.AutoSize = true;
            this.labelSeed.Location = new System.Drawing.Point(4, 91);
            this.labelSeed.Name = "labelSeed";
            this.labelSeed.Size = new System.Drawing.Size(35, 13);
            this.labelSeed.TabIndex = 1;
            this.labelSeed.Text = "Seed:";
            // 
            // txtAlgoTest
            // 
            this.txtAlgoTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAlgoTest.HideSelection = false;
            this.txtAlgoTest.Location = new System.Drawing.Point(176, 3);
            this.txtAlgoTest.Multiline = true;
            this.txtAlgoTest.Name = "txtAlgoTest";
            this.txtAlgoTest.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAlgoTest.Size = new System.Drawing.Size(719, 393);
            this.txtAlgoTest.TabIndex = 4;
            // 
            // labelProgress
            // 
            this.labelProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelProgress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.Location = new System.Drawing.Point(453, 26);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(456, 57);
            this.labelProgress.TabIndex = 22;
            this.labelProgress.Text = "idle";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(3, 24);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(444, 32);
            this.btnStartStop.TabIndex = 5;
            this.btnStartStop.Text = "Start Logging";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(906, 109);
            this.txtResult.TabIndex = 6;
            this.txtResult.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(912, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
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
            this.loadProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadProfileToolStripMenuItem.Text = "Load profile";
            this.loadProfileToolStripMenuItem.Click += new System.EventHandler(this.loadProfileToolStripMenuItem_Click);
            // 
            // saveProfileToolStripMenuItem
            // 
            this.saveProfileToolStripMenuItem.Name = "saveProfileToolStripMenuItem";
            this.saveProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProfileToolStripMenuItem.Text = "Save profile";
            this.saveProfileToolStripMenuItem.Click += new System.EventHandler(this.saveProfileToolStripMenuItem_Click);
            // 
            // saveProfileAsToolStripMenuItem
            // 
            this.saveProfileAsToolStripMenuItem.Name = "saveProfileAsToolStripMenuItem";
            this.saveProfileAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveProfileAsToolStripMenuItem.Text = "Save profile as...";
            this.saveProfileAsToolStripMenuItem.Click += new System.EventHandler(this.saveProfileAsToolStripMenuItem_Click);
            // 
            // newProfileToolStripMenuItem
            // 
            this.newProfileToolStripMenuItem.Name = "newProfileToolStripMenuItem";
            this.newProfileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newProfileToolStripMenuItem.Text = "New profile";
            this.newProfileToolStripMenuItem.Click += new System.EventHandler(this.newProfileToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterSupportedPidsToolStripMenuItem,
            this.connectDisconnectToolStripMenuItem,
            this.parseLogfileToBinToolStripMenuItem});
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
            // parseLogfileToBinToolStripMenuItem
            // 
            this.parseLogfileToBinToolStripMenuItem.Name = "parseLogfileToBinToolStripMenuItem";
            this.parseLogfileToBinToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.parseLogfileToBinToolStripMenuItem.Text = "Parse logfile to bin";
            this.parseLogfileToBinToolStripMenuItem.Click += new System.EventHandler(this.parseLogfileToBinToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.creditsToolStripMenuItem,
            this.homepageToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.creditsToolStripMenuItem.Text = "Credits";
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // homepageToolStripMenuItem
            // 
            this.homepageToolStripMenuItem.Name = "homepageToolStripMenuItem";
            this.homepageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.homepageToolStripMenuItem.Text = "Homepage";
            this.homepageToolStripMenuItem.Click += new System.EventHandler(this.homepageToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 62);
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
            this.splitContainer2.Size = new System.Drawing.Size(906, 535);
            this.splitContainer2.SplitterDistance = 422;
            this.splitContainer2.TabIndex = 5;
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
            this.groupHWSettings.ResumeLayout(false);
            this.categories.ResumeLayout(false);
            this.categories.PerformLayout();
            this.j2534OptionsGroupBox.ResumeLayout(false);
            this.j2534OptionsGroupBox.PerformLayout();
            this.serialOptionsGroupBox.ResumeLayout(false);
            this.serialOptionsGroupBox.PerformLayout();
            this.groupAdvanced.ResumeLayout(false);
            this.groupAdvanced.PerformLayout();
            this.tabVPWConsole.ResumeLayout(false);
            this.tabVPWConsole.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numConsoleScriptDelay)).EndInit();
            this.tabJConsole.ResumeLayout(false);
            this.tabJConsole.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJConsoleScriptDelay)).EndInit();
            this.groupJ2534Options.ResumeLayout(false);
            this.groupJ2534Options.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJ2534PeriodicMsgInterval)).EndInit();
            this.tabAlgoTest.ResumeLayout(false);
            this.tabAlgoTest.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.CheckBox chkVPWFilters;
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
        private System.Windows.Forms.TextBox txtSendBus;
        private System.Windows.Forms.RichTextBox richVPWmessages;
        private System.Windows.Forms.CheckBox chkEnableConsole;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem homepageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnSaveAnalyzerMsgs;
        private System.Windows.Forms.CheckBox chkConsoleTimestamps;
        private System.Windows.Forms.TabPage tabVPWConsole;
        private System.Windows.Forms.Button btnConsoleLoadScript;
        private System.Windows.Forms.Button btnDtcCustom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDtcCustomMode;
        private System.Windows.Forms.Button btnStartStopAnalyzer;
        private System.Windows.Forms.CheckBox chkConsoleUseJ2534Timestamps;
        private System.Windows.Forms.Button btnConnect2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numConsoleScriptDelay;
        private System.Windows.Forms.TextBox txtDtcCustomModule;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkConsole4x;
        private System.Windows.Forms.Button btnConsoleRefresh;
        private System.Windows.Forms.CheckBox chkConsoleAutorefresh;
        private System.Windows.Forms.ComboBox comboJ2534Baudrate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboJ2534Protocol;
        private System.Windows.Forms.GroupBox groupJ2534Options;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboJ2534Connectflag;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox comboJ2534DLL;
        private System.Windows.Forms.TextBox txtJ2534SetPins;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox comboJ2534Init;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtJ2534InitBytes;
        private System.Windows.Forms.Button btnJ2534SettingsLoad;
        private System.Windows.Forms.Button btnJ2534SettingsSaveAs;
        private System.Windows.Forms.NumericUpDown numJ2534PeriodicMsgInterval;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtJ2534PeriodicMsg;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TabPage tabJConsole;
        private System.Windows.Forms.RichTextBox richJConsole;
        private System.Windows.Forms.TextBox txtJConsoleSend;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnJConsoleUploadScript;
        private System.Windows.Forms.NumericUpDown numJConsoleScriptDelay;
        private System.Windows.Forms.CheckBox chkJConsole4x;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.CheckBox chkJConsoleTimestamps;
        private System.Windows.Forms.Button btnJConsoleConnect;
        private System.Windows.Forms.Label labelJconsoleConnected;
        private System.Windows.Forms.Button btnJconsoleStopScript;
        private System.Windows.Forms.Button btnStopScript;
        private System.Windows.Forms.TabPage tabAlgoTest;
        private System.Windows.Forms.Button btnAlgoTest;
        private System.Windows.Forms.TextBox txtAlgoTest;
        private System.Windows.Forms.TextBox txtSeed;
        private System.Windows.Forms.TextBox txtAlgo;
        private System.Windows.Forms.Label labelSeed;
        private System.Windows.Forms.Label labelAlgo;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioFindAlgo;
        private System.Windows.Forms.RadioButton radioFindAllKeys;
        private System.Windows.Forms.RadioButton radioFindKey;
        private System.Windows.Forms.ToolStripMenuItem parseLogfileToBinToolStripMenuItem;
        private System.Windows.Forms.Button btnEmulatorLoadResponses;
        private System.Windows.Forms.CheckBox ChkEmulatorResponseMode;
        private System.Windows.Forms.Button btnEmulatorEditResponses;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtEmulatorId;
        private System.Windows.Forms.Label label21;
    }
}

