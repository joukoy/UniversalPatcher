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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPatcher
            // 
            this.btnPatcher.Location = new System.Drawing.Point(12, 53);
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
            this.btnTuner.Location = new System.Drawing.Point(12, 7);
            this.btnTuner.Name = "btnTuner";
            this.btnTuner.Size = new System.Drawing.Size(96, 40);
            this.btnTuner.TabIndex = 3;
            this.btnTuner.Text = "Tuner";
            this.btnTuner.UseVisualStyleBackColor = true;
            this.btnTuner.Click += new System.EventHandler(this.btnTuner_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(12, 99);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(96, 40);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(12, 145);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(96, 40);
            this.btnAbout.TabIndex = 5;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 229);
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
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnPatcher;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnTuner;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnAbout;
    }
}