
/**
 * @brief ISO-TP message.
 * @details see https://en.wikipedia.org/wiki/ISO_15765-2 for ISO-TP
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
	public class ISO_MESSAGE_t
	{
		public ISO_MESSAGE_t()
		{
			data = new byte[4096];
		}
		public uint id = 0xffffffff;      /**< @brief the from-address of the responding device */
		public ushort length = 0;         /**< @brief size, max 4096 */
		public ushort index = 0;              /**< @brief pointer */
		public byte next = 1;             /**< @brief sequence of next frame */
		public byte[] data;           /**< @brief max ISOTP multiframe message */
		public uint flow_delay = 0; /**< @brief delay between outgoing isoMessageToString */
		public byte flow_counter = 0;     /**< @brief frames to send (until new flow control) */
		public bool flow_active = false;      /**< @brief flow active*/
		public int Bus = 0;			
	}
	/**
 * \brief CAN frame type (standard/extended)
 */
	public enum CAN_frame_format_t
	{
		CAN_frame_std = 0, /**< Standard frame, using 11 bit identifer. */
		CAN_frame_ext = 1  /**< Extended frame, using 29 bit identifer. */
	}

	/**
	 * \brief CAN RTR
	 */
	public enum CAN_RTR_t
	{
		CAN_no_RTR = 0, /**< No RTR frame. */
		CAN_RTR = 1     /**< RTR frame. */
	}

	public struct CAN_FIR_t
	{
		public byte DLC;               /**< \brief [3:0] DLC, Data length container */
		public CAN_RTR_t RTR;             /**< \brief [6:6] RTR, Remote Transmission Request */
		public CAN_frame_format_t FF;     /**< \brief [7:7] Frame Format, see# CAN_frame_format_t*/
		//public uint reserved_24; /**< \brief \internal Reserved */
	}
	public class CAN_frame_t
	{
		public CAN_frame_t()
		{
			data = new byte[8];
		}
		public CAN_FIR_t FIR;  /**< \brief Frame information record*/
		public uint MsgID; /**< \brief Message ID */
		public byte[] data;
		public byte u8(int index) {return data[index]; }
		public int Bus = 0;	
	} 

} 


