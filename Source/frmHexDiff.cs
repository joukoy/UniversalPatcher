using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static UniversalPatcher.ExtensionMethods;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public partial class frmHexDiff : Form
    {
        public frmHexDiff(PcmFile _pcm1, PcmFile _pcm2, List<TableData> _tdList, List<TableData> _tdList2)
        {
            InitializeComponent();
            DrawingControl.SetDoubleBuffered(dataGridView1);
            pcm1 = _pcm1;
            pcm2 = _pcm2;
            tdList = _tdList;
            tdList2 = _tdList2;
        }

        TreeViewMS tree1;
        List<TableData> filteredTableDatas = new List<TableData>();
        private List<TableDiff> tdiffList;
        private PcmFile pcm1;
        private PcmFile pcm2;
        private List<TableData> tdList;
        private List<TableData> tdList2;

        SortOrder strSortOrder = SortOrder.Ascending;
        private int sortIndex = 0;
        private string sortBy = "id";
        BindingSource bindingSource = new BindingSource();


        private void frmHexDiff_Load(object sender, EventArgs e)
        {

            imageList1.Images.Clear();
            string folderIcon = Path.Combine(Application.StartupPath, "Icons", "explorer.ico");
            imageList1.Images.Add(Image.FromFile(folderIcon));
            string iconFolder = Path.Combine(Application.StartupPath, "Icons");
            string[] GalleryArray = System.IO.Directory.GetFiles(iconFolder);
            for (int i = 0; i < GalleryArray.Length; i++)
            {
                if (GalleryArray[i].ToLower().EndsWith(".ico"))
                    imageList1.Images.Add(Path.GetFileName(GalleryArray[i]), Icon.ExtractAssociatedIcon(GalleryArray[i]));
            }

            tree1 = new TreeViewMS();
            splitContainer1.Panel1.Controls.Add(tree1);
            tree1.Dock = DockStyle.Fill;
            tree1.ItemHeight = 18;
            tree1.Indent = 20;
            tree1.HideSelection = false;
            tree1.ImageList = imageList1;

        }
        private class TableDiff
        {
            public Guid id { get; set; }
            public Guid id2 { get; set; }
            public string addr1 { get; set; }
            public string addr2 { get; set; }
            public string TableName { get; set; }
            public string Data1 { get; set; }
            public string Data2 { get; set; }
            public TableData td;
            public TableData td2;
        }

        public void FindDifferences(bool showAsHex)
        {
            try
            {
                if (!showAsHex)
                    this.Text = "File differences";
                tdiffList = new List<TableDiff>();
                labelFileNames.Text = pcm1.FileName + " <> " + pcm2.FileName;

                for (int t = 0; t < tdList.Count; t++)
                {
                    TableData td = tdList[t];
                    TableData td2 = tdList2[t];
                    uint step = (uint)GetElementSize(td.DataType);
                    uint step2 = (uint)GetElementSize(td2.DataType);
                    int count = td.Rows * td.Columns;
                    uint addr = (uint)(td.addrInt + td.Offset);
                    uint addr2 = (uint)(td2.addrInt + td2.Offset);
                    string data1 = "";
                    string data2 = "";
                    string formatStr = "";
                    if (showAsHex)
                        formatStr = "X" + (step * 2).ToString();
                    for (int a = 0; a < count; a++)
                    {
                        if (showAsHex)
                        {
                            data1 += ((UInt64)GetRawValue(pcm1.buf, addr, td, 0,pcm1.platformConfig.MSB)).ToString(formatStr) + " ";
                            data2 += ((UInt64)GetRawValue(pcm2.buf, addr2, td2, 0, pcm2.platformConfig.MSB)).ToString(formatStr) + " ";
                        }
                        else
                        {
                            data1 += GetValue(pcm1.buf, addr, td, 0, pcm1).ToString(formatStr) + " ";
                            data2 += GetValue(pcm2.buf, addr2, td2, 0, pcm2).ToString(formatStr) + " ";
                        }
                        addr += step;
                        addr2 += step2;
                    }

                    TableDiff tDiff = new TableDiff();
                    tDiff.Data1 = data1.Trim();
                    tDiff.Data2 = data2.Trim();
                    tDiff.id = tdList[t].guid;
                    tDiff.TableName = td.TableName;
                    tDiff.td = td;
                    tDiff.id2 = tdList2[t].guid;
                    tDiff.addr1 = td.Address;
                    tDiff.addr2 = td2.Address;
                    tDiff.td2 = td2;
                    tdiffList.Add(tDiff);
                }
                dataGridView1.DataSource = bindingSource;
                bindingSource.DataSource = tdiffList;
                dataGridView1.CellMouseDoubleClick += DataGridView1_CellMouseDoubleClick;
                dataGridView1.ColumnHeaderMouseClick += DataGridView1_ColumnHeaderMouseClick;
                FilterTables();
                tree1.SelectedNodes.Clear();
                TreeParts.AddNodes(tree1.Nodes, pcm1);
                tree1.AfterSelect += Tree1_AfterSelect;
                tree1.ContextMenuStrip = contextMenuStrip1;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHexDiff line " + line + ": " + ex.Message);
            }
        }

        private void Tree1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FilterTables();
            if (e.Node.Nodes.Count == 0 && e.Node.Name != "All" && e.Node.Parent != null)
                TreeParts.AddChildNodes(e.Node, pcm1);
        }

        private void FilterTables()
        {
            try
            {
                List<TableDiff> compareList = new List<TableDiff>();
                if (strSortOrder == SortOrder.Ascending)
                    compareList = tdiffList.OrderBy(x => typeof(TableDiff).GetProperty(sortBy).GetValue(x, null)).ToList();
                else
                    compareList = tdiffList.OrderByDescending(x => typeof(TableDiff).GetProperty(sortBy).GetValue(x, null)).ToList();
                var results = compareList.Where(t => t.TableName.ToString().ToLower().Contains(txtFilter.Text.Trim()));

                if (tree1.Nodes.Count > 0 && !tree1.Nodes["All"].IsSelected && tree1.SelectedNodes.Count > 0)
                {
                    List<string> selectedSegs = new List<string>();
                    List<string> selectedCats = new List<string>();
                    List<string> selectedValTypes = new List<string>();
                    List<string> selectedDimensions = new List<string>();
                    foreach (TreeNode tn in tree1.SelectedNodes)
                    {
                        if (tn.Parent == null)
                            continue;
                        switch (tn.Parent.Name)
                        {
                            case "Segments":
                                selectedSegs.Add(tn.Name);
                                break;
                            case "Categories":
                                selectedCats.Add(tn.Name);
                                break;
                            case "Dimensions":
                                selectedDimensions.Add(tn.Name);
                                break;
                            case "ValueTypes":
                                selectedValTypes.Add(tn.Name);
                                break;
                        }
                        TreeNode tnParent = tn.Parent;
                        while (tnParent.Parent != null)
                        {
                            switch (tnParent.Parent.Name)
                            {
                                case "Segments":
                                    selectedSegs.Add(tnParent.Name);
                                    break;
                                case "Categories":
                                    selectedCats.Add(tnParent.Name);
                                    break;
                                case "Dimensions":
                                    selectedDimensions.Add(tnParent.Name);
                                    break;
                                case "ValueTypes":
                                    selectedValTypes.Add(tnParent.Name);
                                    break;
                            }
                            tnParent = tnParent.Parent;
                        }

                    }

                    if (selectedSegs.Count > 0)
                    {
                        List<TableDiff> newTDList = new List<TableDiff>();
                        foreach (string seg in selectedSegs)
                        {
                            int segNr = 0;
                            for (int s = 0; s < pcm1.segmentinfos.Length; s++)
                                if (pcm1.segmentinfos[s].Name == seg)
                                    segNr = s;
                            uint addrStart = pcm1.segmentAddressDatas[segNr].SegmentBlocks[0].Start;
                            uint addrEnd = pcm1.segmentAddressDatas[segNr].SegmentBlocks[pcm1.segmentAddressDatas[segNr].SegmentBlocks.Count - 1].End;
                            var newResults = results.Where(t => t.td.addrInt >= addrStart && t.td.addrInt <= addrEnd);
                            foreach (TableDiff nTd in newResults)
                                newTDList.Add(nTd);
                        }
                        results = newTDList;
                    }

                    if (selectedCats.Count > 0)
                    {
                        List<TableDiff> newTDList = new List<TableDiff>();
                        foreach (TableDiff tDif in results)
                        {
                            if (selectedCats.Contains(tDif.td.Category))
                                newTDList.Add(tDif);
                        }
                        results = newTDList;
                    }

                    if (selectedValTypes.Count > 0)
                    {
                        List<TableDiff> newTDList = new List<TableDiff>();
                        foreach (string valT in selectedValTypes)
                        {
                            foreach (TableDiff tDif in results)
                            {
                                string tdValT = GetTableValueType(tDif.td).ToString();
                                if (tdValT == valT)
                                    newTDList.Add(tDif);
                            }
                        }
                        results = newTDList;
                    }

                    if (selectedDimensions.Count > 0)
                    {
                        List<TableDiff> newTDList = new List<TableDiff>();
                        foreach (TableDiff tDif in results)
                        {
                            if (selectedDimensions.Contains(tDif.td.Dimensions().ToString() + "D"))
                                newTDList.Add(tDif);
                        }
                        results = newTDList;
                    }
                }
                bindingSource.DataSource = results;
                Application.DoEvents();
                if (dataGridView1.Columns.Count > sortIndex)
                    dataGridView1.Columns[sortIndex].HeaderCell.SortGlyphDirection = strSortOrder;

                filteredTableDatas = new List<TableData>();
                foreach (TableDiff tDif in results)
                    filteredTableDatas.Add(tDif.td);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHexDiff line " + line + ": " + ex.Message);
            }

        }

        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //saveGridLayout(); //Save before reorder!
                sortBy = dataGridView1.Columns[e.ColumnIndex].Name;
                sortIndex = e.ColumnIndex;
                strSortOrder = GetSortOrder(sortIndex);
                FilterTables();
            }
        }

        private SortOrder GetSortOrder(int columnIndex)
        {
            try
            {
                if (dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.Descending
                    || dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection == SortOrder.None)
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    return SortOrder.Ascending;
                }
                else
                {
                    dataGridView1.Columns[columnIndex].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    return SortOrder.Descending;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return SortOrder.Ascending;
        }

        private void DataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1)
                {
                    TableData td = ((TableDiff)dataGridView1.Rows[e.RowIndex].DataBoundItem).td;
                    TableData td2 = ((TableDiff)dataGridView1.Rows[e.RowIndex].DataBoundItem).td2;
                    frmTableEditor frmT = new frmTableEditor();
                    List<TableData> tableIds = new List<TableData>();
                    tableIds.Add(td);
                    frmT.PrepareTable(pcm1, td, tableIds, "A");
                    frmT.AddCompareFiletoMenu(pcm2, null, "B:" + pcm2.FileName,"B");
                    frmT.Show();
                    frmT.LoadTable();
                    frmT.radioSideBySide.Checked = true;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHexDiff line " + line + ": " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void SaveCSV()
        {
            try
            {
                string FileName = SelectSaveFile("CSV files (*.csv)|*.csv|All files (*.*)|*.*");
                if (FileName.Length == 0)
                    return;
                Logger("Writing to file: " + Path.GetFileName(FileName), false);
                using (StreamWriter writetext = new StreamWriter(FileName))
                {
                    string row = "";
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        if (i > 0)
                            row += ";";
                        row += dataGridView1.Columns[i].HeaderText;
                    }
                    writetext.WriteLine(row);
                    for (int r = 0; r < (dataGridView1.Rows.Count - 1); r++)
                    {
                        row = "";
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            if (i > 0)
                                row += ";";
                            if (dataGridView1.Rows[r].Cells[i].Value != null)
                                row += dataGridView1.Rows[r].Cells[i].Value.ToString();
                        }
                        writetext.WriteLine(row);
                    }
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHexDiff line " + line + ": " + ex.Message);
            }
        }


        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            FilterTables();
        }

        private void btnSaveCsv_Click(object sender, EventArgs e)
        {
            SaveCSV();
        }

        private void btnSaveTableList_Click(object sender, EventArgs e)
        {
            try
            {
                string defaultFileName = Path.Combine(Application.StartupPath, "Tuner", Path.GetFileName(pcm1.FileName) + "-" + Path.GetFileName(pcm2.FileName) + ".XML");
                string fName = SelectSaveFile("XML files (*.xml)|*.xml|All files (*.*)|*.*", defaultFileName);
                if (fName.Length == 0)
                    return;

                List<TableData> tableDatas = new List<TableData>();
                for (int i = 0; i < tdList.Count; i++)
                    tableDatas.Add(tdList[i]);
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, tableDatas);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHexDiff line " + line + ": " + ex.Message);
            }
        }

        private void btnShowInTuner_Click(object sender, EventArgs e)
        {
            PcmFile pcmNew = pcm1.ShallowCopy();
            pcmNew.tableDatas = new List<TableData>();
            for (int i = 0; i < tdList.Count; i++)
                pcmNew.tableDatas.Add(tdList[i]);

            PcmFile pcmNew2 = pcm2.ShallowCopy();
            pcmNew2.tableDatas = new List<TableData>();
            for (int i = 0; i < tdList2.Count; i++)
                pcmNew2.tableDatas.Add(tdList2[i]);

            FrmTuner frmT = new FrmTuner(pcmNew,false);
            frmT.AddtoCurrentFileMenu(pcmNew2,false);
            frmT.Show();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = tree1.SelectedNode;
            tn.ExpandAll();
        }

        private void collapseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = tree1.SelectedNode;
            tn.Collapse();
        }

        private void expand2LevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = tree1.SelectedNode;
            foreach (TreeNode childTn in tn.Nodes)
                TreeParts.AddChildNodes(childTn, pcm1);
            tn.ExpandAll();

        }

        private void expand3LevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = tree1.SelectedNode;
            foreach (TreeNode childTn in tn.Nodes)
            {
                TreeParts.AddChildNodes(childTn, pcm1);
                foreach (TreeNode grandChild in childTn.Nodes)
                    TreeParts.AddChildNodes(grandChild,pcm1);
            }
            tn.ExpandAll();
        }

        private void btnCreatePatch_Click(object sender, EventArgs e)
        {
            GenerateTablePatch();
        }

        private void GenerateTablePatch()
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Patches", "newpatch.xmlpatch");
                string patchFname = SelectSaveFile("PATCH files (*.xmlpatch)|*.xmlpatch|ALL files (*.*)|*.*", defName);
                if (patchFname.Length == 0)
                    return;
                string Description = "";
                frmData frmD = new frmData();
                frmD.Text = "Patch Description";
                if (frmD.ShowDialog() == DialogResult.OK)
                    Description = frmD.txtData.Text;
                frmD.Dispose();
                List<XmlPatch> newPatch = new List<XmlPatch>();
                for (int i = 0; i < tdiffList.Count; i++)
                {
                    TableData pTd = tdiffList[i].td;
                    XmlPatch xpatch = new XmlPatch();
                    xpatch.CompatibleOS = "Table:" + pTd.TableName + ",columns:" + pTd.Columns.ToString() + ",rows:" + pTd.Rows.ToString();
                    xpatch.XmlFile = pcm1.configFile;
                    xpatch.Segment = pcm1.GetSegmentName(pTd.addrInt);
                    xpatch.Description = Description;
                    frmTableEditor frmTE = new frmTableEditor();
                    frmTE.PrepareTable(pcm1, pTd, null, "A");
                    frmTE.LoadTable();
                    uint step = (uint)GetElementSize(pTd.DataType);
                    uint addr = (uint)(pTd.addrInt + pTd.Offset);
                    if (pTd.RowMajor)
                    {
                        for (int r = 0; r < pTd.Rows; r++)
                        {
                            for (int c = 0; c < pTd.Columns; c++)
                            {
                                xpatch.Data += GetValue(pcm1.buf, addr, pTd, 0, pcm1).ToString().Replace(",", ".") + " ";
                                addr += step;
                            }
                        }
                    }
                    else
                    {
                        for (int c = 0; c < pTd.Columns; c++)
                        {
                            for (int r = 0; r < pTd.Rows; r++)
                            {
                                xpatch.Data += GetValue(pcm1.buf, addr, pTd, 0, pcm1).ToString().Replace(",", ".") + " ";
                                addr += step;
                            }
                        }
                    }
                    newPatch.Add(xpatch);
                }
                Logger("Saving to file: " + Path.GetFileName(patchFname), false);

                using (FileStream stream = new FileStream(patchFname, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                    writer.Serialize(stream, newPatch);
                    stream.Close();
                }
                Logger(" [OK]");

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmHexDiff line " + line + ": " + ex.Message);
            }
        }
    }
}
