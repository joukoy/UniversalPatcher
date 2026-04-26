using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static LoggerUtils;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using static UniversalPatcher.PidConfig;
using System.Xml.Linq;

namespace UniversalPatcher
{
    public class DataLogger
    {
        public DataLogger()
        {
            SetProtocol();
        }
        private CancellationTokenSource logTokenSource = new CancellationTokenSource();
        private CancellationTokenSource logWriterTokenSource = new CancellationTokenSource();
        private CancellationToken logToken;
        private CancellationToken logWriterToken;
        public Task logTask;
        private Task logWriterTask;
        //public IPort port;
        public bool Connected = false;
        public List<PidConfig> PidProfile { get; set; }
        private StreamWriter logwriter;
        private string logseparator = ";";
        //public Device MainConnection.ObdDevice;
        public int ReceivedBytes = 0;
        public string OS;
        public SlotHandler slothandler;
        public MessageReceiver Receiver;

        public bool LogRunning = false;
        public bool AnalyzerRunning = false;
        private  bool AllSlotsRequested = false;
        private  bool passive;
        public int maxPassiveSlotsPerRequest = 50;
        //public  bool stopLogLoop;
        public  byte priority = Priority.Physical0;
        private  DateTime lastPresent = DateTime.Now;
        private  DateTime lastElmStop = DateTime.Now;

        //public  Queue<Analyzer.AnalyzerData> analyzerq = new Queue<Analyzer.AnalyzerData>();
        public  Queue<LogData> LogFileQueue = new Queue<LogData>();
        private Queue<QueuedCommand> queuedCommands = new Queue<QueuedCommand>();

        public List<LogData> LogDataBuffer;

        //Set these values before StartLogging()
        public DateTime LogStartTime;
        public bool writelog;
        //public  bool useVPWFilters;
        public  bool reverseSlotNumbers;
        public  byte Responsetype;
        public  int maxSlotsPerRequest = 4;   //How many Slots before asking more
        public  int maxSlotsPerMessage = 4;   //How many Slots in one Slot request message
        //private bool VPWProtocol = true;
        public CANDevice CanDevice;
        public CANDevice DtcCurrentModule;

        //public ushort CanPcmAddr;
        //public byte CanPcmAddrByte1;
        //public byte CanPcmAddrByte2;

        public double[] LastCalculatedValues;
        public string[] LastCalculatedStringValues;

        public List<LogParam.PidParameter> PidParams = new List<LogParam.PidParameter>();
        public List<LogParam.PidSettings> SelectedPids = new List<LogParam.PidSettings>();

        public List<DBC.DBCMsg> dbcMessages;

        public enum QueueCmd
        {
            Getdtc,
            GetVin,
            QueryPid,
            Custom
        }
        public class QueuedCommand
        {
            public QueueCmd Cmd { get; set; }
            public CANDevice module { get; set; }
            public bool history { get; set; }
            public int customMode { get; set; }
            public OBDMessage CustomMsg { get; set; }
            public string Description { get; set; }
            public LogParam.PidSettings PidSettings { get; set; }
        }

        public enum LoggingDevType
        {
            Elm,
            Obdlink,
            Avt,
            Jet,
            OBDX,
            J2534,
            UPX_OBD
        }
        public enum LoggingProtocol
        {
            HSCAN,
            LSCAN,
            VPW,
        }

        public enum iPortType
        {
            Serial,
            FTDI,
            TcpIP,
            J2534
        }
        public enum PidMisMatch
        {
            NewCustom,
            UseMaster,
            UseMasterSave,
            UseProfile,
        }

        public class LogData
        {
            public LogData(int Size)
            {
                Values = new double[Size];
                CalculatedValues = new double[Size];
            }
            public double[] Values { get; set; }
            public double[] CalculatedValues { get; set; }
            public long TimeStamp { get; set; }
            public ulong DevTimeStamp { get; set; }
        }

        public class ReadValue
        {
            public ReadValue()
            {
                PidValue = double.MinValue;
                FailureCode = 0;
            }
            public string ErrorText
            {
                get
                {
                    if (FailureCode == 0)
                        return "";
                    return PcmResponses[FailureCode];
                }
            }
            public double PidValue { get; set; }
            public int PidNr { get; set; }
            public long TimeStamp { get; set; }
            public ulong DevTimeStamp { get; set; }
            public byte FailureCode { get; set; }
        }

        public class DTCCodeStatus
        {
            public string Module { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
        }

        public class LogDataEvents
        {
            public class LogDataEvent : EventArgs
            {
                public LogDataEvent(LogData data)
                {
                    this.Data = data;
                }
                public LogData Data { get; internal set; }
            }

            public event EventHandler<LogDataEvent> LogDataAdded;

            protected virtual void OnLogUpdated(LogDataEvent e)
            {
                LogDataAdded?.Invoke(this, e);
            }

            public void Add(LogData data)
            {
                LogDataEvent lde = new LogDataEvent(data);
                OnLogUpdated(lde);
            }

        }

        public Device CreateSerialDevice(string serialPortName, string serialPortDeviceType, iPortType portType, ref IPort port)
        {
            try
            {
                if (portType == iPortType.FTDI)
                    port = new FTDIPort(serialPortName);
                else if (portType == iPortType.Serial)
                    port = new Rs232Port(serialPortName);
                else if (portType == iPortType.TcpIP)
                    port = new TcpIpPort(serialPortName);

                Device device;
                switch (serialPortDeviceType)
                {
                    case OBDXProDevice.DeviceType:
                        device = new OBDXProDevice(port);
                        break;

                    case AvtDevice.DeviceType:
                        device = new AvtDevice(port);
                        break;

                    case JetDevice.DeviceType:
                        device = new JetDevice(port);
                        break;
                    case ElmDevice.DeviceType:
                        device = new ElmDevice(port);
                        break;
                    case Elm327Device.DeviceType:
                        device = new Elm327Device(port);
                        break;
                    case UPX_OBD.DeviceType:
                        device = new UPX_OBD(port);
                        break;
                    default:
                        device = null;
                        break;
                }

                if (device == null)
                {
                    return null;
                }

                return device;
            }
            catch (Exception exception)
            {
                LoggerBold($"Unable to create {serialPortDeviceType} on {serialPortName}.");
                Debug.WriteLine(exception.ToString());
                return null;
            }
        }

        public void SetProtocol()
        {
            Receiver = new MessageReceiver();
            RealTimeControls = LoadRealTimeControls();
            slothandler = new SlotHandler();
        }
        public  bool CreateLog(string path)
        {
            try
            {
                Logger("Writing to logfile: " + path);
                logseparator = AppSettings.LoggerLogSeparator;
                LogFileQueue.Clear();
                if (logwriter != null && logwriter.BaseStream != null)
                {
                    logwriter.Close();
                }
                if (File.Exists(path))
                {                    
                    logwriter = new StreamWriter(path, true);
                }
                else
                {
                    StringBuilder sb = new StringBuilder("Time" + logseparator + "Elapsed time" + logseparator);
                    for (int c = 0; c < SelectedPids.Count; c++)
                    {
                        sb.Append(SelectedPids[c].Parameter.Name);
                        if (c < SelectedPids.Count - 1)
                            sb.Append(logseparator);
                    }
                    string header = sb.ToString();
                    logwriter = new StreamWriter(path);
                    logwriter.WriteLine(header);
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold("Create logfile: " + ex.Message);
            }
            return false;
        }

        public void WriteLog(string[] logvalues, string timestamp)
        {
            try
            {
                if (logwriter != null && writelog)
                {
                    string CultureDecim = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string Decim = AppSettings.LoggerDecimalSeparator;
                    StringBuilder sb = new StringBuilder(timestamp + logseparator + DateTime.Now.Subtract(LogStartTime).ToString() + logseparator);
                    for (int c = 0; c < logvalues.Length; c++)
                    {
                        sb.Append(logvalues[c].Replace(CultureDecim, Decim));
                        if (c < (logvalues.Length - 1))
                            sb.Append(logseparator);
                    }
                    logwriter.WriteLine(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, DataLogger line " + line + ": " + ex.Message);
            }
        }


        private void LogWriterLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            //while (!stopLogLoop)
            while (!logWriterToken.IsCancellationRequested)
            {
                try
                {
                    while (LogFileQueue.Count == 0)
                    {
                        Thread.Sleep(100);
                        if (logWriterToken.IsCancellationRequested)
                        {
                            if (logwriter != null && logwriter.BaseStream != null)
                            {
                                logwriter.Close();
                            }
                            logwriter = null;
                            return;
                        }
                    }
                    LogData ld;
                    lock (LogFileQueue)
                    {
                        ld = LogFileQueue.Dequeue();
                    }
                    LastCalculatedStringValues = slothandler.CalculatePidValues(ld.Values);
                    string tStamp = new DateTime((long)ld.TimeStamp).ToString(AppSettings.LoggerTimestampFormat);
                    WriteLog(LastCalculatedStringValues, tStamp);
                    //Data for Histogram & Graphics:
                    ld.CalculatedValues = slothandler.CalculatePidDoubleValues(ld.Values);
                    Array.Copy(ld.CalculatedValues, LastCalculatedValues, LastCalculatedValues.Length);
                    LoggerDataEvents.Add(ld);
                    LogDataBuffer.Add(ld);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("LogWriterLoop error: " + ex.Message);
                }
            }
            logwriter.Close();
            logwriter = null;
        }
        public void SaveOldProfile(string FileName)
        {
            try
            {
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidConfig>));
                    writer.Serialize(stream, PidProfile);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, DataLogger line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
            }
        }


