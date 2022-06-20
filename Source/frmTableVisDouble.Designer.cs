
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
            this.richTableData1 = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnNextTable = new System.Windows.Forms.Button();
            this.btnPrevTable = new System.Windows.Forms.Button();
            this.radioSegmentTBNames = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.radioShowSegment = new System.Windows.Forms.RadioButton();
            this.numBytesPerRow = new System.Windows.Forms.NumericUpDown();
            this.numExtraBytes = new System.Windows.Forms.NumericUpDown();
            this.radioShowTable = new System.Windows.Forms.RadioButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSelEnd1 = new System.Windows.Forms.Button();
            this.btnSelStart1 = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtInfo1 = new System.Windows.Forms.TextBox();
            this.btnApplyToSelection1 = new System.Windows.Forms.Button();
            this.btnApplyPrimary = new System.Windows.Forms.Button();
            this.numExtraOffset1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFileName1 = new System.Windows.Forms.Label();
            this.btnSelEnd2 = new System.Windows.Forms.Button();
            this.btnSelStart2 = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.richTableData2 = new System.Windows.Forms.RichTextBox();
            this.txtInfo2 = new System.Windows.Forms.TextBox();
            this.btnApplyToSelection2 = new System.Windows.Forms.Button();
            this.btnApplySecondary = new System.Windows.Forms.Button();
            this.numExtraOffset2 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.labelFileName2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBytesPerRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraBytes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset2)).BeginInit();
            this.SuspendLayout();
            // 
            // richTableData1
            // 
            this.richTableData1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTableData1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTableData1.HideSelection = false;
            this.richTableData1.Location = new System.Drawing.Point(0, 0);
            this.richTableData1.Name = "richTableData1";
            this.richTableData1.ReadOnly = true;
            this.richTableData1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTableData1.Size = new System.Drawing.Size(534, 337);
            this.richTableData1.TabIndex = 0;
            this.richTableData1.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnNextTable);
            this.groupBox1.Controls.Add(this.btnPrevTable);
            this.groupBox1.Controls.Add(this.radioSegmentTBNames);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioShowSegment);
            this.groupBox1.Controls.Add(this.numBytesPerRow);
            this.groupBox1.Controls.Add(this.numExtraBytes);
            this.groupBox1.Controls.Add(this.radioShowTable);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1081, 73);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Table:";
            // 
            // btnNextTable
            // 
            this.btnNextTable.Location = new System.Drawing.Point(91, 41);
            this.btnNextTable.Name = "btnNextTable";
            this.btnNextTable.Size = new System.Drawing.Size(32, 23);
            this.btnNextTable.TabIndex = 5;
            this.btnNextTable.Text = ">";
            this.btnNextTable.UseVisualStyleBackColor = true;
            this.btnNextTable.Click += new System.EventHandler(this.btnNextTable_Click);
            // 
            // btnPrevTable
            // 
            this.btnPrevTable.Location = new System.Drawing.Point(53, 41);
            this.btnPrevTable.Name = "btnPrevTable";
            this.btnPrevTable.Size = new System.Drawing.Size(32, 23);
            this.btnPrevTable.TabIndex = 4;
            this.btnPrevTable.Text = "<";
            this.btnPrevTable.UseVisualStyleBackColor = true;
            this.btnPrevTable.Click += new System.EventHandler(this.btnPrevTable_Click);
            // 
            // radioSegmentTBNames
            // 
            this.radioSegmentTBNames.AutoSize = true;
            this.radioSegmentTBNames.Location = new System.Drawing.Point(152, 32);
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
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Bytes/row";
            // 
            // radioShowSegment
            // 
            this.radioShowSegment.AutoSize = true;
            this.radioShowSegment.Location = new System.Drawing.Point(152, 51);
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
            this.numBytesPerRow.Location = new System.Drawing.Point(70, 15);
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
            this.numExtraBytes.Location = new System.Drawing.Point(259, 12);
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
            this.radioShowTable.Location = new System.Drawing.Point(152, 13);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 74);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSelEnd1);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelStart1);
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Controls.Add(this.btnApplyToSelection1);
            this.splitContainer1.Panel1.Controls.Add(this.btnApplyPrimary);
            this.splitContainer1.Panel1.Controls.Add(this.numExtraOffset1);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.labelFileName1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnSelEnd2);
            this.splitContainer1.Panel2.Controls.Add(this.btnSelStart2);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.Controls.Add(this.btnApplyToSelection2);
            this.splitContainer1.Panel2.Controls.Add(this.btnApplySecondary);
            this.splitContainer1.Panel2.Controls.Add(this.numExtraOffset2);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.labelFileName2);
            this.splitContainer1.Size = new System.Drawing.Size(1087, 459);
            this.splitContainer1.SplitterDistance = 542;
            this.splitContainer1.TabIndex = 4;
            // 
            // btnSelEnd1
            // 
            this.btnSelEnd1.Location = new System.Drawing.Point(446, 20);
            this.btnSelEnd1.Name = "btnSelEnd1";
            this.btnSelEnd1.Size = new System.Drawing.Size(53, 22);
            this.btnSelEnd1.TabIndex = 9;
            this.btnSelEnd1.Text = "SelEnd";
            this.btnSelEnd1.UseVisualStyleBackColor = true;
            this.btnSelEnd1.Click += new System.EventHandler(this.btnSelEnd1_Click);
            // 
            // btnSelStart1
            // 
            this.btnSelStart1.Location = new System.Drawing.Point(387, 20);
            this.btnSelStart1.Name = "btnSelStart1";
            this.btnSelStart1.Size = new System.Drawing.Size(53, 22);
            this.btnSelStart1.TabIndex = 8;
            this.btnSelStart1.Text = "SelStart";
            this.btnSelStart1.UseVisualStyleBackColor = true;
            this.btnSelStart1.Click += new System.EventHandler(this.btnSelStart1_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 47);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.richTableData1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtInfo1);
            this.splitContainer2.Size = new System.Drawing.Size(534, 409);
            this.splitContainer2.SplitterDistance = 337;
            this.splitContainer2.TabIndex = 7;
            // 
            // txtInfo1
            // 
            this.txtInfo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo1.Location = new System.Drawing.Point(0, 0);
            this.txtInfo1.Multiline = true;
            this.txtInfo1.Name = "txtInfo1";
            this.txtInfo1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfo1.Size = new System.Drawing.Size(534, 68);
            this.txtInfo1.TabIndex = 6;
            // 
            // btnApplyToSelection1
            // 
            this.btnApplyToSelection1.Location = new System.Drawing.Point(274, 21);
            this.btnApplyToSelection1.Name = "btnApplyToSelection1";
            this.btnApplyToSelection1.Size = new System.Drawing.Size(107, 22);
            this.btnApplyToSelection1.TabIndex = 5;
            this.btnApplyToSelection1.Text = "Apply to selection";
            this.btnApplyToSelection1.UseVisualStyleBackColor = true;
            this.btnApplyToSelection1.Click += new System.EventHandler(this.btnApplyToSelection1_Click);
            // 
            // btnApplyPrimary
            // 
            this.btnApplyPrimary.Location = new System.Drawing.Point(204, 21);
            this.btnApplyPrimary.Name = "btnApplyPrimary";
            this.btnApplyPrimary.Size = new System.Drawing.Size(64, 22);
            this.btnApplyPrimary.TabIndex = 4;
            this.btnApplyPrimary.Text = "Apply";
            this.btnApplyPrimary.UseVisualStyleBackColor = true;
            this.btnApplyPrimary.Click += new System.EventHandler(this.btnApplyPrimary_Click);
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
            // btnSelEnd2
            // 
            this.btnSelEnd2.Location = new System.Drawing.Point(442, 21);
            this.btnSelEnd2.Name = "btnSelEnd2";
            this.btnSelEnd2.Size = new System.Drawing.Size(53, 22);
            this.btnSelEnd2.TabIndex = 11;
            this.btnSelEnd2.Text = "SelEnd";
            this.btnSelEnd2.UseVisualStyleBackColor = true;
            this.btnSelEnd2.Click += new System.EventHandler(this.btnSelEnd2_Click);
            // 
            // btnSelStart2
            // 
            this.btnSelStart2.Location = new System.Drawing.Point(383, 21);
            this.btnSelStart2.Name = "btnSelStart2";
            this.btnSelStart2.Size = new System.Drawing.Size(53, 22);
            this.btnSelStart2.TabIndex = 10;
            this.btnSelStart2.Text = "SelStart";
            this.btnSelStart2.UseVisualStyleBackColor = true;
            this.btnSelStart2.Click += new System.EventHandler(this.btnSelStart2_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(-1, 47);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.richTableData2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtInfo2);
            this.splitContainer3.Size = new System.Drawing.Size(538, 412);
            this.splitContainer3.SplitterDistance = 335;
            this.splitContainer3.TabIndex = 7;
            // 
            // richTableData2
            // 
            this.richTableData2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTableData2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTableData2.HideSelection = false;
            this.richTableData2.Location = new System.Drawing.Point(0, 0);
            this.richTableData2.Name = "richTableData2";
            this.richTableData2.ReadOnly = true;
            this.richTableData2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTableData2.Size = new System.Drawing.Size(538, 335);
            this.richTableData2.TabIndex = 1;
            this.richTableData2.Text = "";
            // 
            // txtInfo2
            // 
            this.txtInfo2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo2.Location = new System.Drawing.Point(0, 0);
            this.txtInfo2.Multiline = true;
            this.txtInfo2.Name = "txtInfo2";
            this.txtInfo2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfo2.Size = new System.Drawing.Size(538, 73);
            this.txtInfo2.TabIndex = 0;
            // 
            // btnApplyToSelection2
            // 
            this.btnApplyToSelection2.Location = new System.Drawing.Point(270, 21);
            this.btnApplyToSelection2.Name = "btnApplyToSelection2";
            this.btnApplyToSelection2.Size = new System.Drawing.Size(107, 22);
            this.btnApplyToSelection2.TabIndex = 6;
            this.btnApplyToSelection2.Text = "Apply to selection";
            this.btnApplyToSelection2.UseVisualStyleBackColor = true;
            this.btnApplyToSelection2.Click += new System.EventHandler(this.btnApplyToSelection2_Click);
            // 
            // btnApplySecondary
            // 
            this.btnApplySecondary.Location = new System.Drawing.Point(200, 21);
            this.btnApplySecondary.Name = "btnApplySecondary";
            this.btnApplySecondary.Size = new System.Drawing.Size(64, 22);
            this.btnApplySecondary.TabIndex = 5;
            this.btnApplySecondary.Text = "Apply";
            this.btnApplySecondary.UseVisualStyleBackColor = true;
            this.btnApplySecondary.Click += new System.EventHandler(this.btnApplySecondary_Click);
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
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset1)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTableData1;
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
        private System.Windows.Forms.Button btnApplyPrimary;
        private System.Windows.Forms.Button btnApplySecondary;
        private System.Windows.Forms.Button btnNextTable;
        private System.Windows.Forms.Button btnPrevTable;
        private System.Windows.Forms.Button btnApplyToSelection1;
        private System.Windows.Forms.Button btnApplyToSelection2;
        private System.Windows.Forms.TextBox txtInfo1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox txtInfo2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSelEnd1;
        private System.Windows.Forms.Button btnSelStart1;
        private System.Windows.Forms.Button btnSelEnd2;
        private System.Windows.Forms.Button btnSelStart2;
    }
}