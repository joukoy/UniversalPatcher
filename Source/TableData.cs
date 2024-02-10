using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using static Upatcher;
using System.Diagnostics;
using static Helpers;
using System.ComponentModel;

namespace UniversalPatcher
{
    public class TableData
    {
        public TableData()
        {
            //id = (uint)tableDatas.Count;
            OS = "";
            TableName = "";
            ExtraTableName = "";
            //Address = "";
            addrInt = uint.MaxValue;
            DispUnits = DisplayUnits.Undefined;
            Math = "X";
            //SavingMath = "X";
            Units = "";
            Category = "";
            //ExtraCategories = "";
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
        public string ExtraTableName { get; set; }
        public string Category { get; set; }

        private Guid _guid;
        public Guid guid
        {
            get
            {
                if (OutputType == OutDataType.Bitmap)
                {
                    DataType = InDataType.UBYTE;
                }
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
        public int ExtraOffset { get; set; }
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
        public string OS_Address { get; set; }

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
        public void UpdateAddressByOS(string PcmOS)
        {
            if (!string.IsNullOrEmpty(OS) && OS.Contains(":") && (string.IsNullOrEmpty(OS_Address) || !OS_Address.Contains(":")))
            {
                OS_Address = OS;
            }
            if (string.IsNullOrEmpty(OS_Address))
                return;
            string[] oParts = OS_Address.Split(',');
            foreach(string osAddr in oParts)
            {
                string[] oaParts = osAddr.Split(':');
                if (oaParts.Length == 2)
                {
                    if (oaParts[0] == PcmOS && HexToUint(oaParts[1], out uint addr))
                    {
                        addrInt = addr;
                        OS = PcmOS;
                        return;
                    }
                }
            }
        }
        public List<string> SubCategories()
        {
            List<string> cats = new List<string>();
            if (!string.IsNullOrEmpty(ExtraCategories))
            {
                string[] parts = ExtraCategories.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cat in parts)
                {
                    cats.Add(cat.Trim());
                }
            }
            return cats;
        }

        public List<string> MainCategories()
        {
            List<string> cats = new List<string>();
            if (!string.IsNullOrEmpty(Category))
            {
                string[] parts = Category.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cat in parts)
                {
                    cats.Add(cat.Trim());
                }
            }
            return cats;
        }

        public void AddCategory(string cat)
        {
            if (string.IsNullOrEmpty(Category))
            {
                Category = cat;
            }
            else
            {
                if (string.IsNullOrEmpty(ExtraCategories))
                {
                    ExtraCategories = cat;                    
                }
                else
                {
                    ExtraCategories += " - " + cat;
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

        public uint StartAddress()
        {
            return (uint)(addrInt + Offset + ExtraOffset);
        }

        public uint EndAddress()
        {
            return (uint)(addrInt + Offset + ExtraOffset + Size());
        }

        public int Size()
        {
            return Rows * Columns * ElementSize() - 1;
        }
        
        public int ElementSize()
        {
            return GetElementSize(DataType);
        }

        public int Elements()
        {
            return Rows * Columns;
        }

        public TableValueType ValueType()
        {
            TableValueType retVal;

            if (Units == null)
                Units = "";
            if (Values.ToLower().StartsWith("patch:") || Values.ToLower().StartsWith("tablepatch:"))
            {
                retVal = TableValueType.patch;
            }
            else if (Values.StartsWith("Enum: "))
            {
                retVal = TableValueType.selection;
            }
            else if (OutputType == OutDataType.Bitmap)
            {
                retVal = TableValueType.bitmap;
            }
            else if (BitMask != null && BitMask.Length > 0)
            {
                retVal = TableValueType.bitmask;
            }
            else if (OutputType == OutDataType.Bitmap ||  Units.ToLower().Contains("boolean") || Units.ToLower().Contains("t/f"))
            {
                retVal = TableValueType.boolean;
            }
            else if (Units.ToLower().Contains("true") && Units.ToLower().Contains("false"))
            {
                retVal = TableValueType.boolean;
            }
            else
            {
                retVal = TableValueType.number;
            }
            return retVal;
        }

        public TableData ShallowCopy(bool newGuid)
        {
            TableData newTd = (TableData)this.MemberwiseClone();
            if (newGuid)
                newTd.guid = Guid.NewGuid();
            return newTd;
        }

        public void ImportFoundTable(int tId, PcmFile PCM)
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
            ExtraCategories = tSeek.ExtraCategories;
            ExtraDescription = tSeek.ExtraDescription;
            ExtraTableName = tSeek.ExtraTableName;

            //if (!PCM.tableCategories.Contains(Category))
              //  PCM.tableCategories.Add(Category);
        }

        public void ImportDTC(PcmFile PCM, ref List<TableData> tdList, bool primary)
        {
            try
            {
                for (int i=0;i< tdList.Count;i++)
                {
                    if (tdList[i].TableName.StartsWith("DTC.") && tdList[i].Category == "DTC")
                    {
                        Debug.WriteLine("DTC table already defined");
                        return;
                    }
                }

                List<DtcCode> dtcCodes;
                if (primary)
                    dtcCodes = PCM.dtcCodes;
                else
                    dtcCodes = PCM.dtcCodes2;
                if (dtcCodes == null)
                {
                    DtcSearch DS = new DtcSearch();
                    dtcCodes = DS.SearchDtc(PCM, primary);
                    if (primary)
                        PCM.dtcCodes = dtcCodes;
                    else
                        PCM.dtcCodes2 = dtcCodes;
                }
                if (dtcCodes== null)
                    return;
                string rHeader = "";
                for (int i = 0; i < dtcCodes.Count; i++)
                {
                    rHeader += dtcCodes[i].Code + ",";
                }
                rHeader = rHeader.Trim(',');

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
                dtcTd.Math = dtc.StatusMath;
                dtcTd.ColumnHeaders = " ";
                if (string.IsNullOrEmpty(dtcTd.Math))
                {
                    dtcTd.Math = "X";
                }
                dtcTd.OS = PCM.OS;
                dtcTd.RowHeaders = rHeader;
                dtcTd.Rows = (ushort)dtcCodes.Count;
                //dtcTd.SavingMath = "X";
                if (dtcCodes[0].P10)
                {
                    //dtcTd.Values = "Enum: 0:Enabled,1:Disabled";
                    //dtcTd.TableName = "DTC-P10.Status";
                    dtcTd.Units = "Boolean";
                    dtcTd.Math = "DTC.DTC_Enable";
                    if (primary)
                        dtcTd.TableName = "DTC.DTC_Enable";
                    else
                        dtcTd.TableName = "DTC2.Enable";
                    dtcTd.Max = 1;
                }
                else if (PCM.dtcCombined)
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
                        dtcTd.TableName = "DTC.DTC_Enable";
                    else
                        dtcTd.TableName = "DTC2.DTC_Enable";
                    dtcTd.Max = 3;
                }
                if (!string.IsNullOrEmpty(dtcCodes[0].Values))
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
                    if (PCM.dtcCodes[0].P10)
                        dtcTd.Math = "DTC.MIL_Enable";
                    else
                        dtcTd.Math = "X";
                    dtcTd.OS = PCM.OS;
                    dtcTd.Max = 1;
                    dtcTd.Units = "Boolean";
                    dtcTd.ColumnHeaders = " ";
                    dtcTd.RowHeaders += rHeader;
                    dtcTd.Rows = (ushort)dtcCodes.Count;
                    //dtcTd.SavingMath = "X";
                    //td.Signed = false;
                    dtcTd.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                    //td.Values = "Enum: 0:No MIL (Lamp always off),1:MIL (Lamp may be commanded on by PCM)";
                    tdList.Insert(1, dtcTd);
                }
                if (dtcCodes[0].P10)
                {
                    dtcTd = new TableData();
                    //dtcTd.TableName = "DTC-P10.Type";
                    dtcTd.TableName = "DTC.Type";
                    dtcTd.addrInt = dtc.TypeAddrInt;
                    dtcTd.Category = "DTC";
                    dtcTd.Origin = "seek";
                    //td.ColumnHeaders = "MIL";
                    dtcTd.Columns = 1;
                    dtcTd.OutputType = OutDataType.Int;
                    dtcTd.Decimals = 0;
                    dtcTd.DataType = InDataType.UBYTE;
                    dtcTd.Math = "DTC.Type";
                    dtcTd.OS = PCM.OS;
                    dtcTd.Max = 1;
                    dtcTd.ColumnHeaders = " ";
                    dtcTd.RowHeaders += rHeader;
                    dtcTd.Rows = (ushort)dtcCodes.Count;
                    dtcTd.Values = "Enum: 0:TypeA,1:TypeB";
                    //dtcTd.SavingMath = "X";
                    //td.Signed = false;
                    dtcTd.TableDescription = "0 = No MIL (Lamp always off) 1 = MIL (Lamp may be commanded on by PCM)";
                    //td.Values = "Enum: 0:No MIL (Lamp always off),1:MIL (Lamp may be commanded on by PCM)";
                    tdList.Insert(2, dtcTd);

                }

                //if (!PCM.tableCategories.Contains("DTC"))
                  //  PCM.tableCategories.Add("DTC");

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
