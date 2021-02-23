
namespace UniversalPatcher
{
    partial class frmMoreSettings
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
            this.numTunerTableMinEquivalency = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numKeypressWait = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numTunerTableMinEquivalency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeypressWait)).BeginInit();
            this.SuspendLayout();
            // 
            // numTunerTableMinEquivalency
            // 
            this.numTunerTableMinEquivalency.Location = new System.Drawing.Point(207, 12);
            this.numTunerTableMinEquivalency.Name = "numTunerTableMinEquivalency";
            this.numTunerTableMinEquivalency.Size = new System.Drawing.Size(44, 20);
            this.numTunerTableMinEquivalency.TabIndex = 0;
            this.numTunerTableMinEquivalency.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Minimum equivalency in table names";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(176, 113);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "More settings later...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "x 100ms wait before search";
            // 
            // numKeypressWait
            // 
            this.numKeypressWait.Location = new System.Drawing.Point(206, 40);
            this.numKeypressWait.Name = "numKeypressWait";
            this.numKeypressWait.Size = new System.Drawing.Size(44, 20);
            this.numKeypressWait.TabIndex = 5;
            this.numKeypressWait.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // frmMoreSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 148);
            this.Controls.Add(this.numKeypressWait);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numTunerTableMinEquivalency);
            this.Name = "frmMoreSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.frmTunerSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numTunerTableMinEquivalency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeypressWait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numTunerTableMinEquivalency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numKeypressWait;
    }
}