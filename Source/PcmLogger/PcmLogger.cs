using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

namespace UniversalPatcher
{
    public static class PcmLogger
    {
        public static List<PidConfig> PidProfile { get; set; }
        //private static SerialPort Port;
        public static OBDXProDevice obdx;
        public static AvtDevice avt;
        private static List<Slot> PidSlots;
        private static StreamWriter logwriter;
        private static string logseparator = ";";
        public static DeviceType Devicetype { get; set; }
        private static Device PcmDevice;
        public static int ReceivedRecords = 0;
        private static List<byte[]> pidRequestMsgs;

        public static Dictionary<int, int> PidNrPidIndex = new Dictionary<int, int>();
        public static double[] LastReadValues;

        public static bool stoploop;
        public static bool passive;
        public static bool writelog;

        public class ReadValue
        {
            public string PidName { get; set; }
            public double PidValue { get; set; }
            public int index { get; set; }
        }

        public class PidConfig
        {
            public PidConfig()
            {
                DataType = PidDataType.uint8;
                Math = "X";
            }
            public string PidNumber { get { return pidnr.ToString("X4"); } set { HexToUshort(value, out pidnr); } }
            public string PidName { get; set; }
            public PidDataType DataType { get; set; }
            public string Math { get; set; }
            public string Units { get; set; }
            public ushort pidnr;
            public byte GetElmentSize()
            {
                if (DataType == PidDataType.uint16 || DataType == PidDataType.int16)
                    return 2;
                else
                    return 1;
            }
            public bool Signed()
            {
                if (DataType == PidDataType.uint8 || DataType == PidDataType.uint16)
                    return false;
                else
                    return true;
            }
        }

        public class Conversion
        {
            public string Units { get; private set; }

            public string Expression { get; private set; }

            public string Format { get; private set; }

            public bool IsBitMapped { get; private set; }

            public int BitIndex { get; private set; }

            public string TrueValue { get; private set; }

            public string FalseValue { get; private set; }

            public Conversion(string units, string expression, string format)
            {
                this.Units = units;
                this.Expression = Sanitize(expression);
                this.Format = format;
                this.IsBitMapped = false;
                this.BitIndex = -1;
                this.TrueValue = null;
                this.FalseValue = null;
            }

            public Conversion(string units, int bitIndex, string trueValue, string falseValue)
            {
                this.Units = units;
                this.Expression = "x";
                this.Format = "";
                this.IsBitMapped = true;
                this.BitIndex = bitIndex;
                this.TrueValue = trueValue;
                this.FalseValue = falseValue;
            }

            public override string ToString()
            {
                return this.Units;
            }

            /// <summary>
            /// The expression parser doesn't support bit-shift operators.
            /// So we hack them into division operators here.
            /// It's not pretty, but it's less ugly than changing the
            /// expressions in the XML file.
            /// </summary>
            private string Sanitize(string input)
            {
                int startIndex = input.IndexOf(">>");
                if (startIndex == -1)
                {
                    return input;
                }

                int endIndex = startIndex;
                char shiftChar = ' ';
                for (int index = startIndex + 2; index < input.Length; index++)
                {
                    endIndex = index;
                    shiftChar = input[index];
                    if (shiftChar == ' ')
                    {
                        continue;
                    }
                    else
                    {
                        endIndex++;
                        break;
                    }
                }

                int shiftCount = shiftChar - '0';
                if (shiftCount < 0 || shiftCount > 15)
                {
                    throw new InvalidOperationException(
                        string.Format("Unable to parse >> operator in \"{0}\"", input));
                }

                string oldText = input.Substring(startIndex, endIndex - startIndex);
                string newText = string.Format("/{0}", Math.Pow(2, shiftCount));
                return input.Replace(oldText, newText);
            }
        }

        public class Parameter
        {
            public Parameter()
            {
                Conversions = new List<Conversion>();
            }
            public string Id { get;  set; }
            public string Name { get;  set; }
            public List<Conversion> Conversions { get; set; }
            public string Description { get;  set; }
            public string DataType { get; set; }
        }

        public enum PidDataType
        {
            uint8 = 1,
            uint16 = 2,
            int8,
            int16
        }

        private class Slot
        {
            public Slot(byte Id)
            {
                this.Id = Id;
                PidIndex = new List<int>();
            }
            public byte Id { get; set; }
            public List<int> PidIndex { get; set; }
            public byte Bytes
            {
                get
                {
                    byte bCount = 0;
                    for (int i = 0; i < PidIndex.Count; i++)
                        bCount += PidProfile[i].GetElmentSize();
                    return bCount;
                }
            }
        }

        public enum DeviceType
        {
            Obdx = 1,
            Avt = 2
        }

