using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

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
                /*
                if (Properties.Settings.Default.startPatcher)
                    Application.Run(new FrmPatcher());
                else
                    Application.Run(new FrmMain());
                */
                /*
                if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "Tuner")
                {
                    Properties.Settings.Default.WorkingMode = 2;//Advanced
                    Properties.Settings.Default.Save();
                    PcmFile pcm = new PcmFile();
                    Application.Run(new FrmTuner(pcm));
                }
                else if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "Patcher")
                {
                    Properties.Settings.Default.WorkingMode = 2;//Advanced
                    Properties.Settings.Default.Save();
                    Application.Run(new FrmPatcher());
                }
                else
                {
                    Application.Run(new FrmMain());
                }
                */
                string[] progArgs = Environment.GetCommandLineArgs();
                if (progArgs.Length > 1)
                {
                    if (progArgs[1].ToLower().Contains("tourist"))
                        Properties.Settings.Default.WorkingMode = 0;
                    else if (progArgs[1].ToLower().Contains("basic"))
                        Properties.Settings.Default.WorkingMode = 1;
                    else if (progArgs[1].ToLower().Contains("advanced"))
                        Properties.Settings.Default.WorkingMode = 2;
                    else
                    {
                        throw new Exception("Usage: " + Path.GetFileName(progArgs[0]) + " [tourist | basic | advanced]");
                    }
                    Properties.Settings.Default.Save();
                }

                upatcher.StartupSettings();

                Application.Run(new FrmPatcher());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(e.Message, "Error");
        }
    }
}
