using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Helpers;
namespace UniversalPatcher
{
    public class KeyAlgorithm
    {
        /// <summary>
        /// Gets the unlock key from the given algorithm index and seed.
        /// </summary>
        /// <remarks>
        /// Updated with correct keygenerator that will support all algos.
        /// Updated January 10, 2020 - Gampy <pcmhacking.net>
        ///   Added algorithm index bounds checking, removed GetKey_?() hacks and removed unused elements.
        ///   Modified March 2022 - joukoy@gmail.com
        ///   Almost complete rewrite 
        /// </remarks>
        public static UInt16 GetKey(int algo, UInt16 seed)
        {
            if ((algo >= 0) && (algo <= 0x3FF))
            {
                if (seed != 0xFFFF)
                {
                    return unchecked((UInt16)KeyAlgo(seed, algo));
                }
                else
                {
                    // 0xFFFF seed is non-standard and indicates that Parameter block is not programmed
                    // so the key is also 0xFFFF. This sometimes happens after SPS flashing.
                    return 0xFFFF;
                }
            }
            else
            {
                return 0x0000;
            }
        }

        public static ushort key_value; // key value

        public static byte[][] bytearray0;
        
        public static void Op_code_add(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)(high_byte << 8);
            ushort u2 = (ushort)(u1 | low_byte);
            key_value = (ushort)(u2 + key_value);
        }

        public static void Op_code_comp(byte high_byte, byte low_byte)
        {
            if (high_byte >= low_byte)
            {
                key_value = (ushort)~key_value;
            }
            else
                key_value = (ushort)(~(key_value) + 1);
        }

        public static void Op_code_rot_lt(byte high_byte, byte low_byte)
        {
            key_value = (ushort)((key_value << high_byte) | (key_value >> (16 - high_byte)));
        }

        public static void Op_code_rot_rt(byte high_byte, byte low_byte)
        {
            key_value = (ushort)((key_value >> low_byte) | (key_value << (16 - low_byte)));
        }

        public static void Op_code_sub(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)(high_byte << 8);
            ushort u2 = (ushort)(u1 | low_byte);
            key_value = (ushort)(key_value - u2);
        }

        public static void Op_code_swap_add(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)((key_value & 0xFF00) >> 8);
            ushort u2 = (ushort)((key_value & 0xFF) << 8);
            ushort u3 = (ushort)(u1 | u2);
            if (high_byte >= low_byte)
            {
                ushort u4 = (ushort)((0xFF & high_byte) << 8);
                u4 = (ushort)(u4 | low_byte);
                key_value = (ushort)(u3 + u4);
            }
            else
            {
                ushort u4 = (ushort)((0xFF & low_byte) << 8);
                ushort u5  = (ushort)(0xFF & high_byte);
                u4 = (ushort)(u4 | u5);
                key_value = (ushort)(u3 + u4);
            }
        }

        public static void Op_code_rol8(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)((key_value << 8) & 0xFF00); //rotate low byte  8 bits left
            ushort u2 = (ushort)((key_value >> 8) & 0x00FF);  //rotate high byte 8 bits right
            key_value =(ushort) (u1 | u2);
        }

        public static void Op_swap_arg_and(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)(low_byte << 8);
            ushort u2 = (ushort)(u1 | high_byte);
            key_value = (ushort)(u2 & key_value);
        }

        public static void Op_code_swap_arg_or(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)(low_byte << 8);
            ushort u2 = (ushort)(u1 | high_byte);
            key_value = (ushort)(u2 | key_value);
        }

        public static void Op_code_swap_arg_add(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)(low_byte << 8);
            ushort u2 = (ushort)(u1 | high_byte);
            key_value = (ushort)(u2 + key_value);
        }

        public static void Op_code_swap_arg_sub(byte high_byte, byte low_byte)
        {
            ushort u1 = (ushort)(low_byte << 8);
            ushort u2 = (ushort)(u1 | high_byte);
            key_value = (ushort)(key_value - u2);
        }

        private static void ReadKeyArray()
        {
            try
            {
                string fName = Path.Combine(Application.StartupPath, "XML", "2byte-keys.txt");
                if (!File.Exists(fName))
                {
                    LoggerBold("KEY file missing, please download: " + Environment.NewLine +
                        "https://github.com/joukoy/UniversalPatcher/blob/master/XML/2byte-keys.txt");
                    LoggerBold("As file:" + Environment.NewLine + fName);
                    return;
                }
                string keyTxt = File.ReadAllText(fName);
                string[] lines = keyTxt.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                List<byte[]> byterows = new List<byte[]>();
                foreach (string line in lines)
                {
                    if (line.StartsWith("/")) //Skip comments
                    {
                        continue;
                    }
                    string[] byteStrs = line.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
                    if (byteStrs.Length >= 13)
                    {
                        byte[] byterow = new byte[13];
                        for (int b = 0; b < 13; b++)
                        {
                            if (byteStrs[b].StartsWith("0x"))
                            {
                                if (HexToByte(byteStrs[b].Replace("0x", ""), out byte val))
                                {
                                    byterow[b] = val;
                                }
                                else
                                {
                                    Debug.WriteLine("ReadKeyArray, parse HEX?? " + line);
                                }
                            }
                            else
                            {
                                if (byte.TryParse(byteStrs[b], out byte val))
                                {
                                    byterow[b] = val;
                                }
                                else
                                {
                                    Debug.WriteLine("ReadKeyArray, parse bytes?? " + line);
                                }
                            }
                        }
                        byterows.Add(byterow);
                    }
                }
                bytearray0 = byterows.ToArray();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, KeyAlgorithm line " + line + ": " + ex.Message);
            }

        }

        public static int KeyAlgo(ushort seed, int algo)
        {
            key_value = seed;
            bool Done = true;
            byte byte1 = 1;
            bool loop = true;
            byte byte2 = 0;

            ReadKeyArray();

            while (loop)
            {
                if ((!Done))
                {
                    loop = false;
                }
                else
                {
                    byte2 = bytearray0[algo][byte1];
                    Debug.WriteLine("(before switch)key_value: " + key_value.ToString("X4") + ", byte1: " + byte1.ToString() + ", byte2: " + byte2.ToString());
                    switch (byte2)
                    {
                        case 5:
                            Op_code_rol8(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 20: //0x14 ADD
                            Op_code_add(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 42:  //0x2A Complement
                            Op_code_comp(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 55:  //0x37
                            Op_code_swap_arg_add(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 76:  //0x4C  Rotate left
                            Op_code_rot_lt(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 82:  //0x52  
                            Op_code_swap_arg_or(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 107:  //0x6B  Rotate right
                            Op_code_rot_rt(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 117:  //0x75
                            Op_code_swap_arg_add(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 126:  //0x7E  Flip Add
                            Op_code_swap_add(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 152:  //0x98  Substract
                            Op_code_sub(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                        case 248:  //0xF8
                            Op_code_swap_arg_sub(bytearray0[algo][byte1 + 1], bytearray0[algo][byte1 + 2]);
                            break;
                    }
                    if (byte1 >= 10)
                    {
                        Done = false;
                    }
                    else
                    {
                        byte1 = (byte)(byte1 + 3);
                    }
                    Debug.WriteLine("key_value: " + key_value.ToString("X4") + ", byte1: " + byte1.ToString() +", byte2: " + byte2.ToString());
                }
            }
            return key_value;
        }
    }
}
