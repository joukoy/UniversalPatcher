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
    public class LogToBinConverter
    {
        public LogToBinConverter(RMode mode)
        {
            readMode = mode;
            corrupted = false;
            if (mode == RMode.CAN23)
            {
                reqByte = 0x23;
                ansByte = 0x63;
                ansRows = 1;
                dataOffset = 9;
                addrOffset = 5;
            }
            else if (mode == RMode.CAN36)
            {
                reqByte = 0x35;
                ansByte = 0x36;
                ansRows = 2;
                dataOffset = 10;
                addrOffset = 6;
            }
            else if (mode == RMode.CAN78)
            {
                reqByte = 0x78;
                addrOffset = 7;
                sizeOffset = 4;
                dataOffset = 5;
            }
        }

        public enum RMode
        {
            VPW = 0,
            CAN = 1,
            CAN23 = 23,
            CAN36 = 36,
            CAN78 = 78
        }
        private RMode readMode;
        private byte[] buf;
        private byte[] compareBuf;
        private int FileSize;
        int row;
        int reqRow;
        uint addr = 0;
        uint size = 0;
        uint received = 0;
        List<uint> mappedAddresses;
        List<uint> origAddresses;
        byte reqByte = 0x23;
        byte ansByte = 0x23;
        int ansRows = 1;
        int dataOffset = 9;
        int addrOffset = 6;
        int sizeOffset = 4;

        //For VPW
        private byte PCMid;
        private byte toolId;

        //For CAN
        private bool corrupted = false;
        private int blockLen;
        private int blockParsed;
        private bool dataRows = false;
        private bool can3byteAddr = false;
        byte[] waitingBytes = new byte[3];

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
        private void ParseLine_VPW(string Line, bool SizeOnly)
        {
            try
            {
                int pos = Line.LastIndexOf("]");
                if (pos > 0)
                {
                    Line = Line.Substring(pos + 1).Trim();
                }
                byte[] tmp = Line.Replace(" ", "").ToBytes();
                if ((tmp.Length == 9 && tmp[3] == 0x35) || (tmp.Length == 10 && tmp[3] == 0x34) || (tmp.Length == 7 && tmp[3] == 0x27))
                {
                    if (toolId == 0)
                    {
                        toolId = tmp[1];
                        PCMid = tmp[2];
                        Logger("PCM: " + PCMid.ToString("X2") + ", Tool: " + toolId.ToString("X2"));
                    }

                }
                if (tmp[1] == PCMid && tmp[2] == toolId && tmp[3] == 0x36)
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
                        LoggerBold("Checksum error! Row: " + row.ToString() + " (" + calc.ToString("X4") + " <> " + msgChk.ToString("X4") + ")");
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
                        for (uint c = addr; c < addr + len; c++)
                        {
                            compareBuf[c] = 0;
                        }
                    }
                }
                else if (tmp[2] == PCMid && tmp[3] == 0x23)
                {
                    waitingBytes[0] = tmp[4];
                    waitingBytes[1] = tmp[5];
                    waitingBytes[2] = tmp[6];
                }
                else if (tmp.Length > 9 && tmp[1] == PCMid && tmp[3] == 0x63 && tmp[4] == waitingBytes[1] && tmp[5] == waitingBytes[2])
                {
                    int addr = waitingBytes[0] << 16 | waitingBytes[1] << 8 | waitingBytes[2];
                    if (SizeOnly)
                    {
                        if ((addr + 4) > FileSize)
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
                        compareBuf[addr] = 0;
                        compareBuf[addr + 1] = 0;
                        compareBuf[addr + 2] = 0;
                        compareBuf[addr + 3] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LogToBin line " + line + ": " + ex.Message);
                LoggerBold(ex.Message);
            }
        }
        private void ParseLine_CAN(string Line, bool SizeOnly)
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
                    blockLen = ReadInt16(lineData, 4, true) - 0x1005;
                    blockParsed = 0;
                    //block = new byte[blockLen];
                    addr = (uint)(lineData[8] << 16 | lineData[9] << 8 | lineData[10]);
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
                        for (uint c = addr; c < addr + copyLen; c++)
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

        private void ParseLine_CAN23_36(string Line, bool SizeOnly)
        {
            try
            {
                int pos = Line.LastIndexOf("]");
                if (pos > 0)
                {
                    Line = Line.Substring(pos + 1).Trim();
                }
                byte[] lineData = Line.Replace(" ", "").ToBytes();
                //if ((lineData.Length == 11 && lineData[0] == 0x00 && lineData[1] == 0x00 && lineData[2] == 0x07 && lineData[4] == reqByte))
                if (((lineData.Length == 10 || lineData.Length == 11) && lineData[0] == 0x00 && lineData[1] == 0x00 &&  lineData[4] == reqByte))
                {
                    //Request for data
                    reqRow = row;
                    if (lineData.Length == 10)
                    {
                        can3byteAddr = true;
                        if (readMode == RMode.CAN23)
                        {
                            dataOffset = 8;
                        }
                        else
                        {
                            dataOffset = 9;
                        }
                    }
                }
                //else if (lineData[0] == 0x00 && lineData[1] == 0x00 && lineData[2] == 0x07 && lineData[4] == ansByte && row == (reqRow + ansRows))
                else if (lineData.Length > dataOffset && lineData[0] == 0x00 && lineData[1] == 0x00  && lineData[4] == ansByte && row == (reqRow + ansRows))
                {
                    //uint fullAddr = (uint)(lineData[6] << 24 | lineData[7] << 16 | lineData[8] << 8 | lineData[9]);
                    uint origOffset = 0;
                    if (can3byteAddr)
                    {
                        addr = (uint)(lineData[addrOffset] << 16 | lineData[addrOffset + 1] << 8 | lineData[addrOffset + 2]);
                    }
                    else
                    {
                        addr = (uint)(lineData[addrOffset + 1] << 16 | lineData[addrOffset + 2] << 8 | lineData[addrOffset + 3]);
                        origOffset = (uint)(lineData[addrOffset] << 24);
                    }
                    int len = lineData.Length - dataOffset;
                    if (SizeOnly)
                    {
                        if (FileSize < (addr + len))
                        {
                            FileSize = (int)(addr + len);
                        }
                        if (!origAddresses.Contains(origOffset))
                        {
                            origAddresses.Add(origOffset);
                        }
                    }
                    else
                    {
                        for (int i=0;i<origAddresses.Count; i++)
                        {
                            if (origAddresses[i] == origOffset)
                            {
                                addr += mappedAddresses[i];
                                break;
                            }
                        }
                        Array.Copy(lineData, dataOffset, buf, addr, len);
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
                if (!ex.Message.StartsWith("Unable to convert"))
                {
                    LoggerBold(ex.Message);
                }
                Debug.WriteLine("Error, LogToBinConverter line " + line + ": " + ex.Message);
            }
        }

        private void ParseLine_CAN78(string Line, bool SizeOnly)
        {
            try
            {
                int pos = Line.LastIndexOf("]");
                if (pos > 0)
                {
                    Line = Line.Substring(pos + 1).Trim();
                }
                byte[] lineData = Line.Replace(" ", "").ToBytes();
                if (lineData.Length > 7)
                {
                    if (lineData[3] == 0xE8)
                    {
                        Debug.WriteLine("PCM answer: " + Line);
                    }
                    else if (lineData[4] == 0x10 && lineData[6] == reqByte)
                    {
                        //Request for data
                        addr = (uint)(lineData[addrOffset] << 24 |  lineData[addrOffset + 1] << 16 | lineData[addrOffset + 2] << 8 | lineData[addrOffset + 3]);
                        size = (uint)((lineData[sizeOffset] << 8 | lineData[sizeOffset+1]) - 0x1000);
                        received = 1;
                        if (SizeOnly)
                        {
                            if (FileSize < (addr + size))
                            {
                                FileSize = (int)(addr + size);
                            }
                        }
                        else
                        {
                            buf[addr] = lineData.LastOrDefault();
                        }
                        addr++;
                        reqRow = row;
                    }
                    else if (lineData.Length > dataOffset && lineData[0] == 0x00 && lineData[1] == 0x00)
                    {
                        int len = (int)Math.Min((lineData.Length - dataOffset), (size - received));
                        if (!SizeOnly)
                        {
                            Array.Copy(lineData, dataOffset, buf, addr, len);
                            Debug.WriteLine("Address: " + addr.ToString("X8") + ", size: " + len.ToString());
                            for (uint c = addr; c < addr + len; c++)
                            {
                                compareBuf[c] = 0;
                            }
                        }
                        addr += (uint)len;
                        received += (uint)len;
                    }
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                if (!ex.Message.StartsWith("Unable to convert"))
                {
                    LoggerBold(ex.Message);
                }
                Debug.WriteLine("Error, LogToBinConverter line " + line + ": " + ex.Message);
            }
        }

        public void ConvertLines(string[] lines, string FileName)
        {
            try
            { 
                //Parse file and check file size:
                row = 0;
                mappedAddresses = new List<uint>();
                origAddresses = new List<uint>();
                foreach (string Line in lines)
                {
                    row++;
                    switch(readMode)
                    {
                        case RMode.VPW:
                            ParseLine_VPW(Line, true);
                            break;
                        case RMode.CAN:
                            ParseLine_CAN(Line, true);
                            break;
                        case RMode.CAN23:
                            ParseLine_CAN23_36(Line, true);
                            break;
                        case RMode.CAN36:
                            ParseLine_CAN23_36(Line, true);
                            break;
                        case RMode.CAN78:
                            ParseLine_CAN78(Line, true);
                            break;
                    }
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
                        if (origAddresses[i] > 0 || mappedAddresses[i] > 0)
                        {
                            Logger("Mapping: " + origAddresses[i].ToString("X8") + " => " + mappedAddresses[i].ToString("X8"));
                        }
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
                    switch (readMode)
                    {
                        case RMode.VPW:
                            ParseLine_VPW(Line, false);
                            break;
                        case RMode.CAN:
                            ParseLine_CAN(Line, false);
                            break;
                        case RMode.CAN23:
                            ParseLine_CAN23_36(Line, false);
                            break;
                        case RMode.CAN36:
                            ParseLine_CAN23_36(Line, false);
                            break;
                        case RMode.CAN78:
                            ParseLine_CAN78(Line, false);
                            break;
                    }
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
                if (corrupted)
                {
                    DialogResult res = MessageBox.Show("File may be corrupted, save anyway?", "Warning", MessageBoxButtons.YesNo);
                    if (res == DialogResult.No)
                    {
                        Logger("Cancel saving");
                        return;
                    }
                }
                Logger("Writing to file: " + FileName);
                WriteBinToFile(FileName, buf);
                Logger("Done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LogToBinConverter line " + line + ": " + ex.Message);
            }
        }
        public void ConvertFile(string FileName)
        {
            try
            {
                string text;
                string binFile = SelectSaveFile(BinFilter, FileName + ".bin");
                if (string.IsNullOrEmpty(binFile))
                {
                    return;
                }
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
                ConvertLines(lines, binFile);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
    }
}
