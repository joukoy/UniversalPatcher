using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            searchRange = "";
            tableRange = "";
            //start = "";
            //distanceMin = 0;
            //distanceMax = int.MaxValue;
            //location = 0;
        }

        public void parseAddresses(PcmFile PCM)
        {
            ParseAddress(searchRange, PCM, out searchBlocks);
            ParseAddress(tableRange, PCM, out tableBlocks);

        }
        //public string ID { get; set; }
        public string searchData { get; set; }
        public string Items { get; set; }
        public string Name { get; set; }
        public string searchRange { get; set; }
        public string tableRange { get; set; }

        public List<Block> searchBlocks;
        public List<Block> tableBlocks;

        //public string start { get; set; }
        //public int distanceMin { get; set; }
        //public int distanceMax { get; set; }
        //public int location { get; set; }

        private bool ParseAddress(string Line, PcmFile PCM,  out List<Block> Blocks)
        {
            Debug.WriteLine("Segment address line: " + Line);
            Blocks = new List<Block>();

            if (Line == null || Line == "")
            {
                //It is ok to have empty address (for CS, not for segment)
                Block B = new Block();
                B.End = PCM.fsize;
                B.Start = 0;
                Blocks.Add(B);
                return true;
            }

            string[] Parts = Line.Split(',');
            int i = 0;

            foreach (string Part in Parts)
            {
                string[] StartEnd = Part.Split('-');
                Block B = new Block();
                int Offset = 0;

                if (StartEnd[0].Contains(">"))
                {
                    string[] SO = StartEnd[0].Split('>');
                    StartEnd[0] = SO[0];
                    uint x;
                    if (!HexToUint(SO[1], out x))
                        throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                    Offset = (int)x;
                }
                if (StartEnd[0].Contains("<"))
                {
                    string[] SO = StartEnd[0].Split('<');
                    StartEnd[0] = SO[0];
                    uint x;
                    if (!HexToUint(SO[1], out x))
                        throw new Exception("Can't decode from HEX: " + SO[1] + " (" + Line + ")");
                    Offset = ~(int)x;
                }


                if (!HexToUint(StartEnd[0].Replace("@", ""), out B.Start))
                {
                    throw new Exception("Can't decode from HEX: " + StartEnd[0].Replace("@", "") + " (" + Line + ")");
                }
                if (StartEnd[0].StartsWith("@"))
                {
                    uint tmpStart = B.Start;
                    B.Start = BEToUint32(PCM.buf, tmpStart);
                    B.End = BEToUint32(PCM.buf, tmpStart + 4);
                    tmpStart += 8;
                }
                else
                {
                    if (!HexToUint(StartEnd[1].Replace("@", ""), out B.End))
                        throw new Exception("Can't decode from HEX: " + StartEnd[1].Replace("@", "") + " (" + Line + ")");
                    if (B.End >= PCM.buf.Length)    //Make 1MB config work with 512kB bin
                        B.End = (uint)PCM.buf.Length - 1;
                }
                if (StartEnd.Length > 1 && StartEnd[1].StartsWith("@"))
                {
                    //Read End address from bin at this address
                    B.End = BEToUint32(PCM.buf, B.End);
                }
                if (StartEnd.Length > 1 && StartEnd[1].EndsWith("@"))
                {
                    //Address is relative to end of bin
                    uint end;
                    if (HexToUint(StartEnd[1].Replace("@", ""), out end))
                        B.End = (uint)PCM.buf.Length - end - 1;
                }
                B.Start += (uint)Offset;
                B.End += (uint)Offset;
                Blocks.Add(B);
                i++;
            }
            foreach (Block B in Blocks)
                Debug.WriteLine("Address block: " + B.Start.ToString("X") + " - " + B.End.ToString("X"));
            return true;
        }


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
