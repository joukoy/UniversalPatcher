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

        public void StartReceiveLoop()
        {
            try
            {
                if (datalogger.LogRunning || ReceiveLoopRunning)
                {
                    return;
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
                    ReceiverTask.Wait(300);
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
                return true;
            }
            else
            {
                if (ReceiverPaused)
                {
                    Logger("Continue receiving");
                    StartReceiveLoop();
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
            while (!receiverToken.IsCancellationRequested)
            {
                try
                {
                    datalogger.LogDevice.ReceiveMessage();
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
