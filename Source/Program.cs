using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace UniversalPatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                if (!Directory.Exists(Path.Combine(Application.StartupPath, "XML")))
                {
                    MessageBox.Show("Incomplete installation, Universalpatcher files missing" + Environment.NewLine + 
                        "Please extract all files from ZIP package", "Incomplete installation",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Upatcher.Startup(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            Environment.Exit(0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string errorMsg = "An application error occurred. Please contact joukoy@gmail.com " +
                "with the following information:\n\n";

            // Since we can't prevent the app from terminating, log this to the event log.
            if (!EventLog.SourceExists("ThreadException"))
            {
                EventLog.CreateEventSource("ThreadException", "Application");
            }

            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            myLog.Source = "ThreadException";
            myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(e.Message, "Error");
        }
    }
}
