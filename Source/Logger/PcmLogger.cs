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

namespace UniversalPatcher
{
    public static class PcmLogger
    {
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
        }

        public static List<PidConfig> PidProfile { get; set; }
        public static OBDXProDevice obdx;
        private static bool stop = true;
        private static List<Slot> slotpids;
        private static StreamWriter logwriter;
        private static string logseparator = ";";

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
            StringBuilder sb = new StringBuilder(DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:FF") + logseparator);
            for (int c=0; c < PidProfile.Count; c++)
            {
                ReadValue val = Rvalues.Where(x => x.PidName == PidProfile[c].PidName).FirstOrDefault();
                if (val == null)
                    sb.Append(logseparator);
                else
                    sb.Append(val.PidValue.ToString() + logseparator);
            }            
            logwriter.WriteLine(sb.ToString().Trim(logseparator[0]));
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
            int pid = 0;
            byte slotNr = 0xFE;
            int rqbytes = 0;
            slotpids = new List<Slot>();
            Slot slot = new Slot(slotNr);
            slotpids.Add(slot);
            byte position = 1;
            while (pid < PidProfile.Count)
            {
                if ((rqbytes + (byte)PidProfile[pid].DataType) > 6)
                {
                    slotNr--;
                    position = 1;
                    slot = new Slot(slotNr);
                    slotpids.Add(slot);
                }
                byte cfgByte = CreateConfigByte(position, (byte)PidProfile[pid].GetElmentSize());
                Debug.WriteLine("ConfigByte: " + cfgByte.ToString("X"));
                byte[] msg= { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x2C, slotNr, cfgByte, 0xFF, 0xFF, 0xFF, 0xFF };
                msg[6] = (byte)(PidProfile[pid].pidnr >> 8);
                msg[7] = (byte)(PidProfile[pid].pidnr);
                rqbytes += PidProfile[pid].GetElmentSize();
                slot.PidIndex.Add(pid);
                pid++;
                position++;
                if (pid < PidProfile.Count && (rqbytes + (byte)PidProfile[pid].DataType) <= 6)
                {
                    msg[8] = (byte)(PidProfile[pid].pidnr >> 8);
                    msg[9] = (byte)(PidProfile[pid].pidnr);
                    rqbytes += (byte)PidProfile[pid].DataType;
                    slot.PidIndex.Add(pid);
                    pid++;
                    position++;
                }
                retVal.Add(msg);
            }
            return retVal;
        }

        public static bool SetBusQuiet()
        {
            try
            {
                byte[] quietMsg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.SilenceBus };
                Response<byte[]> m = obdx.SendDVIPacket(quietMsg);
                if (m.Status == ResponseStatus.Success)
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
                Response<byte[]> m = obdx.SendDVIPacket(Msg);
                if (m.Status == ResponseStatus.Success)
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

        private static List<byte[]> CreateStartMessages()
        {
            List<byte[]> retVal = new List<byte[]>();
            byte slot = slotpids[0].Id;
            byte LastSlot = slotpids[slotpids.Count - 1].Id;
            while(slot >= LastSlot)
            {
                byte[] msg = { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.SendDynamicData, ResponseType.SendFast, 0xFF, 0xFF, 0xFF, 0xFF };
                for (int i=0; i<4 && slot >= LastSlot;i++)
                {
                    msg[i+5]= slot;
                    slot--;
                }
                retVal.Add(msg);
            }
            return retVal;
        }

        private static bool SetHighSpeedMode()
        {
            byte[] msg = new byte[] { Priority.Physical0, DeviceId.Broadcast, DeviceId.Tool, Mode.HighSpeedPrepare };
            Response<byte[]> resp = obdx.SendDVIPacket(msg);
            if (resp.Status != ResponseStatus.Success)
            {
                LoggerBold(resp.Status.ToString());
                return false;
            }
            msg = new byte[] { Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, Mode.HighSpeed };
            resp = obdx.SendDVIPacket(msg);
            if (resp.Status != ResponseStatus.Success)
            {
                LoggerBold(resp.Status.ToString());
                return false;
            }
            Logger("High speed mode on");
            return true;
        }

        public static void StartLogging(OBDXProDevice obdxdev, bool set4x)
        {
            try
            {
                stop = false;
                obdx = obdxdev;
                if (!SetBusQuiet())
                    return;
                if (!SetMode1())
                    return;
                if (SetHighSpeedMode())
                {
                    if (set4x)
                    {
                        obdx.Set4xSpeed();
                    }
                }
                List<byte[]> commands = CreatePidSetupMessages();
                for (int c = 0; c < commands.Count; c++)
                {
                    Debug.WriteLine("Setup pids:");
                    Response<byte[]> resp = obdx.SendDVIPacket(commands[c]);
                    if (resp.Status != ResponseStatus.Success)
                    {
                        LoggerBold(resp.Status.ToString());
                        return;
                    }
                }
                commands = CreateStartMessages();
                for (int c = 0; c < commands.Count; c++)
                {
                    Debug.WriteLine("Start message: " + BitConverter.ToString(commands[c]));
                    Response<byte[]> resp = obdx.SendDVIPacket(commands[c]);
                    if (resp.Status != ResponseStatus.Success)
                    {
                        LoggerBold(resp.Status.ToString());
                        return;
                    }
                }
                obdx.StartLogging();                
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

        public static void StopLogging()
        {
            try
            {
                stop = true;
                obdx.StopLogging();
                if (logwriter.BaseStream != null)
                    logwriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static bool SendTesterPresent()
        {
            //Priority.Physical0, DeviceId.Pcm, DeviceId.Tool, 0x2c, slot, 0x4A 
            byte[] presentMsg = { Priority.Physical0, DeviceId.Broadcast, DeviceId.Tool, 0x3F };
            //presentMsg[presentMsg.Length - 1] = obdx.CalcChecksum(presentMsg);
            //obdx.SendData(presentMsg);
            Debug.WriteLine("Tester present");
            obdx.SendDVIPacket(presentMsg, false);
            return true;
        }

        public class ReadValue
        {
            public string PidName { get; set; }
            public double PidValue { get; set; }
            public int index { get; set; }
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
                    Slot slot = slotpids.Where(x => x.Id == slotNr).FirstOrDefault();
                    if (slot == null)
                    {
                        return retVal;
                    }
                    for (int p = 0; p < slot.PidIndex.Count; p++)
                    {
                        PidConfig pc = PidProfile[slot.PidIndex[p]];
                        byte elementSize = pc.GetElmentSize();
                        if (msg.Length >= (pos + elementSize))
                        {
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
    }
}
