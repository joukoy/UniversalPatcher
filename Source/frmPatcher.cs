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

namespace UinversalPatcher
{
    public partial class FrmPatcher : Form
    {
        public FrmPatcher()
        {
            InitializeComponent();
        }

        private static List<uint> PatchData;
        private static List<uint> PatchAddr;
        byte[] buf;

        private void FrmPatcher_Load(object sender, EventArgs e)
        {
        }

        private void btnOrgFile_Click(object sender, EventArgs e)
        {
            string BinFile = SelectFile();
            if (BinFile.Length > 1)
            {
                txtBaseFile.Text = BinFile;
            }
        }

        private void btnModFile_Click(object sender, EventArgs e)
        {
            string BinFile;
            if (radioCreate.Checked)
                BinFile = SelectFile();
            else
                BinFile = SelectFile("PACTH files (*.patch)|*.patch|ALL files(*.*) | *.*");
            if (BinFile.Length > 1)
            {
                txtModifierFile.Text = BinFile;
            }

        }

        private bool ApplyPatch()
        {
            string line;

            try { 
                StreamReader sr = new StreamReader(txtModifierFile.Text);
                bool FirstLine = true;
                long fsize = new System.IO.FileInfo(txtBaseFile.Text).Length;
                labelBinSize.Text = fsize.ToString();
                buf = ReadBin(txtBaseFile.Text, 0, (uint)fsize);
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
                            if (Addr > buf.Length)
                                throw new FileLoadException(String.Format("Address {0} out of range!",  Addr.ToString("X4")));
                            if (Data > 0xff)
                                throw new FileLoadException(String.Format("Data {0} out of range!", Data.ToString("X4")));
                            //Apply patchrow:
                            Logger("Set address: ".PadRight(16) + Addr.ToString("X4").PadRight(10) + "Data:   " + Data.ToString("X4"));
                            buf[Addr] = byte.Parse(LineParts[1]);
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

        public void CompareBins()
        {
            try
            {
                long fsize = new System.IO.FileInfo(txtBaseFile.Text).Length;
                long fsize2 = new System.IO.FileInfo(txtModifierFile.Text).Length;
                if (fsize != fsize2)
                {
                    MessageBox.Show("Files are different size, will not compare!");
                    return;
                }

                byte[] OrgFile = ReadBin(txtBaseFile.Text, 0, (uint)fsize);
                byte[] ModFile = ReadBin(txtModifierFile.Text, 0, (uint)fsize);
                labelBinSize.Text = fsize.ToString();
                
                for (uint i = 0; i < fsize; i++)
                {
                    if (OrgFile[i] != ModFile[i])
                    {
                        PatchAddr.Add(i);
                        PatchData.Add(ModFile[i]);
                        txtResult.AppendText(i.ToString("X1") + ":" + OrgFile[i].ToString("X1") + "=>" + ModFile[i].ToString("X1") + Environment.NewLine);
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
            PatchData = new List<uint>();
            PatchAddr = new List<uint>();
            txtResult.Text = "";
            if (radioCreate.Checked) { 
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
                Logger("Saving to file: " + PatchFile);
                StreamWriter sw = new StreamWriter(PatchFile);
                sw.WriteLine(labelBinSize.Text);
                sw.WriteLine(txtPatchName.Text);
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
            WriteSegmentToFile(BinFile, 0, (uint)buf.Length, buf);
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
    }
}
