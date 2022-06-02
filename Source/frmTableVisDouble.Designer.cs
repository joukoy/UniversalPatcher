
namespace UniversalPatcher
{
    partial class frmTableVisDouble
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
            this.richTableData = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioSegmentTBNames = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.radioShowSegment = new System.Windows.Forms.RadioButton();
            this.numBytesPerRow = new System.Windows.Forms.NumericUpDown();
            this.numExtraBytes = new System.Windows.Forms.NumericUpDown();
            this.radioShowTable = new System.Windows.Forms.RadioButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.numExtraOffset1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFileName1 = new System.Windows.Forms.Label();
            this.numExtraOffset2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.labelFileName2 = new System.Windows.Forms.Label();
            this.richTableData2 = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytesPerRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraBytes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset2)).BeginInit();
            this.SuspendLayout();
            // 
            // richTableData
            // 
            this.richTableData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTableData.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTableData.HideSelection = false;
            this.richTableData.Location = new System.Drawing.Point(3, 44);
            this.richTableData.Name = "richTableData";
            this.richTableData.ReadOnly = true;
            this.richTableData.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTableData.Size = new System.Drawing.Size(532, 426);
            this.richTableData.TabIndex = 0;
            this.richTableData.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioSegmentTBNames);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioShowSegment);
            this.groupBox1.Controls.Add(this.numBytesPerRow);
            this.groupBox1.Controls.Add(this.numExtraBytes);
            this.groupBox1.Controls.Add(this.radioShowTable);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1081, 56);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show";
            // 
            // radioSegmentTBNames
            // 
            this.radioSegmentTBNames.AutoSize = true;
            this.radioSegmentTBNames.Location = new System.Drawing.Point(177, 33);
            this.radioSegmentTBNames.Name = "radioSegmentTBNames";
            this.radioSegmentTBNames.Size = new System.Drawing.Size(133, 17);
            this.radioSegmentTBNames.TabIndex = 3;
            this.radioSegmentTBNames.TabStop = true;
            this.radioSegmentTBNames.Text = "Segment + tablenames";
            this.radioSegmentTBNames.UseVisualStyleBackColor = true;
            this.radioSegmentTBNames.CheckedChanged += new System.EventHandler(this.radioSegmentTBNames_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1019, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bytes/row";
            // 
            // radioShowSegment
            // 
            this.radioShowSegment.AutoSize = true;
            this.radioShowSegment.Location = new System.Drawing.Point(177, 15);
            this.radioShowSegment.Name = "radioShowSegment";
            this.radioShowSegment.Size = new System.Drawing.Size(67, 17);
            this.radioShowSegment.TabIndex = 2;
            this.radioShowSegment.TabStop = true;
            this.radioShowSegment.Text = "Segment";
            this.radioShowSegment.UseVisualStyleBackColor = true;
            this.radioShowSegment.CheckedChanged += new System.EventHandler(this.radioShowSegment_CheckedChanged);
            // 
            // numBytesPerRow
            // 
            this.numBytesPerRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numBytesPerRow.Location = new System.Drawing.Point(1022, 30);
            this.numBytesPerRow.Name = "numBytesPerRow";
            this.numBytesPerRow.Size = new System.Drawing.Size(53, 20);
            this.numBytesPerRow.TabIndex = 2;
            this.numBytesPerRow.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numBytesPerRow.ValueChanged += new System.EventHandler(this.numBytesPerRow_ValueChanged);
            // 
            // numExtraBytes
            // 
            this.numExtraBytes.Location = new System.Drawing.Point(120, 17);
            this.numExtraBytes.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numExtraBytes.Name = "numExtraBytes";
            this.numExtraBytes.Size = new System.Drawing.Size(51, 20);
            this.numExtraBytes.TabIndex = 1;
            this.numExtraBytes.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numExtraBytes.ValueChanged += new System.EventHandler(this.numExtraBytes_ValueChanged);
            // 
            // radioShowTable
            // 
            this.radioShowTable.AutoSize = true;
            this.radioShowTable.Checked = true;
            this.radioShowTable.Location = new System.Drawing.Point(13, 18);
            this.radioShowTable.Name = "radioShowTable";
            this.radioShowTable.Size = new System.Drawing.Size(103, 17);
            this.radioShowTable.TabIndex = 0;
            this.radioShowTable.TabStop = true;
            this.radioShowTable.Text = "Table, +/- bytes:";
            this.radioShowTable.UseVisualStyleBackColor = true;
            this.radioShowTable.CheckedChanged += new System.EventHandler(this.radioShowTable_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(-3, 60);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.numExtraOffset1);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.labelFileName1);
            this.splitContainer1.Panel1.Controls.Add(this.richTableData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.numExtraOffset2);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.labelFileName2);
            this.splitContainer1.Panel2.Controls.Add(this.richTableData2);
            this.splitContainer1.Size = new System.Drawing.Size(1090, 473);
            this.splitContainer1.SplitterDistance = 538;
            this.splitContainer1.TabIndex = 4;
            // 
            // numExtraOffset1
            // 
            this.numExtraOffset1.Location = new System.Drawing.Point(78, 21);
            this.numExtraOffset1.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numExtraOffset1.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.numExtraOffset1.Name = "numExtraOffset1";
            this.numExtraOffset1.Size = new System.Drawing.Size(120, 20);
            this.numExtraOffset1.TabIndex = 3;
            this.numExtraOffset1.ValueChanged += new System.EventHandler(this.numExtraOffset1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Extra offset:";
            // 
            // labelFileName1
            // 
            this.labelFileName1.AutoSize = true;
            this.labelFileName1.Location = new System.Drawing.Point(6, 6);
            this.labelFileName1.Name = "labelFileName1";
            this.labelFileName1.Size = new System.Drawing.Size(79, 13);
            this.labelFileName1.TabIndex = 1;
            this.labelFileName1.Text = "labelFileName1";
            // 
            // numExtraOffset2
            // 
            this.numExtraOffset2.Location = new System.Drawing.Point(74, 21);
            this.numExtraOffset2.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numExtraOffset2.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.numExtraOffset2.Name = "numExtraOffset2";
            this.numExtraOffset2.Size = new System.Drawing.Size(120, 20);
            this.numExtraOffset2.TabIndex = 4;
            this.numExtraOffset2.ValueChanged += new System.EventHandler(this.numExtraOffset2_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Extra offset:";
            // 
            // labelFileName2
            // 
            this.labelFileName2.AutoSize = true;
            this.labelFileName2.Location = new System.Drawing.Point(3, 6);
            this.labelFileName2.Name = "labelFileName2";
            this.labelFileName2.Size = new System.Drawing.Size(79, 13);
            this.labelFileName2.TabIndex = 2;
            this.labelFileName2.Text = "labelFileName2";
            // 
            // richTableData2
            // 
            this.richTableData2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTableData2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTableData2.HideSelection = false;
            this.richTableData2.Location = new System.Drawing.Point(3, 44);
            this.richTableData2.Name = "richTableData2";
            this.richTableData2.ReadOnly = true;
            this.richTableData2.Size = new System.Drawing.Size(545, 429);
            this.richTableData2.TabIndex = 1;
            this.richTableData2.Text = "";
            // 
            // frmTableVisDouble
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1090, 534);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmTableVisDouble";
            this.Text = "Offset visualizer";
            this.Load += new System.EventHandler(this.frmTableVis_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytesPerRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraBytes)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTableData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioShowSegment;
        private System.Windows.Forms.NumericUpDown numExtraBytes;
        private System.Windows.Forms.RadioButton radioShowTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numBytesPerRow;
        private System.Windows.Forms.RadioButton radioSegmentTBNames;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTableData2;
        private System.Windows.Forms.Label labelFileName1;
        private System.Windows.Forms.Label labelFileName2;
        private System.Windows.Forms.NumericUpDown numExtraOffset1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numExtraOffset2;
        private System.Windows.Forms.Label label3;
    }
}