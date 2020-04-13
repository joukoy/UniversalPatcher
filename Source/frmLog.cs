using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmLog : Form
    {
        public frmLog()
        {
            InitializeComponent();
        }

        public void StartDebug()
        {
            RichTextBoxTraceListener tbtl = new RichTextBoxTraceListener(rtxtLog);
            Debug.Listeners.Add(tbtl);
            this.Text = "Debug";
        }
        private void btnSaveFileInfo_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("Text files (*.txt)|*.txt|All files (*.*)|*.*");
                if (FileName.Length > 1)
                {
                    StreamWriter sw = new StreamWriter(FileName);
                    sw.WriteLine(rtxtLog.Text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }

        }

        public void ShowLog(string msg)
        {
            rtxtLog.Focus();
            rtxtLog.AppendText(msg);
            rtxtLog.Select(rtxtLog.Text.Length - msg.Length + 1, msg.Length);
            rtxtLog.SelectionFont = new Font(rtxtLog.Font, FontStyle.Regular);
            rtxtLog.Select(rtxtLog.Text.Length, 0);
            Application.DoEvents();
            LogText = "";
        }

        public void ShowLogBold(string msg)
        {
            rtxtLog.Focus();
            rtxtLog.AppendText(msg);
            rtxtLog.Select(rtxtLog.Text.Length - msg.Length + 1, msg.Length);
            rtxtLog.SelectionFont = new Font(rtxtLog.Font, FontStyle.Bold);
            rtxtLog.Select(rtxtLog.Text.Length, 0);
            Application.DoEvents();
            LogText = "";

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (LogText.Length > 0)
            {
                rtxtLog.Focus();
                rtxtLog.AppendText(LogText);
                int Start = rtxtLog.Text.Length - 1;
                rtxtLog.Select(rtxtLog.Text.Length - 1, 1);
                rtxtLog.SelectionFont = new Font(rtxtLog.Font, FontStyle.Regular);
                Application.DoEvents();
                LogText = "";
            }
        }
    }
}
