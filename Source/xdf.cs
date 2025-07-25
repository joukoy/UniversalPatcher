using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using static Upatcher;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static Helpers;
using System.Text.RegularExpressions;

namespace UniversalPatcher
{
    public class XDF
    {
        public XDF()
        {

        }
        private PcmFile PCM;
        private List<TableData> tdList;
        private List<string> Categories;


        private struct TableLink
        {
            public string xdfId;
            public string variable;
            public int tdId;
        }

        private enum UnitType
        {
            None = 0,
            External = 1,
            Undefined = 2,
            Unknown = 3,
            AFR = 21,
            Amps = 40,
            Bar = 19,
            Centimeters = 46,
            Centipoise = 95,
            Count = 26,
            CubicCentimeters = 60,
            CubicFeet = 57,
            CubicInches = 58,
            CubicMeters = 59,
            CubicMillimeters = 61,
            CubicYards = 56,
            Days = 81,
            Decibels = 31,
            Celsius = 15,
            Fahrenheit = 14,
            Kelvin = 16,
            DutyCycle = 68,
            FeetPerSecond = 8,
            FluidOunces = 66,
            FootPounds = 75,
            Gallons = 65,
            Grams = 91,
            GramsPerSecond = 11,
            Hertz = 28,
            Horsepower = 70,
            Hours = 82,
            InchesOfMercury = 99,
            InchesOfWater = 101,
            InchPounds = 76,
            Joules = 72,
            Kilograms = 92,
            KilogramsPerHour = 13,
            Kilohertz = 29,
            Kilometers = 42,
            KilometersPerHour = 6,
            KilometersPerLiter = 79,
            Kilopascal = 17,
            Kilowatts = 35,
            Knots = 7,
            Lambda = 22,
            Liters = 62,
            LitersPer100km = 78,
            LV16 = 25,
            LV8 = 24,
            Megahertz = 30,
            Megawatts = 36,
            Meters = 45,
            MetersPerSec = 9,
            MetersPerSecond = 10,
            Microseconds = 86,
            Miles = 43,
            MilesPerGallon = 77,
            MilesPerHour = 5,
            Milliamps = 41,
            Millibar = 20,
            Milligrams = 90,
            Millimeters = 47,
            Milliohms = 39,
            Milliseconds = 85,
            Millivolts = 33,
            Milliwatts = 37,
            Minutes = 83,
            mmOfMercury = 98,
            mmOfWater = 100,
            Multiplier = 97,
            NauticalMile = 44,
            NewtonCentimeter = 74,
            NewtonMeters = 73,
            Newtons = 71,
            Ohms = 38,
            Ounces = 89,
            PascalSecond = 96,
            Percent = 67,
            Pints = 64,
            Poise = 94,
            Pounds = 88,
            PoundsPerHour = 12,
            PoundsPerSquare = 18,
            PulseWidth = 69,
            Quarts = 63,
            RevolutionsPerMin = 4,
            RotationalDegrees = 23,
            Seconds = 84,
            SquareCentimeters = 54,
            SquareFeet = 50,
            SquareInches = 51,
            SquareKilometers = 52,
            SquareMeters = 53,
            SquareMiles = 48,
            SquareMillimeters = 55,
            SquareYards = 49,
            Steps = 27,
            Tonne = 93,
            Tons = 87,
            Volts = 32,
            Watts = 34,
            Years = 80
        }

        private TableData CreateTableFromAxis(XdfAxis ax)
        {
            TableData td = new TableData();
            td.Address = ax.Addr;
            td.ByteOrder = ax.ByteOrder;
            td.Columns = 1;
            td.Rows = ax.IndexCount;
            td.DataType = ax.InputType;
            td.OutputType = ax.OutputType;
            td.Decimals = ax.Decimals;
            td.Math = ax.Math;
            td.Min = ax.Min;
            td.Max = ax.Max;
            td.Units = ax.Units;
            td.Category = "AXIS";
            return td;
        }

