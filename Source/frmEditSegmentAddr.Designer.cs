namespace UniversalPatcher
{
    partial class frmEditSegmentAddr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditSegmentAddr));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioStartAbsolute = new System.Windows.Forms.RadioButton();
            this.radioStartRead = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.numReadPairs = new System.Windows.Forms.NumericUpDown();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioReadSize = new System.Windows.Forms.RadioButton();
            this.radioSize = new System.Windows.Forms.RadioButton();
            this.radioEndAbsolute = new System.Windows.Forms.RadioButton();
            this.radioReadEnd = new System.Windows.Forms.RadioButton();
            this.radioUseStart = new System.Windows.Forms.RadioButton();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.chkEnd = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numBlock = new System.Windows.Forms.NumericUpDown();
            this.labelOf = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numBytes = new System.Windows.Forms.NumericUpDown();
            this.radioStartSeek = new System.Windows.Forms.RadioButton();
            this.btnStartSeek = new System.Windows.Forms.Button();
            this.btnEndSeek = new System.Windows.Forms.Button();
            this.radioEndSeek = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadPairs)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnStartSeek);
            this.groupBox1.Controls.Add(this.numBytes);
            this.groupBox1.Controls.Add(this.radioStartSeek);
            this.groupBox1.Controls.Add(this.radioStartAbsolute);
            this.groupBox1.Controls.Add(this.radioStartRead);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numReadPairs);
            this.groupBox1.Controls.Add(this.txtStart);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Start Address";
            // 
            // radioStartAbsolute
            // 
            this.radioStartAbsolute.AutoSize = true;
            this.radioStartAbsolute.Location = new System.Drawing.Point(9, 23);
            this.radioStartAbsolute.Name = "radioStartAbsolute";
            this.radioStartAbsolute.Size = new System.Drawing.Size(118, 17);
            this.radioStartAbsolute.TabIndex = 1;
            this.radioStartAbsolute.TabStop = true;
            this.radioStartAbsolute.Text = "Use address (HEX):";
            this.radioStartAbsolute.UseVisualStyleBackColor = true;
            this.radioStartAbsolute.CheckedChanged += new System.EventHandler(this.radioStartAbsolute_CheckedChanged);
            // 
            // radioStartRead
            // 
            this.radioStartRead.AutoSize = true;
            this.radioStartRead.Checked = true;
            this.radioStartRead.Location = new System.Drawing.Point(9, 63);
            this.radioStartRead.Name = "radioStartRead";
            this.radioStartRead.Size = new System.Drawing.Size(151, 17);
            this.radioStartRead.TabIndex = 2;
            this.radioStartRead.TabStop = true;
            this.radioStartRead.Text = "Read address from (HEX): ";
            this.radioStartRead.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 6;
            this.label1.Tag = "";
            this.label1.Text = "Address pairs (start, end):";
            // 
            // numReadPairs
            // 
            this.numReadPairs.Location = new System.Drawing.Point(221, 83);
            this.numReadPairs.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numReadPairs.Name = "numReadPairs";
            this.numReadPairs.Size = new System.Drawing.Size(68, 20);
            this.numReadPairs.TabIndex = 4;
            this.numReadPairs.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(162, 40);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(127, 20);
            this.txtStart.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnEndSeek);
            this.groupBox2.Controls.Add(this.radioEndSeek);
            this.groupBox2.Controls.Add(this.radioReadSize);
            this.groupBox2.Controls.Add(this.radioSize);
            this.groupBox2.Controls.Add(this.radioEndAbsolute);
            this.groupBox2.Controls.Add(this.radioReadEnd);
            this.groupBox2.Controls.Add(this.radioUseStart);
            this.groupBox2.Controls.Add(this.txtEnd);
            this.groupBox2.Controls.Add(this.chkEnd);
            this.groupBox2.Location = new System.Drawing.Point(4, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 159);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "End address";
            // 
            // radioReadSize
            // 
            this.radioReadSize.AutoSize = true;
            this.radioReadSize.Location = new System.Drawing.Point(10, 111);
            this.radioReadSize.Name = "radioReadSize";
            this.radioReadSize.Size = new System.Drawing.Size(129, 17);
            this.radioReadSize.TabIndex = 12;
            this.radioReadSize.TabStop = true;
            this.radioReadSize.Text = "Read size from (HEX):";
            this.radioReadSize.UseVisualStyleBackColor = true;
            // 
            // radioSize
            // 
            this.radioSize.AutoSize = true;
            this.radioSize.Location = new System.Drawing.Point(10, 88);
            this.radioSize.Name = "radioSize";
            this.radioSize.Size = new System.Drawing.Size(122, 17);
            this.radioSize.TabIndex = 11;
            this.radioSize.TabStop = true;
            this.radioSize.Text = "Segment size (HEX):";
            this.radioSize.UseVisualStyleBackColor = true;
            this.radioSize.CheckedChanged += new System.EventHandler(this.radioSize_CheckedChanged);
            // 
            // radioEndAbsolute
            // 
            this.radioEndAbsolute.AutoSize = true;
            this.radioEndAbsolute.Location = new System.Drawing.Point(10, 42);
            this.radioEndAbsolute.Name = "radioEndAbsolute";
            this.radioEndAbsolute.Size = new System.Drawing.Size(119, 17);
            this.radioEndAbsolute.TabIndex = 7;
            this.radioEndAbsolute.TabStop = true;
            this.radioEndAbsolute.Text = "Use Address (HEX):";
            this.radioEndAbsolute.UseVisualStyleBackColor = true;
            this.radioEndAbsolute.CheckedChanged += new System.EventHandler(this.radioEndAbsolute_CheckedChanged);
            // 
            // radioReadEnd
            // 
            this.radioReadEnd.AutoSize = true;
            this.radioReadEnd.Location = new System.Drawing.Point(10, 65);
            this.radioReadEnd.Name = "radioReadEnd";
            this.radioReadEnd.Size = new System.Drawing.Size(148, 17);
            this.radioReadEnd.TabIndex = 8;
            this.radioReadEnd.TabStop = true;
            this.radioReadEnd.Text = "Read address from (HEX):";
            this.radioReadEnd.UseVisualStyleBackColor = true;
            this.radioReadEnd.CheckedChanged += new System.EventHandler(this.radioReadEnd_CheckedChanged);
            // 
            // radioUseStart
            // 
            this.radioUseStart.AutoSize = true;
            this.radioUseStart.Checked = true;
            this.radioUseStart.Location = new System.Drawing.Point(10, 19);
            this.radioUseStart.Name = "radioUseStart";
            this.radioUseStart.Size = new System.Drawing.Size(138, 17);
            this.radioUseStart.TabIndex = 6;
            this.radioUseStart.TabStop = true;
            this.radioUseStart.Text = "Read after start address";
            this.radioUseStart.UseVisualStyleBackColor = true;
            this.radioUseStart.CheckedChanged += new System.EventHandler(this.radioUseStart_CheckedChanged);
            // 
            // txtEnd
            // 
            this.txtEnd.Enabled = false;
            this.txtEnd.Location = new System.Drawing.Point(163, 47);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(126, 20);
            this.txtEnd.TabIndex = 9;
            // 
            // chkEnd
            // 
            this.chkEnd.AutoSize = true;
            this.chkEnd.Enabled = false;
            this.chkEnd.Location = new System.Drawing.Point(163, 73);
            this.chkEnd.Name = "chkEnd";
            this.chkEnd.Size = new System.Drawing.Size(103, 17);
            this.chkEnd.TabIndex = 10;
            this.chkEnd.Text = "From END of file";
            this.chkEnd.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 314);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Offset (HEX)";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(91, 311);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(127, 20);
            this.txtOffset.TabIndex = 11;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(323, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(323, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Block:";
            // 
            // numBlock
            // 
            this.numBlock.Location = new System.Drawing.Point(362, 84);
            this.numBlock.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBlock.Name = "numBlock";
            this.numBlock.Size = new System.Drawing.Size(36, 20);
            this.numBlock.TabIndex = 14;
            this.numBlock.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBlock.ValueChanged += new System.EventHandler(this.numBlock_ValueChanged);
            // 
            // labelOf
            // 
            this.labelOf.AutoSize = true;
            this.labelOf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOf.Location = new System.Drawing.Point(369, 110);
            this.labelOf.Name = "labelOf";
            this.labelOf.Size = new System.Drawing.Size(29, 16);
            this.labelOf.TabIndex = 13;
            this.labelOf.Text = "of 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Bytes:";
            // 
            // numBytes
            // 
            this.numBytes.Location = new System.Drawing.Point(222, 109);
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
            this.numBytes.TabIndex = 5;
            this.numBytes.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // radioStartSeek
            // 
            this.radioStartSeek.AutoSize = true;
            this.radioStartSeek.Location = new System.Drawing.Point(9, 43);
            this.radioStartSeek.Name = "radioStartSeek";
            this.radioStartSeek.Size = new System.Drawing.Size(73, 17);
            this.radioStartSeek.TabIndex = 7;
            this.radioStartSeek.TabStop = true;
            this.radioStartSeek.Text = "Use seek:";
            this.radioStartSeek.UseVisualStyleBackColor = true;
            // 
            // btnStartSeek
            // 
            this.btnStartSeek.Location = new System.Drawing.Point(87, 44);
            this.btnStartSeek.Name = "btnStartSeek";
            this.btnStartSeek.Size = new System.Drawing.Size(28, 19);
            this.btnStartSeek.TabIndex = 8;
            this.btnStartSeek.Text = "...";
            this.btnStartSeek.UseVisualStyleBackColor = true;
            this.btnStartSeek.Click += new System.EventHandler(this.btnStartSeek_Click);
            // 
            // btnEndSeek
            // 
            this.btnEndSeek.Location = new System.Drawing.Point(87, 132);
            this.btnEndSeek.Name = "btnEndSeek";
            this.btnEndSeek.Size = new System.Drawing.Size(28, 19);
            this.btnEndSeek.TabIndex = 14;
            this.btnEndSeek.Text = "...";
            this.btnEndSeek.UseVisualStyleBackColor = true;
            this.btnEndSeek.Click += new System.EventHandler(this.btnEndSeek_Click);
            // 
            // radioEndSeek
            // 
            this.radioEndSeek.AutoSize = true;
            this.radioEndSeek.Location = new System.Drawing.Point(10, 134);
            this.radioEndSeek.Name = "radioEndSeek";
            this.radioEndSeek.Size = new System.Drawing.Size(73, 17);
            this.radioEndSeek.TabIndex = 13;
            this.radioEndSeek.TabStop = true;
            this.radioEndSeek.Text = "Use seek:";
            this.radioEndSeek.UseVisualStyleBackColor = true;
            // 
            // frmEditSegmentAddr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 363);
            this.Controls.Add(this.labelOf);
            this.Controls.Add(this.numBlock);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmEditSegmentAddr";
            this.Text = "Edit Segment Address";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadPairs)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBytes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioStartAbsolute;
        private System.Windows.Forms.RadioButton radioStartRead;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioEndAbsolute;
        private System.Windows.Forms.RadioButton radioReadEnd;
        private System.Windows.Forms.RadioButton radioUseStart;
        private System.Windows.Forms.TextBox txtEnd;
        private System.Windows.Forms.CheckBox chkEnd;
        private System.Windows.Forms.NumericUpDown numReadPairs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numBlock;
        private System.Windows.Forms.Label labelOf;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown numBytes;
        private System.Windows.Forms.RadioButton radioReadSize;
        private System.Windows.Forms.RadioButton radioSize;
        private System.Windows.Forms.Button btnStartSeek;
        private System.Windows.Forms.RadioButton radioStartSeek;
        private System.Windows.Forms.Button btnEndSeek;
        private System.Windows.Forms.RadioButton radioEndSeek;
    }
}