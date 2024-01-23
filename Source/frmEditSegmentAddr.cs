using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmEditSegmentAddr : Form
    {
        public frmEditSegmentAddr()
        {
            InitializeComponent();
        }

        public string Result;
        private string[] Blocks;
        private uint CurrentBlock;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void StartMe(string OldAddr)
        {
            Blocks = OldAddr.Split(',');
            labelOf.Text = "of " + Blocks.Length.ToString();
            ParseAddress(Blocks[0]);
            CurrentBlock = 0;
        }
        public void ParseAddress(string OldAddr)
        {

            if (OldAddr.Length == 0)
                return;
            if (OldAddr.StartsWith("seek:"))
                radioStartSeek.Checked = true;
            else if (OldAddr.StartsWith("@"))
                radioStartRead.Checked = true;
            else
                radioStartAbsolute.Checked = true;
            string[] Parts = OldAddr.Split('-');
            if (Parts[0].Contains("*"))
            {
                string[] Parts2 = Parts[0].Split('*');
                Parts[0] = Parts2[0];
                ushort x;
                if (UInt16.TryParse(Parts2[1], out x))
                    numReadPairs.Value = x;
            }
            if (Parts[0].Contains(":"))
            { 
                string[] Parts2 = Parts[0].Split(':');
                if (Parts2[0] == "seek")
                {
                    Parts[0] = Parts2[1];
                    if (Parts2.Length > 2)
                    {
                        ushort x;
                        if (UInt16.TryParse(Parts2[2], out x))
                            numBytes.Value = x;
                    }
                }
                else
                {
                    Parts[0] = Parts2[0];
                    ushort x;
                    if (UInt16.TryParse(Parts2[1], out x))
                        numBytes.Value = x;
                }
            }
            txtStart.Text = Parts[0].Replace("@", "");

            if (Parts.Length > 1)
            {
                if (Parts[1].Contains("<"))
                {
                    string[] Parts2 = Parts[1].Split('<');
                    Parts[1] = Parts2[0];
                    txtOffset.Text = "-" + Parts2[1];
                }
                if (Parts[1].Contains(">"))
                {
                    string[] Parts2 = Parts[1].Split('>');
                    Parts[1] = Parts2[0];
                    txtOffset.Text = Parts2[1];
                }
                if (Parts[1].StartsWith("seek"))
                    radioEndSeek.Checked = true;
                else if (Parts[1].StartsWith("@"))
                    radioReadEnd.Checked = true;
                else if (Parts[1].StartsWith("L@"))
                    radioReadSize.Checked = true;
                else if (Parts[1].StartsWith("L"))
                    radioSize.Checked = true;
                else
                    radioEndAbsolute.Checked = true;
                if (Parts[1].EndsWith("@"))
                    chkEnd.Checked = true;
                else
                    chkEnd.Checked = false;

                txtEnd.Text = Parts[1].Replace("@", "").Replace("L","").Replace("seek:","");
            }
        }

        private bool UpdateBlock(uint Block)
        {
            string BlockText = "";
            if (txtStart.Text.Length == 0)
                return false;
            if (radioStartSeek.Checked)
            {
                BlockText = "seek:";
            }
            else if (radioStartAbsolute.Checked)
            {
                if (txtEnd.Text.Length == 0)
                    return false;
            }
            else
            {
                BlockText += "@";
            }
            BlockText += txtStart.Text;
            BlockText += ":" + numBytes.Value.ToString();
            if (numReadPairs.Value > 1)
            {
                BlockText += "*" + numReadPairs.Value.ToString();
            }

            if (radioEndAbsolute.Checked)
                BlockText += "-" + txtEnd.Text;
            else if (radioEndSeek.Checked)
                BlockText += "-seek:" + txtEnd.Text;
            else if (radioReadEnd.Checked)
                BlockText += "-@" + txtEnd.Text;

            else if (chkEnd.Checked)
                BlockText += "@";

            else if (radioSize.Checked)
                BlockText += "-L" + txtEnd.Text;

            else if (radioReadSize.Checked)
                BlockText += "-L@" + txtEnd.Text;

            if (txtOffset.Text.Length > 0)
            {
                if (txtOffset.Text.StartsWith("-"))
                    BlockText += "<" + txtOffset.Text.Replace("-","");
                else
                    BlockText += ">" + txtOffset.Text;
            }
            Blocks[Block] = BlockText;
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!UpdateBlock(CurrentBlock))
                return;
            Result = "";
            for (int i=0;i < Blocks.Length; i++ )
            {
                if (i > 0)
                    Result += ",";
                Result += Blocks[i];
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void numBlock_ValueChanged(object sender, EventArgs e)
        {
            UpdateBlock(CurrentBlock);
            uint Block = (uint)numBlock.Value - 1;
            if (Block < Blocks.Length)
                ParseAddress(Blocks[Block]);
            else
            {
                string[] tmp = new string[Block + 1];
                Array.Copy(Blocks, 0, tmp, 0, Blocks.Length);
                Blocks = new string[tmp.Length];
                Array.Copy(tmp, 0, Blocks, 0, tmp.Length);
                Blocks[Block] = "0";
                labelOf.Text = "of " + (Block + 1).ToString();
                txtStart.Text = "";
                txtEnd.Text = "";
                txtOffset.Text = "";
                radioStartRead.Checked = true;
                radioUseStart.Checked = true;
                numBytes.Value = 4;
                numReadPairs.Value = 1;
                chkEnd.Checked = false;
            }
            CurrentBlock = Block;
        }

        private void radioStartAbsolute_CheckedChanged(object sender, EventArgs e)
        {
            if (radioStartAbsolute.Checked)
                numReadPairs.Enabled = false;
            else
                numReadPairs.Enabled = true;
        }

        private void ShowEndAddress()
        {
            if (radioUseStart.Checked)
            {
                txtEnd.Enabled = false;
            }
            else
            {
                txtEnd.Enabled = true;
            }
            if (radioEndAbsolute.Checked)
            {
                chkEnd.Enabled = true;
            }
            else
            {
                chkEnd.Enabled = false;
            }
        }

        private void radioUseStart_CheckedChanged(object sender, EventArgs e)
        {
            ShowEndAddress();
        }

        private void radioEndAbsolute_CheckedChanged(object sender, EventArgs e)
        {
            ShowEndAddress();

        }

        private void radioReadEnd_CheckedChanged(object sender, EventArgs e)
        {
            ShowEndAddress();
        }

        private void radioSize_CheckedChanged(object sender, EventArgs e)
        {
            ShowEndAddress();
        }
        private void EditSegmentSeek(TextBox tBox)
        {
            frmEditXML frmE = new frmEditXML();
            frmE.LoadSegmentSeek(frmpatcher.basefile.SegmentSeekFile);
            if (frmE.ShowDialog() == DialogResult.OK)
            {
                SegmentSeek ss = (SegmentSeek)frmE.SelectedObject;
                if (ss != null)
                {
                    tBox.Text = ss.Name;
                }
            }
        }
        private void btnStartSeek_Click(object sender, EventArgs e)
        {
            EditSegmentSeek(txtStart);
        }
        private void btnEndSeek_Click(object sender, EventArgs e)
        {
            EditSegmentSeek(txtEnd);
        }
    }
}
