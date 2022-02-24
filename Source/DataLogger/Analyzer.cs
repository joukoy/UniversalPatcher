using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Helpers;
using static Upatcher;
using static UniversalPatcher.DataLogger;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO.Ports;

namespace UniversalPatcher
{
    public class Analyzer
    {
        public Dictionary<byte, string> FuncAddresses;
        public Dictionary<byte, string> PhysAddresses;
        public bool stoploop { get; set; }
        private bool HideHeartBeat;

        public Analyzer()
        {
            initvalues();
        }

        public class VPWRow
        {
            public long Timestamp { get; set; }
            public string Hdr { get; set; }
            public byte Priority { get; set; }
            public string Mode { get; set; }
            public string ModeType { get; set; }
            public byte[] Message { get; set; }
            public bool IsHeartBeat { get; set; }
        }

        public class AnalyzerData : EventArgs
        {
            //'Hdr', 'Prio', 'Mode', 'Type', 'TA', 'SA', 'Payload'
            public long Timestamp { get; set; }
            public string Hdr { get; set; }
            public string Prio { get; set; }
            public string Mode { get; set; }
            public string Type { get; set; }
            public string TA { get; set; }
            public string SA { get; set; }
            public string Payload { get; set; }

        }
        private void initvalues()
        {
            FuncAddresses = new Dictionary<byte, string>();
            FuncAddresses.Add(0x0B, "(C) Eng Air Intake");
            FuncAddresses.Add(0x12, "(C) Fuel");
            FuncAddresses.Add(0x14, "(C) AC Clutch");
            FuncAddresses.Add(0x1A, "(C) Engine RPM");
            FuncAddresses.Add(0x24, "(C) Wheels");
            FuncAddresses.Add(0x28, "(C) Vehicle Speed");
            FuncAddresses.Add(0x2A, "(C) Traction Control");
            FuncAddresses.Add(0x32, "(C) Brakes");
            FuncAddresses.Add(0x34, "(C) Steering");
            FuncAddresses.Add(0x3A, "(C) Trans");
            FuncAddresses.Add(0x48, "(C) Eng Coolant");
            FuncAddresses.Add(0x4A, "(C) Eng Oil");
            FuncAddresses.Add(0x52, "(C) Engine Sys");
            FuncAddresses.Add(0x58, "(C) Suspension");
            FuncAddresses.Add(0x62, "(C) Cruise Control");
            FuncAddresses.Add(0x72, "(C) Charging System");
            FuncAddresses.Add(0x7A, "(C) Odometer");
            FuncAddresses.Add(0x82, "(C) Fuel System");
            FuncAddresses.Add(0x84, "(C) Vehicle Motion");
            FuncAddresses.Add(0x86, "(C) Ign Switch");
            FuncAddresses.Add(0x92, "(C) Veh Security");
            FuncAddresses.Add(0x96, "(C) Chimes");
            FuncAddresses.Add(0xC6, "(C) Extern Access");
            FuncAddresses.Add(0xCE, "(C) MFG Specific");
            FuncAddresses.Add(0xD2, "(C) Restraints");
            FuncAddresses.Add(0xDA, "(C) Exterior Lamps");
            FuncAddresses.Add(0xDE, "(C) Interior Lamps");
            FuncAddresses.Add(0xE4, "(C) Tires");
            FuncAddresses.Add(0xE6, "(C) Defrost");
            FuncAddresses.Add(0xEA, "(C) MFG Specific");
            FuncAddresses.Add(0xF2, "(C) Ext Environment");
            FuncAddresses.Add(0xFA, "(C) VIN");
            FuncAddresses.Add(0xFE, "(C) Network Control");
            FuncAddresses.Add(0x13, "(S) Fuel");
            FuncAddresses.Add(0x15, "(S) AC Clutch");
            FuncAddresses.Add(0x1B, "(S) Engine RPM");
            FuncAddresses.Add(0x25, "(S) Wheels");
            FuncAddresses.Add(0x29, "(S) Vehicle Speed");
            FuncAddresses.Add(0x2B, "(S) Traction Control");           
            FuncAddresses.Add(0x33, "(S) Brakes");
            FuncAddresses.Add(0x35, "(S) Steering");
            FuncAddresses.Add(0x3B, "(S) Trans");
            FuncAddresses.Add(0x49, "(S) Eng Coolant");
            FuncAddresses.Add(0x4B, "(S) Eng Oil");
            FuncAddresses.Add(0x53, "(S) Engine Sys");
            FuncAddresses.Add(0x59, "(S) Suspension");
            FuncAddresses.Add(0x63, "(S) Cruise Control");
            FuncAddresses.Add(0x73, "(S) Charging System");
            FuncAddresses.Add(0x7B, "(S) Odometer");
            FuncAddresses.Add(0x83, "(S) Fuel System");
            FuncAddresses.Add(0x85, "(S) Vehicle Motion");
            FuncAddresses.Add(0x87, "(S) Ign Switch");
            FuncAddresses.Add(0x93, "(S) Veh Security");
            FuncAddresses.Add(0x97, "(S) Chimes");
            FuncAddresses.Add(0xC7, "(S) Extern Access");
            FuncAddresses.Add(0xCF, "(S) MFG Specific");
            FuncAddresses.Add(0xD3, "(S) Restraints");
            FuncAddresses.Add(0xDB, "(S) Exterior Lamps");
            FuncAddresses.Add(0xDF, "(S) Interior Lamps");
            FuncAddresses.Add(0xE5, "(S) Tires");
            FuncAddresses.Add(0xE7, "(S) Defrost");
            FuncAddresses.Add(0xEB, "(S) MFG Specific");
            FuncAddresses.Add(0xF3, "(S) Ext Environment");
            FuncAddresses.Add(0xFB, "(S) VIN");
            FuncAddresses.Add(0xFF, "(S) Network Control");

            PhysAddresses = new Dictionary<byte, string>();
            PhysAddresses.Add(0x10, "PCM");
            PhysAddresses.Add(0x28, "ABS");
            PhysAddresses.Add(0x40, "BCM");
            PhysAddresses.Add(0x58, "SRS");
            PhysAddresses.Add(0x60, "Cluster");
            PhysAddresses.Add(0x80, "Radio");
            PhysAddresses.Add(0x99, "HVAC");
            PhysAddresses.Add(0xA0, "LDCM");
            PhysAddresses.Add(0xA1, "RDCM");
            PhysAddresses.Add(0xA6, "SCM");
            PhysAddresses.Add(0xB0, "Remotes");
            PhysAddresses.Add(0xF1, "Ext Tool");
        }

