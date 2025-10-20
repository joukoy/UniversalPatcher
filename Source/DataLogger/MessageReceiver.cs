﻿using System;
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
                {
                    device.SetReadTimeout(AppSettings.TimeoutAnalyzerReceive);
                }
                else
                {
                    if (device.LogDeviceType == DataLogger.LoggingDevType.J2534)
                        device.SetReadTimeout(AppSettings.TimeoutJConsoleReceive);
                    else
                        device.SetReadTimeout(AppSettings.TimeoutConsoleReceive);
                }
                if (device.LogDeviceType != DataLogger.LoggingDevType.J2534 && device.LogDeviceType != DataLogger.LoggingDevType.UPX_OBD )
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
                    if (device.LogDeviceType == DataLogger.LoggingDevType.Elm || device.LogDeviceType == DataLogger.LoggingDevType.Obdlink)
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
                if (!ReceiveLoopRunning) // || device.LogDeviceType == DataLogger.LoggingDevType.J2534) 
                {
                    return true;
                }
                Debug.WriteLine("Pausing receiver, secondary: " + secondaryProtocol.ToString());
                ReceiverPaused = true;
                StopReceiveLoop();
                Application.DoEvents();
                if (device.LogDeviceType == DataLogger.LoggingDevType.J2534)  //For J-tools (Old code)
                {
                    device.SetReadTimeout(AppSettings.TimeoutJConsoleReceive);
                }
                else
                {
                    device.SetReadTimeout(AppSettings.TimeoutSerialPortRead);
                }
                return true;
            }
            else
            {
                if (ReceiverPaused)
                {
                    Debug.WriteLine("Continue receiving, secondary: " + secondaryProtocol.ToString());
                    if (analyzerMode)
                    {
                        device.SetReadTimeout(AppSettings.TimeoutAnalyzerReceive);
                    }
                    else
                    {
                        if (device.LogDeviceType == DataLogger.LoggingDevType.J2534)  
                        {
                            device.SetReadTimeout(AppSettings.TimeoutJConsoleReceive);
                        }
                        else
                        {
                            device.SetReadTimeout(AppSettings.TimeoutConsoleReceive);
                        }
                    }
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
            List<OBDMessage> messages = new List<OBDMessage>();
            while (!receiverToken.IsCancellationRequested)
            {
                try
                {
                    if (!device.Connected && AppSettings.LoggerAutoDisconnect)
                    {                   
                        LoggerBold("Device disconnected, stopping receiver");
                        device.Dispose();
                        device = null;
                        return;
                    }
                    if (secondaryProtocol)
                    {
                        messages = device.ReceiveMultipleMessage2(AppSettings.AnalyzerNumMessages);
                    }
                    else
                    {
                        messages = device.ReceiveMultipleMessages(AppSettings.AnalyzerNumMessages, true);
                        for (int m = 0; m < messages.Count; m++)
                        {
                            OBDMessage msg = messages[m];
                            if (msg != null && msg.ElmPrompt)
                            {
                                Debug.WriteLine("Resetting analyzer filter");
                                device.SetAnalyzerFilter();
                                do
                                {
                                    msg = device.ReceiveMessage(true);
                                } while (msg != null);
                            }
                        }
                    }
                    if (messages.Count == 0)
                    {
                        Thread.Sleep(100);
                    }
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
