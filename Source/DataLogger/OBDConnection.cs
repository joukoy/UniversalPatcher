using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using System.IO;
using System.Xml.Linq;
using static UniversalPatcher.DataLogger;
using System.Management;
using J2534DotNet;

namespace UniversalPatcher
{
    public class OBDConnection
    {
        public OBDConnection()
        {
            Protocols = new List<J2534InitParameters>();
            this.WakeUp = false;
            this.StartPeriodic = false;
        }
        public OBDConnection(Device device)
        {
            this.ObdDevice = device;
            Protocols = new List<J2534InitParameters>();
            this.WakeUp = false;
            this.StartPeriodic = false;
        }
        public Device ObdDevice { get; set; }
        public bool Connected { get; set; }
        public LoggingProtocol LoggingProto {get;set;}
        public iPortType PortType { get; set; }
        public IPort port { get; set; }
        public int BaudRate { get; set; }
        public string PortName { get; set; }
        public string SerialportDeviceType { get; set; }
        public J2534DotNet.J2534Device J2534Dev { get; set; }
        public int J2534DevIndex { get; set; }
        public bool Enable4xReadWrite { get; set; }
        public int ReadTimeout { get;set; }
        public int WriteTimeout { get; set; }
        public bool WakeUp { get; set; }
        public bool StartPeriodic { get; set; }
        public List<J2534InitParameters> Protocols { get; set; }
    }
}
