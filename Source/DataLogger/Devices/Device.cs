using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Upatcher;
using static Helpers;
using System.Diagnostics;

namespace UniversalPatcher
{
    /// <summary>
    /// What the code was doing when a timeout happened.
    /// </summary>
    public enum TimeoutScenario
    {
        Undefined = 0,
        Minimum,
        ReadProperty,
        ReadCrc,
        SendKernel,
        ReadMemoryBlock,
        EraseMemoryBlock,
        WriteMemoryBlock,
        DataLogging1,
        DataLogging2,
        DataLogging3,
        DataLogging4,
        Maximum,
    }

    public class MsgEventparameter: EventArgs
    {
        public MsgEventparameter(OBDMessage msg)
        {
            this.Msg = msg;
        }
        public OBDMessage Msg { get; internal set; }
    }

    /// <summary>
    /// VPW can operate at two speeds. It is generally in standard (low speed) mode, but can switch to 4X (high speed).
    /// </summary>
    /// <remarks>
    /// High speed is better whend reading the entire contents of the PCM.
    /// Transitions to high speed must be negotiated, and any module that doesn't
    /// want to switch can force the bus to stay at standard speed. Annoying.
    /// </remarks>
    public enum VpwSpeed
    {
        /// <summary>
        /// 10.4 kpbs. This is the standard VPW speed.
        /// </summary>
        Standard,

        /// <summary>
        /// 41.2 kbps. This is the high VPW speed.
        /// </summary>
        FourX,
    }

    /// <summary>
    /// The Interface classes are responsible for commanding a hardware
    /// interface to send and receive VPW messages.
    /// 
    /// They use the IPort interface to communicate with the hardware.
    /// TODO: Move the IPort stuff into the SerialDevice class, since J2534 devices don't need it.
    /// </summary>
    public abstract class Device : IDisposable
    {
        /// <summary>
        /// Max transmit size.
        /// </summary>
        private int maxSendSize;

        /// <summary>
        /// For the AllPro, we need to tell the interface how long to listen for incoming messages.
        /// For other devices this is not so critical, however I suspect it might still be useful to set serial-port timeouts.
        /// </summary>
        protected TimeoutScenario currentTimeoutScenario = TimeoutScenario.Undefined;


        /// <summary>
        /// Maximum size of sent messages.
        /// </summary>
        /// <remarks>
        /// This is protected, not public, because consumers should use MaxKernelSendSize or MaxFlashWriteSendSize.
        /// </remarks>
        protected int MaxSendSize
        {
            get
            {
                return this.maxSendSize;
            }

            set
            {
                this.maxSendSize = value;
            }
        }

        /// <summary>
        /// Maximum packet size when sending the kernel.
        /// </summary>
        /// <remarks>
        /// For the P04 PCM, the kernel must be sent in a single message,
        /// so the constraint that we use for flash writing needs to be 
        /// ignored for that request.
        /// </remarks>
        public int MaxKernelSendSize
        {
            get
            {
                return this.maxSendSize;
            }
        }

        /// <summary>
        /// Maximum packet size when writing to flash memory.
        /// </summary>
        /// <remarks>
        /// This is smaller than the device's actual max send size, for 3 reasons:
        /// 1) P01/P59 kernels will currently crash with large writes.
        /// 2) Large packets are increasingly susceptible to noise corruption on the VPW bus.
        /// 3) Even on a clean VPW bus, the speed increases negligibly above 2kb.
        /// </remarks>
        public int MaxFlashWriteSendSize
        {
            get
            {
                return Math.Min(1024 + 12, this.maxSendSize);
            }
        }

        /// <summary>
        /// Maximum size of received messages.
        /// </summary>
        public int MaxReceiveSize { get; protected set; }

        /// <summary>
        /// Indicates whether or not the device supports 4X speed.
        /// </summary>
        public bool Supports4X { get; protected set; }

        /// <summary>
        /// Number of messages recevied so far.
        /// </summary>
        public int ReceivedMessageCount { get { return this.queue.Count; } }

        /// <summary>
        /// Queue of messages received from the VPW bus.
        /// </summary>
        private Queue<OBDMessage> queue = new Queue<OBDMessage>();

        /// <summary>
        /// Queue of Log messages received from the VPW bus.
        /// </summary>
        private Queue<OBDMessage> logQueue = new Queue<OBDMessage>();

