﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathParserTK;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    /// <summary>
    /// How much of the PCM to erase and rewrite.
    /// </summary>
    public enum WriteType
    {
        None = 0,
        Compare,
        TestWrite,
        Calibration,
        Parameters,
        OsPlusCalibrationPlusBoot,
        Full,
    }
    /// <summary>
    /// Random bits of code that have no other home.
    /// </summary>
    public static class Utility
    {

        public static MathParser parser = new MathParser();
        /// <summary>
        /// The space character. Used for calls to string.Join().
        /// </summary>
        private static readonly char[] space = new char[] { ' ' };

        /// <summary>
        /// Convert an array of bytes to a hex string.
        /// </summary>
        public static string ToHex(this byte[] bytes)
        {
            return string.Join(" ", bytes.Select(x => x.ToString("X2")));
        }

        /// <summary>
        /// Convert only part of an array of bytes to a hex string.
        /// </summary>
        public static string ToHex(this byte[] bytes, int count)
        {
            return string.Join(" ", bytes.Take(count).Select(x => x.ToString("X2")));
        }

        /// <summary>
        /// Indicates whether the given string contains just hex characters 
        /// and whitespace, or garbage.
        /// </summary>
        public static bool IsHex(this string value)
        {
            for (int index = 0; index < value.Length; index++)
            {
                if (char.IsWhiteSpace(value[index]))
                {
                    continue;
                }

                if (Utility.GetHex(value[index]) == -1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert the given string (which should be in hex format) to a byte array.
        /// </summary>
        public static byte[] ToBytes(this string hex)
        {
            try
            {
                List<byte> retval = new List<byte>();
                for (int x=0; x<hex.Length -1; x+= 2)
                {
                    retval.Add(Convert.ToByte(hex.Substring(x, 2), 16));
                }
                return retval.ToArray();

/*
                // This left-shift-and-multiply step ensures that we don't throw
                // an exception if the string contains an odd number of characters, 
                int bytesToConvert = hex.Length >> 1;
                return Enumerable.Range(0, bytesToConvert * 2)
                                    .Where(x => x % 2 == 0)
                                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                                    .ToArray();
*/
            }
            catch (Exception exception)
            {
                throw new ArgumentException("Unable to convert \"" + hex + "\" from hexadecimal to bytes", exception);
            }
        }

        /// <summary>
        /// Indicate whether the two arrays are identical or not.
        /// </summary>
        public static bool CompareArrays(byte[] actual, params byte[] expected)
        {
            if (actual.Length != expected.Length)
            {
                return false;
            }

            for (int index = 0; index < expected.Length; index++)
            {
                if (actual[index] != expected[index])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Indicate whether the overlap between two byte arrays are identical or not.
        /// </summary>
        public static bool CompareArraysPart(byte[] actual, byte[] expected)
        {
            if (actual == null || expected == null) return false;

            for (int index = 0; index < expected.Length && index < actual.Length; index++)
            {
                if (actual[index] != expected[index])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This removes non-ascii from a byte array
        /// </summary>
        public static byte[] GetPrintable(byte[] input)
        {
            var cleanBuffer = input.Where((b) => (b <= 0x7E) & (b >= 0x30)).ToArray();
            return cleanBuffer;
        }

        /// <summary>
        /// Compare the two operating system IDs, report on the ramifications, set a flag if the write should be halted.
        /// </summary>
        public static void ReportOperatingSystems(UInt32 fileOs, UInt32 pcmOs, WriteType writeType, out bool shouldHalt)
        {
            shouldHalt = false;
            if (fileOs == pcmOs)
            {
                Logger("PCM and image file are both operating system " + fileOs);
            }
            else
            {
                if ((writeType == WriteType.OsPlusCalibrationPlusBoot) || (writeType == WriteType.Full))
                {
                    Logger("Changing PCM to operating system " + fileOs);
                }
                else if (writeType == WriteType.TestWrite)
                {
                    Logger("PCM and image file are different operating systems.");
                    Logger("But we'll ignore that because this is just a test write.");
                }
                else
                {
                    Logger("Flashing this file could render your PCM unusable.");
                    shouldHalt = true;
                }
            }
        }

        /// <summary>
        /// Print out the number of retries, and beg the user to share.
        /// </summary>
        public static void ReportRetryCount(string operation, int retryCount, UInt32 flashChipSize)
        {
            if (retryCount == 0)
            {
                Logger("All " + operation.ToLower() + "-request messages succeeded on the first try. You have an excellent connection to the PCM.");
            }
            else if (retryCount < 3)
            {
                Logger(operation + "-request messages had to be re-sent " + (retryCount == 1 ? "once." : "twice."));
            }
            else
            {
                Logger(operation + "-request messages had to be re-sent " + retryCount + " times.");
            }

            Logger("We're not sure how much retrying is normal for a " + operation.ToLower() + " operation on a " + (flashChipSize / 1024).ToString() + "kb PCM."); 
            Logger("Please help by sharing your results in the PCM Hammer thread at pcmhacking.net.");
        }

        /// <summary>
        /// There's a bug here. I haven't fixed it because after writing this 
        /// I learned that I don't actually need it.
        /// </summary>
        internal static byte ComputeCrc(byte[] bytes)
        {
            int crc = 0xFF;
            for (int index = 0; index < bytes.Length; index++)
            {
                crc ^= bytes[index];
                if ((crc & 0x80) != 0)
                {
                    crc <<= 1;
                    crc ^= 0x11D;
                }
                else
                {
                    crc <<= 1;
                }
            }

            return (byte) crc;
        }

        /// <summary>
        /// Gets the 0-15 value of a hexadecimal numeral.
        /// </summary>
        private static int GetHex(char newCharacter)
        {
            if (newCharacter >= '0' && (newCharacter <= '9'))
            {
                return newCharacter - '0';
            }

            if (newCharacter >= 'a' && (newCharacter <= 'f'))
            {
                return 10 + (newCharacter - 'a');
            }

            if (newCharacter >= 'A' && (newCharacter <= 'F'))
            {
                return 10 + (newCharacter - 'A');
            }

            return -1;
        }
    }
}
