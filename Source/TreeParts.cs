using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;

namespace UniversalPatcher
{
    public static class TreeParts
    {
        private static bool IncludesCollection(TreeNode node, string nodeName, bool parentCheck)
        {
            if (!parentCheck)
            {
                foreach (TreeNode childTn in node.Nodes)
                    if (childTn.Name == nodeName)
                        return true;
            }
            if (node.Name == nodeName)
                return true;
            if (node.Parent == null)
                return false;   //Root-node
            if (node.Parent.Name == nodeName)
                return true;
            return IncludesCollection(node.Parent, nodeName, true);
        }

        private static Patch LoadPatch(string fileName, PcmFile pcm)
        {
            try
            {
                Patch patch = new Patch();
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof(List<XmlPatch>));
                System.IO.StreamReader file = new System.IO.StreamReader(fileName);
                patch.patches = (List<XmlPatch>)reader.Deserialize(file);
                file.Close();
                string CompOS = "";
                patch.Name = patch.patches[0].Name;
                if (patch.patches.Count > 0)
                {
                    string[] OsList = patch.patches[0].CompatibleOS.Split(',');
                    foreach (string OS in OsList)
                    {
                        if (CompOS != "")
                            CompOS += ",";
                        string[] Parts = OS.Split(':');
                        CompOS += Parts[0];
                    }
                }
                bool isCompatible = false;
                for (int x = 0; x < patch.patches.Count; x++)
                {
                    if (CheckPatchCompatibility(patch.patches[x], pcm) < uint.MaxValue)
                    {
                        isCompatible = true;
                    }
                }
                if (isCompatible)
                    return patch;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
            return null;
        }

        public static void AddPatchNodes(TreeNode node, PcmFile pcm)
        {
            if (patches.Count == 0)
            {
                string folder = Path.Combine(Application.StartupPath, "Patches");
                DirectoryInfo d = new DirectoryInfo(folder);
                FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (FileInfo file in Files)
                {
                    Patch patch = LoadPatch(file.FullName, pcm);
                    if (patch != null)
                    {
                        patches.Add(patch);
                    }
                }
            }
            int ind = 0;
            foreach (Patch patch in patches)
            {
                TreeNode tn = new TreeNode(patch.Name);
                tn.Name = patch.Name;
                tn.Tag = ind;
                node.Nodes.Add(tn);
                ind++;
            }

        }

        public static void AddChildNodes(TreeNode node, PcmFile pcm)
        {
            if (node.Name == "Dimensions" || node.Name == "ValueTypes" || node.Name == "Categories" || node.Name == "Segments")
            {
                foreach (TreeNode childTn in node.Nodes)
                    AddChildNodes(childTn, pcm);
                return;
            }

            List<TableData> filteredTableDatas = FilterTD(node, pcm);
            if (!IncludesCollection(node, "Dimensions",false))
                TreeParts.AddDimensions(node.Nodes,filteredTableDatas);
            if (!IncludesCollection(node, "ValueTypes", false))
                TreeParts.AddValueTypes(node.Nodes,filteredTableDatas);
            if (!IncludesCollection(node, "Categories", false))
                TreeParts.AddCategories(node.Nodes, pcm, filteredTableDatas);
            if (!IncludesCollection(node, "Segments", false))
                TreeParts.AddSegments(node.Nodes, pcm, filteredTableDatas);

        }

        public static void AddNodes(TreeNodeCollection parent, PcmFile pcm1)
        {
            parent.Clear();

            TreeNode tn = new TreeNode("All");
            tn.Name = "All";
            tn.ImageKey = "explorer.ico";
            tn.SelectedImageKey = "explorer.ico";
            parent.Add(tn);

            AddDimensions(parent,pcm1.tableDatas);
            AddValueTypes(parent, pcm1.tableDatas);
            AddCategories(parent, pcm1, pcm1.tableDatas);
            AddSegments(parent, pcm1, pcm1.tableDatas);

            
            TreeNode tnP = new TreeNode();
            tnP.Name = "Patches";
            tnP.ImageKey = "patch.ico";
            tnP.SelectedImageKey = "patch.ico";
            parent.Add(tnP);
            
        }

        public static void AddDimensions(TreeNodeCollection parent, List<TableData> filteredTableDatas)
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
        public static void AddValueTypes(TreeNodeCollection parent, List<TableData> filteredTableDatas)
        {

            TreeNode tnT = new TreeNode();
            tnT.Name = "ValueTypes";
            tnT.ImageKey = "valuetype.ico";
            tnT.SelectedImageKey = "valuetype.ico";

            List<string> usedValueTypes = new List<string>();
            for (int i = 0; i < filteredTableDatas.Count; i++)
            {
                string vt = GetTableValueType(filteredTableDatas[i]).ToString();
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

            if (usedValueTypes.Contains("patch"))
            {
                TreeNode tnN = new TreeNode();
                tnN.Name = "patch";
                tnN.ImageKey = "patch.ico";
                tnN.SelectedImageKey = "patch.ico";
                tnT.Nodes.Add(tnN);
            }

            if (tnT.Nodes.Count > 0)
                parent.Add(tnT);

        }
        public static void AddSegments(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas)
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
        public static void AddCategories(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas)
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

        private static List<TableData> FilterTD(TreeNode tn, PcmFile PCM)
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
                            string tdValT = GetTableValueType(td).ToString();
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
