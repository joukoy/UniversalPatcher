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
            this.radioDwordSum = new System.Windows.Forms.RadioButton();
            this.radioNone = new System.Windows.Forms.RadioButton();
            this.radioWordSum = new System.Windows.Forms.RadioButton();
            this.radioSUM = new System.Windows.Forms.RadioButton();
            this.radioCrc32 = new System.Windows.Forms.RadioButton();
            this.radioCrc16 = new System.Windows.Forms.RadioButton();
            this.txtSegmentName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSegmentAddress = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radio2DwordSum = new System.Windows.Forms.RadioButton();
            this.radio2None = new System.Windows.Forms.RadioButton();
            this.radio2WordSum = new System.Windows.Forms.RadioButton();
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioComplement2 = new System.Windows.Forms.RadioButton();
            this.radioComplement1 = new System.Windows.Forms.RadioButton();
            this.radioComplement0 = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radio2Complement2 = new System.Windows.Forms.RadioButton();
            this.radio2Complement1 = new System.Windows.Forms.RadioButton();
            this.radio2Complement0 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPNAddr = new System.Windows.Forms.TextBox();
            this.txtVerAddr = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtNrAddr = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtCS1Block = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCS2Block = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioDwordSum);
            this.groupBox1.Controls.Add(this.radioNone);
            this.groupBox1.Controls.Add(this.radioWordSum);
            this.groupBox1.Controls.Add(this.radioSUM);
            this.groupBox1.Controls.Add(this.radioCrc32);
            this.groupBox1.Controls.Add(this.radioCrc16);
            this.groupBox1.Location = new System.Drawing.Point(11, 316);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(131, 135);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checksum 1 method";
            // 
            // radioDwordSum
            // 
            this.radioDwordSum.AutoSize = true;
            this.radioDwordSum.Location = new System.Drawing.Point(6, 109);
            this.radioDwordSum.Name = "radioDwordSum";
            this.radioDwordSum.Size = new System.Drawing.Size(80, 17);
            this.radioDwordSum.TabIndex = 15;
            this.radioDwordSum.TabStop = true;
            this.radioDwordSum.Text = "Dword Sum";
            this.radioDwordSum.UseVisualStyleBackColor = true;
            // 
            // radioNone
            // 
            this.radioNone.AutoSize = true;
            this.radioNone.Checked = true;
            this.radioNone.Location = new System.Drawing.Point(6, 19);
            this.radioNone.Name = "radioNone";
            this.radioNone.Size = new System.Drawing.Size(51, 17);
            this.radioNone.TabIndex = 10;
            this.radioNone.TabStop = true;
            this.radioNone.Text = "None";
            this.radioNone.UseVisualStyleBackColor = true;
            // 
            // radioWordSum
            // 
            this.radioWordSum.AutoSize = true;
            this.radioWordSum.Location = new System.Drawing.Point(6, 91);
            this.radioWordSum.Name = "radioWordSum";
            this.radioWordSum.Size = new System.Drawing.Size(75, 17);
            this.radioWordSum.TabIndex = 14;
            this.radioWordSum.Text = "Word Sum";
            this.radioWordSum.UseVisualStyleBackColor = true;
            // 
            // radioSUM
            // 
            this.radioSUM.AutoSize = true;
            this.radioSUM.Location = new System.Drawing.Point(6, 72);
            this.radioSUM.Name = "radioSUM";
            this.radioSUM.Size = new System.Drawing.Size(73, 17);
            this.radioSUM.TabIndex = 13;
            this.radioSUM.Text = "Byte SUM";
            this.radioSUM.UseVisualStyleBackColor = true;
            // 
            // radioCrc32
            // 
            this.radioCrc32.AutoSize = true;
            this.radioCrc32.Location = new System.Drawing.Point(6, 54);
            this.radioCrc32.Name = "radioCrc32";
            this.radioCrc32.Size = new System.Drawing.Size(59, 17);
            this.radioCrc32.TabIndex = 12;
            this.radioCrc32.Text = "CRC32";
            this.radioCrc32.UseVisualStyleBackColor = true;
            // 
            // radioCrc16
            // 
            this.radioCrc16.AutoSize = true;
            this.radioCrc16.Location = new System.Drawing.Point(6, 36);
            this.radioCrc16.Name = "radioCrc16";
            this.radioCrc16.Size = new System.Drawing.Size(59, 17);
            this.radioCrc16.TabIndex = 11;
            this.radioCrc16.Text = "CRC16";
            this.radioCrc16.UseVisualStyleBackColor = true;
            // 
            // txtSegmentName
            // 
            this.txtSegmentName.Location = new System.Drawing.Point(196, 155);
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
            // 
            // txtSegmentAddress
            // 
            this.txtSegmentAddress.Location = new System.Drawing.Point(196, 179);
            this.txtSegmentAddress.Name = "txtSegmentAddress";
            this.txtSegmentAddress.Size = new System.Drawing.Size(190, 20);
            this.txtSegmentAddress.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radio2DwordSum);
            this.groupBox2.Controls.Add(this.radio2None);
            this.groupBox2.Controls.Add(this.radio2WordSum);
            this.groupBox2.Controls.Add(this.radio2SUM);
            this.groupBox2.Controls.Add(this.radio2Crc32);
            this.groupBox2.Controls.Add(this.radio2Crc16);
            this.groupBox2.Location = new System.Drawing.Point(264, 316);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(124, 135);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Checksum 2 method";
            // 
            // radio2DwordSum
            // 
            this.radio2DwordSum.AutoSize = true;
            this.radio2DwordSum.Location = new System.Drawing.Point(6, 109);
            this.radio2DwordSum.Name = "radio2DwordSum";
            this.radio2DwordSum.Size = new System.Drawing.Size(83, 17);
            this.radio2DwordSum.TabIndex = 24;
            this.radio2DwordSum.TabStop = true;
            this.radio2DwordSum.Text = "Dword SUM";
            this.radio2DwordSum.UseVisualStyleBackColor = true;
            // 
            // radio2None
            // 
            this.radio2None.AutoSize = true;
            this.radio2None.Checked = true;
            this.radio2None.Location = new System.Drawing.Point(6, 19);
            this.radio2None.Name = "radio2None";
            this.radio2None.Size = new System.Drawing.Size(51, 17);
            this.radio2None.TabIndex = 19;
            this.radio2None.TabStop = true;
            this.radio2None.Text = "None";
            this.radio2None.UseVisualStyleBackColor = true;
            // 
            // radio2WordSum
            // 
            this.radio2WordSum.AutoSize = true;
            this.radio2WordSum.Location = new System.Drawing.Point(6, 91);
            this.radio2WordSum.Name = "radio2WordSum";
            this.radio2WordSum.Size = new System.Drawing.Size(75, 17);
            this.radio2WordSum.TabIndex = 23;
            this.radio2WordSum.Text = "Word Sum";
            this.radio2WordSum.UseVisualStyleBackColor = true;
            // 
            // radio2SUM
            // 
            this.radio2SUM.AutoSize = true;
            this.radio2SUM.Location = new System.Drawing.Point(6, 71);
            this.radio2SUM.Name = "radio2SUM";
            this.radio2SUM.Size = new System.Drawing.Size(73, 17);
            this.radio2SUM.TabIndex = 22;
            this.radio2SUM.Text = "Byte SUM";
            this.radio2SUM.UseVisualStyleBackColor = true;
            // 
            // radio2Crc32
            // 
            this.radio2Crc32.AutoSize = true;
            this.radio2Crc32.Location = new System.Drawing.Point(6, 53);
            this.radio2Crc32.Name = "radio2Crc32";
            this.radio2Crc32.Size = new System.Drawing.Size(59, 17);
            this.radio2Crc32.TabIndex = 21;
            this.radio2Crc32.Text = "CRC32";
            this.radio2Crc32.UseVisualStyleBackColor = true;
            // 
            // radio2Crc16
            // 
            this.radio2Crc16.AutoSize = true;
            this.radio2Crc16.Location = new System.Drawing.Point(6, 36);
            this.radio2Crc16.Name = "radio2Crc16";
            this.radio2Crc16.Size = new System.Drawing.Size(59, 17);
            this.radio2Crc16.TabIndex = 20;
            this.radio2Crc16.Text = "CRC16";
            this.radio2Crc16.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Checksum 1 Address (HEX):";
            // 
            // txtCSA1
            // 
            this.txtCSA1.Location = new System.Drawing.Point(148, 248);
            this.txtCSA1.Name = "txtCSA1";
            this.txtCSA1.Size = new System.Drawing.Size(71, 20);
            this.txtCSA1.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(315, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Checksum 2 Address (HEX):";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtCSA2
            // 
            this.txtCSA2.Location = new System.Drawing.Point(457, 251);
            this.txtCSA2.Name = "txtCSA2";
            this.txtCSA2.Size = new System.Drawing.Size(72, 20);
            this.txtCSA2.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(368, 457);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 35);
            this.button1.TabIndex = 31;
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
            this.listSegments.SelectedIndexChanged += new System.EventHandler(this.listSegments_SelectedIndexChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(449, 179);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(79, 25);
            this.btnDelete.TabIndex = 29;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(449, 150);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(81, 25);
            this.btnAdd.TabIndex = 28;
            this.btnAdd.Text = "Add/edit";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(288, 457);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(74, 35);
            this.btnLoad.TabIndex = 30;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioComplement2);
            this.groupBox5.Controls.Add(this.radioComplement1);
            this.groupBox5.Controls.Add(this.radioComplement0);
            this.groupBox5.Location = new System.Drawing.Point(126, 316);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(132, 135);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            // 
            // radioComplement2
            // 
            this.radioComplement2.AutoSize = true;
            this.radioComplement2.Location = new System.Drawing.Point(6, 57);
            this.radioComplement2.Name = "radioComplement2";
            this.radioComplement2.Size = new System.Drawing.Size(99, 17);
            this.radioComplement2.TabIndex = 18;
            this.radioComplement2.TabStop = true;
            this.radioComplement2.Text = "2\'s Complement";
            this.radioComplement2.UseVisualStyleBackColor = true;
            // 
            // radioComplement1
            // 
            this.radioComplement1.AutoSize = true;
            this.radioComplement1.Location = new System.Drawing.Point(6, 36);
            this.radioComplement1.Name = "radioComplement1";
            this.radioComplement1.Size = new System.Drawing.Size(99, 17);
            this.radioComplement1.TabIndex = 17;
            this.radioComplement1.TabStop = true;
            this.radioComplement1.Text = "1\'s Complement";
            this.radioComplement1.UseVisualStyleBackColor = true;
            // 
            // radioComplement0
            // 
            this.radioComplement0.AutoSize = true;
            this.radioComplement0.Checked = true;
            this.radioComplement0.Location = new System.Drawing.Point(6, 16);
            this.radioComplement0.Name = "radioComplement0";
            this.radioComplement0.Size = new System.Drawing.Size(28, 17);
            this.radioComplement0.TabIndex = 16;
            this.radioComplement0.TabStop = true;
            this.radioComplement0.Text = "-";
            this.radioComplement0.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radio2Complement2);
            this.groupBox6.Controls.Add(this.radio2Complement1);
            this.groupBox6.Controls.Add(this.radio2Complement0);
            this.groupBox6.Location = new System.Drawing.Point(383, 316);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(148, 135);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            // 
            // radio2Complement2
            // 
            this.radio2Complement2.AutoSize = true;
            this.radio2Complement2.Location = new System.Drawing.Point(6, 56);
            this.radio2Complement2.Name = "radio2Complement2";
            this.radio2Complement2.Size = new System.Drawing.Size(99, 17);
            this.radio2Complement2.TabIndex = 27;
            this.radio2Complement2.TabStop = true;
            this.radio2Complement2.Text = "2\'s Complement";
            this.radio2Complement2.UseVisualStyleBackColor = true;
            // 
            // radio2Complement1
            // 
            this.radio2Complement1.AutoSize = true;
            this.radio2Complement1.Location = new System.Drawing.Point(6, 36);
            this.radio2Complement1.Name = "radio2Complement1";
            this.radio2Complement1.Size = new System.Drawing.Size(99, 17);
            this.radio2Complement1.TabIndex = 26;
            this.radio2Complement1.TabStop = true;
            this.radio2Complement1.Text = "1\'s Complement";
            this.radio2Complement1.UseVisualStyleBackColor = true;
            // 
            // radio2Complement0
            // 
            this.radio2Complement0.AutoSize = true;
            this.radio2Complement0.Checked = true;
            this.radio2Complement0.Location = new System.Drawing.Point(6, 19);
            this.radio2Complement0.Name = "radio2Complement0";
            this.radio2Complement0.Size = new System.Drawing.Size(28, 17);
            this.radio2Complement0.TabIndex = 25;
            this.radio2Complement0.TabStop = true;
            this.radio2Complement0.Text = "-";
            this.radio2Complement0.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 277);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Segment PN address";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(315, 280);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Segment Ver address";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // txtPNAddr
            // 
            this.txtPNAddr.Location = new System.Drawing.Point(148, 274);
            this.txtPNAddr.Name = "txtPNAddr";
            this.txtPNAddr.Size = new System.Drawing.Size(71, 20);
            this.txtPNAddr.TabIndex = 7;
            // 
            // txtVerAddr
            // 
            this.txtVerAddr.Location = new System.Drawing.Point(457, 277);
            this.txtVerAddr.Name = "txtVerAddr";
            this.txtVerAddr.Size = new System.Drawing.Size(72, 20);
            this.txtVerAddr.TabIndex = 8;
            this.txtVerAddr.TextChanged += new System.EventHandler(this.txtVerAddr_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 302);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(127, 13);
            this.label12.TabIndex = 31;
            this.label12.Text = "Segment number address";
            // 
            // txtNrAddr
            // 
            this.txtNrAddr.Location = new System.Drawing.Point(148, 299);
            this.txtNrAddr.Name = "txtNrAddr";
            this.txtNrAddr.Size = new System.Drawing.Size(72, 20);
            this.txtNrAddr.TabIndex = 9;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(450, 457);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 34);
            this.btnOK.TabIndex = 33;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtCS1Block
            // 
            this.txtCS1Block.Location = new System.Drawing.Point(196, 202);
            this.txtCS1Block.Name = "txtCS1Block";
            this.txtCS1Block.Size = new System.Drawing.Size(190, 20);
            this.txtCS1Block.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Checksum 1 calculation addresses:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 228);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(174, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Checksum 2 calculation addresses:";
            // 
            // txtCS2Block
            // 
            this.txtCS2Block.Location = new System.Drawing.Point(196, 225);
            this.txtCS2Block.Name = "txtCS2Block";
            this.txtCS2Block.Size = new System.Drawing.Size(190, 20);
            this.txtCS2Block.TabIndex = 4;
            // 
            // frmSegmentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 502);
            this.Controls.Add(this.txtCS2Block);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCS1Block);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtNrAddr);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtVerAddr);
            this.Controls.Add(this.txtPNAddr);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.listSegments);
            this.Controls.Add(this.txtCSA2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCSA1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtSegmentAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSegmentName);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmSegmentSettings";
            this.Text = "Segment settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSegmentSettings_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioWordSum;
        private System.Windows.Forms.RadioButton radioSUM;
        private System.Windows.Forms.RadioButton radioCrc32;
        private System.Windows.Forms.RadioButton radioCrc16;
        private System.Windows.Forms.TextBox txtSegmentName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSegmentAddress;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radio2WordSum;
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
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.ListView listSegments;
        private System.Windows.Forms.RadioButton radioDwordSum;
        private System.Windows.Forms.RadioButton radio2DwordSum;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioComplement2;
        private System.Windows.Forms.RadioButton radioComplement1;
        private System.Windows.Forms.RadioButton radioComplement0;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radio2Complement2;
        private System.Windows.Forms.RadioButton radio2Complement1;
        private System.Windows.Forms.RadioButton radio2Complement0;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPNAddr;
        private System.Windows.Forms.TextBox txtVerAddr;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtNrAddr;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtCS1Block;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCS2Block;
    }
}