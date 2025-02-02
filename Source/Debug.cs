﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace UniversalPatcher
{
    class FileTraceListener : TraceListener
    {
        private StreamWriter writer;
        public FileTraceListener(string FileName)
        {
            writer = new  StreamWriter(FileName);
        }
        ~FileTraceListener()
        {
            writer.Close();
        }
        public override void Write(string msg)
        {
            writer.Write(msg);
        }
        public override void WriteLine(string msg)
        {
            writer.WriteLine(msg);
        }
    }

    class RichTextBoxTraceListener : TraceListener
    {
        private RichTextBox rtBox;
        private Queue<string> dbgq = new Queue<string>();

        public RichTextBoxTraceListener(RichTextBox box)
        {
            this.rtBox = box;
        }

        public void ShowLogtext()
        {
            while (dbgq.Count > 0)
            {
                lock (dbgq)
                {
                    string msg = dbgq.Dequeue();
                    if (msg.Contains(Environment.NewLine))
                    {
                        msg = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + msg;
                    }
                    rtBox.AppendText(msg);
                }
            }
        }

        public override void Write(string msg)
        {
            lock(dbgq)
            {
                dbgq.Enqueue(msg);
            }
            //allows tBox to be updated from different thread
/*            rtBox.Parent.Invoke(new MethodInvoker(delegate ()
            {
                //rtBox.Focus();
                //int Start = rtBox.Text.Length;
                rtBox.AppendText(msg);
                //Helpers.WriteTextFile(Path.Combine(Application.StartupPath, "debug.log"),msg,true);
                //rtBox.Select(Start, msg.Length);
                //Application.DoEvents();
            }));
*/        }

        public override void WriteLine(string msg)
        {
            Write(msg + "\r\n");
        }
    }
}
