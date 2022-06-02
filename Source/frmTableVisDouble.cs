using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmTableVisDouble : Form
    {
        public frmTableVisDouble(PcmFile PCM,TableData td, PcmFile PCM2, TableData td2)
        {
            InitializeComponent();
            this.PCM1 = PCM;
            this.td1 = td;
            this.PCM2 = PCM2;
            this.td2 = td2;
            this.Text = "Table data visualizer [" + td.TableName +"]";
            richTableData.VScroll += RichTableData_VScroll;
            richTableData2.VScroll += RichTableData2_VScroll;
            richTableData.SelectionChanged += RichTableData_SelectionChanged;
            richTableData2.SelectionChanged += RichTableData2_SelectionChanged;
            richTableData.EnableContextMenu();
            richTableData2.EnableContextMenu();
            numExtraOffset1.Value = td.ExtraOffset;
            numExtraOffset2.Value = td2.ExtraOffset;

            CreateHexData(PCM, ref hexDatas1, td1);
            CreateHexData(PCM2, ref hexDatas2, td2);
        }

        private void RichTableData2_SelectionChanged(object sender, EventArgs e)
        {
            richTableData.Select(richTableData2.SelectionStart, richTableData2.SelectionLength);
        }

        private void RichTableData_SelectionChanged(object sender, EventArgs e)
        {
            richTableData2.Select(richTableData.SelectionStart, richTableData.SelectionLength);
        }

        private PcmFile PCM1;
        private TableData td1;
        private PcmFile PCM2;
        private TableData td2;
        private uint selectedByte;
        private HexData[] hexDatas1;
        private HexData[] hexDatas2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref Point lParam);
        const int WM_USER = 0x400;
        const int EM_GETSCROLLPOS = WM_USER + 221;
        const int EM_SETSCROLLPOS = WM_USER + 222;
        private const string Red = "\\cf1 ";
        private const string Blue = "\\cf2 ";
        private const string Green = "\\cf3 ";
        private const string Black = "\\cf4 ";
        private const string Purple = "\\cf5 ";
        private const string LightGray = "\\cf6 ";

