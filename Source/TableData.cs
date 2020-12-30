using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using static upatcher;

namespace UniversalPatcher
{
    public class TableData
    {
        public TableData()
        {
            id = (uint)tableDatas.Count;
            OS = "";
            TableName = "";
            //Address = "";
            addrInt = uint.MaxValue;
            Math = "X";
            SavingMath = "X";
            Units = "";
            Category = "";
            ColumnHeaders = "";
            RowHeaders = "";
            TableDescription = "";
            RowMajor = true;
            //DataType = TypeFloat;
            Floating = false;
            OutputType = OutDataType.Float;
            DataType = InDataType.UNKNOWN;
            Values = "";
        }
        public uint id { get; set; }
        public string OS { get; set; }
        public string TableName { get; set; }
        public string Category { get; set; }
        public uint addrInt;
        public string Address
        {
            get 
            {
                if (addrInt == uint.MaxValue)
                    return "";
                else
                    return addrInt.ToString("X8"); 
            }
            set 
            {
                if (value.Length > 0)
                {
                    UInt32 prevVal = addrInt;
                    if (!HexToUint(value, out addrInt))
                        addrInt = prevVal;
                }
                else
                {
                    addrInt = uint.MaxValue;
                }
            }         
        }
        public int Offset { get; set; }
        public InDataType DataType { get; set; }
        public byte ElementSize;
        public string Math { get; set; }
        public string SavingMath { get; set; }
        public string Units { get; set; }
        public OutDataType OutputType { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public ushort Decimals { get; set; }
        public bool Signed;
        public bool Floating;
        public ushort Columns { get; set; }
        public ushort Rows { get; set; }
        public string BitMask { get; set; }
        public bool RowMajor { get; set; }
        public string Values { get; set; }
        public string ColumnHeaders { get; set; }
        public string RowHeaders { get; set; }
        public string TableDescription { get; set; }
        public void importFoundTable(int tId, PcmFile PCM)
        {

            TableSeek tSeek = tableSeeks[foundTables[tId].configId];
            FoundTable ft = foundTables[tId];

            addrInt = ft.addrInt;
            //Address = ft.Address;
            Category = ft.Category;
            OutputType = tSeek.OutputType;
            //Floating = tSeek.Floating;
            //ElementSize = (byte)(tSeek.Bits / 8);
            DataType = tSeek.DataType;
            Math = tSeek.Math;
            SavingMath = tSeek.SavingMath;
            OS = PCM.OS;
            RowMajor = tSeek.RowMajor;
            Rows = ft.Rows;
            Columns = ft.Columns;
            ColumnHeaders = tSeek.ColHeaders;
            RowHeaders = tSeek.RowHeaders;
            Decimals = tSeek.Decimals;
            //Signed = tSeek.Signed;
            TableDescription = tSeek.Description;
            TableName = ft.Name;
            Units = tSeek.Units;
            Min = tSeek.Min;
            Max = tSeek.Max;
            if (!tableCategories.Contains(Category))
                tableCategories.Add(Category);

        }
    }
}
