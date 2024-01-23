using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using static Upatcher;
using static Helpers;
using System.IO;

namespace UniversalPatcher
{
    public class ChecksumSearcher
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CheckSumSearch(IntPtr Buffer, IntPtr Settings);
        private static CheckSumSearch CkSearch;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool ChecksumSearchIsRunning();
        private static ChecksumSearchIsRunning CkIsRunning;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate CkResult ChecksumSearchGetResults();
        private static ChecksumSearchGetResults CkGetResults;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int ChecksumSearchGetQueueSize();
        private static ChecksumSearchGetQueueSize CkGetQueueSize;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ChecksumSearchStop();
        private static ChecksumSearchStop ckStop;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct CkResult
        {
            public UInt32 Start;
            public UInt32 End;
            public UInt32 CsAddress;
            public UInt64 Cheksum;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SearchSettings
        {
            public UInt32 Start;
            public UInt32 End;
            public UInt32 MinRangeLen;
            public int Method;
            public int Complement;
            public int CsBytes;
            public bool MSB;
            public bool SwapBytes;
            public bool SkipCsAddress;
            public int Threads;
            public UInt64 InitialValue;
            public int CsAddressCount;
            public IntPtr CsAddresses;
        }
        public class CkSearchResult
        {
            public CkSearchResult() { }
            public CkSearchResult(uint start,uint end,uint csaddress,UInt64 csum) 
            {
                this.Start = start;
                this.End = end;
                this.CsAddress = csaddress;
                this.Cheksum = csum;
            }
            public CkSearchResult(CkResult result)
            {
                this.Start = result.Start;
                this.End = result.End;
                this.CsAddress = result.CsAddress;
                this.Cheksum = result.Cheksum;
            }
            public bool Select { get; set; }
            public UInt32 Start { get; set; }
            public UInt32 End { get; set; }
            public UInt32 CsAddress { get; set; }
            public UInt64 Cheksum { get; set; }
        }

        public enum RangeType
        {
            Exact,
            SearchRange,
            AfterRange,
            All
        }
        private IntPtr UintArrayToPtr(uint[] buff)
        {
            //Create a blob big enough for all elements
            IntPtr Ptr = Marshal.AllocHGlobal(buff.Length * Marshal.SizeOf(typeof(uint)));
            //Set array length: (First value in structure)
            byte[] tmp = new byte[buff.Length * sizeof(uint)];
            for (int i = 0; i < buff.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(buff[i]), 0, tmp, i * sizeof(uint), sizeof(uint));
            }
            Marshal.Copy(tmp, 0, Ptr, tmp.Length);
            return Ptr;
        }

        public class CsUtilMethod
        {
            public CsUtilMethod()
            {
                SkipCsAddress = true;
            }
            public CsUtilMethod(CSMethod method)
            {
                this.csMethod = method;
                SkipCsAddress = true;
            }
            public bool Enable { get; set; }
            public string Method
            {
                get { return csMethod.ToString(); }
                set { this.csMethod = (CSMethod)Enum.Parse(typeof(CSMethod), value); }
            }
            public CSBytes CsBytes { get; set; }
            public bool UseBoschAddresses { get; set; }
            public bool SkipCsAddress { get; set; }

            private CSMethod csMethod;
            public CSMethod ChecksumMethod()
            {
                return csMethod;
            }
        }
        public class ChecksumSearchSettings
        {
            public string CalcRange { get; set; }
            public RangeType searchRangeType { get; set; }
            public string Exclude { get; set; }
            public uint MinRangeLen { get; set; }
            public List<CsUtilMethod> Methods { get; set; }
            public ushort Complement { get; set; }
            public string CSAddress { get; set; }
            public RangeType CSAddressType { get; set; }
            public bool BoschFilter0F { get; set; }
            public bool MSB { get; set; }
            public bool SwapBytes { get; set; }
            public UInt64 InitialValue { get; set; }
            public uint Threads { get; set; }
        }

