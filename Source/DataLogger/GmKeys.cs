using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Helpers;
using System.Runtime.InteropServices;
using System.Reflection;

namespace UniversalPatcher
{
    class GmKeys
    {
        public UInt64 Get5byteKey(byte[] seed, byte algo)
        {
            try
            {
                byte[] result = gmkeylib.gmkey.GetKey(seed, (byte)algo);
                if (result == null)
                {
                    LoggerBold("Invalid license for GM 5 byte keys");
                    return UInt64.MaxValue;
                }
                if (result.Length < 5)
                {
                    if (result[0] == 2)
                    {
                        LoggerBold("Invalid seed value for GM 5 byte keys");
                    }
                    else if (result[0] == 0xFE)
                    {
                        LoggerBold("Invalid license for GM 5 byte keys");
                    }
                    else
                    {
                        LoggerBold("Error (" + result[0].ToString() + ") in query, or invalid license for GM 5 byte keys");
                    }
                    return UInt64.MaxValue;
                }
                Debug.WriteLine("Seed: " + BitConverter.ToString(seed) + ", Key: " + result.ToHex() + ", Algo: " + algo.ToString("X"));
                byte[] retVal = new byte[8];
                for (int i=0;i<5;i++)
                {
                    retVal[i + 3] = result[i];
                }
                return ReadUint64(retVal, 0,true);
            }
            catch(Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, GmKeys , line " + line + ": " + ex.Message);
                return UInt64.MaxValue;
            }
        }
    }
}
