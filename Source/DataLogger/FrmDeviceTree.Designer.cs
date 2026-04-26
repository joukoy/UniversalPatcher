
namespace UniversalPatcher
{
    partial class FrmDeviceTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDeviceTree));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtModuleInfo = new System.Windows.Forms.TextBox();
            this.btnDTC = new System.Windows.Forms.Button();
            this.chkDTCAll = new System.Windows.Forms.CheckBox();
            this.btnQueryModules = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(1, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(357, 399);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "UniversalPatcher.ico");
            this.imageList1.Images.SetKeyName(1, "Modify.ico");
            this.imageList1.Images.SetKeyName(2, "canbus.ico");
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(794, 408);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(81, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(707, 408);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtModuleInfo
            // 
            this.txtModuleInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModuleInfo.Location = new System.Drawing.Point(364, 3);
            this.txtModuleInfo.Multiline = true;
            this.txtModuleInfo.Name = "txtModuleInfo";
            this.txtModuleInfo.Size = new System.Drawing.Size(521, 399);
            this.txtModuleInfo.TabIndex = 3;
            // 
            // btnDTC
            // 
            this.btnDTC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDTC.Location = new System.Drawing.Point(277, 408);
            this.btnDTC.Name = "btnDTC";
            this.btnDTC.Size = new System.Drawing.Size(81, 30);
            this.btnDTC.TabIndex = 4;
            this.btnDTC.Text = "Query DTC";
            this.btnDTC.UseVisualStyleBackColor = true;
            this.btnDTC.Click += new System.EventHandler(this.btnDTC_Click);
            // 
            // chkDTCAll
            // 
            this.chkDTCAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDTCAll.AutoSize = true;
            this.chkDTCAll.Location = new System.Drawing.Point(364, 416);
            this.chkDTCAll.Name = "chkDTCAll";
            this.chkDTCAll.Size = new System.Drawing.Size(77, 17);
            this.chkDTCAll.TabIndex = 5;
            this.chkDTCAll.Text = "All devices";
            this.chkDTCAll.UseVisualStyleBackColor = true;
            // 
            // btnQueryModules
            // 
            this.btnQueryModules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQueryModules.Location = new System.Drawing.Point(1, 408);
            this.btnQueryModules.Name = "btnQueryModules";
            this.btnQueryModules.Size = new System.Drawing.Size(165, 30);
            this.btnQueryModules.TabIndex = 6;
            this.btnQueryModules.Text = "Query connected devices";
            this.btnQueryModules.UseVisualStyleBackColor = true;
            this.btnQueryModules.Click += new System.EventHandler(this.btnQueryModules_Click);
            // 
            // FrmDeviceTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 450);
            this.Controls.Add(this.btnQueryModules);
            this.Controls.Add(this.chkDTCAll);
            this.Controls.Add(this.btnDTC);
            this.Controls.Add(this.txtModuleInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.treeView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmDeviceTree";
            this.Text = "Select Device";
            this.Load += new System.EventHandler(this.FrmDeviceTree_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox txtModuleInfo;
        private System.Windows.Forms.Button btnDTC;
        private System.Windows.Forms.CheckBox chkDTCAll;
        private System.Windows.Forms.Button btnQueryModules;
    }
}