        /// <summary>
        /// Number of messages recevied so far.
        /// </summary>
        public int LogMessageCount { get { return this.logQueue.Count; } }

        /// <summary>
        /// Current speed of the VPW bus.
        /// </summary>
        protected VpwSpeed Speed { get; private set; }

        /// <summary>
        /// Enable Disable VPW 4x.
        /// </summary>
        public bool Enable4xReadWrite { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Device()
        {
            // These default values can be overwritten in derived classes.
            this.MaxSendSize = 100;
            this.MaxReceiveSize = 100;
            this.Supports4X = false;
            this.Speed = VpwSpeed.Standard;
        }

        /// <summary>
        /// Finalizer (invoked during garbage collection).
        /// </summary>
        ~Device()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Clean up anything allocated by this instane.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Make the device ready to communicate with the VPW bus.
        /// </summary>
        public abstract bool Initialize(int BaudRate);

        /// <summary>
        /// Set the timeout period to wait for responses to incoming messages.
        /// </summary>
        public abstract TimeoutScenario SetTimeout(TimeoutScenario scenario);

        /// <summary>
        /// Send a message.
        /// </summary>
        public abstract bool SendMessage(OBDMessage message, int responses);

        /// <summary>
        /// Removes any messages that might be waiting in the incoming-message queue. Also clears the buffer.
        /// </summary>
        public void ClearMessageQueue()
        {
            Debug.WriteLine("Clearing: {0} messages from queue", queue.Count);
            this.queue.Clear();
            ClearMessageBuffer();
        }

        /// <summary>
        /// Removes any messages that might be waiting in the log-message queue. Also clears the buffer.
        /// </summary>
        public void ClearLogQueue()
        {
            this.logQueue.Clear();
            ClearMessageBuffer();
        }

        /// <summary>
        /// Clears Serial port buffer or J2534 api buffer
        /// </summary>
        public abstract void ClearMessageBuffer();


        /// <summary>
        /// Reads a message from the VPW bus and returns it.
        /// </summary>
        public OBDMessage ReceiveMessage()
        {
            if (this.queue.Count == 0)
            {
                this.Receive();
            }

            lock (this.queue)
            {
                if (this.queue.Count > 0)
                {
                    return this.queue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Reads a message from the VPW bus and returns it.
        /// </summary>
        public OBDMessage ReceiveLogMessage()
        {
            if (this.logQueue.Count == 0)
            {
                this.Receive();
            }

            lock (this.logQueue)
            {
                if (this.logQueue.Count > 0)
                {
                    return this.logQueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Set the device's VPW data rate.
        /// </summary>
        public bool SetVpwSpeed(VpwSpeed newSpeed)
        {
            if (this.Speed == newSpeed)
            {
                return true;
            }

            if (((newSpeed == VpwSpeed.FourX) && !this.Enable4xReadWrite) || (!this.SetVpwSpeedInternal(newSpeed)))
            {
                return false;
            }

            this.Speed = newSpeed;
            return true;
        }

        /// <summary>
        /// Set the interface to low (false) or high (true) speed
        /// </summary>
        protected abstract bool SetVpwSpeedInternal(VpwSpeed newSpeed);

        /// <summary>
        /// Clean up anything that this instance has allocated.
        /// </summary>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Add a received message to the queue.
        /// </summary>
        protected void Enqueue(OBDMessage message)
        {
            try
            {
                //Debug.WriteLine("Message: " + message.ToString() +", elmprompt: " + message.ElmPrompt);
                MsgEventparameter msg = new MsgEventparameter(message);
                OnMsgReceived(msg);
                if (message.ElmPrompt || ( message.Length > 4 && message.GetBytes()[3] == 0x6A))
                {
                    lock (this.logQueue)
                    {
                        this.logQueue.Enqueue(message);
                    }
                }
                if (message.Length > 3)
                {
                    lock (this.queue)
                    {
                        this.queue.Enqueue(message);
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, Device.Enqueue line " + line + ": " + ex.Message);
            }
        }


        /// <summary>
        /// List for an incoming message of the VPW bus.
        /// </summary>
        public abstract void Receive();

        /// <summary>
        /// Start a loop or set event receiver for listening packets
        /// </summary>
        public virtual void StartLogging(bool passive)
        {
            throw new Exception("StartLogging must be implemented in device");
        }


        /// <summary>
        /// This is no longer used, but is being kept for now since the comments
        /// shed some light on the differences between AllPro and Scantool LX 
        /// (probably not SX) interfaces.
        /// </summary>
        protected int GetVpwTimeoutMilliseconds(TimeoutScenario scenario)
        {
            int packetSize;

            switch (scenario)
            {
                case TimeoutScenario.ReadProperty:
                    // Approximate number of bytes in a get-VIN or get-OSID response.
                    packetSize = 50;
                    break;

                case TimeoutScenario.ReadCrc:
                    // These packets are actually only 15 bytes, but the ReadProperty timeout wasn't
                    // enough for the AllPro at 4x. Still not sure why. So this is a bit of a hack.
                    // TODO: Figure out why the AllPro needs a hack to receive CRC values at 4x.
                    packetSize = 1000;
                    break;

                case TimeoutScenario.ReadMemoryBlock:
                    // Adding 20 bytes to account for the 'read request accepted' 
                    // message that comes before the read payload.
                    packetSize = 20 + this.MaxReceiveSize;

                    // Not sure why this is necessary, but AllPro 2k reads won't work without it.
                    //packetSize = (int) (packetSize * 1.1);
                    packetSize = (int) (packetSize * 2.5);
                    break;

                case TimeoutScenario.SendKernel:
                    packetSize = this.MaxKernelSendSize + 20;
                    break;

                // Not tuned manually yet.
                case TimeoutScenario.DataLogging1:
                    packetSize = 30;
                    break;

                // This one was tuned by hand to avoid timeouts with STPX, and it work well for the AllPro too.
                case TimeoutScenario.DataLogging2:
                    packetSize = 47;
                    break;

                // 64 works for the LX, but the AllPro needs 70.
                case TimeoutScenario.DataLogging3:
                    packetSize = 70;
                    break;

                case TimeoutScenario.DataLogging4:
                    packetSize = 4500;
                    break;

                case TimeoutScenario.Maximum:
                    return 0xFF * 4; 

                default:
                    throw new NotImplementedException("Unknown timeout scenario " + scenario);
            }

            int bitsPerByte = 9; // 8N1 serial
            double bitsPerSecond = this.Speed == VpwSpeed.Standard ? 10.4 : 41.6;
            double milliseconds = (packetSize * bitsPerByte) / bitsPerSecond;

            // Add 10% just in case.
            return (int)(milliseconds * 1.1);
        }

        /// <summary>
        /// Estimate timeouts. The code above seems to do a pretty good job, but this is easier to experiment with.
        /// </summary>
        protected int __GetVpwTimeoutMilliseconds(TimeoutScenario scenario)
        {
            switch (scenario)
            {
                case TimeoutScenario.ReadProperty:
                    return 100;

                case TimeoutScenario.ReadMemoryBlock:
                    return 2500;

                case TimeoutScenario.SendKernel:
                    return 1000;

                default:
                    throw new NotImplementedException("Unknown timeout scenario " + scenario);
            }
        }

        public abstract bool SetLoggingFilter();
        public abstract bool SetAnalyzerFilter();
        public abstract bool RemoveFilters();
        public string CurrentFilter { get; set; }
        public DataLogger.LoggingDevType LogDeviceType { get; set; }


        /// <summary>
        /// Event for updating console when message received
        /// </summary>
        public event EventHandler<MsgEventparameter> MsgReceived;

        protected virtual void OnMsgReceived(MsgEventparameter e)
        {
            MsgReceived?.Invoke(this, e);
        }

        public void MessageReceived(OBDMessage message)
        {
            MsgEventparameter msg = new MsgEventparameter(message);
            OnMsgReceived(msg);
        }

        /// <summary>
        /// Event for updating console when message sent
        /// </summary>
        public event EventHandler<MsgEventparameter> MsgSent;

        protected virtual void OnMsgSent(MsgEventparameter e)
        {
            MsgSent?.Invoke(this, e);
        }

        public void MessageSent(OBDMessage message)
        {
            MsgEventparameter msg = new MsgEventparameter(message);
            OnMsgSent(msg);
        }
    }
}
