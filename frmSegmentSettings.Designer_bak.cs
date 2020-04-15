namespace UniversalPatcher
{
    partial class frmSegmentSettings
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
            this.SuspendLayout();
            // 
            // frmSegmentSettings
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "frmSegmentSettings";
            this.Load += new System.EventHandler(this.frmSegmentSettings_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioWordSum;
        private System.Windows.Forms.RadioButton radioSUM;
        private System.Windows.Forms.RadioButton radioCrc32;
        private System.Windows.Forms.RadioButton radioCrc16;
        private System.Windows.Forms.TextBox txtSegmentName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSegmentAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radio2WordSum;
        private System.Windows.Forms.RadioButton radio2SUM;
        private System.Windows.Forms.RadioButton radio2Crc32;
        private System.Windows.Forms.RadioButton radio2Crc16;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCSA1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCSA2;
        private System.Windows.Forms.RadioButton radioNone;
        private System.Windows.Forms.RadioButton radio2None;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.ListView listSegments;
        private System.Windows.Forms.RadioButton radioDwordSum;
        private System.Windows.Forms.RadioButton radio2DwordSum;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioComplement2;
        private System.Windows.Forms.RadioButton radioComplement1;
        private System.Windows.Forms.RadioButton radioComplement0;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radio2Complement2;
        private System.Windows.Forms.RadioButton radio2Complement1;
        private System.Windows.Forms.RadioButton radio2Complement0;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPNAddr;
        private System.Windows.Forms.TextBox txtVerAddr;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtNrAddr;
    }
}