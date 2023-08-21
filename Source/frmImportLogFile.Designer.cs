
namespace UniversalPatcher
{
    partial class frmImportLogFile
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
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.dataGridTimeStamps = new System.Windows.Forms.DataGridView();
            this.btnOK = new System.Windows.Forms.Button();
            this.labelTstampString = new System.Windows.Forms.Label();
            this.chkElapsed = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTimeStamps)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(6, 6);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(58, 25);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "File:";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Location = new System.Drawing.Point(70, 9);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(369, 20);
            this.txtFileName.TabIndex = 2;
            this.txtFileName.TextChanged += new System.EventHandler(this.txtFileName_TextChanged);
            // 
            // dataGridTimeStamps
            // 
            this.dataGridTimeStamps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridTimeStamps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridTimeStamps.Location = new System.Drawing.Point(6, 72);
            this.dataGridTimeStamps.Name = "dataGridTimeStamps";
            this.dataGridTimeStamps.Size = new System.Drawing.Size(425, 333);
            this.dataGridTimeStamps.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(366, 413);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 25);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Apply";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labelTstampString
            // 
            this.labelTstampString.AutoSize = true;
            this.labelTstampString.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelTstampString.Location = new System.Drawing.Point(6, 45);
            this.labelTstampString.Name = "labelTstampString";
            this.labelTstampString.Size = new System.Drawing.Size(63, 15);
            this.labelTstampString.TabIndex = 4;
            this.labelTstampString.Text = "Timestamp:";
            // 
            // chkElapsed
            // 
            this.chkElapsed.AutoSize = true;
            this.chkElapsed.Location = new System.Drawing.Point(200, 44);
            this.chkElapsed.Name = "chkElapsed";
            this.chkElapsed.Size = new System.Drawing.Size(86, 17);
            this.chkElapsed.TabIndex = 5;
            this.chkElapsed.Text = "Elapsed time";
            this.chkElapsed.UseVisualStyleBackColor = true;
            // 
            // frmImportLogFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 450);
            this.Controls.Add(this.chkElapsed);
            this.Controls.Add(this.labelTstampString);
            this.Controls.Add(this.dataGridTimeStamps);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.btnSelectFile);
            this.Name = "frmImportLogFile";
            this.Text = "Import logfile";
            this.Load += new System.EventHandler(this.frmImportLogFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTimeStamps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.DataGridView dataGridTimeStamps;
        public System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelTstampString;
        private System.Windows.Forms.CheckBox chkElapsed;
    }
}