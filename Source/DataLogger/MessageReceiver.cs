using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.Windows.Forms;

namespace UniversalPatcher
{
    public class MessageReceiver
    {
        private CancellationTokenSource receiverTokenSource = new CancellationTokenSource();
        private CancellationToken receiverToken;
        private bool ReceiverPaused = false;
        public bool ReceiveLoopRunning = false;
        private Task ReceiverTask;
        private Device device;
        private IPort port;
        private bool secondaryProtocol;
        private bool analyzerMode;

        public void StartReceiveLoop(Device device, IPort port, bool secondary, bool analyzermode)
        {
            try
            {
                this.device = device;
                this.port = port;
                this.secondaryProtocol = secondary;
                this.analyzerMode = analyzermode;
                if (ReceiveLoopRunning)
                {
                    return;
                }
                if (analyzerMode)
                    device.SetReadTimeout(AppSettings.TimeoutAnalyzerReceive);
                else
                    device.SetReadTimeout(AppSettings.TimeoutConsoleReceive);
                if (device.LogDeviceType != DataLogger.LoggingDevType.Other)
                {
                    device.SetTimeout(TimeoutScenario.DataLogging3);
                    device.SetAnalyzerFilter();
                }
                receiverTokenSource = new CancellationTokenSource();
                receiverToken = receiverTokenSource.Token;
                ReceiverTask = Task.Factory.StartNew(() => ReceiveLoop(), receiverToken);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, StartReceiveLoop line " + line + ": " + ex.Message);
            }
        }

        public void StopReceiveLoop()
        {
            try
            {
                if (ReceiveLoopRunning)
                {
                    ReceiveLoopRunning = false;
                    receiverTokenSource.Cancel();                    
                    Application.DoEvents();
                    if (port != null)
                    {
                        port.CancelReceive();
                    }
                    if (device.LogDeviceType == DataLogger.LoggingDevType.Other)
                    {
                        //ReceiverTask.Wait(100);
                    }
                    else
                    {
                        datalogger.StopElmReceive();
                        //ReceiverTask.Wait(5000);
                    }
                    ReceiverTask.Wait(100);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, StopReceiveLoop line " + line + ": " + ex.Message);
            }
        }

        public bool SetReceiverPaused(bool Pause)
        {
            if (Pause)
            {
                if (!ReceiveLoopRunning) 
                {
                    return true;
                }
                Debug.WriteLine("Pausing receiver, scondary: " + secondaryProtocol.ToString());
                ReceiverPaused = true;
                StopReceiveLoop();
                Application.DoEvents();
                if (port == null)  //For J-tools
                {
                    device.SetReadTimeout(AppSettings.TimeoutConsoleReceive);
                }
                else
                {
                    device.SetReadTimeout(AppSettings.TimeoutReceive);
                }
                return true;
            }
            else
            {
                if (ReceiverPaused)
                {
                    Debug.WriteLine("Continue receiving, secondary: " + secondaryProtocol.ToString());
                    if (analyzerMode)
                        device.SetReadTimeout(AppSettings.TimeoutAnalyzerReceive);
                    else
                        device.SetReadTimeout(AppSettings.TimeoutConsoleReceive);
                    StartReceiveLoop(device, port, secondaryProtocol, analyzerMode);
                    ReceiverPaused = false;
                }
                return true;
            }
        }

        private void ReceiveLoop()
        {
            ReceiveLoopRunning = true;
            if (secondaryProtocol)
            {
                Debug.WriteLine("Starting receive loop for secondary protocol");
            }
            else
            {
                Debug.WriteLine("Starting receive loop");
            }
            //while (Connected && ReceiveLoopRunning)
            OBDMessage msg = device.ReceiveMessage();
            while (!receiverToken.IsCancellationRequested)
            {
                try
                {
                    if (!device.Connected && AppSettings.LoggerAutoDisconnect)
                    {                   
                        LoggerBold("Device disconnected, stopping receiver");
                        device.Dispose();
                        return;
                    }
                    if (msg != null && msg.ElmPrompt)
                    {
                        Debug.WriteLine("Resetting analyzer filter");
                        device.SetAnalyzerFilter();
                    }
                    if (secondaryProtocol)
                    {
                        device.Receive2();
                    }
                    else
                    {
                        msg = device.ReceiveMessage();
                    }
                    //Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Receiveloop: " + ex.Message);
                }
            }
            Debug.WriteLine("Receive loop end");
            ReceiveLoopRunning = false;
        }
    }
}