        public static Device CreateSerialDevice(string serialPortName, string serialPortDeviceType)
        {
            try
            {
                IPort port;
                port = new Rs232Port(serialPortName);

                Device device;
                switch (serialPortDeviceType)
                {
                    case OBDXProDevice.DeviceType:
                        device = new OBDXProDevice(port);
                        break;

                    case AvtDevice.DeviceType:
                        device = new AvtDevice(port);
                        break;

                    case ElmDevice.DeviceType:
                        device = new ElmDevice(port);
                        break;

                    default:
                        device = null;
                        break;
                }

                if (device == null)
                {
                    return null;
                }

                return device;
            }
            catch (Exception exception)
            {
                LoggerBold($"Unable to create {serialPortDeviceType} on {serialPortName}.");
                Debug.WriteLine(exception.ToString());
                return null;
            }
        }
        public static bool CreateLog(string path)
        {
            try
            {
                Logger("Writing to logfile: " + path);
                logseparator = Properties.Settings.Default.LoggerLogSeparator;
                if (File.Exists(path))
                {
                    logwriter = new StreamWriter(path, true);
                }
                else
                {
                    StringBuilder sb = new StringBuilder("Time" + logseparator);
                    for (int c = 0; c < PidProfile.Count; c++)
                    {
                        sb.Append(PidProfile[c].PidName + logseparator);
                    }
                    string header = sb.ToString().Trim(logseparator[0]);
                    logwriter = new StreamWriter(path);
                    logwriter.WriteLine(header);
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return false;
        }

        public static void WriteLog(List<ReadValue> Rvalues)
        {
            try
            {
                StringBuilder sb = new StringBuilder(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:FF") + logseparator);
                for (int c = 0; c < PidProfile.Count; c++)
                {
                    ReadValue val = Rvalues.Where(x => x.PidName == PidProfile[c].PidName).FirstOrDefault();
                    if (val == null)
                        sb.Append(logseparator);
                    else
                        sb.Append(val.PidValue.ToString().Replace(",", ".") + logseparator);
                }
                logwriter.WriteLine(sb.ToString().Trim(logseparator[0]));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Logger line " + line + ": " + ex.Message);
            }
        }

        public static void LoadProfile(string FileName)
        {            
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                PidProfile = (List<PidConfig>)reader.Deserialize(file);
                file.Close();                 
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Logger line " + line + ": " + ex.Message);
            }            
        }

        public static void SaveProfile(string FileName)
        {
            try
            {
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<PidConfig>));
                    writer.Serialize(stream, PidProfile);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Logger line " + line + ": " + ex.Message);
            }
        }

        private static byte CreateConfigByte(byte position, byte bytes)
        {
            byte pidbyte = 0b01000000; // 01xxxxxx = Pid
            byte pos = (byte)(0xFF & (position << 3)); // xxXXXxxx Position of pid in response
            return (byte)(pidbyte | pos | bytes);      //last 3 bits: data size (bytes)
        }

        public static List<byte[]> CreatePidSetupMessages()
        {
            List<byte[]> retVal = new List<byte[]>();
            int pidIndex = 0;
            byte slotNr = 0xFE;
            PidSlots = new List<Slot>();
            byte position = 1;
            PidNrPidIndex = new Dictionary<int, int>();
            while (pidIndex < PidProfile.Count)
            {
                Slot slot = new Slot(slotNr);
                while (pidIndex < PidProfile.Count && (slot.Bytes + PidProfile[pidIndex].GetElmentSize()) <= 6)
                {
                    slot.PidIndex.Add(pidIndex);
                    PidNrPidIndex.Add(PidProfile[pidIndex].pidnr, pidIndex);
                    pidIndex++;
                }
                PidSlots.Add(slot);
                slotNr--;
            }
            for (int s=0; s < PidSlots.Count;s++)
            {
                pidIndex = 0;
                while (pidIndex < PidSlots[s].PidIndex.Count)
                {
                    slotNr = PidSlots[s].Id;
                    byte[] msg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x2C, slotNr, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                    msg[6] = (byte)(PidProfile[pidIndex].pidnr >> 8);
                    msg[7] = (byte)(PidProfile[pidIndex].pidnr);
                    byte bytes = PidProfile[pidIndex].GetElmentSize();
                    pidIndex++;
                    if (pidIndex< PidSlots[s].PidIndex.Count)
                    {
                        msg[8] = (byte)(PidProfile[pidIndex].pidnr >> 8);
                        msg[9] = (byte)(PidProfile[pidIndex].pidnr);
                        bytes += PidProfile[pidIndex].GetElmentSize();
                        pidIndex++;
                    }
                    byte cfgByte = CreateConfigByte(position, bytes);
                    msg[5] = cfgByte;
                    Debug.WriteLine("ConfigByte: " + cfgByte.ToString("X"));

                    retVal.Add(msg);
                    Debug.WriteLine("Pid setup msg:" + BitConverter.ToString(msg));
                }
                position++;
            }
            return retVal;
        }

