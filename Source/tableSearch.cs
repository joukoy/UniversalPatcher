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
            //ID = "";
            searchData = "";
            Items = "";
            Name = "";
            //start = "";
            //distanceMin = 0;
            //distanceMax = int.MaxValue;
            //location = 0;
        }
        //public string ID { get; set; }
        public string searchData { get; set; }
        public string Items { get; set; }
        public string Name { get; set; }
        //public string start { get; set; }
        //public int distanceMin { get; set; }
        //public int distanceMax { get; set; }
        //public int location { get; set; }
    }
    public class TableSearchResult
    {
        public TableSearchResult()
        {
            hitCount = 1;
        }
        public string OS { get; set; }
        public string File { get; set; }
        public string Name { get; set; }
        public string Segment { get; set; }
        public int hitCount { get; set; }
        public string Search { get; set; }
        public string Found { get; set; }
        public uint AddressInt;
        public string Address { get; set; }
        public string Data { get; set; }
    }

    public class SearchVariable
    {
        public SearchVariable()
        {
            Name = "";
            Data = "";
        }
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
