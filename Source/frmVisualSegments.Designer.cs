
namespace UniversalPatcher
{
    partial class frmVisualSegments
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chkShowOS = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea5.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea5);
            legend5.Name = "Legend1";
            this.chart1.Legends.Add(legend5);
            this.chart1.Location = new System.Drawing.Point(96, 34);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(536, 290);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // chkShowOS
            // 
            this.chkShowOS.AutoSize = true;
            this.chkShowOS.Checked = true;
            this.chkShowOS.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowOS.Location = new System.Drawing.Point(12, 12);
            this.chkShowOS.Name = "chkShowOS";
            this.chkShowOS.Size = new System.Drawing.Size(148, 17);
            this.chkShowOS.TabIndex = 1;
            this.chkShowOS.Text = "Show OS and Free space";
            this.chkShowOS.UseVisualStyleBackColor = true;
            this.chkShowOS.CheckedChanged += new System.EventHandler(this.chkShowOS_CheckedChanged);
            // 
            // frmVisualSegments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkShowOS);
            this.Controls.Add(this.chart1);
            this.Name = "frmVisualSegments";
            this.Text = "Visualize segments";
            this.Load += new System.EventHandler(this.frmVisualSegments_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.CheckBox chkShowOS;
    }
}