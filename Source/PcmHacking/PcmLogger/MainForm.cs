//#define Vpw4x

using PcmLibraryWindowsForms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace PcmHacking
{
    public partial class MainForm : Form, ILogger
    {
        private bool saving;
        private object loggingLock = new object();
        private bool logStopRequested;
        private TaskScheduler uiThreadScheduler;
        private uint osid;

        private const string appName = "PCM Logger";
        private const string defaultFileName = "New Profile";
        private string fileName = defaultFileName;
        private Dictionary<string, DataGridViewRow> parameterIdsToRows;
        private ParameterDatabase database;
        private bool suspendSelectionEvents = true;
        private BindingSource bsEditParams = new BindingSource();
        private BindingSource bsEditUnits = new BindingSource();
        private LogProfile currentProfile = new LogProfile();
        private string currentProfilePath = null;
        private bool currentProfileIsDirty = false;
        private const string fileFilter = "Log Profiles (*.LogProfile)|*.LogProfile|All Files|*.*";
        private const string selectAnotherDevice = "Select another device.";

        public virtual void StatusUpdateActivity(string activity) { }
        public virtual void StatusUpdateTimeRemaining(string remaining) { }
        public virtual void StatusUpdatePercentDone(string percent) { }
        public virtual void StatusUpdateRetryCount(string retries) { }
        public virtual void StatusUpdateProgressBar(double completed, bool visible) { }
        public virtual void StatusUpdateKbps(string Kbps) { }
        public virtual void StatusUpdateReset() { }
        //public virtual void ResetLogs() { }

        //public virtual string GetAppNameAndVersion() { return "MainFormBase.GetAppNameAndVersion is not implemented"; }

        //protected virtual void EnableInterfaceSelection() { }
        //protected virtual void EnableUserInput() { }
        //protected virtual void DisableUserInput() { }


        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Generate a file name for the current log file.
        /// </summary>
        /// 

        /// <summary>
        /// Impolite, but it needs to be short enough to fit in the device-description box.
        /// </summary>

        /// <summary>
        /// The Vehicle object is our interface to the car. It has the device, the message generator, and the message parser.
        /// </summary>
        private Vehicle vehicle;
        protected Vehicle Vehicle { get { return this.vehicle; } }

        /// <summary>
        /// Handle clicking the "Select Interface" button
        /// </summary>
        /// <returns></returns>
        public bool HandleSelectButtonClick()
        {
            using (DevicePicker picker = new DevicePicker(this))
            {
                DialogResult result = picker.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (picker.DeviceCategory == DeviceConfiguration.Constants.DeviceCategorySerial)
                    {
                        if (string.IsNullOrEmpty(picker.SelectedSerialPort))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(picker.SerialPortDeviceType))
                        {
                            return false;
                        }
                    }

                    if (this.vehicle != null)
                    {
                        Debug.WriteLine("Disposing vehicle!");
                        this.vehicle.Dispose();
                        Thread.Sleep(250);
                    }

                    if (picker.DeviceCategory == DeviceConfiguration.Constants.DeviceCategoryJ2534)
                    {
                        if (string.IsNullOrEmpty(picker.J2534DeviceType))
                        {
                            return false;
                        }
                    }

                    DeviceConfiguration.Settings.Enable4xReadWrite = picker.Enable4xReadWrite;
                    DeviceConfiguration.Settings.EnablePassiveMode = picker.EnablePassiveMode;
                    DeviceConfiguration.Settings.DeviceCategory = picker.DeviceCategory;
                    DeviceConfiguration.Settings.J2534DeviceType = picker.J2534DeviceType;
                    DeviceConfiguration.Settings.SerialPort = picker.SelectedSerialPort;
                    DeviceConfiguration.Settings.SerialPortDeviceType = picker.SerialPortDeviceType;
                    DeviceConfiguration.Settings.Save();
                    return this.ResetDevice();
                }
            }
            return false;
        }

        /// <summary>
        /// Close the old interface device and open a new one.
        /// </summary>
        protected bool ResetDevice()
        {
            if (this.vehicle != null)
            {
                this.vehicle.Dispose();
                this.vehicle = null;
            }

            Device device = DeviceFactory.CreateDeviceFromConfigurationSettings(this);
            if (device == null)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.NoDeviceSelected();
                    this.SetSelectedDeviceText(selectAnotherDevice);
                    this.DisableUserInput();
                    this.EnableInterfaceSelection();
                });
                return false;
            }

            this.Invoke((MethodInvoker)delegate ()
            {
                this.SetSelectedDeviceText("Connecting, please wait...");
            });

            Protocol protocol = new Protocol();
            this.vehicle = new Vehicle(
                device,
                protocol,
                this,
                new ToolPresentNotifier(device, protocol, this),
                DeviceConfiguration.Settings.EnablePassiveMode);

            if (!this.InitializeCurrentDevice())
            {
                this.vehicle = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initialize the current device.
        /// </summary>
        protected bool InitializeCurrentDevice()
        {
            if (this.vehicle == null)
            {
                return false;
            }

            this.Invoke((MethodInvoker)delegate ()
            {
                this.DisableUserInput();
                this.ResetLogs();
            });

            this.AddUserMessage(GetAppNameAndVersion());

            try
            {
                // TODO: this should not return a boolean, it should just throw 
                // an exception if it is not able to initialize the device.
                bool initializationTask = this.vehicle.ResetConnection();

                if (!initializationTask)
                {
                    this.AddUserMessage("Unable to initialize " + this.vehicle.DeviceDescription);

                    this.Invoke((MethodInvoker)delegate ()
                    {
                        this.NoDeviceSelected();
                        this.SetSelectedDeviceText(selectAnotherDevice);
                        this.EnableInterfaceSelection();
                    });
                    return false;
                }
            }
            catch (Exception exception)
            {
                this.AddUserMessage("Unable to initialize " + this.vehicle.DeviceDescription);

                this.AddDebugMessage(exception.ToString());
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.NoDeviceSelected();
                    this.SetSelectedDeviceText(selectAnotherDevice);
                    this.EnableInterfaceSelection();
                });
                return false;
            }

            this.Invoke((MethodInvoker)delegate ()
            {
                if (!this.vehicle.Supports4X)
                {
                    DeviceConfiguration.Settings.Enable4xReadWrite = true;
                    DeviceConfiguration.Settings.Save();
                }
                this.vehicle.Enable4xReadWrite = DeviceConfiguration.Settings.Enable4xReadWrite;
            });

            this.ValidDeviceSelectedAsync(this.vehicle.DeviceDescription);

            this.Invoke((MethodInvoker)delegate ()
            {
                this.EnableUserInput();
            });
            return true;
        }

        private string GenerateLogFilePath()
        {
            string file = DateTime.Now.ToString("yyyyMMdd_HHmm") +
                "_" +
                this.fileName +
                ".csv";
            return Path.Combine(Properties.Settings.Default.LogDirectory, file);
        }

        private void SetDirtyFlag(bool newValue)
        {
            if (newValue)
            {
                if (this.Text.EndsWith("*"))
                {
                    if (!this.currentProfileIsDirty)
                    {
                        // This would be a bug - set breakpoint here.
                        this.Text.EndsWith("*");
                    }
                }
                else
                {
                    this.Text = this.Text + "*";
                }
            }
            else
            {
                if (this.Text.EndsWith("*") && this.Text != appName)
                {
                    this.Text = this.Text.Substring(0, this.Text.Length - 1);
                }
                else
                {
                    if (this.currentProfileIsDirty)
                    {
                        // This would be a bug - set breakpoint here.
                        this.Text.EndsWith("*");
                    }
                }
            }

            this.currentProfileIsDirty = newValue;
        }

        private void SetFileName(string fileName)
        {
            this.fileName = fileName;
            this.Text = appName + " - " + fileName;
        }


        private void newButton_Click(object sender, EventArgs e)
        {
            if (this.currentProfileIsDirty)
            {
                if (this.SaveIfNecessary() == DialogResult.Cancel)
                {
                    return;
                }
            }

            this.SetFileName(defaultFileName);
            this.currentProfilePath = null;

            this.SetDirtyFlag(false);

            this.ResetProfile();
            this.UpdateGridFromProfile();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (this.currentProfileIsDirty)
            {
                if (this.SaveIfNecessary() == DialogResult.Cancel)
                {
                    return;
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = fileFilter;
            dialog.Multiselect = false;
            dialog.Title = "Open Log Profile";
            dialog.ValidateNames = true;

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.OpenProfile(dialog.FileName);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (currentProfilePath == null)
            {
                saveAsButton_Click(sender, e);
                return;
            }

            LogProfileWriter.Write(this.currentProfile, this.currentProfilePath);
            this.SetDirtyFlag(false);
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            this.ShowSaveAs();
        }

        private DialogResult SaveIfNecessary()
        {
            DialogResult result = MessageBox.Show(
                this,
                "Would you like to save the current profile before continuing?",
                "The current profile has changed.",
                MessageBoxButtons.YesNoCancel);

            switch (result)
            {
                case DialogResult.Yes:
                    if (string.IsNullOrEmpty(currentProfilePath))
                    {
                        if (this.ShowSaveAs() == DialogResult.Cancel)
                        {
                            return DialogResult.Cancel;
                        }

                        return DialogResult.OK;
                    }
                    else
                    {
                        this.saveButton_Click(this, new EventArgs());
                        return DialogResult.OK;
                    }

                case DialogResult.Cancel:
                    return DialogResult.Cancel;

                default: // DialogResult.No
                    return DialogResult.OK;
            }
        }

        private DialogResult ShowSaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = fileFilter;
            dialog.OverwritePrompt = true;
            dialog.AddExtension = true;
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.currentProfilePath = dialog.FileName;
                this.SetFileName(Path.GetFileNameWithoutExtension(this.currentProfilePath));

                try
                {
                    LogProfileWriter.Write(this.currentProfile, this.currentProfilePath);
                    this.SetDirtyFlag(false);
                }
                catch (Exception exception)
                {
                    this.AddDebugMessage(exception.ToString());
                    this.AddUserMessage(exception.Message);
                }

                foreach (PathDisplayAdapter adapter in this.profileList.Items)
                {
                    if (adapter.Path == this.currentProfilePath)
                    {
                        this.profileList.Items.Remove(adapter);
                        break;
                    }
                }

                this.profileList.Items.Insert(0, new PathDisplayAdapter(this.currentProfilePath));
            }

            return result;
        }

        private void ResetProfile()
        {
            this.currentProfile = new LogProfile();
        }

        private void OpenProfile(string path)
        {
            bool alreadyInList = false;
            foreach (PathDisplayAdapter adapter in this.profileList.Items)
            {
                if (adapter.Path == path)
                {
                    alreadyInList = true;
                    break;
                }
            }

            if (!alreadyInList)
            {
                PathDisplayAdapter newAdapter = new PathDisplayAdapter(path);
                this.profileList.Items.Insert(0, newAdapter);
            }

            this.currentProfilePath = path;
            this.SetFileName(Path.GetFileNameWithoutExtension(this.currentProfilePath));

            LogProfileReader reader = new LogProfileReader(this.database, this.osid, this);
            this.currentProfile = reader.Read(this.currentProfilePath);
            Properties.Settings.Default.LastProfile = this.currentProfilePath;
            this.UpdateGridFromProfile();
            this.SetDirtyFlag(false);
        }

        private void LoadProfileHistory()
        {
            if (Properties.Settings.Default.RecentProfiles != null)
            {
                foreach (string path in Properties.Settings.Default.RecentProfiles)
                {
                    if (File.Exists(path) && !this.profileList.Items.Contains(path))
                    {
                        PathDisplayAdapter adapter = new PathDisplayAdapter(path);
                        this.profileList.Items.Add(adapter);
                    }
                }
            }
        }

        private void SaveProfileHistory()
        {
            StringCollection paths = Properties.Settings.Default.RecentProfiles;
            if (paths == null)
            {
                paths = new StringCollection();
            }

            paths.Clear();

            foreach (PathDisplayAdapter adapter in this.profileList.Items)
            {
                paths.Add(adapter.Path);
            }

            Properties.Settings.Default.RecentProfiles = paths;
            Properties.Settings.Default.Save();
        }

        private void profileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.profileList.SelectedIndex == -1)
            {
                this.removeProfileButton.Enabled = false;
                return;
            }
            else
            {
                this.removeProfileButton.Enabled = true;
            }

            if (this.currentProfileIsDirty)
            {
                if (this.SaveIfNecessary() == DialogResult.Cancel)
                {
                    this.profileList.ClearSelected();
                    return;
                }
            }

            PathDisplayAdapter adapter = (PathDisplayAdapter)this.profileList.SelectedItem;
            if (adapter == null)
            {
                return;
            }

            this.OpenProfile(adapter.Path);
        }

        private void removeProfileButton_Click(object sender, EventArgs e)
        {
            int index = this.profileList.SelectedIndex;
            this.profileList.Items.RemoveAt(this.profileList.SelectedIndex);

            if (index >= this.profileList.Items.Count)
            {
                index = this.profileList.Items.Count - 1;
            }

            this.profileList.SelectedIndex = index;
            this.profileList.Focus();
        }

        private void MoveProfileToTop(string path)
        {
            foreach (PathDisplayAdapter adapter in this.profileList.Items)
            {
                if (adapter.Path == path)
                {
                    this.profileList.Items.Remove(adapter);
                    break;
                }
            }

            PathDisplayAdapter newAdapter = new PathDisplayAdapter(path);
            this.profileList.Items.Insert(0, newAdapter);
        }

        private void FillParameterGrid()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appDirectory = Path.GetDirectoryName(appPath);

            this.database = new ParameterDatabase(Path.Combine(appDirectory, "Logger"));

            string errorMessage;
            if (!this.database.TryLoad(out errorMessage))
            {
                throw new InvalidDataException("Unable to load parameters from XML: " + errorMessage);
            }

            this.parameterIdsToRows = new Dictionary<string, DataGridViewRow>();

            foreach (Parameter parameter in this.database.Parameters)
            {
                if (!parameter.IsSupported(osid))
                {
                    continue;
                }

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.parameterGrid);
                row.Cells[0].Value = false; // enabled
                row.Cells[1].Value = parameter;

                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[2];
                cell.DisplayMember = "Units";
                cell.ValueMember = "Units";
                foreach (Conversion conversion in parameter.Conversions)
                {
                    cell.Items.Add(conversion);
                }
                row.Cells[2].Value = parameter.Conversions.First();

                this.parameterIdsToRows[parameter.Id] = row;
                this.parameterGrid.Rows.Add(row);
            }

            this.suspendSelectionEvents = false;


            if (!this.parameterSearch.Focused)
            {
                this.ShowSearchPrompt();
            }


        }


        private void UpdateGridFromProfile()
        {
            try
            {
                this.suspendSelectionEvents = true;

                foreach (DataGridViewRow row in this.parameterGrid.Rows)
                {
                    row.Cells[0].Value = false;
                }

                foreach (LogColumn column in this.currentProfile.Columns)
                {
                    DataGridViewRow row;
                    if (this.parameterIdsToRows.TryGetValue(column.Parameter.Id, out row))
                    {
                        row.Cells[0].Value = true;

                        DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)(row.Cells[2]);
                        Conversion profileConversion = column.Conversion;
                        string profileUnits = column.Conversion.Units;

                        foreach (Conversion conversion in cell.Items)
                        {
                            if ((conversion == profileConversion) || (conversion.Units == profileUnits))
                            {
                                cell.Value = conversion;
                            }
                        }
                    }
                }
            }
            finally
            {
                this.suspendSelectionEvents = false;
            }
        }

        private void parameterGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCheckBoxCell checkBoxCell = this.parameterGrid.CurrentCell as DataGridViewCheckBoxCell;
            if ((checkBoxCell != null) && checkBoxCell.IsInEditMode && this.parameterGrid.IsCurrentCellDirty)
            {
                this.parameterGrid.EndEdit();
            }
        }

        private void parameterGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.LogProfileChanged();
        }

        private void LogProfileChanged()
        {
            if (this.suspendSelectionEvents)
            {
                return;
            }

            this.ResetProfile();

            this.CreateProfileFromGrid();

            this.SetDirtyFlag(true);
        }

        private void CreateProfileFromGrid()
        {
            this.ResetProfile();

            foreach (DataGridViewRow row in this.parameterGrid.Rows)
            {
                if ((bool)row.Cells[0].Value == true)
                {
                    Parameter parameter = (Parameter)row.Cells[1].Value;
                    Conversion conversion = null;

                    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)(row.Cells[2]);
                    foreach (Conversion candidate in cell.Items)
                    {
                        // The fact that we have to do both kinds of comparisons here really
                        // seems like a bug in the DataGridViewComboBoxCell code:
                        if ((candidate.Units == cell.Value as string) ||
                            (candidate == cell.Value as Conversion))
                        {
                            conversion = candidate;
                            break;
                        }
                    }

                    LogColumn column = new LogColumn(parameter, conversion);
                    this.currentProfile.AddColumn(column);
                }
            }
        }

        private bool showSearchPrompt = true;

        private void ShowSearchPrompt()
        {
            this.parameterSearch.Text = "";
            parameterSearch_Leave(this, new EventArgs());
        }

        private void parameterSearch_Enter(object sender, EventArgs e)
        {
            if (this.showSearchPrompt)
            {
                this.parameterSearch.Text = "";
                this.showSearchPrompt = false;
                return;
            }
        }

        private void parameterSearch_Leave(object sender, EventArgs e)
        {
            if (this.parameterSearch.Text.Length == 0)
            {
                this.showSearchPrompt = true;
                this.parameterSearch.Text = "Search...";
                return;
            }
        }

        private void parameterSearch_TextChanged(object sender, EventArgs e)
        {
            if (this.showSearchPrompt)
            {
                return;
            }

            foreach (DataGridViewRow row in this.parameterGrid.Rows)
            {
                Parameter parameter = row.Cells[1].Value as Parameter;
                if (parameter == null)
                {
                    continue;
                }

                if (parameter.Name.IndexOf(this.parameterSearch.Text, StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    row.Visible = false;
                }
                else
                {
                    row.Visible = true;
                }
            }
        }

        #region MainFormBase override methods

        /// <summary>
        /// Not used.
        /// </summary>
        /// <param name="message"></param>
        public void AddUserMessage(string message)
        {
        }

        /// <summary>
        /// Add a message to the debug pane of the main window.
        /// </summary>
        public void AddDebugMessage(string message)
        {
            string timestamp = DateTime.Now.ToString("hh:mm:ss:fff");

            Task foreground = Task.Factory.StartNew(
                delegate ()
                {
                    try
                    {
                        if (enableDebugCheckbox.Checked)
                            this.debugLog.AppendText("[" + timestamp + "]  " + message + Environment.NewLine);
                    }
                    catch (ObjectDisposedException)
                    {
                        // This will happen if the window is closing. Just ignore it.
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                uiThreadScheduler);
        }

        public void ResetLogs()
        {
            this.debugLog.Clear();
        }

        public string GetAppNameAndVersion()
        {
            return "PCM Logger";
        }

        protected void DisableUserInput()
        {
            this.EnableProfileButtons(false);
            this.profileList.Enabled = false;
            this.parameterGrid.Enabled = false;
            this.parameterSearch.Enabled = false;
            this.selectButton.Enabled = false;
            this.startStopSaving.Enabled = false;
        }

        protected void EnableInterfaceSelection()
        {
            this.selectButton.Enabled = true;
        }

        protected void EnableUserInput()
        {
            this.EnableProfileButtons(true);
            this.profileList.Enabled = true;
            this.parameterGrid.Enabled = true;
            this.parameterSearch.Enabled = true;
            this.selectButton.Enabled = true;
            this.startStopSaving.Enabled = true;
            this.startStopSaving.Focus();
        }

        protected void NoDeviceSelected()
        {
            this.selectButton.Enabled = true;
            this.deviceDescription.Text = "No device selected";

            // This or ValidDeviceSelected will be called after the form loads.
            // Either way, we should let users manipulate profiles.
            this.EnableProfileButtons(true);
        }

        protected void SetSelectedDeviceText(string message)
        {
            this.deviceDescription.Text = message;
        }

        /// <summary>
        /// This is invoked from within the call to base.ResetDevice().
        /// </summary>
        protected void ValidDeviceSelectedAsync(string deviceName)
        {
            this.AddDebugMessage("ValidDeviceSelectedAsync started.");
            Response<uint> response = this.Vehicle.QueryOperatingSystemId(new CancellationToken());
            if (response.Status != ResponseStatus.Success)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.deviceDescription.Text = deviceName + " is unable to connect to the PCM";
                });
                return;
            }

            // This must be assigned prior to calling FillParameterGrid(), 
            // otherwise the RAM parameters will not appear in the grid.
            this.osid = response.Value;
            
            this.Invoke((MethodInvoker)delegate ()
            {
                this.deviceDescription.Text = deviceName + " " + osid.ToString();
                this.startStopSaving.Enabled = true;
                this.parameterGrid.Enabled = true;
                this.EnableProfileButtons(true);
                this.FillParameterGrid();
            });

            string lastProfile = Properties.Settings.Default.LastProfile;
            if (!string.IsNullOrEmpty(lastProfile) && File.Exists(lastProfile))
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    this.OpenProfile(lastProfile);
                });
            }

            // Start pulling data from the PCM
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoggingThread), null);

            this.AddDebugMessage("ValidDeviceSelectedAsync ended.");
        }

        #endregion

        #region Open / Close

        /// <summary>
        /// Open the most-recently-used device, if possible.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Order matters - the scheduler must be set before adding messages.
            this.uiThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.AddDebugMessage("MainForm_Load started.");
                        
            string logDirectory = Properties.Settings.Default.LogDirectory;
            if (string.IsNullOrWhiteSpace(logDirectory))
            {
                logDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Properties.Settings.Default.LogDirectory = logDirectory;
                Properties.Settings.Default.Save();
            }

            // This just saves the trouble of having to keep a const string in 
            // sync with whatever window text is entered in the designer view.
            this.Text = appName;

            this.EnableProfileButtons(false);

            this.LoadProfileHistory();

            enablePassiveModeCheckBox.Checked = DeviceConfiguration.Settings.EnablePassiveMode;

            ThreadPool.QueueUserWorkItem(BackgroundInitialization);

            this.logFilePath.Text = logDirectory;

            this.AddDebugMessage("MainForm_Load ended.");
        }
        
        private void BackgroundInitialization(object unused)
        {
            try
            {
                this.AddDebugMessage("Device reset started.");

                // This will cause the ValidDeviceSelectedAsync callback to be invoked.
                this.ResetDevice();

                this.AddDebugMessage("Device reset completed.");
            }
            catch (Exception exception)
            {
                // Don't try to log messages during shutdown, that doesn't end well
                // because the window handle is no longer valid.
                //
                // There is still a race condition around using logStopRequested for
                // this, but the only deterministic solution involves cross-thread 
                // access to the Form object, which isn't allowed.
                if (!this.logStopRequested) 
                {
                    this.Invoke(
                        (MethodInvoker)
                        delegate ()
                        {
                            if (!this.logStopRequested)
                            {
                                this.AddDebugMessage("BackgroundInitialization: " + exception.ToString());
                            }
                        });
                }
            }
        }

        private void EnableProfileButtons(bool enable)
        {
            this.newButton.Enabled = enable;
            this.openButton.Enabled = enable;
            this.saveButton.Enabled = enable;
            this.saveAsButton.Enabled = enable;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.currentProfileIsDirty)
            {
                if (this.SaveIfNecessary() == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            this.logStopRequested = true;

            this.SaveProfileHistory();

            // It turns out that WaitAll is not supported on an STA thread.
            // WaitHandle.WaitAll(new WaitHandle[] { loggerThreadEnded, writerThreadEnded });
            loggerThreadEnded.WaitOne(1000);
            writerThreadEnded.WaitOne(1000);
        }

        #endregion

        #region Button clicks

        /// <summary>
        /// Select which interface device to use. This opens the Device-Picker dialog box.
        /// </summary>
        protected void selectButton_Click(object sender, EventArgs e)
        {
            HandleSelectButtonClick();
        }

        /// <summary>
        /// Choose which directory to create log files in.
        /// </summary>
        private void setDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Properties.Settings.Default.LogDirectory;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LogDirectory = dialog.SelectedPath;
                Properties.Settings.Default.Save();
                this.logFilePath.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Open a File Explorer window in the log directory.
        /// </summary>
        private void openDirectory_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Settings.Default.LogDirectory);
        }

        /// <summary>
        /// Start or stop logging.
        /// </summary>
        private void startStopSaving_Click(object sender, EventArgs e)
        {
            if (saving)
            {
                this.saving = false;
                this.startStopSaving.Text = "Start &Recording";
                this.loggerProgress.MarqueeAnimationSpeed = 0;
                this.loggerProgress.Visible = false;
                this.logState = LogState.StopSaving;
            }
            else
            {
                this.saving = true;
                this.startStopSaving.Text = "Stop &Recording";
                this.loggerProgress.MarqueeAnimationSpeed = 100;
                this.loggerProgress.Visible = true;
                this.logState = LogState.StartSaving;
            }
        }

        #endregion


        private void enablePassiveModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeviceConfiguration.Settings.EnablePassiveMode = enablePassiveModeCheckBox.Checked;
            DeviceConfiguration.Settings.Save();
            this.ResetDevice();
        }


        private void editParameters_Click(object sender, EventArgs e)
        {
            frmEditPidParameters fep = new frmEditPidParameters();
            fep.Show();
        }
    }
}
