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
            this.btnOK = new System.Windows.Forms.Button();
            this.labelBasefile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboSegments
            // 
            this.comboSegments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSegments.FormattingEnabled = true;
            this.comboSegments.Location = new System.Drawing.Point(58, 32);
            this.comboSegments.Name = "comboSegments";
            this.comboSegments.Size = new System.Drawing.Size(719, 21);
            this.comboSegments.TabIndex = 0;
            this.comboSegments.SelectedIndexChanged += new System.EventHandler(this.comboSegments_SelectedIndexChanged);
            // 
            // listSegments
            // 
            this.listSegments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listSegments.FullRowSelect = true;
            this.listSegments.HideSelection = false;
            this.listSegments.Location = new System.Drawing.Point(3, 102);
            this.listSegments.Name = "listSegments";
            this.listSegments.Size = new System.Drawing.Size(774, 171);
            this.listSegments.TabIndex = 1;
            this.listSegments.UseCompatibleStateImageBehavior = false;
            this.listSegments.SelectedIndexChanged += new System.EventHandler(this.listSegments_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Segment:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Extracted segments:";
            // 
            // btnExtract
            // 
            this.btnExtract.Location = new System.Drawing.Point(3, 279);
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
            this.labelSelectedSegment.Location = new System.Drawing.Point(0, 63);
            this.labelSelectedSegment.Name = "labelSelectedSegment";
            this.labelSelectedSegment.Size = new System.Drawing.Size(10, 13);
            this.labelSelectedSegment.TabIndex = 5;
            this.labelSelectedSegment.Text = "-";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(335, 278);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(106, 26);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(667, 280);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(106, 26);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "Save bin";
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
            // frmSwapSegmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 310);
            this.Controls.Add(this.labelBasefile);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.labelSelectedSegment);
            this.Controls.Add(this.btnExtract);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listSegments);
            this.Controls.Add(this.comboSegments);
            this.Name = "frmSwapSegmentList";
            this.Text = "Swap segment(s):";
            this.Load += new System.EventHandler(this.frmSwapSegmentList_Load);
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
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label labelBasefile;
    }
}