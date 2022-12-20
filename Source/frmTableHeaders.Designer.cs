
namespace UniversalPatcher
{
    partial class frmTableHeaders
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
            this.label3 = new System.Windows.Forms.Label();
            this.numMin = new System.Windows.Forms.NumericUpDown();
            this.numMax = new System.Windows.Forms.NumericUpDown();
            this.numStep = new System.Windows.Forms.NumericUpDown();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStep)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Max:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Step:";
            // 
            // numMin
            // 
            this.numMin.DecimalPlaces = 3;
            this.numMin.Location = new System.Drawing.Point(65, 11);
            this.numMin.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numMin.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numMin.Name = "numMin";
            this.numMin.Size = new System.Drawing.Size(90, 20);
            this.numMin.TabIndex = 3;
            // 
            // numMax
            // 
            this.numMax.DecimalPlaces = 3;
            this.numMax.Location = new System.Drawing.Point(65, 36);
            this.numMax.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numMax.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numMax.Name = "numMax";
            this.numMax.Size = new System.Drawing.Size(89, 20);
            this.numMax.TabIndex = 4;
            // 
            // numStep
            // 
            this.numStep.DecimalPlaces = 3;
            this.numStep.Location = new System.Drawing.Point(65, 61);
            this.numStep.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numStep.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.numStep.Name = "numStep";
            this.numStep.Size = new System.Drawing.Size(88, 20);
            this.numStep.TabIndex = 5;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(179, 14);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(179, 53);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmTableHeaders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 94);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.numStep);
            this.Controls.Add(this.numMax);
            this.Controls.Add(this.numMin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmTableHeaders";
            this.Text = "Set Headers";
            ((System.ComponentModel.ISupportInitialize)(this.numMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.NumericUpDown numMin;
        public System.Windows.Forms.NumericUpDown numMax;
        public System.Windows.Forms.NumericUpDown numStep;
    }
}