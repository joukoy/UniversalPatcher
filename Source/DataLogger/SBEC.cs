using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class SBEC
    {
        byte byte_10008030 =  0x44;
        byte byte_10008031 = 0x41;
        byte byte_10008032 = 0x49;
        byte byte_10008033 = 0x4D;
        byte byte_10008034 = 0x4C;
        byte byte_10008035 = 0x45;
        byte byte_10008036 = 0x52;
        byte byte_10008037 = 0x43;
        byte byte_10008038 = 0x48;
        byte byte_10008039 = 0x52;
        byte byte_1000803A = 0x59;
        byte byte_1000803B = 0x53;
        byte byte_1000803C = 0x4C;
        byte byte_1000803D = 0x45;
        byte byte_1000803E = 0x52;
        byte byte_1000803F = 0x33;
        //Obsolete
        public uint unlock(uint a1)
        {
            ushort v1; // dx
            uint v2; // eax
            uint v3; // edx
            ushort v4; // bp
            uint v6; // [esp+10h] [ebp-8h]
            ushort v7; // [esp+1Ch] [ebp+4h]
            uint v8; // [esp+1Ch] [ebp+4h]

            v1 = (ushort)(a1 & 0xFF00);
            v2 = (uint)((byte)((a1 >> 24) & 0xFF) | (byte)((a1 >> 16) & 0xFF00));
            v3 = (ushort)((byte)((a1 >> 8) & 0XFF0000) | v1);
            v4 = (ushort)(byte_10008030 ^ (ushort)(8 * byte_10008031 ^ (ushort)(4 * byte_10008032 ^ (ushort)(8 * byte_10008033))));
            v7 = 0;
            v6 = 16;
            while (true)
            {
                v7 -= 25033;
                v2 = (ushort)(v2  + ((v3 + v7) ^ ((byte_10008034 ^ (ushort)(8 * (byte_10008035 ^ (ushort)(4 * 
                    (byte_10008036 ^ (ushort)(8 * byte_10008037)))))) +(v3 >> 5)) ^(v4 + 16 * v3)));
                v3 = (ushort)(v3 + ((v2 + v7) ^ ((byte_1000803C ^ (ushort)(8 * (byte_1000803D ^ (ushort)(4 * 
                    (byte_1000803E ^ (ushort)(8 *byte_1000803F)))))) +(v2 >> 5)) ^((byte_10008038 ^ (ushort)(8 * 
                    (byte_10008039 ^ (ushort)(4 * (byte_1000803A ^ (ushort)(8 * byte_1000803B)))))) + 16 * v2)));
                v6--;
                if (v6 == 0)
                    break;
                v4 = (ushort)(byte_10008030 ^ (ushort)(8 * (byte_10008031 ^ (ushort)(4 * (byte_10008032 ^ (ushort)(8 * byte_10008033))))));
            }
            v8 = (uint)(((v2 << 24) & 0xFF000000) | ((v3 << 16) & 0xFF0000) | ((v2 << 8) & 0xFF00) | (v3 & 0xFF));
            return v8;
        }

        public uint calculateKey(uint seed)
        {
            uint KEY_CONSTANT_1 = 0x966AEEB1;
            uint KEY_CONSTANT_2 = 0x440BCE28;
            uint AND_CONSTANT = 4294967295;

            uint tempSeed = (uint)(((seed >> 16) & 0xFF) << 24);
            Debug.WriteLine(tempSeed.ToString("X8"));
            tempSeed = (uint)(tempSeed | (uint)(((seed >> 24) & 0xFF) << 16));
            Debug.WriteLine(tempSeed.ToString("X8"));
            tempSeed = (uint)(tempSeed | (uint)((seed >> 8) & 0xFF));
            Debug.WriteLine(tempSeed.ToString("X8"));
            tempSeed = (uint)(tempSeed | (uint)((seed & 0xFF) << 8));
            Debug.WriteLine(tempSeed.ToString("X8"));

            uint shiftSeed = (uint)((tempSeed << 11) | (tempSeed >> 22)) & AND_CONSTANT;
            Debug.WriteLine(shiftSeed.ToString("X8"));

            uint key = (uint)((shiftSeed ^ (KEY_CONSTANT_1 ^ (uint)(KEY_CONSTANT_2 & (uint)seed))) & AND_CONSTANT);

            return key;

        }

    }
}
