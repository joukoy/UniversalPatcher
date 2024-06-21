using MathParserTK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversalPatcher;
using UniversalPatcher.Properties;
using static Helpers;

public class Upatcher
{
    public Upatcher()
    {
        AppSettings = new UpatcherSettings();
        BadChkFileList = new List<StaticSegmentInfo>();
        parser = new MathParser();
        savingMath = new SavingMath();
    }
    public class DetectRule
    {
        public DetectRule() { }

        public string xml { get; set; }
        public ushort group { get; set; }
        public string grouplogic { get; set; }   //and, or, xor
        public string address { get; set; }
        public UInt64 data { get; set; }
        public string compare { get; set; }        //==, <, >, !=      
        public string hexdata { get; set; }

        public DetectRule ShallowCopy()
        {
            return (DetectRule)this.MemberwiseClone();
        }
    }

    public enum DisplayUnits
    {
        Undefined,
        Metric,
        Imperial
    }

    public enum CSBytes
    {
        _1=1,
        _2 = 2,
        _4=4,
        _8 = 8,
    }

    public class XmlPatch
    {
        public XmlPatch() { }
        public string Name { get; set; }
        public string Description { get; set; }
        public string XmlFile { get; set; }
        public string Segment { get; set; }
        public string CompatibleOS { get; set; }
        public string Data { get; set; }
        public string Rule { get; set; }
        public string HelpFile { get; set; }
        public string PostMessage { get; set; }
    }

    public class Patch
    {
        public Patch()
        {
            patches = new List<XmlPatch>();
        }
        public string Name { get; set; }
        public List<XmlPatch> patches { get; set; }
    }

    public class CVN
    {
        public CVN() 
        {
            AlternateXML = "";
            SegmentNr = "";
            Ver = "";
        }
        public string XmlFile { get; set; }
        public string AlternateXML { get; set; }
        public string SegmentNr { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string cvn { get; set; }
        public string ReferenceCvn { get; set; }

        public CVN ShallowCopy()
        {
            return (CVN)this.MemberwiseClone();
        }
    }
    public struct Block
    {       
        public uint Start;
        public uint End;
    }

    public struct CheckWord
    {
        public string Key;
        public uint Address;
    }

    /*
     File information is read in 3 phases:
     1. XML-file have definitions, how info is stored (SegmentConfig)
     2. Addresses for information is collected from file (SegmentAddressData). 
        For example (OS 12579405): read EngineCal segment address from address 514 => SegmentBlocks => Block1 = 8000 - 15DFFF
        PNAddr is segment address +4 => PNaddr = 8004
     3. Read information from file using collected addresses (SegmentInfo)     
     */
    public struct SegmentAddressData
    {
        public List<Block> SegmentBlocks;
        public List<Block> SwapBlocks;
        public List<Block> CS1Blocks;
        public List<Block> CS2Blocks;
        public List<Block> ExcludeBlocks;
        public AddressData CS1Address;
        public AddressData CS2Address;
        public AddressData PNaddr;
        public AddressData VerAddr;
        public AddressData SegNrAddr;
        public List<CheckWord> Checkwords;
        public List<AddressData> ExtraInfo;
    }

/*    public enum DtcType
    {
        TypeA = 0,
        TypeB = 1,
        Unknown = 99
    }
*/
    public class DtcCode
    {
        public DtcCode()
        {
            codeInt = 0;
            codeAddrInt = uint.MaxValue;
            //CodeAddr = "";
            statusAddrInt = uint.MaxValue;
            TypeAddrInt = uint.MaxValue;
            //StatusAddr = "";
            Description = "";
            Status = 99;
            MilStatus = 99;
            milAddrInt = uint.MaxValue;
            StatusTxt = "";
            StatusMath = "X";
            Type = 0xFF;
            P10 = false;
        }
        public bool P10;
        public UInt16 codeInt;
        public string Code { get; set; }
        public byte Status { get; set; }
        public string StatusTxt { get; set; }
        public byte MilStatus { get; set; }
        public string Description { get; set; }
        public string Values { get; set; }
        public uint codeAddrInt;
        public string CodeAddr
        {
            get
            {
                if (codeAddrInt == uint.MaxValue)
                    return "";
                else
                    return codeAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = codeAddrInt;
                    if (!HexToUint(value, out codeAddrInt))
                        codeAddrInt = prevVal;
                }
                else
                {
                    codeAddrInt = uint.MaxValue;
                }
            }
        }

