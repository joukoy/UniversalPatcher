namespace UniversalPatcher
{
    partial class frmExtractTable
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
            this.txtExtractDescription = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.txtCompatibleOS = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExtractRange = new System.Windows.Forms.TextBox();
            this.labelBinFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtExtractDescription
            // 
            this.txtExtractDescription.Location = new System.Drawing.Point(117, 90);
            this.txtExtractDescription.Name = "txtExtractDescription";
            this.txtExtractDescription.Size = new System.Drawing.Size(226, 20);
            this.txtExtractDescription.TabIndex = 160;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 157;
            this.label6.Text = "Description:";
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtract.Location = new System.Drawing.Point(277, 116);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(66, 23);
            this.btnExtract.TabIndex = 161;
            this.btnExtract.Text = "Extract";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // txtCompatibleOS
            // 
            this.txtCompatibleOS.Location = new System.Drawing.Point(117, 66);
            this.txtCompatibleOS.Name = "txtCompatibleOS";
            this.txtCompatibleOS.Size = new System.Drawing.Size(226, 20);
            this.txtCompatibleOS.TabIndex = 159;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 156;
            this.label5.Text = "Compatible OS list:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 155;
            this.label4.Text = "Address range (HEX):";
            // 
            // txtExtractRange
            // 
            this.txtExtractRange.Location = new System.Drawing.Point(117, 40);
            this.txtExtractRange.Name = "txtExtractRange";
            this.txtExtractRange.Size = new System.Drawing.Size(226, 20);
            this.txtExtractRange.TabIndex = 158;
            // 
            // labelBinFile
            // 
            this.labelBinFile.AutoSize = true;
            this.labelBinFile.Location = new System.Drawing.Point(8, 10);
            this.labelBinFile.Name = "labelBinFile";
            this.labelBinFile.Size = new System.Drawing.Size(10, 13);
            this.labelBinFile.TabIndex = 162;
            this.labelBinFile.Text = "-";
            // 
            // frmExtractTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 147);
            this.Controls.Add(this.labelBinFile);
            this.Controls.Add(this.txtExtractDescription);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.txtCompatibleOS);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtExtractRange);
            this.Name = "frmExtractTable";
            this.Text = "Extract table";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExtractDescription;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtExtractRange;
        public System.Windows.Forms.Label labelBinFile;
        public System.Windows.Forms.TextBox txtCompatibleOS;
    }
}