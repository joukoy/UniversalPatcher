namespace UniversalPatcher
{
    partial class frmTunerExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTunerExplorer));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDimensions = new System.Windows.Forms.TabPage();
            this.tabValueType = new System.Windows.Forms.TabPage();
            this.tabCategory = new System.Windows.Forms.TabPage();
            this.tabSegments = new System.Windows.Forms.TabPage();
            this.tabPatches = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numIconSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCategorySubfolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerFilter = new System.Windows.Forms.Timer(this.components);
            this.btnCollapse = new System.Windows.Forms.Button();
            this.btnExpand = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.labelTableName = new System.Windows.Forms.Label();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.bINFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMultipleBINFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInNewWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIconSize)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1048, 371);
            this.splitContainer1.SplitterDistance = 449;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDimensions);
            this.tabControl1.Controls.Add(this.tabValueType);
            this.tabControl1.Controls.Add(this.tabCategory);
            this.tabControl1.Controls.Add(this.tabSegments);
            this.tabControl1.Controls.Add(this.tabPatches);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList3;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(449, 371);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDimensions
            // 
            this.tabDimensions.ImageKey = "Dimensions.ico";
            this.tabDimensions.Location = new System.Drawing.Point(4, 23);
            this.tabDimensions.Name = "tabDimensions";
            this.tabDimensions.Padding = new System.Windows.Forms.Padding(3);
            this.tabDimensions.Size = new System.Drawing.Size(441, 344);
            this.tabDimensions.TabIndex = 0;
            this.tabDimensions.UseVisualStyleBackColor = true;
            // 
            // tabValueType
            // 
            this.tabValueType.ImageKey = "valuetype.ico";
            this.tabValueType.Location = new System.Drawing.Point(4, 31);
            this.tabValueType.Name = "tabValueType";
            this.tabValueType.Padding = new System.Windows.Forms.Padding(3);
            this.tabValueType.Size = new System.Drawing.Size(441, 336);
            this.tabValueType.TabIndex = 1;
            this.tabValueType.UseVisualStyleBackColor = true;
            // 
            // tabCategory
            // 
            this.tabCategory.ImageKey = "category.ico";
            this.tabCategory.Location = new System.Drawing.Point(4, 23);
            this.tabCategory.Name = "tabCategory";
            this.tabCategory.Size = new System.Drawing.Size(441, 344);
            this.tabCategory.TabIndex = 2;
            this.tabCategory.UseVisualStyleBackColor = true;
            // 
            // tabSegments
            // 
            this.tabSegments.ImageKey = "segments.ico";
            this.tabSegments.Location = new System.Drawing.Point(4, 23);
            this.tabSegments.Name = "tabSegments";
            this.tabSegments.Size = new System.Drawing.Size(441, 344);
            this.tabSegments.TabIndex = 3;
            this.tabSegments.UseVisualStyleBackColor = true;
            // 
            // tabPatches
            // 
            this.tabPatches.ImageKey = "patch.ico";
            this.tabPatches.Location = new System.Drawing.Point(4, 23);
            this.tabPatches.Name = "tabPatches";
            this.tabPatches.Size = new System.Drawing.Size(441, 344);
            this.tabPatches.TabIndex = 4;
            this.tabPatches.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.bINFileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1048, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.openMultipleBINFilesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 27);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtResult);
            this.splitContainer2.Size = new System.Drawing.Size(1048, 426);
            this.splitContainer2.SplitterDistance = 371;
            this.splitContainer2.TabIndex = 1;
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(1048, 51);
            this.txtResult.TabIndex = 0;
            this.txtResult.Text = "";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(349, 2);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(100, 20);
            this.txtFilter.TabIndex = 14;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(311, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Filter:";
            // 
            // numIconSize
            // 
            this.numIconSize.Location = new System.Drawing.Point(265, 4);
            this.numIconSize.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numIconSize.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numIconSize.Name = "numIconSize";
            this.numIconSize.Size = new System.Drawing.Size(40, 20);
            this.numIconSize.TabIndex = 12;
            this.numIconSize.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numIconSize.ValueChanged += new System.EventHandler(this.numIconSize_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(207, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Icon size:";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "explorer.ico");
            this.imageList1.Images.SetKeyName(1, "1d.ico");
            this.imageList1.Images.SetKeyName(2, "2d.ico");
            this.imageList1.Images.SetKeyName(3, "3d.ico");
            this.imageList1.Images.SetKeyName(4, "enum.ico");
            this.imageList1.Images.SetKeyName(5, "enum1d.ico");
            this.imageList1.Images.SetKeyName(6, "enum2d.ico");
            this.imageList1.Images.SetKeyName(7, "enum3d.ico");
            this.imageList1.Images.SetKeyName(8, "flag.ico");
            this.imageList1.Images.SetKeyName(9, "flag1d.ico");
            this.imageList1.Images.SetKeyName(10, "flag2d.ico");
            this.imageList1.Images.SetKeyName(11, "flag3d.ico");
            this.imageList1.Images.SetKeyName(12, "mask.ico");
            this.imageList1.Images.SetKeyName(13, "mask1d.ico");
            this.imageList1.Images.SetKeyName(14, "mask2d.ico");
            this.imageList1.Images.SetKeyName(15, "mask3d.ico");
            this.imageList1.Images.SetKeyName(16, "num.ico");
            this.imageList1.Images.SetKeyName(17, "boolean.ico");
            this.imageList1.Images.SetKeyName(18, "bitmask.ico");
            this.imageList1.Images.SetKeyName(19, "number.ico");
            this.imageList1.Images.SetKeyName(20, "Dimensions.ico");
            this.imageList1.Images.SetKeyName(21, "valuetype.ico");
            this.imageList1.Images.SetKeyName(22, "segments.ico");
            this.imageList1.Images.SetKeyName(23, "category.ico");
            this.imageList1.Images.SetKeyName(24, "eeprom.ico");
            this.imageList1.Images.SetKeyName(25, "engine.ico");
            this.imageList1.Images.SetKeyName(26, "fuel.ico");
            this.imageList1.Images.SetKeyName(27, "os.ico");
            this.imageList1.Images.SetKeyName(28, "speedo.ico");
            this.imageList1.Images.SetKeyName(29, "system.ico");
            this.imageList1.Images.SetKeyName(30, "trans.ico");
            this.imageList1.Images.SetKeyName(31, "enginediag.ico");
            this.imageList1.Images.SetKeyName(32, "transdiag.ico");
            this.imageList1.Images.SetKeyName(33, "patch.ico");
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontToolStripMenuItem,
            this.showCategorySubfolderToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.fontToolStripMenuItem.Text = "Font...";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // showCategorySubfolderToolStripMenuItem
            // 
            this.showCategorySubfolderToolStripMenuItem.Name = "showCategorySubfolderToolStripMenuItem";
            this.showCategorySubfolderToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.showCategorySubfolderToolStripMenuItem.Text = "Show category subfolder";
            this.showCategorySubfolderToolStripMenuItem.Click += new System.EventHandler(this.showCategorySubfolderToolStripMenuItem_Click);
            // 
            // timerFilter
            // 
            this.timerFilter.Tick += new System.EventHandler(this.timerFilter_Tick);
            // 
            // btnCollapse
            // 
            this.btnCollapse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapse.ImageKey = "collapse.ico";
            this.btnCollapse.ImageList = this.imageList2;
            this.btnCollapse.Location = new System.Drawing.Point(483, 1);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Size = new System.Drawing.Size(24, 24);
            this.btnCollapse.TabIndex = 16;
            this.btnCollapse.UseVisualStyleBackColor = true;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExpand.ImageKey = "expand.ico";
            this.btnExpand.ImageList = this.imageList2;
            this.btnExpand.Location = new System.Drawing.Point(453, 1);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(24, 24);
            this.btnExpand.TabIndex = 15;
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "collapse.ico");
            this.imageList2.Images.SetKeyName(1, "expand.ico");
            this.imageList2.Images.SetKeyName(2, "category.ico");
            this.imageList2.Images.SetKeyName(3, "Dimensions.ico");
            this.imageList2.Images.SetKeyName(4, "segments.ico");
            this.imageList2.Images.SetKeyName(5, "valuetype.ico");
            this.imageList2.Images.SetKeyName(6, "patch.ico");
            // 
            // labelTableName
            // 
            this.labelTableName.AutoSize = true;
            this.labelTableName.BackColor = System.Drawing.SystemColors.Window;
            this.labelTableName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTableName.Location = new System.Drawing.Point(574, 3);
            this.labelTableName.Name = "labelTableName";
            this.labelTableName.Size = new System.Drawing.Size(46, 18);
            this.labelTableName.TabIndex = 17;
            this.labelTableName.Text = "Table";
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "category.ico");
            this.imageList3.Images.SetKeyName(1, "Dimensions.ico");
            this.imageList3.Images.SetKeyName(2, "patch.ico");
            this.imageList3.Images.SetKeyName(3, "segments.ico");
            this.imageList3.Images.SetKeyName(4, "valuetype.ico");
            this.imageList3.Images.SetKeyName(5, "category.ico");
            // 
            // bINFileToolStripMenuItem
            // 
            this.bINFileToolStripMenuItem.Name = "bINFileToolStripMenuItem";
            this.bINFileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.bINFileToolStripMenuItem.Text = "BIN file";
            // 
            // openMultipleBINFilesToolStripMenuItem
            // 
            this.openMultipleBINFilesToolStripMenuItem.Name = "openMultipleBINFilesToolStripMenuItem";
            this.openMultipleBINFilesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.openMultipleBINFilesToolStripMenuItem.Text = "Open multiple BIN files";
            this.openMultipleBINFilesToolStripMenuItem.Click += new System.EventHandler(this.openMultipleBINFilesToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(521, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Table:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInNewWindowToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(187, 48);
            // 
            // openInNewWindowToolStripMenuItem
            // 
            this.openInNewWindowToolStripMenuItem.Name = "openInNewWindowToolStripMenuItem";
            this.openInNewWindowToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.openInNewWindowToolStripMenuItem.Text = "Open in new window";
            this.openInNewWindowToolStripMenuItem.Click += new System.EventHandler(this.openInNewWindowToolStripMenuItem_Click);
            // 
            // frmTunerExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelTableName);
            this.Controls.Add(this.btnCollapse);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numIconSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmTunerExplorer";
            this.Text = "Tuner";
            this.Load += new System.EventHandler(this.frmTunerExplorer_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numIconSize)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabDimensions;
        private System.Windows.Forms.TabPage tabValueType;
        private System.Windows.Forms.TabPage tabCategory;
        private System.Windows.Forms.TabPage tabSegments;
        private System.Windows.Forms.TabPage tabPatches;
        private System.Windows.Forms.Button btnCollapse;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numIconSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCategorySubfolderToolStripMenuItem;
        private System.Windows.Forms.Timer timerFilter;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Label labelTableName;
        private System.Windows.Forms.ImageList imageList3;
        private System.Windows.Forms.ToolStripMenuItem bINFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMultipleBINFilesToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openInNewWindowToolStripMenuItem;
    }
}