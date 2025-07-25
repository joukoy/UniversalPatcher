using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;
namespace UniversalPatcher
{
    public partial class frmBinToScript : Form
    {
        public frmBinToScript()
        {
            InitializeComponent();
        }
        private void frmBinToScript_Load(object sender, EventArgs e)
        {
            txtBinFile.Leave += TxtBinFile_Leave;
        }

        private void TxtBinFile_Leave(object sender, EventArgs e)
        {
            if (txtBinFile.Text.Length > 0 && txtScriptFile.Text.Length == 0)
            {
                txtScriptFile.Text = txtBinFile.Text + ".txt";
            }
        }

        private void btnBrowseBin_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select BIN File", BinFilter,txtBinFile.Text);
            if (FileName.Length == 0)
            {
                return;
            }
            txtBinFile.Text = FileName;
            if (txtBinFile.Text.Length > 0 && txtScriptFile.Text.Length == 0)
            {
                txtScriptFile.Text = txtBinFile.Text + ".txt";
            }
        }

        private void btnBrowseScript_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile(ScriptFilter, txtScriptFile.Text);
            if (FileName.Length == 0)
            {
                return;
            }
            txtScriptFile.Text = FileName;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {

                Logger("Reading file: " + txtBinFile.Text);
                string data = ConvertToScript();
                Logger("Done");
                Logger("Writing file: " + txtScriptFile.Text);
                WriteTextFile(txtScriptFile.Text, data);
                Logger("Done");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
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

        private List<Block> ParseAddressBlocks(string AddressTxt, int fSize)
        {
            List<Block> blokcs = new List<Block>();
            if (string.IsNullOrEmpty(AddressTxt))
            {
                Block b = new Block();
                b.Start = 0;
                b.End = (uint)fSize;
                blokcs.Add(b);
            }
            else
            {
                string[] parts = AddressTxt.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string[] startend = part.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (startend.Length == 2)
                    {
                        uint start;
                        uint end;
                        HexToUint(startend[0], out start);
                        HexToUint(startend[1], out end);
                        Block b = new Block();
                        b.Start = start;
                        b.End = end;
                        blokcs.Add(b);
                    }
                }
            }
            return blokcs;
        }

        public string ConvertToScript()
        {
            try
            {
                byte id;
                if (!HexToByte(txtID.Text, out id))
                {
                    LoggerBold("Can't convert from HEX: " + txtID.Text);
                    return null;
                }
                uint TargetAddr = 0;
                if (!HexToUint(txtTargetAddr.Text, out TargetAddr))
                {
                    LoggerBold("Can't convert from HEX: " + txtTargetAddr.Text);
                    return null;
                }
                byte priority;
                if (!HexToByte(txtPriority.Text, out priority))
                {
                    LoggerBold("Can't convert from HEX: " + txtPriority.Text);
                    return null;
                }
                ushort MsgLen;
                if (!HexToUshort(txtMsgLen.Text, out MsgLen))
                {
                    LoggerBold("Can't convert from HEX: " + txtMsgLen.Text);
                    return null;
                }

                StringBuilder script = new StringBuilder();

                byte[] buf = ReadBin(txtBinFile.Text);
                List<byte[]> Rows = new List<byte[]>();
                List<Block> blocks = ParseAddressBlocks(txtParseRange.Text, buf.Length);
                foreach (Block b in blocks)
                {
                    uint addr = b.Start;
                    while (addr < buf.Length && addr <= b.End)
                    {
                        List<byte> data = new List<byte>();
                        if (chkStaticAddr.Checked)
                        {
                            byte[] aBytes = BitConverter.GetBytes(TargetAddr);
                            data.Add(aBytes[2]);
                            data.Add(aBytes[1]);
                            data.Add(aBytes[0]);
                        }
                        else
                        {
                            byte[] aBytes = BitConverter.GetBytes(TargetAddr + addr);
                            data.Add(aBytes[2]);
                            data.Add(aBytes[1]);
                            data.Add(aBytes[0]);
                        }
                        ushort dataLen = 0;
                        for (int a = 0; a < MsgLen && (addr + a) < buf.Length && (addr + a) <= b.End; a++)
                        {
                            data.Add(buf[addr + a]);
                            dataLen++;
                        }
                        byte[] sizeBytes = BitConverter.GetBytes(dataLen);
                        byte up = 0x00;
                        if (chkExecute.Checked && Rows.Count == 0)  //Only first row
                        {
                            up = 0x80;
                        }
                        byte[] hdrBytes = new byte[] { up, sizeBytes[1], sizeBytes[0] };
                        data.InsertRange(0, hdrBytes);
                        ushort chk = CalcChecksum(data.ToArray());
                        byte[] chkBytes = BitConverter.GetBytes(chk);
                        data.Add(chkBytes[1]);
                        data.Add(chkBytes[0]);
                        hdrBytes = new byte[] { priority, id, 0xF0, 0x36 };
                        data.InsertRange(0, hdrBytes);
                        Rows.Add(data.ToArray());
                        addr += dataLen;
                    }
                }
                if (chkExecute.Checked)
                {
                    for (int i = Rows.Count - 1; i >= 0; i--)
                    {
                        script.Append(BitConverter.ToString(Rows[i].ToArray()).Replace("-", " "));
                        if (txtSuffix.Text.Length > 0)
                        {
                            script.Append(txtSuffix.Text);
                        }
                        if (txtAfterMsg.Text.Length > 0)
                        {
                            script.Append(Environment.NewLine + txtAfterMsg.Text);
                        }
                        script.Append(Environment.NewLine + Environment.NewLine);
                    }
                }
                else
                {
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        script.Append(BitConverter.ToString(Rows[i].ToArray()).Replace("-", " "));
                        if (txtSuffix.Text.Length > 0)
                        {
                            script.Append(txtSuffix.Text);
                        }
                        if (txtAfterMsg.Text.Length > 0)
                        {
                            script.Append(Environment.NewLine + txtAfterMsg.Text);
                        }
                        script.Append(Environment.NewLine + Environment.NewLine);

                    }
                }
                return script.ToString();
            }
            catch (Exception ex)
            {
                LoggerBold("ConvertToScript: " + ex.Message);
            }
            return null;
        }

    }
}
