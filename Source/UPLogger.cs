using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class UPLogger
    {
        private string logFile;
        private StreamWriter logwriter;

        ~UPLogger()
        {
            DisableLogFile();
        }
        public class UPLogString : EventArgs
        {
            public UPLogString(string logtext, bool newline, bool bold)
            {
                LogText = logtext;
                Bold = bold;
                NewLine = newline;
            }
            public string LogText { get; internal set; }
            public bool Bold { get; internal set; }
            public bool NewLine { get; internal set; }
        }

        public event EventHandler<UPLogString> UpLogUpdated;

        protected virtual void OnUpLogUpdated(UPLogString e)
        {
            UpLogUpdated?.Invoke(this, e);
        }

        public void Add(string LogText, bool NewLine, bool Bold)
        {
            try
            {
                if (NewLine)
                {
                    LogText += Environment.NewLine;
                }
                UPLogString us = new UPLogString(LogText, NewLine, Bold);
                OnUpLogUpdated(us);
                if (logwriter != null && logwriter.BaseStream.CanWrite)
                {
                    if (Bold)
                    {
                        logwriter.Write("\\b " + LogText + " \\b0");
                    }
                    else
                    {
                        logwriter.Write(LogText.Replace("\\", "\\\\"));
                    }
                    if (NewLine)
                        logwriter.Write("\\par" + Environment.NewLine);

                }
            }
            catch { }
        }

        public void DisplayText(string LogText, bool Bold, System.Windows.Forms.RichTextBox rtb)
        {
            try
            {
                rtb.Parent.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
                {
                    if (Bold)
                    {
                        rtb.SelectionFont = new System.Drawing.Font(rtb.Font, System.Drawing.FontStyle.Bold);
                    }
                    rtb.AppendText(LogText);
                    rtb.SelectionFont = new System.Drawing.Font(rtb.Font, System.Drawing.FontStyle.Regular);
                });
            }
            catch { }
        }

        public void EnableLogFile(string FileName)
        {
            try
            {
                logFile = FileName;
                if (!File.Exists(logFile))
                {
                    logwriter = new StreamWriter(logFile);
                    logwriter.WriteLine(@"{\rtf1\ansi\deff0\nouicompat{\fonttbl{\f0\fnil\fcharset0 Lucida Console;}{\f1\fnil\fcharset0 Lucida Console;}}" + Environment.NewLine);
                    logwriter.WriteLine(@"{\*\generator Riched20 10.0.18362}\viewkind4\uc1" + Environment.NewLine);
                    logwriter.WriteLine(@"\pard\sa200\sl276\slmult1\f0\fs16\lang11" + Environment.NewLine);
                }
                else
                {
                    logwriter = new StreamWriter(logFile, true);
                }
            }
            catch { }
        }

        public void DisableLogFile()
        {
            try
            {
                if (logwriter != null && logwriter.BaseStream.CanWrite)
                {
                    logwriter.WriteLine("}");
                    logwriter.Close();
                    logwriter = null;
                }
            }
            catch { }
        }


    }
}
