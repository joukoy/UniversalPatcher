using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static void Query(Device device, MessageReceiver receiver)
        {
            byte[] queryMsg = { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x02, 0x1A, 0xB0, 0x00, 0x00, 0x00, 0x00 };
            bool m = device.SendMessage(new OBDMessage(queryMsg), 0);
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
            retVal.ModuleName = GetModuleName(retVal.ModuleID.ToString("X"));

            return retVal;
        }

        private static string GetModuleName(string did)
        {
            switch (did)
            {
                case "5": return "Info display Mod";
                case "10": return "PCM";
                case "11": return "ECM";
                case "13": return "FPCM/Chassis";
                case "14": return "Reductant Cont Mod";
                case "15": return "Fuel Pump Driver Mod";
                case "16": return "GPCM";
                case "17": return "Hybrid PIM";
                case "1A": return "TCCM";
                case "18": return "TCM";
                case "19": return "Pwr Take off";
                case "21": return "Adaptive Cruise";
                case "22": return "Chassis Cont Mod";
                case "24": return "VCIM";
                case "25": return "Aux Chassis";
                case "28": return "EBCM";
                case "29": return "Adaptive Cruise";
                case "30": return "PSCM";
                case "31": return "PSCM";
                case "32": return "Rear steering";
                case "34": return "Steering wheel angle";
                case "37": return "RDCM";
                case "38": return "Susp Cont Mod";
                case "39": return "Susp Cont";
                case "40": return "BCM";
                case "43": return "Aux BCM";
                case "44": return "BECM";
                case "45": return "SDGM";
                case "58": return "SDM";
                case "59": return "PPS";
                case "60": return "IPC";
                case "61": return "Info Display Mod";
                case "62": return "Heads up Display";
                case "64": return "Rearview Camera";
                case "65": return "Night Vision";
                case "66": return "Radio/Hvac Conts";
                case "68": return "HVAC Conts";
                case "69": return "Data recorder";
                case "72": return "Headlamp Cont Mod";
                case "73": return "Headlamp H Beam Cont Mod";
                case "74": return "Headlamp Cont Mod";
                case "75": return "Trailer Interface Cont Mod";
                case "80": return "Radio";
                case "81": return "Amplifier";
                case "85": return "Rear Audio Cont Mod";
                case "89": return "XM Mod";
                case "95": return "memory seat interface Mod";
                case "97": return "VCIM"; // 99 HVAC Cont Mod
                case "99": return "HVAC Cont Mod";
                case "0C": return "B195A Nitrogen Oxides Sen 1";
                case "0D": return "B195B Nitrogen Oxides Sen 2";
                case "0E": return "B136 Exh Particulate Matter Sen";
                case "0F": return "M103 Turbo Vane Pos Act";
                case "1B": return "Rear Diff Clutch Cont Mod K16";
                case "1D": return "Generator Cont/Pwr Inverter/Drive";
                case "1E": return "Pwr Inverter/Drive Motor Cont Mod";
                case "1F": return "Auxiliary Trans Fluid Pump";
                case "2A": return "Electronic Brake Cont Mod";
                case "2B": return "Parking Brake Cont Mod";
                case "2C": return "Multi -axis Acceleration Sen Mod";
                case "2D": return "Trailer Brake Cont Mod";
                case "3A": return "Vehicle Level Cont Mod";
                case "3B": return "Air Susp Cont Mod";
                case "5A": return "Rollover Sen Mod";
                case "5B": return "Side Object Sen Mod-Right";
                case "5C": return "Seat Belt Retractor Motor Mod -Left";
                case "5D": return "Pedestrian Impact Detect Mod";
                case "5E": return "Seat Belt Retractor Motor Mod-Right";
                case "6D": return "k157 Video Processing Cont Mod";
                case "6F": return "Chime Alarm Cont Mod";
                case "8D": return "Infotainment Cont";
                case "8E": return "Multimedia Player Interface Mod";
                case "8F": return "Human Machine Interface Mod";
                case "9A": return "Info Display Mod";
                case "9B": return "Auxiliary Heater Cont Mod";
                case "9C": return "Electric A/C Compressor Cont Mod";
                case "9D": return "Coolant Heater Cont Mod";
                case "9F": return "Parking Heater Cont Mod";
                case "A0": return "Driver Door Switch Panel Cont Mod";
                case "A1": return "Pass Door Switch Panel Cont Mod";
                case "A2": return "Door Cont Mod -Left Rear";
                case "A3": return "Door Cont Mod -Right Rear";
                case "A4": return "Liftgate Cont Mod";
                case "A5": return "Liftgate Cont Mod";
                case "A6": return "Seat Mem Cont Mod";
                case "A7": return "Rear Seat Cont Mod";
                case "A8": return "Seat Mem Cont Mod(Driver)";
                case "A9": return "Front Seat Heating Cont Mod";
                case "AA": return "Seat Mem Cont Mod(Pass/Rear)";
                case "AB": return "Pwr Sliding Door Cont Mod";
                case "AC": return "Pwr Sliding Door Cont Mod";
                case "AD": return "Folding Top Cont Mod";
                case "AE": return "Driver Seat Mem Cont Mod";
                case "AF": return "Keyless Entry Cont Mod";
                case "B0": return "Remote Cont Door Lock Receiver";
                case "B1": return "Folding Top Cont Mod";
                case "B2": return "Assist Step Contler";
                case "B8": return "Sunroof Cont Mod";
                case "B9": return "Left Side Object Detect Cont Mod";
                case "BA": return "Right Side Object Detect Cont Mod";
                case "BB": return "Parking Assist Cont Mod";
                case "BC": return "Frontview Camera Mod";
                case "BD": return "Inside Rearview Mirror";
                case "C0": return "Immo/Theft Deterrent Mod";
                case "C1": return "Keyless Entry Cont Mod";
                case "C2": return "Steering Column Lock Cont Mod";
                case "C8": return "DC Charging Cont Mod";
                case "CA": return "Batt Charger Cont Mod";
                case "CB": return "Hybrid Pwrtrain Cont Mod 2";
                case "CC": return "14V Pwr Mod";
                case "CD": return "Batt Energy Cont Mod";
                case "CE": return "Hybrid/ EV Pwrtrain Cont Mod";
                case "CF": return "T6 Pwr Inverter Mod";
                case "D1": return "Frontview Camera Mod";
                case "D2": return "Long Range Radar Sen Mod";
                case "D3": return "Radar Sen Mod -Short Range";
                case "D4": return "Front Object Sen -Left Middle";
                case "D5": return "Front Object Sen -Right Middle";
                case "D6": return "Right Front Short Range Radar Sen Cont";
                case "D7": return "Rear Short Range Radar Sen Mod";
                case "D8": return "Radar Sen Mod -Short Range";
                case "D9": return "Rear Object Sen -Right Middle";
                case "DA": return "Right Rear Short Range Radar Sen Mod";
                case "1C": return "Trans Range Cont Mod";
                default: return "";
            }
        }
    }
}
