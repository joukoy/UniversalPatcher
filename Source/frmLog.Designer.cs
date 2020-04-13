namespace UniversalPatcher
{
    partial class frmLog
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
            this.components = new System.ComponentModel.Container();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.btnSaveFileInfo = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // rtxtLog
            // 
            this.rtxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtLog.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtLog.Location = new System.Drawing.Point(0, 27);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.Size = new System.Drawing.Size(1287, 201);
            this.rtxtLog.TabIndex = 0;
            this.rtxtLog.Text = "";
            // 
            // btnSaveFileInfo
            // 
            this.btnSaveFileInfo.Location = new System.Drawing.Point(0, 1);
            this.btnSaveFileInfo.Name = "btnSaveFileInfo";
            this.btnSaveFileInfo.Size = new System.Drawing.Size(78, 23);
            this.btnSaveFileInfo.TabIndex = 204;
            this.btnSaveFileInfo.Text = "Save log...";
            this.btnSaveFileInfo.UseVisualStyleBackColor = true;
            this.btnSaveFileInfo.Click += new System.EventHandler(this.btnSaveFileInfo_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 223);
            this.Controls.Add(this.btnSaveFileInfo);
            this.Controls.Add(this.rtxtLog);
            this.Name = "frmLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Log";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.Button btnSaveFileInfo;
        private System.Windows.Forms.Timer timer1;
    }
}