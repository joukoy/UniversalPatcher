using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using static Upatcher;
using static Helpers;
using System.IO;
using System.Windows.Forms;

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
        public delegate int CheckSumSearch(IntPtr Buffer, IntPtr Settings, IntPtr ChekcsumValues, IntPtr FilterValues);
        private static CheckSumSearch CkSearch;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool ChecksumSearchIsRunning();
        private static ChecksumSearchIsRunning CkIsRunning;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ChecksumSearchGetResults(out CkResult result);
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
            public int Method;
            public byte Complement;
            public bool ByteSwap;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct SearchSettings
        {
            public UInt32 Start;
            public UInt32 End;
            public UInt32 MinRangeLen;
            public int Method;
            public byte Complement;
            public int CsBytes;
            public bool MSB;
            public bool SwapBytes;
            public bool NoSwapBytes;
            public bool SkipCsAddress;
            public int CsValueCount;
            public int Threads;
            public UInt64 InitialValue;
            public UInt64 Polynomial;
            public UInt64 Xor;
            public int FilterCount;
            public int CsAddressCount;
            public IntPtr CsAddresses;
        }
        public class CkSearchResult
        {
            public CkSearchResult() 
            {
                this.useValue = false;
            }
            public CkSearchResult(uint start,uint end,uint csaddress,UInt64 csum, bool useVal, CSMethod method, byte complement, bool byteswap, UInt64 Poly, UInt64 Xor) 
            {
                this.Start = start;
                this.End = end;
                this.csAddr = csaddress;
                this.Cheksum = csum;
                this.useValue = useVal;
                this.Method = method;
                this.Complement = complement;
                this.ByteSwap = byteswap;
                this.poly = Poly;
                this.Xor = Xor;
            }
            public CkSearchResult(CkResult result,bool useVal)
            {
                this.Start = result.Start;
                this.End = result.End;
                this.csAddr = result.CsAddress;
                this.Cheksum = result.Cheksum;
                this.useValue = useVal;
                this.Method = (CSMethod)result.Method;
                this.Complement = result.Complement;
                this.ByteSwap = result.ByteSwap;
            }
            public bool Select { get; set; }
            public UInt32 Start { get; set; }
            public UInt32 End { get; set; }
            public string CsAddress 
            {
                get 
                { 
                    if (useValue || csAddr == uint.MaxValue)
                    {
                        return "";
                    }
                    else
                    {
                        return csAddr.ToString("X");
                    }
                }
                set 
                {
                    if (string.IsNullOrEmpty(value) || useValue)
                    {
                        csAddr = uint.MaxValue;
                    }
                    else
                    {
                        HexToUint(value, out csAddr);
                    }
                } 
            }
            public UInt64 Cheksum { get; set; }
            public UInt64 Xor { get; set; }
            public string Poly
            {
                get
                {
                    if (poly == UInt64.MaxValue) return "";
                    return poly.ToString("X");
                }
                set 
                {
                    if (string.IsNullOrEmpty(value))
                        poly = UInt64.MaxValue;
                    else
                        HexToUint64(value, out poly); 
                }
            }
            public CSMethod Method { get; set; }
            public byte Complement { get; set; }
            public bool ByteSwap { get; set; }

            private bool useValue;
            private UInt32 csAddr;
            private UInt64 poly;
        }

        public enum RangeType
        {
            Exact,
            SearchRange,
            AfterRange,
            UseValue,
            All
        }

        public class CsUtilMethod
        {
            public CsUtilMethod()
            {
                this.Method = CSMethod.Wordsum;
                this.CsBytes = CSBytes._4;
                this.MSB = true;
                this.NoSwapBytes = true;
                this.Complement0 = true;
                SkipCsAddress = true;
                poly = UInt64.MaxValue;
                Xor = 0;
            }
            public CsUtilMethod(CSMethod method)
            {
                this.Method = method;
                this.CsBytes = CSBytes._4;
                this.MSB = true;
                this.NoSwapBytes = true;
                this.Complement0 = true;
                SkipCsAddress = true;
                poly = UInt64.MaxValue;
                Xor = 0;
            }
            public CSMethod Method { get; set; }
            public CSBytes CsBytes { get; set; }
            public string Poly 
            {
                get
                {
                    if (poly == UInt64.MaxValue) return "";
                    return poly.ToString("X");
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
                        poly = UInt64.MaxValue;
                    else
                        HexToUint64(value, out poly);
                }
            }
            public UInt64 Xor { get; set; }
            public bool SkipCsAddress { get; set; }

            private UInt64 poly;
            public UInt64 InitialValue { get; set; }
            public UInt64 Polynomial()
            {
                return poly;
            }
            public bool UseBoschAddresses { get; set; }
            public bool BoschFilter0F { get; set; }
            public bool MSB { get; set; }
            public bool LSB { get; set; }
            public bool SwapBytes { get; set; }
            public bool NoSwapBytes { get; set; }
            public bool Complement0 { get; set; }
            public bool Complement1 { get; set; }
            public bool Complement2 { get; set; }
        }
        public class ChecksumSearchSettings
        {
            public string CalcRange { get; set; }
            public RangeType searchRangeType { get; set; }
            public string Exclude { get; set; }
            public uint MinRangeLen { get; set; }
            public List<CsUtilMethod> Methods { get; set; }
            public List<byte> Complements { get; set; }
            public string CSAddress { get; set; }
            public RangeType CSAddressType { get; set; }
            public bool ShowResults { get; set; }
            public uint Threads { get; set; }
            public string FilterValues { get; set; }
            public bool MSB { get; set; }
        }
        private IntPtr pDll;
        private IntPtr buffPtr;
        private IntPtr settingsPtr;
        private IntPtr valuesPtr;
        private IntPtr filterPtr;
        public int QueueStartSize;
        private bool useCsValue;
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
            Marshal.FreeHGlobal(buffPtr);
            Marshal.FreeHGlobal(settingsPtr);
            Marshal.FreeHGlobal(valuesPtr);
            Marshal.FreeHGlobal(filterPtr);
        }
        private IntPtr Uint64ArrayToPtr(UInt64[] buff)
        {
            //Create a blob big enough for all elements
            IntPtr Ptr = Marshal.AllocHGlobal(buff.Length * Marshal.SizeOf(typeof(UInt64)));
            byte[] tmp = new byte[buff.Length * sizeof(UInt64)];
            for (int i = 0; i < buff.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(buff[i]), 0, tmp, i * sizeof(UInt64), sizeof(UInt64));
            }
            Marshal.Copy(tmp, 0, Ptr, tmp.Length);
            return Ptr;
        }
        private IntPtr UintArrayToPtr(uint[] buff)
        {
            //Create a blob big enough for all elements
            IntPtr Ptr = Marshal.AllocHGlobal(buff.Length * Marshal.SizeOf(typeof(uint)));
            byte[] tmp = new byte[buff.Length * sizeof(uint)];
            for (int i = 0; i < buff.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(buff[i]), 0, tmp, i * sizeof(uint), sizeof(uint));
            }
            Marshal.Copy(tmp, 0, Ptr, tmp.Length);
            return Ptr;
        }
        private IntPtr ByteArrayToPtr(byte[] buff)
        {
            //Create a blob big enough for all elements
            IntPtr Ptr = Marshal.AllocHGlobal(buff.Length);
            Marshal.Copy(buff, 0, Ptr, buff.Length);
            return Ptr;
        }

        //public void StartCkCalc(byte[] buf, uint start, uint end, uint minrangelen, CSMethod method, List<byte> complements,int CsBytes, bool MSB, bool SwapBytes, bool NoSwapBytes, bool SkipCsAddr, string CsValues, int threads,UInt64 InitialValue, UInt64[] FilterValues, uint[] CsAddresses)
        public void StartCkCalc(byte[] buf, CsUtilMethod cum,  ChecksumSearchSettings cksSettings, uint[] csAddresses)
        {
            try
            {                
                buffPtr = Marshal.AllocHGlobal(buf.Length + 16);
                // Copy the array to unmanaged memory.
                Marshal.Copy(buf, 0, buffPtr, buf.Length);
                List<UInt64> csvalues = new List<UInt64>();
                if (cksSettings.CSAddressType == RangeType.UseValue && !string.IsNullOrEmpty(cksSettings.CSAddress))
                {
                    this.useCsValue = true;
                    string[] csvParts = cksSettings.CSAddress.Split(',');
                    foreach(string cPart in csvParts)
                    {
                        if (HexToUint64(cPart,out UInt64 csVal))
                            csvalues.Add(csVal);
                    }
                    valuesPtr = Uint64ArrayToPtr(csvalues.ToArray());
                }
                else
                {
                    this.useCsValue = false;
                }
                List<ulong> FilterValues = new List<ulong>();
                if (!string.IsNullOrEmpty(cksSettings.FilterValues))
                {
                    string[] fParts = cksSettings.FilterValues.Split(',');
                    foreach (string fPart in fParts)
                    {
                        if (HexToUint64(fPart, out ulong x))
                        {
                            FilterValues.Add(x);
                        }
                    }
                    filterPtr = Uint64ArrayToPtr(FilterValues.ToArray());
                }
                SearchSettings searchSettings = new SearchSettings();
                searchSettings.Complement = 0;
                if (cum.Complement0)
                    searchSettings.Complement = (byte)(searchSettings.Complement | 4);
                if (cum.Complement1)
                    searchSettings.Complement = (byte)(searchSettings.Complement | 1);
                if (cum.Complement2)
                    searchSettings.Complement = (byte)(searchSettings.Complement | 2);

                uint start = 0;
                uint end = frmpatcher.basefile.fsize;
                List<Block> limitBlocks;
                if (cksSettings.searchRangeType != RangeType.All && !string.IsNullOrEmpty(cksSettings.CalcRange) && ParseAddress(cksSettings.CalcRange, frmpatcher.basefile, out limitBlocks))
                {
                    start = limitBlocks[0].Start;
                    end = limitBlocks.LastOrDefault().End;
                }

                searchSettings.CsAddressCount = csAddresses.Length;
                searchSettings.CsAddresses = UintArrayToPtr(csAddresses);
                searchSettings.FilterCount = FilterValues.Count;
                searchSettings.CsBytes = (int)cum.CsBytes;
                searchSettings.End = end;
                searchSettings.InitialValue = cum.InitialValue;
                searchSettings.Method = (int)cum.Method;
                searchSettings.MinRangeLen = (uint)cksSettings.MinRangeLen * (end - start) / 100;
                searchSettings.SkipCsAddress = cum.SkipCsAddress;
                searchSettings.Start = start;
                searchSettings.Threads = (int)cksSettings.Threads;
                searchSettings.CsValueCount = csvalues.Count;
                searchSettings.Polynomial = cum.Polynomial();
                searchSettings.Xor = cum.Xor;
                searchSettings.SwapBytes = cum.SwapBytes;
                searchSettings.NoSwapBytes = cum.NoSwapBytes;
                searchSettings.MSB = cksSettings.MSB;
                int ptrLen = Marshal.SizeOf(searchSettings) + csAddresses.Length * sizeof(uint);
                settingsPtr = Marshal.AllocHGlobal(ptrLen);
                Marshal.StructureToPtr(searchSettings, settingsPtr, false);
                QueueStartSize = CkSearch(buffPtr, settingsPtr, valuesPtr, filterPtr);
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
            CkResult result;
            CkGetResults(out result);            
            while (result.Start < uint.MaxValue)
            {
                if (result.Cheksum != 0 || FilterZero == false)
                {
                    sb.Append("Range: " + result.Start.ToString("X8") + " - " + result.End.ToString("X8") + ", Address: " + result.CsAddress.ToString("X8") + ", result: " + result.Cheksum.ToString("X") + Environment.NewLine);
                }
                CkGetResults(out result);
            }
            return sb.ToString();
        }

        public List<CkSearchResult> GetResults(List<ulong> FilterValues)
        {
            CkResult result;
            List<CkSearchResult> results = new List<CkSearchResult>();
            CkGetResults(out result);
            while (result.Start < uint.MaxValue)
            {
                if (!FilterValues.Contains(result.Cheksum))
                {
                    CkSearchResult cr = new CkSearchResult(result, useCsValue);
                    results.Add(cr);
                }
                CkGetResults(out result);
                if (results.Count > 100)
                {
                    break;
                }
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
