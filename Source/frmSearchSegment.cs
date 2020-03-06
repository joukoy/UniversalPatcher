using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmSearchSegment : Form
    {
        public frmSearchSegment()
        {
            InitializeComponent();
        }

        public int CurrentSegment;

        public void InitMe(int SegmentNr)
        {
            CurrentSegment = SegmentNr;
            if (Segments[SegmentNr].EepromAddresses != null && Segments[SegmentNr].EepromAddresses != "")
            {
                txtEepromAddresses.Text = Segments[SegmentNr].EepromAddresses;
                txtSearchfor.Text = Segments[SegmentNr].EepromSearchfor;
                txtSearchfrom.Text = Segments[SegmentNr].EepromSearchfrom;
                chkNot.Checked = Segments[SegmentNr].EepromSearchNot;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SegmentConfig S = Segments[CurrentSegment];
            S.EepromAddresses = txtEepromAddresses.Text;
            S.EepromSearchfor = txtSearchfor.Text;
            S.EepromSearchfrom = txtSearchfrom.Text;
            S.EepromSearchNot = chkNot.Checked;
            Segments[CurrentSegment] = S;
            this.DialogResult = DialogResult.OK;
        }
    }
}
