// frmTableVisDouble.cs  —  MUOKATTU VERSIO
// DataGridView1 / DataGridView2  →  HexViewControl hexView1 / hexView2
//
// Kaikki DataGridView-spesifiset API-kutsut on korvattu HexViewControl-vastaavilla.
// VisSettings.mouseDownCell / mouseUpCell (DataGridViewCell) jätetty paikoilleen
// (niitä ei enää tarvita, mutta kommentoitu pois rauhallisesti).

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
using static UniversalPatcher.ExtensionMethods;

namespace UniversalPatcher
{
    public partial class frmTableVisDouble2 : Form
    {
        public frmTableVisDouble2(PcmFile PCM1, PcmFile PCM2, TableData td1, TableData td2)
        {
            InitializeComponent();
            SetupHexViews();
            vis1 = new VisSettings(PCM1, td1, true);
            if (PCM2 != null)
            {
                vis2 = new VisSettings(PCM2, td2, false);
            }
        }

        // ─── VisSettings ─────────────────────────────────────────────────────────

        public class VisSettings
        {
            public VisSettings() { }
            public VisSettings(PcmFile PCM, TableData td, bool Primary)
            {
                this.Primary = Primary;
                SortedTds = PCM.tableDatas.OrderBy(x => x.StartAddress()).ToList();
                ChangeTd(PCM, td);
            }
            public void ChangeTd(PcmFile PCM, TableData td)
            {
                this.PCM = PCM;
                if (td != null)
                {
                    this.td    = td.ShallowCopy(false);
                    this.tdOrg = td;
                    int seg = PCM.GetSegmentNumber(td.addrInt);
                    if (seg > -1 && seg != segmentNumber)
                    {
                        segmentNumber = seg;
                        segmentName   = PCM.Segments[seg].Name;
                        segmentstart  = PCM.segmentinfos[seg].GetStartAddr();
                        segmentend    = PCM.segmentinfos[seg].GetEndAddr();
                    }
                }
            }
            public void FilterSegmentTables()
            {
                SegmentTds = new List<TableData>();
                for (int t = 0; t < SortedTds.Count; t++)
                {
                    TableData tmpTd = SortedTds[t];
                    if (segmentNumber == PCM.GetSegmentNumber((uint)(tmpTd.StartAddress())))
                        SegmentTds.Add(tmpTd);
                }
            }
            public PcmFile        PCM;
            public TableData      td       { get; internal set; }
            public TableData      tdOrg;
            public HexData[]      hexDatas;
            public int            buffOffset;
            public int            ExtraOffset;
            public List<TableData> SortedTds;
            public List<TableData> SegmentTds;
            public int            SelStart  = int.MaxValue;
            public int            SelEnd    = -1;
            public int            TdRow     = -1;
            public uint           segmentstart = 0;
            public uint           segmentend   = 0;
            public int            segmentNumber = -1;
            public string         segmentName   = "";
            public List<int>      searchedRows  = new List<int>();
            public List<uint>     foundLocations = new List<uint>();
            public List<uint>     foundBytes     = new List<uint>();
            public int            currentSearched = 0;
            public List<DGROW>    dgrows = new List<DGROW>();
            public bool           Primary { get; internal set; }
            public void ClearSearch()
            {
                currentSearched = 0;
                searchedRows.Clear();
                foundLocations.Clear();
                foundBytes.Clear();
            }
        }

        public struct HexData
        {
            public string TableText;
            public string TableName;
            public char   Prefix;
            public char   Suffix;
            public Color  Color;
            public int    TdIndex;
            public bool   SelectedTD;
            public int    Row;
            public int    Col;
        }

        private enum CopyColors { Generate, Copy, Freeze }

        public class DGROW
        {
            public DGROW()
            {
                Cols      = new List<string>();
                Addresses = new List<uint>();
            }
            public List<string> Cols      { get; set; }
            public List<uint>   Addresses { get; set; }
            public string       HeaderTxt { get; set; }
        }

        // ─── Kentät ──────────────────────────────────────────────────────────────

        private uint          selectedByte;
        public  FrmTuner      tuner;
        public  VisSettings   vis1;
        public  VisSettings   vis2;
        private CopyColors    leftColors  = CopyColors.Generate;
        private CopyColors    rightColors = CopyColors.Generate;
        private bool          SelectionModified = false;

        private Color[] colors =
        {
            Color.FromArgb(255, 192, 192, 255),
            Color.FromArgb(255, 255, 192, 255),
            Color.FromArgb(255, 255, 128, 128),
            Color.FromArgb(255, 255, 192, 128),
            Color.FromArgb(255, 128, 128, 255),
            Color.FromArgb(255, 255, 128, 255),
            Color.Silver,
            Color.FromArgb(255, 255, 128, 0),
            Color.Fuchsia,
            Color.Gray,
            Color.FromArgb(255, 192, 192, 0),
            Color.FromArgb(255, 0, 192, 0),
            Color.FromArgb(255, 0, 192, 192),
            Color.FromArgb(255, 0, 0, 192),
            Color.FromArgb(255, 192, 0, 192),
            Color.FromArgb(255, 64, 64, 64),
            Color.Maroon,
            Color.FromArgb(255, 128, 64, 0),
            Color.Olive,
            Color.Teal,
            Color.Navy,
            Color.FromArgb(255, 128, 64, 64),
            Color.FromArgb(255, 0, 64, 0),
        };

        // ─── Form Load ───────────────────────────────────────────────────────────

