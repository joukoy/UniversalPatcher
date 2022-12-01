using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmGraphics : Form
    {
        public frmGraphics()
        {
            InitializeComponent();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void frmGraphics_Load(object sender, EventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                if (AppSettings.frmGraphicsWindowSize.Width > 0 || AppSettings.frmGraphicsWindowSize.Height > 0)
                {
                    this.WindowState = AppSettings.frmGraphicsWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = AppSettings.frmGraphicsWindowLocation;
                    this.Size = AppSettings.frmGraphicsWindowSize;
                }
            }

        }
        private void frmGraphics_FormClosing(object sender, EventArgs e)
        {
            if (AppSettings.MainWindowPersistence)
            {
                AppSettings.frmGraphicsWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    AppSettings.frmGraphicsWindowLocation = this.Location;
                    AppSettings.frmGraphicsWindowSize = this.Size;
                }
                else
                {
                    AppSettings.frmGraphicsWindowLocation = this.RestoreBounds.Location;
                    AppSettings.frmGraphicsWindowSize = this.RestoreBounds.Size;
                }
                AppSettings.Save();
            }
        }
    }
}
