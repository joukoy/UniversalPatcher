namespace UniversalPatcher
{
    partial class frmManualPatch
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
            this.txtReadAddr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numCheckBit = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numCheckValue = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtXML = new System.Windows.Forms.TextBox();
            this.txtCompOS = new System.Windows.Forms.TextBox();
            this.txtData = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtSegment = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckBit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckValue)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Read byte at address (HEX):";
            // 
            // txtReadAddr
            // 
            this.txtReadAddr.Location = new System.Drawing.Point(171, 26);
            this.txtReadAddr.Name = "txtReadAddr";
            this.txtReadAddr.Size = new System.Drawing.Size(86, 20);
            this.txtReadAddr.TabIndex = 60;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Read bit:";
            // 
            // numCheckBit
            // 
            this.numCheckBit.Location = new System.Drawing.Point(226, 52);
            this.numCheckBit.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numCheckBit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCheckBit.Name = "numCheckBit";
            this.numCheckBit.Size = new System.Drawing.Size(33, 20);
            this.numCheckBit.TabIndex = 70;
            this.numCheckBit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Apply data, if bit is:";
            // 
            // numCheckValue
            // 
            this.numCheckValue.Location = new System.Drawing.Point(226, 77);
            this.numCheckValue.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCheckValue.Name = "numCheckValue";
            this.numCheckValue.Size = new System.Drawing.Size(33, 20);
            this.numCheckValue.TabIndex = 80;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Patch description:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Compatible with XML file:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 107);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "OS:Address:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Data:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(155, 13);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(271, 20);
            this.txtDescription.TabIndex = 10;
            // 
            // txtXML
            // 
            this.txtXML.Location = new System.Drawing.Point(161, 81);
            this.txtXML.Name = "txtXML";
            this.txtXML.Size = new System.Drawing.Size(271, 20);
            this.txtXML.TabIndex = 20;
            // 
            // txtCompOS
            // 
            this.txtCompOS.Location = new System.Drawing.Point(161, 107);
            this.txtCompOS.Name = "txtCompOS";
            this.txtCompOS.Size = new System.Drawing.Size(271, 20);
            this.txtCompOS.TabIndex = 30;
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(161, 131);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(271, 20);
            this.txtData.TabIndex = 40;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtReadAddr);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numCheckBit);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numCheckValue);
            this.groupBox1.Location = new System.Drawing.Point(2, 182);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 103);
            this.groupBox1.TabIndex = 50;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rule";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(366, 248);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(67, 31);
            this.btnOK.TabIndex = 90;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(157, 154);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 91;
            this.label8.Text = "Example: 2C FF 5B";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(157, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(135, 13);
            this.label9.TabIndex = 92;
            this.label9.Text = "Example: 5:0 (set bit 5 to 0)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 93;
            this.label10.Text = "Segment:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtSegment);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtDescription);
            this.groupBox2.Location = new System.Drawing.Point(6, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(427, 74);
            this.groupBox2.TabIndex = 94;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Optional";
            // 
            // txtSegment
            // 
            this.txtSegment.Location = new System.Drawing.Point(155, 40);
            this.txtSegment.Name = "txtSegment";
            this.txtSegment.Size = new System.Drawing.Size(271, 20);
            this.txtSegment.TabIndex = 11;
            // 
            // frmManualPatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 292);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.txtCompOS);
            this.Controls.Add(this.txtXML);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Name = "frmManualPatch";
            this.Text = "Manual patch";
            ((System.ComponentModel.ISupportInitialize)(this.numCheckBit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCheckValue)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.NumericUpDown numCheckValue;
        public System.Windows.Forms.TextBox txtDescription;
        public System.Windows.Forms.TextBox txtXML;
        public System.Windows.Forms.TextBox txtCompOS;
        public System.Windows.Forms.TextBox txtData;
        public System.Windows.Forms.TextBox txtReadAddr;
        public System.Windows.Forms.NumericUpDown numCheckBit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.TextBox txtSegment;
    }
}