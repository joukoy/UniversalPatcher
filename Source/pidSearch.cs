using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using static Upatcher;
using static Helpers;
using System.Windows.Forms;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class PidSearch
    {
        public PidSearch()
        {

        }
        public PidSearch(PcmFile PCM1)
        {
            PCM = PCM1;
            uint step = 8;

            if (PCM.platformConfig.PidSearchString != null && PCM.platformConfig.PidSearchString.Length > 0)
            {
                startAddress = SearchBytes(PCM, PCM.platformConfig.PidSearchString, 0, (uint)(PCM.fsize - PCM.platformConfig.PidSearchString.Length));
                step = PCM.platformConfig.PidSearchStep;
                if (startAddress < uint.MaxValue)
                    SearchPids(step, false);
                return;
            }

            if (PCM.configFile.StartsWith("diesel01"))
            {
                startAddress = SearchBytes(PCM, "00 00 04 * * * * * * * 00 01 04 * * * * * * * 00 02 02", 0, PCM.fsize - 23);
                step = 10;
                if (startAddress < uint.MaxValue)
                    SearchPids(step,true);
                return;
            }
            else if (PCM.configFile.StartsWith("p01"))
            {
                step = 8;
                startAddress = SearchBytes(PCM, "00 01 02 00 * * * * 00 03 01 00 * * * * 00 04 00 00 * * * * 00 05 00 00", 0, PCM.fsize - 28);
                if (startAddress < uint.MaxValue)
                    SearchPids(step, false);
                return;
            }
            else if (PCM.configFile.StartsWith("v6"))
            {
                startAddress = SearchBytes(PCM, "00 00 02 00 * * * * * * 00 01 02 00", 0, PCM.fsize - 14);
                step = 10;
                if (startAddress < uint.MaxValue)
                    SearchPids(step, false);
                return;
            }
            else
            {
                string cnfFile = PCM.configFile;
                foreach (PidSearchConfig psc in pidSearchConfigs)
                {
                    if (psc.XMLFile.ToLower() == cnfFile)
                    {
                        startAddress = SearchBytes(PCM, psc.SearchString, 0, (uint)(PCM.fsize - psc.SearchString.Length));
                        if (startAddress < uint.MaxValue)
                            SearchPids(step, false);
                        return;

                    }
                }
            }
            Logger("PID search not implemented for this file type");
        }

        public class PID
        {
            public string PidNumber
            {
                get
                {
                    if (pidNumberInt == ushort.MaxValue)
                        return "";
                    else
                        return pidNumberInt.ToString("X4");
                }
                set
                {
                    if (value.Length > 0)
                    {
                        ushort prevVal = pidNumberInt;
                        if (!HexToUshort(value.Replace("0x", ""), out pidNumberInt))
                            pidNumberInt = prevVal;
                    }
                    else
                    {
                        pidNumberInt = ushort.MaxValue;
                    }
                }
            }

            //public ushort Bytes { get; set; }
            public InDataType DataType { get; set; }
            public string  Subroutine
            {
                get
                {
                    if (SubroutineInt == uint.MaxValue)
                        return "";
                    else
                        return SubroutineInt.ToString("X4");
                }
                set
                {
                    if (value.Length > 0)
                    {
                        UInt32 prevVal = SubroutineInt;
                        if (!HexToUint(value.Replace("0x", ""), out SubroutineInt))
                            SubroutineInt = prevVal;
                    }
                    else
                    {
                        SubroutineInt = uint.MaxValue;
                    }
                }
            }
            public string RamAddress
            {
                get
                {
                    if (RamAddressInt == uint.MaxValue)
                        return "";
                    else
                        return RamAddressInt.ToString("X8");
                }
                set
                {
                    if (value.Length > 0)
                    {
                        UInt32 prevVal = RamAddressInt;
                        if (!HexToUint(value.Replace("0x", ""), out RamAddressInt))
                            RamAddressInt = prevVal;
                    }
                    else
                    {
                        RamAddressInt = uint.MaxValue;
                    }
                }
            }
            public ushort pidNumberInt;
            public uint SubroutineInt;
            public uint RamAddressInt;
            public string Name { get; set; }
            public string ConversionFactor { get; set; }
            public string Description { get; set; }
            //public bool Signed { get; set; }
            public string Unit { get; set; }
            public string Scaling { get; set; }
        }

        public uint startAddress { get; set; }
        private PcmFile PCM;
        public List<PID> pidList;

        private void SearchPids(uint step, bool diesel)
        {
            pidList = new List<PID>();
            ushort prevPidNumber = 0;            
            for (uint addr = startAddress ; addr < PCM.fsize; addr += step)
            {
                PID pid;
                if (diesel)
                    pid = ReadPidDiesel(addr);
                else
                    pid = ReadPid(addr);
                if (pid.pidNumberInt < prevPidNumber || pid.pidNumberInt == 0xFFFF)
                {
                    break;
                }
                else
                {
                    pidList.Add(pid);
                }
                prevPidNumber = pid.pidNumberInt;
            }
            if (pidList.Count > 0)
            {
                string ospidfile = Path.Combine(Application.StartupPath, "Logger", "ospids", PCM.OS + ".txt");
                if (!File.Exists(ospidfile))
                {
                    StringBuilder sb = new StringBuilder();
                    for (int p=0;p<pidList.Count;p++)
                    {
                        sb.AppendLine(pidList[p].PidNumber);
                    }
                    WriteTextFile(ospidfile, sb.ToString());
                }
            }
        }

        private PID ReadPid(uint addr)
        {
            PID pid = new PID();
            pid.pidNumberInt = PCM.ReadUInt16(addr);
            int bytes = (ushort)(PCM.buf[addr + 2] + 1);
            pid.DataType = ConvertToDataType(bytes, false, false);
            pid.SubroutineInt = PCM.ReadUInt32(addr + 4);
            uint ramStoreAddr = SearchBytes(PCM, "10 38", pid.SubroutineInt, PCM.fsize, 0x4E75);
            if (ramStoreAddr == uint.MaxValue)
                ramStoreAddr = SearchBytes(PCM, "30 38", pid.SubroutineInt, PCM.fsize, 0x4E75);
            if (ramStoreAddr < uint.MaxValue)
            {
                pid.RamAddressInt = PCM.ReadUInt16(ramStoreAddr + 2);
            }
            PidInfo pi = PidDescriptions.Where(x => x.PidNumber == pid.pidNumberInt).FirstOrDefault();
            if (pi != null)
            {
                pid.Name = pi.PidName;
                pid.ConversionFactor = pi.ConversionFactor;
                pid.Description = pi.Description;
                pid.Scaling = pi.Scaling;
                pid.Unit = pi.Unit;
                pid.DataType = pi.DataType;
                //pid.Signed = pi.Signed;
            }
            return pid;
        }

        private PID ReadPidDiesel(uint addr)
        {
            PID pid = new PID();
            pid.pidNumberInt = PCM.ReadUInt16(addr);
            int bytes = (ushort)(PCM.buf[addr + 2]);
            if (bytes > 4)
                bytes = 0;
            pid.DataType = ConvertToDataType(bytes, false, false);
            pid.SubroutineInt = PCM.ReadUInt32(addr + 6);
            pid.RamAddressInt = SearchRamAddressDiesel(pid.SubroutineInt);
            PidInfo pi = PidDescriptions.Where(x => x.PidNumber == pid.pidNumberInt).FirstOrDefault();
            if (pi != null)
            {
                pid.Name = pi.PidName;
                pid.ConversionFactor = pi.ConversionFactor;
                pid.Description = pi.Description;
                pid.Scaling = pi.Scaling;
                pid.Unit = pi.Unit;
                pid.DataType = pi.DataType;
                //pid.Signed = pi.Signed;
            }
            return pid;
        }
        private uint SearchRamAddressDiesel(uint startAddr)
        {
            uint addr = uint.MaxValue;
            uint ramAddress = uint.MaxValue;
            uint skipWord1 = 0xff8f0a;
            uint skipWord2 = 0xff8a7c;

            addr = SearchBytes(PCM, "10 39 * * * * 55 00 67 06", startAddr, PCM.fsize);
            if (addr != uint.MaxValue)
            {
                skipWord1 = PCM.ReadUInt32(addr + 2);
            }
            addr = SearchBytes(PCM, "12 39 * * * * 41 EE FF", startAddr, PCM.fsize);
            if (addr != uint.MaxValue)
            {
                skipWord2 = PCM.ReadUInt32(addr + 2);
            }

            ushort[] wordList = { 0x1039, 0x3039, 0x1239, 0x3239, 0x1e39, 0x3e39, 0x20b9, 0x33f9 };

            for (int s=0;s< wordList.Length;s++)
            {
                for (addr = startAddr; addr < PCM.fsize-1; addr++)
                {
                    if (PCM.ReadUInt16(addr) == 0x4E75)   //End of subroutine
                    {
                        break;
                    }
                    if (PCM.ReadUInt16(addr) == wordList[s]) 
                    {
                        ramAddress = PCM.ReadUInt32(addr + 2);
                        Debug.WriteLine("Searchword: " + wordList[s].ToString("X2") + " found in: " + addr.ToString("X") + " Data: " + ramAddress.ToString("X"));
                        if (ramAddress != skipWord1 && ramAddress != skipWord2)
                            return ramAddress;
                        Debug.WriteLine("Skipped");
                    }
                }
            }
            return uint.MaxValue;
        }
    }

    public class PidSearchConfig
    {
        public string XMLFile { get; set; }
        public string SearchString { get; set; }
        public int Step { get; set; }

        public PidSearchConfig ShallowCopy()
        {
            return (PidSearchConfig)this.MemberwiseClone();
        }
    }
}
