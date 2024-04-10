using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public static class TreeParts
    {
        public class Tnode
        {
            public Tnode(string NodeText, string NodeName, NType NodeType, string ico) 
            {
                filteredTds = new List<TableData>();
                Node = new TreeNode(NodeText);
                Node.Tag = this;
                Node.Name = NodeName;
                Node.ImageKey = ico;
                Node.SelectedImageKey = ico;
                this.NodeType = NodeType;
                ExtraCategory = false;
            }
            public Tnode(string NodeText,string NodeName, NType NodeType, string ico, TreeNode Parent) 
            {
                filteredTds = new List<TableData>();
                Node = new TreeNode(NodeText);
                Node.Text = NodeText;
                Node.Tag = this;
                Node.Name = NodeName;
                Node.ImageKey = ico;
                Node.SelectedImageKey = ico;
                Parent.Nodes.Add(Node);
                this.NodeType = NodeType;
                ParentTnode = (Tnode)Parent.Tag;
                ExtraCategory = false;
                //ParentNodeTypes.AddRange(ParentTnode.ParentNodeTypes);
                //ParentNodeTypes.Add(ParentTnode.NodeType);
            }
            public List<TableData> filteredTds { get; set; }
            public NType NodeType { get; set; }
            public Tnode ParentTnode { get; set; }
            //public List<NType> ParentNodeTypes { get; set; }
            public TreeNode Node { get; set; }
            //public int patchindex { get; set; }
            public Patch Patch { get; set; }
            public TableData Td { get; set; }
            public bool ExtraCategory { get; set; }
            public bool Isroot { get; set; }
        }

        public class Navi
        {
            public Navi()
            {

            }
            public Navi(TabPage Tab, List<string> Path, string Filter, string FilterBy, TableData Td)
            {
                this.Tab = Tab;
                this.Path = new List<string>();
                this.Path.AddRange(Path);
                this.Filter = Filter;
                this.FilterBy = FilterBy;
                this.Td = Td;
                if (this.Filter == null)
                    this.Filter = "";
                if (this.FilterBy == null)
                    this.FilterBy = "";
            }
            public Navi(TabPage Tab, string PathStr, string Filter, string FilterBy, TableData Td)
            {
                string[] pParts = PathStr.Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
                for (int p = pParts.Length - 1; p >= 0; p--)
                    Path.Add(pParts[p]);
                this.Tab = Tab;
                this.Filter = Filter;
                this.FilterBy = FilterBy;
                this.Td = Td;
                if (this.Filter == null)
                    this.Filter = "";
                if (this.FilterBy == null)
                    this.FilterBy = "";
            }

            public TabPage Tab { get; set; }
            public string TabName { 
                get
                {
                    string tabName = "Listmode";
                    if (Tab != null)
                        tabName = Tab.Name;
                    return tabName;
                }
            }
            public string TableName
            {
                get
                {
                    string tableName = "";
                    if (Td != null)
                        tableName = Td.TableName;
                    return tableName;
                }
            }
            public List<string> Path { get; set; }
            public string Filter { get; set; }
            public string FilterBy { get; set; }
            public TableData Td { get; set; }

            public string PathStr()
            {
                StringBuilder sb = new StringBuilder();
                for (int i=Path.Count-1; i>=0; i--)
                {
                    sb.Append(Path[i]);
                    if (i>0)
                        sb.Append(" -> ");
                }
                return sb.ToString();
            }
            //
            //This func returns node's path without node (table) itself
            //
            public string NodePath()
            {
                StringBuilder sb = new StringBuilder();
                for (int i=Path.Count-1;i > 0; i--)
                {
                    sb.Append(Path[i]);
                    if (i<Path.Count -1)
                        sb.Append(" -> ");
                }
                return sb.ToString();
            }

            //
            //"Fingerprint" of navi, for comparing 
            //
            public string NodeSerial()
            {
                string naviSerial = TabName +"-"+ Filter +"-"+ FilterBy +"-"+ PathStr();
                return naviSerial;
            }

            //
            //"Fingerprint" of navi, for comparing 
            //
            public string PathSerial()
            {
                string pathSerial = TabName +"-" +Filter +"-"+ FilterBy +"-"+ NodePath();
                Debug.WriteLine("Pathserial: " + pathSerial);
                return pathSerial;
            }

            public string NaviInfo()
            {
                StringBuilder nSb = new StringBuilder();
                nSb.Append("Tab: " + TabName + Environment.NewLine);
                nSb.Append("Filter: " + Filter + Environment.NewLine);
                nSb.Append("FilterBy: " + FilterBy + Environment.NewLine);
                //nSb.Append("Table: " + TableName + Environment.NewLine);
                nSb.Append("Path: " + PathStr());
                return nSb.ToString();
            }
        }

        public enum NType
        {
            Valuetype,
            Dimensions,
            Category,
            Segment,
            Patch,
            Table,
            Root
        }

        public static bool IncludesCollection(TreeNode node, NType NodeType, bool parentCheck)
        {
            try
            {
                Tnode tnode = (Tnode)node.Tag;
                if (!parentCheck)
                {
                    foreach (TreeNode childTn in node.Nodes)
                    {
                        Tnode child = (Tnode)childTn.Tag;
                        if (child.NodeType == NodeType)
                            return true;
                    }
                }
                if (tnode.NodeType == NodeType)
                    return true;
                if (node.Parent == null)
                    return false;   //Root-node
                if (tnode.ParentTnode != null && tnode.ParentTnode.NodeType == NodeType)
                    return true;
                return IncludesCollection(node.Parent, NodeType, true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
            return false;
        }

        public static Patch LoadPatch(string fileName, PcmFile pcm)
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
                patch.Name = Path.GetFileName(fileName);

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
            try
            {
                if (patches.Count == 0)
                {
                    Logger("Loading patches...");
                    Application.DoEvents();
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
                    Logger("Done");
                }
                int ind = 0;
                foreach (Patch patch in patches)
                {
                    if (!node.Nodes.ContainsKey(patch.Name))
                    {
                        Tnode tn = new Tnode(patch.Name, patch.Name, NType.Patch, "patch.ico", node);
                        Debug.WriteLine("Patch: " + patch.Name);
                        tn.Patch = patch;
                    }
                    ind++;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }

        public static void AddChildNodes(TreeNode node, PcmFile pcm)
        {
            try
            {
                if (node.Name == "Dimensions" || node.Name == "ValueTypes" || node.Name == "Categories" || node.Name == "Segments")
                {
                    foreach (TreeNode childTn in node.Nodes)
                        AddChildNodes(childTn, pcm);
                    return;
                }
                Tnode tnode = (Tnode)node.Tag;
                if (!IncludesCollection(node, NType.Dimensions, false))
                    TreeParts.AddDimensions(node.Nodes, tnode.filteredTds,true);
                if (!IncludesCollection(node, NType.Valuetype, false))
                    TreeParts.AddValueTypes(node.Nodes, tnode.filteredTds,true);
                if (!IncludesCollection(node, NType.Category, false))
                    TreeParts.AddCategories(node.Nodes, tnode.filteredTds,true);
                if (!IncludesCollection(node, NType.Segment, false))
                    TreeParts.AddSegments(node.Nodes, pcm, tnode.filteredTds,true);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }

        public static void AddNodes(TreeNodeCollection parent,PcmFile pcm1, List<TableData>filteredtables, bool AddRoot)
        {
            try
            {
                parent.Clear();

                string txt = "All";
                if (AppSettings.TunerShowTableCount)
                    txt += " [" + filteredtables.Count.ToString() + "]";
                Tnode tn = new Tnode(txt, "All", NType.Root, "explorer.ico");
                tn.filteredTds = filteredtables;
                tn.Isroot = true; 
                tn.Node.ImageKey = "explorer.ico";
                tn.Node.SelectedImageKey = "explorer.ico";
                parent.Add(tn.Node);

                AddDimensions(parent, filteredtables, AddRoot);
                AddValueTypes(parent, filteredtables, AddRoot);
                AddCategories(parent, filteredtables, AddRoot);
                AddSegments(parent, pcm1, filteredtables, AddRoot);


                Tnode tnP = new Tnode("", "Patches", NType.Patch, "patch.ico");
                tnP.Isroot = true;
                parent.Add(tnP.Node);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }

        }

        public static void AddDimensions(TreeNodeCollection parent, List<TableData> filteredTableDatas, bool AddRoot)
        {
            try
            {
                if (filteredTableDatas == null)
                    return;

                Tnode tnD = new Tnode("", "Dimensions", NType.Dimensions, "dimensions.ico");
                tnD.Isroot = true;

                Dictionary<int, List<TableData>> dimensiontables = new Dictionary<int, List<TableData>>();
                for (int i=1; i<=3;i++)
                {
                    dimensiontables.Add(i, new List<TableData>());
                }

                for (int t = 0; t < filteredTableDatas.Count; t++)
                {
                    int d = filteredTableDatas[t].Dimensions();
                    dimensiontables[d].Add(filteredTableDatas[t]);
                }

                for (int i=1; i<=3; i++)
                {
                    if (dimensiontables[i].Count > 0)
                    {
                        string txt = "";
                        if (AppSettings.TunerShowTableCount)
                            txt = " [" + dimensiontables[i].Count.ToString() + "]";
                        Tnode tnode = new Tnode(txt, i.ToString() + "D", NType.Dimensions, i.ToString() + "d.ico", tnD.Node);
                        tnode.filteredTds = dimensiontables[i];
                    }
                }

                if (tnD.Node.Nodes.Count > 0)
                {
                    if (!AddRoot)
                    {
                        foreach (TreeNode node in tnD.Node.Nodes)
                        {
                            parent.Add(node);
                        }
                    }
                    else
                    {
                        parent.Add(tnD.Node);
                    }
/*                    for (int i = 0; i < filteredTableDatas.Count; i++)
                    {
                        int d = filteredTableDatas[i].Dimensions();
                        string dStr = d.ToString() + "D";
                        Tnode tnode = (Tnode)tnD.Node.Nodes[dStr].Tag;
                        tnode.filteredTds.Add(filteredTableDatas[i]);
                    }
*/
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }

        public static void AddValueTypes(TreeNodeCollection parent, List<TableData> filteredTableDatas, bool AddRoot)
        {
            try
            {
                if (filteredTableDatas == null)
                    return;

                Tnode tnT = new Tnode("", "ValueTypes", NType.Valuetype, "valuetype.ico");
                tnT.Isroot = true;

                List<string> usedValueTypes = new List<string>();
                Dictionary<string, List<TableData>> valueTypeTables = new Dictionary<string, List<TableData>>();

                for (int t = 0; t < filteredTableDatas.Count; t++)
                {
                    string vt = filteredTableDatas[t].ValueType().ToString();
                    if (!valueTypeTables.ContainsKey(vt))
                    {
                        valueTypeTables.Add(vt, new List<TableData>());
                        usedValueTypes.Add(vt);
                    }
                    valueTypeTables[vt].Add(filteredTableDatas[t]);
                }

                foreach (string tabletype in usedValueTypes)
                {
                    string txt = "";
                    if (AppSettings.TunerShowTableCount)
                        txt += "[" + valueTypeTables[tabletype].Count.ToString() + "]";
                    Tnode tnode = new Tnode(txt, tabletype, NType.Valuetype, tabletype + ".ico", tnT.Node);
                    tnode.filteredTds = valueTypeTables[tabletype];
                }

                if (tnT.Node.Nodes.Count > 0)
                {
                    if (!AddRoot)
                    {
                        foreach (TreeNode node in tnT.Node.Nodes)
                        {
                            parent.Add(node);
                        }
                    }
                    else
                    {
                        parent.Add(tnT.Node);
                    }
/*                    for (int i = 0; i < filteredTableDatas.Count; i++)
                    {
                        string vt = GetTableValueType(filteredTableDatas[i]).ToString();
                        Tnode tnode = (Tnode)tnT.Node.Nodes[vt].Tag;
                        tnode.filteredTds.Add(filteredTableDatas[i]);
                    }
*/
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }

        public static void AddSegments(TreeNodeCollection parent, PcmFile PCM, List<TableData> filteredTableDatas, bool AddRoot, bool OffsetTool = false)
        {
            try
            {
                if (filteredTableDatas == null || PCM == null || PCM.segmentinfos == null)
                    return;

                string iconFolder = Path.Combine(Application.StartupPath, "Icons");
                string[] GalleryArray = System.IO.Directory.GetFiles(iconFolder);

                Tnode tnS = new Tnode("", "Segments", NType.Segment, "segments.ico");
                if (OffsetTool)
                {
                    tnS.Node.Name = "OffsetTool";
                    tnS.Node.Text = "Offset";
                }
                tnS.Isroot = true;
                List<string> usedSegments = new List<string>();
                Dictionary<string, List<TableData>> segTables = new Dictionary<string, List<TableData>>();
                for (int t = 0; t < filteredTableDatas.Count; t++)
                {
                    string seg = PCM.GetSegmentName(filteredTableDatas[t].addrInt);
                    //Debug.WriteLine(filteredTableDatas[t].TableName + ": " + seg);
                    if (string.IsNullOrEmpty(seg))
                        seg = "Unknown";
                    if (!usedSegments.Contains(seg))
                    {
                        usedSegments.Add(seg);
                        segTables.Add(seg, new List<TableData>());
                    }
                    segTables[seg].Add(filteredTableDatas[t]);
                }

                List<string> tmpL = new List<string>();
                foreach (SegmentInfo s in PCM.segmentinfos)
                {
                    if (usedSegments.Contains(s.Name))
                        tmpL.Add(s.Name);
                }
                usedSegments = tmpL;
                //usedSegments.Sort();

                Tnode segTn;
                for (int i = 0; i < usedSegments.Count; i++)
                {
                    string seg = usedSegments[i];
                    string segName = usedSegments[i];
                    if (OffsetTool)
                        segName += "-offset";
                    string txt = usedSegments[i];
                    if (AppSettings.TunerShowTableCount)
                        txt += " [" + segTables[seg].Count.ToString() + "]";
                    segTn = new Tnode(txt, segName, NType.Segment, "segments.ico", tnS.Node);
                    segTn.filteredTds = segTables[seg];

                    bool found = false;
                    foreach (string icofile in GalleryArray)
                    {
                        double percentage = ComputeSimilarity.CalculateSimilarity(Path.GetFileNameWithoutExtension(icofile).ToLower(), seg.ToLower());
                        if ((int)(percentage * 100) >= 80)
                        {
                            segTn.Node.ImageKey = Path.GetFileName(icofile);
                            segTn.Node.SelectedImageKey = Path.GetFileName(icofile);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        foreach (string icofile in GalleryArray)
                        {
                            if (segTn.Node.Name.ToLower().Contains(Path.GetFileNameWithoutExtension(icofile)))
                            {
                                segTn.Node.ImageKey = Path.GetFileName(icofile);
                                segTn.Node.SelectedImageKey = Path.GetFileName(icofile);
                                found = true;
                                break;
                            }
                        }
                    }
                }
                if (tnS.Node.Nodes.Count > 0)
                {
                    if (!AddRoot)
                    {
                        foreach (TreeNode node in tnS.Node.Nodes)
                        {
                            parent.Add(node);
                        }
                    }
                    else
                    {
                        parent.Add(tnS.Node);
                    }

/*                    for (int i = 0; i < filteredTableDatas.Count; i++)
                    {
                        string seg = PCM.GetSegmentName(filteredTableDatas[i].addrInt);
                        if (!tnS.Node.Nodes.ContainsKey(seg))
                            seg = "Unknown";
                        Tnode tnode = (Tnode)tnS.Node.Nodes[seg].Tag;
                        tnode.filteredTds.Add(filteredTableDatas[i]);
                    }
*/
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }


        public static void AddCategories(TreeNodeCollection parent, List<TableData> TableDatas, bool AddRoot)
        {
            try
            {
                if (TableDatas == null)
                    return;

                Tnode tnC = new Tnode("", "Categories",NType.Category, "category.ico");
                tnC.Isroot = true;

                //Main categories
                List<TableData> filteredTableDatas = TableDatas.OrderBy(x => x.Category).ToList();

                for (int i = 0; i < filteredTableDatas.Count; i++)
                {
                    //string cat = filteredTableDatas[i].Category;
                    string cat = "";
                    string[] mainCats = filteredTableDatas[i].MainCategories().ToArray();
                    if (string.IsNullOrEmpty(filteredTableDatas[i].Category))
                    {
                        cat = EmptyCategories;
                    }
                    else
                    {
                        cat = mainCats[0];
                    }
                    if (!tnC.Node.Nodes.ContainsKey(cat))
                    {
                        _ = new Tnode(cat,cat,NType.Category, "category.ico", tnC.Node);
                    }
                    Tnode subTn = (Tnode)tnC.Node.Nodes[cat].Tag;
                    subTn.filteredTds.Add(filteredTableDatas[i]);
                    if (AppSettings.TunerShowTableCount)
                        subTn.Node.Text = cat + " [" + subTn.filteredTds.Count.ToString() + "]";
                    StringBuilder sb = new StringBuilder(cat);
                    for (int c = 1; c < mainCats.Length; c++)
                    {
                        sb.Append(" - " + mainCats[c]);
                        if (!subTn.Node.Nodes.ContainsKey(sb.ToString())) //&& !subTn.Nodes.ContainsKey(filteredTableDatas[i].ExtraCategories))
                        {
                            _ = new Tnode(mainCats[c], sb.ToString(), NType.Category, "category.ico", subTn.Node);

                        }
                        subTn = (Tnode)subTn.Node.Nodes[sb.ToString()].Tag;
                        subTn.filteredTds.Add(filteredTableDatas[i]);
                        if (AppSettings.TunerShowTableCount)
                            subTn.Node.Text = mainCats[c] + " [" + subTn.filteredTds.Count.ToString() + "]";
                    }
                }

                //Extra categories
                filteredTableDatas = TableDatas.OrderBy(x => x.ExtraCategories).ToList();

                Tnode tnC2;
                if (AppSettings.TunerTreeMode)
                {
                    tnC2 = new Tnode("", "Categories2", NType.Category, "category3.ico");
                }
                else
                {
                    tnC2 = tnC; //Disable second root. 
                }
                for (int i = 0; i < filteredTableDatas.Count; i++)
                {
                    if (string.IsNullOrEmpty(filteredTableDatas[i].ExtraCategories))
                    {
                        if (!tnC2.Node.Nodes.ContainsKey(ExtraCategories))
                            _ = new Tnode(ExtraCategories, ExtraCategories, NType.Category, "category3.ico", tnC2.Node);
                        if (!tnC2.Node.Nodes.ContainsKey(EmptyCategories))
                            _ = new Tnode(EmptyCategories, EmptyCategories, NType.Category, "category3.ico", tnC2.Node);
                        Tnode subTn = (Tnode)tnC2.Node.Nodes[EmptyCategories].Tag;
                        subTn.ExtraCategory = true;
                        subTn.filteredTds.Add(filteredTableDatas[i]);
                        if (AppSettings.TunerShowTableCount)
                            subTn.Node.Text = EmptyCategories + " [" + subTn.filteredTds.Count.ToString() + "]";
                    }
                    else
                    {
                        if (!tnC2.Node.Nodes.ContainsKey(ExtraCategories))
                            _ = new Tnode(ExtraCategories, ExtraCategories, NType.Category, "category3.ico", tnC2.Node);

                        string[] subCats = filteredTableDatas[i].SubCategories().ToArray();
                        if (!tnC2.Node.Nodes.ContainsKey(subCats[0]))
                        {
                            _ = new Tnode(subCats[0], subCats[0], NType.Category, "category3.ico", tnC2.Node);
                        }
                        Tnode subTn = (Tnode)tnC2.Node.Nodes[subCats[0]].Tag;
                        subTn.ExtraCategory = true;
                        subTn.filteredTds.Add(filteredTableDatas[i]);
                        if (AppSettings.TunerShowTableCount)
                            subTn.Node.Text = subCats[0] + " [" + subTn.filteredTds.Count.ToString() + "]";
                        StringBuilder sb = new StringBuilder(subCats[0]);
                        for (int c = 1; c < subCats.Length; c++)
                        {
                            sb.Append(" - " + subCats[c]);
                            if (!subTn.Node.Nodes.ContainsKey(sb.ToString())) //&& !subTn.Nodes.ContainsKey(filteredTableDatas[i].ExtraCategories))
                            {
                                _ = new Tnode(subCats[c], sb.ToString(), NType.Category, "category3.ico", subTn.Node);
                            }
                            subTn = (Tnode)subTn.Node.Nodes[sb.ToString()].Tag;
                            subTn.ExtraCategory = true;
                            subTn.filteredTds.Add(filteredTableDatas[i]);
                            if (AppSettings.TunerShowTableCount)
                                subTn.Node.Text = subCats[c] + " [" + subTn.filteredTds.Count.ToString() + "]";
                        }
                    }
                }
                if (tnC.Node.Nodes.Count > 0)
                {
                    parent.Add(tnC.Node);
                }
                if (AppSettings.TunerTreeMode && tnC2.Node.Nodes.Count > 0)
                {
                    parent.Add(tnC2.Node);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }

        public static List<string> GetCurrentNodePath(TreeViewMS tv)
        {
            try
            {
                if (tv == null || tv.SelectedNodes.Count == 0)
                    return null;
                List<string> path = new List<string>();
                TreeNode node = tv.SelectedNode;
                path.Add(node.Name);
                while (node.Parent != null)
                {
                    node = node.Parent;
                    path.Add(node.Name);
                }
                StringBuilder dbgStr = new StringBuilder("Saved " + tv.Name + ": ");
                for (int i = 0; i < path.Count; i++)
                {
                    dbgStr.Append(path[i] + ", ");
                }
                Debug.WriteLine(dbgStr.ToString());
                return path;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
            return null;
        }

        public static void AddTablesToTree(TreeNode Node)
        {
            Tnode tnode1 = (Tnode)Node.Tag;
            foreach (TreeNode t in Node.Nodes)
            {
                Tnode tx = (Tnode)t.Tag;
                if (tx.NodeType == NType.Table)
                {
                    return; //Files loaded already
                }
            }
            if (tnode1.NodeType == NType.Category)
            {
                for (int i = 0; i < tnode1.filteredTds.Count; i++)
                {
                    TableData td = tnode1.filteredTds[i];
                    string ico = td.ValueType().ToString().Replace("number", "") + td.Dimensions().ToString() + "d.ico";
                    string cat = td.Category;
                    if (tnode1.ExtraCategory)
                    {
                        cat = td.ExtraCategories;
                    }
                    if (string.IsNullOrEmpty(cat))
                    {
                        cat = EmptyCategories;
                    }
                    if (cat == tnode1.Node.Name)
                    {
                        Tnode tTd = new Tnode(td.TableName, td.TableName, NType.Table, ico, Node);
                        tTd.Td = td;
                    }
                }
            }
            else if (!AppSettings.TableExplorerUseCategorySubfolder || tnode1.Isroot )
            {
                for (int i = 0; i < tnode1.filteredTds.Count; i++)
                {
                    TableData td = tnode1.filteredTds[i];
                    string ico = td.ValueType().ToString().Replace("number", "") + td.Dimensions().ToString()+ "d.ico";
                    Tnode tTd = new Tnode(td.TableName, td.TableName, NType.Table, ico, Node);
                    tTd.Td = td;
                }
            }
            Application.DoEvents();
        }

        public static void RestoreNodePath(TreeViewMS tv, List<string> path, PcmFile PCM)
        {
            try
            {
                Debug.WriteLine("Restoring node");                
                if (path == null || path.Count == 0)
                {
                    Debug.WriteLine("Empty path");
                    return;
                }
                StringBuilder dbgStr = new StringBuilder("Restoring " + tv.Name +": ");
                for (int i = 0; i < path.Count; i++)
                {
                    dbgStr.Append(path[i] + ", ");
                }
                Debug.WriteLine(dbgStr.ToString());
                if (!tv.Nodes.ContainsKey(path.Last()))
                   return;
                TreeNode node = tv.Nodes[path.Last()];
                for (int i = path.Count - 2; i >= 0; i--)
                {
                    Tnode tnode1 = (Tnode)node.Tag;
                    if (tv.Name == "treeView1")
                    {
                        if (node.Name != "All" && node.Parent != null)
                            TreeParts.AddChildNodes(node, PCM);
                    }
                    else
                    {
                        if (AppSettings.TableExplorerUseCategorySubfolder &&
                            (node.Parent == null || tnode1.ParentTnode == null || tnode1.ParentTnode.Isroot))
                        {
                            if (!IncludesCollection(node, NType.Category, false))
                                AddCategories(node.Nodes, tnode1.filteredTds, false);
                        }
                        AddTablesToTree(node);
                    }
                    if (!node.Nodes.ContainsKey(path[i]))
                    {
                        Debug.WriteLine("Node " + node.Name + " not contains key: " + path[i]);
                        break;
                    }
                    node = node.Nodes[path[i]];
                }
                tv.SelectedNode = node;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TreeParts line " + line + ": " + ex.Message);
                Debug.WriteLine("Error, TreeParts line " + line + ": " + ex.Message);
            }
        }
    }
}
