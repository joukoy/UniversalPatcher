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
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmSegmenList : Form
    {
        public frmSegmenList()
        {
            InitializeComponent();
        }

        public PcmFile PCM;
        public void InitMe()
        {
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
                if (!PCM.Segments[s].Missing)
                {
                    var item = new ListViewItem(PCM.Segments[s].Name);
                    item.SubItems.Add(PCM.Segments[s].Addresses);
                    item.Tag = s;
                    listSegments.Items.Add(item);
                }
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
                    if (!PCM.Segments[s].Missing)
                    {
                        var item = new ListViewItem(PCM.Segments[s].Name);
                        if (PCM.Segments[s].Addresses != null)
                            item.SubItems.Add(PCM.Segments[s].Addresses);
                        else
                            item.SubItems.Add("");
                        item.Tag = s;
                        listSegments.Items.Add(item);
                    }
                }
                Logger(" [OK]");
                labelXML.Text = PCM.configFile;
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
            SaveXML("");
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
            PCM.SetDefaultValues();
            listSegments.Items.Clear();
            txtVersion.Text = "1";
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

        private void SaveXML(string fileName)
        {
            try
            {
                if (fileName.Length == 0)
                    fileName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*");
                if (fileName.Length == 0)
                    return;

                if (PCM.Segments.Count > 0)
                {
                    SegmentConfig S = PCM.Segments[0];
                    S.Version = txtVersion.Text;
                    PCM.Segments[0] = S;
                }
                Logger("Saving to file: " + Path.GetFileName(fileName), false);
                Debug.WriteLine("Saving to file: " + Path.GetFileName(fileName));

                PCM.SaveConfigFile(fileName);
                labelXML.Text = PCM.configFile;
                frmpatcher.labelXML.Text = PCM.configFile + " (v " + txtVersion.Text + ")"; ;
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }
        private void btnOK_Click(object sender, EventArgs e)
        {

            SaveXML(PCM.configFileFullName);
            this.Close();
        }

        private void listSegments_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSaveOnly_Click(object sender, EventArgs e)
        {
            SaveXML(PCM.configFileFullName);
        }

        private void btnEditXML_Click(object sender, EventArgs e)
        {
            try
            {
                frmEditXML fex = new frmEditXML();
                fex.Show();
                fex.LoadSegemtConfig(PCM);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
