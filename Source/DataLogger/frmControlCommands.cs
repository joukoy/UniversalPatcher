using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static LoggerUtils;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.Threading;

namespace UniversalPatcher
{
    public partial class frmControlCommands : Form
    {
        public frmControlCommands()
        {
            InitializeComponent();
        }

        public class PeriodicCommand
        {
            public PeriodicCommand(int id,string msg,int interval)
            {
                Id = id;
                Msg = msg;
                Interval = interval;
                LastExec = DateTime.Now;
            }
            public int Id { get; set; }
            public string Msg { get; set; }
            public int Interval { get; set; }
            public DateTime LastExec { get; set; }
        }
        public enum SetClear
        {
            Set,
            Clear
        }
        public class UiControl
        {
            public CheckBox Selected { get; set; }
            public Label valueLabel { get; set; }
            public MyCheckBox mCheckBox { get; set; }
            public ComboBox comboBox { get; set; }
            public TrackBar trackBar { get; set; }
        }
        List<UiControl> uiControls = new List<UiControl>();
        List<string> SendMessageList = new List<string>();
        List<PeriodicCommand> periodicCmds = new List<PeriodicCommand>();
        public DataLogger datalogger;
        private void frmControlCommands_Load(object sender, EventArgs e)
        {
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            Setup();
        }

        private void Setup()
        {
            uiControls.Clear();
            periodicCmds.Clear();
            tableLayoutPanel1.Controls.Clear();
            for (int i = 0; i < RealTimeControls.Count; i++)
            {
                RealTimeControl rtc = RealTimeControls[i];
                UiControl uic = new UiControl();
                CheckBox selectedBox = new CheckBox();
                selectedBox.AutoSize = true;
                selectedBox.Text = rtc.Control;
                uic.Selected = selectedBox;
                tableLayoutPanel1.Controls.Add(selectedBox, 0, i);
                Label lb1 = new Label();
                lb1.Text = "";
                lb1.TextAlign = ContentAlignment.BottomCenter;
                ComboBox cb1 = new ComboBox();
                cb1.Tag = i;
                if (!string.IsNullOrEmpty(rtc.EnumValues))
                {
                    Dictionary<string, string> possibleVals = ParseEnumValues(rtc.EnumValues);
                    cb1.DataSource = new BindingSource(possibleVals, null);
                    cb1.ValueMember = "key";
                    cb1.DisplayMember = "value";
                    uic.comboBox = cb1;
                    tableLayoutPanel1.Controls.Add(cb1, 1, i);
                }
                TrackBar tb = new TrackBar();
                uic.trackBar = tb;
                switch (rtc.Controltype)
                {
/*                    case ControlType.OnOff:
                        MyCheckBox mc = new MyCheckBox();
                        mc.AutoSize = false;
                        mc.Width = 50;
                        mc.Tag = i;
                        mc.CheckedChanged += Mc_CheckedChanged;
                        tableLayoutPanel1.Controls.Add(mc, 2, i);
                        uic.mCheckBox = mc;
                        lb1.Text = "Off";
                        break;
*/
                    case ControlType.Slider:
                        tb.Tag = i;
                        tb.Maximum = rtc.MaxValue;
                        tb.Minimum = rtc.MinValue;
                        tb.Value = 0;
                        tb.ValueChanged += Tb_ValueChanged;
                        tableLayoutPanel1.Controls.Add(tb, 2, i);
                        lb1.Text = "0";
                        tb.Tag = i;
                        break;
                    case ControlType.Set:
                        break;
                }
                tableLayoutPanel1.Controls.Add(lb1, 3, i);
                uic.valueLabel = lb1;
                uiControls.Add(uic);
            }

        }

