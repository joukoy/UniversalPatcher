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
            lvwColumnSorter = new ListViewColumnSorter();
            this.listSegments.ListViewItemSorter = lvwColumnSorter;
        }

        private class SegmentData
        {
            public SegmentData(PcmFile pcm) 
            {
                int segmentCount = pcm.segmentinfos.Length;
                name = new string[segmentCount];
                size = new string[segmentCount];
                range = new string[segmentCount];
                for (int s=0; s< pcm.segmentinfos.Length;s++)
                {
                    name[s] = pcm.segmentinfos[s].Name;
                    if (pcm.segmentinfos[s].SwapAddress.Length > 1)
                    {
                        size[s] = pcm.segmentinfos[s].SwapSize;
                        range[s] = pcm.segmentinfos[s].SwapAddress;
                    }
                    else
                    { 
                        size[s] = pcm.segmentinfos[s].Size;
                        range[s] = pcm.segmentinfos[s].Address;
                    }
                }
            }

            public SegmentData(string addrdata, string sizedata)
            {
                string[] addrparts = addrdata.Split(';');
                string[] sizeparts = sizedata.Split(';');
                name = new string[sizeparts.Length];
                size = new string[sizeparts.Length];
                range = new string[sizeparts.Length];
                for (int s=0;s< addrparts.Length;s++)
                {
                    if (addrparts[s].Contains(":") && sizeparts[s].Contains(":"))
                    { 
                        string[] addr = addrparts[s].Split(':');
                        string[] sz = sizeparts[s].Split(':');
                        name[s] = addr[0];
                        range[s] = addr[1];
                        size[s] = sz[1];
                    }
                }
            }
            public string[] name { get; set; }
            public string[] size { get; set; }
            public string[] range { get; set; }

        }

        private ListViewColumnSorter lvwColumnSorter;
        public PcmFile PCM;
        private byte[] SwapBuffer;
        private bool Applied = false;
        private bool Swapped = false;
        private SegmentData PCMsegmentdata;
        private void setuplistview()
        {
            listSegments.Enabled = true;
            listSegments.Clear();
            listSegments.View = View.Details;
            listSegments.FullRowSelect = true;
            listSegments.Columns.Add("Segment");
            listSegments.Columns.Add("Stock");
            listSegments.Columns.Add("Compatible");
            listSegments.Columns.Add("OS");
            listSegments.Columns.Add("P/N");
            listSegments.Columns.Add("Description");
            listSegments.Columns.Add("Size");
            listSegments.Columns.Add("Address");
/*            listSegments.Columns[0].Width = 250;
            listSegments.Columns[1].Width = 50;
            listSegments.Columns[2].Width = 80;
            listSegments.Columns[3].Width = 70;
            listSegments.Columns[4].Width = 100;
            listSegments.Columns[5].Width = 100;
            listSegments.Columns[6].Width = 100;
            listSegments.Columns[7].Width = 100;*/
            if (comboSegments.Text == "OS")
            { 
                for (int s = 0; s < PCM.segmentinfos.Length; s++)
                {
                    listSegments.Columns.Add(PCM.segmentinfos[s].Name);
                    //listSegments.Columns[8 + s].Width = 90;
                }
            }
        }
        public void LoadSegmentList(ref PcmFile PCM1)
        {
            PCM = PCM1;
            PCMsegmentdata = new SegmentData(PCM);
            labelBasefile.Text = Path.GetFileName(PCM.FileName);
            comboSegments.Items.Clear();
            //for (int s=0;s<PCM.segmentinfos.Length;s++)
            comboSegments.ValueMember = "SegNr";
            comboSegments.DisplayMember = "Name";
            comboSegments.DataSource = PCM.segmentinfos;
            setuplistview();
            LoadSegments();
        }

        private string segmentcompatible(SegmentData s1, SegmentData s2, int segNr)
        {
            try {
                if (s1.name[segNr].ToLower().Contains(txtSkiptext.Text.ToLower()) && chkSkipeeprom.Checked)
                {
                    Debug.WriteLine("Skipping segment: " + s1.name[segNr]);
                    return "";
                }
                else
                {
                    if (s1.range[segNr] == s2.range[segNr])
                    {
                        return "1";
                    }
                    else if (s1.size[segNr] == s2.size[segNr])
                    {
                        return "0";
                    }
                    else
                    {
                        return "X";
                    }
                }
            }
            catch 
            {
                return "X";
            }
        }
        private void LoadSegments()
        {
            //listSegments.Items.Clear();
            setuplistview();
            SwapBuffer = null;
            labelSelectedSegment.Text = "-";
            string SegNr = ((SegmentInfo)comboSegments.SelectedItem).SegNr;
            int SegIndex = comboSegments.SelectedIndex;
            labelCurrentPN.Text = "Current P/N: " + PCM.segmentinfos[SegIndex].PN + PCM.segmentinfos[SegIndex].Ver;
            for (int i=0;i< SwapSegments.Count;i++)
            {
                string SegSize;
                string SegAddr;
                if (PCM.segmentinfos[SegIndex].SwapAddress != "")
                { 
                    SegSize = PCM.segmentinfos[SegIndex].SwapSize;
                    SegAddr = PCM.segmentinfos[SegIndex].SwapAddress;
                }
                else
                {
                    SegSize = PCM.segmentinfos[SegIndex].Size;
                    SegAddr = PCM.segmentinfos[SegIndex].Address;
                }

                if (SwapSegments[i].SegIndex == SegIndex && SwapSegments[i].XmlFile == PCM.segmentinfos[SegIndex].XmlFile && SwapSegments[i].Size == SegSize)
                { 
                    var item = new ListViewItem(Path.GetFileName(SwapSegments[i].FileName));
                    item.Tag = Application.StartupPath + SwapSegments[i].FileName;
                    if (SwapSegments[i].Cvn != null && SwapSegments[i].Cvn != "")
                        item.SubItems.Add(CheckStockCVN(SwapSegments[i].PN, SwapSegments[i].Ver, SwapSegments[i].SegNr, SwapSegments[i].Cvn, false, PCM.configFile + ".xml"));
                    else
                        item.SubItems.Add(SwapSegments[i].Stock);
                    SegmentData swapdata;                    

                    bool displaythis = false;
                    if (comboSegments.Text == "OS")
                    {
                        string complevel = "1";
                        swapdata = new SegmentData(SwapSegments[i].SegmentAddresses, SwapSegments[i].SegmentSizes);
                        for (int x = 0; x < PCM.segmentinfos.Length; x++)
                        {
                            string segmentcomp = segmentcompatible(PCMsegmentdata, swapdata, x);
                            if (segmentcomp == "0")
                            {
                                if (complevel == "1")
                                {
                                    complevel = "0";
                                }
                            }
                            else if (segmentcomp == "X")
                            {
                                complevel = "X";
                            }
                        }

                        if (complevel == "1")
                        {
                            item.SubItems.Add("100%");
                            if (chkFullmatch.Checked)
                            { 
                                displaythis = true;
                            }
                        }
                        else if (complevel == "0")
                        {
                            item.SubItems.Add("High chance");
                            item.Tag = null;
                            if (chkHighChance.Checked)
                            { 
                                displaythis = true;
                            }
                        }
                        else
                        {
                            item.SubItems.Add("Less chance");
                            item.Tag = null;
                            if (chkLessChance.Checked)
                            { 
                                displaythis = true;
                            }
                        }
                    }
                    else
                    { 
                        if (SwapSegments[i].OS == PCM.OS)
                        {
                            item.SubItems.Add("100%");
                            if (chkFullmatch.Checked)
                            { 
                                displaythis = true;
                            }
                        }
                        else if (SwapSegments[i].Address == PCM.segmentinfos[SegIndex].Address)
                        {
                            item.SubItems.Add("High chance");
                            if (chkHighChance.Checked)
                            {
                                displaythis = true;
                            }
                        }
                        else
                        {
                            item.SubItems.Add("Less chance");
                            if (chkLessChance.Checked)
                            {
                                displaythis = true;
                            }
                        }
                    }
                    item.SubItems.Add(SwapSegments[i].OS);
                    if (SwapSegments[i].Ver == null )
                        item.SubItems.Add(SwapSegments[i].PN);
                    else
                        item.SubItems.Add(SwapSegments[i].PN + SwapSegments[i].Ver);
                    item.SubItems.Add(SwapSegments[i].Description);
                    item.SubItems.Add(SegSize);
                    item.SubItems.Add(SegAddr);
                    if (comboSegments.Text == "OS" && displaythis)
                    {
                        swapdata = new SegmentData(SwapSegments[i].SegmentAddresses, SwapSegments[i].SegmentSizes);
                        for (int x=0; x < PCM.segmentinfos.Length; x++)
                        {
                            if (radioShow1x0.Checked)
                            {
                                item.SubItems.Add(segmentcompatible(PCMsegmentdata, swapdata, x));
                            }
                            else 
                            {
                                if (!PCM.segmentinfos[x].Name.ToLower().Contains(txtSkiptext.Text.ToLower()) || !chkSkipeeprom.Checked)
                                { 
                                    if (radioShowSize.Checked)
                                    {
                                        item.SubItems.Add(swapdata.size[x]);
                                    }
                                    else
                                    {
                                        item.SubItems.Add(swapdata.range[x]);

                                    }
                                }
                            }

                        }
                    }
                    if (displaythis)
                    { 
                        listSegments.Items.Add(item);
                    }
                }
            }
            listSegments.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        /*
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
                */
        private void comboSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void listSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            try 
            {
                if (comboSegments.Text == "OS")
                {
                    if (listSegments.SelectedItems[0].Tag == null)
                    {
                        Logger("Incompatible segment");
                        return;
                    }
                }
                string FileName = listSegments.SelectedItems[0].Tag.ToString();
                labelSelectedSegment.Text = "Selected: " + listSegments.SelectedItems[0].Text;
                labelSelectedSegment.Tag = FileName;
                uint fsize = (uint)new FileInfo(FileName).Length;
                Logger("Reading file: " + FileName + " (0x" + fsize.ToString("X") +" B)");
                SwapBuffer = ReadBin(FileName, 0, fsize);
                Logger("[OK]");
                Logger("Press \"Apply\" to swap");
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
                if (PCM.segmentinfos[s].SegNr != "")
                {
                    uint segnrAddr = PCM.segmentAddressDatas[s].SegNrAddr.Address - PCM.segmentAddressDatas[s].SegmentBlocks[0].Start;
                    string SwapSegNr = SwapBuffer[segnrAddr].ToString();
                    if (PCM.segmentinfos[s].SegNr != SwapSegNr)
                        throw new Exception(Environment.NewLine + "Segment number doesn't match (" + PCM.segmentinfos[s].SegNr + " != " + SwapSegNr + ")");
                }
                uint Offset = 0;
                if (PCM.segmentinfos[s].SwapAddress.Length > 1)
                { 
                    for (int i = 0; i < PCM.segmentAddressDatas[s].SwapBlocks.Count; i++)
                    {
                        uint Start = PCM.segmentAddressDatas[s].SwapBlocks[i].Start;
                        uint Length = PCM.segmentAddressDatas[s].SwapBlocks[i].End - PCM.segmentAddressDatas[s].SwapBlocks[i].Start + 1;                    
                        Debug.WriteLine("Copy data: " + Start.ToString("X") + " - " + PCM.segmentAddressDatas[s].SwapBlocks[i].End.ToString("X"));
                        Array.Copy(SwapBuffer, Offset, PCM.buf, Start, Length);
                        Offset += Length;
                    }
                }
                else
                { 
                    for (int i = 0; i < PCM.segmentAddressDatas[s].SegmentBlocks.Count; i++)
                    {
                        uint Start = PCM.segmentAddressDatas[s].SegmentBlocks[i].Start;
                        uint Length = PCM.segmentAddressDatas[s].SegmentBlocks[i].End - PCM.segmentAddressDatas[s].SegmentBlocks[i].Start + 1;
                        Debug.WriteLine("Copy data: " + Start.ToString("X") + " - " + PCM.segmentAddressDatas[s].SegmentBlocks[i].End.ToString("X"));
                        Array.Copy(SwapBuffer, Offset, PCM.buf, Start, Length);
                        Offset += Length;
                    }
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
            /*if (comboSegments.Text == "OS")
            {
                Logger("OS swap disabled");
                return;
            }*/
            string FileName = SelectFile();
            if (FileName.Length == 0)
                return;
            try
            {
                int Seg = comboSegments.SelectedIndex;
                uint TotalLength = 0;
                if (PCM.segmentinfos[Seg].SwapAddress.Length > 1)
                {
                    if (!HexToUint(PCM.segmentinfos[Seg].SwapSize, out TotalLength))
                        throw new Exception("Cant't decode HEX: " + PCM.segmentinfos[Seg].Size);
                }
                else
                { 
                    if (!HexToUint(PCM.segmentinfos[Seg].Size, out TotalLength))
                        throw new Exception("Cant't decode HEX: " + PCM.segmentinfos[Seg].Size);
                }
                Logger("Reading segment from file: " + FileName,false);
                uint fsize = (uint)new FileInfo(FileName).Length;
                if (fsize == TotalLength)
                {
                    Logger(" (0x" + fsize.ToString("X") + " B)");
                    SwapBuffer = ReadBin(FileName, 0, fsize);
                    labelSelectedSegment.Text = "Selected: " + Path.GetFileName(FileName);
                    labelSelectedSegment.Tag = FileName;
                    Logger("[OK]");
                    Logger("Press \"Apply\" to swap");
                    Applied = false;
                }
                else if (fsize == PCM.fsize)
                { 
                    PcmFile tmpPCM = new PcmFile(FileName,true,PCM.configFileFullName);
                    //tmpPCM.GetSegmentAddresses();
                    //tmpPCM.GetInfo();
                    /*if (tmpPCM.OS != PCM.OS)
                    {
                        throw new Exception(Environment.NewLine +  "OS mismatch: " + PCM.OS + " <> " + tmpPCM.OS);
                    }*/
                    SwapBuffer = new byte[TotalLength];
                    labelSelectedSegment.Text = "Selected: " + tmpPCM.segmentinfos[Seg].PN + tmpPCM.segmentinfos[Seg].Ver + "  (From file: " + Path.GetFileName(FileName) +")";
                    labelSelectedSegment.Tag = FileName;
                    uint Offset = 0;
                    if (PCM.segmentinfos[Seg].SwapAddress.Length == 0)
                    {
                        for (int s = 0; s < PCM.segmentAddressDatas[Seg].SegmentBlocks.Count; s++)
                        {
                            uint Start = PCM.segmentAddressDatas[Seg].SegmentBlocks[s].Start;
                            uint Length = PCM.segmentAddressDatas[Seg].SegmentBlocks[s].End - PCM.segmentAddressDatas[Seg].SegmentBlocks[s].Start + 1;
                            Array.Copy(tmpPCM.buf, Start, SwapBuffer, Offset, Length);
                            Offset += Length;
                        }
                    }
                    else
                    {
                        for (int s = 0; s < PCM.segmentAddressDatas[Seg].SwapBlocks.Count; s++)
                        {
                            uint Start = PCM.segmentAddressDatas[Seg].SwapBlocks[s].Start;
                            uint Length = PCM.segmentAddressDatas[Seg].SwapBlocks[s].End - PCM.segmentAddressDatas[Seg].SwapBlocks[s].Start + 1;
                            Array.Copy(tmpPCM.buf, Start, SwapBuffer, Offset, Length);
                            Offset += Length;
                        }
                    }
                    Logger(" (0x" + TotalLength.ToString("X") + " B)", false);
                    Logger("[OK]");
                    Logger("Press \"Apply\" to swap");
                    Applied = false;
                }
                else
                {
                    throw new Exception("Unknown file size");
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void listSegments_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listSegments.Sort();
        }

        private void chkFullmatch_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void chkHighChance_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void chkLessChance_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void btnSavelist_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < listSegments.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += listSegments.Columns[i].Text;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < listSegments.Items.Count; r++)
                {
                    row = "";
                    for (int i = 0; i < listSegments.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (listSegments.Items[r].SubItems[i].Text != null)
                            row += listSegments.Items[r].SubItems[i].Text;
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");
        }

        private void radioShow1x0_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void chkSkipeeprom_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void radioShowSize_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }

        private void radioShowRange_CheckedChanged(object sender, EventArgs e)
        {
            LoadSegments();
        }
    }
}
