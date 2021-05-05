using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public static class TreeParts
    {
        private static bool includesCollection(TreeNode node, string nodeName)
        {
            if (node.Parent == null )
                return false;   //Root-node
            if (node.Parent.Name == nodeName)
                return true;
            return includesCollection(node.Parent, nodeName);
        }

        public static void addChildNodes(TreeNode node, PcmFile pcm, List<TableData> filteredTableDatas)
        {
            if (!includesCollection(node, "Dimensions"))
                TreeParts.addDimensions(node.Nodes,filteredTableDatas);
            if (!includesCollection(node, "ValueTypes"))
                TreeParts.addValueTypes(node.Nodes,filteredTableDatas);
            if (!includesCollection(node, "Categories"))
                TreeParts.addCategories(node.Nodes, pcm, filteredTableDatas);
            if (!includesCollection(node, "Segments"))
                TreeParts.addSegments(node.Nodes, pcm, filteredTableDatas);

        }

        public static void addNodes(TreeNodeCollection parent, PcmFile pcm1, List<TableData> filteredTableDatas)
        {
            parent.Clear();

            TreeNode tn = new TreeNode("All");
            tn.Name = "All";
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            parent.Add(tn);

            addDimensions(parent,filteredTableDatas);
            addValueTypes(parent,filteredTableDatas);
            addCategories(parent, pcm1, filteredTableDatas);
            addSegments(parent, pcm1, filteredTableDatas);

        }

        public static void addDimensions(TreeNodeCollection parent, List<TableData> filteredTableDatas)
        {

            TreeNode tnD = new TreeNode();
            tnD.Name = "Dimensions";
            tnD.ImageKey = "dimensions.ico";
            tnD.SelectedImageKey = "dimensions.ico";
            parent.Add(tnD);

            List<int> usedDimension = new List<int>();

            for (int i=0; i< filteredTableDatas.Count; i++)
            {
                int d = filteredTableDatas[i].Dimensions();
                if (!usedDimension.Contains(d))
                    usedDimension.Add(d);
                if (usedDimension.Count == 3)
                    break;
            }

            if (usedDimension.Contains(1))
            {
                TreeNode tn1 = new TreeNode();
                tn1.Name = "1D";
                tn1.ImageKey = "1d.ico";
                tn1.SelectedImageKey = "1d.ico";
                tnD.Nodes.Add(tn1);
            }

            if (usedDimension.Contains(2))
            {
                TreeNode tn2 = new TreeNode();
                tn2.Name = "2D";
                tn2.ImageKey = "2d.ico";
                tn2.SelectedImageKey = "2d.ico";
                tnD.Nodes.Add(tn2);
            }

            if (usedDimension.Contains(3))
            {
                TreeNode tn3 = new TreeNode();
                tn3.Name = "3D";
                tn3.ImageKey = "3d.ico";
                tn3.SelectedImageKey = "3d.ico";
                tnD.Nodes.Add(tn3);
            }

        }
        public static void addValueTypes(TreeNodeCollection parent, List<TableData> filteredTableDatas)
        {

            TreeNode tnT = new TreeNode();
            tnT.Name = "ValueTypes";
            tnT.ImageKey = "valuetype.ico";
            tnT.SelectedImageKey = "valuetype.ico";
            parent.Add(tnT);

            List<string> usedValueTypes = new List<string>();
            for (int i = 0; i < filteredTableDatas.Count; i++)
            {
                string vt = getValueType(filteredTableDatas[i]).ToString();
                if (!usedValueTypes.Contains(vt))
                    usedValueTypes.Add(vt);
                if (usedValueTypes.Count == 4)
                    break;  //all types collected
            }

            if (usedValueTypes.Contains("boolean"))
            {
                TreeNode tnB = new TreeNode();
                tnB.Name = "boolean";
                tnB.ImageKey = "boolean.ico";
                tnB.SelectedImageKey = "boolean.ico";
                tnT.Nodes.Add(tnB);
            }

            if (usedValueTypes.Contains("bitmask"))
            { 
                TreeNode tnM = new TreeNode();
                tnM.Name = "bitmask";
                tnM.ImageKey = "bitmask.ico";
                tnM.SelectedImageKey = "bitmask.ico";
                tnT.Nodes.Add(tnM);
            }

            if (usedValueTypes.Contains("selection"))
            {
                TreeNode tnE = new TreeNode();
                tnE.Name = "selection";
                tnE.ImageKey = "enum.ico";
                tnE.SelectedImageKey = "enum.ico";
                tnT.Nodes.Add(tnE);
            }

            if (usedValueTypes.Contains("number"))
            {
                TreeNode tnN = new TreeNode();
                tnN.Name = "number";
                tnN.ImageKey = "number.ico";
                tnN.SelectedImageKey = "number.ico";
                tnT.Nodes.Add(tnN);
            }
        }
        public static void addSegments(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas)
        {
            string iconFolder = Path.Combine(Application.StartupPath, "Icons");
            string[] GalleryArray = System.IO.Directory.GetFiles(iconFolder);

            TreeNode tnS = new TreeNode();
            tnS.Name = "Segments";
            tnS.ImageKey = "segments.ico";
            tnS.SelectedImageKey = "segments.ico";
            parent.Add(tnS);

            List<string> usedSegments = new List<string>();
            for (int i=0; i< filteredTableDatas.Count; i++)
            {
                string seg = filteredTableDatas[i].Segment(PCM);
                if (!usedSegments.Contains(seg))
                        usedSegments.Add(seg);
            }

            TreeNode segTn;
            for (int i = 0; i < usedSegments.Count; i++)
            {
                segTn = new TreeNode(usedSegments[i]);
                segTn.Name = usedSegments[i];
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
        public static void addCategories(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas)
        {
            TreeNode tnC = new TreeNode();
            tnC.Name = "Categories";
            tnC.ImageKey = "category.ico";
            tnC.SelectedImageKey = "category.ico";
            parent.Add(tnC);

            List<string> usedCategories = new List<string>();
            for (int i=0; i< filteredTableDatas.Count; i++)
            {
                if (!usedCategories.Contains(filteredTableDatas[i].Category))
                    usedCategories.Add(filteredTableDatas[i].Category);
            }
            for (int c = 0; c < usedCategories.Count; c++)
            {
                string cat = usedCategories[c];
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
