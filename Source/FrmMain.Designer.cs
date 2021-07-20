namespace UniversalPatcher
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.btnPatcher = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnTuner = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.groupMode = new System.Windows.Forms.GroupBox();
            this.radioTourist = new System.Windows.Forms.RadioButton();
            this.radioBasic = new System.Windows.Forms.RadioButton();
            this.radioAdvanced = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPatcher
            // 
            this.btnPatcher.Location = new System.Drawing.Point(12, 139);
            this.btnPatcher.Name = "btnPatcher";
            this.btnPatcher.Size = new System.Drawing.Size(96, 40);
            this.btnPatcher.TabIndex = 1;
            this.btnPatcher.Text = "Patcher";
            this.btnPatcher.UseVisualStyleBackColor = true;
            this.btnPatcher.Click += new System.EventHandler(this.btnPatcher_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::UniversalPatcher.Properties.Resources.UniversalPatcher;
            this.pictureBox1.Location = new System.Drawing.Point(119, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(321, 217);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnTuner
            // 
            this.btnTuner.Location = new System.Drawing.Point(12, 93);
            this.btnTuner.Name = "btnTuner";
            this.btnTuner.Size = new System.Drawing.Size(96, 40);
            this.btnTuner.TabIndex = 3;
            this.btnTuner.Text = "Tuner";
            this.btnTuner.UseVisualStyleBackColor = true;
            this.btnTuner.Click += new System.EventHandler(this.btnTuner_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Enabled = false;
            this.btnSettings.Location = new System.Drawing.Point(220, 145);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(96, 40);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Visible = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(12, 185);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(96, 40);
            this.btnAbout.TabIndex = 5;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // groupMode
            // 
            this.groupMode.Controls.Add(this.radioAdvanced);
            this.groupMode.Controls.Add(this.radioBasic);
            this.groupMode.Controls.Add(this.radioTourist);
            this.groupMode.Location = new System.Drawing.Point(12, 6);
            this.groupMode.Name = "groupMode";
            this.groupMode.Size = new System.Drawing.Size(96, 79);
            this.groupMode.TabIndex = 6;
            this.groupMode.TabStop = false;
            this.groupMode.Text = "Mode";
            // 
            // radioTourist
            // 
            this.radioTourist.AutoSize = true;
            this.radioTourist.Checked = true;
            this.radioTourist.Location = new System.Drawing.Point(6, 16);
            this.radioTourist.Name = "radioTourist";
            this.radioTourist.Size = new System.Drawing.Size(57, 17);
            this.radioTourist.TabIndex = 0;
            this.radioTourist.TabStop = true;
            this.radioTourist.Text = "Tourist";
            this.radioTourist.UseVisualStyleBackColor = true;
            this.radioTourist.CheckedChanged += new System.EventHandler(this.radioTourist_CheckedChanged);
            // 
            // radioBasic
            // 
            this.radioBasic.AutoSize = true;
            this.radioBasic.Location = new System.Drawing.Point(6, 35);
            this.radioBasic.Name = "radioBasic";
            this.radioBasic.Size = new System.Drawing.Size(51, 17);
            this.radioBasic.TabIndex = 1;
            this.radioBasic.TabStop = true;
            this.radioBasic.Text = "Basic";
            this.radioBasic.UseVisualStyleBackColor = true;
            this.radioBasic.CheckedChanged += new System.EventHandler(this.radioBasic_CheckedChanged);
            // 
            // radioAdvanced
            // 
            this.radioAdvanced.AutoSize = true;
            this.radioAdvanced.Location = new System.Drawing.Point(6, 56);
            this.radioAdvanced.Name = "radioAdvanced";
            this.radioAdvanced.Size = new System.Drawing.Size(74, 17);
            this.radioAdvanced.TabIndex = 2;
            this.radioAdvanced.TabStop = true;
            this.radioAdvanced.Text = "Advanced";
            this.radioAdvanced.UseVisualStyleBackColor = true;
            this.radioAdvanced.CheckedChanged += new System.EventHandler(this.radioAdvanced_CheckedChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 233);
            this.Controls.Add(this.groupMode);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnTuner);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnPatcher);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "Universal Patcher";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupMode.ResumeLayout(false);
            this.groupMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnPatcher;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnTuner;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.GroupBox groupMode;
        private System.Windows.Forms.RadioButton radioAdvanced;
        private System.Windows.Forms.RadioButton radioBasic;
        private System.Windows.Forms.RadioButton radioTourist;
    }
}