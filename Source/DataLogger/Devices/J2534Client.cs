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
    class J2534Client : Device
    {
        /// <summary>
        /// Configuration settings
        /// </summary>
        public int ReadTimeout = 500;
        public int WriteTimeout = 500;

        /// <summary>
        /// variety of properties used to id channels, filters and status
        /// </summary>
        private const string PortName = "J2534";
        public string ToolName = "";
        public const string DeviceType = "J2534";

        private NamedPipeClientStream cmdPipe;
        private NamedPipeClientStream responsePipe;
        private NamedPipeClientStream proto2CmdPipe;
        private NamedPipeClientStream proto2ResponsePipe;
        private NamedPipeClientStream loggerPipe;

        //Handle to j2534 server process
        Process process;

        //public J2534Client(J2534DotNet.J2534Device jport) : base()
        public J2534Client(int DevNumber) : base()
        {

            // Reduced from 4096+12 for the MDI2
            this.MaxSendSize = 2048 + 12;    // J2534 Standard is 4KB
            this.MaxReceiveSize = 2048 + 12; // J2534 Standard is 4KB
            this.Supports4X = true;
            this.LogDeviceType = DataLogger.LoggingDevType.Other;

            process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = Application.ExecutablePath;
            process.StartInfo.Arguments = "j2534server " + DevNumber.ToString();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Debug.WriteLine("Starting j2534 server");
            process.Start();
            Application.DoEvents();
            Thread.Sleep(300);
            int pId = process.Id;
            cmdPipe = new NamedPipeClientStream(".", pId.ToString() + "j2534cmdpipe", PipeDirection.InOut,PipeOptions.WriteThrough);
            cmdPipe.Connect();            
            responsePipe = new NamedPipeClientStream(".", pId.ToString() + "j2534responsepipe", PipeDirection.InOut, PipeOptions.WriteThrough);
            responsePipe.Connect();
            responsePipe.ReadMode = PipeTransmissionMode.Message;
            proto2CmdPipe = new NamedPipeClientStream(".", pId.ToString() + "proto2cmdpipe", PipeDirection.InOut, PipeOptions.WriteThrough);
            proto2CmdPipe.Connect();
            proto2ResponsePipe = new NamedPipeClientStream(".", pId.ToString() + "proto2responsepipe", PipeDirection.InOut, PipeOptions.WriteThrough);
            proto2ResponsePipe.Connect();
            proto2ResponsePipe.ReadMode = PipeTransmissionMode.Message;
            loggerPipe = new NamedPipeClientStream(".", pId.ToString() + "j2534loggerpipe", PipeDirection.InOut, PipeOptions.WriteThrough);
            loggerPipe.Connect();
            loggerPipe.ReadMode = PipeTransmissionMode.Message;

            //Task.Factory.StartNew(() => ReadMsgLoop());
            Task.Factory.StartNew(() => LoggerLoop());
        }

        private byte[] PipeSendAndReceive(j2534Command command, byte[] msg)
        {
            byte[] retVal = null;
            try
            {
                List<byte> sendBuf = new List<byte>();
                sendBuf.Add((byte)command);
                if (msg != null)
                {
                    sendBuf.AddRange(msg);
                }
                lock (cmdPipe)
                {
                    cmdPipe.Write(sendBuf.ToArray(), 0, sendBuf.Count);
                    cmdPipe.Flush();
                    retVal = ReceiveFromPipe(responsePipe);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, j2534Client line " + line + ": " + ex.Message);
                this.Connected = false;
            }
            return retVal;
        }

        private byte[] PipeSendAndReceiveProto2(j2534Command command, byte[] msg)
        {
            byte[] retVal = null;
            try
            {
                List<byte> sendBuf = new List<byte>();
                sendBuf.Add((byte)command);
                if (msg != null)
                {
                    sendBuf.AddRange(msg);
                }
                lock (proto2CmdPipe)
                {
                    proto2CmdPipe.Write(sendBuf.ToArray(), 0, sendBuf.Count);
                    proto2CmdPipe.Flush();
                    retVal = ReceiveFromPipe(proto2ResponsePipe);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, j2534Client line " + line + ": " + ex.Message);
                this.Connected = false;
            }
            return retVal;
        }


        private byte[] ReceiveFromPipe(NamedPipeClientStream pipe)
        {
            List<byte> msg = new List<byte>();
            try
            {
                byte[] b = new byte[1];
                do
                {
                    pipe.Read(b, 0, 1);
                    msg.Add(b[0]);
                }
                while (!pipe.IsMessageComplete);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, j2534Client line " + line + ": " + ex.Message);
            }
            return msg.ToArray();
        }


        private void LoggerLoop()
        {
            try
            {
                while (true)
                {
                    byte[] msg = ReceiveFromPipe(loggerPipe);
                    if (msg != null)
                    {
                        string logTxt = Encoding.ASCII.GetString(msg);
                        Logger(logTxt,false);
                    }
                    if (!loggerPipe.IsConnected)
                    {
                        Debug.WriteLine("J2534 client: connection to server closed");
                        this.Dispose();
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
                Debug.WriteLine("Error, j2534Client line " + line + ": " + ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Connected = false;
                if (cmdPipe.IsConnected)
                {
                    Task.Factory.StartNew(() =>
                    {
                        byte[] sendBuf = new byte[1];
                        sendBuf[0] = (byte)j2534Command.quit;
                        try
                        {
                            Debug.WriteLine("J2534 Client: Sending quit message to server");
                            cmdPipe.Write(sendBuf, 0, sendBuf.Length);
                            Thread.Sleep(100);
                            Debug.WriteLine("J2534 Client: Done sending quit message to server");
                        }
                        catch { }
                    }).Wait(500);
                }
                if (process != null && !process.HasExited)
                    process.Kill();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, j2534Client line " + line + ": " + ex.Message);
            }
        }

        public override string ToString()
        {
            return "J2534 Client Device";
        }

        // This needs to return Task<bool> for consistency with the Device base class.
        // However it doesn't do anything asynchronous, so to make the code more readable
        // it just wraps a private method that does the real work and returns a bool.
        public override bool Initialize(int BaudRate, J2534InitParameters j2534Init) //Baudrate not used
        {
            return this.InitializeInternal(j2534Init);
        }

        // This returns 'bool' for the sake of readability. That bool needs to be
        // wrapped in a Task object for the public Initialize method.
        private bool InitializeInternal(J2534InitParameters j2534Init)
        {
            try
            {
                Logger("J2534 client initializing...");
                Application.DoEvents();
                string initparms = Helpers.SerializeObjectToXML(j2534Init);
                byte[] param = Encoding.ASCII.GetBytes(initparms);
                byte[] readBuf = PipeSendAndReceive(j2534Command.Initialize, param);
                if (readBuf.Length > 0 && readBuf[0] == 1)
                {
                    //double volts = BitConverter.ToDouble(readBuf,0);
                    //Logger("Battery Voltage is: " + volts.ToString());
                    Logger("J2534 client: Device initialization complete.");
                    Application.DoEvents();
                    this.Connected = true;
                    return true;
                }
                else
                {
                    Logger("J2534 client: Device initialization fail.");
                    Application.DoEvents();
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
                LoggerBold("Error, j2534Client line " + line + ": " + ex.Message);
                Exception ie = ex.InnerException;
                LoggerBold("Error: " + line + ": " + ie.Message);
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
            byte[] param = BitConverter.GetBytes(timeout);
            byte[] readBuf = PipeSendAndReceive(j2534Command.SetWriteTimeout, param);
        }

        public override void SetReadTimeout(int timeout)
        {
            Debug.WriteLine("Setting read timeout to: " + timeout.ToString());
            ReadTimeout = timeout;
            byte[] param = BitConverter.GetBytes(timeout);
            byte[] readBuf = PipeSendAndReceive(j2534Command.SetReadTimeout, param);
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
                byte[] resp = PipeSendAndReceive(j2534Command.Receive, null);
                if (resp.Length == 1)
                {
                    if (resp[0] == 0)
                    {
                        Debug.WriteLine("J2534 Client: Receive failed");
                    }
                    return;
                }
                //OBDMessage oMsg = new OBDMessage(null);
                //oMsg.FromPipeMessage(resp);
                OBDMessage oMsg = Helpers.XmlDeserializeFromString<OBDMessage>(Encoding.ASCII.GetString(resp));
                this.Enqueue(oMsg);
                //Application.DoEvents();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, j2534Client line " + line + ": " + ex.Message);
                Application.DoEvents();
                Thread.Sleep(500); //Throttling...
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
                //byte[] resp = PipeSendAndReceive(j2534Command.Receive2, null);
                byte[] resp = PipeSendAndReceiveProto2(j2534Command.Receive2, null);
                if (resp.Length > 1)
                {
                    //OBDMessage oMsg = new OBDMessage(null);
                    //oMsg.FromPipeMessage(resp);
                    OBDMessage oMsg = Helpers.XmlDeserializeFromString<OBDMessage>(Encoding.ASCII.GetString(resp));
                    this.Enqueue(oMsg);
                }
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

        /// <summary>
        /// Send a message, wait for a response, return the response.
        /// </summary>
        public override bool SendMessage(OBDMessage message, int responses)
        {
            try
            {
                Debug.WriteLine("J2534 client sending message: " + message.ToString());
                this.MessageSent(message);
                string msgStr = Helpers.SerializeObjectToXML<OBDMessage>(message);
                //Logger("Client message: " + message.ToString());
                byte[] readBuf = PipeSendAndReceive(j2534Command.SendMessage, Encoding.ASCII.GetBytes(msgStr));
                if (readBuf[0] == 1)
                {
                    //Logger("J2534 client Message sent ok");
                    return true;
                }
                else
                {
                    Logger("J2534 client Message sent fail.");
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
                LoggerBold("Error, j2534Client line " + line + ": " + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Disconnect from Second protocol
        /// </summary>
        public override bool SetupFilters(string Filters, bool Secondary)
        {
            try
            {                 
                byte[] param = Encoding.ASCII.GetBytes(Filters);
                byte[] sendBuf = new byte[param.Length + 1];
                sendBuf[0] = Convert.ToByte(Secondary); 
                //boolean length is known, send it first. Rest of bytes are filter text
                Array.Copy(param, 0, sendBuf, 1, param.Length);
                byte[] readBuf = PipeSendAndReceive(j2534Command.SetupFilters, sendBuf);
                if (Convert.ToBoolean(readBuf[0]) == true)
                {
                    //Logger("J2534 client Filter setup complete.");
                    return true;
                }
                else
                {
                    Logger("J2534 client Filter setup fail.");
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
                LoggerBold("Error, j2534Client line " + line + ": " + ex.Message);
            }
            return false;
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
                if ((newSpeed == VpwSpeed.FourX) && !this.Enable4xReadWrite)
                {
                    return false;
                }
                Debug.WriteLine("Setting vpwspeed");
                byte[] param = new byte[1];
                param[0] = (byte)newSpeed;
                byte[] readBuf = PipeSendAndReceive(j2534Command.SetVpwSpeed, param);
                return Convert.ToBoolean(readBuf[0]);
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
            return false;
        }

        public override void ClearMessageBuffer()
        {
            try
            {
                byte[] readBuf = PipeSendAndReceive(j2534Command.ClearMessageBuffer, null);
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

        public override bool DisConnectSecondaryProtocol()
        {
            byte[] readBuf = PipeSendAndReceive(j2534Command.DisconnectSecondayProtocol, null);
            return Convert.ToBoolean(readBuf[0]);
        }

        public override bool ConnectSecondaryProtocol(J2534InitParameters j2534Init)
        {
            try
            {
                Debug.WriteLine("Connecting secondary protocol");
                byte[] param = Helpers.ObjectToByteArray(j2534Init);
                byte[] readBuf = PipeSendAndReceive(j2534Command.ConnectSecondaryProtocol, param);
                return Convert.ToBoolean(readBuf[0]);
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


        public override bool SetLoggingFilter()
        {
            try
            {
                Debug.WriteLine("Setting logging filter");
                byte[] readBuf = PipeSendAndReceive(j2534Command.SetLoggingFilter, null);
                return Convert.ToBoolean(readBuf[0]);
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
            return false;
        }

        public override bool SetAnalyzerFilter()
        {
            try
            {
                Debug.WriteLine("Setting analyzer filter");
                byte[] readBuf = PipeSendAndReceive(j2534Command.SetAnalyzerFilter, null);
                return Convert.ToBoolean(readBuf[0]);
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
            return false;
        }

        public override bool RemoveFilters()
        {
            try
            {
                Debug.WriteLine("Removing filters");
                byte[] readBuf = PipeSendAndReceive(j2534Command.RemoveFilters, null);
                return Convert.ToBoolean(readBuf[0]);
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
            return false;
        }

    }
}
