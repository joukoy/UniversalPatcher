/**
* @file isotphandler.cpp
* @brief This module will handle isotp communications.
* @details This module sends out isotp requests and processes incoming frames
*          to re-assemble the original message
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
	public class IsoTP
	{
		public ISO_MESSAGE_t isoMessageIncoming; // declare an ISO-TP message
		public ISO_MESSAGE_t isoMessageOutgoing; // declare an ISO-TP message
		static DateTime lastTime;
		private int Bus;
		/**
		 * Initializes the isotp subsystem
		 */
		public void isotp_init(byte Bus)
		{
			isoMessageIncoming = new ISO_MESSAGE_t();
			isoMessageOutgoing = new ISO_MESSAGE_t();
			lastTime = DateTime.MinValue;
			this.Bus = Bus;
			isotp_reset();
		}

		/**
		 * Resets the isotp state
		 */
		private void isotp_reset()
		{
			//isoMessageIncoming.id = 0xffff;
			//isoMessageOutgoing.id = 0xffff;
			isoMessageIncoming.length = isoMessageIncoming.index = isoMessageOutgoing.length = isoMessageOutgoing.index = 0;
			Debug.WriteLine("IsoTp Reset");
		}

		public static bool isISO15765Frame(uint canId, byte[] data, byte length)
		{
			if (length < 1 || length > 8) return false;

			if ((canId >= 0x7E0 && canId <= 0x7EF) || (canId >= 0x5E0 && canId <= 0x5EF))
			{
				byte pci = (byte)(data[0] & 0xF0);
				if (data[0] == 0)
					return false;
				switch (pci)
				{
					case 0x00: // Single Frame
						return (data[0] & 0x0F) <= 7;
					case 0x10: // First Frame
					case 0x20: // Consecutive Frame
					case 0x30: // Flow Control
						return true;
					default:
						return false;
				}
			}
			else
			{
				return false;
			}
		}

		/**
		 * the ticker is called in the main loop. It handles the sending of NEXT
		 * frames asynchronously
		 */
		public void isotp_ticker()
		{
			try
			{
				CAN_frame_t frame = new CAN_frame_t(); // build the CAN frame
				if (!isoMessageOutgoing.flow_active)
					return; //
				if (DateTime.Now.Subtract(lastTime) < TimeSpan.FromMilliseconds(isoMessageOutgoing.flow_delay))
				{
					Debug.WriteLine("Flow delay not elapsed yet: " + isoMessageOutgoing.flow_delay.ToString());
					return;
				}

				// Prepare the next frame
				frame.FIR.FF = isoMessageOutgoing.id < 0x800 ? CAN_frame_format_t.CAN_frame_std : CAN_frame_format_t.CAN_frame_ext;
				frame.FIR.RTR = CAN_RTR_t.CAN_no_RTR;        // no RTR
				frame.MsgID = isoMessageOutgoing.id; // set the ID
				frame.FIR.DLC = 8;                 //command.requestLength + 1;// set the length. Note some ECU's like DLC 8

				frame.data[0] = (byte)(0x20 | (isoMessageOutgoing.next++ & 0x0f));
				int i;
				for (i = 0; i < 7 && isoMessageOutgoing.index < isoMessageOutgoing.length; i++)
				{
					frame.data[i + 1] = isoMessageOutgoing.data[isoMessageOutgoing.index++];
				}
				for (; i < 7; i++)
				{
					frame.data[i + 1] = 0;
				}

				if (AppSettings.IsoTpDebug)
				{
					Debug.WriteLine("> can:Sending ISOTP NEXT:" + canFrameToString(frame));
				}
				// check if we reached the end of the message
				if (isoMessageOutgoing.length == isoMessageOutgoing.index)
				{
					// Done sending the outgoing message, so reset it and cancel further
					// handling by this ticker
					isoMessageOutgoing.length = isoMessageOutgoing.index = 0;
					isoMessageOutgoing.flow_active = false;

					// At this moment, further sending by the ticker will stop. A new flow control
					// should not come in, but we now expect the answer, so do not invalidate the
					// incoming id
					// isoMessageIncoming.id = 0xffff;
					// the incoming message is full initiaized (id, index)
				}

				if (isoMessageOutgoing.flow_counter != 0)
				{
					if (--isoMessageOutgoing.flow_counter == 0)
					{
						isoMessageOutgoing.flow_active = false;
					}
				}

				can_send(frame); // bus logic needs to be added
				lastTime = DateTime.Now;
			}
			catch (Exception ex)
			{
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(st.FrameCount - 1);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				Debug.WriteLine("Error, isotphandler line " + line + ": " + ex.Message);
			}
		}

		/**
		 * Process an incoming ISO-TP frame
		 * @param frame Incoming CANbus frame
		 * @param bus CANbus (only 0 allowed)* 
		 */
		public void storeIsotpframe(CAN_frame_t frame)
		{
			try
			{
				if (frame.FIR.DLC == 0 || frame.MsgID != isoMessageIncoming.id)
				{
					if (AppSettings.IsoTpDebug)
					{
						Debug.WriteLine("< can:ISO frame of unrequested id:" + isoMessageIncoming.id.ToString("X4") + "," + frame.MsgID.ToString("X4"));
					}
					// new assumption: there is only ONE active ISOTP command going on this means that this condition
					// should reset the ISOTP receiver
					//isotp_reset();
					return;
				}

				byte type = (byte)(frame.data[0] >> 4); // type = first nibble
														// single frame answer ***************************************************
				if (type == 0x0)
				{
					if (AppSettings.IsoTpDebug)
					{
						Debug.WriteLine("< can:ISO SING:" + canFrameToString(frame));
					}

					ushort messageLength = (ushort)(frame.data[0] & 0x0f); // length = second nibble + second byte
					if (messageLength > 7)
					{
						isotp_reset();
						return;
					}
					isoMessageIncoming.length = messageLength;

					// fill up with this initial first-frame data (should always be 6)
					for (int i = 1; i < frame.FIR.DLC && isoMessageIncoming.index < isoMessageIncoming.length; i++)
					{
						isoMessageIncoming.data[isoMessageIncoming.index++] = frame.data[i];
					}
					String dataString = isoMessageToString(isoMessageIncoming);
					if (AppSettings.IsoTpDebug)
						Debug.WriteLine("> can:ISO MSG:" + dataString);
					isotp_received(isoMessageIncoming);
					// isoMessageIncoming.id = 0xffff; // cancel this message so nothing will be added until it is re-initialized
					isotp_reset();
					return;
				}

				// first frame of a multi-framed message *********************************
				if (type == 0x1)
				{
					if (AppSettings.IsoTpDebug)
					{
						Debug.WriteLine("< can:ISO FRST:" + canFrameToString(frame));
					}
					isotp_reset();
					isoMessageIncoming.index = 0;

					// start by requesting requesing the type Consecutive (0x2) frames by sending a Flow frame
					//can_send_flow(isoMessageOutgoing.id);	//Handled in device

					ushort messageLength = (ushort)((frame.data[0] & 0x0f) << 8); // length = second nibble + second byte
					messageLength |= frame.data[1];
					if (messageLength > 4096)
					{
						Debug.WriteLine("< can: length FRST > 4096");
						isotp_reset();
						return;
					}

					isoMessageIncoming.length = messageLength;
					for (int i = 2; i < 8; i++)
					{
						isoMessageIncoming.data[isoMessageIncoming.index++] = frame.data[i];
					}
					return;
				}

				// consecutive frame(s) **************************************************
				if (type == 0x2)
				{
					if (AppSettings.IsoTpDebug)
					{
						Debug.WriteLine("< can:ISO NEXT:" + canFrameToString(frame));
					}

					byte sequence = (byte)(frame.data[0] & 0x0f);
					if (isoMessageIncoming.next != sequence)
					{
						if (AppSettings.IsoTpDebug)
							Debug.WriteLine("< can:ISO Out of sequence, resetting");
						isotp_reset();
					}

					for (int i = 1; i < frame.FIR.DLC && isoMessageIncoming.index < isoMessageIncoming.length; i++)
					{
						isoMessageIncoming.data[isoMessageIncoming.index++] = frame.data[i];
					}

					// wait for next message, rollover from 15 to 0
					isoMessageIncoming.next = (byte)(isoMessageIncoming.next == 15 ? 0 : isoMessageIncoming.next + 1);

					// is this the last part?
					if (isoMessageIncoming.index == isoMessageIncoming.length)
					{
						// output the data
						String dataString = isoMessageToString(isoMessageIncoming);
						if (AppSettings.IsoTpDebug)
							Debug.WriteLine("< can:ISO MSG:" + dataString);
						isotp_received(isoMessageIncoming);
						isotp_reset();
					}
					return;
				}

				// incoming flow control ***********************************************
				if (type == 0x3)
				{
					if (AppSettings.IsoTpDebug)
						Debug.WriteLine("< can:ISO FLOW");
					if (isoMessageOutgoing.index >= isoMessageOutgoing.length)
                    {
						Debug.WriteLine("Incoming flow control, but no next frame waiting??");
						return;
                    }
					//uint8_t flag = isoMessageIncoming.data[0] &0x0f;
					isoMessageOutgoing.flow_counter = frame.data[1];
					isoMessageOutgoing.flow_delay = (uint)(frame.data[2] <= 127 ? frame.data[2] : (frame.data[2] - 0xf0) /1000);
					// to avoid overwhelming the outgoing queue, set minimum to 5 ms 
					// this is experimental.
					if (isoMessageOutgoing.flow_delay < 5)
						isoMessageOutgoing.flow_delay = 5;
					isoMessageOutgoing.flow_active = true;
					lastTime = DateTime.Now;
					return;
				}

				if (AppSettings.IsoTpDebug)
					Debug.WriteLine("< can:ISO ignoring unknown frame type: " + type.ToString());
				isotp_reset();
			}
			catch (Exception ex)
			{
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var errframe = st.GetFrame(st.FrameCount - 1);
				// Get the line number from the stack frame
				var line = errframe.GetFileLineNumber();
				Debug.WriteLine("Error, isotphandler line " + line + ": " + ex.Message);
			}
		}

		/**
		 * Send a flow control frame
		 * @param id id ID of the message
		 * @param bus CANbus (only 0 allowed)
		 */
		private void can_send_flow(uint id)
		{
			CAN_frame_t flow = new CAN_frame_t();
			flow.FIR.FF = id < 0x800 ? CAN_frame_format_t.CAN_frame_std : CAN_frame_format_t.CAN_frame_ext;
			flow.FIR.RTR = CAN_RTR_t.CAN_no_RTR; // no RTR
			flow.MsgID = id;             // send it to the requestId
			flow.FIR.DLC = 8;          // length 8 bytes
			flow.data[0] = 0x30;      // type Flow (3), flag Clear to send (0)
			flow.data[1] = 0x00;      // instruct to send all remaining frames without flow control
			flow.data[2] = 0x00;      // delay between frames <=127 = millis, can maybe set to 0
			flow.data[3] = 0;         // fill-up
			flow.data[4] = 0;         // fill-up
			flow.data[5] = 0;         // fill-up
			flow.data[6] = 0;         // fill-up
			flow.data[7] = 0;         // fill-up
			can_send(flow);
		}

		/**
		 * Send an ISO-TP message
		 * @param id ID of the message
		 * @param length of the payload
		 * @param request payload
		 * @param bus CANbus (only 0 allowed)
		 */
		public void requestIsotp(uint id, byte[] request)
		{
			try
			{
				CAN_frame_t frame = new CAN_frame_t(); // build the CAN frame

				isotp_reset(); // cancel possible ongoing IsoTp run

				CANDevice CanDevice = CANQuery.GetDeviceAddresses((ushort)id);
				isoMessageIncoming.id = (uint)CanDevice.ResID;   // expected ID of answer
				isoMessageIncoming.index = 0; // starting
				isoMessageIncoming.next = 1;
				isoMessageOutgoing.id = id;
				if (isoMessageOutgoing.id == 0) // ID to send request to
				{
					if (AppSettings.IsoTpDebug)
						Debug.WriteLine("> can:" + id.ToString("X4") + " has no corresponding request ID");
					//isotp_config->output_handler(String(id, HEX) + "\n");
					isotp_reset();
					return;
				}
				// store request to send
				isoMessageOutgoing.length = (ushort)request.Length;
				if (isoMessageOutgoing.length > 4096)
				{
					Debug.WriteLine("> can:length request > 4096");
					isotp_reset();
					return;
				}

				for (ushort i = 0; i < request.Length; i++)
				{
					isoMessageOutgoing.data[i] = request[i];
				}
				isoMessageOutgoing.index = 0; // start at the beginning
				isoMessageOutgoing.next = 1;

				// Prepare the initial frame
				frame.FIR.FF = isoMessageOutgoing.id < 0x800 ? CAN_frame_format_t.CAN_frame_std : CAN_frame_format_t.CAN_frame_ext;
				frame.FIR.RTR = CAN_RTR_t.CAN_no_RTR;        // no RTR
				frame.MsgID = isoMessageOutgoing.id; // set the ID
				frame.FIR.DLC = 8;                 //command.requestLength + 1;// set the length. Note some ECU's like DLC 8
				frame.Bus = this.Bus;

				if (isoMessageOutgoing.length <= 7)
				{ // send SING frame
				  // prepare the frame
					frame.data[0] = (byte)(isoMessageOutgoing.length & 0x0f);
					for (int i = 0; i < isoMessageOutgoing.length; i++)
					{ // fill up the other bytes with the request
						frame.data[i + 1] = isoMessageOutgoing.data[i];
					}
					for (int i = isoMessageOutgoing.length; i < 7; i++)
					{
						frame.data[i + 1] = 0; // zero out frame
					}

					// debug
					if (AppSettings.IsoTpDebug)
					{
						Debug.WriteLine("> can:Sending ISOTP SING request:" + canFrameToString(frame));
					}

					// send the frame
					can_send(frame);

					// --> any incoming frames with the given id will be handled by "storeFrame"
					// and send off if complete. But ensure the ticker doesn't do any flow_block
					// control
					isoMessageOutgoing.length = isoMessageOutgoing.index = 0;
				}

				else
				{ // send a FIRST frame
				  // prepare the firt frame
					frame.data[0] = (byte)(0x10 + ((request.Length >> 8) & 0x0f));
					frame.data[1] = (byte)(request.Length & 0xff);
					for (int i = 0; i < 6; i++)
					{ // fill up the other bytes with the first 6 bytes of the request
						frame.data[i + 2] = isoMessageOutgoing.data[isoMessageOutgoing.index++];
					}

					// debug
					if (AppSettings.IsoTpDebug)
					{
						Debug.WriteLine("> can:Sending ISOTP FRST request:" + canFrameToString(frame));
					}

					// send the frame
					can_send(frame);
					// --> any incoming frames with the given id will be handled by "storeFrame" and send off if complete
				}
			}
			catch (Exception ex)
			{
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var errframe = st.GetFrame(st.FrameCount - 1);
				// Get the line number from the stack frame
				var line = errframe.GetFileLineNumber();
				Debug.WriteLine("Error, isotphandler line " + line + ": " + ex.Message);
			}
		}

		/**
		 * Convert a ISO-TP message to readable hex output format, newline terminated
		 * @param message Pointer to the ISO_MESSAGE_t struct
		 */
		String isoMessageToString(ISO_MESSAGE_t message)
		{
			String dataString = message.id.ToString("X") + ",";
			for (int i = 0; i < message.length; i++)
			{
				dataString += message.data[i].ToString("X2");
			}
			return dataString;
		}
		String canFrameToString(CAN_frame_t frame)
		{
			String dataString = frame.MsgID.ToString("X") + ",";
			for (int i = 0; i < frame.FIR.DLC; i++)
			{
				dataString += frame.data[i].ToString("X2");
			}
			return dataString;
		}

		//Can sending
		public class CanFrameEventparameter : EventArgs
		{
			public CanFrameEventparameter(CAN_frame_t frame)
			{
				this.frame = frame;
			}
			public CAN_frame_t frame { get; internal set; }
		}
		public event EventHandler<CanFrameEventparameter> CanSend;

		protected virtual void OnCanSend(CanFrameEventparameter e)
		{
			CanSend?.Invoke(this, e);
		}
		protected void can_send(CAN_frame_t frame)
		{
			frame.Bus = this.Bus;
			CanFrameEventparameter msg = new CanFrameEventparameter(frame);
			OnCanSend(msg);
		}

		//IsoTP message receiving
		public class IsoTpFrameEventparameter : EventArgs
		{
			public IsoTpFrameEventparameter(ISO_MESSAGE_t message)
			{
				this.message = message;
			}
			public ISO_MESSAGE_t message { get; internal set; }
		}
		public event EventHandler<IsoTpFrameEventparameter> IsoTpReceived;

		protected void isotp_received(ISO_MESSAGE_t message)
		{
			message.Bus = this.Bus;
			IsoTpFrameEventparameter msg = new IsoTpFrameEventparameter(message);			
			OnIsoTpReceived(msg);
		}

		protected virtual void OnIsoTpReceived(IsoTpFrameEventparameter e)
		{
			IsoTpReceived?.Invoke(this, e);
		}

	}
}