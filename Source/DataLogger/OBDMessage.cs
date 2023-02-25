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
        private ulong timestamp;

        /// <summary>
        /// When the message was created (From device timestamp).
        /// </summary>
        private ulong devtimestamp;

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
            this.timestamp = (ulong)DateTime.Now.Ticks;
            this.devtimestamp = (ulong)DateTime.Now.Ticks;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OBDMessage(byte[] message)
        {
            this.message = message;
            this.timestamp = (ulong)DateTime.Now.Ticks;
            this.devtimestamp = (ulong)DateTime.Now.Ticks;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OBDMessage(byte[] message, ulong timestamp, ulong error)
        {
            this.message = message;
            this.timestamp = timestamp;
            this.devtimestamp = (ulong)DateTime.Now.Ticks;
            this.error = error;
            SecondaryProtocol = false;
        }

        /// <summary>
        /// When the message was created or recevied (System time).
        /// </summary>
        public ulong TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// When the message was created or received (Device time).
        /// </summary>
        public ulong DevTimeStamp
        {
            get { return this.devtimestamp; }
            set { this.devtimestamp = value; }
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
        /// When sending message in named pipe (byte array) it is organized: timestamp, devtimestamp, Elmrompt, Secondaryprotocol, message data 
        /// NOT IN USE
        /// </summary>
        public void FromPipeMessage(byte[] msg)
        {
            int ulen = sizeof(ulong);
            if (msg.Length > ((2 * ulen) + 2))
            {
                timestamp = BitConverter.ToUInt64(msg, 0);
                devtimestamp = BitConverter.ToUInt64(msg, ulen);
                ElmPrompt = Convert.ToBoolean(msg[2 * ulen]);
                SecondaryProtocol = Convert.ToBoolean(msg[2 * ulen + 1]);
                message = new byte[msg.Length - (2 * ulen) - 2];
                Array.Copy(msg, (2 * ulen) + 2, message, 0, message.Length);
            }
        }

        /// NOT IN USE
        public byte[] ToPipeMessage()
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(BitConverter.GetBytes(timestamp));
            msg.AddRange(BitConverter.GetBytes(devtimestamp));
            msg.Add(Convert.ToByte(ElmPrompt));
            msg.Add(Convert.ToByte(SecondaryProtocol));
            msg.AddRange(message);
            return msg.ToArray();
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
    }
}
