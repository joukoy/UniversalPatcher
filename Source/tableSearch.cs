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
            items = "";
            name = "";
            start = "";
            distanceMin = 0;
            distanceMax = int.MaxValue;
            //location = 0;
        }
        //public string ID { get; set; }
        public string searchData { get; set; }
        public string items { get; set; }
        public string name { get; set; }
        public string start { get; set; }
        public int distanceMin { get; set; }
        public int distanceMax { get; set; }
        //public int location { get; set; }
    }
    public class TableSearchResult
    {
        public TableSearchResult()
        {
            hitCount = 1;
        }
        public string OS { get; set; }
        public string name { get; set; }
        public string segment { get; set; }
        public int hitCount { get; set; }
        public string search { get; set; }
        public string found { get; set; }
        public string data { get; set; }
    }

    public class SearchVariable
    {
        public SearchVariable()
        {
            name = "";
            position = 0;
        }
        public string name { get; set; }
        public uint position { get; set; }
    }
}
