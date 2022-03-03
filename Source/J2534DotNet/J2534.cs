#region Copyright (c) 2010, Michael Kelly
/* 
 * Copyright (c) 2010, Michael Kelly
 * michael.e.kelly@gmail.com
 * http://michael-kelly.com/
 * 
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the organization nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
#endregion License
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace J2534DotNet
{
    public class J2534 : IJ2534
    {
        private J2534Device m_device;
        private J2534DllWrapper m_wrapper;
        private readonly object devLock = new object();

        public bool LoadLibrary(J2534Device device)
        {
            m_device = device;
            m_wrapper = new J2534DllWrapper();
            return m_wrapper.LoadJ2534Library(m_device.FunctionLibrary);
        }

        public bool FreeLibrary()
        {
            try 
            {
                lock (devLock)
                {
                    return m_wrapper.FreeLibrary();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

        }

        public bool isDllLoaded()
        {
            if (m_wrapper == null)
                return false;
            return m_wrapper.isDllLoaded();
        }

        public J2534Err Open(ref int deviceId)
        {
            try
            { 
                IntPtr DeviceNamePtr = IntPtr.Zero;
                IntPtr DeviceIDPtr = Marshal.AllocHGlobal(4);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.Open(DeviceNamePtr, DeviceIDPtr);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    deviceId = Marshal.ReadInt32(DeviceIDPtr);
                }
                Marshal.FreeHGlobal(DeviceIDPtr); 
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err Close(int deviceId)
        {
            try
            {
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Close(deviceId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err Connect(int deviceId, ProtocolID protocolId, ConnectFlag flags, BaudRate baudRate, ref int channelId)
        {
            try
            {
                IntPtr ChannelPtr = Marshal.AllocHGlobal(4);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.Connect(deviceId, (int)protocolId, (int)flags, (int)baudRate, ChannelPtr);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    channelId = Marshal.ReadInt32(ChannelPtr);
                }
                Marshal.FreeHGlobal(ChannelPtr);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err Connect(int deviceId, ProtocolID protocolId, ConnectFlag flags, int baudRate, ref int channelId)
        {
            try
            {
                IntPtr ChannelPtr = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(ChannelPtr, channelId);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.Connect(deviceId, (int)protocolId, (int)flags, baudRate, ChannelPtr);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    channelId = Marshal.ReadInt32(ChannelPtr);
                }
                Marshal.FreeHGlobal(ChannelPtr);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err Disconnect(int channelId)
        {
            try
            {
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Disconnect(channelId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err ReadMsgs(int channelId, ref List<PassThruMsg> msgs, ref int numMsgs, int timeout)
        {
            try
            {
                IntPtr pMsg = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UnsafePassThruMsg))*numMsgs + 4 );
                IntPtr pNextMsg = IntPtr.Zero;
                //IntPtr[] pMsgs = new IntPtr[50];
                IntPtr MsgCount = Marshal.AllocHGlobal(8);
                Marshal.WriteInt64(MsgCount, numMsgs);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.ReadMsgs(channelId, pMsg, MsgCount, timeout);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    //Debug.WriteLine("J2534 received messages: " + numMsgs);
                    pNextMsg = pMsg;
                    numMsgs = (int)Marshal.ReadInt64(MsgCount);
                    for (int i = 0; i < numMsgs; i++)
                    {
                        //pNextMsg = (IntPtr)(Marshal.SizeOf(typeof(UnsafePassThruMsg)) * i + (int)pMsg);
                        UnsafePassThruMsg uMsg = (UnsafePassThruMsg)Marshal.PtrToStructure(pNextMsg, typeof(UnsafePassThruMsg));
                        msgs.Add(ConvertPassThruMsg(uMsg));
                        pNextMsg += Marshal.SizeOf(typeof(UnsafePassThruMsg));
                    }
                }
                Marshal.FreeHGlobal(pMsg);
                Marshal.FreeHGlobal(MsgCount);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err WriteMsgs(int channelId, PassThruMsg msg, ref int numMsgs, int timeout)
        {
            try
            {
                UnsafePassThruMsg uMsg = ConvertPassThruMsg(msg);
                // TODO: change function to accept a list? of PassThruMsg
                IntPtr MsgPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uMsg));
                Marshal.StructureToPtr(uMsg, MsgPtr, false);
                IntPtr MsgCount = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(MsgCount, 1);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.WriteMsgs(channelId, MsgPtr, MsgCount, timeout);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    numMsgs = Marshal.ReadInt32(MsgCount);
                }
                Marshal.FreeHGlobal(MsgPtr);
                Marshal.FreeHGlobal(MsgCount);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err StartPeriodicMsg(int channelId, PassThruMsg msg, ref int msgId, int timeInterval)
        {
            UnsafePassThruMsg uMsg = ConvertPassThruMsg(msg);
            IntPtr MsgPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uMsg));
            Marshal.StructureToPtr(uMsg, MsgPtr, false);
            IntPtr msgIdPtr = Marshal.AllocHGlobal(4);
            J2534Err m = (J2534Err)m_wrapper.StartPeriodicMsg(channelId, MsgPtr, msgIdPtr, timeInterval);
            if (m == J2534Err.STATUS_NOERROR)
            {
                msgId = Marshal.ReadInt32(msgIdPtr);
            }
            return m;
        }

        public J2534Err StopPeriodicMsg(int channelId, int msgId)
        {
            return (J2534Err)m_wrapper.StopPeriodicMsg(channelId, msgId);
        }

        public J2534Err StartMsgFilter
        (
            int channelid,
            FilterType filterType,
            PassThruMsg maskMsg,
            PassThruMsg patternMsg,
            PassThruMsg flowControlMsg,
            ref int filterId
        )
        {
            try
            {
                UnsafePassThruMsg uMaskMsg = ConvertPassThruMsg(maskMsg);
                UnsafePassThruMsg uPatternMsg = ConvertPassThruMsg(patternMsg);
                UnsafePassThruMsg uFlowControlMsg = ConvertPassThruMsg(flowControlMsg);

                IntPtr uMaskPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uMaskMsg));
                Marshal.StructureToPtr(uMaskMsg, uMaskPtr, false);

                IntPtr uPatternPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uPatternMsg));
                Marshal.StructureToPtr(uPatternMsg, uPatternPtr, false);

                IntPtr uFlowControlPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uFlowControlMsg));
                Marshal.StructureToPtr(flowControlMsg, uFlowControlPtr, false);

                IntPtr filterIdPtr = Marshal.AllocHGlobal(4);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.StartMsgFilter
                        (
                            channelid,
                            (int)filterType,
                            uMaskPtr,
                            uPatternPtr,
                            uFlowControlPtr,
                            filterIdPtr
                        );
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    filterId = Marshal.ReadInt32(filterIdPtr);
                }
                Marshal.FreeHGlobal(uMaskPtr);
                Marshal.FreeHGlobal(uPatternPtr);
                Marshal.FreeHGlobal(uFlowControlPtr);
                Marshal.FreeHGlobal(filterIdPtr);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err StartMsgFilter
        (
            int channelid,
            FilterType filterType,
            PassThruMsg maskMsg,
            PassThruMsg patternMsg,
            ref int filterId
        )
        {
            try
            {
                IntPtr FlowMsg = IntPtr.Zero;
                UnsafePassThruMsg uMaskMsg = ConvertPassThruMsg(maskMsg);
                UnsafePassThruMsg uPatternMsg = ConvertPassThruMsg(patternMsg);

                IntPtr uMaskPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uMaskMsg));
                Marshal.StructureToPtr(uMaskMsg, uMaskPtr, false);

                IntPtr uPatternPtr = Marshal.AllocHGlobal(Marshal.SizeOf(uPatternMsg));
                Marshal.StructureToPtr(uPatternMsg, uPatternPtr, false);

                IntPtr filterIdPtr = Marshal.AllocHGlobal(4);

                J2534Err returnValue;
                lock (devLock)
                {

                    returnValue = (J2534Err)m_wrapper.StartPassBlockMsgFilter
                        (
                            channelid,
                            (int)filterType,
                            uMaskPtr,
                            uPatternPtr,
                            FlowMsg,
                            filterIdPtr
                        );
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    filterId = Marshal.ReadInt32(filterIdPtr);
                }
                Marshal.FreeHGlobal(uMaskPtr);
                Marshal.FreeHGlobal(uPatternPtr);
                Marshal.FreeHGlobal(filterIdPtr);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err StopMsgFilter(int channelId, int filterId)
        {
            try
            {
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.StopMsgFilter(channelId, filterId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err SetProgrammingVoltage(int deviceId, PinNumber pinNumber, int voltage)
        {
            lock (devLock)
            {
                return (J2534Err)m_wrapper.SetProgrammingVoltage(deviceId, (int)pinNumber, voltage);
            }
        }

        public J2534Err ReadVersion(int deviceId, ref string firmwareVersion, ref string dllVersion, ref string apiVersion)
        {
            try
            {
                IntPtr pFirmwareVersion = Marshal.AllocHGlobal(120);
                IntPtr pDllVersion = Marshal.AllocHGlobal(120);
                IntPtr pApiVersion = Marshal.AllocHGlobal(120);
                J2534Err returnValue;
                lock (devLock)
                {

                    returnValue = (J2534Err)m_wrapper.ReadVersion(deviceId, pFirmwareVersion, pDllVersion, pApiVersion);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    firmwareVersion = Marshal.PtrToStringAnsi(pFirmwareVersion);
                    dllVersion = Marshal.PtrToStringAnsi(pDllVersion);
                    apiVersion = Marshal.PtrToStringAnsi(pApiVersion);
                }

                Marshal.FreeHGlobal(pFirmwareVersion);
                Marshal.FreeHGlobal(pDllVersion);
                Marshal.FreeHGlobal(pApiVersion);

                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err GetLastError(ref string errorDescription)
        {
            try
            {
                IntPtr pErrorDescription = Marshal.AllocHGlobal(120);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.GetLastError(pErrorDescription);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    errorDescription = Marshal.PtrToStringAnsi(pErrorDescription);
                }

                Marshal.FreeHGlobal(pErrorDescription);

                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err GetConfig(int channelId, ref List<SConfig> config)
        {
            try
            {
                IntPtr Ptr = SconfigToPtr(config.ToArray()); J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.GET_CONFIG, Ptr, IntPtr.Zero);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    IntPtr pNextConf = IntPtr.Add(Ptr,8); //Numofelements in beginning, then Ptr to SConfig structure
                    config.Clear();
                    for (int s = 0; s < config.Count; s++)
                    {
                        SConfig sc = (SConfig)Marshal.PtrToStructure(Ptr, typeof(SConfig));
                        config.Add(sc);
                        pNextConf = IntPtr.Add(pNextConf, Marshal.SizeOf(typeof(SConfig)));
                    }
                }
                Marshal.FreeHGlobal(Ptr);
                return J2534Err.STATUS_NOERROR;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        //
        //Sconfig array:
        //NumOfParams                   [4 bytes]
        //Pointer to Sconfig structure  [4 bytes]
        //Sconfig structure:
        //Parameter                     [4 bytes]
        //Value                         [4 bytes]
        IntPtr SconfigToPtr(SConfig[] config)
        {
            int elementSize = Marshal.SizeOf(config[0]);
            //Create a blob big enough for all elements and two longs (NumOfItems and pItems)
            IntPtr Ptr = Marshal.AllocHGlobal(config.Length * elementSize + 8);
            //Set array length: (First value in structure)
            Marshal.WriteInt32(Ptr, config.Length);
            IntPtr firstElementPtr = IntPtr.Add(Ptr, 8);
            //Write pItems.  To save complexity, the array immediately follows SConfigArray.
            Marshal.WriteIntPtr(Ptr, 4, firstElementPtr);
            for (int i = 0; i < config.Length; i++)
                Marshal.StructureToPtr(config[i], IntPtr.Add(firstElementPtr, i * elementSize), false);
            return Ptr;
        }

        public J2534Err SetConfig(int channelId, ref SConfig[] config)
        {
            IntPtr Ptr = SconfigToPtr(config);
            J2534Err returnValue;
            lock (devLock)
            {
                returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.SET_CONFIG, Ptr, IntPtr.Zero);
            }
            Marshal.FreeHGlobal(Ptr);
            return returnValue;
        }

        public J2534Err ReadBatteryVoltage(int DeviceID, ref int voltage)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = Marshal.AllocHGlobal(8);

                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.Ioctl(DeviceID, (int)Ioctl.READ_VBATT, input, output);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    voltage = Marshal.ReadInt32(output);
                }
                Marshal.FreeHGlobal(output);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err FiveBaudInit(int channelId, byte targetAddress, ref byte keyword1, ref byte keyword2)
        {
            try
            {
                J2534Err returnValue;
                IntPtr input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SByteArray)) + 4);
                IntPtr output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SByteArray)) + 4);

                SByteArray inputArray = new SByteArray();
                SByteArray outputArray = new SByteArray();
                inputArray.NumOfBytes = 1;
                
                IntPtr inputPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SByteArray)) + 8);
                Marshal.WriteIntPtr(inputPtr, 4, IntPtr.Add(inputPtr, 8));
                byte[] targetArray = new byte[] { targetAddress };
                Marshal.Copy(targetArray, 0, IntPtr.Add(inputPtr, 8), 1);
                //unsafe
                {
                    //inputArray.BytePtr[0] = targetAddress;
                    outputArray.NumOfBytes = 2;

                    Marshal.StructureToPtr(inputArray, input, true);
                    Marshal.StructureToPtr(outputArray, output, true);
                    lock (devLock)
                    {
                        returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.FIVE_BAUD_INIT, inputPtr, output);
                    }
                    outputArray = (SByteArray)Marshal.PtrToStructure(output, typeof(SByteArray));
                }
                Marshal.FreeHGlobal(inputPtr);
                Marshal.FreeHGlobal(input);
                Marshal.FreeHGlobal(output);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }
        }

        public J2534Err FastInit(int channelId, PassThruMsg txMsg, ref PassThruMsg rxMsg)
        {
            try
            {
                IntPtr input = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UnsafePassThruMsg)));
                IntPtr output = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(UnsafePassThruMsg)));
                UnsafePassThruMsg uTxMsg = ConvertPassThruMsg(txMsg);
                UnsafePassThruMsg uRxMsg = new UnsafePassThruMsg();

                Marshal.StructureToPtr(uTxMsg, input, false);
                Marshal.StructureToPtr(uRxMsg, output, false);
                J2534Err returnValue;
                lock (devLock)
                {
                    returnValue = (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.FAST_INIT, input, output);
                }
                if (returnValue == J2534Err.STATUS_NOERROR)
                {
                    uRxMsg = (UnsafePassThruMsg)Marshal.PtrToStructure(output, typeof(UnsafePassThruMsg));
                }
                rxMsg = ConvertPassThruMsg(uRxMsg);
                Marshal.FreeHGlobal(input);
                Marshal.FreeHGlobal(output);
                return returnValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err ClearTxBuffer(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_TX_BUFFER, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err ClearRxBuffer(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_RX_BUFFER, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err ClearPeriodicMsgs(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_PERIODIC_MSGS, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err ClearMsgFilters(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_MSG_FILTERS, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err ClearFunctMsgLookupTable(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.CLEAR_FUNCT_MSG_LOOKUP_TABLE, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err AddToFunctMsgLookupTable(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                // TODO: fix this
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.ADD_TO_FUNCT_MSG_LOOKUP_TABLE, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        public J2534Err DeleteFromFunctMsgLookupTable(int channelId)
        {
            try
            {
                IntPtr input = IntPtr.Zero;
                IntPtr output = IntPtr.Zero;
                // TODO: fix this
                lock (devLock)
                {
                    return (J2534Err)m_wrapper.Ioctl(channelId, (int)Ioctl.DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE, input, output);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return J2534Err.ERR_FAILED;
            }

        }

        private UnsafePassThruMsg ConvertPassThruMsg(PassThruMsg msg)
        {
            UnsafePassThruMsg uMsg = new UnsafePassThruMsg();
            try
            {

                uMsg.ProtocolID = (int)msg.ProtocolID;
                uMsg.RxStatus = (int)msg.RxStatus;
                uMsg.Timestamp = msg.Timestamp;
                uMsg.TxFlags = (int)msg.TxFlags;
                uMsg.ExtraDataIndex = msg.ExtraDataIndex;
                uMsg.DataSize = msg.Data.Length;
                unsafe
                {
                    for (int i = 0; i < msg.Data.Length; i++)
                    {
                        uMsg.Data[i] = msg.Data[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return uMsg;
        }

        private static PassThruMsg ConvertPassThruMsg(UnsafePassThruMsg uMsg)
        {
            PassThruMsg msg = new PassThruMsg();
            try
            {
                msg.ProtocolID = (ProtocolID)uMsg.ProtocolID;
                msg.RxStatus = (RxStatus)uMsg.RxStatus;
                msg.Timestamp = uMsg.Timestamp;
                msg.TxFlags = (TxFlag)uMsg.TxFlags;
                msg.ExtraDataIndex = uMsg.ExtraDataIndex;
                msg.Data = new byte[uMsg.DataSize];
                unsafe
                {
                    for (int i = 0; i < uMsg.DataSize; i++)
                    {
                        msg.Data[i] = uMsg.Data[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return msg;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct SByteArray
        {
            public int NumOfBytes;
            public fixed byte BytePtr[2];
        }
    }
}
