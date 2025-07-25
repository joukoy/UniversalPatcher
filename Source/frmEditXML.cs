using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static Upatcher;
using UniversalPatcher.Properties;
using System.Diagnostics;
using static UniversalPatcher.ExtensionMethods;
using static Helpers;
using static UniversalPatcher.Analyzer;
using static LoggerUtils;

namespace UniversalPatcher
{
    public partial class frmEditXML : Form
    {
        public frmEditXML()
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
        }

        private enum XMLTYPE
        {
            Autodetect,
            TableSeek,
            TableData,
            SegmentSeek,
            Cvn,
            DtcSearchConfig,
            PidSearchConfig,
            PidDescriptions,
            Units,
            FileTypes,
            SegmentConfig,
            ObdResponses,
            DeviceNames,
            FunctionNames,
            OBD2Codes,
            CANmodules,
            RealtimeControl,
            XdfPlugins,
            OldPidProfile
        }

        private XMLTYPE xmlType = XMLTYPE.Autodetect;
        private BindingSource bindingSource = new BindingSource();
        private bool starting = true;
        private string sortBy = "";
        private int sortIndex = 0;
        SortOrder strSortOrder = SortOrder.Ascending;
        string fileName = "";
        private PcmFile PCM;
        private List<TableSeek> tSeeks;
        private List<SegmentSeek> sSeeks;
        public List<RealTimeControl> rtControls;
        private List<CVN> sCVN;
        private List<DtcSearchConfig> dtcSC;
        private List<PidSearchConfig> pidSC;
        private List<PidInfo> piddescriptions;
        private List<Units> units;
        private List<FileType> fileTypes;
        private List<DetectRule> detectrules;
        private List<SegmentConfig> segmentconfigs;
        public List<ObdEmu.OBDResponse> obdresponses;
        private List<IdName> devicenames;
        private List<IdName> funcnames;
        private List<CANDevice> canmods;
        private List<XDF.XdfPlugin> xdfplugins;
        private object currentObj;
        private object currentList;
        private Type currentType;
        public object SelectedObject;
        private List<ReDo> redoLog = new List<ReDo>();

