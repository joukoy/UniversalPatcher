namespace UniversalPatcher
{
    partial class frmApplyPatch
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
            this.labelPatchfile = new System.Windows.Forms.Label();
            this.labelBinFile = new System.Windows.Forms.Label();
            this.btnPatchFile = new System.Windows.Forms.Button();
            this.btnBinFile = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnSaveBIN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelPatchfile
            // 
            this.labelPatchfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPatchfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelPatchfile.Location = new System.Drawing.Point(93, 40);
            this.labelPatchfile.Name = "labelPatchfile";
            this.labelPatchfile.Size = new System.Drawing.Size(494, 23);
            this.labelPatchfile.TabIndex = 7;
            this.labelPatchfile.Text = "-";
            // 
            // labelBinFile
            // 
            this.labelBinFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBinFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelBinFile.Location = new System.Drawing.Point(93, 9);
            this.labelBinFile.Name = "labelBinFile";
            this.labelBinFile.Size = new System.Drawing.Size(494, 23);
            this.labelBinFile.TabIndex = 6;
            this.labelBinFile.Text = "-";
            // 
            // btnPatchFile
            // 
            this.btnPatchFile.Location = new System.Drawing.Point(6, 37);
            this.btnPatchFile.Name = "btnPatchFile";
            this.btnPatchFile.Size = new System.Drawing.Size(76, 26);
            this.btnPatchFile.TabIndex = 5;
            this.btnPatchFile.Text = "Patch file";
            this.btnPatchFile.UseVisualStyleBackColor = true;
            this.btnPatchFile.Click += new System.EventHandler(this.btnPatchFile_Click);
            // 
            // btnBinFile
            // 
            this.btnBinFile.Location = new System.Drawing.Point(6, 5);
            this.btnBinFile.Name = "btnBinFile";
            this.btnBinFile.Size = new System.Drawing.Size(76, 26);
            this.btnBinFile.TabIndex = 4;
            this.btnBinFile.Text = "BIN File";
            this.btnBinFile.UseVisualStyleBackColor = true;
            this.btnBinFile.Click += new System.EventHandler(this.btnBinFile_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(6, 69);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(76, 26);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnSaveBIN
            // 
            this.btnSaveBIN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveBIN.Location = new System.Drawing.Point(513, 73);
            this.btnSaveBIN.Name = "btnSaveBIN";
            this.btnSaveBIN.Size = new System.Drawing.Size(76, 26);
            this.btnSaveBIN.TabIndex = 9;
            this.btnSaveBIN.Text = "Save BIN";
            this.btnSaveBIN.UseVisualStyleBackColor = true;
            this.btnSaveBIN.Click += new System.EventHandler(this.btnSaveBIN_Click);
            // 
            // frmApplyPatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 104);
            this.Controls.Add(this.btnSaveBIN);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.labelPatchfile);
            this.Controls.Add(this.labelBinFile);
            this.Controls.Add(this.btnPatchFile);
            this.Controls.Add(this.btnBinFile);
            this.Name = "frmApplyPatch";
            this.Text = "Apply patch";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelPatchfile;
        private System.Windows.Forms.Label labelBinFile;
        private System.Windows.Forms.Button btnPatchFile;
        private System.Windows.Forms.Button btnBinFile;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnSaveBIN;
    }
}