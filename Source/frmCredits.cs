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
    public partial class frmCredits : Form
    {
        public frmCredits()
        {
            InitializeComponent();
        }

        private void Credits_Load(object sender, EventArgs e)
        {
            try
            {
                richCredits.Dock = DockStyle.Fill;
                richCredits.LoadFile("Credits.rtf");
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
                this.Dispose();
            }
        }
    }
}
