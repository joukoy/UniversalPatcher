using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using static upatcher;
using System.Windows.Forms;
using System.Diagnostics;

namespace UniversalPatcher.Properties
{
    public class PidSearch
    {
        public PidSearch(PcmFile PCM1)
        {
            PCM = PCM1;
            uint step = 8;
            loadPidList();
            if (PCM.xmlFile.StartsWith("diesel01"))
            {
                startAddress = searchBytes(PCM, "00 00 04 * * * * * * * 00 01 04 * * * * * * * 00 02 02", 0, PCM.fsize - 23);
                step = 10;
                if (startAddress < uint.MaxValue)
                    searchPids(step,true);
                return;
            }

            if (PCM.xmlFile.StartsWith("p01"))
            {
                step = 8;
                startAddress = searchBytes(PCM, "00 01 02 00 * * * * 00 03 01 00 * * * * 00 04 00 00 * * * * 00 05 00 00", 0, PCM.fsize - 28);
                if (startAddress < uint.MaxValue)
                    searchPids(step,false);
                return;
            }
            if (PCM.xmlFile.StartsWith("v6"))
            {
                startAddress = searchBytes(PCM, "00 00 02 00 * * * * * * 00 01 02 00", 0, PCM.fsize - 14);
                step = 10;
                if (startAddress < uint.MaxValue)
                    searchPids(step,false);
                return;
            }
            throw new Exception("PID search not implemented for this file type");
        }
        public class pidName
        {
            public uint PidNumber { get; set; }
            public string PidName { get; set; }
            public string ConversionFactor { get; set; }
        }
        public class PID
        {
            public string PidNumber { get; set; }
            public ushort Bytes { get; set; }
            public string  Subroutine { get; set; }
            public string  RamAddress { get; set; }
            public ushort pidNumberInt;
            public uint SubroutineInt;
            public uint RamAddressInt;
            public string Name { get; set; }
            public string ConversionFactor { get; set; }
        }
        public uint startAddress { get; set; }
        private PcmFile PCM;
        public List<PID> pidList;
        public List<pidName> pidNameList;

        private void searchPids(uint step, bool diesel)
        {
            pidList = new List<PID>();
            ushort prevPidNumber = 0;            
            for (uint addr = startAddress ; addr < PCM.fsize; addr += step)
            {
                PID pid;
                if (diesel)
                    pid = readPidDiesel(addr);
                else
                    pid = readPid(addr);
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

        }

        private PID readPid(uint addr)
        {
            PID pid = new PID();
            pid.pidNumberInt = BEToUint16(PCM.buf, addr);
            pid.PidNumber = pid.pidNumberInt.ToString("X4");
            pid.Bytes = (ushort)(PCM.buf[addr + 2] + 1);
            if (pid.Bytes == 3)
                pid.Bytes = 4;
            pid.SubroutineInt = BEToUint32(PCM.buf, addr + 4);
            pid.Subroutine = pid.SubroutineInt.ToString("X8");
            uint ramStoreAddr = searchBytes(PCM, "10 38", pid.SubroutineInt, PCM.fsize, 0x4E75);
            if (ramStoreAddr == uint.MaxValue)
                ramStoreAddr = searchBytes(PCM, "30 38", pid.SubroutineInt, PCM.fsize, 0x4E75);
            if (ramStoreAddr < uint.MaxValue)
            {
                pid.RamAddressInt = BEToUint16(PCM.buf, ramStoreAddr + 2);
                pid.RamAddress = pid.RamAddressInt.ToString("X4");
            }
            for (int p=0; p< pidNameList.Count;p++)
            {
                if (pidNameList[p].PidNumber == pid.pidNumberInt)
                {
                    pid.Name = pidNameList[p].PidName;
                    pid.ConversionFactor = pidNameList[p].ConversionFactor;
                    break;
                }
            }
            return pid;
        }
        private PID readPidDiesel(uint addr)
        {
            PID pid = new PID();
            pid.pidNumberInt = BEToUint16(PCM.buf, addr);
            pid.PidNumber = pid.pidNumberInt.ToString("X4");
            pid.Bytes = (ushort)(PCM.buf[addr + 2]);
            if (pid.Bytes > 4)
                pid.Bytes = 0;
            pid.SubroutineInt = BEToUint32(PCM.buf, addr + 6);
            pid.Subroutine = pid.SubroutineInt.ToString("X8");
            pid.RamAddressInt = searchRamAddressDiesel(pid.SubroutineInt);
            if (pid.RamAddressInt != uint.MaxValue)
                pid.RamAddress = pid.RamAddressInt.ToString("X4");
            else
                pid.RamAddress = "";
            for (int p = 0; p < pidNameList.Count; p++)
            {
                if (pidNameList[p].PidNumber == pid.pidNumberInt)
                {
                    pid.Name = pidNameList[p].PidName;
                    pid.ConversionFactor = pidNameList[p].ConversionFactor;
                    break;
                }
            }
            return pid;
        }
        private uint searchRamAddressDiesel(uint startAddr)
        {
            uint addr = uint.MaxValue;
            uint ramAddress = uint.MaxValue;
            uint skipWord1 = 0xff8f0a;
            uint skipWord2 = 0xff8a7c;

            addr = searchBytes(PCM, "10 39 * * * * 55 00 67 06", startAddr, PCM.fsize);
            if (addr != uint.MaxValue)
            {
                skipWord1 = BEToUint32(PCM.buf, addr + 2);
            }
            addr = searchBytes(PCM, "12 39 * * * * 41 EE FF", startAddr, PCM.fsize);
            if (addr != uint.MaxValue)
            {
                skipWord2 = BEToUint32(PCM.buf, addr + 2);
            }

            ushort[] wordList = { 0x1039, 0x3039, 0x1239, 0x3239, 0x1e39, 0x3e39, 0x20b9, 0x33f9 };

            for (int s=0;s< wordList.Length;s++)
            {
                for (addr = startAddr; addr < PCM.fsize-1; addr++)
                {
                    if (BEToUint16(PCM.buf, addr) == 0x4E75)   //End of subroutine
                    {
                        break;
                    }
                    if (BEToUint16(PCM.buf, addr) == wordList[s]) 
                    {
                        ramAddress = BEToUint32(PCM.buf, addr + 2);
                        Debug.WriteLine("Searchword: " + wordList[s].ToString("X2") + " found in: " + addr.ToString("X") + " Data: " + ramAddress.ToString("X"));
                        if (ramAddress != skipWord1 && ramAddress != skipWord2)
                            return ramAddress;
                        Debug.WriteLine("Skipped");
                    }
                }
            }
            return uint.MaxValue;
        }

        public void loadPidList()
        {
            string FileName = Path.Combine(Application.StartupPath, "XML", "pidlist.xml");
            if (!File.Exists(FileName))
                return;
            pidNameList = new List<pidName>();
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<pidName>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            pidNameList = (List<pidName>)reader.Deserialize(file);
            file.Close();
        }

    }
}
