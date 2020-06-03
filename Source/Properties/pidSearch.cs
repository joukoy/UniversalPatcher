using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static upatcher;

namespace UniversalPatcher.Properties
{
    public class PidSearch
    {
        public PidSearch(PcmFile PCM1)
        {
            PCM = PCM1;
            startAddress = searchBytes("00 01 02 00 * * * * 00 03 01 00 * * * * 00 04 00 00 * * * * 00 05 00 00", 0, PCM.fsize-6);
            if (startAddress < uint.MaxValue)
                searchPids();
        }

        public class PID
        {
            public ushort PidNumber { get; set; }
            public ushort Bytes { get; set; }
            public string  Subroutine { get; set; }
            public string  RamAddress { get; set; }
            public uint SubroutineInt;
            public ushort RamAddressInt;
        }
        public uint startAddress { get; set; }
        private PcmFile PCM;
        public List<PID> pidList; 

        private void searchPids()
        {
            pidList = new List<PID>();
            ushort prevPidNumber = 0;            
            for (uint addr = startAddress ; addr < PCM.fsize; addr += 8)
            {
                PID pid = readPID(addr);
                if (pid.PidNumber < prevPidNumber || pid.PidNumber == 0xFFFF)
                {
                    break;
                }
                else
                {
                    pidList.Add(pid);
                }
                prevPidNumber = pid.PidNumber;
            }

        }

        private PID readPID(uint addr)
        {
            PID pid = new PID();
            pid.PidNumber = BEToUint16(PCM.buf, addr);
            pid.Bytes = (ushort)(PCM.buf[addr + 2] + 1);
            if (pid.Bytes == 3)
                pid.Bytes = 4;
            pid.SubroutineInt = BEToUint32(PCM.buf, addr + 4);
            pid.Subroutine = pid.SubroutineInt.ToString("X8");
            uint ramStoreAddr = uint.MaxValue;
            if (pid.Bytes == 1)
                ramStoreAddr = searchBytes("10 38", pid.SubroutineInt, PCM.fsize, 0x4E75) ;
            else if (pid.Bytes == 2)
                ramStoreAddr = searchBytes("30 38", pid.SubroutineInt, PCM.fsize, 0x4E75);
            if (ramStoreAddr < uint.MaxValue)
            {
                pid.RamAddressInt = BEToUint16(PCM.buf, ramStoreAddr + 2);
                pid.RamAddress = pid.RamAddressInt.ToString("X4");
            }
            return pid;
        }
        private uint searchBytes(string searchString, uint Start, uint End, ushort stopVal=0 )
        {
            uint addr;
            string[] searchParts = searchString.Split(' ');

            for (addr=Start; addr < End; addr++)
            {
                bool match = true;
                for (uint part = 0; part < searchParts.Length; part++)
                {
                    if (stopVal != 0 && BEToUint16(PCM.buf,addr) == stopVal)
                    {
                        return uint.MaxValue;
                    }
                    if (searchParts[part] != "*")
                    {
                        byte searchval;
                        HexToByte(searchParts[part], out searchval);
                        if (PCM.buf[addr + part] != searchval)
                        {
                            match = false;
                            break;
                        }
                    }
                }
                if (match)
                {
                    return addr;
                }
            }
            return uint.MaxValue;
        }
    }
}
