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
using System.Threading.Tasks;
using System.Threading;

namespace UniversalPatcher
{
    public partial class frmHistogram : Form
    {
        public frmHistogram(bool live)
        {
            this.liveData = live;
            InitializeComponent();
        }
        public frmHistogram(bool live, PcmFile PCM)
        {
            this.liveData = live;
            this.PCM = PCM;
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
        private int lastDataCount = 0;
        private int dataCount = 0;
        //private DateTime logUpdateTime = DateTime.MinValue;
        private List<int> updatedHitDatas = new List<int>();
        private List<Histogram.HistSettings> hSets;
        private void frmHistogram_Load(object sender, EventArgs e)
        {
            tabTemplateBin.Enter += TabTable_Enter;
            tabSavedSettings.Enter += TabSavedSettings_Enter;
            this.FormClosing += FrmHistogram_FormClosing;
            string fName;
            if (liveData)
            {
                fName = Path.Combine(Application.StartupPath, "Histogram", "autosave-live.xml");
                foreach(string f in AppSettings.HistogramLastLiveProfiles)
                {
                    AddToRecentMenu(f);
                }
            }
            else
            {
                chkGetLiveData.Enabled = false;
                fName = Path.Combine(Application.StartupPath, "Histogram", "autosave-log.xml");
                foreach (string f in AppSettings.HistogramLastLogfileProfiles)
                {
                    AddToRecentMenu(f);
                }
            }
            if (!string.IsNullOrEmpty(fName) && File.Exists(fName))
            {
                LoadProfile(fName);
            }
            FileName = null;

            
        }

        private void TabSavedSettings_Enter(object sender, EventArgs e)
        {
            LoadSettingFiles();
        }

        private void FrmHistogram_FormClosing(object sender, FormClosingEventArgs e)
        {
            try 
            {
                LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
                if (liveData)
                    FileName = Path.Combine(Application.StartupPath, "Histogram", "autosave-live.xml");
                else
                    FileName = Path.Combine(Application.StartupPath, "Histogram", "autosave-log.xml");
                Histogram.HistogramSetup hSet = GetHistogramSettings();
                SaveProfile(FileName);
            }
            catch { }
        }
        private void TabTable_Enter(object sender, EventArgs e)
        {
            AddTunerToTab();
        }


        private void AddToRecentMenu(string fName)
        {
            ToolStripMenuItem mi = new ToolStripMenuItem(fName);
            mi.Click += Mi_Click;
            mi.Text = fName;
            loadRecentToolStripMenuItem.DropDownItems.Add(mi);
        }

        private void Mi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            LoadProfile(mi.Text);
        }

