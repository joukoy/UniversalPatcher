using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmJ2534Server : Form
    {
        private string DeviceName;
        private J2534Server jServer;
        public frmJ2534Server(string DevName)
        {
            DeviceName = DevName;
            InitializeComponent();
        }

        private void frmJ2534Server_Load(object sender, EventArgs e)
        {
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            this.FormClosing += FrmJ2534Server_FormClosing;
            jServer = new J2534Server(DeviceName);
            Task.Factory.StartNew(() => jServer.ServerLoop());
            this.Text = "J2534 Server [" + DeviceName + "]";
        }

        private void FrmJ2534Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            jServer.running = false;
            Logger("J2534 Server Quits");
            Application.DoEvents();
            Application.DoEvents();
            Thread.Sleep(5000);
            Environment.Exit(0);
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, txtResult);
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
