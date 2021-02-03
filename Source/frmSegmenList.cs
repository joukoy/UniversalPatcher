using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmSegmenList : Form
    {
        public frmSegmenList()
        {
            InitializeComponent();
        }

        public PcmFile PCM;
        private string XMLFile;
        public void InitMe()
        {
            XMLFile = PCM.configFileFullName;
            labelXML.Text = PCM.configFile;
            listSegments.Clear();
            listSegments.View = View.Details;
            listSegments.Columns.Add("Segment");
            listSegments.Columns.Add("Address");
            listSegments.Columns.Add("");
            listSegments.Columns[0].Width = 200;
            listSegments.Columns[1].Width = 600;
            listSegments.MultiSelect = false;
            listSegments.CheckBoxes = false;
            listSegments.FullRowSelect = true;
            if (PCM.Segments == null)
                return;
            for (int s = 0; s < PCM.Segments.Count; s++)
            {
                var item = new ListViewItem(PCM.Segments[s].Name);
                item.SubItems.Add(PCM.Segments[s].Addresses);
                item.Tag = s;
                listSegments.Items.Add(item);
            }
            if (PCM.Segments.Count > 0)
                txtVersion.Text = PCM.Segments[0].Version;
        }

        public void LoadFile(string FileName)
        {
            try
            {
                listSegments.Items.Clear();
                for (int s = 0; s < PCM.Segments.Count; s++)
                {
                    var item = new ListViewItem(PCM.Segments[s].Name);
                    if (PCM.Segments[s].Addresses != null)
                        item.SubItems.Add(PCM.Segments[s].Addresses);
                    else
                        item.SubItems.Add("");
                    item.Tag = s;
                    listSegments.Items.Add(item);
                }
                Logger(" [OK]");
                XMLFile = FileName;
                labelXML.Text = Path.GetFileName(XMLFile);
                txtVersion.Text = PCM.Segments[0].Version;
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string FileName = SelectFile("Select XML file", "XML files (*.xml)|*.xml|All files (*.*)|*.*");
            if (FileName.Length < 1)
                return;
            PCM.LoadConfigFile(FileName);
            LoadFile(FileName);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string FileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
                if (FileName.Length < 1)
                    return;
                Logger("Saving to file: " + Path.GetFileName(FileName), false);

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
                    writer.Serialize(stream, PCM.Segments);
                    stream.Close();
                }
                Logger(" [OK]");
                XMLFile = FileName;
                labelXML.Text = Path.GetFileName(XMLFile);
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            frmSegmentSettings frmSS = new frmSegmentSettings();
            int CurrentSel = (int)listSegments.SelectedItems[0].Tag;
            frmSS.EditSegment(PCM, CurrentSel);
            if (frmSS.ShowDialog(this) == DialogResult.OK)
                InitMe();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            PCM.Segments.RemoveAt((int)listSegments.SelectedItems[0].Tag);
            InitMe();

        }

        private void btnNewXML_Click(object sender, EventArgs e)
        {
            PCM.Segments.Clear();
            PCM.configFile = "";
            listSegments.Items.Clear();
            txtVersion.Text = "1";
            XMLFile = "";
            labelXML.Text = "";
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            if (listSegments.SelectedItems[0].Text == PCM.Segments[0].Name)
                return;
            SegmentConfig Stmp = new SegmentConfig();
            int CurrentSel = (int)listSegments.SelectedItems[0].Tag;
            Stmp = PCM.Segments[CurrentSel - 1];
            PCM.Segments[CurrentSel - 1] = PCM.Segments[CurrentSel];
            PCM.Segments[CurrentSel] = Stmp;
            InitMe();
            listSegments.Items[CurrentSel - 1].Selected = true;
            labelXML.Text = "";

        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            if ((int)listSegments.SelectedItems[0].Tag == listSegments.Items.Count - 1)
                return;
            SegmentConfig Stmp = new SegmentConfig();
            int CurrentSel = (int)listSegments.SelectedItems[0].Tag;
            Stmp = PCM.Segments[CurrentSel + 1];
            PCM.Segments[CurrentSel + 1] = PCM.Segments[CurrentSel];
            PCM.Segments[CurrentSel] = Stmp;
            InitMe();
            listSegments.Items[CurrentSel + 1].Selected = true;

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            int CurrentSel = (int)listSegments.SelectedItems[0].Tag;
            SegmentConfig S = PCM.Segments[CurrentSel];
            PCM.Segments.Add(S);
            frmSegmentSettings frmSS = new frmSegmentSettings();
            frmSS.EditSegment(PCM, PCM.Segments.Count - 1);
            if (frmSS.ShowDialog(this) == DialogResult.OK)
                InitMe();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SegmentConfig S = new SegmentConfig();
            PCM.Segments.Add(S);
            frmSegmentSettings frmSS = new frmSegmentSettings();
            frmSS.EditSegment(PCM, PCM.Segments.Count - 1);
            if (frmSS.ShowDialog(this) == DialogResult.OK)
                InitMe();

        }

        private void saveXML()
        {
            try
            {
                string FileName;
                if (XMLFile == "")
                    FileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
                else
                    FileName = XMLFile;
                if (FileName.Length < 1)
                    return;
                SegmentConfig S = PCM.Segments[0];
                S.Version = txtVersion.Text;
                PCM.Segments[0] = S;
                Logger("Saving to file: " + Path.GetFileName(FileName), false);
                Debug.WriteLine("Saving to file: " + Path.GetFileName(FileName));

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
                    writer.Serialize(stream, PCM.Segments);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            saveXML();
            this.Close();
        }

        private void listSegments_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveOnly_Click(object sender, EventArgs e)
        {
            saveXML();
        }
    }
}
