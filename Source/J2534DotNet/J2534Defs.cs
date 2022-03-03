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
using System.Runtime.InteropServices;

namespace J2534DotNet
{
    public class PassThruMsg
    {
        public PassThruMsg() { }
        public PassThruMsg(ProtocolID myProtocolId, TxFlag myTxFlag, byte[] myByteArray)
        {
            ProtocolID = myProtocolId;
            TxFlags = myTxFlag;
            Data = myByteArray;
        }
		public ProtocolID ProtocolID {get; set;}
        public RxStatus RxStatus { get; set; }
        public TxFlag TxFlags { get; set; }
        public int Timestamp { get; set; }
        public int ExtraDataIndex { get; set; }
        public byte[] Data { get; set; }
    }

    [Flags]
    public enum RxStatus
    {
        NONE = 0x00000000,
        TX_MSG_TYPE = 0x00000001,
        START_OF_MESSAGE = 0x00000002,
        RX_BREAK = 0x00000004,
        TX_INDICATION = 0x00000008,
        ISO15765_PADDING_ERROR = 0x00000010,
        ISO15765_ADDR_TYPE = 0x00000080,
        CAN_29BIT_ID = 0x00000100
    }

    [Flags]
    public enum ConnectFlag
    {
        NONE = 0x0000,
        ISO9141_K_LINE_ONLY = 0x1000,
        CAN_ID_BOTH = 0x0800,
        ISO9141_NO_CHECKSUM = 0x0200,
        CAN_29BIT_ID = 0x0100
    }

    [Flags]
    public enum TxFlag
    {
        NONE = 0x00000000,
        SCI_TX_VOLTAGE = 0x00800000,
        SCI_MODE = 0x00400000,
        WAIT_P3_MIN_ONLY = 0x00000200,
        CAN_29BIT_ID = 0x00000100,
        ISO15765_ADDR_TYPE = 0x00000080,
        ISO15765_FRAME_PAD = 0x00000040
    }

    public enum ProtocolID
    {
        J1850VPW = 0x01,
        J1850PWM = 0x02,
        ISO9141 = 0x03,
        ISO14230 = 0x04,
        CAN = 0x05,
        ISO15765 = 0x06,
        SCI_A_ENGINE = 0x07,
        SCI_A_TRANS = 0x08,
        SCI_B_ENGINE = 0x09,
        SCI_B_TRANS = 0x0A,
        //J2534-2 Protocol definitions
        J1850VPW_PS = 0x00008000,
        J1850PWM_PS = 0x00008001,
        ISO9141_PS = 0x00008002,
        ISO14230_PS = 0x00008003,
        CAN_PS = 0x00008004,
        ISO15765_PS = 0x00008005,
        J2610_PS = 0x00008006,
        SW_ISO15765_PS = 0x00008007,
        SW_CAN_PS = 0x00008008,
        GM_UART_PS = 0x00008009,
        UART_ECHO_BYTE_PS = 0x0000800A,
        HONDA_DIAGH_PS = 0x0000800B,
        J1939_PS = 0x0000800C,
        J1708_PS = 0x0000800D,
        TP2_0_PS = 0x0000800E,
        FT_CAN_PS = 0x0000800F,
        FT_ISO15765_PS = 0x00008010,
        FD_CAN_PS = 0x00008011,
        FD_ISO15765_PS = 0x00008012,
        CAN_CH1 = 0x00009000,
        J1850VPW_CH1 = 0x00009080,
        J1850PWM_CH1 = 0x00009100,
        ISO9141_CH1 = 0x00009180,
        ISO14230_CH1 = 0x00009200,
        ISO15765_CH1 = 0x00009280,
        SW_CAN_CAN_CH1 = 0x00009300,
        SW_CAN_ISO15765_CH1 = 0x00009380,
        J2610_CH1 = 0x00009400,
        FT_CAN_CH1 = 0x00009480,
        FT_ISO15765_CH1 = 0x00009500,
        GM_UART_CH1 = 0x00009580,
        ECHO_BYTE_CH1 = 0x00009600,
        HONDA_DIAGH_CH1 = 0x00009680,
        J1939_CH1 = 0x00009700,
        J1708_CH1 = 0x00009780,
        TP2_0_CH1 = 0x00009800,
        FD_CAN_CH1 = 0x00009880,
        FD_ISO15765_CH1 = 0x00009900,
        ANALOG_IN_1 = 0x0000C000,


    }

