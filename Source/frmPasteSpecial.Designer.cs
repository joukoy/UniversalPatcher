
namespace UniversalPatcher
{
    partial class frmPasteSpecial
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
            this.radioAdd = new System.Windows.Forms.RadioButton();
            this.radioMultiply = new System.Windows.Forms.RadioButton();
            this.radioPercent = new System.Windows.Forms.RadioButton();
            this.radioTarget = new System.Windows.Forms.RadioButton();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.radioCustom = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomPositive = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCustomNegative = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtCustomNegative);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCustomPositive);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioCustom);
            this.groupBox1.Controls.Add(this.txtTarget);
            this.groupBox1.Controls.Add(this.radioTarget);
            this.groupBox1.Controls.Add(this.radioPercent);
            this.groupBox1.Controls.Add(this.radioMultiply);
            this.groupBox1.Controls.Add(this.radioAdd);
            this.groupBox1.Location = new System.Drawing.Point(7, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 267);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // radioAdd
            // 
            this.radioAdd.AutoSize = true;
            this.radioAdd.Checked = true;
            this.radioAdd.Location = new System.Drawing.Point(12, 17);
            this.radioAdd.Name = "radioAdd";
            this.radioAdd.Size = new System.Drawing.Size(44, 17);
            this.radioAdd.TabIndex = 0;
            this.radioAdd.TabStop = true;
            this.radioAdd.Text = "Add";
            this.radioAdd.UseVisualStyleBackColor = true;
            // 
            // radioMultiply
            // 
            this.radioMultiply.AutoSize = true;
            this.radioMultiply.Location = new System.Drawing.Point(12, 40);
            this.radioMultiply.Name = "radioMultiply";
            this.radioMultiply.Size = new System.Drawing.Size(60, 17);
            this.radioMultiply.TabIndex = 1;
            this.radioMultiply.Text = "Multiply";
            this.radioMultiply.UseVisualStyleBackColor = true;
            // 
            // radioPercent
            // 
            this.radioPercent.AutoSize = true;
            this.radioPercent.Location = new System.Drawing.Point(12, 63);
            this.radioPercent.Name = "radioPercent";
            this.radioPercent.Size = new System.Drawing.Size(94, 17);
            this.radioPercent.TabIndex = 2;
            this.radioPercent.Text = "Multiply % (+/-)";
            this.radioPercent.UseVisualStyleBackColor = true;
            // 
            // radioTarget
            // 
            this.radioTarget.AutoSize = true;
            this.radioTarget.Location = new System.Drawing.Point(12, 86);
            this.radioTarget.Name = "radioTarget";
            this.radioTarget.Size = new System.Drawing.Size(59, 17);
            this.radioTarget.TabIndex = 3;
            this.radioTarget.Text = "Target:";
            this.radioTarget.UseVisualStyleBackColor = true;
            // 
            // txtTarget
            // 
            this.txtTarget.Location = new System.Drawing.Point(77, 86);
            this.txtTarget.Name = "txtTarget";
            this.txtTarget.Size = new System.Drawing.Size(60, 20);
            this.txtTarget.TabIndex = 4;
            this.txtTarget.Text = "14.7";
            // 
            // radioCustom
            // 
            this.radioCustom.AutoSize = true;
            this.radioCustom.Location = new System.Drawing.Point(12, 109);
            this.radioCustom.Name = "radioCustom";
            this.radioCustom.Size = new System.Drawing.Size(63, 17);
            this.radioCustom.TabIndex = 5;
            this.radioCustom.Text = "Custom:";
            this.radioCustom.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "If clipboard value is positive:";
            // 
            // txtCustomPositive
            // 
            this.txtCustomPositive.Location = new System.Drawing.Point(11, 150);
            this.txtCustomPositive.Name = "txtCustomPositive";
            this.txtCustomPositive.Size = new System.Drawing.Size(229, 20);
            this.txtCustomPositive.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "If clipboard value is negative:";
            // 
            // txtCustomNegative
            // 
            this.txtCustomNegative.Location = new System.Drawing.Point(12, 189);
            this.txtCustomNegative.Name = "txtCustomNegative";
            this.txtCustomNegative.Size = new System.Drawing.Size(228, 20);
            this.txtCustomNegative.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 218);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Table value = X, Clipboard value = C";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 241);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Example: C * 0.5 + X";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 274);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(189, 274);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmPasteSpecial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 306);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmPasteSpecial";
            this.Text = "Paste as:";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.TextBox txtCustomNegative;
        public System.Windows.Forms.TextBox txtCustomPositive;
        public System.Windows.Forms.RadioButton radioCustom;
        public System.Windows.Forms.TextBox txtTarget;
        public System.Windows.Forms.RadioButton radioTarget;
        public System.Windows.Forms.RadioButton radioPercent;
        public System.Windows.Forms.RadioButton radioMultiply;
        public System.Windows.Forms.RadioButton radioAdd;
    }
}