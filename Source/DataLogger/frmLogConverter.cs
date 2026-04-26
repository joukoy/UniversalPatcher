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
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmLogConverter : Form
    {
        public frmLogConverter()
        {
            InitializeComponent();
        }

        private List<byte[]> DataRows;
        private string FileName;
        private DataTable dt;
        private byte[] buf = null;
        private byte[] compareBuf = null;
        private int addr = 0;
        private int blockLen = 0;
        private int FileSize = 0;

        private void frmLogConverter_Load(object sender, EventArgs e)
        {
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            comboAddrSrc.DataSource = Enum.GetValues(typeof(LogConverter.LenSrc));
            comboLenSrc.DataSource = Enum.GetValues(typeof(LogConverter.LenSrc));
            if (!string.IsNullOrEmpty(AppSettings.LastLogConverterXml))
            {
                LoadSettingsFile(AppSettings.LastLogConverterXml);
            }
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            (sender as DataGridView).Rows[e.RowIndex].HeaderCell.Value = "Row: " + e.RowIndex.ToString();
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            int row = dataGridView1.SelectedCells[0].RowIndex;
            if (row < 0 || row >= DataRows.Count)
            {
                return;
            }
            LogConverter lc = CreateSettings();
            ConvertLine(row, lc, true);
            RowType rt = DetectRowType(row);
            txtRowInfo.Text = "Row: " + rt.ToString() + Environment.NewLine;
            if (rt == RowType.Request)
            {
                if (lc.AddressSource == LogConverter.LenSrc.Requestrow)
                {
                    txtRowInfo.Text += "Address: " + addr.ToString("X") + Environment.NewLine; 
                }
                if (lc.BlockSizeSource == LogConverter.LenSrc.Requestrow)
                {
                    txtRowInfo.Text += "Block length: " + blockLen.ToString("X") + " (" + blockLen.ToString() + ")" + Environment.NewLine;
                }
            }
            else if (rt == RowType.Data)
            {
                if (lc.AddressSource == LogConverter.LenSrc.Datarow)
                {
                    txtRowInfo.Text += "Address: " + addr.ToString("X") + Environment.NewLine;
                }
                if (lc.BlockSizeSource == LogConverter.LenSrc.Datarow || lc.BlockSizeSource == LogConverter.LenSrc.Datacount)
                {
                    txtRowInfo.Text += "Block length: " + blockLen.ToString("X") + " (" + blockLen.ToString() + ")" + Environment.NewLine;
                }
                int len = DataRows[row].Length - lc.DataOffset;
                txtRowInfo.Text += "Data bytes in row: 0x" + len.ToString("X") + " (" + len.ToString() + ")";
            }
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, richTextBox1);
        }

        private enum RowType
        {
            Request,
            Data,
            Other
        }
        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView1.RowHeadersWidth = 100;
            SetCellColors();
        }

        private byte[] LineToBytes(string Line)
        {
            try
            {

                int pos = Line.LastIndexOf("]");
                if (pos > 0)
                {
                    Line = Line.Substring(pos + 1).Trim();
                }
                byte[] lineData = Line.Replace(" ", "").ToBytes();
                return lineData;
            }
            catch
            {
                Debug.WriteLine("Unknown data: " + Line);
                return null;
            }
        }

        public void ConvertLines(string[] lines)
        {
            DataRows = new List<byte[]>();
            foreach (string line in lines)
            {
                byte[] lineData = LineToBytes(line);
                if (lineData != null)
                {
                    DataRows.Add(lineData);
                }
            }
            dt = new DataTable();
            for (int i = 0; i <= 20; i++)
            {
                dt.Columns.Add(i.ToString());
            }
            for (int i = 0; i < DataRows.Count; i++)
            {
                DataRow dRow = dt.NewRow();
                for (int c = 0; c < DataRows[i].Length && c < 20; c++)
                {
                    dRow[c] = DataRows[i][c].ToString("X2");
                }
                if (DataRows[i].Length > 20)
                {
                    dRow[19] = "(" + DataRows[i].Length.ToString() + " bytes)";
                }
                dt.Rows.Add(dRow);
            }
            dataGridView1.DataSource = dt;

        }
        public void ReadFile()
        {
            try
            {
                FileName = SelectFile("Select logfile", RtfFTxtFilter);
                if (FileName.Length == 0)
                {
                    return;
                }
                Logger("Reading file: " + FileName);
                string text;
                if (FileName.ToLower().EndsWith(".rtf"))
                {
                    Logger("Converting RTF to TXT...", false);
                    Application.DoEvents();
                    RichTextBox rtb = new RichTextBox();
                    Logger(" [OK]");
                    rtb.LoadFile(FileName);
                    text = rtb.Text;
                }
                else
                {
                    text = File.ReadAllText(FileName);
                }
                string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                ConvertLines(lines);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogConverter line " + line + ": " + ex.Message);
            }

        }
        private void btnReadFile_Click(object sender, EventArgs e)
        {
            ReadFile();
        }

        private List<int> GetSelectionArea()
        {
            List<int> sel = new List<int>();
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return sel;
            }
            int row = dataGridView1.SelectedCells[0].RowIndex;
            if (row < 0 || row >= DataRows.Count)
            {
                return sel;
            }
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                if (dataGridView1.SelectedCells[i].RowIndex != row)
                {
                    LoggerBold("Select bytes from one line!");
                    return sel;
                }
            }
            int start = dataGridView1.SelectedCells[0].ColumnIndex;
            int end = dataGridView1.SelectedCells[dataGridView1.SelectedCells.Count - 1].ColumnIndex;
            if (start > end)
            {
                int tmp = start;
                start = end;
                end = tmp;
            }
            sel.Add(row);
            sel.Add(start);
            sel.Add(end);
            return sel;
        }
        private void setRequestDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> sel = GetSelectionArea();
            if (sel.Count < 3)
            {
                return;
            }
            txtDetectRequest.Text = "";
            for (int c=0; c <= sel[2]; c++)
            {
                if (c < sel[1])
                {
                    txtDetectRequest.Text += "* ";
                }
                else
                {
                    txtDetectRequest.Text += dataGridView1.Rows[sel[0]].Cells[c].Value.ToString() + " ";
                }
            }
            SetCellColors();
        }

        private void setDataDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> sel = GetSelectionArea();
            if (sel.Count < 3)
            {
                return;
            }
            txtDetectData.Text = "";
            for (int c = 0; c <= sel[2]; c++)
            {
                if (c < sel[1])
                {
                    txtDetectData.Text += "* ";
                }
                else
                {
                    txtDetectData.Text += dataGridView1.Rows[sel[0]].Cells[c].Value.ToString() + " ";
                }
            }
            SetCellColors();
        }

        private LogConverter CreateSettings()
        {
            LogConverter lc = new LogConverter();
            lc.AddressBytes = (int)numAddrBytes.Value;
            lc.AddressOffset = (int)numAddrPos.Value;
            lc.AddressSource = (LogConverter.LenSrc)Enum.Parse(typeof(LogConverter.LenSrc), comboAddrSrc.Text);
            lc.BlockSizeSource = (LogConverter.LenSrc)Enum.Parse(typeof(LogConverter.LenSrc), comboLenSrc.Text);
            lc.SetDataDetection(txtDetectData.Text);
            lc.DataOffset = (int)numDataOffset.Value;
            lc.LenBytes = (int)numLenBytes.Value;
            lc.LenOffset = (int)numLenPos.Value;
            lc.SetRequestDetection(txtDetectRequest.Text);
            lc.RowsFrom = (int)numRowsFrom.Value;
            if (numRowsTo.Value < 0)
            {
                lc.RowsTo = int.MaxValue;
            }
            else
            {
                lc.RowsTo = (int)numRowsTo.Value;
            }
            if (HexToByte(txtFillByte.Text, out byte b))
            {
                lc.FillEmpty = b;
            }
            if (HexToInt(txtFixedLen.Text, out int x))
            {
                lc.FixedLength = x;
            }
            if (HexToInt(txtGlobalOffset.Text, out int o))
            {
                lc.GlobalOffset = o;
            }
            return lc;
        }

        private void SetCellColors()
        {
            if (DataRows == null || DataRows.Count == 0)
            {
                return;
            }
            LogConverter lc = CreateSettings();

            int firstrq = 0;
            txtDetectData.BackColor = Color.LightBlue;
            txtDetectRequest.BackColor = Color.LightGreen;
            numAddrBytes.BackColor = Color.Yellow;
            numAddrPos.BackColor = Color.Yellow;
            comboAddrSrc.BackColor = Color.Yellow;
            numLenBytes.BackColor = Color.LightCoral;
            numLenPos.BackColor = Color.LightCoral;
            comboLenSrc.BackColor = Color.LightCoral;
            txtFixedLen.BackColor = Color.LightCoral;
            numDataOffset.BackColor = Color.MediumPurple;
            for (int r=0;r<DataRows.Count;r++)
            {
                byte[] lineData = DataRows[r];
                RowType rt = DetectRowType(r);
                if (r < lc.RowsFrom || r > lc.RowsTo)
                {
                    for (int c = 0; c < 20; c++)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.LightGray;
                    }
                    dataGridView1.Rows[r].HeaderCell.Style.BackColor = Color.LightGray;
                    continue;
                }
                else
                {
                    for (int c = 0; c < 20; c++)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.White;
                    }
                }
                if (rt == RowType.Request)
                {
                    if (firstrq == 0)
                    {
                        firstrq = r;
                    }
                    for (int c=0;c < lc.RequestBytes.Count;c++)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.LightBlue;
                    }
                    if (lc.AddressSource == LogConverter.LenSrc.Requestrow && lc.AddressOffset >= lc.RequestBytes.Count)
                    {
                        for (int a=0;a<lc.AddressBytes; a++)
                        {
                            dataGridView1.Rows[r].Cells[lc.AddressOffset + a].Style.BackColor = Color.Yellow;
                        }
                    }
                    if (lc.BlockSizeSource == LogConverter.LenSrc.Requestrow && lc.LenOffset >= lc.RequestBytes.Count )
                    {
                        for (int a=0; a < lc.LenBytes; a++)
                        {
                            dataGridView1.Rows[r].Cells[lc.LenOffset + a].Style.BackColor = Color.LightCoral;
                        }
                    }
                }
                else if (rt == RowType.Data)
                {
                    for (int c = 0; c < lc.DataBytes.Count; c++)
                    {
                        dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.LightGreen;
                    }
                    if (lc.AddressSource == LogConverter.LenSrc.Datarow)
                    {
                        if (lc.AddressOffset >= lc.DataBytes.Count)
                        {
                            for (int a = 0; a < lc.AddressBytes; a++)
                            {
                                dataGridView1.Rows[r].Cells[lc.AddressOffset + a].Style.BackColor = Color.Yellow;
                            }
                        }
                    }
                    if (lc.DataOffset >= lc.DataBytes.Count)
                    {
                        for (int a=lc.DataOffset;a< 20; a++)
                        {
                            dataGridView1.Rows[r].Cells[a].Style.BackColor = Color.MediumPurple;
                        }
                    }
                    if (lc.BlockSizeSource == LogConverter.LenSrc.Datarow && lc.LenOffset >= lc.RequestBytes.Count)
                    {
                        for (int a = 0; a < lc.LenBytes; a++)
                        {
                            dataGridView1.Rows[r].Cells[lc.LenOffset + a].Style.BackColor = Color.LightCoral;
                        }
                    }
                }
            }
            if (dataGridView1.CurrentCell.RowIndex < 5)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[firstrq].Cells[0];
            }
        }
        private void LoadSettings(LogConverter lc)
        {
            numAddrBytes.Value = lc.AddressBytes;
            numAddrPos.Value = lc.AddressOffset;
            comboAddrSrc.Text = lc.AddressSource.ToString();
            comboLenSrc.Text = lc.BlockSizeSource.ToString();
            txtDetectData.Text = lc.GetDataDetectionsString();
            numDataOffset.Value = lc.DataOffset;
            txtFixedLen.Text = lc.FixedLength.ToString("X");
            numLenBytes.Value = lc.LenBytes;
            numLenPos.Value = lc.LenOffset;
            txtDetectRequest.Text = lc.GetRequestDetectionsString();
            txtFillByte.Text = lc.FillEmpty.ToString("X2");
            numRowsFrom.Value = lc.RowsFrom;
            txtGlobalOffset.Text = lc.GlobalOffset.ToString("X");
            if (lc.RowsTo == int.MaxValue)
            {
                numRowsTo.Value = -1;
            }
            else
            {
                numRowsTo.Value = lc.RowsTo;
            }
            SetCellColors();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string defNme = Path.Combine(Application.StartupPath, "Logger", "LogConverter", "Settings.XML");
            string fName = SelectSaveFile(XmlFilter,defNme);
            if (fName.Length == 0)
            {
                return;
            }
            LogConverter lc = CreateSettings();

            Logger("Saving file " + fName + "...", false);
            using (FileStream stream = new FileStream(fName, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(LogConverter));
                writer.Serialize(stream, lc);
                stream.Close();
            }
            AppSettings.LastLogConverterXml = fName;
            AppSettings.Save();
            labelSettingsFile.Text = Path.GetFileName(fName);
            Logger(" [OK]");


        }

        private void LoadSettingsFile(string fName)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(LogConverter));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                LogConverter lc = (LogConverter)reader.Deserialize(file);
                file.Close();
                AppSettings.LastLogConverterXml = fName;
                AppSettings.Save();
                labelSettingsFile.Text = Path.GetFileName(fName);
                LoadSettings(lc);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogConverter line " + line + ": " + ex.Message);
            }
        }
        private void bntLoad_Click(object sender, EventArgs e)
        {
            string defNme = Path.Combine(Application.StartupPath, "Logger", "LogConverter", "Settings.XML");
            string fName = SelectFile("Select converter settings", XmlFilter,defNme);
            if (fName.Length == 0)
            {
                return;
            }
            LoadSettingsFile(fName);
        }
        
        private RowType DetectRowType(int row)
        {
            LogConverter lc = CreateSettings();
            if (lc.DataBytes.Count > 0)
            {
                bool isData = true;
                for (int c=0;c<lc.DataBytes.Count;c++)
                {
                    if (lc.DataBytes[c] >= 0 && lc.DataBytes[c] != DataRows[row][c])
                    {
                        isData = false;
                        break;
                    }
                }
                if (isData)
                {
                    return RowType.Data;
                }
            }
            if (lc.RequestBytes.Count > 0)
            {
                bool isRequest = true;
                for (int c = 0; c < lc.RequestBytes.Count; c++)
                {
                    if (lc.RequestBytes[c] >= 0 && lc.RequestBytes[c] != DataRows[row][c])
                    {
                        isRequest = false;
                        break;
                    }
                }
                if (isRequest)
                {
                    return RowType.Request;
                }
            }
            return RowType.Other;
        }
        private void setAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> sel = GetSelectionArea();
            if (sel.Count < 3)
            {
                return;
            }
            RowType rt = DetectRowType(sel[0]);
            numAddrBytes.Value = sel[2] - sel[1] + 1;
            numAddrPos.Value = sel[1];
            Logger("Address from " + rt.ToString() + ", bytes " + sel[1].ToString() + " - " + sel[2].ToString());
            if (rt == RowType.Request)
            {
                comboAddrSrc.Text = LogConverter.LenSrc.Requestrow.ToString();
            }
            else if (rt == RowType.Data)
            {
                comboAddrSrc.Text = LogConverter.LenSrc.Datarow.ToString();
            }
            SetCellColors();
        }

        private void setLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> sel = GetSelectionArea();
            if (sel.Count < 3)
            {
                return;
            }
            RowType rt = DetectRowType(sel[0]);
            numLenBytes.Value = sel[2] - sel[1] + 1;
            numLenPos.Value = sel[1];
            Logger("Block length from " + rt.ToString() + ", bytes " + sel[1].ToString() + " - " + sel[2].ToString());
            if (rt == RowType.Request)
            {
                comboLenSrc.Text = LogConverter.LenSrc.Requestrow.ToString();
            }
            else if (rt == RowType.Data)
            {
                comboLenSrc.Text = LogConverter.LenSrc.Datarow.ToString();
            }
            SetCellColors();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void setDataOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            numDataOffset.Value = dataGridView1.SelectedCells[0].ColumnIndex;
            Logger("Setting data offset to: " + dataGridView1.SelectedCells[0].ColumnIndex.ToString());
            SetCellColors();
        }

        private void ConvertLine(int row, LogConverter lc, bool DetectOnly)
        {
            try
            {
                RowType rt = DetectRowType(row);
                byte[] lineData = DataRows[row];
                if (rt == RowType.Request)
                {
                    if (lc.AddressSource == LogConverter.LenSrc.Requestrow && lineData.Length > lc.AddressOffset)
                    {
                        addr = (int)ReadValue(lineData, lc.AddressOffset, lc.AddressBytes) - lc.GlobalOffset;
                    }
                    if (lc.BlockSizeSource == LogConverter.LenSrc.Requestrow && lineData.Length > lc.LenOffset)
                    {
                        blockLen = (int)ReadValue(lineData, lc.LenOffset, lc.LenBytes);
                    }
                    else if (lc.BlockSizeSource == LogConverter.LenSrc.Datacount && lineData.Length > lc.DataOffset)
                    {
                        blockLen = lineData.Length - lc.DataOffset;
                    }
                }
                else if (rt == RowType.Data)
                {
                    if (lc.BlockSizeSource == LogConverter.LenSrc.Datarow && lineData.Length > lc.LenOffset)
                    {
                        blockLen = (int)ReadValue(lineData, lc.LenOffset, lc.LenBytes);
                    }
                    else if (lc.BlockSizeSource == LogConverter.LenSrc.Datacount && lineData.Length > lc.DataOffset)
                    {
                        blockLen = lineData.Length - lc.DataOffset;
                    }
                    if (lc.AddressSource == LogConverter.LenSrc.Datarow && lineData.Length > lc.AddressOffset)
                    {
                        addr = (int)ReadValue(lineData, lc.AddressOffset, lc.AddressBytes) - lc.GlobalOffset;
                    }
                    int len = Math.Min(blockLen, (lineData.Length - lc.DataOffset));
                    if (DetectOnly)
                    {
                        if (len > 0 && FileSize < FileSize + len)
                        {
                            FileSize = addr + len;
                        }
                    }
                    else if (len > 0)
                    {
                        Array.Copy(lineData, lc.DataOffset, buf, addr, len);
                        Debug.WriteLine("Address: " + addr.ToString("X8") + ", size: " + len.ToString());
                        for (int c = addr; c < addr + len; c++)
                        {
                            compareBuf[c] = 0;
                        }
                    }
                    if (lc.AddressSource == LogConverter.LenSrc.Datacount)
                    {
                        addr += len;
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
                Debug.WriteLine("Error, frmLogConverter line " + line + ": " + ex.Message);
            }
        }
        public byte[] ConvertLog()
        {
            try
            {
                blockLen = 0;
                addr = 0;
                FileSize = 0;
                LogConverter lc = CreateSettings();
                if (lc.BlockSizeSource == LogConverter.LenSrc.Fixed)
                {
                    blockLen = lc.FixedLength;
                }

                //Detect filesize:
                for (int r = lc.RowsFrom; r < DataRows.Count && r < lc.RowsTo; r++)
                {
                    byte[] lineData = DataRows[r];
                    ConvertLine(r, lc, true);
                }

                //Create buffers for data
                buf = new byte[FileSize];
                compareBuf = new byte[FileSize];
                for (int a = 0; a < FileSize; a++)
                {
                    buf[a] = lc.FillEmpty;
                    compareBuf[a] = 0xFF;
                }

                //Convert:
                for (int r = lc.RowsFrom; r < DataRows.Count && r < lc.RowsTo; r++)
                {
                    byte[] lineData = DataRows[r];
                    ConvertLine(r, lc, false);
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
                    foreach (string s in skipped)
                    {
                        Logger(s);
                    }
                }

                return buf;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogConverter line " + line + ": " + ex.Message);
            }
            return null;
        }

        public void ConvertLogTxtToBinFile(string[] lines, string FileName, string ConfigFile)
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigFile))
                {
                    string defNme = Path.Combine(Application.StartupPath, "Logger", "LogConverter", "Settings.XML");
                    ConfigFile = SelectFile("Select converter settings", XmlFilter, defNme);
                }
                else
                {
                    if (!File.Exists(ConfigFile))
                    {
                        ConfigFile = Path.Combine(Application.StartupPath, "Logger", "LogConverter", ConfigFile);
                    }
                }
                if (string.IsNullOrEmpty(ConfigFile) || !File.Exists(ConfigFile))
                {
                    LoggerBold("Empty file or file not found: " + ConfigFile);
                    return;
                }
                LoadSettingsFile(ConfigFile);
                ConvertLines(lines);
                byte[] binData = ConvertLog();
                Logger("Writing file: " + FileName + ", size: " + buf.Length.ToString(), false);
                File.WriteAllBytes(FileName, binData);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogConverter line " + line + ": " + ex.Message);
            }

        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] binData = ConvertLog();
                string binFile = SelectSaveFile(BinFilter, FileName + ".bin");
                if (string.IsNullOrEmpty(binFile))
                {
                    return;
                }
                Logger("Writing file: " + binFile + ", size: " + buf.Length.ToString(), false);
                File.WriteAllBytes(binFile, binData);
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmLogConverter line " + line + ": " + ex.Message);
            }
        }

        private void btnUpdateColors_Click(object sender, EventArgs e)
        {
            SetCellColors();
        }

        private void groupXml_Enter(object sender, EventArgs e)
        {

        }

        private void setFirstRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            numRowsFrom.Value = dataGridView1.SelectedCells[0].RowIndex;
            SetCellColors();
        }

        private void setLastRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            numRowsTo.Value = dataGridView1.SelectedCells[0].RowIndex;
            SetCellColors();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
