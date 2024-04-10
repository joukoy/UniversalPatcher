
namespace UniversalPatcher
{
    partial class frmChecksumResearch
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
            this.dataGridCsUtilMethods = new System.Windows.Forms.DataGridView();
            this.chkCSUtilMatchOnly = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.numCsUtilThreads = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkCsUtilFilter = new System.Windows.Forms.CheckBox();
            this.btnCsutilSearchBosch = new System.Windows.Forms.Button();
            this.btnCsUtilFix = new System.Windows.Forms.Button();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtCSAddr = new System.Windows.Forms.TextBox();
            this.labelCsAddress = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtChecksumRange = new System.Windows.Forms.TextBox();
            this.btnTestChecksum = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCsUtilOffset = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioCsUtilReadRange = new System.Windows.Forms.RadioButton();
            this.labelMinRange = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioCsUtilRangeUnknown = new System.Windows.Forms.RadioButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.radioCsUtilRangeLimit = new System.Windows.Forms.RadioButton();
            this.radioCsUtilRangeExact = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioCsAfterPair = new System.Windows.Forms.RadioButton();
            this.radioCsUtilCSValue = new System.Windows.Forms.RadioButton();
            this.radioCsUtilCsAfterRange = new System.Windows.Forms.RadioButton();
            this.radioCsUtilCsExact = new System.Windows.Forms.RadioButton();
            this.richChkData = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCalculationRangeFromResultsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCalculationRangeFromResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStop = new System.Windows.Forms.Button();
            this.chkCsUtilLogResults = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilterValues = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCsUtilMethods)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilThreads)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridCsUtilMethods
            // 
            this.dataGridCsUtilMethods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridCsUtilMethods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCsUtilMethods.Location = new System.Drawing.Point(12, 27);
            this.dataGridCsUtilMethods.Name = "dataGridCsUtilMethods";
            this.dataGridCsUtilMethods.Size = new System.Drawing.Size(627, 169);
            this.dataGridCsUtilMethods.TabIndex = 168;
            // 
            // chkCSUtilMatchOnly
            // 
            this.chkCSUtilMatchOnly.AutoSize = true;
            this.chkCSUtilMatchOnly.Checked = true;
            this.chkCSUtilMatchOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCSUtilMatchOnly.Location = new System.Drawing.Point(562, 212);
            this.chkCSUtilMatchOnly.Name = "chkCSUtilMatchOnly";
            this.chkCSUtilMatchOnly.Size = new System.Drawing.Size(79, 17);
            this.chkCSUtilMatchOnly.TabIndex = 173;
            this.chkCSUtilMatchOnly.Text = "Only match";
            this.chkCSUtilMatchOnly.UseVisualStyleBackColor = true;
            this.chkCSUtilMatchOnly.CheckedChanged += new System.EventHandler(this.chkCSUtilMatchOnly_CheckedChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(511, 282);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(49, 13);
            this.label25.TabIndex = 192;
            this.label25.Text = "Threads:";
            // 
            // numCsUtilThreads
            // 
            this.numCsUtilThreads.Location = new System.Drawing.Point(571, 280);
            this.numCsUtilThreads.Name = "numCsUtilThreads";
            this.numCsUtilThreads.Size = new System.Drawing.Size(70, 20);
            this.numCsUtilThreads.TabIndex = 191;
            this.numCsUtilThreads.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkCsUtilFilter);
            this.groupBox6.Controls.Add(this.btnCsutilSearchBosch);
            this.groupBox6.Location = new System.Drawing.Point(6, 148);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(117, 65);
            this.groupBox6.TabIndex = 188;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Search Bosch inv";
            // 
            // chkCsUtilFilter
            // 
            this.chkCsUtilFilter.AutoSize = true;
            this.chkCsUtilFilter.Checked = true;
            this.chkCsUtilFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCsUtilFilter.Location = new System.Drawing.Point(20, 14);
            this.chkCsUtilFilter.Name = "chkCsUtilFilter";
            this.chkCsUtilFilter.Size = new System.Drawing.Size(69, 17);
            this.chkCsUtilFilter.TabIndex = 157;
            this.chkCsUtilFilter.Text = "Filter 0+F";
            this.chkCsUtilFilter.UseVisualStyleBackColor = true;
            // 
            // btnCsutilSearchBosch
            // 
            this.btnCsutilSearchBosch.Location = new System.Drawing.Point(22, 31);
            this.btnCsutilSearchBosch.Name = "btnCsutilSearchBosch";
            this.btnCsutilSearchBosch.Size = new System.Drawing.Size(70, 25);
            this.btnCsutilSearchBosch.TabIndex = 156;
            this.btnCsutilSearchBosch.Text = "Search ";
            this.btnCsutilSearchBosch.UseVisualStyleBackColor = true;
            this.btnCsutilSearchBosch.Click += new System.EventHandler(this.btnCsutilSearchBosch_Click);
            // 
            // btnCsUtilFix
            // 
            this.btnCsUtilFix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCsUtilFix.Location = new System.Drawing.Point(619, 518);
            this.btnCsUtilFix.Name = "btnCsUtilFix";
            this.btnCsUtilFix.Size = new System.Drawing.Size(109, 23);
            this.btnCsUtilFix.TabIndex = 187;
            this.btnCsUtilFix.Text = "Write Checksum";
            this.btnCsUtilFix.UseVisualStyleBackColor = true;
            this.btnCsUtilFix.Click += new System.EventHandler(this.btnCsUtilFix_Click);
            // 
            // txtExclude
            // 
            this.txtExclude.Enabled = false;
            this.txtExclude.Location = new System.Drawing.Point(64, 164);
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.Size = new System.Drawing.Size(137, 20);
            this.txtExclude.TabIndex = 184;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 167);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(48, 13);
            this.label19.TabIndex = 183;
            this.label19.Text = "Exclude:";
            // 
            // txtCSAddr
            // 
            this.txtCSAddr.Enabled = false;
            this.txtCSAddr.Location = new System.Drawing.Point(76, 16);
            this.txtCSAddr.Name = "txtCSAddr";
            this.txtCSAddr.Size = new System.Drawing.Size(137, 20);
            this.txtCSAddr.TabIndex = 182;
            this.txtCSAddr.TextChanged += new System.EventHandler(this.txtCSAddr_TextChanged);
            // 
            // labelCsAddress
            // 
            this.labelCsAddress.AutoSize = true;
            this.labelCsAddress.Location = new System.Drawing.Point(6, 20);
            this.labelCsAddress.Name = "labelCsAddress";
            this.labelCsAddress.Size = new System.Drawing.Size(48, 13);
            this.labelCsAddress.TabIndex = 181;
            this.labelCsAddress.Text = "Address:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(10, 20);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 13);
            this.label17.TabIndex = 180;
            this.label17.Text = "Range:";
            // 
            // txtChecksumRange
            // 
            this.txtChecksumRange.Enabled = false;
            this.txtChecksumRange.Location = new System.Drawing.Point(64, 17);
            this.txtChecksumRange.Name = "txtChecksumRange";
            this.txtChecksumRange.Size = new System.Drawing.Size(137, 20);
            this.txtChecksumRange.TabIndex = 179;
            // 
            // btnTestChecksum
            // 
            this.btnTestChecksum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestChecksum.Location = new System.Drawing.Point(619, 464);
            this.btnTestChecksum.Name = "btnTestChecksum";
            this.btnTestChecksum.Size = new System.Drawing.Size(109, 23);
            this.btnTestChecksum.TabIndex = 178;
            this.btnTestChecksum.Text = "Calculate";
            this.btnTestChecksum.UseVisualStyleBackColor = true;
            this.btnTestChecksum.Click += new System.EventHandler(this.btnTestChecksum_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCsUtilOffset);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioCsUtilReadRange);
            this.groupBox1.Controls.Add(this.labelMinRange);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioCsUtilRangeUnknown);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.radioCsUtilRangeLimit);
            this.groupBox1.Controls.Add(this.radioCsUtilRangeExact);
            this.groupBox1.Controls.Add(this.txtChecksumRange);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txtExclude);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Location = new System.Drawing.Point(12, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 231);
            this.groupBox1.TabIndex = 193;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calculation range";
            // 
            // txtCsUtilOffset
            // 
            this.txtCsUtilOffset.Enabled = false;
            this.txtCsUtilOffset.Location = new System.Drawing.Point(64, 193);
            this.txtCsUtilOffset.Name = "txtCsUtilOffset";
            this.txtCsUtilOffset.Size = new System.Drawing.Size(136, 20);
            this.txtCsUtilOffset.TabIndex = 191;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 194);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 190;
            this.label3.Text = "Offset:";
            // 
            // radioCsUtilReadRange
            // 
            this.radioCsUtilReadRange.AutoSize = true;
            this.radioCsUtilReadRange.Location = new System.Drawing.Point(13, 136);
            this.radioCsUtilReadRange.Name = "radioCsUtilReadRange";
            this.radioCsUtilReadRange.Size = new System.Drawing.Size(116, 17);
            this.radioCsUtilReadRange.TabIndex = 189;
            this.radioCsUtilReadRange.TabStop = true;
            this.radioCsUtilReadRange.Text = "Read address pairs";
            this.radioCsUtilReadRange.UseVisualStyleBackColor = true;
            this.radioCsUtilReadRange.CheckedChanged += new System.EventHandler(this.radioCsUtilReadRange_CheckedChanged);
            // 
            // labelMinRange
            // 
            this.labelMinRange.AutoSize = true;
            this.labelMinRange.Location = new System.Drawing.Point(37, 64);
            this.labelMinRange.Name = "labelMinRange";
            this.labelMinRange.Size = new System.Drawing.Size(21, 13);
            this.labelMinRange.TabIndex = 188;
            this.labelMinRange.Text = "0%";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 45);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 185;
            this.label1.Text = "Min range";
            // 
            // radioCsUtilRangeUnknown
            // 
            this.radioCsUtilRangeUnknown.AutoSize = true;
            this.radioCsUtilRangeUnknown.Checked = true;
            this.radioCsUtilRangeUnknown.Location = new System.Drawing.Point(13, 119);
            this.radioCsUtilRangeUnknown.Name = "radioCsUtilRangeUnknown";
            this.radioCsUtilRangeUnknown.Size = new System.Drawing.Size(122, 17);
            this.radioCsUtilRangeUnknown.TabIndex = 183;
            this.radioCsUtilRangeUnknown.TabStop = true;
            this.radioCsUtilRangeUnknown.Text = "Unknown, search all";
            this.radioCsUtilRangeUnknown.UseVisualStyleBackColor = true;
            this.radioCsUtilRangeUnknown.CheckedChanged += new System.EventHandler(this.radioCsUtilRangeUnknown_CheckedChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(64, 43);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(137, 45);
            this.trackBar1.TabIndex = 187;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // radioCsUtilRangeLimit
            // 
            this.radioCsUtilRangeLimit.AutoSize = true;
            this.radioCsUtilRangeLimit.Location = new System.Drawing.Point(13, 103);
            this.radioCsUtilRangeLimit.Name = "radioCsUtilRangeLimit";
            this.radioCsUtilRangeLimit.Size = new System.Drawing.Size(169, 17);
            this.radioCsUtilRangeLimit.TabIndex = 182;
            this.radioCsUtilRangeLimit.Text = "Unknown, search inside range";
            this.radioCsUtilRangeLimit.UseVisualStyleBackColor = true;
            // 
            // radioCsUtilRangeExact
            // 
            this.radioCsUtilRangeExact.AutoSize = true;
            this.radioCsUtilRangeExact.Location = new System.Drawing.Point(13, 88);
            this.radioCsUtilRangeExact.Name = "radioCsUtilRangeExact";
            this.radioCsUtilRangeExact.Size = new System.Drawing.Size(52, 17);
            this.radioCsUtilRangeExact.TabIndex = 181;
            this.radioCsUtilRangeExact.Text = "Exact";
            this.radioCsUtilRangeExact.UseVisualStyleBackColor = true;
            this.radioCsUtilRangeExact.CheckedChanged += new System.EventHandler(this.radioCsUtilRangeExact_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioCsAfterPair);
            this.groupBox2.Controls.Add(this.radioCsUtilCSValue);
            this.groupBox2.Controls.Add(this.radioCsUtilCsAfterRange);
            this.groupBox2.Controls.Add(this.radioCsUtilCsExact);
            this.groupBox2.Controls.Add(this.txtCSAddr);
            this.groupBox2.Controls.Add(this.labelCsAddress);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Location = new System.Drawing.Point(235, 202);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 231);
            this.groupBox2.TabIndex = 194;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Checksum Address";
            // 
            // radioCsAfterPair
            // 
            this.radioCsAfterPair.AutoSize = true;
            this.radioCsAfterPair.Location = new System.Drawing.Point(6, 98);
            this.radioCsAfterPair.Name = "radioCsAfterPair";
            this.radioCsAfterPair.Size = new System.Drawing.Size(135, 17);
            this.radioCsAfterPair.TabIndex = 190;
            this.radioCsAfterPair.TabStop = true;
            this.radioCsAfterPair.Text = "Read after address pair";
            this.radioCsAfterPair.UseVisualStyleBackColor = true;
            // 
            // radioCsUtilCSValue
            // 
            this.radioCsUtilCSValue.AutoSize = true;
            this.radioCsUtilCSValue.Location = new System.Drawing.Point(6, 78);
            this.radioCsUtilCSValue.Name = "radioCsUtilCSValue";
            this.radioCsUtilCSValue.Size = new System.Drawing.Size(73, 17);
            this.radioCsUtilCSValue.TabIndex = 189;
            this.radioCsUtilCSValue.TabStop = true;
            this.radioCsUtilCSValue.Text = "Use value";
            this.radioCsUtilCSValue.UseVisualStyleBackColor = true;
            this.radioCsUtilCSValue.CheckedChanged += new System.EventHandler(this.radioCsUtilCSValue_CheckedChanged);
            // 
            // radioCsUtilCsAfterRange
            // 
            this.radioCsUtilCsAfterRange.AutoSize = true;
            this.radioCsUtilCsAfterRange.Checked = true;
            this.radioCsUtilCsAfterRange.Location = new System.Drawing.Point(6, 60);
            this.radioCsUtilCsAfterRange.Name = "radioCsUtilCsAfterRange";
            this.radioCsUtilCsAfterRange.Size = new System.Drawing.Size(77, 17);
            this.radioCsUtilCsAfterRange.TabIndex = 185;
            this.radioCsUtilCsAfterRange.TabStop = true;
            this.radioCsUtilCsAfterRange.Text = "After range";
            this.radioCsUtilCsAfterRange.UseVisualStyleBackColor = true;
            this.radioCsUtilCsAfterRange.CheckedChanged += new System.EventHandler(this.radioCsUtilCsAfterRange_CheckedChanged);
            // 
            // radioCsUtilCsExact
            // 
            this.radioCsUtilCsExact.AutoSize = true;
            this.radioCsUtilCsExact.Location = new System.Drawing.Point(6, 43);
            this.radioCsUtilCsExact.Name = "radioCsUtilCsExact";
            this.radioCsUtilCsExact.Size = new System.Drawing.Size(101, 17);
            this.radioCsUtilCsExact.TabIndex = 183;
            this.radioCsUtilCsExact.TabStop = true;
            this.radioCsUtilCsExact.Text = "Exact/list/range";
            this.radioCsUtilCsExact.UseVisualStyleBackColor = true;
            // 
            // richChkData
            // 
            this.richChkData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richChkData.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richChkData.HideSelection = false;
            this.richChkData.Location = new System.Drawing.Point(12, 439);
            this.richChkData.Name = "richChkData";
            this.richChkData.Size = new System.Drawing.Size(589, 112);
            this.richChkData.TabIndex = 195;
            this.richChkData.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.resultsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(742, 24);
            this.menuStrip1.TabIndex = 196;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSettingsToolStripMenuItem,
            this.saveSettingsAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSettingsToolStripMenuItem
            // 
            this.openSettingsToolStripMenuItem.Name = "openSettingsToolStripMenuItem";
            this.openSettingsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openSettingsToolStripMenuItem.Text = "Load settings";
            this.openSettingsToolStripMenuItem.Click += new System.EventHandler(this.openSettingsToolStripMenuItem_Click);
            // 
            // saveSettingsAsToolStripMenuItem
            // 
            this.saveSettingsAsToolStripMenuItem.Name = "saveSettingsAsToolStripMenuItem";
            this.saveSettingsAsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveSettingsAsToolStripMenuItem.Text = "Save Settings as...";
            this.saveSettingsAsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsAsToolStripMenuItem_Click);
            // 
            // resultsToolStripMenuItem
            // 
            this.resultsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveResultsToolStripMenuItem,
            this.loadCalculationRangeFromResultsToolStripMenuItem1,
            this.loadCalculationRangeFromResultsToolStripMenuItem});
            this.resultsToolStripMenuItem.Name = "resultsToolStripMenuItem";
            this.resultsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.resultsToolStripMenuItem.Text = "Results";
            // 
            // saveResultsToolStripMenuItem
            // 
            this.saveResultsToolStripMenuItem.Name = "saveResultsToolStripMenuItem";
            this.saveResultsToolStripMenuItem.Size = new System.Drawing.Size(285, 22);
            this.saveResultsToolStripMenuItem.Text = "Show Results";
            this.saveResultsToolStripMenuItem.Click += new System.EventHandler(this.saveResultsToolStripMenuItem_Click);
            // 
            // loadCalculationRangeFromResultsToolStripMenuItem1
            // 
            this.loadCalculationRangeFromResultsToolStripMenuItem1.Name = "loadCalculationRangeFromResultsToolStripMenuItem1";
            this.loadCalculationRangeFromResultsToolStripMenuItem1.Size = new System.Drawing.Size(285, 22);
            this.loadCalculationRangeFromResultsToolStripMenuItem1.Text = "Load Calculation range from results....";
            this.loadCalculationRangeFromResultsToolStripMenuItem1.Click += new System.EventHandler(this.loadCalculationRangeFromResultsToolStripMenuItem1_Click);
            // 
            // loadCalculationRangeFromResultsToolStripMenuItem
            // 
            this.loadCalculationRangeFromResultsToolStripMenuItem.Name = "loadCalculationRangeFromResultsToolStripMenuItem";
            this.loadCalculationRangeFromResultsToolStripMenuItem.Size = new System.Drawing.Size(285, 22);
            this.loadCalculationRangeFromResultsToolStripMenuItem.Text = "Load Calculation range from result file...";
            this.loadCalculationRangeFromResultsToolStripMenuItem.Click += new System.EventHandler(this.loadCalculationRangeFromResultsToolStripMenuItem_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(619, 491);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(109, 23);
            this.btnStop.TabIndex = 197;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // chkCsUtilLogResults
            // 
            this.chkCsUtilLogResults.AutoSize = true;
            this.chkCsUtilLogResults.Checked = true;
            this.chkCsUtilLogResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCsUtilLogResults.Location = new System.Drawing.Point(474, 212);
            this.chkCsUtilLogResults.Name = "chkCsUtilLogResults";
            this.chkCsUtilLogResults.Size = new System.Drawing.Size(86, 17);
            this.chkCsUtilLogResults.TabIndex = 198;
            this.chkCsUtilLogResults.Text = "Show results";
            this.chkCsUtilLogResults.UseVisualStyleBackColor = true;
            this.chkCsUtilLogResults.CheckedChanged += new System.EventHandler(this.chkCsUtilLogResults_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(471, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 199;
            this.label2.Text = "Filter values:";
            // 
            // txtFilterValues
            // 
            this.txtFilterValues.Location = new System.Drawing.Point(474, 247);
            this.txtFilterValues.Name = "txtFilterValues";
            this.txtFilterValues.Size = new System.Drawing.Size(167, 20);
            this.txtFilterValues.TabIndex = 200;
            this.txtFilterValues.Text = "0,1";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(653, 27);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 201;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Location = new System.Drawing.Point(653, 56);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 202;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(653, 85);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 203;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // frmChecksumResearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 560);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtFilterValues);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkCsUtilLogResults);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.richChkData);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.numCsUtilThreads);
            this.Controls.Add(this.btnCsUtilFix);
            this.Controls.Add(this.btnTestChecksum);
            this.Controls.Add(this.chkCSUtilMatchOnly);
            this.Controls.Add(this.dataGridCsUtilMethods);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmChecksumResearch";
            this.Text = "Checksum Research";
            this.Load += new System.EventHandler(this.frmChecksumResearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCsUtilMethods)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilThreads)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridCsUtilMethods;
        private System.Windows.Forms.CheckBox chkCSUtilMatchOnly;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.NumericUpDown numCsUtilThreads;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkCsUtilFilter;
        private System.Windows.Forms.Button btnCsutilSearchBosch;
        private System.Windows.Forms.Button btnCsUtilFix;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtCSAddr;
        private System.Windows.Forms.Label labelCsAddress;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtChecksumRange;
        private System.Windows.Forms.Button btnTestChecksum;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioCsUtilRangeUnknown;
        private System.Windows.Forms.RadioButton radioCsUtilRangeLimit;
        private System.Windows.Forms.RadioButton radioCsUtilRangeExact;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioCsUtilCsAfterRange;
        private System.Windows.Forms.RadioButton radioCsUtilCsExact;
        private System.Windows.Forms.RichTextBox richChkData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsAsToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.RadioButton radioCsUtilCSValue;
        private System.Windows.Forms.CheckBox chkCsUtilLogResults;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label labelMinRange;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilterValues;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolStripMenuItem resultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCalculationRangeFromResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCalculationRangeFromResultsToolStripMenuItem1;
        private System.Windows.Forms.RadioButton radioCsUtilReadRange;
        private System.Windows.Forms.TextBox txtCsUtilOffset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioCsAfterPair;
    }
}