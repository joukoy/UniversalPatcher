using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;
using System.Text.RegularExpressions;

using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UniversalPatcher
{
    public partial class FrmPatcher : Form
    {
        public FrmPatcher()
        {
            InitializeComponent();
            txtResult.EnableContextMenu();
            txtDebug.EnableContextMenu();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (Properties.Settings.Default.DebugOn)
            {
                //TextBoxTraceListener tbtl = new TextBoxTraceListener(txtDebug);
                RichTextBoxTraceListener tbtl = new RichTextBoxTraceListener(txtDebug);
                Debug.Listeners.Add(tbtl);
            }
        }


        private frmSegmenList frmSL;
        private PcmFile basefile;
        private PcmFile modfile;
        private CheckBox[] chkSegments;
        private CheckBox[] chkExtractSegments;
        private string LastXML = "";
        private BindingSource bindingSource = new BindingSource();
        private BindingSource CvnSource = new BindingSource();
        private BindingSource Finfosource = new BindingSource();

        private void FrmPatcher_Load(object sender, EventArgs e)
        {
            addCheckBoxes();
            numSuppress.Value = Properties.Settings.Default.SuppressAfter;
            if (numSuppress.Value == 0)
                numSuppress.Value = 10;
            chkDebug.Checked = Properties.Settings.Default.DebugOn;

        }

        public void addCheckBoxes()
        {
            if (LastXML == XMLFile && chkSegments != null && chkSegments.Length == Segments.Count)
                return;
            if (chkSegments != null)
            {
                for (int s = 0; s < chkSegments.Length; s++)
                {
                    chkSegments[s].Dispose();
                    chkExtractSegments[s].Dispose();
                }
            }
            int Left = 6;
            chkSegments = new CheckBox[Segments.Count];
            chkExtractSegments = new CheckBox[Segments.Count];
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = new CheckBox();
                tabCreate.Controls.Add(chk);
                chk.Location = new Point(Left, 80);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                if (!chk.Text.ToLower().Contains("eeprom"))
                    chk.Checked = true;
                chkSegments[s] = chk;

                chk = new CheckBox();
                tabExtractSegments.Controls.Add(chk);
                chk.Location = new Point(Left, 80);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                chk.Checked = true;
                chkExtractSegments[s] = chk;

                Left += chk.Width + 5;

            }
            LastXML = XMLFile;

        }

        private void CheckSegmentCompatibility()
        {
            if ( txtBaseFile.Text == "" || txtModifierFile.Text == "")
                return;

            labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            for (int s = 0; s < Segments.Count; s++)
            {
                string BasePN = basefile.ReadInfo(basefile.binfile[s].PNaddr);
                string ModPN = modfile.ReadInfo(modfile.binfile[s].PNaddr);
                string BaseVer = basefile.ReadInfo(basefile.binfile[s].VerAddr);
                string ModVer = modfile.ReadInfo(modfile.binfile[s].VerAddr);

                if (BasePN != ModPN || BaseVer != ModVer)
                {
                    Logger(Segments[s].Name.PadLeft(11) + " differ: " + BasePN.ToString().PadRight(8) + " " + BaseVer + " <> " + ModPN.ToString().PadRight(8) + " " + ModVer);
                    chkSegments[s].Enabled = false;
                }
                else
                {
                    chkSegments[s].Enabled = true;
                }
            }
        }


        private void ShowFileInfo(PcmFile PCM, bool InfoOnly)
        {
            try
            {                
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(PCM.segmentinfos[i].Name.PadRight(11), false);
                    if (PCM.segmentinfos[i].PN.Length > 1)
                    {
                        if (PCM.segmentinfos[i].Stock == "True")
                            LoggerBold(" PN: " + PCM.segmentinfos[i].PN.PadRight(9), false);
                        else
                            Logger(" PN: " + PCM.segmentinfos[i].PN.PadRight(9), false);
                    }
                    if (PCM.segmentinfos[i].Ver.Length > 1)
                        Logger(", Ver: " + PCM.segmentinfos[i].Ver, false);

                    if (PCM.segmentinfos[i].SegNr.Length > 0)
                        Logger(", Nr: " + PCM.segmentinfos[i].SegNr.PadRight(3), false);
                    if (chkRange.Checked)
                        Logger("[" + PCM.segmentinfos[i].Address + "]", false);
                    if (chkSize.Checked)
                        Logger(", Size: " + PCM.segmentinfos[i].Size.ToString(), false);
                    if (PCM.segmentinfos[i].ExtraInfo != null && PCM.segmentinfos[i].ExtraInfo.Length > 0)
                        Logger(Environment.NewLine + PCM.segmentinfos[i].ExtraInfo, false);

                    if (!txtResult.Text.EndsWith(Environment.NewLine))
                        Logger("");
                }
                if (chkCS1.Checked || chkCS2.Checked)
                {
                    Logger("Checksums:");
                    for (int i = 0; i < Segments.Count; i++)
                    {
                        SegmentConfig S = Segments[i];
                        if (S.CS1Method != CSMethod_None && chkCS1.Checked)
                        {
                            if (PCM.binfile[i].CS1Address.Bytes == 0)
                            {
                                Logger(" Checksum1: " + PCM.segmentinfos[i].CS1Calc, false);
                            }
                            else
                            {
                                if (PCM.segmentinfos[i].CS1 == PCM.segmentinfos[i].CS1Calc)
                                {
                                    Logger(" Checksum 1: " + PCM.segmentinfos[i].CS1 + " [OK]", false);
                                }
                                else
                                {
                                    Logger(" Checksum 1: " + PCM.segmentinfos[i].CS1 + ", Calculated: " + PCM.segmentinfos[i].CS1Calc + " [Fail]", false);
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None && chkCS2.Checked)
                        {
                            if (PCM.binfile[i].CS2Address.Bytes == 0)
                            {
                                Logger(" Checksum2: " + PCM.segmentinfos[i].CS2Calc, false);
                            }
                            else
                            {
                                if (PCM.segmentinfos[i].CS2 == PCM.segmentinfos[i].CS2Calc)
                                {
                                    Logger(" Checksum 2: " + PCM.segmentinfos[i].CS2 + " [OK]", false);
                                }
                                else
                                {
                                    Logger(" Checksum 2:" + PCM.segmentinfos[i].CS2 + ", Calculated: " + PCM.segmentinfos[i].CS2Calc + " [Fail]", false);
                                }
                            }
                        }
                        if (PCM.segmentinfos[i].Stock == "True")
                            LoggerBold("[Stock]", true);

                        if (!txtResult.Text.EndsWith(Environment.NewLine))
                            txtResult.AppendText(Environment.NewLine);
                    }
                }
                if (!InfoOnly)
                {
                    addCheckBoxes();
                    CheckSegmentCompatibility();
                }
                RefreshFileInfo();
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void GetFileInfo(string FileName, ref PcmFile PCM, bool InfoOnly, bool Show = true)
        {
            try
            {
                if (chkAutodetect.Checked)
                {
                    string ConfFile = Autodetect(PCM);
                    Logger("Autodetect: " + ConfFile);
                    if (ConfFile == "" || ConfFile.Contains(Environment.NewLine))
                    {
                        labelXML.Text = "";
                        XMLFile = "";
                        Segments.Clear();
                    }
                    else
                    {
                        ConfFile = Path.Combine(Application.StartupPath, "XML", ConfFile);
                        if (File.Exists(ConfFile))
                        {
                            frmSegmenList frmSL = new frmSegmenList();
                            frmSL.LoadFile(ConfFile);
                        }
                        else
                        {
                            Logger("XML File not found");
                            labelXML.Text = "";
                            XMLFile = "";
                            Segments.Clear();
                            Logger(Environment.NewLine + Path.GetFileName(FileName));
                            return;
                        }
                    }
                }
                if (Segments == null || Segments.Count == 0)
                {
                    labelXML.Text = "";
                    Logger(Environment.NewLine + Path.GetFileName(FileName));
                    addCheckBoxes();
                    return;
                }
                labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
                Logger(Environment.NewLine + Path.GetFileName(FileName) + " (" + labelXML.Text + ")" + Environment.NewLine);
                PCM.GetSegmentAddresses();
                if (Segments.Count > 0)
                    Logger("Segments:");
                PCM.GetInfo();
                if (PCM.OS == null || PCM.OS == "")
                    LoggerBold("Warning: No OS segment defined, limiting functions");
                RefreshCVNlist();
                if (Show)
                    ShowFileInfo(PCM, InfoOnly);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        public void LoadFile(string FileName)
        {
            txtBaseFile.Text = FileName;
            basefile = new PcmFile(FileName);
            labelBinSize.Text = basefile.fsize.ToString();
            GetFileInfo(txtBaseFile.Text, ref basefile, false);
            txtOS.Text = basefile.OS;

        }
        private void btnOrgFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile();
            if (FileName.Length > 1)
            {
                LoadFile(FileName);
            }
        }

        private void btnModFile_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile();
            if (FileName.Length > 1)
            {
                txtModifierFile.Text = FileName;
                modfile = new PcmFile(FileName);
                GetFileInfo(txtModifierFile.Text, ref modfile, false);
            }

        }

        private void RefreshFileInfo() 
        { 
            dataFileInfo.DataSource = null;
            Finfosource.DataSource = null;
            Finfosource.DataSource = ListSegment;
            dataFileInfo.DataSource = Finfosource;
            if (ListSegment == null || ListSegment.Count == 0)
                tabFinfo.Text = "File info";
            else
                tabFinfo.Text = "File info (" + ListSegment.Count.ToString() + ")";
        }

        private void CompareBlock(byte[] OrgFile, byte[] ModFile, uint Start, uint End, string CurrentSegment, AddressData[] SkipList)
        {
            Logger(" [" + Start.ToString("X") + " - " + End.ToString("X") + "] ");
            uint ModCount = 0;
            XmlPatch xpatch = new XmlPatch();
            bool BlockStarted = false;
            for (uint i = Start; i < End; i++)
            {
                if (OrgFile[i] != ModFile[i])
                {
                    bool SkipAddr = false;
                    for (int s=0; s<SkipList.Length; s++)
                    {
                        if (SkipList[s].Bytes > 0 && i >= SkipList[s].Address && i <= (uint)(SkipList[s].Address + SkipList[s].Bytes - 1))
                        {
                            SkipAddr = true;
                        }
                    }
                    if (SkipAddr)
                    {
                        Debug.WriteLine("Skipping: " + i.ToString("X") + "(" + CurrentSegment +")");
                    }
                    else
                    { 
                        if (!BlockStarted)
                        {
                            //Start new block 
                            xpatch = new XmlPatch();
                            xpatch.XmlFile = Path.GetFileName(XMLFile);
                            xpatch.Data = "";
                            xpatch.Description = "";
                            xpatch.Segment = CurrentSegment;
                            xpatch.Data = "";
                            xpatch.CompatibleOS = txtOS.Text + ":" + i.ToString("X");
                            BlockStarted = true;
                        }
                        else
                            xpatch.Data += " ";

                        xpatch.Data += ModFile[i].ToString("X2");
                        ModCount++;
                        if (ModCount <= numSuppress.Value)
                            Logger(i.ToString("X6") + ": " + OrgFile[i].ToString("X2") + " => " + ModFile[i].ToString("X2"));
                    }

                }
                else if (BlockStarted)
                {
                    //No more differences in this block
                    PatchList.Add(xpatch);
                    BlockStarted = false;
                }

            }
            if (BlockStarted)
            {
                PatchList.Add(xpatch);
            }
            if (ModCount > numSuppress.Value)
            {
                Logger("(Suppressing output)");
                Logger("Total: " + ModCount.ToString() + " differences");
            }

        }
        public void CompareBins()
        {
            try
            {
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                uint fsize2 = (uint)new System.IO.FileInfo(txtModifierFile.Text).Length;
                if (fsize != fsize2)
                {
                    Logger("Files are different size, will not compare!");
                    return;
                }
                basefile = new PcmFile(txtBaseFile.Text);
                modfile = new PcmFile(txtModifierFile.Text);
                if (!checkAppendPatch.Checked || PatchList == null)
                    PatchList = new List<XmlPatch>();
                GetFileInfo(txtBaseFile.Text, ref basefile, false, false);
                GetFileInfo(txtModifierFile.Text, ref modfile, false, false);

                labelBinSize.Text = fsize.ToString();
                if (Segments.Count == 0)
                {
                    Logger("No segments defined, comparing complete file");
                    AddressData[] SkipList = new AddressData[0];
                    CompareBlock(basefile.buf, modfile.buf, 0, (uint)fsize, "", SkipList);
                }
                else if (chkCompareAll.Checked)
                {
                    Logger("Comparing complete file");
                    AddressData[] SkipList = new AddressData[0];
                    CompareBlock(basefile.buf, modfile.buf, 0, (uint)fsize, "", SkipList);
                }
                else
                {
                    for (int Snr = 0; Snr < Segments.Count; Snr++)
                    {
                        if (chkSegments[Snr].Enabled && chkSegments[Snr].Checked)
                        {
                            Logger("Comparing segment " + Segments[Snr].Name, false);
                            for (int p = 0; p < basefile.binfile[Snr].SegmentBlocks.Count; p++)
                            {
                                uint Start = basefile.binfile[Snr].SegmentBlocks[p].Start;
                                uint End = basefile.binfile[Snr].SegmentBlocks[p].End;
                                AddressData[] SkipList = new AddressData[2];
                                if (Segments[Snr].CS1Address != null && Segments[Snr].CS1Address != "")
                                    SkipList[0] = basefile.binfile[Snr].CS1Address;
                                if (Segments[Snr].CS2Address != null && Segments[Snr].CS2Address != "")
                                    SkipList[1] = basefile.binfile[Snr].CS2Address;
                                CompareBlock(basefile.buf, modfile.buf, Start, End, Segments[Snr].Name, SkipList);
                            }
                            Logger("");
                        }
                    }
                }
                if (PatchList.Count > 0)
                    Logger("Created patch for OS: " + txtOS.Text);
                else
                    Logger("No differences found");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SuppressAfter = (uint)numSuppress.Value;
            Properties.Settings.Default.Save();
            if (txtBaseFile.Text.Length == 0 || txtModifierFile.Text.Length == 0)
                return;
            if (Segments != null && Segments.Count > 0)
            { 
                labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            }
            if (txtOS.Text.Length == 0)
            {
                txtOS.Text = "ALL";
            }
            txtResult.Text = "";

            CompareBins();
            RefreshDatagrid();
        }

        private void SavePatch(string Description)
        {
            try
            {
                if (PatchList == null || PatchList.Count == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                /*if (Description.Length < 1 && PatchList[0].Description == null)
                {
                    Logger("Supply patch description");
                    return;
                }*/
                string FileName = SelectSaveFile("XMLPATCH files (*.xmlpatch)|*.xmlpatch|All files (*.*)|*.*");
                if (FileName.Length < 1)
                    return;
                Logger("Saving to file: " + Path.GetFileName(FileName), false);
                if (PatchList[0].Description == null)
                {
                    XmlPatch xpatch = PatchList[0];
                    xpatch.Description = Description;
                    PatchList[0] = xpatch;
                }

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    writer.Serialize(stream, PatchList);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void SaveBin()
        {
            try
            {
                if (basefile == null || basefile.buf == null | basefile.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string FileName = SelectSaveFile();
                if (FileName.Length == 0)
                    return;
                Logger("Saving to file: " + FileName);
                WriteBinToFile(FileName, basefile.buf);
                Logger("Done.");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }

        private void btnSaveBin_Click(object sender, EventArgs e)
        {
            SaveBin();
        }

/*        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.Focus();
            int Start = txtResult.Text.Length;
            txtResult.AppendText(LogText);
            txtResult.Select(Start, LogText.Length);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }
*/
/*        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.Focus();
            int Start = txtResult.Text.Length;
            txtResult.AppendText(LogText);
            txtResult.Select(Start, LogText.Length);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }
*/

        private void btnCheckSums_Click(object sender, EventArgs e)
        {
            if (Segments != null && Segments.Count > 0)
            { 
                FixCheckSums(ref basefile);
            }
        }


        private void btnLoadFolder_Click(object sender, EventArgs e)
        {
            string Fldr = SelectFolder("Select folder");
            if (Fldr.Length == 0)
                return;
            txtResult.Text = "";
            DirectoryInfo d = new DirectoryInfo(Fldr);
            FileInfo[] Files = d.GetFiles("*.bin");
            foreach (FileInfo file in Files)
            {
                PcmFile binfile = new PcmFile(file.FullName);
                GetFileInfo(file.FullName, ref binfile, true);
            }

        }

        private void btnSaveFileInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("Text files (*.txt)|*.txt|All files (*.*)|*.*");
                if (FileName.Length > 1)
                {
                    StreamWriter sw = new StreamWriter(FileName);
                    sw.WriteLine(txtResult.Text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }

        }

        private void btnApplypatch_Click(object sender, EventArgs e)
        {
            if (basefile == null || PatchList == null)
            {
                Logger("Nothing to do");
                return;
            }
            ApplyXMLPatch(ref basefile);
            btnCheckSums_Click(sender, e);
        }

        private void ExtractTable(uint Start, uint End, string[] OSlist, string MaskText)
        {
            try
            {
                XmlPatch xpatch = new XmlPatch();
                xpatch.CompatibleOS = OSlist[0] + ":" + Start.ToString("X"); 
                for (int i=1;i < OSlist.Length; i++)
                    xpatch.CompatibleOS += "," + OSlist[i] + ":" + Start.ToString("X");
                xpatch.XmlFile = Path.GetFileName(XMLFile);
                xpatch.Description = txtExtractDescription.Text;
                Logger("Extracting " + Start.ToString("X") + " - " + End.ToString("X"));
                for (uint i = Start; i <= End; i++)
                {
                    if (i > Start)
                        xpatch.Data += " ";
                    if (MaskText.ToLower() == "ff" || MaskText == "") 
                    { 
                        xpatch.Data += basefile.buf[i].ToString("X2");
                    }
                    else
                    {
                        byte Mask = byte.Parse(MaskText, System.Globalization.NumberStyles.HexNumber);
                        xpatch.Data += (basefile.buf[i] & Mask).ToString("X2") + "[" + MaskText +"]";
                    }
                }
                if (PatchList == null)
                    PatchList = new List<XmlPatch>();
                Logger("[OK]");
                PatchList.Add(xpatch);
                RefreshDatagrid();

            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnExtract_Click(object sender, EventArgs e)
        {
            uint Start;
            uint End;
            string MaskText = "";
            if (txtBaseFile.Text.Length == 0)
                return;
            basefile = new PcmFile(txtBaseFile.Text);
            GetFileInfo(txtBaseFile.Text, ref basefile, true, false);
            if (txtCompatibleOS.Text.Length == 0)
                txtCompatibleOS.Text = basefile.OS;
            if (txtCompatibleOS.Text.Length == 0)
                txtCompatibleOS.Text = "ALL";
            string[] OSlist = txtCompatibleOS.Text.Split(',');
            string[] blocks = txtExtractRange.Text.Split(',');
            for (int b=0; b< blocks.Length; b++) 
            {
                MaskText = "";
                string block = blocks[b];
                if (block.Contains("["))
                {
                    string[] AddrMask = block.Split('[');
                    block = AddrMask[0];
                    MaskText = AddrMask[1].Replace("]", "");
                }
                if (block.Contains(":"))
                {
                    string[] StartEnd = block.Split(':');
                    if (!HexToUint(StartEnd[0], out Start))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[0]);
                        return;
                    }
                    if (!HexToUint(StartEnd[1], out End))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[1]);
                        return;
                    }
                    End += Start - 1;
                }
                else
                {
                    if (!block.Contains("-"))
                    {
                        Logger("Supply address range, for example 200-220 or 200:4");
                        return;
                    }
                    string[] StartEnd = block.Split('-');
                    if (!HexToUint(StartEnd[0], out Start))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[0]);
                        return;
                    }
                    if (!HexToUint(StartEnd[1], out End))
                    {
                        Logger("Can't decode HEX value: " + StartEnd[1]);
                        return;
                    }
                }
                ExtractTable(Start, End, OSlist, MaskText);
            }

        }

        private void RefreshDatagrid()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = PatchList;
            dataPatch.DataSource = null;
            dataPatch.DataSource = bindingSource;
            if (PatchList == null || PatchList.Count == 0)
            { 
                tabPatch.Text = "Patch editor";
            }
            else 
            { 
                tabPatch.Text = "Patch editor (" + PatchList.Count.ToString() +")";
            }
        }

        private void btnManualPatch_Click(object sender, EventArgs e)
        {
            frmManualPatch frmM = new frmManualPatch();
            frmM.MdiParent = this;
            if (PatchList != null && PatchList.Count > 0)
            {
                string[] Oslist = PatchList[0].CompatibleOS.Split(',');
                foreach (string os in Oslist)
                {
                    if (frmM.txtCompOS.Text.Length > 0)
                        frmM.txtCompOS.Text += ",";
                    string[] Parts = os.Split(':');
                    frmM.txtCompOS.Text += Parts[0] + ":";
                    frmM.txtXML.Text = PatchList[0].XmlFile;
                    frmM.txtDescription.Text = PatchList[0].Description;
                }
            }
            else
            {
                if (txtOS.Text.Length > 0)
                    frmM.txtCompOS.Text = txtOS.Text + ":";
                else
                    frmM.txtCompOS.Text = "ALL:";
                if (XMLFile != null && XMLFile.Length > 0)
                    frmM.txtXML.Text = Path.GetFileName(XMLFile);
            }
            
            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                if (PatchList == null)
                    PatchList = new List<XmlPatch>();
                XmlPatch XP = new XmlPatch();
                XP.Description = frmM.txtDescription.Text;
                XP.Segment = frmM.txtSegment.Text;
                XP.XmlFile = frmM.txtXML.Text;
                XP.CompatibleOS = frmM.txtCompOS.Text;
                XP.Data = frmM.txtData.Text;
                if (frmM.txtReadAddr.Text.Length > 0)
                {
                    XP.Rule = frmM.txtReadAddr.Text + "[" + frmM.txtMask.Text + "]";
                    if (frmM.chkNOT.Checked)
                        XP.Rule += "!";
                    XP.Rule += frmM.txtValue.Text;
                }
                PatchList.Add(XP);
                RefreshDatagrid();
            }
        }

        private void btnBinLoadPatch_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DebugOn = chkDebug.Checked;
            Properties.Settings.Default.Save();
            if (chkDebug.Checked)
            {
                RichTextBoxTraceListener tbtl = new RichTextBoxTraceListener(txtDebug);
                Debug.Listeners.Add(tbtl);
            }
            else
            {
                Debug.Listeners.Clear();
            }

        }

        public void EditPatchRow()
        {
            if (dataPatch.SelectedRows.Count == 0 && dataPatch.SelectedCells.Count == 0)
                return;
            if (dataPatch.SelectedRows.Count == 0)
                dataPatch.Rows[dataPatch.SelectedCells[0].RowIndex].Selected = true;
            frmManualPatch frmM = new frmManualPatch();
            if (dataPatch.CurrentRow.Cells[0].Value != null)
                frmM.txtDescription.Text = dataPatch.CurrentRow.Cells[0].Value.ToString();
            if (dataPatch.CurrentRow.Cells[1].Value != null)
                frmM.txtXML.Text = dataPatch.CurrentRow.Cells[1].Value.ToString();
            if (dataPatch.CurrentRow.Cells[2].Value != null)
                frmM.txtSegment.Text = dataPatch.CurrentRow.Cells[2].Value.ToString();
            if (dataPatch.CurrentRow.Cells[3].Value != null)
                frmM.txtCompOS.Text = dataPatch.CurrentRow.Cells[3].Value.ToString();
            if (dataPatch.CurrentRow.Cells[4].Value != null)
                frmM.txtData.Text = dataPatch.CurrentRow.Cells[4].Value.ToString();
            if (dataPatch.CurrentRow.Cells[5].Value != null && dataPatch.CurrentRow.Cells[5].Value.ToString().Contains(":"))
            {
                string[] Parts = dataPatch.CurrentRow.Cells[5].Value.ToString().Split(':');
                if (Parts.Length == 3)
                {
                    frmM.txtReadAddr.Text = Parts[0];
                    frmM.txtMask.Text = Parts[1];
                    frmM.txtValue.Text = Parts[2];
                }
            }
            if (dataPatch.CurrentRow.Cells[6].Value != null)
                frmM.txtHelpFile.Text = dataPatch.CurrentRow.Cells[6].Value.ToString();

            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                dataPatch.CurrentRow.Cells[0].Value = frmM.txtDescription.Text;
                dataPatch.CurrentRow.Cells[1].Value = frmM.txtXML.Text;
                dataPatch.CurrentRow.Cells[2].Value = frmM.txtSegment.Text;
                dataPatch.CurrentRow.Cells[3].Value = frmM.txtCompOS.Text;
                dataPatch.CurrentRow.Cells[4].Value = frmM.txtData.Text;
                if (frmM.txtReadAddr.Text.Length > 0)
                {
                    dataPatch.CurrentRow.Cells[5].Value = frmM.txtReadAddr.Text + ":" + frmM.txtMask.Text + ":" + frmM.txtValue.Text;
                }
                dataPatch.CurrentRow.Cells[6].Value = frmM.txtHelpFile.Text;
            }
            frmM.Dispose();

        }

        private void dataPatch_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditPatchRow();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditPatchRow();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataPatch.SelectedRows.Count == 0)
                return;
            dataPatch.Rows.Remove(dataPatch.CurrentRow);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int row = dataPatch.CurrentRow.Index; 
            if (row == 0)
                return;
            XmlPatch CurrentP = PatchList[row];
            PatchList.RemoveAt(row);
            PatchList.Insert(row - 1, CurrentP);
            RefreshDatagrid();
            dataPatch.CurrentCell = dataPatch.Rows[row - 1].Cells[0];
            dataPatch.Rows[row - 1].Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int row = dataPatch.CurrentRow.Index;
            if (row >= dataPatch.Rows.Count - 2)
                return;
            XmlPatch CurrentP = PatchList[row];
            PatchList.RemoveAt(row);
            PatchList.Insert(row + 1, CurrentP);
            RefreshDatagrid();
            dataPatch.CurrentCell = dataPatch.Rows[row + 1].Cells[0];
            dataPatch.Rows[row + 1].Selected = true;
        }

        private void loadConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select XML file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            frmSegmenList frmSL = new frmSegmenList();
            frmSL.LoadFile(FileName);
            frmSL.Dispose();
            labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            addCheckBoxes();

        }

        private void setupSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSL != null && frmSL.Visible)
            {
                frmSL.BringToFront();
                return;
            }
            frmSL = new frmSegmenList();
            frmSL.InitMe();
            if (frmSL.ShowDialog() == DialogResult.OK)
            {
                //addCheckBoxes();
            }

        }

        private void autodetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAutodetect frmAD = new frmAutodetect();
            frmAD.Show();
            frmAD.InitMe();

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.Show();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshDatagrid();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshDatagrid();

        }

        public void LoadPatch()
        {
            try
            {
                string FileName = SelectFile("Select patch file", "XML patch files (*.xmlpatch)|*.xmlpatch|PATCH files (*.patch)|*.patch|ALL files(*.*)|*.*");
                if (FileName.Length > 1)
                {
                    labelPatchname.Text = FileName;
                    Logger("Loading file: " + FileName);
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                    PatchList = (List<XmlPatch>)reader.Deserialize(file);
                    file.Close();
                    if (PatchList.Count > 0)
                    {
                        Logger("Description: " + PatchList[0].Description);
                        string[] OsList = PatchList[0].CompatibleOS.Split(',');
                        string CompOS = "";
                        foreach (string OS in OsList)
                        {
                            if (CompOS != "")
                                CompOS += ",";
                            string[] Parts = OS.Split(':');
                            CompOS += Parts[0];
                        }
                        Logger("For OS: " + CompOS);
                    }
                    btnApplypatch.Enabled = true;
                    RefreshDatagrid();
                    Logger("[OK]");
                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePatch(txtPatchDescription.Text);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try 
            {
                if (dataPatch.Rows[dataPatch.CurrentRow.Index].Cells[6].Value == null)
                    return;
                string FileName = dataPatch.Rows[dataPatch.CurrentRow.Index].Cells[6].Value.ToString();
                FileName = Path.Combine(Application.StartupPath, "Patches" , FileName);
                Process.Start(FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void buttonAddtoStock_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = false;
                int counter = 0;
                for (int i=0;i < ListCVN.Count; i++)
                {
                    CVN stock = ListCVN[i];
                    counter++;
                    if (!CheckStockCVN(stock.PN,stock.Ver,stock.SegmentNr,stock.cvn, false))
                    {
                        //Add if not already in list
                        StockCVN.Add(stock);
                        isNew = true;
                        Debug.WriteLine(stock.PN + " " + stock.Ver + " cvn: " + stock.cvn + " added");
                    }
                    else
                    {
                        Debug.WriteLine(stock.PN + " " + stock.Ver + " cvn: " + stock.cvn + " Already in list");
                    }
                }
                if (counter == 0)
                {
                    Logger("No CVN defined");
                    return;
                }
                if (!isNew)
                {
                    Logger("All segments already in stock list");
                    return;
                }
                Logger("Saving file stockcvn.xml");
                string FileName = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                    writer.Serialize(stream, StockCVN);
                    stream.Close();
                }
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void stockCVNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditXML frmE = new frmEditXML();
            frmE.LoadStockCVN();
            frmE.Show();
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePatch(txtPatchDescription.Text);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshDatagrid();

        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            RefreshDatagrid();

        }

        private void RefreshCVNlist()
        {
            dataCVN.DataSource = null;
            CvnSource.DataSource = null;
            CvnSource.DataSource = ListCVN;
            dataCVN.DataSource = CvnSource;
            if (ListCVN != null && ListCVN.Count > 0)
                tabCVN.Text = "CVN (" + ListCVN.Count.ToString() + ")";
            else
                tabCVN.Text = "CVN";
        }

        private void ClearCVNlist()
        {
            ListCVN = new List<CVN>();
            dataCVN.DataSource = null;
            CvnSource.DataSource = null;
            CvnSource.DataSource = ListCVN;
            dataCVN.DataSource = CvnSource;
        }

        private void btnClearCVN_Click(object sender, EventArgs e)
        {
            ClearCVNlist();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ListSegment.Clear();
            RefreshFileInfo();
        }

        private void btnSaveCSV_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
            if (FileName.Length == 0)
                return;
            Logger("Writing to file: " + Path.GetFileName(FileName), false);
            using (StreamWriter writetext = new StreamWriter(FileName))
            {
                string row = "";
                for (int i = 0; i < dataFileInfo.Columns.Count; i++)
                {
                    if (i > 0)
                        row += ";";
                    row += dataFileInfo.Columns[i].HeaderText;
                }
                writetext.WriteLine(row);
                for (int r = 0; r < (dataFileInfo.Rows.Count - 1); r++)
                {
                    row = "";
                    for (int i = 0; i < dataFileInfo.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        if (dataFileInfo.Rows[r].Cells[i].Value != null)
                            row += dataFileInfo.Rows[r].Cells[i].Value.ToString();
                    }
                    writetext.WriteLine(row);
                }
            }
            Logger(" [OK]");

        }

        private string SegmentFileName(string FnameStart, string Extension)
        {
            string FileName = FnameStart + Extension;
            FileName = FileName.Replace("?", "_");
            if (radioReplace.Checked)
                return FileName;

            if (!Directory.Exists(Path.GetDirectoryName(FnameStart)))
                Directory.CreateDirectory(Path.GetDirectoryName(FnameStart));

            if (!File.Exists(FileName))
            {
                return FileName;
            }

            if (radioSkip.Checked)
            {
                Logger("Skipping duplicate file: " + FileName);
                return "";
            }

            //radioRename checked
            uint Fnr = 0;
            while (File.Exists(FileName))
            {
                Fnr++;
                FileName = FnameStart + "(" + Fnr.ToString() + ")" + Extension;
            }
            return FileName;
        }

        private void ExtractSegments(PcmFile PCM, string Descr, bool AllSegments, string dstFolder)
        {            
            if (PCM.segmentinfos == null)
            {
                Logger("no segments defined");
                return;
            }
            try
            {
                for (int s=0;s<PCM.segmentinfos.Length;s++)
                {
                    if (AllSegments || chkExtractSegments[s].Checked)
                    {
                        string FileName;
                        if (dstFolder.Length > 0)
                        {
                            string FnameStart = Path.Combine(dstFolder, PCM.segmentinfos[s].PN.PadLeft(8,'0'));
                            FileName = SegmentFileName(FnameStart, ".bin");
                        }
                        else
                        {
                            string FnameStart = Path.Combine(Application.StartupPath, "Segments", PCM.OS, PCM.segmentinfos[s].SegNr, PCM.segmentinfos[s].Name + "-" + PCM.segmentinfos[s].PN + PCM.segmentinfos[s].Ver);
                            FileName = SegmentFileName(FnameStart, ".binsegment");
                        }
                        if (FileName.Length > 0) 
                        { 
                            Logger("Writing " + PCM.segmentinfos[s].Name + " to file: " + FileName + ", size: " + PCM.segmentinfos[s].Size + " (0x" + PCM.segmentinfos[s].Size + ")");
                            WriteSegmentToFile(FileName, PCM.binfile[s].SegmentBlocks, PCM.buf);
                            StreamWriter sw = new StreamWriter(FileName + ".txt");
                            sw.WriteLine(Descr);
                            sw.Close();
                            Logger("[OK]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }
        private void btnExtractSegments_Click(object sender, EventArgs e)
        {
            if (txtSegmentDescription.Text.Length == 0)
                txtSegmentDescription.Text = Path.GetFileName(basefile.FileName).Replace(".bin", "");
            ExtractSegments(basefile, txtExtractDescription.Text, false,"");
        }

        private void btnExtractSegmentsFolder_Click(object sender, EventArgs e)
        {
            //string FileName = SelectFile (Title, Filter);
            string Folder = SelectFolder("Select BIN file directory");
            string dstFolder = "";
            if (checkCustomFolder.Checked)
                dstFolder = SelectFolder("Select destination directory");
            if (Folder.Length == 0)
                return;
            DirectoryInfo d = new DirectoryInfo(Folder);
            FileInfo[] Files = d.GetFiles("*.bin");
            foreach (FileInfo file in Files)
            {
                PcmFile PCM = new PcmFile(file.FullName);
                GetFileInfo(file.FullName, ref PCM, true);
                ExtractSegments(PCM, file.Name.Replace(".bin", ""), true, dstFolder);
            }

        }

        private void btnSwapSegments_Click(object sender, EventArgs e)
        {
            if (basefile == null)
            {
                Logger("No file loaded");
                return;
            }
            if (basefile.OS == null || basefile.OS == "")
            {
                Logger("No OS segment defined");
                return;
            }
            frmSwapSegmentList frmSw = new frmSwapSegmentList();
            frmSw.LoadSegmentList(txtBaseFile.Text);
            if (frmSw.ShowDialog(this) == DialogResult.OK)
            {
                basefile = frmSw.PCM;
                FixCheckSums(ref basefile);
                Logger("Segment(s) swapped and checksums fixed (you can save BIN now)");
            }
            frmSw.Dispose();
        }

        private void FixFileChecksum(string FileName)
        {
            try
            {
                basefile = new PcmFile(FileName);
                GetFileInfo(FileName, ref basefile, true, false);
                if (FixCheckSums(ref basefile))
                {
                    Logger("Saving file: " + FileName);
                    WriteBinToFile(FileName, basefile.buf);
                    Logger("[OK]");
                }
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                for (int i= 0; i< frmF.listFiles.CheckedItems.Count; i++)
                {
                    string FileName = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    FixFileChecksum(FileName);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numSuppress_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataCVN_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataFileInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabFinfo_Click(object sender, EventArgs e)
        {

        }
    }

}
