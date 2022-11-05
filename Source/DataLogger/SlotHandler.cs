using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using static LoggerUtils;
using static UniversalPatcher.DataLogger;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class SlotHandler
    {
        public SlotHandler()
        {

        }
        public int ReceivedHPRows = 0;
        public int ReceivedLPRows = 0;
        public Dictionary<byte, int> SlotIndex = new Dictionary<byte, int>();
        public double[] LastPidValues;
        public List<int> ReceivingPids = new List<int>();
        public List<int> HighPriorityPids = new List<int>();
        public List<int> LowPriorityPids = new List<int>();
        public List<Slot> Slots;
        private List<Slot> HighPrioritySlots;
        private List<Slot> LowPrioritySlots;
        public int CurrentSlotIndex = 0;
        public int CurrentHPSlotIndex = 0;
        public int CurrentLPSlotIndex = 0;
        private double[] newPidValues;
        private double[] newHighPriorityPidValues;
        private double[] lowPriorityPidValues;

        public class Slot
        {
            public Slot(byte Id)
            {
                this.Id = Id;
                Pids = new List<int>();
                DataTypes = new List<ProfileDataType>();
            }
            public byte Id { get; set; }
            public List<int> Pids { get; set; }
            public List<ProfileDataType> DataTypes { get; set; }
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
                    case ProfileDataType.UBYTE:
                        return 1;
                    case ProfileDataType.SBYTE:
                        return 1;
                    case ProfileDataType.UWORD:
                        return 2;
                    case ProfileDataType.SWORD:
                        return 2;
                    default:
                        return 1;
                }
            }
        }

        public string[] CalculatePidValues(double[] rawPidValues)
        {
            string[] calculatedvalues = new string[datalogger.PidProfile.Count];
            for (int pp = 0; pp < datalogger.PidProfile.Count; pp++)
            {
                calculatedvalues[pp] = "";
                PidConfig pc = datalogger.PidProfile[pp];
                int ind = ReceivingPids.IndexOf(pc.addr);
                if (ind > -1)
                {
                    if (rawPidValues[ind] > double.MinValue)
                    {
                        double val1 = rawPidValues[ind];
                        double val2 = double.MinValue;
                        if (pc.addr2 > -1)
                        {
                            int ind2 = ReceivingPids.IndexOf(pc.addr2);
                            if (ind2 > -1)
                                val2 = rawPidValues[ind2];
                        }
                        calculatedvalues[pp] = pc.GetCalculatedValue(val1, val2);
                    }
                }
                else
                {
                    //Debug.WriteLine("Pid not found: " + PidProfile[pp].PidName);
                }
            }
            return calculatedvalues;
        }

        public double[] CalculatePidDoubleValues(double[] rawPidValues)
        {
            double[] calculatedvalues = new double[datalogger.PidProfile.Count];
            try
            {
                for (int pp = 0; pp < datalogger.PidProfile.Count; pp++)
                {
                    calculatedvalues[pp] = double.MinValue;
                    PidConfig pc = datalogger.PidProfile[pp];
                    int ind = ReceivingPids.IndexOf(pc.addr);
                    if (ind > -1)
                    {
                        if (rawPidValues[ind] > double.MinValue)
                        {
                            double val1 = rawPidValues[ind];
                            double val2 = double.MinValue;
                            if (pc.addr2 > -1)
                            {
                                int ind2 = ReceivingPids.IndexOf(pc.addr2);
                                if (ind2 > -1)
                                    val2 = rawPidValues[ind2];
                            }
                            calculatedvalues[pp] = pc.GetCalculatedDoubleValue(val1, val2);
                        }
                    }
                    else
                    {
                        //Debug.WriteLine("Pid not found: " + PidProfile[pp].PidName);
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
                LoggerBold("Error, CalculatePidDoubleValues line " + line + ": " + ex.Message);
            }
            return calculatedvalues;
        }

        public bool CreatePidSetupMessages()
        {
            try
            {
                int pidIndex = 0;
                byte SlotNr = 0;
                byte position = 1;
                sbyte step = 1;

                ReceivingPids = new List<int>();
                Slots = new List<Slot>();
                HighPriorityPids = new List<int>();
                LowPriorityPids = new List<int>();
                HighPrioritySlots = new List<Slot>();
                LowPrioritySlots = new List<Slot>();
                SlotIndex = new Dictionary<byte, int>();
                List<ProfileDataType> pidDataTypes = new List<ProfileDataType>();

                if (datalogger.reverseSlotNumbers)
                {
                    step = -1;
                    SlotNr = 0xFE;
                }

                //Generate unique list of pids, HighPriority first:
                for (int p = 0; p < datalogger.PidProfile.Count; p++)
                {
                    if (datalogger.PidProfile[p].HighPriority && !ReceivingPids.Contains(datalogger.PidProfile[p].addr))
                    {
                        ReadValue rv = datalogger.QuerySinglePidValue(datalogger.PidProfile[p].addr, datalogger.PidProfile[p].DataType);
                        if (rv.PidValue > double.MinValue)
                        {
                            ReceivingPids.Add(datalogger.PidProfile[p].addr);
                            pidDataTypes.Add(datalogger.PidProfile[p].DataType);
                            HighPriorityPids.Add(datalogger.PidProfile[p].addr);
                        }
                    }
                    if (datalogger.PidProfile[p].HighPriority && datalogger.PidProfile[p].addr2 > 0 && !ReceivingPids.Contains((ushort)datalogger.PidProfile[p].addr2))
                    {
                        ReadValue rv = datalogger.QuerySinglePidValue((ushort)datalogger.PidProfile[p].addr2, datalogger.PidProfile[p].DataType);
                        if (rv.PidValue > double.MinValue)
                        {
                            ReceivingPids.Add((ushort)datalogger.PidProfile[p].addr2);
                            pidDataTypes.Add(datalogger.PidProfile[p].DataType);
                            HighPriorityPids.Add(datalogger.PidProfile[p].addr);
                        }
                    }
                }
                //Low priority pids:
                for (int p = 0; p < datalogger.PidProfile.Count; p++)
                {
                    if (!datalogger.PidProfile[p].HighPriority && !ReceivingPids.Contains(datalogger.PidProfile[p].addr))
                    {
                        ReadValue rv = datalogger.QuerySinglePidValue(datalogger.PidProfile[p].addr, datalogger.PidProfile[p].DataType);
                        if (rv.PidValue > double.MinValue)
                        {
                            ReceivingPids.Add(datalogger.PidProfile[p].addr);
                            pidDataTypes.Add(datalogger.PidProfile[p].DataType);
                            LowPriorityPids.Add(datalogger.PidProfile[p].addr);
                        }
                    }
                    if (!datalogger.PidProfile[p].HighPriority && datalogger.PidProfile[p].addr2 > 0 && !ReceivingPids.Contains((ushort)datalogger.PidProfile[p].addr2))
                    {
                        ReadValue rv = datalogger.QuerySinglePidValue((ushort)datalogger.PidProfile[p].addr2, datalogger.PidProfile[p].DataType);
                        if (rv.PidValue > double.MinValue)
                        {
                            ReceivingPids.Add((ushort)datalogger.PidProfile[p].addr2);
                            pidDataTypes.Add(datalogger.PidProfile[p].DataType);
                            LowPriorityPids.Add(datalogger.PidProfile[p].addr);
                        }
                    }
                }

                //Add pids to Slots
                while (pidIndex < ReceivingPids.Count)
                {
                    Slot slot = new Slot(SlotNr);
                    bool isHP = false;
                    while (pidIndex < ReceivingPids.Count && (slot.Bytes + GetElementSize((InDataType)pidDataTypes[pidIndex])) <= 6)
                    {
                        if (HighPriorityPids.Contains(ReceivingPids[pidIndex]))
                        {
                            isHP = true;
                        }
                        slot.Pids.Add(ReceivingPids[pidIndex]);
                        slot.DataTypes.Add(pidDataTypes[pidIndex]);
                        pidIndex++;
                    }
                    if (isHP)   //Slot contains at least one High Priority pid
                    {
                        HighPrioritySlots.Add(slot);
                    }
                    else
                    {
                        LowPrioritySlots.Add(slot);
                    }
                    Slots.Add(slot);
                    SlotIndex.Add(SlotNr, Slots.Count - 1);
                    SlotNr = (byte)(SlotNr + step);
                }

                //Slots planned, let's create commands:
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
                        OBDMessage rMsg = datalogger.LogDevice.ReceiveMessage();
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
                            rMsg = datalogger.LogDevice.ReceiveMessage();
                        }
                        pidIndex++;
                        position += bytes;
                    }
                }
                newPidValues = new double[ReceivingPids.Count];
                newHighPriorityPidValues = new double[HighPriorityPids.Count];
                lowPriorityPidValues = new double[LowPriorityPids.Count]; //Only used for speed calculation
                LastPidValues = new double[ReceivingPids.Count];

                //Clear arrays with minimal values
                for (int b = 0; b < LastPidValues.Length; b++)
                {
                    LastPidValues[b] = double.MinValue;
                    newPidValues[b] = double.MinValue;
                }
                for (int b = 0; b < newHighPriorityPidValues.Length; b++)
                {
                    newHighPriorityPidValues[b] = double.MinValue;
                }
                for (int b = 0; b < lowPriorityPidValues.Length; b++)
                {
                    lowPriorityPidValues[b] = double.MinValue;
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
                LoggerBold("Error, CreatePidSetupMessages line " + line + ": " + ex.Message);
                return false;
            }
            return true;
        }

        public byte[] CreateNextSlotRequestMessage()
        {
            List<byte> msg = new List<byte> { datalogger.priority, DeviceId.Pcm, DeviceId.Tool, Mode.SendDynamicData, datalogger.Responsetype };
            try
            {
                int addedHighPrioSlots = 0;

                List<int> LPSlotPlaces = new List<int>();
                switch (LowPrioritySlots.Count)
                {
                    case 1:
                        LPSlotPlaces.Add(1);
                        break;
                    case 2:
                        LPSlotPlaces.Add(1);
                        break;
                    case 3:
                        LPSlotPlaces.Add(1);
                        LPSlotPlaces.Add(3);
                        break;
                    case 4:
                        LPSlotPlaces.Add(1);
                        LPSlotPlaces.Add(3);
                        break;
                    case 5:
                        LPSlotPlaces.Add(1);
                        if (datalogger.maxSlotsPerMessage == 4)
                        {
                            LPSlotPlaces.Add(2);
                            LPSlotPlaces.Add(3);
                        }
                        else
                        {
                            LPSlotPlaces.Add(3);
                            LPSlotPlaces.Add(5);
                        }
                        break;
                    case 6:
                        LPSlotPlaces.Add(1);
                        if (datalogger.maxSlotsPerMessage == 4)
                        {
                            LPSlotPlaces.Add(2);
                            LPSlotPlaces.Add(3);
                        }
                        else
                        {
                            LPSlotPlaces.Add(3);
                            LPSlotPlaces.Add(5);
                        }
                        break;
                    case 7:
                        if (datalogger.maxSlotsPerMessage == 4)
                        {
                            datalogger.HighPriority = false;
                        }
                        else
                        {
                            LPSlotPlaces.Add(0);
                            LPSlotPlaces.Add(2);
                            LPSlotPlaces.Add(4);
                            LPSlotPlaces.Add(6);
                        }
                        break;
                    case 8:
                        if (datalogger.maxSlotsPerMessage == 4)
                        {
                            datalogger.HighPriority = false;
                        }
                        else
                        {
                            LPSlotPlaces.Add(0);
                            LPSlotPlaces.Add(2);
                            LPSlotPlaces.Add(4);
                            LPSlotPlaces.Add(6);
                        }
                        break;
                    case 9:
                        if (datalogger.maxSlotsPerMessage == 4)
                        {
                            throw new Exception("Too many Slots");
                        }
                        else
                        {
                            LPSlotPlaces.Add(1);
                            LPSlotPlaces.Add(2);
                            LPSlotPlaces.Add(3);
                            LPSlotPlaces.Add(4);
                            LPSlotPlaces.Add(5);
                        }
                        break;
                    case 10:
                        if (datalogger.maxSlotsPerMessage == 4)
                        {
                            throw new Exception("Too many Slots");
                        }
                        else
                        {
                            LPSlotPlaces.Add(1);
                            LPSlotPlaces.Add(2);
                            LPSlotPlaces.Add(3);
                            LPSlotPlaces.Add(4);
                            LPSlotPlaces.Add(5);
                        }
                        break;
                    default:
                        datalogger.HighPriority = false;
                        break;
                }

                for (int i = 0; i < datalogger.maxSlotsPerMessage; i++)
                {
                    if (datalogger.HighPriority)
                    {
                        if (HighPrioritySlots.Count > 0 && (!LPSlotPlaces.Contains(i) || LowPrioritySlots.Count == 0))
                        {
                            msg.Add(HighPrioritySlots[CurrentHPSlotIndex].Id);
                            CurrentHPSlotIndex++;
                            if (CurrentHPSlotIndex >= HighPrioritySlots.Count)
                                CurrentHPSlotIndex = 0;
                            addedHighPrioSlots++;
                        }
                        else
                        {
                            msg.Add(LowPrioritySlots[CurrentLPSlotIndex].Id);
                            CurrentLPSlotIndex++;
                            if (CurrentLPSlotIndex >= LowPrioritySlots.Count)
                                CurrentLPSlotIndex = 0;
                        }
                    }
                    else
                    {
                        msg.Add(Slots[CurrentSlotIndex].Id);
                        CurrentSlotIndex++;
                        if (CurrentSlotIndex >= Slots.Count)
                            CurrentSlotIndex = 0;
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
                LoggerBold("Error, CreateNextSlotRequestMessage line " + line + ": " + ex.Message);
            }

            Debug.WriteLine("Slot Request msg:" + BitConverter.ToString(msg.ToArray()) + " HighPriority: " + datalogger.HighPriority.ToString());
            return msg.ToArray();
        }
        public List<ReadValue> ParseMessage(OBDMessage msg)
        {
            List<ReadValue> retVal = new List<ReadValue>();
            try
            {
                if (msg.Length > 6)
                {
                    if (msg[1] != DeviceId.Tool || msg[2] != DeviceId.Pcm)
                    {
                        Debug.WriteLine("This message is not for us?");
                        return retVal;
                    }
                    byte SlotNr = msg[4];
                    Debug.WriteLine("Parsing message: " + msg.ToString() + " - Slot: " + SlotNr.ToString("X"));
                    int pos = 5;
                    Slot slot = Slots[SlotIndex[SlotNr]];
                    if (slot == null)
                    {
                        return retVal;
                    }
                    for (int p = 0; p < slot.Pids.Count; p++)
                    {
                        int pidNr = slot.Pids[p];
                        //Debug.Write(pidNr.ToString("X2") + " ");
                        byte elementSize = slot.PidSize(p);
                        if (msg.Length >= (pos + elementSize))
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
                            }
                            ReadValue rv = new ReadValue();
                            rv.PidNr = pidNr;
                            rv.TimeStamp = msg.TimeStamp;
                            rv.SysTimeStamp = msg.SysTimeStamp;
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
                LoggerBold("Error, ParseMessage line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        public void HandleSlotMessage(OBDMessage oMsg)
        {
            List<ReadValue> newReadValues = ParseMessage(oMsg);
            if (datalogger.HighPriority && !LastPidValues.Contains(double.MinValue))
            {
                //All pids received at least once
                for (int r = 0; r < newReadValues.Count; r++)
                {
                    ReadValue rv = newReadValues[r];
                    int ind = ReceivingPids.IndexOf(rv.PidNr);
                    if (ind > -1)
                        LastPidValues[ind] = rv.PidValue;   //Update directly lastpidvalues, lowpriority pids already there from previous round
                    int hpInd = HighPriorityPids.IndexOf(rv.PidNr);
                    if (hpInd > -1)
                    {
                        //Debug.WriteLine("Received HP pid: " + rv.PidNr.ToString("X2"));
                        newHighPriorityPidValues[hpInd] = rv.PidValue;
                    }
                    else
                    {
                        int lpInd = LowPriorityPids.IndexOf(rv.PidNr);
                        if (lpInd > -1)
                        {
                            //Debug.WriteLine("Received LP pid: " + rv.PidNr.ToString("X2"));
                            lowPriorityPidValues[lpInd] = rv.PidValue;
                        }
                    }
                }

                if (!newHighPriorityPidValues.Contains(double.MinValue))
                {
                    //Debug.WriteLine("All HP pids received");
                    //All High priority pids received
                    //if (datalogger.writelog) //Always add data to logging queue, for graph & histogram
                    {
                        LogData ld = new LogData(LastPidValues.Length);
                        //ld.TimeStamp = newReadValues[0].TimeStamp;
                        ld.TimeStamp = oMsg.TimeStamp;
                        ld.SysTimeStamp = oMsg.SysTimeStamp;
                        Array.Copy(LastPidValues, ld.Values, ld.Values.Length);
                        lock (datalogger.LogFileQueue)
                        {
                            datalogger.LogFileQueue.Enqueue(ld);
                        }
                    }
                    //"Clear" array, so we know when all values are received
                    for (int b = 0; b < newHighPriorityPidValues.Length; b++)
                    {
                        newHighPriorityPidValues[b] = double.MinValue;
                    }
                    ReceivedHPRows++;
                }

                if (!lowPriorityPidValues.Contains(double.MinValue))
                {
                    for (int b = 0; b < lowPriorityPidValues.Length; b++)
                    {
                        lowPriorityPidValues[b] = double.MinValue;
                    }
                    ReceivedLPRows++;
                }
            }
            else //Not priority, or not all pids received yet.
            {
                for (int r = 0; r < newReadValues.Count; r++)
                {
                    ReadValue rv = newReadValues[r];
                    int ind = ReceivingPids.IndexOf(rv.PidNr);
                    if (ind > -1)
                        newPidValues[ind] = rv.PidValue;
                }
                if (!newPidValues.Contains(double.MinValue))
                {
                    //All pids received
                    //if (datalogger.writelog) //Always add data to logging queue, for graph & histogram
                    {
                        LogData ld = new LogData(newPidValues.Length);
                        ld.TimeStamp = newReadValues[0].TimeStamp;
                        ld.SysTimeStamp = newReadValues[0].SysTimeStamp;
                        Array.Copy(newPidValues, ld.Values, ld.Values.Length);
                        lock (datalogger.LogFileQueue)
                        {
                            datalogger.LogFileQueue.Enqueue(ld);
                        }
                    }
                    Array.Copy(newPidValues, LastPidValues, LastPidValues.Length);
                    //"Clear" array, so we know when all values are received
                    for (int b = 0; b < newPidValues.Length; b++)
                    {
                        newPidValues[b] = double.MinValue;
                    }
                    ReceivedHPRows++;
                }
            }

        }


    }
}
