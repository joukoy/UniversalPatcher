using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static upatcher;

namespace UniversalPatcher
{
    public class TableCell
    {
        public TableCell()
        {

        }
        public int Column { get; set; }
        public int Row { get; set; }
        public string RowhHeader { get; set; }
        public string ColHeader { get; set; }
        public uint addr { get; set; }
        //public uint tableId { get; set; }
        public TableData td { get; set; }
        public object lastValue { get; set; }
        public object origValue { get { return getValue(tableInfo.compareFile.pcm.buf,addr, td, 0, tableInfo.compareFile.pcm); }}
        public double origRawValue {get {return getRawValue(tableInfo.compareFile.pcm.buf, addr, td, 0); }}
        public double lastRawValue  { get; set; }
        public TableInfo tableInfo { get; set; }

        public TableCell ShallowCopy()
        {
            return (TableCell)this.MemberwiseClone();
        }

        public double calculatedValue(double rawValue)
        {
            try
            {
                string mathStr = td.Math.ToLower();
                UInt32 bufAddr = addr - tableInfo.compareFile.tableBufferOffset;
                Debug.WriteLine("Last raw value: " + lastRawValue + ", Last value: " + (double)lastRawValue);

                if (mathStr.Contains("table:"))
                {
                    mathStr = readConversionTable(mathStr, tableInfo.pcm);
                }
                if (mathStr.Contains("raw:"))
                {
                    mathStr = readConversionRaw(mathStr, tableInfo.pcm);
                }

                return parser.Parse(mathStr.Replace("x", rawValue.ToString()));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return double.NaN;
            }
        }

        public void saveValue(double val,bool isRawValue = false)
        {
            try
            {
                UInt32 bufAddr = addr - tableInfo.compareFile.tableBufferOffset;

                if (td.OutputType == OutDataType.Flag && td.BitMask != "")
                {
                    bool flag = Convert.ToBoolean(val);
                    saveFlag(bufAddr, flag);
                    return;
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
                        mathStr = readConversionTable(mathStr, tableInfo.pcm);
                    }
                    if (mathStr.Contains("raw:"))
                    {
                        mathStr = readConversionRaw(mathStr, tableInfo.pcm);
                    }

                    newRawValue = savingMath.getSavingValue(mathStr, td, val);
                    Debug.WriteLine("Calculated raw value: " + newRawValue);
                }
                if (td.DataType != InDataType.FLOAT32 && td.DataType != InDataType.FLOAT64)
                    newRawValue = Math.Round(newRawValue);

                double minRawVal = getMinValue(td.DataType);
                double maxRawVal = getMaxValue(td.DataType);

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
                byte[] tableBuffer = tableInfo.compareFile.buf;
                if (td.DataType == InDataType.UBYTE || td.DataType == InDataType.SBYTE)
                    tableBuffer[bufAddr] = (byte)newRawValue;
                if (td.DataType == InDataType.SWORD)
                    SaveShort(tableBuffer, bufAddr, (short)newRawValue);
                if (td.DataType == InDataType.UWORD)
                    SaveUshort(tableBuffer, bufAddr, (ushort)newRawValue);
                if (td.DataType == InDataType.FLOAT32)
                    SaveFloat32(tableBuffer, bufAddr, (Single)newRawValue);
                if (td.DataType == InDataType.INT32)
                    SaveInt32(tableBuffer, bufAddr, (Int32)newRawValue);
                if (td.DataType == InDataType.UINT32)
                    SaveUint32(tableBuffer, bufAddr, (UInt32)newRawValue);
                if (td.DataType == InDataType.FLOAT64)
                    SaveFloat64(tableBuffer, bufAddr, newRawValue);
                if (td.DataType == InDataType.INT64)
                    SaveInt64(tableBuffer, bufAddr, (Int64)newRawValue);
                if (td.DataType == InDataType.UINT64)
                    SaveUint64(tableBuffer, bufAddr, (UInt64)newRawValue);

                lastValue = calculatedValue(newRawValue);
                lastRawValue = newRawValue;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger("Invalid value");
            }
        }

        private void saveFlag(uint bufAddr, bool flag)
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
            }
            else if (td.DataType == InDataType.SWORD || td.DataType == InDataType.UWORD)
            {
                ushort mask = Convert.ToUInt16(maskStr, 16);
                ushort curVal = BEToUint16(tableBuffer, bufAddr);
                ushort newVal;
                if (flag)
                {
                    newVal = (ushort)(curVal | mask);
                }
                else
                {
                    mask = (byte)~mask;
                    newVal = (ushort)(curVal & mask);
                }
                SaveUshort(tableBuffer, bufAddr, newVal);
                lastValue = newVal;
                lastRawValue = newVal;
            }
            else if (td.DataType == InDataType.INT32 || td.DataType == InDataType.UINT32)
            {
                UInt32 mask = Convert.ToUInt32(maskStr, 16);
                UInt32 curVal = BEToUint32(tableBuffer, bufAddr);
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
                SaveUint32(tableBuffer, bufAddr, newVal);
                lastValue = newVal;
                lastRawValue = newVal;
            }
            else if (td.DataType == InDataType.INT64 || td.DataType == InDataType.UINT64)
            {
                UInt64 mask = Convert.ToUInt64(maskStr, 16);
                UInt64 curVal = BEToUint64(tableBuffer, bufAddr);
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
                SaveUint64(tableBuffer, bufAddr, newVal);
                lastValue = newVal;
                lastRawValue = newVal;
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
            tableInfos = new List<TableInfo>();
            tableIds = new List<int>();
            refTableIds = new Dictionary<int, int>();
            filteredTables = new List<TableData>();
        }
        public byte[] buf;
        public uint tableBufferOffset;
        public PcmFile pcm { get; set; }
        public List<TableInfo> tableInfos { get; set; }
        public List<TableData> filteredTables { get; set; }
        public List<int> tableIds { get; set; }
        //public List<TableId> refTableIds { get; set; }
        public Dictionary<int, int> refTableIds { get; set; }
        public string fileLetter { get; set; }
        public int Rows { get; set; }   //How many rows (in multitable)
        public int Cols { get; set; }
    }
}
