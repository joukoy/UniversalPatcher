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
using System.IO;
using System.Xml.Linq;
using static UniversalPatcher.PcmLogger;

namespace UniversalPatcher
{
    public partial class frmLogger : Form
    {
        public frmLogger()
        {
            InitializeComponent();
        }

        private BindingSource bsLogProfile = new BindingSource();
        //private BindingSource bsPidNames = new BindingSource();
        private bool running = false;
        private List<Parameter> parameters = new List<Parameter>();
        private string profileFile;
        private List<ReadValue> readvalues;
        private List<int> receivedPids;
        private string logfilename;

        private void frmLogger_Load(object sender, EventArgs e)
        {
            LogReceivers.Add(txtResult);
            Application.DoEvents();

            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                Debug.WriteLine("No ports");
            }
            else
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    comboSerialPort.Items.Add(ports[i]);
                }
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerPort))
                comboSerialPort.Text = Properties.Settings.Default.LoggerPort;
            else
                btnStartStop.Enabled = false;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLastProfile))
            {
                profileFile = Properties.Settings.Default.LoggerLastProfile;
                PcmLogger.LoadProfile(profileFile);
                this.Text = "Logger - " + profileFile;
            }
            else
            {
                PcmLogger.PidProfile = new List<PcmLogger.PidConfig>();
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LoggerLogFolder))
            {
                txtLogFolder.Text = Properties.Settings.Default.LoggerLogFolder;
            }
            else
            {
                txtLogFolder.Text = Path.Combine(Application.StartupPath, "Logger", "Log");
            }
            txtLogSeparator.Text = Properties.Settings.Default.LoggerLogSeparator;

            comboDeviceType.DataSource = Enum.GetValues(typeof(PcmLogger.DeviceType));
            if (string.IsNullOrEmpty(Properties.Settings.Default.LoggerDeviceType))
                comboDeviceType.Text = Properties.Settings.Default.LoggerDeviceType;

            bsLogProfile.DataSource = PcmLogger.PidProfile;
            dataGridLogProfile.DataSource = bsLogProfile;

            dataGridLogProfile.DataError += DataGridLogProfile_DataError;
            dataGridLogProfile.DataBindingComplete += DataGridLogProfile_DataBindingComplete;
            dataGridPidNames.DataError += DataGridPidNames_DataError;

            LoadStandardParameters();

        }

        private void DataGridPidNames_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        private void LoadStandardParameters()
        {
            try
            {
                string fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Standard.xml");
                XDocument xml = XDocument.Load(fName);
                parameters = new List<Parameter>();
                foreach (XElement parameterElement in xml.Root.Elements("Parameter"))
                {
                    Parameter p = new Parameter();
                    p.Id = parameterElement.Attribute("id").Value;
                    p.Name = parameterElement.Attribute("name").Value;
                    p.Description = parameterElement.Attribute("description").Value;
                    p.DataType = parameterElement.Attribute("storageType").Value;
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
                    parameters.Add(p);
                }

                Parameter tmpPar = new Parameter();
                foreach (var prop in tmpPar.GetType().GetProperties())
                {
                    dataGridPidNames.Columns.Add(prop.Name, prop.Name);
                }
                for (int p = 0; p < parameters.Count; p++)
                {
                    int row = dataGridPidNames.Rows.Add();
                    foreach (var prop in tmpPar.GetType().GetProperties())
                    {
                        if (prop.Name == "Conversions")
                        {
                            DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                            dgc.DataSource = parameters[p].Conversions;
                            dgc.ValueMember = "expression";
                            dgc.DisplayMember = "units";
                            dataGridPidNames.Rows[row].Cells[prop.Name] = dgc;
                            Application.DoEvents();
                            dataGridPidNames.Rows[row].Cells[prop.Name].Value = parameters[p].Conversions[0].Expression;
                        }
                        else
                        {
                            dataGridPidNames.Rows[row].Cells[prop.Name].Value = prop.GetValue(parameters[p], null);
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
                dataGridLogProfile.Columns[0].Width = 70;
                dataGridLogProfile.Columns[1].Width = 250;
                UseComboBoxForEnums(dataGridLogProfile);
                //dataGridLogProfile.Columns[0].DefaultCellStyle.Format = "X2";
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
            PcmLogger.StopLogging();
        }


        private void timerGetData_Tick(object sender, EventArgs e)
        {
            try
            {
                bool first = true;
                while (true)
                {
                    byte[] data = PcmLogger.ReadData();
                    if (data == null)
                        return;
                    //Logger(BitConverter.ToString(data));
                    List<ReadValue> rvs = PcmLogger.ParseMessage(data);
                    if (rvs == null)
                        return;
                    for (int i = 0; i < rvs.Count; i++)
                    {
                        //Logger(rv[i].PidName + ": " + rv[i].PidValue.ToString());
                        ReadValue rv = rvs[i];
                        readvalues.Add(rv);

                        if (!receivedPids.Contains(rv.index))
                            receivedPids.Add(rv.index);
                        if (receivedPids.Count >= PcmLogger.PidProfile.Count)
                        {
                            //All pids received
                            receivedPids = new List<int>();
                            if (chkWriteLog.Checked)
                            {
                                PcmLogger.WriteLog(readvalues);
                            }
                            if (first)
                            {
                                first = false;
                                for (int row = 0; row < dataGridLogData.Rows.Count - 1; row++)
                                {
                                    ReadValue x = readvalues.Where(n => n.PidName == dataGridLogData.Rows[row].HeaderCell.Value.ToString()).FirstOrDefault();
                                    if (x != null)
                                    {
                                        dataGridLogData.Rows[row].Cells["Value"].Value = x.PidValue;
                                    }
                                }

                            }
                            readvalues = new List<ReadValue>();
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

        private void timerPresent_Tick(object sender, EventArgs e)
        {
            PcmLogger.SendTesterPresent();
        }

        private void SelectProfile()
        {
            string defname = Path.Combine(Application.StartupPath, "Logger", "profile1.xml");
            if (!string.IsNullOrEmpty(profileFile))
                defname = profileFile;
            string fname = SelectFile("Select profile file", XmlFilter);
            if (fname.Length == 0)
                return;
            profileFile = fname;
            this.Text = "Logger - " + profileFile;
            PcmLogger.LoadProfile(fname);
            bsLogProfile.DataSource = PcmLogger.PidProfile;
            dataGridLogProfile.DataSource = bsLogProfile;
            
        }

        private void SelectSaveProfile()
        {
            string defname = Path.Combine(Application.StartupPath, "Logger", "profile1.xml");
            if (!string.IsNullOrEmpty(profileFile))
                defname = profileFile;
            string fname = SelectSaveFile(XmlFilter, defname);
            if (fname.Length == 0)
                return;
            profileFile = fname;
            this.Text = "Logger - " + profileFile;
            PcmLogger.SaveProfile(fname);
        }

        private void loadProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectProfile();
        }

        private void btnBrowseProfile_Click(object sender, EventArgs e)
        {
            SelectProfile();
        }

        private void saveProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectSaveProfile();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridPidNames.SelectedRows.Count == 0)
                    return;
                DataGridViewRow dgr = dataGridPidNames.SelectedRows[0];
                PidConfig pc = new PidConfig();
                pc.PidName = dgr.Cells["Name"].Value.ToString();
                ushort pidNr;
                if (HexToUshort(dgr.Cells["Id"].Value.ToString(), out pidNr))
                    pc.pidnr = pidNr;
                //pc.Math = pi.ConversionFactor;
                pc.DataType = (PidDataType)Enum.Parse(typeof(PidDataType), dgr.Cells["DataType"].Value.ToString());
                pc.Math = dgr.Cells["Conversions"].Value.ToString();
                pc.Units = dgr.Cells["Conversions"].FormattedValue.ToString();
                PcmLogger.PidProfile.Add(pc);
                bsLogProfile.DataSource = null;
                bsLogProfile.DataSource = PcmLogger.PidProfile;
            }
            catch(Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void SetupLogDataGrid()
        {
            dataGridLogData.Columns.Clear();
            dataGridLogData.Columns.Add("Value", "Value");
            for (int r=0;r<PcmLogger.PidProfile.Count; r++)
            {
                int ind = dataGridLogData.Rows.Add();
                dataGridLogData.Rows[ind].HeaderCell.Value = PidProfile[r].PidName;
            }
            dataGridLogData.AutoResizeColumns();
            dataGridLogData.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (running)
            {
                btnStartStop.Text = "Start";
                timerPresent.Enabled = false;
                timerGetData.Enabled = false;
                PcmLogger.StopLogging();
            }
            else
            {
                if (PcmLogger.PidProfile.Count == 0)
                {
                    Logger("No profile configured");
                    return;
                }
                btnStartStop.Text = "Stop";
                Properties.Settings.Default.LoggerPort = comboSerialPort.Text;
                Properties.Settings.Default.LoggerDeviceType = comboDeviceType.Text;

                if (txtLogFolder.Text.Length > 0 )
                {
                    Properties.Settings.Default.LoggerLogFolder = txtLogFolder.Text;
                    Properties.Settings.Default.LoggerLogSeparator = txtLogSeparator.Text;
                    logfilename = Path.Combine(txtLogFolder.Text, "log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".csv");
                    if (chkWriteLog.Checked)
                    {
                        PcmLogger.CreateLog(logfilename);
                    }
                }
                Properties.Settings.Default.Save();
                receivedPids = new List<int>();
                readvalues = new List<ReadValue>();
                SetupLogDataGrid();
                Properties.Settings.Default.LoggerLastProfile = profileFile;
                Properties.Settings.Default.Save();
                PcmLogger.InitalizeDevice(comboSerialPort.Text,comboDeviceType.Text);
                PcmLogger.StartLogging(chk4XMode.Checked);
                timerPresent.Enabled = true;
                timerGetData.Enabled = true;
                this.FormClosing += frmLogger_FormClosing;
            }
            running = !running;
        }

        private void comboSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboSerialPort.Text))
                btnStartStop.Enabled = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int row = 0;
            if (dataGridLogProfile.SelectedRows.Count > 0)
            {
                row = dataGridLogProfile.SelectedRows[0].Index;
            }
            else
            {
                if (dataGridLogProfile.SelectedCells.Count == 0)
                {
                    return;
                }
                row = dataGridLogProfile.SelectedCells[0].RowIndex;
            }

            PcmLogger.PidProfile.RemoveAt(row);
            bsLogProfile.DataSource = null;
            bsLogProfile.DataSource = PcmLogger.PidProfile;
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
    }
}
