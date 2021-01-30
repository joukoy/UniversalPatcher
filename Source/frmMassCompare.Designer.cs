namespace UniversalPatcher
{
    partial class frmMassCompare
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
            this.btnSelectFiles = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnSaveCsv = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectFiles
            // 
            this.btnSelectFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFiles.Location = new System.Drawing.Point(713, 417);
            this.btnSelectFiles.Name = "btnSelectFiles";
            this.btnSelectFiles.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFiles.TabIndex = 1;
            this.btnSelectFiles.Text = "Select files";
            this.btnSelectFiles.UseVisualStyleBackColor = true;
            this.btnSelectFiles.Click += new System.EventHandler(this.btnSelectFiles_Click);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(3, 332);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(798, 79);
            this.txtResult.TabIndex = 0;
            this.txtResult.Text = "";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(2, 1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(798, 331);
            this.dataGridView1.TabIndex = 2;
            // 
            // btnSaveCsv
            // 
            this.btnSaveCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCsv.Location = new System.Drawing.Point(626, 417);
            this.btnSaveCsv.Name = "btnSaveCsv";
            this.btnSaveCsv.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCsv.TabIndex = 3;
            this.btnSaveCsv.Text = "Save CSV";
            this.btnSaveCsv.UseVisualStyleBackColor = true;
            this.btnSaveCsv.Click += new System.EventHandler(this.btnSaveCsv_Click);
            // 
            // frmMassCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSaveCsv);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnSelectFiles);
            this.Controls.Add(this.txtResult);
            this.Name = "frmMassCompare";
            this.Text = "Search and Compare";
            this.Load += new System.EventHandler(this.frmMassCompare_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.Button btnSelectFiles;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnSaveCsv;
    }
}