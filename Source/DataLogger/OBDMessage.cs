using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private ulong timestamp;

        /// <summary>
        /// When the message was created (System time).
        /// </summary>
        private ulong systimestamp;

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
        public OBDMessage(byte[] message)
        {
            this.message = message;
            this.timestamp = (ulong)DateTime.Now.Ticks;
            this.systimestamp = (ulong)DateTime.Now.Ticks;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OBDMessage(byte[] message, ulong timestamp, ulong error)
        {
            this.message = message;
            this.timestamp = timestamp;
            this.systimestamp = (ulong)DateTime.Now.Ticks;
            this.error = error;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// When the message was created or recevied (tool time).
        /// </summary>
        public ulong TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// When the message was created or recevied (System time).
        /// </summary>
        public ulong SysTimeStamp
        {
            get { return this.systimestamp; }
            set { this.systimestamp = value; }
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

        public bool ElmPrompt { get; set; }
        public string ElmLine { get; set; }
        public bool SecondaryProtocol { get; set; }
    }
}