        public uint statusAddrInt;
        public string StatusAddr
        {
            get
            {
                if (statusAddrInt == uint.MaxValue)
                    return "";
                else
                    return statusAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = statusAddrInt;
                    if (!HexToUint(value, out statusAddrInt))
                        statusAddrInt = prevVal;
                }
                else
                {
                    statusAddrInt = uint.MaxValue;
                }
            }
        }
        public uint milAddrInt;
        public string MilAddr
        {
            get
            {
                if (milAddrInt == uint.MaxValue)
                    return "";
                else
                    return milAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = milAddrInt;
                    if (!HexToUint(value, out milAddrInt))
                        milAddrInt = prevVal;
                }
                else
                {
                    milAddrInt = uint.MaxValue;
                }
            }
        }

        public uint TypeAddrInt;
        public string TypeAddr
        {
            get
            {
                if (TypeAddrInt == uint.MaxValue)
                    return "";
                else
                    return TypeAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = TypeAddrInt;
                    if (!HexToUint(value, out TypeAddrInt))
                        TypeAddrInt = prevVal;
                }
                else
                {
                    TypeAddrInt = uint.MaxValue;
                }
            }
        }
        public uint milAddrInt2;
        public string MilAddr2
        {
            get
            {
                if (milAddrInt2 == uint.MaxValue)
                    return "";
                else
                    return milAddrInt2.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = milAddrInt2;
                    if (!HexToUint(value, out milAddrInt2))
                        milAddrInt2 = prevVal;
                }
                else
                {
                    milAddrInt2 = uint.MaxValue;
                }
            }
        }
        public string StatusMath;
        public byte Type;
        public string TypeTxt
        {
            get
            {
                if (Type == 0)
                    return "TypeA";
                else if (Type == 1)
                    return "TypeB";
                else
                    return "";
            }
        }
        public byte TypeX;
        public string TypeXTxt
        {
            get
            {
                if (Type == 0)
                    return "Non TypeX";
                else if (Type == 1)
                    return "TypeX";
                else
                    return "";
            }
        }
        public uint TypeXAddrInt;
        public string TypeXAddr
        {
            get
            {
                if (TypeXAddrInt == uint.MaxValue)
                    return "";
                else
                    return TypeXAddrInt.ToString("X8");
            }
            set
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = TypeXAddrInt;
                    if (!HexToUint(value, out TypeXAddrInt))
                        TypeXAddrInt = prevVal;
                }
                else
                {
                    TypeXAddrInt = uint.MaxValue;
                }
            }
        }
        public byte StatusByte { get; set; }
        public byte StatusMask { get; set; }
    }

    public class OBD2Code
    {
        public OBD2Code()
        {

        }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TableView
    {
        public TableView()
        { }
        public uint Row { get; set; }
        public uint addrInt;
        public string Address { get; set; }
        public byte dataInt;
        public string Data { get; set; }
    }

    public class SwapSegment
    {
        //For storing information about extracted calibration segments
        public SwapSegment()
        {

        }
        public string FileName { get; set; }
        public string XmlFile { get; set; }
        public string OS { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string SegNr { get; set; }
        public int SegIndex { get; set; }
        public string Size { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Stock { get; set; }
        public string Cvn { get; set; }
        public string SegmentSizes { get; set; }     //For OS compatibility
        public string SegmentAddresses { get; set; } //For OS compatibility

    }
    public struct AddressData
    {
        public string Name;
        public uint Address;
        public ushort Bytes;
        public AddressDataType Type;
    }

    public struct ReferenceCvn
    {
        public string PN;
        public string CVN;
    }

    public class FileType
    {
        public FileType() { }

        public string Description { get; set; }
        public string Extension { get; set; }
        public bool Default { get; set; }

        public FileType ShallowCopy()
        {
            return (FileType)this.MemberwiseClone();
        }
    }

    public class PidInfo
    {
        public PidInfo()
        {
            PidName = "";
            Description = "";
            ConversionFactor = "";
            Unit = "";
            Scaling = "";
            //Bytes = 1;
            DataType = InDataType.UBYTE;
        }
        public uint PidNumber { get; set; }
        public string PidName { get; set; }
        //public int Bytes { get; set; }
        public InDataType DataType { get; set; }
        public string ConversionFactor { get; set; }
        public string Description { get; set; }
        //public bool Signed { get; set; }
        public string Unit { get; set; }
        public string Scaling { get; set; }
        public PidInfo ShallowCopy()
        {
            return (PidInfo)this.MemberwiseClone();
        }
    }

    public struct SearchedAddress
    {
        public uint Addr;
        public ushort Rows;
        public ushort Columns;
    }

    /*    public const short CSMethod_None = 0;
        public const short CSMethod_crc16 = 1;
        public const short CSMethod_crc32 = 2;
        public const short CSMethod_Bytesum = 3;
        public const short CSMethod_Wordsum = 4;
        public const short CSMethod_Dwordsum = 5;
        public const short CSMethod_BoschInv = 6;
    */

    public enum CSMethod
    {
        None = 0,
        crc16 = 1,
        crc32 = 2,
        Bytesum = 3,
        Wordsum = 4,
        Dwordsum = 5,
        BoschInv = 6,
        Ngc3 = 7,
        Unknown = 99
    }

    public enum j2534Command
    {
        NoCommand,
        Initialize,
        Dispose,
        SetConfigParams,
        SetTimeout,
        SetWriteTimeout,
        SetReadTimeout,
        Receive,
        Receive2,
        StartReceiver,
        StopReceiver,
        ReceiveBufferedMessages,
        SendMessage,
        ConnectSecondaryProtocol,
        DisconnectSecondayProtocol,
        Disconnect,
        SetupFilters,
        ClearFilters,
        SetVpwSpeed,
        ClearMessageBuffer,
        SetLoggingFilter,
        SetAnalyzerFilter,
        RemoveFilters,
        SetProgramminVoltage,
        AddToFunctMsgLookupTable,
        DeleteFromFunctMsgLookupTable,
        ClearFunctMsgLookupTable,
        SetJ2534Configs,
        StartPeriodicMsg,
        StopPeriodicMsg,
        ClearPeriodicMsg,
        Ioctl,
        quit
    }
    //
    // Universalpatcher Program global variables
    //

    public static UpatcherSettings AppSettings;

    public static List<Analyzer.IdName> DeviceNames;
    public static List<Analyzer.IdName> FuncNames;

    public static bool J2534FunctionsIsLoaded = false;
    public static List<DetectRule> DetectRules;
    public static List<XmlPatch> PatchList;
    public static List<Patch> patches;
    public static List<CVN> StockCVN;
    public static List<CVN> ListCVN;
    public static List<CVN> BadCvnList;
    public static List<StaticSegmentInfo> SegmentList;
    public static List<StaticSegmentInfo> BadChkFileList;
    public static List<SwapSegment> SwapSegments;
    //public static List<TableSearchConfig> tableSearchConfig;
    public static List<TableSearchResult> tableSearchResult;
    public static List<TableSearchResult> tableSearchResultNoFilters;
    public static List<TableView> tableViews;
    //public static List<ReferenceCvn> referenceCvnList;
    public static List<FileType> fileTypeList;
    public static List<OBD2Code> OBD2Codes;
    public static List<DtcSearchConfig> dtcSearchConfigs;
    public static List<PidSearchConfig> pidSearchConfigs;
    public static List<TableSeek> tableSeeks;
    public static List<SegmentSeek> segmentSeeks;
    //public static List<TableData> XdfElements;
    public static List<Units> unitList;
    public static List<RichTextBox> LogReceivers;
    public static List<PidInfo> PidDescriptions;
    public static List<CANDevice> CanModules;

    public static string tableSearchFile;
    //public static string tableSeekFile = "";
    public static MathParser parser;
    public static SavingMath savingMath;
    public static FrmPatcher frmpatcher;
    public static frmLogger frmlogger;
    private static frmSplashScreen frmSplash;
    public static DataLogger datalogger;
    public static Analyzer analyzer;

    public static CvnDB cvnDB;
    //public static string[] dtcStatusCombined = { "MIL and reporting off", "Type A/no MIL", "Type B/no MIL", "Type C/no MIL", "Not reported/MIL", "Type A/MIL", "Type B/MIL", "Type C/MIL" };
    //public static string[] dtcStatus = { "1 Trip/immediately", "2 Trips", "Store only", "Disabled" };
    public static string selectedCompareBin;


    public enum AddressDataType
    {
        Float = 1,
        Int = 2,
        Hex = 3,
        Text = 4,
        Flag = 5,
        Filename = 10
    }

    public enum OutDataType
    {
        Float = 1,
        Int = 2,
        Hex = 3,
        Text = 4,
        Flag = 5,
        Bitmap = 6,
        Filename = 10
    }

    public enum InDataType
    {
        UBYTE,              //UNSIGNED INTEGER - 8 BIT
        SBYTE,              //SIGNED INTEGER - 8 BIT
        UWORD,              //UNSIGNED INTEGER - 16 BIT
        SWORD,              //SIGNED INTEGER - 16 BIT
        UINT32,              //UNSIGNED INTEGER - 32 BIT
        INT32,              //SIGNED INTEGER - 32 BIT
        UINT64,           //UNSIGNED INTEGER - 64 BIT
        INT64,            //SIGNED INTEGER - 64 BIT
        FLOAT32,       //SINGLE PRECISION FLOAT - 32 BIT
        FLOAT64,        //DOUBLE PRECISION FLOAT - 64 BIT
        //THREEBYTES,
        UNKNOWN
    }

    public enum TableValueType
    {
        boolean,
        selection,
        bitmask,
        bitmap,
        number,
        patch
    }

    public enum Byte_Order
    {
        PlatformOrder,
        MSB,
        LSB
    }

    public static void Startup(string[] args)
    {
        try
        {
            LoadAppSettings();
            ClipBrd = new List<object>();
            if (args.Length > 0)
            {
                if (args[0] == "-")
                {
                    Debug.WriteLine("Remember previous settings");
                }
                else if (args[0].ToLower().Contains("j2534server"))
                {
                    int DevNumber = -1;
                    if (!int.TryParse(args[1], out DevNumber))
                    {
                        MessageBox.Show("Unknown devicenumber: " + args[1]);
                        Environment.Exit(0);
                    }
                    List<J2534DotNet.J2534Device> jDevList = J2534DotNet.J2534Detect.ListDevices();
                    J2534DotNet.J2534Device selectedDevice = jDevList[DevNumber];
                    if (AppSettings.LoggerStartJ2534ProcessDebug)
                    {
                        frmpatcher = new FrmPatcher();
                        frmpatcher.Show();
                    }
                    if (Upatcher.AppSettings.LoggerJ2534ProcessVisible)
                    {
                        Application.Run(new frmJ2534Server(selectedDevice));
                    }
                    else
                    {
                        J2534Server server = new J2534Server(selectedDevice);
                        server.ServerLoop();
                    }
                    return;
                }
                else if (args[0].ToLower().Contains("tourist"))
                    AppSettings.WorkingMode = 0;
                else if (args[0].ToLower().Contains("basic"))
                    AppSettings.WorkingMode = 1;
                else if (args[0].ToLower().Contains("advanced"))
                    AppSettings.WorkingMode = 2;
                else
                {
                    throw new Exception("Usage: " + Path.GetFileName(Application.ExecutablePath) + " [tourist | basic | advanced] [launcher | tuner]");
                }
                AppSettings.Save();
            }

            LogReceivers = new List<RichTextBox>();
            tableSeeks = new List<TableSeek>();
            segmentSeeks = new List<SegmentSeek>();
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "XML")))
            {
                MessageBox.Show("Incomplete installation, Universalpatcher files missing" + Environment.NewLine +
                    "Please extract all files from ZIP package", "Incomplete installation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Directory.CreateDirectory(Path.Combine(Application.StartupPath, "XML"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Patches")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Patches"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Segments")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Segments"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Log")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Log"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Tuner")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Tuner"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Histogram")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Histogram"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Logger")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logger"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Logger", "Log")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logger", "Log"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Logger", "Profiles")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logger", "Profiles"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logger", "DisplayProfiles"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Logger", "J2534Profiles")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logger", "J2534Profiles"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "Logger", "ospids")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "Logger", "ospids"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "ChecksumSearch")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "ChecksumSearch"));
            if (!Directory.Exists(Path.Combine(Application.StartupPath, "TuneSessions")))
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "TunerSessions"));
            if (AppSettings.LastXMLfolder == "")
                AppSettings.LastXMLfolder = Path.Combine(Application.StartupPath, "XML");
            if (AppSettings.LastPATCHfolder == "")
                AppSettings.LastPATCHfolder = Path.Combine(Application.StartupPath, "Patches");

            frmSplash = new frmSplashScreen();
            if (AppSettings.SplashShowTime > 0)
                frmSplash.Show();
            //System.Drawing.Point xy = new Point((int)(this.Location.X + 300), (int)(this.Location.Y + 150));
            Screen myScreen = Screen.FromPoint(Control.MousePosition);
            System.Drawing.Rectangle area = myScreen.WorkingArea;
            Point xy = new Point(area.Width / 2 - 115, area.Height / 2 - 130);
            frmSplash.moveMe(xy);
            frmSplash.labelProgress.Text = "";
            LoadSettingFiles();

            if (args.Length > 1)
            {
                if (args[1].ToLower().Contains("launcher"))
                {
                    Application.Run(new FrmMain());
                }
                else if (args[1].ToLower().Contains("tuner"))
                {
                    PcmFile pcm = new PcmFile();
                    if (AppSettings.TunerUseSessionTabs)
                    {
                        Application.Run(new frmTunerMain(pcm));
                    }
                    else
                    {
                        Application.Run(new FrmTuner(pcm));
                    }
                }
                else if (args[1].ToLower().Contains("patcher"))
                {
                    Application.Run(new FrmPatcher());
                }
                else if (args[1].ToLower().Contains("logger"))
                {
                    Application.Run(new frmLogger());
                }
                else
                {
                    //Application.Run(new FrmPatcher());
                    PcmFile pcm = new PcmFile();
                    Application.Run(new FrmTuner(pcm));
                }
            }
            else
            {
                PcmFile pcm = new PcmFile();
                if (AppSettings.TunerUseSessionTabs)
                {
                    Application.Run(new frmTunerMain(pcm));
                }
                else
                {
                    Application.Run(new FrmTuner(pcm));
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
            Debug.WriteLine("Patcherfunctions error, line " + line + ": " + ex.Message);
        }
    }

    private static void ShowSplash(string txt, bool newLine = true)
    {
        frmSplash.labelProgress.Text += txt;
        if (newLine)
            frmSplash.labelProgress.Text += Environment.NewLine;
    }

    private static void LoadAppSettings()
    {
        try
        {
            string appSettingsFile = Path.Combine(Application.StartupPath, "universalpatcher.xml");
            if (File.Exists(appSettingsFile))
            {
                Debug.WriteLine("Loading universalpatcher.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(UpatcherSettings));
                System.IO.StreamReader file = new System.IO.StreamReader(appSettingsFile);
                AppSettings = (UpatcherSettings)reader.Deserialize(file);
                file.Close();
            }
            else
            {
                AppSettings = new UpatcherSettings();
            }
        }
        catch (Exception ex)
        {
            AppSettings = new UpatcherSettings();
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.WriteLine("Patcherfunctions error, line " + line + ": " + ex.Message);
        }
    }

    private static void LoadSettingFiles()
    {
        try
        {
            DetectRules = new List<DetectRule>();
            StockCVN = new List<CVN>();
            fileTypeList = new List<FileType>();
            dtcSearchConfigs = new List<DtcSearchConfig>();
            pidSearchConfigs = new List<PidSearchConfig>();
            SwapSegments = new List<SwapSegment>();
            unitList = new List<Units>();
            patches = new List<Patch>();
            //Dirty fix to make system work without stockcvn.xml
            CVN ctmp = new CVN();
            ctmp.cvn = "";
            ctmp.PN = "";
            StockCVN.Add(ctmp);

            cvnDB = new CvnDB();
            cvnDB.LoadDB();

            Logger("Loading configurations... filetypes", false);
            ShowSplash("Loading configurations...");
            ShowSplash("filetypes");
            Application.DoEvents();

            Logger("Loading Pid descriptions...", false);
            ShowSplash("Pid descriptions");
            LoadPidDescriptions();
            Application.DoEvents();

            string FileTypeListFile = Path.Combine(Application.StartupPath, "XML", "filetypes.xml");
            if (File.Exists(FileTypeListFile))
            {
                Debug.WriteLine("Loading filetypes.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<FileType>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileTypeListFile);
                fileTypeList = (List<FileType>)reader.Deserialize(file);
                file.Close();

            }

            Logger(",Function names", false);
            ShowSplash("Function names");
            Application.DoEvents();
            string funcNameFile = Path.Combine(Application.StartupPath, "XML", "functionnames.xml");
            if (File.Exists(funcNameFile))
            {
                Debug.WriteLine("Loading functionnames.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Analyzer.IdName>));
                System.IO.StreamReader file = new System.IO.StreamReader(funcNameFile);
                FuncNames = (List<Analyzer.IdName>)reader.Deserialize(file);
                file.Close();
            }

            Logger(",CANmodules", false);
            ShowSplash("CANmodules");
            Application.DoEvents();
            string canmodulefile = Path.Combine(Application.StartupPath, "XML", "CANModules.xml");
            if (File.Exists(canmodulefile))
            {
                Debug.WriteLine("Loading " + canmodulefile);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CANDevice>));
                System.IO.StreamReader file = new System.IO.StreamReader(canmodulefile);
                CanModules = (List<CANDevice>)reader.Deserialize(file);
                file.Close();
            }

            Logger(",Devicenames", false);
            ShowSplash("Devicenames");
            Application.DoEvents();
            string deviceNameFile = Path.Combine(Application.StartupPath, "XML", "devicenames.xml");
            if (File.Exists(deviceNameFile))
            {
                Debug.WriteLine("Loading devicenames.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Analyzer.IdName>));
                System.IO.StreamReader file = new System.IO.StreamReader(deviceNameFile);
                DeviceNames = (List<Analyzer.IdName>)reader.Deserialize(file);
                file.Close();
            }

            Logger(",dtcsearch", false);
            ShowSplash("dtcsearch");
            Application.DoEvents();
            string CtsSearchFile = Path.Combine(Application.StartupPath, "XML", "DtcSearch.xml");
            if (File.Exists(CtsSearchFile))
            {
                Debug.WriteLine("Loading DtcSearch.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<DtcSearchConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(CtsSearchFile);
                dtcSearchConfigs = (List<DtcSearchConfig>)reader.Deserialize(file);
                file.Close();

            }

            Logger(",pidsearch", false);
            ShowSplash("pidsearch");
            Application.DoEvents();

            string pidSearchFile = Path.Combine(Application.StartupPath, "XML", "PidSearch.xml");
            if (File.Exists(pidSearchFile))
            {
                Debug.WriteLine("Loading PidSearch.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidSearchConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(pidSearchFile);
                pidSearchConfigs = (List<PidSearchConfig>)reader.Deserialize(file);
                file.Close();

            }

            Logger(",autodetect", false);
            ShowSplash("autodetect");
            Application.DoEvents();

            string AutoDetectFile = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
            if (File.Exists(AutoDetectFile))
            {
                Debug.WriteLine("Loading autodetect.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                System.IO.StreamReader file = new System.IO.StreamReader(AutoDetectFile);
                DetectRules = (List<DetectRule>)reader.Deserialize(file);
                file.Close();
            }

            Logger(",extractedsegments", false);
            ShowSplash("extractedsegments");
            Application.DoEvents();

            string SwapSegmentListFile = Path.Combine(Application.StartupPath, "Segments", "extractedsegments.xml");
            if (File.Exists(SwapSegmentListFile))
            {
                Debug.WriteLine("Loading extractedsegments.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<SwapSegment>));
                System.IO.StreamReader file = new System.IO.StreamReader(SwapSegmentListFile);
                SwapSegments = (List<SwapSegment>)reader.Deserialize(file);
                file.Close();

            }

            Logger(",units", false);
            ShowSplash("units");
            Application.DoEvents();

            string unitsFile = Path.Combine(Application.StartupPath, "Tuner", "units.xml");
            if (File.Exists(unitsFile))
            {
                Debug.WriteLine("Loading units.xml");
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<Units>));
                System.IO.StreamReader file = new System.IO.StreamReader(unitsFile);
                unitList = (List<Units>)reader.Deserialize(file);
                file.Close();

            }

            Logger(",OBD2 Codes", false);
            ShowSplash("OBD2 Codes");
            Application.DoEvents();
            LoadOBD2Codes();
            Logger(" - Done");
            ShowSplash("Done");
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, j2534Client line " + line + ": " + ex.Message);
        }

    }

    public static int GetBits(InDataType dataType)
    {
        int bits = 8; // Assume one byte if not defined. OK?
        if (dataType == InDataType.SBYTE || dataType == InDataType.UBYTE)
            bits = 8;
        if (dataType == InDataType.SWORD || dataType == InDataType.UWORD)
            bits = 16;
        if (dataType == InDataType.INT32 || dataType == InDataType.UINT32 || dataType == InDataType.FLOAT32)
            bits = 32;
        if (dataType == InDataType.INT64 || dataType == InDataType.UINT64 || dataType == InDataType.FLOAT64)
            bits = 64;
        if (dataType == InDataType.UNKNOWN)
            Logger("Warning, unknown data type. Assuming UBYTE");

        return bits;
    }
    public static int GetElementSize(InDataType dataType)
    {
        int bytes = 1; // Assume one byte if not defined. OK?
        if (dataType == InDataType.SBYTE || dataType == InDataType.UBYTE)
            bytes = 1;
        if (dataType == InDataType.SWORD || dataType == InDataType.UWORD)
            bytes = 2;
        if (dataType == InDataType.INT32 || dataType == InDataType.UINT32 || dataType == InDataType.FLOAT32)
            bytes = 4;
        if (dataType == InDataType.INT64 || dataType == InDataType.UINT64 || dataType == InDataType.FLOAT64)
            bytes = 8;
        if (dataType == InDataType.UNKNOWN)
            Logger("Warning, unknown data type. Assuming UBYTE");

        return bytes;
    }

    public static bool GetSigned(InDataType dataType)
    {
        bool signed = false;
        if (dataType == InDataType.INT32 || dataType == InDataType.INT64 || dataType == InDataType.SBYTE || dataType == InDataType.SWORD)
            signed = true;
        return signed;
    }

    public enum PidDataType
    {
        uint8 = 1,
        uint16 = 2,
        int8,
        int16,
        uint32,
        int32
    }

    public static InDataType ConvertToDataType(string bitStr, bool signed, bool floating)
    {
        InDataType retVal = InDataType.UNKNOWN;
        int bits = Convert.ToInt32(bitStr);
        retVal = ConvertToDataType(bits / 8, signed, floating);
        return retVal;
    }

    public static InDataType ConvertToDataType(int elementSize, bool Signed, bool floating)
    {
        InDataType DataType = InDataType.UNKNOWN; 
        if (elementSize == 1)
        {
            if (Signed == true)
            {
                DataType = InDataType.SBYTE;
            }
            else
            {
                DataType = InDataType.UBYTE;
            }

        }
        else if (elementSize == 2)
        {
            if (Signed == true)
            {
                DataType = InDataType.SWORD;
            }
            else
            {
                DataType = InDataType.UWORD;
            }

        }
        else if (elementSize == 3)
        {
            DataType = InDataType.UINT32;
        }
        else if (elementSize == 4)
        {
            if (floating)
            {
                DataType = InDataType.FLOAT32;
            }
            else
            {
                if (Signed == true)
                {
                    DataType = InDataType.UINT32;
                }
                else
                {
                    DataType = InDataType.INT32;
                }
            }
        }
        else if (elementSize == 8)
        {
            if (floating)
            {
                DataType = InDataType.FLOAT64;
            }
            else
            {
                if (Signed == true)
                {
                    DataType = InDataType.INT64;
                }
                else
                {
                    DataType = InDataType.UINT64;
                }
            }

        }
        return DataType;
    }

    public static double GetMaxValue (InDataType dType)
    {
        if (dType == InDataType.FLOAT32)
            return float.MaxValue;
        else if (dType == InDataType.FLOAT64)
            return double.MaxValue;
        else if (dType == InDataType.INT32)
            return Int32.MaxValue;
        else if (dType == InDataType.INT64)
            return Int64.MaxValue;
        else if (dType == InDataType.SBYTE)
            return sbyte.MaxValue;
        else if (dType == InDataType.SWORD)
            return Int16.MaxValue;
        else if (dType == InDataType.UBYTE)
            return byte.MaxValue;
        else if (dType == InDataType.UINT32)
            return UInt32.MaxValue;
        else if (dType == InDataType.UINT64)
            return UInt64.MaxValue;
        else if (dType == InDataType.UWORD)
            return UInt16.MaxValue;
        else 
            return byte.MaxValue;

    }

    public static double GetMinValue(InDataType dType)
    {
        if (dType == InDataType.FLOAT32)
            return float.MinValue;
        else if (dType == InDataType.FLOAT64)
            return double.MinValue;
        else if (dType == InDataType.INT32)
            return Int32.MinValue;
        else if (dType == InDataType.INT64)
            return Int64.MinValue;
        else if (dType == InDataType.SBYTE)
            return sbyte.MinValue;
        else if (dType == InDataType.SWORD)
            return Int16.MinValue;
        else if (dType == InDataType.UBYTE)
            return byte.MinValue;
        else if (dType == InDataType.UINT32)
            return UInt32.MinValue;
        else if (dType == InDataType.UINT64)
            return UInt64.MinValue;
        else if (dType == InDataType.UWORD)
            return UInt16.MinValue;
        else
            return byte.MinValue;
    }

    public static string ReadConversionTable(string mathStr, PcmFile PCM)
    {
        //Example: TABLE:'MAF Scalar #1'
        string retVal = mathStr;
        int start = mathStr.IndexOf("table:") + 6;
        int mid = mathStr.IndexOf("'", start + 7);
        string conversionTable = mathStr.Substring(start, mid - start + 1);
        TableData tmpTd = new TableData();
        tmpTd.TableName = conversionTable.Replace("'", "");
        TableData conversionTd = FindTableData(tmpTd, PCM.tableDatas);
        if (conversionTd != null)
        {
            double conversionVal = GetValue(PCM.buf, (uint)(conversionTd.addrInt + conversionTd.Offset + conversionTd.ExtraOffset), conversionTd, 0, PCM);
            retVal = mathStr.Replace("table:" + conversionTable, conversionVal.ToString());
            Debug.WriteLine("Using conversion table: " + conversionTd.TableName);
        }
        return retVal;
    }

    public static string ReadConversionRaw(string mathStr, PcmFile PCM)
    {
        // Example: RAW:0x321:SWORD:MSB
        string retVal = mathStr;
        int start = mathStr.IndexOf("raw:");
        int mid = mathStr.IndexOf(" ", start + 3);
        string rawStr = mathStr.Substring(start, mid - start + 1);
        string[] rawParts = rawStr.Split(':');
        if (rawParts.Length < 3)
        {
            throw new Exception("Unknown RAW definition in Math: " + mathStr);
        }
        InDataType idt =(InDataType) Enum.Parse(typeof(InDataType), rawParts[2].ToUpper());
        TableData tmpTd = new TableData();
        tmpTd.Address = rawParts[1];
        tmpTd.Offset = 0;
        tmpTd.DataType = idt;
        double rawVal = (double)GetRawValue(PCM.buf, tmpTd.addrInt, tmpTd, 0,PCM.platformConfig.MSB);
        if (rawParts.Length > 3 && rawParts[3].StartsWith("lsb"))
        {
            int eSize = GetElementSize(idt);
            rawVal = SwapBytes((UInt64)rawVal,eSize);
        }
        retVal = mathStr.Replace(rawStr, rawVal.ToString());
        return retVal;
    }

    public static string[] LoadHeaderFromTable(TableData headerTd, int count, PcmFile pcm)
    {
        try
        {
            if (headerTd == null)
                return new string[0];

            uint step = (uint)(GetBits(headerTd.DataType) / 8);
            uint addr = (uint)(headerTd.addrInt + headerTd.Offset + headerTd.ExtraOffset);
            string[] retVal = new string[count];
            for (int a = 0; a < count; a++)
            {
                string formatStr = "0.####";
                if (headerTd.Units.Contains("%"))
                    formatStr = "0";
                string header = "";
                if (!string.IsNullOrEmpty(headerTd.Units))
                    header = headerTd.Units.Trim() + " ";
                header += GetValue(pcm.buf, addr, headerTd, 0, pcm).ToString(formatStr).Replace(",", ".");
                retVal[a] = header;
                addr += step;
            }
            return retVal;
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.WriteLine("Patcherfunctions error, line " + line + ": " + ex.Message);
        }
        return new string[0];
    }

    public static double[] LoadHeaderValuesFromTable(TableData headerTd, int count, PcmFile pcm)
    {
        if (headerTd == null)
            return null;

        uint step = (uint)(GetBits(headerTd.DataType) / 8);
        uint addr = (uint)(headerTd.addrInt + headerTd.Offset + headerTd.ExtraOffset);
        double[] retVal = new double[count];
        for (int a = 0; a < count; a++)
        {
            double val = GetValue(pcm.buf, addr, headerTd, 0, pcm);
            retVal[a] = val;
            addr += step;
        }
        return retVal;
    }
    public static double GetValueByRowColumn(byte[] myBuffer, TableData mathTd, int offset, ushort row, ushort column, PcmFile PCM)
    {
        double retVal = 0;
        try
        {
            bool msb = PCM.platformConfig.MSB;
            switch (mathTd.ByteOrder)
            {
                case Byte_Order.PlatformOrder:
                    msb = PCM.platformConfig.MSB;
                    break;
                case Byte_Order.MSB:
                    msb = true;
                    break;
                case Byte_Order.LSB:
                    msb = false;
                    break;
            }

            uint addr = (uint)(mathTd.addrInt + mathTd.Offset + mathTd.ExtraOffset);
            UInt32 bufAddr = (UInt32)(addr - offset);
            uint elemSize = (uint)mathTd.ElementSize();
            byte byteMask = 0x80;
            if (mathTd.RowMajor)
            {
                for (int r = 0; r < mathTd.Rows; r++)
                {
                    if (r > row) break;
                    for (int c = 0; c < mathTd.Columns; c++)
                    {
                        if (mathTd.OutputType == OutDataType.Bitmap)
                        {

                            if (byteMask == 1)
                            {
                                byteMask = 0x80;
                                bufAddr++;
                            }
                            else
                            {
                                byteMask = (byte)(byteMask >> 1);
                            }
                        }
                        else
                        {
                            bufAddr += elemSize;
                        }
                        if (r == row && c == column) break;
                    }
                }
            }
            else
            {
                for (int c = 0; c < mathTd.Columns; c++)
                {
                    if (c > column) break;
                    for (int r = 0; r < mathTd.Rows; r++)
                    {
                        if (mathTd.OutputType == OutDataType.Bitmap)
                        {

                            if (byteMask == 1)
                            {
                                byteMask = 0x80;
                                bufAddr++;
                            }
                            else
                            {
                                byteMask = (byte)(byteMask >> 1);
                            }
                        }
                        else
                        {
                            bufAddr += elemSize;
                        }
                        if (r == row && c == column) break;
                    }
                }
            }

            if (mathTd.Math.StartsWith("DTC"))
            {
                int codeIndex = (int)(addr - mathTd.addrInt);
                switch (mathTd.Math.Substring(4))
                {
                    case "DTC_Enable":
                        return PCM.dtcCodes[codeIndex].Status;
                    case "MIL_Enable":
                        return PCM.dtcCodes[codeIndex].MilStatus;
                    case "Type":
                        return PCM.dtcCodes[codeIndex].Type;
                    default:
                        throw new Exception("Unknown Math: " + mathTd.Math);
                }
            }

            if (mathTd.OutputType == OutDataType.Bitmap)
            {
                if ((myBuffer[bufAddr] & byteMask) == byteMask)
                    return 1;
                else
                    return 0;
            }

            if (mathTd.OutputType == OutDataType.Flag && !string.IsNullOrEmpty(mathTd.BitMask))
            {
                UInt64 mask;
                mask = Convert.ToUInt64(mathTd.BitMask.Replace("0x", ""), 16);
                UInt64 rawVal = (UInt64)GetRawValue(myBuffer, bufAddr, mathTd, offset, PCM.platformConfig.MSB);
                if (((UInt64)rawVal & mask) == mask)
                    return 1;
                else
                    return 0;
            }


            if (mathTd.DataType == InDataType.SBYTE)
                retVal = (sbyte)myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.UBYTE)
                retVal = myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.SWORD)
                retVal = ReadInt16(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.UWORD)
                retVal = ReadUint16(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.INT32)
                retVal = ReadInt32(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.UINT32)
                retVal = ReadUint32(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.INT64)
                retVal = ReadInt64(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.UINT64)
                retVal = ReadUint64(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.FLOAT32)
                retVal = ReadFloat32(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.FLOAT64)
                retVal = ReadFloat64(myBuffer, bufAddr, msb);

            if (string.IsNullOrEmpty(mathTd.Math))
                mathTd.Math = "X";
            string mathStr = mathTd.Math.ToLower().Replace("x", retVal.ToString());
            if (mathStr.Contains("table:"))
            {
                mathStr = ReadConversionTable(mathStr, PCM);
            }
            if (mathStr.Contains("raw:"))
            {
                mathStr = ReadConversionRaw(mathStr, PCM);
            }
            retVal = parser.Parse(mathStr, false);
            //Debug.WriteLine(mathStr);
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.WriteLine("Patcherfunctions error, line " + line + ": " + ex.Message);
        }
        return retVal;
    }


    //
    //Get value from defined table, using defined math functions.
    //
    public static double GetValue(byte[] myBuffer, uint addr, TableData mathTd, int offset, PcmFile PCM)
    {
        double retVal = 0;
        try
        {
            bool msb = PCM.platformConfig.MSB;
            switch (mathTd.ByteOrder)
            {
                case Byte_Order.PlatformOrder:
                    msb = PCM.platformConfig.MSB;
                    break;
                case Byte_Order.MSB:
                    msb = true;
                    break;
                case Byte_Order.LSB:
                    msb = false;
                    break;
            }

            if (mathTd.Math.StartsWith("DTC"))
            {
                int codeIndex = (int)(addr - mathTd.addrInt);
                switch(mathTd.Math.Substring(4))
                {
                    case "DTC_Enable":
                        return PCM.dtcCodes[codeIndex].Status;
                    case "MIL_Enable":
                        return PCM.dtcCodes[codeIndex].MilStatus;
                    case "Type":
                        return PCM.dtcCodes[codeIndex].Type;
                    case "TypeX":
                        return PCM.dtcCodes[codeIndex].TypeX;
                    default:
                        throw new Exception("Unknown Math: " + mathTd.Math);
                }
            }

            UInt32 bufAddr = (UInt32)(addr - offset);

            if (mathTd.OutputType == OutDataType.Flag && !string.IsNullOrEmpty(mathTd.BitMask))
            {
                UInt64 mask;
                mask = Convert.ToUInt64(mathTd.BitMask.Replace("0x", ""), 16);
                UInt64 rawVal = (UInt64)GetRawValue(myBuffer, bufAddr, mathTd, offset, PCM.platformConfig.MSB);
                if (((UInt64) rawVal & mask) == mask)
                    return 1;
                else
                    return 0;
            }


            if (mathTd.DataType == InDataType.SBYTE)
                retVal = (sbyte)myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.UBYTE)
                retVal = myBuffer[bufAddr];
            if (mathTd.DataType == InDataType.SWORD)
                retVal = ReadInt16(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.UWORD)
                retVal = ReadUint16(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.INT32)
                retVal = ReadInt32(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.UINT32)
                retVal = ReadUint32(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.INT64)
                retVal = ReadInt64(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.UINT64)
                retVal = ReadUint64(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.FLOAT32)
                retVal = ReadFloat32(myBuffer, bufAddr, msb);
            if (mathTd.DataType == InDataType.FLOAT64)
                retVal = ReadFloat64(myBuffer, bufAddr, msb);

            if (string.IsNullOrEmpty(mathTd.Math))
                mathTd.Math = "X";
            string mathStr = mathTd.Math.ToLower().Replace("x", retVal.ToString());
            if (mathStr.Contains("table:"))
            {
                mathStr = ReadConversionTable(mathStr, PCM);
            }
            if (mathStr.Contains("raw:"))
            {
                mathStr = ReadConversionRaw(mathStr, PCM);
            }
            retVal = parser.Parse(mathStr, false);
            //Debug.WriteLine(mathStr);
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.WriteLine("Patcherfunctions error, line " + line + ": " + ex.Message);
        }
        return retVal;
    }

    public static double GetRawValue(byte[] myBuffer, UInt32 addr, TableData mathTd, int offset, bool platformMsb)
    {
        UInt32 bufAddr = (uint)(addr - offset);
        double retVal = 0;
        try
        {
            bool msb = platformMsb;
            if (mathTd.OutputType == OutDataType.Bitmap)
            {
                msb = true;
            }
            else
            {
                switch(mathTd.ByteOrder)
                {
                    case Byte_Order.PlatformOrder:
                        msb = platformMsb;
                        break;
                    case Byte_Order.MSB:
                        msb = true;
                        break;
                    case Byte_Order.LSB:
                        msb = false;
                        break;
                }
            }
            switch (mathTd.DataType)
            {
                case InDataType.SBYTE:
                    return (sbyte)myBuffer[bufAddr];
                case InDataType.UBYTE:
                    return (byte)myBuffer[bufAddr];
                case InDataType.SWORD:
                    return (Int16)ReadInt16(myBuffer, bufAddr, msb);
                case InDataType.UWORD:
                    return (UInt16)ReadUint16(myBuffer, bufAddr, msb);
                case InDataType.INT32:
                    return (Int32)ReadInt32(myBuffer, bufAddr, msb);
                case InDataType.UINT32:
                    return (UInt32)ReadUint32(myBuffer, bufAddr, msb);
                case InDataType.INT64:
                    return (Int64)ReadInt64(myBuffer, bufAddr, msb);
                case InDataType.UINT64:
                    return (UInt64)ReadInt64(myBuffer, bufAddr, msb);
                case InDataType.FLOAT32:
                    return (float)ReadFloat32(myBuffer, bufAddr, msb);
                case InDataType.FLOAT64:
                    return ReadFloat64(myBuffer, bufAddr, msb);
            }

        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Patcherfunctions error, line " + line + ": " + ex.Message);
        }

        return retVal;
    }


    public static Dictionary<double, string> ParseEnumHeaders(string eVals)
    {
        if (eVals.ToLower().StartsWith("enum:"))
            eVals = eVals.Substring(5).Trim();
        Dictionary<double, string> retVal = new Dictionary<double, string>();
        string[] posVals = eVals.Split(',');
        for (int r = 0; r < posVals.Length; r++)
        {
            string[] parts = posVals[r].Split(':');
            double val = 0;
            double.TryParse(parts[0], out val);
            string txt = posVals[r];
            if (!retVal.ContainsKey(val))
                retVal.Add(val, txt);
        }
        retVal.Add(double.MaxValue, "------------");
        return retVal;
    }

    public static Dictionary<int, string> ParseIntEnumHeaders(string eVals)
    {
        if (eVals.ToLower().StartsWith("enum:"))
            eVals = eVals.Substring(5).Trim();
        Dictionary<int, string> retVal = new Dictionary<int, string>();
        string[] posVals = eVals.Split(',');
        for (int r = 0; r < posVals.Length; r++)
        {
            string[] parts = posVals[r].Split(':');
            int val = 0;
            int.TryParse(parts[0], out val);
            string txt = posVals[r];
            if (!retVal.ContainsKey(val))
                retVal.Add(val, txt);
        }
        retVal.Add(int.MaxValue, "------------");
        return retVal;
    }

    public static Dictionary<byte, string> ParseDtcValues(string eVals)
    {
        if (eVals.ToLower().StartsWith("enum:"))
            eVals = eVals.Substring(5).Trim();
        Dictionary<byte, string> retVal = new Dictionary<byte, string>();
        string[] posVals = eVals.Split(',');
        for (int r = 0; r < posVals.Length; r++)
        {
            string[] parts = posVals[r].Split(':');
            byte val = 0;
            byte.TryParse(parts[0], out val);
            string txt = parts[1];
            if (!retVal.ContainsKey(val))
                retVal.Add(val, txt);
        }
        retVal.Add(byte.MaxValue, "------------");
        return retVal;
    }

    public static uint CheckPatchCompatibility(XmlPatch xpatch, PcmFile basefile, bool newline = true)
    {
        uint retVal = uint.MaxValue;
        bool isCompatible = false;
        string[] Parts = xpatch.XmlFile.Split(',');
        foreach (string Part in Parts)
        {
            if (Part.ToLower().Replace(".xml","") == basefile.configFile)
                isCompatible = true;
        }
        if (!isCompatible)
        {
            LoggerBold("Incompatible patch, current configfile: " + basefile.configFile + ", patch configile: " + xpatch.XmlFile.Replace(".xml", ""));
            return retVal;
        }

        if (xpatch.CompatibleOS.ToLower().StartsWith("search:"))
        {
            string searchStr = xpatch.CompatibleOS.Substring(7);
            for (int seg = 0; seg < basefile.segmentinfos.Length; seg++)
            {
                if (basefile.segmentinfos[seg].Name.ToLower() == xpatch.Segment.ToLower())
                {
                    Debug.WriteLine("Searching only segment: " + basefile.segmentinfos[seg].Name);
                    for (int b = 0; b < basefile.segmentAddressDatas[seg].SegmentBlocks.Count; b++)
                    {
                        retVal = SearchBytes(basefile, searchStr, basefile.segmentAddressDatas[seg].SegmentBlocks[b].Start, basefile.segmentAddressDatas[seg].SegmentBlocks[b].End);
                        if (retVal < uint.MaxValue)
                            break;
                    }
                }
            }
            if (retVal == uint.MaxValue) //Search whole bin
                retVal = SearchBytes(basefile, searchStr, 0, basefile.fsize);
            if (retVal < uint.MaxValue)
            {
                Logger("Data found at address: " + retVal.ToString("X8"));
                isCompatible = true;
            }
            else
            {
                uint tmpVal = SearchBytes(basefile, xpatch.Data, 0, basefile.fsize);
                if (tmpVal < uint.MaxValue)
                    Logger("Patch is already applied, data found at: " + tmpVal.ToString("X8"));
                else
                    LoggerBold("Data not found. Incompatible patch");
            }
        }
        else if (xpatch.CompatibleOS.ToLower().StartsWith("table:"))
        {
            if (basefile.tableDatas.Count < 3)
                basefile.AutoLoadTunerConfig();
            basefile.ImportDTC();
            basefile.ImportSeekTables();
            string[] tableParts = xpatch.CompatibleOS.Split(',');
            if (tableParts.Length < 3)
            {
                LoggerBold("Incomplete table definition:" + xpatch.CompatibleOS);
            }
            else
            {
                string tbName = "";
                int rows = 1;
                int columns = 1;
                for (int i = 0; i < tableParts.Length; i++)
                {
                    string[] xParts = tableParts[i].Split(':');
                    if (xParts[0].ToLower() == "table")
                        tbName = xParts[1];
                    if (xParts[0].ToLower() == "columns")
                        columns = Convert.ToInt32(xParts[1]);
                    if (xParts[0].ToLower() == "rows")
                        rows = Convert.ToInt32(xParts[1]);
                }
                TableData tmpTd = new TableData();
                tmpTd.TableName = tbName;
                Logger("Table: " + tbName,newline);
                int findId = FindTableDataId(tmpTd, basefile.tableDatas);
                if (findId > -1)
                {
                    TableData findTd = basefile.tableDatas[findId];
                    if (findTd.Rows == rows && findTd.Columns == columns)
                    {
                        isCompatible = true;
                        retVal = (uint)findId;
                    }
                }
            }
        }
        else
        {
            string[] OSlist = xpatch.CompatibleOS.Split(',');
            string BinPN = "";
            foreach (string OS in OSlist)
            {
                Parts = OS.Split(':');
                if (Parts[0] == "ALL")
                {
                    isCompatible = true;
                    if (!HexToUint(Parts[1], out retVal))
                        throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                    Debug.WriteLine("ALL, Addr: " + Parts[1]);
                }
                else
                {
                    if (BinPN == "")
                    {
                        //Search OS once
                        for (int s = 0; s < basefile.Segments.Count; s++)
                        {
                            if (!basefile.Segments[s].Missing)
                            {
                                string PN = basefile.ReadInfo(basefile.segmentAddressDatas[s].PNaddr);
                                if (Parts[0] == PN)
                                {
                                    isCompatible = true;
                                    BinPN = PN;
                                    break;
                                }
                            }
                        }
                    }
                    if (Parts[0] == BinPN)
                    {
                        isCompatible = true;
                        if (!HexToUint(Parts[1], out retVal))
                            throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.CompatibleOS + ")");
                        Debug.WriteLine("OS: " + BinPN + ", Addr: " + Parts[1]);
                    }
                }
            }
        }
        return retVal;
    }

    public static uint ApplyTablePatch(ref PcmFile basefile, XmlPatch xpatch, int tdId)
    {
        int diffCount = 0;
        frmTableEditor frmTE = new frmTableEditor();
        TableData pTd = basefile.tableDatas[tdId];
        frmTE.PrepareTable(basefile, pTd, null, "A");
        //frmTE.loadTable();
        uint addr = (uint)(pTd.addrInt + pTd.Offset + pTd.ExtraOffset );
        uint step = (uint)GetElementSize(pTd.DataType);
        try
        {
            string[] dataParts = xpatch.Data.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            for (int cell = 0; cell < frmTE.compareFiles[0].tableInfos[0].tableCells.Count; cell++)
            {
                TableCell tCell = frmTE.compareFiles[0].tableInfos[0].tableCells[cell];
                double val = Convert.ToDouble(dataParts[cell].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                if (tCell.SaveValue(val))
                    diffCount++;
            }
            //frmTE.saveTable(false);
            Array.Copy(frmTE.compareFiles[0].buf, 0, basefile.buf, frmTE.compareFiles[0].tableBufferOffset, frmTE.compareFiles[0].buf.Length);
            frmTE.Dispose();
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, patcherfunctions line " + line + ": " + ex.Message);

        }
        return (uint)(diffCount * step);
    }

    public static void ApplyTdPatch(TableData td, ref PcmFile PCM)
    {
        try
        {
            XmlPatch tmpPatch = new XmlPatch();
            if (td.CompatibleOS.Length > 0)
                tmpPatch.CompatibleOS = td.CompatibleOS;
            else if (td.OS.Length > 0)
            {
                /*
                string tmpData = td.Values.Substring(7); //Remove "Patch: "
                string[] tmpParts = tmpData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] tmpDParts = tmpParts[0].Split(':');
                tmpPatch.CompatibleOS = td.OS + ":" + tmpDParts[0];
                */
                tmpPatch.CompatibleOS = td.OS + ":0";
            }
            else
                tmpPatch.CompatibleOS = "ALL:0";
            tmpPatch.Description = td.TableDescription;
            tmpPatch.Name = td.TableName;
            tmpPatch.XmlFile = PCM.configFile;
            if (CheckPatchCompatibility(tmpPatch, PCM, true) == uint.MaxValue)
            {
                LoggerBold("Incompatible patch");
                return;
            }
            Logger("Applying patch: " + td.TableName, false);
            string data = td.Values.Substring(7); //Remove "Patch: "
            string[] parts = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int byteCount = 0;
            foreach(string part in parts)
            {
                string[] dParts = part.Split(':');
                if (dParts.Length != 2)
                    throw new Exception(" Data error: " + td.Values);
                uint addr;
                if (!HexToUint(dParts[0], out addr))
                    throw new Exception(" Data error: " + td.Values);

                for (int i = 0; i < dParts[1].Length; i += 2)
                {
                    string byteStr = dParts[1].Substring(i, 2);
                    byte b;
                    if (!HexToByte(byteStr, out b))
                        throw new Exception(" Data error: " + td.Values);
                    if (PCM.buf[addr] != b)
                        byteCount++;
                    PCM.buf[addr] = b;
                    addr++;
                }
            }
            Logger(" [OK]");
            Logger("Modified " + byteCount.ToString() + " bytes");
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold(" Error, applyTdPatch line " + line + ": " + ex.Message);
        }
    }

    public static void ApplyTdTablePatch(ref PcmFile PCM, TableData patchTd)
    {
        XmlPatch xpatch = new XmlPatch();
        xpatch.CompatibleOS = patchTd.CompatibleOS.TrimStart(',');
        xpatch.Data = patchTd.Values.Substring(11).Trim();
        xpatch.Description = patchTd.TableDescription;
        xpatch.Name = patchTd.TableName;
        xpatch.XmlFile = PCM.configFile;
        Logger("Applying patch...");

        uint ind = CheckPatchCompatibility(xpatch, PCM, false);
        if (ind < uint.MaxValue)
        {
            uint bytes = ApplyTablePatch(ref PCM, xpatch, (int)ind);
            Logger(Environment.NewLine +  "Modified: " + bytes.ToString() + " bytes");
        }
    }

    public static bool ApplyXMLPatch(ref PcmFile basefile)
    {
        try
        {
            string PrevSegment = "";
            uint ByteCount = 0;
            string[] Parts;
            string prevDescr = "";

            Logger("Applying patch:");
            foreach (XmlPatch xpatch in PatchList)
            {
                if (xpatch.Description != null && xpatch.Description != "" && xpatch.Description != prevDescr)
                    Logger(xpatch.Description);
                prevDescr = xpatch.Description;
                if (xpatch.Segment != null && xpatch.Segment.Length > 0 && PrevSegment != xpatch.Segment)
                {
                    PrevSegment = xpatch.Segment;
                    Logger("Segment: " + xpatch.Segment);
                }
                uint Addr = CheckPatchCompatibility(xpatch, basefile,false);
                if (Addr < uint.MaxValue)
                {
                    bool PatchRule = true; //If there is no rule, apply patch
                    if (xpatch.Rule != null && (xpatch.Rule.Contains(':') || xpatch.Rule.Contains('[')))
                    {
                        Parts = xpatch.Rule.Split(new char[] { ']', '[', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (Parts.Length == 3)
                        {
                            uint RuleAddr;
                            if (!HexToUint(Parts[0], out RuleAddr))
                                throw new Exception("Can't decode from HEX: " + Parts[0] + " (" + xpatch.Rule + ")");
                            ushort RuleMask;
                            if (!HexToUshort(Parts[1], out RuleMask))
                                throw new Exception("Can't decode from HEX: " + Parts[1] + " (" + xpatch.Rule + ")");
                            ushort RuleValue;
                            if (!HexToUshort(Parts[2].Replace("!", ""), out RuleValue))
                                throw new Exception("Can't decode from HEX: " + Parts[2] + " (" + xpatch.Rule + ")");

                            if (Parts[2].Contains("!"))
                            {
                                if ((basefile.buf[RuleAddr] & RuleMask) != RuleValue)
                                {
                                    PatchRule = true;
                                    Logger("Rule match, applying patch");
                                }
                                else
                                {
                                    PatchRule = false;
                                    Logger("Rule doesn't match, skipping patch");
                                }
                            }
                            else
                            {
                                if ((basefile.buf[RuleAddr] & RuleMask) == RuleValue)
                                {
                                    PatchRule = true;
                                    Logger("Rule match, applying patch");
                                }
                                else
                                {
                                    PatchRule = false;
                                    Logger("Rule doesn't match, skipping patch");
                                }
                            }

                        }
                    }
                    if (PatchRule)
                    {
                        if (xpatch.CompatibleOS.ToLower().StartsWith("table:"))
                        {
                            uint bCount = ApplyTablePatch(ref basefile, xpatch, (int)Addr);
                            Logger(", " + bCount.ToString() + " bytes");
                            ByteCount += bCount;
                        }
                        else
                        {
                            Debug.WriteLine(Addr.ToString("X") + ":" + xpatch.Data);
                            Parts = xpatch.Data.Split(' ');
                            foreach (string Part in Parts)
                            {
                                //Actually add patch data:
                                if (Part.Contains("[") || Part.Contains(":"))
                                {
                                    //Set bits / use Mask
                                    byte Origdata = basefile.buf[Addr];
                                    Debug.WriteLine("Set address: " + Addr.ToString("X") + ", old data: " + Origdata.ToString("X"));
                                    string[] dataparts;
                                    dataparts = Part.Split(new char[] { ']', '[', ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    byte Setdata = byte.Parse(dataparts[0], System.Globalization.NumberStyles.HexNumber);
                                    byte Mask = byte.Parse(dataparts[1].Replace("]", ""), System.Globalization.NumberStyles.HexNumber);

                                    // Set 1
                                    byte tmpMask = (byte)(Mask & Setdata);
                                    byte Newdata = (byte)(Origdata | tmpMask);

                                    // Set 0
                                    tmpMask = (byte)(Mask & ~Setdata);
                                    Newdata = (byte)(Newdata & ~tmpMask);

                                    Debug.WriteLine("New data: " + Newdata.ToString("X"));
                                    basefile.buf[Addr] = Newdata;
                                }
                                else
                                {
                                    //Set byte
                                    if (Part != "*") //Skip wildcards
                                        basefile.buf[Addr] = Byte.Parse(Part, System.Globalization.NumberStyles.HexNumber);
                                }
                                Addr++;
                                ByteCount++;
                            }
                        }
                    }
                    if (xpatch.PostMessage != null && xpatch.PostMessage.Length > 1)
                        LoggerBold(xpatch.PostMessage);
                }
                else
                {
                    LoggerBold("Patch is not compatible");
                    return false;
                }
            }
            Logger("Applied: " + ByteCount.ToString() + " Bytes");
            if (ByteCount > 0)
                Logger("You can save BIN file now");
        }
        catch (Exception ex)
        {
            Logger("Error: " + ex.Message);
            return false;
        }
        return true;
    }

    public static bool CompareTables(TableData td1, TableData td2, PcmFile pcm1, PcmFile pcm2)
    {
        bool match = true;

        if ((td1.Rows * td1.Columns) != (td2.Rows * td2.Columns))
            return false;
        List<double> tableValues = new List<double>();
        uint addr = (uint)(td1.addrInt + td1.Offset + td1.ExtraOffset);
        uint step = (uint)GetElementSize(td1.DataType);
        if (td1.RowMajor)
        {
            for (int r = 0; r < td1.Rows; r++)
            {
                for (int c = 0; c < td1.Columns; c++)
                {
                    double val = GetValue(pcm1.buf,addr,td1,0,pcm1);
                    tableValues.Add(val);
                    addr += step;
                }
            }
        }
        else
        {
            for (int c = 0; c < td1.Columns; c++)
            {
                for (int r = 0; r < td1.Rows; r++)
                {
                    double val = GetValue(pcm1.buf, addr, td1, 0, pcm1);
                    tableValues.Add(val);
                    addr += step;
                }
            }
        }

        addr = (uint)(td2.addrInt + td2.Offset + td2.ExtraOffset);
        step = (uint)GetElementSize(td2.DataType);
        int i = 0;
        if (td2.RowMajor)
        {
            for (int r = 0; r < td2.Rows; r++)
            {
                for (int c = 0; c < td2.Columns; c++)
                {
                    double val = GetValue(pcm2.buf, addr, td2, 0,pcm2);
                    if (val != tableValues[i])
                    {
                        match = false;
                        break;
                    }
                    addr += step;
                    i++;
                }
            }
        }
        else
        {
            for (int c = 0; c < td2.Columns; c++)
            {
                for (int r = 0; r < td2.Rows; r++)
                {
                    double val = GetValue(pcm2.buf, addr, td2, 0, pcm2);
                    if (val != tableValues[i])
                    {
                        match = false;
                        break;
                    }
                    addr += step;
                    i++;
                }
            }
        }
        return match;
    }

    public static void SaveOBD2Codes()
    {
        string OBD2CodeFile = Path.Combine(Application.StartupPath, "XML", "OBD2Codes.xml");
        Logger("Saving file " + OBD2CodeFile + "...", false);
        using (FileStream stream = new FileStream(OBD2CodeFile, FileMode.Create))
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<OBD2Code>));
            writer.Serialize(stream, OBD2Codes);
            stream.Close();
        }
        Logger(" [OK]");
    }

    public static void LoadOBD2Codes()
    {
        string OBD2CodeFile = Path.Combine(Application.StartupPath, "XML", "OBD2Codes.xml");
        if (File.Exists(OBD2CodeFile))
        {
            Debug.WriteLine("Loading OBD2Codes.xml");
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<OBD2Code>));
            System.IO.StreamReader file = new System.IO.StreamReader(OBD2CodeFile);
            OBD2Codes = (List<OBD2Code>)reader.Deserialize(file);
            file.Close();
        }
        else
        {
            OBD2Codes = new List<OBD2Code>();
        }
    }

    public static string AutoDetect(PcmFile PCM)
    {
        AutoDetect autod = new AutoDetect();
        return autod.DetectPlatform(PCM);
    }

    public static UInt64 CalculateChecksum(bool MSB, byte[] Data, AddressData CSAddress, List<Block> CSBlocks,List<Block> ExcludeBlocks, CSMethod Method, short Complement, ushort Bytes, Boolean SwapB, bool dbg=true, UInt64 sum = 0)
    {
        
        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool ChecksumInRange = false;

            if (Method == CSMethod.None)
                return UInt64.MaxValue;
            if (dbg)
                Debug.WriteLine("Calculating checksum, method: " + Method);
            uint BufSize = 0;
            List<Block> Blocks = new List<Block>();

            for (int p = 0; p < CSBlocks.Count; p++)
            {
                Block B = new Block();
                B.Start = CSBlocks[p].Start;
                B.End = CSBlocks[p].End;
                if (CSAddress.Address >= B.Start && CSAddress.Address <= B.End)
                {
                    //Checksum  is located in this block
                    ChecksumInRange = true;
                    if (CSAddress.Address == B.Start)    //At beginning of segment
                    {
                        //At beginning of segment
                        if (dbg)
                            Debug.WriteLine("Checksum is at start of block, skipping");
                        B.Start += CSAddress.Bytes;
                    }
                    else
                    {
                        //Located at middle of block, create new block C, ending before checksum
                        if (dbg)
                            Debug.WriteLine("Checksum is at middle of block, skipping");
                        Block C = new Block();
                        C.Start = B.Start;
                        C.End = CSAddress.Address - 1;
                        Blocks.Add(C);
                        BufSize += C.End - C.Start + 1;
                        B.Start = CSAddress.Address + CSAddress.Bytes; //Move block B to start after Checksum
                    }
                }
                foreach (Block ExcBlock in ExcludeBlocks)
                {
                    if (ExcBlock.Start >= B.Start && ExcBlock.End <= B.End)
                    {
                        if (dbg)
                        {
                            Debug.WriteLine("Excluding block: " + ExcBlock.Start.ToString("X") + " - " + ExcBlock.End.ToString("X"));
                        }
                        //Excluded block in this block
                        if (ExcBlock.Start == B.Start)    //At beginning of segment, move start of block
                        {
                            B.Start = ExcBlock.End + 1;
                        }
                        else
                        {
                            if (ExcBlock.End < B.End - 1)
                            {
                                //Located at middle of block, create new block C, ending before excluded block
                                Block C = new Block();
                                C.Start = B.Start;
                                C.End = ExcBlock.Start - 1;
                                Blocks.Add(C);
                                BufSize += C.End - C.Start + 1;
                                B.Start = ExcBlock.End + 1; //Move block B to start after excluded block
                            }
                            else
                            {
                                //Exclude block at end of block, move end of block backwards
                                B.End = ExcBlock.Start - 1;
                            }
                        }
                    }
                }
                Blocks.Add(B);
                BufSize += B.End - B.Start + 1;
            }

            byte[] tmp = new byte[BufSize];
            uint Offset = 0;
            foreach (Block B in Blocks)
            {
                //Copy blocks to tmp array for calculation
                if (dbg)
                    Debug.WriteLine("Block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
                uint BlockSize = B.End - B.Start + 1;
                Array.Copy(Data, B.Start, tmp, Offset, BlockSize);
                Offset += BlockSize;
            }

            switch (Method)
            {
                case CSMethod.Bytesum:
                    for (uint i = 0; i < tmp.Length; i++)
                    {
                        sum += tmp[i];
                    }
                    break;

                case CSMethod.Wordsum:
                    for (uint i = 0; i < tmp.Length - 1; i += 2)
                    {
                        sum += ReadUint16(tmp, i, MSB);
                    }
                    break;

                case CSMethod.Dwordsum:
                    for (uint i = 0; i < tmp.Length - 3; i += 4)
                    {
                        sum += ReadUint32(tmp, i, MSB);
                    }
                    break;

                case CSMethod.crc16:
                    Crc16 C16 = new Crc16();
                    sum = C16.ComputeChecksum(tmp);
                    break;

                case CSMethod.crc32:
                    Crc32 C32 = new Crc32();
                    sum = C32.ComputeChecksum(tmp);
                    break;
                case CSMethod.BoschInv:
                    for (uint i = 0; i < tmp.Length - 1; i += 2)
                    {
                        sum += ReadUint16(tmp, i, MSB);
                    }
                    Debug.WriteLine("word sum: " + sum.ToString("X"));
                    if (ChecksumInRange)
                    {
                        Debug.Write("0x1FFFE + " + sum.ToString("X") + " = ");
                        sum = 0x1FFFE + sum;
                        Debug.WriteLine(sum.ToString("X"));
                    }
                    if (MSB)
                    {
                        Debug.Write("(" +sum.ToString("X") + " << 32) + (0xFFFFFFFF - " + sum.ToString("X") + ") = ");
                        sum = (sum << 32) + (0xFFFFFFFF - sum);
                        Debug.WriteLine(sum.ToString("X"));
                    }
                    else
                    {
                        Debug.Write("(0xFFFFFFFF - " + sum.ToString("X") + ") << 32) + " + sum.ToString("X") +" = ");
                        sum = ((0xFFFFFFFF - sum) << 32) + sum;
                        Debug.WriteLine(sum.ToString("X"));
                    }
                    break;
                case CSMethod.Ngc3:
                    Ngc3Checksum ngc3 = new Ngc3Checksum();
                    sum = ngc3.CalculateChecksum(tmp, 0, (uint)tmp.Length);
                    break;
            }

            if (Complement == 1)
            {
                sum = ~sum;
            }
            if (Complement == 2)
            {
                sum = ~sum + 1;
            }

            switch (Bytes)
            {
                case 1:
                    sum = (sum & 0xFF);
                    break;
                case 2:
                    sum = (sum & 0xFFFF);
                    break;
                case 4:
                    sum = (sum & 0xFFFFFFFF);
                    break;
            }
            if (SwapB)
            {
                sum = SwapBytes(sum,Bytes);
            }
            if (dbg)
                Debug.WriteLine("Result: " + sum.ToString("X"));
            stopwatch.Stop();
            Debug.WriteLine("CalculateChecksum took " + stopwatch.Elapsed.Milliseconds + " ms");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Checksum calc: " + ex.Message);
            return UInt64.MaxValue;
        }
        return sum;
    }

    public static bool ParseAddress(string Line, PcmFile PCM, out List<Block> Blocks)
    {
        Debug.WriteLine("Segment address line: " + Line);
        Blocks = new List<Block>();

        if (Line == null || Line == "")
        {
            Block B = new Block();
            B.End = PCM.fsize;
            B.Start = 0;
            Blocks.Add(B);
            return true;
        }

        string[] Parts = Line.Split(',');
        int i = 0;

        foreach (string Part in Parts)
        {
            string[] StartEnd = Part.Split('-');
            Block B = new Block();
            int Offset = 0;

            if (StartEnd[0].Contains(">"))
            {
                string[] SO = StartEnd[0].Split('>');
                StartEnd[0] = SO[0];
                uint x;
                if (!HexToUint(SO[1], out x))
                    throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                Offset = (int)x;
            }
            if (StartEnd[0].Contains("<"))
            {
                string[] SO = StartEnd[0].Split('<');
                StartEnd[0] = SO[0];
                uint x;
                if (!HexToUint(SO[1], out x))
                    throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                Offset = ~(int)x;
            }

            bool useLength = false;
            if (StartEnd.Length > 1 && StartEnd[1].StartsWith("L"))
            {
                useLength = true;
                StartEnd[1] = StartEnd[1].Replace("L", "");
            }

            if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
            {
                throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")");
            }
            if (StartEnd[0].StartsWith("@"))
            {
                uint tmpStart = B.Start;
                B.Start = PCM.ReadUInt32(tmpStart);
                B.End = PCM.ReadUInt32(tmpStart + 4);
                tmpStart += 8;
            }
            else if (StartEnd.Length > 1)
            {
                if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                    throw new Exception("Can't decode from HEX: " + StartEnd[1].Replace("@", "") + " (" + Line + ")");
                if (B.End >= PCM.buf.Length)    //Make 1MB config work with 512kB bin
                    B.End = (uint)PCM.buf.Length - 1;
            }
            else if (StartEnd.Length == 1)
            {
                B.End = B.Start;
            }
            else
            {
                if (StartEnd[1].StartsWith("@"))
                {
                    //Read End address from bin at this address
                    B.End = PCM.ReadUInt32(B.End);
                }
                if (StartEnd[1].EndsWith("@"))
                {
                    //Address is relative to end of bin
                    uint end;
                    if (HexToUint(StartEnd[1].Replace("@", ""), out end))
                        B.End = (uint)PCM.buf.Length - end - 1;
                }
            }
            if (useLength)
            {
                B.End = B.Start - 1 + B.End;
            }

            B.Start += (uint)Offset;
            B.End += (uint)Offset;
            Blocks.Add(B);
            i++;
        }
        foreach (Block B in Blocks)
            Debug.WriteLine("Address block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
        return true;
    }

    public static string CheckStockCVN(string PN, string Ver, string SegNr, UInt64 cvnInt, bool AddToList, string XMLFile)
    {
        string retVal = "[n/a]";

        string qry = "";
        if (SegNr.Length > 0)
            qry = "pn = '" + PN + "' AND ver = '" + Ver + "' AND segmentnr = " + SegNr.ToString(); //+ " AND xmlfile = '" + XMLFile + "'";
        else
            qry = "pn = '" + PN + "' AND ver = '" + Ver + "'"; //+ " AND xmlfile = '" + XMLFile + "'";
        DataRow[] res = cvnDB.stockCvn.Select(qry);

        for (int c = 0; c < res.Length; c++)
        {
            if (Path.GetFileName(XMLFile) != res[c]["xmlfile"].ToString() && res[c]["alternatexml"].ToString().Length == 0)
            {
                OleDbCommandBuilder builder = new OleDbCommandBuilder(cvnDB.stockAdapter);
                res[c]["alternatexml"] = Path.GetFileName(XMLFile);
                cvnDB.stockAdapter.UpdateCommand = builder.GetUpdateCommand();
                cvnDB.stockAdapter.Update(cvnDB.stockCvn);
            }
            uint stockCvnInt = 0;
            if(HexToUint(res[c]["cvn"].ToString(), out stockCvnInt))
            if (stockCvnInt == cvnInt)
            {
                retVal = "[stock]";
                break;
            }
            else
            {
                retVal = "[modded]";
                break;
                //return "[modded]";
            }
        }


        if (retVal == "[n/a]")
        {
            //Check if it's in referencelist
            bool cvnMismatch = false;
            uint refC = 0;
            string refString = "";
            qry = "pn = '" + PN + "'";
            res = cvnDB.refCvn.Select(qry);

            for (int r = 0; r < res.Length; r++)
            {
                if (AppSettings.RequireValidVerForStock)
                    if (Ver.Contains("?"))
                    {
                        Logger("No valid version");
                        return "[modded/R]";
                    }
                refString = res[r]["cvn"].ToString();
                cvnMismatch = true;    //Found from referencelist, match not found YET
                if (!HexToUint(refString, out refC))
                {
                    LoggerBold("Can't convert from HEX: " + refString);
                }
                if (refC == cvnInt)
                {
                    Debug.WriteLine("PN: " + PN + " CVN found from Referencelist: " + refString);
                    cvnMismatch = false; //Found from referencelist, no mismatch
					retVal = "[stock]";

                }
                ushort refShort;
                if (!HexToUshort(refString, out refShort))
                {
                    Debug.WriteLine("CheckStockCVN (ushort): Can't convert from HEX: " + refString);
                }
                if (SwapBytes(refShort,4) == cvnInt)
                {
                    Debug.WriteLine("PN: " + PN + " byteswapped CVN found from Referencelist: " + refString);
                    cvnMismatch = false; //Found from referencelist, no mismatch
					retVal = "[stock]";
                }
                else
                {
                    Debug.WriteLine("Byte swapped CVN doesn't match: " + SwapBytes(refShort,4).ToString("X") + " <> " + cvnInt.ToString("X"));
                }                
            }

            if (cvnMismatch) //Found from referencelist, have mismatch
            {
                retVal = "[modded/R]";
                bool isInBadCvnList = false;
                AddToList = false;  //Don't add to CVN list if add to mismatch CVN
                if (BadCvnList == null)
                    BadCvnList = new List<CVN>();
                for (int i = 0; i < BadCvnList.Count; i++)
                {
                    uint badCvnInt = 0;
                    if (HexToUint(BadCvnList[i].cvn, out badCvnInt))
                    {
                        if (BadCvnList[i].PN == PN && badCvnInt == cvnInt)
                        {
                            isInBadCvnList = true;
                            Debug.WriteLine("PN: " + PN + ", CVN: " + cvnInt + " is already in badCvnList");
                            break;
                        }
                    }
                }
                if (!isInBadCvnList)
                {
                    Debug.WriteLine("Adding PN: " + PN + ", CVN: " + cvnInt + " to badCvnList");
                    CVN C1 = new CVN();
                    C1.cvn = cvnInt.ToString("X");
                    C1.PN = PN;
                    C1.SegmentNr = SegNr;
                    C1.Ver = Ver;
                    C1.XmlFile = Path.GetFileName(XMLFile);
                    C1.ReferenceCvn = refString.TrimStart('0');
                    BadCvnList.Add(C1);

                }
            }
        }

        if (AddToList && retVal != "[stock]")
        {
            bool IsinCVNlist = false;
            if (ListCVN == null)
                ListCVN = new List<CVN>();

            for (int c = 0; c < ListCVN.Count; c++)
            {
                //if (ListCVN[c].XmlFile == Path.GetFileName(XMLFile) && ListCVN[c].PN == PN && ListCVN[c].Ver == Ver && ListCVN[c].SegmentNr == SegNr && ListCVN[c].cvn == cvn)
                uint listCvnInt = 0;
                if (HexToUint(ListCVN[c].cvn, out listCvnInt))
                    if (ListCVN[c].PN == PN && ListCVN[c].Ver == Ver && ListCVN[c].SegmentNr == SegNr && listCvnInt == cvnInt)
                    {
                        Debug.WriteLine("Already in CVN list: " + cvnInt);
                        IsinCVNlist = true;
                        break;
                    }
            }
            if (!IsinCVNlist)
            {
                CVN newCvn = new CVN();

                newCvn.PN = PN;
                newCvn.cvn = cvnInt.ToString("X");
                newCvn.SegmentNr = SegNr;
                newCvn.Ver = Ver;
                newCvn.XmlFile = Path.GetFileName(XMLFile);
                qry = "pn = '" + PN + "'";
                res = cvnDB.refCvn.Select(qry);

                if (res.Length > 0)
                    newCvn.ReferenceCvn = res[0]["cvn"].ToString().TrimStart('0');

                ListCVN.Add(newCvn);
            }
        }
        return retVal;
   }

    public static uint SearchBytes(PcmFile PCM, string searchString, uint Start, uint End, ushort stopVal = 0)
    {
        uint addr;
        try
        {
            string[] searchParts = searchString.Trim().Split(' ');
            byte[] bytes = new byte[searchParts.Length];

            for (int b = 0; b < searchParts.Length; b++)
            {
                byte searchval = 0;
                if (searchParts[b] != "*")
                    HexToByte(searchParts[b], out searchval);
                bytes[b] = searchval;
            }

            for (addr = Start; addr < End; addr++)
            {
                bool match = true;
                if (stopVal != 0 && PCM.ReadUInt16(addr) == stopVal)
                {
                    return uint.MaxValue;
                }
                if ((addr + searchParts.Length) > PCM.fsize)
                    return uint.MaxValue;
                for (uint part = 0; part < searchParts.Length; part++)
                {
                    if (searchParts[part] != "*")
                    {
                        if (PCM.buf[addr + part] != bytes[part])
                        {
                            match = false;
                            break;
                        }
                    }
                }
                if (match)
                {
                    return addr;
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
            Debug.WriteLine("Error searchBytes, line " + line + ": " + ex.Message);
        }
        return uint.MaxValue;
    }

    public static SearchedAddress GetAddrbySearchString(PcmFile PCM, string searchStr, ref uint startAddr, uint endAddr, bool conditionalOffset = false, bool signedOffset = false)
    {
        SearchedAddress retVal;
        retVal.Addr = uint.MaxValue;
        retVal.Columns = 0;
        retVal.Rows = 0;
        try
        {
            string modStr = searchStr.Replace("r", "");
            modStr = modStr.Replace("k", "");
            modStr = modStr.Replace("x", "");
            modStr = modStr.Replace("y", "");
            modStr = modStr.Replace("@", "*");
            modStr = modStr.Replace("# ", "* "); //# alone at beginning or middle
            if (modStr.EndsWith("#"))
                modStr = modStr.Replace(" #", " *"); //# alone at end
            modStr = modStr.Replace("#", ""); //For example: #21 00 21
            uint addr = SearchBytes(PCM, modStr, startAddr, endAddr);
            if (addr == uint.MaxValue)
            {
                //Not found
                startAddr = uint.MaxValue;
                return retVal;
            }

            string[] sParts = searchStr.Trim().Split(' ');
            startAddr = addr + (uint)sParts.Length;

            int[] locations = new int[4];
            int l = 0;
            string addrStr = "*";
            if (searchStr.Contains("@")) addrStr = "@";
            else if (searchStr.Contains("*") || searchStr.Contains("#")) addrStr = "*";
            else
            {
                //Address is AFTER searchstring
                retVal.Addr = PCM.ReadUInt32(addr + (uint)sParts.Length);
            }
            for (int p = 0; p < sParts.Length; p++)
            {
                if (sParts[p].Contains(addrStr) && l < 4)
                {
                    locations[l] = p;
                    l++;
                }
                if (sParts[p].Contains("r") || sParts[p].Contains("x"))
                {
                    retVal.Rows = (ushort)PCM.buf[(uint)(addr + p)];
                }
                if (sParts[p].Contains("k") || sParts[p].Contains("y"))
                {
                    retVal.Columns = (ushort) PCM.buf[(uint)(addr + p)];
                }
                if (sParts[p].Contains("#"))
                {
                    retVal.Addr = (uint)(addr + p);
                }

            }
            if (retVal.Addr < uint.MaxValue)
            {
                return retVal;
            }

            //We are here, so we must have @ @  or @ @ @ @  in searchsting
            if (l == 2)
            {
                if (PCM.platformConfig.MSB)
                    retVal.Addr = (uint)(PCM.buf[addr + locations[0]] << 8 | PCM.buf[addr + locations[1]]);
                else
                    retVal.Addr = (uint)(PCM.buf[addr + locations[1]] << 8 | PCM.buf[addr + locations[0]]);

            }
            else if (l == 4)
            {
                if (PCM.platformConfig.MSB)
                    retVal.Addr = (uint)(PCM.buf[addr + locations[0]] << 24 | PCM.buf[addr + locations[1]] << 16 | PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                else
                    retVal.Addr = (uint)(PCM.buf[addr + locations[3]] << 24 | PCM.buf[addr + locations[2]] << 16 | PCM.buf[addr + locations[1]] << 8 | PCM.buf[addr + locations[0]]);

            }
            else
            {
                Logger("!= 2 or 4 @ in searchstring, address need 4 bytes! (" + searchStr + ")");
                retVal.Addr = uint.MaxValue;
            }

            if (conditionalOffset)
            {
                ushort addrWord = (ushort)(PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                if (addrWord > 0x5000)
                    retVal.Addr -= 0x10000;
            }
            if (signedOffset)
            {
                ushort addrWord = (ushort)(PCM.buf[addr + locations[2]] << 8 | PCM.buf[addr + locations[3]]);
                if (addrWord > 0x8000)
                    retVal.Addr -= 0x10000;
            }

        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.WriteLine ("getAddrbySearchString, line " + line + ": " + ex.Message);
        }
        return retVal;
    }


    public static int FindTableDataId(TableData refTd, List<TableData> tdList)
    {
        int pos1 = refTd.TableName.IndexOf("*");
        if (pos1 < 0)
            pos1 = refTd.TableName.Length;

        string refTableName = refTd.TableName.ToLower().Substring(0, pos1).Replace(" ", "_");
        for (int t = 0; t < tdList.Count; t++)
        {
            int pos2 = tdList[t].TableName.IndexOf("*");
            if (pos2 < 0)
                pos2 = tdList[t].TableName.Length;
            //if (pcm1.tableDatas[t].TableName.ToLower().Substring(0, pos2) == refTd.TableName.ToLower().Substring(0, pos1) && pcm1.tableDatas[t].Category.ToLower() == refTd.Category.ToLower())
            if (tdList[t].TableName.ToLower().Substring(0, pos2).Replace(" ","_") == refTableName)
            {
                return t;
            }
        }
        //Not found (exact match) maybe close enough?
        int required = AppSettings.TunerMinTableEquivalency;
        if (required == 100)
            return -1;  //already searched for 100% match
        for (int t = 0; t < tdList.Count; t++)
        {
            int pos2 = tdList[t].TableName.IndexOf("*");
            if (pos2 < 0)
                pos2 = tdList[t].TableName.Length;
            double percentage = ComputeSimilarity.CalculateSimilarity(tdList[t].TableName.ToLower().Substring(0, pos2).Replace(" ", "_"), refTableName);
            if ((int)(percentage * 100) >= required )
            {
                Debug.WriteLine(refTd.TableName + " <=> " + tdList[t].TableName + "; Equivalency: " + (percentage * 100).ToString() + "%");
                return t;
            }
        }

        return -1;
    }

    public static TableData FindTableData(TableData refTd, List<TableData> tdList)
    {
        int pos1 = refTd.TableName.IndexOf("*");
        if (pos1 < 0)
            pos1 = refTd.TableName.Length;

        string refTableName = refTd.TableName.ToLower().Substring(0, pos1).Replace(" ", "_");
        for (int t = 0; t < tdList.Count; t++)
        {
            int pos2 = tdList[t].TableName.IndexOf("*");
            if (pos2 < 0)
                pos2 = tdList[t].TableName.Length;
            //if (pcm1.tableDatas[t].TableName.ToLower().Substring(0, pos2) == refTd.TableName.ToLower().Substring(0, pos1) && pcm1.tableDatas[t].Category.ToLower() == refTd.Category.ToLower())
            if (tdList[t].TableName.ToLower().Substring(0, pos2).Replace(" ", "_") == refTableName)
            {
                return tdList[t];
            }
        }
        //Not found (exact match) maybe close enough?
        int required = AppSettings.TunerMinTableEquivalency;
        if (required == 100)
            return null;  //already searched for 100% match
        for (int t = 0; t < tdList.Count; t++)
        {
            int pos2 = tdList[t].TableName.IndexOf("*");
            if (pos2 < 0)
                pos2 = tdList[t].TableName.Length;
            double percentage = ComputeSimilarity.CalculateSimilarity(tdList[t].TableName.ToLower().Substring(0, pos2).Replace(" ", "_"), refTableName);
            if ((int)(percentage * 100) >= required)
            {
                Debug.WriteLine(refTd.TableName + " <=> " + tdList[t].TableName + "; Equivalency: " + (percentage * 100).ToString() + "%");
                return tdList[t];
            }
        }

        return null;
    }

/*    public static void Logger(string LogText, Boolean NewLine = true)
    {
        try
        {
            Debug.WriteLine(LogText);
            for (int l = LogReceivers.Count - 1; l >= 0;  l--)
            {
                if (LogReceivers[l].IsDisposed)
                    LogReceivers.RemoveAt(l);
            }
            for (int l=0; l< LogReceivers.Count; l++)
            {
                RichTextBox rtb = LogReceivers[l];
                rtb.Parent.Invoke((MethodInvoker)delegate ()
                {
                    rtb.AppendText(LogText);
                    if (NewLine)
                        rtb.AppendText(Environment.NewLine);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
    }

    public static void LoggerBold(string LogText, Boolean NewLine = true)
    {
        try
        {
            Debug.WriteLine(LogText);
            for (int l = LogReceivers.Count - 1; l >= 0; l--)
            {
                if (LogReceivers[l].IsDisposed)
                    LogReceivers.RemoveAt(l);
            }
            for (int l = 0; l < LogReceivers.Count; l++)
            {
                RichTextBox rtb = LogReceivers[l];
                rtb.Parent.Invoke((MethodInvoker)delegate ()
                {
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                    rtb.AppendText(LogText);
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Regular);
                    if (NewLine)
                        rtb.AppendText(Environment.NewLine);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
    }
*/
    public static List<XmlPatch> LoadPatchFile(string fileName)
    {
        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
        System.IO.StreamReader file = new System.IO.StreamReader(fileName);
        List<XmlPatch> pList = (List<XmlPatch>)reader.Deserialize(file);
        file.Close();
        return pList;
    }

    public static List<TableData> FilterTdList(IEnumerable<TableData> results, string filterTxt, string filterBy, bool caseSens)
    {
        TableData tdTmp = new TableData();
        try
        {
            if (caseSens)
                results = results.Where(t => typeof(TableData).GetProperty(filterBy).GetValue(t, null) != null).Where(t => typeof(TableData).GetProperty(filterBy).GetValue(t, null).ToString().Contains(filterTxt.Trim()));
            else
                results = results.Where(t => typeof(TableData).GetProperty(filterBy).GetValue(t, null) != null).Where(t => typeof(TableData).GetProperty(filterBy).GetValue(t, null).ToString().ToLower().Contains(filterTxt.ToLower().Trim()));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            List<TableData> emptyList = new List<TableData>();
            return emptyList;
        }
        return results.ToList();
    }

    public static void LoadPidDescriptions()
    {
        try
        {
            string FileName = Path.Combine(Application.StartupPath, "XML", "PidDescriptions.xml");
            if (!File.Exists(FileName))
                FileName = Path.Combine(Application.StartupPath, "XML", "pidlist.xml");
            if (!File.Exists(FileName))
                return;
            PidDescriptions = new List<PidInfo>();
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidInfo>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            PidDescriptions = (List<PidInfo>)reader.Deserialize(file);
            file.Close();
        }
        catch (Exception ex)
        {
            var st = new StackTrace(ex, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LoggerBold("Error, Patcherfunctions line " + line + ": " + ex.Message);
            Debug.WriteLine("Error, Patcherfunctions line " + line + ": " + ex.Message);
        }
    }

    private class LoggerClass
    {
        public void RunLoggerForm()
        {
            try
            {
                frmLogger fl = new frmLogger();
                Application.Run(fl);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Patcherfunctions line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, Patcherfunctions line " + line + ": " + ex.Message);
            }

        }
    }

    public static void StartLogger(PcmFile PCM)
    {
        try
        {
            if (AppSettings.LoggerUseIntegrated)
            {
                LoggerClass lc = new LoggerClass();
                Thread thread = new Thread(new ThreadStart(lc.RunLoggerForm));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Name = "Datalogger";
                thread.Start();
            }
            else if (string.IsNullOrEmpty(AppSettings.LoggerExternalApp))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = AppSettings.LoggerExternalApp;
                Logger("Executing command: \"" + psi.FileName);
                Process.Start(psi);
            }
            else
            {
                LoggerBold("Integrated logger disabled, external logger not defined");
            }
        }
        catch (Exception ex)
        {
            LoggerBold(ex.Message);
        }
    }

    public static void StartFlashApp(PcmFile PCM, bool WriteCalibration)
    {
        try
        {
            if (AppSettings.FlashApp.Length == 0)
            {
                Logger("No flash application configured");
                return;
            }
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = AppSettings.FlashApp;
            if (WriteCalibration)
            {
                if (PCM == null || !File.Exists(PCM.FileName))
                {
                    LoggerBold("No file loaded");
                    return;
                }
                psi.Arguments = AppSettings.FLashParams.Replace("$file", "\"" + PCM.FileName + "\"");
                if (PCM.BufModified())
                {
                    DialogResult res = MessageBox.Show("File modified.\n\rSave before flashing?", "Save changes?", MessageBoxButtons.YesNoCancel);
                    if (res == DialogResult.Cancel)
                        return;
                    if (res == DialogResult.Yes)
                    {
                        Logger("Saving to file: " + PCM.FileName);
                        PCM.SaveBin(PCM.FileName);
                        Logger("File saved");
                    }
                }
            }
            if (psi.FileName.ToLower().Contains("pcmhammer"))
            {
                Logger("If you need help with PCM Hammer or want to thank the developers, see:");
                Logger("https://github.com/LegacyNsfw/PcmHacks/wiki");

            }
            Logger("Executing command: \"" + psi.FileName + " " + psi.Arguments + "\"");
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            LoggerBold(ex.Message);
        }
    }

 
    public static void SetDtcCode(ref byte[] buffer, uint bufferOffset, int codeIndex, DtcCode dtc, PcmFile PCM)
    {
        if (dtc.P10)
        {
            if (dtc.Status == 0) //Inverted
            {
                buffer[dtc.statusAddrInt - bufferOffset] = (byte)(buffer[dtc.statusAddrInt - bufferOffset] | dtc.StatusMask);
                dtc.StatusTxt = "Enabled";
            }
            else
            {
                buffer[dtc.statusAddrInt - bufferOffset] = (byte)(buffer[dtc.statusAddrInt - bufferOffset] & ~dtc.StatusMask);
                dtc.StatusTxt = "Disabled";
            }

            if (dtc.MilStatus > 0)
            {
                buffer[dtc.milAddrInt - bufferOffset] = (byte)(buffer[dtc.milAddrInt - bufferOffset] | dtc.StatusMask);
                buffer[dtc.milAddrInt2 - bufferOffset] = (byte)(buffer[dtc.milAddrInt2 - bufferOffset] | dtc.StatusMask);
            }
            else
            {
                buffer[dtc.milAddrInt - bufferOffset] = (byte)(buffer[dtc.milAddrInt - bufferOffset] & ~dtc.StatusMask);
                buffer[dtc.milAddrInt2 - bufferOffset] = (byte)(buffer[dtc.milAddrInt2 - bufferOffset] & ~dtc.StatusMask);
            }


            if (dtc.TypeTxt == "TypeB")
                buffer[dtc.TypeAddrInt - bufferOffset] = (byte)(buffer[dtc.TypeAddrInt - bufferOffset] | dtc.StatusMask);
            else
                buffer[dtc.TypeAddrInt - bufferOffset] = (byte)(buffer[dtc.TypeAddrInt - bufferOffset] & ~dtc.StatusMask);
        }
        else
        {
            buffer[dtc.statusAddrInt - bufferOffset] = dtc.Status;
            if (PCM.dtcCombined)
            {
                PCM.dtcCodes[codeIndex].StatusTxt = PCM.dtcValues[dtc.Status].ToString();

                if (dtc.Status > 3)
                    dtc.MilStatus = 1;
                else
                    dtc.MilStatus = 0;
            }
            else
            {
                PCM.dtcCodes[codeIndex].StatusTxt = PCM.dtcValues[dtc.Status].ToString();
                buffer[dtc.milAddrInt - bufferOffset] = dtc.MilStatus;
            }
        }
    }

    public static byte[] SwapFileBytes(byte[] buf, int treatAs)
    {
        byte[] retVal = new byte[buf.Length];
        for (int a = 0; (a + treatAs) <= buf.Length; a += treatAs)
        {
            int s = a + treatAs;
            for (int x=0;x<treatAs;x++)
            {
                retVal[a + x] = buf[s - x - 1];
            }
        }
        return retVal;
    }

    // Function to perform smoothing operation on a 2D table
    public static double[,] Smooth2DTable(double[,] table)
    {
        int rows = table.GetLength(0);
        int cols = table.GetLength(1);

        double[,] smoothedTable = new double[rows, cols];

        // Perform averaging operation
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                double sum = 0;
                int count = 0;

                for (int di = -1; di <= 1; di++)
                {
                    for (int dj = -1; dj <= 1; dj++)
                    {
                        int ni = i + di;
                        int nj = j + dj;

                        if (ni >= 0 && ni < rows && nj >= 0 && nj < cols)
                        {
                            sum += table[ni, nj];
                            count++;
                        }
                    }
                }

                smoothedTable[i, j] = sum / count;
            }
        }

        return smoothedTable;
    }

}