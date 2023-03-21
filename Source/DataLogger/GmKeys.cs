using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    class GmKeys
    {
        public ulong GetKey(byte[] seed, byte algo)
        {
            byte[] result = gmkeylib.gmkey.GetKey(seed, (byte)algo);
            byte[] tmpBytes = new byte[8];
            Array.Copy(result, tmpBytes, 5);
            for (int i = 0; i < 5; i++) //Swap bytes
            {
                tmpBytes[7 - i] = tmpBytes[i];
            }
            Debug.WriteLine("Seed: " + BitConverter.ToString(seed) + ", Key: " + result.ToHex() + ", Algo: " + algo.ToString("X"));
            return BitConverter.ToUInt64(tmpBytes, 0);
        }
    }
}
