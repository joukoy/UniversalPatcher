/*
 *  SRecord.cs
 *  Utility class to create, read, write, and print Motorola S-Record binary records.
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

namespace UniversalPatcher
{
	/// <summary>
	/// SRecordStructure provides the internal data structure which will be used by the SRecord class.
	/// This class is used for internal processing and is declared public to allow the application that instantiates
	/// the SRecord class access to the internal storage.
	/// </summary>
	public class SRecordStructure
	{
		public UInt32 address;  //< The address field. This can be 16, 24, or 32 bits depending on the record type.
		//< The 8-bit array data field, which has a maximum size of 32 bytes.
		public byte[] data = new byte[SRecord.SRECORD_MAX_DATA_LEN / 2];
		public int dataLen;     //< The number of bytes of data stored in this record.
		public int type;        //< The Motorola S-Record type of this record (S0-S9).
		public byte checksum;   //< The checksum of this record. */


	}

	/// <summary>
	/// SRecord is the base class to work with Motorola S-records.
	/// This class will contain all necessary functions to process data using the Motorola S-Record standard.
	/// </summary>
	public class SRecord
	{
		// 768 should be plenty of space to read in an S-Record
		const int SRECORD_RECORD_BUFF_SIZE = 768;
		// Offsets and lengths of various fields in an S-Record record
		const int SRECORD_TYPE_OFFSET = 1;
		const int SRECORD_TYPE_LEN = 1;
		const int SRECORD_COUNT_OFFSET = 2;
		const int SRECORD_COUNT_LEN = 2;
		const int SRECORD_ADDRESS_OFFSET = 4;
		const int SRECORD_CHECKSUM_LEN = 2;
		// Maximum ASCII hex length of the S-Record data field
		public const int SRECORD_MAX_DATA_LEN = 64;
		// Maximum ASCII hex length of the S-Record address field
		const int SRECORD_MAX_ADDRESS_LEN = 8;
		// ASCII hex length of a single byte
		const int SRECORD_ASCII_HEX_BYTE_LEN = 2;
		// Start code offset and value
		const int SRECORD_START_CODE_OFFSET = 0;
		const char SRECORD_START_CODE = 'S';

		const int SRECORD_OK = 0; 			            //< Error code for success or no error.
		const int SRECORD_ERROR_FILE = -1; 		        //< Error code for error while reading from or writing to a file. You may check errno for the exact error if this error code is encountered.
		const int SRECORD_ERROR_EOF = -2; 		        //< Error code for encountering end-of-file when reading from a file.
		const int SRECORD_ERROR_INVALID_RECORD = -3; 	//< Error code for error if an invalid record was read.
		const int SRECORD_ERROR_INVALID_ARGUMENTS = -4; //< Error code for error from invalid arguments passed to function.
		const int SRECORD_ERROR_NEWLINE = -5; 		    //< Error code for encountering a newline with no record when reading from a file.
		const int SRECORD_ERROR_INVALID_STRUCTURE = -6; //< Error code for not building a structure prior to calling the function.

		const int SRECORD_TYPE_S0 = 0; //< Header record, although there is an official format it is often made proprietary by third-parties. 16-bit address normally set to 0x0000 and header information is stored in the data field. This record is unnecessary and commonly not used.
		const int SRECORD_TYPE_S1 = 1; //< Data record with 16-bit address
		const int SRECORD_TYPE_S2 = 2; //< Data record with 24-bit address
		const int SRECORD_TYPE_S3 = 3; //< Data record with 32-bit address
		const int SRECORD_TYPE_S4 = 4; //< Extension by LSI Logic, Inc. See their specification for more details.
		const int SRECORD_TYPE_S5 = 5; //< 16-bit address field that contains the number of S1, S2, and S3 (all data) records transmitted. No data field. */
		const int SRECORD_TYPE_S6 = 6; //< 24-bit address field that contains the number of S1, S2, and S3 (all data) records transmitted. No data field. */
		const int SRECORD_TYPE_S7 = 7; //< Termination record for S3 data records. 32-bit address field contains address of the entry point after the S-Record file has been processed. No data field.
		const int SRECORD_TYPE_S8 = 8; //< Termination record for S2 data records. 24-bit address field contains address of the entry point after the S-Record file has been processed. No data field.
		const int SRECORD_TYPE_S9 = 9; //< Termination record for S1 data records. 16-bit address field contains address of the entry point after the S-Record file has been processed. No data field.

		// Lengths of the ASCII hex encoded address fields of different SRecord types
		static int[] SRecord_Address_Lengths = {
			4, // S0
			4, // S1
			6, // S2
			8, // S3
			8, // S4
			4, // S5
			6, // S6
			8, // S7
			6, // S8
			4, // S9
		};

		SRecordStructure srec = new SRecordStructure(); // internal structure that holds the record information.
		int status = SRECORD_ERROR_INVALID_ARGUMENTS;   // internal variable that saves the status of the last function call.

		// Accessor variable to return status of last function call.
		public int Status
		{
			get { return status; }
		}

		/// <summary>
		/// Initializes a new SRecord structure that is returned upon successful completion of the function,
		/// including up to 32-bit integer address, 8-bit data array, and size of 8-bit data array.
		/// </summary>
		/// <param name="type">The type of S-Record to be defined by the record.</param>
		/// <param name="address">The 16-, 24-, or 32-bit address of the record.</param>
		/// <param name="data">An array of 8-bit data bytes.</param>
		/// <param name="dataLen">The number of data bytes passed in the array.</param>
		/// <returns>SRecordStructure instance or null, if null then query Status class variable for the error.</returns>
		public SRecordStructure NewRecord(int type, UInt32 address, byte[] data, int dataLen)
		{
			// Data length size check, assertion of srec pointer
			if (dataLen < 0 || dataLen > SRECORD_MAX_DATA_LEN / 2 || srec == null)
			{
				status = SRECORD_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			srec.type = type;
			srec.address = address;
			if(data != null)
				Array.Copy(data, srec.data, (long)dataLen);
			srec.dataLen = dataLen;
			srec.checksum = Checksum();

			status = SRECORD_OK;
			return srec;
		}

		/// <summary>
		/// Utility function to read an S-Record from a file
		/// </summary>
		/// <param name="inStream">An instance of the StreamReader class to allow reading the file data.</param>
		/// <returns>SRecordStructure instance or null, if null then query Status class variable for the error.</returns>
		public SRecordStructure Read(StreamReader inStream)
		{
			String recordBuff;
			int asciiAddressLen, asciiDataLen, dataOffset, fieldDataCount, i;

			// Check our record pointer and file pointer
			if (srec == null || inStream == null)
			{
				status = SRECORD_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			try
			{
				// Read Line will return a line from the file.
				recordBuff = inStream.ReadLine();
			}
			catch (Exception)
			{
				status = SRECORD_ERROR_FILE;
				return null;
			}

			// Check if we hit a newline
			if (recordBuff == null || recordBuff.Length == 0)
			{
				status = SRECORD_ERROR_NEWLINE;
				return null;
			}

			// Size check for type and count fields
			if (recordBuff.Length < SRECORD_TYPE_LEN + SRECORD_COUNT_LEN)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			// Check for the S-Record start code at the beginning of every record
			if (recordBuff[SRECORD_START_CODE_OFFSET] != SRECORD_START_CODE)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			// Convert the type field into a usable integer
			srec.type = Convert.ToInt32(recordBuff.Substring(SRECORD_TYPE_OFFSET, SRECORD_TYPE_LEN), 16);

			// Copy the ASCII hex encoding of the count field into hexBuff, convert it to a usable integer
			fieldDataCount = Convert.ToInt32(recordBuff.Substring(SRECORD_COUNT_OFFSET, SRECORD_COUNT_LEN), 16);

			// Check that our S-Record type is valid
			if (srec.type < SRECORD_TYPE_S0 || srec.type > SRECORD_TYPE_S9)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}
			// Get the ASCII hex address length of this particular S-Record type
			asciiAddressLen = SRecord_Address_Lengths[srec.type];

			// Size check for address field
			if (recordBuff.Length < SRECORD_ADDRESS_OFFSET + asciiAddressLen)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			// Copy the ASCII hex encoding of the count field into hexBuff, convert it to a usable integer
			srec.address = Convert.ToUInt32(recordBuff.Substring(SRECORD_ADDRESS_OFFSET, asciiAddressLen), 16);

			// Compute the ASCII hex data length by subtracting the remaining field lengths from the S-Record
			// count field (times 2 to account for the number of characters used in ASCII hex encoding)
			asciiDataLen = (fieldDataCount*2) - asciiAddressLen - SRECORD_CHECKSUM_LEN;
			// Bailout if we get an invalid data length
			if (asciiDataLen < 0 || asciiDataLen > SRECORD_MAX_DATA_LEN)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			// Size check for final data field and checksum field
			if (recordBuff.Length < SRECORD_ADDRESS_OFFSET + asciiAddressLen + asciiDataLen + SRECORD_CHECKSUM_LEN)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			dataOffset = SRECORD_ADDRESS_OFFSET+asciiAddressLen;

			// Loop through each ASCII hex byte of the data field, pull it out into hexBuff,
			// convert it and store the result in the data buffer of the S-Record
			for (i = 0; i < asciiDataLen/2; i++) {
				// Times two i because every byte is represented by two ASCII hex characters
				srec.data[i] = Convert.ToByte(recordBuff.Substring(dataOffset+2*i, SRECORD_ASCII_HEX_BYTE_LEN), 16);
			}
			// Real data len is divided by two because every byte is represented by two ASCII hex characters
			srec.dataLen = asciiDataLen/2;

			// Copy out the checksum ASCII hex encoded byte, and convert it back to a usable integer
			srec.checksum = Convert.ToByte(recordBuff.Substring(dataOffset+asciiDataLen, SRECORD_CHECKSUM_LEN), 16);

			if (srec.checksum != Checksum())
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			status = SRECORD_OK;
			return srec;
		}

		/// <summary>
		///  Utility function to write an S-Record to a file
		/// </summary>
		/// <param name="outStream">An instance of the StreamWriter class to allow writing the file data.</param>
		/// <returns>SRecordStructure instance or null, if null then query Status class variable for the error.</returns>
		public SRecordStructure Write(StreamWriter outStream)
		{
			int asciiAddressLen, fieldDataCount, i;

			// Check our record pointer and file pointer
			if (srec == null || outStream == null)
			{
				status = SRECORD_ERROR_INVALID_ARGUMENTS;
				return null;
			}

			// Check that the type and data length is within range
			if (srec.type < SRECORD_TYPE_S0 || srec.type > SRECORD_TYPE_S9 || srec.dataLen > SRECORD_MAX_DATA_LEN / 2)
			{
				status = SRECORD_ERROR_INVALID_RECORD;
				return null;
			}

			// Compute the record count, address and checksum lengths are halved because record count
			// is the number of bytes left in the record, not the length of the ASCII hex representation
			fieldDataCount = SRecord_Address_Lengths[srec.type]/2 + srec.dataLen + SRECORD_CHECKSUM_LEN/2;

			asciiAddressLen = SRecord_Address_Lengths[srec.type];
			// The offset of the ASCII hex encoded address from zero, this is used so we only write as
			// many bytes of the address as permitted by the S-Record type.
			//asciiAddressOffset = SRECORD_MAX_ADDRESS_LEN-asciiAddressLen;

			try
			{
				outStream.Write(String.Format("{0}{1:X1}{2:X2}",  SRECORD_START_CODE, srec.type, fieldDataCount));
				// Write the ASCII hex representation of the address, starting from the offset to only
				// write as much of the address as permitted by the S-Record type (calculated above).
				// Fix the hex to be 8 hex digits of precision, to fit all 32-bits, including zeros
				String formatStr = String.Format("{{{0}:X{1}}}", 0, asciiAddressLen);
				outStream.Write(String.Format(formatStr,  srec.address));
				// Write each byte of the data, guaranteed to be two hex ASCII characters each since
				// srec->data[i] has the type of byte
				for (i = 0; i < srec.dataLen; i++)
					outStream.Write(String.Format("{0:X2}",  srec.data[i]));
				// Last but not least, the checksum
				outStream.WriteLine(String.Format("{0:X2}",  Checksum()));
			}
			catch(Exception)
			{
				status = SRECORD_ERROR_FILE;
				return null;
			}

			status = SRECORD_OK;
			return srec;
		}

		/// <summary>
		/// Utility function to print the information stored in an S-Record
		/// </summary>
		/// <param name="verbose">A boolean set to false by default, if set to true will provide extended information.</param>
		/// <returns>String which provides the output of the function, this does not write directly to the console.</returns>
		public String Print(bool verbose = false)
		{
			int i;
			String returnString;

			if (srec == null)
			{
				status = SRECORD_ERROR_INVALID_STRUCTURE;
				return null;
			}

			if (verbose)
			{
				returnString = String.Format("S-Record Type: \t\tS{0}\n", srec.type);
				String formatStr = String.Format("S-Record Address: \t0x{{{0}:X{1}}}\n", 0, SRecord_Address_Lengths[srec.type]);
				returnString += String.Format(formatStr, srec.address);
				returnString += String.Format("S-Record Data: \t\t[");
				for (i = 0; i < srec.dataLen; i++)
				{
					if (i + 1 < srec.dataLen)
						returnString += String.Format("0x{0:X02}, ", srec.data[i]);
					else
						returnString += String.Format("0x{0:X02}", srec.data[i]);
				}
				returnString += String.Format("]\n");
				returnString += String.Format("S-Record Checksum: \t0x{0:X2}\n", srec.checksum);
			}
			else
			{
				int fieldDataCount = SRecord_Address_Lengths[srec.type]/2 + srec.dataLen + SRECORD_CHECKSUM_LEN/2;
				returnString = String.Format("{0}{1:X1}{2:X2}", SRECORD_START_CODE, srec.type, fieldDataCount);
				String formatStr = String.Format("{{{0}:X{1}}}", 0, SRecord_Address_Lengths[srec.type]);
				returnString += String.Format(formatStr, srec.address);
				for (i = 0; i < srec.dataLen; i++)
					returnString += String.Format("{0:X2}", srec.data[i]);
				returnString += String.Format("{0:X2}", Checksum());
			}
			status = SRECORD_OK;
			return (returnString);
		}

		/// <summary>
		/// An internal utility function to calculate the checksum of an S-Record
		/// </summary>
		/// <returns>byte which is the checksum of SRecordStructure.</returns>
		internal byte Checksum()
		{
			byte checksum;
			int fieldDataCount, i;

			// Compute the record count, address and checksum lengths are halved because record count
			// is the number of bytes left in the record, not the length of the ASCII hex representation
			fieldDataCount = SRecord_Address_Lengths[srec.type]/2 + srec.dataLen + SRECORD_CHECKSUM_LEN/2;

			// Add the count, address, and data fields together
			checksum = (byte)fieldDataCount;
			// Add each byte of the address individually
			checksum += (byte)(srec.address & 0x000000FF);
			checksum += (byte)((srec.address & 0x0000FF00) >> 8);
			checksum += (byte)((srec.address & 0x00FF0000) >> 16);
			checksum += (byte)((srec.address & 0xFF000000) >> 24);
			for (i = 0; i < srec.dataLen; i++)
				checksum += srec.data[i];

			// One's complement the checksum
			checksum = (byte)~checksum;

			return checksum;
		}

	}
}
