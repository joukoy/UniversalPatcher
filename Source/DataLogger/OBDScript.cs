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
    public class OBDScript
    {
        public OBDScript(Device device, MessageReceiver receiver)
        {
            this.device = device;
            this.receiver = receiver;
            SecondaryProtocol = false;
        }

        public class Variable
        {
            public string Name { get; set; }
            public UInt64 Value { get; set; }
            public int Size { get; set; }
        }
        List<Variable> variables = new List<Variable>();
        Device device;
        MessageReceiver receiver;
        ushort key = 0xFFFF;
        int globaldelay = UniversalPatcher.Properties.Settings.Default.LoggerScriptDelay;
        int breakvalue = -1;
        int breaknotvalue = -1;
        int breakposition = -1;
        public bool stopscript { get; set; }
        List<string> scriptrows;
        int currentrow = 0;
        public bool SecondaryProtocol { get; set; }

        /// <summary>
        /// Calc checksum for byte array for all messages to/from device
        /// </summary>
        private ushort CalcChecksum(byte[] MyArr)
        {
            ushort CalcChksm = 0;
            for (ushort i = 0; i < MyArr.Length; i++)
            {
                //CalcChksm += (ushort)(MyArr[i] << 8 | MyArr[i+1]);
                CalcChksm += (ushort)MyArr[i];
            }
            return CalcChksm;
        }

        public bool HandleLine(string Line, string Line2, ref bool breakloop)
        {
            if (stopscript)
            {
                return false;
            }
            if (Line.Length > 1)
            {
                if (Line.StartsWith("#"))
                {
                    Debug.WriteLine(Line);
                }
                else if (Line.ToLower().StartsWith("setvariable"))
                {
                    if (Line2.Length == 0 )
                    {
                        LoggerBold("Setvariable requires command, but end of file reached?");
                        return false;
                    }
                    SetVariable(Line, Line2);
                }
                else if (Line.ToLower().StartsWith("getseed"))
                {
                    if (Line2.Length == 0)
                    {
                        LoggerBold("getseed requires 2 commands, but end of file reached?");
                        return false;
                    }
                    key = GetSeed(Line, Line2);
                }
                else if (Line.ToLower().StartsWith("break"))
                {
                    string[] parts = Line.Split(':');
                    if (parts.Length != 3)
                    {
                        LoggerBold("Break format: break:position:value");
                    }
                    if (!int.TryParse(parts[1], out breakposition))
                    {
                        LoggerBold("Unknown position: " + parts[1] +" (" + Line +")");
                    }
                    byte val;
                    if (!HexToByte(parts[2], out val))
                    {
                        LoggerBold("Unknown value: " + parts[1] + " (" + Line + ")");
                    }
                    if (parts[0] == "break!")
                        breaknotvalue = (int)val;
                    else
                        breakvalue = (int)val;
                }
                else if (Line.ToLower().StartsWith("globaldelay:"))
                {
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out globaldelay))
                    {
                        Debug.WriteLine("Set global delay to:" + globaldelay.ToString());
                    }
                }
                else if (Line.ToLower().StartsWith("setspeed:"))
                {
                    if (Line.Contains("4x"))
                    {
                        Debug.WriteLine("Setting VPW speed to 4x");
                        device.SetVpwSpeed(VpwSpeed.FourX);
                    }
                    else if (Line.Contains("1x"))
                    {
                        Debug.WriteLine("Setting VPW speed to 1x");
                        device.SetVpwSpeed(VpwSpeed.Standard);
                    }
                    else
                    {
                        Logger("Unknown speed: " + Line.Substring(9));
                    }
                }
                else if (Line.ToLower().StartsWith("t:") || Line.ToLower().StartsWith("delay:"))
                {
                    int delay = 0;
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out delay))
                    {
                        Debug.WriteLine("Delay: {0} ms ", delay);
                        Thread.Sleep(delay);
                    }
                }
                else if (Line.ToLower().StartsWith("t:") || Line.ToLower().StartsWith("readtimeout:"))
                {
                    int tout = 0;
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out tout))
                    {
                        Debug.WriteLine("Setting timeout to: {0} ms ", tout);
                        device.SetReadTimeout(tout);
                    }
                }
                else if (Line.ToLower().StartsWith("t:") || Line.ToLower().StartsWith("writetimeout:"))
                {
                    int tout = 0;
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out tout))
                    {
                        Debug.WriteLine("Setting write timeout to: {0} ms ", tout);
                        device.SetWriteTimeout(tout);
                    }
                }
                else if (Line.ToLower().StartsWith("variable"))
                {
                    Variable v = new Variable();
                    string[] parts = Line.Split(':');
                    if (parts.Length != 4)
                    {
                        LoggerBold("Variable must be in format: variable:name:size:value");
                        return false;
                    }
                    v.Name = parts[1];

                    bool newVar = true;
                    for (int x = 0; x < variables.Count; x++)
                    {
                        if (variables[x].Name == parts[1])
                        {
                            v = variables[x];
                            newVar = false;
                            break;
                        }
                    }
                    int size;
                    if (!int.TryParse(parts[2], out size))
                    {
                        LoggerBold("Unknown size: " + parts[2] + " (" + Line + ")");
                        return false;
                    }
                    v.Size = size;
                    UInt64 val;
                    if (!HexToUint64(parts[3].Replace("+","").Replace("-", ""), out val))
                    {
                        LoggerBold("Unknown value (HEX): " + parts[3] + " (" + Line + ")");
                        return false;
                    }
                    if (parts[3].StartsWith("+"))
                    {
                        v.Value += val;
                    }
                    else if (parts[3].StartsWith("-"))
                    {
                        v.Value -= val;
                    }
                    else
                    {
                        v.Value = val;
                    }
                    if (newVar)
                    {
                        variables.Add(v);
                    }
                    Debug.WriteLine("Variable: " + v.Name + ", value: " + v.Value.ToString("X"));
                }
                else
                {
                    string[] parts = Line.Split(':');
                    string msgTxt = parts[0].Replace(" ", "");
                    for (int x=0; x<variables.Count;x++)
                    {
                        if (msgTxt.Contains(variables[x].Name))
                        {
                            string xformat = "X" + (variables[x].Size * 2).ToString("X");
                            msgTxt = msgTxt.Replace(variables[x].Name, variables[x].Value.ToString(xformat));
                        }
                    }                    
                    msgTxt = msgTxt.Replace("key", key.ToString("X4"));

                    bool addChecksum = false;
                    if (msgTxt.ToLower().Contains("blchk"))
                    {
                        addChecksum = true;
                        msgTxt = msgTxt.Replace("blchk", "");
                    }

                    byte[] msg = msgTxt.ToBytes();

                    if (addChecksum)
                    {
                        byte[] datablock = new byte[msg.Length - 4];
                        Array.Copy(msg, 4, datablock, 0, msg.Length - 4);
                        ushort chk = CalcChecksum(datablock);
                        byte[] newMsg = new byte[msg.Length + 2];
                        Array.Copy(msg, newMsg, msg.Length);
                        newMsg[newMsg.Length - 2] = (byte)(chk >> 8);
                        newMsg[newMsg.Length - 1] = (byte)(chk);
                        msg = newMsg;
                    }
                    int responses = 1;
                    if (msg.Length > 3 && msg[3] == 0x3f)
                    {
                        responses = 0;
                    }
                    if (parts.Length > 1)
                    {
                        int.TryParse(parts[1], out responses);
                    }
                    int timeout = 50;
                    if (parts.Length > 2)
                    {
                        if (int.TryParse(parts[2], out timeout))
                        {
                            device.SetWriteTimeout(timeout);
                            Debug.WriteLine("Setting write timeout to: " + timeout.ToString());
                        }
                    }
                    Debug.WriteLine("Sending data: " + BitConverter.ToString(msg));
                    OBDMessage oMsg = new OBDMessage(msg);
                    oMsg.SecondaryProtocol = SecondaryProtocol;
                    device.ClearMessageQueue();
                    device.SendMessage(oMsg, responses);
                    Thread.Sleep(globaldelay);
                    DateTime starttime = DateTime.Now;
                    for (int r = 0; r < responses;)
                    {
                        OBDMessage rMsg = device.ReceiveMessage();
                        if (rMsg != null && rMsg.Length > 3 && rMsg[0] == oMsg[0] && rMsg[1] == oMsg[2] && rMsg[2] == oMsg[1])
                        {
                            starttime = DateTime.Now;   //Message received, reset timer
                            r++;
                            if (breakposition >= 0 && breakposition < rMsg.Length)
                            {
                                if (breakvalue == rMsg[breakposition])
                                {
                                    breakloop = true;
                                    Debug.WriteLine("Byte at position: " + breakposition.ToString() + " have value: " + breakvalue.ToString("X") + " => Exit loop");
                                    return true;
                                }
                                else
                                {
                                    Debug.WriteLine("Byte at position: " + breakposition.ToString() + " == " + rMsg[breakposition].ToString("X") + " != "+ breakvalue.ToString("X") + " => Continue");
                                }
                                if (breaknotvalue > -1)
                                {
                                    if (breaknotvalue != rMsg[breakposition])
                                    {
                                        breakloop = true;
                                        Debug.WriteLine("Byte at position: " + breakposition.ToString() + " value != " + breaknotvalue.ToString("X") + " => Exit loop");
                                        return true;
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Byte at position: " + breakposition.ToString() + " == " + rMsg[breakposition].ToString("X") + " => Continue");
                                    }
                                }
                            }
                        }
                        if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(timeout))
                        {
                            Debug.WriteLine("Timeout: " + timeout.ToString() + " ms");
                            break;
                        }
                        Application.DoEvents();
                    }
                    breakposition = -1;
                    breakvalue = -1;
                    breaknotvalue = -1;
                }
            }
            return true;
        }

        private void SetVariable(string Line, string Line2)
        {
            string[] parts = Line.Split(':');
            Variable v = new Variable();
            if (parts.Length != 4)
            {
                LoggerBold(parts[0] + " must be in format: " + parts[0] + ":name:size:position");
                return;
            }
            v.Name = parts[1];
            bool newVar = true;
            for (int x = 0; x < variables.Count; x++)
            {
                if (variables[x].Name == parts[1])
                {
                    v = variables[x];
                    newVar = false;
                    break;
                }
            }
            int size;
            if (!int.TryParse(parts[2], out size))
            {
                LoggerBold("Unknown size: " + parts[2] + " (" + Line + ")");
                return;
            }
            v.Size = size;
            int position;
            if (!int.TryParse(parts[3], out position))
            {
                LoggerBold("Unknown position: " + parts[3] + " (" + Line + ")");
                return;
            }
            byte[] msg = Line2.Replace(" ", "").ToBytes();
            Debug.WriteLine("Sending data: " + BitConverter.ToString(msg));
            OBDMessage oMsg = new OBDMessage(msg);
            oMsg.SecondaryProtocol = SecondaryProtocol;
            device.SendMessage(oMsg, 1);
            Thread.Sleep(globaldelay);
            OBDMessage rMsg = null;
            DateTime starttime = DateTime.Now;
            for (int r = 0; r < 1;)
            {
                rMsg = device.ReceiveMessage();
                if (rMsg != null && rMsg.Length >= (position + 2))
                {
                    starttime = DateTime.Now;   //Message received, reset timer
                    r++;
                }
                if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(100))
                {
                    Debug.WriteLine("Timeout: 100 ms");
                    return;
                }
                Application.DoEvents();
            }
            UInt64 val = 0;
            for (int p = 0; p < size; p++)
            {
                if (p + position >= rMsg.Length)
                {
                    LoggerBold("Too short message? " + rMsg.ToString());
                    return;
                }
                val += (UInt64)(rMsg[p + position] << ((size - p - 1) * 8));
            }
            v.Value = val;
            if (newVar)
            {
                variables.Add(v);
            }
            Debug.WriteLine("Variable: " + v.Name + ", value: " + v.Value.ToString("X"));

        }

        private ushort GetSeed(string Line, string Line2)
        {
            ushort returnValue = 0xFFFF;
            for (int x = 0; x < variables.Count; x++)
            {
                if (Line.Contains(variables[x].Name))
                {
                    string xformat = "X" + (variables[x].Size * 2).ToString("X");
                    Line = Line.Replace(variables[x].Name, variables[x].Value.ToString(xformat));
                }
            }

            string[] parts = Line.Split(':');
            if (parts.Length != 3)
            {
                LoggerBold(parts[0] + " must be in format: " + parts[0] + ":position:algo");
                return 0xFFFF;
            }
            int position;
            if (!int.TryParse(parts[1], out position))
            {
                LoggerBold("Unknown position: " + parts[1] + " (" + Line + ")");
                return returnValue;
            }
            int algo;
            if (!HexToInt(parts[2], out algo))
            {
                LoggerBold("Unknown algo: " + parts[2] + " (" + Line + ")");
                return returnValue;
            }

            byte[] msg = Line2.Replace(" ", "").ToBytes();
            Debug.WriteLine("Sending data: " + BitConverter.ToString(msg));
            OBDMessage oMsg = new OBDMessage(msg);
            oMsg.SecondaryProtocol = SecondaryProtocol;
            device.SendMessage(oMsg, 1);
            //Thread.Sleep(globaldelay);
            OBDMessage rMsg = null;
            DateTime starttime = DateTime.Now;
            for (int r = 0; r < 1;)
            {
                rMsg = device.ReceiveMessage();
                if (rMsg != null && rMsg.Length >= (position + 2))
                {
                    starttime = DateTime.Now;   //Message received, reset timer
                    r++;
                }
                if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(100))
                {
                    Debug.WriteLine("Timeout: 100 ms");
                    return returnValue;
                }
                Application.DoEvents();
            }
            ushort seed = (ushort)((rMsg[position] << 8) + rMsg[position + 1]);
            ushort key = KeyAlgorithm.GetKey(algo, seed);
            Debug.WriteLine("Seed: " + seed.ToString("X4") + ", Key: " + key.ToString("X4") + ", Algo: " + algo.ToString("X"));
            return key;
        }

        public void UploadScript(string FileName)
        {
            try
            {
                stopscript = false;
                string Line;

                scriptrows = new List<string>();
                StreamReader sr = new StreamReader(FileName);
                while ((Line = sr.ReadLine()) != null)
                {
                    scriptrows.Add(Line);
                }
                sr.Close();

                receiver.SetReceiverPaused(true);
                device.ClearMessageQueue();
                for (;currentrow<scriptrows.Count; currentrow++)
                {
                    Line = scriptrows[currentrow];
                    bool breakloop = false;
                    if (Line.ToLower().StartsWith("loop"))
                    {
                        string[] parts = Line.Split(':');
                        int cycles;
                        if (parts.Length != 2 || !int.TryParse(parts[1], out cycles))
                        {
                            LoggerBold("Loop must have 2 parts: loop:<cycles>");
                            break;
                        }
                        List<string> lines = new List<string>();
                        while (currentrow < scriptrows.Count)
                        {
                            currentrow++;
                            Line = scriptrows[currentrow];
                            if (Line.ToLower().StartsWith("endloop"))
                            {
                                break;
                            }
                            lines.Add(Line);
                        }
                        Debug.WriteLine("Loop " + lines.Count.ToString() + " lines, " + cycles.ToString() + " cycles ");
                        for (int cycle = 0; cycle < cycles; cycle++)
                        {
                            int l = 0;
                            for (; l<lines.Count;l++)
                            {
                                string Line2 = "";
                                if (l<lines.Count-1)
                                {
                                    Line2 = lines[l + 1];
                                }
                                if (!HandleLine(lines[l], Line2, ref breakloop))
                                {
                                    return;
                                }
                                if (lines[l].ToLower().Contains("getseed")|| lines[l].ToLower().Contains("setvariable"))
                                {
                                    l++;
                                }
                                if (breakloop)
                                {
                                    break;
                                }
                            }
                            if (breakloop)
                            {
                                break;
                            }
                        }
                    }
                    else 
                    {
                        string Line2 = "";
                        if (currentrow < scriptrows.Count - 1)
                        {
                            Line2 = scriptrows[currentrow + 1];
                        }
                        if (!HandleLine(Line, Line2, ref breakloop))
                        {
                            break;
                        }
                        if (scriptrows[currentrow].ToLower().Contains("getseed") || scriptrows[currentrow].ToLower().Contains("setvariable"))
                        {
                            currentrow++;
                        }

                    }
                }
                device.SetWriteTimeout(500); //Revert to default
                receiver.SetReceiverPaused(false);
            }
            catch (Exception ex)
            {
                receiver.SetReceiverPaused(false);
                LoggerBold(ex.Message);
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, UploadScript line " + line + ": " + ex.Message);
            }
        }

    }
}
