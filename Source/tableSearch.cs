using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;

namespace UniversalPatcher
{
    public class TableSearchConfig
    {
        public TableSearchConfig()
        {
            search1 = "";
            rowsearch = "";
            tableSearch = "";
            rowLocation = int.MaxValue;
            tableLocation = int.MaxValue;
            rowSearchDistanceMin = int.MaxValue;
            rowSearchDistanceMax = int.MaxValue;
            tableSearchDistanceMin = int.MaxValue;
            tableSearchDistanceMax = int.MaxValue;
        }
        public string search1 { get; set; }
        public string rowsearch { get; set; }
        public uint rowSearchDistanceMin { get; set; }
        public uint rowSearchDistanceMax { get; set; }
        public int rowLocation { get; set; }
        public string tableSearch { get; set; }
        public uint tableSearchDistanceMin { get; set; }
        public uint tableSearchDistanceMax { get; set; }
        public int tableLocation { get; set; }
    }
    public class TableSearchResult
    {
        public TableSearchResult()
        {

        }
        public string OS { get; set; }
        public string Segment { get; set; }
        public string rows { get; set; }
        public string address { get; set; }
        public string hitCount { get; set; }

    }
}
