namespace UniversalPatcher
{
    partial class FrmMain
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
            this.btnSegments = new System.Windows.Forms.Button();
            this.btnPatcher = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSegments
            // 
            this.btnSegments.Location = new System.Drawing.Point(11, 9);
            this.btnSegments.Name = "btnSegments";
            this.btnSegments.Size = new System.Drawing.Size(97, 44);
            this.btnSegments.TabIndex = 0;
            this.btnSegments.Text = "Setup Segements";
            this.btnSegments.UseVisualStyleBackColor = true;
            this.btnSegments.Click += new System.EventHandler(this.btnSegments_Click);
            // 
            // btnPatcher
            // 
            this.btnPatcher.Location = new System.Drawing.Point(11, 59);
            this.btnPatcher.Name = "btnPatcher";
            this.btnPatcher.Size = new System.Drawing.Size(96, 40);
            this.btnPatcher.TabIndex = 1;
            this.btnPatcher.Text = "Patcher";
            this.btnPatcher.UseVisualStyleBackColor = true;
            this.btnPatcher.Click += new System.EventHandler(this.btnPatcher_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::UniversalPatcher.Properties.Resources.UniversalPatcher;
            this.pictureBox1.Location = new System.Drawing.Point(119, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(321, 217);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 229);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnPatcher);
            this.Controls.Add(this.btnSegments);
            this.Name = "FrmMain";
            this.Text = "Universal Patcher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSegments;
        private System.Windows.Forms.Button btnPatcher;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}