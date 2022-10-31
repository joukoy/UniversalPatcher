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
                On = true;
                Pid = "";
                Scalar = 1;
            }
            public PidScalar(string pid)
            {
                On = true;
                Pid = pid;
                Scalar = 1;
            }
            public bool On { get; set; }
            public string Pid { get; set; }
            public float Scalar { get; set; }
        }

        private class QueuePoint
        {
            public QueuePoint(DataPoint Point, int Serie)
            {
                this.Point = Point;
                this.Serie = Serie;
            }
            public DataPoint Point { get; set; }
            public int Serie { get; set; }
        }

        private class PointData
        {
            public PointData(string Pid, ulong TStamp, double Val, double ScaledVal, int row)
            {
                PidName = Pid;
                TimeStamp = TStamp;
                Value = Val;
                ScaledValue = ScaledVal;
                Row = row;
            }
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
        private int TStamps = 1;
        private Queue<QueuePoint> PointQ = new Queue<QueuePoint>();
        private string ProfileFile;
        private string Title;
        private List<PointDataGroup> pointDatas = new List<PointDataGroup>();
        ToolTip ScrollTip = new ToolTip();
        //public string LastLiveLogFile;
        private List<string> dataSourceNames = new List<string>();
        private List<Bitmap> dataSourceImage = new List<Bitmap>();
        private SeriesChartType ChartType = SeriesChartType.Line;

        private void frmLoggerGraphics_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLogSeparator))
                txtLogSeparator.Text = Properties.Settings.Default.LoggerLogSeparator;
            else
                txtLogSeparator.Text = ",";
            chart1.Legends[0].Docking = Docking.Top;
            if (string.IsNullOrEmpty(Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile))
            {
                Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile = Path.Combine(Application.StartupPath,"Logger", "DisplayProfiles", Path.GetFileName( Properties.Settings.Default.LoggerLastProfile));
                Properties.Settings.Default.Save();
            }
            if (string.IsNullOrEmpty(Properties.Settings.Default.LoggerGraphicsLogLastProfileFile))
            {
                Properties.Settings.Default.LoggerGraphicsLogLastProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(Properties.Settings.Default.LoggerLastProfile));
                Properties.Settings.Default.Save();
            }

            this.FormClosing += FrmLoggerGraphics_FormClosing;
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            numDisplayInterval.Value = Properties.Settings.Default.LoggerGraphicsInterval;
            numShowMax.Value = Properties.Settings.Default.LoggerGraphicsShowMaxTime;
            chkShowPoints.Checked = Properties.Settings.Default.LoggerGraphicsShowPoints;
            SetUpDoubleBuffer(this);
            chart1.MouseClick += Chart1_MouseClick;
            loadCombobox1();
        }

        private void Chart1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.Location;
                var results = chart1.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
                foreach (var result in results)
                {
                    if (result.ChartElementType == ChartElementType.DataPoint)
                    {
                        DataPoint prop = result.Object as DataPoint;
                        if (prop != null)
                        {
                            PointData pd = (PointData)prop.Tag;                           
                            DateTime dt = new DateTime((long)pd.TimeStamp);
                            StringBuilder sb = new StringBuilder("[" + dt.ToString("HH:mm:ss.ffff") + "] ");
                            DataPoint[] points;
                            for (int s = 0; s < chart1.Series.Count; s++)
                            {
                                if (LiveData)
                                    points = chart1.Series[s].Points.Where(X => ((PointData)X.Tag).TimeStamp == pd.TimeStamp).ToArray();
                                else
                                    points = chart1.Series[s].Points.Where(X => ((PointData)X.Tag).Row == pd.Row).ToArray();
                                foreach (DataPoint point in points)
                                {
                                    PointData pData = (PointData)point.Tag;
                                    Debug.WriteLine(pData.PidName + ": " + pData.Value.ToString("0.00"));
                                    sb.Append(" " + pData.PidName + ": " + pData.Value.ToString("0.00") + ",");
                                }
                            }
                            labelDataValues.Text = sb.ToString().Trim(',');
                        }
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
            Properties.Settings.Default.LoggerGraphicsInterval = (int)numDisplayInterval.Value;
            Properties.Settings.Default.LoggerGraphicsShowMaxTime = (int)numShowMax.Value;
            Properties.Settings.Default.LoggerGraphicsShowPoints = chkShowPoints.Checked;
            Properties.Settings.Default.Save();
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
            if (LiveData)
                UpdateLiveGraphics();
            else
                UpdateLogGraphics();
        }

        public void SetupLiveGraphics()
        {
            Title = "Logger Graph";
            pidScalars = new List<PidScalar>();
            chart1.Series.Clear();
            chart1.BackGradientStyle = GradientStyle.None;
            chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
            for (int p = 0; p < datalogger.PidProfile.Count; p++)
            {
                PidScalar ps = new PidScalar(datalogger.PidProfile[p].PidName);
                pidScalars.Add(ps);
                chart1.Series.Add(new Series());
                chart1.Series[p].ChartType = ChartType;
                //chart1.Series[p].ChartType = SeriesChartType.Line;
                //chart1.Series[p].XValueType = ChartValueType.DateTime;
                if (datalogger.PidProfile[p].PidName != null)
                    chart1.Series[p].Name = datalogger.PidProfile[p].PidName;
                chart1.Series[p].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
            }
            dataGridValues.DataSource = pidScalars;
            dataGridValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridValues.Columns["On"].Width = 30;
            //btnApply.Text = "Apply";
            //dataGridValues.CellValueChanged += DataGridValues_CellValueChanged;
            LiveData = true;
            groupLiveSeconds.Enabled = true;
            if (File.Exists(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile))))
            {
                ProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles",Path.GetFileName( Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile));
                LoadProfile();
            }
            ChartArea CA = chart1.ChartAreas[0];  // quick reference
            CA.AxisX.ScaleView.Zoomable = true;
            CA.CursorX.AutoScroll = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            CA.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
            CA.AxisY.MajorGrid.Enabled = false;
            CA.AxisY.MinorTickMark.Enabled = false;
            CA.AxisY.Crossing = 0;
            chkGetLiveData.Checked = true;
            LoggerDataEvents.LogDataAdded += LogEvents_LogDataAdded;
            ScrollStartPoint.Visible = false;
            ScrollPointsPerScreen.Enabled = false;

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
                if (pidScalars[p].On)
                {
                    chart1.Series[p].ChartType = ChartType;
                    //chart1.Series[r].XValueType = ChartValueType.DateTime;
                    if (datalogger.PidProfile[p].PidName != null)
                        chart1.Series[p].Name = datalogger.PidProfile[p].PidName;
                    chart1.Series[p].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                }
                else
                {
                    chart1.Series[p].IsVisibleInLegend = false;
                }
            }
            dataGridValues.DataSource = null;
            dataGridValues.DataSource = pidScalars;
            dataGridValues.Update();
            dataGridValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void QueueliveData(LogData ld)
        {
            try
            {
                string dNowStr = new DateTime((long)ld.TimeStamp).ToString("HH:mm:ss.ffff");
                DateTime dNow = new DateTime((long)ld.TimeStamp);
                for (int p = 0; p < ld.Values.Length; p++)
                {
                    if (pidScalars[p].On)
                    {
                        double orgVal = ld.CalculatedValues[p];
                        if (orgVal == double.MinValue || orgVal == double.MaxValue)
                        {
                            orgVal = 0;
                        }
                        double scaledVal = orgVal * pidScalars[p].Scalar;
                        //Debug.WriteLine(dNow + " - " + val.ToString());
                        //chart1.Series[p].Points.AddXY(DateTime.Now.ToString("HH:mm:ss"), val);
                        DataPoint point = new DataPoint();
                        point.SetValueXY(dNowStr, scaledVal);
                        point.ToolTip = string.Format("[{0}] {1}: {2}", dNowStr, pidScalars[p].Pid, orgVal);
                        //point.Name = pidScalars[p].Pid + "$" + dNowStr;
                        PointData pd = new PointData(pidScalars[p].Pid, ld.TimeStamp, orgVal, scaledVal,0);
                        point.Tag = pd;
                        if (chkShowPoints.Checked)
                            point.MarkerStyle = MarkerStyle.Circle;
                        else
                            point.MarkerStyle = MarkerStyle.None;
                        QueuePoint qp = new QueuePoint(point,p);
                        lock (PointQ)
                        {
                            PointQ.Enqueue(qp);
                        }
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
            chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            TStamps = 0;
            for (int i = 0; i < header.Length; i++)
            {
                if (header[i].ToLower().Contains("time"))
                {
                    TStamps++;
                }
                else
                {
                    break;
                }
            }
            Logger("Using column: '" + header[0] + "' as X (time)");
            if (TStamps > 1 )
            {
                string cols = "";
                for (int i=1;i<TStamps;i++)
                {
                    cols += "'" + header[i] + "',";
                }
                cols = cols.Trim(',');
                Logger("Skipping columns: " + cols);
            }
            SeriesChartType ct = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), comboBox1.Text);
            for (int r = 0; r < header.Length-TStamps; r++)
            {
                PidScalar ps = new PidScalar(header[r + TStamps]);
                pidScalars.Add(ps);
                chart1.Series.Add(new Series());
                chart1.Series[r].ChartType = ChartType;
                //chart1.Series[r].XValueType = ChartValueType.DateTime;
                chart1.Series[r].Name = header[r + TStamps];
                chart1.Series[r].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
            }
            dataGridValues.DataSource = pidScalars;
            dataGridValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridValues.Columns["On"].Width = 30;
            //dataGridValues.CellValueChanged += DataGridValues_CellValueChanged;
            LiveData = false;
            timerDisplayData.Enabled = false;
            groupLiveSeconds.Enabled = false;
            //btnApply.Text = "Show data";
            if (File.Exists(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Properties.Settings.Default.LoggerGraphicsLogLastProfileFile)))
            {
                ProfileFile = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Properties.Settings.Default.LoggerGraphicsLogLastProfileFile);
                //LoadProfile();
                this.Text += " [" + Path.GetFileName(ProfileFile) + "]";
            }
            LoadProfile();
            //UpdateScalars();
            ChartArea CA = chart1.ChartAreas[0];  // quick reference
            CA.AxisX.ScaleView.Zoomable = false;
            CA.CursorX.AutoScroll = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            CA.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.None;
            CA.AxisY.MajorGrid.Enabled = false;
            CA.AxisY.MinorTickMark.Enabled = false;
            CA.AxisY.Crossing = 0;
            chkGetLiveData.Checked = false;
            LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
            ScrollStartPoint.Visible = true;
            ScrollPointsPerScreen.Enabled = true;
            ScrollStartPoint.Value = 0;
        }


        private void UpdateLogGraphics()
        {
            chart1.Series.Clear();
            for (int p = 0; p < pidScalars.Count; p++)
            {
                chart1.Series.Add(new Series());
                if (pidScalars[p].On)
                {
                    chart1.Series[p].ChartType = ChartType;
                    chart1.Series[p].XValueType = ChartValueType.DateTime;
                    chart1.Series[p].Name = pidScalars[p].Pid;
                    chart1.Series[p].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                }
                else
                {
                    chart1.Series[p].IsVisibleInLegend = false;
                }
            }
            dataGridValues.DataSource = pidScalars;
            dataGridValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void ReadDataFromFile()
        {
            try
            {
                if (string.IsNullOrEmpty(logFile))
                {
                    return;
                }
                Logger("Loading logfile: " + logFile, false);
                StreamReader sr = new StreamReader(logFile);
                string hdrLine = sr.ReadLine();
                for (int s = 0; s < chart1.Series.Count; s++)
                    chart1.Series[s].Points.Clear();
                string logLine;
                int row = 0;
                pointDatas = new List<PointDataGroup>();
                while ((logLine = sr.ReadLine()) != null)
                {
                    row++;
                    if (row % 1000 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                    string[] lParts = logLine.Split(new string[] { txtLogSeparator.Text }, StringSplitOptions.None);
                    PointDataGroup pdg = new PointDataGroup(lParts.Length - TStamps);
                    string tStampStr = lParts[0];
                    DateTime tStamp = Convert.ToDateTime(lParts[0]);
                    //LogData ld = new LogData(lParts.Length - TStamps);
                    ulong TimeStamp = (ulong)tStamp.Ticks;
                    for (int r = TStamps; r < lParts.Length && (r - TStamps) < pidScalars.Count; r++)
                    {
                        if (pidScalars[r - TStamps].On)
                        {
                            double origVal;
                            string valStr = lParts[r].Replace(",", ".");
                            if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out origVal))
                            {
                                double scaledVal = origVal * pidScalars[r - TStamps].Scalar;
                                //chart1.Series[r - 1].Points.AddXY(tStamp, val);
                                DataPoint point = new DataPoint();
                                PointData pd = new PointData(pidScalars[r - TStamps].Pid, TimeStamp, origVal, scaledVal,row);
                                pdg.pointDatas[r-TStamps] = pd;
                            }
                        }
                    }
                    pointDatas.Add(pdg);
                }
                sr.Close();
                ScrollStartPoint.Maximum = pointDatas.Count;
                ScrollPointsPerScreen.Maximum = pointDatas.Count;
                ShowSelectedRange();
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, ReadDataFromFile line " + line + ": " + ex.Message);
            }
        }

        private void ShowSelectedRange()
        {
            try
            {
            if (pointDatas.Count == 0)
            {
                return;
            }
            for (int s = 0; s < chart1.Series.Count; s++)
            {
                chart1.Series[s].Points.Clear();
            }
            int startP = ScrollStartPoint.Value;
            if ((ScrollStartPoint.Value + ScrollPointsPerScreen.Value) > pointDatas.Count)
            {
                startP = pointDatas.Count - ScrollPointsPerScreen.Value;
            }
            for (int x = startP; x < (ScrollStartPoint.Value + ScrollPointsPerScreen.Value) && x < pointDatas.Count; x++)
            {
                PointDataGroup pdg = pointDatas[x];
                for (int r = 0; r < pdg.pointDatas.Length; r++)
                {
                    PointData pd = pdg.pointDatas[r];
                    if (pd != null)
                    {
                        DataPoint point = new DataPoint();
                        point.Tag = pd;
                        DateTime tStamp = new DateTime((long)pd.TimeStamp);
                        string tStampStr = tStamp.ToString("HH:mm:ss.ffff");
                        point.SetValueXY(tStampStr, pd.ScaledValue);
                        point.ToolTip = string.Format("[{0}] {1}: {2}", tStampStr, pd.PidName, pd.Value);
                        //point.Name = pd.PidName + "$" + tStamp;
                        if (chkShowPoints.Checked)
                            point.MarkerStyle = MarkerStyle.Circle;
                        else
                            point.MarkerStyle = MarkerStyle.None;
                        chart1.Series[r].Points.Add(point);
                    }
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
                LoggerBold("Error, frmLoggerGraphics line " + line + ": " + ex.Message);
            }
        }

        private void LoadLogFile()
        {
            try
            {
                this.Text = Title + " [" + Path.GetFileName(ProfileFile) + "]";
                LiveData = false;
                StreamReader sr = new StreamReader(logFile);
                string hdrLine = sr.ReadLine();
                sr.Close();
                string[] hdrArray = hdrLine.Split(new string[] { txtLogSeparator.Text }, StringSplitOptions.None);
                SetupLogGraphics(hdrArray, Path.GetFileName(logFile));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LoadLogFile line " + line + ": " + ex.Message);
            }
        }

        private void loadLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectFile("Select Log file", CsvFilter);
                if (string.IsNullOrEmpty(fName))
                    return;
                logFile = fName;
                Title = "Logger Graph - " + fName;
                LoadLogFile();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, loadLogfileToolStripMenuItem_Click line " + line + ": " + ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void UpdateScalars()
        {
            if (pidScalars == null || pidScalars.Count == 0)
                return;
            if (LiveData)
            {
                UpdateLiveGraphics();
            }
            else
            {
                UpdateLogGraphics();
                ReadDataFromFile();
            }

        }

        private void btnShowData_Click(object sender, EventArgs e)
        {
            UpdateScalars();
        }

        private void CleanOldPoints()
        {
            try
            {
                int maxAge = (int)numShowMax.Value;
                for (; ; )
                {
                    PointData pd1 = (PointData)chart1.Series[0].Points[0].Tag;
                    DateTime dt1 = new DateTime((long)pd1.TimeStamp);
                    int pointAge = (int)DateTime.Now.Subtract(dt1).TotalSeconds;
                    if (pointAge > maxAge)
                    {
                        for (int p = 0; p < chart1.Series.Count; p++)
                        {
                            if (chart1.Series[p].Points.Count > 0)
                            {
                                PointData pt = (PointData)chart1.Series[p].Points[0].Tag;
                                DateTime dt = new DateTime((long)pt.TimeStamp);
                                pointAge = (int)DateTime.Now.Subtract(dt).TotalSeconds;
                                if (pointAge > maxAge)
                                {
                                    chart1.Series[p].Points.RemoveAt(0);
                                }
                            }
                        }
                    }
                    else
                    {
                        break; //No more old points
                    }
                }
                chart1.ResetAutoValues();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void timerDisplayData_Tick(object sender, EventArgs e)
        {
            try
            {
                CleanOldPoints();
                QueuePoint qp;
                while (PointQ.Count > 0)
                {

                    lock (PointQ)
                    {
                        qp = PointQ.Dequeue();
                    }
                    chart1.Series[qp.Serie].Points.Add(qp.Point);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LoggerGraphics line " + line + ": " + ex.Message);
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
                    Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile = Path.GetFileName(ProfileFile);
                else
                    Properties.Settings.Default.LoggerGraphicsLogLastProfileFile = Path.GetFileName(ProfileFile);
                Properties.Settings.Default.Save();
                this.Text = Title + " [" + Path.GetFileName(ProfileFile) + "]";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, SaveProfile line " + line + ": " + ex.Message);
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
                Logger("Loading profile: " + ProfileFile +"... ", false);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidScalar>));
                System.IO.StreamReader file = new System.IO.StreamReader(ProfileFile);
                List<PidScalar>  tmpScalars = (List<PidScalar>)reader.Deserialize(file);
                file.Close();

                if (tmpScalars.Count != pidScalars.Count)
                {
                    LoggerBold(" Profile not compatible");
                    ProfileFile = "";
                }
                else
                {
                    Logger(" [OK]");
                    pidScalars = tmpScalars;

                    if (LiveData)
                        Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile = Path.GetFileName(ProfileFile);
                    else
                        Properties.Settings.Default.LoggerGraphicsLogLastProfileFile = Path.GetFileName(ProfileFile);
                    Properties.Settings.Default.Save();
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
                LoggerBold("Error, LoadProfile line " + line + ": " + ex.Message);
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
            string def = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile));
            string fName = SelectFile("Select profilefile", XmlFilter, def);
            if (string.IsNullOrEmpty(fName))
                return;
            ProfileFile = fName;
            LoadProfile();
        }

        private void SaveProfileAs()
        {
            string def = Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles", Path.GetFileName(Properties.Settings.Default.LoggerGraphicsLiveLastProfileFile));
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
            logFile = Properties.Settings.Default.LoggerLastLogfile;
            Title = "Logger Graph - " + Properties.Settings.Default.LoggerLastLogfile;
            LoadLogFile();
        }

        public void StopLiveUpdate()
        {
            timerDisplayData.Enabled = false;
        }
        public void StartLiveUpdate()
        {
            timerDisplayData.Enabled = true;
        }

        private void chkGetLiveData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGetLiveData.Checked && !LiveData)
            {
                SetupLiveGraphics();
            }
            if (!chkGetLiveData.Checked && LiveData)
            {
                LiveData = false;
                LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
            }
        }

        private void ScrollPointsPerScreen_Scroll(object sender, ScrollEventArgs e)
        {
            ShowSelectedRange();
            ShowToolTip(ScrollPointsPerScreen.Value.ToString() + " pt/screen");
        }

        private void ScrollStartPoint_Scroll(object sender, ScrollEventArgs e)
        {
            ShowSelectedRange();
            DateTime dt = new DateTime((long)pointDatas[ScrollStartPoint.Value].pointDatas[0].TimeStamp);
            ShowToolTip(dt.ToString("HH:mm:ss.ffff"));
        }

        private void chkShowPoints_CheckedChanged(object sender, EventArgs e)
        {
            if (!LiveData)
            {
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
    }
}
