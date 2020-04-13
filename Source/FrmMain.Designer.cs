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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.createPatchByCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getFileInfoFromFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixChecksumOfFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cVNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.segmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stockCVNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuView,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1344, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createPatchByCompareToolStripMenuItem,
            this.getFileInfoFromFolderToolStripMenuItem,
            this.fixChecksumOfFilesToolStripMenuItem,
            this.swapSegmentsToolStripMenuItem,
            this.applyPatchToolStripMenuItem,
            this.extractTableToolStripMenuItem,
            this.extractSegmentsToolStripMenuItem});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // createPatchByCompareToolStripMenuItem
            // 
            this.createPatchByCompareToolStripMenuItem.Name = "createPatchByCompareToolStripMenuItem";
            this.createPatchByCompareToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.createPatchByCompareToolStripMenuItem.Text = "Create patch by compare";
            this.createPatchByCompareToolStripMenuItem.Click += new System.EventHandler(this.createPatchByCompareToolStripMenuItem_Click);
            // 
            // getFileInfoFromFolderToolStripMenuItem
            // 
            this.getFileInfoFromFolderToolStripMenuItem.Name = "getFileInfoFromFolderToolStripMenuItem";
            this.getFileInfoFromFolderToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.getFileInfoFromFolderToolStripMenuItem.Text = "Get file info from folder...";
            this.getFileInfoFromFolderToolStripMenuItem.Click += new System.EventHandler(this.getFileInfoFromFolderToolStripMenuItem_Click);
            // 
            // fixChecksumOfFilesToolStripMenuItem
            // 
            this.fixChecksumOfFilesToolStripMenuItem.Name = "fixChecksumOfFilesToolStripMenuItem";
            this.fixChecksumOfFilesToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.fixChecksumOfFilesToolStripMenuItem.Text = "Fix checksum of files...";
            this.fixChecksumOfFilesToolStripMenuItem.Click += new System.EventHandler(this.fixChecksumOfFilesToolStripMenuItem_Click);
            // 
            // swapSegmentsToolStripMenuItem
            // 
            this.swapSegmentsToolStripMenuItem.Name = "swapSegmentsToolStripMenuItem";
            this.swapSegmentsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.swapSegmentsToolStripMenuItem.Text = "Swap segment(s)";
            this.swapSegmentsToolStripMenuItem.Click += new System.EventHandler(this.swapSegmentsToolStripMenuItem_Click);
            // 
            // applyPatchToolStripMenuItem
            // 
            this.applyPatchToolStripMenuItem.Name = "applyPatchToolStripMenuItem";
            this.applyPatchToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.applyPatchToolStripMenuItem.Text = "Apply patch...";
            this.applyPatchToolStripMenuItem.Click += new System.EventHandler(this.applyPatchToolStripMenuItem_Click);
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.cVNToolStripMenuItem,
            this.fileInfoToolStripMenuItem,
            this.patchEditorToolStripMenuItem});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(43, 20);
            this.menuView.Text = "view";
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // cVNToolStripMenuItem
            // 
            this.cVNToolStripMenuItem.Name = "cVNToolStripMenuItem";
            this.cVNToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.cVNToolStripMenuItem.Text = "CVN";
            this.cVNToolStripMenuItem.Click += new System.EventHandler(this.cVNToolStripMenuItem_Click);
            // 
            // fileInfoToolStripMenuItem
            // 
            this.fileInfoToolStripMenuItem.Name = "fileInfoToolStripMenuItem";
            this.fileInfoToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.fileInfoToolStripMenuItem.Text = "File info";
            this.fileInfoToolStripMenuItem.Click += new System.EventHandler(this.fileInfoToolStripMenuItem_Click);
            // 
            // patchEditorToolStripMenuItem
            // 
            this.patchEditorToolStripMenuItem.Name = "patchEditorToolStripMenuItem";
            this.patchEditorToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.patchEditorToolStripMenuItem.Text = "Patch editor";
            this.patchEditorToolStripMenuItem.Click += new System.EventHandler(this.patchEditorToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.segmentsToolStripMenuItem,
            this.autodetectToolStripMenuItem,
            this.stockCVNToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // segmentsToolStripMenuItem
            // 
            this.segmentsToolStripMenuItem.Name = "segmentsToolStripMenuItem";
            this.segmentsToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.segmentsToolStripMenuItem.Text = "Segments";
            this.segmentsToolStripMenuItem.Click += new System.EventHandler(this.segmentsToolStripMenuItem_Click);
            // 
            // autodetectToolStripMenuItem
            // 
            this.autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            this.autodetectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.autodetectToolStripMenuItem.Text = "Autodetect";
            this.autodetectToolStripMenuItem.Click += new System.EventHandler(this.autodetectToolStripMenuItem_Click);
            // 
            // stockCVNToolStripMenuItem
            // 
            this.stockCVNToolStripMenuItem.Name = "stockCVNToolStripMenuItem";
            this.stockCVNToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.stockCVNToolStripMenuItem.Text = "StockCVN";
            this.stockCVNToolStripMenuItem.Click += new System.EventHandler(this.stockCVNToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // extractTableToolStripMenuItem
            // 
            this.extractTableToolStripMenuItem.Name = "extractTableToolStripMenuItem";
            this.extractTableToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.extractTableToolStripMenuItem.Text = "Extract table...";
            this.extractTableToolStripMenuItem.Click += new System.EventHandler(this.extractTableToolStripMenuItem_Click);
            // 
            // extractSegmentsToolStripMenuItem
            // 
            this.extractSegmentsToolStripMenuItem.Name = "extractSegmentsToolStripMenuItem";
            this.extractSegmentsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.extractSegmentsToolStripMenuItem.Text = "Extract Segments...";
            this.extractSegmentsToolStripMenuItem.Click += new System.EventHandler(this.extractSegmentsToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UniversalPatcher.Properties.Resources.UniversalPatcher;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1344, 729);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMain";
            this.Text = "Universal Patcher";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createPatchByCompareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem segmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodetectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stockCVNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getFileInfoFromFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixChecksumOfFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cVNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patchEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem swapSegmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyPatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractSegmentsToolStripMenuItem;
    }
}