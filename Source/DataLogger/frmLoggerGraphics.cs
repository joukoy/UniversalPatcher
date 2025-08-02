using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public frmLoggerGraphics(bool Live)
        {
            this.LiveData = Live;
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
            public PidScalar(string pid, string pidId)
            {
                On = false;
                Bar = false;
                Pid = pid;
                Scalar = 1;
                Max = 1;
                PidId = pidId;
            }
            public string PidId { get; set; }
            public bool On { get; set; }
            public string Pid { get; set; }
            public float Scalar { get; set; }
            public bool Bar { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
        }

        private List<PidScalar> pidScalars;

        private string logFile;
        private bool LiveData;
        private bool PlayBack;
        //private int TStamps = 1;
        private string ProfileFile;
        private string Title;
        ToolTip ScrollTip = new ToolTip();
        //public string LastLiveLogFile;
        private List<string> dataSourceNames = new List<string>();
        private List<Bitmap> dataSourceImage = new List<Bitmap>();
        private SeriesChartType ChartType = SeriesChartType.Line;
        private Point ChartMouseDownLocation;
        private List<BarGraph> vbars = new List<BarGraph>();
        private List<Label> vbarLabels = new List<Label>();
        private List<Label> vbarLabels2 = new List<Label>();
        private DispatcherTimer posTimer = new DispatcherTimer();
        private GraphSettings graphSettings;
        private int prevDataCount = 0;
        private void frmLoggerGraphics_Load(object sender, EventArgs e)
        {
            try
            {
                graphSettings = new GraphSettings(chart1);
                tabAdvanced.Enter += TabAdvanced_Enter;
                tabZoom.Enter += TabZoom_Enter;
                if (!string.IsNullOrEmpty(AppSettings.LoggerLogSeparator))
                    txtLogSeparator.Text = AppSettings.LoggerLogSeparator;
                else
                    txtLogSeparator.Text = ",";
                chart1.Legends[0].Docking = Docking.Top;
                /*
                if (string.IsNullOrEmpty(AppSettings.LoggerGraphicsLiveLastProfileFile))
                {
                    AppSettings.LoggerGraphicsLiveLastProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", "profile1.xml");
                    AppSettings.Save();
                }
                */

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
                chart1.MouseDown += Chart_MouseDown;
                chart1.MouseUp += Chart_MouseUp;
                chart2.MouseDown += Chart_MouseDown;
                chart2.MouseUp += Chart_MouseUp;
                chart1.MouseWheel += Chart_MouseWheel;
                chart2.MouseWheel += Chart_MouseWheel;
                chart1.PostPaint += Chart1_PostPaint;
                showPointsToolStripMenuItem.Checked = AppSettings.LoggerGraphicsShowPoints;
                disableResampleToolStripMenuItem.Checked = AppSettings.LoggerGraphDisableResample;
                chart1.MouseClick += Chart_MouseClick;
                chart2.MouseClick += Chart_MouseClick;
                loadCombobox1();
                dataGridPointValues.Columns.Add("Pid", "Pid");
                dataGridPointValues.Columns.Add("Value", "Value");
                numDisplayInterval.Value = AppSettings.LoggerGraphicsInterval;
                numShowMax.Value = AppSettings.LoggerGraphicsShowMaxTime;
                posTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                posTimer.Tick += PosTimer_Tick;
                txtShowFrom.KeyUp += TxtShowFrom_KeyUp;
                txtShowTo.KeyUp += TxtShowTo_KeyUp;
                if (LiveData)
                {
                    //SetupLiveGraphics();
                }
                else
                {
                    chkGetLiveData.Enabled = false;
                    numShowMax.Enabled = false;
                    chkShowLastSeconds.Enabled = false;
                    chkShowLastSeconds.Checked = false;
                }
                chart1.ChartAreas[0].BackColor = System.Drawing.ColorTranslator.FromHtml(AppSettings.LoggerGrapohicsBackColor);
                chart2.ChartAreas[0].BackColor = System.Drawing.ColorTranslator.FromHtml(AppSettings.LoggerGrapohicsBackColor);
                numLineWidth.Value = AppSettings.LoggerGraphicsLineWidth;
                comboColorPalette.DataSource = Enum.GetValues(typeof(ChartColorPalette));
                comboColorPalette.Text =  (AppSettings.LoggerGraphicsColorPalette).ToString();
                chart1.Palette = AppSettings.LoggerGraphicsColorPalette;
                chart2.Palette = AppSettings.LoggerGraphicsColorPalette;
                this.comboColorPalette.SelectedIndexChanged += new System.EventHandler(this.comboColorPalette_SelectedIndexChanged);
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

        private void Chart1_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (timerDisplayData.Enabled)
            {
                chart1.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum +
                (chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum - chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum) / 2);
            }
            UpdateChart2();
        }

        private void TabZoom_Enter(object sender, EventArgs e)
        {
            SetupGraphProperties(chart2);
            UpdateChart2();
        }

        private void PosTimer_Tick(object sender, EventArgs e)
        {
            posTimer.Stop();
            int diff = graphSettings.ZoomEndPoint - graphSettings.ZoomStartPoint;
            graphSettings.ZoomStartPoint = scrollPosition.Value - diff/2;
            graphSettings.ZoomEndPoint = graphSettings.ZoomStartPoint + diff;
            SetScrollValues();
            ShowSelectedRange();
        }

        private void TxtShowFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    DateTime fDT = new DateTime(graphSettings.FirstSampleTime);
                    string tStampDate = fDT.ToString("yyyy:MM:dd.");
                    TimestampFormat TSF = new TimestampFormat(fDT);
                    graphSettings.ZoomStartTime = TSF.ConvertTimeStamp(txtShowFrom.Text).Ticks;
                    SetScrollValues();
                    ShowSelectedRange();
                }
            }
            catch { }
        }


        private void TxtShowTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    DateTime fDT = new DateTime(graphSettings.FirstSampleTime);
                    string tStampDate = fDT.ToString("yyyy:MM:dd.");
                    TimestampFormat TSF = new TimestampFormat(fDT);
                    graphSettings.ZoomEndTime = TSF.ConvertTimeStamp(txtShowTo.Text).Ticks;
                    SetScrollValues();
                    ShowSelectedRange();
                }
            }
            catch { }
        }


        private void TabAdvanced_Enter(object sender, EventArgs e)
        {
            ShowAdvSettings();
        }

        private void ShowAdvSettings()
        {
            try
            {
                if (dataGridViewAdv.Columns.Count < 2)
                {
                    dataGridViewAdv.Columns.Add("Setting", "Setting");
                    dataGridViewAdv.Columns.Add("Value", "Value");
                }
                dataGridViewAdv.Rows.Clear();
                foreach (PropertyInfo prop in graphSettings.GetType().GetProperties())
                {
                    int row = dataGridViewAdv.Rows.Add();
                    dataGridViewAdv.Rows[row].Cells[0].Value = prop.Name;
                    if (prop.Name.ToLower().Contains("time"))
                    {
                        long ticks = (long)prop.GetValue(graphSettings, null);
                        dataGridViewAdv.Rows[row].Cells[1].Value = new DateTime(ticks).ToString("HH:mm:ss:ffff");
                    }
                    else
                    {
                        dataGridViewAdv.Rows[row].Cells[1].Value = prop.GetValue(graphSettings, null);
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

        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                Chart chart = (Chart)sender;
                if (e.Delta < 0)
                {
                    //chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    if (wheelZoomYToolStripMenuItem1.Checked || wheelZoomXYToolStripMenuItem1.Checked)
                    {
                        chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    }
                    if (wheelZoomXToolStripMenuItem1.Checked || wheelZoomXYToolStripMenuItem1.Checked)
                    {
                        if (graphSettings.zoomStarts.Count > 1)
                        {
                            graphSettings.zoomStarts.RemoveAt(graphSettings.zoomStarts.Count - 1);
                            graphSettings.ZoomStartTime = graphSettings.zoomStarts.Last();
                            graphSettings.zoomEnds.RemoveAt(graphSettings.zoomEnds.Count - 1);
                            graphSettings.ZoomEndTime = graphSettings.zoomEnds.Last();
                            btnPlay.Enabled = true;
                        }
                        else
                        {
                            ResetZoom(false);
                        }
                        ShowSelectedRange();
                    }
                }

                if (e.Delta > 0)
                {
                    double xMin = chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    double posYStart = chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    double posYFinish = chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    if (wheelZoomYToolStripMenuItem1.Checked || wheelZoomXYToolStripMenuItem1.Checked)
                    {
                        chart1.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                    }

                    if (wheelZoomXToolStripMenuItem1.Checked || wheelZoomXYToolStripMenuItem1.Checked)
                    {
                        graphSettings.ZoomStartTime = DateTime.FromOADate(posXStart).Ticks;
                        graphSettings.ZoomEndTime =  DateTime.FromOADate(posXFinish).Ticks;
                        graphSettings.zoomStarts.Add(graphSettings.ZoomStartTime);
                        graphSettings.zoomEnds.Add(graphSettings.ZoomEndTime);
                        ShowSelectedRange();
                        //chart.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    }
                }
                SetScrollValues();
            }
            catch { }
        }

        private void Chart_MouseDown(object sender, MouseEventArgs e)
        {
            ChartMouseDownLocation = e.Location;
        }

        private void SetScrollValues()
        {
            try
            {
                if (graphSettings.ZoomStartPoint == 0 && graphSettings.ZoomEndPoint == graphSettings.SampleCount)
                {
                    scrollPosition.Maximum = 0;
                }
                else
                {
                    int diff = graphSettings.ZoomEndPoint - graphSettings.ZoomStartPoint;
                    scrollPosition.Maximum = (int)(graphSettings.SampleCount - diff / 2) + 1;
                    scrollPosition.Minimum = diff / 2;
                    scrollPosition.Value = graphSettings.ZoomStartPoint + diff / 2;
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
        private void Chart_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (Math.Abs(e.Location.X - ChartMouseDownLocation.X) < 10 && Math.Abs(e.Location.Y - ChartMouseDownLocation.Y) < 10)
                {                    
                    return;
                }
                Chart chart = (Chart)sender;
                ChartArea ChartSelectionArea = chart.ChartAreas[0];
                ChartArea ChartSelectionArea1 = chart1.ChartAreas[0];
                Debug.WriteLine("Chart selection: " + ChartSelectionArea.CursorY.SelectionStart + " - " + ChartSelectionArea.CursorY.SelectionEnd);
                long tmpStart = DateTime.FromOADate(ChartSelectionArea.CursorX.SelectionStart).Ticks;
                long tmpEnd = DateTime.FromOADate(ChartSelectionArea.CursorX.SelectionEnd).Ticks;
                if (tmpStart < (long)graphSettings.FirstSampleTime || tmpStart > (long)graphSettings.LastSampleTime
                    || tmpEnd < (long)graphSettings.FirstSampleTime || tmpEnd > (long)graphSettings.LastSampleTime)
                {
                    Debug.WriteLine("Chart Zoom Selection out of range");
                    return;
                }

                if (zoomXToolStripMenuItem.Checked || zoomXYToolStripMenuItem.Checked)
                {
                    if (Math.Abs(e.Location.X - ChartMouseDownLocation.X) > 10)
                    {
                        graphSettings.ZoomStartTime = tmpStart;
                        graphSettings.ZoomEndTime = tmpEnd;
                        if (graphSettings.ZoomStartTime > graphSettings.ZoomEndTime)
                        {
                            long tmp = graphSettings.ZoomStartTime;
                            graphSettings.ZoomStartTime = graphSettings.ZoomEndTime;
                            graphSettings.ZoomEndTime = tmp;
                        }
                        graphSettings.zoomStarts.Add(graphSettings.ZoomStartTime);
                        graphSettings.zoomEnds.Add(graphSettings.ZoomEndTime);
                        btnPlay.Enabled = true;
                        Debug.WriteLine("Zoom From: " + new DateTime(graphSettings.ZoomStartTime).ToString("mm.ss.ffff") + " to: " + new DateTime(graphSettings.ZoomEndTime).ToString("mm.ss.ffff"));
                        ShowSelectedRange();
                        /*
                            ChartSelectionArea.AxisX.ScaleView.Zoom(
                                Math.Min(ChartSelectionArea.CursorX.SelectionStart, ChartSelectionArea.CursorX.SelectionEnd),
                                Math.Max(ChartSelectionArea.CursorX.SelectionStart, ChartSelectionArea.CursorX.SelectionEnd)
                            );
                        */
                    }
                }
                if (zoomYToolStripMenuItem.Checked || zoomXYToolStripMenuItem.Checked)
                {
                    if (Math.Abs(e.Location.Y - ChartMouseDownLocation.Y) > 10)
                    {
                        //Zoom chart1, even when selection in chart2
                        ChartSelectionArea1.AxisY.ScaleView.Zoom(
                            Math.Min(ChartSelectionArea.CursorY.SelectionStart, ChartSelectionArea.CursorY.SelectionEnd),
                            Math.Max(ChartSelectionArea.CursorY.SelectionStart, ChartSelectionArea.CursorY.SelectionEnd)
                        );
                    }
                }
                // Reset/hide the selection rectangle
                ChartSelectionArea1.CursorX.SetSelectionPosition(0D, 0D);
                ChartSelectionArea1.CursorY.SetSelectionPosition(0D, 0D);
                SetScrollValues();
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
                            //ScrollPointsPerScreen.Value--;
                            if (graphSettings.ZoomStartPoint >0)
                                graphSettings.ZoomStartPoint--;
                            if (graphSettings.ZoomEndPoint<graphSettings.SampleCount-1)
                                graphSettings.ZoomEndPoint++;
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
                            if (scrollPosition.Value < scrollPosition.Maximum)
                            scrollPosition.Value++;
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
                            //ScrollPointsPerScreen.Value++;
                            if (graphSettings.ZoomStartPoint < graphSettings.SampleCount -10)
                                graphSettings.ZoomStartPoint++;
                            if (graphSettings.ZoomEndPoint > graphSettings.ZoomStartPoint +10 )
                                graphSettings.ZoomEndPoint--;
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
                            if (scrollPosition.Value > scrollPosition.Minimum +1)
                                scrollPosition.Value--;
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
                long xvdt = DateTime.FromOADate(chart1.ChartAreas[0].CursorX.Position).Ticks;
                PointDataGroup pdg = graphSettings.pointDataGroups.Where(x => x.pointDatas[0].TimeStamp >= xvdt).FirstOrDefault();
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


        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Point pos = e.Location;

                if (e.Button == MouseButtons.Right)
                {
                    //chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                    //chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
                    ResetZoom(true);
                    return;
                }

                Chart chart = (Chart)sender;
                HitTestResult[] results = chart.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
                double xv = chart.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                double yv = chart.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
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
                        Series S = chart.Series[0];            // short reference
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
                        for (int s = 0; s < chart.Series.Count; s++)
                        {
                            PointData pData = graphSettings.pointDataGroups[pd.Row].pointDatas[s];
                            int r = dataGridPointValues.Rows.Add();
                            if (sb.Length < 2040)
                                sb.Append(" " + pData.PidName + ": " + pData.Value.ToString("0.00") + ",");
                            dataGridPointValues.Rows[r].Cells["Pid"].Value = pData.PidName;
                            dataGridPointValues.Rows[r].Cells["Value"].Value = pData.Value.ToString("0.00");
                            UpdateVbarValue(s, pData.Value);
                        }
                        labelDataValues.Text = sb.ToString().Trim(',');
                    }
                    if (chart.Name == "chart2")
                        chart1.ChartAreas[0].CursorX.SetCursorPosition(xv);
                    else
                        chart2.ChartAreas[0].CursorX.SetCursorPosition(xv);
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

        private void SetupGraphProperties(Chart chart)
        {
            chart.BackGradientStyle = GradientStyle.None;
            
            ChartArea CA = chart.ChartAreas[0];  // quick reference            
            CA.Area3DStyle.Enable3D = false;
            //CA.AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            CA.CursorX.Interval = 0;
            //CA.AxisX.IntervalOffsetType = DateTimeIntervalType.Milliseconds;
            CA.AxisX.LabelStyle.Format = "HH:mm:ss";
            CA.CursorX.AutoScroll = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            CA.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.None;
            CA.AxisX.MajorGrid.Enabled = false;
            CA.AxisX.MinorTickMark.Enabled = false;
            //CA.AxisX.Crossing = 0;

            CA.CursorY.IsUserSelectionEnabled = true;
            CA.AxisY.MajorGrid.Enabled = false;
            CA.AxisY.MinorTickMark.Enabled = false;
            CA.AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
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
                chart2.Series.Clear();
                for (int r = 0; r < datalogger.SelectedPids.Count; r++)
                {
                    LogParam.PidSettings pidProfile = datalogger.SelectedPids[r];
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    PidScalar ps = new PidScalar(parm.Name, parm.Id);
                    pidScalars.Add(ps);
                    chart1.Series.Add(new Series());
                    chart1.Series[r].ChartType = ChartType;
                    //chart1.Series[r].BorderDashStyle = ChartDashStyle.DashDotDot;
                    chart1.Series[r].BorderWidth = AppSettings.LoggerGraphicsLineWidth;
                    chart1.Series[r].XValueType = ChartValueType.DateTime;
                    if (parm.Name != null)
                        chart1.Series[r].Name = r.ToString() + "-" +  parm.Name;
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
            graphSettings.pointDataGroups = new List<PointDataGroup>();
            ImportPidProfile();
            
            SetupGraphProperties(chart1);
            groupLiveSeconds.Enabled = true;
/*            if (!string.IsNullOrEmpty(AppSettings.LoggerGraphicsLiveLastProfileFile) && File.Exists(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile))))
            {
                ProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile));
                LoadProfile();
            }
*/
            chkGetLiveData.Checked = true;
            loadLogfileToolStripMenuItem.Enabled = false;
            loadLastLogfileToolStripMenuItem.Enabled = false;
            chkShowLastSeconds.Enabled = true;
            chkShowLastSeconds.Checked = true;
        }

        private void LogEvents_LogDataAdded(object sender, DataLogger.LogDataEvents.LogDataEvent e)
        {
            QueueliveData(e.Data);
        }

        public void UpdateLiveGraphics()
        {
            chart1.Series.Clear();
            chart2.Series.Clear();
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
                    PointData pd = new PointData(pidScalars[p].PidId, pidScalars[p].Pid, ld.TimeStamp, orgVal, scaledVal, graphSettings.SampleCount-1);
                    pdg.pointDatas[p] = pd;
                }
                graphSettings.pointDataGroups.Add(pdg);
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
            chart2.Series.Clear();
            SetupGraphProperties(chart1);
            //TStamps = 0;
            SeriesChartType ct = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), comboBox1.Text);
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
            //ScrollPointsPerScreen.Enabled = true;
            scrollPosition.Maximum = 0;
            scrollPosition.Value = 0;
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
                for (int pd = 0; pd < graphSettings.SampleCount; pd++)
                {
                    for (int p = 0; p < pidScalars.Count; p++)
                    {
                        if (graphSettings.pointDataGroups[pd].pointDatas[p].Value > double.MinValue)
                            graphSettings.pointDataGroups[pd].pointDatas[p].ScaledValue = pidScalars[p].Scalar * graphSettings.pointDataGroups[pd].pointDatas[p].Value;
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
                //PlayBack = false;
                datalogger.LoadLogFile(logFile);
                ImportPidProfile();
                DetectMinMax();
                ImportLogDataBuffer();
                if (datalogger.SelectedPids.Count > 10)
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

        private void ShowSelectedRange(bool setScrollValue = true)
        {
            try
            {
                int startP=0;
                int endP = graphSettings.SampleCount;
                List<PointDataGroup> pValues = graphSettings.pointDataGroups;

                if (graphSettings.ZoomStartTime > 0)
                {
                    txtShowTo.Text = new DateTime(graphSettings.ZoomEndTime).ToString("HH:mm:ss.ffff");
                    txtShowFrom.Text = new DateTime(graphSettings.ZoomStartTime).ToString("HH:mm:ss.ffff");
                }
                if (graphSettings.SampleCount == 0)
                {
                    return;
                }
                if (graphSettings.ZoomAreaLength > chart1.Width*2)
                {
                    pValues = ResampleData(graphSettings.ZoomStartPoint, graphSettings.ZoomEndPoint, chart1);
                    startP = 0;
                    endP = pValues.Count;
                }
                else
                {
                    pValues = graphSettings.pointDataGroups;
                    startP = graphSettings.ZoomStartPoint;
                    endP = graphSettings.ZoomEndPoint;
                }
                Stopwatch timer = new Stopwatch();
                timer.Start();
                for (int r=0;r< chart1.Series.Count; r++)
                {
                    chart1.Series[r].Points.SuspendUpdates();
                    chart1.Series[r].Points.Clear();
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
                for (int r = 0; r < chart1.Series.Count; r++)
                {
                    chart1.Series[r].Points.ResumeUpdates();
                }
                //Show vbars for last value:
                if (endP <= 0)
                {
                    Debug.WriteLine("Range end: " + endP.ToString());
                    return;
                }
                PointDataGroup pdg2 = pValues[endP-1];
                for (int r = 0; r < pdg2.pointDatas.Length; r++)
                {
                    UpdateVbarValue(r ,pdg2.pointDatas[r].ScaledValue);
                }
                if (tabControl1.SelectedTab == tabAdvanced)
                {
                    ShowAdvSettings();
                }
                if (setScrollValue)
                {
                    SetScrollValues();
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
                for (int r = 0; r < chart1.Series.Count; r++)
                {
                    chart1.Series[r].Points.ResumeUpdates();
                }
            }
        }

        private void UpdateChart2()
        {
            try
            {
                if (tabControl1.SelectedTab != tabZoom)
                {
                    return;
                }

                if (chart2.Series.Count != chart1.Series.Count)
                {
                    chart2.Series.Clear();
                    for (int r = 0; r < chart1.Series.Count; r++)
                    {
                        chart2.Series.Add(new Series());
                        chart2.Series[r].ChartType = ChartType;
                        chart2.Series[r].XValueType = ChartValueType.DateTime;
                        chart2.Series[r].Name = chart1.Series[r].Name;
                        chart2.Series[r].Color = chart1.Series[r].Color;
                        chart2.Series[r].IsVisibleInLegend = false;
                    }
                }
                if (prevDataCount != graphSettings.SampleCount)
                {
                    prevDataCount = graphSettings.SampleCount;
                    List<PointDataGroup> pValues = graphSettings.pointDataGroups;
                    if (pValues.Count > (chart2.Width * 2))
                    {
                        pValues = ResampleData(0, graphSettings.SampleCount, chart2);
                    }

                    for (int r = 0; r < chart2.Series.Count; r++)
                    {
                        chart2.Series[r].Points.SuspendUpdates();
                        chart2.Series[r].Points.Clear();
                    }
                    for (int x = 0; x < pValues.Count; x++)
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
                                //point.ToolTip = string.Format("[{0}] {1}: {2}", tStampStr, pd.PidName, pd.Value);
                                if (showPointsToolStripMenuItem.Checked)
                                    point.MarkerStyle = MarkerStyle.Circle;
                                else
                                    point.MarkerStyle = MarkerStyle.None;
                                chart2.Series[r].Points.Add(point);
                            }
                        }
                    }
                }
                ChartArea ChartSelectionArea2 = chart2.ChartAreas[0];
                //ChartSelectionArea2.CursorX.SetSelectionPosition(chart1.ChartAreas[0].AxisX.Minimum,chart1.ChartAreas[0].AxisX.Maximum);
                //ChartSelectionArea2.CursorY.SetSelectionPosition(chart1.ChartAreas[0].AxisY.Minimum, chart1.ChartAreas[0].AxisY.Maximum);
                ChartSelectionArea2.CursorX.SetSelectionPosition(chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum, chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum);
                ChartSelectionArea2.CursorY.SetSelectionPosition(chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum, chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum);
                if (cursorXToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
                {
                    chart2.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].CursorX.Position);
                }
                if (cursorYToolStripMenuItem.Checked || cursorXYToolStripMenuItem.Checked)
                {
                    chart2.ChartAreas[0].CursorY.SetCursorPosition(chart1.ChartAreas[0].CursorY.Position);
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
            finally
            {
                for (int r = 0; r < chart2.Series.Count; r++)
                {
                    chart2.Series[r].Points.ResumeUpdates();
                }
            }
        }

        private void LoadLogFile()
        {
            try
            {
                this.Text = Title + " [" + Path.GetFileName(ProfileFile) + "]";
                labelDataValues.Text = "Click data point to show values";
                chart1.Series.Clear();
                chart2.Series.Clear();
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
                ResetZoom(true);
                //LoadProfile();
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
                //double loc = (chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum) / 2;
                //double loc = (chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum - chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum) / 2;
                
                if (PlayBack)
                {
                    //Move forward:
                    graphSettings.Forward((long)(numDisplayInterval.Value * numPlaybackSpeed.Value * 1000));
                    if (graphSettings.ZoomEndPoint >= graphSettings.SampleCount - 1)
                    {
                        timerDisplayData.Enabled = false;
                        chkPlaying.Checked = false;
                    }

                }
                else if (LiveData)
                {
                    if (chkShowLastSeconds.Checked)
                    {
                        if (graphSettings.LastSampleTime > 0)
                        {
                            long startT = new DateTime(graphSettings.LastSampleTime).AddMilliseconds((double)(-1 * numShowMax.Value * 1000)).Ticks;
                            graphSettings.ZoomStartTime = startT;
                            graphSettings.ZoomEndPoint = graphSettings.SampleCount - 1;
                        }
                    }
                    else
                    {
                        graphSettings.ZoomEndPoint = graphSettings.SampleCount - 1;
                    }
                }

                ShowSelectedRange();
                //chart1.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum + loc);
                //chart1.ChartAreas[0].CursorX.SetCursorPosition(chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 
                  //  (chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum - chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum) / 2);
                ShowCurrentValues();
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
            string fName = SelectFile("Select profilefile", DisplayProfileFilter, def);
            if (string.IsNullOrEmpty(fName))
                return;
            ProfileFile = fName;
            LoadProfile();
        }

        private void SaveProfileAs()
        {
            string def = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(AppSettings.LoggerGraphicsLiveLastProfileFile));
            string fName = SelectSaveFile(DisplayProfileFilter, def);
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
            groupPlayback.Enabled = true;
            LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
        }

        public void StartLiveUpdate()
        {
            ResetZoom(true);
            this.PlayBack = false;
            chkPlaying.Checked = true;
            timerDisplayData.Enabled = true;
            groupPlayback.Enabled = false;
            LoggerDataEvents.LogDataAdded += LogEvents_LogDataAdded;
        }

        private void chkGetLiveData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGetLiveData.Checked )
            {
                SetupLiveGraphics();
                //ImportLogDataBuffer();
                StartLiveUpdate();
            }
            if (!chkGetLiveData.Checked && LiveData)
            {
                StopLiveUpdate();
                ShowSelectedRange();
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

        private List<PointDataGroup> ResampleData(int StartPoint, int EndPoint, Chart chart)
        {
            List<PointDataGroup> retVal = new List<PointDataGroup>();
            try
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();

                int samplesize = (int)Math.Ceiling((decimal)(EndPoint - StartPoint)/chart.Width);
                if (samplesize < 2)
                {
                    Debug.WriteLine("No resampling, too small range");
                    for (int p=StartPoint; p < EndPoint; p++)
                    {
                        retVal.Add(graphSettings.pointDataGroups[p]);
                    }
                    return retVal;
                }

                int pidcount = graphSettings.pointDataGroups[0].pointDatas.Length;
                Debug.WriteLine("Resampling, samplesize: " + samplesize.ToString()+", Original range: " + (EndPoint-StartPoint).ToString()+ ", Chart width: " + chart.Width.ToString());
                for (int pos = StartPoint; (pos + samplesize) < EndPoint; pos += samplesize)
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
                            PointData pd = graphSettings.pointDataGroups[pos + x].pointDatas[pid];
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
                            pdgmin.pointDatas[pid] = graphSettings.pointDataGroups[maxrow].pointDatas[pid];
                            pdgmax.pointDatas[pid] = graphSettings.pointDataGroups[minrow].pointDatas[pid];
                        }
                        else
                        {
                            pdgmin.pointDatas[pid] = graphSettings.pointDataGroups[minrow].pointDatas[pid];
                            pdgmax.pointDatas[pid] = graphSettings.pointDataGroups[maxrow].pointDatas[pid];
                        }
                    }
                    retVal.Add(pdgmin);
                    retVal.Add(pdgmax);
                }
                Debug.WriteLine("Resampled size: " + (retVal.Count / 2).ToString());
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
                for (int p = 0; p < graphSettings.SampleCount; p++)
                {
                    if (Math.Abs(graphSettings.pointDataGroups[p].pointDatas[pid].Value) > max)
                    {
                        max = Math.Abs(graphSettings.pointDataGroups[p].pointDatas[pid].Value);
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
            SetupGraphProperties(chart1);
        }

        private void zoomYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = false;
            zoomYToolStripMenuItem.Checked = true;
            zoomXYToolStripMenuItem.Checked = false;
            noZoomToolStripMenuItem.Checked = false;
            SetupGraphProperties(chart1);

        }

        private void zoomXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = false;
            zoomYToolStripMenuItem.Checked = false;
            zoomXYToolStripMenuItem.Checked = true;
            noZoomToolStripMenuItem.Checked = false;
            SetupGraphProperties(chart1);

        }

        private void cusrorXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = true;
            cursorYToolStripMenuItem.Checked = false;
            cursorXYToolStripMenuItem.Checked = false;
            noCursorToolStripMenuItem.Checked = false;
            SetupGraphProperties(chart1);

        }

        private void cursorYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = false;
            cursorYToolStripMenuItem.Checked = true;
            cursorXYToolStripMenuItem.Checked = false;
            noCursorToolStripMenuItem.Checked = false;
            SetupGraphProperties(chart1);

        }

        private void cursorXYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = false;
            cursorYToolStripMenuItem.Checked = false;
            cursorXYToolStripMenuItem.Checked = true;
            noCursorToolStripMenuItem.Checked = false;
            SetupGraphProperties(chart1);
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetZoom(true);
        }

        private void noZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomXToolStripMenuItem.Checked = false;
            zoomYToolStripMenuItem.Checked = false;
            zoomXYToolStripMenuItem.Checked = false;
            noZoomToolStripMenuItem.Checked = true;
            SetupGraphProperties(chart1);
        }

        private void noCursorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cursorXToolStripMenuItem.Checked = false;
            cursorYToolStripMenuItem.Checked = false;
            cursorXYToolStripMenuItem.Checked = false;
            noCursorToolStripMenuItem.Checked = true;
            SetupGraphProperties(chart1);
        }

        private void ResetZoom(bool ResetY)
        {
            if (ResetY)
            {
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);
            }
            //timerDisplayData.Enabled = false;
            btnPlay.Enabled = false;
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
            graphSettings.ZoomStartPoint = 0;
            graphSettings.ZoomEndTime = graphSettings.LastSampleTime;
            graphSettings.zoomStarts.Clear();
            graphSettings.zoomEnds.Clear();
            scrollPosition.Maximum = 0;
            scrollPosition.Value = 0;
            ShowSelectedRange();

        }
        private void ImportLogDataBuffer()
        {
            try
            {
                graphSettings.pointDataGroups = new List<PointDataGroup>();
                for (int i = 0; i < datalogger.LogDataBuffer.Count; i++)
                {
                    LogData ld = datalogger.LogDataBuffer[i];
                    PointDataGroup pdg = new PointDataGroup(ld.CalculatedValues.Length);
                    for (int p = 0; p < ld.Values.Length; p++)
                    {
                        double orgVal = ld.CalculatedValues[p];
                        double scaledVal = orgVal;
                        PointData pd = new PointData(pidScalars[p].PidId, pidScalars[p].Pid, ld.TimeStamp, orgVal, scaledVal, graphSettings.SampleCount - 1);
                        pdg.pointDatas[p] = pd;
                    }
                    graphSettings.pointDataGroups.Add(pdg);
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

        /*
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
            ShowSelectedRange();
        }
        */
        private void numPlaybackSpeed_ValueChanged(object sender, EventArgs e)
        {
            
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayBack = true;
            groupLiveSeconds.Enabled = true;
            //StartLiveUpdate(true);
            timerDisplayData.Enabled = true;
            chkPlaying.Checked = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            StopLiveUpdate();
            timerDisplayData.Enabled = false;
            chkPlaying.Checked = false;
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

        private void scrollPosition_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                //if (posTimer.IsEnabled)
                //posTimer.Stop();
                //posTimer.Start();
                int diff = graphSettings.ZoomEndPoint - graphSettings.ZoomStartPoint;
                graphSettings.ZoomStartPoint = scrollPosition.Value - diff / 2;
                graphSettings.ZoomEndPoint = graphSettings.ZoomStartPoint + diff;
                SetScrollValues();
                ShowSelectedRange(false);

                DateTime dt = new DateTime((long)graphSettings.pointDataGroups[scrollPosition.Value].pointDatas[0].TimeStamp);
                ShowToolTip(dt.ToString("HH:mm:ss.ffff"));
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

        private void chkPlaying_CheckedChanged(object sender, EventArgs e)
        {
            timerDisplayData.Enabled = chkPlaying.Checked;
            if (chkPlaying.Enabled)
            {
                PlayBack = true;
                groupLiveSeconds.Enabled = true;
            }
        }

        private void chkShowLastSeconds_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowLastSeconds.Checked)
            {
                graphSettings.ZoomStartPoint = 0;
                graphSettings.ZoomEndPoint = graphSettings.SampleCount - 1;
            }
            ShowSelectedRange();
        }

        private void btnBackgroundColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = chart1.ChartAreas[0].BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                chart1.ChartAreas[0].BackColor = colorDialog1.Color;
                chart2.ChartAreas[0].BackColor = colorDialog1.Color;
            }
            AppSettings.LoggerGrapohicsBackColor = System.Drawing.ColorTranslator.ToHtml(colorDialog1.Color);
            AppSettings.Save();
        }

        private void numLineWidth_ValueChanged(object sender, EventArgs e)
        {
            for (int x=0; x<chart1.Series.Count;x++)
            {
                chart1.Series[x].BorderWidth = (int)numLineWidth.Value;
            }
            AppSettings.LoggerGraphicsLineWidth = (int)numLineWidth.Value;
            AppSettings.Save();
        }

        private void comboColorPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerGraphicsColorPalette = (ChartColorPalette)comboColorPalette.SelectedItem;
            chart1.Palette = (ChartColorPalette)comboColorPalette.SelectedItem;
            chart2.Palette = (ChartColorPalette)comboColorPalette.SelectedItem;
            AppSettings.Save();
        }

        private void chkBarAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < pidScalars.Count; i++)
            {
                pidScalars[i].Bar = chkBarAll.Checked;
            }
            this.Refresh();

        }
    }
}
