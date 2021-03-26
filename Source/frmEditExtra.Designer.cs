namespace UniversalPatcher
{
    partial class frmEditExtra
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditExtra));
            this.labelName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioCheckword = new System.Windows.Forms.RadioButton();
            this.radioRelative = new System.Windows.Forms.RadioButton();
            this.radioAbsolute = new System.Windows.Forms.RadioButton();
            this.numBytes = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioInt = new System.Windows.Forms.RadioButton();
            this.radioText = new System.Windows.Forms.RadioButton();
            this.radioHEX = new System.Windows.Forms.RadioButton();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.comboCheckword = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listExtra = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(3, 6);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(90, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(169, 20);
            this.txtName.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Address (HEX):";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(90, 30);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(169, 20);
            this.txtAddress.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioCheckword);
            this.groupBox1.Controls.Add(this.radioRelative);
            this.groupBox1.Controls.Add(this.radioAbsolute);
            this.groupBox1.Location = new System.Drawing.Point(4, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 79);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // radioCheckword
            // 
            this.radioCheckword.AutoSize = true;
            this.radioCheckword.Location = new System.Drawing.Point(3, 35);
            this.radioCheckword.Name = "radioCheckword";
            this.radioCheckword.Size = new System.Drawing.Size(104, 17);
            this.radioCheckword.TabIndex = 3;
            this.radioCheckword.TabStop = true;
            this.radioCheckword.Text = "From checkword";
            this.radioCheckword.UseVisualStyleBackColor = true;
            this.radioCheckword.CheckedChanged += new System.EventHandler(this.radioCheckword_CheckedChanged);
            // 
            // radioRelative
            // 
            this.radioRelative.AutoSize = true;
            this.radioRelative.Checked = true;
            this.radioRelative.Location = new System.Drawing.Point(3, 17);
            this.radioRelative.Name = "radioRelative";
            this.radioRelative.Size = new System.Drawing.Size(114, 17);
            this.radioRelative.TabIndex = 2;
            this.radioRelative.TabStop = true;
            this.radioRelative.Text = "From segment start";
            this.radioRelative.UseVisualStyleBackColor = true;
            this.radioRelative.CheckedChanged += new System.EventHandler(this.radioRelative_CheckedChanged);
            // 
            // radioAbsolute
            // 
            this.radioAbsolute.AutoSize = true;
            this.radioAbsolute.Location = new System.Drawing.Point(3, 53);
            this.radioAbsolute.Name = "radioAbsolute";
            this.radioAbsolute.Size = new System.Drawing.Size(66, 17);
            this.radioAbsolute.TabIndex = 4;
            this.radioAbsolute.Text = "Absolute";
            this.radioAbsolute.UseVisualStyleBackColor = true;
            this.radioAbsolute.CheckedChanged += new System.EventHandler(this.radioAbsolute_CheckedChanged);
            // 
            // numBytes
            // 
            this.numBytes.Location = new System.Drawing.Point(136, 173);
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
            this.numBytes.TabIndex = 9;
            this.numBytes.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 153);
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
            this.groupBox2.Location = new System.Drawing.Point(4, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(95, 82);
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
            this.radioInt.TabIndex = 6;
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
            this.radioText.TabIndex = 8;
            this.radioText.Text = "Text";
            this.radioText.UseVisualStyleBackColor = true;
            // 
            // radioHEX
            // 
            this.radioHEX.AutoSize = true;
            this.radioHEX.Location = new System.Drawing.Point(6, 34);
            this.radioHEX.Name = "radioHEX";
            this.radioHEX.Size = new System.Drawing.Size(44, 17);
            this.radioHEX.TabIndex = 7;
            this.radioHEX.Text = "Hex";
            this.radioHEX.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(265, 100);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(48, 27);
            this.btnAdd.TabIndex = 11;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(370, 189);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(57, 27);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // comboCheckword
            // 
            this.comboCheckword.Enabled = false;
            this.comboCheckword.FormattingEnabled = true;
            this.comboCheckword.Location = new System.Drawing.Point(136, 88);
            this.comboCheckword.Name = "comboCheckword";
            this.comboCheckword.Size = new System.Drawing.Size(121, 21);
            this.comboCheckword.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(133, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Checkword Key:";
            // 
            // listExtra
            // 
            this.listExtra.FormattingEnabled = true;
            this.listExtra.Location = new System.Drawing.Point(265, 3);
            this.listExtra.Name = "listExtra";
            this.listExtra.Size = new System.Drawing.Size(161, 95);
            this.listExtra.TabIndex = 10;
            this.listExtra.SelectedIndexChanged += new System.EventHandler(this.listExtra_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(370, 156);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 28);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(370, 99);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(56, 28);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(315, 99);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(55, 28);
            this.btnReplace.TabIndex = 12;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // frmEditExtra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 225);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.listExtra);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboCheckword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numBytes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.labelName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEditExtra";
            this.Text = "Edit Extra info";
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
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label labelName;
        public System.Windows.Forms.TextBox txtName;
        public System.Windows.Forms.TextBox txtAddress;
        public System.Windows.Forms.RadioButton radioRelative;
        public System.Windows.Forms.RadioButton radioAbsolute;
        public System.Windows.Forms.NumericUpDown numBytes;
        public System.Windows.Forms.RadioButton radioInt;
        public System.Windows.Forms.RadioButton radioText;
        public System.Windows.Forms.RadioButton radioHEX;
        private System.Windows.Forms.RadioButton radioCheckword;
        private System.Windows.Forms.ComboBox comboCheckword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listExtra;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnReplace;
    }
}