        private IntPtr pDll;
        private IntPtr pnt;
        private IntPtr settingsPnt;
        public int QueueStartSize;
        public bool LoadCkLibrary()
        {
            pDll = LoadLibrary("uptools.dll");

            if (pDll == IntPtr.Zero)
                return false;
            IntPtr pAddressOfFunctionToCall = GetProcAddress(pDll, "ChecksumSearchIsRunning");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                CkIsRunning = (ChecksumSearchIsRunning)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(ChecksumSearchIsRunning));
            pAddressOfFunctionToCall = GetProcAddress(pDll, "CheckSumSearch");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                CkSearch = (CheckSumSearch)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(CheckSumSearch));
            pAddressOfFunctionToCall = GetProcAddress(pDll, "ChecksumSearchGetResults");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                CkGetResults = (ChecksumSearchGetResults)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(ChecksumSearchGetResults));
            pAddressOfFunctionToCall = GetProcAddress(pDll, "ChecksumSearchGetQueueSize");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                CkGetQueueSize = (ChecksumSearchGetQueueSize)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(ChecksumSearchGetQueueSize));
            pAddressOfFunctionToCall = GetProcAddress(pDll, "ChecksumSearchStop");
            if (pAddressOfFunctionToCall != IntPtr.Zero)
                ckStop = (ChecksumSearchStop)Marshal.GetDelegateForFunctionPointer(
                                                                                    pAddressOfFunctionToCall,
                                                                                    typeof(ChecksumSearchStop));

            return true;
        }

        public void FreeCkLibrary()
        {
            FreeLibrary(pDll);
            Marshal.FreeHGlobal(pnt);
            Marshal.FreeHGlobal(settingsPnt);
        }

        public void StartCkCalc(byte[] buf, uint start, uint end, uint minrangelen, CSMethod method, int complement,int CsBytes, bool MSB, bool SwapBytes, bool SkipCsAddr, int threads,UInt64 InitialValue, uint[] CsAddresses)
        {
            pnt = Marshal.AllocHGlobal(buf.Length + 16);
            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(buf, 0, pnt, buf.Length);
                SearchSettings searchSettings = new SearchSettings();
                searchSettings.Complement = complement;
                searchSettings.CsAddressCount = CsAddresses.Length;
                searchSettings.CsAddresses = UintArrayToPtr(CsAddresses);
                searchSettings.CsBytes = CsBytes;
                searchSettings.End = end;
                searchSettings.InitialValue = InitialValue;
                searchSettings.Method = (int)method;
                searchSettings.MinRangeLen = minrangelen;
                searchSettings.MSB = MSB;
                searchSettings.SkipCsAddress = SkipCsAddr;
                searchSettings.Start = start;
                searchSettings.SwapBytes = SwapBytes;
                searchSettings.Threads = threads;
                int ptrLen = Marshal.SizeOf(searchSettings) + CsAddresses.Length * sizeof(uint);
                settingsPnt = Marshal.AllocHGlobal(ptrLen);
                Marshal.StructureToPtr(searchSettings, settingsPnt, false);
                QueueStartSize = CkSearch(pnt, settingsPnt);
            }
            finally
            {
                // Free the unmanaged memory. AFTER UNLOADING DLL!
                //Marshal.FreeHGlobal(pnt);
            }

        }

        public bool isRunning()
        {
            return CkIsRunning();
        }

        public string GetResultStr(bool FilterZero)
        {
            StringBuilder sb = new StringBuilder();
            CkResult result = CkGetResults();            
            while (result.Start < uint.MaxValue)
            {
                if (result.Cheksum != 0 || FilterZero == false)
                {
                    sb.Append("Range: " + result.Start.ToString("X8") + " - " + result.End.ToString("X8") + ", Address: " + result.CsAddress.ToString("X8") + ", result: " + result.Cheksum.ToString("X") + Environment.NewLine);
                }
                result = CkGetResults();
            }
            return sb.ToString();
        }

        public List<CkSearchResult> GetResults(bool FilterZero)
        {
            List<CkSearchResult> results = new List<CkSearchResult>();
            CkResult result = CkGetResults();
            while (result.Start < uint.MaxValue)
            {
                if (result.Cheksum != 0 || FilterZero == false)
                {
                    CkSearchResult cr = new CkSearchResult(result);
                    results.Add(cr);
                }
                result = CkGetResults();
            }
            return results;
        }
        public int GetQueueSize()
        {
            return CkGetQueueSize();
        }
        public void Stop()
        {
            ckStop();
        }

        public static void SaveSettings(ChecksumSearchSettings settings, string fName)
        {
            try
            {
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(ChecksumSearchSettings));
                    writer.Serialize(stream, settings);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
        public static ChecksumSearchSettings LoadSettings(string fName)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(ChecksumSearchSettings));
                System.IO.StreamReader file = new System.IO.StreamReader(fName);
                ChecksumSearchSettings settings = (ChecksumSearchSettings)reader.Deserialize(file);
                file.Close();
                return settings;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                return null;
            }
        }
    }
}
