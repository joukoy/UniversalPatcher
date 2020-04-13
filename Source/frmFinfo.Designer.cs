namespace UniversalPatcher
{
    partial class frmFinfo
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
            this.btnSaveCSV = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.dataFileInfo = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataFileInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSaveCSV
            // 
            this.btnSaveCSV.Location = new System.Drawing.Point(688, 3);
            this.btnSaveCSV.Name = "btnSaveCSV";
            this.btnSaveCSV.Size = new System.Drawing.Size(100, 26);
            this.btnSaveCSV.TabIndex = 7;
            this.btnSaveCSV.Text = "Save CSV";
            this.btnSaveCSV.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(1, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(87, 26);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // dataFileInfo
            // 
            this.dataFileInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataFileInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataFileInfo.Location = new System.Drawing.Point(-4, 30);
            this.dataFileInfo.Name = "dataFileInfo";
            this.dataFileInfo.Size = new System.Drawing.Size(803, 438);
            this.dataFileInfo.TabIndex = 5;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(94, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(87, 26);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // frmFinfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnSaveCSV);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.dataFileInfo);
            this.Name = "frmFinfo";
            this.Text = "File info";
            this.Load += new System.EventHandler(this.frmFinfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataFileInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSaveCSV;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView dataFileInfo;
        private System.Windows.Forms.Button btnRefresh;
    }
}