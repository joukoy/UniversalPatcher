using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;
using System.IO;
using static LoggerUtils;

namespace UniversalPatcher
{
    public partial class frmHistogram : Form
    {
        public frmHistogram()
        {
            InitializeComponent();
        }

        private Dictionary<string, int> dgColumnHeaders;
        private Dictionary<string, int> dgRowHeaders;
        private PcmFile PCM;
        private TableData tData;
        private double[] ColumnHeaders;
        private double[] RowHeaders;
        private Histogram histogram;
        private string FileName;
        private Histogram.HistogramSetup hSetup;

        //For remembering last values:
        private decimal colMin;
        private decimal colMax;
        private decimal colStep;
        private decimal rowMin;
        private decimal rowMax;
        private decimal rowStep;
        private bool liveData = false;

        private void frmHistogram_Load(object sender, EventArgs e)
        {
            tabHistogram.Enter += TabHistogram_Enter;
            this.FormClosing += FrmHistogram_FormClosing;
        }

        private void FrmHistogram_FormClosing(object sender, FormClosingEventArgs e)
        {
            try 
            {
                LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
            }
            catch { }
        }

        private void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TabHistogram_Enter(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }

        public void AddTunerToTab()
        {
            if (PCM == null)
            {
                PCM = new PcmFile();
            }
            FrmTuner frmT = new FrmTuner(PCM);
            frmT.Dock = DockStyle.Fill;
            frmT.FormBorderStyle = FormBorderStyle.None;
            frmT.TopLevel = false;
            tabSelectTable.Controls.Add(frmT);
            frmT.Histogram = this;
            frmT.Show();
        }

