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
        private DateTime LogStartTime;
        private bool ProfileDirty = false;
        //private BindingSource AnalyzeBS = new BindingSource();
        private List<ushort> SupportedPids;
        private PidConfig ClipBrd;
        private int keyDelayCounter = 0;
        public List<J2534DotNet.J2534Device> jDevList;
        private SelectedTab selectedtab = SelectedTab.Logger;

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
            tabAdvanced.Enter += TabAdvanced_Enter;

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
            txtVPWmessages.EnableContextMenu();
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
                    txtVPWmessages.SelectionColor = Color.DarkGreen;
                    if (chkConsoleTimestamps.Checked)
                    {
                        string tStamp = "[" + new DateTime((long)e.Msg.SysTimeStamp).ToString("HH:mm:ss.fff") + "] ";
                        if (chkConsoleUseJ2534Timestamps.Checked)
                        {
                            tStamp += "[" + e.Msg.TimeStamp.ToString() + "] ";
                        }
                        txtVPWmessages.AppendText(tStamp);
                    }
                    txtVPWmessages.AppendText(BitConverter.ToString(e.Msg.GetBytes()).Replace("-", " ") + Environment.NewLine);
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
                txtVPWmessages.SelectionColor = Color.Red;
                if (chkConsoleTimestamps.Checked)
                {
                    string tStamp = "[" + new DateTime((long)e.Msg.SysTimeStamp).ToString("HH:mm:ss.fff") + "] ";
                    if (chkConsoleUseJ2534Timestamps.Checked)
                    {
                        tStamp += "[" + e.Msg.TimeStamp.ToString() + "] ";
                    }
                    txtVPWmessages.AppendText(tStamp);
                }
                txtVPWmessages.AppendText(BitConverter.ToString(e.Msg.GetBytes()).Replace("-", " ") + Environment.NewLine);
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
                    e.Handled = true;
                    byte[] msg = Utility.ToBytes(txtSendBus.Text.Replace(" ", ""));
                    OBDMessage oMsg = new OBDMessage(msg);
                    if (datalogger.LogRunning)
                    {
                        datalogger.QueueCustomCmd(oMsg);
                    }
                    else
                    {
                        if (!datalogger.LogDevice.SendMessage(oMsg, -50))
                        {
                            LoggerBold("Error sending message");
                            return;
                        }
                        OBDMessage rMsg = datalogger.LogDevice.ReceiveMessage();
                        while (rMsg != null)
                            rMsg = datalogger.LogDevice.ReceiveMessage();
                    }
                    txtVPWmessages.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
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
            AddToProfile();
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
                        if (LoadDefaults && !string.IsNullOrEmpty(Properties.Settings.Default.LoggerFTDIPort) && comboSerialPort.Items.Contains(Properties.Settings.Default.LoggerFTDIPort))
                            comboSerialPort.Text = Properties.Settings.Default.LoggerFTDIPort;
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
                                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerComPort) && comboSerialPort.Items.Contains(Properties.Settings.Default.LoggerComPort))
                                    comboSerialPort.Text = Properties.Settings.Default.LoggerComPort;
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
                }

                if (LoadDefaults)
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerJ2534Device))
                    {
                        j2534DeviceList.Text = Properties.Settings.Default.LoggerJ2534Device;
                    }
                    else if (jDevList.Count > 0)
                    {
                        j2534DeviceList.Text = jDevList.FirstOrDefault().Name;
                    }
                    j2534RadioButton.Checked = Properties.Settings.Default.LoggerUseJ2534;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (Properties.Settings.Default.MainWindowPersistence)
                {
                    if (Properties.Settings.Default.LoggerWindowSize.Width > 0 || Properties.Settings.Default.LoggerWindowSize.Height > 0)
                    {
                        this.WindowState = Properties.Settings.Default.LoggerWindowState;
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        this.Location = Properties.Settings.Default.LoggerWindowPosition;
                        this.Size = Properties.Settings.Default.LoggerWindowSize;
                    }

                }

                chkFTDI.Checked = Properties.Settings.Default.LoggerUseFTDI;
                comboSerialDeviceType.Items.Add(AvtDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(AvtLegacyDevice.DeviceType);
                comboSerialDeviceType.Items.Add(ElmDevice.DeviceType);
                //comboSerialDeviceType.Items.Add(LegacyElmDeviceImplementation.DeviceType);
                comboSerialDeviceType.Items.Add(OBDXProDevice.DeviceType);
                comboSerialDeviceType.Text = Properties.Settings.Default.LoggerSerialDeviceType;

                LoadPorts(true);

                //chkAdvanced.Checked = Properties.Settings.Default.LoggerShowAdvanced;
                chkVPWFilters.Checked = Properties.Settings.Default.LoggerUseFilters;
                chkEnableConsole.Checked = Properties.Settings.Default.LoggerEnableConsole;
                chkConsoleTimestamps.Checked = Properties.Settings.Default.LoggerConsoleTimestamps;
                chkConsoleUseJ2534Timestamps.Checked = Properties.Settings.Default.LoggerConsoleJ2534Timestamps;
                numConsoleScriptDelay.Value = Properties.Settings.Default.LoggerScriptDelay;

                if (Properties.Settings.Default.LoggerConsoleFont != null)
                {
                    txtVPWmessages.Font = Properties.Settings.Default.LoggerConsoleFont;
                }

                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLogFolder))
                {
                    txtLogFolder.Text = Properties.Settings.Default.LoggerLogFolder;
                }
                else
                {
                    txtLogFolder.Text = Path.Combine(Application.StartupPath, "Logger", "Log");
                }

                comboResponseMode.DataSource = Enum.GetValues(typeof(ResponseTypes));

                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLogSeparator))
                    txtLogSeparator.Text = Properties.Settings.Default.LoggerLogSeparator;
                else
                    txtLogSeparator.Text = ",";
                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerResponseMode))
                {
                    comboResponseMode.Text = Properties.Settings.Default.LoggerResponseMode;
                }
                chkPriority.Checked = Properties.Settings.Default.LoggerUsePriority;
                comboBaudRate.Text = Properties.Settings.Default.LoggerBaudRate.ToString();


                datalogger.PidProfile = new List<LoggerUtils.PidConfig>();
                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLastProfile) && File.Exists(Properties.Settings.Default.LoggerLastProfile))
                {
                    profileFile = Properties.Settings.Default.LoggerLastProfile;
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
            Debug.WriteLine("Closing frmLogger");
            datalogger.stopLogLoop = true;
            if (ProfileDirty)
            {
                DialogResult dialogResult = MessageBox.Show("Save profile modifications?", "Save profile", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SelectSaveProfile();
                }
            }
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.LoggerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.LoggerWindowPosition = this.Location;
                    Properties.Settings.Default.LoggerWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.LoggerWindowPosition = this.RestoreBounds.Location;
                    Properties.Settings.Default.LoggerWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.Save();
            }

            Thread.Sleep(100);
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
                TimeSpan elapsed = DateTime.Now.Subtract(LogStartTime);
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

        private void AddToProfile()
        {
            try
            {
                if (dataGridPidNames.SelectedRows.Count == 0)
                    return;
                
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

                    datalogger.PidProfile.Add(pc);
                    ProfileDirty = true;
                    bsLogProfile.DataSource = null;
                    bsLogProfile.DataSource = datalogger.PidProfile;
                    if (datalogger.Connected)
                    {
                        QueryPid(pc);
                    }
                    CheckMaxPids();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddToProfile();
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
                Properties.Settings.Default.LoggerFTDIPort = comboSerialPort.Text;
            else
                Properties.Settings.Default.LoggerComPort = comboSerialPort.Text;
            Properties.Settings.Default.LoggerUseJ2534 = j2534RadioButton.Checked;
            Properties.Settings.Default.LoggerJ2534Device = j2534DeviceList.Text;
            Properties.Settings.Default.LoggerSerialDeviceType = comboSerialDeviceType.Text;
            Properties.Settings.Default.LoggerBaudRate = Convert.ToInt32(comboBaudRate.Text);

            if (txtLogFolder.Text.Length > 0)
            {
                Properties.Settings.Default.LoggerLogFolder = txtLogFolder.Text;
                Properties.Settings.Default.LoggerLogSeparator = txtLogSeparator.Text;
            }
            Properties.Settings.Default.LoggerLastProfile = profileFile;
            Properties.Settings.Default.LoggerResponseMode = comboResponseMode.Text;
            Properties.Settings.Default.LoggerUseFTDI = chkFTDI.Checked;
            //Properties.Settings.Default.LoggerShowAdvanced = chkAdvanced.Checked;
            Properties.Settings.Default.LoggerUsePriority = chkPriority.Checked;
            Properties.Settings.Default.LoggerUseFilters = chkVPWFilters.Checked;
            Properties.Settings.Default.LoggerEnableConsole = chkEnableConsole.Checked;
            Properties.Settings.Default.LoggerConsoleTimestamps = chkConsoleTimestamps.Checked;
            Properties.Settings.Default.LoggerConsoleJ2534Timestamps = chkConsoleUseJ2534Timestamps.Checked;
            Properties.Settings.Default.LoggerConsoleFont = txtVPWmessages.Font;
            Properties.Settings.Default.LoggerScriptDelay = (int) numConsoleScriptDelay.Value;
            Properties.Settings.Default.Save();

        }
        private void StopLogging()
        {
            try
            {
                Logger("Stopping, wait...");
                Application.DoEvents();
                datalogger.StopLogging();
                btnStartStop.Text = "Start Logging";
                timerShowData.Enabled = false;
                groupLogSettings.Enabled = true;
                groupDTC.Enabled = true;
                //btnGetVINCode.Enabled = true;
                if (chkEnableConsole.Checked)
                {
                    datalogger.StartReceiveLoop();
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
                Logger("Connecting...");
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
                }
                datalogger.LogDevice.MsgReceived += LogDevice_DTC_MsgReceived;
                if (chkEnableConsole.Checked)
                {
                    datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                    datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                }
                if (!datalogger.LogDevice.Initialize(Convert.ToInt32(comboBaudRate.Text)))
                {
                    datalogger.port.Dispose();
                    datalogger.LogDevice.Dispose();
                    return false;
                }
                datalogger.LogDevice.Enable4xReadWrite = true;

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
                if (chkEnableConsole.Checked)
                {
                    datalogger.StartReceiveLoop();
                    datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                    datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                }
                Application.DoEvents();
                groupHWSettings.Enabled = false;
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold("Connection failed. Check settings");
                return false;
            }
        }


        private void StartLogging()
        {
            try
            {
                labelProgress.Text = "";
                if (datalogger.PidProfile.Count == 0)
                {
                    Logger("No profile configured");
                    return;
                }
                if (!Connect())
                {
                    return;
                }
                SetupLogDataGrid();
                LogStartTime = DateTime.Now;
                datalogger.Responsetype = Convert.ToByte(Enum.Parse(typeof(ResponseTypes), comboResponseMode.Text));
                datalogger.writelog = chkWriteLog.Checked;
                datalogger.useRawValues = chkRawValues.Checked;
                datalogger.reverseSlotNumbers = chkReverseSlotNumbers.Checked;
                datalogger.HighPriority = chkPriority.Checked;
                //PcmLogger.HighPriorityWeight = (int)numHighPriorityWeight.Value;
                if (chkWriteLog.Checked)
                {
                    logfilename = Path.Combine(txtLogFolder.Text, "log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm.ss") + ".csv");
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
                        groupDTC.Enabled = false;
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
                StopLogging();
            }
            else
            {
                StartLogging();
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
            }
        }

        private void j2534RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.j2534RadioButton.Checked)
            {
                serialOptionsGroupBox.Enabled = false;
                j2534OptionsGroupBox.Enabled = true;
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
            if (txtLogSeparator.Text.Length > 1)
            {
                txtLogSeparator.Text = txtLogSeparator.Text.Substring(0, 1);
            }
        }

        private void StartAnalyzer()
        {
            try
            {
                if (!Connect(false))
                {
                    return;
                }
                datalogger.StopReceiveLoop();
                analyzer = new Analyzer();
                analyzer.RowAnalyzed += Analyzer_RowAnalyzed;
                string devtype = comboSerialDeviceType.Text;
                if (j2534RadioButton.Checked)
                    devtype = "J2534 Device";
                analyzer.StartAnalyzer(devtype, chkHideHeartBeat.Checked);
                btnStartStopAnalyzer.Text = "Stop Analyzer";
                datalogger.AnalyzerRunning = true;
                groupDTC.Enabled = false;
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

        private void StopAnalyzer()
        {
            try
            {
                analyzer.StopAnalyzer();
                btnStartStopAnalyzer.Text = "Start Analyzer";
                datalogger.AnalyzerRunning = false;
                groupDTC.Enabled = true;
                if (chkEnableConsole.Checked)
                {
                    datalogger.StartReceiveLoop();
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
            Connect();
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
                if (e.Msg.Length > 5 && e.Msg.GetBytes()[1] == DeviceId.Tool && e.Msg.GetBytes()[3] == 0x59)
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

            if (chkDtcAllModules.Checked)
            {
                datalogger.ClearTroubleCodes(DeviceId.Broadcast);
            }
            else
            {
                byte module = (byte)comboModule.SelectedValue;
                datalogger.ClearTroubleCodes(module);
            }
        }

        private void QueryPid(PidConfig pc)
        {
            try
            {
                if (pc == null)
                {
                    int r = dataGridLogProfile.CurrentCell.RowIndex;
                    if (r < 0)
                        return;
                    pc = (PidConfig)dataGridLogProfile.Rows[r].DataBoundItem;
                }

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
            QueryPid(null);
        }

        private void Disconnect()
        {
            datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
            datalogger.LogDevice.MsgReceived -= LogDevice_DTC_MsgReceived;

            if (chkEnableConsole.Checked || datalogger.ReceiveLoopRunning)
            {
                datalogger.StopReceiveLoop();
                datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
                datalogger.LogDevice.MsgSent -= LogDevice_MsgSent;
            }
            if (datalogger.LogRunning)
            {
                StopLogging();
                Thread.Sleep(500);
            }
            if (datalogger.AnalyzerRunning)
            {
                StopAnalyzer();
                Thread.Sleep(500);
            }
            datalogger.LogDevice.Dispose();
            datalogger.Connected = false;
            labelConnected.Text = "Disconnected - OS: " + datalogger.OS;
            groupHWSettings.Enabled = true;
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
                datalogger.QueryVIN();
            }
            else
            {
                datalogger.QueueVINRequest();
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
            if (keyDelayCounter > Properties.Settings.Default.keyPressWait100ms)
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
                    datalogger.StartReceiveLoop();
                    datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                    datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                }
                else
                {
                    datalogger.StopReceiveLoop();
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
            datalogger.UploadScript(fName);
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
                StopAnalyzer();
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
            if (chkConsole4x.Checked)
            {
                if (!datalogger.Connected)
                {
                    Logger("Not conneted");
                    return;
                }
                datalogger.LogDevice.SetVpwSpeed(VpwSpeed.FourX);
            }
            else if (datalogger.Connected)
            {
                datalogger.LogDevice.SetVpwSpeed(VpwSpeed.Standard);
            }
        }

    }
}
