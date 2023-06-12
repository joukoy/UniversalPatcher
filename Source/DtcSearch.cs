using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Management.Instrumentation;
using static Helpers;

namespace UniversalPatcher
{
    public class DtcSearchConfig
    {
        public DtcSearchConfig()
        {
            MilTable = "opcode";
            CodeOffset = 1;
            CodeSteps = 4;
            StatusOffset = 1;
            MilOffset = 1;
            StatusSteps = 1;
            MilSteps = 1;
            ConditionalOffset = "";
            Linear = true;
            CodesPerStatus = 1;
        }
        public string XMLFile { get; set; }
        public string CodeSearch { get; set; }
        public string StatusSearch { get; set; }
        public string MilSearch { get; set; }
        public string MilTable { get; set; }
        public Int64 CodeOffset { get; set; }
        public int CodeSteps { get; set; }
        public Int64 StatusOffset { get; set; }
        public Int64 MilOffset { get; set; }
        public int StatusSteps { get; set; }
        public int MilSteps { get; set; }
        public string ConditionalOffset { get; set; }
        public string Values { get; set; }
        public bool Linear { get; set; }
        public int CodesPerStatus { get; set; }
        public string StatusMath { get; set; }

        public DtcSearchConfig ShallowCopy()
        {
            return (DtcSearchConfig)this.MemberwiseClone();
        }
    }

    public class DtcSearch
    {
        public DtcSearch()
        {

        }

        public static string DecodeDTC(string code)
        {
            switch (code[0])
            {
                case '0':
                    return "P0" + code.Substring(1);
                case '1':
                    return "P1" + code.Substring(1);
                case '2':
                    return "P2" + code.Substring(1);
                case '3':
                    return "P3" + code.Substring(1);
                case '4':
                    return "C0" + code.Substring(1);
                case '5':
                    return "C1" + code.Substring(1);
                case '6':
                    return "C2" + code.Substring(1);
                case '7':
                    return "C3" + code.Substring(1);
                case '8':
                    return "B0" + code.Substring(1);
                case '9':
                    return "B1" + code.Substring(1);
                case 'A':
                    return "B2" + code.Substring(1);
                case 'B':
                    return "B3" + code.Substring(1);
                case 'C':
                    return "U0" + code.Substring(1);
                case 'D':
                    return "U1" + code.Substring(1);
                case 'E':
                    return "U2" + code.Substring(1);
                case 'F':
                    return "U3" + code.Substring(1);
                default:
                    return "P" + code;
            }
        }

        public static string GetDtcDescription(string dtcCode)
        {
            string retVal = "";
            for (int o = 0; o < OBD2Codes.Count; o++)
            {
                if (dtcCode == OBD2Codes[o].Code)
                {
                    retVal = OBD2Codes[o].Description;
                    break;
                }
            }
            return retVal;
        }

