using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmImportFile : Form
    {
        public frmImportFile()
        {
            InitializeComponent();
        }

        public string outFileName = "";

        private List<uint> SegStarts = new List<uint>();
        private List<uint> SegEnds = new List<uint>();
        private List<RecordBlock> rBlocks = new List<RecordBlock>();
        //private List<ImportSegment> importSegs = new List<ImportSegment>();
        private BindingSource bindingSource = new BindingSource();

        private class RecordBlock
        {
            public RecordBlock()
            {
                Name = "";
                data = new List<byte>();
                dataStart = uint.MaxValue;
                segEnd = uint.MaxValue;
                segStart = uint.MaxValue;
                Select = true;
                FileName = "";
                blockType = 0;
            }
            public uint segStart;
            public uint segEnd;
            public uint dataStart;
            public uint dataEnd;
            public List<byte> data;
            public ushort blockType; // 0 = Data, 1 = Gap

            public string Name { get; set; }
            public string Start
            {
                get { return dataStart.ToString("X"); }
                set { HexToUint(value, out dataStart); }
            }
            public string End
            {
                get { return dataEnd.ToString("X"); }
                set { HexToUint(value, out dataEnd); }
            }
/*            public string Range
            {
                get
                {
                    string retVal = "";
                    if (segStart < uint.MaxValue)
                        retVal = segStart.ToString("X") + " - ";
                    else
                        retVal = dataStart.ToString("X") + " - ";
                    if (segEnd < uint.MaxValue)
                        retVal += segEnd.ToString("X");
                    return retVal;
                }
            }
            public string DataRange
            {
                get
                {
                    string retVal = "";
                    if (dataStart < uint.MaxValue)
                        retVal = dataStart.ToString("X") + " - ";
                    if (dataEnd < uint.MaxValue)
                        retVal += dataEnd.ToString("X");
                    return retVal;
                }
            }*/
            public bool Select { get; set; }
            public string FileName { get; set; }
        }

        /*private class ImportSegment
        {
            public ImportSegment()
            {
                Name = "";
                Import = true;
            }
            public string Name { get; set; }
            public string Range { get; set; }
            public string DataRange { get; set; }
            public bool Import { get; set; }
            public string FileName { get; set; }
        }*/


        private void frmImportFile_Load(object sender, EventArgs e)
        {
            LogReceivers.Add(txtResult);
            refreshData();
        }

        private void refreshData()
        {
            bindingSource.DataSource = null;
            dataGridView1.DataSource = null;
            bindingSource.DataSource = rBlocks;
            dataGridView1.DataSource = bindingSource;
        }

        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            txtResult.AppendText(LogText);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        public void importMotorola(string fileName)
        {
            try
            {
                this.Text = "Import Motorola S-Record";
                labelFileName.Text = fileName;
                SRecord srecord = new SRecord();
                StreamReader file = new StreamReader(labelFileName.Text);
                RecordBlock rblock = new RecordBlock();
                bool firstRec = true;
                while (true)
                {
                    SRecordStructure rRec = srecord.Read(file);
                    if (rRec == null)
                    {
                        rBlocks.Add(rblock);
                        break;
                    }
                    if (firstRec)
                    {
                        rblock.segStart = rRec.address;
                        firstRec = false;
                    }

                    if (rRec.address > (rblock.dataStart + rblock.data.Count)) //gap
                    {
                        rBlocks.Add(rblock);
                        rblock = new RecordBlock();
                        rblock.segStart = rRec.address;
                    }
                    if (rRec.type > 0 && rRec.type < 4) //Data block
                    {
                        for (int b = 0; b < rRec.dataLen; b++)
                            rblock.data.Add(rRec.data[b]);
                        if (rblock.dataStart == uint.MaxValue)
                            rblock.dataStart = rRec.address;
                        rblock.dataEnd = (uint)(rRec.address + rRec.dataLen - 1);
                    }
                }

                //txtOffset.Text = rBlocks[0].dataStart.ToString("X");
                txtFileSize.Text = (rBlocks[rBlocks.Count - 1].dataEnd - rBlocks[0].dataStart + 1).ToString("X");
                addGaps();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmImport line " + line + ": " + ex.Message);
            }

        }

        private void addGaps()
        {
            uint totalData = 0;
            for (int b = 0; b < rBlocks.Count; b++)
            {
                //Logger("Segment start: " + rBlocks[b].segStart.ToString("X"));
                if (b == 0 && rBlocks[b].dataStart > rBlocks[b].segStart)
                {
                    //Logger("Gap: " + rBlocks[b].segStart.ToString("X") + " - " + (rBlocks[b].dataStart - 1).ToString("X"));
                    RecordBlock iblock = new RecordBlock();
                    iblock.blockType = 1;
                    iblock.segStart = rBlocks[0].segStart;
                    iblock.dataStart = rBlocks[0].segStart;
                    iblock.dataEnd = rBlocks[0].dataStart - 1;
                    rBlocks.Insert(0, iblock);
                }

                if (b > 0 && rBlocks[b].dataStart > (rBlocks[b - 1].dataEnd + 1))
                {
                    //Logger("Gap: " + (rBlocks[b - 1].dataEnd + 1).ToString("X") + " - " + (rBlocks[b].dataStart - 1).ToString("X"));
                    RecordBlock iblock = new RecordBlock();
                    iblock.blockType = 1;
                    //iblock.segStart = rBlocks[0].segStart;
                    iblock.dataStart = rBlocks[b - 1].dataEnd + 1;
                    iblock.dataEnd = rBlocks[b].dataStart - 1;
                    rBlocks.Insert(b, iblock);

                }
                //Logger("Segment data: " + rBlocks[b].dataStart.ToString("X") + " - " + rBlocks[b].dataEnd.ToString("X"));
                totalData += (uint)rBlocks[b].data.Count;
            }
            for (int b = 0; b < rBlocks.Count; b++)
            {
                if (rBlocks[b].blockType == 0)
                    rBlocks[b].Name = "Data-" + b.ToString();
                else
                    rBlocks[b].Name = "Gap-" + b.ToString();
            }
            Logger("Total data: " + totalData.ToString());

        }

        public void importIntel(string fileName)
        {
            try
            {
                this.Text = "Import Intel HEX";
                labelFileName.Text = fileName;
                IntelHex intelHex = new IntelHex();
                StreamReader file = new StreamReader(labelFileName.Text);
                bool firstRec = true;
                RecordBlock iblock = new RecordBlock();
                while (true)
                {
                    IntelHexStructure ihex = intelHex.Read(file);
                    if (ihex == null)
                    {
                        rBlocks.Add(iblock);
                        break;
                    }
                    if (firstRec)
                    {
                        iblock.segStart = ihex.address;
                        firstRec = false;
                    }
                    if (ihex.address > (iblock.dataStart + iblock.data.Count)) //gap
                    {
                        rBlocks.Add(iblock);
                        iblock = new RecordBlock();
                        iblock.segStart = ihex.address;
                    }
                    if (ihex.type == 0) //Data block
                    {
                        for (int b = 0; b < ihex.dataLen; b++)
                            iblock.data.Add(ihex.data[b]);
                        if (iblock.dataStart == uint.MaxValue)
                            iblock.dataStart = ihex.address;
                        iblock.dataEnd = (uint)(ihex.address + ihex.dataLen - 1);
                    }
                }

                addGaps();
                txtFileSize.Text = (rBlocks[rBlocks.Count - 1].dataEnd - rBlocks[0].dataStart + 1).ToString("X");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmImport line " + line + ": " + ex.Message);
            }

        }

        public static List<byte> Decompress2(List<byte> input)
        {
            const int THRESHOLD = 2;
            const int BufferSize = 1 << 10;
            const int DictionarySize = 34;
            var text_buf = new byte[BufferSize];
            int inputIdx = 0;
            var output = new List<byte>();
            int bufferIdx = BufferSize - DictionarySize; //r
            byte c = 0;
            ushort flags;


            for (int i = 0; i < BufferSize - DictionarySize; i++)
                text_buf[i] = 0x0;

            flags = 0;
            for (; ; )
            {
                if (((flags >>= 1) & 0x100) == 0)
                {
                    if (inputIdx < input.Count) c = input[inputIdx++]; else break;
                    flags = (ushort)(c | 0xFF00);  /* uses higher byte cleverly */
                }   /* to count eight */
                if ((flags & 1) > 0)
                {
                    if (inputIdx < input.Count) c = input[inputIdx++]; else break;
                    output.Add(c);
                    text_buf[bufferIdx++] = c;
                    bufferIdx &= (BufferSize - 1);
                }
                else
                {
                    int i = 0;
                    int j = 0;
                    if (inputIdx < input.Count) i = input[inputIdx++]; else break;
                    if (inputIdx < input.Count) j = input[inputIdx++]; else break;
                    i |= ((j & 0xE0) << 3);
                    j = (j & 0x1F) + THRESHOLD;
                    for (int k = 0; k <= j; k++)
                    {
                        c = text_buf[(i + k) & (BufferSize - 1)];
                        output.Add(c);
                        text_buf[bufferIdx++] = c;
                        bufferIdx &= (BufferSize - 1);
                    }
                }
            }

            return output;
        }



        public void importSGM(string fileName)
        {
            try
            {
                this.Text = "Import SGM file";
                labelFileName.Text = fileName;
                Logger("Reading file: " + fileName, false);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(SWCNT));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                SWCNT swcnt = (SWCNT)reader.Deserialize(file);
                file.Close();
                Logger(" [OK], converting...");

                SWCNTDATEN data = (SWCNTDATEN)swcnt.Items[3];
                SWCNTDATENDATENBLOECKEDATENBLOCK[] segments = data.DATENBLOECKE;

                int blockCount = segments.Length;
                //List<byte[]> binBlocks = new List<byte[]>();
                int size = 0;
                uint totalData = 0;
                for (int i = 0; i < blockCount; i++)
                {
                    RecordBlock rBlock = new RecordBlock();
                    string b64Data = segments[i].DATENBLOCKDATEN;
                    b64Data = b64Data.Substring(b64Data.IndexOf("base64") + 6);
                    byte[] b = System.Convert.FromBase64String(b64Data);
                    string bStr = System.Text.Encoding.UTF8.GetString(b);
                    List<byte> compressed = new List<byte>();
                    int pos = bStr.IndexOf("[END_ADDRESS]");
                    for (; pos < b.Length; pos++)
                    {
                        if (b[pos] == 0x0a && b[pos + 1] == 0x1a && b[pos + 2] == 1)
                            break;
                    }

                    int pos2 = (b[pos + 2] << 8) + b[pos + 3];
                    for (int y = pos2; y < b.Length; y++)
                        compressed.Add(b[y]);
                    //string uncompressed = Decompress(compressed);
                    //rBlock.data = Encoding.ASCII.GetBytes(uncompressed).ToList();
                    rBlock.data = Decompress2(compressed);
                    //rBlock.data = b.ToList();
                    totalData += (uint)rBlock.data.Count;
                    //binBlocks.Add(b);
                    int start;
                    HexToInt(segments[i].STARTADR.Replace("0x", ""), out start);
                    rBlock.dataStart = (uint)start;
                    rBlock.dataEnd = (uint)(rBlock.dataStart + rBlock.data.Count);
                    int bSize = 0;
                    HexToInt(segments[i].GROESSEDEKOMPRIMIERT.Replace("0x", ""), out bSize);
                    if (size < (start + bSize))
                        size = start + bSize;
                    if (i == 0)
                    {
                        if (start > 0)
                            Logger("Gap: 0 - " + (start - 1).ToString("X") + ", ", false);
                    }
                    else
                    {
                        if (start > (rBlocks[i - 1].dataEnd + 1))
                            Logger("Gap: " + (rBlocks[i - 1].dataEnd + 1).ToString("X") + " - " + (start - 1).ToString("X") + ", ", false);

                    }
                    Logger("Data: " + start.ToString("X") + " - " + (start + rBlock.data.Count).ToString("X"));
                    Logger("Segment: " + segments[i].DATENBLOCKNAME + " ", false);
                    SWCNTDATENDATENBLOECKEDATENBLOCKDATENBLOCKCHECK[] cs = segments[i].DATENBLOCKCHECK;
                    SWCNTDATENDATENBLOECKEDATENBLOCKLOESCHBEREICH[] segBlocks = segments[i].LOESCHBEREICH;
                    string sbStr = "";
                    for (int sb = 0; sb < segBlocks.Length; sb++)
                    {
                        sbStr += segBlocks[sb].STARTADR.Replace("0x", "") + " - " + segBlocks[sb].ENDADR.Replace("0x", "") + ", ";
                    }
                    //sbStr = sbStr.Trim(',');
                    Logger(sbStr, false);

                    for (int x = 0; x < cs.Length; x++)
                    {
                        Logger("Checksum: " + cs[x].CHECKSUMME.Replace("0x", "") + " (" + cs[x].STARTADR.Replace("0x", "") + " - " + cs[x].ENDADR.Replace("0x", "") + ")");
                        int csend = 0;
                        HexToInt(cs[x].ENDADR.Replace("0x", ""), out csend);
                        if (csend > size)
                            size = csend;
                    }
                    rBlocks.Add(rBlock);
                }
                txtOffset.Text = "0";
                txtFileSize.Text = size.ToString("X");
                Logger("Total data: " + totalData.ToString());
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmImport line " + line + ": " + ex.Message);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                /*uint offset = 0;
                if (!HexToUint(txtOffset.Text, out offset))
                    throw new Exception("Can't decode HEX offset: " + txtOffset.Text);*/
                int fsize = 0;
                if (!HexToInt(txtFileSize.Text, out fsize))
                    throw new Exception("Can't convert files size from HEX: " + txtFileSize.Text);


                if (chkSplit.Checked)
                {
                    string fldr = SelectFolder("Save to folder:");
                    for (int i = 0; i < rBlocks.Count; i++)
                    {
                        if (rBlocks[i].Select && rBlocks[i].blockType == 0)
                        {
                            outFileName = Path.Combine(fldr, rBlocks[i].FileName);
                            Logger("Saving to file: " + outFileName, false);
                            WriteBinToFile(outFileName, rBlocks[i].data.ToArray());
                            Logger(" [OK]");
                        }                        
                    }
                    Logger("[OK]");
                    //this.DialogResult = DialogResult.No;
                }
                else
                {
                    uint minFSize = 0;
                    for (int i = 0; i < rBlocks.Count; i++)
                    {
                        if (rBlocks[i].Select)
                            minFSize += rBlocks[i].dataEnd - rBlocks[i].dataStart + 1;
                    }
                    if (fsize < minFSize)
                    {
                        LoggerBold("Minimum filesize for current selections: " + minFSize.ToString("X"));
                        return;
                    }
                    string defName = Path.GetFileNameWithoutExtension(labelFileName.Text) + ".bin";
                    outFileName = SelectSaveFile("BIN (*.bin)|*.bin|All(*.*)|*.*", defName);
                    if (outFileName.Length == 0)
                        return;
                    Logger("Saving to file: " + outFileName);

                    List<byte> fillBytes = new List<byte>();
                    if (txtFillGaps.Text.Length > 0)
                    {
                        string[] fillStr = txtFillGaps.Text.Split(' ');
                        foreach (string str in fillStr)
                        {
                            byte b;
                            HexToByte(str, out b);
                            fillBytes.Add(b);
                        }
                    }

                    byte[] buf = new byte[fsize];
                    if (fillBytes.Count > 0)
                    {
                        for (uint addr = 0; addr < fsize; addr++)
                        {
                            for (int x = 0; x < fillBytes.Count; x++)
                                if ((addr + x) < fsize)
                                    buf[addr + x] = fillBytes[x];
                        }
                    }
                    uint pos = 0;
                    for (int r = 0; r < rBlocks.Count; r++)
                    {
                        if (rBlocks[r].Select)
                        {
                            if (rBlocks[r].blockType == 0)
                            {
                                Logger("Block: " + rBlocks[r].Name + ": " + pos.ToString("X") + " - " + (pos + rBlocks[r].data.Count - 1).ToString("X"));
                                Array.Copy(rBlocks[r].data.ToArray(), 0, buf, pos, rBlocks[r].data.Count);
                                pos += (uint)rBlocks[r].data.Count ;
                            }
                            else
                            {
                                pos += rBlocks[r].dataEnd - rBlocks[r].dataStart + 1;
                            }
                        }
                    }
                    WriteBinToFile(outFileName, buf);
                    Logger("[OK]");
                    //this.DialogResult = DialogResult.OK;
                }
                //this.Close();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmImport line " + line + ": " + ex.Message);
            }
        }

        private void chkSplit_CheckedChanged(object sender, EventArgs e)
        {
            string baseFile = labelFileName.Text;
            for (int i = 0; i < rBlocks.Count; i++)
            {
                if (rBlocks[i].blockType == 0 && rBlocks[i].FileName.Length == 0)
                {

                    if (rBlocks[i].Name.Length > 0)
                        rBlocks[i].FileName = Path.GetFileNameWithoutExtension(baseFile) + "-" + rBlocks[i].Name + ".bin";
                    else
                        rBlocks[i].FileName = Path.GetFileNameWithoutExtension(baseFile) + "-" + i.ToString() + ".bin";
                }
            }
            refreshData();
        }
    }
}
