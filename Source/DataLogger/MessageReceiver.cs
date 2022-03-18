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

        public void StartReceiveLoop(Device dev, IPort port)
        {
            try
            {
                this.device = dev;
                this.port = port;
                if ( ReceiveLoopRunning)
                {
                    return;
                }
                if (dev.LogDeviceType != DataLogger.LoggingDevType.Other)
                {
                    dev.SetTimeout(TimeoutScenario.DataLogging3);
                    dev.SetAnalyzerFilter();
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
                Logger("Pausing receiver...");
                ReceiverPaused = true;
                StopReceiveLoop();
                Application.DoEvents();
                if (port != null)  //Not for J-tools
                {
                    device.SetReadTimeout(500);
                }
                return true;
            }
            else
            {
                if (ReceiverPaused)
                {
                    Logger("Continue receiving");
                    if (port != null)
                    {
                        device.SetReadTimeout(2000);
                    }
                    StartReceiveLoop(device, port);
                    ReceiverPaused = false;
                }
                return true;
            }
        }

        private void ReceiveLoop()
        {
            ReceiveLoopRunning = true;
            Debug.WriteLine("Starting receive loop");
            //while (Connected && ReceiveLoopRunning)
            OBDMessage msg = device.ReceiveMessage();
            while (!receiverToken.IsCancellationRequested)
            {
                try
                {
                    if (msg != null && msg.ElmPrompt)
                    {
                        Debug.WriteLine("Resetting analyzer filter");
                        device.SetAnalyzerFilter();
                    }
                    msg = device.ReceiveMessage();
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
