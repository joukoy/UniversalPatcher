namespace UniversalPatcher
{
    partial class FrmPatcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPatcher));
            this.btnOrgFile = new System.Windows.Forms.Button();
            this.btnModFile = new System.Windows.Forms.Button();
            this.txtBaseFile = new System.Windows.Forms.TextBox();
            this.txtModifierFile = new System.Windows.Forms.TextBox();
            this.btnCompare = new System.Windows.Forms.Button();
            this.btnSaveBin = new System.Windows.Forms.Button();
            this.txtPatchDescription = new System.Windows.Forms.TextBox();
            this.labelBinSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDescr = new System.Windows.Forms.Label();
            this.labelXML = new System.Windows.Forms.Label();
            this.numSuppress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkCompareAll = new System.Windows.Forms.CheckBox();
            this.chkAutodetect = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.btnSearch = new System.Windows.Forms.Button();
            this.chkLogtodisplay = new System.Windows.Forms.CheckBox();
            this.chkLogtoFile = new System.Windows.Forms.CheckBox();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnSaveFileInfo = new System.Windows.Forms.Button();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.btnSaveDebug = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtDebug = new System.Windows.Forms.RichTextBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.tabPatch = new System.Windows.Forms.TabPage();
            this.btnSaveAllPatches = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnDelPatch = new System.Windows.Forms.Button();
            this.btnAddPatch = new System.Windows.Forms.Button();
            this.listPatches = new System.Windows.Forms.ListBox();
            this.dataPatch = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnLoadPatch = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnManualPatch = new System.Windows.Forms.Button();
            this.labelPatchname = new System.Windows.Forms.Label();
            this.tabCVN = new System.Windows.Forms.TabPage();
            this.checkAutorefreshCVNlist = new System.Windows.Forms.CheckBox();
            this.btnRefreshCvnList = new System.Windows.Forms.Button();
            this.btnClearCVN = new System.Windows.Forms.Button();
            this.btnAddtoStock = new System.Windows.Forms.Button();
            this.dataCVN = new System.Windows.Forms.DataGridView();
            this.tabBadCvn = new System.Windows.Forms.TabPage();
            this.BtnRefreshBadCvn = new System.Windows.Forms.Button();
            this.btnClearBadCvn = new System.Windows.Forms.Button();
            this.dataGridBadCvn = new System.Windows.Forms.DataGridView();
            this.tabFinfo = new System.Windows.Forms.TabPage();
            this.btnSaveDecCsv = new System.Windows.Forms.Button();
            this.checkAutorefreshFileinfo = new System.Windows.Forms.CheckBox();
            this.btnRefreshFileinfo = new System.Windows.Forms.Button();
            this.btnSaveCSV = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.dataFileInfo = new System.Windows.Forms.DataGridView();
            this.tabCsAddress = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.btnClearCSAddresses = new System.Windows.Forms.Button();
            this.btnSaveCSaddresses = new System.Windows.Forms.Button();
            this.listCSAddresses = new System.Windows.Forms.ListView();
            this.tabBadChkFile = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.chkAutoRefreshBadChkFile = new System.Windows.Forms.CheckBox();
            this.btnRefreshBadChkFile = new System.Windows.Forms.Button();
            this.btnSaveCsvBadChkFile = new System.Windows.Forms.Button();
            this.btnClearBadchkFile = new System.Windows.Forms.Button();
            this.dataBadChkFile = new System.Windows.Forms.DataGridView();
            this.tabSearchedTables = new System.Windows.Forms.TabPage();
            this.btnShowTableData = new System.Windows.Forms.Button();
            this.chkTableSearchNoFilters = new System.Windows.Forms.CheckBox();
            this.btnClearSearchedTables = new System.Windows.Forms.Button();
            this.btnSaveSearchedTables = new System.Windows.Forms.Button();
            this.dataGridSearchedTables = new System.Windows.Forms.DataGridView();
            this.tabPIDList = new System.Windows.Forms.TabPage();
            this.btnClearPidList = new System.Windows.Forms.Button();
            this.btnSavePidList = new System.Windows.Forms.Button();
            this.dataGridPIDlist = new System.Windows.Forms.DataGridView();
            this.tabDTC = new System.Windows.Forms.TabPage();
            this.btnSetDTC = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.btnClearDTC = new System.Windows.Forms.Button();
            this.btnSaveCsvDTC = new System.Windows.Forms.Button();
            this.dataGridDTC = new System.Windows.Forms.DataGridView();
            this.tabTableSeek = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSearchTableSeek = new System.Windows.Forms.TextBox();
            this.comboTableCategory = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnReadTinyTunerDB = new System.Windows.Forms.Button();
            this.btnEditTable = new System.Windows.Forms.Button();
            this.dataGridTableSeek = new System.Windows.Forms.DataGridView();
            this.btnClearTableSeek = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.btnCrossTableSearch = new System.Windows.Forms.Button();
            this.chkExtra = new System.Windows.Forms.CheckBox();
            this.chkCS2 = new System.Windows.Forms.CheckBox();
            this.chkCS1 = new System.Windows.Forms.CheckBox();
            this.chkSize = new System.Windows.Forms.CheckBox();
            this.chkRange = new System.Windows.Forms.CheckBox();
            this.btnLoadFolder = new System.Windows.Forms.Button();
            this.tabFunction = new System.Windows.Forms.TabControl();
            this.tabApply = new System.Windows.Forms.TabPage();
            this.btnTuner = new System.Windows.Forms.Button();
            this.btnFixFilesChecksum = new System.Windows.Forms.Button();
            this.btnSwapSegments = new System.Windows.Forms.Button();
            this.btnBinLoadPatch = new System.Windows.Forms.Button();
            this.btnCheckSums = new System.Windows.Forms.Button();
            this.btnApplypatch = new System.Windows.Forms.Button();
            this.tabFileinfo = new System.Windows.Forms.TabPage();
            this.groupSearch = new System.Windows.Forms.GroupBox();
            this.txtCustomSearchString = new System.Windows.Forms.TextBox();
            this.txtCustomSearchStartAddress = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chkCustomTableSearch = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnCustomFindAll = new System.Windows.Forms.Button();
            this.btnCustomSearch = new System.Windows.Forms.Button();
            this.btnCustomSearchNext = new System.Windows.Forms.Button();
            this.chkSearchPids = new System.Windows.Forms.CheckBox();
            this.chkTableSeek = new System.Windows.Forms.CheckBox();
            this.chkSearchDTC = new System.Windows.Forms.CheckBox();
            this.chkSearchTables = new System.Windows.Forms.CheckBox();
            this.tabCreate = new System.Windows.Forms.TabPage();
            this.chkForceCompare = new System.Windows.Forms.CheckBox();
            this.numCrossVariation = new System.Windows.Forms.NumericUpDown();
            this.checkAppendPatch = new System.Windows.Forms.CheckBox();
            this.txtOS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabExtract = new System.Windows.Forms.TabPage();
            this.txtExtractDescription = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.txtCompatibleOS = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExtractRange = new System.Windows.Forms.TextBox();
            this.tabExtractSegments = new System.Windows.Forms.TabPage();
            this.checkExtractShowinfo = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioRename = new System.Windows.Forms.RadioButton();
            this.radioReplace = new System.Windows.Forms.RadioButton();
            this.radioSkip = new System.Windows.Forms.RadioButton();
            this.btnExtractSegmentsFolder = new System.Windows.Forms.Button();
            this.txtSegmentDescription = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnExtractSegments = new System.Windows.Forms.Button();
            this.tabChecksumUtil = new System.Windows.Forms.TabPage();
            this.chkCsUtilSwapBytes = new System.Windows.Forms.CheckBox();
            this.btnCsUtilFix = new System.Windows.Forms.Button();
            this.numCSBytes = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtCSAddr = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtChecksumRange = new System.Windows.Forms.TextBox();
            this.btnTestChecksum = new System.Windows.Forms.Button();
            this.chkCSUtilTryAll = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioCSUtilComplement2 = new System.Windows.Forms.RadioButton();
            this.radioCSUtilComplement1 = new System.Windows.Forms.RadioButton();
            this.radioCSUtilComplement0 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioCSUtilDwordSum = new System.Windows.Forms.RadioButton();
            this.radioCSUtilWordSum = new System.Windows.Forms.RadioButton();
            this.radioCSUtilSUM = new System.Windows.Forms.RadioButton();
            this.radioCSUtilCrc32 = new System.Windows.Forms.RadioButton();
            this.radioCSUtilCrc16 = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stockCVNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTableSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dTCSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pIDSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.segmentSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oBD2CodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rememberWindowSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableTunerAutloadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moreSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.tabPatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPatch)).BeginInit();
            this.tabCVN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataCVN)).BeginInit();
            this.tabBadCvn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBadCvn)).BeginInit();
            this.tabFinfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataFileInfo)).BeginInit();
            this.tabCsAddress.SuspendLayout();
            this.tabBadChkFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataBadChkFile)).BeginInit();
            this.tabSearchedTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSearchedTables)).BeginInit();
            this.tabPIDList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPIDlist)).BeginInit();
            this.tabDTC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDTC)).BeginInit();
            this.tabTableSeek.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTableSeek)).BeginInit();
            this.tabFunction.SuspendLayout();
            this.tabApply.SuspendLayout();
            this.tabFileinfo.SuspendLayout();
            this.groupSearch.SuspendLayout();
            this.tabCreate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCrossVariation)).BeginInit();
            this.tabExtract.SuspendLayout();
            this.tabExtractSegments.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabChecksumUtil.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCSBytes)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOrgFile
            // 
            this.btnOrgFile.Location = new System.Drawing.Point(12, 31);
            this.btnOrgFile.Name = "btnOrgFile";
            this.btnOrgFile.Size = new System.Drawing.Size(78, 25);
            this.btnOrgFile.TabIndex = 14;
            this.btnOrgFile.Text = "Original file";
            this.btnOrgFile.UseVisualStyleBackColor = true;
            this.btnOrgFile.Click += new System.EventHandler(this.btnOrgFile_Click);
            // 
            // btnModFile
            // 
            this.btnModFile.Location = new System.Drawing.Point(6, 3);
            this.btnModFile.Name = "btnModFile";
            this.btnModFile.Size = new System.Drawing.Size(78, 25);
            this.btnModFile.TabIndex = 110;
            this.btnModFile.Text = "Modified file";
            this.btnModFile.UseVisualStyleBackColor = true;
            this.btnModFile.Click += new System.EventHandler(this.btnModFile_Click);
            // 
            // txtBaseFile
            // 
            this.txtBaseFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBaseFile.Location = new System.Drawing.Point(96, 34);
            this.txtBaseFile.Name = "txtBaseFile";
            this.txtBaseFile.Size = new System.Drawing.Size(744, 20);
            this.txtBaseFile.TabIndex = 15;
            // 
            // txtModifierFile
            // 
            this.txtModifierFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModifierFile.Location = new System.Drawing.Point(90, 6);
            this.txtModifierFile.Name = "txtModifierFile";
            this.txtModifierFile.Size = new System.Drawing.Size(742, 20);
            this.txtModifierFile.TabIndex = 111;
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(6, 29);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(78, 25);
            this.btnCompare.TabIndex = 112;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // btnSaveBin
            // 
            this.btnSaveBin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveBin.Location = new System.Drawing.Point(725, 6);
            this.btnSaveBin.Name = "btnSaveBin";
            this.btnSaveBin.Size = new System.Drawing.Size(108, 25);
            this.btnSaveBin.TabIndex = 188;
            this.btnSaveBin.Text = "Save bin";
            this.btnSaveBin.UseVisualStyleBackColor = true;
            this.btnSaveBin.Click += new System.EventHandler(this.btnSaveBin_Click);
            // 
            // txtPatchDescription
            // 
            this.txtPatchDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatchDescription.Location = new System.Drawing.Point(129, 57);
            this.txtPatchDescription.Name = "txtPatchDescription";
            this.txtPatchDescription.Size = new System.Drawing.Size(471, 20);
            this.txtPatchDescription.TabIndex = 116;
            // 
            // labelBinSize
            // 
            this.labelBinSize.AutoSize = true;
            this.labelBinSize.Location = new System.Drawing.Point(65, 8);
            this.labelBinSize.Name = "labelBinSize";
            this.labelBinSize.Size = new System.Drawing.Size(10, 13);
            this.labelBinSize.TabIndex = 9;
            this.labelBinSize.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "BIN Size:";
            // 
            // labelDescr
            // 
            this.labelDescr.AutoSize = true;
            this.labelDescr.Location = new System.Drawing.Point(24, 62);
            this.labelDescr.Name = "labelDescr";
            this.labelDescr.Size = new System.Drawing.Size(92, 13);
            this.labelDescr.TabIndex = 11;
            this.labelDescr.Text = "Patch description:";
            // 
            // labelXML
            // 
            this.labelXML.AutoSize = true;
            this.labelXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXML.Location = new System.Drawing.Point(275, 9);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(13, 16);
            this.labelXML.TabIndex = 16;
            this.labelXML.Text = "-";
            // 
            // numSuppress
            // 
            this.numSuppress.Location = new System.Drawing.Point(406, 3);
            this.numSuppress.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSuppress.Name = "numSuppress";
            this.numSuppress.Size = new System.Drawing.Size(42, 20);
            this.numSuppress.TabIndex = 201;
            this.numSuppress.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(341, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Show max:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(449, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "patch rows";
            // 
            // chkCompareAll
            // 
            this.chkCompareAll.AutoSize = true;
            this.chkCompareAll.Location = new System.Drawing.Point(241, 35);
            this.chkCompareAll.Name = "chkCompareAll";
            this.chkCompareAll.Size = new System.Drawing.Size(167, 17);
            this.chkCompareAll.TabIndex = 114;
            this.chkCompareAll.Text = "Compare all (ignore segments)";
            this.chkCompareAll.UseVisualStyleBackColor = true;
            // 
            // chkAutodetect
            // 
            this.chkAutodetect.AutoSize = true;
            this.chkAutodetect.Checked = true;
            this.chkAutodetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutodetect.Location = new System.Drawing.Point(150, 7);
            this.chkAutodetect.Name = "chkAutodetect";
            this.chkAutodetect.Size = new System.Drawing.Size(110, 17);
            this.chkAutodetect.TabIndex = 11;
            this.chkAutodetect.Text = "Autodetect config";
            this.chkAutodetect.UseVisualStyleBackColor = true;
            this.chkAutodetect.CheckedChanged += new System.EventHandler(this.chkAutodetect_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabInfo);
            this.tabControl1.Controls.Add(this.tabDebug);
            this.tabControl1.Controls.Add(this.tabPatch);
            this.tabControl1.Controls.Add(this.tabCVN);
            this.tabControl1.Controls.Add(this.tabBadCvn);
            this.tabControl1.Controls.Add(this.tabFinfo);
            this.tabControl1.Controls.Add(this.tabCsAddress);
            this.tabControl1.Controls.Add(this.tabBadChkFile);
            this.tabControl1.Controls.Add(this.tabSearchedTables);
            this.tabControl1.Controls.Add(this.tabPIDList);
            this.tabControl1.Controls.Add(this.tabDTC);
            this.tabControl1.Controls.Add(this.tabTableSeek);
            this.tabControl1.Location = new System.Drawing.Point(0, 191);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(843, 370);
            this.tabControl1.TabIndex = 200;
            // 
            // tabInfo
            // 
            this.tabInfo.AutoScroll = true;
            this.tabInfo.Controls.Add(this.btnSearch);
            this.tabInfo.Controls.Add(this.chkLogtodisplay);
            this.tabInfo.Controls.Add(this.chkLogtoFile);
            this.tabInfo.Controls.Add(this.txtResult);
            this.tabInfo.Controls.Add(this.numSuppress);
            this.tabInfo.Controls.Add(this.label2);
            this.tabInfo.Controls.Add(this.btnSaveFileInfo);
            this.tabInfo.Controls.Add(this.label3);
            this.tabInfo.Controls.Add(this.label1);
            this.tabInfo.Controls.Add(this.labelBinSize);
            this.tabInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tabInfo.Location = new System.Drawing.Point(4, 22);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabInfo.Size = new System.Drawing.Size(835, 344);
            this.tabInfo.TabIndex = 0;
            this.tabInfo.Text = "Log";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(549, 1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 205;
            this.btnSearch.Text = "Search...";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // chkLogtodisplay
            // 
            this.chkLogtodisplay.AutoSize = true;
            this.chkLogtodisplay.Checked = true;
            this.chkLogtodisplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLogtodisplay.Location = new System.Drawing.Point(146, 6);
            this.chkLogtodisplay.Name = "chkLogtodisplay";
            this.chkLogtodisplay.Size = new System.Drawing.Size(91, 17);
            this.chkLogtodisplay.TabIndex = 204;
            this.chkLogtodisplay.Text = "Log to display";
            this.chkLogtodisplay.UseVisualStyleBackColor = true;
            // 
            // chkLogtoFile
            // 
            this.chkLogtoFile.AutoSize = true;
            this.chkLogtoFile.Location = new System.Drawing.Point(259, 5);
            this.chkLogtoFile.Name = "chkLogtoFile";
            this.chkLogtoFile.Size = new System.Drawing.Size(72, 17);
            this.chkLogtoFile.TabIndex = 203;
            this.chkLogtoFile.Text = "Log to file";
            this.chkLogtoFile.UseVisualStyleBackColor = true;
            this.chkLogtoFile.CheckedChanged += new System.EventHandler(this.chkLogtoFile_CheckedChanged);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(2, 26);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(833, 317);
            this.txtResult.TabIndex = 202;
            this.txtResult.Text = "";
            // 
            // btnSaveFileInfo
            // 
            this.btnSaveFileInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveFileInfo.Location = new System.Drawing.Point(757, 2);
            this.btnSaveFileInfo.Name = "btnSaveFileInfo";
            this.btnSaveFileInfo.Size = new System.Drawing.Size(78, 23);
            this.btnSaveFileInfo.TabIndex = 171;
            this.btnSaveFileInfo.Text = "Save log...";
            this.btnSaveFileInfo.UseVisualStyleBackColor = true;
            this.btnSaveFileInfo.Click += new System.EventHandler(this.btnSaveFileInfo_Click);
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.btnSaveDebug);
            this.tabDebug.Controls.Add(this.button2);
            this.tabDebug.Controls.Add(this.txtDebug);
            this.tabDebug.Controls.Add(this.chkDebug);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(835, 344);
            this.tabDebug.TabIndex = 1;
            this.tabDebug.Text = "Debug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // btnSaveDebug
            // 
            this.btnSaveDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDebug.Location = new System.Drawing.Point(760, -1);
            this.btnSaveDebug.Name = "btnSaveDebug";
            this.btnSaveDebug.Size = new System.Drawing.Size(75, 23);
            this.btnSaveDebug.TabIndex = 214;
            this.btnSaveDebug.Text = "Save log...";
            this.btnSaveDebug.UseVisualStyleBackColor = true;
            this.btnSaveDebug.Click += new System.EventHandler(this.btnSaveDebug_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(92, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 213;
            this.button2.Text = "Search...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtDebug
            // 
            this.txtDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDebug.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDebug.Location = new System.Drawing.Point(2, 24);
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(833, 323);
            this.txtDebug.TabIndex = 212;
            this.txtDebug.Text = "";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(5, 3);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(73, 17);
            this.chkDebug.TabIndex = 211;
            this.chkDebug.Text = "Debug on";
            this.chkDebug.UseVisualStyleBackColor = true;
            this.chkDebug.CheckedChanged += new System.EventHandler(this.chkDebug_CheckedChanged);
            // 
            // tabPatch
            // 
            this.tabPatch.Controls.Add(this.btnSaveAllPatches);
            this.tabPatch.Controls.Add(this.splitContainer1);
            this.tabPatch.Controls.Add(this.btnRefresh);
            this.tabPatch.Controls.Add(this.btnNew);
            this.tabPatch.Controls.Add(this.btnLoadPatch);
            this.tabPatch.Controls.Add(this.btnSave);
            this.tabPatch.Controls.Add(this.btnHelp);
            this.tabPatch.Controls.Add(this.btnEdit);
            this.tabPatch.Controls.Add(this.btnDown);
            this.tabPatch.Controls.Add(this.btnUp);
            this.tabPatch.Controls.Add(this.btnDelete);
            this.tabPatch.Controls.Add(this.btnManualPatch);
            this.tabPatch.Controls.Add(this.labelPatchname);
            this.tabPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPatch.Location = new System.Drawing.Point(4, 22);
            this.tabPatch.Name = "tabPatch";
            this.tabPatch.Size = new System.Drawing.Size(835, 344);
            this.tabPatch.TabIndex = 2;
            this.tabPatch.Text = "Patch editor";
            this.tabPatch.UseVisualStyleBackColor = true;
            // 
            // btnSaveAllPatches
            // 
            this.btnSaveAllPatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveAllPatches.Location = new System.Drawing.Point(768, 1);
            this.btnSaveAllPatches.Name = "btnSaveAllPatches";
            this.btnSaveAllPatches.Size = new System.Drawing.Size(67, 23);
            this.btnSaveAllPatches.TabIndex = 262;
            this.btnSaveAllPatches.Text = "Save All";
            this.btnSaveAllPatches.UseVisualStyleBackColor = true;
            this.btnSaveAllPatches.Click += new System.EventHandler(this.btnSaveAllPatches_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnDelPatch);
            this.splitContainer1.Panel1.Controls.Add(this.btnAddPatch);
            this.splitContainer1.Panel1.Controls.Add(this.listPatches);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataPatch);
            this.splitContainer1.Size = new System.Drawing.Size(780, 323);
            this.splitContainer1.SplitterDistance = 210;
            this.splitContainer1.TabIndex = 261;
            // 
            // btnDelPatch
            // 
            this.btnDelPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelPatch.Location = new System.Drawing.Point(53, 293);
            this.btnDelPatch.Name = "btnDelPatch";
            this.btnDelPatch.Size = new System.Drawing.Size(46, 20);
            this.btnDelPatch.TabIndex = 262;
            this.btnDelPatch.Text = "Del";
            this.btnDelPatch.UseVisualStyleBackColor = true;
            this.btnDelPatch.Click += new System.EventHandler(this.btnDelPatch_Click);
            // 
            // btnAddPatch
            // 
            this.btnAddPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddPatch.Location = new System.Drawing.Point(5, 293);
            this.btnAddPatch.Name = "btnAddPatch";
            this.btnAddPatch.Size = new System.Drawing.Size(42, 20);
            this.btnAddPatch.TabIndex = 261;
            this.btnAddPatch.Text = "Add";
            this.btnAddPatch.UseVisualStyleBackColor = true;
            this.btnAddPatch.Click += new System.EventHandler(this.btnAddPatch_Click);
            // 
            // listPatches
            // 
            this.listPatches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPatches.FormattingEnabled = true;
            this.listPatches.Location = new System.Drawing.Point(0, 0);
            this.listPatches.Name = "listPatches";
            this.listPatches.Size = new System.Drawing.Size(210, 290);
            this.listPatches.TabIndex = 260;
            this.listPatches.SelectedIndexChanged += new System.EventHandler(this.listPatches_SelectedIndexChanged);
            // 
            // dataPatch
            // 
            this.dataPatch.AllowUserToOrderColumns = true;
            this.dataPatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPatch.Location = new System.Drawing.Point(0, 0);
            this.dataPatch.Name = "dataPatch";
            this.dataPatch.Size = new System.Drawing.Size(566, 323);
            this.dataPatch.TabIndex = 0;
            this.dataPatch.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataPatch_CellContentDoubleClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(540, 1);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(59, 23);
            this.btnRefresh.TabIndex = 250;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click_1);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.Location = new System.Drawing.Point(605, 1);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(49, 23);
            this.btnNew.TabIndex = 251;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnLoadPatch
            // 
            this.btnLoadPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadPatch.Location = new System.Drawing.Point(658, 1);
            this.btnLoadPatch.Name = "btnLoadPatch";
            this.btnLoadPatch.Size = new System.Drawing.Size(49, 23);
            this.btnLoadPatch.TabIndex = 252;
            this.btnLoadPatch.Text = "Load";
            this.btnLoadPatch.UseVisualStyleBackColor = true;
            this.btnLoadPatch.Click += new System.EventHandler(this.btnLoadPatch_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(713, 1);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(49, 23);
            this.btnSave.TabIndex = 253;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.Location = new System.Drawing.Point(789, 140);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(43, 22);
            this.btnHelp.TabIndex = 257;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(789, 58);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(43, 22);
            this.btnEdit.TabIndex = 254;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Location = new System.Drawing.Point(790, 221);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(43, 22);
            this.btnDown.TabIndex = 259;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Location = new System.Drawing.Point(789, 193);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(43, 22);
            this.btnUp.TabIndex = 258;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(789, 112);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(43, 22);
            this.btnDelete.TabIndex = 256;
            this.btnDelete.Text = "Del";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnManualPatch
            // 
            this.btnManualPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManualPatch.Location = new System.Drawing.Point(789, 86);
            this.btnManualPatch.Name = "btnManualPatch";
            this.btnManualPatch.Size = new System.Drawing.Size(43, 22);
            this.btnManualPatch.TabIndex = 255;
            this.btnManualPatch.Text = "Add";
            this.btnManualPatch.UseVisualStyleBackColor = true;
            this.btnManualPatch.Click += new System.EventHandler(this.btnManualPatch_Click);
            // 
            // labelPatchname
            // 
            this.labelPatchname.AutoSize = true;
            this.labelPatchname.Location = new System.Drawing.Point(8, 9);
            this.labelPatchname.Name = "labelPatchname";
            this.labelPatchname.Size = new System.Drawing.Size(10, 13);
            this.labelPatchname.TabIndex = 131;
            this.labelPatchname.Text = "-";
            // 
            // tabCVN
            // 
            this.tabCVN.Controls.Add(this.checkAutorefreshCVNlist);
            this.tabCVN.Controls.Add(this.btnRefreshCvnList);
            this.tabCVN.Controls.Add(this.btnClearCVN);
            this.tabCVN.Controls.Add(this.btnAddtoStock);
            this.tabCVN.Controls.Add(this.dataCVN);
            this.tabCVN.Location = new System.Drawing.Point(4, 22);
            this.tabCVN.Name = "tabCVN";
            this.tabCVN.Size = new System.Drawing.Size(835, 344);
            this.tabCVN.TabIndex = 3;
            this.tabCVN.Text = "CVN";
            this.tabCVN.UseVisualStyleBackColor = true;
            // 
            // checkAutorefreshCVNlist
            // 
            this.checkAutorefreshCVNlist.AutoSize = true;
            this.checkAutorefreshCVNlist.Checked = true;
            this.checkAutorefreshCVNlist.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutorefreshCVNlist.Location = new System.Drawing.Point(151, 10);
            this.checkAutorefreshCVNlist.Name = "checkAutorefreshCVNlist";
            this.checkAutorefreshCVNlist.Size = new System.Drawing.Size(83, 17);
            this.checkAutorefreshCVNlist.TabIndex = 180;
            this.checkAutorefreshCVNlist.Text = "Auto refresh";
            this.checkAutorefreshCVNlist.UseVisualStyleBackColor = true;
            this.checkAutorefreshCVNlist.CheckedChanged += new System.EventHandler(this.checkAutorefreshCVNlist_CheckedChanged);
            // 
            // btnRefreshCvnList
            // 
            this.btnRefreshCvnList.Location = new System.Drawing.Point(73, 6);
            this.btnRefreshCvnList.Name = "btnRefreshCvnList";
            this.btnRefreshCvnList.Size = new System.Drawing.Size(62, 21);
            this.btnRefreshCvnList.TabIndex = 179;
            this.btnRefreshCvnList.Text = "Refresh";
            this.btnRefreshCvnList.UseVisualStyleBackColor = true;
            this.btnRefreshCvnList.Click += new System.EventHandler(this.btnRefreshCvnList_Click);
            // 
            // btnClearCVN
            // 
            this.btnClearCVN.Location = new System.Drawing.Point(5, 5);
            this.btnClearCVN.Name = "btnClearCVN";
            this.btnClearCVN.Size = new System.Drawing.Size(62, 22);
            this.btnClearCVN.TabIndex = 178;
            this.btnClearCVN.Text = "Clear";
            this.btnClearCVN.UseVisualStyleBackColor = true;
            this.btnClearCVN.Click += new System.EventHandler(this.btnClearCVN_Click);
            // 
            // btnAddtoStock
            // 
            this.btnAddtoStock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddtoStock.Location = new System.Drawing.Point(696, 3);
            this.btnAddtoStock.Name = "btnAddtoStock";
            this.btnAddtoStock.Size = new System.Drawing.Size(136, 22);
            this.btnAddtoStock.TabIndex = 177;
            this.btnAddtoStock.Text = "Add to: \"stockcvn.xml\"";
            this.btnAddtoStock.UseVisualStyleBackColor = true;
            this.btnAddtoStock.Click += new System.EventHandler(this.buttonAddtoStock_Click);
            // 
            // dataCVN
            // 
            this.dataCVN.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataCVN.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataCVN.Location = new System.Drawing.Point(1, 33);
            this.dataCVN.Name = "dataCVN";
            this.dataCVN.Size = new System.Drawing.Size(831, 310);
            this.dataCVN.TabIndex = 0;
            // 
            // tabBadCvn
            // 
            this.tabBadCvn.Controls.Add(this.BtnRefreshBadCvn);
            this.tabBadCvn.Controls.Add(this.btnClearBadCvn);
            this.tabBadCvn.Controls.Add(this.dataGridBadCvn);
            this.tabBadCvn.Location = new System.Drawing.Point(4, 22);
            this.tabBadCvn.Name = "tabBadCvn";
            this.tabBadCvn.Size = new System.Drawing.Size(835, 344);
            this.tabBadCvn.TabIndex = 9;
            this.tabBadCvn.Text = "Mismatch CVN";
            this.tabBadCvn.UseVisualStyleBackColor = true;
            // 
            // BtnRefreshBadCvn
            // 
            this.BtnRefreshBadCvn.Location = new System.Drawing.Point(71, 7);
            this.BtnRefreshBadCvn.Name = "BtnRefreshBadCvn";
            this.BtnRefreshBadCvn.Size = new System.Drawing.Size(62, 21);
            this.BtnRefreshBadCvn.TabIndex = 182;
            this.BtnRefreshBadCvn.Text = "Refresh";
            this.BtnRefreshBadCvn.UseVisualStyleBackColor = true;
            this.BtnRefreshBadCvn.Click += new System.EventHandler(this.BtnRefreshBadCvn_Click);
            // 
            // btnClearBadCvn
            // 
            this.btnClearBadCvn.Location = new System.Drawing.Point(3, 6);
            this.btnClearBadCvn.Name = "btnClearBadCvn";
            this.btnClearBadCvn.Size = new System.Drawing.Size(62, 22);
            this.btnClearBadCvn.TabIndex = 181;
            this.btnClearBadCvn.Text = "Clear";
            this.btnClearBadCvn.UseVisualStyleBackColor = true;
            this.btnClearBadCvn.Click += new System.EventHandler(this.btnClearBadCvn_Click);
            // 
            // dataGridBadCvn
            // 
            this.dataGridBadCvn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridBadCvn.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridBadCvn.Location = new System.Drawing.Point(0, 34);
            this.dataGridBadCvn.Name = "dataGridBadCvn";
            this.dataGridBadCvn.Size = new System.Drawing.Size(839, 310);
            this.dataGridBadCvn.TabIndex = 1;
            // 
            // tabFinfo
            // 
            this.tabFinfo.Controls.Add(this.btnSaveDecCsv);
            this.tabFinfo.Controls.Add(this.checkAutorefreshFileinfo);
            this.tabFinfo.Controls.Add(this.btnRefreshFileinfo);
            this.tabFinfo.Controls.Add(this.btnSaveCSV);
            this.tabFinfo.Controls.Add(this.btnClear);
            this.tabFinfo.Controls.Add(this.dataFileInfo);
            this.tabFinfo.Location = new System.Drawing.Point(4, 22);
            this.tabFinfo.Name = "tabFinfo";
            this.tabFinfo.Size = new System.Drawing.Size(835, 344);
            this.tabFinfo.TabIndex = 4;
            this.tabFinfo.Text = "File info";
            this.tabFinfo.UseVisualStyleBackColor = true;
            // 
            // btnSaveDecCsv
            // 
            this.btnSaveDecCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveDecCsv.Location = new System.Drawing.Point(638, 3);
            this.btnSaveDecCsv.Name = "btnSaveDecCsv";
            this.btnSaveDecCsv.Size = new System.Drawing.Size(116, 23);
            this.btnSaveDecCsv.TabIndex = 7;
            this.btnSaveDecCsv.Text = "Save CSV(dec)";
            this.btnSaveDecCsv.UseVisualStyleBackColor = true;
            this.btnSaveDecCsv.Click += new System.EventHandler(this.btnSaveDecCsv_Click);
            // 
            // checkAutorefreshFileinfo
            // 
            this.checkAutorefreshFileinfo.AutoSize = true;
            this.checkAutorefreshFileinfo.Checked = true;
            this.checkAutorefreshFileinfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutorefreshFileinfo.Location = new System.Drawing.Point(152, 8);
            this.checkAutorefreshFileinfo.Name = "checkAutorefreshFileinfo";
            this.checkAutorefreshFileinfo.Size = new System.Drawing.Size(83, 17);
            this.checkAutorefreshFileinfo.TabIndex = 6;
            this.checkAutorefreshFileinfo.Text = "Auto refresh";
            this.checkAutorefreshFileinfo.UseVisualStyleBackColor = true;
            this.checkAutorefreshFileinfo.CheckedChanged += new System.EventHandler(this.checkAutorefreshFileinfo_CheckedChanged);
            // 
            // btnRefreshFileinfo
            // 
            this.btnRefreshFileinfo.Location = new System.Drawing.Point(74, 4);
            this.btnRefreshFileinfo.Name = "btnRefreshFileinfo";
            this.btnRefreshFileinfo.Size = new System.Drawing.Size(62, 21);
            this.btnRefreshFileinfo.TabIndex = 5;
            this.btnRefreshFileinfo.Text = "Refresh";
            this.btnRefreshFileinfo.UseVisualStyleBackColor = true;
            this.btnRefreshFileinfo.Click += new System.EventHandler(this.btnRefreshFileinfo_Click);
            // 
            // btnSaveCSV
            // 
            this.btnSaveCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCSV.Location = new System.Drawing.Point(760, 3);
            this.btnSaveCSV.Name = "btnSaveCSV";
            this.btnSaveCSV.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCSV.TabIndex = 4;
            this.btnSaveCSV.Text = "Save CSV";
            this.btnSaveCSV.UseVisualStyleBackColor = true;
            this.btnSaveCSV.Click += new System.EventHandler(this.btnSaveCSV_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(6, 4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(62, 21);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dataFileInfo
            // 
            this.dataFileInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataFileInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataFileInfo.Location = new System.Drawing.Point(1, 31);
            this.dataFileInfo.Name = "dataFileInfo";
            this.dataFileInfo.Size = new System.Drawing.Size(834, 312);
            this.dataFileInfo.TabIndex = 0;
            this.dataFileInfo.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataFileInfo_CellContentDoubleClick);
            // 
            // tabCsAddress
            // 
            this.tabCsAddress.Controls.Add(this.label9);
            this.tabCsAddress.Controls.Add(this.btnClearCSAddresses);
            this.tabCsAddress.Controls.Add(this.btnSaveCSaddresses);
            this.tabCsAddress.Controls.Add(this.listCSAddresses);
            this.tabCsAddress.Location = new System.Drawing.Point(4, 22);
            this.tabCsAddress.Name = "tabCsAddress";
            this.tabCsAddress.Size = new System.Drawing.Size(835, 344);
            this.tabCsAddress.TabIndex = 5;
            this.tabCsAddress.Text = "Gm-v6 info";
            this.tabCsAddress.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(96, 11);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(255, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "GM V6 Checksum address: (ignore for other binaries)";
            // 
            // btnClearCSAddresses
            // 
            this.btnClearCSAddresses.Location = new System.Drawing.Point(7, 5);
            this.btnClearCSAddresses.Name = "btnClearCSAddresses";
            this.btnClearCSAddresses.Size = new System.Drawing.Size(67, 22);
            this.btnClearCSAddresses.TabIndex = 2;
            this.btnClearCSAddresses.Text = "Clear";
            this.btnClearCSAddresses.UseVisualStyleBackColor = true;
            this.btnClearCSAddresses.Click += new System.EventHandler(this.btnClearCSAddresses_Click);
            // 
            // btnSaveCSaddresses
            // 
            this.btnSaveCSaddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCSaddresses.Location = new System.Drawing.Point(760, 4);
            this.btnSaveCSaddresses.Name = "btnSaveCSaddresses";
            this.btnSaveCSaddresses.Size = new System.Drawing.Size(75, 25);
            this.btnSaveCSaddresses.TabIndex = 1;
            this.btnSaveCSaddresses.Text = "Save list...";
            this.btnSaveCSaddresses.UseVisualStyleBackColor = true;
            this.btnSaveCSaddresses.Click += new System.EventHandler(this.btnSaveCSaddresses_Click);
            // 
            // listCSAddresses
            // 
            this.listCSAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listCSAddresses.HideSelection = false;
            this.listCSAddresses.Location = new System.Drawing.Point(2, 31);
            this.listCSAddresses.Name = "listCSAddresses";
            this.listCSAddresses.Size = new System.Drawing.Size(834, 316);
            this.listCSAddresses.TabIndex = 0;
            this.listCSAddresses.UseCompatibleStateImageBehavior = false;
            // 
            // tabBadChkFile
            // 
            this.tabBadChkFile.Controls.Add(this.label10);
            this.tabBadChkFile.Controls.Add(this.chkAutoRefreshBadChkFile);
            this.tabBadChkFile.Controls.Add(this.btnRefreshBadChkFile);
            this.tabBadChkFile.Controls.Add(this.btnSaveCsvBadChkFile);
            this.tabBadChkFile.Controls.Add(this.btnClearBadchkFile);
            this.tabBadChkFile.Controls.Add(this.dataBadChkFile);
            this.tabBadChkFile.Location = new System.Drawing.Point(4, 22);
            this.tabBadChkFile.Name = "tabBadChkFile";
            this.tabBadChkFile.Size = new System.Drawing.Size(835, 344);
            this.tabBadChkFile.TabIndex = 6;
            this.tabBadChkFile.Text = "bad chk file";
            this.tabBadChkFile.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(257, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(185, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Segments of files with bad checksum:";
            // 
            // chkAutoRefreshBadChkFile
            // 
            this.chkAutoRefreshBadChkFile.AutoSize = true;
            this.chkAutoRefreshBadChkFile.Checked = true;
            this.chkAutoRefreshBadChkFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoRefreshBadChkFile.Location = new System.Drawing.Point(153, 6);
            this.chkAutoRefreshBadChkFile.Name = "chkAutoRefreshBadChkFile";
            this.chkAutoRefreshBadChkFile.Size = new System.Drawing.Size(83, 17);
            this.chkAutoRefreshBadChkFile.TabIndex = 10;
            this.chkAutoRefreshBadChkFile.Text = "Auto refresh";
            this.chkAutoRefreshBadChkFile.UseVisualStyleBackColor = true;
            // 
            // btnRefreshBadChkFile
            // 
            this.btnRefreshBadChkFile.Location = new System.Drawing.Point(72, 3);
            this.btnRefreshBadChkFile.Name = "btnRefreshBadChkFile";
            this.btnRefreshBadChkFile.Size = new System.Drawing.Size(62, 21);
            this.btnRefreshBadChkFile.TabIndex = 9;
            this.btnRefreshBadChkFile.Text = "Refresh";
            this.btnRefreshBadChkFile.UseVisualStyleBackColor = true;
            this.btnRefreshBadChkFile.Click += new System.EventHandler(this.btnRefreshBadChkFile_Click);
            // 
            // btnSaveCsvBadChkFile
            // 
            this.btnSaveCsvBadChkFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCsvBadChkFile.Location = new System.Drawing.Point(757, 4);
            this.btnSaveCsvBadChkFile.Name = "btnSaveCsvBadChkFile";
            this.btnSaveCsvBadChkFile.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCsvBadChkFile.TabIndex = 8;
            this.btnSaveCsvBadChkFile.Text = "Save CSV";
            this.btnSaveCsvBadChkFile.UseVisualStyleBackColor = true;
            this.btnSaveCsvBadChkFile.Click += new System.EventHandler(this.btnSaveCsvBadChkFile_Click);
            // 
            // btnClearBadchkFile
            // 
            this.btnClearBadchkFile.Location = new System.Drawing.Point(4, 3);
            this.btnClearBadchkFile.Name = "btnClearBadchkFile";
            this.btnClearBadchkFile.Size = new System.Drawing.Size(62, 21);
            this.btnClearBadchkFile.TabIndex = 7;
            this.btnClearBadchkFile.Text = "Clear";
            this.btnClearBadchkFile.UseVisualStyleBackColor = true;
            this.btnClearBadchkFile.Click += new System.EventHandler(this.btnClearBadchkFile_Click);
            // 
            // dataBadChkFile
            // 
            this.dataBadChkFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataBadChkFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataBadChkFile.Location = new System.Drawing.Point(-1, 30);
            this.dataBadChkFile.Name = "dataBadChkFile";
            this.dataBadChkFile.Size = new System.Drawing.Size(837, 312);
            this.dataBadChkFile.TabIndex = 6;
            this.dataBadChkFile.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataBadChkFile_CellContentDoubleClick);
            // 
            // tabSearchedTables
            // 
            this.tabSearchedTables.Controls.Add(this.btnShowTableData);
            this.tabSearchedTables.Controls.Add(this.chkTableSearchNoFilters);
            this.tabSearchedTables.Controls.Add(this.btnClearSearchedTables);
            this.tabSearchedTables.Controls.Add(this.btnSaveSearchedTables);
            this.tabSearchedTables.Controls.Add(this.dataGridSearchedTables);
            this.tabSearchedTables.Location = new System.Drawing.Point(4, 22);
            this.tabSearchedTables.Name = "tabSearchedTables";
            this.tabSearchedTables.Size = new System.Drawing.Size(835, 344);
            this.tabSearchedTables.TabIndex = 7;
            this.tabSearchedTables.Text = "Searched Tables";
            this.tabSearchedTables.UseVisualStyleBackColor = true;
            // 
            // btnShowTableData
            // 
            this.btnShowTableData.Location = new System.Drawing.Point(166, 4);
            this.btnShowTableData.Name = "btnShowTableData";
            this.btnShowTableData.Size = new System.Drawing.Size(75, 23);
            this.btnShowTableData.TabIndex = 4;
            this.btnShowTableData.Text = "Show data";
            this.btnShowTableData.UseVisualStyleBackColor = true;
            this.btnShowTableData.Click += new System.EventHandler(this.btnShowTableData_Click);
            // 
            // chkTableSearchNoFilters
            // 
            this.chkTableSearchNoFilters.AutoSize = true;
            this.chkTableSearchNoFilters.Location = new System.Drawing.Point(259, 7);
            this.chkTableSearchNoFilters.Name = "chkTableSearchNoFilters";
            this.chkTableSearchNoFilters.Size = new System.Drawing.Size(122, 17);
            this.chkTableSearchNoFilters.TabIndex = 3;
            this.chkTableSearchNoFilters.Text = "No filters/hit counter";
            this.chkTableSearchNoFilters.UseVisualStyleBackColor = true;
            this.chkTableSearchNoFilters.CheckedChanged += new System.EventHandler(this.chkTableSearchNoFilters_CheckedChanged);
            // 
            // btnClearSearchedTables
            // 
            this.btnClearSearchedTables.Location = new System.Drawing.Point(3, 3);
            this.btnClearSearchedTables.Name = "btnClearSearchedTables";
            this.btnClearSearchedTables.Size = new System.Drawing.Size(75, 23);
            this.btnClearSearchedTables.TabIndex = 2;
            this.btnClearSearchedTables.Text = "Clear";
            this.btnClearSearchedTables.UseVisualStyleBackColor = true;
            this.btnClearSearchedTables.Click += new System.EventHandler(this.btnClearSearchedTables_Click);
            // 
            // btnSaveSearchedTables
            // 
            this.btnSaveSearchedTables.Location = new System.Drawing.Point(84, 3);
            this.btnSaveSearchedTables.Name = "btnSaveSearchedTables";
            this.btnSaveSearchedTables.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSearchedTables.TabIndex = 1;
            this.btnSaveSearchedTables.Text = "Save CSV";
            this.btnSaveSearchedTables.UseVisualStyleBackColor = true;
            this.btnSaveSearchedTables.Click += new System.EventHandler(this.btnSaveSearchedTables_Click);
            // 
            // dataGridSearchedTables
            // 
            this.dataGridSearchedTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridSearchedTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSearchedTables.Location = new System.Drawing.Point(1, 35);
            this.dataGridSearchedTables.Name = "dataGridSearchedTables";
            this.dataGridSearchedTables.Size = new System.Drawing.Size(835, 308);
            this.dataGridSearchedTables.TabIndex = 0;
            this.dataGridSearchedTables.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridSearchedTables_CellContentDoubleClick);
            // 
            // tabPIDList
            // 
            this.tabPIDList.Controls.Add(this.btnClearPidList);
            this.tabPIDList.Controls.Add(this.btnSavePidList);
            this.tabPIDList.Controls.Add(this.dataGridPIDlist);
            this.tabPIDList.Location = new System.Drawing.Point(4, 22);
            this.tabPIDList.Name = "tabPIDList";
            this.tabPIDList.Size = new System.Drawing.Size(835, 344);
            this.tabPIDList.TabIndex = 8;
            this.tabPIDList.Text = "PIDs";
            this.tabPIDList.UseVisualStyleBackColor = true;
            // 
            // btnClearPidList
            // 
            this.btnClearPidList.Location = new System.Drawing.Point(8, 4);
            this.btnClearPidList.Name = "btnClearPidList";
            this.btnClearPidList.Size = new System.Drawing.Size(75, 23);
            this.btnClearPidList.TabIndex = 4;
            this.btnClearPidList.Text = "Clear";
            this.btnClearPidList.UseVisualStyleBackColor = true;
            this.btnClearPidList.Click += new System.EventHandler(this.btnClearPidList_Click);
            // 
            // btnSavePidList
            // 
            this.btnSavePidList.Location = new System.Drawing.Point(89, 4);
            this.btnSavePidList.Name = "btnSavePidList";
            this.btnSavePidList.Size = new System.Drawing.Size(75, 23);
            this.btnSavePidList.TabIndex = 3;
            this.btnSavePidList.Text = "Save CSV";
            this.btnSavePidList.UseVisualStyleBackColor = true;
            this.btnSavePidList.Click += new System.EventHandler(this.btnSavePidList_Click);
            // 
            // dataGridPIDlist
            // 
            this.dataGridPIDlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridPIDlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPIDlist.Location = new System.Drawing.Point(1, 33);
            this.dataGridPIDlist.Name = "dataGridPIDlist";
            this.dataGridPIDlist.Size = new System.Drawing.Size(834, 310);
            this.dataGridPIDlist.TabIndex = 0;
            // 
            // tabDTC
            // 
            this.tabDTC.Controls.Add(this.btnSetDTC);
            this.tabDTC.Controls.Add(this.label14);
            this.tabDTC.Controls.Add(this.btnClearDTC);
            this.tabDTC.Controls.Add(this.btnSaveCsvDTC);
            this.tabDTC.Controls.Add(this.dataGridDTC);
            this.tabDTC.Location = new System.Drawing.Point(4, 22);
            this.tabDTC.Name = "tabDTC";
            this.tabDTC.Size = new System.Drawing.Size(835, 344);
            this.tabDTC.TabIndex = 10;
            this.tabDTC.Text = "DTC";
            this.tabDTC.UseVisualStyleBackColor = true;
            // 
            // btnSetDTC
            // 
            this.btnSetDTC.Location = new System.Drawing.Point(170, 3);
            this.btnSetDTC.Name = "btnSetDTC";
            this.btnSetDTC.Size = new System.Drawing.Size(75, 23);
            this.btnSetDTC.TabIndex = 10;
            this.btnSetDTC.Text = "Set DTC";
            this.btnSetDTC.UseVisualStyleBackColor = true;
            this.btnSetDTC.Click += new System.EventHandler(this.btnSetDTC_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(337, 8);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(81, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "GM DTC codes";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // btnClearDTC
            // 
            this.btnClearDTC.Location = new System.Drawing.Point(8, 3);
            this.btnClearDTC.Name = "btnClearDTC";
            this.btnClearDTC.Size = new System.Drawing.Size(75, 23);
            this.btnClearDTC.TabIndex = 7;
            this.btnClearDTC.Text = "Clear";
            this.btnClearDTC.UseVisualStyleBackColor = true;
            this.btnClearDTC.Click += new System.EventHandler(this.btnClearDTC_Click);
            // 
            // btnSaveCsvDTC
            // 
            this.btnSaveCsvDTC.Location = new System.Drawing.Point(89, 3);
            this.btnSaveCsvDTC.Name = "btnSaveCsvDTC";
            this.btnSaveCsvDTC.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCsvDTC.TabIndex = 6;
            this.btnSaveCsvDTC.Text = "Save CSV";
            this.btnSaveCsvDTC.UseVisualStyleBackColor = true;
            this.btnSaveCsvDTC.Click += new System.EventHandler(this.btnSaveCsvDTC_Click);
            // 
            // dataGridDTC
            // 
            this.dataGridDTC.AllowUserToAddRows = false;
            this.dataGridDTC.AllowUserToDeleteRows = false;
            this.dataGridDTC.AllowUserToOrderColumns = true;
            this.dataGridDTC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridDTC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridDTC.Location = new System.Drawing.Point(1, 32);
            this.dataGridDTC.MultiSelect = false;
            this.dataGridDTC.Name = "dataGridDTC";
            this.dataGridDTC.Size = new System.Drawing.Size(831, 310);
            this.dataGridDTC.TabIndex = 5;
            this.dataGridDTC.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridDTC_CellContentDoubleClick);
            // 
            // tabTableSeek
            // 
            this.tabTableSeek.Controls.Add(this.label16);
            this.tabTableSeek.Controls.Add(this.txtSearchTableSeek);
            this.tabTableSeek.Controls.Add(this.comboTableCategory);
            this.tabTableSeek.Controls.Add(this.label15);
            this.tabTableSeek.Controls.Add(this.btnReadTinyTunerDB);
            this.tabTableSeek.Controls.Add(this.btnEditTable);
            this.tabTableSeek.Controls.Add(this.dataGridTableSeek);
            this.tabTableSeek.Controls.Add(this.btnClearTableSeek);
            this.tabTableSeek.Location = new System.Drawing.Point(4, 22);
            this.tabTableSeek.Name = "tabTableSeek";
            this.tabTableSeek.Size = new System.Drawing.Size(835, 344);
            this.tabTableSeek.TabIndex = 11;
            this.tabTableSeek.Text = "Table Seek";
            this.tabTableSeek.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(3, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Filter:";
            // 
            // txtSearchTableSeek
            // 
            this.txtSearchTableSeek.Location = new System.Drawing.Point(41, 6);
            this.txtSearchTableSeek.Name = "txtSearchTableSeek";
            this.txtSearchTableSeek.Size = new System.Drawing.Size(105, 20);
            this.txtSearchTableSeek.TabIndex = 6;
            this.txtSearchTableSeek.TextChanged += new System.EventHandler(this.txtSearchTableSeek_TextChanged);
            // 
            // comboTableCategory
            // 
            this.comboTableCategory.FormattingEnabled = true;
            this.comboTableCategory.Location = new System.Drawing.Point(203, 6);
            this.comboTableCategory.Name = "comboTableCategory";
            this.comboTableCategory.Size = new System.Drawing.Size(142, 21);
            this.comboTableCategory.TabIndex = 5;
            this.comboTableCategory.SelectedIndexChanged += new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(152, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "Category:";
            // 
            // btnReadTinyTunerDB
            // 
            this.btnReadTinyTunerDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadTinyTunerDB.Enabled = false;
            this.btnReadTinyTunerDB.Location = new System.Drawing.Point(793, 3);
            this.btnReadTinyTunerDB.Name = "btnReadTinyTunerDB";
            this.btnReadTinyTunerDB.Size = new System.Drawing.Size(165, 23);
            this.btnReadTinyTunerDB.TabIndex = 3;
            this.btnReadTinyTunerDB.Text = "Read TinyTuner DB (V6 only)";
            this.btnReadTinyTunerDB.UseVisualStyleBackColor = true;
            this.btnReadTinyTunerDB.Visible = false;
            this.btnReadTinyTunerDB.Click += new System.EventHandler(this.btnReadTinyTunerDB_Click);
            // 
            // btnEditTable
            // 
            this.btnEditTable.Location = new System.Drawing.Point(566, 5);
            this.btnEditTable.Name = "btnEditTable";
            this.btnEditTable.Size = new System.Drawing.Size(75, 23);
            this.btnEditTable.TabIndex = 2;
            this.btnEditTable.Text = "Edit table";
            this.btnEditTable.UseVisualStyleBackColor = true;
            this.btnEditTable.Click += new System.EventHandler(this.btnEditTable_Click);
            // 
            // dataGridTableSeek
            // 
            this.dataGridTableSeek.AllowUserToAddRows = false;
            this.dataGridTableSeek.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridTableSeek.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridTableSeek.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridTableSeek.Location = new System.Drawing.Point(-2, 32);
            this.dataGridTableSeek.Name = "dataGridTableSeek";
            this.dataGridTableSeek.Size = new System.Drawing.Size(834, 312);
            this.dataGridTableSeek.TabIndex = 1;
            this.dataGridTableSeek.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridTableSeek_CellContentDoubleClick);
            // 
            // btnClearTableSeek
            // 
            this.btnClearTableSeek.Location = new System.Drawing.Point(485, 5);
            this.btnClearTableSeek.Name = "btnClearTableSeek";
            this.btnClearTableSeek.Size = new System.Drawing.Size(75, 23);
            this.btnClearTableSeek.TabIndex = 0;
            this.btnClearTableSeek.Text = "Clear";
            this.btnClearTableSeek.UseVisualStyleBackColor = true;
            this.btnClearTableSeek.Click += new System.EventHandler(this.btnClearTableSeek_Click);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(714, 36);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 120;
            this.label13.Text = "Max variation:";
            this.label13.Visible = false;
            // 
            // btnCrossTableSearch
            // 
            this.btnCrossTableSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCrossTableSearch.Location = new System.Drawing.Point(717, 57);
            this.btnCrossTableSearch.Name = "btnCrossTableSearch";
            this.btnCrossTableSearch.Size = new System.Drawing.Size(116, 23);
            this.btnCrossTableSearch.TabIndex = 118;
            this.btnCrossTableSearch.Text = "Cross table search";
            this.btnCrossTableSearch.UseVisualStyleBackColor = true;
            this.btnCrossTableSearch.Visible = false;
            this.btnCrossTableSearch.Click += new System.EventHandler(this.btnCrossTableSearch_Click);
            // 
            // chkExtra
            // 
            this.chkExtra.AutoSize = true;
            this.chkExtra.Checked = true;
            this.chkExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExtra.Location = new System.Drawing.Point(328, 10);
            this.chkExtra.Name = "chkExtra";
            this.chkExtra.Size = new System.Drawing.Size(70, 17);
            this.chkExtra.TabIndex = 176;
            this.chkExtra.Text = "Extra info";
            this.chkExtra.UseVisualStyleBackColor = true;
            // 
            // chkCS2
            // 
            this.chkCS2.AutoSize = true;
            this.chkCS2.Checked = true;
            this.chkCS2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCS2.Location = new System.Drawing.Point(240, 10);
            this.chkCS2.Name = "chkCS2";
            this.chkCS2.Size = new System.Drawing.Size(85, 17);
            this.chkCS2.TabIndex = 175;
            this.chkCS2.Text = "Checksum 2";
            this.chkCS2.UseVisualStyleBackColor = true;
            // 
            // chkCS1
            // 
            this.chkCS1.AutoSize = true;
            this.chkCS1.Checked = true;
            this.chkCS1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCS1.Location = new System.Drawing.Point(149, 10);
            this.chkCS1.Name = "chkCS1";
            this.chkCS1.Size = new System.Drawing.Size(85, 17);
            this.chkCS1.TabIndex = 174;
            this.chkCS1.Text = "Checksum 1";
            this.chkCS1.UseVisualStyleBackColor = true;
            // 
            // chkSize
            // 
            this.chkSize.AutoSize = true;
            this.chkSize.Checked = true;
            this.chkSize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSize.Location = new System.Drawing.Point(507, 10);
            this.chkSize.Name = "chkSize";
            this.chkSize.Size = new System.Drawing.Size(91, 17);
            this.chkSize.TabIndex = 173;
            this.chkSize.Text = "Segment Size";
            this.chkSize.UseVisualStyleBackColor = true;
            // 
            // chkRange
            // 
            this.chkRange.AutoSize = true;
            this.chkRange.Checked = true;
            this.chkRange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRange.Location = new System.Drawing.Point(404, 10);
            this.chkRange.Name = "chkRange";
            this.chkRange.Size = new System.Drawing.Size(98, 17);
            this.chkRange.TabIndex = 172;
            this.chkRange.Text = "Segment range";
            this.chkRange.UseVisualStyleBackColor = true;
            // 
            // btnLoadFolder
            // 
            this.btnLoadFolder.Location = new System.Drawing.Point(6, 3);
            this.btnLoadFolder.Name = "btnLoadFolder";
            this.btnLoadFolder.Size = new System.Drawing.Size(124, 29);
            this.btnLoadFolder.TabIndex = 170;
            this.btnLoadFolder.Text = "Show info for files...";
            this.btnLoadFolder.UseVisualStyleBackColor = true;
            this.btnLoadFolder.Click += new System.EventHandler(this.btnLoadFolder_Click);
            // 
            // tabFunction
            // 
            this.tabFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabFunction.Controls.Add(this.tabApply);
            this.tabFunction.Controls.Add(this.tabFileinfo);
            this.tabFunction.Controls.Add(this.tabCreate);
            this.tabFunction.Controls.Add(this.tabExtract);
            this.tabFunction.Controls.Add(this.tabExtractSegments);
            this.tabFunction.Controls.Add(this.tabChecksumUtil);
            this.tabFunction.Location = new System.Drawing.Point(2, 60);
            this.tabFunction.Name = "tabFunction";
            this.tabFunction.SelectedIndex = 0;
            this.tabFunction.Size = new System.Drawing.Size(846, 129);
            this.tabFunction.TabIndex = 100;
            // 
            // tabApply
            // 
            this.tabApply.Controls.Add(this.btnTuner);
            this.tabApply.Controls.Add(this.btnFixFilesChecksum);
            this.tabApply.Controls.Add(this.btnSwapSegments);
            this.tabApply.Controls.Add(this.btnBinLoadPatch);
            this.tabApply.Controls.Add(this.btnCheckSums);
            this.tabApply.Controls.Add(this.btnApplypatch);
            this.tabApply.Controls.Add(this.btnSaveBin);
            this.tabApply.Location = new System.Drawing.Point(4, 22);
            this.tabApply.Name = "tabApply";
            this.tabApply.Padding = new System.Windows.Forms.Padding(3);
            this.tabApply.Size = new System.Drawing.Size(838, 103);
            this.tabApply.TabIndex = 1;
            this.tabApply.Text = "Modify bin";
            this.tabApply.UseVisualStyleBackColor = true;
            // 
            // btnTuner
            // 
            this.btnTuner.Location = new System.Drawing.Point(349, 6);
            this.btnTuner.Name = "btnTuner";
            this.btnTuner.Size = new System.Drawing.Size(107, 25);
            this.btnTuner.TabIndex = 189;
            this.btnTuner.Text = "Tuner";
            this.btnTuner.UseVisualStyleBackColor = true;
            this.btnTuner.Click += new System.EventHandler(this.btnTuner_Click);
            // 
            // btnFixFilesChecksum
            // 
            this.btnFixFilesChecksum.Location = new System.Drawing.Point(181, 37);
            this.btnFixFilesChecksum.Name = "btnFixFilesChecksum";
            this.btnFixFilesChecksum.Size = new System.Drawing.Size(162, 25);
            this.btnFixFilesChecksum.TabIndex = 186;
            this.btnFixFilesChecksum.Text = "Fix checksum of files...";
            this.btnFixFilesChecksum.UseVisualStyleBackColor = true;
            this.btnFixFilesChecksum.Click += new System.EventHandler(this.btnFixFilesChecksum_Click);
            // 
            // btnSwapSegments
            // 
            this.btnSwapSegments.Location = new System.Drawing.Point(236, 6);
            this.btnSwapSegments.Name = "btnSwapSegments";
            this.btnSwapSegments.Size = new System.Drawing.Size(107, 25);
            this.btnSwapSegments.TabIndex = 183;
            this.btnSwapSegments.Text = "Swap segment(s)";
            this.btnSwapSegments.UseVisualStyleBackColor = true;
            this.btnSwapSegments.Click += new System.EventHandler(this.btnSwapSegments_Click);
            // 
            // btnBinLoadPatch
            // 
            this.btnBinLoadPatch.Location = new System.Drawing.Point(9, 6);
            this.btnBinLoadPatch.Name = "btnBinLoadPatch";
            this.btnBinLoadPatch.Size = new System.Drawing.Size(107, 25);
            this.btnBinLoadPatch.TabIndex = 181;
            this.btnBinLoadPatch.Text = "Load patch";
            this.btnBinLoadPatch.UseVisualStyleBackColor = true;
            this.btnBinLoadPatch.Click += new System.EventHandler(this.btnBinLoadPatch_Click);
            // 
            // btnCheckSums
            // 
            this.btnCheckSums.Location = new System.Drawing.Point(10, 37);
            this.btnCheckSums.Name = "btnCheckSums";
            this.btnCheckSums.Size = new System.Drawing.Size(165, 25);
            this.btnCheckSums.TabIndex = 184;
            this.btnCheckSums.Text = "Fix checksums of current file";
            this.btnCheckSums.UseVisualStyleBackColor = true;
            this.btnCheckSums.Click += new System.EventHandler(this.btnCheckSums_Click);
            // 
            // btnApplypatch
            // 
            this.btnApplypatch.Location = new System.Drawing.Point(123, 6);
            this.btnApplypatch.Name = "btnApplypatch";
            this.btnApplypatch.Size = new System.Drawing.Size(107, 25);
            this.btnApplypatch.TabIndex = 182;
            this.btnApplypatch.Text = "Apply current patch";
            this.btnApplypatch.UseVisualStyleBackColor = true;
            this.btnApplypatch.Click += new System.EventHandler(this.btnApplypatch_Click);
            // 
            // tabFileinfo
            // 
            this.tabFileinfo.Controls.Add(this.groupSearch);
            this.tabFileinfo.Controls.Add(this.chkSearchPids);
            this.tabFileinfo.Controls.Add(this.chkTableSeek);
            this.tabFileinfo.Controls.Add(this.chkSearchDTC);
            this.tabFileinfo.Controls.Add(this.chkSearchTables);
            this.tabFileinfo.Controls.Add(this.chkExtra);
            this.tabFileinfo.Controls.Add(this.btnLoadFolder);
            this.tabFileinfo.Controls.Add(this.chkCS2);
            this.tabFileinfo.Controls.Add(this.chkCS1);
            this.tabFileinfo.Controls.Add(this.chkRange);
            this.tabFileinfo.Controls.Add(this.chkSize);
            this.tabFileinfo.Location = new System.Drawing.Point(4, 22);
            this.tabFileinfo.Name = "tabFileinfo";
            this.tabFileinfo.Size = new System.Drawing.Size(838, 103);
            this.tabFileinfo.TabIndex = 2;
            this.tabFileinfo.Text = "File info";
            this.tabFileinfo.UseVisualStyleBackColor = true;
            // 
            // groupSearch
            // 
            this.groupSearch.Controls.Add(this.txtCustomSearchString);
            this.groupSearch.Controls.Add(this.txtCustomSearchStartAddress);
            this.groupSearch.Controls.Add(this.label11);
            this.groupSearch.Controls.Add(this.chkCustomTableSearch);
            this.groupSearch.Controls.Add(this.label12);
            this.groupSearch.Controls.Add(this.btnCustomFindAll);
            this.groupSearch.Controls.Add(this.btnCustomSearch);
            this.groupSearch.Controls.Add(this.btnCustomSearchNext);
            this.groupSearch.Location = new System.Drawing.Point(342, 30);
            this.groupSearch.Name = "groupSearch";
            this.groupSearch.Size = new System.Drawing.Size(459, 70);
            this.groupSearch.TabIndex = 190;
            this.groupSearch.TabStop = false;
            // 
            // txtCustomSearchString
            // 
            this.txtCustomSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomSearchString.Location = new System.Drawing.Point(121, 13);
            this.txtCustomSearchString.Name = "txtCustomSearchString";
            this.txtCustomSearchString.Size = new System.Drawing.Size(229, 20);
            this.txtCustomSearchString.TabIndex = 179;
            // 
            // txtCustomSearchStartAddress
            // 
            this.txtCustomSearchStartAddress.Location = new System.Drawing.Point(186, 41);
            this.txtCustomSearchStartAddress.Name = "txtCustomSearchStartAddress";
            this.txtCustomSearchStartAddress.Size = new System.Drawing.Size(84, 20);
            this.txtCustomSearchStartAddress.TabIndex = 180;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 44);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(175, 13);
            this.label11.TabIndex = 181;
            this.label11.Text = "Start searching from address (HEX):";
            // 
            // chkCustomTableSearch
            // 
            this.chkCustomTableSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCustomTableSearch.AutoSize = true;
            this.chkCustomTableSearch.Location = new System.Drawing.Point(356, 16);
            this.chkCustomTableSearch.Name = "chkCustomTableSearch";
            this.chkCustomTableSearch.Size = new System.Drawing.Size(88, 17);
            this.chkCustomTableSearch.TabIndex = 186;
            this.chkCustomTableSearch.Text = "Table search";
            this.chkCustomTableSearch.UseVisualStyleBackColor = true;
            this.chkCustomTableSearch.CheckedChanged += new System.EventHandler(this.chkCustomTableSearch_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 13);
            this.label12.TabIndex = 182;
            this.label12.Text = "Custom search string:";
            // 
            // btnCustomFindAll
            // 
            this.btnCustomFindAll.Location = new System.Drawing.Point(402, 38);
            this.btnCustomFindAll.Name = "btnCustomFindAll";
            this.btnCustomFindAll.Size = new System.Drawing.Size(51, 23);
            this.btnCustomFindAll.TabIndex = 185;
            this.btnCustomFindAll.Text = "Find all";
            this.btnCustomFindAll.UseVisualStyleBackColor = true;
            this.btnCustomFindAll.Click += new System.EventHandler(this.btnCustomFindAll_Click);
            // 
            // btnCustomSearch
            // 
            this.btnCustomSearch.Location = new System.Drawing.Point(276, 38);
            this.btnCustomSearch.Name = "btnCustomSearch";
            this.btnCustomSearch.Size = new System.Drawing.Size(54, 23);
            this.btnCustomSearch.TabIndex = 183;
            this.btnCustomSearch.Text = "Search";
            this.btnCustomSearch.UseVisualStyleBackColor = true;
            this.btnCustomSearch.Click += new System.EventHandler(this.btnCustomSearch_Click);
            // 
            // btnCustomSearchNext
            // 
            this.btnCustomSearchNext.Location = new System.Drawing.Point(336, 38);
            this.btnCustomSearchNext.Name = "btnCustomSearchNext";
            this.btnCustomSearchNext.Size = new System.Drawing.Size(60, 23);
            this.btnCustomSearchNext.TabIndex = 184;
            this.btnCustomSearchNext.Text = "Find next";
            this.btnCustomSearchNext.UseVisualStyleBackColor = true;
            this.btnCustomSearchNext.Click += new System.EventHandler(this.btnCustomSearchNext_Click);
            // 
            // chkSearchPids
            // 
            this.chkSearchPids.AutoSize = true;
            this.chkSearchPids.Checked = true;
            this.chkSearchPids.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSearchPids.Location = new System.Drawing.Point(244, 77);
            this.chkSearchPids.Name = "chkSearchPids";
            this.chkSearchPids.Size = new System.Drawing.Size(86, 17);
            this.chkSearchPids.TabIndex = 189;
            this.chkSearchPids.Text = "Search PIDs";
            this.chkSearchPids.UseVisualStyleBackColor = true;
            // 
            // chkTableSeek
            // 
            this.chkTableSeek.AutoSize = true;
            this.chkTableSeek.Checked = true;
            this.chkTableSeek.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTableSeek.Location = new System.Drawing.Point(149, 76);
            this.chkTableSeek.Name = "chkTableSeek";
            this.chkTableSeek.Size = new System.Drawing.Size(81, 17);
            this.chkTableSeek.TabIndex = 188;
            this.chkTableSeek.Text = "Table Seek";
            this.chkTableSeek.UseVisualStyleBackColor = true;
            // 
            // chkSearchDTC
            // 
            this.chkSearchDTC.AutoSize = true;
            this.chkSearchDTC.Checked = true;
            this.chkSearchDTC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSearchDTC.Location = new System.Drawing.Point(244, 53);
            this.chkSearchDTC.Name = "chkSearchDTC";
            this.chkSearchDTC.Size = new System.Drawing.Size(85, 17);
            this.chkSearchDTC.TabIndex = 187;
            this.chkSearchDTC.Text = "Search DTC";
            this.chkSearchDTC.UseVisualStyleBackColor = true;
            // 
            // chkSearchTables
            // 
            this.chkSearchTables.AutoSize = true;
            this.chkSearchTables.Location = new System.Drawing.Point(149, 53);
            this.chkSearchTables.Name = "chkSearchTables";
            this.chkSearchTables.Size = new System.Drawing.Size(95, 17);
            this.chkSearchTables.TabIndex = 177;
            this.chkSearchTables.Text = "Search Tables";
            this.chkSearchTables.UseVisualStyleBackColor = true;
            // 
            // tabCreate
            // 
            this.tabCreate.Controls.Add(this.chkForceCompare);
            this.tabCreate.Controls.Add(this.label13);
            this.tabCreate.Controls.Add(this.numCrossVariation);
            this.tabCreate.Controls.Add(this.btnCrossTableSearch);
            this.tabCreate.Controls.Add(this.checkAppendPatch);
            this.tabCreate.Controls.Add(this.txtOS);
            this.tabCreate.Controls.Add(this.label7);
            this.tabCreate.Controls.Add(this.btnModFile);
            this.tabCreate.Controls.Add(this.txtModifierFile);
            this.tabCreate.Controls.Add(this.btnCompare);
            this.tabCreate.Controls.Add(this.chkCompareAll);
            this.tabCreate.Controls.Add(this.labelDescr);
            this.tabCreate.Controls.Add(this.txtPatchDescription);
            this.tabCreate.Location = new System.Drawing.Point(4, 22);
            this.tabCreate.Name = "tabCreate";
            this.tabCreate.Padding = new System.Windows.Forms.Padding(3);
            this.tabCreate.Size = new System.Drawing.Size(838, 103);
            this.tabCreate.TabIndex = 0;
            this.tabCreate.Text = "Create Patch";
            this.tabCreate.UseVisualStyleBackColor = true;
            // 
            // chkForceCompare
            // 
            this.chkForceCompare.AutoSize = true;
            this.chkForceCompare.Location = new System.Drawing.Point(414, 35);
            this.chkForceCompare.Name = "chkForceCompare";
            this.chkForceCompare.Size = new System.Drawing.Size(110, 17);
            this.chkForceCompare.TabIndex = 121;
            this.chkForceCompare.Text = "Force if same size";
            this.chkForceCompare.UseVisualStyleBackColor = true;
            this.chkForceCompare.CheckedChanged += new System.EventHandler(this.chkForceCompare_CheckedChanged);
            // 
            // numCrossVariation
            // 
            this.numCrossVariation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numCrossVariation.Location = new System.Drawing.Point(796, 32);
            this.numCrossVariation.Name = "numCrossVariation";
            this.numCrossVariation.Size = new System.Drawing.Size(36, 20);
            this.numCrossVariation.TabIndex = 119;
            this.numCrossVariation.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numCrossVariation.Visible = false;
            // 
            // checkAppendPatch
            // 
            this.checkAppendPatch.AutoSize = true;
            this.checkAppendPatch.Location = new System.Drawing.Point(537, 35);
            this.checkAppendPatch.Name = "checkAppendPatch";
            this.checkAppendPatch.Size = new System.Drawing.Size(63, 17);
            this.checkAppendPatch.TabIndex = 117;
            this.checkAppendPatch.Text = "Append";
            this.checkAppendPatch.UseVisualStyleBackColor = true;
            // 
            // txtOS
            // 
            this.txtOS.Location = new System.Drawing.Point(129, 32);
            this.txtOS.Name = "txtOS";
            this.txtOS.Size = new System.Drawing.Size(106, 20);
            this.txtOS.TabIndex = 113;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(91, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "OS:";
            // 
            // tabExtract
            // 
            this.tabExtract.Controls.Add(this.txtExtractDescription);
            this.tabExtract.Controls.Add(this.label6);
            this.tabExtract.Controls.Add(this.btnExtract);
            this.tabExtract.Controls.Add(this.txtCompatibleOS);
            this.tabExtract.Controls.Add(this.label5);
            this.tabExtract.Controls.Add(this.label4);
            this.tabExtract.Controls.Add(this.txtExtractRange);
            this.tabExtract.Location = new System.Drawing.Point(4, 22);
            this.tabExtract.Name = "tabExtract";
            this.tabExtract.Size = new System.Drawing.Size(838, 103);
            this.tabExtract.TabIndex = 3;
            this.tabExtract.Text = "Extract table";
            this.tabExtract.UseVisualStyleBackColor = true;
            // 
            // txtExtractDescription
            // 
            this.txtExtractDescription.Location = new System.Drawing.Point(124, 56);
            this.txtExtractDescription.Name = "txtExtractDescription";
            this.txtExtractDescription.Size = new System.Drawing.Size(226, 20);
            this.txtExtractDescription.TabIndex = 153;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Description:";
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Location = new System.Drawing.Point(764, 9);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(66, 23);
            this.btnExtract.TabIndex = 154;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // txtCompatibleOS
            // 
            this.txtCompatibleOS.Location = new System.Drawing.Point(124, 32);
            this.txtCompatibleOS.Name = "txtCompatibleOS";
            this.txtCompatibleOS.Size = new System.Drawing.Size(226, 20);
            this.txtCompatibleOS.TabIndex = 152;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Compatible OS list:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Address range (HEX):";
            // 
            // txtExtractRange
            // 
            this.txtExtractRange.Location = new System.Drawing.Point(124, 6);
            this.txtExtractRange.Name = "txtExtractRange";
            this.txtExtractRange.Size = new System.Drawing.Size(226, 20);
            this.txtExtractRange.TabIndex = 150;
            // 
            // tabExtractSegments
            // 
            this.tabExtractSegments.Controls.Add(this.checkExtractShowinfo);
            this.tabExtractSegments.Controls.Add(this.groupBox2);
            this.tabExtractSegments.Controls.Add(this.btnExtractSegmentsFolder);
            this.tabExtractSegments.Controls.Add(this.txtSegmentDescription);
            this.tabExtractSegments.Controls.Add(this.label8);
            this.tabExtractSegments.Controls.Add(this.btnExtractSegments);
            this.tabExtractSegments.Location = new System.Drawing.Point(4, 22);
            this.tabExtractSegments.Name = "tabExtractSegments";
            this.tabExtractSegments.Size = new System.Drawing.Size(838, 103);
            this.tabExtractSegments.TabIndex = 4;
            this.tabExtractSegments.Text = "Extract segments";
            this.tabExtractSegments.UseVisualStyleBackColor = true;
            // 
            // checkExtractShowinfo
            // 
            this.checkExtractShowinfo.AutoSize = true;
            this.checkExtractShowinfo.Checked = true;
            this.checkExtractShowinfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkExtractShowinfo.Location = new System.Drawing.Point(243, 21);
            this.checkExtractShowinfo.Name = "checkExtractShowinfo";
            this.checkExtractShowinfo.Size = new System.Drawing.Size(93, 17);
            this.checkExtractShowinfo.TabIndex = 506;
            this.checkExtractShowinfo.Text = "Display fileinfo";
            this.checkExtractShowinfo.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioRename);
            this.groupBox2.Controls.Add(this.radioReplace);
            this.groupBox2.Controls.Add(this.radioSkip);
            this.groupBox2.Location = new System.Drawing.Point(2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 42);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Duplicates";
            // 
            // radioRename
            // 
            this.radioRename.AutoSize = true;
            this.radioRename.Checked = true;
            this.radioRename.Location = new System.Drawing.Point(143, 19);
            this.radioRename.Name = "radioRename";
            this.radioRename.Size = new System.Drawing.Size(65, 17);
            this.radioRename.TabIndex = 504;
            this.radioRename.TabStop = true;
            this.radioRename.Text = "Rename";
            this.radioRename.UseVisualStyleBackColor = true;
            // 
            // radioReplace
            // 
            this.radioReplace.AutoSize = true;
            this.radioReplace.Location = new System.Drawing.Point(72, 19);
            this.radioReplace.Name = "radioReplace";
            this.radioReplace.Size = new System.Drawing.Size(65, 17);
            this.radioReplace.TabIndex = 503;
            this.radioReplace.Text = "Replace";
            this.radioReplace.UseVisualStyleBackColor = true;
            // 
            // radioSkip
            // 
            this.radioSkip.AutoSize = true;
            this.radioSkip.Location = new System.Drawing.Point(7, 19);
            this.radioSkip.Name = "radioSkip";
            this.radioSkip.Size = new System.Drawing.Size(46, 17);
            this.radioSkip.TabIndex = 502;
            this.radioSkip.Text = "Skip";
            this.radioSkip.UseVisualStyleBackColor = true;
            // 
            // btnExtractSegmentsFolder
            // 
            this.btnExtractSegmentsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractSegmentsFolder.Location = new System.Drawing.Point(654, 9);
            this.btnExtractSegmentsFolder.Name = "btnExtractSegmentsFolder";
            this.btnExtractSegmentsFolder.Size = new System.Drawing.Size(171, 26);
            this.btnExtractSegmentsFolder.TabIndex = 505;
            this.btnExtractSegmentsFolder.Text = "Extract all segments from files..";
            this.btnExtractSegmentsFolder.UseVisualStyleBackColor = true;
            this.btnExtractSegmentsFolder.Click += new System.EventHandler(this.btnExtractSegmentsFolder_Click);
            // 
            // txtSegmentDescription
            // 
            this.txtSegmentDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSegmentDescription.Location = new System.Drawing.Point(84, 45);
            this.txtSegmentDescription.Name = "txtSegmentDescription";
            this.txtSegmentDescription.Size = new System.Drawing.Size(564, 20);
            this.txtSegmentDescription.TabIndex = 500;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Description:";
            // 
            // btnExtractSegments
            // 
            this.btnExtractSegments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractSegments.Location = new System.Drawing.Point(654, 41);
            this.btnExtractSegments.Name = "btnExtractSegments";
            this.btnExtractSegments.Size = new System.Drawing.Size(171, 26);
            this.btnExtractSegments.TabIndex = 501;
            this.btnExtractSegments.Text = "Extract current file";
            this.btnExtractSegments.UseVisualStyleBackColor = true;
            this.btnExtractSegments.Click += new System.EventHandler(this.btnExtractSegments_Click);
            // 
            // tabChecksumUtil
            // 
            this.tabChecksumUtil.Controls.Add(this.chkCsUtilSwapBytes);
            this.tabChecksumUtil.Controls.Add(this.btnCsUtilFix);
            this.tabChecksumUtil.Controls.Add(this.numCSBytes);
            this.tabChecksumUtil.Controls.Add(this.label20);
            this.tabChecksumUtil.Controls.Add(this.txtExclude);
            this.tabChecksumUtil.Controls.Add(this.label19);
            this.tabChecksumUtil.Controls.Add(this.txtCSAddr);
            this.tabChecksumUtil.Controls.Add(this.label18);
            this.tabChecksumUtil.Controls.Add(this.label17);
            this.tabChecksumUtil.Controls.Add(this.txtChecksumRange);
            this.tabChecksumUtil.Controls.Add(this.btnTestChecksum);
            this.tabChecksumUtil.Controls.Add(this.chkCSUtilTryAll);
            this.tabChecksumUtil.Controls.Add(this.groupBox5);
            this.tabChecksumUtil.Controls.Add(this.groupBox1);
            this.tabChecksumUtil.Location = new System.Drawing.Point(4, 22);
            this.tabChecksumUtil.Name = "tabChecksumUtil";
            this.tabChecksumUtil.Size = new System.Drawing.Size(838, 103);
            this.tabChecksumUtil.TabIndex = 5;
            this.tabChecksumUtil.Text = "Checksum research";
            this.tabChecksumUtil.UseVisualStyleBackColor = true;
            // 
            // chkCsUtilSwapBytes
            // 
            this.chkCsUtilSwapBytes.AutoSize = true;
            this.chkCsUtilSwapBytes.Location = new System.Drawing.Point(184, 83);
            this.chkCsUtilSwapBytes.Name = "chkCsUtilSwapBytes";
            this.chkCsUtilSwapBytes.Size = new System.Drawing.Size(81, 17);
            this.chkCsUtilSwapBytes.TabIndex = 133;
            this.chkCsUtilSwapBytes.Text = "Swap bytes";
            this.chkCsUtilSwapBytes.UseVisualStyleBackColor = true;
            // 
            // btnCsUtilFix
            // 
            this.btnCsUtilFix.Location = new System.Drawing.Point(512, 71);
            this.btnCsUtilFix.Name = "btnCsUtilFix";
            this.btnCsUtilFix.Size = new System.Drawing.Size(109, 23);
            this.btnCsUtilFix.TabIndex = 153;
            this.btnCsUtilFix.Text = "Write Checksum";
            this.btnCsUtilFix.UseVisualStyleBackColor = true;
            this.btnCsUtilFix.Click += new System.EventHandler(this.btnCsUtilFix_Click);
            // 
            // numCSBytes
            // 
            this.numCSBytes.Location = new System.Drawing.Point(369, 80);
            this.numCSBytes.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numCSBytes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCSBytes.Name = "numCSBytes";
            this.numCSBytes.Size = new System.Drawing.Size(137, 20);
            this.numCSBytes.TabIndex = 152;
            this.numCSBytes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(298, 81);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(36, 13);
            this.label20.TabIndex = 151;
            this.label20.Text = "Bytes:";
            // 
            // txtExclude
            // 
            this.txtExclude.Location = new System.Drawing.Point(369, 34);
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.Size = new System.Drawing.Size(137, 20);
            this.txtExclude.TabIndex = 150;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(298, 39);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(48, 13);
            this.label19.TabIndex = 149;
            this.label19.Text = "Exclude:";
            // 
            // txtCSAddr
            // 
            this.txtCSAddr.Location = new System.Drawing.Point(369, 57);
            this.txtCSAddr.Name = "txtCSAddr";
            this.txtCSAddr.Size = new System.Drawing.Size(137, 20);
            this.txtCSAddr.TabIndex = 148;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(298, 60);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(64, 13);
            this.label18.TabIndex = 147;
            this.label18.Text = "CS address:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(297, 13);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 13);
            this.label17.TabIndex = 146;
            this.label17.Text = "Range:";
            // 
            // txtChecksumRange
            // 
            this.txtChecksumRange.Location = new System.Drawing.Point(369, 10);
            this.txtChecksumRange.Name = "txtChecksumRange";
            this.txtChecksumRange.Size = new System.Drawing.Size(137, 20);
            this.txtChecksumRange.TabIndex = 145;
            // 
            // btnTestChecksum
            // 
            this.btnTestChecksum.Location = new System.Drawing.Point(512, 39);
            this.btnTestChecksum.Name = "btnTestChecksum";
            this.btnTestChecksum.Size = new System.Drawing.Size(109, 23);
            this.btnTestChecksum.TabIndex = 144;
            this.btnTestChecksum.Text = "Calculate";
            this.btnTestChecksum.UseVisualStyleBackColor = true;
            this.btnTestChecksum.Click += new System.EventHandler(this.btnTestChecksum_Click);
            // 
            // chkCSUtilTryAll
            // 
            this.chkCSUtilTryAll.AutoSize = true;
            this.chkCSUtilTryAll.Location = new System.Drawing.Point(513, 13);
            this.chkCSUtilTryAll.Name = "chkCSUtilTryAll";
            this.chkCSUtilTryAll.Size = new System.Drawing.Size(54, 17);
            this.chkCSUtilTryAll.TabIndex = 143;
            this.chkCSUtilTryAll.Text = "Try all";
            this.chkCSUtilTryAll.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioCSUtilComplement2);
            this.groupBox5.Controls.Add(this.radioCSUtilComplement1);
            this.groupBox5.Controls.Add(this.radioCSUtilComplement0);
            this.groupBox5.Location = new System.Drawing.Point(178, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(114, 81);
            this.groupBox5.TabIndex = 142;
            this.groupBox5.TabStop = false;
            // 
            // radioCSUtilComplement2
            // 
            this.radioCSUtilComplement2.AutoSize = true;
            this.radioCSUtilComplement2.Location = new System.Drawing.Point(6, 54);
            this.radioCSUtilComplement2.Name = "radioCSUtilComplement2";
            this.radioCSUtilComplement2.Size = new System.Drawing.Size(99, 17);
            this.radioCSUtilComplement2.TabIndex = 132;
            this.radioCSUtilComplement2.Text = "2\'s Complement";
            this.radioCSUtilComplement2.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilComplement1
            // 
            this.radioCSUtilComplement1.AutoSize = true;
            this.radioCSUtilComplement1.Location = new System.Drawing.Point(6, 34);
            this.radioCSUtilComplement1.Name = "radioCSUtilComplement1";
            this.radioCSUtilComplement1.Size = new System.Drawing.Size(99, 17);
            this.radioCSUtilComplement1.TabIndex = 131;
            this.radioCSUtilComplement1.Text = "1\'s Complement";
            this.radioCSUtilComplement1.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilComplement0
            // 
            this.radioCSUtilComplement0.AutoSize = true;
            this.radioCSUtilComplement0.Checked = true;
            this.radioCSUtilComplement0.Location = new System.Drawing.Point(6, 16);
            this.radioCSUtilComplement0.Name = "radioCSUtilComplement0";
            this.radioCSUtilComplement0.Size = new System.Drawing.Size(28, 17);
            this.radioCSUtilComplement0.TabIndex = 130;
            this.radioCSUtilComplement0.TabStop = true;
            this.radioCSUtilComplement0.Text = "-";
            this.radioCSUtilComplement0.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioCSUtilDwordSum);
            this.groupBox1.Controls.Add(this.radioCSUtilWordSum);
            this.groupBox1.Controls.Add(this.radioCSUtilSUM);
            this.groupBox1.Controls.Add(this.radioCSUtilCrc32);
            this.groupBox1.Controls.Add(this.radioCSUtilCrc16);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 81);
            this.groupBox1.TabIndex = 141;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checksum method";
            // 
            // radioCSUtilDwordSum
            // 
            this.radioCSUtilDwordSum.AutoSize = true;
            this.radioCSUtilDwordSum.Location = new System.Drawing.Point(78, 54);
            this.radioCSUtilDwordSum.Name = "radioCSUtilDwordSum";
            this.radioCSUtilDwordSum.Size = new System.Drawing.Size(80, 17);
            this.radioCSUtilDwordSum.TabIndex = 125;
            this.radioCSUtilDwordSum.Text = "Dword Sum";
            this.radioCSUtilDwordSum.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilWordSum
            // 
            this.radioCSUtilWordSum.AutoSize = true;
            this.radioCSUtilWordSum.Location = new System.Drawing.Point(78, 19);
            this.radioCSUtilWordSum.Name = "radioCSUtilWordSum";
            this.radioCSUtilWordSum.Size = new System.Drawing.Size(75, 17);
            this.radioCSUtilWordSum.TabIndex = 124;
            this.radioCSUtilWordSum.Text = "Word Sum";
            this.radioCSUtilWordSum.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilSUM
            // 
            this.radioCSUtilSUM.AutoSize = true;
            this.radioCSUtilSUM.Location = new System.Drawing.Point(78, 36);
            this.radioCSUtilSUM.Name = "radioCSUtilSUM";
            this.radioCSUtilSUM.Size = new System.Drawing.Size(73, 17);
            this.radioCSUtilSUM.TabIndex = 123;
            this.radioCSUtilSUM.Text = "Byte SUM";
            this.radioCSUtilSUM.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilCrc32
            // 
            this.radioCSUtilCrc32.AutoSize = true;
            this.radioCSUtilCrc32.Location = new System.Drawing.Point(6, 36);
            this.radioCSUtilCrc32.Name = "radioCSUtilCrc32";
            this.radioCSUtilCrc32.Size = new System.Drawing.Size(59, 17);
            this.radioCSUtilCrc32.TabIndex = 122;
            this.radioCSUtilCrc32.Text = "CRC32";
            this.radioCSUtilCrc32.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilCrc16
            // 
            this.radioCSUtilCrc16.AutoSize = true;
            this.radioCSUtilCrc16.Checked = true;
            this.radioCSUtilCrc16.Location = new System.Drawing.Point(6, 19);
            this.radioCSUtilCrc16.Name = "radioCSUtilCrc16";
            this.radioCSUtilCrc16.Size = new System.Drawing.Size(59, 17);
            this.radioCSUtilCrc16.TabIndex = 121;
            this.radioCSUtilCrc16.TabStop = true;
            this.radioCSUtilCrc16.Text = "CRC16";
            this.radioCSUtilCrc16.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(843, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuMain";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem3.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveToolStripMenuItem.Text = "&Save bin";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click_1);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveAsToolStripMenuItem.Text = "Save bin &as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadConfigToolStripMenuItem,
            this.setupSegmentsToolStripMenuItem,
            this.autodetectToolStripMenuItem,
            this.stockCVNToolStripMenuItem,
            this.editTableSearchToolStripMenuItem,
            this.fileTypesToolStripMenuItem,
            this.dTCSearchToolStripMenuItem,
            this.pIDSearchToolStripMenuItem,
            this.tableSeekToolStripMenuItem,
            this.segmentSeekToolStripMenuItem,
            this.oBD2CodesToolStripMenuItem,
            this.rememberWindowSizeToolStripMenuItem,
            this.disableTunerAutloadConfigToolStripMenuItem,
            this.moreSettingsToolStripMenuItem,
            this.advancedModeToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(61, 20);
            this.toolStripMenuItem1.Text = "&Settings";
            // 
            // loadConfigToolStripMenuItem
            // 
            this.loadConfigToolStripMenuItem.Name = "loadConfigToolStripMenuItem";
            this.loadConfigToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.loadConfigToolStripMenuItem.Text = "Load config (XML)";
            this.loadConfigToolStripMenuItem.Click += new System.EventHandler(this.loadConfigToolStripMenuItem_Click);
            // 
            // setupSegmentsToolStripMenuItem
            // 
            this.setupSegmentsToolStripMenuItem.Name = "setupSegmentsToolStripMenuItem";
            this.setupSegmentsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.setupSegmentsToolStripMenuItem.Text = "Edit config (XML)";
            this.setupSegmentsToolStripMenuItem.Click += new System.EventHandler(this.setupSegmentsToolStripMenuItem_Click);
            // 
            // autodetectToolStripMenuItem
            // 
            this.autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            this.autodetectToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.autodetectToolStripMenuItem.Text = "Autodetect";
            this.autodetectToolStripMenuItem.Click += new System.EventHandler(this.autodetectToolStripMenuItem_Click);
            // 
            // stockCVNToolStripMenuItem
            // 
            this.stockCVNToolStripMenuItem.Name = "stockCVNToolStripMenuItem";
            this.stockCVNToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.stockCVNToolStripMenuItem.Text = "Stock CVN";
            this.stockCVNToolStripMenuItem.Click += new System.EventHandler(this.stockCVNToolStripMenuItem_Click);
            // 
            // editTableSearchToolStripMenuItem
            // 
            this.editTableSearchToolStripMenuItem.Name = "editTableSearchToolStripMenuItem";
            this.editTableSearchToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.editTableSearchToolStripMenuItem.Text = "Table Search ";
            this.editTableSearchToolStripMenuItem.Click += new System.EventHandler(this.editTableSearchToolStripMenuItem_Click);
            // 
            // fileTypesToolStripMenuItem
            // 
            this.fileTypesToolStripMenuItem.Name = "fileTypesToolStripMenuItem";
            this.fileTypesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.fileTypesToolStripMenuItem.Text = "File Types";
            this.fileTypesToolStripMenuItem.Click += new System.EventHandler(this.fileTypesToolStripMenuItem_Click);
            // 
            // dTCSearchToolStripMenuItem
            // 
            this.dTCSearchToolStripMenuItem.Name = "dTCSearchToolStripMenuItem";
            this.dTCSearchToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.dTCSearchToolStripMenuItem.Text = "DTC Search";
            this.dTCSearchToolStripMenuItem.Click += new System.EventHandler(this.dTCSearchToolStripMenuItem_Click);
            // 
            // pIDSearchToolStripMenuItem
            // 
            this.pIDSearchToolStripMenuItem.Name = "pIDSearchToolStripMenuItem";
            this.pIDSearchToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.pIDSearchToolStripMenuItem.Text = "PID Search";
            this.pIDSearchToolStripMenuItem.Click += new System.EventHandler(this.pIDSearchToolStripMenuItem_Click);
            // 
            // tableSeekToolStripMenuItem
            // 
            this.tableSeekToolStripMenuItem.Name = "tableSeekToolStripMenuItem";
            this.tableSeekToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.tableSeekToolStripMenuItem.Text = "TableSeek";
            this.tableSeekToolStripMenuItem.Click += new System.EventHandler(this.tableSeekToolStripMenuItem_Click);
            // 
            // segmentSeekToolStripMenuItem
            // 
            this.segmentSeekToolStripMenuItem.Name = "segmentSeekToolStripMenuItem";
            this.segmentSeekToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.segmentSeekToolStripMenuItem.Text = "SegmentSeek";
            this.segmentSeekToolStripMenuItem.Click += new System.EventHandler(this.segmentSeekToolStripMenuItem_Click);
            // 
            // oBD2CodesToolStripMenuItem
            // 
            this.oBD2CodesToolStripMenuItem.Name = "oBD2CodesToolStripMenuItem";
            this.oBD2CodesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.oBD2CodesToolStripMenuItem.Text = "OBD2 Codes";
            this.oBD2CodesToolStripMenuItem.Click += new System.EventHandler(this.oBD2CodesToolStripMenuItem_Click);
            // 
            // rememberWindowSizeToolStripMenuItem
            // 
            this.rememberWindowSizeToolStripMenuItem.Name = "rememberWindowSizeToolStripMenuItem";
            this.rememberWindowSizeToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.rememberWindowSizeToolStripMenuItem.Text = "Remember window size";
            this.rememberWindowSizeToolStripMenuItem.Click += new System.EventHandler(this.rememberWindowSizeToolStripMenuItem_Click);
            // 
            // disableTunerAutloadConfigToolStripMenuItem
            // 
            this.disableTunerAutloadConfigToolStripMenuItem.Name = "disableTunerAutloadConfigToolStripMenuItem";
            this.disableTunerAutloadConfigToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.disableTunerAutloadConfigToolStripMenuItem.Text = "Disable tuner autoload config";
            this.disableTunerAutloadConfigToolStripMenuItem.Click += new System.EventHandler(this.disableTunerAutoloadConfigToolStripMenuItem_Click);
            // 
            // moreSettingsToolStripMenuItem
            // 
            this.moreSettingsToolStripMenuItem.Name = "moreSettingsToolStripMenuItem";
            this.moreSettingsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.moreSettingsToolStripMenuItem.Text = "More settings...";
            this.moreSettingsToolStripMenuItem.Click += new System.EventHandler(this.moreSettingsToolStripMenuItem_Click);
            // 
            // advancedModeToolStripMenuItem
            // 
            this.advancedModeToolStripMenuItem.Name = "advancedModeToolStripMenuItem";
            this.advancedModeToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.advancedModeToolStripMenuItem.Text = "Advanced Mode";
            this.advancedModeToolStripMenuItem.Click += new System.EventHandler(this.advancedModeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem2.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // FrmPatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 561);
            this.Controls.Add(this.tabFunction);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkAutodetect);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.txtBaseFile);
            this.Controls.Add(this.btnOrgFile);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmPatcher";
            this.Text = "Universal patcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPatcher_FormClosing);
            this.Load += new System.EventHandler(this.FrmPatcher_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabInfo.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.tabPatch.ResumeLayout(false);
            this.tabPatch.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataPatch)).EndInit();
            this.tabCVN.ResumeLayout(false);
            this.tabCVN.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataCVN)).EndInit();
            this.tabBadCvn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBadCvn)).EndInit();
            this.tabFinfo.ResumeLayout(false);
            this.tabFinfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataFileInfo)).EndInit();
            this.tabCsAddress.ResumeLayout(false);
            this.tabCsAddress.PerformLayout();
            this.tabBadChkFile.ResumeLayout(false);
            this.tabBadChkFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataBadChkFile)).EndInit();
            this.tabSearchedTables.ResumeLayout(false);
            this.tabSearchedTables.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSearchedTables)).EndInit();
            this.tabPIDList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPIDlist)).EndInit();
            this.tabDTC.ResumeLayout(false);
            this.tabDTC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDTC)).EndInit();
            this.tabTableSeek.ResumeLayout(false);
            this.tabTableSeek.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTableSeek)).EndInit();
            this.tabFunction.ResumeLayout(false);
            this.tabApply.ResumeLayout(false);
            this.tabFileinfo.ResumeLayout(false);
            this.tabFileinfo.PerformLayout();
            this.groupSearch.ResumeLayout(false);
            this.groupSearch.PerformLayout();
            this.tabCreate.ResumeLayout(false);
            this.tabCreate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCrossVariation)).EndInit();
            this.tabExtract.ResumeLayout(false);
            this.tabExtract.PerformLayout();
            this.tabExtractSegments.ResumeLayout(false);
            this.tabExtractSegments.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabChecksumUtil.ResumeLayout(false);
            this.tabChecksumUtil.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCSBytes)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBaseFile;
        private System.Windows.Forms.TextBox txtModifierFile;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnSaveBin;
        private System.Windows.Forms.TextBox txtPatchDescription;
        private System.Windows.Forms.Label labelBinSize;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnOrgFile;
        public System.Windows.Forms.Button btnModFile;
        private System.Windows.Forms.Label labelDescr;
        private System.Windows.Forms.NumericUpDown numSuppress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkCompareAll;
        private System.Windows.Forms.CheckBox chkAutodetect;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.Button btnLoadFolder;
        private System.Windows.Forms.Button btnSaveFileInfo;
        private System.Windows.Forms.CheckBox chkExtra;
        private System.Windows.Forms.CheckBox chkCS2;
        private System.Windows.Forms.CheckBox chkCS1;
        private System.Windows.Forms.CheckBox chkSize;
        private System.Windows.Forms.CheckBox chkRange;
        private System.Windows.Forms.TabControl tabFunction;
        private System.Windows.Forms.TabPage tabCreate;
        private System.Windows.Forms.TabPage tabApply;
        private System.Windows.Forms.Button btnApplypatch;
        private System.Windows.Forms.TabPage tabFileinfo;
        private System.Windows.Forms.TabPage tabExtract;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtExtractRange;
        private System.Windows.Forms.TextBox txtCompatibleOS;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.TextBox txtExtractDescription;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtOS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPatch;
        private System.Windows.Forms.DataGridView dataPatch;
        private System.Windows.Forms.Button btnCheckSums;
        private System.Windows.Forms.Label labelPatchname;
        private System.Windows.Forms.Button btnManualPatch;
        private System.Windows.Forms.Button btnBinLoadPatch;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupSegmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem autodetectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnAddtoStock;
        private System.Windows.Forms.ToolStripMenuItem stockCVNToolStripMenuItem;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnLoadPatch;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tabCVN;
        private System.Windows.Forms.DataGridView dataCVN;
        private System.Windows.Forms.Button btnClearCVN;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.TabPage tabFinfo;
        private System.Windows.Forms.DataGridView dataFileInfo;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveCSV;
        private System.Windows.Forms.RichTextBox txtDebug;
        private System.Windows.Forms.CheckBox checkAppendPatch;
        private System.Windows.Forms.TabPage tabExtractSegments;
        private System.Windows.Forms.Button btnExtractSegments;
        private System.Windows.Forms.TextBox txtSegmentDescription;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnExtractSegmentsFolder;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioRename;
        private System.Windows.Forms.RadioButton radioReplace;
        private System.Windows.Forms.RadioButton radioSkip;
        private System.Windows.Forms.Button btnSwapSegments;
        private System.Windows.Forms.Button btnFixFilesChecksum;
        private System.Windows.Forms.CheckBox checkExtractShowinfo;
        private System.Windows.Forms.CheckBox checkAutorefreshFileinfo;
        private System.Windows.Forms.Button btnRefreshFileinfo;
        private System.Windows.Forms.CheckBox checkAutorefreshCVNlist;
        private System.Windows.Forms.Button btnRefreshCvnList;
        private System.Windows.Forms.CheckBox chkLogtoFile;
        private System.Windows.Forms.CheckBox chkLogtodisplay;
        private System.Windows.Forms.TabPage tabCsAddress;
        private System.Windows.Forms.ListView listCSAddresses;
        private System.Windows.Forms.Button btnSaveCSaddresses;
        private System.Windows.Forms.Button btnClearCSAddresses;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage tabBadChkFile;
        private System.Windows.Forms.Button btnRefreshBadChkFile;
        private System.Windows.Forms.Button btnSaveCsvBadChkFile;
        private System.Windows.Forms.Button btnClearBadchkFile;
        private System.Windows.Forms.DataGridView dataBadChkFile;
        private System.Windows.Forms.CheckBox chkAutoRefreshBadChkFile;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnSaveDebug;
        private System.Windows.Forms.ToolStripMenuItem editTableSearchToolStripMenuItem;
        private System.Windows.Forms.TabPage tabSearchedTables;
        private System.Windows.Forms.CheckBox chkSearchTables;
        private System.Windows.Forms.DataGridView dataGridSearchedTables;
        private System.Windows.Forms.Button btnSaveSearchedTables;
        private System.Windows.Forms.Button btnClearSearchedTables;
        private System.Windows.Forms.TabPage tabPIDList;
        private System.Windows.Forms.Button btnClearPidList;
        private System.Windows.Forms.Button btnSavePidList;
        private System.Windows.Forms.DataGridView dataGridPIDlist;
        private System.Windows.Forms.CheckBox chkTableSearchNoFilters;
        private System.Windows.Forms.TabPage tabBadCvn;
        private System.Windows.Forms.Button BtnRefreshBadCvn;
        private System.Windows.Forms.Button btnClearBadCvn;
        private System.Windows.Forms.DataGridView dataGridBadCvn;
        private System.Windows.Forms.Button btnCustomSearch;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCustomSearchStartAddress;
        private System.Windows.Forms.TextBox txtCustomSearchString;
        private System.Windows.Forms.Button btnCustomSearchNext;
        private System.Windows.Forms.Button btnCustomFindAll;
        private System.Windows.Forms.CheckBox chkCustomTableSearch;
        private System.Windows.Forms.ToolStripMenuItem fileTypesToolStripMenuItem;
        private System.Windows.Forms.Button btnCrossTableSearch;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numCrossVariation;
        private System.Windows.Forms.Button btnSaveDecCsv;
        private System.Windows.Forms.TabPage tabDTC;
        private System.Windows.Forms.Button btnClearDTC;
        private System.Windows.Forms.Button btnSaveCsvDTC;
        private System.Windows.Forms.DataGridView dataGridDTC;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnSetDTC;
        private System.Windows.Forms.ToolStripMenuItem dTCSearchToolStripMenuItem;
        private System.Windows.Forms.Button btnShowTableData;
        private System.Windows.Forms.CheckBox chkSearchDTC;
        private System.Windows.Forms.ToolStripMenuItem tableSeekToolStripMenuItem;
        private System.Windows.Forms.TabPage tabTableSeek;
        private System.Windows.Forms.Button btnClearTableSeek;
        private System.Windows.Forms.DataGridView dataGridTableSeek;
        private System.Windows.Forms.CheckBox chkTableSeek;
        private System.Windows.Forms.CheckBox chkSearchPids;
        private System.Windows.Forms.Button btnEditTable;
        private System.Windows.Forms.ToolStripMenuItem rememberWindowSizeToolStripMenuItem;
        private System.Windows.Forms.Button btnReadTinyTunerDB;
        private System.Windows.Forms.ComboBox comboTableCategory;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox chkForceCompare;
        private System.Windows.Forms.TextBox txtSearchTableSeek;
        private System.Windows.Forms.Button btnTuner;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableTunerAutloadConfigToolStripMenuItem;
        public System.Windows.Forms.Label labelXML;
        private System.Windows.Forms.ToolStripMenuItem moreSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oBD2CodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem segmentSeekToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pIDSearchToolStripMenuItem;
        private System.Windows.Forms.TabPage tabChecksumUtil;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtChecksumRange;
        private System.Windows.Forms.Button btnTestChecksum;
        private System.Windows.Forms.CheckBox chkCSUtilTryAll;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioCSUtilComplement2;
        private System.Windows.Forms.RadioButton radioCSUtilComplement1;
        private System.Windows.Forms.RadioButton radioCSUtilComplement0;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioCSUtilDwordSum;
        private System.Windows.Forms.RadioButton radioCSUtilWordSum;
        private System.Windows.Forms.RadioButton radioCSUtilSUM;
        private System.Windows.Forms.RadioButton radioCSUtilCrc32;
        private System.Windows.Forms.RadioButton radioCSUtilCrc16;
        private System.Windows.Forms.TextBox txtCSAddr;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown numCSBytes;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button btnCsUtilFix;
        private System.Windows.Forms.CheckBox chkCsUtilSwapBytes;
        private System.Windows.Forms.ListBox listPatches;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSaveAllPatches;
        private System.Windows.Forms.Button btnDelPatch;
        private System.Windows.Forms.Button btnAddPatch;
        private System.Windows.Forms.ToolStripMenuItem advancedModeToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupSearch;
    }
}