        public int LoadProfileFromCsv(string FileName)
        {
            int tStamps = 0;
            try
            {
                StreamReader sr = new StreamReader(FileName);
                string hdrLine = sr.ReadLine();
                sr.Close();
                string[] hdrArray = hdrLine.Split(new string[] { AppSettings.LoggerLogSeparator }, StringSplitOptions.None);
                for (int i = 0; i < hdrArray.Length; i++)
                {
                    if (hdrArray[i].ToLower().Contains("time"))
                    {
                        tStamps++;
                    }
                    else
                    {
                        break;
                    }
                }
                Logger("Creating new profile from CSV...");
                SelectedPids = new List<LogParam.PidSettings>();
                for (int p = tStamps; p < hdrArray.Length; p++)
                {
                    LogParam.PidParameter parm = PidParams.Where(X => X.Name.ToLower() == hdrArray[p].ToLower()).FirstOrDefault();
                    if (parm == null)
                    {
                        //Logger("Unknown PID: " + hdrArray[p] + ", Adding temporary PID configuration");
                        parm = new LogParam.PidParameter();
                        parm.Name = hdrArray[p];
                        parm.Type = LogParam.DefineBy.Math;
                        parm.AddPid("0003", "Raw");
                        parm.Id = "Math" + parm.Name;
                        parm.Conversions.Add(new Conversion("Raw", "x", "0.00"));
                        //PidParams.Add(parm);
                    }
                    LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                    if (parm.Conversions[0].IsBitMapped)
                    {
                        pidSettings.Units = parm.Conversions[0];
                    }
                    else
                    {
                        pidSettings.Units = parm.Conversions.Where(X => X.Units == "Raw").FirstOrDefault();
                    }
                    if (pidSettings.Units == null)
                    {
                        pidSettings.Units = parm.Conversions[0];
                    }
                    SelectedPids.Add(pidSettings);
                }
                Logger("OK");

/*                PidProfile = new List<PidConfig>();
                for (int p=tStamps; p< hdrArray.Length;p++)
                {
                    PidConfig pc = new PidConfig();
                    pc.PidName = hdrArray[p];
                    pc.addr = 0xffffff;
                    pc.Math = "X";
                    PidProfile.Add(pc);
                }
*/
                slothandler = new SlotHandler();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            return tStamps;
        }

        public static uint GetPidAddressbySearchstring(LogParam.PidSettings pidProfile)
        {
            List<LogParam.Location> locations = new List<LogParam.Location>();
            LogParam.Location location = pidProfile.Parameter.Locations.Where(X => X.Os.Contains("search")).FirstOrDefault();
            if (location == null)
            {
                return uint.MaxValue; //No search strings defined
            }
            if (frmlogger.PCM == null || frmlogger.PCM.buf.Length == 0)
            {
                Logger("Select bin file to search PID address for " + pidProfile.Parameter.Name);
                string fName = SelectFile("Select BIN file");
                if (fName.Length == 0)
                    return uint.MaxValue;
                frmlogger.PCM = new PcmFile(fName, true, "");
            }
            if (frmlogger.PCM != null && !string.IsNullOrEmpty(frmlogger.PCM.configFile))
            {
                string platformStr = "search:" + frmlogger.PCM.configFile.ToLower();
                locations = pidProfile.Parameter.Locations.Where(X => X.Os == platformStr).ToList();
            }
            uint addr = uint.MaxValue;
            foreach (LogParam.Location loc in locations)
            {
                string searchStr = loc.Os;
                uint startAddr = 0;
                addr = GetAddrbySearchString(frmlogger.PCM, searchStr.Replace("search:", ""), ref startAddr, frmlogger.PCM.fsize).Addr;
                if (addr < uint.MaxValue)
                {
                    addr = 0xFF0000 + addr;
                    break;
                }
            }
            return addr;
        }

        public bool SetPidAddressByOS(LogParam.PidSettings pidSettings, bool Verbose)
        {
            LogParam.Location location = pidSettings.Parameter.Locations.Where(X => X.Os == OS).FirstOrDefault();
            if (location == null)
            {
                uint addr = GetPidAddressbySearchstring(pidSettings);
                if (addr == uint.MaxValue)
                {
                    if (Verbose)
                    {
                        Logger("Address not available");
                    }
                    return false;
                }
                location = new LogParam.Location();
                location.addr = (int)addr;
                location.Os = frmlogger.PCM.OS;
                pidSettings.Parameter.Locations.Add(location);
                pidSettings.Os = location;
            }
            pidSettings.Os = location;
            return true;
        }
        public string QueryPid(LogParam.PidSettings pidProfile, bool Verbose)
        {
            string retVal = null;
            try
            {

                LogParam.PidParameter parm = pidProfile.Parameter;
                if (parm.Type == LogParam.DefineBy.Address)
                {
                    if (Verbose)
                    {
                        Logger("Setting address by OS " + OS);
                    }
                    if (!SetPidAddressByOS(pidProfile, Verbose))
                    {
                        return "";
                    }
                }
                else if (parm.Type == LogParam.DefineBy.Math)
                {
                    foreach(LogParam.LogPid lPid in parm.Pids)
                    {
                        if (lPid.Parameter.Type == LogParam.DefineBy.Address)
                        {
                            LogParam.Location location = lPid.Parameter.Locations.Where(X => X.Os == OS).FirstOrDefault();
                            if (location == null)
                            {
                                if (Verbose)
                                {
                                    Logger("Address not available");
                                }
                                return null;
                            }
                            pidProfile.Os = location;
                        }
                    }
                }
                retVal =  parm.GetCalculatedStringValue(pidProfile, true);
                if (Verbose)
                {
                    if (string.IsNullOrEmpty(retVal))
                    {
                        Logger("Error, or not supported pid: " + pidProfile.Parameter.Name);
                    }
                    else
                    {
                        if (pidProfile.Units.Units == "Raw")
                        {
                            Logger("PID: " + parm.Name + ", Value: " + retVal + " (" + pidProfile.Units.ToString() + ")");

                        }
                        else
                        {
                            Logger("PID: " + parm.Name + ", Value: " + retVal + " " + pidProfile.Units.ToString());
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
                LoggerBold("Error, datalogger line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        private bool TestPid(LogParam.PidSettings pidSettings)
        {
            ReadValue rv = QuerySinglePidValue(pidSettings.Parameter);
            if (rv.FailureCode == 0)
            {
                Debug.WriteLine("Pid ok: " + pidSettings.Parameter.Name);
                return true;
            }
            else
            {
                Debug.WriteLine("Pid failed, error: " + rv.ErrorText + ", run second test with 2C");
                List<LogParam.PidSettings> tmpdPids = new List<LogParam.PidSettings>();
                tmpdPids.Add(pidSettings);
                slothandler = new SlotHandler();
                if (!slothandler.CreatePidSetupMessages(tmpdPids))
                {
                    Debug.WriteLine("Incompatible pid: " + pidSettings.Parameter.Name);
                    return false;
                }
                else
                {
                    Debug.WriteLine("Secondary pid test succesful, pid: " + pidSettings.Parameter.Name);
                    return true;
                }
            }

        }
        public void RemoveUnsupportedPids()
        {
            try
            {
                Logger("Checking pid compatibility... ");
                Receiver.StopReceiveLoop();

                for (int p = SelectedPids.Count - 1; p >= 0; p--)
                {
                    LogParam.PidSettings pidSettings = SelectedPids[p];
                    if (pidSettings.Parameter.Type == LogParam.DefineBy.Math)
                    {
                        foreach (LogParam.PidParameter pidParameter in pidSettings.Parameter.GetLinkedPids())
                        {
                            LogParam.PidSettings tmpSettings = new LogParam.PidSettings(pidParameter);
                            if (TestPid(tmpSettings) == false)
                            {
                                Logger("Removing incompatible pid: " + pidSettings.Parameter.Name);
                                SelectedPids.Remove(pidSettings);
                                break;  //Don't test other pids of this parameter
                            }
                        }
                    }
                    else if (pidSettings.Parameter.Type == LogParam.DefineBy.Pid)
                    {
                        if (TestPid(pidSettings) == false)
                        {
                            Logger("Removing incompatible pid: " + pidSettings.Parameter.Name);
                            SelectedPids.Remove(pidSettings);
                        }
                    }
                    else if (pidSettings.Parameter.Type == LogParam.DefineBy.Address)
                    {
                        if (pidSettings.Os == null || TestPid(pidSettings) == false)
                        {
                            Logger("Removing incompatible pid: " + pidSettings.Parameter.Name);
                            SelectedPids.Remove(pidSettings);
                        }
                    }
                }            
                Logger("Done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, datalogger line " + line + ": " + ex.Message);
            }
        }

        public ReadValue QuerySinglePidValue(LogParam.PidParameter parm)
        {
            //Receiver.SetReceiverPaused(true);
            ReadValue rv = new ReadValue();
            try
            {
                ushort expectedSrc;
                OBDMessage request = null;
                int addr = parm.Address;
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    expectedSrc = (ushort)(DeviceId.Tool << 8 | DeviceId.Pcm);
                    if (parm.Address > ushort.MaxValue) //RAM
                    {
                        request = new OBDMessage(new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x23, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr, 0x01 });
                    }
                    else
                    {
                        request = new OBDMessage(new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x22, (byte)(addr >> 8), (byte)addr, 0x01 });
                    }
                }
                else //CAN
                {
                    expectedSrc = (ushort)CanDevice.ResID;
                    if (parm.Address > ushort.MaxValue) //RAM
                    {
                        request = new OBDMessage(new byte[] { 0x00, 0x00, CanDevice.RequestByte1, CanDevice.RequestByte2, 0x23, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr, 0x01 });
                    }
                    else
                    {
                        request = new OBDMessage(new byte[] { 0x00, 0x00, CanDevice.RequestByte1, CanDevice.RequestByte2, 0x22, (byte)(addr >> 8), (byte)addr });
                    }
                }
                MainConnection.ObdDevice.ReceiveBufferedMessages();
                MainConnection.ObdDevice.ClearMessageQueue();
                //MainConnection.ObdDevice.SetTimeout(TimeoutScenario.ReadProperty);
                if (MainConnection.ObdDevice.SendMessage(request,1))
                {
                    Application.DoEvents();
                    OBDMessage resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    //if (resp.GetBytes()[3] == 0x7f)
                    DateTime startTime = DateTime.Now;
                    int retry = 0;
                    while (true)
                    {
                        //if (resp != null && resp.Length > 3)
                        if (ValidateQueryResponse(resp,expectedSrc))
                        {
                            rv = ParseSinglePidMessage(resp, parm);
                            if (rv.FailureCode == 0x23)
                            {
                                MainConnection.ObdDevice.SendMessage(request, 1);
                            }
                            else if (rv.FailureCode == 0x31)
                            {
                                LoggerBold(Environment.NewLine +  "Pid not supported: " + addr.ToString("X4"));
                                //Receiver.SetReceiverPaused(false);
                                return rv;
                            }
                            else if (rv.FailureCode > 0)
                            {
                                LoggerBold("Pid request failed, pid: " + addr.ToString("X4") + ", Error: " + rv.ErrorText);
                                //Receiver.SetReceiverPaused(false);
                                return rv;
                            }
                            if (rv.PidNr == addr)
                            {
                                Debug.WriteLine("Pid received after " + retry.ToString() + " retries");
                                //Receiver.SetReceiverPaused(false);
                                return rv;
                            }
                            else
                            {
                                Debug.WriteLine("Requested pid: " + addr.ToString("X4") + ", received: " + rv.PidNr.ToString("X4"));
                            }
                        }
                        Application.DoEvents();
                        if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(2000))
                        {
                            LoggerBold("Timeout requesting pid: " + addr.ToString("X4"));
                            rv.FailureCode = 0x21; //Busy
                            //Receiver.SetReceiverPaused(false);
                            return rv;
                        }
                        retry++;
                        resp = MainConnection.ObdDevice.ReceiveMessage(true);
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
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            ///Receiver.SetReceiverPaused(false);
            return rv;
        }


        public ReadValue QueryModulePidValue(int addr, ushort module)
        {
            //Receiver.SetReceiverPaused(true);
            ReadValue rv = new ReadValue();
            try
            {
                OBDMessage request = null;
                ushort expectedSrc;
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    expectedSrc = (ushort)(DeviceId.Tool << 8 | module);
                    if (addr > ushort.MaxValue) //RAM
                    {
                        request = new OBDMessage(new byte[] { Priority.Physical0, (byte)module, DeviceId.Tool, 0x23, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr, 0x01 });
                    }
                    else
                    {
                        request = new OBDMessage(new byte[] { Priority.Physical0,(byte)module, DeviceId.Tool, 0x22, (byte)(addr >> 8), (byte)addr, 0x01 });
                    }
                }
                else //CAN
                {
                    expectedSrc = module;
                    byte moduleByte1 = (byte)(module >> 8);
                    byte moduleByte2 = (byte)(module);
                    if (addr > ushort.MaxValue) //RAM
                    {
                        request = new OBDMessage(new byte[] { 0x00, 0x00, moduleByte1, moduleByte2, 0x23, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr, 0x01 });
                    }
                    else
                    {
                        request = new OBDMessage(new byte[] { 0x00, 0x00, moduleByte1, moduleByte2, 0x22, (byte)(addr >> 8), (byte)addr });
                    }
                }
                MainConnection.ObdDevice.ReceiveBufferedMessages();
                MainConnection.ObdDevice.ClearMessageQueue();
                if (MainConnection.ObdDevice.SendMessage(request, 1))
                {
                    Application.DoEvents();
                    OBDMessage resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    //if (resp.GetBytes()[3] == 0x7f)
                    DateTime startTime = DateTime.Now;
                    int retry = 0;
                    int offset=0;
                    if (MainConnection.LoggingProto != LoggingProtocol.VPW)
                    {
                        offset = 1;
                    }
                    while (true)
                    {
                        //if (resp != null && resp.Length > offset + 3)
                        if (ValidateQueryResponse(resp,expectedSrc))
                        {
                            if (resp[offset + 3] == 0x7f)
                            {
                                rv.FailureCode = resp.GetBytes().Last();
                                if (rv.FailureCode == 0x31) //31 = Out of range
                                {
                                    LoggerBold("Pid not supported: " + addr.ToString("X4"));
                                    return rv;
                                }
                                else if (rv.FailureCode == 0x23)
                                {
                                    Debug.WriteLine("Routine not ready, retry");
                                    MainConnection.ObdDevice.SendMessage(request, 1);
                                }
                                else
                                {
                                    LoggerBold("Pid request failed, pid: " + addr.ToString("X4") + ", Error: " + rv.ErrorText);
                                    return rv;
                                }
                            }
                            else if (resp[offset + 3] == 0x62)
                            {
                                rv.PidNr = ReadUint16(resp.GetBytes(), (uint)(offset + 4), true);
                            }
                            else if (resp[offset + 3] == 0x63)
                            {
                                byte[] tmp = new byte[4];
                                Array.Copy(resp.GetBytes(), offset + 4, tmp, 2, 2);
                                tmp[1] = 0xFF;
                                rv.PidNr = (int)ReadUint32(tmp, 0, true);
                            }
                            if (rv.PidNr == addr)
                            {
                                Debug.WriteLine("Pid received after " + retry.ToString() + " retries");
                                string val = "";
                                for (int x=offset +6; x < resp.Length; x++)
                                {
                                    val += resp[x].ToString("X2") + " ";
                                }
                                Logger("Pid: " + addr.ToString("X4") + ", value: " + val);
                                return rv;
                            }
                            else
                            {
                                Debug.WriteLine("Requested pid: " + addr.ToString("X4") + ", received: " + rv.PidNr.ToString("X4"));
                            }
                        }
                        Application.DoEvents();
                        retry++;
                        if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(1000))
                        {
                            LoggerBold("Timeout requesting pid: " + addr.ToString("X4"));
                            return rv;
                        }
                        resp = MainConnection.ObdDevice.ReceiveMessage(true);
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
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            //Receiver.SetReceiverPaused(false);
            return rv;
        }

        public bool SetBusQuiet()
        {
            try
            {
                if (MainConnection.LoggingProto != LoggingProtocol.VPW)
                {
                    Debug.WriteLine("No bus quiet for CAN");
                    return true;
                }
                Debug.WriteLine("Set bus quiet");
                byte[] quietMsg = { priority, DeviceId.Broadcast, DeviceId.Tool, Mode.ExitKernel };
                bool m = MainConnection.ObdDevice.SendMessage(new OBDMessage(quietMsg), 10);
                if (m)
                {
                    //Debug.WriteLine("OK");
                }
                else
                {
                    Debug.WriteLine("Unable to set bus quiet");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(quietMsg, b => b.ToString("X2"))));
                    return false;
                }
                //Thread.Sleep(10);
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold("SetBusQuiet: " + ex.Message);
                return false;
            }
        }



        public Response<List<byte>> QueryDevicesOnBus(bool waitanswer)
        {
            //Receiver.SetReceiverPaused(true);
            List<byte> retVal = new List<byte>();
            try
            {
                if (waitanswer)
                {
                    Logger("Waiting for devices", false);
                }
                Debug.WriteLine("Devices on bus?");
                //MainConnection.ObdDevice.ClearMessageBuffer();
                MainConnection.ObdDevice.ClearMessageQueue();
                //MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging3);
                OBDMessage queryMsg1 = new OBDMessage(new  byte[]{ Priority.Physical0High, DeviceId.Broadcast, DeviceId.Tool, 0x20 });
                OBDMessage queryMsg2 = new OBDMessage(new byte[] { Priority.Physical0, DeviceId.Broadcast, DeviceId.Tool, 0x28, 0x00 });
                OBDMessage[] querys = { queryMsg1, queryMsg2 };
                foreach (OBDMessage queryMsg in querys)
                {
                    bool m = MainConnection.ObdDevice.SendMessage(queryMsg, 100);
                    if (m)
                    {
                        //Debug.WriteLine("OK" );
                    }
                    else
                    {
                        Logger("No respond to Query devices message");
                        Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg.Data, b => b.ToString("X2"))));
                        //Receiver.SetReceiverPaused(false);
                        return new Response<List<byte>>(ResponseStatus.Error, retVal);
                    }
                    if (waitanswer)
                    {
                        for (int wait = 0; wait < 3; wait++)
                        {
                            Thread.Sleep(100);
                            DateTime startTime = DateTime.Now;
                            OBDMessage resp;
                            do
                            {
                                resp = MainConnection.ObdDevice.ReceiveMessage(true);
                                if (resp != null && resp.Length > 3 && resp[1] == DeviceId.Tool && resp[3] == 0x60)
                                {
                                    byte module = resp[2];
                                    if (!retVal.Contains(module))
                                        retVal.Add(module);
                                    Debug.WriteLine("Response: " + resp.ToString());
                                }
                                if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(300))
                                {
                                    Debug.WriteLine("Timeout waiting for null message");
                                    break;
                                }
                            } while (resp != null);
                            Logger(".", false);
                            Application.DoEvents();
                        }
                    }
                }
                Logger("[Done]");
                //Receiver.SetReceiverPaused(false);
                return new Response<List<byte>>(ResponseStatus.Success, retVal);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            //Receiver.SetReceiverPaused(false);
            return new Response<List<byte>>(ResponseStatus.Error, retVal);
        }

        public Response<List<OBDMessage>> QueryModules(int DevCount)
        {
            List<OBDMessage> retVal = new List<OBDMessage>();
            try
            {
                Debug.WriteLine("Modules?");
                MainConnection.ObdDevice.ClearMessageQueue();
                //Receiver.SetReceiverPaused(true);
                Logger("Querying modules", false);
                Application.DoEvents();
                OBDMessage resp = null;
                for (byte modId = 0; modId < 0xFF; modId++)
                {
                    if (!Connected)
                    {
                        return new Response<List<OBDMessage>>(ResponseStatus.Error, retVal);
                    }
                    byte[] queryMsg = { Priority.Physical0High, DeviceId.Broadcast, DeviceId.Tool, 0x3c, modId };
                    //MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging1);
                    MainConnection.ObdDevice.ReceiveBufferedMessages();
                    MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), DevCount + 5);
                    if (modId % 10 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                    Thread.Sleep(20);
                    DateTime startTime1 = DateTime.Now;
                    do
                    {
                        resp = MainConnection.ObdDevice.ReceiveMessage(false);
                        if (resp != null && resp.Length > 5 && resp[1] == DeviceId.Tool && resp[3] == 0x7c)
                        {
                            retVal.Add(resp);
                            Debug.WriteLine("Response: " + resp.ToString());
                        }
                        Application.DoEvents();
                        if (DateTime.Now.Subtract(startTime1) > TimeSpan.FromMilliseconds(1000))
                        {
                            Debug.WriteLine("Timeout waiting for null message");
                            break;
                        }
                    } while (resp != null);
                }
                Thread.Sleep(100);
                DateTime startTime2 = DateTime.Now;
                do
                {
                    resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    if (resp != null && resp.Length > 5 && resp[1] == DeviceId.Tool && resp[3] == 0x7c)
                    {
                        retVal.Add(resp);
                        Debug.WriteLine("Response: " + resp.ToString());
                    }
                    Application.DoEvents();
                    if (DateTime.Now.Subtract(startTime2) > TimeSpan.FromMilliseconds(1000))
                    {
                        Debug.WriteLine("Timeout waiting for null message");
                        break;
                    }
                } while (resp != null);
                Logger(" [Done]");
                //Receiver.SetReceiverPaused(false);
                return new Response<List<OBDMessage>>(ResponseStatus.Success, retVal);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
                //Receiver.SetReceiverPaused(false);
                return new Response<List<OBDMessage>>(ResponseStatus.Error, retVal);
            }
        }

        public int[] SetCanQueryFilter(ushort module)
        {
            try
            {
                if (module == CanDevice.ResID)
                {
                    return null;    //Filter already set for CPM
                }
                CANDevice cd = CANQuery.GetDeviceAddresses(module);
                string filterTxt = "Type: FLOW_CONTROL_FILTER,Name: CANtmpFlowFilter" + Environment.NewLine;
                filterTxt += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                filterTxt += "Pattern: " + cd.ResID.ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                filterTxt += "FlowControl: " + cd.RequestID.ToString("X8") + ",RxStatus:NONE,TxFlags:NONE" + Environment.NewLine;
                filterTxt += "Type: PASS_FILTER,Name: CANtmpPassFilter" + Environment.NewLine;
                filterTxt += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                filterTxt += "Pattern: " + cd.DiagID.ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                Application.DoEvents();
                return MainConnection.ObdDevice.SetupFilters(filterTxt, false, false);

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
                return null;
            }
        }

        public Response<List<OBDMessage>> QueryCANModules(ushort module)
        {
            List<OBDMessage> retVal = new List<OBDMessage>();
            int[] filterIds = SetCanQueryFilter(module);
            Thread.Sleep(100);
            ResponseStatus responseStatus = ResponseStatus.Success;
            try
            {
                Debug.WriteLine("Modules?");
                //MainConnection.ObdDevice.ClearMessageBuffer();
                MainConnection.ObdDevice.ClearMessageQueue();
                Receiver.SetReceiverPaused(true);
                Logger("Querying modules", false);
                Application.DoEvents();
                OBDMessage resp = null;
                for (byte modId = 0; modId < 0xFF; modId++)
                {
                    byte[] queryMsg = { 0x00, 0x00, (byte)(module >> 8), (byte)module, 0x1a, modId };
                    //MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging1);
                    MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), 5);
                    if (modId % 10 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                    Thread.Sleep(50);
                    DateTime startTime = DateTime.Now;
                    do
                    {
                        resp = MainConnection.ObdDevice.ReceiveMessage(false);
                        if (resp != null && resp.Length > 5 && resp[4] == 0x5A)
                        {
                            retVal.Add(resp);
                            Debug.WriteLine("Response: " + resp.ToString());
                        }
                        if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(300))
                        {
                            Debug.WriteLine("Timeout waiting for null message");
                            break;
                        }
                    } while (resp != null);
                }

                Thread.Sleep(100);
                DateTime startTime2 = DateTime.Now;
                do
                {
                    resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    if (resp != null && resp.Length > 5 && resp[4] == 0x5A)
                    {
                        retVal.Add(resp);
                        Debug.WriteLine("Response: " + resp.ToString());
                    }
                    if (DateTime.Now.Subtract(startTime2) > TimeSpan.FromMilliseconds(300))
                    {
                        Debug.WriteLine("Timeout waiting for null message");
                        break;
                    }
                } while (resp != null) ;
                Logger(" [Done]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
                responseStatus = ResponseStatus.Error;
            }
            finally
            {
                if (filterIds != null)
                {
                    MainConnection.ObdDevice.RemoveFilters(filterIds);
                }
                Receiver.SetReceiverPaused(false);
            }
            return new Response<List<OBDMessage>>(responseStatus, retVal);
        }

