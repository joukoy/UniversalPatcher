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
        }
        public int Column { get; set; }
        public int Row { get; set; }
        public string RowhHeader { get; set; }
        public string ColHeader { get; set; }
        public uint addr { get; set; }
        //public uint tableId { get; set; }
        public TableData td { get; set; }
        public object lastValue { get; set; }
        public double origRawValue {get {return GetRawValue(tableInfo.compareFile.pcm.buf, addr, td, 0,tableInfo.compareFile.pcm.platformConfig.MSB); }}
        public double lastRawValue  { get; set; }
        public byte[] lastRawBytes { get; set; }
        public double cmpValue { get; set; }
        public TableInfo tableInfo { get; set; }
        public object origValue 
        { 
            get 
            { 
                if (td.OutputType == OutDataType.Bitmap)
                {
                    byte mask = (byte)(0x01 << (Row % 8));
                    if ((tableInfo.compareFile.pcm.buf[addr] & mask) == mask)
                        return 1;
                    else
                        return 0;
                }
                return GetValue(tableInfo.compareFile.pcm.buf, addr, td, 0, tableInfo.compareFile.pcm); 
            } 
        }
        private bool MSB 
        { 
            get 
            {
                if (td.ByteOrder == Byte_Order.LSB)
                    return false;
                else if (td.ByteOrder == Byte_Order.MSB)
                    return true;
                else
                    return tableInfo.compareFile.pcm.platformConfig.MSB; 
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
                UInt32 bufAddr = addr - tableInfo.compareFile.tableBufferOffset;
                Debug.WriteLine("Last raw value: " + lastRawValue + ", Last value: " + (double)lastRawValue);

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

        public bool SaveValue(double val,bool isRawValue = false)
        {
            bool retVal = false;    //Return true if value modified
            try
            {
                byte[] tableBuffer = tableInfo.compareFile.buf;
                UInt32 bufAddr = addr - tableInfo.compareFile.tableBufferOffset;
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
                    SetDtcCode(ref tableBuffer, tableInfo.compareFile.tableBufferOffset, codeIndex, tableInfo.pcm.dtcCodes[codeIndex], tableInfo.pcm);
                    lastValue = val;
                    return true;
                }
                if (td.OutputType == OutDataType.Flag && !string.IsNullOrEmpty(td.BitMask))
                {
                    bool flag = Convert.ToBoolean(val);
                    SaveFlag(bufAddr, flag);
                    return true;
                }
                if (td.OutputType == OutDataType.Bitmap)
                {
                    bool flag = Convert.ToBoolean(val);
                    //bufAddr = (uint)(td.addrInt + td.Offset + td.ExtraOffset - tableInfo.compareFile.tableBufferOffset);
                    SaveBitmap(bufAddr, flag);
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
                            newRawValue = ((UInt64)origRawValue & ~mask) + val;
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
                            newRawValue = ((UInt64)origRawValue & ~mask) + val;
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
                    tableBuffer[bufAddr] = (byte)newRawValue;
                if (td.DataType == InDataType.SWORD)
                    SaveShort(tableBuffer, bufAddr, (short)newRawValue,MSB);
                if (td.DataType == InDataType.UWORD)
                    SaveUshort(tableBuffer, bufAddr, (ushort)newRawValue, MSB);
                if (td.DataType == InDataType.FLOAT32)
                    SaveFloat32(tableBuffer, bufAddr, (Single)newRawValue, MSB);
                if (td.DataType == InDataType.INT32)
                    SaveInt32(tableBuffer, bufAddr, (Int32)newRawValue, MSB);
                if (td.DataType == InDataType.UINT32)
                    SaveUint32(tableBuffer, bufAddr, (UInt32)newRawValue, MSB);
                if (td.DataType == InDataType.FLOAT64)
                    SaveFloat64(tableBuffer, bufAddr, newRawValue, MSB);
                if (td.DataType == InDataType.INT64)
                    SaveInt64(tableBuffer, bufAddr, (Int64)newRawValue, MSB);
                if (td.DataType == InDataType.UINT64)
                    SaveUint64(tableBuffer, bufAddr, (UInt64)newRawValue, MSB);
                if (newRawValue != lastRawValue)
                    retVal = true;
                lastValue = CalculatedValue(newRawValue);
                lastRawValue = newRawValue;
                Array.Copy(tableBuffer, bufAddr, lastRawBytes, 0, td.ElementSize());
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger("Invalid value");
            }
            return retVal;
        }

        private void SaveFlag(uint bufAddr, bool flag)
        {
            byte[] tableBuffer = tableInfo.compareFile.buf;
            string maskStr = "FF";
            if (td.BitMask != null)
                maskStr = td.BitMask.Replace("0x", "");
            if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
            {
                byte mask = Convert.ToByte(maskStr, 16);
                byte newVal;
                if (flag)
                {
                    newVal = (byte)(tableBuffer[bufAddr] | mask);
                    tableBuffer[bufAddr] = newVal;
                }
                else
                {
                    mask = (byte)~mask;
                    newVal = (byte)(tableBuffer[bufAddr] & mask);
                    tableBuffer[bufAddr] = newVal;
                }
                lastValue = newVal;
                lastRawValue = newVal;
                Array.Copy(tableBuffer, bufAddr, lastRawBytes, 0, td.ElementSize());
            }
            else if (td.DataType == InDataType.SWORD || td.DataType == InDataType.UWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = ReadUint16(tableBuffer, bufAddr, MSB);
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
                SaveUshort(tableBuffer, bufAddr, newVal, MSB);
                lastValue = newVal;
                lastRawValue = newVal;
                Array.Copy(tableBuffer, bufAddr, lastRawBytes, 0, td.ElementSize());
            }
            else if (td.DataType == InDataType.INT32 || td.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = ReadUint32(tableBuffer, bufAddr, MSB);
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
                SaveUint32(tableBuffer, bufAddr, newVal, MSB);
                lastValue = newVal;
                lastRawValue = newVal;
                Array.Copy(tableBuffer, bufAddr, lastRawBytes, 0, td.ElementSize());
            }
            else if (td.DataType == InDataType.INT64 || td.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                UInt64 curVal = ReadUint64(tableBuffer, bufAddr, MSB);
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
                SaveUint64(tableBuffer, bufAddr, newVal, MSB);
                lastValue = newVal;
                lastRawValue = newVal;
                Array.Copy(tableBuffer, bufAddr, lastRawBytes, 0, td.ElementSize());
            }
        }
        private void SaveBitmap(uint bufAddr, bool flag)
        {
            byte[] tableBuffer = tableInfo.compareFile.buf;
            byte bit = (byte)(Row % 8);
            byte mask = (byte)(0x01 << bit );
            byte newVal;
            if (flag)
            {
                newVal = (byte)(tableBuffer[bufAddr] | mask);
                tableBuffer[bufAddr] = newVal;
                lastValue = 1;
            }
            else
            {
                mask = (byte)~mask;
                newVal = (byte)(tableBuffer[bufAddr] & mask);
                tableBuffer[bufAddr] = newVal;
                lastValue = 0;
            }
            lastRawValue = newVal;
            Array.Copy(tableBuffer, bufAddr, lastRawBytes, 0, td.ElementSize());
        }

    }



    public class TableInfo
    {
        public TableInfo(PcmFile _pcm, TableData _td)
        {
            pcm = _pcm;
            td = _td;
            tableCells = new List<TableCell>();
        }
        public PcmFile pcm { get; set; }
        //public uint tdId { get; set; }
        public TableData td { get; set; }
        public List<TableCell> tableCells { get; set; }
        public CompareFile compareFile { get; set; }
    }

    public class CompareFile
    {
        public CompareFile(PcmFile _pcm)
        {
            pcm = _pcm;
            NaviCurrent = pcm.NaviCurrent;
            tableInfos = new List<TableInfo>();
            tableIds = new List<Guid>();
            refTableIds = new Dictionary<Guid, TableData>();
            filteredTables = new List<TableData>();
        }
        public byte[] buf;
        public uint tableBufferOffset;
        public PcmFile pcm { get; set; }
        public List<TableInfo> tableInfos { get; set; }
        public List<TableData> filteredTables { get; set; }
        public List<Guid> tableIds { get; set; }
        //public List<TableId> refTableIds { get; set; }
        public Dictionary<Guid, TableData> refTableIds { get; set; }
        public string fileLetter { get; set; }
        public int Rows { get; set; }   //How many rows (in multitable)
        public int Cols { get; set; }
        public int NaviCurrent { get; set; }    //For navigating in TableEditor, without moving PCM navi
    }
}
