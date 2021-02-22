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
    }
}
