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
            this.radioCS1DwordSum = new System.Windows.Forms.RadioButton();
            this.radioCS1None = new System.Windows.Forms.RadioButton();
            this.radioCS1WordSum = new System.Windows.Forms.RadioButton();
            this.radioCS1SUM = new System.Windows.Forms.RadioButton();
            this.radioCS1Crc32 = new System.Windows.Forms.RadioButton();
            this.radioCS1Crc16 = new System.Windows.Forms.RadioButton();
            this.txtSegmentName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSegmentAddress = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioCS2DwordSum = new System.Windows.Forms.RadioButton();
            this.radioCS2None = new System.Windows.Forms.RadioButton();
            this.radioCS2WordSum = new System.Windows.Forms.RadioButton();
            this.radioCS2SUM = new System.Windows.Forms.RadioButton();
            this.radioCS2Crc32 = new System.Windows.Forms.RadioButton();
            this.radioCS2Crc16 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCS1Address = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCS2Address = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioCS1Complement2 = new System.Windows.Forms.RadioButton();
            this.radioCS1Complement1 = new System.Windows.Forms.RadioButton();
            this.radioCS1Complement0 = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radioCS2Complement2 = new System.Windows.Forms.RadioButton();
            this.radioCS2Complement1 = new System.Windows.Forms.RadioButton();
            this.radioCS2Complement0 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtPNAddr = new System.Windows.Forms.TextBox();
            this.txtVerAddr = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtNrAddr = new System.Windows.Forms.TextBox();
            this.txtCS1Block = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCS2Block = new System.Windows.Forms.TextBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.checkSwapBytes1 = new System.Windows.Forms.CheckBox();
            this.checkSwapBytes2 = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtExtrainfo = new System.Windows.Forms.TextBox();
            this.chkEeprom = new System.Windows.Forms.CheckBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.labelComment = new System.Windows.Forms.Label();
            this.btnEditSegmentAddr = new System.Windows.Forms.Button();
            this.btnCs1Block = new System.Windows.Forms.Button();
            this.btnCs2block = new System.Windows.Forms.Button();
            this.btnSegNrAddr = new System.Windows.Forms.Button();
            this.btnPNAddr = new System.Windows.Forms.Button();
            this.btnCS1Addr = new System.Windows.Forms.Button();
            this.btnExtraAddr = new System.Windows.Forms.Button();
            this.btnVerAddr = new System.Windows.Forms.Button();
            this.btnCS2Addr = new System.Windows.Forms.Button();
            this.labelXML = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioCS1DwordSum);
            this.groupBox1.Controls.Add(this.radioCS1None);
            this.groupBox1.Controls.Add(this.radioCS1WordSum);
            this.groupBox1.Controls.Add(this.radioCS1SUM);
            this.groupBox1.Controls.Add(this.radioCS1Crc32);
            this.groupBox1.Controls.Add(this.radioCS1Crc16);
            this.groupBox1.Location = new System.Drawing.Point(11, 210);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(122, 135);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Checksum 1 method";
            // 
            // radioCS1DwordSum
            // 
            this.radioCS1DwordSum.AutoSize = true;
            this.radioCS1DwordSum.Location = new System.Drawing.Point(6, 109);
            this.radioCS1DwordSum.Name = "radioCS1DwordSum";
            this.radioCS1DwordSum.Size = new System.Drawing.Size(80, 17);
            this.radioCS1DwordSum.TabIndex = 15;
            this.radioCS1DwordSum.TabStop = true;
            this.radioCS1DwordSum.Text = "Dword Sum";
            this.radioCS1DwordSum.UseVisualStyleBackColor = true;
            // 
            // radioCS1None
            // 
            this.radioCS1None.AutoSize = true;
            this.radioCS1None.Checked = true;
            this.radioCS1None.Location = new System.Drawing.Point(6, 19);
            this.radioCS1None.Name = "radioCS1None";
            this.radioCS1None.Size = new System.Drawing.Size(51, 17);
            this.radioCS1None.TabIndex = 10;
            this.radioCS1None.TabStop = true;
            this.radioCS1None.Text = "None";
            this.radioCS1None.UseVisualStyleBackColor = true;
            // 
            // radioCS1WordSum
            // 
            this.radioCS1WordSum.AutoSize = true;
            this.radioCS1WordSum.Location = new System.Drawing.Point(6, 91);
            this.radioCS1WordSum.Name = "radioCS1WordSum";
            this.radioCS1WordSum.Size = new System.Drawing.Size(75, 17);
            this.radioCS1WordSum.TabIndex = 14;
            this.radioCS1WordSum.Text = "Word Sum";
            this.radioCS1WordSum.UseVisualStyleBackColor = true;
            // 
            // radioCS1SUM
            // 
            this.radioCS1SUM.AutoSize = true;
            this.radioCS1SUM.Location = new System.Drawing.Point(6, 72);
            this.radioCS1SUM.Name = "radioCS1SUM";
            this.radioCS1SUM.Size = new System.Drawing.Size(73, 17);
            this.radioCS1SUM.TabIndex = 13;
            this.radioCS1SUM.Text = "Byte SUM";
            this.radioCS1SUM.UseVisualStyleBackColor = true;
            // 
            // radioCS1Crc32
            // 
            this.radioCS1Crc32.AutoSize = true;
            this.radioCS1Crc32.Location = new System.Drawing.Point(6, 54);
            this.radioCS1Crc32.Name = "radioCS1Crc32";
            this.radioCS1Crc32.Size = new System.Drawing.Size(59, 17);
            this.radioCS1Crc32.TabIndex = 12;
            this.radioCS1Crc32.Text = "CRC32";
            this.radioCS1Crc32.UseVisualStyleBackColor = true;
            // 
            // radioCS1Crc16
            // 
            this.radioCS1Crc16.AutoSize = true;
            this.radioCS1Crc16.Location = new System.Drawing.Point(6, 36);
            this.radioCS1Crc16.Name = "radioCS1Crc16";
            this.radioCS1Crc16.Size = new System.Drawing.Size(59, 17);
            this.radioCS1Crc16.TabIndex = 11;
            this.radioCS1Crc16.Text = "CRC16";
            this.radioCS1Crc16.UseVisualStyleBackColor = true;
            // 
            // txtSegmentName
            // 
            this.txtSegmentName.Location = new System.Drawing.Point(196, 41);
            this.txtSegmentName.Name = "txtSegmentName";
            this.txtSegmentName.Size = new System.Drawing.Size(190, 20);
            this.txtSegmentName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Segment name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Segment addresses (HEX):";
            // 
            // txtSegmentAddress
            // 
            this.txtSegmentAddress.Location = new System.Drawing.Point(196, 65);
            this.txtSegmentAddress.Name = "txtSegmentAddress";
            this.txtSegmentAddress.Size = new System.Drawing.Size(190, 20);
            this.txtSegmentAddress.TabIndex = 2;
            this.txtSegmentAddress.DoubleClick += new System.EventHandler(this.txtSegmentAddress_DoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioCS2DwordSum);
            this.groupBox2.Controls.Add(this.radioCS2None);
            this.groupBox2.Controls.Add(this.radioCS2WordSum);
            this.groupBox2.Controls.Add(this.radioCS2SUM);
            this.groupBox2.Controls.Add(this.radioCS2Crc32);
            this.groupBox2.Controls.Add(this.radioCS2Crc16);
            this.groupBox2.Location = new System.Drawing.Point(264, 210);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(124, 135);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Checksum 2 method";
            // 
            // radioCS2DwordSum
            // 
            this.radioCS2DwordSum.AutoSize = true;
            this.radioCS2DwordSum.Location = new System.Drawing.Point(6, 109);
            this.radioCS2DwordSum.Name = "radioCS2DwordSum";
            this.radioCS2DwordSum.Size = new System.Drawing.Size(83, 17);
            this.radioCS2DwordSum.TabIndex = 24;
            this.radioCS2DwordSum.TabStop = true;
            this.radioCS2DwordSum.Text = "Dword SUM";
            this.radioCS2DwordSum.UseVisualStyleBackColor = true;
            // 
            // radioCS2None
            // 
            this.radioCS2None.AutoSize = true;
            this.radioCS2None.Checked = true;
            this.radioCS2None.Location = new System.Drawing.Point(6, 19);
            this.radioCS2None.Name = "radioCS2None";
            this.radioCS2None.Size = new System.Drawing.Size(51, 17);
            this.radioCS2None.TabIndex = 19;
            this.radioCS2None.TabStop = true;
            this.radioCS2None.Text = "None";
            this.radioCS2None.UseVisualStyleBackColor = true;
            // 
            // radioCS2WordSum
            // 
            this.radioCS2WordSum.AutoSize = true;
            this.radioCS2WordSum.Location = new System.Drawing.Point(6, 91);
            this.radioCS2WordSum.Name = "radioCS2WordSum";
            this.radioCS2WordSum.Size = new System.Drawing.Size(75, 17);
            this.radioCS2WordSum.TabIndex = 23;
            this.radioCS2WordSum.Text = "Word Sum";
            this.radioCS2WordSum.UseVisualStyleBackColor = true;
            // 
            // radioCS2SUM
            // 
            this.radioCS2SUM.AutoSize = true;
            this.radioCS2SUM.Location = new System.Drawing.Point(6, 71);
            this.radioCS2SUM.Name = "radioCS2SUM";
            this.radioCS2SUM.Size = new System.Drawing.Size(73, 17);
            this.radioCS2SUM.TabIndex = 22;
            this.radioCS2SUM.Text = "Byte SUM";
            this.radioCS2SUM.UseVisualStyleBackColor = true;
            // 
            // radioCS2Crc32
            // 
            this.radioCS2Crc32.AutoSize = true;
            this.radioCS2Crc32.Location = new System.Drawing.Point(6, 53);
            this.radioCS2Crc32.Name = "radioCS2Crc32";
            this.radioCS2Crc32.Size = new System.Drawing.Size(59, 17);
            this.radioCS2Crc32.TabIndex = 21;
            this.radioCS2Crc32.Text = "CRC32";
            this.radioCS2Crc32.UseVisualStyleBackColor = true;
            // 
            // radioCS2Crc16
            // 
            this.radioCS2Crc16.AutoSize = true;
            this.radioCS2Crc16.Location = new System.Drawing.Point(6, 36);
            this.radioCS2Crc16.Name = "radioCS2Crc16";
            this.radioCS2Crc16.Size = new System.Drawing.Size(59, 17);
            this.radioCS2Crc16.TabIndex = 20;
            this.radioCS2Crc16.Text = "CRC16";
            this.radioCS2Crc16.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Checksum 1 Address (HEX):";
            // 
            // txtCS1Address
            // 
            this.txtCS1Address.Location = new System.Drawing.Point(148, 134);
            this.txtCS1Address.Name = "txtCS1Address";
            this.txtCS1Address.Size = new System.Drawing.Size(86, 20);
            this.txtCS1Address.TabIndex = 5;
            this.txtCS1Address.DoubleClick += new System.EventHandler(this.txtCS1Address_Doubleclick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(272, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Checksum 2 Address (HEX):";
            // 
            // txtCS2Address
            // 
            this.txtCS2Address.Location = new System.Drawing.Point(411, 137);
            this.txtCS2Address.Name = "txtCS2Address";
            this.txtCS2Address.Size = new System.Drawing.Size(86, 20);
            this.txtCS2Address.TabIndex = 6;
            this.txtCS2Address.DoubleClick += new System.EventHandler(this.txtCS2Address_Doubleclick);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(450, 74);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(77, 25);
            this.btnApply.TabIndex = 28;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioCS1Complement2);
            this.groupBox5.Controls.Add(this.radioCS1Complement1);
            this.groupBox5.Controls.Add(this.radioCS1Complement0);
            this.groupBox5.Location = new System.Drawing.Point(139, 210);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(119, 88);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            // 
            // radioCS1Complement2
            // 
            this.radioCS1Complement2.AutoSize = true;
            this.radioCS1Complement2.Location = new System.Drawing.Point(6, 57);
            this.radioCS1Complement2.Name = "radioCS1Complement2";
            this.radioCS1Complement2.Size = new System.Drawing.Size(99, 17);
            this.radioCS1Complement2.TabIndex = 18;
            this.radioCS1Complement2.TabStop = true;
            this.radioCS1Complement2.Text = "2\'s Complement";
            this.radioCS1Complement2.UseVisualStyleBackColor = true;
            // 
            // radioCS1Complement1
            // 
            this.radioCS1Complement1.AutoSize = true;
            this.radioCS1Complement1.Location = new System.Drawing.Point(6, 36);
            this.radioCS1Complement1.Name = "radioCS1Complement1";
            this.radioCS1Complement1.Size = new System.Drawing.Size(99, 17);
            this.radioCS1Complement1.TabIndex = 17;
            this.radioCS1Complement1.TabStop = true;
            this.radioCS1Complement1.Text = "1\'s Complement";
            this.radioCS1Complement1.UseVisualStyleBackColor = true;
            // 
            // radioCS1Complement0
            // 
            this.radioCS1Complement0.AutoSize = true;
            this.radioCS1Complement0.Checked = true;
            this.radioCS1Complement0.Location = new System.Drawing.Point(6, 16);
            this.radioCS1Complement0.Name = "radioCS1Complement0";
            this.radioCS1Complement0.Size = new System.Drawing.Size(28, 17);
            this.radioCS1Complement0.TabIndex = 16;
            this.radioCS1Complement0.TabStop = true;
            this.radioCS1Complement0.Text = "-";
            this.radioCS1Complement0.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radioCS2Complement2);
            this.groupBox6.Controls.Add(this.radioCS2Complement1);
            this.groupBox6.Controls.Add(this.radioCS2Complement0);
            this.groupBox6.Location = new System.Drawing.Point(392, 210);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(139, 88);
            this.groupBox6.TabIndex = 20;
            this.groupBox6.TabStop = false;
            // 
            // radioCS2Complement2
            // 
            this.radioCS2Complement2.AutoSize = true;
            this.radioCS2Complement2.Location = new System.Drawing.Point(6, 56);
            this.radioCS2Complement2.Name = "radioCS2Complement2";
            this.radioCS2Complement2.Size = new System.Drawing.Size(99, 17);
            this.radioCS2Complement2.TabIndex = 27;
            this.radioCS2Complement2.TabStop = true;
            this.radioCS2Complement2.Text = "2\'s Complement";
            this.radioCS2Complement2.UseVisualStyleBackColor = true;
            // 
            // radioCS2Complement1
            // 
            this.radioCS2Complement1.AutoSize = true;
            this.radioCS2Complement1.Location = new System.Drawing.Point(6, 36);
            this.radioCS2Complement1.Name = "radioCS2Complement1";
            this.radioCS2Complement1.Size = new System.Drawing.Size(99, 17);
            this.radioCS2Complement1.TabIndex = 26;
            this.radioCS2Complement1.TabStop = true;
            this.radioCS2Complement1.Text = "1\'s Complement";
            this.radioCS2Complement1.UseVisualStyleBackColor = true;
            // 
            // radioCS2Complement0
            // 
            this.radioCS2Complement0.AutoSize = true;
            this.radioCS2Complement0.Checked = true;
            this.radioCS2Complement0.Location = new System.Drawing.Point(6, 19);
            this.radioCS2Complement0.Name = "radioCS2Complement0";
            this.radioCS2Complement0.Size = new System.Drawing.Size(28, 17);
            this.radioCS2Complement0.TabIndex = 25;
            this.radioCS2Complement0.TabStop = true;
            this.radioCS2Complement0.Text = "-";
            this.radioCS2Complement0.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Segment PN address";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(272, 162);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Segment Ver address";
            // 
            // txtPNAddr
            // 
            this.txtPNAddr.Location = new System.Drawing.Point(148, 156);
            this.txtPNAddr.Name = "txtPNAddr";
            this.txtPNAddr.Size = new System.Drawing.Size(86, 20);
            this.txtPNAddr.TabIndex = 7;
            this.txtPNAddr.DoubleClick += new System.EventHandler(this.txtPNAddr_Doubleclick);
            // 
            // txtVerAddr
            // 
            this.txtVerAddr.Location = new System.Drawing.Point(411, 159);
            this.txtVerAddr.Name = "txtVerAddr";
            this.txtVerAddr.Size = new System.Drawing.Size(86, 20);
            this.txtVerAddr.TabIndex = 8;
            this.txtVerAddr.DoubleClick += new System.EventHandler(this.txtVerAddr_Doubleclick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 183);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(127, 13);
            this.label12.TabIndex = 31;
            this.label12.Text = "Segment number address";
            // 
            // txtNrAddr
            // 
            this.txtNrAddr.Location = new System.Drawing.Point(148, 179);
            this.txtNrAddr.Name = "txtNrAddr";
            this.txtNrAddr.Size = new System.Drawing.Size(86, 20);
            this.txtNrAddr.TabIndex = 9;
            this.txtNrAddr.DoubleClick += new System.EventHandler(this.txtNrAddr_Doubleclick);
            // 
            // txtCS1Block
            // 
            this.txtCS1Block.Location = new System.Drawing.Point(196, 88);
            this.txtCS1Block.Name = "txtCS1Block";
            this.txtCS1Block.Size = new System.Drawing.Size(190, 20);
            this.txtCS1Block.TabIndex = 3;
            this.txtCS1Block.DoubleClick += new System.EventHandler(this.txtCS1Block_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Checksum 1 calculation addresses:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(174, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Checksum 2 calculation addresses:";
            // 
            // txtCS2Block
            // 
            this.txtCS2Block.Location = new System.Drawing.Point(196, 111);
            this.txtCS2Block.Name = "txtCS2Block";
            this.txtCS2Block.Size = new System.Drawing.Size(190, 20);
            this.txtCS2Block.TabIndex = 4;
            this.txtCS2Block.DoubleClick += new System.EventHandler(this.txtCS2Block_DoubleClick);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(450, 104);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 27);
            this.btnHelp.TabIndex = 37;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // checkSwapBytes1
            // 
            this.checkSwapBytes1.AutoSize = true;
            this.checkSwapBytes1.Location = new System.Drawing.Point(145, 301);
            this.checkSwapBytes1.Name = "checkSwapBytes1";
            this.checkSwapBytes1.Size = new System.Drawing.Size(81, 17);
            this.checkSwapBytes1.TabIndex = 16;
            this.checkSwapBytes1.Text = "Swap bytes";
            this.checkSwapBytes1.UseVisualStyleBackColor = true;
            // 
            // checkSwapBytes2
            // 
            this.checkSwapBytes2.AutoSize = true;
            this.checkSwapBytes2.Location = new System.Drawing.Point(398, 304);
            this.checkSwapBytes2.Name = "checkSwapBytes2";
            this.checkSwapBytes2.Size = new System.Drawing.Size(82, 17);
            this.checkSwapBytes2.TabIndex = 39;
            this.checkSwapBytes2.Text = "Swap Bytes";
            this.checkSwapBytes2.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(450, 42);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 26);
            this.btnOK.TabIndex = 40;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(272, 183);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "Extra info:";
            // 
            // txtExtrainfo
            // 
            this.txtExtrainfo.Location = new System.Drawing.Point(411, 182);
            this.txtExtrainfo.Name = "txtExtrainfo";
            this.txtExtrainfo.Size = new System.Drawing.Size(86, 20);
            this.txtExtrainfo.TabIndex = 42;
            this.txtExtrainfo.DoubleClick += new System.EventHandler(this.txtExtrainfo_Doubleclick);
            // 
            // chkEeprom
            // 
            this.chkEeprom.AutoSize = true;
            this.chkEeprom.Location = new System.Drawing.Point(13, 351);
            this.chkEeprom.Name = "chkEeprom";
            this.chkEeprom.Size = new System.Drawing.Size(271, 17);
            this.chkEeprom.TabIndex = 43;
            this.chkEeprom.Text = "P01 or P59 Eeprom segment (other settings ignored)";
            this.chkEeprom.UseVisualStyleBackColor = true;
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(9, 391);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(522, 20);
            this.txtComment.TabIndex = 44;
            // 
            // labelComment
            // 
            this.labelComment.AutoSize = true;
            this.labelComment.Location = new System.Drawing.Point(6, 371);
            this.labelComment.Name = "labelComment";
            this.labelComment.Size = new System.Drawing.Size(59, 13);
            this.labelComment.TabIndex = 45;
            this.labelComment.Text = "Comments:";
            // 
            // btnEditSegmentAddr
            // 
            this.btnEditSegmentAddr.Location = new System.Drawing.Point(386, 65);
            this.btnEditSegmentAddr.Name = "btnEditSegmentAddr";
            this.btnEditSegmentAddr.Size = new System.Drawing.Size(27, 20);
            this.btnEditSegmentAddr.TabIndex = 46;
            this.btnEditSegmentAddr.Text = "..";
            this.btnEditSegmentAddr.UseVisualStyleBackColor = true;
            this.btnEditSegmentAddr.Click += new System.EventHandler(this.btnEditSegmentAddr_Click);
            // 
            // btnCs1Block
            // 
            this.btnCs1Block.Location = new System.Drawing.Point(386, 88);
            this.btnCs1Block.Name = "btnCs1Block";
            this.btnCs1Block.Size = new System.Drawing.Size(27, 20);
            this.btnCs1Block.TabIndex = 47;
            this.btnCs1Block.Text = "..";
            this.btnCs1Block.UseVisualStyleBackColor = true;
            this.btnCs1Block.Click += new System.EventHandler(this.btnCs1Block_Click);
            // 
            // btnCs2block
            // 
            this.btnCs2block.Location = new System.Drawing.Point(386, 111);
            this.btnCs2block.Name = "btnCs2block";
            this.btnCs2block.Size = new System.Drawing.Size(27, 20);
            this.btnCs2block.TabIndex = 48;
            this.btnCs2block.Text = "..";
            this.btnCs2block.UseVisualStyleBackColor = true;
            this.btnCs2block.Click += new System.EventHandler(this.btnCs2block_Click);
            // 
            // btnSegNrAddr
            // 
            this.btnSegNrAddr.Location = new System.Drawing.Point(234, 179);
            this.btnSegNrAddr.Name = "btnSegNrAddr";
            this.btnSegNrAddr.Size = new System.Drawing.Size(27, 20);
            this.btnSegNrAddr.TabIndex = 51;
            this.btnSegNrAddr.Text = "..";
            this.btnSegNrAddr.UseVisualStyleBackColor = true;
            this.btnSegNrAddr.Click += new System.EventHandler(this.btnSegNrAddr_Click);
            // 
            // btnPNAddr
            // 
            this.btnPNAddr.Location = new System.Drawing.Point(234, 156);
            this.btnPNAddr.Name = "btnPNAddr";
            this.btnPNAddr.Size = new System.Drawing.Size(27, 20);
            this.btnPNAddr.TabIndex = 50;
            this.btnPNAddr.Text = "..";
            this.btnPNAddr.UseVisualStyleBackColor = true;
            this.btnPNAddr.Click += new System.EventHandler(this.btnPNAddr_Click);
            // 
            // btnCS1Addr
            // 
            this.btnCS1Addr.Location = new System.Drawing.Point(234, 134);
            this.btnCS1Addr.Name = "btnCS1Addr";
            this.btnCS1Addr.Size = new System.Drawing.Size(27, 20);
            this.btnCS1Addr.TabIndex = 49;
            this.btnCS1Addr.Text = "..";
            this.btnCS1Addr.UseVisualStyleBackColor = true;
            this.btnCS1Addr.Click += new System.EventHandler(this.btnCS1Addr_Click);
            // 
            // btnExtraAddr
            // 
            this.btnExtraAddr.Location = new System.Drawing.Point(498, 181);
            this.btnExtraAddr.Name = "btnExtraAddr";
            this.btnExtraAddr.Size = new System.Drawing.Size(27, 20);
            this.btnExtraAddr.TabIndex = 54;
            this.btnExtraAddr.Text = "..";
            this.btnExtraAddr.UseVisualStyleBackColor = true;
            this.btnExtraAddr.Click += new System.EventHandler(this.btnExtraAddr_Click);
            // 
            // btnVerAddr
            // 
            this.btnVerAddr.Location = new System.Drawing.Point(498, 158);
            this.btnVerAddr.Name = "btnVerAddr";
            this.btnVerAddr.Size = new System.Drawing.Size(27, 20);
            this.btnVerAddr.TabIndex = 53;
            this.btnVerAddr.Text = "..";
            this.btnVerAddr.UseVisualStyleBackColor = true;
            this.btnVerAddr.Click += new System.EventHandler(this.btnVerAddr_Click);
            // 
            // btnCS2Addr
            // 
            this.btnCS2Addr.Location = new System.Drawing.Point(498, 136);
            this.btnCS2Addr.Name = "btnCS2Addr";
            this.btnCS2Addr.Size = new System.Drawing.Size(27, 20);
            this.btnCS2Addr.TabIndex = 52;
            this.btnCS2Addr.Text = "..";
            this.btnCS2Addr.UseVisualStyleBackColor = true;
            this.btnCS2Addr.Click += new System.EventHandler(this.btnCS2Addr_Click);
            // 
            // labelXML
            // 
            this.labelXML.AutoSize = true;
            this.labelXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXML.Location = new System.Drawing.Point(8, 9);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(13, 16);
            this.labelXML.TabIndex = 55;
            this.labelXML.Text = "-";
            // 
            // frmSegmentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 421);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.btnExtraAddr);
            this.Controls.Add(this.btnVerAddr);
            this.Controls.Add(this.btnCS2Addr);
            this.Controls.Add(this.btnSegNrAddr);
            this.Controls.Add(this.btnPNAddr);
            this.Controls.Add(this.btnCS1Addr);
            this.Controls.Add(this.btnCs2block);
            this.Controls.Add(this.btnCs1Block);
            this.Controls.Add(this.btnEditSegmentAddr);
            this.Controls.Add(this.labelComment);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.chkEeprom);
            this.Controls.Add(this.txtExtrainfo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.checkSwapBytes2);
            this.Controls.Add(this.checkSwapBytes1);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.txtCS2Block);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCS1Block);
            this.Controls.Add(this.txtNrAddr);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtVerAddr);
            this.Controls.Add(this.txtPNAddr);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtCS2Address);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtCS1Address);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtSegmentAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSegmentName);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmSegmentSettings";
            this.Text = "Segment settings";
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
        private System.Windows.Forms.RadioButton radioCS1WordSum;
        private System.Windows.Forms.RadioButton radioCS1SUM;
        private System.Windows.Forms.RadioButton radioCS1Crc32;
        private System.Windows.Forms.RadioButton radioCS1Crc16;
        private System.Windows.Forms.TextBox txtSegmentName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSegmentAddress;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioCS2WordSum;
        private System.Windows.Forms.RadioButton radioCS2SUM;
        private System.Windows.Forms.RadioButton radioCS2Crc32;
        private System.Windows.Forms.RadioButton radioCS2Crc16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCS1Address;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCS2Address;
        private System.Windows.Forms.RadioButton radioCS1None;
        private System.Windows.Forms.RadioButton radioCS2None;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.RadioButton radioCS1DwordSum;
        private System.Windows.Forms.RadioButton radioCS2DwordSum;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioCS1Complement2;
        private System.Windows.Forms.RadioButton radioCS1Complement1;
        private System.Windows.Forms.RadioButton radioCS1Complement0;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radioCS2Complement2;
        private System.Windows.Forms.RadioButton radioCS2Complement1;
        private System.Windows.Forms.RadioButton radioCS2Complement0;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPNAddr;
        private System.Windows.Forms.TextBox txtVerAddr;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtNrAddr;
        private System.Windows.Forms.TextBox txtCS1Block;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCS2Block;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.CheckBox checkSwapBytes1;
        private System.Windows.Forms.CheckBox checkSwapBytes2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtExtrainfo;
        private System.Windows.Forms.CheckBox chkEeprom;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label labelComment;
        private System.Windows.Forms.Button btnEditSegmentAddr;
        private System.Windows.Forms.Button btnCs1Block;
        private System.Windows.Forms.Button btnCs2block;
        private System.Windows.Forms.Button btnSegNrAddr;
        private System.Windows.Forms.Button btnPNAddr;
        private System.Windows.Forms.Button btnCS1Addr;
        private System.Windows.Forms.Button btnExtraAddr;
        private System.Windows.Forms.Button btnVerAddr;
        private System.Windows.Forms.Button btnCS2Addr;
        private System.Windows.Forms.Label labelXML;
    }
}