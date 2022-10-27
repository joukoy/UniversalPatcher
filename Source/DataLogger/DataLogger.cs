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

namespace UniversalPatcher
{
    public class DataLogger
    {
        public DataLogger()
        {
            Receiver = new MessageReceiver();
        }
        private CancellationTokenSource logTokenSource = new CancellationTokenSource();
        private CancellationTokenSource logWriterTokenSource = new CancellationTokenSource();
        private CancellationToken logToken;
        private CancellationToken logWriterToken;
        public Task logTask;
        private Task logWriterTask;
        public IPort port;
        public  bool Connected = false;
        public  List<PidConfig> PidProfile { get; set; }
        private  StreamWriter logwriter;
        private  string logseparator = ";";
        public  Device LogDevice;
        public  int ReceivedBytes = 0;
        public  string OS;
        public  SlotHandler slothandler;
        public MessageReceiver Receiver;

        public bool LogRunning = false;
        public bool AnalyzerRunning = false;
        private  bool AllSlotsRequested = false;
        private  bool passive;
        public  int maxPassiveSlotsPerMsg = 50;
        public  bool stopLogLoop;
        public  byte priority = Priority.Physical0;
        private  DateTime lastPresent = DateTime.Now;
        private  DateTime lastElmStop = DateTime.Now;

        //public  Queue<Analyzer.AnalyzerData> analyzerq = new Queue<Analyzer.AnalyzerData>();
        public  Queue<LogData> LogFileQueue = new Queue<LogData>();
        private Queue<QueuedCommand> queuedCommands = new Queue<QueuedCommand>();


        //Set these values before StartLogging()
        public DateTime LogStartTime;
        public bool writelog;
        public  bool useRawValues;
        public  bool useVPWFilters;
        public  bool reverseSlotNumbers;
        public  byte Responsetype;
        public  int maxSlotsPerRequest = 4;   //How many Slots before asking more
        public  int maxSlotsPerMessage = 4;   //How many Slots in one Slot request message
        public  bool HighPriority = false;

        private enum QueueCmd
        {
            Getdtc,
            GetVin,
            Custom
        }
        private class QueuedCommand
        {
            public QueueCmd Cmd { get; set; }
            public byte param1 { get; set; }
            public byte param2 { get; set; }
            public OBDMessage CustomMsg { get; set; }
            public string Description { get; set; }
        }

        public enum LoggingDevType
        {
            Elm,
            Obdlink,
            Other
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
            public ulong TimeStamp { get; set; }
            public ulong SysTimeStamp { get; set; }
        }

        public class ReadValue
        {
            public ReadValue()
            {
                PidValue = double.MinValue;
            }
            public double PidValue { get; set; }
            public int PidNr { get; set; }
            public ulong TimeStamp { get; set; }
            public ulong SysTimeStamp { get; set; }
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

