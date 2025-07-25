
namespace UniversalPatcher
{
    partial class frmSignalView
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
            this.dataGridBitView = new System.Windows.Forms.DataGridView();
            this.dataGridCanData = new System.Windows.Forms.DataGridView();
            this.dataGridMask = new System.Windows.Forms.DataGridView();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBitView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCanData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMask)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridBitView
            // 
            this.dataGridBitView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridBitView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridBitView.Location = new System.Drawing.Point(12, 35);
            this.dataGridBitView.Name = "dataGridBitView";
            this.dataGridBitView.Size = new System.Drawing.Size(262, 382);
            this.dataGridBitView.TabIndex = 0;
            // 
            // dataGridCanData
            // 
            this.dataGridCanData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridCanData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCanData.Location = new System.Drawing.Point(280, 35);
            this.dataGridCanData.Name = "dataGridCanData";
            this.dataGridCanData.Size = new System.Drawing.Size(524, 382);
            this.dataGridCanData.TabIndex = 1;
            // 
            // dataGridMask
            // 
            this.dataGridMask.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridMask.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridMask.Location = new System.Drawing.Point(810, 35);
            this.dataGridMask.Name = "dataGridMask";
            this.dataGridMask.Size = new System.Drawing.Size(262, 382);
            this.dataGridMask.TabIndex = 2;
            // 
            // txtValue
            // 
            this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtValue.Location = new System.Drawing.Point(13, 428);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(1059, 20);
            this.txtValue.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(328, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "CAN frame";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(581, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Bit data values";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(833, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Signal mask (BIN)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Editor bit indices";
            // 
            // frmSignalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 452);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.dataGridMask);
            this.Controls.Add(this.dataGridCanData);
            this.Controls.Add(this.dataGridBitView);
            this.Name = "frmSignalView";
            this.Text = "Signal View";
            this.Load += new System.EventHandler(this.frmSignalView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridBitView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCanData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMask)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridBitView;
        private System.Windows.Forms.DataGridView dataGridCanData;
        private System.Windows.Forms.DataGridView dataGridMask;
        public System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}