        public Response<List<OBDMessage>> QueryFreezeFrames(CANDevice module)
        {
            List<OBDMessage> retVal = new List<OBDMessage>();
            try
            {
                Debug.WriteLine("Freeze frames?");
                //MainConnection.ObdDevice.ClearMessageBuffer();
                MainConnection.ObdDevice.ClearMessageQueue();
                Receiver.SetReceiverPaused(true);
                Logger("Querying freeze frames", false);
                //MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging1);
                //MainConnection.ObdDevice.SetReadTimeout(500);
                Application.DoEvents();
                OBDMessage resp = null;
                byte[] queryMsg;
                ushort expectedSrc;
                int offset;
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    expectedSrc = (ushort)(DeviceId.Tool<<8 | (byte)module.ModuleID);
                    offset = 0;
                }
                else
                {
                    expectedSrc = (ushort)module.RequestID;
                    offset = 1;
                }
                for (byte x = 0; x < 6; x++)
                {

                    if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                    {
                        queryMsg = new byte[] { Priority.Physical0High, (byte)module.ModuleID, DeviceId.Tool, 0x12, x, 0xFF, 0xFF };
                        offset = 0;
                    }
                    else
                    {
                        //queryMsg = new byte[] { 0x00,0x00,(byte)(module >>8),(byte)module, 0x12, x, 0xFF, 0xFF };
                        queryMsg = new byte[] { 0x00, 0x00, (byte)(module.DiagID >> 8), (byte)module.DiagID, 0x12, x };
                        offset = 1;
                    }
                    MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), 500);
                    Logger(".", false);
                    Application.DoEvents();
                    Thread.Sleep(1000);
                    //if (MainConnection.ObdDevice.LogDeviceType == LoggingDevType.J2534)
                    {
                        //MainConnection.ObdDevice.SetReadTimeout(1000);
                        DateTime startTime1 = DateTime.Now;
                        do
                        {
                            resp = MainConnection.ObdDevice.ReceiveMessage(false);
                            //if (resp != null && resp.Length > 6 && resp[2] == DeviceId.Pcm && resp[3] == 0x52)
                            if (ValidateQueryResponse(resp, expectedSrc) && resp[offset + 3] == 0x52)
                            {
                                retVal.Add(resp);
                                Debug.WriteLine("Response: " + resp.ToString());
                                Application.DoEvents();
                            }
                            if (DateTime.Now.Subtract(startTime1) > TimeSpan.FromMilliseconds(300))
                            {
                                Debug.WriteLine("Timeout waiting for null message");
                                break;
                            }
                        } while (resp != null);
                        //MainConnection.ObdDevice.SetReadTimeout(AppSettings.TimeoutJConsoleReceive);
                    }
                }
                Logger(" [Done]");
                Thread.Sleep(100);
                Logger("Waiting answer...", false);
                DateTime startTime2 = DateTime.Now;
                do
                {
                    resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    //if (resp != null && resp.Length > 6 && resp[2] == DeviceId.Pcm && resp[3] == 0x52)
                    if (ValidateQueryResponse(resp, expectedSrc) && resp[offset + 3] == 0x52)
                    {
                        retVal.Add(resp);
                        Debug.WriteLine("Response: " + resp.ToString());
                    }
                    Application.DoEvents();
                    if (DateTime.Now.Subtract(startTime2) > TimeSpan.FromMilliseconds(300))
                    {
                        Debug.WriteLine("Timeout waiting for null message");
                        break;
                    }
                } while (resp != null);
                Logger(" [Done]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
                Receiver.SetReceiverPaused(false);
                return new Response<List<OBDMessage>>(ResponseStatus.Error, retVal);
            }
            Receiver.SetReceiverPaused(false);
            return new Response<List<OBDMessage>>(ResponseStatus.Success, retVal);
        }

