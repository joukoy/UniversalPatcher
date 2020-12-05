namespace UniversalPatcher
{
    partial class frmTuner
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnEditTable = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnSearchTableSeek = new System.Windows.Forms.Button();
            this.txtSearchTableSeek = new System.Windows.Forms.TextBox();
            this.comboTableCategory = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTableSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importXDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTinyTunerDBV6OnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(2, 56);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(861, 334);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnEditTable
            // 
            this.btnEditTable.Location = new System.Drawing.Point(12, 27);
            this.btnEditTable.Name = "btnEditTable";
            this.btnEditTable.Size = new System.Drawing.Size(120, 23);
            this.btnEditTable.TabIndex = 6;
            this.btnEditTable.Text = "Edit Table";
            this.btnEditTable.UseVisualStyleBackColor = true;
            this.btnEditTable.Click += new System.EventHandler(this.btnEditTable_Click);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(2, 389);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(861, 104);
            this.txtResult.TabIndex = 7;
            this.txtResult.Text = "";
            // 
            // btnSearchTableSeek
            // 
            this.btnSearchTableSeek.Location = new System.Drawing.Point(443, 27);
            this.btnSearchTableSeek.Name = "btnSearchTableSeek";
            this.btnSearchTableSeek.Size = new System.Drawing.Size(65, 22);
            this.btnSearchTableSeek.TabIndex = 15;
            this.btnSearchTableSeek.Text = "Search";
            this.btnSearchTableSeek.UseVisualStyleBackColor = true;
            this.btnSearchTableSeek.Click += new System.EventHandler(this.btnSearchTableSeek_Click);
            // 
            // txtSearchTableSeek
            // 
            this.txtSearchTableSeek.Location = new System.Drawing.Point(342, 29);
            this.txtSearchTableSeek.Name = "txtSearchTableSeek";
            this.txtSearchTableSeek.Size = new System.Drawing.Size(96, 20);
            this.txtSearchTableSeek.TabIndex = 14;
            // 
            // comboTableCategory
            // 
            this.comboTableCategory.FormattingEnabled = true;
            this.comboTableCategory.Location = new System.Drawing.Point(191, 29);
            this.comboTableCategory.Name = "comboTableCategory";
            this.comboTableCategory.Size = new System.Drawing.Size(142, 21);
            this.comboTableCategory.TabIndex = 13;
            this.comboTableCategory.SelectedIndexChanged += new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(135, 33);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Category:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.importToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(862, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadXMLToolStripMenuItem,
            this.saveXMLToolStripMenuItem,
            this.saveBINToolStripMenuItem,
            this.clearTableToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadXMLToolStripMenuItem
            // 
            this.loadXMLToolStripMenuItem.Name = "loadXMLToolStripMenuItem";
            this.loadXMLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadXMLToolStripMenuItem.Text = "Load XML";
            this.loadXMLToolStripMenuItem.Click += new System.EventHandler(this.loadXMLToolStripMenuItem_Click);
            // 
            // saveXMLToolStripMenuItem
            // 
            this.saveXMLToolStripMenuItem.Name = "saveXMLToolStripMenuItem";
            this.saveXMLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveXMLToolStripMenuItem.Text = "Save XML";
            this.saveXMLToolStripMenuItem.Click += new System.EventHandler(this.saveXMLToolStripMenuItem_Click);
            // 
            // saveBINToolStripMenuItem
            // 
            this.saveBINToolStripMenuItem.Name = "saveBINToolStripMenuItem";
            this.saveBINToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveBINToolStripMenuItem.Text = "Save BIN";
            this.saveBINToolStripMenuItem.Click += new System.EventHandler(this.saveBINToolStripMenuItem_Click);
            // 
            // clearTableToolStripMenuItem
            // 
            this.clearTableToolStripMenuItem.Name = "clearTableToolStripMenuItem";
            this.clearTableToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clearTableToolStripMenuItem.Text = "Clear Table";
            this.clearTableToolStripMenuItem.Click += new System.EventHandler(this.clearTableToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importDTCToolStripMenuItem,
            this.importTableSeekToolStripMenuItem,
            this.importXDFToolStripMenuItem,
            this.importTinyTunerDBV6OnlyToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.importToolStripMenuItem.Text = "Import";
            // 
            // importDTCToolStripMenuItem
            // 
            this.importDTCToolStripMenuItem.Name = "importDTCToolStripMenuItem";
            this.importDTCToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importDTCToolStripMenuItem.Text = "Import DTC";
            this.importDTCToolStripMenuItem.Click += new System.EventHandler(this.importDTCToolStripMenuItem_Click);
            // 
            // importTableSeekToolStripMenuItem
            // 
            this.importTableSeekToolStripMenuItem.Name = "importTableSeekToolStripMenuItem";
            this.importTableSeekToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importTableSeekToolStripMenuItem.Text = "Import TableSeek";
            this.importTableSeekToolStripMenuItem.Click += new System.EventHandler(this.importTableSeekToolStripMenuItem_Click);
            // 
            // importXDFToolStripMenuItem
            // 
            this.importXDFToolStripMenuItem.Name = "importXDFToolStripMenuItem";
            this.importXDFToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importXDFToolStripMenuItem.Text = "Import XDF";
            this.importXDFToolStripMenuItem.Click += new System.EventHandler(this.importXDFToolStripMenuItem_Click);
            // 
            // importTinyTunerDBV6OnlyToolStripMenuItem
            // 
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Name = "importTinyTunerDBV6OnlyToolStripMenuItem";
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Text = "Import TinyTuner DB (V6 only)";
            this.importTinyTunerDBV6OnlyToolStripMenuItem.Click += new System.EventHandler(this.importTinyTunerDBV6OnlyToolStripMenuItem_Click);
            // 
            // frmTuner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 492);
            this.Controls.Add(this.btnSearchTableSeek);
            this.Controls.Add(this.txtSearchTableSeek);
            this.Controls.Add(this.comboTableCategory);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnEditTable);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmTuner";
            this.Text = "Tuner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTuner_FormClosing);
            this.Load += new System.EventHandler(this.frmTuner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnEditTable;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnSearchTableSeek;
        private System.Windows.Forms.TextBox txtSearchTableSeek;
        private System.Windows.Forms.ComboBox comboTableCategory;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBINToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDTCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTableSeekToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importXDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTinyTunerDBV6OnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearTableToolStripMenuItem;
    }
}