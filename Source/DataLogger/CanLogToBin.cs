using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using static Helpers;
using System.Diagnostics;

namespace UniversalPatcher
{
    class CanLogToBin
    {
        private byte[] buf;
        private byte[] compareBuf;
        private int FileSize;
        private int row = 0;
        private byte PCMid;
        private byte toolId;

        //Multiframe:
        //private byte[] block;
        private int addr;
        private int blockLen;
        private int blockParsed;
        private bool dataRows = false;
        private int reqRow;

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
                if (dataRows == false && lineData.Length == 12 && lineData[4] == 0x07 && lineData[5] == 0x35 && (toolId == 0 || lineData[3] == toolId))
                {
                    //Tool request for data
                    reqRow = row;
                    if (toolId == 0)
                    {
                        toolId = lineData[3];
                    }
                    //blockLen = ReadInt16(lineData, 7, true);
                }
                else if (lineData.Length == 11 && row == (reqRow + 2) && (PCMid == 0 || PCMid == lineData[3]))
                {
                    //PCM answer, length, address etc...
                    if (PCMid == 0)
                    {
                        PCMid = lineData[3];
                    }
                    blockLen = ReadInt16(lineData, 4, true) - 0x1005 ;
                    blockParsed = 0;
                    //block = new byte[blockLen];
                    addr = lineData[8] << 16 | lineData[9] << 8 | lineData[10];
                    if (SizeOnly)
                    {
                        if (FileSize < (addr + blockLen))
                        {
                            FileSize = (int)(addr + blockLen);
                            //Debug.WriteLine("Addr: " + addr.ToString() + ", len: " + blockLen.ToString());
                        }
                    }
                    dataRows = true;
                }
                else if (lineData.Length == 6 && lineData[4] == 0x01 && lineData[5] == 0x75)
                {
                    //PCM Ack
                }
                else if (lineData.Length == 12 && dataRows && lineData[3] == PCMid)
                {
                    //7 bytes/msg
                    blockParsed += 7;
                    int copyLen = 7;
                    if (blockParsed > blockLen)
                    {
                        copyLen = blockParsed - blockLen + 1;
                    }
                    if (blockParsed >= blockLen)
                    {
                        dataRows = false;
                    }
                    if (!SizeOnly)
                    {
                        Array.Copy(lineData, 5, buf, addr, copyLen);
                        for (int c = addr; c < addr + copyLen; c++)
                        {
                            compareBuf[c] = 0;
                        }
                    }
                    addr += 7;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, ParseLine, line " + line + ": " + ex.Message);
                LoggerBold("File row: " + row);
            }
        }

        public void ConvertFile(string FileName)
        {
            try
            {
                string text;
                PCMid = 0;
                toolId = 0; 
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
                Logger("Calculating file size", false);
                Application.DoEvents();
                row = 0;
                foreach (string Line in lines)
                {
                    row++;
                    ParseLine(Line, true);
                    if (row % 10000 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                }
                Logger(" [OK]");
                Logger("Tool: " + toolId.ToString("X2"));
                Logger("PCM: " + PCMid.ToString("X2"));
                Application.DoEvents();
                if (FileSize == 0)
                {
                    Logger("Data not found");
                    return;
                }
                buf = new byte[FileSize];
                compareBuf = new byte[FileSize];
                for (int a = 0; a < FileSize; a++)
                {
                    buf[a] = 0xFF;
                    compareBuf[a] = 0xFF;
                }

                Logger("Parsing file ... ", false);
                Application.DoEvents();

                //Parse again but get data this time:
                row = 0;
                foreach (string Line in lines)
                {
                    row++;
                    ParseLine(Line, false);
                    if (row % 10000 == 0)
                    {
                        Logger(".", false);
                        Application.DoEvents();
                    }
                }
                Logger(" [Done]");
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

                Logger(" [OK]");
                Logger("Writing to file: " + FileName + ".bin");
                WriteBinToFile(FileName + ".bin", buf);
                Logger("Done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Convertfile, line " + line + ": " + ex.Message);
                LoggerBold("File row: " + row);
            }
        }

    }
}
