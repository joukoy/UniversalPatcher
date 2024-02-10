using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmTunerMain : Form
    {
        public frmTunerMain()
        {
            InitializeComponent();
        }

        public frmTunerMain(PcmFile PCM)
        {
            InitializeComponent();
            this.PCM = PCM;
        }

        private PcmFile PCM;
        public enum SessionType
        {
            Empty,
            New,
            Load,
            SaveAs,
            Map
        }
        private void frmTunerMain_Load(object sender, EventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                if (AppSettings.TunerMainWindowSize.Width > 0 || AppSettings.TunerMainWindowSize.Height > 0)
                {
                    this.WindowState = AppSettings.TunerMainWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = AppSettings.TunerMainWindowLocation;
                    this.Size = AppSettings.TunerMainWindowSize;
                }
            }

            this.FormClosing += FrmTunerMain_FormClosing;
            AddSession(SessionType.Empty);
        }

        private void FrmTunerMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                AppSettings.TunerMainWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    AppSettings.TunerMainWindowLocation = this.Location;
                    AppSettings.TunerMainWindowSize = this.Size;
                }
                else
                {
                    AppSettings.TunerMainWindowLocation = this.RestoreBounds.Location;
                    AppSettings.TunerMainWindowSize = this.RestoreBounds.Size;
                }
            }
            if (AppSettings.ConfirmProgramExit)
            {
                foreach (TabPage tabPage in tabControl1.TabPages)
                {
                    FrmTuner ft = (FrmTuner)tabPage.Controls[0];
                    ft.Close();
                }
            }
        }

        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public void AddSession(SessionType SType)
        {
            try
            {
                PcmFile newPCM = new PcmFile();
                if (PCM != null)
                    newPCM = PCM;
                PCM = null; //Assign PCM only for first session
                FrmTuner ft = new FrmTuner(newPCM);
                ft.Dock = DockStyle.Fill;
                ft.FormBorderStyle = FormBorderStyle.None;
                ft.TopLevel = false;
                ft.TunerMain = this;
                string SessionName;
                switch(SType)
                {
                    case SessionType.Empty:
                        SessionName = "New Session";
                        break;
                    case SessionType.New:
                        SessionName = ft.SaveSession("",true);
                        break;
                    case SessionType.Load:
                        SessionName = ft.LoadSession();
                        break;
                    case SessionType.SaveAs:
                        SessionName = ft.SaveSession("",true);
                        break;
                    case SessionType.Map:
                        SessionName = ft.CreateMapSession();
                        break;
                    default:
                        SessionName = "";
                        break;
                }
                if (!string.IsNullOrEmpty(SessionName))
                {
                    tabControl1.TabPages.Add(SessionName);
                    tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(ft);
                    ft.myTab = tabControl1.TabPages[tabControl1.TabPages.Count - 1];
                    ft.Show();
                    tabControl1.SelectedTab = tabControl1.TabPages[tabControl1.TabPages.Count - 1];
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTunerMain , line " + line + ": " + ex.Message);
            }
        }
  
        public void CloseSession(TabPage tabPage)
        {
            FrmTuner ft = (FrmTuner)tabPage.Controls[0];
            ft.Close();
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
            if (tabControl1.TabPages.Count == 0)
            {
                this.Close();
            }
        }

        private void createMapSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSession(SessionType.Map);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmTuner ft = (FrmTuner)tabControl1.SelectedTab.Controls[0];
            ft.SaveSession(ft.SessionName,true);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmTuner ft = (FrmTuner)tabControl1.SelectedTab.Controls[0];
            ft.SaveSession("",true);
        }
    }
}
