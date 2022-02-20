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
        private BindingSource AnalyzeBS = new BindingSource();
        private bool AnalyzerActive = false;        
        private List<ushort> SupportedPids;
        private PidConfig ClipBrd;
        private int keyDelayCounter = 0;

        private void frmLogger_Load(object sender, EventArgs e)
        {
            LogReceivers.Add(txtResult);
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

            CurrentMode = RunMode.NotRunning;
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
            AnalyzerActive = false;
            if (CurrentMode == RunMode.NotRunning)
            {
                btnStartStop.Text = "Start Logging";
            }
        }

        private void TabAnalyzer_Enter(object sender, EventArgs e)
        {
            AnalyzerActive = true;
            if (CurrentMode == RunMode.NotRunning)
            {
                btnStartStop.Text = "Start Analyzer";
            }
        }

        private void DataGridAnalyzer_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void ListProfiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (listProfiles.SelectedItems.Count == 0 || CurrentMode != RunMode.NotRunning)
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
            SetupLogDataGrid();
            AnalyzerActive = false;
            if (CurrentMode == RunMode.NotRunning)
            {
                btnStartStop.Text = "Start Logging";
            }
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
                chkBusFilters.Checked = Properties.Settings.Default.LoggerUseFilters;

                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLogFolder))
                {
                    txtLogFolder.Text = Properties.Settings.Default.LoggerLogFolder;
                }
                else
                {
                    txtLogFolder.Text = Path.Combine(Application.StartupPath, "Logger", "Log");
                }

                comboResponseMode.DataSource = Enum.GetValues(typeof(ResponseTypes));

                if (Properties.Settings.Default.LoggerShowAdvanced)
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLogSeparator))
                        txtLogSeparator.Text = Properties.Settings.Default.LoggerLogSeparator;
                    else
                        txtLogSeparator.Text = ",";
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerResponseMode))
                    {
                        comboResponseMode.Text = Properties.Settings.Default.LoggerResponseMode;
                    }
                    chkPriority.Checked = Properties.Settings.Default.LoggerUsePriority;
                }
                comboBaudRate.Text = Properties.Settings.Default.LoggerBaudRate.ToString();


                DataLogger.PidProfile = new List<LoggerUtils.PidConfig>();
                if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLastProfile) && File.Exists(Properties.Settings.Default.LoggerLastProfile))
                {
                    profileFile = Properties.Settings.Default.LoggerLastProfile;
                    DataLogger.LoadProfile(profileFile);
                    this.Text = "Logger - " + profileFile;
                    CheckMaxPids();
                }
                bsLogProfile.DataSource = DataLogger.PidProfile;
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
                    if (SupportedPids == null || SupportedPids.Contains(pid))
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
                    if (!string.IsNullOrEmpty(OS))
                    {
                        bool supported = false;
                        for (int l=0;l< results[p].Locations.Count;l++)
                        {
                            if (results[p].Locations[l].os == OS)
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
                            if (!string.IsNullOrWhiteSpace(OS))
                            {
                                Location l = results[p].Locations.Where(x => x.os == OS).FirstOrDefault();
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
            Debug.WriteLine(e.Exception);
        }

        private void frmLogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataLogger.stopLogLoop = true;
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
                    LastCalcValues = new string[slothandler.LastPidValues.Length];
                    for (int c = 0; c < LastCalcValues.Length; c++)
                        LastCalcValues[c] = slothandler.LastPidValues[c].ToString(); ;
                }
                else
                {
                    LastCalcValues = slothandler.CalculatePidValues(slothandler.LastPidValues);
                }
                for (int row=0; row< LastCalcValues.Length;row++)
                {
                    dataGridLogData.Rows[row].Cells["Value"].Value = LastCalcValues[row];
                }
                TimeSpan elapsed = DateTime.Now.Subtract(LogStartTime);
                int speed = (int)(slothandler.ReceivedHPRows / elapsed.TotalSeconds);
                int lpSpeed = (int)(slothandler.ReceivedLPRows / elapsed.TotalSeconds);
                string elapsedStr = elapsed.Hours.ToString() + ":" + elapsed.Minutes.ToString() + ":" + elapsed.Seconds.ToString();
                if (chkPriority.Checked)
                {
                    labelProgress.Text = "Elapsed: " + elapsedStr + ", Received:" +Environment.NewLine +  
                        "High Priority: " + slothandler.ReceivedHPRows.ToString() + " rows (" + speed.ToString() + " /s)" + Environment.NewLine +
                        "Low priority: " + slothandler.ReceivedLPRows.ToString() +" rows (" + lpSpeed.ToString() + " /s)";
                }
                else
                {
                    labelProgress.Text = "Elapsed: " + elapsedStr + ", Received: " + slothandler.ReceivedHPRows.ToString() + " rows (" + speed.ToString() + " /s)";
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
            DataLogger.LoadProfile(fname);
            bsLogProfile.DataSource = DataLogger.PidProfile;
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
            DataLogger.SaveProfile(fname);
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
            for (int i=0; i<PidProfile.Count; i++)
            {
                if (!pids.Contains(PidProfile[i].addr))
                {
                    pids.Add(PidProfile[i].addr);
                    bytesTotal += GetElementSize((InDataType)PidProfile[i].DataType);
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
                            if (Connected && !string.IsNullOrEmpty(OS))
                            {
                                List<Location> locations = rp.Locations;
                                if (locations != null)
                                {
                                    Location l = locations.Where(x => x.os == OS).FirstOrDefault();
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

                    DataLogger.PidProfile.Add(pc);
                    ProfileDirty = true;
                    bsLogProfile.DataSource = null;
                    bsLogProfile.DataSource = DataLogger.PidProfile;
                    if (Connected)
                    {
                        if (!radioParamRam.Checked)
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
            if (CurrentMode != RunMode.NotRunning)
                return; //Dont change settings while logging
            dataGridLogData.Columns.Clear();
            dataGridLogData.Columns.Add("Value", "Value");
            if (!chkRawValues.Checked)
                dataGridLogData.Columns.Add("Units", "Units");
            dataGridLogData.Columns[0].ReadOnly = true;
            for (int r=0;r<DataLogger.PidProfile.Count; r++)
            {
                if (chkRawValues.Checked)
                {
                    bool exist = false;
                    for (int row = 0; row < dataGridLogData.Rows.Count - 1; row++)
                    {
                        if (dataGridLogData.Rows[row].HeaderCell.Value.ToString() == PidProfile[r].Address)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        int ind = dataGridLogData.Rows.Add();
                        dataGridLogData.Rows[ind].HeaderCell.Value = PidProfile[r].Address;
                    }
                }
                else
                {
                    int ind = dataGridLogData.Rows.Add();
                    dataGridLogData.Rows[ind].HeaderCell.Value = PidProfile[r].PidName;
                    dataGridLogData.Rows[ind].Cells["Units"].Value = PidProfile[r].Units;
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
            Properties.Settings.Default.LoggerUseFilters = chkBusFilters.Checked;
            Properties.Settings.Default.Save();

        }
        private void StopLogging()
        {
            try
            {
                Logger("Stopping, wait...");
                Application.DoEvents();
                DataLogger.StopLogging();
                btnStartStop.Text = "Start Logging";
                timerShowData.Enabled = false;
                groupLogSettings.Enabled = true;
                groupDTC.Enabled = true;
                btnGetVINCode.Enabled = true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private bool Connect(bool ShowOs = true)
        {
            useBusFilters = chkBusFilters.Checked;
            if (Connected)
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
                LogDevice = CreateSerialDevice(sPort, comboSerialDeviceType.Text, chkFTDI.Checked);
                if (!LogDevice.Initialize(Convert.ToInt32(comboBaudRate.Text)))
                {
                    port.Dispose();
                    LogDevice.Dispose();
                    return false;
                }
            }
            else
            {
                J2534DotNet.J2534Device dev = jDevList[j2534DeviceList.SelectedIndex];
                //passThru.LoadLibrary(dev);
                LogDevice = new J2534Device(dev);
                if (!LogDevice.Initialize(0))
                {
                    LogDevice.Dispose();
                    return false;
                }
            }
            string os = "";
            if (ShowOs)
            {
                os = QueryPcmOS();
            }
            if (!string.IsNullOrEmpty(os))
                labelConnected.Text = "Connected, OS:" + os;
            else
                labelConnected.Text = "Connected";
            Connected = true;
            SaveSettings();
            Application.DoEvents();
            groupHWSettings.Enabled = false;
            return true;
        }


        private void StartLogging()
        {
            try
            {
                labelProgress.Text = "";
                if (DataLogger.PidProfile.Count == 0)
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
                DataLogger.Responsetype = Convert.ToByte(Enum.Parse(typeof(ResponseTypes), comboResponseMode.Text));
                DataLogger.writelog = chkWriteLog.Checked;
                DataLogger.useRawValues = chkRawValues.Checked;
                DataLogger.reverseSlotNumbers = chkReverseSlotNumbers.Checked;
                DataLogger.HighPriority = chkPriority.Checked;
                //PcmLogger.HighPriorityWeight = (int)numHighPriorityWeight.Value;
                if (chkWriteLog.Checked)
                {
                    logfilename = Path.Combine(txtLogFolder.Text, "log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm.ss") + ".csv");
                    if (!DataLogger.CreateLog(logfilename))
                    {
                        return;
                    }
                }
                if (DataLogger.StartLogging())
                {
                    timerShowData.Enabled = true;
                    btnStartStop.Text = "Stop Logging";
                    groupLogSettings.Enabled = false;
                    btnGetVINCode.Enabled = false;
                    CurrentMode = RunMode.LogRunning;
                    groupDTC.Enabled = false;
                }
                else
                {
                    CurrentMode = RunMode.NotRunning;
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
            if (CurrentMode == RunMode.LogRunning)
            {
                StopLogging();
            }
            else if (CurrentMode == RunMode.AnalyzeRunning)
            {
                StopAnalyzer();
            }
            else if (AnalyzerActive)
            {
                StartAnalyzer();
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
                    DataLogger.PidProfile.RemoveAt(removeInd[i]);
                }
                ProfileDirty = true;
                bsLogProfile.DataSource = null;
                bsLogProfile.DataSource = DataLogger.PidProfile;
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
                analyzer = new Analyzer();
                analyzer.RowAnalyzed += Analyzer_RowAnalyzed;
                string devtype = comboSerialDeviceType.Text;
                if (j2534RadioButton.Checked)
                    devtype = "J2534 Device";
                analyzer.StartAnalyzer(devtype, chkHideHeartBeat.Checked);
                btnStartStop.Text = "Stop Analyzer";
                CurrentMode = RunMode.AnalyzeRunning;
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
                    dataGridAnalyzer.Rows[r].Cells[prop.Name].Value = prop.GetValue(e, null);
                }
                dataGridAnalyzer.CurrentCell = dataGridAnalyzer.Rows[r].Cells[0];
                //AddAnalyzerRowToGrid(e); 
            });
        }

        private void StopAnalyzer()
        {
            try
            {
                timerAnalyzer.Enabled = false;
                analyzer.StopAnalyzer();
                btnStartStop.Text = "Start Analyzer";
                CurrentMode = RunMode.NotRunning;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void timerAnalyzer_Tick(object sender, EventArgs e)
        {
            try
            {
/*                if (prevACount < analyzer.aData.Count)
                {
                    //AnalyzeBS.DataSource = analyzer.aData;
                    for (int a = prevACount; a < analyzer.aData.Count; a++)
                    {
                        Analyzer.AnalyzerData ad = analyzer.aData[a];
                        int r = dataGridAnalyzer.Rows.Add();
                        foreach (var prop in ad.GetType().GetProperties())
                        {
                            dataGridAnalyzer.Rows[r].Cells[prop.Name].Value = prop.GetValue(ad, null);
                        }
                        dataGridAnalyzer.CurrentCell = dataGridAnalyzer.Rows[r].Cells[0];
                    }
                    prevACount = analyzer.aData.Count;
                    Application.DoEvents();
                }
*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void filterSupportedPidsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select BIN file");
            if (fName.Length == 0)
                return;
            PcmFile PCM = new PcmFile(fName, true, "");
            OS = PCM.OS;
            PidSearch pidSearch = new PidSearch(PCM);
            if (pidSearch.pidList != null && pidSearch.pidList.Count > 0)
            {
                SupportedPids = new List<ushort>();
                for (int s=0; s < pidSearch.pidList.Count; s++)
                {
                    SupportedPids.Add(pidSearch.pidList[s].pidNumberInt);
                }
                LoadParameters();
            }
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
            dataGridDtcCodes.Rows.Clear();
            Connect();
            //if (!SetLoggingPaused())
            //    return;
            List<DTCCodeStatus> codelist = new List<DTCCodeStatus>();
            if (chkDtcAllModules.Checked)
            {
                codelist = RequestDTCCodes(DeviceId.Broadcast, mode);
            }
            else
            {
                byte module = (byte)comboModule.SelectedValue;
                codelist = RequestDTCCodes(module, mode); 
            }            

            for (int i=0;i<codelist.Count; i++)
            {
                int r = dataGridDtcCodes.Rows.Add();
                dataGridDtcCodes.Rows[r].Cells[0].Value = codelist[i].Module;
                dataGridDtcCodes.Rows[r].Cells[1].Value = codelist[i].Code;
                dataGridDtcCodes.Rows[r].Cells[2].Value = codelist[i].Description;
                dataGridDtcCodes.Rows[r].Cells[3].Value = codelist[i].Status;
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
                ClearTroubleCodes(DeviceId.Broadcast);
            }
            else
            {
                byte module = (byte)comboModule.SelectedValue;
                ClearTroubleCodes(module);
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
                rv = QuerySinglePidValue(pc.addr, pc.DataType);
                if (pc.addr2 > 0)
                {
                    rv2 = QuerySinglePidValue(pc.addr2, pc.Pid2DataType);
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
            LogDevice.Dispose();
            Connected = false;
            labelConnected.Text = "Disconnected";
            groupHWSettings.Enabled = true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (Connected)
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
            if (Connected)
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
            string vin = QueryVIN();
            if (vin.Length > 0)
            {
                Logger("VIN Code:" + vin);
            }
        }

        private void btnClearAnalyzerGrid_Click(object sender, EventArgs e)
        {
            dataGridAnalyzer.Rows.Clear();
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
            if (Connected)
            {
                if (chkBusFilters.Checked)
                {
                    LogDevice.SetLoggingFilter();
                }
                else
                {
                    LogDevice.RemoveFilters();
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
                bsLogProfile.DataSource = PidProfile;
            }
        }

        private void saveProfileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectSaveProfile();
        }

        private void cutRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = dataGridLogProfile.CurrentRow.Index;
            ClipBrd = PidProfile[i];
            PidProfile.RemoveAt(i);
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = PidProfile;
            ProfileDirty = true;
        }

        private void pasteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClipBrd == null)
                return;
            int i = dataGridLogProfile.CurrentRow.Index;
            PidProfile.Insert(i, ClipBrd);
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = PidProfile;
            ProfileDirty = true;
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = dataGridLogProfile.CurrentRow.Index;
            ClipBrd = PidProfile[i];
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
            PidProfile = new List<PidConfig>();
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = PidProfile;
            profileFile = "";
            this.Text = "Logger";
        }
    }
}
