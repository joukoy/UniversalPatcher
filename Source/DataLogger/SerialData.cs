using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class SerialString
    {
        public SerialString(string data, long timestamp, bool prompt)
        {
            Data = data;
            TimeStamp = timestamp;
            Prompt = prompt;
        }
        public string Data { get; internal set; }
        public long TimeStamp { get; internal set; }
        public bool Prompt { get; internal set; }
        //If one string have multiple messages, store timestamps here
        public List<long> TimeStamps { get; set; }  
    }
    public class SerialByte
    {
        public SerialByte(int size)
        {
            TimeStamp = DateTime.Now.Ticks;
            Data = new byte[size];
        }
        public long TimeStamp { get; set; }
        public byte[] Data { get; set; }
    }

}
