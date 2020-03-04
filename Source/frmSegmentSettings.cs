using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmSegmentSettings : Form
    {
        public frmSegmentSettings()
        {
            InitializeComponent();
        }

        private int CurrentSegment;

        private void btnApply_Click(object sender, EventArgs e)
        {
            SegmentConfig S = new SegmentConfig();

            S.Name = txtSegmentName.Text;
            S.Addresses = txtSegmentAddress.Text;

            S.CS1Address = txtCS1Address.Text;
            S.CS2Address = txtCS2Address.Text;
            S.CS1Blocks = txtCS1Block.Text;
            S.CS2Blocks = txtCS2Block.Text;
           
            S.PNAddr = txtPNAddr.Text;
            S.VerAddr = txtVerAddr.Text;
            S.SegNrAddr = txtNrAddr.Text;
            S.ExtraInfo = txtExtrainfo.Text;
            S.Eeprom = chkEeprom.Checked;
            S.Comment = txtComment.Text;

            if (radioCS1None.Checked)
                S.CS1Method = CSMethod_None;
            if (radioCS1Crc16.Checked)
                S.CS1Method = CSMethod_crc16;
            if (radioCS1Crc32.Checked)
                S.CS1Method = CSMethod_crc32;
            if (radioCS1SUM.Checked)
                S.CS1Method = CSMethod_Bytesum;
            if (radioCS1WordSum.Checked)
                S.CS1Method = CSMethod_Wordsum;
            if (radioCS1DwordSum.Checked)
                S.CS1Method = CSMethod_Dwordsum;

            if (radioCS2None.Checked)
                S.CS2Method = CSMethod_None;
            if (radioCS2Crc16.Checked)
                S.CS2Method = CSMethod_crc16;
            if (radioCS2Crc32.Checked)
                S.CS2Method = CSMethod_crc32;
            if (radioCS2SUM.Checked)
                S.CS2Method = CSMethod_Bytesum;
            if (radioCS2WordSum.Checked)
                S.CS2Method = CSMethod_Wordsum;
            if (radioCS2DwordSum.Checked)
                S.CS2Method = CSMethod_Dwordsum;
            S.CS1SwapBytes = checkSwapBytes1.Checked;

            if (radioCS1Complement0.Checked)
                S.CS1Complement = 0;
            if (radioCS1Complement1.Checked)
                S.CS1Complement = 1;
            if (radioCS1Complement2.Checked)
                S.CS1Complement = 2;

            if (radioCS2Complement0.Checked)
                S.CS2Complement = 0;
            if (radioCS2Complement1.Checked)
                S.CS2Complement = 1;
            if (radioCS2Complement2.Checked)
                S.CS2Complement = 2;
            S.CS2SwapBytes = checkSwapBytes2.Checked;
            Segments[CurrentSegment] = S;
        }

        public void EditSegment(int SegNr)
        {
            labelXML.Text = Path.GetFileName(XMLFile);
            CurrentSegment = SegNr;
            SegmentConfig S = Segments[SegNr];
            txtSegmentName.Text = S.Name;
            txtSegmentAddress.Text = S.Addresses;
            txtCS1Address.Text = S.CS1Address;
            txtCS2Address.Text = S.CS2Address;
            txtCS1Block.Text = S.CS1Blocks;
            txtCS2Block.Text = S.CS2Blocks;
            txtPNAddr.Text = S.PNAddr;
            txtVerAddr.Text = S.VerAddr;
            txtNrAddr.Text = S.SegNrAddr;
            txtExtrainfo.Text = S.ExtraInfo;
            chkEeprom.Checked = S.Eeprom;
            txtComment.Text = S.Comment;

            checkSwapBytes1.Checked = S.CS1SwapBytes;
            checkSwapBytes2.Checked = S.CS2SwapBytes;
            if (S.CS1Method == CSMethod_None)
                radioCS1None.Checked = true;
            if (S.CS1Method == CSMethod_crc16)
                radioCS1Crc16.Checked = true;
            if (S.CS1Method == CSMethod_crc32)
                radioCS1Crc32.Checked = true;
            if (S.CS1Method == CSMethod_Bytesum)
                radioCS1SUM.Checked = true;
            if (S.CS1Method == CSMethod_Wordsum)
                radioCS1WordSum.Checked = true;
            if (S.CS1Method == CSMethod_Dwordsum)
                radioCS1DwordSum.Checked = true;
            if (S.CS2Method == CSMethod_None)
                radioCS2None.Checked = true;
            if (S.CS2Method == CSMethod_crc16)
                radioCS2Crc16.Checked = true;
            if (S.CS2Method == CSMethod_crc32)
                radioCS2Crc32.Checked = true;
            if (S.CS2Method == CSMethod_Bytesum)
                radioCS2SUM.Checked = true;
            if (S.CS2Method == CSMethod_Wordsum)
                radioCS2WordSum.Checked = true;
            if (S.CS2Method == CSMethod_Dwordsum)
                radioCS2DwordSum.Checked = true;
            if (S.CS1Complement == 0)
                radioCS1Complement0.Checked = true;
            if (S.CS1Complement == 1)
                radioCS1Complement1.Checked = true;
            if (S.CS1Complement == 2)
                radioCS1Complement2.Checked = true;
            if (S.CS2Complement == 0)
                radioCS2Complement0.Checked = true;
            if (S.CS2Complement == 1)
                radioCS2Complement1.Checked = true;
            if (S.CS2Complement == 2)
                radioCS2Complement2.Checked = true;

        }


        private void btnHelp_Click(object sender, EventArgs e)
        {
            string HelpFile = Path.Combine(Application.StartupPath, "help.txt");
            if (File.Exists(HelpFile))
                System.Diagnostics.Process.Start(@HelpFile);
            else
                MessageBox.Show("Missing helpfile", "File missing");

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnApply_Click(sender, e);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string EditAddress(string OldAddress, bool extra=false)
        {
            frmEditAddress frmE = new frmEditAddress();
            frmE.ParseAddress(OldAddress,extra);
            string Res = OldAddress;
            if (frmE.ShowDialog(this) == DialogResult.OK)
            {
                Res = frmE.Result;
            }
            return Res;
        }

        private string EditSegmentAddress(string OldAddress)
        {
            frmEditSegmentAddr frmE = new frmEditSegmentAddr();
            frmE.StartMe(OldAddress);
            string Res = OldAddress;
            if (frmE.ShowDialog(this) == DialogResult.OK)
            {
                Res = frmE.Result;
            }
            return Res;

        }
        private void txtCS1Address_Doubleclick(object sender,EventArgs e)
        {
            txtCS1Address.Text = EditAddress(txtCS1Address.Text);
        }
        private void btnCS1Addr_Click(object sender, EventArgs e)
        {
            txtCS1Address.Text = EditAddress(txtCS1Address.Text);
        }
        private void txtPNAddr_Doubleclick(object sender, EventArgs e)
        {
            txtPNAddr.Text = EditAddress(txtPNAddr.Text);
        }
        private void btnPNAddr_Click(object sender, EventArgs e)
        {
            txtPNAddr.Text = EditAddress(txtPNAddr.Text);
        }

        private void txtNrAddr_Doubleclick(object sender, EventArgs e)
        {
            txtNrAddr.Text = EditAddress(txtNrAddr.Text);
        }
        private void btnSegNrAddr_Click(object sender, EventArgs e)
        {
            txtNrAddr.Text = EditAddress(txtNrAddr.Text);
        }

        private void txtCS2Address_Doubleclick(object sender, EventArgs e)
        {
            txtCS2Address.Text = EditAddress(txtCS2Address.Text);
        }
        private void btnCS2Addr_Click(object sender, EventArgs e)
        {
            txtCS2Address.Text = EditAddress(txtCS2Address.Text);
        }

        private void txtVerAddr_Doubleclick(object sender, EventArgs e)
        {
            txtVerAddr.Text = EditAddress(txtVerAddr.Text);
        }
        private void btnVerAddr_Click(object sender, EventArgs e)
        {
            txtVerAddr.Text = EditAddress(txtVerAddr.Text);
        }

        private void txtExtrainfo_Doubleclick(object sender, EventArgs e)
        {
            if (txtExtrainfo.Text.Length == 0)
                txtExtrainfo.Text = EditAddress(txtExtrainfo.Text, true);
            else
                txtExtrainfo.Text += "," + EditAddress(txtExtrainfo.Text, true);
        }
        private void btnExtraAddr_Click(object sender, EventArgs e)
        {
            if (txtExtrainfo.Text.Length == 0)
                txtExtrainfo.Text = EditAddress(txtExtrainfo.Text, true);
            else
                txtExtrainfo.Text += "," + EditAddress(txtExtrainfo.Text, true);
        }


        private void txtSegmentAddress_DoubleClick(object sender, EventArgs e)
        {
            txtSegmentAddress.Text = EditSegmentAddress(txtSegmentAddress.Text);
        }
        private void btnEditSegmentAddr_Click(object sender, EventArgs e)
        {
            txtSegmentAddress.Text = EditSegmentAddress(txtSegmentAddress.Text);
        }

        private void txtCS1Block_DoubleClick(object sender, EventArgs e)
        {
            txtCS1Block.Text = EditSegmentAddress(txtCS1Block.Text);
        }
        private void btnCs1Block_Click(object sender, EventArgs e)
        {
            txtCS1Block.Text = EditSegmentAddress(txtCS1Block.Text);
        }

        private void txtCS2Block_DoubleClick(object sender, EventArgs e)
        {
            txtCS2Block.Text = EditSegmentAddress(txtCS2Block.Text);
        }
        private void btnCs2block_Click(object sender, EventArgs e)
        {
            txtCS2Block.Text = EditSegmentAddress(txtCS2Block.Text);
        }

    }
}
