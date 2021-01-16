using System;
using System.Windows.Forms;

namespace UniversalPatcher
{
    partial class frmTableEditor
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtMath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.labelUnits = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCsvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoResizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapXyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showRawHEXValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableTooltipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGraphicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkSwapXY = new System.Windows.Forms.CheckBox();
            this.numColumn = new System.Windows.Forms.NumericUpDown();
            this.labelColumn = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColumn)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(2, 72);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(796, 376);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellEndEdit);
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // txtMath
            // 
            this.txtMath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMath.Location = new System.Drawing.Point(2, 25);
            this.txtMath.Name = "txtMath";
            this.txtMath.Size = new System.Drawing.Size(736, 20);
            this.txtMath.TabIndex = 1;
            this.txtMath.Text = "X*1";
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(744, 25);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(54, 21);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // labelUnits
            // 
            this.labelUnits.AutoSize = true;
            this.labelUnits.Location = new System.Drawing.Point(6, 50);
            this.labelUnits.Name = "labelUnits";
            this.labelUnits.Size = new System.Drawing.Size(10, 13);
            this.labelUnits.TabIndex = 5;
            this.labelUnits.Text = "-";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.exportCsvToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(129, 92);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // exportCsvToolStripMenuItem
            // 
            this.exportCsvToolStripMenuItem.Name = "exportCsvToolStripMenuItem";
            this.exportCsvToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.exportCsvToolStripMenuItem.Text = "Export csv";
            this.exportCsvToolStripMenuItem.Click += new System.EventHandler(this.exportCsvToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.graphToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.exportCSVToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exportCSVToolStripMenuItem1
            // 
            this.exportCSVToolStripMenuItem1.Name = "exportCSVToolStripMenuItem1";
            this.exportCSVToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.exportCSVToolStripMenuItem1.Text = "Export CSV";
            this.exportCSVToolStripMenuItem1.Click += new System.EventHandler(this.exportCSVToolStripMenuItem1_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoResizeToolStripMenuItem,
            this.swapXyToolStripMenuItem,
            this.showRawHEXValuesToolStripMenuItem,
            this.disableTooltipsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // autoResizeToolStripMenuItem
            // 
            this.autoResizeToolStripMenuItem.Name = "autoResizeToolStripMenuItem";
            this.autoResizeToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.autoResizeToolStripMenuItem.Text = "Auto Resize";
            this.autoResizeToolStripMenuItem.Click += new System.EventHandler(this.autoResizeToolStripMenuItem_Click);
            // 
            // swapXyToolStripMenuItem
            // 
            this.swapXyToolStripMenuItem.Name = "swapXyToolStripMenuItem";
            this.swapXyToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.swapXyToolStripMenuItem.Text = "Swap x/y";
            this.swapXyToolStripMenuItem.Click += new System.EventHandler(this.swapXyToolStripMenuItem_Click);
            // 
            // showRawHEXValuesToolStripMenuItem
            // 
            this.showRawHEXValuesToolStripMenuItem.Name = "showRawHEXValuesToolStripMenuItem";
            this.showRawHEXValuesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.showRawHEXValuesToolStripMenuItem.Text = "Show Raw HEX values";
            this.showRawHEXValuesToolStripMenuItem.Click += new System.EventHandler(this.showRawHEXValuesToolStripMenuItem_Click);
            // 
            // disableTooltipsToolStripMenuItem
            // 
            this.disableTooltipsToolStripMenuItem.Name = "disableTooltipsToolStripMenuItem";
            this.disableTooltipsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.disableTooltipsToolStripMenuItem.Text = "Disable Tooltips";
            this.disableTooltipsToolStripMenuItem.Click += new System.EventHandler(this.disableTooltipsToolStripMenuItem_Click);
            // 
            // graphToolStripMenuItem
            // 
            this.graphToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGraphicToolStripMenuItem});
            this.graphToolStripMenuItem.Name = "graphToolStripMenuItem";
            this.graphToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.graphToolStripMenuItem.Text = "Graph";
            // 
            // showGraphicToolStripMenuItem
            // 
            this.showGraphicToolStripMenuItem.Name = "showGraphicToolStripMenuItem";
            this.showGraphicToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.showGraphicToolStripMenuItem.Text = "Show graphic";
            this.showGraphicToolStripMenuItem.Click += new System.EventHandler(this.showGraphicToolStripMenuItem_Click);
            // 
            // chkSwapXY
            // 
            this.chkSwapXY.AutoSize = true;
            this.chkSwapXY.Location = new System.Drawing.Point(226, 6);
            this.chkSwapXY.Name = "chkSwapXY";
            this.chkSwapXY.Size = new System.Drawing.Size(75, 17);
            this.chkSwapXY.TabIndex = 7;
            this.chkSwapXY.Text = "Swap X/Y";
            this.chkSwapXY.UseVisualStyleBackColor = true;
            this.chkSwapXY.CheckedChanged += new System.EventHandler(this.chkSwapXY_CheckedChanged);
            // 
            // numColumn
            // 
            this.numColumn.Enabled = false;
            this.numColumn.Location = new System.Drawing.Point(358, 3);
            this.numColumn.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numColumn.Name = "numColumn";
            this.numColumn.Size = new System.Drawing.Size(34, 20);
            this.numColumn.TabIndex = 8;
            this.numColumn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numColumn.Visible = false;
            this.numColumn.ValueChanged += new System.EventHandler(this.numColumn_ValueChanged);
            // 
            // labelColumn
            // 
            this.labelColumn.AutoSize = true;
            this.labelColumn.Location = new System.Drawing.Point(307, 7);
            this.labelColumn.Name = "labelColumn";
            this.labelColumn.Size = new System.Drawing.Size(45, 13);
            this.labelColumn.TabIndex = 9;
            this.labelColumn.Text = "Column:";
            this.labelColumn.Visible = false;
            // 
            // frmTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelColumn);
            this.Controls.Add(this.numColumn);
            this.Controls.Add(this.chkSwapXY);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.labelUnits);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtMath);
            this.Controls.Add(this.dataGridView1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmTableEditor";
            this.Text = "Table Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTableEditor_FormClosing);
            this.Load += new System.EventHandler(this.frmTableEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColumn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtMath;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label labelUnits;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCsvToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCSVToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoResizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem swapXyToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSwapXY;
        private System.Windows.Forms.ToolStripMenuItem showRawHEXValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableTooltipsToolStripMenuItem;
        private ToolStripMenuItem graphToolStripMenuItem;
        private ToolStripMenuItem showGraphicToolStripMenuItem;
        private Label labelColumn;
        public NumericUpDown numColumn;
    }
}