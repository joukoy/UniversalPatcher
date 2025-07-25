using J2534DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    [Serializable]
    public class J2534InitParameters
    {
        public J2534InitParameters()
        {
            this.VPWLogger = false;
            Protocol = ProtocolID.J1850VPW;
            Baudrate = "";
            Sconfigs = "";
            Kinit = KInit.None;
            InitBytes = "";
            Connectflag = ConnectFlag.NONE;
            PerodicMsg = "";
            PassFilters = "";
            Secondary = false;
            UsePrimaryChannel = false;
            SeparateProtoByChannel = false;
        }
        public J2534InitParameters(bool VpwLogger)
        {
            this.VPWLogger = VpwLogger;
            Protocol = ProtocolID.J1850VPW; 
            Baudrate = "";
            Sconfigs = "";
            Kinit = KInit.None;
            InitBytes = "";
            Connectflag = ConnectFlag.NONE;
            PerodicMsg = "";
            PassFilters = "";
            Secondary = false;
            UsePrimaryChannel = false;
            SeparateProtoByChannel = false;
        }
        public bool VPWLogger { get; set; }
        public bool Secondary { get; set; }
        public ProtocolID Protocol { get; set; }
        public string Baudrate { get; set; }
        public string Sconfigs { get; set; }
        public KInit Kinit { get; set; }
        public string InitBytes { get; set; }
        public ConnectFlag Connectflag { get; set; }
        public string PerodicMsg { get; set; }
        public int PeriodicInterval { get; set; }
        public string PassFilters { get; set; }
        public bool UsePrimaryChannel { get; set; }
        public bool SeparateProtoByChannel { get; set; }
        [NonSerialized]
        public CANDevice CanPCM; //For ELM327
    }
}
