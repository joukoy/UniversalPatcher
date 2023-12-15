using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    class NgcKeys
    {
       
        public int unlockengine(int a1)
        {
            int v1; // ecx
            UInt16 v2; // ax
            uint v3; // ecx
            int v4; // eax
            short[] v6 = new short[8]; // [esp+0h] [ebp-14h]

            v1 = 0;
            v6[0] = unchecked((Int16)0x8A4F);
            v6[1] = unchecked((Int16)0x5245);
            v6[2] = unchecked((Int16)0x9308);
            v6[3] = unchecked((Int16)0xD997);
            v6[4] = unchecked((Int16)0xF4F5);
            v6[5] = unchecked((Int16)0xE324);
            v6[6] = unchecked((Int16)0xC76F);
            v6[7] = unchecked((Int16)0x5535);

/*            v6[0] = unchecked((Int16)0x4F8A);
            v6[1] = unchecked((Int16)0x4552);
            v6[2] = unchecked((Int16)0x0893);
            v6[3] = unchecked((Int16)0x97D9);
            v6[4] = unchecked((Int16)0xF5F4);
            v6[5] = unchecked((Int16)0x24E3);
            v6[6] = unchecked((Int16)0x6FC7);
            v6[7] = unchecked((Int16)0x3555);
*/
            if ((a1 & 1) != 0)
                v1 = 1;
            v2 = (UInt16)(a1 >> 1);
            if (v1 != 0)
                v2 = (UInt16)(v2 | 0x8000u);
            v3 = v2;
            v4 = (ushort)v6[(v2 >> 10) & 7];
            //LOWORD(v4) = v6[v3 & 7] ^ v6[v3 >> 13] ^ v6[(v3 >> 3) & 7] ^ v6[(v3 >> 7) & 7] ^ v4;
            ushort v4u = (ushort)(v6[v3 & 7] ^ v6[v3 >> 13] ^ v6[(v3 >> 3) & 7] ^ v6[(v3 >> 7) & 7] ^ v4);
            //v4 = (int)((v4 & 0xFFFF0000) | v4u);
            v4 = (int)(((v4 & 0xFFFF0000) << 16) | v4u);
            return a1 ^ v4 ^ 0x537E;
        }

    }
}
