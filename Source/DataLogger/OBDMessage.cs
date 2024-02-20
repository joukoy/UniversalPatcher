using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Helpers;

namespace UniversalPatcher
{
    /// <summary>
    /// Message is a thin wrapper around an array of bytes.
    /// </summary>
    /// <remarks>
    /// I'll admit that this might be overkill as opposed to just passing around byte arrays.
    /// But the ToString method makes messages easier to view in the debugger.
    /// </remarks>
    public class OBDMessage
    {
        /// <summary>
        /// The message content.
        /// </summary>
        private byte[] message;

        /// <summary>
        /// When the message was created (Tool timestamp if available).
        /// </summary>
        private long timestamp;

        /// <summary>
        /// Error code, if applicable.
        /// </summary>
        private ulong error;

        /// <summary>
        /// Returns the length of the message.
        /// </summary>
        public int Length
        {
            get
            {
                if (this.message == null)
                    return 0;
                return this.message.Length;
            }
        }

        /// <summary>
        /// Get the Nth byte of the message.
        /// </summary>
        public byte this[int index]
        {
            get
            {
                return this.message[index];
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OBDMessage()
        {
            this.timestamp = DateTime.Now.Ticks;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OBDMessage(byte[] message)
        {
            this.message = message;
            this.timestamp = DateTime.Now.Ticks;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OBDMessage(byte[] message, long timestamp, ulong error)
        {
            this.message = message;
            this.timestamp = timestamp;
            this.error = error;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// When the message was created or recevied (System time).
        /// </summary>
        public long TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// The error associated with creating or receiving this message.
        /// </summary>
        public ulong Error
        {
            get { return this.error; }
            set { this.error = value; }
        }

        /// <summary>
        /// Message can't be serialized without public access to data...
        /// </summary>
        public byte[] Data
        {
            get { return this.message; }
            set { this.message = value; }
        }

        /// <summary>
        /// Get the raw bytes.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public byte[] GetBytes()
        {
            return this.message;
        }

        /// <summary>
        /// Generate a descriptive string for this message.
        /// </summary>
        /// <remarks>
        /// This is the most valuable thing - it makes messages easy to view in the debugger.
        /// </remarks>
        public override string ToString()
        {
            return string.Join(" ", Array.ConvertAll(message, b => b.ToString("X2")));
        }

        /// <summary>
        /// When Elm device is waiting for new command, it shows prompt >
        /// </summary>
        public bool ElmPrompt { get; set; }

        //public string ElmLine { get; set; }

        /// <summary>
        /// J2534 Console can connect to two protocols
        /// </summary>
        public bool SecondaryProtocol { get; set; }

        /// <summary>
        /// J2534 TxFlags
        /// </summary>
        public J2534DotNet.TxFlag Txflag { get; set; }

        /// <summary>
        /// J2534 RxStatus
        /// </summary>
        public J2534DotNet.RxStatus Rxstatus { get; set; }

        /// <summary>
        /// J2534 device original timestamp, no conversions
        /// </summary>
        public long DeviceTimestamp { get; set; }

    }
}
