using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            labelOf.Text = "Of " + Blocks.Length.ToString();
            ParseAddress(Blocks[0]);
            CurrentBlock = 0;
        }
        public void ParseAddress(string OldAddr)
        {

            if (OldAddr.Length == 0)
                return;
            if (OldAddr.StartsWith("@"))
                radioReadStart.Checked = true;
            else
                radioStartAbsolute.Checked = true;
            string[] Parts = OldAddr.Split('-');
            if (Parts[0].Contains(":"))
            { 
                string[] Parts2 = Parts[0].Split(':');
                Parts[0] = Parts2[0];
                ushort x;
                UInt16.TryParse(Parts2[1], out x);
                numReadPairs.Value = x;
            }
            txtStart.Text = Parts[0].Replace("@", "");

            if (Parts.Length > 1)
            {
                if (Parts[1].StartsWith("@"))
                    radioReadEnd.Checked = true;
                else
                    radioEndAbsolute.Checked = true;
                if (Parts[1].EndsWith("@"))
                    chkEnd.Checked = true;
                else
                    chkEnd.Checked = false;

                txtEnd.Text = Parts[1].Replace("@", "");
            }
        }

        private void UpdateBlock(uint Block)
        {
            string BlockText = "";
            if (txtEnd.Text.Length == 0)
                return;
            if (radioStartAbsolute.Checked)
            {
                if (txtEnd.Text.Length == 0)
                    return;
                BlockText += txtStart.Text ;
            }
            if (radioReadStart.Checked)
            {
                BlockText += "@" + txtStart.Text;
            }
            if (numReadPairs.Value > 1)
            {
                BlockText += numReadPairs.Value.ToString();
            }

            if (radioEndAbsolute.Checked)
                BlockText += "-" + txtEnd.Text;

            if (radioReadEnd.Checked)
                BlockText += "-@" + txtEnd.Text;

            if (chkEnd.Checked)
                BlockText += "@";

            if (txtOffset.Text.Length > 0)
            {
                if (txtOffset.Text.StartsWith("-"))
                    BlockText += "<" + txtOffset.Text;
                else
                    BlockText += ">" + txtOffset.Text;
            }
            Blocks[Block] = BlockText;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            UpdateBlock(CurrentBlock);
            Result = "";
            for (int i=0;i < Blocks.Length; i++ )
            {
                if (i > 0)
                    Result += ",";
                Result += Blocks[i];
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
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
                ParseAddress(Blocks[Block]);
                labelOf.Text = "of " + (Block + 1).ToString();
            }
            CurrentBlock = Block;
        }
    }
}
