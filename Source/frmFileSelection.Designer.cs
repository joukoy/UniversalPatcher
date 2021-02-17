using System;

namespace UniversalPatcher
{
    partial class frmFileSelection
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
            this.listFiles = new System.Windows.Forms.ListView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.btnCustomdst = new System.Windows.Forms.Button();
            this.labelCustomdst = new System.Windows.Forms.Label();
            this.chkSubfolders = new System.Windows.Forms.CheckBox();
            this.chkIncludeCustomFileTypes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // listFiles
            // 
            this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listFiles.HideSelection = false;
            this.listFiles.Location = new System.Drawing.Point(3, 83);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(546, 367);
            this.listFiles.TabIndex = 0;
            this.listFiles.UseCompatibleStateImageBehavior = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(447, 50);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 27);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Fix checksums!";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(479, 1);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(57, 24);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(5, 50);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(70, 27);
            this.btnSelectAll.TabIndex = 3;
            this.btnSelectAll.Text = "Select All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolder.Location = new System.Drawing.Point(3, 3);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(470, 20);
            this.txtFolder.TabIndex = 4;
            this.txtFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFolder_KeyDown);
            // 
            // btnCustomdst
            // 
            this.btnCustomdst.Location = new System.Drawing.Point(81, 50);
            this.btnCustomdst.Name = "btnCustomdst";
            this.btnCustomdst.Size = new System.Drawing.Size(117, 27);
            this.btnCustomdst.TabIndex = 5;
            this.btnCustomdst.Text = "Custom destination:";
            this.btnCustomdst.UseVisualStyleBackColor = true;
            this.btnCustomdst.Visible = false;
            this.btnCustomdst.Click += new System.EventHandler(this.btnCustomdst_Click);
            // 
            // labelCustomdst
            // 
            this.labelCustomdst.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCustomdst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelCustomdst.Location = new System.Drawing.Point(204, 54);
            this.labelCustomdst.Name = "labelCustomdst";
            this.labelCustomdst.Size = new System.Drawing.Size(239, 23);
            this.labelCustomdst.TabIndex = 6;
            this.labelCustomdst.Visible = false;
            // 
            // chkSubfolders
            // 
            this.chkSubfolders.AutoSize = true;
            this.chkSubfolders.Location = new System.Drawing.Point(6, 27);
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
            this.chkIncludeCustomFileTypes.Location = new System.Drawing.Point(124, 27);
            this.chkIncludeCustomFileTypes.Name = "chkIncludeCustomFileTypes";
            this.chkIncludeCustomFileTypes.Size = new System.Drawing.Size(142, 17);
            this.chkIncludeCustomFileTypes.TabIndex = 8;
            this.chkIncludeCustomFileTypes.Text = "Include custom file types";
            this.chkIncludeCustomFileTypes.UseVisualStyleBackColor = true;
            this.chkIncludeCustomFileTypes.CheckedChanged += new System.EventHandler(this.chkIncludeCustomFileTypes_CheckedChanged);
            // 
            // frmFileSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 450);
            this.Controls.Add(this.chkIncludeCustomFileTypes);
            this.Controls.Add(this.chkSubfolders);
            this.Controls.Add(this.labelCustomdst);
            this.Controls.Add(this.btnCustomdst);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listFiles);
            this.Name = "frmFileSelection";
            this.Text = "Select files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFileSelection_FormClosing);
            this.Load += new System.EventHandler(this.frmFileSelection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ListView listFiles;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.TextBox txtFolder;
        public System.Windows.Forms.Button btnCustomdst;
        public System.Windows.Forms.Label labelCustomdst;
        public System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkSubfolders;
        private System.Windows.Forms.CheckBox chkIncludeCustomFileTypes;
    }
}