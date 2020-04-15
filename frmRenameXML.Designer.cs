namespace UniversalPatcher
{
    partial class frmRenameXML
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOldXML = new System.Windows.Forms.TextBox();
            this.txtNewXML = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Old XML:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "New XML:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txtOldXML
            // 
            this.txtOldXML.Location = new System.Drawing.Point(83, 10);
            this.txtOldXML.Name = "txtOldXML";
            this.txtOldXML.Size = new System.Drawing.Size(172, 20);
            this.txtOldXML.TabIndex = 2;
            // 
            // txtNewXML
            // 
            this.txtNewXML.Location = new System.Drawing.Point(83, 37);
            this.txtNewXML.Name = "txtNewXML";
            this.txtNewXML.Size = new System.Drawing.Size(171, 20);
            this.txtNewXML.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(201, 60);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 27);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmRenameXML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 90);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtNewXML);
            this.Controls.Add(this.txtOldXML);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmRenameXML";
            this.Text = "Rename XML";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.TextBox txtOldXML;
        public System.Windows.Forms.TextBox txtNewXML;
    }
}