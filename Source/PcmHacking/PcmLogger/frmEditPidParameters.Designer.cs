
namespace PcmHacking
{
    partial class frmEditPidParameters
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SelectParameterEditButton = new System.Windows.Forms.Button();
            this.labelParameterEditFIle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.parameterEditGrid = new System.Windows.Forms.DataGridView();
            this.paramId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.paramUnits = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.paramIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ApplyParameterEditButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.valueEditGrid = new System.Windows.Forms.DataGridView();
            this.conversionEditGrid = new System.Windows.Forms.DataGridView();
            this.valParameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parameterEditGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueEditGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.conversionEditGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.SelectParameterEditButton);
            this.splitContainer1.Panel1.Controls.Add(this.labelParameterEditFIle);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.parameterEditGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ApplyParameterEditButton);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 471;
            this.splitContainer1.TabIndex = 3;
            // 
            // SelectParameterEditButton
            // 
            this.SelectParameterEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectParameterEditButton.Location = new System.Drawing.Point(403, 6);
            this.SelectParameterEditButton.Name = "SelectParameterEditButton";
            this.SelectParameterEditButton.Size = new System.Drawing.Size(65, 23);
            this.SelectParameterEditButton.TabIndex = 3;
            this.SelectParameterEditButton.Text = "Select";
            this.SelectParameterEditButton.UseVisualStyleBackColor = true;
            this.SelectParameterEditButton.Click += new System.EventHandler(this.SelectParameterEditButton_Click);
            // 
            // labelParameterEditFIle
            // 
            this.labelParameterEditFIle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelParameterEditFIle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelParameterEditFIle.Location = new System.Drawing.Point(39, 9);
            this.labelParameterEditFIle.Name = "labelParameterEditFIle";
            this.labelParameterEditFIle.Size = new System.Drawing.Size(358, 20);
            this.labelParameterEditFIle.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "File:";
            // 
            // parameterEditGrid
            // 
            this.parameterEditGrid.AllowUserToAddRows = false;
            this.parameterEditGrid.AllowUserToDeleteRows = false;
            this.parameterEditGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.parameterEditGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.parameterEditGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.paramId,
            this.paramName,
            this.paramUnits,
            this.paramIndex});
            this.parameterEditGrid.Location = new System.Drawing.Point(0, 35);
            this.parameterEditGrid.Name = "parameterEditGrid";
            this.parameterEditGrid.Size = new System.Drawing.Size(471, 415);
            this.parameterEditGrid.TabIndex = 0;
            // 
            // paramId
            // 
            this.paramId.HeaderText = "Id";
            this.paramId.Name = "paramId";
            this.paramId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.paramId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.paramId.Width = 50;
            // 
            // paramName
            // 
            this.paramName.HeaderText = "Name";
            this.paramName.Name = "paramName";
            this.paramName.Width = 245;
            // 
            // paramUnits
            // 
            this.paramUnits.HeaderText = "Units";
            this.paramUnits.Name = "paramUnits";
            // 
            // paramIndex
            // 
            this.paramIndex.HeaderText = "index";
            this.paramIndex.Name = "paramIndex";
            this.paramIndex.Visible = false;
            // 
            // ApplyParameterEditButton
            // 
            this.ApplyParameterEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyParameterEditButton.Location = new System.Drawing.Point(238, 6);
            this.ApplyParameterEditButton.Name = "ApplyParameterEditButton";
            this.ApplyParameterEditButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyParameterEditButton.TabIndex = 4;
            this.ApplyParameterEditButton.Text = "Apply";
            this.ApplyParameterEditButton.UseVisualStyleBackColor = true;
            this.ApplyParameterEditButton.Click += new System.EventHandler(this.ApplyParameterEditButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 35);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.valueEditGrid);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.conversionEditGrid);
            this.splitContainer2.Size = new System.Drawing.Size(322, 412);
            this.splitContainer2.SplitterDistance = 186;
            this.splitContainer2.TabIndex = 3;
            // 
            // valueEditGrid
            // 
            this.valueEditGrid.AllowUserToAddRows = false;
            this.valueEditGrid.AllowUserToDeleteRows = false;
            this.valueEditGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.valueEditGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.valParameter,
            this.valValue});
            this.valueEditGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueEditGrid.Location = new System.Drawing.Point(0, 0);
            this.valueEditGrid.Name = "valueEditGrid";
            this.valueEditGrid.Size = new System.Drawing.Size(322, 186);
            this.valueEditGrid.TabIndex = 1;
            // 
            // conversionEditGrid
            // 
            this.conversionEditGrid.AllowUserToDeleteRows = false;
            this.conversionEditGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.conversionEditGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conversionEditGrid.Location = new System.Drawing.Point(0, 0);
            this.conversionEditGrid.Name = "conversionEditGrid";
            this.conversionEditGrid.Size = new System.Drawing.Size(322, 222);
            this.conversionEditGrid.TabIndex = 2;
            // 
            // valParameter
            // 
            this.valParameter.HeaderText = "Parameter";
            this.valParameter.Name = "valParameter";
            this.valParameter.ReadOnly = true;
            // 
            // valValue
            // 
            this.valValue.HeaderText = "Value";
            this.valValue.Name = "valValue";
            this.valValue.Width = 250;
            // 
            // frmEditPidParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frmEditPidParameters";
            this.Text = "Edit Pid Parameters * WARNING * Incmomplete software, may corrupt files when Appl" +
    "y!";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.parameterEditGrid)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.valueEditGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.conversionEditGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button SelectParameterEditButton;
        private System.Windows.Forms.Label labelParameterEditFIle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView parameterEditGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramId;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramName;
        private System.Windows.Forms.DataGridViewComboBoxColumn paramUnits;
        private System.Windows.Forms.DataGridViewTextBoxColumn paramIndex;
        private System.Windows.Forms.Button ApplyParameterEditButton;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView valueEditGrid;
        private System.Windows.Forms.DataGridView conversionEditGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn valParameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn valValue;
    }
}