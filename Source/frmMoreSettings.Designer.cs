
namespace UniversalPatcher
{
    partial class frmMoreSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMoreSettings));
            this.numTunerTableMinEquivalency = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numKeypressWait = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numTunerMinEqOther = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMultitableChars = new System.Windows.Forms.TextBox();
            this.chkDisableAutoCS = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkRequireValidVerForStock = new System.Windows.Forms.CheckBox();
            this.chkAutoOpenImportedFile = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkDisplayMetric = new System.Windows.Forms.CheckBox();
            this.chkDisplayImperial = new System.Windows.Forms.CheckBox();
            this.chkDisplayUndefined = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numSplashTime = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numTunerTableMinEquivalency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeypressWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTunerMinEqOther)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSplashTime)).BeginInit();
            this.SuspendLayout();
            // 
            // numTunerTableMinEquivalency
            // 
            this.numTunerTableMinEquivalency.Location = new System.Drawing.Point(250, 23);
            this.numTunerTableMinEquivalency.Name = "numTunerTableMinEquivalency";
            this.numTunerTableMinEquivalency.Size = new System.Drawing.Size(44, 20);
            this.numTunerTableMinEquivalency.TabIndex = 0;
            this.numTunerTableMinEquivalency.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Minimum equivalency in table names (%)";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(225, 318);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(213, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "x 100ms wait between keypress and search";
            // 
            // numKeypressWait
            // 
            this.numKeypressWait.Location = new System.Drawing.Point(249, 16);
            this.numKeypressWait.Name = "numKeypressWait";
            this.numKeypressWait.Size = new System.Drawing.Size(44, 20);
            this.numKeypressWait.TabIndex = 5;
            this.numKeypressWait.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numKeypressWait.ValueChanged += new System.EventHandler(this.numKeypressWait_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(238, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Minimum equivalency in other table properties (%)";
            // 
            // numTunerMinEqOther
            // 
            this.numTunerMinEqOther.Location = new System.Drawing.Point(250, 47);
            this.numTunerMinEqOther.Name = "numTunerMinEqOther";
            this.numTunerMinEqOther.Size = new System.Drawing.Size(44, 20);
            this.numTunerMinEqOther.TabIndex = 6;
            this.numTunerMinEqOther.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numTunerTableMinEquivalency);
            this.groupBox1.Controls.Add(this.numTunerMinEqOther);
            this.groupBox1.Location = new System.Drawing.Point(8, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 83);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Matching table search";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numKeypressWait);
            this.groupBox2.Location = new System.Drawing.Point(9, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 47);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "List filtering";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Multitable chars (space separated)";
            // 
            // txtMultitableChars
            // 
            this.txtMultitableChars.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMultitableChars.Location = new System.Drawing.Point(188, 151);
            this.txtMultitableChars.Name = "txtMultitableChars";
            this.txtMultitableChars.Size = new System.Drawing.Size(113, 23);
            this.txtMultitableChars.TabIndex = 11;
            // 
            // chkDisableAutoCS
            // 
            this.chkDisableAutoCS.AutoSize = true;
            this.chkDisableAutoCS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkDisableAutoCS.Location = new System.Drawing.Point(9, 237);
            this.chkDisableAutoCS.Name = "chkDisableAutoCS";
            this.chkDisableAutoCS.Size = new System.Drawing.Size(150, 17);
            this.chkDisableAutoCS.TabIndex = 12;
            this.chkDisableAutoCS.Text = "Disable auto checksum fix";
            this.chkDisableAutoCS.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkRequireValidVerForStock);
            this.groupBox3.Location = new System.Drawing.Point(10, 180);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(292, 51);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Stock CVN Check";
            // 
            // chkRequireValidVerForStock
            // 
            this.chkRequireValidVerForStock.AutoSize = true;
            this.chkRequireValidVerForStock.Checked = true;
            this.chkRequireValidVerForStock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRequireValidVerForStock.Location = new System.Drawing.Point(10, 19);
            this.chkRequireValidVerForStock.Name = "chkRequireValidVerForStock";
            this.chkRequireValidVerForStock.Size = new System.Drawing.Size(243, 17);
            this.chkRequireValidVerForStock.TabIndex = 0;
            this.chkRequireValidVerForStock.Text = "Mark modified if \"version\" have no valid chars";
            this.chkRequireValidVerForStock.UseVisualStyleBackColor = true;
            // 
            // chkAutoOpenImportedFile
            // 
            this.chkAutoOpenImportedFile.AutoSize = true;
            this.chkAutoOpenImportedFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAutoOpenImportedFile.Location = new System.Drawing.Point(168, 237);
            this.chkAutoOpenImportedFile.Name = "chkAutoOpenImportedFile";
            this.chkAutoOpenImportedFile.Size = new System.Drawing.Size(134, 17);
            this.chkAutoOpenImportedFile.TabIndex = 14;
            this.chkAutoOpenImportedFile.Text = "Auto open imported file";
            this.chkAutoOpenImportedFile.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkDisplayMetric);
            this.groupBox4.Controls.Add(this.chkDisplayImperial);
            this.groupBox4.Controls.Add(this.chkDisplayUndefined);
            this.groupBox4.Location = new System.Drawing.Point(9, 260);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(288, 48);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Display units";
            // 
            // chkDisplayMetric
            // 
            this.chkDisplayMetric.AutoSize = true;
            this.chkDisplayMetric.Location = new System.Drawing.Point(172, 19);
            this.chkDisplayMetric.Name = "chkDisplayMetric";
            this.chkDisplayMetric.Size = new System.Drawing.Size(55, 17);
            this.chkDisplayMetric.TabIndex = 2;
            this.chkDisplayMetric.Text = "Metric";
            this.chkDisplayMetric.UseVisualStyleBackColor = true;
            // 
            // chkDisplayImperial
            // 
            this.chkDisplayImperial.AutoSize = true;
            this.chkDisplayImperial.Location = new System.Drawing.Point(94, 18);
            this.chkDisplayImperial.Name = "chkDisplayImperial";
            this.chkDisplayImperial.Size = new System.Drawing.Size(62, 17);
            this.chkDisplayImperial.TabIndex = 1;
            this.chkDisplayImperial.Text = "Imperial";
            this.chkDisplayImperial.UseVisualStyleBackColor = true;
            // 
            // chkDisplayUndefined
            // 
            this.chkDisplayUndefined.AutoSize = true;
            this.chkDisplayUndefined.Location = new System.Drawing.Point(13, 18);
            this.chkDisplayUndefined.Name = "chkDisplayUndefined";
            this.chkDisplayUndefined.Size = new System.Drawing.Size(75, 17);
            this.chkDisplayUndefined.TabIndex = 0;
            this.chkDisplayUndefined.Text = "Undefined";
            this.chkDisplayUndefined.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 323);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Splash screen showtime (s)";
            // 
            // numSplashTime
            // 
            this.numSplashTime.Location = new System.Drawing.Point(160, 321);
            this.numSplashTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numSplashTime.Name = "numSplashTime";
            this.numSplashTime.Size = new System.Drawing.Size(50, 20);
            this.numSplashTime.TabIndex = 17;
            // 
            // frmMoreSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 353);
            this.Controls.Add(this.numSplashTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.chkAutoOpenImportedFile);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.chkDisableAutoCS);
            this.Controls.Add(this.txtMultitableChars);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMoreSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.frmTunerSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numTunerTableMinEquivalency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeypressWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTunerMinEqOther)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSplashTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numTunerTableMinEquivalency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numKeypressWait;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numTunerMinEqOther;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMultitableChars;
        private System.Windows.Forms.CheckBox chkDisableAutoCS;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkRequireValidVerForStock;
        private System.Windows.Forms.CheckBox chkAutoOpenImportedFile;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkDisplayMetric;
        private System.Windows.Forms.CheckBox chkDisplayImperial;
        private System.Windows.Forms.CheckBox chkDisplayUndefined;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numSplashTime;
    }
}