        public void ClearTroubleCodes(CANDevice canDev)
        {
            try
            {
                Receiver.SetReceiverPaused(true);
                string moduleStr = canDev.ModuleName;
                Logger("Clearing codes of: " + moduleStr);
                //OBDMessage msg = new OBDMessage(new byte[] { Priority.Functional0, 0x6A, DeviceId.Tool, Mode.ClearDiagnosticTroubleCodes });
                OBDMessage msg;
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                    msg = new OBDMessage(new byte[] { Priority.Physical0, canDev.ModuleID, DeviceId.Tool, 0x14 });
                else
                    msg = new OBDMessage(new byte[] { 0x00, 0x00, (byte)(canDev.DiagID >> 8), (byte)canDev.DiagID, 0x04 });
                MainConnection.ObdDevice.SendMessage(msg, 1);
                OBDMessage resp = MainConnection.ObdDevice.ReceiveMessage(true);
                if (resp != null)
                    Debug.WriteLine("Clear DTC response: " + resp.ToString());
                Logger("Done");
            }
            catch (Exception ex)
            {
                LoggerBold("ClearTroubleCodes:" + ex.Message);
            }
            Receiver.SetReceiverPaused(false);
        }


        public  bool SetMode1()
        {
            try
            {
                Debug.WriteLine("Set mode to 1");
                byte[] Msg = { priority, DeviceId.Pcm, DeviceId.Tool, 0x01, 0x01 };
                bool m = MainConnection.ObdDevice.SendMessage(new OBDMessage(Msg),1);
                if (!m)
                {
                    Logger("Unable to set mode 1");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(Msg, b => b.ToString("X2"))));
                    return false;
                }
                while (MainConnection.ObdDevice.ReceivedMessageCount > 0)
                {
                    OBDMessage resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    Debug.WriteLine("Response: " + resp.ToString());
                }
                Thread.Sleep(50);
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                return false;
            }
        }


        /*
                public  bool InitalizeDevice(string comport, string devtype, bool ftdi, int BaudRate)
                {
                    try
                    {                
                        PcmDevice = CreateSerialDevice(comport, devtype, ftdi);
                        if (PcmDevice.Initialize(BaudRate))
                        {
                            return true;
                        }
                        Thread.Sleep(50);
                    }
                    catch (Exception ex)
                    {
                        LoggerBold(ex.Message);
                    }
                    if (PcmDevice != null)
                    {
                        PcmDevice.Dispose();
                    }
                    return false;
                }

        */
        /*        public  bool InitalizeJDevice(string devtype)
                {
                    try
                    {
                        J2534DotNet.J2534Device dev = J2534DeviceFinder.FindInstalledJ2534DLLs().Where(x => x.Name == devtype).FirstOrDefault();
                        PcmDevice = new J2534Device(dev);
                        if (PcmDevice.Initialize(0))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        LoggerBold(ex.Message);
                    }
                    return false;
                }
        */
        public void StopLogging()
        {
            logWriterTokenSource.Cancel();
            logTokenSource.Cancel();
            Application.DoEvents();
            if (passive)
            {
                if (MainConnection.ObdDevice.LogDeviceType != LoggingDevType.J2534 )
                {
                    //DateTime startWait = DateTime.Now;
                    //while (DateTime.Now.Subtract(lastPresent).TotalMilliseconds< 5000)
                    Debug.WriteLine("Waiting for log loop to end");
                    while (LogRunning)
                    {
                        Thread.Sleep(100);
                        Application.DoEvents();
                    }
                    Debug.WriteLine("Logging loop finished");
                }
            }
            if (MainConnection.LoggingProto != LoggingProtocol.VPW)
            {
                slothandler.ResetFilters();
            }
        }

        public bool StartLogging()
        {
            try
            {
                ReceivedBytes = 0;
                if (MainConnection.ObdDevice == null)
                {
                    LoggerBold("Connection failed");
                    return false;
                }
                if (Receiver.ReceiveLoopRunning) // && MainConnection.ObdDevice.LogDeviceType != LoggingDevType.J2534)
                {
                    Receiver.StopReceiveLoop();
                }

                MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging1);

                if (AnalyzerRunning == false && MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    if (!MainConnection.ObdDevice.SetLoggingFilter())
                        return false;
                }
/*                if (!UseVPW)
                {
                   slothandler.ResetFilters();
                }
*/
                Logger("Pid setup...");
                Application.DoEvents();
                if (!slothandler.CreatePidSetupMessages(SelectedPids))
                {
                    return false;
                }
                Logger("Requesting pids...");
                if (!RequestFirstSlots())
                {
                    return false;
                }
                if (MainConnection.LoggingProto != LoggingProtocol.VPW)
                {
                    slothandler.SetupPassivePidFilters();
                }
                LastCalculatedValues = new double[SelectedPids.Count];
                LastCalculatedStringValues = new string[SelectedPids.Count];
                logTokenSource = new CancellationTokenSource();
                logToken = logTokenSource.Token;
                LogDataBuffer = new List<LogData>();
                logTask = Task.Factory.StartNew(() => DataLoggingLoop(), logToken);
                if (writelog)
                {
                    logWriterTokenSource = new CancellationTokenSource();
                    logWriterToken = logWriterTokenSource.Token;
                    logWriterTask = Task.Factory.StartNew(() => LogWriterLoop(), logWriterToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                logwriter = null;
                MainConnection.ObdDevice.Dispose();
                MainConnection.ObdDevice = null;
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, StartLogging line " + line + ": " + ex.Message);
                return false;
            }
        }

        private bool RequestFirstSlots()
        {
            AllSlotsRequested = false;
            if (MainConnection.LoggingProto == LoggingProtocol.VPW)
            {
                if (Responsetype == ResponseType.SendOnce)
                {
                    passive = false;
                    maxSlotsPerMessage = 6;
                    maxSlotsPerRequest = 6;
                }
                else
                {
                    passive = true;
                    maxSlotsPerMessage = 4;
                    maxSlotsPerRequest = 4;
                }
                if (Responsetype == ResponseType.SendFast)
                {
                    Responsetype = ResponseType.SendMedium2;    //VPW Can't start with 0x24
                }
                if (QueryDevicesOnBus(false).Status != ResponseStatus.Success)
                    return false;
                if (!SetBusQuiet())
                    return false;
                if (!SetBusQuiet())
                    return false;
            }
            else
            {
                if (Responsetype == ResponseType.SendOnce)
                {
                    maxSlotsPerRequest = 5;
                    maxSlotsPerMessage = 5;
                    passive = false;
                }
                else
                {
                    maxSlotsPerRequest = 16;
                    maxSlotsPerMessage = 16;
                    passive = true;
                }
            }

            Application.DoEvents();

            if (passive)
            {
                lastPresent = DateTime.Now;
                //elmStopTreshold = 1000;
                MainConnection.ObdDevice.SetTimeout(AppSettings.TimeoutLoggingPassive);
                MainConnection.ObdDevice.ClearLogQueue();
                if (!RequestPassiveModeSlots(true))
                {
                    LoggerBold("Error requesting Slots");
                    return false;
                }
            }
            else
            {
                //elmStopTreshold = 50;
                if (MainConnection.ObdDevice.LogDeviceType == LoggingDevType.Obdlink)
                {
                    MainConnection.ObdDevice.SetTimeout(AppSettings.TimeoutLoggingActiveObdlink);
                }
                else
                {
                    MainConnection.ObdDevice.SetTimeout(AppSettings.TimeoutLoggingActive);
                }
                MainConnection.ObdDevice.ClearLogQueue();
                RequestNextSlots(true);
            }
            return true;
        }

        public bool RequestNextSlots(bool FirstRequest)
        {
            if (slothandler.Slots.Count > 0)
            {
                OBDMessage msg;
                byte[] rq = slothandler.CreateNextSlotRequestMessage(FirstRequest);
                msg = new OBDMessage(rq);
                Debug.WriteLine("Requesting Slots: " + msg.ToString());
                bool resp = MainConnection.ObdDevice.SendMessage(msg, -maxSlotsPerRequest);
                Thread.Sleep(100);
                Application.DoEvents();
                return resp;
            }
            else
            {
                return false;
            }
        }


        public  string QueryPcmOS()
        {
            try
            {
                Receiver.SetReceiverPaused(true);
                Debug.WriteLine("OS?");
                //MainConnection.ObdDevice.ClearMessageBuffer();
                MainConnection.ObdDevice.ClearMessageQueue();
                byte[] queryMsg;
                uint pos;
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    queryMsg = new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.ReadBlock, 0x0A }; //6C 10 F0 3C 0A
                    pos = 5;
                }
                else
                {
                    queryMsg = new byte[] { 0x00, 0x00, CanDevice.RequestByte1, CanDevice.RequestByte2, 0x1A, 0xC1 };
                    pos = 6;
                }
                bool m = MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), 1);
                if (!m)
                {
                    //Logger("No respond to OS Query message");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                    return null;
                }
                Thread.Sleep(30);
                OBDMessage resp = MainConnection.ObdDevice.ReceiveMessage(true);
                DateTime startTime = DateTime.Now;
                while (true)
                {
                    if (resp != null && resp.Length > 5)
                    {
                        if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                        {
                            if (resp[1] == DeviceId.Tool && resp[2] == DeviceId.Pcm)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (resp[2] == CanDevice.RequestByte1 && resp[3] == CanDevice.RequestByte2 && resp[4] == 0x5A)
                            {
                                break;
                            }
                            if (resp[2] == CanDevice.ResByte1 && resp[3] == CanDevice.ResByte2 && resp[4] == 0x5A)
                            {
                                break;
                            }
                        }
                    }
                    if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(1000))
                    {
                        LoggerBold("Timeout requesting OS");
                        return null;
                    }
                    resp = MainConnection.ObdDevice.ReceiveMessage(true);
                }
                uint os = ReadUint32(resp.GetBytes(), pos, true);
                Debug.WriteLine("Response: " + resp.ToString());
                OS = os.ToString();
                Receiver.SetReceiverPaused(false);
                return os.ToString();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("QyeryPcmOS: " + ex.Message);
                Receiver.SetReceiverPaused(false);
                return null;
            }
        }

        public string QueryVIN(CANDevice canDev)
        {
            string vin = "";
            try
            {
                Debug.WriteLine("VIN?");
                Receiver.SetReceiverPaused(true);
                MainConnection.ObdDevice.ClearMessageQueue();
                //MainConnection.ObdDevice.ClearMessageBuffer();
                byte[] vinbytes = new byte[3*6];
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    for (byte v = 0; v < 3; v++)
                    {
                        MainConnection.ObdDevice.ReceiveBufferedMessages();
                        byte[] queryMsg = { Priority.Physical0, canDev.ModuleID, DeviceId.Tool, Mode.ReadBlock, (byte)(v + 1) };
                        bool m = MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), 1);
                        if (!m)
                        {
                            Logger("No response to VIN Query message");
                            Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                            Receiver.SetReceiverPaused(false);
                            return "";
                        }
                        //Thread.Sleep(100);
                        Application.DoEvents();
                        OBDMessage resp;
                        DateTime startTime = DateTime.Now;
                        while (true)
                        {
                            resp = MainConnection.ObdDevice.ReceiveMessage(true);
                            if (resp == null)
                                Debug.WriteLine("Null msg");
                            else
                                Debug.WriteLine("Msg: " + resp.ToString());
                            if (resp != null && resp.Length > 10 && resp[1] == DeviceId.Tool && resp[2] == canDev.ModuleID && resp[3] == 0x7C && resp[4] == v + 1)
                            {
                                Array.Copy(resp.GetBytes(), 5, vinbytes, v * 6, 6);
                                Debug.WriteLine("Response: " + resp.ToString());
                                break;
                            }
                            if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(1000))
                            {
                                LoggerBold("Timeout receiving VIN code");
                                Receiver.SetReceiverPaused(false);
                                return "";
                            }
                        } 
                    }
                }
                else //CAN
                {
                    byte[] queryMsg = { 0x00, 0x00, canDev.RequestByte1, canDev.RequestByte2, 0x1A, 0x90 };
                    bool m = MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), 1);
                    if (!m)
                    {
                        Logger("No response to VIN Query message");
                        Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                        Receiver.SetReceiverPaused(false);
                        return "";
                    }
                    Thread.Sleep(100);
                    OBDMessage resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    DateTime startTime = DateTime.Now;
                    while (true)
                    {
                        if (resp != null && resp.Length >= 23 && ValidateQueryResponse(resp, (ushort)canDev.ResID)) // resp[2] == CanPcmAddrByte1 && resp[3] == 0xE8)
                        {                            
                            Array.Copy(resp.GetBytes(), 6, vinbytes, 1, 17);
                            break;
                        }
                        if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(1000))
                        {
                            LoggerBold("Timeout requesting VIN");
                            return "";
                        }
                        resp = MainConnection.ObdDevice.ReceiveMessage(true);
                    }
                }
                Receiver.SetReceiverPaused(false);
                vin = Encoding.ASCII.GetString(vinbytes, 1, 17);
                vin = Regex.Replace(vin, "[^a-zA-Z0-9]", "?");
                Logger("VIN Code:" + vin);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("QueryVIN: " + ex.Message);
            }
            Receiver.SetReceiverPaused(false);
            return vin;
        }

        public DTCCodeStatus DecodeDTCstatus(byte[] msg, CANDevice module)
        {
            DTCCodeStatus dcs = new DTCCodeStatus();
            try
            {
                ushort dtc;
                ushort stat;
                if (module.Subnet == LoggingProtocol.VPW)
                {
                    dtc = ReadUint16(msg, 4, true);
                    stat = msg[6];
                }
                else
                {
                    dtc = ReadUint16(msg, 5, true);
                    stat = ReadUint16(msg,7,true);
                }
                //if (stat > 0)
                {
                    string code = DtcSearch.DecodeDTC(dtc.ToString("X4"));
                    dcs.Code = code;
                    dcs.Module = module.ModuleName;
                    OBD2Code descr = OBD2Codes.Where(x => x.Code == code).FirstOrDefault();
                    if (descr != null)
                        dcs.Description = descr.Description;
                    dcs.Status = "";
                    if (stat != 0xFF)
                    {
                        if ((stat & 0x80) == 0x80)
                        {
                            dcs.Status += "MIL ILLUMINATED,";
                        }
                        if ((stat & 0x40) == 0x80)
                        {
                            dcs.Status += "MIL PENDING,";
                        }
                        if ((stat & 0x20) == 0x80)
                        {
                            dcs.Status += "MIL PREVIOUSLY ILLUMINATED -OLD CODE,";
                        }
                        if ((stat & 0x10) == 0x80)
                        {
                            dcs.Status += "STORED TROUBLE CODE (FREEZE FRAME DATA AVAILABLE),";
                        }
                        if ((stat & 0x08) == 0x80)
                        {
                            dcs.Status += "GM SPECIFIC STATUS 1,";
                        }
                        if ((stat & 0x80) == 0x04)
                        {
                            dcs.Status += "GM SPECIFIC STATUS 0,";
                        }
                        if ((stat & 0x80) == 0x02)
                        {
                            dcs.Status += "CURRENT DTC CODE,";
                        }
                        if ((stat & 0x80) == 0x01)
                        {
                            dcs.Status += "IMMATURE DTC CODE,";
                        }
                        dcs.Status = dcs.Status.Trim(',');
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
                Debug.WriteLine("Error, DecodeDTCstatus line " + line + ": " + ex.Message);
            }

            return dcs;
        }

        public void QueueCustomCmd(OBDMessage Msg, string Description)
        {
            Logger("Adding message to queue");
            QueuedCommand command = new QueuedCommand();
            command.Cmd = QueueCmd.Custom;
            command.CustomMsg = Msg;
            command.Description = Description;
            lock (queuedCommands)
            {
                queuedCommands.Enqueue(command);
            }
        }

        public void QueueDtcRequest(CANDevice module, bool history, int custom = -1)
        {
            Logger("Adding DTC request to queue");
            QueuedCommand command = new QueuedCommand();
            command.Cmd = QueueCmd.Getdtc;
            command.module = module;
            command.customMode = custom;
            command.history = history;
            lock(queuedCommands)
            {
                queuedCommands.Enqueue(command);
            }
        }
        public void QueueVINRequest(CANDevice module)
        {
            Logger("Adding VIN request to queue");
            QueuedCommand command = new QueuedCommand();
            command.module = module;
            command.Cmd = QueueCmd.GetVin;
            lock (queuedCommands)
            {
                queuedCommands.Enqueue(command);
            }
        }
        public void QueuePidQuery(LogParam.PidSettings pidSettings )
        {
            QueuedCommand command = new QueuedCommand();
            command.Cmd = QueueCmd.QueryPid;
            command.PidSettings = pidSettings;
            lock (queuedCommands)
            {
                queuedCommands.Enqueue(command);
            }
        }

        public bool RequestDTCCodes(CANDevice module, bool history, bool WaitAnswer, int custom = -1)
        {
            bool retVal = true;
            try
            {
                OBDMessage msg;
                module.DTCCodes.Clear();
                int[] filterids1 = new int[0];
                int[] filterids2 = new int[0];
                if (module.Subnet == LoggingProtocol.VPW)
                {
                    byte mode = 2;
                    if (history)
                        mode = 0x10;
                    if (custom > -1)
                        mode = (byte)custom;
                    msg = new OBDMessage(new byte[] { Priority.Physical0, (byte)module.RequestID, DeviceId.Tool, 0x19, mode, 0xFF, 0x00 });
                }
                else
                {
                    /*
                    if (module.RequestID != CanDevice.RequestID)
                    {
                        AddFilterForDevice(MainConnection.ObdDevice, module, ref filterids1, ref filterids2);
                    }
                    */
                    byte mode = 0x1A;
                    if (history)
                        mode = 0x11;
                    if (custom > -1)
                        mode = (byte)custom;
                    msg = new OBDMessage(new byte[] { 0x00,0x00,module.RequestByte1, module.RequestByte2, 0xA9, 0x81, mode });
                }
                Logger("Requesting DTC codes for " + module.ModuleName);
                Receiver.SetReceiverPaused(true);
                bool m = MainConnection.ObdDevice.SendMessage(msg, -500);
                if (!m)
                {
                    Debug.WriteLine("Error sending request, continue anyway");
                    //return false;
                }
                if (WaitAnswer)
                {
                    Thread.Sleep(100);
                    Debug.WriteLine("Receiving DTC codes...");
                    //byte[] endframe = new byte[] { Priority.Physical0, DeviceId.Tool, module, 0x59, 0x00, 0x00, 0xFF };
                    DateTime startTime = DateTime.Now;
                    OBDMessage resp;
                    do
                    {
                        resp = MainConnection.ObdDevice.ReceiveMessage(true);
                        if (resp != null)
                        {
                            Debug.WriteLine("DTC received message: " + resp.ToString());
                            if (module.Subnet == LoggingProtocol.VPW)
                            {
                                if (resp.Length > 6 && resp[1] == DeviceId.Tool && resp[3] == 0x59)
                                {
                                    DTCCodeStatus dcs = datalogger.DecodeDTCstatus(resp.GetBytes(), module);
                                    module.DTCCodes.Add(dcs);
                                }
                            }
                            else //CAN
                            {
                                //ushort module = Convert.ToUInt16(comboModule.SelectedValue);
                                if (datalogger.ValidateQueryResponse(resp, (ushort)module.RequestID) && resp[4] == 0x81)
                                {
                                    DTCCodeStatus dcs = datalogger.DecodeDTCstatus(resp.GetBytes(), module);
                                    module.DTCCodes.Add(dcs);
                                }
                            }
                        }
                        if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(1000))
                        {
                            Debug.WriteLine("Timeout waiting for null message");
                            break;
                        }
                    } while (resp != null);
                }
                MainConnection.ObdDevice.RemoveFilters(filterids1);
                MainConnection.ObdDevice.RemoveFilters2(filterids2);
                Logger("Done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, RequestDTCCodes line " + line + ": " + ex.Message);
                retVal = false;
                
            }
            Receiver.SetReceiverPaused(false);
            return retVal;
        }


        public  bool SendTesterPresent(bool force)
        {
            try
            {
                if (force || DateTime.Now.Subtract(lastPresent) >= TimeSpan.FromMilliseconds(AppSettings.LoggerTesterPresentInterval)) //4500
                {
                    Debug.WriteLine("Seconds since last testerpresent: " + DateTime.Now.Subtract(lastPresent));
                    {
                        byte[] presentMsg;
                        if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                            presentMsg = new byte[]{ priority, DeviceId.Broadcast, DeviceId.Tool, 0x3F };
                        else
                            presentMsg = new byte[] { 0x00, 0x00, CanDevice.RequestByte1, CanDevice.RequestByte2, 0x3e };
                        MainConnection.ObdDevice.SendMessage(new OBDMessage(presentMsg), -maxSlotsPerRequest);
                        lastPresent = DateTime.Now;
                        Debug.WriteLine("Sent Tester present, force: " + force);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                return false;
            }
            return true;
        }

        public ReadValue ParseSinglePidMessage(OBDMessage msg, LogParam.PidParameter parm)
        {
            ReadValue retVal = new ReadValue();
            try
            {
                int offset = 0;
                if (MainConnection.LoggingProto != LoggingProtocol.VPW)
                {
                    offset = 1; //CAN protocol have 1 more header byte
                }
                if (msg.Length > offset + 5)
                {
                    Debug.WriteLine("Parsing message: " + msg.ToString());
                    int pos;
                    ReadValue rv = new ReadValue();
                    if (msg[offset + 3] == 0x7f) 
                    {
                        if (msg.GetBytes().Last() == 0x31) //31 = Out of range
                        {
                            Debug.WriteLine("Unsupported pid");
                        }
                        else if (msg.GetBytes().Last() == 0x23) //23 = Routine not ready
                        {
                            Debug.WriteLine("Routine not ready, retry");
                        }
                        rv.FailureCode = msg.GetBytes().Last();
                        return rv;
                    }
                    else if (msg[offset + 3] == 0x62)
                    {
                        rv.PidNr = ReadUint16(msg.GetBytes(), (uint)(offset + 4), true);
                    }
                    else if (msg[offset + 3] == 0x63)
                    {
                        byte[] tmp = new byte[4];
                        Array.Copy(msg.GetBytes(), offset + 4, tmp, 2, 2);
                        tmp[1] = 0xFF;
                        rv.PidNr = (int)ReadUint32(tmp, 0, true);
                    }
                    if (rv.PidNr != parm.Address)
                    {
                        Debug.WriteLine("Wrong pid, or other message");
                        return rv;
                    }
                    pos = offset + 6;
                    double val = 0;
                    int elementSize = parm.GetElementSize();
                    //Fix datatype:
                    if (msg.Length != pos + elementSize)
                    {
                        LogParam.ProfileDataType origType = parm.DataType;
                        string logStr = "Fixing pid "+ parm.Id + " datatype, original: " + parm.DataType.ToString();
                        int realSize = msg.Length - pos - elementSize + 1;
                        Debug.WriteLine("Pid real size: " + realSize.ToString());
                        if (parm.DataType == LogParam.ProfileDataType.UBYTE || 
                            parm.DataType == LogParam.ProfileDataType.UWORD || 
                            parm.DataType == LogParam.ProfileDataType.UINT32)
                        {
                            switch(realSize)
                            {
                                case 1:
                                    parm.DataType = LogParam.ProfileDataType.UBYTE;
                                    break;
                                case 2:
                                    parm.DataType = LogParam.ProfileDataType.UWORD;
                                    break;
                                case 3:
                                    parm.DataType = LogParam.ProfileDataType.UWORD;
                                    break;
                                case 4:
                                    parm.DataType = LogParam.ProfileDataType.UINT32;
                                    break;
                                default:
                                    Debug.WriteLine("Something wrong, pid size can't be " + realSize.ToString());
                                    return rv;
                            }
                        }
                        else
                        {
                            switch (realSize)
                            {
                                case 1:
                                    parm.DataType = LogParam.ProfileDataType.SBYTE;
                                    break;
                                case 2:
                                    parm.DataType = LogParam.ProfileDataType.SWORD;
                                    break;
                                case 3:
                                    parm.DataType = LogParam.ProfileDataType.SWORD;
                                    break;
                                case 4:
                                    parm.DataType = LogParam.ProfileDataType.INT32;
                                    break;
                                default:
                                    Debug.WriteLine("Something wrong, pid size can't be " + realSize.ToString());
                                    return rv;
                            }

                        }
                        if (origType != parm.DataType)
                        {
                            logStr += ", new: " + parm.DataType.ToString();
                            Logger(logStr);
                        }
                    }
                    switch (parm.DataType)
                    {
                        case LogParam.ProfileDataType.UBYTE:
                            val = (byte)msg[pos];
                            break;
                        case LogParam.ProfileDataType.SBYTE:
                            val = (sbyte)msg[pos];
                            break;
                        case LogParam.ProfileDataType.UWORD :
                            if (msg.Length <= pos + 1)
                            {
                                rv.FailureCode = 0x22;
                            }
                            else
                            {
                                val = ReadUint16(msg.GetBytes(), (uint)pos, true);
                            }
                            break;
                        case LogParam.ProfileDataType.SWORD:
                            if (msg.Length <= pos + 1)
                            {
                                rv.FailureCode = 0x22;
                            }
                            else
                            {
                                val = ReadInt16(msg.GetBytes(), (uint)pos, true);
                            }
                            break;
                        case LogParam.ProfileDataType.UINT32:
                            if (msg.Length <= pos + 3)
                            {
                                rv.FailureCode = 0x22;
                            }
                            else
                            {
                                val = ReadUint32(msg.GetBytes(), (uint)pos, true);
                            }
                            break;
                        case LogParam.ProfileDataType.INT32:
                            if (msg.Length <= pos + 3)
                            {
                                rv.FailureCode = 0x22;
                            }
                            else
                            {
                                val = ReadInt32(msg.GetBytes(), (uint)pos, true);
                            }
                            break;
                    }
                    rv.PidValue = val;
                    Debug.WriteLine("Pid: " + rv.PidNr.ToString("X4") + ", Value: " + val);
                    return rv;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, ParseSinglePidMessage line " + line + ": " + ex.Message);
            }
            return new ReadValue();
        }

        
        private bool RequestPassiveModeSlots(bool FirstRequest)
        {
            lastPresent = DateTime.Now;
            if (!RequestNextSlots(FirstRequest))
                return false;
            if (MainConnection.LoggingProto == LoggingProtocol.VPW)
            {
                if (Responsetype == ResponseType.SendFast)
                {
                    Responsetype = ResponseType.SendMedium2;
                }
                else if (Responsetype == ResponseType.SendMedium2)
                {
                    Responsetype = ResponseType.SendFast;
                }
                if (MainConnection.ObdDevice.LogDeviceType != LoggingDevType.Elm && MainConnection.ObdDevice.LogDeviceType != LoggingDevType.Obdlink) //If Elm device, need to wait for prompt
                {
                    Thread.Sleep(100);
                    if (!RequestNextSlots(FirstRequest))
                        return false;
                }
            }
            return true;
        }

        public void ELMPromptReceived()
        {
            Debug.WriteLine("Elm prompt received");

            if (logToken.IsCancellationRequested)
            {
                SetBusQuiet();
                return;
            }

            SendQueuedCommand();

            if (passive)
            {
                //if (DateTime.Now.Subtract(lastPresent) < TimeSpan.FromMilliseconds(4500) &&  DateTime.Now.Subtract(lastElmPrompt) < TimeSpan.FromSeconds(1))
                  //  return;

                if (!AllSlotsRequested)
                {
                    //MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging4);
                    if (!RequestPassiveModeSlots(false))
                    {
                        LoggerBold("Error requesting Slots");
                        StopLogging();
                        return;
                    }
                    AllSlotsRequested = true;
                }
                else
                {
                    SendTesterPresent(true);
                }
            }
            else
            {
                RequestNextSlots(false);
            }

        }

        public void StopElmReceive()
        {
            try
            {
                if (MainConnection.ObdDevice.LogDeviceType == LoggingDevType.Elm || MainConnection.ObdDevice.LogDeviceType != LoggingDevType.Obdlink)
                {
                    Debug.WriteLine("Time (ms) since last elmStop: " + DateTime.Now.Subtract(lastElmStop).TotalMilliseconds);
                    //if (force || DateTime.Now.Subtract(lastPresent) >= TimeSpan.FromMilliseconds(4500) || DateTime.Now.Subtract(lastElmStop) >= TimeSpan.FromMilliseconds(elmStopTreshold))
                    {
                        //Stop current receive
                        Debug.WriteLine("Stop elm receive");
                        MainConnection.port.Send(Encoding.ASCII.GetBytes("X \r"));
/*                        if (MainConnection.ObdDevice.LogDeviceType == LoggingDevType.Elm)
                        {
                            Thread.Sleep(50);
                            byte[] queryMsg = { Priority.Physical0, 0x3, DeviceId.Tool, 0x3f };
                            MainConnection.ObdDevice.SendMessage(new OBDMessage(queryMsg), 1);
                        }
                        Thread.Sleep(200);
*/                        //Thread.Sleep(50);
                        OBDMessage msg = MainConnection.ObdDevice.ReceiveMessage(true);
                        while (msg != null && !msg.ElmPrompt)
                        {
                            Debug.WriteLine("Elm stop received msg: " + msg.ToString() + ", ElmPrompt: " + msg.ElmPrompt);
                            msg = MainConnection.ObdDevice.ReceiveMessage(true);
                        }
                        lastElmStop = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("StopElmReceive: " + ex.Message);
            }
        }

        private  bool ValidateLogMessage(OBDMessage oMsg, ref int SlotCount)
        {
            try
            {
                byte lastSlot = 0;
                if (slothandler.Slots.Count > 0)
                {
                    lastSlot = slothandler.Slots.Last().Id;
                }
                int bytePos;
                if (MainConnection.LoggingProto == LoggingProtocol.VPW)
                {
                    if (!Utility.CompareArraysPart(oMsg.GetBytes(), new byte[] { priority, DeviceId.Tool, DeviceId.Pcm }))
                    {
                        Debug.WriteLine("Unknown msg");
                        return false;
                    }
                    if (oMsg.Length < 11)
                    {
                        Debug.WriteLine("Short msg");
                        return false;
                    }
                    byte slot = oMsg[4];
                    if ((reverseSlotNumbers && slot < lastSlot) || (!reverseSlotNumbers && slot > lastSlot))    //It's not slot. Maybe 2A ?
                    {
                        return false;
                    }
                    bytePos = 3;
                }
                else
                {
                    if (oMsg.Length < 8)
                    {
                        Debug.WriteLine("Short msg");
                        return false;
                    }
                    int src = ReadInt32(oMsg.Data, 0, true);
                    if (slothandler.PassiveSources.Contains(src))
                    {
                        return true;
                    }
                    ushort msgSrc = (ushort)(oMsg[2] << 8 | oMsg[3]);
                    if (oMsg[0] != 0x00 || oMsg[1] != 0x00 || msgSrc != CanDevice.DiagID) // || oMsg[3] != 0xE8)
                    {
                        Debug.WriteLine("Unknown msg");
                        return false;
                    }
                    bytePos = 4;
                }


                byte PcmResponse = oMsg[bytePos];
                if (PcmResponse == 0x7F)
                {
                    Debug.WriteLine(oMsg.ToString() + ": " + PcmResponses[oMsg.GetBytes().Last()]);
                    SlotCount++;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, DataLogger line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
                return false;
            }                
        }

        public bool ValidateQueryResponse(OBDMessage oMsg, ushort ExpectedSrc)
        {
            if (oMsg == null)
            {
                return false;
            }
            //Debug.WriteLine("Validating message: " + oMsg.ToString());
            if (MainConnection.LoggingProto == LoggingProtocol.VPW)
            {
                if (oMsg.Length < 3)
                {
                    //Debug.WriteLine("Short message");
                    return false;
                }
                //priority, DeviceId.Tool, DeviceId.Pcm
                byte eSrc = (byte)(ExpectedSrc >> 8);
                byte eDst = (byte)(ExpectedSrc & 0xFF);
                if (eSrc != oMsg[1])
                {
                    Debug.WriteLine("ValidateQueryResponse: wrong source: " + oMsg[1].ToString("X2"));
                    return false;
                }
                if (eDst != oMsg[2])
                {
                    Debug.WriteLine("ValidateQueryResponse: wrong destination: " + oMsg[2].ToString("X2"));
                    return false;
                }
            }
            else
            {
                if (oMsg.Length < 5)
                {
                    //Debug.WriteLine("Short msg");
                    return false;
                }
                if (oMsg[0] != 0 || oMsg[1] != 0)
                {
                    //Debug.WriteLine("Unknown msg");
                    return false;
                }
                CANDevice cd = CANQuery.GetDeviceAddresses(ExpectedSrc);
                ushort msgSrc = (ushort)(oMsg[2] << 8 | oMsg[3]);
                if (msgSrc != cd.DiagID && msgSrc != cd.RequestID && msgSrc != cd.ResID) 
                {
                    //Debug.WriteLine("Unknown msg");
                    return false;
                }
            }

            return true;
        }
        public void LoadLogFile(string LogFile)
        {
            try
            {
                Logger("Loading file: " + LogFile);
                LogDataBuffer = new List<LogData>();
                DateTime startTime = DateTime.MinValue;
                DateTime prevTStamp = DateTime.MinValue;
                StreamReader sr = new StreamReader(LogFile);
                string logLine = sr.ReadLine();
                int tStamps = LoadProfileFromCsv(LogFile);
                int elapsedIndex = -1;
                string[] hdrArray = logLine.Split(new string[] { AppSettings.LoggerLogSeparator }, StringSplitOptions.None);
                for (int x = 0; x < hdrArray.Length; x++)
                {
                    if (hdrArray[x].ToLower().Contains("elapsed time"))
                    {
                        elapsedIndex = x;
                        break;
                    }
                }
 /*               for (int h=0; h<hdrArray.Length; h++)
                {
                    int count = 1;
                    for (int h2=h+1;h2<hdrArray.Length; h2++)
                    {
                        if (hdrArray[h2] == hdrArray[h])
                        {
                            Logger("Renaming duplicate column: " + hdrArray[h2] + " => ", false);
                            hdrArray[h2] = hdrArray[h] + " #" + count.ToString();
                            Logger(hdrArray[h2]);
                            count++;
                        }
                    }
                }*/
                int faketimestamps = 0;
                int row = 0;
                DateTime fDT = File.GetCreationTime(LogFile);
                string tStampDate = fDT.ToString("yyyy.MM.dd.");
                bool tStampErrorReported = false;
                TimestampFormat TSF = new TimestampFormat(fDT);
                while ((logLine = sr.ReadLine()) != null)
                {
                    string[] lParts = logLine.Split(new string[] { AppSettings.LoggerLogSeparator }, StringSplitOptions.None);
                    if (row < 2 && lParts.Length != hdrArray.Length)
                    {
                        throw new Exception(Environment.NewLine + "Column count don't match header. Check Log separator!");
                    }
                    if (lParts.Length != hdrArray.Length)
                    {
                        Logger("Columns don't match in row " + (row + 1).ToString() + ", skipping");
                        continue;
                    }
                    double val;
                    LogData ld = new LogData(lParts.Length - tStamps);
                    DateTime tStamp = TSF.ConvertTimeStamp(lParts[0]);
                    if (tStamp == fDT && !tStampErrorReported)
                    {
                        tStampErrorReported = true;
                        Logger("Timestamp format not match, Format: " + AppSettings.LoggerTimestampFormat + ", Timestamp: " + lParts[0]);
                    }
                    if (prevTStamp.Ticks >= tStamp.Ticks)
                    {
                        tStamp = prevTStamp.AddMilliseconds(1);
                        faketimestamps++;
                    }
                    prevTStamp = tStamp;
                    ld.TimeStamp = tStamp.Ticks;
                    if (startTime == DateTime.MinValue)
                    {
                        startTime = tStamp;
                    }
                    else if (elapsedIndex > -1)
                    {
                        string elapsedStr = lParts[elapsedIndex];
                        TimeSpan elapsed = TimeSpan.Parse(elapsedStr);
                        tStamp = startTime.Add(elapsed);
                    }
                    for (int r = tStamps; r < lParts.Length; r++)
                    {
                        if (string.IsNullOrEmpty(lParts[r]))
                        {
                            ld.Values[r - tStamps] = double.MinValue;
                        }
                        else
                        {
                            string valStr = lParts[r].Replace(",", ".");
                            if (OffStrings.Contains(valStr.ToLower()))
                                valStr = "0";
                            else if (OnStrings.Contains(valStr.ToLower()))
                                valStr = "1";

                            if (double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                            {
                                ld.Values[r - tStamps] = val;
                            }
                            else
                            {
                                ld.Values[r - tStamps] = double.MinValue;
                            }
                        }
                    }
                    ld.CalculatedValues = ld.Values;
                    LogDataBuffer.Add(ld);
                    row++;
                }
                sr.Close();
                if (faketimestamps > (LogDataBuffer.Count/100))
                {
                    Logger(Environment.NewLine + "Timestamps adjusted, timing may not be accurate (" + faketimestamps.ToString() + " of " + LogDataBuffer.Count.ToString()+")");
                }
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, DataLogger line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
            }
        }

        private bool SendQueuedCommand()
        {
            if (queuedCommands.Count == 0)
            {
                return true;
            }
            while (queuedCommands.Count > 0)
            {
                QueuedCommand command;
                lock (queuedCommands)
                {
                    command = queuedCommands.Dequeue();
                }

                Application.DoEvents();
                MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging3);
                switch (command.Cmd)
                {
                    case QueueCmd.Getdtc:
                        int[] fltrs1 = new int[0];
                        int[] fltrs2 = new int[0];

                        if (command.module.Subnet != LoggingProtocol.VPW)
                        {
                            AddFilterForDevice(MainConnection.ObdDevice, command.module, ref fltrs1, ref fltrs2);
                        }
                        RequestDTCCodes(command.module, command.history, true, command.customMode);
                        if (command.module.Subnet != LoggingProtocol.VPW)
                        {
                            MainConnection.ObdDevice.RemoveFilters(fltrs1);
                            MainConnection.ObdDevice.RemoveFilters2(fltrs2);
                        }

                        break;
                    case QueueCmd.GetVin:
                        QueryVIN(datalogger.CanDevice);
                        break;
                    case QueueCmd.QueryPid:
                        QueryPid(command.PidSettings, true);
                        break;
                    case QueueCmd.Custom:
                        Logger("Sending queued command: " + command.Description);
                        MainConnection.ObdDevice.SendMessage(command.CustomMsg, 1);
                        //MainConnection.ObdDevice.ReceiveMessage(true);
                        break;
                }
            }
            if (passive)
            {
                MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging4);
                //SendTesterPresent(false);
            }
            else
            {
                if (MainConnection.ObdDevice.LogDeviceType != LoggingDevType.Obdlink)
                {
                    MainConnection.ObdDevice.SetTimeout(TimeoutScenario.DataLogging3);
                }

            }
            return true;
        }

        private void RequestMoreData()
        {
            if (passive)
            {
                if (maxSlotsPerRequest < maxPassiveSlotsPerRequest) //Started with fewer Slots
                {
                    if (MainConnection.ObdDevice.LogDeviceType == LoggingDevType.Obdlink)
                    {
                        //Started with 4 Slots, now asking 50 more
                        RequestNextSlots(false);
                    }
                    Debug.WriteLine("Set Slots per request to: " + maxPassiveSlotsPerRequest);
                    maxSlotsPerRequest = maxPassiveSlotsPerRequest;
                }
                SendTesterPresent(false);
            }
            else //SendOnce
            {
                if (!RequestNextSlots(false))    // (??)Works with Obdlink, because we know how many Slots are coming
                    throw new Exception("Error in Slot request");
            }
        }

        public void DataLoggingLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            DateTime LastRecvTime = DateTime.Now;
            AllSlotsRequested = false;
            slothandler.ReceivedRows = 0;
            int NumMessages = 1;
            Logger("Logging started");
            while (!logToken.IsCancellationRequested)
            {
                try
                {
                    List<OBDMessage> oMsgs = new List<OBDMessage>();
                    int SlotCount = 0;
                    int retryCount = 0; //Receiving ok?

                    while ( SlotCount < maxSlotsPerRequest) //Loop there until requested Slots are handled
                    {
                        if (MainConnection.ObdDevice == null || !Connected)
                        {
                            break;
                        }
                        if (logToken.IsCancellationRequested) 
                        {
                            if (MainConnection.ObdDevice.LogDeviceType == LoggingDevType.Obdlink || MainConnection.ObdDevice.LogDeviceType == LoggingDevType.Elm)
                            {
                                //Elm device can stop only when prompt received
                                Debug.WriteLine("Stopping soon, waiting elm prompt");
                            }
                            else
                            {
                                Debug.WriteLine("Stopping...");
                                break;
                            }
                        }
                        if (queuedCommands.Count > 0 && !passive)
                        {
                            SendQueuedCommand();
                        }
                        if (queuedCommands.Count > 0 && MainConnection.LoggingProto != LoggingProtocol.VPW)
                        {
                            Logger("Waiting " + AppSettings.LoggerCANWaitSecondsStopMessages.ToString() + " seconds for PCM to stop sending data");
                            Thread.Sleep(AppSettings.LoggerCANWaitSecondsStopMessages * 1000); //Wait until PCM stop sending data
                            SendQueuedCommand();
                            RequestFirstSlots();
                            LastRecvTime = DateTime.Now;
                        }
                        if (queuedCommands.Count == 0 && passive)
                        {
                            SendTesterPresent(false);
                        }
                        
                        //Receive DATA:
                        oMsgs = MainConnection.ObdDevice.ReceiveLogMessages(NumMessages);
                        if (oMsgs.Count == 0)
                        {
                            //Debug.WriteLine("Received null message");
                            if (queuedCommands.Count > 0 && DateTime.Now.Subtract(LastRecvTime) > TimeSpan.FromMilliseconds(300))
                            {
                                SendQueuedCommand();
                                RequestFirstSlots();
                                LastRecvTime = DateTime.Now;
                            }
                            else if (DateTime.Now.Subtract(LastRecvTime) > TimeSpan.FromSeconds(AppSettings.LoggerRetryAfterSeconds))
                            {
                                if (retryCount < AppSettings.LoggerRetryCount)
                                {
                                    Logger("Data not received in "+AppSettings.LoggerRetryAfterSeconds.ToString()+" seconds, sending new request");
                                    //slothandler.CreatePidSetupMessages(datalogger.SelectedPids);
                                    NumMessages = 1;
                                    RequestFirstSlots();
                                    LastRecvTime = DateTime.Now;
                                    retryCount++;
                                }
                                else
                                {
                                    LoggerBold("No data after retries, giving up");
                                    logTokenSource.Cancel();
                                    break;
                                }
                            }
                            continue;
                        }
                        foreach (OBDMessage oMsg in oMsgs)
                        {
                            Debug.WriteLine("Received: " + oMsg.ToString() + ", Elmprompt: " + oMsg.ElmPrompt + " Slot count: " + SlotCount.ToString());
                            if (logToken.IsCancellationRequested)
                            {
                                break;
                            }
                            if (oMsg.ElmPrompt)
                            {
                                ELMPromptReceived();
                                //continue;
                            }
                            if (!ValidateLogMessage(oMsg, ref SlotCount))
                            {
                                continue;
                            }
                            //We really have received a Slot!
                            SlotCount++;
                            LastRecvTime = DateTime.Now;
                            Debug.WriteLine("Decoding message");
                            slothandler.HandleSlotMessage(oMsg);
                            Application.DoEvents();
                        }
                    } //Inner logloop
                    if (logToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (queuedCommands.Count == 0)
                    {
                        RequestMoreData();
                        NumMessages = AppSettings.LoggerLoggingMessagesPerRequest;
                    }
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    Debug.WriteLine("Error, logLoop line " + line + ": " + ex.Message);
                    if (MainConnection.ObdDevice == null || !Connected)
                    {
                        return;
                    }
                }
            } //Logloop
            Logger("Logging stopped");
            if (MainConnection.ObdDevice != null && Connected)
            {
                MainConnection.ObdDevice.ClearLogQueue();
                SetBusQuiet();
                SetBusQuiet();
                MainConnection.ObdDevice.SetTimeout(TimeoutScenario.Maximum);
                //MainConnection.ObdDevice.SetReadTimeout(AppSettings.TimeoutReceive);
                //MainConnection.ObdDevice.SetWriteTimeout(AppSettings.TimeoutScriptWrite);
            }
            LogRunning = false;
            return;
        }


    }
}