        private void frmEditXML_Load(object sender, EventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                if (AppSettings.EditXMLWindowSize.Width > 0 || AppSettings.EditXMLWindowSize.Height > 0)
                {
                    this.WindowState = AppSettings.EditXMLWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = AppSettings.EditXMLWindowLocation;
                    this.Size = AppSettings.EditXMLWindowSize;
                }
            }
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView1.DataError += DataGridView1_DataError;
            dataGridView1.KeyPress += DataGridView1_KeyPress;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.UserAddedRow += DataGridView1_UserAddedRow;
            dataGridView1.UserDeletedRow += DataGridView1_UserDeletedRow;
            //dataGridView1.CellMouseClick += DataGridView1_CellMouseClick;
        }

        private void frmEditXML_FormClosing(object sender, EventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                AppSettings.EditXMLWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    AppSettings.EditXMLWindowLocation = this.Location;
                    AppSettings.EditXMLWindowSize = this.Size;
                }
                else
                {
                    AppSettings.EditXMLWindowLocation = this.RestoreBounds.Location;
                    AppSettings.EditXMLWindowSize = this.RestoreBounds.Size;
                }
                AppSettings.Save();
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterTables();
        }

        private void DataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dataGridView1.BeginEdit(true);
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        private void FillFilterBy()
        {
            foreach (var prop in currentObj.GetType().GetProperties())
            {
                if (string.IsNullOrEmpty(sortBy))
                {
                    sortBy = prop.Name;
                    comboFilterBy.Text = sortBy;
                }
                //Add to filter by-combo
                comboFilterBy.Items.Add(prop.Name);
            }
            comboFilterBy.SelectedIndexChanged += ComboFilterBy_SelectedIndexChanged;
        }

        private void ComboFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTables();
        }

        public void LoadRules()
        {
            xmlType = XMLTYPE.Autodetect;
            detectrules = new List<DetectRule>();
            currentList = detectrules;
            currentObj = new DetectRule();
            currentType = typeof(DetectRule);
            for (int i=0;i<DetectRules.Count;i++)
            {
                DetectRule dr = DetectRules[i].ShallowCopy();
                detectrules.Add(dr);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = detectrules;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            currentList = detectrules;
            UseComboBoxForEnums(dataGridView1);
            FillFilterBy();
        }

        public void LoadStockCVN()
        {
            xmlType = XMLTYPE.Cvn;
            this.Text = "Edit stock CVN list";
            sCVN = new List<CVN>();
            currentList = sCVN;
            currentObj = new CVN();
            currentType = typeof(CVN);
            for (int i=0; i<StockCVN.Count;i++)
            {
                CVN cvn = StockCVN[i].ShallowCopy();
                sCVN.Add(cvn);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = sCVN;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
            FillFilterBy();
        }

        public void LoadOldPidProfile(string fName)
        {
            xmlType = XMLTYPE.OldPidProfile;
            this.Text = "Edit Old PID profile";
            fileName = fName;
            currentList = datalogger.PidProfile;
            currentObj = new PidConfig();
            currentType = typeof(PidConfig);
            bindingSource.DataSource = null;
            bindingSource.DataSource = datalogger.PidProfile;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
            FillFilterBy();
        }

        public void LoadDTCSearchConfig()
        {
            xmlType = XMLTYPE.DtcSearchConfig;
            this.Text = "Edit DTC Search config";
            dtcSC = new List<DtcSearchConfig>();
            currentList = dtcSC;
            currentObj = new DtcSearchConfig();
            currentType = typeof(DtcSearchConfig);
            for (int i=0; i< dtcSearchConfigs.Count;i++)
            {
                DtcSearchConfig dsc = dtcSearchConfigs[i].ShallowCopy();
                dtcSC.Add(dsc);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = dtcSC;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Columns["ConditionalOffset"].ToolTipText = "Possible values:code,status,mil (Multiple values allowed)";
            UseComboBoxForEnums(dataGridView1);
            FillFilterBy();
        }

        public void LoadPIDSearchConfig()
        {
            xmlType = XMLTYPE.PidSearchConfig;
            this.Text = "Edit PID Search config";
            pidSC = new List<PidSearchConfig>();
            currentList = pidSC;
            currentObj = new PidSearchConfig();
            currentType = typeof(PidSearchConfig);
            for (int i=0; i< pidSearchConfigs.Count; i++)
            {
                PidSearchConfig psc = pidSearchConfigs[i].ShallowCopy();
                pidSC.Add(psc);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = pidSC;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
            FillFilterBy();
        }

        public void LoadPIDDescriptions()
        {
            xmlType = XMLTYPE.PidDescriptions;
            this.Text = "Edit PID Descriptions";
            piddescriptions = new List<PidInfo>();
            currentList = piddescriptions;
            currentObj = new PidInfo();
            currentType = typeof(PidInfo);
            for (int i = 0; i < PidDescriptions.Count; i++)
            {
                PidInfo pi = PidDescriptions[i].ShallowCopy();
                piddescriptions.Add(pi);
            }
            scalingConversionFactorToolStripMenuItem.Enabled = true;
            bindingSource.DataSource = null;
            bindingSource.DataSource = piddescriptions;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
            testMathToolStripMenuItem.Enabled = true;
            FillFilterBy();
        }

        public void LoadTableSeek(string fname)
        {
            if (fname.Length > 0 &&  File.Exists(fname))
                tSeeks = TableSeek.LoadTableSeekFile(fname);
            else
                tSeeks = new List<TableSeek>();
            SetupTableSeek(fname);
        }
        public void EditCurrentTableSeek(string fname)
        {
            tSeeks = new List<TableSeek>();
            foreach (TableSeek ts in tableSeeks)
            {
                tSeeks.Add(ts.ShallowCopy());
            }
            SetupTableSeek(fname);
        }

        private void SetupTableSeek(string fname)
        {
            xmlType = XMLTYPE.TableSeek;
            fileName = fname;
            this.Text = "Edit Table Seek config";
            currentObj = new TableSeek();
            currentList = tSeeks;
            currentType = typeof(TableSeek);
            RefreshTableSeek();
            FillFilterBy();
            //undoRedoToolStripMenuItem.Enabled = true;
        }

        private void DataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                object obj = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                string itmName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                AddToRedoLog(obj, currentList, xmlType.ToString(), itmName, "", ReDo.RedoAction.Delete, "", "", dataGridView1.CurrentCell.RowIndex);
            }
        }

        private void DataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                object obj = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                string itmName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                AddToRedoLog(obj, currentList, xmlType.ToString(), itmName, "", ReDo.RedoAction.Add, "", "");
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                var oldValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
                var newValue = e.FormattedValue;
                if (dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format.Contains("X"))
                {
                    if (HexToUint64(newValue.ToString(), out UInt64 intVal))
                    {
                        newValue = intVal;
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = intVal;
                    }
                }
                object obj = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                string prop = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                string itmName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                AddToRedoLog(obj, currentList, xmlType.ToString(), itmName,prop, ReDo.RedoAction.Edit, oldValue, newValue);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmEditXML, line " + line + ": " + ex.Message);
            }
        }


        public void LoadSegmentSeek(string fname)
        {
            xmlType = XMLTYPE.SegmentSeek;
            fileName = fname;
            this.Text = "Edit Segment Seek config";
            currentObj = new SegmentSeek();
            currentType = typeof(SegmentSeek);
            if (fname.Length > 0 &&  File.Exists(fname))
                sSeeks = SegmentSeek.LoadSegmentSeekFile(fname);
            else
                sSeeks = new List<SegmentSeek>();
            currentList = sSeeks;
            RefreshSegmentSeek();
            FillFilterBy();
        }

        public void LoadRealTimeControlCommands()
        {
            xmlType = XMLTYPE.RealtimeControl;
            if (!string.IsNullOrEmpty(AppSettings.ControlCommandsFile))
            {
                fileName = AppSettings.ControlCommandsFile;
            }
            else
            {
                fileName = Path.Combine(Application.StartupPath, "XML", "realtimecontrol.xml");

            }
            this.Text = "Edit Realtime Control Commands";
            currentObj = new RealTimeControl();
            currentType = typeof(RealTimeControl);
            if (File.Exists(fileName))
                rtControls = LoadRealTimeControls();
            else
                rtControls = new List<RealTimeControl>();
            currentList = rtControls;
            RefreshRealTimeControls();
            FillFilterBy();
        }

        public void LoadObdResponses(string fname)
        {
            xmlType = XMLTYPE.ObdResponses;
            fileName = fname;
            this.Text = "Edit OBD Responses";
            currentObj = new ObdEmu.OBDResponse();
            currentType = typeof(ObdEmu.OBDResponse);
            if (fname.Length > 0 && File.Exists(fname))
                obdresponses = ObdEmu.LoadObdResponseFile(fname);
            else
                obdresponses = new List<ObdEmu.OBDResponse>();
            currentList = obdresponses;
            RefreshObdResponses();
            FillFilterBy();
        }

        public void LoadTableData()
        {
            xmlType = XMLTYPE.TableData;
            this.Text = "Table data";
            currentList = tableViews;
            currentObj = new TableData();
            currentType = typeof(TableData);
            bindingSource.DataSource = null;
            bindingSource.DataSource = tableViews;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            btnSave.Visible = false;
            dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            UseComboBoxForEnums(dataGridView1);
            FillFilterBy();
        }

        public void LoadSegmentConfig(PcmFile PCM)
        {
            xmlType = XMLTYPE.SegmentConfig;
            this.PCM = PCM;
            segmentconfigs = new List<SegmentConfig>();
            currentList = segmentconfigs;
            currentObj = new SegmentConfig();
            currentType = typeof(SegmentConfig);
            for (int s = 0; s < PCM.Segments.Count; s++)
                segmentconfigs.Add(PCM.Segments[s].ShallowCopy());
            this.Text = "Segment Config";
            bindingSource.DataSource = null;
            bindingSource.DataSource = segmentconfigs;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
            dataGridView1.Columns["CS1Method"].ToolTipText = "For backward compatibility. Ignore";
            dataGridView1.Columns["CS2Method"].ToolTipText = "For backward compatibility. Ignore";
            for (int r=0; r< dataGridView1.Rows.Count;r++)
            {
                dataGridView1.Rows[r].Cells["CS1Method"].ToolTipText = "For backward compatibility. Ignore";
                dataGridView1.Rows[r].Cells["CS2Method"].ToolTipText = "For backward compatibility. Ignore";
            }
            FillFilterBy();
        }
        public void LoadXdfPlugins()
        {
            xmlType = XMLTYPE.XdfPlugins;
            this.Text = "XDF Checksum plugins";
            xdfplugins = new List<XDF.XdfPlugin>();
            currentObj = new XDF.XdfPlugin();
            currentType = typeof(XDF.XdfPlugin);
            string pluginXml = Path.Combine(Application.StartupPath, "XML", "XdfPlugins.xml");
            if (File.Exists(pluginXml))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<XDF.XdfPlugin>));
                System.IO.StreamReader file = new System.IO.StreamReader(pluginXml);
                xdfplugins = (List<XDF.XdfPlugin>)reader.Deserialize(file);
                file.Close();
            }
            labelHelp.Text = "";
            if (xdfplugins.Count == 0)
            {
                XDF.XdfPlugin xdfplugin= new XDF.XdfPlugin();
                xdfplugin.Platform = "p01-p59";
                xdfplugin.pluginmoduleid = "a8d1fdd7-d985-4b09-9979-2735b69cc9a6";
                xdfplugin.Url = "https://github.com/joukoy/gm-checksum-plugins/raw/refs/heads/master/GM-P01-P59-checksum-plugin.dll";
                xdfplugins.Add(xdfplugin);
            }
            currentList = xdfplugins;
            bindingSource.DataSource = null;
            bindingSource.DataSource = xdfplugins;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            FillFilterBy();
        }

        public void LoadFileTypes()
        {
            xmlType = XMLTYPE.FileTypes;
            this.Text = "File Types";
            fileTypes = new List<FileType>();
            currentObj = new FileType();
            currentType = typeof(FileType);
            for (int i=0; i<fileTypeList.Count;i++)
            {
                FileType ft = fileTypeList[i].ShallowCopy();
                fileTypes.Add(ft);
            }
            labelHelp.Text = "Select ONE as default. Example: Description = BIN files, Extension = *.bin;*.dat";
            if (fileTypes.Count == 0)
            {
                FileType ft = new FileType();
                ft.Default = true;
                ft.Description = "BIN files";
                ft.Extension = "*.bin";
                fileTypes.Add(ft);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = fileTypes;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            FillFilterBy();
        }
        public void LoadUnits()
        {
            xmlType = XMLTYPE.Units;
            this.Text = "Units";
            units = new List<Units>();
            currentList = units;
            currentObj = new Units();
            currentType = typeof(Units);
            for (int i=0; i<unitList.Count;i++)
            {
                Units u = unitList[i].ShallowCopy();
                units.Add(u);
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = units;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            FillFilterBy();
        }

        public void LoadCANModules()
        {
            xmlType = XMLTYPE.CANmodules;
            this.Text = "CAN modules";
            canmods = new List<CANDevice>();
            currentList = canmods;
            currentObj = new CANDevice();
            currentType = typeof(CANDevice);
            if (CanModules == null || CanModules.Count == 0)
            {
                string fName = SelectFile("Select id-file", TxtFilter);
                if (string.IsNullOrEmpty(fName))
                {
                    return;
                }
                CanModules = new List<CANDevice>();
                string content = ReadTextFile(fName);
                string[] lines = content.Split('\n');
                foreach (string line in lines)
                {
                    CANDevice cd = new CANDevice();
                    string[] lParts = line.Split('\t');
                    if (lParts.Length >= 6)
                    {
                        cd.ModuleName = lParts[0];
                        cd.ModuleDescription = lParts[5];
                        if (HexToByte(lParts[1], out byte ecuid))
                            cd.ModuleID = ecuid;
                        if (HexToUint(lParts[2], out uint rqid))
                            cd.RequestID = (int)rqid;
                        if (HexToUint(lParts[3], out uint resid))
                            cd.ResID = (int)resid;
                        if (HexToUint(lParts[4], out uint diagid))
                            cd.DiagID = (int)diagid;
                    }
                    canmods.Add(cd);
                }
            }
            else
            {
                for (int i = 0; i < CanModules.Count; i++)
                {
                    CANDevice c = CanModules[i].ShallowCopy();
                    canmods.Add(c);
                }
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = canmods;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            for (int c=0; c<4; c++)
                dataGridView1.Columns[c].DefaultCellStyle.Format = "X4";
            FillFilterBy();
        }

        public void LoadDeviceNames()
        {
            xmlType = XMLTYPE.DeviceNames;
            this.Text = "Device names";
            devicenames = new List<IdName>();
            currentList = devicenames;
            currentObj = new IdName();
            currentType = typeof(IdName);
            if (DeviceNames == null || DeviceNames.Count == 0)
            {
                if (analyzer == null)
                {
                    analyzer = new Analyzer();
                }
                for (byte b = 0; b < 0xFF; b++)
                {
                    if (analyzer.PhysAddresses.ContainsKey(b))
                    {
                        IdName idn = new IdName(b, analyzer.PhysAddresses[b]);
                        devicenames.Add(idn);
                    }
                }
            }
            else
            {
                for (int i = 0; i < DeviceNames.Count; i++)
                {
                    IdName d = DeviceNames[i].ShallowCopy();
                    devicenames.Add(d);
                }
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = devicenames;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Columns[0].DefaultCellStyle.Format = "X2";
            FillFilterBy();
        }

        public void LoadFuncNames()
        {
            xmlType = XMLTYPE.FunctionNames;
            this.Text = "Function names";
            funcnames = new List<IdName>();
            currentList = funcnames;
            currentObj = new IdName();
            currentType = typeof(IdName);
            if (FuncNames == null || FuncNames.Count == 0)
            {
                if (analyzer == null)
                {
                    analyzer = new Analyzer();
                }
                for (byte b = 0; b < 0xFF; b++)
                {
                    if (analyzer.FuncAddresses.ContainsKey(b))
                    {
                        IdName idn = new IdName(b, analyzer.FuncAddresses[b]);
                        funcnames.Add(idn);
                    }
                }
            }
            else
            {
                for (int i = 0; i < FuncNames.Count; i++)
                {
                    IdName d = FuncNames[i].ShallowCopy();
                    funcnames.Add(d);
                }
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = funcnames;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            dataGridView1.Columns[0].DefaultCellStyle.Format = "X2";
            FillFilterBy();
        }

        public void LoadOBD2CodeList()
        {
            xmlType = XMLTYPE.OBD2Codes;
            this.Text = "Edit OBD2 Codes";
            currentObj = new OBD2Code();
            currentType = typeof(OBD2Code);
            LoadOBD2Codes();
            currentList = OBD2Codes;
            RefreshOBD2Codes();
            FillFilterBy();
        }

        private void Refreshdgrid()
        {
            object tmp = bindingSource.DataSource;
            bindingSource.DataSource = null;
            dataGridView1.DataSource = null;
            bindingSource.DataSource = tmp;
            dataGridView1.DataSource = bindingSource;
        }


        private void RefreshOBD2Codes()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = OBD2Codes;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
        }


        private void RefreshTableSeek()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = tSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }
        private void RefreshRealTimeControls()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = rtControls;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }

        private void RefreshSegmentSeek()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = sSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            UseComboBoxForEnums(dataGridView1);
        }

        private void RefreshObdResponses()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = obdresponses;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;
            //UseComboBoxForEnums(dataGridView1);
        }

        private void SaveThis(string fName)
        {
            try
            {
                dataGridView1.NotifyCurrentCellDirty(true);
                dataGridView1.EndEdit();
                switch (xmlType)
                {
                    case XMLTYPE.Cvn:
                        Logger("Saving file stockcvn.xml", false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                            writer.Serialize(stream, sCVN);
                            stream.Close();
                        }
                        StockCVN = sCVN;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.FileTypes:
                        Logger("Saving file filetypes.xml", false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "filetypes.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<FileType>));
                            writer.Serialize(stream, fileTypes);
                            stream.Close();
                        }
                        fileTypeList = fileTypes;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.DtcSearchConfig:
                        Logger("Saving file DtcSearch.xml", false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "DtcSearch.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DtcSearchConfig>));
                            writer.Serialize(stream, dtcSC);
                            stream.Close();
                        }
                        dtcSearchConfigs = dtcSC;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.PidSearchConfig:
                        Logger("Saving file PidSearch.xml", false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "PidSearch.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidSearchConfig>));
                            writer.Serialize(stream, pidSC);
                            stream.Close();
                        }
                        pidSearchConfigs = pidSC;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.PidDescriptions:
                        Logger("Saving file PidDescriptions.xml", false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "PidDescriptions.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidInfo>));
                            writer.Serialize(stream, piddescriptions);
                            stream.Close();
                        }
                        PidDescriptions = piddescriptions;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.ObdResponses:
                        Logger("Saving file " + fName, false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "Logger", "Responses.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<ObdEmu.OBDResponse>));
                            writer.Serialize(stream, obdresponses);
                            stream.Close();
                        }
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.OBD2Codes:
                        SaveOBD2Codes(fName);
                        break;
                    case XMLTYPE.TableSeek:
                        if (fName.Length == 0)
                            fName = SelectSaveFile(SegmentseekFilter, "new-tableseek.xml");
                        if (fName.Length == 0)
                            return;
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSeek>));
                            writer.Serialize(stream, tSeeks);
                            stream.Close();
                        }
                        tableSeeks = tSeeks;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.SegmentSeek:
                        if (fName.Length == 0)
                            fName = SelectSaveFile(SegmentseekFilter, "new-segmentseek.xml");
                        if (fName.Length == 0)
                            return;
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentSeek>));
                            writer.Serialize(stream, sSeeks);
                            stream.Close();
                        }
                        segmentSeeks = sSeeks;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.Units:
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "Tuner", "units.xml");
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Units>));
                            writer.Serialize(stream, units);
                            stream.Close();
                        }
                        unitList = units;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.FunctionNames:
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "functionnames.xml");
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<IdName>));
                            writer.Serialize(stream, funcnames);
                            stream.Close();
                        }
                        FuncNames = funcnames;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.DeviceNames:
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "devicenames.xml");
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<IdName>));
                            writer.Serialize(stream, devicenames);
                            stream.Close();
                        }
                        DeviceNames = devicenames;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.RealtimeControl:
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "realtimecontrol.xml");
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<RealTimeControl>));
                            writer.Serialize(stream, rtControls);
                            stream.Close();
                        }
                        RealTimeControls = rtControls;
                        AppSettings.ControlCommandsFile = fName;
                        AppSettings.Save();
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.CANmodules:
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "CANModules.xml");
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CANDevice>));
                            writer.Serialize(stream, canmods);
                            stream.Close();
                        }
                        CanModules = canmods;
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.XdfPlugins:
                        fName = Path.Combine(Application.StartupPath, "XML", "XdfPlugins.xml");
                        Logger("Saving file " + fName, false);
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<XDF.XdfPlugin>));
                            writer.Serialize(stream, xdfplugins);
                            stream.Close();
                        }
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.SegmentConfig:
                        if (fName.Length == 0)
                            fName = PCM.segmentFile;
                        Logger("Saving file " + fName, false);
                        PCM.Segments = segmentconfigs;
                        PCM.SaveConfigFile(fName);
                        Logger(" [OK]");
                        break;
                    case XMLTYPE.OldPidProfile:
                        Logger("Saving file " + fileName, false);
                        datalogger.SaveOldProfile(fileName);
                        Logger(" [OK]");
                        break;
                    default:
                        Logger("Saving file autodetect.xml", false);
                        if (fName.Length == 0)
                            fName = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
                        using (FileStream stream = new FileStream(fName, FileMode.Create))
                        {
                            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                            writer.Serialize(stream, detectrules);
                            stream.Close();
                        }
                        DetectRules = detectrules;
                        Logger(" [OK]");
                        break;
                }            
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveThis(fileName);
                if (dataGridView1.SelectedRows.Count > 0)
                    SelectedObject = dataGridView1.SelectedRows[0].DataBoundItem;
                else if (dataGridView1.SelectedCells.Count > 0)
                    SelectedObject = dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].DataBoundItem;
                else
                    SelectedObject = dataGridView1.Rows[0].DataBoundItem;
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
                this.DialogResult = DialogResult.Abort;
            }
        }

        private void SaveCSV()
        {
            string FileName = SelectSaveFile(CsvFilter);
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataGridView1.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataGridView1.Rows[r].Cells[i].Value != null)
                            row += dataGridView1.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");
        }
        private void btnSaveCSV_Click(object sender, EventArgs e)
        {
            SaveCSV();
        }


        private void ImportCSV()
        {
            //Example:
            //SPARK_ADVANCE;KA_PISTON_SLAP_SPARK_RETARD;00013D8E;81294;330;3C 0A 00 74 0B 20 7C @ @ @ @ 4E B9;2;;-05;80 FC 00 14 74 0B 20 7C @ @ @ @ 4E B9;1;;06-07
            //06-07 is "extension", add *extension to tablename
            try
            {
                string FileName = SelectFile("Select CSV file", CsvFilter);
                if (FileName.Length == 0)
                    return;
                StreamReader sr = new StreamReader(FileName);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] lineparts = line.Split(';');
                    if (lineparts.Length > 5)
                    {
                        Debug.WriteLine(line);
                        for (int i = 5; i < lineparts.Length; i += 4)
                        {
                            TableSeek ts = new TableSeek();
                            ts.Category = lineparts[0];
                            ts.Name = lineparts[1];
                            if (ts.Name.ToLower().StartsWith("k_dyna_air_coefficient"))
                            {
                                ts.RefAddress = lineparts[3];

                            }
                            else
                            {
                                ts.RefAddress = lineparts[2];
                            }
                            ts.SearchStr = lineparts[i];
                            string xtension = "";
                            if (lineparts.Length >= i + 1)
                                ts.UseHit = lineparts[i + 1];
                            if (lineparts.Length >= i + 2 && lineparts[i + 2].Length > 0)
                                ts.Offset = Convert.ToInt64(lineparts[i + 2]);
                            if (lineparts.Length >= i + 3 && lineparts[i + 3].Length > 0)
                                xtension = lineparts[i + 3];
                            // ts.Name += "*" + lineparts[i + 3];

                            for (int s = 0; s < tSeeks.Count; s++)
                            {
                                if (tSeeks[s].Name != null && tSeeks[s].Category != null && tSeeks[s].Name.ToLower() == lineparts[1].ToLower() && tSeeks[s].Category.ToLower() == ts.Category.ToLower())
                                {
                                    TableSeek tsNew = new TableSeek();
                                    tsNew = tSeeks[s].ShallowCopy();
                                    tsNew.SearchStr = ts.SearchStr;
                                    tsNew.UseHit = ts.UseHit;
                                    tsNew.Offset = ts.Offset;
                                    tsNew.RefAddress = ts.RefAddress;
                                    if (xtension.Length > 0)
                                        tsNew.Name += "*" + xtension;
                                    tSeeks.Add(tsNew);
                                    Debug.WriteLine(tsNew.Name);
                                    break;
                                }
                            }

                        }
                    }
                }
                sr.Close();

                for (int s = tSeeks.Count - 1; s >= 0; s--)
                {
                    if (tSeeks[s].Name.ToLower().StartsWith("ka_") || tSeeks[s].Name.ToLower().StartsWith("ke_") || tSeeks[s].Name.ToLower().StartsWith("kv_"))
                        tSeeks[s].Name = tSeeks[s].Name.Substring(3);
                    if (tSeeks[s].SearchStr.Length == 0)
                        tSeeks.RemoveAt(s);
                }
                bindingSource.DataSource = null;
                bindingSource.DataSource = tSeeks;
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = bindingSource;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {
            ImportCSV();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (xmlType == XMLTYPE.TableSeek && starting)
            {
                dataGridView1.AutoResizeColumns();
                dataGridView1.Columns["SearchStr"].Width = 100;
                dataGridView1.Columns["RowHeaders"].Width = 100;
                dataGridView1.Columns["Colheaders"].Width = 100;
                starting = false;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveThis(fileName);
        }

        private void saveCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCSV();
        }

        private void ImportOBD2()
        {
            string FileName = SelectFile("Select CSV file", CsvFilter);
            if (FileName.Length == 0)
                return;
            Logger("Importing file: " + FileName, false);
            StreamReader sr = new StreamReader(FileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                int pos = line.IndexOf(';');
                if (pos < 0)
                    pos = line.IndexOf(',');
                if (pos > -1)
                {
                    OBD2Code oc = new OBD2Code();
                    oc.Code = line.Substring(0,pos).Trim();
                    oc.Description = line.Substring(pos+1).Trim();
                    bool exist = false;
                    for (int i = 0; i < OBD2Codes.Count; i++)
                    {
                        if (OBD2Codes[i].Code == oc.Code)
                        {
                            exist = true;
                            OBD2Codes[i].Description = oc.Description;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        OBD2Codes.Add(oc);
                    }
                }
            }
            Logger(" [OK]");
            sr.Close();
            
        }
        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Text.Contains("OBD2"))
            {
                ImportOBD2();
            }
            else
            {
                ImportCSV();
            }
        }

        private void dataGridView1_Dataerror(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        public void FilterTables()
        {
            try
            {

                switch (xmlType)
                {
                    case XMLTYPE.Autodetect:
                        List<DetectRule> compareList = detectrules.Where(t => typeof(DetectRule).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareList = compareList.OrderBy(x => typeof(DetectRule).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareList = compareList.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareList;
                        break;
                    case XMLTYPE.Cvn:
                        List<CVN> compareCvnList = sCVN.Where(t => typeof(CVN).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareCvnList = compareCvnList.OrderBy(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareCvnList = compareCvnList.OrderByDescending(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareCvnList;
                        break;
                    case XMLTYPE.DeviceNames:
                        List<IdName> compareDevNames = devicenames.Where(t => typeof(IdName).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareDevNames = compareDevNames.OrderBy(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareDevNames = compareDevNames.OrderByDescending(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareDevNames;
                        break;
                    case XMLTYPE.DtcSearchConfig:
                        List<DtcSearchConfig> compareDTCSearchList = dtcSC.Where(t => typeof(DtcSearchConfig).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareDTCSearchList = compareDTCSearchList.OrderBy(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareDTCSearchList = compareDTCSearchList.OrderByDescending(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareDTCSearchList;
                        break;
                    case XMLTYPE.FileTypes:
                        List<FileType> compareFileTypes = fileTypes.Where(t => typeof(FileType).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareFileTypes = compareFileTypes.OrderBy(x => typeof(FileType).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareFileTypes = compareFileTypes.OrderByDescending(x => typeof(FileType).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareFileTypes;
                        break;
                    case XMLTYPE.FunctionNames:
                        List<IdName> compareFuncNames = funcnames.Where(t => typeof(IdName).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareFuncNames = compareFuncNames.OrderBy(x => typeof(IdName).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareFuncNames = compareFuncNames.OrderByDescending(x => typeof(IdName).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareFuncNames;
                        break;
                    case XMLTYPE.OBD2Codes:
                        List<OBD2Code> compareOBD2List = OBD2Codes.Where(t => typeof(OBD2Code).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareOBD2List = compareOBD2List.OrderBy(x => typeof(OBD2Code).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareOBD2List = compareOBD2List.OrderByDescending(x => typeof(OBD2Code).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareOBD2List;
                        break;
                    case XMLTYPE.TableSeek:
                        List<TableSeek> compareTSeekList = tSeeks.Where(t => typeof(TableSeek).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareTSeekList = compareTSeekList.OrderBy(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareTSeekList = compareTSeekList.OrderByDescending(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareTSeekList;
                        break;
                    case XMLTYPE.TableData:
                        List<TableData> compareTableDatas = PCM.tableDatas.Where(t => typeof(TableData).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareTableDatas = compareTableDatas.OrderBy(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareTableDatas = compareTableDatas.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareTableDatas;
                        break;
                    case XMLTYPE.SegmentSeek:
                        List<SegmentSeek> compareSegmentSeeks = segmentSeeks.Where(t => typeof(SegmentSeek).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareSegmentSeeks = compareSegmentSeeks.OrderBy(x => typeof(SegmentSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareSegmentSeeks = compareSegmentSeeks.OrderByDescending(x => typeof(SegmentSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareSegmentSeeks;
                        break;
                    case XMLTYPE.PidSearchConfig:
                        List<PidSearchConfig> comparePidSearchConfigs = pidSearchConfigs.Where(t => typeof(PidSearchConfig).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            comparePidSearchConfigs = comparePidSearchConfigs.OrderBy(x => typeof(PidSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            comparePidSearchConfigs = comparePidSearchConfigs.OrderByDescending(x => typeof(PidSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = comparePidSearchConfigs;
                        break;
                    case XMLTYPE.PidDescriptions:
                        List<PidInfo> comparePidDescrList = piddescriptions.Where(t => typeof(PidInfo).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            comparePidDescrList = comparePidDescrList.OrderBy(x => typeof(PidInfo).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            comparePidDescrList = comparePidDescrList.OrderByDescending(x => typeof(PidInfo).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = comparePidDescrList;
                        break;
                    case XMLTYPE.SegmentConfig:
                        List<SegmentConfig> compareSegmentConfigs = segmentconfigs.Where(t => typeof(SegmentConfig).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareSegmentConfigs = compareSegmentConfigs.OrderBy(x => typeof(SegmentConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareSegmentConfigs = compareSegmentConfigs.OrderByDescending(x => typeof(SegmentConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareSegmentConfigs;
                        break;
                    case XMLTYPE.RealtimeControl:
                        List<RealTimeControl> compareRtcs = rtControls.Where(t => typeof(RealTimeControl).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareRtcs = compareRtcs.OrderBy(x => typeof(RealTimeControl).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareRtcs = compareRtcs.OrderByDescending(x => typeof(RealTimeControl).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareRtcs;
                        break;
                    case XMLTYPE.Units:
                        List<Units> compareUnitList = unitList.Where(t => typeof(Units).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareUnitList = compareUnitList.OrderBy(x => typeof(Units).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareUnitList = compareUnitList.OrderByDescending(x => typeof(Units).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareUnitList;
                        break;
                    case XMLTYPE.CANmodules:
                        List<CANDevice> compareCanList = canmods.Where(t => typeof(CANDevice).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareCanList = compareCanList.OrderBy(x => typeof(CANDevice).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareCanList = compareCanList.OrderByDescending(x => typeof(CANDevice).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareCanList;
                        break;
                }
                dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                sortBy = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                sortIndex = e.ColumnIndex;
                strSortOrder = GetSortOrder(sortIndex);
                FilterTables();
            }
        }
        private SortOrder GetSortOrder(int columnIndex)
        {
            try
            {
                if (dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                    || dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    return SortOrder.Ascending;
                }
                else
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    return SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return SortOrder.Ascending;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectSaveFile(XmlFilter,fileName);
            if (fName.Length == 0)
                return;
            SaveThis(fName);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmPropertyEditor fpe = new frmPropertyEditor();
            object myObj = dataGridView1.CurrentRow.DataBoundItem;            
            fpe.LoadObject(myObj, xmlType.ToString());
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                Refreshdgrid();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardValue();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Copy to clipboard
                CopyToClipboard();

                //Clear selected cells
                foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                {
                    object obj = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                    string prop = dataGridView1.Columns[dgvCell.ColumnIndex].HeaderText;
                    string itmName = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells[0].Value.ToString();
                    AddToRedoLog(obj, currentList, xmlType.ToString(), itmName, prop, ReDo.RedoAction.Delete, dgvCell.Value, "", dataGridView1.CurrentCell.RowIndex);
                    dgvCell.Value = string.Empty;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmEditXML, line " + line + ": " + ex.Message);
            }
        }

        private void CopyToClipboard()
        {
            try
            {
                //Copy to clipboard
                DataObject dataObj = dataGridView1.GetClipboardContent();
                if (dataObj != null)
                    Clipboard.SetDataObject(dataObj);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        private void PasteClipboardValue()
        {
            try
            {
                //Show Error if no cell is selected
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please select a cell", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Get the starting Cell
                DataGridViewCell startCell = GetStartCell(dataGridView1);
                //Get the clipboard value in a dictionary
                Dictionary<int, Dictionary<int, string>> cbValue =
                        ClipBoardValues(Clipboard.GetText());

                int iRowIndex = startCell.RowIndex;
                foreach (int rowKey in cbValue.Keys)
                {
                    int iColIndex = startCell.ColumnIndex;
                    foreach (int cellKey in cbValue[rowKey].Keys)
                    {
                        //Check if the index is within the limit
                        if (iColIndex <= dataGridView1.Columns.Count - 1
                        && iRowIndex <= dataGridView1.Rows.Count - 1)
                        {                            
                            DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];
                            //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                            object obj = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].DataBoundItem;
                            string prop = dataGridView1.Columns[cell.ColumnIndex].HeaderText;
                            string itmName = dataGridView1.Rows[cell.RowIndex].Cells[0].Value.ToString();
                            AddToRedoLog(obj, currentList, xmlType.ToString(), itmName, prop, ReDo.RedoAction.Edit, cell.Value, cbValue[rowKey][cellKey]);

                            cell.Value = cbValue[rowKey][cellKey];
                            dataGridView1.BeginEdit(true);
                            dataGridView1.NotifyCurrentCellDirty(true);
                            dataGridView1.EndEdit();
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmEditXML, line " + line + ": " + ex.Message);
            }
        }

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            try
            {
                String[] lines = clipboardValue.Split('\n');

                for (int i = 0; i <= lines.Length - 1; i++)
                {
                    copyValues[i] = new Dictionary<int, string>();
                    String[] lineContent = lines[i].Split('\t');

                    //if an empty cell value copied, then set the dictionary with an empty string
                    //else Set value to dictionary
                    if (lineContent.Length == 0)
                        copyValues[i][0] = string.Empty;
                    else
                    {
                        for (int j = 0; j <= lineContent.Length - 1; j++)
                            copyValues[i][j] = lineContent[j];
                    }
                }
            }
            catch { }
            return copyValues;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cutRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                for (int c = 0; c < dataGridView1.SelectedCells.Count; c++)
                    dataGridView1.Rows[dataGridView1.SelectedCells[c].RowIndex].Selected = true;
            }
            ClipBrd = new List<object>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                object obj = row.DataBoundItem;
                ClipBrd.Add(obj);
                string itmName = row.Cells[0].Value.ToString();
                AddToRedoLog(obj, currentList, xmlType.ToString(), itmName, "", ReDo.RedoAction.Delete, "", "", row.Index);
            }
            //CopyToClipboard();
            for (int r = 0; r < dataGridView1.SelectedRows.Count; r++)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
        }

        private void copyRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                for (int c=0; c< dataGridView1.SelectedCells.Count; c++)
                    dataGridView1.Rows[dataGridView1.SelectedCells[c].RowIndex].Selected = true;
            }
            ClipBrd = new List<object>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                string objData = SerializeObjectToXML(row.DataBoundItem);
                object obj = Helpers.XmlDeserializeFromString(objData,currentType);
                ClipBrd.Add(obj);
            }
            //CopyToClipboard();
        }

        private void pasteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Selected = true;
                }
                int row = dataGridView1.SelectedCells[0].RowIndex;
                //currentObj = Activator.CreateInstance(currentObj.GetType());
                object testObj = ClipBrd[0];
                bool isTablSeek = false;
                if (xmlType == XMLTYPE.TableSeek && testObj.GetType() == typeof(TableData))
                {
                    isTablSeek = true;
                }
                dataGridView1.CancelEdit();
                for (int i = 0; i < ClipBrd.Count; i++)
                {
                    //bindingSource.Insert(row, currentObj);
                    if (isTablSeek)
                    {
                        TableData newTd = (TableData)ClipBrd[i];
                        TableSeek newTs = new TableSeek();
                        newTs.ImportTableData(newTd);
                        bindingSource.Insert(row, newTs);
                    }
                    else
                    {
                        string objData = SerializeObjectToXML(ClipBrd[i]);
                        object obj = Helpers.XmlDeserializeFromString(objData, currentType);
                        bindingSource.Insert(row, obj);
                    }
                    dataGridView1.Rows[row].Cells[0].Selected = true;
                    string itmName = dataGridView1.Rows[row].Cells[0].Value.ToString();
                    AddToRedoLog(ClipBrd[i], currentList, xmlType.ToString(), itmName, "", ReDo.RedoAction.Add, "", "",row);
                    //PasteClipboardValue();
                }
                /*                dataGridView1.BeginEdit(true);
                                dataGridView1.NotifyCurrentCellDirty(true);
                                dataGridView1.EndEdit();
                                dataGridView1.ClearSelection();
                */
                //dataGridView1.CancelEdit();
                dataGridView1.Update();
                dataGridView1.Refresh();
                this.Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Selected = true;
                }
                int row = dataGridView1.SelectedCells[0].RowIndex;
                currentObj = Activator.CreateInstance(currentObj.GetType());
                bindingSource.Insert(row, currentObj);
                dataGridView1.BeginEdit(true);
                dataGridView1.NotifyCurrentCellDirty(true);
                dataGridView1.EndEdit();
                dataGridView1.ClearSelection();
                string itmName = dataGridView1.Rows[row].Cells[0].Value.ToString();
                AddToRedoLog(currentObj, currentList, xmlType.ToString(), itmName, "", ReDo.RedoAction.Add, "", "",row);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void scalingConversionFactorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Columns[dataGridView1.Columns.Count - 1].HeaderText != "Std")
            {
                dataGridView1.Columns.Add("Std", "Std");
            }
            frmLogger fl = new frmLogger();
            fl.ReloadPidParams(true,true);
            for (int r=0;r<piddescriptions.Count;r++)
            {
                PidInfo pi = piddescriptions[r];
                string math = MathConverter.ScalingToConversionFactor(pi);
                if (!string.IsNullOrEmpty(math) && math != "X/X" && math != "X*X")
                pi.ConversionFactor = math;
                PidConfig pc = fl.GetConversionFromStd((ushort)pi.PidNumber);
                if (pc != null && dataGridView1.Rows.Count > r)
                {
                    dataGridView1.Rows[r].Cells["Std"].Value = pc.Math;
                }
                //else
                //Debug.WriteLine(dataGridView1.Rows[r].Cells["PidNumber"].Value.ToString());
            }
            dataGridView1.EndEdit();
            bindingSource.EndEdit();
            dataGridView1.Refresh();
            this.Refresh();
        }

        private void testMathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int row = dataGridView1.CurrentCell.RowIndex;
                PidInfo pi = (PidInfo)dataGridView1.Rows[row].DataBoundItem;
                FrmAsk fa = new FrmAsk();
                fa.Text = "Raw value";
                fa.labelAsk.Text = "Raw value (HEX):";
                if (fa.ShowDialog() == DialogResult.OK)
                {
                    if (HexToUint(fa.TextBox1.Text, out UInt32 val))
                    {
                        PidConfig pc = new PidConfig();
                        pc.Math = pi.ConversionFactor;
                        List<SlotHandler.PidVal> PidValues = new List<SlotHandler.PidVal>();
                        SlotHandler.PidVal pv = new SlotHandler.PidVal(pc.addr,val);
                        PidValues.Add(pv);
                        string result = pc.GetCalculatedValue(PidValues);
                        Logger("Result: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void txtSearch_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void comboFilterBy_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void undoRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRedo frmR = new frmRedo();
            frmR.ShowDialog();
            dataGridView1.Update();
            this.Refresh();
        }
    }
}
