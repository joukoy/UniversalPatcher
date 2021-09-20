
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
            this.radioDesktop = new System.Windows.Forms.RadioButton();
            this.radioCustom = new System.Windows.Forms.RadioButton();
            this.chkPatcherTourist = new System.Windows.Forms.CheckBox();
            this.chkPatcherBasic = new System.Windows.Forms.CheckBox();
            this.chkPatcherAdvanced = new System.Windows.Forms.CheckBox();
            this.chkTunerTourist = new System.Windows.Forms.CheckBox();
            this.chkTunerBasic = new System.Windows.Forms.CheckBox();
            this.chkTunerAdvanced = new System.Windows.Forms.CheckBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
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
            this.groupBox1.Size = new System.Drawing.Size(354, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Location";
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
            // chkPatcherTourist
            // 
            this.chkPatcherTourist.AutoSize = true;
            this.chkPatcherTourist.Location = new System.Drawing.Point(24, 80);
            this.chkPatcherTourist.Name = "chkPatcherTourist";
            this.chkPatcherTourist.Size = new System.Drawing.Size(98, 17);
            this.chkPatcherTourist.TabIndex = 1;
            this.chkPatcherTourist.Text = "Patcher-Tourist";
            this.chkPatcherTourist.UseVisualStyleBackColor = true;
            // 
            // chkPatcherBasic
            // 
            this.chkPatcherBasic.AutoSize = true;
            this.chkPatcherBasic.Location = new System.Drawing.Point(24, 102);
            this.chkPatcherBasic.Name = "chkPatcherBasic";
            this.chkPatcherBasic.Size = new System.Drawing.Size(92, 17);
            this.chkPatcherBasic.TabIndex = 2;
            this.chkPatcherBasic.Text = "Patcher-Basic";
            this.chkPatcherBasic.UseVisualStyleBackColor = true;
            // 
            // chkPatcherAdvanced
            // 
            this.chkPatcherAdvanced.AutoSize = true;
            this.chkPatcherAdvanced.Location = new System.Drawing.Point(24, 125);
            this.chkPatcherAdvanced.Name = "chkPatcherAdvanced";
            this.chkPatcherAdvanced.Size = new System.Drawing.Size(115, 17);
            this.chkPatcherAdvanced.TabIndex = 3;
            this.chkPatcherAdvanced.Text = "Patcher-Advanced";
            this.chkPatcherAdvanced.UseVisualStyleBackColor = true;
            // 
            // chkTunerTourist
            // 
            this.chkTunerTourist.AutoSize = true;
            this.chkTunerTourist.Location = new System.Drawing.Point(171, 80);
            this.chkTunerTourist.Name = "chkTunerTourist";
            this.chkTunerTourist.Size = new System.Drawing.Size(89, 17);
            this.chkTunerTourist.TabIndex = 4;
            this.chkTunerTourist.Text = "Tuner-Tourist";
            this.chkTunerTourist.UseVisualStyleBackColor = true;
            // 
            // chkTunerBasic
            // 
            this.chkTunerBasic.AutoSize = true;
            this.chkTunerBasic.Location = new System.Drawing.Point(171, 102);
            this.chkTunerBasic.Name = "chkTunerBasic";
            this.chkTunerBasic.Size = new System.Drawing.Size(83, 17);
            this.chkTunerBasic.TabIndex = 5;
            this.chkTunerBasic.Text = "Tuner-Basic";
            this.chkTunerBasic.UseVisualStyleBackColor = true;
            // 
            // chkTunerAdvanced
            // 
            this.chkTunerAdvanced.AutoSize = true;
            this.chkTunerAdvanced.Location = new System.Drawing.Point(171, 125);
            this.chkTunerAdvanced.Name = "chkTunerAdvanced";
            this.chkTunerAdvanced.Size = new System.Drawing.Size(106, 17);
            this.chkTunerAdvanced.TabIndex = 6;
            this.chkTunerAdvanced.Text = "Tuner-Advanced";
            this.chkTunerAdvanced.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(283, 119);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 7;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // frmCreateShortcuts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 156);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.chkTunerAdvanced);
            this.Controls.Add(this.chkTunerBasic);
            this.Controls.Add(this.chkTunerTourist);
            this.Controls.Add(this.chkPatcherAdvanced);
            this.Controls.Add(this.chkPatcherBasic);
            this.Controls.Add(this.chkPatcherTourist);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmCreateShortcuts";
            this.Text = "Create program shortcuts";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}