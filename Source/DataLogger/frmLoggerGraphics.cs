using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Helpers;
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

        private List<PidScalar> pidScalars;
        private string logFile;
        private bool LiveData;
        private int TStamps = 1;
        private Queue<QueuePoint> PointQ = new Queue<QueuePoint>();
        private string ProfileFile;
        private string Title;
        public string LastLiveLogFile;

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
            Title = "Logger Graphics";
            pidScalars = new List<PidScalar>();
            chart1.Series.Clear();
            chart1.BackGradientStyle = GradientStyle.None;
            chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
            for (int p = 0; p < datalogger.PidProfile.Count; p++)
            {
                PidScalar ps = new PidScalar(datalogger.PidProfile[p].PidName);
                pidScalars.Add(ps);
                chart1.Series.Add(new Series());
                chart1.Series[p].ChartType = SeriesChartType.Line;
                //chart1.Series[r].XValueType = ChartValueType.DateTime;
                if (datalogger.PidProfile[p].PidName != null)
                    chart1.Series[p].Name = datalogger.PidProfile[p].PidName;
                chart1.Series[p].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
            }
            dataGridValues.DataSource = pidScalars;
            dataGridValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
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
                    chart1.Series[p].ChartType = SeriesChartType.Line;
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

        public void QueueliveData(double[] LastCalcValues)
        {
            try
            {
                string dNow = DateTime.Now.ToString("HH:mm:ss");
                for (int p = 0; p < LastCalcValues.Length; p++)
                {
                    if (pidScalars[p].On)
                    {
                        double orgVal = LastCalcValues[p];
                        if (orgVal == double.MinValue || orgVal == double.MaxValue)
                        {
                            orgVal = 0;
                        }
                        double val = orgVal * pidScalars[p].Scalar;
                        //Debug.WriteLine(dNow + " - " + val.ToString());
                        //chart1.Series[p].Points.AddXY(DateTime.Now.ToString("HH:mm:ss"), val);
                        DataPoint point = new DataPoint();
                        point.SetValueXY(dNow, val);
                        point.ToolTip = string.Format("[{0}] {1}: {2}", dNow, pidScalars[p].Pid, orgVal);
                        point.Tag = DateTime.Now;
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

        private void SetupLogGraphics(string[] header, string Title)
        {
            pidScalars = new List<PidScalar>();
            //pidScalars.Add(new PidScalar("TimeStamp"));  
            this.Text = Title;
            this.Title = Title;
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
            for (int r = 0; r < header.Length-TStamps; r++)
            {
                PidScalar ps = new PidScalar(header[r + TStamps]);
                pidScalars.Add(ps);
                chart1.Series.Add(new Series());
                chart1.Series[r].ChartType = SeriesChartType.Line;
                chart1.Series[r].XValueType = ChartValueType.DateTime;
                chart1.Series[r].Name = header[r + TStamps];
                chart1.Series[r].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
            }
            dataGridValues.DataSource = pidScalars;
            dataGridValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
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
            CA.AxisX.ScaleView.Zoomable = true;
            CA.CursorX.AutoScroll = true;
            CA.CursorX.IsUserSelectionEnabled = true;
            CA.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All;
        }


        private void UpdateLogGraphics()
        {
            chart1.Series.Clear();
            for (int p = 0; p < pidScalars.Count; p++)
            {
                chart1.Series.Add(new Series());
                if (pidScalars[p].On)
                {
                    chart1.Series[p].ChartType = SeriesChartType.Line;
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
                while ((logLine = sr.ReadLine()) != null)
                {
                    //Custom handling: read OS:Segmentaddress pairs from file
                    row++;
                    if (row % 1000 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                    if (row > 1000 && !chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 1000);
                    }
                    string[] lParts = logLine.Split(new string[] { txtLogSeparator.Text }, StringSplitOptions.None);
                    string tStamp = lParts[0];
                    for (int r = TStamps; r < lParts.Length; r++)
                    {
                        if (pidScalars[r - TStamps].On)
                        {
                            double origVal;
                            string valStr = lParts[r].Replace(",", ".");
                            if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out origVal))
                            {
                                double val = origVal * pidScalars[r - TStamps].Scalar;
                                //chart1.Series[r - 1].Points.AddXY(tStamp, val);
                                DataPoint point = new DataPoint();
                                point.SetValueXY(tStamp, val);
                                point.ToolTip = string.Format("[{0}] {1}: {2}", tStamp, pidScalars[r-TStamps].Pid, origVal);
                                if (chkShowPoints.Checked)
                                    point.MarkerStyle = MarkerStyle.Circle;
                                else
                                    point.MarkerStyle = MarkerStyle.None;
                                chart1.Series[r- TStamps].Points.Add(point);
                            }
                        }
                    }
                }
                sr.Close();
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
                Title = fName;
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
                    int pointAge = (int)DateTime.Now.Subtract((DateTime)chart1.Series[0].Points[0].Tag).TotalSeconds;
                    if (pointAge > maxAge)
                    {
                        for (int p = 0; p < chart1.Series.Count; p++)
                        {
                            if (chart1.Series[p].Points.Count > 0)
                            {
                                pointAge = (int)DateTime.Now.Subtract((DateTime)chart1.Series[p].Points[0].Tag).TotalSeconds;
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
            CleanOldPoints();
            QueuePoint qp;
            //labelQueueLen.Text = "Queue length: " + PointQ.Count.ToString();
            while (PointQ.Count > 0)
            {

                lock (PointQ)
                {
                    qp = PointQ.Dequeue();
                }
                chart1.Series[qp.Serie].Points.Add(qp.Point);
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
            logFile = LastLiveLogFile;
            Title = LastLiveLogFile;
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
    }
}
