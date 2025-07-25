
namespace UniversalPatcher
{
    partial class frmPidMismatch
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
            this.labelMessage = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkSaveSelection = new System.Windows.Forms.CheckBox();
            this.radioUseProfile = new System.Windows.Forms.RadioButton();
            this.radioUseMaster = new System.Windows.Forms.RadioButton();
            this.radioNewCustom = new System.Windows.Forms.RadioButton();
            this.labelMasterPid = new System.Windows.Forms.Label();
            this.labelProfilePid = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.radioUseMasterAndSave = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new System.Drawing.Point(16, 14);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(35, 13);
            this.labelMessage.TabIndex = 0;
            this.labelMessage.Text = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioUseMasterAndSave);
            this.groupBox1.Controls.Add(this.btnOK);
            this.groupBox1.Controls.Add(this.chkSaveSelection);
            this.groupBox1.Controls.Add(this.radioUseProfile);
            this.groupBox1.Controls.Add(this.radioUseMaster);
            this.groupBox1.Controls.Add(this.radioNewCustom);
            this.groupBox1.Location = new System.Drawing.Point(12, 172);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(761, 143);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(677, 105);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(78, 29);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkSaveSelection
            // 
            this.chkSaveSelection.AutoSize = true;
            this.chkSaveSelection.Location = new System.Drawing.Point(13, 112);
            this.chkSaveSelection.Name = "chkSaveSelection";
            this.chkSaveSelection.Size = new System.Drawing.Size(122, 17);
            this.chkSaveSelection.TabIndex = 3;
            this.chkSaveSelection.Text = "Remember selection";
            this.chkSaveSelection.UseVisualStyleBackColor = true;
            // 
            // radioUseProfile
            // 
            this.radioUseProfile.AutoSize = true;
            this.radioUseProfile.Location = new System.Drawing.Point(13, 89);
            this.radioUseProfile.Name = "radioUseProfile";
            this.radioUseProfile.Size = new System.Drawing.Size(160, 17);
            this.radioUseProfile.TabIndex = 2;
            this.radioUseProfile.TabStop = true;
            this.radioUseProfile.Text = "Update masterlist from profile";
            this.radioUseProfile.UseVisualStyleBackColor = true;
            // 
            // radioUseMaster
            // 
            this.radioUseMaster.AutoSize = true;
            this.radioUseMaster.Location = new System.Drawing.Point(13, 46);
            this.radioUseMaster.Name = "radioUseMaster";
            this.radioUseMaster.Size = new System.Drawing.Size(137, 17);
            this.radioUseMaster.TabIndex = 1;
            this.radioUseMaster.TabStop = true;
            this.radioUseMaster.Text = "Use PID from master list";
            this.radioUseMaster.UseVisualStyleBackColor = true;
            // 
            // radioNewCustom
            // 
            this.radioNewCustom.AutoSize = true;
            this.radioNewCustom.Checked = true;
            this.radioNewCustom.Location = new System.Drawing.Point(13, 23);
            this.radioNewCustom.Name = "radioNewCustom";
            this.radioNewCustom.Size = new System.Drawing.Size(139, 17);
            this.radioNewCustom.TabIndex = 0;
            this.radioNewCustom.TabStop = true;
            this.radioNewCustom.Text = "Add as new custom PID";
            this.radioNewCustom.UseVisualStyleBackColor = true;
            // 
            // labelMasterPid
            // 
            this.labelMasterPid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelMasterPid.Location = new System.Drawing.Point(17, 55);
            this.labelMasterPid.Name = "labelMasterPid";
            this.labelMasterPid.Size = new System.Drawing.Size(364, 114);
            this.labelMasterPid.TabIndex = 2;
            this.labelMasterPid.Text = "label2";
            // 
            // labelProfilePid
            // 
            this.labelProfilePid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelProfilePid.Location = new System.Drawing.Point(400, 55);
            this.labelProfilePid.Name = "labelProfilePid";
            this.labelProfilePid.Size = new System.Drawing.Size(373, 114);
            this.labelProfilePid.TabIndex = 3;
            this.labelProfilePid.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Master list PID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(397, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Profile PID:";
            // 
            // radioUseMasterAndSave
            // 
            this.radioUseMasterAndSave.AutoSize = true;
            this.radioUseMasterAndSave.Location = new System.Drawing.Point(13, 66);
            this.radioUseMasterAndSave.Name = "radioUseMasterAndSave";
            this.radioUseMasterAndSave.Size = new System.Drawing.Size(257, 17);
            this.radioUseMasterAndSave.TabIndex = 5;
            this.radioUseMasterAndSave.TabStop = true;
            this.radioUseMasterAndSave.Text = "Use PID from master list and save updated profile";
            this.radioUseMasterAndSave.UseVisualStyleBackColor = true;
            // 
            // frmPidMismatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 324);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelProfilePid);
            this.Controls.Add(this.labelMasterPid);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelMessage);
            this.Name = "frmPidMismatch";
            this.Text = "Resolve Pid Mismatch";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.CheckBox chkSaveSelection;
        public System.Windows.Forms.RadioButton radioUseProfile;
        public System.Windows.Forms.RadioButton radioUseMaster;
        public System.Windows.Forms.RadioButton radioNewCustom;
        public System.Windows.Forms.Label labelMasterPid;
        public System.Windows.Forms.Label labelProfilePid;
        public System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.RadioButton radioUseMasterAndSave;
    }
}