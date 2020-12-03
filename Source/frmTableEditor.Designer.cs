using System;

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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtMath = new System.Windows.Forms.TextBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.chkAutoResize = new System.Windows.Forms.CheckBox();
            this.chkTranspose = new System.Windows.Forms.CheckBox();
            this.labelUnits = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.dataGridView1.Location = new System.Drawing.Point(2, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(796, 408);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellEndEdit);
            // 
            // txtMath
            // 
            this.txtMath.Location = new System.Drawing.Point(8, 0);
            this.txtMath.Name = "txtMath";
            this.txtMath.Size = new System.Drawing.Size(100, 20);
            this.txtMath.TabIndex = 1;
            this.txtMath.Text = "X*1";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(114, 3);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(68, 31);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // chkAutoResize
            // 
            this.chkAutoResize.AutoSize = true;
            this.chkAutoResize.Location = new System.Drawing.Point(188, 2);
            this.chkAutoResize.Name = "chkAutoResize";
            this.chkAutoResize.Size = new System.Drawing.Size(78, 17);
            this.chkAutoResize.TabIndex = 3;
            this.chkAutoResize.Text = "Auto resize";
            this.chkAutoResize.UseVisualStyleBackColor = true;
            this.chkAutoResize.CheckedChanged += new System.EventHandler(this.chkAutoResize_CheckedChanged);
            // 
            // chkTranspose
            // 
            this.chkTranspose.AutoSize = true;
            this.chkTranspose.Location = new System.Drawing.Point(188, 17);
            this.chkTranspose.Name = "chkTranspose";
            this.chkTranspose.Size = new System.Drawing.Size(71, 17);
            this.chkTranspose.TabIndex = 4;
            this.chkTranspose.Text = "Swap x/y";
            this.chkTranspose.UseVisualStyleBackColor = true;
            this.chkTranspose.CheckedChanged += new System.EventHandler(this.chkTranspose_CheckedChanged);
            // 
            // labelUnits
            // 
            this.labelUnits.AutoSize = true;
            this.labelUnits.Location = new System.Drawing.Point(12, 24);
            this.labelUnits.Name = "labelUnits";
            this.labelUnits.Size = new System.Drawing.Size(10, 13);
            this.labelUnits.TabIndex = 5;
            this.labelUnits.Text = "-";
            // 
            // frmTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelUnits);
            this.Controls.Add(this.chkTranspose);
            this.Controls.Add(this.chkAutoResize);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtMath);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmTableEditor";
            this.Text = "Table Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTableEditor_FormClosing);
            this.Load += new System.EventHandler(this.frmTableEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtMath;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.CheckBox chkAutoResize;
        private System.Windows.Forms.CheckBox chkTranspose;
        private System.Windows.Forms.Label labelUnits;
    }
}