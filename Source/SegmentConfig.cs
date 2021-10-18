using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;

namespace UniversalPatcher
{
    public class SegmentConfig
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Addresses{ get; set; }    //Segment addresses, can be multiple parts
        public string SwapAddress{ get; set; }  //Segment addresses, can be multiple parts, used for segment swapping
        public string CS1Address{ get; set; }           //Checksum 1 Address
        public string CS2Address{ get; set; }           //Checksum 2 Address
        public CSMethod Checksum1Method { get; set; }     //Checksum 1 calculation method
        public CSMethod Checksum2Method { get; set; }     //Checksum 2 calculation method        
        public string CS1Blocks{ get; set; }       //Calculate checksum 1 from these addresses
        public string CS2Blocks{ get; set; }       //Calculate checksum 2 from these addresses
        public short CS1Complement{ get; set; }   //Calculate 1's or 2's Complement from Checksum?
        public short CS2Complement{ get; set; }   //Calculate 1's or 2's Complement from Checksum?
        public bool CS1SwapBytes{ get; set; }
        public bool CS2SwapBytes{ get; set; }
        public int CVN{ get; set; }             //0=None, 1=Checksum 1, 2=Checksum 2
        public bool Eeprom{ get; set; }         //Special case: P01 or P59 Eeprom segment
        public string PNAddr{ get; set; }
        public string VerAddr{ get; set; }
        public string SegNrAddr{ get; set; }
        public string ExtraInfo{ get; set; }
        public string Comment{ get; set; }
        public string CheckWords{ get; set; }
        public string SearchAddresses{ get; set; }  //Possible start addresses for searched segment
        public string Searchfor{ get; set; }  //search if this found/not found in segment
        public bool Hidden{ get; set; }
        public bool Missing { get; set; }


        public ushort CS1Method
        {
            get { return (ushort)Checksum1Method; }
            set { Checksum1Method = (CSMethod)value; }
        }

        public ushort CS2Method
        {
            get { return (ushort)Checksum2Method; }
            set { Checksum2Method = (CSMethod)value; }
        }

        public SegmentConfig ShallowCopy()
        {
            return (SegmentConfig)this.MemberwiseClone();
        }

    }
}
