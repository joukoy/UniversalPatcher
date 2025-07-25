using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static LoggerUtils;
using static UniversalPatcher.DataLogger;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
//using static UniversalPatcher.PidConfig;

namespace UniversalPatcher
{
    public class SlotHandler
    {
        public SlotHandler(bool UseVPW)
        {
            VPWProtocol = UseVPW;
        }
        public int ReceivedRows = 0;
        public Dictionary<byte, int> SlotIndex = new Dictionary<byte, int>();
        public double[] LastPidValues;
        public List<int> ReceivingPids = new List<int>();
        public List<Slot> Slots;
        public int CurrentSlotIndex = 0;
        private double[] newPidValues;
        public List<int> PassiveSources;
        public bool VPWProtocol = true;
        //public PidValSetTime[] pidValSetTimes;
        public Hertz[] pidHertz;
        public class PidValSetTime
        {
            public PidValSetTime()
            {
                SetTimes = new List<DateTime>();
            }
            public List<DateTime> SetTimes { get; set; }
            public double Herz()
            {
                if (SetTimes.Count == 0)
                {
                    return -1;
                }
                if (DateTime.Now.Subtract(SetTimes.LastOrDefault()) > TimeSpan.FromMilliseconds(1000))
                {
                    return -1;
                }
                int last = SetTimes.Count - 1;
                for (int i = last; i >= 0; i--)
                {
                    TimeSpan tSpan = SetTimes[last].Subtract(SetTimes[i]);
                    if (tSpan > TimeSpan.FromMilliseconds(1000) && i < last - 3)
                    {
                        int count = last - i;
                        return count / tSpan.TotalSeconds;
                    }
                }
                return -1;
            }
        }
        public class PidVal
        {
            public PidVal(int Pid, double Val)
            {
                this.Pid = Pid;
                this.Val = Val;
            }
            public int Pid;
            public double Val;
        }

        public class Slot
        {
            public Slot(byte Id)
            {
                this.Id = Id;
                Pids = new List<int>();
                DataTypes = new List<LogParam.ProfileDataType>();
            }
            public byte Id { get; set; }
            public List<int> Pids { get; set; }
            public List<LogParam.ProfileDataType> DataTypes { get; set; }
            public byte Bytes
            {
                get
                {
                    byte retval = 0;
                    for (int i = 0; i < Pids.Count; i++)
                        retval += PidSize(i);
                    return retval;
                }
            }
            public byte PidSize(int pidindex)
            {
                switch (DataTypes[pidindex])
                {
                    case LogParam.ProfileDataType.UBYTE:
                        return 1;
                    case LogParam.ProfileDataType.SBYTE:
                        return 1;
                    case LogParam.ProfileDataType.UWORD:
                        return 2;
                    case LogParam.ProfileDataType.SWORD:
                        return 2;
                    //case ProfileDataType.THREEBYTES:
                      //  return 3;
                    case LogParam.ProfileDataType.UINT32:
                        return 4;
                    case LogParam.ProfileDataType.INT32:
                        return 4;
                    default:
                        return 1;
                }
            }
        }

        public double GetPidHerz(int pid)
        {
            int idx = ReceivingPids.IndexOf(pid);
            if (idx < 0)
                return -1;
            return pidHertz[idx].GetHertz();
        }

