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
            if (PCM2 != null)
            {
                vis2 = new VisSettings(PCM2, td2, false);
            }
        }

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
                    this.td = td.ShallowCopy(false);
                    this.tdOrg = td;
                    int seg = PCM.GetSegmentNumber(td.addrInt);
                    if (seg > -1 && seg != segmentNumber)
                    {
                        segmentNumber = seg;
                        segmentName = PCM.Segments[seg].Name;
                        segmentstart = PCM.segmentinfos[seg].GetStartAddr();
                        segmentend = PCM.segmentinfos[seg].GetEndAddr();
                    }
                }
            }
/*            public HexData GetHexDataByAddress(int Address)
            {
                int idx = hexDataAddresses.IndexOf(Address);
                if (idx < 0)
                {
                    HexData hxd = new HexData();
                    hexDatas.Add(hxd);
                    hexDataAddresses.Add(Address);
                    return hxd;
                }
                else
                {
                    return hexDatas[idx];
                }
            }
*/
            public void FilterSegmentTables()
            {
                SegmentTds = new List<TableData>();
                for (int t = 0; t < SortedTds.Count; t++)
                {
                    TableData tmpTd = SortedTds[t];
                    if (segmentNumber == PCM.GetSegmentNumber((uint)(tmpTd.StartAddress())))
                    {
                        SegmentTds.Add(tmpTd);
                    }
                }
            }
            public PcmFile PCM;
            public TableData td { get; internal set; }
            public TableData tdOrg;
            public HexData[] hexDatas;
            public int buffOffset;
            public int ExtraOffset;
            public List<TableData> SortedTds;
            public List<TableData> SegmentTds;
            public int SelStart = int.MaxValue;
            public int SelEnd = -1;
            public int TdRow = -1;
            public uint segmentstart = 0;
            public uint segmentend = 0;
            public int segmentNumber = -1;
            public string segmentName = "";
            public List<int> searchedRows = new List<int>();
            public List<uint> foundLocations = new List<uint>();
            public List<uint> foundBytes = new List<uint>();
            public int currentSearched = 0;
            public List<DGROW> dgrows = new List<DGROW>();
            public bool Primary { get; internal set; }
            public DataGridViewCell mouseDownCell { get; set; }
            public DataGridViewCell mouseUpCell { get; set; }
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
            public char Prefix;
            public char Suffix;
            public Color Color;
            public int TdIndex;
            public bool SelectedTD;
            public int Row;
            public int Col;
        }
        private enum CopyColors
        {
            Generate,
            Copy,
            Freeze
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
        //private int offset = 0;
        public VisSettings vis1;
        public VisSettings vis2;
        private int cellCount = 0;
        private CopyColors leftColors = CopyColors.Generate;
        private CopyColors rightColors = CopyColors.Generate;
        private bool SelectionModified = false;

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
            dataGridView1.VirtualMode = true;
            dataGridView2.VirtualMode = true;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
            dataGridView2.KeyDown += DataGridView2_KeyDown;
            this.KeyPreview = true;
            this.KeyDown += FrmTableVisDouble_KeyDown;
            radioShowSegment.Text = "Segment [" + vis1.segmentName + "]";
            labelFileName1.Text = vis1.PCM.FileName;
            if (vis2 != null && vis2.td != null)
            {
                labelFileName2.Text = vis2.PCM.FileName;
            }
            else
            {
                chkSyncScroll.Checked = false;
                chkSyncScroll.Enabled = false;
                btnApplytoRight.Enabled = false;
            }
            comboCopyColorsLeft.ValueMember = "Value";
            comboCopyColorsLeft.DisplayMember = "Name";
            comboCopyColorsLeft.DataSource = Enum.GetValues(typeof(CopyColors)).Cast<object>().Select(v => new
            {
                Value = v,
                Name = v.ToString()
            }).ToList();
            comboCopyColorsRight.Text = CopyColors.Generate.ToString();
            comboCopyColorsRight.ValueMember = "Value";
            comboCopyColorsRight.DisplayMember = "Name";
            comboCopyColorsRight.DataSource = Enum.GetValues(typeof(CopyColors)).Cast<object>().Select(v => new
            {
                Value = v,
                Name = v.ToString()
            }).ToList();
            comboCopyColorsRight.Text = CopyColors.Generate.ToString();
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.C && e.Control)
                {
                    // copy logic
                    StringBuilder sb = new StringBuilder();
                    List<uint> SelectAddresses = GetSelectedAddresses(dataGridView1, vis1);
                    foreach (uint addr in SelectAddresses)
                    {
                        sb.Append(vis1.PCM.buf[addr].ToString("X2") + " ");
                    }
                    Clipboard.SetDataObject(sb.ToString().Trim());
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.ShiftKey)
                {
                    vis1.mouseDownCell = dataGridView1.CurrentCell;
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
        private void DataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.C && e.Control)
                {
                    // copy logic
                    StringBuilder sb = new StringBuilder();
                    List<uint> SelectAddresses = GetSelectedAddresses(dataGridView2, vis2);
                    foreach (uint addr in SelectAddresses)
                    {
                        sb.Append(vis2.PCM.buf[addr].ToString("X2") + " ");
                    }
                    Clipboard.SetDataObject(sb.ToString().Trim());
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.ShiftKey)
                {
                    vis2.mouseDownCell = dataGridView2.CurrentCell;
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

        private void DataGridView2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                vis2.mouseUpCell = dataGridView2.CurrentCell;
                ModifySelection(dataGridView2, vis2);
                SyncSelection(dataGridView2, dataGridView1, vis2, vis1);
            }

        }

        private void DataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            
            Debug.WriteLine("Key up in " + dataGridView1.CurrentCell.RowIndex.ToString() + ", " + dataGridView1.CurrentCell.ColumnIndex.ToString());
            if (e.KeyCode == Keys.ShiftKey)
            {
                vis1.mouseUpCell = dataGridView1.CurrentCell;
                ModifySelection(dataGridView1, vis1);
                SyncSelection(dataGridView1, dataGridView2, vis1, vis2);
            }
        }

        private void DataGridView2_MouseUp(object sender, MouseEventArgs e)
        {            
            SyncSelection(dataGridView2,dataGridView1,vis2,vis1);
        }

        private void DataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            
            //Debug.WriteLine("Mouse up in " + dataGridView1.CurrentCell.RowIndex.ToString() + ", " + dataGridView1.CurrentCell.ColumnIndex.ToString());
            //ModifySelection(dataGridView1, vis1);
            //SyncSelection(dataGridView1,dataGridView2,vis1,vis2);
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
                bool found = false;
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
                            HexData hxd = vis1.hexDatas[addr - vis1.buffOffset];
                            e.Value = hxd.Prefix + vis1.PCM.buf[addr].ToString("X2") + hxd.Suffix;
                            dataGridView1.Rows[r].Cells[c].Style.ForeColor = hxd.Color;
                            dataGridView1.Rows[r].Cells[c].ToolTipText = hxd.TableName;
                            cellCount++;
                            found = true;
                        }
                    }
                }
                if (!found)
                {
                    e.Value = null;
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
                        int addr = (int)dgrow.Addresses[c];
                        if (c == 0)
                        {
                            dataGridView2.Rows[r].HeaderCell.Value = dgrow.HeaderTxt;
                        }
                        HexData hxd = vis2.hexDatas[addr - vis2.buffOffset];
                        e.Value = hxd.Prefix + vis2.PCM.buf[addr].ToString("X2") + hxd.Suffix;
                        dataGridView2.Rows[r].Cells[c].ToolTipText = hxd.TableName;
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
            try
            {
                int r = e.RowIndex;
                int c = e.ColumnIndex;
                if (r >= 0 && c >= 0)
                {
                    e.CellStyle.BackColor = Color.White;
                    if (c < vis1.dgrows[r].Cols.Count)
                    {
                        DGROW dgrow = vis1.dgrows[r];
                        uint addr = dgrow.Addresses[c];
                        HexData hxd = vis1.hexDatas[addr - vis1.buffOffset];
                        dataGridView1.Rows[r].Cells[c].Style.ForeColor = hxd.Color;
                        if (vis1.foundBytes.Contains(addr))
                        {
                            e.CellStyle.BackColor = Color.Yellow;
                        }
                        if (hxd.SelectedTD)
                        {
                            e.CellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                            dataGridView1.Rows[r].HeaderCell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                        }
                        if (c == 0 && radioSegmentTBNames.Checked)
                        {
                            dataGridView1.Rows[r].HeaderCell.Style.ForeColor = hxd.Color;
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
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
        }
        private void DataGridView2_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
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
                        HexData hxd = vis2.hexDatas[(int)addr - vis2.buffOffset];
                        dataGridView2.Rows[r].Cells[c].Style.ForeColor = hxd.Color;
                        if (vis2.foundBytes.Contains(addr))
                        {
                            e.CellStyle.BackColor = Color.Yellow;
                        }
                        if (hxd.SelectedTD)
                        {
                            e.CellStyle.Font = new Font(dataGridView2.Font, FontStyle.Bold);
                            dataGridView2.Rows[r].HeaderCell.Style.Font = new Font(dataGridView2.Font, FontStyle.Bold);
                        }
                        if (c == 0 && radioSegmentTBNames.Checked)
                        {
                            dataGridView2.Rows[r].HeaderCell.Style.ForeColor = hxd.Color;
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
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
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
        private void ModifySelection(DataGridView dgv, VisSettings vis)
        {
            try
            {
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                dataGridView2.SelectionChanged -= DataGridView2_SelectionChanged;

                if (vis.mouseDownCell == null || vis.mouseUpCell == null)
                {
                    return;
                }

                SelectionModified = true;

                if (dgv.SelectedCells.Count == 0)
                {
                    return;
                }
                DataGridViewCell firstCell = vis.mouseDownCell;
                DataGridViewCell lastCell = vis.mouseUpCell; //dgv.CurrentCell;
                if (firstCell.RowIndex > lastCell.RowIndex || (firstCell.RowIndex == lastCell.RowIndex && firstCell.ColumnIndex > lastCell.ColumnIndex))
                {
                    Debug.WriteLine("Swapping cells");
                    DataGridViewCell tmpCell = lastCell;
                    lastCell = firstCell;
                    firstCell = tmpCell;
                }

                Debug.WriteLine("Modify selection, first cell: " + firstCell.RowIndex.ToString() + "," + firstCell.ColumnIndex.ToString() +
                    ", last cell: " + lastCell.RowIndex.ToString() + "," + lastCell.ColumnIndex.ToString());
                dgv.ClearSelection();

                if (firstCell.RowIndex < lastCell.RowIndex)
                {
                    //First row
                    for (int c = firstCell.ColumnIndex; c < dgv.ColumnCount; c++)
                    {
                        dgv.Rows[firstCell.RowIndex].Cells[c].Selected = true;
                    }
                    //"Middle" rows
                    for (int r = firstCell.RowIndex + 1; r < lastCell.RowIndex; r++)
                    {
                        for (int c = 0; c < dgv.ColumnCount; c++)
                        {
                            dgv.Rows[r].Cells[c].Selected = true;
                        }
                    }
                    //Last row
                    for (int c = 0; c <= lastCell.ColumnIndex; c++)
                    {
                        dgv.Rows[lastCell.RowIndex].Cells[c].Selected = true;
                    }
                }
                else
                {
                    //One row only
                    for (int c = firstCell.ColumnIndex; c <= lastCell.ColumnIndex; c++)
                    {
                        dgv.Rows[firstCell.RowIndex].Cells[c].Selected = true;
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
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;

        }
        private List<uint> GetSelectedAddresses(DataGridView Dgv, VisSettings vis)
        {
            List<uint> selectedAddresses = new List<uint>();
            for (int x = 0; x < Dgv.SelectedCells.Count; x++)
            {
                DGROW dgrow = vis.dgrows[Dgv.SelectedCells[x].RowIndex];
                if (Dgv.SelectedCells[x].ColumnIndex < dgrow.Addresses.Count)
                {
                    selectedAddresses.Add((uint)(dgrow.Addresses[Dgv.SelectedCells[x].ColumnIndex]));
                }
            }
            selectedAddresses.Sort();
            return selectedAddresses;
        }
        private void RestoreSelection(List<uint> SelectedAddresses)
        {
            dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged -= DataGridView2_SelectionChanged;
            try
            {
                dataGridView1.ClearSelection();
                foreach (uint a in SelectedAddresses)
                {
                    int buffAddr = (int)(a - vis1.buffOffset);
                    if (buffAddr > 0 && buffAddr < vis1.hexDatas.Length)
                    {
                        dataGridView1.Rows[vis1.hexDatas[buffAddr].Row].Cells[vis1.hexDatas[buffAddr].Col].Selected = true;
                    }
                }
                SyncSelection(dataGridView1, dataGridView2, vis1, vis2);
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
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
        }
        private void SyncSelection(DataGridView DgvSrc, DataGridView DgvDst, VisSettings VisSrc, VisSettings VisDst)
        {
            if (!SelectionModified)
            {
                return;
            }
            SelectionModified = false;
            dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged -= DataGridView2_SelectionChanged;
            dataGridView1.Scroll -= DataGridView1_Scroll;
            dataGridView2.Scroll -= DataGridView2_Scroll;
            try
            {
                Debug.WriteLine("SyncSelection");
                DgvDst.ClearSelection();
                Application.DoEvents();
                if (DgvSrc.SelectedCells.Count > 0)
                {
                    List<uint> selectedAddresses = GetSelectedAddresses(DgvSrc, VisSrc);
                    int scrollRow = -2;
                    uint scrollAddr = VisSrc.dgrows[DgvSrc.FirstDisplayedScrollingRowIndex].Addresses[0];
                    uint scrollAddr2 = (uint)(scrollAddr + (VisDst.ExtraOffset - VisSrc.ExtraOffset));
                    for (int r = 0; r < selectedAddresses.Count; r++)
                    {
                        uint addr = (uint)(selectedAddresses[r] + VisDst.ExtraOffset - VisSrc.ExtraOffset);
                        int buffAddr = (int)(addr - VisDst.buffOffset);
                        if (buffAddr >= 0 && buffAddr < VisDst.hexDatas.Length)
                        {
                            int row = VisDst.hexDatas[buffAddr].Row;
                            int col = VisDst.hexDatas[buffAddr].Col;
                            //Debug.WriteLine("Left selected addr " + addr.ToString("X") + ", row " + row.ToString() + " col " + col.ToString());
                            DgvDst.Rows[row].Cells[col].Selected = true;
                            if (addr == scrollAddr2)
                            {
                                scrollRow = row;
                            }
                        }
                    }
                    if (scrollRow >= 0)
                    {
                        DgvDst.FirstDisplayedScrollingRowIndex = scrollRow;
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
                LoggerBold("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
            dataGridView1.Scroll += DataGridView1_Scroll;
            dataGridView2.Scroll += DataGridView2_Scroll;
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
        }
  
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("DataGridView1_SelectionChanged");
            //ModifySelection(dataGridView1, vis1);            
        }
        private void DataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("DataGridView2_SelectionChanged");
            //ModifySelection(dataGridView2, vis2);
        }

        private void SyncScroll1()
        {
            Debug.WriteLine("SyncScroll1");
            dataGridView2.Scroll -= DataGridView2_Scroll;
            try
            {
                if (chkSyncScroll.Checked)
                {
                    uint scrollAddr = vis1.dgrows[dataGridView1.FirstDisplayedScrollingRowIndex].Addresses[0];
                    int scrollAddr2 = (int)(scrollAddr - (vis1.ExtraOffset - vis2.ExtraOffset));
                    int buffAddr2 = (int)(scrollAddr2 - vis2.buffOffset);
                    if (buffAddr2 >= 0 && buffAddr2 < vis2.hexDatas.Length)
                    {
                        int scrollRow = vis2.hexDatas[buffAddr2].Row;
                        dataGridView2.FirstDisplayedScrollingRowIndex = scrollRow;
                    }
                }
            }
            catch { }
            dataGridView2.Scroll += DataGridView2_Scroll;
        }
        private void SyncScroll2()
        {
            Debug.WriteLine("SyncScroll2");
            dataGridView1.Scroll -= DataGridView1_Scroll;
            try
            {
                if (chkSyncScroll.Checked)
                {
                    uint scrollAddr = vis2.dgrows[dataGridView2.FirstDisplayedScrollingRowIndex].Addresses[0];
                    int scrollAddr1 = (int)(scrollAddr - (vis2.ExtraOffset - vis1.ExtraOffset));
                    int buffAddr1 = (int)(scrollAddr1 - vis1.buffOffset);
                    if (buffAddr1 >= 0 && buffAddr1 < vis2.hexDatas.Length)
                    {
                        int scrollRow = vis1.hexDatas[buffAddr1].Row;
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
        private HexData[] CopyHexData(HexData[] Source, int EOffset)
        {
            HexData[] retVal = new HexData[Source.Length];
            for (int a = 0; a < Source.Length; a++)
            {
                int b = a + EOffset;
                if (b >= 0 && b < retVal.Length)
                {
                    retVal[b] = Source[a];
                }
            }
            return retVal;
        }

        private Block GetBufferSize(VisSettings vis)
        {
            int bufStart = (int)vis.PCM.segmentinfos[vis.segmentNumber].GetStartAddr();
            int bufEnd = (int)vis.PCM.segmentinfos[vis.segmentNumber].GetEndAddr();
            if (vis.ExtraOffset < 0)
            {
                //bufStart += vis.ExtraOffset;
                if (radioShowTable.Checked && bufStart > (vis.td.StartAddressNoExtra() + vis.ExtraOffset - numFrontBytes.Value))
                {
                    bufStart = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset - numFrontBytes.Value);
                }
                else if (bufStart > (vis.td.StartAddressNoExtra() + vis.ExtraOffset))
                {
                    bufStart = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset);
                }
            }
            else if(radioShowTable.Checked && bufStart > (vis.td.StartAddressNoExtra() - numFrontBytes.Value))
            {
                bufStart = (int)(vis.td.StartAddressNoExtra() - numFrontBytes.Value);
            }
            if (vis.ExtraOffset > 0)
            {
                if ((vis.td.addrInt - vis.ExtraOffset) < bufStart)
                {
                    //Example: Table address: 0x4000, segment start 0x4000, other side must scroll to 0x4000 - extraoffset to match 
                    bufStart = (int)(vis.td.addrInt - vis.ExtraOffset);
                }
                //bufEnd += vis.ExtraOffset;
                if (radioShowTable.Checked && (vis.td.EndAddressNoExtra() + vis.ExtraOffset + numAfterBytes.Value) > bufEnd)
                {
                    bufEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset + numAfterBytes.Value);
                }
                else if ((vis.td.EndAddressNoExtra() + vis.ExtraOffset) > bufEnd)
                {
                    bufEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset);
                }
            }
            if ((vis.td.StartAddressNoExtra() + vis.ExtraOffset) < bufStart)
            {
                bufStart = (int)(vis.td.StartAddress() + vis.ExtraOffset);
            }
            if ((vis.td.EndAddressNoExtra() + vis.ExtraOffset) > bufEnd)
            {
                bufEnd = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset);
            }
            if (bufEnd > vis.PCM.buf.Length)
            {
                bufEnd = vis.PCM.buf.Length;
            }
            if (bufStart < 0)
            {
                bufStart = 0;
            }
            Block b = new Block();
            b.Start = (uint)bufStart;
            b.End = (uint)bufEnd;
            return b;
        }
        private void CreateHexDatas()
        {
            try
            {
                Debug.WriteLine("CreateHexDatas");
                int bufStart, bufEnd;
                Block b1, b2;

                b1 = GetBufferSize(vis1);
                if (vis2 != null && vis2.td != null)
                {
                    b2 = GetBufferSize(vis2);
                    bufStart = (int)Math.Min(b1.Start, b2.Start);
                    bufEnd = (int)Math.Max(b1.End, b2.End);
                }
                else
                {
                    bufStart = (int)b1.Start;
                    bufEnd = (int)b1.End;
                }


                if (leftColors == CopyColors.Generate)
                {
                    CreateHexData(ref vis1, bufStart, bufEnd);
                }
                if (vis2 != null && vis2.td != null)
                {
                    if (rightColors == CopyColors.Generate)
                    {
                        CreateHexData(ref vis2, bufStart, bufEnd);
                    }
                    else if (rightColors == CopyColors.Copy)
                    {
                        vis2.hexDatas = CopyHexData(vis1.hexDatas, vis2.ExtraOffset);
                        vis2.buffOffset = vis1.buffOffset;
                    }
                }
                if (leftColors == CopyColors.Copy)
                {
                    vis1.hexDatas = CopyHexData(vis2.hexDatas, vis1.ExtraOffset);
                    vis1.buffOffset = vis2.buffOffset;
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
        private void CreateHexData(ref VisSettings vis, int bufStart, int bufEnd)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                vis.buffOffset = bufStart;
                vis.hexDatas = new HexData[bufEnd - bufStart + 1];
                int c = 0;
                vis.FilterSegmentTables();
                for (int t = 0; t < vis.SegmentTds.Count; t++)
                {
                    TableData tmpTd = vis.SegmentTds[t];
                    int hexAddr = (int)tmpTd.StartAddress();
                    int buffAddr = hexAddr - vis.buffOffset;
                    if (buffAddr < 0 || buffAddr >= vis.hexDatas.Length)
                    {
                        Debug.WriteLine("Address out of buffer: " + buffAddr.ToString());
                        continue;
                    }
                    vis.hexDatas[buffAddr].TableText = tmpTd.TableName + ": " + tmpTd.Address;
                    vis.hexDatas[buffAddr].TableText += "+" + tmpTd.Offset.ToString();
                    vis.hexDatas[buffAddr].TableText += "+" + tmpTd.extraoffset;
                    vis.hexDatas[buffAddr].TableText += " - " + tmpTd.EndAddress().ToString("X8");
                    vis.hexDatas[buffAddr].Prefix = '(';
                    int endAddr = (int)tmpTd.EndAddress();
                    vis.hexDatas[endAddr - vis.buffOffset].Suffix = ')';

                    for (int a = hexAddr; a <= endAddr && a < vis.PCM.fsize; a++)
                    {
                        vis.hexDatas[a - vis.buffOffset].Color = colors[c];
                        vis.hexDatas[a - vis.buffOffset].TdIndex = t;
                        vis.hexDatas[a - vis.buffOffset].TableName = tmpTd.TableName;
                    }
                    c++;
                    if (c >= (colors.Length - 1))
                    {
                        c = 0;
                    }
                }
                //Show selected table in brackets [tablename]
                int tdAddr = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset - vis.buffOffset);
                vis.hexDatas[tdAddr].TableText = vis.td.TableName + ": " + vis.td.Address;
                vis.hexDatas[tdAddr].TableText += "+" + vis.td.Offset.ToString();
                vis.hexDatas[tdAddr].TableText += "+" + vis.ExtraOffset;
                vis.hexDatas[tdAddr].TableText += " - " + (vis.td.EndAddressNoExtra() + vis.ExtraOffset).ToString("X8");
                vis.hexDatas[tdAddr].Prefix = '[';
                int tdEnd = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset - vis.buffOffset);
                vis.hexDatas[tdEnd].Suffix = ']';
                Debug.WriteLine("Table start: " + tdAddr.ToString("X"));
                Debug.WriteLine("Table End: " + tdEnd.ToString("X"));
                int start = (int)vis.td.addrInt;
                int end = (int)vis.td.addrInt + vis.td.Size();
                if (vis.td.Offset < 0)
                    start += vis.td.Offset;
                if (vis.ExtraOffset < 0)
                    start += vis.ExtraOffset;
                if (vis.ExtraOffset > 0)
                    end += vis.ExtraOffset;
                if (vis.td.Offset > 0)
                    end += vis.td.Offset;
                int OffsetEnd = (int)(vis.td.StartAddressNoExtra());
                int ExtraOffsetEnd = (int)(vis.td.StartAddressNoExtra() + vis.ExtraOffset);
                for (int addr = start; addr <= end && addr < vis.PCM.buf.Length; addr++)
                {
                    int buffAddr = addr - vis.buffOffset;
                    vis.hexDatas[buffAddr].SelectedTD = true;
                    vis.hexDatas[buffAddr].TableName = vis.td.TableName;
                    if (addr >= vis.td.StartAddressNoExtra() + vis.ExtraOffset && addr <= vis.td.EndAddressNoExtra() + vis.ExtraOffset)
                    {
                        vis.hexDatas[buffAddr].Color = Color.LightCoral;
                        vis.hexDatas[buffAddr].SelectedTD = true;
                    }
                    else if (vis.td.Offset > 0 && addr >= vis.td.addrInt && addr < OffsetEnd)
                    {
                        vis.hexDatas[buffAddr].Color = Color.Purple;
                    }
                    else if (vis.td.Offset < 0 && addr <= vis.td.addrInt && addr >= OffsetEnd)
                    {
                        vis.hexDatas[buffAddr].Color = Color.Purple;
                    }
                    else if (vis.ExtraOffset > 0 && addr >= OffsetEnd && addr < ExtraOffsetEnd)
                    {
                        vis.hexDatas[buffAddr].Color = Color.Green;
                    }
                    else if (vis.ExtraOffset < 0 && addr <= OffsetEnd && addr >= ExtraOffsetEnd)
                    {
                        vis.hexDatas[buffAddr].Color = Color.Green;
                    }

                    if (addr == selectedByte)
                    {
                        vis.hexDatas[buffAddr].Color = Color.Red;
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
            timer.Stop();
            Debug.WriteLine("CreateHexData time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
        }

        public void ShowTables(uint SelectedByte)
        {
            Debug.WriteLine("ShowTables");
            try
            {
                this.Text = "Table data visualizer [" + vis1.td.TableName + "]";
                numExtraOffset1.Value = vis1.td.extraoffset;
                numExtraOffset1.Minimum = -1 * (vis1.td.StartAddressNoExtra());
                numExtraOffset1.Maximum = vis1.PCM.buf.Length - vis1.td.EndAddressNoExtra() -1 ;
                this.selectedByte = SelectedByte;
                CreateHexDatas();
                numExtraOffset1.ValueChanged += numExtraOffset1_ValueChanged;
                dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
                if (vis2 != null && vis2.td != null)
                {
                    numExtraOffset2.Value = vis2.td.extraoffset;
                    numExtraOffset2.ValueChanged += numExtraOffset2_ValueChanged;
                    numExtraOffset2.Minimum = -1 * (vis2.td.StartAddressNoExtra());
                    numExtraOffset2.Maximum = vis2.PCM.buf.Length - vis2.td.EndAddressNoExtra() - 1;
                    dataGridView1.Scroll += DataGridView1_Scroll;
                    dataGridView2.Scroll += DataGridView2_Scroll;
                    dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
                }
                dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
                dataGridView2.ColumnHeaderMouseClick += DataGridView2_ColumnHeaderMouseClick;
                dataGridView1.CellPainting += DataGridView1_CellPainting;
                dataGridView2.CellPainting += DataGridView2_CellPainting;
                dataGridView1.CellValueNeeded += DataGridView1_CellValueNeeded;
                dataGridView2.CellValueNeeded += DataGridView2_CellValueNeeded;
                dataGridView1.MouseUp += DataGridView1_MouseUp;
                dataGridView2.MouseUp += DataGridView2_MouseUp;
                dataGridView1.CellMouseUp += DataGridView1_CellMouseUp;
                dataGridView1.CellMouseDown += DataGridView1_CellMouseDown;
                dataGridView2.CellMouseUp += DataGridView2_CellMouseUp;
                dataGridView2.CellMouseDown += DataGridView2_CellMouseDown;
                dataGridView1.KeyUp += DataGridView1_KeyUp;
                dataGridView2.KeyUp += DataGridView2_KeyUp;
                UpdateDisplay(true);
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

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }
            vis1.mouseDownCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Debug.WriteLine("Mouse down in " + vis1.mouseDownCell.RowIndex.ToString() + ", " + vis1.mouseDownCell.ColumnIndex.ToString());
        }

        private void DataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            vis1.mouseUpCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Debug.WriteLine("Mouse down in " + vis1.mouseUpCell.RowIndex.ToString() + ", " + vis1.mouseUpCell.ColumnIndex.ToString());
            ModifySelection(dataGridView1, vis1);
            SyncSelection(dataGridView1, dataGridView2, vis1, vis2);
        }
        private void DataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            vis1.mouseDownCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Debug.WriteLine("Mouse down in " + vis1.mouseDownCell.RowIndex.ToString() + ", " + vis1.mouseDownCell.ColumnIndex.ToString());
        }

        private void DataGridView2_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            vis2.mouseUpCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Debug.WriteLine("Mouse down in " + vis2.mouseUpCell.RowIndex.ToString() + ", " + vis2.mouseUpCell.ColumnIndex.ToString());
            ModifySelection(dataGridView2, vis2);
            SyncSelection(dataGridView2, dataGridView1, vis2, vis1);
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

                List<string> SelectedTdNames = new List<string>();
                if (dgv.SelectedCells.Count > 0)
                {
                    Start = int.MaxValue;
                    for (int c = 0; c < dgv.SelectedCells.Count; c++)
                    {
                        int row = dgv.SelectedCells[c].RowIndex;
                        int col = dgv.SelectedCells[c].ColumnIndex;
                        if (row < vis.dgrows.Count && col < vis.dgrows[row].Addresses.Count)
                        {
                            int addr = (int)vis.dgrows[row].Addresses[col];
                            string tdName = vis.hexDatas[addr-vis.buffOffset].TableName;
                            if (tdName != null && !SelectedTdNames.Contains(tdName))
                            {
                                SelectedTdNames.Add(tdName);
                                //selTables.Add(vis.SegmentTds.Where(X => X.TableName == tdName).FirstOrDefault());
                                selTables.Add(vis.SegmentTds[vis.hexDatas[addr - vis.buffOffset].TdIndex]);
                            }
                            if (addr < Start)
                            {
                                Start = addr;
                            }
                            if (addr > End)
                            {
                                End = addr;
                            }
                        }
                    }
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
                txtInfo.AppendText("Tables: " + Environment.NewLine);
                for (int t = 0; t < selTables.Count;t++)
                {
                    txtInfo.AppendText(selTables[t].TableName + " [" + selTables[t].StartAddress().ToString("X") + "] ("
                        + selTables[t].addrInt.ToString("X") + " + " + selTables[t].Offset.ToString()
                        + " + " + selTables[t].extraoffset.ToString() + ")" + Environment.NewLine);
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

            return selTables;
        }


        public void ChangeSelection(uint selectedByte)
        {
            Debug.WriteLine("ChangeSelection");
            try
            {
                this.selectedByte = selectedByte;
                int row = vis1.hexDatas[selectedByte - vis1.buffOffset].Row;
                int col = vis1.hexDatas[selectedByte - vis1.buffOffset].Col;
                dataGridView1.ClearSelection();
                dataGridView1.Rows[row].Cells[col].Selected = true;
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


        public void UpdateDisplay(bool ScrollToSelected)
        {
            try
            {
                Debug.WriteLine("UpdateDisplay");
                int r = dataGridView1.FirstDisplayedScrollingRowIndex;
                int currentAddr = -1;
                if (r > -1)
                {
                    currentAddr = (int)vis1.dgrows[r].Addresses[0];
                }
                if (vis2 != null && vis2.td != null)
                {
                    DisplayData(selectedByte, true);
                }
                else
                {
                    splitContainer1.Panel2.Hide();
                    splitContainer1.SplitterDistance = splitContainer1.Width - 5;
                }
                List<uint> SelectedAddresses = GetSelectedAddresses(dataGridView1, vis1);
                DisplayData(selectedByte, false);
                RestoreSelection(SelectedAddresses);
                if (ScrollToSelected && vis1.TdRow >= 0 && vis1.TdRow < dataGridView1.RowCount)
                {
                    //DrawingControl.SuspendDrawing(dataGridView1);
                    Debug.WriteLine("Scrolling to line: " + vis1.TdRow);
                    dataGridView1.FirstDisplayedScrollingRowIndex = vis1.TdRow;
                    //DrawingControl.ResumeDrawing(dataGridView1);
                }
                else if (currentAddr > -1 && (currentAddr - vis1.buffOffset) < vis1.hexDatas.Length && dataGridView1.Rows.Count > vis1.hexDatas[currentAddr - vis1.buffOffset].Row)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = vis1.hexDatas[currentAddr - vis1.buffOffset].Row;
                }
                Application.DoEvents();
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

        public void DisplayData(uint selectedByte, bool Secondary)
        {
            Debug.WriteLine("DisplayData");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            cellCount = 0;
            DataGridView dgv = dataGridView1;
            //dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
            //dataGridView2.SelectionChanged -= DataGridView2_SelectionChanged;
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

                int bytesPerRow = (int)numBytesPerRow.Value;
                int start = 0;
                int end = 0;
                if (radioShowTable.Checked)
                {
                    start = (int)vis.td.addrInt;
                    if (vis.td.Offset < 0)
                        start += vis.td.Offset;
                    if (vis.ExtraOffset < 0)
                        start += vis.ExtraOffset;
                    end = (int)vis.td.EndAddressNoExtra();
                    if (vis.ExtraOffset > 0)
                        end = (int)(vis.td.EndAddressNoExtra() + vis.ExtraOffset);
                    start = (int)(start - numFrontBytes.Value);
                    end = (int)(end + numAfterBytes.Value);
                }
                else
                {
                    /*                    int start1 = (int)vis1.PCM.segmentinfos[vis1.segmentNumber].GetStartAddr();
                                        int end1 = (int)vis1.PCM.segmentinfos[vis1.segmentNumber].GetEndAddr();
                                        int start2 = int.MaxValue;
                                        int end2 = int.MaxValue;
                                        int start3 = (int)(vis1.td.StartAddressNoExtra() + vis1.ExtraOffset);
                                        int end3 = int.MaxValue;
                                        if (vis2 != null && vis2.td != null)
                                        {
                                            start2 = (int)(vis2.PCM.segmentinfos[vis2.segmentNumber].GetStartAddr());
                                            end2 = (int)vis2.PCM.segmentinfos[vis2.segmentNumber].GetEndAddr();
                                            end3 = (int)(vis2.td.StartAddressNoExtra() + vis2.ExtraOffset);
                                        }

                                        start = Math.Min(start1, start2);
                                        end = Math.Max(end1, end2);
                                        start = Math.Min(start, start3);
                                        end = Math.Max(end, end3);
                    */
                    start = vis1.buffOffset;
                    end = vis1.buffOffset + vis1.hexDatas.Length - 1;
                }
                if (start < 0)
                {
                    start = 0;
                }
                if (end >= vis.PCM.buf.Length - 2)
                {
                    end = (int)vis.PCM.buf.Length - 2;
                }

                if (Secondary)
                {
                    //start += (int)numExtraOffset2.Value;
                    if (numExtraOffset2.Value > 0)
                    {
                        end += (int)numExtraOffset2.Value;
                    }
                    Debug.WriteLine("Secondary: Start: " + start.ToString() +", end: " + end.ToString() +", Offset: " + numExtraOffset2.Value.ToString());
                }
                else
                {
                    //start += (int)numExtraOffset1.Value;
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
                dgv.Rows.Clear();
                for (uint addr = (uint)start; addr <= end && addr < vis.PCM.buf.Length; addr++)
                {
                    int bufAddr = (int)addr - vis.buffOffset;
                    if (bufAddr < 0)
                    {
                        Debug.WriteLine("Address outside of hexdata: " + bufAddr.ToString());
                        continue;
                    }
                    if (bufAddr >= vis.hexDatas.Length)
                    {
                        Debug.WriteLine("Address outside of hexdata: " + bufAddr.ToString());
                        break;
                    }
                    if (vis.hexDatas[bufAddr].Prefix == 0)
                        vis.hexDatas[bufAddr].Prefix = ' ';
                    if (vis.hexDatas[bufAddr].Suffix == 0)
                        vis.hexDatas[bufAddr].Suffix = ' ';

                    if (radioSegmentTBNames.Checked && !string.IsNullOrEmpty(vis.hexDatas[bufAddr].TableText))
                    {
                        if (dgrow.Addresses.Count > 0)
                        {
                            dgrow = new DGROW();
                            row++;
                            vis.dgrows.Add(dgrow);
                        }
                        dgrow.HeaderTxt = vis.hexDatas[bufAddr].TableText;
                        col = 0;
                    }
                    if (addr == (vis.td.StartAddressNoExtra() + vis.ExtraOffset))
                    {
                        vis.TdRow = row;
                        Debug.WriteLine("Setting TdRow to: " + row.ToString() + " td.addrInt = " + vis.td.addrInt.ToString("X"));
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
                    vis.hexDatas[bufAddr].Row = row;
                    vis.hexDatas[bufAddr].Col = col;
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
                LoggerBold("Error, DisplayData, line " + line + ": " + ex.Message);
            }
            DrawingControl.ResumeDrawing(dgv);
            timer.Stop();
            Debug.WriteLine("Displaydata time Taken: " + timer.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
            Debug.WriteLine("Cells displayed: " + cellCount.ToString());
            //dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            //dataGridView2.SelectionChanged += DataGridView2_SelectionChanged;
        }

        private void radioShowSegment_CheckedChanged(object sender, EventArgs e)
        {
            if (radioShowSegment.Checked)
            {
                dataGridView1.RowHeadersWidth = 80;
                dataGridView2.RowHeadersWidth = 80;
                CreateHexDatas();
                UpdateDisplay(true);
            }
        }

        private void numFrontBytes_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("numExtraBytes_ValueChanged");
            CreateHexDatas();
            UpdateDisplay(false);
        }

        private void numBytesPerRow_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("numBytesPerRow_ValueChanged");
            SetupDatagrids();
            UpdateDisplay(false);
        }

        private void radioSegmentTBNames_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSegmentTBNames.Checked)
            {
                dataGridView1.RowHeadersWidth = 350;
                dataGridView2.RowHeadersWidth = 350;
                CreateHexDatas();
                UpdateDisplay(true);
            }
        }

        private void radioShowTable_CheckedChanged(object sender, EventArgs e)
        {
            if (radioShowTable.Checked)
            {
                dataGridView1.RowHeadersWidth = 80;
                dataGridView2.RowHeadersWidth = 80;
                CreateHexDatas();
                UpdateDisplay(true);
            }
        }

        private void numExtraOffset1_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("numExtraOffset1_ValueChanged");
            //vis1.td.ExtraOffset = (int)numExtraOffset1.Value;
            vis1.ExtraOffset = (int)numExtraOffset1.Value;
            CreateHexDatas();
            UpdateDisplay(false);
        }

        private void numExtraOffset2_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("numExtraOffset2_ValueChanged");
            //vis2.td.ExtraOffset = (int)numExtraOffset2.Value;
            vis2.ExtraOffset = (int)numExtraOffset2.Value;
            CreateHexDatas();
            UpdateDisplay(false);
            //SyncSelection2(false);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void btnApplyPrimary_Click(object sender, EventArgs e)
        {
            vis1.tdOrg.extraoffset = (int)numExtraOffset1.Value;
            if (tuner != null)
            {
                //tuner.RefreshTablelist();
                tuner.RefreshFast();
            }
        }

        private void btnApplySecondary_Click(object sender, EventArgs e)
        {
            vis2.tdOrg.extraoffset = (int)numExtraOffset2.Value;
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
                if (vis2 != null && vis2.td != null)
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
                if (vis2 != null && vis2.td != null)
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
                    selTables[t].extraoffset = (int)numExtraOffset1.Value;
                }
                //CreateHexData(PCM1, ref hexDatas1, td1, SortedTds1);
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                CreateHexDatas();
                UpdateDisplay(false);
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
                    selTables[t].extraoffset = (int)numExtraOffset2.Value;
                }
                //CreateHexData(PCM2, ref hexDatas2, td2, SortedTds2);
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                CreateHexDatas();
                UpdateDisplay(false);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void SelectRange(DataGridView dgv, VisSettings vis)
        {
            Debug.WriteLine("SelectRange");
            try
            {
                if (vis.SelStart >= 0 && vis.SelEnd > 0 && vis.hexDatas.Length > (vis.SelStart - vis.buffOffset) && vis.hexDatas.Length > (vis.SelEnd - vis.buffOffset))
                {
                    dgv.ClearSelection();
                    if (vis.SelStart > vis.SelEnd)
                    {
                        int tmp = vis.SelStart;
                        vis.SelStart = vis.SelEnd;
                        vis.SelEnd = tmp;
                    }
                    for (int a = vis.SelStart; a < vis.SelEnd; a++)
                    {
                        dgv.Rows[vis.hexDatas[a-vis.buffOffset].Row].Cells[vis.hexDatas[a-vis.buffOffset].Col].Selected = true;
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

        private int GetSelectedAddress(DataGridView dgv, VisSettings vis)
        {
            Debug.WriteLine("GetSelectedAddress");
            try
            {
                if (dgv.SelectedCells.Count > 0)
                {
                    int row = dgv.SelectedCells[0].RowIndex;
                    int col = dgv.SelectedCells[0].ColumnIndex;
                    if (row < vis.dgrows.Count && col < vis.dgrows[row].Addresses.Count)
                    {
                        return (int)vis.dgrows[row].Addresses[col];
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
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
            return -1;
        }
        private void btnSelStart1_Click(object sender, EventArgs e)
        {
            vis1.SelStart = GetSelectedAddress(dataGridView1, vis1);
            SelectRange(dataGridView1, vis1);
            SyncSelection(dataGridView1,dataGridView2,vis1,vis2);
        }

        private void btnSelEnd1_Click(object sender, EventArgs e)
        {
            vis1.SelEnd = GetSelectedAddress(dataGridView1, vis1);
            SelectRange(dataGridView1, vis1);
            SyncSelection(dataGridView1, dataGridView2, vis1, vis2);
            Application.DoEvents();
        }

        private void btnSelStart2_Click(object sender, EventArgs e)
        {
            vis2.SelStart = GetSelectedAddress(dataGridView2, vis2);
            SelectRange(dataGridView2, vis2);
            SyncSelection(dataGridView2, dataGridView1, vis2, vis1);
        }

        private void btnSelEnd2_Click(object sender, EventArgs e)
        {
            vis2.SelEnd = GetSelectedAddress(dataGridView2, vis2);
            SelectRange(dataGridView2, vis2);
            SyncSelection(dataGridView2, dataGridView1, vis2, vis1);
        }

        private void btnApplytoRight_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> selTables = GetSelectedTables(false);
                int offset = (int)numExtraOffset2.Value;
                for (int t = 0; t < selTables.Count; t++)
                {
                    TableData tdR = FindTableData(selTables[t], vis2.SortedTds);
                    if (tdR != null)
                    {
                        Logger("Applying offset: " + offset.ToString() + " to table: " + tdR.TableName);
                        if (offset == 0)
                            tdR.ExtraOffset = "-0";
                        else
                            tdR.extraoffset = offset;
                    }
                }
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                //ShowTables(selectedByte);
                CreateHexDatas();
                UpdateDisplay(false);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Applytoright, line " + line + ": " + ex.Message);

            }
        }

        private void btnApplytoLeft_Click(object sender, EventArgs e)
        {
            try
            {
                List<TableData> selTables = GetSelectedTables(true);
                int offset = (int)numExtraOffset1.Value;
                for (int t = 0; t < selTables.Count; t++)
                {
                    TableData tdL = FindTableData(selTables[t], vis1.SortedTds);
                    if (tdL != null)
                    {
                        Logger("Applying offset: " + offset.ToString() + " to table: " + tdL.TableName);
                        if (offset == 0)
                            tdL.ExtraOffset = "-0";
                        else
                            tdL.extraoffset = offset;
                    }
                }
                if (tuner != null)
                {
                    tuner.RefreshFast();
                }
                //ShowTables(selectedByte);
                CreateHexDatas();
                UpdateDisplay(false);

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Applytoleft, line " + line + ": " + ex.Message);

            }

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
            try
            {
                ClearSearch();
                Debug.Write("Searching: ");
                for (int i = 0; i < searchBytes.Count; i++)
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
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableVisDouble, line " + line + ": " + ex.Message);
            }
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

        private void numAfterBytes_ValueChanged(object sender, EventArgs e)
        {
            CreateHexDatas();
            UpdateDisplay(false);
        }

        private void comboCopyColorsLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                leftColors = (CopyColors)comboCopyColorsLeft.SelectedValue;
                CreateHexDatas();
                UpdateDisplay(false);
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

        private void comboCopyColorsRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                rightColors = (CopyColors)comboCopyColorsRight.SelectedValue;
                CreateHexDatas();
                UpdateDisplay(false);
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

        private void numExtraBytes_ValueChanged(object sender, EventArgs e)
        {
            numFrontBytes.Value = numExtraBytes.Value;
            numAfterBytes.Value = numExtraBytes.Value;
        }
    }
}
