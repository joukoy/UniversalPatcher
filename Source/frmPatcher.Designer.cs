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
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnSaveBin = new System.Windows.Forms.Button();
            this.txtPatchDescription = new System.Windows.Forms.TextBox();
            this.labelBinSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDescr = new System.Windows.Forms.Label();
            this.btnSegments = new System.Windows.Forms.Button();
            this.btnCheckSums = new System.Windows.Forms.Button();
            this.labelXML = new System.Windows.Forms.Label();
            this.btnShowPatch = new System.Windows.Forms.Button();
            this.numSuppress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.chkCompareAll = new System.Windows.Forms.CheckBox();
            this.chkAutodetect = new System.Windows.Forms.CheckBox();
            this.btnAutodetect = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.chkExtra = new System.Windows.Forms.CheckBox();
            this.chkCS2 = new System.Windows.Forms.CheckBox();
            this.chkCS1 = new System.Windows.Forms.CheckBox();
            this.chkSize = new System.Windows.Forms.CheckBox();
            this.chkRange = new System.Windows.Forms.CheckBox();
            this.btnSaveFileInfo = new System.Windows.Forms.Button();
            this.btnLoadFolder = new System.Windows.Forms.Button();
            this.tabFunction = new System.Windows.Forms.TabControl();
            this.tabCreate = new System.Windows.Forms.TabPage();
            this.txtOS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSavePatch = new System.Windows.Forms.Button();
            this.btnShowNewPatch = new System.Windows.Forms.Button();
            this.tabApply = new System.Windows.Forms.TabPage();
            this.btnPatchfile = new System.Windows.Forms.Button();
            this.txtPatchfile = new System.Windows.Forms.TextBox();
            this.btnApplypatch = new System.Windows.Forms.Button();
            this.tabExtract = new System.Windows.Forms.TabPage();
            this.txtExtractDescription = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.txtCompatibleOS = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExtractRange = new System.Windows.Forms.TextBox();
            this.tabFileinfo = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.tabDebug.SuspendLayout();
            this.tabFunction.SuspendLayout();
            this.tabCreate.SuspendLayout();
            this.tabApply.SuspendLayout();
            this.tabExtract.SuspendLayout();
            this.tabFileinfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOrgFile
            // 
            this.btnOrgFile.Location = new System.Drawing.Point(12, 31);
            this.btnOrgFile.Name = "btnOrgFile";
            this.btnOrgFile.Size = new System.Drawing.Size(78, 25);
            this.btnOrgFile.TabIndex = 12;
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
            this.txtBaseFile.Size = new System.Drawing.Size(617, 20);
            this.txtBaseFile.TabIndex = 13;
            // 
            // txtModifierFile
            // 
            this.txtModifierFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModifierFile.Location = new System.Drawing.Point(90, 6);
            this.txtModifierFile.Name = "txtModifierFile";
            this.txtModifierFile.Size = new System.Drawing.Size(617, 20);
            this.txtModifierFile.TabIndex = 111;
            this.txtModifierFile.TextChanged += new System.EventHandler(this.txtModifierFile_TextChanged);
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
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(3, 3);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(702, 316);
            this.txtResult.TabIndex = 6;
            // 
            // btnSaveBin
            // 
            this.btnSaveBin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveBin.Location = new System.Drawing.Point(628, 31);
            this.btnSaveBin.Name = "btnSaveBin";
            this.btnSaveBin.Size = new System.Drawing.Size(78, 25);
            this.btnSaveBin.TabIndex = 135;
            this.btnSaveBin.Text = "Save bin";
            this.btnSaveBin.UseVisualStyleBackColor = true;
            this.btnSaveBin.Click += new System.EventHandler(this.btnSaveBin_Click);
            // 
            // txtPatchDescription
            // 
            this.txtPatchDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatchDescription.Location = new System.Drawing.Point(129, 57);
            this.txtPatchDescription.Name = "txtPatchDescription";
            this.txtPatchDescription.Size = new System.Drawing.Size(478, 20);
            this.txtPatchDescription.TabIndex = 116;
            // 
            // labelBinSize
            // 
            this.labelBinSize.AutoSize = true;
            this.labelBinSize.Location = new System.Drawing.Point(361, 195);
            this.labelBinSize.Name = "labelBinSize";
            this.labelBinSize.Size = new System.Drawing.Size(10, 13);
            this.labelBinSize.TabIndex = 9;
            this.labelBinSize.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 195);
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
            // btnSegments
            // 
            this.btnSegments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSegments.Location = new System.Drawing.Point(514, 3);
            this.btnSegments.Name = "btnSegments";
            this.btnSegments.Size = new System.Drawing.Size(91, 25);
            this.btnSegments.TabIndex = 210;
            this.btnSegments.Text = "Setup segments";
            this.btnSegments.UseVisualStyleBackColor = true;
            this.btnSegments.Click += new System.EventHandler(this.btnSegments_Click);
            // 
            // btnCheckSums
            // 
            this.btnCheckSums.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckSums.Location = new System.Drawing.Point(526, 31);
            this.btnCheckSums.Name = "btnCheckSums";
            this.btnCheckSums.Size = new System.Drawing.Size(96, 25);
            this.btnCheckSums.TabIndex = 134;
            this.btnCheckSums.Text = "Fix checksums";
            this.btnCheckSums.UseVisualStyleBackColor = true;
            this.btnCheckSums.Click += new System.EventHandler(this.btnCheckSums_Click);
            // 
            // labelXML
            // 
            this.labelXML.AutoSize = true;
            this.labelXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXML.Location = new System.Drawing.Point(228, 9);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(13, 16);
            this.labelXML.TabIndex = 16;
            this.labelXML.Text = "-";
            // 
            // btnShowPatch
            // 
            this.btnShowPatch.Location = new System.Drawing.Point(90, 31);
            this.btnShowPatch.Name = "btnShowPatch";
            this.btnShowPatch.Size = new System.Drawing.Size(78, 25);
            this.btnShowPatch.TabIndex = 133;
            this.btnShowPatch.Text = "Show patch";
            this.btnShowPatch.UseVisualStyleBackColor = true;
            this.btnShowPatch.Click += new System.EventHandler(this.btnShowPatch_Click);
            // 
            // numSuppress
            // 
            this.numSuppress.Location = new System.Drawing.Point(122, 191);
            this.numSuppress.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSuppress.Name = "numSuppress";
            this.numSuppress.Size = new System.Drawing.Size(42, 20);
            this.numSuppress.TabIndex = 190;
            this.numSuppress.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSuppress.ValueChanged += new System.EventHandler(this.numSuppress_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Suppress display after:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(170, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "rows";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 5);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(78, 25);
            this.btnLoad.TabIndex = 10;
            this.btnLoad.Text = "Load config";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
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
            this.chkAutodetect.Location = new System.Drawing.Point(96, 10);
            this.chkAutodetect.Name = "chkAutodetect";
            this.chkAutodetect.Size = new System.Drawing.Size(110, 17);
            this.chkAutodetect.TabIndex = 11;
            this.chkAutodetect.Text = "Autodetect config";
            this.chkAutodetect.UseVisualStyleBackColor = true;
            // 
            // btnAutodetect
            // 
            this.btnAutodetect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAutodetect.Location = new System.Drawing.Point(611, 3);
            this.btnAutodetect.Name = "btnAutodetect";
            this.btnAutodetect.Size = new System.Drawing.Size(102, 25);
            this.btnAutodetect.TabIndex = 211;
            this.btnAutodetect.Text = "Setup autodetect";
            this.btnAutodetect.UseVisualStyleBackColor = true;
            this.btnAutodetect.Click += new System.EventHandler(this.btnAutodetect_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabInfo);
            this.tabControl1.Controls.Add(this.tabDebug);
            this.tabControl1.Location = new System.Drawing.Point(0, 213);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(716, 348);
            this.tabControl1.TabIndex = 200;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.txtResult);
            this.tabInfo.Location = new System.Drawing.Point(4, 22);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tabInfo.Size = new System.Drawing.Size(708, 322);
            this.tabInfo.TabIndex = 0;
            this.tabInfo.Text = "Info";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.txtDebug);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(708, 322);
            this.tabDebug.TabIndex = 1;
            this.tabDebug.Text = "Debug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // txtDebug
            // 
            this.txtDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDebug.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDebug.Location = new System.Drawing.Point(3, 3);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDebug.Size = new System.Drawing.Size(702, 316);
            this.txtDebug.TabIndex = 0;
            // 
            // chkExtra
            // 
            this.chkExtra.AutoSize = true;
            this.chkExtra.Checked = true;
            this.chkExtra.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExtra.Location = new System.Drawing.Point(550, 10);
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
            this.chkCS2.Location = new System.Drawing.Point(462, 10);
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
            this.chkCS1.Location = new System.Drawing.Point(371, 10);
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
            this.chkSize.Location = new System.Drawing.Point(274, 10);
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
            this.chkRange.Location = new System.Drawing.Point(171, 10);
            this.chkRange.Name = "chkRange";
            this.chkRange.Size = new System.Drawing.Size(98, 17);
            this.chkRange.TabIndex = 172;
            this.chkRange.Text = "Segment range";
            this.chkRange.UseVisualStyleBackColor = true;
            // 
            // btnSaveFileInfo
            // 
            this.btnSaveFileInfo.Location = new System.Drawing.Point(94, 3);
            this.btnSaveFileInfo.Name = "btnSaveFileInfo";
            this.btnSaveFileInfo.Size = new System.Drawing.Size(71, 28);
            this.btnSaveFileInfo.TabIndex = 171;
            this.btnSaveFileInfo.Text = "Save As...";
            this.btnSaveFileInfo.UseVisualStyleBackColor = true;
            this.btnSaveFileInfo.Click += new System.EventHandler(this.btnSaveFileInfo_Click);
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
            this.tabFunction.Controls.Add(this.tabApply);
            this.tabFunction.Controls.Add(this.tabExtract);
            this.tabFunction.Controls.Add(this.tabFileinfo);
            this.tabFunction.Location = new System.Drawing.Point(2, 60);
            this.tabFunction.Name = "tabFunction";
            this.tabFunction.SelectedIndex = 0;
            this.tabFunction.Size = new System.Drawing.Size(719, 129);
            this.tabFunction.TabIndex = 100;
            // 
            // tabCreate
            // 
            this.tabCreate.Controls.Add(this.txtOS);
            this.tabCreate.Controls.Add(this.label7);
            this.tabCreate.Controls.Add(this.btnSavePatch);
            this.tabCreate.Controls.Add(this.btnShowNewPatch);
            this.tabCreate.Controls.Add(this.btnModFile);
            this.tabCreate.Controls.Add(this.txtModifierFile);
            this.tabCreate.Controls.Add(this.btnCompare);
            this.tabCreate.Controls.Add(this.chkCompareAll);
            this.tabCreate.Controls.Add(this.labelDescr);
            this.tabCreate.Controls.Add(this.txtPatchDescription);
            this.tabCreate.Location = new System.Drawing.Point(4, 22);
            this.tabCreate.Name = "tabCreate";
            this.tabCreate.Padding = new System.Windows.Forms.Padding(3);
            this.tabCreate.Size = new System.Drawing.Size(711, 103);
            this.tabCreate.TabIndex = 0;
            this.tabCreate.Text = "Create Patch";
            this.tabCreate.UseVisualStyleBackColor = true;
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
            // btnSavePatch
            // 
            this.btnSavePatch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSavePatch.Location = new System.Drawing.Point(617, 54);
            this.btnSavePatch.Name = "btnSavePatch";
            this.btnSavePatch.Size = new System.Drawing.Size(90, 25);
            this.btnSavePatch.TabIndex = 117;
            this.btnSavePatch.Text = "Save Patch";
            this.btnSavePatch.UseVisualStyleBackColor = true;
            this.btnSavePatch.Click += new System.EventHandler(this.btnSavePatch_Click);
            // 
            // btnShowNewPatch
            // 
            this.btnShowNewPatch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnShowNewPatch.Location = new System.Drawing.Point(617, 30);
            this.btnShowNewPatch.Name = "btnShowNewPatch";
            this.btnShowNewPatch.Size = new System.Drawing.Size(90, 22);
            this.btnShowNewPatch.TabIndex = 115;
            this.btnShowNewPatch.Text = "Show Patch";
            this.btnShowNewPatch.UseVisualStyleBackColor = true;
            this.btnShowNewPatch.Click += new System.EventHandler(this.btnShowNewPatch_Click);
            // 
            // tabApply
            // 
            this.tabApply.Controls.Add(this.btnPatchfile);
            this.tabApply.Controls.Add(this.txtPatchfile);
            this.tabApply.Controls.Add(this.btnApplypatch);
            this.tabApply.Controls.Add(this.btnShowPatch);
            this.tabApply.Controls.Add(this.btnSaveBin);
            this.tabApply.Controls.Add(this.btnCheckSums);
            this.tabApply.Location = new System.Drawing.Point(4, 22);
            this.tabApply.Name = "tabApply";
            this.tabApply.Padding = new System.Windows.Forms.Padding(3);
            this.tabApply.Size = new System.Drawing.Size(711, 103);
            this.tabApply.TabIndex = 1;
            this.tabApply.Text = "Apply Patch";
            this.tabApply.UseVisualStyleBackColor = true;
            // 
            // btnPatchfile
            // 
            this.btnPatchfile.Location = new System.Drawing.Point(5, 5);
            this.btnPatchfile.Name = "btnPatchfile";
            this.btnPatchfile.Size = new System.Drawing.Size(78, 25);
            this.btnPatchfile.TabIndex = 130;
            this.btnPatchfile.Text = "Patch File";
            this.btnPatchfile.UseVisualStyleBackColor = true;
            this.btnPatchfile.Click += new System.EventHandler(this.btnPatchfile_Click);
            // 
            // txtPatchfile
            // 
            this.txtPatchfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPatchfile.Location = new System.Drawing.Point(89, 8);
            this.txtPatchfile.Name = "txtPatchfile";
            this.txtPatchfile.Size = new System.Drawing.Size(617, 20);
            this.txtPatchfile.TabIndex = 131;
            // 
            // btnApplypatch
            // 
            this.btnApplypatch.Enabled = false;
            this.btnApplypatch.Location = new System.Drawing.Point(5, 31);
            this.btnApplypatch.Name = "btnApplypatch";
            this.btnApplypatch.Size = new System.Drawing.Size(78, 25);
            this.btnApplypatch.TabIndex = 132;
            this.btnApplypatch.Text = "Apply";
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
            this.tabExtract.Size = new System.Drawing.Size(711, 103);
            this.tabExtract.TabIndex = 3;
            this.tabExtract.Text = "Extract table";
            this.tabExtract.UseVisualStyleBackColor = true;
            // 
            // txtExtractDescription
            // 
            this.txtExtractDescription.Location = new System.Drawing.Point(124, 58);
            this.txtExtractDescription.Name = "txtExtractDescription";
            this.txtExtractDescription.Size = new System.Drawing.Size(225, 20);
            this.txtExtractDescription.TabIndex = 152;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 61);
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
            this.btnExtract.TabIndex = 153;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // txtCompatibleOS
            // 
            this.txtCompatibleOS.Location = new System.Drawing.Point(124, 32);
            this.txtCompatibleOS.Name = "txtCompatibleOS";
            this.txtCompatibleOS.Size = new System.Drawing.Size(226, 20);
            this.txtCompatibleOS.TabIndex = 151;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 35);
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
            this.txtExtractRange.LostFocus += new System.EventHandler(this.txtExtractRange_LostFocus);
            // 
            // tabFileinfo
            // 
            this.tabFileinfo.Controls.Add(this.chkExtra);
            this.tabFileinfo.Controls.Add(this.btnLoadFolder);
            this.tabFileinfo.Controls.Add(this.chkCS2);
            this.tabFileinfo.Controls.Add(this.btnSaveFileInfo);
            this.tabFileinfo.Controls.Add(this.chkCS1);
            this.tabFileinfo.Controls.Add(this.chkRange);
            this.tabFileinfo.Controls.Add(this.chkSize);
            this.tabFileinfo.Location = new System.Drawing.Point(4, 22);
            this.tabFileinfo.Name = "tabFileinfo";
            this.tabFileinfo.Size = new System.Drawing.Size(711, 103);
            this.tabFileinfo.TabIndex = 2;
            this.tabFileinfo.Text = "File info";
            this.tabFileinfo.UseVisualStyleBackColor = true;
            // 
            // FrmPatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 561);
            this.Controls.Add(this.tabFunction);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnAutodetect);
            this.Controls.Add(this.chkAutodetect);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSuppress);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.btnSegments);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelBinSize);
            this.Controls.Add(this.txtBaseFile);
            this.Controls.Add(this.btnOrgFile);
            this.Name = "FrmPatcher";
            this.Text = "Universal patcher";
            this.Load += new System.EventHandler(this.FrmPatcher_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabInfo.PerformLayout();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.tabFunction.ResumeLayout(false);
            this.tabCreate.ResumeLayout(false);
            this.tabCreate.PerformLayout();
            this.tabApply.ResumeLayout(false);
            this.tabApply.PerformLayout();
            this.tabExtract.ResumeLayout(false);
            this.tabExtract.PerformLayout();
            this.tabFileinfo.ResumeLayout(false);
            this.tabFileinfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBaseFile;
        private System.Windows.Forms.TextBox txtModifierFile;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnSaveBin;
        private System.Windows.Forms.TextBox txtPatchDescription;
        private System.Windows.Forms.Label labelBinSize;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnOrgFile;
        public System.Windows.Forms.Button btnModFile;
        private System.Windows.Forms.Label labelDescr;
        private System.Windows.Forms.Button btnSegments;
        private System.Windows.Forms.Button btnCheckSums;
        private System.Windows.Forms.Label labelXML;
        private System.Windows.Forms.Button btnShowPatch;
        private System.Windows.Forms.NumericUpDown numSuppress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.CheckBox chkCompareAll;
        private System.Windows.Forms.CheckBox chkAutodetect;
        private System.Windows.Forms.Button btnAutodetect;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.TabPage tabDebug;
        private System.Windows.Forms.TextBox txtDebug;
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
        public System.Windows.Forms.Button btnPatchfile;
        private System.Windows.Forms.TextBox txtPatchfile;
        private System.Windows.Forms.Button btnApplypatch;
        private System.Windows.Forms.TabPage tabFileinfo;
        private System.Windows.Forms.Button btnShowNewPatch;
        private System.Windows.Forms.Button btnSavePatch;
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
    }
}