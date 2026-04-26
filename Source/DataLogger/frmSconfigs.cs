using J2534DotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static LoggerUtils;

namespace UniversalPatcher
{
    public partial class frmSconfigs : Form
    {
        public frmSconfigs(string sconf)
        {
            InitializeComponent();
            txtSConfigs.Text = sconf.Replace("[","").Replace("]",Environment.NewLine);
        }

        public string SConfigs { get { return txtSConfigs.Text; } }
        private void frmSconfigs_Load(object sender, EventArgs e)
        {
            foreach (var cp in Enum.GetValues(typeof(ConfigParameter)))
            {
                listBox1.Items.Add(cp.ToString());
            }
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0)
            {
                return;
            }
            string cfg = listBox1.SelectedItem.ToString();
            if (j2534HexConfigs.Contains(cfg))
            {
                radioDec.Enabled = false;
                radioHex.Enabled = false;
            }
            else
            {
                radioDec.Enabled = true;
                radioHex.Enabled = true;
            }
            switch(cfg)
            {
                case "DATA_RATE":
                    labelValue.Text = "5-500000";
                    radioDec.Checked = true;
                    break;
                case "LOOPBACK":
                    labelValue.Text = "0 (OFF), 1 (ON)";
                    radioDec.Checked = true;
                    break;
                case "NODE_ADDRESS":
                    labelValue.Text = "J1850PWM: $00-$FF";
                    radioHex.Checked = true;
                    break;
                case "NETWORK_LINE":
                    labelValue.Text = "J1850PWM: 0 (BUS_NORMAL), 1 (BUS_PLUS), 2 (BUS_MINUS) Default: 0";
                    radioDec.Checked = true;
                    break;
                case "P1_MAX":
                    labelValue.Text = "ISO9141 or ISO14230: $1-$FFFF (.5 ms per bit) Default: 40 (20ms)";
                    radioHex.Checked = true;
                    break;
                case "P3_MIN":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (.5 ms per bit) Default:110 (55ms)";
                    radioHex.Checked = true;
                    break;
                case "P4_MIN":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (.5 ms per bit) Default:10 (5ms)";
                    radioHex.Checked = true;
                    break;
                case "W0":
                    labelValue.Text = "ISO9141: $0-$FFFF (1 ms per bit) Default: 300";
                    radioHex.Checked = true;
                    break;
                case "W1":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 300";
                    radioHex.Checked = true;
                    break;
                case "W2":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 20";
                    radioHex.Checked = true;
                    break;
                case "W3":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 20";
                    radioHex.Checked = true;
                    break;
                case "W4":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 50";
                    radioHex.Checked = true;
                    break;
                case "W5":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 300";
                    radioHex.Checked = true;
                    break;
                case "TIDLE":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 300";
                    radioHex.Checked = true;
                    break;
                case "TINIL":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 25";
                    radioHex.Checked = true;
                    break;
                case "TWUP":
                    labelValue.Text = "ISO9141 or ISO14230: $0-$FFFF (1 ms per bit) Default: 50";
                    radioHex.Checked = true;
                    break;
                case "PARITY":
                    labelValue.Text = "ISO9141 or ISO14230: 0 (NO_PARITY), 1 (ODD_PARITY), 2 (EVEN_PARITY) Default: 0";
                    radioDec.Checked = true;
                    break;
                case "BIT_SAMPLE_POINT":
                    labelValue.Text = "CAN: 0-100 (1% per bit) Default: 80";
                    radioDec.Checked = true;
                    break;
                case "SYNC_JUMP_WIDTH":
                    labelValue.Text = "CAN: 0-100 (1% per bit) Default: 15";
                    radioDec.Checked = true;
                    break;
                case "T1_MAX":
                    labelValue.Text = "SCI: $0-$FFFF (1 ms per bit) Default: 20";
                    radioHex.Checked = true;
                    break;
                case "T2_MAX":
                    labelValue.Text = "SCI: $0-$FFFF (1 ms per bit) Default: 100";
                    radioHex.Checked = true;
                    break;
                case "T3_MAX":
                    labelValue.Text = "SCI: $0-$FFFF (1 ms per bit) Default: 50";
                    radioHex.Checked = true;
                    break;
                case "T4_MAX":
                    labelValue.Text = "SCI: $0-$FFFF (1 ms per bit) Default: 20";
                    radioHex.Checked = true;
                    break;
                case "T5_MAX":
                    labelValue.Text = "SCI: $0-$FFFF (1 ms per bit) Default: 100";
                    radioHex.Checked = true;
                    break;
                case "ISO15765_BS":
                    labelValue.Text = "ISO15765: $0-$FF Default: 0";
                    radioHex.Checked = true;
                    break;
                case "ISO15765_STMIN":
                    labelValue.Text = "ISO15765: $0-$FF Default: 0";
                    radioHex.Checked = true;
                    break;
                case "ISO15765_BS_TX":
                    labelValue.Text = "ISO15765: $0-$FF,FFFF Default: $FFFF";
                    radioHex.Checked = true;
                    break;
                case "ISO15765_STMIN_TX":
                    labelValue.Text = "ISO15765: $0-$FF,FFFF Default: $FFFF";
                    radioHex.Checked = true;
                    break;
                case "DATA_BITS":
                    labelValue.Text = "ISO9141 or ISO14230: 0 (8 data bits), 1 (7 data bits) Default: 0";
                    radioDec.Checked = true;
                    break;
                case "FIVE_BAUD_MOD":
                    labelValue.Text = "ISO9141 or ISO14230: 0 (ISO 9141-2/14230-4), 1 (Inv KB2), 2 (Inv Addr), 3 (ISO 9141) Default: 0";
                    radioDec.Checked = true;
                    break;
                case "ISO15765_WFT_MAX":
                    labelValue.Text = "ISO15765: $0-$FF Default: 0";
                    radioHex.Checked = true;
                    break;
                case "CAN_MIXED_FORMAT":
                    labelValue.Text = "0: CAN_MIXED_FORMAT_OFF: Messages will be treated as ISO 15765 ONLY" + Environment.NewLine +
                        "1: CAN_MIXED_FORMAT_ON: Messages will be treated as either ISO 15765 or an unformatted CAN frame" + Environment.NewLine +
                        "2: CAN_MIXED_FORMAT_ALL_FRAMES: Messages will be treated as ISO 15765, an unformatted CAN frame, or both";
                    radioDec.Checked = true;
                    break;
                case "J1962_PINS":
                    labelValue.Text = "XXYY, where XX=main pin, YY=secondary pin" + Environment.NewLine + "" +
                        "XX!=YY, except $0000." + Environment.NewLine + "Exclude pins 4, 5, and 16.";
                    break;
                case "J1939_PINS":
                    labelValue.Text = "XXYY, where XX=main pin, YY=secondary pin" + Environment.NewLine + "" +
                        "XX!=YY, except $0000." + Environment.NewLine + "Exclude pins 4, 5, and 16.";
                    break;
                case "J1708_PINS":
                    labelValue.Text = "XXYY, where XX=main pin, YY=secondary pin" + Environment.NewLine + "" +
                        "XX!=YY, except $0000." + Environment.NewLine + "Exclude pins 4, 5, and 16.";
                    break;
                case "SW_CAN_HS_DATA_RATE":
                    labelValue.Text = "SWCAN: 5-500000 Default: 83333";
                    radioDec.Checked = true;
                    break;
                case "SW_CAN_SPEEDCHANGE_ENABLE":
                    labelValue.Text = "SWCAN: 0 (DISABLE_SPDCHANGE), 1 (ENABLE_SPDCHANGE) Default: 0";
                    radioDec.Checked = true;
                    break;
                case "SW_CAN_RES_SWITCH":
                    labelValue.Text = "SWCAN: 0 (DISCONNECT_RESISTOR), 1 (CONNECT_RESISTOR), 2 (AUTO_ RESISTOR) Default: 0";
                    radioDec.Checked = true;
                    break;
                case "ACTIVE_CHANNELS":
                    labelValue.Text = "ANALOG: 0-$FFFFFFFF";
                    radioHex.Checked = true;
                    break;
                case "SAMPLE_RATE":
                    labelValue.Text = "ANALOG: 0-$FFFFFFFF Default: 0" + Environment.NewLine +
                        "(high bit changes meaning from samples/sec to seconds/sample)";
                    radioHex.Checked = true;
                    break;
                case "SAMPLES_PER_READING":
                    labelValue.Text = "ANALOG: 1-$FFFFFFFF Default: 1";
                    radioHex.Checked = true;
                    break;
                case "READINGS_PER_MSG":
                    labelValue.Text = "ANALOG: 1-$00000408 (1 - 1032) Default: 1";
                    break;
                case "AVERAGING_METHOD":
                    labelValue.Text = "ANALOG: 0-$FFFFFFFF Default: 0" + Environment.NewLine +
                        "0: SIMPLE_AVERAGE: Simple arithmetic mean" + Environment.NewLine +
                        "1: MAX_LIMIT_AVERAGE: Choose the biggest value" + Environment.NewLine +
                        "2: MIN_LIMIT_AVERAGE: Choose the lowest value" + Environment.NewLine +
                        "3: MEDIAN_AVERAGE: Choose arithmetic median" + Environment.NewLine;
                    radioDec.Checked = true;
                    break;
                case "SAMPLE_RESOLUTION":
                    labelValue.Text = "ANALOG READ-ONLY: $1-$20 (1 - 32)";
                    break;
                case "INPUT_RANGE_LOW":
                    labelValue.Text = "ANALOG READ-ONLY: $80000000-$7FFFFFFF (-2147483648-2147483647)";
                    break;
                case "INPUT_RANGE_HIGH":
                    labelValue.Text = "ANALOG READ-ONLY: $80000000-$7FFFFFFF (-2147483648-2147483647)";
                    break;
                default:
                    labelValue.Text = "";
                    break;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count ==0)
            {
                return;
            }
            string cfg = listBox1.SelectedItem.ToString();
            if (!txtSConfigs.Text.Contains(cfg))
            {
                if (chkAppendRow.Checked)
                {
                    if (string.IsNullOrEmpty(txtSConfigs.Text))
                    {
                        txtSConfigs.Text = cfg + "=" + txtValue.Text;
                    }
                    else
                    {
                        int pos = txtSConfigs.Text.IndexOf(Environment.NewLine, txtSConfigs.SelectionStart);
                        if (pos < 0)
                        {
                            pos = txtSConfigs.Text.Length;
                        }
                        if (pos >= 0)
                        {
                            txtSConfigs.SelectionStart = pos;
                        }
                        if (pos > 0 && txtSConfigs.Text[pos-1] != '\n')
                        {
                            txtSConfigs.Paste(",");
                        }
                        txtSConfigs.Paste(cfg + "=" + txtValue.Text);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtSConfigs.Text))
                    {
                        txtSConfigs.Text = txtSConfigs.Text.Trim() + Environment.NewLine;
                    }
                    txtSConfigs.Text += cfg + "=" + txtValue.Text;
                }
            }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            radioHex.Checked = txtValue.Text.Contains("$");
            radioDec.Checked = !radioHex.Checked;
        }

        private void radioHex_CheckedChanged(object sender, EventArgs e)
        {
            if (radioHex.Checked && !txtValue.Text.StartsWith("$"))
            {
                txtValue.Text = "$" + txtValue.Text;
            }
            if (radioDec.Checked)
            {
                txtValue.Text = txtValue.Text.Replace("$", "");
            }
        }
    }
}
