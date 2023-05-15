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
using System.Threading.Tasks;

namespace UniversalPatcher
{
    public class Analyzer
    {
        public Dictionary<byte, string> FuncAddresses;
        public Dictionary<byte, string> PhysAddresses;
        private bool HideHeartBeat;
        private bool waiting4x = false;


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

        public class IdName
        {
            public IdName() { }
            public IdName(byte Id, string Name) 
            {
                this.ID = Id;
                this.Name = Name;
            }
            public byte ID { get; set; }
            public string Name { get; set; }
            public IdName ShallowCopy()
            {
                return (IdName)this.MemberwiseClone();
            }

        }

        private void initvalues()
        {
            FuncAddresses = new Dictionary<byte, string>();
            if (FuncNames != null && FuncNames.Count > 0)
            {
                for (int i = 0; i < FuncNames.Count; i++)
                    FuncAddresses.Add(FuncNames[i].ID, FuncNames[i].Name);
            }
            else
            {

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
            }

            PhysAddresses = new Dictionary<byte, string>();
            if (DeviceNames != null && DeviceNames.Count > 0)
            {
                for (int i = 0; i < DeviceNames.Count; i++)
                    PhysAddresses.Add(DeviceNames[i].ID, DeviceNames[i].Name);
            }
            else
            {
                PhysAddresses.Add(0x10, "[10] PCM");
                PhysAddresses.Add(5, "[5] Info display Mod");
                PhysAddresses.Add(0x11, "[11] ECM");
                PhysAddresses.Add(0x13, "[13] FPCM/Chassis");
                PhysAddresses.Add(0x14, "[14] Reductant Cont Mod");
                PhysAddresses.Add(0x15, "[15] Fuel Pump Driver Mod");
                PhysAddresses.Add(0x16, "[16] GPCM");
                PhysAddresses.Add(0x17, "[17] Hybrid PIM");
                PhysAddresses.Add(0x1A, "[1A] TCCM");
                PhysAddresses.Add(0x18, "[18] TCM");
                PhysAddresses.Add(0x19, "[19] Pwr Take off");
                PhysAddresses.Add(0x21, "[21] Adaptive Cruise");
                PhysAddresses.Add(0x22, "[22] Chassis Cont Mod");
                PhysAddresses.Add(0x24, "[24] VCIM");
                PhysAddresses.Add(0x25, "[25] Aux Chassis");
                PhysAddresses.Add(0x28, "[28] EBCM");
                PhysAddresses.Add(0x29, "[29] Adaptive Cruise");
                PhysAddresses.Add(0x30, "[30] PSCM");
                PhysAddresses.Add(0x31, "[31] PSCM");
                PhysAddresses.Add(0x32, "[32] Rear steering");
                PhysAddresses.Add(0x34, "[34] Steering wheel angle");
                PhysAddresses.Add(0x37, "[37] RDCM");
                PhysAddresses.Add(0x38, "[38] Susp Cont Mod");
                PhysAddresses.Add(0x39, "[39] Susp Cont");
                PhysAddresses.Add(0x40, "[40] BCM");
                PhysAddresses.Add(0x43, "[43] Aux BCM");
                PhysAddresses.Add(0x44, "[44] BECM");
                PhysAddresses.Add(0x45, "[45] SDGM");
                PhysAddresses.Add(0x58, "[58] SDM");
                PhysAddresses.Add(0x59, "[59] PPS");
                PhysAddresses.Add(0x60, "[60] IPC");
                PhysAddresses.Add(0x61, "[61] Info Display Mod");
                PhysAddresses.Add(0x62, "[62] Heads up Display");
                PhysAddresses.Add(0x64, "[64] Rearview Camera");
                PhysAddresses.Add(0x65, "[65] Night Vision");
                PhysAddresses.Add(0x66, "[66] Radio/Hvac Conts");
                PhysAddresses.Add(0x68, "[68] HVAC Conts");
                PhysAddresses.Add(0x69, "[69] Data recorder");
                PhysAddresses.Add(0x72, "[72] Headlamp Cont Mod");
                PhysAddresses.Add(0x73, "[73] Headlamp H Beam Cont Mod");
                PhysAddresses.Add(0x74, "[74] Headlamp Cont Mod");
                PhysAddresses.Add(0x75, "[75] Trailer Interface Cont Mod");
                PhysAddresses.Add(0x80, "[80] Radio");
                PhysAddresses.Add(0x81, "[81] Amplifier");
                PhysAddresses.Add(0x85, "[85] Rear Audio Cont Mod");
                PhysAddresses.Add(0x89, "[89] XM Mod");
                PhysAddresses.Add(0x95, "[95] memory seat interface Mod");
                PhysAddresses.Add(0x97, "[97] VCIM"); // 99 HVAC Cont Mod
                PhysAddresses.Add(0x99, "[99] HVAC Cont Mod");
                PhysAddresses.Add(0x0C, "[0C] B195A Nitrogen Oxides Sen 1");
                PhysAddresses.Add(0x0D, "[0D] B195B Nitrogen Oxides Sen 2");
                PhysAddresses.Add(0x0E, "[0E] B136 Exh Particulate Matter Sen");
                PhysAddresses.Add(0x0F, "[0F] M103 Turbo Vane Pos Act");
                PhysAddresses.Add(0x1B, "[1B] Rear Diff Clutch Cont Mod K16");
                PhysAddresses.Add(0x1D, "[1D] Generator Cont/Pwr Inverter/Drive");
                PhysAddresses.Add(0x1E, "[1E] Pwr Inverter/Drive Motor Cont Mod");
                PhysAddresses.Add(0x1F, "[1F] Auxiliary Trans Fluid Pump");
                PhysAddresses.Add(0x2A, "[2A] Electronic Brake Cont Mod");
                PhysAddresses.Add(0x2B, "[2B] Parking Brake Cont Mod");
                PhysAddresses.Add(0x2C, "[2C] Multi -axis Acceleration Sen Mod");
                PhysAddresses.Add(0x2D, "[2D] Trailer Brake Cont Mod");
                PhysAddresses.Add(0x3A, "[3A] Vehicle Level Cont Mod");
                PhysAddresses.Add(0x3B, "[3B] Air Susp Cont Mod");
                PhysAddresses.Add(0x5A, "[5A] Rollover Sen Mod");
                PhysAddresses.Add(0x5B, "[5B] Side Object Sen Mod-Right");
                PhysAddresses.Add(0x5C, "[5C] Seat Belt Retractor Motor Mod -Left");
                PhysAddresses.Add(0x5D, "[5D] Pedestrian Impact Detect Mod");
                PhysAddresses.Add(0x5E, "[5E] Seat Belt Retractor Motor Mod-Right");
                PhysAddresses.Add(0x6D, "[6D] k157 Video Processing Cont Mod");
                PhysAddresses.Add(0x6F, "[6F] Chime Alarm Cont Mod");
                PhysAddresses.Add(0x8D, "[8D] Infotainment Cont");
                PhysAddresses.Add(0x8E, "[8E] Multimedia Player Interface Mod");
                PhysAddresses.Add(0x8F, "[8F] Human Machine Interface Mod");
                PhysAddresses.Add(0x9A, "[9A] Info Display Mod");
                PhysAddresses.Add(0x9B, "[9B] Auxiliary Heater Cont Mod");
                PhysAddresses.Add(0x9C, "[9C] Electric A/C Compressor Cont Mod");
                PhysAddresses.Add(0x9D, "[9D] Coolant Heater Cont Mod");
                PhysAddresses.Add(0x9F, "[9F] Parking Heater Cont Mod");
                PhysAddresses.Add(0xA0, "[A0] Driver Door Switch Panel Cont Mod");
                PhysAddresses.Add(0xA1, "[A1] Pass Door Switch Panel Cont Mod");
                PhysAddresses.Add(0xA2, "[A2] Door Cont Mod -Left Rear");
                PhysAddresses.Add(0xA3, "[A3] Door Cont Mod -Right Rear");
                PhysAddresses.Add(0xA4, "[A4] Liftgate Cont Mod");
                PhysAddresses.Add(0xA5, "[A5] Liftgate Cont Mod");
                PhysAddresses.Add(0xA6, "[A6] Seat Mem Cont Mod");
                PhysAddresses.Add(0xA7, "[A7] Rear Seat Cont Mod");
                PhysAddresses.Add(0xA8, "[A8] Seat Mem Cont Mod(Driver)");
                PhysAddresses.Add(0xA9, "[A9] Front Seat Heating Cont Mod");
                PhysAddresses.Add(0xAA, "[AA] Seat Mem Cont Mod(Pass/Rear)");
                PhysAddresses.Add(0xAB, "[AB] Pwr Sliding Door Cont Mod");
                PhysAddresses.Add(0xAC, "[AC] Pwr Sliding Door Cont Mod");
                PhysAddresses.Add(0xAD, "[AD] Folding Top Cont Mod");
                PhysAddresses.Add(0xAE, "[AE] Driver Seat Mem Cont Mod");
                PhysAddresses.Add(0xAF, "[AF] Keyless Entry Cont Mod");
                PhysAddresses.Add(0xB0, "[B0] Remote Cont Door Lock Receiver");
                PhysAddresses.Add(0xB1, "[B1] Folding Top Cont Mod");
                PhysAddresses.Add(0xB2, "[B2] Assist Step Contler");
                PhysAddresses.Add(0xB8, "[B8] Sunroof Cont Mod");
                PhysAddresses.Add(0xB9, "[B9] Left Side Object Detect Cont Mod");
                PhysAddresses.Add(0xBA, "[BA] Right Side Object Detect Cont Mod");
                PhysAddresses.Add(0xBB, "[BB] Parking Assist Cont Mod");
                PhysAddresses.Add(0xBC, "[BC] Frontview Camera Mod");
                PhysAddresses.Add(0xBD, "[BD] Inside Rearview Mirror");
                PhysAddresses.Add(0xC0, "[C0] Immo/Theft Deterrent Mod");
                PhysAddresses.Add(0xC1, "[C1] Keyless Entry Cont Mod");
                PhysAddresses.Add(0xC2, "[C2] Steering Column Lock Cont Mod");
                PhysAddresses.Add(0xC8, "[C8] DC Charging Cont Mod");
                PhysAddresses.Add(0xCA, "[CA] Batt Charger Cont Mod");
                PhysAddresses.Add(0xCB, "[CB] Hybrid Pwrtrain Cont Mod 2");
                PhysAddresses.Add(0xCC, "[CC] 14V Pwr Mod");
                PhysAddresses.Add(0xCD, "[CD] Batt Energy Cont Mod");
                PhysAddresses.Add(0xCE, "[CE] Hybrid/ EV Pwrtrain Cont Mod");
                PhysAddresses.Add(0xCF, "[CF] T6 Pwr Inverter Mod");
                PhysAddresses.Add(0xD1, "[D1] Frontview Camera Mod");
                PhysAddresses.Add(0xD2, "[D2] Long Range Radar Sen Mod");
                PhysAddresses.Add(0xD3, "[D3] Radar Sen Mod -Short Range");
                PhysAddresses.Add(0xD4, "[D4] Front Object Sen -Left Middle");
                PhysAddresses.Add(0xD5, "[D5] Front Object Sen -Right Middle");
                PhysAddresses.Add(0xD6, "[D6] Right Front Short Range Radar Sen Cont");
                PhysAddresses.Add(0xD7, "[D7] Rear Short Range Radar Sen Mod");
                PhysAddresses.Add(0xD8, "[D8] Radar Sen Mod -Short Range");
                PhysAddresses.Add(0xD9, "[D9] Rear Object Sen -Right Middle");
                PhysAddresses.Add(0xDA, "[DA] Right Rear Short Range Radar Sen Mod");
                PhysAddresses.Add(0x1C, "[1C] Trans Range Cont Mod");
                PhysAddresses.Add(0xF1, "[F1] Ext Tool");
            }
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
                Logger("Waiting data...");
                if (datalogger.LogRunning)
                {
                    datalogger.LogDevice.RemoveFilters(null);
                }
                else
                {
                    datalogger.LogDevice.SetTimeout(TimeoutScenario.Maximum);
                    datalogger.LogDevice.SetAnalyzerFilter();
                }
                datalogger.LogDevice.MsgReceived += LogDevice_MsgReceived;
                datalogger.LogDevice.MsgSent += LogDevice_MsgSent;
                
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

        private void LogDevice_MsgSent(object sender, MsgEventparameter e)
        {
            HandleMessage(e.Msg);
        }

        private void LogDevice_MsgReceived(object sender, MsgEventparameter e)
        {
            HandleMessage(e.Msg);
        }

        public void StopAnalyzer()
        {
            datalogger.LogDevice.MsgReceived -= LogDevice_MsgReceived;
            datalogger.LogDevice.MsgSent -= LogDevice_MsgSent;
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
                        //azd.TA = addr.ToString("X2") + " " + PhysAddresses[addr];
                        azd.TA = PhysAddresses[addr];
                    else
                        azd.TA = addr.ToString("X2");
                }
                addr = vRow.Message[2];
                if (PhysAddresses.ContainsKey(addr))
                    //azd.SA = addr.ToString("X2") + " " + PhysAddresses[addr];
                    azd.SA = PhysAddresses[addr];
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

        private void HandleMessage(OBDMessage rcv)
        {
/*            if (!datalogger.LogRunning && datalogger.LogDevice.LogDeviceType == LoggingDevType.Elm && rcv.ElmPrompt)
            {
                datalogger.LogDevice.SetAnalyzerFilter();
            }
*/
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
                    if (datalogger.LogDevice.SetVpwSpeed(VpwSpeed.FourX))
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

    }
}
