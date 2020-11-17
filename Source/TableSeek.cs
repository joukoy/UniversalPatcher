using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;

namespace UniversalPatcher
{
    public class TableSeek
    {
        public TableSeek()
        {
            Name = "";
            XmlFile = "";
            SearchStr = "";
            Rows = 0;
            Columns = 0;
            RowHeaders = "";
            ColHeaders = "";
            Math = "X";
            Offset = 0;
            ConditionalOffset = false;
        }
        public string Name { get; set; }
        public string XmlFile { get; set; }
        public string SearchStr { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string RowHeaders { get; set; }
        public string ColHeaders { get; set; }
        public string Math { get; set; }
        public int Offset { get; set; }
        public bool ConditionalOffset { get; set; }

        public void seekTables(PcmFile PCM)
        {
            for (int s=0; s<tableSeeks.Count; s++)
            {
                if (tableSeeks[s].XmlFile.ToLower() == PCM.xmlFile)
                {
                    uint startAddr = 0;
                    uint addr = getAddrbySearchString(PCM, tableSeeks[s].SearchStr, ref startAddr, tableSeeks[s].ConditionalOffset);
                    if (addr < PCM.fsize)
                    {
                        FoundTable ft = new FoundTable();
                        ft.configId = s;
                        ft.Name = tableSeeks[s].Name;
                        ft.addrInt = addr;
                        ft.Address = addr.ToString("X8");
                        foundTables.Add(ft);
                    }
                }
            }
        }
    }

    public class FoundTable
    {
        public FoundTable()
        {
            Name = "";
            addrInt = uint.MaxValue;
            Address = "";
            configId = -1;
        }
        public string Name { get; set; }
        public uint addrInt;
        public string Address { get; set; }
        public int configId;
    }


}
