using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static LoggerUtils;
using static UniversalPatcher.DataLogger;
using System.Diagnostics;
using static UniversalPatcher.PidConfig;
using static Upatcher;

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
        public int CurrentHPSlotIndex = 0;
        public int CurrentLPSlotIndex = 0;
        private double[] newPidValues;

        public bool VPWProtocol = true;

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
                DataTypes = new List<PidConfig.ProfileDataType>();
            }
            public byte Id { get; set; }
            public List<int> Pids { get; set; }
            public List<PidConfig.ProfileDataType> DataTypes { get; set; }
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
                    case PidConfig.ProfileDataType.UBYTE:
                        return 1;
                    case PidConfig.ProfileDataType.SBYTE:
                        return 1;
                    case PidConfig.ProfileDataType.UWORD:
                        return 2;
                    case PidConfig.ProfileDataType.SWORD:
                        return 2;
                    //case ProfileDataType.THREEBYTES:
                      //  return 3;
                    case PidConfig.ProfileDataType.UINT32:
                        return 4;
                    case PidConfig.ProfileDataType.INT32:
                        return 4;
                    default:
                        return 1;
                }
            }
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
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    double pidVal = double.MinValue;
                    if (HexToInt(parm.Id, out int addr))
                    {
                        int idx = ReceivingPids.IndexOf(addr);
                        pidVal = pidValues[idx];
                    }
                    calculatedvalues[pp] = parm.GetCalculatedStringValue(pidProfile, false);
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
                    
                    calculatedDoubleValues[pp] = parm.GetCalculatedValue(pidProfile);
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
            return calculatedDoubleValues;
        }

        public bool CreatePidSetupMessages()
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
                List<ProfileDataType> pidDataTypes = new List<ProfileDataType>();

                if (datalogger.reverseSlotNumbers)
                {
                    step = -1;
                    SlotNr = 0xFE;
                }

                if (!VPWProtocol)
                {
                    bytesPerSlot = 7;
                }
                //Generate unique list of pids, HighPriority first:
                foreach (LogParam.PidSettings pidProfile in datalogger.SelectedPids)
                {
                     
                    LogParam.PidParameter parm = pidProfile.Parameter;
                    if (parm == null)
                    {
                        LoggerBold("Skipping unknown PID: " + parm.Name);
                        continue;
                    }
                    if (parm.Type == LogParam.DefineBy.Pid)
                    {
                        if (HexToInt(parm.Id.Replace("0x", ""), out int addr) && !ReceivingPids.Contains(addr))
                        {
                            ReceivingPids.Add(addr);
                            pidDataTypes.Add((ProfileDataType)parm.DataType);
                        }
                    }
                    else if (parm.Type == LogParam.DefineBy.Address)
                    {
                        int addr = pidProfile.Os.addr;
                        if (addr >= 0 && !ReceivingPids.Contains(addr))
                        {
                            ReceivingPids.Add(addr);
                            pidDataTypes.Add((ProfileDataType)parm.DataType);
                        }
                    }
                    else if (parm.Type == LogParam.DefineBy.Math)
                    {
                        foreach (LogParam.LogPid lPid in parm.Pids)
                        {
                            LogParam.PidParameter mParm = lPid.Parameter;
                            if (mParm != null)
                            {
                                if (HexToInt(mParm.Id.Replace("0x", ""), out int addr) && !ReceivingPids.Contains(addr))
                                {
                                    ReceivingPids.Add(addr);
                                    pidDataTypes.Add((PidConfig.ProfileDataType)mParm.DataType);
                                }

                            }
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
                                byte cfgByte = CreateConfigByte(position, bytes, (byte)DefineBy.Address);
                                msg[5] = cfgByte;
                            }
                            else
                            {
                                msg[6] = (byte)(Slots[s].Pids[pidIndex] >> 8);
                                msg[7] = (byte)(Slots[s].Pids[pidIndex]);
                                bytes = Slots[s].PidSize(pidIndex);
                                byte cfgByte = CreateConfigByte(position, bytes, (byte)DefineBy.Pid);
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
                            while (rMsg != null)
                            {
                                if (rMsg.Length > 4)
                                {
                                    Debug.WriteLine("Response: " + rMsg.ToString());
                                    if (rMsg[3] == 0x7f)
                                    {
                                        LoggerBold("Pid " + Slots[s].Pids[pidIndex].ToString("X4") + " Error: " + PcmResponses[rMsg.GetBytes().Last()]);
                                        return false;
                                    }
                                    if (rMsg[3] == 0x6C)
                                    {
                                        break;
                                    }
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
                        List<byte> msg = new List<byte> { 0x00, 0x00, datalogger.CanPcmAddrByte1, datalogger.CanPcmAddrByte2, 0x2C, SlotNr };
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
                        datalogger.LogDevice.ReceiveBufferedMessages();
                        pidIndex++;
                        position += bytes;
                    }
                }
                newPidValues = new double[ReceivingPids.Count];
                LastPidValues = new double[ReceivingPids.Count];

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

        public byte[] CreateNextSlotRequestMessage()
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
                msg = new List<byte> { 0x00, 0x00, datalogger.CanPcmAddrByte1, datalogger.CanPcmAddrByte2, 0xAA, mode };
            }
            try
            {
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
                    else
                    {
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
                                case ProfileDataType.UBYTE:
                                    val = (byte)msg[pos];
                                    pos++;
                                    break;
                                case ProfileDataType.SBYTE:
                                    val = (sbyte)msg[pos];
                                    pos++;
                                    break;
                                case ProfileDataType.UWORD:
                                    //val = BitConverter.ToUInt16(msg.GetBytes(), pos);
                                    UInt16 tmp = (UInt16)(msg.GetBytes()[pos] << 8);
                                    tmp += (byte)msg.GetBytes()[pos + 1];
                                    val = tmp;
                                    pos += 2;
                                    break;
                                case ProfileDataType.SWORD:
                                    //val = BitConverter.ToInt16(msg.GetBytes(), pos);
                                    Int16 itmp = (Int16)(msg.GetBytes()[pos] << 8);
                                    itmp += (byte)msg.GetBytes()[pos + 1];
                                    val = itmp;
                                    pos += 2;
                                    break;
                                case ProfileDataType.UINT32:
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
                                case ProfileDataType.INT32:
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
                        newPidValues[ind] = rv.PidValue;
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
