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
    public partial class frmAutodetect : Form
    {
        public frmAutodetect()
        {
            InitializeComponent();
        }

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
            string FileName = SelectFile("Select XML-file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            comboXML.Text = Path.GetFileName(FileName).ToLower();
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
            UInt64 x;
            HexToUint64(txtData.Text, out x);
            DR.data = x;
            DR.group = (ushort) numGroup.Value;
            DR.grouplogic = comboGroupLogic.Text;
            DR.xml = comboXML.Text;
            DetectRules.Add(DR);

            var item = new ListViewItem(DR.address);
            item.SubItems.Add(DR.compare);
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
            int i = (int)listRules.SelectedItems[0].Tag;
            DetectRules.RemoveAt(i);
            listRules.SelectedItems[0].Remove();
        }

        private void comboXML_SelectedIndexChanged(object sender, EventArgs e)
        {
            listRules.Items.Clear();
            for (int s = 0; s < DetectRules.Count; s++)
            {
                if (DetectRules[s].xml.ToLower() == comboXML.Text)
                {
                    var item = new ListViewItem(DetectRules[s].address);
                    item.SubItems.Add(DetectRules[s].compare);
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

            int d = (int)listRules.SelectedItems[0].Tag;
            DR.address = txtAddress.Text;
            DR.compare = comboCompare.Text;
            UInt64 x;
            HexToUint64(txtData.Text, out x);
            DR.data = x;
            DR.group = (ushort)numGroup.Value;
            DR.grouplogic = comboGroupLogic.Text;
            DR.xml = comboXML.Text;

            DetectRules[d] = DR;

            listRules.SelectedItems[0].SubItems[0].Text = txtAddress.Text;
            listRules.SelectedItems[0].SubItems[1].Text = comboCompare.Text;
            listRules.SelectedItems[0].SubItems[2].Text = txtData.Text;
            listRules.SelectedItems[0].SubItems[3].Text = numGroup.Value.ToString();
            listRules.SelectedItems[0].SubItems[4].Text = comboGroupLogic.Text;

        }
        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtStatus.AppendText(LogText);
            if (NewLine)
                txtStatus.AppendText(Environment.NewLine);
            Application.DoEvents();
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
                comboXML_SelectedIndexChanged(sender,e);
            }
            frmN.Dispose();
        }
    }

}
