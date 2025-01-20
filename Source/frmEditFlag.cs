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
            uint addr = td.StartAddress();
            if (td.TableDescription.Length > 1)
                labelDescription.Text = td.TableDescription;
            else
                labelDescription.Text = td.TableName;
            this.Text = td.TableName;
            string maskStr = td.BitMask;
            if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
            {
                Byte mask = Convert.ToByte(maskStr, 16);
                if ((PCM.buf[addr] & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }
            if (td.DataType == InDataType.UWORD || td.DataType == InDataType.SWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                if ((PCM.ReadUInt16(addr) & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }
            if (td.DataType == InDataType.INT32 || td.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                if ((PCM.ReadUInt32(addr) & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }
            if (td.DataType == InDataType.INT64 || td.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                if ((PCM.ReadUInt64(addr) & mask) == mask)
                    chkFlag.Checked = true;
                else
                    chkFlag.Checked = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string maskStr = td.BitMask;
            uint addr = td.StartAddress();
            if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
            {
                byte mask = Convert.ToByte(maskStr, 16);
                if (chkFlag.Checked)
                {
                    PCM.buf[addr] = (byte)(PCM.buf[addr] | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    PCM.buf[addr] = (byte)(PCM.buf[addr] & mask);
                }
            }
            else if (td.DataType == InDataType.SWORD || td.DataType == InDataType.UWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = PCM.ReadUInt16(addr);
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
                PCM.SaveUshort(addr, newVal);
            }
            else if (td.DataType == InDataType.INT32 || td.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = PCM.ReadUInt32(addr);
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
                PCM.SaveUint32(addr, newVal);
            }
            else if (td.DataType == InDataType.INT64 || td.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                UInt64 curVal = PCM.ReadUInt64(addr);
                UInt64 newVal;
                if (chkFlag.Checked)
                {
                    newVal = (UInt64)(curVal | mask);
                }
                else
                {
                    mask = ~mask;
                    newVal = (UInt64)(curVal & mask);
                }
                PCM.SaveUint64(addr, newVal);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
