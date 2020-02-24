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

            S.CS1Address = txtCS1Address.Text;
            S.CS2Address = txtCS2Address.Text;
            S.CS1Blocks = txtCS1Block.Text;
            S.CS2Blocks = txtCS2Block.Text;
           
            S.PNAddr = txtPNAddr.Text;
            S.VerAddr = txtVerAddr.Text;
            S.SegNrAddr = txtNrAddr.Text;

            if (radioCS1None.Checked)
                S.CS1Method = CSMethod_None;
            if (radioCS1Crc16.Checked)
                S.CS1Method = CSMethod_crc16;
            if (radioCS1Crc32.Checked)
                S.CS1Method = CSMethod_crc32;
            if (radioCS1SUM.Checked)
                S.CS1Method = CSMethod_Bytesum;
            if (radioCS1WordSum.Checked)
                S.CS1Method = CSMethod_Wordsum;
            if (radioCS1DwordSum.Checked)
                S.CS1Method = CSMethod_Dwordsum;

            if (radioCS2None.Checked)
                S.CS2Method = CSMethod_None;
            if (radioCS2Crc16.Checked)
                S.CS2Method = CSMethod_crc16;
            if (radioCS2Crc32.Checked)
                S.CS2Method = CSMethod_crc32;
            if (radioCS2SUM.Checked)
                S.CS2Method = CSMethod_Bytesum;
            if (radioCS2WordSum.Checked)
                S.CS2Method = CSMethod_Wordsum;
            if (radioCS2DwordSum.Checked)
                S.CS2Method = CSMethod_Dwordsum;


            if (radioCS1Complement0.Checked)
                S.CS1Complement = 0;
            if (radioCS1Complement1.Checked)
                S.CS1Complement = 1;
            if (radioCS1Complement2.Checked)
                S.CS1Complement = 2;

            if (radioCS2Complement0.Checked)
                S.CS2Complement = 0;
            if (radioCS2Complement1.Checked)
                S.CS2Complement = 1;
            if (radioCS2Complement2.Checked)
                S.CS2Complement = 2;

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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try { 
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadFile(string FileName)
        {
            try
            {
                Segments.Clear();

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            LoadFile(FileName);
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
                    txtCS1Address.Text = S.CS1Address;
                    txtCS2Address.Text = S.CS2Address;
                    txtCS1Block.Text = S.CS1Blocks;
                    txtCS2Block.Text = S.CS2Blocks;
                    txtPNAddr.Text = S.PNAddr;
                    txtVerAddr.Text = S.VerAddr;
                    txtNrAddr.Text = S.SegNrAddr;
                    if (S.CS1Method == CSMethod_None)
                        radioCS1None.Checked = true;
                    if (S.CS1Method == CSMethod_crc16)
                        radioCS1Crc16.Checked = true;
                    if (S.CS1Method == CSMethod_crc32)
                        radioCS1Crc32.Checked = true;
                    if (S.CS1Method == CSMethod_Bytesum)
                        radioCS1SUM.Checked = true;
                    if (S.CS1Method == CSMethod_Wordsum)
                        radioCS1WordSum.Checked = true;
                    if (S.CS1Method == CSMethod_Dwordsum)
                        radioCS1DwordSum.Checked = true;
                    if (S.CS2Method == CSMethod_None)
                        radioCS2None.Checked = true;
                    if (S.CS2Method == CSMethod_crc16)
                        radioCS2Crc16.Checked = true;
                    if (S.CS2Method == CSMethod_crc32)
                        radioCS2Crc32.Checked = true;
                    if (S.CS2Method == CSMethod_Bytesum)
                        radioCS2SUM.Checked = true;
                    if (S.CS2Method == CSMethod_Wordsum)
                        radioCS2WordSum.Checked = true;
                    if (S.CS2Method == CSMethod_Dwordsum)
                        radioCS2DwordSum.Checked = true;
                    if (S.CS1Complement == 0)
                        radioCS1Complement0.Checked = true;
                    if (S.CS1Complement == 1)
                        radioCS1Complement1.Checked = true;
                    if (S.CS1Complement == 2)
                        radioCS1Complement2.Checked = true;
                    if (S.CS2Complement == 0)
                        radioCS2Complement0.Checked = true;
                    if (S.CS2Complement == 1)
                        radioCS2Complement1.Checked = true;
                    if (S.CS2Complement == 2)
                        radioCS2Complement2.Checked = true;

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
