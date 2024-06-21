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
    public partial class frmSplashScreen : Form
    {
        public frmSplashScreen()
        {
            InitializeComponent();
        }

        public void moveMe(System.Drawing.Point location)
        {
            this.Location = location;
        }
        private void labelProgress_Click(object sender, EventArgs e)
        {

        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {
            if (AppSettings.SplashShowTime > 0)
            {
                timer1.Interval = AppSettings.SplashShowTime * 1000;
                timer1.Enabled = true;
            }
            else
            {
                this.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