        public event EventHandler<AnalyzerData> RowAnalyzed;

        protected virtual void OnRowAnalyzed(AnalyzerData e)
        {
            RowAnalyzed?.Invoke(this, e);
        }


        public void StartAnalyzer(string DevType, bool hideHeartBeat)
        {
            try
            {
                if (LoggerUtils.analyzerData == null)
                    LoggerUtils.analyzerData = new List<OBDMessage>();
                HideHeartBeat = hideHeartBeat;
                stoploop = false;
                Logger("Waiting data...");
                LogDevice.SetTimeout(TimeoutScenario.Maximum);
                LogDevice.SetAnalyzerFilter();
                ThreadPool.QueueUserWorkItem(new WaitCallback(analyzerloop), null);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("StartAnalyzer line " + line + ": " + ex.Message);
            }
        }

        public void StopAnalyzer()
        {
            stoploop = true;            
        }

        public VPWRow ProcessLine(OBDMessage msg)
        {
            VPWRow vRow = new VPWRow();
            try
            {
                if (msg.Length < 3)
                    return null;
                Debug.WriteLine("Analyzer processing: " + msg.ToString());
                byte priority = (byte)(msg[2] >> 5);
                string mode = "F";
                string modeType = "?";
                if ((msg[0] & 0x04) != 0)
                    mode = "P";
                byte modeByte = (byte)(msg[0] & 0x0F);
                if (modeByte == 0x0C)
                    modeType = "N-N";

                switch (msg[0] & 0x0F)
                {
                    case 0x08:
                        modeType = "Func";
                        break;
                    case 0x09:
                        modeType = "Broadcast";
                        break;
                    case 0x0A:
                        modeType = "Query";
                        break;
                    case 0x0B:
                        modeType = "Read";
                        break;
                }

                if ((msg[0] & 0x10) == 0x10)
                    mode = "?H";
                if ((msg[0] & 0x08) == 0x00)
                    mode = "?IFR";

                // Check if is a heart beat
                bool isHeartBeat = false;
                if (msg[3] == 0xFF || msg[3] == 0xFE)
                    if (msg.Length == 6 && msg[5] == 0x03)
                        isHeartBeat = true;
                vRow.Timestamp = (long)msg.TimeStamp;
                vRow.Priority = priority;
                vRow.Mode = mode;
                vRow.ModeType = modeType;
                vRow.Message = msg.GetBytes();
                vRow.IsHeartBeat = isHeartBeat;
                vRow.Hdr = msg[0].ToString("X");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, Analyzer ProcessLine line " + line + ": " + ex.Message);

            }
            return vRow;
        }

