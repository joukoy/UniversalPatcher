using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using static upatcher;
using System.Diagnostics;

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
            DispUnits = DisplayUnits.Undefined;
            Math = "X";
            //SavingMath = "X";
            Units = "";
            Category = "";
            ColumnHeaders = "";
            RowHeaders = "";
            TableDescription = "";
            RowMajor = true;
            //DataType = TypeFloat;
            //Floating = false;
            OutputType = OutDataType.Float;
            DataType = InDataType.UNKNOWN;
            Origin = "";
            Values = "";
            ExtraDescription = "";
            CompatibleOS = "";
            BitMask = "";
            ByteOrder = Byte_Order.PlatformOrder;
        }
        //public uint id { get; set; }
        public string TableName { get; set; }
        public string Category { get; set; }
        private Guid _guid;
        public Guid guid
        {
            get
            {
                if (_guid == Guid.Empty)
                {
                    _guid = Guid.NewGuid();
                }
                return _guid;
            }
            set
            {
                _guid = value;
            }
        }
        public string OS { get; set; }
        public DisplayUnits DispUnits { get; set; }
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
        //public byte ElementSize;
        public string Math { get; set; }
        //public string SavingMath { get; set; }
        public string Units { get; set; }
        public OutDataType OutputType { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public ushort Decimals { get; set; }
        //public bool Signed;
        //public bool Floating;
        public ushort Columns { get; set; }
        public ushort Rows { get; set; }
        public Byte_Order ByteOrder { get; set; }
        public string BitMask { get; set; }
        public bool RowMajor { get; set; }
        public string Origin { get; set; }
        public string Values { get; set; }
        public string ColumnHeaders { get; set; }
        public string RowHeaders { get; set; }
        public string ExtraCategories { get; set; }
        public string TableDescription { get; set; }
        public string ExtraDescription { get; set; }
        private string altOS;
        public string CompatibleOS 
        { 
            get { return altOS; }
            set
            {
                altOS = value;
                if (altOS.Length > 0)
                {
                    if (!altOS.ToLower().StartsWith("table:"))
                    {
                        if (!altOS.StartsWith(","))
                            altOS = "," + altOS;
                        if (!altOS.EndsWith(","))
                            altOS += ",";
                    }
                }
            }
        }
        public int Dimensions ()
        {
            if (Rows < 2 && Columns < 2)
                return 1;
            else if (Rows > 1 && Columns > 1)
                return 3;
            else
                return 2;
        }


        public uint startAddress()
        {
            return (uint)(addrInt + Offset);
        }

        public uint endAddress()
        {
            return (uint)(addrInt + Offset + size());
        }

        public int size()
        {
            return Rows * Columns * elementSize() - 1;
        }
        
        public int elementSize()
        {
            return getElementSize(DataType);
        }

        public int elements()
        {
            return Rows * Columns;
        }

        public TableData ShallowCopy(bool newGuid)
        {
            TableData newTd = (TableData)this.MemberwiseClone();
            if (newGuid)
                newTd.guid = Guid.NewGuid();
            return newTd;
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
            //SavingMath = tSeek.SavingMath;
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
            DispUnits = tSeek.DispUnits;
            if (!PCM.tableCategories.Contains(Category))
                PCM.tableCategories.Add(Category);

        }

        public void importDTC(PcmFile PCM, ref List<TableData> tdList, bool primary)
        {
            try
            {
                List<DtcCode> dtcCodes;
                if (primary)
                    dtcCodes = PCM.dtcCodes;
                else
                    dtcCodes = PCM.dtcCodes2;
                if (dtcCodes == null)
                {
                    DtcSearch DS = new DtcSearch();
                    dtcCodes = DS.searchDtc(PCM, primary);
                }
                if (dtcCodes== null)
                    return;
                TableData dtcTd = new TableData();
                DtcCode dtc = dtcCodes[0];
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
                for (int i = 0; i < dtcCodes.Count; i++)
                {
                    dtcTd.RowHeaders += dtcCodes[i].Code + ",";
                }
                dtcTd.RowHeaders = dtcTd.RowHeaders.Trim(',');
                dtcTd.Rows = (ushort)dtcCodes.Count;
                //dtcTd.SavingMath = "X";
                if (PCM.dtcCombined)
                {
                    //td.TableDescription = "00 MIL and reporting off, 01 type A/no mil, 02 type B/no mil, 03 type C/no mil, 04 not reported/mil, 05 type A/mil, 06 type B/mil, 07 type c/mil";
                    dtcTd.Values = "Enum: 00:MIL and reporting off,01:type A/no mil,02:type B/no mil,03:type C/no mil,04:not reported/mil,05:type A/mil,06:type B/mil,07:type c/mil";
                    if (primary)
                        dtcTd.TableName = "DTC";
                    else
                        dtcTd.TableName = "DTC2";
                    dtcTd.Max = 7;
                }
                else
                {
                    //td.TableDescription = "0 = 1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY), 1 = 2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles), 2 = Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC), 3 = Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
                    dtcTd.Values = "Enum: 0:1 Trip (MIL IMMEDIATELY),1:2 Trips (MIL if DTC active two drive cycles),2:(No MIL store DTC),3:Not Reported (DTC Disabled)";
                    if (primary)
                        dtcTd.TableName = "DTC.Codes";
                    else
                        dtcTd.TableName = "DTC2.Codes";
                    dtcTd.Max = 3;
                }
                if (dtcCodes[0].Values != null && dtcCodes[0].Values.Length > 0)
                    dtcTd.Values = dtcCodes[0].Values;

                tdList.Insert(0, dtcTd);

                if (!PCM.dtcCombined)
                {
                    dtcTd = new TableData();
                    if (primary)
                        dtcTd.TableName = "DTC.MIL_Enable";
                    else
                        dtcTd.TableName = "DTC2.MIL_Enable";
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
                    dtcTd.Max = 1;
                    dtcTd.Units = "Boolean";
                    for (int i = 0; i < dtcCodes.Count; i++)
                    {
                        dtcTd.RowHeaders += dtcCodes[i].Code + ",";
                    }
                    dtcTd.RowHeaders = dtcTd.RowHeaders.Trim(',');
                    dtcTd.Rows = (ushort)dtcCodes.Count;
                    //dtcTd.SavingMath = "X";
                    //td.Signed = false;
                    dtcTd.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                    //td.Values = "Enum: 0:No MIL (Lamp always off),1:MIL (Lamp may be commanded on by PCM)";
                    tdList.Insert(1, dtcTd);
                }
                if (!PCM.tableCategories.Contains("DTC"))
                    PCM.tableCategories.Add("DTC");

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, importDTC, line " + line + ": " + ex.Message);
            }

        }

    }
}
