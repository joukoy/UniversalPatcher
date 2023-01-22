using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Upatcher;

namespace UniversalPatcher
{
    /// <summary>
    /// This class is responsible for sending and receiving data over a serial port.
    /// I would have called it 'SerialPort' but that name was already taken...
    /// </summary>
    class Rs232Port : IPort
    {
        private string name;
        private SerialPort port;
        private bool promptReceived = false;
        private Queue<SerialByte> internalQueue = new Queue<SerialByte>();
        private readonly object devLock = new object();

        private CancellationTokenSource receiverTokenSource = new CancellationTokenSource();
        private CancellationToken receiverToken;

        public bool PromptReceived()
        {
            return this.promptReceived;
        }

        private int RTimeout;
        /// <summary>
        /// This is an experiment that did not end well the first time, but I still think it should work.
        /// </summary>
        // private Action<object, SerialDataReceivedEventArgs> dataReceivedCallback;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Rs232Port(string name)
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
            Debug.WriteLine("Closing port (ClosePort)");
            this.port.DataReceived -= DataReceived;
            if (this.port.IsOpen)
            {
                this.port.Close();
            }
        }
        /// <summary>
        /// Open the serial port.
        /// </summary>
        void IPort.OpenAsync(PortConfiguration configuration)
        {
            // Clean up the existing SerialPort object, if we have one.
            if (this.port != null)
            {
                this.port.DataReceived -= DataReceived;
                if (this.port.IsOpen)
                {
                    Debug.WriteLine("Port still open, closing before Dispose!");
                    this.port.Close();
                }
                this.port.Dispose();
            }
            SerialPortConfiguration config = configuration as SerialPortConfiguration;
            this.port = new SerialPort(this.name);
            this.port.BaudRate = config.BaudRate;
            this.port.DataBits = 8;
            this.port.Parity = Parity.None;
            this.port.StopBits = StopBits.One;
            if (config.Timeout == 0) config.Timeout = 1000; // default to 1 second but allow override.
            this.port.ReadTimeout = config.Timeout;
            this.port.WriteTimeout = config.Timeout;
            //For event handling:
            this.port.ReceivedBytesThreshold = 1;
            this.port.RtsEnable = true;
            RTimeout = config.Timeout;

            this.port.Open();

            // This line must come AFTER the call to port.Open().
            // Attempting to use the BaseStream member will throw an exception otherwise.
            //
            // However, even after setting the BaseStream.ReadTimout property, calls to
            // BaseStream.ReadAsync will hang indefinitely. It turns out that you have 
            // to implement the timeout yourself if you use the async approach.

            receiverTokenSource = new CancellationTokenSource();
            receiverToken = receiverTokenSource.Token;

            this.port.BaseStream.ReadTimeout = this.port.ReadTimeout;
            this.port.WriteTimeout = 500;
            this.port.ErrorReceived += Port_ErrorReceived;
            this.port.DataReceived += DataReceived;
            //Task receiveTask = Task.Factory.StartNew(() => DataReceiverLoop());
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

        /// <summary>
        /// Close the serial port.
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine("Closing port (Dispose)");
            if (this.port != null)
            {
                if (this.port.IsOpen)
                {
                    this.port.Close();
                    //Thread.Sleep(100);
                }
                this.port.Dispose();
                this.port = null;
            }
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
            //this.port.DataReceived -= DataReceived;
            lock (devLock)
            {
                this.port.BaseStream.Write(buffer, 0, buffer.Length); //.AwaitWithTimeout(TimeSpan.FromSeconds(5));
            }
            // This flush is probably not strictly necessary, but just in case...
            this.port.BaseStream.Flush(); //.AwaitWithTimeout(TimeSpan.FromSeconds(5));
            //this.port.Write(buffer, 0, buffer.Length);
            //this.port.DataReceived += DataReceived;
/*            if (buffer.Length > 5)
            {
                Analyzer.VPWRow vrow = PcmLogger.analyzer.ProcessLine(buffer);
                Analyzer.AnalyzerData msg = PcmLogger.analyzer.AnalyzeMsg(vrow);
                PcmLogger.analyzer.aData.Add(msg);
            }
*/
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
            if (this.port != null && !this.port.IsOpen)
            {
                this.Dispose();
                return 0;
            }
            int rCount = 0;
            int pos = offset;
            DateTime startTime = DateTime.Now;
            receiverTokenSource = new CancellationTokenSource();
            receiverToken = receiverTokenSource.Token;
            //Debug.WriteLine("RS232 reading: " + count + ", available: " + this.internalQueue.Count);
            while (this.internalQueue.Count < count)
            {
                Thread.Sleep(1);
                if (DateTime.Now.Subtract(startTime) > TimeSpan.FromMilliseconds(RTimeout))
                {
                    //Debug.WriteLine("RS232 waiting for: " + count + ", available: " + this.internalQueue.Count);
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
            return rCount;
        }

        /// <summary>
        /// Discard anything in the input and output buffers.
        /// </summary>
        public void DiscardBuffers()
        {
            this.port.DiscardInBuffer();
            lock (this.internalQueue)
            {
                this.internalQueue.Clear();
            }
            this.port.DiscardOutBuffer();
        }

        /// <summary>
        /// Sets the read timeout.
        /// </summary>
        public void SetTimeout(int milliseconds)
        {
            try
            {
                this.port.ReadTimeout = milliseconds;
                //RTimeout = milliseconds;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Sets the write timeout.
        /// </summary>
        public void SetWriteTimeout(int milliseconds)
        {
            Debug.WriteLine("Setting write timeout to: " + milliseconds.ToString());
            this.port.WriteTimeout = milliseconds;
        }

        /// <summary>
        /// Sets the Read timeout.
        /// </summary>
        public void SetReadTimeout(int milliseconds)
        {
            Debug.WriteLine("Setting read timeout to: " + milliseconds.ToString());
            RTimeout = milliseconds;
        }

        /// <summary>
        /// Serial data loop
        /// </summary>
        private void DataReceiverLoop()
        {
                while (true)
                {
                    try
                    {
                        if (port == null)
                        {
                            Debug.WriteLine("Port closed, exit RS232 loop");
                            return;
                        }

                        int bytes = this.port.BytesToRead;
                        if (bytes > 0)
                        {
                            byte[] rx = new byte[bytes];

                            int received = this.port.Read(rx, 0, bytes);
                            //Debug.WriteLine("RS232: " + Encoding.ASCII.GetString(rx));
                            lock (this.internalQueue)
                            {
                                for (int i = 0; i < bytes; i++)
                                {
                                    SerialByte sb = new SerialByte(1);
                                    sb.Data[0] = rx[i];
                                    this.internalQueue.Enqueue(sb);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("RS232 Receive: " + ex.Message);
                    }
            
                }
        }

        /// <summary>
        /// Serial data callback. Didn't work the first time, but I still have hopes...
        /// </summary>
        private void DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            try
            {
                List<SerialByte> rBytes = new List<SerialByte>();
                int bytes = this.port.BytesToRead;

                while (bytes > 0)
                {
                    byte[] rx = new byte[bytes];
                    int received = this.port.Read(rx, 0, bytes);
                    for (int i = 0; i < bytes; i++)
                    {
                        SerialByte sb = new SerialByte(1);
                        sb.Data[0] = rx[i];
                        rBytes.Add(sb);
                    }
                    bytes = this.port.BytesToRead;
                }
                //Debug.WriteLine("RS232: " + Encoding.ASCII.GetString(rx));
                lock (this.internalQueue)
                {
                    for (int i = 0; i < rBytes.Count; i++)
                    {
                        this.internalQueue.Enqueue(rBytes[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RS232 Receive: " + ex.Message);
            }
        }

        /// <summary>
        /// Indicates the number of bytes waiting in the queue.
        /// </summary>
        int IPort.GetReceiveQueueSize()
        {
            //return this.port.BytesToRead;
            return internalQueue.Count;
        }
    }
}

