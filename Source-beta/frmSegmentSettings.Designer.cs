namespace UniversalPatcher
{
    partial class frmSegmentSettings
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioNone = new System.Windows.Forms.RadioButton();
            this.radioInvSum = new System.Windows.Forms.RadioButton();
            this.radioSUM = new System.Windows.Forms.RadioButton();
            this.radioCrc32 = new System.Windows.Forms.RadioButton();
            this.radioCrc16 = new System.Windows.Forms.RadioButton();
            this.txtSegmentName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSegmentAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radio2None = new System.Windows.Forms.RadioButton();
            this.radio2InvSum = new System.Windows.Forms.RadioButton();
            this.radio2SUM = new System.Windows.Forms.RadioButton();
            this.radio2Crc32 = new System.Windows.Forms.RadioButton();
            this.radio2Crc16 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCSA1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCSA2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listSegments = new System.Windows.Forms.ListView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnModify = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioByte = new System.Windows.Forms.RadioButton();
            this.radioWord = new System.Windows.Forms.RadioButton();
            this.radioDword = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radio2Dword = new System.Windows.Forms.RadioButton();
            this.radio2Word = new System.Windows.Forms.RadioButton();
            this.radio2Byte = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioNone);
            this.groupBox1.Controls.Add(this.radioInvSum);
            this.groupBox1.Controls.Add(this.radioSUM);
            this.groupBox1.Controls.Add(this.radioCrc32);
            this.groupBox1.Controls.Add(this.radioCrc16);
            this.groupBox1.Location = new System.Drawing.Point(69, 273);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(134, 126);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checksum 1 method";
            // 
            // radioNone
            // 
            this.radioNone.AutoSize = true;
            this.radioNone.Checked = true;
            this.radioNone.Location = new System.Drawing.Point(6, 19);
            this.radioNone.Name = "radioNone";
            this.radioNone.Size = new System.Drawing.Size(51, 17);
            this.radioNone.TabIndex = 4;
            this.radioNone.TabStop = true;
            this.radioNone.Text = "None";
            this.radioNone.UseVisualStyleBackColor = true;
            // 
            // radioInvSum
            // 
            this.radioInvSum.AutoSize = true;
            this.radioInvSum.Location = new System.Drawing.Point(6, 91);
            this.radioInvSum.Name = "radioInvSum";
            this.radioInvSum.Size = new System.Drawing.Size(76, 17);
            this.radioInvSum.TabIndex = 3;
            this.radioInvSum.Text = "Invert Sum";
            this.radioInvSum.UseVisualStyleBackColor = true;
            // 
            // radioSUM
            // 
            this.radioSUM.AutoSize = true;
            this.radioSUM.Location = new System.Drawing.Point(6, 72);
            this.radioSUM.Name = "radioSUM";
            this.radioSUM.Size = new System.Drawing.Size(73, 17);
            this.radioSUM.TabIndex = 2;
            this.radioSUM.Text = "Byte SUM";
            this.radioSUM.UseVisualStyleBackColor = true;
            // 
            // radioCrc32
            // 
            this.radioCrc32.AutoSize = true;
            this.radioCrc32.Location = new System.Drawing.Point(6, 54);
            this.radioCrc32.Name = "radioCrc32";
            this.radioCrc32.Size = new System.Drawing.Size(59, 17);
            this.radioCrc32.TabIndex = 1;
            this.radioCrc32.Text = "CRC32";
            this.radioCrc32.UseVisualStyleBackColor = true;
            // 
            // radioCrc16
            // 
            this.radioCrc16.AutoSize = true;
            this.radioCrc16.Location = new System.Drawing.Point(6, 36);
            this.radioCrc16.Name = "radioCrc16";
            this.radioCrc16.Size = new System.Drawing.Size(59, 17);
            this.radioCrc16.TabIndex = 0;
            this.radioCrc16.Text = "CRC16";
            this.radioCrc16.UseVisualStyleBackColor = true;
            // 
            // txtSegmentName
            // 
            this.txtSegmentName.Location = new System.Drawing.Point(149, 153);
            this.txtSegmentName.Name = "txtSegmentName";
            this.txtSegmentName.Size = new System.Drawing.Size(190, 20);
            this.txtSegmentName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Segment name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Segment addresses (HEX):";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtSegmentAddress
            // 
            this.txtSegmentAddress.Location = new System.Drawing.Point(149, 179);
            this.txtSegmentAddress.Name = "txtSegmentAddress";
            this.txtSegmentAddress.Size = new System.Drawing.Size(190, 20);
            this.txtSegmentAddress.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(345, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Example: 0-550,1000-4000";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radio2None);
            this.groupBox2.Controls.Add(this.radio2InvSum);
            this.groupBox2.Controls.Add(this.radio2SUM);
            this.groupBox2.Controls.Add(this.radio2Crc32);
            this.groupBox2.Controls.Add(this.radio2Crc16);
            this.groupBox2.Location = new System.Drawing.Point(253, 273);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(133, 126);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Checksum 2 method";
            // 
            // radio2None
            // 
            this.radio2None.AutoSize = true;
            this.radio2None.Checked = true;
            this.radio2None.Location = new System.Drawing.Point(6, 19);
            this.radio2None.Name = "radio2None";
            this.radio2None.Size = new System.Drawing.Size(51, 17);
            this.radio2None.TabIndex = 5;
            this.radio2None.TabStop = true;
            this.radio2None.Text = "None";
            this.radio2None.UseVisualStyleBackColor = true;
            // 
            // radio2InvSum
            // 
            this.radio2InvSum.AutoSize = true;
            this.radio2InvSum.Location = new System.Drawing.Point(6, 91);
            this.radio2InvSum.Name = "radio2InvSum";
            this.radio2InvSum.Size = new System.Drawing.Size(76, 17);
            this.radio2InvSum.TabIndex = 3;
            this.radio2InvSum.Text = "Invert Sum";
            this.radio2InvSum.UseVisualStyleBackColor = true;
            // 
            // radio2SUM
            // 
            this.radio2SUM.AutoSize = true;
            this.radio2SUM.Location = new System.Drawing.Point(6, 71);
            this.radio2SUM.Name = "radio2SUM";
            this.radio2SUM.Size = new System.Drawing.Size(73, 17);
            this.radio2SUM.TabIndex = 2;
            this.radio2SUM.Text = "Byte SUM";
            this.radio2SUM.UseVisualStyleBackColor = true;
            // 
            // radio2Crc32
            // 
            this.radio2Crc32.AutoSize = true;
            this.radio2Crc32.Location = new System.Drawing.Point(6, 53);
            this.radio2Crc32.Name = "radio2Crc32";
            this.radio2Crc32.Size = new System.Drawing.Size(59, 17);
            this.radio2Crc32.TabIndex = 1;
            this.radio2Crc32.Text = "CRC32";
            this.radio2Crc32.UseVisualStyleBackColor = true;
            // 
            // radio2Crc16
            // 
            this.radio2Crc16.AutoSize = true;
            this.radio2Crc16.Location = new System.Drawing.Point(6, 36);
            this.radio2Crc16.Name = "radio2Crc16";
            this.radio2Crc16.Size = new System.Drawing.Size(59, 17);
            this.radio2Crc16.TabIndex = 0;
            this.radio2Crc16.Text = "CRC16";
            this.radio2Crc16.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Checksum 1 Address (HEX):";
            // 
            // txtCSA1
            // 
            this.txtCSA1.Location = new System.Drawing.Point(149, 205);
            this.txtCSA1.Name = "txtCSA1";
            this.txtCSA1.Size = new System.Drawing.Size(190, 20);
            this.txtCSA1.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Checksum 2 Address (HEX):";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtCSA2
            // 
            this.txtCSA2.Location = new System.Drawing.Point(148, 237);
            this.txtCSA2.Name = "txtCSA2";
            this.txtCSA2.Size = new System.Drawing.Size(191, 20);
            this.txtCSA2.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(404, 364);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 35);
            this.button1.TabIndex = 11;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listSegments
            // 
            this.listSegments.HideSelection = false;
            this.listSegments.Location = new System.Drawing.Point(2, 3);
            this.listSegments.Name = "listSegments";
            this.listSegments.Size = new System.Drawing.Size(538, 141);
            this.listSegments.TabIndex = 12;
            this.listSegments.UseCompatibleStateImageBehavior = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(369, 150);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(50, 25);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(425, 150);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(52, 25);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnModify
            // 
            this.btnModify.Location = new System.Drawing.Point(483, 150);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(46, 25);
            this.btnModify.TabIndex = 15;
            this.btnModify.Text = "Modify";
            this.btnModify.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(404, 326);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(74, 35);
            this.btnLoad.TabIndex = 16;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioDword);
            this.groupBox3.Controls.Add(this.radioWord);
            this.groupBox3.Controls.Add(this.radioByte);
            this.groupBox3.Location = new System.Drawing.Point(344, 198);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(185, 34);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            // 
            // radioByte
            // 
            this.radioByte.AutoSize = true;
            this.radioByte.Location = new System.Drawing.Point(6, 10);
            this.radioByte.Name = "radioByte";
            this.radioByte.Size = new System.Drawing.Size(45, 17);
            this.radioByte.TabIndex = 0;
            this.radioByte.Text = "byte";
            this.radioByte.UseVisualStyleBackColor = true;
            // 
            // radioWord
            // 
            this.radioWord.AutoSize = true;
            this.radioWord.Checked = true;
            this.radioWord.Location = new System.Drawing.Point(58, 10);
            this.radioWord.Name = "radioWord";
            this.radioWord.Size = new System.Drawing.Size(48, 17);
            this.radioWord.TabIndex = 1;
            this.radioWord.TabStop = true;
            this.radioWord.Text = "word";
            this.radioWord.UseVisualStyleBackColor = true;
            // 
            // radioDword
            // 
            this.radioDword.AutoSize = true;
            this.radioDword.Location = new System.Drawing.Point(111, 10);
            this.radioDword.Name = "radioDword";
            this.radioDword.Size = new System.Drawing.Size(54, 17);
            this.radioDword.TabIndex = 2;
            this.radioDword.Text = "dword";
            this.radioDword.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radio2Dword);
            this.groupBox4.Controls.Add(this.radio2Word);
            this.groupBox4.Controls.Add(this.radio2Byte);
            this.groupBox4.Location = new System.Drawing.Point(344, 231);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(179, 36);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            // 
            // radio2Dword
            // 
            this.radio2Dword.AutoSize = true;
            this.radio2Dword.Location = new System.Drawing.Point(111, 10);
            this.radio2Dword.Name = "radio2Dword";
            this.radio2Dword.Size = new System.Drawing.Size(54, 17);
            this.radio2Dword.TabIndex = 2;
            this.radio2Dword.Text = "dword";
            this.radio2Dword.UseVisualStyleBackColor = true;
            // 
            // radio2Word
            // 
            this.radio2Word.AutoSize = true;
            this.radio2Word.Checked = true;
            this.radio2Word.Location = new System.Drawing.Point(57, 9);
            this.radio2Word.Name = "radio2Word";
            this.radio2Word.Size = new System.Drawing.Size(48, 17);
            this.radio2Word.TabIndex = 1;
            this.radio2Word.TabStop = true;
            this.radio2Word.Text = "word";
            this.radio2Word.UseVisualStyleBackColor = true;
            // 
            // radio2Byte
            // 
            this.radio2Byte.AutoSize = true;
            this.radio2Byte.Location = new System.Drawing.Point(6, 10);
            this.radio2Byte.Name = "radio2Byte";
            this.radio2Byte.Size = new System.Drawing.Size(45, 17);
            this.radio2Byte.TabIndex = 0;
            this.radio2Byte.Text = "byte";
            this.radio2Byte.UseVisualStyleBackColor = true;
            // 
            // frmSegmentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 412);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnModify);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.listSegments);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtCSA2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCSA1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSegmentAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSegmentName);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmSegmentSettings";
            this.Text = "Segment settings";
            this.Load += new System.EventHandler(this.frmSegmentSettings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioInvSum;
        private System.Windows.Forms.RadioButton radioSUM;
        private System.Windows.Forms.RadioButton radioCrc32;
        private System.Windows.Forms.RadioButton radioCrc16;
        private System.Windows.Forms.TextBox txtSegmentName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSegmentAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radio2InvSum;
        private System.Windows.Forms.RadioButton radio2SUM;
        private System.Windows.Forms.RadioButton radio2Crc32;
        private System.Windows.Forms.RadioButton radio2Crc16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCSA1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCSA2;
        private System.Windows.Forms.RadioButton radioNone;
        private System.Windows.Forms.RadioButton radio2None;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.ListView listSegments;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioDword;
        private System.Windows.Forms.RadioButton radioWord;
        private System.Windows.Forms.RadioButton radioByte;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radio2Dword;
        private System.Windows.Forms.RadioButton radio2Word;
        private System.Windows.Forms.RadioButton radio2Byte;
    }
}