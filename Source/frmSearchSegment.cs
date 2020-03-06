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
            if (Segments[SegmentNr].SearchAddresses != null && Segments[SegmentNr].SearchAddresses != "")
            {
                txtSearchAddresses.Text = Segments[SegmentNr].SearchAddresses;
                txtSearchfor.Text = Segments[SegmentNr].Searchfor;
                txtSearchfrom.Text = Segments[SegmentNr].Searchfrom;
                chkNot.Checked = Segments[SegmentNr].SearchNot;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SegmentConfig S = Segments[CurrentSegment];
            S.SearchAddresses = txtSearchAddresses.Text;
            S.Searchfor = txtSearchfor.Text;
            S.Searchfrom = txtSearchfrom.Text;
            S.SearchNot = chkNot.Checked;
            Segments[CurrentSegment] = S;
            this.DialogResult = DialogResult.OK;
        }
    }
}
