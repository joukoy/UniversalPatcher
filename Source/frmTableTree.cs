using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private void frmTableTree_Load(object sender, EventArgs e)
        {
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;

            if (Properties.Settings.Default.TableExplorerFont != null)
                treeView1.Font = Properties.Settings.Default.TableExplorerFont;

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

        public void loadTree(List<TableData> tdList, frmTuner tuner)
        {
            this.tuner = tuner;
            this.tdList = tdList;
            setIconSize();
            treeView1.ImageList = imageList1;

            TreeNode tn = new TreeNode("Dimensions");
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(tn);

            tn.Nodes.Add(createTreeNode("1D"));
            tn.Nodes.Add(createTreeNode("2D"));
            tn.Nodes.Add(createTreeNode("3D"));

            for (int i = 0; i < tdList.Count; i++)
            {
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;

                TableValueType vt = getValueType(tdList[i]);
                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    tnChild.ImageKey = "mask.ico";
                    tnChild.SelectedImageKey = "mask.ico";
                }
                else if (vt == TableValueType.boolean)
                {
                    tnChild.ImageKey = "flag.ico";
                    tnChild.SelectedImageKey = "flag.ico";
                }
                else if (vt == TableValueType.selection)
                {
                    tnChild.ImageKey = "enum.ico";
                    tnChild.SelectedImageKey = "enum.ico";
                }
                else
                {
                    tnChild.ImageKey = "Num.ico";
                    tnChild.SelectedImageKey = "Num.ico";
                }


                if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                {
                    tn.Nodes["1D"].Nodes.Add(tnChild);
                }
                else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                {
                    tn.Nodes["2D"].Nodes.Add(tnChild);
                }
                else
                {
                    tn.Nodes["3D"].Nodes.Add(tnChild);
                }
            }

            tn = new TreeNode("Value type");
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(tn);

            tn.Nodes.Add(createTreeNode("number"));
            tn.Nodes.Add(createTreeNode("enum"));
            tn.Nodes.Add(createTreeNode("bitmask"));
            tn.Nodes.Add(createTreeNode("boolean"));

            for (int i = 0; i < tdList.Count; i++)
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
                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    tn.Nodes["bitmask"].Nodes.Add(tnChild);
                }
                else if (vt == TableValueType.boolean)
                {
                    tn.Nodes["boolean"].Nodes.Add(tnChild);
                }
                else if (vt == TableValueType.selection)
                {
                    tn.Nodes["enum"].Nodes.Add(tnChild);
                }
                else
                {
                    tn.Nodes["number"].Nodes.Add(tnChild);
                }
            }


            TreeNode cTn = new TreeNode("Category");
            cTn.ImageKey = "explorer.ico";
            cTn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(cTn);
            TreeNode cTnChild;
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

            for (int i=0; i< tdList.Count; i++)
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
                cTn.Nodes[cat].Nodes.Add(tnChild);

                int seg = tuner.PCM.GetSegmentNumber(tdList[i].addrInt);
                if (seg > -1)
                {
                    TreeNode tnClone = (TreeNode)tnChild.Clone();
                    if (!sTn.Nodes[seg].Nodes.ContainsKey(cat))
                    {
                        TreeNode tnNew = new TreeNode(cat);
                        tnNew.Name = cat;
                        sTn.Nodes[seg].Nodes.Add(tnNew);
                    }
                    sTn.Nodes[seg].Nodes[cat].Nodes.Add(tnClone);
                }
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

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
    }
}
