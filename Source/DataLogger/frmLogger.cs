using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

//using FTD2XX_NET;
//using SAE.J2534;

namespace UniversalPatcher
{
    public partial class frmLogger : Form
    {
        public frmLogger()
        {
            InitializeComponent();
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
        private SelectedTab selectedtab = SelectedTab.Logger;
        bool waiting4x = false;
        bool jConsoleWaiting4x = false;
        JConsole jConsole;
        OBDScript oscript;
        OBDScript joscript;
        ObdEmu oresp;
        private string jConsoleLogFile;
        StreamWriter jConsoleStream;
        int prevSlotCount = 0;
        int failCount = 0;
        int CANqueryCounter;
        int canQuietResponses;
        int canDeviceResponses;
        DateTime lastResponseTime;
        List <CANDevice> canDevs;
        private frmLoggerGraphics GraphicsForm;
        private frmHistogram HstForm;
        private ToolTip ScrollTip = new ToolTip();

        private void frmLogger_Load(object sender, EventArgs e)
        {
            datalogger = new DataLogger();
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
            txtJConsolePassFilters.Enter += TxtPassFilters_Enter;
            txtJConsolePassFilters2.Enter += TxtPassFilters_Enter;
        }

        private void TxtPassFilters_Enter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            string ttTxt = "Usage: mask:pattern:type | mask:pattern:type\r\nType: Pass | Block | Flow";
            TextBox txt = (TextBox)sender;
            tt.Show(ttTxt, txt, 3000);
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
                    if (!ConnectJConsole())
                    {
                        return;
                    }
                    MessageReceiver receiver = jConsole.Receiver;
                    if (radioJConsoleProto2.Checked)
                    {
                        receiver = jConsole.Receiver2;
                        joscript.SecondaryProtocol = true;
                    }
                    HandleConsoleCommand(jConsole.JDevice, receiver, txtJConsoleSend.Text, joscript);
                    e.Handled = true;
                    richJConsole.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
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
            selectedtab = SelectedTab.Advanced;
        }