        public Device CreateSerialDevice(string serialPortName, string serialPortDeviceType, bool ftdi)
        {
            try
            {
                if (ftdi)
                    port = new FTDIPort(serialPortName);
                else
                    port = new Rs232Port(serialPortName);

                Device device;
                switch (serialPortDeviceType)
                {
                    case OBDXProDevice.DeviceType:
                        device = new OBDXProDevice(port);
                        break;

                    case AvtDevice.DeviceType:
                        device = new AvtDevice(port);
                        break;

                    case LegacyElmDeviceImplementation.DeviceType:
                        device = new ElmDevice(port);
                        break;

                    case ElmDevice.DeviceType:
                        device = new ElmDevice(port);
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

        public  bool CreateLog(string path)
        {
            try
            {
                Logger("Writing to logfile: " + path);
                logseparator = Properties.Settings.Default.LoggerLogSeparator;
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
                    if (useRawValues)
                    {
                        List<string> pids = new List<string>();
                        for (int c = 0; c < PidProfile.Count; c++)
                        {
                            if (!pids.Contains(PidProfile[c].Address))
                            {
                                pids.Add(PidProfile[c].Address);
                            }
                        }
                        for (int c=0; c < pids.Count; c++)
                        {
                            string s = pids[c];
                            sb.Append(s);
                            if (c<pids.Count-1)
                                 sb.Append(logseparator);
                        }
                    }
                    else
                    {
                        for (int c = 0; c < PidProfile.Count; c++)
                        {
                            sb.Append(PidProfile[c].PidName);
                            if (c < PidProfile.Count - 1)
                                sb.Append(logseparator);
                        }
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
                if (logwriter != null)
                {
                    string CultureDecim = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string Decim = Properties.Settings.Default.LoggerDecimalSeparator;
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
                LoggerBold("Error, WriteLog line " + line + ": " + ex.Message);
            }
        }


        private  void LogWriterLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            //while (!stopLogLoop)
            while (!logWriterToken.IsCancellationRequested)
            {
                while (LogFileQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    if (stopLogLoop)
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
                ld.CalculatedValues = slothandler.CalculatePidDoubleValues(ld.Values);
                if (useRawValues)
                {
                    string[] ldvalues = new string[ld.Values.Length];
                    for (int l = 0; l < ld.Values.Length; l++)
                        ldvalues[l] = ld.Values[l].ToString();
                    WriteLog(ldvalues, ld.TimeStamp.ToString());

                }
                else
                {
                    string tStamp = new DateTime((long)ld.SysTimeStamp).ToString(Properties.Settings.Default.LoggerTimestampFormat);
                    //tStamp += " [" + ld.TimeStamp.ToString() + "]";
                    WriteLog(slothandler.CalculatePidValues(ld.Values), tStamp );
                }
                //Data for Histogram & Graphics:
                LoggerDataEvents.Add(ld);
            }
            logwriter.Close();
            logwriter = null;
        }

        public  void LoadProfile(string FileName)
        {            
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                PidProfile = (List<PidConfig>)reader.Deserialize(file);
                file.Close();
                Properties.Settings.Default.LoggerLastProfile = FileName;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LoadProfile line " + line + ": " + ex.Message);
            }            
        }

        public  void SaveProfile(string FileName)
        {
            try
            {
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidConfig>));
                    writer.Serialize(stream, PidProfile);
                    stream.Close();
                }
                Properties.Settings.Default.LoggerLastProfile = FileName;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, SaveProfile line " + line + ": " + ex.Message);
            }
        }


        public  ReadValue QuerySinglePidValue(int addr, ProfileDataType dataType)
        {
            ReadValue rv = new ReadValue();
            try
            {
                Receiver.SetReceiverPaused(true);
                OBDMessage request = null;
                if (addr > ushort.MaxValue) //RAM
                {
                    request = new OBDMessage(new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x23, (byte)(addr >> 16), (byte)(addr >> 8), (byte)addr, 0x01 });
                }
                else
                {
                    request = new OBDMessage(new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x22, (byte)(addr >> 8), (byte)addr, 0x01 });
                }
                if (LogDevice.SendMessage(request,1))
                {
                    Thread.Sleep(2);
                    OBDMessage resp = LogDevice.ReceiveMessage();
                    //if (resp.GetBytes()[3] == 0x7f)
                    if (resp == null)
                    {
                        LoggerBold("Pid: " + addr.ToString("X4") + " Error, null response");
                        Receiver.SetReceiverPaused(false);
                        return rv;
                    }

                    while (resp != null)
                    {
                        if (resp.Length > 3 && resp[3] == 0x7f)
                        {
                            string errStr = "Unknown error";
                            int p = resp.Length - 1;
                            if (PcmResponses.ContainsKey(resp[p]))
                                errStr = PcmResponses[resp[p]];
                            LoggerBold("Pid: " + addr.ToString("X4") + " Error: " + errStr);
                            break;
                        }
                        else
                        {
                            rv = ParseSinglePidMessage(resp, dataType);
                            if (rv.PidNr == addr)
                            {
                                break;
                            }
                            else
                            {
                                Debug.WriteLine("Requested pid: " + addr.ToString("X") + ", received: " + rv.PidNr);
                                resp = LogDevice.ReceiveMessage();
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
                LoggerBold("Error, QuerySinglePidValue line " + line + ": " + ex.Message);
            }
            Receiver.SetReceiverPaused(false);
            return rv;
        }


        public bool SetBusQuiet()
        {
            try
            {
                Debug.WriteLine("Set bus quiet");
                byte[] quietMsg = { priority, DeviceId.Broadcast, DeviceId.Tool, Mode.ExitKernel };
                bool m = LogDevice.SendMessage(new OBDMessage(quietMsg), 10);
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
            List<byte> retVal = new List<byte>();
            try
            {
                Debug.WriteLine("Devices on bus?");
                LogDevice.ClearMessageBuffer();
                LogDevice.ClearMessageQueue();
                byte[] queryMsg = { Priority.Physical0High, DeviceId.Broadcast, DeviceId.Tool, 0x20 };
                bool m = LogDevice.SendMessage(new OBDMessage(queryMsg) ,1);
                if (m)
                {
                    //Debug.WriteLine("OK" );
                }
                else
                {
                    Logger("No respond to Query devices message");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                    return new Response<List<byte>>(ResponseStatus.Error,retVal);
                }
                if (waitanswer)
                {
                    Thread.Sleep(10);
                    while (LogDevice.ReceivedMessageCount > 0)
                    {
                        //Just in case there is more data
                        OBDMessage resp = LogDevice.ReceiveMessage();
                        byte module = resp.GetBytes()[2];
                        if (!retVal.Contains(module))
                            retVal.Add(module);
                        Debug.WriteLine("Response: " + resp.ToString());
                    }
                }
                return new Response<List<byte>>(ResponseStatus.Success, retVal);
            }
            catch (Exception ex)
            {
                LoggerBold("Query devices on bus: " + ex.Message);
            }
            return new Response<List<byte>>(ResponseStatus.Error, retVal);
        }

        public  void ClearTroubleCodes(byte module)
        {
            try
            {
                Receiver.SetReceiverPaused(true);
                string moduleStr = module.ToString("X2");
                if (analyzer.PhysAddresses.ContainsKey(module))
                    moduleStr = analyzer.PhysAddresses[module];
                Logger("Clearing codes of: " + moduleStr);
                //OBDMessage msg = new OBDMessage(new byte[] { Priority.Functional0, 0x6A, DeviceId.Tool, Mode.ClearDiagnosticTroubleCodes });
                OBDMessage msg = new OBDMessage(new byte[] { Priority.Physical0, module, DeviceId.Tool, 0x10, 0x00 });
                LogDevice.SendMessage(msg, 1);
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
                bool m = LogDevice.SendMessage(new OBDMessage(Msg),1);
                if (!m)
                {
                    Logger("Unable to set mode 1");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(Msg, b => b.ToString("X2"))));
                    return false;
                }
                while (LogDevice.ReceivedMessageCount > 0)
                {
                    OBDMessage resp = LogDevice.ReceiveMessage();
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
            logTokenSource.Cancel();
            stopLogLoop = true;
            logTask.Wait(300);
        }

        public  bool StartLogging()
        {
            try
            {
                ReceivedBytes = 0;

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
                    Responsetype = ResponseType.SendMedium2;    //Can't start with 0x24
                }

                if (LogDevice == null )
                {
                    LoggerBold("Connection failed");
                    return false;
                }

                if (Receiver.ReceiveLoopRunning)
                {
                    Receiver.StopReceiveLoop();
                }

                LogDevice.SetTimeout(TimeoutScenario.DataLogging1);
                LogDevice.SetReadTimeout(100);

                if (QueryDevicesOnBus(false).Status != ResponseStatus.Success)
                    return false;
                if (!SetBusQuiet())
                    return false;
                if (!SetBusQuiet())
                    return false;
                if (!AnalyzerRunning)
                {
                    if (!LogDevice.SetLoggingFilter())
                        return false;
                }
                //if (!SetMode1())
                //  return false;
                //SetHighSpeedMode(); //Not for logging!
                //Thread.Sleep(100);
                Logger("Pid setup...");
                Application.DoEvents();
                slothandler = new SlotHandler();
                bool resp = slothandler.CreatePidSetupMessages();
                if (!resp)
                {
                    return false;
                }
                Logger("Requesting pids...");
                Application.DoEvents();

                if (passive)
                {
                    lastPresent = DateTime.Now;
                    //elmStopTreshold = 1000;
                    LogDevice.SetTimeout(TimeoutScenario.DataLogging4);
                    if (!RequestPassiveModeSlots())
                    {
                        LoggerBold("Error requesting Slots");
                        return false;
                    }
                }
                else
                {
                    //elmStopTreshold = 50;
                    if (LogDevice.LogDeviceType == LoggingDevType.Obdlink)
                    {
                        LogDevice.SetTimeout(TimeoutScenario.Minimum);
                    }
                    else
                    {
                        LogDevice.SetTimeout(TimeoutScenario.DataLogging3);
                    }
                    RequestNextSlots();
                }
                //Thread.Sleep(10);

                stopLogLoop = false;
                logTokenSource = new CancellationTokenSource();
                logToken = logTokenSource.Token;
                logTask = Task.Factory.StartNew(() => DataLoggingLoop(), logToken);
                logWriterTokenSource = new CancellationTokenSource();
                logWriterToken = logWriterTokenSource.Token;
                logWriterTask = Task.Factory.StartNew(() => LogWriterLoop(), logWriterToken);
                return true;
            }
            catch (Exception ex)
            {
                logwriter = null;
                LogDevice.Dispose();
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, StartLogging line " + line + ": " + ex.Message);
                return false;
            }
        }

        public  bool RequestNextSlots()
        {
            OBDMessage msg;
            byte[] rq = slothandler.CreateNextSlotRequestMessage();
            msg = new OBDMessage(rq);
            Debug.WriteLine("Requesting Slots: " + msg.ToString());
            bool resp =  LogDevice.SendMessage(msg, -maxSlotsPerRequest);
            return resp;
        }


        public  string QueryPcmOS()
        {
            try
            {
                Debug.WriteLine("OS?");
                LogDevice.ClearMessageBuffer();
                LogDevice.ClearMessageQueue();
                byte[] queryMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.ReadBlock, 0x0A }; //6C 10 F0 3C 0A
                bool m = LogDevice.SendMessage(new OBDMessage(queryMsg), 1);
                if (!m)
                {
                    //Logger("No respond to OS Query message");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                    return null;
                }
                Thread.Sleep(30);
                OBDMessage resp = LogDevice.ReceiveMessage();
                if (resp == null || resp.Length < 5)
                {
                    //LoggerBold("Empty response");
                    return null;
                }
                uint os = ReadUint32(resp.GetBytes(), 5, true);
                Debug.WriteLine("Response: " + resp.ToString());
                OS = os.ToString();
                return os.ToString();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("QyeryPcmOS: " + ex.Message);
                return null;
            }
        }

        public  ReadValue QueryRAM(int address, ProfileDataType dataType)
        {
            ReadValue rv = new ReadValue();
            try
            {                
                Debug.WriteLine("RAM? Address: " + address);
                byte byte1 = (byte)(address >> 16);
                byte byte2 = (byte)(address >> 8);
                byte byte3 = (byte)address;

                Receiver.SetReceiverPaused(true);

                byte[] queryMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.ReadBlock,0x89, byte1, byte2, byte3, 0xFF }; //6C 10 F0 3C 0A
                Debug.WriteLine("RAM query: " + BitConverter.ToString(queryMsg));
                bool m = LogDevice.SendMessage(new OBDMessage(queryMsg), 1);
                if (!m)
                {
                    Logger("No respond to RAM Query message");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                    Receiver.SetReceiverPaused(false);
                    return null;
                }
                Thread.Sleep(30);
                OBDMessage resp = LogDevice.ReceiveMessage();
                if (resp == null || resp.GetBytes()[3] == 0x7f)
                {
                    LoggerBold("Address: " + address.ToString("X8") + " not supported, or other error");
                    Receiver.SetReceiverPaused(false);
                    return rv;
                }
                rv = ParseSinglePidMessage(resp, dataType);
            }
            catch (Exception ex)
            {
                LoggerBold("QyeryRAM: " + ex.Message);
            }
            Receiver.SetReceiverPaused(false);
            return rv;
        }

        public void QueryVIN()
        {
            string vin = "";
            try
            {
                Debug.WriteLine("VIN?");
                Receiver.SetReceiverPaused(true);
                LogDevice.ClearMessageBuffer();
                LogDevice.ClearMessageQueue();
                byte[] vinbytes = new byte[3*6];
                for (byte v = 0; v < 3; v++)
                {
                    byte[] queryMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.ReadBlock, (byte)(v+1) };
                    bool m = LogDevice.SendMessage(new OBDMessage(queryMsg),1);
                    if (!m)
                    {
                        Logger("No response to VIN Query message");
                        Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                        Receiver.SetReceiverPaused(false);
                        return;
                    }
                    Thread.Sleep(100);
                    OBDMessage resp = LogDevice.ReceiveMessage();
                    while (resp != null)
                    {
                        if (resp.Length > 10 && resp[3] == 0x7C)
                        {
                            Array.Copy(resp.GetBytes(), 5, vinbytes, v * 6, 6);
                            Debug.WriteLine("Response: " + resp.ToString());
                            break;
                        }
                        resp = LogDevice.ReceiveMessage();
                    }
                }
                vin = Encoding.ASCII.GetString(vinbytes, 1, 17);
                Logger("VIN Code:" + vin);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("QueryVIN: " + ex.Message);
            }
            Receiver.SetReceiverPaused(false);
        }

