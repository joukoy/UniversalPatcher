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
            if (UniversalPatcher.Properties.Settings.Default.MainWindowPersistence)
            {
                if (UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowSize.Width > 0 || UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowSize.Height > 0)
                {
                    this.WindowState = UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowLocation;
                    this.Size = UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowSize;
                }
            }

        }
        private void frmGraphics_FormClosing(object sender, EventArgs e)
        {
            if (UniversalPatcher.Properties.Settings.Default.MainWindowPersistence)
            {
                UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowLocation = this.Location;
                    UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowSize = this.Size;
                }
                else
                {
                    UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowLocation = this.RestoreBounds.Location;
                    UniversalPatcher.Properties.Settings.Default.frmGraphicsWindowSize = this.RestoreBounds.Size;
                }
                UniversalPatcher.Properties.Settings.Default.Save();
            }
        }
    }
}
