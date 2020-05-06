using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace UniversalPatcher
{

    class RichTextBoxTraceListener : TraceListener
    {
        private RichTextBox rtBox;

        public RichTextBoxTraceListener(RichTextBox box)
        {
            this.rtBox = box;
        }

        public override void Write(string msg)
        {
            //allows tBox to be updated from different thread
            rtBox.Parent.Invoke(new MethodInvoker(delegate ()
            {
                //rtBox.Focus();
                //int Start = rtBox.Text.Length;
                rtBox.AppendText(msg);
                //rtBox.Select(Start, msg.Length);
                //Application.DoEvents();
            }));
        }

        public override void WriteLine(string msg)
        {
            Write(msg + "\r\n");
        }
    }
}
