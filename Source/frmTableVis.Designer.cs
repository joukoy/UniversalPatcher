
namespace UniversalPatcher
{
    partial class frmTableVis
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
            this.radioShowSegment = new System.Windows.Forms.RadioButton();
            this.numExtraBytes = new System.Windows.Forms.NumericUpDown();
            this.radioShowTable = new System.Windows.Forms.RadioButton();
            this.numBytesPerRow = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.radioSegmentTBNames = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraBytes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBytesPerRow)).BeginInit();
            this.SuspendLayout();
            // 
            // richTableData
            // 
            this.richTableData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTableData.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTableData.HideSelection = false;
            this.richTableData.Location = new System.Drawing.Point(2, 53);
            this.richTableData.Name = "richTableData";
            this.richTableData.ReadOnly = true;
            this.richTableData.Size = new System.Drawing.Size(552, 396);
            this.richTableData.TabIndex = 0;
            this.richTableData.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioSegmentTBNames);
            this.groupBox1.Controls.Add(this.radioShowSegment);
            this.groupBox1.Controls.Add(this.numExtraBytes);
            this.groupBox1.Controls.Add(this.radioShowTable);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(416, 50);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show";
            // 
            // radioShowSegment
            // 
            this.radioShowSegment.AutoSize = true;
            this.radioShowSegment.Location = new System.Drawing.Point(178, 8);
            this.radioShowSegment.Name = "radioShowSegment";
            this.radioShowSegment.Size = new System.Drawing.Size(67, 17);
            this.radioShowSegment.TabIndex = 2;
            this.radioShowSegment.TabStop = true;
            this.radioShowSegment.Text = "Segment";
            this.radioShowSegment.UseVisualStyleBackColor = true;
            this.radioShowSegment.CheckedChanged += new System.EventHandler(this.radioShowSegment_CheckedChanged);
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
            // 
            // numBytesPerRow
            // 
            this.numBytesPerRow.Location = new System.Drawing.Point(489, 19);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(428, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bytes/row";
            // 
            // radioSegmentTBNames
            // 
            this.radioSegmentTBNames.AutoSize = true;
            this.radioSegmentTBNames.Location = new System.Drawing.Point(178, 26);
            this.radioSegmentTBNames.Name = "radioSegmentTBNames";
            this.radioSegmentTBNames.Size = new System.Drawing.Size(133, 17);
            this.radioSegmentTBNames.TabIndex = 3;
            this.radioSegmentTBNames.TabStop = true;
            this.radioSegmentTBNames.Text = "Segment + tablenames";
            this.radioSegmentTBNames.UseVisualStyleBackColor = true;
            this.radioSegmentTBNames.CheckedChanged += new System.EventHandler(this.radioSegmentTBNames_CheckedChanged);
            // 
            // frmTableVis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numBytesPerRow);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.richTableData);
            this.Name = "frmTableVis";
            this.Text = "Table data visualizer";
            this.Load += new System.EventHandler(this.frmTableVis_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraBytes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBytesPerRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}