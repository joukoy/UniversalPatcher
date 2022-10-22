using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.frmGraphicsWindowSize.Width > 0 || Properties.Settings.Default.frmGraphicsWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.frmGraphicsWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.frmGraphicsWindowLocation;
                    this.Size = Properties.Settings.Default.frmGraphicsWindowSize;
                }
            }

        }
        private void frmGraphics_FormClosing(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.frmGraphicsWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmGraphicsWindowLocation = this.Location;
                    Properties.Settings.Default.frmGraphicsWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.frmGraphicsWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.frmGraphicsWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.Save();
            }
        }
    }
}