        private void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            throw new NotImplementedException();
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
        public void AddTunerToTab()
        {
            if (tabTemplateBin.Controls.Count > 0)
            {
                return;
            }
            if (PCM == null)
            {
                PCM = new PcmFile();
            }
            FrmTuner frmT = new FrmTuner(PCM, this);
            frmT.Dock = DockStyle.Fill;
            frmT.FormBorderStyle = FormBorderStyle.None;
            frmT.TopLevel = false;
            tabTemplateBin.Controls.Add(frmT);
            frmT.Show();
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
                if (pcm != null && tData.ColumnHeaders.ToLower().StartsWith("table:") || tData.ColumnHeaders.ToLower().StartsWith("guid:"))
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
                if (pcm != null && tData.RowHeaders.ToLower().StartsWith("table:") || tData.RowHeaders.ToLower().StartsWith("guid:"))
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
                if (liveData)
                    SetupLiveParameters();
                else
                    LoadHistogram();

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
                ClearDisplay();
                if (liveData || string.IsNullOrEmpty(txtLogFile.Text))
                {
                    return;
                }
                histogram.ParseCsvFile(txtLogFile.Text);
                //histogram.LogDatas = new List<Histogram.CsvData>();
                histogram.CountHits(comboXparam.Text, comboYparam.Text, comboValueparam.Text, comboSkipParam.Text, (double)numSkipValue.Value, (ushort)numDecimals.Value);
                hSetup = GetHistogramSettings();
                dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
                if (histogram.HitDatas == null)
                { 
                    return; 
                }
                for (int i = 0; i < histogram.HitDatas.Count; i++)
                {
                    Histogram.HitData hd = histogram.HitDatas[i];
                    if (hd.Values.Count > 0)
                    {
                        DisplayHitData(hd);
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
            if (string.IsNullOrEmpty(comboXparam.Text) || string.IsNullOrEmpty(comboValueparam.Text) || string.IsNullOrEmpty(txtColHeaders.Text) || string.IsNullOrEmpty(txtRowHeaders.Text))
            {
                LoggerBold("Select histogram parameters first!");
                return;
            }
            if (liveData)
                SetupLiveParameters();
            else
                LoadHistogram();
            //tabHistogram.Select();
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
            CheckComboSelections();
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

        private void ClearDisplay()
        {
            dataGridView1.SelectionChanged -= DataGridView1_SelectionChanged;
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                for (int c = 0; c < dataGridView1.Columns.Count; c++)
                {
                    dataGridView1.Rows[r].Cells[c].Value = null;
                    dataGridView1.Rows[r].Cells[c].Style.BackColor = Color.FromArgb(hSetup.MidColor);
                }
            }
            string formatStr = "0";
            for (int f = 1; f <= (int)numDecimals.Value; f++)
            {
                if (f == 1) formatStr += ".";
                formatStr += "0";
            }
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.DefaultCellStyle.Format = formatStr;
            }
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

        }
        public void SetupLiveParameters()
        {
            liveData = true;
            groupLogfile.Enabled = false;
            histogram = new Histogram(ColumnHeaders, RowHeaders);
            List<string> pids = new List<string>();
            foreach (LogParam.PidSettings p in datalogger.SelectedPids)
            {
                LogParam.PidParameter parm = p.Parameter;
                pids.Add(parm.Name);
            }
            histogram.Parameters = pids.ToArray();
            SetupParameterLists();
            hSetup= GetHistogramSettings();
            ClearDisplay();
        }

        private void txtCsvFile_TextChanged(object sender, EventArgs e)
        {
            GetParameters();
        }

        private void AutoResize()
        {
            return;
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

        private void CheckComboSelections()
        {
            if (histogram == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(comboXparam.Text) && !histogram.Parameters.Contains(comboXparam.Text))
            {
                comboXparam.Text = "";
            }
            if (!string.IsNullOrEmpty(comboYparam.Text) && !histogram.Parameters.Contains(comboYparam.Text))
            {
                comboYparam.Text = "";
            }
            if (!string.IsNullOrEmpty(comboValueparam.Text) && !histogram.Parameters.Contains(comboValueparam.Text))
            {
                comboValueparam.Text = "";
            }
            if (!string.IsNullOrEmpty(comboSkipParam.Text) && !histogram.Parameters.Contains(comboSkipParam.Text))
            {
                comboSkipParam.Text = "";
            }
            if (string.IsNullOrEmpty(comboXparam.Text) || string.IsNullOrEmpty(comboYparam.Text) || string.IsNullOrEmpty(comboValueparam.Text))
            {
                chkGetLiveData.Checked = false;
                chkGetLiveData.Enabled = false;
            }
            else if (liveData)
            {
                chkGetLiveData.Enabled = true;
            }
        }
        private void LoadProfile(string fName)
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
                if (string.IsNullOrEmpty(fName))
                {
                    fName = SelectFile("Select Historgram setup", HistogramFilter, defName);
                    if (fName.Length == 0)
                        return;
                }
                FileName = fName;
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Histogram.HistogramSetup));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                hSetup = (Histogram.HistogramSetup)reader.Deserialize(file);
                file.Close();
                AddFileToRecentList(fName);

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
                numDecimals.Value = hSetup.Decimals;

                if (!string.IsNullOrEmpty(txtColHeaders.Text) && !string.IsNullOrEmpty(txtRowHeaders.Text))
                {
                    SetupManualTemplate();
                }

