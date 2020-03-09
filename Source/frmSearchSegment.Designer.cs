namespace UniversalPatcher
{
    partial class frmSearchSegment
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
            this.chkNot = new System.Windows.Forms.CheckBox();
            this.txtSearchfor = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearchAddresses = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkNot
            // 
            this.chkNot.AutoSize = true;
            this.chkNot.Checked = true;
            this.chkNot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNot.Location = new System.Drawing.Point(113, 51);
            this.chkNot.Name = "chkNot";
            this.chkNot.Size = new System.Drawing.Size(120, 17);
            this.chkNot.TabIndex = 3;
            this.chkNot.Text = "Match if NOT found";
            this.chkNot.UseVisualStyleBackColor = true;
            // 
            // txtSearchfor
            // 
            this.txtSearchfor.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchfor.Location = new System.Drawing.Point(113, 25);
            this.txtSearchfor.Name = "txtSearchfor";
            this.txtSearchfor.Size = new System.Drawing.Size(193, 20);
            this.txtSearchfor.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Search for:from";
            // 
            // txtSearchAddresses
            // 
            this.txtSearchAddresses.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchAddresses.Location = new System.Drawing.Point(113, 1);
            this.txtSearchAddresses.Name = "txtSearchAddresses";
            this.txtSearchAddresses.Size = new System.Drawing.Size(194, 20);
            this.txtSearchAddresses.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Segment addresses:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(323, 39);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(59, 29);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmSearchSegment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 81);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkNot);
            this.Controls.Add(this.txtSearchfor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSearchAddresses);
            this.Controls.Add(this.label1);
            this.Name = "frmSearchSegment";
            this.Text = "Search segment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkNot;
        private System.Windows.Forms.TextBox txtSearchfor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSearchAddresses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
    }
}