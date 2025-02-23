using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmDashboard : Form
    {

        public frmDashboard()
        {
            InitializeComponent();
        }
        public List<UpGauge> Gauges = new List<UpGauge>();
        private bool modified = false;
        private void frmGauges_Load(object sender, EventArgs e)
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel1.Location = new Point(12, 58);
            tableLayoutPanel1.Size = new Size(776, 380);
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(tableLayoutPanel1);
            if (AppSettings.MainWindowPersistence)
            {
                if (AppSettings.DashboardWindowSize.Width > 0 || AppSettings.DashboardWindowSize.Height > 0)
                {
                    this.WindowState = AppSettings.DashboardWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = AppSettings.DashboardWindowLocation;
                    this.Size = AppSettings.DashboardWindowSize;
                }
            }
            this.Resize += FrmDashboard_Resize;
            this.FormClosing += FrmDashboard_FormClosing;
            numRows.Value = AppSettings.DashboardRows;
            numColumns.Value = AppSettings.DashboardCols;
            if (!string.IsNullOrEmpty(AppSettings.DashboardLastFile))
            {
                LoadXML(AppSettings.DashboardLastFile);
            }
            LoadSettings();
        }

        private void FrmDashboard_Resize(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadXML(string FileName)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    Debug.WriteLine(FileName + " not exist, skip");
                    return;
                }
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<UpGauge>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                Gauges = (List<UpGauge>)reader.Deserialize(file);
                file.Close();
                AppSettings.DashboardLastFile = FileName;
                AppSettings.Save();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDashboard line " + line + ": " + ex.Message);
            }

        }
        private void FrmDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                AppSettings.DashboardWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    AppSettings.DashboardWindowLocation = this.Location;
                    AppSettings.DashboardWindowSize = this.Size;
                }
                else
                {
                    AppSettings.DashboardWindowLocation = this.RestoreBounds.Location;
                    AppSettings.DashboardWindowSize = this.RestoreBounds.Size;
                }
            }
            if (modified)
            {
                if (MessageBox.Show("Save settings?","Save settings?",MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    SaveGauges(AppSettings.DashboardLastFile);
                }
            }

        }

        private void LoadSettings()
        {
            tableLayoutPanel1.Controls.Clear();
            foreach (UpGauge g in Gauges)
            {
                g.Gauge.Click -= Gauge_Click;
                g.TxtBox.Click -= TxtBox_Click;
            }
            tableLayoutPanel1.RowCount = (int)numRows.Value;
            tableLayoutPanel1.ColumnCount = (int)numColumns.Value;
            int rsize = (int)(100 / numRows.Value);
            int csize = (int)(100 / numColumns.Value);
            for (int r = 0; r < numRows.Value; r++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent,rsize));
            }
            for (int c = 0; c < numColumns.Value; c++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, csize));
            }

            for (int r = 0; r < numRows.Value; r++)
            {
                for (int c = 0; c < numColumns.Value; c++)
                {
                    UpGauge gauge = Gauges.Where(X => X.Column == c && X.Row == r).FirstOrDefault();
                    if (gauge == null)
                    {
                        Button button = new Button();
                        button.Text = "Setup";
                        button.Width = 100;
                        button.Height = 30;
                        button.Top = 100;
                        button.Left = 100;
                        button.Tag = new Point(c, r);
                        button.Click += Button_Click;
                        tableLayoutPanel1.Controls.Add(button, c, r);
                    }
                    else
                    {
                        if (gauge.Type == UpGauge.GaugeType.Analog)
                        {
                            tableLayoutPanel1.Controls.Add(gauge.Gauge, c, r);
                            gauge.Gauge.Tag = new Point(c, r);
                            gauge.Gauge.Refresh();
                            gauge.Gauge.Click += Gauge_Click;
                        }
                        else
                        {
                            tableLayoutPanel1.Controls.Add(gauge.TxtBox, c, r);
                            gauge.TxtBox.Tag = new Point(c, r);
                            int cellWidth = (int)(tableLayoutPanel1.Width / numColumns.Value);
                            int fontsize = cellWidth / 10;
                            Debug.WriteLine("Font size: " + fontsize.ToString());
                            gauge.TxtBox.Font = new Font("Consolas", fontsize, FontStyle.Bold);
                            gauge.TxtBox.Top = (int)(tableLayoutPanel1.Height / numRows.Value / 2);
                            gauge.TxtBox.Text = gauge.CapText.PadLeft(8);
                            gauge.TxtBox.Click += TxtBox_Click; 
                        }
                    }
                }
            }
        }

        private void TxtBox_Click(object sender, EventArgs e)
        {
            Point point = (Point)((Label)sender).Tag;
            EditGauge(point);
        }

        private void Gauge_Click(object sender, EventArgs e)
        {
            Point point = (Point)((GdiSpeedometerApp.GDISpeedometer)sender).Tag;
            EditGauge(point);
        }

        private void SaveGauges(string FileName)
        {
            try
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    AppSettings.DashboardLastFile = Path.Combine(Application.StartupPath, "Logger", "Dashboard", "Dashboard.xml");
                    FileName = SelectSaveFile(XmlFilter, AppSettings.DashboardLastFile);
                    if (string.IsNullOrEmpty(FileName))
                    {
                        return;
                    }
                }
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<UpGauge>));
                    writer.Serialize(stream, Gauges);
                    stream.Close();
                }
                modified = false;
                AppSettings.DashboardLastFile = FileName;
                AppSettings.Save();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDashboard line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
            }

        }

        private void EditGauge(Point point)
        {
            try
            {
                UpGauge gauge = Gauges.Where(X => X.Column == point.X && X.Row == point.Y).FirstOrDefault();
                if (gauge == null)
                {
                    gauge = new UpGauge();
                    gauge.Column = point.X;
                    gauge.Row = point.Y;
                    Gauges.Add(gauge);
                }
                frmGaugeSettings fgs = new frmGaugeSettings(gauge);
                DialogResult res = fgs.ShowDialog();
                if (res == DialogResult.OK)
                {
                    LoadSettings();
                    modified = true;
                }
                else if (res == DialogResult.No)
                {
                    Gauges.Remove(gauge);
                    LoadSettings();
                    modified = true;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmDashboard line " + line + ": " + ex.Message);
            }

        }
        private void Button_Click(object sender, EventArgs e)
        {
            Point point = (Point)((Button)sender).Tag;
            EditGauge(point);
        }

        private void numRows_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.DashboardRows = (int)numRows.Value;
            AppSettings.Save();
            LoadSettings();
        }

        private void numColumns_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.DashboardCols = (int)numColumns.Value;
            AppSettings.Save();
            LoadSettings();
        }

        public void ShowValue(string PidName, double value)
        {
            List<UpGauge> gauges = Gauges.Where(X => X.PidName == PidName).ToList();
            if (gauges == null)
            {
                return;
            }
            foreach (UpGauge gauge in gauges)
            {
                if (gauge.Type == UpGauge.GaugeType.Analog)
                {
                    gauge.Gauge.Speed = value;
                }
                else if (gauge.Type == UpGauge.GaugeType.Digital)
                {
                    gauge.TxtBox.Text = gauge.CapText.PadLeft(8) + Environment.NewLine + value.ToString().PadLeft(9);
                }
                else
                {
                    if (value == 0)
                    {
                        gauge.TxtBox.Text = gauge.CapText.PadLeft(8) + Environment.NewLine + "OFF".PadLeft(8);
                    }
                    else
                    {
                        gauge.TxtBox.Text = gauge.CapText.PadLeft(8) + Environment.NewLine + "ON".PadLeft(8);
                    }
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AppSettings.DashboardLastFile))
            {
                AppSettings.DashboardLastFile = Path.Combine(Application.StartupPath, "Logger", "Dashboard", "Dashboard.xml");
            }
            string fName = SelectFile("Select dahboard config", XmlFilter, AppSettings.DashboardLastFile);
            if (fName.Length == 0)
            {
                return;
            }
            LoadXML(fName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGauges(AppSettings.DashboardLastFile);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGauges(null);
        }
    }
}
