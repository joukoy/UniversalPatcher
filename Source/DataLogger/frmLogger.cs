using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using static Upatcher;
using static Helpers;
using static LoggerUtils;
using System.IO;
using System.Xml.Linq;
using static UniversalPatcher.DataLogger;
using System.Management;
using J2534DotNet;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using static UniversalPatcher.ExtensionMethods;
using System.Runtime.InteropServices;
using static UniversalPatcher.PidConfig;

//using FTD2XX_NET;
//using SAE.J2534;

namespace UniversalPatcher
{
    public partial class frmLogger : Form
    {
        public frmLogger()
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(richVPWmessages);
            DrawingControl.SetDoubleBuffered(richJConsole);
        }

        private BindingSource bsParams = new BindingSource();
        private List<Parameter> stdParameters = new List<Parameter>();
        private List<RamParameter> ramParameters = new List<RamParameter>();
        private List<MathParameter> mathParameters = new List<MathParameter>();
        private BindingList<LogParam.PidParameter> profilebl = new BindingList<LogParam.PidParameter>();
        private string profileFile;
        private string logfilename;
        private bool ProfileDirty = false;
        private List<int> SupportedPids;
        private int keyDelayCounter = 0;
        public List<J2534DotNet.J2534Device> jDevList;
        bool waiting4x = false;
        bool jConsoleWaiting4x = false;
        JConsole jConsole;
        OBDScript oscript;
        OBDScript joscript;
        StreamWriter jConsoleStream;
        StreamWriter vpwConsoleStream;
        int canQuietResponses;
        int canDeviceResponses;
        DateTime lastResponseTime;
        List<CANDevice> canDevs;
        private frmLoggerGraphics GraphicsForm;
        private frmHistogram HstForm;
        private ToolTip ScrollTip = new ToolTip();
        public PcmFile PCM;
        private string CurrentPortName;
        private string jConsoleFilters;
        private string jConsoleFilters2;
        private bool jConsoleTimestamps;
        private bool jConsoleDevTimestamps;
        private bool ConsoleTimestamps;
        private bool ConsoleDevTimestamps;
        private bool jConsoleVPW = false;
        private bool jConsoleCAN = false;
        private FileTraceListener DebugFileListener;
        private frmDashboard dashboard;
        private BindingList<LogParam.PidParameter> pidparams;
        private string PidParamFile;
        private bool DisableConversionChanges = false;
        private ushort DtcCurrentModule;
        private string sortBy_Profile = "";
        private int sortIndex_Profile = 0;
        SortOrder strSortOrder_Profile = SortOrder.Ascending;
        private string sortBy_PidEditor = "";
        private int sortIndex_PidEditor = 0;
        SortOrder strSortOrder_PidEditor = SortOrder.Ascending;
        private CancellationTokenSource pidTesterTokenSource = new CancellationTokenSource();
        private CancellationToken pidTesterToken;
        private bool DisablePidEditorChanges = false;
        private List<int> PassivePidCanIds;
        private Dictionary<int, string> PassivePidOrigins;
        private List<LogParam.PidSettings> PassivePidSettings;
        int profilecurrentrow = 0;
        int profilecurrentcell = 0;
        private DateTime DisconnectTime = DateTime.MaxValue;
        private bool RequestRestartLogging = false;
        private bool floodTestActive = false;

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref Point lParam);

        [DllImport("user32")]
        static extern int GetScrollInfo(IntPtr hwnd, int nBar, ref SCROLLINFO scrollInfo);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        public struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int min;
            public int max;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        const int WM_USER = 0x400;
        const int EM_GETSCROLLPOS = WM_USER + 221;
        const int EM_SETSCROLLPOS = WM_USER + 222;
        const int EM_SCROLL = 0xB5;
        const int SB_LINEDOWN = 1;
        const int SB_LINEUP = 0;
        //private uint WM_VSCROLL = 0x115;

        public class LogText
        {
            public LogText() { }
            public LogText(Color Color, string LogTxt, long TimeStamp)
            {
                this.color = Color;
                this.Txt = LogTxt;
                this.TimeStamp = TimeStamp;
            }
            public Color color { get; set; }
            public string Txt { get; set; }
            public long TimeStamp { get; set; }
        }
        public class CombinedCanData
        {
            public CombinedCanData() { }
            public CombinedCanData(int canid) 
            {
                this.canid = canid;
            }
            //private string dataStr;
            private byte[] databytes;
            public byte[] DataBytes
            {
                get { return databytes; }
                set { databytes = value; hertz.AddTime(); }
            }

            public int canid;
            public string CanID
            {
                get { return canid.ToString("X8"); }
                set { HexToInt(value.Replace("0x", ""), out canid); }
            }
            public string Data 
            {
                get { return databytes.ToHex(); }
                set { databytes = value.Replace(" ","").ToBytes(); hertz.AddTime(); } 
            }
            public string Hz { get { return hertz.GetHertz().ToString("0.0"); } }
            public Hertz hertz = new Hertz();
        }
        private List<CombinedCanData> combinedCanDatas;

        public List<LogText> logTexts;
        private int lastLogRowCount = 0;
        public List<LogText> jconsolelogTexts;
        private int jconsolelastLogRowCount = 0;
        private Queue<LogText> consoleLogQueue = new Queue<LogText>();
        private Queue<LogText> jconsoleLogQueue = new Queue<LogText>();
        private Queue<Analyzer.AnalyzerData> analyzedRows = new Queue<Analyzer.AnalyzerData>();
        ContextMenuStrip rtcMenu = new ContextMenuStrip();

        private void frmLogger_Load(object sender, EventArgs e)
        {
            frmlogger = this;
            SetWorkingMode();
            this.KeyUp += FrmLogger_KeyUp;
            logTexts = new List<LogText>();
            jconsolelogTexts = new List<LogText>();
            datalogger = new DataLogger(radioVPW.Checked);
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            //comboPidEditorType.ValueMember = "Value";
            //comboPidEditorType.DisplayMember = "Name";
            comboConnectFlag.DataSource = Enum.GetValues(typeof(ConnectFlag));
            comboPidEditorType.DataSource = Enum.GetValues(typeof(LogParam.DefineBy));
            comboPidEditorDatatype.DataSource = Enum.GetValues(typeof(LogParam.ProfileDataType));
            if (!string.IsNullOrEmpty(datalogger.OS))
            {
                labelConnected.Text = "Disconnected - OS: " + datalogger.OS;
            }
            dataGridLogData.Columns.Add("Value", "Value");
            dataGridLogData.Columns.Add("Units", "Units");
            dataGridLogData.Columns.Add("Hz", "Hz");
            dataGridSelectedPids.Columns.Add("Pid", "Pid");
            dataGridSelectedPids.Columns.Add("Value", "Value");
            dataGridSelectedPids.Columns["Value"].Visible = false;
            dataGridSelectedPids.Columns.Add("Units", "Units");
            dataGridSelectedPids.Columns.Add("Id", "Id");
            DataGridViewButtonColumn dgb = new DataGridViewButtonColumn()
            {
                Name = "Remove",
                HeaderText = "",
                Text = "X",
                UseColumnTextForButtonValue = true
            };
            dataGridSelectedPids.Columns.Insert(0, dgb);
            Application.DoEvents();
            comboBaudRate.DataSource = SupportedBaudRates;
            LoadSettings();
            btnStartStop.Text = "Start logging (" + comboStartLoggingKey.Text + ")";
            initPcmResponses();
            SetupPidEditor();

            if (!string.IsNullOrEmpty(AppSettings.LoggerLastProfile) && File.Exists(AppSettings.LoggerLastProfile))
            {
                profileFile = AppSettings.LoggerLastProfile;
                LoadProfile(profileFile);
            }
            LoadProfileList();
            //tabPidEditor.Enter += TabPidEditor_Enter;
            //ReloadPidParams(true, true);

            dataGridViewPidEditor.SelectionChanged += DataGridViewPidEditor_SelectionChanged;
            dataGridViewPidEditor.DataError += DataGridViewPidEditor_DataError;
            dataGridViewPidEditor.DataBindingComplete += DataGridViewPidEditor_DataBindingComplete;
            dataGridViewPidEditor.ColumnHeaderMouseClick += DataGridViewPidEditor_ColumnHeaderMouseClick;
            dataGridViewPidEditorConversions.SelectionChanged += DataGridViewPidEditorConversions_SelectionChanged;
            txtPidname.Validating += TxtPidname_Validating;
            txtPidId.Validating += TxtPidId_Validating;
            comboPidEditorDatatype.Validating += ComboPidEditorDatatype_Validating;
            this.FormClosing += frmLogger_FormClosing;
            tabLog.Enter += TabLog_Enter;
            tabProfile.Enter += TabProfile_Enter;

            dataGridLogData.DataError += DataGridLogData_DataError;
            dataGridProfile.DataBindingComplete += dataGridProfile_DataBindingComplete;
            dataGridProfile.CellClick += DataGridProfile_CellClick;
            dataGridProfile.ColumnHeaderMouseClick += DataGridProfile_ColumnHeaderMouseClick;
            dataGridProfile.DataError += dataGridProfile_DataError;
            dataGridViewPidEditorPids.DataError += DataGridViewPidEditorPids_DataError;
            dataGridViewPidEditorConversions.DataError += DataGridViewPidEditorConversions_DataError;
            dataGridSelectedPids.CellValueChanged += DataGridSelectedPids_CellValueChanged;
            dataGridSelectedPids.DataError += DataGridSelectedPids_DataError;
            dataGridSelectedPids.CellClick += DataGridSelectedPids_CellClick;

            comboProfileCategory.KeyPress += ProfileCombos_KeyPress;
            comboProfilePlatform.KeyPress += ProfileCombos_KeyPress;
            comboProfileFilename.KeyPress += ProfileCombos_KeyPress;

            SerialPortService.PortsChanged += SerialPortService_PortsChanged;
            dataGridAnalyzer.DataError += DataGridAnalyzer_DataError;
            comboBaudRate.KeyPress += ComboBaudRate_KeyPress;
            txtSendBus.KeyPress += TxtSendBus_KeyPress;
            richVPWmessages.EnableContextMenu();
            richJConsole.EnableContextMenu();
            txtJ2534SetPins.Enter += TxtJ2534SetPins_Enter;
            txtJConsoleSend.KeyPress += TxtJConsoleSend_KeyPress;
            chkConsole4x.Enter += ChkConsole4x_Enter;
            chkJConsole4x.Enter += ChkConsole4x_Enter;
            this.numRestartLoggingAfter.ValueChanged += new System.EventHandler(this.numRestartLoggingAfter_ValueChanged);
            this.chkRestartLogging.CheckedChanged += new System.EventHandler(this.chkRestartLogging_CheckedChanged);

            chkVpwBuffered.Checked = true;
            chkJconsoleUsebuffer.Checked = true;
            WB = new WideBand();
            this.comboWBType.SelectedIndexChanged += new System.EventHandler(this.comboWBType_SelectedIndexChanged);
            this.comboWBport.SelectedIndexChanged += new System.EventHandler(this.comboWBport_SelectedIndexChanged);
            FillPlatformCombo();
            comboFilterByOS.SelectedIndexChanged += new System.EventHandler(comboFilterByOS_SelectedIndexChanged);
            for (int r = 0; r < RealTimeControls.Count; r++)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem();
                mi.Text = RealTimeControls[r].Control;
                mi.Tag = RealTimeControls[r].CommandSet;
                mi.Click += rtcMenu_Click;
                rtcMenu.Items.Add(mi);
            }
        }


        private void ShowSelectedPassivePid()
        {
            try
            {
                if (dataGridAnalyzer.SelectedCells.Count == 0 ||
                    dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["CanID"].Value == null ||
                    dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["Data"].Value == null)
                {
                    return;
                }
                List<LogParam.PidParameter> passParms = new List<LogParam.PidParameter>();
                int canid;
                HexToInt(dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["CanID"].Value.ToString(), out canid);
                byte[] data = dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["Data"].Value.ToString().Replace(" ", "").ToBytes();
                List<LogParam.PidSettings> pidSettings =
                    datalogger.SelectedPids.Where(X => X.Parameter.Type == LogParam.DefineBy.Passive && X.Parameter.Address == canid).ToList();
                foreach (LogParam.PidSettings pid in pidSettings)
                {
                    passParms.Add(pid.Parameter);
                    UInt64 val = SlotHandler.ExtractPassivePidData(data, pid.Parameter.PassivePid.BitStartPos, pid.Parameter.PassivePid.NumBits, pid.Parameter.PassivePid.MSB);
                    pid.LastPassiveRawValue = val;
                    string valStr = pid.Parameter.GetCalculatedStringValue(pid, false);
                    Logger(pid.Parameter.Name + ": " + valStr);
                    frmSignalView fsv = new frmSignalView();
                    fsv.txtValue.Text = "BitStartPos: " + pid.Parameter.PassivePid.BitStartPos.ToString() +
                        ", NumBits: " + pid.Parameter.PassivePid.NumBits.ToString() +
                        ", Data: " + val.ToString("0.0") + " (0x" + val.ToString("X") + "), Expression: " +
                        pid.Units.Expression.Replace("x", val.ToString("0.0")) + ", Calculated value: " + valStr;
                    fsv.Show();
                    fsv.LoadSignal(data, pid.Parameter);
                }
                List<LogParam.PidParameter> parms = datalogger.PidParams.Where(X => X.Type == LogParam.DefineBy.Passive && X.Address == canid).ToList();
                foreach (LogParam.PidParameter parm in parms)
                {
                    if (!passParms.Contains(parm))
                    {
                        passParms.Add(parm);
                        LogParam.PidSettings pid = new LogParam.PidSettings(parm);
                        pid.Units = parm.Conversions[0];
                        UInt64 val = SlotHandler.ExtractPassivePidData(data, pid.Parameter.PassivePid.BitStartPos, pid.Parameter.PassivePid.NumBits, pid.Parameter.PassivePid.MSB);
                        pid.LastPassiveRawValue = val;
                        string valStr = pid.Parameter.GetCalculatedStringValue(pid, false);
                        Logger(pid.Parameter.Name + ": " + valStr);
                        frmSignalView fsv = new frmSignalView();
                        fsv.txtValue.Text = "BitStartPos: " + pid.Parameter.PassivePid.BitStartPos.ToString() +
                            ", NumBits: " + pid.Parameter.PassivePid.NumBits.ToString() +
                        ", Data: " + val.ToString("0.0") + " (0x" + val.ToString("X") + "), Expression: " +
                        pid.Units.Expression.Replace("x", val.ToString("0.0")) + ", Calculated value: " + valStr;
                        fsv.Show();
                        fsv.LoadSignal(data, parm);
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
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void DataGridAnalyzer_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridAnalyzer.SelectedCells.Count == 0 ||
                        dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["CanID"].Value == null ||
                        dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["Data"].Value == null)
                {
                    return;
                }
                List<LogParam.PidParameter> passParms = new List<LogParam.PidParameter>();
                int canid;
                HexToInt(dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["CanID"].Value.ToString(), out canid);
                if (PassivePidCanIds.Contains(canid))
                {
                    showSignalBitsToolStripMenuItem.Enabled = true;
                }
                else
                {
                    showSignalBitsToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void TabPidEditor_Enter(object sender, EventArgs e)
        {
            ReloadPidParams(true, false);
        }

        private void ComboPidEditorDatatype_Validating(object sender, CancelEventArgs e)
        {
            if (DisablePidEditorChanges)
            {
                return;
            }
            if (dataGridViewPidEditor.SelectedCells.Count == 0)
            {
                return;
            }
            int row = dataGridViewPidEditor.SelectedCells[0].RowIndex;
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[row].DataBoundItem;
            parm.DataType = (LogParam.ProfileDataType)comboPidEditorDatatype.SelectedIndex;
        }

        private void TxtPidId_Validating(object sender, CancelEventArgs e)
        {
            if (DisablePidEditorChanges)
            {
                return;
            }
            if (dataGridViewPidEditor.SelectedCells.Count == 0)
            {
                return;
            }
            int row = dataGridViewPidEditor.SelectedCells[0].RowIndex;
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[row].DataBoundItem;
            if (parm.Type != LogParam.DefineBy.Pid)
            {
                parm.Id = txtPidId.Text;
            }
        }

        private void TxtPidname_Validating(object sender, CancelEventArgs e)
        {
            if (DisablePidEditorChanges)
            {
                return;
            }
            if (dataGridViewPidEditor.SelectedCells.Count == 0)
            {
                return;
            }
            int row = dataGridViewPidEditor.SelectedCells[0].RowIndex;
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[row].DataBoundItem;
            parm.Name = txtPidname.Text;
            if (string.IsNullOrEmpty(txtPidId.Text))
            {
                switch(parm.Type)
                {
                    case LogParam.DefineBy.Address:
                        txtPidId.Text = "Ram" + txtPidname.Text.Replace(" ", "").Replace("/", "");
                        break;
                    case LogParam.DefineBy.Math:
                        txtPidId.Text = "Math" + txtPidname.Text.Replace(" ", "").Replace("/", "");
                        break;
                    case LogParam.DefineBy.WB_Afr:
                        txtPidId.Text = "SerialWB" + txtPidname.Text.Replace(" ", "").Replace("/", "");
                        break;
                    case LogParam.DefineBy.WB_Raw:
                        txtPidId.Text = "SerialWB" + txtPidname.Text.Replace(" ", "").Replace("/", "");
                        break;
                }
            }
        }

        private void DataGridSelectedPids_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int removeColumn = dataGridSelectedPids.Columns["Remove"].Index;
            if (e.ColumnIndex == removeColumn)
            {
                LogParam.PidSettings pidSettings = (LogParam.PidSettings)dataGridSelectedPids.Rows[e.RowIndex].Tag;
                datalogger.SelectedPids.Remove(pidSettings);
                dataGridSelectedPids.Rows.RemoveAt(e.RowIndex);
                CheckMaxPids();
                SetProfileDirty(true,true);
            }
        }

        private void TabProfile_Enter(object sender, EventArgs e)
        {
            //ReloadPidParams(false, true);
            //SetupLogDataGrid();
        }

        private void DataGridSelectedPids_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void DataGridSelectedPids_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex >= 0 && e.ColumnIndex < dataGridSelectedPids.Columns.Count && dataGridSelectedPids.Columns[e.ColumnIndex].Name == "Units")
                {
                    DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridSelectedPids.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (cb.Value != null)
                    {
                        LogParam.PidSettings pidSettings = dataGridSelectedPids.Rows[e.RowIndex].Tag as LogParam.PidSettings;
                        string units = cb.Value.ToString();
                        if (pidSettings.Units.Units != units)
                        {
                            pidSettings.Units = pidSettings.Parameter.Conversions.Where(X => X.Units == units).FirstOrDefault();
                            Debug.WriteLine("Units: " + pidSettings.Units.ToString());
                            SetProfileDirty(true,false);
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
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void DataGridProfile_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 0)
                {
                    //Button
                    LogParam.PidParameter parm = (LogParam.PidParameter)dataGridProfile.Rows[e.RowIndex].DataBoundItem;
                    LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                    datalogger.SelectedPids.Add(pidSettings);
                    AddPidToSelectedPidGrid(pidSettings);
                    CheckMaxPids();
                    SetProfileDirty(true,true);
                    dataGridProfile.EndEdit();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void ProfileCombos_KeyPress(object sender, KeyPressEventArgs e)
        {
            btnSaveProfile.Enabled = true;
        }


        private void FrmLogger_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == AppSettings.LoggerStartKey)
            {
                StartStopLogging();
            }
            else if (e.KeyCode == Keys.F1)
            {
                AboutBox1 aboutBox = new AboutBox1();
                aboutBox.Show();

            }
        }


        private void DataGridViewPidEditor_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dataGridViewPidEditor.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
            {
                sortBy_PidEditor = dataGridViewPidEditor.Columns[e.ColumnIndex].HeaderText;
                sortIndex_PidEditor = e.ColumnIndex;
                strSortOrder_PidEditor = GetSortOrder(sortIndex_PidEditor, dataGridViewPidEditor);
                ReloadPidParams(true, false);
            }
        }

        private void DataGridProfile_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left  && dataGridProfile.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
            {
                sortBy_Profile= dataGridProfile.Columns[e.ColumnIndex].HeaderText;
                sortIndex_Profile = e.ColumnIndex;
                strSortOrder_Profile = GetSortOrder(sortIndex_Profile, dataGridProfile);
                ReloadPidParams(false, true);
            }
        }

        private SortOrder GetSortOrder(int columnIndex, DataGridView dgv)
        {
            try
            {
                if (dgv.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                    || dgv.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                {
                    dgv.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    return SortOrder.Ascending;
                }
                else
                {
                    dgv.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    return SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return SortOrder.Ascending;
        }

        private void LoadProfile(string fName)
        {
            profileFile = fName;
            comboProfileFilename.Text = Path.GetFileName(fName);
            this.Text = "Logger - " + profileFile;
            datalogger.SelectedPids = LogParamFile.LoadProfile(fName);
            AppSettings.LoggerLastProfile = fName;
            AppSettings.Save();
            SetupLogDataGrid();
            ReloadPidParams(false, true);
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.SetupLiveGraphics();
            }
            SelectPlatform_Category(fName);
            UpdateProfile();
            SetProfileDirty(false,true);
        }
        private void FillPlatformCombo()
        {
            try
            {
                List<string> handled = new List<string>();
                if (!comboProfilePlatform.Items.Contains("_All"))
                {
                    comboProfilePlatform.Items.Add("_All");
                }
                if (!comboProfilePlatform.Items.Contains("_Undefined"))
                {
                    comboProfilePlatform.Items.Add("_Undefined");
                }
                if (!comboProfileCategory.Items.Contains("_All"))
                {
                    comboProfileCategory.Items.Add("_All");
                }
                if (!comboProfileCategory.Items.Contains("_Undefined"))
                {
                    comboProfileCategory.Items.Add("_Undefined");
                }
                foreach (DetectRule dr in DetectRules)
                {
                    if (!handled.Contains(dr.xml)) 
                    {
                        handled.Add(dr.xml);
                        string platform = dr.xml.Replace(".xml", "");
                        if (!comboProfilePlatform.Items.Contains(platform))
                        {
                            comboProfilePlatform.Items.Add(platform);
                        }
                        string plFolder = Path.Combine(Application.StartupPath, "Logger", "Profiles", platform);
                        if (Directory.Exists(plFolder))
                        {
                            DirectoryInfo di = new DirectoryInfo(plFolder);
                            DirectoryInfo[] dInfo = di.GetDirectories("*.*");
                            foreach (DirectoryInfo d in dInfo)
                            {
                                if (!comboProfileCategory.Items.Contains(d.Name))
                                {
                                    comboProfileCategory.Items.Add(d.Name);
                                }
                            }
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
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }


        private void TreeProfiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (datalogger.LogRunning || timerPlayback.Enabled)
            {
                return;
            }
            string fName = (string)e.Node.Tag;
            if (File.Exists(fName))
            {
                if (ProfileDirty)
                {
                    DialogResult dialogResult = MessageBox.Show("Save profile modifications?", "Save profile", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == DialogResult.Yes)
                    {
                        SelectSaveProfile();
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                LoadProfile(fName);
            }
        }


        private void SetProfileDirty(bool Dirty, bool LoggingRestart)
        {
            ProfileDirty = Dirty;
            btnSaveProfile.Enabled = Dirty;
            if (LoggingRestart)
            {
                RequestRestartLogging = true;
                //RestartLogging(false);
            }
        }


        private void dataGridProfile_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                if (!dataGridProfile.Columns.Contains("Enable"))
                {
                    DataGridViewButtonColumn dgb = new DataGridViewButtonColumn()
                    {
                        Name = "Enable",
                        HeaderText = "",
                        Text = "+",
                        UseColumnTextForButtonValue = true
                    };
                    dataGridProfile.Columns.Insert(0, dgb);
                }
                //dataGridProfile.Columns["DataType"].Visible = false;
                dataGridProfile.Columns["Type"].Visible = false;
                dataGridProfile.Columns["Id"].ReadOnly = true;
                dataGridProfile.Columns["Name"].ReadOnly = true;
                dataGridProfile.Columns["Description"].ReadOnly = true;
                Application.DoEvents();
                dataGridProfile.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                if (!string.IsNullOrEmpty(sortBy_Profile))
                {
                    dataGridProfile.Columns[sortIndex_Profile].HeaderCell.SortGlyphDirection = strSortOrder_Profile;
                }
                dataGridProfile.RowHeadersWidth = 5;
                dataGridProfile.CurrentCell = dataGridProfile.Rows[profilecurrentrow].Cells[profilecurrentcell];
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void DataGridViewPidEditorConversions_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DisableConversionChanges = true;
                if (dataGridViewPidEditorConversions.SelectedCells.Count == 0)
                {
                    return;
                }
                Conversion conversion = (Conversion)dataGridViewPidEditorConversions.Rows[dataGridViewPidEditorConversions.SelectedCells[0].RowIndex].DataBoundItem;
                chkConversionBitMapped.Checked = conversion.IsBitMapped;
                numConversionBitIndex.Value = conversion.BitIndex;
                txtConversionFalseValue.Text = conversion.FalseValue;
                txtConversionTrueValue.Text = conversion.TrueValue;
                txtConversionFormat.Text = conversion.Format;
                txtConversionExpression.Text = conversion.Expression;
                txtConversionUnits.Text = conversion.Units;
                DisableConversionChanges = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void ShowEnabledPidsValues(bool Verbose)
        {
            try
            {
                if (Connect(radioVPW.Checked, true, true, oscript))
                {
                    dataGridSelectedPids.Columns["Value"].Visible = true;
                    datalogger.Receiver.SetReceiverPaused(true);
                    int row = 0;
                    foreach (LogParam.PidSettings pidProfile in datalogger.SelectedPids)
                    {
                        string pidVal = datalogger.QueryPid(pidProfile, Verbose);
                        dataGridSelectedPids.Rows[row].Cells["Value"].Value = pidVal;
                        dataGridLogData.Rows[row].Cells["Value"].Value = pidVal;
                        row++;
                        Application.DoEvents();
                        Thread.Sleep(30);
                    }
                    datalogger.Receiver.SetReceiverPaused(false);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void SetupPidEditor()
        {
            try
            {
                if (radioVPW.Checked)
                    PidParamFile = Path.Combine(Application.StartupPath, "Logger", "PidParameters-VPW.XML");
                else
                    PidParamFile = Path.Combine(Application.StartupPath, "Logger", "PidParameters-CAN.XML");
                if (File.Exists(PidParamFile))
                {
                    LogParamFile.LoadParamfile(PidParamFile);
                    ReloadPidParams(true, true);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void DataGridViewPidEditorConversions_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine("DataGridViewPidEditorConversions_DataError: " + e.Exception.Message);
        }

        private void DataGridViewPidEditorPids_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void DataGridViewPidEditor_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                //UseComboBoxForEnums(dataGridViewPidEditor);
                //dataGridViewPidEditor.Columns["Enabled"].Visible = false;
                if (!string.IsNullOrEmpty(sortBy_PidEditor))
                {
                    dataGridViewPidEditor.Columns[sortIndex_PidEditor].HeaderCell.SortGlyphDirection = strSortOrder_PidEditor;
                }
                dataGridViewPidEditor.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                Application.DoEvents();
                DisablePidEditorChanges = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void DataGridViewPidEditor_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void ShowParmPids(LogParam.PidParameter parm)
        {
            try
            {
                dataGridViewPidEditorPids.Rows.Clear();
                dataGridViewPidEditorPids.Columns.Clear();
                dataGridViewPidEditorPids.Columns.Add("Id", "Id");
                dataGridViewPidEditorPids.Columns.Add("Variable", "Variable");
                dataGridViewPidEditorPids.Columns.Add("Units", "Units");
                for (int r = 0; r < parm.Pids.Count; r++)
                {
                    dataGridViewPidEditorPids.Rows.Add();
                    LogParam.LogPid lPid = parm.Pids[r];
                    dataGridViewPidEditorPids.Rows[r].Tag = lPid;
                    LogParam.PidParameter pParm = lPid.Parameter;
                    dataGridViewPidEditorPids.Rows[r].Cells["Id"].Value = pParm.Name;
                    dataGridViewPidEditorPids.Rows[r].Cells["Variable"].Value = lPid.Variable;
                    DataGridViewComboBoxCell dgcUnits = new DataGridViewComboBoxCell();
                    dgcUnits.DataSource = pParm.Conversions;
                    dgcUnits.ValueMember = "Units";
                    dgcUnits.DisplayMember = "Units";
                    dataGridViewPidEditorPids.Rows[r].Cells["Units"] = dgcUnits;
                    dataGridViewPidEditorPids.Rows[r].Cells["Units"].Value = lPid.Units.Units;
                    if (lPid.Parameter.Type == LogParam.DefineBy.Address)
                    {
                        dataGridViewPidEditorPids.Columns.Add("Os", "Os");
                        DataGridViewComboBoxCell dgcOs = new DataGridViewComboBoxCell();
                        dgcOs.DataSource = lPid.Parameter.Locations;
                        dgcOs.ValueMember = "Os";
                        dgcOs.DisplayMember = "Os";
                        dataGridViewPidEditorPids.Rows[r].Cells["Os"] = dgcOs;
                        if (lPid.Os != null)
                        {
                            dataGridViewPidEditorPids.Rows[r].Cells["Os"].Value = lPid.Os.Os;
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void LoadPidEditorValues(LogParam.PidParameter parm)
        {

            txtPidname.Text = parm.Name;
            txtPidId.Text = parm.Id;
            comboPidEditorType.Text = parm.Type.ToString();
            chkPideditorCustom.Checked = parm.Custom;
            comboPidEditorDatatype.Text = parm.DataType.ToString();
            dataGridViewPidEditorPids.DataSource = null;
            dataGridViewPidEditorPids.Rows.Clear();
            dataGridViewPidEditorPids.Columns.Clear();
            dataGridViewPidEditorPids.AllowUserToAddRows = true;
            dataGridViewPidEditorPids.AllowUserToDeleteRows = true;
            labelPidEditorId.Text = "Id:";

            if (parm.Type == LogParam.DefineBy.Pid)
            {
                dataGridViewPidEditorPids.Visible = false;
                labelProfilePids.Visible = false;
            }
            else
            {
                dataGridViewPidEditorPids.Visible = true;
                labelProfilePids.Visible = true;
                if (parm.Type == LogParam.DefineBy.Address)
                {
                    labelProfilePids.Text = "Locations:";
                    BindingList<LogParam.Location> locations = new BindingList<LogParam.Location>(parm.Locations);
                    //dataGridViewPidEditorPids.ContextMenuStrip = null;
                    dataGridViewPidEditorPids.DataSource = locations;
                }
                else if (parm.Type == LogParam.DefineBy.Math)
                {
                    labelProfilePids.Text = "Pids:";
                    ShowParmPids(parm);
                }
                else if (parm.Type == LogParam.DefineBy.Passive)
                {
                    labelProfilePids.Text = "Passive pid;";
                    labelPidEditorId.Text = "CAN Id:";
                    BindingList<LogParam.PassivePidSettings> pPids = new BindingList<LogParam.PassivePidSettings>();
                    pPids.Add(parm.PassivePid);
                    dataGridViewPidEditorPids.DataSource = pPids;
                    dataGridViewPidEditorPids.AllowUserToAddRows = false;
                    dataGridViewPidEditorPids.AllowUserToDeleteRows = false;
                }
                else
                {
                    labelProfilePids.Visible =false;
                    dataGridViewPidEditorPids.Visible = false;
                    labelProfilePids.Visible = false;
                }
            }
            BindingList<Conversion> conversions = new BindingList<Conversion>(parm.Conversions);
            dataGridViewPidEditorConversions.DataSource = conversions;

        }
        private void DataGridViewPidEditor_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DisablePidEditorChanges = true;
                if (dataGridViewPidEditor.SelectedCells.Count == 0)
                {
                    return;
                }
                dataGridViewPidEditorPids.DataSource = null;
                dataGridViewPidEditorConversions.DataSource = null;
                Application.DoEvents();
                int row = dataGridViewPidEditor.SelectedCells[0].RowIndex;
                if (row < 0)
                {
                    Debug.WriteLine("Row " + row.ToString() + " selected, exit");
                    return;
                }
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[row].DataBoundItem;
                if (parm == null)
                {
                    dataGridViewPidEditorPids.Visible = false;
                    labelProfilePids.Visible = false;
                    dataGridViewPidEditorConversions.Rows.Clear();
                    return;
                }
                LoadPidEditorValues(parm);
                DisablePidEditorChanges = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }


        private void rtcMenu_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
                string cmd = (string)menuitem.Tag;
                byte[] msg = cmd.Replace(" ","").ToBytes();
                OBDMessage oMsg = new OBDMessage(msg);
                if (datalogger.LogRunning)
                {
                    QueuedCommand qcmd = new QueuedCommand();
                    datalogger.QueueCustomCmd(oMsg, menuitem.Text);
                }
                else
                {
                    datalogger.LogDevice.SendMessage(oMsg, 1);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void LoadOsPidFiles()
        {
            try
            {
                comboFilterByOS.Items.Clear();
                string folder = Path.Combine(Application.StartupPath, "Logger", "ospids");
                DirectoryInfo d = new DirectoryInfo(folder);
                FileInfo[] Files = d.GetFiles("*.txt", SearchOption.AllDirectories);
                comboFilterByOS.Items.Add("-");
                foreach (FileInfo file in Files)
                {
                    string os = file.Name.Replace(".txt", "");
                    comboFilterByOS.Items.Add(os);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void RichVPWmessages_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int index = richVPWmessages.SelectionStart;
                int line = richVPWmessages.GetLineFromCharIndex(index);
                int lastline = GetRichBoxDisplayableLines(richVPWmessages);
                if (e.KeyCode == Keys.Down && line >= lastline && vScrollBarVpwConsole.Value < vScrollBarVpwConsole.Maximum)
                {
                    vScrollBarVpwConsole.Value++;
                    VPWConsoleScroll();
                }
                else if (e.KeyCode == Keys.Up && line == 0 && vScrollBarVpwConsole.Value > 0)
                {
                    vScrollBarVpwConsole.Value--;
                    VPWConsoleScroll();
                    richVPWmessages.Select(index, 0);
                }
                else if (e.KeyCode == Keys.PageDown)                    
                {
                    if (vScrollBarVpwConsole.Value < (vScrollBarVpwConsole.Maximum - lastline))
                        vScrollBarVpwConsole.Value += lastline;
                    else
                        vScrollBarVpwConsole.Value = vScrollBarVpwConsole.Maximum;
                    VPWConsoleScroll();
                }
                else if (e.KeyCode == Keys.PageUp) 
                {
                    if (vScrollBarVpwConsole.Value > lastline)
                        vScrollBarVpwConsole.Value -= lastline;
                    else
                        vScrollBarVpwConsole.Value = 0;
                    VPWConsoleScroll();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void RichJConsole_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int index = richJConsole.SelectionStart;
                int line = richJConsole.GetLineFromCharIndex(index);
                int lastline = GetRichBoxDisplayableLines(richJConsole);
                if (e.KeyCode == Keys.Down && line >= lastline && vScrollBarJConsole.Value < vScrollBarJConsole.Maximum)
                {
                    vScrollBarJConsole.Value++;
                    JConsoleScroll();
                }
                else if (e.KeyCode == Keys.Up && line == 0 && vScrollBarJConsole.Value > 0)
                {
                    vScrollBarJConsole.Value--;
                    JConsoleScroll();
                    richJConsole.Select(index, 0);
                }
                else if (e.KeyCode == Keys.PageDown) 
                {
                    if (vScrollBarJConsole.Value < (vScrollBarJConsole.Maximum - lastline))
                        vScrollBarJConsole.Value += lastline;
                    else
                        vScrollBarJConsole.Value = vScrollBarJConsole.Maximum;
                    JConsoleScroll();
                }
                else if (e.KeyCode == Keys.PageUp) 
                {
                    if (vScrollBarJConsole.Value > lastline)
                        vScrollBarJConsole.Value -= lastline;
                    else
                        vScrollBarJConsole.Value = 0;
                    JConsoleScroll();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void RichVPWmessages_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0) 
                {
                    if (vScrollBarVpwConsole.Value < vScrollBarVpwConsole.Maximum)
                    {
                        vScrollBarVpwConsole.Value++;
                        VPWConsoleScroll();
                    }
                }
                else 
                {
                    if (vScrollBarVpwConsole.Value > 0)
                    {
                        vScrollBarVpwConsole.Value--;
                        VPWConsoleScroll();
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void RichJConsole_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0) 
                {
                    if (vScrollBarJConsole.Value < vScrollBarJConsole.Maximum)
                    {
                        vScrollBarJConsole.Value++;
                        JConsoleScroll();
                    }
                }
                else
                {
                    if (vScrollBarJConsole.Value > 0)
                    {
                        vScrollBarJConsole.Value--;
                        JConsoleScroll();
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void AppendLogText(Color Color, string LogTxt, long TimeStamp)
        {
            LogText lt = new LogText(Color, LogTxt, TimeStamp);
            logTexts.Add(lt);
            //logTexts.Sort((s1, s2) => s1.TimeStamp.CompareTo(s2.TimeStamp));
            if (chkVpwToScreen.Checked && !chkVpwBuffered.Checked)
            {
                consoleLogQueue.Enqueue(lt);
            }
        }

        private void AppendJconsoleLogText(Color Color, string LogTxt, long TimeStamp)
        {
            LogText lt = new LogText(Color, LogTxt, TimeStamp);
            jconsolelogTexts.Add(lt);
            //jconsolelogTexts.Sort((s1, s2) => s1.TimeStamp.CompareTo(s2.TimeStamp));
            if (chkJconsoleToScreen.Checked && !chkJconsoleUsebuffer.Checked)
            {
                jconsoleLogQueue.Enqueue(lt);
            }
        }

        private void ChkConsole4x_Enter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            string ttTxt = "If enabled, tool will automatically switch to 4x mode in listen mode, or by script command";
            CheckBox chk = (CheckBox)sender;
            tt.Show(ttTxt, chk, 3000);
        }

        private void HandleConsoleCommand(Device device, MessageReceiver receiver, string cmd, OBDScript script)
        {
            try
            {
                bool brk = false;
                receiver.SetReceiverPaused(true);
                script.HandleLine(cmd, "", ref brk);
                receiver.SetReceiverPaused(false);
                return;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, HandleConsoleCommand line " + line + ": " + ex.Message);
            }
        }

        private void TxtJConsoleSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    if (!ConnectJConsole(null,null))
                    {
                        return;
                    }
                    MessageReceiver receiver = jConsole.Receiver;
                    joscript.SecondaryProtocol = radioJConsoleProto2.Checked;
                    if (radioJConsoleProto2.Checked)
                    {
                        receiver = jConsole.Receiver2;
                    }
                    joscript.stopscript = false;
                    HandleConsoleCommand(jConsole.JDevice, receiver, txtJConsoleSend.Text, joscript);
                    e.Handled = true;
                    //richJConsole.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void TxtJ2534SetPins_Enter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            string ttTxt = "Enter HEX value 0000XXYY, where\rXX=main pin\rYY=secondary pin";
            tt.Show(ttTxt, txtJ2534SetPins, 3000);
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, txtResult);
        }

        private enum SelectedTab
        {
            Logger,
            Profile,
            Analyzer,
            Dtc,
            Settings,
            Advanced
        }
        private void LogDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                return;
                StringBuilder sMsg = new StringBuilder();
                if (ConsoleTimestamps)
                {
                    sMsg.Append("[" + new DateTime(e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                }
                if (ConsoleDevTimestamps)
                {
                    sMsg.Append("[" + e.Msg.DeviceTimestamp.ToString("D20") + "] ");
                }
                //sMsg.Append(" Diff: " + new DateTime((long)(e.Msg.TimeStamp - (ulong)e.Msg.DevTimeStamp)).ToString("yyyy MM dd HH:mm:ss.fff") + "| ");
                sMsg.Append(e.Msg.ToString() + Environment.NewLine);
                AppendLogText(Color.DarkGreen, sMsg.ToString(), e.Msg.TimeStamp);

                if (vpwConsoleStream != null)
                {
                    vpwConsoleStream.WriteLine("\\cf1 " + sMsg.ToString() + "\\par");
                }

                if (e.Msg.Length > 3 && datalogger.UseVPW())
                {
                    byte[] rcv = e.Msg.GetBytes();
                    if (rcv[1] == 0xfe && rcv[3] == 0xa0)
                    {
                        Debug.WriteLine("Received 0xFE, , 0xA0 - Ready to switch to 4x");
                        waiting4x = true;
                    }
                    if (waiting4x && rcv[1] == 0xfe && rcv[3] == 0xa1)
                    {
                        waiting4x = false;
                        Debug.WriteLine("Received 0xFE, , 0xA1 - switching to 4x");
                        if (datalogger.LogDevice.SetVpwSpeed(VpwSpeed.FourX))
                            Debug.WriteLine("Switched to 4X");
                        else
                            Debug.WriteLine("Switch to 4X failed");
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void JDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                    return;

                Application.DoEvents();
                StringBuilder logTxt = new StringBuilder();
                if (jConsoleTimestamps)
                {
                    logTxt.Append("[" + new DateTime((long)e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                }
                if (jConsoleDevTimestamps)
                {
                    logTxt.Append("[" + e.Msg.DeviceTimestamp.ToString("D20") + "] ");
                }

                logTxt.Append(e.Msg.ToString() + Environment.NewLine);
                if (e.Msg.SecondaryProtocol)
                {
                    AppendJconsoleLogText(Color.Aquamarine, logTxt.ToString(), e.Msg.TimeStamp);
                }
                else
                {
                    AppendJconsoleLogText(Color.DarkGreen, logTxt.ToString(), e.Msg.TimeStamp);
                }
                if (jConsoleStream != null)
                {
                    if (e.Msg.SecondaryProtocol)
                    {
                        jConsoleStream.WriteLine("\\cf1 " + logTxt.ToString() +"\\par");
                    }
                    else
                    {
                        jConsoleStream.WriteLine("\\cf2 " + logTxt.ToString() + "\\par");
                    }
                }
                if (e.Msg.Length > 3 && jConsoleVPW)
                {
                    byte[] rcv = e.Msg.GetBytes();
                    if (rcv[1] == 0xfe && rcv[3] == 0xa0)
                    {
                        Debug.WriteLine("Received 0xFE, , 0xA0 - Ready to switch to 4x");
                        jConsoleWaiting4x = true;
                    }
                    if (jConsoleWaiting4x && rcv[1] == 0xfe && rcv[3] == 0xa1)
                    {
                        jConsoleWaiting4x = false;
                        Debug.WriteLine("Received 0xFE, , 0xA1 - switching to 4x");
                        if (jConsole.JDevice.SetVpwSpeed(VpwSpeed.FourX))
                            Debug.WriteLine("Switched to 4X");
                        else
                            Debug.WriteLine("Switch to 4X failed");
                    }
                }
                if (timerKeepBusQuiet.Enabled && jConsoleCAN)
                {
                    byte[] rcv = e.Msg.GetBytes();
                    if (rcv.Length == 12)
                    {
                        if (canDeviceResponses < 0)
                        {
                            lastResponseTime = DateTime.Now;
                            //Still waiting for quiet responses
                            canQuietResponses++;
                            Debug.WriteLine("Quiet message count: " + canQuietResponses.ToString());
                        }
                        else
                        {
                            if (rcv[5] == 0x5A && rcv[6] == 0xB0)
                            {
                                lastResponseTime = DateTime.Now;
                                CANDevice cDev = CANQuery.DecodeMsg(rcv);
                                if (cDev != null)
                                {
                                    canDevs.Add(cDev);
                                    //richJConsole.AppendText(cDev.ToString() + Environment.NewLine);
                                    AppendJconsoleLogText(Color.Black, cDev.ToString() + Environment.NewLine, e.Msg.TimeStamp);
                                }
                            }
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void LogDevice_MsgSent(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                    return;
                StringBuilder logTxt = new StringBuilder();
                if (ConsoleTimestamps)
                {
                    logTxt.Append("[" + new DateTime((long)e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                }
                if (ConsoleDevTimestamps)
                {
                    logTxt.Append("[" + e.Msg.DeviceTimestamp.ToString("D20") + "] ");
                }

                logTxt.Append(e.Msg.ToString());
                if (e.Msg.Error > 0)
                {
                    logTxt.Append(" (" + ((J2534Err)e.Msg.Error).ToString() + ")");
                }
                logTxt.Append(Environment.NewLine);

                AppendLogText(Color.Red, logTxt.ToString(), e.Msg.TimeStamp);

                if (vpwConsoleStream != null)
                {
                    vpwConsoleStream.WriteLine("\\cf3 " + logTxt.ToString() + "\\par");
                }
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void JDevice_MsgSent(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                    return;

                Application.DoEvents();
                StringBuilder logTxt = new StringBuilder();
                if (jConsoleTimestamps)
                {
                    logTxt.Append( "[" + new DateTime((long)e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                }
                if (jConsoleDevTimestamps)
                {
                    logTxt.Append("[" + e.Msg.DeviceTimestamp.ToString("D20") + "] ");
                }

                logTxt.Append(e.Msg.ToString() + Environment.NewLine);
                if (e.Msg.SecondaryProtocol)
                {
                    AppendJconsoleLogText(Color.Purple, logTxt.ToString(), e.Msg.TimeStamp);
                }
                else
                {
                    AppendJconsoleLogText(Color.Red, logTxt.ToString(), e.Msg.TimeStamp);
                }
                if (jConsoleStream != null)
                {
                    if (e.Msg.SecondaryProtocol)
                    {
                        jConsoleStream.WriteLine("\\cf4 " + logTxt.ToString() + "\\par");
                    }
                    else
                    {
                        jConsoleStream.WriteLine("\\cf3 " + logTxt.ToString() + "\\par");
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void TxtSendBus_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    if (!Connect(radioVPW.Checked, true, true,null))
                    {
                        return;
                    }
                    oscript.stopscript = false;
                    HandleConsoleCommand(datalogger.LogDevice, datalogger.Receiver, txtSendBus.Text, oscript);
                    e.Handled = true;
                    richVPWmessages.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                if (!datalogger.LogRunning)
                {
                    datalogger.Receiver.SetReceiverPaused(false);
                }
            }
        }

        private void TxtParamSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            keyDelayCounter = 0;
            timerSearchParams.Enabled = true;
        }

        private void TxtParamSearch_Leave(object sender, EventArgs e)
        {
            if (txtParamSearch.Text.Length == 0)
                txtParamSearch.Text = "Search...";
        }

        private void TxtParamSearch_Enter(object sender, EventArgs e)
        {
            if (txtParamSearch.Text == "Search...")
                txtParamSearch.Text = "";
        }

        private void ComboBaudRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void DataGridAnalyzer_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void AddSubNodes(DirectoryInfo[] dInfo, TreeNode tn)
        {
            string path = (string)tn.Tag;
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files;
            Files = d.GetFiles("*.*");
            foreach (FileInfo file in Files)
            {
                TreeNode fTn = tn.Nodes.Add(file.Name);
                fTn.Tag = file.FullName;
                fTn.ImageKey = "modify.ico";
                fTn.SelectedImageKey = "modify.ico";
                if (file.FullName == profileFile)
                {
                    treeProfiles.SelectedNode = fTn;
                }
            }

            foreach (DirectoryInfo di in dInfo.Where(X=>X.Parent.FullName == path))
            {
                TreeNode tnNew = tn.Nodes.Add(di.Name);
                tnNew.ImageKey = "category.ico";
                tnNew.SelectedImageKey = "explorer.ico";
                tnNew.Tag = di.FullName;
                AddSubNodes(dInfo, tnNew);
            }
        }

        private void LoadProfileList()
        {
            string ProfilePath = Path.Combine(Application.StartupPath, "Logger", "Profiles");

            treeProfiles.AfterSelect -= TreeProfiles_AfterSelect;
            treeProfiles.Nodes.Clear();
            DirectoryInfo dirs = new DirectoryInfo(ProfilePath);
            DirectoryInfo[] dInfo = dirs.GetDirectories("*.*", SearchOption.AllDirectories);
            foreach (DirectoryInfo di in dInfo.Where(X=>X.Parent.FullName == ProfilePath))
            {
                TreeNode tn = treeProfiles.Nodes.Add(di.Name);
                tn.Tag = di.FullName;
                tn.SelectedImageKey = "explorer.ico";
                tn.ImageKey = "category.ico";
                AddSubNodes(dInfo, tn);
            }

            DirectoryInfo d = new DirectoryInfo(ProfilePath);
            FileInfo[] Files;
            Files = d.GetFiles("*.*");
            foreach (FileInfo file in Files)
            {
                TreeNode fTn = treeProfiles.Nodes.Add(file.Name);
                fTn.Tag = file.FullName;
                fTn.ImageKey = "modify.ico";
                fTn.SelectedImageKey = "modify.ico";
                if (file.FullName == profileFile)
                {
                    treeProfiles.SelectedNode = fTn;
                }
            }
            treeProfiles.AfterSelect += TreeProfiles_AfterSelect;
            UpdateComboProfileFileNames();
        }

        private void SerialPortService_PortsChanged(object sender, PortsChangedArgs e)
        {
            try
            {
                Debug.WriteLine("Ports modified. Current port: " + CurrentPortName);
                //if (chkFTDI.Checked)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        LoadPorts(false);
                        if (radioFTDI.Checked && datalogger.Connected && serialRadioButton.Checked && !comboSerialPort.Items.Contains(CurrentPortName))
                        {
                            LoggerBold("FTDI Device disconnected");
                            Disconnect(true);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void DataGridLogData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void TabLog_Enter(object sender, EventArgs e)
        {
            SetupLogDataGrid();
        }

        private void LoadJPorts()
        {
            try
            {
                j2534DeviceList.Items.Clear();
                jDevList = J2534DotNet.J2534Detect.ListDevices();
                foreach (J2534DotNet.J2534Device device in jDevList)
                {
                    this.j2534DeviceList.Items.Add(device.Name);
                    comboJ2534DLL.Items.Add(device.Name);
                }

                comboJConsoleConfig.Items.Clear();
                comboJConsoleConfig2.Items.Clear();
                foreach (string cfg in Enum.GetNames(typeof(ConfigParameter)))
                {
                    comboJConsoleConfig.Items.Add(cfg);
                    comboJConsoleConfig2.Items.Add(cfg);
                }
                //if (LoadDefaults)
                {
                    if (!string.IsNullOrEmpty(AppSettings.LoggerJ2534Device))
                    {
                        j2534DeviceList.Text = AppSettings.LoggerJ2534Device;
                        comboJ2534DLL.Text = AppSettings.LoggerJ2534Device;
                    }
                    else if (jDevList.Count > 0)
                    {
                        j2534DeviceList.Text = jDevList.FirstOrDefault().Name;
                        comboJ2534DLL.Text = jDevList.FirstOrDefault().Name;
                    }
                    //j2534RadioButton.Checked = AppSettings.LoggerUseJ2534;
                    //groupProtocol.Enabled = AppSettings.LoggerUseJ2534;
                }
                LoadJ2534Protocols();
                //comboJ2534Connectflag.DataSource = Enum.GetValues(typeof(J2534DotNet.ConnectFlag));
                comboJ2534Connectflag.Items.Clear();
                comboJ2534Connectflag2.Items.Clear();
                foreach (string item in Enum.GetNames(typeof(J2534DotNet.ConnectFlag)))
                {
                    comboJ2534Connectflag.Items.Add(item);
                    comboJ2534Connectflag2.Items.Add(item);
                }
                //comboJ2534Connectflag.SelectedIndex = 0;
                if (!String.IsNullOrEmpty(AppSettings.LoggerJ2534SettingsFile))
                {
                    J2534InitParameters JSettings = LoadJ2534Settings(AppSettings.LoggerJ2534SettingsFile);
                    LoadJ2534InitParameters(JSettings);
                }
                if (!String.IsNullOrEmpty(AppSettings.LoggerJ2534SettingsFile2))
                {
                    J2534InitParameters JSettings = LoadJ2534Settings(AppSettings.LoggerJ2534SettingsFile2);
                    LoadJ2534InitParameters2(JSettings);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LoadPorts line " + line + ": " + ex.Message);
            }
        }

        private void LoadPorts(bool LoadDefaults)
        {
            try
            {
                comboSerialPort.Items.Clear();
                comboWBport.Items.Clear();
                if (IsRunningUnderWine())
                {
/*                    chkFTDI.Checked = false;
                    chkFTDI.Enabled = false;
                    j2534OptionsGroupBox.Enabled = false;
                    j2534RadioButton.Enabled = false;
                    serialRadioButton.Checked = true;
*/
                    string fname = Path.Combine(Application.StartupPath, "serialports.txt");
                    if (File.Exists("serialports.txt"))
                    {
                        StreamReader sr = new StreamReader(fname);
                        string Line;
                        while ((Line = sr.ReadLine()) != null)
                        {
                            string[] portdata = Line.Split('\t');
                            if (portdata.Length == 3)
                            {
                                string s = portdata[2].ToUpper() + ": " + portdata[1];
                                comboSerialPort.Items.Add(s);
                                comboWBport.Items.Add(s);
                            }
                        }
                        sr.Close();
                        if (LoadDefaults)
                        {
                            if (!string.IsNullOrEmpty(AppSettings.LoggerComPort) && comboSerialPort.Items.Contains(AppSettings.LoggerComPort))
                                comboSerialPort.Text = AppSettings.LoggerComPort;
                            else
                                comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                            if (!string.IsNullOrEmpty(AppSettings.WBSerial) && comboWBport.Items.Contains(AppSettings.WBSerial))
                                comboWBport.Text = AppSettings.WBSerial;
                            else
                                comboWBport.Text = comboWBport.Items[0].ToString();
                        }
                    }
                    return;
                }
                if (radioFTDI.Checked)
                {
                    string[] ftdiDevs = new string[0];
                    Task.Factory.StartNew(() =>
                    {
                        ftdiDevs = FTDI_Finder.FindFtdiDevices().ToArray();
                        for (int retry = 0; retry < 100; retry++)
                        {
                            if (!ftdiDevs.Contains("Unknown: Failure"))
                            {
                                Debug.WriteLine("FTDI list ok after " + retry.ToString() + " retries");
                                break;
                            }
                            Thread.Sleep(retry * 100);
                            ftdiDevs = FTDI_Finder.FindFtdiDevices().ToArray();
                        }
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            if (ftdiDevs.Length > 0)
                            {
                                comboSerialPort.Items.AddRange(ftdiDevs);
                                if (LoadDefaults && !string.IsNullOrEmpty(AppSettings.LoggerFTDIPort) && comboSerialPort.Items.Contains(AppSettings.LoggerFTDIPort))
                                    comboSerialPort.Text = AppSettings.LoggerFTDIPort;
                                else
                                    comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                            }
                            else
                            {
                                comboSerialPort.Text = "";
                            }
                        });
                    });
                }
                else if (radioRS232.Checked)
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                    {
                        var portnames = SerialPort.GetPortNames();
                        var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                        List<string> portList = portnames.Select(n => n + ": " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                        if (portList.Count > 0)
                        {
                            foreach (string s in portList)
                            {
                                Console.WriteLine(s);
                                if (!comboSerialPort.Items.Contains(s))
                                {
                                    comboSerialPort.Items.Add(s);
                                    comboWBport.Items.Add(s);
                                }
                            }
                            if (LoadDefaults)
                            {
                                if (!string.IsNullOrEmpty(AppSettings.LoggerComPort) && comboSerialPort.Items.Contains(AppSettings.LoggerComPort))
                                    comboSerialPort.Text = AppSettings.LoggerComPort;
                                else
                                    comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                                if (!string.IsNullOrEmpty(AppSettings.WBSerial) && comboWBport.Items.Contains(AppSettings.WBSerial))
                                    comboWBport.Text = AppSettings.WBSerial;
                                else
                                    comboWBport.Text = comboWBport.Items[0].ToString();
                            }
                        }
                        else
                        {
                            comboSerialPort.Text = "";
                            comboWBport.Text = "";
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
                Debug.WriteLine("Error, LoadPorts line " + line + ": " + ex.Message);
            }
        }

        private void LoadJ2534Protocols()
        {
            //comboJ2534Protocol.Items.Clear();
            try
            {
                J2534DotNet.J2534Device dev = jDevList[comboJ2534DLL.SelectedIndex];

                comboJ2534Protocol.Items.Clear();
                comboJ2534Protocol2.Items.Clear();
                foreach (string item in dev.Protocols)
                {
                    comboJ2534Protocol.Items.Add(item);
                    comboJ2534Protocol2.Items.Add(item);
                }

                comboJ2534Init.Items.Clear();
                comboJ2534Init2.Items.Clear();
                foreach (string item in Enum.GetNames(typeof(KInit)))
                {
                    comboJ2534Init.Items.Add(item);
                    comboJ2534Init2.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LoadJ2534Protocols line " + line + ": " + ex.Message);
            }
        }

        private void LoadJ2534Baudrates()
        {
            try
            {
                comboJ2534Baudrate.Items.Clear();
                foreach (string item in Enum.GetNames(typeof(J2534DotNet.BaudRate)))
                {
                    if (item.StartsWith(comboJ2534Protocol.Text.Replace("_PS", "").Replace("SW_","")))
                    {
                        comboJ2534Baudrate.Items.Add(item);
                    }
                }
                if (comboJ2534Baudrate.Items.Count > 0)
                {
                    comboJ2534Baudrate.SelectedIndex = 0;
                }
                else
                {
                    comboJ2534Baudrate.Text = "";
                }                
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LoadJ2534Baudrates line " + line + ": " + ex.Message);
            }
        }

        private void LoadJ2534Baudrates2()
        {
            try
            {
                comboJ2534Baudrate2.Items.Clear();
                foreach (string item in Enum.GetNames(typeof(J2534DotNet.BaudRate)))
                {
                    if (item.StartsWith(comboJ2534Protocol2.Text.Replace("_PS", "").Replace("SW_", "")))
                    {
                        comboJ2534Baudrate2.Items.Add(item);
                    }
                }
                if (comboJ2534Baudrate2.Items.Count > 0)
                {
                    comboJ2534Baudrate2.SelectedIndex = 0;
                }
                else
                {
                    comboJ2534Baudrate2.Text = "";
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LoadJ2534Baudrates2 line " + line + ": " + ex.Message);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (AppSettings.MainWindowPersistence)
                {
                    if (AppSettings.LoggerWindowSize.Width > 0 || AppSettings.LoggerWindowSize.Height > 0)
                    {
                        this.WindowState = AppSettings.LoggerWindowState;
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        this.Location = AppSettings.LoggerWindowPosition;
                        this.Size = AppSettings.LoggerWindowSize;
                    }

                }

                txtTstampFormat.Text = AppSettings.LoggerTimestampFormat;
                j2534RadioButton.Checked = AppSettings.LoggerUseJ2534;
                if (AppSettings.LoggerPortType == iPortType.Serial)
                {
                    radioRS232.Checked = true;
                    labelSerialport.Text = "Port:";
                }
                else if (AppSettings.LoggerPortType == iPortType.FTDI)
                {
                    radioFTDI.Checked = true;
                    labelSerialport.Text = "Port:";
                }
                else if (AppSettings.LoggerPortType == iPortType.TcpIP)
                {
                    radioTCPIP.Checked = true;
                    comboSerialPort.Text = AppSettings.LoggerDeviceUrl;
                    labelSerialport.Text = "IP:port";
                }
                comboSerialDeviceType.Items.Add(AvtDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(AvtLegacyDevice.DeviceType);
                comboSerialDeviceType.Items.Add(Elm327Device.DeviceType);
                comboSerialDeviceType.Items.Add(ElmDevice.DeviceType);
                comboSerialDeviceType.Items.Add(JetDevice.DeviceType);
                comboSerialDeviceType.Items.Add(OBDXProDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(UPX_OBD.DeviceType);

                comboSerialDeviceType.Text = AppSettings.LoggerSerialDeviceType;

                txtWBCanId.Text = AppSettings.WBCanID;
                comboWBType.DataSource = Enum.GetValues(typeof(WideBand.WBType));
                comboWBType.Text = AppSettings.Wbtype.ToString();
                if (AppSettings.Wbtype == WideBand.WBType.Elm327_CAN)
                {
                    txtWBCanId.Enabled = true;
                }
                txtWBCanId.Validating += txtWBCanId_TextChanged;
                LoadPorts(true);
                LoadJPorts();

                //chkAdvanced.Checked = AppSettings.LoggerShowAdvanced;
                chkVPWFilters.Checked = AppSettings.LoggerUseFilters;
                //chkEnableConsole.Checked = AppSettings.LoggerEnableConsole;
                chkConsoleTimestamps.Checked = AppSettings.LoggerConsoleTimestamps;
                chkVpwDevTimestamps.Checked = AppSettings.LoggerConsoleDevTimestamps;
                chkJConsoleTimestamps.Checked = AppSettings.JConsoleTimestamps;
                chkJconsoleDevTimestamps.Checked = AppSettings.JConsoleDevTimestamps;
                chkJConsole4x.Checked = AppSettings.JConsole4x;
                chkConsole4x.Checked = AppSettings.LoggerConsole4x;
                chkTestPidCompatibility.Checked = AppSettings.LoggerTestPidCompatibility;
                comboJ2534DLL.Text = AppSettings.JConsoleDevice;
                numConsoleScriptDelay.Value = AppSettings.LoggerScriptDelay;
                chkStartJ2534Process.Checked = AppSettings.LoggerStartJ2534Process;
                chkJ2534ServerVisible.Checked = AppSettings.LoggerJ2534ProcessVisible;
                chkJ2534ServerCreateDebugLog.Checked = AppSettings.LoggerJ2534ProcessWriteDebugLog;
                chkAutoDisconnect.Checked = AppSettings.LoggerAutoDisconnect;
                numRetryDelay.Value = AppSettings.RetryWriteDelay;
                numRetryTimes.Value = AppSettings.RetryWriteTimes;
                j2534OptionsGroupBox.Enabled = AppSettings.LoggerUseJ2534;
                txtPcmAddress.Text = AppSettings.LoggerCanPcmAddress.ToString("X4");
                comboConnectFlag.Text = AppSettings.LoggerConnectFlag.ToString();
                numAnalyzerNumMessages.Value = AppSettings.AnalyzerNumMessages;
                if (AppSettings.LoggerRestartAfterSeconds > 0)
                    chkRestartLogging.Checked = true;
                else
                    chkRestartLogging.Checked = false;
                numRestartLoggingAfter.Value = Math.Abs(AppSettings.LoggerRestartAfterSeconds);

                if (AppSettings.loggerProtocol == LoggingProtocol.HSCAN)
                {
                    radioHSCAN.Checked = true;
                    groupCanParams.Enabled = true;
                }
                else if (AppSettings.loggerProtocol == LoggingProtocol.LSCAN)
                {
                    radioLSCan.Checked = true;
                    groupCanParams.Enabled = true;
                }
                else if (AppSettings.loggerProtocol == LoggingProtocol.VPW)
                {
                    radioVPW.Checked = true;
                    groupCanParams.Enabled = false;
                }
                chkStartPeriodic.Checked = AppSettings.LoggerStartPeriodic;
                chkWakeUp.Checked = AppSettings.LoggerWakeUp;

                comboStartLoggingKey.Items.Clear();
                foreach (string keyTxt in Enum.GetNames(typeof(Keys)))
                {
                    comboStartLoggingKey.Items.Add(keyTxt);
                }
                comboStartLoggingKey.Text = AppSettings.LoggerStartKey.ToString();
                if (AppSettings.LoggerConsoleFont != null)
                {
                    richVPWmessages.Font = AppSettings.LoggerConsoleFont.ToFont();
                }

                if (!string.IsNullOrEmpty(AppSettings.LoggerLogFolder))
                {
                    txtLogFolder.Text = AppSettings.LoggerLogFolder;
                }
                else
                {
                    txtLogFolder.Text = Path.Combine(Application.StartupPath, "Logger", "Log");
                }

                comboResponseMode.DataSource = Enum.GetValues(typeof(ResponseTypes));

                if (!string.IsNullOrEmpty(AppSettings.LoggerLogSeparator))
                    txtLogSeparator.Text = AppSettings.LoggerLogSeparator;
                else
                    txtLogSeparator.Text = ",";
                if (!string.IsNullOrEmpty(AppSettings.LoggerDecimalSeparator))
                    txtDecimalSeparator.Text = AppSettings.LoggerDecimalSeparator;
                else
                    txtDecimalSeparator.Text = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if (!string.IsNullOrEmpty(AppSettings.LoggerResponseMode))
                {
                    comboResponseMode.Text = AppSettings.LoggerResponseMode;
                }
                comboBaudRate.Text = AppSettings.LoggerBaudRate.ToString();
                LoadModuleList();
                datalogger.PidProfile = new List<PidConfig>();  //OLD profile
                LoadOsPidFiles();
                if (PCM != null && !string.IsNullOrEmpty(PCM.OS))
                {
                    FilterPidsByBin(PCM);
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppSettings.LoggerLastFilterOS))
                    {
                        comboFilterByOS.Text = AppSettings.LoggerLastFilterOS;
                        FilterPidsByOS();
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void LoadModuleList()
        {
            analyzer = new Analyzer();

            if (radioVPW.Checked)
            {
                //comboModule.DataSource = new BindingSource(analyzer.PhysAddresses, null);
                List<KeyValuePair<byte, string>> modules = analyzer.PhysAddresses.ToList(); 
                comboModule.DataSource = modules;
                comboModule.DisplayMember = "Value";
                comboModule.ValueMember = "Key";
                comboModule.Text = "ECU";
                labelProtocol.Text = "VPW";
            }
            else
            {
                List<KeyValuePair<ushort, string>> modules = new List<KeyValuePair<ushort, string>>();
                for (int c=0; c< CanModules.Count; c++)
                {
                    KeyValuePair<ushort, string> item = new KeyValuePair<ushort, string>((ushort)CanModules[c].RequestID, CanModules[c].ModuleName + " [" + CanModules[c].RequestID.ToString("X4") + "]");
                    modules.Add(item);
                }
                comboModule.DataSource = modules;
                comboModule.DisplayMember = "Value";
                comboModule.ValueMember = "Key";
                comboModule.Text = "ECM [07E0]";
                labelProtocol.Text = "CAN";
            }
        }

        private void dataGridProfile_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //Debug.WriteLine(e.Exception);
        }

        public void ReloadPidParams( bool LoadEditor, bool LoadProfile)
        {
            try
            {
                if (LoadEditor)
                {
                    dataGridViewPidEditor.DataSource = null;
                    if (!chkPidEditorShowCustom.Checked && !chkPidEditorShowMaster.Checked)
                    {
                        return;
                    }
                    List<LogParam.PidParameter> parmlist = datalogger.PidParams;
                    if (chkPidEditorProfileOnly.Checked)
                    {
                        parmlist = new List<LogParam.PidParameter>();
                        foreach(LogParam.PidSettings pidSettings in datalogger.SelectedPids)
                        {
                            LogParam.PidParameter  parm = pidSettings.Parameter;
                            parmlist.Add(parm);
                            if (parm.Type == LogParam.DefineBy.Math && parm.Pids != null && parm.Pids.Count > 0)
                            {
                                parmlist.AddRange(parm.GetLinkedPids());
                            }
                        }
                    }
                    if (chkPidEditorShowMaster.Checked && chkPidEditorShowCustom.Checked)
                    {
                        //All set
                    }
                    else if (chkPidEditorShowCustom.Checked)
                    {
                        parmlist = parmlist.Where(X => X.Custom == true).ToList();
                    }
                    else //Only master checked
                    {
                        parmlist = parmlist.Where(X => X.Custom == false).ToList();
                    }
                    if (!string.IsNullOrEmpty(txtPidEditorFilter.Text))
                    {
                        string searchStr = txtPidEditorFilter.Text.ToLower();
                        parmlist = parmlist.Where(X => X.Name.ToLower().Contains(searchStr)).ToList();
                    }
                    if (!string.IsNullOrEmpty(sortBy_PidEditor))
                    {
                        if (strSortOrder_PidEditor == SortOrder.Ascending)
                            parmlist = parmlist.OrderBy(x => typeof(LogParam.PidParameter).GetProperty(sortBy_PidEditor).GetValue(x, null)).ToList();
                        else
                            parmlist = parmlist.OrderByDescending(x => typeof(LogParam.PidParameter).GetProperty(sortBy_PidEditor).GetValue(x, null)).ToList();
                    }
                    pidparams = new BindingList<LogParam.PidParameter>(parmlist);
                    dataGridViewPidEditor.DataSource = pidparams;
                }
                if (LoadProfile)
                {
                    if (dataGridProfile.SelectedCells.Count > 0)
                    {
                        profilecurrentrow = dataGridProfile.CurrentCell.RowIndex;
                        profilecurrentcell = dataGridProfile.CurrentCell.ColumnIndex;
                    }
                    dataGridProfile.Columns.Clear();
                    dataGridProfile.DataSource = null;
                    List<LogParam.PidParameter> parmlist2 = datalogger.PidParams;
                    if (chkFilterParamsByOS.Checked && SupportedPids != null)
                    {
                        List<LogParam.PidParameter> parmlist3 = new List<LogParam.PidParameter>();
                        foreach (LogParam.PidParameter prm in parmlist2)
                        {

                            switch (prm.Type)
                            {
                                case LogParam.DefineBy.Pid:
                                    if (SupportedPids.Contains(prm.Address))
                                    {
                                        parmlist3.Add(prm);
                                    }
                                    break;
                                case LogParam.DefineBy.Address:
                                    LogParam.Location loc = prm.Locations.Where(X => X.Os == comboFilterByOS.Text).FirstOrDefault();
                                    if (loc != null)
                                    {
                                        parmlist3.Add(prm);
                                    }
                                    else
                                    {
                                        foreach (LogParam.Location locs in prm.Locations)
                                        {
                                            if (locs.Os.Contains("search"))
                                            {
                                                parmlist3.Add(prm);
                                            }
                                        }
                                    }
                                    break;
                                case LogParam.DefineBy.Math:
                                    bool pidsOk = true;
                                    foreach (LogParam.LogPid logPid in prm.Pids)
                                    {
                                        if (!parmlist3.Contains(logPid.Parameter))
                                        {
                                            pidsOk = false;
                                            break;
                                        }
                                    }
                                    if (pidsOk)
                                    {
                                        parmlist3.Add(prm);
                                    }
                                    break;
                                default:
                                    parmlist3.Add(prm);
                                    break;
                            }

                        }
                        parmlist2 = parmlist3;
                    }
                    if (!string.IsNullOrEmpty(txtParamSearch.Text))
                    {
                        string searchStr = txtParamSearch.Text.ToLower();
                        parmlist2 = parmlist2.Where(X => X.Name.ToLower().Contains(searchStr)).ToList();
                    }
                    if (!string.IsNullOrEmpty(sortBy_Profile))
                    {
                        if (strSortOrder_Profile == SortOrder.Ascending)
                            parmlist2 = parmlist2.OrderBy(x => typeof(LogParam.PidParameter).GetProperty(sortBy_Profile).GetValue(x, null)).ToList();
                        else
                            parmlist2 = parmlist2.OrderByDescending(x => typeof(LogParam.PidParameter).GetProperty(sortBy_Profile).GetValue(x, null)).ToList();
                    }
                    profilebl = new BindingList<LogParam.PidParameter>(parmlist2);
                    dataGridProfile.DataSource = profilebl;
                    Application.DoEvents();
                    Debug.WriteLine("Loading profile, params: " + parmlist2.Count.ToString());
                }
                DisablePidEditorChanges = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                datalogger.Receiver.SetReceiverPaused(false);
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
            DrawingControl.ResumeDrawing(dataGridProfile);
        }
       

        private void frmLogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (AppSettings.ConfirmProgramExit)
                {
                    if (MessageBox.Show("Close logger?", "Exit now?", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                Debug.WriteLine("Closing frmLogger");
                WB.Discard();
                if (datalogger != null && datalogger.LogDevice != null)
                {
                    if (datalogger.Receiver != null)
                    {
                        datalogger.Receiver.StopReceiveLoop();
                    }
                    if (datalogger.LogRunning)
                    {
                        datalogger.StopLogging();
                    }
                    //Disconnect(true);
                    datalogger.LogDevice.Dispose();
                    datalogger.LogDevice = null;
                }
                if (jConsole != null && jConsole.JDevice != null && jConsole.Connected)
                {
                    DisconnectJConsole(true);
                }
                if (ProfileDirty)
                {
                    DialogResult dialogResult = MessageBox.Show("Save profile modifications?", "Save profile", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        SelectSaveProfile();
                    }
                }
                if (AppSettings.MainWindowPersistence)
                {
                    AppSettings.LoggerWindowState = this.WindowState;
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        AppSettings.LoggerWindowPosition = this.Location;
                        AppSettings.LoggerWindowSize = this.Size;
                    }
                    else
                    {
                        AppSettings.LoggerWindowPosition = this.RestoreBounds.Location;
                        AppSettings.LoggerWindowSize = this.RestoreBounds.Size;
                    }
                    AppSettings.Save();
                }
                Thread.Sleep(100);
            }
            catch { }
        }

        private void RestartLogging(bool Reconnect)
        {
            if (chkLoggingStarted.Checked)
            {
                timerShowData.Enabled = false;
                datalogger.StopLogging();
                if (Reconnect)
                {
                    Disconnect(true);
                    Connect(radioVPW.Checked, false, false, oscript);
                    Debug.WriteLine("1s sleep between connect & logging...");
                    Thread.Sleep(1000);
                    StartLogging(false);
                }
                else
                {
                    StartLogging(chkTestPidCompatibility.Checked);
                }
                timerShowData.Enabled = true;
            }
        }
        private void timerShowData_Tick(object sender, EventArgs e)
        {
            try
            {
                if (RequestRestartLogging)
                {
                    RequestRestartLogging = false;
                    RestartLogging(false);
                    return;
                }
                if (!datalogger.LogRunning)
                {
                    if (chkRestartLogging.Checked && chkLoggingStarted.Checked)
                    {
                        if (DisconnectTime == DateTime.MaxValue)
                        {
                            DisconnectTime = DateTime.Now;
                        }
                        labelProgress.Text = "Restarting logging in " + 
                            (AppSettings.LoggerRestartAfterSeconds - DateTime.Now.Subtract(DisconnectTime).TotalSeconds).ToString("0") +
                            " seconds...";
                        if (DateTime.Now.Subtract(DisconnectTime) > TimeSpan.FromSeconds(AppSettings.LoggerRestartAfterSeconds))
                        {
                            RestartLogging(true);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Logging stopped, clean up");
                        StopLogging(false, true);
                    }
                    return;
                }
                string[] LastCalcValues;
                LastCalcValues = datalogger.LastCalculatedStringValues;
                for (int row=0; row< LastCalcValues.Length;row++)
                {
                    dataGridLogData.Rows[row].Cells["Value"].Value = LastCalcValues[row];
                    dataGridSelectedPids.Rows[row].Cells["Value"].Value = LastCalcValues[row];
                    dataGridLogData.Rows[row].Cells["Hz"].Value = datalogger.SelectedPids[row].Herz().ToString("0.0");
                }

                TimeSpan elapsed = DateTime.Now.Subtract(datalogger.LogStartTime);
                int speed = (int)(datalogger.slothandler.ReceivedRows / elapsed.TotalSeconds);
                string elapsedStr = elapsed.Hours.ToString() + ":" + elapsed.Minutes.ToString() + ":" + elapsed.Seconds.ToString();
                labelProgress.Text = "Elapsed: " + elapsedStr + ", Received: " + datalogger.slothandler.ReceivedRows.ToString() + " rows (" + speed.ToString() + " /s)";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void SelectProfile()
        {
            try
            {
                string defname = Path.Combine(Application.StartupPath, "Logger", "Profiles", "profile1.xml");
                if (!string.IsNullOrEmpty(profileFile) && File.Exists(profileFile))
                {
                    defname = profileFile;
                }
                string fname = SelectFile("Select profile file", ProfileFilter, defname);
                if (fname.Length == 0)
                {
                    return;
                }
                LoadProfile(fname);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void SelectSaveProfile(string fname = "")
        {
            try
            {
                dataGridProfile.EndEdit();
                //GetPidSelections();
                string defname = Path.Combine(Application.StartupPath, "Logger", "Profiles", "profile1.xml");
                if (!string.IsNullOrEmpty(profileFile))
                {
                    defname = profileFile;
                }
                if (!string.IsNullOrEmpty(comboProfilePlatform.Text))
                {
                    string platform = comboProfilePlatform.Text.Replace("_Undefined", "").Replace("_All", "");

                    string platformFoler = Path.Combine(Application.StartupPath, "Logger", "Profiles", platform);
                    if (!Directory.Exists(platformFoler))
                    {
                        Directory.CreateDirectory(platformFoler);
                    }

                    if (string.IsNullOrEmpty(comboProfileCategory.Text))
                    {
                        defname = Path.Combine(platformFoler, "profile1.xml");
                    }
                    else
                    {
                        string category = comboProfileCategory.Text.Replace("_Undefined", "").Replace("_All", "");
                        string prFolder = Path.Combine(platformFoler, category);
                        if (!Directory.Exists(prFolder))
                        {
                            Directory.CreateDirectory(prFolder);
                        }
                        defname = Path.Combine(platformFoler, comboProfileCategory.Text, "profile1.xml");
                    }
                }
                if (string.IsNullOrEmpty(fname))
                {
                    fname = SelectSaveFile(ProfileFilter, defname);
                }
                if (fname.Length == 0)
                    return;
                profileFile = fname;
                comboProfileFilename.Text = Path.GetFileName(fname);
                this.Text = "Logger - " + profileFile;
                LogParamFile.SaveProfile(fname, datalogger.SelectedPids);
                AppSettings.LoggerLastProfile = profileFile;
                SetProfileDirty(false,false);
                LoadProfileList();
                FillPlatformCombo();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void loadProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectProfile();
        }

        private void saveProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectSaveProfile(profileFile);
        }

        private bool CheckMaxPids()
        {
            try
            {
                List<int> pids = new List<int>();
                int bytesTotal = 0;
                int maxBytes = 48;
                for (int i = 0; i < datalogger.SelectedPids.Count; i++)
                {
                    LogParam.PidSettings pidProfile = datalogger.SelectedPids[i];
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    if (parm.Type == LogParam.DefineBy.Pid || parm.Type == LogParam.DefineBy.Address)
                    {
                        if (!pids.Contains(parm.Address))
                        {
                            pids.Add(parm.Address);
                            bytesTotal += GetElementSize((InDataType)parm.DataType);
                        }
                    }
                    else if (parm.Type == LogParam.DefineBy.Math)
                    {
                        foreach(LogParam.LogPid lPid in parm.Pids)
                        {
                            LogParam.PidParameter pParm = lPid.Parameter;
                            if (!pids.Contains(pParm.Address))
                            {
                                pids.Add(pParm.Address);
                                bytesTotal += GetElementSize((InDataType)pParm.DataType);
                            }

                        }
                    }
                }
                if (!radioVPW.Checked)
                {
                    maxBytes = 128;
                }
                labelProgress.Text = "Profile size: " + bytesTotal.ToString() + " bytes, Maximum: " + maxBytes.ToString();
                if (bytesTotal > maxBytes)
                {
                    LoggerBold("Profile have total: " + bytesTotal.ToString() + " bytes, maximum is " + maxBytes.ToString() + " bytes");
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
                LoggerBold("Connection failed. Check settings");
                return false;
            }
        }
        private void UpdateProfile()
        {
            if (HstForm != null && HstForm.Visible)
            {
                //HstForm.SetupLiveParameters();
            }
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.SetupLiveGraphics();
            }

        }

        private void AddPidToSelectedPidGrid(LogParam.PidSettings pidSettings)
        {
            int row = dataGridSelectedPids.Rows.Add();
            dataGridSelectedPids.Rows[row].Cells["Pid"].Value = pidSettings.Parameter.Name;
            dataGridSelectedPids.Rows[row].Tag = pidSettings;
            dataGridSelectedPids.Rows[row].Cells["Id"].Value = pidSettings.Parameter.Id;
            DataGridViewComboBoxCell dgcC = new DataGridViewComboBoxCell();
            dgcC.DataSource = pidSettings.Parameter.Conversions;
            dgcC.ValueMember = "Units";
            dgcC.DisplayMember = "Units";
            dataGridSelectedPids.Rows[row].Cells["Units"] = dgcC;
            if (pidSettings.Units != null)
            {
                dataGridSelectedPids.Rows[row].Cells["Units"].Value = pidSettings.Units;
            }
        }
        private void SetupLogDataGrid()
        {
            try
            {
                if (datalogger.LogRunning || timerPlayback.Enabled)
                    return; //Dont change settings while logging or playback
                dataGridLogData.Rows.Clear(); 
                dataGridLogData.Columns[0].ReadOnly = true;
                for (int row = 0; row < datalogger.SelectedPids.Count; row++)
                {
                    LogParam.PidSettings pidProfile = datalogger.SelectedPids[row];
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    {
                        dataGridLogData.Rows.Add();
                        dataGridLogData.Rows[row].Tag = pidProfile;
                        dataGridLogData.Rows[row].HeaderCell.Value = parm.Name;
                        
                        if (pidProfile.Units != null)
                        {
                            dataGridLogData.Rows[row].Cells["Units"].Value = pidProfile.Units.Units;
                        }
                    }
                }
                Application.DoEvents();
                this.Refresh();
                dataGridLogData.AutoResizeColumns();
                dataGridLogData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                dataGridLogData.Columns[0].Width = 100;
                dataGridSelectedPids.Rows.Clear();
                for (int row = 0; row < datalogger.SelectedPids.Count; row++)
                {
                    LogParam.PidSettings pidSettings = datalogger.SelectedPids[row];
                    AddPidToSelectedPidGrid(pidSettings);
                }
                Application.DoEvents();
                dataGridSelectedPids.RowHeadersWidth = 4;
                dataGridSelectedPids.Columns[0].Width = 20;
                //dataGridSelectedPids.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                dataGridSelectedPids.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                CheckMaxPids();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void SaveSettings()
        {
            if (radioRS232.Checked)
            {
                AppSettings.LoggerPortType = iPortType.Serial;
                AppSettings.LoggerComPort = comboSerialPort.Text;
            }
            else if (radioFTDI.Checked )
            {
                AppSettings.LoggerPortType = iPortType.FTDI;
                AppSettings.LoggerFTDIPort = comboSerialPort.Text;
            }
            else if (radioTCPIP.Checked)
            {
                AppSettings.LoggerPortType = iPortType.TcpIP;
                AppSettings.LoggerDeviceUrl = comboSerialPort.Text;
            }
            AppSettings.LoggerUseJ2534 = j2534RadioButton.Checked;
            AppSettings.LoggerJ2534Device = j2534DeviceList.Text;
            AppSettings.LoggerSerialDeviceType = comboSerialDeviceType.Text;
            AppSettings.LoggerBaudRate = Convert.ToInt32(comboBaudRate.Text);
            AppSettings.LoggerJ2534ProcessWriteDebugLog = chkJ2534ServerCreateDebugLog.Checked;

            if (txtLogFolder.Text.Length > 0)
            {
                AppSettings.LoggerLogFolder = txtLogFolder.Text;
                AppSettings.LoggerLogSeparator = txtLogSeparator.Text;
            }
            AppSettings.LoggerLastProfile = profileFile;
            AppSettings.LoggerResponseMode = comboResponseMode.Text;
            //AppSettings.LoggerShowAdvanced = chkAdvanced.Checked;
            AppSettings.LoggerUseFilters = chkVPWFilters.Checked;
            //AppSettings.LoggerEnableConsole = chkEnableConsole.Checked;
            AppSettings.LoggerConsoleTimestamps = chkConsoleTimestamps.Checked;
            AppSettings.LoggerConsoleDevTimestamps = chkVpwDevTimestamps.Checked;
            AppSettings.LoggerConsoleFont = SerializableFont.FromFont(richVPWmessages.Font);
            AppSettings.LoggerScriptDelay = (int) numConsoleScriptDelay.Value;
            AppSettings.LoggerConsole4x = chkConsole4x.Checked;
            AppSettings.JConsole4x = chkJConsole4x.Checked;
            AppSettings.JConsoleTimestamps = chkJConsoleTimestamps.Checked;
            AppSettings.JConsoleDevTimestamps = chkJconsoleDevTimestamps.Checked;
            AppSettings.JConsoleDevice = comboJ2534DLL.Text;
            AppSettings.LoggerStartJ2534Process = chkStartJ2534Process.Checked;
            AppSettings.LoggerJ2534ProcessVisible = chkJ2534ServerVisible.Checked;
            AppSettings.LoggerAutoDisconnect = chkAutoDisconnect.Checked;
            if (radioVPW.Checked)
                AppSettings.loggerProtocol = LoggingProtocol.VPW;
            else if (radioHSCAN.Checked)
                AppSettings.loggerProtocol = LoggingProtocol.HSCAN;
            else if (radioLSCan.Checked)
                AppSettings.loggerProtocol = LoggingProtocol.LSCAN;
            AppSettings.LoggerStartPeriodic = chkStartPeriodic.Checked;
            AppSettings.LoggerWakeUp = chkWakeUp.Checked;
            chkTestPidCompatibility.Checked = chkTestPidCompatibility.Checked;
            if (HexToUshort(txtPcmAddress.Text, out ushort pcmaddr))
                AppSettings.LoggerCanPcmAddress = pcmaddr;
            AppSettings.LoggerConnectFlag = (ConnectFlag)comboConnectFlag.SelectedValue;
            AppSettings.Save();

        }

        private void StopLogging(bool disconnect, bool StartReceiver)
        {
            try
            {
                Logger("Stopping, wait...");
                if (GraphicsForm != null && GraphicsForm.Visible)
                {
                    GraphicsForm.StopLiveUpdate();
                }
                timerShowData.Enabled = false;
                chkContinuousPidTest.Enabled = true;
                //btnTestEnabledPids.Enabled = true;
                //btnQueryPid.Enabled = true;
                //btnTestMasterlistPids.Enabled = true;
                Application.DoEvents();
                datalogger.StopLogging();
                btnStartStop.Text = "Start logging (" + comboStartLoggingKey.Text + ")";
                labelProgress.Text = "Logging stopped";
                chkLoggingStarted.Checked = false;
                groupLogSettings.Enabled = true;
                groupDTC.Enabled = true;
                hScrollPlayback.Maximum = datalogger.LogDataBuffer.Count;
                //btnGetVINCode.Enabled = true;
                if (!disconnect && StartReceiver)
                {
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port, false,datalogger.AnalyzerRunning);
                }               
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public bool Connect(bool UseVPW, bool StartReceiver, bool ShowOs, OBDScript Oscript)
        {
            try
            {

                if (datalogger.Connected)
                {
                    return true;
                }
                if (UseVPW)
                {
                    Logger("Connecting (VPW)...");
                    datalogger.useVPWFilters = chkVPWFilters.Checked;
                    //Set for default values:
                    datalogger.CanDevice = CANQuery.GetDeviceAddresses(AppSettings.LoggerCanPcmAddress);
                }
                else
                {
                    Logger("Connecting (CAN)...");
                    datalogger.useVPWFilters = false;

                    if (HexToUshort(txtPcmAddress.Text, out ushort canid))
                    {
                        datalogger.CanDevice = CANQuery.GetDeviceAddresses(canid);
                    }
                    else
                    {
                        datalogger.CanDevice = CANQuery.GetDeviceAddresses(AppSettings.LoggerCanPcmAddress);
                    }
                }
                Application.DoEvents();
                if (serialRadioButton.Checked)
                {
                    iPortType portType = iPortType.Serial;
                    if (radioTCPIP.Checked)
                    {
                        portType = iPortType.TcpIP;
                        datalogger.LogDevice = datalogger.CreateSerialDevice(comboSerialPort.Text, comboSerialDeviceType.Text, portType);
                    }
                    else
                    {
                        if (radioFTDI.Checked)
                        {
                            portType = iPortType.FTDI;
                        }
                        CurrentPortName = comboSerialPort.Text;
                        datalogger.LogDevice = datalogger.CreateSerialDevice(CurrentPortName, comboSerialDeviceType.Text, portType);
                    }
                }
                else
                {
                    if (jConsole != null && jConsole.Connected && j2534DeviceList.Text == comboJ2534DLL.Text)
                    {
                        LoggerBold("Device in use");
                        return false;
                    }
                    J2534DotNet.J2534Device dev = jDevList[j2534DeviceList.SelectedIndex];
                    //passThru.LoadLibrary(dev);
                    if (chkStartJ2534Process.Checked)
                        datalogger.LogDevice = new J2534Client(j2534DeviceList.SelectedIndex);
                    else
                        datalogger.LogDevice = new J2534Device(dev);
                    //datalogger.LogDevice.SetReadTimeout(100);
                }
                datalogger.LogDevice.MsgReceived += LogDevice_DTC_MsgReceived;
                datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                J2534InitParameters jParams;
                if (UseVPW)
                {
                    jParams = new J2534InitParameters(true);
                    jParams.Protocol = ProtocolID.J1850VPW;
                    jParams.Baudrate =  BaudRate.J1850VPW_10400;
                }
                else
                {
                    jParams = new J2534InitParameters();
                    jParams.SeparateProtoByChannel = true;
                    if (radioHSCAN.Checked)
                    {
                        jParams.Sconfigs = "CAN_MIXED_FORMAT=1";
                        jParams.Protocol = ProtocolID.ISO15765;
                        jParams.Baudrate = BaudRate.ISO15765_500000;
                    }
                    else
                    {
                        //jParams.Sconfigs = "J1962_PINS=$00000100|CAN_MIXED_FORMAT=1";
                        jParams.Sconfigs = "J1962_PINS=$00000100|CAN_MIXED_FORMAT=1";
                        jParams.Protocol = ProtocolID.SW_ISO15765_PS;
                        jParams.Baudrate = BaudRate.ISO15765_33K3;
                    }
                    jParams.CanPCM = datalogger.CanDevice;
                    jParams.Connectflag = (ConnectFlag)comboConnectFlag.SelectedValue;
                    jParams.PassFilters = "Type:FLOW_CONTROL_FILTER,Name:CANFlasherFlow" + Environment.NewLine;
                    jParams.PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "Pattern:" + datalogger.CanDevice.ResID.ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "FlowControl:" + datalogger.CanDevice.RequestID.ToString("X8") + ",RxStatus:NONE,TxFlags:NONE" + Environment.NewLine;
                }
                if (!datalogger.LogDevice.Initialize(Convert.ToInt32(comboBaudRate.Text), jParams))
                {
                    if (datalogger.port != null)
                    {
                        datalogger.port.ClosePort();
                        datalogger.port.Dispose();
                    }
                    datalogger.LogDevice.Dispose();
                    datalogger.LogDevice = null;
                    return false;
                }
                if (Oscript == null)
                {
                    oscript = new OBDScript(datalogger.LogDevice, null);
                }
                else
                {
                    oscript = Oscript;
                    oscript.device = datalogger.LogDevice;
                }
                datalogger.LogDevice.Enable4xReadWrite = chkConsole4x.Checked;

                if (serialRadioButton.Checked)
                {
                    datalogger.LogDevice.SetWriteTimeout(AppSettings.TimeoutSerialPortWrite);
                    datalogger.LogDevice.SetReadTimeout(AppSettings.TimeoutSerialPortRead);
                }
                else
                {
                    datalogger.LogDevice.SetWriteTimeout(AppSettings.TimeoutJ2534Write);
                    datalogger.LogDevice.SetReadTimeout(AppSettings.TimeoutReceive);
                }

                if (!UseVPW)
                {
                    jParams = new J2534InitParameters();
                    if (radioHSCAN.Checked)
                    {
                        jParams.Protocol = ProtocolID.CAN;
                        jParams.Baudrate = BaudRate.ISO15765_500000;
                    }
                    else
                    {
                        //datalogger.LogDevice.SetJ2534Configs( "J1962_PINS=$00000100",false);
                        jParams.Protocol = ProtocolID.SW_CAN_PS;
                        jParams.Baudrate = BaudRate.ISO15765_33K3;
                    }
                    jParams.Secondary = true;
                    jParams.SeparateProtoByChannel = true;
                    jParams.UsePrimaryChannel = true;
                    jParams.PassFilters = "Type:PASS_FILTER,Name:CANLoggerPass" + Environment.NewLine;
                    jParams.PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "Pattern:" + datalogger.CanDevice.DiagID.ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    datalogger.LogDevice.ConnectSecondaryProtocol(jParams);
                    if(radioLSCan.Checked && chkWakeUp.Checked)
                    {
                        byte[] wakeMsg = new byte[] { 00, 00, 01, 00 };
                        OBDMessage wMsg = new OBDMessage(wakeMsg);
                        wMsg.Txflag = TxFlag.SW_CAN_HV_TX;
                        wMsg.SecondaryProtocol = true;
                        datalogger.LogDevice.SendMessage(wMsg, 0);
                        Thread.Sleep(500);
                    }
                    if (chkStartPeriodic.Checked)
                    {
                        string pMsg = "00 00 01 01 FE 3E:2500:ISO15765_FRAME_PAD|ISO15765_ADDR_TYPE";
                        datalogger.LogDevice.StartPeriodicMsg(pMsg, false);
                    }
                    if (datalogger.LogDevice.LogDeviceType == DataLogger.LoggingDevType.Elm)
                    {
                        Logger("Message filtering enabled for CAN ELM device. Functions limited");
                        // return;
                    }
                }
                string os = "";
                if (ShowOs)
                {
                   os = datalogger.QueryPcmOS();
                }
                if (!string.IsNullOrEmpty(os))
                {
                    labelConnected.Text = "Connected - OS: " + os;
                    if (comboFilterByOS.Items.Contains(os))
                    {
                        comboFilterByOS.Text = os;
                    }
                }
                else
                {
                    labelConnected.Text = "Connected";
                }
                Logger("Connected");
                btnConnect.Text = "Disconnect";
                datalogger.Connected = true;
                SaveSettings();
                if (StartReceiver)
                {
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port, false,datalogger.AnalyzerRunning);
                }
                Application.DoEvents();
                groupHWSettings.Enabled = false;
                groupProtocol.Enabled = false;
                j2534OptionsGroupBox.Enabled = false;
                timerDeviceStatus.Enabled = true;
                timerShowLogTxt.Interval = AppSettings.LoggerConsoleDisplayInterval;
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
                LoggerBold("Connection failed. Check settings");
                return false;
            }
        }
        public bool ReConnect()
        {
            try
            {

                J2534InitParameters jParams;
                jParams = new J2534InitParameters();
                jParams.SeparateProtoByChannel = true;
                if (radioVPW.Checked)
                {
                    jParams.Protocol = ProtocolID.J1850VPW;
                    jParams.Baudrate = BaudRate.J1850VPW_10400;
                }
                else
                {
                    if (radioHSCAN.Checked)
                    {
                        jParams.Sconfigs = "CAN_MIXED_FORMAT=1";
                        jParams.Protocol = ProtocolID.ISO15765;
                        jParams.Baudrate = BaudRate.ISO15765_500000;
                    }
                    else
                    {
                        //jParams.Sconfigs = "J1962_PINS=$00000100|CAN_MIXED_FORMAT=1";
                        jParams.Sconfigs = "J1962_PINS=$00000100|CAN_MIXED_FORMAT=1";
                        jParams.Protocol = ProtocolID.SW_ISO15765_PS;
                        jParams.Baudrate = BaudRate.ISO15765_33K3;
                    }
                    jParams.CanPCM = datalogger.CanDevice;
                    jParams.Connectflag = (ConnectFlag)comboConnectFlag.SelectedValue;
                    jParams.PassFilters = "Type:FLOW_CONTROL_FILTER,Name:CANFlasherFlow" + Environment.NewLine;
                    jParams.PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "Pattern:0000" + datalogger.CanDevice.ResID.ToString("X4") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "FlowControl:0000" + datalogger.CanDevice.RequestID.ToString("X4") + ",RxStatus:NONE,TxFlags:NONE" + Environment.NewLine;
                }
                if (!datalogger.LogDevice.PassthruConnect(jParams))
                {
                    if (datalogger.port != null)
                    {
                        datalogger.port.ClosePort();
                        datalogger.port.Dispose();
                    }
                    datalogger.LogDevice.Dispose();
                    datalogger.LogDevice = null;
                    return false;
                }
                datalogger.LogDevice.Enable4xReadWrite = chkConsole4x.Checked;

                datalogger.LogDevice.SetWriteTimeout(AppSettings.TimeoutJ2534Write);
                datalogger.LogDevice.SetReadTimeout(AppSettings.TimeoutReceive);

                jParams = new J2534InitParameters();
                if (radioVPW.Checked)
                {
                    if (datalogger.AnalyzerRunning)
                    {
                        datalogger.LogDevice.SetAnalyzerFilter();
                    }
                    else
                    {
                        datalogger.LogDevice.SetLoggingFilter();
                    }
                }
                else
                {
                    if (radioHSCAN.Checked)
                    {
                        jParams.Protocol = ProtocolID.CAN;
                        jParams.Baudrate = BaudRate.ISO15765_500000;
                    }
                    else
                    {
                        //datalogger.LogDevice.SetJ2534Configs( "J1962_PINS=$00000100",false);
                        jParams.Protocol = ProtocolID.SW_CAN_PS;
                        jParams.Baudrate = BaudRate.ISO15765_33K3;
                    }
                    jParams.Secondary = true;
                    jParams.SeparateProtoByChannel = true;
                    jParams.UsePrimaryChannel = true;
                    jParams.PassFilters = "Type:PASS_FILTER,Name:CANFlasherPass" + Environment.NewLine;
                    jParams.PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "Pattern:0000" + datalogger.CanDevice.DiagID.ToString("X4") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    datalogger.LogDevice.ConnectSecondaryProtocol(jParams);
                    if (radioLSCan.Checked && chkWakeUp.Checked)
                    {
                        byte[] wakeMsg = new byte[] { 00, 00, 01, 00 };
                        OBDMessage wMsg = new OBDMessage(wakeMsg);
                        wMsg.Txflag = TxFlag.SW_CAN_HV_TX;
                        wMsg.SecondaryProtocol = true;
                        datalogger.LogDevice.SendMessage(wMsg, 0);
                        Thread.Sleep(500);
                    }
                    if (chkStartPeriodic.Checked)
                    {
                        string pMsg = "00 00 01 01 FE 3E:2500:ISO15765_FRAME_PAD|ISO15765_ADDR_TYPE";
                        datalogger.LogDevice.StartPeriodicMsg(pMsg, false);
                    }
                }
                datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port, false, datalogger.AnalyzerRunning);
                Logger("Connected");
                Application.DoEvents();
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
                LoggerBold("Connection failed. Check settings");
                return false;
            }
        }

        private J2534InitParameters CreateJ2534InitParameters()
        {
            J2534InitParameters initParameters = new J2534InitParameters(false);
            if (comboJ2534Init.Text.Length > 0)
            {
                initParameters.Kinit = (KInit)Enum.Parse(typeof(KInit), comboJ2534Init.Text);
                if (txtJ2534InitBytes.Text.Length > 1)
                {
                    initParameters.InitBytes = txtJ2534InitBytes.Text;
                }
            }
            if (txtJ2534SetPins.Text.Length > 0)
            {
                initParameters.Sconfigs = "J1962_PINS = " + txtJ2534SetPins.Text;
            }
            if (txtJConsoleConfigs.Text.Length > 0)
            {
                if (!string.IsNullOrEmpty(initParameters.Sconfigs))
                {
                    initParameters.Sconfigs += " | ";
                }
                initParameters.Sconfigs += txtJConsoleConfigs.Text;
            }
            initParameters.Protocol = (ProtocolID)Enum.Parse(typeof(ProtocolID), comboJ2534Protocol.Text);
            initParameters.Baudrate = (BaudRate)Enum.Parse(typeof(BaudRate), comboJ2534Baudrate.Text);
            if (comboJ2534Connectflag.Text.Length > 0)
            {
                initParameters.Connectflag = (ConnectFlag)Enum.Parse(typeof(ConnectFlag), comboJ2534Connectflag.Text);
            }
            initParameters.PerodicMsg = txtJ2534PeriodicMsg.Text;
            initParameters.PeriodicInterval = (int)numJ2534PeriodicMsgInterval.Value;
            initParameters.PassFilters = jConsoleFilters;
            return initParameters;
        }

        private J2534InitParameters CreateJ2534InitParameters2()
        {
            J2534InitParameters initParameters = new J2534InitParameters(false);
            if (comboJ2534Init2.Text.Length > 0)
            {
                initParameters.Kinit = (KInit)Enum.Parse(typeof(KInit), comboJ2534Init2.Text);
                if (txtJ2534InitBytes2.Text.Length > 1)
                {
                    initParameters.InitBytes = txtJ2534InitBytes2.Text;
                }
            }
            if (txtJ2534SetPins2.Text.Length > 0)
            {
                initParameters.Sconfigs = "J1962_PINS = " + txtJ2534SetPins2.Text;
            }
            if (txtJConsoleConfigs2.Text.Length > 0)
            {
                if (!string.IsNullOrEmpty(initParameters.Sconfigs))
                {
                    initParameters.Sconfigs += " | ";
                }
                initParameters.Sconfigs += txtJConsoleConfigs2.Text;
            }
            initParameters.Protocol = (ProtocolID)Enum.Parse(typeof(ProtocolID), comboJ2534Protocol2.Text);
            initParameters.Baudrate = (BaudRate)Enum.Parse(typeof(BaudRate),comboJ2534Baudrate2.Text);
            if (comboJ2534Connectflag2.Text.Length > 0)
            {
                initParameters.Connectflag = (ConnectFlag)Enum.Parse(typeof(ConnectFlag), comboJ2534Connectflag2.Text);
            }
            initParameters.PerodicMsg = txtJ2534PeriodicMsg2.Text;
            initParameters.PeriodicInterval = (int)numJ2534PeriodicMsgInterval2.Value;
            initParameters.PassFilters = jConsoleFilters2;
            initParameters.Secondary = true;
            initParameters.UsePrimaryChannel = chkUsePrimaryChannel.Checked;
            return initParameters;
        }

        private void LoadJ2534InitParameters(J2534InitParameters initParameters)
        {
            if (initParameters == null)
            {
                initParameters = new J2534InitParameters();
            }
            comboJ2534Protocol.Text = initParameters.Protocol.ToString();
            Application.DoEvents();
            txtJ2534SetPins.Text = "";
            comboJ2534Init.Text = initParameters.Kinit.ToString();
            txtJConsoleConfigs.Text = initParameters.Sconfigs;
            comboJ2534Baudrate.Text = initParameters.Baudrate.ToString();
            comboJ2534Connectflag.Text = initParameters.Connectflag.ToString();
            txtJ2534InitBytes.Text = initParameters.InitBytes;
            txtJ2534PeriodicMsg.Text = initParameters.PerodicMsg;
            numJ2534PeriodicMsgInterval.Value = initParameters.PeriodicInterval;
            jConsoleFilters = initParameters.PassFilters;
        }

        private void LoadJ2534InitParameters2(J2534InitParameters initParameters)
        {
            if (initParameters == null)
            {
                initParameters = new J2534InitParameters();
            }
            txtJ2534SetPins2.Text = "";
            comboJ2534Protocol2.Text = initParameters.Protocol.ToString();
            Application.DoEvents();
            comboJ2534Init2.Text = initParameters.Kinit.ToString();
            txtJConsoleConfigs2.Text = initParameters.Sconfigs;
            comboJ2534Baudrate2.Text = initParameters.Baudrate.ToString();
            comboJ2534Connectflag2.Text = initParameters.Connectflag.ToString();
            txtJ2534InitBytes2.Text = initParameters.InitBytes;
            txtJ2534PeriodicMsg2.Text = initParameters.PerodicMsg;
            numJ2534PeriodicMsgInterval2.Value = initParameters.PeriodicInterval;
            jConsoleFilters2 = initParameters.PassFilters;
            chkUsePrimaryChannel.Checked = initParameters.UsePrimaryChannel;
        }
        public bool ConnectJConsole(OBDScript oscript, J2534InitParameters InitParms)
        {
            try
            {
                if (jConsole != null && jConsole.Connected)
                {
                    return true;
                }
                if (datalogger != null && datalogger.Connected && j2534RadioButton.Checked && j2534DeviceList.Text == comboJ2534DLL.Text)
                {
                    LoggerBold("Device in use");
                    return false;
                }
                jConsole = new JConsole();
                jConsole.Receiver = new MessageReceiver();
                Application.DoEvents();
                J2534InitParameters initParameters = new J2534InitParameters(false);
                if (InitParms == null)
                {
                    initParameters = CreateJ2534InitParameters();
                }
                else
                {
                    initParameters = InitParms;
                }
                LoadJ2534InitParameters(initParameters);
                J2534DotNet.J2534Device dev = jDevList[comboJ2534DLL.SelectedIndex];
                if (AppSettings.LoggerStartJ2534Process)
                    jConsole.JDevice = new J2534Client(comboJ2534DLL.SelectedIndex);
                else
                    jConsole.JDevice = new J2534Device(dev);
                //jConsole.JDevice.SetProtocol(protocol, baudrate, flag);
                jConsole.JDevice.MsgSent += JDevice_MsgSent;
                jConsole.JDevice.MsgReceived += JDevice_MsgReceived;
                if (!jConsole.JDevice.Initialize(0, initParameters))
                {
                    jConsole.JDevice.Dispose();
                    jConsole.JDevice = null;
                    return false;
                }
                jConsole.JDevice.Enable4xReadWrite = chkJConsole4x.Checked;
                jConsole.JDevice.SetReadTimeout(AppSettings.TimeoutJConsoleReceive);
                labelJconsoleConnected.Text = "Connected";
                jConsole.Connected = true;
                SaveSettings();
                jConsole.Receiver.StartReceiveLoop(jConsole.JDevice, null, false, false);
                if (oscript == null)
                {
                    joscript = new OBDScript(jConsole.JDevice, jConsole);
                }
                else
                {
                    joscript = oscript;
                    joscript.device = jConsole.JDevice;
                }
                Application.DoEvents();
                //groupJ2534Options.Enabled = false;
                btnJConsoleConnectSecondProtocol.Enabled = true;
                timerDeviceStatus.Enabled = true;
                timerJconsoleShowLogText.Interval = AppSettings.LoggerConsoleDisplayInterval;
                if (chkUsePrimaryChannel.Checked)
                {
                    J2534InitParameters JSettings2 = CreateJ2534InitParameters2();
                    ConnectJConsole2(JSettings2);
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                LoggerBold("Connection failed. Check Jconsole settings");
                return false;
            }
        }

        public bool ReConnectJConsole()
        {
            try
            {
                J2534InitParameters initParameters = CreateJ2534InitParameters();
                LoadJ2534InitParameters(initParameters);
                
                if (!jConsole.JDevice.PassthruConnect(initParameters))
                {
                    jConsole.JDevice.Dispose();
                    jConsole.JDevice = null;
                    return false;
                }
                jConsole.JDevice.Enable4xReadWrite = chkJConsole4x.Checked;
                labelJconsoleConnected.Text = "Connected";
                jConsole.Connected = true;
                Application.DoEvents();
                //groupJ2534Options.Enabled = false;
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                LoggerBold("Connection failed. Check Jconsole settings");
                return false;
            }
        }

        private bool StartLogging(bool TestPids)
        {
            try
            {
                Logger("Start logging...");
                DisconnectTime = DateTime.MaxValue;
                AppSettings.LoggerTimestampFormat = txtTstampFormat.Text;
                AppSettings.LoggerDecimalSeparator = txtDecimalSeparator.Text;
                AppSettings.LoggerLogSeparator = txtLogSeparator.Text;
                AppSettings.Save();
                labelProgress.Text = "";
                labelTimeStamp.Text = "-";
                chkContinuousPidTest.Enabled = false;
                chkContinuousPidTest.Checked = false;
                if (!radioVPW.Checked && serialRadioButton.Checked && !comboSerialDeviceType.Text.StartsWith("UPX"))
                {
                    Logger("Using SendOnce mode, other modes not supported with Serial device & CAN");
                    datalogger.Responsetype = (byte)ResponseTypes.SendOnce;
                }
                else 
                {
                    datalogger.Responsetype = Convert.ToByte(Enum.Parse(typeof(ResponseTypes), comboResponseMode.Text));
                }
                datalogger.writelog = chkWriteLog.Checked;
                datalogger.reverseSlotNumbers = chkReverseSlotNumbers.Checked;
                RequestRestartLogging = false;
                //btnTestEnabledPids.Enabled = false;
                //btnQueryPid.Enabled = false;
                //btnTestMasterlistPids.Enabled = false;
                if (datalogger.SelectedPids.Count == 0 )
                {
                    Logger("No profile configured");
                    return false;
                }
                if (!Connect(radioVPW.Checked, false,true, null))
                {
                    return false;
                }
                if (!radioVPW.Checked)
                {
                    datalogger.slothandler.ResetFilters();
                }
                for (int x = 0; x < 10; x++)
                {
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
                foreach(LogParam.PidSettings pidSettings in datalogger.SelectedPids.Where(X=>X.Parameter.Type == LogParam.DefineBy.Address))
                {
                    datalogger.SetPidAddressByOS(pidSettings, true);
/*                    if (pidSettings.Os == null)
                    {
                        datalogger.SelectedPids.Remove(pidSettings);
                    }
*/                }
                if (TestPids)
                {
                    string bakProfiles = Path.Combine(Application.StartupPath, "Logger", "Profiles", "Bak");
                    if (!Directory.Exists(bakProfiles))
                    {
                        Directory.CreateDirectory(bakProfiles);
                    }
                    string bakFile = Path.Combine(bakProfiles, DateTime.Now.ToString("yy-MM-dd-HH-mm") + ".XML");
                    Logger("Saving profile backup:");
                    LogParamFile.SaveProfile(bakFile, datalogger.SelectedPids);
                    datalogger.RemoveUnsupportedPids();
                    if (datalogger.SelectedPids.Count == 0)
                    {
                        LoggerBold("All pids removed, restoring backup. Logging stopped");
                        datalogger.SelectedPids = LogParamFile.LoadProfile(bakFile);
                        return false;
                    }
                }
                if (GraphicsForm != null && GraphicsForm.Visible)
                {
                    GraphicsForm.StartLiveUpdate();
                }
                SetupLogDataGrid();
                if (chkWriteLog.Checked)
                {
                    datalogger.LogStartTime = DateTime.Now;
                    logfilename = Path.Combine(txtLogFolder.Text, "log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm.ss") + ".csv");
                    AppSettings.LoggerLastLogfile = logfilename;
                    if (!datalogger.CreateLog(logfilename))
                    {
                        return false;
                    }
                }
                if (datalogger.StartLogging(radioVPW.Checked))
                {
                    timerShowData.Enabled = true;
                    btnStartStop.Text = "Stop logging (" + comboStartLoggingKey.Text + ")";
                    chkLoggingStarted.Checked = true;
                    groupLogSettings.Enabled = false;
                    //btnGetVINCode.Enabled = false;
                    datalogger.LogRunning = true;
                    dataGridSelectedPids.Columns["Value"].Visible = true;
                    if (datalogger.LogDevice.LogDeviceType == LoggingDevType.Elm)
                    {
                        //groupDTC.Enabled = false;
                    }
                    AppSettings.Save();
                }
                else
                {
                    datalogger.LogRunning = false;
                    groupLogSettings.Enabled = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                return false;
            }
        }

        private void StartStopLogging()
        {
            btnStartStop.Enabled = false;
            if (chkLoggingStarted.Checked)
            {
                StopLogging(false, true);
            }
            else
            {
                if (!StartLogging(chkTestPidCompatibility.Checked))
                {
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port, false, datalogger.AnalyzerRunning);
                }
            }
            btnStartStop.Enabled = true;

        }
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            StartStopLogging();
        }

        private void comboSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboSerialPort.Text))
            {
                btnStartStop.Enabled = true;
            }
        }


        private void btnBrowsLogFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string defFldr = Path.Combine(Application.StartupPath, "Logger", "Log");
                string path = SelectFolder("Select log folder", defFldr);
                if (path.Length == 0)
                    return;
                txtLogFolder.Text = path;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void serialRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            queryCANDevicesMainConnectionToolStripMenuItem.Enabled = !serialRadioButton.Checked;
            if (serialRadioButton.Checked)
            {
                serialOptionsGroupBox.Enabled = true;
                j2534OptionsGroupBox.Enabled = false;
                if (comboSerialDeviceType.Text.Contains("CAN"))
                {
                    groupProtocol.Enabled = true;                    
                }
                else
                {
                    groupProtocol.Enabled = false;
                    radioVPW.Checked = true;
                }
            }
        }

        private void j2534RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.j2534RadioButton.Checked)
            {
                serialOptionsGroupBox.Enabled = false;
                j2534OptionsGroupBox.Enabled = true;
                groupProtocol.Enabled = true;
                //groupJ2534Options.Visible = true;
            }

        }


        private void comboSerialDeviceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (j2534RadioButton.Checked)
            {
                groupProtocol.Enabled = true;
                groupCanParams.Enabled = true;
                return;
            }
            if (comboSerialDeviceType.Text== ScanToolDeviceImplementation.DeviceType 
                || comboSerialDeviceType.Text == LegacyElmDeviceImplementation.DeviceType )
            {
                //comboResponseMode.Text = "SendOnce";
                //comboResponseMode.Enabled = false;
                groupProtocol.Enabled = false;
                groupCanParams.Enabled = false;
                radioVPW.Checked = true;
            }
            else if (comboSerialDeviceType.Text == AvtDevice.DeviceType)
            {
                groupProtocol.Enabled = false;
                groupCanParams.Enabled = false;
                radioVPW.Checked = true;
                comboBaudRate.Text = "57600";
            }
            else if (comboSerialDeviceType.Text == UPX_OBD.DeviceType)
            {
                groupProtocol.Enabled = true;
                groupCanParams.Enabled = true;
            }
            else if (comboSerialDeviceType.Text == Elm327Device.DeviceType)
            {
                groupProtocol.Enabled = true;
                groupCanParams.Enabled = true;
            }
            else
            {
                groupProtocol.Enabled = false;
                groupCanParams.Enabled = false;
                radioVPW.Checked = true;
                //comboResponseMode.Enabled = true;
                //comboResponseMode.Text = "SendFast";
            }

        }

        private void chkFTDI_CheckedChanged(object sender, EventArgs e)
        {
            comboSerialPort.Text = "";
            LoadPorts(true);
        }


        private void j2534DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void txtLogSeparator_TextChanged(object sender, EventArgs e)
        {
/*            if (txtLogSeparator.Text.Length > 1)
            {
                txtLogSeparator.Text = txtLogSeparator.Text.Substring(0, 1);
            }
*/        
        }
        private void AddPassivePidtoAnalyzer(LogParam.PidSettings pidSettings, ref List<LogParam.PidParameter> definedParms)
        {
            if (!PassivePidCanIds.Contains(pidSettings.Parameter.Address))
            {
                PassivePidCanIds.Add(pidSettings.Parameter.Address);
            }
            if (!PassivePidOrigins.ContainsKey(pidSettings.Parameter.Address) && !string.IsNullOrEmpty(pidSettings.Parameter.PassivePid.Origin))
            {
                PassivePidOrigins.Add(pidSettings.Parameter.Address, pidSettings.Parameter.PassivePid.Origin);
            }
            if (!definedParms.Contains(pidSettings.Parameter))
            {
                definedParms.Add(pidSettings.Parameter);
                PassivePidSettings.Add(pidSettings);
                int r = dataGridPassivePidValues.Rows.Add();
                dataGridPassivePidValues.Rows[r].Tag = pidSettings;
                dataGridPassivePidValues.Rows[r].Cells["CanId"].Value = pidSettings.Parameter.Address.ToString("X8");
                dataGridPassivePidValues.Rows[r].Cells["Origin"].Value = pidSettings.Parameter.PassivePid.Origin;
                dataGridPassivePidValues.Rows[r].Cells["Signal"].Value = pidSettings.Parameter.Name;
                dataGridPassivePidValues.Rows[r].Cells["Units"].Value = pidSettings.Units.Units;
                dataGridPassivePidValues.Rows[r].Visible = false;
            }

        }
        private void CollectPassivePidIds()
        {
            dataGridPassivePidValues.Rows.Clear();
            dataGridPassivePidValues.Columns.Clear();
            dataGridPassivePidValues.Columns.Add("CanId", "CanID");
            dataGridPassivePidValues.Columns.Add("Origin", "Origin");
            dataGridPassivePidValues.Columns.Add("Signal", "Signal");
            dataGridPassivePidValues.Columns.Add("Value", "Value");
            dataGridPassivePidValues.Columns.Add("Units", "Units");

            PassivePidSettings = new List<LogParam.PidSettings>();
            PassivePidCanIds = new List<int>();
            PassivePidOrigins = new Dictionary<int, string>();
            List<LogParam.PidParameter> definedParms = new List<LogParam.PidParameter>();
            List<LogParam.PidSettings> pids = datalogger.SelectedPids.Where(X => X.Parameter.Type == LogParam.DefineBy.Passive).ToList();
            foreach (LogParam.PidSettings pidSettings in pids)
            {
                AddPassivePidtoAnalyzer(pidSettings, ref definedParms);
            }
            List<LogParam.PidParameter> parms = datalogger.PidParams.Where(X => X.Type == LogParam.DefineBy.Passive).ToList();
            foreach (LogParam.PidParameter parm in parms)
            {
                LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                pidSettings.Units = parm.Conversions[0];
                AddPassivePidtoAnalyzer(pidSettings, ref definedParms);
            }
            dataGridPassivePidValues.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        private void StartAnalyzer()
        {
            try
            {
                if (!Connect(radioVPW.Checked, false, true, null))
                {
                    return;
                }
                datalogger.LogDevice.SetAnalyzerFilter();
                dataGridAnalyzer.SelectionChanged -= DataGridAnalyzer_SelectionChanged; //Avoid duplicates and disable for original analyzer
                dataGridAnalyzer.Columns.Clear();
                splitAnalyzer.Panel2Collapsed = true;
                splitAnalyzer.Panel2.Hide();
                if (chkCombinePids.Checked)
                {
                    showSignalBitsToolStripMenuItem.Enabled = true;
                    CollectPassivePidIds();
                    if (PassivePidCanIds.Count > 0)
                    {
                        splitAnalyzer.Panel2Collapsed = false;
                        splitAnalyzer.Panel2.Show();
                        labelPassivePidValues.Visible = true;
                    }
                    dataGridAnalyzer.Columns.Add("CanID", "CAN id");
                    dataGridAnalyzer.Columns.Add("Origin", "Origin");
                    dataGridAnalyzer.Columns.Add("Data", "Data");
                    dataGridAnalyzer.Columns.Add("Hz", "Hz");
                    dataGridAnalyzer.Columns["CanID"].Width = 70;
                    dataGridAnalyzer.Columns["Origin"].Width = 100;
                    dataGridAnalyzer.Columns["Data"].Width = 200;
                    dataGridAnalyzer.Columns["Hz"].Width = 50;
                    combinedCanDatas = new List<CombinedCanData>();
                    //blPassiveCanDatas = new BindingList<PassiveCanData>(passiveCanDatas);
                    //dataGridAnalyzer.DataSource = null;
                    //dataGridAnalyzer.DataSource = blPassiveCanDatas;
                    //dataGridAnalyzer.DataBindingComplete += DataGridAnalyzer_DataBindingComplete;
                    //datalogger.LogDevice.RemoveFilters(null);
                    datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived1_PassivePids;
                    dataGridAnalyzer.SelectionChanged += DataGridAnalyzer_SelectionChanged;
                    Application.DoEvents();
                    timerAnalyzerCombined.Enabled = true;
                }
                else
                {
                    showSignalBitsToolStripMenuItem.Enabled = false;
                    Analyzer.AnalyzerData ad = new Analyzer.AnalyzerData();
                    foreach (var prop in ad.GetType().GetProperties())
                    {
                        dataGridAnalyzer.Columns.Add(prop.Name, prop.Name);
                    }
                    analyzer = new Analyzer();
                    analyzer.RowAnalyzed += Analyzer_RowAnalyzed;
                    string devtype = comboSerialDeviceType.Text;
                    if (j2534RadioButton.Checked)
                        devtype = "J2534 Device";
                    analyzer.StartAnalyzer(devtype, chkHideHeartBeat.Checked);
                    timerAnalyzer.Enabled = true;
                }
                datalogger.AnalyzerRunning = true;
                btnStartStopAnalyzer.Text = "Stop Analyzer";
                if (!datalogger.Receiver.ReceiveLoopRunning)
                {
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port, false, true);
                }
                //groupDTC.Enabled = false;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void DataGridAnalyzer_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridAnalyzer.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void LogDevice_MsgReceived1_PassivePids(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg.Data.Length < 5)
                {
                    Debug.WriteLine("Short message");
                    return;
                }
                int canid = ReadInt32(e.Msg.Data, 0, true);
                CombinedCanData canData = combinedCanDatas.Where(X => X.canid == canid).FirstOrDefault();
                if (canData == null)
                {
                    canData = new CombinedCanData(canid);
                    combinedCanDatas.Add(canData);
                }
                byte[] data = new byte[e.Msg.Length - 4];
                Array.Copy(e.Msg.Data, 4, data, 0, data.Length);
                canData.DataBytes = data;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }

        }

        private void Analyzer_RowAnalyzed(object sender, Analyzer.AnalyzerData e)
        {
            analyzedRows.Enqueue(e);
        }

        private void StopAnalyzer()
        {
            try
            {
                if (radioVPW.Checked)
                {
                    datalogger.LogDevice.SetLoggingFilter();
                }
                else
                {
                    datalogger.slothandler.ResetFilters();
                    if (datalogger.LogRunning)
                    {
                        datalogger.slothandler.SetupPassivePidFilters();
                    }
                }
                if (chkCombinePids.Checked)
                {
                    datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived1_PassivePids;
                    dataGridAnalyzer.DataBindingComplete -= DataGridAnalyzer_DataBindingComplete;
                    timerAnalyzerCombined.Enabled = false;
                }
                else
                {
                    analyzer.StopAnalyzer();
                    timerAnalyzer.Enabled = false;
                }
                btnStartStopAnalyzer.Text = "Start Analyzer";
                datalogger.AnalyzerRunning = false;
                groupDTC.Enabled = true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void FilterPidsByBin(PcmFile PCM)
        {
            try
            {
                datalogger.OS = PCM.OS;
                PidSearch pidSearch = new PidSearch(PCM);
                if (pidSearch.pidList != null && pidSearch.pidList.Count > 0)
                {
                    SupportedPids = new List<int>();
                    for (int s = 0; s < pidSearch.pidList.Count; s++)
                    {
                        SupportedPids.Add(pidSearch.pidList[s].pidNumberInt);
                    }
                    LoadOsPidFiles();
                    ReloadPidParams(false, true);
                    comboFilterByOS.Text = PCM.OS;
                    comboProfilePlatform.Text = PCM.configFile;
                }
                if (datalogger.Connected)
                {
                    labelConnected.Text = "Connected - OS: " + PCM.OS;
                }
                else
                {
                    labelConnected.Text = "Disconnected - OS: " + PCM.OS;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void filterSupportedPidsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select BIN file");
            if (fName.Length == 0)
                return;
            PCM = new PcmFile(fName, true, "");            
            FilterPidsByBin(PCM);
        }

        private void comboResponseMode_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void chkPriority_CheckedChanged(object sender, EventArgs e)
        {
/*            if (chkPriority.Checked)
            {
                comboResponseMode.Text = "SendOnce";
            }
*/
        }

        private void getDtcCodes(byte mode)
        {
            dataGridDtcCodes.Rows.Clear();
            dataGridDtcCodes.Columns["Conversion"].Visible = false;
            dataGridDtcCodes.Columns["Scaling"].Visible = false;
            if (!Connect(radioVPW.Checked, true, true, null))
            {
                return;
            }
            Thread.Sleep(100);
            //List<DTCCodeStatus> codelist = new List<DTCCodeStatus>();
            if (radioVPW.Checked)
            {
                if (chkDtcAllModules.Checked)
                {
                    DtcCurrentModule = DeviceId.Broadcast;
                    //codelist = datalogger.RequestDTCCodes(module, mode);
                }
                else
                {
                    DtcCurrentModule = Convert.ToByte(comboModule.SelectedValue);
                    //codelist = datalogger.RequestDTCCodes(module, mode); 
                }
            }
            else
            {
                DtcCurrentModule = Convert.ToUInt16(comboModule.SelectedValue);
                if (mode == 2)
                {
                    mode = 0xa9;
                }
            }
            if (datalogger.LogRunning)
            {
                datalogger.QueueDtcRequest(DtcCurrentModule, mode);
            }
            else
            {
                datalogger.RequestDTCCodes(DtcCurrentModule, mode, true);
            }
        }

        private void LogDevice_DTC_MsgReceived(object sender, MsgEventparameter e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    if (e.Msg == null)
                    {
                        return;
                    }
                    if (radioVPW.Checked)
                    {
                        if (e.Msg.Length > 6 && e.Msg[1] == DeviceId.Tool && e.Msg[3] == 0x59)
                        {
                            DTCCodeStatus dcs = datalogger.DecodeDTCstatus(e.Msg.GetBytes(),0);
                            if (!string.IsNullOrEmpty(dcs.Module))
                            {
                                int r = dataGridDtcCodes.Rows.Add();
                                dataGridDtcCodes.Rows[r].Cells[0].Value = dcs.Module;
                                dataGridDtcCodes.Rows[r].Cells[1].Value = dcs.Code;
                                dataGridDtcCodes.Rows[r].Cells[2].Value = dcs.Description;
                                dataGridDtcCodes.Rows[r].Cells[3].Value = dcs.Status;
                            }
                        }
                    }
                    else //CAN
                    {
                        //ushort module = Convert.ToUInt16(comboModule.SelectedValue);
                        if (datalogger.ValidateQueryResponse(e.Msg, DtcCurrentModule) && e.Msg[4] == 0x81)
                        {

                            DTCCodeStatus dcs = datalogger.DecodeDTCstatus(e.Msg.GetBytes(), DtcCurrentModule);
                            if (!string.IsNullOrEmpty(dcs.Module))
                            {
                                int r = dataGridDtcCodes.Rows.Add();
                                //dataGridDtcCodes.Rows[r].Cells[0].Value = dcs.Module;
                                dataGridDtcCodes.Rows[r].Cells[0].Value = DtcCurrentModule.ToString("X4");
                                dataGridDtcCodes.Rows[r].Cells[1].Value = dcs.Code;
                                dataGridDtcCodes.Rows[r].Cells[2].Value = dcs.Description;
                                dataGridDtcCodes.Rows[r].Cells[3].Value = dcs.Status;
                            }
                            /*                            
                                int r = dataGridDtcCodes.Rows.Add();

                                    string data = "";
                                for (int i = 5; i < e.Msg.Length; i++)
                                    data += e.Msg[i].ToString("X2") + " ";
                                dataGridDtcCodes.Rows[r].Cells[1].Value = data;
                            */
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LogDevice_DTC_MsgReceived line " + line + ": " + ex.Message);
            }
        }

        private void btnCurrentDTC_Click(object sender, EventArgs e)
        {
            if (radioVPW.Checked)
                getDtcCodes(2);//2 = Current data
            else
                getDtcCodes(0x1a);//0x1a = Current data
        }

        private void btnHistoryDTC_Click(object sender, EventArgs e)
        {
            getDtcCodes(0x10);
        }

        private void btnClearCodes_Click(object sender, EventArgs e)
        {
            if (!radioVPW.Checked)
            {
                LoggerBold("Not implemented for CAN");
                //return;
            }
            Connect(radioVPW.Checked, true, true, null);

            ushort module = Convert.ToUInt16(comboModule.SelectedValue);
            if (chkDtcAllModules.Checked)
            {
                module = DeviceId.Broadcast;
            }
            if (datalogger.LogRunning && datalogger.LogDevice.LogDeviceType == LoggingDevType.Elm && radioVPW.Checked)
            {
                OBDMessage msg = new OBDMessage(new byte[] { Priority.Physical0, (byte)module, DeviceId.Tool, 0x20, 0x00 });
                datalogger.QueueCustomCmd(msg, "Clear DTC codes");
            }
            else
            {
                datalogger.ClearTroubleCodes(module);
            }
        }

        private void btnQueryPid_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridSelectedPids.SelectedCells.Count == 0)
                {
                    return;
                }
                Connect(radioVPW.Checked, true,true, null);
                datalogger.Receiver.SetReceiverPaused(true);
                dataGridSelectedPids.Columns["Value"].Visible = true;
                for (int s = 0; s < dataGridSelectedPids.SelectedCells.Count; s++)
                {
                    int row = dataGridSelectedPids.SelectedCells[s].RowIndex;
                    LogParam.PidSettings pidSettings = (LogParam.PidSettings)dataGridSelectedPids.Rows[row].Tag;
                    if (dataGridSelectedPids.Rows[row].Cells["Units"].Value != null)
                    {
                        pidSettings.Units = pidSettings.Parameter.Conversions.Where(X => X.Units == dataGridSelectedPids.Rows[row].Cells["Units"].Value.ToString()).FirstOrDefault();
                    }
                    string pidVal = datalogger.QueryPid(pidSettings, true);
                    dataGridSelectedPids.Rows[row].Cells["Value"].Value = pidVal;
                    dataGridLogData.Rows[row].Cells["Value"].Value = pidVal;
                }
                datalogger.Receiver.SetReceiverPaused(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmlogger line " + line + ": " + ex.Message);
            }
        }

        public void Disconnect(bool StopScript)
        {
            try
            {
                Logger("Disconnecting...", false);
                btnConnect.Text = "Connect";
                timerDeviceStatus.Enabled = false;
                chkVpwToFile.Checked = false;
                if (StopScript)
                {
                    oscript.stopscript = true;
                }
                datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
                datalogger.LogDevice.MsgReceived -= LogDevice_DTC_MsgReceived;

                if (datalogger.Receiver.ReceiveLoopRunning)
                {
                    datalogger.Receiver.StopReceiveLoop();
                    datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
                    datalogger.LogDevice.MsgSent -= LogDevice_MsgSent;
                }
                if (datalogger.LogRunning)
                {
                    StopLogging(true, false);
                }
                if (datalogger.AnalyzerRunning)
                {
                    StopAnalyzer();
                }
                datalogger.LogDevice.Dispose();
                datalogger.LogDevice = null;
                datalogger.Connected = false;
                if (string.IsNullOrEmpty(datalogger.OS))
                    labelConnected.Text = "Disconnected";
                else
                    labelConnected.Text = "Disconnected - OS: " + datalogger.OS;
                groupHWSettings.Enabled = true;
                if (j2534RadioButton.Checked)
                {
                    groupProtocol.Enabled = true;
                    j2534OptionsGroupBox.Enabled = true;
                }
                if (comboSerialDeviceType.Text.StartsWith(UPX_OBD.DeviceType) || comboSerialDeviceType.Text == Elm327Device.DeviceType)
                {
                    groupProtocol.Enabled = true;
                }
                Logger(" [Done]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LogDevice_DTC_MsgReceived line " + line + ": " + ex.Message);
            }
        }

        public void DisconnectJConsole(bool StopScript)
        {
            timerDeviceStatus.Enabled = false;
            chkJConsoleToFile.Checked = false;
            if (StopScript)
            {
                joscript.stopscript = true;
            }
            jConsole.JDevice.MsgReceived -= LogDevice_MsgReceived;
            jConsole.JDevice.MsgReceived -= LogDevice_DTC_MsgReceived;
            DisconnectSecondaryProto();
            jConsole.Receiver.StopReceiveLoop();
            jConsole.JDevice.MsgReceived -= LogDevice_MsgReceived;
            jConsole.JDevice.MsgSent -= LogDevice_MsgSent;
            jConsole.JDevice.Dispose();
            jConsole.JDevice = null;
            jConsole.Connected = false;
            labelJconsoleConnected.Text = "Disconnected";
            Logger("Disconnected");
            groupJ2534Options.Enabled = true;
            groupJConsoleProto.Enabled = false;
            btnJConsoleConnectSecondProtocol.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            btnConnect.Enabled = false;
            if (datalogger.Connected)
            {
                Disconnect(true);
            }
            else
            {
                Connect(radioVPW.Checked,true,true, null);
            }
            btnConnect.Enabled = true;
            btnConnect.Enabled = true;
        }

        private void chkDtcAllModules_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDtcAllModules.Checked)
                comboModule.Enabled = false;
            else
                comboModule.Enabled = true;
        }

        private void connectDisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datalogger.Connected)
            {
                Disconnect(true);
            }
            else
            {
                Connect(radioVPW.Checked,true,true, null);
            }
        }

        private void btnGetVINCode_Click(object sender, EventArgs e)
        {
            Connect(radioVPW.Checked,true, true, null);
            if (datalogger.LogRunning)
            {
                datalogger.QueueVINRequest();
            }
            else
            {                
                datalogger.QueryVIN();
            }

        }

        private void btnClearAnalyzerGrid_Click(object sender, EventArgs e)
        {
            dataGridAnalyzer.Rows.Clear();
            analyzerData.Clear();
        }

        private void btnAnalyzerSaveCsv_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile(CsvFilter);
                if (FileName.Length == 0)
                    return;
                Logger("Writing to file: " + Path.GetFileName(FileName), false);
                using (StreamWriter writetext = new StreamWriter(FileName))
                {
                    string row = "";
                    for (int i = 0; i < dataGridAnalyzer.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        row += dataGridAnalyzer.Columns[i].HeaderText;
                    }
                    writetext.WriteLine(row);
                    for (int r = 0; r < (dataGridAnalyzer.Rows.Count - 1); r++)
                    {
                        row = "";
                        for (int i = 0; i < dataGridAnalyzer.Columns.Count; i++)
                        {
                            if (i > 0)
                                row += ";";
                            if (dataGridAnalyzer.Rows[r].Cells[i].Value != null)
                                row += dataGridAnalyzer.Rows[r].Cells[i].Value.ToString();
                        }
                        writetext.WriteLine(row);
                    }
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        private void chkBusFilters_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerUseFilters = chkVPWFilters.Checked;
            if (datalogger.Connected && radioVPW.Checked)
            {
                datalogger.LogDevice.SetLoggingFilter();
            }
        }


        private void saveProfileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectSaveProfile();
        }

        private void timerSearchParams_Tick(object sender, EventArgs e)
        {
            keyDelayCounter++;
            if (keyDelayCounter > AppSettings.keyPressWait100ms)
            {
                timerSearchParams.Enabled = false;
                ReloadPidParams(false,true);
            }
        }

        private void newProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            datalogger.SelectedPids.Clear();
            for (int row=0;row < dataGridProfile.Rows.Count; row++)
            {
                dataGridProfile.Rows[row].Cells["Show"].Value = false;
            }
            ReloadPidParams(false,true);
            profileFile = "";
            this.Text = "Logger";
        }


        private void homepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "https://universalpatcher.net/";
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCredits fc = new frmCredits();
            fc.Show();
        }

        private void btnSaveAnalyzerMsgs_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectSaveFile(AnalyzerFilter);
                if (fName.Length == 0)
                    return;
                for (int a = 0; a < analyzerData.Count; a++)
                {
                    OBDMessage msg = analyzerData[a];
                    Logger("Writing to file: " + Path.GetFileName(fName), false);
                    using (StreamWriter writetext = new StreamWriter(fName))
                    {
                        string line = "[" + new DateTime((long)msg.TimeStamp).ToString("yyyy-MM-dd-HH:mm:ss:FFFFF") + "] " + msg.ToString();
                        writetext.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnConsoleLoadScript_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select script file", ScriptFilter, AppSettings.LastScriptFile);
            if (fName.Length == 0)
                return;
            AppSettings.LastScriptFile = fName;
            if (!Connect(radioVPW.Checked,true, true, null))
            {
                return;
            }
            
            Logger("Sending file: " + fName);
            oscript = new OBDScript(datalogger.LogDevice, null);
            //datalogger.LogDevice.ClearMessageBuffer();
            datalogger.Receiver.SetReceiverPaused(true);
            Task.Factory.StartNew(() =>
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    btnStopScript.Enabled = true;
                    oscript.UploadScript(fName);
                    btnStopScript.Enabled = false;
                    Logger("Done");
                    datalogger.Receiver.SetReceiverPaused(false);
                });
            });
        }

        private void btnDtcCustom_Click(object sender, EventArgs e)
        {
            byte mode;
            DtcCurrentModule = Convert.ToUInt16(comboModule.SelectedValue); 
            if (txtDtcCustomModule.Text.Length > 0)
            {
                HexToUshort(txtDtcCustomModule.Text, out DtcCurrentModule);
            }
            if (!HexToByte(txtDtcCustomMode.Text,out mode))
            {
                LoggerBold("Unknown HEX number: " + txtDtcCustomMode.Text);
                return;
            }

            dataGridDtcCodes.Rows.Clear();
            Connect(radioVPW.Checked,true, true, null);
            if (datalogger.LogRunning)
            {
                datalogger.QueueDtcRequest(DtcCurrentModule, mode);
            }
            else
            {
                datalogger.RequestDTCCodes(DtcCurrentModule, mode, true);
            }
        }

        private void btnStartStopAnalyzer_Click(object sender, EventArgs e)
        {
            if (datalogger.AnalyzerRunning)
            {
                StopAnalyzer();
            }
            else 
            {
                StartAnalyzer();
            }
        }

        private void btnConnect2_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            btnConnect.Enabled = false;
            if (datalogger.Connected)
            {
                Disconnect(true);
            }
            else
            {
                Connect(radioVPW.Checked,true,true, null);
            }
            btnConnect.Enabled = true;
            btnConnect.Enabled = true;
        }

        private void txtSendBus_TextChanged(object sender, EventArgs e)
        {

        }

         private void chkConsole4x_CheckedChanged(object sender, EventArgs e)
        {
            if (datalogger != null && datalogger.Connected)
            {
                datalogger.LogDevice.Enable4xReadWrite = chkConsole4x.Checked;
            }
        }

        private void chkConsoleAutorefresh_CheckedChanged(object sender, EventArgs e)
        {
/*            if (chkConsoleAutorefresh.Checked)
            {
                if (!datalogger.Connected)
                {
                    return;
                }
                datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port);
            }
            else
            {
                datalogger.Receiver.StopReceiveLoop();
            }
*/
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void comboJ2534DLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadJ2534Protocols();
        }

        private void comboJ2534Protocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadJ2534Baudrates();
            if (comboJ2534Protocol.Text.Contains("VPW"))
                jConsoleVPW = true;
            else
                jConsoleVPW = false;
            if (comboJ2534Protocol.Text.Contains("CAN"))
                jConsoleCAN = true;
            else
                jConsoleCAN = false;
        }

        private void comboJ2534Init_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboJ2534Init.Text  == KInit.FastInit_J1979.ToString())
            {
                txtJ2534InitBytes.Text = "C1 33 F1 81";
            }
            else if (comboJ2534Init.Text == KInit.FastInit_GMDelco.ToString())
            {
                txtJ2534InitBytes.Text = "81 11 F1 81";
                txtJ2534PeriodicMsg.Text = "80 11 F1 01 3E";
                //0x80, 0x11, 0xF1, 0x01, 0x3E
            }
            else if (comboJ2534Init.Text == KInit.FastInit_ME7_5.ToString())
            {
                txtJ2534InitBytes.Text = "81 01 F1 81";
                txtJ2534PeriodicMsg.Text = "02 3E 01";
                //0x80, 0x11, 0xF1, 0x01, 0x3E
            }
            else
            {
                txtJ2534InitBytes.Text = "";
            }
        }

        private void btnJ2534SettingsSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile.xml");
                string FileName = SelectSaveFile(JProfileFilter, defName);
                if (FileName.Length == 0)
                    return;
                Logger("Saving file: " + FileName);
                J2534InitParameters JSettings = CreateJ2534InitParameters();
                LoggerUtils.SaveJ2534Settings(FileName, JSettings);
                AppSettings.LoggerJ2534SettingsFile = FileName;
                AppSettings.Save();
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void btnJ2534SettingsLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile.xml");
                string FileName = SelectFile("Select J2534 settings", JProfileFilter, defName);
                if (FileName.Length == 0)
                    return;
                //Logger("Loading file: " + FileName);
                J2534InitParameters JSettings = LoadJ2534Settings(FileName);
                LoadJ2534InitParameters(JSettings);
                AppSettings.LoggerJ2534SettingsFile = FileName;
                AppSettings.Save();
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void chkConsoleUseJ2534Timestamps_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnJConsoleConnect_Click(object sender, EventArgs e)
        {
            if (jConsole != null && jConsole.Connected)
            {
                DisconnectJConsole(true);
            }
            else
            {
                ConnectJConsole(null,null);
            }
        }

        private void txtJConsoleSend_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnJConsoleUploadScript_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectFile("Select script file", ScriptFilter, AppSettings.LastJScriptFile);
                if (fName.Length == 0)
                    return;
                AppSettings.LastJScriptFile = fName;
                J2534InitParameters initParameters = null;
                J2534InitParameters initParameters2 = null;
                StreamReader sr = new StreamReader(fName);
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    if (!Line.StartsWith("#"))
                    {
                        break;
                    }
                }
                sr.Close();
                if (Line != null && Line.StartsWith("connect"))
                {
                    string ProfileFileName = null;
                    string[] jParts = Line.Split(':');
                    if (jParts.Length > 1)
                    {
                        ProfileFileName = jParts[1];
                    }
                    if (ProfileFileName != null)
                    {
                        string profileFile = Path.Combine(Path.GetDirectoryName(fName), ProfileFileName);
                        if (!File.Exists(profileFile))
                        {
                            profileFile = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", ProfileFileName);
                        }
                        if (!File.Exists(profileFile))
                        {
                            LoggerBold("File not found: " + ProfileFileName);
                            return;
                        }
                        if (ProfileFileName.EndsWith("xmlx"))
                        {
                            List<J2534InitParameters> JSettings = LoadJ2534SettingsList(profileFile);
                            initParameters = JSettings[0];
                            initParameters2 = JSettings[1];
                        }
                        else
                        {
                            initParameters = LoadJ2534Settings(profileFile);
                        }
                    }
                }

                if (!ConnectJConsole(null, initParameters))
                {
                    return;
                }
                if (initParameters2 != null)
                {
                    ConnectJConsole2(initParameters2);
                }

                Logger("Sending file: " + fName);
                joscript = new OBDScript(jConsole.JDevice, jConsole);
                joscript.SecondaryProtocol = radioJConsoleProto2.Checked;
                //jConsole.JDevice.ClearMessageBuffer();
                if (radioJConsoleProto2.Checked)
                {
                    jConsole.Receiver2.SetReceiverPaused(true);
                }
                else
                {
                    jConsole.Receiver.SetReceiverPaused(true);
                }
                Task.Factory.StartNew(() =>
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        btnJconsoleStopScript.Enabled = true;
                        joscript.UploadScript(fName);
                        btnJconsoleStopScript.Enabled = false;
                        Logger("Done");
                        if (radioJConsoleProto2.Checked)
                        {
                            jConsole.Receiver2.SetReceiverPaused(false);
                        }
                        else
                        {
                            jConsole.Receiver.SetReceiverPaused(false);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void numJConsoleScriptDelay_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerScriptDelay = (int)numConsoleScriptDelay.Value;
        }

        private void numConsoleScriptDelay_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerScriptDelay = (int)numConsoleScriptDelay.Value;
        }

        private void btnStopJScript_Click(object sender, EventArgs e)
        {
            Logger("Stopping script");
            joscript.stopscript = true;
        }

        private void btnStopScript_Click_1(object sender, EventArgs e)
        {
            Logger("Stopping script");
            oscript.stopscript = true;

        }

        private void btnGenerateAlgo_Click(object sender, EventArgs e)
        {
            string fName = SelectFile();
            byte[] seedData = ReadBin(fName);
            int a = 0;
            StringBuilder sb = new StringBuilder();
            while (a < seedData.Length)
            {
                sb.Append("new byte[]{");
                for (int i = 0; i < 13 && a<seedData.Length; i++)
                {
                    sb.Append("0x" + seedData[a].ToString("X2") );
                    if (i<12)
                    {
                        sb.Append(",");
                    }
                    a++;
                }
                sb.Append("}," + Environment.NewLine);
            }
            fName = fName.Replace("bin", "txt");
            WriteTextFile(fName, sb.ToString());
        }

        private ushort EecvAlgoTest()
        {
            int seed;
            byte seedByte;
            int challengeCount = 0;

            if (!HexToInt(txtSeed.Text, out seed))
            {
                Logger("Unknown HEX value: " + txtSeed.Text);
                return 0;
            }
            if (!HexToByte(txtAlgo.Text, out seedByte))
            {
                Logger("Unknown HEX value: " + txtAlgo.Text);
                return 0;
            }

            byte seedByte1;
            byte seedByte2;
            byte seedByte3;

            seedByte1 = (byte)((seed >> 16) & 0xff);
            seedByte2 = (byte)((seed >> 8) & 0xff);
            seedByte3 = (byte)((seed) & 0xff);

            if (chkEecvSecondKey.Checked)
                challengeCount = 1;
            EECV eecv = new EECV();
            ushort result = eecv.CalculateKey(challengeCount, seedByte, seedByte1, seedByte2, seedByte3);

            return result;

        }
        private void btnAlgoTest_Click(object sender, EventArgs e)
        {
            try
            {
                int algo;
                ushort seed = 0;
                bool found = false;
                int algoFrom = 0;
                int algoTo = 0x300;

                if (radioEecv.Checked)
                {
                    txtAlgoTest.AppendText("Seed: " + txtSeed.Text + ", Seedbyte: " + txtAlgo.Text + ", Key: " + EecvAlgoTest().ToString("X4"));
                    return;
                }
                if (radioFindNgc4.Checked)
                {
                    NgcKeys nk = new NgcKeys();
                    int ngcSeed;
                    int key;
                    if (HexToInt(txtSeed.Text, out ngcSeed))
                    {
                        key = nk.unlockengine(ngcSeed);
                        txtAlgoTest.AppendText("Seed: " + ngcSeed.ToString("X8") + ", Key: " + key.ToString("X8") + Environment.NewLine);
                    }
                    return;
                }
                if (radioFindSbec.Checked)
                {
                    SBEC sb = new SBEC();
                    uint sbSeed;
                    uint key;
                    if (HexToUint(txtSeed.Text, out sbSeed))
                    {
                        key = sb.calculateKey(sbSeed);
                        txtAlgoTest.AppendText("Seed: " + sbSeed.ToString("X8") + ", Key (calculateKey): " + key.ToString("X8") + Environment.NewLine);
                        key = sb.unlock(sbSeed);
                        txtAlgoTest.AppendText("Seed: " + sbSeed.ToString("X8") + ", Key (unlock): " + key.ToString("X8") + Environment.NewLine);
                    }
                    return;
                }
                if (txtAlgoRange.Text.Contains("-"))
                {
                    string[] parts = txtAlgoRange.Text.Split('-');
                    HexToInt(parts[0].Trim(), out algoFrom);
                    HexToInt(parts[1].Trim(), out algoTo);
                }

                if (!HexToUshort(txtSeed.Text, out seed))
                {
                    Logger("Hex to int? " + txtSeed.Text);
                    return;
                }
                txtAlgoTest.AppendText("Calculating..." + Environment.NewLine);
                if (radioFindKey.Checked)
                {
                    if (!HexToInt(txtAlgo.Text, out algo))
                    {
                        Logger("Hex to int? " + txtAlgo.Text);
                        return;
                    }
                    ushort key = KeyAlgorithm.GetKey(algo, seed);
                    txtAlgoTest.AppendText("Algo: " + algo.ToString("X4") + ", seed: " + seed.ToString("X4") + ", key: " + key.ToString("X4") + Environment.NewLine);
                    found = true;
                }
                else if (radioFindAllKeys.Checked)
                {
                    for (algo = algoFrom; algo < algoTo; algo++)
                    {
                        ushort key = KeyAlgorithm.GetKey(algo, seed);
                        txtAlgoTest.AppendText("Algo: " + algo.ToString("X4") + ", seed: " + seed.ToString("X4") + ", key: " + key.ToString("X4") + Environment.NewLine);
                    }
                    found = true;
                }
                else
                {
                    ushort targetKey;
                    if (!HexToUshort(txtAlgo.Text, out targetKey))
                    {
                        Logger("Hex to ushort? " + txtAlgo.Text);
                        return;
                    }
                    for (algo = algoFrom; algo < algoTo; algo++)
                    {
                        ushort key = KeyAlgorithm.GetKey(algo, seed);
                        if (key == targetKey)
                        {
                            found = true;
                            txtAlgoTest.AppendText("seed: " + seed.ToString("X4") + ", key: " + key.ToString("X4") + ", algo: " + algo.ToString("X4") + Environment.NewLine);
                        }
                    }
                }
                if (found)
                {
                    txtAlgoTest.AppendText("[OK]" + Environment.NewLine);
                }
                else
                {
                    txtAlgoTest.AppendText("Not found" + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void radioFindKey_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFindKey.Checked)
            {
                labelAlgo.Text = "Algo:";
                labelSeed.Text = "Seed:";
                txtAlgo.Enabled = true;
                txtAlgoRange.Enabled = false;
            }
        }

        private void radioFindAllKeys_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFindAllKeys.Checked)
            {
                labelAlgo.Text = "Algo:";
                labelSeed.Text = "Seed:";
                txtAlgo.Enabled = false;
                txtAlgoRange.Enabled = true;
            }
        }

        private void radioFindAlgo_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFindAlgo.Checked)
            {
                labelAlgo.Text = "Key:";
                labelSeed.Text = "Seed:";
                txtAlgo.Enabled = true;
                txtAlgoRange.Enabled = true;
            }
        }

        private void chkJConsole4x_CheckedChanged(object sender, EventArgs e)
        {
            if (jConsole != null && jConsole.Connected)
            {
                jConsole.JDevice.Enable4xReadWrite = chkJConsole4x.Checked;
            }
        }

        private void parseLogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtFilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter ltb = new LogToBinConverter(0);
            Logger("Reading file: " + fName);
            Application.DoEvents();
            ltb.ConvertFile(fName);
        }

        public void ConnectJConsole2(J2534InitParameters JSettings)
        {
            try
            {
                if (jConsole.Connected2)
                {
                    //return;
                }
                if (jConsole.JDevice.ConnectSecondaryProtocol(JSettings))
                {
                    groupJConsoleProto.Enabled = true;
                    if (!jConsole.Connected2)
                    {
                        jConsole.Receiver2 = new MessageReceiver();
                        jConsole.Receiver2.StartReceiveLoop(jConsole.JDevice, jConsole.port, true, false);
                    }
                    jConsole.Connected2 = true;
                    //btnJConsoleConnectSecondProtocol.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LoggerBold("Protocol 2 connection failed. Check settings");
                Debug.WriteLine(ex.Message);
            }
        }
        private void btnJConsoleConnectSecondProtocol_Click(object sender, EventArgs e)
        {
            J2534InitParameters JSettings2 = CreateJ2534InitParameters2();
            ConnectJConsole2(JSettings2);
        }

        private void btnJConsoleAddConfig_Click(object sender, EventArgs e)
        {
            if (txtJConsoleConfigs.Text.Length> 0)
            {
                txtJConsoleConfigs.AppendText(" | ");
            }
            txtJConsoleConfigs.AppendText(comboJConsoleConfig.Text + " = 1");
        }



        private void parseBinfileToScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBinToScript fbts = new frmBinToScript();
            fbts.Show();
        }

        private void btnJ2534SettingsSaveAs2_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile2.xml");
                string FileName = SelectSaveFile(JProfileFilter, defName);
                if (FileName.Length == 0)
                    return;
                Logger("Saving file: " + FileName);
                J2534InitParameters JSettings = CreateJ2534InitParameters2();
                LoggerUtils.SaveJ2534Settings(FileName, JSettings);
                AppSettings.LoggerJ2534SettingsFile2 = FileName;
                AppSettings.Save();
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void btnJ2534SettingsLoad2_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile2.xml");
                string FileName = SelectFile("Select J2534 settings", JProfileFilter, defName);
                if (FileName.Length == 0)
                    return;
                Logger("Loading file: " + FileName);
                J2534InitParameters JSettings = LoadJ2534Settings(FileName);
                LoadJ2534InitParameters2(JSettings);
                AppSettings.LoggerJ2534SettingsFile2 = FileName;
                AppSettings.Save();
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void comboJ2534Protocol2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadJ2534Baudrates2();
        }

        private void comboJ2534Init2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboJ2534Init2.Text == KInit.FastInit_J1979.ToString())
            {
                txtJ2534InitBytes2.Text = "C1 33 F1 81";
            }
            else if (comboJ2534Init2.Text == KInit.FastInit_GMDelco.ToString())
            {
                txtJ2534InitBytes2.Text = "81 11 F1 81";
                txtJ2534PeriodicMsg2.Text = "80 11 F1 01 3E";
            }
            else if (comboJ2534Init2.Text == KInit.FastInit_ME7_5.ToString())
            {
                txtJ2534InitBytes2.Text = "81 01 F1 81";
                txtJ2534PeriodicMsg2.Text = "02 3E 01";
            }
            else
            {
                txtJ2534InitBytes2.Text = "";
            }

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void btnJConsoleReset_Click(object sender, EventArgs e)
        {
            LoadJ2534InitParameters(null);
        }

        private void btnJConsoleReset2_Click(object sender, EventArgs e)
        {
            LoadJ2534InitParameters2(null);
        }

        private void btnJConsoleAddConfig2_Click(object sender, EventArgs e)
        {
            if (txtJConsoleConfigs2.Text.Length > 0)
            {
                txtJConsoleConfigs2.AppendText(" | ");
            }
            txtJConsoleConfigs2.AppendText(comboJConsoleConfig2.Text + " = 1");

        }
        private void DisconnectSecondaryProto()
        {
            if (jConsole.Connected2)
            {
                Logger("Disconnecting secondary protocol");
                jConsole.Receiver2.StopReceiveLoop();
                jConsole.JDevice.DisConnectSecondaryProtocol();
                jConsole.Connected2 = false;
                btnJConsoleConnectSecondProtocol.Enabled = true;
                radioJConsoleProto1.Checked = true;
                groupJConsoleProto.Enabled = false;
                Logger("Done");
            }
        }
        private void btnJconsoleSecProtoDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectSecondaryProto();
        }

        private void chkJConsoleToFile_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkJConsoleToFile.Checked)
                {
                    if (jConsoleStream != null)
                    {
                        jConsoleStream.WriteLine("}");
                        jConsoleStream.Close();
                        jConsoleStream = null;
                    }
                    return;
                }
                string fName = SelectSaveFile(RtfFilter);
                if (fName.Length == 0)
                {
                    chkJConsoleToFile.Checked = false;
                    return;
                }
                //jConsoleLogFile = fName;
                jConsoleStream = new StreamWriter(fName);
                jConsoleStream.WriteLine("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1035{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New;}}");
                jConsoleStream.WriteLine("{\\colortbl ;\\red127\\green255\\blue212;\\red0\\green100\\blue0;\\red255\\green0\\blue0;\\red128\\green0\\blue128;}");
                //jConsoleStream.WriteLine("viewkind4\\uc1\\pard\\f0\\fs17");
                Logger("Writing to file: " + fName);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnJconsoleConfigFilters_Click(object sender, EventArgs e)
        {
            if (jConsole == null || jConsole.JDevice == null)
            {
                return;
            }
            jConsole.JDevice.SetupFilters(jConsoleFilters, false, true);
        }

        private void btnJconsoleConfigFilters2_Click(object sender, EventArgs e)
        {
            if (jConsole == null || jConsole.JDevice == null)
            {
                return;
            }
            jConsole.JDevice.SetupFilters(jConsoleFilters2, true, true);
        }

        private void btnDateTimeHelp_Click(object sender, EventArgs e)
        {            
            string url = "https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings";
            System.Diagnostics.Process.Start(url);
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void parseCANLogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtFilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.VPW);
            cltb.ConvertFile(fName);

        }

        private void queryCANDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //CANqueryCounter = 0;
                bool restoreConnection = false;
                if (jConsole != null && jConsole.Connected)
                {
                    restoreConnection = true;
                    //DisconnectJConsole(true);
                }
                else if (string.IsNullOrEmpty(comboJ2534Protocol.Text))
                {
                    comboJ2534Protocol.Text = ProtocolID.ISO15765.ToString();
                    comboJ2534Baudrate.Text = BaudRate.ISO15765_500000.ToString();
                    Application.DoEvents();
                }
                if (!ConnectJConsole(null,null))
                {
                    return;
                }
                canQuietResponses = 0;
                canDeviceResponses = -1;
                lastResponseTime = DateTime.Now;
                //timerKeepBusQuiet.Enabled = true;
                //timerWaitCANQuery.Enabled = true;
                //canDevs = new List<CANDevice>();
                dataGridCANDevices.DataSource = null;
                canDevs = SearchAllDevicesOnBus(jConsole.JDevice);
                dataGridCANDevices.DataSource = canDevs;
                dataGridCANDevices.Columns[0].DefaultCellStyle.Format = "X4";
                dataGridCANDevices.Columns[1].DefaultCellStyle.Format = "X4";
                dataGridCANDevices.Columns[2].DefaultCellStyle.Format = "X4";
                dataGridCANDevices.Columns[3].DefaultCellStyle.Format = "X2";
                if (restoreConnection)
                {
                    Logger("Restoring original connection");
                    jConsole.JDevice.PassthruDisconnect();
                    ReConnectJConsole();
                }
                else
                {
                    DisconnectJConsole(true);
                }

                Logger("Done");
                //jConsole.SetCANBusQuiet();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void timerKeepBusQuiet_Tick(object sender, EventArgs e)
        {
            jConsole.KeepCANBusQuiet();
        }

        private void timerWaitCANQuery_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now.Subtract(lastResponseTime);
            if (elapsed.TotalMilliseconds > 200)
            {
                if (canDeviceResponses < 0)
                {
                    canDeviceResponses = 0;
                    if (jConsole != null && jConsole.Receiver != null)
                    {
                        jConsole.Receiver.SetReceiverPaused(true);
                    }
                    CANQuery.Query(jConsole.JDevice);
                    if (jConsole != null && jConsole.Receiver != null)
                    {
                        jConsole.Receiver.SetReceiverPaused(false);
                    }
                }
                else
                {
                    timerWaitCANQuery.Enabled = false;
                    timerKeepBusQuiet.Enabled = false;
                    dataGridCANDevices.DataSource = canDevs;
                    dataGridCANDevices.Columns[0].DefaultCellStyle.Format = "X4";
                    dataGridCANDevices.Columns[1].DefaultCellStyle.Format = "X4";
                    dataGridCANDevices.Columns[2].DefaultCellStyle.Format = "X4";
                    dataGridCANDevices.Columns[3].DefaultCellStyle.Format = "X2";
                    Logger("Done");
                }
            }
        }

        private void ShowGraphics(bool Live)
        {
            try
            {
                GraphicsForm = new frmLoggerGraphics(Live);
                GraphicsForm.Text = "Logger Graph";
                GraphicsForm.Show();
                if (Live)
                {
                    GraphicsForm.SetupLiveGraphics();
                    if (datalogger.LogRunning || timerPlayback.Enabled)
                    {
                        GraphicsForm.StartLiveUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void listProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void playbackLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //string fName = SelectFile("Select Log file", CsvFilter);
                //if (string.IsNullOrEmpty(fName))
                //return;
                frmImportLogFile fil = new frmImportLogFile();
                if (fil.ShowDialog() == DialogResult.OK)
                {
                    string fName = fil.txtFileName.Text;
                    AppSettings.LoggerTimestampFormat = txtTstampFormat.Text;
                    AppSettings.LoggerDecimalSeparator = txtDecimalSeparator.Text;
                    AppSettings.LoggerLogSeparator = txtLogSeparator.Text;
                    AppSettings.Save();
                    profileFile = "";
                    datalogger.LoadLogFile(fName);
                    SetupLogDataGrid();
                    hScrollPlayback.Maximum = datalogger.LogDataBuffer.Count;
                    UpdateProfile();
                }
                fil.Dispose();

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

        private void SetPlaybackSpeed()
        {
            if (datalogger.LogDataBuffer != null && datalogger.LogDataBuffer.Count > 2)
            {
                DateTime t1 = new DateTime((long)datalogger.LogDataBuffer[1].TimeStamp);
                DateTime t2 = new DateTime((long)datalogger.LogDataBuffer[2].TimeStamp);
                TimeSpan step = t2.Subtract(t1);
                int ival = (int)((decimal)step.TotalMilliseconds / numPlaybackSpeed.Value);
                if (ival == 0)
                {
                    ival = 1;
                }
                timerPlayback.Interval = ival;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (datalogger.LogDataBuffer == null || datalogger.LogDataBuffer.Count == 0)
            {
                Logger("Empty buffer");
                return;
            }
            hScrollPlayback.Maximum = datalogger.LogDataBuffer.Count;
            SetPlaybackSpeed();
            timerPlayback.Enabled = true;
            /*
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.SetupPlayBack();
                GraphicsForm.StartLiveUpdate(true);
            }
            */
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            timerPlayback.Enabled = false;
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.StopLiveUpdate();
            }
        }

        private void timerPlayback_Tick(object sender, EventArgs e)
        {
            try
            {
                hScrollPlayback.Value++;
                if (hScrollPlayback.Value == hScrollPlayback.Maximum)
                {
                    timerPlayback.Enabled = false;
                    if (GraphicsForm != null && GraphicsForm.Visible)
                    {
                        GraphicsForm.StopLiveUpdate();
                    }
                    return;
                }
                PlayBack();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void PlayBack()
        {
            try
            {
                if (hScrollPlayback.Value >= datalogger.LogDataBuffer.Count)
                {
                    return;
                }
                LogData ld = datalogger.LogDataBuffer[hScrollPlayback.Value];
                LoggerDataEvents.Add(ld);
                //LastCalcValues = datalogger.slothandler.CalculatePidValues(datalogger.slothandler.LastPidValues);
                for (int row = 0; row < ld.Values.Length && row < dataGridLogData.Rows.Count; row++)
                {
                    dataGridLogData.Rows[row].Cells["Value"].Value = ld.CalculatedValues[row].ToString();
                }
                DateTime dt = new DateTime((long)ld.TimeStamp);
                labelTimeStamp.Text = dt.ToString("HH.mm.ss.ffff");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void hScrollPlayback_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (GraphicsForm != null && GraphicsForm.Visible)
                {
                    GraphicsForm.UpdateLogGraphics();
                }
                PlayBack();
                DateTime dt = new DateTime((long)datalogger.LogDataBuffer[hScrollPlayback.Value].TimeStamp);
                ShowToolTip(dt.ToString("HH:mm:ss.ffff"));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void ShowToolTip(string message)
        {
            ScrollTip.Show(message, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 20, 2000);
            //ScrollTip.Show(message, this,this.Location.X, this.Location.Y, 1000);
        }

        private void numPlaybackSpeed_ValueChanged(object sender, EventArgs e)
        {
            SetPlaybackSpeed();
        }

        private void chkVpwToFile_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkVpwToFile.Checked)
                {
                    if (vpwConsoleStream != null)
                    {
                        vpwConsoleStream.WriteLine("}");
                        vpwConsoleStream.Close();
                        vpwConsoleStream = null;
                    }
                    return;
                }
                string fName = SelectSaveFile(RtfFilter);
                if (fName.Length == 0)
                {
                    chkVpwToFile.Checked = false;
                    return;
                }
                //jConsoleLogFile = fName;
                vpwConsoleStream = new StreamWriter(fName);
                vpwConsoleStream.WriteLine("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1035{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New;}}");
                vpwConsoleStream.WriteLine("{\\colortbl ;\\red127\\green255\\blue212;\\red0\\green100\\blue0;\\red255\\green0\\blue0;\\red128\\green0\\blue128;}");
                //jConsoleStream.WriteLine("viewkind4\\uc1\\pard\\f0\\fs17");
                Logger("Writing to file: " + fName);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private int QueryDevsOnBus()
        {
            if (!Connect(radioVPW.Checked, true, true, null))
            {
                return 0;
            }
            Thread.Sleep(100);
            dataGridDtcCodes.Rows.Clear();
            dataGridDtcCodes.Columns["Conversion"].Visible = false;
            dataGridDtcCodes.Columns["Scaling"].Visible = false;
            datalogger.Receiver.SetReceiverPaused(true);
            Response<List<byte>> modules = datalogger.QueryDevicesOnBus(true);
            datalogger.Receiver.SetReceiverPaused(false);
            if (modules.Status != ResponseStatus.Success)
            {
                return 0;
            }
            for (int m = 0; m < modules.Value.Count; m++)
            {
                byte b = modules.Value[m];
                int r = dataGridDtcCodes.Rows.Add();
                if (analyzer.PhysAddresses.ContainsKey(b))
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = analyzer.PhysAddresses[b];
                }
                else
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = b;
                }
            }
            return modules.Value.Count;
        }

        private void btnQueryDevices_Click(object sender, EventArgs e)
        {
            QueryDevsOnBus();
        }

        private void QueryCanModules()
        {
            Response<List<OBDMessage>> msgs = datalogger.QueryCANModules((ushort)comboModule.SelectedValue);
            datalogger.Receiver.SetReceiverPaused(false);
            if (msgs.Status != ResponseStatus.Success)
            {
                return;
            }
            for (int m = 0; m < msgs.Value.Count; m++)
            {
                int r = dataGridDtcCodes.Rows.Add();
                OBDMessage msg = msgs.Value[m];
                byte modId = msg[5];
                CANDevice dev = CanModules.Where(X => X.ModuleID == modId).FirstOrDefault();
                if (dev == null)
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = modId.ToString("X4");
                }
                else
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = dev.ModuleName + " [" + modId.ToString("X4") + "]";
                    //dataGridDtcCodes.Rows[r].Cells["Description"].Value = dev.ModuleDescription; //WRONG
                    dataGridDtcCodes.Rows[r].Cells["Code"].Value = modId.ToString("X2");
                }
                byte[] data = new byte[msg.Length - 6];
                Array.Copy(msg.GetBytes(), 6, data, 0, msg.Length - 6);
                dataGridDtcCodes.Rows[r].Cells[3].Value = BitConverter.ToString(data);
            }

        }

        private void btnQueryModules_Click(object sender, EventArgs e)
        {
            if (!Connect(radioVPW.Checked,true, true, null))
            {
                return;
            }
            //datalogger.QueryDevicesOnBus(false);
            Thread.Sleep(100);
            dataGridDtcCodes.Rows.Clear();
            dataGridDtcCodes.Columns["Conversion"].Visible = false;
            dataGridDtcCodes.Columns["Scaling"].Visible = false;
            datalogger.Receiver.SetReceiverPaused(true);
            if (!radioVPW.Checked)
            {
                QueryCanModules();
                return;
            }
            Response<List<byte>> modules = datalogger.QueryDevicesOnBus(true);
            Response<List<OBDMessage>> msgs = datalogger.QueryModules(modules.Value.Count);
            datalogger.Receiver.SetReceiverPaused(false);
            if (msgs.Status != ResponseStatus.Success)
            {
                return;
            }
            for (int m=0;m<msgs.Value.Count;m++)
            {
                int r = dataGridDtcCodes.Rows.Add();
                OBDMessage msg = msgs.Value[m];
                if (analyzer.PhysAddresses.ContainsKey(msg[2]))
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = analyzer.PhysAddresses[msg[2]];
                }
                else
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = msg[2];
                }
                byte[] data = new byte[msg.Length - 4];
                Array.Copy(msg.GetBytes(), 4, data, 0, msg.Length - 4);
                dataGridDtcCodes.Rows[r].Cells[3].Value = BitConverter.ToString(data);
            }
        }

        public PidConfig GetConversionFromStd(ushort pid)
        {
            PidConfig pc = new PidConfig();
            Parameter sp = stdParameters.Where(x => x.Id == pid.ToString("X")).FirstOrDefault();
            if (sp == null)
                return null;
            pc.Type = DefineBy.Pid;
            pc.addr = pid;
            PidDataType pd = (PidDataType)Enum.Parse(typeof(PidDataType), sp.DataType);
            pc.DataType = ConvertToDataType(pd);
            List<Conversion> conversions = sp.Conversions;
            pc.IsBitMapped = conversions[0].IsBitMapped;
            pc.BitIndex = conversions[0].BitIndex;
            if (pc.IsBitMapped)
            {
                pc.Math = conversions[0].TrueValue + "," + conversions[0].FalseValue;
            }
            else
            {
                pc.Math = sp.Conversions[0].Expression;
            }
            return pc;
        }


        private void btnGetFreezeFrames_Click(object sender, EventArgs e)
        {
            ushort module;
            dataGridDtcCodes.Rows.Clear();
            dataGridDtcCodes.Columns["Conversion"].Visible = true;
            dataGridDtcCodes.Columns["Scaling"].Visible = true;
            if (!Connect(radioVPW.Checked, true, true, null))
            {
                return;
            }
            Thread.Sleep(100);
            //List<DTCCodeStatus> codelist = new List<DTCCodeStatus>();
            if (radioVPW.Checked)
            {
                if (chkDtcAllModules.Checked)
                {
                    module = DeviceId.Broadcast;
                    //codelist = datalogger.RequestDTCCodes(module, mode);
                }
                else
                {
                    module = (byte)comboModule.SelectedValue;
                    //codelist = datalogger.RequestDTCCodes(module, mode); 
                }
            }
            else //CAN
            {
                module = Convert.ToUInt16(comboModule.SelectedValue);
            }
            Response<List<OBDMessage>> msgs = datalogger.QueryFreezeFrames(module);
            if (msgs.Status != ResponseStatus.Success)
            {
                return;
            }
            byte recNr = 0;
            byte lastRecNr = 0xFF;
            for (int m = 0; m < msgs.Value.Count; m++)
            {
                int r = dataGridDtcCodes.Rows.Add();
                OBDMessage msg = msgs.Value[m];
                byte[] data = msg.GetBytes();
                recNr = data[4];
                ushort dtcNr;
                ushort pid;
                ushort pidData;
                PidInfo pidInfo;
                string status = "";
                if (analyzer.PhysAddresses.ContainsKey(data[2]))
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = analyzer.PhysAddresses[data[2]];
                }
                else
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = data[2];
                }
                if (data.Length == 8 || data.Length == 9)
                {
                    if (recNr != lastRecNr)
                    {
                        //First row for this record number
                        status = "Record number: " + recNr.ToString("X2");
                        dtcNr = (ushort)(data[7] << 8 | data[8]);
                        //status += " Dtc: " + DtcSearch.DecodeDTC(dtcNr.ToString("X4"));
                        string dtcCode = DtcSearch.DecodeDTC(dtcNr.ToString("X4"));
                        dataGridDtcCodes.Rows[r].Cells["Code"].Value = dtcCode;
                        dataGridDtcCodes.Rows[r].Cells["Description"].Value = DtcSearch.GetDtcDescription(dtcCode);
                    }
                    else 
                    {
                        status = recNr.ToString() + " - ";
                        pid = (ushort)(data[5] << 8 | data[6]);
                        pidInfo = PidDescriptions.Where(x => x.PidNumber == pid).FirstOrDefault();
                        if (pidInfo == null)
                            pidInfo = new PidInfo();
                        dataGridDtcCodes.Rows[r].Cells["Code"].Value = pid.ToString("X4");
                        dataGridDtcCodes.Rows[r].Cells["Description"].Value = pidInfo.PidName;
                        dataGridDtcCodes.Rows[r].Cells["Scaling"].Value = pidInfo.Scaling;
                        //status += " PID: " + pidInfo.PidName;
                        if (data.Length == 9)
                            pidData = (ushort)(data[7] << 8 | data[8]);
                        else
                            pidData = data[7];

                        string unit = "";
                        if (!string.IsNullOrEmpty(pidInfo.Unit))
                        {
                            unit = pidInfo.Unit.ToLower();
                        }
                        PidConfig pc;
                        pc = GetConversionFromStd(pid);
                        if (pc != null)
                        {
                            dataGridDtcCodes.Rows[r].Cells["Conversion"].Value = pc.Math;
                        }
                        else if (!string.IsNullOrEmpty(pidInfo.ConversionFactor))
                        {
                            pc = new PidConfig();
                            string math = pidInfo.ConversionFactor;
                            pc.Units = pidInfo.Unit;
                            if (math.StartsWith("bit:"))
                            {
                                string bitNr = math.Substring(4, 1);
                                math = math.Substring(5).Trim();
                                if (byte.TryParse(bitNr, out byte b))
                                    pc.BitIndex = b;
                            }
                            string[] parts = math.Split('=');
                            if (parts.Length == 3)
                            {
                                //Bitmapped
                                math = math.Replace(" ", ",");
                                pc.IsBitMapped = true;
                            }
                            dataGridDtcCodes.Rows[r].Cells["Conversion"].Value = pidInfo.ConversionFactor;
                            pc.Math = math;
                        }
                        else if (!string.IsNullOrEmpty(pidInfo.Scaling))
                        {
                            pc = new PidConfig();
                            pc.Units = pidInfo.Unit;
                            if (pidInfo.Scaling == "N/A")
                            {
                                pc.Math = "X";
                            }
                            else if (pidInfo.Scaling.Contains("="))
                            {
                                string[] parts = pidInfo.Scaling.Split('=');
                                if (parts.Length == 3)
                                {
                                    //Bitmapped
                                    pc.Math = pidInfo.Scaling.Replace(" ",",");
                                    pc.IsBitMapped = true;
                                }
                                else if (parts.Length == 2 && parts[0].Trim() != "DTC")
                                {
                                    string rightSide = parts[1].ToUpper().Trim();
                                    if (rightSide == "COUNT" || rightSide == "COUNTS" || rightSide == "STEPS" || rightSide == "SECONDS")
                                    {
                                        pc.Math = "X";
                                    }
                                    else if (parts[0].Trim() != "N")
                                    {
                                        pc.Math = parts[1].Replace("N", "X");
                                        pc.Math = pc.Math.Replace("Y", "X");
                                        pc.Math = pc.Math.Replace("E", "X");
                                        if (!string.IsNullOrEmpty(unit) && pc.Math.ToLower().Contains(unit))
                                        {
                                            pc.Math = pc.Math.ToLower().Replace(unit, "");
                                        }
                                    }
                                }
                                dataGridDtcCodes.Rows[r].Cells["Conversion"].Value = pc.Math;

                            }
                            //string math = pidInfo.ConversionFactor.ToLower().Replace("x", pidData.ToString());
                            //status += " Value: " + parser.Parse(math).ToString();
                        }
                        if (pc != null && !string.IsNullOrEmpty(pc.Math))
                        {
                            List<SlotHandler.PidVal> PidValues = new List<SlotHandler.PidVal>();
                            SlotHandler.PidVal pv = new SlotHandler.PidVal(pc.addr, pidData);
                            PidValues.Add(pv);
                            string val = pc.GetCalculatedValue(PidValues);
                            if (string.IsNullOrEmpty(val))
                                Logger("Bad math: " + pc.Math);
                            else
                                status += val +" ";
                            status += pc.Units + " [" + pidData.ToString("X4") + "]";
                        }
                    }
                }
                else
                {
                    byte[] tmp = new byte[data.Length - 5];
                    Array.Copy(data, 5, tmp, 0, data.Length - 5);
                    status += BitConverter.ToString(tmp);
                }
                dataGridDtcCodes.Rows[r].Cells["Status"].Value = status;
                lastRecNr = recNr;
            }
        }

        private void chkForkJ2534_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerStartJ2534Process = chkStartJ2534Process.Checked;
            chkJ2534ServerVisible.Enabled = chkStartJ2534Process.Checked;
        }

        private void groupAdvanced_Enter(object sender, EventArgs e)
        {

        }

        private void chkJ2534ServerVisible_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerJ2534ProcessVisible = chkJ2534ServerVisible.Checked;
            AppSettings.Save();
        }

        private void saveCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCsvDatagridview(dataGridDtcCodes);
        }

        private void loadCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCsvDatagridview(dataGridDtcCodes);
        }

        private void timerDeviceStatus_Tick(object sender, EventArgs e)
        {
            if (!chkAutoDisconnect.Checked)
            {
                return;
            }
            if (datalogger.Connected && !datalogger.LogDevice.Connected)
            {
                Logger("Device disconnected");
                timerDeviceStatus.Enabled = false;
                Disconnect(false);
            }
            if (jConsole != null && jConsole.Connected && !jConsole.JDevice.Connected)
            {
                Logger("J-Device disconnected");
                timerDeviceStatus.Enabled = false;
                DisconnectJConsole(false);
            }
        }

        private void btnTimeouts_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            UpatcherSettings tmpSettings = AppSettings.ShallowCopy();
            fpe.resetToDefaultValueToolStripMenuItem.Enabled = true;
            fpe.txtFilter.Text = "timeout";
            fpe.LoadObject(tmpSettings, "Timeouts");
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                AppSettings = tmpSettings;
                AppSettings.Save();
                Logger("Settings saved");
            }
        }

        private void btnCanFilters_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            UpatcherSettings tmpSettings = AppSettings.ShallowCopy();
            fpe.resetToDefaultValueToolStripMenuItem.Enabled = true;
            fpe.txtFilter.Text = "loggerfilter";
            fpe.LoadObject(tmpSettings, "LoggerFilters");
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                AppSettings = tmpSettings;
                AppSettings.Save();
                Logger("Settings saved");
            }
        }

        private void btnJconsoleEditFilters_Click(object sender, EventArgs e)
        {
            try
            {
                frmJ2534Filters fjf = new frmJ2534Filters();
                fjf.LoadTxtFilter(jConsoleFilters);
                if (fjf.ShowDialog() == DialogResult.OK)
                {
                    jConsoleFilters = fjf.GetFinalFilter();
                    JFilters jf = new JFilters(jConsoleFilters, 0);
                    labelJconsoleFilters.Text = "Filters: (" + jf.MessageFilters.Count.ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void btnJconsoleEDitFilters2_Click(object sender, EventArgs e)
        {
            try
            {
                frmJ2534Filters fjf = new frmJ2534Filters();
                fjf.LoadTxtFilter(jConsoleFilters2);
                if (fjf.ShowDialog() == DialogResult.OK)
                {
                    jConsoleFilters2 = fjf.GetFinalFilter();
                    JFilters jf = new JFilters(jConsoleFilters2, 0);
                    labelJconsoleFilters2.Text = "Filters: (" + jf.MessageFilters.Count.ToString() + ")";
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void radioJConsoleProto2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioJConsoleProto1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveProtocols_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile.xmlx");
                string FileName = SelectSaveFile(XmlXFilter, defName);
                if (FileName.Length == 0)
                    return;
                Logger("Saving file: " + FileName);
                J2534InitParameters JSettings = CreateJ2534InitParameters();
                J2534InitParameters JSettings2 = CreateJ2534InitParameters2();
                List<J2534InitParameters> jsl = new List<J2534InitParameters>();
                jsl.Add(JSettings);
                jsl.Add(JSettings2);
                LoggerUtils.SaveJ2534SettingsList(FileName, jsl);
                AppSettings.LoggerJ2534SettingsFile = "";
                AppSettings.LoggerJ2534SettingsFile2 = "";
                AppSettings.Save();
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void btnLoadProtocols_Click(object sender, EventArgs e)
        {
            string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile.xmlx");
            string FileName = SelectFile("Select J2534 settings", JProfileFilter, defName);
            if (FileName.Length == 0)
                return;
            //Logger("Loading file: " + FileName);
            List<J2534InitParameters> JSettings = LoadJ2534SettingsList(FileName);
            LoadJ2534InitParameters(JSettings[0]);
            LoadJ2534InitParameters2(JSettings[1]);
            AppSettings.Save();
            Logger("[OK]");

        }

        private void timerShowLogTxt_Tick(object sender, EventArgs e)
        {
            try
            {
                if (WB != null && WB.AFR > 0)
                {
                    labelAFR.Text = "AFR: " + WB.AFR.ToString("#0.0");
                }
                else
                {
                    //labelAFR.Text = "";
                }
                if (chkVpwToScreen.Checked && chkVpwBuffered.Checked == false)
                {
                    while (consoleLogQueue.Count > 0)
                    {
                        LogText lt = consoleLogQueue.Dequeue();
                        richVPWmessages.SelectionColor = lt.color;
                        richVPWmessages.AppendText(lt.Txt);
                    }
                    Application.DoEvents();
                    return;
                }
                if (lastLogRowCount == logTexts.Count || chkVpwToScreen.Checked == false)
                {
                    return;
                }

                int displayableLines = GetRichBoxDisplayableLines(richVPWmessages);
                int start = 0;
                if (logTexts.Count > displayableLines)
                {
                    start = logTexts.Count - displayableLines;
                }
                richVPWmessages.Text = "";
                for (int i = start; i < logTexts.Count; i++)
                {
                    richVPWmessages.SelectionColor = logTexts[i].color;
                    richVPWmessages.AppendText(logTexts[i].Txt);
                }
                if (logTexts.Count > displayableLines)
                {
                    vScrollBarVpwConsole.Maximum = logTexts.Count - displayableLines;
                    vScrollBarVpwConsole.Value = logTexts.Count - displayableLines;
                }
                lastLogRowCount = logTexts.Count;
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void VPWConsoleScroll()
        {
            try
            {
                int start = vScrollBarVpwConsole.Value;
                int displayableLines = GetRichBoxDisplayableLines(richVPWmessages);
                if (logTexts.Count < displayableLines || (start + displayableLines) > logTexts.Count)
                {
                    return;
                }
                if ((start + displayableLines) > logTexts.Count)
                {
                    start = logTexts.Count - displayableLines;
                }
                LockWindowUpdate(richVPWmessages.Handle);
                richVPWmessages.Text = "";
                for (int i = start; i < (start + displayableLines); i++)
                {
                    richVPWmessages.SelectionColor = logTexts[i].color;
                    richVPWmessages.AppendText(logTexts[i].Txt);
                }
                LockWindowUpdate(IntPtr.Zero);
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void vScrollBarVpwConsole_Scroll(object sender, ScrollEventArgs e)
        {
            VPWConsoleScroll();
        }

        private void chkJconsoleToScreen_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void timerJconsoleShowLogText_Tick(object sender, EventArgs e)
        {
            try
            {
                if (chkJconsoleToScreen.Checked && chkJconsoleUsebuffer.Checked == false)
                {
                    while (jconsoleLogQueue.Count > 0)
                    {
                        LogText lt = jconsoleLogQueue.Dequeue();
                        richJConsole.SelectionColor = lt.color;
                        richJConsole.AppendText(lt.Txt);
                    }
                    Application.DoEvents();
                    return;
                }

                if (jconsolelastLogRowCount == jconsolelogTexts.Count || chkJconsoleToScreen.Checked == false)
                {
                    return;
                }
                int displayableLines = GetRichBoxDisplayableLines(richJConsole);
                int start = 0;
                if (jconsolelogTexts.Count > displayableLines)
                {
                    start = jconsolelogTexts.Count - displayableLines;
                }
                richJConsole.Text = "";
                for (int i = start; i < jconsolelogTexts.Count; i++)
                {
                    richJConsole.SelectionColor = jconsolelogTexts[i].color;
                    richJConsole.AppendText(jconsolelogTexts[i].Txt);
                }
                if (jconsolelogTexts.Count > displayableLines)
                {
                    vScrollBarJConsole.Maximum = jconsolelogTexts.Count - displayableLines;
                    vScrollBarJConsole.Value = jconsolelogTexts.Count - displayableLines;
                }
                jconsolelastLogRowCount = jconsolelogTexts.Count;
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void JConsoleScroll()
        {
            try
            {
                int start = vScrollBarJConsole.Value;
                int displayableLines = GetRichBoxDisplayableLines(richJConsole);
                if (jconsolelogTexts.Count < displayableLines || (start + displayableLines) > jconsolelogTexts.Count)
                {
                    return;
                }
                LockWindowUpdate(richJConsole.Handle);
                richJConsole.Text = "";
                for (int i = start; i < (start + displayableLines); i++)
                {
                    richJConsole.SelectionColor = jconsolelogTexts[i].color;
                    richJConsole.AppendText(jconsolelogTexts[i].Txt);
                }
                LockWindowUpdate(IntPtr.Zero);
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void vScrollBarJConsole_Scroll(object sender, ScrollEventArgs e)
        {
            JConsoleScroll();
        } 

        private void saveVPWConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectSaveFile(RtfFilter);
                if (fName.Length == 0)
                {
                    return;
                }
                Logger("Saving to file " + fName, false);
                //RichTextBox rtb = new RichTextBox();
                Application.DoEvents();
                StreamWriter cStream = new StreamWriter(fName);
                cStream.WriteLine("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1035{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New;}}");
                cStream.WriteLine("{\\colortbl ;\\red127\\green255\\blue212;\\red0\\green100\\blue0;\\red255\\green0\\blue0;\\red128\\green0\\blue128;}");
                for (int r = 0; r < logTexts.Count; r++)
                {
                    if (r % 300 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                    //rtb.SelectionColor = logTexts[r].color;
                    //rtb.AppendText(logTexts[r].Txt);
                    string colort = "cf1";
                    if (logTexts[r].color == Color.Aquamarine)
                        colort = "cf1";
                    else if (logTexts[r].color == Color.DarkGreen)
                        colort = "cf2";
                    else if (logTexts[r].color == Color.Red)
                        colort = "cf3";
                    else if (logTexts[r].color == Color.Purple)
                        colort = "cf4";
                    cStream.WriteLine("\\" + colort + " " + logTexts[r].Txt + "\\par");
                }
                cStream.WriteLine("}");
                cStream.Close();
                //rtb.SaveFile(fName);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void saveJConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectSaveFile(RtfFilter);
                if (fName.Length == 0)
                {
                    return;
                }
                Logger("Saving to file " + fName, false);
                Application.DoEvents();
                //RichTextBox rtb = new RichTextBox();
                StreamWriter jStream = new StreamWriter(fName);
                jStream.WriteLine("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1035{\\fonttbl{\\f0\\fnil\\fcharset0 Courier New;}}");
                jStream.WriteLine("{\\colortbl ;\\red127\\green255\\blue212;\\red0\\green100\\blue0;\\red255\\green0\\blue0;\\red128\\green0\\blue128;}");

                for (int r = 0; r < jconsolelogTexts.Count; r++)
                {
                    if (r%300 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                    //rtb.SelectionColor = jconsolelogTexts[r].color;
                    //rtb.AppendText(jconsolelogTexts[r].Txt);
                    string colort = "cf1";
                    if (jconsolelogTexts[r].color == Color.Aquamarine)
                            colort = "cf1";
                    else if (jconsolelogTexts[r].color == Color.DarkGreen)
                            colort = "cf2";
                    else if (jconsolelogTexts[r].color == Color.Red)
                        colort = "cf3";
                    else if (jconsolelogTexts[r].color == Color.Purple)
                        colort = "cf4";                    
                    jStream.WriteLine("\\"+colort+" " + jconsolelogTexts[r].Txt + "\\par");
                }
                jStream.WriteLine("}");
                jStream.Close();
                //rtb.SaveFile(fName);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void chkVpwBuffered_CheckedChanged(object sender, EventArgs e)
        {
            //timerShowLogTxt.Enabled = chkVpwBuffered.Checked;
            vScrollBarVpwConsole.Visible = chkVpwBuffered.Checked;
            if (chkVpwBuffered.Checked)
            {
                richVPWmessages.KeyDown += RichVPWmessages_KeyDown;
                richVPWmessages.MouseWheel += RichVPWmessages_MouseWheel;
            }
            else
            {
                richVPWmessages.KeyDown -= RichVPWmessages_KeyDown;
                richVPWmessages.MouseWheel -= RichVPWmessages_MouseWheel;
                LockWindowUpdate(richVPWmessages.Handle);
                richVPWmessages.Text = "";
                for (int i = 0; i < logTexts.Count; i++)
                {
                    richVPWmessages.SelectionColor = logTexts[i].color;
                    richVPWmessages.AppendText(logTexts[i].Txt);
                }
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void chkVpwToScreen_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkJconsoleUsebuffer_CheckedChanged(object sender, EventArgs e)
        {
            //timerJconsoleShowLogText.Enabled = chkJconsoleUsebuffer.Checked;
            vScrollBarJConsole.Visible = chkJconsoleUsebuffer.Checked;
            if (chkJconsoleUsebuffer.Checked)
            {
                richJConsole.MouseWheel += RichJConsole_MouseWheel;
                richJConsole.KeyDown += RichJConsole_KeyDown;
            }
            else
            {
                richJConsole.MouseWheel -= RichJConsole_MouseWheel;
                richJConsole.KeyDown -= RichJConsole_KeyDown;
                LockWindowUpdate(richJConsole.Handle);
                richJConsole.Text = "";
                for (int i = 0; i < jconsolelogTexts.Count; i++)
                {
                    richJConsole.SelectionColor = jconsolelogTexts[i].color;
                    richJConsole.AppendText(jconsolelogTexts[i].Txt);
                }
                LockWindowUpdate(IntPtr.Zero);
            }
        }

        private void numRetryTimes_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.RetryWriteTimes = (int)numRetryTimes.Value;
        }

        private void numRetryDelay_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.RetryWriteDelay = (int)numRetryDelay.Value;
        }

        private void radioEecv_CheckedChanged(object sender, EventArgs e)
        {
            if (radioEecv.Checked)
            {
                labelAlgo.Text = "Seed byte:";
                labelSeed.Text = "Seed:";
                txtAlgo.Enabled = true;
                txtAlgoRange.Enabled = false;
                chkEecvSecondKey.Enabled = true;
            }
            else
            {
                chkEecvSecondKey.Enabled = false;
            }
        }

        private void FilterPidsByOS()
        {
            try
            {                
                if (string.IsNullOrEmpty(comboFilterByOS.Text) || comboFilterByOS.Text == "-")
                {
                    SupportedPids = null;
                }
                else
                {
                    string ospidfile = Path.Combine(Application.StartupPath, "Logger", "ospids", comboFilterByOS.Text + ".txt");
                    if (File.Exists(ospidfile))
                    {
                        SupportedPids = new List<int>();
                        StreamReader sr = new StreamReader(ospidfile);
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (HexToUshort(line, out ushort pid))
                            {
                                SupportedPids.Add(pid);
                            }
                        }
                    }
                }
                if (chkFilterParamsByOS.Checked)
                {
                    ReloadPidParams(false, true);
                }
                AppSettings.LoggerLastFilterOS = comboFilterByOS.Text;
                AppSettings.Save();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }

        }

        private void comboFilterByOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterPidsByOS();
        }

        private void btnTestPids_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Connect(radioVPW.Checked, false, true, null))
                {
                    return;
                }
                comboFilterByOS.Text = datalogger.OS;
                SupportedPids = new List<int>();
                Application.DoEvents();
                datalogger.Receiver.SetReceiverPaused(true);
                foreach (LogParam.PidParameter parm in datalogger.PidParams.Where(X=>X.Type == LogParam.DefineBy.Pid))
                {
                    if (!SupportedPids.Contains(parm.Address))
                    {
                        ReadValue rv = datalogger.QuerySinglePidValue(parm);
                        if (rv.PidNr == parm.Address && rv.FailureCode == 0 && rv.PidValue > double.MinValue)
                        {
                            SupportedPids.Add(parm.Address);
                        }
                    }
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
                if (string.IsNullOrEmpty(datalogger.OS))
                {
                    string ospidfile = Path.Combine(Application.StartupPath, "Logger", "ospids", datalogger.OS + ".txt");
                    if (!File.Exists(ospidfile))
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int p = 0; p < SupportedPids.Count; p++)
                        {
                            sb.AppendLine(SupportedPids[p].ToString("X4"));
                        }
                        WriteTextFile(ospidfile, sb.ToString());
                    }
                }
                ReloadPidParams(false, true);
                datalogger.Receiver.SetReceiverPaused(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void btnTestModulePids_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Connect(radioVPW.Checked,false,true,null))
                {
                    return;
                }
                Logger("Querying pids...");
                string[] rParts = txtPidRange.Text.Split('-');
                if (rParts.Length !=2)
                {
                    return;
                }
                ushort start;
                ushort end;
                if (!HexToUshort(rParts[0], out start))
                {
                    LoggerBold("Unknown range: " + txtPidRange.Text);
                    return;
                }
                if (!HexToUshort(rParts[1], out end))
                {
                    LoggerBold("Unknown range: " + txtPidRange.Text);
                    return;
                }
                datalogger.Receiver.SetReceiverPaused(true);
                int[] filterIds = null;
                if (!radioVPW.Checked && (ushort)comboModule.SelectedValue != datalogger.CanDevice.ResID)
                {
                    filterIds = datalogger.SetCanQueryFilter((ushort)comboModule.SelectedValue);
                }
                ushort module = Convert.ToUInt16(comboModule.SelectedValue);
                for (ushort p=start; p<=end;p++)
                {
                    ReadValue rv = datalogger.QueryModulePidValue(p, module);
                    if (!datalogger.Connected)
                    {
                        return;
                    }
                }
                if (filterIds != null)
                {
                    datalogger.LogDevice.RemoveFilters(filterIds);
                }
                Logger("Done");
                datalogger.Receiver.SetReceiverPaused(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                datalogger.Receiver.SetReceiverPaused(false);
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void radioCAN_CheckedChanged(object sender, EventArgs e)
        {
            datalogger.SetProtocol(radioVPW.Checked);
            LoadModuleList();
            SetupPidEditor();
        }

        private void radioVPW_CheckedChanged(object sender, EventArgs e)
        {
            groupCanParams.Enabled = !radioVPW.Checked;
            //serialRadioButton.Checked = !radioVPW.Checked;
            chkCombinePids.Enabled = !radioVPW.Checked;
            if (radioVPW.Checked)
            {
                chkCombinePids.Checked = false;
            }

        }

        private void labelReceiver_Click(object sender, EventArgs e)
        {

        }

        private void btnStartIdleTraffic_Click(object sender, EventArgs e)
        {
            if (datalogger.Connected)
            {
                datalogger.LogDevice.StartIdleTraffic((int)numIdleTrafficInterval.Value);
            }
        }

        private void clearVPWConsoleDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logTexts = new List<LogText>();
            richVPWmessages.Clear();
        }

        private void clearJConsoleDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jconsolelogTexts = new List<LogText>();
            richJConsole.Clear();
        }

        private void parseCANMode36LogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtFilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.CAN36);
            cltb.ConvertFile(fName);
        }

        private void parseCAN23LogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtFilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.CAN23);
            cltb.ConvertFile(fName);
        }

        private void btnSendControlCommand_Click(object sender, EventArgs e)
        {
            frmControlCommands fcc = new frmControlCommands();
            fcc.datalogger = datalogger;
            fcc.Show();
        }

        private void graphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowGraphics(true);
        }

        private void histogramLiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PCM == null)
            {
                PCM = new PcmFile();
            }
            HstForm = new frmHistogram(true, PCM);
            HstForm.Show();
            HstForm.SetupLiveParameters();

        }

        private void histogramLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PCM == null)
            {
                PCM = new PcmFile();
            }
            HstForm = new frmHistogram(false, PCM);
            HstForm.Show();

        }

        private void sendControlCommandsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmControlCommands fcc = new frmControlCommands();
            fcc.datalogger = datalogger;
            fcc.Show();

        }

        private void graphicsLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowGraphics(false);

        }

        private void chkVpwDevTimestamps_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleDevTimestamps = chkVpwDevTimestamps.Checked;
        }

        private void chkConsoleTimestamps_CheckedChanged(object sender, EventArgs e)
        {
            ConsoleTimestamps = chkConsoleTimestamps.Checked;
        }

        private void parseLogfileToBinToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void parseCAN78LogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtFilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.CAN78);
            cltb.ConvertFile(fName);
        }

        private void comboWBType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.Wbtype = (WideBand.WBType)comboWBType.SelectedItem;
            if (AppSettings.Wbtype == WideBand.WBType.Elm327_CAN)
            {
                txtWBCanId.Enabled = true;
                AppSettings.WBCanID = txtWBCanId.Text;
            }
            else
            {
                txtWBCanId.Enabled = false;
            }
            //RestartWB();
        }

        private void RestartWB()
        {
            try
            {
                AppSettings.WBSerial = comboWBport.Text;
                AppSettings.Wbtype = (WideBand.WBType)comboWBType.SelectedItem;
                if (AppSettings.Wbtype == WideBand.WBType.Elm327_CAN)
                {
                    txtWBCanId.Enabled = true;
                    AppSettings.WBCanID = txtWBCanId.Text;
                }
                else
                {
                    txtWBCanId.Enabled = false;
                }
                AppSettings.Save();
                if (WB != null)
                {
                    WB.Discard();
                    WB = null;
                }
                WB = new WideBand();
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace(ex, true);
                StackFrame frame = st.GetFrame(st.FrameCount - 1);
                int line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void comboWBport_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.WBSerial = comboWBport.Text;
            //RestartWB();
        }

        private void chkJConsoleTimestamps_CheckedChanged(object sender, EventArgs e)
        {
            jConsoleTimestamps = chkJConsoleTimestamps.Checked;
        }

        private void chkJconsoleDevTimestamps_CheckedChanged(object sender, EventArgs e)
        {
            jConsoleDevTimestamps = chkJconsoleDevTimestamps.Checked;
        }

        private void parseKline23LogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtFilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.KLINE23);
            cltb.ConvertFile(fName);

        }

        private void patcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmpatcher = new FrmPatcher();
            frmpatcher.Show();
        }

        private void tunerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmTuner tuner = new FrmTuner(PCM);
            tuner.Show();
        }

        private void launcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMain fMain = new FrmMain();
            fMain.Show();
        }

        private void createDebugLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createDebugLogToolStripMenuItem.Checked = !createDebugLogToolStripMenuItem.Checked;

            if (createDebugLogToolStripMenuItem.Checked)
            {
                string fName = SelectSaveFile(DebuglogFilter);
                if (string.IsNullOrEmpty(fName))
                {
                    createDebugLogToolStripMenuItem.Checked = false;
                    return;
                }
                DebugFileListener = new FileTraceListener(fName);
                Debug.Listeners.Add(DebugFileListener);
            }
            else
            {
                Debug.Listeners.Remove(DebugFileListener);
                DebugFileListener.CloseLog();
            }
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dashboard = new frmDashboard();
            dashboard.Show();
        }


        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPidEditor.SelectedCells.Count == 0)
                {
                    return;
                }
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
                if (parm.Type == LogParam.DefineBy.Math)
                {
                    frmPidSelector fps = new frmPidSelector();
                    if (fps.ShowDialog() == DialogResult.OK)
                    {
                        parm.AddPid(fps.Id, fps.Conversion);
                        ShowParmPids(parm);
                    }
                }
                else if (parm.Type == LogParam.DefineBy.Address)
                {
                    parm.Locations.Add(new LogParam.Location());
                    dataGridViewPidEditorPids.DataSource = null;
                    dataGridViewPidEditorPids.DataSource = parm.Locations;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPidEditorPids.SelectedCells.Count == 0 || dataGridViewPidEditor.SelectedCells.Count == 0)
                {
                    return;
                }
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
                if (parm.Type == LogParam.DefineBy.Math)
                {
                    LogParam.LogPid pid = (LogParam.LogPid)dataGridViewPidEditorPids.Rows[dataGridViewPidEditorPids.SelectedCells[0].RowIndex].Tag;
                    parm.Pids.Remove(pid);
                    ShowParmPids(parm);
                }
                else if (parm.Type == LogParam.DefineBy.Address)
                {
                    LogParam.Location location = (LogParam.Location)dataGridViewPidEditorPids.Rows[dataGridViewPidEditorPids.SelectedCells[0].RowIndex].DataBoundItem;
                    parm.Locations.Remove(location);

                    dataGridViewPidEditorPids.DataSource = null;
                    BindingList<LogParam.Location> locations = new BindingList<LogParam.Location>(parm.Locations);
                    dataGridViewPidEditorPids.DataSource = locations;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string defname;
            if (radioVPW.Checked)
            {
                defname = Path.Combine(Application.StartupPath, "Logger", "PidPrameters-VPW.XML");
            }
            else
            {
                defname = Path.Combine(Application.StartupPath, "Logger", "PidPrameters-CAN.XML");
            }
            string fName = SelectFile("Select PID file", PidParmFilter, defname);
            if (fName.Length == 0)
            {
                return;
            }
            LogParamFile.LoadParamfile(fName);
            ReloadPidParams(true,true);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogParamFile.SaveParamfile(PidParamFile); 
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectSaveFile(PidParmFilter, PidParamFile);
            LogParamFile.SaveParamfile(fName);
        }

        private void importPCMHammerFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogParamFile.ImportPCMHammerFiles();
            ReloadPidParams(true,true);
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPidEditor.SelectedCells.Count == 0)
                {
                    return;
                }
                Conversion conversion = new Conversion("", "", "");
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
                parm.Conversions.Add(conversion);
                dataGridViewPidEditorConversions.DataSource = null;
                dataGridViewPidEditorConversions.DataSource = parm.Conversions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPidEditorConversions.SelectedCells.Count == 0)
                {
                    return;
                }
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
                Conversion conversion = (Conversion)dataGridViewPidEditorConversions.Rows[dataGridViewPidEditorConversions.SelectedCells[0].RowIndex].DataBoundItem;
                parm.Conversions.Remove(conversion);
                dataGridViewPidEditorConversions.DataSource = null;
                dataGridViewPidEditorConversions.DataSource = parm.Conversions;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void removeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPidEditor.SelectedCells.Count == 0)
                {
                    return;
                }
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
                if (MessageBox.Show("Remove pid: " + parm.Name + "?", "Remove pid?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    datalogger.PidParams.Remove(parm);
                    pidparams.ResetBindings();
                    dataGridViewPidEditor.Update();
                    dataGridViewPidEditor.Refresh();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AddNewPid()
        {
            try
            {
                LogParam.PidParameter parm = new LogParam.PidParameter();
                datalogger.PidParams.Add(parm);
                pidparams.ResetBindings();
                dataGridViewPidEditor.Update();
                dataGridViewPidEditor.Refresh();
                dataGridViewPidEditor.CurrentCell = dataGridViewPidEditor.Rows[dataGridViewPidEditor.Rows.Count - 2].Cells[1];
                LoadPidEditorValues(parm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void addToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddNewPid();
        }

        private void showOldPIDProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string defname = Path.Combine(Application.StartupPath, "Logger","Profiles", "profile1.xml");
            string fName = SelectFile("Select PID profile", ProfileFilter, defname);
            if (fName.Length == 0)
            {
                return;
            }
            LogParamFile.ImportOldProfile(fName);
            frmEditXML fex = new frmEditXML();
            fex.LoadOldPidProfile(fName);
            fex.Show();
        }

        private void SetConversionValues()
        {
            try
            {
                if (dataGridViewPidEditorConversions.SelectedCells.Count == 0 || DisableConversionChanges)
                {
                    return;
                }
                DisableConversionChanges = true;
                Conversion conversion = (Conversion)dataGridViewPidEditorConversions.Rows[dataGridViewPidEditorConversions.SelectedCells[0].RowIndex].DataBoundItem;
                conversion.IsBitMapped = chkConversionBitMapped.Checked;
                conversion.BitIndex = (int)numConversionBitIndex.Value;
                conversion.TrueValue = txtConversionTrueValue.Text;
                conversion.FalseValue = txtConversionFalseValue.Text;
                conversion.Expression = txtConversionExpression.Text;
                conversion.Units = txtConversionUnits.Text;
                conversion.Format = txtConversionFormat.Text;
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
                dataGridViewPidEditorConversions.DataSource = null;
                dataGridViewPidEditorConversions.DataSource = parm.Conversions;
                DisableConversionChanges = false;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }
        private void chkConversionBitMapped_CheckedChanged(object sender, EventArgs e)
        {
            txtConversionFalseValue.Enabled = chkConversionBitMapped.Checked;
            txtConversionTrueValue.Enabled = chkConversionBitMapped.Checked;
            numConversionBitIndex.Enabled = chkConversionBitMapped.Checked;
            SetConversionValues();
        }

        private void numConversionBitIndex_ValueChanged(object sender, EventArgs e)
        {
            SetConversionValues();
        }

        private void txtConversionTrueValue_TextChanged(object sender, EventArgs e)
        {
            SetConversionValues();
        }

        private void txtConversionFalseValue_TextChanged(object sender, EventArgs e)
        {
            SetConversionValues();
        }

        private void txtConversionUnits_TextChanged(object sender, EventArgs e)
        {
            SetConversionValues();
        }

        private void txtConversionFormat_TextChanged(object sender, EventArgs e)
        {
            SetConversionValues();
        }

        private void txtConversionExpression_TextChanged(object sender, EventArgs e)
        {
            SetConversionValues();
        }

        private void txtParamSearch_TextChanged(object sender, EventArgs e)
        {
            ReloadPidParams(false,true);
        }

        private void txtPidEditorFilter_TextChanged(object sender, EventArgs e)
        {
            ReloadPidParams(true,false);
        }

        private void chkFilterParamsByOS_CheckedChanged(object sender, EventArgs e)
        {
            ReloadPidParams(false, true);
        }

        private void btnTestEnabledPids_Click(object sender, EventArgs e)
        {
            ShowEnabledPidsValues(true);
            Logger("OK");
        }

        private void btnFilterByBin_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select BIN file");
            if (fName.Length == 0)
                return;
            PCM = new PcmFile(fName, true, "");
            FilterPidsByBin(PCM);
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            string fName = null;
            if (!string.IsNullOrEmpty(comboProfileFilename.Text))
            {
                string platform = comboProfilePlatform.Text.Replace("_Undefined", "").Replace("_All","");
                string category = comboProfileCategory.Text.Replace("_Undefined", "").Replace("_All", "");
                fName = Path.Combine(Application.StartupPath, "Logger", "Profiles", platform, category, comboProfileFilename.Text);
                if (!fName.ToLower().EndsWith(".xml"))
                {
                    fName += ".xml";
                }
            }
            SelectSaveProfile(fName);
        }

        private void UpdateComboProfileFileNames()
        {
            try
            {
                string platform = "";
                string category = "";
                SearchOption sOption = SearchOption.AllDirectories;
                if (!string.IsNullOrEmpty(comboProfileCategory.Text) && comboProfileCategory.Text != "_All")
                {
                    if (comboProfileCategory.Text == "_Undefined")
                    {
                        sOption = SearchOption.TopDirectoryOnly;
                    }
                    else
                    {
                        category = comboProfileCategory.Text;
                    }
                }
                if (!string.IsNullOrEmpty(comboProfilePlatform.Text) && comboProfilePlatform.Text != "_All")
                {
                    if (comboProfilePlatform.Text == "_Undefined")
                    {
                        sOption = SearchOption.TopDirectoryOnly;
                    }
                    else
                    {
                        platform = comboProfilePlatform.Text;
                    }
                }
                string ProfilePath = Path.Combine(Application.StartupPath, "Logger", "Profiles", platform, category);

                string currentFile = "";
                comboProfileFilename.Items.Clear();
                comboProfileFilename.ValueMember = "Value";
                comboProfileFilename.DisplayMember = "Key";
                if (Directory.Exists(ProfilePath))
                {
                    DirectoryInfo d = new DirectoryInfo(ProfilePath);
                    FileInfo[] Files;
                    Files = d.GetFiles("*.*", sOption);
                    foreach (FileInfo file in Files)
                    {
                        KeyValuePair<string, string> itm = new KeyValuePair<string, string>(file.Name, file.FullName);
                        comboProfileFilename.Items.Add(itm);
                        if (profileFile == file.FullName)
                        {
                            currentFile = file.Name;
                        }
                    }
                }
                comboProfileFilename.Text = currentFile;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void comboProfilePlatform_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboProfileFileNames();
        }

        private void comboProfileCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateComboProfileFileNames();
        }

        private void btnProfileLoad_Click(object sender, EventArgs e)
        {
            string platform = comboProfilePlatform.Text.Replace("_Undefined", "").Replace("_All", "");
            string category = comboProfileCategory.Text.Replace("_Undefined", "").Replace("_All", "");
            string fName = Path.Combine(Application.StartupPath, "Logger", "Profiles", platform, category, comboProfileFilename.Text);
            if (File.Exists(fName))
            {
                LoadProfile(fName);
            }
            else
            {
                Logger("File not found: " + fName);
            }
        }

        private void comboStartLoggingKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerStartKey = (Keys)Enum.Parse(typeof(Keys), comboStartLoggingKey.Text);
            if (datalogger.LogRunning)
                btnStartStop.Text = "Stop logging (" + comboStartLoggingKey.Text + ")";
            else
                btnStartStop.Text = "Start logging (" + comboStartLoggingKey.Text + ")";
        }

        private void SelectPlatform_Category(string fName)
        {
            try
            {
                this.comboProfilePlatform.SelectedIndexChanged -= new System.EventHandler(this.comboProfilePlatform_SelectedIndexChanged);
                this.comboProfileCategory.SelectedIndexChanged -= new System.EventHandler(this.comboProfileCategory_SelectedIndexChanged);
                string subFolder = Path.GetDirectoryName(fName);
                string profFolder = Path.Combine(Application.StartupPath, "Logger", "Profiles");
                if (subFolder.Contains(profFolder))
                {
                    string subPart = subFolder.Replace(profFolder, "");
                    string[] fParts = subPart.Trim('\\').Split('\\');
                    if (fParts.Length > 0)
                    {
                        comboProfilePlatform.Text = fParts[0];
                    }
                    if (fParts.Length > 1)
                    {
                        comboProfileCategory.Text = fParts[1];
                    }
                }
                if (string.IsNullOrEmpty(comboProfilePlatform.Text))
                {
                    comboProfilePlatform.Text = "_Undefined";
                }
                if (string.IsNullOrEmpty(comboProfileCategory.Text))
                {
                    comboProfileCategory.Text = "_Undefined";
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
            this.comboProfilePlatform.SelectedIndexChanged += new System.EventHandler(this.comboProfilePlatform_SelectedIndexChanged);
            this.comboProfileCategory.SelectedIndexChanged += new System.EventHandler(this.comboProfileCategory_SelectedIndexChanged);

        }
        private void comboProfileFilename_SelectedIndexChanged(object sender, EventArgs e)
        {
            KeyValuePair<string, string> itm = (KeyValuePair<string, string>)comboProfileFilename.SelectedItem;
            SelectPlatform_Category(itm.Value);
        }

        private void addToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (dataGridProfile.SelectedCells.Count ==0)
            {
                return;
            }
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridProfile.Rows[dataGridProfile.SelectedCells[0].RowIndex].DataBoundItem;
            LogParam.PidSettings pidSettings = new LogParam.PidSettings();
            pidSettings.Parameter = parm;
            datalogger.SelectedPids.Add(pidSettings);
            SetProfileDirty(true,true);
            AddPidToSelectedPidGrid(pidSettings);
            CheckMaxPids();
            //SetupLogDataGrid();
        }

        private void removeToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (dataGridSelectedPids.SelectedCells.Count == 0)
            {
                return;
            }
            LogParam.PidSettings pidSettings = (LogParam.PidSettings)dataGridSelectedPids.Rows[dataGridSelectedPids.SelectedCells[0].RowIndex].Tag;            
            datalogger.SelectedPids.Remove(pidSettings);
            dataGridSelectedPids.Rows.RemoveAt(dataGridSelectedPids.SelectedCells[0].RowIndex);
            SetProfileDirty(true,true);
            //SetupLogDataGrid();
        }

        private void removeToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (dataGridLogData.SelectedCells.Count == 0)
            {
                return;
            }
            LogParam.PidSettings pidSettings = (LogParam.PidSettings)dataGridLogData.Rows[dataGridLogData.SelectedCells[0].RowIndex].Tag;
            datalogger.SelectedPids.Remove(pidSettings);
            SetProfileDirty(true,true);
            SetupLogDataGrid();
        }

        private void txtPidname_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboPidEditorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DisablePidEditorChanges)
            {
                Debug.WriteLine("DisablePidEditorChanges");
                return;
            }
            if (dataGridViewPidEditor.SelectedCells.Count == 0)
            {
                return;
            }
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
            parm.Type = (LogParam.DefineBy)comboPidEditorType.SelectedItem;
            txtPidId.Text = "";            
            dataGridViewPidEditor.Update();
            this.Refresh();
            LoadPidEditorValues(parm);
        }

        private void btnAddNewPid_Click(object sender, EventArgs e)
        {
            AddNewPid();
        }

        private void DuplicatePid()
        {
            if (dataGridViewPidEditor.SelectedCells.Count == 0)
            {
                return;
            }
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
            LogParam.PidParameter parmNew = parm.ShallowCopy();
            datalogger.PidParams.Add(parmNew);
            pidparams.ResetBindings();
            dataGridViewPidEditor.Update();
            this.Refresh();
            dataGridViewPidEditor.CurrentCell = dataGridViewPidEditor.Rows[dataGridViewPidEditor.Rows.Count - 2].Cells[1];
        }
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DuplicatePid();
        }

        private void btnDuplicatePid_Click(object sender, EventArgs e)
        {
            DuplicatePid();
        }

        private void SetWorkingMode()
        {
            switch(AppSettings.WorkingMode)
            {
                case 0: //Tourist
                    if (tabControl1.Contains(tabAnalyzer))
                    {
                        tabControl1.Controls.Remove(tabAnalyzer);
                    }
                    if (tabControl1.Contains(tabVPWConsole))
                    {
                        tabControl1.Controls.Remove(tabVPWConsole);
                    }
                    if (tabControl1.Contains(tabJConsole))
                    {
                        tabControl1.Controls.Remove(tabJConsole);
                    }
                    if (tabControl1.Contains(tabAlgoTest))
                    {
                        tabControl1.Controls.Remove(tabAlgoTest);
                    }
                    if (tabControl1.Contains(tabCANdevices))
                    {
                        tabControl1.Controls.Remove(tabCANdevices);
                    }
                    if (tabControl1.Contains(tabPidEditor))
                    {
                        tabControl1.Controls.Remove(tabPidEditor);
                    }
                    if (tabControl1.Contains(tabDTC))
                    {
                        tabControl1.Controls.Remove(tabDTC);
                    }
                    groupAdvanced.Visible = false;
                    actionToolStripMenuItem.Visible = false;
                    groupPlayback.Visible = false;
                    touristToolStripMenuItem.Checked = true;
                    basicToolStripMenuItem.Checked = false;
                    advancedToolStripMenuItem.Checked = false;
                    playbackLogfileToolStripMenuItem.Visible = false;
                    saveVPWConsoleToolStripMenuItem.Visible = false;
                    saveJConsoleToolStripMenuItem.Visible = false;
                    sendControlCommandsToolStripMenuItem.Visible = false;
                    showOldPIDProfileToolStripMenuItem.Visible = false;
                    break;
                case 1: //Basic
                    if (tabControl1.Contains(tabAnalyzer))
                    {
                        tabControl1.Controls.Remove(tabAnalyzer);
                    }
                    if (tabControl1.Contains(tabVPWConsole))
                    {
                        tabControl1.Controls.Remove(tabVPWConsole);
                    }
                    if (tabControl1.Contains(tabJConsole))
                    {
                        tabControl1.Controls.Remove(tabJConsole);
                    }
                    if (tabControl1.Contains(tabAlgoTest))
                    {
                        tabControl1.Controls.Remove(tabAlgoTest);
                    }
                    if (tabControl1.Contains(tabCANdevices))
                    {
                        tabControl1.Controls.Remove(tabCANdevices);
                    }
                    if (tabControl1.Contains(tabPidEditor))
                    {
                        tabControl1.Controls.Remove(tabPidEditor);
                    }
                    if (!tabControl1.Contains(tabDTC))
                    {
                        tabControl1.Controls.Add(tabDTC);
                    }
                    groupAdvanced.Visible = false;
                    actionToolStripMenuItem.Visible = false;
                    groupPlayback.Visible = true;
                    touristToolStripMenuItem.Checked = false;
                    basicToolStripMenuItem.Checked = true;
                    advancedToolStripMenuItem.Checked = false;
                    playbackLogfileToolStripMenuItem.Visible = false;
                    saveVPWConsoleToolStripMenuItem.Visible = false;
                    saveJConsoleToolStripMenuItem.Visible = false;
                    sendControlCommandsToolStripMenuItem.Visible = false;
                    showOldPIDProfileToolStripMenuItem.Visible = true;
                    break;
                case 2:
                    if (!tabControl1.Contains(tabPidEditor))
                    {
                        tabControl1.Controls.Add(tabPidEditor);
                    }
                    if (!tabControl1.Contains(tabAnalyzer))
                    {
                        tabControl1.Controls.Add(tabAnalyzer);
                    }
                    if (!tabControl1.Contains(tabDTC))
                    {
                        tabControl1.Controls.Add(tabDTC);
                    }
                    if (!tabControl1.Contains(tabVPWConsole))
                    {
                        tabControl1.Controls.Add(tabVPWConsole);
                    }
                    if (!tabControl1.Contains(tabJConsole))
                    {
                        tabControl1.Controls.Add(tabJConsole);
                    }
                    if (!tabControl1.Contains(tabAlgoTest))
                    {
                        tabControl1.Controls.Add(tabAlgoTest);
                    }
                    if (!tabControl1.Contains(tabCANdevices))
                    {
                        tabControl1.Controls.Add(tabCANdevices);
                    }
                    groupAdvanced.Visible = true;
                    groupPlayback.Visible = true;
                    actionToolStripMenuItem.Visible = true;
                    touristToolStripMenuItem.Checked = false;
                    basicToolStripMenuItem.Checked = false;
                    advancedToolStripMenuItem.Checked = true;
                    playbackLogfileToolStripMenuItem.Visible = true;
                    saveVPWConsoleToolStripMenuItem.Visible = true;
                    saveJConsoleToolStripMenuItem.Visible = true;
                    sendControlCommandsToolStripMenuItem.Visible = true;
                    showOldPIDProfileToolStripMenuItem.Visible = true;
                    break;
            }
        }

        private void touristToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.WorkingMode = 0;
            AppSettings.Save();
            SetWorkingMode();
        }

        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.WorkingMode = 1;
            AppSettings.Save();
            SetWorkingMode();
        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppSettings.WorkingMode = 2;
            AppSettings.Save();
            SetWorkingMode();
        }

        private void chkPideditorCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (DisablePidEditorChanges)
            {
                return;
            }
            if (dataGridViewPidEditor.SelectedCells.Count == 0)
            {
                return;
            }
            LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[dataGridViewPidEditor.SelectedCells[0].RowIndex].DataBoundItem;
            parm.Custom = chkPideditorCustom.Checked;
            pidparams.ResetBindings();
            dataGridViewPidEditor.Update();
            this.Refresh();
        }

        private void chkPidEditorShowMaster_CheckedChanged(object sender, EventArgs e)
        {
            ReloadPidParams(true, false);
        }

        private void chkPidEditorShowCustom_CheckedChanged(object sender, EventArgs e)
        {
            ReloadPidParams(true, false);
        }

        private void chkPidEditorProfileOnly_CheckedChanged(object sender, EventArgs e)
        {
            ReloadPidParams(true, false);
        }

        private void btnPidEditorTestMasterlistPids_Click(object sender, EventArgs e)
        {
            if (dataGridProfile.SelectedCells.Count == 0)
            {
                return;
            }
            dataGridSelectedPids.Columns["Value"].Visible = true;
            if (datalogger.LogRunning)
            {
                Logger("Queue PID request commands");
                for (int s = 0; s < dataGridProfile.SelectedCells.Count; s++)
                {
                    int row = dataGridProfile.SelectedCells[s].RowIndex;
                    LogParam.PidParameter parm = (LogParam.PidParameter)dataGridProfile.Rows[row].DataBoundItem;
                    LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                    datalogger.QueuePidQuery(pidSettings);
                }
            }
            else
            {
                datalogger.Receiver.SetReceiverPaused(true);
                Connect(radioVPW.Checked, true, true, null);
                for (int s = 0; s < dataGridProfile.SelectedCells.Count; s++)
                {
                    int row = dataGridProfile.SelectedCells[s].RowIndex;
                    LogParam.PidParameter parm = (LogParam.PidParameter)dataGridProfile.Rows[row].DataBoundItem;
                    LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                    datalogger.QueryPid(pidSettings, true);
                }
                datalogger.Receiver.SetReceiverPaused(false);
            }
        }

        private void chkProfileTestPidsTimer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContinuousPidTest.Checked)
            {
                if (!Connect(radioVPW.Checked, true, true, oscript))
                {
                    chkContinuousPidTest.Checked = false;
                    return;
                }
                dataGridSelectedPids.Columns["Value"].Visible = true;
                pidTesterTokenSource = new CancellationTokenSource();
                pidTesterToken = pidTesterTokenSource.Token;
                Task.Factory.StartNew(() => PidTesterLoop(), pidTesterToken);
            }
            else
            {
                pidTesterTokenSource.Cancel();
            }
        }

        private void PidTesterLoop()
        {
            try
            {
                datalogger.Receiver.StopReceiveLoop();
                while (!pidTesterTokenSource.IsCancellationRequested)
                {
                    int row = 0;
                    List<LogParam.PidSettings> pids = new List<LogParam.PidSettings>();
                    pids.AddRange(datalogger.SelectedPids);
                    foreach (LogParam.PidSettings pidProfile in pids) //Crash here, if directly looping datalogger.SelectedPids
                    {
                        string pidVal = datalogger.QueryPid(pidProfile,false);
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            if (dataGridSelectedPids.Rows.Count > row)
                            {
                                dataGridSelectedPids.Rows[row].Cells["Value"].Value = pidVal;
                            }
                            if (dataGridLogData.Rows.Count > row)
                            {
                                dataGridLogData.Rows[row].Cells["Value"].Value = pidVal;
                            }
                        });
                        row++;
                    }
                    Thread.Sleep(100);
                }
                if (!datalogger.LogRunning)
                {
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port, false, datalogger.AnalyzerRunning);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void btnAllSettings_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            UpatcherSettings tmpSettings = AppSettings.ShallowCopy();
            fpe.resetToDefaultValueToolStripMenuItem.Enabled = true;
            fpe.txtFilter.Text = "logger";
            fpe.LoadObject(tmpSettings, "All settings");
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                AppSettings = tmpSettings;
                AppSettings.Save();
                Logger("Settings saved");
            }
        }

        private void UpdateProfileFiles(List<LogParam.PidParameter> parms)
        {
            int updatedProfiles = 0;
            Logger("Updating profiles...");
            string ProfilePath = Path.Combine(Application.StartupPath, "Logger", "Profiles");
            DirectoryInfo d = new DirectoryInfo(ProfilePath);
            FileInfo[] Files;
            Files = d.GetFiles("*.xml", SearchOption.AllDirectories);
            foreach (FileInfo file in Files)
            {
                if (LogParamFile.UpdateProfile(file.FullName, parms))
                {
                    updatedProfiles++;
                }
            }
            Logger("Updated "+updatedProfiles.ToString()+" of "+Files.Length.ToString()+" profiles");
        }
        private void massUpdateSelectedPIDsToAllProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<LogParam.PidParameter> parms = new List<LogParam.PidParameter>();
            List<int> rows = new List<int>();
            for  (int c=0; c< dataGridViewPidEditor.SelectedCells.Count;c++)
            {
                if (!rows.Contains(dataGridViewPidEditor.SelectedCells[c].RowIndex))
                {
                    rows.Add(dataGridViewPidEditor.SelectedCells[c].RowIndex);
                }
            }
            foreach (int row in rows)
            {
                LogParam.PidParameter parm = (LogParam.PidParameter)dataGridViewPidEditor.Rows[row].DataBoundItem;
                parms.Add(parm);
            }
            if (parms.Count == 0)
            {
                Logger("No PIDs selected");
                return;
            }
            UpdateProfileFiles(parms);
        }

        private void massUpdateALLPIDsToAllProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateProfileFiles(datalogger.PidParams);
        }

        private void comboPidEditorDatatype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPidId_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPidEditorImportDBC_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewPidEditor.SelectedCells.Count == 0)
                {
                    return;
                }
                frmSelectDBC fdbc = new frmSelectDBC(null);
                DialogResult result = fdbc.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (DBC.DBCSignal signal in fdbc.SelectedSignals)
                    {
                        LogParam.PidParameter parm = DBC.DBCtoParameter(fdbc.SelectedMsg, signal);
                        datalogger.PidParams.Add(parm);
                    }
                    //ReloadPidParams(true, false);
                }
                else if (result == DialogResult.Yes)
                {
                    foreach (DBC.DBCMsg msg in fdbc.SelectedMsgs)
                    {
                        foreach (DBC.DBCSignal signal in msg.Signals)
                        {
                            LogParam.PidParameter parm = DBC.DBCtoParameter(msg, signal);
                            datalogger.PidParams.Add(parm);
                        }
                    }
                    //ReloadPidParams(true, false);
                }
                else
                {
                    return;
                }
                DisablePidEditorChanges = true;
                pidparams.ResetBindings();
                dataGridViewPidEditor.Update();
                dataGridViewPidEditor.Refresh();
                dataGridViewPidEditor.FirstDisplayedScrollingRowIndex = dataGridViewPidEditor.Rows.Count - 1;
                //dataGridViewPidEditor.CurrentCell = dataGridViewPidEditor.Rows[dataGridViewPidEditor.Rows.Count - 2].Cells[1];
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }

        }

        private void dataGridViewPidEditorConversions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private LogParam.PidParameter FindParmFromMasterList(LogParam.PidParameter parm)
        {
            List<LogParam.PidParameter> masterParms = datalogger.PidParams.Where(X => X.Type ==parm.Type && 
            X.Name == parm.Name && X.Id == parm.Id).ToList();
            foreach(LogParam.PidParameter mParm in masterParms)
            {
                string mParmStr = LogParamFile.ParameterToXml(mParm).ToString();
                string parmStr = LogParamFile.ParameterToXml(parm).ToString();
                if (parmStr == mParmStr)
                {
                    return mParm;
                }
            }
            Debug.WriteLine("PID \"" + parm.Name + "\" not found from master list, adding as custom PID");
            parm.Custom = true;
            datalogger.PidParams.Add(parm);
            return parm;
        }
        private void btnPidProfileImportBdc_Click(object sender, EventArgs e)
        {
            try
            {
                frmSelectDBC fdbc = new frmSelectDBC(null);
                DialogResult result = fdbc.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (DBC.DBCSignal signal in fdbc.SelectedSignals)
                    {
                        LogParam.PidParameter parm = DBC.DBCtoParameter(fdbc.SelectedMsg, signal);
                        parm = FindParmFromMasterList(parm);
                        LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                        datalogger.SelectedPids.Add(pidSettings);
                        AddPidToSelectedPidGrid(pidSettings);
                    }
                    //ReloadPidParams(true, false);
                }
                else if (result == DialogResult.Yes)
                {
                    foreach (DBC.DBCMsg msg in fdbc.SelectedMsgs)
                    {
                        foreach (DBC.DBCSignal signal in msg.Signals)
                        {
                            LogParam.PidParameter parm = DBC.DBCtoParameter(msg, signal);
                            parm = FindParmFromMasterList(parm);
                            LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                            datalogger.SelectedPids.Add(pidSettings);
                            AddPidToSelectedPidGrid(pidSettings);
                        }
                    }
                }
                else
                {
                    return;
                }
                CheckMaxPids();
                SetProfileDirty(true,true);
                //SetupLogDataGrid();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void timerAnalyzerCombined_Tick(object sender, EventArgs e)
        {
            for (int r=0;r< combinedCanDatas.Count; r++)
            {
                CombinedCanData data = combinedCanDatas[r];
                if (dataGridAnalyzer.Rows.Count <= r)
                {
                    dataGridAnalyzer.Rows.Add();
                    dataGridAnalyzer.Rows[r].Tag = data;
                    dataGridAnalyzer.Rows[r].Cells["CanID"].Value = data.CanID;
                    if (PassivePidCanIds.Contains(data.canid))
                    {
                        dataGridAnalyzer.Rows[r].Cells["CanID"].Style.Font = new Font(dataGridAnalyzer.Font, FontStyle.Bold);
                        dataGridAnalyzer.Rows[r].Cells["Data"].Style.Font = new Font(dataGridAnalyzer.Font, FontStyle.Bold);
                        dataGridAnalyzer.Rows[r].Cells["Hz"].Style.Font = new Font(dataGridAnalyzer.Font, FontStyle.Bold);
                        if (PassivePidOrigins.ContainsKey(data.canid))
                        {
                            dataGridAnalyzer.Rows[r].Cells["Origin"].Value = PassivePidOrigins[data.canid];
                        }
                    }
                }
                dataGridAnalyzer.Rows[r].Cells["Data"].Value = data.Data;
                dataGridAnalyzer.Rows[r].Cells["Hz"].Value = data.Hz;
                for (int p = 0; p < PassivePidSettings.Count; p++)
                {
                    LogParam.PidSettings pid = PassivePidSettings[p];
                    if (pid.Parameter.Address == data.canid && pid.Parameter.PassivePid.MsgLength == data.DataBytes.Length)
                    {
                        dataGridPassivePidValues.Rows[p].Visible = true;
                        if (pid.Parameter.PassivePid.NumBits > 32)
                        {
                            dataGridPassivePidValues.Rows[p].Cells["Value"].Value = SlotHandler.ExtractPassivePidText(data.DataBytes, pid.Parameter.PassivePid.BitStartPos, pid.Parameter.PassivePid.NumBits, pid.Parameter.PassivePid.MSB);
                        }
                        else
                        {
                            UInt64 val = SlotHandler.ExtractPassivePidData(data.DataBytes, pid.Parameter.PassivePid.BitStartPos, pid.Parameter.PassivePid.NumBits, pid.Parameter.PassivePid.MSB);
                            pid.LastPassiveRawValue = val;
                            string valStr = pid.Parameter.GetCalculatedStringValue(pid, false);
                            dataGridPassivePidValues.Rows[p].Cells["Value"].Value = valStr;
                        }
                    }
                }
            }
        }

        private void chkJ2534ServerCreateDebugLog_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerJ2534ProcessWriteDebugLog = chkJ2534ServerCreateDebugLog.Checked;
            AppSettings.Save();
        }

        private void numAnalyzerNumMessages_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.AnalyzerNumMessages = (int)numAnalyzerNumMessages.Value;
            AppSettings.Save();
        }

         private void showSignalBitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSelectedPassivePid();
        }

        private void searchSignalFromDBCFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridAnalyzer.SelectedCells.Count == 0 ||
                    dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["CanID"].Value == null ||
                    dataGridAnalyzer.Rows[dataGridAnalyzer.CurrentCell.RowIndex].Cells["Data"].Value == null)
                {
                    return;
                }
                List<CombinedCanData> datas = new List<CombinedCanData>();
                for (int c = 0; c < dataGridAnalyzer.SelectedCells.Count; c++)
                {
                    CombinedCanData data = dataGridAnalyzer.Rows[dataGridAnalyzer.SelectedCells[c].RowIndex].Tag as CombinedCanData;
                    if (!datas.Contains(data))
                    {
                        datas.Add(data);
                    }
                }

                string dbcPath = SelectFolder("Select DBC folder");
                if (dbcPath.Length == 0)
                {
                    return;
                }
                List<DBC.DBCMsg> foundMsgs = new List<DBC.DBCMsg>();
                List<LogParam.PidParameter> parms = new List<LogParam.PidParameter>();
                DirectoryInfo d = new DirectoryInfo(dbcPath);
                FileInfo[] Files = d.GetFiles("*.dbc", SearchOption.AllDirectories);
                foreach (FileInfo file in Files)
                {
                    List<DBC.DBCMsg> msgs = DBC.ReadDbcFile(file.FullName);
                    foreach (CombinedCanData data in datas)
                    {
                        List<DBC.DBCMsg> matchMsgs =  msgs.Where(X => X.CANid == data.canid && X.DataLength == data.DataBytes.Length).ToList();
                        if (matchMsgs != null && matchMsgs.Count > 0)
                        {
                            foundMsgs.AddRange(matchMsgs);
                            foreach (DBC.DBCMsg msg in matchMsgs)
                            {
                                Logger("File: " + file.FullName + ", Message: " + msg.MessageName);
                                foreach (DBC.DBCSignal signal in msg.Signals)
                                {
                                    LogParam.PidParameter parm = DBC.DBCtoParameter(msg, signal);
                                    parms.Add(parm);
                                    if (string.IsNullOrEmpty(signal.Description))
                                    {
                                        Logger("Signal: " + signal.SignalName);
                                    }
                                    else
                                    {
                                        Logger("Signal: " + signal.SignalName + ": " + signal.Description);
                                    }
                                }
                            }
                        }
                    }
                }
                if (parms.Count == 0)
                {
                    Logger("Signals not found");
                }
                else
                {
                    frmSelectDBC fdbc = new frmSelectDBC(foundMsgs);
                    DialogResult result = fdbc.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        foreach (DBC.DBCSignal signal in fdbc.SelectedSignals)
                        {
                            LogParam.PidParameter parm = DBC.DBCtoParameter(fdbc.SelectedMsg, signal);
                            datalogger.PidParams.Add(parm);
                            LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                            pidSettings.Units = parm.Conversions[0];
                            List<LogParam.PidParameter> tmpParms = new List<LogParam.PidParameter>();
                            AddPassivePidtoAnalyzer(pidSettings, ref tmpParms);
                        }
                    }
                    else if (result == DialogResult.Yes)
                    {
                        foreach (DBC.DBCMsg msg in fdbc.SelectedMsgs)
                        {
                            foreach (DBC.DBCSignal signal in msg.Signals)
                            {
                                LogParam.PidParameter parm = DBC.DBCtoParameter(msg, signal);
                                datalogger.PidParams.Add(parm);
                                LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                                pidSettings.Units = parm.Conversions[0];
                                List<LogParam.PidParameter> tmpParms = new List<LogParam.PidParameter>();
                                AddPassivePidtoAnalyzer(pidSettings, ref tmpParms);
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                    DisablePidEditorChanges = true;
                    pidparams.ResetBindings();
                    splitAnalyzer.Panel2Collapsed = false;
                    splitAnalyzer.Panel2.Show();
                    for (int c = 0; c < dataGridAnalyzer.SelectedCells.Count; c++)
                    {
                        int r = dataGridAnalyzer.SelectedCells[c].RowIndex;
                        dataGridAnalyzer.Rows[r].Cells["CanID"].Style.Font = new Font(dataGridAnalyzer.Font, FontStyle.Bold);
                        dataGridAnalyzer.Rows[r].Cells["Data"].Style.Font = new Font(dataGridAnalyzer.Font, FontStyle.Bold);
                        dataGridAnalyzer.Rows[r].Cells["Hz"].Style.Font = new Font(dataGridAnalyzer.Font, FontStyle.Bold);
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
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void chkRestartLogging_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRestartLogging.Checked)
            {
                AppSettings.LoggerRestartAfterSeconds = (int)numRestartLoggingAfter.Value;
            }
            else
            {
                AppSettings.LoggerRestartAfterSeconds = (int)numRestartLoggingAfter.Value * -1;
            }
            AppSettings.Save();
        }

        private void numRestartLoggingAfter_ValueChanged(object sender, EventArgs e)
        {
            if (chkRestartLogging.Checked)
            {
                AppSettings.LoggerRestartAfterSeconds = (int)numRestartLoggingAfter.Value;
            }
            else
            {
                AppSettings.LoggerRestartAfterSeconds = (int)numRestartLoggingAfter.Value * -1;
            }
            AppSettings.Save();

        }

        private void dataGridLogData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void timerAnalyzer_Tick(object sender, EventArgs e)
        {
            if (analyzedRows.Count == 0)
            {
                return;
            }
            try
            {
                DrawingControl.SuspendDrawing(dataGridAnalyzer);
                for (int m = 0; m < 100 && m < analyzedRows.Count; m++)
                {
                    Analyzer.AnalyzerData analyzedRow = analyzedRows.Dequeue();
                    int r = dataGridAnalyzer.Rows.Add();
                    foreach (var prop in analyzedRow.GetType().GetProperties())
                    {
                        dataGridAnalyzer.Rows[r].Cells[prop.Name].Value = prop.GetValue(analyzedRow, null);
                    }
                    dataGridAnalyzer.CurrentCell = dataGridAnalyzer.Rows[r].Cells[0];
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmLogger line " + line + ": " + ex.Message);
            }

            DrawingControl.ResumeDrawing(dataGridAnalyzer);
        }

        private void txtWBCanId_TextChanged(object sender, EventArgs e)
        {
            AppSettings.WBCanID = txtWBCanId.Text;
            RestartWB();
        }

        private void btnWbMoreSettings_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            UpatcherSettings tmpSettings = AppSettings.ShallowCopy();
            fpe.resetToDefaultValueToolStripMenuItem.Enabled = true;
            fpe.txtFilter.Text = "wb";
            fpe.LoadObject(tmpSettings, "WB settings");
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                AppSettings = tmpSettings;
                AppSettings.Save();
                Logger("Settings saved");
            }

        }

        private void btnProfileRefresh_Click(object sender, EventArgs e)
        {
            ReloadPidParams(false, true);
            SetupLogDataGrid();
        }

        private void radioTCPIP_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTCPIP.Checked)
            {
                labelSerialport.Text = "IP:port";
                comboSerialPort.Text = AppSettings.LoggerDeviceUrl;
            }
        }

        private void radioRS232_CheckedChanged(object sender, EventArgs e)
        {
            if (radioRS232.Checked)
            {
                labelSerialport.Text = "Port:";
                comboSerialPort.Text = "";
                LoadPorts(true);
            }
        }

        private void radioFTDI_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFTDI.Checked)
            {
                labelSerialport.Text = "Port:";
                comboSerialPort.Text = "";
                LoadPorts(true);
            }
        }

        private void serialportServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSerialPortServer fsps = new frmSerialPortServer();
            fsps.Show();
        }

        private void chkFloodActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkFloodActive.Checked)
                {
                    byte flByte;
                    if (!HexToByte(txtFloodByte.Text, out flByte))
                    {
                        LoggerBold("Unknown byte: " + txtFloodByte.Text);
                        chkFloodActive.Checked = false;
                        return;
                    }
                    byte[] flBuf = new byte[(int)numFloodByteCount.Value];
                    for (int f=0;f< numFloodByteCount.Value; f++)
                    {
                        flBuf[f] = flByte;
                    }
                    Connect(radioVPW.Checked, false, false, oscript);
                    datalogger.LogDevice.SetWriteTimeout(2000);
                    if (chkFlood4x.Checked)
                    {
                        datalogger.LogDevice.SetVpwSpeed(VpwSpeed.FourX);
                    }
                    else
                    {
                        datalogger.LogDevice.SetVpwSpeed(VpwSpeed.Standard);
                    }
                    Application.DoEvents();
                    floodTestActive = true;
                    Task.Factory.StartNew(() => { floodtest(flBuf); });
                }
                else
                {
                    floodTestActive = false;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                chkFloodActive.Checked = false;
                LoggerBold("Error, frmLogger line " + line + ": " + ex.Message);
            }
        }

        private void floodtest(byte[] flBuf)
        {
            OBDMessage oMsg = new OBDMessage(flBuf);
            while (floodTestActive)
            {
                datalogger.LogDevice.SendMessage(oMsg, 0);
                Thread.Sleep(1);
                Application.DoEvents();
            }

        }

        private void radioLSCan_CheckedChanged(object sender, EventArgs e)
        {
            chkWakeUp.Enabled = radioLSCan.Checked;
            datalogger.SetProtocol(radioVPW.Checked);
            LoadModuleList();
            SetupPidEditor();
        }

        private void chkCombinePids_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void queryCANDevicesMainConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //CANqueryCounter = 0;
                bool RestoreConnection = datalogger.Connected;
                if (!Connect(radioVPW.Checked, false,false,oscript))
                {
                    return;
                }
                datalogger.Receiver.StopReceiveLoop();
                dataGridCANDevices.DataSource = null;
                canDevs = SearchAllDevicesOnBus(datalogger.LogDevice);
                dataGridCANDevices.DataSource = canDevs;
                Application.DoEvents();
                dataGridCANDevices.Columns[1].DefaultCellStyle.Format = "X4";
                dataGridCANDevices.Columns[2].DefaultCellStyle.Format = "X4";
                dataGridCANDevices.Columns[3].DefaultCellStyle.Format = "X4";
                dataGridCANDevices.Columns[4].DefaultCellStyle.Format = "X4";
                if (RestoreConnection)
                {
                    Logger("Restoring original connection");
                    datalogger.LogDevice.PassthruDisconnect();
                    if (!ReConnect())
                    {
                        return;
                    }
                }
                else
                {
                    Disconnect(true);
                }
                Logger("Done");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void btnWBApply_Click(object sender, EventArgs e)
        {
            RestartWB();

        }

        private void tabSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnWbInitCmds_Click(object sender, EventArgs e)
        {
            string wbInitFile = Path.Combine(Application.StartupPath, "XML", "wbinit.txt");
            try
            {
                System.Diagnostics.Process.Start(wbInitFile);
            }
            catch(Exception ex)
            {
                LoggerBold(wbInitFile + ": " + ex.Message);
            }
        }
    }
}