                if (!liveData && !string.IsNullOrEmpty(hSetup.LogFile) && txtLogFile.Text.Length == 0)
                {
                    txtLogFile.Text = hSetup.LogFile;
                }
                if (txtLogFile.Text.Length > 0)
                {
                    LoadHistogram();
                }
                CheckComboSelections();
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
        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadProfile("");
        }

        private Histogram.HistogramSetup GetHistogramSettings()
        {
            Histogram.HistogramSetup newSetup = new Histogram.HistogramSetup();
            newSetup.XParameter = comboXparam.Text;
            newSetup.YParameter = comboYparam.Text;
            newSetup.ValueParameter = comboValueparam.Text;
            newSetup.SkipParameter = comboSkipParam.Text;
            newSetup.SkipValue = (double)numSkipValue.Value;
            newSetup.CsvSeparator = txtColumnSeparator.Text;
            newSetup.LogFile = txtLogFile.Text;
            newSetup.HighColor = btnColorHigh.BackColor.ToArgb();
            newSetup.MidColor = btnColorMid.BackColor.ToArgb();
            newSetup.LowColor = btnColorLow.BackColor.ToArgb();
            newSetup.HighValue = (double)numRangeMax.Value;
            newSetup.LowValue = (double)numRangeMin.Value;
            newSetup.ColumnHeaders = txtColHeaders.Text;
            newSetup.RowHeaders = txtRowHeaders.Text;
            newSetup.Decimals = (ushort)numDecimals.Value;
            return newSetup;
        }

        private void AddFileToRecentList(string fName)
        {
            if (Path.GetFileName(fName).StartsWith("autosave-"))
            {
                return;
            }
            if (liveData)
            {
                if (AppSettings.HistogramLastLiveProfiles.Contains(fName))
                {
                    return;
                }
                if (AppSettings.HistogramLastLiveProfiles.Count > 10)
                {
                    AppSettings.HistogramLastLiveProfiles.RemoveAt(0);
                }
                AppSettings.HistogramLastLiveProfiles.Add(fName);
            }
            else
            {
                if (AppSettings.HistogramLastLogfileProfiles.Contains(fName))
                {
                    return;
                }
                if (AppSettings.HistogramLastLogfileProfiles.Count > 10)
                {
                    AppSettings.HistogramLastLogfileProfiles.RemoveAt(0);
                }
                AppSettings.HistogramLastLogfileProfiles.Add(fName);
            }
            AppSettings.Save();
            AddToRecentMenu(fName);
        }
        private void SaveProfile(string fName)
        {
            try
            {
                if (string.IsNullOrEmpty(fName))
                {
                    string defName = "";
                    if (!string.IsNullOrEmpty(FileName))
                        defName = FileName;
                    else
                        defName = Path.Combine(Application.StartupPath, "Histogram", tData.TableName + ".xml");
                    fName = SelectSaveFile(HistogramFilter, defName);
                    if (fName.Length == 0)
                        return;
                }
                FileName = fName;
                hSetup = GetHistogramSettings();
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Histogram.HistogramSetup));
                    writer.Serialize(stream, hSetup);
                    stream.Close();
                }
                AppSettings.Save();
                AddFileToRecentList(fName);
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
        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProfile("");
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

        private void UpdateLiveData()
        {
            try
            {
                if (updatedHitDatas.Count > 0)
                {
                    if (hSetup == null)
                    {
                        if (string.IsNullOrEmpty(comboXparam.Text) || string.IsNullOrEmpty(comboValueparam.Text))
                        {
                            //LoggerBold("Select histogram parameters first!");
                            return;
                        }
                        LoadHistogram();
                    }
                    List<int> tmpHits = updatedHitDatas;
                    updatedHitDatas = new List<int>();
                    //tmpHits.AddRange(updatedHitDatas);
                    //updatedHitDatas.Clear();

                    for (int i = 0; i < tmpHits.Count; i++)
                    {
                        int idx = tmpHits[i];
                        Histogram.HitData hd = histogram.HitDatas[idx];
                        DisplayHitData(hd);
                    }
                    dataGridView1.Refresh();
                    ShowCellInfo();
                    lastDataCount = histogram.LogDataCount;
                }
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
                hSetup = GetHistogramSettings();
                LoggerDataEvents.LogDataAdded += LogEvents_LogDataAdded;
            }
            else
            {
                LoggerDataEvents.LogDataAdded -= LogEvents_LogDataAdded;
            }
        }

        private void DisplayHitData(Histogram.HitData hd)
        {
            double cellValue = hd.CurrentAverage;
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
            //dataGridView1.Refresh();
            ShowCellInfo();

        }

        private void LogEvents_LogDataAdded(object sender, DataLogger.LogDataEvents.LogDataEvent e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate () //Event handler, can't directly handle UI
                {
                    int idx = histogram.CountHitsIncrement(e.Data.CalculatedValues, comboXparam.Text, comboYparam.Text, comboValueparam.Text, comboSkipParam.Text, (double)numSkipValue.Value, (ushort)numDecimals.Value);
                    if (idx >= 0 && !updatedHitDatas.Contains(idx))
                    {
                        updatedHitDatas.Add(idx);
                    }
                });
                dataCount++;
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

        private void btnGenColHeaders_Click(object sender, EventArgs e)
        {
            frmTableHeaders fth = new frmTableHeaders();
            fth.numMin.Value = colMin;
            fth.numMax.Value = colMax;
            fth.numStep.Value = colStep;
            if (fth.ShowDialog() == DialogResult.OK)
            {
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
                txtRowHeaders.Text = fth.HeaderStr;
                rowMin = fth.numMin.Value;
                rowMax = fth.numMax.Value;
                rowStep = fth.numStep.Value;
            }
            fth.Dispose();
        }

        private void comboXparam_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timerLiveData_Tick(object sender, EventArgs e)
        {
            UpdateLiveData();
        }

        private void btnLoadTable_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabTemplateBin;
