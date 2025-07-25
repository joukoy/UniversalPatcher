
namespace UniversalPatcher
{
    partial class frmSelectDBC
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
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDBCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridMsg = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridBitView = new System.Windows.Forms.DataGridView();
            this.dataGridSignal = new System.Windows.Forms.DataGridView();
            this.btnImportSignals = new System.Windows.Forms.Button();
            this.btnImportAllMsgs = new System.Windows.Forms.Button();
            this.labelByteNumber = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelFileName = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMsg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBitView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSignal)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(854, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadDBCToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadDBCToolStripMenuItem
            // 
            this.loadDBCToolStripMenuItem.Name = "loadDBCToolStripMenuItem";
            this.loadDBCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadDBCToolStripMenuItem.Text = "Load DBC";
            this.loadDBCToolStripMenuItem.Click += new System.EventHandler(this.loadDBCToolStripMenuItem_Click);
            // 
            // dataGridMsg
            // 
            this.dataGridMsg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridMsg.Location = new System.Drawing.Point(0, 0);
            this.dataGridMsg.Name = "dataGridMsg";
            this.dataGridMsg.Size = new System.Drawing.Size(293, 453);
            this.dataGridMsg.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridMsg);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.labelByteNumber);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridBitView);
            this.splitContainer1.Panel2.Controls.Add(this.dataGridSignal);
            this.splitContainer1.Size = new System.Drawing.Size(854, 453);
            this.splitContainer1.SplitterDistance = 293;
            this.splitContainer1.TabIndex = 2;
            // 
            // dataGridBitView
            // 
            this.dataGridBitView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridBitView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridBitView.Location = new System.Drawing.Point(25, 196);
            this.dataGridBitView.Name = "dataGridBitView";
            this.dataGridBitView.Size = new System.Drawing.Size(531, 256);
            this.dataGridBitView.TabIndex = 1;
            // 
            // dataGridSignal
            // 
            this.dataGridSignal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridSignal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSignal.Location = new System.Drawing.Point(0, 0);
            this.dataGridSignal.Name = "dataGridSignal";
            this.dataGridSignal.Size = new System.Drawing.Size(557, 177);
            this.dataGridSignal.TabIndex = 0;
            // 
            // btnImportSignals
            // 
            this.btnImportSignals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportSignals.Location = new System.Drawing.Point(637, 492);
            this.btnImportSignals.Name = "btnImportSignals";
            this.btnImportSignals.Size = new System.Drawing.Size(205, 25);
            this.btnImportSignals.TabIndex = 3;
            this.btnImportSignals.Text = "Import selected signals";
            this.btnImportSignals.UseVisualStyleBackColor = true;
            this.btnImportSignals.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnImportAllMsgs
            // 
            this.btnImportAllMsgs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImportAllMsgs.Location = new System.Drawing.Point(12, 492);
            this.btnImportAllMsgs.Name = "btnImportAllMsgs";
            this.btnImportAllMsgs.Size = new System.Drawing.Size(205, 25);
            this.btnImportAllMsgs.TabIndex = 4;
            this.btnImportAllMsgs.Text = "Import all selected messages";
            this.btnImportAllMsgs.UseVisualStyleBackColor = true;
            this.btnImportAllMsgs.Click += new System.EventHandler(this.btnImportAllMsgs_Click);
            // 
            // labelByteNumber
            // 
            this.labelByteNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelByteNumber.AutoSize = true;
            this.labelByteNumber.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelByteNumber.Location = new System.Drawing.Point(3, 217);
            this.labelByteNumber.Name = "labelByteNumber";
            this.labelByteNumber.Size = new System.Drawing.Size(73, 13);
            this.labelByteNumber.TabIndex = 2;
            this.labelByteNumber.Text = "byte number";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(97, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bit number";
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelFileName.Location = new System.Drawing.Point(133, 5);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(37, 15);
            this.labelFileName.TabIndex = 5;
            this.labelFileName.Text = "label2";
            // 
            // frmSelectDBC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 529);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.btnImportAllMsgs);
            this.Controls.Add(this.btnImportSignals);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmSelectDBC";
            this.Text = "Select signals from DBC";
            this.Load += new System.EventHandler(this.frmSelectDBC_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMsg)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBitView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSignal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDBCToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridMsg;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridSignal;
        private System.Windows.Forms.Button btnImportSignals;
        private System.Windows.Forms.Button btnImportAllMsgs;
        private System.Windows.Forms.DataGridView dataGridBitView;
        private System.Windows.Forms.Label labelByteNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelFileName;
    }
}