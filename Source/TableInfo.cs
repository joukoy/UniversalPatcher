using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public class TableCell
    {
        public TableCell()
        {

        }
        public TableCell(TableData td, TableInfo tinfo, uint addr, int column, int row, string rowheader, string colheader)
        {
            this.td = td;
            this.tableInfo = tinfo;
            this.addr = addr;
            this.Column = column;
            this.Row = row;
            this.RowhHeader = rowheader;
            this.ColHeader = colheader;
            lastRawBytes = new byte[td.ElementSize()];
            origRawBytes = new byte[td.ElementSize()];
            Match = false;
            if ((addr + td.ElementSize()) >= tableInfo.pcm.buf.Length)
            {
                Debug.WriteLine("Table: " + td.TableName + ", address 0x" + addr.ToString("X") + " out of range");
                lastValue = (double)0;
                return;
            }
            Array.Copy(tableInfo.pcm.buf, addr, origRawBytes, 0, td.ElementSize());
            Array.Copy(tableInfo.pcm.buf, addr, lastRawBytes, 0, td.ElementSize());
            lastValue = GetValue(tableInfo.pcm.buf, addr, td, 0, tableInfo.pcm);

        }
        public int Column { get; set; }
        public int Row { get; set; }
        public string RowhHeader { get; set; }
        public string ColHeader { get; set; }
        public uint addr { get; set; }
        //public uint tableId { get; set; }
        public TableData td { get; set; }
        public object lastValue { get; internal set; }
        public UInt64 origRawValue 
        { 
            get 
            {
                UInt64 retVal = 0;
                if (tableInfo.pcm.platformConfig.MSB)
                {
                    for (int b = 0; b < origRawBytes.Length; b++)
                    {
                        retVal = retVal << 8 | (byte)origRawBytes[b];
                    }
                }
                else
                {
                    for (int b = origRawBytes.Length - 1; b >= 0; b--)
                    {
                        retVal = retVal << 8 | (byte)origRawBytes[b];
                    }
                }
                return retVal;
            } 
        }
        public UInt64 lastRawValue
        {
            get
            {
                UInt64 retVal = 0;
                if (tableInfo.pcm.platformConfig.MSB)
                {
                    for (int b = 0; b < lastRawBytes.Length; b++)
                    {
                        retVal = retVal << 8 | (byte)lastRawBytes[b];
                    }
                }
                else
                {
                    for (int b = lastRawBytes.Length - 1; b >= 0; b--)
                    {
                        retVal = retVal << 8 | (byte)lastRawBytes[b];
                    }
                }
                return retVal;
            }
        }
        //public double lastRawValue  { get; set; }
        public byte[] lastRawBytes { get; set; }
        public byte[] origRawBytes { get; internal set; }
        public double cmpValue { get; set; }
        public TableInfo tableInfo { get; set; }
        //For peek/infobox:
        public string ValueText { get; set; }
        public bool Match { get; set; }

        public object origValue 
        { 
            get 
            { 
                if (td.OutputType == OutDataType.Bitmap)
                {
                    byte mask = (byte)(0x01 << (Row % 8));
                    if ((tableInfo.pcm.buf[addr] & mask) == mask)
                        return 1;
                    else
                        return 0;
                }
                return GetValue(tableInfo.pcm.buf, addr, td, 0, tableInfo.compareFile.pcm); 
            } 
        }
        public bool MSB 
        { 
            get 
            {
                if (td.ByteOrder == Byte_Order.LSB)
                    return false;
                else if (td.ByteOrder == Byte_Order.MSB)
                    return true;
                else
                    return tableInfo.pcm.platformConfig.MSB; 
            } 
        }

        public TableCell ShallowCopy()
        {
            return (TableCell)this.MemberwiseClone();
        }

        public double CalculatedValue(double rawValue)
        {
            try
            {
                string mathStr = td.Math.ToLower();
                //Debug.WriteLine("Last raw value: " + lastRawValue + ", Last value: " + (double)lastRawValue);

                if (mathStr.Contains("table:"))
                {
                    mathStr = ReadConversionTable(mathStr, tableInfo.pcm);
                }
                if (mathStr.Contains("raw:"))
                {
                    mathStr = ReadConversionRaw(mathStr, tableInfo.pcm);
                }

                return parser.Parse(mathStr.Replace("x", rawValue.ToString()));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return double.NaN;
            }
        }

        public bool SetValue(double val,bool isRawValue = false)
        {
            bool retVal = false;    //Return true if value modified
            try
            {
                if (td.Math.StartsWith("DTC"))
                {
                    int codeIndex = (int)(addr - td.addrInt);
                    switch (td.Math.Substring(4))
                    {
                        case "DTC_Enable":
                            tableInfo.pcm.dtcCodes[codeIndex].Status = (byte)val;
                            break;
                        case "MIL_Enable":
                            tableInfo.pcm.dtcCodes[codeIndex].MilStatus = (byte)val;
                            break;
                        case "Type":
                            tableInfo.pcm.dtcCodes[codeIndex].Type = (byte)val;
                            break;
                        default:
                            throw new Exception("Unknown Math: " + td.Math);
                    }
                    //SetDtcCode(ref tableBuffer, tableInfo.compareFile.tableBufferOffset, codeIndex, tableInfo.pcm.dtcCodes[codeIndex], tableInfo.pcm);
                    lastValue = val;
                    return true;
                }
                if (td.OutputType == OutDataType.Flag && !string.IsNullOrEmpty(td.BitMask))
                {
                    bool flag = Convert.ToBoolean(val);
                    SetFlag(flag);
                    return true;
                }
                if (td.OutputType == OutDataType.Bitmap)
                {
                    bool flag = Convert.ToBoolean(val);
                    SetBitmap(flag);
                    return true;
                }
                double newRawValue;
                if (isRawValue)
                {
                    newRawValue = val;
                }
                else
                {
                    string mathStr = td.Math.ToLower();
                    if (mathStr.Contains("table:"))
                    {
                        mathStr = ReadConversionTable(mathStr, tableInfo.pcm);
                    }
                    if (mathStr.Contains("raw:"))
                    {
                        mathStr = ReadConversionRaw(mathStr, tableInfo.pcm);
                    }

                    if (mathStr.Contains("&"))
                    {
                        string[] mParts = mathStr.Split('&');
                        if (mParts.Length == 2)
                        {
                            UInt64 mask = UInt64.Parse(mParts[1]);
                            newRawValue = (origRawValue & ~mask) + val;
                        }
                        else
                        {
                            throw new Exception("Only simple 'x & y' bitmask AND supported");
                        }
                    }
                    else if (mathStr.Contains("|"))
                    {
                        string[] mParts = mathStr.Split('|');
                        if (mParts.Length == 2)
                        {
                            UInt64 mask = UInt64.Parse(mParts[1]);
                            newRawValue = (origRawValue & ~mask) + val;
                        }
                        else
                        {
                            throw new Exception("Only simple 'x | y' bitmask OR supported");
                        }
                    }

                    else
                    {
                        newRawValue = savingMath.GetSavingValue(mathStr, td, val);
                    }
                    Debug.WriteLine("Calculated raw value: " + newRawValue);
                }
                if (td.DataType != InDataType.FLOAT32 && td.DataType != InDataType.FLOAT64)
                    newRawValue = Math.Round(newRawValue);

                double minRawVal = GetMinValue(td.DataType);
                double maxRawVal = GetMaxValue(td.DataType);

                if (newRawValue < minRawVal)
                {
                    newRawValue = minRawVal;
                    Debug.WriteLine("Too small value entered");
                }
                else if (newRawValue > maxRawVal)
                {
                    newRawValue = maxRawVal;
                    Debug.WriteLine("Too big value entered");
                }
                if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
                    lastRawBytes[0] = (byte)newRawValue;
                if (td.DataType == InDataType.SWORD)
                    SaveShort(lastRawBytes, 0, (short)newRawValue,MSB);
                if (td.DataType == InDataType.UWORD)
                    SaveUshort(lastRawBytes, 0, (ushort)newRawValue, MSB);
                if (td.DataType == InDataType.FLOAT32)
                    SaveFloat32(lastRawBytes, 0, (Single)newRawValue, MSB);
                if (td.DataType == InDataType.INT32)
                    SaveInt32(lastRawBytes, 0, (Int32)newRawValue, MSB);
                if (td.DataType == InDataType.UINT32)
                    SaveUint32(lastRawBytes, 0, (UInt32)newRawValue, MSB);
                if (td.DataType == InDataType.FLOAT64)
                    SaveFloat64(lastRawBytes, 0, newRawValue, MSB);
                if (td.DataType == InDataType.INT64)
                    SaveInt64(lastRawBytes, 0, (Int64)newRawValue, MSB);
                if (td.DataType == InDataType.UINT64)
                    SaveUint64(lastRawBytes, 0, (UInt64)newRawValue, MSB);
                if (newRawValue != lastRawValue)
                    retVal = true;
                lastValue = CalculatedValue(newRawValue);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger("Invalid value");
            }
            return retVal;
        }

        private void SetFlag(bool flag)
        {
            string maskStr = "FF";
            if (td.BitMask != null)
                maskStr = td.BitMask.Replace("0x", "");
            if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
            {
                byte mask = Convert.ToByte(maskStr, 16);
                byte newVal;
                if (flag)
                {
                    newVal = (byte)(lastRawBytes[0] | mask);
                    lastRawBytes[0] = newVal;
                }
                else
                {
                    mask = (byte)~mask;
                    newVal = (byte)(lastRawBytes[0] & mask);
                    lastRawBytes[0] = newVal;
                }
                lastValue = newVal;
            }
            else if (td.DataType == InDataType.SWORD || td.DataType == InDataType.UWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = ReadUint16(lastRawBytes, 0, MSB);
                ushort newVal;
                if (flag)
                {
                    newVal = (ushort)(curVal | mask);
                }
                else
                {
                    mask = (ushort)~mask;
                    newVal = (ushort)(curVal & mask);
                }
                SaveUshort(lastRawBytes, 0, newVal, MSB);
                lastValue = newVal;
            }
            else if (td.DataType == InDataType.INT32 || td.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = ReadUint32(lastRawBytes, 0, MSB);
                UInt32 newVal;
                if (flag)
                {
                    newVal = (UInt32)(curVal | mask);
                }
                else
                {
                    mask = ~mask;
                    newVal = (UInt32)(curVal & mask);
                }
                SaveUint32(lastRawBytes, 0, newVal, MSB);
                lastValue = newVal;
            }
            else if (td.DataType == InDataType.INT64 || td.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                UInt64 curVal = ReadUint64(lastRawBytes, 0, MSB);
                UInt64 newVal;
                if (flag)
                {
                    newVal = (UInt64)(curVal | mask);
                }
                else
                {
                    mask = ~mask;
                    newVal = (UInt64)(curVal & mask);
                }
                SaveUint64(lastRawBytes, 0, newVal, MSB);
                lastValue = newVal;
            }
        }
        private void SetBitmap(bool flag)
        {
            byte bit = (byte)(Row % 8);
            byte mask = (byte)(0x01 << bit );
            byte newVal;
            if (flag)
            {
                newVal = (byte)(lastRawBytes[0] | mask);
                lastRawBytes[0] = newVal;
                lastValue = 1;
            }
            else
            {
                mask = (byte)~mask;
                newVal = (byte)(lastRawBytes[0] & mask);
                lastRawBytes[0] = newVal;
                lastValue = 0;
            }
        }

    }

    public class TableInfo
    {
        public TableInfo(PcmFile _pcm, TableData _td)
        {
            pcm = _pcm;
            td = _td;
            tableCells = new List<TableCell>();
            MinVal = double.MaxValue;
            MaxVal = double.MinValue;
        }
        public PcmFile pcm { get; set; }
        //public uint tdId { get; set; }
        public TableData td { get; set; }
        public List<TableCell> tableCells { get; set; }
        public TableCell[,] tableCellArray { get; set; }
        public CompareFile compareFile { get; set; }
        //For coloring:
        public double MinVal { get; set; }
        public double MaxVal { get; set; }
        public int Rows { get; internal set; }
        public int Columns { get; internal set; }

        public bool isModified()
        {
            foreach (TableCell tCell in tableCells)
            {
                if (!tCell.origRawBytes.SequenceEqual(tCell.lastRawBytes))
                {
                    return true;
                }
            }
            return false;
        }
        public void SaveCellsToPcmBuffer()
        {
            foreach(TableCell tCell in tableCells)
            {
                if (td.Math.StartsWith("DTC"))
                {
                    int codeIndex = (int)(tCell.addr - td.addrInt);
                    SetDtcCode(ref pcm.buf, 0, codeIndex, pcm.dtcCodes[codeIndex], pcm);
                }
                else
                {
                    Array.Copy(tCell.lastRawBytes, 0, pcm.buf, tCell.addr, tCell.lastRawBytes.Length);
                }
            }
        }
        public void ParseTable(bool disableMultiTable, bool duplicateTableName, bool parseHeaders)
        {
            try
            {
                Rows = td.Rows;
                Columns = td.Columns;
                List<string> colHeaders = new List<string>();
                List<string> rowHeaders = new List<string>();

                if (td.TableName.ToLower().EndsWith(".data"))
                {
                    Rows = GetRowCountFromTable(td, pcm);
                    Columns = GetColumnsFromTable(td, pcm);
                }
                tableCellArray = new TableCell[Rows, Columns];

                if (parseHeaders)
                {
                    string[] cHeaders = td.ColumnHeaders.Split(',');
                    if (td.ColumnHeaders.ToLower().StartsWith("table:") || td.ColumnHeaders.ToLower().StartsWith("guid:"))
                    {
                        TableData headerTd = pcm.GetTdbyHeader(td.ColumnHeaders);
                        cHeaders = LoadHeaderFromTable(headerTd, td.Columns, pcm);
                    }

                    string[] rHeaders = td.RowHeaders.Split(',');
                    if (td.RowHeaders.ToLower().StartsWith("table:") || td.RowHeaders.ToLower().StartsWith("guid:"))
                    {
                        TableData headerTd = pcm.GetTdbyHeader(td.RowHeaders);
                        rHeaders = LoadHeaderFromTable(headerTd, td.Rows, pcm);
                    }
                    this.tableCellArray = new TableCell[Rows, Columns];
                    string RowPrefix = "";
                    string colPrefix = "";
                    if (!disableMultiTable)
                    {
                        string[] nParts = td.TableName.Split(new char[] { ']', '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nParts.Length > 1)
                        {
                            //"Real" multitable
                            string TableName = nParts[0];
                            if (nParts.Length == 2)
                            {
                                colPrefix = nParts[1].Trim();
                            }
                            if (nParts.Length == 3)
                            {
                                colPrefix = nParts[1].Trim();
                                RowPrefix = nParts[2].Trim();
                            }
                            if (nParts.Length > 3)
                            {
                                colPrefix = nParts[1].Trim();
                                for (int i = 1; i < 4; i++)
                                    RowPrefix += "[" + nParts[i].Trim() + "]";
                            }
                            colPrefix += " ";
                            RowPrefix += " ";
                        }
                    }

                    for (int c = 0; c < td.Columns; c++)
                    {
                        string cHdr = "";
                        if (cHeaders.Length >= c + 1 && cHeaders[c].Length > 0)
                            cHdr = cHeaders[c];
                        else
                            cHdr = td.Units + " " + c.ToString();
                        if (duplicateTableName)
                            cHdr += " [" + td.guid.ToString().Substring(0, 4) + "]";
                        if (colHeaders.Contains(colPrefix + cHdr))
                            colHeaders.Add(colPrefix + cHdr + c.ToString());
                        else
                            colHeaders.Add(colPrefix + cHdr);
                    }
                    for (int r = 0; r < td.Rows; r++)
                    {
                        string rHdr = "";
                        if (rHeaders.Length >= r + 1 && rHeaders[r].Length > 0)
                            rHdr = rHeaders[r];
                        else
                            rHdr = "(" + r.ToString() + ")";
                        if (rowHeaders.Contains(RowPrefix + rHdr))
                            rowHeaders.Add(RowPrefix + rHdr + r.ToString());
                        else
                            rowHeaders.Add(RowPrefix + rHdr);
                    }
                }
                else
                {
                    for (int r = 0; r < Rows; r++)
                    {
                        rowHeaders.Add("");
                    }
                    for (int c = 0; c < Columns; c++)
                    {
                        colHeaders.Add("");
                    }
                }

                if (td.OutputType == OutDataType.Bitmap)
                {
                    td.DataType = InDataType.UBYTE;
                }
                uint addr = td.StartAddress();
                uint step = (uint)td.EffectiveElementStride();
                int minorbytes = td.EffectiveMinorStride();

                if (td.RowMajor)
                {
                    for (int r = 0; r < td.Rows; r++)
                    {
                        for (int c = 0; c < td.Columns; c++)
                        {
                            TableCell tc = new TableCell(td, this, addr, c, r, rowHeaders[r], colHeaders[c]);
                            tableCells.Add(tc);
                            tableCellArray[r, c] = tc;
                            addr += step;
                        }
                        addr += (uint)minorbytes;
                    }
                }
                else
                {
                    // not rowmajor
                    for (int c = 0; c < td.Columns; c++)
                    {
                        for (int r = 0; r < td.Rows; r++)
                        {
                            TableCell tc = new TableCell(td, this, addr, c, r, rowHeaders[r], colHeaders[c]);
                            addr += step;
                            this.tableCells.Add(tc);
                            tableCellArray[r, c] = tc;
                        }
                        addr += (uint)minorbytes;
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
                LoggerBold("Error, TableInfo line " + line + ": " + ex.Message);
            }
        }
        public string GetMinMaxString()
        {
            string minMax = " [";
            if (td.Min > double.MinValue)
                minMax += " Min: " + td.Min.ToString();
            if (td.Max < double.MaxValue)
                minMax += " Max: " + td.Max.ToString();
            if (minMax == " [")
                minMax = "";
            else
                minMax += "] ";
            return minMax;
        }
        public string Parse1dTablePeekValue()
        {
            try
            {
                Rows = 1;
                Columns = 1;
                tableCellArray = new TableCell[1, 1];
                TableCell tc = new TableCell(td, this, td.StartAddress(), 0, 0, td.RowHeaders, td.ColumnHeaders);
                //double curVal = GetValue(pcm.buf, (uint)td.StartAddress(), td, 0, pcm);
                double curVal = (double)tc.lastValue;
                UInt64 rawVal = tc.origRawValue;
                string valTxt = curVal.ToString();
                string unitTxt = " " + td.Units;
                string maskTxt = "";
                string minMax = " [";
                if (td.Min > double.MinValue)
                    minMax += " Min: " + td.Min.ToString();
                if (td.Max < double.MaxValue)
                    minMax += " Max: " + td.Max.ToString();
                if (minMax == " [")
                    minMax = "";
                else
                    minMax += "] ";
                TableValueType vt = td.ValueType();
                if (vt == TableValueType.bitmask)
                {
                    unitTxt = "";
                    UInt64 maskVal = Convert.ToUInt64(td.BitMask.Replace("0x", ""), 16);
                    if ((rawVal & maskVal) == maskVal)
                        valTxt = "Set";
                    else
                        valTxt = "Unset";
                    string maskBits = Convert.ToString((Int64)maskVal, 2);
                    int bit = -1;
                    for (int i = 0; 1 <= maskBits.Length; i++)
                    {
                        if (((maskVal & (UInt64)(1 << i)) != 0))
                        {
                            bit = i + 1;
                            break;
                        }
                    }
                    if (bit > -1)
                    {
                        string rawBinVal = Convert.ToString((Int64)rawVal, 2);
                        rawBinVal = rawBinVal.PadLeft(GetBits(td.DataType), '0');
                        maskTxt = " [" + rawBinVal + "], bit $" + bit.ToString();
                    }
                }
                else if (vt == TableValueType.boolean)
                {
                    unitTxt = ", Unset/Set";
                    if (curVal > 0)
                        valTxt = "Set, " + valTxt;
                    else
                        valTxt = "Unset, " + valTxt;
                }
                else if (vt == TableValueType.selection)
                {
                    Dictionary<double, string> possibleVals = ParseEnumHeaders(td.Values);
                    if (possibleVals.ContainsKey(curVal))
                        unitTxt = " (" + possibleVals[curVal] + ")";
                    else
                        unitTxt = " (Out of range)";
                }
                string formatStr = "X" + (GetElementSize(td.DataType) * 2).ToString();
                string rawTxt = "";
                switch (td.DataType)
                {
                    case InDataType.FLOAT32:
                        rawTxt = ((Int32)rawVal).ToString(formatStr);
                        break;
                    case InDataType.FLOAT64:
                        rawTxt = ((Int64)rawVal).ToString(formatStr);
                        break;
                    case InDataType.INT64:
                        rawTxt = ((Int64)rawVal).ToString(formatStr);
                        break;
                    case InDataType.INT32:
                        rawTxt = ((Int32)rawVal).ToString(formatStr);
                        break;
                    case InDataType.UINT64:
                        rawTxt = ((UInt64)rawVal).ToString(formatStr);
                        break;
                    case InDataType.UINT32:
                        rawTxt = ((UInt32)rawVal).ToString(formatStr);
                        break;
                    case InDataType.SWORD:
                        rawTxt = ((Int16)rawVal).ToString(formatStr);
                        break;
                    case InDataType.UWORD:
                        rawTxt = ((UInt16)rawVal).ToString(formatStr);
                        break;
                    case InDataType.SBYTE:
                        rawTxt = ((sbyte)rawVal).ToString(formatStr);
                        break;
                    case InDataType.UBYTE:
                        rawTxt = ((byte)rawVal).ToString(formatStr);
                        break;
                    default:
                        rawTxt = ((Int32)rawVal).ToString(formatStr);
                        break;
                }
                tc.ValueText = valTxt;
                tableCells.Add(tc);
                tableCellArray[0, 0] = tc;
                return unitTxt + "[" + rawTxt + "]" + minMax + maskTxt;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TableInfo line " + line + ": " + ex.Message);
            }
            return "";
        }
    }

    public class CompareFile
    {
        public CompareFile(PcmFile _pcm)
        {
            pcm = _pcm;
            NaviCurrent = pcm.NaviCurrent;
            tableInfos = new List<TableInfo>();
            filteredTables = new List<TableData>();
        }
        public PcmFile pcm { get; set; }
        public List<TableInfo> tableInfos { get; set; }
        public List<TableData> filteredTables { get; set; }
        public string fileLetter { get; set; }
        public int Rows { get; set; }   //How many rows (in multitable)
        public int Cols { get; set; }
        public int NaviCurrent { get; set; }    //For navigating in TableEditor, without moving PCM navi
        public bool Active { get; set; }
    }
}
