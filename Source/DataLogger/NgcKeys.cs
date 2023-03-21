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
            UInt16[] v6 = new UInt16[8]; // [esp+0h] [ebp-14h]

            v1 = 0;
            v6[0] = 0x8A4F;
            v6[1] = 0x5245;
            v6[2] = 0x9308;
            v6[3] = 0xD997;
            v6[4] = 0xF4F5;
            v6[5] = 0xE324;
            v6[6] = 0xC76F;
            v6[7] = 0x5535;
            if ((a1 & 1) != 0)
                v1 = 1;
            v2 = (UInt16)(a1 >> 1);
            if (v1 > 0)
                v2 = (UInt16)(v2 | 0x8000u);
            v3 = v2;
            v4 = v6[(v2 >> 10) & 7];
            //LOWORD(v4) = v6[v3 & 7] ^ v6[v3 >> 13] ^ v6[(v3 >> 3) & 7] ^ v6[(v3 >> 7) & 7] ^ v4;
            ushort v4u = (ushort)(v6[v3 & 7] ^ v6[v3 >> 13] ^ v6[(v3 >> 3) & 7] ^ v6[(v3 >> 7) & 7] ^ v4);
            v4 = (int)((v4 & 0xFFFF0000) | v4u);
            return a1 ^ v4 ^ 0x537E;
        }
    }
}