        public AnalyzerData AnalyzeMsg(VPWRow vRow)
        {
            AnalyzerData azd = new AnalyzerData();
            try
            {
                azd.TA = "NA";
                azd.SA = "NA";
                byte addr = vRow.Message[1];
                if (vRow.Mode == "F")
                {
                    if (FuncAddresses.ContainsKey(addr))
                        azd.TA = addr.ToString("X2") + " " + FuncAddresses[addr];
                    else
                        azd.TA = addr.ToString("X2");
                }
                else
                {
                    if (PhysAddresses.ContainsKey(addr))
                        azd.TA = addr.ToString("X2") + " " + PhysAddresses[addr];
                    else
                        azd.TA = addr.ToString("X2");
                }
                addr = vRow.Message[2];
                if (PhysAddresses.ContainsKey(addr))
                    azd.SA = addr.ToString("X2") + " " + PhysAddresses[addr];
                else
                    azd.SA = addr.ToString("X2");
                azd.Payload = "";
                for (int p = 3; p < vRow.Message.Length; p++)
                {
                    azd.Payload += vRow.Message[p].ToString("X2") + " ";
                }
                azd.Timestamp = vRow.Timestamp;
                azd.Payload = azd.Payload.Trim();
                azd.Mode = vRow.Mode;
                azd.Prio = vRow.Priority.ToString("X");
                azd.Type = vRow.ModeType;
                azd.Hdr = vRow.Hdr;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, AnalyzeMsg line " + line + ": " + ex.Message);

            }
            return azd;
        }

        private void analyzerloop(object threadContext)
        {
            LogDevice.ClearMessageBuffer();
            LogDevice.ClearMessageQueue();
            bool waiting4x = false;

            while (!stoploop)
            {
                try
                {
                    OBDMessage rcv = LogDevice.ReceiveMessage();
                    if (rcv == null)
                    {
                        continue;
                    }
                    if (LogDevice.LogDeviceType == LoggingDevType.Elm && rcv.ElmPrompt)
                    {
                        LogDevice.SetAnalyzerFilter();
                    }
                    if (rcv.Length > 3)
                    {
                        if (rcv[1] == 0xfe && rcv[3] == 0xa0)
                        {
                            Debug.WriteLine("Received 0xFE, , 0xA0 - Ready to switch to 4x");
                            waiting4x = true;
                        }
                        if (waiting4x && rcv[1] == 0xfe && rcv[3] == 0xa1)
                        {
                            waiting4x = false;
                            Debug.WriteLine("Received 0xFE, , 0xA1 - switching to 4x");
                            LogDevice.Enable4xReadWrite = true;
                            if (LogDevice.SetVpwSpeed(VpwSpeed.FourX))
                                Debug.WriteLine("Switched to 4X");
                            else
                                Debug.WriteLine("Switch to 4X failed");
                        }

                        Debug.WriteLine("ByteArray: " + rcv.ToString());
                        VPWRow vRow = ProcessLine(rcv);
                        if (vRow != null)
                        {
                            if (vRow.IsHeartBeat && HideHeartBeat)
                            {
                                Debug.WriteLine("Hiding Heartbeat: " + rcv.ToString());
                            }
                            else
                            {
                                AnalyzerData msg = AnalyzeMsg(vRow);
                                LoggerUtils.analyzerData.Add(rcv);
                                OnRowAnalyzed(msg);
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
                    Debug.WriteLine("Error, analyzerloop line " + line + ": " + ex.Message);
                    //Thread.Sleep(100);
                }
            }
            try
            {
                Debug.WriteLine("Exit from analyzer loop");
                LogDevice.SetLoggingFilter();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, analyzerloop line " + line + ": " + ex.Message);
            }
        }

    }
}