/*        private class HexData
        {
            public HexData()
            {
                //Hex = "";
                TableText = "";
                Prefix = " ";
                Suffix = " ";
                DataStart = false;
                Color = Blue;
            }
            //public string Hex { get; set; }
            public string TableText { get; set; }
            public string Prefix { get; set; }
            public bool DataStart { get; set; }
            public string Suffix { get; set; }
            public string Color { get; set; }
        }
*/
        private struct HexData
        {
            public string TableText;
            public string Prefix;
            public string Suffix;
            public string Color;
        }

        private void CreateHexData(PcmFile pcm, ref HexData[] hexDatas, TableData tData)
        {
            try
            {
                hexDatas = new HexData[pcm.fsize];                
/*                for (int a = 0; a < pcm.fsize; a++)
                {
                    HexData hd = new HexData();
                    //hd.hex = ToHexString(buf[a - start]);
                    hd.Hex = pcm.buf[a].ToString("X2");
                    hexDatas.Add(hd);
                }
*/
                for (int t = 0; t < pcm.tableDatas.Count; t++)
                {
                    TableData tmpTd = pcm.tableDatas[t];
                    int hexAddr = (int)tmpTd.StartAddress();
                    hexDatas[hexAddr].Prefix = "[";
                    if (tmpTd.Offset > 0 || tmpTd.ExtraOffset > 0)
                    {
                        hexDatas[hexAddr].TableText = tmpTd.TableName + ": " + tmpTd.Address + "+" + tmpTd.Offset.ToString("X") + "+" + tmpTd.ExtraOffset.ToString("X") + " - " + tmpTd.EndAddress().ToString("X8");
                    }
                    else
                    {
                        //hexDatas[hexAddr].prefix = tmpTd.TableName + ": " + tmpTd.Address + " - " + tmpTd.EndAddress().ToString("X8") + " [";
                        hexDatas[hexAddr].TableText = tmpTd.TableName + ": " + tmpTd.Address + " - " + tmpTd.EndAddress().ToString("X8");
                    }
                    hexAddr = (int)(tmpTd.StartAddress());
                    if (hexAddr > -1 && hexAddr < hexDatas.Length)
                        hexDatas[hexAddr].Prefix = "(";
                    hexAddr = (int)(tmpTd.EndAddress());
                    if (hexAddr > -1 && hexAddr < hexDatas.Length)
                        hexDatas[hexAddr].Suffix = ")";
                }
                hexDatas[(int)tData.StartAddress()].Prefix = "[";
                hexDatas[(int)tData.EndAddress()].Suffix = "]";
                int start = (int)tData.addrInt;
                if (tData.Offset < 0)
                    start += tData.Offset;
                if (tData.ExtraOffset < 0)
                    start += tData.ExtraOffset;
                for (int addr = start; addr <= tData.EndAddress(); addr++)
                {
                    hexDatas[addr].Color = Black;
                    int OffsetEnd = (int)(tData.addrInt + tData.Offset);
                    int ExtraOffsetEnd = (int)(tData.addrInt + tData.Offset + tData.ExtraOffset);

                    if (tData.Offset > 0 && addr >= tData.addrInt && addr < OffsetEnd)
                    {
                        hexDatas[addr].Color = Purple;
                    }
                    else if (tData.Offset < 0 && addr <= tData.addrInt && addr >= OffsetEnd)
                    {
                        hexDatas[addr].Color = Purple;
                    }
                    if (tData.ExtraOffset > 0 && addr >= OffsetEnd && addr < ExtraOffsetEnd)
                    {
                        hexDatas[addr].Color = Green;
                    }
                    else if (tData.ExtraOffset < 0 && addr <= OffsetEnd && addr >= ExtraOffsetEnd)
                    {
                        hexDatas[addr].Color = Green;
                    }

                    if (addr == selectedByte)
                    {
                        hexDatas[addr].Color = Red;
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
                LoggerBold("Error, CreateHexData, line " + line + ": " + ex.Message);
            }
        }

        private void RichTableData_VScroll(object sender, EventArgs e)
        {
            try
            {
                Point pt = new Point();
                SendMessage(richTableData.Handle, EM_GETSCROLLPOS, 0, ref pt);
                SendMessage(richTableData2.Handle, EM_SETSCROLLPOS, 0, ref pt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void RichTableData2_VScroll(object sender, EventArgs e)
        {
            try
            {
                Point pt = new Point();
                SendMessage(richTableData2.Handle, EM_GETSCROLLPOS, 0, ref pt);
                SendMessage(richTableData.Handle, EM_SETSCROLLPOS, 0, ref pt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void frmTableVis_Load(object sender, EventArgs e)
        {
        }

        public void ChangeSelection(uint selectedByte)
        {
            this.selectedByte = selectedByte;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            DisplayData(selectedByte,  true);
            DisplayData(selectedByte,  false);
        }

        public void DisplayData(uint selectedByte, bool Secondary)
        {
            try
            {
                PcmFile pcm = PCM1;
                RichTextBox rBox = richTableData;
                TableData tData = this.td1;
                HexData[] hexDatas = this.hexDatas1;
                this.selectedByte = selectedByte;

                int start = 0;
                int start1 = 0;
                int start2 = 0;
                int end = 0;
                int end1 = 0;
                int end2 = 0;
                int offset = (int)(td2.StartAddress() - td1.StartAddress());
                int segmentstart = 0;
                int segmentend = (int)pcm.fsize;
                int tableWithOffsetStart = 0;

                if (Secondary)
                {
                    rBox = richTableData2;
                    pcm = PCM2;
                    tData = this.td2;
                    labelFileName2.Text = pcm.FileName;
                    hexDatas = this.hexDatas2;

                }
                else
                {
                    labelFileName1.Text = pcm.FileName;
                }
                int seg = pcm.GetSegmentNumber(tData.addrInt);
                if (seg > -1)
                {
                    radioShowSegment.Text = "Segment [" + pcm.Segments[seg].Name + "]";
                    segmentstart = (int)pcm.segmentinfos[seg].GetStartAddr();
                    segmentend = (int)pcm.segmentinfos[seg].GetEndAddr();
                }


                if (radioShowTable.Checked)
                {
                    start1 = (int)td1.addrInt;
                    if (td1.Offset < 0)
                        start1 += td1.Offset;
                    if (td1.ExtraOffset < 0)
                        start1 += td1.ExtraOffset;

                    start2 = (int)td2.addrInt;
                    if (td2.Offset < 0)
                        start2 += td2.Offset;
                    if (td2.ExtraOffset < 0)
                        start2 += td2.ExtraOffset;

                    end1 = (int)td1.EndAddress();
                    end2 = (int)td2.EndAddress();

                    if (start1 < start2)
                        start = start1;
                    else
                        start = start2;

                    if (end1 > end2)
                        end = end1;
                    else
                        end = end2;

                    if (Secondary && offset > 0)
                        start += offset;
                    if (!Secondary && offset < 0)
                        start -= offset;

                    start = (int)(start - numExtraBytes.Value);
                    end = (int)(end + numExtraBytes.Value);

                }
                else
                {
                    start1 = (int)PCM1.segmentinfos[seg].GetStartAddr();
                    end1 = (int)PCM1.segmentinfos[seg].GetEndAddr();
                    start2 = (int)(PCM2.segmentinfos[seg].GetStartAddr());
                    end2 = (int)PCM2.segmentinfos[seg].GetEndAddr();

                    if (start1 < start2)
                        start = start1;
                    else
                        start = start2;

                    if (Secondary && offset > 0)
                        start += offset;
                    if (!Secondary && offset < 0)
                        start -= offset;

                    if (end1 > end2)
                        end = end1;
                    else
                        end = end2;

                }
                if (Secondary && offset < 0)
                    end += offset;
                if (!Secondary && offset > 0)
                    end -= offset;

                tableWithOffsetStart = (int)tData.addrInt;
                if (tData.Offset < 0)
                    tableWithOffsetStart += tData.Offset;
                if (tData.ExtraOffset < 0)
                    tableWithOffsetStart += tData.ExtraOffset;

                if (Secondary)
                {
                    Debug.WriteLine("Secondary: Start: " + start.ToString() +", end: " + end.ToString() +", Offset: " + offset.ToString());
                }
                else
                {
                    Debug.WriteLine("Primary: Start: " + start.ToString() + ", end: " + end.ToString());
                }
                Debug.WriteLine("Segment start: " + segmentstart + ", Segment End: " + segmentend);
                Debug.WriteLine("Addrint: " + tData.addrInt.ToString() + ", Table start: " + tData.StartAddress().ToString());

                rBox.Clear();

                rBox.SelectionColor = Color.Red;
                rBox.AppendText("REMOVETHIS" ); //cf1 = red
                rBox.SelectionColor = Color.Blue;
                rBox.AppendText("REMOVETHIS"); //cf2 = blue
                rBox.SelectionColor = Color.Green;
                rBox.AppendText("REMOVETHIS"); //cf3 = green
                rBox.SelectionColor = Color.Black;
                rBox.AppendText("REMOVETHIS" ); //cf4 = black
                rBox.SelectionColor = Color.Purple;
                rBox.AppendText("REMOVETHIS"); //cf5 = purple
                rBox.SelectionColor = Color.LightGray;
                rBox.AppendText("REMOVETHIS"); //cf6 = Aquamarine

                StringBuilder sb = new StringBuilder(rBox.Rtf.Replace("REMOVETHIS", ""));
                sb.Length -= 3;
                sb.Append(Environment.NewLine);
                string lastColor = Blue;
                sb.Append(Blue);

                int r = 1;
                int pos = 0;
                for (int addr = start; addr <= end && addr < pcm.fsize; addr++)
                {
                    if (addr < 0)
                    {
                        sb.Append("    ");
                        if (r >= numBytesPerRow.Value)
                        {
                            sb.Append("\\par" + Environment.NewLine);
                            r = 1;
                        }
                        r++;
                        continue;
                    }
                    if (string.IsNullOrEmpty(hexDatas[addr].Prefix))
                        hexDatas[addr].Prefix = " ";
                    if (string.IsNullOrEmpty(hexDatas[addr].Suffix))
                        hexDatas[addr].Suffix = " ";
                    if (string.IsNullOrEmpty(hexDatas[addr].Color))
                    {
                        if (addr < segmentstart || addr > segmentend)
                            hexDatas[addr].Color = LightGray;
                        else
                            hexDatas[addr].Color = Blue;
                    }

                    if (addr == tData.StartAddress())
                    {
                        //pos = sb.Length;
                        rBox.Rtf = sb.ToString();
                        pos = rBox.TextLength;
                    }
                    if (lastColor != hexDatas[addr].Color)
                    {
                        sb.Append(hexDatas[addr].Color);
                        lastColor = hexDatas[addr].Color;
                    }
                    if (radioSegmentTBNames.Checked)
                    {
                        if (addr == tableWithOffsetStart)
                        {
                            sb.Append("\\par" + Environment.NewLine + Black + hexDatas[tData.StartAddress()].TableText + "\\par" + Environment.NewLine + hexDatas[addr].Color);
                            r = 1;
                        }
                        else if (hexDatas[addr].Prefix == "(")
                        {
                            sb.Append("\\par" + Environment.NewLine + hexDatas[addr].TableText + "\\par" + Environment.NewLine);
                            r = 1;
                        }
                    }
                    if (r >= numBytesPerRow.Value)
                    {
                        sb.Append("\\par" + Environment.NewLine);
                        r = 1;
                    }

                    sb.Append(hexDatas[addr].Prefix + pcm.buf[addr].ToString("X2") + hexDatas[addr].Suffix);
                    r++;
                }

                sb.Append("\\par" + Environment.NewLine + "}");
                rBox.Rtf = sb.ToString();
                //int pos = rBox.Find("]");
                pos = rBox.Find("]",pos,RichTextBoxFinds.None);
                if (pos > -1)
                    rBox.Select(pos, 0);
            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, DisplayData, line " + line + ": " + ex.Message);
            }
        }

        private void radioShowSegment_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void radioShowBin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void numExtraBytes_ValueChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void numBytesPerRow_ValueChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void radioSegmentTBNames_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void radioShowTable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numExtraOffset1_ValueChanged(object sender, EventArgs e)
        {
            td1.ExtraOffset = (int)numExtraOffset1.Value;
            CreateHexData(PCM1, ref hexDatas1, td1);
            UpdateDisplay();
        }

        private void numExtraOffset2_ValueChanged(object sender, EventArgs e)
        {
            td2.ExtraOffset = (int)numExtraOffset2.Value;
            CreateHexData(PCM2, ref hexDatas2, td2);
            UpdateDisplay();
        }
    }
}