        public DTCCodeStatus DecodeDTCstatus(byte[] msg)
        {
            DTCCodeStatus dcs = new DTCCodeStatus();
            try
            {
                ushort dtc = ReadUint16(msg, 4, true);
                byte stat = msg[6];
                if (stat > 0)
                {
                    string code = DtcSearch.DecodeDTC(dtc.ToString("X4"));
                    dcs.Code = code;
                    byte module = msg[2];
                    dcs.Module = module.ToString("X2");
                    if (analyzer.PhysAddresses.ContainsKey(module))
                        dcs.Module = analyzer.PhysAddresses[module];
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
                            dcs.Status += "STORED TROUBLE CODE (FREEZE FRAMD DATA AVAILABLE),";
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

        public void QueueDtcRequest(byte module, byte mode)
        {
            Logger("Adding DTC request to queue");
            QueuedCommand command = new QueuedCommand();
            command.Cmd = QueueCmd.Getdtc;
            command.param1 = module;
            command.param2 = mode;
            lock(queuedCommands)
            {
                queuedCommands.Enqueue(command);
            }
        }
        public void QueueVINRequest()
        {
            Logger("Adding VIN request to queue");
            QueuedCommand command = new QueuedCommand();
            command.Cmd = QueueCmd.GetVin;
            lock (queuedCommands)
            {
                queuedCommands.Enqueue(command);
            }
        }

        public bool RequestDTCCodes(byte module, byte mode)
        {
            try
            {
                string moduleStr = module.ToString("X2");
                if (module == DeviceId.Broadcast)
                {
                    moduleStr = "Broadcast";
                }
                if (analyzer.PhysAddresses.ContainsKey(module))
                    moduleStr = analyzer.PhysAddresses[module];
                Logger("Requesting DTC codes for " + moduleStr);
                Receiver.SetReceiverPaused(true);
                OBDMessage msg = new OBDMessage(new byte[] { Priority.Physical0, module, DeviceId.Tool, 0x19, mode, 0xFF, 0x00 });
                bool m = LogDevice.SendMessage(msg, -50);
                if (!m)
                {
                    Debug.WriteLine("Error sending request, continue anyway");
                    //return false;
                }
                Thread.Sleep(100);
                Debug.WriteLine("Receiving DTC codes...");
                //byte[] endframe = new byte[] { Priority.Physical0, DeviceId.Tool, module, 0x59, 0x00, 0x00, 0xFF };
                OBDMessage resp = LogDevice.ReceiveMessage();
                while (resp != null) // && !Utility.CompareArraysPart(resp.GetBytes(), endframe))
                {
                    Debug.WriteLine("DTC received message: " + resp.ToString());
                    resp = LogDevice.ReceiveMessage();
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
                Debug.WriteLine("Error, RequestDTCCodes line " + line + ": " + ex.Message);
                return false;
            }
            Receiver.SetReceiverPaused(false);
            return true;
        }


        public  bool SendTesterPresent(bool force)
        {
            try
            {
                if (force || DateTime.Now.Subtract(lastPresent) >= TimeSpan.FromMilliseconds(4500))
                {
                    Debug.WriteLine("Seconds since last testerpresent: " + DateTime.Now.Subtract(lastPresent));
                    {
                        byte[] presentMsg = { priority, DeviceId.Broadcast, DeviceId.Tool, 0x3F };
                        LogDevice.SendMessage(new OBDMessage(presentMsg), -maxSlotsPerRequest);
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

        public  ReadValue ParseSinglePidMessage(OBDMessage msg, ProfileDataType datatype)
        {
            ReadValue retVal = new ReadValue();
            try
            {
                if (msg.Length > 6)
                {
                    Debug.WriteLine("Parsing message: " + msg.ToString());
                    int pos = 5;
                    double val = 0;
                    switch (datatype)
                    {
                        case ProfileDataType.UBYTE:
                            val = (byte)msg[6];
                            pos++;
                            break;
                        case ProfileDataType.SBYTE:
                            val = (sbyte)msg[6];
                            pos++;
                            break;
                        case ProfileDataType.UWORD:
                            val = ReadUint16(msg.GetBytes(), 6, true);
                            pos += 2;
                            break;
                        case ProfileDataType.SWORD:
                            val = ReadInt16(msg.GetBytes(), 6,true);
                            pos += 2;
                            break;
                    }
                    ReadValue rv = new ReadValue();
                    if (msg[3] == 0x62)
                    {
                        rv.PidNr = ReadUint16(msg.GetBytes(), 4, true);
                    }
                    else if (msg[3] == 0x63)
                    {
                        byte[] tmp = new byte[4];
                        Array.Copy(msg.GetBytes(), 4, tmp, 2, 2);
                        tmp[1] = 0xFF;
                        rv.PidNr = (int)ReadUint32(tmp, 0, true);
                    }
                    rv.PidValue = val;
                    return rv;
                    //Debug.WriteLine("Pid: " + rv.PidName + ", "+ pc.DataType + ", " + val + " => " + pc.Math + " => " + rv.PidValue);
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
            return retVal;
        }

        
        private  bool RequestPassiveModeSlots()
        {
            lastPresent = DateTime.Now;
            if (!RequestNextSlots())
                return false;
            if (Responsetype == ResponseType.SendFast)
            {
                Responsetype = ResponseType.SendMedium2;
            }
            else if (Responsetype == ResponseType.SendMedium2)
            {
                Responsetype = ResponseType.SendFast;
            }
            if (LogDevice.LogDeviceType == LoggingDevType.Other) //If Elm device, need to wait for prompt
            {
                if (!RequestNextSlots())
                    return false;
            }
            return true;
        }

        public  void ELMPromptReceived()
        {
            Debug.WriteLine("Elm prompt received");

            if (stopLogLoop)
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
                    //LogDevice.SetTimeout(TimeoutScenario.DataLogging4);
                    if (!RequestPassiveModeSlots())
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
                RequestNextSlots();
            }

        }

        public void StopElmReceive()
        {
            try
            {
                if (LogDevice.LogDeviceType != LoggingDevType.Other)
                {
                    Debug.WriteLine("Time (ms) since last elmStop: " + DateTime.Now.Subtract(lastElmStop).TotalMilliseconds);
                    //if (force || DateTime.Now.Subtract(lastPresent) >= TimeSpan.FromMilliseconds(4500) || DateTime.Now.Subtract(lastElmStop) >= TimeSpan.FromMilliseconds(elmStopTreshold))
                    {
                        //Stop current receive
                        Debug.WriteLine("Stop elm receive");
                        port.Send(Encoding.ASCII.GetBytes("X \r"));
                        if (LogDevice.LogDeviceType == LoggingDevType.Elm)
                        {
                            Thread.Sleep(50);
                            byte[] queryMsg = { Priority.Physical0, 0x3, DeviceId.Tool, 0x3f };
                            LogDevice.SendMessage(new OBDMessage(queryMsg), 1);
                        }
                        Thread.Sleep(200);
                        //Thread.Sleep(50);
                        OBDMessage msg = LogDevice.ReceiveMessage();
                        while (msg != null && !msg.ElmPrompt)
                        {
                            Debug.WriteLine("Elm stop received msg: " + msg.ToString() + ", ElmPrompt: " + msg.ElmPrompt);
                            msg = LogDevice.ReceiveMessage();
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

        private  bool ValidateMessage(OBDMessage oMsg, ref int SlotCount)
        {
            byte lastSlot = slothandler.Slots.Last().Id;
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

            byte[] data = oMsg.GetBytes();
            byte PcmResponse = data[3];
            if (PcmResponse == 0x7F)
            {
                Debug.WriteLine(oMsg.ToString() + ": " + PcmResponses[oMsg.GetBytes().Last()]);
                SlotCount++;
                return false;
            }

            byte slot = data[4];
            if ((reverseSlotNumbers && slot < lastSlot) || (!reverseSlotNumbers && slot > lastSlot))    //It's not slot. Maybe 2A ?
            {
                return false;
            }

            return true;
        }
               
        private bool SendQueuedCommand()
        {
            if (queuedCommands.Count == 0)
            {
                return true;
            }
            QueuedCommand command;
            lock (queuedCommands)
            {
                command = queuedCommands.Dequeue();
            }

            Application.DoEvents();
            SetBusQuiet();
            Thread.Sleep(10);
            datalogger.LogDevice.SetTimeout(TimeoutScenario.DataLogging3);
            Thread.Sleep(10);
            SetBusQuiet();
/*            OBDMessage resp = LogDevice.ReceiveMessage();
            while (resp != null)
            {
                resp = LogDevice.ReceiveMessage();
            }
*/
            switch (command.Cmd)
            {
                case QueueCmd.Getdtc:
                    RequestDTCCodes(command.param1, command.param2);
                    break;
                case QueueCmd.GetVin:
                    QueryVIN();
                    break;
                case QueueCmd.Custom:
                    Logger("Sending queued command: " + command.Description);
                    LogDevice.SendMessage(command.CustomMsg, 1);
                    LogDevice.ReceiveMessage();
                    break;
            }
            if (passive)
            {
                maxSlotsPerMessage = 4;
                LogDevice.SetTimeout(TimeoutScenario.DataLogging4);
                AllSlotsRequested = false;
                RequestPassiveModeSlots();
            }
            else
            {
                if (LogDevice.LogDeviceType != LoggingDevType.Obdlink)
                {
                    LogDevice.SetTimeout(TimeoutScenario.DataLogging3);
                }

            }
            return true;
        }

        public void DataLoggingLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            int totalSlots = 0;
            ulong prevSlotTime = 0;

            AllSlotsRequested = false;

            Logger("Logging started");
            //PcmDevice.ClearMessageBuffer();
            //PcmDevice.ClearMessageQueue();
            
            //while (!stopLogLoop)
            
            while (!logToken.IsCancellationRequested)
            {
                try
                {
                    int SlotCount = 0;
                    while ( SlotCount < maxSlotsPerRequest && !stopLogLoop) //Loop there until requested Slots are handled
                    {
                        if (LogDevice == null || !Connected)
                        {
                            break;
                        }

                        OBDMessage oMsg = LogDevice.ReceiveLogMessage();
                        if (oMsg == null)
                        {
                            continue;
                        }
                        Debug.WriteLine("Received: " + oMsg.ToString() +", Elmprompt: " + oMsg.ElmPrompt);
                        if (oMsg.ElmPrompt)
                        {
                            ELMPromptReceived();
                            continue;
                        }
                        if (!ValidateMessage(oMsg,ref SlotCount))
                        {
                            continue;
                        }
                        //We really have received a Slot!
                        SlotCount++;
                        totalSlots++;
                        if (totalSlots == 10)
                        {
                            prevSlotTime = oMsg.TimeStamp;
                        }
                        if (totalSlots == 110)
                        {
                            ulong tLast = oMsg.TimeStamp;
                            ulong SlotDelay = tLast - prevSlotTime;
                            Debug.WriteLine("Time for 100 Slots: " + (SlotDelay / 10000).ToString() + " ms");
                        }
                        if (passive)
                        {
                            SendTesterPresent(false);
                        }
                        slothandler.HandleSlotMessage(oMsg);
                    } //Inner logloop

                    if (LogDevice.LogDeviceType == LoggingDevType.Other)
                    {
                        if (!SendQueuedCommand())
                        {
                            return;     //If receieved Stop-command, return
                        }
                    }
                    Application.DoEvents();
                    if (passive)
                    {
                        if (maxSlotsPerRequest < maxPassiveSlotsPerMsg) //Started with fewer Slots
                        {
                            if (LogDevice.LogDeviceType == LoggingDevType.Obdlink)
                            {
                                //Started with 4 Slots, now asking 50 more
                                RequestNextSlots();
                            }
                            Debug.WriteLine("Set Slots per request to: " + maxPassiveSlotsPerMsg);
                            maxSlotsPerRequest = maxPassiveSlotsPerMsg;
                        }
                        SendTesterPresent(false);
                    }
                    else //SendOnce
                    {
                        if (!RequestNextSlots())    //Works with Obdlink, because we know how many Slots are coming
                            throw new Exception("Error in Slot request");
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
                    if (LogDevice == null || !Connected)
                    {
                        return;
                    }
                }
            } //Logloop
            Logger("Logging stopped");
            if (LogDevice != null && Connected)
            { 
                SetBusQuiet();
                SetBusQuiet();
                LogDevice.SetTimeout(TimeoutScenario.Maximum);
            }
            LogRunning = false;
            return;
        }

    }
}
