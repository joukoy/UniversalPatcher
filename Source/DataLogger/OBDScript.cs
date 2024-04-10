using J2534DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using static Helpers;
using static LoggerUtils;
using static Upatcher;

namespace UniversalPatcher
{
    public class OBDScript
    {
        public OBDScript(Device device, JConsole Jconsole)
        {
            this.jconsole = Jconsole;
            this.device = device;
            SecondaryProtocol = false;
            LoadFilters();
        }

        public class Variable
        {
            public string Name { get; set; }
            public UInt64 Value { get; set; }
            public int Size { get; set; }
        }
        List<Variable> variables = new List<Variable>();
        public Device device;
        UInt64 key = 0xFFFF;
        int globaldelay = AppSettings.LoggerScriptDelay;
        int breakvalue = -1;
        int breaknotvalue = -1;
        int breakposition = -1;
        public bool stopscript { get; set; }
        List<string> scriptrows;
        int currentrow = 0;
        public bool SecondaryProtocol { get; set; }
        int defaultResponses = 1;
        private J2534DotNet.TxFlag txflag;
        private J2534DotNet.RxStatus rxstatus;
        int receiveTimeout = AppSettings.TimeoutConsoleReceive;
        int writeTimeout = AppSettings.TimeoutScriptRead;
        int readTimeout = AppSettings.TimeoutScriptRead;
        private JConsole jconsole;
        private string ScriptFolder;
        private List<JFilter> savedFilters;
        private void LoadFilters()
        {
            if (File.Exists(savedFiltersFile))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<JFilter>));
                StreamReader file = new StreamReader(savedFiltersFile);
                savedFilters = (List<JFilter>)reader.Deserialize(file);
                file.Close();
            }
            else
            {
                savedFilters = new List<JFilter>();
            }
        }
        public bool ReConnect(string ProfileFileName)
        {
            J2534InitParameters initParameters = null;
            if (ProfileFileName != null)
            {
                string profileFile = Path.Combine(ScriptFolder, ProfileFileName);
                if (!File.Exists(profileFile))
                {
                    profileFile = Path.Combine(Application.StartupPath, "Logger", "J2534Profiles", ProfileFileName);
                }
                if (!File.Exists(profileFile))
                {
                    LoggerBold("File not found: " + ProfileFileName);
                    return false;
                }
                initParameters = LoadJ2534Settings(profileFile);
            }

            if (jconsole == null)
            {
                if (!frmlogger.Connect(false, true, false,this))
                {
                    return false;
                }
            }
            else
            {
                if (!frmlogger.ConnectJConsole(this,initParameters))
                {
                    return false;
                }
            }
            return true;
        }

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
                stopscript = false;
                return false;
            }
            Line = Line.Trim();
            Debug.WriteLine("Executing line: " + Line + " Line2: " + Line2);
            if (Line.Length > 1)
            {
                if (Line.StartsWith("#"))
                {
                    Debug.WriteLine(Line);
                }
                else if (Line.StartsWith("$"))
                {
                    string[] lParts = Line.Split('=');
                    if (lParts.Length == 2)
                    {
                        Variable v = new Variable();
                        v.Name = lParts[0];
                        v.Size = 1;
                        if (HexToUint(lParts[1], out uint val))
                        {
                            v.Value = val;
                            variables.Add(v);
                        }
                    }
                }
                else if (Line.ToLower().StartsWith("logger:"))
                {
                    string msgTxt = Line.Substring(7);
                    for (int x = 0; x < variables.Count; x++)
                    {
                        if (msgTxt.Contains(variables[x].Name))
                        {
                            string xformat = "X" + (variables[x].Size * 2).ToString("X");
                            msgTxt = msgTxt.Replace(variables[x].Name, variables[x].Value.ToString(xformat));
                        }
                    }
                    msgTxt = msgTxt.Replace("key", key.ToString("X4"));
                    Logger(msgTxt);
                }
                else if (Line.ToLower().StartsWith("disconnect"))
                {
                    if (jconsole == null)
                    {
                        frmlogger.Disconnect(false);
                    }
                    else
                    {
                        frmlogger.DisconnectJConsole(false);
                    }
                }
                else if (Line.ToLower().StartsWith("connect"))
                {
                    string[] jParts = Line.Split(':');
                    if (jParts.Length > 1)
                    {
                        return ReConnect(jParts[1]);
                    }
                    else
                    {
                        return ReConnect(null);
                    }
                }
                else if (Line.ToLower().StartsWith("savebin:"))
                {
                    string[] sParts = Line.Split(':');
                    string mode = "vpw";
                    int secondP = Line.Length;
                    if (sParts.Length >= 3)
                    {
                        mode = sParts[1].ToLower();
                        secondP = Line.IndexOf(":", 9) + 1 ;
                    }
                    string fName = Line.Substring(secondP);
                    if (fName.StartsWith("?") || string.IsNullOrEmpty(fName))
                    {
                        fName = SelectSaveFile(BinFilter);
                    }
                    if (fName.Length == 0)
                    {
                        LoggerBold("Missing filename for savebin!");
                        return false;
                    }
                    string[] logTxt;
                    if (jconsole == null)
                    {
                        logTxt = new string[frmlogger.logTexts.Count];
                        for (int r = 0; r < frmlogger.logTexts.Count; r++)
                        {
                            logTxt[r] = frmlogger.logTexts[r].Txt;
                        }
                    }
                    else
                    {
                        logTxt = new string[frmlogger.jconsolelogTexts.Count];
                        for (int r = 0; r < frmlogger.jconsolelogTexts.Count; r++)
                        {
                            logTxt[r] = frmlogger.jconsolelogTexts[r].Txt;
                        }
                    }
                    LogToBinConverter.RMode rMode = (LogToBinConverter.RMode)Enum.Parse(typeof(LogToBinConverter.RMode),mode.ToUpper());
                    LogToBinConverter ltb = new LogToBinConverter(rMode);
                    Application.DoEvents();
                    ltb.ConvertLines(logTxt, fName);
                }
                else if (Line.ToLower().StartsWith("askvariable:"))
                {
                    string[] vParts = Line.Split(':');
                    if (vParts.Length < 4)
                    {
                        LoggerBold("Usage: askvariable:variablename:size:hex/int[:comment]");
                        return false;
                    }
                    Variable v = new Variable();
                    v.Name = vParts[1];
                    v.Size = Convert.ToInt32(vParts[2]);
                    FrmAsk fa = new FrmAsk();
                    fa.Text = "Input value";
                    if (vParts.Length > 4)
                        fa.labelAsk.Text = vParts[4] + " (" + vParts[3] + ")";
                    else
                        fa.labelAsk.Text = vParts[2] + " (" + vParts[3] + ")";
                    if (fa.ShowDialog() == DialogResult.OK)
                    {
                        if (vParts[3].Contains("hex"))
                        {
                            if (HexToUint64(fa.TextBox1.Text, out UInt64 val))
                            {
                                v.Value = val;
                                variables.Add(v);
                            }
                            else
                            {
                                LoggerBold("Can't convert from HEX: " + fa.TextBox1.Text);
                                return false;
                            }
                        }
                        else
                        {
                            v.Value = Convert.ToUInt64(fa.TextBox1.Text);
                            variables.Add(v);
                        }
                    }
                    else
                    {
                        Logger("User cancel");
                        return false;
                    }
                }
                else if (Line.ToLower().StartsWith("printall"))
                {
                    bool printPrimary = true;
                    bool printSecondary = true;
                    string[] pParts = Line.Split(':');
                    int tOut = 10000;
                    if (pParts.Length > 1)
                    {
                        if (pParts[1] == ":1")
                        {
                            printSecondary = false;
                        }
                        else if (pParts[1] == ":2")
                        {
                            printPrimary = false;
                        }
                    }
                    if (pParts.Length > 2)
                    {
                        int.TryParse(pParts[2], out tOut);
                    }
                    DateTime starttime = DateTime.Now;
                    while (true)
                    {
                        if (printPrimary)
                        {
                            device.ReceiveMessage(true);
                        }
                        if (printSecondary)
                        {
                            device.ReceiveMessage2();
                        }
                        if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(tOut))
                        {
                            break;
                        }
                        Application.DoEvents();
                    }
                }
                else if (Line.ToLower().StartsWith("popup:"))
                {
                    string[] lParts = Line.Split(':');
                    if (lParts.Length == 2)
                    {
                        MessageBox.Show(lParts[1], "OBDScript");
                    }
                }
                else if (Line.ToLower().StartsWith("setconfigs:"))
                {
                    string[] lParts = Line.Split(':');
                    if (lParts.Length == 2)
                    {
                        device.SetJ2534Configs(lParts[1], SecondaryProtocol);
                    }
                }
                else if (Line.ToLower().StartsWith("setfilter:"))
                {
                    string[] lParts = Line.Split(':');
                    if (lParts.Length >= 3)
                    {
                        bool clearold = false;
                        bool secondary = SecondaryProtocol;
                        if (lParts[2].ToUpper().StartsWith("Y"))
                            clearold = true;
                        if (lParts.Length > 3 && lParts[3] == "1")
                            secondary = false;
                        if (lParts.Length > 3 && lParts[3] == "2")
                            secondary = true;
                        if (lParts.Length > 3 && lParts[3].ToLower().StartsWith("p"))   //Same as send msg, use another proto for sending
                            secondary = !SecondaryProtocol;
                        for (int f=0;f<savedFilters.Count;f++)
                        {
                            JFilter jf = savedFilters[f];
                            if (jf.FilterName == lParts[1])
                            {
                                device.SetupFilters(jf.Filter, secondary, clearold);
                                break;
                            }
                        }
                    }
                    else
                    {
                        LoggerBold("Usage: setfilter:filtername:<clearold>, example: setfilter:MyFilter01:N[:1/2]");
                    }
                }
                else if (Line.ToLower().StartsWith("clearfilters"))
                {
                    device.SetAnalyzerFilter();
                }
                else if (Line.ToLower().StartsWith("addtofunctmsg:"))
                {
                    string[] lParts = Line.Split(':');
                    if (lParts.Length == 2)
                    {
                        byte[] funcAddr = GetByteParameters(lParts[1]);
                        device.AddToFunctMsgLookupTable(funcAddr, SecondaryProtocol);
                    }
                    else
                    {
                        LoggerBold("Usage: addtofunctmsg:<FuncAddr>, example: addtofunctmsg:0A 12");
                    }
                }
                else if (Line.ToLower().StartsWith("deletefromfunctmsg:"))
                {
                    string[] lParts = Line.Split(':');
                    if (lParts.Length == 2)
                    {
                        byte[] funcAddr = GetByteParameters(lParts[1]);
                        device.DeleteFromFunctMsgLookupTable(funcAddr, SecondaryProtocol);
                    }
                    else
                    {
                        LoggerBold("Usage: deletefromfunctmsg:<FuncAddr>, example: deletefromfunctmsg:0A 12");
                    }
                }
                else if (Line.ToLower().StartsWith("clearfunctmsg"))
                {
                    device.ClearFunctMsgLookupTable(SecondaryProtocol);
                }
                else if (Line.ToLower().StartsWith("startperiodic:"))
                {
                    string pMsg = Line.Substring(Line.IndexOf(":") + 1);
                    device.StartPeriodicMsg(pMsg, SecondaryProtocol);
                }
                else if (Line.ToLower().StartsWith("stopperiodic:"))
                {
                    string pMsg = Line.Substring(Line.IndexOf(":") + 1);
                    device.StopPeriodicMsg(pMsg, SecondaryProtocol);
                }
                else if (Line.ToLower().StartsWith("clearperiodic"))
                {
                    device.ClearPeriodicMsg(SecondaryProtocol);
                }
                else if (Line.ToLower().StartsWith("wait:"))
                {
                    string[] lParts = Line.Split(':');
                    if (lParts.Length >= 2)
                    {
                        DateTime starttime = DateTime.Now;
                        string msg = lParts[1].Replace(" ", "");
                        OBDMessage rMsg;
                        string rMsgStr = "";
                        int timeo = 10000;
                        if (lParts.Length > 2 && int.TryParse(lParts[2], out int to))
                        {
                            timeo = to;
                        }
                        do
                        {
                            rMsg = device.ReceiveMessage(true);
                            Application.DoEvents();
                            if (rMsg == null)
                            {
                                Thread.Sleep(100);
                            }
                            else
                            {
                                rMsgStr = rMsg.ToString().Replace(" ", "");
                            }
                            if (rMsgStr != msg && DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(timeo))
                            {
                                Debug.WriteLine("Timeout (" + timeo.ToString() + "ms)");
                                break;
                            }

                        } while (rMsg == null || rMsgStr != msg);
                    }
                    else
                    {
                        LoggerBold("Usage: wait:<message>[:timeout]");
                    }
                }
                else if (Line.ToLower().StartsWith("programminvoltage:"))
                {
                    string[] setParts = Line.Split(':');
                    if (setParts.Length != 3)
                    {
                        LoggerBold("Usage: programminvoltage:PIN:Voltage (*1000)");
                        LoggerBold("Examples: programminvoltage:9:12000, programminvoltage:9:OFF programminvoltage:9:GND");
                        return false;
                    }
                    uint volts;
                    PinNumber pin;
                    if (setParts[2].ToLower().Trim() == "off")
                    {
                        volts = 0xFFFFFFFF;
                    }
                    else if (setParts[2].ToLower().Trim() == "gnd")
                    {
                        volts = 0xFFFFFFFE;
                    }
                    else if (!uint.TryParse(setParts[2], out volts))
                    {
                        LoggerBold("Unknown voltage: " + setParts[2]);
                        return false;
                    }
                    if (int.TryParse(setParts[1], out int pinInt))
                    {
                        pin = (PinNumber)pinInt;
                    }
                    else
                    {
                        LoggerBold("Unknown pin: " + setParts[1]);
                        return false;
                    }
                    device.SetProgramminVoltage(pin, volts);
                }
                else if (Line.ToLower().StartsWith("set:"))
                {
                    string[] setParts = Line.Split(':');
                    if (setParts.Length > 2)
                    {
                        switch (setParts[1].ToLower())
                        {
                            case "txflags":
                                txflag = ParseTxFLags(setParts[2]);
                                Debug.WriteLine("Setting TxFlags: " + txflag.ToString().Replace(",", "|"));
                                break;
                            case "rxstatus":
                                rxstatus = ParseRxStatus(setParts[2]);
                                Debug.WriteLine("Setting RxStatus: " + rxstatus.ToString().Replace(",", "|"));
                                break;
                        }
                    }
                }
                else if (Line.ToLower().StartsWith("clear:"))
                {
                    string[] setParts = Line.Split(':');
                    if (setParts.Length > 1)
                    {
                        switch (setParts[1].ToLower())
                        {
                            case "txflags":
                                txflag = J2534DotNet.TxFlag.NONE;
                                Debug.WriteLine("Clearing RxStatus");
                                break;
                            case "rxstatus":
                                rxstatus = J2534DotNet.RxStatus.NONE;
                                Debug.WriteLine("Clearing TxFlags");
                                break;
                        }
                    }
                }
                else if (Line.ToLower().StartsWith("setvariable"))
                {
                    if (Line2.Length == 0)
                    {
                        LoggerBold("Setvariable requires command, but end of file reached?");
                        return false;
                    }
                    SetVariable(Line, Line2);
                }
                else if (Line.ToLower().StartsWith("getseed") || Line.ToLower().StartsWith("eecvseed") || Line.ToLower().StartsWith("ngcengineseed"))
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
                        LoggerBold("Unknown position: " + parts[1] + " (" + Line + ")");
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
                else if (Line.ToLower().StartsWith("defaultresponses:"))
                {
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out defaultResponses))
                    {
                        Debug.WriteLine("Set default responses to:" + defaultResponses.ToString());
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
                else if (Line.ToLower().StartsWith("d:") || Line.ToLower().StartsWith("delay:"))
                {
                    int delay = 0;
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out delay))
                    {
                        Debug.WriteLine("Delay: {0} ms ", delay);
                        if (delay > 100)
                        {
                            DateTime startT = DateTime.Now;
                            while (DateTime.Now.Subtract(startT) < TimeSpan.FromMilliseconds(delay))
                            {
                                Application.DoEvents();
                                Thread.Sleep(10);
                            }
                        }
                        else
                        {
                            Thread.Sleep(delay);
                        }
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
                        readTimeout = tout;
                    }
                }
                else if (Line.ToLower().StartsWith("ioctl:"))
                {
                    int tout = 0;
                    string[] parts = Line.Split(':');
                    if (parts.Length < 2)
                    {
                        LoggerBold("Usage: IOCTL:function[:argument]");
                        return false;
                    }
                    else
                    {
                        Ioctl ioctlFunc;
                        int arg = 0;
                        if (HexToInt(parts[1], out tout))
                        {
                            Debug.WriteLine("Ioctl: ", tout);
                            ioctlFunc = (Ioctl)tout;
                        }
                        else if (!Enum.TryParse<Ioctl>(parts[1], out ioctlFunc))
                        {
                            LoggerBold("Usage: IOCTL:function[:argument]");
                            return false;
                        }
                        if (parts.Length > 2 && int.TryParse(parts[2], out int tmpArg))
                        {
                            arg = tmpArg;
                        }
                        Response<int> ioctlResp = device.Ioctl((int)ioctlFunc, arg);
                        if (ioctlResp.Status == ResponseStatus.Success)
                        {
                            Logger("Ioctl " + ioctlFunc.ToString() + " result: " + ioctlResp.Value.ToString() + " (0x" + ioctlResp.Value.ToString("X")+")");
                        }
                        else
                        {
                            Logger("Ioctl " + ioctlFunc.ToString() + " returned error");
                        }
                    }
                }
                else if (Line.ToLower().StartsWith("w:") || Line.ToLower().StartsWith("writetimeout:"))
                {
                    int tout = 0;
                    string[] parts = Line.Split(':');
                    if (int.TryParse(parts[1], out tout))
                    {
                        //Debug.WriteLine("Setting write timeout to: {0} ms ", tout);
                        device.SetWriteTimeout(tout);
                        writeTimeout = tout;
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
                    if (!HexToUint64(parts[3].Replace("+", "").Replace("-", ""), out val))
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
                    for (int x = 0; x < variables.Count; x++)
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
                    int responses = defaultResponses;
                    if (msg.Length > 3 && msg[3] == 0x3f)
                    {
                        responses = 0;
                    }
                    bool useSecondaryProtocol = SecondaryProtocol;
                    if (parts.Length > 1)
                    {
                        if (parts[1].Contains("p"))
                        {
                            useSecondaryProtocol = !useSecondaryProtocol;
                        }
                        int.TryParse(parts[1].Replace("p", ""), out responses);
                    }
                    Debug.WriteLine("Waiting {0} responses", responses);
                    if (parts.Length > 2)
                    {
                        int wtimeout;
                        if (int.TryParse(parts[2], out wtimeout))
                        {
                            device.SetWriteTimeout(wtimeout);
                        }
                    }
                    else
                    {
                        device.SetWriteTimeout(writeTimeout);
                    }
                    Debug.WriteLine("Sending data: " + BitConverter.ToString(msg));
                    OBDMessage oMsg = new OBDMessage(msg);
                    oMsg.SecondaryProtocol = useSecondaryProtocol;
                    oMsg.Txflag = txflag;
                    oMsg.Rxstatus = rxstatus;
                    //device.ClearMessageQueue();
                    Debug.WriteLine("Sending message at " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                    device.SendMessage(oMsg, responses);
                    Debug.WriteLine("Done sending message at " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                    Thread.Sleep(globaldelay);
                    Debug.WriteLine("globaldelay done at " + DateTime.Now.ToString("HH.mm.ss.ffff"));
                    DateTime starttime = DateTime.Now;
                    for (int r = 0; r < responses;)
                    {
                        OBDMessage rMsg;
                        if (!useSecondaryProtocol)
                            rMsg = device.ReceiveMessage(true);
                        else
                            rMsg = device.ReceiveMessage2();
                        if (rMsg != null && rMsg.Length > 3) // && rMsg[0] == oMsg[0] && rMsg[1] == oMsg[2] && rMsg[2] == oMsg[1])
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
                                    Debug.WriteLine("Byte at position: " + breakposition.ToString() + " == " + rMsg[breakposition].ToString("X") + " != " + breakvalue.ToString("X") + " => Continue");
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
                        if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(receiveTimeout))
                        {
                            Debug.WriteLine("Timeout: " + receiveTimeout.ToString() + " ms");
                            break;
                        }
                        if (device.LogDeviceType == DataLogger.LoggingDevType.Elm && device.ReceivedMessageCount == 0)
                        {
                            Debug.WriteLine("Elm device, no messages in queue");
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
            oMsg.Txflag = txflag;
            oMsg.Rxstatus = rxstatus;
            device.SendMessage(oMsg, 1);
            Thread.Sleep(globaldelay);
            OBDMessage rMsg = null;
            DateTime starttime = DateTime.Now;
            for (int r = 0; r < 1;)
            {
                rMsg = device.ReceiveMessage(true);
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

        private UInt64 GetSeed(string Line, string Line2)
        {
            int byteCount = 2;
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
            if (parts[0].ToLower().StartsWith("ngcengineseed"))
            {
                if (parts.Length != 2 && parts.Length !=3)
                {
                    LoggerBold(parts[0] + " must be in format: " + parts[0] + ":position[:bytes(2/4)]");
                    return 0xFFFF;
                }
                if (parts.Length == 3)
                {
                    if (!int.TryParse(parts[2], out byteCount))
                    {
                        LoggerBold("Unknown number of bytes: " + parts[2]);
                        return 0xFFFF;
                    }
                }
            }
            else if (parts.Length < 3)
            {
                if (parts[0].ToLower().StartsWith("eecv"))
                {
                    LoggerBold(parts[0] + " must be in format: " + parts[0] + ":position:seedbyte");
                }
                else
                {
                    LoggerBold(parts[0] + " must be in format: " + parts[0] + ":position:algo");
                }
                return 0xFFFF;
            }
            int position;
            if (!int.TryParse(parts[1], out position))
            {
                LoggerBold("Unknown position: " + parts[1] + " (" + Line + ")");
                return returnValue;
            }
            int algo=0;
            bool fivebytes = false;
            if (parts.Length > 2)
            {
                if (parts[2].ToLower().StartsWith("f0"))
                {
                    fivebytes = true;
                    parts[2] = parts[2].Substring(2);
                }
                if (!HexToInt(parts[2], out algo))
                {
                    LoggerBold("Unknown algo: " + parts[2] + " (" + Line + ")");
                    return returnValue;
                }
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
                rMsg = device.ReceiveMessage(true);
                if (rMsg != null && rMsg.Length >= (position + 2))
                {
                    starttime = DateTime.Now;   //Message received, reset timer
                    r++;
                }
                else if (DateTime.Now.Subtract(starttime) > TimeSpan.FromMilliseconds(100))
                {
                    Debug.WriteLine("Timeout: 100 ms");
                    return returnValue;
                }
                Application.DoEvents();
            }
            if (fivebytes)
            {
                byte[] seed = new byte[] { rMsg[position + 4], rMsg[position + 3], rMsg[position + 2], rMsg[position + 1], rMsg[position] };
                GmKeys gk = new GmKeys();
                key = gk.Get5byteKey(seed, (byte)algo);
                return key;
            }
            else if (parts[0].ToLower().StartsWith("eecv"))
            {
                Debug.WriteLine("Calculating EEC-V key...");
                EECV eecv = new EECV();
                int chCount = 0;
                if (parts.Length > 3 && HexToByte(parts[3], out byte cc))
                {
                    chCount = cc;
                }
                key = eecv.CalculateKey(chCount, (byte)algo, rMsg[position], rMsg[position + 1], rMsg[position + 2]);
                Debug.WriteLine("Seed: " + rMsg[position].ToString("X2") + rMsg[position + 1].ToString("X2") + rMsg[position + 2].ToString("X2") + ", Key: " + key.ToString("X4") + ", seedbyte: " + algo.ToString("X"));
                return key;

            }
            else if (parts[0].ToLower().StartsWith("ngcengine"))
            {
                NgcKeys nk = new NgcKeys();
                int seed;

                if (byteCount == 2)
                {
                    seed = (ushort)((rMsg[position] << 8) + rMsg[position + 1]);
                }
                else
                {
                    seed = (int)((rMsg[position] << 24) + (rMsg[position+1] << 16) + (rMsg[position+2] << 8) + rMsg[position + 3]);
                }
                key = (ulong)nk.unlockengine(seed);
                Debug.WriteLine("Seed: " + seed.ToString("X4") + ", Key: " + key.ToString("X4") );
                return key;
            }
            else
            {
                ushort seed = (ushort)((rMsg[position] << 8) + rMsg[position + 1]);
                key = KeyAlgorithm.GetKey(algo, seed);
                Debug.WriteLine("Seed: " + seed.ToString("X4") + ", Key: " + key.ToString("X4") + ", Algo: " + algo.ToString("X"));
                return key;
            }
        }


        public void UploadScript(string FileName)
        {
            try
            {
                stopscript = false;
                string Line;
                ScriptFolder = Path.GetDirectoryName(FileName);

                scriptrows = new List<string>();
                StreamReader sr = new StreamReader(FileName);
                while ((Line = sr.ReadLine()) != null)
                {
                    scriptrows.Add(Line);
                }
                sr.Close();

                //device.ClearMessageQueue();
                for (; currentrow < scriptrows.Count; currentrow++)
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
                            for (; l < lines.Count; l++)
                            {
                                string Line2 = "";
                                if (l < lines.Count - 1)
                                {
                                    Line2 = lines[l + 1];
                                }
                                if (!HandleLine(lines[l], Line2, ref breakloop))
                                {
                                    return;
                                }
                                if (lines[l].ToLower().Contains("getseed") || lines[l].ToLower().Contains("eecvseed") || lines[l].ToLower().Contains("ngcengineseed") || lines[l].ToLower().Contains("setvariable"))
                                {
                                    l++;
                                }
                                if (breakloop)
                                {
                                    break;
                                }
                                Application.DoEvents();
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
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        if (!HandleLine(Line, Line2, ref breakloop))
                        {
                            break;
                        }
                        timer.Stop();
                        Debug.WriteLine("Handleline time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
                        Application.DoEvents();
                        if (scriptrows[currentrow].ToLower().Contains("getseed") || scriptrows[currentrow].ToLower().Contains("eecvseed") || scriptrows[currentrow].ToLower().Contains("ngcengineseed") || scriptrows[currentrow].ToLower().Contains("setvariable"))
                        {
                            currentrow++;
                        }

                    }
                }
                stopscript = false;
            }
            catch (Exception ex)
            {
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
