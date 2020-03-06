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
            this.btnSave = new System.Windows.Forms.Button();
            this.txtPatchName = new System.Windows.Forms.TextBox();
            this.labelBinSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDescr = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioApply = new System.Windows.Forms.RadioButton();
            this.radioCreate = new System.Windows.Forms.RadioButton();
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
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOrgFile
            // 
            this.btnOrgFile.Location = new System.Drawing.Point(12, 31);
            this.btnOrgFile.Name = "btnOrgFile";
            this.btnOrgFile.Size = new System.Drawing.Size(78, 25);
            this.btnOrgFile.TabIndex = 1;
            this.btnOrgFile.Text = "Original file";
            this.btnOrgFile.UseVisualStyleBackColor = true;
            this.btnOrgFile.Click += new System.EventHandler(this.btnOrgFile_Click);
            // 
            // btnModFile
            // 
            this.btnModFile.Location = new System.Drawing.Point(12, 62);
            this.btnModFile.Name = "btnModFile";
            this.btnModFile.Size = new System.Drawing.Size(78, 25);
            this.btnModFile.TabIndex = 2;
            this.btnModFile.Text = "Modified file";
            this.btnModFile.UseVisualStyleBackColor = true;
            this.btnModFile.Click += new System.EventHandler(this.btnModFile_Click);
            // 
            // txtBaseFile
            // 
            this.txtBaseFile.Location = new System.Drawing.Point(96, 34);
            this.txtBaseFile.Name = "txtBaseFile";
            this.txtBaseFile.Size = new System.Drawing.Size(619, 20);
            this.txtBaseFile.TabIndex = 3;
            // 
            // txtModifierFile
            // 
            this.txtModifierFile.Location = new System.Drawing.Point(96, 65);
            this.txtModifierFile.Name = "txtModifierFile";
            this.txtModifierFile.Size = new System.Drawing.Size(617, 20);
            this.txtModifierFile.TabIndex = 4;
            this.txtModifierFile.TextChanged += new System.EventHandler(this.txtModifierFile_TextChanged);
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(12, 117);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(78, 25);
            this.btnCompare.TabIndex = 5;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // txtResult
            // 
            this.txtResult.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(12, 148);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(705, 234);
            this.txtResult.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(640, 420);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(72, 35);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save patch";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtPatchName
            // 
            this.txtPatchName.Enabled = false;
            this.txtPatchName.Location = new System.Drawing.Point(107, 391);
            this.txtPatchName.Name = "txtPatchName";
            this.txtPatchName.Size = new System.Drawing.Size(606, 20);
            this.txtPatchName.TabIndex = 8;
            // 
            // labelBinSize
            // 
            this.labelBinSize.AutoSize = true;
            this.labelBinSize.Location = new System.Drawing.Point(150, 123);
            this.labelBinSize.Name = "labelBinSize";
            this.labelBinSize.Size = new System.Drawing.Size(10, 13);
            this.labelBinSize.TabIndex = 9;
            this.labelBinSize.Text = "-";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "BIN Size:";
            // 
            // labelDescr
            // 
            this.labelDescr.AutoSize = true;
            this.labelDescr.Location = new System.Drawing.Point(9, 391);
            this.labelDescr.Name = "labelDescr";
            this.labelDescr.Size = new System.Drawing.Size(92, 13);
            this.labelDescr.TabIndex = 11;
            this.labelDescr.Text = "Patch description:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioApply);
            this.groupBox1.Controls.Add(this.radioCreate);
            this.groupBox1.Location = new System.Drawing.Point(14, -10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 38);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // radioApply
            // 
            this.radioApply.AutoSize = true;
            this.radioApply.Location = new System.Drawing.Point(120, 16);
            this.radioApply.Name = "radioApply";
            this.radioApply.Size = new System.Drawing.Size(82, 17);
            this.radioApply.TabIndex = 1;
            this.radioApply.TabStop = true;
            this.radioApply.Text = "Apply Patch";
            this.radioApply.UseVisualStyleBackColor = true;
            this.radioApply.CheckedChanged += new System.EventHandler(this.radioApply_CheckedChanged);
            // 
            // radioCreate
            // 
            this.radioCreate.AutoSize = true;
            this.radioCreate.Checked = true;
            this.radioCreate.Location = new System.Drawing.Point(12, 16);
            this.radioCreate.Name = "radioCreate";
            this.radioCreate.Size = new System.Drawing.Size(87, 17);
            this.radioCreate.TabIndex = 0;
            this.radioCreate.TabStop = true;
            this.radioCreate.Text = "Create Patch";
            this.radioCreate.UseVisualStyleBackColor = true;
            this.radioCreate.CheckedChanged += new System.EventHandler(this.radioCreate_CheckedChanged);
            // 
            // btnSegments
            // 
            this.btnSegments.Location = new System.Drawing.Point(406, 420);
            this.btnSegments.Name = "btnSegments";
            this.btnSegments.Size = new System.Drawing.Size(72, 35);
            this.btnSegments.TabIndex = 14;
            this.btnSegments.Text = "Setup segments";
            this.btnSegments.UseVisualStyleBackColor = true;
            this.btnSegments.Click += new System.EventHandler(this.btnSegments_Click);
            // 
            // btnCheckSums
            // 
            this.btnCheckSums.Location = new System.Drawing.Point(484, 420);
            this.btnCheckSums.Name = "btnCheckSums";
            this.btnCheckSums.Size = new System.Drawing.Size(72, 35);
            this.btnCheckSums.TabIndex = 15;
            this.btnCheckSums.Text = "Fix checksums";
            this.btnCheckSums.UseVisualStyleBackColor = true;
            this.btnCheckSums.Click += new System.EventHandler(this.btnCheckSums_Click);
            // 
            // labelXML
            // 
            this.labelXML.AutoSize = true;
            this.labelXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXML.Location = new System.Drawing.Point(228, 6);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(13, 16);
            this.labelXML.TabIndex = 16;
            this.labelXML.Text = "-";
            // 
            // btnShowPatch
            // 
            this.btnShowPatch.Enabled = false;
            this.btnShowPatch.Location = new System.Drawing.Point(562, 420);
            this.btnShowPatch.Name = "btnShowPatch";
            this.btnShowPatch.Size = new System.Drawing.Size(72, 35);
            this.btnShowPatch.TabIndex = 17;
            this.btnShowPatch.Text = "Show patch";
            this.btnShowPatch.UseVisualStyleBackColor = true;
            this.btnShowPatch.Click += new System.EventHandler(this.btnShowPatch_Click);
            // 
            // numSuppress
            // 
            this.numSuppress.Location = new System.Drawing.Point(640, 123);
            this.numSuppress.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSuppress.Name = "numSuppress";
            this.numSuppress.Size = new System.Drawing.Size(42, 20);
            this.numSuppress.TabIndex = 18;
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
            this.label2.Location = new System.Drawing.Point(523, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Suppress display after:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(688, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "rows";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(328, 420);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(72, 35);
            this.btnLoad.TabIndex = 21;
            this.btnLoad.Text = "Load config";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // chkCompareAll
            // 
            this.chkCompareAll.AutoSize = true;
            this.chkCompareAll.Location = new System.Drawing.Point(248, 125);
            this.chkCompareAll.Name = "chkCompareAll";
            this.chkCompareAll.Size = new System.Drawing.Size(167, 17);
            this.chkCompareAll.TabIndex = 22;
            this.chkCompareAll.Text = "Compare all (ignore segments)";
            this.chkCompareAll.UseVisualStyleBackColor = true;
            // 
            // chkAutodetect
            // 
            this.chkAutodetect.AutoSize = true;
            this.chkAutodetect.Checked = true;
            this.chkAutodetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutodetect.Location = new System.Drawing.Point(12, 427);
            this.chkAutodetect.Name = "chkAutodetect";
            this.chkAutodetect.Size = new System.Drawing.Size(110, 17);
            this.chkAutodetect.TabIndex = 23;
            this.chkAutodetect.Text = "Autodetect config";
            this.chkAutodetect.UseVisualStyleBackColor = true;
            // 
            // btnAutodetect
            // 
            this.btnAutodetect.Location = new System.Drawing.Point(250, 420);
            this.btnAutodetect.Name = "btnAutodetect";
            this.btnAutodetect.Size = new System.Drawing.Size(72, 35);
            this.btnAutodetect.TabIndex = 24;
            this.btnAutodetect.Text = "Setup autodetect";
            this.btnAutodetect.UseVisualStyleBackColor = true;
            this.btnAutodetect.Click += new System.EventHandler(this.btnAutodetect_Click);
            // 
            // FrmPatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 458);
            this.Controls.Add(this.btnAutodetect);
            this.Controls.Add(this.chkAutodetect);
            this.Controls.Add(this.chkCompareAll);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSuppress);
            this.Controls.Add(this.btnShowPatch);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.btnCheckSums);
            this.Controls.Add(this.btnSegments);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelDescr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelBinSize);
            this.Controls.Add(this.txtPatchName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.txtModifierFile);
            this.Controls.Add(this.txtBaseFile);
            this.Controls.Add(this.btnModFile);
            this.Controls.Add(this.btnOrgFile);
            this.Name = "FrmPatcher";
            this.Text = "Create patch";
            this.Load += new System.EventHandler(this.FrmPatcher_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtBaseFile;
        private System.Windows.Forms.TextBox txtModifierFile;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtPatchName;
        private System.Windows.Forms.Label labelBinSize;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnOrgFile;
        public System.Windows.Forms.Button btnModFile;
        private System.Windows.Forms.Label labelDescr;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioApply;
        private System.Windows.Forms.RadioButton radioCreate;
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
    }
}