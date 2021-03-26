namespace UniversalPatcher
{
    partial class frmEditDetectAddress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditDetectAddress));
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkEndofFile = new System.Windows.Forms.CheckBox();
            this.radioStartAbsolute = new System.Windows.Forms.RadioButton();
            this.numBytes = new System.Windows.Forms.NumericUpDown();
            this.radioStartRead = new System.Windows.Forms.RadioButton();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Bytes:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(325, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(325, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkEndofFile);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.radioStartAbsolute);
            this.groupBox1.Controls.Add(this.numBytes);
            this.groupBox1.Controls.Add(this.radioStartRead);
            this.groupBox1.Controls.Add(this.txtStart);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 94);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // chkEndofFile
            // 
            this.chkEndofFile.AutoSize = true;
            this.chkEndofFile.Location = new System.Drawing.Point(10, 63);
            this.chkEndofFile.Name = "chkEndofFile";
            this.chkEndofFile.Size = new System.Drawing.Size(98, 17);
            this.chkEndofFile.TabIndex = 23;
            this.chkEndofFile.Text = "From end of file";
            this.chkEndofFile.UseVisualStyleBackColor = true;
            // 
            // radioStartAbsolute
            // 
            this.radioStartAbsolute.AutoSize = true;
            this.radioStartAbsolute.Location = new System.Drawing.Point(10, 19);
            this.radioStartAbsolute.Name = "radioStartAbsolute";
            this.radioStartAbsolute.Size = new System.Drawing.Size(118, 17);
            this.radioStartAbsolute.TabIndex = 1;
            this.radioStartAbsolute.TabStop = true;
            this.radioStartAbsolute.Text = "Use address (HEX):";
            this.radioStartAbsolute.UseVisualStyleBackColor = true;
            // 
            // numBytes
            // 
            this.numBytes.Location = new System.Drawing.Point(222, 68);
            this.numBytes.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numBytes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBytes.Name = "numBytes";
            this.numBytes.Size = new System.Drawing.Size(68, 20);
            this.numBytes.TabIndex = 21;
            this.numBytes.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // radioStartRead
            // 
            this.radioStartRead.AutoSize = true;
            this.radioStartRead.Checked = true;
            this.radioStartRead.Location = new System.Drawing.Point(10, 40);
            this.radioStartRead.Name = "radioStartRead";
            this.radioStartRead.Size = new System.Drawing.Size(151, 17);
            this.radioStartRead.TabIndex = 0;
            this.radioStartRead.TabStop = true;
            this.radioStartRead.Text = "Read address from (HEX): ";
            this.radioStartRead.UseVisualStyleBackColor = true;
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(163, 31);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(127, 20);
            this.txtStart.TabIndex = 1;
            // 
            // frmEditDetectAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 117);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEditDetectAddress";
            this.Text = "Edit address";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkEndofFile;
        private System.Windows.Forms.RadioButton radioStartAbsolute;
        public System.Windows.Forms.NumericUpDown numBytes;
        private System.Windows.Forms.RadioButton radioStartRead;
        private System.Windows.Forms.TextBox txtStart;
    }
}