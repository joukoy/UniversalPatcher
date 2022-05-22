
namespace UniversalPatcher
{
    partial class frmExtraOffset
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
            this.chkFilterOutOfRange = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkColorCoding = new System.Windows.Forms.CheckBox();
            this.chkOffsetBytes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkFilterOutOfRange
            // 
            this.chkFilterOutOfRange.AutoSize = true;
            this.chkFilterOutOfRange.Location = new System.Drawing.Point(12, 59);
            this.chkFilterOutOfRange.Name = "chkFilterOutOfRange";
            this.chkFilterOutOfRange.Size = new System.Drawing.Size(139, 17);
            this.chkFilterOutOfRange.TabIndex = 8;
            this.chkFilterOutOfRange.Text = "Filter out of range tables";
            this.chkFilterOutOfRange.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(173, 43);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 25);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(173, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 25);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(12, 32);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(135, 20);
            this.TextBox1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Range:";
            // 
            // chkColorCoding
            // 
            this.chkColorCoding.AutoSize = true;
            this.chkColorCoding.Location = new System.Drawing.Point(12, 82);
            this.chkColorCoding.Name = "chkColorCoding";
            this.chkColorCoding.Size = new System.Drawing.Size(85, 17);
            this.chkColorCoding.TabIndex = 10;
            this.chkColorCoding.Text = "Color coding";
            this.chkColorCoding.UseVisualStyleBackColor = true;
            // 
            // chkOffsetBytes
            // 
            this.chkOffsetBytes.AutoSize = true;
            this.chkOffsetBytes.Location = new System.Drawing.Point(12, 105);
            this.chkOffsetBytes.Name = "chkOffsetBytes";
            this.chkOffsetBytes.Size = new System.Drawing.Size(117, 17);
            this.chkOffsetBytes.TabIndex = 11;
            this.chkOffsetBytes.Text = "Search offset bytes";
            this.chkOffsetBytes.UseVisualStyleBackColor = true;
            // 
            // frmExtraOffset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(239, 126);
            this.Controls.Add(this.chkOffsetBytes);
            this.Controls.Add(this.chkColorCoding);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkFilterOutOfRange);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.TextBox1);
            this.Name = "frmExtraOffset";
            this.Text = "Extra offset range";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox chkFilterOutOfRange;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox chkColorCoding;
        public System.Windows.Forms.CheckBox chkOffsetBytes;
    }
}