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

        private string ConvertMath(string math)
        {
            string retVal = math.ToLower();

            if (retVal.StartsWith("x+"))
                retVal = retVal.Replace("x+", "x-");
            else if (retVal.StartsWith("x-"))
                retVal = retVal.Replace("x-", "x+");
            else if (retVal.StartsWith("x*"))
                retVal = retVal.Replace("x*", "x/");
            else if (retVal.StartsWith("x/"))
                retVal = retVal.Replace("x/", "x*");
            else if (retVal.Contains("*"))
                retVal = math.Replace("*", "/");

            return retVal;
        }


        private struct TableLink
        {
            public string xdfId;
            public string variable;
            public int tdId;
        }

        private enum LabelSource
        {
            Manual,
            Internal,
            Linked,
            LinkedScale
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
                    foreach (XElement lbl in axle.Elements("LABEL"))
                    {
                        Headers += lbl.Attribute("value").Value + ",";
                    }
                    if (Headers != null)
                        Headers = Headers.Trim(',');

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

                    if (axle.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                        Addr = axle.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                    string tmp = axle.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim();
                    elementSize = (byte)(Convert.ToInt32(tmp) / 8);
                    Math = axle.Element("MATH").Attribute("equation").Value.Trim().Replace("*.", "*0.");
                    Math = Math.Replace("/.", "/0.").ToLower();
                    Math = Math.Replace("+ -", "-").Replace("+-", "-").Replace("++", "+").Replace("+ + ", "+");
                    if (axle.Element("decimalpl") != null)
                        Decimals = Convert.ToUInt16(axle.Element("decimalpl").Value);
                    if (axle.Element("outputtype") != null)
                        OutputType = (OutDataType)Convert.ToUInt16(axle.Element("outputtype").Value);
                    if (axle.Element("EMBEDDEDDATA") != null && axle.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                    {
                        byte flags = Convert.ToByte(axle.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value, 16);
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
                            RowMajor = false;
                        else
                            RowMajor = true;
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
                        categories.Add(catId,catTxt);
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
                            xdf.Units = ax.Units;
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
                    if (element.Attribute("uniqueid") != null)
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
                doc = new XDocument(XElement.Load(fname));
                ConvertXdf(doc);
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
                flags =(byte)(flags | 4);

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
                if (PCM.OS != null && PCM.OS.Length > 0)
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

                        tableText.Replace("REPLACE-TABLEADDRESS", ((uint)(td.addrInt + td.Offset + td.ExtraOffset)).ToString("X"));
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:"))
                            descr += ", " + td.Values;
                        descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        tableText.Replace("REPLACE-TABLEDESCRIPTION", descr);
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
                        /*if (td.Values.StartsWith("Enum: ") && !descr.ToLower().Contains("enum"))
                        {
                            string[] hParts = td.Values.Substring(6).Split(',');
                            for (int x = 0; x < hParts.Length; x++)
                            {
                                descr += Environment.NewLine + hParts[x];
                            }
                        }*/

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
                        tableText.Replace("REPLACE-TABLEADDRESS",((uint)(td.addrInt + td.Offset + td.ExtraOffset)).ToString("X"));
                        string descr = td.TableDescription;
                        if (td.Values.ToLower().StartsWith("enum:"))
                            descr += ", " + td.Values;
                        descr = descr.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                        tableText.Replace("REPLACE-TABLEDESCRIPTION", descr);
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
                        tableText.Replace("REPLACE-TABLEADDRESS", ((uint)(td.addrInt + td.Offset + td.ExtraOffset)).ToString("X"));
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
    
    }
}
