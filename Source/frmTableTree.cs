using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        public void loadTree(List<TableData> tdList, frmTuner tuner)
        {
            this.tuner = tuner;
            this.tdList = tdList;
            setIconSize();
            treeView1.ImageList = imageList1;
            TreeNode tn1 = new TreeNode("1D");
            tn1.ImageKey = "1d.ico";
            tn1.SelectedImageKey = "1d.ico";
            TreeNode tn2 = new TreeNode("2D");
            tn2.ImageKey = "2d.ico";
            tn1.SelectedImageKey = "2d.ico";
            TreeNode tn3 = new TreeNode("3D");
            tn3.ImageKey = "3d.ico";
            tn1.SelectedImageKey = "3d.ico";

            for (int i = 0; i < tdList.Count; i++)
            {
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;

                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    tnChild.ImageKey = "mask.ico";
                    tnChild.SelectedImageKey = "mask.ico";
                }
                else if (tdList[i].OutputType == upatcher.OutDataType.Flag || (tdList[i].Units != null && tdList[i].Units.ToLower().Contains("boolean")))
                {
                    tnChild.ImageKey = "flag.ico";
                    tnChild.SelectedImageKey = "flag.ico";
                }
                else if (tdList[i].Values.StartsWith("Enum:"))
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
                    tn1.Nodes.Add(tnChild);
                }
                else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                {
                    tn2.Nodes.Add(tnChild);
                }
                else
                {
                    tn3.Nodes.Add(tnChild);
                }
            }

            TreeNode[] tnArray = new TreeNode[] { tn1, tn2, tn3 };
            TreeNode tn = new TreeNode("Dimensions", tnArray);
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(tn);

            tn1 = new TreeNode("number");
            tn1.ImageKey = "Num.ico";
            tn1.SelectedImageKey = "Num.ico";
            tn2 = new TreeNode("enum");
            tn2.ImageKey = "enum.ico";
            tn2.SelectedImageKey = "enum.ico";
            tn3 = new TreeNode("bitmask");
            tn3.ImageKey = "mask.ico";
            tn3.SelectedImageKey = "mask.ico";
            TreeNode tn4 = new TreeNode("boolean");
            tn4.ImageKey = "flag.ico";
            tn4.SelectedImageKey = "flag.ico";
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

                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    tn3.Nodes.Add(tnChild);
                }
                else if (tdList[i].OutputType == upatcher.OutDataType.Flag || (tdList[i].Units != null && tdList[i].Units.ToLower().Contains("boolean")))
                {
                    tn4.Nodes.Add(tnChild);
                }
                else if (tdList[i].Values.StartsWith("Enum:"))
                {
                    tn2.Nodes.Add(tnChild);
                }
                else
                {
                    tn1.Nodes.Add(tnChild);
                }
            }

            tnArray = new TreeNode[] { tn1, tn2, tn3, tn4 };
            tn = new TreeNode("Value type", tnArray);
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            treeView1.Nodes.Add(tn);

            List<string> catList = new List<string>();
            List<TreeNode> tnList = new List<TreeNode>();
            for (int i=0; i< tdList.Count; i++)
            {
                string cat = tdList[i].Category;
                TreeNode tnCat;
                int ind = catList.IndexOf(cat);
                if (ind < 0)
                {
                    tnCat = new TreeNode(cat);
                    tnList.Add(tnCat);
                    catList.Add(cat);
                }
                else
                {
                    tnCat = tnList[ind];
                }
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;
                string ico = "";
                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    ico = "mask";
                }
                else if (tdList[i].OutputType == upatcher.OutDataType.Flag || (tdList[i].Units != null && tdList[i].Units.ToLower().Contains("boolean")))
                {
                    ico = "flag";
                }
                else if (tdList[i].Values.StartsWith("Enum:"))
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

                tnCat.Nodes.Add(tnChild);
            }
            tn = new TreeNode("Category");
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            for (int c = 0; c < tnList.Count; c++)
                tn.Nodes.Add(tnList[c]);
            treeView1.Nodes.Add(tn);
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
