using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;
using System.IO;
using System.Diagnostics;

namespace UniversalPatcher
{
    public partial class frmSwapSegmentList : Form
    {
        public frmSwapSegmentList()
        {
            InitializeComponent();
            txtResult.EnableContextMenu();
        }

        public PcmFile PCM;
        private byte[] SwapBuffer;
        private bool Applied = false;
        private bool Swapped = false;
        public void LoadSegmentList(ref PcmFile PCM1)
        {
            PCM = PCM1;
            labelBasefile.Text = Path.GetFileName(PCM.FileName);
            comboSegments.Items.Clear();
            //for (int s=0;s<PCM.segmentinfos.Length;s++)
            comboSegments.ValueMember = "SegNr";
            comboSegments.DisplayMember = "Name";
            comboSegments.DataSource = PCM.segmentinfos;

            listSegments.Enabled = true;
            listSegments.Clear();
            listSegments.View = View.Details;
            listSegments.Columns.Add("Segment");
            listSegments.Columns.Add("Description");
            listSegments.Columns[0].Width = 250;
            listSegments.Columns[1].Width = 400;
            LoadSegments();
        }

        private void LoadSegments()
        {
            listSegments.Items.Clear();
            SwapBuffer = null;
            labelSelectedSegment.Text = "-";
            if (comboSegments.Text == "OS")
            {
                return;
            }
            string SegNr = ((SegmentInfo)comboSegments.SelectedItem).SegNr;
            string Folder = Path.Combine(Application.StartupPath, "Segments", PCM.OS, SegNr);
            if (!Directory.Exists(Folder))
                return;
            DirectoryInfo d = new DirectoryInfo(Folder);
            FileInfo[] Files = d.GetFiles("*.binsegment");
            foreach (FileInfo file in Files)
            {
                var item = new ListViewItem(file.Name);
                string DescrFile = file.FullName + ".txt";
                if (File.Exists(DescrFile))
                {
                    StreamReader sr = new StreamReader(DescrFile);
                    string line = sr.ReadLine();
                    sr.Close();
                    item.SubItems.Add(line);
                }
                item.Tag = file.FullName;
                listSegments.Items.Add(item);
            }

        }
        private void comboSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }
        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.Focus();
            int Start = txtResult.Text.Length;
            txtResult.AppendText(LogText);
            txtResult.Select(Start, LogText.Length);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            Application.DoEvents();
        }

        private void listSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            try 
            {
                if (comboSegments.Text == "OS")
                {
                    Logger("OS swap disabled");
                    return;
                }
                string FileName = listSegments.SelectedItems[0].Tag.ToString();
                labelSelectedSegment.Text = "Selected: " + listSegments.SelectedItems[0].Text;
                labelSelectedSegment.Tag = FileName;
                uint fsize = (uint)new FileInfo(FileName).Length;
                Logger("Reading file: " + FileName);
                SwapBuffer = ReadBin(FileName, 0, fsize);
                Logger("[OK] Press \"Apply\" to swap");
                Applied = false;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private bool ApplySegment()
        {
            if (SwapBuffer == null)
            {
                Logger("No segment selected");
                return false;
            }
            try
            {
                Logger("Applying segment: " + Path.GetFileName(labelSelectedSegment.Tag.ToString()), false);
                int s = comboSegments.SelectedIndex;
                uint Offset = 0;
                for (int i = 0; i < PCM.binfile[s].SegmentBlocks.Count; i++)
                {
                    uint Start = PCM.binfile[s].SegmentBlocks[i].Start;
                    uint Length = PCM.binfile[s].SegmentBlocks[i].End - PCM.binfile[s].SegmentBlocks[i].Start + 1;                    
                    Debug.WriteLine("Copy data: " + Start.ToString("X") + " - " + PCM.binfile[s].SegmentBlocks[i].End.ToString("X"));
                    Array.Copy(SwapBuffer, Offset, PCM.buf, Start, Length);
                    Offset += Length;
                }
                Logger(" [OK]");
                Applied = true;
                Swapped = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
                return false;
            }

        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplySegment();          
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Applied && SwapBuffer != null)
            {
                if (!ApplySegment())
                    return;
            }
            if (Swapped)
                this.DialogResult = DialogResult.OK;
            else
                this.DialogResult = DialogResult.None;
            this.Close();
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            if (comboSegments.Text == "OS")
            {
                Logger("OS swap disabled");
                return;
            }
            string FileName = SelectFile();
            if (FileName.Length == 0)
                return;
            try
            {
                Logger("Reading segment from file: " + FileName);
                PcmFile tmpPCM = new PcmFile(FileName);
                tmpPCM.GetSegmentAddresses();
                tmpPCM.GetInfo();
                if (tmpPCM.OS != PCM.OS)
                {
                    throw new Exception(Environment.NewLine +  "OS mismatch: " + PCM.OS + " <> " + tmpPCM.OS);
                }
                int Seg = comboSegments.SelectedIndex;
                uint TotalLength = 0;
                if (!HexToUint(PCM.segmentinfos[Seg].Size, out TotalLength))
                    throw new Exception("Cant't decode HEX: " + PCM.segmentinfos[Seg].Size);
                SwapBuffer = new byte[TotalLength];
                labelSelectedSegment.Text = "Selected: " + tmpPCM.segmentinfos[Seg].PN + tmpPCM.segmentinfos[Seg].Ver + "  (From file: " + Path.GetFileName(FileName) +")";
                labelSelectedSegment.Tag = FileName;
                uint Offset = 0;
                for (int s=0; s < PCM.binfile[Seg].SegmentBlocks.Count; s++)
                {
                    uint Start = PCM.binfile[Seg].SegmentBlocks[s].Start;
                    uint Length = PCM.binfile[Seg].SegmentBlocks[s].End - PCM.binfile[Seg].SegmentBlocks[s].Start + 1;
                    Array.Copy(tmpPCM.buf, Start, SwapBuffer,Offset, Length);
                    Offset += Length;
                }
                Logger(" (" + TotalLength.ToString() + " B)", false);
                Logger("[OK] Press \"Apply\" to swap");
                Applied = false;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
    }
}
