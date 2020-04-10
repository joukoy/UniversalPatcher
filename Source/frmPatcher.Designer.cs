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
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnSaveFileInfo = new System.Windows.Forms.Button();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.txtDebug = new System.Windows.Forms.RichTextBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.tabPatch = new System.Windows.Forms.TabPage();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnManualPatch = new System.Windows.Forms.Button();
            this.labelPatchname = new System.Windows.Forms.Label();
            this.dataPatch = new System.Windows.Forms.DataGridView();
            this.tabCVN = new System.Windows.Forms.TabPage();
            this.btnClearCVN = new System.Windows.Forms.Button();
            this.btnAddtoStock = new System.Windows.Forms.Button();
            this.dataCVN = new System.Windows.Forms.DataGridView();
            this.tabFinfo = new System.Windows.Forms.TabPage();
            this.btnSaveCSV = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.dataFileInfo = new System.Windows.Forms.DataGridView();
            this.chkExtra = new System.Windows.Forms.CheckBox();
            this.chkCS2 = new System.Windows.Forms.CheckBox();
            this.chkCS1 = new System.Windows.Forms.CheckBox();
            this.chkSize = new System.Windows.Forms.CheckBox();
            this.chkRange = new System.Windows.Forms.CheckBox();
            this.btnLoadFolder = new System.Windows.Forms.Button();
            this.tabFunction = new System.Windows.Forms.TabControl();
            this.tabCreate = new System.Windows.Forms.TabPage();
            this.checkAppendPatch = new System.Windows.Forms.CheckBox();
            this.txtOS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabFileinfo = new System.Windows.Forms.TabPage();
            this.tabApply = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSwapSegments = new System.Windows.Forms.Button();
            this.btnBinLoadPatch = new System.Windows.Forms.Button();
            this.btnCheckSums = new System.Windows.Forms.Button();
            this.btnApplypatch = new System.Windows.Forms.Button();
            this.tabExtract = new System.Windows.Forms.TabPage();
            this.txtExtractDescription = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.txtCompatibleOS = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExtractRange = new System.Windows.Forms.TextBox();
            this.tabExtractSegments = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioRename = new System.Windows.Forms.RadioButton();
            this.radioReplace = new System.Windows.Forms.RadioButton();
            this.radioSkip = new System.Windows.Forms.RadioButton();
            this.btnExtractSegmentsFolder = new System.Windows.Forms.Button();
            this.txtSegmentDescription = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnExtractSegments = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stockCVNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.tabPatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPatch)).BeginInit();
            this.tabCVN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataCVN)).BeginInit();
            this.tabFinfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataFileInfo)).BeginInit();
            this.tabFunction.SuspendLayout();
            this.tabCreate.SuspendLayout();
            this.tabFileinfo.SuspendLayout();
            this.tabApply.SuspendLayout();
            this.tabExtract.SuspendLayout();
            this.tabExtractSegments.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.txtBaseFile.Size = new System.Drawing.Size(685, 20);
            this.txtBaseFile.TabIndex = 15;
            // 
            // txtModifierFile
            // 
            this.txtModifierFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModifierFile.Location = new System.Drawing.Point(90, 6);
            this.txtModifierFile.Name = "txtModifierFile";
            this.txtModifierFile.Size = new System.Drawing.Size(685, 20);
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
            this.btnSaveBin.Location = new System.Drawing.Point(666, 6);
            this.btnSaveBin.Name = "btnSaveBin";
            this.btnSaveBin.Size = new System.Drawing.Size(108, 25);
            this.btnSaveBin.TabIndex = 184;
            this.btnSaveBin.Text = "Save bin";
            this.btnSaveBin.UseVisualStyleBackColor = true;
            this.btnSaveBin.Click += new System.EventHandler(this.btnSaveBin_Click);
            // 
            // txtPatchDescription
            // 
            this.txtPatchDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatchDescription.Location = new System.Drawing.Point(129, 57);
            this.txtPatchDescription.Name = "txtPatchDescription";
            this.txtPatchDescription.Size = new System.Drawing.Size(546, 20);
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
            this.labelXML.Location = new System.Drawing.Point(260, 8);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(13, 16);
            this.labelXML.TabIndex = 16;
            this.labelXML.Text = "-";
            // 
            // numSuppress
            // 
            this.numSuppress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numSuppress.Location = new System.Drawing.Point(657, 3);
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
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(592, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Show max:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(700, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "patch rows";
            // 
            // chkCompareAll
            // 
            this.chkCompareAll.AutoSize = true;
            this.chkCompareAll.Location = new System.Drawing.Point(257, 34);
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
            this.chkAutodetect.Location = new System.Drawing.Point(130, 9);
            this.chkAutodetect.Name = "chkAutodetect";
            this.chkAutodetect.Size = new System.Drawing.Size(110, 17);
            this.chkAutodetect.TabIndex = 11;
            this.chkAutodetect.Text = "Autodetect config";
            this.chkAutodetect.UseVisualStyleBackColor = true;
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
            this.tabControl1.Controls.Add(this.tabFinfo);
            this.tabControl1.Location = new System.Drawing.Point(0, 191);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(784, 370);
            this.tabControl1.TabIndex = 200;
            // 
            // tabInfo
            // 
            this.tabInfo.AutoScroll = true;
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
            this.tabInfo.Size = new System.Drawing.Size(776, 344);
            this.tabInfo.TabIndex = 0;
            this.tabInfo.Text = "Log";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(2, 26);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(774, 317);
            this.txtResult.TabIndex = 202;
            this.txtResult.Text = "";
            // 
            // btnSaveFileInfo
            // 
            this.btnSaveFileInfo.Location = new System.Drawing.Point(508, 1);
            this.btnSaveFileInfo.Name = "btnSaveFileInfo";
            this.btnSaveFileInfo.Size = new System.Drawing.Size(78, 23);
            this.btnSaveFileInfo.TabIndex = 171;
            this.btnSaveFileInfo.Text = "Save log...";
            this.btnSaveFileInfo.UseVisualStyleBackColor = true;
            this.btnSaveFileInfo.Click += new System.EventHandler(this.btnSaveFileInfo_Click);
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.txtDebug);
            this.tabDebug.Controls.Add(this.chkDebug);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(776, 344);
            this.tabDebug.TabIndex = 1;
            this.tabDebug.Text = "Debug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // txtDebug
            // 
            this.txtDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDebug.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDebug.Location = new System.Drawing.Point(2, 24);
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(774, 323);
            this.txtDebug.TabIndex = 212;
            this.txtDebug.Text = "";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Checked = true;
            this.chkDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDebug.Location = new System.Drawing.Point(0, 3);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(73, 17);
            this.chkDebug.TabIndex = 211;
            this.chkDebug.Text = "Debug on";
            this.chkDebug.UseVisualStyleBackColor = true;
            this.chkDebug.CheckedChanged += new System.EventHandler(this.chkDebug_CheckedChanged);
            // 
            // tabPatch
            // 
            this.tabPatch.Controls.Add(this.btnRefresh);
            this.tabPatch.Controls.Add(this.btnNew);
            this.tabPatch.Controls.Add(this.btnLoad);
            this.tabPatch.Controls.Add(this.btnSave);
            this.tabPatch.Controls.Add(this.btnHelp);
            this.tabPatch.Controls.Add(this.btnEdit);
            this.tabPatch.Controls.Add(this.btnDown);
            this.tabPatch.Controls.Add(this.btnUp);
            this.tabPatch.Controls.Add(this.btnDelete);
            this.tabPatch.Controls.Add(this.btnManualPatch);
            this.tabPatch.Controls.Add(this.labelPatchname);
            this.tabPatch.Controls.Add(this.dataPatch);
            this.tabPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPatch.Location = new System.Drawing.Point(4, 22);
            this.tabPatch.Name = "tabPatch";
            this.tabPatch.Size = new System.Drawing.Size(776, 344);
            this.tabPatch.TabIndex = 2;
            this.tabPatch.Text = "Patch editor";
            this.tabPatch.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(549, 0);
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
            this.btnNew.Location = new System.Drawing.Point(614, 0);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(49, 23);
            this.btnNew.TabIndex = 251;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(667, 0);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(49, 23);
            this.btnLoad.TabIndex = 252;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click_1);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(722, 0);
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
            this.btnHelp.Location = new System.Drawing.Point(729, 138);
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
            this.btnEdit.Location = new System.Drawing.Point(729, 56);
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
            this.btnDown.Location = new System.Drawing.Point(730, 219);
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
            this.btnUp.Location = new System.Drawing.Point(729, 191);
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
            this.btnDelete.Location = new System.Drawing.Point(729, 110);
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
            this.btnManualPatch.Location = new System.Drawing.Point(729, 84);
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
            // dataPatch
            // 
            this.dataPatch.AllowUserToOrderColumns = true;
            this.dataPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataPatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataPatch.Location = new System.Drawing.Point(1, 27);
            this.dataPatch.Name = "dataPatch";
            this.dataPatch.Size = new System.Drawing.Size(722, 320);
            this.dataPatch.TabIndex = 0;
            this.dataPatch.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataPatch_CellContentDoubleClick);
            // 
            // tabCVN
            // 
            this.tabCVN.Controls.Add(this.btnClearCVN);
            this.tabCVN.Controls.Add(this.btnAddtoStock);
            this.tabCVN.Controls.Add(this.dataCVN);
            this.tabCVN.Location = new System.Drawing.Point(4, 22);
            this.tabCVN.Name = "tabCVN";
            this.tabCVN.Size = new System.Drawing.Size(776, 344);
            this.tabCVN.TabIndex = 3;
            this.tabCVN.Text = "CVN";
            this.tabCVN.UseVisualStyleBackColor = true;
            // 
            // btnClearCVN
            // 
            this.btnClearCVN.Location = new System.Drawing.Point(5, 7);
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
            this.btnAddtoStock.Location = new System.Drawing.Point(637, 7);
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
            this.dataCVN.Size = new System.Drawing.Size(776, 310);
            this.dataCVN.TabIndex = 0;
            // 
            // tabFinfo
            // 
            this.tabFinfo.Controls.Add(this.btnSaveCSV);
            this.tabFinfo.Controls.Add(this.btnClear);
            this.tabFinfo.Controls.Add(this.dataFileInfo);
            this.tabFinfo.Location = new System.Drawing.Point(4, 22);
            this.tabFinfo.Name = "tabFinfo";
            this.tabFinfo.Size = new System.Drawing.Size(776, 344);
            this.tabFinfo.TabIndex = 4;
            this.tabFinfo.Text = "File info";
            this.tabFinfo.UseVisualStyleBackColor = true;
            // 
            // btnSaveCSV
            // 
            this.btnSaveCSV.Location = new System.Drawing.Point(693, 4);
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
            this.dataFileInfo.Size = new System.Drawing.Size(778, 312);
            this.dataFileInfo.TabIndex = 0;
            // 
            // chkExtra
            // 
            this.chkExtra.AutoSize = true;
            this.chkExtra.Checked = true;
            this.chkExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExtra.Location = new System.Drawing.Point(402, 10);
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
            this.chkCS2.Location = new System.Drawing.Point(314, 10);
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
            this.chkCS1.Location = new System.Drawing.Point(223, 10);
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
            this.chkSize.Location = new System.Drawing.Point(581, 10);
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
            this.chkRange.Location = new System.Drawing.Point(478, 10);
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
            this.btnLoadFolder.Size = new System.Drawing.Size(80, 29);
            this.btnLoadFolder.TabIndex = 170;
            this.btnLoadFolder.Text = "Select Folder";
            this.btnLoadFolder.UseVisualStyleBackColor = true;
            this.btnLoadFolder.Click += new System.EventHandler(this.btnLoadFolder_Click);
            // 
            // tabFunction
            // 
            this.tabFunction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabFunction.Controls.Add(this.tabCreate);
            this.tabFunction.Controls.Add(this.tabFileinfo);
            this.tabFunction.Controls.Add(this.tabApply);
            this.tabFunction.Controls.Add(this.tabExtract);
            this.tabFunction.Controls.Add(this.tabExtractSegments);
            this.tabFunction.Location = new System.Drawing.Point(2, 60);
            this.tabFunction.Name = "tabFunction";
            this.tabFunction.SelectedIndex = 0;
            this.tabFunction.Size = new System.Drawing.Size(787, 129);
            this.tabFunction.TabIndex = 100;
            // 
            // tabCreate
            // 
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
            this.tabCreate.Size = new System.Drawing.Size(779, 103);
            this.tabCreate.TabIndex = 0;
            this.tabCreate.Text = "Create Patch";
            this.tabCreate.UseVisualStyleBackColor = true;
            // 
            // checkAppendPatch
            // 
            this.checkAppendPatch.AutoSize = true;
            this.checkAppendPatch.Location = new System.Drawing.Point(445, 34);
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
            // tabFileinfo
            // 
            this.tabFileinfo.Controls.Add(this.chkExtra);
            this.tabFileinfo.Controls.Add(this.btnLoadFolder);
            this.tabFileinfo.Controls.Add(this.chkCS2);
            this.tabFileinfo.Controls.Add(this.chkCS1);
            this.tabFileinfo.Controls.Add(this.chkRange);
            this.tabFileinfo.Controls.Add(this.chkSize);
            this.tabFileinfo.Location = new System.Drawing.Point(4, 22);
            this.tabFileinfo.Name = "tabFileinfo";
            this.tabFileinfo.Size = new System.Drawing.Size(779, 103);
            this.tabFileinfo.TabIndex = 2;
            this.tabFileinfo.Text = "File info";
            this.tabFileinfo.UseVisualStyleBackColor = true;
            // 
            // tabApply
            // 
            this.tabApply.Controls.Add(this.button1);
            this.tabApply.Controls.Add(this.btnSwapSegments);
            this.tabApply.Controls.Add(this.btnBinLoadPatch);
            this.tabApply.Controls.Add(this.btnCheckSums);
            this.tabApply.Controls.Add(this.btnApplypatch);
            this.tabApply.Controls.Add(this.btnSaveBin);
            this.tabApply.Location = new System.Drawing.Point(4, 22);
            this.tabApply.Name = "tabApply";
            this.tabApply.Padding = new System.Windows.Forms.Padding(3);
            this.tabApply.Size = new System.Drawing.Size(779, 103);
            this.tabApply.TabIndex = 1;
            this.tabApply.Text = "Modify bin";
            this.tabApply.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(124, 37);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 25);
            this.button1.TabIndex = 186;
            this.button1.Text = "Fix checksum of files...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSwapSegments
            // 
            this.btnSwapSegments.Location = new System.Drawing.Point(236, 6);
            this.btnSwapSegments.Name = "btnSwapSegments";
            this.btnSwapSegments.Size = new System.Drawing.Size(107, 25);
            this.btnSwapSegments.TabIndex = 185;
            this.btnSwapSegments.Text = "Swap segment(s)";
            this.btnSwapSegments.UseVisualStyleBackColor = true;
            this.btnSwapSegments.Click += new System.EventHandler(this.btnSwapSegments_Click);
            // 
            // btnBinLoadPatch
            // 
            this.btnBinLoadPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
            this.btnCheckSums.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckSums.Location = new System.Drawing.Point(10, 37);
            this.btnCheckSums.Name = "btnCheckSums";
            this.btnCheckSums.Size = new System.Drawing.Size(107, 25);
            this.btnCheckSums.TabIndex = 183;
            this.btnCheckSums.Text = "Fix checksums";
            this.btnCheckSums.UseVisualStyleBackColor = true;
            this.btnCheckSums.Click += new System.EventHandler(this.btnCheckSums_Click);
            // 
            // btnApplypatch
            // 
            this.btnApplypatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplypatch.Location = new System.Drawing.Point(123, 6);
            this.btnApplypatch.Name = "btnApplypatch";
            this.btnApplypatch.Size = new System.Drawing.Size(107, 25);
            this.btnApplypatch.TabIndex = 182;
            this.btnApplypatch.Text = "Apply current patch";
            this.btnApplypatch.UseVisualStyleBackColor = true;
            this.btnApplypatch.Click += new System.EventHandler(this.btnApplypatch_Click);
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
            this.tabExtract.Size = new System.Drawing.Size(779, 103);
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
            this.btnExtract.Location = new System.Drawing.Point(637, 9);
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
            this.tabExtractSegments.Controls.Add(this.groupBox2);
            this.tabExtractSegments.Controls.Add(this.btnExtractSegmentsFolder);
            this.tabExtractSegments.Controls.Add(this.txtSegmentDescription);
            this.tabExtractSegments.Controls.Add(this.label8);
            this.tabExtractSegments.Controls.Add(this.btnExtractSegments);
            this.tabExtractSegments.Location = new System.Drawing.Point(4, 22);
            this.tabExtractSegments.Name = "tabExtractSegments";
            this.tabExtractSegments.Size = new System.Drawing.Size(779, 103);
            this.tabExtractSegments.TabIndex = 4;
            this.tabExtractSegments.Text = "Extract segments";
            this.tabExtractSegments.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioRename);
            this.groupBox2.Controls.Add(this.radioReplace);
            this.groupBox2.Controls.Add(this.radioSkip);
            this.groupBox2.Location = new System.Drawing.Point(3, 33);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(284, 42);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Duplicates";
            // 
            // radioRename
            // 
            this.radioRename.AutoSize = true;
            this.radioRename.Location = new System.Drawing.Point(143, 19);
            this.radioRename.Name = "radioRename";
            this.radioRename.Size = new System.Drawing.Size(65, 17);
            this.radioRename.TabIndex = 2;
            this.radioRename.Text = "Rename";
            this.radioRename.UseVisualStyleBackColor = true;
            // 
            // radioReplace
            // 
            this.radioReplace.AutoSize = true;
            this.radioReplace.Location = new System.Drawing.Point(72, 19);
            this.radioReplace.Name = "radioReplace";
            this.radioReplace.Size = new System.Drawing.Size(65, 17);
            this.radioReplace.TabIndex = 1;
            this.radioReplace.Text = "Replace";
            this.radioReplace.UseVisualStyleBackColor = true;
            // 
            // radioSkip
            // 
            this.radioSkip.AutoSize = true;
            this.radioSkip.Checked = true;
            this.radioSkip.Location = new System.Drawing.Point(7, 19);
            this.radioSkip.Name = "radioSkip";
            this.radioSkip.Size = new System.Drawing.Size(46, 17);
            this.radioSkip.TabIndex = 0;
            this.radioSkip.TabStop = true;
            this.radioSkip.Text = "Skip";
            this.radioSkip.UseVisualStyleBackColor = true;
            // 
            // btnExtractSegmentsFolder
            // 
            this.btnExtractSegmentsFolder.Location = new System.Drawing.Point(318, 48);
            this.btnExtractSegmentsFolder.Name = "btnExtractSegmentsFolder";
            this.btnExtractSegmentsFolder.Size = new System.Drawing.Size(266, 21);
            this.btnExtractSegmentsFolder.TabIndex = 3;
            this.btnExtractSegmentsFolder.Text = "Extract all segments from all files in folder...";
            this.btnExtractSegmentsFolder.UseVisualStyleBackColor = true;
            this.btnExtractSegmentsFolder.Click += new System.EventHandler(this.btnExtractSegmentsFolder_Click);
            // 
            // txtSegmentDescription
            // 
            this.txtSegmentDescription.Location = new System.Drawing.Point(83, 7);
            this.txtSegmentDescription.Name = "txtSegmentDescription";
            this.txtSegmentDescription.Size = new System.Drawing.Size(565, 20);
            this.txtSegmentDescription.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Description:";
            // 
            // btnExtractSegments
            // 
            this.btnExtractSegments.Location = new System.Drawing.Point(651, 3);
            this.btnExtractSegments.Name = "btnExtractSegments";
            this.btnExtractSegments.Size = new System.Drawing.Size(123, 26);
            this.btnExtractSegments.TabIndex = 0;
            this.btnExtractSegments.Text = "Extract current file";
            this.btnExtractSegments.UseVisualStyleBackColor = true;
            this.btnExtractSegments.Click += new System.EventHandler(this.btnExtractSegments_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuMain";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadConfigToolStripMenuItem,
            this.setupSegmentsToolStripMenuItem,
            this.autodetectToolStripMenuItem,
            this.stockCVNToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(61, 20);
            this.toolStripMenuItem1.Text = "Settings";
            // 
            // loadConfigToolStripMenuItem
            // 
            this.loadConfigToolStripMenuItem.Name = "loadConfigToolStripMenuItem";
            this.loadConfigToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.loadConfigToolStripMenuItem.Text = "Load config";
            this.loadConfigToolStripMenuItem.Click += new System.EventHandler(this.loadConfigToolStripMenuItem_Click);
            // 
            // setupSegmentsToolStripMenuItem
            // 
            this.setupSegmentsToolStripMenuItem.Name = "setupSegmentsToolStripMenuItem";
            this.setupSegmentsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.setupSegmentsToolStripMenuItem.Text = "Segments";
            this.setupSegmentsToolStripMenuItem.Click += new System.EventHandler(this.setupSegmentsToolStripMenuItem_Click);
            // 
            // autodetectToolStripMenuItem
            // 
            this.autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            this.autodetectToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.autodetectToolStripMenuItem.Text = "Autodetect";
            this.autodetectToolStripMenuItem.Click += new System.EventHandler(this.autodetectToolStripMenuItem_Click);
            // 
            // stockCVNToolStripMenuItem
            // 
            this.stockCVNToolStripMenuItem.Name = "stockCVNToolStripMenuItem";
            this.stockCVNToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.stockCVNToolStripMenuItem.Text = "Stock CVN";
            this.stockCVNToolStripMenuItem.Click += new System.EventHandler(this.stockCVNToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem2.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // FrmPatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tabFunction);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkAutodetect);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.txtBaseFile);
            this.Controls.Add(this.btnOrgFile);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmPatcher";
            this.Text = "Universal patcher";
            this.Load += new System.EventHandler(this.FrmPatcher_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabInfo.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.tabPatch.ResumeLayout(false);
            this.tabPatch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataPatch)).EndInit();
            this.tabCVN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataCVN)).EndInit();
            this.tabFinfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataFileInfo)).EndInit();
            this.tabFunction.ResumeLayout(false);
            this.tabCreate.ResumeLayout(false);
            this.tabCreate.PerformLayout();
            this.tabFileinfo.ResumeLayout(false);
            this.tabFileinfo.PerformLayout();
            this.tabApply.ResumeLayout(false);
            this.tabExtract.ResumeLayout(false);
            this.tabExtract.PerformLayout();
            this.tabExtractSegments.ResumeLayout(false);
            this.tabExtractSegments.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.Label labelXML;
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
        private System.Windows.Forms.Button btnLoad;
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
        private System.Windows.Forms.Button button1;
    }
}