        private void frmTableVis_Load(object sender, EventArgs e)
        {
            hexView1.SelectionChanged += HexView1_SelectionChanged;
            hexView2.SelectionChanged += HexView2_SelectionChanged;
            hexView1.Scrolled         += HexView1_Scrolled;
            hexView2.Scrolled         += HexView2_Scrolled;
            //hexView1.KeyDown          += HexView1_KeyDown;
            //hexView2.KeyDown          += HexView2_KeyDown;

            this.KeyPreview = true;
            this.KeyDown   += FrmTableVisDouble_KeyDown;

            radioShowSegment.Text = "Segment [" + vis1.segmentName + "]";
            labelFileName1.Text   = vis1.PCM.FileName;

            if (vis2 != null && vis2.td != null)
            {
                labelFileName2.Text = vis2.PCM.FileName;
            }
            else
            {
                chkSyncScroll.Checked  = false;
                chkSyncScroll.Enabled  = false;
                btnApplytoRight.Enabled = false;
            }

            comboCopyColorsLeft.ValueMember   = "Value";
            comboCopyColorsLeft.DisplayMember = "Name";
            comboCopyColorsLeft.DataSource    = Enum.GetValues(typeof(CopyColors))
                .Cast<object>()
                .Select(v => new { Value = v, Name = v.ToString() })
                .ToList();
            comboCopyColorsLeft.Text = CopyColors.Generate.ToString();

            comboCopyColorsRight.ValueMember   = "Value";
            comboCopyColorsRight.DisplayMember = "Name";
            comboCopyColorsRight.DataSource    = Enum.GetValues(typeof(CopyColors))
                .Cast<object>()
                .Select(v => new { Value = v, Name = v.ToString() })
                .ToList();
            comboCopyColorsRight.Text = CopyColors.Generate.ToString();
        }

        // ─── HexView-tapahtumat ───────────────────────────────────────────────────

