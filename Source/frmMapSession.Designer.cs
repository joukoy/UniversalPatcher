
namespace UniversalPatcher
{
    partial class frmMapSession
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMapSession));
            this.txtRefBin = new System.Windows.Forms.TextBox();
            this.btnRefBin = new System.Windows.Forms.Button();
            this.btnRefDef = new System.Windows.Forms.Button();
            this.txtRefTableList = new System.Windows.Forms.TextBox();
            this.btnMapPin = new System.Windows.Forms.Button();
            this.txtMapPin = new System.Windows.Forms.TextBox();
            this.txtSessionName = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkUseAutoloadTables = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtRefBin
            // 
            this.txtRefBin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRefBin.Location = new System.Drawing.Point(114, 80);
            this.txtRefBin.Name = "txtRefBin";
            this.txtRefBin.Size = new System.Drawing.Size(367, 20);
            this.txtRefBin.TabIndex = 0;
            // 
            // btnRefBin
            // 
            this.btnRefBin.Location = new System.Drawing.Point(12, 80);
            this.btnRefBin.Name = "btnRefBin";
            this.btnRefBin.Size = new System.Drawing.Size(96, 21);
            this.btnRefBin.TabIndex = 1;
            this.btnRefBin.Text = "Reference bin";
            this.btnRefBin.UseVisualStyleBackColor = true;
            this.btnRefBin.Click += new System.EventHandler(this.btnRefBin_Click);
            // 
            // btnRefDef
            // 
            this.btnRefDef.Location = new System.Drawing.Point(12, 107);
            this.btnRefDef.Name = "btnRefDef";
            this.btnRefDef.Size = new System.Drawing.Size(96, 21);
            this.btnRefDef.TabIndex = 3;
            this.btnRefDef.Text = "Ref tablelist";
            this.btnRefDef.UseVisualStyleBackColor = true;
            this.btnRefDef.Click += new System.EventHandler(this.btnRefDef_Click);
            // 
            // txtRefTableList
            // 
            this.txtRefTableList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRefTableList.Location = new System.Drawing.Point(114, 106);
            this.txtRefTableList.Name = "txtRefTableList";
            this.txtRefTableList.Size = new System.Drawing.Size(367, 20);
            this.txtRefTableList.TabIndex = 2;
            // 
            // btnMapPin
            // 
            this.btnMapPin.Location = new System.Drawing.Point(12, 54);
            this.btnMapPin.Name = "btnMapPin";
            this.btnMapPin.Size = new System.Drawing.Size(96, 21);
            this.btnMapPin.TabIndex = 5;
            this.btnMapPin.Text = "Map bin";
            this.btnMapPin.UseVisualStyleBackColor = true;
            this.btnMapPin.Click += new System.EventHandler(this.btnMapPin_Click);
            // 
            // txtMapPin
            // 
            this.txtMapPin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMapPin.Location = new System.Drawing.Point(114, 54);
            this.txtMapPin.Name = "txtMapPin";
            this.txtMapPin.Size = new System.Drawing.Size(367, 20);
            this.txtMapPin.TabIndex = 4;
            // 
            // txtSessionName
            // 
            this.txtSessionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSessionName.Location = new System.Drawing.Point(114, 27);
            this.txtSessionName.Name = "txtSessionName";
            this.txtSessionName.Size = new System.Drawing.Size(367, 20);
            this.txtSessionName.TabIndex = 6;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(493, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(381, 144);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "Create Session";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkUseAutoloadTables
            // 
            this.chkUseAutoloadTables.AutoSize = true;
            this.chkUseAutoloadTables.Location = new System.Drawing.Point(114, 132);
            this.chkUseAutoloadTables.Name = "chkUseAutoloadTables";
            this.chkUseAutoloadTables.Size = new System.Drawing.Size(140, 17);
            this.chkUseAutoloadTables.TabIndex = 10;
            this.chkUseAutoloadTables.Text = "Use Autoloaded tablelist";
            this.chkUseAutoloadTables.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Session name:";
            // 
            // frmMapSession
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 184);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUseAutoloadTables);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtSessionName);
            this.Controls.Add(this.btnMapPin);
            this.Controls.Add(this.txtMapPin);
            this.Controls.Add(this.btnRefDef);
            this.Controls.Add(this.txtRefTableList);
            this.Controls.Add(this.btnRefBin);
            this.Controls.Add(this.txtRefBin);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMapSession";
            this.Text = "Create MAP session";
            this.Load += new System.EventHandler(this.frmMapSession_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRefBin;
        private System.Windows.Forms.Button btnRefDef;
        private System.Windows.Forms.Button btnMapPin;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.TextBox txtRefBin;
        public System.Windows.Forms.TextBox txtRefTableList;
        public System.Windows.Forms.TextBox txtMapPin;
        public System.Windows.Forms.TextBox txtSessionName;
        public System.Windows.Forms.CheckBox chkUseAutoloadTables;
        private System.Windows.Forms.Label label1;
    }
}