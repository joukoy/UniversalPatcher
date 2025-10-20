using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;
using System.Net.Sockets;
using System.Net;
using static LoggerUtils;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Management;
using System.IO.Ports;

namespace UniversalPatcher
{
    public partial class frmSerialPortServer : Form
    {
        public frmSerialPortServer()
        {
            InitializeComponent();
        }
        //private Socket mySocket;
        private TcpListener listener;
        private NetworkStream ns;
        IPort port;
        bool Connected = false;
        private void frmSerialPortServer_Load(object sender, EventArgs e)
        {
            //uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            numServerPort.Value = AppSettings.SerialServerPort;
            comboBaudRate.DataSource = SupportedBaudRates;
            comboBaudRate.Text = AppSettings.SerialServerBaudRate.ToString();
            if (AppSettings.SerialServerDevicePortType == DataLogger.iPortType.FTDI)
            {
                radioFTDI.Checked = true;
            }
            else
            {
                radioRS232.Checked = true;
            }
            LoadPorts();
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, richLogger);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Logger("Starting server, port: " + numServerPort.Value.ToString("00"));
                AppSettings.SerialServerPort = (int)numServerPort.Value;
                AppSettings.SerialServerBaudRate = Int32.Parse(comboBaudRate.Text);
                string CurrentPortName = comboSerialPort.Text;
                string sPort = comboSerialPort.Text;
                string[] sParts = sPort.Split(':');
                if (sParts.Length > 1)
                    sPort = sParts[0].Trim();

                if (radioFTDI.Checked)
                {
                    AppSettings.SerialServerDevicePortType = DataLogger.iPortType.FTDI;
                    AppSettings.SerialServerFtdiDevice = comboSerialPort.Text;
                    port = new FTDIPort(sPort);

                }
                else
                {
                    AppSettings.SerialServerDevicePortType = DataLogger.iPortType.Serial;
                    AppSettings.SerialServerDevice = comboSerialPort.Text;
                    port = new Rs232Port(sPort);
                }
                SerialPortConfiguration configuration = new SerialPortConfiguration();
                configuration.Timeout = 1000;
                configuration.DtrEnable = true;
                configuration.RtsEnable = true; //Reset device
                configuration.BaudRate = AppSettings.SerialServerBaudRate;
                port.OpenAsync(configuration);
                AppSettings.Save();
                Connected = true;
                Task.Factory.StartNew(() => TcpTask());
                Task.Factory.StartNew(() => SerialTask());
                Logger("Server started");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmSerialPortServer line " + line + ": " + ex.Message);
            }

        }

        void SerialTask()
        {
            SerialByte sb = new SerialByte(1);
            while (Connected)
            {
                try
                {
                    if (port.Receive(sb, 0, 1) == 1)
                    {
                        ns.Write(sb.Data, 0, 1);
                        //Debug.WriteLine("SpSrv com: " + sb.Data[0].ToString("X"));
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                catch { }
            }
            if (port != null && port.PortOpen())
            {
                port.ClosePort();
                port.Dispose();
                port = null;
            }
            Debug.WriteLine("Port closed");
        }
        void TcpTask()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, AppSettings.SerialServerPort);
            listener = new TcpListener(ipEndPoint);
            listener.Start();
            while (Connected)
            {
                TcpClient newClient = listener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(newClient);
            }
            listener.Stop();
            Logger("Server stopped");
        }
        private void HandleClientComm(object client)
        {
            try
            {
                TcpClient tcpClient = (TcpClient)client;
                ns = tcpClient.GetStream();
                byte[] buf = new byte[1];
                Logger("Client connected");
                Application.DoEvents();
                while (tcpClient.Connected)
                {
                    try
                    {
                        if (ns.Read(buf, 0, 1) == 1)
                        {
                            port.Send(buf);
                            //Debug.WriteLine("SpSrv ns: " + buf[0].ToString("X"));
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("Serialport server TCP: " + ex.Message);
                    }
                }
                Logger("Client disconnected");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmSerialPortServer line " + line + ": " + ex.Message);
            }
        }

        private void comboSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadPorts()
        {
            try
            {
                comboSerialPort.Items.Clear();
                if (IsRunningUnderWine())
                {
                    string fname = Path.Combine(Application.StartupPath, "serialports.txt");
                    if (File.Exists("serialports.txt"))
                    {
                        StreamReader sr = new StreamReader(fname);
                        string Line;
                        while ((Line = sr.ReadLine()) != null)
                        {
                            string[] portdata = Line.Split('\t');
                            if (portdata.Length == 3)
                            {
                                string s = portdata[2].ToUpper() + ": " + portdata[1];
                                comboSerialPort.Items.Add(s);
                            }
                        }
                        sr.Close();
                        if (!string.IsNullOrEmpty(AppSettings.SerialServerDevice) && comboSerialPort.Items.Contains(AppSettings.SerialServerDevice))
                            comboSerialPort.Text = AppSettings.SerialServerDevice;
                        else
                            comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                    }
                    return;
                }
                if (radioFTDI.Checked)
                {
                    string[] ftdiDevs = new string[0];
                    Task.Factory.StartNew(() =>
                    {
                        ftdiDevs = FTDI_Finder.FindFtdiDevices().ToArray();
                        for (int retry = 0; retry < 100; retry++)
                        {
                            if (!ftdiDevs.Contains("Unknown: Failure"))
                            {
                                Debug.WriteLine("FTDI list ok after " + retry.ToString() + " retries");
                                break;
                            }
                            Thread.Sleep(retry * 100);
                            ftdiDevs = FTDI_Finder.FindFtdiDevices().ToArray();
                        }
                        this.Invoke((MethodInvoker)delegate ()
                        {
                            if (ftdiDevs.Length > 0)
                            {
                                comboSerialPort.Items.AddRange(ftdiDevs);
                                if (!string.IsNullOrEmpty(AppSettings.SerialServerFtdiDevice) && comboSerialPort.Items.Contains(AppSettings.SerialServerFtdiDevice))
                                    comboSerialPort.Text = AppSettings.SerialServerFtdiDevice;
                                else
                                    comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                            }
                            else
                            {
                                comboSerialPort.Text = "";
                            }
                        });
                    });
                }
                else if (radioRS232.Checked)
                {
                    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                    {
                        var portnames = SerialPort.GetPortNames();
                        var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                        List<string> portList = portnames.Select(n => n + ": " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                        if (portList.Count > 0)
                        {
                            foreach (string s in portList)
                            {
                                Console.WriteLine(s);
                                if (!comboSerialPort.Items.Contains(s))
                                {
                                    comboSerialPort.Items.Add(s);
                                }
                            }
                            if (!string.IsNullOrEmpty(AppSettings.SerialServerDevice) && comboSerialPort.Items.Contains(AppSettings.SerialServerDevice))
                                comboSerialPort.Text = AppSettings.SerialServerDevice;
                            else
                                comboSerialPort.Text = comboSerialPort.Items[0].ToString();
                        }
                        else
                        {
                            comboSerialPort.Text = "";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, LoadPorts line " + line + ": " + ex.Message);
            }
        }

        private void frmSerialPortServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Connected = false;
            Thread.Sleep(100);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Connected = false;
        }
        private void Logger(string LogText)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                richLogger.AppendText(LogText + Environment.NewLine);
            });
        }
        private void LoggerBold(string LogText)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                richLogger.AppendText("\\b " + LogText + " \\b0" + Environment.NewLine);
            });
        }

        private void radioFTDI_CheckedChanged(object sender, EventArgs e)
        {
            LoadPorts();
        }
    }
}
