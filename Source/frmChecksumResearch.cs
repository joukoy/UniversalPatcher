using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.Threading;
using System.IO;
using static UniversalPatcher.ChecksumSearcher;

namespace UniversalPatcher
{
    public partial class frmChecksumResearch : Form
    {
        public frmChecksumResearch()
        {
            InitializeComponent();
        }

        //public PcmFile frmpatcher.basefile;
        private List<uint> BoschInvAddresses;
        private bool stopChecksumResearch = false;
        ChecksumSearchSettings settings;
        private BindingList<CsUtilMethod> csUtilMethods;
        BindingSource bindingsource = new BindingSource();
        private List<CkSearchResult> SearchResults;
        private List<UInt64> FilterValues;

        private void frmChecksumResearch_Load(object sender, EventArgs e)
        {
            csUtilMethods = new BindingList<CsUtilMethod>();
            csUtilMethods.Add(new CsUtilMethod());
            //bindingsource.DataSource = csUtilMethods;
            dataGridCsUtilMethods.DataSource = csUtilMethods;
            dataGridCsUtilMethods.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridCsUtilMethods.DataError += DataGridCsUtilMethods_DataError;
            UseComboBoxForEnums(dataGridCsUtilMethods);
            dataGridCsUtilMethods.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridCsUtilMethods.SelectionChanged += DataGridCsUtilMethods_SelectionChanged;
            string settingsFile = Path.Combine(Application.StartupPath, "ChecksumSearch", "Settings.XML");
            if (File.Exists(settingsFile))
            {
                settings = ChecksumSearcher.LoadSettings(settingsFile);
            }
            if (settings != null)
            {
                ApplySettings();
                RefreshCsMethods();
            }
        }

        private void DataGridCsUtilMethods_SelectionChanged(object sender, EventArgs e)
        {
            ShowChkData();
        }

