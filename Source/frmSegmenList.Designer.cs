namespace UniversalPatcher
{
    partial class frmSegmenList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSegmenList));
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.listSegments = new System.Windows.Forms.ListView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.labelXML = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNewXML = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.btnSaveOnly = new System.Windows.Forms.Button();
            this.btnEditXML = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(470, 70);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(71, 26);
            this.btnMoveDown.TabIndex = 2;
            this.btnMoveDown.Text = "Move down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(470, 38);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(71, 26);
            this.btnMoveUp.TabIndex = 1;
            this.btnMoveUp.Text = "Move up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // listSegments
            // 
            this.listSegments.HideSelection = false;
            this.listSegments.Location = new System.Drawing.Point(3, 38);
            this.listSegments.Name = "listSegments";
            this.listSegments.Size = new System.Drawing.Size(461, 281);
            this.listSegments.TabIndex = 0;
            this.listSegments.UseCompatibleStateImageBehavior = false;
            this.listSegments.SelectedIndexChanged += new System.EventHandler(this.listSegments_SelectedIndexChanged);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(470, 228);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(71, 27);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(470, 195);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(70, 27);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(470, 162);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(71, 27);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(163, 325);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(71, 27);
            this.btnOpen.TabIndex = 9;
            this.btnOpen.Text = "Open...";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // labelXML
            // 
            this.labelXML.AutoSize = true;
            this.labelXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXML.Location = new System.Drawing.Point(19, 10);
            this.labelXML.Name = "labelXML";
            this.labelXML.Size = new System.Drawing.Size(13, 16);
            this.labelXML.TabIndex = 49;
            this.labelXML.Text = "-";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(86, 325);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(71, 27);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Save As...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNewXML
            // 
            this.btnNewXML.Location = new System.Drawing.Point(9, 325);
            this.btnNewXML.Name = "btnNewXML";
            this.btnNewXML.Size = new System.Drawing.Size(71, 27);
            this.btnNewXML.TabIndex = 7;
            this.btnNewXML.Text = "New XML";
            this.btnNewXML.UseVisualStyleBackColor = true;
            this.btnNewXML.Click += new System.EventHandler(this.btnNewXML_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(470, 130);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(70, 26);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(469, 319);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(71, 39);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "Save and Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(419, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 50;
            this.label1.Text = "Version:";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(470, 9);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(65, 20);
            this.txtVersion.TabIndex = 51;
            this.txtVersion.Text = "1";
            // 
            // btnSaveOnly
            // 
            this.btnSaveOnly.Location = new System.Drawing.Point(469, 287);
            this.btnSaveOnly.Name = "btnSaveOnly";
            this.btnSaveOnly.Size = new System.Drawing.Size(72, 31);
            this.btnSaveOnly.TabIndex = 52;
            this.btnSaveOnly.Text = "Save";
            this.btnSaveOnly.UseVisualStyleBackColor = true;
            this.btnSaveOnly.Click += new System.EventHandler(this.btnSaveOnly_Click);
            // 
            // btnEditXML
            // 
            this.btnEditXML.Location = new System.Drawing.Point(321, 325);
            this.btnEditXML.Name = "btnEditXML";
            this.btnEditXML.Size = new System.Drawing.Size(71, 27);
            this.btnEditXML.TabIndex = 53;
            this.btnEditXML.Text = "Edit XML";
            this.btnEditXML.UseVisualStyleBackColor = true;
            this.btnEditXML.Click += new System.EventHandler(this.btnEditXML_Click);
            // 
            // frmSegmenList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 362);
            this.Controls.Add(this.btnEditXML);
            this.Controls.Add(this.btnSaveOnly);
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnNewXML);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.labelXML);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.listSegments);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSegmenList";
            this.Text = "Segments";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        public System.Windows.Forms.ListView listSegments;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label labelXML;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnNewXML;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.Button btnSaveOnly;
        private System.Windows.Forms.Button btnEditXML;
    }
}