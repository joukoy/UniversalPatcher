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
using System.Runtime.InteropServices;

namespace UniversalPatcher
{
    public partial class frmTableVisDouble : Form
    {
        public frmTableVisDouble()
        {
            InitializeComponent();
            richTableData1.MouseWheel += RichTableData_MouseWheel;
            richTableData2.SelectionChanged += RichTableData2_SelectionChanged;
            richTableData1.SelectionChanged += RichTableData1_SelectionChanged;
            richTableData1.SelectionChanged += RichTableData1_SelectionChangedMirror;
            richTableData2.SelectionChanged += RichTableData2_SelectionChangedMirror;
            if (td2 != null)
            {
                richTableData1.VScroll += RichTableData_VScroll;
                richTableData2.VScroll += RichTableData2_VScroll;
                richTableData2.EnableContextMenu();
            }
            colorStr = new string[colors.Length];
            int c = 7;
            for (int i=0;i<colors.Length;i++)
            {
                colorStr[i] = "\\cf" + c.ToString() + " ";
                c++;
            }
            richTableData1.EnableContextMenu();
            //ShowTables(PCM1, td1, PCM2, td2, selectedByte);
        }


        private struct HexData
        {
            public string TableText;
            public string Prefix;
            public string Suffix;
            public string Color;
            public int TdIndex;
        }


        private PcmFile PCM1;
        private TableData td1;
        private TableData td1Org;
        private PcmFile PCM2;
        private TableData td2;
        private TableData td2Org;
        private uint selectedByte;
        private HexData[] hexDatas1;
        private HexData[] hexDatas2;
        private int pos1;
        private int pos2;
        public FrmTuner tuner;
        private List<TableData> SortedTds1;
        private List<TableData> SortedTds2;
        private Dictionary<int, int> SelectionToAddress1;
        private Dictionary<int, int> SelectionToAddress2;
        private int StartPoint1;
        private int EndPoint1;
        private int StartPoint2;
        private int EndPoint2;
        private int offset = 0;
        private int ScrollPos1 = -1;
        private int ScrollPos2 = -1;

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref Point lParam);

        [DllImport("user32")]
        static extern int GetScrollInfo(IntPtr hwnd, int nBar, ref SCROLLINFO scrollInfo);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int min;
            public int max;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        const int WM_USER = 0x400;
        const int EM_GETSCROLLPOS = WM_USER + 221;
        const int EM_SETSCROLLPOS = WM_USER + 222;
        const int EM_SCROLL = 0xB5;
        const int SB_LINEDOWN = 1;
        const int SB_LINEUP = 0;
        private uint WM_VSCROLL = 0x115;
        private const uint SB_PAGEUP = 2;
        private const uint SB_PAGEDOWN = 3;
        private const uint SB_TOP = 6;
        private const uint SB_BOTTOM = 7;

        private const string Red = "\\cf1 ";
        private const string Blue = "\\cf2 ";
        private const string Green = "\\cf3 ";
        private const string Black = "\\cf4 ";
        private const string Purple = "\\cf5 ";
        private const string LightGray = "\\cf6 ";


        private Color[] colors =
        {
                        //Color.FromArgb(255, 255, 192, 192), //Pink?
                        //Color.FromArgb(255, 255, 224, 192),
                        //Color.FromArgb(255, 255, 255, 192),
                        //Color.FromArgb(255, 192, 255, 192),
                        //Color.FromArgb(255, 192, 255, 255),
                        Color.FromArgb(255, 192, 192, 255),
                        Color.FromArgb(255, 255, 192, 255),
                        //Color.FromArgb(255, 224, 224, 224),
                        Color.FromArgb(255, 255, 128, 128),
                        Color.FromArgb(255, 255, 192, 128),
                        //Color.FromArgb(255, 255, 255, 128),
                        //Color.FromArgb(255, 128, 255, 128),
                        //Color.FromArgb(255, 128, 255, 255),
                        Color.FromArgb(255, 128, 128, 255),
                        Color.FromArgb(255, 255, 128, 255),
                        Color.Silver,
                        //Color.Red,
                        Color.FromArgb(255, 255, 128, 0),
                        //Color.Yellow,
                        //Color.Lime,
                        //Color.Cyan,
                        //Color.Blue,
                        Color.Fuchsia,
                        Color.Gray,
                        //Color.FromArgb(255, 192, 0, 0),
                        //Color.FromArgb(255, 192, 64, 0),
                        Color.FromArgb(255, 192, 192, 0),
                        Color.FromArgb(255, 0, 192, 0),
                        Color.FromArgb(255, 0, 192, 192),
                        Color.FromArgb(255, 0, 0, 192),
                        Color.FromArgb(255, 192, 0, 192),
                        Color.FromArgb(255, 64, 64, 64),
                        Color.Maroon,
                        Color.FromArgb(255, 128, 64, 0),
                        Color.Olive,
                        //Color.Green,
                        Color.Teal,
                        Color.Navy,
                        //Color.Purple,
                        //Color.Black,
                        //Color.FromArgb(255, 64, 0, 0),
                        Color.FromArgb(255, 128, 64, 64),
                        //Color.FromArgb(255, 64, 64, 0),
                        Color.FromArgb(255, 0, 64, 0),
                        //Color.FromArgb(255, 0, 64, 64),
                        //Color.FromArgb(255, 0, 0, 64),
                        //Color.FromArgb(255, 64, 0, 64),
                    };

