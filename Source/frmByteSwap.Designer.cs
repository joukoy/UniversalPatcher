
namespace UniversalPatcher
{
    partial class frmByteSwap
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
            this.radioTwo = new System.Windows.Forms.RadioButton();
            this.radioFour = new System.Windows.Forms.RadioButton();
            this.radioEight = new System.Windows.Forms.RadioButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnApply);
            this.groupBox1.Controls.Add(this.radioEight);
            this.groupBox1.Controls.Add(this.radioFour);
            this.groupBox1.Controls.Add(this.radioTwo);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 115);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Treat data as";
            // 
            // radioTwo
            // 
            this.radioTwo.AutoSize = true;
            this.radioTwo.Checked = true;
            this.radioTwo.Location = new System.Drawing.Point(20, 24);
            this.radioTwo.Name = "radioTwo";
            this.radioTwo.Size = new System.Drawing.Size(96, 17);
            this.radioTwo.TabIndex = 0;
            this.radioTwo.TabStop = true;
            this.radioTwo.Text = "Ushort (16 bits)";
            this.radioTwo.UseVisualStyleBackColor = true;
            // 
            // radioFour
            // 
            this.radioFour.AutoSize = true;
            this.radioFour.Location = new System.Drawing.Point(20, 47);
            this.radioFour.Name = "radioFour";
            this.radioFour.Size = new System.Drawing.Size(91, 17);
            this.radioFour.TabIndex = 1;
            this.radioFour.Text = "Dword (32 bit)";
            this.radioFour.UseVisualStyleBackColor = true;
            // 
            // radioEight
            // 
            this.radioEight.AutoSize = true;
            this.radioEight.Location = new System.Drawing.Point(20, 70);
            this.radioEight.Name = "radioEight";
            this.radioEight.Size = new System.Drawing.Size(86, 17);
            this.radioEight.TabIndex = 2;
            this.radioEight.Text = "Quad (64 bit)";
            this.radioEight.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(167, 47);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // frmByteSwap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 141);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmByteSwap";
            this.Text = "Swap bytes";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnApply;
        public System.Windows.Forms.RadioButton radioEight;
        public System.Windows.Forms.RadioButton radioFour;
        public System.Windows.Forms.RadioButton radioTwo;
    }
}