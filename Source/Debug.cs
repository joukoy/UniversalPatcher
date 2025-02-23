using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using static Helpers;

namespace UniversalPatcher
{
    class FileTraceListener : TraceListener
    {
        private StreamWriter writer;
        private string filename;
        public FileTraceListener(string FileName)
        {
            try
            {
                this.filename = FileName;
                Logger("Creating debug logfile: " + FileName);
                writer = new StreamWriter(FileName);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Debug.cs , line " + line + ": " + ex.Message);
            }
        }
        public void CloseLog()
        {
            try
            {
                Logger("Closing debug logfile: " + filename);
                writer.Close();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Debug.cs , line " + line + ": " + ex.Message);
            }
        }
        public override void Write(string msg)
        {
            try
            {
                writer.Write(msg);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Debug.cs , line " + line + ": " + ex.Message);
            }
        }
        public override void WriteLine(string msg)
        {
            try
            {
                writer.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + msg);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Debug.cs , line " + line + ": " + ex.Message);
            }
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