    public enum BaudRate
    {
        ISO9141 = 10400,
        ISO9141_10400 = 10400,
        ISO9141_10000 = 10000,

        ISO14230 = 10400,
        ISO14230_10400 = 10400,
        ISO14230_10000 = 10000,

        J1850PWM = 41600,
        J1850PWM_41600 = 41600,
        J1850PWM_83300 = 83300,

        J1850VPW = 10400,
        J1850VPW_10400 = 10400,
        J1850VPW_41600 = 41600,

        CAN = 500000,
        CAN_125000 = 125000,
        CAN_250000 = 250000,
        CAN_500000 = 500000,

        ISO15765 = 500000,
        ISO15765_125000 = 125000,
        ISO15765_250000 = 250000,
        ISO15765_500000 = 500000
    }

    public enum PinNumber
    {
        AUX = 0,
        PIN_6 = 6,
        PIN_9 = 9,
        PIN_11 = 11,
        PIN_12 = 12,
        PIN_13 = 13,
        PIN_14 = 14,
        PIN_15 = 15
    }

    public enum FilterType
    {
        PASS_FILTER = 0x01,
        BLOCK_FILTER = 0x02,
        FLOW_CONTROL_FILTER = 0x03
    }

    enum Ioctl
    {
        GET_CONFIG = 0x01,
        SET_CONFIG = 0x02,
        READ_VBATT = 0x03,
        FIVE_BAUD_INIT = 0x04,
        FAST_INIT = 0x05,
        CLEAR_TX_BUFFER = 0x07,
        CLEAR_RX_BUFFER = 0x08,
        CLEAR_PERIODIC_MSGS = 0x09,
        CLEAR_MSG_FILTERS = 0x0A,
        CLEAR_FUNCT_MSG_LOOKUP_TABLE = 0x0B,
        ADD_TO_FUNCT_MSG_LOOKUP_TABLE = 0x0C,
        DELETE_FROM_FUNCT_MSG_LOOKUP_TABLE = 0x0D,
        READ_PROG_VOLTAGE = 0x0E
    }

    public enum ConfigParameter
    {
        NONE = 0,
        DATA_RATE = 0x01,
        LOOP_BACK = 0x03,
        NODE_ADDRESS = 0x04,
        NETWORK_LINE = 0x05,
        P1_MIN = 0x06,
        P1_MAX = 0x07,
        P2_MIN = 0x08,
        P2_MAX = 0x09,
        P3_MIN = 0x0A,
        P3_MAX = 0x0B,
        P4_MIN = 0x0C,
        P4_MAX = 0x0D,
        W0 = 0x19,
        W1 = 0x0E,
        W2 = 0x0F,
        W3 = 0x10,
        W4 = 0x11,
        W5 = 0x12,
        TIDLE = 0x13,
        TINIL = 0x14,
        TWUP = 0x15,
        PARITY = 0x16,
        BIT_SAMPLE_POINT = 0x17,
        SYNC_JUMP_WIDTH = 0x18,
        T1_MAX = 0x1A,
        T2_MAX = 0x1B,
        T3_MAX = 0x24,
        T4_MAX = 0x1C,
        T5_MAX = 0x1D,
        ISO15765_BS = 0x1E,
        ISO15765_STMIN = 0x1F,
        DATA_BITS = 0x20,
        FIVE_BAUD_MOD = 0x21,
        BS_TX = 0x22,
        STMIN_TX = 0x23,
        ISO15765_WFT_MAX = 0x25,

