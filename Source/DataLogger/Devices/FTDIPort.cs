using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FTD2XX_NET;
using static Upatcher;

namespace UniversalPatcher
{
    /// <summary>
    /// This class is responsible for sending and receiving data over a serial port.
    /// I would have called it 'SerialPort' but that name was already taken...
    /// </summary>
    class FTDIPort : IPort
    {
        private string name;
        private FTDI port;
        private Queue<SerialByte> internalQueue = new Queue<SerialByte>();
        private int RTimeout = 500;
        private int WTimeout = 500;
        private static EventWaitHandle waitHandle;
        private bool promptReceived = false;
        private Task receiveTask;
        private readonly object devLock = new object();
        private CancellationTokenSource receiverTokenSource = new CancellationTokenSource();
        private CancellationToken receiverToken;

        public bool PromptReceived()
        {
            return this.promptReceived;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FTDIPort(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// This returns the string that appears in the drop-down list.
        /// </summary>
        public override string ToString()
        {
            return this.name;
        }

        bool IPort.PortOpen()
        {
            return port.IsOpen;
        }

        void IPort.ClosePort()
        {
            //ev.Set();
            if (this.port.IsOpen)
            {
                this.port.Close();
            }
        }

        /// <summary>
        /// Open the FTDI port.
        /// </summary>
        void IPort.OpenAsync(PortConfiguration configuration)
        {
            // Clean up the existing FTDI object, if we have one.
            if (this.port != null)
            {
                if (this.port.IsOpen)
                {
                    Debug.WriteLine("Port still open, closing before Dispose!");
                    this.port.Close();
                }
                this.port = null;
            }


            SerialPortConfiguration config = configuration as SerialPortConfiguration;
            this.port = new FTDI();
            RTimeout = config.Timeout;

            FTDI.FT_STATUS ftStatus = port.OpenBySerialNumber(this.name);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to open device (error " + ftStatus.ToString() + ")");
            }

            ftStatus = this.port.SetBaudRate((uint)config.BaudRate);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set baud rate (error " + ftStatus.ToString() + ")");
            }

            ftStatus = this.port.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
            }

            if (config.Timeout == 0) config.Timeout = 1000; // default to 1 second but allow override.
            ftStatus = this.port.SetTimeouts((uint)config.Timeout, 500);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set timeout (error " + ftStatus.ToString() + ")");
            }

