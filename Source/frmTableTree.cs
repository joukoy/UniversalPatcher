using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private frmTuner tuner;

        public void loadTree(List<TableData> tdList, frmTuner tuner)
        {
            this.tuner = tuner;
            treeView1.ImageList = imageList1;
            TreeNode tn1 = new TreeNode("1D");
            tn1.ImageKey = "1d.png";
            tn1.SelectedImageKey = "1d.png";
            TreeNode tn2 = new TreeNode("2D");
            tn2.ImageKey = "2d.png";
            tn1.SelectedImageKey = "2d.png";
            TreeNode tn3 = new TreeNode("3D");
            tn3.ImageKey = "3d.png";
            tn1.SelectedImageKey = "3d.png";

            for (int i = 0; i < tdList.Count; i++)
            {
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;

                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    tnChild.ImageKey = "mask.png";
                    tnChild.SelectedImageKey = "mask.png";
                }
                else if (tdList[i].OutputType == upatcher.OutDataType.Flag)
                {
                    tnChild.ImageKey = "flag.png";
                    tnChild.SelectedImageKey = "flag.png";
                }
                else if (tdList[i].Values.StartsWith("Enum:"))
                {
                    tnChild.ImageKey = "enum.png";
                    tnChild.SelectedImageKey = "enum.png";
                }
                else
                {
                    tnChild.ImageKey = "Num.png";
                    tnChild.SelectedImageKey = "Num.png";
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
            treeView1.Nodes.Add(tn);

            tn1 = new TreeNode("number");
            tn1.ImageKey = "Num.png";
            tn1.SelectedImageKey = "Num.png";
            tn2 = new TreeNode("enum");
            tn2.ImageKey = "enum.png";
            tn2.SelectedImageKey = "enum.png";
            tn3 = new TreeNode("bitmask");
            tn3.ImageKey = "mask.png";
            tn3.SelectedImageKey = "mask.png";
            TreeNode tn4 = new TreeNode("boolean");
            tn4.ImageKey = "flag.png";
            tn4.SelectedImageKey = "flag.png";
            for (int i = 0; i < tdList.Count; i++)
            {
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;

                if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                {
                    tnChild.ImageKey = "1d.png";
                    tnChild.SelectedImageKey = "1d.png";
                }
                else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                {
                    tnChild.ImageKey = "2d.png";
                    tnChild.SelectedImageKey = "2d.png";
                }
                else
                {
                    tnChild.ImageKey = "3d.png";
                    tnChild.SelectedImageKey = "3d.png";
                }

                if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                {
                    tn3.Nodes.Add(tnChild);
                }
                else if (tdList[i].OutputType == upatcher.OutDataType.Flag)
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
                if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                {
                    tnChild.ImageKey = "1d.png";
                    tnChild.SelectedImageKey = "1d.png";
                }
                else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                {
                    tnChild.ImageKey = "2d.png";
                    tnChild.SelectedImageKey = "2d.png";
                }
                else
                {
                    tnChild.ImageKey = "3d.png";
                    tnChild.SelectedImageKey = "3d.png";
                }
                tnCat.Nodes.Add(tnChild);
            }
            tn = new TreeNode("Category");
            for (int c = 0; c < tnList.Count; c++)
                tn.Nodes.Add(tnList[c]);
            treeView1.Nodes.Add(tn);
        }
        private void frmTableTree_Load(object sender, EventArgs e)
        {
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;
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
    }
}
