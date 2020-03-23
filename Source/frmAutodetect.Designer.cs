namespace UniversalPatcher
{
    partial class frmAutodetect
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboGroupLogic = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.listRules = new System.Windows.Forms.ListView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioData = new System.Windows.Forms.RadioButton();
            this.radioFilesize = new System.Windows.Forms.RadioButton();
            this.txtData = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboCompare = new System.Windows.Forms.ComboBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAddr = new System.Windows.Forms.Button();
            this.numGroup = new System.Windows.Forms.NumericUpDown();
            this.comboXML = new System.Windows.Forms.ComboBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnEditXML = new System.Windows.Forms.Button();
            this.btnRenameXML = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGroup)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "XML file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Rule group:";
            // 
            // comboGroupLogic
            // 
            this.comboGroupLogic.BackColor = System.Drawing.SystemColors.Window;
            this.comboGroupLogic.FormattingEnabled = true;
            this.comboGroupLogic.Items.AddRange(new object[] {
            "And",
            "Or",
            "Xor"});
            this.comboGroupLogic.Location = new System.Drawing.Point(172, 54);
            this.comboGroupLogic.Name = "comboGroupLogic";
            this.comboGroupLogic.Size = new System.Drawing.Size(57, 21);
            this.comboGroupLogic.TabIndex = 3;
            this.comboGroupLogic.Text = "And";
            this.comboGroupLogic.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboCompare_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Group logic:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(235, 5);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(57, 21);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // listRules
            // 
            this.listRules.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listRules.HideSelection = false;
            this.listRules.Location = new System.Drawing.Point(5, 193);
            this.listRules.Name = "listRules";
            this.listRules.Size = new System.Drawing.Size(379, 131);
            this.listRules.TabIndex = 20;
            this.listRules.UseCompatibleStateImageBehavior = false;
            this.listRules.SelectedIndexChanged += new System.EventHandler(this.listRules_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(298, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 28);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(298, 107);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(86, 23);
            this.btnAdd.TabIndex = 13;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioData);
            this.groupBox1.Controls.Add(this.radioFilesize);
            this.groupBox1.Location = new System.Drawing.Point(5, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 30);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // radioData
            // 
            this.radioData.AutoSize = true;
            this.radioData.Checked = true;
            this.radioData.Location = new System.Drawing.Point(8, 9);
            this.radioData.Name = "radioData";
            this.radioData.Size = new System.Drawing.Size(48, 17);
            this.radioData.TabIndex = 4;
            this.radioData.TabStop = true;
            this.radioData.Text = "Data";
            this.radioData.UseVisualStyleBackColor = true;
            // 
            // radioFilesize
            // 
            this.radioFilesize.AutoSize = true;
            this.radioFilesize.Location = new System.Drawing.Point(79, 9);
            this.radioFilesize.Name = "radioFilesize";
            this.radioFilesize.Size = new System.Drawing.Size(62, 17);
            this.radioFilesize.TabIndex = 5;
            this.radioFilesize.Text = "File size";
            this.radioFilesize.UseVisualStyleBackColor = true;
            this.radioFilesize.CheckedChanged += new System.EventHandler(this.radioFilesize_CheckedChanged);
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(94, 164);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(135, 20);
            this.txtData.TabIndex = 9;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(94, 117);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(135, 20);
            this.txtAddress.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Address:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Value (HEX):";
            // 
            // comboCompare
            // 
            this.comboCompare.FormattingEnabled = true;
            this.comboCompare.Items.AddRange(new object[] {
            "==",
            "!=",
            "<",
            ">"});
            this.comboCompare.Location = new System.Drawing.Point(172, 140);
            this.comboCompare.Name = "comboCompare";
            this.comboCompare.Size = new System.Drawing.Size(57, 21);
            this.comboCompare.TabIndex = 8;
            this.comboCompare.Text = "==";
            this.comboCompare.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboCompare_KeyPress);
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(298, 167);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(86, 23);
            this.btnDel.TabIndex = 15;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAddr
            // 
            this.btnAddr.Location = new System.Drawing.Point(233, 116);
            this.btnAddr.Name = "btnAddr";
            this.btnAddr.Size = new System.Drawing.Size(30, 20);
            this.btnAddr.TabIndex = 7;
            this.btnAddr.Text = "...";
            this.btnAddr.UseVisualStyleBackColor = true;
            this.btnAddr.Click += new System.EventHandler(this.btnAddr_Click);
            // 
            // numGroup
            // 
            this.numGroup.Location = new System.Drawing.Point(172, 30);
            this.numGroup.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numGroup.Name = "numGroup";
            this.numGroup.Size = new System.Drawing.Size(57, 20);
            this.numGroup.TabIndex = 2;
            this.numGroup.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numGroup.ValueChanged += new System.EventHandler(this.numGroup_ValueChanged);
            // 
            // comboXML
            // 
            this.comboXML.FormattingEnabled = true;
            this.comboXML.Location = new System.Drawing.Point(68, 6);
            this.comboXML.Name = "comboXML";
            this.comboXML.Size = new System.Drawing.Size(161, 21);
            this.comboXML.TabIndex = 0;
            this.comboXML.SelectedIndexChanged += new System.EventHandler(this.comboXML_SelectedIndexChanged);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(298, 136);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(86, 25);
            this.btnReplace.TabIndex = 14;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(6, 330);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(378, 54);
            this.txtStatus.TabIndex = 30;
            // 
            // btnEditXML
            // 
            this.btnEditXML.Location = new System.Drawing.Point(298, 78);
            this.btnEditXML.Name = "btnEditXML";
            this.btnEditXML.Size = new System.Drawing.Size(86, 23);
            this.btnEditXML.TabIndex = 12;
            this.btnEditXML.Text = "Edit All";
            this.btnEditXML.UseVisualStyleBackColor = true;
            this.btnEditXML.Click += new System.EventHandler(this.btnEditXML_Click);
            // 
            // btnRenameXML
            // 
            this.btnRenameXML.Location = new System.Drawing.Point(298, 37);
            this.btnRenameXML.Name = "btnRenameXML";
            this.btnRenameXML.Size = new System.Drawing.Size(86, 28);
            this.btnRenameXML.TabIndex = 11;
            this.btnRenameXML.Text = "Rename XML";
            this.btnRenameXML.UseVisualStyleBackColor = true;
            this.btnRenameXML.Click += new System.EventHandler(this.btnRenameXML_Click);
            // 
            // frmAutodetect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 387);
            this.Controls.Add(this.btnRenameXML);
            this.Controls.Add(this.btnEditXML);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.comboXML);
            this.Controls.Add(this.numGroup);
            this.Controls.Add(this.btnAddr);
            this.Controls.Add(this.btnDel);
            this.Controls.Add(this.comboCompare);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.listRules);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboGroupLogic);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmAutodetect";
            this.Text = "Autodetect settings";
            this.Load += new System.EventHandler(this.frmAutodetect_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGroup)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboGroupLogic;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ListView listRules;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioData;
        private System.Windows.Forms.RadioButton radioFilesize;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboCompare;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAddr;
        private System.Windows.Forms.NumericUpDown numGroup;
        private System.Windows.Forms.ComboBox comboXML;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnEditXML;
        private System.Windows.Forms.Button btnRenameXML;
    }
}