/*            frmTableSelector fts = new frmTableSelector();
            if (fts.ShowDialog() == DialogResult.OK)
            {
                SetupTable(fts.PCM,fts.selectedTd);
            }
*/
        }

        private void btnRefreshParams_Click(object sender, EventArgs e)
        {
            if (liveData)
            {
                SetupLiveParameters();
            }
            else
            {
                GetParameters();
            }
        }

        private void loadRecentToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void chkShowSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowSettings.Checked)
            {
                splitContainer1.Panel2Collapsed = false;
                splitContainer1.Panel2.Show();
            }
            else
            {
                splitContainer1.Panel2Collapsed = true;
                splitContainer1.Panel2.Hide();
            }
        }

        private void btnSaveCurrent_Click(object sender, EventArgs e)
        {
            SaveProfile("");
            LoadSettingFiles();
        }
        private void LoadSettingFiles()
        {
            hSets = new List<Histogram.HistSettings>();
            DirectoryInfo d = new DirectoryInfo(Path.Combine(Application.StartupPath,"Histogram"));
            FileInfo[] files = d.GetFiles(Path.GetFileName("*.xml"));
            foreach (FileInfo f in files)
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Histogram.HistogramSetup));
                System.IO.StreamReader file = new System.IO.StreamReader(f.FullName);
                Histogram.HistogramSetup hS = (Histogram.HistogramSetup)reader.Deserialize(file);
                hSets.Add(new Histogram.HistSettings(f.Name, hS));
                file.Close();
            }
            dataGridViewSavedSettings.DataSource = null;
            dataGridViewSavedSettings.DataSource = hSets;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            int row = dataGridViewSavedSettings.SelectedCells[0].RowIndex;
            Histogram.HistSettings hss = (Histogram.HistSettings)dataGridViewSavedSettings.Rows[row].DataBoundItem;
            string fName = Path.Combine(Application.StartupPath, "Histogram", hss.Name);
            LoadProfile(fName);
        }

        private void txtColumnSeparator_TextChanged(object sender, EventArgs e)
        {
            GetParameters();
        }
    }
}
