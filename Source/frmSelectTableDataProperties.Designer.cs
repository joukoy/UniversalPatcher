
namespace UniversalPatcher
{
    partial class frmSelectTableDataProperties
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
            this.btnOK = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioAddMissing = new System.Windows.Forms.RadioButton();
            this.radioOwerwriteAdd = new System.Windows.Forms.RadioButton();
            this.radioOwerwrite = new System.Windows.Forms.RadioButton();
            this.radioAdd = new System.Windows.Forms.RadioButton();
            this.labelAction = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(386, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(12, 11);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(69, 17);
            this.chkSelectAll.TabIndex = 1;
            this.chkSelectAll.Text = "Select all";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(6, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(462, 417);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.radioAddMissing);
            this.groupBox2.Controls.Add(this.radioOwerwriteAdd);
            this.groupBox2.Controls.Add(this.radioOwerwrite);
            this.groupBox2.Controls.Add(this.radioAdd);
            this.groupBox2.Location = new System.Drawing.Point(287, 113);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(168, 116);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Copy mode:";
            this.groupBox2.Visible = false;
            // 
            // radioAddMissing
            // 
            this.radioAddMissing.AutoSize = true;
            this.radioAddMissing.Location = new System.Drawing.Point(6, 39);
            this.radioAddMissing.Name = "radioAddMissing";
            this.radioAddMissing.Size = new System.Drawing.Size(94, 17);
            this.radioAddMissing.TabIndex = 3;
            this.radioAddMissing.Text = "Add if not exist";
            this.radioAddMissing.UseVisualStyleBackColor = true;
            // 
            // radioOwerwriteAdd
            // 
            this.radioOwerwriteAdd.AutoSize = true;
            this.radioOwerwriteAdd.Location = new System.Drawing.Point(6, 85);
            this.radioOwerwriteAdd.Name = "radioOwerwriteAdd";
            this.radioOwerwriteAdd.Size = new System.Drawing.Size(151, 17);
            this.radioOwerwriteAdd.TabIndex = 2;
            this.radioOwerwriteAdd.Text = "Owerwite if exist, add if not";
            this.radioOwerwriteAdd.UseVisualStyleBackColor = true;
            // 
            // radioOwerwrite
            // 
            this.radioOwerwrite.AutoSize = true;
            this.radioOwerwrite.Location = new System.Drawing.Point(6, 62);
            this.radioOwerwrite.Name = "radioOwerwrite";
            this.radioOwerwrite.Size = new System.Drawing.Size(104, 17);
            this.radioOwerwrite.TabIndex = 1;
            this.radioOwerwrite.Text = "Owerwrite if exist";
            this.radioOwerwrite.UseVisualStyleBackColor = true;
            // 
            // radioAdd
            // 
            this.radioAdd.AutoSize = true;
            this.radioAdd.Checked = true;
            this.radioAdd.Location = new System.Drawing.Point(6, 19);
            this.radioAdd.Name = "radioAdd";
            this.radioAdd.Size = new System.Drawing.Size(81, 17);
            this.radioAdd.TabIndex = 0;
            this.radioAdd.TabStop = true;
            this.radioAdd.Text = "Add as new";
            this.radioAdd.UseVisualStyleBackColor = true;
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAction.Location = new System.Drawing.Point(101, 11);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(131, 16);
            this.labelAction.TabIndex = 3;
            this.labelAction.Text = "Select properties:";
            // 
            // frmSelectTableDataProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 450);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.btnOK);
            this.Name = "frmSelectTableDataProperties";
            this.Text = "Select properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button btnOK;
        public System.Windows.Forms.Label labelAction;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.RadioButton radioAddMissing;
        public System.Windows.Forms.RadioButton radioOwerwriteAdd;
        public System.Windows.Forms.RadioButton radioOwerwrite;
        public System.Windows.Forms.RadioButton radioAdd;
    }
}