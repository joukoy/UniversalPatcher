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
            this.btnAbout = new System.Windows.Forms.Button();
            this.groupMode = new System.Windows.Forms.GroupBox();
            this.radioAdvanced = new System.Windows.Forms.RadioButton();
            this.radioBasic = new System.Windows.Forms.RadioButton();
            this.radioTourist = new System.Windows.Forms.RadioButton();
            this.btnLogger = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioPatcher = new System.Windows.Forms.RadioButton();
            this.radioTuner = new System.Windows.Forms.RadioButton();
            this.radioLogger = new System.Windows.Forms.RadioButton();
            this.radioLauncher = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createShortcutsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serialportServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupMode.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPatcher
            // 
            this.btnPatcher.Location = new System.Drawing.Point(12, 142);
            this.btnPatcher.Name = "btnPatcher";
            this.btnPatcher.Size = new System.Drawing.Size(96, 23);
            this.btnPatcher.TabIndex = 1;
            this.btnPatcher.Text = "Patcher";
            this.btnPatcher.UseVisualStyleBackColor = true;
            this.btnPatcher.Click += new System.EventHandler(this.btnPatcher_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::UniversalPatcher.Properties.Resources.UniversalPatcher;
            this.pictureBox1.Location = new System.Drawing.Point(115, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(320, 218);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnTuner
            // 
            this.btnTuner.Location = new System.Drawing.Point(12, 113);
            this.btnTuner.Name = "btnTuner";
            this.btnTuner.Size = new System.Drawing.Size(96, 23);
            this.btnTuner.TabIndex = 1;
            this.btnTuner.Text = "Tuner";
            this.btnTuner.UseVisualStyleBackColor = true;
            this.btnTuner.Click += new System.EventHandler(this.btnTuner_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(12, 205);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(96, 23);
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
            this.groupMode.Location = new System.Drawing.Point(12, 26);
            this.groupMode.Name = "groupMode";
            this.groupMode.Size = new System.Drawing.Size(96, 79);
            this.groupMode.TabIndex = 6;
            this.groupMode.TabStop = false;
            this.groupMode.Text = "Mode";
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
            // btnLogger
            // 
            this.btnLogger.Location = new System.Drawing.Point(13, 174);
            this.btnLogger.Name = "btnLogger";
            this.btnLogger.Size = new System.Drawing.Size(96, 23);
            this.btnLogger.TabIndex = 7;
            this.btnLogger.Text = "Logger";
            this.btnLogger.UseVisualStyleBackColor = true;
            this.btnLogger.Click += new System.EventHandler(this.btnLogger_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioPatcher);
            this.groupBox1.Controls.Add(this.radioTuner);
            this.groupBox1.Controls.Add(this.radioLogger);
            this.groupBox1.Controls.Add(this.radioLauncher);
            this.groupBox1.Location = new System.Drawing.Point(13, 230);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(422, 47);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Open at startup";
            // 
            // radioPatcher
            // 
            this.radioPatcher.AutoSize = true;
            this.radioPatcher.Location = new System.Drawing.Point(292, 21);
            this.radioPatcher.Name = "radioPatcher";
            this.radioPatcher.Size = new System.Drawing.Size(62, 17);
            this.radioPatcher.TabIndex = 3;
            this.radioPatcher.TabStop = true;
            this.radioPatcher.Text = "Patcher";
            this.radioPatcher.UseVisualStyleBackColor = true;
            this.radioPatcher.CheckedChanged += new System.EventHandler(this.radioPatcher_CheckedChanged);
            // 
            // radioTuner
            // 
            this.radioTuner.AutoSize = true;
            this.radioTuner.Location = new System.Drawing.Point(211, 21);
            this.radioTuner.Name = "radioTuner";
            this.radioTuner.Size = new System.Drawing.Size(53, 17);
            this.radioTuner.TabIndex = 2;
            this.radioTuner.TabStop = true;
            this.radioTuner.Text = "Tuner";
            this.radioTuner.UseVisualStyleBackColor = true;
            this.radioTuner.CheckedChanged += new System.EventHandler(this.radioTuner_CheckedChanged);
            // 
            // radioLogger
            // 
            this.radioLogger.AutoSize = true;
            this.radioLogger.Location = new System.Drawing.Point(137, 21);
            this.radioLogger.Name = "radioLogger";
            this.radioLogger.Size = new System.Drawing.Size(58, 17);
            this.radioLogger.TabIndex = 1;
            this.radioLogger.TabStop = true;
            this.radioLogger.Text = "Logger";
            this.radioLogger.UseVisualStyleBackColor = true;
            this.radioLogger.CheckedChanged += new System.EventHandler(this.radioLogger_CheckedChanged);
            // 
            // radioLauncher
            // 
            this.radioLauncher.AutoSize = true;
            this.radioLauncher.Checked = true;
            this.radioLauncher.Location = new System.Drawing.Point(14, 21);
            this.radioLauncher.Name = "radioLauncher";
            this.radioLauncher.Size = new System.Drawing.Size(95, 17);
            this.radioLauncher.TabIndex = 0;
            this.radioLauncher.TabStop = true;
            this.radioLauncher.Text = "Launcher (this)";
            this.radioLauncher.UseVisualStyleBackColor = true;
            this.radioLauncher.CheckedChanged += new System.EventHandler(this.radioLauncher_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(444, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createShortcutsToolStripMenuItem,
            this.serialportServerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // createShortcutsToolStripMenuItem
            // 
            this.createShortcutsToolStripMenuItem.Name = "createShortcutsToolStripMenuItem";
            this.createShortcutsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.createShortcutsToolStripMenuItem.Text = "Create shortcuts";
            this.createShortcutsToolStripMenuItem.Click += new System.EventHandler(this.createShortcutsToolStripMenuItem_Click);
            // 
            // serialportServerToolStripMenuItem
            // 
            this.serialportServerToolStripMenuItem.Name = "serialportServerToolStripMenuItem";
            this.serialportServerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.serialportServerToolStripMenuItem.Text = "Serialport server";
            this.serialportServerToolStripMenuItem.Click += new System.EventHandler(this.serialportServerToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 288);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnLogger);
            this.Controls.Add(this.groupMode);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnTuner);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnPatcher);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "Universal Patcher";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupMode.ResumeLayout(false);
            this.groupMode.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnPatcher;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnTuner;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.GroupBox groupMode;
        private System.Windows.Forms.RadioButton radioAdvanced;
        private System.Windows.Forms.RadioButton radioBasic;
        private System.Windows.Forms.RadioButton radioTourist;
        private System.Windows.Forms.Button btnLogger;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioLauncher;
        private System.Windows.Forms.RadioButton radioPatcher;
        private System.Windows.Forms.RadioButton radioTuner;
        private System.Windows.Forms.RadioButton radioLogger;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createShortcutsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serialportServerToolStripMenuItem;
    }
}