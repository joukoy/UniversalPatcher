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
    public partial class frmExtractTable : Form
    {
        public frmExtractTable()
        {
            InitializeComponent();
        }

        private PcmFile PCM;
        private void btnExtract_Click(object sender, EventArgs e)
        {
            uint Start;
            uint End;
            string MaskText = "";
            string[] OSlist = txtCompatibleOS.Text.Split(',');
            string[] blocks = txtExtractRange.Text.Split(',');
            for (int b = 0; b < blocks.Length; b++)
            {
                MaskText = "";
                string block = blocks[b];
                if (block.Contains("["))
                {
                    string[] AddrMask = block.Split('[');
                    block = AddrMask[0];
                    MaskText = AddrMask[1].Replace("]", "");
                }
                if (block.Contains(":"))
                {
                    string[] StartEnd = block.Split(':');
                    if (!HexToUint(StartEnd[0], out Start))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[0]);
                        return;
                    }
                    if (!HexToUint(StartEnd[1], out End))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[1]);
                        return;
                    }
                    End += Start - 1;
                }
                else
                {
                    if (!block.Contains("-"))
                    {
                        Logger("Supply address range, for example 200-220 or 200:4");
                        return;
                    }
                    string[] StartEnd = block.Split('-');
                    if (!HexToUint(StartEnd[0], out Start))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[0]);
                        return;
                    }
                    if (!HexToUint(StartEnd[1], out End))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[1]);
                        return;
                    }
                }
                ExtractTable(Start, End, OSlist, MaskText,PCM,txtExtractDescription.Text);
            }

        }

        public void initMe(string fileName)
        {
            PCM = new PcmFile(fileName);
            labelBinFile.Text = fileName;
            GetFileInfo(labelBinFile.Text, ref PCM, true, true);
            if (txtCompatibleOS.Text.Length == 0)
                txtCompatibleOS.Text = PCM.OS;
            if (txtCompatibleOS.Text.Length == 0)
                txtCompatibleOS.Text = "ALL";
        }
    }
}
