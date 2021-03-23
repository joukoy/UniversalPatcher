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
            TreeNode tn1 = new TreeNode("1d");
            TreeNode tn2 = new TreeNode("2d");
            TreeNode tn3 = new TreeNode("3d");
            for (int i = 0; i < tdList.Count; i++)
            {
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;
                if (tdList[i].Rows == 1 && tdList[i].Columns == 1)
                    tn1.Nodes.Add(tnChild);
                else if (tdList[i].Rows > 1 && tdList[i].Columns == 1)
                    tn2.Nodes.Add(tnChild);
                else 
                    tn3.Nodes.Add(tnChild);
            }

            TreeNode[] tnArray = new TreeNode[] { tn1, tn2, tn3 };
            TreeNode tn = new TreeNode("Dimensions", tnArray);
            treeView1.Nodes.Add(tn);

            tn1 = new TreeNode("number");
            tn2 = new TreeNode("enum");
            tn3 = new TreeNode("bitmask");
            TreeNode tn4 = new TreeNode("boolean");
            for (int i = 0; i < tdList.Count; i++)
            {
                TreeNode tnChild = new TreeNode(tdList[i].TableName);
                tnChild.Tag = i;
                if (tdList[i].OutputType == upatcher.OutDataType.Flag)
                    tn4.Nodes.Add(tnChild);
                else if (tdList[i].Values.StartsWith("Enum:"))
                    tn2.Nodes.Add(tnChild);
                else if (tdList[i].BitMask != null && tdList[i].BitMask.Length > 0)
                    tn3.Nodes.Add(tnChild);
                else
                    tn1.Nodes.Add(tnChild);
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
