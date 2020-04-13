namespace UniversalPatcher
{
    partial class frmCVN
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
            this.btnClearCVN = new System.Windows.Forms.Button();
            this.btnAddtoStock = new System.Windows.Forms.Button();
            this.dataCVN = new System.Windows.Forms.DataGridView();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataCVN)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClearCVN
            // 
            this.btnClearCVN.Location = new System.Drawing.Point(4, 2);
            this.btnClearCVN.Name = "btnClearCVN";
            this.btnClearCVN.Size = new System.Drawing.Size(62, 24);
            this.btnClearCVN.TabIndex = 181;
            this.btnClearCVN.Text = "Clear";
            this.btnClearCVN.UseVisualStyleBackColor = true;
            this.btnClearCVN.Click += new System.EventHandler(this.btnClearCVN_Click);
            // 
            // btnAddtoStock
            // 
            this.btnAddtoStock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddtoStock.Location = new System.Drawing.Point(577, 2);
            this.btnAddtoStock.Name = "btnAddtoStock";
            this.btnAddtoStock.Size = new System.Drawing.Size(136, 24);
            this.btnAddtoStock.TabIndex = 180;
            this.btnAddtoStock.Text = "Add to: \"stockcvn.xml\"";
            this.btnAddtoStock.UseVisualStyleBackColor = true;
            this.btnAddtoStock.Click += new System.EventHandler(this.btnAddtoStock_Click);
            // 
            // dataCVN
            // 
            this.dataCVN.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataCVN.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataCVN.Location = new System.Drawing.Point(0, 28);
            this.dataCVN.Name = "dataCVN";
            this.dataCVN.Size = new System.Drawing.Size(713, 337);
            this.dataCVN.TabIndex = 179;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(83, 2);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(62, 24);
            this.btnRefresh.TabIndex = 182;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // frmCVN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 366);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClearCVN);
            this.Controls.Add(this.btnAddtoStock);
            this.Controls.Add(this.dataCVN);
            this.Name = "frmCVN";
            this.Text = "CVN list";
            ((System.ComponentModel.ISupportInitialize)(this.dataCVN)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClearCVN;
        private System.Windows.Forms.Button btnAddtoStock;
        private System.Windows.Forms.DataGridView dataCVN;
        private System.Windows.Forms.Button btnRefresh;
    }
}