            ftStatus = this.port.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0x11, 0x13);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set flow control (error " + ftStatus.ToString() + ")");
            }

            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            ftStatus = this.port.SetEventNotification(FTD2XX_NET.FTDI.FT_EVENTS.FT_EVENT_RXCHAR, waitHandle);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception("Failed to set FTDI event handler (error " + ftStatus.ToString() + ")");
            }
            receiverTokenSource = new CancellationTokenSource();
            receiverToken = receiverTokenSource.Token;

            //ThreadPool.QueueUserWorkItem(new WaitCallback(DataReceived), null);
            receiveTask = Task.Factory.StartNew(() => DataReceiver());
        }

        //private void DataReceived(object threadContext)
        private void DataReceiver()
        {
            try
            {
                Thread.CurrentThread.IsBackground = true;
                Debug.WriteLine("FTDI loop started");
                while (true)
                {
                    waitHandle.WaitOne();

                    if (port == null)
                    {
                        Debug.WriteLine("Port closed, exit FTDI loop");
                        return;
                    }
                    uint bytes = 0;
                    FTDI.FT_STATUS ftst = this.port.GetRxBytesAvailable(ref bytes);
                    if (ftst != FTDI.FT_STATUS.FT_OK)
                    {
                        Debug.WriteLine("FTDI error: " + ftst.ToString());
                        continue;
                    }
                    byte[] rx = new byte[bytes];

                    uint numBytesRead = 0;
                    port.Read(rx, bytes, ref numBytesRead);
                    lock (this.internalQueue)
                    {
                        for (int i = 0; i < bytes; i++)
                        {
                            SerialByte sb = new SerialByte(1);
                            sb.Data[0] = rx[i];
                            this.internalQueue.Enqueue(sb);
                        }
                    }
                    //Debug.WriteLine("Receiving FTDI data, bytes: " + BitConverter.ToString(rx));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FTDI error: " + ex.Message);
            }
        }

        /// <summary>
        /// Close the serial port.
        /// </summary>
        public void Dispose()
        {
            if (this.port != null)
            {
                if (this.port.IsOpen)
                {
                    this.port.Close();
                    //Thread.Sleep(100);
                }
                this.port = null;
            }
            waitHandle.Set();
        }

        /// <summary>
        /// Send a sequence of bytes over the serial port.
        /// </summary>
        void IPort.Send(byte[] buffer)
        {
            if (port == null || !port.IsOpen)
            {
                return;
            }
            uint bytes = 0;
            lock (devLock)
            {
                FTDI.FT_STATUS ftStatus = port.Write(buffer, buffer.Length, ref bytes);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    Debug.WriteLine("FTDI write error: " + ftStatus.ToString());
                }
            }
        }

        /// <summary>
        /// Cancel current receive task
        /// </summary>
        void IPort.CancelReceive()
        {
            receiverTokenSource.Cancel();
        }

        /// <summary>
        /// Receive a sequence of bytes over the serial port.
        /// </summary>
        int IPort.Receive(SerialByte buffer, int offset, int count)
        {
            // Using the BaseStream causes data to be lost.
            //return this.port.Read(buffer, offset, count);
            int rCount = 0;
            int pos = offset;
            DateTime startTime = DateTime.Now;
            receiverTokenSource = new CancellationTokenSource();
            receiverToken = receiverTokenSource.Token;

            while (this.internalQueue.Count < count)
            {
                Thread.Sleep(1);
                if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(RTimeout))
                {
                    Debug.WriteLine("FTDI waiting for: " + count + ", received: " + this.internalQueue.Count);
                    throw new TimeoutException();
                }
                if (receiverToken.IsCancellationRequested)
                {
                    Debug.WriteLine("Receive cancelled");
                    throw new TimeoutException();
                }
            }
            lock (this.internalQueue)
            {
                SerialByte sb = internalQueue.Dequeue();
                buffer.TimeStamp = sb.TimeStamp;
                buffer.Data[pos] = sb.Data[0];
                pos++;
                rCount++;
                while (rCount < count)
                {
                    buffer.Data[pos] = internalQueue.Dequeue().Data[0];
                    pos++;
                    rCount++;
                    datalogger.ReceivedBytes++;
                }
            }
            //Debug.WriteLine("FTDI RX: " + BitConverter.ToString(buffer.Data));
            return rCount;
        }

        /// <summary>
        /// Discard anything in the input and output buffers.
        /// </summary>
        public void DiscardBuffers()
        {
            this.port.Purge(FTDI.FT_PURGE.FT_PURGE_RX | FTDI.FT_PURGE.FT_PURGE_TX);
        }

        /// <summary>
        /// Sets the read timeout.
        /// </summary>
        public void SetTimeout(int milliseconds)
        {
            this.port.SetTimeouts((uint)milliseconds,(uint)WTimeout);
            Debug.WriteLine("Setting FTDI timeout to: " + milliseconds.ToString());
            //RTimeout = milliseconds;
        }

        /// <summary>
        /// Sets the write timeout.
        /// </summary>
        public void SetWriteTimeout(int milliseconds)
        {
            this.port.SetTimeouts((uint)RTimeout, (uint)milliseconds);
            WTimeout = milliseconds;
        }

        /// <summary>
        /// Sets the Read timeout.
        /// </summary>
        public void SetReadTimeout(int milliseconds)
        {
            RTimeout = milliseconds;
        }

        /// <summary>
        /// Indicates the number of bytes waiting in the queue.
        /// </summary>
        int IPort.GetReceiveQueueSize()
        {
            return internalQueue.Count;
        }
    }
}

