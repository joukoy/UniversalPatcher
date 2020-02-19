using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmSegmentSettings : Form
    {
        public frmSegmentSettings()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void frmSegmentSettings_Load(object sender, EventArgs e)
        {

        }
        public void InitMe()
        {
            listSegments.Clear();
            listSegments.View = View.Details;
            listSegments.Columns.Add("Segment");
            listSegments.Columns.Add("Address");
            listSegments.Columns[0].Width = 200;
            listSegments.Columns[1].Width = 600;
            //listSegments.MultiSelect = true;
            listSegments.CheckBoxes = true;

        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            Segment S = new Segment();
            S.Name = txtSegmentName.Text;
            S.Addresses = txtSegmentAddress.Text;
            UInt32.TryParse(txtCSA1.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out S.CSA1);
            UInt32.TryParse(txtCSA2.Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out S.CSA2);
            if (radioNone.Checked)
                S.CSMethod1 = CSMethod_None;
            if (radioCrc16.Checked)
                S.CSMethod1 = CSMethod_crc16;
            if (radioCrc32.Checked)
                S.CSMethod1 = CSMethod_crc32;
            if (radioSUM.Checked)
                S.CSMethod1 = CSMethod_sum;
            if (radioInvSum.Checked)
                S.CSMethod1 = CSMethod_invsum;

            if (radio2None.Checked)
                S.CSMethod2 = CSMethod_None;
            if (radio2Crc16.Checked)
                S.CSMethod2 = CSMethod_crc16;
            if (radio2Crc32.Checked)
                S.CSMethod2 = CSMethod_crc32;
            if (radio2SUM.Checked)
                S.CSMethod2 = CSMethod_sum;
            if (radio2InvSum.Checked)
                S.CSMethod2 = CSMethod_invsum;

            if (radioByte.Checked)
                S.CSSize1 = 1;
            if (radioWord.Checked)
                S.CSSize1 = 2;
            if (radioDword.Checked)
                S.CSSize1 = 4;

            if (radio2Byte.Checked)
                S.CSSize2 = 1;
            if (radio2Word.Checked)
                S.CSSize2 = 2;
            if (radio2Dword.Checked)
                S.CSSize2 = 4;

            Segments.Add(S);

            var item = new ListViewItem(txtSegmentName.Text);
            item.SubItems.Add(txtSegmentAddress.Text);
            listSegments.Items.Add(item);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;

            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Segment>));
                writer.Serialize(stream, Segments);
                stream.Close();
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Segments.Clear();
            string FileName = SelectFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;

            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<Segment>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            Segments = (List<Segment>)reader.Deserialize(file);
            file.Close();

            foreach (Segment S in Segments)
            {
                var item = new ListViewItem(S.Name);
                item.SubItems.Add(S.Addresses);                
                listSegments.Items.Add(item);
            }

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
