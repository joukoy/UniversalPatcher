
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
            this.btnConnect2 = new System.Windows.Forms.Button();
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
            this.groupAdvanced = new System.Windows.Forms.GroupBox();
            this.chkFilterParamsByOS = new System.Windows.Forms.CheckBox();
            this.chkVPWFilters = new System.Windows.Forms.CheckBox();
            this.chkPriority = new System.Windows.Forms.CheckBox();
            this.chkRawValues = new System.Windows.Forms.CheckBox();
            this.chkReverseSlotNumbers = new System.Windows.Forms.CheckBox();
            this.labelResponseMode = new System.Windows.Forms.Label();
            this.comboResponseMode = new System.Windows.Forms.ComboBox();
            this.tabVPWConsole = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupJ2534Options = new System.Windows.Forms.GroupBox();
            this.numJ2534PeriodicMsgInterval = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.txtJ2534PeriodicMsg = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnJ2534SettingsLoad = new System.Windows.Forms.Button();
            this.btnJ2534SettingsSaveAs = new System.Windows.Forms.Button();
            this.txtJ2534InitBytes = new System.Windows.Forms.TextBox();
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.chkConsoleAutorefresh = new System.Windows.Forms.CheckBox();
            this.btnConsoleRefresh = new System.Windows.Forms.Button();
            this.chkConsole4x = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numConsoleScriptDelay = new System.Windows.Forms.NumericUpDown();
            this.chkConsoleUseJ2534Timestamps = new System.Windows.Forms.CheckBox();
            this.chkEnableConsole = new System.Windows.Forms.CheckBox();
            this.chkConsoleTimestamps = new System.Windows.Forms.CheckBox();
            this.btnConsoleLoadScript = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.txtVPWmessages = new System.Windows.Forms.RichTextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupJ2534Options.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJ2534PeriodicMsgInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConsoleScriptDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
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
            this.comboSerialPort.Location = new System.Drawing.Point(108, 62);
            this.comboSerialPort.Margin = new System.Windows.Forms.Padding(4);
            this.comboSerialPort.Name = "comboSerialPort";
            this.comboSerialPort.Size = new System.Drawing.Size(339, 24);
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
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1216, 520);
            this.tabControl1.TabIndex = 3;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.groupLogSettings);
            this.tabLog.Controls.Add(this.dataGridLogData);
            this.tabLog.Location = new System.Drawing.Point(4, 25);
            this.tabLog.Margin = new System.Windows.Forms.Padding(4);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(4);
            this.tabLog.Size = new System.Drawing.Size(1208, 491);
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
            this.groupLogSettings.Margin = new System.Windows.Forms.Padding(4);
            this.groupLogSettings.Name = "groupLogSettings";
            this.groupLogSettings.Padding = new System.Windows.Forms.Padding(4);
            this.groupLogSettings.Size = new System.Drawing.Size(584, 489);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 17);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(561, 126);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logfile";
            // 
            // chkWriteLog
            // 
            this.chkWriteLog.AutoSize = true;
            this.chkWriteLog.Checked = true;
            this.chkWriteLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriteLog.Location = new System.Drawing.Point(12, 90);
            this.chkWriteLog.Margin = new System.Windows.Forms.Padding(4);
            this.chkWriteLog.Name = "chkWriteLog";
            this.chkWriteLog.Size = new System.Drawing.Size(80, 20);
            this.chkWriteLog.TabIndex = 9;
            this.chkWriteLog.Text = "Write log";
            this.chkWriteLog.UseVisualStyleBackColor = true;
            // 
            // btnBrowsLogFolder
            // 
            this.btnBrowsLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsLogFolder.Location = new System.Drawing.Point(509, 49);
            this.btnBrowsLogFolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowsLogFolder.Name = "btnBrowsLogFolder";
            this.btnBrowsLogFolder.Size = new System.Drawing.Size(44, 28);
            this.btnBrowsLogFolder.TabIndex = 12;
            this.btnBrowsLogFolder.Text = "...";
            this.btnBrowsLogFolder.UseVisualStyleBackColor = true;
            this.btnBrowsLogFolder.Click += new System.EventHandler(this.btnBrowsLogFolder_Click);
            // 
            // txtLogSeparator
            // 
            this.txtLogSeparator.Location = new System.Drawing.Point(192, 87);
            this.txtLogSeparator.Margin = new System.Windows.Forms.Padding(4);
            this.txtLogSeparator.Name = "txtLogSeparator";
            this.txtLogSeparator.Size = new System.Drawing.Size(39, 22);
            this.txtLogSeparator.TabIndex = 14;
            this.txtLogSeparator.Text = ",";
            this.txtLogSeparator.TextChanged += new System.EventHandler(this.txtLogSeparator_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 16);
            this.label2.TabIndex = 11;
            this.label2.Text = "Logfolder:";
            // 
            // labelSeparator
            // 
            this.labelSeparator.AutoSize = true;
            this.labelSeparator.Location = new System.Drawing.Point(109, 91);
            this.labelSeparator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSeparator.Name = "labelSeparator";
            this.labelSeparator.Size = new System.Drawing.Size(71, 16);
            this.labelSeparator.TabIndex = 13;
            this.labelSeparator.Text = "Separator:";
            // 
            // txtLogFolder
            // 
            this.txtLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFolder.Location = new System.Drawing.Point(8, 52);
            this.txtLogFolder.Margin = new System.Windows.Forms.Padding(4);
            this.txtLogFolder.Name = "txtLogFolder";
            this.txtLogFolder.Size = new System.Drawing.Size(492, 22);
            this.txtLogFolder.TabIndex = 10;
            // 
            // listProfiles
            // 
            this.listProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listProfiles.FormattingEnabled = true;
            this.listProfiles.ItemHeight = 16;
            this.listProfiles.Location = new System.Drawing.Point(12, 167);
            this.listProfiles.Margin = new System.Windows.Forms.Padding(4);
            this.listProfiles.Name = "listProfiles";
            this.listProfiles.Size = new System.Drawing.Size(563, 276);
            this.listProfiles.TabIndex = 29;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 146);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 16);
            this.label3.TabIndex = 30;
            this.label3.Text = "Profile:";
            // 
            // dataGridLogData
            // 
            this.dataGridLogData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridLogData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLogData.Location = new System.Drawing.Point(591, 4);
            this.dataGridLogData.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridLogData.Name = "dataGridLogData";
            this.dataGridLogData.Size = new System.Drawing.Size(611, 477);
            this.dataGridLogData.TabIndex = 8;
            // 
            // tabProfile
            // 
            this.tabProfile.Controls.Add(this.splitContainer1);
            this.tabProfile.Location = new System.Drawing.Point(4, 25);
            this.tabProfile.Margin = new System.Windows.Forms.Padding(4);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Size = new System.Drawing.Size(1208, 491);
            this.tabProfile.TabIndex = 2;
            this.tabProfile.Text = "Profile";
            this.tabProfile.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
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
            this.splitContainer1.Size = new System.Drawing.Size(1208, 491);
            this.splitContainer1.SplitterDistance = 491;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtParamSearch);
            this.groupBox2.Controls.Add(this.radioParamMath);
            this.groupBox2.Controls.Add(this.radioParamRam);
            this.groupBox2.Controls.Add(this.radioParamStd);
            this.groupBox2.Location = new System.Drawing.Point(5, 4);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(484, 38);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // txtParamSearch
            // 
            this.txtParamSearch.Location = new System.Drawing.Point(271, 10);
            this.txtParamSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtParamSearch.Name = "txtParamSearch";
            this.txtParamSearch.Size = new System.Drawing.Size(204, 22);
            this.txtParamSearch.TabIndex = 3;
            this.txtParamSearch.Text = "Search...";
            // 
            // radioParamMath
            // 
            this.radioParamMath.AutoSize = true;
            this.radioParamMath.Location = new System.Drawing.Point(180, 11);
            this.radioParamMath.Margin = new System.Windows.Forms.Padding(4);
            this.radioParamMath.Name = "radioParamMath";
            this.radioParamMath.Size = new System.Drawing.Size(55, 20);
            this.radioParamMath.TabIndex = 2;
            this.radioParamMath.TabStop = true;
            this.radioParamMath.Text = "Math";
            this.radioParamMath.UseVisualStyleBackColor = true;
            this.radioParamMath.CheckedChanged += new System.EventHandler(this.radioParamMath_CheckedChanged);
            // 
            // radioParamRam
            // 
            this.radioParamRam.AutoSize = true;
            this.radioParamRam.Location = new System.Drawing.Point(107, 11);
            this.radioParamRam.Margin = new System.Windows.Forms.Padding(4);
            this.radioParamRam.Name = "radioParamRam";
            this.radioParamRam.Size = new System.Drawing.Size(56, 20);
            this.radioParamRam.TabIndex = 1;
            this.radioParamRam.Text = "RAM";
            this.radioParamRam.UseVisualStyleBackColor = true;
            this.radioParamRam.CheckedChanged += new System.EventHandler(this.radioParamRam_CheckedChanged);
            // 
            // radioParamStd
            // 
            this.radioParamStd.AutoSize = true;
            this.radioParamStd.Checked = true;
            this.radioParamStd.Location = new System.Drawing.Point(8, 11);
            this.radioParamStd.Margin = new System.Windows.Forms.Padding(4);
            this.radioParamStd.Name = "radioParamStd";
            this.radioParamStd.Size = new System.Drawing.Size(81, 20);
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
            this.dataGridPidNames.Location = new System.Drawing.Point(0, 43);
            this.dataGridPidNames.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridPidNames.Name = "dataGridPidNames";
            this.dataGridPidNames.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPidNames.Size = new System.Drawing.Size(491, 449);
            this.dataGridPidNames.TabIndex = 1;
            // 
            // btnQueryPid
            // 
            this.btnQueryPid.Location = new System.Drawing.Point(4, 389);
            this.btnQueryPid.Margin = new System.Windows.Forms.Padding(4);
            this.btnQueryPid.Name = "btnQueryPid";
            this.btnQueryPid.Size = new System.Drawing.Size(37, 33);
            this.btnQueryPid.TabIndex = 4;
            this.btnQueryPid.Text = "?";
            this.btnQueryPid.UseVisualStyleBackColor = true;
            this.btnQueryPid.Click += new System.EventHandler(this.btnQueryPid_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(4, 94);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(37, 28);
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
            this.dataGridLogProfile.Location = new System.Drawing.Point(49, 0);
            this.dataGridLogProfile.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridLogProfile.Name = "dataGridLogProfile";
            this.dataGridLogProfile.Size = new System.Drawing.Size(658, 487);
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
            this.btnRemove.Location = new System.Drawing.Point(4, 140);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(37, 28);
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
            this.tabAnalyzer.Location = new System.Drawing.Point(4, 25);
            this.tabAnalyzer.Margin = new System.Windows.Forms.Padding(4);
            this.tabAnalyzer.Name = "tabAnalyzer";
            this.tabAnalyzer.Size = new System.Drawing.Size(1208, 491);
            this.tabAnalyzer.TabIndex = 4;
            this.tabAnalyzer.Text = "Analyzer";
            this.tabAnalyzer.UseVisualStyleBackColor = true;
            // 
            // btnStartStopAnalyzer
            // 
            this.btnStartStopAnalyzer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStopAnalyzer.Location = new System.Drawing.Point(1025, 4);
            this.btnStartStopAnalyzer.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStopAnalyzer.Name = "btnStartStopAnalyzer";
            this.btnStartStopAnalyzer.Size = new System.Drawing.Size(169, 28);
            this.btnStartStopAnalyzer.TabIndex = 8;
            this.btnStartStopAnalyzer.Text = "Start Analyzer";
            this.btnStartStopAnalyzer.UseVisualStyleBackColor = true;
            this.btnStartStopAnalyzer.Click += new System.EventHandler(this.btnStartStopAnalyzer_Click);
            // 
            // btnSaveAnalyzerMsgs
            // 
            this.btnSaveAnalyzerMsgs.Location = new System.Drawing.Point(372, 4);
            this.btnSaveAnalyzerMsgs.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveAnalyzerMsgs.Name = "btnSaveAnalyzerMsgs";
            this.btnSaveAnalyzerMsgs.Size = new System.Drawing.Size(141, 27);
            this.btnSaveAnalyzerMsgs.TabIndex = 7;
            this.btnSaveAnalyzerMsgs.Text = "Save messages";
            this.btnSaveAnalyzerMsgs.UseVisualStyleBackColor = true;
            this.btnSaveAnalyzerMsgs.Click += new System.EventHandler(this.btnSaveAnalyzerMsgs_Click);
            // 
            // btnAnalyzerSaveCsv
            // 
            this.btnAnalyzerSaveCsv.Location = new System.Drawing.Point(264, 2);
            this.btnAnalyzerSaveCsv.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnalyzerSaveCsv.Name = "btnAnalyzerSaveCsv";
            this.btnAnalyzerSaveCsv.Size = new System.Drawing.Size(100, 28);
            this.btnAnalyzerSaveCsv.TabIndex = 6;
            this.btnAnalyzerSaveCsv.Text = "Save CSV";
            this.btnAnalyzerSaveCsv.UseVisualStyleBackColor = true;
            this.btnAnalyzerSaveCsv.Click += new System.EventHandler(this.btnAnalyzerSaveCsv_Click);
            // 
            // btnClearAnalyzerGrid
            // 
            this.btnClearAnalyzerGrid.Location = new System.Drawing.Point(156, 4);
            this.btnClearAnalyzerGrid.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearAnalyzerGrid.Name = "btnClearAnalyzerGrid";
            this.btnClearAnalyzerGrid.Size = new System.Drawing.Size(100, 26);
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
            this.chkHideHeartBeat.Location = new System.Drawing.Point(11, 10);
            this.chkHideHeartBeat.Margin = new System.Windows.Forms.Padding(4);
            this.chkHideHeartBeat.Name = "chkHideHeartBeat";
            this.chkHideHeartBeat.Size = new System.Drawing.Size(119, 20);
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
            this.dataGridAnalyzer.Location = new System.Drawing.Point(0, 34);
            this.dataGridAnalyzer.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridAnalyzer.Name = "dataGridAnalyzer";
            this.dataGridAnalyzer.Size = new System.Drawing.Size(1203, 449);
            this.dataGridAnalyzer.TabIndex = 3;
            // 
            // tabDTC
            // 
            this.tabDTC.Controls.Add(this.groupDTC);
            this.tabDTC.Controls.Add(this.dataGridDtcCodes);
            this.tabDTC.Location = new System.Drawing.Point(4, 25);
            this.tabDTC.Margin = new System.Windows.Forms.Padding(4);
            this.tabDTC.Name = "tabDTC";
            this.tabDTC.Size = new System.Drawing.Size(1208, 491);
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
            this.groupDTC.Location = new System.Drawing.Point(4, 4);
            this.groupDTC.Margin = new System.Windows.Forms.Padding(4);
            this.groupDTC.Name = "groupDTC";
            this.groupDTC.Padding = new System.Windows.Forms.Padding(4);
            this.groupDTC.Size = new System.Drawing.Size(265, 481);
            this.groupDTC.TabIndex = 9;
            this.groupDTC.TabStop = false;
            // 
            // txtDtcCustomModule
            // 
            this.txtDtcCustomModule.Location = new System.Drawing.Point(191, 386);
            this.txtDtcCustomModule.Margin = new System.Windows.Forms.Padding(4);
            this.txtDtcCustomModule.Name = "txtDtcCustomModule";
            this.txtDtcCustomModule.Size = new System.Drawing.Size(51, 22);
            this.txtDtcCustomModule.TabIndex = 14;
            this.txtDtcCustomModule.Text = "10";
            this.txtDtcCustomModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 386);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 16);
            this.label10.TabIndex = 13;
            this.label10.Text = "Custom module:";
            // 
            // txtDtcCustomMode
            // 
            this.txtDtcCustomMode.Location = new System.Drawing.Point(191, 357);
            this.txtDtcCustomMode.Margin = new System.Windows.Forms.Padding(4);
            this.txtDtcCustomMode.Name = "txtDtcCustomMode";
            this.txtDtcCustomMode.Size = new System.Drawing.Size(51, 22);
            this.txtDtcCustomMode.TabIndex = 12;
            this.txtDtcCustomMode.Text = "10";
            this.txtDtcCustomMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnDtcCustom
            // 
            this.btnDtcCustom.Location = new System.Drawing.Point(27, 418);
            this.btnDtcCustom.Margin = new System.Windows.Forms.Padding(4);
            this.btnDtcCustom.Name = "btnDtcCustom";
            this.btnDtcCustom.Size = new System.Drawing.Size(216, 36);
            this.btnDtcCustom.TabIndex = 11;
            this.btnDtcCustom.Text = "Get custom DTC codes";
            this.btnDtcCustom.UseVisualStyleBackColor = true;
            this.btnDtcCustom.Click += new System.EventHandler(this.btnDtcCustom_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(31, 361);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Custom dtc:";
            // 
            // btnHistoryDTC
            // 
            this.btnHistoryDTC.Location = new System.Drawing.Point(27, 118);
            this.btnHistoryDTC.Margin = new System.Windows.Forms.Padding(4);
            this.btnHistoryDTC.Name = "btnHistoryDTC";
            this.btnHistoryDTC.Size = new System.Drawing.Size(216, 36);
            this.btnHistoryDTC.TabIndex = 1;
            this.btnHistoryDTC.Text = "Get History DTC codes";
            this.btnHistoryDTC.UseVisualStyleBackColor = true;
            this.btnHistoryDTC.Click += new System.EventHandler(this.btnHistoryDTC_Click);
            // 
            // btnGetVINCode
            // 
            this.btnGetVINCode.Location = new System.Drawing.Point(27, 204);
            this.btnGetVINCode.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetVINCode.Name = "btnGetVINCode";
            this.btnGetVINCode.Size = new System.Drawing.Size(216, 36);
            this.btnGetVINCode.TabIndex = 8;
            this.btnGetVINCode.Text = "Get VIN Code";
            this.btnGetVINCode.UseVisualStyleBackColor = true;
            this.btnGetVINCode.Click += new System.EventHandler(this.btnGetVINCode_Click);
            // 
            // btnCurrentDTC
            // 
            this.btnCurrentDTC.Location = new System.Drawing.Point(27, 75);
            this.btnCurrentDTC.Margin = new System.Windows.Forms.Padding(4);
            this.btnCurrentDTC.Name = "btnCurrentDTC";
            this.btnCurrentDTC.Size = new System.Drawing.Size(216, 36);
            this.btnCurrentDTC.TabIndex = 0;
            this.btnCurrentDTC.Text = "Get current DTC codes";
            this.btnCurrentDTC.UseVisualStyleBackColor = true;
            this.btnCurrentDTC.Click += new System.EventHandler(this.btnCurrentDTC_Click);
            // 
            // comboModule
            // 
            this.comboModule.FormattingEnabled = true;
            this.comboModule.Location = new System.Drawing.Point(103, 42);
            this.comboModule.Margin = new System.Windows.Forms.Padding(4);
            this.comboModule.Name = "comboModule";
            this.comboModule.Size = new System.Drawing.Size(139, 24);
            this.comboModule.TabIndex = 2;
            // 
            // chkDtcAllModules
            // 
            this.chkDtcAllModules.AutoSize = true;
            this.chkDtcAllModules.Location = new System.Drawing.Point(117, 14);
            this.chkDtcAllModules.Margin = new System.Windows.Forms.Padding(4);
            this.chkDtcAllModules.Name = "chkDtcAllModules";
            this.chkDtcAllModules.Size = new System.Drawing.Size(97, 20);
            this.chkDtcAllModules.TabIndex = 6;
            this.chkDtcAllModules.Text = "All modules";
            this.chkDtcAllModules.UseVisualStyleBackColor = true;
            this.chkDtcAllModules.CheckedChanged += new System.EventHandler(this.chkDtcAllModules_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(31, 46);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 3;
            this.label8.Text = "Module:";
            // 
            // btnClearCodes
            // 
            this.btnClearCodes.Location = new System.Drawing.Point(27, 161);
            this.btnClearCodes.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearCodes.Name = "btnClearCodes";
            this.btnClearCodes.Size = new System.Drawing.Size(216, 36);
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
            this.dataGridDtcCodes.Location = new System.Drawing.Point(277, 0);
            this.dataGridDtcCodes.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridDtcCodes.Name = "dataGridDtcCodes";
            this.dataGridDtcCodes.Size = new System.Drawing.Size(923, 483);
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
            this.tabSettings.Controls.Add(this.btnConnect2);
            this.tabSettings.Controls.Add(this.groupHWSettings);
            this.tabSettings.Controls.Add(this.groupAdvanced);
            this.tabSettings.Location = new System.Drawing.Point(4, 25);
            this.tabSettings.Margin = new System.Windows.Forms.Padding(4);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(1208, 491);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // btnConnect2
            // 
            this.btnConnect2.Location = new System.Drawing.Point(512, 235);
            this.btnConnect2.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect2.Name = "btnConnect2";
            this.btnConnect2.Size = new System.Drawing.Size(219, 44);
            this.btnConnect2.TabIndex = 23;
            this.btnConnect2.Text = "Connect/ Disconnect";
            this.btnConnect2.UseVisualStyleBackColor = true;
            this.btnConnect2.Click += new System.EventHandler(this.btnConnect2_Click);
            // 
            // groupHWSettings
            // 
            this.groupHWSettings.Controls.Add(this.categories);
            this.groupHWSettings.Controls.Add(this.j2534OptionsGroupBox);
            this.groupHWSettings.Controls.Add(this.serialOptionsGroupBox);
            this.groupHWSettings.Location = new System.Drawing.Point(11, 4);
            this.groupHWSettings.Margin = new System.Windows.Forms.Padding(4);
            this.groupHWSettings.Name = "groupHWSettings";
            this.groupHWSettings.Padding = new System.Windows.Forms.Padding(4);
            this.groupHWSettings.Size = new System.Drawing.Size(481, 297);
            this.groupHWSettings.TabIndex = 22;
            this.groupHWSettings.TabStop = false;
            // 
            // categories
            // 
            this.categories.Controls.Add(this.j2534RadioButton);
            this.categories.Controls.Add(this.serialRadioButton);
            this.categories.Location = new System.Drawing.Point(8, 23);
            this.categories.Margin = new System.Windows.Forms.Padding(4);
            this.categories.Name = "categories";
            this.categories.Padding = new System.Windows.Forms.Padding(4);
            this.categories.Size = new System.Drawing.Size(232, 87);
            this.categories.TabIndex = 18;
            this.categories.TabStop = false;
            this.categories.Text = "Device &Category";
            // 
            // j2534RadioButton
            // 
            this.j2534RadioButton.AutoSize = true;
            this.j2534RadioButton.Location = new System.Drawing.Point(9, 57);
            this.j2534RadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.j2534RadioButton.Name = "j2534RadioButton";
            this.j2534RadioButton.Size = new System.Drawing.Size(107, 20);
            this.j2534RadioButton.TabIndex = 1;
            this.j2534RadioButton.Text = "&J2534 Device";
            this.j2534RadioButton.UseVisualStyleBackColor = true;
            this.j2534RadioButton.CheckedChanged += new System.EventHandler(this.j2534RadioButton_CheckedChanged);
            // 
            // serialRadioButton
            // 
            this.serialRadioButton.AutoSize = true;
            this.serialRadioButton.Checked = true;
            this.serialRadioButton.Location = new System.Drawing.Point(9, 25);
            this.serialRadioButton.Margin = new System.Windows.Forms.Padding(4);
            this.serialRadioButton.Name = "serialRadioButton";
            this.serialRadioButton.Size = new System.Drawing.Size(134, 20);
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
            this.j2534OptionsGroupBox.Location = new System.Drawing.Point(252, 23);
            this.j2534OptionsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.j2534OptionsGroupBox.Name = "j2534OptionsGroupBox";
            this.j2534OptionsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.j2534OptionsGroupBox.Size = new System.Drawing.Size(220, 89);
            this.j2534OptionsGroupBox.TabIndex = 19;
            this.j2534OptionsGroupBox.TabStop = false;
            this.j2534OptionsGroupBox.Text = "J2534 Device Options";
            // 
            // j2534DeviceList
            // 
            this.j2534DeviceList.FormattingEnabled = true;
            this.j2534DeviceList.Location = new System.Drawing.Point(9, 46);
            this.j2534DeviceList.Margin = new System.Windows.Forms.Padding(4);
            this.j2534DeviceList.Name = "j2534DeviceList";
            this.j2534DeviceList.Size = new System.Drawing.Size(193, 24);
            this.j2534DeviceList.TabIndex = 1;
            this.j2534DeviceList.SelectedIndexChanged += new System.EventHandler(this.j2534DeviceList_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 16);
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
            this.serialOptionsGroupBox.Location = new System.Drawing.Point(8, 118);
            this.serialOptionsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.serialOptionsGroupBox.Name = "serialOptionsGroupBox";
            this.serialOptionsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.serialOptionsGroupBox.Size = new System.Drawing.Size(464, 174);
            this.serialOptionsGroupBox.TabIndex = 20;
            this.serialOptionsGroupBox.TabStop = false;
            this.serialOptionsGroupBox.Text = "Serialport options";
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.FormattingEnabled = true;
            this.comboBaudRate.Location = new System.Drawing.Point(107, 132);
            this.comboBaudRate.Margin = new System.Windows.Forms.Padding(4);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(340, 24);
            this.comboBaudRate.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 142);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 16);
            this.label7.TabIndex = 19;
            this.label7.Text = "Baudrate:";
            // 
            // chkFTDI
            // 
            this.chkFTDI.AutoSize = true;
            this.chkFTDI.Location = new System.Drawing.Point(13, 23);
            this.chkFTDI.Margin = new System.Windows.Forms.Padding(4);
            this.chkFTDI.Name = "chkFTDI";
            this.chkFTDI.Size = new System.Drawing.Size(85, 20);
            this.chkFTDI.TabIndex = 18;
            this.chkFTDI.Text = "Use FTDI";
            this.chkFTDI.UseVisualStyleBackColor = true;
            this.chkFTDI.CheckedChanged += new System.EventHandler(this.chkFTDI_CheckedChanged);
            // 
            // comboSerialDeviceType
            // 
            this.comboSerialDeviceType.FormattingEnabled = true;
            this.comboSerialDeviceType.Location = new System.Drawing.Point(108, 96);
            this.comboSerialDeviceType.Margin = new System.Windows.Forms.Padding(4);
            this.comboSerialDeviceType.Name = "comboSerialDeviceType";
            this.comboSerialDeviceType.Size = new System.Drawing.Size(339, 24);
            this.comboSerialDeviceType.TabIndex = 17;
            this.comboSerialDeviceType.SelectedIndexChanged += new System.EventHandler(this.comboSerialDeviceType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 103);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 16);
            this.label4.TabIndex = 16;
            this.label4.Text = "Device Type";
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
            this.groupAdvanced.Location = new System.Drawing.Point(500, 14);
            this.groupAdvanced.Margin = new System.Windows.Forms.Padding(4);
            this.groupAdvanced.Name = "groupAdvanced";
            this.groupAdvanced.Padding = new System.Windows.Forms.Padding(4);
            this.groupAdvanced.Size = new System.Drawing.Size(252, 190);
            this.groupAdvanced.TabIndex = 34;
            this.groupAdvanced.TabStop = false;
            this.groupAdvanced.Text = "Advanced settings";
            // 
            // chkFilterParamsByOS
            // 
            this.chkFilterParamsByOS.AutoSize = true;
            this.chkFilterParamsByOS.Checked = true;
            this.chkFilterParamsByOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFilterParamsByOS.Location = new System.Drawing.Point(12, 87);
            this.chkFilterParamsByOS.Margin = new System.Windows.Forms.Padding(4);
            this.chkFilterParamsByOS.Name = "chkFilterParamsByOS";
            this.chkFilterParamsByOS.Size = new System.Drawing.Size(168, 20);
            this.chkFilterParamsByOS.TabIndex = 36;
            this.chkFilterParamsByOS.Text = "Filter parameters by OS";
            this.chkFilterParamsByOS.UseVisualStyleBackColor = true;
            // 
            // chkVPWFilters
            // 
            this.chkVPWFilters.AutoSize = true;
            this.chkVPWFilters.Location = new System.Drawing.Point(12, 65);
            this.chkVPWFilters.Margin = new System.Windows.Forms.Padding(4);
            this.chkVPWFilters.Name = "chkVPWFilters";
            this.chkVPWFilters.Size = new System.Drawing.Size(125, 20);
            this.chkVPWFilters.TabIndex = 35;
            this.chkVPWFilters.Text = "Use VPW Filters";
            this.chkVPWFilters.UseVisualStyleBackColor = true;
            this.chkVPWFilters.CheckedChanged += new System.EventHandler(this.chkBusFilters_CheckedChanged);
            // 
            // chkPriority
            // 
            this.chkPriority.AutoSize = true;
            this.chkPriority.Location = new System.Drawing.Point(12, 110);
            this.chkPriority.Margin = new System.Windows.Forms.Padding(4);
            this.chkPriority.Name = "chkPriority";
            this.chkPriority.Size = new System.Drawing.Size(96, 20);
            this.chkPriority.TabIndex = 34;
            this.chkPriority.Text = "Use Priority";
            this.chkPriority.UseVisualStyleBackColor = true;
            this.chkPriority.CheckedChanged += new System.EventHandler(this.chkPriority_CheckedChanged);
            // 
            // chkRawValues
            // 
            this.chkRawValues.AutoSize = true;
            this.chkRawValues.Location = new System.Drawing.Point(12, 23);
            this.chkRawValues.Margin = new System.Windows.Forms.Padding(4);
            this.chkRawValues.Name = "chkRawValues";
            this.chkRawValues.Size = new System.Drawing.Size(97, 20);
            this.chkRawValues.TabIndex = 26;
            this.chkRawValues.Text = "Raw values";
            this.chkRawValues.UseVisualStyleBackColor = true;
            // 
            // chkReverseSlotNumbers
            // 
            this.chkReverseSlotNumbers.AutoSize = true;
            this.chkReverseSlotNumbers.Checked = true;
            this.chkReverseSlotNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReverseSlotNumbers.Location = new System.Drawing.Point(12, 44);
            this.chkReverseSlotNumbers.Margin = new System.Windows.Forms.Padding(4);
            this.chkReverseSlotNumbers.Name = "chkReverseSlotNumbers";
            this.chkReverseSlotNumbers.Size = new System.Drawing.Size(182, 20);
            this.chkReverseSlotNumbers.TabIndex = 27;
            this.chkReverseSlotNumbers.Text = "Slot numbers start from FE";
            this.chkReverseSlotNumbers.UseVisualStyleBackColor = true;
            // 
            // labelResponseMode
            // 
            this.labelResponseMode.AutoSize = true;
            this.labelResponseMode.Location = new System.Drawing.Point(8, 130);
            this.labelResponseMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResponseMode.Name = "labelResponseMode";
            this.labelResponseMode.Size = new System.Drawing.Size(112, 16);
            this.labelResponseMode.TabIndex = 24;
            this.labelResponseMode.Text = "Response Mode:";
            // 
            // comboResponseMode
            // 
            this.comboResponseMode.FormattingEnabled = true;
            this.comboResponseMode.Location = new System.Drawing.Point(8, 150);
            this.comboResponseMode.Margin = new System.Windows.Forms.Padding(4);
            this.comboResponseMode.Name = "comboResponseMode";
            this.comboResponseMode.Size = new System.Drawing.Size(221, 24);
            this.comboResponseMode.TabIndex = 23;
            this.comboResponseMode.SelectedIndexChanged += new System.EventHandler(this.comboResponseMode_SelectedIndexChanged);
            // 
            // tabVPWConsole
            // 
            this.tabVPWConsole.Controls.Add(this.splitContainer4);
            this.tabVPWConsole.Location = new System.Drawing.Point(4, 25);
            this.tabVPWConsole.Margin = new System.Windows.Forms.Padding(4);
            this.tabVPWConsole.Name = "tabVPWConsole";
            this.tabVPWConsole.Size = new System.Drawing.Size(1208, 491);
            this.tabVPWConsole.TabIndex = 6;
            this.tabVPWConsole.Text = "VPW Console";
            this.tabVPWConsole.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer4.Size = new System.Drawing.Size(1208, 491);
            this.splitContainer4.SplitterDistance = 483;
            this.splitContainer4.SplitterWidth = 5;
            this.splitContainer4.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupJ2534Options);
            this.groupBox3.Controls.Add(this.btnConnect);
            this.groupBox3.Controls.Add(this.chkConsoleAutorefresh);
            this.groupBox3.Controls.Add(this.btnConsoleRefresh);
            this.groupBox3.Controls.Add(this.chkConsole4x);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numConsoleScriptDelay);
            this.groupBox3.Controls.Add(this.chkConsoleUseJ2534Timestamps);
            this.groupBox3.Controls.Add(this.chkEnableConsole);
            this.groupBox3.Controls.Add(this.chkConsoleTimestamps);
            this.groupBox3.Controls.Add(this.btnConsoleLoadScript);
            this.groupBox3.Location = new System.Drawing.Point(11, 18);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(468, 464);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "VPW Console";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
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
            this.groupJ2534Options.Enabled = false;
            this.groupJ2534Options.Location = new System.Drawing.Point(185, 12);
            this.groupJ2534Options.Margin = new System.Windows.Forms.Padding(4);
            this.groupJ2534Options.Name = "groupJ2534Options";
            this.groupJ2534Options.Padding = new System.Windows.Forms.Padding(4);
            this.groupJ2534Options.Size = new System.Drawing.Size(275, 354);
            this.groupJ2534Options.TabIndex = 50;
            this.groupJ2534Options.TabStop = false;
            this.groupJ2534Options.Text = "J2534 options";
            // 
            // numJ2534PeriodicMsgInterval
            // 
            this.numJ2534PeriodicMsgInterval.Location = new System.Drawing.Point(111, 261);
            this.numJ2534PeriodicMsgInterval.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.numJ2534PeriodicMsgInterval.Name = "numJ2534PeriodicMsgInterval";
            this.numJ2534PeriodicMsgInterval.Size = new System.Drawing.Size(151, 22);
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
            this.label19.Location = new System.Drawing.Point(10, 263);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(83, 16);
            this.label19.TabIndex = 63;
            this.label19.Text = "Interval: (ms)";
            // 
            // txtJ2534PeriodicMsg
            // 
            this.txtJ2534PeriodicMsg.Location = new System.Drawing.Point(110, 233);
            this.txtJ2534PeriodicMsg.Name = "txtJ2534PeriodicMsg";
            this.txtJ2534PeriodicMsg.Size = new System.Drawing.Size(152, 22);
            this.txtJ2534PeriodicMsg.TabIndex = 62;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 233);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(90, 16);
            this.label18.TabIndex = 61;
            this.label18.Text = "Periodic msg:";
            // 
            // btnJ2534SettingsLoad
            // 
            this.btnJ2534SettingsLoad.Location = new System.Drawing.Point(158, 304);
            this.btnJ2534SettingsLoad.Name = "btnJ2534SettingsLoad";
            this.btnJ2534SettingsLoad.Size = new System.Drawing.Size(105, 26);
            this.btnJ2534SettingsLoad.TabIndex = 60;
            this.btnJ2534SettingsLoad.Text = "Load...";
            this.btnJ2534SettingsLoad.UseVisualStyleBackColor = true;
            this.btnJ2534SettingsLoad.Click += new System.EventHandler(this.btnJ2534SettingsLoad_Click);
            // 
            // btnJ2534SettingsSaveAs
            // 
            this.btnJ2534SettingsSaveAs.Location = new System.Drawing.Point(11, 304);
            this.btnJ2534SettingsSaveAs.Name = "btnJ2534SettingsSaveAs";
            this.btnJ2534SettingsSaveAs.Size = new System.Drawing.Size(105, 26);
            this.btnJ2534SettingsSaveAs.TabIndex = 59;
            this.btnJ2534SettingsSaveAs.Text = "Save as...";
            this.btnJ2534SettingsSaveAs.UseVisualStyleBackColor = true;
            this.btnJ2534SettingsSaveAs.Click += new System.EventHandler(this.btnJ2534SettingsSaveAs_Click);
            // 
            // txtJ2534InitBytes
            // 
            this.txtJ2534InitBytes.Location = new System.Drawing.Point(110, 204);
            this.txtJ2534InitBytes.Margin = new System.Windows.Forms.Padding(4);
            this.txtJ2534InitBytes.Name = "txtJ2534InitBytes";
            this.txtJ2534InitBytes.Size = new System.Drawing.Size(152, 22);
            this.txtJ2534InitBytes.TabIndex = 58;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 207);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 16);
            this.label17.TabIndex = 57;
            this.label17.Text = "Init bytes:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 177);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(27, 16);
            this.label16.TabIndex = 56;
            this.label16.Text = "Init:";
            // 
            // comboJ2534Init
            // 
            this.comboJ2534Init.FormattingEnabled = true;
            this.comboJ2534Init.Location = new System.Drawing.Point(111, 174);
            this.comboJ2534Init.Margin = new System.Windows.Forms.Padding(4);
            this.comboJ2534Init.Name = "comboJ2534Init";
            this.comboJ2534Init.Size = new System.Drawing.Size(152, 24);
            this.comboJ2534Init.TabIndex = 55;
            this.comboJ2534Init.SelectedIndexChanged += new System.EventHandler(this.comboJ2534Init_SelectedIndexChanged);
            // 
            // txtJ2534SetPins
            // 
            this.txtJ2534SetPins.Location = new System.Drawing.Point(111, 144);
            this.txtJ2534SetPins.Margin = new System.Windows.Forms.Padding(4);
            this.txtJ2534SetPins.Name = "txtJ2534SetPins";
            this.txtJ2534SetPins.Size = new System.Drawing.Size(152, 22);
            this.txtJ2534SetPins.TabIndex = 54;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 147);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 16);
            this.label15.TabIndex = 53;
            this.label15.Text = "Set pins:";
            // 
            // comboJ2534DLL
            // 
            this.comboJ2534DLL.FormattingEnabled = true;
            this.comboJ2534DLL.Location = new System.Drawing.Point(111, 31);
            this.comboJ2534DLL.Margin = new System.Windows.Forms.Padding(4);
            this.comboJ2534DLL.Name = "comboJ2534DLL";
            this.comboJ2534DLL.Size = new System.Drawing.Size(153, 24);
            this.comboJ2534DLL.TabIndex = 52;
            this.comboJ2534DLL.SelectedIndexChanged += new System.EventHandler(this.comboJ2534DLL_SelectedIndexChanged);
            // 
            // comboJ2534Connectflag
            // 
            this.comboJ2534Connectflag.FormattingEnabled = true;
            this.comboJ2534Connectflag.Location = new System.Drawing.Point(110, 114);
            this.comboJ2534Connectflag.Margin = new System.Windows.Forms.Padding(4);
            this.comboJ2534Connectflag.Name = "comboJ2534Connectflag";
            this.comboJ2534Connectflag.Size = new System.Drawing.Size(153, 24);
            this.comboJ2534Connectflag.TabIndex = 50;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 34);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(86, 16);
            this.label14.TabIndex = 51;
            this.label14.Text = "&Device Type";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 117);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 16);
            this.label13.TabIndex = 49;
            this.label13.Text = "Connectflag:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 61);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 16);
            this.label11.TabIndex = 46;
            this.label11.Text = "Protocol:";
            // 
            // comboJ2534Baudrate
            // 
            this.comboJ2534Baudrate.FormattingEnabled = true;
            this.comboJ2534Baudrate.Location = new System.Drawing.Point(110, 85);
            this.comboJ2534Baudrate.Margin = new System.Windows.Forms.Padding(4);
            this.comboJ2534Baudrate.Name = "comboJ2534Baudrate";
            this.comboJ2534Baudrate.Size = new System.Drawing.Size(153, 24);
            this.comboJ2534Baudrate.TabIndex = 48;
            // 
            // comboJ2534Protocol
            // 
            this.comboJ2534Protocol.FormattingEnabled = true;
            this.comboJ2534Protocol.Location = new System.Drawing.Point(111, 58);
            this.comboJ2534Protocol.Margin = new System.Windows.Forms.Padding(4);
            this.comboJ2534Protocol.Name = "comboJ2534Protocol";
            this.comboJ2534Protocol.Size = new System.Drawing.Size(153, 24);
            this.comboJ2534Protocol.TabIndex = 45;
            this.comboJ2534Protocol.SelectedIndexChanged += new System.EventHandler(this.comboJ2534Protocol_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 88);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 16);
            this.label12.TabIndex = 47;
            this.label12.Text = "Baudrate:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(15, 146);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(148, 42);
            this.btnConnect.TabIndex = 21;
            this.btnConnect.Text = "Connect/ Disconnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // chkConsoleAutorefresh
            // 
            this.chkConsoleAutorefresh.AutoSize = true;
            this.chkConsoleAutorefresh.Location = new System.Drawing.Point(8, 101);
            this.chkConsoleAutorefresh.Margin = new System.Windows.Forms.Padding(4);
            this.chkConsoleAutorefresh.Name = "chkConsoleAutorefresh";
            this.chkConsoleAutorefresh.Size = new System.Drawing.Size(98, 20);
            this.chkConsoleAutorefresh.TabIndex = 44;
            this.chkConsoleAutorefresh.Text = "Auto refresh";
            this.chkConsoleAutorefresh.UseVisualStyleBackColor = true;
            this.chkConsoleAutorefresh.CheckedChanged += new System.EventHandler(this.chkConsoleAutorefresh_CheckedChanged);
            // 
            // btnConsoleRefresh
            // 
            this.btnConsoleRefresh.Location = new System.Drawing.Point(269, 396);
            this.btnConsoleRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnConsoleRefresh.Name = "btnConsoleRefresh";
            this.btnConsoleRefresh.Size = new System.Drawing.Size(180, 38);
            this.btnConsoleRefresh.TabIndex = 43;
            this.btnConsoleRefresh.Text = "Refresh";
            this.btnConsoleRefresh.UseVisualStyleBackColor = true;
            this.btnConsoleRefresh.Click += new System.EventHandler(this.btnConsoleRefresh_Click);
            // 
            // chkConsole4x
            // 
            this.chkConsole4x.AutoSize = true;
            this.chkConsole4x.Location = new System.Drawing.Point(8, 80);
            this.chkConsole4x.Margin = new System.Windows.Forms.Padding(4);
            this.chkConsole4x.Name = "chkConsole4x";
            this.chkConsole4x.Size = new System.Drawing.Size(78, 20);
            this.chkConsole4x.TabIndex = 42;
            this.chkConsole4x.Text = "4x mode";
            this.chkConsole4x.UseVisualStyleBackColor = true;
            this.chkConsole4x.CheckedChanged += new System.EventHandler(this.chkConsole4x_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 344);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 16);
            this.label9.TabIndex = 41;
            this.label9.Text = "Script delay (ms)";
            // 
            // numConsoleScriptDelay
            // 
            this.numConsoleScriptDelay.Location = new System.Drawing.Point(15, 364);
            this.numConsoleScriptDelay.Margin = new System.Windows.Forms.Padding(4);
            this.numConsoleScriptDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numConsoleScriptDelay.Name = "numConsoleScriptDelay";
            this.numConsoleScriptDelay.Size = new System.Drawing.Size(68, 22);
            this.numConsoleScriptDelay.TabIndex = 40;
            // 
            // chkConsoleUseJ2534Timestamps
            // 
            this.chkConsoleUseJ2534Timestamps.AutoSize = true;
            this.chkConsoleUseJ2534Timestamps.Location = new System.Drawing.Point(8, 62);
            this.chkConsoleUseJ2534Timestamps.Margin = new System.Windows.Forms.Padding(4);
            this.chkConsoleUseJ2534Timestamps.Name = "chkConsoleUseJ2534Timestamps";
            this.chkConsoleUseJ2534Timestamps.Size = new System.Drawing.Size(140, 20);
            this.chkConsoleUseJ2534Timestamps.TabIndex = 39;
            this.chkConsoleUseJ2534Timestamps.Text = "J2534 Timestamps";
            this.chkConsoleUseJ2534Timestamps.UseVisualStyleBackColor = true;
            // 
            // chkEnableConsole
            // 
            this.chkEnableConsole.AutoSize = true;
            this.chkEnableConsole.Location = new System.Drawing.Point(8, 23);
            this.chkEnableConsole.Margin = new System.Windows.Forms.Padding(4);
            this.chkEnableConsole.Name = "chkEnableConsole";
            this.chkEnableConsole.Size = new System.Drawing.Size(155, 20);
            this.chkEnableConsole.TabIndex = 37;
            this.chkEnableConsole.Text = "Enable VPW console";
            this.chkEnableConsole.UseVisualStyleBackColor = true;
            this.chkEnableConsole.CheckedChanged += new System.EventHandler(this.chkEnableConsole_CheckedChanged);
            // 
            // chkConsoleTimestamps
            // 
            this.chkConsoleTimestamps.AutoSize = true;
            this.chkConsoleTimestamps.Location = new System.Drawing.Point(8, 43);
            this.chkConsoleTimestamps.Margin = new System.Windows.Forms.Padding(4);
            this.chkConsoleTimestamps.Name = "chkConsoleTimestamps";
            this.chkConsoleTimestamps.Size = new System.Drawing.Size(102, 20);
            this.chkConsoleTimestamps.TabIndex = 38;
            this.chkConsoleTimestamps.Text = "Timestamps";
            this.chkConsoleTimestamps.UseVisualStyleBackColor = true;
            // 
            // btnConsoleLoadScript
            // 
            this.btnConsoleLoadScript.Location = new System.Drawing.Point(15, 394);
            this.btnConsoleLoadScript.Margin = new System.Windows.Forms.Padding(4);
            this.btnConsoleLoadScript.Name = "btnConsoleLoadScript";
            this.btnConsoleLoadScript.Size = new System.Drawing.Size(148, 38);
            this.btnConsoleLoadScript.TabIndex = 0;
            this.btnConsoleLoadScript.Text = "Upload script";
            this.btnConsoleLoadScript.UseVisualStyleBackColor = true;
            this.btnConsoleLoadScript.Click += new System.EventHandler(this.btnConsoleLoadScript_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.txtVPWmessages);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtSendBus);
            this.splitContainer3.Size = new System.Drawing.Size(720, 491);
            this.splitContainer3.SplitterDistance = 457;
            this.splitContainer3.TabIndex = 3;
            // 
            // txtVPWmessages
            // 
            this.txtVPWmessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVPWmessages.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVPWmessages.HideSelection = false;
            this.txtVPWmessages.Location = new System.Drawing.Point(0, 0);
            this.txtVPWmessages.Margin = new System.Windows.Forms.Padding(4);
            this.txtVPWmessages.Name = "txtVPWmessages";
            this.txtVPWmessages.Size = new System.Drawing.Size(720, 457);
            this.txtVPWmessages.TabIndex = 2;
            this.txtVPWmessages.Text = "";
            this.txtVPWmessages.WordWrap = false;
            // 
            // txtSendBus
            // 
            this.txtSendBus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtSendBus.Location = new System.Drawing.Point(0, 8);
            this.txtSendBus.Margin = new System.Windows.Forms.Padding(4);
            this.txtSendBus.Name = "txtSendBus";
            this.txtSendBus.Size = new System.Drawing.Size(720, 22);
            this.txtSendBus.TabIndex = 1;
            this.txtSendBus.TextChanged += new System.EventHandler(this.txtSendBus_TextChanged);
            // 
            // labelProgress
            // 
            this.labelProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelProgress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.Location = new System.Drawing.Point(623, 32);
            this.labelProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(583, 70);
            this.labelProgress.TabIndex = 22;
            this.labelProgress.Text = "idle";
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(5, 32);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(592, 39);
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
            this.txtResult.Margin = new System.Windows.Forms.Padding(4);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(1216, 132);
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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1216, 24);
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.creditsToolStripMenuItem.Text = "Credits";
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // homepageToolStripMenuItem
            // 
            this.homepageToolStripMenuItem.Name = "homepageToolStripMenuItem";
            this.homepageToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.homepageToolStripMenuItem.Text = "Homepage";
            this.homepageToolStripMenuItem.Click += new System.EventHandler(this.homepageToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 79);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
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
            this.splitContainer2.Size = new System.Drawing.Size(1216, 657);
            this.splitContainer2.SplitterDistance = 520;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 5;
            // 
            // labelConnected
            // 
            this.labelConnected.AutoSize = true;
            this.labelConnected.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelConnected.Location = new System.Drawing.Point(261, 7);
            this.labelConnected.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelConnected.Name = "labelConnected";
            this.labelConnected.Size = new System.Drawing.Size(93, 18);
            this.labelConnected.TabIndex = 24;
            this.labelConnected.Text = "Disconnected";
            // 
            // timerSearchParams
            // 
            this.timerSearchParams.Tick += new System.EventHandler(this.timerSearchParams_Tick);
            // 
            // frmLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 736);
            this.Controls.Add(this.labelConnected);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnStartStop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
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
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupJ2534Options.ResumeLayout(false);
            this.groupJ2534Options.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJ2534PeriodicMsgInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numConsoleScriptDelay)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
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
        private System.Windows.Forms.RichTextBox txtVPWmessages;
        private System.Windows.Forms.CheckBox chkEnableConsole;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creditsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem homepageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnSaveAnalyzerMsgs;
        private System.Windows.Forms.CheckBox chkConsoleTimestamps;
        private System.Windows.Forms.TabPage tabVPWConsole;
        private System.Windows.Forms.SplitContainer splitContainer4;
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
        private System.Windows.Forms.SplitContainer splitContainer3;
    }
}