        private List<DtcCode> SearchP10Dtc(PcmFile PCM, DtcSearchConfig dConfig, uint codeAddr)
        {
            List<DtcCode> retVal = new List<DtcCode>();
            try
            {
                byte prevMask = 0xFF;
                string header = "";
                ushort tableLen = 0;
                ushort lastStatusByte = 0;
                uint[] StatusTables = new uint[4];

                string searchStr = "14 30 41 b0 @ @ @ @ c4 13 67";
                //uint addr1 = SearchBytes(PCM, searchStr, 0, PCM.fsize) + 11;
                uint startAddr = 0;
                uint endAddr = PCM.fsize;
                uint addr1;  //GetAddrbySearchString(PCM, searchStr, ref startAddr, endAddr).Addr;
                for (int t = 0; t < 3; t++)
                {
                    //addr1 = SearchBytes(PCM, searchStr, addr1, PCM.fsize) + 11;
                    addr1 = GetAddrbySearchString(PCM, searchStr, ref startAddr, endAddr).Addr;
                    StatusTables[t] = addr1;
                }
                searchStr = "14 30 41 b0 @ @ @ @ c4 13 66";
                //addr1 = SearchBytes(PCM, searchStr, 0, PCM.fsize);
                startAddr = 0;
                addr1 = GetAddrbySearchString(PCM, searchStr, ref startAddr, endAddr).Addr;
                if (addr1 == uint.MaxValue)
                {
                    searchStr = "76 * 45 F9 @ @ @ @ 47 F8 * * 16";
                    startAddr = 0;
                    endAddr = PCM.fsize;
                    addr1 = GetAddrbySearchString(PCM, searchStr, ref startAddr, endAddr).Addr;
                    //addr1 = SearchBytes(PCM, searchStr, addr1, PCM.fsize) + 11;
                }
                if (addr1 < uint.MaxValue)
                {
                    StatusTables[3] = addr1;
                }

                //Status tables:
                // 0 & 2 = Enable / Disable Mil
                // 1 = TypeA/TypeB
                // 3 =  Enable/Disable DTC
                // 0 = status1, 1 = status2, 2 = status4, 3 = status3 (PCM internal handling?)

                //for (uint addr = (uint)(codeAddr + dConfig.CodeOffset); addr < PCM.fsize; addr += (uint)dConfig.CodeSteps)
                for (uint addr = codeAddr; addr < PCM.fsize; addr += (uint)dConfig.CodeSteps)
                {
                    DtcCode dtc = new DtcCode();
                    dtc.P10 = true;
                    dtc.codeAddrInt = addr;
                    dtc.Values = dConfig.Values;
                    dtc.StatusMath = "X";
                    dtc.codeInt = PCM.ReadUInt16(addr);
                    string codeTmp = dtc.codeInt.ToString("X4");
                    dtc.Code = DecodeDTC(codeTmp);
                    dtc.Description = GetDtcDescription(dtc.Code);
                    dtc.Values = "Enum: 0:Enabled, 1: Disabled";
                    dtc.StatusByte = PCM.buf[addr - 1];
                    dtc.StatusMask = PCM.buf[addr - 2];

                    dtc.milAddrInt = StatusTables[0] + dtc.StatusByte;
                    dtc.TypeAddrInt = StatusTables[1] + dtc.StatusByte;
                    dtc.milAddrInt2 = StatusTables[2] + dtc.StatusByte;
                    dtc.statusAddrInt = StatusTables[3] + dtc.StatusByte;

                    if (dtc.StatusByte > lastStatusByte)
                    {
                        lastStatusByte = dtc.StatusByte;
                    }
                    if (prevMask != 0xFF && dtc.StatusMask != 1)
                    {
                        if ((prevMask << 1) != dtc.StatusMask)
                        {
                            Debug.WriteLine("Mask not increasing");
                            break;
                        }
                    }
                    prevMask = dtc.StatusMask;
                    header += dtc.Code + ",";
                    if ((PCM.buf[StatusTables[0] + dtc.StatusByte] & dtc.StatusMask) == 0)
                        dtc.MilStatus = 0;
                    else
                        dtc.MilStatus = 1;

                    if ((PCM.buf[StatusTables[1] + dtc.StatusByte] & dtc.StatusMask) == 0)
                        dtc.Type = 0;
                    else
                        dtc.Type =1;

                    if ((PCM.buf[StatusTables[3] + dtc.StatusByte] & dtc.StatusMask) == 0)
                    {
                        dtc.Status = 1;
                        dtc.StatusTxt = "Enabled";
                    }
                    else
                    {
                        dtc.Status = 0;
                        dtc.StatusTxt = "Disabled";
                    }
                    retVal.Add(dtc);
                    tableLen++;
                }
                header = header.Trim(',');
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("DTC search, line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        public List<DtcCode> SearchDtc(PcmFile PCM, bool primary)
        {
            List<DtcCode> retVal = new List<DtcCode>();
            try
            {
                LoadOBD2Codes();
                //Search DTC codes:
                uint codeAddr = uint.MaxValue;
                string searchStr;
                int configIndex = 0;
                uint startAddr = 0;
                bool condOffsetCode = false;
                bool condOffsetStatus = false;
                bool condOffsetMil = false;
                bool signedOffsetCode = false;
                bool signedOffsetStatus = false;
                bool signedOffsetMil = false;
                uint statusAddr = uint.MaxValue;
                uint milAddr = uint.MaxValue;
                bool linear = true;
                int codePerStatus = 1;

                string cnfFile = PCM.configFile;
                if (primary)
                    PCM.dtcCombined = false;
                else
                    cnfFile += "-dtc2";


                for (configIndex = 0; configIndex < dtcSearchConfigs.Count; configIndex++)
                {
                    DtcSearchConfig dConfig = dtcSearchConfigs[configIndex];
                    linear = dConfig.Linear;
                    if (dConfig.CodesPerStatus > 0)
                        codePerStatus = dConfig.CodesPerStatus;

                    if (dConfig.XMLFile != null && cnfFile == dConfig.XMLFile.ToLower())
                    {
                        searchStr = dConfig.CodeSearch;
                        startAddr = 0;
                        condOffsetCode = false;

                        string[] condParts = dConfig.ConditionalOffset.Trim().Split(',');
                        foreach (string conPart in condParts)
                        {
                            switch(conPart.Trim())
                            {
                                case "code":
                                    condOffsetCode = true;
                                    break;
                                case "status":
                                    condOffsetStatus = true;
                                    break;
                                case "mil":
                                    condOffsetMil = true;
                                    break;
                                case "signed:code":
                                    signedOffsetCode = true;
                                    break;
                                case "signed:status":
                                    signedOffsetStatus = true;
                                    break;
                                case "signed:mil":
                                    signedOffsetMil = true;
                                    break;
                            }
                        }

                        codeAddr = GetAddrbySearchString(PCM, searchStr,ref startAddr,PCM.fsize, condOffsetCode, signedOffsetCode).Addr;
                        //Check if we found status table, too:
                        startAddr = 0;
                        statusAddr = GetAddrbySearchString(PCM, dConfig.StatusSearch, ref startAddr,PCM.fsize, condOffsetStatus, signedOffsetStatus).Addr;
                        if (codeAddr < uint.MaxValue) //If found
                            codeAddr = (uint)(codeAddr + dConfig.CodeOffset);
                        if (statusAddr < uint.MaxValue)
                            statusAddr = (uint)(statusAddr + dConfig.StatusOffset);
                        if (codeAddr < PCM.fsize && statusAddr < PCM.fsize)
                        {
                            Debug.WriteLine("Code search string: " + searchStr);
                            Debug.WriteLine("DTC code table address: " + codeAddr.ToString("X"));

                            if (!string.IsNullOrEmpty(dConfig.StatusMath) && dConfig.StatusMath.ToLower().Contains("p10"))
                            {
                                PCM.dtcValues = ParseDtcValues("0:Enabled,1:Disabled");
                                retVal = SearchP10Dtc(PCM, dConfig, codeAddr);
                                return retVal;
                            }

                            if (condOffsetStatus)
                            {
                                uint a = codeAddr;
                                int prevCode = PCM.ReadUInt16(a);
                                for (int x = 0; x < 30; x++)
                                {
                                    //Check if codes (30 first) are increasing
                                    a += (uint)dConfig.CodeSteps;
                                    int nextCode = PCM.ReadUInt16(a);
                                    if (nextCode <= prevCode)
                                    {
                                        //Not increasing, use offset
                                        codeAddr += 0x10000;
                                        break;
                                    }
                                    prevCode = PCM.ReadUInt16(a);
                                }
                            }
                            Debug.WriteLine("DTC status table address: " + statusAddr.ToString("X"));
                            break;
                        }
                        else
                        {
                            codeAddr = 0;
                        }

                    }
                }

                if (codeAddr == uint.MaxValue || codeAddr == 0)
                {
                    if (PCM.configFile == "e38" || PCM.configFile == "e67")
                    {
                        PCM.dtcCombined = true;
                        string vals = "Enum: 0:MIL and reporting off,1:Type A/no MIL,2:Type B/no MIL,3:Type C/no MIL,4:Not reported/MIL,5:Type A/MIL,6:Type B/MIL,7:Type C/MIL";
                        PCM.dtcValues = ParseDtcValues(vals.ToLower().Replace("enum: ", ""));
                        return SearchDtcE38(PCM);
                    }
                    else if (primary)
                    {
                        LoggerBold("DTC search: can't find DTC code table");
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }

                string values;
                if (!string.IsNullOrEmpty(dtcSearchConfigs[configIndex].Values))
                {
                    values = dtcSearchConfigs[configIndex].Values;
                }
                else if (dtcSearchConfigs[configIndex].MilTable == "combined")
                {
                    values = "Enum: 0:MIL and reporting off,1:Type A/no MIL,2:Type B/no MIL,3:Type C/no MIL,4:Not reported/MIL,5:Type A/MIL,6:Type B/MIL,7:Type C/MIL";
                }
                else
                {
                    values = "Enum: 0:1 Trip/immediately,1:2 Trips,2:Store only,3:Disabled"; ;
                }
                PCM.dtcValues = ParseDtcValues(values);

                //Read codes:
                bool dCodes = false;
                uint addr2 = statusAddr;
                uint codePerStatusaddr = statusAddr;
                uint codePerStatusCounter = 0;

                for (uint addr = codeAddr; addr < PCM.fsize; addr += (uint)dtcSearchConfigs[configIndex].CodeSteps)
                {
                    addr2 += (uint)dtcSearchConfigs[configIndex].StatusSteps;
                    DtcCode dtc = new DtcCode();
                    dtc.codeAddrInt = addr;
                    dtc.Values = dtcSearchConfigs[configIndex].Values;
                    dtc.StatusMath = dtcSearchConfigs[configIndex].StatusMath;
                    dtc.CodeAddr = addr.ToString("X8");
                    dtc.codeInt = PCM.ReadUInt16(addr);
                    string codeTmp = dtc.codeInt.ToString("X");
                    if (dtc.codeInt < 10 && retVal.Count > 10 && linear)
                            break;
                    if (dCodes && linear)
                    {
                        //There should be only D or E (U) codes after first D or E code
                        if (!codeTmp.StartsWith("D") && !codeTmp.StartsWith("E"))
                            break;
                    }
                    codeTmp = dtc.codeInt.ToString("X4");
/*                    if (codeTmp.StartsWith("6") || codeTmp.StartsWith("7") || codeTmp.StartsWith("8")
                        || codeTmp.StartsWith("9") || codeTmp.StartsWith("B") || codeTmp.StartsWith("F"))
                    {
                        Debug.WriteLine("DTC Code out of range: " + codeTmp);
                        break;
                    }
*/
                    dtc.Code = DecodeDTC(codeTmp);


                    byte statusByte;
                    dtc.statusAddrInt = codePerStatusaddr;
                    statusByte = PCM.buf[codePerStatusaddr];

                    if (!string.IsNullOrEmpty(dtcSearchConfigs[configIndex].StatusMath) && dtcSearchConfigs[configIndex].StatusMath.Contains("&"))
                    {
                        string[] sParts = dtcSearchConfigs[configIndex].StatusMath.Split('&');
                        if (sParts.Length == 2)
                        {
                            byte mask = byte.Parse(sParts[1]);
                            dtc.Status = (byte)(statusByte & mask);
                        }
                    }
                    else
                    {
                        dtc.Status = statusByte;
                    }
                    codePerStatusCounter++;
                    if (codePerStatusCounter >= codePerStatus)
                    {
                        codePerStatusCounter = 0;
                        codePerStatusaddr++;
                    }
                    if (PCM.dtcValues.ContainsKey(dtc.Status))
                    {
                        dtc.StatusTxt = PCM.dtcValues[dtc.Status].ToString();
                    }
                    else
                    {
                        Debug.WriteLine("DTC Status out of range: " + dtc.Status);
                        break;
                    }



                    if (codeTmp.StartsWith("D") || codeTmp.StartsWith("E")) 
                        dCodes = true;
                    //Find description for code:
                    dtc.Description = GetDtcDescription(dtc.Code);
                    retVal.Add(dtc);
                }

                //PCM.dtcCodes = PCM.dtcCodes.OrderBy(x => x.codeInt).ToList();
                List<uint> milAddrList = new List<uint>();

                if (dtcSearchConfigs[configIndex].MilTable == "afterstatus")
                {
                    milAddr = (uint)(statusAddr + retVal.Count + (uint)dtcSearchConfigs[configIndex].MilOffset);
                    if (PCM.configFile == "p01-p59" && PCM.buf[milAddr - 1] == 0xFF) milAddr++; //P59 hack: If there is FF before first byte, skip first byte 
                    milAddrList.Add(milAddr);
                }
                else if (dtcSearchConfigs[configIndex].MilTable == "combined")
                {
                    //Do nothing for now
                    milAddr = 0;
                    milAddrList.Add(milAddr);
                    PCM.dtcCombined = true;
                }
                else
                {
                    //Search MIL table
                    startAddr = 0;
                    for (int i = 0; i < 30; i++)
                    {
                        milAddr = GetAddrbySearchString(PCM, dtcSearchConfigs[configIndex].MilSearch, ref startAddr, PCM.fsize, condOffsetMil, signedOffsetMil).Addr;
                        if (milAddr < uint.MaxValue)
                        {

                            milAddr +=  (uint)dtcSearchConfigs[configIndex].MilOffset;
                            if (milAddr < PCM.fsize) //Hit
                            {
                                milAddrList.Add(milAddr);
                                //break;
                            }
                        }
                        else
                        {
                            //Not found
                            break;
                        }
                    }
                }


                if (milAddrList.Count > 1)
                {
                    //IF have multiple hits for search, use table which starts after FF, ends before FF
                    for (int m = 0; m < milAddrList.Count; m++)
                    {
                        Debug.WriteLine("MIL Start: " + (milAddrList[m] - 1).ToString("X"));
                        Debug.WriteLine("MIL End: " + (milAddrList[m] + retVal.Count).ToString("X"));
                        if (PCM.buf[milAddrList[m] - 2] == 0xFF && PCM.buf[milAddrList[m] + retVal.Count] == 0xFF)
                        {
                            milAddr = milAddrList[m];
                            break;
                        }
                    }
                    if (milAddr == uint.MaxValue)
                    {
                        //We didn't found it, use first from list
                        milAddr = milAddrList[0];
                    }
                }
                else milAddr = milAddrList[0];
                Debug.WriteLine("MIL: " + milAddr.ToString("X"));
                if (milAddr >= PCM.fsize)
                {
                    LoggerBold( "DTC search: MIL table address out of address range:" + milAddr.ToString("X8"));
                    return null;
                }

                //Read MIL status:
                int dtcNr = 0;
                uint addr3 = milAddr;
                codePerStatusaddr = statusAddr;
                codePerStatusCounter = 0;

                for (; dtcNr < retVal.Count; addr3 += (uint)dtcSearchConfigs[configIndex].MilSteps)
                {

                    DtcCode dtc = retVal[dtcNr];

                    if (PCM.dtcCombined)
                    {
                        if (dtc.Status > 4)
                            dtc.MilStatus = 1;
                        else
                            dtc.MilStatus = 0;
                    }
                    else
                    {
                        //Read MIL bytes:
                        dtc.milAddrInt = addr3;
                        dtc.MilAddr = addr3.ToString("X8");
                        dtc.MilStatus = PCM.buf[addr3];
                    }
                    retVal.RemoveAt(dtcNr);
                    retVal.Insert(dtcNr, dtc);
                    dtcNr++;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold( "DTC search, line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        //Search GM e38/e67 DTC codes
        public List<DtcCode> SearchDtcE38(PcmFile PCM)
        {
            List<DtcCode> retVal = new List<DtcCode>();
            try
            {
                if (PCM.OSSegment == -1)
                {
                    LoggerBold("DTC search: No OS segment??");
                    return null;
                }
                if (PCM.DiagSegment == -1)
                {
                    LoggerBold( "DTC search: No Diagnostic segment??");
                    return null;
                }

                //Get codes from OS segment:
                string searchStr = "3D 80 * * 39 8C * * 7D 7B DA 14 7C 8C 5A 2E 80 BD";
                uint opCodeAddr = uint.MaxValue;
                uint tableStart = 0;
                for (int b = 0; b < PCM.segmentAddressDatas[PCM.OSSegment].SegmentBlocks.Count; b++)
                {
                    opCodeAddr = SearchBytes(PCM, searchStr, PCM.segmentAddressDatas[PCM.OSSegment].SegmentBlocks[b].Start, PCM.segmentAddressDatas[PCM.OSSegment].SegmentBlocks[b].End);
                    if (opCodeAddr < uint.MaxValue)
                    {
                        ushort highBytes = PCM.ReadUInt16((uint)(opCodeAddr + 2));
                        ushort lowBytes = PCM.ReadUInt16((uint)(opCodeAddr + 6));
                        tableStart = (uint)(highBytes << 16 | lowBytes);
                        ushort tmp = (ushort)(opCodeAddr & 0xffff);
                        if (tmp > 0x5000) tableStart -= 0x10000; //Some kind of address offset 
                        bool dCodes = false;
                        for (uint addr = tableStart; addr < PCM.fsize; addr += 2)
                        {
                            DtcCode dtc = new DtcCode();
                            dtc.codeAddrInt = addr;
                            dtc.CodeAddr = addr.ToString("X8");
                            dtc.codeInt = PCM.ReadUInt16(addr);

                            string codeTmp = dtc.codeInt.ToString("X4");
                            if (dCodes && !codeTmp.StartsWith("D"))
                            {
                                break;
                            }
                            if (codeTmp.StartsWith("D"))
                            {
                                dCodes = true;
                            }
                            dtc.Code = DecodeDTC(codeTmp);
                            dtc.Description = GetDtcDescription(dtc.Code);
                            retVal.Add(dtc);
                        }
                        break;
                    }
                }

                int dtcCount = retVal.Count;

                //Search by opcode:
                opCodeAddr = SearchBytes(PCM, "3C A0 * * 38 A5 * * 7D 85 50", 0, PCM.fsize);
                if (opCodeAddr < uint.MaxValue)
                {
                    ushort highBytes = PCM.ReadUInt16((uint)(opCodeAddr + 2));
                    ushort lowBytes = PCM.ReadUInt16((uint)(opCodeAddr + 6));
                    tableStart = (uint)(highBytes << 16 | lowBytes);
                }

                if (tableStart > 0)
                {
                    int dtcNr = 0;
                    for (uint addr2 = tableStart; addr2 < tableStart + dtcCount; addr2++)
                    {
                        if (PCM.buf[addr2] == 0xFF)
                        {
                            return retVal;
                        }
                        DtcCode dtc = retVal[dtcNr];
                        dtc.statusAddrInt = addr2;
                        //dtc.StatusAddr = addr2.ToString("X8");
                        //dtc.Status = PCM.buf[addr2];
                        byte statusByte = PCM.buf[addr2];
                        if (statusByte > 3)
                            dtc.MilStatus = 1;
                        else
                            dtc.MilStatus = 0;
                        dtc.Status = statusByte;
                        dtc.StatusTxt = PCM.dtcValues[dtc.Status].ToString();
                        retVal.RemoveAt(dtcNr);
                        retVal.Insert(dtcNr, dtc);
                        dtcNr++;
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
                LoggerBold( "DTC search, line " + line + ": " + ex.Message);
                return null;
            }
            return retVal;
        }

    }

}
