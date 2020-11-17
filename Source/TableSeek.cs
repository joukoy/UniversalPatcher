using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class TableSeek
    {
        public TableSeek()
        {
            Name = "";
            //XmlFile = "";
            SearchStr = "";
            Rows = 0;
            Columns = 0;
            RowHeaders = "";
            ColHeaders = "";
            Math = "X";
            Offset = 0;
            ConditionalOffset = false;
            Bits = 16;
            Decimals = 2;
            DataType = 1;
            UseHit = 1;
        }

        public string Name { get; set; }
        //public string XmlFile { get; set; }
        public string SearchStr { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string RowHeaders { get; set; }
        public string ColHeaders { get; set; }
        public string Math { get; set; }
        public int Offset { get; set; }
        public bool ConditionalOffset { get; set; }
        public ushort Bits { get; set; }
        public ushort Decimals { get; set; }
        public ushort DataType { get; set; }
        public ushort UseHit { get; set; }
        public void seekTables(PcmFile PCM)
        {
            string fileName = Path.Combine(Application.StartupPath, "XML", "TableSeek-" + PCM.xmlFile + ".xml");
            if (File.Exists(fileName))
            {
                Debug.WriteLine("Loading " + fileName);
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableSeek>));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                tableSeeks = (List<TableSeek>)reader.Deserialize(file);
                file.Close();

            }
            else
            {
                tableSeeks = new List<TableSeek>();
                return;
            }

            for (int s=0; s<tableSeeks.Count; s++)
            {
                uint startAddr = 0;
                uint addr = uint.MaxValue;
                for (int hit = 1; hit <= tableSeeks[s].UseHit; hit++)
                {
                    addr = getAddrbySearchString(PCM, tableSeeks[s].SearchStr, ref startAddr, tableSeeks[s].ConditionalOffset);
                }
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
