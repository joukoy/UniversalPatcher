namespace UniversalPatcher
{
    partial class frmSwapSegmentList
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
            this.comboSegments = new System.Windows.Forms.ComboBox();
            this.listSegments = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExtract = new System.Windows.Forms.Button();
            this.labelSelectedSegment = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.labelBasefile = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkLessChance = new System.Windows.Forms.CheckBox();
            this.chkHighChance = new System.Windows.Forms.CheckBox();
            this.chkFullmatch = new System.Windows.Forms.CheckBox();
            this.btnSavelist = new System.Windows.Forms.Button();
            this.radioShow1x0 = new System.Windows.Forms.RadioButton();
            this.radioShowRange = new System.Windows.Forms.RadioButton();
            this.chkSkipeeprom = new System.Windows.Forms.CheckBox();
            this.txtSkiptext = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioShowSize = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboSegments
            // 
            this.comboSegments.FormattingEnabled = true;
            this.comboSegments.Location = new System.Drawing.Point(58, 28);
            this.comboSegments.Name = "comboSegments";
            this.comboSegments.Size = new System.Drawing.Size(330, 21);
            this.comboSegments.TabIndex = 0;
            this.comboSegments.SelectedIndexChanged += new System.EventHandler(this.comboSegments_SelectedIndexChanged);
            // 
            // listSegments
            // 
            this.listSegments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSegments.HideSelection = false;
            this.listSegments.Location = new System.Drawing.Point(3, 116);
            this.listSegments.Name = "listSegments";
            this.listSegments.Size = new System.Drawing.Size(774, 190);
            this.listSegments.TabIndex = 1;
            this.listSegments.UseCompatibleStateImageBehavior = false;
            this.listSegments.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listSegments_ColumnClick);
            this.listSegments.SelectedIndexChanged += new System.EventHandler(this.listSegments_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Segment:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Extracted segments:";
            // 
            // btnExtract
            // 
            this.btnExtract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExtract.Location = new System.Drawing.Point(3, 312);
            this.btnExtract.Name = "btnExtract";
            this.btnExtract.Size = new System.Drawing.Size(106, 26);
            this.btnExtract.TabIndex = 4;
            this.btnExtract.Text = "Extract from file...";
            this.btnExtract.UseVisualStyleBackColor = true;
            this.btnExtract.Click += new System.EventHandler(this.btnExtract_Click);
            // 
            // labelSelectedSegment
            // 
            this.labelSelectedSegment.AutoSize = true;
            this.labelSelectedSegment.Location = new System.Drawing.Point(0, 75);
            this.labelSelectedSegment.Name = "labelSelectedSegment";
            this.labelSelectedSegment.Size = new System.Drawing.Size(10, 13);
            this.labelSelectedSegment.TabIndex = 5;
            this.labelSelectedSegment.Text = "-";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Location = new System.Drawing.Point(329, 312);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(106, 26);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(4, 344);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(773, 137);
            this.txtResult.TabIndex = 10;
            this.txtResult.Text = "";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(671, 312);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(106, 26);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labelBasefile
            // 
            this.labelBasefile.AutoSize = true;
            this.labelBasefile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBasefile.Location = new System.Drawing.Point(5, 7);
            this.labelBasefile.Name = "labelBasefile";
            this.labelBasefile.Size = new System.Drawing.Size(12, 16);
            this.labelBasefile.TabIndex = 9;
            this.labelBasefile.Text = "-";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkLessChance);
            this.groupBox1.Controls.Add(this.chkHighChance);
            this.groupBox1.Controls.Add(this.chkFullmatch);
            this.groupBox1.Location = new System.Drawing.Point(532, 75);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 38);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show:";
            // 
            // chkLessChance
            // 
            this.chkLessChance.AutoSize = true;
            this.chkLessChance.Location = new System.Drawing.Point(157, 16);
            this.chkLessChance.Name = "chkLessChance";
            this.chkLessChance.Size = new System.Drawing.Size(83, 17);
            this.chkLessChance.TabIndex = 2;
            this.chkLessChance.Text = "less chance";
            this.chkLessChance.UseVisualStyleBackColor = true;
            this.chkLessChance.CheckedChanged += new System.EventHandler(this.chkLessChance_CheckedChanged);
            // 
            // chkHighChance
            // 
            this.chkHighChance.AutoSize = true;
            this.chkHighChance.Location = new System.Drawing.Point(64, 16);
            this.chkHighChance.Name = "chkHighChance";
            this.chkHighChance.Size = new System.Drawing.Size(87, 17);
            this.chkHighChance.TabIndex = 1;
            this.chkHighChance.Text = "High chance";
            this.chkHighChance.UseVisualStyleBackColor = true;
            this.chkHighChance.CheckedChanged += new System.EventHandler(this.chkHighChance_CheckedChanged);
            // 
            // chkFullmatch
            // 
            this.chkFullmatch.AutoSize = true;
            this.chkFullmatch.Checked = true;
            this.chkFullmatch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFullmatch.Location = new System.Drawing.Point(6, 16);
            this.chkFullmatch.Name = "chkFullmatch";
            this.chkFullmatch.Size = new System.Drawing.Size(52, 17);
            this.chkFullmatch.TabIndex = 0;
            this.chkFullmatch.Text = "100%";
            this.chkFullmatch.UseVisualStyleBackColor = true;
            this.chkFullmatch.CheckedChanged += new System.EventHandler(this.chkFullmatch_CheckedChanged);
            // 
            // btnSavelist
            // 
            this.btnSavelist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavelist.Location = new System.Drawing.Point(563, 312);
            this.btnSavelist.Name = "btnSavelist";
            this.btnSavelist.Size = new System.Drawing.Size(106, 26);
            this.btnSavelist.TabIndex = 12;
            this.btnSavelist.Text = "Save list...";
            this.btnSavelist.UseVisualStyleBackColor = true;
            this.btnSavelist.Click += new System.EventHandler(this.btnSavelist_Click);
            // 
            // radioShow1x0
            // 
            this.radioShow1x0.AutoSize = true;
            this.radioShow1x0.Location = new System.Drawing.Point(119, 36);
            this.radioShow1x0.Name = "radioShow1x0";
            this.radioShow1x0.Size = new System.Drawing.Size(54, 17);
            this.radioShow1x0.TabIndex = 0;
            this.radioShow1x0.Text = "1/X/0";
            this.radioShow1x0.UseVisualStyleBackColor = true;
            this.radioShow1x0.CheckedChanged += new System.EventHandler(this.radioShow1x0_CheckedChanged);
            // 
            // radioShowRange
            // 
            this.radioShowRange.AutoSize = true;
            this.radioShowRange.Checked = true;
            this.radioShowRange.Location = new System.Drawing.Point(5, 36);
            this.radioShowRange.Name = "radioShowRange";
            this.radioShowRange.Size = new System.Drawing.Size(57, 17);
            this.radioShowRange.TabIndex = 3;
            this.radioShowRange.TabStop = true;
            this.radioShowRange.Text = "Range";
            this.radioShowRange.UseVisualStyleBackColor = true;
            this.radioShowRange.CheckedChanged += new System.EventHandler(this.radioShowRange_CheckedChanged);
            // 
            // chkSkipeeprom
            // 
            this.chkSkipeeprom.AutoSize = true;
            this.chkSkipeeprom.Checked = true;
            this.chkSkipeeprom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkipeeprom.Location = new System.Drawing.Point(5, 15);
            this.chkSkipeeprom.Name = "chkSkipeeprom";
            this.chkSkipeeprom.Size = new System.Drawing.Size(202, 17);
            this.chkSkipeeprom.TabIndex = 13;
            this.chkSkipeeprom.Text = "Skip segments, name containing text:";
            this.chkSkipeeprom.UseVisualStyleBackColor = true;
            this.chkSkipeeprom.CheckedChanged += new System.EventHandler(this.chkSkipeeprom_CheckedChanged);
            // 
            // txtSkiptext
            // 
            this.txtSkiptext.Location = new System.Drawing.Point(223, 13);
            this.txtSkiptext.Name = "txtSkiptext";
            this.txtSkiptext.Size = new System.Drawing.Size(128, 20);
            this.txtSkiptext.TabIndex = 14;
            this.txtSkiptext.Text = "eeprom";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.radioShowSize);
            this.groupBox2.Controls.Add(this.radioShowRange);
            this.groupBox2.Controls.Add(this.chkSkipeeprom);
            this.groupBox2.Controls.Add(this.radioShow1x0);
            this.groupBox2.Controls.Add(this.txtSkiptext);
            this.groupBox2.Location = new System.Drawing.Point(408, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(362, 62);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "OS compatibility";
            // 
            // radioShowSize
            // 
            this.radioShowSize.AutoSize = true;
            this.radioShowSize.Location = new System.Drawing.Point(68, 36);
            this.radioShowSize.Name = "radioShowSize";
            this.radioShowSize.Size = new System.Drawing.Size(45, 17);
            this.radioShowSize.TabIndex = 15;
            this.radioShowSize.TabStop = true;
            this.radioShowSize.Text = "Size";
            this.radioShowSize.UseVisualStyleBackColor = true;
            this.radioShowSize.CheckedChanged += new System.EventHandler(this.radioShowSize_CheckedChanged);
            // 
            // frmSwapSegmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 481);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSavelist);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelBasefile);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.labelSelectedSegment);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listSegments);
            this.Controls.Add(this.comboSegments);
            this.Name = "frmSwapSegmentList";
            this.Text = "Swap segment(s):";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboSegments;
        private System.Windows.Forms.ListView listSegments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label labelSelectedSegment;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelBasefile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkLessChance;
        private System.Windows.Forms.CheckBox chkHighChance;
        private System.Windows.Forms.CheckBox chkFullmatch;
        private System.Windows.Forms.Button btnSavelist;
        private System.Windows.Forms.RadioButton radioShowRange;
        private System.Windows.Forms.RadioButton radioShow1x0;
        private System.Windows.Forms.CheckBox chkSkipeeprom;
        private System.Windows.Forms.TextBox txtSkiptext;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioShowSize;
    }
}