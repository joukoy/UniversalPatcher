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
    public partial class frmSearchSegment : Form
    {
        public frmSearchSegment()
        {
            InitializeComponent();
        }

        public int CurrentSegment;
        private PcmFile PCM;
        public void InitMe(PcmFile PCM1, int SegmentNr)
        {
            CurrentSegment = SegmentNr;
            PCM = PCM1;
            if (PCM.Segments[SegmentNr].SearchAddresses != null && PCM.Segments[SegmentNr].SearchAddresses != "")
            {
                txtSearchAddresses.Text = PCM.Segments[SegmentNr].SearchAddresses;
                txtSearchfor.Text = PCM.Segments[SegmentNr].Searchfor;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SegmentConfig S = PCM.Segments[CurrentSegment];
            S.SearchAddresses = txtSearchAddresses.Text;
            S.Searchfor = txtSearchfor.Text;
            //S.Searchfrom = txtSearchfrom.Text;
            //S.SearchNot = chkNot.Checked;
            PCM.Segments[CurrentSegment] = S;
            this.DialogResult = DialogResult.OK;
        }
    }
}
