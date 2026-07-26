using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using MathParserTK;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using static UniversalPatcher.ExtensionMethods;
using static Helpers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace UniversalPatcher
{
    public partial class frmTableEditor : Form
    {
        public frmTableEditor(FrmTuner tuner)
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
            this.tuner = tuner;
        }


        private enum ColType
        {
            Flag,
            Combo,
            Value
        }

        private enum ShowMode
        {
            normal,
            compare,
            sideBySide,
            sideBySideTxt,
            compareAll,
            diff,
            diff2
        }

        private class MultiTableName
        {
            public MultiTableName(string fullName, int columnPos)
            {
                RowName = "";
                string[] separators = AppSettings.MultitableChars.Split(' ');
                string[] nParts = fullName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                //string[] nParts = fullName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                TableName = nParts[0];
                if (nParts.Length == 2)
                {
                    columnName = nParts[1].Trim();
                }
                if (nParts.Length == 3)
                {
                    columnName = nParts[1].Trim();
                    RowName = nParts[2].Trim();
                }
                if (nParts.Length > 3)
                {                    
                    columnName = nParts[columnPos].Trim();
                    for (int i = 1; i < 4; i++)
                        if (i != columnPos)
                        RowName += "[" + nParts[i].Trim() + "]";
                }

            }
            public string TableName { get; set;}
            public string columnName { get; set; }
            public string RowName { get; set; }
        }

        //List of loaded files (for compare) File 0 is always "master" or A
        public List<CompareFile> compareFiles = new List<CompareFile>();
        private TableInfo[] compareTableInfos;
        //List of selected tables in tuner (current node in tree)
        public List<TableData> tunerFilteredTables = new List<TableData>();
        int currentTunerTd = -1;
        public string tableName = "";
        Font dataFont;

        private bool only1d = false;    //Show multiple 1D tables as one multirow table
        public bool disableMultiTable = false;
        public bool multiSelect = false;    //Manually selected multiple files
        private bool duplicateTableName = false;    // Multiple tables wit equal name, but some other setting may differ
        public int currentFile = 0;
        public int currentCmpFile = 1;

        public FrmTuner tuner;
        private string lastTable = "";
        List<CheckBox> fileCheckBoxes;

        int multiplierDecimals = 3;
        int decimals = 0;

        //frmTableVis ftv;
        frmTableVisDouble2 ftvd;

        private Dictionary<string, int> dgColumnHeaders;
        private Dictionary<string, int> dgRowHeaders;

        private ShowMode showMode = ShowMode.normal;
        private bool showRawHex = false;
        private bool enableDiff = false;
        private bool editingHex = false;
        bool disableTooltips = false;
        ToolTip NaviTip = new ToolTip();
        ToolTip UpDownTip = new ToolTip();
        private static readonly Color[] gradientStops = {
            System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerColorsMin),  // lime green  (min)
            System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerColorsMid2),  // yellow
            System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerColorsMid2),  // orange
            System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerColorsMax),  // deep red    (max)
        };
        private Color HexDataColor = System.Drawing.ColorTranslator.FromHtml("#056017");
        private static readonly Regex HexChar = new Regex(@"[0-9A-Fa-f]");
        private HexPanel hexpanel;
        private bool AutoResizeTmpDisabled = false;
        private void frmTableEditor_Load(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.WorkingMode < 2)
                {
                    groupExtraOffset.Visible = false;
                }
                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                if (AppSettings.TableEditorFont == null)
                    dataFont = new Font("Consolas", 9);
                else
                    dataFont = AppSettings.TableEditorFont.ToFont();
                hexpanel = new HexPanel();
                hexpanel.BytesPerRow = AppSettings.TunerHexWindowColumns;
                hexpanel.ShowHeaders = AppSettings.TunerHexWindowHeaders;
                hexpanel.ShowOffsets = AppSettings.TunerHexWindowOffsets;
                showOffsetsToolStripMenuItem.Checked = AppSettings.TunerHexWindowOffsets;
                showHeadersToolStripMenuItem.Checked = AppSettings.TunerHexWindowHeaders;
                hexpanel.ShowAscii = false;
                hexpanel.TextFont = AppSettings.TunerHexWindowFont.ToFont();
                splitContainer1.Panel2.Controls.Add(hexpanel);
                hexpanel.Dock = DockStyle.Fill;
                hexpanel.SelectionChanged += Hexpanel_SelectionChanged;
                hexpanel.ContextMenuStrip = contextMenuHexWindowSettings;
                hexpanel.BackColor = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowBackColor);
                hexpanel.ColorHex = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowDataColor);
                hexpanel.ColorModified = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowModifiedColor);
                highlightBackgroundToolStripMenuItem.Checked = AppSettings.TunerHexWindowHighlightBackground;
                hexpanel.HighlightBackground = AppSettings.TunerHexWindowHighlightBackground;
                if (AppSettings.TunerHexWindowShow)
                {
                    showHEXWindowToolStripMenuItem.Checked = true;
                    splitContainer1.SplitterDistance = splitContainer1.Width - AppSettings.TunerHexWindowWidth;
                    btnToggleHexview.ImageKey = "Collapse.png";
                }
                else
                {
                    splitContainer1.Panel2Collapsed = true;
                    btnToggleHexview.ImageKey = "Expand.png";
                }
                this.Resize += FrmTableEditor_Resize;
                splitContainer1.MouseUp += SplitContainer1_MouseUp;
                numTuneValue.Tag = numTuneValue.Value;
                autoResizeToolStripMenuItem.Checked = AppSettings.TableEditorAutoResize;
                addressToolStripMenuItem.Checked = AppSettings.TableEditorHexShowAddress;
                binaryToolStripMenuItem.Checked = AppSettings.TableEditorHexShowBinary;
                decimalToolStripMenuItem.Checked = AppSettings.TableEditorHexShowDecimal;

                if (AppSettings.TunerColorsMode == ConditionalColors.Off)
                {
                    offToolStripMenuItem.Checked = true;
                }
                else if (AppSettings.TunerColorsMode == ConditionalColors.Settings)
                {
                    tableSettingsToolStripMenuItem.Checked = true;
                }
                else
                {
                    tableValuesToolStripMenuItem.Checked = true;
                }
                if (AppSettings.TableEditorAutoResize)
                {
                    AutoResize();
                }
                else if (AppSettings.MainWindowPersistence)
                {
                    if (AppSettings.TableEditorWindowSize.Width > 0 || AppSettings.TableEditorWindowSize.Height > 0)
                    {
                        this.WindowState = AppSettings.TableEditorWindowState;
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        this.Location = AppSettings.TableEditorWindowLocation;
                        this.Size = AppSettings.TableEditorWindowSize;
                    }
                }
                disableTooltipsToolStripMenuItem.Checked = false;
                rememberCompareSelectionToolStripMenuItem.Checked = AppSettings.TableEditorRememberCompare;
                if (AppSettings.TableEditorRememberCompare)
                {
                    chkSwapXY.Checked = tuner.SwapXy;
                    showRawHEXValuesToolStripMenuItem.Checked = tuner.ShowAsHex;
                    chkRawHex.Checked = tuner.ShowAsHex;
                    if (groupSelectCompare.Enabled)
                    {
                        switch (tuner.CompareSelection)
                        {
                            case 0:
                                radioOriginal.Checked = true;
                                break;
                            case 1:
                                radioCompareFile.Checked = true;
                                break;
                            case 2:
                                radioSideBySide.Checked = true;
                                break;
                            case 3:
                                radioSideBySideText.Checked = true;
                                break;
                            case 4:
                                radioCompareAll.Checked = true;
                                break;
                            case 5:
                                radioDifference.Checked = true;
                                break;
                            case 6:
                                radioDifference2.Checked = true;
                                break;

                        }
                        switch (tuner.CompareType)
                        {
                            case 0:
                                radioAbsolute.Checked = true;
                                break;
                            case 1:
                                radioMultiplier.Checked = true;
                                break;
                            case 2:
                                radioPercent.Checked = true;
                                break;
                        }
                    }
                }
                dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
                dataGridView1.RowHeaderMouseClick += DataGridView1_RowHeaderMouseClick;
                dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
                dataGridView1.CellClick += DataGridView1_SelectionChanged;
                dataGridView1.CellMouseMove += DataGridView1_CellMouseMove;
                dataGridView1.CellMouseLeave += DataGridView1_CellMouseLeave;
                dataGridView1.ColumnAdded += DataGridView1_ColumnAdded;
                rewToolStripMenuItem.MouseDown += NavigatorMenuItem_MouseDown;
                fwdToolStripMenuItem.MouseDown += NavigatorMenuItem_MouseDown;
                if (this.Parent == null)
                {
                    rewToolStripMenuItem.Visible = true;
                    fwdToolStripMenuItem.Visible = true;
                    upToolStripMenuItem.Visible = true;
                    downToolStripMenuItem.Visible = true;
                    upToolStripMenuItem.MouseHover += UpToolStripMenuItem_MouseHover;
                    downToolStripMenuItem.MouseHover += DownToolStripMenuItem_MouseHover;
                    rewToolStripMenuItem.MouseHover += Navigator_MouseHover;
                    fwdToolStripMenuItem.MouseHover += Navigator_MouseHover;
                    CompareFile selectedFile = compareFiles[currentFile];
                    TableData td = selectedFile.tableInfos[0].td;
                    SetUpDownToolTips();
                }
                else
                {
                    rewToolStripMenuItem.Visible = false;
                    fwdToolStripMenuItem.Visible = false;
                    upToolStripMenuItem.Visible = false;
                    downToolStripMenuItem.Visible = false;

                }
                for (int c=2;c<=16;c+=2)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem(c.ToString());
                    mi.Click += HexWindowColumnsMenu_Click;
                    if (c == AppSettings.TunerHexWindowColumns)
                    {
                        mi.Checked = true;
                    }
                    columnsToolStripMenuItem.DropDownItems.Add(mi);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.FillWeight = 1;
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedCells.Count == 0 || dataGridView1.SelectedCells[0].Tag == null) return;
                if (!AppSettings.TableCellMouseHover) return;
                TableCell tCell = (TableCell)dataGridView1.SelectedCells[0].Tag;
                ShowCellInfo(tCell, true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count || e.ColumnIndex < 0 || e.ColumnIndex >= dataGridView1.Columns.Count) return;
                if (!AppSettings.TableCellMouseHover) return;
                TableCell tCell = (TableCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                ShowCellInfo(tCell, true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }

        private void Hexpanel_SelectionChanged(object sender, HexSelectionEventArgs e)
        {
            if (e.Start < 0) return; // nothing selected
            Debug.WriteLine(e.ToString());
            try
            {
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                dataGridView1.CellClick -= DataGridView1_SelectionChanged;
                //Debug.WriteLine("First byte: " + selectedByte.ToString() + ", length: " + selectedByteCount.ToString());
                dataGridView1.ClearSelection();
                TableCell tCell = (TableCell)dataGridView1.Rows[0].Cells[0].Tag;
                int[] selectedBytes = hexpanel.GetSelectedOffsets();
                List<int> selectedAddresses = new List<int>();
                int elementSize = tCell.td.ElementSize();
                foreach(int selectedByte in selectedBytes)
                {
                    selectedAddresses.Add((int)(tCell.td.StartAddress() + selectedByte - hexpanel.BracketStart));
                }
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        tCell = (TableCell)dataGridView1.Rows[r].Cells[c].Tag;
                        if (tCell != null)
                        {
                            for (int a = (int)tCell.addr; a < (tCell.addr + elementSize); a++)
                            {
                                if (selectedAddresses.Contains(a))
                                {
                                    dataGridView1.Rows[r].Cells[c].Selected = true;
                                }
                            }
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellClick += DataGridView1_SelectionChanged;
        }

        private void HexWindowColumnsMenu_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem m in columnsToolStripMenuItem.DropDownItems)
            {
                m.Checked = false;
            }
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            mi.Checked = true;
            AppSettings.TunerHexWindowColumns = int.Parse(mi.Text);
            hexpanel.BytesPerRow = AppSettings.TunerHexWindowColumns;
            AutoResize();
        }

        private void SplitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                AppSettings.TunerHexWindowWidth = splitContainer1.Width - splitContainer1.SplitterDistance;
                Debug.WriteLine("Hex window size: " + AppSettings.TunerHexWindowWidth.ToString());
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void FrmTableEditor_Resize(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.TunerHexWindowShow)
                {
                    int w = splitContainer1.Width - AppSettings.TunerHexWindowWidth;
                    if (w > 0)
                    {
                        splitContainer1.SplitterDistance = splitContainer1.Width - AppSettings.TunerHexWindowWidth;
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
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void ShowNaviSelection()
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            List<TreeParts.Navi> navigator = compareFiles[currentFile].pcm.Navigator;
            for (int i = 0; i < navigator.Count; i++)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(navigator[i].PathStr());
                if (i == compareFiles[currentFile].NaviCurrent)
                    mi.Checked = true;
                mi.Click += Mi_Click;
                mi.Tag = i;
                cms.Items.Add(mi);
            }
            cms.Show(System.Windows.Forms.Cursor.Position);

        }

        private void Mi_Click(object sender, EventArgs e)
        {
            int pos = (int)((ToolStripMenuItem)sender).Tag;
            Navigate(pos);
        }

        private void NavigatorMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                ShowNaviSelection();
        }

        private void Navigator_MouseHover(object sender, EventArgs e)
        {
            ShowNaviTip();
        }

        private void ShowNaviTip()
        {
            List<TreeParts.Navi> navi = compareFiles[currentFile].pcm.Navigator;
            PcmFile pcm = compareFiles[currentFile].pcm;
            int position = compareFiles[currentFile].NaviCurrent;
            string message = "Navigator: " + (position + 1).ToString() + "/" + navi.Count.ToString();
            NaviTip.Show(message, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 30, 2000);
        }

        public void SaveOnExit()
        {
            if (compareFiles == null || compareFiles.Count == 0)
            {
                return;
            }
            bool tableModified = false;
            for (int a = 0; a < compareFiles[0].tableInfos.Count; a++)
            {
                if (compareFiles[0].tableInfos[a].isModified())
                {
                    tableModified = true;
                    break;
                }
            }

            if (tableModified)
            {
                DialogResult dialogResult = MessageBox.Show("Apply modifications?", "Apply modifications?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SaveTable(true);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }

        }

        private void frmTableEditor_FormClosing(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.MainWindowPersistence)
                {
                    AppSettings.TableEditorWindowState = this.WindowState;
                    if (this.WindowState == FormWindowState.Normal)
                    {
                        AppSettings.TableEditorWindowLocation = this.Location;
                        AppSettings.TableEditorWindowSize = this.Size;
                    }
                    else
                    {
                        AppSettings.TableEditorWindowLocation = this.RestoreBounds.Location;
                        AppSettings.TableEditorWindowSize = this.RestoreBounds.Size;
                    }
                }
                AppSettings.Save();

                SaveOnExit();

                if (ftvd != null && ftvd.Visible)
                    ftvd.Dispose();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        //Set default values before opening new table
        //Required for tree/docked mode tuner
        public void CleanUp()
        {
            groupSelectCompare.Enabled = false;
            compareToolStripMenuItem.DropDownItems.Clear();
            compareFiles = new List<CompareFile>();

            chkSwapXY.Enabled = true;
            this.numDecimals.ValueChanged -= new System.EventHandler(this.numDecimals_ValueChanged);
            numDecimals.Value = -1;
            this.numDecimals.ValueChanged += new System.EventHandler(this.numDecimals_ValueChanged);
            decimals = 0;

            tableName = "";
            only1d = false;    //Show multiple 1D tables as one multirow table
            multiSelect = false;    //Manually selected multiple files
            duplicateTableName = false;    // Multiple tables wit equal name, but some other setting may differ
            currentFile = 0;
            currentCmpFile = 1;
            //currentTunerTd = -1;
            lastTable = "";
        }

        private void ShowSelectionInHexWindow(int elementSize)
        {

            try
            {
                if (editingHex || AppSettings.TunerHexWindowShow == false )
                {
                    return;
                }
                List<int> selectedBytes = new List<int>();
                for (int s = 0; s < dataGridView1.SelectedCells.Count; s++)
                {
                    TableCell tCell = (TableCell)dataGridView1.SelectedCells[s].Tag;
                    if (tCell != null)
                    {
                        int byteNr = (int)(tCell.addr);
                        for (int b = 0; b < tCell.td.ElementSize(); b++)
                        {
                            selectedBytes.Add(byteNr + b);
                        }
                    }
                }
                hexpanel.SetExternalSelection(selectedBytes.ToArray());
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void ShowCellInfo(TableCell tCell, bool InfoOnly)
        {
            try
            {
                if (tCell == null || tCell.addr == (uint.MaxValue - 1))
                {
                    labelInfo.Text = "";
                    return; //OBD2 Description, or empty
                }
                string thisTable = tCell.td.TableName;
                if (!InfoOnly && thisTable != lastTable && tuner != null && this.Parent != null)
                {
                    lastTable = thisTable;
                    tuner.ShowTableDescription(tCell.tableInfo.compareFile.pcm, tCell.td);
                }
                string minMaxTxt = "";
                TableData tData = tCell.td;
                double minRaw = GetMinValue(tData.DataType);
                double maxRaw = GetMaxValue(tData.DataType);
                double min = tCell.CalculatedValue(minRaw);
                if (minRaw == 0 && double.IsNaN(min))
                {
                    minRaw = 1;
                    min = tCell.CalculatedValue(minRaw);
                }
                double max = tCell.CalculatedValue(maxRaw);

                string formatStr = "0";
                if (numDecimals.Value > 0)
                {
                    formatStr = "0.";
                    for (int f = 0; f < (int)numDecimals.Value; f++)
                    {
                        //if (f == 1) formatStr += ".";
                        formatStr += "0";
                    }
                }
                if (min > max)
                {
                    //swap
                    double tmp = max;
                    max = min;
                    min = tmp;
                }

                if (min < tData.Min || max > tData.Max)
                {
                    minMaxTxt = " Soft limits: Min " + tData.Min.ToString(formatStr) + " Max " + tData.Max.ToString(formatStr) +
                        " Hard limits: Min " + min.ToString(formatStr) + " Max " + max.ToString(formatStr);

                }
                else
                {
                    minMaxTxt = "Min " + min.ToString(formatStr) + " Max " + max.ToString(formatStr);
                }
                string valTxt = " Last value " + Convert.ToDouble(tCell.lastValue).ToString(formatStr) + " Saved value " + Convert.ToDouble(tCell.origValue).ToString(formatStr);
                labelInfo.Text = valTxt + minMaxTxt;
                //if (!tData.Math.StartsWith("DTC"))
                {
                    labelInfo.Text += " Address: " + tCell.addr.ToString("X");
                }
                if (tData.OutputType == OutDataType.Bitmap)
                {
                    labelInfo.Text += " Bit: " + (tCell.Row % 8).ToString();
                }
                if (!InfoOnly)
                {
                    if (ftvd != null && ftvd.Visible)
                    {
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            ftvd.ChangeSelection(tCell.addr);
                        });
                    }

                    ShowSelectionInHexWindow(tCell.td.ElementSize());
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0 || dataGridView1.SelectedCells[0].Tag == null)
                return;
            TableCell tCell = (TableCell)dataGridView1.SelectedCells[0].Tag;
            ShowCellInfo(tCell, false);
        }

        private void SetupColorRanges(TableInfo tInfo)
        {
            if (AppSettings.TunerColorsMode == ConditionalColors.Settings)
            {
                tInfo.MinVal = tInfo.td.Min;
                tInfo.MaxVal = tInfo.td.Max;
            }
            else
            {
                tInfo.MinVal = double.MaxValue;
                tInfo.MaxVal = double.MinValue;
                for (int t=0; t< tInfo.tableCells.Count;t++)
                {
                    TableCell tc = tInfo.tableCells[t];
                    if (Convert.ToDouble(tc.lastValue) > tInfo.MaxVal)
                    {
                        tInfo.MaxVal = Convert.ToDouble(tc.lastValue);
                    }
                    if (Convert.ToDouble(tc.lastValue) < tInfo.MinVal)
                    {
                        tInfo.MinVal = Convert.ToDouble(tc.lastValue);
                    }
                }
            }

        }
        private void ParseTableInfo(CompareFile cmpFile)
        {
            try
            {
                PcmFile pcm = cmpFile.pcm;
               
                int totalRows = 0;
                int totalCols = 0;
                foreach(TableData tData in cmpFile.filteredTables)
                {
                    totalRows += tData.Rows;
                    totalCols += tData.Columns;
                    TableInfo tInfo = new TableInfo(pcm, tData);
                    tInfo.compareFile = cmpFile;
                    tInfo.ParseTable(disableMultiTable, duplicateTableName, true);
                    SetupColorRanges(tInfo);
                    cmpFile.tableInfos.Add(tInfo);
                }
                compareFiles.Add(cmpFile);
                if (totalRows < totalCols && AppSettings.TunerXYSwapWideTables)
                {
                    SetXYswapped(true);
                }
                else
                {
                    SetXYswapped(false);
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void DataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Form.ModifierKeys != Keys.Control )
                dataGridView1.ClearSelection();
            for (int c = 0; c < dataGridView1.Columns.Count; c++)
                dataGridView1.Rows[e.RowIndex].Cells[c].Selected = true;
        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Form.ModifierKeys != Keys.Control)
                dataGridView1.ClearSelection();
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
                dataGridView1.Rows[r].Cells[e.ColumnIndex].Selected = true;
        }

        public void PrepareTable(PcmFile pcm, TableData td,List<TableData> tableTds, string fileLetter)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                CompareFile orgFile = new CompareFile(pcm);
                orgFile.fileLetter = fileLetter;
                radioOriginal.Text = fileLetter;
                tableName = td.TableName;

                if (tableTds == null)
                {
                    tableTds = new List<TableData>();
                    tableTds.Add(td);
                }

                if (tableTds.Count > 1)
                {
                    multiSelect = true;
                    PrepareMultiTable(orgFile, td, tableTds, td.extraoffset);
                    return;
                }
                if (!disableMultiTable)
                {
                    if (td.TableName.ToLower().EndsWith(".xval") || td.TableName.ToLower().EndsWith(".yval"))
                    {
                        int ExtraOffset = td.extraoffset;
                        for (int x = 0; x < pcm.tableDatas.Count; x++)
                        {
                            if (pcm.tableDatas[x].TableName.ToLower() == td.TableName.ToLower().Replace(".yval", ".data").Replace(".xval", ".data"))
                            {
                                td = pcm.tableDatas[x];
                                PrepareMultiTable(orgFile, td, null, ExtraOffset);
                                return;
                            }
                        }
                    }
                    string[] separators = AppSettings.MultitableChars.Split(' ');
                    if (separators.Any(td.TableName.Contains))
                    {
                        //if (td.TableName.ToLower().Contains(" vs.") || td.TableName.StartsWith("Header.") || td.TableName.EndsWith(".Data") || td.TableName.EndsWith(".xVal") || td.TableName.EndsWith(".yVal") || td.TableName.EndsWith(".Size"))
                        if (td.TableName.ToLower().Contains(" vs.") || td.TableName.StartsWith("Header.") || td.TableName.EndsWith(".Data") || td.TableName.EndsWith(".Size"))
                        {
                            //Special case, "Normal" table, but header values from tables, WITH different table as multiplier
                            Debug.WriteLine("Special case, not real multitable");
                        }
                        else
                        {
                            MultiTableName mtn = new MultiTableName(td.TableName, 1);
                            tableName = mtn.TableName;
                            for (int t = 0; t < pcm.tableDatas.Count; t++)
                            {
                                if (pcm.tableDatas[t].Category == td.Category && pcm.tableDatas[t].TableName.StartsWith(mtn.TableName) && pcm.tableDatas[t].TableName != td.TableName)
                                {
                                    //It is multitable
                                    PrepareMultiTable(orgFile, pcm.tableDatas[t], null, td.extraoffset);
                                    return;
                                }
                            }
                        }
                    }
                }
                orgFile.filteredTables = new List<TableData>();
                orgFile.filteredTables.Add(tableTds[0]);
                ParseTableInfo(orgFile);
                SetMyText();

                stopwatch.Stop();
                Debug.WriteLine("prepareTable time Taken: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }


        private void PrepareMultiTable(CompareFile cmpFile, TableData tData, List<TableData> tableTds, int ExtraOffset)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                int maxCols = 0;
                int maxRows = 0;
                int cols = 0;
                int rows = 0;

                if (tableTds != null && tableTds.Count > 1)
                {
                    //Manually selected multiple tables
                    cmpFile.filteredTables = new List<TableData>();
                    //tableTds.Sort();

                    List<string> tableNameList = new List<string>();
                    for (int i = 0; i < tableTds.Count; i++)
                    {
                        TableData mTd =tableTds[i];
                        mTd.extraoffset = ExtraOffset;
                        if (tableNameList.Contains(mTd.TableName))
                        {
                            duplicateTableName = true;
                        }
                        else
                        {
                            tableNameList.Add(mTd.TableName);
                        }
                        if (mTd.Columns > maxCols)
                            maxCols = mTd.Columns;
                        if (mTd.Rows > maxRows)
                            maxRows = mTd.Rows;
                    }
                    for (int i = 0; i < tableTds.Count; i++)
                    {
                        cmpFile.filteredTables.Add(tableTds[i]);

                    }
                    rows = tableTds.Count;
                    cols = maxCols;
                    if (maxCols < 2 && maxRows < 2)
                        only1d = true;
                }
                else
                {
                    //Multible tables which are meant to be linked together
                    string filterName = tData.TableName.Substring(0, tableName.Length + 1);
                    var results = cmpFile.pcm.tableDatas.Where(t => t.TableName.StartsWith(filterName));
                    cmpFile.filteredTables = new List<TableData>(results.ToList());
                    cmpFile.filteredTables = cmpFile.filteredTables.OrderBy(o => o.addrInt).ToList();
                    cols = cmpFile.filteredTables.Count;
                    rows = cmpFile.filteredTables[0].Rows;
                    tableTds = new List<TableData>();
                    for (int i = 0; i < cmpFile.filteredTables.Count; i++)
                    {
                        tableTds.Add(cmpFile.filteredTables[i]);
                        tableTds[i].extraoffset = ExtraOffset;
                    }
                }

                //cmpFile.tableIds = tableTds;
                cmpFile.Rows = rows;
                cmpFile.Cols = cols;
                ParseTableInfo(cmpFile);
                SetMyText();
                stopwatch.Stop();
                Debug.WriteLine("prepareMultiTable time Taken: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void ModifyRadioText(string menuTxt)
        {
            if (menuTxt == null || menuTxt.Length == 0)
                return;
            string selectedBin = GetFileLetter(menuTxt);
            radioCompareFile.Text = selectedBin;
            radioDifference.Text = radioOriginal.Text + " > " + selectedBin;
            radioDifference2.Text = radioOriginal.Text + " < " + selectedBin;
            radioSideBySide.Text = radioOriginal.Text + " | " + selectedBin;
            radioSideBySideText.Text = radioOriginal.Text + " [" + selectedBin + "]";
            radioCompareAll.Text = radioOriginal.Text + " | *";
        }

        private string GetFileLetter(string menuTxt)
        {
            string retVal = "";

            int pos = menuTxt.IndexOf(':');
            if (pos > -1)
                retVal = menuTxt.Substring(0, pos);
            return retVal;
        }

        public void AddCompareFiletoMenu(PcmFile cmpPCM, string menuTxt, string selectedFile)
        {
            //If cmpTd is not null AND cmpPCM.OS == PCM.OS, cmpTd is used as is (Compare 2 tables)
            try
            {
                CompareFile cmpFile = new CompareFile(cmpPCM);
                ToolStripMenuItem menuitem = new ToolStripMenuItem(cmpPCM.FileName);
                menuitem.Tag = cmpFile;
                menuitem.Name = Path.GetFileName(cmpPCM.FileName);
                if (menuTxt.Length > 0)
                {
                    menuitem.Text = menuTxt;
                    cmpFile.fileLetter = GetFileLetter(menuTxt);
                }
                else
                {
                    int lastFile = 0;
                    foreach (ToolStripMenuItem mi in compareToolStripMenuItem.DropDownItems)
                        lastFile++;
                    string fLetter = Base26Encode(lastFile);
                    menuitem.Text = fLetter + ": " + cmpPCM.FileName;
                    cmpFile.fileLetter = fLetter;
                }
                foreach(TableInfo tInfo in compareFiles[0].tableInfos)
                {
                    TableData cmpTd = FindTableData(tInfo.td, cmpPCM.tableDatas);
                    if (cmpTd != null)
                    {
                        cmpFile.filteredTables.Add(cmpTd);
                    }
                }
                ParseTableInfo(cmpFile);
                menuitem.Click += compareSelection_Click;
                if (cmpFile.fileLetter == selectedFile || (compareToolStripMenuItem.DropDownItems.Count == 0 && selectedFile == ""))
                {
                    menuitem.Checked = true;
                    groupSelectCompare.Enabled = true;
                    ModifyRadioText(menuTxt);
                    //currentCmpFile = compareToolStripMenuItem.DropDownItems.Count;
                }
                compareToolStripMenuItem.DropDownItems.Add(menuitem);
                currentCmpFile = FindFile(selectedFile);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void compareSelection_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in compareToolStripMenuItem.DropDownItems)
                mi.Checked = false;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            ModifyRadioText(menuitem.Text);
            menuitem.Checked = true;
            CompareFile cmpFile = (CompareFile)menuitem.Tag;
            currentCmpFile = FindFile(cmpFile.fileLetter);
            selectedCompareBin = cmpFile.fileLetter;
            if (radioCompareFile.Checked)
                SelectFile(cmpFile.fileLetter);
            //prepareCompareTable(cmpFile); //Not again
            LoadTable();
            SetMyText();
        }

        public void LoadSeekTable(int tId, PcmFile pcm)
        {
            try
            {
                CompareFile cmpFile = new CompareFile(pcm);
                if (!pcm.seekTablesImported)
                    pcm.ImportSeekTables();
                TableSeek tSeek = tableSeeks[pcm.foundTables[tId].configId];
                this.Text = "Table Editor: " + pcm.foundTables[tId].Name;

                FoundTable ft = pcm.foundTables[tId];
                for (int f = 0; f < pcm.tableDatas.Count; f++)
                {
                    if (pcm.tableDatas[f].TableName == tSeek.Name && pcm.tableDatas[f].addrInt == ft.addrInt)
                    {
                        PrepareTable(pcm, pcm.tableDatas[f], null, "A");
                        LoadTable();
                        break;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }
        public void SetCellValue(int row, int col, TableCell tCell, TableCell cmpTCell)
        {
            try
            {
                if (row < 0 || row >= dataGridView1.Rows.Count || col < 0 || col >= dataGridView1.Columns.Count)
                {
                    Debug.WriteLine("Error, position " + row.ToString() + "," + col.ToString() + " out of grid");
                    return;
                }
                TableData mathTd = tCell.td;
                double curVal = Convert.ToDouble(tCell.lastValue);
                double origVal = Convert.ToDouble(tCell.origValue);
                double curRawValue = tCell.lastRawValue;
                double cmpRawValue = UInt64.MaxValue;
                double cmpVal = double.MinValue;
                if (cmpTCell != null && !radioOriginal.Checked)
                {
                    cmpVal = Convert.ToDouble(cmpTCell.lastValue);
                    cmpRawValue = (double)cmpTCell.lastRawValue;
                    tCell.cmpValue = cmpVal;
                }

                if (showMode == ShowMode.sideBySideTxt)
                {
                    string curTxt = "";
                    string cmpTxt = "";
                    string formatStr = "0";
                    if (showRawHEXValuesToolStripMenuItem.Checked)
                    {
                        formatStr = "X" + (mathTd.ElementSize() * 2).ToString();
                        curTxt = curRawValue.ToString(formatStr);
                        if (cmpRawValue < UInt64.MaxValue)
                            cmpTxt = ((uint)cmpRawValue).ToString(formatStr);
                    }
                    else
                    {
                        if (mathTd.OutputType == OutDataType.Text)
                        {
                            curTxt = Convert.ToChar((ushort)curVal).ToString();
                            if (cmpVal > double.MinValue)
                            {
                                cmpTxt = Convert.ToChar((ushort)cmpVal).ToString();
                            }
                        }
                        else if (mathTd.OutputType == OutDataType.Bitmap || (mathTd.OutputType == OutDataType.Flag && mathTd.BitMask != null && mathTd.BitMask.Length > 0))
                        {
                            curTxt = curVal.ToString();
                            if (cmpRawValue < UInt64.MaxValue)
                            {
                                cmpTxt = cmpVal.ToString();
                            }
                        }
                        else if (mathTd.OutputType == OutDataType.Hex)
                        {
                            formatStr = "X" + (GetElementSize(mathTd.DataType) * 2).ToString();
                            curTxt = curRawValue.ToString(formatStr);
                            if (cmpRawValue < UInt64.MaxValue)
                                cmpTxt = cmpRawValue.ToString(formatStr);
                        }
                        else if (mathTd.OutputType == OutDataType.Int)
                        {
                            curTxt = ((int)curVal).ToString();
                            if (cmpVal > double.MinValue)
                                cmpTxt = ((int)cmpVal).ToString();
                        }
                        else
                        {

                            for (int f = 1; f <= (int)numDecimals.Value; f++)
                            {
                                if (f == 1) formatStr += ".";
                                formatStr += "0";
                            }
                            //formatStr += "#";
                            curTxt = curVal.ToString(formatStr);
                            if (cmpVal > double.MinValue)
                                cmpTxt = cmpVal.ToString(formatStr);
                        }
                    }
                    if (cmpTCell == null)
                        cmpTxt = "";
                    dataGridView1.Rows[row].Cells[col].Value = curTxt + " [" + cmpTxt + "]";
                    dataGridView1.Rows[row].Cells[col].Tag = tCell;
                    if (curVal == cmpVal)
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightBlue;
                    else
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                    return;
                }

                //Not side by side text mode, continue...
                double showVal = curVal;
                double showRawVal = curRawValue;
                if (cmpTCell != null)
                {
                    if (showMode == ShowMode.diff)
                    {
                        if (radioMultiplier.Checked)
                            showVal = curVal / cmpVal;
                        else if (radioPercent.Checked)
                            showVal = curVal / cmpVal * 100 - 100;
                        else
                            showVal = curVal - cmpVal;
                        showRawVal = curRawValue - cmpRawValue;
                    }
                    else if (showMode == ShowMode.diff2)
                    {
                        if (radioMultiplier.Checked)
                            showVal = cmpVal / curVal;
                        else if (radioPercent.Checked)
                            showVal = cmpVal / curVal * 100 - 100;
                        else
                            showVal = cmpVal - curVal;
                        showRawVal = cmpRawValue - curRawValue;

                    }
                }

                if (showRawHex)
                {
                    if (mathTd.OutputType == OutDataType.Bitmap)
                    {
                        dataGridView1.Rows[row].Cells[col].Value = Convert.ToByte(tCell.lastRawValue).ToString("X2") + " [" + Convert.ToString((byte)curRawValue, 2).PadLeft(8,'0') + "]";
                    }
                    else
                    {
                        if (!addressToolStripMenuItem.Checked && !binaryToolStripMenuItem.Checked && !decimalToolStripMenuItem.Checked)
                        {
                            switch (mathTd.DataType)
                            {
                                case InDataType.FLOAT32:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToSingle(tCell.lastRawValue);
                                    break;
                                case InDataType.FLOAT64:
                                    dataGridView1.Rows[row].Cells[col].Value = tCell.lastRawValue;
                                    break;
                                case InDataType.INT64:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToInt64(tCell.lastRawValue);
                                    break;
                                case InDataType.INT32:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToInt32(tCell.lastRawValue);
                                    break;
                                case InDataType.UINT64:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToUInt64(tCell.lastRawValue);
                                    break;
                                case InDataType.UINT32:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToUInt32(tCell.lastRawValue);
                                    break;
                                case InDataType.SWORD:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToInt16(tCell.lastRawValue);
                                    break;
                                case InDataType.UWORD:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToUInt16(tCell.lastRawValue);
                                    break;
                                case InDataType.SBYTE:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToSByte(tCell.lastRawValue);
                                    break;
                                case InDataType.UBYTE:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToByte(tCell.lastRawValue);
                                    break;
                                default:
                                    dataGridView1.Rows[row].Cells[col].Value = Convert.ToInt32(tCell.lastRawValue);
                                    break;
                            }
                        }
                        else
                        {
                            string formatStr = "X" + (mathTd.ElementSize() * 2).ToString();
                            StringBuilder hexStr = new StringBuilder();
                            if (addressToolStripMenuItem.Checked)
                                hexStr.Append(tCell.addr.ToString("X8") + ": ");
                            if (Convert.ToDouble(tCell.lastValue) < 0)
                                Debug.WriteLine("Neg");
                            hexStr.Append(string.Join("", Array.ConvertAll(tCell.lastRawBytes, b => b.ToString("X2"))));
                            if (binaryToolStripMenuItem.Checked)
                                hexStr.Append(" [" + string.Join("", Array.ConvertAll(tCell.lastRawBytes, b => Convert.ToString(b, 2))).PadLeft(mathTd.ElementSize() * 8, '0') + "]");
                            if (decimalToolStripMenuItem.Checked)
                            {
                                switch (mathTd.DataType)
                                {
                                    case InDataType.FLOAT32:
                                        hexStr.Append("[" + Convert.ToSingle(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.FLOAT64:
                                        hexStr.Append("[" + tCell.lastRawValue + "]");
                                        break;
                                    case InDataType.INT64:
                                        hexStr.Append("[" + Convert.ToInt64(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.INT32:
                                        hexStr.Append("[" + Convert.ToInt32(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.UINT64:
                                        hexStr.Append("[" + Convert.ToUInt64(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.UINT32:
                                        hexStr.Append("[" + Convert.ToUInt32(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.SWORD:
                                        hexStr.Append("[" + Convert.ToInt16(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.UWORD:
                                        hexStr.Append("[" + Convert.ToUInt16(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.SBYTE:
                                        hexStr.Append("[" + Convert.ToSByte(tCell.lastRawValue) + "]");
                                        break;
                                    case InDataType.UBYTE:
                                        hexStr.Append("[" + Convert.ToByte(tCell.lastRawValue) + "]");
                                        break;
                                    default:
                                        hexStr.Append("[" + Convert.ToInt32(tCell.lastRawValue) + "]");
                                        break;
                                }
                            }
                            dataGridView1.Rows[row].Cells[col].Value = hexStr.ToString();
                        }
                    }
                }
                else
                {
                    switch (mathTd.OutputType)
                    {
                        case OutDataType.Float:
                            dataGridView1.Rows[row].Cells[col].Value = showVal;
                            break;
                        case OutDataType.Bitmap:
                            dataGridView1.Rows[row].Cells[col].Value = (int)showVal;
                            break;
                        case OutDataType.Flag:
                            dataGridView1.Rows[row].Cells[col].Value = (int)showVal;
                            break;
                        case OutDataType.Hex:
                            dataGridView1.Rows[row].Cells[col].Value = (uint)showVal;
                            break;
                        case OutDataType.Int:
                            dataGridView1.Rows[row].Cells[col].Value = (int)showVal;
                            break;
                        default:
                            dataGridView1.Rows[row].Cells[col].Value = showVal;
                            break;
                    }
                }
                dataGridView1.Rows[row].Cells[col].Tag = tCell;
                SetCellColor(row, col,tCell);
                if (!disableTooltips && mathTd.TableDescription != null)
                {
                    if (mathTd.TableDescription.Length > 200)
                        dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription.Substring(0, 200);
                    else
                        dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }

        private static Color GetGradientColor(double value, double minVal, double maxVal)
        {
            if (maxVal <= minVal) return Color.White;
            double t = Math.Max(0.0, Math.Min(1.0, (value - minVal) / (maxVal - minVal)));
            double scaled = t * (gradientStops.Length - 1);
            int i = Math.Min((int)scaled, gradientStops.Length - 2);
            double frac = scaled - i;
            Color a = gradientStops[i], b = gradientStops[i + 1];
            return Color.FromArgb(
                (int)(a.R + frac * (b.R - a.R)),
                (int)(a.G + frac * (b.G - a.G)),
                (int)(a.B + frac * (b.B - a.B))
            );
        }

        private void SetCellColor(int row, int col, TableCell tCell)
        {
            try
            {
                if (row < 0 || row >= dataGridView1.Rows.Count || col < 0 || col >= dataGridView1.Columns.Count)
                {
                    Debug.WriteLine("Error, position " + row.ToString() + "," + col.ToString() + " out of grid");
                    return;
                }
                TableData mathTd = tCell.td;
                double curVal = Convert.ToDouble(tCell.lastValue);
                double origVal = Convert.ToDouble(tCell.origValue);
                Color[] colors =
                     {
                        //Color.FromArgb(255, 255, 192, 192), //Pink?
                        Color.FromArgb(255, 255, 224, 192),
                        Color.FromArgb(255, 255, 255, 192),
                        Color.FromArgb(255, 192, 255, 192),
                        Color.FromArgb(255, 192, 255, 255),
                        Color.FromArgb(255, 192, 192, 255),
                        Color.FromArgb(255, 255, 192, 255),
                        Color.FromArgb(255, 224, 224, 224),
                        Color.FromArgb(255, 255, 128, 128),
                        Color.FromArgb(255, 255, 192, 128),
                        Color.FromArgb(255, 255, 255, 128),
                        Color.FromArgb(255, 128, 255, 128),
                        Color.FromArgb(255, 128, 255, 255),
                        Color.FromArgb(255, 128, 128, 255),
                        Color.FromArgb(255, 255, 128, 255),
                        Color.Silver,
                        Color.Red,
                        Color.FromArgb(255, 255, 128, 0),
                        Color.Yellow,
                        Color.Lime,
                        Color.Cyan,
                        Color.Blue,
                        Color.Fuchsia,
                        Color.Gray,
                        Color.FromArgb(255, 192, 0, 0),
                        Color.FromArgb(255, 192, 64, 0),
                        Color.FromArgb(255, 192, 192, 0),
                        Color.FromArgb(255, 0, 192, 0),
                        Color.FromArgb(255, 0, 192, 192),
                        Color.FromArgb(255, 0, 0, 192),
                        Color.FromArgb(255, 192, 0, 192),
                        Color.FromArgb(255, 64, 64, 64),
                        Color.Maroon,
                        Color.FromArgb(255, 128, 64, 0),
                        Color.Olive,
                        Color.Green,
                        Color.Teal,
                        Color.Navy,
                        Color.Purple,
                        Color.Black,
                        Color.FromArgb(255, 64, 0, 0),
                        Color.FromArgb(255, 128, 64, 64),
                        Color.FromArgb(255, 64, 64, 0),
                        Color.FromArgb(255, 0, 64, 0),
                        Color.FromArgb(255, 0, 64, 64),
                        Color.FromArgb(255, 0, 0, 64),
                        Color.FromArgb(255, 64, 0, 64),
                    };

                if (showMode == ShowMode.sideBySide || showMode == ShowMode.compareAll)
                {
                    if (tCell.tableInfo.compareFile.pcm.FileName != compareFiles[0].pcm.FileName)
                    {
                        //Compare Cell
                        string colTxt = "[" + compareFiles[0].fileLetter + "]" + dataGridView1.Columns[col].HeaderText.Substring(3);
                        string rowTxt = dataGridView1.Rows[row].HeaderCell.Value.ToString();
                        int orgCol = GetColumnByHeader(colTxt);
                        int orgRow = GetRowByHeader(rowTxt);
                        if (dataGridView1.Rows[orgRow].Cells[orgCol].Tag != null)
                        {
                            TableCell tOrigCell = (TableCell)dataGridView1.Rows[orgRow].Cells[orgCol].Tag;
                            if (Convert.ToDouble(tOrigCell.lastValue) != Convert.ToDouble(tCell.lastValue))
                            {
                                dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightPink;
                            }
                            else
                            {
                                string fLetter = dataGridView1.Columns[col].HeaderText.Substring(1, 1);
                                char fl = fLetter[0];
                                int nr = fl - 'A';
                                if (nr > colors.Length - 1)
                                    nr = colors.Length - 1;
                                dataGridView1.Rows[row].Cells[col].Style.BackColor = colors[nr];
                            }
                        }
                        return;
                    }
                }
                if (dataGridView1.Columns[col].GetType() != typeof(DataGridViewComboBoxColumn) &&
                    dataGridView1.Rows[row].Cells[col].GetType() != typeof(DataGridViewComboBoxCell))
                {
                    //Debug.WriteLine("Setting color for " + row.ToString() + ", " + col.ToString() +", Current value: " + curVal.ToString() +", origVal: " + origVal.ToString());
                    if (dataGridView1.Rows[row].Cells[col].ReadOnly)
                    {
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.LightGray;
                    }
                    else if (curVal != origVal)
                    {
                        dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.Yellow;
                        if (!disableTooltips)
                        {
                            dataGridView1.Rows[row].Cells[col].ToolTipText = "Original value: " + origVal.ToString();
                        }
                    }
                    else
                    {
                        if (AppSettings.TunerColorsMode != ConditionalColors.Off)
                        {
                            dataGridView1.Rows[row].Cells[col].Style.BackColor =
                                                           GetGradientColor(curVal, tCell.tableInfo.MinVal, tCell.tableInfo.MaxVal);
                        }
                        if (!disableTooltips)
                            dataGridView1.Rows[row].Cells[col].ToolTipText = mathTd.TableDescription;
                    }
                }
                else if (dataGridView1.Columns[col].GetType() == typeof(DataGridViewComboBoxColumn) || dataGridView1.Rows[row].Cells[col].GetType() == typeof(DataGridViewComboBoxCell))
                {
                    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView1.Rows[row].Cells[col];
                    if (curVal != origVal)
                    {
                        cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Italic);
                        if (!disableTooltips)
                            cell.ToolTipText = "Original value: " + origVal.ToString();
                    }
                    else
                    {
                        cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
                        if (!disableTooltips)
                            cell.ToolTipText = mathTd.TableDescription;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private int GetColumnByHeader(string hdrTxt)
        {
            int ind;
            hdrTxt = hdrTxt.Trim();
            if (dgColumnHeaders.ContainsKey(hdrTxt))
            {
                ind = dgColumnHeaders[hdrTxt];
            }
            else
            {
                ind = dataGridView1.Columns.Add(hdrTxt, hdrTxt);
                dgColumnHeaders.Add(hdrTxt, ind);
            }
            return ind;
        }

        private int GetRowByHeader(string hdrTxt)
        {
            int ind;
            hdrTxt = hdrTxt.Trim();
            if (dgRowHeaders.ContainsKey(hdrTxt))
            {
                ind = dgRowHeaders[hdrTxt];
            }
            else
            {
                ind = dataGridView1.Rows.Add();
                dataGridView1.Rows[ind].HeaderCell.Value = hdrTxt;
                dgRowHeaders.Add(hdrTxt, ind);
            }
            return ind;
        }

        private void AddCellByType(TableData ft, int gridRow, int gridCol)
        {
            if (showMode == ShowMode.sideBySideTxt || showRawHex)
                return;
            try
            {
                TableValueType vt = ft.ValueType();
                if (vt == TableValueType.boolean || vt == TableValueType.bitmask || vt == TableValueType.bitmap)
                {
                    DataGridViewCheckBoxCell dgc = new DataGridViewCheckBoxCell();
                    dgc.Style.NullValue = false;
                    dataGridView1.Rows[gridRow].Cells[gridCol] = dgc;
                    if (vt == TableValueType.bitmap && dataGridView1.Rows[gridRow].HeaderCell.Value.ToString().ToLower().StartsWith("not defined"))
                    {
                        dataGridView1.Rows[gridRow].Cells[gridCol].ReadOnly = true;
                    }
                }
                else if (vt == TableValueType.selection)
                {
                    DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
                    if (ft.OutputType == OutDataType.Float)
                    {
                        Dictionary<double, string> possibleVals = ParseEnumHeaders(ft.Values);
                        dgc.DataSource = new BindingSource(possibleVals, null);
                    }
                    else
                    {
                        Dictionary<int, string> possibleVals = ParseIntEnumHeaders(ft.Values);
                        dgc.DataSource = new BindingSource(possibleVals, null);
                    }
                    dgc.ValueMember = "key";
                    dgc.DisplayMember = "value";
                    dataGridView1.Rows[gridRow].Cells[gridCol] = dgc;
                }
                else
                {
                    //at least one table which difference can be shown
                    enableDiff = true;
                }

            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);

            }
        }

        public void LoadTable()
        {
            try
            {
                if (compareFiles.Count == 0)
                {
                    Debug.WriteLine("Table Editor: LoadTable: No files loaded, exit");
                    return;
                }
                if (groupSelectCompare.Enabled == false)
                {
                    //If only one file loaded, can't use any of compare modes
                    radioOriginal.Checked = true;
                }
                this.dataGridView1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                this.numExtraOffset.ValueChanged -= new System.EventHandler(this.numExtraOffset_ValueChanged);
                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                dataGridView1.ColumnHeadersHeightSizeMode =  DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                dgColumnHeaders = new Dictionary<string, int>();
                dgRowHeaders = new Dictionary<string, int>();

                CompareFile selectedFile = compareFiles[currentFile];
                PcmFile PCM = selectedFile.pcm;
                TableData td = selectedFile.tableInfos[0].td;
                //ShowTdinHexWindow(td, PCM.buf);

                showRawHex = showRawHEXValuesToolStripMenuItem.Checked;
                disableTooltips = disableTooltipsToolStripMenuItem.Checked;
                enableDiff = false;
                numExtraOffset.Value = td.extraoffset;

                if (td.Units.ToLower().Contains("bitmask"))
                    labelUnits.Text = "Units: Boolean";
                else
                    labelUnits.Text = "Units: " + (td.Units ?? "");
                if (td.ValueType() == TableValueType.selection)
                    labelUnits.Text += ", Values: " + td.Values;

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                bool xySwapped = chkSwapXY.Checked;

                //if (selectedFile.Cols > 5 && selectedFile.Rows < 5)
                if (compareFiles[0].Cols > 5 && compareFiles[0].Rows < 5)
                    xySwapped = !chkSwapXY.Checked;

                List<CompareFile> cmpFiles = new List<CompareFile>();
                CompareFile diffFile = null;
                if (radioDifference.Checked || radioDifference2.Checked || radioSideBySideText.Checked)
                {
                    diffFile = compareFiles[currentCmpFile];
                }
                if (radioSideBySide.Checked && compareFiles.Count > currentCmpFile)
                {
                    diffFile = compareFiles[currentCmpFile];
                    cmpFiles.Add(compareFiles[currentCmpFile]);
                }
                if (radioCompareAll.Checked)
                {
                    for (int i = 1; i < compareFiles.Count; i++)
                    {
                        cmpFiles.Add(compareFiles[i]);
                    }
                }

                compareTableInfos = new TableInfo[compareFiles.Count];
                CompareFile sFile = compareFiles[currentFile];
                ShowTdinHexWindow(sFile.tableInfos[0].tableCells[0]);
                Debug.WriteLine("LoadTable, ShowTdInHewindow done, time Taken: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
                for (int tbl = 0; tbl < sFile.tableInfos.Count; tbl++)
                {
                    TableData ft = sFile.tableInfos[tbl].td;
                    TableInfo cmpTinfo = null;

                    int gridCol = 0;
                    int gridRow = 0;
                    SetupColorRanges(sFile.tableInfos[tbl]);
                    //Find maximum cell count from all comparefiles:
                    int cellCount = sFile.tableInfos[tbl].tableCells.Count;
                    for (int d = 1; d < compareFiles.Count; d++)
                    {
                        TableData cmpTd = FindTableData(ft, compareFiles[d].pcm.tableDatas);
                        if (cmpTd != null)
                        {
                            cmpTinfo = compareFiles[d].tableInfos.Where(X=>X.td.TableName == cmpTd.TableName).FirstOrDefault();
                            if (cmpTinfo != null)
                            {
                                compareTableInfos[d] = cmpTinfo;
                                if (cmpTinfo.tableCells.Count > cellCount)
                                {
                                    cellCount = cmpTinfo.tableCells.Count;
                                }
                            }
                        }
                    }

                    DrawingControl.SuspendDrawing(dataGridView1);
                    uint prevAddr = 0;
                    for (int cell = 0; cell < cellCount; cell++)
                    {
                        TableCell cmpCell = null;
                        if (sFile.tableInfos[tbl].tableCells.Count > cell)
                        {
                            //Original file may have less cells than some of compare files?
                            TableCell tcell = sFile.tableInfos[tbl].tableCells[cell];
                            if (diffFile != null)   //RadioDifference checked
                            {

                                if (compareTableInfos[currentCmpFile] != null)
                                {
                                    cmpTinfo = compareTableInfos[currentCmpFile];
                                    cmpCell = cmpTinfo.tableCells.Where(X => X.RowhHeader == tcell.RowhHeader && X.ColHeader == tcell.ColHeader).FirstOrDefault();
                                }
                            }
                            string colHdr;
                            string rowHdr;
                            if (!xySwapped)
                            {
                                if (ft.OutputType == OutDataType.Bitmap && showRawHex)
                                {
                                    if (tcell.addr == prevAddr)
                                    {
                                        //Bitmapped table have max 8 cells/address, show only 1 cell/address
                                        continue;
                                    }
                                    prevAddr = tcell.addr;
                                    rowHdr = tcell.addr.ToString("X2");
                                    colHdr = "";
                                }
                                else
                                {
                                    colHdr = tcell.ColHeader;
                                    rowHdr = tcell.RowhHeader;
                                }
                                if (only1d)
                                {
                                    colHdr = "[" + selectedFile.fileLetter + "] "; //Show only [A]
                                    rowHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                                else if (showMode == ShowMode.sideBySide || showMode == ShowMode.compareAll)
                                {
                                    if (multiSelect)
                                        colHdr = "[" + selectedFile.fileLetter + "] " + "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                    else
                                        colHdr = "[" + selectedFile.fileLetter + "] " + tcell.ColHeader;
                                }
                                else if (multiSelect)
                                {
                                    colHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                            }
                            else
                            {
                                if (ft.OutputType == OutDataType.Bitmap && showRawHex)
                                {
                                    if (tcell.addr == prevAddr)
                                    {
                                        //Bitmapped table have max 8 cells/address, show only 1 cell/address
                                        continue;
                                    }
                                    prevAddr = tcell.addr;
                                    colHdr = tcell.addr.ToString("X2");
                                    rowHdr = "";
                                }
                                else
                                {
                                    colHdr = tcell.RowhHeader;
                                    rowHdr = tcell.ColHeader;
                                }
                                if (only1d)
                                {
                                    rowHdr = "[" + selectedFile.fileLetter + "] "; //Show only [A]
                                    colHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                                else if (showMode == ShowMode.sideBySide || showMode == ShowMode.compareAll)
                                {
                                    if (multiSelect)
                                    {
                                        rowHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                        colHdr = "[" + selectedFile.fileLetter + "] " + tcell.RowhHeader;
                                    }
                                    else
                                    {
                                        rowHdr = tcell.ColHeader;
                                        colHdr = "[" + selectedFile.fileLetter + "] " + tcell.RowhHeader;
                                    }
                                }
                                else if (multiSelect)
                                {
                                    rowHdr = "[" + tcell.td.TableName + "] " + tcell.ColHeader;
                                }
                            }
                            gridCol = GetColumnByHeader(colHdr);
                            gridRow = GetRowByHeader(rowHdr);
                            //Debug.WriteLine("Grid col: " + gridCol.ToString() + ", Gridrow: " + gridRow.ToString());
                            AddCellByType(ft, gridRow, gridCol);
                            SetCellValue(gridRow, gridCol, tcell, cmpCell);
                        }
                        for (int d = 0; d < cmpFiles.Count; d++)
                        {

                            if (radioCompareAll.Checked)
                            {
                                bool selected = false;
                                for (int x = 0; x < fileCheckBoxes.Count; x++)
                                    if (fileCheckBoxes[x].Text == cmpFiles[d].fileLetter && fileCheckBoxes[x].Checked)
                                        selected = true;
                                if (!selected)
                                    continue;   
                            }
                            TableData compTd = null;
                            if (compareTableInfos[currentCmpFile] != null)
                            {
                                cmpTinfo = compareTableInfos[currentCmpFile];
                                compTd = compareTableInfos[currentCmpFile].td;
                            }

                            if (compTd != null)
                            {
                                if (cmpTinfo.tableCells.Count > cell)
                                {
                                    cmpCell = cmpTinfo.tableCells[cell];
                                    string cmpColHdr = "";
                                    string cmpRowHdr = "";
                                    if (!xySwapped)
                                    {
                                        cmpColHdr = cmpCell.ColHeader;
                                        cmpRowHdr = cmpCell.RowhHeader;
                                        if (only1d)
                                        {
                                            cmpColHdr = "[" + cmpFiles[d].fileLetter + "] ";
                                            cmpRowHdr = "[" + compTd.TableName + "] " + cmpCell.ColHeader;
                                        }
                                        else
                                        {
                                            if (multiSelect)
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + "[" + compTd.TableName + "] " + cmpCell.ColHeader;
                                            else
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + cmpCell.ColHeader;
                                        }
                                        if (ft.OutputType == OutDataType.Bitmap && showRawHex)
                                        {
                                            cmpRowHdr = cmpCell.addr.ToString("X2");
                                        }
                                    }
                                    else
                                    {
                                        cmpRowHdr = cmpCell.ColHeader;
                                        cmpColHdr = cmpCell.RowhHeader;
                                        if (only1d)
                                        {
                                            cmpRowHdr = "[" + cmpFiles[d].fileLetter + "] ";
                                            cmpColHdr = "[" + compTd.TableName + "] " + cmpCell.ColHeader;
                                        }
                                        else
                                        {
                                            if (multiSelect)
                                            {
                                                cmpRowHdr = "[" + compTd.TableName + "] " + cmpCell.ColHeader;
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + cmpCell.RowhHeader;
                                            }
                                            else
                                            {
                                                cmpRowHdr = cmpCell.ColHeader;
                                                cmpColHdr = "[" + cmpFiles[d].fileLetter + "] " + cmpCell.RowhHeader;
                                            }
                                        }
                                        if (ft.OutputType == OutDataType.Bitmap && showRawHex)
                                        {
                                            cmpColHdr = cmpCell.addr.ToString("X2");
                                        }
                                    }
                                    gridCol = GetColumnByHeader(cmpColHdr);
                                    gridRow = GetRowByHeader(cmpRowHdr);
                                    AddCellByType(cmpCell.td, gridRow, gridCol);
                                    SetCellValue(gridRow, gridCol, cmpCell, null);
                                }
                            }

                        }

                    }
                }
                Debug.WriteLine("LoadTable, SetCellValue done for main all files, time Taken: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));


                if (td.TableName.StartsWith("DTC") && (td.OutputType != OutDataType.Bitmap || showRawHex == false))
                {
                    ShowDtcDescriptions();
                }


                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        if (dataGridView1.Rows[r].Cells[c].Tag == null)
                        {
                            dataGridView1.Rows[r].Cells[c].ReadOnly = true;
                            if (radioSideBySide.Checked)
                            {
                                if (dataGridView1.Rows[r].Cells[c].Value == null)
                                    dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.DarkGray;
                            }
                            else
                            {
                                dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.DarkGray;
                            }
                        }
                    }
                }
                SetDataGridLayout(td);
                dataGridView1.EndEdit();
                //ShowCellInfo((TableCell)dataGridView1.Rows[0].Cells[0].Tag, false);
                Debug.WriteLine("LoadTable time before resume layout: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
                DrawingControl.ResumeDrawing(dataGridView1);
                if (enableDiff)
                {
                    radioDifference.Enabled = true;
                    radioDifference2.Enabled = true;
                }
                stopwatch.Stop();
                Debug.WriteLine("LoadTable time Taken: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
                if (ftvd != null && ftvd.Visible)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        ftvd.UpdateDisplay(true);
                    });
                }

                for (int t = 0; t < tunerFilteredTables.Count; t++)
                {
                    if (tunerFilteredTables[t].guid == td.guid)
                    {
                        currentTunerTd = t;
                        break;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
            this.numExtraOffset.ValueChanged += new System.EventHandler(this.numExtraOffset_ValueChanged);
        }

        private void ShowDtcDescriptions()
        {
            try
            {
                DtcSearch ds = new DtcSearch();
                if (OBD2Codes == null || OBD2Codes.Count == 0)
                    LoadOBD2Codes();
                if (OBD2Codes.Count == 0)
                    return;
                chkSwapXY.Enabled = false;
                searchCodeFromGoogleToolStripMenuItem.Visible = true;
                DataGridViewColumn dgc = new DataGridViewColumn();
                dgc.Name = "Description";
                dgc.HeaderText = "Description";
                dgc.CellTemplate = new DataGridViewTextBoxCell();
                dataGridView1.Columns.Insert(0, dgc);
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    string descr = DtcSearch.GetDtcDescription(dataGridView1.Rows[r].HeaderCell.Value.ToString());
                    dataGridView1.Rows[r].Cells["Description"].Value = descr;
                    if (dataGridView1.Rows[r].Cells[1].Tag != null)
                    {
                        TableCell tc = (TableCell)dataGridView1.Rows[r].Cells[1].Tag;
                        TableCell tcDescr = tc.ShallowCopy();
                        tcDescr.addr = uint.MaxValue - 1;
                        dataGridView1.Rows[r].Cells["Description"].Tag = tcDescr;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void SetDataGridLayout(TableData td)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (numDecimals.Value < 0 && td != null)
                {
                    this.numDecimals.ValueChanged -= new System.EventHandler(this.numDecimals_ValueChanged);
                    numDecimals.Value = td.Decimals;
                    decimals = td.Decimals;
                    this.numDecimals.ValueChanged += new System.EventHandler(this.numDecimals_ValueChanged);
                }
                string formatStr = "0";
                if ((showRawHEXValuesToolStripMenuItem.Checked && !addressToolStripMenuItem.Checked && 
                    !binaryToolStripMenuItem.Checked && !decimalToolStripMenuItem.Checked )|| td.OutputType == OutDataType.Hex)
                {
                    formatStr = "X" + (td.ElementSize() * 2).ToString();
                }
                else if (td.OutputType == OutDataType.Text || td.OutputType == OutDataType.Flag || td.OutputType == OutDataType.Bitmap)
                {
                    formatStr = "";
                }
                else
                {
                    for (int f = 1; f <= (int)numDecimals.Value ; f++)
                    {
                        if (f == 1) formatStr += ".";
                        formatStr += "0";
                    }
                    //formatStr += "#";
                }
                foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
                {
                    dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (showRawHEXValuesToolStripMenuItem.Checked || td.OutputType == OutDataType.Hex)
                        dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    else if (dgvc.HeaderText != "Description")
                        dgvc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvc.DefaultCellStyle.Font = dataFont;
                    if (formatStr != "" && dgvc.GetType() != typeof(DataGridViewComboBoxColumn) )
                        dgvc.DefaultCellStyle.Format = formatStr;
                }
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                if (autoResizeToolStripMenuItem.Checked) AutoResize();
                stopwatch.Stop();
                Debug.WriteLine("setDataGridLayout time Taken: " + stopwatch.Elapsed.TotalMilliseconds.ToString("#,##0.00 'milliseconds'"));
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                TableCell tc = new TableCell(); 
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                {
                    tc = (TableCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                }
                if (tc == null) return;
                if ( dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null || String.IsNullOrWhiteSpace(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                {
                    if (tc.lastValue != null)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = tc.lastValue;
                        return;
                    }
                }

                TableData td = tc.td;
                if (e.RowIndex > -1)
                {
                    if (td.TableName.StartsWith("DTC") && tc.addr == (uint.MaxValue - 1))
                    {
                        //OBD2 Description
                        OBD2Code oc = new OBD2Code();
                        oc.Code = dataGridView1.Rows[e.RowIndex].HeaderCell.Value.ToString();
                        oc.Description = dataGridView1.Rows[e.RowIndex].Cells["Description"].Value.ToString();
                        bool codeFound = false;
                        for (int o = 0; o < OBD2Codes.Count; o++)
                        {
                            if (OBD2Codes[o].Code == oc.Code)
                            {
                                OBD2Codes[o].Description = oc.Description;
                                codeFound = true;
                                break;
                            }
                        }
                        if (!codeFound)
                        {
                            OBD2Codes.Add(oc);
                        }
                    }
                    else
                    {
                        this.dataGridView1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                        if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != tc.lastValue)
                        {
                            SaveValue(e.RowIndex, e.ColumnIndex, tc);
                        }
                        SetCellColor(e.RowIndex, e.ColumnIndex, tc);
                        this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                    }
                }
                if (ftvd != null && ftvd.Visible)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        ftvd.UpdateDisplay(true);
                    });
                }
                int location = (int)(tc.addr);
                for (int i=0;i<tc.td.ElementSize();i++)
                {
                    hexpanel.SetByte(location +i, tc.lastRawBytes[i]);
                }
                hexpanel.AddHighlight(location, tc.td.ElementSize(), System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowModifiedColor), "Modified");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }
        public void SaveValue(int r, int c, TableCell tCell)
        {            
            double newValue = double.MinValue;
            TableData mathTd = tCell.td;
            try
            {
                if (chkRawHex.Checked && (addressToolStripMenuItem.Checked || binaryToolStripMenuItem.Checked || decimalToolStripMenuItem.Checked))
                {
                    Debug.WriteLine("Can't save in extended HEX mode");
                    return;
                }
                if (dataGridView1.Rows[r].Cells[c].GetType() == typeof(DataGridViewComboBoxCell))
                {
                    DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dataGridView1.Rows[r].Cells[c];
                    newValue = Convert.ToDouble(cb.Value);
                }
                else if (showRawHEXValuesToolStripMenuItem.Checked)
                {
                    newValue = (double)Convert.ToInt64(dataGridView1.Rows[r].Cells[c].Value.ToString(), 16);
                }
                else
                {
                    if (tCell.td.OutputType == OutDataType.Hex)
                    {
                        newValue = (double)Convert.ToInt64(dataGridView1.Rows[r].Cells[c].Value.ToString(), 16);
                    }
                    else
                    {
                        newValue = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
                    }
                    if (radioDifference.Checked)
                    {
                        if (radioAbsolute.Checked)
                            newValue = (double)tCell.lastValue - newValue;
                        else if (radioMultiplier.Checked)
                            newValue = (double)tCell.cmpValue * newValue;
                        else if (radioPercent.Checked)
                            newValue =  (100 + newValue) / 100 * (double)tCell.cmpValue;
                    }
                    else if (radioDifference2.Checked)
                    {
                        if (radioAbsolute.Checked)
                            newValue =  newValue + (double)tCell.lastValue;
                        else if (radioMultiplier.Checked)
                            newValue = (double)tCell.cmpValue / newValue;
                        else if(radioPercent.Checked)
                            newValue = (100 - newValue) / 100 * (double)tCell.cmpValue;
                    }
                }
                
                if (newValue == double.MaxValue) return;

                if (showRawHEXValuesToolStripMenuItem.Checked)
                {
                    tCell.SetValue(newValue, true);
                }
                else
                { 
                    if (dataGridView1.Columns[c].GetType() != typeof(DataGridViewComboBoxColumn)
                        && dataGridView1.Rows[r].Cells[c].GetType() != typeof(DataGridViewComboBoxCell))
                    {
                        if (newValue > mathTd.Max)
                            //  newValue = mathTd.Max;
                            Logger("Warning: Value " + newValue.ToString() + " > Max value (" + mathTd.Max.ToString() + ")");
                        if (newValue < mathTd.Min)
                            //newValue = mathTd.Min;
                            Logger("Warning: Value " + newValue.ToString() + " < Max value (" + mathTd.Min.ToString() + ")");

                    }
                    tCell.SetValue(newValue);
                    if (radioDifference.Checked || radioDifference2.Checked)
                        LoadTable();
                    else
                        dataGridView1.Rows[r].Cells[c].Value = tCell.lastValue;
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
                dataGridView1.Rows[r].Cells[c].Value = tCell.lastValue;
            }
        }

        public void SaveTable(bool useDataGrid)
        {
            try
            {
                if (useDataGrid)
                    dataGridView1.EndEdit();
                for (int a = 0; a < compareFiles[0].tableInfos.Count; a++)
                {
                    if (compareFiles[0].tableInfos[a].isModified())
                    {
                        compareFiles[0].tableInfos[a].SaveCellsToPcmBuffer();
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    if (dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Tag != null && cell.Value != null)
                    {
                        string mathStr = txtMath.Text.ToLower().Replace("x", cell.Value.ToString());
                        double newvalue = parser.Parse(mathStr);
                        cell.Value = newvalue;
                        TableCell tc = (TableCell)dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Tag;
                        SaveValue(cell.RowIndex, cell.ColumnIndex, tc);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void AutoResize()
        {
            try
            {
                if (this.ParentForm != null || AutoResizeTmpDisabled)
                {
                    //No resize if docked to tuner panel
                    return;
                }
                int dgv_width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + dataGridView1.RowHeadersWidth;
                int dgv_height = dataGridView1.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridView1.ColumnHeadersHeight;
                if (AppSettings.TunerHexWindowShow)
                {
                    SizeF f = hexpanel.RequiredSizeForBrackets();
                    dgv_height = Math.Max(dgv_height, (int)(f.Height + 30));
                    int hexWidth = splitContainer1.Width - (int)(f.Width);
                    if (hexWidth > 0)
                    {
                        splitContainer1.SplitterDistance = hexWidth;
                        AppSettings.TunerHexWindowWidth = splitContainer1.Panel2.Width;
                    }
                    dgv_width += (int)f.Width + 10;
                }
                if (dgv_width < 550) dgv_width = 550;
                Screen myScreen = Screen.FromPoint(MousePosition);
                System.Drawing.Rectangle area = myScreen.WorkingArea;
                if ((dgv_width + 150) > area.Width)
                    this.Width = area.Width - 50;
                else
                    this.Width = dgv_width + 50; //150
                if ((dgv_height + 100) > area.Height)
                    this.Height = area.Height - 50;
                else
                    this.Height = dgv_height + 150; //175
                if (AppSettings.TunerHexWindowShow)
                {
                    hexpanel.ScrollToBrackets();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Copy to clipboard
            CopyToClipboard();

            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                dgvCell.Value = string.Empty;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Perform paste Operation
            PasteClipboardValue();
        }
        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void PasteClipboardValue()
        {
            try
            {
                //Show Error if no cell is selected
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please select a cell", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                dataGridView1.BeginEdit(true);

                //Get the starting Cell
                DataGridViewCell startCell = GetStartCell(dataGridView1);
                //Get the clipboard value in a dictionary
                Dictionary<int, Dictionary<int, string>> cbValue =
                        ClipBoardValues(Clipboard.GetText());

                int iRowIndex = startCell.RowIndex;
                foreach (int rowKey in cbValue.Keys)
                {
                    int iColIndex = startCell.ColumnIndex;
                    foreach (int cellKey in cbValue[rowKey].Keys)
                    {
                        //Check if the index is within the limit
                        if (iColIndex <= dataGridView1.Columns.Count - 1
                        && iRowIndex <= dataGridView1.Rows.Count - 1)
                        {
                            DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                            //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                            //if ((chkPasteToSelectedCells.Checked && cell.Selected) || (!chkPasteToSelectedCells.Checked))
                            cell.Value = cbValue[rowKey][cellKey];
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
                dataGridView1.EndEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionary with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }

        private void ExportCsv()
        {
            try
            {

                string FileName = SelectSaveFile(CsvFilter);
                if (FileName.Length == 0)
                    return;
                using (StreamWriter writetext = new StreamWriter(FileName))
                {
                    string row = ";";
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        row += dataGridView1.Columns[i].HeaderText;
                    }
                    writetext.WriteLine(row);
                    for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                    {
                        row = dataGridView1.Rows[r].HeaderCell.Value.ToString() + ";";
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            if (i > 0)
                                row += ";";
                            if (dataGridView1.Rows[r].Cells[i].Value != null)
                                row += dataGridView1.Rows[r].Cells[i].Value.ToString();
                        }
                        writetext.WriteLine(row);
                    }
                }
                MessageBox.Show(FileName, "CSV Export done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void exportCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCsv();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTable(true);
        }

        private void exportCSVToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportCsv();
        }

        private void autoResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoResizeToolStripMenuItem.Checked)
                autoResizeToolStripMenuItem.Checked = false;
            else
                autoResizeToolStripMenuItem.Checked = true;
            AppSettings.TableEditorAutoResize = autoResizeToolStripMenuItem.Checked;
            AppSettings.Save();
            if (autoResizeToolStripMenuItem.Checked)
            {
                AutoResize();
            }

        }

        private void SetXYswapped(bool swapped)
        {
            this.chkSwapXY.CheckedChanged -= new System.EventHandler(this.chkSwapXY_CheckedChanged);
            chkSwapXY.Checked = swapped;
            tuner.SwapXy = swapped;
            this.chkSwapXY.CheckedChanged += new System.EventHandler(this.chkSwapXY_CheckedChanged);
        }

        private void chkSwapXY_CheckedChanged(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void showRawHEXValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showRawHEXValuesToolStripMenuItem.Checked = !showRawHEXValuesToolStripMenuItem.Checked;
            chkRawHex.Checked = showRawHEXValuesToolStripMenuItem.Checked;
            tuner.ShowAsHex = showRawHEXValuesToolStripMenuItem.Checked;
            LoadTable();
        }

        private void disableTooltipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disableTooltipsToolStripMenuItem.Checked)
            {
                disableTooltips = false;
                disableTooltipsToolStripMenuItem.Checked = false;
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        TableCell tc = (TableCell)dataGridView1.Rows[r].Cells[c].Tag;
                        if (tc.td.TableDescription != null && dataGridView1.Rows[r].Cells[c].ToolTipText == null)
                            dataGridView1.Rows[r].Cells[c].ToolTipText = tc.td.TableDescription;
                    }
                }
            }
            else
            {
                disableTooltips = true;
                disableTooltipsToolStripMenuItem.Checked = true;
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        dataGridView1.Rows[r].Cells[c].ToolTipText = null;
                    }
                }
            }
        }

        private void showGraphicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TableData td = compareFiles[currentFile].tableInfos[0].td;
                frmGraphics fg = new frmGraphics();
                fg.Text = td.TableName;
                fg.Show();
                fg.chart1.Series.Clear();
                double minVal = double.MaxValue;
                double maxVal = double.MinValue;

                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    fg.chart1.Series.Add(new Series());
                    fg.chart1.Series[r].ChartType = SeriesChartType.Line;
                    if (dataGridView1.Rows[r].HeaderCell.Value != null)
                        fg.chart1.Series[r].Name = dataGridView1.Rows[r].HeaderCell.Value.ToString();
                    fg.chart1.Series[r].ToolTip = "[#SERIESNAME][#VALX]: #VAL";
                    int point = 0;
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        double val = Convert.ToDouble(dataGridView1.Rows[r].Cells[c].Value);
                        if (val > maxVal) maxVal = val;
                        if (val < minVal) minVal = val;
                        fg.chart1.Series[r].Points.AddXY(dataGridView1.Columns[c].HeaderCell.Value, val);
                        fg.chart1.Series[r].Points[point].MarkerStyle = MarkerStyle.Circle;
                        fg.chart1.Series[r].Points[point].MarkerSize = 5;
                        point++;
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (!e.Exception.Message.Contains("DataGridViewComboBoxCell"))
                Debug.WriteLine(e.Exception);
        }

        private void SelectFile(string letter)
        {
            for (int l = 0; l < compareFiles.Count; l++)
            {
                if (compareFiles[l].fileLetter == letter)
                {
                    currentFile = l;
                    break;
                }
            }

        }

        private int FindFile(string letter)
        {
            for (int l = 0; l < compareFiles.Count; l++)
            {
                if (compareFiles[l].fileLetter == letter)
                {
                    return l;
                }
            }
            return -1;
        }


        private void radioCompareFile_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCompareFile.Checked)
            {
                tuner.CompareSelection = 1;
                showMode = ShowMode.compare;
                SelectFile(radioCompareFile.Text);
                dataGridView1.BackgroundColor = Color.Red;
                SetMyText();
                LoadTable();
            }

        }

        private void radioDifference_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDifference.Checked)
            {
                tuner.CompareSelection = 5;
                showMode = ShowMode.diff;
            }

            if (radioDifference.Checked || radioDifference2.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                SetMyText();
                groupDifference.Visible = true;
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = multiplierDecimals;
                }
                LoadTable();
            }
            else
            {
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = decimals;
                }

                groupDifference.Visible = false;
            }
        }

        private void radioSideBySide_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSideBySide.Checked)
            {
                tuner.CompareSelection = 2;
                showMode = ShowMode.sideBySide;
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                //disableSaving = true;
                SetMyText();
                LoadTable();
            }
            graphToolStripMenuItem.Enabled = !radioSideBySide.Checked;
            copyFromCompareToolStripMenuItem.Enabled = radioSideBySide.Checked;
        }

        private void radioOriginal_CheckedChanged(object sender, EventArgs e)
        {
            if (radioOriginal.Checked)
            {
                tuner.CompareSelection = 0;
                showMode = ShowMode.normal;
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Gray;
                SetMyText();
                LoadTable();
            }
        }

        private void SetMyText()
        {
            try
            {
                this.Text = "Tuner: " + compareFiles[currentFile].tableInfos[0].td.TableName + " [";
                if (radioOriginal.Checked)
                    this.Text += compareFiles[currentFile].pcm.FileName + "]";
                if (radioDifference.Checked || radioDifference2.Checked || radioSideBySide.Checked || radioSideBySideText.Checked)
                    this.Text += compareFiles[currentFile].pcm.FileName + " - " + compareFiles[currentCmpFile].pcm.FileName + "]";
                if (radioCompareFile.Checked)
                    this.Text += compareFiles[currentCmpFile].pcm.FileName + "]";
                if (radioCompareAll.Checked)
                    this.Text += compareFiles[currentFile].pcm.FileName + " - * ]";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SetMyText: " + ex.Message);
            }
        }

        private void numDecimals_ValueChanged(object sender, EventArgs e)
        {
            if ((radioDifference.Checked || radioDifference2.Checked) && radioMultiplier.Checked)
                multiplierDecimals = (int)numDecimals.Value;
            else
                decimals = (int)numDecimals.Value;
            LoadTable();
        }

        private void dataFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.Font = dataFont;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                dataFont = fontDlg.Font;
                AppSettings.TableEditorFont = SerializableFont.FromFont(dataFont);
                AppSettings.Save();
            }
            fontDlg.Dispose();
            LoadTable();
        }

        private void saveOBD2DescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            SaveOBD2Codes(null);
        }

        private void searchCodeFromGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableData td = compareFiles[currentFile].tableInfos[0].td;
            if (!td.TableName.StartsWith("DTC"))
                return;
            string url = "https://www.google.com/search?q=Chevrolet+" + dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].HeaderCell.Value.ToString();
            System.Diagnostics.Process.Start(url);

        }
        private void copyFromCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> rows = new List<int>();
                List<int> cols = new List<int>();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    rows.Add(dataGridView1.SelectedCells[i].RowIndex);
                    cols.Add(dataGridView1.SelectedCells[i].ColumnIndex);

                }
                dataGridView1.BeginEdit(true);
                for (int i = 0; i < rows.Count; i++)
                {
                    int row = rows[i];
                    int col = cols[i];
                    if (dataGridView1.Rows[row].Cells[col].Tag != null)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[row].Cells[col];
                        var val = dataGridView1.Rows[row].Cells[col + 1].Value;
                        dataGridView1.Rows[row].Cells[col].Value = val;
                    }
                }
                dataGridView1.EndEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void radioSideBySideText_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSideBySideText.Checked)
            {
                tuner.CompareSelection = 3;
                showMode = ShowMode.sideBySideTxt;
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                LoadTable();
                SetMyText();
            }
        }

        private void AddFileCheckBoxes()
        {
            if (fileCheckBoxes != null && fileCheckBoxes.Count == compareFiles.Count - 1)
            {
                for (int i = 0; i < fileCheckBoxes.Count; i++)
                    fileCheckBoxes[i].Visible = true;
                return;
            }
            int left = 186;
            fileCheckBoxes = new List<CheckBox>();
            for (int i=1; i < compareFiles.Count; i++)
            {
                CheckBox cBox = new CheckBox();
                cBox.Text = compareFiles[i].fileLetter;
                cBox.Checked = true;
                this.Controls.Add(cBox);
                fileCheckBoxes.Add(cBox);
                cBox.Location = new Point(left, 50);
                cBox.BringToFront();
                cBox.CheckedChanged += CBox_CheckedChanged;
                if (cBox.Text.Length == 1)
                    left += 30;
                else
                    left += 40;
            }
        }

        private void CBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void radioCompareAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCompareAll.Checked)
            {
                tuner.CompareSelection = 4;
                showMode = ShowMode.compareAll;
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                AddFileCheckBoxes();
                LoadTable();
                SetMyText();
            }
            else
            {
                for (int i = 0; i < fileCheckBoxes.Count; i++)
                    fileCheckBoxes[i].Visible = false;
            }
        }

        private void TuneCellValues(double step)
        {
            try
            {
                List<Point> SelectedCells = new List<Point>();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    Point p = new Point(dataGridView1.SelectedCells[i].RowIndex, dataGridView1.SelectedCells[i].ColumnIndex);
                    SelectedCells.Add(p);
                }
                dataGridView1.BeginEdit(true);
                //for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                foreach(Point p in SelectedCells)
                {
                    TableCell tCell = (TableCell)dataGridView1.Rows[p.X].Cells[p.Y].Tag;
                    TableData mathTd = tCell.td;
                    double rawVal = (double)tCell.lastRawValue;
                    double newRawVal = rawVal + step;
                    Debug.WriteLine("Row: " + p.X + ", col: " + p.Y + ", Old raw: " + tCell.lastRawValue + ", new raw: " + newRawVal);
                    tCell.SetValue(newRawVal, true);
                    double val = Convert.ToDouble(tCell.lastValue);

                    dataGridView1.Rows[p.X].Cells[p.Y].Value = val;
                    SetCellColor(p.X, p.Y, tCell);
                }
                this.dataGridView1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                dataGridView1.EndEdit();
                this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellValueChanged);
                foreach (Point p in SelectedCells)
                {
                    dataGridView1.Rows[p.X].Cells[p.Y].Selected = true;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void numTuneValue_ValueChanged(object sender, EventArgs e)
        {
            if (radioDifference.Checked || radioDifference2.Checked || dataGridView1.SelectedCells.Count == 0)
                return;
            decimal oldVal = (decimal)numTuneValue.Tag;
            decimal newVal = numTuneValue.Value;
            Debug.WriteLine("Old:" + oldVal + ", new: " + newVal);
            if (newVal > oldVal) 
                TuneCellValues(1);
            else
                TuneCellValues(-1);
            numTuneValue.Tag = newVal;
        }

        private void radioAbsolute_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAbsolute.Checked)
            {
                tuner.CompareType = 0;
                LoadTable();
            }
        }

        private void radioMultiplier_CheckedChanged(object sender, EventArgs e)
        {
            if (radioMultiplier.Checked)
            {
                tuner.CompareType = 1;
                numDecimals.Value = multiplierDecimals;
                LoadTable();
            }
            else
            {
                numDecimals.Value = decimals;
            }
        }

        private void radioPercent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPercent.Checked)
            {
                tuner.CompareType = 2;
                //disableSaving = true;
                LoadTable();
            }
        }

        private void radioDifference2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDifference2.Checked)
            {
                tuner.CompareSelection = 6;
                showMode = ShowMode.diff2;
            }

            if (radioDifference.Checked || radioDifference2.Checked)
            {
                currentFile = 0;
                dataGridView1.BackgroundColor = Color.Red;
                SetMyText();
                groupDifference.Visible = true;
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = multiplierDecimals;
                }
                LoadTable();
            }
            else
            {
                if (radioMultiplier.Checked)
                {
                    numDecimals.Value = decimals;
                }

                groupDifference.Visible = false;
            }

        }

        private void copyTableFromCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                for (int id = 0; id < compareFiles[0].tableInfos.Count; id++)
                {
                    TableInfo ti = compareFiles[0].tableInfos[id];
                    if (compareTableInfos[currentCmpFile] != null)
                    {
                        TableInfo cmpTi = compareTableInfos[currentCmpFile];
                        for (int cell = 0; cell < ti.tableCells.Count; cell++)
                        {
                            ti.tableCells[cell].SetValue(Convert.ToDouble(cmpTi.tableCells[cell].lastValue));
                        }
                    }
                }
                LoadTable();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void chkRawHex_CheckedChanged(object sender, EventArgs e)
        {
            showRawHEXValuesToolStripMenuItem.Checked = chkRawHex.Checked;
            tuner.ShowAsHex = chkRawHex.Checked;
            LoadTable();
        }

        private void pasteSpecialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSpecial();
        }

        private void PasteSpecial()
        {
            try
            {
                //Show Error if no cell is selected
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please select a cell", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                frmPasteSpecial fps = new frmPasteSpecial();
                if (fps.ShowDialog() != DialogResult.OK)
                    return;

                string cellPosMath = "X";
                string cellNegMath = "X";
                if (fps.radioAdd.Checked)
                {
                    cellPosMath = "X+C";
                    cellNegMath = "X+C";
                }
                else if (fps.radioMultiply.Checked)
                {
                    cellPosMath = "C*X";
                    cellNegMath = "C*X";
                }
                else if (fps.radioPercent.Checked)
                {
                    cellPosMath = "(100+C)/100*X";
                    cellNegMath = "(100+C)/100*X";
                }
                else if (fps.radioTarget.Checked)
                {
                    double target = Convert.ToDouble(fps.txtTarget.Text, System.Globalization.CultureInfo.InvariantCulture);
                    cellPosMath = "C/" + target.ToString()+"*X";
                    cellNegMath = "C/" + target.ToString() + "*X"; 
                }
                else if (fps.radioCustom.Checked)
                {
                    cellPosMath = fps.txtCustomPositive.Text;
                    cellNegMath = fps.txtCustomNegative.Text;
                }

                dataGridView1.BeginEdit(true);

                //Get the starting Cell
                DataGridViewCell startCell = GetStartCell(dataGridView1);
                //Get the clipboard value in a dictionary
                Dictionary<int, Dictionary<int, string>> cbValue =
                        ClipBoardValues(Clipboard.GetText());

                int iRowIndex = startCell.RowIndex;
                foreach (int rowKey in cbValue.Keys)
                {
                    int iColIndex = startCell.ColumnIndex;
                    foreach (int cellKey in cbValue[rowKey].Keys)
                    {
                        //Check if the index is within the limit
                        if (iColIndex <= dataGridView1.Columns.Count - 1
                        && iRowIndex <= dataGridView1.Rows.Count - 1)
                        {
                            DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                            //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                            //if ((chkPasteToSelectedCells.Checked && cell.Selected) || (!chkPasteToSelectedCells.Checked))
                            double cbVal;
                            Debug.WriteLine(cbValue[rowKey][cellKey].ToString(CultureInfo.InvariantCulture));
                            if (Double.TryParse(cbValue[rowKey][cellKey].ToString(CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.CurrentCulture, out cbVal))
                            {
                                //double cbVal = Convert.ToDouble(cbValue[rowKey][cellKey], System.Globalization.CultureInfo.InvariantCulture, out cbVal);
                                string mathTxt = "X";
                                if (cbVal >= 0)
                                    mathTxt = cellPosMath.Replace("X", cell.Value.ToString());
                                else
                                    mathTxt = cellNegMath.Replace("X", cell.Value.ToString());
                                mathTxt = mathTxt.Replace("C", cbVal.ToString());
                                mathTxt = mathTxt.Replace("+-", "-");
                                Debug.WriteLine(mathTxt);
                                cell.Value = parser.Parse(mathTxt);
                            }
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
                dataGridView1.EndEdit();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void showTableVisualizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                uint addr = 0;
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    TableCell tCell = (TableCell)dataGridView1.SelectedCells[0].Tag;
                    addr = tCell.addr;
                }
                StartVisualizer(compareFiles[currentFile].pcm, compareFiles[currentFile].tableInfos[0].td, null, null, addr);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        //[STAThread]
        private void StartVisualizer(PcmFile PCM1, TableData td1, PcmFile PCM2, TableData td2, uint SelectedByte)
        {
            ftvd = new frmTableVisDouble2(PCM1, PCM2,td1,td2);
            ftvd.ShowTables(SelectedByte);
            ftvd.Show();
            //Application.Run(ftvd);
        }

        private void showHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHistogram fh = new frmHistogram(false);
            fh.Show();
            CompareFile selectedFile = compareFiles[currentFile];
            PcmFile PCM = selectedFile.pcm;
            TableData td = selectedFile.tableInfos[0].td;
            fh.SetupTable(PCM, td);

        }

        private void offsetVisualizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (compareFiles.Count == 1)
                {
                    Logger("Please open another file");
                    return;
                }
                else
                {
                    uint addr = 0;
                    if (dataGridView1.SelectedCells.Count > 0)
                    {
                        TableCell tCell = (TableCell)dataGridView1.SelectedCells[0].Tag;
                        addr = tCell.addr;
                    }
                    StartVisualizer(compareFiles[currentFile].pcm, compareFiles[currentFile].tableInfos[0].td, compareFiles[currentCmpFile].pcm, compareFiles[currentCmpFile].tableInfos[0].td, addr);
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void rememberCompareSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rememberCompareSelectionToolStripMenuItem.Checked = !rememberCompareSelectionToolStripMenuItem.Checked;
            AppSettings.TableEditorRememberCompare = rememberCompareSelectionToolStripMenuItem.Checked;
            AppSettings.Save();
        }

        private void Navigate(int position)
        {
            List<TreeParts.Navi> navi = compareFiles[currentFile].pcm.Navigator;
            TableData td = navi[position].Td;
            PcmFile pcm = compareFiles[currentFile].pcm;
            string message = "Navigator: " + (position + 1).ToString() + "/" + navi.Count.ToString();
            NaviTip.Show(message, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 30, 2000);
            CleanUp();
            PrepareTable(pcm, td, null, tuner.currentBin);
            LoadTable();
            compareFiles[currentFile].NaviCurrent = position;
        }

        private void rewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (compareFiles[currentFile].NaviCurrent > 0)
                {
                    compareFiles[currentFile].NaviCurrent--;
                    Navigate(compareFiles[currentFile].NaviCurrent);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void fwdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (compareFiles[currentFile].NaviCurrent < compareFiles[currentFile].pcm.Navigator.Count - 1)
                {
                    compareFiles[currentFile].NaviCurrent++;
                    Navigate(compareFiles[currentFile].NaviCurrent);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }

        private void SetUpDownToolTips()
        {
            try
            {
                if (currentTunerTd > 0)
                {
                    upToolStripMenuItem.ToolTipText = "Previous: " + tunerFilteredTables[currentTunerTd - 1].TableName;
                }
                else
                {
                    upToolStripMenuItem.ToolTipText = null;
                }
                if (currentTunerTd < tunerFilteredTables.Count - 1)
                {
                    downToolStripMenuItem.ToolTipText = "Next: " + tunerFilteredTables[currentTunerTd + 1].TableName;
                }
                else
                {
                    downToolStripMenuItem.ToolTipText = null;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void UpDownTableList(bool down)
        {
            try
            {
                if (compareFiles == null || compareFiles.Count == 0)
                {
                    Debug.WriteLine("No files in comparelist");
                    return;
                }
                CompareFile selectedFile = compareFiles[currentFile];
                TableData td = selectedFile.tableInfos[0].td;
                PcmFile pcm = selectedFile.pcm;
                if (currentTunerTd == -1)
                {
                    LoggerBold("Error in table list");
                    return;
                }
                if (down)
                {
                    if (currentTunerTd < tunerFilteredTables.Count - 1)
                        currentTunerTd++;
                    else
                        return;
                }
                else
                {
                    if (currentTunerTd > 0)
                        currentTunerTd--;
                    else
                        return;
                }
                SaveOnExit();

                td = tunerFilteredTables[currentTunerTd];
                CleanUp();
                ReloadTable(selectedFile, td);
                if (this.Parent == null)
                {
                    tuner.SelectTableFromList(pcm,currentTunerTd);
                }

                SetUpDownToolTips();
                if (down)
                    ShowDownToolTip();
                else
                    ShowUpToolTip();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpDownTableList(true);
        }

        private void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpDownTableList(false);
        }

        private void ShowDownToolTip()
        {
            if (downToolStripMenuItem.ToolTipText != null)
            {
                UpDownTip.Show(downToolStripMenuItem.ToolTipText, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 20, 2000);
            }

        }
        private void DownToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ShowDownToolTip();
        }

        private void ShowUpToolTip()
        {
            if (upToolStripMenuItem.ToolTipText != null)
            {
                UpDownTip.Show(upToolStripMenuItem.ToolTipText, this, System.Windows.Forms.Cursor.Position.X - this.Location.X, System.Windows.Forms.Cursor.Position.Y - this.Location.Y - 20, 2000);
            }
        }
        private void UpToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            ShowUpToolTip();
        }


        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addressToolStripMenuItem.Checked = !addressToolStripMenuItem.Checked;
            AppSettings.TableEditorHexShowAddress = addressToolStripMenuItem.Checked;
            LoadTable();
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            binaryToolStripMenuItem.Checked = !binaryToolStripMenuItem.Checked;
            AppSettings.TableEditorHexShowBinary = binaryToolStripMenuItem.Checked;
            LoadTable();
        }

        private void decimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            decimalToolStripMenuItem.Checked = !decimalToolStripMenuItem.Checked;
            AppSettings.TableEditorHexShowDecimal = decimalToolStripMenuItem.Checked; 
            LoadTable();
        }

        private void smoothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<double> CurrentValues = new List<double>();
                int minR = int.MaxValue;
                int maxR = 0;
                int minC = int.MaxValue;
                int maxC = 0;
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int r = dataGridView1.SelectedCells[i].RowIndex;
                    int c = dataGridView1.SelectedCells[i].ColumnIndex;
                    if (r > maxR) maxR = r;
                    if (r < minR) minR = r;
                    if (c > maxC) maxC = c;
                    if (c < minC) minC = c;
                }
                int rows = maxR - minR + 1;
                int cols = maxC - minC + 1;
                double[,] table = new double[rows, cols];
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int r = dataGridView1.SelectedCells[i].RowIndex;
                    int c = dataGridView1.SelectedCells[i].ColumnIndex;
                    table[r - minR, c - minC] = Convert.ToDouble(dataGridView1.SelectedCells[i].Value);
                }
                double[,] smoothed = Smooth2DTable(table);
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    int r = dataGridView1.SelectedCells[i].RowIndex;
                    int c = dataGridView1.SelectedCells[i].ColumnIndex;
                    dataGridView1.SelectedCells[i].Value = smoothed[r - minR, c - minC];
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private double GetCellValue(DataGridView dgv, int row, int col)
        {
            var val = dgv[col, row].Value;
            if (val == null || !double.TryParse(val.ToString(), out double result))
                throw new Exception($"Invalid numeric value at [{row},{col}]");

            return result;
        }

        private void InterpolateSelectedArea(DataGridView dgv)
        {
            try
            {
                if (dgv.SelectedCells.Count == 0)
                    return;

                int minRow = int.MaxValue, maxRow = int.MinValue;
                int minCol = int.MaxValue, maxCol = int.MinValue;

                foreach (DataGridViewCell cell in dgv.SelectedCells)
                {
                    minRow = Math.Min(minRow, cell.RowIndex);
                    maxRow = Math.Max(maxRow, cell.RowIndex);
                    minCol = Math.Min(minCol, cell.ColumnIndex);
                    maxCol = Math.Max(maxCol, cell.ColumnIndex);
                }

                int rows = maxRow - minRow + 1;
                int cols = maxCol - minCol + 1;

                // ─────────────────────────────────────────────
                // 1×N → horizontal interpolation
                // ─────────────────────────────────────────────
                if (rows == 1 && cols >= 2)
                {
                    double start = GetCellValue(dgv, minRow, minCol);
                    double end = GetCellValue(dgv, minRow, maxCol);

                    for (int c = 0; c < cols; c++)
                    {
                        double t = cols == 1 ? 0 : (double)c / (cols - 1);
                        double value = start + t * (end - start);

                        dgv[minCol + c, minRow].Value = Math.Round(value, 3);
                    }
                    return;
                }

                // ─────────────────────────────────────────────
                // N×1 → vertical interpolation
                // ─────────────────────────────────────────────
                if (cols == 1 && rows >= 2)
                {
                    double start = GetCellValue(dgv, minRow, minCol);
                    double end = GetCellValue(dgv, maxRow, minCol);

                    for (int r = 0; r < rows; r++)
                    {
                        double t = rows == 1 ? 0 : (double)r / (rows - 1);
                        double value = start + t * (end - start);

                        dgv[minCol, minRow + r].Value = Math.Round(value, 3);
                    }
                    return;
                }

                // ─────────────────────────────────────────────
                // NxM → bilinear interpolation
                // ─────────────────────────────────────────────
                if (rows >= 2 && cols >= 2)
                {
                    double tl = GetCellValue(dgv, minRow, minCol);
                    double tr = GetCellValue(dgv, minRow, maxCol);
                    double bl = GetCellValue(dgv, maxRow, minCol);
                    double br = GetCellValue(dgv, maxRow, maxCol);

                    for (int r = 0; r < rows; r++)
                    {
                        double v = (double)r / (rows - 1);

                        for (int c = 0; c < cols; c++)
                        {
                            double u = (double)c / (cols - 1);

                            double value =
                                (1 - u) * (1 - v) * tl +
                                u * (1 - v) * tr +
                                (1 - u) * v * bl +
                                u * v * br;

                            dgv[minCol + c, minRow + r].Value = Math.Round(value, 3);
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
        }

        private void interpolateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InterpolateSelectedArea(dataGridView1);
        }

        private void ShowTdinHexWindow(TableCell tCell)
        {
            hexpanel.SelectionChanged -= Hexpanel_SelectionChanged;

            try
            {
                if (showHEXWindowToolStripMenuItem.Checked && !editingHex)
                {
                    int tableStart = (int)tCell.td.StartAddress();
                    int tableEnd = (int)tCell.td.EndAddress();
                    //int tableEnd = tableStart + tCell.tableInfo.compareFile.buf.Length - 1;
                    hexpanel.SetData(tCell.tableInfo.compareFile.pcm.buf);
                    if (tCell.tableInfo.compareFile.filteredTables.Count > 1)
                    {
                        int min = int.MaxValue;
                        int max = int.MinValue;
                        foreach (TableData td in tCell.tableInfo.compareFile.filteredTables)
                        {
                            if (td.StartAddress() < min) min = (int)td.StartAddress();
                            if (td.EndAddress() > max) max = (int)td.StartAddress();
                        }
                        hexpanel.SetBrackets(min, max);
                    }
                    else
                    {
                        hexpanel.SetBrackets(tableStart, tableEnd);
                    }
                    for (int r=0;r<dataGridView1.Rows.Count;r++)
                    {
                        for (int c=0; c<dataGridView1.Columns.Count;c++)
                        {
                            tCell = (TableCell)dataGridView1.Rows[r].Cells[c].Tag;
                            if (tCell.lastRawValue != tCell.origRawValue)
                            {
                                int location = (int)(tCell.addr - tCell.td.StartAddress() + hexpanel.BracketStart);
                                hexpanel.AddHighlight(location, tCell.td.ElementSize(), System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowModifiedColor),"Modified");
                            }
                        }
                    }
                    hexpanel.AddHighlight(0, tableStart, Color.LightBlue);
                    hexpanel.AddHighlight(tableEnd + 1, tCell.tableInfo.compareFile.pcm.buf.Length - tableEnd, Color.LightBlue);
                    if (!AppSettings.TableEditorAutoResize)
                    {
                        hexpanel.ScrollToBrackets();
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
                LoggerBold("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            hexpanel.SelectionChanged += Hexpanel_SelectionChanged;
        }

        private void ToggleHexview()
        {
            try
            {
                showHEXWindowToolStripMenuItem.Checked = !showHEXWindowToolStripMenuItem.Checked;
                AppSettings.TunerHexWindowShow = showHEXWindowToolStripMenuItem.Checked;
                if (showHEXWindowToolStripMenuItem.Checked)
                {
                    btnToggleHexview.ImageKey = "Collapse.png";
                    splitContainer1.Panel2Collapsed = false;
                    if (!AppSettings.TableEditorAutoResize)
                    {
                        int w = splitContainer1.Width - AppSettings.TunerHexWindowWidth;
                        if (w > 0)
                        {
                            splitContainer1.SplitterDistance = w;
                        }
                    }
                    if (dataGridView1.Rows.Count > 0)
                    {
                        TableCell tCell = (TableCell)dataGridView1.Rows[0].Cells[0].Tag;
                        ShowTdinHexWindow(tCell);
                    }
                }
                else
                {
                    btnToggleHexview.ImageKey = "Expand.png";
                    splitContainer1.Panel2Collapsed = true;
                }
                AutoResize();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void showHEXWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleHexview();
        }

        private void tableSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableSettingsToolStripMenuItem.Checked = true;
            tableValuesToolStripMenuItem.Checked = false;
            offToolStripMenuItem.Checked = false;
            AppSettings.TunerColorsMode = ConditionalColors.Settings;
            LoadTable();
            AppSettings.Save();
        }

        private void tableValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableSettingsToolStripMenuItem.Checked = false;
            tableValuesToolStripMenuItem.Checked = true;
            offToolStripMenuItem.Checked = false;
            AppSettings.TunerColorsMode = ConditionalColors.Values;
            LoadTable();
            AppSettings.Save();
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            offToolStripMenuItem.Checked = true;
            tableSettingsToolStripMenuItem.Checked = false;
            tableValuesToolStripMenuItem.Checked = false;
            AppSettings.TunerColorsMode = ConditionalColors.Off;
            LoadTable();
            AppSettings.Save();
        }

        private void ApplyHexEdit()
        {
            try
            {
                editingHex = true;
                byte[] tableHexData = hexpanel.GetData();
                int elementsize = 1;
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for (int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        bool modified = false;
                        TableCell tCell = (TableCell)dataGridView1.Rows[r].Cells[c].Tag;
                        if (tCell != null)
                        {
                            elementsize = tCell.td.ElementSize();
                            for (int b = 0; b < tCell.lastRawBytes.Length; b++)
                            {
                                if (tCell.lastRawBytes[b] != tableHexData[tCell.addr + b])
                                {
                                    modified = true;
                                }
                            }
                            if (modified)
                            {
                                double newVal = GetRawValue(tableHexData, tCell.addr, tCell.td, 0, tCell.tableInfo.pcm.platformConfig.MSB);
                                tCell.SetValue(newVal, true);
                                SetCellValue(r, c, tCell, null);
                            }
                        }
                    }
                }
                editingHex = false;
                ShowSelectionInHexWindow(elementsize);
                Debug.WriteLine("Hex apply done");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            editingHex = false;

        }
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fdlg = new FontDialog();
            fdlg.Font = hexpanel.TextFont;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                hexpanel.TextFont = fdlg.Font;
                hexpanel.Invalidate();
                Application.DoEvents();
                AutoResize();
                AppSettings.TunerHexWindowFont = SerializableFont.FromFont(fdlg.Font);
                AppSettings.Save();
            }
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();
            clrDialog.Color = hexpanel.BackColor;
            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                //save the colour that the user chose
                hexpanel.BackColor = clrDialog.Color;
                AppSettings.TunerHexWindowBackColor = System.Drawing.ColorTranslator.ToHtml(clrDialog.Color);
                AppSettings.Save();
                hexpanel.Invalidate();
            }
        }

        private void modifiedColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();
            Color oldColor = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowModifiedColor);
            clrDialog.Color = oldColor;
            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                //save the colour that the user chose
                AppSettings.TunerHexWindowModifiedColor = System.Drawing.ColorTranslator.ToHtml(clrDialog.Color);
                hexpanel.ReplaceHighlightColor(oldColor, clrDialog.Color);
                AppSettings.Save();
            }
        }

        private void selectionColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();
            Color oldColor = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowSelectionColor);
            clrDialog.Color = oldColor;
            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                //save the colour that the user chose
                hexpanel.ReplaceHighlightColor(oldColor, clrDialog.Color);
                AppSettings.TunerHexWindowSelectionColor = System.Drawing.ColorTranslator.ToHtml(clrDialog.Color);
                AppSettings.Save();
            }
        }

        private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();
            clrDialog.Color = hexpanel.ColorHex;
            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                //save the colour that the user chose
                hexpanel.ColorHex = clrDialog.Color;
                AppSettings.TunerHexWindowDataColor = System.Drawing.ColorTranslator.ToHtml(clrDialog.Color);
                AppSettings.Save();
                hexpanel.Invalidate();
            }
        }

        private void highlightBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highlightBackgroundToolStripMenuItem.Checked = !highlightBackgroundToolStripMenuItem.Checked;
            AppSettings.TunerHexWindowHighlightBackground = highlightBackgroundToolStripMenuItem.Checked;
            hexpanel.HighlightBackground = highlightBackgroundToolStripMenuItem.Checked;
            hexpanel.Invalidate();
            AppSettings.Save();
        }

        private void showHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHeadersToolStripMenuItem.Checked = !showHeadersToolStripMenuItem.Checked;
            AppSettings.TunerHexWindowHeaders = showHeadersToolStripMenuItem.Checked;
            hexpanel.ShowHeaders = AppSettings.TunerHexWindowHeaders;
            AppSettings.Save();
        }

        private void showOffsetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showOffsetsToolStripMenuItem.Checked = !showOffsetsToolStripMenuItem.Checked;
            AppSettings.TunerHexWindowOffsets = showOffsetsToolStripMenuItem.Checked;
            hexpanel.ShowOffsets = AppSettings.TunerHexWindowOffsets;
            AppSettings.Save();
            AutoResize();
        }

        private void showAsciiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAsciiToolStripMenuItem.Checked = !showAsciiToolStripMenuItem.Checked;
            AppSettings.TunerHexWindowAscii = showAsciiToolStripMenuItem.Checked;
            hexpanel.ShowAscii = showAsciiToolStripMenuItem.Checked;
            AppSettings.Save();
            AutoResize();
        }

        private void resetColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hexpanel.ReplaceHighlightColor(System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowSelectionColor), Color.Red);
            hexpanel.ReplaceHighlightColor(System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowModifiedColor), Color.Yellow);
            AppSettings.TunerHexWindowBackColor = System.Drawing.ColorTranslator.ToHtml(Color.Black);
            AppSettings.TunerHexWindowDataColor = "#056017";
            AppSettings.TunerHexWindowSelectionColor = System.Drawing.ColorTranslator.ToHtml(Color.Red);
            AppSettings.TunerHexWindowModifiedColor = System.Drawing.ColorTranslator.ToHtml(Color.Yellow);
            AppSettings.Save();
            hexpanel.BackColor = Color.Black;
            hexpanel.ColorHex = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowDataColor);
            hexpanel.ColorModified = Color.Yellow;
        }

        private void applyEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyHexEdit();
        }

        private void cancelEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableCell tCell = (TableCell)dataGridView1.Rows[0].Cells[0].Tag;
            ShowTdinHexWindow(tCell);

        }

        private void otherDataColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();
            Color oldColor = System.Drawing.ColorTranslator.FromHtml(AppSettings.TunerHexWindowOtherDataColor);
            clrDialog.Color = oldColor;
            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                hexpanel.ReplaceHighlightColor(oldColor, clrDialog.Color);
                //save the colour that the user chose
                AppSettings.TunerHexWindowOtherDataColor = System.Drawing.ColorTranslator.ToHtml(clrDialog.Color);
                AppSettings.Save();
                hexpanel.Invalidate();
            }

        }

        private void ReloadTable(CompareFile selectedFile, TableData td)
        {
            try
            {
                List<TableData> tds = new List<TableData>();
                tds.Add(td);
                PrepareTable(selectedFile.pcm, td, tds, tuner.currentBin);
                for (int l = 0; l < tuner.LoadedPcms.Count; l++)
                {
                    PcmFile cmpPcm = tuner.LoadedPcms[l];
                    if (cmpPcm.FileName != selectedFile.pcm.FileName)
                    {
                        TableData cmpTd = FindTableData(td, cmpPcm.tableDatas);
                        if (cmpTd != null)
                        {
                            AddCompareFiletoMenu(cmpPcm, tuner.FileLetters[l] + ":" + cmpPcm.FileName, selectedCompareBin);
                            groupSelectCompare.Enabled = true;
                        }
                    }
                }
                LoadTable();
                hexpanel.ScrollToBrackets();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }

        }
        private void numExtraOffset_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                AutoResizeTmpDisabled = true;
                SaveOnExit();
                CompareFile selectedFile = compareFiles[currentFile];
                TableData td = selectedFile.tableInfos[0].td.ShallowCopy(false);
                //TableData td = selectedFile.tableInfos[0].td;
                CleanUp();
                td.extraoffset = (int)numExtraOffset.Value;
                tuner.RefreshGrid();
                ReloadTable(selectedFile, td);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmTableEditor line " + line + ": " + ex.Message);
            }
            AutoResizeTmpDisabled = false;
        }

        private void btnApplyExtraOffset_Click(object sender, EventArgs e)
        {
            CompareFile selectedFile = compareFiles[currentFile];
            TableData td = selectedFile.tableInfos[0].td;
            td.extraoffset = (int)numExtraOffset.Value;
        }

        private void setExtraoffsetToPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pos = hexpanel.SelectedStart;
            if (pos < 0) return;
            CompareFile selectedFile = compareFiles[currentFile];
            TableData td = selectedFile.tableInfos[0].td;
            numExtraOffset.Value = pos - td.StartAddressNoExtra();
        }

        private void scrollToTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hexpanel.ScrollToBrackets();
        }

        private void btnToggleHexview_Click(object sender, EventArgs e)
        {
            ToggleHexview();
        }

    }
}