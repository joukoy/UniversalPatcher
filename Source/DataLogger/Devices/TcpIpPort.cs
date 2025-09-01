using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Upatcher;
using System.Collections.Concurrent;

namespace UniversalPatcher
{
    /// <summary>
    /// This class is responsible for sending and receiving data over a TCPIP 
    /// </summary>
    class TcpIpPort : IPort
    {
        private string name;
        private TcpClient client;
        private NetworkStream ns;
        private BlockingCollection<SerialByte> internalQueue = new BlockingCollection<SerialByte>();
        private readonly object devLock = new object();

        private CancellationTokenSource receiverTokenSource = new CancellationTokenSource();
        private CancellationToken receiverToken;

        public bool PromptReceived()
        {
            return false;
        }

        private int RTimeout;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TcpIpPort(string name)
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
            return client.Connected;
        }

        void IPort.ClosePort()
        {
            receiverTokenSource.Cancel();
            if (client != null && client.Connected)
            {
                Debug.WriteLine("Closing port (ClosePort)");
                client.Close();
            }
        }
        /// <summary>
        /// Open the serial port.
        /// </summary>
        void IPort.OpenAsync(PortConfiguration configuration)
        {
            try
            {
                // Clean up the existing TCPIP object, if we have one.
                if (this.client != null)
                {
                    if (this.client.Connected)
                    {
                        Debug.WriteLine("Port still open, closing before reconnect!");
                        this.client.Close();
                    }
                }
                Debug.WriteLine("Opening port " + this.name);
                SerialPortConfiguration config = configuration as SerialPortConfiguration;
                string[] parts = this.name.Split(':');
                int port;
                if (parts.Length < 2 || !int.TryParse(parts[1], out port))
                {
                    port = 22222;
                }
                IPAddress ipAddress;
                if (!IPAddress.TryParse(parts[0], out ipAddress))
                {
                    IPHostEntry dnsInfo = Dns.GetHostEntry(parts[0]);
                    ipAddress = dnsInfo.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                }
                IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, port);
                client = new TcpClient();
                client.Connect(serverEndPoint);
                ns = client.GetStream();
                Debug.WriteLine("Connected to: " + this.name);
                if (config.Timeout == 0) config.Timeout = 1000; // default to 1 second but allow override.
                SetReadTimeout(config.Timeout);
                SetWriteTimeout(config.Timeout);
               
                receiverTokenSource = new CancellationTokenSource();
                receiverToken = receiverTokenSource.Token;
                Task.Factory.StartNew(() => DataReceiverLoop(), receiverToken);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, TcpIpPort line " + line + ": " + ex.Message);
            }
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
            receiverTokenSource.Cancel();
            if (client != null && client.Connected)
            {
                client.Close();
            }
            
        }

        /// <summary>
        /// Send a sequence of bytes over the TCPIP socket.
        /// </summary>
        void IPort.Send(byte[] buffer)
        {
            if (client == null || !client.Connected)
            {
                return;
            }
            lock (devLock)
            {
                ns.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Cancel current receive task
        /// </summary>
        void IPort.CancelReceive()
        {
            //receiverTokenSource.Cancel();
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
            int remainms = RTimeout;
            DateTime startTime = DateTime.Now;
            receiverTokenSource = new CancellationTokenSource();
            receiverToken = receiverTokenSource.Token;
            while (rCount < count)
            {
                if (internalQueue.TryTake(out SerialByte sb, remainms,receiverToken))
                {
                    buffer.Data[pos] = sb.Data[0];
                    pos++;
                    rCount++;
                    remainms = (int)(RTimeout - DateTime.Now.Subtract(startTime).TotalMilliseconds);
                }
                else
                {
                    //Debug.WriteLine("TCPIP port timeout: " + RTimeout.ToString());
                    throw new TimeoutException();
                    break;
                }
                if (receiverToken.IsCancellationRequested)
                {
                    Debug.WriteLine("Receive cancelled");
                    throw new TimeoutException();
                }
                if (this.client == null || !this.client.Connected)
                {
                    this.Dispose();
                    return 0;
                }
            }
            return rCount;
        }

        /// <summary>
        /// Discard anything in the input and output buffers.
        /// </summary>
        public void DiscardBuffers()
        {
            if (client == null)
            {
                return;
            }
            while (internalQueue.TryTake(out _)) { }
        }

        /// <summary>
        /// Sets the read timeout.
        /// </summary>
        public void SetTimeout(int milliseconds)
        {
            try
            {
                this.client.ReceiveTimeout = milliseconds;
                this.client.SendTimeout = milliseconds;
                Debug.WriteLine("Setting TCPIP timeout to: " + milliseconds.ToString());
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
            Debug.WriteLine("Setting TCPIP write timeout to: " + milliseconds.ToString());
            this.client.SendTimeout = milliseconds;
            this.ns.WriteTimeout = milliseconds;
        }

        /// <summary>
        /// Sets the Read timeout.
        /// </summary>
        public void SetReadTimeout(int milliseconds)
        {
            Debug.WriteLine("Setting port read timeout to: " + milliseconds.ToString());
            this.client.ReceiveTimeout = milliseconds;
            this.ns.ReadTimeout = milliseconds;
            RTimeout = milliseconds;
        }

        /// <summary>
        /// Serial data loop
        /// </summary>
        private void DataReceiverLoop()
        {
            Debug.WriteLine("TCPIP port starting receiver");
            while (!receiverToken.IsCancellationRequested )
            {
                try
                {
                    if (client == null || !client.Connected)
                    {
                        Debug.WriteLine("Port closed, exit TCPIP loop");
                        return;
                    }

                    SerialByte sb = new SerialByte(1);
                    //if (ns.DataAvailable)
                    {
                        int rx = ns.Read(sb.Data,0,1);
                        if (rx == 1)
                        {
                            this.internalQueue.Add(sb);
                        }
                    }
                    /*
                    else
                    {
                        Thread.Sleep(10);
                    }
                    */
                }
                catch (Exception ex)
                {
                    //Debug.WriteLine("TCPIP Receive: " + ex.Message);
                }
            
            }
            Debug.WriteLine("TCPIP port receiver stopped");
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

