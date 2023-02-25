using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    class JConsole
    {
        public Device JDevice;
        public IPort port;
        public bool Connected = false;
        public bool Connected2 = false;
        public MessageReceiver Receiver;
        public MessageReceiver Receiver2;

        public bool SetCANBusQuiet()
        {
            try
            {
                Debug.WriteLine("Set bus quiet");
                byte[] quietMsg = { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x01, 0x28, 0x00, 0x00, 0x00, 0x00, 0x00 };
                bool m = JDevice.SendMessage(new OBDMessage(quietMsg), 10);
                if (m)
                {
                    //Debug.WriteLine("OK");
                }
                else
                {
                    Debug.WriteLine("Unable to set bus quiet");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(quietMsg, b => b.ToString("X2"))));
                    return false;
                }
                //Thread.Sleep(10);
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold("SetBusQuiet: " + ex.Message);
                return false;
            }
        }

        public bool KeepCANBusQuiet()
        {
            try
            {
                Debug.WriteLine("Keep bus quiet");
                byte[] quietMsg = { 0x00, 0x00, 0x01, 0x01, 0xFE, 0x01, 0x3E, 0x00, 0x00, 0x00, 0x00, 0x00 };
                bool m = JDevice.SendMessage(new OBDMessage(quietMsg), 10);
                if (m)
                {
                    //Debug.WriteLine("OK");
                }
                else
                {
                    Debug.WriteLine("Unable to keep bus quiet");
                    Debug.WriteLine("Expected " + string.Join(" ", Array.ConvertAll(quietMsg, b => b.ToString("X2"))));
                    return false;
                }
                //Thread.Sleep(10);
                return true;
            }
            catch (Exception ex)
            {
                LoggerBold("SetBusQuiet: " + ex.Message);
                return false;
            }
        }

    }
}
