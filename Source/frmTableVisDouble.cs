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
    public partial class frmTableVisDouble : Form
    {
        public frmTableVisDouble(PcmFile PCM1, PcmFile PCM2, TableData td1, TableData td2)
        {
            InitializeComponent();
            SetupDatagrids();
            vis1 = new VisSettings(PCM1, td1, true);
            vis2 = new VisSettings(PCM2, td2, false);
        }

        public class VisSettings
        {
            public VisSettings() { }
            public VisSettings(PcmFile PCM, TableData td, bool Primary) 
            {
                this.Primary = Primary;
                ChangeTd(PCM, td);
            }
            public void ChangeTd(PcmFile PCM, TableData td)
            {
                this.PCM = PCM;
                if (td != null)
                {
                    this.td = td.ShallowCopy(false);
                    this.tdOrg = td;
                    int seg = PCM.GetSegmentNumber(td.addrInt);
                    if (seg > -1)
                    {
                        segmentNumber = seg;
                        segmentName = PCM.Segments[seg].Name;
                        segmentstart = PCM.segmentinfos[seg].GetStartAddr();
                        segmentend = PCM.segmentinfos[seg].GetEndAddr();
                    }
                }
            }
            public PcmFile PCM;
            public TableData td { get; internal set; }
            public TableData tdOrg;
            public HexData[] hexDatas;
            public int ExtraOffset;
            public List<TableData> SortedTds;
            public int SelStart = int.MaxValue;
            public int SelEnd = -1;
            public int TdRow = -1;
            public uint segmentstart = 0;
            public uint segmentend = 0;
            public int segmentNumber = 0;
            public string segmentName = "";
            public List<int> searchedRows = new List<int>();
            public List<uint> foundLocations = new List<uint>();
            public List<uint> foundBytes = new List<uint>();
            public int currentSearched = 0;
            public List<DGROW> dgrows = new List<DGROW>();
            public bool Primary { get; internal set; }
            public void ClearSelection()
            {
                SelStart = int.MaxValue;
                SelEnd = -1;
            }
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
            public string Prefix;
            public string Suffix;
            public Color Color;
            public int TdIndex;
            public bool SelectedTD;
        }

        public class DGROW
        {
            public DGROW() 
            {
                Cols = new List<string>();
                Addresses = new List<uint>();
            }
            public List<string> Cols { get; set; }
            public List<uint> Addresses { get; set; }
            public string HeaderTxt { get; set; }
        }
        private uint selectedByte;
        public FrmTuner tuner;
        private int offset = 0;
        public VisSettings vis1;
        public VisSettings vis2;

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

        private void frmTableVis_Load(object sender, EventArgs e)
        {
            DrawingControl.SetDoubleBuffered(dataGridView1);
            DrawingControl.SetDoubleBuffered(dataGridView2);
            dataGridView1.Scroll += DataGridView1_Scroll;
            dataGridView2.Scroll += DataGridView2_Scroll;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
            dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
            dataGridView2.ColumnHeaderMouseClick += DataGridView2_ColumnHeaderMouseClick;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
            dataGridView2.CellPainting += DataGridView2_CellPainting;
            dataGridView1.CellValueNeeded += DataGridView1_CellValueNeeded;
            dataGridView2.CellValueNeeded += DataGridView2_CellValueNeeded;
            this.KeyPreview = true;
            this.KeyDown += FrmTableVisDouble_KeyDown;
            radioShowSegment.Text = "Segment [" + vis1.segmentName + "]";
            labelFileName1.Text = vis1.PCM.FileName;
            if (vis2.td != null)
            {
                labelFileName2.Text = vis2.PCM.FileName;
            }
            else
            {
                chkSyncScroll.Checked = false;
                chkSyncScroll.Enabled = false;
            }
        }

        private void FrmTableVisDouble_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("Keycode: " + e.KeyCode.ToString());
            DataGridView dgv;
            NumericUpDown numUD;
            if (dataGridView1.Focused)
            {
                dgv = dataGridView1;
                numUD = numExtraOffset1;
            }
            else
            {
                dgv = dataGridView2;
                numUD = numExtraOffset2;
            }

            if (e.KeyCode == Keys.Add)
            {
                if (dgv.SelectedCells.Count > 0)
                {
                    numUD.Value += dgv.SelectedCells.Count;
                    e.Handled = true;
                }
            }
            if (e.KeyCode == Keys.Subtract)
            {
                if (dgv.SelectedCells.Count > 0 && numUD.Value >= dgv.SelectedCells.Count)
                {
                    numUD.Value -= dgv.SelectedCells.Count;
                    e.Handled = true;
                }
            }
        }
        private void DataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                int r = e.RowIndex;
                int c = e.ColumnIndex;
                if (vis1.dgrows.Count > r)
                {
                    DGROW dgrow = vis1.dgrows[r];
                    if (c < dgrow.Cols.Count)
                    {
                        uint addr = dgrow.Addresses[c];
                        {
                            if (c == 0)
                            {
                                dataGridView1.Rows[r].HeaderCell.Value = dgrow.HeaderTxt;
                            }
                            e.Value = vis1.hexDatas[addr].Prefix + vis1.PCM.buf[addr].ToString("X2") + vis1.hexDatas[addr].Suffix;
                            dataGridView1.Rows[r].Cells[c].Style.ForeColor = vis1.hexDatas[addr].Color;
                            dataGridView1.Rows[r].Cells[c].ToolTipText = vis1.hexDatas[addr].TableName;

                        }
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
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }

        }
        private void DataGridView2_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                int r = e.RowIndex;
                int c = e.ColumnIndex;
                if (vis2.dgrows.Count > r)
                { 
                    DGROW dgrow = vis2.dgrows[r];
                    if (c < dgrow.Addresses.Count)
                    {
                        uint addr = dgrow.Addresses[c];
                        if (c == 0)
                        {
                            dataGridView2.Rows[r].HeaderCell.Value = dgrow.HeaderTxt;
                        }
                        e.Value = vis2.hexDatas[addr].Prefix + vis2.PCM.buf[addr].ToString("X2") + vis2.hexDatas[addr].Suffix;
                        dataGridView2.Rows[r].Cells[c].ToolTipText = vis2.hexDatas[addr].TableName;
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
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
        }
        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (r>=0 && c>= 0)
            {
                e.CellStyle.BackColor = Color.White;
                if  (c<vis1.dgrows[r].Cols.Count)
                {
                    DGROW dgrow = vis1.dgrows[r];
                    uint addr = dgrow.Addresses[c];
                    dataGridView1.Rows[r].Cells[c].Style.ForeColor = vis1.hexDatas[addr].Color;
                    if (vis1.foundBytes.Contains(addr))
                    {
                        e.CellStyle.BackColor = Color.Yellow;
                    }
                    if (vis1.hexDatas[addr].SelectedTD)
                    {
                        e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                        dataGridView1.Rows[r].HeaderCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    }
                    if (c == 0 && radioSegmentTBNames.Checked)
                    {
                        dataGridView1.Rows[r].HeaderCell.Style.ForeColor = vis1.hexDatas[addr].Color;
                    }
                }

            }
        }
        private void DataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            if (r >= 0 && c >= 0)
            {
                e.CellStyle.BackColor = Color.White;
                if (c < vis2.dgrows[r].Cols.Count)
                {
                    DGROW dgrow = vis2.dgrows[r];
                    uint addr = dgrow.Addresses[c];
                    dataGridView2.Rows[r].Cells[c].Style.ForeColor = vis2.hexDatas[addr].Color;
                    if (vis2.foundBytes.Contains(addr))
                    {
                        e.CellStyle.BackColor = Color.Yellow;
                    }
                    if (vis2.hexDatas[addr].SelectedTD)
                    {
                        e.CellStyle.Font = new Font(dataGridView2.Font, FontStyle.Bold);
                        dataGridView2.Rows[r].HeaderCell.Style.Font = new Font(dataGridView2.Font, FontStyle.Bold);
                    }
                    if (c == 0 && radioSegmentTBNames.Checked)
                    {
                        dataGridView2.Rows[r].HeaderCell.Style.ForeColor = vis2.hexDatas[addr].Color;
                    }

                }

            }
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }
        private void DataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }
        private void SetupDatagrids()
        {
            try
            {
                dataGridView1.Columns.Clear();
                dataGridView2.Columns.Clear();
                for (int c = 0; c < numBytesPerRow.Value; c++)
                {
                    dataGridView1.Columns.Add(c.ToString("X"), c.ToString("X"));
                    dataGridView2.Columns.Add(c.ToString("X"), c.ToString("X"));
                    dataGridView1.Columns[c.ToString("X")].Width = 35;
                    dataGridView2.Columns[c.ToString("X")].Width = 35;
                }
                dataGridView1.GridColor = Color.White;
                dataGridView2.GridColor = Color.White;
                dataGridView1.VirtualMode = true;
                dataGridView2.VirtualMode = true;
                dataGridView1.RowHeadersWidth = 80;
                dataGridView2.RowHeadersWidth = 80;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
        }
        private void ModifySelection(DataGridView dgv)
        {
            int r1 = dgv.CurrentCell.RowIndex;
            int r2 = dgv.SelectedCells[dgv.SelectedCells.Count - 1].RowIndex;
            int c1 = dgv.CurrentCell.ColumnIndex;
            int c2 = dgv.SelectedCells[dgv.SelectedCells.Count - 1].ColumnIndex;
            if (r2 != r1)
            {
                Debug.WriteLine("Selection row1: " + r1 + ", Selection row2: " + r2);
                dgv.ClearSelection();
                if (r1 > r2)
                {
                    int tmp = r1;
                    r1 = r2;
                    r2 = tmp;
                    tmp = c1;
                    c1 = c2;
                    c2 = tmp;
                }
                for (int c = c1; c < dgv.ColumnCount; c++)
                {
                    dgv.Rows[r1].Cells[c].Selected = true;
                }
                for (int r = r1 + 1; r < r2; r++)
                {
                    for (int c = 0; c < dgv.ColumnCount; c++)
                    {
                        dgv.Rows[r].Cells[c].Selected = true;
                    }
                }
                for (int c = 0; c < c2; c++)
                {
                    dgv.Rows[r2].Cells[c].Selected = true;
                }

            }

        }
        private void SyncSelection1(bool MouseSelection)
        {
            try
            {
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                dataGridView2.SelectionChanged -= DataGridView2_SelectionChanged;
                dataGridView2.ClearSelection();
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    vis1.ClearSelection();
                    vis2.ClearSelection();
                }
                else
                {
                    if (MouseSelection)
                    {
                        ModifySelection(dataGridView1);
                    }
                    List<uint> selectedAddresses = new List<uint>();
                    for (int x = 0; x < dataGridView1.SelectedCells.Count; x++)
                    {
                        DGROW dgrow = vis1.dgrows[dataGridView1.SelectedCells[x].RowIndex];
                        if (dataGridView1.SelectedCells[x].ColumnIndex < dgrow.Addresses.Count)
                        {
                            selectedAddresses.Add((uint)(dgrow.Addresses[dataGridView1.SelectedCells[x].ColumnIndex] + numExtraOffset2.Value - numExtraOffset1.Value));
                            if (dataGridView1.SelectedCells[x].RowIndex < vis1.SelStart)
                                vis1.SelStart = dataGridView1.SelectedCells[x].RowIndex;
                            if (dataGridView1.SelectedCells[x].RowIndex > vis1.SelEnd)
                                vis1.SelEnd = dataGridView1.SelectedCells[x].RowIndex;
                        }
                    }
                    selectedAddresses.Sort();
                    int scrollRow = -2;
                    uint scrollAddr = vis1.dgrows[dataGridView1.FirstDisplayedScrollingRowIndex].Addresses[0];
                    for (int r = 0; r < vis2.dgrows.Count; r++)
                    {
                        DGROW dgrow = vis2.dgrows[r];
                        for (int c = 0; c < dgrow.Addresses.Count; c++)
                        {
                            uint addr = dgrow.Addresses[c];
                            if (selectedAddresses.Contains(addr))
                            {
                                dataGridView2.Rows[r].Cells[c].Selected = true;
                                if (r < vis2.SelStart)
                                    vis2.SelStart = r;
                                if (r > vis2.SelEnd)
                                    vis2.SelEnd = r;
                            }
                            if (addr == (uint)(scrollAddr + (numExtraOffset2.Value - numExtraOffset1.Value)))
                            {
                                scrollRow = r;
                            }
                        }
                    }
                    if (scrollRow >= 0)
                    {
                        dataGridView2.Scroll -= DataGridView2_Scroll;
                        dataGridView2.FirstDisplayedScrollingRowIndex = scrollRow;
                    }
                }
                GetSelectedTables(false);
                GetSelectedTables(true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
            dataGridView2.Scroll += DataGridView2_Scroll;

        }
        private void SyncSelection2(bool MouseSelection)
        {
            try
            {
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                dataGridView2.SelectionChanged -= DataGridView2_SelectionChanged;
                dataGridView1.ClearSelection();
                if (dataGridView2.SelectedCells.Count == 0)
                {
                    vis1.ClearSelection();
                    vis2.ClearSelection();
                }
                else
                {
                    if (MouseSelection)
                    {
                        ModifySelection(dataGridView2);
                    }
                    List<uint> selectedAddresses = new List<uint>();
                    for (int x = 0; x < dataGridView2.SelectedCells.Count; x++)
                    {
                        DGROW dgrow = vis2.dgrows[dataGridView2.SelectedCells[x].RowIndex];
                        if (dataGridView2.SelectedCells[x].ColumnIndex < dgrow.Addresses.Count)
                        {
                            selectedAddresses.Add((uint)(dgrow.Addresses[dataGridView2.SelectedCells[x].ColumnIndex] + numExtraOffset1.Value - numExtraOffset2.Value));
                            if (dataGridView2.SelectedCells[x].RowIndex < vis2.SelStart)
                                vis2.SelStart = dataGridView2.SelectedCells[x].RowIndex;
                            if (dataGridView2.SelectedCells[x].RowIndex > vis2.SelEnd)
                                vis2.SelEnd = dataGridView2.SelectedCells[x].RowIndex;
                        }
                    }
                    int scrollRow = -2;
                    uint scrollAddr = vis2.dgrows[dataGridView2.FirstDisplayedScrollingRowIndex].Addresses[0];
                    for (int r = 0; r < vis1.dgrows.Count; r++)
                    {
                        DGROW dgrow = vis1.dgrows[r];
                        for (int c = 0; c < dgrow.Addresses.Count; c++)
                        {
                            uint addr = dgrow.Addresses[c];
                            if (selectedAddresses.Contains(addr))
                            {
                                dataGridView1.Rows[r].Cells[c].Selected = true;
                                if (r < vis1.SelStart)
                                    vis1.SelStart = r;
                                if (r > vis1.SelEnd)
                                    vis1.SelEnd = r;
                            }
                            if (addr == (uint)(scrollAddr + (numExtraOffset1.Value - numExtraOffset2.Value)))
                            {
                                scrollRow = r;
                            }

                        }
                    }
                    if (scrollRow >= 0)
                    {
                        dataGridView1.Scroll -= DataGridView1_Scroll;
                        dataGridView1.FirstDisplayedScrollingRowIndex = scrollRow;
                    }

                }
                GetSelectedTables(false);
                GetSelectedTables(true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
            dataGridView1.Scroll += DataGridView1_Scroll;

        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            SyncSelection1(true);
        }
        private void DataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            SyncSelection2(true);
        }

        private void SyncScroll1()
        {
            dataGridView2.Scroll -= DataGridView2_Scroll;
            try
            {
                if (chkSyncScroll.Checked)
                {
                    int scrollRow = -2;
                    uint scrollAddr = vis1.dgrows[dataGridView1.FirstDisplayedScrollingRowIndex].Addresses[0];
                    for (int r = 0; r < vis2.dgrows.Count; r++)
                    {
                        DGROW dgrow = vis2.dgrows[r];
                        for (int c = 0; c < dgrow.Addresses.Count; c++)
                        {
                            uint addr = dgrow.Addresses[c];
                            if (addr == (uint)(scrollAddr + (numExtraOffset2.Value - numExtraOffset1.Value)))
                            {
                                scrollRow = r;
                                break;
                            }
                        }
                    }
                    if (scrollRow >= 0)
                    {
                        dataGridView2.FirstDisplayedScrollingRowIndex = scrollRow;
                    }
                }
            }
            catch { }
            dataGridView2.Scroll += DataGridView2_Scroll;
        }
        private void SyncScroll2()
        {
            dataGridView1.Scroll -= DataGridView1_Scroll;
            try
            {
                if (chkSyncScroll.Checked)
                {
                    int scrollRow = -2;
                    uint scrollAddr = vis2.dgrows[dataGridView2.FirstDisplayedScrollingRowIndex].Addresses[0];
                    for (int r = 0; r < vis1.dgrows.Count; r++)
                    {
                        DGROW dgrow = vis1.dgrows[r];
                        for (int c = 0; c < dgrow.Addresses.Count; c++)
                        {
                            uint addr = dgrow.Addresses[c];
                            if (addr == (uint)(scrollAddr + (numExtraOffset1.Value - numExtraOffset2.Value)))
                            {
                                scrollRow = r;
                                break;
                            }

                        }
                    }
                    if (scrollRow >= 0)
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = scrollRow;
                    }
                }
            }
            catch { }
            dataGridView1.Scroll += DataGridView1_Scroll;

        }
        private void DataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            SyncScroll1();
        }

        private void DataGridView2_Scroll(object sender, ScrollEventArgs e)
        {
            SyncScroll2();
        }

        public void ShowTables(uint SelectedByte)
        {
            this.Text = "Table data visualizer [" + vis1.td.TableName + "]";
            numExtraOffset1.Value = vis1.td.ExtraOffset;
            this.selectedByte = SelectedByte;
            vis1.SortedTds  = vis1.PCM.tableDatas.OrderBy(x => x.StartAddress()).ToList();
            CreateHexData(ref vis1);
            if (vis2.td != null)
            {
                vis2.SortedTds = vis2.PCM.tableDatas.OrderBy(x => x.StartAddress()).ToList();
                numExtraOffset2.Value = vis2.td.ExtraOffset;
                if (chkCopyColors1.Checked)
                {
                    CopyColors1();
                }
                else
                {
                    CreateHexData(ref vis2);
                }
                numExtraOffset2.ValueChanged += numExtraOffset2_ValueChanged;
            }
            numExtraOffset1.ValueChanged += numExtraOffset1_ValueChanged;
            UpdateDisplay(true);
        }

        private List<TableData> GetSelectedTables(bool Secondary)
        {
            List<TableData> selTables = new List<TableData>();
            try
            {
                int Start = -1;
                int End = -1;
                DataGridView dgv = dataGridView1;
                TextBox txtInfo = txtInfo1;
                VisSettings vis = vis1;
                if (Secondary)
                {
                    vis = vis2;
                    dgv = dataGridView2;
                    txtInfo = txtInfo2;
                }

                if (dgv.SelectedCells.Count > 0)
                {
                    Start = (int)vis.dgrows[dgv.SelectedCells[0].RowIndex].Addresses[dgv.SelectedCells[0].ColumnIndex];
                    End = (int)vis.dgrows[dgv.SelectedCells[dgv.SelectedCells.Count - 1].RowIndex].Addresses[dgv.SelectedCells[dgv.SelectedCells.Count - 1].ColumnIndex];
                }
                if (End < Start)
                {
                    int tmp = Start;
                    Start = End;
                    End = tmp;
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
                for (int t = 0; t < vis.SortedTds.Count; t++)
                {
                    if (vis.SortedTds[t].StartAddress() + vis.ExtraOffset >= Start)
                    {
                        TStart = t;
                        break;
                    }
                }
                for (int t = TStart; t < vis.SortedTds.Count; t++)
                {
                    if (vis.SortedTds[t].EndAddress() + vis.ExtraOffset > End)
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
                    txtInfo.AppendText(vis.SortedTds[t].TableName + " [" + vis.SortedTds[t].StartAddress().ToString("X") + "] ("
                        + vis.SortedTds[t].addrInt.ToString("X") + " + " + vis.SortedTds[t].Offset.ToString()
                        + " + " + vis.SortedTds[t].ExtraOffset.ToString() + ")" + Environment.NewLine);
                    selTables.Add(vis.SortedTds[t]);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }

            return selTables;
        }

        private void CreateHexData(ref VisSettings vis)
        {
            try
            {
                vis.hexDatas = new HexData[vis.PCM.fsize];
                int c = 0;
                
                for (int t = 0; t < vis.SortedTds.Count; t++)
                {
                    TableData tmpTd = vis.SortedTds[t];
                    if (tmpTd.guid == vis.td.guid)
                    {
                        //tmpTd.ExtraOffset = vis.td.ExtraOffset;
                    }
                    if (vis.segmentNumber == vis.PCM.GetSegmentNumber(tmpTd.addrInt))
                    {
                        int hexAddr = (int)tmpTd.StartAddress() + vis.ExtraOffset;
                        if (hexAddr > (vis.hexDatas.Length - 1) || hexAddr < 0)
                        {
                            Debug.WriteLine("Bad table address: " + hexAddr.ToString("X"));
                            continue;
                        }
                        vis.hexDatas[hexAddr].TableText = tmpTd.TableName + ": " + tmpTd.Address;
                        if (tmpTd.Offset >= 0)
                        {
                            vis.hexDatas[hexAddr].TableText += "+" + tmpTd.Offset.ToString();
                        }
                        else
                        {
                            vis.hexDatas[hexAddr].TableText += "-" + (-1 * tmpTd.Offset).ToString();
                        }
                        if (tmpTd.ExtraOffset >= 0)
                        {
                            vis.hexDatas[hexAddr].TableText += "+" + tmpTd.ExtraOffset.ToString() ;
                        }
                        else
                        {
                            vis.hexDatas[hexAddr].TableText += "-" + (-1 * tmpTd.ExtraOffset).ToString();
                        }
                        vis.hexDatas[hexAddr].TableText += " - " + tmpTd.EndAddress().ToString("X8");
                        //hexDatas[hexAddr].TableText = hexDatas[hexAddr].TableText.PadRight((int)(numBytesPerRow.Value*4));
                        //hexAddr = (int)(tmpTd.StartAddress());
                        if (hexAddr > -1 && hexAddr < vis.hexDatas.Length)
                            vis.hexDatas[hexAddr].Prefix = "(";
                        int endAddr = (int)(tmpTd.EndAddress() + vis.ExtraOffset);
                        if (endAddr > -1 && endAddr < vis.hexDatas.Length)
                            vis.hexDatas[endAddr].Suffix = ")";

                        for (int a = hexAddr; a <= endAddr && a < vis.hexDatas.Length; a++)
                        {
                            vis.hexDatas[a].Color = colors[c];
                            vis.hexDatas[a].TdIndex = t;
                            vis.hexDatas[a].TableName = tmpTd.TableName;
                        }
                        c++;
                        if (c >= (colors.Length - 1))
                            c = 0;
                    }
                }
                vis.hexDatas[(int)vis.td.StartAddress() + vis.ExtraOffset].Prefix = "[";
                vis.hexDatas[(int)vis.td.EndAddress() + vis.ExtraOffset].Suffix = "]";
                int start = (int)vis.td.addrInt;
                int end = (int)vis.td.EndAddress() + vis.ExtraOffset;
                if (vis.td.Offset < 0)
                    start += vis.td.Offset;
                if (vis.td.ExtraOffset < 0)
                    start += vis.td.ExtraOffset;
                int OffsetEnd = (int)(vis.td.StartAddress());
                int ExtraOffsetEnd = (int)(vis.td.StartAddress() + vis.ExtraOffset);
                for (int addr = start; addr <= end && addr < vis.hexDatas.Length; addr++)
                {
                    //hexDatas[addr].Data = pcm.buf[addr];
                    vis.hexDatas[addr].SelectedTD = true;
                    if (addr >= vis.td.StartAddress() + vis.ExtraOffset && addr <= vis.td.EndAddress() + vis.ExtraOffset)
                    {
                        vis.hexDatas[addr].Color = Color.LightCoral;
                        vis.hexDatas[addr].SelectedTD = true;
                    }
                    else if (vis.td.Offset > 0 && addr >= vis.td.addrInt && addr < OffsetEnd)
                    {
                        vis.hexDatas[addr].Color = Color.Purple;
                    }
                    else if (vis.td.Offset < 0 && addr <= vis.td.addrInt && addr >= OffsetEnd)
                    {
                        vis.hexDatas[addr].Color = Color.Purple;
                    }
                    else if (vis.td.ExtraOffset > 0 && addr >= OffsetEnd && addr < ExtraOffsetEnd)
                    {
                        vis.hexDatas[addr].Color = Color.Green;
                    }
                    else if (vis.td.ExtraOffset < 0 && addr <= OffsetEnd && addr >= ExtraOffsetEnd)
                    {
                        vis.hexDatas[addr].Color = Color.Green;
                    }

                    if (addr == selectedByte)
                    {
                        vis.hexDatas[addr].Color = Color.Red;
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
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
        }

        public void ChangeSelection(uint selectedByte)
        {
            this.selectedByte = selectedByte;
            for (int r = 0; r < vis1.dgrows.Count; r++)
            {
                DGROW dgrow = vis1.dgrows[r];
                for (int c = 0; c < dgrow.Addresses.Count; c++)
                {
                    uint addr = dgrow.Addresses[c];
                    if (addr == selectedByte)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[r].Cells[c].Selected = true;
                        break;
                    }

                }
            }

        }

        public void UpdateDisplay(bool ScrollToSelected)
        {
            if (vis2.td != null)
            {                
                DisplayData(selectedByte, true);
            }
            else
            {
                splitContainer1.Panel2.Hide();
                splitContainer1.SplitterDistance = splitContainer1.Width - 5;
            }
            DisplayData(selectedByte,  false);
            if (ScrollToSelected && vis1.TdRow >= 0 && vis1.TdRow < dataGridView1.RowCount)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.TdRow;
            }
            Application.DoEvents();
        }

        public void DisplayData(uint selectedByte, bool Secondary)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            DataGridView dgv = dataGridView1;
            try
            {
                VisSettings vis;
                if (Secondary)
                {
                    vis = vis2;
                    dgv = dataGridView2;
                }
                else
                {
                    vis = vis1;
                    dgv = dataGridView1;
                }
                this.selectedByte = selectedByte;

                int start = 0;
                int start1 = 0;
                int start2 = 0;
                int end = 0;
                int end1 = 0;
                int end2 = 0;
                int bytesPerRow = (int)numBytesPerRow.Value;
                if (vis2.td != null)
                {
                    offset = (int)((vis2.td.StartAddress() + vis1.ExtraOffset) - (vis1.td.StartAddress() + vis2.ExtraOffset));
                }
                else
                {
                    start2 = (int)vis1.td.addrInt;
                    end2 = (int)vis1.td.addrInt + vis1.td.Size();
                }

                if (radioShowTable.Checked)
                {
                    start = (int)vis.td.addrInt;
                    if (vis.td.Offset < 0)
                        start += vis.td.Offset;
                    if (vis.td.ExtraOffset < 0)
                        start += vis.td.ExtraOffset;
                    end = (int)vis.td.EndAddress();

                    start = (int)(start - numExtraBytes.Value);
                    end = (int)(end + numExtraBytes.Value);
                }
                else
                {
                    start1 = (int)vis1.PCM.segmentinfos[vis1.segmentNumber].GetStartAddr();
                    end1 = (int)vis1.PCM.segmentinfos[vis1.segmentNumber].GetEndAddr();
                    if (vis2.td != null)
                    {
                        start2 = (int)(vis2.PCM.segmentinfos[vis2.segmentNumber].GetStartAddr());
                        end2 = (int)vis2.PCM.segmentinfos[vis2.segmentNumber].GetEndAddr();
                    }
                    start = Math.Min(start1, start2);
                    end = Math.Max(end1,end2);
                }
                if (start< 0)
                {
                    start = 0;
                }
                if (end > vis.PCM.fsize)
                {
                    end =(int)vis.PCM.fsize;
                }

                if (Secondary)
                {
                    start += (int)numExtraOffset2.Value;
                    if (numExtraOffset2.Value > 0)
                    {
                        end += (int)numExtraOffset2.Value;
                    }
                    Debug.WriteLine("Secondary: Start: " + start.ToString() +", end: " + end.ToString() +", Offset: " + offset.ToString());
                }
                else
                {
                    start += (int)numExtraOffset1.Value;
                    if (numExtraOffset1.Value > 0)
                    {
                        end += (int)numExtraOffset1.Value;
                    }
                    Debug.WriteLine("Primary: Start: " + start.ToString() + ", end: " + end.ToString());
                }
                Debug.WriteLine("Segment start: " + vis.segmentstart + ", Segment End: " + vis.segmentend);
                Debug.WriteLine("Addrint: " + vis.td.addrInt.ToString() + ", Table start: " + vis.td.StartAddress().ToString());

                DrawingControl.SuspendDrawing(dgv);

                int col = 0;
                int row = 0;
                vis.dgrows.Clear();
                DGROW dgrow = new DGROW();
                vis.dgrows.Add(dgrow);
                for (uint addr = (uint)start; addr <= end && addr < vis.PCM.fsize; addr++)
                {
                    if (string.IsNullOrEmpty(vis.hexDatas[addr].Prefix))
                        vis.hexDatas[addr].Prefix = " ";
                    if (string.IsNullOrEmpty(vis.hexDatas[addr].Suffix))
                        vis.hexDatas[addr].Suffix = " ";

                    if (radioSegmentTBNames.Checked && !string.IsNullOrEmpty(vis.hexDatas[addr].TableText))
                    {
                        dgrow = new DGROW();
                        dgrow.HeaderTxt = vis.hexDatas[addr].TableText;
                        col = 0;
                        row++;
                        vis.dgrows.Add(dgrow);
                    }
                    if (addr == vis.td.addrInt)
                    {
                        vis.TdRow = row;
                    }
                    if (col >= bytesPerRow)
                    {

                        row++;
                        col = 0;
                        dgrow = new DGROW();
                        vis.dgrows.Add(dgrow);
                    }
                    if (string.IsNullOrEmpty(dgrow.HeaderTxt))
                    {
                        dgrow.HeaderTxt = addr.ToString("X6");
                    }
                    dgrow.Cols.Add(vis.PCM.buf[addr].ToString("X2"));
                    dgrow.Addresses.Add(addr);
                    col++;
                }
                Debug.WriteLine("Generate DG data time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
                Application.DoEvents();
                dgv.RowCount = vis.dgrows.Count;
                dgv.Refresh();
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
            DrawingControl.ResumeDrawing(dgv);
            timer.Stop();
            Debug.WriteLine("Displaydata time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

        }

        private void radioShowSegment_CheckedChanged(object sender, EventArgs e)
        {
            if (radioShowSegment.Checked)
            {
                dataGridView1.RowHeadersWidth = 80;
                dataGridView2.RowHeadersWidth = 80;
                UpdateDisplay(true);
            }
        }

        private void numExtraBytes_ValueChanged(object sender, EventArgs e)
        {
            UpdateDisplay(false);
        }

        private void numBytesPerRow_ValueChanged(object sender, EventArgs e)
        {
            SetupDatagrids();
            UpdateDisplay(true);
        }

        private void radioSegmentTBNames_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSegmentTBNames.Checked)
            {
                dataGridView1.RowHeadersWidth = 350;
                dataGridView2.RowHeadersWidth = 350;
                UpdateDisplay(true);
            }
        }

        private void radioShowTable_CheckedChanged(object sender, EventArgs e)
        {
            if (radioShowTable.Checked)
            {
                dataGridView1.RowHeadersWidth = 80;
                dataGridView2.RowHeadersWidth = 80;
                UpdateDisplay(true);
            }
        }

        private void numExtraOffset1_ValueChanged(object sender, EventArgs e)
        {
            //vis1.td.ExtraOffset = (int)numExtraOffset1.Value;
            vis1.ExtraOffset = (int)numExtraOffset1.Value;
            CreateHexData(ref vis1);
            if (chkCopyColors1.Checked)
            {
                CopyColors1();
            }
            DisplayData(selectedByte, false);
            SyncSelection1(false);
        }

        private void numExtraOffset2_ValueChanged(object sender, EventArgs e)
        {
            //vis2.td.ExtraOffset = (int)numExtraOffset2.Value;
            vis2.ExtraOffset = (int)numExtraOffset2.Value;
            if (chkCopyColors1.Checked)
            {
                CopyColors1();
            }
            else
            {
                CreateHexData(ref vis2);
            }
            DisplayData(selectedByte, true);
            SyncSelection1(false);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void btnApplyPrimary_Click(object sender, EventArgs e)
        {
            vis1.tdOrg.ExtraOffset = (int)numExtraOffset1.Value;
            if (tuner != null)
            {
                //tuner.RefreshTablelist();
                tuner.RefreshFast();
            }
        }

        private void btnApplySecondary_Click(object sender, EventArgs e)
        {
            vis2.tdOrg.ExtraOffset = (int)numExtraOffset2.Value;
            if (tuner != null)
            {
                //tuner.RefreshTablelist();
                tuner.RefreshFast();
            }

        }

        private void btnPrevTable_Click(object sender, EventArgs e)
        {
            int x = FindTableDataId(vis1.td, vis1.PCM.tableDatas);
            x--;
            if (x >-1)
            {
                TableData td1 = vis1.PCM.tableDatas[x];
                vis1.ChangeTd(vis1.PCM, td1);
                if (vis2.td != null)
                {
                    TableData td2 = FindTableData(vis1.td, vis2.PCM.tableDatas);
                    vis2.ChangeTd(vis2.PCM, td2);
                }
                ShowTables(selectedByte);
            }
        }

        private void btnNextTable_Click(object sender, EventArgs e)
        {
            int x = FindTableDataId(vis1.td, vis1.PCM.tableDatas);
            x++;
            if (x > -1)
            {
                TableData td1 = vis1.PCM.tableDatas[x];
                vis1.ChangeTd(vis1.PCM, td1);
                if (vis2.td != null)
                {
                    TableData td2 = FindTableData(vis1.td, vis2.PCM.tableDatas);
                    vis2.ChangeTd(vis2.PCM, td2);
                }
                ShowTables(selectedByte);
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
                ShowTables(selectedByte);
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
                ShowTables(selectedByte);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnSelStart1_Click(object sender, EventArgs e)
        {
            if (vis1.SelStart > -1 && vis1.SelStart < dataGridView1.Rows.Count)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.SelStart;
            }
        }

        private void btnSelEnd1_Click(object sender, EventArgs e)
        {
            if (vis1.SelEnd < dataGridView1.Rows.Count)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.SelEnd;
            }
        }

        private void btnSelStart2_Click(object sender, EventArgs e)
        {
            if (vis2.SelStart > -1 && vis2.SelStart < dataGridView2.Rows.Count)
            {
                dataGridView2.FirstDisplayedScrollingRowIndex = vis2.SelStart;
            }
        }

        private void btnSelEnd2_Click(object sender, EventArgs e)
        {
            if (vis2.SelEnd < dataGridView2.Rows.Count)
            {
                dataGridView2.FirstDisplayedScrollingRowIndex = vis2.SelEnd;
            }
        }

        private void btnApplytoRight_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> selTables = GetSelectedTables(false);
                for (int t = 0; t < selTables.Count; t++)
                {
                    TableData tdR = FindTableData(selTables[t], vis2.SortedTds);
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
                ShowTables(selectedByte);
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
                    TableData tdL = FindTableData(selTables[t], vis1.SortedTds);
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
                ShowTables(selectedByte);
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

        private void CopyColors1()
        {
            /*
            for (int i=0;i< vis1.hexDatas.Length;i++)
            {
                if ((i+offset) > 0 && (i+offset)< vis2.PCM.fsize)
                    vis2.hexDatas[i+offset] = vis1.hexDatas[i];
            }
            */
            vis2.hexDatas = (HexData[])vis1.hexDatas.Clone();
            UpdateDisplay(false);
        }

        private void CopyColors2()
        {
            for (int i = 0; i < vis2.hexDatas.Length; i++)
            {
                if ((i - offset) > 0 && (i - offset) < vis1.PCM.fsize)
                    vis1.hexDatas[i - offset] = vis2.hexDatas[i];
            }
            //UpdateDisplay(false);
        }

        private void btnScrollToSelected_Click(object sender, EventArgs e)
        {
            if (vis1.TdRow >= 0 && vis1.TdRow < dataGridView1.RowCount)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.TdRow;
            }
        }

        private void btnScrollToSelected2_Click(object sender, EventArgs e)
        {
            if (vis2.TdRow >= 0 && vis2.TdRow < dataGridView2.RowCount)
            {
                dataGridView2.FirstDisplayedScrollingRowIndex = vis2.TdRow;
            }
        }

        private void btnCreateTable1_Click(object sender, EventArgs e)
        {
            try
            {
                int Start = -1;
                int End = -1;

                if (dataGridView1.SelectedCells.Count > 0)
                {
                    int r = dataGridView1.SelectedCells[0].RowIndex;
                    int c = dataGridView1.SelectedCells[0].ColumnIndex;
                    if (c < vis1.dgrows[r].Addresses.Count)
                    {
                        Start = (int)vis1.dgrows[r].Addresses[c];
                    }
                    r = dataGridView1.SelectedCells[dataGridView1.SelectedCells.Count - 1].RowIndex;
                    c = dataGridView1.SelectedCells[dataGridView1.SelectedCells.Count - 1].ColumnIndex;
                    if (c < vis1.dgrows[r].Addresses.Count)
                    {
                        End = (int)vis1.dgrows[r].Addresses[c];
                    }
                }
                if (End < Start)
                {
                    int tmp = Start;
                    Start = End;
                    End = tmp;
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
                    vis1.PCM.tableDatas.Add(newTd);
                    ShowTables(selectedByte);
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
        }

        private void btnCreateTable2_Click(object sender, EventArgs e)
        {
            try
            { 
                int Start = -1;
                int End = -1;

                if (dataGridView2.SelectedCells.Count > 0)
                {
                    int r = dataGridView2.SelectedCells[0].RowIndex;
                    int c = dataGridView2.SelectedCells[0].ColumnIndex;
                    if (c < vis2.dgrows[r].Addresses.Count)
                    {
                        Start = (int)vis2.dgrows[r].Addresses[c];
                    }
                    r = dataGridView2.SelectedCells[dataGridView2.SelectedCells.Count - 1].RowIndex;
                    c = dataGridView2.SelectedCells[dataGridView2.SelectedCells.Count - 1].ColumnIndex;
                    if (c < vis2.dgrows[r].Addresses.Count)
                    {
                        End = (int)vis2.dgrows[r].Addresses[c];
                    }
                }
                if (End < Start)
                {
                    int tmp = Start;
                    Start = End;
                    End = tmp;
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
                    vis2.PCM.tableDatas.Add(newTd);
                    ShowTables(selectedByte);
                }
                fte.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }

        }

        private void SearchCells(List<byte> searchBytes)
        {
            ClearSearch();
            Debug.Write("Searching: ");
            for (int i=0;i<searchBytes.Count;i++)
            {
                Debug.Write(searchBytes[i].ToString("X2") + " ");
            }
            Debug.WriteLine("");
            for (uint a = vis1.segmentstart; a < vis1.segmentend; a++)
            {
                bool found = true;
                for (int a2 = 0; a2 < searchBytes.Count; a2++)
                {
                    if (vis1.PCM.buf[a + a2] != searchBytes[a2])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    vis1.foundLocations.Add((uint)a);
                    listBox1.Items.Add(a.ToString("X10"));
                    for (uint b = a; b < a + searchBytes.Count; b++)
                    {
                        vis1.foundBytes.Add(b);
                    }
                }
            }
            for (int r = 0; r < vis1.dgrows.Count; r++)
            {
                DGROW dgrow = vis1.dgrows[r];
                for (int c = 0; c < dgrow.Addresses.Count; c++)
                {
                    uint addr = dgrow.Addresses[c];
                    if (vis1.foundBytes.Contains(addr))
                    {
                        if (!vis1.searchedRows.Contains(r))
                        {
                            vis1.searchedRows.Add(r);
                        }
                    }
                }
            }
            //dataGridView1.Invalidate(true);
            //dataGridView1.Refresh();
            
            for (uint a = vis2.segmentstart; a < vis2.segmentend; a++)
            {
                bool found = true;
                for (int a2 = 0; a2 < searchBytes.Count; a2++)
                {
                    if (vis2.PCM.buf[a + a2] != searchBytes[a2])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    vis2.foundLocations.Add((uint)a);
                    listBox2.Items.Add(a.ToString("X10"));
                    for (uint b = a; b < a + searchBytes.Count; b++)
                    {
                        vis2.foundBytes.Add(b);
                    }
                }
            }
            for (int r = 0; r < vis2.dgrows.Count; r++)
            {
                DGROW dgrow = vis2.dgrows[r];
                for (int c = 0; c < dgrow.Addresses.Count; c++)
                {
                    uint addr = dgrow.Addresses[c];
                    if (vis2.foundBytes.Contains(addr))
                    {
                        if (!vis2.searchedRows.Contains(r))
                        {
                            vis2.searchedRows.Add(r);
                        }
                    }
                }
            }
            //dataGridView2.Invalidate(true);
            //dataGridView2.Refresh();
            txtInfo1.AppendText("Found in: " + vis1.foundLocations.Count.ToString() + " locations" + Environment.NewLine);
            txtInfo2.AppendText("Found in: " + vis2.foundLocations.Count.ToString() + " locations" + Environment.NewLine);

        }
        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<uint> sdc = new List<uint>();
            foreach (DataGridViewCell dgc in dataGridView1.SelectedCells)
            {
                DGROW dgrow = vis1.dgrows[dgc.RowIndex];                
                sdc.Add(dgrow.Addresses[dgc.ColumnIndex]);
            }
            sdc.Sort();
            List<byte> searchBytes = new List<byte>();
            foreach (uint a in sdc)
            {
                searchBytes.Add(vis1.PCM.buf[a]);
            }
            SearchCells(searchBytes);
        }

        private void searchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<uint> sdc = new List<uint>();
            foreach (DataGridViewCell dgc in dataGridView2.SelectedCells)
            {
                DGROW dgrow = vis2.dgrows[dgc.RowIndex];
                sdc.Add(dgrow.Addresses[dgc.ColumnIndex]);
            }
            sdc.Sort();
            List<byte> searchBytes = new List<byte>();
            foreach (uint a in sdc)
            {
                searchBytes.Add(vis2.PCM.buf[a]);
            }
            SearchCells(searchBytes);
        }
        private void ClearSearch()
        {
            vis1.ClearSearch();
            vis2.ClearSearch();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            dataGridView1.Refresh();
            dataGridView2.Refresh();
        }
        private void clearSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void clearSearchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void showFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vis1.searchedRows.Count > 0)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[0];
            }
        }

        private void showPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vis1.currentSearched > 0)
            {
                vis1.currentSearched--;
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[vis1.currentSearched];
            }
        }

        private void showNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vis1.currentSearched < vis1.searchedRows.Count -1)
            {
                vis1.currentSearched++;
                dataGridView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[vis1.currentSearched];
            }

        }

        private void showLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            vis1.currentSearched = vis1.searchedRows.Count - 1;
            dataGridView1.FirstDisplayedScrollingRowIndex = vis1.searchedRows[vis1.currentSearched];
        }

        private void showFirstToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2.searchedRows.Count > 0)
            {
                dataGridView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[0];
            }

        }

        private void showPreviousToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2.currentSearched > 0)
            {
                vis2.currentSearched--;
                dataGridView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[vis2.currentSearched];
            }

        }

        private void showNextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (vis2.currentSearched < vis2.searchedRows.Count - 1)
            {
                vis2.currentSearched++;
                dataGridView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[vis2.currentSearched];
            }

        }

        private void showLastToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            vis2.currentSearched = vis2.searchedRows.Count - 1;
            dataGridView2.FirstDisplayedScrollingRowIndex = vis2.searchedRows[vis2.currentSearched];

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int addr;
            if (!HexToInt(listBox1.SelectedItem.ToString(), out addr))
            {
                return;
            }
            for (int r = 0; r <vis1.dgrows.Count; r++)
            {
                DGROW dgrow = vis1.dgrows[r];
                if (dgrow.Addresses[0] >= addr)
                {
                    if (r == 0) r = 1;
                    dataGridView1.FirstDisplayedScrollingRowIndex = r - 1;
                    break;
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int addr;
            if (!HexToInt(listBox2.SelectedItem.ToString(), out addr))
            {
                return;
            }
            for (int r = 0; r < vis2.dgrows.Count; r++)
            {
                DGROW dgrow = vis2.dgrows[r];
                if (dgrow.Addresses[0] >= addr)
                {
                    if (r == 0) r = 1;
                    dataGridView2.FirstDisplayedScrollingRowIndex = r - 1;
                    break;
                }
            }
        }

        private void chkCopyColors1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCopyColors1.Checked)
            {
                CopyColors1();
            }
            else
            {
                CreateHexData(ref vis2);
            }
        }
    }
}
