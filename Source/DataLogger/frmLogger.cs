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

        private BindingSource bsLogProfile = new BindingSource();
        private BindingSource bsParams = new BindingSource();
        //private BindingSource bsPidNames = new BindingSource();
        private List<Parameter> stdParameters = new List<Parameter>();
        private List<RamParameter> ramParameters = new List<RamParameter>();
        private List<MathParameter> mathParameters = new List<MathParameter>();
        private string profileFile;
        private string logfilename;
        private bool ProfileDirty = false;
        //private BindingSource AnalyzeBS = new BindingSource();
        private List<ushort> SupportedPids;
        private PidConfig ClipBrd;
        private int keyDelayCounter = 0;
        public List<J2534DotNet.J2534Device> jDevList;
        //private SelectedTab selectedtab = SelectedTab.Logger;
        bool waiting4x = false;
        bool jConsoleWaiting4x = false;
        JConsole jConsole;
        OBDScript oscript;
        OBDScript joscript;
        //private string jConsoleLogFile;
        StreamWriter jConsoleStream;
        StreamWriter vpwConsoleStream;
        int prevSlotCount = 0;
        int failCount = 0;
        //int CANqueryCounter;
        int canQuietResponses;
        int canDeviceResponses;
        DateTime lastResponseTime;
        List <CANDevice> canDevs;
        private frmLoggerGraphics GraphicsForm;
        private frmHistogram HstForm;
        private ToolTip ScrollTip = new ToolTip();
        public PcmFile PCM;
        private string CurrentPortName;
        private string jConsoleFilters;
        private string jConsoleFilters2;

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
        public List<LogText> logTexts;
        int lastLogRowCount = 0;
        public List<LogText> jconsolelogTexts;
        int jconsolelastLogRowCount = 0;
        Queue<LogText> consoleLogQueue = new Queue<LogText>();
        Queue<LogText> jconsoleLogQueue = new Queue<LogText>();
        ContextMenuStrip rtcMenu = new ContextMenuStrip();

        private void frmLogger_Load(object sender, EventArgs e)
        {
            frmlogger = this;
            logTexts = new List<LogText>();
            jconsolelogTexts = new List<LogText>();
            datalogger = new DataLogger(AppSettings.LoggerUseVPW);
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            if (!string.IsNullOrEmpty(datalogger.OS))
            {
                labelConnected.Text = "Disconnected - OS: " + datalogger.OS;
            }

            //LogReceivers.Add(txtResult);
            Application.DoEvents();
            comboBaudRate.DataSource = SupportedBaudRates;
            LoadSettings();
            LoadStdParams();
            LoadProfileList();
            initPcmResponses();
            this.FormClosing += frmLogger_FormClosing;
            tabLog.Enter += TabLog_Enter;
            tabAnalyzer.Enter += TabAnalyzer_Enter;
            tabProfile.Enter += TabProfile_Enter;
            tabSettings.Enter += TabSettings_Enter;
            tabSettings.Leave += TabSettings_Leave;
            tabDTC.Enter += TabDTC_Enter;
            tabVPWConsole.Enter += TabAdvanced_Enter;

            dataGridLogData.DataError += DataGridLogData_DataError;
            dataGridPidNames.CellContentDoubleClick += DataGridPidNames_CellContentDoubleClick;
            dataGridLogProfile.CellContentDoubleClick += DataGridLogProfile_CellContentDoubleClick;
            dataGridLogProfile.CellEndEdit += DataGridLogProfile_CellEndEdit;
            SerialPortService.PortsChanged += SerialPortService_PortsChanged;
            listProfiles.MouseClick += ListProfiles_MouseClick;
            dataGridLogProfile.DataError += DataGridLogProfile_DataError;
            dataGridLogProfile.DataBindingComplete += DataGridLogProfile_DataBindingComplete;
            dataGridPidNames.DataError += DataGridPidNames_DataError;
            dataGridAnalyzer.DataError += DataGridAnalyzer_DataError;
            comboBaudRate.KeyPress += ComboBaudRate_KeyPress;
            txtParamSearch.Enter += TxtParamSearch_Enter;
            txtParamSearch.Leave += TxtParamSearch_Leave;
            txtParamSearch.KeyPress += TxtParamSearch_KeyPress;
            txtSendBus.KeyPress += TxtSendBus_KeyPress;
            richVPWmessages.EnableContextMenu();
            richJConsole.EnableContextMenu();
            txtJ2534SetPins.Enter += TxtJ2534SetPins_Enter;
            txtJConsoleSend.KeyPress += TxtJConsoleSend_KeyPress;
            chkConsole4x.Enter += ChkConsole4x_Enter;
            chkJConsole4x.Enter += ChkConsole4x_Enter;
            chkVpwBuffered.Checked = true;
            chkJconsoleUsebuffer.Checked = true;
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
            comboFilterByOS.Items.Clear();
            string folder =  Path.Combine(Application.StartupPath, "Logger", "ospids");
            DirectoryInfo d = new DirectoryInfo(folder);
            FileInfo[] Files = d.GetFiles("*.txt", SearchOption.AllDirectories);
            comboFilterByOS.Items.Add("-");
            foreach (FileInfo file in Files)
            {
                string os = file.Name.Replace(".txt", "");
                comboFilterByOS.Items.Add(os);
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

        private void TabAdvanced_Enter(object sender, EventArgs e)
        {
            //selectedtab = SelectedTab.Advanced;
        }

        private void TabDTC_Enter(object sender, EventArgs e)
        {
            //selectedtab = SelectedTab.Dtc;
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

        private void TabSettings_Leave(object sender, EventArgs e)
        {
        }

        private void LogDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                    return;
                this.Invoke((MethodInvoker)delegate () {
                    StringBuilder sMsg = new StringBuilder();
                    if (chkConsoleTimestamps.Checked)
                    {
                        sMsg.Append("[" + new DateTime(e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                    }
                    if (chkVpwDevTimestamps.Checked)
                    {
                        sMsg.Append("[" + e.Msg.DeviceTimestamp.ToString("D20") + "] ");
                    }
                    //sMsg.Append(" Diff: " + new DateTime((long)(e.Msg.TimeStamp - (ulong)e.Msg.DevTimeStamp)).ToString("yyyy MM dd HH:mm:ss.fff") + "| ");
                    sMsg.Append(e.Msg.ToString() + Environment.NewLine);
                    AppendLogText(Color.DarkGreen, sMsg.ToString(), e.Msg.TimeStamp);

                    if (chkVpwToFile.Checked && vpwConsoleStream != null)
                    {
                        vpwConsoleStream.WriteLine("\\cf1 " + sMsg.ToString() + "\\par");
                    }

                    if (e.Msg.Length > 3 && ( serialRadioButton.Checked || comboJ2534Protocol.Text.Contains("VPW")))
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

        private void JDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                    return;

                Application.DoEvents();
                this.Invoke((MethodInvoker)delegate () {
                    StringBuilder logTxt = new StringBuilder();
                    if (chkJConsoleTimestamps.Checked)
                    {
                        logTxt.Append("[" + new DateTime((long)e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                    }
                    if (chkJconsoleDevTimestamps.Checked)
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
                    if (chkJConsoleToFile.Checked && jConsoleStream != null)
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
                    if (e.Msg.Length > 3 &&  comboJ2534Protocol.Text.Contains("VPW"))
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
                    if (timerKeepBusQuiet.Enabled && comboJ2534Protocol.Text.Contains("CAN"))
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

        private void LogDevice_MsgSent(object sender, MsgEventparameter e)
        {
            try
            {
                if (e.Msg == null || e.Msg.Length == 0)
                    return;
                this.Invoke((MethodInvoker)delegate () {
                    StringBuilder logTxt = new StringBuilder();
                    if (chkConsoleTimestamps.Checked)
                    {
                        logTxt.Append("[" + new DateTime((long)e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                    }
                    if (chkVpwDevTimestamps.Checked)
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

                    if (chkVpwToFile.Checked && vpwConsoleStream != null)
                    {
                        vpwConsoleStream.WriteLine("\\cf3 " + logTxt.ToString() + "\\par");
                    }
                });
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

                this.Invoke((MethodInvoker)delegate () {
                    Application.DoEvents();
                    StringBuilder logTxt = new StringBuilder();
                    if (chkJConsoleTimestamps.Checked)
                    {
                        logTxt.Append( "[" + new DateTime((long)e.Msg.TimeStamp).ToString("HH:mm:ss.fff") + "] ");
                    }
                    if (chkJconsoleDevTimestamps.Checked)
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
                    if (chkJConsoleToFile.Checked && jConsoleStream != null)
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


        private void TabSettings_Enter(object sender, EventArgs e)
        {
            //selectedtab = SelectedTab.Settings;
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

        private void TabProfile_Enter(object sender, EventArgs e)
        {
            //selectedtab = SelectedTab.Profile;
        }

        private void TabAnalyzer_Enter(object sender, EventArgs e)
        {
            //selectedtab = SelectedTab.Analyzer;
        }

        private void DataGridAnalyzer_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void ListProfiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (datalogger.LogRunning || timerPlayback.Enabled)
                return;
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

            SelectProfile(Path.Combine(Application.StartupPath,"Logger", "Profiles", listProfiles.Text));            
        }

        private void LoadProfileList()
        {
            listProfiles.Items.Clear();
            string ProfilePath = Path.Combine(Application.StartupPath, "Logger", "Profiles");
            DirectoryInfo d = new DirectoryInfo(ProfilePath);
            FileInfo[] Files;
            Files = d.GetFiles("*.*");
            foreach (FileInfo file in Files)
            {
                listProfiles.Items.Add(file.Name);
            }
            if (!string.IsNullOrEmpty(profileFile) && listProfiles.Items.Contains(profileFile))
            {
                listProfiles.SelectedIndex = listProfiles.Items.IndexOf(profileFile);
            }
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
                        if (chkFTDI.Checked && datalogger.Connected && serialRadioButton.Checked && !comboSerialPort.Items.Contains(CurrentPortName))
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


        private void DataGridLogProfile_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ProfileDirty = true;
            Debug.WriteLine("Dirt");
        }

        private void DataGridLogProfile_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            RemoveFromProfile();
        }

        private void DataGridPidNames_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AddSelectedPidsToProfile();
        }

        private void DataGridLogData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void TabLog_Enter(object sender, EventArgs e)
        {
            //selectedtab = SelectedTab.Logger;
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
                    j2534RadioButton.Checked = AppSettings.LoggerUseJ2534;
                    groupProtocol.Enabled = AppSettings.LoggerUseJ2534;
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
                if (chkFTDI.Checked)
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
                else
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
                                    comboSerialPort.Items.Add(s);
                            }
                            if (LoadDefaults)
                            {
                                if (!string.IsNullOrEmpty(AppSettings.LoggerComPort) && comboSerialPort.Items.Contains(AppSettings.LoggerComPort))
                                    comboSerialPort.Text = AppSettings.LoggerComPort;
                                else
                                    comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                            }
                        }
                        else
                        {
                            comboSerialPort.Text = "";
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
                chkFTDI.Checked = AppSettings.LoggerUseFTDI;
                comboSerialDeviceType.Items.Add(AvtDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(AvtLegacyDevice.DeviceType);
                comboSerialDeviceType.Items.Add(ElmDevice.DeviceType);
                comboSerialDeviceType.Items.Add(JetDevice.DeviceType);
                comboSerialDeviceType.Items.Add(OBDXProDevice.DeviceType);
                comboSerialDeviceType.Text = AppSettings.LoggerSerialDeviceType;

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
                comboJ2534DLL.Text = AppSettings.JConsoleDevice;
                numConsoleScriptDelay.Value = AppSettings.LoggerScriptDelay;
                chkStartJ2534Process.Checked = AppSettings.LoggerStartJ2534Process;
                chkJ2534ServerVisible.Checked = AppSettings.LoggerJ2534ProcessVisible;
                chkAutoDisconnect.Checked = AppSettings.LoggerAutoDisconnect;
                numRetryDelay.Value = AppSettings.RetryWriteDelay;
                numRetryTimes.Value = AppSettings.RetryWriteTimes;
                numResetAfter.Value = AppSettings.LoggerResetAfterMiss;
                radioVPW.Checked = AppSettings.LoggerUseVPW;
                radioCAN.Checked = !AppSettings.LoggerUseVPW;
                txtPcmAddress.Enabled = !AppSettings.LoggerUseVPW;
                txtPcmAddress.Text = AppSettings.LoggerCanPcmAddress.ToString("X4");

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
                chkPriority.Checked = AppSettings.LoggerUsePriority;
                comboBaudRate.Text = AppSettings.LoggerBaudRate.ToString();

                datalogger.PidProfile = new List<PidConfig>();
                if (!string.IsNullOrEmpty(AppSettings.LoggerLastProfile) && File.Exists(AppSettings.LoggerLastProfile))
                {
                    profileFile = AppSettings.LoggerLastProfile;
                    datalogger.LoadProfile(profileFile);
                    this.Text = "Logger - " + profileFile;
                    CheckMaxPids();
                }
                bsLogProfile.DataSource = datalogger.PidProfile;
                dataGridLogProfile.DataSource = bsLogProfile;

                LoadModuleList();

                dataGridAnalyzer.Columns.Clear();
                Analyzer.AnalyzerData ad = new Analyzer.AnalyzerData();
                foreach (var prop in ad.GetType().GetProperties())
                {
                    dataGridAnalyzer.Columns.Add(prop.Name, prop.Name);
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

        private void DataGridPidNames_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //Debug.WriteLine(e.Exception);
        }

        public void LoadStdParams()
        {
            try
            {
                if (chkFilterParamsByOS.Checked && comboFilterByOS.Text != "-" && SupportedPids != null && SupportedPids.Count > 0)
                {
                    Logger("Filtering parameters by OS: " + comboFilterByOS.Text);
                }
                if (stdParameters == null || stdParameters.Count == 0)
                {
                    string fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Standard.xml");
                    XDocument xml = XDocument.Load(fName);
                    stdParameters = new List<Parameter>();
                    int indx = 0;
                    foreach (XElement parameterElement in xml.Root.Elements("Parameter"))
                    {
                        Parameter p = new Parameter();
                        p.Id = parameterElement.Attribute("id").Value;
                        p.Name = parameterElement.Attribute("name").Value;
                        p.Description = parameterElement.Attribute("description").Value;
                        p.DataType = parameterElement.Attribute("storageType").Value;
                        p.index = indx;
                        foreach (XElement conversion in parameterElement.Elements("Conversion"))
                        {
                            string units = conversion.Attribute("units").Value;
                            string expression = conversion.Attribute("expression").Value;
                            string format = conversion.Attribute("format").Value;
                            if (parameterElement.Attribute("bitMapped").Value.ToLower() == "true")
                            {
                                int bitindex = Convert.ToInt32(parameterElement.Attribute("bitIndex").Value);
                                string[] truefalse = expression.Split(',');
                                p.Conversions.Add(new Conversion(units, bitindex, truefalse[0], truefalse[1]));
                            }
                            else
                            {
                                p.Conversions.Add(new Conversion(units, expression, format));
                            }
                        }
                        stdParameters.Add(p);
                        indx++;
                    }
                }

                //dataGridPidNames.DataSource = bsParams;
                //bsParams.DataSource = stdParameters;

                dataGridPidNames.Columns.Clear();
                Parameter tmpPar = new Parameter();
                foreach (var prop in tmpPar.GetType().GetProperties())
                {
                    dataGridPidNames.Columns.Add(prop.Name, prop.Name);
                }

                List<Parameter> results = new List<Parameter>();
                if (txtParamSearch.Text.Length > 0 && txtParamSearch.Text != "Search...")
                    results = stdParameters.Where(x => x.Name.ToLower().Contains(txtParamSearch.Text.ToLower())).ToList();
                else
                    results = stdParameters;

                dataGridPidNames.Columns["index"].Visible = false;
                for (int p = 0; p < results.Count; p++)
                {
                    ushort pid;
                    HexToUshort(results[p].Id.Replace("0x", ""), out pid);
                    if (!chkFilterParamsByOS.Checked || SupportedPids == null || SupportedPids.Contains(pid))
                    {
                        int row = dataGridPidNames.Rows.Add();
                        foreach (var prop in tmpPar.GetType().GetProperties())
                        {
                            if (prop.Name == "Conversions")
                            {
                                DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                                dataGridPidNames.Rows[row].Cells["Conversions"].Tag = results[p].Conversions;
                                dgc.DataSource = results[p].Conversions;
                                dgc.ValueMember = "expression";
                                dgc.DisplayMember = "units";
                                dataGridPidNames.Rows[row].Cells[prop.Name] = dgc;
                                Application.DoEvents();
                                dataGridPidNames.Rows[row].Cells[prop.Name].Value = results[p].Conversions[0].Expression;
                            }
                            else
                            {
                                dataGridPidNames.Rows[row].Cells[prop.Name].Value = prop.GetValue(results[p], null);
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
        private void LoadRamParams()
        {
            try
            {
                if (chkFilterParamsByOS.Checked && comboFilterByOS.Text != "-")
                {
                    Logger("Filtering parameters by OS: " + comboFilterByOS.Text);
                }
                else
                {
                    Logger("Tip: Connect PCM or load BIN; RAM parameters will be filtered by OS");
                }


                if (ramParameters == null || ramParameters.Count == 0)
                {
                    string fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Ram.xml");
                    XDocument xml = XDocument.Load(fName);
                    ramParameters = new List<RamParameter>();
                    int indx = 0;
                    foreach (XElement parameterElement in xml.Root.Elements("RamParameter"))
                    {
                        RamParameter ramP = new RamParameter();
                        ramP.Id = parameterElement.Attribute("id").Value;
                        ramP.Name = parameterElement.Attribute("name").Value;
                        ramP.Description = parameterElement.Attribute("description").Value;
                        ramP.DataType = parameterElement.Attribute("storageType").Value;
                        ramP.index = indx;
                        foreach (XElement conversion in parameterElement.Elements("Conversion"))
                        {
                            string units = conversion.Attribute("units").Value;
                            string expression = conversion.Attribute("expression").Value;
                            string format = conversion.Attribute("format").Value;
                            if (parameterElement.Attribute("bitMapped").Value.ToLower() == "true")
                            {
                                int bitindex = Convert.ToInt32(parameterElement.Attribute("bitIndex").Value);
                                string[] truefalse = expression.Split(',');
                                ramP.Conversions.Add(new Conversion(units, bitindex, truefalse[0], truefalse[1]));
                            }
                            else
                            {
                                ramP.Conversions.Add(new Conversion(units, expression, format));
                            }
                        }
                        foreach (XElement location in parameterElement.Elements("Location"))
                        {
                            Location l = new Location();
                            l.os = location.Attribute("os").Value;
                            l.address = location.Attribute("address").Value;
                            ramP.Locations.Add(l);
                        }
                        ramParameters.Add(ramP);
                        indx++;
                    }
                }

                List<RamParameter> results = new List<RamParameter>();
                if (txtParamSearch.Text.Length > 0 && txtParamSearch.Text != "Search...")
                    results = ramParameters.Where(x => x.Name.ToLower().Contains(txtParamSearch.Text.ToLower())).ToList();
                else
                    results = ramParameters;


                dataGridPidNames.Columns.Clear();
                RamParameter tmpPar = new RamParameter();
                foreach (var prop in tmpPar.GetType().GetProperties())
                {
                    dataGridPidNames.Columns.Add(prop.Name, prop.Name);
                }
                dataGridPidNames.Columns["index"].Visible = false;
                for (int p = 0; p < results.Count; p++)
                {
                    if (chkFilterParamsByOS.Checked && !string.IsNullOrEmpty(datalogger.OS))
                    {
                        bool supported = false;
                        for (int l=0;l< results[p].Locations.Count;l++)
                        {
                            if (results[p].Locations[l].os == datalogger.OS)
                            {
                                supported = true;
                                break;
                            }
                        }
                        if (!supported)
                        {
                            continue;
                        }
                    }

                    int row = dataGridPidNames.Rows.Add();
                    foreach (var prop in tmpPar.GetType().GetProperties())
                    {
                        if (prop.Name == "Conversions")
                        {
                            DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                            dataGridPidNames.Rows[row].Cells["Conversions"].Tag = results[p].Conversions;
                            dgc.DataSource = results[p].Conversions;
                            dgc.ValueMember = "expression";
                            dgc.DisplayMember = "units";
                            dataGridPidNames.Rows[row].Cells[prop.Name] = dgc;
                            Application.DoEvents();
                            dataGridPidNames.Rows[row].Cells[prop.Name].Value = results[p].Conversions[0].Expression;
                        }
                        else if (prop.Name == "Locations")
                        {
                            DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                            dataGridPidNames.Rows[row].Cells[prop.Name].Tag = results[p].Locations;
                            dgc.DataSource = results[p].Locations;
                            dgc.ValueMember = "address";
                            dgc.DisplayMember = "os";
                            dataGridPidNames.Rows[row].Cells[prop.Name] = dgc;
                            Application.DoEvents();
                            if (!string.IsNullOrWhiteSpace(datalogger.OS))
                            {
                                Location l = results[p].Locations.Where(x => x.os == datalogger.OS).FirstOrDefault();
                                if (l != null)
                                {
                                    dataGridPidNames.Rows[row].Cells[prop.Name].Value = l.address;
                                }
                            }
                        }
                        else
                        {
                            dataGridPidNames.Rows[row].Cells[prop.Name].Value = prop.GetValue(results[p], null);
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

        private void LoadMathParams()
        {
            try
            {
                if (mathParameters == null || mathParameters.Count == 0)
                {
                    string fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Math.xml");
                    XDocument xml = XDocument.Load(fName);
                    mathParameters = new List<MathParameter>();
                    int indx = 0;
                    foreach (XElement parameterElement in xml.Root.Elements("MathParameter"))
                    {
                        MathParameter mathP = new MathParameter();
                        mathP.Id = parameterElement.Attribute("id").Value;
                        mathP.Name = parameterElement.Attribute("name").Value;
                        mathP.Description = parameterElement.Attribute("description").Value;
                        mathP.xParameterId = parameterElement.Attribute("xParameterId").Value;
                        mathP.yParameterId = parameterElement.Attribute("yParameterId").Value;
                        mathP.xDataType = stdParameters.Where(x => x.Id == mathP.xParameterId).FirstOrDefault().DataType;
                        mathP.yDataType = stdParameters.Where(y => y.Id == mathP.yParameterId).FirstOrDefault().DataType;
                        mathP.index = indx;
                        foreach (XElement conversion in parameterElement.Elements("Conversion"))
                        {
                            string units = conversion.Attribute("units").Value;
                            string expression = conversion.Attribute("expression").Value;
                            string format = conversion.Attribute("format").Value;
                            mathP.Conversions.Add(new Conversion(units, expression, format));
                        }
                        mathParameters.Add(mathP);
                        indx++;
                    }
                }

                List<MathParameter> results = new List<MathParameter>();
                if (txtParamSearch.Text.Length > 0 && txtParamSearch.Text != "Search...")
                    results = mathParameters.Where(x => x.Name.ToLower().Contains(txtParamSearch.Text.ToLower())).ToList();
                else
                    results = mathParameters;

                dataGridPidNames.Columns.Clear();
                MathParameter tmpPar = new MathParameter();
                foreach (var prop in tmpPar.GetType().GetProperties())
                {
                    dataGridPidNames.Columns.Add(prop.Name, prop.Name);
                }
                dataGridPidNames.Columns["index"].Visible = false;
                for (int p = 0; p < results.Count; p++)
                {
                    ushort pid;
                    HexToUshort(results[p].xParameterId.Replace("0x", ""), out pid);
                    ushort pid2;
                    HexToUshort(results[p].yParameterId.Replace("0x", ""), out pid2);
                    if (SupportedPids == null || (SupportedPids.Contains(pid) && SupportedPids.Contains(pid2)))
                    {
                        int row = dataGridPidNames.Rows.Add();
                        foreach (var prop in tmpPar.GetType().GetProperties())
                        {
                            if (prop.Name == "Conversions")
                            {
                                DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                                dataGridPidNames.Rows[row].Cells["Conversions"].Tag = results[p].Conversions;
                                dgc.DataSource = results[p].Conversions;
                                dgc.ValueMember = "expression";
                                dgc.DisplayMember = "units";
                                dataGridPidNames.Rows[row].Cells[prop.Name] = dgc;
                                Application.DoEvents();
                                dataGridPidNames.Rows[row].Cells[prop.Name].Value = results[p].Conversions[0].Expression;
                            }
                            else
                            {
                                dataGridPidNames.Rows[row].Cells[prop.Name].Value = prop.GetValue(results[p], null);
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

        private void DataGridPidNames_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                dataGridPidNames.Columns[0].Width = 70;
                dataGridPidNames.Columns[1].Width = 150;
                //dataGridPidNames.Columns[0].DefaultCellStyle.Format = "X4";
                //UseComboBoxForEnums(dataGridPidNames);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGridLogProfile_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                dataGridLogProfile.Columns[0].Width = 180;
                for (int c=1;c < dataGridLogProfile.Columns.Count; c++)
                    dataGridLogProfile.Columns[c].Width = 70;
                UseComboBoxForEnums(dataGridLogProfile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGridLogProfile_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //Debug.WriteLine(e.Exception);
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
                    Disconnect(true);
                }
                if (jConsole != null && jConsole.JDevice != null && jConsole.Connected)
                {
                    DisconnectJConsole(true);
                }
                datalogger.stopLogLoop = true;
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

        private void RestartLogging()
        {
            timerShowData.Enabled = false;
            StopLogging(true, false);
            StartLogging(true);
            failCount = 0;
            timerShowData.Enabled = true;
        }

        private void ResetProfile()
        {
            timerShowData.Enabled = false;
            StopLogging(false, false);
            StartLogging(false);
            failCount = 0;
            timerShowData.Enabled = true;
        }

        private void timerShowData_Tick(object sender, EventArgs e)
        {
            try
            {
                string[] LastCalcValues;
                if (chkRawValues.Checked)
                {
                    LastCalcValues = new string[datalogger.slothandler.LastPidValues.Length];
                    for (int c = 0; c < LastCalcValues.Length; c++)
                        LastCalcValues[c] = datalogger.slothandler.LastPidValues[c].ToString(); ;
                }
                else
                {
                    LastCalcValues = datalogger.slothandler.CalculatePidValues(datalogger.slothandler.LastPidValues);
                }
                for (int row=0; row< LastCalcValues.Length;row++)
                {
                    dataGridLogData.Rows[row].Cells["Value"].Value = LastCalcValues[row];
                }

                TimeSpan elapsed = DateTime.Now.Subtract(datalogger.LogStartTime);
                int speed = (int)(datalogger.slothandler.ReceivedHPRows / elapsed.TotalSeconds);
                int lpSpeed = (int)(datalogger.slothandler.ReceivedLPRows / elapsed.TotalSeconds);
                string elapsedStr = elapsed.Hours.ToString() + ":" + elapsed.Minutes.ToString() + ":" + elapsed.Seconds.ToString();
                if (chkPriority.Checked)
                {
                    labelProgress.Text = "Elapsed: " + elapsedStr + ", Received:" +Environment.NewLine +  
                        "High Priority: " + datalogger.slothandler.ReceivedHPRows.ToString() + " rows (" + speed.ToString() + " /s)" + Environment.NewLine +
                        "Low priority: " + datalogger.slothandler.ReceivedLPRows.ToString() +" rows (" + lpSpeed.ToString() + " /s)";
                }
                else
                {
                    labelProgress.Text = "Elapsed: " + elapsedStr + ", Received: " + datalogger.slothandler.ReceivedHPRows.ToString() + " rows (" + speed.ToString() + " /s)";
                }
                if (prevSlotCount == datalogger.slothandler.ReceivedHPRows)
                {
                    failCount++;  
                    if (failCount > numResetAfter.Value)
                    {
                        RestartLogging();
                    }
                }
                else
                {
                    failCount = 0;
                }
                prevSlotCount = datalogger.slothandler.ReceivedHPRows;
                //(" + ReceivedBytes.ToString() +")";
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

        private void SelectProfile(string fname)
        {
            string defname = Path.Combine(Application.StartupPath, "Logger", "Profiles", "profile1.xml");
            if (!string.IsNullOrEmpty(profileFile) && File.Exists(profileFile))
                defname = profileFile;
            if (string.IsNullOrEmpty(fname))
            {
                fname = SelectFile("Select profile file", XmlFilter, defname);
            }
            if (fname.Length == 0)
                return;
            profileFile = fname;
            this.Text = "Logger - " + profileFile;
            datalogger.LoadProfile(fname);
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.SetupLiveGraphics();
            }
            bsLogProfile.DataSource = datalogger.PidProfile;
            dataGridLogProfile.DataSource = bsLogProfile;
            dataGridLogData.Rows.Clear();
            SetupLogDataGrid();
            ProfileDirty = false;
            CheckMaxPids();
            UpdateProfile();
        }

        private void SelectSaveProfile(string fname = "")
        {
            string defname = Path.Combine(Application.StartupPath, "Logger", "Profiles","profile1.xml");
            if (!string.IsNullOrEmpty(profileFile))
                defname = profileFile;
            if (fname.Length == 0)
                fname = SelectSaveFile(XmlFilter, defname);
            if (fname.Length == 0)
                return;
            profileFile = fname;
            this.Text = "Logger - " + profileFile;
            datalogger.SaveProfile(fname);
            ProfileDirty = false;
            LoadProfileList();
        }

        private void loadProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectProfile("");
        }

        private void saveProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectSaveProfile(profileFile);
        }

        private void CheckMaxPids()
        {
            List<int> pids = new List<int>();
            int bytesTotal = 0;
            int maxBytes = 48;
            for (int i=0; i< datalogger.PidProfile.Count; i++)
            {
                if (!pids.Contains(datalogger.PidProfile[i].addr))
                {
                    pids.Add(datalogger.PidProfile[i].addr);
                    bytesTotal += GetElementSize((InDataType)datalogger.PidProfile[i].DataType);
                }
            }
            if (radioCAN.Checked)
            {
                maxBytes = 128;
            }
            labelProgress.Text = "Profile size: " + bytesTotal.ToString() + " bytes, Maximum: " + maxBytes.ToString();
            if (bytesTotal > maxBytes)
            {
                LoggerBold("Profile have total: " + bytesTotal.ToString() + " bytes, maximum is "+maxBytes.ToString()+" bytes");
            }
        }

        private void AddToProfile(PidConfig pc)
        {
            try
            {
                if (datalogger.Connected && datalogger.LogRunning == false)
                {
                    QueryPid(pc);
                }
                datalogger.PidProfile.Add(pc);
                CheckMaxPids();
                ProfileDirty = true;
                //bsLogProfile.DataSource = null;
                //bsLogProfile.DataSource = datalogger.PidProfile;
                bsLogProfile.ResetBindings(false);
                dataGridLogProfile.Update();
                this.Refresh();
                UpdateProfile();
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
        private void AddSelectedPidsToProfile()
        {
            if (!datalogger.Connected)
            {
                Logger("Not connected - adding pids without testing compatibility");
            }
            List<PidConfig> pds = ConvertSelectedPidConfigs();
            datalogger.Receiver.SetReceiverPaused(true);
            foreach (PidConfig pc in pds)
            {
                AddToProfile(pc);
                Application.DoEvents();
            }
            if (datalogger.LogRunning)
            {
                ResetProfile();
            }
            else
            {
                datalogger.Receiver.SetReceiverPaused(false);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelectedPidsToProfile();
        }

        private void SetupLogDataGrid()
        {
            if (datalogger.LogRunning || timerPlayback.Enabled)
                return; //Dont change settings while logging or playback
            dataGridLogData.Columns.Clear();
            dataGridLogData.Columns.Add("Value", "Value");
            if (!chkRawValues.Checked)
                dataGridLogData.Columns.Add("Units", "Units");
            dataGridLogData.Columns[0].ReadOnly = true;
            for (int r=0;r< datalogger.PidProfile.Count; r++)
            {
                if (chkRawValues.Checked)
                {
                    bool exist = false;
                    for (int row = 0; row < dataGridLogData.Rows.Count - 1; row++)
                    {
                        if (dataGridLogData.Rows[row].HeaderCell.Value.ToString() == datalogger.PidProfile[r].Address)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        int ind = dataGridLogData.Rows.Add();
                        dataGridLogData.Rows[ind].HeaderCell.Value = datalogger.PidProfile[r].Address;
                    }
                }
                else
                {
                    int ind = dataGridLogData.Rows.Add();
                    dataGridLogData.Rows[ind].HeaderCell.Value = datalogger.PidProfile[r].PidName;
                    dataGridLogData.Rows[ind].Cells["Units"].Value = datalogger.PidProfile[r].Units;
                }
            }
            dataGridLogData.AutoResizeColumns();
            dataGridLogData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

        }
        private void SaveSettings()
        {
            if (chkFTDI.Checked)
                AppSettings.LoggerFTDIPort = comboSerialPort.Text;
            else
                AppSettings.LoggerComPort = comboSerialPort.Text;
            AppSettings.LoggerUseJ2534 = j2534RadioButton.Checked;
            AppSettings.LoggerJ2534Device = j2534DeviceList.Text;
            AppSettings.LoggerSerialDeviceType = comboSerialDeviceType.Text;
            AppSettings.LoggerBaudRate = Convert.ToInt32(comboBaudRate.Text);

            if (txtLogFolder.Text.Length > 0)
            {
                AppSettings.LoggerLogFolder = txtLogFolder.Text;
                AppSettings.LoggerLogSeparator = txtLogSeparator.Text;
            }
            AppSettings.LoggerLastProfile = profileFile;
            AppSettings.LoggerResponseMode = comboResponseMode.Text;
            AppSettings.LoggerUseFTDI = chkFTDI.Checked;
            //AppSettings.LoggerShowAdvanced = chkAdvanced.Checked;
            AppSettings.LoggerUsePriority = chkPriority.Checked;
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
            AppSettings.LoggerUseVPW = radioVPW.Checked;
            AppSettings.LoggerResetAfterMiss = (int)numResetAfter.Value;
            if (HexToUshort(txtPcmAddress.Text, out ushort pcmaddr))
                AppSettings.LoggerCanPcmAddress = pcmaddr;
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
                Application.DoEvents();
                datalogger.StopLogging();
                btnStartStop.Text = "Start Logging";
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

                datalogger.VPWProtocol = radioVPW.Checked;
                if (datalogger.Connected)
                {
                    return true;
                }
                if (UseVPW)
                {
                    Logger("Connecting (VPW)...");
                    datalogger.useVPWFilters = chkVPWFilters.Checked;
                }
                else
                {
                    Logger("Connecting (CAN)...");
                    datalogger.useVPWFilters = false;

                    if (!HexToUshort(txtPcmAddress.Text, out datalogger.CanPcmAddr))
                    {
                        datalogger.CanPcmAddr = AppSettings.LoggerCanPcmAddress;
                    }
                    datalogger.CanPcmAddrByte1 = (byte)(datalogger.CanPcmAddr >> 8);
                    datalogger.CanPcmAddrByte2 = (byte)datalogger.CanPcmAddr;
                }
                Application.DoEvents();
                if (serialRadioButton.Checked)
                {
                    CurrentPortName = comboSerialPort.Text;
                    string sPort = comboSerialPort.Text;
                    string[] sParts = sPort.Split(':');
                    if (sParts.Length > 1)
                        sPort = sParts[0].Trim();
                    datalogger.LogDevice = datalogger.CreateSerialDevice(sPort, comboSerialDeviceType.Text, chkFTDI.Checked);
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
                }
                else
                {
                    jParams = new J2534InitParameters();
                    jParams.SeparateProtoByChannel = true;
                    jParams.Protocol = ProtocolID.ISO15765;
                    jParams.Baudrate = "ISO15765_500000";
                    jParams.Sconfigs = "CAN_MIXED_FORMAT=1";
                    CANDevice cd = CANQuery.GetDeviceAddresses(datalogger.CanPcmAddr);
                    jParams.PassFilters = "Type:FLOW_CONTROL_FILTER,Name:CANloggerFlow" + Environment.NewLine;
                    jParams.PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "Pattern:0000"+cd.ResID.ToString("X4")+",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "FlowControl:0000"+cd.RequestID.ToString("X4")+",RxStatus:NONE,TxFlags:NONE" + Environment.NewLine;
                }
                if (!datalogger.LogDevice.Initialize(Convert.ToInt32(comboBaudRate.Text), jParams))
                {
                    if (datalogger.port != null)
                    {
                        datalogger.port.ClosePort();
                        datalogger.port.Dispose();
                    }
                    datalogger.LogDevice.Dispose();
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
                datalogger.TestedPids = new List<int>();

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
                    jParams.Protocol = ProtocolID.CAN;                    
                    jParams.Baudrate = "CAN_500000";
                    jParams.Secondary = true;
                    jParams.SeparateProtoByChannel = true;
                    jParams.UsePrimaryChannel = true;
                    CANDevice cd = CANQuery.GetDeviceAddresses(datalogger.CanPcmAddr);
                    jParams.PassFilters = "Type:PASS_FILTER,Name:CANloggerPass" + Environment.NewLine;
                    jParams.PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    jParams.PassFilters += "Pattern:0000"+cd.DiagID.ToString("X4")+",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                    datalogger.LogDevice.ConnectSecondaryProtocol(jParams);
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
                btnConnect.Text = "Disconnect";
                datalogger.Connected = true;
                SaveSettings();
                if (radioParamRam.Checked)
                {
                    LoadParameters();
                }
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
            initParameters.Baudrate = comboJ2534Baudrate.Text;
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
            initParameters.Baudrate = comboJ2534Baudrate2.Text;
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
            comboJ2534Baudrate.Text = initParameters.Baudrate;
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
            comboJ2534Baudrate2.Text = initParameters.Baudrate;
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
                    LoadJ2534InitParameters(initParameters);
                }
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
                LoggerBold("Connection failed. Check settings");
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private void StartLogging(bool ReConnect)
        {
            try
            {
                Logger("Start logging...");
                AppSettings.LoggerTimestampFormat = txtTstampFormat.Text;
                AppSettings.LoggerDecimalSeparator = txtDecimalSeparator.Text;
                AppSettings.LoggerLogSeparator = txtLogSeparator.Text;
                AppSettings.Save();
                labelProgress.Text = "";
                labelTimeStamp.Text = "-";
                if (datalogger.PidProfile.Count == 0 || datalogger.PidProfile[0].addr == 0xffffff)
                {
                    Logger("No profile configured");
                    return;
                }
                if (!Connect(radioVPW.Checked, false,true, null))
                {
                    return;
                }
                datalogger.RemoveUnsupportedPids();
                if (datalogger.PidProfile.Count == 0)
                {
                    LoggerBold("No compatible pids configured");
                    return;
                }
                if (GraphicsForm != null && GraphicsForm.Visible)
                {
                    GraphicsForm.StartLiveUpdate();
                }
                SetupLogDataGrid();
                datalogger.Responsetype = Convert.ToByte(Enum.Parse(typeof(ResponseTypes), comboResponseMode.Text));
                datalogger.writelog = chkWriteLog.Checked;
                datalogger.useRawValues = chkRawValues.Checked;
                datalogger.reverseSlotNumbers = chkReverseSlotNumbers.Checked;
                datalogger.HighPriority = chkPriority.Checked;
                //PcmLogger.HighPriorityWeight = (int)numHighPriorityWeight.Value;
                if (chkWriteLog.Checked)
                {
                    if (!ReConnect)
                    {
                        datalogger.LogStartTime = DateTime.Now;
                        logfilename = Path.Combine(txtLogFolder.Text, "log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm.ss") + ".csv");
                        AppSettings.LoggerLastLogfile = logfilename;
                        AppSettings.Save();
                    }
                    if (!datalogger.CreateLog(logfilename))
                    {
                        return;
                    }
                }
                if (datalogger.StartLogging(radioVPW.Checked, ReConnect))
                {
                    timerShowData.Enabled = true;
                    btnStartStop.Text = "Stop Logging";
                    groupLogSettings.Enabled = false;
                    //btnGetVINCode.Enabled = false;
                    datalogger.LogRunning = true;
                    if (datalogger.LogDevice.LogDeviceType == LoggingDevType.Elm)
                    {
                        //groupDTC.Enabled = false;
                    }
                }
                else
                {
                    datalogger.LogRunning = false;
                    groupLogSettings.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            btnStartStop.Enabled = false;
            if (datalogger.LogRunning)
            {
                StopLogging(false, true);
            }
            else
            {
                StartLogging(false);
            }
            btnStartStop.Enabled = true;
        }

        private void comboSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboSerialPort.Text))
            {
                btnStartStop.Enabled = true;
            }
        }

        private void RemoveFromProfile()
        {
            try
            {
                List<int> removeInd = new List<int>();
                if (dataGridLogProfile.SelectedRows.Count > 0)
                {
                    for (int r = 0; r < dataGridLogProfile.SelectedRows.Count; r++)
                    {
                        removeInd.Add(dataGridLogProfile.SelectedRows[r].Index);
                    }
                }
                else
                {
                    if (dataGridLogProfile.SelectedCells.Count == 0)
                    {
                        return;
                    }
                    for (int c = 0; c < dataGridLogProfile.SelectedCells.Count; c++)
                    {
                        int row = dataGridLogProfile.SelectedCells[c].RowIndex;
                        if (!removeInd.Contains(row))
                        {
                            removeInd.Add(row);
                        }
                    }
                }

                removeInd.Sort();
                removeInd.Reverse();
                for (int i = 0; i < removeInd.Count; i++)
                {
                    datalogger.PidProfile.RemoveAt(removeInd[i]);
                }
                ProfileDirty = true;
                bsLogProfile.DataSource = null;
                bsLogProfile.DataSource = datalogger.PidProfile;
                CheckMaxPids();
                if (datalogger.LogRunning)
                {
                    ResetProfile();
                }
                if (HstForm != null && HstForm.Visible)
                {
                    HstForm.SetupLiveParameters();
                }
                if (GraphicsForm != null && GraphicsForm.Visible)
                {
                    GraphicsForm.SetupLiveGraphics();
                }

            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveFromProfile();
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
            if (serialRadioButton.Checked)
            {
                radioVPW.Checked = true;
                serialOptionsGroupBox.Enabled = true;
                j2534OptionsGroupBox.Enabled = false;
                groupProtocol.Enabled = false;
                //groupJ2534Options.Visible = false;
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
            if (comboSerialDeviceType.Text== ScanToolDeviceImplementation.DeviceType 
                || comboSerialDeviceType.Text == LegacyElmDeviceImplementation.DeviceType )
            {
                //comboResponseMode.Text = "SendOnce";
                //comboResponseMode.Enabled = false;
            }
            else if (comboSerialDeviceType.Text == AvtDevice.DeviceType)
            {
                comboBaudRate.Text = "57600";
            }
            else
            {
                comboResponseMode.Enabled = true;
                comboResponseMode.Text = "SendFast";
            }

        }

        private void chkFTDI_CheckedChanged(object sender, EventArgs e)
        {
            comboSerialPort.Text = "";
            LoadPorts(true);
        }

/*        private void chkAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAdvanced.Checked)
            {
                groupAdvanced.Visible = true;
                txtLogSeparator.Visible = true;
                labelSeparator.Visible = true;
            }
            else
            {
                groupAdvanced.Visible = false;
                txtLogSeparator.Visible = false;
                labelSeparator.Visible = false;
            }
        }
*/
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

        private void StartAnalyzer()
        {
            try
            {
                if (!Connect(radioVPW.Checked, false, true, null))
                {
                    return;
                }
                analyzer = new Analyzer();
                analyzer.RowAnalyzed += Analyzer_RowAnalyzed;
                string devtype = comboSerialDeviceType.Text;
                if (j2534RadioButton.Checked)
                    devtype = "J2534 Device";
                analyzer.StartAnalyzer(devtype, chkHideHeartBeat.Checked);
                btnStartStopAnalyzer.Text = "Stop Analyzer";
                datalogger.AnalyzerRunning = true;
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

        private void Analyzer_RowAnalyzed(object sender, Analyzer.AnalyzerData e)
        {
            this.Invoke((MethodInvoker)delegate () //Event handler, can't directly handle UI
            {
                int r = dataGridAnalyzer.Rows.Add();
                foreach (var prop in e.GetType().GetProperties())
                {
                    if (prop.Name == "Timestamp")
                    {
                        dataGridAnalyzer.Rows[r].Cells[prop.Name].Value = new DateTime(Convert.ToInt64(prop.GetValue(e, null))).ToString("HH:mm:ss:fff");
                    }
                    else
                    {
                        dataGridAnalyzer.Rows[r].Cells[prop.Name].Value = prop.GetValue(e, null);
                    }
                }
                dataGridAnalyzer.CurrentCell = dataGridAnalyzer.Rows[r].Cells[0];
                //AddAnalyzerRowToGrid(e); 
            });
        }

        private void StopAnalyzer(bool disconnect)
        {
            try
            {
                analyzer.StopAnalyzer();
                btnStartStopAnalyzer.Text = "Start Analyzer";
                datalogger.AnalyzerRunning = false;
                groupDTC.Enabled = true;
                //if (disconnect || !chkConsoleAutorefresh.Checked)
                {
                    //datalogger.Receiver.StopReceiveLoop();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void FilterPidsByBin(PcmFile PCM)
        {
            datalogger.OS = PCM.OS;
            PidSearch pidSearch = new PidSearch(PCM);
            if (pidSearch.pidList != null && pidSearch.pidList.Count > 0)
            {
                SupportedPids = new List<ushort>();
                for (int s = 0; s < pidSearch.pidList.Count; s++)
                {
                    SupportedPids.Add(pidSearch.pidList[s].pidNumberInt);
                }
                LoadOsPidFiles();
                LoadParameters();
                comboFilterByOS.Text = PCM.OS;
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
/*            if (comboResponseMode.Text != "SendOnce")
            {
                chkPriority.Checked = false;
            }
*/  
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
            ushort module;
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
                    module = DeviceId.Broadcast;
                    //codelist = datalogger.RequestDTCCodes(module, mode);
                }
                else
                {
                    module = Convert.ToByte(comboModule.SelectedValue);
                    //codelist = datalogger.RequestDTCCodes(module, mode); 
                }
            }
            else
            {
                module = Convert.ToUInt16(comboModule.SelectedValue);
                if (mode == 2)
                {
                    mode = 0xa9;
                }
            }
            if (datalogger.LogRunning)
            {
                datalogger.QueueDtcRequest(module, mode);
            }
            else
            {
                datalogger.RequestDTCCodes(module, mode);
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
                            DTCCodeStatus dcs = datalogger.DecodeDTCstatus(e.Msg.GetBytes());
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
                        //00 00 05 E8 81 00 00 00 FF 00 00 00
                        ushort module = Convert.ToUInt16(comboModule.SelectedValue);
                        if (datalogger.ValidateQueryResponse(e.Msg, module) && e.Msg[4] == 0x81)
                        {
                            int r = dataGridDtcCodes.Rows.Add();
                            dataGridDtcCodes.Rows[r].Cells[0].Value = module.ToString("X4");
                            string data = "";
                            for (int i = 5; i < e.Msg.Length; i++)
                                data += e.Msg[i].ToString("X2") + " ";

                            dataGridDtcCodes.Rows[r].Cells[1].Value = data;
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
            if (radioCAN.Checked)
                getDtcCodes(0x1a);//0x1a = Current data
            else
                getDtcCodes(2);//2 = Current data
        }

        private void btnHistoryDTC_Click(object sender, EventArgs e)
        {
            getDtcCodes(0x10);
        }

        private void btnClearCodes_Click(object sender, EventArgs e)
        {
            if (radioCAN.Checked)
            {
                LoggerBold("Not implemented for CAN");
                //return;
            }
            Connect(radioVPW.Checked, true, true, null);

            ushort module = (ushort)comboModule.SelectedValue;
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

        private List<PidConfig> ConvertSelectedPidConfigs()
        {
            List<PidConfig> retVal = new List<PidConfig>();
            try
            {
                if (dataGridPidNames.SelectedRows.Count == 0)
                    return null;

                if (!datalogger.Connected)
                {
                    Logger("Not connected - adding pids without testing compatibility");
                }

                for (int selRow = 0; selRow < dataGridPidNames.SelectedRows.Count; selRow++)
                {
                    DataGridViewRow dgr = dataGridPidNames.SelectedRows[selRow];
                    int idx = Convert.ToInt32(dgr.Cells["index"].Value);
                    PidConfig pc = new PidConfig();
                    pc.PidName = dgr.Cells["Name"].Value.ToString();
                    ushort pidNr;
                    ushort pid2Nr;
                    if (radioParamMath.Checked)
                    {
                        MathParameter mp = mathParameters[idx];
                        pc.Type = DefineBy.Math;
                        pc.Math = dgr.Cells["Conversions"].Value.ToString();
                        if (HexToUshort(mp.xParameterId, out pidNr))
                            pc.addr = pidNr;
                        if (HexToUshort(mp.yParameterId, out pid2Nr))
                            pc.addr2 = pid2Nr;
                        PidDataType xpd = (PidDataType)Enum.Parse(typeof(PidDataType), mp.xDataType);
                        pc.DataType = ConvertToDataType(xpd);
                        PidDataType ypd = (PidDataType)Enum.Parse(typeof(PidDataType), mp.yDataType);
                        pc.Pid2DataType = ConvertToDataType(ypd);
                    }
                    else if (radioParamRam.Checked)
                    {
                        RamParameter rp = ramParameters[idx];
                        pc.addr = -1;
                        pc.Type = DefineBy.Address;
                        pc.Math = dgr.Cells["Conversions"].Value.ToString();
                        PidDataType pd = (PidDataType)Enum.Parse(typeof(PidDataType), rp.DataType);
                        pc.DataType = ConvertToDataType(pd);
                        if (string.IsNullOrEmpty(dgr.Cells["Locations"].FormattedValue.ToString()))
                        {
                            if (datalogger.Connected && !string.IsNullOrEmpty(datalogger.OS))
                            {
                                List<Location> locations = rp.Locations;
                                if (locations != null)
                                {
                                    Location l = locations.Where(x => x.os == datalogger.OS).FirstOrDefault();
                                    if (l != null)
                                    {
                                        pc.Address = l.address;
                                    }
                                }
                            }
                        }
                        else
                        {
                            pc.Address = dgr.Cells["Locations"].Value.ToString();
                        }
                    }
                    else if (radioParamStd.Checked)
                    {
                        Parameter sp = stdParameters[idx];
                        pc.Type = DefineBy.Pid;
                        if (HexToUshort(sp.Id, out pidNr))
                            pc.addr = pidNr;
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
                            pc.Math = dgr.Cells["Conversions"].Value.ToString();
                        }
                    }
                    pc.Units = dgr.Cells["Conversions"].FormattedValue.ToString();

                    retVal.Add(pc);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return retVal;
        }

        private bool QueryPid(PidConfig pc)
        {
            try
            {
                Connect(radioVPW.Checked,true,true, null);

                ReadValue rv;
                ReadValue rv2 = new ReadValue();
                //rv = QueryRAM(pc.addr, pc.DataType);
                rv = datalogger.QuerySinglePidValue(pc.addr, pc.DataType);
                if (rv.FailureCode != 0) 
                {
                    return false;
                }
                if (pc.addr2 > 0)
                {
                    rv2 = datalogger.QuerySinglePidValue(pc.addr2, pc.Pid2DataType);
                }
                if (rv.PidValue > double.MinValue)
                {
                    string calcVal = pc.GetCalculatedValue(rv.PidValue, rv2.PidValue);
                    Logger(pc.PidName + ": " + calcVal + " " + pc.Units);
                    Application.DoEvents();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return false;
        }

        private void btnQueryPid_Click(object sender, EventArgs e)
        {
            Connect(radioVPW.Checked, true,true, null);
            List<PidConfig> pds = ConvertSelectedPidConfigs();
            datalogger.Receiver.SetReceiverPaused(true);
            foreach (PidConfig pc in pds)
            {
                QueryPid(pc);
            }
            datalogger.Receiver.SetReceiverPaused(false);
        }

        public void Disconnect(bool StopScript)
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
                StopAnalyzer(true);
            }
            datalogger.LogDevice.Dispose();
            datalogger.Connected = false;
            if (string.IsNullOrEmpty(datalogger.OS))
                labelConnected.Text = "Disconnected";
            else
                labelConnected.Text = "Disconnected - OS: " + datalogger.OS;
            groupHWSettings.Enabled = true;
            if (j2534RadioButton.Checked)
                groupProtocol.Enabled = true;
            j2534OptionsGroupBox.Enabled = true;
            Logger(" [Done]");
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

        private void radioParamStd_CheckedChanged(object sender, EventArgs e)
        {
            if (radioParamStd.Checked)
            {
                LoadStdParams();
            }
        }

        private void radioParamRam_CheckedChanged(object sender, EventArgs e)
        {
            if (radioParamRam.Checked)
            {
                LoadRamParams();
                btnTestPids.Enabled = false;
            }
        }

        private void radioParamMath_CheckedChanged(object sender, EventArgs e)
        {
            if (radioParamMath.Checked)
            {
                LoadMathParams();
                btnTestPids.Enabled = false;
            }
        }

        private void chkBusFilters_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.LoggerUseFilters = chkVPWFilters.Checked;
            if (datalogger.Connected)
            {
                if (chkVPWFilters.Checked)
                {
                    datalogger.LogDevice.SetLoggingFilter();
                }
                else
                {
                    datalogger.LogDevice.RemoveFilters(null);
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            fpe.LoadObject(dataGridLogProfile.CurrentRow.DataBoundItem);
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                bsLogProfile.DataSource = null;
                bsLogProfile.DataSource = datalogger.PidProfile;
            }
        }

        private void saveProfileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectSaveProfile();
        }

        private void cutRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = dataGridLogProfile.CurrentRow.Index;
            ClipBrd = datalogger.PidProfile[i];
            datalogger.PidProfile.RemoveAt(i);
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = datalogger.PidProfile;
            ProfileDirty = true;
        }

        private void pasteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClipBrd == null)
                return;
            int i = dataGridLogProfile.CurrentRow.Index;
            datalogger.PidProfile.Insert(i, ClipBrd);
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = datalogger.PidProfile;
            ProfileDirty = true;
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = dataGridLogProfile.CurrentRow.Index;
            ClipBrd = datalogger.PidProfile[i];
        }

        private void LoadParameters()
        {
            if (radioParamStd.Checked)
                LoadStdParams();
            else if (radioParamRam.Checked)
                LoadRamParams();
            else
                LoadMathParams();
        }

        private void timerSearchParams_Tick(object sender, EventArgs e)
        {
            keyDelayCounter++;
            if (keyDelayCounter > AppSettings.keyPressWait100ms)
            {
                timerSearchParams.Enabled = false;
                LoadParameters();
            }
        }

        private void newProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            datalogger.PidProfile = new List<PidConfig>();
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = datalogger.PidProfile;
            profileFile = "";
            this.Text = "Logger";
        }

        private void chkEnableConsole_CheckedChanged(object sender, EventArgs e)
        {
/*            if (datalogger.Connected)
            {
                if (chkEnableConsole.Checked)
                {
                    //if (chkConsoleAutorefresh.Checked)
                    {
                        //datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port);
                    }
                    datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                    datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                }
                else
                {
                    datalogger.Receiver.StopReceiveLoop();
                    datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
                    datalogger.LogDevice.MsgSent -= LogDevice_MsgSent;
                }
            }
*/        }

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
                string fName = SelectSaveFile(TxtFilter);
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
            string fName = SelectFile("Select script file", TxtFilter, AppSettings.LastScriptFile);
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
            ushort module = Convert.ToUInt16(comboModule.SelectedValue); 
            if (txtDtcCustomModule.Text.Length > 0)
            {
                HexToUshort(txtDtcCustomModule.Text, out module);
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
                datalogger.QueueDtcRequest(module, mode);
            }
            else
            {
                datalogger.RequestDTCCodes(module, mode);
            }
        }

        private void btnStartStopAnalyzer_Click(object sender, EventArgs e)
        {
            if (datalogger.AnalyzerRunning)
            {
                StopAnalyzer(false);
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

        private void btnConsoleRefresh_Click(object sender, EventArgs e)
        {
            if (datalogger.Connected)
            {
                datalogger.LogDevice.Receive(true);
            }
            else
            {
                Logger("Not connected");
            }
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
            string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles","profile.xml");
            string FileName = SelectSaveFile(XmlFilter,defName);
            if (FileName.Length == 0)
                return;
            Logger("Saving file: " + FileName);
            J2534InitParameters JSettings = CreateJ2534InitParameters();
            LoggerUtils.SaveJ2534Settings(FileName, JSettings);
            AppSettings.LoggerJ2534SettingsFile = FileName;
            AppSettings.Save();
            Logger("[OK]");
        }

        private void btnJ2534SettingsLoad_Click(object sender, EventArgs e)
        {
            string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile.xml");
            string FileName = SelectFile("Select J2534 settings", XmlFilter,defName);
            if (FileName.Length == 0)
                return;
            //Logger("Loading file: " + FileName);
            J2534InitParameters JSettings = LoadJ2534Settings(FileName);
            LoadJ2534InitParameters(JSettings);
            AppSettings.LoggerJ2534SettingsFile = FileName;
            AppSettings.Save();
            Logger("[OK]");
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
            string fName = SelectFile("Select script file", TxtFilter, AppSettings.LastJScriptFile);
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
            string fName = SelectFile("Select log file", RtfFTxtilter);
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
            string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile2.xml");
            string FileName = SelectSaveFile(XmlFilter, defName);
            if (FileName.Length == 0)
                return;
            Logger("Saving file: " + FileName);
            J2534InitParameters JSettings = CreateJ2534InitParameters2();
            LoggerUtils.SaveJ2534Settings(FileName, JSettings);
            AppSettings.LoggerJ2534SettingsFile2 = FileName;
            AppSettings.Save();
            Logger("[OK]");
        }

        private void btnJ2534SettingsLoad2_Click(object sender, EventArgs e)
        {
            string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile2.xml");
            string FileName = SelectFile("Select J2534 settings", XmlFilter, defName);
            if (FileName.Length == 0)
                return;
            Logger("Loading file: " + FileName);
            J2534InitParameters JSettings = LoadJ2534Settings(FileName);
            LoadJ2534InitParameters2(JSettings);
            AppSettings.LoggerJ2534SettingsFile2 = FileName;
            AppSettings.Save();
            Logger("[OK]");
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
            string fName = SelectFile("Select log file", RtfFTxtilter);
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
                ConnectJConsole(null,null);
                canQuietResponses = 0;
                canDeviceResponses = -1;
                lastResponseTime = DateTime.Now;
                timerKeepBusQuiet.Enabled = true;
                timerWaitCANQuery.Enabled = true;
                canDevs = new List<CANDevice>();
                dataGridCANDevices.DataSource = null;
                jConsole.SetCANBusQuiet();
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
                    jConsole.Receiver.SetReceiverPaused(true);
                    CANQuery.Query(jConsole.JDevice);
                    jConsole.Receiver.SetReceiverPaused(false);
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
            if (hScrollPlayback.Value >= datalogger.LogDataBuffer.Count)
            {
                return;
            }
            LogData ld = datalogger.LogDataBuffer[hScrollPlayback.Value];
            LoggerDataEvents.Add(ld);
            //LastCalcValues = datalogger.slothandler.CalculatePidValues(datalogger.slothandler.LastPidValues);
            for (int row=0; row<ld.Values.Length && row < dataGridLogData.Rows.Count; row++)
            {
                dataGridLogData.Rows[row].Cells["Value"].Value = ld.CalculatedValues[row].ToString();
            }
            DateTime dt = new DateTime((long)ld.TimeStamp);
            labelTimeStamp.Text = dt.ToString("HH.mm.ss.ffff");
/*
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.PlayBackStep(hScrollPlayback.Value);
            }
*/
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
                for (int i=0; i<CanModules.Count;i++)
                {
                    if (CanModules[i].ModuleID == modId)
                    {
                        dataGridDtcCodes.Rows[r].Cells[0].Value = CanModules[i].ModuleName +" [" + modId.ToString("X4") + "]";
                        break;
                    }
                }
                if (dataGridDtcCodes.Rows[r].Cells[0].Value == null)
                {
                    dataGridDtcCodes.Rows[r].Cells[0].Value = modId.ToString("X4");
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
            if (radioCAN.Checked)
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
                            string val = pc.GetCalculatedValue(pidData, 0);
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

        private void btnQueyPid2_Click(object sender, EventArgs e)
        {
            Connect(radioVPW.Checked, true, true, null);
            List<int> selectedRows = new List<int>();
            foreach (DataGridViewCell cell in dataGridLogProfile.SelectedCells)
            {
                if (!selectedRows.Contains(cell.RowIndex))
                {
                    selectedRows.Add(cell.RowIndex);
                }
            }
            datalogger.Receiver.SetReceiverPaused(true);
            for (int r=0;r<selectedRows.Count;r++)
            {
                PidConfig pc = (PidConfig)dataGridLogProfile.Rows[selectedRows[r]].DataBoundItem;
                QueryPid(pc);
            }
            datalogger.Receiver.SetReceiverPaused(false);
        }


        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void txtJConsolePassFilters_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnTimeouts_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            UpatcherSettings tmpSettings = AppSettings.ShallowCopy();
            fpe.resetToDefaultValueToolStripMenuItem.Enabled = true;
            fpe.txtFilter.Text = "timeout";
            fpe.LoadObject(tmpSettings);
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
            fpe.LoadObject(tmpSettings);
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
            string FileName = SelectFile("Select J2534 settings", XmlXFilter, defName);
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
                if (comboFilterByOS.Text == "-")
                {
                    SupportedPids = null;
                }
                else
                {
                    SupportedPids = new List<ushort>();
                    string ospidfile = Path.Combine(Application.StartupPath, "Logger", "ospids", comboFilterByOS.Text + ".txt");
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
                LoadParameters();
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
            if (!Connect(radioVPW.Checked, false, true, null))
            {
                return;
            }
            SupportedPids = new List<ushort>();
            Application.DoEvents();
            datalogger.Receiver.SetReceiverPaused(true);
            for (int r = 0; r < stdParameters.Count; r++)
            {
                Parameter sp = stdParameters[r];
                PidConfig pc = new PidConfig();
                pc.Type = DefineBy.Pid;
                ushort pidNr;
                if (HexToUshort(sp.Id, out pidNr))
                    pc.addr = pidNr;
                PidDataType pd = (PidDataType)Enum.Parse(typeof(PidDataType), sp.DataType);
                pc.DataType = ConvertToDataType(pd);

                if (!SupportedPids.Contains(pidNr))
                {
                    ReadValue rv = datalogger.QuerySinglePidValue(pidNr, pc.DataType);
                    if (rv.PidNr == pidNr && rv.FailureCode == 0 && rv.PidValue > double.MinValue)
                    {
                        SupportedPids.Add(pidNr);
                    }
                }
                Application.DoEvents();
                Thread.Sleep(10);
            }
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
            comboFilterByOS.Text = datalogger.OS;
            datalogger.Receiver.SetReceiverPaused(false);
            LoadParameters();
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
                if (radioCAN.Checked && (ushort)comboModule.SelectedValue != datalogger.CanPcmAddr)
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
            LoadModuleList();
            if (radioCAN.Checked)
                txtPcmAddress.Enabled = true;
            else
                txtPcmAddress.Enabled = false;
        }

        private void radioVPW_CheckedChanged(object sender, EventArgs e)
        {

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
            string fName = SelectFile("Select log file", RtfFTxtilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.CAN36);
            cltb.ConvertFile(fName);
        }

        private void parseCAN23LogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtilter);
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
        }

        private void chkConsoleTimestamps_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void parseLogfileToBinToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void parseCAN78LogfileToBinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", RtfFTxtilter);
            if (fName.Length == 0)
                return;
            LogToBinConverter cltb = new LogToBinConverter(LogToBinConverter.RMode.CAN78);
            cltb.ConvertFile(fName);
        }
    }
}
