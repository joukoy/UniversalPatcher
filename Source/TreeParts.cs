using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UniversalPatcher
{
    public static class TreeParts
    {
        private static bool includesCollection(TreeNode node, string nodeName)
        {
            if (node.Parent.Name == nodeName)
                return true;
            if (node.Parent.Parent == null)
                return false;   //Root-node
            return includesCollection(node.Parent, nodeName);
        }

        public static void addChildNodes(TreeNode node, PcmFile PCM)
        {
            if (!includesCollection(node, "Dimensions"))
                TreeParts.addDimensions(node.Nodes);
            if (!includesCollection(node, "ValueTypes"))
                TreeParts.addValueTypes(node.Nodes);
            if (!includesCollection(node, "Categories"))
                TreeParts.addCategories(node.Nodes, PCM);
            if (!includesCollection(node, "Segments"))
                TreeParts.addSegments(node.Nodes, PCM);

        }

        public static void addNodes(TreeNodeCollection parent, PcmFile pcm1)
        {
            parent.Clear();

            TreeNode tn = new TreeNode("All");
            tn.Name = "All";
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            parent.Add(tn);

            addDimensions(parent);
            addValueTypes(parent);
            addCategories(parent, pcm1);
            addSegments(parent, pcm1);

        }

        public static void addDimensions(TreeNodeCollection parent)
        {

            TreeNode tnD = new TreeNode();
            tnD.Name = "Dimensions";
            tnD.ImageKey = "dimensions.ico";
            tnD.SelectedImageKey = "dimensions.ico";
            parent.Add(tnD);

            TreeNode tn1 = new TreeNode();
            tn1.Name = "1D";
            tn1.ImageKey = "1d.ico";
            tn1.SelectedImageKey = "1d.ico";
            tnD.Nodes.Add(tn1);

            TreeNode tn2 = new TreeNode();
            tn2.Name = "2D";
            tn2.ImageKey = "2d.ico";
            tn2.SelectedImageKey = "2d.ico";
            tnD.Nodes.Add(tn2);

            TreeNode tn3 = new TreeNode();
            tn3.Name = "3D";
            tn3.ImageKey = "3d.ico";
            tn3.SelectedImageKey = "3d.ico";
            tnD.Nodes.Add(tn3);

        }
        public static void addValueTypes(TreeNodeCollection parent)
        {

            TreeNode tnT = new TreeNode();
            tnT.Name = "ValueTypes";
            tnT.ImageKey = "valuetype.ico";
            tnT.SelectedImageKey = "valuetype.ico";
            parent.Add(tnT);

            TreeNode tnB = new TreeNode();
            tnB.Name = "boolean";
            tnB.ImageKey = "boolean.ico";
            tnB.SelectedImageKey = "boolean.ico";
            tnT.Nodes.Add(tnB);

            TreeNode tnM = new TreeNode();
            tnM.Name = "mask";
            tnM.ImageKey = "bitmask.ico";
            tnM.SelectedImageKey = "bitmask.ico";
            tnT.Nodes.Add(tnM);

            TreeNode tnE = new TreeNode();
            tnE.Name = "selection";
            tnE.ImageKey = "enum.ico";
            tnE.SelectedImageKey = "enum.ico";
            tnT.Nodes.Add(tnE);

            TreeNode tnN = new TreeNode();
            tnN.Name = "number";
            tnN.ImageKey = "number.ico";
            tnN.SelectedImageKey = "number.ico";
            tnT.Nodes.Add(tnN);

        }
        public static void addSegments(TreeNodeCollection parent, PcmFile PCM)
        {
            string iconFolder = Path.Combine(Application.StartupPath, "Icons");
            string[] GalleryArray = System.IO.Directory.GetFiles(iconFolder);

            TreeNode tnS = new TreeNode();
            tnS.Name = "Segments";
            tnS.ImageKey = "segments.ico";
            tnS.SelectedImageKey = "segments.ico";
            parent.Add(tnS);

            TreeNode segTn;
            for (int i = 0; i < PCM.Segments.Count; i++)
            {
                segTn = new TreeNode(PCM.Segments[i].Name);
                segTn.Name = PCM.Segments[i].Name;
                segTn.ImageKey = "segments.ico";
                segTn.SelectedImageKey = "segments.ico";

                bool found = false;
                foreach (string icofile in GalleryArray)
                {
                    double percentage = ComputeSimilarity.CalculateSimilarity(Path.GetFileNameWithoutExtension(icofile).ToLower(), segTn.Name.ToLower());
                    if ((int)(percentage * 100) >= 80)
                    {
                        segTn.ImageKey = Path.GetFileName(icofile);
                        segTn.SelectedImageKey = Path.GetFileName(icofile);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    foreach (string icofile in GalleryArray)
                    {
                        if (segTn.Name.ToLower().Contains(Path.GetFileNameWithoutExtension(icofile)))
                        {
                            segTn.ImageKey = Path.GetFileName(icofile);
                            segTn.SelectedImageKey = Path.GetFileName(icofile);
                            found = true;
                            break;
                        }
                    }
                }
                tnS.Nodes.Add(segTn);
            }

        }
        public static void addCategories(TreeNodeCollection parent, PcmFile PCM)
        {
            TreeNode tnC = new TreeNode();
            tnC.Name = "Categories";
            tnC.ImageKey = "category.ico";
            tnC.SelectedImageKey = "category.ico";
            parent.Add(tnC);

            for (int c = 0; c < PCM.tableCategories.Count; c++)
            {
                string cat = PCM.tableCategories[c];
                if (cat != "_All")
                {
                    TreeNode cTnChild = new TreeNode(cat);
                    cTnChild.Name = cat;
                    cTnChild.ImageKey = "category.ico";
                    cTnChild.SelectedImageKey = "category.ico";
                    tnC.Nodes.Add(cTnChild);
                }
            }

        }

    }
}
