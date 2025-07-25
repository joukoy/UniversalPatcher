
namespace UniversalPatcher
{
    partial class frmSerialPortServer
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
            this.richLogger = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numServerPort = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.radioFTDI = new System.Windows.Forms.RadioButton();
            this.radioRS232 = new System.Windows.Forms.RadioButton();
            this.labelSerialport = new System.Windows.Forms.Label();
            this.comboSerialPort = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBaudRate = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numServerPort)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // richLogger
            // 
            this.richLogger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richLogger.HideSelection = false;
            this.richLogger.Location = new System.Drawing.Point(2, 152);
            this.richLogger.Name = "richLogger";
            this.richLogger.Size = new System.Drawing.Size(625, 131);
            this.richLogger.TabIndex = 0;
            this.richLogger.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port:";
            // 
            // numServerPort
            // 
            this.numServerPort.Location = new System.Drawing.Point(64, 26);
            this.numServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numServerPort.Name = "numServerPort";
            this.numServerPort.Size = new System.Drawing.Size(62, 20);
            this.numServerPort.TabIndex = 2;
            this.numServerPort.Value = new decimal(new int[] {
            22222,
            0,
            0,
            0});
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(533, 17);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 28);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // radioFTDI
            // 
            this.radioFTDI.AutoSize = true;
            this.radioFTDI.Location = new System.Drawing.Point(82, 26);
            this.radioFTDI.Name = "radioFTDI";
            this.radioFTDI.Size = new System.Drawing.Size(49, 17);
            this.radioFTDI.TabIndex = 27;
            this.radioFTDI.TabStop = true;
            this.radioFTDI.Text = "FTDI";
            this.radioFTDI.UseVisualStyleBackColor = true;
            this.radioFTDI.CheckedChanged += new System.EventHandler(this.radioFTDI_CheckedChanged);
            // 
            // radioRS232
            // 
            this.radioRS232.AutoSize = true;
            this.radioRS232.Location = new System.Drawing.Point(9, 26);
            this.radioRS232.Name = "radioRS232";
            this.radioRS232.Size = new System.Drawing.Size(51, 17);
            this.radioRS232.TabIndex = 26;
            this.radioRS232.TabStop = true;
            this.radioRS232.Text = "Serial";
            this.radioRS232.UseVisualStyleBackColor = true;
            // 
            // labelSerialport
            // 
            this.labelSerialport.AutoSize = true;
            this.labelSerialport.Location = new System.Drawing.Point(6, 58);
            this.labelSerialport.Name = "labelSerialport";
            this.labelSerialport.Size = new System.Drawing.Size(29, 13);
            this.labelSerialport.TabIndex = 25;
            this.labelSerialport.Text = "Port:";
            // 
            // comboSerialPort
            // 
            this.comboSerialPort.FormattingEnabled = true;
            this.comboSerialPort.Location = new System.Drawing.Point(72, 55);
            this.comboSerialPort.Name = "comboSerialPort";
            this.comboSerialPort.Size = new System.Drawing.Size(264, 21);
            this.comboSerialPort.TabIndex = 24;
            this.comboSerialPort.SelectedIndexChanged += new System.EventHandler(this.comboSerialPort_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBaudRate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.labelSerialport);
            this.groupBox1.Controls.Add(this.comboSerialPort);
            this.groupBox1.Controls.Add(this.radioFTDI);
            this.groupBox1.Controls.Add(this.radioRS232);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(342, 116);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numServerPort);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(373, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(143, 71);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server";
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(533, 55);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(74, 30);
            this.btnStop.TabIndex = 31;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Baudrate:";
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.FormattingEnabled = true;
            this.comboBaudRate.Location = new System.Drawing.Point(72, 84);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(264, 21);
            this.comboBaudRate.TabIndex = 29;
            // 
            // frmSerialPortServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 283);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.richLogger);
            this.Name = "frmSerialPortServer";
            this.Text = "Serialport Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSerialPortServer_FormClosing);
            this.Load += new System.EventHandler(this.frmSerialPortServer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numServerPort)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richLogger;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numServerPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RadioButton radioFTDI;
        private System.Windows.Forms.RadioButton radioRS232;
        private System.Windows.Forms.Label labelSerialport;
        private System.Windows.Forms.ComboBox comboSerialPort;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBaudRate;
    }
}