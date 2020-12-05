namespace UniversalPatcher
{
    partial class frmTuner
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
            this.btnImportTableSeek = new System.Windows.Forms.Button();
            this.btnImportDTC = new System.Windows.Forms.Button();
            this.btnImportXdf = new System.Windows.Forms.Button();
            this.btnSaveXML = new System.Windows.Forms.Button();
            this.btnEditTable = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.btnSaveBin = new System.Windows.Forms.Button();
            this.btnLoadXml = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearchTableSeek = new System.Windows.Forms.Button();
            this.txtSearchTableSeek = new System.Windows.Forms.TextBox();
            this.comboTableCategory = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnReadTinyTunerDB = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(2, 65);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(861, 283);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnImportTableSeek
            // 
            this.btnImportTableSeek.Location = new System.Drawing.Point(542, 35);
            this.btnImportTableSeek.Name = "btnImportTableSeek";
            this.btnImportTableSeek.Size = new System.Drawing.Size(120, 23);
            this.btnImportTableSeek.TabIndex = 1;
            this.btnImportTableSeek.Text = "Import TableSeek";
            this.btnImportTableSeek.UseVisualStyleBackColor = true;
            this.btnImportTableSeek.Click += new System.EventHandler(this.btnImportTableSeek_Click);
            // 
            // btnImportDTC
            // 
            this.btnImportDTC.Location = new System.Drawing.Point(542, 7);
            this.btnImportDTC.Name = "btnImportDTC";
            this.btnImportDTC.Size = new System.Drawing.Size(120, 23);
            this.btnImportDTC.TabIndex = 3;
            this.btnImportDTC.Text = "Import DTC";
            this.btnImportDTC.UseVisualStyleBackColor = true;
            this.btnImportDTC.Click += new System.EventHandler(this.btnImportDTC_Click);
            // 
            // btnImportXdf
            // 
            this.btnImportXdf.Location = new System.Drawing.Point(668, 7);
            this.btnImportXdf.Name = "btnImportXdf";
            this.btnImportXdf.Size = new System.Drawing.Size(120, 23);
            this.btnImportXdf.TabIndex = 4;
            this.btnImportXdf.Text = "Import XDF";
            this.btnImportXdf.UseVisualStyleBackColor = true;
            this.btnImportXdf.Click += new System.EventHandler(this.btnImportXdf_Click);
            // 
            // btnSaveXML
            // 
            this.btnSaveXML.Location = new System.Drawing.Point(136, 7);
            this.btnSaveXML.Name = "btnSaveXML";
            this.btnSaveXML.Size = new System.Drawing.Size(120, 23);
            this.btnSaveXML.TabIndex = 5;
            this.btnSaveXML.Text = "Save XML";
            this.btnSaveXML.UseVisualStyleBackColor = true;
            this.btnSaveXML.Click += new System.EventHandler(this.btnSaveXML_Click);
            // 
            // btnEditTable
            // 
            this.btnEditTable.Location = new System.Drawing.Point(12, 36);
            this.btnEditTable.Name = "btnEditTable";
            this.btnEditTable.Size = new System.Drawing.Size(120, 23);
            this.btnEditTable.TabIndex = 6;
            this.btnEditTable.Text = "Edit Table";
            this.btnEditTable.UseVisualStyleBackColor = true;
            this.btnEditTable.Click += new System.EventHandler(this.btnEditTable_Click);
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Location = new System.Drawing.Point(2, 347);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(861, 104);
            this.txtResult.TabIndex = 7;
            this.txtResult.Text = "";
            // 
            // btnSaveBin
            // 
            this.btnSaveBin.Location = new System.Drawing.Point(262, 7);
            this.btnSaveBin.Name = "btnSaveBin";
            this.btnSaveBin.Size = new System.Drawing.Size(120, 23);
            this.btnSaveBin.TabIndex = 8;
            this.btnSaveBin.Text = "Save Bin";
            this.btnSaveBin.UseVisualStyleBackColor = true;
            this.btnSaveBin.Click += new System.EventHandler(this.btnSaveBin_Click);
            // 
            // btnLoadXml
            // 
            this.btnLoadXml.Location = new System.Drawing.Point(10, 7);
            this.btnLoadXml.Name = "btnLoadXml";
            this.btnLoadXml.Size = new System.Drawing.Size(120, 23);
            this.btnLoadXml.TabIndex = 9;
            this.btnLoadXml.Text = "Load XML";
            this.btnLoadXml.UseVisualStyleBackColor = true;
            this.btnLoadXml.Click += new System.EventHandler(this.btnLoadXml_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(388, 7);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearchTableSeek
            // 
            this.btnSearchTableSeek.Location = new System.Drawing.Point(443, 36);
            this.btnSearchTableSeek.Name = "btnSearchTableSeek";
            this.btnSearchTableSeek.Size = new System.Drawing.Size(65, 22);
            this.btnSearchTableSeek.TabIndex = 15;
            this.btnSearchTableSeek.Text = "Search";
            this.btnSearchTableSeek.UseVisualStyleBackColor = true;
            this.btnSearchTableSeek.Click += new System.EventHandler(this.btnSearchTableSeek_Click);
            // 
            // txtSearchTableSeek
            // 
            this.txtSearchTableSeek.Location = new System.Drawing.Point(342, 38);
            this.txtSearchTableSeek.Name = "txtSearchTableSeek";
            this.txtSearchTableSeek.Size = new System.Drawing.Size(96, 20);
            this.txtSearchTableSeek.TabIndex = 14;
            // 
            // comboTableCategory
            // 
            this.comboTableCategory.FormattingEnabled = true;
            this.comboTableCategory.Location = new System.Drawing.Point(191, 38);
            this.comboTableCategory.Name = "comboTableCategory";
            this.comboTableCategory.Size = new System.Drawing.Size(142, 21);
            this.comboTableCategory.TabIndex = 13;
            this.comboTableCategory.SelectedIndexChanged += new System.EventHandler(this.comboTableCategory_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(135, 42);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Category:";
            // 
            // btnReadTinyTunerDB
            // 
            this.btnReadTinyTunerDB.Enabled = false;
            this.btnReadTinyTunerDB.Location = new System.Drawing.Point(668, 35);
            this.btnReadTinyTunerDB.Name = "btnReadTinyTunerDB";
            this.btnReadTinyTunerDB.Size = new System.Drawing.Size(165, 23);
            this.btnReadTinyTunerDB.TabIndex = 11;
            this.btnReadTinyTunerDB.Text = "Read TinyTuner DB (V6 only)";
            this.btnReadTinyTunerDB.UseVisualStyleBackColor = true;
            this.btnReadTinyTunerDB.Click += new System.EventHandler(this.btnReadTinyTunerDB_Click);
            // 
            // frmTuner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 450);
            this.Controls.Add(this.btnSearchTableSeek);
            this.Controls.Add(this.txtSearchTableSeek);
            this.Controls.Add(this.comboTableCategory);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btnReadTinyTunerDB);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnLoadXml);
            this.Controls.Add(this.btnSaveBin);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnEditTable);
            this.Controls.Add(this.btnSaveXML);
            this.Controls.Add(this.btnImportXdf);
            this.Controls.Add(this.btnImportDTC);
            this.Controls.Add(this.btnImportTableSeek);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmTuner";
            this.Text = "Tuner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTuner_FormClosing);
            this.Load += new System.EventHandler(this.frmTuner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnImportTableSeek;
        private System.Windows.Forms.Button btnImportDTC;
        private System.Windows.Forms.Button btnImportXdf;
        private System.Windows.Forms.Button btnSaveXML;
        private System.Windows.Forms.Button btnEditTable;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnSaveBin;
        private System.Windows.Forms.Button btnLoadXml;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSearchTableSeek;
        private System.Windows.Forms.TextBox txtSearchTableSeek;
        private System.Windows.Forms.ComboBox comboTableCategory;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btnReadTinyTunerDB;
    }
}