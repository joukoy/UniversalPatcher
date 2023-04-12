using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace UniversalPatcher
{
    public class UpatcherSettings
    {
        public UpatcherSettings()
        {

            SuppressAfter = 10;
            AutorefreshCVNlist = true;
            AutorefreshFileinfo = true;
            MainWindowPersistence = true;
            TableEditorAutoResize = true;
            TunerModeColumns = "TableName,Category,Units,Columns,Rows,TableDescription";
            TunerMinTableEquivalency = 100;
            TableEditorMinTableEquivalency = 100;
            keyPressWait100ms = 2;
            TableEditorMinOtherEquivalency = 100;
            MassCopyTableFilenameExtra = "-new";
            TableExplorerIconSize = 16;
            TunerTreeMode = true;
            TunerAutomulti1d = true;
            MulitableChars = ". [ ]";
            WorkingMode = 1;
            RequireValidVerForStock = true;
            TunerShowUnitsUndefined = true;
            TunerShowUnitsImperial = true;
            TunerShowUnitsMetric = true;
            SplashShowTime = 4;
            FlashApp = "PCMHammer\\pcmhammer.exe";
            FLashParams = "--writecalibration $file";
            LoggerUseIntegrated = true;
            LoggerResponseMode = "SendOnce";
            LoggerBaudRate = 115200;
            LoggerScriptDelay = 10;
            JConsole4x = true;
            LoggerConsole4x = true;
            LoggerGraphicsInterval = 1;
            LoggerGraphicsShowMaxTime = 10;
            LoggerTimestampFormat = "HH:mm:ss.ffff";
            LoggerLogSeparator = ",";
            LoggerDecimalSeparator = ".";
            TunerShowTableCount = true;
            TunerModeColumns = "TableName,Category,Units,Columns,Rows,TableDescription";
            ConfigModeColumnWidth = "180,114,32,63,86,71,48,81,100,50,78,49,69,60,54,43,58,64,43,78,100,100,243,100,100";
            TunerModeColumnWidth = "192,110,100,100,100,100,100,100,100,72,100,100,100,100,60,46,100,100,100,100,100,100,197,100,100";
            NavigatorMaxTablesPerNode = 3;
            NavigatorMaxTablesTotal = 50;
            LoggerStartJ2534Process = true;
            LoggerJ2534ProcessVisible = false;
            LoggerAutoDisconnect = true;
            LoggerFilterStartOfMessage = true;
            LoggerFilterTXIndication = true;
            LoggerFilterRxStatusCustom = "";
            LoggerUseVPW = true;
            LoggerConsoleDisplayInterval = 200;
            RetryWriteTimes = 3;
            RetryWriteDelay = 10;
            LoggerResetAfterMiss = 50;
            ClearFuncAddrOnDisconnect = true;

            TimeoutLoggingActive = TimeoutScenario.DataLogging3;
            TimeoutLoggingActiveObdlink = TimeoutScenario.Minimum;
            TimeoutLoggingPassive = TimeoutScenario.DataLogging4;
            TimeoutReceive = 100;
            TimeoutLoggingReceive = 100;
            TimeoutScriptRead = 100;
            TimeoutScriptWrite = 100;
            TimeoutPortRead = 1000;
            TimeoutAnalyzerReceive = 3000;
            TimeoutConsoleReceive = 100;
            TimeoutLoggingWrite = 100;
            //TimeoutJconsoleWrite = 100;
            //TimeoutJ2534DeviceRead = 20;

            TableEditorFont = new SerializableFont(new Font("Consolas", 8));
            TableExplorerFont = new SerializableFont(new Font("Arial", 8));
            PatcherLogFont = new SerializableFont(new Font("Consolas", 8));
            DebugFont = new SerializableFont(new Font("Consolas", 8));
            LoggerConsoleFont = new SerializableFont(new Font("Consolas", 8));
        }
        public string LastXMLfolder { get; set; }
        public string LastPATCHfolder { get; set; }
        public string LastBINfolder { get; set; }
        public uint SuppressAfter { get; set; }
        public bool DebugOn { get; set; }
        public bool AutorefreshCVNlist { get; set; }
        public bool AutorefreshFileinfo { get; set; }
        public bool MainWindowPersistence { get; set; }
        public Size MainWindowSize { get; set; }
        public FormWindowState MainWindowState { get; set; }
        public Point MainWindowLocation { get; set; }
        public Size EditXMLWindowSize { get; set; }
        public FormWindowState EditXMLWindowState { get; set; }
        public Point EditXMLWindowLocation { get; set; }
        public Size FileSelectionWindowSize { get; set; }
        public FormWindowState FileSelectionWindowState { get; set; }
        public Point FileSelectionWindowLocation { get; set; }
        public Size TableEditorWindowSize { get; set; }
        public FormWindowState TableEditorWindowState { get; set; }
        public Point TableEditorWindowLocation { get; set; }
        public bool TableEditorAutoResize { get; set; }
        public Size TunerWindowSize { get; set; }
        public FormWindowState TunerWindowState { get; set; }
        public Point TunerWindowLocation { get; set; }
        public Size frmGraphicsWindowSize { get; set; }
        public FormWindowState frmGraphicsWindowState { get; set; }
        public Point frmGraphicsWindowLocation { get; set; }
        public bool disableTunerAutoloadSettings { get; set; }
        public string TunerModeColumns { get; set; }
        public string ConfigModeColumnOrder { get; set; }
        public string ConfigModeColumnWidth { get; set; }
        public string TunerModeColumnWidth { get; set; }
        public Size TunerLogWindowSize { get; set; }
        public int PatcherSplitterDistance { get; set; }
        //public Font TableEditorFont { get; set; }
        public Size frmTdWindowSize { get; set; }
        public FormWindowState frmTdWindowState { get; set; }
        public Point frmTdWindowLocaction { get; set; }
        public Size MassCompareWindowSize { get; set; }
        public FormWindowState MassCompareWindowState { get; set; }
        public Point MassCompareWindowLocation { get; set; }
        public int MassCompareSplitWidth { get; set; }
        public Size MassModifyTableDataWindowSize { get; set; }
        public FormWindowState MassModifyTableDataWindowState { get; set; }
        public Point MassModifyTableDataWindowLocation { get; set; }
        public int MassModifyTableDataWindowSplitterDistance { get; set; }
        public int TunerMinTableEquivalency { get; set; }
        public int TableEditorMinTableEquivalency { get; set; }
        public int keyPressWait100ms { get; set; }
        public int TableEditorMinOtherEquivalency { get; set; }
        public Point MassCopyTableWindowLocation { get; set; }
        public FormWindowState MassCopyTableWindowState { get; set; }
        public Size MassCopyTableWindowSize { get; set; }
        public int MassCopyTableSplitHeight { get; set; }
        public string MassCopyTableFilenameExtra { get; set; }
        //public Font TableExplorerFont { get; set; }
        public int TableExplorerIconSize { get; set; }
        public Size TableExplorerWindowSize { get; set; }
        public FormWindowState TableExplorerWindowState { get; set; }
        public Point TableExplorerWindowPosition { get; set; }
        public bool TableExplorerUseCategorySubfolder { get; set; }
        public Point TunerExplorerWindowLocation { get; set; }
        public FormWindowState TunerExplorerWindowState { get; set; }
        public Size TunerExplorerWindowSize { get; set; }
        public int TunerExplorerWindowSplitterDistance { get; set; }
        public int TunerExplorerWindowSplitter2Distance { get; set; }
        public bool TunerTreeMode { get; set; }
        public bool TunerAutomulti1d { get; set; }
        public int TunerListModeTreeWidth { get; set; }
        public string MulitableChars { get; set; }
        public bool DisableAutoFixChecksum { get; set; }
        public int WorkingMode { get; set; }
        public bool CvnPopupAccepted { get; set; }
        public bool RequireValidVerForStock { get; set; }
        public bool AutomaticOpenImportedFile { get; set; }
        public bool TunerShowUnitsUndefined { get; set; }
        public bool TunerShowUnitsImperial { get; set; }
        public bool TunerShowUnitsMetric { get; set; }
        public int SplashShowTime { get; set; }
        //public Font PatcherLogFont { get; set; }
        //public Font DebugFont { get; set; }
        public bool xdfImportUseTableName { get; set; }
        public string FlashApp { get; set; }
        public string FLashParams { get; set; }
        public bool LoggerUseIntegrated { get; set; }
        public string LoggerExternalApp { get; set; }
        public string LoggerComPort { get; set; }
        public string LoggerLastProfile { get; set; }
        public string LoggerLogFolder { get; set; }
        public string LoggerJ2534Device { get; set; }
        public bool LoggerPassive { get; set; }
        public bool LoggerUseJ2534 { get; set; }
        public string LoggerSerialDeviceType { get; set; }
        public string LoggerResponseMode { get; set; }
        public bool LoggerUseFTDI { get; set; }
        public bool LoggerShowAdvanced { get; set; }
        public string LoggerFTDIPort { get; set; }
        public int LoggerBaudRate { get; set; }
        public bool LoggerUsePriority { get; set; }
        public bool LoggerUseFilters { get; set; }
        public bool LoggerUseVPW { get; set; }
        public Size LoggerWindowSize { get; set; }
        public FormWindowState LoggerWindowState { get; set; }
        public Point LoggerWindowPosition { get; set; }
        public bool LoggerEnableConsole { get; set; }
        public bool LoggerConsoleTimestamps { get; set; }
        //public Font LoggerConsoleFont { get; set; }
        public bool LoggerConsoleJ2534Timestamps { get; set; }
        public int LoggerScriptDelay { get; set; }
        public string LoggerJ2534SettingsFile { get; set; }
        public string JConsoleDevice { get; set; }
        public bool JConsoleTimestamps { get; set; }
        public bool JConsole4x { get; set; }
        public bool LoggerConsole4x { get; set; }
        public string LoggerJ2534SettingsFile2 { get; set; }
        public int PasteSpecialMode { get; set; }
        public string PasteSpecialPositiveFormula { get; set; }
        public string PasteSpecialNegativeFormula { get; set; }
        public string PasteSpecialTarget { get; set; }
        public string LoggerGraphicsLiveLastProfileFile { get; set; }
        public string LoggerGraphicsLogLastProfileFile { get; set; }
        public int LoggerGraphicsInterval { get; set; }
        public int LoggerGraphicsShowMaxTime { get; set; }
        public bool LoggerGraphicsShowPoints { get; set; }
        public string LoggerLastLogfile { get; set; }
        public string LoggerTimestampFormat { get; set; }
        public string LoggerLogSeparator { get; set; }
        public string LoggerDecimalSeparator { get; set; }
        public int LoggerGraphicsMouseZoom { get; set; }
        public int LoggerGraphicsMouseWheel { get; set; }
        public int LoggerGraphicsMouseCursor { get; set; }
        public bool TableEditorRememberCompare { get; set; }
        public int TableEditorCompareSelection { get; set; }
        public int TableEditorCompareType { get; set; }
        public bool TunerShowTableCount { get; set; }
        public int NavigatorMaxTablesPerNode { get; set; }
        public int NavigatorMaxTablesTotal { get; set; }
        public bool LoggerStartJ2534Process { get; set; }
        public bool LoggerJ2534ProcessVisible { get; set; }
        public bool LoggerAutoDisconnect { get; set; }
        public bool LoggerFilterStartOfMessage { get; set; }
        public bool LoggerFilterTXIndication { get; set; }
        public string LoggerFilterRxStatusCustom { get; set; }
        public int LoggerConsoleDisplayInterval { get; set; }
        public int RetryWriteTimes { get; set; }
        public int RetryWriteDelay { get; set; }
        public int LoggerResetAfterMiss { get; set; }
        public bool ClearFuncAddrOnDisconnect { get; set; }

        public TimeoutScenario TimeoutLoggingActive { get; set; }
        public TimeoutScenario TimeoutLoggingActiveObdlink { get; set; }
        public TimeoutScenario TimeoutLoggingPassive { get; set; }
        public int TimeoutScriptRead { get; set; }
        public int TimeoutPortRead { get; set; }
        public int TimeoutAnalyzerReceive { get; set; }
        public int TimeoutReceive { get; set; }
        public int TimeoutLoggingReceive { get; set; }
        public int TimeoutScriptWrite { get; set; }
        public int TimeoutConsoleReceive { get; set; }
        public int TimeoutLoggingWrite { get; set; }
        //public int TimeoutJconsoleWrite { get; set; }
        //public int TimeoutJ2534DeviceRead { get; set; }

        public SerializableFont TableEditorFont { get; set; }
        public SerializableFont TableExplorerFont { get; set; }
        public SerializableFont PatcherLogFont { get; set; }
        public SerializableFont DebugFont { get; set; }
        public SerializableFont LoggerConsoleFont { get; set; }

        public void Save()
        {
            string xmlfile = Path.Combine(Application.StartupPath, "universalpatcher.xml");
            using (FileStream stream = new FileStream(xmlfile, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(UpatcherSettings));
                writer.Serialize(stream, this);
                stream.Close();
            }
        }
        public UpatcherSettings ShallowCopy()
        {
            return (UpatcherSettings)this.MemberwiseClone();
        }

    }
    /// <summary>
    /// Font descriptor, that can be xml-serialized
    /// </summary>
    public class SerializableFont
    {
        public string FontFamily { get; set; }
        public GraphicsUnit GraphicsUnit { get; set; }
        public float Size { get; set; }
        public FontStyle Style { get; set; }

        /// <summary>
        /// Intended for xml serialization purposes only
        /// </summary>
        private SerializableFont() { }

        public SerializableFont(Font f)
        {
            FontFamily = f.FontFamily.Name;
            GraphicsUnit = f.Unit;
            Size = f.Size;
            Style = f.Style;
        }

        public SerializableFont(string f)
        {
            string[] parts = f.Split(';');
            if (parts.Length == 3)
            {
                FontFamily = parts[0];
                if (float.TryParse(parts[1], out float s))
                    Size = s;
                if (Enum.TryParse<FontStyle>(parts[2], out FontStyle fs))
                    Style = fs;
                GraphicsUnit = GraphicsUnit.Point;
                //if (Enum.TryParse<GraphicsUnit>(parts[3], out GraphicsUnit gu))
                  //  GraphicsUnit = gu;
            }
        }

        public static SerializableFont FromFont(Font f)
        {
            return new SerializableFont(f);
        }

        public Font ToFont()
        {
            if (string.IsNullOrEmpty(FontFamily))
            {
                return new Font("Consolas", 8);
            }
            return new Font(FontFamily, Size, Style, GraphicsUnit);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(FontFamily);
            sb.Append(";" + Size.ToString());
            sb.Append(";" + Style.ToString());
            //sb.Append(";" + GraphicsUnit.ToString());
            return sb.ToString();
        }

    }
    
}
