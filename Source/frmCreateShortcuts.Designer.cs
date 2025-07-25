
namespace UniversalPatcher
{
    partial class frmCreateShortcuts
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
            this.radioCustom = new System.Windows.Forms.RadioButton();
            this.radioDesktop = new System.Windows.Forms.RadioButton();
            this.chkPatcherTourist = new System.Windows.Forms.CheckBox();
            this.chkPatcherBasic = new System.Windows.Forms.CheckBox();
            this.chkPatcherAdvanced = new System.Windows.Forms.CheckBox();
            this.chkTunerTourist = new System.Windows.Forms.CheckBox();
            this.chkTunerBasic = new System.Windows.Forms.CheckBox();
            this.chkTunerAdvanced = new System.Windows.Forms.CheckBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.chkLoggerAdvanced = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkLoggerBasic = new System.Windows.Forms.CheckBox();
            this.chkLoggerTourist = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkSerialportServer = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioCustom);
            this.groupBox1.Controls.Add(this.radioDesktop);
            this.groupBox1.Location = new System.Drawing.Point(9, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(399, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
            // 
            // radioCustom
            // 
            this.radioCustom.AutoSize = true;
            this.radioCustom.Location = new System.Drawing.Point(15, 39);
            this.radioCustom.Name = "radioCustom";
            this.radioCustom.Size = new System.Drawing.Size(60, 17);
            this.radioCustom.TabIndex = 1;
            this.radioCustom.TabStop = true;
            this.radioCustom.Text = "Other...";
            this.radioCustom.UseVisualStyleBackColor = true;
            this.radioCustom.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
            // 
            // radioDesktop
            // 
            this.radioDesktop.AutoSize = true;
            this.radioDesktop.Checked = true;
            this.radioDesktop.Location = new System.Drawing.Point(15, 16);
            this.radioDesktop.Name = "radioDesktop";
            this.radioDesktop.Size = new System.Drawing.Size(65, 17);
            this.radioDesktop.TabIndex = 0;
            this.radioDesktop.TabStop = true;
            this.radioDesktop.Text = "Desktop";
            this.radioDesktop.UseVisualStyleBackColor = true;
            // 
            // chkPatcherTourist
            // 
            this.chkPatcherTourist.AutoSize = true;
            this.chkPatcherTourist.Location = new System.Drawing.Point(6, 21);
            this.chkPatcherTourist.Name = "chkPatcherTourist";
            this.chkPatcherTourist.Size = new System.Drawing.Size(98, 17);
            this.chkPatcherTourist.TabIndex = 1;
            this.chkPatcherTourist.Text = "Patcher-Tourist";
            this.chkPatcherTourist.UseVisualStyleBackColor = true;
            // 
            // chkPatcherBasic
            // 
            this.chkPatcherBasic.AutoSize = true;
            this.chkPatcherBasic.Location = new System.Drawing.Point(6, 43);
            this.chkPatcherBasic.Name = "chkPatcherBasic";
            this.chkPatcherBasic.Size = new System.Drawing.Size(92, 17);
            this.chkPatcherBasic.TabIndex = 2;
            this.chkPatcherBasic.Text = "Patcher-Basic";
            this.chkPatcherBasic.UseVisualStyleBackColor = true;
            // 
            // chkPatcherAdvanced
            // 
            this.chkPatcherAdvanced.AutoSize = true;
            this.chkPatcherAdvanced.Location = new System.Drawing.Point(6, 66);
            this.chkPatcherAdvanced.Name = "chkPatcherAdvanced";
            this.chkPatcherAdvanced.Size = new System.Drawing.Size(115, 17);
            this.chkPatcherAdvanced.TabIndex = 3;
            this.chkPatcherAdvanced.Text = "Patcher-Advanced";
            this.chkPatcherAdvanced.UseVisualStyleBackColor = true;
            // 
            // chkTunerTourist
            // 
            this.chkTunerTourist.AutoSize = true;
            this.chkTunerTourist.Location = new System.Drawing.Point(6, 21);
            this.chkTunerTourist.Name = "chkTunerTourist";
            this.chkTunerTourist.Size = new System.Drawing.Size(89, 17);
            this.chkTunerTourist.TabIndex = 4;
            this.chkTunerTourist.Text = "Tuner-Tourist";
            this.chkTunerTourist.UseVisualStyleBackColor = true;
            // 
            // chkTunerBasic
            // 
            this.chkTunerBasic.AutoSize = true;
            this.chkTunerBasic.Location = new System.Drawing.Point(6, 43);
            this.chkTunerBasic.Name = "chkTunerBasic";
            this.chkTunerBasic.Size = new System.Drawing.Size(83, 17);
            this.chkTunerBasic.TabIndex = 5;
            this.chkTunerBasic.Text = "Tuner-Basic";
            this.chkTunerBasic.UseVisualStyleBackColor = true;
            // 
            // chkTunerAdvanced
            // 
            this.chkTunerAdvanced.AutoSize = true;
            this.chkTunerAdvanced.Location = new System.Drawing.Point(6, 66);
            this.chkTunerAdvanced.Name = "chkTunerAdvanced";
            this.chkTunerAdvanced.Size = new System.Drawing.Size(106, 17);
            this.chkTunerAdvanced.TabIndex = 6;
            this.chkTunerAdvanced.Text = "Tuner-Advanced";
            this.chkTunerAdvanced.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(323, 237);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 7;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // chkLoggerAdvanced
            // 
            this.chkLoggerAdvanced.AutoSize = true;
            this.chkLoggerAdvanced.Location = new System.Drawing.Point(10, 66);
            this.chkLoggerAdvanced.Name = "chkLoggerAdvanced";
            this.chkLoggerAdvanced.Size = new System.Drawing.Size(111, 17);
            this.chkLoggerAdvanced.TabIndex = 8;
            this.chkLoggerAdvanced.Text = "Logger-Advanced";
            this.chkLoggerAdvanced.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkPatcherAdvanced);
            this.groupBox2.Controls.Add(this.chkPatcherTourist);
            this.groupBox2.Controls.Add(this.chkPatcherBasic);
            this.groupBox2.Location = new System.Drawing.Point(12, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(142, 91);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Patcher";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkTunerBasic);
            this.groupBox3.Controls.Add(this.chkTunerTourist);
            this.groupBox3.Controls.Add(this.chkTunerAdvanced);
            this.groupBox3.Location = new System.Drawing.Point(12, 177);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(141, 88);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tuner";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkLoggerBasic);
            this.groupBox4.Controls.Add(this.chkLoggerTourist);
            this.groupBox4.Controls.Add(this.chkLoggerAdvanced);
            this.groupBox4.Location = new System.Drawing.Point(160, 80);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(146, 91);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Logger";
            // 
            // chkLoggerBasic
            // 
            this.chkLoggerBasic.AutoSize = true;
            this.chkLoggerBasic.Location = new System.Drawing.Point(10, 43);
            this.chkLoggerBasic.Name = "chkLoggerBasic";
            this.chkLoggerBasic.Size = new System.Drawing.Size(88, 17);
            this.chkLoggerBasic.TabIndex = 10;
            this.chkLoggerBasic.Text = "Logger-Basic";
            this.chkLoggerBasic.UseVisualStyleBackColor = true;
            // 
            // chkLoggerTourist
            // 
            this.chkLoggerTourist.AutoSize = true;
            this.chkLoggerTourist.Location = new System.Drawing.Point(10, 20);
            this.chkLoggerTourist.Name = "chkLoggerTourist";
            this.chkLoggerTourist.Size = new System.Drawing.Size(94, 17);
            this.chkLoggerTourist.TabIndex = 9;
            this.chkLoggerTourist.Text = "Logger-Tourist";
            this.chkLoggerTourist.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkSerialportServer);
            this.groupBox5.Location = new System.Drawing.Point(164, 177);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(141, 87);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Serialport server";
            // 
            // chkSerialportServer
            // 
            this.chkSerialportServer.AutoSize = true;
            this.chkSerialportServer.Location = new System.Drawing.Point(14, 22);
            this.chkSerialportServer.Name = "chkSerialportServer";
            this.chkSerialportServer.Size = new System.Drawing.Size(104, 17);
            this.chkSerialportServer.TabIndex = 0;
            this.chkSerialportServer.Text = "Serialport Server";
            this.chkSerialportServer.UseVisualStyleBackColor = true;
            // 
            // frmCreateShortcuts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 270);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmCreateShortcuts";
            this.Text = "Create program shortcuts";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioCustom;
        private System.Windows.Forms.RadioButton radioDesktop;
        private System.Windows.Forms.CheckBox chkPatcherTourist;
        private System.Windows.Forms.CheckBox chkPatcherBasic;
        private System.Windows.Forms.CheckBox chkPatcherAdvanced;
        private System.Windows.Forms.CheckBox chkTunerTourist;
        private System.Windows.Forms.CheckBox chkTunerBasic;
        private System.Windows.Forms.CheckBox chkTunerAdvanced;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.CheckBox chkLoggerAdvanced;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkLoggerBasic;
        private System.Windows.Forms.CheckBox chkLoggerTourist;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkSerialportServer;
    }
}