        private void TabDTC_Enter(object sender, EventArgs e)
        {
            selectedtab = SelectedTab.Dtc;
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
                this.Invoke((MethodInvoker)delegate ()
                {
                    richVPWmessages.SelectionColor = Color.DarkGreen;
                    if (chkConsoleTimestamps.Checked)
                    {
                        string tStamp = "[" + new DateTime((long)e.Msg.SysTimeStamp).ToString("HH:mm:ss.fff") + "] ";
                        if (chkConsoleUseJ2534Timestamps.Checked)
                        {
                            tStamp += "[" + e.Msg.TimeStamp.ToString() + "] ";
                        }
                        richVPWmessages.AppendText(tStamp);
                    }
                    richVPWmessages.AppendText(BitConverter.ToString(e.Msg.GetBytes()).Replace("-", " ") + Environment.NewLine);

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

                    if (ChkEmulatorResponseMode.Checked)
                    {
                        List<string> resps = oresp.GetResponses(e.Msg);
                        for (int i=0; i< resps.Count;i++)
                        {
                            HandleConsoleCommand(datalogger.LogDevice, datalogger.Receiver, resps[i], oscript);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void JDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    string logTxt = "";
                    if (chkJConsoleTimestamps.Checked)
                    {
                        logTxt = "[" + new DateTime((long)e.Msg.SysTimeStamp).ToString("HH:mm:ss.fff") + "] ";
                    }
                    if (chkConsoleUseJ2534Timestamps.Checked)
                    {
                        logTxt += "[" + e.Msg.TimeStamp.ToString() + "] ";
                    }
                    logTxt += BitConverter.ToString(e.Msg.GetBytes()).Replace("-", " ") + Environment.NewLine;
                    if (chkJconsoleToScreen.Checked)
                    {
                        if (e.Msg.SecondaryProtocol)
                        {
                            richJConsole.SelectionColor = Color.Aquamarine;
                        }
                        else
                        {
                            richJConsole.SelectionColor = Color.DarkGreen;
                        }
                        richJConsole.AppendText(logTxt);
                    }
                    if (chkJConsoleToFile.Checked)
                    {
                        if (e.Msg.SecondaryProtocol)
                        {
                            jConsoleStream.WriteLine("\\cf1 " + logTxt +"\\par");
                        }
                        else
                        {
                            jConsoleStream.WriteLine("\\cf2 " + logTxt + "\\par");
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
                                        richJConsole.AppendText(cDev.ToString() + Environment.NewLine);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void LogDevice_MsgSent(object sender, MsgEventparameter e)
        {
            try
            {
            this.Invoke((MethodInvoker)delegate () {
                richVPWmessages.SelectionColor = Color.Red;
                if (chkConsoleTimestamps.Checked)
                {
                    string tStamp = "[" + new DateTime((long)e.Msg.SysTimeStamp).ToString("HH:mm:ss.fff") + "] ";
                    if (chkConsoleUseJ2534Timestamps.Checked)
                    {
                        tStamp += "[" + e.Msg.TimeStamp.ToString() + "] ";
                    }
                    richVPWmessages.AppendText(tStamp);
                }
                richVPWmessages.AppendText(BitConverter.ToString(e.Msg.GetBytes()).Replace("-", " ") + Environment.NewLine);
            });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void JDevice_MsgSent(object sender, MsgEventparameter e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate () {
                    string logTxt = "";
                    if (chkJConsoleTimestamps.Checked)
                    {
                        logTxt = "[" + new DateTime((long)e.Msg.SysTimeStamp).ToString("HH:mm:ss.fff") + "] ";
                    }
                    if (chkConsoleUseJ2534Timestamps.Checked)
                    {
                        logTxt += "[" + e.Msg.TimeStamp.ToString() + "] ";
                    }
                    logTxt += BitConverter.ToString(e.Msg.GetBytes()).Replace("-", " ") + Environment.NewLine;
                    if (chkJconsoleToScreen.Checked)
                    {
                        if (e.Msg.SecondaryProtocol)
                        {
                            richJConsole.SelectionColor = Color.Purple;
                        }
                        else
                        {
                            richJConsole.SelectionColor = Color.Red;
                        }
                        richJConsole.AppendText(logTxt);
                    }
                    if (chkJConsoleToFile.Checked)
                    {
                        if (e.Msg.SecondaryProtocol)
                        {
                            jConsoleStream.WriteLine("\\cf4 " + logTxt + "\\par");
                        }
                        else
                        {
                            jConsoleStream.WriteLine("\\cf3 " + logTxt + "\\par");
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void TabSettings_Enter(object sender, EventArgs e)
        {
            selectedtab = SelectedTab.Settings;
        }

        private void TxtSendBus_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    if (!Connect())
                    {
                        return;
                    }
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
            selectedtab = SelectedTab.Profile;
        }

        private void TabAnalyzer_Enter(object sender, EventArgs e)
        {
            selectedtab = SelectedTab.Analyzer;
        }

        private void DataGridAnalyzer_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void ListProfiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (datalogger.LogRunning)
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
            Debug.WriteLine("Ports modified");
            this.Invoke((MethodInvoker)delegate(){ LoadPorts(false);});
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
            selectedtab = SelectedTab.Logger;
            SetupLogDataGrid();
        }

        private void LoadPorts(bool LoadDefaults)
        {
            try
            {
                comboSerialPort.Items.Clear();
                if (chkFTDI.Checked)
                {
                    string[] ftdiDevs = FTDI_Finder.FindFtdiDevices().ToArray();
                    if (ftdiDevs.Length > 0)
                    {
                        comboSerialPort.Items.AddRange(ftdiDevs);
                        if (LoadDefaults && !string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerFTDIPort) && comboSerialPort.Items.Contains(UniversalPatcher.Properties.Settings.Default.LoggerFTDIPort))
                            comboSerialPort.Text = UniversalPatcher.Properties.Settings.Default.LoggerFTDIPort;
                        else
                            comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                    }
                    else
                    {
                        comboSerialPort.Text = "";
                    }
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
                                if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerComPort) && comboSerialPort.Items.Contains(UniversalPatcher.Properties.Settings.Default.LoggerComPort))
                                    comboSerialPort.Text = UniversalPatcher.Properties.Settings.Default.LoggerComPort;
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
                if (LoadDefaults)
                {
                    if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerJ2534Device))
                    {
                        j2534DeviceList.Text = UniversalPatcher.Properties.Settings.Default.LoggerJ2534Device;
                        comboJ2534DLL.Text = UniversalPatcher.Properties.Settings.Default.LoggerJ2534Device;
                    }
                    else if (jDevList.Count > 0)
                    {
                        j2534DeviceList.Text = jDevList.FirstOrDefault().Name;
                        comboJ2534DLL.Text = jDevList.FirstOrDefault().Name;
                    }
                    j2534RadioButton.Checked = UniversalPatcher.Properties.Settings.Default.LoggerUseJ2534;
                }
                LoadJ2534Protocols();
                //comboJ2534Connectflag.DataSource = Enum.GetValues(typeof(J2534DotNet.ConnectFlag));
                comboJ2534Connectflag.Items.Clear();
                comboJ2534Connectflag2.Items.Clear();
                foreach(string item in Enum.GetNames(typeof(J2534DotNet.ConnectFlag)))
                {
                    comboJ2534Connectflag.Items.Add(item);
                    comboJ2534Connectflag2.Items.Add(item);
                }
                //comboJ2534Connectflag.SelectedIndex = 0;
                if (!String.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile))
                {
                    J2534InitParameters JSettings = LoadJ2534Settings(UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile);
                    LoadJ2534InitParameters(JSettings);
                }
                if (!String.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile2))
                {
                    J2534InitParameters JSettings = LoadJ2534Settings(UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile2);
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
                foreach (string item in Enum.GetNames(typeof(LoggerUtils.KInit)))
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
                if (UniversalPatcher.Properties.Settings.Default.MainWindowPersistence)
                {
                    if (UniversalPatcher.Properties.Settings.Default.LoggerWindowSize.Width > 0 || UniversalPatcher.Properties.Settings.Default.LoggerWindowSize.Height > 0)
                    {
                        this.WindowState = UniversalPatcher.Properties.Settings.Default.LoggerWindowState;
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        this.Location = UniversalPatcher.Properties.Settings.Default.LoggerWindowPosition;
                        this.Size = UniversalPatcher.Properties.Settings.Default.LoggerWindowSize;
                    }

                }

                txtTstampFormat.Text = UniversalPatcher.Properties.Settings.Default.LoggerTimestampFormat;
                chkFTDI.Checked = UniversalPatcher.Properties.Settings.Default.LoggerUseFTDI;
                comboSerialDeviceType.Items.Add(AvtDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(AvtLegacyDevice.DeviceType);
                comboSerialDeviceType.Items.Add(ElmDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(LegacyElmDeviceImplementation.DeviceType);
                comboSerialDeviceType.Items.Add(OBDXProDevice.DeviceType);
                comboSerialDeviceType.Text = UniversalPatcher.Properties.Settings.Default.LoggerSerialDeviceType;

                LoadPorts(true);

                //chkAdvanced.Checked = UniversalPatcher.Properties.Settings.Default.LoggerShowAdvanced;
                chkVPWFilters.Checked = UniversalPatcher.Properties.Settings.Default.LoggerUseFilters;
                chkEnableConsole.Checked = UniversalPatcher.Properties.Settings.Default.LoggerEnableConsole;
                chkConsoleTimestamps.Checked = UniversalPatcher.Properties.Settings.Default.LoggerConsoleTimestamps;
                chkConsoleUseJ2534Timestamps.Checked = UniversalPatcher.Properties.Settings.Default.LoggerConsoleJ2534Timestamps;
                chkJConsoleTimestamps.Checked = UniversalPatcher.Properties.Settings.Default.JConsoleTimestamps;
                chkJConsole4x.Checked = UniversalPatcher.Properties.Settings.Default.JConsole4x;
                chkConsole4x.Checked = UniversalPatcher.Properties.Settings.Default.LoggerConsole4x;
                comboJ2534DLL.Text = UniversalPatcher.Properties.Settings.Default.JConsoleDevice;
                numConsoleScriptDelay.Value = UniversalPatcher.Properties.Settings.Default.LoggerScriptDelay;

                if (UniversalPatcher.Properties.Settings.Default.LoggerConsoleFont != null)
                {
                    richVPWmessages.Font = UniversalPatcher.Properties.Settings.Default.LoggerConsoleFont;
                }

                if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerLogFolder))
                {
                    txtLogFolder.Text = UniversalPatcher.Properties.Settings.Default.LoggerLogFolder;
                }
                else
                {
                    txtLogFolder.Text = Path.Combine(Application.StartupPath, "Logger", "Log");
                }

                comboResponseMode.DataSource = Enum.GetValues(typeof(ResponseTypes));

                if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerLogSeparator))
                    txtLogSeparator.Text = UniversalPatcher.Properties.Settings.Default.LoggerLogSeparator;
                else
                    txtLogSeparator.Text = ",";
                if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerDecimalSeparator))
                    txtDecimalSeparator.Text = UniversalPatcher.Properties.Settings.Default.LoggerDecimalSeparator;
                else
                    txtDecimalSeparator.Text = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerResponseMode))
                {
                    comboResponseMode.Text = UniversalPatcher.Properties.Settings.Default.LoggerResponseMode;
                }
                chkPriority.Checked = UniversalPatcher.Properties.Settings.Default.LoggerUsePriority;
                comboBaudRate.Text = UniversalPatcher.Properties.Settings.Default.LoggerBaudRate.ToString();


                datalogger.PidProfile = new List<LoggerUtils.PidConfig>();
                if (!string.IsNullOrEmpty(UniversalPatcher.Properties.Settings.Default.LoggerLastProfile) && File.Exists(UniversalPatcher.Properties.Settings.Default.LoggerLastProfile))
                {
                    profileFile = UniversalPatcher.Properties.Settings.Default.LoggerLastProfile;
                    datalogger.LoadProfile(profileFile);
                    this.Text = "Logger - " + profileFile;
                    CheckMaxPids();
                }
                bsLogProfile.DataSource = datalogger.PidProfile;
                dataGridLogProfile.DataSource = bsLogProfile;

                analyzer = new Analyzer();
                //comboModule.DataSource = new BindingSource(analyzer.PhysAddresses, null);
                List<KeyValuePair<byte, string>> modules = analyzer.PhysAddresses.ToList(); //new List<System.Collections.Generic.KeyValuePair<byte, string>>();
                //modules.Add(new KeyValuePair<byte, string>( 0xFF, "ALL"));
                comboModule.DataSource = modules;
                comboModule.DisplayMember = "Value";
                comboModule.ValueMember = "Key";
                comboModule.Text = "ECU";

                dataGridAnalyzer.Columns.Clear();
                Analyzer.AnalyzerData ad = new Analyzer.AnalyzerData();
                foreach (var prop in ad.GetType().GetProperties())
                {
                    dataGridAnalyzer.Columns.Add(prop.Name, prop.Name);
                }

            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        private void DataGridPidNames_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //Debug.WriteLine(e.Exception);
        }

        private void LoadStdParams()
        {
            try
            {
                if (!chkFilterParamsByOS.Checked && !string.IsNullOrEmpty(datalogger.OS) && SupportedPids != null && SupportedPids.Count > 0)
                {
                    Logger("Filtering parameters by OS: " + datalogger.OS);
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
                if (chkFilterParamsByOS.Checked && !string.IsNullOrEmpty(datalogger.OS))
                {
                    Logger("Filtering parameters by OS: " + datalogger.OS);
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
                    Disconnect();
                }
                if (jConsole != null && jConsole.JDevice != null && jConsole.Connected)
                {
                    DisconnectJConsole();
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
                if (UniversalPatcher.Properties.Settings.Default.MainWindowPersistence)
                {
                    UniversalPatcher.Properties.Settings.Default.LoggerWindowState = this.WindowState;
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        UniversalPatcher.Properties.Settings.Default.LoggerWindowPosition = this.Location;
                        UniversalPatcher.Properties.Settings.Default.LoggerWindowSize = this.Size;
                    }
                    else
                    {
                        UniversalPatcher.Properties.Settings.Default.LoggerWindowPosition = this.RestoreBounds.Location;
                        UniversalPatcher.Properties.Settings.Default.LoggerWindowSize = this.RestoreBounds.Size;
                    }
                    UniversalPatcher.Properties.Settings.Default.Save();
                }

                Thread.Sleep(100);
            }
            catch { }
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
                    if (failCount > 15)
                    {
                        timerShowData.Enabled = false;
                        StopLogging(true);
                        StartLogging(true);
                        failCount = 0;
                        timerShowData.Enabled = true;
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
            for (int i=0; i< datalogger.PidProfile.Count; i++)
            {
                if (!pids.Contains(datalogger.PidProfile[i].addr))
                {
                    pids.Add(datalogger.PidProfile[i].addr);
                    bytesTotal += GetElementSize((InDataType)datalogger.PidProfile[i].DataType);
                }
            }
            labelProgress.Text = "Profile size: " + bytesTotal.ToString() + " bytes, Maximum: 48";
            if (bytesTotal > 48)
            {
                LoggerBold("Profile have total: " + bytesTotal.ToString() + " bytes, maximum is 48 bytes");
            }
        }

        private void AddToProfile(PidConfig pc)
        {
            try
            {
                if (datalogger.Connected)
                {
                    QueryPid(pc);
                }
                CheckMaxPids();
                datalogger.PidProfile.Add(pc);
                ProfileDirty = true;
                bsLogProfile.DataSource = null;
                bsLogProfile.DataSource = datalogger.PidProfile;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void AddSelectedPidsToProfile()
        {
            if (!datalogger.Connected)
            {
                Logger("Not connected - adding pids without testing compatibility");
            }

            List<PidConfig> pds = ConvertSelectedPidConfigs();
            foreach (PidConfig pc in pds)
            {
                AddToProfile(pc);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelectedPidsToProfile();
        }

        private void SetupLogDataGrid()
        {
            if (datalogger.LogRunning)
                return; //Dont change settings while logging
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
                UniversalPatcher.Properties.Settings.Default.LoggerFTDIPort = comboSerialPort.Text;
            else
                UniversalPatcher.Properties.Settings.Default.LoggerComPort = comboSerialPort.Text;
            UniversalPatcher.Properties.Settings.Default.LoggerUseJ2534 = j2534RadioButton.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerJ2534Device = j2534DeviceList.Text;
            UniversalPatcher.Properties.Settings.Default.LoggerSerialDeviceType = comboSerialDeviceType.Text;
            UniversalPatcher.Properties.Settings.Default.LoggerBaudRate = Convert.ToInt32(comboBaudRate.Text);

            if (txtLogFolder.Text.Length > 0)
            {
                UniversalPatcher.Properties.Settings.Default.LoggerLogFolder = txtLogFolder.Text;
                UniversalPatcher.Properties.Settings.Default.LoggerLogSeparator = txtLogSeparator.Text;
            }
            UniversalPatcher.Properties.Settings.Default.LoggerLastProfile = profileFile;
            UniversalPatcher.Properties.Settings.Default.LoggerResponseMode = comboResponseMode.Text;
            UniversalPatcher.Properties.Settings.Default.LoggerUseFTDI = chkFTDI.Checked;
            //UniversalPatcher.Properties.Settings.Default.LoggerShowAdvanced = chkAdvanced.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerUsePriority = chkPriority.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerUseFilters = chkVPWFilters.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerEnableConsole = chkEnableConsole.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerConsoleTimestamps = chkConsoleTimestamps.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerConsoleJ2534Timestamps = chkConsoleUseJ2534Timestamps.Checked;
            UniversalPatcher.Properties.Settings.Default.LoggerConsoleFont = richVPWmessages.Font;
            UniversalPatcher.Properties.Settings.Default.LoggerScriptDelay = (int) numConsoleScriptDelay.Value;
            UniversalPatcher.Properties.Settings.Default.LoggerConsole4x = chkConsole4x.Checked;
            UniversalPatcher.Properties.Settings.Default.JConsole4x = chkJConsole4x.Checked;
            UniversalPatcher.Properties.Settings.Default.JConsoleTimestamps = chkJConsoleTimestamps.Checked;
            UniversalPatcher.Properties.Settings.Default.JConsoleDevice = comboJ2534DLL.Text;
            UniversalPatcher.Properties.Settings.Default.Save();

        }

        private void StopLogging(bool disconnect)
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
                if (!disconnect)
                {
                    if (datalogger.AnalyzerRunning || chkConsoleAutorefresh.Checked)
                    { 
                        datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port);
                    }
                }               
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private bool Connect(bool ShowOs = true)
        {
            try
            {
                datalogger.useVPWFilters = chkVPWFilters.Checked;
                if (datalogger.Connected)
                {
                    return true;
                }
                Logger("Connecting (VPW)...");
                Application.DoEvents();
                if (serialRadioButton.Checked)
                {
                    string sPort = comboSerialPort.Text;
                    string[] sParts = sPort.Split(':');
                    if (sParts.Length > 1)
                        sPort = sParts[0].Trim();
                    datalogger.LogDevice = datalogger.CreateSerialDevice(sPort, comboSerialDeviceType.Text, chkFTDI.Checked);
                }
                else
                {
                    J2534DotNet.J2534Device dev = jDevList[j2534DeviceList.SelectedIndex];
                    //passThru.LoadLibrary(dev);
                    datalogger.LogDevice = new J2534Device(dev);
                    datalogger.LogDevice.SetReadTimeout(100);
                }
                datalogger.LogDevice.MsgReceived += LogDevice_DTC_MsgReceived;
                if (chkEnableConsole.Checked)
                {
                    datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                    datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                }
                if (!datalogger.LogDevice.Initialize(Convert.ToInt32(comboBaudRate.Text), new J2534InitParameters(true)))
                {
                    datalogger.port.Dispose();
                    datalogger.LogDevice.Dispose();
                    return false;
                }
                oscript = new OBDScript(datalogger.LogDevice, datalogger.Receiver);
                datalogger.LogDevice.Enable4xReadWrite = chkConsole4x.Checked;

                if (serialRadioButton.Checked)
                {
                    datalogger.LogDevice.SetTimeout(TimeoutScenario.DataLogging3);
                    datalogger.LogDevice.SetReadTimeout(2000);
                }
                string os = "";
                if (ShowOs)
                {
                    os = datalogger.QueryPcmOS();
                    if (os == null)
                    {
                        //return false;
                    }
                }
                if (!string.IsNullOrEmpty(os))
                    labelConnected.Text = "Connected - OS: " + os;
                else
                    labelConnected.Text = "Connected";
                datalogger.Connected = true;
                SaveSettings();
                if (radioParamRam.Checked)
                {
                    LoadParameters();
                }
                if (chkConsoleAutorefresh.Checked)
                {
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port);
                }
                Application.DoEvents();
                groupHWSettings.Enabled = false;
                j2534OptionsGroupBox.Enabled = false;
                return true;
            }
            catch (Exception ex)
            {
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
            initParameters.PriodicInterval = (int)numJ2534PeriodicMsgInterval.Value;
            initParameters.PassFilters = txtJConsolePassFilters.Text;
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
            initParameters.PriodicInterval = (int)numJ2534PeriodicMsgInterval2.Value;
            initParameters.PassFilters = txtJConsolePassFilters2.Text;
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
            numJ2534PeriodicMsgInterval.Value = initParameters.PriodicInterval;
            txtJConsolePassFilters.Text = initParameters.PassFilters;
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
            numJ2534PeriodicMsgInterval2.Value = initParameters.PriodicInterval;
            txtJConsolePassFilters2.Text = initParameters.PassFilters;
        }

        private bool ConnectJConsole()
        {
            try
            {
                if (jConsole != null && jConsole.Connected)
                {
                    return true;
                }
                jConsole = new JConsole();
                jConsole.Receiver = new MessageReceiver();
                Application.DoEvents();
                J2534InitParameters initParameters = new J2534InitParameters(false);
                initParameters = CreateJ2534InitParameters();
                J2534DotNet.J2534Device dev = jDevList[comboJ2534DLL.SelectedIndex];
                jConsole.JDevice = new J2534Device(dev);
                //jConsole.JDevice.SetProtocol(protocol, baudrate, flag);
                jConsole.JDevice.MsgSent += JDevice_MsgSent;
                jConsole.JDevice.MsgReceived += JDevice_MsgReceived;
                if (!jConsole.JDevice.Initialize(0, initParameters))
                {
                    jConsole.port.Dispose();
                    jConsole.JDevice.Dispose();
                    return false;
                }
                jConsole.JDevice.Enable4xReadWrite = chkJConsole4x.Checked;
                jConsole.JDevice.SetReadTimeout(100);
                labelJconsoleConnected.Text = "Connected";
                jConsole.Connected = true;
                SaveSettings();
                jConsole.Receiver.StartReceiveLoop(jConsole.JDevice, null);
                joscript = new OBDScript(jConsole.JDevice, jConsole.Receiver);
                Application.DoEvents();
                //groupJ2534Options.Enabled = false;
                btnJConsoleConnectSecondProtocol.Enabled = true;
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
                UniversalPatcher.Properties.Settings.Default.LoggerTimestampFormat = txtTstampFormat.Text;
                UniversalPatcher.Properties.Settings.Default.LoggerDecimalSeparator = txtDecimalSeparator.Text;
                UniversalPatcher.Properties.Settings.Default.LoggerLogSeparator = txtLogSeparator.Text;
                UniversalPatcher.Properties.Settings.Default.Save();
                labelProgress.Text = "";
                if (datalogger.PidProfile.Count == 0 || datalogger.PidProfile[0].addr == 0xffffff)
                {
                    Logger("No profile configured");
                    return;
                }
                if (!Connect())
                {
                    return;
                }
                if (GraphicsForm != null && GraphicsForm.Visible)
                {
                    GraphicsForm.StartLiveUpdate(false);
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
                        UniversalPatcher.Properties.Settings.Default.LoggerLastLogfile = logfilename;
                        UniversalPatcher.Properties.Settings.Default.Save();
                    }
                    if (!datalogger.CreateLog(logfilename))
                    {
                        return;
                    }
                }
                if (datalogger.StartLogging())
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
            if (datalogger.LogRunning)
            {
                StopLogging(false);
            }
            else
            {
                StartLogging(false);
            }
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
            }
            catch(Exception ex)
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
                serialOptionsGroupBox.Enabled = true;
                j2534OptionsGroupBox.Enabled = false;                
                //groupJ2534Options.Visible = false;
            }
        }

        private void j2534RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.j2534RadioButton.Checked)
            {
                serialOptionsGroupBox.Enabled = false;
                j2534OptionsGroupBox.Enabled = true;
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
            LoadPorts(false);
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
                if (!Connect(false))
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
                    datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port);
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
                if (disconnect || !chkConsoleAutorefresh.Checked)
                {
                    datalogger.Receiver.StopReceiveLoop();
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
                LoadParameters();
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
            PcmFile PCM = new PcmFile(fName, true, "");            
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
            byte module;
            dataGridDtcCodes.Rows.Clear();
            if (!Connect())
            {
                return;
            }
            //List<DTCCodeStatus> codelist = new List<DTCCodeStatus>();
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
                if (e.Msg.Length > 6 && e.Msg.GetBytes()[1] == DeviceId.Tool && e.Msg.GetBytes()[3] == 0x59)
                {
                    DTCCodeStatus dcs = datalogger.DecodeDTCstatus(e.Msg.GetBytes());
                    if (!string.IsNullOrEmpty(dcs.Module))
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            int r = dataGridDtcCodes.Rows.Add();
                            dataGridDtcCodes.Rows[r].Cells[0].Value = dcs.Module;
                            dataGridDtcCodes.Rows[r].Cells[1].Value = dcs.Code;
                            dataGridDtcCodes.Rows[r].Cells[2].Value = dcs.Description;
                            dataGridDtcCodes.Rows[r].Cells[3].Value = dcs.Status;
                        });
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
                Debug.WriteLine("Error, LogDevice_DTC_MsgReceived line " + line + ": " + ex.Message);
            }
        }

