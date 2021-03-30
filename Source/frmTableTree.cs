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
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmTableTree : Form
    {
        public frmTableTree()
        {
            InitializeComponent();
        }

        public int selectedId;
        private List<TableData> tdList;
        private frmTuner tuner;
        int keyDelayCounter = 0;

        private void frmTableTree_Load(object sender, EventArgs e)
        {
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            treeView1.BeforeExpand += TreeView1_BeforeExpand;
            treeView1.AfterExpand += TreeView1_AfterExpand;
            
            if (Properties.Settings.Default.TableExplorerFont != null)
                treeView1.Font = Properties.Settings.Default.TableExplorerFont;

            useCategorySubfolderToolStripMenuItem.Checked = Properties.Settings.Default.TableExplorerUseCategorySubfolder;

            numIconSize.Value = Properties.Settings.Default.TableExplorerIconSize;

            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.TableExplorerWindowSize.Width > 0 || Properties.Settings.Default.TableExplorerWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.TableExplorerWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.TableExplorerWindowPosition;
                    this.Size = Properties.Settings.Default.TableExplorerWindowSize;
                }

            }

            this.FormClosing += FrmTableTree_FormClosing;
        }

        private void TreeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
                return;
            if (e.Node.Text == "Dimensions")
                if (filterNode("Dimensions"))
                    loadDimensions(treeView1.Nodes["Dimensions"]);
            if (e.Node.Text == "Value type")
                if (filterNode("Value type"))
                    loadValueTypes(treeView1.Nodes["Value type"]);
            if (e.Node.Text == "Category")
                if (filterNode("Category"))
                    loadCategories(treeView1.Nodes["Category"]);
            if (e.Node.Text == "Segments")
                if (filterNode("Segments"))
                    loadSegments(treeView1.Nodes["Segments"]);
        }

        private void FrmTableTree_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.TableExplorerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.TableExplorerWindowPosition = this.Location;
                    Properties.Settings.Default.TableExplorerWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.TableExplorerWindowPosition = this.RestoreBounds.Location;
                    Properties.Settings.Default.TableExplorerWindowSize = this.RestoreBounds.Size;
                }
            }
        }

        private TreeNode createTreeNode(string txt)
        {
            TreeNode tn = new TreeNode(txt);
            tn.Name = txt;
            tn.ImageKey = txt + ".ico";
            tn.SelectedImageKey = txt + ".ico";
            return tn;
        }

        private void loadDimensions(TreeNode parent)
        {
            for (int i = 0; i < tdList.Count; i++)
            {
                if (tdList[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {
                    TreeNode tnChild = new TreeNode(tdList[i].TableName);
                    tnChild.Tag = i;

                    TableValueType vt = getValueType(tdList[i]);
                    if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                    {
                        tnChild.ImageKey = "bitmask.ico";
                        tnChild.SelectedImageKey = "bitmask.ico";
                    }
                    else if (vt == TableValueType.boolean)
                    {
                        tnChild.ImageKey = "boolean.ico";
                        tnChild.SelectedImageKey = "boolean.ico";
                    }
                    else if (vt == TableValueType.selection)
                    {
                        tnChild.ImageKey = "enum.ico";
                        tnChild.SelectedImageKey = "enum.ico";
                    }
                    else
                    {
                        tnChild.ImageKey = "number.ico";
                        tnChild.SelectedImageKey = "number.ico";
                    }

                    string nodeKey = "";
                    if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                        nodeKey = "1D";
                    else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                        nodeKey = "2D";
                    else
                        nodeKey = "3D";

                    if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                    {
                        parent.Nodes[nodeKey].Nodes.Add(tnChild);
                    }
                    else
                    {
                        string cat = tdList[i].Category;
                        if (cat == "")
                            cat = "(Empty)";
                        if (!parent.Nodes[nodeKey].Nodes.ContainsKey(cat))
                        {
                            TreeNode dimCatTn = new TreeNode(cat);
                            dimCatTn.Name = cat;
                            parent.Nodes[nodeKey].Nodes.Add(dimCatTn);
                        }
                        parent.Nodes[nodeKey].Nodes[cat].Nodes.Add(tnChild);
                    }
                }
            }

        }

        private void loadValueTypes(TreeNode parent)
        {
            for (int i = 0; i < tdList.Count; i++)
            {
                if (tdList[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {
                    TreeNode tnChild = new TreeNode(tdList[i].TableName);
                    tnChild.Tag = i;

                    if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                    {
                        tnChild.ImageKey = "1d.ico";
                        tnChild.SelectedImageKey = "1d.ico";
                    }
                    else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                    {
                        tnChild.ImageKey = "2d.ico";
                        tnChild.SelectedImageKey = "2d.ico";
                    }
                    else
                    {
                        tnChild.ImageKey = "3d.ico";
                        tnChild.SelectedImageKey = "3d.ico";
                    }

                    TableValueType vt = getValueType(tdList[i]);
                    string nodeKey = "";
                    if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                        nodeKey = "bitmask";
                    else if (vt == TableValueType.boolean)
                        nodeKey = "boolean";
                    else if (vt == TableValueType.selection)
                        nodeKey = "enum";
                    else
                        nodeKey = "number";

                    if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                    {
                        parent.Nodes[nodeKey].Nodes.Add(tnChild);
                    }
                    else
                    {
                        string cat = tdList[i].Category;
                        if (cat == "")
                            cat = "(Empty)";
                        if (!parent.Nodes[nodeKey].Nodes.ContainsKey(cat))
                        {
                            TreeNode vtCatTn = new TreeNode(cat);
                            vtCatTn.Name = cat;
                            parent.Nodes[nodeKey].Nodes.Add(vtCatTn);
                        }
                        parent.Nodes[nodeKey].Nodes[cat].Nodes.Add(tnChild);
                    }
                }
            }

        }

        private void loadCategories(TreeNode parent)
        {
            for (int i = 0; i < tdList.Count; i++)
            {
                if (tdList[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {

                    TreeNode tnChild = new TreeNode(tdList[i].TableName);
                    tnChild.Tag = i;
                    string ico = "";
                    TableValueType vt = getValueType(tdList[i]);
                    if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                    {
                        ico = "mask";
                    }
                    else if (vt == TableValueType.boolean)
                    {
                        ico = "flag";
                    }
                    else if (vt == TableValueType.selection)
                    {
                        ico = "enum";
                    }

                    if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                    {
                        ico += "1d.ico";
                    }
                    else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                    {
                        ico += "2d.ico";
                    }
                    else
                    {
                        ico += "3d.ico";
                    }

                    tnChild.ImageKey = ico;
                    tnChild.SelectedImageKey = ico;

                    string cat = tdList[i].Category;
                    if (cat == "")
                        cat = "(Empty)";
                    parent.Nodes[cat].Nodes.Add(tnChild);

                }
            }

        }

        private void loadSegments(TreeNode parent)
        {
            for (int i = 0; i < tdList.Count; i++)
            {
                if (tdList[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {

                    TreeNode tnChild = new TreeNode(tdList[i].TableName);
                    tnChild.Tag = i;
                    string ico = "";
                    TableValueType vt = getValueType(tdList[i]);
                    if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                    {
                        ico = "mask";
                    }
                    else if (vt == TableValueType.boolean)
                    {
                        ico = "flag";
                    }
                    else if (vt == TableValueType.selection)
                    {
                        ico = "enum";
                    }

                    if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                    {
                        ico += "1d.ico";
                    }
                    else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                    {
                        ico += "2d.ico";
                    }
                    else
                    {
                        ico += "3d.ico";
                    }

                    tnChild.ImageKey = ico;
                    tnChild.SelectedImageKey = ico;

                    string cat = tdList[i].Category;
                    if (cat == "")
                        cat = "(Empty)";

                    int seg = tuner.PCM.GetSegmentNumber(tdList[i].addrInt);
                    if (seg > -1)
                    {
                        if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                        {
                            parent.Nodes[seg].Nodes.Add(tnChild);
                        }
                        else
                        {
                            if (!parent.Nodes[seg].Nodes.ContainsKey(cat))
                            {
                                TreeNode tnNew = new TreeNode(cat);
                                tnNew.Name = cat;
                                parent.Nodes[seg].Nodes.Add(tnNew);
                            }
                            parent.Nodes[seg].Nodes[cat].Nodes.Add(tnChild);
                        }
                    }
                }
            }            
        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0].Nodes.Count > 0)
                return; //already loaded
            if (e.Node.Parent != null)
                return; //Not top-level node
            if (e.Node.Text == "Dimensions")
                loadDimensions(e.Node);
            else if (e.Node.Text == "Value type")
                loadValueTypes(e.Node);
            else if (e.Node.Text == "Category")
                loadCategories(e.Node);
            else if (e.Node.Text == "Segments")
                loadSegments(e.Node);
        }

        public void loadTree(List<TableData> tdList, frmTuner tuner)
        {
            this.tuner = tuner;
            this.tdList = tdList;
            setIconSize();
            treeView1.ImageList = imageList1;
            treeView1.Nodes.Clear();
            TreeNode tn = new TreeNode("Dimensions");
            tn.Name = "Dimensions";
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(tn);

            tn.Nodes.Add(createTreeNode("1D"));
            tn.Nodes.Add(createTreeNode("2D"));
            tn.Nodes.Add(createTreeNode("3D"));


            tn = new TreeNode("Value type");
            tn.Name = "Value type";
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(tn);

            tn.Nodes.Add(createTreeNode("number"));
            tn.Nodes.Add(createTreeNode("enum"));
            tn.Nodes.Add(createTreeNode("bitmask"));
            tn.Nodes.Add(createTreeNode("boolean"));

            TreeNode cTn = new TreeNode("Category");
            cTn.Name = "Category";
            cTn.ImageKey = "explorer.ico";
            cTn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(cTn);
            TreeNode cTnChild = new TreeNode();
            for (int i=0; i< tuner.PCM.tableCategories.Count; i++)
            {
                string cate = tuner.PCM.tableCategories[i];
                if (cate != "_All")
                {
                    if (cate == "")
                        cate = "(Empty)";
                    cTnChild = new TreeNode(cate);
                    cTnChild.Name = cate;
                    cTnChild.ImageKey = "explorer.ico";
                    cTnChild.SelectedImageKey = "explorer.ico";
                    cTn.Nodes.Add(cTnChild);
                }
            }

            TreeNode sTn = new TreeNode("Segments");
            sTn.Name = "Segments";
            sTn.ImageKey = "explorer.ico";
            sTn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(sTn);

            TreeNode segTn;
            for (int i = 0; i < tuner.PCM.Segments.Count;i++)
            {
                segTn = new TreeNode(tuner.PCM.Segments[i].Name);
                segTn.Name = tuner.PCM.Segments[i].Name;
                sTn.Nodes.Add(segTn);
            }


        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                return;
            tuner.showTableDescription((int)e.Node.Tag);
        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (e.Node.Tag != null)
            {
                selectedId = (int)e.Node.Tag;
                tuner.openTableEditor(selectedId);
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.Font = treeView1.Font;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                treeView1.Font = fontDlg.Font;
                Properties.Settings.Default.TableExplorerFont = treeView1.Font;
                Properties.Settings.Default.Save();
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void setIconSize()
        {
            int iconSize = (int)numIconSize.Value;
            Properties.Settings.Default.TableExplorerIconSize = iconSize;
            imageList1.ImageSize = new Size(iconSize,iconSize);
            imageList1.Images.Clear();
            string iconFolder = Path.Combine(Application.StartupPath, "Icons");
            string folderIcon = Path.Combine(Application.StartupPath, "Icons", "explorer.ico");
            imageList1.Images.Add(Image.FromFile(folderIcon));
            string[] GalleryArray = System.IO.Directory.GetFiles(iconFolder);
            for (int i=0; i< GalleryArray.Length;i++)
            {
                if (GalleryArray[i].ToLower().EndsWith(".ico"))
                {
                    
                    imageList1.Images.Add(Path.GetFileName(GalleryArray[i]), Icon.ExtractAssociatedIcon(GalleryArray[i]));
                }
            }
            treeView1.ItemHeight = iconSize + 2;
            treeView1.Indent = iconSize + 4;
        }
        private void numIconSize_ValueChanged(object sender, EventArgs e)
        {
            setIconSize();
        }

        private void useCategorySubfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useCategorySubfolderToolStripMenuItem.Checked = !useCategorySubfolderToolStripMenuItem.Checked;
            Properties.Settings.Default.TableExplorerUseCategorySubfolder = useCategorySubfolderToolStripMenuItem.Checked;
            filterTables();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            keyDelayCounter = 0;
            timerFilter.Enabled = true;
        }

        private bool filterNode(string nodeKey)
        {
            bool isVisible = false;
            if (treeView1.Nodes[nodeKey].IsExpanded)
            {
                isVisible = true;
                foreach (TreeNode tn in treeView1.Nodes[nodeKey].Nodes)
                {
                    while (tn.Nodes.Count > 0)
                        tn.Nodes[0].Remove();
                }
            }
            return isVisible;
        }

        private void filterTables()
        {
            if(filterNode("Dimensions"))
                loadDimensions(treeView1.Nodes["Dimensions"]);
            if (filterNode("Value type"))
                loadValueTypes(treeView1.Nodes["Value type"]);
            if (filterNode("Category"))
                loadCategories(treeView1.Nodes["Category"]);
            if (filterNode("Segments"))
                loadSegments(treeView1.Nodes["Segments"]);
        }

        private void timerFilter_Tick(object sender, EventArgs e)
        {
            keyDelayCounter++;
            if (keyDelayCounter > Properties.Settings.Default.keyPressWait100ms)
            {
                //loadTree(tdList,tuner);
                filterTables();
                timerFilter.Enabled = false;
            }

        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }
    }
}
