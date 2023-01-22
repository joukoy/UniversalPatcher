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
            autodetect,
            tableseek,
            tabledata,
            segmentseek,
            cvn,
            dtcsearchconfig,
            pidsearchconfig,
            piddescriptions,
            units,
            filetypes,
            segmentconfig,
            obdresponses,
            devicenames,
            funcnames,
            obd2codes
        }

        private XMLTYPE xmlType = XMLTYPE.autodetect;
        private BindingSource bindingSource = new BindingSource();
        private bool starting = true;
        private string sortBy = "";
        private int sortIndex = 0;
        SortOrder strSortOrder = SortOrder.Ascending;
        string fileName = "";
        private PcmFile PCM;
        private List<TableSeek> tSeeks;
        private List<SegmentSeek> sSeeks;
        private List<CVN> sCVN;
        private List<DtcSearchConfig> dtcSC;
        private List<PidSearchConfig> pidSC;
        private List<PidInfo> piddescriptions;
        private List<Units> units;
        private List<FileType> fileTypes;
        private List<DetectRule> detectrules;
        private List<SegmentConfig> segmentconfig;
        public List<ObdEmu.OBDResponse> obdresponses;
        private List<IdName> devicenames;
        private List<IdName> funcnames;
        private object currentObj;
        private object currentList;

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
            xmlType = XMLTYPE.autodetect;
            detectrules = new List<DetectRule>();
            currentObj = new DetectRule();
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
            xmlType = XMLTYPE.cvn;
            this.Text = "Edit stock CVN list";
            sCVN = new List<CVN>();
            currentObj = new CVN();
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

        public void LoadDTCSearchConfig()
        {
            xmlType = XMLTYPE.dtcsearchconfig;
            this.Text = "Edit DTC Search config";
            dtcSC = new List<DtcSearchConfig>();
            currentObj = new DtcSearchConfig();
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
            xmlType = XMLTYPE.pidsearchconfig;
            this.Text = "Edit PID Search config";
            pidSC = new List<PidSearchConfig>();
            currentObj = new PidSearchConfig();
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
            xmlType = XMLTYPE.piddescriptions;
            this.Text = "Edit PID Descriptions";
            piddescriptions = new List<PidInfo>();
            currentObj = new PidInfo();
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
            xmlType = XMLTYPE.tableseek;
            fileName = fname;
            this.Text = "Edit Table Seek config";
            currentObj = new TableSeek();
            if (fname.Length > 0 &&  File.Exists(fname))
                tSeeks = TableSeek.LoadTableSeekFile(fname);
            else
                tSeeks = new List<TableSeek>();
            RefreshTableSeek();
            FillFilterBy();
        }

        public void LoadSegmentSeek(string fname)
        {
            xmlType = XMLTYPE.segmentseek;
            fileName = fname;
            this.Text = "Edit Segment Seek config";
            currentObj = new SegmentSeek();
            if (fname.Length > 0 &&  File.Exists(fname))
                sSeeks = SegmentSeek.LoadSegmentSeekFile(fname);
            else
                sSeeks = new List<SegmentSeek>();
            RefreshSegmentSeek();
            FillFilterBy();
        }

        public void LoadObdResponses(string fname)
        {
            xmlType = XMLTYPE.obdresponses;
            fileName = fname;
            this.Text = "Edit OBD Responses";
            currentObj = new ObdEmu.OBDResponse();
            if (fname.Length > 0 && File.Exists(fname))
                obdresponses = ObdEmu.LoadObdResponseFile(fname);
            else
                obdresponses = new List<ObdEmu.OBDResponse>();
            RefreshObdResponses();
            FillFilterBy();
        }

        public void LoadTableData()
        {
            xmlType = XMLTYPE.tabledata;
            this.Text = "Table data";
            currentObj = new TableData();
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
            xmlType = XMLTYPE.segmentconfig;
            this.PCM = PCM;
            segmentconfig = new List<SegmentConfig>();
            currentObj = new SegmentConfig();
            for (int s = 0; s < PCM.Segments.Count; s++)
                segmentconfig.Add(PCM.Segments[s].ShallowCopy());
            this.Text = "Segment Config";
            bindingSource.DataSource = null;
            bindingSource.DataSource = segmentconfig;
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

        public void LoadFileTypes()
        {
            xmlType = XMLTYPE.filetypes;
            this.Text = "File Types";
            fileTypes = new List<FileType>();
            currentObj = new FileType();
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
            xmlType = XMLTYPE.units;
            this.Text = "Units";
            units = new List<Units>();
            currentObj = new Units();
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

        public void LoadDeviceNames()
        {
            xmlType = XMLTYPE.devicenames;
            this.Text = "Device names";
            devicenames = new List<IdName>();
            currentObj = new IdName();
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
            xmlType = XMLTYPE.funcnames;
            this.Text = "Function names";
            funcnames = new List<IdName>();
            currentObj = new IdName();
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
            xmlType = XMLTYPE.obd2codes;
            this.Text = "Edit OBD2 Codes";
            currentObj = new OBD2Code();
            LoadOBD2Codes();
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

            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            //dataGridView1.Columns["ConditionalOffset"].ToolTipText = "If set, and Opcode Address last 2 bytes > 0x5000, Offset = -10000";
            //dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
            UseComboBoxForEnums(dataGridView1);
        }

        private void RefreshSegmentSeek()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = sSeeks;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource;

            //dataGridView1.Columns["DataType"].ToolTipText = "1=Floating, 2=Integer, 3=Hex, 4=Ascii";
            //dataGridView1.Columns["ConditionalOffset"].ToolTipText = "If set, and Opcode Address last 2 bytes > 0x5000, Offset = -10000";
            //dataGridView1.Columns["DataType"].ToolTipText = "UBYTE,SBYTE,UWORD,SWORD,UINT32,INT32,UINT64,INT64,FLOAT32,FLOAT64";
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
                    case XMLTYPE.cvn:
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
                    case XMLTYPE.filetypes:
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
                    case XMLTYPE.dtcsearchconfig:
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
                    case XMLTYPE.pidsearchconfig:
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
                    case XMLTYPE.piddescriptions:
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
                    case XMLTYPE.obdresponses:
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
                    case XMLTYPE.obd2codes:
                        SaveOBD2Codes();
                        break;
                    case XMLTYPE.tableseek:
                        if (fName.Length == 0)
                            fName = SelectSaveFile(XmlFilter, "new-tableseek.xml");
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
                    case XMLTYPE.segmentseek:
                        if (fName.Length == 0)
                            fName = SelectSaveFile(XmlFilter, "new-segmentseek.xml");
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
                    case XMLTYPE.units:
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
                    case XMLTYPE.funcnames:
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
                    case XMLTYPE.devicenames:
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
                    case XMLTYPE.segmentconfig:
                        if (fName.Length == 0)
                            fName = PCM.segmentFile;
                        Logger("Saving file " + fName, false);
                        PCM.Segments = segmentconfig;
                        PCM.SaveConfigFile(fName);
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
            SaveThis(fileName);
            this.Close();
            this.DialogResult = DialogResult.OK;
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
            if (this.Text.Contains("Table Seek") && starting)
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
                switch(xmlType)
                {
                    case XMLTYPE.autodetect:
                        List<DetectRule> compareList = detectrules.Where(t => typeof(DetectRule).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareList = compareList.OrderBy(x => typeof(DetectRule).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareList = compareList.OrderByDescending(x => typeof(TableData).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareList;
                        break;
                    case XMLTYPE.cvn:
                        List<CVN> compareCvnList = sCVN.Where(t => typeof(CVN).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                        if (strSortOrder == SortOrder.Ascending)
                            compareCvnList = compareCvnList.OrderBy(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                        else
                            compareCvnList = compareCvnList.OrderByDescending(x => typeof(CVN).GetProperty(sortBy).GetValue(x, null)).ToList();
                        bindingSource.DataSource = compareCvnList;
                        break;
                    case XMLTYPE.devicenames:
                        break;
                    case XMLTYPE.dtcsearchconfig:
                        break;
                    case XMLTYPE.filetypes:
                        break;
                    case XMLTYPE.funcnames:
                        break;
                    case XMLTYPE.obd2codes:

                    case XMLTYPE.tableseek:
                        break;
                    case XMLTYPE.tabledata:
                        break;
                    case XMLTYPE.segmentseek:
                        break;
                    case XMLTYPE.pidsearchconfig:
                        break;
                    case XMLTYPE.piddescriptions:
                        break;
                    case XMLTYPE.segmentconfig:
                        break;
                    case XMLTYPE.units:
                        break;
                }

                if (this.Text.Contains("Table Seek"))
                {
                    List<TableSeek> compareTSeekList = tSeeks.Where(t => typeof(TableSeek).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                    if (strSortOrder == SortOrder.Ascending)
                        compareTSeekList = compareTSeekList.OrderBy(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareTSeekList = compareTSeekList.OrderByDescending(x => typeof(TableSeek).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareTSeekList;
                }
                else if (this.Text.Contains("Autodetect"))
                {
                }
                else if (this.Text.Contains("stock CVN"))
                {
                }
                else if (this.Text.Contains("DTC Search"))
                {
                    List<DtcSearchConfig> compareDTCSearchList = dtcSC.Where(t => typeof(DtcSearchConfig).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.Trim())).ToList();
                    if (strSortOrder == SortOrder.Ascending)
                        compareDTCSearchList = compareDTCSearchList.OrderBy(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        compareDTCSearchList = compareDTCSearchList.OrderByDescending(x => typeof(DtcSearchConfig).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = compareDTCSearchList;
                }
                else if (this.Text.Contains("PID Descriptions"))
                {
                    List<PidInfo> comparePidDescrList = piddescriptions.Where(t => typeof(PidInfo).GetProperty(comboFilterBy.Text).GetValue(t, null).ToString().ToLower().Contains(txtSearch.Text.ToLower().Trim())).ToList();
                    if (strSortOrder == SortOrder.Ascending)
                        comparePidDescrList = comparePidDescrList.OrderBy(x => typeof(PidInfo).GetProperty(sortBy).GetValue(x, null)).ToList();
                    else
                        comparePidDescrList = comparePidDescrList.OrderByDescending(x => typeof(PidInfo).GetProperty(sortBy).GetValue(x, null)).ToList();
                    bindingSource.DataSource = comparePidDescrList;
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
            fpe.LoadObject(myObj);
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
                    dgvCell.Value = string.Empty;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
                            /*if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                                (!chkPasteToSelectedCells.Checked))*/
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
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
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
            }
            //CopyToClipboard();
            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
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
                object obj = row.DataBoundItem;
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
                bool isTableData = false;
                if (xmlType == XMLTYPE.tableseek && testObj.GetType() == typeof(TableData))
                {
                    isTableData = true;
                }
                for (int i = 0; i < ClipBrd.Count; i++)
                {
                    //bindingSource.Insert(row, currentObj);
                    if (isTableData)
                    {
                        TableData newTd = (TableData)ClipBrd[i];
                        TableSeek newTs = new TableSeek();
                        newTs.ImportTableData(newTd);
                        bindingSource.Insert(row, newTs);
                    }
                    else
                    {
                        bindingSource.Insert(row, ClipBrd[i]);
                    }
                    dataGridView1.Rows[row].Cells[0].Selected = true;
                    //PasteClipboardValue();
                }
                dataGridView1.BeginEdit(true);
                dataGridView1.NotifyCurrentCellDirty(true);
                dataGridView1.EndEdit();
                dataGridView1.ClearSelection();
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
            fl.LoadStdParams();
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
                        string result = pc.GetCalculatedValue(val, 0);
                        Logger("Result: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }


        /*        private void pidDatatypesToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    for (int i=0; i< piddescriptions.Count; i++)
                    {
                        PidInfo pi = piddescriptions[i];
                        switch (pi.Bytes)
                        {
                            case 1:
                                if (pi.Signed)
                                    piddescriptions[i].DataType = InDataType.SBYTE;
                                else
                                    piddescriptions[i].DataType = InDataType.UBYTE;
                                break;
                            case 2:
                                if (pi.Signed)
                                    piddescriptions[i].DataType = InDataType.SWORD;
                                else
                                    piddescriptions[i].DataType = InDataType.UWORD;
                                break;
                            case 3:
                                if (pi.Signed)
                                    piddescriptions[i].DataType = InDataType.THREEBYTES;
                                else
                                    piddescriptions[i].DataType = InDataType.THREEBYTES;
                                break;
                            case 4:
                                if (pi.Signed)
                                    piddescriptions[i].DataType = InDataType.INT32;
                                else
                                    piddescriptions[i].DataType = InDataType.UINT32;
                                break;
                        }
                    }
                    dataGridView1.EndEdit();
                    bindingSource.EndEdit();
                    dataGridView1.Refresh();
                    this.Refresh();

                }
        */
    }
}
