namespace UniversalPatcher
{
    partial class frmHexDiff
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHexDiff));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelFileNames = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.btnSaveCsv = new System.Windows.Forms.Button();
            this.btnSaveTableList = new System.Windows.Forms.Button();
            this.btnShowInTuner = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(681, 423);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // labelFileNames
            // 
            this.labelFileNames.AutoSize = true;
            this.labelFileNames.BackColor = System.Drawing.Color.White;
            this.labelFileNames.Location = new System.Drawing.Point(384, 5);
            this.labelFileNames.Name = "labelFileNames";
            this.labelFileNames.Size = new System.Drawing.Size(11, 13);
            this.labelFileNames.TabIndex = 1;
            this.labelFileNames.Text = "*";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(290, 3);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(88, 20);
            this.txtFilter.TabIndex = 2;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // btnSaveCsv
            // 
            this.btnSaveCsv.Location = new System.Drawing.Point(12, 0);
            this.btnSaveCsv.Name = "btnSaveCsv";
            this.btnSaveCsv.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCsv.TabIndex = 3;
            this.btnSaveCsv.Text = "Save CSV";
            this.btnSaveCsv.UseVisualStyleBackColor = true;
            this.btnSaveCsv.Click += new System.EventHandler(this.btnSaveCsv_Click);
            // 
            // btnSaveTableList
            // 
            this.btnSaveTableList.Location = new System.Drawing.Point(90, 1);
            this.btnSaveTableList.Name = "btnSaveTableList";
            this.btnSaveTableList.Size = new System.Drawing.Size(94, 23);
            this.btnSaveTableList.TabIndex = 4;
            this.btnSaveTableList.Text = "Save Tablelist";
            this.btnSaveTableList.UseVisualStyleBackColor = true;
            this.btnSaveTableList.Click += new System.EventHandler(this.btnSaveTableList_Click);
            // 
            // btnShowInTuner
            // 
            this.btnShowInTuner.Location = new System.Drawing.Point(190, 0);
            this.btnShowInTuner.Name = "btnShowInTuner";
            this.btnShowInTuner.Size = new System.Drawing.Size(94, 23);
            this.btnShowInTuner.TabIndex = 5;
            this.btnShowInTuner.Text = "Show in Tuner";
            this.btnShowInTuner.UseVisualStyleBackColor = true;
            this.btnShowInTuner.Click += new System.EventHandler(this.btnShowInTuner_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 29);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(804, 423);
            this.splitContainer1.SplitterDistance = 119;
            this.splitContainer1.TabIndex = 6;
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
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 70);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.expandAllToolStripMenuItem.Text = "Expand all";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.collapseToolStripMenuItem.Text = "Collapse";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
            // 
            // frmHexDiff
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnShowInTuner);
            this.Controls.Add(this.btnSaveTableList);
            this.Controls.Add(this.btnSaveCsv);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.labelFileNames);
            this.Name = "frmHexDiff";
            this.Text = "File differences (HEX)";
            this.Load += new System.EventHandler(this.frmHexDiff_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelFileNames;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Button btnSaveCsv;
        private System.Windows.Forms.Button btnSaveTableList;
        private System.Windows.Forms.Button btnShowInTuner;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
    }
}