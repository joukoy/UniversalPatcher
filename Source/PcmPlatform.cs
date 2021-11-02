using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class PcmPlatform
    {
        public PcmPlatform()
        {
            MSB = true;
        }
        public bool MSB { get; set; }
        public string SegmentFile { get; set; }
        public string TableSeekFile { get; set; }
        public string SegmentSeekFile { get; set; }
        public string PidSearchString { get; set; }
        public uint PidSearchStep { get; set; }
        public string DTC2 { get; set; }
    }
}
