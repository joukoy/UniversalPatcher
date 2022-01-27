using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using PcmHacking;

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

                if (args.Length > 0)
                {
                    if (args[0] == "-") ;
                        //Remember previous
                    else if (args[0].ToLower().Contains("tourist"))
                        Properties.Settings.Default.WorkingMode = 0;
                    else if (args[0].ToLower().Contains("basic"))
                        Properties.Settings.Default.WorkingMode = 1;
                    else if (args[0].ToLower().Contains("advanced"))
                        Properties.Settings.Default.WorkingMode = 2;
                    else
                    {
                        throw new Exception("Usage: " + Path.GetFileName(Application.ExecutablePath) + " [tourist | basic | advanced] [launcher | tuner]");
                    }
                    Properties.Settings.Default.Save();
                }

                Upatcher.StartupSettings();


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
                    else if (args[1].ToLower().Contains("logger"))
                    {
                        Application.Run(new MainForm());
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
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(e.Message, "Error");
        }
    }
}
