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
                Upatcher.StartupSettings(args);

                if (args.Length > 1)
                {
                    if (args[1].ToLower().Contains("launcher"))
                    {
                        Application.Run(new FrmMain());
                    }
                    else if (args[1].ToLower().Contains("tuner"))
                    {
                        PcmFile pcm = new PcmFile();
                        Application.Run(new FrmTuner(pcm));
                    }
                    else if (args[1].ToLower().Contains("patcher"))
                    {
                        Application.Run(new FrmPatcher());
                    }
                    else if (args[1].ToLower().Contains("logger"))
                    {
                        Application.Run(new frmLogger());
                    }
                    else
                    {
                        //Application.Run(new FrmPatcher());
                        PcmFile pcm = new PcmFile();
                        Application.Run(new FrmTuner(pcm));
                    }
                }
                else
                {
                    PcmFile pcm = new PcmFile();
                    Application.Run(new FrmTuner(pcm));
                    //Application.Run(new FrmPatcher());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
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
