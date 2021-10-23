/*
 *  IntelHex.cs
 *  Utility class to create, read, write, and print Intel HEX8 binary records.
 *
 *  Original Written by Vanya A. Sergeev <vsergeev@gmail.com>
 *  Version 1.0.5 - February 2011
 *
 *  Modified by Jerry G. Scherer <scherej1@hotmail.com> for C#
 *  Version 1.0.0 - May 2012
 */
using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace UniversalPatcher
{
	/// <summary>
	/// IntelHexStructure provides the internal data structure which will be used by the IntelHex class.
	/// This class is used for internal processing and is declared public to allow the application that instantiates
	/// the IntelHex class access to the internal storage.
	/// </summary>
	public class IntelHexStructure
	{
		//public UInt16 address;  //< The 16-bit address field.
		public uint address;  //< The 32-bit address field.
							  //< The 8-bit array data field, which has a maximum size of 256 bytes.
		public byte[] data = new byte[IntelHex.IHEX_MAX_DATA_LEN / 2];
		public int dataLen;     //< The number of bytes of data stored in this record.
		public int type;        //< The Intel HEX8 record type of this record.
		public byte checksum;   //< The checksum of this record.

	}

	/// <summary>
	/// IntelHex is the base class to work with Intel Hex records.
	/// This class will contain all necessary functions to process data using the Intel Hex record standard.
	/// </summary>
	public class IntelHex
	{
		// 768 should be plenty of space to read in a Intel HEX8 record
		const int IHEX_RECORD_BUFF_SIZE = 768;
		// Offsets and lengths of various fields in an Intel HEX8 record
		const int IHEX_COUNT_OFFSET = 1;
		const int IHEX_COUNT_LEN = 2;
		const int IHEX_ADDRESS_OFFSET = 3;
		const int IHEX_ADDRESS_LEN = 4;
		const int IHEX_TYPE_OFFSET = 7;
		const int IHEX_TYPE_LEN = 2;
		const int IHEX_DATA_OFFSET = 9;
		const int IHEX_CHECKSUM_LEN = 2;
		public const int IHEX_MAX_DATA_LEN = 512;
		// Ascii hex encoded length of a single byte
		const int IHEX_ASCII_HEX_BYTE_LEN = 2;
		// Start code offset and value
		const int IHEX_START_CODE_OFFSET = 0;
		const char IHEX_START_CODE = ':';

		const int IHEX_OK = 0; 				            //< Error code for success or no error.
		const int IHEX_ERROR_FILE = -1; 			    //< Error code for error while reading from or writing to a file. You may check errno for the exact error if this error code is encountered.
		const int IHEX_ERROR_EOF = -2; 			        //< Error code for encountering end-of-file when reading from a file.
		const int IHEX_ERROR_INVALID_RECORD = -3; 	    //< Error code for error if an invalid record was read.
		const int IHEX_ERROR_INVALID_ARGUMENTS = -4; 	//< Error code for error from invalid arguments passed to function.
		const int IHEX_ERROR_NEWLINE = -5;		        //< Error code for encountering a newline with no record when reading from a file.
		const int IHEX_ERROR_INVALID_STRUCTURE = -6;    //< Error code for not building a structure prior to calling the function.

		const int IHEX_TYPE_00 = 0;                     //< Data Record
		const int IHEX_TYPE_01 = 1;                     //< End of File Record
		const int IHEX_TYPE_02 = 2;                     //< Extended Segment Address Record
		const int IHEX_TYPE_03 = 3;                     //< Start Segment Address Record
		const int IHEX_TYPE_04 = 4;                     //< Extended Linear Address Record
		const int IHEX_TYPE_05 = 5;                     //< Start Linear Address Record

		IntelHexStructure irec = new IntelHexStructure();   // internal structure that holds the record information.
		int status = IHEX_ERROR_INVALID_ARGUMENTS;          // internal variable that saves the status of the last function call.
		uint segmentAddress = 0;

		// Accessor variable to return status of last function call.
		public int Status
		{
			get { return status; }
		}

		/// <summary>
		/// Initializes a new IntelHex structure that is returned upon successful completion of the function,
		/// including up to 16-bit integer address, 8-bit data array, and size of 8-bit data array.
		/// </summary>
		/// <param name="type">The type of Intel HEX record to be defined by the record.</param>
		/// <param name="address">The 16-, 24-, or 32-bit address of the record.</param>
		/// <param name="data">An array of 8-bit data bytes.</param>
		/// <param name="dataLen">The number of data bytes passed in the array.</param>
		/// <returns>IntelHexStructure instance or null, if null then query Status class variable for the error.</returns>
		public IntelHexStructure NewRecord(int type, UInt16 address, byte[] data, int dataLen)
		{
			// Data length size check, assertion of irec pointer
			if (dataLen < 0 || dataLen > IHEX_MAX_DATA_LEN / 2 || irec == null)
			{
				status = IHEX_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			irec.type = type;
			irec.address = address;
			if(data != null)
				Array.Copy(data, irec.data, (long)dataLen);
			irec.dataLen = dataLen;
			irec.checksum = Checksum();

			status = IHEX_OK;
			return irec;
		}

		/// <summary>
		/// Utility function to read an Intel HEX8 record from a file
		/// </summary>
		/// <param name="inStream">An instance of the StreamReader class to allow reading the file data.</param>
		/// <returns>IntelHexStructure instance or null, if null then query Status class variable for the error.</returns>
		public IntelHexStructure Read(StreamReader inStream)
		{
			String recordBuff;
			int dataCount, i;

			// Check our record pointer and file pointer
			if (irec == null || inStream == null)
			{
				status = IHEX_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			try
			{
				// Read Line will return a line from the file.
				recordBuff = inStream.ReadLine();
			}
			catch (Exception)
			{
				status = IHEX_ERROR_FILE;
				return null;
			}

			// Check if we hit a newline
			if (recordBuff == null || recordBuff.Length == 0)
			{
				status = IHEX_ERROR_NEWLINE;
				return null;
			}

			// Size check for start code, count, address, and type fields
			if (recordBuff.Length < (1 + IHEX_COUNT_LEN + IHEX_ADDRESS_LEN + IHEX_TYPE_LEN))
			{
				status = IHEX_ERROR_INVALID_RECORD;
				return null;
			}

			// Check the for colon start code
			if (recordBuff[IHEX_START_CODE_OFFSET] != IHEX_START_CODE)
			{
				status = IHEX_ERROR_INVALID_RECORD;
				return null;
			}

			// Copy the ASCII hex encoding of the count field into hexBuff, convert it to a usable integer
			dataCount = Convert.ToInt16(recordBuff.Substring(IHEX_COUNT_OFFSET, IHEX_COUNT_LEN), 16);

			// Copy the ASCII hex encoding of the Type field into hexBuff, convert it to a usable integer
			irec.type = Convert.ToInt16(recordBuff.Substring(IHEX_TYPE_OFFSET, IHEX_TYPE_LEN), 16);
			
			switch (irec.type)
            {
				case 1://End of file
					return null;
					break;
				case 2: //Extended address
					segmentAddress = (uint)(Convert.ToUInt16(recordBuff.Substring(IHEX_DATA_OFFSET, IHEX_ADDRESS_LEN), 16) * 16);
					Debug.WriteLine("Segment address: " + segmentAddress.ToString("X"));
					break;
				case 4: //Extended Linear Address 
					segmentAddress = (uint)(Convert.ToUInt16(recordBuff.Substring(IHEX_DATA_OFFSET, IHEX_ADDRESS_LEN), 16) << 16);
					Debug.WriteLine(segmentAddress.ToString("X"));
					break;
				case 5:
					segmentAddress = (uint)(Convert.ToUInt32(recordBuff.Substring(IHEX_DATA_OFFSET, 8), 16));
					Debug.WriteLine(segmentAddress.ToString("X"));
					break;
			}

			// Copy the ASCII hex encoding of the address field into hexBuff, convert it to a usable integer
			UInt16 addrLow = Convert.ToUInt16(recordBuff.Substring(IHEX_ADDRESS_OFFSET, IHEX_ADDRESS_LEN), 16);
			irec.address = segmentAddress + addrLow;

			// Size check for start code, count, address, type, data and checksum fields
			if (recordBuff.Length < (1 + IHEX_COUNT_LEN + IHEX_ADDRESS_LEN + IHEX_TYPE_LEN + dataCount * 2 + IHEX_CHECKSUM_LEN))
			{
				status = IHEX_ERROR_INVALID_RECORD;
				return null;
			}

			// Loop through each ASCII hex byte of the data field, pull it out into hexBuff,
			// convert it and store the result in the data buffer of the Intel HEX8 record
			for (i = 0; i < dataCount; i++)
			{
				// Times two i because every byte is represented by two ASCII hex characters
				irec.data[i] = Convert.ToByte(recordBuff.Substring(IHEX_DATA_OFFSET + 2 * i, IHEX_ASCII_HEX_BYTE_LEN), 16);
			}
			irec.dataLen = dataCount;

			// Copy the ASCII hex encoding of the checksum field into hexBuff, convert it to a usable integer
			irec.checksum = Convert.ToByte(recordBuff.Substring(IHEX_DATA_OFFSET+dataCount*2, IHEX_CHECKSUM_LEN), 16);

			if (irec.checksum != Checksum())
			{
				status = IHEX_ERROR_INVALID_RECORD;
				return null;
			}

			status = IHEX_OK;
			return irec;
		}

		// Utility function to write an Intel HEX8 record to a file
		public IntelHexStructure Write(StreamWriter outStream)
		{
			int i;

			// Check our record pointer and file pointer
			if (irec == null || outStream == null)
			{
				status = IHEX_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			// Check that the data length is in range
			if (irec.dataLen > IHEX_MAX_DATA_LEN / 2)
			{
				status = IHEX_ERROR_INVALID_RECORD;
				return null;
			}

			try
			{
				// Write the start code, data count, address, and type fields
				outStream.Write(String.Format("{0}{1:X2}{2:X4}{3:X2}", IHEX_START_CODE, irec.dataLen, irec.address, irec.type));
				// Write the data bytes
				for (i = 0; i < irec.dataLen; i++)
					outStream.Write(String.Format("{0:X2}",  irec.data[i]));
				// Last but not least, the checksum
				outStream.WriteLine(String.Format("{0:X2}",  Checksum()));
			}
			catch(Exception)
			{
				status = IHEX_ERROR_FILE;
				return null;
			}

			status = IHEX_OK;
			return irec;
		}

		/// <summary>
		/// Utility function to print the information stored in an Intel HEX8 record
		/// </summary>
		/// <param name="verbose">A boolean set to false by default, if set to true will provide extended information.</param>
		/// <returns>String which provides the output of the function, this does not write directly to the console.</returns>
		public String Print(bool verbose = false)
		{
			int i;
			String returnString;

			if (verbose)
			{
				returnString = String.Format("Intel HEX8 Record Type: \t{0}\n", irec.type);
				returnString += String.Format("Intel HEX8 Record Address: \t0x{0:X4}\n", irec.address);
				returnString += String.Format("Intel HEX8 Record Data: \t[");
				for (i = 0; i < irec.dataLen; i++)
				{
					if (i + 1 < irec.dataLen)
						returnString += String.Format("0x{0:X02}, ", irec.data[i]);
					else
						returnString += String.Format("0x{0:X02}", irec.data[i]);
				}
				returnString += String.Format("]\n");
				returnString += String.Format("Intel HEX8 Record Checksum: \t0x{0:X2}\n", irec.checksum);
			}
			else
			{
				returnString = String.Format("{0}{1:X2}{2:X4}{3:X2}", IHEX_START_CODE, irec.dataLen, irec.address, irec.type);
				for (i = 0; i < irec.dataLen; i++)
					returnString += String.Format("{0:X2}", irec.data[i]);
				returnString += String.Format("{0:X2}", Checksum());
			}
			status = IHEX_OK;
			return (returnString);
		}

		/// <summary>
		/// An internal utility function to calculate the checksum of an Intel HEX8 record
		/// </summary>
		/// <returns>byte which is the checksum of IntelHexStructure.</returns>
		internal byte Checksum()
		{
			byte checksum;
			int i;

			// Add the data count, type, address, and data bytes together
			checksum = (byte)irec.dataLen;
			checksum += (byte)irec.type;
			checksum += (byte)irec.address;
			checksum += (byte)((irec.address & 0xFF00)>>8);
			for (i = 0; i < irec.dataLen; i++)
				checksum += irec.data[i];

			// Two's complement on checksum
			checksum =(byte)(~checksum + 1);

			return checksum;
		}
	}
}