        public double GetLastPidValue(int pid)
        {
            int idx = ReceivingPids.IndexOf(pid);
            if (idx < 0)
                return double.MinValue;
            return LastPidValues[idx];
        }
        public string[] CalculatePidValues(double[] pidValues)
        {
            //double[] calculatedDoubleValues = CalculatePidDoubleValues(pidValues);
            string[] calculatedvalues = new string[datalogger.SelectedPids.Count];
            try
            {
                for (int pp=0;pp<datalogger.SelectedPids.Count;pp++)
                {
                    LogParam.PidSettings pidProfile = datalogger.SelectedPids[pp];
                    calculatedvalues[pp] = pidProfile.Parameter.GetCalculatedStringValue(pidProfile, false);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, SlotHandler line " + line + ": " + ex.Message);
            }
            return calculatedvalues;
        }

        public double[] CalculatePidDoubleValues(double[] rawPidValues)
        {
            double[] calculatedDoubleValues = new double[datalogger.SelectedPids.Count];
            try
            {
                for (int pp = 0; pp < datalogger.SelectedPids.Count; pp++)
                {
                    LogParam.PidSettings pidProfile = datalogger.SelectedPids[pp];
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    
                    calculatedDoubleValues[pp] = parm.GetCalculatedValue(pidProfile,false);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, Slothandler line " + line + ": " + ex.Message);
            }
            return calculatedDoubleValues;
        }
        public void SetupPassivePidFilters()
        {
            for (int d=0;d<PassiveSources.Count;d++)
            {
                string PassFilters = "Type: PASS_FILTER,Name: CANloggerPass" + d.ToString() + Environment.NewLine;
                PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                PassFilters += "Pattern: " + PassiveSources[d].ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
                datalogger.LogDevice.SetupFilters(PassFilters, true, false);

            }
        }
        public void ResetFilters()
        {
            Debug.WriteLine("Slothandler reset filters");
            string FlowFilters = "Type: FLOW_CONTROL_FILTER,Name: CANloggerFlow" + Environment.NewLine;
            FlowFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
            FlowFilters += "Pattern: " + datalogger.CanDevice.ResID.ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
            FlowFilters += "FlowControl: " + datalogger.CanDevice.RequestID.ToString("X8") + ",RxStatus:NONE,TxFlags:NONE" + Environment.NewLine;
            datalogger.LogDevice.SetupFilters(FlowFilters, false, true);

            string PassFilters = "Type: PASS_FILTER,Name: CANloggerPass" + Environment.NewLine;
            PassFilters += "Mask: FFFFFFFF,RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
            PassFilters += "Pattern: " + datalogger.CanDevice.DiagID.ToString("X8") + ",RxStatus: NONE,TxFlags: NONE" + Environment.NewLine;
            datalogger.LogDevice.SetupFilters(PassFilters, true, true);
            
        }

        public bool CreatePidSetupMessages(List<LogParam.PidSettings> SelectedPids)
        {
            try
            {
                int pidIndex = 0;
                byte SlotNr = 0;
                byte position = 1;
                sbyte step = 1;
                int bytesPerSlot = 6;

                ReceivingPids = new List<int>();
                Slots = new List<Slot>();
                SlotIndex = new Dictionary<byte, int>();
                List<LogParam.ProfileDataType> pidDataTypes = new List<LogParam.ProfileDataType>();
                PassiveSources = new List<int>();

                if (datalogger.reverseSlotNumbers)
                {
                    step = -1;
                    SlotNr = 0xFE;
                }

                if (!VPWProtocol)
                {
                    bytesPerSlot = 7;
                    //ResetFilters();
                }
                //Generate unique list of pids:
                foreach (LogParam.PidSettings pidProfile in SelectedPids)
                {
                     
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    if (parm == null)
                    {
                        LoggerBold("Skipping unknown PID: " + parm.Name);
                        continue;
                    }
                    if (parm.Conversions.Count == 0 || (parm.Conversions.Count > 0 && string.IsNullOrEmpty(parm.Conversions[0].Units)))
                    {
                        Debug.WriteLine("No conversion, adding Raw");
                        parm.Conversions.Add(new Conversion("Raw", "x", "0.00"));
                        pidProfile.Units = parm.Conversions[0];
                    }
                    if (parm.Type == LogParam.DefineBy.Pid)
                    {
                        if (!ReceivingPids.Contains(parm.Address))
                        {
                            Debug.WriteLine("Adding pid: " + parm.Id + ", Datatype: " + parm.DataType.ToString());
                            ReceivingPids.Add(parm.Address);
                            pidDataTypes.Add((LogParam.ProfileDataType)parm.DataType);
                        }
                    }
                    else if (parm.Type == LogParam.DefineBy.Address)
                    {
                        int addr = pidProfile.Os.addr;
                        if (addr >= 0 && !ReceivingPids.Contains(addr))
                        {
                            ReceivingPids.Add(addr);
                            pidDataTypes.Add((LogParam.ProfileDataType)parm.DataType);
                        }
                    }
                    else if (parm.Type == LogParam.DefineBy.Math)
                    {
                        List<LogParam.PidParameter> mParms = parm.GetLinkedPids();
                        foreach (LogParam.PidParameter mParm in mParms)
                        {
                            if (mParm.Type == LogParam.DefineBy.Pid && !ReceivingPids.Contains(mParm.Address))
                            {
                                ReceivingPids.Add(mParm.Address);
                                pidDataTypes.Add((LogParam.ProfileDataType)mParm.DataType);
                            }
                        }
                    }
                    else if (parm.Type == LogParam.DefineBy.Passive)
                    {
                        if (!PassiveSources.Contains(parm.Address))
                        {
                            PassiveSources.Add(parm.Address);
                        }
                    }
                }
                //Add pids to Slots
                while (pidIndex < ReceivingPids.Count)
                {
                    Slot slot = new Slot(SlotNr);
                    while (pidIndex < ReceivingPids.Count && (slot.Bytes + GetElementSize((InDataType)pidDataTypes[pidIndex])) <= bytesPerSlot)
                    {
                        slot.Pids.Add(ReceivingPids[pidIndex]);
                        slot.DataTypes.Add(pidDataTypes[pidIndex]);
                        pidIndex++;
                    }
                    Slots.Add(slot);
                    SlotIndex.Add(SlotNr, Slots.Count - 1);
                    SlotNr = (byte)(SlotNr + step);
                }

                //Slots planned, let's create commands:
                if (VPWProtocol)
                {
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        pidIndex = 0;
                        position = 1;
                        byte bytes = 0;
                        while (pidIndex < Slots[s].Pids.Count)
                        {
                            SlotNr = Slots[s].Id;
                            byte[] msg = { datalogger.priority, DeviceId.Pcm, DeviceId.Tool, 0x2C, SlotNr, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                            if (Slots[s].Pids[pidIndex] > ushort.MaxValue) //RAM
                            {
                                msg[6] = (byte)(Slots[s].Pids[pidIndex] >> 16);
                                msg[7] = (byte)(Slots[s].Pids[pidIndex] >> 8);
                                msg[8] = (byte)(Slots[s].Pids[pidIndex]);
                                bytes = Slots[s].PidSize(pidIndex);
                                byte cfgByte = CreateConfigByte(position, bytes, (byte)LogParam.DefineBy.Address);
                                msg[5] = cfgByte;
                            }
                            else
                            {
                                msg[6] = (byte)(Slots[s].Pids[pidIndex] >> 8);
                                msg[7] = (byte)(Slots[s].Pids[pidIndex]);
                                bytes = Slots[s].PidSize(pidIndex);
                                byte cfgByte = CreateConfigByte(position, bytes, (byte)LogParam.DefineBy.Pid);
                                msg[5] = cfgByte;
                                //Debug.WriteLine("ConfigByte: " + cfgByte.ToString("X"));
                            }
                            Debug.WriteLine("Pid setup msg:" + BitConverter.ToString(msg));
                            bool resp = datalogger.LogDevice.SendMessage(new OBDMessage(msg), 1);
                            if (!resp)
                            {
                                LoggerBold("Error, Pid setup failed");
                                return false;
                            }
                            OBDMessage rMsg = datalogger.LogDevice.ReceiveMessage(true);
                            DateTime starttime = DateTime.Now;
                            while (true)
                            {
                                if (rMsg != null && rMsg.Length > 4)
                                {
                                    Debug.WriteLine("Response: " + rMsg.ToString());
                                    if (rMsg[3] == 0x7f)
                                    {
                                        LoggerBold("Pid " + Slots[s].Pids[pidIndex].ToString("X4") + " Error: " + PcmResponses[rMsg.GetBytes().Last()]);
                                        return false;
                                    }
                                    if (rMsg[3] == 0x6C && rMsg[4] == SlotNr)
                                    {
                                        Debug.WriteLine("Received positive response, pid ok");
                                        break;
                                    }
                                }
                                if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(1500))
                                {
                                    LoggerBold("Timeout in pid setup");
                                    return false;
                                }
                                rMsg = datalogger.LogDevice.ReceiveMessage(true);
                            }
                            pidIndex++;
                            position += bytes;
                        }
                    }
                }
                else //CAN
                {
                    for (int s = 0; s < Slots.Count; s++)
                    {
                        pidIndex = 0;
                        position = 1;
                        byte bytes = 0;
                        SlotNr = Slots[s].Id;
                        List<byte> msg = new List<byte> { 0x00, 0x00, datalogger.CanDevice.RequestByte1, datalogger.CanDevice.RequestByte2, 0x2C, SlotNr };
                        while (pidIndex < Slots[s].Pids.Count)
                        {
                            msg.Add((byte)(Slots[s].Pids[pidIndex] >> 8));
                            msg.Add((byte)(Slots[s].Pids[pidIndex]));
                            pidIndex++;
                        }
                        Debug.WriteLine("Pid setup msg:" + BitConverter.ToString(msg.ToArray()));
                        bool resp = datalogger.LogDevice.SendMessage(new OBDMessage(msg.ToArray()), 1);
                        if (!resp)
                        {
                            LoggerBold("Error, Pid setup failed");
                            return false;
                        }
                        OBDMessage rMsg = datalogger.LogDevice.ReceiveMessage(true);
                        DateTime starttime = DateTime.Now;
                        while (true)
                        {
                            if (rMsg != null && rMsg.Length > 5)
                            {
                                Debug.WriteLine("Response: " + rMsg.ToString());
                                if (rMsg[4] == 0x7f)
                                {
                                    LoggerBold("Pid " + Slots[s].Pids[pidIndex-1].ToString("X4") + " Error: " + PcmResponses[rMsg.GetBytes().Last()]);
                                    return false;
                                }
                                if (rMsg[4] == 0x6C && rMsg[5] == SlotNr)
                                {
                                    Debug.WriteLine("Positive response, pid is supported");
                                    break;
                                }
                            }
                            if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(1000))
                            {
                                LoggerBold("Timeout in pid setup");
                                return false;
                            }
                            rMsg = datalogger.LogDevice.ReceiveMessage(true);
                        }

                        //datalogger.LogDevice.ReceiveBufferedMessages();
                        pidIndex++;
                        position += bytes;
                    }
                }
                newPidValues = new double[ReceivingPids.Count];
                LastPidValues = new double[ReceivingPids.Count];
                pidHertz = new Hertz[ReceivingPids.Count];
                for (int v=0;v<pidHertz.Length;v++)
                {
                    pidHertz[v] = new Hertz();
                }

                //Clear arrays with minimal values
                for (int b = 0; b < LastPidValues.Length; b++)
                {
                    LastPidValues[b] = double.MinValue;
                    newPidValues[b] = double.MinValue;
                }

                for (int i = 0; i < Slots.Count; i++)
                {
                    Debug.Write("Slot: " + Slots[i].Id.ToString("X2") + ", Pids: ");
                    for (int j = 0; j < Slots[i].Pids.Count; j++)
                    {
                        Debug.Write(Slots[i].Pids[j].ToString("X2") + " ");
                    }
                    Debug.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Slothandler line " + line + ": " + ex.Message);
                return false;
            }
            return true;
        }

        public byte[] CreateNextSlotRequestMessage(bool FirstRequest)
        {
            List<byte> msg;
            if (VPWProtocol)
            {
                msg = new List<byte> { datalogger.priority, DeviceId.Pcm, DeviceId.Tool, Mode.SendDynamicData, datalogger.Responsetype };
            }
            else
            {
                byte mode = 0;
                switch(datalogger.Responsetype)
                {
                    case ResponseType.SendOnce:
                        mode = 1;
                        break;
                    case ResponseType.SendSlowly:
                        mode = 2;
                        break;
                    case ResponseType.SendMedium1:
                        mode = 3;
                        break;
                    case ResponseType.SendMedium2:
                        mode = 3;
                        break;
                    case ResponseType.SendFast:
                        mode = 4;
                        break;
                }
                msg = new List<byte> { 0x00, 0x00, datalogger.CanDevice.RequestByte1, datalogger.CanDevice.RequestByte2, 0xAA, mode };
            }
            try
            {
                if (Slots.Count == 0)
                {
                    return null;
                }
                for (int i = 0; i < datalogger.maxSlotsPerMessage; i++)
                {
                    msg.Add(Slots[CurrentSlotIndex].Id);
                    CurrentSlotIndex++;
                    if (CurrentSlotIndex >= Slots.Count)
                        CurrentSlotIndex = 0;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Slothandler line " + line + ": " + ex.Message);
            }

            Debug.WriteLine("Slot Request msg:" + BitConverter.ToString(msg.ToArray()) );
            return msg.ToArray();
        }

        public static UInt64 ExtractPassivePidData(byte[] data, int startBit, int bitCount, bool MSB)
        {
            if (data == null || bitCount <= 0 || bitCount > 64)
            {
                Logger("Invalid input parameters.");
                return UInt64.MaxValue;
            }

            int byteIndex = startBit / 8;  // Start byte index
            int bitOffset = startBit % 8;  // Offset within the byte
            if (MSB)
            {
                bitOffset = 7 - bitOffset;
            }
            int totalBits = bitOffset + bitCount;  // Total bits including offset
            int bytesNeeded = (totalBits + 7) / 8; // Number of bytes required
            if (!MSB)
            {
                byteIndex = data.Length -byteIndex - bytesNeeded;
            }
            if (byteIndex + bytesNeeded > data.Length)
            {
                Logger("Not enough data bytes to extract the requested bits.");
                return UInt64.MaxValue;
            }
            UInt64 extractedValue = 0;
            for (int i = 0; i < bytesNeeded; i++)
            {
                extractedValue |= (UInt64)((UInt64)data[byteIndex + i] << ((bytesNeeded - i - 1) * 8));
            }
            if (MSB)
            {
                int lastbit = bitOffset + bitCount;
                int leadingbits = (7 - lastbit + 1) % 8;
                extractedValue >>= leadingbits;  // Shift right to remove unwanted leading bits
            }
            else
            {
                extractedValue >>= bitOffset;  // Shift right to remove unwanted leading bits
            }
            extractedValue &= (UInt64)((1 << bitCount) - 1);  // Mask only required bits

            return extractedValue;
        }

        public static string ExtractPassivePidText(byte[] data, int startBit, int bitCount, bool MSB)
        {
            if (data == null || bitCount <= 0)
                throw new ArgumentException("Invalid input parameters.");

            int byteIndex = startBit / 8;  // Start byte index
            int bitOffset = startBit % 8;  // Offset within the byte
            if (MSB)
            {
                bitOffset = 7 - bitOffset; //Bits: 76543210 76543210 ....
            }
            int totalBits = bitOffset + bitCount;  // Total bits including offset
            int bytesNeeded = (totalBits + 7) / 8; // Number of bytes required
            if (!MSB)
            {
                byteIndex = data.Length - byteIndex - bytesNeeded;
            }

            if (byteIndex + bytesNeeded > data.Length)
                throw new ArgumentException("Not enough data bytes to extract the requested bits.");

            byte[] extractedBytes = new byte[bytesNeeded];
            for (int i = 0; i < bytesNeeded; i++)
            {
                extractedBytes[i] = (byte)(data[byteIndex + i]);
            }
            return Encoding.ASCII.GetString(extractedBytes).TrimEnd('\0');
        }

        public List<ReadValue> ParseMessage(OBDMessage msg)
        {
            List<ReadValue> retVal = new List<ReadValue>();
            try
            {
                if (msg.Length > 6)
                {
                    int pos = 5; 
                    if (VPWProtocol)
                    {
                        if (msg[1] != DeviceId.Tool || msg[2] != DeviceId.Pcm)
                        {
                            Debug.WriteLine("This message is not for us?");
                            return retVal;
                        }
                    }
                    else //CAN
                    { 
                       
                        int src = ReadInt32(msg.Data, 0, true);
                        if (PassiveSources.Contains(src))
                        {
                            Debug.WriteLine("CANID match, possible passive pid: CanID: " + src.ToString("X8") + ", MsgLength: " + (msg.Length -4).ToString());
                            List<LogParam.PidSettings> pids = datalogger.SelectedPids.Where(X => X.Parameter.Type == LogParam.DefineBy.Passive && 
                                X.Parameter.Address == src && X.Parameter.PassivePid.MsgLength == (msg.Length - 4)).ToList();
                            if (pids.Count > 0)
                            {
                                Debug.WriteLine("Found " + pids.Count.ToString() + " matching passive pids");
                                foreach (LogParam.PidSettings ps in pids)
                                {
                                    Debug.WriteLine("Match with " + ps.Parameter.Name);
                                    if (ps.Parameter.PassivePid.NumBits > 32)
                                    {
                                        ps.LastPassiveTextValue = ExtractPassivePidText(msg.Data, ps.Parameter.PassivePid.BitStartPos + 32,
                                            ps.Parameter.PassivePid.NumBits, ps.Parameter.PassivePid.MSB);
                                    }
                                    else
                                    {
                                        UInt64 val = ExtractPassivePidData(msg.Data, ps.Parameter.PassivePid.BitStartPos + 32,
                                            ps.Parameter.PassivePid.NumBits, ps.Parameter.PassivePid.MSB);
                                        ps.LastPassiveRawValue = val;
                                    }
                                }
                                return retVal;
                            }
                        }                            
                        if (msg[0] != 0x00 || msg[3] != 0xE8)
                        {
                            Debug.WriteLine("This message is not for us?");
                            return retVal;
                        }
                    }
                    byte SlotNr = msg[4];
                    Debug.WriteLine("Parsing message: " + msg.ToString() + " - Slot: " + SlotNr.ToString("X"));
                    if (!SlotIndex.ContainsKey(SlotNr))
                    {
                        Debug.WriteLine("Slot not found: " + SlotNr.ToString("X"));
                        return retVal;
                    }
                    Slot slot = Slots[SlotIndex[SlotNr]];
                    if (slot == null)
                    {
                        return retVal;
                    }
                    for (int p = 0; p < slot.Pids.Count; p++)
                    {
                        int pidNr = slot.Pids[p];
                        //Debug.Write(pidNr.ToString("X2") + " ");
                        //byte elementSize = slot.PidSize(p);
                        int dataLen = msg.Length - pos;
                        //if (msg.Length >= (pos + elementSize))
                        if (dataLen > 0)
                        {
                            //Debug.WriteLine("Received pid (index): " + pid);
                            double val = 0;
                            switch (slot.DataTypes[p])
                            {
                                case LogParam.ProfileDataType.UBYTE:
                                    val = (byte)msg[pos];
                                    pos++;
                                    break;
                                case LogParam.ProfileDataType.SBYTE:
                                    val = (sbyte)msg[pos];
                                    pos++;
                                    break;
                                case LogParam.ProfileDataType.UWORD:
                                    //val = BitConverter.ToUInt16(msg.GetBytes(), pos);
                                    UInt16 tmp = (UInt16)(msg.GetBytes()[pos] << 8);
                                    tmp += (byte)msg.GetBytes()[pos + 1];
                                    val = tmp;
                                    pos += 2;
                                    break;
                                case LogParam.ProfileDataType.SWORD:
                                    //val = BitConverter.ToInt16(msg.GetBytes(), pos);
                                    Int16 itmp = (Int16)(msg.GetBytes()[pos] << 8);
                                    itmp += (byte)msg.GetBytes()[pos + 1];
                                    val = itmp;
                                    pos += 2;
                                    break;
                                case LogParam.ProfileDataType.UINT32:
                                    if (dataLen == 4)
                                    {
                                        val = BitConverter.ToUInt32(msg.GetBytes(), pos);
                                        pos += 4;
                                    }
                                    else if (dataLen == 3)
                                    {
                                        UInt16 tmp3 = (UInt16)(msg.GetBytes()[pos] << 16);
                                        tmp3 += (UInt16)(msg.GetBytes()[pos + 1] << 8);
                                        tmp3 += (byte)msg.GetBytes()[pos + 2];
                                        val = tmp3;
                                        pos += 3;
                                    }
                                    break;
                                case LogParam.ProfileDataType.INT32:
                                    val = BitConverter.ToInt32  (msg.GetBytes(), pos);
                                    pos += 4;
                                    break;
                            }
                            ReadValue rv = new ReadValue();
                            rv.PidNr = pidNr;
                            rv.TimeStamp = msg.TimeStamp;
                            rv.PidValue = val;
                            retVal.Add(rv);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Debug.Write("Slot: " + SlotNr.ToString("X2"));
                    for (int p=0;p<retVal.Count;p++)
                    {
                         Debug.Write(", pid: " + retVal[p].PidNr.ToString("X4") + ", value: " + retVal[p].PidValue.ToString());
                    }
                    Debug.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Slothandler line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        public void HandleSlotMessage(OBDMessage oMsg)
        {
            try
            {
                List<ReadValue> newReadValues = ParseMessage(oMsg);
                for (int r = 0; r < newReadValues.Count; r++)
                {
                    ReadValue rv = newReadValues[r];
                    int ind = ReceivingPids.IndexOf(rv.PidNr);
                    if (ind > -1)
                    {
                        newPidValues[ind] = rv.PidValue;
                        pidHertz[ind].AddTime();
                    }
                }
                if (!newPidValues.Contains(double.MinValue))
                {
                    Array.Copy(newPidValues, LastPidValues, LastPidValues.Length);
                    Debug.WriteLine("All  pids received");
                    LogData ld = new LogData(LastPidValues.Length);
                    ld.TimeStamp = oMsg.TimeStamp;
                    Array.Copy(newPidValues, ld.Values, ld.Values.Length);
                    lock (datalogger.LogFileQueue)
                    {
                        datalogger.LogFileQueue.Enqueue(ld);
                    }
                    //"Clear" array, so we know when all values are received
                    for (int b = 0; b < newPidValues.Length; b++)
                    {
                        newPidValues[b] = double.MinValue;
                    }
                    ReceivedRows++;

                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Slothandler line " + line + ": " + ex.Message);
            }
        }
    }
}
