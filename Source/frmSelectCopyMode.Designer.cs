
namespace UniversalPatcher
{
    partial class frmSelectCopyMode
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
            this.radioOwerwrite = new System.Windows.Forms.RadioButton();
            this.radioOwerwriteAdd = new System.Windows.Forms.RadioButton();
            this.radioAddMissing = new System.Windows.Forms.RadioButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioAddMissing);
            this.groupBox1.Controls.Add(this.radioOwerwriteAdd);
            this.groupBox1.Controls.Add(this.radioOwerwrite);
            this.groupBox1.Controls.Add(this.radioAdd);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 116);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Copy mode:";
            // 
            // radioAdd
            // 
            this.radioAdd.AutoSize = true;
            this.radioAdd.Location = new System.Drawing.Point(6, 19);
            this.radioAdd.Name = "radioAdd";
            this.radioAdd.Size = new System.Drawing.Size(81, 17);
            this.radioAdd.TabIndex = 0;
            this.radioAdd.TabStop = true;
            this.radioAdd.Text = "Add as new";
            this.radioAdd.UseVisualStyleBackColor = true;
            // 
            // radioOwerwrite
            // 
            this.radioOwerwrite.AutoSize = true;
            this.radioOwerwrite.Location = new System.Drawing.Point(6, 62);
            this.radioOwerwrite.Name = "radioOwerwrite";
            this.radioOwerwrite.Size = new System.Drawing.Size(104, 17);
            this.radioOwerwrite.TabIndex = 1;
            this.radioOwerwrite.TabStop = true;
            this.radioOwerwrite.Text = "Owerwrite if exist";
            this.radioOwerwrite.UseVisualStyleBackColor = true;
            // 
            // radioOwerwriteAdd
            // 
            this.radioOwerwriteAdd.AutoSize = true;
            this.radioOwerwriteAdd.Location = new System.Drawing.Point(6, 85);
            this.radioOwerwriteAdd.Name = "radioOwerwriteAdd";
            this.radioOwerwriteAdd.Size = new System.Drawing.Size(151, 17);
            this.radioOwerwriteAdd.TabIndex = 2;
            this.radioOwerwriteAdd.TabStop = true;
            this.radioOwerwriteAdd.Text = "Owerwite if exist, add if not";
            this.radioOwerwriteAdd.UseVisualStyleBackColor = true;
            // 
            // radioAddMissing
            // 
            this.radioAddMissing.AutoSize = true;
            this.radioAddMissing.Location = new System.Drawing.Point(6, 39);
            this.radioAddMissing.Name = "radioAddMissing";
            this.radioAddMissing.Size = new System.Drawing.Size(94, 17);
            this.radioAddMissing.TabIndex = 3;
            this.radioAddMissing.TabStop = true;
            this.radioAddMissing.Text = "Add if not exist";
            this.radioAddMissing.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(198, 12);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // frmSelectCopyMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 138);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmSelectCopyMode";
            this.Text = "Select copy mode";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioAddMissing;
        private System.Windows.Forms.RadioButton radioOwerwriteAdd;
        private System.Windows.Forms.RadioButton radioOwerwrite;
        private System.Windows.Forms.RadioButton radioAdd;
        private System.Windows.Forms.Button btnApply;
    }
}