
namespace UniversalPatcher
{
    partial class frmTableSelector
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
            this.btnLoadXML = new System.Windows.Forms.Button();
            this.btnLoadBin = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 50);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(794, 398);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnLoadXML
            // 
            this.btnLoadXML.Location = new System.Drawing.Point(15, 12);
            this.btnLoadXML.Name = "btnLoadXML";
            this.btnLoadXML.Size = new System.Drawing.Size(75, 23);
            this.btnLoadXML.TabIndex = 1;
            this.btnLoadXML.Text = "Load XML";
            this.btnLoadXML.UseVisualStyleBackColor = true;
            this.btnLoadXML.Click += new System.EventHandler(this.btnLoadXML_Click);
            // 
            // btnLoadBin
            // 
            this.btnLoadBin.Location = new System.Drawing.Point(96, 12);
            this.btnLoadBin.Name = "btnLoadBin";
            this.btnLoadBin.Size = new System.Drawing.Size(75, 23);
            this.btnLoadBin.TabIndex = 2;
            this.btnLoadBin.Text = "Load BIN";
            this.btnLoadBin.UseVisualStyleBackColor = true;
            this.btnLoadBin.Click += new System.EventHandler(this.btnLoadBin_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(700, 13);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Select";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(188, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filter:";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(233, 16);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(106, 20);
            this.txtFilter.TabIndex = 5;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // frmTableSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnLoadBin);
            this.Controls.Add(this.btnLoadXML);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmTableSelector";
            this.Text = "Select table";
            this.Load += new System.EventHandler(this.frmTableSelector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnLoadXML;
        private System.Windows.Forms.Button btnLoadBin;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilter;
    }
}