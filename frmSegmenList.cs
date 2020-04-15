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

        public void InitMe()
        {
            labelXML.Text = Path.GetFileName(XMLFile);
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
            if (Segments == null)
                return;
            for (int s = 0; s < Segments.Count; s++)
            {
                var item = new ListViewItem(Segments[s].Name);
                item.SubItems.Add(Segments[s].Addresses);
                item.Tag = s;
                listSegments.Items.Add(item);
            }
            if (Segments.Count > 0)
                txtVersion.Text = Segments[0].Version;
        }

        public void LoadFile(string FileName)
        {
            try
            {
                Segments.Clear();
                Logger("Loading file: " + Path.GetFileName(FileName), false);
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                Segments = (List<SegmentConfig>)reader.Deserialize(file);
                file.Close();

                if (Segments[0].Version == null || Segments[0].Version == "")
                {
                    SegmentConfig S = Segments[0];
                    S.Version = "1";
                    Segments[0] = S;
                }
                listSegments.Items.Clear();
                for (int s = 0; s < Segments.Count; s++)
                {
                    var item = new ListViewItem(Segments[s].Name);
                    if (Segments[s].Addresses != null)
                        item.SubItems.Add(Segments[s].Addresses);
                    else
                        item.SubItems.Add("");
                    item.Tag = s;
                    listSegments.Items.Add(item);
                }
                Logger(" [OK]");
                XMLFile = FileName;
                labelXML.Text = Path.GetFileName(XMLFile);
                txtVersion.Text = Segments[0].Version;
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
                    writer.Serialize(stream, Segments);
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
            frmSS.EditSegment(CurrentSel);
            if (frmSS.ShowDialog(this) == DialogResult.OK)
                InitMe();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            Segments.RemoveAt((int)listSegments.SelectedItems[0].Tag);
            InitMe();

        }

        private void btnNewXML_Click(object sender, EventArgs e)
        {
            Segments.Clear();
            listSegments.Items.Clear();
            txtStatus.Text = "";
            txtVersion.Text = "1";
            XMLFile = "";
            labelXML.Text = "";
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            if (listSegments.SelectedItems[0].Text == Segments[0].Name)
                return;
            SegmentConfig Stmp = new SegmentConfig();
            int CurrentSel = (int)listSegments.SelectedItems[0].Tag;
            Stmp = Segments[CurrentSel - 1];
            Segments[CurrentSel - 1] = Segments[CurrentSel];
            Segments[CurrentSel] = Stmp;
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
            Stmp = Segments[CurrentSel + 1];
            Segments[CurrentSel + 1] = Segments[CurrentSel];
            Segments[CurrentSel] = Stmp;
            InitMe();
            listSegments.Items[CurrentSel + 1].Selected = true;

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (listSegments.SelectedItems.Count == 0)
                return;
            int CurrentSel = (int)listSegments.SelectedItems[0].Tag;
            SegmentConfig S = Segments[CurrentSel];
            Segments.Add(S);
            frmSegmentSettings frmSS = new frmSegmentSettings();
            frmSS.EditSegment(Segments.Count - 1);
            if (frmSS.ShowDialog(this) == DialogResult.OK)
                InitMe();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SegmentConfig S = new SegmentConfig();
            Segments.Add(S);
            frmSegmentSettings frmSS = new frmSegmentSettings();
            frmSS.EditSegment(Segments.Count - 1);
            if (frmSS.ShowDialog(this) == DialogResult.OK)
                InitMe();

        }

        private void btnOK_Click(object sender, EventArgs e)
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
                SegmentConfig S = Segments[0];
                S.Version = txtVersion.Text;
                Segments[0] = S;
                Logger("Saving to file: " + Path.GetFileName(FileName), false);
                Debug.WriteLine ("Saving to file: " + Path.GetFileName(FileName));

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<SegmentConfig>));
                    writer.Serialize(stream, Segments);
                    stream.Close();
                }
                Logger(" [OK]");
                this.Close();
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }
        }

        private void listSegments_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtStatus.AppendText(LogText);
            if (NewLine)
                txtStatus.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

    }
}
