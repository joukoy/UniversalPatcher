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

namespace UniversalPatcher
{
    public partial class FrmPatcher : Form
    {
        public FrmPatcher()
        {
            InitializeComponent();
        }

        private frmSegmentSettings frmSS;
        private static List<uint> PatchData;
        private static List<uint> PatchAddr;
        private byte[] Basebuf;
        private byte[] Modbuf;
        private BinFile[] basefile;
        private BinFile[] modfile;

        private void FrmPatcher_Load(object sender, EventArgs e)
        {
            this.Show();
            labelXML.Text = Path.GetFileName(XMLFile);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]) )
            {
                //&& Path.GetExtension(args[0]) == ".ini"
                Logger(args[1]);
                frmSegmentSettings frmSS = new frmSegmentSettings();
                //frmS.Show();
                frmSS.LoadFile(args[1]);
            }
            addCheckBoxes();
        }

        private bool[] GetSelections()
        {
            bool[] Selections = new bool[Segments.Count];

            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i].Tag != null)
                {
                    int s = (int)this.Controls[i].Tag;
                    if (this.Controls[i].Text == Segments[s].Name)
                    {
                        CheckBox chk = this.Controls[i] as CheckBox;
                        if (chk.Enabled)
                            Selections[s] = chk.Checked;
                        else
                            Selections[s] = false;
                    }
                }
            }
            return Selections;
        }

        public void addCheckBoxes()
        {
            int i = 0;

            for (i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i].Tag != null && (int)this.Controls[i].Tag > 0 && (int)this.Controls[i].Tag <= Segments.Count)
                    this.Controls[i].Dispose();
            }
            i = 0;
            int Left = 12;
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = new CheckBox();
                this.Controls.Add(chk);
                chk.Location = new Point(Left, 91);
                chk.Text = Segments[s].Name;
                chk.AutoSize = true;
                Left += chk.Width + 5;
                chk.Tag = s;
                chk.Checked = true;
                i++;
            }

        }

        private void CheckSegmentCompatibility()
        {
            labelXML.Text = Path.GetFileName(XMLFile);
            addCheckBoxes();
            for (int s = 0; s < Segments.Count; s++)
            {
                CheckBox chk = null;
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (this.Controls[i].Tag != null && (int)this.Controls[i].Tag == s)
                    {
                        chk = this.Controls[i] as CheckBox;
                    }
                }
                string BasePN = ReadInfo(Basebuf, basefile[s].PNaddr);
                string ModPN = ReadInfo(Modbuf, modfile[s].PNaddr);
                string BaseVer = ReadInfo(Basebuf, basefile[s].VerAddr);
                string ModVer = ReadInfo(Modbuf, modfile[s].VerAddr);

                if (BasePN != ModPN || BaseVer != ModVer)
                {
                    Logger(Segments[s].Name.PadLeft(11) + " differ: " + BasePN.ToString().PadRight(8) + " " + BaseVer + " <> " + ModPN.ToString().PadRight(8) + " " + ModVer);
                    chk.Enabled = false;
                }
                else
                {
                    chk.Enabled = true;
                }
            }
        }

        private void GetFileInfo(string FileName, ref BinFile[] binfile, ref byte[] buf)
        {
            labelXML.Text = Path.GetFileName(XMLFile);
            addCheckBoxes();
            try
            {
                Logger(Environment.NewLine + Path.GetFileName(FileName));
                uint fsize = (uint)new FileInfo(FileName).Length;
                buf = ReadBin(FileName, 0, fsize);
                GetSegmentAddresses(buf, out binfile);
                Logger("Segments:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    string tmp = "";
                    for (int s = 0; s < binfile[i].SegmentBlocks.Count; s++)
                    {
                        if (s > 0)
                            tmp += ", ";
                        tmp = binfile[i].SegmentBlocks[s].Start.ToString("X4") + " - " + binfile[i].SegmentBlocks[s].End.ToString("X4");
                    }
                    Logger(S.Name.PadRight(11) + (" [" + tmp + "]").PadRight(15),false);
                    string PN = ReadInfo(buf, binfile[i].PNaddr);
                    Logger(", PN: " + PN.PadRight(9), false);

                    string Ver = ReadInfo(buf, binfile[i].VerAddr);
                    Logger(", Ver: " + Ver, false);

                    string SNr = ReadInfo(buf, binfile[i].SegNrAddr);
                    Logger(", Nr: " + SNr);
                }
                for (int i=0; i< Segments.Count;i ++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(S.Name);
                    if (S.CS1Method != CSMethod_None)
                    {
                        string tmp = "";
                        for (int s = 0; s < binfile[i].CS1Blocks.Count; s++)
                        {
                            if (s > 0)
                                tmp += ", ";
                            tmp += binfile[i].CS1Blocks[s].Start.ToString("X4") + " - " + binfile[i].CS1Blocks[s].End.ToString("X4");
                        }
                        string CS1Calc = CalculateChecksum(buf, binfile[i].CS1Address, binfile[i].CS1Blocks,binfile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, binfile[i].CS1Address.Bytes,S.CS1SwapBytes).ToString("X4");
                        if (binfile[i].CS1Address.Bytes == 0)
                            Logger(" Checksum1: [" + tmp + "] " + CS1Calc);
                        else
                        { 
                            string CS1 = ReadInfo(buf,binfile[i].CS1Address);
                            if (CS1 == CS1Calc)
                                Logger(" Checksum 1: [" + tmp + "] " + CS1 + " [OK]");
                            else
                                Logger(" Checksum 1: [" + tmp + "] " + CS1 + ", Calculated: " + CS1Calc + " [Fail]");
                        }
                    }

                    if (S.CS2Method != CSMethod_None)
                    {
                        string tmp = "";
                        for (int s = 0; s < binfile[i].CS2Blocks.Count; s++)
                        {
                            if (s > 0)
                                tmp += ", ";
                            tmp += binfile[i].CS2Blocks[s].Start.ToString("X4") + " - " + binfile[i].CS2Blocks[s].End.ToString("X4") ;
                        }
                        string CS2Calc = CalculateChecksum(buf, binfile[i].CS2Address, binfile[i].CS2Blocks, binfile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, binfile[i].CS2Address.Bytes,S.CS2SwapBytes).ToString("X4");
                        if (binfile[i].CS2Address.Bytes == 0)
                            Logger(" Checksum1: [" + tmp + "] " + CS2Calc);
                        else
                        {
                            string CS2 = ReadInfo(buf, binfile[i].CS2Address);
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: [" + tmp + "] " + CS2 + " [OK]");
                            else
                                Logger(" Checksum 2: [" + tmp + "] " + CS2 + ", Calculated: " + CS2Calc + " [Fail]");
                        }
                    }


                }
                if (radioCreate.Checked && txtBaseFile.Text != "" && txtModifierFile.Text != "")
                    CheckSegmentCompatibility();
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }
        private void btnOrgFile_Click(object sender, EventArgs e)
        {
            txtBaseFile.Text = SelectFile();
            if (txtBaseFile.Text.Length > 1)
            {
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                labelBinSize.Text = fsize.ToString(); 
                basefile = new BinFile[Segments.Count];
                Basebuf = new byte[fsize];
                GetFileInfo(txtBaseFile.Text, ref basefile, ref Basebuf);
            }
        }

        private void btnModFile_Click(object sender, EventArgs e)
        {
            if (radioCreate.Checked)
            { 
                txtModifierFile.Text = SelectFile();                
                if (txtModifierFile.Text.Length > 1)
                {
                    uint fsize = (uint)new System.IO.FileInfo(txtModifierFile.Text).Length;
                    modfile = new BinFile[Segments.Count];
                    Modbuf = new byte[fsize];
                    GetFileInfo(txtModifierFile.Text, ref modfile, ref Modbuf);
                }
            }
            else
            {
                try 
                {
                    txtModifierFile.Text = SelectFile("PACTH files (*.patch)|*.patch|ALL files(*.*) | *.*");
                    if (txtModifierFile.Text.Length > 1)
                    {
                        string line;
                        StreamReader sr = new StreamReader(txtModifierFile.Text);
                        line = sr.ReadLine();
                        Logger("Patch is for bin size: " + line);
                        line = sr.ReadLine();
                        Logger(line);
                        line = sr.ReadLine();
                        if (line.Contains(".xml"))
                        {
                            if (File.Exists(line))
                            {
                                if (frmSS == null)
                                    frmSS = new frmSegmentSettings();
                                frmSS.LoadFile(line);
                            }
                        }
                        sr.Close();
                    }
                }
                catch(Exception ex)
                {
                    Logger("Error: " + ex.Message);
                }
            }

        }

        private bool ApplyPatch()
        {
            string line;

            try { 
                StreamReader sr = new StreamReader(txtModifierFile.Text);
                bool FirstLine = true;
                uint fsize = (uint)new System.IO.FileInfo(txtBaseFile.Text).Length;
                Basebuf = new byte[fsize];
                basefile = new BinFile[Segments.Count];
                labelBinSize.Text = fsize.ToString();
                GetFileInfo(txtBaseFile.Text, ref basefile, ref Basebuf);
                while ((line = sr.ReadLine()) != null)
                {
                    if (FirstLine)
                    {
                        if (line != fsize.ToString())
                        {
                            Logger("File size doesn't match patch");
                            return false;
                        }
                    }
                    FirstLine = false;

                    string tmp = Regex.Replace(line, "[^0-9:]", "");
                    if (tmp != line)
                        Logger("(" + line + ")");
                    else
                    {
                        string[] LineParts = line.Split(':');
                        if (LineParts.Length >1) { 
                            uint Addr = uint.Parse(LineParts[0]);
                            uint Data = uint.Parse(LineParts[1]);
                            if (Addr > Basebuf.Length)
                                throw new FileLoadException(String.Format("Address {0} out of range!",  Addr.ToString("X4")));
                            if (Data > 0xff)
                                throw new FileLoadException(String.Format("Data {0} out of range!", Data.ToString("X4")));
                            //Apply patchrow:
                            Logger("Set address: ".PadRight(16) + Addr.ToString("X4").PadRight(10) + "Data:   " + Data.ToString("X4"));
                            Basebuf[Addr] = byte.Parse(LineParts[1]);
                        }
                    }
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
                return false;
            }
            Logger("[OK]");
            return true;
        }

        private void CompareBlock(byte[] OrgFile, byte[] ModFile, uint Start, uint End)
        {
            Logger(" [" + Start.ToString("X") + " - " + End.ToString("X")+ "] ");
            for (uint i = Start; i < End; i++)
            {
                if (OrgFile[i] != ModFile[i])
                {
                    PatchAddr.Add(i);
                    PatchData.Add(ModFile[i]);
                    txtResult.AppendText(i.ToString("X6") + ": " + OrgFile[i].ToString("X2") + " => " + ModFile[i].ToString("X2") + Environment.NewLine);
                }

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
                    MessageBox.Show("Files are different size, will not compare!");
                    return;
                }
                basefile = new BinFile[Segments.Count];
                modfile = new BinFile[Segments.Count];
                Basebuf = new byte[fsize];
                Modbuf = new byte[fsize2];
                GetFileInfo(txtBaseFile.Text, ref basefile, ref Basebuf);
                GetFileInfo(txtModifierFile.Text, ref modfile, ref Modbuf);

                labelBinSize.Text = fsize.ToString();

                if (Segments.Count == 0)
                {
                    Logger("No segments defined, comparing complete file");
                    CompareBlock(Basebuf, Modbuf, 0, (uint)fsize);
                }
                else
                {
                    bool[] Selections = GetSelections();
                    for (int Snr = 0; Snr < Segments.Count; Snr++)
                    {
                        if (Selections[Snr])
                        {
                            Logger("Comparing segment " + Segments[Snr].Name, false);
                            for (int p=0;p<basefile[Snr].SegmentBlocks.Count ;p++)
                            {
                                uint Start = basefile[Snr].SegmentBlocks[p].Start;
                                uint End = basefile[Snr].SegmentBlocks[p].End;
                                CompareBlock(Basebuf, Modbuf,Start,End );
                            }
                            Logger("");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }
         private void btnCompare_Click(object sender, EventArgs e)
        {
            labelXML.Text = Path.GetFileName(XMLFile);
            PatchData = new List<uint>();
            PatchAddr = new List<uint>();
            txtResult.Text = "";
            if (radioCreate.Checked) 
            {
                CompareBins();
                if (PatchAddr.Count>0)
                {
                    btnSave.Enabled = true;
                    btnSave.Text = "Save patch";
                    txtPatchName.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    txtPatchName.Enabled = false;
                }
            }
            else //Apply patch selected
            {
                if (ApplyPatch())
                {
                    btnSave.Enabled = true;
                    txtPatchName.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    txtPatchName.Enabled = false;
                }
            }

        }

        private void SavePatch()
        {
            try
            {
                if (txtPatchName.Text.Length < 1)
                {
                    Logger("Supply patch description");
                    return;
                }
                string PatchName = labelBinSize.Text + "-" + txtPatchName.Text;
                string PatchFile = SelectSaveFile("PACTH files (*.patch)|*.patch|ALL files(*.*) | *.*");
                if (PatchFile.Length < 1)
                    return;
                Logger("Saving to file: " + PatchFile);
                StreamWriter sw = new StreamWriter(PatchFile);
                sw.WriteLine(labelBinSize.Text);
                sw.WriteLine(txtPatchName.Text);
                sw.WriteLine(XMLFile);
                for (int i = 0; i < PatchAddr.Count; i++)
                {
                    sw.WriteLine(PatchAddr[i].ToString() + ":" + PatchData[i].ToString());
                }
                sw.Close();
                Logger("Patch saved");
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
            string BinFile = SelectSaveFile();
            Logger("Saving to file: " + BinFile);
            WriteSegmentToFile(BinFile, 0, (uint)Basebuf.Length, Basebuf);
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (radioCreate.Checked)
                SavePatch();
            else
                SaveBin();
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }
        private void txtModifierFile_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioCreate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCreate.Checked)
            {
                btnOrgFile.Text = "Original file";
                btnModFile.Text = "Modified file";
                btnCompare.Text = "Compare";
                btnSave.Text = "Save Patch";
                txtPatchName.Visible = true;
                labelDescr.Visible = true;
            }
            else
            {
                btnOrgFile.Text = "BIN file";
                btnModFile.Text = "Patch File";
                btnCompare.Text = "Apply Patch";
                btnSave.Text = "Save BIN";
                txtPatchName.Visible = false;
                labelDescr.Visible = false;
            }
        }


        private void btnSegments_Click(object sender, EventArgs e)
        {
            if (frmSS != null && frmSS.Visible)
            {
                frmSS.BringToFront();
                return;
            }
            frmSS = new frmSegmentSettings();
            //frmSS.Show();
            frmSS.InitMe();
            if (frmSS.ShowDialog() == DialogResult.OK)
            {
                addCheckBoxes();
            }
        }

        private void btnCheckSums_Click(object sender, EventArgs e)
        {
            FixCheckSums();

        }
        private void FixCheckSums()
        {
            try
            {
                Logger("Segments:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(S.Name);
                    if (S.CS1Method != CSMethod_None)
                    {
                        uint CS1 = 0;
                        uint CS1Calc = CalculateChecksum(Basebuf, basefile[i].CS1Address, basefile[i].CS1Blocks, basefile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, basefile[i].CS1Address.Bytes, S.CS1SwapBytes);
                        if (basefile[i].CS1Address.Bytes == 1)
                        {
                            CS1 = Basebuf[basefile[i].CS1Address.Address];
                        }
                        else if (basefile[i].CS1Address.Bytes == 2)
                        { 
                            CS1 = BEToUint16(Basebuf, basefile[i].CS1Address.Address);
                        }
                        else if (basefile[i].CS1Address.Bytes == 4)
                        {
                            CS1 = BEToUint32(Basebuf, basefile[i].CS1Address.Address);
                        }
                        if (CS1 == CS1Calc)
                            Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                        else
                        {
                            if (basefile[i].CS1Address.Bytes == 0)
                            {
                                Logger(" Checksum 1: " + CS1Calc.ToString("X4") + " [Not saved]");
                            }
                            else { 
                                if (basefile[i].CS1Address.Bytes == 1)
                                    Basebuf[basefile[i].CS1Address.Address] = (byte)CS1Calc;
                                else if (basefile[i].CS1Address.Bytes == 2)
                                {
                                    Basebuf[basefile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF00) >> 8);
                                    Basebuf[basefile[i].CS1Address.Address + 1] = (byte)(CS1Calc & 0xFF);
                                }
                                else if (basefile[i].CS1Address.Bytes == 4)
                                {
                                    Basebuf[basefile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF000000) >> 24);
                                    Basebuf[basefile[i].CS1Address.Address + 1] = (byte)((CS1Calc & 0xFF0000) >> 16);
                                    Basebuf[basefile[i].CS1Address.Address + 2] = (byte)((CS1Calc & 0xFF00) >> 8);
                                    Basebuf[basefile[i].CS1Address.Address + 3] = (byte)(CS1Calc & 0xFF);

                                }
                                Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                            }
                        }
                    }

                    if (S.CS2Method != CSMethod_None)
                    {
                        uint CS2 = 0;
                        uint CS2Calc = CalculateChecksum(Basebuf, basefile[i].CS2Address, basefile[i].CS2Blocks, basefile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, basefile[i].CS2Address.Bytes,S.CS2SwapBytes);
                            if (basefile[i].CS2Address.Bytes == 1)
                            {
                                CS2 = Basebuf[basefile[i].CS2Address.Address];
                            }
                            else if (basefile[i].CS2Address.Bytes == 2)
                            {
                                CS2 = BEToUint16(Basebuf, basefile[i].CS2Address.Address);
                            }
                            else if (basefile[i].CS2Address.Bytes == 4)
                            {
                                CS2 = BEToUint32(Basebuf, basefile[i].CS2Address.Address);
                            }
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                            else
                            {
                                if (basefile[i].CS2Address.Bytes == 0)
                                {
                                    Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    if (basefile[i].CS2Address.Bytes == 1)
                                        Basebuf[basefile[i].CS2Address.Address] = (byte)CS2Calc;
                                    else if (basefile[i].CS2Address.Bytes == 2)
                                    {
                                        Basebuf[basefile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        Basebuf[basefile[i].CS2Address.Address + 1] = (byte)(CS2Calc & 0xFF);
                                    }
                                    else if (basefile[i].CS2Address.Bytes == 4)
                                    {
                                        Basebuf[basefile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF000000) >> 24);
                                        Basebuf[basefile[i].CS2Address.Address + 1] = (byte)((CS2Calc & 0xFF0000) >> 16);
                                        Basebuf[basefile[i].CS2Address.Address + 2] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        Basebuf[basefile[i].CS2Address.Address + 3] = (byte)(CS2Calc & 0xFF);

                                    }
                                    Logger(" Checksum 2: " + CS2.ToString("X") + " => " + CS2Calc.ToString("X4") + " [Fixed]");
                                }
                            }
                    }



                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

    }
}