        public static bool SetBusQuiet()
        {
            try
            {
                byte[] quietMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.SilenceBus };
                bool m = PcmDevice.SendMessage(new OBDMessage(quietMsg));
                if (m)
                {
                    Debug.WriteLine("Bus set to quiet");
                }
                else
                {
                    Logger("Unable to set bus quiet");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(quietMsg, b => b.ToString("X2"))));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                return false;
            }
        }

        public static bool SetMode1()
        {
            try
            {
                byte[] Msg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x01, 0x01 };
                bool m = PcmDevice.SendMessage(new OBDMessage(Msg));
                if (m)
                {
                    Debug.WriteLine("Mode set to 1");
                }
                else
                {
                    Logger("Unable to set mode 1");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(Msg, b => b.ToString("X2"))));
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                return false;
            }
        }

        private static List<byte[]> CreatePidRequestMessages(byte RespType)
        {
            List<byte[]> retVal = new List<byte[]>();
            byte slot = PidSlots[0].Id;
            byte LastSlot = PidSlots.Last().Id;
            while(slot >= LastSlot)
            {
                /*
                byte RespType = ResponseType.SendOnce;
                if (passive)
                {
                    RespType = ResponseType.SendFast;
                } */
                byte[] msg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.SendDynamicData, RespType, 0xFF, 0xFF, 0xFF, 0xFF };
                for (int i=0; i<4 && slot >= LastSlot;i++)
                {
                    msg[i+5]= slot;
                    slot--;
                }
                retVal.Add(msg);
                Debug.WriteLine("Pid Start msg:" + BitConverter.ToString(msg));
            }
            return retVal;
        }

        private static bool SetHighSpeedMode()
        {
            byte[] msg = new byte[] { Priority.Physical0, DeviceId.Broadcast, DeviceId.Tool, Mode.HighSpeedPrepare };
            bool resp = PcmDevice.SendMessage(new OBDMessage(msg));
            if (!resp)
            {
                return false;
            }
            msg = new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.HighSpeed };
            resp = PcmDevice.SendMessage(new OBDMessage(msg));
            if (!resp)
            {
                return false;
            }
            Logger("High speed mode on");
            return true;
        }

        public static bool InitalizeDevice(string comport, string devtype)
        {
            try
            {                
                PcmDevice = CreateSerialDevice(comport, devtype);
                if (PcmDevice.Initialize())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            if (PcmDevice != null)
            {
                PcmDevice.Dispose();
            }
            return false;
        }

        public static bool InitalizeJDevice(string devtype)
        {
            try
            {
                J2534DotNet.J2534Device dev = J2534DeviceFinder.FindInstalledJ2534DLLs().Where(x => x.Name == devtype).FirstOrDefault();
                PcmDevice = new J2534Device(dev);
                if (PcmDevice.Initialize())
                    return true;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return false;
        }

        public static bool StartLogging(byte resptype, bool writelogOn)
        {
            try
            {
                if (resptype != ResponseType.SendOnce)
                    passive = true;
                else
                    passive = false;
                writelog = writelogOn;

                if (!SetBusQuiet())
                    return false;
                if (!SetMode1())
                    return false;
                //SetHighSpeedMode();
                List<byte[]> commands = CreatePidSetupMessages();
                for (int c = 0; c < commands.Count; c++)
                {
                    Debug.WriteLine("Setup pids:");
                    bool resp = PcmDevice.SendMessage(new OBDMessage(commands[c]));
                    if (!resp)
                    {
                        LoggerBold("Error, Pid setup failed");
                        return false;
                    }
                }
                pidRequestMsgs = CreatePidRequestMessages(resptype);

/*
                for (int c = 0; c < pidRequestMsgs.Count; c++)
                {
                    Debug.WriteLine("Pid request message: " + BitConverter.ToString(pidRequestMsgs[c]));
                    bool resp = PcmDevice.SendMessage(new OBDMessage(pidRequestMsgs[c]));
                    if (!resp)
                    {
                        LoggerBold("Error, Pid request messages failed");
                        return false;
                    }
                }
*/
                stoploop = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback(LogLoop), null);
                return true;
            }
            catch (Exception ex)
            {
                logwriter = null;
                PcmDevice.Dispose();
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Logger line " + line + ": " + ex.Message);
                return false;
            }
        }

        public static void RequestNextPids()
        {
            for (int c = 0; c < pidRequestMsgs.Count; c++)
            {
                OBDMessage msg = new OBDMessage(pidRequestMsgs[c]);
                PcmDevice.SendMessage(msg);
            }
            //Debug.WriteLine("Requesting pids...");
        }

        public static void StopLogging()
        {
            stoploop = true;
            Thread.Sleep(100);
            for (int c = 0; c < pidRequestMsgs.Count; c++)
            {
                byte[] data = pidRequestMsgs[c];
                data[4] = 0; //Stop sending
                OBDMessage msg = new OBDMessage(data);
                PcmDevice.SendMessage(msg);
            }
            PcmDevice.ClearMessageBuffer();
            PcmDevice.ClearMessageQueue();
            PcmDevice.Dispose();
            if (logwriter != null)
            {
                logwriter.Close();
                logwriter = null;
            }
        }

        public static byte[] ReceiveMessage()
        {
            return PcmDevice.ReceiveMessage().GetBytes();
        }

        public static void Receive()
        {
            PcmDevice.Receive();
        }

        public static bool SendTesterPresent()
        {
            //Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x2c, slot, 0x4A 
            byte[] presentMsg = { Priority.Physical0High, DeviceId.Broadcast, DeviceId.Tool, 0x3F };
            //presentMsg[presentMsg.Length - 1] = obdx.CalcChecksum(presentMsg);
            //obdx.SendData(presentMsg);
            Debug.WriteLine("Tester present");
            PcmDevice.SendMessage(new OBDMessage(presentMsg));
            return true;
        }


        public static List<ReadValue> ParseMessage(byte[] msg)
        {
            List<ReadValue> retVal = new List<ReadValue>();
            try
            {
                if (msg.Length > 6)
                {
                    Debug.WriteLine("Parsing message: " + BitConverter.ToString(msg));
                    byte slotNr = msg[4];
                    int pos = 5;
                    Slot slot = PidSlots.Where(x => x.Id == slotNr).FirstOrDefault();
                    if (slot == null)
                    {
                        return retVal;
                    }
                    for (int p = 0; p < slot.PidIndex.Count; p++)
                    {
                        int pid = slot.PidIndex[p];
                        PidConfig pc = PidProfile[pid];
                        byte elementSize = pc.GetElmentSize();
                        if (msg.Length >= (pos + elementSize))
                        {
                            //Debug.WriteLine("Received pid (index): " + pid);
                            double val = 0;
                            switch (pc.DataType)
                            {
                                case PidDataType.uint8:
                                    val = (byte)msg[pos];
                                    pos++;
                                    break;
                                case PidDataType.int8:
                                    val = (sbyte)msg[pos];
                                    pos++;
                                    break;
                                case PidDataType.uint16:
                                    val = (ushort)(msg[pos] << 8 & msg[pos + 1]);
                                    pos += 2;
                                    break;
                                case PidDataType.int16:
                                    val = (Int16)(msg[pos] << 8 & msg[pos + 1]);
                                    pos += 2;
                                    break;
                            }
                            ReadValue rv = new ReadValue();
                            string mathStr = pc.Math.ToLower().Replace("x", val.ToString());
                            rv.PidValue = parser.Parse(mathStr);
                            rv.PidName = pc.PidName;
                            rv.index = slot.PidIndex[p];
                            retVal.Add(rv);
                        }
                        else
                        {
                            break;
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
                LoggerBold("Error, Logger line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        public static void LogLoop(object threadContext)
        {
            List<ReadValue> rValues = new List<ReadValue>();
            List<int> receivedPids = new List<int>();
            LastReadValues = new double[PidProfile.Count];
            DateTime lastPresent = DateTime.Now;
            if (passive)
            {
                RequestNextPids();
            }
            while (!stoploop)
            {
                try
                {
                    if (passive)
                    {
                        if (DateTime.Now.Subtract(lastPresent) > TimeSpan.FromSeconds(4))
                        {
                            SendTesterPresent();
                            lastPresent = DateTime.Now;
                        }
                    }
                    else
                    {
                        RequestNextPids();
                    }
                    byte[] data = PcmDevice.ReceiveMessage().GetBytes();
                    List<ReadValue> newValues = ParseMessage(data);
                    for (int r = 0; r < newValues.Count; r++)
                    {
                        ReadValue rv = newValues[r];
                        LastReadValues[rv.index] = rv.PidValue;
                        if (!receivedPids.Contains(rv.index))
                        {
                            receivedPids.Add(rv.index);
                            rValues.Add(rv);
                        }
                    }
                    //Debug.WriteLine("Received: " + receivedPids.Count + ", Total: " + PidProfile.Count);
                    if (receivedPids.Count >= PidProfile.Count)
                    {
                        //All pids received
                        receivedPids = new List<int>();
                        if (writelog)
                        {
                            WriteLog(rValues);
                        }
                        ReceivedRecords++;
                    }

                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    Debug.WriteLine("Error, logLoop line " + line + ": " + ex.Message);
                }
            }
        }
    }
}
