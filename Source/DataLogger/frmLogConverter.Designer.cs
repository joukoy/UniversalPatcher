
namespace UniversalPatcher
{
    partial class frmLogConverter
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogConverter));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setRequestDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDataDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDataOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.comboAddrSrc = new System.Windows.Forms.ComboBox();
            this.comboLenSrc = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numAddrBytes = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numLenBytes = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRowInfo = new System.Windows.Forms.TextBox();
            this.btnUpdateColors = new System.Windows.Forms.Button();
            this.labelSettingsFile = new System.Windows.Forms.Label();
            this.bntLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtFixedLen = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.numDataOffset = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numLenPos = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numAddrPos = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnReadFile = new System.Windows.Forms.Button();
            this.txtDetectData = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDetectRequest = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupXml = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtFillByte = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.numRowsFrom = new System.Windows.Forms.NumericUpDown();
            this.numRowsTo = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.setFirstRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setLastRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label14 = new System.Windows.Forms.Label();
            this.txtGlobalOffset = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAddrBytes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLenBytes)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDataOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLenPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddrPos)).BeginInit();
            this.groupXml.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRowsFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRowsTo)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Location = new System.Drawing.Point(5, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(920, 468);
            this.dataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setRequestDetectionToolStripMenuItem,
            this.setDataDetectionToolStripMenuItem,
            this.setAddressToolStripMenuItem,
            this.setLengthToolStripMenuItem,
            this.setDataOffsetToolStripMenuItem,
            this.setFirstRowToolStripMenuItem,
            this.setLastRowToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 158);
            // 
            // setRequestDetectionToolStripMenuItem
            // 
            this.setRequestDetectionToolStripMenuItem.Name = "setRequestDetectionToolStripMenuItem";
            this.setRequestDetectionToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setRequestDetectionToolStripMenuItem.Text = "Set Request detection";
            this.setRequestDetectionToolStripMenuItem.Click += new System.EventHandler(this.setRequestDetectionToolStripMenuItem_Click);
            // 
            // setDataDetectionToolStripMenuItem
            // 
            this.setDataDetectionToolStripMenuItem.Name = "setDataDetectionToolStripMenuItem";
            this.setDataDetectionToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setDataDetectionToolStripMenuItem.Text = "Set Data detection";
            this.setDataDetectionToolStripMenuItem.Click += new System.EventHandler(this.setDataDetectionToolStripMenuItem_Click);
            // 
            // setAddressToolStripMenuItem
            // 
            this.setAddressToolStripMenuItem.Name = "setAddressToolStripMenuItem";
            this.setAddressToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setAddressToolStripMenuItem.Text = "Set address";
            this.setAddressToolStripMenuItem.Click += new System.EventHandler(this.setAddressToolStripMenuItem_Click);
            // 
            // setLengthToolStripMenuItem
            // 
            this.setLengthToolStripMenuItem.Name = "setLengthToolStripMenuItem";
            this.setLengthToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setLengthToolStripMenuItem.Text = "Set length";
            this.setLengthToolStripMenuItem.Click += new System.EventHandler(this.setLengthToolStripMenuItem_Click);
            // 
            // setDataOffsetToolStripMenuItem
            // 
            this.setDataOffsetToolStripMenuItem.Name = "setDataOffsetToolStripMenuItem";
            this.setDataOffsetToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setDataOffsetToolStripMenuItem.Text = "Set data offset";
            this.setDataOffsetToolStripMenuItem.Click += new System.EventHandler(this.setDataOffsetToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Address from:";
            // 
            // comboAddrSrc
            // 
            this.comboAddrSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboAddrSrc.FormattingEnabled = true;
            this.comboAddrSrc.Location = new System.Drawing.Point(126, 121);
            this.comboAddrSrc.Name = "comboAddrSrc";
            this.comboAddrSrc.Size = new System.Drawing.Size(123, 21);
            this.comboAddrSrc.TabIndex = 2;
            // 
            // comboLenSrc
            // 
            this.comboLenSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboLenSrc.FormattingEnabled = true;
            this.comboLenSrc.Location = new System.Drawing.Point(125, 200);
            this.comboLenSrc.Name = "comboLenSrc";
            this.comboLenSrc.Size = new System.Drawing.Size(124, 21);
            this.comboLenSrc.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 203);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Length from:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(169, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "bytes:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // numAddrBytes
            // 
            this.numAddrBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numAddrBytes.Location = new System.Drawing.Point(210, 174);
            this.numAddrBytes.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numAddrBytes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAddrBytes.Name = "numAddrBytes";
            this.numAddrBytes.Size = new System.Drawing.Size(39, 20);
            this.numAddrBytes.TabIndex = 6;
            this.numAddrBytes.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 229);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "bytes:";
            // 
            // numLenBytes
            // 
            this.numLenBytes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numLenBytes.Location = new System.Drawing.Point(211, 227);
            this.numLenBytes.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numLenBytes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLenBytes.Name = "numLenBytes";
            this.numLenBytes.Size = new System.Drawing.Size(38, 20);
            this.numLenBytes.TabIndex = 8;
            this.numLenBytes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Request detection: ( * = any byte)";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtGlobalOffset);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.numRowsTo);
            this.groupBox1.Controls.Add(this.numRowsFrom);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtFillByte);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtRowInfo);
            this.groupBox1.Controls.Add(this.btnUpdateColors);
            this.groupBox1.Controls.Add(this.txtFixedLen);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.numDataOffset);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.numLenPos);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numAddrPos);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btnConvert);
            this.groupBox1.Controls.Add(this.btnReadFile);
            this.groupBox1.Controls.Add(this.txtDetectData);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtDetectRequest);
            this.groupBox1.Controls.Add(this.comboLenSrc);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numLenBytes);
            this.groupBox1.Controls.Add(this.comboAddrSrc);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numAddrBytes);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(931, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 473);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // txtRowInfo
            // 
            this.txtRowInfo.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRowInfo.Location = new System.Drawing.Point(10, 385);
            this.txtRowInfo.Multiline = true;
            this.txtRowInfo.Name = "txtRowInfo";
            this.txtRowInfo.Size = new System.Drawing.Size(239, 82);
            this.txtRowInfo.TabIndex = 26;
            // 
            // btnUpdateColors
            // 
            this.btnUpdateColors.Location = new System.Drawing.Point(164, 327);
            this.btnUpdateColors.Name = "btnUpdateColors";
            this.btnUpdateColors.Size = new System.Drawing.Size(85, 23);
            this.btnUpdateColors.TabIndex = 25;
            this.btnUpdateColors.Text = "Update colors";
            this.btnUpdateColors.UseVisualStyleBackColor = true;
            this.btnUpdateColors.Click += new System.EventHandler(this.btnUpdateColors_Click);
            // 
            // labelSettingsFile
            // 
            this.labelSettingsFile.AutoSize = true;
            this.labelSettingsFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSettingsFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSettingsFile.Location = new System.Drawing.Point(6, 13);
            this.labelSettingsFile.Name = "labelSettingsFile";
            this.labelSettingsFile.Size = new System.Drawing.Size(13, 17);
            this.labelSettingsFile.TabIndex = 24;
            this.labelSettingsFile.Text = "-";
            // 
            // bntLoad
            // 
            this.bntLoad.Location = new System.Drawing.Point(148, 35);
            this.bntLoad.Name = "bntLoad";
            this.bntLoad.Size = new System.Drawing.Size(102, 23);
            this.bntLoad.TabIndex = 23;
            this.bntLoad.Text = "Load settings...";
            this.bntLoad.UseVisualStyleBackColor = true;
            this.bntLoad.Click += new System.EventHandler(this.bntLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 35);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 23);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "Save settings...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtFixedLen
            // 
            this.txtFixedLen.Location = new System.Drawing.Point(212, 251);
            this.txtFixedLen.Name = "txtFixedLen";
            this.txtFixedLen.Size = new System.Drawing.Size(37, 20);
            this.txtFixedLen.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 254);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Length (fixed) [HEX]:";
            // 
            // numDataOffset
            // 
            this.numDataOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDataOffset.Location = new System.Drawing.Point(212, 277);
            this.numDataOffset.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numDataOffset.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDataOffset.Name = "numDataOffset";
            this.numDataOffset.Size = new System.Drawing.Size(38, 20);
            this.numDataOffset.TabIndex = 19;
            this.numDataOffset.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 279);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Data offset:";
            // 
            // numLenPos
            // 
            this.numLenPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numLenPos.Location = new System.Drawing.Point(124, 227);
            this.numLenPos.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numLenPos.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLenPos.Name = "numLenPos";
            this.numLenPos.Size = new System.Drawing.Size(39, 20);
            this.numLenPos.TabIndex = 17;
            this.numLenPos.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 229);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Length position:";
            // 
            // numAddrPos
            // 
            this.numAddrPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numAddrPos.Location = new System.Drawing.Point(125, 174);
            this.numAddrPos.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numAddrPos.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAddrPos.Name = "numAddrPos";
            this.numAddrPos.Size = new System.Drawing.Size(38, 20);
            this.numAddrPos.TabIndex = 15;
            this.numAddrPos.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 178);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Address position:";
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(166, 356);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(83, 23);
            this.btnConvert.TabIndex = 13;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnReadFile
            // 
            this.btnReadFile.Location = new System.Drawing.Point(12, 356);
            this.btnReadFile.Name = "btnReadFile";
            this.btnReadFile.Size = new System.Drawing.Size(83, 23);
            this.btnReadFile.TabIndex = 11;
            this.btnReadFile.Text = "Read file...";
            this.btnReadFile.UseVisualStyleBackColor = true;
            this.btnReadFile.Click += new System.EventHandler(this.btnReadFile_Click);
            // 
            // txtDetectData
            // 
            this.txtDetectData.Location = new System.Drawing.Point(10, 70);
            this.txtDetectData.Name = "txtDetectData";
            this.txtDetectData.Size = new System.Drawing.Size(241, 20);
            this.txtDetectData.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Data detection: ( * = any byte)";
            // 
            // txtDetectRequest
            // 
            this.txtDetectRequest.Location = new System.Drawing.Point(10, 31);
            this.txtDetectRequest.Name = "txtDetectRequest";
            this.txtDetectRequest.Size = new System.Drawing.Size(241, 20);
            this.txtDetectRequest.TabIndex = 10;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.HideSelection = false;
            this.richTextBox1.Location = new System.Drawing.Point(6, 475);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(919, 70);
            this.richTextBox1.TabIndex = 12;
            this.richTextBox1.Text = "";
            // 
            // groupXml
            // 
            this.groupXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupXml.Controls.Add(this.labelSettingsFile);
            this.groupXml.Controls.Add(this.btnSave);
            this.groupXml.Controls.Add(this.bntLoad);
            this.groupXml.Location = new System.Drawing.Point(931, 4);
            this.groupXml.Name = "groupXml";
            this.groupXml.Size = new System.Drawing.Size(264, 67);
            this.groupXml.TabIndex = 13;
            this.groupXml.TabStop = false;
            this.groupXml.Enter += new System.EventHandler(this.groupXml_Enter);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 304);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(122, 13);
            this.label11.TabIndex = 27;
            this.label11.Text = "Fill empty area with byte:";
            // 
            // txtFillByte
            // 
            this.txtFillByte.Location = new System.Drawing.Point(212, 301);
            this.txtFillByte.Name = "txtFillByte";
            this.txtFillByte.Size = new System.Drawing.Size(37, 20);
            this.txtFillByte.TabIndex = 28;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 98);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "Use rows:";
            // 
            // numRowsFrom
            // 
            this.numRowsFrom.Location = new System.Drawing.Point(82, 96);
            this.numRowsFrom.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numRowsFrom.Name = "numRowsFrom";
            this.numRowsFrom.Size = new System.Drawing.Size(73, 20);
            this.numRowsFrom.TabIndex = 30;
            // 
            // numRowsTo
            // 
            this.numRowsTo.Location = new System.Drawing.Point(180, 95);
            this.numRowsTo.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numRowsTo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numRowsTo.Name = "numRowsTo";
            this.numRowsTo.Size = new System.Drawing.Size(70, 20);
            this.numRowsTo.TabIndex = 31;
            this.numRowsTo.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(163, 98);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(10, 13);
            this.label13.TabIndex = 32;
            this.label13.Text = "-";
            // 
            // setFirstRowToolStripMenuItem
            // 
            this.setFirstRowToolStripMenuItem.Name = "setFirstRowToolStripMenuItem";
            this.setFirstRowToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setFirstRowToolStripMenuItem.Text = "Set first row";
            this.setFirstRowToolStripMenuItem.Click += new System.EventHandler(this.setFirstRowToolStripMenuItem_Click);
            // 
            // setLastRowToolStripMenuItem
            // 
            this.setLastRowToolStripMenuItem.Name = "setLastRowToolStripMenuItem";
            this.setLastRowToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.setLastRowToolStripMenuItem.Text = "Set last row";
            this.setLastRowToolStripMenuItem.Click += new System.EventHandler(this.setLastRowToolStripMenuItem_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 150);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(100, 13);
            this.label14.TabIndex = 33;
            this.label14.Text = "Global offset (HEX):";
            // 
            // txtGlobalOffset
            // 
            this.txtGlobalOffset.Location = new System.Drawing.Point(127, 147);
            this.txtGlobalOffset.Name = "txtGlobalOffset";
            this.txtGlobalOffset.Size = new System.Drawing.Size(121, 20);
            this.txtGlobalOffset.TabIndex = 34;
            // 
            // frmLogConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1195, 548);
            this.Controls.Add(this.groupXml);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLogConverter";
            this.Text = "Log Converter";
            this.Load += new System.EventHandler(this.frmLogConverter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numAddrBytes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLenBytes)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDataOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLenPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAddrPos)).EndInit();
            this.groupXml.ResumeLayout(false);
            this.groupXml.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRowsFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRowsTo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboAddrSrc;
        private System.Windows.Forms.ComboBox comboLenSrc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numAddrBytes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numLenBytes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtDetectData;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDetectRequest;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnReadFile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.NumericUpDown numLenPos;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numAddrPos;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripMenuItem setRequestDetectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setDataDetectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setLengthToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numDataOffset;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtFixedLen;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button bntLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolStripMenuItem setDataOffsetToolStripMenuItem;
        private System.Windows.Forms.Label labelSettingsFile;
        private System.Windows.Forms.Button btnUpdateColors;
        private System.Windows.Forms.TextBox txtRowInfo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numRowsTo;
        private System.Windows.Forms.NumericUpDown numRowsFrom;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtFillByte;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupXml;
        private System.Windows.Forms.ToolStripMenuItem setFirstRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setLastRowToolStripMenuItem;
        private System.Windows.Forms.TextBox txtGlobalOffset;
        private System.Windows.Forms.Label label14;
    }
}