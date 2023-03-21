using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static LoggerUtils;
using static Helpers;
using J2534DotNet;
using System.Diagnostics;
using System.IO;

namespace UniversalPatcher
{
    public partial class frmJ2534Filters : Form
    {
        public frmJ2534Filters()
        {
            InitializeComponent();
        }

        private enum MsgType
        {
            mask,
            pattern,
            flowcontrol
        }
        private MsgFilter msgfilter;
        public List<JFilter> finalFilter;
        private List<JFilter> savedPresets;
        private List<JFilter> savedFilters;
        //private JFilter currentFilter;
        private readonly string savedPresetsFile = Path.Combine(Application.StartupPath, "Logger", "savedpresets.xml");
        private readonly string savedFiltersFile = Path.Combine(Application.StartupPath, "Logger", "savedfilters.xml");
        private int currentFilter = 0;
        //private readonly string emptytext = "Type here or select...";

        private class ComboBoxItem
        {
            public string Txt { get; set; }
            public int Value { get; set; }
            public ComboBoxItem(string text, int value)
            {
                Txt = text;
                Value = value;
            }
        }


        private void frmJ2534Filters_Load(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Columns.Add("Filter", "Filter");
                int row = dataGridView1.Rows.Add();
                MsgFilter tmpFilter;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                if (finalFilter == null || finalFilter.Count == 0)
                {
                    finalFilter = new List<JFilter>();
                    finalFilter.Add(new JFilter());
                    tmpFilter = new MsgFilter();
                }
                else
                {
                    tmpFilter = new MsgFilter(finalFilter[0].Filter, ProtocolID.J1850VPW);
                    ShowFinalFilter();
                }
                comboFType.Items.Clear();
                comboFType.DataSource = EnumWithName<FilterType>.ParseEnum();
                comboFType.DisplayMember = "Name";
                txtMask.Leave += TxtMask_Leave;
                txtPattern.Leave += TxtPattern_Leave;
                txtFlow.Leave += TxtFlow_Leave;
                comboPresets.Leave += ComboPresets_Leave;
                //dataGridView1.Columns.Add("Name", "Name");
                //dataGridView1.Columns[0].Width = 200;
                //dataGridView1.Columns[1].Width = 700;
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
                msgfilter = tmpFilter;
                ShowCurrentFilter();
                LoadFilters();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void ComboPresets_Leave(object sender, EventArgs e)
        {
            msgfilter.FilterName = comboPresets.Text;
            comboPresets.Leave -= ComboPresets_Leave;
            ShowCurrentFilter();
            comboPresets.Leave += ComboPresets_Leave;
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                currentFilter = dataGridView1.SelectedCells[0].RowIndex;
                msgfilter = new MsgFilter(finalFilter[currentFilter].Filter,ProtocolID.CAN);
                comboPresets.Text = msgfilter.FilterName;
                ShowCurrentFilter();
            }
            catch { }
        }