        private Dictionary<string, string> ParseEnumValues(string eVals)
        {
            if (eVals.ToLower().StartsWith("enum:"))
                eVals = eVals.Substring(5).Trim();
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            string[] posVals = eVals.Split(',');
            for (int r = 0; r < posVals.Length; r++)
            {
                string[] parts = posVals[r].Split(':');
                if (!retVal.ContainsKey(parts[0]))
                    retVal.Add(parts[0], parts[1]);
            }
            return retVal;
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, richLogText);
        }

        private void Mc_CheckedChanged(object sender, EventArgs e)
        {
            MyCheckBox mc = (MyCheckBox)sender;
            int ind = (int)mc.Tag;
            if (RealTimeControls[ind].Controltype == ControlType.Slider)
            {
                return;
            }
            if (mc.CheckState == CheckState.Checked)
                uiControls[ind].valueLabel.Text = "On";
            else
                uiControls[ind].valueLabel.Text = "Off";
        }

        private void SendMessages(bool Show)
        {
            try
            {
                List<string> msgList = new List<string>();
                for (int i = 0; i < SendMessageList.Count; i++)
                {
                    string msg = SendMessageList[i];
                    if (msg.Length <= 10)
                    {
                        msgList.Add(msg);
                    }
                    else
                    {
                        string subMsg = SendMessageList[i].Substring(0, 10);
                        int pos = -1;
                        for (int x = 0; x < msgList.Count; x++)
                        {
                            string msg2 = msgList[x];
                            if (msg2.Length == msg.Length)
                            {
                                string subMsg2 = msgList[x].Substring(0, 10);
                                if (subMsg == subMsg2)
                                {
                                    pos = x;
                                    break;
                                }
                            }
                        }
                        if (pos < 0) //Matching submode message not found from list
                        {
                            msgList.Add(msg);
                        }
                        else
                        {
                            //Matching submode message found from list, combine messages
                            byte[] bytes1 = msg.ToBytes();
                            byte[] bytes2 = msgList[pos].ToBytes();
                            for (int b = 5; b < bytes1.Length; b++)
                            {
                                bytes2[b] = (byte)(bytes1[b] | bytes2[b]);
                            }
                            msgList[pos] = bytes2.ToHex().Replace(" ", "");
                        }
                    }
                }
                if (!datalogger.LogRunning)
                {
                    datalogger.Receiver.SetReceiverPaused(true);
                }
                for (int m = 0; m < msgList.Count; m++)
                {
                    OBDMessage oMsg = new OBDMessage(msgList[m].ToBytes());
                    if (Show)
                    {
                        Logger("Sending command: " + oMsg.GetBytes().ToHex());
                    }
                    else
                    {
                        Debug.WriteLine("Sending command: " + oMsg.GetBytes().ToHex());
                    }
                    if (datalogger.LogRunning)
                    {
                        datalogger.QueueCustomCmd(oMsg, "RTC msg");
                    }
                    else
                    {
                        if (datalogger.Connected)
                        {
                            datalogger.LogDevice.SendMessage(oMsg, 1);
                            Thread.Sleep(100);
                        }
                        else
                        {
                            Logger("Not connected");
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
                Debug.WriteLine("Error, frmControlCommands line " + line + ": " + ex.Message);
            }
            datalogger.Receiver.SetReceiverPaused(false);
            SendMessageList.Clear();
        }

        private void AddCommand(SetClear setClear, int ind, bool IsPeriodic)
        {
            try
            {
                RealTimeControl rtc = RealTimeControls[ind];
                string cmd;
                string logTxt = "Adding command: '" + rtc.Control + "' [" + setClear.ToString()+ "]";
                if (setClear == SetClear.Set)
                {
                    cmd = rtc.CommandSet;
                }
                else
                {
                    cmd = rtc.CommandClear;
                }
                if (string.IsNullOrEmpty(cmd))
                {
                    return;
                }
                if (rtc.Controltype == ControlType.Slider && cmd.ToLower().Contains("value:"))
                {
                    string format = "X2";
                    string[] bytelist = cmd.Split(' ');
                    string valBytesStr = "";
                    foreach (string b in bytelist)
                    {
                        if (b.ToLower().StartsWith("value:"))
                        {
                            string ValByteCount = b.Substring("value:".Length);
                            format = "X" + (Convert.ToInt32(ValByteCount) * 2).ToString();
                            valBytesStr = b;
                            break;
                        }
                    }
                    logTxt += " [" + uiControls[ind].trackBar.Value + "]";
                    string valStr = uiControls[ind].trackBar.Value.ToString(format);
                    cmd = cmd.Replace(valBytesStr, valStr);
                }
                if (!string.IsNullOrEmpty(rtc.EnumValues))
                {
                    string valStr = (string)uiControls[ind].comboBox.SelectedValue;
                    cmd = cmd.Replace("ENUM", valStr);
                    logTxt += " [" + uiControls[ind].comboBox.Text + "] ";
                }
                if (!IsPeriodic)
                {
                    Logger(logTxt + " (" + cmd + ")");
                }
                string msg = cmd.ToUpper().Replace(" ", "");
                SendMessageList.Add(msg);
                bool isInList = false;
/*                foreach (PeriodicCommand pc in periodicCmds)
                {
                    if (pc.Id == ind)
                    {
                        isInList = true;
                        break;
                    }
                }
*/
                if (!IsPeriodic && !isInList && datalogger.Connected && setClear == SetClear.Set)
                {
                    if (rtc.Interval > 0)
                    {
                        PeriodicCommand pc = new PeriodicCommand(ind, msg, rtc.Interval);
                        periodicCmds.Add(pc);
                        Logger("'" + rtc.Control + "' running with " + rtc.Interval.ToString() + "s interval");
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
                LoggerBold("Error, frmControlCommands line " + line + ": " + ex.Message);
            }
        }

        private void Tb_ValueChanged(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            int ind = (int)tb.Tag;
            uiControls[ind].valueLabel.Text = tb.Value.ToString();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            SendMessageList = new List<string>();
            for (int i=0;i<uiControls.Count;i++)
            {
                UiControl uic = uiControls[i];
                if (uic.Selected.Checked)
                {
                    AddCommand(SetClear.Set, i,false);
                }
            }
            SendMessages(true);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            periodicCmds.Clear();
            SendMessageList = new List<string>();
            for (int i = 0; i < uiControls.Count; i++)
            {
                UiControl uic = uiControls[i];
                if (uic.Selected.Checked)
                {
                    RealTimeControl rtc = RealTimeControls[i];
                    AddCommand(SetClear.Clear, i, false);
                }
            }
            SendMessages(true);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select XML", XmlFilter, AppSettings.ControlCommandsFile);
            if (fName.Length == 0)
            {
                return;
            }
            AppSettings.ControlCommandsFile = fName;
            AppSettings.Save();
            RealTimeControls = LoadRealTimeControls();
            Setup();
        }


        private void commandEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML frmE = new frmEditXML();
            frmE.LoadRealTimeControlCommands();            
            if (frmE.ShowDialog() == DialogResult.OK)
            {
                RealTimeControls = LoadRealTimeControls();
                Setup();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!datalogger.Connected)
            {
                return;
            }
            labelPeriodic.Text = "";
            string pText = "";
            DateTime now = DateTime.Now;
            for (int p=0;p<periodicCmds.Count;p++)
            {
                PeriodicCommand periodicCommand = periodicCmds[p];
                if (now.Subtract(periodicCommand.LastExec) >= TimeSpan.FromSeconds(periodicCommand.Interval))
                {
                    AddCommand(SetClear.Set, periodicCommand.Id,true);
                    pText += RealTimeControls[periodicCommand.Id].Control + " | ";
                    periodicCmds[p].LastExec = now;
                }
            }
            labelPeriodic.Text = pText.Trim(' ').Trim('|');
            if (SendMessageList.Count > 0)
            {
                SendMessages(false);
            }
        }
    }
}
