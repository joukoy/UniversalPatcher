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
    public static class DataLogger
    {
        public static Task logTask;
        private static Task logWriterTask;
        public static IPort port;
        public static Analyzer analyzer;
        public static bool Connected = false;
        public static List<PidConfig> PidProfile { get; set; }
        public static List<J2534DotNet.J2534Device> jDevList;
        private static StreamWriter logwriter;
        private static string logseparator = ";";
        public static Device LogDevice;
        public static int ReceivedBytes = 0;
        public static string OS;
        public static SlotHandler slothandler;

        public static Dictionary<byte, string> PcmResponses;

        private static bool AllSlotsRequested = false;
        public static RunMode CurrentMode;
        private static bool passive;
        public static int maxPassiveSlotsPerMsg = 50;
        public static bool stopLogLoop;
        private static EventWaitHandle delayedMsgHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static EventWaitHandle pauseWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        public static byte priority = Priority.Physical0;
        private static DateTime lastPresent = DateTime.Now;
        private static DateTime lastElmStop = DateTime.Now;

        public static Queue<Analyzer.AnalyzerData> analyzerq = new Queue<Analyzer.AnalyzerData>();
        public static Queue<LogData> LogFileQueue = new Queue<LogData>();

        //Set these values before StartLogging()
        public static bool writelog;
        public static bool useRawValues;
        public static bool useBusFilters;
        public static bool reverseSlotNumbers;
        public static byte Responsetype;
        public static int maxSlotsPerRequest = 4;   //How many Slots before asking more
        public static int maxSlotsPerMessage = 4;   //How many Slots in one Slot request message
        public static bool HighPriority = false;

        public enum RunMode
        {
            NotRunning = 0,
            LogRunning = 1,
            AnalyzeRunning = 2,
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
            }
            public double[] Values { get; set; }
            public ulong TimeStamp { get; set; }
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
        }


        public class DTCCodeStatus
        {
            public string Module { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
        }

        public static readonly List<string> SupportedBaudRates = new List<string>
        {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400",
            "460800",
            "921600"
        };

        public static Device CreateSerialDevice(string serialPortName, string serialPortDeviceType, bool ftdi)
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

        public static bool CreateLog(string path)
        {
            try
            {
                Logger("Writing to logfile: " + path);
                logseparator = Properties.Settings.Default.LoggerLogSeparator;
                LogFileQueue.Clear();
                if (File.Exists(path))
                {
                    logwriter = new StreamWriter(path, true);
                }
                else
                {
                    StringBuilder sb = new StringBuilder("Time" + logseparator);
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
                        foreach (string s in pids)
                        {
                            sb.Append(s + logseparator);
                        }
                    }
                    else
                    {
                        for (int c = 0; c < PidProfile.Count; c++)
                        {
                            sb.Append(PidProfile[c].PidName + logseparator);
                        }
                    }
                    string header = sb.ToString().Trim(logseparator[0]);
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

        public static void WriteLog(string[] logvalues, string timestamp)
        {
            try
            {
                StringBuilder sb = new StringBuilder(timestamp + logseparator);
                for (int c = 0; c < logvalues.Length; c++)
                {
                    sb.Append(logvalues[c].Replace(",", ".") + logseparator);
                }
                logwriter.WriteLine(sb.ToString().Trim(logseparator[0]));
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


        private static void LogWriterLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            while (!stopLogLoop)
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
                if (useRawValues)
                {
                    string[] ldvalues = new string[ld.Values.Length];
                    for (int l = 0; l < ld.Values.Length; l++)
                        ldvalues[l] = ld.Values[l].ToString();
                    WriteLog(ldvalues, ld.TimeStamp.ToString());
                }
                else
                {
                    WriteLog(slothandler.CalculatePidValues(ld.Values), DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:FFFFF"));
                }
            }
            logwriter.Close();
            logwriter = null;
        }

        public static void LoadProfile(string FileName)
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

        public static void SaveProfile(string FileName)
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


        public static ReadValue QuerySinglePidValue(int addr, ProfileDataType dataType)
        {
            ReadValue rv = new ReadValue();
            try
            {
                LogDevice.ClearMessageQueue();
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
                        return rv;
                    }

                    while (resp != null)
                    {
                        if (resp.GetBytes()[3] == 0x7f)
                        {
                            string errStr = "Unknown error";
                            int p = resp.Length - 1;
                            if (PcmResponses.ContainsKey(resp[p]))
                                errStr = PcmResponses[resp[p]];
                            LoggerBold("Pid: " + addr.ToString("X4") + " Error: " + errStr);
                            return rv;
                        }
                        else
                        {
                            rv = ParseSinglePidMessage(resp, dataType);
                            if (rv.PidNr == addr)
                            {
                                return rv;
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
            return rv;
        }


        public static bool SetBusQuiet()
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
                    Logger("Unable to set bus quiet");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(quietMsg, b => b.ToString("X2"))));
                    return false;
                }
                Thread.Sleep(10);
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold("SetBusQuiet: " + ex.Message);
                return false;
            }
        }

        public static Response<List<byte>> QueryDevicesOnBus(bool waitanswer)
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

        public static void ClearTroubleCodes(byte module)
        {
            try
            {
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
        }

        
        public static bool SetMode1()
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
        public static bool InitalizeDevice(string comport, string devtype, bool ftdi, int BaudRate)
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
/*        public static bool InitalizeJDevice(string devtype)
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
        public static bool StartLogging()
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

                LogDevice.SetTimeout(TimeoutScenario.DataLogging1);

                if (QueryDevicesOnBus(false).Status != ResponseStatus.Success)
                    return false;
                if (!SetBusQuiet())
                    return false;
                if (!SetBusQuiet())
                    return false;
                if (!LogDevice.SetLoggingFilter())
                    return false;
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
                    if (LogDevice.LogDeviceType != LoggingDevType.Obdlink)
                    {
                        LogDevice.SetTimeout(TimeoutScenario.DataLogging3);
                    }
                    RequestNextSlots();
                }
                //Thread.Sleep(10);

                stopLogLoop = false;
                pauseWaitHandle.Set();
                logTask = Task.Factory.StartNew(() => DataLoggingLoop());
                logWriterTask = Task.Factory.StartNew(() => LogWriterLoop());
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

        public static bool RequestNextSlots()
        {
            OBDMessage msg;
            byte[] rq = slothandler.CreateNextSlotRequestMessage();
            msg = new OBDMessage(rq);
            Debug.WriteLine("Requesting Slots: " + msg.ToString());
            bool resp =  LogDevice.SendMessage(msg, -maxSlotsPerRequest);
            return resp;
        }

        public static void StopLogging()
        {
            stopLogLoop = true;
        }

        public static string QueryPcmOS()
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
                    Logger("No respond to OS Query message");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                    return null;
                }
                Thread.Sleep(30);
                OBDMessage resp = LogDevice.ReceiveMessage();
                if (resp == null || resp.Length < 5)
                {
                    LoggerBold("Empty response");
                    return null;
                }
                uint os = ReadUint32(resp.GetBytes(), 5, true);
                Debug.WriteLine("Response: " + resp.ToString());
                OS = os.ToString();
                return os.ToString();
            }
            catch(Exception ex)
            {
                LoggerBold("QyeryPcmOS: " + ex.Message);
                return null;
            }
        }

        public static ReadValue QueryRAM(int address, ProfileDataType dataType)
        {
            ReadValue rv = new ReadValue();
            try
            {                
                Debug.WriteLine("RAM? Address: " + address);
                LogDevice.ClearMessageBuffer();
                LogDevice.ClearMessageQueue();
                byte byte1 = (byte)(address >> 16);
                byte byte2 = (byte)(address >> 8);
                byte byte3 = (byte)address;

                byte[] queryMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.ReadBlock,0x89, byte1, byte2, byte3, 0xFF }; //6C 10 F0 3C 0A
                Debug.WriteLine("RAM query: " + BitConverter.ToString(queryMsg));
                bool m = LogDevice.SendMessage(new OBDMessage(queryMsg), 1);
                if (!m)
                {
                    Logger("No respond to RAM Query message");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                    return null;
                }
                Thread.Sleep(30);
                OBDMessage resp = LogDevice.ReceiveMessage();
                if (resp == null || resp.GetBytes()[3] == 0x7f)
                {
                    LoggerBold("Address: " + address.ToString("X8") + " not supported, or other error");
                    return rv;
                }
                rv = ParseSinglePidMessage(resp, dataType);
            }
            catch (Exception ex)
            {
                LoggerBold("QyeryRAM: " + ex.Message);
            }
            return rv;
        }

        public static string QueryVIN()
        {
            string vin = "";
            try
            {
                Debug.WriteLine("VIN?");
                LogDevice.ClearMessageBuffer();
                LogDevice.ClearMessageQueue();
                byte[] vinbytes = new byte[3*6];
                for (byte v = 0; v < 3; v++)
                {
                    byte[] queryMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.ReadBlock, (byte)(v+1) };
                    bool m = LogDevice.SendMessage(new OBDMessage(queryMsg),1);
                    if (!m)
                    {
                        Logger("No respond to VIN Query message");
                        Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(queryMsg, b => b.ToString("X2"))));
                        return "";
                    }
                    Thread.Sleep(100);
                    OBDMessage resp = LogDevice.ReceiveMessage();
                    Array.Copy(resp.GetBytes(), 5, vinbytes, v * 6, 6);
                    Debug.WriteLine("Response: " + resp.ToString());
                }
                vin = Encoding.ASCII.GetString(vinbytes, 1, 17);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("QueryVIN: " + ex.Message);
            }
            return vin.ToString();
        }

        private static DTCCodeStatus DecodeDTCstatus(byte[] msg)
        {
            DTCCodeStatus dcs = new DTCCodeStatus();
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
                    dcs.Description = descr.Description ;
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
            return dcs;
        }

        public static List<DTCCodeStatus> RequestDTCCodes(byte module, byte mode)
        {
            List<DTCCodeStatus> retVal = new List<DTCCodeStatus>();
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
                if (CurrentMode != RunMode.NotRunning && LogDevice.LogDeviceType != LoggingDevType.Other)
                    Logger("Log running, may result to empty list");
                if (OBD2Codes == null || OBD2Codes.Count == 0)
                    LoadOBD2Codes();
                //SetLoggingPaused(true);
                OBDMessage msg = new OBDMessage(new byte[] { Priority.Physical0, module, DeviceId.Tool, 0x19, mode, 0xFF, 0x00 });
                bool m = LogDevice.SendMessage(msg, -50);
                if (!m)
                {
                    LoggerBold("Error sending request");
                    return retVal;
                }
                //byte[] endframe = new byte[] { Priority.Physical0, DeviceId.Tool, module, 0x59, 0x00, 0x00, 0xFF };
                Thread.Sleep(10);
                OBDMessage resp = LogDevice.ReceiveMessage();
                //Logger("Received:" + resp.ToString());
                while (resp != null) // & !Utility.CompareArraysPart(resp.GetBytes(), endframe))
                {
                    Debug.WriteLine(resp.ToString());
                    if (resp.Length > 5 && resp.GetBytes()[1] == DeviceId.Tool && resp.GetBytes()[3] == 0x59)
                    {
                        DTCCodeStatus dcs = DecodeDTCstatus(resp.GetBytes());
                        if (!string.IsNullOrEmpty(dcs.Module))
                        {
                            retVal.Add(dcs);
                        }
                    }
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
                LoggerBold("Error, RequestDTCCodes line " + line + ": " + ex.Message);
            }
            return retVal;
        }


        public static bool SendTesterPresent(bool force)
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

        public static ReadValue ParseSinglePidMessage(OBDMessage msg, ProfileDataType datatype)
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

/*        public static bool SetLoggingPaused(bool Pause)
        {
            if (CurrentMode == RunMode.NotRunning)
            {
                return true;
            }
            if (Pause)
            {
                Logger("Pausing log...");
                pauseWaitHandle.Reset();
                Application.DoEvents();
                if (LogDevice.LogDeviceType != LoggingDevType.Other)
                {
                    StopElmReceive(true);
                    Thread.Sleep(50);
                    OBDMessage msg = null;
                    while (msg == null || msg.ElmPrompt == false)
                    {
                        msg = LogDevice.ReceiveMessage();
                    }
                    
                }
                return true;
            }
            else
            {
                Thread.Sleep(50);
                pauseWaitHandle.Set();
                Logger("Continue logging");
                return true;
            }
        }
*/        
        private static bool RequestPassiveModeSlots()
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

        public static void ELMPromptReceived()
        {
            Debug.WriteLine("Elm prompt received");

            if (stopLogLoop)
            {
                SetBusQuiet();
                return;
            }

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

        private static void StopElmReceive()
        {
            if (LogDevice.LogDeviceType != LoggingDevType.Other)
            {
                Debug.WriteLine("Time (ms) since last elmStop: " + DateTime.Now.Subtract(lastElmStop).TotalMilliseconds);
                //if (force || DateTime.Now.Subtract(lastPresent) >= TimeSpan.FromMilliseconds(4500) || DateTime.Now.Subtract(lastElmStop) >= TimeSpan.FromMilliseconds(elmStopTreshold))
                {
                    //Stop current receive
                    Debug.WriteLine("Stop elm receive");
                    port.Send(Encoding.ASCII.GetBytes("X \r"));
                    lastElmStop = DateTime.Now;
                }
            }
        }

        private static bool ValidateMessage(OBDMessage oMsg, ref int SlotCount)
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
               
        public static void DataLoggingLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            int totalSlots = 0;
            ulong prevSlotTime = 0;

            AllSlotsRequested = false;

            Logger("Logging started");
            //PcmDevice.ClearMessageBuffer();
            //PcmDevice.ClearMessageQueue();
            while (!stopLogLoop)
            {
                try
                {
                    int SlotCount = 0;
                    while ( SlotCount < maxSlotsPerRequest && !stopLogLoop) //Loop there until requested Slots are handled
                    {
                        pauseWaitHandle.WaitOne();  //If asked to pause, wait here

                        OBDMessage oMsg = LogDevice.ReceiveLogMessage();
                        if (oMsg == null)
                        {
                            continue;
                        }
                        //Debug.WriteLine("Received: " + oMsg.ToString() +", Elmprompt: " + oMsg.ElmPrompt);
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
                }
            } //Logloop
            SetBusQuiet();
            Logger("Logging stopped");
            CurrentMode = RunMode.NotRunning;
            SetBusQuiet();
            LogDevice.SetTimeout(TimeoutScenario.Maximum);
            return;
        }

    }
}
