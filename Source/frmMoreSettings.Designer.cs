
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
            this.chkStartPatcher = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numTunerTableMinEquivalency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeypressWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTunerMinEqOther)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.btnOK.Location = new System.Drawing.Point(226, 227);
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
            this.chkDisableAutoCS.Location = new System.Drawing.Point(152, 180);
            this.chkDisableAutoCS.Name = "chkDisableAutoCS";
            this.chkDisableAutoCS.Size = new System.Drawing.Size(150, 17);
            this.chkDisableAutoCS.TabIndex = 12;
            this.chkDisableAutoCS.Text = "Disable auto checksum fix";
            this.chkDisableAutoCS.UseVisualStyleBackColor = true;
            // 
            // chkStartPatcher
            // 
            this.chkStartPatcher.AutoSize = true;
            this.chkStartPatcher.Location = new System.Drawing.Point(133, 203);
            this.chkStartPatcher.Name = "chkStartPatcher";
            this.chkStartPatcher.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkStartPatcher.Size = new System.Drawing.Size(169, 17);
            this.chkStartPatcher.TabIndex = 13;
            this.chkStartPatcher.Text = "Set patcher as startup window";
            this.chkStartPatcher.UseVisualStyleBackColor = true;
            // 
            // frmMoreSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 253);
            this.Controls.Add(this.chkStartPatcher);
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
        private System.Windows.Forms.CheckBox chkStartPatcher;
    }
}