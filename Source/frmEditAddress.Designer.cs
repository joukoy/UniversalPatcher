namespace UniversalPatcher
{
    partial class frmEditAddress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditAddress));
            this.label2 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioEndSegment = new System.Windows.Forms.RadioButton();
            this.radioEndFile = new System.Windows.Forms.RadioButton();
            this.radioRelative = new System.Windows.Forms.RadioButton();
            this.radioAbsolute = new System.Windows.Forms.RadioButton();
            this.numBytes = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioInt = new System.Windows.Forms.RadioButton();
            this.radioText = new System.Windows.Forms.RadioButton();
            this.radioHEX = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Address (HEX):";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(95, 12);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(177, 20);
            this.txtAddress.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioEndSegment);
            this.groupBox1.Controls.Add(this.radioEndFile);
            this.groupBox1.Controls.Add(this.radioRelative);
            this.groupBox1.Controls.Add(this.radioAbsolute);
            this.groupBox1.Location = new System.Drawing.Point(9, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 122);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // radioEndSegment
            // 
            this.radioEndSegment.AutoSize = true;
            this.radioEndSegment.Location = new System.Drawing.Point(3, 63);
            this.radioEndSegment.Name = "radioEndSegment";
            this.radioEndSegment.Size = new System.Drawing.Size(124, 17);
            this.radioEndSegment.TabIndex = 4;
            this.radioEndSegment.TabStop = true;
            this.radioEndSegment.Text = "From end of segment";
            this.radioEndSegment.UseVisualStyleBackColor = true;
            // 
            // radioEndFile
            // 
            this.radioEndFile.AutoSize = true;
            this.radioEndFile.Location = new System.Drawing.Point(3, 86);
            this.radioEndFile.Name = "radioEndFile";
            this.radioEndFile.Size = new System.Drawing.Size(97, 17);
            this.radioEndFile.TabIndex = 3;
            this.radioEndFile.TabStop = true;
            this.radioEndFile.Text = "From end of file";
            this.radioEndFile.UseVisualStyleBackColor = true;
            // 
            // radioRelative
            // 
            this.radioRelative.AutoSize = true;
            this.radioRelative.Checked = true;
            this.radioRelative.Location = new System.Drawing.Point(3, 17);
            this.radioRelative.Name = "radioRelative";
            this.radioRelative.Size = new System.Drawing.Size(114, 17);
            this.radioRelative.TabIndex = 1;
            this.radioRelative.TabStop = true;
            this.radioRelative.Text = "From segment start";
            this.radioRelative.UseVisualStyleBackColor = true;
            // 
            // radioAbsolute
            // 
            this.radioAbsolute.AutoSize = true;
            this.radioAbsolute.Location = new System.Drawing.Point(3, 40);
            this.radioAbsolute.Name = "radioAbsolute";
            this.radioAbsolute.Size = new System.Drawing.Size(99, 17);
            this.radioAbsolute.TabIndex = 2;
            this.radioAbsolute.Text = "From start of file";
            this.radioAbsolute.UseVisualStyleBackColor = true;
            // 
            // numBytes
            // 
            this.numBytes.Location = new System.Drawing.Point(204, 37);
            this.numBytes.Maximum = new decimal(new int[] {
            1000,
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
            this.numBytes.TabIndex = 3;
            this.numBytes.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Bytes:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioInt);
            this.groupBox2.Controls.Add(this.radioText);
            this.groupBox2.Controls.Add(this.radioHEX);
            this.groupBox2.Location = new System.Drawing.Point(200, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(72, 77);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Show as:";
            // 
            // radioInt
            // 
            this.radioInt.AutoSize = true;
            this.radioInt.Checked = true;
            this.radioInt.Location = new System.Drawing.Point(6, 14);
            this.radioInt.Name = "radioInt";
            this.radioInt.Size = new System.Drawing.Size(58, 17);
            this.radioInt.TabIndex = 4;
            this.radioInt.TabStop = true;
            this.radioInt.Text = "Integer";
            this.radioInt.UseVisualStyleBackColor = true;
            // 
            // radioText
            // 
            this.radioText.AutoSize = true;
            this.radioText.Location = new System.Drawing.Point(6, 53);
            this.radioText.Name = "radioText";
            this.radioText.Size = new System.Drawing.Size(46, 17);
            this.radioText.TabIndex = 6;
            this.radioText.Text = "Text";
            this.radioText.UseVisualStyleBackColor = true;
            // 
            // radioHEX
            // 
            this.radioHEX.AutoSize = true;
            this.radioHEX.Location = new System.Drawing.Point(6, 34);
            this.radioHEX.Name = "radioHEX";
            this.radioHEX.Size = new System.Drawing.Size(44, 17);
            this.radioHEX.TabIndex = 5;
            this.radioHEX.Text = "Hex";
            this.radioHEX.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(215, 146);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(57, 27);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(152, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(57, 27);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmEditAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 182);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numBytes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEditAddress";
            this.Text = "Edit Address";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.TextBox txtAddress;
        public System.Windows.Forms.RadioButton radioRelative;
        public System.Windows.Forms.RadioButton radioAbsolute;
        public System.Windows.Forms.NumericUpDown numBytes;
        public System.Windows.Forms.RadioButton radioInt;
        public System.Windows.Forms.RadioButton radioText;
        public System.Windows.Forms.RadioButton radioHEX;
        private System.Windows.Forms.RadioButton radioEndSegment;
        private System.Windows.Forms.RadioButton radioEndFile;
    }
}