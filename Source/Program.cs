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
                upatcher.StartupSettings();
                switch (Properties.Settings.Default.StartupForm)
                {
                    case 0:
                        Application.Run(new FrmMain());
                        break;
                    case 1:
                        PcmFile pcm = new PcmFile();
                        Application.Run(new frmTuner(pcm));
                        break;
                    case 2:
                        Application.Run(new FrmPatcher());
                        break;
                    default:
                        Application.Run(new FrmMain());
                        break;

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
