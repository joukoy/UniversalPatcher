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
    class CAN23LogToBin
    {
        public CAN23LogToBin()
        {

        }

        private byte[] buf;
        private byte[] compareBuf;
        private int FileSize;
        int row;
        int reqRow;
        uint addr = 0;
        List<uint> mappedAddresses;
        List<uint> origAddresses;

        private void ParseLine(string Line, bool SizeOnly)
        {
            try
            {
                int pos = Line.LastIndexOf("]");
                if (pos > 0)
                {
                    Line = Line.Substring(pos + 1).Trim();
                }
                byte[] lineData = Line.Replace(" ", "").ToBytes();
                if ((lineData.Length == 11 && lineData[0] == 0x00 && lineData[1] == 0x00 && lineData[2] == 0x07 && lineData[4] == 0x23))
                {
                    //Request for data
                    reqRow = row;
                }
                else if (lineData[0] == 0x00 && lineData[1] == 0x00 && lineData[2] == 0x07 && lineData[4] == 0x63 && row == (reqRow + 1))
                {
                    addr = (uint)(lineData[6] << 16 | lineData[7] << 8 | lineData[8]);
                    //uint fullAddr = (uint)(lineData[5] << 24 | lineData[6] << 16 | lineData[7] << 8 | lineData[8]);
                    uint origOffset = (uint)(lineData[5] << 24);

                    int len = lineData.Length - 9;
                    if (SizeOnly)
                    {
                        if (FileSize < (addr + len))
                        {
                            FileSize = (int)(addr + len);
                        }
                        if (!origAddresses.Contains(origOffset))
                        {
                            origAddresses.Add((origOffset));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < origAddresses.Count; i++)
                        {
                            if (origAddresses[i] == origOffset)
                            {
                                addr += mappedAddresses[i];
                                break;
                            }
                        }
                        Array.Copy(lineData, 9, buf, addr, len);
                        Debug.WriteLine("Address: " + addr.ToString("X8") + ", size: " + len.ToString());
                        for (uint c=addr;c<addr+len;c++)
                        {
                            compareBuf[c] = 0;
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LogToBin line " + line + ": " + ex.Message);
                if (!ex.Message.StartsWith("Unable to convert"))
                {
                    LoggerBold(ex.Message);
                }
            }
        }

        public void ConvertLines(string[] lines, string FileName)
        {
            //Parse file and check file size:
            row = 0;
            mappedAddresses = new List<uint>();
            origAddresses = new List<uint>();
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

            if (origAddresses.Count > 0)
            {
                int origSize = FileSize;
                int offset = 0;
                for (int i = 0; i < origAddresses.Count; i++)
                {
                    mappedAddresses.Add((uint)offset);
                    FileSize = offset + origSize;
                    offset += origSize;
                    Logger("Mapping: " + origAddresses[i].ToString("X8") + " => " + mappedAddresses[i].ToString("X8"));
                }
            }

            buf = new byte[FileSize];
            compareBuf = new byte[FileSize];
            for (int a = 0; a < FileSize; a++)
            {
                buf[a] = 0xFF;
                compareBuf[a] = 0xFF;
            }

            //Parse again but get data this time:
            row = 0;
            foreach (string Line in lines)
            {
                row++;
                ParseLine(Line, false);
            }
            List<string> skipped = new List<string>();
            string range = "";
            for (int a = 0; a < FileSize; a++)
            {
                if (compareBuf[a] == 0xFF)
                {
                    if (range.Length == 0)
                    {
                        range = a.ToString("X4");
                    }
                }
                else if (range.Length > 0)
                {
                    range += " - " + a.ToString("X4");
                    skipped.Add(range);
                    range = "";
                }
            }
            if (range.Length > 0)
            {
                range += " - " + FileSize.ToString("X4");
                skipped.Add(range);
            }
            if (skipped.Count > 0)
            {
                Logger("Missed parts in log:");
                foreach (string r in skipped)
                {
                    Logger(r);
                }
            }
            Logger("Writing to file: " + FileName);
            WriteBinToFile(FileName, buf);
            Logger("Done");

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
                string binFile = SelectSaveFile(BinFilter, FileName + ".bin");
                if (!string.IsNullOrEmpty(binFile))
                {
                    ConvertLines(lines, FileName);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
    }
}
