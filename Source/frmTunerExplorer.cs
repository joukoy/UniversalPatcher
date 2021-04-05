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
    public partial class frmTunerExplorer : Form
    {
        public frmTunerExplorer()
        {
            InitializeComponent();
        }

        public PcmFile PCM;
        private frmTuner tuner;
        private TreeView treeDimensions;
        private TreeView treeValueType;
        private TreeView treeCategory;
        private TreeView treeSegments;
        private string[] GalleryArray;
        private int keyDelayCounter = 0;
        string currentTab;
        int iconSize;

        private void frmTunerExplorer_Load(object sender, EventArgs e)
        {
            labelTableName.Text = "";
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.TunerExplorerWindowSize.Width > 0 || Properties.Settings.Default.TunerExplorerWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.TunerExplorerWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.TunerExplorerWindowLocation;
                    this.Size = Properties.Settings.Default.TunerExplorerWindowSize;
                }
                if (Properties.Settings.Default.TunerExplorerWindowSplitterDistance > 0)
                    splitContainer1.SplitterDistance = Properties.Settings.Default.TunerExplorerWindowSplitterDistance;
                if (Properties.Settings.Default.TunerExplorerWindowSplitter2Distance > 0)
                    splitContainer2.SplitterDistance = Properties.Settings.Default.TunerExplorerWindowSplitter2Distance;
            }
            showCategorySubfolderToolStripMenuItem.Checked = Properties.Settings.Default.TableExplorerUseCategorySubfolder;
            this.FormClosing += FrmTunerExplorer_FormClosing;
            LogReceivers.Add(txtResult);
            tabDimensions.Enter += TabDimensions_Enter;
            tabValueType.Enter += TabValueType_Enter;
            tabPatches.Enter += TabPatches_Enter;
            tabCategory.Enter += TabCategory_Enter;
            tabSegments.Enter += TabSegments_Enter;
            currentTab = "Dimensions";
            if (PCM.dtcCodes.Count > 0)
            {
                tuner = new frmTuner(PCM);
                addtoCurrentFileMenu(PCM);
                selectPCM();
                filterTables();
                loadDimensions();
            }
        }
        private void FrmTunerExplorer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.TunerExplorerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.TunerExplorerWindowLocation = this.Location;
                    Properties.Settings.Default.TunerExplorerWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.TunerExplorerWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.TunerExplorerWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.TunerExplorerWindowSplitterDistance = splitContainer1.SplitterDistance;
                Properties.Settings.Default.TunerExplorerWindowSplitter2Distance = splitContainer2.SplitterDistance;
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

        private void loadDimensions()
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            if (tabDimensions.Controls.Contains(treeDimensions))
            {
                foreach (TreeNode tn in treeDimensions.Nodes)
                    tn.Nodes.Clear();
            }
            else
            {
                treeDimensions = new TreeView();
                setIconSize();
                treeDimensions.ImageList = imageList1;
                treeDimensions.CheckBoxes = true;
                treeDimensions.Dock = DockStyle.Fill;
                tabDimensions.Controls.Add(treeDimensions);
                treeDimensions.AfterCheck += Tree_AfterCheck;
                treeDimensions.AfterSelect += Tree_AfterSelect;
                treeDimensions.NodeMouseClick += Tree_NodeMouseClick;
                treeDimensions.Nodes.Add(createTreeNode("1D"));
                treeDimensions.Nodes.Add(createTreeNode("2D"));
                treeDimensions.Nodes.Add(createTreeNode("3D"));
            }

            for (int i = 0; i < PCM.tableDatas.Count; i++)
            {
                if (PCM.tableDatas[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {
                    TreeNode tnChild = new TreeNode(PCM.tableDatas[i].TableName);
                    tnChild.Tag = i;

                    TableValueType vt = getValueType(PCM.tableDatas[i]);
                    if (PCM.tableDatas[i].BitMask != null && PCM.tableDatas[i].BitMask.Length > 0)
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
                    if (PCM.tableDatas[i].Rows == 1 && PCM.tableDatas[i].Columns == 1)
                        nodeKey = "1D";
                    else if (PCM.tableDatas[i].Rows > 1 && PCM.tableDatas[i].Columns == 1)
                        nodeKey = "2D";
                    else
                        nodeKey = "3D";

                    if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                    {
                        treeDimensions.Nodes[nodeKey].Nodes.Add(tnChild);
                    }
                    else
                    {
                        string cat = PCM.tableDatas[i].Category;
                        if (cat == "")
                            cat = "(Empty)";
                        if (!treeDimensions.Nodes[nodeKey].Nodes.ContainsKey(cat))
                        {
                            TreeNode dimCatTn = new TreeNode(cat);
                            dimCatTn.Name = cat;
                            dimCatTn.ImageKey = "category.ico";
                            dimCatTn.SelectedImageKey = "category.ico";
                            treeDimensions.Nodes[nodeKey].Nodes.Add(dimCatTn);
                        }
                        treeDimensions.Nodes[nodeKey].Nodes[cat].Nodes.Add(tnChild);
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void Tree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void findCheckdNodes(TreeNode tn, ref List<int> tableIds)
        {
            if (tn.Nodes.Count > 0)
            {
                foreach (TreeNode tnChild in tn.Nodes)
                    findCheckdNodes(tnChild, ref tableIds);
            }
            else
            {
                if (tn.Checked)
                    tableIds.Add((int)tn.Tag);
            }
        }

        private void Tree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            clearPanel2();
            List<int> tableIds = new List<int>();
            foreach (TreeNode tn in e.Node.TreeView.Nodes)
            {
                findCheckdNodes(tn,ref tableIds);
            }
            if (tableIds.Count > 0)
                tuner.openTableEditor(tableIds, this);
        }

        private void Tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;
            clearPanel2();
            List<int> tableIds = new List<int>();
            tableIds.Add((int)e.Node.Tag);
            labelTableName.Text = tuner.openTableEditor(tableIds,this);
        }


        private void TabSegments_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Segments")
                return;
            currentTab = "Segments";
            clearPanel2();
            loadSegments();
        }


        private void showPatchSelector()
        {
            frmPatchSelector frmP = new frmPatchSelector();
            frmP.basefile = tuner.PCM;
            frmP.tunerForm = tuner;
            frmP.TopLevel = false;
            frmP.Dock = DockStyle.Fill;
            frmP.FormBorderStyle = FormBorderStyle.None;
            //frmP.splitContainer1.SplitterWidth = frmP.splitContainer1.Height;
            frmP.splitContainer1.Panel2Collapsed = true;
            frmP.splitContainer1.Panel2.Hide();
            splitContainer1.Panel2.Controls.Add(frmP);
            frmP.Show();
            Application.DoEvents();
            frmP.loadPatches();
        }

        private void TabPatches_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Patches")
                return;
            currentTab = "Patches";
            clearPanel2();
            showPatchSelector();
        }

        private void TabValueType_Enter(object sender, EventArgs e)
        {
            if (currentTab == "ValueType")
                return;
            currentTab = "ValueType";
            clearPanel2();
            loadValueTypes();
        }

        private void TabDimensions_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Dimensions")
                return;
            currentTab = "Dimensions";
            clearPanel2();
            loadDimensions();
        }

        private void TabCategory_Enter(object sender, EventArgs e)
        {
            if (currentTab == "Category")
                return;
            currentTab = "Category";
            clearPanel2();
            loadCategories();
        }

        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            txtResult.AppendText(LogText);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        private void setIconSize()
        {
            if (iconSize != (int)numIconSize.Value)
            {
                //Size modified since last call
                iconSize = (int)numIconSize.Value;
                imageList1.ImageSize = new Size(iconSize, iconSize);
                imageList1.Images.Clear();
                string folderIcon = Path.Combine(Application.StartupPath, "Icons", "explorer.ico");
                imageList1.Images.Add(Image.FromFile(folderIcon));
                string iconFolder = Path.Combine(Application.StartupPath, "Icons");
                GalleryArray = System.IO.Directory.GetFiles(iconFolder);
                for (int i = 0; i < GalleryArray.Length; i++)
                {
                    if (GalleryArray[i].ToLower().EndsWith(".ico"))
                    {

                        imageList1.Images.Add(Path.GetFileName(GalleryArray[i]), Icon.ExtractAssociatedIcon(GalleryArray[i]));
                    }
                }
            }
            if (treeDimensions != null)
            {
                treeDimensions.ItemHeight = iconSize + 2;
                treeDimensions.Indent = iconSize + 4;
                treeDimensions.Font = Properties.Settings.Default.TableExplorerFont;
            }
            if (treeCategory != null)
            {
                treeCategory.ItemHeight = iconSize + 2;
                treeCategory.Indent = iconSize + 4;
                treeCategory.Font = Properties.Settings.Default.TableExplorerFont;
            }
            if (treeSegments != null)
            {
                treeSegments.ItemHeight = iconSize + 2;
                treeSegments.Indent = iconSize + 4;
                treeSegments.Font = Properties.Settings.Default.TableExplorerFont;
            }
            if (treeValueType != null)
            {
                treeValueType.ItemHeight = iconSize + 2;
                treeValueType.Indent = iconSize + 4;
                treeValueType.Font = Properties.Settings.Default.TableExplorerFont;
            }
        }

        private void importTableSeek()
        {
            PCM.importSeekTables();
            Logger("OK");
        }

        private void importTinyTunerDB()
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDBtoTableData(PCM, PCM.tableDatas));

        }

        private void importDTC()
        {
            Logger("Importing DTC codes... ", false);
            bool haveDTC = false;
            for (int t = 0; t < PCM.tableDatas.Count; t++)
            {
                if (PCM.tableDatas[t].TableName == "DTC" || PCM.tableDatas[t].TableName == "DTC.Codes")
                {
                    haveDTC = true;
                    Logger(" DTC codes already defined");
                    break;
                }
            }
            if (!haveDTC)
            {
                TableData tdTmp = new TableData();
                tdTmp.importDTC(PCM, ref PCM.tableDatas);
                Logger(" [OK]");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newFile = SelectFile();
            if (newFile.Length == 0) 
                return;
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
            PCM = newPCM;
            //loadConfigforPCM(ref PCM);
            addtoCurrentFileMenu(newPCM);
            tuner.PCM = PCM;
            filterTables();
        }
        public void addtoCurrentFileMenu(PcmFile newPCM)
        {
            if (tuner == null)
                tuner = new frmTuner(newPCM);
            else
                tuner.addtoCurrentFileMenu(newPCM);
            foreach (ToolStripMenuItem mi in bINFileToolStripMenuItem.DropDownItems)
                mi.Checked = false;
            ToolStripMenuItem menuitem = new ToolStripMenuItem(newPCM.FileName);
            menuitem.Name = newPCM.FileName;
            menuitem.Tag = newPCM;
            menuitem.Checked = true;
            bINFileToolStripMenuItem.DropDownItems.Add(menuitem);
            menuitem.Click += Menuitem_Click;

        }
        private void Menuitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            bool isChecked = menuitem.Checked;
            menuitem.Checked = !isChecked;
            PCM = (PcmFile)menuitem.Tag;
            selectPCM();
        }

        private void selectPCM()
        {
            foreach (ToolStripMenuItem mi in bINFileToolStripMenuItem.DropDownItems)
                mi.Checked = false;

            ToolStripMenuItem mitem = (ToolStripMenuItem)bINFileToolStripMenuItem.DropDownItems[PCM.FileName];
            mitem.Checked = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger("Saving to file: " + PCM.FileName);
            PCM.saveBin(PCM.FileName);
            Logger("Done.");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (PCM == null || PCM.buf == null | PCM.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string fileName = SelectSaveFile("BIN files (*.bin)|*.bin|ALL files(*.*)|*.*", PCM.FileName);
                if (fileName.Length == 0)
                    return;

                Logger("Saving to file: " + fileName);
                PCM.saveBin(fileName);
                this.Text = "Tuner " + Path.GetFileName(fileName);
                Logger("Done.");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void numIconSize_ValueChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();

            setIconSize();
            Properties.Settings.Default.TableExplorerIconSize = (int)numIconSize.Value;
            Properties.Settings.Default.Save();
            Cursor.Current = Cursors.Default;

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.Font = Properties.Settings.Default.TableExplorerFont;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                if (treeDimensions != null)
                    treeDimensions.Font = fontDlg.Font;
                if (treeCategory != null)
                    treeCategory.Font = fontDlg.Font;
                if (treeSegments != null)
                    treeSegments.Font = fontDlg.Font;
                if (treeValueType != null)
                    treeValueType.Font = fontDlg.Font;
                Properties.Settings.Default.TableExplorerFont = fontDlg.Font;
                Properties.Settings.Default.Save();
            }

        }
        private void loadValueTypes()
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            if (tabValueType.Controls.Contains(treeValueType))
            {
                foreach (TreeNode tn in treeValueType.Nodes)
                    tn.Nodes.Clear();
            }
            else
            {
                treeValueType = new TreeView();
                setIconSize();
                treeValueType.ImageList = imageList1;
                treeValueType.CheckBoxes = true;
                treeValueType.Dock = DockStyle.Fill;
                tabValueType.Controls.Add(treeValueType);
                treeValueType.AfterSelect += Tree_AfterSelect;
                treeValueType.AfterCheck += Tree_AfterCheck;
                treeValueType.NodeMouseClick += Tree_NodeMouseClick;
                treeValueType.Nodes.Add(createTreeNode("number"));
                treeValueType.Nodes.Add(createTreeNode("enum"));
                treeValueType.Nodes.Add(createTreeNode("bitmask"));
                treeValueType.Nodes.Add(createTreeNode("boolean"));
            }

            for (int i = 0; i < PCM.tableDatas.Count; i++)
            {
                if (PCM.tableDatas[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {
                    TreeNode tnChild = new TreeNode(PCM.tableDatas[i].TableName);
                    tnChild.Tag = i;

                    if (PCM.tableDatas[i].Rows == 1 && PCM.tableDatas[i].Columns == 1)
                    {
                        tnChild.ImageKey = "1d.ico";
                        tnChild.SelectedImageKey = "1d.ico";
                    }
                    else if (PCM.tableDatas[i].Rows > 1 && PCM.tableDatas[i].Columns == 1)
                    {
                        tnChild.ImageKey = "2d.ico";
                        tnChild.SelectedImageKey = "2d.ico";
                    }
                    else
                    {
                        tnChild.ImageKey = "3d.ico";
                        tnChild.SelectedImageKey = "3d.ico";
                    }

                    TableValueType vt = getValueType(PCM.tableDatas[i]);
                    string nodeKey = "";
                    if (PCM.tableDatas[i].BitMask != null && PCM.tableDatas[i].BitMask.Length > 0)
                        nodeKey = "bitmask";
                    else if (vt == TableValueType.boolean)
                        nodeKey = "boolean";
                    else if (vt == TableValueType.selection)
                        nodeKey = "enum";
                    else
                        nodeKey = "number";

                    if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                    {
                        treeValueType.Nodes[nodeKey].Nodes.Add(tnChild);
                    }
                    else
                    {
                        string cat = PCM.tableDatas[i].Category;
                        if (cat == "")
                            cat = "(Empty)";
                        if (!treeValueType.Nodes[nodeKey].Nodes.ContainsKey(cat))
                        {
                            TreeNode vtCatTn = new TreeNode(cat);
                            vtCatTn.Name = cat;
                            vtCatTn.ImageKey = "category.ico";
                            vtCatTn.SelectedImageKey = "category.ico";
                            treeValueType.Nodes[nodeKey].Nodes.Add(vtCatTn);
                        }
                        treeValueType.Nodes[nodeKey].Nodes[cat].Nodes.Add(tnChild);
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void loadCategories()
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            if (tabCategory.Controls.Contains(treeCategory))
            {
                treeCategory.Nodes.Clear();
            }
            else
            {
                treeCategory = new TreeView();
                setIconSize();
                treeCategory.ImageList = imageList1;
                treeCategory.CheckBoxes = true;

                treeCategory.Dock = DockStyle.Fill;
                tabCategory.Controls.Add(treeCategory);
                treeCategory.AfterSelect += Tree_AfterSelect;
                treeCategory.AfterCheck += Tree_AfterCheck;
                treeCategory.NodeMouseClick += Tree_NodeMouseClick;
            }
            for (int i = 0; i < PCM.tableDatas.Count; i++)
            {
                if (PCM.tableDatas[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {

                    TreeNode tnChild = new TreeNode(PCM.tableDatas[i].TableName);
                    tnChild.Tag = i;
                    string ico = "";
                    TableValueType vt = getValueType(PCM.tableDatas[i]);
                    if (PCM.tableDatas[i].BitMask != null && PCM.tableDatas[i].BitMask.Length > 0)
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

                    if (PCM.tableDatas[i].Rows == 1 && PCM.tableDatas[i].Columns == 1)
                    {
                        ico += "1d.ico";
                    }
                    else if (PCM.tableDatas[i].Rows > 1 && PCM.tableDatas[i].Columns == 1)
                    {
                        ico += "2d.ico";
                    }
                    else
                    {
                        ico += "3d.ico";
                    }

                    tnChild.ImageKey = ico;
                    tnChild.SelectedImageKey = ico;

                    string cat = PCM.tableDatas[i].Category;
                    if (cat == "")
                        cat = "(Empty)";
                    if (!treeCategory.Nodes.ContainsKey(cat))
                    {
                        TreeNode cTnChild = new TreeNode(cat);
                        cTnChild.Name = cat;
                        cTnChild.ImageKey = "category.ico";
                        cTnChild.SelectedImageKey = "category.ico";
                        treeCategory.Nodes.Add(cTnChild);
                    }
                    treeCategory.Nodes[cat].Nodes.Add(tnChild);
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void loadSegments()
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            if (tabSegments.Controls.Contains(treeSegments))
            {
                treeSegments.Nodes.Clear();
            }
            else
            {
                treeSegments = new TreeView();
                setIconSize();
                treeSegments.ImageList = imageList1;
                treeSegments.CheckBoxes = true;

                treeSegments.Dock = DockStyle.Fill;
                tabSegments.Controls.Add(treeSegments);
                treeSegments.AfterSelect += Tree_AfterSelect;
                treeSegments.AfterCheck += Tree_AfterCheck;
                treeSegments.NodeMouseClick += Tree_NodeMouseClick;
            }
            TreeNode segTn;
            for (int i = 0; i < tuner.PCM.Segments.Count; i++)
            {
                segTn = new TreeNode(tuner.PCM.Segments[i].Name);
                segTn.Name = tuner.PCM.Segments[i].Name;
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
                treeSegments.Nodes.Add(segTn);
            }

            for (int i = 0; i < PCM.tableDatas.Count; i++)
            {
                if (PCM.tableDatas[i].TableName.ToLower().Contains(txtFilter.Text.ToLower()))
                {

                    TreeNode tnChild = new TreeNode(PCM.tableDatas[i].TableName);
                    tnChild.Tag = i;
                    string ico = "";
                    TableValueType vt = getValueType(PCM.tableDatas[i]);
                    if (PCM.tableDatas[i].BitMask != null && PCM.tableDatas[i].BitMask.Length > 0)
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

                    if (PCM.tableDatas[i].Rows == 1 && PCM.tableDatas[i].Columns == 1)
                    {
                        ico += "1d.ico";
                    }
                    else if (PCM.tableDatas[i].Rows > 1 && PCM.tableDatas[i].Columns == 1)
                    {
                        ico += "2d.ico";
                    }
                    else
                    {
                        ico += "3d.ico";
                    }

                    tnChild.ImageKey = ico;
                    tnChild.SelectedImageKey = ico;

                    string cat = PCM.tableDatas[i].Category;
                    if (cat == "")
                        cat = "(Empty)";

                    int seg = tuner.PCM.GetSegmentNumber(PCM.tableDatas[i].addrInt);
                    if (seg > -1)
                    {
                        if (!Properties.Settings.Default.TableExplorerUseCategorySubfolder)
                        {
                            treeSegments.Nodes[seg].Nodes.Add(tnChild);
                        }
                        else
                        {
                            if (!treeSegments.Nodes[seg].Nodes.ContainsKey(cat))
                            {
                                TreeNode tnNew = new TreeNode(cat);
                                tnNew.Name = cat;
                                tnNew.ImageKey = "category.ico";
                                tnNew.SelectedImageKey = "category.ico";
                                treeSegments.Nodes[seg].Nodes.Add(tnNew);
                            }
                            treeSegments.Nodes[seg].Nodes[cat].Nodes.Add(tnChild);
                        }
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void filterTables()
        {
            clearPanel2();            
            if (tabControl1.SelectedTab.Name == "tabDimensions")
                loadDimensions();
            else if (tabControl1.SelectedTab.Name == "tabValueType")
                loadValueTypes();
            else if (tabControl1.SelectedTab.Name == "tabCategory")
                loadCategories();
            else if (tabControl1.SelectedTab.Name == "tabSegments")
                loadSegments();
        }

        private void showCategorySubfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showCategorySubfolderToolStripMenuItem.Checked = !showCategorySubfolderToolStripMenuItem.Checked;
            Properties.Settings.Default.TableExplorerUseCategorySubfolder = showCategorySubfolderToolStripMenuItem.Checked;
            filterTables();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            keyDelayCounter = 0;
            timerFilter.Enabled = true;

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
            try
            {
                TreeView tree = (TreeView)tabControl1.SelectedTab.Controls[0];
                tree.ExpandAll();
            }
            catch { };
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            try
            {
                TreeView tree = (TreeView)tabControl1.SelectedTab.Controls[0];
                tree.CollapseAll();
            }
            catch { };
        }

        private void clearPanel2()
        {
            foreach (var x in splitContainer1.Panel2.Controls.OfType<Form>())
            {
                x.Close();
            }
            labelTableName.Text = "";
        }

        private void openMultipleBINFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFileSelection frmF = new frmFileSelection();
            frmF.btnOK.Text = "Open files";
            frmF.LoadFiles(UniversalPatcher.Properties.Settings.Default.LastBINfolder);
            if (frmF.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < frmF.listFiles.CheckedItems.Count; i++)
                {
                    string newFile = frmF.listFiles.CheckedItems[i].Tag.ToString();
                    Logger("Opening file: " + newFile);
                    PcmFile newPCM = new PcmFile(newFile, true, PCM.configFileFullName);
                    addtoCurrentFileMenu(newPCM);
                    PCM = newPCM;
                }
                selectPCM();
            }

        }

        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeView tv = (TreeView)tabControl1.SelectedTab.Controls[0];
            if (tv.SelectedNode == null)
                return;
            if (tv.SelectedNode.Tag != null)
            {
                List<int> tableIds = new List<int>();
                tableIds.Add((int)tv.SelectedNode.Tag);
                tuner.openTableEditor(tableIds);
            }

        }
    }
}
