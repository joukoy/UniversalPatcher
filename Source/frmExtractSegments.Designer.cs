using System;

namespace UniversalPatcher
{
    partial class frmExtractSegments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExtractSegments));
            this.listFiles = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnCustomdst = new System.Windows.Forms.Button();
            this.labelCustomdst = new System.Windows.Forms.Label();
            this.chkSubfolders = new System.Windows.Forms.CheckBox();
            this.chkIncludeCustomFileTypes = new System.Windows.Forms.CheckBox();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.groupFileNames = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilenameSuffix = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtFileNamePrefix = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupFileNames.SuspendLayout();
            this.SuspendLayout();
            // 
            // listFiles
            // 
            this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listFiles.HideSelection = false;
            this.listFiles.Location = new System.Drawing.Point(3, 156);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(777, 294);
            this.listFiles.TabIndex = 0;
            this.listFiles.UseCompatibleStateImageBehavior = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(546, 117);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(64, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Extract";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(710, 1);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(64, 24);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(12, 3);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(692, 20);
            this.txtFolder.TabIndex = 4;
            this.txtFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFolder_KeyDown);
            // 
            // btnCustomdst
            // 
            this.btnCustomdst.Location = new System.Drawing.Point(12, 79);
            this.btnCustomdst.Name = "btnCustomdst";
            this.btnCustomdst.Size = new System.Drawing.Size(111, 23);
            this.btnCustomdst.TabIndex = 5;
            this.btnCustomdst.Text = "Custom destination:";
            this.btnCustomdst.UseVisualStyleBackColor = true;
            this.btnCustomdst.Click += new System.EventHandler(this.btnCustomdst_Click);
            // 
            // labelCustomdst
            // 
            this.labelCustomdst.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCustomdst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelCustomdst.Location = new System.Drawing.Point(124, 79);
            this.labelCustomdst.Name = "labelCustomdst";
            this.labelCustomdst.Size = new System.Drawing.Size(580, 23);
            this.labelCustomdst.TabIndex = 6;
            // 
            // chkSubfolders
            // 
            this.chkSubfolders.AutoSize = true;
            this.chkSubfolders.Location = new System.Drawing.Point(12, 26);
            this.chkSubfolders.Name = "chkSubfolders";
            this.chkSubfolders.Size = new System.Drawing.Size(112, 17);
            this.chkSubfolders.TabIndex = 7;
            this.chkSubfolders.Text = "Include subfolders";
            this.chkSubfolders.UseVisualStyleBackColor = true;
            this.chkSubfolders.CheckedChanged += new System.EventHandler(this.chkSubfolders_CheckedChanged);
            // 
            // chkIncludeCustomFileTypes
            // 
            this.chkIncludeCustomFileTypes.AutoSize = true;
            this.chkIncludeCustomFileTypes.Checked = true;
            this.chkIncludeCustomFileTypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIncludeCustomFileTypes.Location = new System.Drawing.Point(130, 26);
            this.chkIncludeCustomFileTypes.Name = "chkIncludeCustomFileTypes";
            this.chkIncludeCustomFileTypes.Size = new System.Drawing.Size(142, 17);
            this.chkIncludeCustomFileTypes.TabIndex = 8;
            this.chkIncludeCustomFileTypes.Text = "Include custom file types";
            this.chkIncludeCustomFileTypes.UseVisualStyleBackColor = true;
            this.chkIncludeCustomFileTypes.CheckedChanged += new System.EventHandler(this.chkIncludeCustomFileTypes_CheckedChanged);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(710, 133);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(69, 17);
            this.chkSelectAll.TabIndex = 9;
            this.chkSelectAll.Text = "Select all";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // groupFileNames
            // 
            this.groupFileNames.Controls.Add(this.label3);
            this.groupFileNames.Controls.Add(this.txtFileNamePrefix);
            this.groupFileNames.Controls.Add(this.label1);
            this.groupFileNames.Controls.Add(this.txtFilenameSuffix);
            this.groupFileNames.Location = new System.Drawing.Point(6, 108);
            this.groupFileNames.Name = "groupFileNames";
            this.groupFileNames.Size = new System.Drawing.Size(305, 42);
            this.groupFileNames.TabIndex = 11;
            this.groupFileNames.TabStop = false;
            this.groupFileNames.Text = "Filenames";
            this.groupFileNames.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "P/N";
            // 
            // txtFilenameSuffix
            // 
            this.txtFilenameSuffix.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilenameSuffix.Location = new System.Drawing.Point(161, 16);
            this.txtFilenameSuffix.Name = "txtFilenameSuffix";
            this.txtFilenameSuffix.Size = new System.Drawing.Size(100, 20);
            this.txtFilenameSuffix.TabIndex = 11;
            this.txtFilenameSuffix.Text = "-$Ver#$cvn";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(78, 49);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(626, 20);
            this.txtDescription.TabIndex = 13;
            // 
            // txtFileNamePrefix
            // 
            this.txtFileNamePrefix.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFileNamePrefix.Location = new System.Drawing.Point(22, 18);
            this.txtFileNamePrefix.Name = "txtFileNamePrefix";
            this.txtFileNamePrefix.Size = new System.Drawing.Size(100, 20);
            this.txtFileNamePrefix.TabIndex = 13;
            this.txtFileNamePrefix.Text = "Custom $Nr-";
            this.txtFileNamePrefix.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = ".bin";
            // 
            // frmExtractSegments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 450);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupFileNames);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.chkIncludeCustomFileTypes);
            this.Controls.Add(this.chkSubfolders);
            this.Controls.Add(this.labelCustomdst);
            this.Controls.Add(this.btnCustomdst);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmExtractSegments";
            this.Text = "Select files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFileSelection_FormClosing);
            this.Load += new System.EventHandler(this.frmFileSelection_Load);
            this.groupFileNames.ResumeLayout(false);
            this.groupFileNames.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ListView listFiles;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFolder;
        public System.Windows.Forms.Button btnCustomdst;
        public System.Windows.Forms.Label labelCustomdst;
        public System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkSubfolders;
        private System.Windows.Forms.CheckBox chkIncludeCustomFileTypes;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.GroupBox groupFileNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtDescription;
        public System.Windows.Forms.TextBox txtFilenameSuffix;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtFileNamePrefix;
    }
}