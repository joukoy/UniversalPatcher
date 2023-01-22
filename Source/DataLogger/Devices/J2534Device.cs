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
        public List<ulong> Filters;
        private int DeviceID;
        private int ChannelID;
        private int ChannelID2;
        private ProtocolID Protocol;
        private ProtocolID Protocol2;
        public bool IsProtocolOpen;
        private const string PortName = "J2534";
        public string ToolName = "";
        private int periodigMsgId = -1;
        private int periodigMsgId2 = -1;
        public const string DeviceType = "J2534";
        ulong timeDiff = long.MaxValue;
        long lastTstamp = long.MaxValue;
        readonly ulong ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

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
            this.LogDeviceType = DataLogger.LoggingDevType.Other;
            ChannelID = -1;
            ChannelID2 = -1;
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
        public override bool Initialize(int BaudRate, LoggerUtils.J2534InitParameters j2534Init) //Baudrate not really used
        {
            return this.InitializeInternal(j2534Init);
        }

        private void SetTimeDiff()
        {
            try
            {
                ClearMessageBuffer();
                //byte[] queryMsg = { Priority.Physical0High, DeviceId.Broadcast, DeviceId.Tool, 0x20 };
                //bool m = SendMessage(new OBDMessage(queryMsg), 1);

                for (int retry=0;retry<1000;retry++)
                {
                    int NumMessages = 1;
                    List<PassThruMsg> rxMsgs = new List<PassThruMsg>();
                    J2534Err jErr = J2534Port.Functions.ReadMsgs((int)ChannelID, ref rxMsgs, ref NumMessages, 1000);
                    if (jErr == J2534Err.STATUS_NOERROR)
                    {
                        if (NumMessages > 0)
                        {
                            timeDiff = (ulong)DateTime.Now.Ticks - ((ulong)rxMsgs.Last().Timestamp * ticksPerMicrosecond);
                            Debug.WriteLine("Time diff (function): " + timeDiff.ToString() + " retries: " + retry.ToString());
                        }
                        lastTstamp = rxMsgs.Last().Timestamp;
                        break;
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

        // This returns 'bool' for the sake of readability. That bool needs to be
        // wrapped in a Task object for the public Initialize method.
        private bool InitializeInternal(LoggerUtils.J2534InitParameters j2534Init)
        {
            try
            {
                Filters = new List<ulong>();

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

                Debug.WriteLine("Connected to the device.");

                //Optional.. read API,firmware version ect here
                //read voltage
                volts = ReadVoltage();
                if (volts.Status != ResponseStatus.Success)
                {
                    Logger("Unable to read battery voltage.");
                }
                else
                {
                    Logger("Battery Voltage is: " + volts.Value.ToString());
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
                Debug.WriteLine("Protocol Set");
                J2534FunctionsIsLoaded = true;
                //receiveTask = Task.Factory.StartNew(() => DataReceiver());
                //SetTimeDiff();
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
                    Receive();
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


        /// <summary>
        /// Set J2534 parameters
        /// </summary>
        private bool SetConfigParams(J2534InitParameters j2534Init, int ChID)
        {
            try
            {
                if (!string.IsNullOrEmpty(j2534Init.Sconfigs))
                {
                    Debug.WriteLine("Setting Sconfigs: " + j2534Init.Sconfigs);
                    string[] scParts = j2534Init.Sconfigs.Split('|');
                    foreach (string scp in scParts)
                    {
                        string[] parts = scp.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            ConfigParameter cp = (ConfigParameter)Enum.Parse(typeof(ConfigParameter), parts[0]);
                            int val = 0;
                            HexToInt(parts[1], out val);
                            if (cp != ConfigParameter.NONE)
                            {
                                SConfig sconfig = new SConfig(cp, val);
                                SConfig[] sc = new SConfig[1];
                                sc[0] = sconfig;
                                SetConfig(sc, ChID);
                            }
                        }
                    }
                }
                if (j2534Init.Kinit == LoggerUtils.KInit.FastInit_J1979 || j2534Init.Kinit == LoggerUtils.KInit.FastInit_GMDelco || j2534Init.Kinit == LoggerUtils.KInit.FastInit_ME7_5)
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
                else if (j2534Init.Kinit == LoggerUtils.KInit.FiveBaudInit_J1979)
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
                    byte[] data = j2534Init.PerodicMsg.Replace(" ", "").ToBytes();
                    PassThruMsg txMsg = new PassThruMsg(j2534Init.Protocol, 0, data);
                    int pMsgId = -1;
                    J2534Err jErr = J2534Port.Functions.StartPeriodicMsg(ChID, txMsg, ref pMsgId, j2534Init.PeriodicInterval);
                    if (jErr != J2534Err.STATUS_NOERROR)
                    {
                        Logger("Periodic message error: " + jErr.ToString());
                        //return false;
                    }
                    if (ChID == ChannelID)
                    {
                        Debug.WriteLine("Started periodic message: "+ j2534Init.PerodicMsg + " for protocol 1, ID: " + pMsgId.ToString());
                        periodigMsgId = pMsgId;
                    }
                    else
                    {
                        Debug.WriteLine("Started periodic message: " + j2534Init.PerodicMsg + " for protocol 2, ID: " + pMsgId.ToString());
                        periodigMsgId2 = pMsgId;
                    }
                }
                if (string.IsNullOrEmpty(j2534Init.PassFilters))
                {
                    byte[] mask = new byte[] { 0 };
                    byte[] pattern = new byte[] { 0 };
                    Response<J2534Err> m = SetFilter(mask, pattern, 0, TxFlag.NONE, FilterType.PASS_FILTER, ChID, j2534Init.Protocol);
                    if (m.Status != ResponseStatus.Success)
                    {
                        LoggerBold("Failed to set filter, J2534 error: " + m.ToString());
                        return false;
                    }
                }
                else
                {
                    string[] filters = j2534Init.PassFilters.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string filter in filters)
                    {
                        string[] fParts = filter.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (fParts.Length == 2 || fParts.Length == 3)
                        {
                            FilterType ftype = FilterType.PASS_FILTER;
                            if (fParts.Length == 3)
                            {
                                if (fParts[2].ToLower().Contains("block"))
                                    ftype = FilterType.BLOCK_FILTER;
                                else if (fParts[2].ToLower().Contains("flow"))
                                    ftype = FilterType.FLOW_CONTROL_FILTER;

                            }
                            byte[] mask = fParts[0].Replace(" ","").ToBytes();
                            byte[] pattern = fParts[1].Replace(" ", "").ToBytes();
                            Response<J2534Err> m = SetFilter(mask, pattern, 0, TxFlag.NONE, ftype, ChID, j2534Init.Protocol);
                            if (m.Status != ResponseStatus.Success)
                            {
                                LoggerBold("Failed to set filter, J2534 error: " + m.ToString());
                                return false;
                            }
                            Debug.WriteLine("Filter set");
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
            Debug.WriteLine("Setting read timeout to: " + timeout.ToString());
            ReadTimeout = timeout;
        }


        /// <summary>
        /// This will process incoming messages for up to 500ms looking for a message
        /// </summary>
        public Response<OBDMessage> FindResponse(OBDMessage expected)
        {
            //Debug.WriteLine("FindResponse called");
            for (int iterations = 0; iterations < 5; iterations++)
            {
                OBDMessage response = this.ReceiveMessage();
                    if (Utility.CompareArraysPart(response.GetBytes(), expected.GetBytes()))
                        return Response.Create(ResponseStatus.Success, response);
                Thread.Sleep(100);
            }

            return Response.Create(ResponseStatus.Timeout, (OBDMessage)null);
        }

        /// <summary>
        /// Read an network packet from the interface, and return a Response/Message
        /// </summary>
        public override void Receive()
        {
            //Debug.WriteLine("Trace: Read Network Packet");
            try
            {
                int NumMessages = 1;
                List<PassThruMsg> rxMsgs = new List<PassThruMsg>();
                J2534Err jErr;
                Application.DoEvents();
                ulong tNow = (ulong)DateTime.Now.Ticks;
                jErr = J2534Port.Functions.ReadMsgs((int)ChannelID, ref rxMsgs, ref NumMessages, ReadTimeout);
                if (jErr == J2534Err.STATUS_NOERROR)
                {
                    if (timeDiff == long.MaxValue)
                    {
                        SetTimeDiff();
                    }
                    if (rxMsgs.Last().Timestamp < lastTstamp && NumMessages > 0)
                    {
                        timeDiff = tNow - ((ulong)rxMsgs.Last().Timestamp * ticksPerMicrosecond);
                        Debug.WriteLine("Time diff: " + timeDiff.ToString());
                    }
                    lastTstamp = rxMsgs.Last().Timestamp;
                    for (int m = 0; m < rxMsgs.Count; m++)
                    {
                        //Debug.WriteLine("RX: " + rxMsgs[m].Data.ToHex()); //Debug messages hang program, maybe too frequent?
                        //this.Enqueue(new OBDMessage(rxMsgs[m].Data, (ulong)rxMsgs[m].Timestamp, (ulong)OBDError));
                        OBDMessage oMsg = new OBDMessage(rxMsgs[m].Data, (ulong)tNow, (ulong)OBDError);
                        oMsg.DevTimeStamp = (ulong)rxMsgs[m].Timestamp * ticksPerMicrosecond;
                        oMsg.TimeStamp = (ulong)(oMsg.DevTimeStamp + timeDiff);
                        this.Enqueue(oMsg);
                    }
                }
                else
                {
                    if (jErr != J2534Err.ERR_BUFFER_EMPTY)
                    {
                        Debug.WriteLine("Receive error: " + jErr);
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
                Application.DoEvents();
                if (ChannelID2 > -1)
                {
                    jErr = J2534Port.Functions.ReadMsgs((int)ChannelID2, ref rxMsgs, ref NumMessages, ReadTimeout);
                    if (jErr == J2534Err.STATUS_NOERROR)
                    {
                        for (int m = 0; m < rxMsgs.Count; m++)
                        {
                            //Debug.WriteLine("RX: " + rxMsgs[m].Data.ToHex()); //Debug messages hang program, maybe too frequent?
                            //this.Enqueue(new OBDMessage(rxMsgs[m].Data, (ulong)rxMsgs[m].Timestamp, (ulong)OBDError));
                            OBDMessage msg = new OBDMessage(rxMsgs[m].Data, (ulong)DateTime.Now.Ticks, (ulong)OBDError);
                            msg.SecondaryProtocol = true;
                            this.Enqueue(msg);
                        }
                    }
                    else
                    {
                        if (jErr != J2534Err.ERR_BUFFER_EMPTY)
                        {
                            Debug.WriteLine("Receive error: " + jErr);
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
        /// Send a message, wait for a response, return the response.
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
                    proto = Protocol2;
                    chid = ChannelID2;
                }
                PassThruMsg TempMsg = new PassThruMsg(proto, TxFlag.NONE, message.GetBytes());
                int NumMsgs = 1;

                message.DevTimeStamp = (ulong)DateTime.Now.Ticks - timeDiff;
                this.MessageSent(message);
                Application.DoEvents();
                OBDError = J2534Port.Functions.WriteMsgs((int)chid, TempMsg, ref NumMsgs, WriteTimeout);
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    //Debug messages here...check why failed..
                    string errMsg = "";
                    J2534Port.Functions.GetLastError(ref errMsg);
                    Debug.WriteLine("J2534 WriteMsgs error: " + errMsg);
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
                    return Response.Create(ResponseStatus.Success, true);
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

                Process proc = Process.GetCurrentProcess();
                foreach (ProcessModule dll in proc.Modules)
                {
                    if (dll.FileName == J2534Port.LoadedDevice.FunctionLibrary)
                    {
                        return true;
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
                DeviceID = 0;
                ChannelID = 0;
                Filters.Clear();
                OBDError = 0;
                int tmpId = 0;
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
                    Response.Create(ResponseStatus.Success, OBDError);
                }
                if (ChannelID2 > -1)
                {
                    DisconnectFromSecondProtocol();
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
                int tmpChannel = 0;
                OBDError = J2534Port.Functions.Connect((int)DeviceID, ReqProtocol, ConnectFlags, Speed, ref tmpChannel);
                if (OBDError != J2534Err.STATUS_NOERROR) return Response.Create(ResponseStatus.Error, OBDError);
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
                Debug.WriteLine("Disconnecting primary protocol");

                if (periodigMsgId > -1)
                {
                    Debug.WriteLine("Stopping perodic message, id: " + periodigMsgId.ToString());
                    J2534Port.Functions.StopPeriodicMsg(ChannelID, periodigMsgId);
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

        /// <summary>
        /// Disconnect from Second protocol
        /// </summary>
        private Response<J2534Err> DisconnectFromSecondProtocol()
        {
            try
            {
                Debug.WriteLine("Disconnecting secondary protocol");
                if (periodigMsgId2 > -1)
                {
                    Debug.WriteLine("Stopping perodic message, id: " + periodigMsgId2.ToString());
                    J2534Port.Functions.StopPeriodicMsg(ChannelID2, periodigMsgId2);
                }
                OBDError = J2534Port.Functions.Disconnect((int)ChannelID2);
                ChannelID2 = -1;
                if (OBDError != J2534Err.STATUS_NOERROR)
                {
                    Debug.WriteLine("Error disconnecting protocol: " + OBDError.ToString());
                    return Response.Create(ResponseStatus.Error, OBDError);
                }
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
        /// Disconnect from Second protocol
        /// </summary>
        public override bool SetupFilters(string Filters, bool Secondary)
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
                J2534Port.Functions.ClearMsgFilters(ChID);
                string[] filters = Filters.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string filter in filters)
                {
                    string[] fParts = filter.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fParts.Length == 2 || fParts.Length == 3)
                    {
                        FilterType ftype = FilterType.PASS_FILTER;
                        if (fParts.Length == 3)
                        {
                            if (fParts[2].ToLower().Contains("block"))
                                ftype = FilterType.BLOCK_FILTER;
                            else if (fParts[2].ToLower().Contains("flow"))
                                ftype = FilterType.FLOW_CONTROL_FILTER;

                        }
                        byte[] mask = fParts[0].Replace(" ", "").ToBytes();
                        byte[] pattern = fParts[1].Replace(" ", "").ToBytes();
                        Response<J2534Err> m = SetFilter(mask, pattern, 0, TxFlag.NONE, ftype, ChID, Proto);
                        if (m.Status != ResponseStatus.Success)
                        {
                            LoggerBold("Failed to set filter, J2534 error: " + m.ToString());
                            return false;
                        }
                        Debug.WriteLine("Filter set");
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
        /// Set filter
        /// </summary>
        private Response<J2534Err> SetFilter(byte[] Mask,byte[] Pattern,UInt32 FlowControl,TxFlag txflag,FilterType Filtertype, int ChID, ProtocolID proto)
        {
            try
            {
                PassThruMsg FlowMsg = new PassThruMsg();
                PassThruMsg maskMsg = new PassThruMsg(proto, txflag, Mask);
                PassThruMsg patternMsg = new PassThruMsg(proto, txflag, Pattern);
                //PassThruMsg maskMsg = new PassThruMsg(proto, txflag, new Byte[] { (byte)(0xFF & (Mask >> 24)), (byte)(0xFF & (Mask >> 16)), (byte)(0xFF & (Mask >> 8)), (byte)(0xFF & Mask) });
                //PassThruMsg patternMsg = new PassThruMsg(proto, txflag, new Byte[] { (byte)(0xFF & (Pattern >> 24)), (byte)(0xFF & (Pattern >> 16)), (byte)(0xFF & (Pattern >> 8)), (byte)(0xFF & Pattern) });

                int tempfilter = 0;
                Debug.WriteLine("Setting filter, Mask: " + BitConverter.ToString(Mask) + ", Pattern: " + BitConverter.ToString(Pattern));
                //OBDError = J2534Port.Functions.StartMsgFilter((int)ChannelID, Filtertype, ref maskMsg, ref patternMsg, ref FlowMsg, ref tempfilter);
                OBDError = J2534Port.Functions.StartMsgFilter(ChID, Filtertype, maskMsg, patternMsg, ref tempfilter);

                if (OBDError != J2534Err.STATUS_NOERROR) return Response.Create(ResponseStatus.Error, OBDError);
                Filters.Add((ulong)tempfilter);
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
                /*            for (int i = 0; i < Filters.Count; i++)
                            {
                                OBDError = J2534Port.Functions.StopMsgFilter((int)ChannelID, (int)Filters[i]);
                                if (OBDError != J2534Err.STATUS_NOERROR)
                                {
                                    Response.Create(ResponseStatus.Error, OBDError);
                                }
                            }
                */
                OBDError = J2534Port.Functions.ClearMsgFilters(ChannelID);
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
        /// Set the interface to low (false) or high (true) speed
        /// </summary>
        /// <remarks>
        /// The caller must also tell the PCM to switch speeds
        /// </remarks>
        protected override bool SetVpwSpeedInternal(VpwSpeed newSpeed)
        {
            try
            {
                if (newSpeed == VpwSpeed.Standard)
                {
                    Debug.WriteLine("J2534 setting VPW 1X");
                    //Disconnect from current protocol
                    DisconnectFromProtocol();
                    //Connect at new speed
                    ConnectToProtocol(ProtocolID.J1850VPW, BaudRate.J1850VPW_10400, ConnectFlag.NONE, ref ChannelID);
                    if (CurrentFilter == "analyzer")
                        SetAnalyzerFilter();
                    else
                        SetLoggingFilter();
                }
                else
                {
                    Debug.WriteLine("J2534 setting VPW 4X");
                    //Disconnect from current protocol
                    DisconnectFromProtocol();
                    //Connect at new speed
                    ConnectToProtocol(ProtocolID.J1850VPW, BaudRate.J1850VPW_41600, ConnectFlag.NONE, ref ChannelID);

                    //Set Filter
                    //SetFilter(0xFEFFFF, 0x6CF010, 0, TxFlag.NONE, FilterType.PASS_FILTER);
                    if (CurrentFilter == "analyzer")
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

        public override bool DisConnectSecondaryProtocol()
        {
            if( DisconnectFromSecondProtocol().Status == ResponseStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool ConnectSecondaryProtocol(J2534InitParameters j2534Init)
        {
            try
            {
                int ChID = 0;
                Response<J2534Err> m = ConnectToProtocol(j2534Init.Protocol, (BaudRate)Enum.Parse(typeof(BaudRate), j2534Init.Baudrate), j2534Init.Connectflag, ref ChID);
                if (m.Status != ResponseStatus.Success)
                {
                    LoggerBold("Error setting protocol: " + m.ToString());
                    return false;
                }
                ChannelID2 = ChID;
                Protocol2 = j2534Init.Protocol;
                if (!SetConfigParams(j2534Init, ChannelID2))
                {
                    Logger("Failed to set parameters");
                    DisconnectFromSecondProtocol();
                    return false;
                }

                Logger("Secondary protocol connected: " + Protocol.ToString() + ", ChannelID: " + ChID.ToString());
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
                    RemoveFilters();
                    this.CurrentFilter = "logging";
                    return true;
                }

                Debug.WriteLine("Set logging filter");
                ClearFilters();
                //Response<J2534Err> m = SetFilter(0xFEFFFF, 0x6CF010, 0, TxFlag.NONE, FilterType.PASS_FILTER);
                byte[] mask = new byte[] { 0xFF, 0x00 };
                byte[] pattern = new byte[] { 0xF0, 0x00 };
                Response<J2534Err> m = SetFilter(mask,pattern, 0, TxFlag.NONE, FilterType.PASS_FILTER, ChannelID, Protocol);
                if (m.Status != ResponseStatus.Success)
                {
                    Debug.WriteLine("Failed to set filter, J2534 error: " + m.ToString());
                    return false;
                }
                Debug.WriteLine("Filter set");
                this.CurrentFilter = "logging";

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

                Response<J2534Err> m = SetFilter(mask,pattern, 0, TxFlag.NONE, FilterType.PASS_FILTER, ChannelID, Protocol);
                if (m.Status != ResponseStatus.Success)
                {
                    Debug.WriteLine("Failed to set filter, J2534 error: " + m.ToString());
                    return false;
                }
                Debug.WriteLine("Filter set");
                this.CurrentFilter = "analyzer";
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

        public override bool RemoveFilters()
        {
            try
            {
                Debug.WriteLine("Removing filters");
                ClearFilters();
                byte[] mask = new byte[] { 0 };
                byte[] pattern = new byte[] { 0 };

                Response<J2534Err> m = SetFilter(mask, pattern, 0, TxFlag.NONE, FilterType.PASS_FILTER, ChannelID, Protocol);
                if (m.Status != ResponseStatus.Success)
                {
                    Debug.WriteLine("Failed to set filter, J2534 error: " + m.ToString());
                    return false;
                }
                Debug.WriteLine("Filter set");
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
