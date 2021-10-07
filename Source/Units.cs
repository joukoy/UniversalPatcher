using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class Units
    {
        public string Abbreviation { get; set; }
        public string Unit { get; set; }

        public Units ShallowCopy()
        {
            return (Units)this.MemberwiseClone();
        }
    }
}
