using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Threading;
using static Helpers;
using static UniversalPatcher.DataLogger;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmLoggerGraphics : Form
    {
        public frmLoggerGraphics()
        {
            InitializeComponent();
        }

        public class PidScalar
        {
            public PidScalar()
            {
                On = false;
                Bar = false;
                Pid = "";
                Scalar = 1;
                Max = 1;
            }
            public PidScalar(string pid, int pidId)
            {
                On = false;
                Bar = false;
                Pid = pid;
                Scalar = 1;
                Max = 1;
                PidId = pidId;
            }
            public int PidId { get; set; }
            public bool On { get; set; }
            public string Pid { get; set; }
            public float Scalar { get; set; }
            public bool Bar { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
        }


        private class PointData
        {
            public PointData(int pidId, string Pid, ulong TStamp, double Val, double ScaledVal, int row)
            {
                PidId = pidId;
                PidName = Pid;
                TimeStamp = TStamp;
                Value = Val;
                ScaledValue = ScaledVal;
                Row = row;
            }
            public int PidId { get; set; }
            public string PidName { get; set; }
            public ulong TimeStamp { get; set; }
            public double Value { get; set; }
            public double ScaledValue { get; set; }
            public int Row { get; set; }
        }

        private class PointDataGroup
        {
            public PointDataGroup(int count)
            {
                pointDatas = new PointData[count];
            }
            public PointData[] pointDatas { get; set; }
        }

        private List<PidScalar> pidScalars;

        private string logFile;
        private bool LiveData;
        private bool PlayBack;
        //private int TStamps = 1;
        private string ProfileFile;
        private string Title;
        private List<PointDataGroup> pointDataGroups = new List<PointDataGroup>();
        ToolTip ScrollTip = new ToolTip();
        //public string LastLiveLogFile;
        private List<string> dataSourceNames = new List<string>();
        private List<Bitmap> dataSourceImage = new List<Bitmap>();
        private SeriesChartType ChartType = SeriesChartType.Line;
        private Point ChartMouseDownLocation;
        private List<BarGraph> vbars = new List<BarGraph>();
        private List<Label> vbarLabels = new List<Label>();
        private List<Label> vbarLabels2 = new List<Label>();
        private DispatcherTimer zoomTimer = new DispatcherTimer();
        private DispatcherTimer startTimer = new DispatcherTimer();
        private void frmLoggerGraphics_Load(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(AppSettings.LoggerLogSeparator))
                    txtLogSeparator.Text = AppSettings.LoggerLogSeparator;
                else
                    txtLogSeparator.Text = ",";
                chart1.Legends[0].Docking = Docking.Top;
                if (string.IsNullOrEmpty(AppSettings.LoggerGraphicsLiveLastProfileFile))
                {
                    AppSettings.LoggerGraphicsLiveLastProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerLastProfile));
                    AppSettings.Save();
                }
                if (string.IsNullOrEmpty(AppSettings.LoggerGraphicsLogLastProfileFile))
                {
                    AppSettings.LoggerGraphicsLogLastProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerLastProfile));
                    AppSettings.Save();
                }

                switch (AppSettings.LoggerGraphicsMouseCursor)
                {
                    case 0:
                        cursorXToolStripMenuItem.Checked = true;
                        break;
                    case 1:
                        cursorYToolStripMenuItem.Checked = true;
                        break;
                    case 2:
                        cursorXYToolStripMenuItem.Checked = true;
                        break;
                    case 3:
                        noCursorToolStripMenuItem.Checked = true;
                        break;
                }
                switch (AppSettings.LoggerGraphicsMouseZoom)
                {
                    case 0:
                        zoomXToolStripMenuItem.Checked = true;
                        break;
                    case 1:
                        zoomYToolStripMenuItem.Checked = true;
                        break;
                    case 2:
                        zoomXYToolStripMenuItem.Checked = true;
                        break;
                    case 3:
                        noZoomToolStripMenuItem.Checked = true;
                        break;
                }
                switch (AppSettings.LoggerGraphicsMouseWheel)
                {
                    case 0:
                        wheelZoomXToolStripMenuItem1.Checked = true;
                        break;
                    case 1:
                        wheelZoomYToolStripMenuItem1.Checked = true;
                        break;
                    case 2:
                        wheelZoomXYToolStripMenuItem1.Checked = true;
                        break;
                    case 3:
                        noWheelZoomToolStripMenuItem1.Checked = true;
                        break;
                }
                this.FormClosing += FrmLoggerGraphics_FormClosing;
                uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
                chart1.MouseDown += Chart1_MouseDown;
                chart1.MouseUp += Chart1_MouseUp;
                chart1.MouseWheel += Chart1_MouseWheel;
                showPointsToolStripMenuItem.Checked = AppSettings.LoggerGraphicsShowPoints;
                disableResampleToolStripMenuItem.Checked = AppSettings.LoggerGraphDisableResample;
                //SetUpDoubleBuffer(this);
                chart1.MouseClick += Chart1_MouseClick;
                loadCombobox1();
                dataGridPointValues.Columns.Add("Pid", "Pid");
                dataGridPointValues.Columns.Add("Value", "Value");
                numDisplayInterval.Value = AppSettings.LoggerGraphicsInterval;
                numShowMax.Value = AppSettings.LoggerGraphicsShowMaxTime;
                zoomTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                zoomTimer.Tick += Timer_Tick;
                startTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                startTimer.Tick += StartTimer_Tick;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void StartTimer_Tick(object sender, EventArgs e)
        {
            startTimer.Stop();
            ShowSelectedRange();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            zoomTimer.Stop();
            ShowSelectedRange();
        }

        private void Chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }

                if (e.Delta > 0)
                {
                    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    double posYStart = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    double posYFinish = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    if (wheelZoomXToolStripMenuItem1.Checked || wheelZoomXYToolStripMenuItem1.Checked)
                        chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    if (wheelZoomYToolStripMenuItem1.Checked || wheelZoomXYToolStripMenuItem1.Checked)
                        chart1.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void Chart1_MouseDown(object sender, MouseEventArgs e)
        {
            ChartMouseDownLocation = e.Location;
        }

        private void Chart1_MouseUp(object sender, MouseEventArgs e)
        {
            ChartArea ChartSelectionArea = chart1.ChartAreas[0];
            // Check if rectangle has at least 10 pixels with and hright

            if (zoomXToolStripMenuItem.Checked || zoomXYToolStripMenuItem.Checked)
            {
                if (Math.Abs(e.Location.X - ChartMouseDownLocation.X) > 10)
                {
                    ChartSelectionArea.AxisX.ScaleView.Zoom(
                        Math.Min(ChartSelectionArea.CursorX.SelectionStart, ChartSelectionArea.CursorX.SelectionEnd),
                        Math.Max(ChartSelectionArea.CursorX.SelectionStart, ChartSelectionArea.CursorX.SelectionEnd)
                    );
                }
            }
            if (zoomYToolStripMenuItem.Checked || zoomXYToolStripMenuItem.Checked)
            {
                if (Math.Abs(e.Location.Y - ChartMouseDownLocation.Y) > 10)
                {
                    ChartSelectionArea.AxisY.ScaleView.Zoom(
                        Math.Min(ChartSelectionArea.CursorY.SelectionStart, ChartSelectionArea.CursorY.SelectionEnd),
                        Math.Max(ChartSelectionArea.CursorY.SelectionStart, ChartSelectionArea.CursorY.SelectionEnd)
                    );
                }
            }
            // Reset/hide the selection rectangle
            ChartSelectionArea.CursorX.SetSelectionPosition(0D, 0D);
            ChartSelectionArea.CursorY.SetSelectionPosition(0D, 0D);
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.Down:
                        if (cursorYToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
                        {
                            double step = chart1.ChartAreas[0].AxisY.Maximum / 100;
                            chart1.ChartAreas[0].CursorY.SetCursorPosition(chart1.ChartAreas[0].CursorY.Position - step);
                        }
                        else
                        {
                            ScrollPointsPerScreen.Value--;
                            ShowSelectedRange();
                        }
                        break;
                    case Keys.Right:
                        if (cursorXToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
                        {
                            double xv = chart1.ChartAreas[0].CursorX.Position;
                            Series S = chart1.Series[0];            // short reference
                            DataPoint pNext = S.Points.Select(x => x)
                                                    .Where(x => x.XValue > xv)
                                                    .DefaultIfEmpty(S.Points.First()).First();

                            chart1.ChartAreas[0].CursorX.SetCursorPosition(pNext.XValue);
                            //chart1.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].CursorX.Position + 0.0000001);
                            ShowCurrentValues();
                        }
                        else
                        {
                            ScrollStartPoint.Value++;
                            ShowSelectedRange();
                        }
                        break;
                    case Keys.Up:
                        if (cursorYToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
                        {
                            double step = chart1.ChartAreas[0].AxisY.Maximum / 100;
                            chart1.ChartAreas[0].CursorY.SetCursorPosition(chart1.ChartAreas[0].CursorY.Position + step);
                        }
                        else
                        {
                            ScrollPointsPerScreen.Value++;
                            ShowSelectedRange();
                        }
                        break;
                    case Keys.Left:
                        if (cursorXToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
                        {
                            double xv = chart1.ChartAreas[0].CursorX.Position;
                            Series S = chart1.Series[0];            // short reference
                            DataPoint pPrev = S.Points.Select(x => x)
                                                    .Where(x => x.XValue < xv)
                                                    .DefaultIfEmpty(S.Points.First()).Last();

                            chart1.ChartAreas[0].CursorX.SetCursorPosition(pPrev.XValue);
                            //chart1.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].CursorX.Position - 0.0000001);
                            ShowCurrentValues();
                        }
                        else
                        {
                            ScrollStartPoint.Value--;
                            ShowSelectedRange();
                        }
                        break;
                    default:
                        return base.ProcessCmdKey(ref msg, keyData);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowCurrentValues()
        {
            try
            {
                double xv = chart1.ChartAreas[0].CursorX.Position;
                ulong xvdt = (ulong)DateTime.FromOADate(chart1.ChartAreas[0].CursorX.Position).Ticks;
                PointDataGroup pdg = pointDataGroups.Where(x => x.pointDatas[0].TimeStamp >= xvdt).First();
                if (pdg != null)
                {
                    DateTime dt = new DateTime((long)pdg.pointDatas[0].TimeStamp);
                    dataGridPointValues.Rows.Clear();
                    dataGridPointValues.Rows.Add();
                    dataGridPointValues.Rows[0].Cells["Pid"].Value = "Time";
                    dataGridPointValues.Rows[0].Cells["Value"].Value = dt.ToString("HH:mm:ss.ffff");
                    StringBuilder sb = new StringBuilder("[" + dt.ToString("HH:mm:ss.ffff") + "] ");
                    for (int s = 0; s < chart1.Series.Count; s++)
                    {
                        PointData pData = pdg.pointDatas[s];
                        int r = dataGridPointValues.Rows.Add();
                        sb.Append(" " + pData.PidName + ": " + pData.Value.ToString("0.00") + ",");
                        dataGridPointValues.Rows[r].Cells["Pid"].Value = pData.PidName;
                        dataGridPointValues.Rows[r].Cells["Value"].Value = pData.Value.ToString("0.00");
                        UpdateVbarValue(s, pData.Value);
                    }
                    labelDataValues.Text = sb.ToString().Trim(',');
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }

        }


        private void Chart1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Point pos = e.Location;

                if (e.Button == MouseButtons.Right)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                }

                HitTestResult[] results = chart1.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
                foreach (var result in results)
                {
                    DataPoint prop;
                    if (result.ChartElementType == ChartElementType.DataPoint)
                    {
                        prop = result.Object as DataPoint;
                        //Debug.WriteLine("Hit: " + prop.XValue);
                    }
                    else
                    {
                        double xv = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                        double yv = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                        Series S = chart1.Series[0];            // short reference
                        DataPoint pPrev = S.Points.Select(x => x)
                                                .Where(x => x.XValue >= xv)
                                                .DefaultIfEmpty(S.Points.First()).First();
/*                        DataPoint pNext = S.Points.Select(x => x)
                                                .Where(x => x.XValue <= xv)
                                                .DefaultIfEmpty(S.Points.Last()).Last();
*/
                        prop = pPrev;
                        //Debug.WriteLine("Prev: " + pPrev.XValue);
                    }
                    //DataPoint prop = nearestpoint;
                    if (prop != null)
                    {
                        PointData pd = (PointData)prop.Tag;
                        DateTime dt = new DateTime((long)pd.TimeStamp);
                        StringBuilder sb = new StringBuilder("[" + dt.ToString("HH:mm:ss.ffff") + "] ");
                        //DataPoint[] points;

                        dataGridPointValues.Rows.Clear();
                        dataGridPointValues.Rows.Add();
                        dataGridPointValues.Rows[0].Cells["Pid"].Value = "Time";
                        dataGridPointValues.Rows[0].Cells["Value"].Value = dt.ToString("HH:mm:ss.ffff");
                        for (int s = 0; s < chart1.Series.Count; s++)
                        {
                            PointData pData = pointDataGroups[pd.Row].pointDatas[s];
                            int r = dataGridPointValues.Rows.Add();
                            if (sb.Length < 2040)
                                sb.Append(" " + pData.PidName + ": " + pData.Value.ToString("0.00") + ",");
                            dataGridPointValues.Rows[r].Cells["Pid"].Value = pData.PidName;
                            dataGridPointValues.Rows[r].Cells["Value"].Value = pData.Value.ToString("0.00");
                            UpdateVbarValue(s, pData.Value);
                        }
                        labelDataValues.Text = sb.ToString().Trim(',');
                    }
                    
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void FrmLoggerGraphics_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProfile();
            AppSettings.LoggerGraphicsInterval = (int)numDisplayInterval.Value;
            AppSettings.LoggerGraphicsShowMaxTime = (int)numShowMax.Value;
            AppSettings.LoggerGraphicsShowPoints = showPointsToolStripMenuItem.Checked;
            if (cursorXToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseCursor = 0;
            if (cursorYToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseCursor = 1;
            if (cursorXYToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseCursor = 2;
            if (noCursorToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseCursor = 3;

            if(zoomXToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseZoom = 0;
            if(zoomYToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseZoom = 1;
            if(zoomXYToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseZoom = 2;
            if(noZoomToolStripMenuItem.Checked)
                AppSettings.LoggerGraphicsMouseZoom = 3;


            if(wheelZoomXToolStripMenuItem1.Checked)
                AppSettings.LoggerGraphicsMouseWheel = 0;
            if(wheelZoomYToolStripMenuItem1.Checked)
                AppSettings.LoggerGraphicsMouseWheel = 1;
            if(wheelZoomXYToolStripMenuItem1.Checked)
                AppSettings.LoggerGraphicsMouseWheel = 2;
            if(noWheelZoomToolStripMenuItem1.Checked)
                AppSettings.LoggerGraphicsMouseWheel = 3;

            AppSettings.Save();
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, txtResult);
        }

        public static void SetUpDoubleBuffer(System.Windows.Forms.Control chartControlForm)
        {

            System.Reflection.PropertyInfo formProp =
            typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            formProp.SetValue(chartControlForm, true, null);
        }

        private void DataGridValues_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLogGraphics();
        }

        private void SetupGraphProperties()
        {
            chart1.BackGradientStyle = GradientStyle.None;
            
            ChartArea CA = chart1.ChartAreas[0];  // quick reference            
            CA.Area3DStyle.Enable3D = false;
            //CA.AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            CA.CursorX.Interval = 0;
            //CA.AxisX.IntervalOffsetType = DateTimeIntervalType.Milliseconds;
            CA.AxisX.LabelStyle.Format = "HH:mm:ss";
            CA.CursorX.AutoScroll = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            CA.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
            CA.AxisX.MajorGrid.Enabled = false;
            CA.AxisX.MinorTickMark.Enabled = false;
            //CA.AxisX.Crossing = 0;

            CA.CursorY.IsUserSelectionEnabled = true;
            CA.AxisY.MajorGrid.Enabled = false;
            CA.AxisY.MinorTickMark.Enabled = false;
            //CA.AxisY.Crossing = 0;

            //Use special handling
            CA.AxisX.ScaleView.Zoomable = false;
            CA.AxisY.ScaleView.Zoomable = false;

/*            if (zoomXToolStripMenuItem.Checked || zoomXYToolStripMenuItem.Checked)
            {
                CA.AxisX.ScaleView.Zoomable = true;
            }
            else
            {
                CA.AxisX.ScaleView.Zoomable = false;
            }

            if (zoomYToolStripMenuItem.Checked || zoomXYToolStripMenuItem.Checked)
            {
                CA.AxisY.ScaleView.Zoomable = true;
            }
            else
            {
                CA.AxisY.ScaleView.Zoomable = false;
            }
*/

            if (cursorXToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked )
            {
                CA.CursorX.IsUserEnabled = true;
                CA.CursorX.IsUserSelectionEnabled = true;
                //CA.CursorX.LineColor = Color.Red;
                //CA.CursorX.LineWidth = 3;
                CA.CursorX.LineDashStyle = ChartDashStyle.Solid;
            }
            else
            {
                CA.CursorX.IsUserEnabled = false;
                CA.CursorX.LineDashStyle = ChartDashStyle.NotSet;
            }

            if (cursorYToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
            {
                CA.CursorY.IsUserEnabled = true;
                CA.CursorY.IsUserSelectionEnabled = true;
                CA.CursorY.LineDashStyle = ChartDashStyle.Solid;
            }
            else
            {
                CA.CursorY.IsUserEnabled = false;
                CA.CursorY.LineDashStyle = ChartDashStyle.NotSet;
            }
        }

        private void ImportPidProfile()
        {
            try
            {
                pidScalars = new List<PidScalar>();
                chart1.Series.Clear();
                for (int r = 0; r < datalogger.PidProfile.Count; r++)
                {
                    PidScalar ps = new PidScalar(datalogger.PidProfile[r].PidName, datalogger.PidProfile[r].addr);
                    pidScalars.Add(ps);
                    chart1.Series.Add(new Series());
                    chart1.Series[r].ChartType = ChartType;
                    chart1.Series[r].XValueType = ChartValueType.DateTime;
                    if (datalogger.PidProfile[r].PidName != null)
                        chart1.Series[r].Name = r.ToString() + "-" +  datalogger.PidProfile[r].PidName;
                    chart1.Series[r].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                }
                dataGridSettings.DataSource = pidScalars;
                dataGridSettings.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dataGridSettings.Columns["On"].Width = 30;
                dataGridSettings.Columns["Bar"].Width = 30;
                dataGridSettings.Columns["Max"].Width = 30;
                AddVBars();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        public void SetupLiveGraphics()
        {
            Title = "Logger Graph";
            pointDataGroups = new List<PointDataGroup>();
            ImportPidProfile();
            SetupGraphProperties();
            LiveData = true;
            groupLiveSeconds.Enabled = true;
            if (File.Exists(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile))))
            {
                ProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile));
                LoadProfile();
            }
            chkGetLiveData.Checked = true;
        }

        private void LogEvents_LogDataAdded(object sender, DataLogger.LogDataEvents.LogDataEvent e)
        {
            QueueliveData(e.Data);
        }

        public void UpdateLiveGraphics()
        {
            chart1.Series.Clear();
            chart1.BackGradientStyle = GradientStyle.None;
            chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
            for (int p = 0; p < pidScalars.Count; p++)
            {
                chart1.Series.Add(new Series());
                chart1.Series[p].ChartType = ChartType;
                //chart1.Series[r].XValueType = ChartValueType.DateTime;
                if (datalogger.PidProfile[p].PidName != null)
                    chart1.Series[p].Name = datalogger.PidProfile[p].PidName;
                chart1.Series[p].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                //chart1.Series[p].IsVisibleInLegend = false;
                if (!pidScalars[p].On)
                {
                    chart1.Series[p].Enabled = false;
                }
            }
            dataGridSettings.DataSource = null;
            dataGridSettings.DataSource = pidScalars;
            dataGridSettings.Update();
            dataGridSettings.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void QueueliveData(LogData ld)
        {
            try
            {
                PointDataGroup pdg = new PointDataGroup(ld.CalculatedValues.Length);
                for (int p = 0; p < ld.Values.Length; p++)
                {
                    double orgVal = ld.CalculatedValues[p];
                    if (orgVal == double.MinValue || orgVal == double.MaxValue)
                    {
                        orgVal = 0;
                    }
                    double scaledVal = orgVal * pidScalars[p].Scalar;
                    PointData pd = new PointData(pidScalars[p].PidId, pidScalars[p].Pid, ld.TimeStamp, orgVal, scaledVal, pointDataGroups.Count-1);
                    pdg.pointDatas[p] = pd;
                }
                pointDataGroups.Add(pdg);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, ShowliveData line " + line + ": " + ex.Message);
            }
        }

        private void SetupLogGraphics(string[] header, string FileName)
        {
            pidScalars = new List<PidScalar>();
            //pidScalars.Add(new PidScalar("TimeStamp"));  
            this.Text = "Logger Graph - " + FileName;
            Title = this.Text;
            chart1.Series.Clear();
            SetupGraphProperties();
            //TStamps = 0;
            SeriesChartType ct = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), comboBox1.Text);
            LiveData = false;
            timerDisplayData.Enabled = false;
            //groupLiveSeconds.Enabled = false;
            if (File.Exists(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", AppSettings.LoggerGraphicsLogLastProfileFile)))
            {
                ProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", AppSettings.LoggerGraphicsLogLastProfileFile);
                //LoadProfile();
                this.Text += " [" + Path.GetFileName(ProfileFile) + "]";
            }

            chkGetLiveData.Checked = false;
            LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
            ScrollStartPoint.Visible = true;
            ScrollPointsPerScreen.Enabled = true;
            ScrollStartPoint.Value = 0;
        }

        private void UpdateVbarValue(int pidId, double value)
        {
            try
            {
                for (int v = 0; v < vbars.Count; v++)
                {
                    int p = (int)vbars[v].Tag;
                    if (p == pidId)
                    {
                        int percentval = (int)((value - pidScalars[pidId].Min) / (pidScalars[pidId].Max - pidScalars[pidId].Min) * 100);
                        if (percentval > 100)
                        {
                            percentval = 100;
                        }
                        if (percentval < 0)
                        {
                            percentval = 0;
                        }
                        vbars[v].SetValue( percentval);
                        vbarLabels2[v].Text = value.ToString("0.##");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void AddVBars()
        {
            for (int v=0; v < vbars.Count; v++)
            {
                vbars[v].Dispose();
                vbarLabels[v].Dispose();
                vbarLabels2[v].Dispose();
            }
            vbars = new List<BarGraph>();
            vbarLabels = new List<Label>();
            vbarLabels2 = new List<Label>();
            int left = 10;
            int barWidth = 60;
            splitContainer3.Panel1Collapsed = false;
            splitContainer3.Panel1.Show();
            for (int p = 0; p < pidScalars.Count; p++)
            {
                if (pidScalars[p].Bar)
                {
                    BarGraph vbar = new BarGraph();
                    vbar.Left = left;
                    vbar.Top = 20;
                    vbar.Width = barWidth;
                    vbar.Height = splitContainer3.Panel1.Height - 25;
                    vbar.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
                    vbar.ForeColor = Color.Red;
                    vbar.Tag = p;
                    //vbar.MarqueeAnimationSpeed = 1;
                    splitContainer3.Panel1.Controls.Add(vbar);
                    vbars.Add(vbar);
                    vbar.Show();

                    Label lb = new Label();
                    lb.Text = pidScalars[p].Pid;
                    lb.Left = left;
                    lb.Top = 1;
                    lb.AutoSize = true;
                    splitContainer3.Panel1.Controls.Add(lb);
                    vbarLabels.Add(lb);

                    Label lb2 = new Label();
                    lb2.Text = "";
                    lb2.Left = left + 10;
                    lb2.Top = (int)(splitContainer3.Panel1.Height /2);
                    lb2.AutoSize = true;
                    lb2.BackColor = Color.White;
                    splitContainer3.Panel1.Controls.Add(lb2);
                    lb2.BringToFront();
                    vbarLabels2.Add(lb2);

                    if (lb.Width > barWidth)
                        left += lb.Width + 5;
                    else
                        left += barWidth + 5;
                }
            }
            if (vbarLabels.Count == 0)
            {
                splitContainer3.Panel1Collapsed = true;
                splitContainer3.Panel1.Hide();
            }
            else
            {
                int total = vbarLabels.Last().Left + vbarLabels.Last().Width;
                if (total > (0.9 * splitContainer3.Width))
                    total = (int)(0.9 * splitContainer3.Width);
                splitContainer3.SplitterDistance = total;
            }
        }

        public void UpdateLogGraphics()
        {
            try
            {
                AddVBars();

                for (int p = 0; p < pidScalars.Count; p++)
                {
                    if (pidScalars[p].On)
                    {
                        chart1.Series[p].Enabled = true;
                    }
                    else
                    {
                        chart1.Series[p].Enabled = false;
                    }

                }
                for (int pd = 0; pd < pointDataGroups.Count; pd++)
                {
                    for (int p = 0; p < pidScalars.Count; p++)
                    {
                        if (pointDataGroups[pd].pointDatas[p].Value > double.MinValue)
                            pointDataGroups[pd].pointDatas[p].ScaledValue = pidScalars[p].Scalar * pointDataGroups[pd].pointDatas[p].Value;
                    }
                }
                dataGridSettings.DataSource = pidScalars;
                dataGridSettings.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void DetectMinMax()
        {
            try
            {
                for (int i = 0; i < pidScalars.Count; i++)
                {
                    double min = double.MaxValue;
                    double max = double.MinValue;
                    for (int x = 0; x < datalogger.LogDataBuffer.Count; x++)
                    {
                        if (datalogger.LogDataBuffer[x].Values[i] > max)
                            max = datalogger.LogDataBuffer[x].Values[i];
                        if (datalogger.LogDataBuffer[x].Values[i] < min)
                            min = datalogger.LogDataBuffer[x].Values[i];
                    }
                    pidScalars[i].Min = min;
                    pidScalars[i].Max = max;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }
        private void ReadDataFromFile()
        {
            try
            {
                if (string.IsNullOrEmpty(logFile))
                {
                    return;
                }
                LiveData = false;
                PlayBack = false;
                datalogger.LoadLogFile(logFile);
                ImportPidProfile();
                DetectMinMax();
                ImportLogDataBuffer();
                if (datalogger.PidProfile.Count > 10)
                {
                    chkSelectAll.Checked = false;
                    Logger("More than 10 pids, all pids disabled by default. Select pids you want and click Apply");
                }
                else
                {
                    chkSelectAll.Checked = true;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void ShowSelectedRange()
        {
            try
            {
                int startP=0;
                int endP = pointDataGroups.Count;
                List<PointDataGroup> pValues = pointDataGroups;

                ScrollStartPoint.Maximum = pointDataGroups.Count;
                ScrollPointsPerScreen.Maximum = pointDataGroups.Count;
                ScrollPointsPerScreen.Minimum = 10;

                if (pointDataGroups.Count == 0)
                {
                    return;
                }
                for (int s = 0; s < chart1.Series.Count; s++)
                {
                    chart1.Series[s].Points.Clear();
                }

                if (PlayBack && timerDisplayData.Enabled)
                {
                    startP = ScrollStartPoint.Value;
                    DateTime startTd = new DateTime((long)pointDataGroups[startP].pointDatas[0].TimeStamp);
                    ulong endTstamp = (ulong)startTd.AddSeconds((double)numShowMax.Value).Ticks;
                    for (int p=startP; p<pointDataGroups.Count;p++)
                    {
                        if (pointDataGroups[p].pointDatas[0].TimeStamp >= endTstamp)
                        {
                            endP = p;
                            break;
                        }
                    }
                }
                else if (timerDisplayData.Enabled)
                {
                    endP = pointDataGroups.Count;
                    startP = 0;
                    DateTime endTd = new DateTime((long)pointDataGroups.Last().pointDatas[0].TimeStamp);
                    ulong startTstamp = (ulong)endTd.AddSeconds((double)numShowMax.Value * -1).Ticks;
                    for (int p=pointDataGroups.Count -1; p >= 0; p--)
                    {
                        if (pointDataGroups[p].pointDatas[0].TimeStamp <= startTstamp)
                        {
                            startP = p;
                            break;
                        }
                    }
                }
                else
                {
                    startP = ScrollStartPoint.Value;
                    endP = ScrollStartPoint.Value + ScrollPointsPerScreen.Value;
                    if (endP > pointDataGroups.Count)
                    {
                        startP = pointDataGroups.Count - ScrollPointsPerScreen.Value;
                        endP = pointDataGroups.Count;
                    }
                    if (startP < 0)
                    {
                        startP = 0;
                    }

                    if (ScrollPointsPerScreen.Value > 2000 && disableResampleToolStripMenuItem.Checked == false)
                    {
                        pValues = ResampleData();
                        startP = 0;
                        endP = pValues.Count;
                    }
                    else
                    {
                        labelZoom.ForeColor = Color.Black;
                        labelZoom.Text = "Zoom";
                    }
                }
                Stopwatch timer = new Stopwatch();
                timer.Start();
                for (int r=0;r< chart1.Series.Count; r++)
                {
                    chart1.Series[r].Points.SuspendUpdates();
                }
                for (int x = startP; x < endP; x++)
                {
                    PointDataGroup pdg = pValues[x];
                    for (int r = 0; r < pdg.pointDatas.Length; r++)
                    {
                        PointData pd = pdg.pointDatas[r];
                        if (pd != null && pd.Value > double.MinValue)
                        {
                            DataPoint point = new DataPoint();
                            point.Tag = pd;
                            DateTime tStamp = new DateTime((long)pd.TimeStamp);
                            string tStampStr = tStamp.ToString("HH:mm:ss.ffff");
                            point.SetValueXY(tStamp, pd.ScaledValue);
                            point.ToolTip = string.Format("[{0}] {1}: {2}", tStampStr, pd.PidName, pd.Value);
                            if (showPointsToolStripMenuItem.Checked)
                                point.MarkerStyle = MarkerStyle.Circle;
                            else
                                point.MarkerStyle = MarkerStyle.None;
                            chart1.Series[r].Points.Add(point);
                        }
                    }
                }
                //Show vbars for last value:
                PointDataGroup pdg2 = pValues[endP-1];
                for (int r = 0; r < pdg2.pointDatas.Length; r++)
                {
                    UpdateVbarValue(r ,pdg2.pointDatas[r].ScaledValue);
                }
                for (int r = 0; r < chart1.Series.Count; r++)
                {
                    chart1.Series[r].Points.ResumeUpdates();
                }

                Debug.WriteLine("Graph data set Time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void LoadLogFile()
        {
            try
            {
                this.Text = Title + " [" + Path.GetFileName(ProfileFile) + "]";
                labelDataValues.Text = "Click data point to show values";
                chart1.Series.Clear();
                chkSelectAll.Checked = false;
                StreamReader sr = new StreamReader(logFile);
                string hdrLine = sr.ReadLine();
                sr.Close();
                AppSettings.LoggerLogSeparator = txtLogSeparator.Text;
                AppSettings.Save();
                string[] hdrArray = hdrLine.Split(new string[] { txtLogSeparator.Text }, StringSplitOptions.None);
                SetupLogGraphics(hdrArray, Path.GetFileName(logFile));
                groupPlayback.Enabled = true;
                ReadDataFromFile();
                LoadProfile();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void loadLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmImportLogFile fil = new frmImportLogFile();
                if (fil.ShowDialog() == DialogResult.OK)
                {
                    string fName = fil.txtFileName.Text;
                    logFile = fName;
                    Title = "Logger Graph - " + fName;
                    LoadLogFile();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void UpdateScalars()
        {
            if (pidScalars == null || pidScalars.Count == 0)
                return;
            UpdateLogGraphics();
            ShowSelectedRange();

        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            UpdateScalars();
        }

         private void timerDisplayData_Tick(object sender, EventArgs e)
        {
            try
            {
                ShowSelectedRange();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void numDisplayInterval_ValueChanged(object sender, EventArgs e)
        {
            timerDisplayData.Interval = (int)(1000 * numDisplayInterval.Value);
        }

        private void SaveProfile()
        {
            try
            {
                if (string.IsNullOrEmpty(ProfileFile) || pidScalars == null || pidScalars.Count == 0)
                {
                    return;
                }
                Logger("Saving profile: " + ProfileFile + "... ", false);
                using (FileStream stream = new FileStream(ProfileFile, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidScalar>));
                    writer.Serialize(stream, pidScalars);
                    stream.Close();
                }
                Logger(" [OK]");
                if (LiveData)
                    AppSettings.LoggerGraphicsLiveLastProfileFile = Path.GetFileName(ProfileFile);
                else
                    AppSettings.LoggerGraphicsLogLastProfileFile = Path.GetFileName(ProfileFile);
                AppSettings.Save();
                this.Text = Title + " [" + Path.GetFileName(ProfileFile) + "]";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }

        }

        private void LoadProfile()
        {
            try
            {
                if (string.IsNullOrEmpty(ProfileFile))
                {
                    UpdateScalars();
                    return;
                }
                Logger("Loading profile: " + ProfileFile + "... ", false);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidScalar>));
                System.IO.StreamReader file = new System.IO.StreamReader(ProfileFile);
                List<PidScalar> tmpScalars = (List<PidScalar>)reader.Deserialize(file);
                file.Close();

                bool compatible = true;
                for (int i=0; i< pidScalars.Count; i++)
                {
                    if (pidScalars[i].Pid != tmpScalars[i].Pid)
                    {
                        compatible = false;
                        break;
                    }
                }
                if (tmpScalars.Count != pidScalars.Count || !compatible)
                {
                    LoggerBold(" Profile not compatible");
                    ProfileFile = "";
                }
                else
                {
                    Logger(" [OK]");
                    pidScalars = tmpScalars;

                    if (LiveData)
                        AppSettings.LoggerGraphicsLiveLastProfileFile = Path.GetFileName(ProfileFile);
                    else
                        AppSettings.LoggerGraphicsLogLastProfileFile = Path.GetFileName(ProfileFile);
                    AppSettings.Save();
                }
                UpdateScalars();
                this.Text = Title + " [" + Path.GetFileName(ProfileFile) + "]";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void saveProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileFile))
                SaveProfileAs();
            else
                SaveProfile();
        }

        private void loadProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string def = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile));
            string fName = SelectFile("Select profilefile", XmlFilter, def);
            if (string.IsNullOrEmpty(fName))
                return;
            ProfileFile = fName;
            LoadProfile();
        }

        private void SaveProfileAs()
        {
            string def = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile));
            string fName = SelectSaveFile(XmlFilter, def);
            if (string.IsNullOrEmpty(fName))
                return;
            ProfileFile = fName;
            SaveProfile();
        }

        private void saveProfileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProfileAs();
        }

        private void loadLastLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logFile = AppSettings.LoggerLastLogfile;
            Title = "Logger Graph - " + AppSettings.LoggerLastLogfile;
            LoadLogFile();
        }

        public void StopLiveUpdate()
        {
            timerDisplayData.Enabled = false;
            ScrollStartPoint.Visible = true;
            ScrollPointsPerScreen.Enabled = true;
            groupPlayback.Enabled = true;
            LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
        }

        public void StartLiveUpdate(bool PlayBack)
        {
            this.PlayBack = PlayBack;
            this.LiveData = true;
            timerDisplayData.Enabled = true;
            ScrollPointsPerScreen.Enabled = false;
            if (!PlayBack)
            {
                groupPlayback.Enabled = false;
                ScrollStartPoint.Visible = false;
                LoggerDataEvents.LogDataAdded += LogEvents_LogDataAdded;
            }
        }

        private void chkGetLiveData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGetLiveData.Checked )
            {
                if (!LiveData)
                    SetupLiveGraphics();
                StartLiveUpdate(PlayBack);
            }
            if (!chkGetLiveData.Checked && LiveData)
            {
                StopLiveUpdate();                
            }
        }

        private void ScrollPointsPerScreen_Scroll(object sender, ScrollEventArgs e)
        {
            //ShowSelectedRange();
            if (zoomTimer.IsEnabled) 
                zoomTimer.Stop();
            zoomTimer.Start();
            Debug.WriteLine("ScrollPointsPerScreen_Scroll");
            ShowToolTip(ScrollPointsPerScreen.Value.ToString() + " pt/screen");
        }

        private void ScrollStartPoint_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                //ShowSelectedRange();
                if (startTimer.IsEnabled)
                    startTimer.Stop();
                startTimer.Start();
                Debug.WriteLine("ScrollStartPoint_Scroll");
                DateTime dt = new DateTime((long)pointDataGroups[ScrollStartPoint.Value].pointDatas[0].TimeStamp);
                ShowToolTip(dt.ToString("HH:mm:ss.ffff"));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void ShowToolTip(string message)
        {
            ScrollTip.Show(message, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 20, 2000);
            //ScrollTip.Show(message, this,this.Location.X, this.Location.Y, 1000);
        }

        private void loadCombobox1()
        {
            // Get ChartTypes and Images 
            var resourceStream = typeof(Chart).Assembly
                    .GetManifestResourceStream("System.Windows.Forms.DataVisualization.Charting.Design.resources");
            //List<string> usableTypes = new List<string>() {"Area", "StepLine", "Line","FastLine", "Bubble", "Spline", "Column", "Point", "FastPoint"};
            List<string> usableTypes = new List<string>() { "Area", "StepLine", "Line", "FastLine", "Spline", "Column", "Point", "FastPoint" };
            using (System.Resources.ResourceReader resReader = new ResourceReader(resourceStream))
            {
                var dictEnumerator = resReader.GetEnumerator();

                while (dictEnumerator.MoveNext())
                {
                    var ent = dictEnumerator.Entry;
                    string typeStr = ent.Key.ToString().Replace("ChartType", "");
                    if (usableTypes.Contains(typeStr))
                    {
                        dataSourceNames.Add(typeStr);
                        dataSourceImage.Add(ent.Value as Bitmap);
                    }
                }
            }

            //Load ChartType Into combobox
            comboBox1.DataSource = dataSourceNames;
            comboBox1.MaxDropDownItems = 10;
            comboBox1.IntegralHeight = false;
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DrawItem += comboBox1_DrawItem;
            comboBox1.Text = "Line";
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ChartType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), comboBox1.Text);
                for (int s = 0; s < chart1.Series.Count; s++)
                {
                    chart1.Series[s].ChartType = ChartType;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                // Get text string
                var txt = comboBox1.GetItemText(comboBox1.Items[e.Index]);

                // Specify points for drawing
                var r1 = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1,
                        2 * (e.Bounds.Height - 2), e.Bounds.Height - 2);

                var r2 = Rectangle.FromLTRB(r1.Right + 2, e.Bounds.Top,
                        e.Bounds.Right, e.Bounds.Bottom);

                //Draw Image from list
                e.Graphics.DrawImage(dataSourceImage[e.Index], r1);
                e.Graphics.DrawRectangle(Pens.Black, r1);
                TextRenderer.DrawText(e.Graphics, txt, comboBox1.Font, r2,
                        comboBox1.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }

        private List<PointDataGroup> ResampleData()
        {
            List<PointDataGroup> retVal = new List<PointDataGroup>();
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                int samplesize = ScrollPointsPerScreen.Value / 500;
                int pointsLeft = pointDataGroups.Count - ScrollStartPoint.Value;
                if (pointsLeft < ScrollPointsPerScreen.Value)
                {
                    if (pointsLeft < 2000)
                    {
                        for (int p=ScrollStartPoint.Value; p < pointDataGroups.Count; p++)
                        {
                            retVal.Add(pointDataGroups[p]);
                            labelZoom.ForeColor = Color.Black;
                            labelZoom.Text = "Zoom";
                        }
                        return retVal;
                    }
                    else
                    {
                        samplesize = pointsLeft / 500;
                    }
                }
                labelZoom.ForeColor = Color.Red;
                labelZoom.Text = "Zoom (resampled)";
                int pidcount = pointDataGroups[0].pointDatas.Length;
                int endP = ScrollPointsPerScreen.Value + ScrollStartPoint.Value;
                if (endP > pointDataGroups.Count)
                {
                    endP = pointDataGroups.Count;
                }
                Debug.WriteLine("Resampling, samplesize: " + samplesize.ToString());
                for (int pos = ScrollStartPoint.Value; (pos + samplesize) < endP; pos += samplesize)
                {
                    PointDataGroup pdgmin = new PointDataGroup(pidcount);
                    PointDataGroup pdgmax = new PointDataGroup(pidcount);
                    for (int pid = 0; pid < pidcount; pid++)
                    {
                        double min = double.MaxValue;
                        double max = double.MinValue;
                        int minrow = -1;
                        int maxrow = -1;
                        for (int x = 0; x < samplesize; x++)
                        {
                            PointData pd = pointDataGroups[pos + x].pointDatas[pid];
                            if (pd.Value < min)
                            {
                                min = pd.Value;
                                minrow = pos + x;
                            }
                            if (pd.Value > max)
                            {
                                max = pd.Value;
                                maxrow = pos + x;
                            }
                        }
                        if (minrow < 0)
                        {
                            minrow = pos;
                        }
                        if (maxrow < 0)
                        {
                            maxrow = pos;
                        }
                        //Debug.WriteLine("pid: " + pid + ", minrow: " + minrow + ", maxrow: " + maxrow);
                        if (minrow > maxrow)
                        {
                            //Don't mix pointorder
                            pdgmin.pointDatas[pid] = pointDataGroups[maxrow].pointDatas[pid];
                            pdgmax.pointDatas[pid] = pointDataGroups[minrow].pointDatas[pid];
                        }
                        else
                        {
                            pdgmin.pointDatas[pid] = pointDataGroups[minrow].pointDatas[pid];
                            pdgmax.pointDatas[pid] = pointDataGroups[maxrow].pointDatas[pid];
                        }
                    }
                    retVal.Add(pdgmin);
                    retVal.Add(pdgmax);
                }
                Debug.WriteLine("Resampled size: " + retVal.Count.ToString());
                timer.Stop();
                Debug.WriteLine("Resample Time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
            }
            return retVal;
        }

        private void btnAutoscale_Click(object sender, EventArgs e)
        {
        }

        private void autoscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int pid = 0; pid < pidScalars.Count; pid++)
            {
                double max = 0;
                for (int p = 0; p < pointDataGroups.Count; p++)
                {
                    if (Math.Abs(pointDataGroups[p].pointDatas[pid].Value) > max)
                    {
                        max = Math.Abs(pointDataGroups[p].pointDatas[pid].Value);
                    }
                }
                if (max > 0)
                {
                    pidScalars[pid].Scalar = (float)(100 / max);
                }
            }
            UpdateScalars();

        }

        private void showPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPointsToolStripMenuItem.Checked = !showPointsToolStripMenuItem.Checked;
            ShowSelectedRange();
        }

        private void zoomXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = true;
            zoomYToolStripMenuItem.Checked = false;
            zoomXYToolStripMenuItem.Checked = false;
            noZoomToolStripMenuItem.Checked = false;
            SetupGraphProperties();
        }

        private void zoomYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = false;
            zoomYToolStripMenuItem.Checked = true;
            zoomXYToolStripMenuItem.Checked = false;
            noZoomToolStripMenuItem.Checked = false;
            SetupGraphProperties();

        }

        private void zoomXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = false;
            zoomYToolStripMenuItem.Checked = false;
            zoomXYToolStripMenuItem.Checked = true;
            noZoomToolStripMenuItem.Checked = false;
            SetupGraphProperties();

        }

        private void cusrorXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = true;
            cursorYToolStripMenuItem.Checked = false;
            cursorXYToolStripMenuItem.Checked = false;
            noCursorToolStripMenuItem.Checked = false;
            SetupGraphProperties();

        }

        private void cursorYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = false;
            cursorYToolStripMenuItem.Checked = true;
            cursorXYToolStripMenuItem.Checked = false;
            noCursorToolStripMenuItem.Checked = false;
            SetupGraphProperties();

        }

        private void cursorXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = false;
            cursorYToolStripMenuItem.Checked = false;
            cursorXYToolStripMenuItem.Checked = true;
            noCursorToolStripMenuItem.Checked = false;
            SetupGraphProperties();
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
        }

        private void noZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = false;
            zoomYToolStripMenuItem.Checked = false;
            zoomXYToolStripMenuItem.Checked = false;
            noZoomToolStripMenuItem.Checked = true;
            SetupGraphProperties();
        }

        private void noCursorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = false;
            cursorYToolStripMenuItem.Checked = false;
            cursorXYToolStripMenuItem.Checked = false;
            noCursorToolStripMenuItem.Checked = true;
            SetupGraphProperties();
        }

        private void ImportLogDataBuffer()
        {
            try
            {
                pointDataGroups = new List<PointDataGroup>();
                for (int i = 0; i < datalogger.LogDataBuffer.Count; i++)
                {
                    LogData ld = datalogger.LogDataBuffer[i];
                    PointDataGroup pdg = new PointDataGroup(ld.CalculatedValues.Length);
                    for (int p = 0; p < ld.Values.Length; p++)
                    {
                        double orgVal = ld.CalculatedValues[p];
/*                        if (orgVal == double.MinValue || orgVal == double.MaxValue)
                        {
                            orgVal = 0;
                        }
*/                       
                        double scaledVal = orgVal;
                        PointData pd = new PointData(pidScalars[p].PidId, pidScalars[p].Pid, ld.TimeStamp, orgVal, scaledVal, pointDataGroups.Count - 1);
                        pdg.pointDatas[p] = pd;
                    }
                    pointDataGroups.Add(pdg);
                }
                ScrollPointsPerScreen.Maximum = pointDataGroups.Count;
                ScrollPointsPerScreen.Value = ScrollPointsPerScreen.Maximum;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        public void SetupPlayBack()
        {
            SetupLiveGraphics();
            PlayBack = true;
            ImportPidProfile();
            ImportLogDataBuffer();
        }

        public void PlayBackStep(int Position)
        {
            ScrollStartPoint.Value = Position;
        }

        private void numPlaybackSpeed_ValueChanged(object sender, EventArgs e)
        {
            SetPlaybackSpeed();
        }
        private void SetPlaybackSpeed()
        {
            if (pointDataGroups.Count > 2)
            {
                DateTime t1 = new DateTime((long)pointDataGroups[1].pointDatas[0].TimeStamp);
                DateTime t2 = new DateTime((long)pointDataGroups[2].pointDatas[0].TimeStamp);
                TimeSpan step = t2.Subtract(t1);
                int ival = (int)((decimal)step.TotalMilliseconds / numPlaybackSpeed.Value);
                if (ival == 0)
                {
                    ival = 1;
                }
                timerPlayback.Interval = ival;
            }
        }
       
        private void timerPlayback_Tick(object sender, EventArgs e)
        {
            try
            {
                double loc = (chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum) / 2;
                //if (chart1.ChartAreas[0].CursorX.Position > chart1.ChartAreas[0].AxisX.Minimum && chart1.ChartAreas[0].CursorX.Position < chart1.ChartAreas[0].AxisX.Maximum)
                  //  loc = chart1.ChartAreas[0].CursorX.Position - chart1.ChartAreas[0].AxisX.Minimum;
                ScrollStartPoint.Value++;
                if (ScrollStartPoint.Value == ScrollStartPoint.Maximum)
                {
                    timerPlayback.Enabled = false;
                    StopLiveUpdate();
                    return;
                }
                chart1.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].AxisX.Minimum + loc);               
                ShowCurrentValues();
                //ShowSelectedRange();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }


        private void btnPlay_Click(object sender, EventArgs e)
        {
            groupLiveSeconds.Enabled = true;
            ScrollStartPoint.Enabled = true;
            StartLiveUpdate(true);
            timerPlayback.Enabled = true;
            timerDisplayData.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            StopLiveUpdate();
            timerPlayback.Enabled = false;
            timerDisplayData.Enabled = false;
        }

        private void zoomXToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            wheelZoomXToolStripMenuItem1.Checked = true;
            wheelZoomYToolStripMenuItem1.Checked = false;
            wheelZoomXYToolStripMenuItem1.Checked = false;
            noWheelZoomToolStripMenuItem1.Checked = false;
        }

        private void zoomYToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            wheelZoomXToolStripMenuItem1.Checked = false;
            wheelZoomYToolStripMenuItem1.Checked = true;
            wheelZoomXYToolStripMenuItem1.Checked = false;
            noWheelZoomToolStripMenuItem1.Checked = false;
        }

        private void zoomXYToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            wheelZoomXToolStripMenuItem1.Checked = false;
            wheelZoomYToolStripMenuItem1.Checked = false;
            wheelZoomXYToolStripMenuItem1.Checked = true;
            noWheelZoomToolStripMenuItem1.Checked = false;
        }

        private void noZoomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            wheelZoomXToolStripMenuItem1.Checked = false;
            wheelZoomYToolStripMenuItem1.Checked = false;
            wheelZoomXYToolStripMenuItem1.Checked = false;
            noWheelZoomToolStripMenuItem1.Checked = true;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i=0; i<pidScalars.Count;i++)
            {
                pidScalars[i].On = chkSelectAll.Checked;
            }
            this.Refresh();
        }

        private void disableResampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            disableResampleToolStripMenuItem.Checked = !disableResampleToolStripMenuItem.Checked;
            AppSettings.LoggerGraphDisableResample = disableResampleToolStripMenuItem.Checked;
            ShowSelectedRange();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
