using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using static Helpers;

namespace UniversalPatcher
{
    class JConsole
    {
        public Device JDevice;
        public IPort port;
        public bool Connected = false;
        public MessageReceiver Receiver;

    }
}
