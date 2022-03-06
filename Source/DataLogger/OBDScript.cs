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
    class OBDScript
    {
        public class Variable
        {
            public string Name { get; set; }
            public UInt64 Value { get; set; }
            public int Size { get; set; }
        }
        List<Variable> variables = new List<Variable>();
        Device device;
        MessageReceiver receiver;

        private bool HandleLine(string Line)
        {
            if (Line.Length > 1)
            {
                if (Line.StartsWith("#"))
                {
                    Debug.WriteLine(Line);
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
                else if (Line.ToLower().StartsWith("t:"))
                {
                    int delay = 0;
                    if (int.TryParse(Line.Substring(2), out delay))
                    {
                        Debug.WriteLine("Delay: {0} ms ", delay);
                        Thread.Sleep(delay);
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

                    for (int x = 0; x < variables.Count; x++)
                    {
                        if (variables[x].Name == parts[1])
                        {
                            v = variables[x];
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
                    byte[] msg = msgTxt.ToBytes();
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
                        int.TryParse(parts[2], out timeout);
                    }
                    Debug.WriteLine("Sending data: " + BitConverter.ToString(msg));
                    OBDMessage oMsg = new OBDMessage(msg);
                    device.SendMessage(oMsg, responses);
                    Thread.Sleep(UniversalPatcher.Properties.Settings.Default.LoggerScriptDelay);
                    DateTime starttime = DateTime.Now;
                    for (int r = 0; r < responses;)
                    {
                        OBDMessage rMsg = device.ReceiveMessage();
                        if (rMsg != null)
                        {
                            starttime = DateTime.Now;   //Message received, reset timer
                            r++;
                        }
                        if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(timeout))
                        {
                            Debug.WriteLine("Timeout: " + timeout.ToString() + " ms");
                            break;
                        }
                        Application.DoEvents();
                    }
                }
            }
            return true;
        }

        public void UploadScript(string FileName, Device device, MessageReceiver receiver)
        {
            try
            {
                this.device = device;
                this.receiver = receiver;
                device.ClearMessageQueue();
                StreamReader sr = new StreamReader(FileName);
                string Line;
                receiver.SetReceiverPaused(true);
                while ((Line = sr.ReadLine()) != null)
                {
                    if (Line.ToLower().StartsWith("loop"))
                    {
                        string[] parts = Line.Split(':');
                        int cycles;
                        if (parts.Length != 2 || !int.TryParse(parts[1], out cycles))
                        {
                            LoggerBold("Loop must have 2 parts: loop:<cycles>");
                            return;
                        }
                        List<string> lines = new List<string>();
                        while ((Line = sr.ReadLine()) != null)
                        {
                            if (Line.ToLower().StartsWith("endloop"))
                            {
                                break;
                            }
                            lines.Add(Line);
                        }
                        Debug.WriteLine("Loop " + lines.Count.ToString() + " lines, " + cycles.ToString() + " cycles ");
                        for (int cycle = 0; cycle < cycles; cycle++)
                        {
                            for (int l=0; l<lines.Count;l++)
                            {
                                if (!HandleLine(lines[l]))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else if (!HandleLine(Line))
                    {
                        return;
                    }
                }
                sr.Close();
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
