using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public class CANDevice
    {
        public int ResID { get; set; }
        public int RequestID { get; set; }
        public int DiagID { get; set; }
        public byte ModuleID { get; set; }
        public string ModuleName { get; set; }

        public override string ToString()
        {
            return "ResID:" + ResID.ToString("X4") + " RequestID: " + RequestID.ToString("X4") +
                " DiagID: " + DiagID.ToString("X4") + " ModuleID: " + ModuleID.ToString("X") + ": " + ModuleName;
        }
    }
    public static class CANQuery
    {
        public static void Query(Device device)
        {
            try
            {
                Logger("Sending CAN device query");
                byte[] queryMsg = { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x02, 0x1A, 0xB0, 0x00, 0x00, 0x00, 0x00 };
                bool m = device.SendMessage(new OBDMessage(queryMsg), 0);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, CANQuery line " + line + ": " + ex.Message);
            }
        }

        public static CANDevice DecodeMsg(byte[] rcv)
        {
            CANDevice retVal = new CANDevice();
            retVal.ResID = rcv[2] << 8 | rcv[3];
            if (rcv[2] >= 0x07)
            {
                retVal.RequestID = retVal.ResID - 8;
            }
            else
            {
                retVal.RequestID = retVal.ResID - 0x400;
            }
            retVal.DiagID = retVal.RequestID - 1;
            retVal.ModuleID = rcv[7];
            retVal.ModuleName = GetModuleName(retVal.ModuleID);

            return retVal;
        }

        private static string GetModuleName(byte did)
        {
            if (analyzer.PhysAddresses.ContainsKey(did))
            {
                return analyzer.PhysAddresses[did];
            }
            else
            {
                return "";
            }
        }

        private static string GetModuleName_Old(int did)
        {
            switch (did)
            {
                case 5: return "Info display Mod";
                case 0x10: return "PCM";
                case 0x11: return "ECM";
                case 0x13: return "FPCM/Chassis";
                case 0x14: return "Reductant Cont Mod";
                case 0x15: return "Fuel Pump Driver Mod";
                case 0x16: return "GPCM";
                case 0x17: return "Hybrid PIM";
                case 0x1A: return "TCCM";
                case 0x18: return "TCM";
                case 0x19: return "Pwr Take off";
                case 0x21: return "Adaptive Cruise";
                case 0x22: return "Chassis Cont Mod";
                case 0x24: return "VCIM";
                case 0x25: return "Aux Chassis";
                case 0x28: return "EBCM";
                case 0x29: return "Adaptive Cruise";
                case 0x30: return "PSCM";
                case 0x31: return "PSCM";
                case 0x32: return "Rear steering";
                case 0x34: return "Steering wheel angle";
                case 0x37: return "RDCM";
                case 0x38: return "Susp Cont Mod";
                case 0x39: return "Susp Cont";
                case 0x40: return "BCM";
                case 0x43: return "Aux BCM";
                case 0x44: return "BECM";
                case 0x45: return "SDGM";
                case 0x58: return "SDM";
                case 0x59: return "PPS";
                case 0x60: return "IPC";
                case 0x61: return "Info Display Mod";
                case 0x62: return "Heads up Display";
                case 0x64: return "Rearview Camera";
                case 0x65: return "Night Vision";
                case 0x66: return "Radio/Hvac Conts";
                case 0x68: return "HVAC Conts";
                case 0x69: return "Data recorder";
                case 0x72: return "Headlamp Cont Mod";
                case 0x73: return "Headlamp H Beam Cont Mod";
                case 0x74: return "Headlamp Cont Mod";
                case 0x75: return "Trailer Interface Cont Mod";
                case 0x80: return "Radio";
                case 0x81: return "Amplifier";
                case 0x85: return "Rear Audio Cont Mod";
                case 0x89: return "XM Mod";
                case 0x95: return "memory seat interface Mod";
                case 0x97: return "VCIM"; // 99 HVAC Cont Mod
                case 0x99: return "HVAC Cont Mod";
                case 0x0C: return "B195A Nitrogen Oxides Sen 1";
                case 0x0D: return "B195B Nitrogen Oxides Sen 2";
                case 0x0E: return "B136 Exh Particulate Matter Sen";
                case 0x0F: return "M103 Turbo Vane Pos Act";
                case 0x1B: return "Rear Diff Clutch Cont Mod K16";
                case 0x1D: return "Generator Cont/Pwr Inverter/Drive";
                case 0x1E: return "Pwr Inverter/Drive Motor Cont Mod";
                case 0x1F: return "Auxiliary Trans Fluid Pump";
                case 0x2A: return "Electronic Brake Cont Mod";
                case 0x2B: return "Parking Brake Cont Mod";
                case 0x2C: return "Multi -axis Acceleration Sen Mod";
                case 0x2D: return "Trailer Brake Cont Mod";
                case 0x3A: return "Vehicle Level Cont Mod";
                case 0x3B: return "Air Susp Cont Mod";
                case 0x5A: return "Rollover Sen Mod";
                case 0x5B: return "Side Object Sen Mod-Right";
                case 0x5C: return "Seat Belt Retractor Motor Mod -Left";
                case 0x5D: return "Pedestrian Impact Detect Mod";
                case 0x5E: return "Seat Belt Retractor Motor Mod-Right";
                case 0x6D: return "k157 Video Processing Cont Mod";
                case 0x6F: return "Chime Alarm Cont Mod";
                case 0x8D: return "Infotainment Cont";
                case 0x8E: return "Multimedia Player Interface Mod";
                case 0x8F: return "Human Machine Interface Mod";
                case 0x9A: return "Info Display Mod";
                case 0x9B: return "Auxiliary Heater Cont Mod";
                case 0x9C: return "Electric A/C Compressor Cont Mod";
                case 0x9D: return "Coolant Heater Cont Mod";
                case 0x9F: return "Parking Heater Cont Mod";
                case 0xA0: return "Driver Door Switch Panel Cont Mod";
                case 0xA1: return "Pass Door Switch Panel Cont Mod";
                case 0xA2: return "Door Cont Mod -Left Rear";
                case 0xA3: return "Door Cont Mod -Right Rear";
                case 0xA4: return "Liftgate Cont Mod";
                case 0xA5: return "Liftgate Cont Mod";
                case 0xA6: return "Seat Mem Cont Mod";
                case 0xA7: return "Rear Seat Cont Mod";
                case 0xA8: return "Seat Mem Cont Mod(Driver)";
                case 0xA9: return "Front Seat Heating Cont Mod";
                case 0xAA: return "Seat Mem Cont Mod(Pass/Rear)";
                case 0xAB: return "Pwr Sliding Door Cont Mod";
                case 0xAC: return "Pwr Sliding Door Cont Mod";
                case 0xAD: return "Folding Top Cont Mod";
                case 0xAE: return "Driver Seat Mem Cont Mod";
                case 0xAF: return "Keyless Entry Cont Mod";
                case 0xB0: return "Remote Cont Door Lock Receiver";
                case 0xB1: return "Folding Top Cont Mod";
                case 0xB2: return "Assist Step Contler";
                case 0xB8: return "Sunroof Cont Mod";
                case 0xB9: return "Left Side Object Detect Cont Mod";
                case 0xBA: return "Right Side Object Detect Cont Mod";
                case 0xBB: return "Parking Assist Cont Mod";
                case 0xBC: return "Frontview Camera Mod";
                case 0xBD: return "Inside Rearview Mirror";
                case 0xC0: return "Immo/Theft Deterrent Mod";
                case 0xC1: return "Keyless Entry Cont Mod";
                case 0xC2: return "Steering Column Lock Cont Mod";
                case 0xC8: return "DC Charging Cont Mod";
                case 0xCA: return "Batt Charger Cont Mod";
                case 0xCB: return "Hybrid Pwrtrain Cont Mod 2";
                case 0xCC: return "14V Pwr Mod";
                case 0xCD: return "Batt Energy Cont Mod";
                case 0xCE: return "Hybrid/ EV Pwrtrain Cont Mod";
                case 0xCF: return "T6 Pwr Inverter Mod";
                case 0xD1: return "Frontview Camera Mod";
                case 0xD2: return "Long Range Radar Sen Mod";
                case 0xD3: return "Radar Sen Mod -Short Range";
                case 0xD4: return "Front Object Sen -Left Middle";
                case 0xD5: return "Front Object Sen -Right Middle";
                case 0xD6: return "Right Front Short Range Radar Sen Cont";
                case 0xD7: return "Rear Short Range Radar Sen Mod";
                case 0xD8: return "Radar Sen Mod -Short Range";
                case 0xD9: return "Rear Object Sen -Right Middle";
                case 0xDA: return "Right Rear Short Range Radar Sen Mod";
                case 0x1C: return "Trans Range Cont Mod";
                default: return "";
            }
        }
    }
}
