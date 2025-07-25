using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalPatcher
{
    /// <summary>
    /// The IPort implementations encapsulate the differences between serial 
    /// ports, J2534 passthrough devices, and whatever else we end up using.
    /// </summary>
    public interface IPort : IDisposable
    {
        /// <summary>
        /// Open the port.
        /// </summary>
        void OpenAsync(PortConfiguration configuration);

        /// <summary>
        /// Close the port.
        /// </summary>
        void ClosePort();

        /// <summary>
        /// Send a sequence of bytes.
        /// </summary>
        void Send(byte[] buffer);

        /// <summary>
        /// Receive a buffer of bytes.
        /// </summary>
        int Receive(SerialByte buffer, int offset, int count);

        /// <summary>
        /// Discard anything in the input and output buffers.
        /// </summary>
        void DiscardBuffers();

        /// <summary>
        /// Indicates the number of bytes waiting in the receive queue.
        /// </summary>
        int GetReceiveQueueSize();

        /// <summary>
        /// Sets the timeout for incoming messages;
        /// </summary>
        void SetTimeout(int milliseconds);

        /// <summary>
        /// Sets the timeout for sending messages;
        /// </summary>
        void SetWriteTimeout(int milliseconds);

        /// <summary>
        /// Sets the timeout for reading messages;
        /// </summary>
        void SetReadTimeout(int milliseconds);

        /// <summary>
        /// Cancel current receive;
        /// </summary>
        void CancelReceive();

        /// <summary>
        /// Get status of port;
        /// </summary>
        bool PortOpen();

        /// <summary>
        /// Sets the timeout for incoming messages;
        /// </summary>
        // bool PromptReceived();

        /*
                /// <summary>
                /// Set callback function for event handler
                /// </summary>
                void SetEventHandler(bool passive);

                /// <summary>
                /// UnSet callback function for event handler
                /// </summary>
                void StopEventHandler();
        */
    }

    public class PortConfiguration
    {

    }


    public class SerialPortConfiguration : PortConfiguration
    {
        public SerialPortConfiguration()
        {
            RtsEnable = true;
            DtrEnable = false;
        }
        public int BaudRate { get; set; }
        public int Timeout { get; set; }
        public bool RtsEnable { get; set; }
        public bool DtrEnable { get; set; }
    }
    public class TcpIpConfiguration : PortConfiguration
    {
    }

}
