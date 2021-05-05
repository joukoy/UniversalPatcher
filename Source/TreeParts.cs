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
            if (node.Name == nodeName)
                return true;
            if (node.Parent == null)
                return false;   //Root-node
            if (node.Parent.Name == nodeName)
                return true;
            return includesCollection(node.Parent, nodeName);
        }

        public static void addChildNodes(TreeNode node, PcmFile pcm)
        {
            if (node.Name == "Dimensions" || node.Name == "ValueTypes" || node.Name == "Categories" || node.Name == "Segments")
            {
                foreach (TreeNode childTn in node.Nodes)
                    addChildNodes(childTn, pcm);
                return;
            }

            List<TableData> filteredTableDatas = filterTD(node, pcm);
            if (!includesCollection(node, "Dimensions"))
                TreeParts.addDimensions(node.Nodes,filteredTableDatas);
            if (!includesCollection(node, "ValueTypes"))
                TreeParts.addValueTypes(node.Nodes,filteredTableDatas);
            if (!includesCollection(node, "Categories"))
                TreeParts.addCategories(node.Nodes, pcm, filteredTableDatas);
            if (!includesCollection(node, "Segments"))
                TreeParts.addSegments(node.Nodes, pcm, filteredTableDatas);

        }

        public static void addNodes(TreeNodeCollection parent, PcmFile pcm1)
        {
            parent.Clear();

            TreeNode tn = new TreeNode("All");
            tn.Name = "All";
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            parent.Add(tn);

            addDimensions(parent,pcm1.tableDatas);
            addValueTypes(parent, pcm1.tableDatas);
            addCategories(parent, pcm1, pcm1.tableDatas);
            addSegments(parent, pcm1, pcm1.tableDatas);

        }

        public static void addDimensions(TreeNodeCollection parent, List<TableData> filteredTableDatas)
        {

            TreeNode tnD = new TreeNode();
            tnD.Name = "Dimensions";
            tnD.ImageKey = "dimensions.ico";
            tnD.SelectedImageKey = "dimensions.ico";

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

            if (tnD.Nodes.Count > 0)
                parent.Add(tnD);

        }
        public static void addValueTypes(TreeNodeCollection parent, List<TableData> filteredTableDatas)
        {

            TreeNode tnT = new TreeNode();
            tnT.Name = "ValueTypes";
            tnT.ImageKey = "valuetype.ico";
            tnT.SelectedImageKey = "valuetype.ico";

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

            if (tnT.Nodes.Count > 0)
                parent.Add(tnT);

        }
        public static void addSegments(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas)
        {
            string iconFolder = Path.Combine(Application.StartupPath, "Icons");
            string[] GalleryArray = System.IO.Directory.GetFiles(iconFolder);

            TreeNode tnS = new TreeNode();
            tnS.Name = "Segments";
            tnS.ImageKey = "segments.ico";
            tnS.SelectedImageKey = "segments.ico";

            List<string> usedSegments = new List<string>();
            for (int i=0; i< filteredTableDatas.Count; i++)
            {
                string seg = PCM.GetSegmentName(filteredTableDatas[i].addrInt);
                if (seg.Length > 0 && !usedSegments.Contains(seg))
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
            if (tnS.Nodes.Count > 0)
                parent.Add(tnS);

        }
        public static void addCategories(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas)
        {
            TreeNode tnC = new TreeNode();
            tnC.Name = "Categories";
            tnC.ImageKey = "category.ico";
            tnC.SelectedImageKey = "category.ico";

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
            if (tnC.Nodes.Count > 0)
                parent.Add(tnC);

        }

        private static List<TableData> filterTD(TreeNode tn, PcmFile PCM)
        {
            List<string> selectedSegs = new List<string>();
            List<string> selectedCats = new List<string>();
            List<string> selectedValTypes = new List<string>();
            List<string> selectedDimensions = new List<string>();

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

            List<TableData> results = PCM.tableDatas;
            if (selectedSegs.Count > 0)
            {
                List<TableData> newTDList = new List<TableData>();
                foreach (string seg in selectedSegs)
                {
                    int segNr = 0;
                    for (int s = 0; s < PCM.segmentinfos.Length; s++)
                        if (PCM.segmentinfos[s].Name == seg)
                            segNr = s;
                    uint addrStart = PCM.segmentAddressDatas[segNr].SegmentBlocks[0].Start;
                    uint addrEnd = PCM.segmentAddressDatas[segNr].SegmentBlocks[PCM.segmentAddressDatas[segNr].SegmentBlocks.Count - 1].End;
                    var newResults = results.Where(t => t.addrInt >= addrStart && t.addrInt <= addrEnd);
                    foreach (TableData nTd in newResults)
                        newTDList.Add(nTd);
                }
                results = newTDList;
            }

            if (selectedCats.Count > 0)
            {
                List<TableData> newTDList = new List<TableData>();
                foreach (TableData td in results)
                {
                    if (selectedCats.Contains(td.Category))
                        newTDList.Add(td);
                }
                results = newTDList;
            }

            if (selectedValTypes.Count > 0)
            {
                List<TableData> newTDList = new List<TableData>();
                foreach (string valT in selectedValTypes)
                {
                    if (valT == "mask")
                    {
                        foreach (TableData td in results)
                        {
                            if (td.BitMask != null && td.BitMask.Length > 0)
                                newTDList.Add(td);
                        }

                    }
                    else
                    {
                        foreach (TableData td in results)
                        {
                            string tdValT = getValueType(td).ToString();
                            if (tdValT == valT)
                                newTDList.Add(td);
                        }
                    }
                }
                results = newTDList;
            }

            if (selectedDimensions.Count > 0)
            {
                List<TableData> newTDList = new List<TableData>();
                foreach (TableData td in results)
                {
                    if (selectedDimensions.Contains(td.Dimensions().ToString() + "D"))
                        newTDList.Add(td);
                }
                results = newTDList;
            }
            return results;

        }
    }
}
