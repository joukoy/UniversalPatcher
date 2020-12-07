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
    public class XDF
    {
        public XDF()
        {

        }
        private PcmFile PCM;
        private string ConvertXdf(XDocument doc)
        {
            string retVal = "";
            try
            {
                List<string> categories = new List<string>();

                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFHEADER"))
                {
                    foreach (XElement cat in element.Elements("CATEGORY"))
                    {
                        string category = cat.Attribute("name").Value;
                        categories.Add(category);
                        if (!tableCategories.Contains(category))
                            tableCategories.Add(category);
                    }
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFTABLE"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;

                    string RowHeaders = "";
                    string ColHeaders = "";
                    string addr = "";
                    string size = "";
                    string math = "";
                    foreach (XElement axle in element.Elements("XDFAXIS"))
                    {
                        if (axle.Attribute("id").Value == "x")
                        {
                            xdf.Columns = Convert.ToUInt16(axle.Element("indexcount").Value);
                            foreach (XElement lbl in axle.Elements("LABEL"))
                            {
                                ColHeaders += lbl.Attribute("value").Value + ",";
                            }
                            ColHeaders = ColHeaders.Trim(',');
                            xdf.ColumnHeaders = ColHeaders;
                        }
                        if (axle.Attribute("id").Value == "y")
                        {
                            xdf.Rows = Convert.ToUInt16(axle.Element("indexcount").Value);
                            foreach (XElement lbl in axle.Elements("LABEL"))
                            {
                                RowHeaders += lbl.Attribute("value").Value + ",";
                            }
                            RowHeaders = RowHeaders.Trim(',');
                            xdf.RowHeaders = RowHeaders;
                        }
                        if (axle.Attribute("id").Value == "z")
                        {
                            addr = axle.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                            xdf.AddrInt = Convert.ToUInt32(addr, 16);
                            xdf.Address = addr;
                            string tmp = axle.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim();
                            size = (Convert.ToInt32(tmp) / 8).ToString();
                            xdf.ElementSize = (byte)(Convert.ToInt32(tmp) / 8);
                            math = axle.Element("MATH").Attribute("equation").Value.Trim();
                            xdf.Math = math;
                            xdf.SavingMath = xdf.Math.Replace("*", "/");
                            xdf.Decimals = Convert.ToUInt16(axle.Element("decimalpl").Value);
                            if (axle.Element("outputtype") != null)
                                xdf.OutputType = Convert.ToUInt16(axle.Element("outputtype").Value);
                        }
                    }
                    xdf.TableName = element.Element("title").Value;
                    if (element.Element("units") != null)
                        xdf.Units = element.Element("units").Value;
                    /*if (element.Element("datatype") != null)
                        xdf.DataType = Convert.ToByte(element.Element("datatype").Value);*/
                    if (element.Element("EMBEDDEDDATA") != null && element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                    {
                        byte flags = Convert.ToByte(element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value,16);
                        xdf.Signed = Convert.ToBoolean(flags & 1);
                        if ((flags & 0x10000) == 0x10000)
                            xdf.Floating = true;
                        else
                            xdf.Floating = false;
                        if ((flags & 4) == 4)
                            xdf.RowMajor = true;
                        else
                            xdf.RowMajor = false;
                    }
                    int catid = Convert.ToInt16(element.Element("CATEGORYMEM").Attribute("category").Value);
                    xdf.Category = categories[catid - 1];
                    if (element.Element("description") != null)
                        xdf.TableDescription = element.Element("description").Value;

                    tableDatas.Add(xdf);
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFCONSTANT"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    if (element.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                    {
                        xdf.TableName = element.Element("title").Value;
                        xdf.AddrInt = Convert.ToUInt32(element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim(), 16);
                        xdf.Address = element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                        xdf.ElementSize = (byte)(Convert.ToInt32(element.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim()) / 8);
                        xdf.Math = element.Element("MATH").Attribute("equation").Value.Trim();
                        if (element.Element("units") != null)
                            xdf.Units = element.Element("units").Value;
                        if (element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                        {
                            byte flags = Convert.ToByte(element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value,16);
                            xdf.Signed = Convert.ToBoolean(flags & 1);
                            if ((flags & 0x10000) == 0x10000)
                                xdf.Floating = true;
                            else
                                xdf.Floating = false;
                        }
                        xdf.Columns = 1;
                        xdf.Rows = 1;
                        xdf.RowMajor = false;
                        int catid = Convert.ToInt16(element.Element("CATEGORYMEM").Attribute("category").Value);
                        xdf.Category = categories[catid - 1];
                        if (element.Element("description") != null)
                            xdf.TableDescription = element.Element("description").Value;

                        tableDatas.Add(xdf);

                    }
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFFLAG"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    if (element.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                    {
                        xdf.TableName = element.Element("title").Value;
                        xdf.AddrInt = Convert.ToUInt32(element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim(), 16);
                        xdf.Address = element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                        xdf.ElementSize = (byte)(Convert.ToInt32(element.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim()) / 8);
                        xdf.Math = "X";
                        xdf.Units = "Mask: " + element.Element("mask").Value;
                        xdf.Columns = 1;
                        xdf.Rows = 1;
                        xdf.RowMajor = false;
                        int catid = Convert.ToInt16(element.Element("CATEGORYMEM").Attribute("category").Value);
                        xdf.Category = categories[catid - 1];
                        if (element.Element("description") != null)
                            xdf.TableDescription = element.Element("description").Value;

                        tableDatas.Add(xdf);

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
                retVal +="XdfImport, line " + line + ": " + ex.Message + Environment.NewLine;
            }
            return retVal;

        }

        public string  importXdf(PcmFile PCM1)
        {
            string retVal = "";
            try
            {
                PCM = PCM1;
                XDocument doc;
                string fname = SelectFile("Select XDF file", "xdf files (*.xdf)|*.xdf|ALL files (*.*| *.*");
                if (fname.Length == 0)
                    return retVal;
                retVal = "Importing file " + fname + "...";

                doc = new XDocument(new XComment(" Written " + DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo)), XElement.Load(fname));
                retVal += ConvertXdf(doc);
                retVal += "Done" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                retVal += "XdfImport, line " + line + ": " + ex.Message + Environment.NewLine;
            }
            return retVal;
        }

    }
}
