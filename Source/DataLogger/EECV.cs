using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    class EECV
    {
		public ushort CalculateKey(int ChallengeCount, byte SeedByte, byte SeedByte1, byte SeedByte2, byte SeedByte3)
		{
			int algoIndex;
			byte KeyReq1;
			byte ResultByte1;
			byte ResultByte2;

			if ((SeedByte & 0x01) == 0)
			{
				algoIndex = SeedByte3 & 0x03;
				KeyReq1 = SeedByte2;
				ResultByte2 = SolveCrypto(ChallengeCount, algoIndex, KeyReq1, SeedByte);

				algoIndex = (SeedByte3 & 0x0C) >> 2;
				KeyReq1 = SeedByte1;
				ResultByte1 = SolveCrypto(ChallengeCount, algoIndex, KeyReq1, SeedByte);
			}
			else
			{
				algoIndex = (SeedByte3 & 0x30) >> 4;
				KeyReq1 = SeedByte2;
				ResultByte2 = SolveCrypto(ChallengeCount, algoIndex, KeyReq1, SeedByte);

				algoIndex = (SeedByte3 & 0xC0) >> 6;
				KeyReq1 = SeedByte1;
				ResultByte1 = SolveCrypto(ChallengeCount, algoIndex, KeyReq1, SeedByte);
			}
			return (ushort)(ResultByte1 << 8 | ResultByte2);
		}

		byte SecurityAlgo0(int ChallengeCount, byte CalcByte1, byte CalcByte2)
		{
			byte tmpByte1 = CalcByte1;
			byte tmpByte2 = CalcByte1;
			byte tmpByte3 = CalcByte2;

			CalcByte1 += 0x64;

			if (ChallengeCount > 0)
				CalcByte1 -= 1;

			CalcByte1 *= CalcByte1;

			while (tmpByte3 > 1)
			{
				tmpByte1 *= tmpByte2;
				tmpByte3 -= 1;
			}

			if (tmpByte3 == 0)
				tmpByte1 = 1;

			tmpByte1 -= 1;
			tmpByte1 *= CalcByte2;
			CalcByte1 += tmpByte1;

			return CalcByte1;
		}

		byte SecurityAlgo1(int ChallengeCount, byte CalcByte1, byte CalcByte2)
		{
			CalcByte1 += 0x05;

			if (ChallengeCount > 0)
				CalcByte1 = (byte)(0x100 - CalcByte1);

			byte tmpByte1 = (byte)(CalcByte1 * CalcByte1);
			tmpByte1 *= CalcByte1;
			CalcByte1 = CalcByte2;
			CalcByte1 *= 0x05;
			CalcByte1 -= tmpByte1;

			return CalcByte1;
		}

		byte SecurityAlgo2(int ChallangeCount, byte CalcByte1, byte CalcByte2)
		{
			byte tmpByte1 = CalcByte2;
			byte tmpByte2 = (byte)(CalcByte1 * CalcByte1);
			CalcByte1 *= tmpByte2;
			tmpByte1 *= tmpByte1;
			CalcByte1 -= tmpByte1;
			CalcByte1 += 0x0A;

			if (ChallangeCount > 0)
				CalcByte1 += 1;

			CalcByte1 *= CalcByte1;

			return CalcByte1;
		}

		byte SecurityAlgo3(int ChallengeCount, byte CalcByte1, byte CalcByte2)
		{
			byte tmpByte1 = (byte)(CalcByte1 + 0x22);

			CalcByte1 = (byte)(CalcByte1 * CalcByte1 + CalcByte2 - 0x28);

			if (ChallengeCount > 0)
				tmpByte1 -= 1;

			CalcByte1 += (byte)(tmpByte1 * tmpByte1 * tmpByte1);

			return CalcByte1;
		}

		byte SolveCrypto(int ChallangeCount, int AlgoIndex,  byte CalcByte1, byte CalcByte2)
		{
			byte retVal = 0;

			switch (AlgoIndex)
			{
				case 0:
					retVal = SecurityAlgo0(ChallangeCount, CalcByte1, CalcByte2);
					break;

				case 1:
					retVal = SecurityAlgo1(ChallangeCount, CalcByte1, CalcByte2);
					break;

				case 2:
					retVal = SecurityAlgo2(ChallangeCount, CalcByte1, CalcByte2);
					break;

				case 3:
					retVal = SecurityAlgo3(ChallangeCount, CalcByte1, CalcByte2);
					break;

				default:
					throw new Exception("Unknown algorithm: " + AlgoIndex.ToString());
			}

			return retVal;
		}


	}
}