        private string[] colorStr;

        private void RichTableData_MouseWheel(object sender, MouseEventArgs e)
        {
            if (radioSegmentTBNames.Checked)
            {
                return;
            }
            if (e.Delta < 0)
            {
                SendMessage(richTableData2.Handle, WM_VSCROLL, (IntPtr)SB_LINEDOWN, IntPtr.Zero);
            }
            else
            {
                SendMessage(richTableData2.Handle, WM_VSCROLL, (IntPtr)SB_LINEUP, IntPtr.Zero);
            }
        }

        private void VScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Point pt = new Point();
            pt.Y = e.NewValue;
            SendMessage(richTableData1.Handle, EM_SETSCROLLPOS, 0, ref pt);
            SendMessage(richTableData2.Handle, EM_SETSCROLLPOS, 0, ref pt);
        }

        public void ShowTables(PcmFile PCM1, TableData td1, PcmFile PCM2, TableData td2, uint SelectedByte)
        {
            this.Text = "Table data visualizer [" + td1.TableName + "]";
            this.PCM1 = PCM1;
            this.td1 = td1.ShallowCopy(false);
            this.td1Org = td1;
            this.PCM2 = PCM2;
            this.td2Org = td2;
            numExtraOffset1.Value = td1.ExtraOffset;
            this.selectedByte = SelectedByte;
            SortedTds1  = PCM1.tableDatas.OrderBy(x => x.StartAddress()).ToList();

            CreateHexData(PCM1, ref hexDatas1, this.td1, SortedTds1);
            if (td2 != null)
            {
                SortedTds2 = PCM2.tableDatas.OrderBy(x => x.StartAddress()).ToList();
                this.td2 = td2.ShallowCopy(false);
                numExtraOffset2.Value = td2.ExtraOffset;
                CreateHexData(PCM2, ref hexDatas2, td2, SortedTds2);
                numExtraOffset2.ValueChanged += numExtraOffset2_ValueChanged;
            }
            numExtraOffset1.ValueChanged += numExtraOffset1_ValueChanged;
            UpdateDisplay();
        }

        private List<TableData> GetSelectedTables(bool Secondary)
        {
            int Start = -1;
            int End = -1;
            RichTextBox richBox = richTableData1;
            Dictionary<int, int> SelectionToAddr = SelectionToAddress1;
            TextBox txtInfo = txtInfo1;
            List<TableData> SortedTds = SortedTds1;
            List<TableData> selTables = new List<TableData>();
            if (Secondary)
            {
                richBox = richTableData2;
                SelectionToAddr = SelectionToAddress2;
                txtInfo = txtInfo2;
                SortedTds = SortedTds2;
            }

            for (int s = richBox.SelectionStart; s < (richBox.SelectionStart + richBox.SelectionLength); s++)
            {
                if (SelectionToAddr.ContainsKey(s))
                {
                    Start = SelectionToAddr[s];
                    break;
                }
            }
            for (int s = richBox.SelectionStart + richBox.SelectionLength; s >= richBox.SelectionStart; s--)
            {
                if (SelectionToAddr.ContainsKey(s))
                {
                    End = SelectionToAddr[s];
                    break;
                }
            }

            if (Start < 0 || End < 0)
            {
                txtInfo.Text = "";
                return selTables;
            }
            txtInfo.Text = "Selection range: " + Start.ToString("X") + " - " + End.ToString("X") + Environment.NewLine;
            txtInfo.AppendText("Tables: ");

            int TStart = 0;
            int TEnd = 0;
            for (int t = 0; t < SortedTds.Count; t++)
            {
                if (SortedTds[t].StartAddress() >= Start)
                {
                    TStart = t;
                    break;
                }
            }
            for (int t = TStart; t < SortedTds.Count; t++)
            {
                if (SortedTds[t].EndAddress() > End)
                {
                    break;
                }
                else
                {
                    TEnd = t;
                }
            }
            txtInfo.AppendText(Environment.NewLine);
            for (int t = TStart; t <= TEnd; t++)
            {
                txtInfo.AppendText(SortedTds[t].TableName + " [" + SortedTds[t].StartAddress().ToString("X") + "] ("
                    + SortedTds[t].addrInt.ToString("X") + " + " + SortedTds[t].Offset.ToString()
                    + " + " + SortedTds[t].ExtraOffset.ToString() + ")" + Environment.NewLine);
                selTables.Add(SortedTds[t]);
            }
            return selTables;
        }

        private void RichTableData2_SelectionChangedMirror(object sender, EventArgs e)
        {
            if (!radioSegmentTBNames.Checked)
            {
                richTableData1.SelectionChanged -= RichTableData1_SelectionChangedMirror;
                richTableData1.Select(richTableData2.SelectionStart, richTableData2.SelectionLength);
                richTableData1.SelectionChanged += RichTableData1_SelectionChangedMirror;
            }
        }

        private void RichTableData1_SelectionChangedMirror(object sender, EventArgs e)
        {
            if (td2 != null && !radioSegmentTBNames.Checked)
            {
                richTableData2.SelectionChanged -= RichTableData2_SelectionChangedMirror;
                richTableData2.Select(richTableData1.SelectionStart, richTableData1.SelectionLength);
                richTableData2.SelectionChanged += RichTableData2_SelectionChangedMirror;
            }
        }

        private void RichTableData2_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectionToAddress2 == null)
                return;

            if (richTableData2.SelectionStart == 0 && richTableData2.SelectionLength == richTableData2.TextLength)
                return;
            if (richTableData1.SelectionStart == 0 && richTableData1.SelectionLength == richTableData1.TextLength)
                return;

            GetSelectedTables(true);
        }

        private void RichTableData1_SelectionChanged(object sender, EventArgs e)
        {

            if (SelectionToAddress1 == null)
                return;

            if (richTableData1.SelectionStart == 0 && richTableData1.SelectionLength == richTableData1.TextLength)
                return;
            if (richTableData2.SelectionStart == 0 && richTableData2.SelectionLength == richTableData2.TextLength)
                return;
            GetSelectedTables(false);
        }

        public void ScrollTo(IntPtr hWnd, int Position)
        {
            SetScrollPos(hWnd, 0x1, Position, true);
            PostMessage(hWnd, 0x115, 4 + 0x10000 * Position, 0);
        }

        public int GetRtbPos(RichTextBox rtb)
        {
            SCROLLINFO scrollInfo = new SCROLLINFO();
            scrollInfo.cbSize = Marshal.SizeOf(scrollInfo);
            //SIF_RANGE = 0x1, SIF_TRACKPOS = 0x10,  SIF_PAGE= 0x2
            scrollInfo.fMask = 0x10 | 0x1 | 0x2;
            GetScrollInfo(rtb.Handle, 1, ref scrollInfo);//nBar = 1 -> VScrollbar
            return scrollInfo.nTrackPos + scrollInfo.nPage;
        }

        private void CreateHexData(PcmFile pcm, ref HexData[] hexDatas, TableData tData, List<TableData> TableDatas)
        {
            try
            {
                hexDatas = new HexData[pcm.fsize];


                int c = 0;
                int segNr = pcm.GetSegmentNumber(tData.addrInt);

                for (int t = 0; t < TableDatas.Count; t++)
                {
                    TableData tmpTd = TableDatas[t];
                    if (tmpTd.guid == tData.guid)
                    {
                        tmpTd.ExtraOffset = tData.ExtraOffset;
                    }
                    if (segNr == pcm.GetSegmentNumber(tmpTd.addrInt))
                    {
                        int hexAddr = (int)tmpTd.StartAddress();
                        if (hexAddr > (hexDatas.Length - 1) || hexAddr < 0)
                        {
                            Debug.WriteLine("Bad table address: " + hexAddr.ToString("X"));
                            continue;
                        }
/*                        if (tmpTd.Offset < 0)
                            hexAddr += tmpTd.Offset;
                        if (tmpTd.ExtraOffset < 0)
                            hexAddr += tmpTd.ExtraOffset;
*/                        //hexDatas[hexAddr].Prefix = "[";
                        hexDatas[hexAddr].TableText = tmpTd.TableName + ": " + tmpTd.Address;
                        if (tmpTd.Offset >= 0)
                        {
                            hexDatas[hexAddr].TableText += "+" + tmpTd.Offset.ToString();
                        }
                        else
                        {
                            hexDatas[hexAddr].TableText += "-" + (-1 * tmpTd.Offset).ToString();
                        }
                        if (tmpTd.ExtraOffset >= 0)
                        {
                            hexDatas[hexAddr].TableText += "+" + tmpTd.ExtraOffset.ToString() ;
                        }
                        else
                        {
                            hexDatas[hexAddr].TableText += "-" + (-1 * tmpTd.ExtraOffset).ToString();
                        }
                        hexDatas[hexAddr].TableText += " - " + tmpTd.EndAddress().ToString("X8");
                        //hexDatas[hexAddr].TableText = hexDatas[hexAddr].TableText.PadRight((int)(numBytesPerRow.Value*4));

                        hexAddr = (int)(tmpTd.StartAddress());
                        if (hexAddr > -1 && hexAddr < hexDatas.Length)
                            hexDatas[hexAddr].Prefix = "(";
                        hexAddr = (int)(tmpTd.EndAddress());
                        if (hexAddr > -1 && hexAddr < hexDatas.Length)
                            hexDatas[hexAddr].Suffix = ")";
                        for (int a = (int)tmpTd.StartAddress(); a <= tmpTd.EndAddress() && a<hexDatas.Length; a++)
                        {
                            hexDatas[a].Color = colorStr[c];
                            hexDatas[a].TdIndex = t;
                        }
                        c++;
                        if (c >= (colors.Length - 1))
                            c = 0;
                    }
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
                    int OffsetEnd = (int)(tData.addrInt + tData.Offset);
                    int ExtraOffsetEnd = (int)(tData.addrInt + tData.Offset + tData.ExtraOffset);

                    if (addr >= tData.StartAddress() && addr <= tData.EndAddress())
                    {
                        hexDatas[addr].Color = Black;
                    }
                    else if (tData.Offset > 0 && addr >= tData.addrInt && addr < OffsetEnd)
                    {
                        hexDatas[addr].Color = Purple;
                    }
                    else if (tData.Offset < 0 && addr <= tData.addrInt && addr >= OffsetEnd)
                    {
                        hexDatas[addr].Color = Purple;
                    }
                    else if (tData.ExtraOffset > 0 && addr >= OffsetEnd && addr < ExtraOffsetEnd)
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
                
                //richTableData2.VScroll -= RichTableData2_VScroll;
/*                Point pt = new Point();
                SendMessage(richTableData.Handle, EM_GETSCROLLPOS, 0, ref pt);
                SendMessage(richTableData2.Handle, EM_SETSCROLLPOS, 0, ref pt);
                ScrollTo(richTableData2.Handle, pt.Y);
*/            
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
                if (radioSegmentTBNames.Checked)
                {
                    return;
                }
                Point pt = new Point();
                SendMessage(richTableData2.Handle, EM_GETSCROLLPOS, 0, ref pt);
                //Debug.WriteLine("Rich2 pos: " + pt.Y.ToString());
                SendMessage(richTableData1.Handle, EM_SETSCROLLPOS, 0, ref pt);
                ScrollTo(richTableData1.Handle, pt.Y);
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

        private int DetermineVMax(RichTextBox RTB)
        {
            // Y position of the first character
            int y1 = RTB.GetPositionFromCharIndex(0).Y;

            // Y position of the last character
            int y2 = RTB.GetPositionFromCharIndex(RTB.TextLength).Y;

            // First option: The height of the line - returns 43
            double heightLine = Math.Ceiling(RTB.Font.GetHeight());

            // Second option: The height of the line - returns 45
            // NOTE: GetFirstCharIndexFromLine( NUMBER )
            // 0 is the first line, 1 is the second line, etc.
            //int index = targetCtrl.GetFirstCharIndexFromLine(1);
            //int heightLine = targetCtrl.GetPositionFromCharIndex(index).Y;

            // Absolute difference between the position of the 1st and the last characters
            int absoluteDifference = Math.Abs(y1 - y2);

            // RichTextBox height
            int heightRtb = RTB.ClientSize.Height;

            // Maximum vertical scroll value
            // NOTE: if you don't add a line height, the RTB content will not be displayed in full
            int max = absoluteDifference - heightRtb + (int)heightLine;
            return max;
        }

        public void UpdateDisplay()
        {
            int sStart1 = richTableData1.SelectionStart;
            int sLen1 = richTableData1.SelectionLength;
            int sStart2 = richTableData2.SelectionStart;
            int sLen2 = richTableData2.SelectionLength;
            Point pt1 = new Point();
            Point pt2 = new Point();
            SendMessage(richTableData2.Handle, EM_GETSCROLLPOS, 0, ref pt2);
            SendMessage(richTableData1.Handle, EM_GETSCROLLPOS, 0, ref pt1);
            if (td2 != null)
            {                
                DisplayData(selectedByte, true);
                if (sLen2 > 0)
                {
                    richTableData2.Select(sStart2, sLen2);
                    SendMessage(richTableData2.Handle, EM_SETSCROLLPOS, 0, ref pt2);
                    ScrollTo(richTableData2.Handle, pt2.Y);
                }
                else
                {
                    richTableData2.Select(richTableData2.TextLength, 0);
                    richTableData2.Select(pos2, 0);
                }
                if (radioSegmentTBNames.Checked)
                {
                    richTableData1.ScrollBars = RichTextBoxScrollBars.Vertical;
                }
                else
                {
                    richTableData1.ScrollBars = RichTextBoxScrollBars.None;
                }
            }
            else
            {
                splitContainer1.Panel2.Hide();
                splitContainer1.SplitterDistance = splitContainer1.Width - 5;
                richTableData1.ScrollBars = RichTextBoxScrollBars.Both;
            }
            DisplayData(selectedByte,  false);
            if (sLen1 > 0)
            {
                richTableData1.Select(sStart1, sLen1);
                SendMessage(richTableData1.Handle, EM_SETSCROLLPOS, 0, ref pt1);
                ScrollTo(richTableData1.Handle, pt1.Y);
            }
            else
            {
                richTableData1.Select(richTableData1.TextLength, 0);
                richTableData1.Select(pos1, 0);
            }
            Application.DoEvents();
            SendMessage(richTableData1.Handle, EM_GETSCROLLPOS, 0, ref pt1);
            ScrollPos1 = pt1.Y;
            SendMessage(richTableData2.Handle, EM_GETSCROLLPOS, 0, ref pt2);
            ScrollPos2 = pt2.Y;
        }

        public void DisplayData(uint selectedByte, bool Secondary)
        {
            try
            {
                PcmFile pcm = PCM1;
                RichTextBox rBox = richTableData1;
                TableData tData = this.td1;
                HexData[] hexDatas = this.hexDatas1;
                this.selectedByte = selectedByte;
                StartPoint1 = -1;
                EndPoint1 = -1;
                StartPoint2 = -1;
                EndPoint2 = -1;

                int start = 0;
                int start1 = 0;
                int start2 = 0;
                int end = 0;
                int end1 = 0;
                int end2 = 0;
                int segmentstart = 0;
                int segmentend = (int)pcm.fsize;

                if (td2 != null)
                {
                    offset = (int)(td2.StartAddress() - td1.StartAddress());
                }
                if (Secondary)
                {
                    rBox = richTableData2;
                    pcm = PCM2;
                    tData = this.td2;
                    labelFileName2.Text = pcm.FileName;
                    hexDatas = this.hexDatas2;
                    SelectionToAddress2 = new Dictionary<int, int>();
                }
                else
                {
                    labelFileName1.Text = pcm.FileName;
                    SelectionToAddress1 = new Dictionary<int, int>();
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
                    end1 = (int)td1.EndAddress();

                    if (td2 != null)
                    {
                        start2 = (int)td2.addrInt;
                        if (td2.Offset < 0)
                            start2 += td2.Offset;
                        if (td2.ExtraOffset < 0)
                            start2 += td2.ExtraOffset;
                        end2 = (int)td2.EndAddress();
                    }

                    if (start1 < start2 || td2 == null)
                        start = start1;
                    else
                        start = start2;

                    if (end1 > end2 || td2 == null)
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
                    if (td2 != null)
                    {
                        start2 = (int)(PCM2.segmentinfos[seg].GetStartAddr());
                        end2 = (int)PCM2.segmentinfos[seg].GetEndAddr();
                    }

                    if (radioSegmentTBNames.Checked)
                    {
                        if (Secondary)
                        {
                            start = start2;
                            end = end2;
                        }
                        else
                        {
                            start = start1;
                            end = end1;
                        }
                    }
                    else
                    {
                        if (start1 < start2 || td2 == null)
                            start = start1;
                        else
                            start = start2;

                        if (Secondary && offset > 0)
                            start += offset;
                        if (!Secondary && offset < 0)
                            start -= offset;

                        if (end1 > end2 || td2 == null)
                            end = end1;
                        else
                            end = end2;
                    }
                }
                if (Secondary && offset < 0)
                    end += offset;
                if (!Secondary && offset > 0)
                    end -= offset;
                int tblStartAddress = (int)tData.StartAddress();

                if (Secondary)
                {
                    Debug.WriteLine("Secondary: Start: " + start.ToString() +", end: " + end.ToString() +", Offset: " + offset.ToString());
                }
                else
                {
                    Debug.WriteLine("Primary: Start: " + start.ToString() + ", end: " + end.ToString());
                }
                Debug.WriteLine("Segment start: " + segmentstart + ", Segment End: " + segmentend);
                Debug.WriteLine("Addrint: " + tData.addrInt.ToString() + ", Table start: " + tblStartAddress.ToString());

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
                rBox.AppendText("REMOVETHIS"); //cf6 = LightGray
                for (int i=0;i<colors.Length;i++)
                {
                    rBox.SelectionColor = colors[i];
                    rBox.AppendText("REMOVETHIS"); 
                }

                int txtLen = 2;
                StringBuilder sb = new StringBuilder(rBox.Rtf.Replace("REMOVETHIS", ""));
                sb.Length -= 3;
                sb.Append(Environment.NewLine);
                string lastColor = "";
                //sb.Append(Blue);

                int r = 1;
                for (int addr = start; addr <= end && addr < pcm.fsize; addr++)
                {
                    if (addr < 0)
                    {
                        sb.Append("    ");
                        txtLen += 4;
                        if (r > numBytesPerRow.Value)
                        {
                            sb.Append("\\par" + Environment.NewLine);
                            txtLen++;
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

                    if (addr == tblStartAddress)
                    {
                        if (Secondary)
                            pos2 = txtLen;
                        else
                            pos1 = txtLen;
                    }

                    if (lastColor != hexDatas[addr].Color)
                    {
                        sb.Append(hexDatas[addr].Color);
                        lastColor = hexDatas[addr].Color;
                    }
                    if (radioSegmentTBNames.Checked)
                    {
                        //if (hexDatas[addr].Prefix == "(" || addr == TblStartAddress)
                        if (!string.IsNullOrEmpty(hexDatas[addr].TableText))
                        {
                            sb.Append("\\par" + Environment.NewLine + hexDatas[addr].TableText + "\\par" + Environment.NewLine);
                            txtLen += hexDatas[addr].TableText.Length + 2;
                            r = 1;
                        }
                    }
                    if (r > numBytesPerRow.Value)
                    {
                        sb.Append("\\par" + Environment.NewLine);
                        txtLen++;
                        r = 1;
                    }

                    if (Secondary)
                        SelectionToAddress2.Add(txtLen, addr);
                    else
                        SelectionToAddress1.Add(txtLen, addr);
                    sb.Append(hexDatas[addr].Prefix + pcm.buf[addr].ToString("X2") + hexDatas[addr].Suffix);
                    txtLen += 4;
                    r++;
                }

                sb.Append("\\par" + Environment.NewLine + "}");
                rBox.Rtf = sb.ToString();
            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, DisplayData, line " + line + ": " + ex.Message);
            }
        }

        private void radioShowSegment_CheckedChanged(object sender, EventArgs e)
        {
            richTableData1.SelectionLength = 0;
            richTableData2.SelectionLength = 0;
            if (radioShowSegment.Checked)
            {
                UpdateDisplay();
            }
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
            richTableData1.SelectionLength = 0;
            richTableData2.SelectionLength = 0;
            if (radioSegmentTBNames.Checked)
            {
                UpdateDisplay();
            }
        }

        private void radioShowTable_CheckedChanged(object sender, EventArgs e)
        {
            richTableData1.SelectionLength = 0;
            richTableData2.SelectionLength = 0;
            if (radioShowTable.Checked)
            {
                UpdateDisplay();
            }
        }

        private void numExtraOffset1_ValueChanged(object sender, EventArgs e)
        {
            td1.ExtraOffset = (int)numExtraOffset1.Value;
            CreateHexData(PCM1, ref hexDatas1, td1, SortedTds1);
            UpdateDisplay();
        }

        private void numExtraOffset2_ValueChanged(object sender, EventArgs e)
        {
            td2.ExtraOffset = (int)numExtraOffset2.Value;
            CreateHexData(PCM2, ref hexDatas2, td2, SortedTds2);
            UpdateDisplay();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnColorTest_Click(object sender, EventArgs e)
        {
            richTableData1.Text = "";
            for (int c=0; c<colors.Length;c++)
            {
                richTableData1.SelectionColor = colors[c];
                richTableData1.AppendText("Color: " + c.ToString() + "; " + colors[c].ToString() + Environment.NewLine);
            }
        }

        private void btnApplyPrimary_Click(object sender, EventArgs e)
        {
            td1Org.ExtraOffset = (int)numExtraOffset1.Value;
            if (tuner != null)
            {
                //tuner.RefreshTablelist();
                tuner.RefreshFast();
            }
        }

        private void btnApplySecondary_Click(object sender, EventArgs e)
        {
            td2Org.ExtraOffset = (int)numExtraOffset2.Value;
            if (tuner != null)
            {
                //tuner.RefreshTablelist();
                tuner.RefreshFast();
            }

        }

        private void btnPrevTable_Click(object sender, EventArgs e)
        {
            int x = FindTableDataId(td1, PCM1.tableDatas);
            x--;
            if (x >-1)
            {
                TableData tdTmp = PCM1.tableDatas[x];
                TableData compTd = null;
                if (td2 != null)
                {
                    compTd = FindTableData(tdTmp, PCM2.tableDatas);
                }
                ShowTables(PCM1, tdTmp, PCM2, compTd, selectedByte);
            }
        }

        private void btnNextTable_Click(object sender, EventArgs e)
        {
            int x = FindTableDataId(td1, PCM1.tableDatas);
            x++;
            if (x > -1)
            {
                TableData tdTmp = PCM1.tableDatas[x];
                TableData compTd = null;
                if (td2 != null)
                {
                    compTd = FindTableData(tdTmp, PCM2.tableDatas);
                }
                ShowTables(PCM1, tdTmp, PCM2, compTd, selectedByte);
            }

        }


        private void btnApplyToSelection1_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> selTables = GetSelectedTables(false);
                for (int t = 0; t < selTables.Count; t++)
                {
                    Logger("Updating table: " + selTables[t].TableName);
                    selTables[t].ExtraOffset = (int)numExtraOffset1.Value;
                }
                //CreateHexData(PCM1, ref hexDatas1, td1, SortedTds1);
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                ShowTables(PCM1,td1Org,PCM2,td2Org,selectedByte);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void btnApplyToSelection2_Click(object sender, EventArgs e)
        {
            try
            {

                List<TableData> selTables = GetSelectedTables(true);
                for (int t = 0; t < selTables.Count; t++)
                {
                    Logger("Updating table: " + selTables[t].TableName);
                    selTables[t].ExtraOffset = (int)numExtraOffset2.Value;
                }
                //CreateHexData(PCM2, ref hexDatas2, td2, SortedTds2);
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                ShowTables(PCM1, td1Org, PCM2, td2Org, selectedByte);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnSelStart1_Click(object sender, EventArgs e)
        {
            StartPoint1 = richTableData1.SelectionStart;
            if (EndPoint1 > -1)
            {
                richTableData1.Select(StartPoint1, EndPoint1 - StartPoint1);
            }
        }

        private void btnSelEnd1_Click(object sender, EventArgs e)
        {
            EndPoint1 = richTableData1.SelectionStart;
            if (StartPoint1 > -1)
            {
                richTableData1.Select(StartPoint1, EndPoint1 - StartPoint1);
            }
        }

        private void btnSelStart2_Click(object sender, EventArgs e)
        {
            StartPoint2 = richTableData2.SelectionStart;
            if (EndPoint2 > -1 )
            {
                richTableData2.Select(StartPoint2, EndPoint2 - StartPoint2);
            }
        }

        private void btnSelEnd2_Click(object sender, EventArgs e)
        {
            EndPoint2 = richTableData2.SelectionStart;
            if (StartPoint2 > -1 )
            {
                richTableData1.Select(StartPoint2, EndPoint2 - StartPoint2);
            }
        }

        private void btnApplytoRight_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> selTables = GetSelectedTables(false);
                for (int t = 0; t < selTables.Count; t++)
                {
                    TableData tdR = FindTableData(selTables[t], SortedTds2);
                    if (tdR != null)
                    {
                        Logger("Applying offset: " + offset.ToString() + " to table: " + tdR.TableName);
                        tdR.ExtraOffset = offset;
                    }
                }
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                ShowTables(PCM1, td1Org, PCM2, td2Org, selectedByte);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, Applytoright, line " + line + ": " + ex.Message);

            }
        }

        private void btnApplytoLeft_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> selTables = GetSelectedTables(true);
                for (int t = 0; t < selTables.Count; t++)
                {
                    TableData tdL = FindTableData(selTables[t], SortedTds1);
                    if (tdL != null)
                    {
                        Logger("Applying offset: " + (-1 * offset).ToString() + " to table: " + tdL.TableName);
                        tdL.ExtraOffset = -1 * offset;
                    }
                }
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                ShowTables(PCM1, td1Org, PCM2, td2Org, selectedByte);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, Applytoleft, line " + line + ": " + ex.Message);

            }

        }

        private void btnCloneColors_Click(object sender, EventArgs e)
        {
            for (int i=0;i<hexDatas1.Length;i++)
            {
                if ((i+offset) > 0 && (i+offset)< PCM2.fsize)
                    hexDatas2[i+offset] = hexDatas1[i];
            }
            UpdateDisplay();
        }

        private void btnCloneColors2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < hexDatas2.Length; i++)
            {
                if ((i - offset) > 0 && (i - offset) < PCM1.fsize)
                    hexDatas1[i - offset] = hexDatas2[i];
            }
            UpdateDisplay();

        }

        private void btnScrollToSelected_Click(object sender, EventArgs e)
        {
            if (radioSegmentTBNames.Checked)
            {
                Point pt1 = new Point(0, ScrollPos1);
                SendMessage(richTableData1.Handle, EM_SETSCROLLPOS, 0, ref pt1);
                ScrollTo(richTableData1.Handle, ScrollPos1);
            }
            else
            {
                Point pt2 = new Point(0, ScrollPos2);
                SendMessage(richTableData2.Handle, EM_SETSCROLLPOS, 0, ref pt2);
                ScrollTo(richTableData2.Handle, ScrollPos2);
            }
        }

        private void btnScrollToSelected2_Click(object sender, EventArgs e)
        {
            Point pt2 = new Point(0, ScrollPos2);
            SendMessage(richTableData2.Handle, EM_SETSCROLLPOS, 0, ref pt2);
            ScrollTo(richTableData2.Handle, ScrollPos2);
        }

        private void btnCreateTable1_Click(object sender, EventArgs e)
        {
            int Start = -1;
            int End = -1;
            for (int s = richTableData1.SelectionStart; s < (richTableData1.SelectionStart + richTableData1.SelectionLength); s++)
            {
                if (SelectionToAddress1.ContainsKey(s))
                {
                    Start = SelectionToAddress1[s];
                    break;
                }
            }
            if (Start < 0)
            {
                return;
            }
            for (int s = richTableData1.SelectionStart + richTableData1.SelectionLength; s >= richTableData1.SelectionStart; s--)
            {
                if (SelectionToAddress1.ContainsKey(s))
                {
                    End = SelectionToAddress1[s];
                    break;
                }
            }

            TableData newTd = new TableData();
            newTd.addrInt = (uint)Start;
            if (End - Start > 0)
                newTd.Rows = (ushort)(End - Start);
            else
                newTd.Rows = 1;
            newTd.Columns = 1;
            frmTdEditor fte = new frmTdEditor();
            fte.td = newTd;
            fte.LoadTd();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                PCM1.tableDatas.Add(newTd);
                ShowTables(PCM1, td1, PCM2, td2, selectedByte);
            }
            fte.Dispose();
        }

        private void btnCreateTable2_Click(object sender, EventArgs e)
        {
            int Start = -1;
            int End = -1;
            for (int s = richTableData2.SelectionStart; s < (richTableData2.SelectionStart + richTableData2.SelectionLength); s++)
            {
                if (SelectionToAddress2.ContainsKey(s))
                {
                    Start = SelectionToAddress2[s];
                    break;
                }
            }
            if (Start < 0)
            {
                return;
            }
            for (int s = richTableData2.SelectionStart + richTableData2.SelectionLength; s >= richTableData2.SelectionStart; s--)
            {
                if (SelectionToAddress2.ContainsKey(s))
                {
                    End = SelectionToAddress2[s];
                    break;
                }
            }

            TableData newTd = new TableData();
            newTd.addrInt = (uint)Start;
            if (End - Start > 0)
                newTd.Rows = (ushort)(End - Start);
            else
                newTd.Rows = 1;
            newTd.Columns = 1;
            frmTdEditor fte = new frmTdEditor();
            fte.td = newTd;
            fte.LoadTd();
            if (fte.ShowDialog() == DialogResult.OK)
            {
                PCM2.tableDatas.Add(newTd);
                ShowTables(PCM1, td1, PCM2, td2, selectedByte);
            }
            fte.Dispose();

        }
    }
}
