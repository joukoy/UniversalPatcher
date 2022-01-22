using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmAutodetect : Form
    {
        public frmAutodetect()
        {
            InitializeComponent();
        }

        public PcmFile PCM;

        public void InitMe()
        {
            listRules.Clear();
            listRules.View = View.Details;
            listRules.Columns.Add("Address");
            listRules.Columns.Add("");
            listRules.Columns.Add("Data");
            listRules.Columns.Add("Group");
            listRules.Columns.Add(" ");
            listRules.Columns[0].Width = 70;
            listRules.Columns[1].Width = 30;
            listRules.Columns[2].Width = 150;
            listRules.Columns[3].Width = 50;
            listRules.MultiSelect = false;
            listRules.CheckBoxes = false;
            listRules.FullRowSelect = true;
            LoadXMLList();

        }

        private void LoadXMLList()
        {
            Logger("loading list...");
            comboXML.Items.Clear();
            for (int d=0;d<DetectRules.Count;d++)
            {
                if (!comboXML.Items.Contains(DetectRules[d].xml.ToLower()))
                    comboXML.Items.Add(DetectRules[d].xml.ToLower());
            }
            if (comboXML.Text.Length == 0)
                comboXML.Text = DetectRules[0].xml.ToLower();
            Logger("[OK]");
        }

        private void frmAutodetect_Load(object sender, EventArgs e)
        {

        }

        private string EditAddress(string OldAddress, bool extra = false)
        {
            frmEditDetectAddress frmE = new frmEditDetectAddress();
            frmE.ParseAddress(OldAddress);
            string Res = OldAddress;
            if (frmE.ShowDialog(this) == DialogResult.OK)
            {
                Res = frmE.Result;
            }
            return Res;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select XML-file", XmlFilter);
            if (FileName.Length < 1)
                return;
            comboXML.Text = Path.GetFileName(FileName).ToLower();
            SelectXML();
        }

        private void btnAddr_Click(object sender, EventArgs e)
        {
            txtAddress.Text = EditAddress(txtAddress.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text == "" || txtData.Text == "")
                return;
            DetectRule DR = new DetectRule();
            DR.address = txtAddress.Text;
            DR.compare = comboCompare.Text;
            if (radioData.Checked && comboCompare.Text == "==")
            {
                DR.hexdata = txtData.Text;
            }
            else
            {
                UInt64 x;
                HexToUint64(txtData.Text, out x);
                DR.data = x;
            }
            DR.group = (ushort) numGroup.Value;
            DR.grouplogic = comboGroupLogic.Text;
            DR.xml = comboXML.Text;
            DetectRules.Add(DR);

            int bytes = 1;
            if (txtAddress.Text.Contains(":"))
            {
                string[] Parts = txtAddress.Text.Split(':');
                int.TryParse(Parts[1], out bytes);
            }

            var item = new ListViewItem(DR.address);
            item.SubItems.Add(DR.compare);
            if (DR.hexdata!= null && DR.hexdata.Length > 0)
                item.SubItems.Add(DR.hexdata.PadLeft(bytes*2,'0'));
            else
                item.SubItems.Add(DR.data.ToString("X"));
            item.SubItems.Add(DR.group.ToString());
            item.SubItems.Add(DR.grouplogic);
            item.Tag = DetectRules.Count - 1;
            listRules.Items.Add(item);

            comboGroupLogic.Enabled = false;
            LoadXMLList();

        }

        private void radioFilesize_CheckedChanged(object sender, EventArgs e)
        {
            if (radioFilesize.Checked)
            {
                txtAddress.Text = "filesize";
                txtAddress.Enabled = false;
            }
            else
            {
                txtAddress.Text = "";
                txtAddress.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Logger("Saving file autodetect.xml");
                string FileName = Path.Combine(Application.StartupPath, "XML", "autodetect.xml");
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<DetectRule>));
                    writer.Serialize(stream, DetectRules);
                    stream.Close();
                }
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }


        private void numGroup_ValueChanged(object sender, EventArgs e)
        {
            comboGroupLogic.Enabled = true;
            for (int s=0; s < DetectRules.Count; s++)
            {
                if (DetectRules[s].xml.ToLower() == comboXML.Text && DetectRules[s].group == numGroup.Value)
                {
                    comboGroupLogic.Text = DetectRules[s].grouplogic;
                    comboGroupLogic.Enabled = false;
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (listRules.SelectedItems.Count == 0)
                return;
            try
            {
                int i = (int)listRules.SelectedItems[0].Tag;
                DetectRules.RemoveAt(i);
                listRules.SelectedItems[0].Remove();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void SelectXML()
        {
            listRules.Items.Clear();
            for (int s = 0; s < DetectRules.Count; s++)
            {
                if (DetectRules[s].xml.ToLower() == comboXML.Text)
                {
                    var item = new ListViewItem(DetectRules[s].address);
                    item.SubItems.Add(DetectRules[s].compare);
                    if (DetectRules[s].hexdata != null && DetectRules[s].hexdata.Length > 0)
                        item.SubItems.Add(DetectRules[s].hexdata);
                    else
                        item.SubItems.Add(DetectRules[s].data.ToString("X"));
                    item.SubItems.Add(DetectRules[s].group.ToString());
                    item.SubItems.Add(DetectRules[s].grouplogic);
                    item.Tag = s;
                    listRules.Items.Add(item);

                    numGroup.Value = DetectRules[s].group;
                    comboGroupLogic.Text = DetectRules[s].grouplogic;
                    comboGroupLogic.Enabled = false;
                }
            }

        }
        private void comboXML_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectXML();
        }

        private void listRules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listRules.SelectedItems.Count == 0)
                return;
            int d = (int)listRules.SelectedItems[0].Tag;
            numGroup.Value = DetectRules[d].group;
            comboGroupLogic.Text = DetectRules[d].grouplogic;
            if (DetectRules[d].address == "filesize")
                radioFilesize.Checked = true;
            else
                radioData.Checked = true;
            txtAddress.Text = DetectRules[d].address;
            comboCompare.Text = DetectRules[d].compare;
            if (DetectRules[d].hexdata != null && DetectRules[d].hexdata.Length > 0)
                txtData.Text = DetectRules[d].hexdata;
            else
                txtData.Text = DetectRules[d].data.ToString("X");
        }

        private void comboCompare_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void comboGroupLogic_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text == "" || txtData.Text == "")
                return;
            if (listRules.SelectedItems.Count == 0)
                return;

            DetectRule DR = new DetectRule();

            int bytes = 1;
            if (txtAddress.Text.Contains(":"))
            {
                string[] Parts = txtAddress.Text.Split(':');
                int.TryParse(Parts[1], out bytes);
            }

            int d = (int)listRules.SelectedItems[0].Tag;
            DR.address = txtAddress.Text;
            DR.compare = comboCompare.Text;
            if (comboCompare.Text == "==")
            {
                DR.hexdata = txtData.Text.PadLeft(bytes*2,'0');
            }
            else
            {
                UInt64 x;
                HexToUint64(txtData.Text, out x);
                DR.data = x;
            }
            DR.group = (ushort)numGroup.Value;
            DR.grouplogic = comboGroupLogic.Text;
            DR.xml = comboXML.Text;

            DetectRules[d] = DR;

            listRules.SelectedItems[0].SubItems[0].Text = txtAddress.Text;
            listRules.SelectedItems[0].SubItems[1].Text = comboCompare.Text;
            listRules.SelectedItems[0].SubItems[2].Text = txtData.Text.PadLeft(bytes * 2, '0');
            listRules.SelectedItems[0].SubItems[3].Text = numGroup.Value.ToString();
            listRules.SelectedItems[0].SubItems[4].Text = comboGroupLogic.Text;

        }
        private void btnEditXML_Click(object sender, EventArgs e)
        {
            frmEditXML frmEX = new frmEditXML();
            frmEX.LoadRules();
            frmEX.ShowDialog(this);
            InitMe();
        }

        private void btnRenameXML_Click(object sender, EventArgs e)
        {
            frmRenameXML frmN = new frmRenameXML();
            frmN.txtOldXML.Text = comboXML.Text;
            frmN.txtNewXML.Text = comboXML.Text;
            if (frmN.ShowDialog(this) == DialogResult.OK)
            {
                for (int d=0; d< DetectRules.Count;d++)
                {
                    if (DetectRules[d].xml.ToLower() == frmN.txtOldXML.Text.ToLower())
                    {
                        DetectRule DR = DetectRules[d];
                        DR.xml = frmN.txtNewXML.Text;
                        DetectRules[d] = DR;
                    }
                }
                InitMe();
                if (frmN.txtOldXML.Text == comboXML.Text)
                    comboXML.Text = frmN.txtNewXML.Text;
                SelectXML();
            }
            frmN.Dispose();
        }

        public void readDataFromBin()
        {
            uint addr;
            string address = txtAddress.Text;
            if (address.Length == 0)
                return ;

            string[] Parts = address.Split(':');
            HexToUint(Parts[0].Replace("@", ""), out addr);
            if (address.StartsWith("@"))
                addr = PCM.ReadUInt32(addr);
            if (Parts[0].EndsWith("@"))
                addr = (uint)PCM.buf.Length - addr;

            uint bytes = 1;
            uint.TryParse(Parts[1], out bytes);
            StringBuilder readBytes = new StringBuilder();
            for (int a = 0; a < bytes; a++)
            {
                readBytes.Append(PCM.buf[addr + a].ToString("X2"));
            }
            txtData.Text = readBytes.ToString();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                if (PCM.buf != null && PCM.buf.Length > 0)
                {
                    readDataFromBin();

                }
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int bytes = 1;
                if (txtAddress.Text.Contains(":"))
                {
                    string[] Parts = txtAddress.Text.Split(':');
                    int.TryParse(Parts[1], out bytes);
                }

                if (txtData.Text.Length > (bytes * 2))
                    txtData.BackColor = Color.Red;
                else
                    txtData.BackColor = Color.White;
            }
            catch { }
        }
    }

}
