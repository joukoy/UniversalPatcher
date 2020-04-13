namespace UniversalPatcher
{
    partial class frmCompare
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
            this.btnFile1 = new System.Windows.Forms.Button();
            this.btnFile2 = new System.Windows.Forms.Button();
            this.labelFile1 = new System.Windows.Forms.Label();
            this.labelFile2 = new System.Windows.Forms.Label();
            this.chkAutodetect = new System.Windows.Forms.CheckBox();
            this.labelXML = new System.Windows.Forms.Label();
            this.btnLoadConfig = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.numSuppress = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkAppendPatch = new System.Windows.Forms.CheckBox();
            this.chkCompareAll = new System.Windows.Forms.CheckBox();
            this.txtOS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelBinSize = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFile1
            // 
            this.btnFile1.Location = new System.Drawing.Point(6, 5);
            this.btnFile1.Name = "btnFile1";
            this.btnFile1.Size = new System.Drawing.Size(76, 26);
            this.btnFile1.TabIndex = 0;
            this.btnFile1.Text = "File 1";
            this.btnFile1.UseVisualStyleBackColor = true;
            this.btnFile1.Click += new System.EventHandler(this.btnFile1_Click);
            // 
            // btnFile2
            // 
            this.btnFile2.Location = new System.Drawing.Point(6, 37);
            this.btnFile2.Name = "btnFile2";
            this.btnFile2.Size = new System.Drawing.Size(76, 26);
            this.btnFile2.TabIndex = 1;
            this.btnFile2.Text = "File 2";
            this.btnFile2.UseVisualStyleBackColor = true;
            this.btnFile2.Click += new System.EventHandler(this.btnFile2_Click);
            // 
            // labelFile1
            // 
            this.labelFile1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFile1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFile1.Location = new System.Drawing.Point(93, 9);
            this.labelFile1.Name = "labelFile1";
            this.labelFile1.Size = new System.Drawing.Size(695, 23);
            this.labelFile1.TabIndex = 2;
            this.labelFile1.Text = "-";
            this.labelFile1.Click += new System.EventHandler(this.labelFile1_Click);
            // 
            // labelFile2
            // 
            this.labelFile2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFile2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFile2.Location = new System.Drawing.Point(93, 40);
            this.labelFile2.Name = "labelFile2";
            this.labelFile2.Size = new System.Drawing.Size(695, 23);
            this.labelFile2.TabIndex = 3;
            this.labelFile2.Text = "-";
            this.labelFile2.Click += new System.EventHandler(this.labelFile2_Click);
            // 
            // chkAutodetect
            // 
            this.chkAutodetect.AutoSize = true;
            this.chkAutodetect.Checked = true;
            this.chkAutodetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutodetect.Location = new System.Drawing.Point(93, 75);
            this.chkAutodetect.Name = "chkAutodetect";
            this.chkAutodetect.Size = new System.Drawing.Size(110, 17);
            this.chkAutodetect.TabIndex = 4;
            this.chkAutodetect.Text = "Autodetect config";
            this.chkAutodetect.UseVisualStyleBackColor = true;
            // 
            // labelXML
            // 
            this.labelXML.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelXML.Location = new System.Drawing.Point(220, 68);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(568, 23);
            this.labelXML.TabIndex = 5;
            // 
            // btnLoadConfig
            // 
            this.btnLoadConfig.Location = new System.Drawing.Point(6, 68);
            this.btnLoadConfig.Name = "btnLoadConfig";
            this.btnLoadConfig.Size = new System.Drawing.Size(76, 26);
            this.btnLoadConfig.TabIndex = 6;
            this.btnLoadConfig.Text = "Load config";
            this.btnLoadConfig.UseVisualStyleBackColor = true;
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(6, 118);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(76, 26);
            this.btnCompare.TabIndex = 7;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // numSuppress
            // 
            this.numSuppress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numSuppress.Location = new System.Drawing.Point(545, 122);
            this.numSuppress.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSuppress.Name = "numSuppress";
            this.numSuppress.Size = new System.Drawing.Size(42, 20);
            this.numSuppress.TabIndex = 204;
            this.numSuppress.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(480, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 202;
            this.label2.Text = "Show max:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(588, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 203;
            this.label3.Text = "patch rows";
            // 
            // checkAppendPatch
            // 
            this.checkAppendPatch.AutoSize = true;
            this.checkAppendPatch.Location = new System.Drawing.Point(409, 124);
            this.checkAppendPatch.Name = "checkAppendPatch";
            this.checkAppendPatch.Size = new System.Drawing.Size(63, 17);
            this.checkAppendPatch.TabIndex = 206;
            this.checkAppendPatch.Text = "Append";
            this.checkAppendPatch.UseVisualStyleBackColor = true;
            // 
            // chkCompareAll
            // 
            this.chkCompareAll.AutoSize = true;
            this.chkCompareAll.Location = new System.Drawing.Point(236, 125);
            this.chkCompareAll.Name = "chkCompareAll";
            this.chkCompareAll.Size = new System.Drawing.Size(167, 17);
            this.chkCompareAll.TabIndex = 205;
            this.chkCompareAll.Text = "Compare all (ignore segments)";
            this.chkCompareAll.UseVisualStyleBackColor = true;
            // 
            // txtOS
            // 
            this.txtOS.Location = new System.Drawing.Point(124, 122);
            this.txtOS.Name = "txtOS";
            this.txtOS.Size = new System.Drawing.Size(106, 20);
            this.txtOS.TabIndex = 208;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(93, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 207;
            this.label7.Text = "OS:";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 210;
            this.label1.Text = "BIN Size:";
            // 
            // labelBinSize
            // 
            this.labelBinSize.AutoSize = true;
            this.labelBinSize.Location = new System.Drawing.Point(60, 152);
            this.labelBinSize.Name = "labelBinSize";
            this.labelBinSize.Size = new System.Drawing.Size(10, 13);
            this.labelBinSize.TabIndex = 209;
            this.labelBinSize.Text = "-";
            // 
            // frmCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 174);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelBinSize);
            this.Controls.Add(this.txtOS);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.checkAppendPatch);
            this.Controls.Add(this.chkCompareAll);
            this.Controls.Add(this.numSuppress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnLoadConfig);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.chkAutodetect);
            this.Controls.Add(this.labelFile2);
            this.Controls.Add(this.labelFile1);
            this.Controls.Add(this.btnFile2);
            this.Controls.Add(this.btnFile1);
            this.Name = "frmCompare";
            this.Text = "Compare files";
            ((System.ComponentModel.ISupportInitialize)(this.numSuppress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFile1;
        private System.Windows.Forms.Button btnFile2;
        private System.Windows.Forms.Label labelFile1;
        private System.Windows.Forms.Label labelFile2;
        private System.Windows.Forms.CheckBox chkAutodetect;
        private System.Windows.Forms.Label labelXML;
        private System.Windows.Forms.Button btnLoadConfig;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.NumericUpDown numSuppress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkAppendPatch;
        private System.Windows.Forms.CheckBox chkCompareAll;
        private System.Windows.Forms.TextBox txtOS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelBinSize;
    }
}