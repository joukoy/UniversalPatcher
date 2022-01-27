using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcmHacking
{
    /// <summary>
    /// This class is just here to enable testing without any actual interface hardware.
    /// </summary>
    /// <remarks>
    /// Eventually the Receive method should return simulated VPW responses.
    /// </remarks>
    public class MockPort : IPort
    {
        public const string PortName = "Mock Port";

        private MockPcm pcm;
    
        public MockPort(ILogger logger)
        {
            this.pcm = new MockPcm(logger);
        }

        /// <summary>
        /// This returns the string that appears in the drop-down list.
        /// </summary>
        public override string ToString()
        {
            return PortName;
        }

        /// <summary>
        /// Pretend to open a port.
        /// </summary>
        void IPort.OpenAsync(PortConfiguration configuration)
        {
        }

        /// <summary>
        /// Pretend to close a port.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Send bytes to the mock PCM.
        /// </summary>
        void IPort.Send(byte[] buffer)
        {
            this.pcm.ResetCommunications();

            foreach(byte b in buffer)
            {
                this.pcm.Push(b);
            }

            this.pcm.EndOfData();
        }

        /// <summary>
        /// Receive bytes from the mock PCM.
        /// </summary>
        int IPort.Receive(byte[] buffer, int offset, int count)
        {
            byte[] responseBuffer = this.pcm.GetResponse();

            int index = 0;
            for(; index < count && index < responseBuffer.Length; index++)
            {
                buffer[offset + index] = responseBuffer[index];
            }

            return index;
        }

        /// <summary>
        /// Discard anything in the input and output buffers.
        /// </summary>
        public void DiscardBuffers()
        {
        }

        /// <summary>
        /// Sets the read timeout.
        /// </summary>
        public void SetTimeout(int milliseconds)
        {
        }

        /// <summary>
        /// Indicates the number of bytes waiting in the queue.
        /// </summary>
        int IPort.GetReceiveQueueSize()
        {
            // return Task.FromResult(0);
            throw new NotImplementedException();
        }
    }
}