        private void HexView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    var sb = new StringBuilder();
                    foreach (uint addr in hexView1.GetSelectedAddresses())
                        sb.Append(vis1.PCM.buf[addr].ToString("X2") + " ");
                    Clipboard.SetDataObject(sb.ToString().Trim());
                    e.Handled = true;
                }
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void HexView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    var sb = new StringBuilder();
                    foreach (uint addr in hexView2.GetSelectedAddresses())
                        sb.Append(vis2.PCM.buf[addr].ToString("X2") + " ");
                    Clipboard.SetDataObject(sb.ToString().Trim());
                    e.Handled = true;
                }
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void FrmTableVisDouble_KeyDown(object sender, KeyEventArgs e)
        {
            HexViewControl hv  = hexView1.Focused ? hexView1 : hexView2;
            NumericUpDown  numUD = hexView1.Focused ? numExtraOffset1 : numExtraOffset2;

            if (e.KeyCode == Keys.Add)
            {
                if (hv.SelectedCount > 0) { numUD.Value += hv.SelectedCount; e.Handled = true; }
            }
            if (e.KeyCode == Keys.Subtract)
            {
                if (hv.SelectedCount > 0 && numUD.Value >= hv.SelectedCount)
                { numUD.Value -= hv.SelectedCount; e.Handled = true; }
            }
        }

        private void HexView1_SelectionChanged(object sender, HexSelectionChangedEventArgs e)
        {
            //if (!SelectionModified) return;
            SyncSelection(hexView1, hexView2, vis1, vis2);
        }

        private void HexView2_SelectionChanged(object sender, HexSelectionChangedEventArgs e)
        {
            //if (!SelectionModified) return;
            SyncSelection(hexView2, hexView1, vis2, vis1);
        }

        private bool _syncingScroll = false;

        private void HexView1_Scrolled(object sender, HexScrollEventArgs e)
        {
            if (!chkSyncScroll.Checked || _syncingScroll) return;
            _syncingScroll = true;
            SyncScroll1();
            _syncingScroll = false;
        }

        private void HexView2_Scrolled(object sender, HexScrollEventArgs e)
        {
            if (!chkSyncScroll.Checked || _syncingScroll) return;
            _syncingScroll = true;
            SyncScroll2();
            _syncingScroll = false;
        }

        // ─── Setup ───────────────────────────────────────────────────────────────

        private void SetupHexViews()
        {
            hexView1.BytesPerRow   = (int)numBytesPerRow.Value;
            hexView2.BytesPerRow   = (int)numBytesPerRow.Value;
            hexView1.RowHeaderWidth = 80;
            hexView2.RowHeaderWidth = 80;
        }

        // ─── Valinnan synkronointi ────────────────────────────────────────────────

        private List<uint> GetSelectedAddresses(HexViewControl hv, VisSettings vis)
        {
            return hv.GetSelectedAddresses();
        }

        private void RestoreSelection(List<uint> SelectedAddresses)
        {
            try
            {
                hexView1.SetSelectedAddresses(SelectedAddresses);
                SyncSelection(hexView1, hexView2, vis1, vis2);
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void SyncSelection(HexViewControl hvSrc, HexViewControl hvDst,
                                   VisSettings visSrc, VisSettings visDst)
        {
            //if (!SelectionModified) return;
            if (visDst == null) { GetSelectedTables(false); return; }

            SelectionModified = false;
            try
            {
                List<uint> srcAddrs   = hvSrc.GetSelectedAddresses();
                var        dstAddrs   = new List<uint>();
                int        scrollRow  = -2;
                int        firstRow   = hvSrc.FirstDisplayedScrollingRowIndex;

                uint scrollAddr  = firstRow < visSrc.dgrows.Count && visSrc.dgrows[firstRow].Addresses.Count > 0
                    ? visSrc.dgrows[firstRow].Addresses[0]
                    : 0;
                uint scrollAddr2 = (uint)(scrollAddr + (visDst.ExtraOffset - visSrc.ExtraOffset));

                foreach (uint addr in srcAddrs)
                {
                    uint dstAddr  = (uint)(addr + visDst.ExtraOffset - visSrc.ExtraOffset);
                    int  bufIdx   = (int)(dstAddr - visDst.buffOffset);
                    if (visDst.hexDatas != null &&
                        bufIdx >= 0 && bufIdx < visDst.hexDatas.Length)
                    {
                        dstAddrs.Add(dstAddr);
                        if (dstAddr == scrollAddr2)
                            scrollRow = visDst.hexDatas[bufIdx].Row;
                    }
                }

                hvDst.SetSelectedAddresses(dstAddrs);
                if (scrollRow >= 0)
                    hvDst.FirstDisplayedScrollingRowIndex = scrollRow;

                GetSelectedTables(false);
                GetSelectedTables(true);
            }
            catch (Exception ex) { LogErr(ex); }            
        }

        // ─── Vierityssynkronointi ─────────────────────────────────────────────────

        private void SyncScroll1()
        {
            if (vis2 == null || vis2.hexDatas == null) return;
            try
            {
                int r = hexView1.FirstDisplayedScrollingRowIndex;
                if (r >= vis1.dgrows.Count) return;
                uint addr1 = vis1.dgrows[r].Addresses[0];
                uint addr2 = (uint)(addr1 + vis2.ExtraOffset - vis1.ExtraOffset);
                int  bufIdx = (int)(addr2 - vis2.buffOffset);
                if (bufIdx >= 0 && bufIdx < vis2.hexDatas.Length)
                    hexView2.FirstDisplayedScrollingRowIndex = vis2.hexDatas[bufIdx].Row;
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void SyncScroll2()
        {
            if (vis1 == null || vis1.hexDatas == null) return;
            try
            {
                int r = hexView2.FirstDisplayedScrollingRowIndex;
                if (r >= vis2.dgrows.Count) return;
                uint addr2 = vis2.dgrows[r].Addresses[0];
                uint addr1 = (uint)(addr2 + vis1.ExtraOffset - vis2.ExtraOffset);
                int  bufIdx = (int)(addr1 - vis1.buffOffset);
                if (bufIdx >= 0 && bufIdx < vis1.hexDatas.Length)
                    hexView1.FirstDisplayedScrollingRowIndex = vis1.hexDatas[bufIdx].Row;
            }
            catch (Exception ex) { LogErr(ex); }
        }

        // ─── HexData-luonti (muuttumaton logiikka) ───────────────────────────────

        private HexData[] CopyHexData(HexData[] Source, int EOffset)
        {
            HexData[] retVal = new HexData[Source.Length];
            for (int a = 0; a < Source.Length; a++)
            {
                int b = a + EOffset;
                if (b >= 0 && b < retVal.Length)
                    retVal[b] = Source[a];
            }
            return retVal;
        }

        private Block GetBufferSize(VisSettings vis)
        {
            int bufStart = (int)vis.PCM.segmentinfos[vis.segmentNumber].GetStartAddr();
            int bufEnd   = (int)vis.PCM.segmentinfos[vis.segmentNumber].GetEndAddr();

            if (vis.ExtraOffset < 0)
            {
                if (radioShowTable.Checked && bufStart > (vis.td.StartAddressNoExtra() + vis.ExtraOffset - numFrontBytes.Value))
                    bufStart = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset - numFrontBytes.Value);
                else if (bufStart > (vis.td.StartAddressNoExtra() + vis.ExtraOffset))
                    bufStart = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset);
            }
            else if (radioShowTable.Checked && bufStart > (vis.td.StartAddressNoExtra() - numFrontBytes.Value))
                bufStart = (int)(vis.td.StartAddressNoExtra() - numFrontBytes.Value);

            if (vis.ExtraOffset > 0)
            {
                if ((vis.td.addrInt - vis.ExtraOffset) < bufStart)
                    bufStart = (int)(vis.td.addrInt - vis.ExtraOffset);
                if (radioShowTable.Checked && (vis.td.EndAddressNoExtra() + vis.ExtraOffset + numAfterBytes.Value) > bufEnd)
                    bufEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset + numAfterBytes.Value);
                else if ((vis.td.EndAddressNoExtra() + vis.ExtraOffset) > bufEnd)
                    bufEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset);
            }

            if ((vis.td.StartAddressNoExtra() + vis.ExtraOffset) < bufStart)
                bufStart = (int)(vis.td.StartAddress() + vis.ExtraOffset);
            if ((vis.td.EndAddressNoExtra() + vis.ExtraOffset) > bufEnd)
                bufEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset);
            if (bufEnd   > vis.PCM.buf.Length) bufEnd   = vis.PCM.buf.Length;
            if (bufStart < 0)                  bufStart = 0;

            Block b = new Block(); b.Start = (uint)bufStart; b.End = (uint)bufEnd;
            return b;
        }

        private void CreateHexDatas()
        {
            try
            {
                int bufStart, bufEnd;
                Block b1 = GetBufferSize(vis1);
                if (vis2 != null && vis2.td != null)
                {
                    Block b2 = GetBufferSize(vis2);
                    bufStart = (int)Math.Min(b1.Start, b2.Start);
                    bufEnd   = (int)Math.Max(b1.End,   b2.End);
                }
                else { bufStart = (int)b1.Start; bufEnd = (int)b1.End; }

                if (leftColors  == CopyColors.Generate) CreateHexData(ref vis1, bufStart, bufEnd);
                if (vis2 != null && vis2.td != null)
                {
                    if      (rightColors == CopyColors.Generate) CreateHexData(ref vis2, bufStart, bufEnd);
                    else if (rightColors == CopyColors.Copy)
                    { vis2.hexDatas = CopyHexData(vis1.hexDatas, vis2.ExtraOffset); vis2.buffOffset = vis1.buffOffset; }
                }
                if (leftColors == CopyColors.Copy)
                { vis1.hexDatas = CopyHexData(vis2.hexDatas, vis1.ExtraOffset); vis1.buffOffset = vis2.buffOffset; }
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void CreateHexData(ref VisSettings vis, int bufStart, int bufEnd)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                vis.buffOffset = bufStart;
                vis.hexDatas   = new HexData[bufEnd - bufStart + 1];
                int c = 0;
                vis.FilterSegmentTables();
                for (int t = 0; t < vis.SegmentTds.Count; t++)
                {
                    TableData tmpTd   = vis.SegmentTds[t];
                    int       hexAddr = (int)tmpTd.StartAddress();
                    int       buffAddr= hexAddr - vis.buffOffset;
                    if (buffAddr < 0 || buffAddr >= vis.hexDatas.Length) continue;

                    vis.hexDatas[buffAddr].TableText  = tmpTd.TableName + ": " + tmpTd.Address;
                    vis.hexDatas[buffAddr].TableText += "+" + tmpTd.Offset.ToString();
                    vis.hexDatas[buffAddr].TableText += "+" + tmpTd.extraoffset;
                    vis.hexDatas[buffAddr].TableText += " - " + tmpTd.EndAddress().ToString("X8");
                    vis.hexDatas[buffAddr].Prefix     = '(';

                    int endAddr = (int)tmpTd.EndAddress() - vis.buffOffset;
                    vis.hexDatas[endAddr].Suffix = ')';

                    for (int a = buffAddr; a <= endAddr && a < vis.PCM.fsize; a++)
                    {
                        vis.hexDatas[a].Color   = colors[c];
                        vis.hexDatas[a].TdIndex = t;
                        vis.hexDatas[a].TableName = tmpTd.TableName;
                    }
                    c++; if (c >= colors.Length - 1) c = 0;
                }

                int tdAddr = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset - vis.buffOffset);
                vis.hexDatas[tdAddr].TableText  = vis.td.TableName + ": " + vis.td.Address;
                vis.hexDatas[tdAddr].TableText += "+" + vis.td.Offset.ToString();
                vis.hexDatas[tdAddr].TableText += "+" + vis.ExtraOffset;
                vis.hexDatas[tdAddr].TableText += " - " + (vis.td.EndAddressNoExtra() + vis.ExtraOffset).ToString("X8");
                vis.hexDatas[tdAddr].Prefix     = '[';

                int tdEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset - vis.buffOffset);
                vis.hexDatas[tdEnd].Suffix = ']';

                int start = (int)vis.td.addrInt;
                int end   = (int)vis.td.addrInt + vis.td.Size();
                if (vis.td.Offset    < 0) start += vis.td.Offset;
                if (vis.ExtraOffset  < 0) start += vis.ExtraOffset;
                if (vis.ExtraOffset  > 0) end   += vis.ExtraOffset;
                if (vis.td.Offset    > 0) end   += vis.td.Offset;

                int OffsetEnd      = (int)(vis.td.StartAddressNoExtra());
                int ExtraOffsetEnd = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset);

                for (int addr = start; addr <= end && addr < vis.PCM.buf.Length; addr++)
                {
                    int buffAddr = addr - vis.buffOffset;
                    vis.hexDatas[buffAddr].SelectedTD = true;
                    vis.hexDatas[buffAddr].TableName  = vis.td.TableName;
                    if      (addr >= vis.td.StartAddressNoExtra() + vis.ExtraOffset && addr <= vis.td.EndAddressNoExtra() + vis.ExtraOffset)
                    { vis.hexDatas[buffAddr].Color = Color.LightCoral; vis.hexDatas[buffAddr].SelectedTD = true; }
                    else if (vis.td.Offset > 0 && addr >= vis.td.addrInt && addr < OffsetEnd)
                        vis.hexDatas[buffAddr].Color = Color.Purple;
                    else if (vis.td.Offset < 0 && addr <= vis.td.addrInt && addr >= OffsetEnd)
                        vis.hexDatas[buffAddr].Color = Color.Purple;
                    else if (vis.ExtraOffset > 0 && addr >= OffsetEnd && addr < ExtraOffsetEnd)
                        vis.hexDatas[buffAddr].Color = Color.Green;
                    else if (vis.ExtraOffset < 0 && addr <= OffsetEnd && addr >= ExtraOffsetEnd)
                        vis.hexDatas[buffAddr].Color = Color.Green;
                    if (addr == selectedByte)
                        vis.hexDatas[buffAddr].Color = Color.Red;
                }
            }
            catch (Exception ex) { LogErr(ex); }
            timer.Stop();
            Debug.WriteLine("CreateHexData: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 ms"));
        }

        // ─── ShowTables / UpdateDisplay / DisplayData ─────────────────────────────

        public void ShowTables(uint SelectedByte)
        {
            Debug.WriteLine("ShowTables");
            try
            {
                this.Text      = "Table data visualizer [" + vis1.td.TableName + "]";
                this.selectedByte = SelectedByte;

                numExtraOffset1.ValueChanged -= numExtraOffset1_ValueChanged;
                numExtraOffset2.ValueChanged -= numExtraOffset2_ValueChanged;

                numExtraOffset1.Value   = vis1.td.extraoffset;
                numExtraOffset1.Minimum = -1 * (vis1.td.StartAddressNoExtra());
                numExtraOffset1.Maximum = vis1.PCM.buf.Length - vis1.td.EndAddressNoExtra() - 1;

                CreateHexDatas();

                numExtraOffset1.ValueChanged += numExtraOffset1_ValueChanged;
                if (vis2 != null && vis2.td != null)
                {
                    numExtraOffset2.Value   = vis2.td.extraoffset;
                    numExtraOffset2.Minimum = -1 * (vis2.td.StartAddressNoExtra());
                    numExtraOffset2.Maximum = vis2.PCM.buf.Length - vis2.td.EndAddressNoExtra() - 1;
                    numExtraOffset2.ValueChanged += numExtraOffset2_ValueChanged;
                }

                UpdateDisplay(true);
            }
            catch (Exception ex) { LogErr(ex); }
        }

        public void UpdateDisplay(bool ScrollToSelected)
        {
            try
            {
                int  r           = hexView1.FirstDisplayedScrollingRowIndex;
                int  currentAddr = -1;
                if (r > -1 && r < vis1.dgrows.Count && vis1.dgrows[r].Addresses.Count > 0)
                    currentAddr = (int)vis1.dgrows[r].Addresses[0];

                if (vis2 != null && vis2.td != null)
                    DisplayData(selectedByte, true);
                else
                {
                    splitContainer1.Panel2.Hide();
                    splitContainer1.SplitterDistance = splitContainer1.Width - 5;
                }

                List<uint> SelectedAddresses = hexView1.GetSelectedAddresses();
                DisplayData(selectedByte, false);
                RestoreSelection(SelectedAddresses);

                if (ScrollToSelected && vis1.TdRow >= 0 && vis1.TdRow < hexView1.RowCount)
                    hexView1.FirstDisplayedScrollingRowIndex = vis1.TdRow;
                else if (currentAddr > -1 && vis1.hexDatas != null &&
                         (currentAddr - vis1.buffOffset) < vis1.hexDatas.Length)
                    hexView1.FirstDisplayedScrollingRowIndex =
                        vis1.hexDatas[currentAddr - vis1.buffOffset].Row;
            }
            catch (Exception ex) { LogErr(ex); }
        }

        public void DisplayData(uint selectedByte, bool Secondary)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                VisSettings    vis = Secondary ? vis2 : vis1;
                HexViewControl hv  = Secondary ? hexView2 : hexView1;
                this.selectedByte  = selectedByte;

                int bytesPerRow = (int)numBytesPerRow.Value;
                int start = 0, end = 0;

                if (radioShowTable.Checked)
                {
                    start = (int)vis.td.addrInt;
                    if (vis.td.Offset    < 0) start += vis.td.Offset;
                    if (vis.ExtraOffset  < 0) start += vis.ExtraOffset;
                    end   = (int)vis.td.EndAddressNoExtra();
                    if (vis.ExtraOffset  > 0) end    = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset);
                    start = (int)(start - numFrontBytes.Value);
                    end   = (int)(end   + numAfterBytes.Value);
                }
                else
                {
                    start = vis1.buffOffset;
                    end   = vis1.buffOffset + vis1.hexDatas.Length - 1;
                }

                if (start < 0) start = 0;
                if (end >= vis.PCM.buf.Length - 2) end = (int)vis.PCM.buf.Length - 2;

                if (Secondary && numExtraOffset2.Value > 0) end += (int)numExtraOffset2.Value;
                else if (!Secondary && numExtraOffset1.Value > 0) end += (int)numExtraOffset1.Value;

                // Rakenna DGROW-lista
                int col = 0, row = 0;
                vis.dgrows.Clear();
                DGROW dgrow = new DGROW();
                vis.dgrows.Add(dgrow);

                for (uint addr = (uint)start; addr <= end && addr < vis.PCM.buf.Length; addr++)
                {
                    int bufAddr = (int)addr - vis.buffOffset;
                    if (bufAddr < 0) continue;
                    if (bufAddr >= vis.hexDatas.Length) break;

                    if (vis.hexDatas[bufAddr].Prefix == 0) vis.hexDatas[bufAddr].Prefix = ' ';
                    if (vis.hexDatas[bufAddr].Suffix == 0) vis.hexDatas[bufAddr].Suffix = ' ';

                    if (radioSegmentTBNames.Checked && !string.IsNullOrEmpty(vis.hexDatas[bufAddr].TableText))
                    {
                        if (dgrow.Addresses.Count > 0)
                        { dgrow = new DGROW(); row++; vis.dgrows.Add(dgrow); }
                        dgrow.HeaderTxt = vis.hexDatas[bufAddr].TableText;
                        col = 0;
                    }

                    if (addr == (vis.td.StartAddressNoExtra() + vis.ExtraOffset))
                    {
                        vis.TdRow = row;
                        Debug.WriteLine("TdRow = " + row + ", td.addrInt=" + vis.td.addrInt.ToString("X"));
                    }

                    if (col >= bytesPerRow) { row++; col = 0; dgrow = new DGROW(); vis.dgrows.Add(dgrow); }
                    if (string.IsNullOrEmpty(dgrow.HeaderTxt)) dgrow.HeaderTxt = addr.ToString("X6");

                    vis.hexDatas[bufAddr].Row = row;
                    vis.hexDatas[bufAddr].Col = col;
                    dgrow.Cols.Add(vis.PCM.buf[addr].ToString("X2"));
                    dgrow.Addresses.Add(addr);
                    col++;
                }

                // Syötetään data HexViewControlille
                hv.BytesPerRow = bytesPerRow;
                hv.SetData(vis.dgrows, vis.hexDatas, vis.buffOffset, vis.PCM.buf);
                hv.SetFoundBytes(vis.foundBytes.Cast<uint>());
            }
            catch (Exception ex) { LogErr(ex); }
            timer.Stop();
            Debug.WriteLine("DisplayData: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 ms"));
        }

        // ─── ChangeSelection / ChangeSelection ───────────────────────────────────

        public void ShowTables(uint SelectedByte, bool dummy) => ShowTables(SelectedByte); // yhteensopivuus

        public void ChangeSelection(uint selectedByte)
        {
            try
            {
                this.selectedByte = selectedByte;
                int bufIdx = (int)(selectedByte - vis1.buffOffset);
                if (vis1.hexDatas != null && bufIdx >= 0 && bufIdx < vis1.hexDatas.Length)
                    hexView1.SelectAddress(selectedByte);
            }
            catch (Exception ex) { LogErr(ex); }
        }

        // ─── GetSelectedTables (käyttää nyt HexViewControlia) ────────────────────

        private List<TableData> GetSelectedTables(bool Secondary)
        {
            var selTables = new List<TableData>();
            try
            {
                HexViewControl hv      = Secondary ? hexView2 : hexView1;
                TextBox        txtInfo = Secondary ? txtInfo2 : txtInfo1;
                VisSettings    vis     = Secondary ? vis2     : vis1;
                if (vis == null) return selTables;

                var      selectedTdNames = new List<string>();
                int      Start = int.MaxValue, End = -1;
                List<uint> selAddrs = hv.GetSelectedAddresses();

                foreach (uint addr in selAddrs)
                {
                    int bufIdx = (int)addr - vis.buffOffset;
                    if (vis.hexDatas == null || bufIdx < 0 || bufIdx >= vis.hexDatas.Length) continue;
                    string tdName = vis.hexDatas[bufIdx].TableName;
                    if (tdName != null && !selectedTdNames.Contains(tdName))
                    {
                        selectedTdNames.Add(tdName);
                        selTables.Add(vis.SegmentTds[vis.hexDatas[bufIdx].TdIndex]);
                    }
                    if ((int)addr < Start) Start = (int)addr;
                    if ((int)addr > End)   End   = (int)addr;
                }

                if (Start < 0 || End < 0) { txtInfo.Text = ""; return selTables; }
                if (End < Start) { int t = Start; Start = End; End = t; }

                txtInfo.Text = "Selection range: " + Start.ToString("X") + " - " + End.ToString("X") + Environment.NewLine;
                txtInfo.AppendText("Tables: " + Environment.NewLine);
                foreach (var st in selTables)
                    txtInfo.AppendText(st.TableName + " [" + st.StartAddress().ToString("X") + "] ("
                        + st.addrInt.ToString("X") + " + " + st.Offset + " + " + st.extraoffset + ")" + Environment.NewLine);
            }
            catch (Exception ex) { LogErr(ex); }
            return selTables;
        }

        // ─── SelectRange (address-based) ─────────────────────────────────────────

        private void SelectRange(HexViewControl hv, VisSettings vis)
        {
            if (vis.SelStart >= 0 && vis.SelEnd > 0 &&
                vis.hexDatas != null &&
                vis.hexDatas.Length > (vis.SelStart - vis.buffOffset) &&
                vis.hexDatas.Length > (vis.SelEnd   - vis.buffOffset))
            {
                int s = vis.SelStart, e = vis.SelEnd;
                if (s > e) { int tmp = s; s = e; e = tmp; }
                var addrs = new List<uint>();
                for (int a = s; a < e; a++) addrs.Add((uint)a);
                hv.SetSelectedAddresses(addrs);
            }
        }

        private int GetSelectedAddress(HexViewControl hv, VisSettings vis)
        {
            var addrs = hv.GetSelectedAddresses();
            return addrs.Count > 0 ? (int)addrs[0] : -1;
        }

        // ─── UI-tapahtumakäsittelijät ─────────────────────────────────────────────

        private void radioShowSegment_CheckedChanged(object sender, EventArgs e)
        {
            if (radioShowSegment.Checked)
            {
                hexView1.RowHeaderWidth = 80;
                hexView2.RowHeaderWidth = 80;
                CreateHexDatas();
                UpdateDisplay(true);
            }
        }

        private void radioSegmentTBNames_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSegmentTBNames.Checked)
            {
                hexView1.RowHeaderWidth = 350;
                hexView2.RowHeaderWidth = 350;
                CreateHexDatas();
                UpdateDisplay(true);
            }
        }

        private void radioShowTable_CheckedChanged(object sender, EventArgs e)
        {
            if (radioShowTable.Checked)
            {
                hexView1.RowHeaderWidth = 80;
                hexView2.RowHeaderWidth = 80;
                CreateHexDatas();
                UpdateDisplay(true);
            }
        }

        private void numBytesPerRow_ValueChanged(object sender, EventArgs e)
        {
            hexView1.BytesPerRow = (int)numBytesPerRow.Value;
            hexView2.BytesPerRow = (int)numBytesPerRow.Value;
            UpdateDisplay(false);
        }

        private void numFrontBytes_ValueChanged(object sender, EventArgs e)
        {
            CreateHexDatas(); UpdateDisplay(false);
        }

        private void numAfterBytes_ValueChanged(object sender, EventArgs e)
        {
            CreateHexDatas(); UpdateDisplay(false);
        }

        private void numExtraBytes_ValueChanged(object sender, EventArgs e)
        {
            numFrontBytes.Value = numExtraBytes.Value;
            numAfterBytes.Value = numExtraBytes.Value;
        }

        private void numExtraOffset1_ValueChanged(object sender, EventArgs e)
        {
            vis1.ExtraOffset = (int)numExtraOffset1.Value;
            CreateHexDatas(); UpdateDisplay(false);
        }

        private void numExtraOffset2_ValueChanged(object sender, EventArgs e)
        {
            vis2.ExtraOffset = (int)numExtraOffset2.Value;
            CreateHexDatas(); UpdateDisplay(false);
        }

        private void comboCopyColorsLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { leftColors  = (CopyColors)comboCopyColorsLeft.SelectedValue;  CreateHexDatas(); UpdateDisplay(false); }
            catch (Exception ex) { LogErr(ex); }
        }

        private void comboCopyColorsRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            try { rightColors = (CopyColors)comboCopyColorsRight.SelectedValue; CreateHexDatas(); UpdateDisplay(false); }
            catch (Exception ex) { LogErr(ex); }
        }

        private void btnApplyPrimary_Click(object sender, EventArgs e)
        {
            vis1.tdOrg.extraoffset = (int)numExtraOffset1.Value;
            tuner?.RefreshFast();
        }

        private void btnApplySecondary_Click(object sender, EventArgs e)
        {
            vis2.tdOrg.extraoffset = (int)numExtraOffset2.Value;
            tuner?.RefreshFast();
        }

        private void btnPrevTable_Click(object sender, EventArgs e)
        {
            int x = FindTableDataId(vis1.td, vis1.PCM.tableDatas) - 1;
            if (x > -1)
            {
                vis1.ChangeTd(vis1.PCM, vis1.PCM.tableDatas[x]);
                if (vis2 != null && vis2.td != null)
                    vis2.ChangeTd(vis2.PCM, FindTableData(vis1.td, vis2.PCM.tableDatas));
                ShowTables(selectedByte);
            }
        }

        private void btnNextTable_Click(object sender, EventArgs e)
        {
            int x = FindTableDataId(vis1.td, vis1.PCM.tableDatas) + 1;
            if (x > -1 && x < vis1.PCM.tableDatas.Count)
            {
                vis1.ChangeTd(vis1.PCM, vis1.PCM.tableDatas[x]);
                if (vis2 != null && vis2.td != null)
                    vis2.ChangeTd(vis2.PCM, FindTableData(vis1.td, vis2.PCM.tableDatas));
                ShowTables(selectedByte);
            }
        }

        private void btnApplyToSelection1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var td in GetSelectedTables(false))
                {
                    Logger("Updating table: " + td.TableName);
                    td.extraoffset = (int)numExtraOffset1.Value;
                }
                tuner?.RefreshFast();
                CreateHexDatas(); UpdateDisplay(false);
            }
            catch (Exception ex) { LoggerBold(ex.Message); }
        }

        private void btnApplyToSelection2_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var td in GetSelectedTables(true))
                {
                    Logger("Updating table: " + td.TableName);
                    td.extraoffset = (int)numExtraOffset2.Value;
                }
                tuner?.RefreshFast();
                CreateHexDatas(); UpdateDisplay(false);
            }
            catch (Exception ex) { LoggerBold(ex.Message); }
        }

        private void btnSelStart1_Click(object sender, EventArgs e)
        {
            vis1.SelStart = GetSelectedAddress(hexView1, vis1);
            SelectRange(hexView1, vis1);
            SyncSelection(hexView1, hexView2, vis1, vis2);
        }

        private void btnSelEnd1_Click(object sender, EventArgs e)
        {
            vis1.SelEnd = GetSelectedAddress(hexView1, vis1);
            SelectRange(hexView1, vis1);
            SyncSelection(hexView1, hexView2, vis1, vis2);
        }

        private void btnSelStart2_Click(object sender, EventArgs e)
        {
            vis2.SelStart = GetSelectedAddress(hexView2, vis2);
            SelectRange(hexView2, vis2);
            SyncSelection(hexView2, hexView1, vis2, vis1);
        }

        private void btnSelEnd2_Click(object sender, EventArgs e)
        {
            vis2.SelEnd = GetSelectedAddress(hexView2, vis2);
            SelectRange(hexView2, vis2);
            SyncSelection(hexView2, hexView1, vis2, vis1);
        }

        private void btnApplytoRight_Click(object sender, EventArgs e)
        {
            try
            {
                int offset = (int)numExtraOffset2.Value;
                foreach (var td in GetSelectedTables(false))
                {
                    TableData tdR = FindTableData(td, vis2.SortedTds);
                    if (tdR != null)
                    {
                        Logger("Applying offset: " + offset + " to table: " + tdR.TableName);
                        if (offset == 0) tdR.ExtraOffset = "-0"; else tdR.extraoffset = offset;
                    }
                }
                tuner?.RefreshFast();
                CreateHexDatas(); UpdateDisplay(false);
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void btnApplytoLeft_Click(object sender, EventArgs e)
        {
            try
            {
                int offset = (int)numExtraOffset1.Value;
                foreach (var td in GetSelectedTables(true))
                {
                    TableData tdL = FindTableData(td, vis1.SortedTds);
                    if (tdL != null)
                    {
                        Logger("Applying offset: " + offset + " to table: " + tdL.TableName);
                        if (offset == 0) tdL.ExtraOffset = "-0"; else tdL.extraoffset = offset;
                    }
                }
                tuner?.RefreshFast();
                CreateHexDatas(); UpdateDisplay(false);
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void btnScrollToSelected_Click(object sender, EventArgs e)
        {
            if (vis1.TdRow >= 0 && vis1.TdRow < hexView1.RowCount)
                hexView1.FirstDisplayedScrollingRowIndex = vis1.TdRow;
        }

        private void btnScrollToSelected2_Click(object sender, EventArgs e)
        {
            if (vis2 != null && vis2.TdRow >= 0 && vis2.TdRow < hexView2.RowCount)
                hexView2.FirstDisplayedScrollingRowIndex = vis2.TdRow;
        }

        private void btnCreateTable1_Click(object sender, EventArgs e)
        {
            try
            {
                var addrs = hexView1.GetSelectedAddresses();
                if (addrs.Count == 0) return;
                int Start = (int)addrs.First(), End = (int)addrs.Last();
                if (End < Start) { int t = Start; Start = End; End = t; }
                TableData newTd = new TableData { addrInt = (uint)Start };
                newTd.Rows    = (ushort)(End - Start > 0 ? End - Start : 1);
                newTd.Columns = 1;
                frmTdEditor fte = new frmTdEditor { td = newTd };
                fte.LoadTd();
                if (fte.ShowDialog() == DialogResult.OK) { vis1.PCM.tableDatas.Add(newTd); ShowTables(selectedByte); }
                fte.Dispose();
            }
            catch (Exception ex) { LogErr(ex); }
        }

        private void btnCreateTable2_Click(object sender, EventArgs e)
        {
            try
            {
                var addrs = hexView2.GetSelectedAddresses();
                if (addrs.Count == 0) return;
                int Start = (int)addrs.First(), End = (int)addrs.Last();
                if (End < Start) { int t = Start; Start = End; End = t; }
                TableData newTd = new TableData { addrInt = (uint)Start };
                newTd.Rows    = (ushort)(End - Start > 0 ? End - Start : 1);
                newTd.Columns = 1;
                frmTdEditor fte = new frmTdEditor { td = newTd };
                fte.LoadTd();
                if (fte.ShowDialog() == DialogResult.OK) { vis2.PCM.tableDatas.Add(newTd); ShowTables(selectedByte); }
                fte.Dispose();
            }
            catch (Exception ex) { LogErr(ex); }
        }

        // ─── Haku ────────────────────────────────────────────────────────────────

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var addrs = hexView1.GetSelectedAddresses();
            var bytes = addrs.Select(a => vis1.PCM.buf[a]).ToList();
            SearchCells(bytes);
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var addrs = hexView2.GetSelectedAddresses();
            var bytes = addrs.Select(a => vis2.PCM.buf[a]).ToList();
            SearchCells(bytes);
        }

        private void ClearSearch()
        {
            vis1.ClearSearch();
            vis2?.ClearSearch();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            hexView1.SetFoundBytes(Enumerable.Empty<uint>());
            hexView2.SetFoundBytes(Enumerable.Empty<uint>());
        }

        private void clearSearchToolStripMenuItem_Click(object sender, EventArgs e)  => ClearSearch();
        private void clearSearchToolStripMenuItem1_Click(object sender, EventArgs e) => ClearSearch();

        private void SearchCells(List<byte> searchBytes)
        {
            try
            {
                ClearSearch();
                for (uint a = vis1.segmentstart; a < vis1.segmentend; a++)
                {
                    bool found = true;
                    for (int a2 = 0; a2 < searchBytes.Count; a2++)
                        if (vis1.PCM.buf[a + a2] != searchBytes[a2]) { found = false; break; }
                    if (found)
                    {
                        vis1.foundLocations.Add(a);
                        listBox1.Items.Add(a.ToString("X10"));
                        for (uint b = a; b < a + searchBytes.Count; b++) vis1.foundBytes.Add(b);
                    }
                }
                foreach (DGROW dgrow in vis1.dgrows)
                    foreach (uint addr in dgrow.Addresses)
                        if (vis1.foundBytes.Contains(addr) && !vis1.searchedRows.Contains(vis1.dgrows.IndexOf(dgrow)))
                            vis1.searchedRows.Add(vis1.dgrows.IndexOf(dgrow));

                if (vis2 != null && vis2.td != null)
                {
                    for (uint a = vis2.segmentstart; a < vis2.segmentend; a++)
                    {
                        bool found = true;
                        for (int a2 = 0; a2 < searchBytes.Count; a2++)
                            if (vis2.PCM.buf[a + a2] != searchBytes[a2]) { found = false; break; }
                        if (found)
                        {
                            vis2.foundLocations.Add(a);
                            listBox2.Items.Add(a.ToString("X10"));
                            for (uint b = a; b < a + searchBytes.Count; b++) vis2.foundBytes.Add(b);
                        }
                    }
                    foreach (DGROW dgrow in vis2.dgrows)
                        foreach (uint addr in dgrow.Addresses)
                            if (vis2.foundBytes.Contains(addr) && !vis2.searchedRows.Contains(vis2.dgrows.IndexOf(dgrow)))
                                vis2.searchedRows.Add(vis2.dgrows.IndexOf(dgrow));
                }

                hexView1.SetFoundBytes(vis1.foundBytes.Cast<uint>());
                if (vis2 != null && vis2.td != null) hexView2.SetFoundBytes(vis2.foundBytes.Cast<uint>());

                txtInfo1.AppendText("Found in: " + vis1.foundLocations.Count + " locations" + Environment.NewLine);
                if (vis2 != null && vis2.td != null)
                    txtInfo2.AppendText("Found in: " + vis2.foundLocations.Count + " locations" + Environment.NewLine);
            }
            catch (Exception ex) { Debug.WriteLine("SearchCells: " + ex.Message); }
        }

        private void showFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vis1.searchedRows.Count > 0) hexView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[0];
        }

        private void showPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vis1.currentSearched > 0) hexView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[--vis1.currentSearched];
        }

        private void showNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vis1.currentSearched < vis1.searchedRows.Count - 1)
                hexView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[++vis1.currentSearched];
        }

        private void showLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vis1.currentSearched = vis1.searchedRows.Count - 1;
            hexView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[vis1.currentSearched];
        }

        private void showFirstToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2 != null && vis2.searchedRows.Count > 0) hexView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[0];
        }

        private void showPreviousToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2 != null && vis2.currentSearched > 0) hexView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[--vis2.currentSearched];
        }

        private void showNextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2 != null && vis2.currentSearched < vis2.searchedRows.Count - 1)
                hexView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[++vis2.currentSearched];
        }

        private void showLastToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2 != null)
            {
                vis2.currentSearched = vis2.searchedRows.Count - 1;
                hexView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[vis2.currentSearched];
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!HexToInt(listBox1.SelectedItem.ToString(), out int addr)) return;
            for (int r = 0; r < vis1.dgrows.Count; r++)
                if (vis1.dgrows[r].Addresses[0] >= addr)
                { hexView1.FirstDisplayedScrollingRowIndex = Math.Max(0, r - 1); break; }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vis2 == null) return;
            if (!HexToInt(listBox2.SelectedItem.ToString(), out int addr)) return;
            for (int r = 0; r < vis2.dgrows.Count; r++)
                if (vis2.dgrows[r].Addresses[0] >= addr)
                { hexView2.FirstDisplayedScrollingRowIndex = Math.Max(0, r - 1); break; }
        }

        // ─── Apumetodi virheenkäsittelyyn ────────────────────────────────────────

        private void LogErr(Exception ex)
        {
            var st    = new System.Diagnostics.StackTrace(ex, true);
            var frame = st.GetFrame(st.FrameCount - 1);
            LoggerBold("Error, frmTableVisDouble, line " + frame?.GetFileLineNumber() + ": " + ex.Message);
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fdlg = new FontDialog();
            fdlg.Font = hexView1.TextFont;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                hexView1.TextFont = fdlg.Font;
                hexView2.TextFont = fdlg.Font;
            }
        }
    }
}
