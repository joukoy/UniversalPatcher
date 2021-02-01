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
            //id = (uint)tableDatas.Count;
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
            Origin = "";
            Values = "";
            ExtraDescription = "";
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
                    if (!HexToUint(value.Replace("0x",""), out addrInt))
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
        public string Origin { get; set; }
        public string Values { get; set; }
        public string ColumnHeaders { get; set; }
        public string RowHeaders { get; set; }
        public string TableDescription { get; set; }
        public string ExtraDescription { get; set; }

        public TableData ShallowCopy()
        {
            return (TableData)this.MemberwiseClone();
        }

        public void importFoundTable(int tId, PcmFile PCM)
        {

            TableSeek tSeek = tableSeeks[PCM.foundTables[tId].configId];
            FoundTable ft = PCM.foundTables[tId];

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
            Origin = "seek";
            Values = tSeek.Values;
            Min = tSeek.Min;
            Max = tSeek.Max;
            if (!PCM.tableCategories.Contains(Category))
                PCM.tableCategories.Add(Category);

        }
        public void importDTC(PcmFile PCM, ref List<TableData>tdList)
        {
            if (PCM.dtcCodes.Count == 0)
            {
                DtcSearch DS = new DtcSearch();
                Logger(DS.searchDtc(PCM));
            }
            if (PCM.dtcCodes.Count == 0)
                return;
            TableData dtcTd = new TableData();
            dtcCode dtc = PCM.dtcCodes[0];
            dtcTd.Origin = "seek";
            dtcTd.addrInt = dtc.statusAddrInt;
            dtcTd.Category = "DTC";
            dtcTd.Columns = 1;
            //td.Floating = false;
            dtcTd.OutputType = OutDataType.Int;
            dtcTd.Decimals = 0;
            dtcTd.DataType = InDataType.UBYTE;
            dtcTd.Math = "X";
            dtcTd.OS = PCM.OS;
            for (int i = 0; i < PCM.dtcCodes.Count; i++)
            {
                dtcTd.RowHeaders += PCM.dtcCodes[i].Code + ",";
            }
            dtcTd.RowHeaders = dtcTd.RowHeaders.Trim(',');
            dtcTd.Rows = (ushort)PCM.dtcCodes.Count;
            dtcTd.SavingMath = "X";
            if (PCM.dtcCombined)
            {
                //td.TableDescription = "00 MIL and reporting off, 01 type A/no mil, 02 type B/no mil, 03 type C/no mil, 04 not reported/mil, 05 type A/mil, 06 type B/mil, 07 type c/mil";
                dtcTd.Values = "Enum: 00:MIL and reporting off,01:type A/no mil,02:type B/no mil,03:type C/no mil,04:not reported/mil,05:type A/mil,06:type B/mil,07:type c/mil";
                dtcTd.TableName = "DTC";
            }
            else
            {
                //td.TableDescription = "0 = 1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY), 1 = 2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles), 2 = Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC), 3 = Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                dtcTd.Values = "Enum: 0:1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY),1:2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles),2:Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC),3:Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                dtcTd.TableName = "DTC.Codes";
            }

            tdList.Insert(0, dtcTd);

            if (!PCM.dtcCombined)
            {
                dtcTd = new TableData();
                dtcTd.TableName = "DTC.MIL_Enable";
                dtcTd.addrInt = dtc.milAddrInt;
                dtcTd.Category = "DTC";
                dtcTd.Origin = "seek";
                //td.ColumnHeaders = "MIL";
                dtcTd.Columns = 1;
                dtcTd.OutputType = OutDataType.Flag;
                dtcTd.Decimals = 0;
                dtcTd.DataType = InDataType.UBYTE;
                dtcTd.Math = "X";
                dtcTd.OS = PCM.OS;
                for (int i = 0; i < PCM.dtcCodes.Count; i++)
                {
                    dtcTd.RowHeaders += PCM.dtcCodes[i].Code + ",";
                }
                dtcTd.Rows = (ushort)PCM.dtcCodes.Count;
                dtcTd.SavingMath = "X";
                //td.Signed = false;
                dtcTd.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                //td.Values = "Enum: 0:No MIL (Lamp always off),1:MIL (Lamp may be commanded on by PCM)";
                tdList.Insert(1, dtcTd);
            }
        }

    }
}