        public class XdfPlugin
        {
            public string Platform { get; set; }
            public string Filename { get; set; }
            public string pluginmoduleid { get; set; }
            public string Url { get; set; }
        }
        private class XdfAxis
        {
            public XdfAxis()
            {
                Addr = "";
            }
            public string Addr { get; set; }
            public string Units { get; set; }
            public string Math { get; set; }
            public string Headers { get; set; }
            public string LinkId { get; set; }
            public ushort IndexCount { get; set; }
            public ushort Decimals { get; set; }
            public byte elementSize { get; set; }
            public OutDataType OutputType { get; set; }
            public InDataType InputType { get; set; }
            public bool Signed { get; set; }
            public bool Floating { get; set; }
            public Byte_Order ByteOrder { get; set; }
            public bool RowMajor { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public List<string> multiMath = new List<string>();
            public List<string> multiAddr = new List<string>();
            public List<TableLink> tableLinks = new List<TableLink>();
            public List<TableLink> tableTargets = new List<TableLink>();

            public void ConvertAxis(XElement axle, ref List<TableData>tdList)
            {
                try
                {

                    if (axle.Element("units") != null && axle.Element("units").Value.Length > 0)
                    {
                        Units = axle.Element("units").Value;
                    }
                    else if (axle.Element("unittype") != null)
                    {
                        int ut = Convert.ToInt32(axle.Element("unittype").Value);
                        if (ut > 0)
                            Units = ((UnitType)ut).ToString();
                    }
                    if (axle.Element("indexcount") != null)
                        IndexCount = Convert.ToUInt16(axle.Element("indexcount").Value);

                    foreach (XElement lbl in axle.Elements("LABEL"))
                    {
                        Headers += lbl.Attribute("value").Value + ",";
                        //if (axle.Attribute("id").Value != "z" && !string.IsNullOrEmpty(Units))
                          //  Headers += " " + Units;
                        //Headers += ",";
                    }
                    if (Headers != null)
                        Headers = Headers.Trim(',');

                    if (axle.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                        Addr = axle.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                    string tmp = axle.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim();
                    elementSize = (byte)(Convert.ToInt32(tmp) / 8);
                    if (axle.Element("MATH") == null || axle.Element("MATH").Attribute("equation") == null)
                    {
                        Debug.WriteLine("Math missing!");
                    }
                    else
                    {
                        Math = axle.Element("MATH").Attribute("equation").Value.Trim().Replace("*.", "*0.");
                        Math = Math.Replace("/.", "/0.").ToLower();
                        Math = Math.Replace("+ -", "-").Replace("+-", "-").Replace("++", "+").Replace("+ + ", "+");
                    }
                    if (axle.Element("decimalpl") != null)
                        Decimals = Convert.ToUInt16(axle.Element("decimalpl").Value);
                    if (axle.Element("outputtype") != null)
                        OutputType = (OutDataType)Convert.ToUInt16(axle.Element("outputtype").Value);
                    if (axle.Element("EMBEDDEDDATA") != null && axle.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                    {
                        ushort flags = Convert.ToUInt16(axle.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value, 16);
                        Signed = Convert.ToBoolean(flags & 1);
                        if ((flags & 0x10000) == 0x10000)
                            Floating = true;
                        else
                            Floating = false;
                        if ((flags & 2) == 2)
                        {
                            ByteOrder = Byte_Order.LSB;
                        }
                        if ((flags & 4) == 4)
                            RowMajor = true;
                        else
                            RowMajor = false;
                    }
                    InputType = ConvertToDataType(elementSize, Signed, Floating);
                    if (axle.Element("min") != null)
                        Min = ParseDblValue(axle.Element("min").Value);
                    else
                        Min = GetMinValue(InputType);
                    if (axle.Element("max") != null)
                        Max = ParseDblValue(axle.Element("max").Value);
                    else
                        Max = GetMaxValue(InputType);

                    if (axle.Element("embedinfo") != null && axle.Element("embedinfo").Attribute("linkobjid") != null)
                    {
                        LinkId = axle.Element("embedinfo").Attribute("linkobjid").Value.ToString();
                    }
                    foreach (XElement rowMath in axle.Elements("MATH"))
                    {
                        if (rowMath.Element("VAR") != null && rowMath.Element("VAR").Attribute("address") != null)
                        {
                            //Table have different address for every (?) row
                            Debug.WriteLine(rowMath.Element("VAR").Attribute("address").Value);
                            Debug.WriteLine(rowMath.Attribute("equation").Value);
                            multiAddr.Add(rowMath.Element("VAR").Attribute("address").Value);
                            multiMath.Add(rowMath.Attribute("equation").Value);
                        }
                        foreach (XElement mathVar in rowMath.Elements("VAR"))
                        {
                            if (mathVar.Attribute("id") != null)
                            {
                                string mId = mathVar.Attribute("id").Value.ToLower();
                                if (mId != "x")
                                {
                                    if (mathVar.Attribute("type") != null && mathVar.Attribute("type").Value == "link")
                                    {
                                        //Get math values from other table
                                        string linktable = mathVar.Attribute("linkid").Value;
                                        TableLink tl = new TableLink();
                                        tl.tdId = tdList.Count;
                                        tl.variable = mId;
                                        tl.xdfId = linktable;
                                        tableLinks.Add(tl);
                                    }
                                    if (mathVar.Attribute("type") != null && mathVar.Attribute("type").Value == "address")
                                    {
                                        string addrStr = mathVar.Attribute("address").Value.ToLower().Replace("0x", "");
                                        string bits = "8";
                                        bool lsb = false;
                                        bool isSigned = false;
                                        if (mathVar.Attribute("sizeinbits") != null)
                                            bits = mathVar.Attribute("sizeinbits").Value;
                                        if (mathVar.Attribute("flags") != null)
                                        {
                                            byte flags = Convert.ToByte(mathVar.Attribute("flags").Value, 16);
                                            isSigned = Convert.ToBoolean(flags & 1);
                                            if ((flags & 4) == 4)
                                                lsb = true;
                                            else
                                                lsb = false;
                                        }
                                        InDataType idt = ConvertToDataType(bits, isSigned, false);
                                        string replaceStr = "raw:" + addrStr + ":" + idt.ToString();
                                        if (lsb)
                                            replaceStr += ":lsb ";
                                        else
                                            replaceStr += ":msb ";
                                        Math = Math.Replace(mId, replaceStr);
                                        Math = Math.Replace("+ -", "-").Replace("+-", "-").Replace("++", "+").Replace("+ + ", "+");
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
                    Logger("ConvertAxis, line " + line + ": " + ex.Message + Environment.NewLine);
                }
            }
        }

        private static double ParseDblValue(string dblStr)
        {
            double retVal = 0;
            try
            {
                string[] dParts = dblStr.Split('.');
                if (dParts.Length > 2)
                {
                    if (dParts[dParts.Length - 1].Replace("0", "").Length == 0) //Only zeros
                        dblStr = dblStr.Substring(0, dblStr.Length - dParts[dParts.Length - 1].Length -1); //remove .0000 at end
                }
                retVal = Convert.ToDouble(dblStr, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Logger("ParseDblValue, line " + line + ": " + ex.Message + Environment.NewLine);
            }
            return retVal;
        }

        private void ConvertXdf(XDocument doc)
        {
            try
            {
                //List<string> categories = new List<string>();
                Dictionary<int, string> categories = new Dictionary<int, string>();
                Dictionary<string, Guid> TpIdGuid = new Dictionary<string, Guid>();
                List<TableLink> tableLinks = new List<TableLink>();
                List<TableLink> tableTargets = new List<TableLink>();                

                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFHEADER"))
                {
                    foreach (XElement cat in element.Elements("CATEGORY"))
                    {
                        string catTxt = cat.Attribute("name").Value;
                        int catId = Convert.ToInt32(cat.Attribute("index").Value.Replace("0x",""),16);
                        if (!categories.ContainsKey(catId))
                        {
                            categories.Add(catId, catTxt);
                        }
                    }
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFTABLE"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    xdf.Origin = "xdf";
                    xdf.Min = double.MinValue;
                    xdf.Max = double.MaxValue;

                    List<string> multiMath = new List<string>();
                    List<string> multiAddr = new List<string>();
                    if (PatchList == null)
                        PatchList = new List<XmlPatch>();

                    xdf.TableName = element.Element("title").Value.Replace(".", " ").Trim();
                    if (element.Attribute("uniqueid") != null)
                    {
                        if (TpIdGuid.ContainsKey(element.Attribute("uniqueid").Value))
                            Logger("Duplicate id: " + element.Attribute("uniqueid").Value);
                        else
                            TpIdGuid.Add(element.Attribute("uniqueid").Value, xdf.guid);
                    }

                    foreach (XElement axle in element.Elements("XDFAXIS"))
                    {
                        XdfAxis ax = new XdfAxis();
                        ax.ConvertAxis(axle, ref tdList);
                        if (axle.Attribute("id").Value == "x")
                        {
                            xdf.Columns = ax.IndexCount;
                            if (ax.LinkId != null)
                            {
                                xdf.ColumnHeaders = "TunerPro: " + ax.LinkId;
                            }
                            else if (ax.Addr != null && ax.Addr.Length > 0)
                            {
                                TableData tdNew = CreateTableFromAxis(ax);
                                tdNew.TableName = xdf.TableName + "-X";
                                tdList.Add(tdNew);
                                xdf.ColumnHeaders = "Table: " + tdNew.TableName;
                            }
                            else
                            {
                                xdf.ColumnHeaders = ax.Headers;
                            }
                        }
                        if (axle.Attribute("id").Value == "y")
                        {
                            xdf.Rows = ax.IndexCount;
                            if (ax.LinkId != null)
                            {
                                xdf.RowHeaders = "TunerPro: " + ax.LinkId;
                            }
                            else if (ax.Addr != null && ax.Addr.Length > 0)
                            {
                                TableData tdNew = CreateTableFromAxis(ax);
                                tdNew.TableName = xdf.TableName + "-Y";
                                tdList.Add(tdNew);
                                xdf.RowHeaders = "Table: " + tdNew.TableName;
                            }
                            else
                            {
                                xdf.RowHeaders = ax.Headers;
                            }
                        }
                        if (axle.Attribute("id").Value == "z")
                        {
                            xdf.Units = ax.Units;
                            xdf.Address = ax.Addr;
                            xdf.Math = ax.Math;
                            xdf.Decimals = ax.Decimals;
                            xdf.OutputType = ax.OutputType;
                            xdf.ByteOrder = ax.ByteOrder;
                            xdf.RowMajor = ax.RowMajor;
                            xdf.DataType = ax.InputType;
                            xdf.Min = ax.Min;
                            xdf.Max = ax.Max;
                            tableLinks.AddRange(ax.tableLinks);
                            tableTargets.AddRange(ax.tableTargets);

                            multiAddr.AddRange(ax.multiAddr);
                            multiMath.AddRange(ax.multiMath);
                        }
                    }
                    if (element.Element("units") != null)
                        xdf.Units = element.Element("units").Value;
                    if (element.Element("CATEGORYMEM") != null && element.Element("CATEGORYMEM").Attribute("category") != null)
                    {
                        int catid= 0;
                        foreach (XElement catEle in element.Elements("CATEGORYMEM"))
                        {
                            catid = Convert.ToInt16(catEle.Attribute("category").Value);
                            Debug.WriteLine(catid);
                            if (categories.ContainsKey(catid - 1))
                            {
                                xdf.AddCategory(categories[catid - 1]);
                            }
                        }
                    }
                    if (element.Element("description") != null)
                        xdf.TableDescription = element.Element("description").Value;

                    if (multiMath.Count > 0 && xdf.Rows == multiMath.Count)
                    {
                        string[] rowHdr = xdf.RowHeaders.Replace(".","").Split(',');
                        for (int m=0; m< multiMath.Count; m++)
                        {
                            TableData tdNew = new TableData();
                            tdNew = xdf.ShallowCopy(true);
                            tdNew.Rows = 1; //Convert to single-row, multitable
                            tdNew.Math = multiMath[m];
                            tdNew.Address = multiAddr[m];
                            if (rowHdr.Length >= m - 1)
                                tdNew.TableName += "." + rowHdr[m];
                            else
                                tdNew.TableName += "." + m.ToString();
                            tdList.Add(tdNew);
                        }
                    }
                    else
                    {
                        tdList.Add(xdf);
                    }
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFCONSTANT"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    xdf.Origin = "xdf";
                    xdf.Min = double.MinValue;
                    xdf.Max = double.MaxValue;
                    int elementSize = 0;
                    bool Signed = false;
                    bool Floating = false;

                    xdf.TableName = element.Element("title").Value.Replace(".", " ").Trim();
                    xdf.Math = element.Element("MATH").Attribute("equation").Value.Trim().Replace("*.", "*0.").Replace("/.", "/0.");
                    xdf.Math = xdf.Math.Replace("+ -", "-").Replace("+-", "-").Replace("++", "+").Replace("+ + ", "+");
                    if (element.Element("outputtype") != null)
                        xdf.OutputType = (OutDataType)Convert.ToUInt16(element.Element("outputtype").Value);

                    if (element.Attribute("uniqueid") != null)
                    {
                        TableLink tl = new TableLink();
                        tl.xdfId = element.Attribute("uniqueid").Value;
                        tl.tdId = tdList.Count;
                        tableTargets.Add(tl);
                        if (TpIdGuid.ContainsKey(tl.xdfId))
                            Logger("Duplicate id: " + tl.xdfId);
                        else
                            TpIdGuid.Add( element.Attribute("uniqueid").Value, xdf.guid);
                    }

                    if (element.Element("units") != null && element.Element("units").Value.Length > 0)
                    {
                        xdf.Units = element.Element("units").Value;
                    }
                    else if (element.Element("unittype") != null)
                    {
                        int ut = Convert.ToInt32(element.Element("unittype").Value);
                        if (ut > 0)
                            xdf.Units = ((UnitType)ut).ToString();
                    }

                    if (element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                    {
                        byte flags = Convert.ToByte(element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value, 16);
                        Signed = Convert.ToBoolean(flags & 1);
                        if ((flags & 0x10000) == 0x10000)
                            Floating = true;
                        else
                            Floating = false;
                    }
                    if (element.Element("description") != null)
                        xdf.TableDescription = element.Element("description").Value;
                    elementSize = (byte)(Convert.ToInt32(element.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim()) / 8);
                    xdf.DataType = ConvertToDataType(elementSize, Signed, Floating);
                    if (element.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                        xdf.Address = element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                    if (element.Element("CATEGORYMEM") != null && element.Element("CATEGORYMEM").Attribute("category") != null)
                    {
                        int catid = 0;
                        foreach (XElement catEle in element.Elements("CATEGORYMEM"))
                        {
                            catid = Convert.ToInt16(catEle.Attribute("category").Value);
                            if (categories.ContainsKey(catid - 1))
                            {
                                //if (xdf.Category.Length > 0)
                                  //  xdf.Category += " - ";
                                //xdf.Category += categories[catid - 1];
                                xdf.AddCategory(categories[catid - 1]);
                            }
                        }
                    }
                    if (element.Element("rangelow") != null)
                        xdf.Min = ParseDblValue(element.Element("rangelow").Value);
                    else 
                        xdf.Min = GetMinValue(xdf.DataType);

                    if (element.Element("rangehigh") != null)
                        xdf.Max = ParseDblValue(element.Element("rangehigh").Value);
                    else
                        xdf.Max = GetMaxValue(xdf.DataType);

                    xdf.Columns = 1;
                    xdf.Rows = 1;
                    xdf.RowMajor = false;
                    tdList.Add(xdf);
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFFLAG"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    xdf.Origin = "xdf";
                    xdf.Min = double.MinValue;
                    xdf.Max = double.MaxValue;
                    if (element.Attribute("uniqueid") != null && !TpIdGuid.ContainsKey(element.Attribute("uniqueid").Value))
                    {
                        TpIdGuid.Add(element.Attribute("uniqueid").Value, xdf.guid);
                    }
                    if (element.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                        xdf.Address = element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                    xdf.TableName = element.Element("title").Value.Replace(".", " ").Trim();
                    int elementSize = (byte)(Convert.ToInt32(element.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim()) / 8);
                    xdf.Math = "X";
                    xdf.BitMask = element.Element("mask").Value;
                    xdf.OutputType = OutDataType.Flag;
                    xdf.Columns = 1;
                    xdf.Rows = 1;
                    xdf.RowMajor = false;
                    if (element.Element("CATEGORYMEM") != null && element.Element("CATEGORYMEM").Attribute("category") != null)
                    {
                        int catid = 0;
                        foreach (XElement catEle in element.Elements("CATEGORYMEM"))
                        {
                            catid = Convert.ToInt16(catEle.Attribute("category").Value);
                            if (categories.ContainsKey(catid - 1))
                            {
                                xdf.AddCategory(categories[catid - 1]);
                            }
                        }
                    }
                    if (element.Element("description") != null)
                        xdf.TableDescription = element.Element("description").Value;
                    xdf.DataType = ConvertToDataType(elementSize, false, false);
                    tdList.Add(xdf);
                }

                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFPATCH"))
                {
                    TableData patchTd = new TableData();
                    patchTd.addrInt = 0;
                    patchTd.DataType = InDataType.UBYTE;
                    patchTd.TableName = element.Element("title").Value.Replace(":", ";").Trim();
                    if (element.Attribute("uniqueid") != null)
                    {
                        TpIdGuid.Add(element.Attribute("uniqueid").Value, patchTd.guid);
                    }
                    if (element.Element("description") != null)
                        patchTd.TableDescription = element.Element("description").Value;
                    if (element.Element("CATEGORYMEM") != null && element.Element("CATEGORYMEM").Attribute("category") != null)
                    {
                        int catid = 0;
                        foreach (XElement catEle in element.Elements("CATEGORYMEM"))
                        {
                            catid = Convert.ToInt16(catEle.Attribute("category").Value);
                            if (categories.ContainsKey(catid - 1))
                            {
                                patchTd.AddCategory(categories[catid - 1]);
                            }
                        }
                    }
                    patchTd.Values = "Patch: ";
                    foreach (XElement patchEle in element.Elements("XDFPATCHENTRY"))
                    {
                        if (patchEle.Attribute("address") != null)
                            patchTd.Values += patchEle.Attribute("address").Value.Trim().Replace("0x", "") + ":";
                        if (patchEle.Attribute("patchdata") != null)
                            patchTd.Values += patchEle.Attribute("patchdata").Value;
                        patchTd.Values += ",";
                    }
                    patchTd.Values.Trim(',');
                    tdList.Add(patchTd);
                }

                for (int i = 0; i< tableLinks.Count; i++)
                {
                    TableLink tl = tableLinks[i];
                    TableData linkTd = tdList[tl.tdId];
                    for (int t = 0; t < tableTargets.Count; t++)
                    {
                        if (tableTargets[t].xdfId == tl.xdfId)
                        {
                            TableData targetTd = tdList[tableTargets[t].tdId];
                            linkTd.Math = linkTd.Math.Replace(tl.variable, "TABLE:'" + targetTd.TableName + "'");
                            //linkTd.SavingMath = linkTd.SavingMath.Replace(tl.variable, "TABLE:'" + targetTd.TableName + "'");
                            break;
                        }
                    }
                }

                for (int i=0; i< tdList.Count; i++)
                {
                    TableData td = tdList[i];
/*                    for (int c = 0; c < td.Categories.Count; c++)
                    {
                        if (!PCM.tableCategories.Contains(td.Categories[c]))
                            PCM.tableCategories.Add(td.Categories[c]);
                    }
*/
                    if (td.ColumnHeaders.StartsWith("TunerPro:"))
                    {
                        string id = td.ColumnHeaders.Replace("TunerPro: ", "").Trim();
                        if (TpIdGuid.ContainsKey(id))
                        {
                            for (int t=0; t<tdList.Count;t++)
                            {
                                if (tdList[t].guid == TpIdGuid[id])
                                {
                                    if (AppSettings.xdfImportUseTableName)
                                        td.ColumnHeaders = td.ColumnHeaders.Replace("TunerPro: ", "Table: ").Replace(id, tdList[t].TableName);
                                    else
                                        td.ColumnHeaders = td.ColumnHeaders.Replace("TunerPro: ", "guid: ").Replace(id, tdList[t].guid.ToString());
                                    break;
                                }
                            }
                        }
                    }
                    if (td.RowHeaders.StartsWith("TunerPro:"))
                    {
                        string id = td.RowHeaders.Replace("TunerPro: ", "").Trim();
                        if (TpIdGuid.ContainsKey(id))
                        {
                            for (int t = 0; t < tdList.Count; t++)
                            {
                                if (tdList[t].guid == TpIdGuid[id])
                                {
                                    if (AppSettings.xdfImportUseTableName)
                                        td.RowHeaders = td.RowHeaders.Replace("TunerPro: ", "Table: ").Replace(id, tdList[t].TableName);
                                    else
                                        td.RowHeaders = td.RowHeaders.Replace("TunerPro: ", "guid: ").Replace(id, tdList[t].guid.ToString());
                                    break;
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
                LoggerBold ("XdfImport, line " + line + ": " + ex.Message + Environment.NewLine);
            }
        }

        public void ImportXdf(PcmFile PCM1, List<TableData> tdList1)
        {
            try
            {
                PCM = PCM1;
                tdList = tdList1;
                XDocument doc;
                string fname = SelectFile("Select XDF file", XdfFilter);
                if (fname.Length == 0)
                    return ;
                Logger("Importing file " + fname + "...",false);

                //doc = new XDocument(new XComment(" Written " + DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo)), XElement.Load(fname));
                try
                {
                    doc = new XDocument(XElement.Load(fname));
                    ConvertXdf(doc);
                }
                catch
                {
                    LoggerBold("Password protected, corrupted, or unsupported file.");
                }
                Logger("Done" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("XdfImport, line " + line + ": " + ex.Message + Environment.NewLine);
            }
        }

        private string LinkConversionHeader(string HdrTxt, Dictionary<string, string> tableNameTPid)
        {
            //Examples: Table: haderTable
            //Guid: headerTable
            string retVal = "";
            string conversionTable = HdrTxt.Substring(HdrTxt.IndexOf(':') + 1 ).Trim(); //Remove: table: or guid:
            if (tableNameTPid.ContainsKey(conversionTable))
            {
                string idTxt = tableNameTPid[conversionTable];
                retVal = "      <embedinfo type=\"3\" linkobjid=\"0x" + idTxt + "\" />";                
            }
            return retVal;
        }
        private string LinkConversionHeader2(string HdrTxt, Dictionary<string, string> tableNameTPid)
        {
            //Examples: Table: haderTable
            //Guid: headerTable
            string retVal = "";
            string conversionTable = HdrTxt.Substring(HdrTxt.IndexOf(':') + 1).Trim(); //Remove: table: or guid:
            if (tableNameTPid.ContainsKey(conversionTable))
            {
                retVal = "0x" + tableNameTPid[conversionTable];
            }
            return retVal;
        }

        private string LinkConversionTable(string mathStr, Dictionary<string,string> tableNameTPid, out string linkTxt)
        {
            //Example: TABLE:'MAF Scalar #1'
            string retVal = mathStr;
            linkTxt = "";
            int start = mathStr.IndexOf("table:") + 6;
            int mid = mathStr.IndexOf("'", start + 7);
            string conversionTable = mathStr.Substring(start, mid - start + 1);            
            string table = conversionTable.Replace("'", "");
            if (tableNameTPid.ContainsKey(conversionTable))
            {
                retVal = mathStr.Replace("table:" + conversionTable, "s");
                linkTxt = Environment.NewLine + "      <VAR id=\"s\" type=\"link\" linkid=\"0x" + tableNameTPid[conversionTable] +  "\" />";
            }

            return retVal;
        }

        private string LinkConversionGuid(string mathStr, Dictionary<string, string> tableNameTPid, out string linkTxt)
        {
            //Example: TABLE:'MAF Scalar #1'
            string retVal = mathStr;
            linkTxt = "";
            int start = mathStr.IndexOf("guid:") + 5;
            int mid = mathStr.IndexOf("'", start + 6);
            string guidTxt = mathStr.Substring(start, mid - start + 1);
            string table = guidTxt.Replace("'", "");
            if (tableNameTPid.ContainsKey(guidTxt))
            {
                retVal = mathStr.Replace("table:" + guidTxt, "s");
                linkTxt = Environment.NewLine + "      <VAR id=\"s\" type=\"link\" linkid=\"0x" + tableNameTPid[guidTxt] + "\" />";
            }

            return retVal;
        }

        private string LinkConversionRaw2(string mathStr, out XElement xdfLink)
        {
            // Example: RAW:0x321:SWORD:MSB
            xdfLink = new XElement("VAR");
            string retVal = mathStr;
            int start = mathStr.IndexOf("raw:");
            int mid = mathStr.IndexOf(" ", start + 3);
            string rawStr = mathStr.Substring(start, mid - start + 1);
            string[] rawParts = rawStr.Split(':');
            if (rawParts.Length < 3)
            {
                throw new Exception("Unknown RAW definition in Math: " + mathStr);
            }
            InDataType idt = (InDataType)Enum.Parse(typeof(InDataType), rawParts[2].ToUpper());
            int bits = GetBits(idt);
            string Address = rawParts[1];
            byte flags = 00;
            if (rawParts[2].Substring(0, 1) != "u")  //uint,ushort,ubyte
                flags = 1;
            if (rawParts[3] == "msb")
                flags =(byte)(flags | 4);
            xdfLink.SetAttributeValue("id", "r");
            xdfLink.SetAttributeValue("id", "r");
            xdfLink.SetAttributeValue("type", "address");
            xdfLink.SetAttributeValue("address", rawParts[1]);
            xdfLink.SetAttributeValue("sizeinbits", bits.ToString());
            xdfLink.SetAttributeValue("flags", "0x" + flags.ToString("X"));

            retVal = mathStr.Replace(rawStr, "r");
            return retVal;
        }
        private string LinkConversionRaw(string mathStr, out string linkTxt)
        {
            // Example: RAW:0x321:SWORD:MSB
            linkTxt = "";
            string retVal = mathStr;
            int start = mathStr.IndexOf("raw:");
            int mid = mathStr.IndexOf(" ", start + 3);
            string rawStr = mathStr.Substring(start, mid - start + 1);
            string[] rawParts = rawStr.Split(':');
            if (rawParts.Length < 3)
            {
                throw new Exception("Unknown RAW definition in Math: " + mathStr);
            }
            InDataType idt = (InDataType)Enum.Parse(typeof(InDataType), rawParts[2].ToUpper());
            int bits = GetBits(idt);
            string Address = rawParts[1];
            byte flags = 00;
            if (rawParts[2].Substring(0, 1) != "u")  //uint,ushort,ubyte
                flags = 1;
            if (rawParts[3] == "msb")
                flags = (byte)(flags | 4);

            linkTxt = Environment.NewLine + "        <VAR id=\"r\" type=\"address\" address=\"" + rawParts[1]
                + "\" sizeinbits=\"" + bits.ToString() + "\" flags=\"0x" + flags.ToString("X") + "\" />";

            retVal = mathStr.Replace(rawStr, "r");
            return retVal;
        }

        private string CreateCategoryRows(TableData td, int lastCategory)
        {
            int s = PCM.GetSegmentNumber(td.addrInt);
            if (s == -1) s = lastCategory;
            string cats = "";
            //<CATEGORYMEM index="0" category="5" />
            List<string> tdCats = new List<string>();
            tdCats.Add(td.Category);
            tdCats.AddRange(td.SubCategories());
            for (int c = 0; c < tdCats.Count; c++)
            {
                string cat = tdCats[c];
                for (int x = 1; x < Categories.Count; x++)
                {
                    if (Categories[x] == cat)
                    {
                        cats += "    <CATEGORYMEM index=\"" + c.ToString() + "\" category=\"" + x.ToString() + "\" />" + Environment.NewLine;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(cats))
            {
                cats = "    <CATEGORYMEM index=\"0\" category=\"" + s.ToString() + "\" />" + Environment.NewLine;
            }
            return cats;
        }
        private XElement CreateCategoryElement(TableData td, int lastCategory)
        {
            int s = PCM.GetSegmentNumber(td.addrInt);
            if (s == -1) s = lastCategory;
            XElement xdfCat = new XElement("CATEGORYMEM");
            List<string> tdCats = new List<string>();
            tdCats.Add(td.Category);
            tdCats.AddRange(td.SubCategories());
            bool found = false;
            for (int c = 0; c < tdCats.Count; c++)
            {
                string cat = tdCats[c];
                for (int x = 1; x < Categories.Count; x++)
                {
                    if (Categories[x] == cat)
                    {
                        found = true;
                        xdfCat.SetAttributeValue("index", c.ToString());
                        xdfCat.SetAttributeValue("category", x.ToString());
                        break;
                    }
                }
            }
            if (!found)
            {
                xdfCat.SetAttributeValue("index", "0");
                xdfCat.SetAttributeValue("category", s.ToString());
            }
            return xdfCat;
        }

        public string ExportXdf(PcmFile basefile, List<TableData> tdList1)
        {
            PCM = basefile;
            tdList = tdList1;
            string retVal = "";
            StringBuilder tableRows;
            StringBuilder tableText = new StringBuilder();
            string templateTxt = "";
            int lastCategory = 0;
            int dtcCategory = 0;
            int uniqId = 0x100;
            //List<int> uniqIds = new List<int>();
            //List<string> tableNames = new List<string>();   //Store constant id/name for later use

            Dictionary<string, int> tableNameNr = new Dictionary<string, int>();
            Dictionary<string, string> tableNameTPid = new Dictionary<string, string>();
            Dictionary<string, string> tableGuidTPid = new Dictionary<string, string>();

            try
            {
                Categories = new List<string>();
                Categories.AddRange(PCM.tableCategories);
                //Reserve TunerPro ID for all tables and add to dictionaries:
                for (int nr =0; nr < tdList.Count;nr++)
                {
                    TableData td1 = tdList[nr];
                    if (!tableNameNr.ContainsKey(td1.TableName))
                    {
                        tableNameNr.Add(td1.TableName, nr);
                        tableNameTPid.Add(td1.TableName, uniqId.ToString("X"));
                    }
                    tableGuidTPid.Add(td1.guid.ToString(), uniqId.ToString("X"));
                    uniqId++;
                }
                string fName = Path.Combine(Application.StartupPath, "Templates", "xdfheader.txt");
                StringBuilder xdfText = new StringBuilder(ReadTextFile(fName));
                xdfText.Replace("REPLACE-TIMESTAMP", DateTime.Today.ToString("MM/dd/yyyy H:mm"));
                xdfText.Replace("REPLACE-OSID", basefile.OS);
                xdfText.Replace("REPLACE-BINSIZE", basefile.fsize.ToString("X"));
                for (int s = 1; s < Categories.Count; s++)
                {
                    tableText.Append( "     <CATEGORY index = \"0x" + (s - 1).ToString("X") + "\" name = \"" + Categories[s].Replace("&","+") + "\" />" + Environment.NewLine);
                    lastCategory = s;
                }
                dtcCategory = lastCategory + 1;
                tableText.Append("     <CATEGORY index = \"0x" + (dtcCategory - 1).ToString("X") + "\" name = \"DTC\" />" + Environment.NewLine);
                lastCategory = dtcCategory + 1;
                tableText.Append("     <CATEGORY index = \"0x" + (lastCategory - 1).ToString("X") + "\" name = \"Other\" />");
                xdfText.Replace("REPLACE-CATEGORYNAME", tableText.ToString() + Environment.NewLine);
                if (!PCM.platformConfig.MSB)
                {
                    xdfText.Replace("lsbfirst=\"0\"", "lsbfirst=\"1\"");
                }
                
                fName = Path.Combine(Application.StartupPath, "Templates", basefile.configFile + "-checksum.txt");
                if (File.Exists(fName))
                    xdfText.Append(ReadTextFile(fName));
                else
                    LoggerBold("File not found: " + fName + " - add checksum calculation manually, if necessary.");


                //Add OS ID:
                fName = Path.Combine(Application.StartupPath, "Templates", "xdfconstant.txt");
                if (PCM.OS != null && PCM.OSSegment < PCM.segmentinfos.Length)
                {
                    templateTxt = ReadTextFile(fName);
                    tableText = new StringBuilder(templateTxt.Replace("REPLACE-TABLEID", uniqId.ToString("X")));
                    uniqId++;
                    tableText.Replace("REPLACE-LINKMATH", "");
                    tableText.Replace("REPLACE-MATH", "x");

                    tableText.Replace("REPLACE-TABLEDESCRIPTION", "DON&apos;T MODIFY");
                    tableText.Replace("REPLACE-TABLETITLE", "OS ID - Don&apos;t modify, must match XDF!");
                    tableText.Replace("REPLACE-BITS", "32");
                    tableText.Replace("REPLACE-MINVALUE", basefile.OS);
                    tableText.Replace("REPLACE-MAXVALUE", basefile.OS);
                    tableText.Replace("REPLACE-TABLEADDRESS", basefile.segmentAddressDatas[basefile.OSSegment].PNaddr.Address.ToString("X"));
                    //tableText.Replace("REPLACE-CATEGORY", (basefile.OSSegment + 1).ToString("X"));
                    string cat = "    <CATEGORYMEM index=\"0\" category=\"" + (basefile.OSSegment + 1).ToString("X") + "\" />" + Environment.NewLine;
                    tableText.Replace("REPLACE-CATEGORIES", cat);
                    xdfText.Append(tableText);
                }
                
                fName = Path.Combine(Application.StartupPath, "Templates", "xdfconstant.txt");
                templateTxt = ReadTextFile(fName);
                for (int t = 0; t < tdList.Count; t++)
                {
                    //Add all constants
                    TableData td = tdList[t];
                    if (td.Columns < 2 && td.Rows < 2 && td.OutputType != OutDataType.Flag)
                    {
                        string tableName = td.TableName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); ;
                        if (td.TableName == null || td.TableName.Length == 0)
                            tableName = td.Address;
                        tableText = new StringBuilder(templateTxt.Replace("REPLACE-TABLETITLE", tableName));
                        string cat = CreateCategoryRows(td, lastCategory);
                        tableText.Replace("REPLACE-CATEGORIES", cat);
                        tableText.Replace("REPLACE-TABLEID", tableGuidTPid[td.guid.ToString()]);

                        string linkTxt = "";
                        string mathTxt = td.Math.ToLower();
                        if (mathTxt.Contains("raw:"))
                        {
                            mathTxt = LinkConversionRaw(mathTxt, out linkTxt);
                        }
                        tableText.Replace("REPLACE-LINKMATH", linkTxt);
                        tableText.Replace("REPLACE-MATH", mathTxt);

                        tableText.Replace("REPLACE-TABLEADDRESS", td.StartAddress().ToString("X"));
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:"))
                            descr += ", " + td.Values;
                        if (descr != null)
                        {
                            descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                            if (descr.Length > 900)
                            {
                                descr = descr.Substring(0, 900);
                            }
                            tableText.Replace("REPLACE-TABLEDESCRIPTION", descr);
                        }
                        tableText.Replace("REPLACE-BITS", GetBits(td.DataType).ToString());
                        tableText.Replace("REPLACE-MINVALUE", td.Min.ToString().Replace(",","."));
                        tableText.Replace("REPLACE-MAXVALUE", td.Max.ToString().Replace(",", "."));
                        tableText.Replace("REPLACE-UNITS", td.Units);
                        tableText.Replace("REPLACE-DECIMALS", td.Decimals.ToString());
                        tableText.Replace("REPLACE-OUTPUTTYPE", ((ushort)td.OutputType).ToString());
                        xdfText.Append(tableText);       //Add generated table to end of xdfText
                    }
                }
                
                fName = Path.Combine(Application.StartupPath, "Templates", "xdftable.txt");
                templateTxt =ReadTextFile(fName);
                for (int t = 0; t < tdList.Count; t++)
                {
                    //Add all tables
                    TableData td = tdList[t];
                    if (td.Dimensions() > 1)
                    {
                        string tableName = td.TableName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); 
                        if (td.TableName == null || td.TableName.Length == 0)
                            tableName = td.Address;
                        tableText = new StringBuilder(templateTxt.Replace("REPLACE-TABLETITLE", tableName));
                        string cat = CreateCategoryRows(td, lastCategory);
                        tableText.Replace("REPLACE-CATEGORIES", cat);

                        string linkTxt = "";
                        string mathTxt = td.Math.ToLower();
                        if (mathTxt.Contains("table:"))
                            mathTxt = LinkConversionTable(mathTxt, tableNameTPid, out linkTxt);
                        if (mathTxt.Contains("raw:"))
                        {
                            string tmpTxt = "";
                            mathTxt = LinkConversionRaw(mathTxt, out tmpTxt);
                            linkTxt += tmpTxt;
                        }
                        tableText.Replace("REPLACE-LINKMATH", linkTxt);
                        tableText.Replace("REPLACE-MATH", mathTxt);

                        tableText.Replace("REPLACE-TABLEID", tableGuidTPid[td.guid.ToString()]);
                        tableText.Replace("REPLACE-UNITS", td.Units);
                        tableText.Replace("REPLACE-ROWCOUNT", td.Rows.ToString());
                        tableText.Replace("REPLACE-COLCOUNT", td.Columns.ToString());
                        tableText.Replace("REPLACE-BITS", GetBits(td.DataType).ToString());
                        tableText.Replace("REPLACE-DECIMALS", td.Decimals.ToString());
                        tableText.Replace("REPLACE-OUTPUTTYPE", ((ushort)td.OutputType).ToString());
                        tableText.Replace("REPLACE-TABLEADDRESS",td.StartAddress().ToString("X"));
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:"))
                            descr += ", " + td.Values;
                        if (descr != null)
                        {
                            descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                            if (descr.Length > 900)
                            {
                                descr = descr.Substring(0, 900);
                            }
                            tableText.Replace("REPLACE-TABLEDESCRIPTION", descr);
                        }
                        tableText.Replace("REPLACE-MINVALUE", td.Min.ToString().Replace(",", "."));
                        tableText.Replace("REPLACE-MAXVALUE", td.Max.ToString().Replace(",", "."));
                        int tableFlags = 0;
                        if (GetSigned(td.DataType))
                        {
                            tableFlags++;
                        }
                        if (td.ByteOrder == Byte_Order.LSB)
                        {
                            tableFlags += 2;
                        }
                        if (td.RowMajor == false)
                        {
                            tableFlags += 4;
                        }
                        tableText.Replace("REPLACE-TYPEFLAGS", tableFlags.ToString("X2"));

                        tableRows = new StringBuilder();
                        string embedinfo = "";
                        if (td.RowHeaders == "")
                        {
                            for (int d = 0; d < td.Rows; d++)
                            {
                                tableRows.Append("     <LABEL index=\"" + d.ToString() + "\" value=\"" + d.ToString() + "\" />");
                                if (d < td.Rows - 1)
                                    tableRows.Append(Environment.NewLine);
                            }
                        }
                        else if (td.RowHeaders.ToLower().StartsWith("table:"))
                        {
                            embedinfo = LinkConversionHeader(td.RowHeaders, tableNameTPid);
                        }
                        else if (td.RowHeaders.ToLower().StartsWith("guid:"))
                        {
                            embedinfo = LinkConversionHeader(td.RowHeaders, tableGuidTPid);
                        }
                        else
                        {
                            string[] hParts = td.RowHeaders.Split(',');
                            for (int row = 0; row < hParts.Length; row++)
                            {
                                tableRows.Append("     <LABEL index=\"" + row.ToString() + "\" value=\"" + hParts[row] + "\" />");
                                if (row < hParts.Length - 1)
                                    tableRows.Append(Environment.NewLine);
                            }
                        }
                        tableText = tableText.Replace("REPLACE-TABLEROWS", tableRows.ToString());
                        tableText = tableText.Replace("REPLACE-EMBEDINFOY", embedinfo);

                        StringBuilder tableCols = new StringBuilder();
                        if (td.ColumnHeaders == "")
                        {
                            for (int d = 0; d < td.Columns; d++)
                            {
                                tableCols.Append("     <LABEL index=\"" + d.ToString() + "\" value=\"" + d.ToString() + "\" />");
                                if (d < td.Columns - 1)
                                    tableCols.Append(Environment.NewLine);
                            }
                        }
                        else if (td.ColumnHeaders.ToLower().StartsWith("table:"))
                        {
                            embedinfo = LinkConversionHeader(td.ColumnHeaders, tableNameTPid);
                        }
                        else if (td.RowHeaders.ToLower().StartsWith("guid:"))
                        {
                            embedinfo = LinkConversionHeader(td.ColumnHeaders, tableGuidTPid);
                        }
                        else
                        {
                            string[] hParts = td.ColumnHeaders.Split(',');
                            for (int col = 0; col < hParts.Length; col++)
                            {
                                tableCols.Append("     <LABEL index=\"" + col.ToString() + "\" value=\"" + hParts[col] + "\" />");
                                if (col < hParts.Length - 1)
                                    tableCols.Append( Environment.NewLine);
                            }
                        }
                        tableText = tableText.Replace("REPLACE-TABLECOLS", tableCols.ToString());
                        tableText = tableText.Replace("REPLACE-EMBEDINFOX", embedinfo);

                        xdfText.Append(tableText.ToString());       //Add generated table to end of xdfText
                    }
                }
                
                
                fName = Path.Combine(Application.StartupPath, "Templates", "xdfFlag.txt");
                templateTxt = ReadTextFile(fName);
                for (int t = 0; t < tdList.Count; t++)
                {
                    //Add all flags
                    TableData td = tdList[t];
                    if (td.OutputType == OutDataType.Flag)
                    {
                        string tableName = td.TableName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); 
                        if (td.TableName == null || td.TableName.Length == 0)
                            tableName = td.Address;
                        tableText = new StringBuilder(templateTxt.Replace("REPLACE-TABLETITLE", tableName));
                        string cat = CreateCategoryRows(td, lastCategory);
                        tableText.Replace("REPLACE-CATEGORIES", cat);
                        tableText.Replace("REPLACE-TABLEID", tableGuidTPid[td.guid.ToString()]);
                        tableText.Replace("REPLACE-TABLEADDRESS", td.StartAddress().ToString("X"));
                        tableText.Replace("REPLACE-TABLEDESCRIPTION", "");
                        tableText.Replace("REPLACE-BITS", GetBits(td.DataType).ToString());
                        tableText.Replace("REPLACE-MASK", td.BitMask);
                        xdfText.Append(tableText);       //Add generated table to end of xdfText
                    }
                }
                

                xdfText.Append("</XDFFORMAT>" + Environment.NewLine);
                string defFname = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions", basefile.OS + "-generated.xdf");
                string fileName = SelectSaveFile(XdfFilter,defFname);
                if (fileName.Length == 0)
                    return "";
                retVal += "Writing to file: " + Path.GetFileName(fileName);
                WriteTextFile(fileName, xdfText.ToString());
                retVal += " [OK]";

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                retVal += ("Export XDF, line " + line + ": " + ex.Message);
            }
            return retVal;
        }

 
        private void AddChecksumElements(ref XElement xdfFormat, PcmFile PCM)
        {
            try
            {
                List<XdfPlugin> xdfplugins;
                XdfPlugin xdfPlugin = null;
                string pluginXml = Path.Combine(Application.StartupPath, "XML", "XdfPlugins.xml");
                if (File.Exists(pluginXml))
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<XdfPlugin>));
                    System.IO.StreamReader file = new System.IO.StreamReader(pluginXml);
                    xdfplugins = (List<XdfPlugin>)reader.Deserialize(file);
                    file.Close();
                    xdfPlugin = xdfplugins.Where(X => X.Platform.ToLower() == PCM.configFile.ToLower()).FirstOrDefault();
                }
                if (xdfPlugin != null)
                {
                    XElement xdfCS = new XElement("XDFCHECKSUM");
                    xdfCS.SetAttributeValue("uniqueid", "0x99");
                    xdfCS.SetElementValue("title", PCM.OS + " checksum");
                    XElement xdfCsRegion = new XElement("REGION");
                    xdfCsRegion.SetElementValue("pluginmoduleid", xdfPlugin.pluginmoduleid);
                    xdfCsRegion.SetElementValue("datastart", "0x0");
                    xdfCsRegion.SetElementValue("dataend", "0x" + PCM.buf.Length.ToString("X"));
                    xdfCsRegion.SetElementValue("storeaddress", "0x0");
                    xdfCsRegion.SetElementValue("calculationmethod", "0x0");
                    xdfCS.Add(xdfCsRegion);
                    xdfFormat.Add(xdfCS);
                    string pluginPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Plugins", xdfPlugin.Filename);
                    if (!File.Exists(pluginPath))
                    {
                        //System.Net.WebClient wc = new System.Net.WebClient();
                        Logger("XDF requires Checksum plugin. Download from: ");
                        Logger(xdfPlugin.Url);
                        Logger(" => " + pluginPath);
                        //wc.DownloadFile(xdfPlugin.Url, path);
                    }
                }
                else if (PCM.Segments.Count == 0)
                {
                    LoggerBold("Add checksum calculation manually!!");
                }
                else
                {
                    bool incompatibleCS = false;
                    foreach (SegmentConfig sc in PCM.Segments)
                    {
                        if (sc.CS1Blocks.Contains(",") || sc.CS2Blocks.Contains(",") ||
                            (sc.CS1Method != (ushort)CSMethod.Bytesum && sc.CS1Method != (ushort)CSMethod.Wordsum && sc.CS1Method != (ushort)CSMethod.Dwordsum))
                        {
                            incompatibleCS = true;
                            break;
                        }
                    }
                    if (incompatibleCS)
                    {
                        LoggerBold("Add checksum calculation manually!!");
                    }
                    else
                    {
                        for (int seg = 0; seg < PCM.Segments.Count; seg++)
                        {
                            SegmentConfig sc = PCM.Segments[seg];
                            if (sc.CS1Method == (ushort)CSMethod.Bytesum || sc.CS1Method == (ushort)CSMethod.Wordsum || sc.CS1Method == (ushort)CSMethod.Dwordsum)
                            {
                                SegmentInfo si = PCM.segmentinfos[seg];
                                SegmentAddressData sa = PCM.segmentAddressDatas[seg];
                                XElement xdfCS = new XElement("XDFCHECKSUM");
                                xdfCS.SetAttributeValue("uniqueid", seg.ToString());
                                xdfCS.SetElementValue("title", si.Name + " checksum 1");
                                XElement xdfCsRegion = new XElement("REGION");
                                xdfCsRegion.SetElementValue("datastart", "0x" + sa.CS1Blocks[0].Start.ToString("X"));
                                xdfCsRegion.SetElementValue("dataend", "0x" + sa.CS1Blocks[0].End.ToString("X"));
                                if (sc.CS1Method == (ushort)CSMethod.Wordsum)
                                    xdfCsRegion.SetElementValue("datasizebits", "0x10");
                                if (sc.CS1Method == (ushort)CSMethod.Dwordsum)
                                    xdfCsRegion.SetElementValue("datasizebits", "0x20");
                                xdfCsRegion.SetElementValue("storesizebits", "0x" + (sa.CS1Address.Bytes * 8).ToString("X"));
                                xdfCsRegion.SetElementValue("storeaddress", "0x" + sa.CS1Address.Address.ToString("X"));
                                if (sc.CS1Complement == 0)
                                    xdfCsRegion.SetElementValue("calculationmethod", "0x0");
                                else if (sc.CS1Complement == 1)
                                    xdfCsRegion.SetElementValue("calculationmethod", "0x2");
                                else if (sc.CS1Complement == 2)
                                    xdfCsRegion.SetElementValue("calculationmethod", "0x1");
                                xdfCS.Add(xdfCsRegion);
                                xdfFormat.Add(xdfCS);
                            }
                            if (sc.CS2Method == (ushort)CSMethod.Bytesum || sc.CS2Method == (ushort)CSMethod.Wordsum || sc.CS2Method == (ushort)CSMethod.Dwordsum)
                            {
                                SegmentInfo si = PCM.segmentinfos[seg];
                                SegmentAddressData sa = PCM.segmentAddressDatas[seg];
                                XElement xdfCS = new XElement("XDFCHECKSUM");
                                xdfCS.SetAttributeValue("uniqueid", seg.ToString());
                                xdfCS.SetElementValue("title", si.Name + " checksum 2");
                                XElement xdfCsRegion = new XElement("REGION");
                                xdfCsRegion.SetElementValue("datastart", "0x" + sa.CS2Blocks[0].Start.ToString("X"));
                                xdfCsRegion.SetElementValue("dataend", "0x" + sa.CS2Blocks[0].End.ToString("X"));
                                if (sc.CS2Method == (ushort)CSMethod.Wordsum)
                                    xdfCsRegion.SetElementValue("datasizebits", "0x10");
                                if (sc.CS2Method == (ushort)CSMethod.Dwordsum)
                                    xdfCsRegion.SetElementValue("datasizebits", "0x20");
                                xdfCsRegion.SetElementValue("storesizebits", "0x" + (sa.CS2Address.Bytes * 8).ToString("X"));
                                xdfCsRegion.SetElementValue("storeaddress", "0x" + sa.CS2Address.Address.ToString("X"));
                                if (sc.CS2Complement == 0)
                                    xdfCsRegion.SetElementValue("calculationmethod", "0x0");
                                else if (sc.CS2Complement == 1)
                                    xdfCsRegion.SetElementValue("calculationmethod", "0x2");
                                else if (sc.CS2Complement == 2)
                                    xdfCsRegion.SetElementValue("calculationmethod", "0x1");
                                xdfCS.Add(xdfCsRegion);
                                xdfFormat.Add(xdfCS);
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
                LoggerBold("XDF, line " + line + ": " + ex.Message);
            }
        }

        private int GetXdfOutputType(TableData td)
        {
            int retVal = 1;
            switch (td.OutputType)
            {
                case OutDataType.Float:
                    retVal = 1;
                    break;
                case OutDataType.Bitmap:
                    retVal = 2;
                    break;
                case OutDataType.Int:
                    retVal = 2;
                    break;
                case OutDataType.Flag:
                    retVal = 2;
                    break;
                case OutDataType.Hex:
                    retVal = 3;
                    break;
                case OutDataType.Text:
                    retVal = 4;
                    break;
                case OutDataType.Filename:
                    retVal = 4;
                    break;
            }
            return retVal;
        }
        public void ExportXdf2(PcmFile basefile, List<TableData> tdList1)
        {
            PCM = basefile;
            tdList = tdList1;
            int lastCategory = 0;
            int dtcCategory = 0;
            int uniqId = 0x100;
            //List<int> uniqIds = new List<int>();
            //List<string> tableNames = new List<string>();   //Store constant id/name for later use

            Dictionary<string, int> tableNameNr = new Dictionary<string, int>();
            Dictionary<string, string> tableNameTPid = new Dictionary<string, string>();
            Dictionary<string, string> tableGuidTPid = new Dictionary<string, string>();

            try
            {

                Categories = new List<string>();
                Categories.AddRange(PCM.tableCategories);
                //Reserve TunerPro ID for all tables and add to dictionaries:
                for (int nr = 0; nr < tdList.Count; nr++)
                {
                    TableData td1 = tdList[nr];
                    if (!tableNameNr.ContainsKey(td1.TableName))
                    {
                        tableNameNr.Add(td1.TableName, nr);
                        tableNameTPid.Add(td1.TableName, uniqId.ToString("X"));
                    }
                    tableGuidTPid.Add(td1.guid.ToString(), uniqId.ToString("X"));
                    uniqId++;
                }

                XElement xdfFormat = new XElement("XDFFORMAT");
                xdfFormat.SetAttributeValue("Version", "1.60");
                XElement xdfHdr = new XElement("XDFHEADER");
                xdfHdr.SetElementValue("flags", "0x1");
                xdfHdr.SetElementValue("description", "");
                if (string.IsNullOrEmpty(PCM.OS))
                    xdfHdr.SetElementValue("deftitle", "Universalpatcher exported XDF");
                else
                    xdfHdr.SetElementValue("deftitle", PCM.OS);
                XElement xdfBaseOffset = new XElement("BASEOFFSET");
                xdfBaseOffset.SetAttributeValue("offset", "0");
                xdfBaseOffset.SetAttributeValue("subtract", "0");
                xdfHdr.Add(xdfBaseOffset);
                XElement xdfDefaults = new XElement("DEFAULTS");
                xdfDefaults.SetAttributeValue("datasizeinbits", "8");
                xdfDefaults.SetAttributeValue("sigdigits", "2");
                xdfDefaults.SetAttributeValue("outputtype", "1");
                xdfDefaults.SetAttributeValue("signed", "0");
                if (PCM.platformConfig.MSB)
                    xdfDefaults.SetAttributeValue("lsbfirst", "0");
                else
                    xdfDefaults.SetAttributeValue("lsbfirst", "1");
                xdfDefaults.SetAttributeValue("float", "0");
                xdfHdr.Add(xdfDefaults);
                XElement xdfRegion = new XElement("REGION");
                xdfRegion.SetAttributeValue("type", "0xFFFFFFFF");
                xdfRegion.SetAttributeValue("startaddress", "0x0");
                xdfRegion.SetAttributeValue("size", basefile.fsize.ToString("X"));
                xdfRegion.SetAttributeValue("regionflags", "0x0");
                xdfRegion.SetAttributeValue("name", "Binary File");
                xdfRegion.SetAttributeValue("desc", "This region describes the bin file edited by this XDF");
                xdfHdr.Add(xdfRegion);

                List<string> addedCategories = new List<string>();
                for (int s = 1; s < Categories.Count; s++)
                {
                    string catName = Categories[s].Replace("&", "+");
                    if (!addedCategories.Contains(catName))
                    {
                        addedCategories.Add(catName);
                        XElement xdfCat = new XElement("CATEGORY");
                        xdfCat.SetAttributeValue("index", "0x" + (s - 1).ToString("X"));
                        xdfCat.SetAttributeValue("name", catName);
                        xdfHdr.Add(xdfCat);
                        lastCategory = s;
                    }
                }
                if (!addedCategories.Contains("DTC"))
                {
                    dtcCategory = lastCategory + 1;
                    XElement xdfDtcCat = new XElement("CATEGORY");
                    xdfDtcCat.SetAttributeValue("index", "0x" + (dtcCategory - 1).ToString("X"));
                    xdfDtcCat.SetAttributeValue("name", "DTC");
                    xdfHdr.Add(xdfDtcCat);
                }
                if (!addedCategories.Contains("Other"))
                {
                    lastCategory = dtcCategory + 1;
                    XElement xdfOtherCat = new XElement("CATEGORY");
                    xdfOtherCat.SetAttributeValue("index", "0x" + (dtcCategory - 1).ToString("X"));
                    xdfOtherCat.SetAttributeValue("name", "Other");
                    xdfHdr.Add(xdfOtherCat);
                }
                xdfFormat.Add(xdfHdr);

                AddChecksumElements(ref xdfFormat, basefile);

                //Add OS ID:
                if (PCM.OS != null && basefile.OSSegment < basefile.Segments.Count)
                {
                    XElement xdfOsId = new XElement("XDFCONSTANT");
                    xdfOsId.SetAttributeValue("uniqueid", uniqId.ToString("X"));
                    xdfOsId.SetAttributeValue("flags", "0xC");
                    uniqId++;
                    xdfOsId.SetElementValue("title", "OS ID - Dont modify, must match XDF!");
                    xdfOsId.SetElementValue("description", "DONT MODIFY");
                    XElement xdfOsidCat = new XElement("CATEGORYMEM");
                    xdfOsidCat.SetAttributeValue("index", "0");
                    xdfOsidCat.SetAttributeValue("category", (basefile.OSSegment + 1).ToString("X"));
                    xdfOsId.Add(xdfOsidCat);
                    XElement xdfOsIdEmbed = new XElement("EMBEDDEDDATA");
                    xdfOsIdEmbed.SetAttributeValue("mmedaddress", "0x" + basefile.segmentAddressDatas[basefile.OSSegment].PNaddr.Address.ToString("X"));
                    xdfOsIdEmbed.SetAttributeValue("mmedelementsizebits", (basefile.segmentAddressDatas[basefile.OSSegment].PNaddr.Bytes * 8).ToString());
                    xdfOsIdEmbed.SetAttributeValue("mmedmajorstridebits", "0");
                    xdfOsIdEmbed.SetAttributeValue("mmedminorstridebits", "0");
                    xdfOsId.Add(xdfOsIdEmbed);
                    xdfOsId.SetElementValue("units", "");
                    //xdfOsId.SetElementValue("decimalpl", "");
                    xdfOsId.SetElementValue("outputtype", "2");
                    xdfOsId.SetElementValue("rangehigh", basefile.OS);
                    xdfOsId.SetElementValue("rangelow", basefile.OS);
                    xdfOsId.SetElementValue("datatype", "2");
                    xdfOsId.SetElementValue("unittype", "2");
                    XElement xdfOsidDalink = new XElement("DALINK");
                    xdfOsidDalink.SetAttributeValue("index","0");
                    xdfOsId.Add(xdfOsidDalink);
                    XElement xdfOsidMath = new XElement("MATH");
                    xdfOsidMath.SetAttributeValue("equation","X");
                    XElement xdfOsidMathVar = new XElement("VAR");
                    xdfOsidMathVar.SetAttributeValue("id", "X");
                    xdfOsidMath.Add(xdfOsidMathVar);
                    xdfOsId.Add(xdfOsidMath);
                    xdfFormat.Add(xdfOsId);
                }

                for (int t = 0; t < tdList.Count; t++)
                {
                    //Add all constants
                    TableData td = tdList[t];
                    if (td.Columns < 2 && td.Rows < 2 && td.OutputType != OutDataType.Flag)
                    {
                        string tableName = td.TableName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); ;
                        if (td.TableName == null || td.TableName.Length == 0)
                            tableName = td.Address;

                        XElement xdfConst = new XElement("XDFCONSTANT");
                        xdfConst.SetAttributeValue("uniqueid", tableGuidTPid[td.guid.ToString()]);
                        xdfConst.SetAttributeValue("flags", "0xC");
                        xdfConst.SetElementValue("title", tableName);
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:") && !descr.ToLower().Contains("enum:")) 
                            descr += ", " + td.Values;
                        descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        if (descr.Length > 900)
                        {
                            descr = descr.Substring(0, 900);
                        }
                        xdfConst.SetElementValue("description", descr);
                        XElement xdfConstCat = CreateCategoryElement(td, lastCategory);
                        xdfConst.Add(xdfConstCat);
                        XElement xdfConstEmbed = new XElement("EMBEDDEDDATA");
                        xdfConstEmbed.SetAttributeValue("mmedaddress", "0x" + td.StartAddress().ToString("X"));
                        xdfConstEmbed.SetAttributeValue("mmedelementsizebits", GetBits(td.DataType).ToString());
                        xdfConstEmbed.SetAttributeValue("mmedmajorstridebits", "0");
                        xdfConstEmbed.SetAttributeValue("mmedminorstridebits", "0");
                        xdfConst.Add(xdfConstEmbed);
                        xdfConst.SetElementValue("units", td.Units);
                        xdfConst.SetElementValue("decimalpl", td.Decimals.ToString());
                        xdfConst.SetElementValue("outputtype", GetXdfOutputType(td));
                        xdfConst.SetElementValue("rangehigh", td.Max.ToString().Replace(",", "."));
                        xdfConst.SetElementValue("rangelow", td.Min.ToString().Replace(",", "."));
                        xdfConst.SetElementValue("datatype", "2");
                        xdfConst.SetElementValue("unittype", "2");
                        XElement xdfConstDalink = new XElement("DALINK");
                        xdfConstDalink.SetAttributeValue("index", "0");
                        XElement xdfConstMath = new XElement("MATH");
                        XElement xdfConstMathVar = new XElement("VAR");
                        xdfConst.Add(xdfConstDalink);
                        string mathTxt = td.Math.ToLower();
                        if (mathTxt.Contains("raw:"))
                        {
                            mathTxt = LinkConversionRaw2(mathTxt, out XElement xdfConstLink);
                            xdfConstMathVar.Add(xdfConstLink);
                        }
                        xdfConstMath.SetAttributeValue("equation", mathTxt);
                        xdfConstMathVar.SetAttributeValue("id", "X");
                        xdfConstMath.Add(xdfConstMathVar);
                        xdfConst.Add(xdfConstMath);
                        xdfFormat.Add(xdfConst);
                    }
                }

                for (int t = 0; t < tdList.Count; t++)
                {
                    //Add all tables
                    TableData td = tdList[t];
                    if (td.Dimensions() > 1)
                    {
                        string tableName = td.TableName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"); ;
                        if (td.TableName == null || td.TableName.Length == 0)
                            tableName = td.Address;

                        XElement xdfTable = new XElement("XDFTABLE");
                        xdfTable.SetAttributeValue("uniqueid", tableGuidTPid[td.guid.ToString()]);
                        xdfTable.SetAttributeValue("flags", "0x30");
                        xdfTable.SetElementValue("title", tableName);
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:") && !descr.ToLower().Contains("enum:"))
                        {
                            descr += ", " + td.Values;
                        }
                        descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        if (descr.Length > 900)
                        {
                            descr = descr.Substring(0, 900);
                        }
                        xdfTable.SetElementValue("description", descr);
                        XElement xdfTableCat = CreateCategoryElement(td, lastCategory);
                        xdfTable.Add(xdfTableCat);
                        //Add X axis
                        {
                            XElement xdfXaxis = new XElement("XDFAXIS");
                            xdfXaxis.SetAttributeValue("id", "x");
                            xdfXaxis.SetAttributeValue("unigueid", "0x0");
                            XElement xdfXAxisEmbed = new XElement("EMBEDDEDDATA");
                            xdfXAxisEmbed.SetAttributeValue("mmedelementsizebits", GetBits(td.DataType).ToString());
                            xdfXAxisEmbed.SetAttributeValue("mmedmajorstridebits", "-32");
                            xdfXAxisEmbed.SetAttributeValue("mmedminorstridebits", "0");
                            xdfXaxis.Add(xdfXAxisEmbed);
                            xdfXaxis.SetElementValue("indexcount", td.Columns.ToString());
                            xdfXaxis.SetElementValue("units", td.Units);
                            xdfXaxis.SetElementValue("outputtype", "4");
                            xdfXaxis.SetElementValue("datatype", "2");
                            xdfXaxis.SetElementValue("unittype", "0");

                            if (string.IsNullOrEmpty(td.ColumnHeaders))
                            {
                                for (int d = 0; d < td.Columns; d++)
                                {
                                    XElement xdfXcol = new XElement("LABEL");
                                    xdfXcol.SetAttributeValue("index", d.ToString());
                                    xdfXcol.SetAttributeValue("value", d.ToString());
                                    xdfXaxis.Add(xdfXcol);
                                }
                            }
                            else if (td.ColumnHeaders.ToLower().StartsWith("table:"))
                            {
                                string linkXTxt = LinkConversionHeader2(td.RowHeaders, tableNameTPid);
                                XElement xdfXembed = new XElement("embedinfo");
                                xdfXembed.SetAttributeValue("type", "3");
                                xdfXembed.SetAttributeValue("linkobjid", linkXTxt);
                                xdfXaxis.Add(xdfXembed);
                            }
                            else if (td.ColumnHeaders.ToLower().StartsWith("guid:"))
                            {
                                string linkXTxt = LinkConversionHeader2(td.RowHeaders, tableGuidTPid);
                                XElement xdfXembed = new XElement("embedinfo");
                                xdfXembed.SetAttributeValue("type", "3");
                                xdfXembed.SetAttributeValue("linkobjid", linkXTxt);
                                xdfXaxis.Add(xdfXembed);
                            }
                            else
                            {
                                string[] hParts = td.ColumnHeaders.Split(',');
                                for (int row = 0; row < hParts.Length; row++)
                                {
                                    XElement xdfXLabel = new XElement("LABEL");
                                    xdfXLabel.SetAttributeValue("index", row.ToString());
                                    xdfXLabel.SetAttributeValue("value", hParts[row]);
                                    xdfXaxis.Add(xdfXLabel);
                                }
                            }

                            XElement xdfTableDalink = new XElement("DALINK");
                            xdfTableDalink.SetAttributeValue("index", "0");
                            xdfXaxis.Add(xdfTableDalink);
                            XElement xdfXMath = new XElement("MATH");
                            xdfXMath.SetAttributeValue("equation", "X");
                            XElement xdfXMathVar = new XElement("VAR");
                            xdfXMathVar.SetAttributeValue("id", "X");
                            xdfXMath.Add(xdfXMathVar);
                            xdfXaxis.Add(xdfXMath);
                            xdfTable.Add(xdfXaxis);
                        }
                        //Add Y axis
                        {
                            XElement xdfYaxis = new XElement("XDFAXIS");
                            xdfYaxis.SetAttributeValue("id", "y");
                            xdfYaxis.SetAttributeValue("unigueid", "0x1");
                            XElement xdfYaxisEmbed = new XElement("EMBEDDEDDATA");
                            xdfYaxisEmbed.SetAttributeValue("mmedelementsizebits", GetBits(td.DataType).ToString());
                            xdfYaxisEmbed.SetAttributeValue("mmedmajorstridebits", "-32");
                            xdfYaxisEmbed.SetAttributeValue("mmedminorstridebits", "0");
                            xdfYaxis.Add(xdfYaxisEmbed);
                            xdfYaxis.SetElementValue("indexcount", td.Rows.ToString());
                            xdfYaxis.SetElementValue("outputtype", "4");
                            xdfYaxis.SetElementValue("datatype", "2");
                            xdfYaxis.SetElementValue("unittype", "2");
                            if (td.RowHeaders == "")
                            {
                                for (int d = 0; d < td.Rows; d++)
                                {
                                    XElement xdfYrow = new XElement("LABEL");
                                    xdfYrow.SetAttributeValue("index", d.ToString());
                                    xdfYrow.SetAttributeValue("value", d.ToString());
                                    xdfYaxis.Add(xdfYrow);
                                }
                            }
                            else if (td.RowHeaders.ToLower().StartsWith("table:"))
                            {
                                string linkYTxt = LinkConversionHeader2(td.RowHeaders, tableNameTPid);
                                XElement xdfYembed = new XElement("embedinfo");
                                xdfYembed.SetAttributeValue("type", "3");
                                xdfYembed.SetAttributeValue("linkobjid", linkYTxt);
                                xdfYaxis.Add(xdfYembed);
                            }
                            else if (td.RowHeaders.ToLower().StartsWith("guid:"))
                            {
                                string linkYTxt = LinkConversionHeader2(td.RowHeaders, tableGuidTPid);
                                XElement xdfYembed = new XElement("embedinfo");
                                xdfYembed.SetAttributeValue("type", "3");
                                xdfYembed.SetAttributeValue("linkobjid", linkYTxt);
                                xdfYaxis.Add(xdfYembed);
                            }
                            else
                            {
                                string[] hParts = td.RowHeaders.Split(',');
                                for (int row = 0; row < hParts.Length; row++)
                                {
                                    XElement xdfYLabel = new XElement("LABEL");
                                    xdfYLabel.SetAttributeValue("index", row.ToString());
                                    xdfYLabel.SetAttributeValue("value", hParts[row]);
                                    xdfYaxis.Add(xdfYLabel);
                                }
                            }
                            XElement xdfYMath = new XElement("MATH");
                            xdfYMath.SetAttributeValue("equation", "X");
                            XElement xdfYMathVar = new XElement("VAR");
                            xdfYMathVar.SetAttributeValue("id", "X");
                            xdfYMath.Add(xdfYMathVar);
                            xdfYaxis.Add(xdfYMath);
                            xdfTable.Add(xdfYaxis);
                        }
                        //Add Z axis
                        {
                            XElement xdfZaxis = new XElement("XDFAXIS");
                            xdfZaxis.SetAttributeValue("id", "z");

                            XElement xdfZaxisEmbed = new XElement("EMBEDDEDDATA");
                            int tableFlags = 0;
                            if (GetSigned(td.DataType))
                            {
                                tableFlags++;
                            }
                            if (td.ByteOrder == Byte_Order.LSB)
                            {
                                tableFlags += 2;
                            }
                            if (td.RowMajor == true)
                            {
                                tableFlags += 4;
                            }
                            if (tableFlags > 0)
                            {
                                xdfZaxisEmbed.SetAttributeValue("mmedtypeflags", "0x" + tableFlags.ToString("X2"));
                            }
                            xdfZaxisEmbed.SetAttributeValue("mmedaddress", "0x" + td.StartAddress().ToString("X"));
                            xdfZaxisEmbed.SetAttributeValue("mmedelementsizebits", GetBits(td.DataType).ToString());
                            xdfZaxisEmbed.SetAttributeValue("mmedrowcount", td.Rows.ToString());
                            xdfZaxisEmbed.SetAttributeValue("mmedcolcount", td.Columns.ToString());
                            xdfZaxisEmbed.SetAttributeValue("mmedmajorstridebits", "0");
                            xdfZaxisEmbed.SetAttributeValue("mmedminorstridebits", "0");
                            xdfZaxis.Add(xdfZaxisEmbed);
                            xdfZaxis.SetElementValue("decimaplpl", td.Decimals.ToString());
                            xdfZaxis.SetElementValue("min", td.Min.ToString());
                            xdfZaxis.SetElementValue("max", td.Max.ToString());
                            xdfZaxis.SetElementValue("outputtype", GetXdfOutputType(td));
                            xdfTable.Add(xdfZaxis);
                            XElement xdfZmath = new XElement("MATH");
                            XElement xdfZmathVar = new XElement("VAR");
                            xdfZmathVar.SetAttributeValue("id", "X");
                            string linkTxt = "";
                            string mathTxt = td.Math.ToLower();
                            if (mathTxt.Contains("table:"))
                                mathTxt = LinkConversionTable(mathTxt, tableNameTPid, out linkTxt);
                            if (mathTxt.Contains("raw:"))
                            {
                                mathTxt = LinkConversionRaw2(mathTxt, out XElement xdfTableLink);
                                xdfZmathVar.Add(xdfTableLink);
                            }
                            xdfZmath.SetAttributeValue("equation", mathTxt);
                            xdfZmathVar.SetAttributeValue("id", "X");
                            xdfZmath.Add(xdfZmathVar);
                            xdfZaxis.Add(xdfZmath);
                        }
                        xdfFormat.Add(xdfTable);

                    }
                }

                for (int t = 0; t < tdList.Count; t++)
                {
                    //Add all flags
                    TableData td = tdList[t];
                    if (td.OutputType == OutDataType.Flag && td.Dimensions() == 1)
                    {
                        string tableName = td.TableName.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        if (td.TableName == null || td.TableName.Length == 0)
                            tableName = td.Address;

                        XElement xdfFlag = new XElement("XDFFLAG");
                        xdfFlag.SetAttributeValue("uniqueid", tableGuidTPid[td.guid.ToString()]);
                        xdfFlag.SetElementValue("title", tableName);
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:") && !descr.ToLower().Contains("enum:"))
                            descr += ", " + td.Values;
                        descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        if (descr.Length > 900)
                        {
                            descr = descr.Substring(0, 900);
                        }
                        xdfFlag.SetElementValue("description", descr);
                        XElement xdfFlagCat = CreateCategoryElement(td, lastCategory);
                        xdfFlag.Add(xdfFlagCat);
                        XElement xdfFlagEmbed = new XElement("EMBEDDEDDATA");
                        xdfFlagEmbed.SetAttributeValue("mmedaddress", td.StartAddress().ToString("X"));
                        xdfFlagEmbed.SetAttributeValue("mmedelementsizebits", GetBits(td.DataType).ToString());
                        xdfFlagEmbed.SetAttributeValue("mmedmajorstridebits", "0");
                        xdfFlagEmbed.SetAttributeValue("mmedminorstridebits", "0");
                        xdfFlag.Add(xdfFlagEmbed);
                        xdfFlag.SetElementValue("mask", td.BitMask);
                        xdfFormat.Add(xdfFlag);

                    }
                }
                //XDocument xdfDoc = new XDocument();
                //xdfDoc.Add(xdfFormat);

                StringBuilder sb = new StringBuilder("<!-- Written " + DateTime.Now.ToString("MM/dd/yyyy H:mm") + " -->" + Environment.NewLine);
                sb.Append(xdfFormat.ToString());
                string defFname = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions", basefile.OS + "-generated.xdf");
                string fileName = SelectSaveFile(XdfFilter, defFname);
                if (fileName.Length == 0)
                    return;
                Logger( "Writing to file: " + Path.GetFileName(fileName));
                File.WriteAllText(fileName,sb.ToString());
                Logger( "[OK]");

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold ("Export XDF, line " + line + ": " + ex.Message +" Inner ex: " + ex.InnerException);
            }
        }

    }
}
