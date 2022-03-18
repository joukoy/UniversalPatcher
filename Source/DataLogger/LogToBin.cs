using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;

namespace UniversalPatcher
{
    class LogToBin
    {
        public LogToBin()
        {

        }

        private byte[] buf;
        private int FileSize;
        private bool corrupted = false;
        byte[] waitingBytes =  new byte[3];
        int row;

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

        private void ParseLine(string Line, bool SizeOnly)
        {
            try
            {
                int pos = Line.IndexOf("]");
                if (pos > 0)
                {
                    Line = Line.Substring(pos + 1).Trim();
                }
                byte[] tmp = Line.Replace(" ", "").ToBytes();
                if (tmp[1] == 0xf0 && tmp[2] == 0x10 && tmp[3] == 0x36)
                {
                    int len = ReadInt16(tmp, 5, true);
                    if ((len + 12) != tmp.Length && SizeOnly)
                    {
                        LoggerBold("Message size not match! Row: " + row.ToString());
                        corrupted = true;
                    }
                    uint addr = (uint)(tmp[7] << 16 | tmp[8] << 8 | tmp[9]);
                    byte[] payload = new byte[len];
                    Array.Copy(tmp, 10, payload, 0, len);

                    byte[] fullMsg = new byte[tmp.Length - 6];
                    Array.Copy(tmp, 4, fullMsg, 0, tmp.Length - 6);
                    ushort calc = CalcChecksum(fullMsg);
                    ushort msgChk = (ushort)(tmp[tmp.Length - 2] << 8 | tmp.Last());
                    if (calc != msgChk && SizeOnly)
                    {
                        LoggerBold("Checksum error! Row: " + row.ToString()+ " (" + calc.ToString("X4") + " <> " + msgChk.ToString("X4")+")") ;
                        corrupted = true;
                    }
                    if (SizeOnly)
                    {
                        if (FileSize < (addr + len))
                        {
                            FileSize = (int)(addr + len);
                        }
                    }
                    else
                    {
                        Array.Copy(payload, 0, buf, addr, len);
                    }
                }
                else if (tmp[2] == 0xF0 && tmp[3] == 0x23)
                {
                    waitingBytes[0] = tmp[4];
                    waitingBytes[1] = tmp[5];
                    waitingBytes[2] = tmp[6];
                }
                else if (tmp.Length > 9 && tmp[1] == 0xF0 && tmp[3] == 0x63 && tmp[4] == waitingBytes[1] && tmp[5] == waitingBytes[2])
                {
                    int addr = waitingBytes[0] << 16 | waitingBytes[1] << 8 | waitingBytes[2];
                    if (SizeOnly)
                    {
                        if (addr > FileSize)
                        {
                            FileSize = addr + 4;
                        }
                    }
                    else
                    {
                        buf[addr] = tmp[6];
                        buf[addr + 1] = tmp[7];
                        buf[addr + 2] = tmp[8];
                        buf[addr + 3] = tmp[9];
                    }
                }
            }
            catch(Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        public void ConvertFile(string FileName)
        {
            try
            {
                string text;
                if (FileName.ToLower().EndsWith(".rtf"))
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.LoadFile(FileName);
                    text = rtb.Text;
                }
                else
                {
                    text = ReadTextFile(FileName);
                }
                string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                //Parse file and check file size:
                row = 0;
                corrupted = false;
                foreach (string Line in lines)
                {
                    row++;
                    ParseLine(Line, true);
                }

                if (FileSize == 0)
                {
                    Logger("Data not found");
                    return;
                }
                buf = new byte[FileSize];
                for (int a = 0; a < FileSize; a++)
                {
                    buf[a] = 0xFF;
                }

                //Parse again but get data this time:
                foreach (string Line in lines)
                {
                    ParseLine(Line, false);
                }
                if (corrupted)
                {
                    DialogResult res = MessageBox.Show("File may be corrupted, save anyway?", "Warning", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No)
                    {
                        Logger("Cancel saving");
                        return;
                    }
                }
                Logger("Writing to file: " + FileName + ".bin");
                WriteBinToFile(FileName + ".bin", buf);
                Logger("Done");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
    }
}
