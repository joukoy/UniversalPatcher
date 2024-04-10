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
using System.IO.Pipes;

namespace UniversalPatcher
{
    /// <summary>
    /// This class encapsulates all code that is unique to the J2534 interface.
    /// </summary>
    ///

    class J2534Server 
    {
        //private NamedPipeServerStream msgReceivePipe;
        private NamedPipeServerStream loggerPipe;
        private NamedPipeServerStream msgEventPipe;
        private Device LogDevice;
        public bool running = true;
        private Queue<string> msgEventQueue = new Queue<string>();

        public J2534Server(J2534DotNet.J2534Device jDevice) 
        {
            this.LogDevice = new J2534Device(jDevice);
            loggerPipe = new NamedPipeServerStream(ProcessId.ToString() + "j2534loggerpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough);
            msgEventPipe = new NamedPipeServerStream(ProcessId.ToString() + "j2534msgeventpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough);
        }

        private byte[] ReceiveFromPipe(NamedPipeServerStream pipe)
        {
            List<byte> msg = new List<byte>();
            byte[] b = new byte[1];
            do
            {
                pipe.Read(b, 0, 1);
                msg.Add(b[0]);
            }
            while (!pipe.IsMessageComplete);
            return msg.ToArray();
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            Task.Factory.StartNew(() =>
            {
                byte[] msg = Encoding.ASCII.GetBytes(e.LogText);
                loggerPipe.Write(msg, 0, msg.Length);
                loggerPipe.Flush();
            });
        }

        private void LogDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            string msg = "R" + Helpers.SerializeObjectToXML<OBDMessage>(e.Msg);
            msgEventQueue.Enqueue(msg);
        }

        private void LogDevice_MsgSent(object sender, MsgEventparameter e)
        {
            string msg = "S" + Helpers.SerializeObjectToXML<OBDMessage>(e.Msg);
            msgEventQueue.Enqueue(msg);
        }

        private void MessageEventLoop()
        {
            while (running)
            {
                try
                {
                    if (msgEventQueue.Count > 0)
                    {
                        string msg = msgEventQueue.Dequeue();
                        msgEventPipe.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                        msgEventPipe.Flush();
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    var frame = st.GetFrame(st.FrameCount - 1);
                    var line = frame.GetFileLineNumber();
                    Debug.WriteLine("Error, j2534Server line " + line + ": " + ex.Message);
                }
            }
        }

        public void ServerLoop()
        {
            try
            {
                Task.Factory.StartNew(() => Protocol2Loop());
                Task.Factory.StartNew(() => MessageEventLoop());
                using (NamedPipeServerStream cmdPipeServer = new NamedPipeServerStream(ProcessId.ToString() + "j2534cmdpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough))
                {
                    using (NamedPipeServerStream responsePipeServer = new NamedPipeServerStream(ProcessId.ToString() + "j2534responsepipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough))
                    {
                        
                        int readTimeout = 100;
                        byte[] okMessage = new byte[] { 1 };
                        byte[] failMessage = new byte[] { 0 };

                        Logger("Server is waiting for a client");
                        Application.DoEvents();
                        cmdPipeServer.WaitForConnection();
                        responsePipeServer.WaitForConnection();
                        //msgReceivePipe.WaitForConnection();
                        loggerPipe.WaitForConnection();
                        msgEventPipe.WaitForConnection();
                        Logger("Server have connection from client");
                        uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
                        Application.DoEvents();

                        while (running)
                        {
                            try
                            {
                                byte[] msg = ReceiveFromPipe(cmdPipeServer);
                                if (!cmdPipeServer.IsConnected)
                                {
                                    LoggerBold("Pipe closed?");
                                    break;
                                }
                                //First byte = Command, other bytes = parameters
                                j2534Command cmd = (j2534Command)msg[0];
                                //Logger("cmd: " + cmd.ToString());
                                //Application.DoEvents();
                                byte[] data = new byte[1];
                                if (msg.Length > 1)
                                {
                                    data = new byte[msg.Length - 1];
                                    Array.Copy(msg, 1, data, 0, data.Length);
                                }
                                byte[] retVal = okMessage;
                                switch (cmd)
                                {
                                    case j2534Command.quit:
                                        //Logger("quit\n");
                                        running = false;
                                        break;
                                    case j2534Command.Initialize:
                                        string initparms = Encoding.ASCII.GetString(data);
                                        J2534InitParameters j2534Init = Helpers.XmlDeserializeFromString<J2534InitParameters>(initparms);
                                        //Logger("Filters: " + j2534Init.PassFilters);
                                        //Logger("Initializing...");
                                        Application.DoEvents();
                                        if (!LogDevice.Initialize(0, j2534Init))
                                        {
                                            Logger("J2534 server: Device initialization failed");
                                            Application.DoEvents();
                                            running = false;
                                            retVal = failMessage;
                                        }
                                        else
                                        {
                                            LogDevice.MsgSent += LogDevice_MsgSent;
                                            LogDevice.MsgReceived += LogDevice_MsgReceived;
                                        }
                                        Application.DoEvents();
                                        break;
                                    case j2534Command.SetWriteTimeout:
                                        int wtimeout = BitConverter.ToInt32(data, 0);
                                        LogDevice.SetWriteTimeout(wtimeout);
                                        break;
                                    case j2534Command.SetReadTimeout:
                                        readTimeout = BitConverter.ToInt32(data, 0);
                                        LogDevice.SetReadTimeout(readTimeout);
                                        break;
                                    case j2534Command.Receive:
                                        bool waitfortimeout = Convert.ToBoolean(data[0]);
                                        OBDMessage oMsg = LogDevice.ReceiveMessage(waitfortimeout);                                        
                                        if (oMsg != null)
                                        {
                                            Debug.WriteLine("Jserver Received msg: " + oMsg.ToString());
                                            string msgStr = Helpers.SerializeObjectToXML<OBDMessage>(oMsg);
                                            retVal = Encoding.ASCII.GetBytes(msgStr);
                                        }
                                        break;
                                    case j2534Command.ReceiveBufferedMessages:
                                        LogDevice.ReceiveBufferedMessages();
                                        break;
                                    case j2534Command.Receive2:
                                        //LogDevice.Receive2();
                                        oMsg = LogDevice.ReceiveMessage2();
                                        if (oMsg != null)
                                        {
                                            //retVal = oMsg.ToPipeMessage();
                                            string msgStr = Helpers.SerializeObjectToXML<OBDMessage>(oMsg);
                                            retVal = Encoding.ASCII.GetBytes(msgStr);
                                        }
                                        break;
                                    case j2534Command.SendMessage:
                                        //OBDMessage message = new OBDMessage(null);
                                        //message.FromPipeMessage(data);
                                        Debug.WriteLine("Msg send start");
                                        OBDMessage message = Helpers.XmlDeserializeFromString<OBDMessage>(Encoding.ASCII.GetString(data));
                                        //Logger("Server message: " + message.ToString());
                                        bool m = LogDevice.SendMessage(message, 1);
                                        if (!m)
                                        {
                                            retVal = failMessage;
                                        }
                                        Debug.WriteLine("Msg send end");
                                        break;
                                    case j2534Command.SetupFilters:
                                        bool secondary = Convert.ToBoolean(data[0]);
                                        bool clearold = Convert.ToBoolean(data[1]);
                                        byte[] tmp = new byte[data.Length - 2];
                                        Array.Copy(data, 2, tmp, 0, tmp.Length);
                                        string filters = System.Text.Encoding.Default.GetString(tmp);
                                        Logger("J2534 server setting filters: " + filters + ", secondary: " + secondary.ToString());
                                        int[] filterIds = LogDevice.SetupFilters(filters, secondary, clearold);
                                        if (filterIds == null)
                                        {
                                            Logger("J2534 server: Filter setup failed");
                                            retVal = failMessage;
                                        }
                                        else
                                        {
                                            retVal = new byte[filterIds.Length * 4];
                                            Buffer.BlockCopy(filterIds, 0, retVal, 0, retVal.Length);
                                        }
                                        Logger("J2534 server filters set");
                                        break;
                                    case j2534Command.ClearMessageBuffer:
                                        LogDevice.ClearMessageBuffer();
                                        break;
                                    case j2534Command.DisconnectSecondayProtocol:
                                        if (!LogDevice.DisConnectSecondaryProtocol())
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.Disconnect:
                                        LogDevice.Disconnect();
                                        break;
                                    case j2534Command.ConnectSecondaryProtocol:
                                        J2534InitParameters jParams = (J2534InitParameters)Helpers.ByteArrayToObject(data);
                                        if (!LogDevice.ConnectSecondaryProtocol(jParams))
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.SetLoggingFilter:
                                        if (!LogDevice.SetLoggingFilter())
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.SetAnalyzerFilter:
                                        if (!LogDevice.SetAnalyzerFilter())
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.RemoveFilters:
                                        int[] fIds = null;
                                        if (data != null && data.Length > 3)
                                        {
                                            fIds = new int[data.Length / 4];
                                            for (int i=0;(i * 4) < data.Length; i++)
                                                fIds[i] = BitConverter.ToInt32(data, i*4);
                                        }
                                        if (!LogDevice.RemoveFilters(fIds))
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.SetVpwSpeed:
                                        VpwSpeed vpwspeed = (VpwSpeed)data[0];
                                        Logger("Setting VPW speed: " + vpwspeed.ToString());
                                        LogDevice.Enable4xReadWrite = true; //If setting is disabled, client will not send request
                                        if (!LogDevice.SetVpwSpeed(vpwspeed))
                                        {
                                            Logger("Setting VPW speed failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.SetProgramminVoltage:
                                        PinNumber pin = (PinNumber)data[0];
                                        uint volts = BitConverter.ToUInt32(data,1);
                                        Logger("Setting programminvoltage, pin: " + pin.ToString() + ", volts:" + volts.ToString());
                                        if (!LogDevice.SetProgramminVoltage(pin,volts))
                                        {
                                            Logger("Setting programming voltage failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.AddToFunctMsgLookupTable:
                                        Logger("Adding to functional addresses: " + BitConverter.ToString(data));
                                        secondary = Convert.ToBoolean(data.Last());
                                        Array.Resize(ref data, data.Length - 1);
                                        if (!LogDevice.AddToFunctMsgLookupTable(data, secondary))
                                        {
                                            Logger("Add to functional addresses failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.DeleteFromFunctMsgLookupTable:
                                        Logger("Deleting from functional addresses: " + BitConverter.ToString(data));
                                        secondary = Convert.ToBoolean(data.Last());
                                        Array.Resize(ref data, data.Length - 1);
                                        if (!LogDevice.DeleteFromFunctMsgLookupTable(data, secondary))
                                        {
                                            Logger("Delete from functional addresses failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.ClearFunctMsgLookupTable:
                                        Logger("Clearing functional addresses");
                                        secondary = Convert.ToBoolean(data[0]);
                                        if (!LogDevice.ClearFunctMsgLookupTable(secondary))
                                        {
                                            Logger("Clear functional addresses failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.SetJ2534Configs:
                                        secondary = Convert.ToBoolean(data.Last());
                                        Array.Resize(ref data, data.Length - 1);
                                        string cfg = System.Text.Encoding.Default.GetString(data);
                                        //Logger("Setting J2534 config parameters: " + cfg);
                                        if (!LogDevice.SetJ2534Configs(cfg, secondary))
                                        {
                                            Logger("Setting J2534 config parameters failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.StartPeriodicMsg:
                                        secondary = Convert.ToBoolean(data.Last());
                                        Array.Resize(ref data, data.Length - 1);
                                        string pMsg = System.Text.Encoding.Default.GetString(data);
                                        if (!LogDevice.StartPeriodicMsg(pMsg, secondary))
                                        {
                                            Logger("Starting periodic message failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.StopPeriodicMsg:
                                        secondary = Convert.ToBoolean(data.Last());
                                        Array.Resize(ref data, data.Length - 1);
                                        pMsg = System.Text.Encoding.Default.GetString(data);
                                        if (!LogDevice.StopPeriodicMsg(pMsg, secondary))
                                        {
                                            Logger("Stopping periodic message failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.ClearPeriodicMsg:
                                        Logger("Clearing periodic messages");
                                        secondary = Convert.ToBoolean(data[0]);
                                        if (!LogDevice.ClearPeriodicMsg(secondary))
                                        {
                                            Logger("Clear periodic messages failed!");
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.Ioctl:
                                        int ioctlFunc = BitConverter.ToInt32(data, 0);
                                        int input = BitConverter.ToInt32(data, 4);
                                        Response<int> resp = LogDevice.Ioctl(ioctlFunc, input);
                                        if (resp.Status == ResponseStatus.Success)
                                        {
                                            retVal = BitConverter.GetBytes(resp.Value);
                                        }
                                        else
                                        {
                                            Logger("Ioctl fail");
                                            retVal = failMessage;
                                        }
                                        break;
                                    default:
                                        LoggerBold("Unknown command!" + msg[0].ToString("X2"));
                                        retVal = failMessage;
                                        break;
                                }
                                if (cmd != j2534Command.quit)
                                {
                                    responsePipeServer.Write(retVal, 0, retVal.Length);
                                }
                            }
                            catch (Exception ex)
                            {
                                var st = new StackTrace(ex, true);
                                var frame = st.GetFrame(st.FrameCount - 1);
                                var line = frame.GetFileLineNumber();
                                Logger("Error, j2534Server line " + line + ": " + ex.Message);
                                if (!responsePipeServer.IsConnected)
                                {
                                    break;
                                }
                                else
                                {
                                    responsePipeServer.Write(failMessage, 0, failMessage.Length);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                Logger("Error, j2534Server line " + line + ": " + ex.Message);
            }
            Logger("J2534 Server Quits");
            LogDevice.Dispose();
            Application.DoEvents();
            Thread.Sleep(100);
            Application.DoEvents();
            Environment.Exit(0);
        }


        public void Protocol2Loop()
        {
            try
            {
                using (NamedPipeServerStream cmdPipeServer = new NamedPipeServerStream(ProcessId.ToString() + "proto2cmdpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough))
                {
                    using (NamedPipeServerStream responsePipeServer = new NamedPipeServerStream(ProcessId.ToString() + "proto2responsepipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough))
                    {
                        byte[] okMessage = new byte[] { 1 };
                        byte[] failMessage = new byte[] { 0 };

                        Logger("Protocol2 is waiting for a client");
                        Application.DoEvents();
                        cmdPipeServer.WaitForConnection();
                        responsePipeServer.WaitForConnection();
                        Logger("Protocol2 have connection from client");
                        Application.DoEvents();

                        while (running)
                        {
                            try
                            {
                                byte[] msg = ReceiveFromPipe(cmdPipeServer);
                                if (!cmdPipeServer.IsConnected)
                                {
                                    LoggerBold("Protocol2 pipe closed?");
                                    break;
                                }
                                //First byte = Command, other bytes = parameters
                                j2534Command cmd = (j2534Command)msg[0];
                                //Logger("cmd: " + cmd.ToString());
                                //Application.DoEvents();
                                byte[] data = new byte[1];
                                if (msg.Length > 1)
                                {
                                    data = new byte[msg.Length - 1];
                                    Array.Copy(msg, 1, data, 0, data.Length);
                                }
                                byte[] retVal = okMessage;
                                switch (cmd)
                                {
                                    case j2534Command.Receive2:
                                        OBDMessage oMsg = LogDevice.ReceiveMessage2();
                                        if (oMsg != null)
                                        {
                                            //Logger("Received: " + oMsg.ToString());
                                            //retVal = oMsg.ToPipeMessage();
                                            string msgStr = Helpers.SerializeObjectToXML<OBDMessage>(oMsg);
                                            retVal = Encoding.ASCII.GetBytes(msgStr);
                                        }
                                        break;
                                    default:
                                        LoggerBold("Unknown command!" + msg[0].ToString("X2"));
                                        retVal = failMessage;
                                        break;
                                }
                                responsePipeServer.Write(retVal, 0, retVal.Length);
                            }
                            catch (Exception ex)
                            {
                                var st = new StackTrace(ex, true);
                                var frame = st.GetFrame(st.FrameCount - 1);
                                var line = frame.GetFileLineNumber();
                                Logger("Error, protocol2 line " + line + ": " + ex.Message);
                                if (!responsePipeServer.IsConnected)
                                {
                                    break;
                                }
                                else
                                {
                                    responsePipeServer.Write(failMessage, 0, failMessage.Length);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var frame = st.GetFrame(st.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                Logger("Error, j2534Server line " + line + ": " + ex.Message);
            }
            Logger("J2534 Server Quits");
            Application.DoEvents();
            Thread.Sleep(1000);
            Environment.Exit(0);
        }


    }
}
