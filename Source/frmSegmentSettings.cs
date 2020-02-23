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

        public void InitMe()
        {
            listSegments.Clear();
            listSegments.View = View.Details;
            listSegments.Columns.Add("Segment");
            listSegments.Columns.Add("Address");
            listSegments.Columns.Add("");
            listSegments.Columns[0].Width = 200;
            listSegments.Columns[1].Width = 600;
            //listSegments.MultiSelect = true;
            //listSegments.CheckBoxes = true;
            listSegments.FullRowSelect = true;
            if (Segments == null)
                return;
            foreach (SegmentConfig S in Segments)
            {
                var item = new ListViewItem(S.Name);
                item.SubItems.Add(S.Addresses);
                listSegments.Items.Add(item);
            }
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            SegmentConfig S = new SegmentConfig();
            bool isNew = true;
            foreach (SegmentConfig Se in Segments)
            {
                if (Se.Name == txtSegmentName.Text)
                {
                    isNew = false;
                    S = Se;
                }
            }

            S.Name = txtSegmentName.Text;
/*            if (txtSegmentAddress.Text.StartsWith("@"))
                UInt32.TryParse(txtSegmentAddress.Text.Replace("@", ""), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out S.Address);
            else*/
                S.Addresses = txtSegmentAddress.Text;

            S.CSA1 = txtCSA1.Text;
            S.CSA2 = txtCSA2.Text;
            S.CSA1Block = txtCS1Block.Text;
            S.CSA2Block = txtCS2Block.Text;
           
            S.PNAddr = txtPNAddr.Text;
            S.VerAddr = txtVerAddr.Text;
            S.SegNrAddr = txtNrAddr.Text;

            if (radioNone.Checked)
                S.CSMethod1 = CSMethod_None;
            if (radioCrc16.Checked)
                S.CSMethod1 = CSMethod_crc16;
            if (radioCrc32.Checked)
                S.CSMethod1 = CSMethod_crc32;
            if (radioSUM.Checked)
                S.CSMethod1 = CSMethod_Bytesum;
            if (radioWordSum.Checked)
                S.CSMethod1 = CSMethod_Wordsum;
            if (radioDwordSum.Checked)
                S.CSMethod1 = CSMethod_Dwordsum;

            if (radio2None.Checked)
                S.CSMethod2 = CSMethod_None;
            if (radio2Crc16.Checked)
                S.CSMethod2 = CSMethod_crc16;
            if (radio2Crc32.Checked)
                S.CSMethod2 = CSMethod_crc32;
            if (radio2SUM.Checked)
                S.CSMethod2 = CSMethod_Bytesum;
            if (radio2WordSum.Checked)
                S.CSMethod2 = CSMethod_Wordsum;
            if (radio2DwordSum.Checked)
                S.CSMethod2 = CSMethod_Dwordsum;


            if (radioComplement0.Checked)
                S.Complement1 = 0;
            if (radioComplement1.Checked)
                S.Complement1 = 1;
            if (radioComplement2.Checked)
                S.Complement1 = 2;

            if (radio2Complement0.Checked)
                S.Complement2 = 0;
            if (radio2Complement1.Checked)
                S.Complement2 = 1;
            if (radio2Complement2.Checked)
                S.Complement2 = 2;

            if (isNew)
            {
                Segments.Add(S);
                var item = new ListViewItem(txtSegmentName.Text);
                item.SubItems.Add(txtSegmentAddress.Text);
                listSegments.Items.Add(item);
            }
            else
            {
                for (int i = 0; i<Segments.Count; i++)
                {
                    if (Segments[i].Name == S.Name )
                    {
                        Segments[i] = S;
                    }
                }
                for (int i = 0; i < listSegments.Items.Count; i++)
                {
                    if (listSegments.Items[i].SubItems[0].Text == S.Name)
                    {
                        listSegments.Items[i].SubItems[1].Text = txtSegmentAddress.Text;
                    }

                }
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string FileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;

            using (FileStream stream = new FileStream(FileName, FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
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
                new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
            System.IO.StreamReader file = new System.IO.StreamReader(FileName);
            Segments = (List<SegmentConfig>)reader.Deserialize(file);
            file.Close();

            listSegments.Items.Clear();
            foreach (SegmentConfig S in Segments)
            {
                var item = new ListViewItem(S.Name);
                if (S.Addresses != null)
                    item.SubItems.Add(S.Addresses);
                else
                    item.SubItems.Add("");
                listSegments.Items.Add(item);
            }

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void listSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count < 1)
                return;
            foreach (SegmentConfig S in Segments)
            {
                if (S.Name == listSegments.SelectedItems[0].Text)
                {
                    txtSegmentName.Text = S.Name;
                    txtSegmentAddress.Text = S.Addresses;
                    txtCSA1.Text = S.CSA1;
                    txtCSA2.Text = S.CSA2;
                    txtCS1Block.Text = S.CSA1Block;
                    txtCS2Block.Text = S.CSA2Block;
                    txtPNAddr.Text = S.PNAddr;
                    txtVerAddr.Text = S.VerAddr;
                    txtNrAddr.Text = S.SegNrAddr;
                    if (S.CSMethod1 == CSMethod_None)
                        radioNone.Checked = true;
                    if (S.CSMethod1 == CSMethod_crc16)
                        radioCrc16.Checked = true;
                    if (S.CSMethod1 == CSMethod_crc32)
                        radioCrc32.Checked = true;
                    if (S.CSMethod1 == CSMethod_Bytesum)
                        radioSUM.Checked = true;
                    if (S.CSMethod1 == CSMethod_Wordsum)
                        radioWordSum.Checked = true;
                    if (S.CSMethod1 == CSMethod_Dwordsum)
                        radioDwordSum.Checked = true;
                    if (S.CSMethod2 == CSMethod_None)
                        radio2None.Checked = true;
                    if (S.CSMethod2 == CSMethod_crc16)
                        radio2Crc16.Checked = true;
                    if (S.CSMethod2 == CSMethod_crc32)
                        radio2Crc32.Checked = true;
                    if (S.CSMethod2 == CSMethod_Bytesum)
                        radio2SUM.Checked = true;
                    if (S.CSMethod2 == CSMethod_Wordsum)
                        radio2WordSum.Checked = true;
                    if (S.CSMethod2 == CSMethod_Dwordsum)
                        radio2DwordSum.Checked = true;
                    if (S.Complement1 == 0)
                        radioComplement0.Checked = true;
                    if (S.Complement1 == 1)
                        radioComplement1.Checked = true;
                    if (S.Complement1 == 2)
                        radioComplement2.Checked = true;
                    if (S.Complement2 == 0)
                        radio2Complement0.Checked = true;
                    if (S.Complement2 == 1)
                        radio2Complement1.Checked = true;
                    if (S.Complement2 == 2)
                        radio2Complement2.Checked = true;

                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            for (int s = 0; s < Segments.Count; s++)
            {
                if (Segments[s].Name == listSegments.SelectedItems[0].Text)
                {
                    Segments.RemoveAt(s);
                }
            }
            listSegments.SelectedItems[0].Remove();
        }

        private void frmSegmentSettings_FormClosing(object sender, EventArgs e)
        {
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtVerAddr_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