        private void btnCurrentDTC_Click(object sender, EventArgs e)
        {
            getDtcCodes(2);//2 = Current data
        }

        private void btnHistoryDTC_Click(object sender, EventArgs e)
        {
            getDtcCodes(0x10);
        }

        private void btnClearCodes_Click(object sender, EventArgs e)
        {
            Connect();

            byte module = (byte)comboModule.SelectedValue;
            if (chkDtcAllModules.Checked)
            {
                module = DeviceId.Broadcast;
            }
            if (datalogger.LogRunning && datalogger.LogDevice.LogDeviceType == LoggingDevType.Elm)
            {
                OBDMessage msg = new OBDMessage(new byte[] { Priority.Physical0, module, DeviceId.Tool, 0x10, 0x00 });
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

        private void QueryPid(PidConfig pc)
        {
            try
            {
                Connect();

                ReadValue rv;
                ReadValue rv2 = new ReadValue();
                //rv = QueryRAM(pc.addr, pc.DataType);
                rv = datalogger.QuerySinglePidValue(pc.addr, pc.DataType);
                if (pc.addr2 > 0)
                {
                    rv2 = datalogger.QuerySinglePidValue(pc.addr2, pc.Pid2DataType);
                }
                if (rv.PidValue > double.MinValue)
                {
                    string calcVal = pc.GetCalculatedValue(rv.PidValue, rv2.PidValue);
                    Logger(pc.PidName + ": " + calcVal + " " + pc.Units);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnQueryPid_Click(object sender, EventArgs e)
        {
            Connect();
            List<PidConfig> pds = ConvertSelectedPidConfigs();
            foreach (PidConfig pc in pds)
            {
                QueryPid(pc);
            }
        }

        private void Disconnect()
        {
            oscript.stopscript = true;
            datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
            datalogger.LogDevice.MsgReceived -= LogDevice_DTC_MsgReceived;

            if (chkEnableConsole.Checked || datalogger.Receiver.ReceiveLoopRunning)
            {
                datalogger.Receiver.StopReceiveLoop();
                datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
                datalogger.LogDevice.MsgSent -= LogDevice_MsgSent;
            }
            if (datalogger.LogRunning)
            {
                StopLogging(true);
            }
            if (datalogger.AnalyzerRunning)
            {
                StopAnalyzer(true);
            }
            datalogger.LogDevice.Dispose();
            datalogger.Connected = false;
            labelConnected.Text = "Disconnected - OS: " + datalogger.OS;
            groupHWSettings.Enabled = true;
            j2534OptionsGroupBox.Enabled = true;
        }

        private void DisconnectJConsole()
        {
            chkJConsoleToFile.Checked = false;
            joscript.stopscript = true;
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
            if (datalogger.Connected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
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
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void btnGetVINCode_Click(object sender, EventArgs e)
        {
            Connect();
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
            }
        }

        private void radioParamMath_CheckedChanged(object sender, EventArgs e)
        {
            if (radioParamMath.Checked)
            {
                LoadMathParams();
            }
        }

        private void chkBusFilters_CheckedChanged(object sender, EventArgs e)
        {
            if (datalogger.Connected)
            {
                if (chkVPWFilters.Checked)
                {
                    datalogger.LogDevice.SetLoggingFilter();
                }
                else
                {
                    datalogger.LogDevice.RemoveFilters();
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
            if (keyDelayCounter > UniversalPatcher.Properties.Settings.Default.keyPressWait100ms)
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
            if (datalogger.Connected)
            {
                if (chkEnableConsole.Checked)
                {
                    if (chkConsoleAutorefresh.Checked)
                    {
                        datalogger.Receiver.StartReceiveLoop(datalogger.LogDevice, datalogger.port);
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
            string fName = SelectFile("Select script file", TxtFilter);
            if (fName.Length == 0)
                return;
            if (!Connect())
            {
                return;
            }
            Logger("Sending file: " + fName);
            oscript = new OBDScript(datalogger.LogDevice, datalogger.Receiver);
            btnStopScript.Enabled = true;
            oscript.UploadScript(fName);
            btnStopScript.Enabled = false;
            Logger("Done");
        }

        private void btnDtcCustom_Click(object sender, EventArgs e)
        {
            byte mode;
            byte module = (byte)comboModule.SelectedValue; 
            if (txtDtcCustomModule.Text.Length > 0)
            {
                HexToByte(txtDtcCustomModule.Text, out module);
            }
            if (!HexToByte(txtDtcCustomMode.Text,out mode))
            {
                LoggerBold("Unknown HEX number: " + txtDtcCustomMode.Text);
                return;
            }

            dataGridDtcCodes.Rows.Clear();
            Connect();
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
            if (datalogger.Connected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }

        }

        private void txtSendBus_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConsoleRefresh_Click(object sender, EventArgs e)
        {
            if (datalogger.Connected)
            {
                datalogger.LogDevice.Receive();
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
            if (chkConsoleAutorefresh.Checked)
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
            if (comboJ2534Init.Text  == LoggerUtils.KInit.FastInit_J1979.ToString())
            {
                txtJ2534InitBytes.Text = "C1 33 F1 81";
            }
            else if (comboJ2534Init.Text == LoggerUtils.KInit.FastInit_GMDelco.ToString())
            {
                txtJ2534InitBytes.Text = "81 11 F1 81";
                txtJ2534PeriodicMsg.Text = "80 11 F1 01 3E";
                //0x80, 0x11, 0xF1, 0x01, 0x3E
            }
            else if (comboJ2534Init.Text == LoggerUtils.KInit.FastInit_ME7_5.ToString())
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
            UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile = FileName;
            UniversalPatcher.Properties.Settings.Default.Save();
            Logger("[OK]");
        }

        private void btnJ2534SettingsLoad_Click(object sender, EventArgs e)
        {
            string defName = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", "profile.xml");
            string FileName = SelectFile("Select J2534 settings", XmlFilter,defName);
            if (FileName.Length == 0)
                return;
            Logger("Loading file: " + FileName);
            J2534InitParameters JSettings = LoadJ2534Settings(FileName);
            LoadJ2534InitParameters(JSettings);
            UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile = FileName;
            UniversalPatcher.Properties.Settings.Default.Save();
            Logger("[OK]");
        }

        private void chkConsoleUseJ2534Timestamps_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnJConsoleConnect_Click(object sender, EventArgs e)
        {
            if (jConsole != null && jConsole.Connected)
            {
                DisconnectJConsole();
            }
            else
            {
                ConnectJConsole();
            }
        }

        private void txtJConsoleSend_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnJConsoleUploadScript_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select script file", TxtFilter);
            if (fName.Length == 0)
                return;
            if (!ConnectJConsole())
            {
                return;
            }
            Logger("Sending file: " + fName);
            joscript = new OBDScript(jConsole.JDevice, jConsole.Receiver);
            joscript.SecondaryProtocol = radioJConsoleProto2.Checked;
            btnJconsoleStopScript.Enabled = true;
            joscript.UploadScript(fName);
            btnJconsoleStopScript.Enabled = false;
            Logger("Done");
        }

        private void numJConsoleScriptDelay_ValueChanged(object sender, EventArgs e)
        {
            UniversalPatcher.Properties.Settings.Default.LoggerScriptDelay = (int)numConsoleScriptDelay.Value;
        }

        private void numConsoleScriptDelay_ValueChanged(object sender, EventArgs e)
        {
            UniversalPatcher.Properties.Settings.Default.LoggerScriptDelay = (int)numConsoleScriptDelay.Value;
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

        private void btnAlgoTest_Click(object sender, EventArgs e)
        {
            int algo;
            ushort seed = 0;
            bool found = false;
            int algoFrom = 0;
            int algoTo = 0x300;

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
            LogToBin ltb = new LogToBin();
            ltb.ConvertFile(fName);
        }

        private void btnConsoleEditResponses_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectFile("Select response file", XmlFilter);
                frmEditXML frmE = new frmEditXML();
                frmE.LoadObdResponses(fName);
                if (frmE.ShowDialog() == DialogResult.OK)
                {
                    oresp.Responses = frmE.obdresponses;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void OpenEmu()
        {
            if (ChkEmulatorResponseMode.Checked && oresp == null)
            {
                byte id;
                if (!HexToByte(txtEmulatorId.Text, out id))
                {
                    LoggerBold("Can't convert from hex: " + txtEmulatorId.Text);
                    return;
                }
                oresp = new ObdEmu(id);
            }
        }

        private void btnConsoleLoadResponses_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select response file", XmlFilter);
            if (fName.Length == 0)
            {
                return;
            }
            OpenEmu();
            oresp.Responses = ObdEmu.LoadObdResponseFile(fName);
        }

        private void ChkConsoleResponseMode_CheckedChanged(object sender, EventArgs e)
        {
            OpenEmu();
        }


        private void btnJConsoleConnectSecondProtocol_Click(object sender, EventArgs e)
        {
            if (jConsole.Connected2)
            {
                return;
            }
            J2534InitParameters JSettings = CreateJ2534InitParameters2();
            if (jConsole.JDevice.ConnectSecondaryProtocol(JSettings))
            {
                groupJConsoleProto.Enabled = true;
                jConsole.Connected2 = true;
                jConsole.Receiver2 = new MessageReceiver();
                jConsole.Receiver2.SecondaryProtocol = true;
                jConsole.Receiver2.StartReceiveLoop(jConsole.JDevice,jConsole.port);
                btnJConsoleConnectSecondProtocol.Enabled = false;
            }
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
            UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile2 = FileName;
            UniversalPatcher.Properties.Settings.Default.Save();
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
            UniversalPatcher.Properties.Settings.Default.LoggerJ2534SettingsFile2 = FileName;
            UniversalPatcher.Properties.Settings.Default.Save();
            Logger("[OK]");
        }

        private void comboJ2534Protocol2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadJ2534Baudrates2();
        }

        private void comboJ2534Init2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboJ2534Init2.Text == LoggerUtils.KInit.FastInit_J1979.ToString())
            {
                txtJ2534InitBytes2.Text = "C1 33 F1 81";
            }
            else if (comboJ2534Init2.Text == LoggerUtils.KInit.FastInit_GMDelco.ToString())
            {
                txtJ2534InitBytes2.Text = "81 11 F1 81";
                txtJ2534PeriodicMsg2.Text = "80 11 F1 01 3E";
            }
            else if (comboJ2534Init2.Text == LoggerUtils.KInit.FastInit_ME7_5.ToString())
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
                jConsoleLogFile = fName;
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

        private void button1_Click(object sender, EventArgs e)
        {
            richJConsole.AppendText("Default" + Environment.NewLine);
            richJConsole.SelectionColor = Color.Aquamarine;
            richJConsole.AppendText("Aquamarine" + Environment.NewLine);
            richJConsole.SelectionColor = Color.DarkGreen;
            richJConsole.AppendText("DarkGreen" + Environment.NewLine);

            richJConsole.SelectionColor = Color.Red;
            richJConsole.AppendText("Red" + Environment.NewLine);
            richJConsole.SelectionColor = Color.Purple;
            richJConsole.AppendText("Purple" + Environment.NewLine);

        }

        private void btnJconsoleConfigFilters_Click(object sender, EventArgs e)
        {
            if (jConsole == null || jConsole.JDevice == null)
            {
                return;
            }
            jConsole.JDevice.SetupFilters(txtJConsolePassFilters.Text, false);
        }

        private void btnJconsoleConfigFilters2_Click(object sender, EventArgs e)
        {
            if (jConsole == null || jConsole.JDevice == null)
            {
                return;
            }
            jConsole.JDevice.SetupFilters(txtJConsolePassFilters2.Text, true);
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
            CanLogToBin cltb = new CanLogToBin();
            cltb.ConvertFile(fName);

        }

        private void queryCANDevicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CANqueryCounter = 0;
            canQuietResponses = 0;
            canDeviceResponses = -1;
            lastResponseTime = DateTime.Now;
            timerKeepBusQuiet.Enabled = true;
            timerWaitCANQuery.Enabled = true;
            canDevs = new List<CANDevice>();
            dataGridCANDevices.DataSource = null;
            jConsole.SetCANBusQuiet();
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
                    CANQuery.Query(jConsole.JDevice,jConsole.Receiver);
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
                }
            }
        }

        private void SetupLiveGraphics()
        {
            try
            {
                GraphicsForm = new frmLoggerGraphics();
                GraphicsForm.Text = "Logger Graph";
                GraphicsForm.Show();
                GraphicsForm.SetupLiveGraphics();
                if (datalogger.LogRunning || timerPlayback.Enabled)
                {
                    GraphicsForm.StartLiveUpdate(timerPlayback.Enabled);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void showSavedLogGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                frmLoggerGraphics frmLG = new frmLoggerGraphics();
                frmLG.Text = "Logger Graphics";
                frmLG.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void listProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnShowGraphics_Click(object sender, EventArgs e)
        {
            SetupLiveGraphics();
        }

        private void btnShowHistogram_Click(object sender, EventArgs e)
        {
            HstForm = new frmHistogram();
            HstForm.Show();
            HstForm.AddTunerToTab();
            HstForm.SetupLiveParameters();
        }


        private void playbackLogfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string fName = SelectFile("Select Log file", CsvFilter);
                if (string.IsNullOrEmpty(fName))
                    return;
                UniversalPatcher.Properties.Settings.Default.LoggerTimestampFormat = txtTstampFormat.Text;
                UniversalPatcher.Properties.Settings.Default.LoggerDecimalSeparator = txtDecimalSeparator.Text;
                UniversalPatcher.Properties.Settings.Default.LoggerLogSeparator = txtLogSeparator.Text;
                UniversalPatcher.Properties.Settings.Default.Save();
                datalogger.LoadLogFile(fName);
                SetupLogDataGrid();
                hScrollPlayback.Maximum = datalogger.LogDataBuffer.Count;

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
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.SetupPlayBack();
                //GraphicsForm.SetupLiveGraphics();
                GraphicsForm.StartLiveUpdate(true);
            }
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
            if (GraphicsForm != null && GraphicsForm.Visible)
            {
                GraphicsForm.PlayBackStep(hScrollPlayback.Value);
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
    }
}
