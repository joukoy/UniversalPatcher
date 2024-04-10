using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    class Ngc3Checksum
    {
        public Ngc3Checksum()
        {
            string tbFile = Path.Combine(Application.StartupPath, "XML", "ngc3tables.txt");
            if (File.Exists(tbFile))
            {
                string tablestr = ReadTextFile(tbFile);
                string[] lines = tablestr.Split('\n');
                if (lines.Length > 3 )
                {
                    Table1 = ConvertTableValues(lines[0]);
                    Table2 = ConvertTableValues(lines[1]);
                    Table3 = ConvertTableValues(lines[2]);
                    Table4 = ConvertTableValues(lines[3]);
                }
            }
            if (Table4 == null) //Not initialized yet
            {
                GenerateTables();
            }
        }
        private ushort[] Table1;
        private ushort[] Table2;
        private ushort[] Table3;
        private ushort[] Table4;
        private ushort[] ConvertTableValues(string line)
        {
            ushort[] retVal = new ushort[16];
            string[] valStrs = line.Split(',');
            if (valStrs.Length > 15)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (HexToUshort(valStrs[i], out ushort val))
                    {
                        retVal[i] = val;
                    }
                }
            }
            return retVal;
        }
        private void GenerateTables()
        {
            Table1 = new ushort[] {
                0x0000, 0xE003, 0x4003, 0xA000,
                0x8006, 0x6005, 0xC005, 0x2006,
                0x8009, 0x600A, 0xC00A, 0x2009,
                0x000F, 0xE00C, 0x400C, 0xA00F
            };
            Table2 = new ushort[] {
                0x0000, 0x8603, 0x8C03, 0x0A00,
                0x9803, 0x1E00, 0x1400, 0x9203,
                0xB003, 0x3600, 0x3C00, 0xBA03,
                0x2800, 0xAE03, 0xA403, 0x2200
            };
            Table3 = new ushort[] {
                0x0000, 0x8063, 0x80C3, 0x00A0,
                0x8183, 0x01E0, 0x0140, 0x8123,
                0x8303, 0x0360, 0x03C0, 0x83A3,
                0x0280, 0x82E3, 0x8243, 0x0220
            };
            Table4 = new ushort[] {
                0x0000, 0x8005, 0x800F, 0x000A,
                0x801B, 0x001E, 0x0014, 0x8011,
                0x8033, 0x0036, 0x003C, 0x8039,
                0x0028, 0x802D, 0x8027, 0x0022
            };

        }
        public ushort CalculateChecksum(byte[] data,uint start, uint end)
        {
            ushort ckSum = 0;
            for (uint addr = start; addr < end; addr += 2)
            {
                ushort v3 = (ushort)(ckSum ^ ReadUint16(data, addr, true));
                ckSum = (ushort)(Table1[(v3 >> 12)] ^
                    Table2[(v3 & 0xF00) >> 8] ^
                    Table3[(v3 & 0xF0) >> 4] ^
                    Table4[(v3 & 0x0F)]);
            }
            return ckSum;
        }
        public void savetables(string fName)
        {
            string tablestr = "";
            for (int i = 0; i < 16; i++)
            {
                tablestr += Table1[i].ToString("X4") + "," ;
            }
            tablestr = tablestr.Trim(',') + Environment.NewLine;
            for (int i = 0; i < 16; i++)
            {
                tablestr += Table2[i].ToString("X4") + ",";
            }
            tablestr = tablestr.Trim(',') + Environment.NewLine;
            for (int i = 0; i < 16; i++)
            {
                tablestr += Table3[i].ToString("X4") + ",";
            }
            tablestr = tablestr.Trim(',') + Environment.NewLine;
            for (int i = 0; i < 16; i++)
            {
                tablestr += Table4[i].ToString("X4") + ",";
            }
            tablestr = tablestr.Trim(',') + Environment.NewLine;
            WriteTextFile(fName, tablestr);
        }
    }
}
