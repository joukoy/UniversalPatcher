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
        private Device LogDevice;
        public bool running = true;

        public J2534Server(string DevName) 
        {
            List<J2534DotNet.J2534Device> jDevList = J2534DotNet.J2534Detect.ListDevices();
            J2534DotNet.J2534Device selectedDevice = jDevList.First();
            foreach (J2534DotNet.J2534Device jDev in jDevList)
            {
                if (jDev.Name == DevName)
                {
                    selectedDevice = jDev;
                    break;
                }
            }
            this.LogDevice = new J2534Device(selectedDevice);

            loggerPipe = new NamedPipeServerStream(ProcessId.ToString() + "j2534loggerpipe", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.WriteThrough);
        }

        private void SendToPipe(NamedPipeServerStream pipe, byte[] msg)
        {
            Task.Factory.StartNew(() =>
            {
                pipe.Write(msg, 0, msg.Length);
                pipe.Flush();
            }).Wait(5000);
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

        public void ServerLoop()
        {
            try
            {
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
                                        Logger("quit\n");
                                        running = false;
                                        break;
                                    case j2534Command.Initialize:
                                        string initparms = Encoding.ASCII.GetString(data);
                                        J2534InitParameters j2534Init = (J2534InitParameters)Helpers.XmlDeserializeFromString<J2534InitParameters>(initparms);
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
                                        Logger("Initialized");
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
                                        OBDMessage oMsg = LogDevice.ReceiveMessage();
                                        if (oMsg != null)
                                        {
                                            retVal = oMsg.ToPipeMessage();
                                        }
                                        break;
                                    case j2534Command.Receive2:
                                        LogDevice.Receive2();
                                        oMsg = LogDevice.ReceiveMessage();
                                        if (oMsg != null)
                                        {
                                            retVal = oMsg.ToPipeMessage();
                                        }
                                        break;
                                    case j2534Command.SendMessage:
                                        OBDMessage message = new OBDMessage(null);
                                        message.FromPipeMessage(data);
                                        if (!LogDevice.SendMessage(message, 1))
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    case j2534Command.SetupFilters:
                                        bool secondary = Convert.ToBoolean(data[0]);
                                        byte[] tmp = new byte[data.Length - 1];
                                        Array.Copy(data, 1, tmp, 0, tmp.Length);
                                        string filters = System.Text.Encoding.Default.GetString(tmp);
                                        Logger("J2534 server setting filters " + filters + ", secondary: " + secondary.ToString());
                                        if (!LogDevice.SetupFilters(filters, secondary))
                                        {
                                            Logger("J2534 server: Filter setup failed");
                                            retVal = failMessage;
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
                                        if (!LogDevice.SetAnalyzerFilter())
                                        {
                                            retVal = failMessage;
                                        }
                                        break;
                                    default:
                                        LoggerBold("Unknown command!" + msg[0].ToString("X2"));
                                        retVal = failMessage;
                                        break;
                                }
                                SendToPipe(responsePipeServer, retVal);
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
                                SendToPipe(responsePipeServer, failMessage);
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
            Application.DoEvents();
            Logger("Server Quits");
            Thread.Sleep(5000);
            Environment.Exit(0);
        }

    }
}
