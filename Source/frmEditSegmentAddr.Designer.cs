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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioStartAbsolute = new System.Windows.Forms.RadioButton();
            this.radioReadStart = new System.Windows.Forms.RadioButton();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioEndAbsolute = new System.Windows.Forms.RadioButton();
            this.radioReadEnd = new System.Windows.Forms.RadioButton();
            this.radioUseStart = new System.Windows.Forms.RadioButton();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.chkEnd = new System.Windows.Forms.CheckBox();
            this.numReadPairs = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numBlock = new System.Windows.Forms.NumericUpDown();
            this.labelOf = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadPairs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioStartAbsolute);
            this.groupBox1.Controls.Add(this.radioReadStart);
            this.groupBox1.Location = new System.Drawing.Point(4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 63);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Start Address";
            // 
            // radioStartAbsolute
            // 
            this.radioStartAbsolute.AutoSize = true;
            this.radioStartAbsolute.Location = new System.Drawing.Point(10, 41);
            this.radioStartAbsolute.Name = "radioStartAbsolute";
            this.radioStartAbsolute.Size = new System.Drawing.Size(118, 17);
            this.radioStartAbsolute.TabIndex = 1;
            this.radioStartAbsolute.TabStop = true;
            this.radioStartAbsolute.Text = "Use address (HEX):";
            this.radioStartAbsolute.UseVisualStyleBackColor = true;
            // 
            // radioReadStart
            // 
            this.radioReadStart.AutoSize = true;
            this.radioReadStart.Checked = true;
            this.radioReadStart.Location = new System.Drawing.Point(10, 19);
            this.radioReadStart.Name = "radioReadStart";
            this.radioReadStart.Size = new System.Drawing.Size(151, 17);
            this.radioReadStart.TabIndex = 0;
            this.radioReadStart.TabStop = true;
            this.radioReadStart.Text = "Read address from (HEX): ";
            this.radioReadStart.UseVisualStyleBackColor = true;
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(173, 12);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(127, 20);
            this.txtStart.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioEndAbsolute);
            this.groupBox2.Controls.Add(this.radioReadEnd);
            this.groupBox2.Controls.Add(this.radioUseStart);
            this.groupBox2.Location = new System.Drawing.Point(4, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(164, 90);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "End address";
            // 
            // radioEndAbsolute
            // 
            this.radioEndAbsolute.AutoSize = true;
            this.radioEndAbsolute.Location = new System.Drawing.Point(10, 65);
            this.radioEndAbsolute.Name = "radioEndAbsolute";
            this.radioEndAbsolute.Size = new System.Drawing.Size(119, 17);
            this.radioEndAbsolute.TabIndex = 2;
            this.radioEndAbsolute.TabStop = true;
            this.radioEndAbsolute.Text = "Use Address (HEX):";
            this.radioEndAbsolute.UseVisualStyleBackColor = true;
            // 
            // radioReadEnd
            // 
            this.radioReadEnd.AutoSize = true;
            this.radioReadEnd.Location = new System.Drawing.Point(10, 42);
            this.radioReadEnd.Name = "radioReadEnd";
            this.radioReadEnd.Size = new System.Drawing.Size(148, 17);
            this.radioReadEnd.TabIndex = 1;
            this.radioReadEnd.TabStop = true;
            this.radioReadEnd.Text = "Read address from (HEX):";
            this.radioReadEnd.UseVisualStyleBackColor = true;
            // 
            // radioUseStart
            // 
            this.radioUseStart.AutoSize = true;
            this.radioUseStart.Checked = true;
            this.radioUseStart.Location = new System.Drawing.Point(10, 19);
            this.radioUseStart.Name = "radioUseStart";
            this.radioUseStart.Size = new System.Drawing.Size(138, 17);
            this.radioUseStart.TabIndex = 0;
            this.radioUseStart.TabStop = true;
            this.radioUseStart.Text = "Read after start address";
            this.radioUseStart.UseVisualStyleBackColor = true;
            // 
            // txtEnd
            // 
            this.txtEnd.Location = new System.Drawing.Point(174, 137);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(126, 20);
            this.txtEnd.TabIndex = 3;
            // 
            // chkEnd
            // 
            this.chkEnd.AutoSize = true;
            this.chkEnd.Location = new System.Drawing.Point(174, 159);
            this.chkEnd.Name = "chkEnd";
            this.chkEnd.Size = new System.Drawing.Size(103, 17);
            this.chkEnd.TabIndex = 4;
            this.chkEnd.Text = "From END of file";
            this.chkEnd.UseVisualStyleBackColor = true;
            // 
            // numReadPairs
            // 
            this.numReadPairs.Location = new System.Drawing.Point(175, 59);
            this.numReadPairs.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numReadPairs.Name = "numReadPairs";
            this.numReadPairs.Size = new System.Drawing.Size(125, 20);
            this.numReadPairs.TabIndex = 5;
            this.numReadPairs.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 6;
            this.label1.Tag = "";
            this.label1.Text = "Address pairs (start, end):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Offset (HEX)";
            // 
            // txtOffset
            // 
            this.txtOffset.Location = new System.Drawing.Point(173, 192);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(127, 20);
            this.txtOffset.TabIndex = 8;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(323, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(323, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Block:";
            // 
            // numBlock
            // 
            this.numBlock.Location = new System.Drawing.Point(362, 102);
            this.numBlock.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBlock.Name = "numBlock";
            this.numBlock.Size = new System.Drawing.Size(36, 20);
            this.numBlock.TabIndex = 12;
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
            this.labelOf.Location = new System.Drawing.Point(369, 128);
            this.labelOf.Name = "labelOf";
            this.labelOf.Size = new System.Drawing.Size(29, 16);
            this.labelOf.TabIndex = 13;
            this.labelOf.Text = "of 1";
            // 
            // frmEditSegmentAddr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 225);
            this.Controls.Add(this.labelOf);
            this.Controls.Add(this.numBlock);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numReadPairs);
            this.Controls.Add(this.chkEnd);
            this.Controls.Add(this.txtEnd);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtStart);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmEditSegmentAddr";
            this.Text = "Edit Segment Address";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReadPairs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioStartAbsolute;
        private System.Windows.Forms.RadioButton radioReadStart;
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
    }
}