        public void LoadTxtFilter(string filter)
        {
            try
            {
                if (filter == null)
                {
                    return;
                }
                JFilters jfs = new JFilters(filter, ProtocolID.J1850VPW);
                finalFilter = new List<JFilter>();
                for (int i = 0; i < jfs.MessageFilters.Count; i++)
                {
                    JFilter jf = new JFilter();
                    jf.Filter = jfs.MessageFilters[i].ToString();
                    finalFilter.Add(jf);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        public string GetFinalFilter()
        {
            string retVal = "";
            for (int i = 0; i < finalFilter.Count; i++)
            {
                retVal += finalFilter[i].Filter + Environment.NewLine;
            }
            return retVal;
        }
        private void TxtFlow_Leave(object sender, EventArgs e)
        {
            this.txtFlow.TextChanged -= new System.EventHandler(this.txtFlow_TextChanged);
            ShowCurrentFilter();
            this.txtFlow.TextChanged += new System.EventHandler(this.txtFlow_TextChanged);
        }

        private void TxtPattern_Leave(object sender, EventArgs e)
        {
            this.txtPattern.TextChanged -= new System.EventHandler(this.txtPattern_TextChanged);
            ShowCurrentFilter();
            this.txtPattern.TextChanged += new System.EventHandler(this.txtPattern_TextChanged);
        }

        private void TxtMask_Leave(object sender, EventArgs e)
        {
            this.txtMask.TextChanged -= new System.EventHandler(this.txtMask_TextChanged);
            ShowCurrentFilter();
            this.txtMask.TextChanged += new System.EventHandler(this.txtMask_TextChanged);
        }

        private void LoadFilters()
        {
            try
            {
                if (!File.Exists(savedPresetsFile))
                {
                    savedPresets = new List<JFilter>();
                }
                else
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<JFilter>));
                    StreamReader file = new StreamReader(savedPresetsFile);
                    savedPresets = (List<JFilter>)reader.Deserialize(file);
                    file.Close();
                    foreach (JFilter jf in savedPresets)
                    {
                        comboPresets.Items.Add(jf.FilterName);
                    }
                }
                if (comboPresets.Text.Length == 0)
                {
                    comboPresets.Text = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");
                }
                if (!File.Exists(savedFiltersFile))
                {
                    savedFilters = new List<JFilter>();
                }
                else
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<JFilter>));
                    StreamReader file = new StreamReader(savedFiltersFile);
                    savedFilters = (List<JFilter>)reader.Deserialize(file);
                    file.Close();
                    foreach (JFilter jf in savedFilters)
                    {
                        comboFilters.Items.Add(jf.FilterName);
                    }
                }
                if (comboFilters.Text.Length == 0)
                {
                    comboFilters.Text = DateTime.Now.ToString("yy-MM-dd-HH-mm-ss");
                }
                this.comboFType.SelectedIndexChanged += new System.EventHandler(this.comboFType_SelectedIndexChanged);
                this.comboPresets.SelectedIndexChanged += new System.EventHandler(this.comboPresets_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void SavePresets()
        {
            try
            {
                using (FileStream stream = new FileStream(savedPresetsFile, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<JFilter>));
                    writer.Serialize(stream, savedPresets);
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
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void SaveFilters()
        {
            try
            {

                JFilter jf = savedFilters.Where(x => x.FilterName == comboFilters.Text).FirstOrDefault();
                if (jf == null)
                {
                    jf = new JFilter();
                    jf.FilterName = comboFilters.Text;
                    savedFilters.Add(jf);
                    comboFilters.Items.Add(comboFilters.Text);
                }
                jf.Filter = GetFinalFilter();
                using (FileStream stream = new FileStream(savedFiltersFile, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<JFilter>));
                    writer.Serialize(stream, savedFilters);
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
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (finalFilter.Count == 0)
            {
                AddToFinalFilter();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            finalFilter = new List<JFilter>();
            finalFilter.Add(new JFilter());
            currentFilter = 0;
            ShowFinalFilter();
        }

        private void ShowCurrentFilter()
        {
            try
            {
                comboFType.Text = msgfilter.FilterType.ToString();
                comboPresets.Text = msgfilter.FilterName;
                if (msgfilter.Mask.Data == null)
                    txtMask.Text = "";
                else
                    txtMask.Text = msgfilter.Mask.Data.ToHex().Replace(" ", "");
                if (msgfilter.Pattern.Data == null)
                    txtPattern.Text = "";
                else
                    txtPattern.Text = msgfilter.Pattern.Data.ToHex().Replace(" ", "");
                if (msgfilter.FlowControl.Data == null)
                    txtFlow.Text = "";
                else
                    txtFlow.Text = msgfilter.FlowControl.Data.ToHex().Replace(" ", "");
                btnMaskRx.Text = "RxStatus: " + msgfilter.Mask.RxStatus.ToString().Replace(",", "|");
                btnPatternRx.Text = "RxStatus: " + msgfilter.Pattern.RxStatus.ToString().Replace(",", "|");
                btnFlowRx.Text = "RxStatus: " + msgfilter.FlowControl.RxStatus.ToString().Replace(",", "|");
                btnMaskTx.Text = "TxFlags: " + msgfilter.Mask.TxFlags.ToString().Replace(",", "|");
                btnPatternTx.Text = "TxFlags: " + msgfilter.Pattern.TxFlags.ToString().Replace(",", "|");
                btnFlowTx.Text = "TxFlags: " + msgfilter.FlowControl.TxFlags.ToString().Replace(",", "|");
                if (currentFilter >= finalFilter.Count)
                {
                    dataGridView1.Rows.Add();
                    finalFilter.Add(new JFilter());
                }
                finalFilter[currentFilter].Filter = msgfilter.ToString();
                finalFilter[currentFilter].FilterName = comboPresets.Text;
                dataGridView1.Rows[currentFilter].Cells[0].Value = msgfilter.ToString();
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRows();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void comboPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            JFilter jf = savedPresets.Where(x => x.FilterName == comboPresets.Text).FirstOrDefault();
            if (jf != null)
            {
                msgfilter = new MsgFilter(jf);
                ShowCurrentFilter();
            }
        }

        private void btnPresetsMenu_Click(object sender, EventArgs e)
        {
            contextMenuStripSavedFilters.Show(btnSavedCombosMenu, new Point(0, btnSavedCombosMenu.Height));
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboPresets.Text.Length == 0)
                {
                    return;
                }
                JFilter jf = savedPresets.Where(x => x.FilterName == comboPresets.Text).FirstOrDefault();
                if (jf == null)
                {
                    jf = new JFilter();
                    jf.FilterName = comboPresets.Text;
                    savedPresets.Add(jf);
                    comboPresets.Items.Add(comboPresets.Text);
                }
                jf.Filter = msgfilter.ToString();
                SavePresets();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void btnFiltersMenu_Click(object sender, EventArgs e)
        {
            if (comboFilters.Text.Length == 0 )
            {
                return;
            }
            contextMenuStripFilterCombos.Show(btnFiltersMenu, new Point(0, btnFiltersMenu.Height));
        }

        private void saveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SaveFilters();
        }

        private void ShowFinalFilter()
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < finalFilter.Count; i++)
            {
                int row = dataGridView1.Rows.Add();
                dataGridView1.Rows[row].Cells[0].Value = finalFilter[i].Filter;
            }
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRows();
        }

        private void addToFiltersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToFinalFilter();
        }

        private void comboFilterCombos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                JFilter jf = savedFilters.Where(x => x.FilterName == comboFilters.Text).FirstOrDefault();
                LoadTxtFilter(jf.Filter);
                if (jf != null)
                {
                    ShowFinalFilter();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int f = 0; f < savedPresets.Count; f++)
                {
                    if (savedPresets[f].FilterName == comboPresets.Text)
                    {
                        savedPresets.RemoveAt(f);
                        break;
                    }
                }
                if (comboPresets.SelectedIndex >= 0)
                    comboPresets.Items.RemoveAt(comboPresets.SelectedIndex);
                comboPresets.Text = "";
                SavePresets();
                msgfilter = new MsgFilter();
                ShowCurrentFilter();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                for (int f = 0; f < savedFilters.Count; f++)
                {
                    if (savedFilters[f].FilterName == comboFilters.Text)
                    {
                        savedFilters.RemoveAt(f);
                        break;
                    }
                }
                comboFilters.Items.RemoveAt(comboFilters.SelectedIndex);
                comboFilters.Text = "";
                finalFilter = new List<JFilter>();
                ShowFinalFilter();
                SaveFilters();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void ShowTxPopup(MsgType msgtype, TxFlag txflag)
        {
            try
            {
                CheckedListBox chkList = new CheckedListBox();
                chkList.Tag = msgtype;
                string[] selected = txflag.ToString().Replace(" ", "").Split(',');
                int i = 0;
                chkList.Items.Add("Select all");
                foreach (string item in Enum.GetNames(typeof(J2534DotNet.TxFlag)))
                {
                    chkList.Items.Add(item);
                    if (selected.Contains(chkList.Items[i]))
                    {
                        chkList.SetItemChecked(i, true);
                    }
                    i++;
                }
                chkList.Height = (chkList.Items.Count + 1) * (chkList.GetItemHeight(0) + 5) + 15;
                Application.DoEvents();
                chkList.Width = 200;
                chkList.CheckOnClick = true;
                chkList.SelectedValueChanged += ChkList_Tx_SelectedValueChanged1;
                PopupWindow popup = new PopupWindow(chkList, Cursor.Position);
                popup.Show();
                popup.Top = Cursor.Position.Y;
                popup.Left = Cursor.Position.X;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void ChkList_Tx_SelectedValueChanged1(object sender, EventArgs e)
        {
            try
            {
                CheckedListBox chklist = (CheckedListBox)sender;
                MsgType msgtype = (MsgType)chklist.Tag;
                string selection = "";
                if (chklist.SelectedIndex == 0)
                {
                    for (int c = 0; c < chklist.Items.Count; c++)
                    {
                        chklist.SetItemChecked(c, chklist.GetItemChecked(0));
                    }
                }
                foreach (int selInd in chklist.CheckedIndices)
                {
                    if (selInd > 0)
                    {
                        selection += chklist.Items[selInd].ToString() + "|";
                    }
                }
                switch (msgtype)
                {
                    case MsgType.mask:
                        msgfilter.Mask.TxFlags = ParseTxFLags(selection.Trim('|'));
                        break;
                    case MsgType.pattern:
                        msgfilter.Pattern.TxFlags = ParseTxFLags(selection.Trim('|'));
                        break;
                    case MsgType.flowcontrol:
                        msgfilter.FlowControl.TxFlags = ParseTxFLags(selection.Trim('|'));
                        break;
                }
                ShowCurrentFilter();
                Debug.WriteLine("Sender: " + chklist.Name + ", selection: " + selection);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void ShowRXPopup(MsgType msgtype, RxStatus rxstat)
        {
            try
            {
                CheckedListBox chkList = new CheckedListBox();
                chkList.Tag = msgtype;
                string[] selected = rxstat.ToString().Replace(" ", "").Split(',');
                int i = 0;
                chkList.Items.Add("Select all");
                foreach (string item in Enum.GetNames(typeof(J2534DotNet.RxStatus)))
                {
                    chkList.Items.Add(item);
                    if (selected.Contains(chkList.Items[i]))
                    {
                        chkList.SetItemChecked(i, true);
                    }
                    i++;
                }
                chkList.Height = (chkList.Items.Count + 1) * (chkList.GetItemHeight(0) + 5) + 15;
                Application.DoEvents();
                chkList.Width = 200;
                chkList.CheckOnClick = true;
                chkList.SelectedValueChanged += ChkList_Rx_SelectedValueChanged;
                PopupWindow popup = new PopupWindow(chkList, Cursor.Position);
                popup.Show();
                popup.Top = Cursor.Position.Y;
                popup.Left = Cursor.Position.X;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void btnMaskRxPopup_Click(object sender, EventArgs e)
        {
            if (msgfilter == null)
            {
                msgfilter = new MsgFilter();
            }
            ShowRXPopup(MsgType.mask, msgfilter.Mask.RxStatus);
        }

        private void ChkList_Rx_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                CheckedListBox chklist = (CheckedListBox)sender;
                MsgType msgtype = (MsgType)chklist.Tag;
                string selection = "";
                if (chklist.SelectedIndex == 0)
                {
                    for (int c = 0; c < chklist.Items.Count; c++)
                    {
                        chklist.SetItemChecked(c, chklist.GetItemChecked(0));
                    }
                }
                foreach (int selInd in chklist.CheckedIndices)
                {
                    if (selInd > 0)
                    {
                        selection += chklist.Items[selInd].ToString() + "|";
                    }
                }
                switch (msgtype)
                {
                    case MsgType.mask:
                        msgfilter.Mask.RxStatus = ParseRxStatus(selection.Trim('|'));
                        break;
                    case MsgType.pattern:
                        msgfilter.Pattern.RxStatus = ParseRxStatus(selection.Trim('|'));
                        break;
                    case MsgType.flowcontrol:
                        msgfilter.FlowControl.RxStatus = ParseRxStatus(selection.Trim('|'));
                        break;
                }
                ShowCurrentFilter();
                Debug.WriteLine("Sender: " + chklist.Name + ", selection: " + selection);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void txtMask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                msgfilter.Mask.Data = txtMask.Text.ToBytes();
            }
            catch { };
        }

        private void txtPattern_TextChanged(object sender, EventArgs e)
        {
            try
            {
                msgfilter.Pattern.Data = txtPattern.Text.ToBytes();
            }
            catch { };

        }

        private void txtFlow_TextChanged(object sender, EventArgs e)
        {
            try
            {
                msgfilter.FlowControl.Data = txtFlow.Text.ToBytes();
            }
            catch { };
        }

        private void comboFType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                msgfilter.FilterType = ((EnumWithName<FilterType>)comboFType.SelectedItem).Value;
                this.comboFType.SelectedIndexChanged -= new System.EventHandler(this.comboFType_SelectedIndexChanged);
                ShowCurrentFilter();
                this.comboFType.SelectedIndexChanged += new System.EventHandler(this.comboFType_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void btnPatternRx_Click(object sender, EventArgs e)
        {
            if (msgfilter == null)
            {
                msgfilter = new MsgFilter();
            }
            ShowRXPopup(MsgType.pattern, msgfilter.Pattern.RxStatus);

        }

        private void btnFlowRx_Click(object sender, EventArgs e)
        {
            if (msgfilter == null)
            {
                msgfilter = new MsgFilter();
            }
            ShowRXPopup(MsgType.flowcontrol, msgfilter.FlowControl.RxStatus);

        }

        private void btnMaskTx_Click(object sender, EventArgs e)
        {
            ShowTxPopup(MsgType.mask, msgfilter.Mask.TxFlags);
        }

        private void btnPatternTx_Click(object sender, EventArgs e)
        {
            ShowTxPopup(MsgType.pattern, msgfilter.Pattern.TxFlags);
        }

        private void btnFlowTx_Click(object sender, EventArgs e)
        {
            ShowTxPopup(MsgType.flowcontrol, msgfilter.FlowControl.TxFlags);
        }

        private void ResetFilter()
        {
            msgfilter = new MsgFilter();
            ShowCurrentFilter();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetFilter();
        }

        private void AddToFinalFilter()
        {
            try
            {
                JFilter jf = new JFilter();
                jf.FilterName = comboPresets.Text;
                jf.Filter = msgfilter.ToString();
                finalFilter.Add(jf);
                ShowFinalFilter();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }
        private void btnAddToFilter_Click(object sender, EventArgs e)
        {
            AddToFinalFilter();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                msgfilter = new MsgFilter();
                finalFilter.Add(new JFilter());
                currentFilter = dataGridView1.Rows.Add();
                dataGridView1.CurrentCell = dataGridView1.Rows[currentFilter].Cells[0];
                comboPresets.Text = msgfilter.FilterName;
                ResetFilter();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btnDeleteFromFinalFilter_Click(object sender, EventArgs e)
        {
            try
            {
                finalFilter.RemoveAt(currentFilter);
                currentFilter = 0;
                if (finalFilter.Count == 0)
                    finalFilter.Add(new JFilter());
                ShowFinalFilter();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmJ2534Filters " + line + ": " + ex.Message);
            }
        }
    }
}
