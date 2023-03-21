
namespace UniversalPatcher
{
    partial class frmJ2534Filters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmJ2534Filters));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMask = new System.Windows.Forms.TextBox();
            this.comboFType = new System.Windows.Forms.ComboBox();
            this.txtPattern = new System.Windows.Forms.TextBox();
            this.txtFlow = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.contextMenuStripSavedFilters = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboPresets = new System.Windows.Forms.ComboBox();
            this.btnSavedCombosMenu = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.comboFilters = new System.Windows.Forms.ComboBox();
            this.btnFiltersMenu = new System.Windows.Forms.Button();
            this.contextMenuStripFilterCombos = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMaskRx = new System.Windows.Forms.Button();
            this.btnMaskTx = new System.Windows.Forms.Button();
            this.btnPatternTx = new System.Windows.Forms.Button();
            this.btnPatternRx = new System.Windows.Forms.Button();
            this.btnFlowTx = new System.Windows.Forms.Button();
            this.btnFlowRx = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDeleteFromFinalFilter = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStripSavedFilters.SuspendLayout();
            this.contextMenuStripFilterCombos.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mask:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Pattern:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "FlowControl:";
            // 
            // txtMask
            // 
            this.txtMask.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMask.Location = new System.Drawing.Point(108, 82);
            this.txtMask.Name = "txtMask";
            this.txtMask.Size = new System.Drawing.Size(139, 23);
            this.txtMask.TabIndex = 9;
            this.txtMask.TextChanged += new System.EventHandler(this.txtMask_TextChanged);
            // 
            // comboFType
            // 
            this.comboFType.FormattingEnabled = true;
            this.comboFType.Location = new System.Drawing.Point(108, 57);
            this.comboFType.Name = "comboFType";
            this.comboFType.Size = new System.Drawing.Size(139, 21);
            this.comboFType.TabIndex = 10;
            // 
            // txtPattern
            // 
            this.txtPattern.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPattern.Location = new System.Drawing.Point(108, 108);
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Size = new System.Drawing.Size(139, 23);
            this.txtPattern.TabIndex = 15;
            this.txtPattern.TextChanged += new System.EventHandler(this.txtPattern_TextChanged);
            // 
            // txtFlow
            // 
            this.txtFlow.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFlow.Location = new System.Drawing.Point(108, 134);
            this.txtFlow.Name = "txtFlow";
            this.txtFlow.Size = new System.Drawing.Size(139, 23);
            this.txtFlow.TabIndex = 20;
            this.txtFlow.TextChanged += new System.EventHandler(this.txtFlow_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(793, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(66, 23);
            this.btnOK.TabIndex = 30;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "Filter name:";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(353, 19);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 33;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // contextMenuStripSavedFilters
            // 
            this.contextMenuStripSavedFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem1,
            this.deleteToolStripMenuItem1,
            this.resetToolStripMenuItem});
            this.contextMenuStripSavedFilters.Name = "contextMenuStrip1";
            this.contextMenuStripSavedFilters.Size = new System.Drawing.Size(108, 70);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // comboPresets
            // 
            this.comboPresets.FormattingEnabled = true;
            this.comboPresets.Location = new System.Drawing.Point(108, 31);
            this.comboPresets.Name = "comboPresets";
            this.comboPresets.Size = new System.Drawing.Size(217, 21);
            this.comboPresets.TabIndex = 37;
            // 
            // btnSavedCombosMenu
            // 
            this.btnSavedCombosMenu.Location = new System.Drawing.Point(331, 29);
            this.btnSavedCombosMenu.Name = "btnSavedCombosMenu";
            this.btnSavedCombosMenu.Size = new System.Drawing.Size(26, 23);
            this.btnSavedCombosMenu.TabIndex = 38;
            this.btnSavedCombosMenu.Text = "...";
            this.btnSavedCombosMenu.UseVisualStyleBackColor = true;
            this.btnSavedCombosMenu.Click += new System.EventHandler(this.btnPresetsMenu_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(24, 34);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(69, 13);
            this.label13.TabIndex = 39;
            this.label13.Text = "Preset name:";
            // 
            // comboFilters
            // 
            this.comboFilters.FormattingEnabled = true;
            this.comboFilters.Location = new System.Drawing.Point(77, 19);
            this.comboFilters.Name = "comboFilters";
            this.comboFilters.Size = new System.Drawing.Size(217, 21);
            this.comboFilters.TabIndex = 40;
            this.comboFilters.SelectedIndexChanged += new System.EventHandler(this.comboFilterCombos_SelectedIndexChanged);
            // 
            // btnFiltersMenu
            // 
            this.btnFiltersMenu.Location = new System.Drawing.Point(300, 19);
            this.btnFiltersMenu.Name = "btnFiltersMenu";
            this.btnFiltersMenu.Size = new System.Drawing.Size(26, 23);
            this.btnFiltersMenu.TabIndex = 41;
            this.btnFiltersMenu.Text = "...";
            this.btnFiltersMenu.UseVisualStyleBackColor = true;
            this.btnFiltersMenu.Click += new System.EventHandler(this.btnFiltersMenu_Click);
            // 
            // contextMenuStripFilterCombos
            // 
            this.contextMenuStripFilterCombos.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem2,
            this.deleteToolStripMenuItem2});
            this.contextMenuStripFilterCombos.Name = "contextMenuStripFilterCombos";
            this.contextMenuStripFilterCombos.Size = new System.Drawing.Size(108, 48);
            // 
            // saveToolStripMenuItem2
            // 
            this.saveToolStripMenuItem2.Name = "saveToolStripMenuItem2";
            this.saveToolStripMenuItem2.Size = new System.Drawing.Size(107, 22);
            this.saveToolStripMenuItem2.Text = "Save";
            this.saveToolStripMenuItem2.Click += new System.EventHandler(this.saveToolStripMenuItem2_Click);
            // 
            // deleteToolStripMenuItem2
            // 
            this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
            this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem2.Text = "Delete";
            this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.deleteToolStripMenuItem2_Click);
            // 
            // btnMaskRx
            // 
            this.btnMaskRx.Location = new System.Drawing.Point(253, 81);
            this.btnMaskRx.Name = "btnMaskRx";
            this.btnMaskRx.Size = new System.Drawing.Size(300, 21);
            this.btnMaskRx.TabIndex = 43;
            this.btnMaskRx.Text = "RxStatus";
            this.btnMaskRx.UseVisualStyleBackColor = true;
            this.btnMaskRx.Click += new System.EventHandler(this.btnMaskRxPopup_Click);
            // 
            // btnMaskTx
            // 
            this.btnMaskTx.Location = new System.Drawing.Point(559, 82);
            this.btnMaskTx.Name = "btnMaskTx";
            this.btnMaskTx.Size = new System.Drawing.Size(300, 21);
            this.btnMaskTx.TabIndex = 44;
            this.btnMaskTx.Text = "TxFlags";
            this.btnMaskTx.UseVisualStyleBackColor = true;
            this.btnMaskTx.Click += new System.EventHandler(this.btnMaskTx_Click);
            // 
            // btnPatternTx
            // 
            this.btnPatternTx.Location = new System.Drawing.Point(559, 110);
            this.btnPatternTx.Name = "btnPatternTx";
            this.btnPatternTx.Size = new System.Drawing.Size(300, 21);
            this.btnPatternTx.TabIndex = 46;
            this.btnPatternTx.Text = "TxFlags";
            this.btnPatternTx.UseVisualStyleBackColor = true;
            this.btnPatternTx.Click += new System.EventHandler(this.btnPatternTx_Click);
            // 
            // btnPatternRx
            // 
            this.btnPatternRx.Location = new System.Drawing.Point(253, 109);
            this.btnPatternRx.Name = "btnPatternRx";
            this.btnPatternRx.Size = new System.Drawing.Size(300, 21);
            this.btnPatternRx.TabIndex = 45;
            this.btnPatternRx.Text = "RxStatus";
            this.btnPatternRx.UseVisualStyleBackColor = true;
            this.btnPatternRx.Click += new System.EventHandler(this.btnPatternRx_Click);
            // 
            // btnFlowTx
            // 
            this.btnFlowTx.Location = new System.Drawing.Point(559, 136);
            this.btnFlowTx.Name = "btnFlowTx";
            this.btnFlowTx.Size = new System.Drawing.Size(300, 21);
            this.btnFlowTx.TabIndex = 48;
            this.btnFlowTx.Text = "TxFlags";
            this.btnFlowTx.UseVisualStyleBackColor = true;
            this.btnFlowTx.Click += new System.EventHandler(this.btnFlowTx_Click);
            // 
            // btnFlowRx
            // 
            this.btnFlowRx.Location = new System.Drawing.Point(253, 135);
            this.btnFlowRx.Name = "btnFlowRx";
            this.btnFlowRx.Size = new System.Drawing.Size(300, 21);
            this.btnFlowRx.TabIndex = 47;
            this.btnFlowRx.Text = "RxStatus";
            this.btnFlowRx.UseVisualStyleBackColor = true;
            this.btnFlowRx.Click += new System.EventHandler(this.btnFlowRx_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.comboFType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnFlowTx);
            this.groupBox1.Controls.Add(this.txtMask);
            this.groupBox1.Controls.Add(this.btnFlowRx);
            this.groupBox1.Controls.Add(this.txtPattern);
            this.groupBox1.Controls.Add(this.btnPatternTx);
            this.groupBox1.Controls.Add(this.txtFlow);
            this.groupBox1.Controls.Add(this.btnPatternRx);
            this.groupBox1.Controls.Add(this.btnMaskTx);
            this.groupBox1.Controls.Add(this.btnMaskRx);
            this.groupBox1.Controls.Add(this.comboPresets);
            this.groupBox1.Controls.Add(this.btnSavedCombosMenu);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(879, 176);
            this.groupBox1.TabIndex = 51;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preset";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox2.Controls.Add(this.btnDeleteFromFinalFilter);
            this.GroupBox2.Controls.Add(this.btnAddNew);
            this.GroupBox2.Controls.Add(this.dataGridView1);
            this.GroupBox2.Controls.Add(this.comboFilters);
            this.GroupBox2.Controls.Add(this.btnFiltersMenu);
            this.GroupBox2.Controls.Add(this.btnClear);
            this.GroupBox2.Controls.Add(this.label11);
            this.GroupBox2.Controls.Add(this.btnOK);
            this.GroupBox2.Location = new System.Drawing.Point(2, 184);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(878, 314);
            this.GroupBox2.TabIndex = 52;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Filter";
            this.GroupBox2.Enter += new System.EventHandler(this.GroupBox2_Enter);
            // 
            // btnDeleteFromFinalFilter
            // 
            this.btnDeleteFromFinalFilter.Location = new System.Drawing.Point(515, 19);
            this.btnDeleteFromFinalFilter.Name = "btnDeleteFromFinalFilter";
            this.btnDeleteFromFinalFilter.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFromFinalFilter.TabIndex = 52;
            this.btnDeleteFromFinalFilter.Text = "Delete";
            this.btnDeleteFromFinalFilter.UseVisualStyleBackColor = true;
            this.btnDeleteFromFinalFilter.Click += new System.EventHandler(this.btnDeleteFromFinalFilter_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Location = new System.Drawing.Point(434, 19);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(75, 23);
            this.btnAddNew.TabIndex = 51;
            this.btnAddNew.Text = "Add";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 54);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(875, 259);
            this.dataGridView1.TabIndex = 50;
            // 
            // frmJ2534Filters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 500);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmJ2534Filters";
            this.Text = "J2534 Filter Editor";
            this.Load += new System.EventHandler(this.frmJ2534Filters_Load);
            this.contextMenuStripSavedFilters.ResumeLayout(false);
            this.contextMenuStripFilterCombos.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMask;
        private System.Windows.Forms.ComboBox comboFType;
        private System.Windows.Forms.TextBox txtPattern;
        private System.Windows.Forms.TextBox txtFlow;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSavedFilters;
        private System.Windows.Forms.ComboBox comboPresets;
        private System.Windows.Forms.Button btnSavedCombosMenu;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboFilters;
        private System.Windows.Forms.Button btnFiltersMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFilterCombos;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem2;
        private System.Windows.Forms.Button btnMaskRx;
        private System.Windows.Forms.Button btnMaskTx;
        private System.Windows.Forms.Button btnPatternTx;
        private System.Windows.Forms.Button btnPatternRx;
        private System.Windows.Forms.Button btnFlowTx;
        private System.Windows.Forms.Button btnFlowRx;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox GroupBox2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnDeleteFromFinalFilter;
    }
}