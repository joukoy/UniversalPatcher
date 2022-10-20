namespace UniversalPatcher
{
    partial class frmSetDTC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetDTC));
            this.comboDtcStatus = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelMil = new System.Windows.Forms.Label();
            this.comboMIL = new System.Windows.Forms.ComboBox();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.labelType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboDtcStatus
            // 
            this.comboDtcStatus.FormattingEnabled = true;
            this.comboDtcStatus.Location = new System.Drawing.Point(76, 26);
            this.comboDtcStatus.Name = "comboDtcStatus";
            this.comboDtcStatus.Size = new System.Drawing.Size(313, 21);
            this.comboDtcStatus.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Status:";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Location = new System.Drawing.Point(9, 7);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(32, 13);
            this.labelCode.TabIndex = 2;
            this.labelCode.Text = "Code";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(314, 106);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Set";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(12, 106);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(73, 7);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(10, 13);
            this.labelDescription.TabIndex = 5;
            this.labelDescription.Text = "-";
            // 
            // labelMil
            // 
            this.labelMil.AutoSize = true;
            this.labelMil.Location = new System.Drawing.Point(9, 52);
            this.labelMil.Name = "labelMil";
            this.labelMil.Size = new System.Drawing.Size(28, 13);
            this.labelMil.TabIndex = 6;
            this.labelMil.Text = "MIL:";
            // 
            // comboMIL
            // 
            this.comboMIL.FormattingEnabled = true;
            this.comboMIL.Location = new System.Drawing.Point(75, 52);
            this.comboMIL.Name = "comboMIL";
            this.comboMIL.Size = new System.Drawing.Size(313, 21);
            this.comboMIL.TabIndex = 7;
            // 
            // comboType
            // 
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(76, 79);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(313, 21);
            this.comboType.TabIndex = 9;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(10, 79);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(34, 13);
            this.labelType.TabIndex = 8;
            this.labelType.Text = "Type:";
            // 
            // frmSetDTC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 137);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.comboMIL);
            this.Controls.Add(this.labelMil);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboDtcStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSetDTC";
            this.Text = "Set DTC";
            this.Load += new System.EventHandler(this.frmSetDTC_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.ComboBox comboDtcStatus;
        public System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelMil;
        public System.Windows.Forms.ComboBox comboMIL;
        public System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label labelType;
    }
}