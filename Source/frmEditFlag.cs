using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmEditFlag : Form
    {
        public frmEditFlag()
        {
            InitializeComponent();
        }
        private PcmFile PCM;
        TableData td;
        private void frmEditFlag_Load(object sender, EventArgs e)
        {

        }
        public void loadFlag(PcmFile PCM1, TableData td1)
        {
            PCM = PCM1;
            td = td1;
            if (td.TableDescription.Length > 1)
                labelDescription.Text = td.TableDescription;
            else
                labelDescription.Text = td.TableName;
            this.Text = td.TableName;
            string maskStr = td.BitMask;
            if (td.ElementSize == 1)
            {
                Byte mask = Convert.ToByte(maskStr, 16);
                if ((PCM.buf[td.AddrInt] & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }
            else if (td.ElementSize == 2)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                if ((BEToInt16(PCM.buf,td.AddrInt) & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }
            else if (td.ElementSize == 4)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                if ((BEToInt32(PCM.buf, td.AddrInt) & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string maskStr = td.BitMask;
            if (td.ElementSize == 1)
            {
                byte mask = Convert.ToByte(maskStr, 16);
                if (chkFlag.Checked)
                {                    
                    PCM.buf[td.AddrInt] = (byte)(PCM.buf[td.AddrInt] | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    PCM.buf[td.AddrInt] = (byte)(PCM.buf[td.AddrInt] & mask);
                }
            }
            else if (td.ElementSize == 2)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = BEToUint16(PCM.buf, td.AddrInt);
                ushort newVal;
                if (chkFlag.Checked)
                {
                    newVal = (ushort)(curVal | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    newVal = (ushort)(curVal & mask);
                }
                Byte[] b = BitConverter.GetBytes(newVal);
                PCM.buf[td.AddrInt] = b[1];
                PCM.buf[td.AddrInt + 1] = b[0];
            }
            else if (td.ElementSize == 4)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = BEToUint32(PCM.buf, td.AddrInt);
                UInt32 newVal;
                if (chkFlag.Checked)
                {
                    newVal = (UInt32)(curVal | mask);
                }
                else
                {
                    mask = ~mask;
                    newVal = (UInt32)(curVal & mask);
                }
                Byte[] b = BitConverter.GetBytes(newVal);
                PCM.buf[td.AddrInt] = b[3];
                PCM.buf[td.AddrInt + 1] = b[2];
                PCM.buf[td.AddrInt + 2] = b[1];
                PCM.buf[td.AddrInt + 3] = b[0];
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
