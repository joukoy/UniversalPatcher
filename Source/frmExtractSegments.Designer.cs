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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioRename = new System.Windows.Forms.RadioButton();
            this.radioReplace = new System.Windows.Forms.RadioButton();
            this.radioSkip = new System.Windows.Forms.RadioButton();
            this.btnSelectfiles = new System.Windows.Forms.Button();
            this.labelCustomFolder = new System.Windows.Forms.Label();
            this.btnCustomFolder = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioRename);
            this.groupBox2.Controls.Add(this.radioReplace);
            this.groupBox2.Controls.Add(this.radioSkip);
            this.groupBox2.Location = new System.Drawing.Point(4, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 42);
            this.groupBox2.TabIndex = 508;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Duplicates";
            // 
            // radioRename
            // 
            this.radioRename.AutoSize = true;
            this.radioRename.Location = new System.Drawing.Point(143, 19);
            this.radioRename.Name = "radioRename";
            this.radioRename.Size = new System.Drawing.Size(65, 17);
            this.radioRename.TabIndex = 504;
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
            this.radioSkip.Checked = true;
            this.radioSkip.Location = new System.Drawing.Point(7, 19);
            this.radioSkip.Name = "radioSkip";
            this.radioSkip.Size = new System.Drawing.Size(46, 17);
            this.radioSkip.TabIndex = 502;
            this.radioSkip.TabStop = true;
            this.radioSkip.Text = "Skip";
            this.radioSkip.UseVisualStyleBackColor = true;
            // 
            // btnSelectfiles
            // 
            this.btnSelectfiles.Location = new System.Drawing.Point(252, 23);
            this.btnSelectfiles.Name = "btnSelectfiles";
            this.btnSelectfiles.Size = new System.Drawing.Size(91, 30);
            this.btnSelectfiles.TabIndex = 513;
            this.btnSelectfiles.Text = "Select file(s)...";
            this.btnSelectfiles.UseVisualStyleBackColor = true;
            this.btnSelectfiles.Click += new System.EventHandler(this.btnSelectfiles_Click);
            // 
            // labelCustomFolder
            // 
            this.labelCustomFolder.AutoSize = true;
            this.labelCustomFolder.Location = new System.Drawing.Point(8, 87);
            this.labelCustomFolder.Name = "labelCustomFolder";
            this.labelCustomFolder.Size = new System.Drawing.Size(10, 13);
            this.labelCustomFolder.TabIndex = 514;
            this.labelCustomFolder.Text = "-";
            // 
            // btnCustomFolder
            // 
            this.btnCustomFolder.Location = new System.Drawing.Point(9, 61);
            this.btnCustomFolder.Name = "btnCustomFolder";
            this.btnCustomFolder.Size = new System.Drawing.Size(104, 23);
            this.btnCustomFolder.TabIndex = 515;
            this.btnCustomFolder.Text = "Custom folder";
            this.btnCustomFolder.UseVisualStyleBackColor = true;
            this.btnCustomFolder.Click += new System.EventHandler(this.btnCustomFolder_Click);
            // 
            // frmExtractSegments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 111);
            this.Controls.Add(this.btnCustomFolder);
            this.Controls.Add(this.labelCustomFolder);
            this.Controls.Add(this.btnSelectfiles);
            this.Controls.Add(this.groupBox2);
            this.Name = "frmExtractSegments";
            this.Text = "Extract segments";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioRename;
        private System.Windows.Forms.RadioButton radioReplace;
        private System.Windows.Forms.RadioButton radioSkip;
        private System.Windows.Forms.Button btnSelectfiles;
        private System.Windows.Forms.Label labelCustomFolder;
        private System.Windows.Forms.Button btnCustomFolder;
    }
}