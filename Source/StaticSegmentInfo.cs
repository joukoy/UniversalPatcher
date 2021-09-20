using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UniversalPatcher
{
    public class StaticSegmentInfo
    {
        public StaticSegmentInfo(SegmentInfo segmentInfo)
        {
            foreach (var prop in segmentInfo.GetType().GetProperties())
            {
                try
                {
                    Type myType = typeof(StaticSegmentInfo);
                    PropertyInfo myPropInfo = myType.GetProperty(prop.Name);
                    myPropInfo.SetValue(this, prop.GetValue(segmentInfo, null), null);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string XmlFile { get; set; }
        public string Address { get; set; }
        public string SwapAddress { get; set; }
        public string SwapSize { get; set; }
        public string Size { get; set; }
        public string CS1 { get; set; }
        public string CS2 { get; set; }
        public string CS1Calc { get; set; }
        public string CS2Calc { get; set; }
        public string cvn { get; set; }
        public string Stock { get; set; }
        public string PN { get; set; }
        public string Ver { get; set; }
        public string SegNr { get; set; }
        public string ExtraInfo { get; set; }

    }
}
