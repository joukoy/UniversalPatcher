
namespace UniversalPatcher
{
    partial class frmImportFile
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.labelFileName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileSize = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFillGaps = new System.Windows.Forms.TextBox();
            this.chkSplit = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Offset (HEX):";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(101, 6);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(100, 20);
            this.txtOffset.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(482, 29);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save bin";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 58);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(652, 262);
            this.txtResult.TabIndex = 5;
            this.txtResult.Text = "";
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(207, 9);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(10, 13);
            this.labelFileName.TabIndex = 6;
            this.labelFileName.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "File size (HEX):";
            // 
            // txtFileSize
            // 
            this.txtFileSize.Location = new System.Drawing.Point(101, 31);
            this.txtFileSize.Name = "txtFileSize";
            this.txtFileSize.Size = new System.Drawing.Size(100, 20);
            this.txtFileSize.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Fill gaps:";
            // 
            // txtFillGaps
            // 
            this.txtFillGaps.Location = new System.Drawing.Point(261, 31);
            this.txtFillGaps.Name = "txtFillGaps";
            this.txtFillGaps.Size = new System.Drawing.Size(100, 20);
            this.txtFillGaps.TabIndex = 10;
            this.txtFillGaps.Text = "FF 00";
            // 
            // chkSplit
            // 
            this.chkSplit.AutoSize = true;
            this.chkSplit.Location = new System.Drawing.Point(367, 33);
            this.chkSplit.Name = "chkSplit";
            this.chkSplit.Size = new System.Drawing.Size(86, 17);
            this.chkSplit.TabIndex = 11;
            this.chkSplit.Text = "Segment/file";
            this.chkSplit.UseVisualStyleBackColor = true;
            // 
            // frmImportFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 320);
            this.Controls.Add(this.chkSplit);
            this.Controls.Add(this.txtFillGaps);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFileSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.label1);
            this.Name = "frmImportFile";
            this.Text = "Import";
            this.Load += new System.EventHandler(this.frmImportFile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFileSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFillGaps;
        private System.Windows.Forms.CheckBox chkSplit;
    }
}