        private int GetColumnByHeader(string hdrTxt)
        {
            int ind = int.MinValue;
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
            int ind = int.MinValue;
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

        public void SetupTable(PcmFile pcm, TableData tData)
        {
            this.PCM = pcm;
            this.tData = tData;

            try
            {
                dataGridView1.Columns.Clear();
                this.Text = "Histogram: " + tData.TableName;
                Logger("Loading table: " + tData.TableName + " as template");
                dgColumnHeaders = new Dictionary<string, int>();
                dgRowHeaders = new Dictionary<string, int>();
                numDecimals.Value = tData.Decimals;

                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
                dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

                string[] cHeaders;
                if (tData.ColumnHeaders.ToLower().StartsWith("table:") || tData.ColumnHeaders.ToLower().StartsWith("guid:"))
                {
                    TableData headerTd = pcm.GetTdbyHeader(tData.ColumnHeaders);
                    cHeaders = LoadHeaderFromTable(headerTd, tData.Columns, pcm);
                    ColumnHeaders = LoadHeaderValuesFromTable(headerTd, tData.Columns, pcm);
                }
                else
                {
                    cHeaders = tData.ColumnHeaders.Split(',');
                    ColumnHeaders = new double[cHeaders.Length];
                    for (int c=0; c < cHeaders.Length;c++)
                    {
                        string hNr = Regex.Replace(cHeaders[c], "[^0-9]", "");
                        if (Double.TryParse(hNr.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double h))
                            ColumnHeaders[c] = h;
                        else
                            ColumnHeaders[c] = c;
                    }
                }

                for (int c = 0; c < tData.Columns; c++)
                {
                    string hdrTxt = c.ToString();
                    if (cHeaders != null && cHeaders.Length > c)
                    {
                        hdrTxt = cHeaders[c];
                    }
                    int ind = dataGridView1.Columns.Add(hdrTxt, hdrTxt);
                    dgColumnHeaders.Add(hdrTxt, ind);
                }

                string[] rHeaders;
                if (tData.RowHeaders.ToLower().StartsWith("table:") || tData.RowHeaders.ToLower().StartsWith("guid:"))
                {
                    TableData headerTd = pcm.GetTdbyHeader(tData.RowHeaders);
                    rHeaders = LoadHeaderFromTable(headerTd, tData.Rows, pcm);
                    RowHeaders = LoadHeaderValuesFromTable(headerTd, tData.Rows, pcm);
                }
                else
                {
                    rHeaders = tData.RowHeaders.Split(',');
                    RowHeaders = new double[rHeaders.Length];
                    for (int r = 0; r < rHeaders.Length; r++)
                    {
                        string hNr = Regex.Replace(rHeaders[r], "[^0-9,.]", "");
                        RowHeaders[r] = Convert.ToDouble(hNr.Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    }

                }

                for (int r = 0; r < tData.Rows; r++)
                {
                    string hdrTxt = r.ToString();
                    if (rHeaders != null && rHeaders.Length > r)
                    {
                        hdrTxt = rHeaders[r];
                    }
                    int ind = dataGridView1.Rows.Add();
                    dataGridView1.Rows[ind].HeaderCell.Value = hdrTxt;
                    dgRowHeaders.Add(hdrTxt, ind);
                }

                StringBuilder colBuilder = new StringBuilder();
                for (int c=0; c<ColumnHeaders.Length; c++)
                {
                    colBuilder.Append(ColumnHeaders[c].ToString() + ",");
                }
                txtColHeaders.Text = colBuilder.ToString().Trim(',');

                StringBuilder rowBuilder = new StringBuilder();
                for (int r=0;r < RowHeaders.Length; r++)
                {
                    rowBuilder.Append(RowHeaders[r].ToString() + ",");
                }
                txtRowHeaders.Text = rowBuilder.ToString().Trim(',');

                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                AutoResize();
                labelSelectTable.Visible = false;
                radioTemplateUseTable.Checked = true;
                Logger("Select parameters in Settings-tab");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHistogram, line " + line + ": " + ex.Message);
            }

        }

        private void btnBrowseCsv_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select log file", CsvFilter);
            if (fName.Length == 0)
                return;
            txtLogFile.Text = fName;

        }

        private void LoadHistogram()
        {
            try
            {
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                for (int r=0;r<dataGridView1.Rows.Count;r++)
                {
                    for (int c=0; c < dataGridView1.Columns.Count;c++)
                    {
                        dataGridView1.Rows[r].Cells[c].Value = null;
                    }
                }
                if (!string.IsNullOrEmpty(txtLogFile.Text))
                {
                    liveData = false;
                    histogram.ParseCsvFile(txtLogFile.Text);
                }
                else
                {
                    return;
                }
                    //histogram.LogDatas = new List<Histogram.CsvData>();
                histogram.CountHits(comboXparam.Text, comboYparam.Text, comboValueparam.Text, comboSkipParam.Text, (double)numSkipValue.Value, (ushort)numDecimals.Value);
                GetHistogramSettings();
                for (int i = 0; i < histogram.HitDatas.Count; i++)
                {
                    Histogram.HitData hd = histogram.HitDatas[i];
                    if (hd.Values.Count > 0)
                    {
                        double cellValue = hd.Average;
                        dataGridView1.Rows[hd.Row].Cells[hd.Column].Value = cellValue;
                        if (cellValue > hSetup.HighValue)
                            dataGridView1.Rows[hd.Row].Cells[hd.Column].Style.BackColor = Color.FromArgb(hSetup.HighColor);
                        else if (cellValue < hSetup.LowValue)
                            dataGridView1.Rows[hd.Row].Cells[hd.Column].Style.BackColor = Color.FromArgb(hSetup.LowColor);
                        else
                            dataGridView1.Rows[hd.Row].Cells[hd.Column].Style.BackColor = Color.FromArgb(hSetup.MidColor);
                        string tipTxt = "Min: " + hd.Values.Min().ToString() +", Max: " + hd.Values.Max().ToString() + ", Hits: " + hd.Values.Count.ToString();
                        dataGridView1.Rows[hd.Row].Cells[hd.Column].ToolTipText = tipTxt;
                        dataGridView1.Rows[hd.Row].Cells[hd.Column].Tag = hd;
                    }
                }
                dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHistogram line " + line + ": " + ex.Message);
            }


        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            ShowCellInfo();
        }

        private void btnReadCsv_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboXparam.Text) || string.IsNullOrEmpty(comboValueparam.Text))
            {
                LoggerBold("Select histogram parameters first!");
                return;
            }
            LoadHistogram();
            tabHistogram.Select();
        }

        private void SetupParameterLists()
        {
            comboXparam.Items.Clear();
            comboYparam.Items.Clear();
            comboValueparam.Items.Clear();
            comboSkipParam.Items.Clear();
            for (int i = 0; i < histogram.Parameters.Length; i++)
            {
                string par = histogram.Parameters[i];
                comboXparam.Items.Add(par);
                comboYparam.Items.Add(par);
                comboValueparam.Items.Add(par);
                comboSkipParam.Items.Add(par);
            }
        }

        private void GetParameters()
        {
            try
            {
                histogram = new Histogram(ColumnHeaders,RowHeaders);
                StreamReader sr = new StreamReader(txtLogFile.Text);
                string line;
                line = sr.ReadLine();
                histogram.ParseCsvHeader(line, txtColumnSeparator.Text);
                sr.Close();
                SetupParameterLists();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        public void SetupLiveParameters()
        {
            liveData = true;
            histogram = new Histogram(ColumnHeaders, RowHeaders);
            List<string> pids = new List<string>();
            foreach (PidConfig p in datalogger.PidProfile)
            {
                pids.Add(p.PidName);
            }
            histogram.Parameters = pids.ToArray();
            SetupParameterLists();
            GetHistogramSettings();
        }

        private void txtCsvFile_TextChanged(object sender, EventArgs e)
        {
            GetParameters();
        }

        private void AutoResize()
        {
            try
            {
                int dgv_width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + dataGridView1.RowHeadersWidth;
                if (dgv_width < 550) dgv_width = 550;
                int dgv_height = dataGridView1.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridView1.ColumnHeadersHeight;
                Screen myScreen = Screen.FromPoint(MousePosition);
                System.Drawing.Rectangle area = myScreen.WorkingArea;
                if ((dgv_width + 150) > area.Width)
                    this.Width = area.Width - 50;
                else
                    this.Width = dgv_width + 50; //150
                if ((dgv_height + 120) > area.Height)
                    this.Height = area.Height - 50;
                else
                    this.Height = dgv_height + 120; 
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHistogram line " + line + ": " + ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Histogram", "settings.xml");
                if (tData != null && !string.IsNullOrEmpty(tData.TableName))
                {
                    defName = Path.Combine(Application.StartupPath, "Histogram", tData.TableName + ".xml");
                }
                else
                {
                    tData = new TableData();
                    tData.Decimals = (ushort)numDecimals.Value;
                }
                string fName = SelectFile("Select Historgram setup", XmlFilter,defName);
                if (fName.Length == 0)
                    return;
                FileName = fName;
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Histogram.HistogramSetup));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                hSetup = (Histogram.HistogramSetup)reader.Deserialize(file);
                file.Close();

                comboXparam.Text = hSetup.XParameter;
                comboYparam.Text = hSetup.YParameter;
                comboValueparam.Text = hSetup.ValueParameter;
                comboSkipParam.Text = hSetup.SkipParameter;
                numSkipValue.Value = (decimal)hSetup.SkipValue;
                txtColumnSeparator.Text = hSetup.CsvSeparator;
                btnColorHigh.BackColor = Color.FromArgb(hSetup.HighColor);
                btnColorMid.BackColor = Color.FromArgb(hSetup.MidColor);
                btnColorLow.BackColor = Color.FromArgb(hSetup.LowColor);
                numRangeMax.Value = (decimal)hSetup.HighValue;
                numRangeMin.Value = (decimal)hSetup.LowValue;
                txtColHeaders.Text = hSetup.ColumnHeaders;
                txtRowHeaders.Text = hSetup.RowHeaders;
                radioTemplateManual.Checked = hSetup.ManualHeaders;
                numDecimals.Value = hSetup.Decimals;

                if (!string.IsNullOrEmpty(txtColHeaders.Text) && !string.IsNullOrEmpty(txtRowHeaders.Text))
                {
                    SetupManualTemplate();
                }

                if (!string.IsNullOrEmpty(hSetup.LogFile) && txtLogFile.Text.Length == 0)
                {
                    txtLogFile.Text = hSetup.LogFile;
                }
                if (txtLogFile.Text.Length > 0)
                {
                    LoadHistogram();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHistogram line " + line + ": " + ex.Message);
            }
        }

        private void GetHistogramSettings()
        {
            hSetup = new Histogram.HistogramSetup();
            hSetup.XParameter = comboXparam.Text;
            hSetup.YParameter = comboYparam.Text;
            hSetup.ValueParameter = comboValueparam.Text;
            hSetup.SkipParameter = comboSkipParam.Text;
            hSetup.SkipValue = (double)numSkipValue.Value;
            hSetup.CsvSeparator = txtColumnSeparator.Text;
            hSetup.LogFile = txtLogFile.Text;
            hSetup.HighColor = btnColorHigh.BackColor.ToArgb();
            hSetup.MidColor = btnColorMid.BackColor.ToArgb();
            hSetup.LowColor = btnColorLow.BackColor.ToArgb();
            hSetup.HighValue = (double)numRangeMax.Value;
            hSetup.LowValue = (double)numRangeMin.Value;
            hSetup.ColumnHeaders = txtColHeaders.Text;
            hSetup.RowHeaders = txtRowHeaders.Text;
            hSetup.ManualHeaders = radioTemplateManual.Checked;
            hSetup.Decimals = (ushort)numDecimals.Value;
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = "";
                if (!string.IsNullOrEmpty(FileName))
                    defName = FileName;
                else
                    defName = Path.Combine(Application.StartupPath, "Histogram", tData.TableName + ".xml");
                string fName = SelectSaveFile(XmlFilter, defName);
                if (fName.Length == 0)
                    return;

                FileName = fName;
                GetHistogramSettings();
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Histogram.HistogramSetup));
                    writer.Serialize(stream, hSetup);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHistogram line " + line + ": " + ex.Message);
            }
        }

        private void btnColorHigh_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnColorHigh.BackColor = colorDialog.Color;
                hSetup.HighColor = colorDialog.Color.ToArgb();
            }
            colorDialog.Dispose();
        }

        private void btnColorMid_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnColorMid.BackColor = colorDialog.Color;
                hSetup.MidColor = colorDialog.Color.ToArgb();
            }
            colorDialog.Dispose();
        }

        private void btnColorLow_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                btnColorLow.BackColor = colorDialog.Color;
                hSetup.LowColor = colorDialog.Color.ToArgb();
            }
            colorDialog.Dispose();
        }

        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void AddLogData(double[] logdata)
        {
            try
            {
                if (hSetup == null)
                {
                    if (string.IsNullOrEmpty(comboXparam.Text) || string.IsNullOrEmpty(comboValueparam.Text))
                    {
                        LoggerBold("Select histogram parameters first!");
                        return;
                    }
                    LoadHistogram();
                }
                histogram.AddData(logdata);
                histogram.CountHits(comboXparam.Text, comboYparam.Text, comboValueparam.Text, comboSkipParam.Text, (double)numSkipValue.Value, (ushort)numDecimals.Value);
                for (int i = 0; i < histogram.HitDatas.Count; i++)
                {
                    Histogram.HitData hd = histogram.HitDatas[i];
                    if (hd.Values.Count > 0)
                    {
                        double cellValue = hd.Average;
                        dataGridView1.Rows[hd.Row].Cells[hd.Column].Value = cellValue;
                        if (cellValue > hSetup.HighValue)
                            dataGridView1.Rows[hd.Row].Cells[hd.Column].Style.BackColor = Color.FromArgb(hSetup.HighColor);
                        else if (cellValue < hSetup.LowValue)
                            dataGridView1.Rows[hd.Row].Cells[hd.Column].Style.BackColor = Color.FromArgb(hSetup.LowColor);
                        else
                            dataGridView1.Rows[hd.Row].Cells[hd.Column].Style.BackColor = Color.FromArgb(hSetup.MidColor);
                        string tipTxt = "Min: " + hd.Values.Min().ToString() + ", Max: " + hd.Values.Max().ToString() + ", Hits: " + hd.Values.Count.ToString();
                        dataGridView1.Rows[hd.Row].Cells[hd.Column].ToolTipText = tipTxt;
                        dataGridView1.Rows[hd.Row].Cells[hd.Column].Tag = hd;
                    }
                }
                ShowCellInfo();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmHistogram line " + line + ": " + ex.Message);
            }
        }

        private void ShowCellInfo()
        {
            if (dataGridView1.SelectedCells.Count > 0 && dataGridView1.SelectedCells[0].Tag != null)
            {
                Histogram.HitData hd = (Histogram.HitData)dataGridView1.SelectedCells[0].Tag;
                labelCellinfo.Text = "Min: " + hd.Values.Min().ToString() + ", Max: " + hd.Values.Max().ToString() + ", Hits: " + hd.Values.Count.ToString();
            }
            else
            {
                labelCellinfo.Text = "";
            }
        }

        private void chkGetLiveData_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGetLiveData.Checked)
            {
                GetHistogramSettings();
                LoggerDataEvents.LogDataAdded += LogEvents_LogDataAdded;
            }
            else
            {
                LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
            }
        }

        private void LogEvents_LogDataAdded(object sender, DataLogger.LogDataEvents.LogDataEvent e)
        {
            try
            {
                //double[] lastDoubleValues = datalogger.slothandler.CalculatePidDoubleValues(e.Data.Values);
                this.Invoke((MethodInvoker)delegate () //Event handler, can't directly handle UI
                {
                    AddLogData(e.Data.CalculatedValues);
                });
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmHistogram line " + line + ": " + ex.Message);
            }
        }

        private void SetupManualTemplate()
        {
            try
            {
                string[] cols = txtColHeaders.Text.Split(',');
                string[] rows = txtRowHeaders.Text.Split(',');

                if (tData == null)
                {
                    this.tData = new TableData();
                    tData.Decimals = (ushort)numDecimals.Value;
                }
                dgColumnHeaders = new Dictionary<string, int>();
                dgRowHeaders = new Dictionary<string, int>();
                ColumnHeaders = new double[cols.Length];
                RowHeaders = new double[rows.Length];

                dataGridView1.Columns.Clear();
                for (int c = 0; c < cols.Length; c++)
                {
                    string col = cols[c];
                    dataGridView1.Columns.Add(col, col);
                    dgColumnHeaders.Add(col, c);
                    ColumnHeaders[c] = Convert.ToDouble(col, System.Globalization.CultureInfo.InvariantCulture);
                }

                for (int r=0;r < rows.Length; r++)
                {
                    string row = rows[r];
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[r].HeaderCell.Value = row;
                    dgRowHeaders.Add(row, r);
                    RowHeaders[r] = Convert.ToDouble(row, System.Globalization.CultureInfo.InvariantCulture);
                }
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                AutoResize();
                labelSelectTable.Visible = false;
                if (liveData)
                    SetupLiveParameters();
                else
                    LoadHistogram();
                //GetParameters();

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, frmHistogram line " + line + ": " + ex.Message);
            }

        }

        private void btnApplyTemplate_Click(object sender, EventArgs e)
        {
            SetupManualTemplate();
        }

        private void radioTemplateManual_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTemplateManual.Checked)
                btnApplyTemplate.Enabled = true;
            else
                btnApplyTemplate.Enabled = false;
        }

        private void btnGenColHeaders_Click(object sender, EventArgs e)
        {
            frmTableHeaders fth = new frmTableHeaders();
            fth.numMin.Value = colMin;
            fth.numMax.Value = colMax;
            fth.numStep.Value = colStep;
            if (fth.ShowDialog() == DialogResult.OK)
            {
                radioTemplateManual.Checked = true;
                txtColHeaders.Text = fth.HeaderStr;
                colMin = fth.numMin.Value;
                colMax = fth.numMax.Value;
                colStep = fth.numStep.Value;
            }
            fth.Dispose();
        }

        private void btnGenRowHeaders_Click(object sender, EventArgs e)
        {
            frmTableHeaders fth = new frmTableHeaders();
            fth.numMin.Value = rowMin;
            fth.numMax.Value = rowMax;
            fth.numStep.Value = rowStep;
            if (fth.ShowDialog() == DialogResult.OK)
            {
                radioTemplateManual.Checked = true;
                txtRowHeaders.Text = fth.HeaderStr;
                rowMin = fth.numMin.Value;
                rowMax = fth.numMax.Value;
                rowStep = fth.numStep.Value;
            }
            fth.Dispose();
        }
    }
}