        private void DataGridCsUtilMethods_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void radioCsUtilRangeExact_CheckedChanged(object sender, EventArgs e)
        {
            txtExclude.Enabled = radioCsUtilRangeExact.Checked;
            trackBar1.Visible = !radioCsUtilRangeExact.Checked;
        }
        private void SearchBoschInv()
        {
            try
            {
                BoschInvAddresses = new List<uint>();
                Logger("Searching Bosch Inv checksum addresses...");
                for (uint a = 0; a < (frmpatcher.basefile.fsize - 7); a++)
                {
                    uint dWord1 = frmpatcher.basefile.ReadUInt32(a);
                    uint dWord2 = frmpatcher.basefile.ReadUInt32(a + 4);
                    if (dWord1 == ~dWord2)
                    {
                        if (!chkCsUtilFilter.Checked || dWord1.ToString("X8").Replace("F", "").Replace("0", "") != "")
                        {
                            Logger("Address: " + a.ToString("X8") + ": " + dWord1.ToString("X8") + " " + dWord2.ToString("X8"));
                            BoschInvAddresses.Add(a);
                        }
                    }
                }
                Logger("Done. Added: " + BoschInvAddresses.Count.ToString() + " addresses to list");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
        }

        private bool GetSearchResults(ChecksumSearcher searcher)
        {
            List<CkSearchResult> cresults = searcher.GetResults(FilterValues);
            if (cresults.Count > 0)
            {
                SearchResults.AddRange(cresults);
                if (chkCsUtilLogResults.Checked)
                {
                    foreach (CkSearchResult result in cresults)
                    {
                        if (stopChecksumResearch)
                            return true;
                        if (radioCsUtilCSValue.Checked)
                            Logger("Range: " + result.Start.ToString("X8") + " - " + result.End.ToString("X8") + ", result: " + result.Cheksum.ToString("X") + Environment.NewLine);
                        else
                            Logger("Range: " + result.Start.ToString("X8") + " - " + result.End.ToString("X8") + ", Address: " + result.CsAddress + ", result: " + result.Cheksum.ToString("X") + Environment.NewLine);
                        Application.DoEvents();
                    }
                }
            }
            if (cresults.Count > 0)
                return true;
            else
                return false;
        }
        private void csUtilWaitForSearcher(ChecksumSearcher searcher)
        {
            int x = 0;
            richChkData.Clear();
            richChkData.SelectionColor = Color.Black;
            while (searcher.isRunning())
            {
                if (stopChecksumResearch)
                {
                    searcher.Stop();
                }
                GetSearchResults(searcher);
                x++;
                if (x >= 10)
                {
                    x = 0;
                    richChkData.Text = "Queue: " + searcher.GetQueueSize().ToString() + "/" + searcher.QueueStartSize.ToString() +
                        Environment.NewLine + "Results: " + SearchResults.Count.ToString();
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
            while(GetSearchResults(searcher));
        }

        List<byte> GetSelectedComplements(CsUtilMethod cum)
        {
            List<byte> selectedComplements = new List<byte>();
            if (cum.Complement0)
                selectedComplements.Add(0);
            if (cum.Complement1)
                selectedComplements.Add(1);
            if (cum.Complement2)
                selectedComplements.Add(2);
            return selectedComplements;
        }

        private void CsUtilSearchCsRange()
        {
            try
            {
                List<Block> csAddressBlocks = new List<Block>();
                List<uint> csAddresses = new List<uint>();
                BoschInvAddresses = null;
                string csVals = "";
                if (radioCsUtilCSValue.Checked)
                    csVals = txtCSAddr.Text;
                if (radioCsUtilCsAfterRange.Checked)
                {
                    Block b = new Block();
                    b.Start = uint.MaxValue;
                    b.End = uint.MaxValue;
                    csAddressBlocks.Add(b);
                }
                else if (!string.IsNullOrEmpty(txtCSAddr.Text))
                {
                    ParseAddress(txtCSAddr.Text, frmpatcher.basefile, out csAddressBlocks);
                }
                foreach (Block block in csAddressBlocks)
                {
                    for (UInt64 csA = block.Start; csA <= block.End; csA++)
                    {
                        csAddresses.Add((uint)csA);
                    }
                }

                ChecksumSearcher searcher = new ChecksumSearcher();
                if (!searcher.LoadCkLibrary())
                {
                    throw new Exception("DLL load failure");
                }
                ChecksumSearchSettings searchSettings = GetSettings();
                foreach (CsUtilMethod cum in csUtilMethods)
                {
                    CSMethod csm = cum.Method;
                    foreach (bool MSB in new[] { true, false })
                    {
                        if (MSB == true && cum.MSB == false)
                        {
                            continue;
                        }
                        if (MSB == false && cum.LSB == false) 
                        {
                            continue;
                        }
                        searchSettings.MSB = MSB;
                        Logger("Method: " + csm.ToString() + ", MSB:" + MSB.ToString());
                        if (csm == CSMethod.BoschInv && cum.UseBoschAddresses)
                        {
                            if (BoschInvAddresses == null)
                                SearchBoschInv();

                            if (stopChecksumResearch) return;
                            searcher.StartCkCalc(frmpatcher.basefile.buf, cum, searchSettings, BoschInvAddresses.ToArray());
                            csUtilWaitForSearcher(searcher);
                        }
                        else
                        {
                            if (stopChecksumResearch) return;
                            searcher.StartCkCalc(frmpatcher.basefile.buf, cum, searchSettings, csAddresses.ToArray());
                            csUtilWaitForSearcher(searcher);
                        }
                    }
                }
                richChkData.Text = "Done";
                searcher.FreeCkLibrary();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
        }
        private UInt64 csUtilReadOldvalue(uint csAddress, ushort csBytes, bool MSB)
        {
            if (csBytes == 1)
                return frmpatcher.basefile.buf[csAddress];
            else if (csBytes == 2)
                return ReadUint16(frmpatcher.basefile.buf, csAddress, MSB);
            else if (csBytes == 4)
                return ReadUint32(frmpatcher.basefile.buf, csAddress, MSB);
            else if (csBytes == 8)
                return ReadUint64(frmpatcher.basefile.buf, csAddress, MSB);

            return UInt64.MaxValue;
        }
        private void csUtilCalcCS()
        {
            try
            {
                List<Block> excludes = new List<Block>();
                if (txtExclude.Text.Length > 0)
                {
                    ParseAddress(txtExclude.Text, frmpatcher.basefile, out excludes);
                }

                List<Block> rangeBlocks;
                ParseAddress(txtChecksumRange.Text, frmpatcher.basefile, out rangeBlocks);

                List<Block> csaBlocks;
                List<uint> csAddresses = new List<uint>();
                if (!string.IsNullOrEmpty(txtCSAddr.Text) && (txtCSAddr.Text.Contains(",") || txtCSAddr.Text.Contains("-")))
                {
                    uint csA = uint.MaxValue;
                    if (ParseAddress(txtCSAddr.Text, frmpatcher.basefile, out csaBlocks))
                    {
                        foreach (Block block in csaBlocks)
                        {
                            for (csA = block.Start; csA <= block.End; csA++)
                            {
                                if (stopChecksumResearch) return;
                                if (csA < uint.MaxValue)
                                {
                                    csAddresses.Add(csA);
                                }
                            }
                        }
                    }
                }
                else
                {
                    uint csA = uint.MaxValue;
                    if (!string.IsNullOrEmpty(txtCSAddr.Text))
                        HexToUint(txtCSAddr.Text, out csA);
                    if (radioCsUtilCsAfterRange.Checked)
                    {
                        csA = rangeBlocks.LastOrDefault().End + 1;
                    }
                    csAddresses.Add(csA);
                }


                AddressData csAddr;
                csAddr.Name = "CS";
                csAddr.Type = 0;
                BoschInvAddresses = null;
                foreach (CsUtilMethod cum in csUtilMethods)
                {
                    CSMethod csm = cum.Method;
                    List<byte> selectedComplements = GetSelectedComplements(cum);
                    List<uint> cumCsAddresses;
                    if (csm == CSMethod.BoschInv && cum.UseBoschAddresses)
                    {
                        if (BoschInvAddresses == null)
                            SearchBoschInv();
                        cumCsAddresses = BoschInvAddresses;
                    }
                    else
                    {
                        cumCsAddresses = csAddresses;
                    }
                    foreach (uint csA in cumCsAddresses)
                    {
                        csAddr.Address = csA;
                        uint csAddress = csA;
                        foreach (byte complement in selectedComplements)
                        {
                            csAddr.Bytes = (ushort)cum.CsBytes;
                            foreach (bool MSB in new[] { true, false })
                            {
                                if (MSB && cum.MSB == false)
                                {
                                    continue;
                                }
                                if (!MSB && cum.LSB == false)
                                {
                                    continue;
                                }

                                UInt64 oldVal = csUtilReadOldvalue(csA, (ushort)cum.CsBytes, MSB);
                                UInt64 csCalc = CalculateChecksum(MSB, frmpatcher.basefile.buf, csAddr, rangeBlocks, excludes,
                                            csm, (short)complement, csAddr.Bytes, false, true, cum.InitialValue);
                                UInt64 csCalcSwap = SwapBytes(csCalc, 8);

                                if (csCalc == oldVal && cum.NoSwapBytes && !FilterValues.Contains(csCalc))
                                {
                                    if (chkCsUtilLogResults.Checked)
                                        LoggerBold("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ", No byteswap, MSB: "+MSB.ToString()+", result: " + csCalc.ToString("X") + " [Match]");
                                    CkSearchResult csr = new CkSearchResult(rangeBlocks[0].Start, rangeBlocks.LastOrDefault().End, csA, csCalc, radioCsUtilCSValue.Checked, csm, complement, false, cum.Polynomial(), cum.Xor);
                                    SearchResults.Add(csr);
                                }
                                else if (csCalcSwap == oldVal && cum.SwapBytes && !FilterValues.Contains(csCalcSwap))
                                {
                                    if (chkCsUtilLogResults.Checked)
                                        LoggerBold("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ", Byteswap,  MSB: " + MSB.ToString() + ", result: " + csCalc.ToString("X") + " [Match]");
                                    CkSearchResult csr = new CkSearchResult(rangeBlocks[0].Start, rangeBlocks.LastOrDefault().End, csA, csCalcSwap, radioCsUtilCSValue.Checked, csm, complement, true, cum.Polynomial(), cum.Xor);
                                    SearchResults.Add(csr);
                                }
                                else if (chkCsUtilLogResults.Checked && !chkCSUtilMatchOnly.Checked)
                                {
                                    Logger("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ",  MSB: " + MSB.ToString() + ", result: " + csCalc.ToString("X"));
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
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
            return;
        }
        private void ChecksumResearch()
        {
            try
            {
                if (frmpatcher.basefile.FileName == null)
                {
                    Logger("No BIN file selected");
                    return;
                }
                Logger("Checksum research:");

                SearchResults = new List<CkSearchResult>();
                FilterValues = new List<ulong>();
                string[] fParts = txtFilterValues.Text.Split(',');
                foreach(string fPart in fParts)
                {
                    if (HexToUint64(fPart,out ulong x))
                    {
                        FilterValues.Add(x);
                    }
                }

                if (!radioCsUtilRangeExact.Checked)
                {
                    CsUtilSearchCsRange();
                }
                else
                {
                    csUtilCalcCS();
                }
                Logger("Found " + SearchResults.Count.ToString() + " matches");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
            Logger("Done");
        }



        private void btnCsutilSearchBosch_Click(object sender, EventArgs e)
        {
            SearchBoschInv();
        }

        private void btnTestChecksum_Click(object sender, EventArgs e)
        {
            stopChecksumResearch = false;
            btnStop.Enabled = true;
            btnTestChecksum.Enabled = false;
            Application.DoEvents();
            ChecksumResearch();
            btnStop.Enabled = false;
            btnTestChecksum.Enabled = true;
        }
        private void ShowChkData()
        {
            uint chkAddr = 0;
            if (!HexToUint(txtCSAddr.Text, out chkAddr))
                return;
            if (radioCsUtilCSValue.Checked)
                return;
            try
            {
                int seg = frmpatcher.basefile.GetSegmentNumber(chkAddr);
                richChkData.Text = "";
                uint segStartAddr = 0;
                uint segEndAddr = frmpatcher.basefile.fsize;
                uint rStart = uint.MaxValue;
                uint rEnd = 0;

                int row = dataGridCsUtilMethods.SelectedCells[0].RowIndex;
                CsUtilMethod cum = (CsUtilMethod)dataGridCsUtilMethods.Rows[row].DataBoundItem;
                if (cum == null)
                    return;
                if (txtChecksumRange.Text.Contains("-"))
                {
                    string[] rParts = txtChecksumRange.Text.Split('-');
                    if (rParts.Length == 2)
                    {
                        HexToUint(rParts[0], out rStart);
                        HexToUint(rParts[1], out rEnd);
                    }
                }

                if (seg > -1)
                {
                    segStartAddr = frmpatcher.basefile.segmentAddressDatas[seg].SegmentBlocks[0].Start;
                    segEndAddr = frmpatcher.basefile.segmentAddressDatas[seg].SegmentBlocks[frmpatcher.basefile.segmentAddressDatas[seg].SegmentBlocks.Count - 1].End;
                }
                uint start = segStartAddr;
                if ((int)(chkAddr - 4) > 0)
                    start = chkAddr - 4;
                uint prevSegAddr = uint.MaxValue;
                uint nextSegAddr = uint.MaxValue;
                for (uint a = start; a < frmpatcher.basefile.fsize && a < (chkAddr + 10); a++)
                {
                    if (a >= chkAddr && a < (chkAddr + (uint)cum.CsBytes))
                    {
                        richChkData.SelectionColor = Color.Red;
                    }
                    else if (a >= segStartAddr && a <= segEndAddr)
                    {
                        richChkData.SelectionColor = Color.Black;
                    }
                    else if (a < segStartAddr)
                    {
                        richChkData.SelectionColor = Color.LightBlue;
                        prevSegAddr = a;
                    }
                    else
                    {
                        richChkData.SelectionColor = Color.LightGreen;
                        nextSegAddr = a;
                    }
                    if (a >= rStart && a <= rEnd)
                        richChkData.SelectionFont = new Font(richChkData.SelectionFont, FontStyle.Underline);
                    else
                        richChkData.SelectionFont = new Font(richChkData.SelectionFont, FontStyle.Regular);
                    if (a == segEndAddr || a == (segStartAddr - 1))
                        richChkData.AppendText(frmpatcher.basefile.buf[a].ToString("X2") + "|");
                    else
                        richChkData.AppendText(frmpatcher.basefile.buf[a].ToString("X2") + " ");
                }

                if (rEnd > (chkAddr + 8))
                {
                    richChkData.AppendText("... ");
                    for (uint a = rEnd - 4; a < frmpatcher.basefile.fsize && a < (rEnd + 4); a++)
                    {
                        if (a >= chkAddr && a < (chkAddr + (uint)cum.CsBytes))
                        {
                            richChkData.SelectionColor = Color.Red;
                        }
                        else if (a >= segStartAddr && a <= segEndAddr)
                        {
                            richChkData.SelectionColor = Color.Black;
                        }
                        else if (a < segStartAddr)
                        {
                            richChkData.SelectionColor = Color.LightBlue;
                            prevSegAddr = a;
                        }
                        else
                        {
                            richChkData.SelectionColor = Color.LightGreen;
                            nextSegAddr = a;
                        }
                        if (a >= rStart && a <= rEnd)
                            richChkData.SelectionFont = new Font(richChkData.SelectionFont, FontStyle.Underline);
                        else
                            richChkData.SelectionFont = new Font(richChkData.SelectionFont, FontStyle.Regular);
                        if (a == segEndAddr || a == (segStartAddr - 1))
                            richChkData.AppendText(frmpatcher.basefile.buf[a].ToString("X2") + "|");
                        else
                            richChkData.AppendText(frmpatcher.basefile.buf[a].ToString("X2") + " ");
                    }
                }

                richChkData.SelectionColor = Color.Black;
                richChkData.SelectionFont = new Font(richChkData.SelectionFont, FontStyle.Underline);
                richChkData.AppendText(Environment.NewLine + "Underlined:selected range");
                richChkData.SelectionFont = new Font(richChkData.SelectionFont, FontStyle.Regular);
                if (seg > -1)
                    richChkData.AppendText(Environment.NewLine + "Black:" + frmpatcher.basefile.Segments[seg].Name);
                string prevSegment = frmpatcher.basefile.GetSegmentName(prevSegAddr);
                if (prevSegment != "")
                {
                    richChkData.SelectionColor = Color.LightBlue;
                    richChkData.AppendText(Environment.NewLine + "Lightblue:" + prevSegment);
                }
                string nextSegment = frmpatcher.basefile.GetSegmentName(nextSegAddr);
                if (nextSegment != "")
                {
                    richChkData.SelectionColor = Color.LightGreen;
                    richChkData.AppendText(Environment.NewLine + "LightGreen:" + nextSegment);
                }
                richChkData.SelectionColor = Color.Red;
                richChkData.AppendText(Environment.NewLine + "Red:Selected bytes");

                richChkData.SelectionColor = Color.Black;

            }
            catch { };

        }

        private void btnCsUtilFix_Click(object sender, EventArgs e)
        {
            try
            {

                if (csUtilMethods.Count != 1)
                {                 
                    LoggerBold("Select only one method");
                    return;
                }
                CsUtilMethod cum = csUtilMethods[0];

                short complement = 0;
                if (cum.Complement1)
                    complement++;
                if (cum.Complement2)
                    complement += 2;
                if (complement > 2 || (cum.Complement0 && complement > 0))
                {
                    LoggerBold("Select only one complement value");
                    return;
                }
                AddressData csAddr;
                csAddr.Address = uint.MaxValue;
                csAddr.Bytes = (ushort)cum.CsBytes;
                csAddr.Type = AddressDataType.Int;
                csAddr.Name = "CS Address";
                uint csA;
                if (HexToUint(txtCSAddr.Text, out csA))
                    csAddr.Address = csA;
                List<Block> blocks;
                ParseAddress(txtChecksumRange.Text, frmpatcher.basefile, out blocks);
                List<Block> excblocks;
                ParseAddress(txtExclude.Text, frmpatcher.basefile, out excblocks);

                UInt64 csCalc = CalculateChecksum(cum.MSB, frmpatcher.basefile.buf, csAddr, blocks, excblocks, cum.Method, complement, (ushort)cum.CsBytes, cum.SwapBytes, true, cum.InitialValue);

                UInt64 oldVal = csUtilReadOldvalue(csA,(ushort)cum.CsBytes,cum.MSB );
                switch(cum.CsBytes)
                {
                    case CSBytes._1:
                        frmpatcher.basefile.buf[csA] = (byte)csCalc;
                        break;
                    case CSBytes._2:
                        frmpatcher.basefile.SaveUshort(csA, (ushort)csCalc);
                        break;
                    case CSBytes._4:
                        frmpatcher.basefile.SaveUint32(csA, (uint)csCalc);
                        break;
                    case CSBytes._8:
                        frmpatcher.basefile.SaveUint64(csA, csCalc);
                        break;
                }
                ShowChkData();
                Logger("Checksum: " + SegmentInfo.CsToString(oldVal, (int)cum.CsBytes, cum.Method, cum.MSB) + " => " + SegmentInfo.CsToString(csCalc, (int)cum.CsBytes, cum.Method, cum.MSB) + " [Fixed]");
                Logger("You can save BIN file now");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }

        }

        private void txtCSAddr_TextChanged(object sender, EventArgs e)
        {
            ShowChkData();
        }

        private void chkCSUtilMatchOnly_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioCsUtilRangeUnknown_CheckedChanged(object sender, EventArgs e)
        {
            txtChecksumRange.Enabled = !radioCsUtilRangeUnknown.Checked;
        }

        private void radioCsUtilCsAfterRange_CheckedChanged(object sender, EventArgs e)
        {
            txtCSAddr.Enabled = !radioCsUtilCsAfterRange.Checked;
        }

        private void ApplySettings()
        {
            csUtilMethods.Clear();
            foreach(CsUtilMethod cs in settings.Methods)
                csUtilMethods.Add (cs);
            txtChecksumRange.Text = settings.CalcRange;
            if (settings.searchRangeType == ChecksumSearcher.RangeType.Exact)
                radioCsUtilRangeExact.Checked = true;
            else if (settings.searchRangeType == ChecksumSearcher.RangeType.SearchRange)
                radioCsUtilRangeLimit.Checked = true;
            else
                radioCsUtilRangeUnknown.Checked = true;
            txtExclude.Text = settings.Exclude;
            txtCSAddr.Text = settings.CSAddress;
            if (settings.CSAddressType == ChecksumSearcher.RangeType.Exact)
                radioCsUtilCsExact.Checked = true;
            else if (settings.CSAddressType == RangeType.UseValue)
                radioCsUtilCSValue.Checked = true;
            else
                radioCsUtilCsAfterRange.Checked = true;
            chkCsUtilLogResults.Checked = settings.ShowResults;            
            txtFilterValues.Text = settings.FilterValues;
            numCsUtilThreads.Value = settings.Threads;
            trackBar1.Value = (int)settings.MinRangeLen;
            dataGridCsUtilMethods.DataSource = null;
            dataGridCsUtilMethods.DataSource = csUtilMethods;
            UseComboBoxForEnums(dataGridCsUtilMethods);
        }
        private ChecksumSearchSettings GetSettings()
        {
            ChecksumSearchSettings cSettings = new ChecksumSearcher.ChecksumSearchSettings();
            cSettings.Methods = csUtilMethods.ToList();
            cSettings.CalcRange = txtChecksumRange.Text;
            if (radioCsUtilRangeExact.Checked)
                cSettings.searchRangeType = ChecksumSearcher.RangeType.Exact;
            else if (radioCsUtilRangeLimit.Checked)
                cSettings.searchRangeType = ChecksumSearcher.RangeType.SearchRange;
            else
                cSettings.searchRangeType = ChecksumSearcher.RangeType.All;
            cSettings.Exclude = txtExclude.Text;
            cSettings.CSAddress= txtCSAddr.Text;
            if (radioCsUtilCsExact.Checked)
                cSettings.CSAddressType = ChecksumSearcher.RangeType.Exact;
            else if (radioCsUtilCSValue.Checked)
                cSettings.CSAddressType = RangeType.UseValue;
            else
                cSettings.CSAddressType = ChecksumSearcher.RangeType.AfterRange;
            cSettings.ShowResults = chkCsUtilLogResults.Checked;
            cSettings.FilterValues = txtFilterValues.Text;
            cSettings.Threads = (uint)numCsUtilThreads.Value;
            cSettings.MinRangeLen = (uint)trackBar1.Value;

            return cSettings;
        }
        private void openSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string defFile = Path.Combine(Application.StartupPath, "ChecksumSearch", "Settings.XML");
            string fName = SelectFile("Select config file", XmlFilter, defFile);
            if (string.IsNullOrEmpty(fName))
                return;
            settings = ChecksumSearcher.LoadSettings(fName);
            if (settings != null)
            {
                ApplySettings();
            }
        }

        private void saveSettingsAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string defFile = Path.Combine(Application.StartupPath, "ChecksumSearch","Settings.XML");
            string fName = SelectSaveFile( XmlFilter, defFile);
            if (string.IsNullOrEmpty(fName))
                return;
            settings = GetSettings();
            ChecksumSearcher.SaveSettings(settings, fName);
        }

        private void saveResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChecksumResults fcr = new frmChecksumResults();
            fcr.Show();
            fcr.LoadResults(SearchResults, frmpatcher.basefile.OS);
        }

        private void loadCalculationRangeFromResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string defFile = Path.Combine(Application.StartupPath, "ChecksumSearch", frmpatcher.basefile.OS + ".XML");
                string fName = SelectFile("Select result file", XmlFilter, defFile);
                if (string.IsNullOrEmpty(fName))
                    return;
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CkSearchResult>));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                List<CkSearchResult> cResults = (List<CkSearchResult>)reader.Deserialize(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                foreach (CkSearchResult cr in cResults)
                {
                    sb.Append(cr.Start.ToString("X") + "-" + cr.End.ToString("X") + ",");
                }
                txtChecksumRange.Text = sb.ToString().Trim(',');
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopChecksumResearch = true;
            btnStop.Enabled = false;
        }

        private void radioCsUtilCSValue_CheckedChanged(object sender, EventArgs e)
        {
            if (radioCsUtilCSValue.Checked)
            {
                labelCsAddress.Text = "Value:";
            }
            else
            {
                labelCsAddress.Text = "Address:";
            }
        }

        private void chkCsUtilLogResults_CheckedChanged(object sender, EventArgs e)
        {
            chkCSUtilMatchOnly.Enabled = chkCsUtilLogResults.Checked;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            labelMinRange.Text = trackBar1.Value.ToString() + "%";
        }

        private void RefreshCsMethods()
        {
            dataGridCsUtilMethods.Update();
            dataGridCsUtilMethods.Refresh();
            this.Refresh();
            Application.DoEvents();
            dataGridCsUtilMethods.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            CsUtilMethod cum = new CsUtilMethod();
            frmPropertyEditor fpe = new frmPropertyEditor();
            fpe.LoadObject(cum);
            if (fpe.ShowDialog() == DialogResult.OK)
            {
                csUtilMethods.Add(cum);
                RefreshCsMethods();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                CsUtilMethod cum;
                if (csUtilMethods.Count == 0)
                {
                    cum = new CsUtilMethod();
                }
                else
                {
                    if (dataGridCsUtilMethods.SelectedCells.Count == 0)
                    {
                        Logger("No method selected");
                        return;
                    }
                    int r = dataGridCsUtilMethods.SelectedCells[0].RowIndex;
                    cum = (CsUtilMethod)dataGridCsUtilMethods.Rows[r].DataBoundItem;
                }
                frmPropertyEditor fpe = new frmPropertyEditor();
                fpe.LoadObject(cum);
                if (fpe.ShowDialog() == DialogResult.OK)
                {
                    if (csUtilMethods.Count == 0)
                    {
                        csUtilMethods.Add(cum);
                    }
                    RefreshCsMethods();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int r = dataGridCsUtilMethods.SelectedCells[0].RowIndex;
                csUtilMethods.RemoveAt(r);
                RefreshCsMethods();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }

        }

        private void loadCalculationRangeFromResultsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                frmChecksumResults fcr = new frmChecksumResults();
                fcr.LoadResults(SearchResults, frmpatcher.basefile.OS);
                fcr.btnOK.Enabled = true;
                fcr.btnOK.Visible = true;
                if (fcr.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CkSearchResult cr in fcr.SelectedResults)
                    {
                        sb.Append(cr.Start.ToString("X") + "-" + cr.End.ToString("X") + ",");
                    }
                    txtChecksumRange.Text = sb.ToString().Trim(',');
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("frmChecksumResearch, line " + line + ": " + ex.Message);
            }
        }

    }
}
