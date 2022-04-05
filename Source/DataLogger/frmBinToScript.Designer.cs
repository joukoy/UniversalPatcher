
namespace UniversalPatcher
{
    partial class frmBinToScript
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
            this.txtBinFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtScriptFile = new System.Windows.Forms.TextBox();
            this.btnBrowseBin = new System.Windows.Forms.Button();
            this.btnBrowseScript = new System.Windows.Forms.Button();
            this.chkStaticAddr = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTargetAddr = new System.Windows.Forms.TextBox();
            this.chkExecute = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMsgLen = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPriority = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtParseRange = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAfterMsg = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "BIN File:";
            // 
            // txtBinFile
            // 
            this.txtBinFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBinFile.Location = new System.Drawing.Point(109, 9);
            this.txtBinFile.Name = "txtBinFile";
            this.txtBinFile.Size = new System.Drawing.Size(470, 20);
            this.txtBinFile.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Script File:";
            // 
            // txtScriptFile
            // 
            this.txtScriptFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptFile.Location = new System.Drawing.Point(108, 35);
            this.txtScriptFile.Name = "txtScriptFile";
            this.txtScriptFile.Size = new System.Drawing.Size(471, 20);
            this.txtScriptFile.TabIndex = 3;
            // 
            // btnBrowseBin
            // 
            this.btnBrowseBin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseBin.Location = new System.Drawing.Point(585, 6);
            this.btnBrowseBin.Name = "btnBrowseBin";
            this.btnBrowseBin.Size = new System.Drawing.Size(38, 23);
            this.btnBrowseBin.TabIndex = 4;
            this.btnBrowseBin.Text = "...";
            this.btnBrowseBin.UseVisualStyleBackColor = true;
            this.btnBrowseBin.Click += new System.EventHandler(this.btnBrowseBin_Click);
            // 
            // btnBrowseScript
            // 
            this.btnBrowseScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseScript.Location = new System.Drawing.Point(585, 32);
            this.btnBrowseScript.Name = "btnBrowseScript";
            this.btnBrowseScript.Size = new System.Drawing.Size(38, 23);
            this.btnBrowseScript.TabIndex = 5;
            this.btnBrowseScript.Text = "...";
            this.btnBrowseScript.UseVisualStyleBackColor = true;
            this.btnBrowseScript.Click += new System.EventHandler(this.btnBrowseScript_Click);
            // 
            // chkStaticAddr
            // 
            this.chkStaticAddr.AutoSize = true;
            this.chkStaticAddr.Location = new System.Drawing.Point(197, 63);
            this.chkStaticAddr.Name = "chkStaticAddr";
            this.chkStaticAddr.Size = new System.Drawing.Size(93, 17);
            this.chkStaticAddr.TabIndex = 6;
            this.chkStaticAddr.Text = "Static address";
            this.chkStaticAddr.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Target Address:";
            // 
            // txtTargetAddr
            // 
            this.txtTargetAddr.Location = new System.Drawing.Point(108, 60);
            this.txtTargetAddr.Name = "txtTargetAddr";
            this.txtTargetAddr.Size = new System.Drawing.Size(80, 20);
            this.txtTargetAddr.TabIndex = 8;
            this.txtTargetAddr.Text = "0";
            // 
            // chkExecute
            // 
            this.chkExecute.AutoSize = true;
            this.chkExecute.Location = new System.Drawing.Point(296, 63);
            this.chkExecute.Name = "chkExecute";
            this.chkExecute.Size = new System.Drawing.Size(65, 17);
            this.chkExecute.TabIndex = 9;
            this.chkExecute.Text = "Execute";
            this.chkExecute.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "ID:";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(108, 85);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(79, 20);
            this.txtID.TabIndex = 11;
            this.txtID.Text = "10";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(548, 186);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 12;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Message Length:";
            // 
            // txtMsgLen
            // 
            this.txtMsgLen.Location = new System.Drawing.Point(108, 111);
            this.txtMsgLen.Name = "txtMsgLen";
            this.txtMsgLen.Size = new System.Drawing.Size(80, 20);
            this.txtMsgLen.TabIndex = 14;
            this.txtMsgLen.Text = "FF";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(194, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Priority:";
            // 
            // txtPriority
            // 
            this.txtPriority.Location = new System.Drawing.Point(241, 85);
            this.txtPriority.Name = "txtPriority";
            this.txtPriority.Size = new System.Drawing.Size(78, 20);
            this.txtPriority.TabIndex = 16;
            this.txtPriority.Text = "6C";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Parse range:";
            // 
            // txtParseRange
            // 
            this.txtParseRange.Location = new System.Drawing.Point(108, 137);
            this.txtParseRange.Name = "txtParseRange";
            this.txtParseRange.Size = new System.Drawing.Size(210, 20);
            this.txtParseRange.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(327, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "(Optional)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Message suffix:";
            // 
            // txtSuffix
            // 
            this.txtSuffix.Location = new System.Drawing.Point(108, 164);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(78, 20);
            this.txtSuffix.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(204, 168);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(238, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Example: :2:500 (added after each message row)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 196);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "After each msg:";
            // 
            // txtAfterMsg
            // 
            this.txtAfterMsg.Location = new System.Drawing.Point(109, 196);
            this.txtAfterMsg.Name = "txtAfterMsg";
            this.txtAfterMsg.Size = new System.Drawing.Size(208, 20);
            this.txtAfterMsg.TabIndex = 24;
            // 
            // frmBinToScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 228);
            this.Controls.Add(this.txtAfterMsg);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtSuffix);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtParseRange);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtPriority);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtMsgLen);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkExecute);
            this.Controls.Add(this.txtTargetAddr);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkStaticAddr);
            this.Controls.Add(this.btnBrowseScript);
            this.Controls.Add(this.btnBrowseBin);
            this.Controls.Add(this.txtScriptFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBinFile);
            this.Controls.Add(this.label1);
            this.Name = "frmBinToScript";
            this.Text = "Parse Bin To Script";
            this.Load += new System.EventHandler(this.frmBinToScript_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBinFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtScriptFile;
        private System.Windows.Forms.Button btnBrowseBin;
        private System.Windows.Forms.Button btnBrowseScript;
        private System.Windows.Forms.CheckBox chkStaticAddr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTargetAddr;
        private System.Windows.Forms.CheckBox chkExecute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMsgLen;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPriority;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtParseRange;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSuffix;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAfterMsg;
    }
}