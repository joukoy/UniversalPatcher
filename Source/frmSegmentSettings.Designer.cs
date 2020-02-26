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
            this.btnSave = new System.Windows.Forms.Button();
            this.listSegments = new System.Windows.Forms.ListView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
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
            this.btnNewXML = new System.Windows.Forms.Button();
            this.checkSwapBytes1 = new System.Windows.Forms.CheckBox();
            this.checkSwapBytes2 = new System.Windows.Forms.CheckBox();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
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
            this.groupBox1.Location = new System.Drawing.Point(11, 316);
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
            this.groupBox2.Controls.Add(this.radioCS2DwordSum);
            this.groupBox2.Controls.Add(this.radioCS2None);
            this.groupBox2.Controls.Add(this.radioCS2WordSum);
            this.groupBox2.Controls.Add(this.radioCS2SUM);
            this.groupBox2.Controls.Add(this.radioCS2Crc32);
            this.groupBox2.Controls.Add(this.radioCS2Crc16);
            this.groupBox2.Location = new System.Drawing.Point(264, 316);
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
            this.label4.Location = new System.Drawing.Point(7, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Checksum 1 Address (HEX):";
            // 
            // txtCS1Address
            // 
            this.txtCS1Address.Location = new System.Drawing.Point(148, 248);
            this.txtCS1Address.Name = "txtCS1Address";
            this.txtCS1Address.Size = new System.Drawing.Size(71, 20);
            this.txtCS1Address.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(315, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Checksum 2 Address (HEX):";
            // 
            // txtCS2Address
            // 
            this.txtCS2Address.Location = new System.Drawing.Point(457, 251);
            this.txtCS2Address.Name = "txtCS2Address";
            this.txtCS2Address.Size = new System.Drawing.Size(72, 20);
            this.txtCS2Address.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(454, 457);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 35);
            this.btnSave.TabIndex = 31;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // listSegments
            // 
            this.listSegments.HideSelection = false;
            this.listSegments.Location = new System.Drawing.Point(2, 3);
            this.listSegments.Name = "listSegments";
            this.listSegments.Size = new System.Drawing.Size(461, 141);
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
            this.btnAdd.Text = "Apply";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(374, 457);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(74, 35);
            this.btnLoad.TabIndex = 30;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioCS1Complement2);
            this.groupBox5.Controls.Add(this.radioCS1Complement1);
            this.groupBox5.Controls.Add(this.radioCS1Complement0);
            this.groupBox5.Location = new System.Drawing.Point(139, 316);
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
            this.groupBox6.Location = new System.Drawing.Point(392, 316);
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
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(12, 457);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 33);
            this.btnHelp.TabIndex = 37;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnNewXML
            // 
            this.btnNewXML.Location = new System.Drawing.Point(286, 458);
            this.btnNewXML.Name = "btnNewXML";
            this.btnNewXML.Size = new System.Drawing.Size(82, 34);
            this.btnNewXML.TabIndex = 38;
            this.btnNewXML.Text = "New XML";
            this.btnNewXML.UseVisualStyleBackColor = true;
            this.btnNewXML.Click += new System.EventHandler(this.btnNewXML_Click);
            // 
            // checkSwapBytes1
            // 
            this.checkSwapBytes1.AutoSize = true;
            this.checkSwapBytes1.Location = new System.Drawing.Point(145, 407);
            this.checkSwapBytes1.Name = "checkSwapBytes1";
            this.checkSwapBytes1.Size = new System.Drawing.Size(81, 17);
            this.checkSwapBytes1.TabIndex = 16;
            this.checkSwapBytes1.Text = "Swap bytes";
            this.checkSwapBytes1.UseVisualStyleBackColor = true;
            // 
            // checkSwapBytes2
            // 
            this.checkSwapBytes2.AutoSize = true;
            this.checkSwapBytes2.Location = new System.Drawing.Point(398, 410);
            this.checkSwapBytes2.Name = "checkSwapBytes2";
            this.checkSwapBytes2.Size = new System.Drawing.Size(82, 17);
            this.checkSwapBytes2.TabIndex = 39;
            this.checkSwapBytes2.Text = "Swap Bytes";
            this.checkSwapBytes2.UseVisualStyleBackColor = true;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(468, 38);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(71, 26);
            this.btnMoveUp.TabIndex = 40;
            this.btnMoveUp.Text = "Move up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(468, 69);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(71, 23);
            this.btnMoveDown.TabIndex = 41;
            this.btnMoveDown.Text = "Move down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // frmSegmentSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 502);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.checkSwapBytes2);
            this.Controls.Add(this.checkSwapBytes1);
            this.Controls.Add(this.btnNewXML);
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
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.listSegments);
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
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.ListView listSegments;
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
        private System.Windows.Forms.Button btnNewXML;
        private System.Windows.Forms.CheckBox checkSwapBytes1;
        private System.Windows.Forms.CheckBox checkSwapBytes2;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
    }
}