using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using J2534;
using J2534DotNet;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;
using static Helpers;
using static Upatcher;
using System.Windows.Forms;
using static LoggerUtils;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates all code that is unique to the J2534 interface.
    /// </summary>
    ///
    class J2534Device : Device
    {
        /// <summary>
        /// Configuration settings
        /// </summary>
        public int ReadTimeout = 500;
        public int WriteTimeout = 500;

        /// <summary>
        /// variety of properties used to id channels, filters and status
        /// </summary>
        private J2534_Struct J2534Port;
        public List<int> Filters;
        public List<int> Filters2;
        private int DeviceID;
        private int ChannelID;
        private int ChannelID2;
        private ProtocolID Protocol;
        private ProtocolID Protocol2;
        public bool IsProtocolOpen;
        private const string PortName = "J2534";
        public string ToolName = "";
        //private int periodigMsgId = -1;
        //private int periodigMsgId2 = -1;
        public const string DeviceType = "J2534";
        private Dictionary<string, int> protocol_channel;
        private Dictionary<string, int> periodicMsgIds1;
        private Dictionary<string, int> periodicMsgIds2;
        private static J2534DotNet.RxStatus? _customfilter;
        private bool isSimulator = false;
        private bool SeparateProtocolByChannel;
        private J2534DotNet.RxStatus CustomFilter
        {
            get
            {
                if (_customfilter == null)
                {
                    J2534DotNet.RxStatus rxstatus = J2534DotNet.RxStatus.NONE;
                    string[] fParts = AppSettings.LoggerFilterRxStatusCustom.Split('|');
                    foreach (string fp in fParts)
                    {
                        if (Enum.TryParse<J2534DotNet.RxStatus>(fp.ToUpper(), out J2534DotNet.RxStatus f))
                        {
                            rxstatus = rxstatus | f;
                        }
                    }
                    _customfilter = rxstatus;
                }
                return _customfilter.Value;
            }
        }

        /// <summary>
        /// global error variable for reading/writing. (Could be done on the fly)
        /// TODO, keep record of all errors for debug
        /// </summary>
        public J2534Err OBDError;

        /// <summary>
        /// J2534 has two parts.
        /// J2534device which has the supported protocols ect as indicated by dll and registry.
        /// J2534extended which is al the actual commands and functions to be used. 
        /// </summary>
        struct J2534_Struct
        {
            //public J2534Extended Functions;
            public J2534DotNet.J2534Device LoadedDevice;
            public J2534 Functions;
        }

        public J2534Device(J2534DotNet.J2534Device jport) : base()
        {
            J2534Port = new J2534_Struct();
            //J2534Port.Functions = new J2534Extended();
            J2534Port.Functions = new J2534();
            J2534Port.LoadedDevice = jport;

            // Reduced from 4096+12 for the MDI2
            this.MaxSendSize = 2048 + 12;    // J2534 Standard is 4KB
            this.MaxReceiveSize = 2048 + 12; // J2534 Standard is 4KB
            this.Supports4X = true;
            this.LogDeviceType = DataLogger.LoggingDevType.J2534;
            ChannelID = -1;
            ChannelID2 = -1;
            protocol_channel = new Dictionary<string, int>();
            periodicMsgIds1 = new Dictionary<string, int>();
            periodicMsgIds2 = new Dictionary<string, int>();
            if (jport.Name.ToLower().Contains("sim"))
            {
                isSimulator = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisconnectTool();
        }

        public override string ToString()
        {
            return "J2534 Device";
        }

        // This needs to return Task<bool> for consistency with the Device base class.
        // However it doesn't do anything asynchronous, so to make the code more readable
        // it just wraps a private method that does the real work and returns a bool.
        public override bool Initialize(int BaudRate, J2534InitParameters j2534Init) //Baudrate not really used
        {
            return this.InitializeInternal(j2534Init);
        }

        // This returns 'bool' for the sake of readability. That bool needs to be
        // wrapped in a Task object for the public Initialize method.
        private bool InitializeInternal(J2534InitParameters j2534Init)
        {
            try
            {
                Filters = new List<int>();
                Filters2 = new List<int>();

                Logger("Initializing " + this.ToString());

                Response<J2534Err> m; // hold returned messages for processing
                Response<bool> m2;
                Response<double> volts;

                //Check J2534 API
                //Debug.WriteLine(J2534Port.Functions.ToString());

                //Check not already loaded
                //if (IsLoaded == true)
                if (DetectLibrary())
                {
                    Debug.WriteLine("DLL already loaded, unloading before proceeding");
                    bool result = CloseLibrary();
                    if (!result)
                    {
                        Logger("Error closing loaded DLL");
                        //return false;
                    }
                    else
                    {
                        Debug.WriteLine("Existing DLL successfully unloaded.");
                    }
                }

                //Connect to requested DLL
                m2 = LoadLibrary(J2534Port.LoadedDevice);
                if (m2.Status != ResponseStatus.Success)
                {
                    Logger("Unable to load the J2534 DLL.");
                    return false;
                }
                Debug.WriteLine("Loaded DLL");
                //connect to scantool
                m = ConnectTool();
                if (m.Status != ResponseStatus.Success)
                {
                    Logger("Unable to connect to the device: " + m.Value);
                    return false;
                }

                Logger("Connected to the device, ID: " + DeviceID.ToString());

                //Optional.. read API,firmware version ect here
                //read voltage
                volts = ReadVoltage();
                if (volts.Status == ResponseStatus.Success)
                {
                    Logger("Battery Voltage is: " + volts.Value.ToString());
                }
                else
                {
                    Logger("Unable to read battery voltage.");
                }
                Response<string> ver = ReadVersion();
                if (ver.Status == ResponseStatus.Success)
                {
                    Logger(ver.Value);
                }
                else
                {
                    Logger("Unable to read device version.");
                }

                //Set Protocol
                if (j2534Init.VPWLogger)
                {
                    m = ConnectToProtocol(ProtocolID.J1850VPW, BaudRate.J1850VPW_10400, ConnectFlag.NONE, ref ChannelID);
                    if (m.Status != ResponseStatus.Success)
                    {
                        Logger("Failed to set protocol, J2534 error: " + m.ToString());
                        return false;
                    }
                    Protocol = ProtocolID.J1850VPW;
                    SetLoggingFilter();
                }
                else
                {
                    /*
                    Logger("Protocol: " + j2534Init.Protocol);
                    Logger("Baudrate: " + j2534Init.Baudrate);
                    Logger("Connectflag: " + j2534Init.Connectflag.ToString());                    
                    */
                    m = ConnectToProtocol(j2534Init.Protocol, (BaudRate)Enum.Parse(typeof(BaudRate), j2534Init.Baudrate), j2534Init.Connectflag, ref ChannelID);
                    if (m.Status != ResponseStatus.Success)
                    {
                        Logger("Failed to set protocol, J2534 error: " + m.ToString());
                        return false;
                    }
                    Protocol = j2534Init.Protocol;
                    if (!SetConfigParams(j2534Init, ChannelID))
                    {
                        Logger("Failed to set parameters");
                        return false;
                    }
                }
                //For debugging J2534 SIM DLL:
                //Logger("Setting programming voltage");
                //OBDError = J2534Port.Functions.SetProgrammingVoltage(DeviceID, PinNumber.PIN_12, 0xFFFFFFFF);
                //OBDError = J2534Port.Functions.SetProgrammingVoltage(DeviceID, PinNumber.PIN_9, 0xFFFFFFFF);
                Debug.WriteLine("Protocol Set");
                J2534FunctionsIsLoaded = true;
                //receiveTask = Task.Factory.StartNew(() => DataReceiver());
                this.Connected = true;
                Logger("Device initialization complete.");
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        public override bool SetProgramminVoltage(PinNumber pinNumber, uint voltage)
        {
            OBDError = J2534Port.Functions.SetProgrammingVoltage(DeviceID, pinNumber, voltage);
            if (OBDError == J2534Err.STATUS_NOERROR)
            {
                if (voltage == 0xFFFFFFFF)
                    Debug.WriteLine("Programming voltage set, pin: " + pinNumber.ToString() + " OFF");
                else if (voltage == 0xFFFFFFFE)
                    Debug.WriteLine("Programming voltage set, pin: " + pinNumber.ToString() + " GND");
                else
                    Debug.WriteLine("Programming voltage set, pin: " + pinNumber.ToString() + ", volts: " + ((float)voltage/1000).ToString("00.00"));
                return true;
            }
            else
            {
                Logger("Set programming voltage failed: " + OBDError.ToString());
                return false;
            }
        }

        public override bool AddToFunctMsgLookupTable(byte[] FuncAddr, bool secondary)
        {
            if (secondary)
                OBDError = J2534Port.Functions.AddToFunctMsgLookupTable(ChannelID2, FuncAddr);
            else
                OBDError = J2534Port.Functions.AddToFunctMsgLookupTable(ChannelID, FuncAddr);
            if (OBDError == J2534Err.STATUS_NOERROR)
            {
                Debug.WriteLine("Added functional addresses: " + BitConverter.ToString(FuncAddr));
                return true;
            }
            return false;
        }
        public override bool DeleteFromFunctMsgLookupTable(byte[] FuncAddr, bool secondary)
        {
            if (secondary)
                OBDError = J2534Port.Functions.DeleteFromFunctMsgLookupTable(ChannelID2, FuncAddr);
            else
                OBDError = J2534Port.Functions.DeleteFromFunctMsgLookupTable(ChannelID, FuncAddr);
            if (OBDError == J2534Err.STATUS_NOERROR)
            {
                Debug.WriteLine("Deleted functional addresses: " + BitConverter.ToString(FuncAddr));
                return true;
            }
            return false;
        }
        public override bool ClearFunctMsgLookupTable(bool secondary)
        {
            if (secondary)
                OBDError = J2534Port.Functions.ClearFunctMsgLookupTable(ChannelID2);
            else
                OBDError = J2534Port.Functions.ClearFunctMsgLookupTable(ChannelID);
            if (OBDError == J2534Err.STATUS_NOERROR)
            {
                Debug.WriteLine("Cleared functional addresses");
                return true;
            }
            return false;
        }

        public override bool SetJ2534Configs(string Configs, bool secondary)
        {
            bool retVal;
            if (secondary)
                retVal = SetSconfig(Configs, ChannelID2);
            else
                retVal = SetSconfig(Configs, ChannelID);
            return retVal;
        }

        public override bool StartPeriodicMsg(string PeriodicMsg, bool secondary)
        {
            ProtocolID proto = Protocol;
            int ChID = ChannelID;
            int Interval = 1000;
            if (secondary)
            {
                if (periodicMsgIds2.ContainsKey(PeriodicMsg))
                {
                    Logger("Periodic message already started, skipping");
                    return true;
                }
                proto = Protocol2;
                ChID = ChannelID2;
            }
            else
            {
                if (periodicMsgIds1.ContainsKey(PeriodicMsg))
                {
                    Logger("Periodic message already started, skipping");
                    return true;
                }
            }
            TxFlag tf = TxFlag.NONE;
            string[] pParts = PeriodicMsg.Split(':');
            if (pParts.Length > 1)
            {
                Int32.TryParse(pParts[1], out Interval);
            }
            if (pParts.Length > 2)
            {
                tf = ParseTxFLags(pParts[2]);
            }
            byte[] data = pParts[0].Replace(" ", "").ToBytes();
            PassThruMsg txMsg = new PassThruMsg(Protocol, 0, data);
            txMsg.TxFlags = tf;
            int pMsgId = -1;
            J2534Err jErr = J2534Port.Functions.StartPeriodicMsg(ChID, txMsg, ref pMsgId, Interval);
            if (jErr != J2534Err.STATUS_NOERROR)
            {
                Logger("Periodic message error: " + jErr.ToString());
                return false;
            }
            if (!secondary)
            {
                Logger("Started periodic message: " + pParts[0] + " for protocol 1, ID: " + pMsgId.ToString());
                //periodigMsgId = pMsgId;
                periodicMsgIds1.Add(PeriodicMsg, pMsgId);
            }
            else
            {
                Logger("Started periodic message: " + pParts[0] + " for protocol 2, ID: " + pMsgId.ToString());
                //periodigMsgId2 = pMsgId;
                periodicMsgIds2.Add(PeriodicMsg, pMsgId);
            }
            return true;
        }

        public override bool StopPeriodicMsg(string PeriodicMsg, bool secondary)
        {
            string[] pParts = PeriodicMsg.Split(':');
            J2534Err err = J2534Err.ERR_FAILED;
            if (secondary)
            {
                if (periodicMsgIds2.ContainsKey(PeriodicMsg))
                {
                    Logger("Stopping periodic message: " + pParts[0] + " for protocol 2, ID: " + periodicMsgIds2[PeriodicMsg].ToString());
                    err = J2534Port.Functions.StopPeriodicMsg(ChannelID2, periodicMsgIds2[PeriodicMsg]);
                    periodicMsgIds2.Remove(PeriodicMsg);
                }
            }
            else
            {
                if (periodicMsgIds1.ContainsKey(PeriodicMsg))
                {
                    Logger("Stopping periodic message: " + pParts[0] + " for protocol 1, ID: " + periodicMsgIds1[PeriodicMsg].ToString());
                    err = J2534Port.Functions.StopPeriodicMsg(ChannelID, periodicMsgIds1[PeriodicMsg]);
                    periodicMsgIds1.Remove(PeriodicMsg);
                }
            }
            if (err == J2534Err.STATUS_NOERROR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool ClearPeriodicMsg(bool secondary)
        {
            J2534Err err;
            if (secondary)
            {
                err = J2534Port.Functions.ClearPeriodicMsgs(ChannelID2);
                periodicMsgIds2 = new Dictionary<string, int>();
            }
            else
            {
                err = J2534Port.Functions.ClearPeriodicMsgs(ChannelID);
                periodicMsgIds1 = new Dictionary<string, int>();
            }
            if (err == J2534Err.STATUS_NOERROR)
            {
                Logger("Cleared periodic messages for protocol: " + (Convert.ToInt32(secondary) + 1).ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        //Not in use
        private void DataReceiver()
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                Debug.WriteLine("J2534 receive loop started");
                while (true)
                {
                    if (ChannelID < 0)
                    {
                        Debug.WriteLine("Port closed, exit J2534 loop");
                        return;
                    }
                    Receive(true);
                    if (ChannelID2 > -1)
                    { 
                        Receive2(); 
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
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
        }

        private bool SetSconfig(string scp, int ChID)
        {
            bool retVal = true;
            string[] parts = scp.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                ConfigParameter cp = (ConfigParameter)Enum.Parse(typeof(ConfigParameter), parts[0].ToUpper());
                if (cp == ConfigParameter.ADD_TO_FUNCT_MSG_LOOKUP_TABLE)
                {
                    byte[] funcAddr = GetByteParameters(parts[1]);
                    J2534Port.Functions.AddToFunctMsgLookupTable(ChID, funcAddr);
                }
                else if (cp == ConfigParameter.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE)
                {
                    byte[] funcAddr = GetByteParameters(parts[1]);
                    J2534Port.Functions.DeleteFromFunctMsgLookupTable(ChID, funcAddr);
                }
                else
                {
                    int val = 0;
                    //if (parts[1].StartsWith("$") | !Int32.TryParse(parts[1], out val))
                    HexToInt(parts[1].Replace("$", ""), out val);
                    if (cp != ConfigParameter.NONE)
                    {
                        SConfig sconfig = new SConfig(cp, val);
                        SConfig[] sc = new SConfig[1];
                        sc[0] = sconfig;
                        Logger("Setting: " + cp.ToString() + " = " + val.ToString());
                        if (!SetConfig(sc, ChID))
                        {
                            retVal = false;
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Set J2534 parameters
        /// </summary>
        private bool SetConfigParams(J2534InitParameters j2534Init, int ChID)
        {
            try
            {
                SeparateProtocolByChannel = j2534Init.SeparateProtoByChannel;
                if (!string.IsNullOrEmpty(j2534Init.Sconfigs))
                {
                    Debug.WriteLine("Setting Sconfigs: " + j2534Init.Sconfigs);
                    string[] scParts = j2534Init.Sconfigs.Split('|');
                    foreach (string scp in scParts)
                    {
                        SetSconfig(scp, ChID);
                    }
                }
                if (j2534Init.Kinit == KInit.FastInit_J1979 || j2534Init.Kinit == KInit.FastInit_GMDelco || j2534Init.Kinit == KInit.FastInit_ME7_5)
                {
                    //byte[] data = { 0xC1,0x33,0xF1,0x81 };
                    PassThruMsg txMsg = new PassThruMsg(ProtocolID.ISO14230, 0, j2534Init.InitBytes.Replace(" ", "").ToBytes());
                    PassThruMsg rxMsg = new PassThruMsg();
                    J2534Err jErr = J2534Port.Functions.FastInit(ChID, txMsg, ref rxMsg);
                    if (jErr != J2534Err.STATUS_NOERROR)
                    {
                        LoggerBold("Fastinit error: " + jErr.ToString());
                        //return false;
                    }
                }
                else if (j2534Init.Kinit == KInit.FiveBaudInit_J1979)
                {
                    byte k1 = 0;
                    byte k2 = 0;
                    J2534Port.Functions.FiveBaudInit(ChID, 0x33, ref k1, ref k2);
                }
                else if (!string.IsNullOrEmpty(j2534Init.InitBytes))
                {
                    OBDMessage txMsg = new OBDMessage(j2534Init.InitBytes.Replace(" ", "").ToBytes());
                    if (ChID == ChannelID2)
                    {
                        txMsg.SecondaryProtocol = true;
                    }
                    if (!SendMessage(txMsg, 1))
                    {
                        LoggerBold("Error sending init bytes");
                    }
                }
                if (!string.IsNullOrEmpty(j2534Init.PerodicMsg))
                {
                    //Insert interval between message and txflags:
                    string[] pParts = j2534Init.PerodicMsg.Split(':');
                    string pMsg = pParts[0] + ":" + j2534Init.PeriodicInterval.ToString();
                    if (pParts.Length > 1)
                    {
                        pMsg += pParts[1];
                    }
                    if (ChID == ChannelID)
                    {
                        StartPeriodicMsg(pMsg, false);
                    }
                    else
                    {
                        StartPeriodicMsg(pMsg, true);
                    }
                }
                if (string.IsNullOrEmpty(j2534Init.PassFilters))
                {
                    byte[] mask = new byte[] { 0 };
                    byte[] pattern = new byte[] { 0 };
                    Response<J2534Err> m = SetFilter(mask, pattern, TxFlag.NONE, FilterType.PASS_FILTER, ChID, j2534Init.Protocol);
                    if (m.Status != ResponseStatus.Success)
                    {
                        LoggerBold("Failed to set filter, J2534 error: " + m.ToString());
                        return false;
                    }
                    this.CurrentFilter = FilterMode.None;
                }
                else
                {
                    SetupFilters(j2534Init.PassFilters, j2534Init.Secondary, true);
                    this.CurrentFilter = FilterMode.Custom;
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
                LoggerBold("Error, SetConfigParams line " + line + ": " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        public override TimeoutScenario SetTimeout(TimeoutScenario scenario)
        {
            return this.currentTimeoutScenario;
        }

        public override void SetWriteTimeout(int timeout)
        {
            Debug.WriteLine("Setting write timeout to: " + timeout.ToString());
            WriteTimeout = timeout;
        }

        public override void SetReadTimeout(int timeout)
        {
            ReadTimeout = timeout;
            //ReadTimeout = AppSettings.TimeoutJ2534DeviceRead;
            Debug.WriteLine("Setting read timeout to: " + ReadTimeout);
        }


        /// <summary>
        /// This will process incoming messages for up to 500ms looking for a message
        /// </summary>
        public Response<OBDMessage> FindResponse(OBDMessage expected)
        {
            //Debug.WriteLine("FindResponse called");
            for (int iterations = 0; iterations < 5; iterations++)
            {
                OBDMessage response = this.ReceiveMessage(true);
                    if (Utility.CompareArraysPart(response.GetBytes(), expected.GetBytes()))
                        return Response.Create(ResponseStatus.Success, response);
                Thread.Sleep(100);
            }

            return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
        }

        public override void ReceiveBufferedMessages()
        {
            Debug.WriteLine("Receiving messages from device buffer...");
            bool haveMessages;
            do
            {
                haveMessages = ReceiveData(false);
            } while (haveMessages);
            Debug.WriteLine("Done");
        }

        private bool ReceiveData(bool WaitForTimeout)
        {
            int currentReadTimeout = ReadTimeout;
            //Debug.WriteLine("Trace: Read Network Packet");
            try
            {
                if (!WaitForTimeout)
                {
                    ReadTimeout = 5;
                }
                int NumMessages = 1;
                List<PassThruMsg> rxMsgs = new List<PassThruMsg>();
                J2534Err jErr;
                DateTime startTime = DateTime.Now;
                NumMessages = 1;
                bool skip = true;
                while (skip)
                {
                    jErr = J2534Port.Functions.ReadMsgs((int)ChannelID, ref rxMsgs, ref NumMessages, ReadTimeout);
                    Application.DoEvents();
                    if (jErr == J2534Err.STATUS_NOERROR)
                    {
                        for (int m = 0; m < rxMsgs.Count; m++)
                        {
                            if (AppSettings.LoggerFilterStartOfMessage && (rxMsgs[m].RxStatus & RxStatus.START_OF_MESSAGE) == RxStatus.START_OF_MESSAGE)
                            {
                                skip = true;
                                Debug.WriteLine("Skipping START_OF_MESSAGE");
                            }
                            else if (AppSettings.LoggerFilterTXIndication && (rxMsgs[m].RxStatus & RxStatus.TX_INDICATION) == RxStatus.TX_INDICATION)
                            {
                                skip = true;
                                Debug.WriteLine("Skipping TX_INDICATION");
                            }
                            else if (!string.IsNullOrEmpty(AppSettings.LoggerFilterRxStatusCustom) && rxMsgs[m].RxStatus == CustomFilter)
                            {
                                skip = true;
                                Debug.WriteLine("Skipping: " + CustomFilter.ToString());
                            }
                            else
                            {
                                skip = false;
                                //Debug.WriteLine("RX: " + rxMsgs[m].Data.ToHex()); //Debug messages hang program, maybe too frequent?
                                OBDMessage oMsg = new OBDMessage(rxMsgs[m].Data, DateTime.Now.Ticks, (ulong)OBDError);
                                oMsg.DeviceTimestamp = rxMsgs[m].Timestamp;
                                if (SeparateProtocolByChannel == false && rxMsgs[m].ProtocolID != Protocol && rxMsgs[m].ProtocolID == Protocol2)
                                {
                                    oMsg.SecondaryProtocol = true;
                                    this.Enqueue2(oMsg, true);
                                }
                                else
                                {
                                    this.Enqueue(oMsg, true);
                                }
                                ReadTimeout = currentReadTimeout;
                                return true;
                            }
                        }
                        if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(ReadTimeout))
                        {
                            Debug.WriteLine("J2534 Receive timeout: " + ReadTimeout.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        if (jErr == J2534Err.ERR_DEVICE_NOT_CONNECTED)
                        {
                            this.Connected = false;
                            Thread.Sleep(500);
                        }
                        else if (jErr == J2534Err.ERR_BUFFER_EMPTY)
                        {
                            if (isSimulator)
                            {
                                Thread.Sleep(AppSettings.TimeoutReceive);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Receive error: " + jErr);
                        }
                        ReadTimeout = currentReadTimeout;
                        return false;
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
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            ReadTimeout = currentReadTimeout;
            return false;
        }
        /// <summary>
        /// Read an network packet from the interface, and return a Response/Message
        /// </summary>
        public override void Receive(bool WaitForTimeout)
        {
            ReceiveData(WaitForTimeout);
        }

        /// <summary>
        /// Read an network packet from the interface, and return a Response/Message
        /// </summary>
        public override void Receive2()
        {
            //Debug.WriteLine("Trace: Read Network Packet");
            try
            {
                int NumMessages = 1;
                List<PassThruMsg> rxMsgs = new List<PassThruMsg>();
                J2534Err jErr;
                if (ChannelID2 > -1)
                {
                    bool skip = true;
                    while (skip)
                    {
                        jErr = J2534Port.Functions.ReadMsgs((int)ChannelID2, ref rxMsgs, ref NumMessages, ReadTimeout);
                        Application.DoEvents();
                        if (jErr == J2534Err.STATUS_NOERROR)
                        {
                            for (int m = 0; m < rxMsgs.Count; m++)
                            {
                                if (AppSettings.LoggerFilterStartOfMessage && (rxMsgs[m].RxStatus & RxStatus.START_OF_MESSAGE) == RxStatus.START_OF_MESSAGE)
                                {
                                    skip = true;
                                    Debug.WriteLine("Skipping START_OF_MESSAGE");
                                }
                                else if (AppSettings.LoggerFilterTXIndication && (rxMsgs[m].RxStatus & RxStatus.TX_INDICATION) == RxStatus.TX_INDICATION)
                                {
                                    skip = true;
                                    Debug.WriteLine("Skipping TX_INDICATION");
                                }
                                //else if (AppSettings.LoggerFilterStartOfMessage && rxMsgs[m].RxStatus == (RxStatus.TX_MSG_TYPE | RxStatus.TX_INDICATION))
                                //{
                                //   Debug.WriteLine("Skipping TX_MSG_TYPE|TX_INDICATION");
                                //}
                                else if (!string.IsNullOrEmpty(AppSettings.LoggerFilterRxStatusCustom) && rxMsgs[m].RxStatus == CustomFilter)
                                {
                                    skip = true;
                                    Debug.WriteLine("Skipping: " + CustomFilter.ToString());
                                }
                                else
                                {
                                    skip = false;
                                    //Debug.WriteLine("RX: " + rxMsgs[m].Data.ToHex()); //Debug messages hang program, maybe too frequent?
                                    //this.Enqueue(new OBDMessage(rxMsgs[m].Data, (ulong)rxMsgs[m].Timestamp, (ulong)OBDError));
                                    OBDMessage msg = new OBDMessage(rxMsgs[m].Data, DateTime.Now.Ticks, (ulong)OBDError);
                                    msg.DeviceTimestamp = rxMsgs[m].Timestamp;
                                    if (rxMsgs[m].ProtocolID != Protocol2 && rxMsgs[m].ProtocolID == Protocol)
                                    {
                                        this.Enqueue(msg, true);
                                    }
                                    else
                                    {
                                        msg.SecondaryProtocol = true;
                                        this.Enqueue2(msg, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (jErr == J2534Err.ERR_DEVICE_NOT_CONNECTED)
                            {
                                this.Connected = false;
                                Thread.Sleep(500);
                            }
                            else if (jErr == J2534Err.ERR_BUFFER_EMPTY)
                            {
                                if (isSimulator)
                                {
                                    Thread.Sleep(AppSettings.TimeoutReceive);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Receive error: " + jErr);
                            }
                            return;
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
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Send a message, wait for a response
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            try
            {
                //Debug.WriteLine("Send request called");
                //Debug.WriteLine("TX: " + message.GetBytes().ToHex()); //Too much debugging?
                int chid = ChannelID;
                ProtocolID proto = Protocol;
                if (message.SecondaryProtocol)
                {
                    chid = ChannelID2;
                    proto = Protocol2;
                }
                PassThruMsg TempMsg = new PassThruMsg(proto, message.Txflag,message.Rxstatus, message.GetBytes());
                TempMsg.Timestamp = (uint)DateTime.Now.Ticks;
                int NumMsgs = 1;

                //this.MessageSent(message);
                for (int retry = 0; retry < AppSettings.RetryWriteTimes; retry++)
                {                    
                    OBDError = J2534Port.Functions.WriteMsgs((int)chid,TempMsg, ref NumMsgs, WriteTimeout);
                    message.TimeStamp = DateTime.Now.Ticks;
                    message.DeviceTimestamp = DateTime.Now.Ticks;
                    message.Error = (ulong)OBDError;
                    this.MessageSent(message);
                    if (OBDError == J2534Err.STATUS_NOERROR)
                    {
                        break;
                    }
                    else
                    {
                        ReceiveBufferedMessages();
                        Debug.WriteLine("Write error: " + OBDError.ToString() + ", retrying...");
                        Thread.Sleep(AppSettings.RetryWriteDelay);
                    }
                }
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    //Debug messages here...check why failed..
                    string errMsg = "";
                    J2534Port.Functions.GetLastError(ref errMsg);
                    LoggerBold("J2534 WriteMsgs error: " + OBDError.ToString() + " (" + errMsg + ")");
                    return false;
                }
                //Debug.WriteLine("J2534 WriteMsgs sent: " + NumMsgs);
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// load in dll
        /// </summary>
        private Response<bool> LoadLibrary(J2534DotNet.J2534Device TempDevice)
        {
            try
            {
                ToolName = TempDevice.Name;
                J2534Port.LoadedDevice = TempDevice;
                if (DetectLibrary())
                {
                    Debug.WriteLine("Library already loaded");
                    //return Response.Create(ResponseStatus.Success, true);
                }
                if (J2534Port.Functions.LoadLibrary(J2534Port.LoadedDevice))
                {
                    return Response.Create(ResponseStatus.Success, true);
                }
                else
                {
                    return Response.Create(ResponseStatus.Error, false);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, false);

        }

        /// <summary>
        /// Check if dll is loaded in memory
        /// </summary>
        private bool DetectLibrary()
        {
            try
            {
                if (!J2534Port.Functions.isDllLoaded())
                {
                    Debug.WriteLine("Library not loaded: DLL is null");
                    return false;
                }

                Process proc = Process.GetCurrentProcess();
                foreach (ProcessModule dll in proc.Modules)
                {
                    if (dll.FileName == J2534Port.LoadedDevice.FunctionLibrary)
                    {
                        Debug.WriteLine("Library detected: " + dll.FileName);
                        return true;
                    }
                }
                Debug.WriteLine("Library not loaded");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// unload dll
        /// </summary>
        private bool CloseLibrary()
        {
            try
            {
                if (!DetectLibrary())
                {
                    Debug.WriteLine("Library not loaded");
                    return true; //Not loaded
                }
                Debug.WriteLine("Library loaded, unloading");
                if (J2534Port.Functions.FreeLibrary())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Connects to physical scantool
        /// </summary>
        private Response<J2534Err> ConnectTool()
        {
            try
            {
                DeviceID = 1;
                ChannelID = 2;
                Filters.Clear();
                Filters2.Clear();
                OBDError = 0;
                int tmpId = 1;
                OBDError = J2534Port.Functions.Open(ref tmpId);
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    return Response.Create(ResponseStatus.Error, OBDError);
                }
                else
                {
                    DeviceID = tmpId;
                    return Response.Create(ResponseStatus.Success, OBDError);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        /// <summary>
        /// Disconnects from physical scantool
        /// </summary>
        private Response<J2534Err> DisconnectTool()
        {
            try
            {
                if (!J2534FunctionsIsLoaded)
                {
                    return Response.Create(ResponseStatus.Success, OBDError);
                }
                if (ChannelID2 > -1)
                {
                    DisConnectSecondaryProtocol();
                }
                if (ChannelID > -1)
                {
                    DisconnectFromProtocol();
                }
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    Debug.WriteLine("J2534 Disconnect: " + OBDError.ToString());
                }
                OBDError = J2534Port.Functions.Close((int)DeviceID);
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    Debug.WriteLine("J2534 Close: " + OBDError.ToString());
                }
                //J2534Port.Functions.FreeLibrary();
                CloseLibrary();
                return Response.Create(ResponseStatus.Success, OBDError);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        /// <summary>
        /// keep record if DLL has been loaded in
        /// </summary>
        public bool IsLoaded
        {
            get 
            {
                if (J2534Port.Functions == null)
                    return false;
                return J2534Port.Functions.isDllLoaded(); 
            }
        }

        /// <summary>
        /// connect to selected protocol
        /// Must provide protocol, speed, connection flags, recommended optional is pins
        /// </summary>
        private Response<J2534Err> ConnectToProtocol(ProtocolID ReqProtocol, BaudRate Speed, ConnectFlag ConnectFlags, ref int ChID)
        {
            try
            {
                int tmpChannel = ChID;
                string protoSpeedStr = ReqProtocol.ToString() + Speed.ToString();
                if (protocol_channel.ContainsKey(protoSpeedStr))
                {
                    tmpChannel = protocol_channel[protoSpeedStr];
                }
                else
                {
                    OBDError = J2534Port.Functions.Connect((int)DeviceID, ReqProtocol, ConnectFlags, Speed, ref tmpChannel);
                    if (OBDError != J2534Err.STATUS_NOERROR) return Response.Create(ResponseStatus.Error, OBDError);
                    protocol_channel.Add(protoSpeedStr, tmpChannel);
                }
                Logger("Connected protocol: " + ReqProtocol.ToString() + " Speed: " + Speed.ToString());
                ChID = tmpChannel;
                IsProtocolOpen = true;
                return Response.Create(ResponseStatus.Success, OBDError);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        /// <summary>
        /// Disconnect from protocol
        /// </summary>
        private Response<J2534Err> DisconnectFromProtocol()
        {
            try
            {

                foreach (KeyValuePair<string, int> pMsg in periodicMsgIds1)
                {
                    Logger("Stopping perodic message, id: " + pMsg.Value.ToString());
                    J2534Port.Functions.StopPeriodicMsg(ChannelID, pMsg.Value);
                }
                periodicMsgIds1.Clear();
                if (AppSettings.ClearFuncAddrOnDisconnect)
                {
                    Logger("Clearing functional address table (primary protocol)");
                    ClearFunctMsgLookupTable(false);
                }
                Logger("Disconnecting primary protocol");
                if (protocol_channel.ContainsValue(ChannelID))
                {
                    var item = protocol_channel.First(x => x.Value == ChannelID);
                    protocol_channel.Remove(item.Key);
                }
                OBDError = J2534Port.Functions.Disconnect((int)ChannelID);
                ChannelID = -1;
                if (OBDError != J2534Err.STATUS_NOERROR) return Response.Create(ResponseStatus.Error, OBDError);
                IsProtocolOpen = false;
                return Response.Create(ResponseStatus.Success, OBDError);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        public override void Disconnect()
        {
            DisconnectTool();
        }

        /// <summary>
        /// Disconnect from Second protocol
        /// </summary>
        public override bool DisConnectSecondaryProtocol()
        {
            try
            {
                if (periodicMsgIds2.Count > 0)
                {
                    foreach (KeyValuePair<string, int> pMsg in periodicMsgIds2)
                    {
                        Logger("Stopping perodic message, id: " + pMsg.Value.ToString());
                        J2534Port.Functions.StopPeriodicMsg(ChannelID2, pMsg.Value);
                    }
                }
                if (AppSettings.ClearFuncAddrOnDisconnect)
                {
                    Logger("Clearing functional address table (secondary protocol)");
                    ClearFunctMsgLookupTable(true);
                }
                Logger("Disconnecting secondary protocol");
                if (ChannelID != ChannelID2)
                {
                    OBDError = J2534Port.Functions.Disconnect((int)ChannelID2);
                    var item = protocol_channel.First(x => x.Value == ChannelID2);
                    protocol_channel.Remove(item.Key);
                }
                ChannelID2 = -1;
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    Debug.WriteLine("Error disconnecting protocol: " + OBDError.ToString());
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
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Disconnect from Second protocol
        /// </summary>
        public override int[] SetupFilters(string FilterStr, bool Secondary, bool ClearOld)
        {
            try
            {
                int ChID = ChannelID;
                ProtocolID Proto = Protocol;
                if (Secondary)
                {
                    ChID = ChannelID2;
                    Proto = Protocol2;
                }
                if (ClearOld)
                {
                    if (Secondary)
                    {
                        ClearSecondaryFilters();
                    }
                    else
                    {
                        ClearFilters();
                    }

                }
                //J2534Port.Functions.ClearMsgFilters(ChID);
                Logger("Adding filters");
                Logger(FilterStr);
                JFilters jfilters = new JFilters(FilterStr, Proto);
                List<int> newFiltes = new List<int>();
                foreach (MsgFilter mfilter in jfilters.MessageFilters)
                {
                    int filterId = -1;
                    if (mfilter.FilterType == FilterType.FLOW_CONTROL_FILTER)
                    {
                        OBDError = J2534Port.Functions.StartMsgFilter(ChID, mfilter.FilterType, mfilter.Mask, mfilter.Pattern, mfilter.FlowControl, ref filterId);
                    }
                    else
                    {
                        OBDError = J2534Port.Functions.StartMsgFilter(ChID, mfilter.FilterType, mfilter.Mask, mfilter.Pattern, ref filterId);
                    }
                    if (OBDError != J2534Err.STATUS_NOERROR)
                    {
                        LoggerBold("Failed to set filter, J2534 error: " + OBDError.ToString());
                        return null;
                    }
                    newFiltes.Add(filterId);
                    if (Secondary)
                    {
                        Filters2.Add(filterId);
                    }
                    else
                    {
                        Filters.Add(filterId);
                    }
                    Logger("Added filter, ID: " + filterId.ToString());
                    Debug.WriteLine("Filter set");
                }
                return newFiltes.ToArray(); 
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Read battery voltage
        /// </summary>
        public Response<double> ReadVoltage()
        {
            try
            {
                double Volts = 0;
                int VoltsAsInt = 0;
                OBDError = J2534Port.Functions.ReadBatteryVoltage((int)DeviceID, ref VoltsAsInt);
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    return Response.Create(ResponseStatus.Error, Volts);
                }
                else
                {
                    Volts = VoltsAsInt / 1000.0;

                    return Response.Create(ResponseStatus.Success, Volts);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, (double)0);
        }

        /// <summary>
        /// Read battery voltage
        /// </summary>
        public Response<string> ReadVersion()
        {
            try
            {
                string firmware ="";
                string dll ="";
                string api = "";
                OBDError = J2534Port.Functions.ReadVersion((int)DeviceID, ref firmware,ref dll,ref api);
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    return Response.Create(ResponseStatus.Error, "");
                }
                else
                {
                    string retVal = "Versions: Firmware: " + firmware + ", DLL: " + dll + ", Api: " + api;
                    return Response.Create(ResponseStatus.Success, retVal);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, "");
        }

        /// <summary>
        /// Set filter
        /// </summary>
        private Response<J2534Err> SetFilter(byte[] Mask,byte[] Pattern,TxFlag txflag,FilterType Filtertype, int ChID, ProtocolID proto)
        {
            try
            {
                PassThruMsg maskMsg = new PassThruMsg(proto, txflag, Mask);
                PassThruMsg patternMsg = new PassThruMsg(proto, txflag, Pattern);

                int tempfilter = 0;
                Debug.WriteLine("Setting filter, Mask: " + BitConverter.ToString(Mask) + ", Pattern: " + BitConverter.ToString(Pattern));
                OBDError = J2534Port.Functions.StartMsgFilter(ChID, Filtertype, maskMsg, patternMsg, ref tempfilter);
                if (OBDError != J2534Err.STATUS_NOERROR) 
                    return Response.Create(ResponseStatus.Error, OBDError);
                Filters.Add(tempfilter);
                return Response.Create(ResponseStatus.Success, OBDError);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        /// <summary>
        /// Clear filters
        /// </summary>
        private Response<J2534Err> ClearFilters()
        {
            try
            {
                for (int i = 0; i < Filters.Count; i++)
                {
                    OBDError = J2534Port.Functions.StopMsgFilter((int)ChannelID, (int)Filters[i]);
                    if (OBDError != J2534Err.STATUS_NOERROR)
                    {
                        return Response.Create(ResponseStatus.Error, OBDError);
                    }
                }
                
                //OBDError = J2534Port.Functions.ClearMsgFilters(ChannelID);
                Filters.Clear();
                return Response.Create(ResponseStatus.Success, OBDError);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        /// <summary>
        /// Clear filters
        /// </summary>
        private Response<J2534Err> ClearSecondaryFilters()
        {
            try
            {
                for (int i = 0; i < Filters2.Count; i++)
                {
                    OBDError = J2534Port.Functions.StopMsgFilter(ChannelID2, Filters2[i]);
                    if (OBDError != J2534Err.STATUS_NOERROR)
                    {
                        Response.Create(ResponseStatus.Error, OBDError);
                    }
                }

                //OBDError = J2534Port.Functions.ClearMsgFilters(ChannelID);
                Filters2.Clear();
                return Response.Create(ResponseStatus.Success, OBDError);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return Response.Create(ResponseStatus.Error, J2534Err.ERR_FAILED);
        }

        /// <summary>
        /// Set the interface to low (false) or high (true) speed
        /// </summary>
        /// <remarks>
        /// The caller must also tell the PCM to switch speeds
        /// </remarks>
        protected override bool SetVpwSpeedInternal(VpwSpeed newSpeed)
        {
            try
            {
                ClearFilters();
                //Disconnect from current protocol
                DisconnectFromProtocol();
                if (newSpeed == VpwSpeed.Standard)
                {
                    Debug.WriteLine("J2534 setting VPW 1X");
                    //Connect at new speed
                    ConnectToProtocol(ProtocolID.J1850VPW, BaudRate.J1850VPW_10400, ConnectFlag.NONE, ref ChannelID);
                    if (CurrentFilter == FilterMode.Analyzer)
                        SetAnalyzerFilter();
                    else
                        SetLoggingFilter();
                }
                else
                {
                    Debug.WriteLine("J2534 setting VPW 4X");
                    //Connect at new speed
                    ConnectToProtocol(ProtocolID.J1850VPW, BaudRate.J1850VPW_41600, ConnectFlag.NONE, ref ChannelID);
                    //Set Filter
                    //SetFilter(0xFEFFFF, 0x6CF010, 0, TxFlag.NONE, FilterType.PASS_FILTER);
                    if (CurrentFilter == FilterMode.Analyzer)
                        SetAnalyzerFilter();
                    else
                        SetLoggingFilter();
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
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        public override void ClearMessageBuffer()
        {
            try
            {
                J2534Port.Functions.ClearRxBuffer((int)ChannelID);
                J2534Port.Functions.ClearTxBuffer((int)ChannelID);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
        }
        public override Response<int> Ioctl(int ioctlID, int input)
        {
            int result = 0;
            try
            {
                J2534Err jErr = J2534Port.Functions.IoctlPassthru(DeviceID, ioctlID, input, ref result);
                Debug.WriteLine("Ioctl " + ioctlID.ToString("X") + ": 0x" + input.ToString("X") + ", result: 0x" + result.ToString("X"));
                if (jErr == J2534Err.STATUS_NOERROR)
                    return new Response<int>(ResponseStatus.Success, result);
                else
                    return new Response<int>(ResponseStatus.Error, result);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return new Response<int>(ResponseStatus.Error, 0);
        }
        public override bool ConnectSecondaryProtocol(J2534InitParameters j2534Init)
        {
            try
            {
                if (j2534Init.UsePrimaryChannel)
                {
                    ChannelID2 = ChannelID;
                }
                else
                {
                    int ChID = 3;

                    string protoSpeedStr = j2534Init.Protocol.ToString() + j2534Init.Baudrate.ToString();
                    if (protocol_channel.ContainsKey(protoSpeedStr))
                    {
                        ChID = protocol_channel[protoSpeedStr];
                    }
                    else
                    {
                        OBDError = J2534Port.Functions.Connect((int)DeviceID, j2534Init.Protocol, j2534Init.Connectflag, (BaudRate)Enum.Parse(typeof(BaudRate), j2534Init.Baudrate), ref ChID);
                        if (OBDError != J2534Err.STATUS_NOERROR)
                        {
                            LoggerBold("Error setting protocol: " + OBDError.ToString());
                            return false;
                        }
                        protocol_channel.Add(protoSpeedStr, ChID);
                    }
                    ChannelID2 = ChID;
                }
                Protocol2 = j2534Init.Protocol;
                if (!SetConfigParams(j2534Init, ChannelID2))
                {
                    Logger("Failed to set parameters");
                    if (ChannelID != ChannelID2)
                    {
                        DisConnectSecondaryProtocol();
                    }
                    return false;
                }

                Logger("Secondary protocol connected: " + Protocol2.ToString() + ", ChannelID: " + ChannelID2.ToString());
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, ConnectSecondaryProtocol line " + line + ": " + ex.Message);
            }
            return false;
        }

        private bool SetConfig(SConfig[] sc, int ChID)
        {
            //SConfig sc = new SConfig((ConfigParameter)Parameter,Value);
            J2534Err m = J2534Port.Functions.SetConfig(ChID, ref sc);
            if (m != J2534Err.STATUS_NOERROR)
            {
                LoggerBold("Error setting config: " + m.ToString());
                return false;
            }
            return true;
        }

        public override bool SetLoggingFilter()
        {
            try
            {
                if (AppSettings.LoggerUseFilters == false)
                {
                    RemoveFilters(null);
                    this.CurrentFilter = FilterMode.Logging;
                    return true;
                }

                Debug.WriteLine("Set logging filter");
                ClearFilters();
                //Response<J2534Err> m = SetFilter(0xFEFFFF, 0x6CF010, 0, TxFlag.NONE, FilterType.PASS_FILTER);
                byte[] mask = new byte[] { 0xFF, 0x00 };
                byte[] pattern = new byte[] { 0xF0, 0x00 };
                Response<J2534Err> m = SetFilter(mask,pattern, TxFlag.NONE, FilterType.PASS_FILTER, ChannelID, Protocol);
                if (m.Status != ResponseStatus.Success)
                {
                    Debug.WriteLine("Failed to set filter, J2534 error: " + m.ToString());
                    return false;
                }
                Debug.WriteLine("Filter set");
                this.CurrentFilter = FilterMode.Logging;

                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        public override bool SetAnalyzerFilter()
        {
            try
            {
                Debug.WriteLine("Setting analyzer filter");

                ClearFilters();
                byte[] mask = new byte[] { 0 };
                byte[] pattern = new byte[] { 0 };

                Response<J2534Err> m = SetFilter(mask,pattern, TxFlag.NONE, FilterType.PASS_FILTER, ChannelID, Protocol);
                if (m.Status != ResponseStatus.Success)
                {
                    Debug.WriteLine("Failed to set filter, J2534 error: " + m.ToString());
                    return false;
                }
                Debug.WriteLine("Filter set");
                this.CurrentFilter = FilterMode.Analyzer;
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

        public override bool RemoveFilters(int[] filterIds)
        {
            try
            {
                if (filterIds == null)
                {
                    Logger("Removing all filters");
                    ClearFilters();
                    byte[] mask = new byte[] { 0 };
                    byte[] pattern = new byte[] { 0 };

                    Response<J2534Err> m = SetFilter(mask, pattern, TxFlag.NONE, FilterType.PASS_FILTER, ChannelID, Protocol);
                    if (m.Status != ResponseStatus.Success)
                    {
                        Debug.WriteLine("Failed to set filter, J2534 error: " + m.ToString());
                        return false;
                    }
                    Debug.WriteLine("Filter set");
                    this.CurrentFilter = FilterMode.None;
                }
                else
                {
                    for (int i = Filters.Count - 1; i >= 0; i--)
                    {
                        if (filterIds.Contains(Filters[i]))
                        {
                            Logger("Removing filter, ID: " + Filters[i].ToString());
                            OBDError = J2534Port.Functions.StopMsgFilter((int)ChannelID, (int)Filters[i]);
                            if (OBDError != J2534Err.STATUS_NOERROR)
                            {
                                return false;
                            }
                            Filters.RemoveAt(i);
                        }
                    }

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
                LoggerBold("Error, j2534Device line " + line + ": " + ex.Message);
            }
            return false;
        }

    }
}
