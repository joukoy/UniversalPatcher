using System;
using System.Windows.Forms;

namespace UniversalPatcher
{
    partial class FrmTuner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTuner));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.comboTableCategory = new System.Windows.Forms.ComboBox();
            this.labelCategory = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBinAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllBINFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMultipleBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCompareBINToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadFileFromDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createProgramShortcutsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.touristToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.archiveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.archiveSessionWithCommentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createMapSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSessionAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreArchivedSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.createDebugLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caseSensitiveFilteringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableConfigAutoloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableMultitableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extraOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendToPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendFocusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorSegmentsFromCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moreSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnlyMappedTablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTablesWithEmptyAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeviewSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.sortColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveColumnsPresetAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadColumnsPresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetTunerModeColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resizeNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoresizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autosaveColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mapExtraOffsetButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableExtraoffsetButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xDFToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tinyTunerDBV6OnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMlgeneratorImportCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVexperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSV2ExperimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sGMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.intelHEXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.motorolaSrecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xDFnewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLGeneratorExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDifferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDifferencesHEXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editTableSeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.massModifyTableListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.massModifyTableListsSelectFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameDuplicateTablenamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOffsetVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoRedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.applyPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readWritePCMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTablelistxmlTableseekImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTablelistnewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveXMLAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveExtraOffsetTablelistAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameTablelistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.addNewTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateAddressesByOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.swapTablenameExtratablenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undeleteTableconfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homepageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fwdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchAndCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchAndCompareAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchTablesextraoffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareSelectedTablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySelectedTablesToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.dToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.enumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.booleanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bitmaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHistogramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testExtraOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addressRelativeDiffToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateTableConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToTableseekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editOSAddressPairsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addressRelativeDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTablesToExistingPatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createPatchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.axistablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openXaxisTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openYaxisTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMathtableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editXaxisTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editYaxisTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMathtableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.comboFilterBy = new System.Windows.Forms.ComboBox();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtDescription2 = new System.Windows.Forms.RichTextBox();
            this.txtDescription = new System.Windows.Forms.RichTextBox();
            this.splitContainerListView = new System.Windows.Forms.SplitContainer();
            this.splitContainerListMode = new System.Windows.Forms.SplitContainer();
            this.splitContainerListTree = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new UniversalPatcher.TreeViewMS();
            this.contextMenuStripListTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expand2LevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expand3LevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSegmentOffsetNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupTestExtraOffset = new System.Windows.Forms.GroupBox();
            this.comboExtraOffsetResults = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkTestExtraOffsetOffsetBytes = new System.Windows.Forms.CheckBox();
            this.txtTestExtraOffset = new System.Windows.Forms.TextBox();
            this.chkTestExtraOffsetColorCoding = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkTestExtraOffsetFilterOutOfRange = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCategory = new System.Windows.Forms.TabPage();
            this.tabMultiTree = new System.Windows.Forms.TabPage();
            this.tabDimensions = new System.Windows.Forms.TabPage();
            this.tabValueType = new System.Windows.Forms.TabPage();
            this.tabSegments = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.chkShowTreeHover = new System.Windows.Forms.CheckBox();
            this.groupNavigator = new System.Windows.Forms.GroupBox();
            this.numNaviMaxTablesTotal = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numNaviMaxTablesPerNode = new System.Windows.Forms.NumericUpDown();
            this.btnExplorerFont = new System.Windows.Forms.Button();
            this.chkShowTableCount = new System.Windows.Forms.CheckBox();
            this.chkAutoMulti1d = new System.Windows.Forms.CheckBox();
            this.chkShowCategorySubfolder = new System.Windows.Forms.CheckBox();
            this.labelIconSize = new System.Windows.Forms.Label();
            this.numIconSize = new System.Windows.Forms.NumericUpDown();
            this.tabPatches = new System.Windows.Forms.TabPage();
            this.tabFileInfo = new System.Windows.Forms.TabPage();
            this.tabControlFileInfo = new System.Windows.Forms.TabControl();
            this.imageListPng = new System.Windows.Forms.ImageList(this.components);
            this.groupUndeleteTableConfig = new System.Windows.Forms.GroupBox();
            this.btnCloseUndelete = new System.Windows.Forms.Button();
            this.btnRestoreTableConfig = new System.Windows.Forms.Button();
            this.dataGridTrashBin = new System.Windows.Forms.DataGridView();
            this.contextMenuTrashBin = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerFilter = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.labelTableName = new System.Windows.Forms.Label();
            this.labelBy = new System.Windows.Forms.Label();
            this.contextMenuStripTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInNewWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInListModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToAxistableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareSelectedTablesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripPatch = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyPatchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.txtMath = new System.Windows.Forms.TextBox();
            this.groupExtraOffset = new System.Windows.Forms.GroupBox();
            this.btnShowExtraOffsetSearch = new System.Windows.Forms.Button();
            this.btnCancelExtraOffset = new System.Windows.Forms.Button();
            this.btnTestNextTable = new System.Windows.Forms.Button();
            this.btnTestPrevTable = new System.Windows.Forms.Button();
            this.btnExtraOffsetCopyFromTest = new System.Windows.Forms.Button();
            this.btnExtraOffsetCopyToTest = new System.Windows.Forms.Button();
            this.btnExtraOffsetTestApply = new System.Windows.Forms.Button();
            this.numExtraOffsetTest = new System.Windows.Forms.NumericUpDown();
            this.btnExtraOffsetTest = new System.Windows.Forms.Button();
            this.numExtraOffset = new System.Windows.Forms.NumericUpDown();
            this.btnExtraOffsetPrev = new System.Windows.Forms.Button();
            this.imageListPngSmall = new System.Windows.Forms.ImageList(this.components);
            this.btnExtraOffsetNext = new System.Windows.Forms.Button();
            this.labelMatch = new System.Windows.Forms.Label();
            this.radioListMode = new System.Windows.Forms.RadioButton();
            this.contextMenuRestorePath = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restorePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treemodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listmodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radioTreeMode = new System.Windows.Forms.RadioButton();
            this.imageListPngBig = new System.Windows.Forms.ImageList(this.components);
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnFlash = new System.Windows.Forms.Button();
            this.btnCollapse = new System.Windows.Forms.Button();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnCollapseTree = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListView)).BeginInit();
            this.splitContainerListView.Panel1.SuspendLayout();
            this.splitContainerListView.Panel2.SuspendLayout();
            this.splitContainerListView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListMode)).BeginInit();
            this.splitContainerListMode.Panel1.SuspendLayout();
            this.splitContainerListMode.Panel2.SuspendLayout();
            this.splitContainerListMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListTree)).BeginInit();
            this.splitContainerListTree.Panel1.SuspendLayout();
            this.splitContainerListTree.Panel2.SuspendLayout();
            this.splitContainerListTree.SuspendLayout();
            this.contextMenuStripListTree.SuspendLayout();
            this.groupTestExtraOffset.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.groupNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNaviMaxTablesTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNaviMaxTablesPerNode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIconSize)).BeginInit();
            this.tabFileInfo.SuspendLayout();
            this.groupUndeleteTableConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTrashBin)).BeginInit();
            this.contextMenuTrashBin.SuspendLayout();
            this.contextMenuStripTree.SuspendLayout();
            this.contextMenuStripPatch.SuspendLayout();
            this.groupExtraOffset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffsetTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset)).BeginInit();
            this.contextMenuRestorePath.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(570, 395);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            this.dataGridView1.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView1_DataError);
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DataGridView1_UserDeletingRow);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataGridView1_MouseDown);
            this.dataGridView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DataGridView1_MouseUp);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(50, 34);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(107, 20);
            this.txtFilter.TabIndex = 14;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // comboTableCategory
            // 
            this.comboTableCategory.DropDownWidth = 300;
            this.comboTableCategory.FormattingEnabled = true;
            this.comboTableCategory.Location = new System.Drawing.Point(375, 34);
            this.comboTableCategory.Name = "comboTableCategory";
            this.comboTableCategory.Size = new System.Drawing.Size(133, 21);
            this.comboTableCategory.TabIndex = 13;
            // 
            // labelCategory
            // 
            this.labelCategory.AutoSize = true;
            this.labelCategory.Location = new System.Drawing.Point(317, 38);
            this.labelCategory.Name = "labelCategory";
            this.labelCategory.Size = new System.Drawing.Size(52, 13);
            this.labelCategory.TabIndex = 12;
            this.labelCategory.Text = "Category:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.currentFileToolStripMenuItem,
            this.tableListToolStripMenuItem,
            this.utilitiesToolStripMenuItem,
            this.xmlToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.revToolStripMenuItem,
            this.fwdToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1066, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadBINToolStripMenuItem,
            this.saveBINToolStripMenuItem,
            this.saveBinAsToolStripMenuItem,
            this.saveAllBINFilesToolStripMenuItem,
            this.openMultipleBINToolStripMenuItem,
            this.openCompareBINToolStripMenuItem,
            this.reloadFileFromDiskToolStripMenuItem,
            this.createProgramShortcutsToolStripMenuItem,
            this.modeToolStripMenuItem,
            this.sessionToolStripMenuItem,
            this.toolStripSeparator7,
            this.createDebugLogToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadBINToolStripMenuItem
            // 
            this.loadBINToolStripMenuItem.Name = "loadBINToolStripMenuItem";
            this.loadBINToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.loadBINToolStripMenuItem.Text = "Open BIN";
            this.loadBINToolStripMenuItem.Click += new System.EventHandler(this.loadBINToolStripMenuItem_Click);
            // 
            // saveBINToolStripMenuItem
            // 
            this.saveBINToolStripMenuItem.Name = "saveBINToolStripMenuItem";
            this.saveBINToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.saveBINToolStripMenuItem.Text = "&Save BIN";
            this.saveBINToolStripMenuItem.Click += new System.EventHandler(this.saveBINToolStripMenuItem_Click);
            // 
            // saveBinAsToolStripMenuItem
            // 
            this.saveBinAsToolStripMenuItem.Name = "saveBinAsToolStripMenuItem";
            this.saveBinAsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.saveBinAsToolStripMenuItem.Text = "Save BIN &As...";
            this.saveBinAsToolStripMenuItem.Click += new System.EventHandler(this.saveBinAsToolStripMenuItem_Click);
            // 
            // saveAllBINFilesToolStripMenuItem
            // 
            this.saveAllBINFilesToolStripMenuItem.Name = "saveAllBINFilesToolStripMenuItem";
            this.saveAllBINFilesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.saveAllBINFilesToolStripMenuItem.Text = "Save All BIN files";
            this.saveAllBINFilesToolStripMenuItem.Click += new System.EventHandler(this.saveAllBINFilesToolStripMenuItem_Click);
            // 
            // openMultipleBINToolStripMenuItem
            // 
            this.openMultipleBINToolStripMenuItem.Name = "openMultipleBINToolStripMenuItem";
            this.openMultipleBINToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openMultipleBINToolStripMenuItem.Text = "Open multiple BIN";
            this.openMultipleBINToolStripMenuItem.Click += new System.EventHandler(this.openMultipleBINToolStripMenuItem_Click);
            // 
            // openCompareBINToolStripMenuItem
            // 
            this.openCompareBINToolStripMenuItem.Name = "openCompareBINToolStripMenuItem";
            this.openCompareBINToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openCompareBINToolStripMenuItem.Text = "Open Compare BIN";
            this.openCompareBINToolStripMenuItem.Click += new System.EventHandler(this.openCompareBINToolStripMenuItem_Click);
            // 
            // reloadFileFromDiskToolStripMenuItem
            // 
            this.reloadFileFromDiskToolStripMenuItem.Name = "reloadFileFromDiskToolStripMenuItem";
            this.reloadFileFromDiskToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.reloadFileFromDiskToolStripMenuItem.Text = "Reload file from disk";
            this.reloadFileFromDiskToolStripMenuItem.Click += new System.EventHandler(this.reloadFileFromDiskToolStripMenuItem_Click);
            // 
            // createProgramShortcutsToolStripMenuItem
            // 
            this.createProgramShortcutsToolStripMenuItem.Name = "createProgramShortcutsToolStripMenuItem";
            this.createProgramShortcutsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.createProgramShortcutsToolStripMenuItem.Text = "Create program shortcuts";
            this.createProgramShortcutsToolStripMenuItem.Click += new System.EventHandler(this.createProgramShortcutsToolStripMenuItem_Click);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.touristToolStripMenuItem,
            this.basicToolStripMenuItem,
            this.advancedToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // touristToolStripMenuItem
            // 
            this.touristToolStripMenuItem.Name = "touristToolStripMenuItem";
            this.touristToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.touristToolStripMenuItem.Text = "Tourist";
            this.touristToolStripMenuItem.Click += new System.EventHandler(this.touristToolStripMenuItem_Click);
            // 
            // basicToolStripMenuItem
            // 
            this.basicToolStripMenuItem.Name = "basicToolStripMenuItem";
            this.basicToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.basicToolStripMenuItem.Text = "Basic";
            this.basicToolStripMenuItem.Click += new System.EventHandler(this.basicToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.advancedToolStripMenuItem.Text = "Advanced";
            this.advancedToolStripMenuItem.Click += new System.EventHandler(this.advancedToolStripMenuItem_Click);
            // 
            // sessionToolStripMenuItem
            // 
            this.sessionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archiveSessionToolStripMenuItem,
            this.archiveSessionWithCommentsToolStripMenuItem,
            this.closeSessionToolStripMenuItem,
            this.createMapSessionToolStripMenuItem,
            this.loadSessionToolStripMenuItem,
            this.newSessionToolStripMenuItem,
            this.saveSessionToolStripMenuItem,
            this.saveSessionAsToolStripMenuItem,
            this.restoreArchivedSessionToolStripMenuItem});
            this.sessionToolStripMenuItem.Name = "sessionToolStripMenuItem";
            this.sessionToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.sessionToolStripMenuItem.Text = "Session";
            // 
            // archiveSessionToolStripMenuItem
            // 
            this.archiveSessionToolStripMenuItem.Name = "archiveSessionToolStripMenuItem";
            this.archiveSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.archiveSessionToolStripMenuItem.Text = "Archive session";
            this.archiveSessionToolStripMenuItem.Click += new System.EventHandler(this.archiveSessionToolStripMenuItem_Click);
            // 
            // archiveSessionWithCommentsToolStripMenuItem
            // 
            this.archiveSessionWithCommentsToolStripMenuItem.Name = "archiveSessionWithCommentsToolStripMenuItem";
            this.archiveSessionWithCommentsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.archiveSessionWithCommentsToolStripMenuItem.Text = "Archive session with comments";
            this.archiveSessionWithCommentsToolStripMenuItem.Click += new System.EventHandler(this.archiveSessionWithCommentsToolStripMenuItem_Click);
            // 
            // closeSessionToolStripMenuItem
            // 
            this.closeSessionToolStripMenuItem.Name = "closeSessionToolStripMenuItem";
            this.closeSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.closeSessionToolStripMenuItem.Text = "Close session";
            this.closeSessionToolStripMenuItem.Click += new System.EventHandler(this.closeSessionToolStripMenuItem_Click);
            // 
            // createMapSessionToolStripMenuItem
            // 
            this.createMapSessionToolStripMenuItem.Name = "createMapSessionToolStripMenuItem";
            this.createMapSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.createMapSessionToolStripMenuItem.Text = "Create map session";
            this.createMapSessionToolStripMenuItem.Click += new System.EventHandler(this.createMapSessionToolStripMenuItem_Click);
            // 
            // loadSessionToolStripMenuItem
            // 
            this.loadSessionToolStripMenuItem.Name = "loadSessionToolStripMenuItem";
            this.loadSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.loadSessionToolStripMenuItem.Text = "Load session";
            this.loadSessionToolStripMenuItem.Click += new System.EventHandler(this.loadSessionToolStripMenuItem_Click);
            // 
            // newSessionToolStripMenuItem
            // 
            this.newSessionToolStripMenuItem.Name = "newSessionToolStripMenuItem";
            this.newSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.newSessionToolStripMenuItem.Text = "New session";
            this.newSessionToolStripMenuItem.Click += new System.EventHandler(this.newSessionToolStripMenuItem_Click);
            // 
            // saveSessionToolStripMenuItem
            // 
            this.saveSessionToolStripMenuItem.Name = "saveSessionToolStripMenuItem";
            this.saveSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.saveSessionToolStripMenuItem.Text = "Save session";
            this.saveSessionToolStripMenuItem.Click += new System.EventHandler(this.saveSessionToolStripMenuItem_Click);
            // 
            // saveSessionAsToolStripMenuItem
            // 
            this.saveSessionAsToolStripMenuItem.Name = "saveSessionAsToolStripMenuItem";
            this.saveSessionAsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.saveSessionAsToolStripMenuItem.Text = "Save session as...";
            this.saveSessionAsToolStripMenuItem.Click += new System.EventHandler(this.saveSessionAsToolStripMenuItem_Click);
            // 
            // restoreArchivedSessionToolStripMenuItem
            // 
            this.restoreArchivedSessionToolStripMenuItem.Name = "restoreArchivedSessionToolStripMenuItem";
            this.restoreArchivedSessionToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.restoreArchivedSessionToolStripMenuItem.Text = "Restore archived session";
            this.restoreArchivedSessionToolStripMenuItem.Click += new System.EventHandler(this.restoreArchivedSessionToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(206, 6);
            // 
            // createDebugLogToolStripMenuItem
            // 
            this.createDebugLogToolStripMenuItem.Name = "createDebugLogToolStripMenuItem";
            this.createDebugLogToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.createDebugLogToolStripMenuItem.Text = "Create debug log...";
            this.createDebugLogToolStripMenuItem.Click += new System.EventHandler(this.createDebugLogToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.caseSensitiveFilteringToolStripMenuItem,
            this.disableConfigAutoloadToolStripMenuItem,
            this.disableMultitableToolStripMenuItem,
            this.extraOffsetToolStripMenuItem,
            this.fontToolStripMenuItem,
            this.mirrorSegmentsFromCompareToolStripMenuItem,
            this.moreSettingsToolStripMenuItem,
            this.allSettingsToolStripMenuItem,
            this.showOnlyMappedTablesToolStripMenuItem,
            this.showTablesWithEmptyAddressToolStripMenuItem,
            this.treeviewSettingsToolStripMenuItem,
            this.unitsToolStripMenuItem,
            this.toolStripSeparator8,
            this.sortColumnsToolStripMenuItem,
            this.saveColumnsPresetAsToolStripMenuItem,
            this.loadColumnsPresetToolStripMenuItem,
            this.resetTunerModeColumnsToolStripMenuItem,
            this.resizeColumnsToolStripMenuItem,
            this.autosaveColumnsToolStripMenuItem,
            this.toolStripSeparator4,
            this.mapExtraOffsetButtonsToolStripMenuItem,
            this.enableExtraoffsetButtonsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // caseSensitiveFilteringToolStripMenuItem
            // 
            this.caseSensitiveFilteringToolStripMenuItem.Name = "caseSensitiveFilteringToolStripMenuItem";
            this.caseSensitiveFilteringToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.caseSensitiveFilteringToolStripMenuItem.Text = "Case sensitive filtering";
            this.caseSensitiveFilteringToolStripMenuItem.Click += new System.EventHandler(this.caseSensitiveFilteringToolStripMenuItem_Click);
            // 
            // disableConfigAutoloadToolStripMenuItem
            // 
            this.disableConfigAutoloadToolStripMenuItem.Name = "disableConfigAutoloadToolStripMenuItem";
            this.disableConfigAutoloadToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.disableConfigAutoloadToolStripMenuItem.Text = "Disable config autoload";
            this.disableConfigAutoloadToolStripMenuItem.Click += new System.EventHandler(this.disableConfigAutoloadToolStripMenuItem_Click);
            // 
            // disableMultitableToolStripMenuItem
            // 
            this.disableMultitableToolStripMenuItem.Name = "disableMultitableToolStripMenuItem";
            this.disableMultitableToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.disableMultitableToolStripMenuItem.Text = "Disable multitable";
            this.disableMultitableToolStripMenuItem.Click += new System.EventHandler(this.disableMultitableToolStripMenuItem_Click);
            // 
            // extraOffsetToolStripMenuItem
            // 
            this.extraOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearPreviewToolStripMenuItem,
            this.appendToPreviewToolStripMenuItem,
            this.appendFocusToolStripMenuItem});
            this.extraOffsetToolStripMenuItem.Name = "extraOffsetToolStripMenuItem";
            this.extraOffsetToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.extraOffsetToolStripMenuItem.Text = "Extra offset +/-";
            // 
            // clearPreviewToolStripMenuItem
            // 
            this.clearPreviewToolStripMenuItem.Checked = true;
            this.clearPreviewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clearPreviewToolStripMenuItem.Name = "clearPreviewToolStripMenuItem";
            this.clearPreviewToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.clearPreviewToolStripMenuItem.Text = "Clear preview";
            this.clearPreviewToolStripMenuItem.Click += new System.EventHandler(this.clearPreviewToolStripMenuItem_Click);
            // 
            // appendToPreviewToolStripMenuItem
            // 
            this.appendToPreviewToolStripMenuItem.Name = "appendToPreviewToolStripMenuItem";
            this.appendToPreviewToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.appendToPreviewToolStripMenuItem.Text = "Append to preview";
            this.appendToPreviewToolStripMenuItem.Click += new System.EventHandler(this.appendToPreviewToolStripMenuItem_Click);
            // 
            // appendFocusToolStripMenuItem
            // 
            this.appendFocusToolStripMenuItem.Name = "appendFocusToolStripMenuItem";
            this.appendFocusToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.appendFocusToolStripMenuItem.Text = "Append + focus";
            this.appendFocusToolStripMenuItem.Click += new System.EventHandler(this.appendFocusToolStripMenuItem_Click);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.fontToolStripMenuItem.Text = "Font";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // mirrorSegmentsFromCompareToolStripMenuItem
            // 
            this.mirrorSegmentsFromCompareToolStripMenuItem.Name = "mirrorSegmentsFromCompareToolStripMenuItem";
            this.mirrorSegmentsFromCompareToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.mirrorSegmentsFromCompareToolStripMenuItem.Text = "Mirror segments from compare";
            this.mirrorSegmentsFromCompareToolStripMenuItem.Click += new System.EventHandler(this.mirrorSegmentsFromCompareToolStripMenuItem_Click);
            // 
            // moreSettingsToolStripMenuItem
            // 
            this.moreSettingsToolStripMenuItem.Name = "moreSettingsToolStripMenuItem";
            this.moreSettingsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.moreSettingsToolStripMenuItem.Text = "More settings...";
            this.moreSettingsToolStripMenuItem.Click += new System.EventHandler(this.moreSettingsToolStripMenuItem_Click);
            // 
            // allSettingsToolStripMenuItem
            // 
            this.allSettingsToolStripMenuItem.Name = "allSettingsToolStripMenuItem";
            this.allSettingsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.allSettingsToolStripMenuItem.Text = "All settings";
            this.allSettingsToolStripMenuItem.Click += new System.EventHandler(this.allSettingsToolStripMenuItem_Click);
            // 
            // showOnlyMappedTablesToolStripMenuItem
            // 
            this.showOnlyMappedTablesToolStripMenuItem.Name = "showOnlyMappedTablesToolStripMenuItem";
            this.showOnlyMappedTablesToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.showOnlyMappedTablesToolStripMenuItem.Text = "Show only mapped tables";
            this.showOnlyMappedTablesToolStripMenuItem.Click += new System.EventHandler(this.showOnlyMappedTablesToolStripMenuItem_Click);
            // 
            // showTablesWithEmptyAddressToolStripMenuItem
            // 
            this.showTablesWithEmptyAddressToolStripMenuItem.Name = "showTablesWithEmptyAddressToolStripMenuItem";
            this.showTablesWithEmptyAddressToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.showTablesWithEmptyAddressToolStripMenuItem.Text = "Show tables with empty address";
            this.showTablesWithEmptyAddressToolStripMenuItem.Click += new System.EventHandler(this.showTablesWithEmptyAddressToolStripMenuItem_Click);
            // 
            // treeviewSettingsToolStripMenuItem
            // 
            this.treeviewSettingsToolStripMenuItem.Name = "treeviewSettingsToolStripMenuItem";
            this.treeviewSettingsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.treeviewSettingsToolStripMenuItem.Text = "Treeview settings";
            this.treeviewSettingsToolStripMenuItem.Click += new System.EventHandler(this.treeviewSettingsToolStripMenuItem_Click);
            // 
            // unitsToolStripMenuItem
            // 
            this.unitsToolStripMenuItem.Name = "unitsToolStripMenuItem";
            this.unitsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.unitsToolStripMenuItem.Text = "Units";
            this.unitsToolStripMenuItem.Click += new System.EventHandler(this.unitsToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(240, 6);
            // 
            // sortColumnsToolStripMenuItem
            // 
            this.sortColumnsToolStripMenuItem.Name = "sortColumnsToolStripMenuItem";
            this.sortColumnsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.sortColumnsToolStripMenuItem.Text = "Select/Sort Columns";
            this.sortColumnsToolStripMenuItem.Click += new System.EventHandler(this.sortColumnsToolStripMenuItem_Click);
            // 
            // saveColumnsPresetAsToolStripMenuItem
            // 
            this.saveColumnsPresetAsToolStripMenuItem.Name = "saveColumnsPresetAsToolStripMenuItem";
            this.saveColumnsPresetAsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.saveColumnsPresetAsToolStripMenuItem.Text = "Save columns preset as...";
            this.saveColumnsPresetAsToolStripMenuItem.Click += new System.EventHandler(this.saveColumnsPresetAsToolStripMenuItem_Click);
            // 
            // loadColumnsPresetToolStripMenuItem
            // 
            this.loadColumnsPresetToolStripMenuItem.Name = "loadColumnsPresetToolStripMenuItem";
            this.loadColumnsPresetToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.loadColumnsPresetToolStripMenuItem.Text = "Load columns preset";
            this.loadColumnsPresetToolStripMenuItem.Click += new System.EventHandler(this.loadColumnsPresetToolStripMenuItem_Click);
            // 
            // resetTunerModeColumnsToolStripMenuItem
            // 
            this.resetTunerModeColumnsToolStripMenuItem.Name = "resetTunerModeColumnsToolStripMenuItem";
            this.resetTunerModeColumnsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.resetTunerModeColumnsToolStripMenuItem.Text = "Reset columns";
            this.resetTunerModeColumnsToolStripMenuItem.Click += new System.EventHandler(this.resetTunerModeColumnsToolStripMenuItem_Click);
            // 
            // resizeColumnsToolStripMenuItem
            // 
            this.resizeColumnsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resizeNowToolStripMenuItem,
            this.autoresizeToolStripMenuItem});
            this.resizeColumnsToolStripMenuItem.Name = "resizeColumnsToolStripMenuItem";
            this.resizeColumnsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.resizeColumnsToolStripMenuItem.Text = "Resize columns";
            // 
            // resizeNowToolStripMenuItem
            // 
            this.resizeNowToolStripMenuItem.Name = "resizeNowToolStripMenuItem";
            this.resizeNowToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.resizeNowToolStripMenuItem.Text = "Resize now";
            this.resizeNowToolStripMenuItem.Click += new System.EventHandler(this.resizeNowToolStripMenuItem_Click);
            // 
            // autoresizeToolStripMenuItem
            // 
            this.autoresizeToolStripMenuItem.Name = "autoresizeToolStripMenuItem";
            this.autoresizeToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.autoresizeToolStripMenuItem.Text = "Autoresize";
            this.autoresizeToolStripMenuItem.Click += new System.EventHandler(this.autoresizeToolStripMenuItem_Click);
            // 
            // autosaveColumnsToolStripMenuItem
            // 
            this.autosaveColumnsToolStripMenuItem.Name = "autosaveColumnsToolStripMenuItem";
            this.autosaveColumnsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.autosaveColumnsToolStripMenuItem.Text = "Autosave columns";
            this.autosaveColumnsToolStripMenuItem.Click += new System.EventHandler(this.autosaveColumnsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(240, 6);
            // 
            // mapExtraOffsetButtonsToolStripMenuItem
            // 
            this.mapExtraOffsetButtonsToolStripMenuItem.Name = "mapExtraOffsetButtonsToolStripMenuItem";
            this.mapExtraOffsetButtonsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.mapExtraOffsetButtonsToolStripMenuItem.Text = "Map Extraoffset buttons";
            this.mapExtraOffsetButtonsToolStripMenuItem.Click += new System.EventHandler(this.mapExtraOffsetButtonsToolStripMenuItem_Click);
            // 
            // enableExtraoffsetButtonsToolStripMenuItem
            // 
            this.enableExtraoffsetButtonsToolStripMenuItem.Name = "enableExtraoffsetButtonsToolStripMenuItem";
            this.enableExtraoffsetButtonsToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.enableExtraoffsetButtonsToolStripMenuItem.Text = "Enable Extraoffset Buttons";
            this.enableExtraoffsetButtonsToolStripMenuItem.Click += new System.EventHandler(this.enableExtraoffsetButtonsToolStripMenuItem_Click);
            // 
            // currentFileToolStripMenuItem
            // 
            this.currentFileToolStripMenuItem.Name = "currentFileToolStripMenuItem";
            this.currentFileToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.currentFileToolStripMenuItem.Text = "BIN file";
            // 
            // tableListToolStripMenuItem
            // 
            this.tableListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem});
            this.tableListToolStripMenuItem.Name = "tableListToolStripMenuItem";
            this.tableListToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.tableListToolStripMenuItem.Text = "Tablelist";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // utilitiesToolStripMenuItem
            // 
            this.utilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.exportToolStripMenuItem1,
            this.findDifferencesToolStripMenuItem,
            this.findDifferencesHEXToolStripMenuItem,
            this.editTableSeekToolStripMenuItem,
            this.massModifyTableListsToolStripMenuItem,
            this.massModifyTableListsSelectFilesToolStripMenuItem,
            this.renameDuplicateTablenamesToolStripMenuItem,
            this.histogramToolStripMenuItem,
            this.showOffsetVisualizerToolStripMenuItem,
            this.undoRedoToolStripMenuItem,
            this.toolStripSeparator6,
            this.applyPatchToolStripMenuItem,
            this.launcherToolStripMenuItem,
            this.loggerToolStripMenuItem,
            this.patcherToolStripMenuItem,
            this.readWritePCMToolStripMenuItem,
            this.swapSegmentsToolStripMenuItem});
            this.utilitiesToolStripMenuItem.Name = "utilitiesToolStripMenuItem";
            this.utilitiesToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.utilitiesToolStripMenuItem.Text = "Utilities";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dTCToolStripMenuItem,
            this.tableSeekToolStripMenuItem,
            this.xDFToolStripMenuItem1,
            this.tinyTunerDBV6OnlyToolStripMenuItem,
            this.xMlgeneratorImportCSVToolStripMenuItem,
            this.cSVexperimentalToolStripMenuItem,
            this.cSV2ExperimentalToolStripMenuItem,
            this.sGMToolStripMenuItem,
            this.intelHEXToolStripMenuItem,
            this.motorolaSrecordToolStripMenuItem});
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(233, 22);
            this.importToolStripMenuItem1.Text = "Import";
            // 
            // dTCToolStripMenuItem
            // 
            this.dTCToolStripMenuItem.Name = "dTCToolStripMenuItem";
            this.dTCToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.dTCToolStripMenuItem.Text = "DTC";
            this.dTCToolStripMenuItem.Click += new System.EventHandler(this.dTCToolStripMenuItem_Click);
            // 
            // tableSeekToolStripMenuItem
            // 
            this.tableSeekToolStripMenuItem.Name = "tableSeekToolStripMenuItem";
            this.tableSeekToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.tableSeekToolStripMenuItem.Text = "TableSeek";
            this.tableSeekToolStripMenuItem.Click += new System.EventHandler(this.tableSeekToolStripMenuItem_Click);
            // 
            // xDFToolStripMenuItem1
            // 
            this.xDFToolStripMenuItem1.Name = "xDFToolStripMenuItem1";
            this.xDFToolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.xDFToolStripMenuItem1.Text = "XDF";
            this.xDFToolStripMenuItem1.Click += new System.EventHandler(this.xDFToolStripMenuItem1_Click);
            // 
            // tinyTunerDBV6OnlyToolStripMenuItem
            // 
            this.tinyTunerDBV6OnlyToolStripMenuItem.Name = "tinyTunerDBV6OnlyToolStripMenuItem";
            this.tinyTunerDBV6OnlyToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.tinyTunerDBV6OnlyToolStripMenuItem.Text = "TinyTuner DB (V6 Only)";
            this.tinyTunerDBV6OnlyToolStripMenuItem.Click += new System.EventHandler(this.tinyTunerDBV6OnlyToolStripMenuItem_Click);
            // 
            // xMlgeneratorImportCSVToolStripMenuItem
            // 
            this.xMlgeneratorImportCSVToolStripMenuItem.Name = "xMlgeneratorImportCSVToolStripMenuItem";
            this.xMlgeneratorImportCSVToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.xMlgeneratorImportCSVToolStripMenuItem.Text = "XML-generator CSV";
            this.xMlgeneratorImportCSVToolStripMenuItem.Click += new System.EventHandler(this.xMlgeneratorImportCSVToolStripMenuItem_Click);
            // 
            // cSVexperimentalToolStripMenuItem
            // 
            this.cSVexperimentalToolStripMenuItem.Name = "cSVexperimentalToolStripMenuItem";
            this.cSVexperimentalToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.cSVexperimentalToolStripMenuItem.Text = "CSV (experimental)";
            this.cSVexperimentalToolStripMenuItem.Click += new System.EventHandler(this.cSVexperimentalToolStripMenuItem_Click);
            // 
            // cSV2ExperimentalToolStripMenuItem
            // 
            this.cSV2ExperimentalToolStripMenuItem.Name = "cSV2ExperimentalToolStripMenuItem";
            this.cSV2ExperimentalToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.cSV2ExperimentalToolStripMenuItem.Text = "CSV2 (Experimental)";
            this.cSV2ExperimentalToolStripMenuItem.Click += new System.EventHandler(this.cSV2ExperimentalToolStripMenuItem_Click);
            // 
            // sGMToolStripMenuItem
            // 
            this.sGMToolStripMenuItem.Enabled = false;
            this.sGMToolStripMenuItem.Name = "sGMToolStripMenuItem";
            this.sGMToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.sGMToolStripMenuItem.Text = "SGM";
            this.sGMToolStripMenuItem.Click += new System.EventHandler(this.sGMToolStripMenuItem_Click);
            // 
            // intelHEXToolStripMenuItem
            // 
            this.intelHEXToolStripMenuItem.Name = "intelHEXToolStripMenuItem";
            this.intelHEXToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.intelHEXToolStripMenuItem.Text = "Intel HEX";
            this.intelHEXToolStripMenuItem.Click += new System.EventHandler(this.intelHEXToolStripMenuItem_Click);
            // 
            // motorolaSrecordToolStripMenuItem
            // 
            this.motorolaSrecordToolStripMenuItem.Name = "motorolaSrecordToolStripMenuItem";
            this.motorolaSrecordToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.motorolaSrecordToolStripMenuItem.Text = "Motorola S-record";
            this.motorolaSrecordToolStripMenuItem.Click += new System.EventHandler(this.motorolaSrecordToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cSVToolStripMenuItem,
            this.xDFnewToolStripMenuItem,
            this.xMLGeneratorExportToolStripMenuItem});
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(233, 22);
            this.exportToolStripMenuItem1.Text = "Export";
            // 
            // cSVToolStripMenuItem
            // 
            this.cSVToolStripMenuItem.Name = "cSVToolStripMenuItem";
            this.cSVToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.cSVToolStripMenuItem.Text = "CSV";
            this.cSVToolStripMenuItem.Click += new System.EventHandler(this.cSVToolStripMenuItem_Click);
            // 
            // xDFnewToolStripMenuItem
            // 
            this.xDFnewToolStripMenuItem.Name = "xDFnewToolStripMenuItem";
            this.xDFnewToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.xDFnewToolStripMenuItem.Text = "XDF";
            this.xDFnewToolStripMenuItem.Click += new System.EventHandler(this.xDFnewToolStripMenuItem_Click);
            // 
            // xMLGeneratorExportToolStripMenuItem
            // 
            this.xMLGeneratorExportToolStripMenuItem.Name = "xMLGeneratorExportToolStripMenuItem";
            this.xMLGeneratorExportToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.xMLGeneratorExportToolStripMenuItem.Text = "XML Generator CSV";
            this.xMLGeneratorExportToolStripMenuItem.Click += new System.EventHandler(this.xMLGeneratorExportToolStripMenuItem_Click);
            // 
            // findDifferencesToolStripMenuItem
            // 
            this.findDifferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFileToolStripMenuItem});
            this.findDifferencesToolStripMenuItem.Name = "findDifferencesToolStripMenuItem";
            this.findDifferencesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.findDifferencesToolStripMenuItem.Text = "Find differences";
            // 
            // selectFileToolStripMenuItem
            // 
            this.selectFileToolStripMenuItem.Name = "selectFileToolStripMenuItem";
            this.selectFileToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.selectFileToolStripMenuItem.Text = "Select file...";
            this.selectFileToolStripMenuItem.Click += new System.EventHandler(this.selectFileToolStripMenuItem_Click);
            // 
            // findDifferencesHEXToolStripMenuItem
            // 
            this.findDifferencesHEXToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFileToolStripMenuItem1});
            this.findDifferencesHEXToolStripMenuItem.Name = "findDifferencesHEXToolStripMenuItem";
            this.findDifferencesHEXToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.findDifferencesHEXToolStripMenuItem.Text = "Find differences (HEX)";
            // 
            // selectFileToolStripMenuItem1
            // 
            this.selectFileToolStripMenuItem1.Name = "selectFileToolStripMenuItem1";
            this.selectFileToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
            this.selectFileToolStripMenuItem1.Text = "Select file...";
            this.selectFileToolStripMenuItem1.Click += new System.EventHandler(this.selectFileToolStripMenuItem1_Click);
            // 
            // editTableSeekToolStripMenuItem
            // 
            this.editTableSeekToolStripMenuItem.Name = "editTableSeekToolStripMenuItem";
            this.editTableSeekToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.editTableSeekToolStripMenuItem.Text = "Edit TableSeek";
            this.editTableSeekToolStripMenuItem.Click += new System.EventHandler(this.editTableSeekToolStripMenuItem_Click);
            // 
            // massModifyTableListsToolStripMenuItem
            // 
            this.massModifyTableListsToolStripMenuItem.Name = "massModifyTableListsToolStripMenuItem";
            this.massModifyTableListsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.massModifyTableListsToolStripMenuItem.Text = "TableList editor";
            this.massModifyTableListsToolStripMenuItem.Click += new System.EventHandler(this.massModifyTableListsToolStripMenuItem_Click);
            // 
            // massModifyTableListsSelectFilesToolStripMenuItem
            // 
            this.massModifyTableListsSelectFilesToolStripMenuItem.Name = "massModifyTableListsSelectFilesToolStripMenuItem";
            this.massModifyTableListsSelectFilesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.massModifyTableListsSelectFilesToolStripMenuItem.Text = "TableList editor (Select files)";
            this.massModifyTableListsSelectFilesToolStripMenuItem.Click += new System.EventHandler(this.massModifyTableListsSelectFilesToolStripMenuItem_Click);
            // 
            // renameDuplicateTablenamesToolStripMenuItem
            // 
            this.renameDuplicateTablenamesToolStripMenuItem.Name = "renameDuplicateTablenamesToolStripMenuItem";
            this.renameDuplicateTablenamesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.renameDuplicateTablenamesToolStripMenuItem.Text = "Rename duplicate tablenames";
            this.renameDuplicateTablenamesToolStripMenuItem.Click += new System.EventHandler(this.renameDuplicateTablenamesToolStripMenuItem_Click);
            // 
            // histogramToolStripMenuItem
            // 
            this.histogramToolStripMenuItem.Name = "histogramToolStripMenuItem";
            this.histogramToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.histogramToolStripMenuItem.Text = "Show Histogram";
            this.histogramToolStripMenuItem.Click += new System.EventHandler(this.histogramToolStripMenuItem_Click);
            // 
            // showOffsetVisualizerToolStripMenuItem
            // 
            this.showOffsetVisualizerToolStripMenuItem.Name = "showOffsetVisualizerToolStripMenuItem";
            this.showOffsetVisualizerToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.showOffsetVisualizerToolStripMenuItem.Text = "Show offset visualizer";
            this.showOffsetVisualizerToolStripMenuItem.Click += new System.EventHandler(this.showOffsetVisualizerToolStripMenuItem_Click);
            // 
            // undoRedoToolStripMenuItem
            // 
            this.undoRedoToolStripMenuItem.Name = "undoRedoToolStripMenuItem";
            this.undoRedoToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.undoRedoToolStripMenuItem.Text = "Undo/Redo";
            this.undoRedoToolStripMenuItem.Click += new System.EventHandler(this.undoRedoToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(230, 6);
            // 
            // applyPatchToolStripMenuItem
            // 
            this.applyPatchToolStripMenuItem.Name = "applyPatchToolStripMenuItem";
            this.applyPatchToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.applyPatchToolStripMenuItem.Text = "Apply patch...";
            this.applyPatchToolStripMenuItem.Click += new System.EventHandler(this.applyPatchToolStripMenuItem_Click);
            // 
            // launcherToolStripMenuItem
            // 
            this.launcherToolStripMenuItem.Name = "launcherToolStripMenuItem";
            this.launcherToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.launcherToolStripMenuItem.Text = "Launcher";
            this.launcherToolStripMenuItem.Click += new System.EventHandler(this.launcherToolStripMenuItem_Click);
            // 
            // loggerToolStripMenuItem
            // 
            this.loggerToolStripMenuItem.Name = "loggerToolStripMenuItem";
            this.loggerToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.loggerToolStripMenuItem.Text = "Logger";
            this.loggerToolStripMenuItem.Click += new System.EventHandler(this.loggerToolStripMenuItem_Click);
            // 
            // patcherToolStripMenuItem
            // 
            this.patcherToolStripMenuItem.Name = "patcherToolStripMenuItem";
            this.patcherToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.patcherToolStripMenuItem.Text = "Patcher";
            this.patcherToolStripMenuItem.Click += new System.EventHandler(this.patcherToolStripMenuItem_Click_1);
            // 
            // readWritePCMToolStripMenuItem
            // 
            this.readWritePCMToolStripMenuItem.Name = "readWritePCMToolStripMenuItem";
            this.readWritePCMToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.readWritePCMToolStripMenuItem.Text = "Read/Write PCM";
            this.readWritePCMToolStripMenuItem.Click += new System.EventHandler(this.readWritePCMToolStripMenuItem_Click);
            // 
            // swapSegmentsToolStripMenuItem
            // 
            this.swapSegmentsToolStripMenuItem.Name = "swapSegmentsToolStripMenuItem";
            this.swapSegmentsToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.swapSegmentsToolStripMenuItem.Text = "Swap segments...";
            this.swapSegmentsToolStripMenuItem.Click += new System.EventHandler(this.swapSegmentsToolStripMenuItem_Click);
            // 
            // xmlToolStripMenuItem
            // 
            this.xmlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadXMLToolStripMenuItem,
            this.loadTablelistxmlTableseekImportToolStripMenuItem,
            this.loadTablelistnewToolStripMenuItem,
            this.saveXMLToolStripMenuItem,
            this.saveXMLAsToolStripMenuItem,
            this.saveExtraOffsetTablelistAsToolStripMenuItem,
            this.clearTableToolStripMenuItem,
            this.renameTablelistToolStripMenuItem,
            this.toolStripSeparator5,
            this.addNewTableToolStripMenuItem,
            this.updateAddressesByOSToolStripMenuItem,
            this.swapTablenameExtratablenameToolStripMenuItem,
            this.undeleteTableconfigToolStripMenuItem});
            this.xmlToolStripMenuItem.Name = "xmlToolStripMenuItem";
            this.xmlToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.xmlToolStripMenuItem.Text = "Xml";
            // 
            // loadXMLToolStripMenuItem
            // 
            this.loadXMLToolStripMenuItem.Name = "loadXMLToolStripMenuItem";
            this.loadXMLToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.loadXMLToolStripMenuItem.Text = "Load Tablelist";
            this.loadXMLToolStripMenuItem.Click += new System.EventHandler(this.loadXMLToolStripMenuItem_Click);
            // 
            // loadTablelistxmlTableseekImportToolStripMenuItem
            // 
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Name = "loadTablelistxmlTableseekImportToolStripMenuItem";
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Text = "Load tablelist(xml+Tableseek import)";
            this.loadTablelistxmlTableseekImportToolStripMenuItem.Click += new System.EventHandler(this.loadTablelistxmlTableseekImportToolStripMenuItem_Click);
            // 
            // loadTablelistnewToolStripMenuItem
            // 
            this.loadTablelistnewToolStripMenuItem.Name = "loadTablelistnewToolStripMenuItem";
            this.loadTablelistnewToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.loadTablelistnewToolStripMenuItem.Text = "Load tablelist (new)";
            this.loadTablelistnewToolStripMenuItem.Click += new System.EventHandler(this.loadTablelistnewToolStripMenuItem_Click);
            // 
            // saveXMLToolStripMenuItem
            // 
            this.saveXMLToolStripMenuItem.Name = "saveXMLToolStripMenuItem";
            this.saveXMLToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.saveXMLToolStripMenuItem.Text = "Save Tablelist";
            this.saveXMLToolStripMenuItem.Click += new System.EventHandler(this.saveXMLToolStripMenuItem_Click);
            // 
            // saveXMLAsToolStripMenuItem
            // 
            this.saveXMLAsToolStripMenuItem.Name = "saveXMLAsToolStripMenuItem";
            this.saveXMLAsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.saveXMLAsToolStripMenuItem.Text = "Save Tablelist As...";
            this.saveXMLAsToolStripMenuItem.Click += new System.EventHandler(this.saveXMLAsToolStripMenuItem_Click);
            // 
            // saveExtraOffsetTablelistAsToolStripMenuItem
            // 
            this.saveExtraOffsetTablelistAsToolStripMenuItem.Name = "saveExtraOffsetTablelistAsToolStripMenuItem";
            this.saveExtraOffsetTablelistAsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.saveExtraOffsetTablelistAsToolStripMenuItem.Text = "Save Mapped Tables As...";
            this.saveExtraOffsetTablelistAsToolStripMenuItem.Click += new System.EventHandler(this.saveMappedTablesAsToolStripMenuItem_Click);
            // 
            // clearTableToolStripMenuItem
            // 
            this.clearTableToolStripMenuItem.Name = "clearTableToolStripMenuItem";
            this.clearTableToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.clearTableToolStripMenuItem.Text = "&Clear Tablelist";
            this.clearTableToolStripMenuItem.Click += new System.EventHandler(this.clearTableToolStripMenuItem_Click);
            // 
            // renameTablelistToolStripMenuItem
            // 
            this.renameTablelistToolStripMenuItem.Name = "renameTablelistToolStripMenuItem";
            this.renameTablelistToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.renameTablelistToolStripMenuItem.Text = "Rename tablelist";
            this.renameTablelistToolStripMenuItem.Click += new System.EventHandler(this.renameTablelistToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(268, 6);
            // 
            // addNewTableToolStripMenuItem
            // 
            this.addNewTableToolStripMenuItem.Name = "addNewTableToolStripMenuItem";
            this.addNewTableToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.addNewTableToolStripMenuItem.Text = "Add new table";
            this.addNewTableToolStripMenuItem.Click += new System.EventHandler(this.addNewTableToolStripMenuItem_Click);
            // 
            // updateAddressesByOSToolStripMenuItem
            // 
            this.updateAddressesByOSToolStripMenuItem.Name = "updateAddressesByOSToolStripMenuItem";
            this.updateAddressesByOSToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.updateAddressesByOSToolStripMenuItem.Text = "Update Addresses by OS";
            this.updateAddressesByOSToolStripMenuItem.Click += new System.EventHandler(this.updateAddressesByOSToolStripMenuItem_Click);
            // 
            // swapTablenameExtratablenameToolStripMenuItem
            // 
            this.swapTablenameExtratablenameToolStripMenuItem.Name = "swapTablenameExtratablenameToolStripMenuItem";
            this.swapTablenameExtratablenameToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.swapTablenameExtratablenameToolStripMenuItem.Text = "Swap Tablename <-> Extratablename";
            this.swapTablenameExtratablenameToolStripMenuItem.Click += new System.EventHandler(this.swapTablenameExtratablenameToolStripMenuItem_Click);
            // 
            // undeleteTableconfigToolStripMenuItem
            // 
            this.undeleteTableconfigToolStripMenuItem.Name = "undeleteTableconfigToolStripMenuItem";
            this.undeleteTableconfigToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.undeleteTableconfigToolStripMenuItem.Text = "Undelete Tableconfigs...";
            this.undeleteTableconfigToolStripMenuItem.Click += new System.EventHandler(this.undeleteTableconfigToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.creditsToolStripMenuItem,
            this.homepageToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // creditsToolStripMenuItem
            // 
            this.creditsToolStripMenuItem.Name = "creditsToolStripMenuItem";
            this.creditsToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.creditsToolStripMenuItem.Text = "Credits";
            this.creditsToolStripMenuItem.Click += new System.EventHandler(this.creditsToolStripMenuItem_Click);
            // 
            // homepageToolStripMenuItem
            // 
            this.homepageToolStripMenuItem.Name = "homepageToolStripMenuItem";
            this.homepageToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.homepageToolStripMenuItem.Text = "Homepage";
            this.homepageToolStripMenuItem.Click += new System.EventHandler(this.homepageToolStripMenuItem_Click);
            // 
            // revToolStripMenuItem
            // 
            this.revToolStripMenuItem.Name = "revToolStripMenuItem";
            this.revToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.revToolStripMenuItem.Text = "<<";
            this.revToolStripMenuItem.Click += new System.EventHandler(this.revToolStripMenuItem_Click);
            // 
            // fwdToolStripMenuItem
            // 
            this.fwdToolStripMenuItem.Name = "fwdToolStripMenuItem";
            this.fwdToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fwdToolStripMenuItem.Text = ">>";
            this.fwdToolStripMenuItem.Click += new System.EventHandler(this.fwdToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.copyRowToolStripMenuItem1,
            this.pasteRowToolStripMenuItem,
            this.editTableToolStripMenuItem,
            this.searchAndCompareToolStripMenuItem,
            this.searchAndCompareAllToolStripMenuItem,
            this.searchTablesextraoffsetToolStripMenuItem,
            this.compareSelectedTablesToolStripMenuItem,
            this.copySelectedTablesToToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.showHistogramToolStripMenuItem,
            this.testExtraOffsetToolStripMenuItem,
            this.addressRelativeDiffToolStripMenuItem1,
            this.toolStripSeparator1,
            this.editRowToolStripMenuItem,
            this.insertRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.duplicateTableConfigToolStripMenuItem,
            this.copyToTableseekToolStripMenuItem,
            this.editOSAddressPairsToolStripMenuItem,
            this.toolStripSeparator3,
            this.addressRelativeDiffToolStripMenuItem,
            this.createPatchToolStripMenuItem,
            this.addTablesToExistingPatchToolStripMenuItem,
            this.createPatchToolStripMenuItem1,
            this.toolStripSeparator2,
            this.axistablesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(231, 594);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // copyRowToolStripMenuItem1
            // 
            this.copyRowToolStripMenuItem1.Name = "copyRowToolStripMenuItem1";
            this.copyRowToolStripMenuItem1.Size = new System.Drawing.Size(230, 22);
            this.copyRowToolStripMenuItem1.Text = "Copy row";
            this.copyRowToolStripMenuItem1.Click += new System.EventHandler(this.copyRowToolStripMenuItem1_Click);
            // 
            // pasteRowToolStripMenuItem
            // 
            this.pasteRowToolStripMenuItem.Name = "pasteRowToolStripMenuItem";
            this.pasteRowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.pasteRowToolStripMenuItem.Text = "Paste row";
            this.pasteRowToolStripMenuItem.Click += new System.EventHandler(this.pasteRowToolStripMenuItem_Click);
            // 
            // editTableToolStripMenuItem
            // 
            this.editTableToolStripMenuItem.Name = "editTableToolStripMenuItem";
            this.editTableToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.editTableToolStripMenuItem.Text = "Edit table(s)";
            this.editTableToolStripMenuItem.Click += new System.EventHandler(this.editTableToolStripMenuItem_Click);
            // 
            // searchAndCompareToolStripMenuItem
            // 
            this.searchAndCompareToolStripMenuItem.Name = "searchAndCompareToolStripMenuItem";
            this.searchAndCompareToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.searchAndCompareToolStripMenuItem.Text = "Search and compare";
            this.searchAndCompareToolStripMenuItem.Click += new System.EventHandler(this.searchAndCompareToolStripMenuItem_Click);
            // 
            // searchAndCompareAllToolStripMenuItem
            // 
            this.searchAndCompareAllToolStripMenuItem.Name = "searchAndCompareAllToolStripMenuItem";
            this.searchAndCompareAllToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.searchAndCompareAllToolStripMenuItem.Text = "Search and compare All";
            this.searchAndCompareAllToolStripMenuItem.Click += new System.EventHandler(this.searchAndCompareAllToolStripMenuItem_Click);
            // 
            // searchTablesextraoffsetToolStripMenuItem
            // 
            this.searchTablesextraoffsetToolStripMenuItem.Name = "searchTablesextraoffsetToolStripMenuItem";
            this.searchTablesextraoffsetToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.searchTablesextraoffsetToolStripMenuItem.Text = "Search tables (extraoffset)";
            // 
            // compareSelectedTablesToolStripMenuItem
            // 
            this.compareSelectedTablesToolStripMenuItem.Name = "compareSelectedTablesToolStripMenuItem";
            this.compareSelectedTablesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.compareSelectedTablesToolStripMenuItem.Text = "Compare selected tables";
            this.compareSelectedTablesToolStripMenuItem.Click += new System.EventHandler(this.compareSelectedTablesToolStripMenuItem_Click);
            // 
            // copySelectedTablesToToolStripMenuItem
            // 
            this.copySelectedTablesToToolStripMenuItem.Name = "copySelectedTablesToToolStripMenuItem";
            this.copySelectedTablesToToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.copySelectedTablesToToolStripMenuItem.Text = "Copy selected tables to...";
            this.copySelectedTablesToToolStripMenuItem.Click += new System.EventHandler(this.copySelectedTablesToToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dToolStripMenuItem,
            this.dToolStripMenuItem1,
            this.dToolStripMenuItem2,
            this.enumToolStripMenuItem,
            this.booleanToolStripMenuItem,
            this.bitmaskToolStripMenuItem,
            this.numberToolStripMenuItem});
            this.selectToolStripMenuItem.Enabled = false;
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.selectToolStripMenuItem.Text = "Select all...";
            this.selectToolStripMenuItem.Visible = false;
            // 
            // dToolStripMenuItem
            // 
            this.dToolStripMenuItem.Name = "dToolStripMenuItem";
            this.dToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.dToolStripMenuItem.Text = "1d";
            // 
            // dToolStripMenuItem1
            // 
            this.dToolStripMenuItem1.Name = "dToolStripMenuItem1";
            this.dToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.dToolStripMenuItem1.Text = "2d";
            // 
            // dToolStripMenuItem2
            // 
            this.dToolStripMenuItem2.Name = "dToolStripMenuItem2";
            this.dToolStripMenuItem2.Size = new System.Drawing.Size(117, 22);
            this.dToolStripMenuItem2.Text = "3d";
            // 
            // enumToolStripMenuItem
            // 
            this.enumToolStripMenuItem.Name = "enumToolStripMenuItem";
            this.enumToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.enumToolStripMenuItem.Text = "enum";
            // 
            // booleanToolStripMenuItem
            // 
            this.booleanToolStripMenuItem.Name = "booleanToolStripMenuItem";
            this.booleanToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.booleanToolStripMenuItem.Text = "boolean";
            // 
            // bitmaskToolStripMenuItem
            // 
            this.bitmaskToolStripMenuItem.Name = "bitmaskToolStripMenuItem";
            this.bitmaskToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.bitmaskToolStripMenuItem.Text = "bitmask";
            // 
            // numberToolStripMenuItem
            // 
            this.numberToolStripMenuItem.Name = "numberToolStripMenuItem";
            this.numberToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.numberToolStripMenuItem.Text = "number";
            // 
            // showHistogramToolStripMenuItem
            // 
            this.showHistogramToolStripMenuItem.Name = "showHistogramToolStripMenuItem";
            this.showHistogramToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.showHistogramToolStripMenuItem.Text = "Show Histogram";
            this.showHistogramToolStripMenuItem.Click += new System.EventHandler(this.showHistogramToolStripMenuItem_Click);
            // 
            // testExtraOffsetToolStripMenuItem
            // 
            this.testExtraOffsetToolStripMenuItem.Name = "testExtraOffsetToolStripMenuItem";
            this.testExtraOffsetToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.testExtraOffsetToolStripMenuItem.Text = "Test extra offset";
            this.testExtraOffsetToolStripMenuItem.Click += new System.EventHandler(this.testExtraOffsetToolStripMenuItem_Click);
            // 
            // addressRelativeDiffToolStripMenuItem1
            // 
            this.addressRelativeDiffToolStripMenuItem1.Name = "addressRelativeDiffToolStripMenuItem1";
            this.addressRelativeDiffToolStripMenuItem1.Size = new System.Drawing.Size(230, 22);
            this.addressRelativeDiffToolStripMenuItem1.Text = "Segment address diff";
            this.addressRelativeDiffToolStripMenuItem1.Click += new System.EventHandler(this.addressRelativeDiffToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
            // 
            // editRowToolStripMenuItem
            // 
            this.editRowToolStripMenuItem.Enabled = false;
            this.editRowToolStripMenuItem.Name = "editRowToolStripMenuItem";
            this.editRowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.editRowToolStripMenuItem.Text = "Edit table config";
            this.editRowToolStripMenuItem.Click += new System.EventHandler(this.editRowToolStripMenuItem_Click);
            // 
            // insertRowToolStripMenuItem
            // 
            this.insertRowToolStripMenuItem.Enabled = false;
            this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
            this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.insertRowToolStripMenuItem.Text = "Insert table config";
            this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Enabled = false;
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.deleteRowToolStripMenuItem.Text = "Delete table config";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // duplicateTableConfigToolStripMenuItem
            // 
            this.duplicateTableConfigToolStripMenuItem.Enabled = false;
            this.duplicateTableConfigToolStripMenuItem.Name = "duplicateTableConfigToolStripMenuItem";
            this.duplicateTableConfigToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.duplicateTableConfigToolStripMenuItem.Text = "Duplicate table config";
            this.duplicateTableConfigToolStripMenuItem.Click += new System.EventHandler(this.duplicateTableConfigToolStripMenuItem_Click);
            // 
            // copyToTableseekToolStripMenuItem
            // 
            this.copyToTableseekToolStripMenuItem.Name = "copyToTableseekToolStripMenuItem";
            this.copyToTableseekToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.copyToTableseekToolStripMenuItem.Text = "Copy to tableseek";
            this.copyToTableseekToolStripMenuItem.Click += new System.EventHandler(this.copyToTableseekToolStripMenuItem_Click);
            // 
            // editOSAddressPairsToolStripMenuItem
            // 
            this.editOSAddressPairsToolStripMenuItem.Name = "editOSAddressPairsToolStripMenuItem";
            this.editOSAddressPairsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.editOSAddressPairsToolStripMenuItem.Text = "Edit OS:Address pairs";
            this.editOSAddressPairsToolStripMenuItem.Click += new System.EventHandler(this.editOSAddressPairsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(227, 6);
            // 
            // addressRelativeDiffToolStripMenuItem
            // 
            this.addressRelativeDiffToolStripMenuItem.Name = "addressRelativeDiffToolStripMenuItem";
            this.addressRelativeDiffToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.addressRelativeDiffToolStripMenuItem.Text = "Address relative diff";
            // 
            // createPatchToolStripMenuItem
            // 
            this.createPatchToolStripMenuItem.Name = "createPatchToolStripMenuItem";
            this.createPatchToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.createPatchToolStripMenuItem.Text = "Create patch...";
            this.createPatchToolStripMenuItem.Click += new System.EventHandler(this.createPatchToolStripMenuItem_Click);
            // 
            // addTablesToExistingPatchToolStripMenuItem
            // 
            this.addTablesToExistingPatchToolStripMenuItem.Name = "addTablesToExistingPatchToolStripMenuItem";
            this.addTablesToExistingPatchToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.addTablesToExistingPatchToolStripMenuItem.Text = "Add tables to existing patch...";
            this.addTablesToExistingPatchToolStripMenuItem.Click += new System.EventHandler(this.addTablesToExistingPatchToolStripMenuItem_Click);
            // 
            // createPatchToolStripMenuItem1
            // 
            this.createPatchToolStripMenuItem1.Name = "createPatchToolStripMenuItem1";
            this.createPatchToolStripMenuItem1.Size = new System.Drawing.Size(230, 22);
            this.createPatchToolStripMenuItem1.Text = "Create patch as a Table";
            this.createPatchToolStripMenuItem1.Click += new System.EventHandler(this.createPatchToolStripMenuItem1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // axistablesToolStripMenuItem
            // 
            this.axistablesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openXaxisTableToolStripMenuItem,
            this.openYaxisTableToolStripMenuItem,
            this.openMathtableToolStripMenuItem,
            this.editXaxisTableToolStripMenuItem,
            this.editYaxisTableToolStripMenuItem,
            this.editMathtableToolStripMenuItem});
            this.axistablesToolStripMenuItem.Name = "axistablesToolStripMenuItem";
            this.axistablesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.axistablesToolStripMenuItem.Text = "Axis-tables";
            // 
            // openXaxisTableToolStripMenuItem
            // 
            this.openXaxisTableToolStripMenuItem.Enabled = false;
            this.openXaxisTableToolStripMenuItem.Name = "openXaxisTableToolStripMenuItem";
            this.openXaxisTableToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openXaxisTableToolStripMenuItem.Text = "Open X-axis table";
            this.openXaxisTableToolStripMenuItem.Click += new System.EventHandler(this.openXaxisTableToolStripMenuItem_Click);
            // 
            // openYaxisTableToolStripMenuItem
            // 
            this.openYaxisTableToolStripMenuItem.Enabled = false;
            this.openYaxisTableToolStripMenuItem.Name = "openYaxisTableToolStripMenuItem";
            this.openYaxisTableToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openYaxisTableToolStripMenuItem.Text = "Open Y-axis table";
            this.openYaxisTableToolStripMenuItem.Click += new System.EventHandler(this.openYaxisTableToolStripMenuItem_Click);
            // 
            // openMathtableToolStripMenuItem
            // 
            this.openMathtableToolStripMenuItem.Enabled = false;
            this.openMathtableToolStripMenuItem.Name = "openMathtableToolStripMenuItem";
            this.openMathtableToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openMathtableToolStripMenuItem.Text = "Open Math-table";
            this.openMathtableToolStripMenuItem.Click += new System.EventHandler(this.openMathtableToolStripMenuItem_Click);
            // 
            // editXaxisTableToolStripMenuItem
            // 
            this.editXaxisTableToolStripMenuItem.Enabled = false;
            this.editXaxisTableToolStripMenuItem.Name = "editXaxisTableToolStripMenuItem";
            this.editXaxisTableToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.editXaxisTableToolStripMenuItem.Text = "Edit X-axis table config";
            this.editXaxisTableToolStripMenuItem.Click += new System.EventHandler(this.editXaxisTableToolStripMenuItem_Click);
            // 
            // editYaxisTableToolStripMenuItem
            // 
            this.editYaxisTableToolStripMenuItem.Enabled = false;
            this.editYaxisTableToolStripMenuItem.Name = "editYaxisTableToolStripMenuItem";
            this.editYaxisTableToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.editYaxisTableToolStripMenuItem.Text = "Edit Y-axis table config";
            this.editYaxisTableToolStripMenuItem.Click += new System.EventHandler(this.editYaxisTableToolStripMenuItem_Click);
            // 
            // editMathtableToolStripMenuItem
            // 
            this.editMathtableToolStripMenuItem.Enabled = false;
            this.editMathtableToolStripMenuItem.Name = "editMathtableToolStripMenuItem";
            this.editMathtableToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.editMathtableToolStripMenuItem.Text = "Edit Math-table config";
            this.editMathtableToolStripMenuItem.Click += new System.EventHandler(this.editMathtableToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Filter:";
            // 
            // comboFilterBy
            // 
            this.comboFilterBy.FormattingEnabled = true;
            this.comboFilterBy.Location = new System.Drawing.Point(190, 34);
            this.comboFilterBy.Name = "comboFilterBy";
            this.comboFilterBy.Size = new System.Drawing.Size(121, 21);
            this.comboFilterBy.TabIndex = 18;
            this.comboFilterBy.SelectedIndexChanged += new System.EventHandler(this.comboFilterBy_SelectedIndexChanged);
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(529, 88);
            this.txtResult.TabIndex = 19;
            this.txtResult.Text = "";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtDescription2);
            this.splitContainer1.Panel1.Controls.Add(this.txtResult);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtDescription);
            this.splitContainer1.Size = new System.Drawing.Size(1066, 88);
            this.splitContainer1.SplitterDistance = 529;
            this.splitContainer1.TabIndex = 21;
            // 
            // txtDescription2
            // 
            this.txtDescription2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription2.Location = new System.Drawing.Point(69, 36);
            this.txtDescription2.Name = "txtDescription2";
            this.txtDescription2.Size = new System.Drawing.Size(300, 40);
            this.txtDescription2.TabIndex = 20;
            this.txtDescription2.Text = "";
            this.txtDescription2.Visible = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtDescription.Location = new System.Drawing.Point(0, 0);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(533, 88);
            this.txtDescription.TabIndex = 0;
            this.txtDescription.Text = "";
            // 
            // splitContainerListView
            // 
            this.splitContainerListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerListView.Location = new System.Drawing.Point(0, 72);
            this.splitContainerListView.Name = "splitContainerListView";
            this.splitContainerListView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerListView.Panel1
            // 
            this.splitContainerListView.Panel1.Controls.Add(this.splitContainerListMode);
            // 
            // splitContainerListView.Panel2
            // 
            this.splitContainerListView.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainerListView.Size = new System.Drawing.Size(1066, 487);
            this.splitContainerListView.SplitterDistance = 395;
            this.splitContainerListView.TabIndex = 22;
            // 
            // splitContainerListMode
            // 
            this.splitContainerListMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerListMode.Location = new System.Drawing.Point(0, 0);
            this.splitContainerListMode.Name = "splitContainerListMode";
            // 
            // splitContainerListMode.Panel1
            // 
            this.splitContainerListMode.Panel1.Controls.Add(this.splitContainerListTree);
            this.splitContainerListMode.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainerListMode.Panel2
            // 
            this.splitContainerListMode.Panel2.Controls.Add(this.groupUndeleteTableConfig);
            this.splitContainerListMode.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainerListMode.Size = new System.Drawing.Size(1066, 395);
            this.splitContainerListMode.SplitterDistance = 492;
            this.splitContainerListMode.TabIndex = 3;
            // 
            // splitContainerListTree
            // 
            this.splitContainerListTree.Location = new System.Drawing.Point(15, 6);
            this.splitContainerListTree.Name = "splitContainerListTree";
            this.splitContainerListTree.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerListTree.Panel1
            // 
            this.splitContainerListTree.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainerListTree.Panel2
            // 
            this.splitContainerListTree.Panel2.Controls.Add(this.groupTestExtraOffset);
            this.splitContainerListTree.Size = new System.Drawing.Size(360, 386);
            this.splitContainerListTree.SplitterDistance = 271;
            this.splitContainerListTree.TabIndex = 3;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStripListTree;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Indent = 20;
            this.treeView1.ItemHeight = 18;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeView1.SelectedNodes")));
            this.treeView1.Size = new System.Drawing.Size(360, 271);
            this.treeView1.TabIndex = 2;
            // 
            // contextMenuStripListTree
            // 
            this.contextMenuStripListTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.expand2LevelsToolStripMenuItem,
            this.expand3LevelsToolStripMenuItem,
            this.collapseToolStripMenuItem,
            this.addSegmentOffsetNodeToolStripMenuItem});
            this.contextMenuStripListTree.Name = "contextMenuStripListTree";
            this.contextMenuStripListTree.Size = new System.Drawing.Size(210, 114);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.expandAllToolStripMenuItem.Text = "Expand 1 level";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // expand2LevelsToolStripMenuItem
            // 
            this.expand2LevelsToolStripMenuItem.Name = "expand2LevelsToolStripMenuItem";
            this.expand2LevelsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.expand2LevelsToolStripMenuItem.Text = "Expand 2 levels";
            this.expand2LevelsToolStripMenuItem.Click += new System.EventHandler(this.expand2LevelsToolStripMenuItem_Click);
            // 
            // expand3LevelsToolStripMenuItem
            // 
            this.expand3LevelsToolStripMenuItem.Name = "expand3LevelsToolStripMenuItem";
            this.expand3LevelsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.expand3LevelsToolStripMenuItem.Text = "Expand 3 levels";
            this.expand3LevelsToolStripMenuItem.Click += new System.EventHandler(this.expand3LevelsToolStripMenuItem_Click);
            // 
            // collapseToolStripMenuItem
            // 
            this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
            this.collapseToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.collapseToolStripMenuItem.Text = "Collapse";
            this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
            // 
            // addSegmentOffsetNodeToolStripMenuItem
            // 
            this.addSegmentOffsetNodeToolStripMenuItem.Name = "addSegmentOffsetNodeToolStripMenuItem";
            this.addSegmentOffsetNodeToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.addSegmentOffsetNodeToolStripMenuItem.Text = "Add Segment offset node";
            this.addSegmentOffsetNodeToolStripMenuItem.Click += new System.EventHandler(this.addSegmentOffsetNodeToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "explorer.png");
            this.imageList1.Images.SetKeyName(1, "1d.png");
            this.imageList1.Images.SetKeyName(2, "2d.png");
            this.imageList1.Images.SetKeyName(3, "3d.png");
            this.imageList1.Images.SetKeyName(4, "Apply.png");
            this.imageList1.Images.SetKeyName(5, "bitmask.png");
            this.imageList1.Images.SetKeyName(6, "bitmask1d.png");
            this.imageList1.Images.SetKeyName(7, "bitmask2d.png");
            this.imageList1.Images.SetKeyName(8, "bitmask3d.png");
            this.imageList1.Images.SetKeyName(9, "boolean.png");
            this.imageList1.Images.SetKeyName(10, "boolean1d.png");
            this.imageList1.Images.SetKeyName(11, "boolean2d.png");
            this.imageList1.Images.SetKeyName(12, "boolean3d.png");
            this.imageList1.Images.SetKeyName(13, "boot.png");
            this.imageList1.Images.SetKeyName(14, "canbus.png");
            this.imageList1.Images.SetKeyName(15, "Cancel.png");
            this.imageList1.Images.SetKeyName(16, "category.png");
            this.imageList1.Images.SetKeyName(17, "Category_old.png");
            this.imageList1.Images.SetKeyName(18, "Category2.png");
            this.imageList1.Images.SetKeyName(19, "category3.png");
            this.imageList1.Images.SetKeyName(20, "collapse.png");
            this.imageList1.Images.SetKeyName(21, "CollapseLeft.png");
            this.imageList1.Images.SetKeyName(22, "CollapseUp.png");
            this.imageList1.Images.SetKeyName(23, "Connection.png");
            this.imageList1.Images.SetKeyName(24, "Convert.png");
            this.imageList1.Images.SetKeyName(25, "Dimensions.png");
            this.imageList1.Images.SetKeyName(26, "DTC.png");
            this.imageList1.Images.SetKeyName(27, "eeprom.png");
            this.imageList1.Images.SetKeyName(28, "engine.png");
            this.imageList1.Images.SetKeyName(29, "enginediag.png");
            this.imageList1.Images.SetKeyName(30, "enum.png");
            this.imageList1.Images.SetKeyName(31, "enum1d.png");
            this.imageList1.Images.SetKeyName(32, "enum2d.png");
            this.imageList1.Images.SetKeyName(33, "enum3d.png");
            this.imageList1.Images.SetKeyName(34, "Execute.png");
            this.imageList1.Images.SetKeyName(35, "expand.png");
            this.imageList1.Images.SetKeyName(36, "ExpandDown.png");
            this.imageList1.Images.SetKeyName(37, "ExpandRight.png");
            this.imageList1.Images.SetKeyName(38, "flag1d.png");
            this.imageList1.Images.SetKeyName(39, "flag2d.png");
            this.imageList1.Images.SetKeyName(40, "flag3d.png");
            this.imageList1.Images.SetKeyName(41, "flash.png");
            this.imageList1.Images.SetKeyName(42, "Flask.png");
            this.imageList1.Images.SetKeyName(43, "FolderClosed.png");
            this.imageList1.Images.SetKeyName(44, "FolderOpen.png");
            this.imageList1.Images.SetKeyName(45, "fuel.png");
            this.imageList1.Images.SetKeyName(46, "Histogram.png");
            this.imageList1.Images.SetKeyName(47, "info.png");
            this.imageList1.Images.SetKeyName(48, "listmode.png");
            this.imageList1.Images.SetKeyName(49, "mask1d.png");
            this.imageList1.Images.SetKeyName(50, "mask2d.png");
            this.imageList1.Images.SetKeyName(51, "mask3d.png");
            this.imageList1.Images.SetKeyName(52, "Modify.png");
            this.imageList1.Images.SetKeyName(53, "Multiview.png");
            this.imageList1.Images.SetKeyName(54, "number.png");
            this.imageList1.Images.SetKeyName(55, "os.png");
            this.imageList1.Images.SetKeyName(56, "patch.png");
            this.imageList1.Images.SetKeyName(57, "pieces.png");
            this.imageList1.Images.SetKeyName(58, "RunLiveUnitTest.png");
            this.imageList1.Images.SetKeyName(59, "SearchGo.png");
            this.imageList1.Images.SetKeyName(60, "segments.png");
            this.imageList1.Images.SetKeyName(61, "selection1d.png");
            this.imageList1.Images.SetKeyName(62, "selection2d.png");
            this.imageList1.Images.SetKeyName(63, "selection3d.png");
            this.imageList1.Images.SetKeyName(64, "Settings.png");
            this.imageList1.Images.SetKeyName(65, "Sitemap.png");
            this.imageList1.Images.SetKeyName(66, "speedo.png");
            this.imageList1.Images.SetKeyName(67, "stapler.png");
            this.imageList1.Images.SetKeyName(68, "system.png");
            this.imageList1.Images.SetKeyName(69, "Test.png");
            this.imageList1.Images.SetKeyName(70, "trans.png");
            this.imageList1.Images.SetKeyName(71, "transdiag.png");
            this.imageList1.Images.SetKeyName(72, "TreeView.png");
            this.imageList1.Images.SetKeyName(73, "Undo_Redo.png");
            this.imageList1.Images.SetKeyName(74, "valuetype.png");
            this.imageList1.Images.SetKeyName(75, "selection.png");
            // 
            // groupTestExtraOffset
            // 
            this.groupTestExtraOffset.Controls.Add(this.comboExtraOffsetResults);
            this.groupTestExtraOffset.Controls.Add(this.label5);
            this.groupTestExtraOffset.Controls.Add(this.chkTestExtraOffsetOffsetBytes);
            this.groupTestExtraOffset.Controls.Add(this.txtTestExtraOffset);
            this.groupTestExtraOffset.Controls.Add(this.chkTestExtraOffsetColorCoding);
            this.groupTestExtraOffset.Controls.Add(this.btnOK);
            this.groupTestExtraOffset.Controls.Add(this.chkTestExtraOffsetFilterOutOfRange);
            this.groupTestExtraOffset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupTestExtraOffset.Location = new System.Drawing.Point(0, 0);
            this.groupTestExtraOffset.Name = "groupTestExtraOffset";
            this.groupTestExtraOffset.Size = new System.Drawing.Size(360, 111);
            this.groupTestExtraOffset.TabIndex = 19;
            this.groupTestExtraOffset.TabStop = false;
            this.groupTestExtraOffset.Text = "Test Extra Offset";
            // 
            // comboExtraOffsetResults
            // 
            this.comboExtraOffsetResults.FormattingEnabled = true;
            this.comboExtraOffsetResults.Location = new System.Drawing.Point(10, 79);
            this.comboExtraOffsetResults.Name = "comboExtraOffsetResults";
            this.comboExtraOffsetResults.Size = new System.Drawing.Size(194, 21);
            this.comboExtraOffsetResults.TabIndex = 19;
            this.comboExtraOffsetResults.SelectedIndexChanged += new System.EventHandler(this.comboExtraOffsetResults_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Range:";
            // 
            // chkTestExtraOffsetOffsetBytes
            // 
            this.chkTestExtraOffsetOffsetBytes.AutoSize = true;
            this.chkTestExtraOffsetOffsetBytes.Location = new System.Drawing.Point(9, 56);
            this.chkTestExtraOffsetOffsetBytes.Name = "chkTestExtraOffsetOffsetBytes";
            this.chkTestExtraOffsetOffsetBytes.Size = new System.Drawing.Size(117, 17);
            this.chkTestExtraOffsetOffsetBytes.TabIndex = 18;
            this.chkTestExtraOffsetOffsetBytes.Text = "Search offset bytes";
            this.chkTestExtraOffsetOffsetBytes.UseVisualStyleBackColor = true;
            // 
            // txtTestExtraOffset
            // 
            this.txtTestExtraOffset.Location = new System.Drawing.Point(54, 13);
            this.txtTestExtraOffset.Name = "txtTestExtraOffset";
            this.txtTestExtraOffset.Size = new System.Drawing.Size(150, 20);
            this.txtTestExtraOffset.TabIndex = 12;
            this.txtTestExtraOffset.Text = "-10,10";
            // 
            // chkTestExtraOffsetColorCoding
            // 
            this.chkTestExtraOffsetColorCoding.AutoSize = true;
            this.chkTestExtraOffsetColorCoding.Location = new System.Drawing.Point(227, 12);
            this.chkTestExtraOffsetColorCoding.Name = "chkTestExtraOffsetColorCoding";
            this.chkTestExtraOffsetColorCoding.Size = new System.Drawing.Size(85, 17);
            this.chkTestExtraOffsetColorCoding.TabIndex = 17;
            this.chkTestExtraOffsetColorCoding.Text = "Color coding";
            this.chkTestExtraOffsetColorCoding.UseVisualStyleBackColor = true;
            this.chkTestExtraOffsetColorCoding.Visible = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(151, 48);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(54, 25);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkTestExtraOffsetFilterOutOfRange
            // 
            this.chkTestExtraOffsetFilterOutOfRange.AutoSize = true;
            this.chkTestExtraOffsetFilterOutOfRange.Location = new System.Drawing.Point(9, 39);
            this.chkTestExtraOffsetFilterOutOfRange.Name = "chkTestExtraOffsetFilterOutOfRange";
            this.chkTestExtraOffsetFilterOutOfRange.Size = new System.Drawing.Size(139, 17);
            this.chkTestExtraOffsetFilterOutOfRange.TabIndex = 15;
            this.chkTestExtraOffsetFilterOutOfRange.Text = "Filter out of range tables";
            this.chkTestExtraOffsetFilterOutOfRange.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabCategory);
            this.tabControl1.Controls.Add(this.tabMultiTree);
            this.tabControl1.Controls.Add(this.tabDimensions);
            this.tabControl1.Controls.Add(this.tabValueType);
            this.tabControl1.Controls.Add(this.tabSegments);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabPatches);
            this.tabControl1.Controls.Add(this.tabFileInfo);
            this.tabControl1.ImageList = this.imageListPng;
            this.tabControl1.Location = new System.Drawing.Point(381, 21);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(436, 360);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Visible = false;
            // 
            // tabCategory
            // 
            this.tabCategory.ImageKey = "category.png";
            this.tabCategory.Location = new System.Drawing.Point(4, 31);
            this.tabCategory.Name = "tabCategory";
            this.tabCategory.Size = new System.Drawing.Size(428, 325);
            this.tabCategory.TabIndex = 2;
            this.tabCategory.UseVisualStyleBackColor = true;
            this.tabCategory.Click += new System.EventHandler(this.tabCategory_Click);
            // 
            // tabMultiTree
            // 
            this.tabMultiTree.ImageKey = "Multiview.png";
            this.tabMultiTree.Location = new System.Drawing.Point(4, 31);
            this.tabMultiTree.Name = "tabMultiTree";
            this.tabMultiTree.Size = new System.Drawing.Size(428, 325);
            this.tabMultiTree.TabIndex = 7;
            this.tabMultiTree.UseVisualStyleBackColor = true;
            // 
            // tabDimensions
            // 
            this.tabDimensions.ImageKey = "Dimensions.png";
            this.tabDimensions.Location = new System.Drawing.Point(4, 31);
            this.tabDimensions.Name = "tabDimensions";
            this.tabDimensions.Padding = new System.Windows.Forms.Padding(3);
            this.tabDimensions.Size = new System.Drawing.Size(428, 325);
            this.tabDimensions.TabIndex = 0;
            this.tabDimensions.UseVisualStyleBackColor = true;
            // 
            // tabValueType
            // 
            this.tabValueType.ImageKey = "valuetype.png";
            this.tabValueType.Location = new System.Drawing.Point(4, 31);
            this.tabValueType.Name = "tabValueType";
            this.tabValueType.Padding = new System.Windows.Forms.Padding(3);
            this.tabValueType.Size = new System.Drawing.Size(428, 325);
            this.tabValueType.TabIndex = 1;
            this.tabValueType.UseVisualStyleBackColor = true;
            // 
            // tabSegments
            // 
            this.tabSegments.ImageKey = "segments.png";
            this.tabSegments.Location = new System.Drawing.Point(4, 31);
            this.tabSegments.Name = "tabSegments";
            this.tabSegments.Size = new System.Drawing.Size(428, 325);
            this.tabSegments.TabIndex = 3;
            this.tabSegments.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.chkShowTreeHover);
            this.tabSettings.Controls.Add(this.groupNavigator);
            this.tabSettings.Controls.Add(this.btnExplorerFont);
            this.tabSettings.Controls.Add(this.chkShowTableCount);
            this.tabSettings.Controls.Add(this.chkAutoMulti1d);
            this.tabSettings.Controls.Add(this.chkShowCategorySubfolder);
            this.tabSettings.Controls.Add(this.labelIconSize);
            this.tabSettings.Controls.Add(this.numIconSize);
            this.tabSettings.ImageKey = "Settings.png";
            this.tabSettings.Location = new System.Drawing.Point(4, 31);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(428, 325);
            this.tabSettings.TabIndex = 5;
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // chkShowTreeHover
            // 
            this.chkShowTreeHover.AutoSize = true;
            this.chkShowTreeHover.Location = new System.Drawing.Point(10, 78);
            this.chkShowTreeHover.Name = "chkShowTreeHover";
            this.chkShowTreeHover.Size = new System.Drawing.Size(244, 17);
            this.chkShowTreeHover.TabIndex = 35;
            this.chkShowTreeHover.Text = "Show description for table under mouse cursor";
            this.chkShowTreeHover.UseVisualStyleBackColor = true;
            // 
            // groupNavigator
            // 
            this.groupNavigator.Controls.Add(this.numNaviMaxTablesTotal);
            this.groupNavigator.Controls.Add(this.label4);
            this.groupNavigator.Controls.Add(this.label3);
            this.groupNavigator.Controls.Add(this.numNaviMaxTablesPerNode);
            this.groupNavigator.Location = new System.Drawing.Point(10, 138);
            this.groupNavigator.Name = "groupNavigator";
            this.groupNavigator.Size = new System.Drawing.Size(219, 72);
            this.groupNavigator.TabIndex = 33;
            this.groupNavigator.TabStop = false;
            this.groupNavigator.Text = "Navigator";
            this.groupNavigator.Enter += new System.EventHandler(this.groupNavigator_Enter);
            // 
            // numNaviMaxTablesTotal
            // 
            this.numNaviMaxTablesTotal.Location = new System.Drawing.Point(152, 39);
            this.numNaviMaxTablesTotal.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numNaviMaxTablesTotal.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNaviMaxTablesTotal.Name = "numNaviMaxTablesTotal";
            this.numNaviMaxTablesTotal.Size = new System.Drawing.Size(54, 20);
            this.numNaviMaxTablesTotal.TabIndex = 34;
            this.numNaviMaxTablesTotal.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Remember max tables total";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Remember max tables/node";
            // 
            // numNaviMaxTablesPerNode
            // 
            this.numNaviMaxTablesPerNode.Location = new System.Drawing.Point(152, 14);
            this.numNaviMaxTablesPerNode.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numNaviMaxTablesPerNode.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numNaviMaxTablesPerNode.Name = "numNaviMaxTablesPerNode";
            this.numNaviMaxTablesPerNode.Size = new System.Drawing.Size(54, 20);
            this.numNaviMaxTablesPerNode.TabIndex = 31;
            // 
            // btnExplorerFont
            // 
            this.btnExplorerFont.Location = new System.Drawing.Point(13, 104);
            this.btnExplorerFont.Name = "btnExplorerFont";
            this.btnExplorerFont.Size = new System.Drawing.Size(75, 23);
            this.btnExplorerFont.TabIndex = 30;
            this.btnExplorerFont.Text = "Font...";
            this.btnExplorerFont.UseVisualStyleBackColor = true;
            this.btnExplorerFont.Click += new System.EventHandler(this.btnExplorerFont_Click);
            // 
            // chkShowTableCount
            // 
            this.chkShowTableCount.AutoSize = true;
            this.chkShowTableCount.Location = new System.Drawing.Point(219, 32);
            this.chkShowTableCount.Name = "chkShowTableCount";
            this.chkShowTableCount.Size = new System.Drawing.Size(109, 17);
            this.chkShowTableCount.TabIndex = 29;
            this.chkShowTableCount.Text = "Show table count";
            this.chkShowTableCount.UseVisualStyleBackColor = true;
            // 
            // chkAutoMulti1d
            // 
            this.chkAutoMulti1d.AutoSize = true;
            this.chkAutoMulti1d.Location = new System.Drawing.Point(10, 55);
            this.chkAutoMulti1d.Name = "chkAutoMulti1d";
            this.chkAutoMulti1d.Size = new System.Drawing.Size(181, 17);
            this.chkAutoMulti1d.TabIndex = 28;
            this.chkAutoMulti1d.Text = "Automatic multitable for 1d tables";
            this.chkAutoMulti1d.UseVisualStyleBackColor = true;
            this.chkAutoMulti1d.CheckedChanged += new System.EventHandler(this.chkAutoMulti1d_CheckedChanged);
            // 
            // chkShowCategorySubfolder
            // 
            this.chkShowCategorySubfolder.AutoSize = true;
            this.chkShowCategorySubfolder.Location = new System.Drawing.Point(10, 32);
            this.chkShowCategorySubfolder.Name = "chkShowCategorySubfolder";
            this.chkShowCategorySubfolder.Size = new System.Drawing.Size(144, 17);
            this.chkShowCategorySubfolder.TabIndex = 0;
            this.chkShowCategorySubfolder.Text = "Show Category subfolder";
            this.chkShowCategorySubfolder.UseVisualStyleBackColor = true;
            this.chkShowCategorySubfolder.CheckedChanged += new System.EventHandler(this.chkShowCategorySubfolder_CheckedChanged);
            // 
            // labelIconSize
            // 
            this.labelIconSize.AutoSize = true;
            this.labelIconSize.Location = new System.Drawing.Point(7, 9);
            this.labelIconSize.Name = "labelIconSize";
            this.labelIconSize.Size = new System.Drawing.Size(52, 13);
            this.labelIconSize.TabIndex = 26;
            this.labelIconSize.Text = "Icon size:";
            // 
            // numIconSize
            // 
            this.numIconSize.Location = new System.Drawing.Point(65, 6);
            this.numIconSize.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numIconSize.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numIconSize.Name = "numIconSize";
            this.numIconSize.Size = new System.Drawing.Size(40, 20);
            this.numIconSize.TabIndex = 27;
            this.numIconSize.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numIconSize.ValueChanged += new System.EventHandler(this.numIconSize_ValueChanged);
            // 
            // tabPatches
            // 
            this.tabPatches.ImageKey = "patch.png";
            this.tabPatches.Location = new System.Drawing.Point(4, 31);
            this.tabPatches.Name = "tabPatches";
            this.tabPatches.Size = new System.Drawing.Size(428, 325);
            this.tabPatches.TabIndex = 4;
            this.tabPatches.UseVisualStyleBackColor = true;
            // 
            // tabFileInfo
            // 
            this.tabFileInfo.Controls.Add(this.tabControlFileInfo);
            this.tabFileInfo.ImageKey = "info.png";
            this.tabFileInfo.Location = new System.Drawing.Point(4, 31);
            this.tabFileInfo.Name = "tabFileInfo";
            this.tabFileInfo.Size = new System.Drawing.Size(428, 325);
            this.tabFileInfo.TabIndex = 6;
            this.tabFileInfo.UseVisualStyleBackColor = true;
            // 
            // tabControlFileInfo
            // 
            this.tabControlFileInfo.Location = new System.Drawing.Point(93, 89);
            this.tabControlFileInfo.Name = "tabControlFileInfo";
            this.tabControlFileInfo.SelectedIndex = 0;
            this.tabControlFileInfo.Size = new System.Drawing.Size(184, 138);
            this.tabControlFileInfo.TabIndex = 0;
            // 
            // imageListPng
            // 
            this.imageListPng.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPng.ImageStream")));
            this.imageListPng.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListPng.Images.SetKeyName(0, "explorer.png");
            this.imageListPng.Images.SetKeyName(1, "1d.png");
            this.imageListPng.Images.SetKeyName(2, "2d.png");
            this.imageListPng.Images.SetKeyName(3, "3d.png");
            this.imageListPng.Images.SetKeyName(4, "Apply.png");
            this.imageListPng.Images.SetKeyName(5, "bitmask.png");
            this.imageListPng.Images.SetKeyName(6, "bitmask1d.png");
            this.imageListPng.Images.SetKeyName(7, "bitmask2d.png");
            this.imageListPng.Images.SetKeyName(8, "bitmask3d.png");
            this.imageListPng.Images.SetKeyName(9, "boolean.png");
            this.imageListPng.Images.SetKeyName(10, "boolean1d.png");
            this.imageListPng.Images.SetKeyName(11, "boolean2d.png");
            this.imageListPng.Images.SetKeyName(12, "boolean3d.png");
            this.imageListPng.Images.SetKeyName(13, "boot.png");
            this.imageListPng.Images.SetKeyName(14, "canbus.png");
            this.imageListPng.Images.SetKeyName(15, "Cancel.png");
            this.imageListPng.Images.SetKeyName(16, "category.png");
            this.imageListPng.Images.SetKeyName(17, "Category_old.png");
            this.imageListPng.Images.SetKeyName(18, "Category2.png");
            this.imageListPng.Images.SetKeyName(19, "category3.png");
            this.imageListPng.Images.SetKeyName(20, "collapse.png");
            this.imageListPng.Images.SetKeyName(21, "CollapseLeft.png");
            this.imageListPng.Images.SetKeyName(22, "CollapseUp.png");
            this.imageListPng.Images.SetKeyName(23, "Connection.png");
            this.imageListPng.Images.SetKeyName(24, "Convert.png");
            this.imageListPng.Images.SetKeyName(25, "Dimensions.png");
            this.imageListPng.Images.SetKeyName(26, "DTC.png");
            this.imageListPng.Images.SetKeyName(27, "eeprom.png");
            this.imageListPng.Images.SetKeyName(28, "engine.png");
            this.imageListPng.Images.SetKeyName(29, "enginediag.png");
            this.imageListPng.Images.SetKeyName(30, "enum.png");
            this.imageListPng.Images.SetKeyName(31, "enum1d.png");
            this.imageListPng.Images.SetKeyName(32, "enum2d.png");
            this.imageListPng.Images.SetKeyName(33, "enum3d.png");
            this.imageListPng.Images.SetKeyName(34, "Execute.png");
            this.imageListPng.Images.SetKeyName(35, "expand.png");
            this.imageListPng.Images.SetKeyName(36, "ExpandDown.png");
            this.imageListPng.Images.SetKeyName(37, "ExpandRight.png");
            this.imageListPng.Images.SetKeyName(38, "flag1d.png");
            this.imageListPng.Images.SetKeyName(39, "flag2d.png");
            this.imageListPng.Images.SetKeyName(40, "flag3d.png");
            this.imageListPng.Images.SetKeyName(41, "flash.png");
            this.imageListPng.Images.SetKeyName(42, "Flask.png");
            this.imageListPng.Images.SetKeyName(43, "FolderClosed.png");
            this.imageListPng.Images.SetKeyName(44, "FolderOpen.png");
            this.imageListPng.Images.SetKeyName(45, "fuel.png");
            this.imageListPng.Images.SetKeyName(46, "Histogram.png");
            this.imageListPng.Images.SetKeyName(47, "info.png");
            this.imageListPng.Images.SetKeyName(48, "listmode.png");
            this.imageListPng.Images.SetKeyName(49, "mask1d.png");
            this.imageListPng.Images.SetKeyName(50, "mask2d.png");
            this.imageListPng.Images.SetKeyName(51, "mask3d.png");
            this.imageListPng.Images.SetKeyName(52, "Modify.png");
            this.imageListPng.Images.SetKeyName(53, "Multiview.png");
            this.imageListPng.Images.SetKeyName(54, "number.png");
            this.imageListPng.Images.SetKeyName(55, "os.png");
            this.imageListPng.Images.SetKeyName(56, "patch.png");
            this.imageListPng.Images.SetKeyName(57, "pieces.png");
            this.imageListPng.Images.SetKeyName(58, "RunLiveUnitTest.png");
            this.imageListPng.Images.SetKeyName(59, "SearchGo.png");
            this.imageListPng.Images.SetKeyName(60, "segments.png");
            this.imageListPng.Images.SetKeyName(61, "selection.png");
            this.imageListPng.Images.SetKeyName(62, "selection1d.png");
            this.imageListPng.Images.SetKeyName(63, "selection2d.png");
            this.imageListPng.Images.SetKeyName(64, "selection3d.png");
            this.imageListPng.Images.SetKeyName(65, "Settings.png");
            this.imageListPng.Images.SetKeyName(66, "Sitemap.png");
            this.imageListPng.Images.SetKeyName(67, "speedo.png");
            this.imageListPng.Images.SetKeyName(68, "stapler.png");
            this.imageListPng.Images.SetKeyName(69, "system.png");
            this.imageListPng.Images.SetKeyName(70, "Test.png");
            this.imageListPng.Images.SetKeyName(71, "trans.png");
            this.imageListPng.Images.SetKeyName(72, "transdiag.png");
            this.imageListPng.Images.SetKeyName(73, "TreeView.png");
            this.imageListPng.Images.SetKeyName(74, "Undo_Redo.png");
            this.imageListPng.Images.SetKeyName(75, "valuetype.png");
            this.imageListPng.Images.SetKeyName(76, "search.png");
            this.imageListPng.Images.SetKeyName(77, "category.ico");
            // 
            // groupUndeleteTableConfig
            // 
            this.groupUndeleteTableConfig.Controls.Add(this.btnCloseUndelete);
            this.groupUndeleteTableConfig.Controls.Add(this.btnRestoreTableConfig);
            this.groupUndeleteTableConfig.Controls.Add(this.dataGridTrashBin);
            this.groupUndeleteTableConfig.Location = new System.Drawing.Point(56, 55);
            this.groupUndeleteTableConfig.Name = "groupUndeleteTableConfig";
            this.groupUndeleteTableConfig.Size = new System.Drawing.Size(364, 305);
            this.groupUndeleteTableConfig.TabIndex = 2;
            this.groupUndeleteTableConfig.TabStop = false;
            this.groupUndeleteTableConfig.Text = "Undelete tableconfig";
            this.groupUndeleteTableConfig.Visible = false;
            // 
            // btnCloseUndelete
            // 
            this.btnCloseUndelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseUndelete.Location = new System.Drawing.Point(283, 19);
            this.btnCloseUndelete.Name = "btnCloseUndelete";
            this.btnCloseUndelete.Size = new System.Drawing.Size(75, 23);
            this.btnCloseUndelete.TabIndex = 3;
            this.btnCloseUndelete.Text = "Close";
            this.btnCloseUndelete.UseVisualStyleBackColor = true;
            this.btnCloseUndelete.Click += new System.EventHandler(this.btnCloseUndelete_Click);
            // 
            // btnRestoreTableConfig
            // 
            this.btnRestoreTableConfig.Location = new System.Drawing.Point(6, 19);
            this.btnRestoreTableConfig.Name = "btnRestoreTableConfig";
            this.btnRestoreTableConfig.Size = new System.Drawing.Size(75, 23);
            this.btnRestoreTableConfig.TabIndex = 2;
            this.btnRestoreTableConfig.Text = "Restore";
            this.btnRestoreTableConfig.UseVisualStyleBackColor = true;
            this.btnRestoreTableConfig.Click += new System.EventHandler(this.btnRestoreTableConfig_Click);
            // 
            // dataGridTrashBin
            // 
            this.dataGridTrashBin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridTrashBin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridTrashBin.ContextMenuStrip = this.contextMenuTrashBin;
            this.dataGridTrashBin.Location = new System.Drawing.Point(6, 48);
            this.dataGridTrashBin.Name = "dataGridTrashBin";
            this.dataGridTrashBin.Size = new System.Drawing.Size(352, 251);
            this.dataGridTrashBin.TabIndex = 1;
            // 
            // contextMenuTrashBin
            // 
            this.contextMenuTrashBin.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToolStripMenuItem});
            this.contextMenuTrashBin.Name = "contextMenuTrashBin";
            this.contextMenuTrashBin.Size = new System.Drawing.Size(114, 26);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // timerFilter
            // 
            this.timerFilter.Tick += new System.EventHandler(this.timerFilter_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(581, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Table:";
            // 
            // labelTableName
            // 
            this.labelTableName.AutoSize = true;
            this.labelTableName.BackColor = System.Drawing.SystemColors.Window;
            this.labelTableName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTableName.Location = new System.Drawing.Point(624, 4);
            this.labelTableName.Name = "labelTableName";
            this.labelTableName.Size = new System.Drawing.Size(74, 18);
            this.labelTableName.TabIndex = 30;
            this.labelTableName.Text = "tablename";
            // 
            // labelBy
            // 
            this.labelBy.AutoSize = true;
            this.labelBy.Location = new System.Drawing.Point(163, 37);
            this.labelBy.Name = "labelBy";
            this.labelBy.Size = new System.Drawing.Size(21, 13);
            this.labelBy.TabIndex = 31;
            this.labelBy.Text = "by:";
            // 
            // contextMenuStripTree
            // 
            this.contextMenuStripTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInNewWindowToolStripMenuItem,
            this.openInListModeToolStripMenuItem,
            this.goToAxistableToolStripMenuItem,
            this.compareSelectedTablesToolStripMenuItem1,
            this.sortByToolStripMenuItem});
            this.contextMenuStripTree.Name = "contextMenuStripTree";
            this.contextMenuStripTree.Size = new System.Drawing.Size(204, 114);
            // 
            // openInNewWindowToolStripMenuItem
            // 
            this.openInNewWindowToolStripMenuItem.Name = "openInNewWindowToolStripMenuItem";
            this.openInNewWindowToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openInNewWindowToolStripMenuItem.Text = "Open in new window";
            this.openInNewWindowToolStripMenuItem.Click += new System.EventHandler(this.openInNewWindowToolStripMenuItem_Click);
            // 
            // openInListModeToolStripMenuItem
            // 
            this.openInListModeToolStripMenuItem.Name = "openInListModeToolStripMenuItem";
            this.openInListModeToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openInListModeToolStripMenuItem.Text = "Open in list mode";
            this.openInListModeToolStripMenuItem.Click += new System.EventHandler(this.openInListModeToolStripMenuItem_Click);
            // 
            // goToAxistableToolStripMenuItem
            // 
            this.goToAxistableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xAxisToolStripMenuItem,
            this.yAxisToolStripMenuItem,
            this.mathToolStripMenuItem});
            this.goToAxistableToolStripMenuItem.Name = "goToAxistableToolStripMenuItem";
            this.goToAxistableToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.goToAxistableToolStripMenuItem.Text = "Go to Axis-table";
            // 
            // xAxisToolStripMenuItem
            // 
            this.xAxisToolStripMenuItem.Name = "xAxisToolStripMenuItem";
            this.xAxisToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.xAxisToolStripMenuItem.Text = "X-Axis";
            this.xAxisToolStripMenuItem.Click += new System.EventHandler(this.xAxisToolStripMenuItem_Click);
            // 
            // yAxisToolStripMenuItem
            // 
            this.yAxisToolStripMenuItem.Name = "yAxisToolStripMenuItem";
            this.yAxisToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.yAxisToolStripMenuItem.Text = "Y-Axis";
            this.yAxisToolStripMenuItem.Click += new System.EventHandler(this.yAxisToolStripMenuItem_Click);
            // 
            // mathToolStripMenuItem
            // 
            this.mathToolStripMenuItem.Name = "mathToolStripMenuItem";
            this.mathToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.mathToolStripMenuItem.Text = "Math";
            this.mathToolStripMenuItem.Click += new System.EventHandler(this.mathToolStripMenuItem_Click);
            // 
            // compareSelectedTablesToolStripMenuItem1
            // 
            this.compareSelectedTablesToolStripMenuItem1.Name = "compareSelectedTablesToolStripMenuItem1";
            this.compareSelectedTablesToolStripMenuItem1.Size = new System.Drawing.Size(203, 22);
            this.compareSelectedTablesToolStripMenuItem1.Text = "Compare selected tables";
            this.compareSelectedTablesToolStripMenuItem1.Click += new System.EventHandler(this.compareSelectedTablesToolStripMenuItem1_Click);
            // 
            // sortByToolStripMenuItem
            // 
            this.sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
            this.sortByToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.sortByToolStripMenuItem.Text = "Sort by";
            // 
            // contextMenuStripPatch
            // 
            this.contextMenuStripPatch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyPatchToolStripMenuItem1});
            this.contextMenuStripPatch.Name = "contextMenuStripPatch";
            this.contextMenuStripPatch.Size = new System.Drawing.Size(139, 26);
            // 
            // applyPatchToolStripMenuItem1
            // 
            this.applyPatchToolStripMenuItem1.Name = "applyPatchToolStripMenuItem1";
            this.applyPatchToolStripMenuItem1.Size = new System.Drawing.Size(138, 22);
            this.applyPatchToolStripMenuItem1.Text = "Apply patch";
            this.applyPatchToolStripMenuItem1.Click += new System.EventHandler(this.applyPatchToolStripMenuItem1_Click);
            // 
            // txtMath
            // 
            this.txtMath.Location = new System.Drawing.Point(514, 35);
            this.txtMath.Name = "txtMath";
            this.txtMath.Size = new System.Drawing.Size(82, 20);
            this.txtMath.TabIndex = 33;
            this.txtMath.Text = "X*1";
            // 
            // groupExtraOffset
            // 
            this.groupExtraOffset.Controls.Add(this.btnShowExtraOffsetSearch);
            this.groupExtraOffset.Controls.Add(this.btnCancelExtraOffset);
            this.groupExtraOffset.Controls.Add(this.btnTestNextTable);
            this.groupExtraOffset.Controls.Add(this.btnTestPrevTable);
            this.groupExtraOffset.Controls.Add(this.btnExtraOffsetCopyFromTest);
            this.groupExtraOffset.Controls.Add(this.btnExtraOffsetCopyToTest);
            this.groupExtraOffset.Controls.Add(this.btnExtraOffsetTestApply);
            this.groupExtraOffset.Controls.Add(this.numExtraOffsetTest);
            this.groupExtraOffset.Controls.Add(this.btnExtraOffsetTest);
            this.groupExtraOffset.Controls.Add(this.numExtraOffset);
            this.groupExtraOffset.Controls.Add(this.btnExtraOffsetPrev);
            this.groupExtraOffset.Controls.Add(this.btnExtraOffsetNext);
            this.groupExtraOffset.Location = new System.Drawing.Point(662, 23);
            this.groupExtraOffset.Name = "groupExtraOffset";
            this.groupExtraOffset.Size = new System.Drawing.Size(402, 43);
            this.groupExtraOffset.TabIndex = 35;
            this.groupExtraOffset.TabStop = false;
            this.groupExtraOffset.Text = "Extra offset";
            // 
            // btnShowExtraOffsetSearch
            // 
            this.btnShowExtraOffsetSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowExtraOffsetSearch.ImageKey = "search.png";
            this.btnShowExtraOffsetSearch.ImageList = this.imageListPng;
            this.btnShowExtraOffsetSearch.Location = new System.Drawing.Point(363, 9);
            this.btnShowExtraOffsetSearch.Name = "btnShowExtraOffsetSearch";
            this.btnShowExtraOffsetSearch.Size = new System.Drawing.Size(33, 23);
            this.btnShowExtraOffsetSearch.TabIndex = 14;
            this.btnShowExtraOffsetSearch.UseVisualStyleBackColor = true;
            this.btnShowExtraOffsetSearch.Click += new System.EventHandler(this.btnShowExtraOffsetSearch_Click);
            // 
            // btnCancelExtraOffset
            // 
            this.btnCancelExtraOffset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelExtraOffset.ImageKey = "Cancel.png";
            this.btnCancelExtraOffset.ImageList = this.imageListPng;
            this.btnCancelExtraOffset.Location = new System.Drawing.Point(327, 9);
            this.btnCancelExtraOffset.Name = "btnCancelExtraOffset";
            this.btnCancelExtraOffset.Size = new System.Drawing.Size(33, 23);
            this.btnCancelExtraOffset.TabIndex = 13;
            this.btnCancelExtraOffset.UseVisualStyleBackColor = true;
            this.btnCancelExtraOffset.Click += new System.EventHandler(this.btnCancelExtraOffset_Click);
            // 
            // btnTestNextTable
            // 
            this.btnTestNextTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestNextTable.Location = new System.Drawing.Point(270, 10);
            this.btnTestNextTable.Name = "btnTestNextTable";
            this.btnTestNextTable.Size = new System.Drawing.Size(19, 23);
            this.btnTestNextTable.TabIndex = 12;
            this.btnTestNextTable.Text = "˅";
            this.btnTestNextTable.UseVisualStyleBackColor = true;
            this.btnTestNextTable.Click += new System.EventHandler(this.btnTestNextTable_Click);
            // 
            // btnTestPrevTable
            // 
            this.btnTestPrevTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestPrevTable.Location = new System.Drawing.Point(250, 10);
            this.btnTestPrevTable.Name = "btnTestPrevTable";
            this.btnTestPrevTable.Size = new System.Drawing.Size(19, 23);
            this.btnTestPrevTable.TabIndex = 11;
            this.btnTestPrevTable.Text = "˄";
            this.btnTestPrevTable.UseVisualStyleBackColor = true;
            this.btnTestPrevTable.Click += new System.EventHandler(this.btnTestPrevTable_Click);
            // 
            // btnExtraOffsetCopyFromTest
            // 
            this.btnExtraOffsetCopyFromTest.Font = new System.Drawing.Font("Consolas", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtraOffsetCopyFromTest.Location = new System.Drawing.Point(128, 19);
            this.btnExtraOffsetCopyFromTest.Name = "btnExtraOffsetCopyFromTest";
            this.btnExtraOffsetCopyFromTest.Size = new System.Drawing.Size(19, 15);
            this.btnExtraOffsetCopyFromTest.TabIndex = 10;
            this.btnExtraOffsetCopyFromTest.Text = "<";
            this.btnExtraOffsetCopyFromTest.UseVisualStyleBackColor = true;
            this.btnExtraOffsetCopyFromTest.Click += new System.EventHandler(this.btnExtraOffsetCopyFromTest_Click);
            // 
            // btnExtraOffsetCopyToTest
            // 
            this.btnExtraOffsetCopyToTest.Font = new System.Drawing.Font("Consolas", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtraOffsetCopyToTest.Location = new System.Drawing.Point(128, 6);
            this.btnExtraOffsetCopyToTest.Name = "btnExtraOffsetCopyToTest";
            this.btnExtraOffsetCopyToTest.Size = new System.Drawing.Size(19, 15);
            this.btnExtraOffsetCopyToTest.TabIndex = 9;
            this.btnExtraOffsetCopyToTest.Text = ">";
            this.btnExtraOffsetCopyToTest.UseVisualStyleBackColor = true;
            this.btnExtraOffsetCopyToTest.Click += new System.EventHandler(this.btnExtraOffsetCopyToTest_Click);
            // 
            // btnExtraOffsetTestApply
            // 
            this.btnExtraOffsetTestApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtraOffsetTestApply.ImageKey = "Apply.png";
            this.btnExtraOffsetTestApply.ImageList = this.imageListPng;
            this.btnExtraOffsetTestApply.Location = new System.Drawing.Point(292, 9);
            this.btnExtraOffsetTestApply.Name = "btnExtraOffsetTestApply";
            this.btnExtraOffsetTestApply.Size = new System.Drawing.Size(33, 23);
            this.btnExtraOffsetTestApply.TabIndex = 8;
            this.btnExtraOffsetTestApply.UseVisualStyleBackColor = true;
            this.btnExtraOffsetTestApply.Click += new System.EventHandler(this.btnExtraOffsetTestApply_Click);
            // 
            // numExtraOffsetTest
            // 
            this.numExtraOffsetTest.Location = new System.Drawing.Point(148, 13);
            this.numExtraOffsetTest.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numExtraOffsetTest.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.numExtraOffsetTest.Name = "numExtraOffsetTest";
            this.numExtraOffsetTest.Size = new System.Drawing.Size(55, 20);
            this.numExtraOffsetTest.TabIndex = 7;
            this.numExtraOffsetTest.ValueChanged += new System.EventHandler(this.numExtraOffsetTest_ValueChanged);
            // 
            // btnExtraOffsetTest
            // 
            this.btnExtraOffsetTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtraOffsetTest.ImageKey = "Test.png";
            this.btnExtraOffsetTest.ImageList = this.imageListPng;
            this.btnExtraOffsetTest.Location = new System.Drawing.Point(209, 10);
            this.btnExtraOffsetTest.Name = "btnExtraOffsetTest";
            this.btnExtraOffsetTest.Size = new System.Drawing.Size(33, 23);
            this.btnExtraOffsetTest.TabIndex = 6;
            this.btnExtraOffsetTest.UseVisualStyleBackColor = true;
            this.btnExtraOffsetTest.Click += new System.EventHandler(this.btnExtraOffsetTest_Click);
            // 
            // numExtraOffset
            // 
            this.numExtraOffset.Location = new System.Drawing.Point(70, 13);
            this.numExtraOffset.Maximum = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.numExtraOffset.Minimum = new decimal(new int[] {
            1410065407,
            2,
            0,
            -2147483648});
            this.numExtraOffset.Name = "numExtraOffset";
            this.numExtraOffset.Size = new System.Drawing.Size(55, 20);
            this.numExtraOffset.TabIndex = 3;
            this.numExtraOffset.ValueChanged += new System.EventHandler(this.numExtraOffset_ValueChanged);
            // 
            // btnExtraOffsetPrev
            // 
            this.btnExtraOffsetPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtraOffsetPrev.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtraOffsetPrev.ImageKey = "CollapseLeft.png";
            this.btnExtraOffsetPrev.ImageList = this.imageListPngSmall;
            this.btnExtraOffsetPrev.Location = new System.Drawing.Point(6, 13);
            this.btnExtraOffsetPrev.Name = "btnExtraOffsetPrev";
            this.btnExtraOffsetPrev.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnExtraOffsetPrev.Size = new System.Drawing.Size(25, 20);
            this.btnExtraOffsetPrev.TabIndex = 4;
            this.btnExtraOffsetPrev.UseVisualStyleBackColor = true;
            this.btnExtraOffsetPrev.Click += new System.EventHandler(this.btnExtraOffsetPrev_Click);
            // 
            // imageListPngSmall
            // 
            this.imageListPngSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPngSmall.ImageStream")));
            this.imageListPngSmall.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListPngSmall.Images.SetKeyName(0, "1d.png");
            this.imageListPngSmall.Images.SetKeyName(1, "2d.png");
            this.imageListPngSmall.Images.SetKeyName(2, "3d.png");
            this.imageListPngSmall.Images.SetKeyName(3, "Apply.png");
            this.imageListPngSmall.Images.SetKeyName(4, "bitmask.png");
            this.imageListPngSmall.Images.SetKeyName(5, "bitmask1d.png");
            this.imageListPngSmall.Images.SetKeyName(6, "bitmask2d.png");
            this.imageListPngSmall.Images.SetKeyName(7, "bitmask3d.png");
            this.imageListPngSmall.Images.SetKeyName(8, "boolean.png");
            this.imageListPngSmall.Images.SetKeyName(9, "boolean1d.png");
            this.imageListPngSmall.Images.SetKeyName(10, "boolean2d.png");
            this.imageListPngSmall.Images.SetKeyName(11, "boolean3d.png");
            this.imageListPngSmall.Images.SetKeyName(12, "boot.png");
            this.imageListPngSmall.Images.SetKeyName(13, "canbus.png");
            this.imageListPngSmall.Images.SetKeyName(14, "Cancel.png");
            this.imageListPngSmall.Images.SetKeyName(15, "category.png");
            this.imageListPngSmall.Images.SetKeyName(16, "Category_old.png");
            this.imageListPngSmall.Images.SetKeyName(17, "Category2.png");
            this.imageListPngSmall.Images.SetKeyName(18, "category3.png");
            this.imageListPngSmall.Images.SetKeyName(19, "collapse.png");
            this.imageListPngSmall.Images.SetKeyName(20, "CollapseLeft.png");
            this.imageListPngSmall.Images.SetKeyName(21, "CollapseUp.png");
            this.imageListPngSmall.Images.SetKeyName(22, "Connection.png");
            this.imageListPngSmall.Images.SetKeyName(23, "Convert.png");
            this.imageListPngSmall.Images.SetKeyName(24, "Dimensions.png");
            this.imageListPngSmall.Images.SetKeyName(25, "DTC.png");
            this.imageListPngSmall.Images.SetKeyName(26, "eeprom.png");
            this.imageListPngSmall.Images.SetKeyName(27, "engine.png");
            this.imageListPngSmall.Images.SetKeyName(28, "enginediag.png");
            this.imageListPngSmall.Images.SetKeyName(29, "enum.png");
            this.imageListPngSmall.Images.SetKeyName(30, "enum1d.png");
            this.imageListPngSmall.Images.SetKeyName(31, "enum2d.png");
            this.imageListPngSmall.Images.SetKeyName(32, "enum3d.png");
            this.imageListPngSmall.Images.SetKeyName(33, "Execute.png");
            this.imageListPngSmall.Images.SetKeyName(34, "expand.png");
            this.imageListPngSmall.Images.SetKeyName(35, "ExpandDown.png");
            this.imageListPngSmall.Images.SetKeyName(36, "ExpandRight.png");
            this.imageListPngSmall.Images.SetKeyName(37, "explorer.png");
            this.imageListPngSmall.Images.SetKeyName(38, "flag1d.png");
            this.imageListPngSmall.Images.SetKeyName(39, "flag2d.png");
            this.imageListPngSmall.Images.SetKeyName(40, "flag3d.png");
            this.imageListPngSmall.Images.SetKeyName(41, "flash.png");
            this.imageListPngSmall.Images.SetKeyName(42, "Flask.png");
            this.imageListPngSmall.Images.SetKeyName(43, "FolderClosed.png");
            this.imageListPngSmall.Images.SetKeyName(44, "FolderOpen.png");
            this.imageListPngSmall.Images.SetKeyName(45, "fuel.png");
            this.imageListPngSmall.Images.SetKeyName(46, "Histogram.png");
            this.imageListPngSmall.Images.SetKeyName(47, "info.png");
            this.imageListPngSmall.Images.SetKeyName(48, "listmode.png");
            this.imageListPngSmall.Images.SetKeyName(49, "mask1d.png");
            this.imageListPngSmall.Images.SetKeyName(50, "mask2d.png");
            this.imageListPngSmall.Images.SetKeyName(51, "mask3d.png");
            this.imageListPngSmall.Images.SetKeyName(52, "Modify.png");
            this.imageListPngSmall.Images.SetKeyName(53, "Multiview.png");
            this.imageListPngSmall.Images.SetKeyName(54, "number.png");
            this.imageListPngSmall.Images.SetKeyName(55, "os.png");
            this.imageListPngSmall.Images.SetKeyName(56, "patch.png");
            this.imageListPngSmall.Images.SetKeyName(57, "pieces.png");
            this.imageListPngSmall.Images.SetKeyName(58, "RunLiveUnitTest.png");
            this.imageListPngSmall.Images.SetKeyName(59, "SearchGo.png");
            this.imageListPngSmall.Images.SetKeyName(60, "segments.png");
            this.imageListPngSmall.Images.SetKeyName(61, "selection.png");
            this.imageListPngSmall.Images.SetKeyName(62, "selection1d.png");
            this.imageListPngSmall.Images.SetKeyName(63, "selection2d.png");
            this.imageListPngSmall.Images.SetKeyName(64, "selection3d.png");
            this.imageListPngSmall.Images.SetKeyName(65, "Settings.png");
            this.imageListPngSmall.Images.SetKeyName(66, "Sitemap.png");
            this.imageListPngSmall.Images.SetKeyName(67, "speedo.png");
            this.imageListPngSmall.Images.SetKeyName(68, "stapler.png");
            this.imageListPngSmall.Images.SetKeyName(69, "system.png");
            this.imageListPngSmall.Images.SetKeyName(70, "Test.png");
            this.imageListPngSmall.Images.SetKeyName(71, "trans.png");
            this.imageListPngSmall.Images.SetKeyName(72, "transdiag.png");
            this.imageListPngSmall.Images.SetKeyName(73, "TreeView.png");
            this.imageListPngSmall.Images.SetKeyName(74, "Undo_Redo.png");
            this.imageListPngSmall.Images.SetKeyName(75, "valuetype.png");
            // 
            // btnExtraOffsetNext
            // 
            this.btnExtraOffsetNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtraOffsetNext.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtraOffsetNext.ImageKey = "ExpandRight.png";
            this.btnExtraOffsetNext.ImageList = this.imageListPngSmall;
            this.btnExtraOffsetNext.Location = new System.Drawing.Point(39, 13);
            this.btnExtraOffsetNext.Name = "btnExtraOffsetNext";
            this.btnExtraOffsetNext.Size = new System.Drawing.Size(25, 20);
            this.btnExtraOffsetNext.TabIndex = 2;
            this.btnExtraOffsetNext.UseVisualStyleBackColor = true;
            this.btnExtraOffsetNext.Click += new System.EventHandler(this.btnExtraOffsetNext_Click);
            // 
            // labelMatch
            // 
            this.labelMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMatch.AutoSize = true;
            this.labelMatch.Location = new System.Drawing.Point(986, 5);
            this.labelMatch.Name = "labelMatch";
            this.labelMatch.Size = new System.Drawing.Size(78, 13);
            this.labelMatch.TabIndex = 36;
            this.labelMatch.Text = "Match 100.0 %";
            this.labelMatch.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelMatch.Visible = false;
            // 
            // radioListMode
            // 
            this.radioListMode.AutoSize = true;
            this.radioListMode.ImageKey = "listmode.png";
            this.radioListMode.ImageList = this.imageListPngSmall;
            this.radioListMode.Location = new System.Drawing.Point(460, 3);
            this.radioListMode.MaximumSize = new System.Drawing.Size(40, 40);
            this.radioListMode.Name = "radioListMode";
            this.radioListMode.Padding = new System.Windows.Forms.Padding(4);
            this.radioListMode.Size = new System.Drawing.Size(38, 24);
            this.radioListMode.TabIndex = 1;
            this.radioListMode.UseVisualStyleBackColor = true;
            this.radioListMode.CheckedChanged += new System.EventHandler(this.radioListMode_CheckedChanged);
            // 
            // contextMenuRestorePath
            // 
            this.contextMenuRestorePath.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restorePathToolStripMenuItem,
            this.treemodeToolStripMenuItem,
            this.listmodeToolStripMenuItem});
            this.contextMenuRestorePath.Name = "contextMenuRestorePath";
            this.contextMenuRestorePath.Size = new System.Drawing.Size(144, 70);
            // 
            // restorePathToolStripMenuItem
            // 
            this.restorePathToolStripMenuItem.Name = "restorePathToolStripMenuItem";
            this.restorePathToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.restorePathToolStripMenuItem.Text = "Restore path:";
            // 
            // treemodeToolStripMenuItem
            // 
            this.treemodeToolStripMenuItem.Name = "treemodeToolStripMenuItem";
            this.treemodeToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.treemodeToolStripMenuItem.Text = "Treemode";
            this.treemodeToolStripMenuItem.Visible = false;
            // 
            // listmodeToolStripMenuItem
            // 
            this.listmodeToolStripMenuItem.Name = "listmodeToolStripMenuItem";
            this.listmodeToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.listmodeToolStripMenuItem.Text = "Listmode";
            this.listmodeToolStripMenuItem.Visible = false;
            // 
            // radioTreeMode
            // 
            this.radioTreeMode.AllowDrop = true;
            this.radioTreeMode.AutoSize = true;
            this.radioTreeMode.Checked = true;
            this.radioTreeMode.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioTreeMode.ImageKey = "category.ico";
            this.radioTreeMode.ImageList = this.imageListPng;
            this.radioTreeMode.Location = new System.Drawing.Point(504, 1);
            this.radioTreeMode.Name = "radioTreeMode";
            this.radioTreeMode.Padding = new System.Windows.Forms.Padding(2);
            this.radioTreeMode.Size = new System.Drawing.Size(42, 28);
            this.radioTreeMode.TabIndex = 0;
            this.radioTreeMode.TabStop = true;
            this.radioTreeMode.UseVisualStyleBackColor = true;
            this.radioTreeMode.CheckedChanged += new System.EventHandler(this.radioTreeMode_CheckedChanged);
            // 
            // imageListPngBig
            // 
            this.imageListPngBig.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListPngBig.ImageStream")));
            this.imageListPngBig.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListPngBig.Images.SetKeyName(0, "1d.png");
            this.imageListPngBig.Images.SetKeyName(1, "2d.png");
            this.imageListPngBig.Images.SetKeyName(2, "3d.png");
            this.imageListPngBig.Images.SetKeyName(3, "Apply.png");
            this.imageListPngBig.Images.SetKeyName(4, "bitmask.png");
            this.imageListPngBig.Images.SetKeyName(5, "bitmask1d.png");
            this.imageListPngBig.Images.SetKeyName(6, "bitmask2d.png");
            this.imageListPngBig.Images.SetKeyName(7, "bitmask3d.png");
            this.imageListPngBig.Images.SetKeyName(8, "boolean.png");
            this.imageListPngBig.Images.SetKeyName(9, "boolean1d.png");
            this.imageListPngBig.Images.SetKeyName(10, "boolean2d.png");
            this.imageListPngBig.Images.SetKeyName(11, "boolean3d.png");
            this.imageListPngBig.Images.SetKeyName(12, "boot.png");
            this.imageListPngBig.Images.SetKeyName(13, "canbus.png");
            this.imageListPngBig.Images.SetKeyName(14, "Cancel.png");
            this.imageListPngBig.Images.SetKeyName(15, "category.png");
            this.imageListPngBig.Images.SetKeyName(16, "Category_old.png");
            this.imageListPngBig.Images.SetKeyName(17, "Category2.png");
            this.imageListPngBig.Images.SetKeyName(18, "category3.png");
            this.imageListPngBig.Images.SetKeyName(19, "collapse.png");
            this.imageListPngBig.Images.SetKeyName(20, "CollapseLeft.png");
            this.imageListPngBig.Images.SetKeyName(21, "CollapseUp.png");
            this.imageListPngBig.Images.SetKeyName(22, "Connection.png");
            this.imageListPngBig.Images.SetKeyName(23, "Convert.png");
            this.imageListPngBig.Images.SetKeyName(24, "Dimensions.png");
            this.imageListPngBig.Images.SetKeyName(25, "DTC.png");
            this.imageListPngBig.Images.SetKeyName(26, "eeprom.png");
            this.imageListPngBig.Images.SetKeyName(27, "engine.png");
            this.imageListPngBig.Images.SetKeyName(28, "enginediag.png");
            this.imageListPngBig.Images.SetKeyName(29, "enum.png");
            this.imageListPngBig.Images.SetKeyName(30, "enum1d.png");
            this.imageListPngBig.Images.SetKeyName(31, "enum2d.png");
            this.imageListPngBig.Images.SetKeyName(32, "enum3d.png");
            this.imageListPngBig.Images.SetKeyName(33, "Execute.png");
            this.imageListPngBig.Images.SetKeyName(34, "expand.png");
            this.imageListPngBig.Images.SetKeyName(35, "ExpandDown.png");
            this.imageListPngBig.Images.SetKeyName(36, "ExpandRight.png");
            this.imageListPngBig.Images.SetKeyName(37, "explorer.png");
            this.imageListPngBig.Images.SetKeyName(38, "flag1d.png");
            this.imageListPngBig.Images.SetKeyName(39, "flag2d.png");
            this.imageListPngBig.Images.SetKeyName(40, "flag3d.png");
            this.imageListPngBig.Images.SetKeyName(41, "flash.png");
            this.imageListPngBig.Images.SetKeyName(42, "Flask.png");
            this.imageListPngBig.Images.SetKeyName(43, "FolderClosed.png");
            this.imageListPngBig.Images.SetKeyName(44, "FolderOpen.png");
            this.imageListPngBig.Images.SetKeyName(45, "fuel.png");
            this.imageListPngBig.Images.SetKeyName(46, "Histogram.png");
            this.imageListPngBig.Images.SetKeyName(47, "info.png");
            this.imageListPngBig.Images.SetKeyName(48, "listmode.png");
            this.imageListPngBig.Images.SetKeyName(49, "mask1d.png");
            this.imageListPngBig.Images.SetKeyName(50, "mask2d.png");
            this.imageListPngBig.Images.SetKeyName(51, "mask3d.png");
            this.imageListPngBig.Images.SetKeyName(52, "Modify.png");
            this.imageListPngBig.Images.SetKeyName(53, "Multiview.png");
            this.imageListPngBig.Images.SetKeyName(54, "number.png");
            this.imageListPngBig.Images.SetKeyName(55, "os.png");
            this.imageListPngBig.Images.SetKeyName(56, "patch.png");
            this.imageListPngBig.Images.SetKeyName(57, "pieces.png");
            this.imageListPngBig.Images.SetKeyName(58, "RunLiveUnitTest.png");
            this.imageListPngBig.Images.SetKeyName(59, "SearchGo.png");
            this.imageListPngBig.Images.SetKeyName(60, "segments.png");
            this.imageListPngBig.Images.SetKeyName(61, "selection.png");
            this.imageListPngBig.Images.SetKeyName(62, "selection1d.png");
            this.imageListPngBig.Images.SetKeyName(63, "selection2d.png");
            this.imageListPngBig.Images.SetKeyName(64, "selection3d.png");
            this.imageListPngBig.Images.SetKeyName(65, "Settings.png");
            this.imageListPngBig.Images.SetKeyName(66, "Sitemap.png");
            this.imageListPngBig.Images.SetKeyName(67, "speedo.png");
            this.imageListPngBig.Images.SetKeyName(68, "stapler.png");
            this.imageListPngBig.Images.SetKeyName(69, "system.png");
            this.imageListPngBig.Images.SetKeyName(70, "Test.png");
            this.imageListPngBig.Images.SetKeyName(71, "trans.png");
            this.imageListPngBig.Images.SetKeyName(72, "transdiag.png");
            this.imageListPngBig.Images.SetKeyName(73, "TreeView.png");
            this.imageListPngBig.Images.SetKeyName(74, "Undo_Redo.png");
            this.imageListPngBig.Images.SetKeyName(75, "valuetype.png");
            // 
            // btnExecute
            // 
            this.btnExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecute.ImageKey = "Execute.png";
            this.btnExecute.ImageList = this.imageListPng;
            this.btnExecute.Location = new System.Drawing.Point(602, 31);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(39, 27);
            this.btnExecute.TabIndex = 34;
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnFlash
            // 
            this.btnFlash.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFlash.ImageKey = "flash.png";
            this.btnFlash.ImageList = this.imageListPng;
            this.btnFlash.Location = new System.Drawing.Point(549, 4);
            this.btnFlash.Name = "btnFlash";
            this.btnFlash.Size = new System.Drawing.Size(26, 25);
            this.btnFlash.TabIndex = 32;
            this.btnFlash.UseVisualStyleBackColor = true;
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
            // 
            // btnCollapse
            // 
            this.btnCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapse.ImageKey = "collapse.png";
            this.btnCollapse.ImageList = this.imageListPng;
            this.btnCollapse.Location = new System.Drawing.Point(347, 32);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Size = new System.Drawing.Size(24, 24);
            this.btnCollapse.TabIndex = 25;
            this.btnCollapse.UseVisualStyleBackColor = true;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpand.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExpand.ImageKey = "expand.png";
            this.btnExpand.ImageList = this.imageListPng;
            this.btnExpand.Location = new System.Drawing.Point(317, 32);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(24, 24);
            this.btnExpand.TabIndex = 24;
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnCollapseTree
            // 
            this.btnCollapseTree.ImageKey = "collapse.png";
            this.btnCollapseTree.ImageList = this.imageListPngSmall;
            this.btnCollapseTree.Location = new System.Drawing.Point(0, 53);
            this.btnCollapseTree.Name = "btnCollapseTree";
            this.btnCollapseTree.Size = new System.Drawing.Size(19, 19);
            this.btnCollapseTree.TabIndex = 37;
            this.btnCollapseTree.UseVisualStyleBackColor = true;
            this.btnCollapseTree.Click += new System.EventHandler(this.btnCollapseTree_Click);
            // 
            // FrmTuner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1066, 559);
            this.Controls.Add(this.btnCollapseTree);
            this.Controls.Add(this.labelMatch);
            this.Controls.Add(this.groupExtraOffset);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.txtMath);
            this.Controls.Add(this.btnFlash);
            this.Controls.Add(this.radioListMode);
            this.Controls.Add(this.radioTreeMode);
            this.Controls.Add(this.labelBy);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.labelTableName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCollapse);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.comboFilterBy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboTableCategory);
            this.Controls.Add(this.labelCategory);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainerListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmTuner";
            this.Text = "Tuner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTuner_FormClosing);
            this.Load += new System.EventHandler(this.frmTuner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerListView.Panel1.ResumeLayout(false);
            this.splitContainerListView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListView)).EndInit();
            this.splitContainerListView.ResumeLayout(false);
            this.splitContainerListMode.Panel1.ResumeLayout(false);
            this.splitContainerListMode.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListMode)).EndInit();
            this.splitContainerListMode.ResumeLayout(false);
            this.splitContainerListTree.Panel1.ResumeLayout(false);
            this.splitContainerListTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerListTree)).EndInit();
            this.splitContainerListTree.ResumeLayout(false);
            this.contextMenuStripListTree.ResumeLayout(false);
            this.groupTestExtraOffset.ResumeLayout(false);
            this.groupTestExtraOffset.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.groupNavigator.ResumeLayout(false);
            this.groupNavigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNaviMaxTablesTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNaviMaxTablesPerNode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIconSize)).EndInit();
            this.tabFileInfo.ResumeLayout(false);
            this.groupUndeleteTableConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridTrashBin)).EndInit();
            this.contextMenuTrashBin.ResumeLayout(false);
            this.contextMenuStripTree.ResumeLayout(false);
            this.contextMenuStripPatch.ResumeLayout(false);
            this.groupExtraOffset.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffsetTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExtraOffset)).EndInit();
            this.contextMenuRestorePath.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.ComboBox comboTableCategory;
        private System.Windows.Forms.Label labelCategory;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBINToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editTableToolStripMenuItem;
        private Label label1;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem showTablesWithEmptyAddressToolStripMenuItem;
        private ComboBox comboFilterBy;
        private ToolStripMenuItem disableMultitableToolStripMenuItem;
        private ToolStripMenuItem saveBinAsToolStripMenuItem;
        private ToolStripMenuItem unitsToolStripMenuItem;
        private RichTextBox txtResult;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainerListView;
        private ToolStripMenuItem resetTunerModeColumnsToolStripMenuItem;
        private ToolStripMenuItem loadBINToolStripMenuItem;
        private ToolStripMenuItem currentFileToolStripMenuItem;
        private ToolStripMenuItem tableListToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem insertRowToolStripMenuItem;
        private ToolStripMenuItem deleteRowToolStripMenuItem;
        private ToolStripMenuItem editRowToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem duplicateTableConfigToolStripMenuItem;
        private ToolStripMenuItem searchAndCompareToolStripMenuItem;
        private ToolStripMenuItem utilitiesToolStripMenuItem;
        private ToolStripMenuItem importToolStripMenuItem1;
        private ToolStripMenuItem dTCToolStripMenuItem;
        private ToolStripMenuItem tableSeekToolStripMenuItem;
        private ToolStripMenuItem xDFToolStripMenuItem1;
        private ToolStripMenuItem tinyTunerDBV6OnlyToolStripMenuItem;
        private ToolStripMenuItem xMlgeneratorImportCSVToolStripMenuItem;
        private ToolStripMenuItem cSVexperimentalToolStripMenuItem;
        private ToolStripMenuItem cSV2ExperimentalToolStripMenuItem;
        private ToolStripMenuItem exportToolStripMenuItem1;
        private ToolStripMenuItem cSVToolStripMenuItem;
        private ToolStripMenuItem xMLGeneratorExportToolStripMenuItem;
        private ToolStripMenuItem xmlToolStripMenuItem;
        private ToolStripMenuItem loadXMLToolStripMenuItem;
        private ToolStripMenuItem saveXMLToolStripMenuItem;
        private ToolStripMenuItem saveXMLAsToolStripMenuItem;
        private ToolStripMenuItem clearTableToolStripMenuItem;
        private ToolStripMenuItem loadTablelistxmlTableseekImportToolStripMenuItem;
        private ToolStripMenuItem loadTablelistnewToolStripMenuItem;
        private ToolStripMenuItem openMultipleBINToolStripMenuItem;
        private ToolStripMenuItem saveAllBINFilesToolStripMenuItem;
        private ToolStripMenuItem disableConfigAutoloadToolStripMenuItem;
        private ToolStripMenuItem findDifferencesToolStripMenuItem;
        private ToolStripMenuItem searchAndCompareAllToolStripMenuItem;
        private ToolStripMenuItem compareSelectedTablesToolStripMenuItem;
        private ToolStripMenuItem massModifyTableListsToolStripMenuItem;
        private ToolStripMenuItem massModifyTableListsSelectFilesToolStripMenuItem;
        private ToolStripMenuItem moreSettingsToolStripMenuItem;
        private Timer timerFilter;
        private ToolStripMenuItem caseSensitiveFilteringToolStripMenuItem;
        private ToolStripMenuItem copySelectedTablesToToolStripMenuItem;
        private ToolStripMenuItem applyPatchToolStripMenuItem;
        private ToolStripMenuItem createPatchToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabDimensions;
        private TabPage tabValueType;
        private TabPage tabCategory;
        private TabPage tabSegments;
        private TabPage tabPatches;
        private Button btnCollapse;
        private Button btnExpand;
        private ImageList imageList1;
        private NumericUpDown numIconSize;
        private Label labelIconSize;
        private RadioButton radioListMode;
        private RadioButton radioTreeMode;
        private ToolStripMenuItem selectToolStripMenuItem;
        private ToolStripMenuItem dToolStripMenuItem;
        private ToolStripMenuItem dToolStripMenuItem1;
        private ToolStripMenuItem dToolStripMenuItem2;
        private ToolStripMenuItem enumToolStripMenuItem;
        private ToolStripMenuItem booleanToolStripMenuItem;
        private ToolStripMenuItem bitmaskToolStripMenuItem;
        private ToolStripMenuItem numberToolStripMenuItem;
        private Label label2;
        public Label labelTableName;
        private Label labelBy;
        private TabPage tabSettings;
        private CheckBox chkShowCategorySubfolder;
        private ContextMenuStrip contextMenuStripTree;
        private ToolStripMenuItem openInNewWindowToolStripMenuItem;
        private CheckBox chkAutoMulti1d;
        private ToolStripMenuItem selectFileToolStripMenuItem;
        private ToolStripMenuItem findDifferencesHEXToolStripMenuItem;
        private ToolStripMenuItem selectFileToolStripMenuItem1;
        private TabPage tabFileInfo;
        private TabControl tabControlFileInfo;
        private ToolStripMenuItem compareSelectedTablesToolStripMenuItem1;
        private TreeViewMS treeView1;
        private SplitContainer splitContainerListMode;
        private ContextMenuStrip contextMenuStripListTree;
        private ToolStripMenuItem expandAllToolStripMenuItem;
        private ToolStripMenuItem collapseToolStripMenuItem;
        private ToolStripMenuItem expand2LevelsToolStripMenuItem;
        private ToolStripMenuItem expand3LevelsToolStripMenuItem;
        private ToolStripMenuItem reloadFileFromDiskToolStripMenuItem;
        private ToolStripMenuItem addTablesToExistingPatchToolStripMenuItem;
        private ContextMenuStrip contextMenuStripPatch;
        private ToolStripMenuItem applyPatchToolStripMenuItem1;
        private ToolStripMenuItem swapSegmentsToolStripMenuItem;
        private ToolStripMenuItem openCompareBINToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem homepageToolStripMenuItem;
        private ToolStripMenuItem sGMToolStripMenuItem;
        private ToolStripMenuItem intelHEXToolStripMenuItem;
        private ToolStripMenuItem motorolaSrecordToolStripMenuItem;
        private ToolStripMenuItem createPatchToolStripMenuItem1;
        private ToolStripMenuItem sortByToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private Button btnFlash;
        private ToolStripMenuItem histogramToolStripMenuItem;
        private ToolStripMenuItem showHistogramToolStripMenuItem;
        private ToolStripMenuItem axistablesToolStripMenuItem;
        private ToolStripMenuItem openXaxisTableToolStripMenuItem;
        private ToolStripMenuItem openYaxisTableToolStripMenuItem;
        private ToolStripMenuItem openMathtableToolStripMenuItem;
        private ToolStripMenuItem editXaxisTableToolStripMenuItem;
        private ToolStripMenuItem editYaxisTableToolStripMenuItem;
        private ToolStripMenuItem editMathtableToolStripMenuItem;
        private ToolStripMenuItem loggerToolStripMenuItem;
        private ToolStripMenuItem readWritePCMToolStripMenuItem;
        private ToolStripMenuItem patcherToolStripMenuItem;
        private ToolStripMenuItem modeToolStripMenuItem;
        private ToolStripMenuItem touristToolStripMenuItem;
        private ToolStripMenuItem basicToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem createProgramShortcutsToolStripMenuItem;
        private ToolStripMenuItem creditsToolStripMenuItem;
        private Button btnExecute;
        private TextBox txtMath;
        private ToolStripMenuItem testExtraOffsetToolStripMenuItem;
        private GroupBox groupExtraOffset;
        private Button btnExtraOffsetNext;
        private NumericUpDown numExtraOffset;
        private Button btnExtraOffsetPrev;
        private ToolStripMenuItem extraOffsetToolStripMenuItem;
        private ToolStripMenuItem clearPreviewToolStripMenuItem;
        private ToolStripMenuItem appendToPreviewToolStripMenuItem;
        private ToolStripMenuItem appendFocusToolStripMenuItem;
        private ToolStripMenuItem addressRelativeDiffToolStripMenuItem1;
        private ToolStripMenuItem addressRelativeDiffToolStripMenuItem;
        private Button btnExtraOffsetTest;
        private Button btnExtraOffsetTestApply;
        private NumericUpDown numExtraOffsetTest;
        private ToolStripMenuItem addNewTableToolStripMenuItem;
        private ToolStripMenuItem showOffsetVisualizerToolStripMenuItem;
        private ToolStripMenuItem mirrorSegmentsFromCompareToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private TabPage tabMultiTree;
        private ToolStripMenuItem fontToolStripMenuItem;
        private CheckBox chkShowTableCount;
        private Button btnExplorerFont;
        private ToolStripMenuItem treeviewSettingsToolStripMenuItem;
        private ToolStripMenuItem addSegmentOffsetNodeToolStripMenuItem;
        private ToolStripMenuItem renameDuplicateTablenamesToolStripMenuItem;
        private ToolStripMenuItem revToolStripMenuItem;
        private ToolStripMenuItem fwdToolStripMenuItem;
        private ToolStripMenuItem openInListModeToolStripMenuItem;
        private ToolStripMenuItem goToAxistableToolStripMenuItem;
        private ToolStripMenuItem xAxisToolStripMenuItem;
        private ToolStripMenuItem yAxisToolStripMenuItem;
        private ToolStripMenuItem mathToolStripMenuItem;
        private Label label3;
        private NumericUpDown numNaviMaxTablesPerNode;
        private GroupBox groupNavigator;
        private NumericUpDown numNaviMaxTablesTotal;
        private Label label4;
        private ToolStripMenuItem copyRowToolStripMenuItem1;
        private ToolStripMenuItem pasteRowToolStripMenuItem;
        private ToolStripMenuItem updateAddressesByOSToolStripMenuItem;
        private ToolStripMenuItem editOSAddressPairsToolStripMenuItem;
        private ToolStripMenuItem renameTablelistToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem showOnlyMappedTablesToolStripMenuItem;
        private ToolStripMenuItem saveExtraOffsetTablelistAsToolStripMenuItem;
        private ToolStripMenuItem copyToTableseekToolStripMenuItem;
        private ToolStripMenuItem editTableSeekToolStripMenuItem;
        private ToolStripMenuItem sortColumnsToolStripMenuItem;
        private ToolStripMenuItem undoRedoToolStripMenuItem;
        private ToolStripMenuItem xDFnewToolStripMenuItem;
        private ToolStripMenuItem launcherToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem createDebugLogToolStripMenuItem;
        private ToolStripMenuItem swapTablenameExtratablenameToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem loadColumnsPresetToolStripMenuItem;
        private ToolStripMenuItem saveColumnsPresetAsToolStripMenuItem;
        private ToolStripMenuItem resizeColumnsToolStripMenuItem;
        private ToolStripMenuItem resizeNowToolStripMenuItem;
        private ToolStripMenuItem autoresizeToolStripMenuItem;
        private ToolStripMenuItem autosaveColumnsToolStripMenuItem;
        private ToolStripMenuItem sessionToolStripMenuItem;
        private ToolStripMenuItem createMapSessionToolStripMenuItem;
        private ToolStripMenuItem saveSessionToolStripMenuItem;
        private ToolStripMenuItem saveSessionAsToolStripMenuItem;
        private ToolStripMenuItem closeSessionToolStripMenuItem;
        private ToolStripMenuItem newSessionToolStripMenuItem;
        private ToolStripMenuItem archiveSessionToolStripMenuItem;
        private ToolStripMenuItem restoreArchivedSessionToolStripMenuItem;
        private ToolStripMenuItem loadSessionToolStripMenuItem;
        private ToolStripMenuItem archiveSessionWithCommentsToolStripMenuItem;
        private Button btnExtraOffsetCopyFromTest;
        private Button btnExtraOffsetCopyToTest;
        private Button btnTestPrevTable;
        private Button btnTestNextTable;
        private Button btnCancelExtraOffset;
        private ToolStripMenuItem mapExtraOffsetButtonsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem enableExtraoffsetButtonsToolStripMenuItem;
        private CheckBox chkShowTreeHover;
        private Label labelMatch;
        public DataGridView dataGridView1;
        private RichTextBox txtDescription;
        private ToolStripMenuItem allSettingsToolStripMenuItem;
        private ImageList imageListPng;
        private ImageList imageListPngSmall;
        private ToolStripMenuItem searchTablesextraoffsetToolStripMenuItem;
        private ImageList imageListPngBig;
        private Button btnCollapseTree;
        private SplitContainer splitContainerListTree;
        private Button btnShowExtraOffsetSearch;
        private GroupBox groupTestExtraOffset;
        private Label label5;
        public CheckBox chkTestExtraOffsetOffsetBytes;
        public TextBox txtTestExtraOffset;
        public CheckBox chkTestExtraOffsetColorCoding;
        public Button btnOK;
        public CheckBox chkTestExtraOffsetFilterOutOfRange;
        private ComboBox comboExtraOffsetResults;
        private ContextMenuStrip contextMenuRestorePath;
        private ToolStripMenuItem restorePathToolStripMenuItem;
        private ToolStripMenuItem treemodeToolStripMenuItem;
        private ToolStripMenuItem listmodeToolStripMenuItem;
        private RichTextBox txtDescription2;
        private DataGridView dataGridTrashBin;
        private ContextMenuStrip contextMenuTrashBin;
        private ToolStripMenuItem restoreToolStripMenuItem;
        private ToolStripMenuItem undeleteTableconfigToolStripMenuItem;
        private GroupBox groupUndeleteTableConfig;
        private Button btnCloseUndelete;
        private Button btnRestoreTableConfig;
    }
}