        //J2534-2
        CAN_MIXED_FORMAT = 0x00008000,
        J1962_PINS = 0x00008001,
        SW_CAN_HS_DATA_RATE = 0x00008010,
        SW_CAN_SPEEDCHANGE_ENABLE = 0x00008011,
        SW_CAN_RES_SWITCH = 0x00008012,
        ACTIVE_CHANNELS = 0x00008020,
        SAMPLE_RATE = 0x00008021,
        SAMPLES_PER_READING = 0x00008022,
        READINGS_PER_MSG = 0x00008023,
        AVERAGING_METHOD = 0x00008024,
        SAMPLE_RESOLUTION = 0x00008025,
        INPUT_RANGE_LOW = 0x00008026,
        INPUT_RANGE_HIGH = 0x00008027,
        UEB_T0_MIN = 0x00008028,
        UEB_T1_MAX = 0x00008029,
        UEB_T2_MAX = 0x0000802A,
        UEB_T3_MAX = 0x0000802B,
        UEB_T4_MIN = 0x0000802C,
        UEB_T5_MAX = 0x0000802D,
        UEB_T6_MAX = 0x0000802E,
        UEB_T7_MIN = 0x0000802F,
        UEB_T7_MAX = 0x00008030,
        UEB_T9_MIN = 0x00008031,
        J1939_PINS = 0x0000803D,
        J1708_PINS = 0x0000803E,
        J1939_T1 = 0x0000803F,
        J1939_T2 = 0x00008040,
        J1939_T3 = 0x00008041,
        J1939_T4 = 0x00008042,
        J1939_BRDCST_MIN_DELAY = 0x00008043,
        TP2_0_T_BR_INT = 0x00008044,
        TP2_0_T_E = 0x00008045,
        TP2_0_MNTC = 0x00008046,
        TP2_0_T_CTA = 0x00008047,
        TP2_0_MNCT = 0x00008048,
        TP2_0_MNTB = 0x00008049,
        TP2_0_MNT = 0x0000804A,
        TP2_0_T_WAIT = 0x0000804B,
        TP2_0_T1 = 0x0000804C,
        TP2_0_T3 = 0x0000804D,
        TP2_0_IDENTIFER = 0x0000804E,
        TP2_0_RXIDPASSIVE = 0x0000804F,
        FD_CAN_DATA_PHASE_RATE = 0x0000805C,
        FD_ISO15765_TX_DATA_LENGTH = 0x0000805D,
        HS_CAN_TERMINATION = 0x0000805E,
        N_CR_MAX = 0x0000805F,
        ISO15765_PAD_VALUE = 0x00008060,

        ISO15765_SIMULTANEOUS = 0x10000000, //Drewtech
        DT_ISO15765_PAD_BYTE = 0x10000001,  //Drewtech
        ADC_READINGS_PER_SECOND = 0x10000,  //Drewtech
        ADC_READINGS_PER_SAMPLE = 0x20000   //Drewtech

    }

    public enum J2534Err
    {
        STATUS_NOERROR = 0x00,
        ERR_NOT_SUPPORTED = 0x01,
        ERR_INVALID_CHANNEL_ID = 0x02,
        ERR_INVALID_PROTOCOL_ID = 0x03,
        ERR_NULL_PARAMETER = 0x04,
        ERR_INVALID_FLAGS = 0x06,
        ERR_FAILED = 0x07,
        ERR_DEVICE_NOT_CONNECTED = 0x08,
        ERR_TIMEOUT = 0x09,
        ERR_INVALID_MSG = 0x0A,
        ERR_INVALID_TIME_INTERVAL = 0x0B,
        ERR_EXCEEDED_LIMIT = 0x0C,
        ERR_INVALID_MSG_ID = 0x0D,
        ERR_DEVICE_IN_USE = 0x0E,
        ERR_INVALID_IOCTL_ID = 0x0F,
        ERR_BUFFER_EMPTY = 0x10,
        ERR_BUFFER_FULL = 0x11,
        ERR_BUFFER_OVERFLOW = 0x12,
        ERR_PIN_INVALID = 0x13,
        ERR_CHANNEL_IN_USE = 0x14,
        ERR_MSG_PROTOCOL_ID = 0x15,
        ERR_INVALID_FILTER_ID = 0x16,
        ERR_NO_FLOW_CONTROL = 0x17,
        ERR_NOT_UNIQUE = 0x18,
        ERR_INVALID_BAUDRATE = 0x19,
        ERR_INVALID_DEVICE_ID = 0x1A
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SConfig
    {
        [FieldOffset(0), MarshalAs(UnmanagedType.U4)]
        public ConfigParameter Parameter;
        [FieldOffset(4), MarshalAs(UnmanagedType.U4)]
        public int Value;

        public SConfig(ConfigParameter Parameter, int Value)
        {
            this.Parameter = Parameter;
            this.Value = Value;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SconfigList
    {
        public int NumOfParams;
        public IntPtr ConfigPtr;
    }
}
