namespace UniversalPatcher
{
    partial class frmDbEditor
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
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tinyTunerDataSet = new UniversalPatcher.TinyTunerDataSet();
            this.tableDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableDataTableAdapter = new UniversalPatcher.TinyTunerDataSetTableAdapters.TableDataTableAdapter();
            this.indexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mapNumberDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.startPositionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.elementSizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.factorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.displayWholeNumbersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allowNegativeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnHeadersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rowCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rowHeadersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableDescriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainCategoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subCategoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oKtoExportDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateAddedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastModifiedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tinyTunerDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.indexDataGridViewTextBoxColumn,
            this.mapNumberDataGridViewTextBoxColumn,
            this.tableNameDataGridViewTextBoxColumn,
            this.startPositionDataGridViewTextBoxColumn,
            this.elementSizeDataGridViewTextBoxColumn,
            this.factorDataGridViewTextBoxColumn,
            this.unitsDataGridViewTextBoxColumn,
            this.displayWholeNumbersDataGridViewTextBoxColumn,
            this.allowNegativeDataGridViewTextBoxColumn,
            this.columnCountDataGridViewTextBoxColumn,
            this.columnHeadersDataGridViewTextBoxColumn,
            this.rowCountDataGridViewTextBoxColumn,
            this.rowHeadersDataGridViewTextBoxColumn,
            this.tableDescriptionDataGridViewTextBoxColumn,
            this.mainCategoryDataGridViewTextBoxColumn,
            this.subCategoryDataGridViewTextBoxColumn,
            this.oKtoExportDataGridViewTextBoxColumn,
            this.dateAddedDataGridViewTextBoxColumn,
            this.lastModifiedDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.tableDataBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(4, 33);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(793, 415);
            this.dataGridView1.TabIndex = 0;
            // 
            // tinyTunerDataSet
            // 
            this.tinyTunerDataSet.DataSetName = "TinyTunerDataSet";
            this.tinyTunerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tableDataBindingSource
            // 
            this.tableDataBindingSource.DataMember = "TableData";
            this.tableDataBindingSource.DataSource = this.tinyTunerDataSet;
            // 
            // tableDataTableAdapter
            // 
            this.tableDataTableAdapter.ClearBeforeFill = true;
            // 
            // indexDataGridViewTextBoxColumn
            // 
            this.indexDataGridViewTextBoxColumn.DataPropertyName = "Index";
            this.indexDataGridViewTextBoxColumn.HeaderText = "Index";
            this.indexDataGridViewTextBoxColumn.Name = "indexDataGridViewTextBoxColumn";
            // 
            // mapNumberDataGridViewTextBoxColumn
            // 
            this.mapNumberDataGridViewTextBoxColumn.DataPropertyName = "MapNumber";
            this.mapNumberDataGridViewTextBoxColumn.HeaderText = "MapNumber";
            this.mapNumberDataGridViewTextBoxColumn.Name = "mapNumberDataGridViewTextBoxColumn";
            // 
            // tableNameDataGridViewTextBoxColumn
            // 
            this.tableNameDataGridViewTextBoxColumn.DataPropertyName = "TableName";
            this.tableNameDataGridViewTextBoxColumn.HeaderText = "TableName";
            this.tableNameDataGridViewTextBoxColumn.Name = "tableNameDataGridViewTextBoxColumn";
            // 
            // startPositionDataGridViewTextBoxColumn
            // 
            this.startPositionDataGridViewTextBoxColumn.DataPropertyName = "StartPosition";
            this.startPositionDataGridViewTextBoxColumn.HeaderText = "StartPosition";
            this.startPositionDataGridViewTextBoxColumn.Name = "startPositionDataGridViewTextBoxColumn";
            // 
            // elementSizeDataGridViewTextBoxColumn
            // 
            this.elementSizeDataGridViewTextBoxColumn.DataPropertyName = "ElementSize";
            this.elementSizeDataGridViewTextBoxColumn.HeaderText = "ElementSize";
            this.elementSizeDataGridViewTextBoxColumn.Name = "elementSizeDataGridViewTextBoxColumn";
            // 
            // factorDataGridViewTextBoxColumn
            // 
            this.factorDataGridViewTextBoxColumn.DataPropertyName = "Factor";
            this.factorDataGridViewTextBoxColumn.HeaderText = "Factor";
            this.factorDataGridViewTextBoxColumn.Name = "factorDataGridViewTextBoxColumn";
            // 
            // unitsDataGridViewTextBoxColumn
            // 
            this.unitsDataGridViewTextBoxColumn.DataPropertyName = "Units";
            this.unitsDataGridViewTextBoxColumn.HeaderText = "Units";
            this.unitsDataGridViewTextBoxColumn.Name = "unitsDataGridViewTextBoxColumn";
            // 
            // displayWholeNumbersDataGridViewTextBoxColumn
            // 
            this.displayWholeNumbersDataGridViewTextBoxColumn.DataPropertyName = "DisplayWholeNumbers";
            this.displayWholeNumbersDataGridViewTextBoxColumn.HeaderText = "DisplayWholeNumbers";
            this.displayWholeNumbersDataGridViewTextBoxColumn.Name = "displayWholeNumbersDataGridViewTextBoxColumn";
            // 
            // allowNegativeDataGridViewTextBoxColumn
            // 
            this.allowNegativeDataGridViewTextBoxColumn.DataPropertyName = "AllowNegative";
            this.allowNegativeDataGridViewTextBoxColumn.HeaderText = "AllowNegative";
            this.allowNegativeDataGridViewTextBoxColumn.Name = "allowNegativeDataGridViewTextBoxColumn";
            // 
            // columnCountDataGridViewTextBoxColumn
            // 
            this.columnCountDataGridViewTextBoxColumn.DataPropertyName = "ColumnCount";
            this.columnCountDataGridViewTextBoxColumn.HeaderText = "ColumnCount";
            this.columnCountDataGridViewTextBoxColumn.Name = "columnCountDataGridViewTextBoxColumn";
            // 
            // columnHeadersDataGridViewTextBoxColumn
            // 
            this.columnHeadersDataGridViewTextBoxColumn.DataPropertyName = "ColumnHeaders";
            this.columnHeadersDataGridViewTextBoxColumn.HeaderText = "ColumnHeaders";
            this.columnHeadersDataGridViewTextBoxColumn.Name = "columnHeadersDataGridViewTextBoxColumn";
            // 
            // rowCountDataGridViewTextBoxColumn
            // 
            this.rowCountDataGridViewTextBoxColumn.DataPropertyName = "RowCount";
            this.rowCountDataGridViewTextBoxColumn.HeaderText = "RowCount";
            this.rowCountDataGridViewTextBoxColumn.Name = "rowCountDataGridViewTextBoxColumn";
            // 
            // rowHeadersDataGridViewTextBoxColumn
            // 
            this.rowHeadersDataGridViewTextBoxColumn.DataPropertyName = "RowHeaders";
            this.rowHeadersDataGridViewTextBoxColumn.HeaderText = "RowHeaders";
            this.rowHeadersDataGridViewTextBoxColumn.Name = "rowHeadersDataGridViewTextBoxColumn";
            // 
            // tableDescriptionDataGridViewTextBoxColumn
            // 
            this.tableDescriptionDataGridViewTextBoxColumn.DataPropertyName = "TableDescription";
            this.tableDescriptionDataGridViewTextBoxColumn.HeaderText = "TableDescription";
            this.tableDescriptionDataGridViewTextBoxColumn.Name = "tableDescriptionDataGridViewTextBoxColumn";
            // 
            // mainCategoryDataGridViewTextBoxColumn
            // 
            this.mainCategoryDataGridViewTextBoxColumn.DataPropertyName = "MainCategory";
            this.mainCategoryDataGridViewTextBoxColumn.HeaderText = "MainCategory";
            this.mainCategoryDataGridViewTextBoxColumn.Name = "mainCategoryDataGridViewTextBoxColumn";
            // 
            // subCategoryDataGridViewTextBoxColumn
            // 
            this.subCategoryDataGridViewTextBoxColumn.DataPropertyName = "SubCategory";
            this.subCategoryDataGridViewTextBoxColumn.HeaderText = "SubCategory";
            this.subCategoryDataGridViewTextBoxColumn.Name = "subCategoryDataGridViewTextBoxColumn";
            // 
            // oKtoExportDataGridViewTextBoxColumn
            // 
            this.oKtoExportDataGridViewTextBoxColumn.DataPropertyName = "OKtoExport";
            this.oKtoExportDataGridViewTextBoxColumn.HeaderText = "OKtoExport";
            this.oKtoExportDataGridViewTextBoxColumn.Name = "oKtoExportDataGridViewTextBoxColumn";
            // 
            // dateAddedDataGridViewTextBoxColumn
            // 
            this.dateAddedDataGridViewTextBoxColumn.DataPropertyName = "DateAdded";
            this.dateAddedDataGridViewTextBoxColumn.HeaderText = "DateAdded";
            this.dateAddedDataGridViewTextBoxColumn.Name = "dateAddedDataGridViewTextBoxColumn";
            // 
            // lastModifiedDataGridViewTextBoxColumn
            // 
            this.lastModifiedDataGridViewTextBoxColumn.DataPropertyName = "LastModified";
            this.lastModifiedDataGridViewTextBoxColumn.HeaderText = "LastModified";
            this.lastModifiedDataGridViewTextBoxColumn.Name = "lastModifiedDataGridViewTextBoxColumn";
            // 
            // frmDbEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmDbEditor";
            this.Text = "DB Editor";
            this.Load += new System.EventHandler(this.frmDbEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tinyTunerDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDataBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private TinyTunerDataSet tinyTunerDataSet;
        private System.Windows.Forms.BindingSource tableDataBindingSource;
        private TinyTunerDataSetTableAdapters.TableDataTableAdapter tableDataTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn indexDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mapNumberDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tableNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn startPositionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn elementSizeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn factorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn displayWholeNumbersDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn allowNegativeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnHeadersDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rowCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rowHeadersDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tableDescriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mainCategoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn subCategoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn oKtoExportDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateAddedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastModifiedDataGridViewTextBoxColumn;
    }
}