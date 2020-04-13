using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmCompare : Form
    {
        public frmCompare()
        {
            InitializeComponent();
        }

        private PcmFile PCM1;
        private PcmFile PCM2;
        private CheckBox[] chkSegments;
        private CheckBox[] chkExtractSegments;
        private string LastXML = "";

        public void addCheckBoxes()
        {
            if (LastXML == XMLFile && chkSegments != null && chkSegments.Length == Segments.Count)
                return;
            if (chkSegments != null)
            {
                for (int s = 0; s < chkSegments.Length; s++)
                {
                    chkSegments[s].Dispose();
                }
            }
            int Left = 6;
            chkSegments = new CheckBox[Segments.Count];
            chkExtractSegments = new CheckBox[Segments.Count];
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = new CheckBox();
                this.Controls.Add(chk);
                chk.Location = new Point(Left, 100);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                chk.Tag = s;
                if (!chk.Text.ToLower().Contains("eeprom"))
                    chk.Checked = true;
                chkSegments[s] = chk;
                Left += chk.Width + 5;

            }
            LastXML = XMLFile;
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
                else
                    txtOS.Text = PCM1.OS;
                //RefreshCVNlist();
                if (Show)
                    ShowFileInfo(PCM, InfoOnly);
                addCheckBoxes();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void btnFile1_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile();
            if (fileName.Length == 0)
                return;
            labelFile1.Text = fileName;
            PCM1 = new PcmFile(fileName);
            GetFileInfo(fileName, ref PCM1, false, true);
        }

        private void btnFile2_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile();
            if (fileName.Length == 0)
                return;
            labelFile2.Text = fileName;
            PCM2 = new PcmFile(fileName);
            GetFileInfo(fileName, ref PCM2,true, true);
        }

        private void CompareBlock(byte[] OrgFile, byte[] PCM2, uint Start, uint End, string CurrentSegment, AddressData[] SkipList)
        {
            Logger(" [" + Start.ToString("X") + " - " + End.ToString("X") + "] ");
            uint ModCount = 0;
            XmlPatch xpatch = new XmlPatch();
            bool BlockStarted = false;
            for (uint i = Start; i < End; i++)
            {
                if (OrgFile[i] != PCM2[i])
                {
                    bool SkipAddr = false;
                    for (int s = 0; s < SkipList.Length; s++)
                    {
                        if (SkipList[s].Bytes > 0 && i >= SkipList[s].Address && i <= (uint)(SkipList[s].Address + SkipList[s].Bytes - 1))
                        {
                            SkipAddr = true;
                        }
                    }
                    if (SkipAddr)
                    {
                        Debug.WriteLine("Skipping: " + i.ToString("X") + "(" + CurrentSegment + ")");
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

                        xpatch.Data += PCM2[i].ToString("X2");
                        ModCount++;
                        if (ModCount <= numSuppress.Value)
                            Logger(i.ToString("X6") + ": " + OrgFile[i].ToString("X2") + " => " + PCM2[i].ToString("X2"));
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
                if (PCM1.fsize != PCM2.fsize)
                {
                    Logger("Files are different size, will not compare!");
                    return;
                }
                if (!checkAppendPatch.Checked || PatchList == null)
                    PatchList = new List<XmlPatch>();

                labelBinSize.Text = PCM1.fsize.ToString();
                if (Segments.Count == 0)
                {
                    Logger("No segments defined, comparing complete file");
                    AddressData[] SkipList = new AddressData[0];
                    CompareBlock(PCM1.buf, PCM2.buf, 0, (uint)PCM1.fsize, "", SkipList);
                }
                else if (chkCompareAll.Checked)
                {
                    Logger("Comparing complete file");
                    AddressData[] SkipList = new AddressData[0];
                    CompareBlock(PCM1.buf, PCM2.buf, 0, (uint)PCM1.fsize, "", SkipList);
                }
                else
                {
                    for (int Snr = 0; Snr < Segments.Count; Snr++)
                    {
                        if (chkSegments[Snr].Enabled && chkSegments[Snr].Checked)
                        {
                            Logger("Comparing segment " + Segments[Snr].Name, false);
                            for (int p = 0; p < PCM1.binfile[Snr].SegmentBlocks.Count; p++)
                            {
                                uint Start = PCM1.binfile[Snr].SegmentBlocks[p].Start;
                                uint End = PCM1.binfile[Snr].SegmentBlocks[p].End;
                                AddressData[] SkipList = new AddressData[2];
                                if (Segments[Snr].CS1Address != null && Segments[Snr].CS1Address != "")
                                    SkipList[0] = PCM1.binfile[Snr].CS1Address;
                                if (Segments[Snr].CS2Address != null && Segments[Snr].CS2Address != "")
                                    SkipList[1] = PCM1.binfile[Snr].CS2Address;
                                CompareBlock(PCM1.buf, PCM2.buf, Start, End, Segments[Snr].Name, SkipList);
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
            if (labelFile1.Text.Length == 0 || labelFile2.Text.Length == 0)
                return;
            if (Segments != null && Segments.Count > 0)
            {
                labelXML.Text = Path.GetFileName(XMLFile) + " (v " + Segments[0].Version + ")";
            }
            if (txtOS.Text.Length == 0)
            {
                txtOS.Text = "ALL";
            }

            CompareBins();
            if (FormPatcheditor == null || !FormPatcheditor.Visible)
            {
                FormPatcheditor = new frmPatcheditor();
                FormPatcheditor.MdiParent = FormMain;
                FormPatcheditor.Show();
            }
            FormPatcheditor.RefreshDatagrid();
        }

        private void btnSaveFileInfo_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void labelFile1_Click(object sender, EventArgs e)
        {

        }

        private void labelFile2_Click(object sender, EventArgs e)
        {

        }
    }
}
