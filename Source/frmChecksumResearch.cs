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
        ChecksumSearcher.ChecksumSearchSettings settings;
        private List<CsUtilMethod> csUtilMethods;
        private List<CkSearchResult> SearchResults;

        private void frmChecksumResearch_Load(object sender, EventArgs e)
        {
            csUtilMethods = new List<CsUtilMethod>();
            foreach (CSMethod csm in Enum.GetValues(typeof(CSMethod)))
            {
                if (csm != CSMethod.None && csm != CSMethod.Unknown)
                {
                    CsUtilMethod cum = new CsUtilMethod(csm);
                    cum.CsBytes = CSBytes._1;
                    if (csm == CSMethod.crc16 )
                        cum.CsBytes = CSBytes._2;
                    else if (csm == CSMethod.crc32 || csm == CSMethod.Wordsum || csm == CSMethod.Dwordsum)
                        cum.CsBytes = CSBytes._4;
                    if (csm == CSMethod.BoschInv)
                    {
                        cum.CsBytes = CSBytes._8;
                        cum.UseBoschAddresses = true;
                    }
                    csUtilMethods.Add(cum);
                }
            }
            dataGridCsUtilMethods.DataSource = csUtilMethods;
            dataGridCsUtilMethods.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridCsUtilMethods.DataError += DataGridCsUtilMethods_DataError;
            UseComboBoxForEnums(dataGridCsUtilMethods);

        }

        private void DataGridCsUtilMethods_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void radioCsUtilRangeExact_CheckedChanged(object sender, EventArgs e)
        {
            txtExclude.Enabled = radioCsUtilRangeExact.Checked;
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

        private void GetSearchResults(ChecksumSearcher searcher)
        {
            List<CkSearchResult> cresults = searcher.GetResults(chkCSUtilFilterZero.Checked);
            if (cresults.Count > 0)
            {
                SearchResults.AddRange(cresults);
                foreach (CkSearchResult result in cresults)
                {
                    Logger("Range: " + result.Start.ToString("X8") + " - " + result.End.ToString("X8") + ", Address: " + result.CsAddress.ToString("X8") + ", result: " + result.Cheksum.ToString("X") + Environment.NewLine);
                }
            }

        }
        private void csUtilWaitForSearcher(ChecksumSearcher searcher)
        {
            int x = 0;
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
                    richChkData.Text = "Queue: " + searcher.GetQueueSize().ToString() + "/" + searcher.QueueStartSize.ToString();
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
            GetSearchResults(searcher);
        }

        List<int> GetSelectedComplements()
        {
            List<int> selectedComplements = new List<int>();
            if (radioCSUtilComplement0.Checked)
                selectedComplements.Add(0);
            if (radioCSUtilComplement1.Checked)
                selectedComplements.Add(1);
            if (radioCSUtilComplement2.Checked)
                selectedComplements.Add(2);
            if (radioCsUtilComplementAll.Checked)
            {
                for (int c = 0; c <= 2; c++)
                {
                    selectedComplements.Add(c);
                }
            }
            return selectedComplements;
        }

        private void CsUtilSearchCsRange()
        {
            try
            {
                uint minRangeLen = (uint)numCsUtilMinRangeLength.Value;
                List<int> selectedComplements = GetSelectedComplements();
                //uint csAddr = uint.MaxValue;
                List<Block> csAddressBlocks = new List<Block>();
                List<uint> csAddresses = new List<uint>();
                BoschInvAddresses = null;
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
                List<Block> limitBlocks;
                uint start = 0;
                uint max = frmpatcher.basefile.fsize;
                if (!radioCsUtilRangeUnknown.Checked && !string.IsNullOrEmpty(txtChecksumRange.Text) && ParseAddress(txtChecksumRange.Text, frmpatcher.basefile, out limitBlocks))
                {
                    start = limitBlocks[0].Start;
                    max = limitBlocks.LastOrDefault().End;
                }

                int threadCount = (int)numCsUtilThreads.Value;
                ChecksumSearcher searcher = new ChecksumSearcher();
                if (!searcher.LoadCkLibrary())
                {
                    throw new Exception("DLL load failure");
                }
                for (int r = 0; r < dataGridCsUtilMethods.Rows.Count; r++)
                {
                    CsUtilMethod cum = (CsUtilMethod)dataGridCsUtilMethods.Rows[r].DataBoundItem;
                    CSMethod csm = cum.ChecksumMethod();
                    if (cum.Enable)
                    {
                        foreach (int complement in selectedComplements)
                        {
                            Logger("Method: " + csm.ToString() + ", Complement: " + complement.ToString());
                            if (csm == CSMethod.BoschInv && cum.UseBoschAddresses)
                            {
                                if (BoschInvAddresses == null)
                                    SearchBoschInv();

                                if (stopChecksumResearch) return;
                                searcher.StartCkCalc(frmpatcher.basefile.buf, start, max, minRangeLen, csm, complement, (int)cum.CsBytes, 
                                    chkCsMSB.Checked, chkCsUtilSwapBytes.Checked,cum.SkipCsAddress, threadCount,
                                    (UInt64)numCsUtilInitValue.Value, BoschInvAddresses.ToArray());
                                csUtilWaitForSearcher(searcher);
                            }
                            else
                            {
                                if (stopChecksumResearch) return;
                                searcher.StartCkCalc(frmpatcher.basefile.buf, start, max, minRangeLen, csm, complement, (int)cum.CsBytes, 
                                    chkCsMSB.Checked, chkCsUtilSwapBytes.Checked,cum.SkipCsAddress, threadCount, 
                                    (UInt64)numCsUtilInitValue.Value, csAddresses.ToArray());
                                csUtilWaitForSearcher(searcher);
                            }
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
        private UInt64 csUtilCalcCS(uint csAddress, List<Block> blocks)
        {
            try
            {
                List<int> selectedComplements = GetSelectedComplements();
                List<Block> excludes = new List<Block>();
                if (txtExclude.Text.Length > 0)
                {
                    ParseAddress(txtExclude.Text, frmpatcher.basefile, out excludes);
                }
                AddressData csAddr;
                csAddr.Address = csAddress;
                csAddr.Name = "CS";
                csAddr.Type = 0;
                BoschInvAddresses = null;
                foreach (CsUtilMethod cum in csUtilMethods)
                {
                    CSMethod csm = cum.ChecksumMethod();
                    if (cum.Enable)
                    {
                        if (csm == CSMethod.BoschInv && cum.UseBoschAddresses)
                        {
                            if (BoschInvAddresses == null)
                                SearchBoschInv();
                            foreach (uint csA in BoschInvAddresses)
                            {
                                UInt64 oldVal = csUtilReadOldvalue(csA, (ushort)cum.CsBytes, chkCsMSB.Checked);
                                csAddr.Address = csA;
                                csAddress = csA;
                                foreach (int complement in selectedComplements)
                                {
                                    csAddr.Bytes = (ushort)cum.CsBytes;
                                    //Logger("Method: " + csm.ToString() + ", Complement: " + complement.ToString());
                                    UInt64 csCalc = CalculateChecksum(chkCsMSB.Checked, frmpatcher.basefile.buf, csAddr, blocks, excludes, csm, (short)complement, csAddr.Bytes, chkCsUtilSwapBytes.Checked, true, (UInt64)numCsUtilInitValue.Value);
                                    if (csCalc == oldVal)
                                    {
                                        LoggerBold("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X") + " [Match]");
                                        CkSearchResult csr = new CkSearchResult(blocks[0].Start, blocks.LastOrDefault().End,csA, csCalc);
                                        SearchResults.Add(csr);
                                    }
                                    else if (!chkCSUtilMatchOnly.Checked)
                                    {
                                        Logger("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
                                    }
                                }

                            }
                        }
                        else
                        {
                            UInt64 oldVal = csUtilReadOldvalue(csAddress, (ushort)cum.CsBytes, chkCsMSB.Checked);
                            foreach (int complement in selectedComplements)
                            {
                                csAddr.Bytes = (ushort)cum.CsBytes;
                                //Logger("Method: " + csm.ToString() + ", Complement: " + complement.ToString());
                                UInt64 csCalc = CalculateChecksum(chkCsMSB.Checked, frmpatcher.basefile.buf, csAddr, blocks, excludes, csm, (short)complement, csAddr.Bytes, chkCsUtilSwapBytes.Checked, true, (UInt64)numCsUtilInitValue.Value);
                                if (csCalc == oldVal)
                                {
                                    LoggerBold("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X") + " [Match]");
                                    CkSearchResult csr = new CkSearchResult(blocks[0].Start, blocks.LastOrDefault().End, csAddress, csCalc);
                                    SearchResults.Add(csr);
                                }
                                else if (!chkCSUtilMatchOnly.Checked)
                                {
                                    Logger("Address: " + csAddress.ToString("X8") + ", Method: " + csm.ToString() + ", Complement: " + complement.ToString() + ", result: " + csCalc.ToString("X"));
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
            return 0;
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

                uint csAddr = uint.MaxValue;
                List<Block> csaBlocks;
                SearchResults = new List<CkSearchResult>();

                if (!radioCsUtilRangeExact.Checked)
                {
                    CsUtilSearchCsRange();
                }
                else
                {
                    List<Block> rangeBlocks;
                    bool MSB = chkCsMSB.Checked;
                    ParseAddress(txtChecksumRange.Text, frmpatcher.basefile, out rangeBlocks);
                    if (!string.IsNullOrEmpty(txtCSAddr.Text) && (txtCSAddr.Text.Contains(",") || txtCSAddr.Text.Contains("-")))
                    {
                        if (ParseAddress(txtCSAddr.Text, frmpatcher.basefile, out csaBlocks))
                        {
                            foreach (Block block in csaBlocks)
                            {
                                for (csAddr = block.Start; csAddr <= block.End; csAddr++)
                                {
                                    if (stopChecksumResearch) return;
                                    if (csAddr < uint.MaxValue)
                                    {
                                        Logger("Checksum address: " + csAddr.ToString("X8"));
                                    }
                                    csUtilCalcCS(csAddr, rangeBlocks);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtCSAddr.Text))
                            HexToUint(txtCSAddr.Text, out csAddr);

                        if (radioCsUtilCsAfterRange.Checked)
                        {
                            csAddr = rangeBlocks.LastOrDefault().End + 1;
                        }
                        csUtilCalcCS(csAddr, rangeBlocks);
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
            Application.DoEvents();
            ChecksumResearch();
            btnStop.Enabled = false;
        }
        private void ShowChkData()
        {
            uint chkAddr = 0;
            if (!HexToUint(txtCSAddr.Text, out chkAddr))
                return;

            try
            {
                int seg = frmpatcher.basefile.GetSegmentNumber(chkAddr);
                richChkData.Text = "";
                uint segStartAddr = 0;
                uint segEndAddr = frmpatcher.basefile.fsize;
                uint rStart = uint.MaxValue;
                uint rEnd = 0;

                CsUtilMethod cum = csUtilMethods.Where(X => X.Enable).FirstOrDefault();
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
                CSMethod method = CSMethod.Bytesum;
                AddressData csAddr;
                csAddr.Address = uint.MaxValue;
                csAddr.Bytes = 2;
                csAddr.Name = "CS";
                csAddr.Type = AddressDataType.Int;
                ushort csBytes = 1;
                int selCount = 0;
                for (int i=0;i<csUtilMethods.Count;i++)
                {
                    if (csUtilMethods[i].Enable)
                    {
                        if (selCount > 0)
                        {
                            LoggerBold("Select only one method");
                            return;
                        }
                        selCount++;
                        method = csUtilMethods[i].ChecksumMethod();
                        csBytes = (ushort)csUtilMethods[i].CsBytes;
                    }
                }
                if (radioCsUtilComplementAll.Checked)
                {
                    LoggerBold("Select only one complement value");
                    return;
                }
                short complement = 0;
                if (radioCSUtilComplement1.Checked)
                    complement = 1;
                else if (radioCSUtilComplement2.Checked)
                    complement = 2;
                uint csA;
                if (HexToUint(txtCSAddr.Text, out csA))
                    csAddr.Address = csA;
                List<Block> blocks;
                ParseAddress(txtChecksumRange.Text, frmpatcher.basefile, out blocks);
                List<Block> excblocks;
                ParseAddress(txtExclude.Text, frmpatcher.basefile, out excblocks);

                UInt64 csCalc = CalculateChecksum(chkCsMSB.Checked, frmpatcher.basefile.buf, csAddr, blocks, excblocks, method, complement, csBytes, chkCsUtilSwapBytes.Checked, true, (UInt64)numCsUtilInitValue.Value);

                UInt64 oldVal = csUtilReadOldvalue(csA,csBytes,chkCsMSB.Checked );
                switch(csBytes)
                {
                    case 1:
                        frmpatcher.basefile.buf[csA] = (byte)csCalc;
                        break;
                    case 2:
                        frmpatcher.basefile.SaveUshort(csA, (ushort)csCalc);
                        break;
                    case 4:
                        frmpatcher.basefile.SaveUint32(csA, (uint)csCalc);
                        break;
                    case 8:
                        frmpatcher.basefile.SaveUint64(csA, csCalc);
                        break;
                }
                ShowChkData();
                Logger("Checksum: " + SegmentInfo.CsToString(oldVal, (int)csBytes, method, chkCsMSB.Checked) + " => " + SegmentInfo.CsToString(csCalc, csBytes, method, chkCsMSB.Checked) + " [Fixed]");
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
            chkCSUtilFilterZero.Enabled = chkCSUtilMatchOnly.Checked;
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
            csUtilMethods = settings.Methods;
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
            else
                radioCsUtilCsAfterRange.Checked = true;
            switch(settings.Complement)
            {
                case 0:
                    radioCSUtilComplement0.Checked = true;
                    break;
                case 1:
                    radioCSUtilComplement1.Checked = true;
                    break;
                case 2:
                    radioCSUtilComplement2.Checked = true;
                    break;
                default:
                    radioCsUtilComplementAll.Checked = true;
                    break;
            }
            chkCsMSB.Checked = settings.MSB;
            chkCsUtilSwapBytes.Checked = settings.SwapBytes;
            numCsUtilInitValue.Value = settings.InitialValue;
            numCsUtilThreads.Value = settings.Threads;
            dataGridCsUtilMethods.DataSource = null;
            dataGridCsUtilMethods.DataSource = csUtilMethods;
            UseComboBoxForEnums(dataGridCsUtilMethods);
        }
        private ChecksumSearcher.ChecksumSearchSettings GetSettings()
        {
            ChecksumSearcher.ChecksumSearchSettings cSettings = new ChecksumSearcher.ChecksumSearchSettings();
            cSettings.Methods = csUtilMethods;
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
            else
                cSettings.CSAddressType = ChecksumSearcher.RangeType.AfterRange;            
            if (radioCSUtilComplement0.Checked)
                cSettings.Complement = 0;
            else if (radioCSUtilComplement1.Checked)
                cSettings.Complement = 1;
            else if (radioCSUtilComplement2.Checked)
                cSettings.Complement = 2;
            else if (radioCsUtilComplementAll.Checked)
                cSettings.Complement = 99;
            cSettings.MSB = chkCsMSB.Checked;
            cSettings.SwapBytes = chkCsUtilSwapBytes.Checked;
            cSettings.InitialValue = (UInt64)numCsUtilInitValue.Value;
            cSettings.Threads = (uint)numCsUtilThreads.Value;
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
    }
}
