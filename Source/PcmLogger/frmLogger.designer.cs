
namespace UniversalPatcher
{
    partial class frmLogger
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
            this.timerShowData = new System.Windows.Forms.Timer(this.components);
            this.timerPresent = new System.Windows.Forms.Timer(this.components);
            this.comboSerialPort = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.serialOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.comboSerialDeviceType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.j2534OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.j2534DeviceList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.categories = new System.Windows.Forms.GroupBox();
            this.j2534RadioButton = new System.Windows.Forms.RadioButton();
            this.serialRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkWriteLog = new System.Windows.Forms.CheckBox();
            this.btnBrowsLogFolder = new System.Windows.Forms.Button();
            this.txtLogSeparator = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLogFolder = new System.Windows.Forms.TextBox();
            this.dataGridLogData = new System.Windows.Forms.DataGridView();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.tabProfile = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridPidNames = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dataGridLogProfile = new System.Windows.Forms.DataGridView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labelProgress = new System.Windows.Forms.Label();
            this.comboResponseMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.serialOptionsGroupBox.SuspendLayout();
            this.j2534OptionsGroupBox.SuspendLayout();
            this.categories.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogData)).BeginInit();
            this.tabProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPidNames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogProfile)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerShowData
            // 
            this.timerShowData.Interval = 300;
            this.timerShowData.Tick += new System.EventHandler(this.timerShowData_Tick);
            // 
            // timerPresent
            // 
            this.timerPresent.Interval = 4000;
            this.timerPresent.Tick += new System.EventHandler(this.timerPresent_Tick);
            // 
            // comboSerialPort
            // 
            this.comboSerialPort.FormattingEnabled = true;
            this.comboSerialPort.Location = new System.Drawing.Point(82, 19);
            this.comboSerialPort.Name = "comboSerialPort";
            this.comboSerialPort.Size = new System.Drawing.Size(130, 21);
            this.comboSerialPort.TabIndex = 2;
            this.comboSerialPort.SelectedIndexChanged += new System.EventHandler(this.comboSerialPort_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Controls.Add(this.tabProfile);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(922, 427);
            this.tabControl1.TabIndex = 3;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.label7);
            this.tabLog.Controls.Add(this.label6);
            this.tabLog.Controls.Add(this.comboResponseMode);
            this.tabLog.Controls.Add(this.labelProgress);
            this.tabLog.Controls.Add(this.serialOptionsGroupBox);
            this.tabLog.Controls.Add(this.j2534OptionsGroupBox);
            this.tabLog.Controls.Add(this.categories);
            this.tabLog.Controls.Add(this.groupBox1);
            this.tabLog.Controls.Add(this.dataGridLogData);
            this.tabLog.Controls.Add(this.btnStartStop);
            this.tabLog.Location = new System.Drawing.Point(4, 22);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(914, 401);
            this.tabLog.TabIndex = 0;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // serialOptionsGroupBox
            // 
            this.serialOptionsGroupBox.Controls.Add(this.comboSerialDeviceType);
            this.serialOptionsGroupBox.Controls.Add(this.label1);
            this.serialOptionsGroupBox.Controls.Add(this.comboSerialPort);
            this.serialOptionsGroupBox.Controls.Add(this.label4);
            this.serialOptionsGroupBox.Location = new System.Drawing.Point(150, 109);
            this.serialOptionsGroupBox.Name = "serialOptionsGroupBox";
            this.serialOptionsGroupBox.Size = new System.Drawing.Size(223, 81);
            this.serialOptionsGroupBox.TabIndex = 20;
            this.serialOptionsGroupBox.TabStop = false;
            this.serialOptionsGroupBox.Text = "Serialport options";
            // 
            // comboSerialDeviceType
            // 
            this.comboSerialDeviceType.FormattingEnabled = true;
            this.comboSerialDeviceType.Location = new System.Drawing.Point(82, 50);
            this.comboSerialDeviceType.Name = "comboSerialDeviceType";
            this.comboSerialDeviceType.Size = new System.Drawing.Size(130, 21);
            this.comboSerialDeviceType.TabIndex = 17;
            this.comboSerialDeviceType.SelectedIndexChanged += new System.EventHandler(this.comboSerialDeviceType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Device Type";
            // 
            // j2534OptionsGroupBox
            // 
            this.j2534OptionsGroupBox.Controls.Add(this.j2534DeviceList);
            this.j2534OptionsGroupBox.Controls.Add(this.label5);
            this.j2534OptionsGroupBox.Location = new System.Drawing.Point(150, 196);
            this.j2534OptionsGroupBox.Name = "j2534OptionsGroupBox";
            this.j2534OptionsGroupBox.Size = new System.Drawing.Size(223, 72);
            this.j2534OptionsGroupBox.TabIndex = 19;
            this.j2534OptionsGroupBox.TabStop = false;
            this.j2534OptionsGroupBox.Text = "J2534 Device Options";
            // 
            // j2534DeviceList
            // 
            this.j2534DeviceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.j2534DeviceList.FormattingEnabled = true;
            this.j2534DeviceList.Location = new System.Drawing.Point(7, 37);
            this.j2534DeviceList.Name = "j2534DeviceList";
            this.j2534DeviceList.Size = new System.Drawing.Size(211, 21);
            this.j2534DeviceList.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "&Device Type";
            // 
            // categories
            // 
            this.categories.Controls.Add(this.j2534RadioButton);
            this.categories.Controls.Add(this.serialRadioButton);
            this.categories.Location = new System.Drawing.Point(6, 109);
            this.categories.Name = "categories";
            this.categories.Size = new System.Drawing.Size(138, 71);
            this.categories.TabIndex = 18;
            this.categories.TabStop = false;
            this.categories.Text = "Device &Category";
            // 
            // j2534RadioButton
            // 
            this.j2534RadioButton.AutoSize = true;
            this.j2534RadioButton.Location = new System.Drawing.Point(7, 46);
            this.j2534RadioButton.Name = "j2534RadioButton";
            this.j2534RadioButton.Size = new System.Drawing.Size(91, 17);
            this.j2534RadioButton.TabIndex = 1;
            this.j2534RadioButton.Text = "&J2534 Device";
            this.j2534RadioButton.UseVisualStyleBackColor = true;
            this.j2534RadioButton.CheckedChanged += new System.EventHandler(this.j2534RadioButton_CheckedChanged);
            // 
            // serialRadioButton
            // 
            this.serialRadioButton.AutoSize = true;
            this.serialRadioButton.Checked = true;
            this.serialRadioButton.Location = new System.Drawing.Point(7, 20);
            this.serialRadioButton.Name = "serialRadioButton";
            this.serialRadioButton.Size = new System.Drawing.Size(110, 17);
            this.serialRadioButton.TabIndex = 0;
            this.serialRadioButton.TabStop = true;
            this.serialRadioButton.Text = "&Serial Port Device";
            this.serialRadioButton.UseVisualStyleBackColor = true;
            this.serialRadioButton.CheckedChanged += new System.EventHandler(this.serialRadioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkWriteLog);
            this.groupBox1.Controls.Add(this.btnBrowsLogFolder);
            this.groupBox1.Controls.Add(this.txtLogSeparator);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtLogFolder);
            this.groupBox1.Location = new System.Drawing.Point(6, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(367, 102);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logfile";
            // 
            // chkWriteLog
            // 
            this.chkWriteLog.AutoSize = true;
            this.chkWriteLog.Checked = true;
            this.chkWriteLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWriteLog.Location = new System.Drawing.Point(9, 73);
            this.chkWriteLog.Name = "chkWriteLog";
            this.chkWriteLog.Size = new System.Drawing.Size(68, 17);
            this.chkWriteLog.TabIndex = 9;
            this.chkWriteLog.Text = "Write log";
            this.chkWriteLog.UseVisualStyleBackColor = true;
            // 
            // btnBrowsLogFolder
            // 
            this.btnBrowsLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsLogFolder.Location = new System.Drawing.Point(328, 40);
            this.btnBrowsLogFolder.Name = "btnBrowsLogFolder";
            this.btnBrowsLogFolder.Size = new System.Drawing.Size(33, 23);
            this.btnBrowsLogFolder.TabIndex = 12;
            this.btnBrowsLogFolder.Text = "...";
            this.btnBrowsLogFolder.UseVisualStyleBackColor = true;
            this.btnBrowsLogFolder.Click += new System.EventHandler(this.btnBrowsLogFolder_Click);
            // 
            // txtLogSeparator
            // 
            this.txtLogSeparator.Location = new System.Drawing.Point(144, 71);
            this.txtLogSeparator.Name = "txtLogSeparator";
            this.txtLogSeparator.Size = new System.Drawing.Size(30, 20);
            this.txtLogSeparator.TabIndex = 14;
            this.txtLogSeparator.Text = ",";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Logfolder:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Separator:";
            // 
            // txtLogFolder
            // 
            this.txtLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogFolder.Location = new System.Drawing.Point(6, 42);
            this.txtLogFolder.Name = "txtLogFolder";
            this.txtLogFolder.Size = new System.Drawing.Size(316, 20);
            this.txtLogFolder.TabIndex = 10;
            // 
            // dataGridLogData
            // 
            this.dataGridLogData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridLogData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLogData.Location = new System.Drawing.Point(379, 4);
            this.dataGridLogData.Name = "dataGridLogData";
            this.dataGridLogData.Size = new System.Drawing.Size(534, 396);
            this.dataGridLogData.TabIndex = 8;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(150, 274);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(223, 89);
            this.btnStartStop.TabIndex = 5;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // tabProfile
            // 
            this.tabProfile.Controls.Add(this.splitContainer1);
            this.tabProfile.Location = new System.Drawing.Point(4, 22);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.Size = new System.Drawing.Size(914, 401);
            this.tabProfile.TabIndex = 2;
            this.tabProfile.Text = "Profile";
            this.tabProfile.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridPidNames);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnAdd);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridLogProfile);
            this.splitContainer1.Panel2.Controls.Add(this.btnRemove);
            this.splitContainer1.Size = new System.Drawing.Size(914, 401);
            this.splitContainer1.SplitterDistance = 373;
            this.splitContainer1.TabIndex = 4;
            // 
            // dataGridPidNames
            // 
            this.dataGridPidNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPidNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridPidNames.Location = new System.Drawing.Point(0, 0);
            this.dataGridPidNames.Name = "dataGridPidNames";
            this.dataGridPidNames.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridPidNames.Size = new System.Drawing.Size(373, 401);
            this.dataGridPidNames.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 76);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(28, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dataGridLogProfile
            // 
            this.dataGridLogProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridLogProfile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLogProfile.Location = new System.Drawing.Point(37, 0);
            this.dataGridLogProfile.Name = "dataGridLogProfile";
            this.dataGridLogProfile.Size = new System.Drawing.Size(497, 398);
            this.dataGridLogProfile.TabIndex = 0;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(3, 114);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(28, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(922, 128);
            this.txtResult.TabIndex = 6;
            this.txtResult.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(922, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadProfileToolStripMenuItem,
            this.saveProfileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadProfileToolStripMenuItem
            // 
            this.loadProfileToolStripMenuItem.Name = "loadProfileToolStripMenuItem";
            this.loadProfileToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.loadProfileToolStripMenuItem.Text = "Load profile";
            this.loadProfileToolStripMenuItem.Click += new System.EventHandler(this.loadProfileToolStripMenuItem_Click);
            // 
            // saveProfileToolStripMenuItem
            // 
            this.saveProfileToolStripMenuItem.Name = "saveProfileToolStripMenuItem";
            this.saveProfileToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.saveProfileToolStripMenuItem.Text = "Save profile";
            this.saveProfileToolStripMenuItem.Click += new System.EventHandler(this.saveProfileToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtResult);
            this.splitContainer2.Size = new System.Drawing.Size(922, 559);
            this.splitContainer2.SplitterDistance = 427;
            this.splitContainer2.TabIndex = 5;
            // 
            // labelProgress
            // 
            this.labelProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelProgress.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.Location = new System.Drawing.Point(6, 377);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(367, 21);
            this.labelProgress.TabIndex = 22;
            this.labelProgress.Text = "idle";
            // 
            // comboResponseMode
            // 
            this.comboResponseMode.FormattingEnabled = true;
            this.comboResponseMode.Location = new System.Drawing.Point(7, 209);
            this.comboResponseMode.Name = "comboResponseMode";
            this.comboResponseMode.Size = new System.Drawing.Size(137, 21);
            this.comboResponseMode.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Response Mode:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 236);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 74);
            this.label7.TabIndex = 25;
            this.label7.Text = "In SendOnce mode PC send new request after every response. Fastest method for sho" +
    "rt pid list(?)";
            // 
            // frmLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 583);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmLogger";
            this.Text = "Logger";
            this.Load += new System.EventHandler(this.frmLogger_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.serialOptionsGroupBox.ResumeLayout(false);
            this.serialOptionsGroupBox.PerformLayout();
            this.j2534OptionsGroupBox.ResumeLayout(false);
            this.j2534OptionsGroupBox.PerformLayout();
            this.categories.ResumeLayout(false);
            this.categories.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogData)).EndInit();
            this.tabProfile.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPidNames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLogProfile)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timerShowData;
        private System.Windows.Forms.Timer timerPresent;
        private System.Windows.Forms.ComboBox comboSerialPort;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TabPage tabProfile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridLogProfile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileToolStripMenuItem;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridView dataGridPidNames;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.DataGridView dataGridLogData;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.CheckBox chkWriteLog;
        private System.Windows.Forms.Button btnBrowsLogFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLogFolder;
        private System.Windows.Forms.TextBox txtLogSeparator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboSerialDeviceType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox j2534OptionsGroupBox;
        private System.Windows.Forms.ComboBox j2534DeviceList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox categories;
        private System.Windows.Forms.RadioButton j2534RadioButton;
        private System.Windows.Forms.RadioButton serialRadioButton;
        private System.Windows.Forms.GroupBox serialOptionsGroupBox;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboResponseMode;
        private System.Windows.Forms.Label label7;
    }
}

