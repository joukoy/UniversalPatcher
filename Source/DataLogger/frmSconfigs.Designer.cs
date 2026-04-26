
namespace UniversalPatcher
{
    partial class frmSconfigs
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
            this.txtSConfigs = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupValue = new System.Windows.Forms.GroupBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.radioHex = new System.Windows.Forms.RadioButton();
            this.radioDec = new System.Windows.Forms.RadioButton();
            this.chkAppendRow = new System.Windows.Forms.CheckBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.groupValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSConfigs
            // 
            this.txtSConfigs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSConfigs.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSConfigs.Location = new System.Drawing.Point(4, 404);
            this.txtSConfigs.Multiline = true;
            this.txtSConfigs.Name = "txtSConfigs";
            this.txtSConfigs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSConfigs.Size = new System.Drawing.Size(653, 83);
            this.txtSConfigs.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(571, 493);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(78, 26);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 500);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Comma separated settings in line are sent together";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(4, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(465, 290);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 5;
            this.listBox1.DoubleClick += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(571, 208);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(78, 26);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupValue
            // 
            this.groupValue.Controls.Add(this.txtValue);
            this.groupValue.Controls.Add(this.radioHex);
            this.groupValue.Controls.Add(this.radioDec);
            this.groupValue.Location = new System.Drawing.Point(494, 68);
            this.groupValue.Name = "groupValue";
            this.groupValue.Size = new System.Drawing.Size(162, 111);
            this.groupValue.TabIndex = 7;
            this.groupValue.TabStop = false;
            this.groupValue.Text = "Value";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(8, 75);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(147, 20);
            this.txtValue.TabIndex = 2;
            this.txtValue.Text = "1";
            this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
            // 
            // radioHex
            // 
            this.radioHex.AutoSize = true;
            this.radioHex.Location = new System.Drawing.Point(10, 45);
            this.radioHex.Name = "radioHex";
            this.radioHex.Size = new System.Drawing.Size(47, 17);
            this.radioHex.TabIndex = 1;
            this.radioHex.Text = "HEX";
            this.radioHex.UseVisualStyleBackColor = true;
            this.radioHex.CheckedChanged += new System.EventHandler(this.radioHex_CheckedChanged);
            // 
            // radioDec
            // 
            this.radioDec.AutoSize = true;
            this.radioDec.Checked = true;
            this.radioDec.Location = new System.Drawing.Point(10, 22);
            this.radioDec.Name = "radioDec";
            this.radioDec.Size = new System.Drawing.Size(63, 17);
            this.radioDec.TabIndex = 0;
            this.radioDec.TabStop = true;
            this.radioDec.Text = "Decimal";
            this.radioDec.UseVisualStyleBackColor = true;
            // 
            // chkAppendRow
            // 
            this.chkAppendRow.AutoSize = true;
            this.chkAppendRow.Location = new System.Drawing.Point(499, 185);
            this.chkAppendRow.Name = "chkAppendRow";
            this.chkAppendRow.Size = new System.Drawing.Size(94, 17);
            this.chkAppendRow.TabIndex = 8;
            this.chkAppendRow.Text = "Append to line";
            this.chkAppendRow.UseVisualStyleBackColor = true;
            // 
            // labelValue
            // 
            this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelValue.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValue.Location = new System.Drawing.Point(6, 298);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(650, 103);
            this.labelValue.TabIndex = 9;
            // 
            // frmSconfigs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 526);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.chkAppendRow);
            this.Controls.Add(this.groupValue);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtSConfigs);
            this.Name = "frmSconfigs";
            this.Text = "Sconfigs";
            this.Load += new System.EventHandler(this.frmSconfigs_Load);
            this.groupValue.ResumeLayout(false);
            this.groupValue.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSConfigs;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox groupValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.RadioButton radioHex;
        private System.Windows.Forms.RadioButton radioDec;
        private System.Windows.Forms.CheckBox chkAppendRow;
        private System.Windows.Forms.Label labelValue;
    }
}