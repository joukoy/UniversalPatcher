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
using System.Text.RegularExpressions;

using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UniversalPatcher
{
    public partial class frmPatcheditor : Form
    {
        public frmPatcheditor()
        {
            InitializeComponent();
        }

        private BindingSource bindingSource = new BindingSource();

        private void SavePatch(string Description)
        {
            try
            {
                if (PatchList == null || PatchList.Count == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                /*if (Description.Length < 1 && PatchList[0].Description == null)
                {
                    Logger("Supply patch description");
                    return;
                }*/
                string FileName = SelectSaveFile("XMLPATCH files (*.xmlpatch)|*.xmlpatch|All files (*.*)|*.*");
                if (FileName.Length < 1)
                    return;
                Logger("Saving to file: " + Path.GetFileName(FileName), false);
                if (PatchList[0].Description == null)
                {
                    XmlPatch xpatch = PatchList[0];
                    xpatch.Description = Description;
                    PatchList[0] = xpatch;
                }

                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    writer.Serialize(stream, PatchList);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        public void RefreshDatagrid()
        {
            bindingSource.DataSource = null;
            bindingSource.DataSource = PatchList;
            dataPatch.DataSource = null;
            dataPatch.DataSource = bindingSource;
        }

        private void btnManualPatch_Click(object sender, EventArgs e)
        {
            frmManualPatch frmM = new frmManualPatch();
            frmM.MdiParent = this;
            if (PatchList != null && PatchList.Count > 0)
            {
                string[] Oslist = PatchList[0].CompatibleOS.Split(',');
                foreach (string os in Oslist)
                {
                    if (frmM.txtCompOS.Text.Length > 0)
                        frmM.txtCompOS.Text += ",";
                    string[] Parts = os.Split(':');
                    frmM.txtCompOS.Text += Parts[0] + ":";
                    frmM.txtXML.Text = PatchList[0].XmlFile;
                    frmM.txtDescription.Text = PatchList[0].Description;
                }
            }
            else
            {
                frmM.txtCompOS.Text = "ALL:";
                if (XMLFile != null && XMLFile.Length > 0)
                    frmM.txtXML.Text = Path.GetFileName(XMLFile);
            }
            
            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                if (PatchList == null)
                    PatchList = new List<XmlPatch>();
                XmlPatch XP = new XmlPatch();
                XP.Description = frmM.txtDescription.Text;
                XP.Segment = frmM.txtSegment.Text;
                XP.XmlFile = frmM.txtXML.Text;
                XP.CompatibleOS = frmM.txtCompOS.Text;
                XP.Data = frmM.txtData.Text;
                if (frmM.txtReadAddr.Text.Length > 0)
                {
                    XP.Rule = frmM.txtReadAddr.Text + "[" + frmM.txtMask.Text + "]";
                    if (frmM.chkNOT.Checked)
                        XP.Rule += "!";
                    XP.Rule += frmM.txtValue.Text;
                }
                PatchList.Add(XP);
                RefreshDatagrid();
            }
        }

        private void btnBinLoadPatch_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        public void EditPatchRow()
        {
            if (dataPatch.SelectedRows.Count == 0 && dataPatch.SelectedCells.Count == 0)
                return;
            if (dataPatch.SelectedRows.Count == 0)
                dataPatch.Rows[dataPatch.SelectedCells[0].RowIndex].Selected = true;
            frmManualPatch frmM = new frmManualPatch();
            if (dataPatch.CurrentRow.Cells[0].Value != null)
                frmM.txtDescription.Text = dataPatch.CurrentRow.Cells[0].Value.ToString();
            if (dataPatch.CurrentRow.Cells[1].Value != null)
                frmM.txtXML.Text = dataPatch.CurrentRow.Cells[1].Value.ToString();
            if (dataPatch.CurrentRow.Cells[2].Value != null)
                frmM.txtSegment.Text = dataPatch.CurrentRow.Cells[2].Value.ToString();
            if (dataPatch.CurrentRow.Cells[3].Value != null)
                frmM.txtCompOS.Text = dataPatch.CurrentRow.Cells[3].Value.ToString();
            if (dataPatch.CurrentRow.Cells[4].Value != null)
                frmM.txtData.Text = dataPatch.CurrentRow.Cells[4].Value.ToString();
            if (dataPatch.CurrentRow.Cells[5].Value != null && dataPatch.CurrentRow.Cells[5].Value.ToString().Contains(":"))
            {
                string[] Parts = dataPatch.CurrentRow.Cells[5].Value.ToString().Split(':');
                if (Parts.Length == 3)
                {
                    frmM.txtReadAddr.Text = Parts[0];
                    frmM.txtMask.Text = Parts[1];
                    frmM.txtValue.Text = Parts[2];
                }
            }
            if (dataPatch.CurrentRow.Cells[6].Value != null)
                frmM.txtHelpFile.Text = dataPatch.CurrentRow.Cells[6].Value.ToString();

            if (frmM.ShowDialog(this) == DialogResult.OK)
            {
                dataPatch.CurrentRow.Cells[0].Value = frmM.txtDescription.Text;
                dataPatch.CurrentRow.Cells[1].Value = frmM.txtXML.Text;
                dataPatch.CurrentRow.Cells[2].Value = frmM.txtSegment.Text;
                dataPatch.CurrentRow.Cells[3].Value = frmM.txtCompOS.Text;
                dataPatch.CurrentRow.Cells[4].Value = frmM.txtData.Text;
                if (frmM.txtReadAddr.Text.Length > 0)
                {
                    dataPatch.CurrentRow.Cells[5].Value = frmM.txtReadAddr.Text + ":" + frmM.txtMask.Text + ":" + frmM.txtValue.Text;
                }
                dataPatch.CurrentRow.Cells[6].Value = frmM.txtHelpFile.Text;
            }
            frmM.Dispose();

        }

        private void dataPatch_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditPatchRow();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditPatchRow();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataPatch.SelectedRows.Count == 0)
                return;
            dataPatch.Rows.Remove(dataPatch.CurrentRow);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int row = dataPatch.CurrentRow.Index; 
            if (row == 0)
                return;
            XmlPatch CurrentP = PatchList[row];
            PatchList.RemoveAt(row);
            PatchList.Insert(row - 1, CurrentP);
            RefreshDatagrid();
            dataPatch.CurrentCell = dataPatch.Rows[row - 1].Cells[0];
            dataPatch.Rows[row - 1].Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int row = dataPatch.CurrentRow.Index;
            if (row >= dataPatch.Rows.Count - 2)
                return;
            XmlPatch CurrentP = PatchList[row];
            PatchList.RemoveAt(row);
            PatchList.Insert(row + 1, CurrentP);
            RefreshDatagrid();
            dataPatch.CurrentCell = dataPatch.Rows[row + 1].Cells[0];
            dataPatch.Rows[row + 1].Selected = true;
        }

        public void LoadPatch(string FileName = "")
        {
            try
            {
                if (FileName == "")
                    FileName = SelectFile("Select patch file", "XML patch files (*.xmlpatch)|*.xmlpatch|PATCH files (*.patch)|*.patch|ALL files(*.*)|*.*");
                if (FileName.Length > 1)
                {
                    labelPatchname.Text = FileName;
                    Logger("Loading file: " + FileName);
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                    PatchList = (List<XmlPatch>)reader.Deserialize(file);
                    file.Close();
                    if (PatchList.Count > 0)
                    {
                        Logger("Description: " + PatchList[0].Description);
                        string[] OsList = PatchList[0].CompatibleOS.Split(',');
                        string CompOS = "";
                        foreach (string OS in OsList)
                        {
                            if (CompOS != "")
                                CompOS += ",";
                            string[] Parts = OS.Split(':');
                            CompOS += Parts[0];
                        }
                        Logger("For OS: " + CompOS);
                    }
                    RefreshDatagrid();
                    Logger("[OK]");
                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadPatch();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePatch("");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDatagrid();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            PatchList = new List<XmlPatch>();
            RefreshDatagrid();

        }
    }

}
