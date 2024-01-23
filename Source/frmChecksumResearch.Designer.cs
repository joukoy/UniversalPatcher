
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioCsUtilComplementAll = new System.Windows.Forms.RadioButton();
            this.radioCSUtilComplement2 = new System.Windows.Forms.RadioButton();
            this.radioCSUtilComplement1 = new System.Windows.Forms.RadioButton();
            this.radioCSUtilComplement0 = new System.Windows.Forms.RadioButton();
            this.chkCSUtilFilterZero = new System.Windows.Forms.CheckBox();
            this.chkCSUtilMatchOnly = new System.Windows.Forms.CheckBox();
            this.chkCsMSB = new System.Windows.Forms.CheckBox();
            this.chkCsUtilSwapBytes = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.numCsUtilThreads = new System.Windows.Forms.NumericUpDown();
            this.numCsUtilInitValue = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkCsUtilFilter = new System.Windows.Forms.CheckBox();
            this.btnCsutilSearchBosch = new System.Windows.Forms.Button();
            this.btnCsUtilFix = new System.Windows.Forms.Button();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtCSAddr = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtChecksumRange = new System.Windows.Forms.TextBox();
            this.btnTestChecksum = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numCsUtilMinRangeLength = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.radioCsUtilRangeUnknown = new System.Windows.Forms.RadioButton();
            this.radioCsUtilRangeLimit = new System.Windows.Forms.RadioButton();
            this.radioCsUtilRangeExact = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioCsUtilCsAfterRange = new System.Windows.Forms.RadioButton();
            this.radioCsUtilCsExact = new System.Windows.Forms.RadioButton();
            this.richChkData = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadCalculationRangeFromResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCsUtilMethods)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilInitValue)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilMinRangeLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridCsUtilMethods
            // 
            this.dataGridCsUtilMethods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCsUtilMethods.Location = new System.Drawing.Point(12, 27);
            this.dataGridCsUtilMethods.Name = "dataGridCsUtilMethods";
            this.dataGridCsUtilMethods.Size = new System.Drawing.Size(524, 169);
            this.dataGridCsUtilMethods.TabIndex = 168;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioCsUtilComplementAll);
            this.groupBox5.Controls.Add(this.radioCSUtilComplement2);
            this.groupBox5.Controls.Add(this.radioCSUtilComplement1);
            this.groupBox5.Controls.Add(this.radioCSUtilComplement0);
            this.groupBox5.Location = new System.Drawing.Point(542, 27);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(96, 97);
            this.groupBox5.TabIndex = 169;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Complement";
            // 
            // radioCsUtilComplementAll
            // 
            this.radioCsUtilComplementAll.AutoSize = true;
            this.radioCsUtilComplementAll.Location = new System.Drawing.Point(5, 66);
            this.radioCsUtilComplementAll.Name = "radioCsUtilComplementAll";
            this.radioCsUtilComplementAll.Size = new System.Drawing.Size(53, 17);
            this.radioCsUtilComplementAll.TabIndex = 133;
            this.radioCsUtilComplementAll.TabStop = true;
            this.radioCsUtilComplementAll.Text = "Try all";
            this.radioCsUtilComplementAll.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilComplement2
            // 
            this.radioCSUtilComplement2.AutoSize = true;
            this.radioCSUtilComplement2.Location = new System.Drawing.Point(5, 47);
            this.radioCSUtilComplement2.Name = "radioCSUtilComplement2";
            this.radioCSUtilComplement2.Size = new System.Drawing.Size(38, 17);
            this.radioCSUtilComplement2.TabIndex = 132;
            this.radioCSUtilComplement2.Text = "2\'s";
            this.radioCSUtilComplement2.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilComplement1
            // 
            this.radioCSUtilComplement1.AutoSize = true;
            this.radioCSUtilComplement1.Location = new System.Drawing.Point(6, 30);
            this.radioCSUtilComplement1.Name = "radioCSUtilComplement1";
            this.radioCSUtilComplement1.Size = new System.Drawing.Size(38, 17);
            this.radioCSUtilComplement1.TabIndex = 131;
            this.radioCSUtilComplement1.Text = "1\'s";
            this.radioCSUtilComplement1.UseVisualStyleBackColor = true;
            // 
            // radioCSUtilComplement0
            // 
            this.radioCSUtilComplement0.AutoSize = true;
            this.radioCSUtilComplement0.Checked = true;
            this.radioCSUtilComplement0.Location = new System.Drawing.Point(6, 15);
            this.radioCSUtilComplement0.Name = "radioCSUtilComplement0";
            this.radioCSUtilComplement0.Size = new System.Drawing.Size(28, 17);
            this.radioCSUtilComplement0.TabIndex = 130;
            this.radioCSUtilComplement0.TabStop = true;
            this.radioCSUtilComplement0.Text = "-";
            this.radioCSUtilComplement0.UseVisualStyleBackColor = true;
            // 
            // chkCSUtilFilterZero
            // 
            this.chkCSUtilFilterZero.AutoSize = true;
            this.chkCSUtilFilterZero.Checked = true;
            this.chkCSUtilFilterZero.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCSUtilFilterZero.Location = new System.Drawing.Point(561, 344);
            this.chkCSUtilFilterZero.Name = "chkCSUtilFilterZero";
            this.chkCSUtilFilterZero.Size = new System.Drawing.Size(63, 17);
            this.chkCSUtilFilterZero.TabIndex = 176;
            this.chkCSUtilFilterZero.Text = "Filter 00";
            this.chkCSUtilFilterZero.UseVisualStyleBackColor = true;
            // 
            // chkCSUtilMatchOnly
            // 
            this.chkCSUtilMatchOnly.AutoSize = true;
            this.chkCSUtilMatchOnly.Checked = true;
            this.chkCSUtilMatchOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCSUtilMatchOnly.Location = new System.Drawing.Point(476, 344);
            this.chkCSUtilMatchOnly.Name = "chkCSUtilMatchOnly";
            this.chkCSUtilMatchOnly.Size = new System.Drawing.Size(79, 17);
            this.chkCSUtilMatchOnly.TabIndex = 173;
            this.chkCSUtilMatchOnly.Text = "Only match";
            this.chkCSUtilMatchOnly.UseVisualStyleBackColor = true;
            this.chkCSUtilMatchOnly.CheckedChanged += new System.EventHandler(this.chkCSUtilMatchOnly_CheckedChanged);
            // 
            // chkCsMSB
            // 
            this.chkCsMSB.AutoSize = true;
            this.chkCsMSB.Checked = true;
            this.chkCsMSB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCsMSB.Location = new System.Drawing.Point(476, 298);
            this.chkCsMSB.Name = "chkCsMSB";
            this.chkCsMSB.Size = new System.Drawing.Size(49, 17);
            this.chkCsMSB.TabIndex = 172;
            this.chkCsMSB.Text = "MSB";
            this.chkCsMSB.UseVisualStyleBackColor = true;
            // 
            // chkCsUtilSwapBytes
            // 
            this.chkCsUtilSwapBytes.AutoSize = true;
            this.chkCsUtilSwapBytes.Location = new System.Drawing.Point(476, 321);
            this.chkCsUtilSwapBytes.Name = "chkCsUtilSwapBytes";
            this.chkCsUtilSwapBytes.Size = new System.Drawing.Size(81, 17);
            this.chkCsUtilSwapBytes.TabIndex = 170;
            this.chkCsUtilSwapBytes.Text = "Swap bytes";
            this.chkCsUtilSwapBytes.UseVisualStyleBackColor = true;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(473, 395);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(49, 13);
            this.label25.TabIndex = 192;
            this.label25.Text = "Threads:";
            // 
            // numCsUtilThreads
            // 
            this.numCsUtilThreads.Location = new System.Drawing.Point(542, 393);
            this.numCsUtilThreads.Name = "numCsUtilThreads";
            this.numCsUtilThreads.Size = new System.Drawing.Size(70, 20);
            this.numCsUtilThreads.TabIndex = 191;
            this.numCsUtilThreads.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // numCsUtilInitValue
            // 
            this.numCsUtilInitValue.Location = new System.Drawing.Point(542, 367);
            this.numCsUtilInitValue.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numCsUtilInitValue.Name = "numCsUtilInitValue";
            this.numCsUtilInitValue.Size = new System.Drawing.Size(70, 20);
            this.numCsUtilInitValue.TabIndex = 190;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(473, 369);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(63, 13);
            this.label24.TabIndex = 189;
            this.label24.Text = "Initial value:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkCsUtilFilter);
            this.groupBox6.Controls.Add(this.btnCsutilSearchBosch);
            this.groupBox6.Location = new System.Drawing.Point(9, 82);
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
            this.btnCsUtilFix.Location = new System.Drawing.Point(476, 269);
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
            this.txtExclude.Location = new System.Drawing.Point(64, 102);
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.Size = new System.Drawing.Size(137, 20);
            this.txtExclude.TabIndex = 184;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 105);
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
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 13);
            this.label18.TabIndex = 181;
            this.label18.Text = "Address:";
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
            this.btnTestChecksum.Location = new System.Drawing.Point(476, 215);
            this.btnTestChecksum.Name = "btnTestChecksum";
            this.btnTestChecksum.Size = new System.Drawing.Size(109, 23);
            this.btnTestChecksum.TabIndex = 178;
            this.btnTestChecksum.Text = "Calculate";
            this.btnTestChecksum.UseVisualStyleBackColor = true;
            this.btnTestChecksum.Click += new System.EventHandler(this.btnTestChecksum_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numCsUtilMinRangeLength);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioCsUtilRangeUnknown);
            this.groupBox1.Controls.Add(this.radioCsUtilRangeLimit);
            this.groupBox1.Controls.Add(this.radioCsUtilRangeExact);
            this.groupBox1.Controls.Add(this.txtChecksumRange);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.txtExclude);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Location = new System.Drawing.Point(12, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 155);
            this.groupBox1.TabIndex = 193;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calculation range";
            // 
            // numCsUtilMinRangeLength
            // 
            this.numCsUtilMinRangeLength.Location = new System.Drawing.Point(117, 127);
            this.numCsUtilMinRangeLength.Margin = new System.Windows.Forms.Padding(2);
            this.numCsUtilMinRangeLength.Maximum = new decimal(new int[] {
            410065408,
            2,
            0,
            0});
            this.numCsUtilMinRangeLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCsUtilMinRangeLength.Name = "numCsUtilMinRangeLength";
            this.numCsUtilMinRangeLength.Size = new System.Drawing.Size(82, 20);
            this.numCsUtilMinRangeLength.TabIndex = 186;
            this.numCsUtilMinRangeLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 128);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 185;
            this.label1.Text = "Min range length:";
            // 
            // radioCsUtilRangeUnknown
            // 
            this.radioCsUtilRangeUnknown.AutoSize = true;
            this.radioCsUtilRangeUnknown.Checked = true;
            this.radioCsUtilRangeUnknown.Location = new System.Drawing.Point(13, 74);
            this.radioCsUtilRangeUnknown.Name = "radioCsUtilRangeUnknown";
            this.radioCsUtilRangeUnknown.Size = new System.Drawing.Size(122, 17);
            this.radioCsUtilRangeUnknown.TabIndex = 183;
            this.radioCsUtilRangeUnknown.TabStop = true;
            this.radioCsUtilRangeUnknown.Text = "Unknown, search all";
            this.radioCsUtilRangeUnknown.UseVisualStyleBackColor = true;
            this.radioCsUtilRangeUnknown.CheckedChanged += new System.EventHandler(this.radioCsUtilRangeUnknown_CheckedChanged);
            // 
            // radioCsUtilRangeLimit
            // 
            this.radioCsUtilRangeLimit.AutoSize = true;
            this.radioCsUtilRangeLimit.Location = new System.Drawing.Point(13, 58);
            this.radioCsUtilRangeLimit.Name = "radioCsUtilRangeLimit";
            this.radioCsUtilRangeLimit.Size = new System.Drawing.Size(169, 17);
            this.radioCsUtilRangeLimit.TabIndex = 182;
            this.radioCsUtilRangeLimit.Text = "Unknown, search inside range";
            this.radioCsUtilRangeLimit.UseVisualStyleBackColor = true;
            // 
            // radioCsUtilRangeExact
            // 
            this.radioCsUtilRangeExact.AutoSize = true;
            this.radioCsUtilRangeExact.Location = new System.Drawing.Point(13, 43);
            this.radioCsUtilRangeExact.Name = "radioCsUtilRangeExact";
            this.radioCsUtilRangeExact.Size = new System.Drawing.Size(52, 17);
            this.radioCsUtilRangeExact.TabIndex = 181;
            this.radioCsUtilRangeExact.Text = "Exact";
            this.radioCsUtilRangeExact.UseVisualStyleBackColor = true;
            this.radioCsUtilRangeExact.CheckedChanged += new System.EventHandler(this.radioCsUtilRangeExact_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioCsUtilCsAfterRange);
            this.groupBox2.Controls.Add(this.radioCsUtilCsExact);
            this.groupBox2.Controls.Add(this.txtCSAddr);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Location = new System.Drawing.Point(235, 202);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(222, 155);
            this.groupBox2.TabIndex = 194;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Checksum Address";
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
            this.richChkData.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richChkData.HideSelection = false;
            this.richChkData.Location = new System.Drawing.Point(12, 363);
            this.richChkData.Name = "richChkData";
            this.richChkData.Size = new System.Drawing.Size(445, 123);
            this.richChkData.TabIndex = 195;
            this.richChkData.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 196;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSettingsToolStripMenuItem,
            this.saveSettingsAsToolStripMenuItem,
            this.saveResultsToolStripMenuItem,
            this.loadCalculationRangeFromResultsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSettingsToolStripMenuItem
            // 
            this.openSettingsToolStripMenuItem.Name = "openSettingsToolStripMenuItem";
            this.openSettingsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.openSettingsToolStripMenuItem.Text = "Open settings";
            this.openSettingsToolStripMenuItem.Click += new System.EventHandler(this.openSettingsToolStripMenuItem_Click);
            // 
            // saveSettingsAsToolStripMenuItem
            // 
            this.saveSettingsAsToolStripMenuItem.Name = "saveSettingsAsToolStripMenuItem";
            this.saveSettingsAsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.saveSettingsAsToolStripMenuItem.Text = "Save Settings as...";
            this.saveSettingsAsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsAsToolStripMenuItem_Click);
            // 
            // saveResultsToolStripMenuItem
            // 
            this.saveResultsToolStripMenuItem.Name = "saveResultsToolStripMenuItem";
            this.saveResultsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.saveResultsToolStripMenuItem.Text = "Results...";
            this.saveResultsToolStripMenuItem.Click += new System.EventHandler(this.saveResultsToolStripMenuItem_Click);
            // 
            // loadCalculationRangeFromResultsToolStripMenuItem
            // 
            this.loadCalculationRangeFromResultsToolStripMenuItem.Name = "loadCalculationRangeFromResultsToolStripMenuItem";
            this.loadCalculationRangeFromResultsToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.loadCalculationRangeFromResultsToolStripMenuItem.Text = "Load Calculation range from results...";
            this.loadCalculationRangeFromResultsToolStripMenuItem.Click += new System.EventHandler(this.loadCalculationRangeFromResultsToolStripMenuItem_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(476, 242);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(109, 23);
            this.btnStop.TabIndex = 197;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // frmChecksumResearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 495);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.richChkData);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.numCsUtilThreads);
            this.Controls.Add(this.numCsUtilInitValue);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.btnCsUtilFix);
            this.Controls.Add(this.btnTestChecksum);
            this.Controls.Add(this.chkCSUtilFilterZero);
            this.Controls.Add(this.chkCSUtilMatchOnly);
            this.Controls.Add(this.chkCsMSB);
            this.Controls.Add(this.chkCsUtilSwapBytes);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.dataGridCsUtilMethods);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmChecksumResearch";
            this.Text = "Checksum Research";
            this.Load += new System.EventHandler(this.frmChecksumResearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCsUtilMethods)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilInitValue)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCsUtilMinRangeLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridCsUtilMethods;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioCsUtilComplementAll;
        private System.Windows.Forms.RadioButton radioCSUtilComplement2;
        private System.Windows.Forms.RadioButton radioCSUtilComplement1;
        private System.Windows.Forms.RadioButton radioCSUtilComplement0;
        private System.Windows.Forms.CheckBox chkCSUtilFilterZero;
        private System.Windows.Forms.CheckBox chkCSUtilMatchOnly;
        private System.Windows.Forms.CheckBox chkCsMSB;
        private System.Windows.Forms.CheckBox chkCsUtilSwapBytes;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.NumericUpDown numCsUtilThreads;
        private System.Windows.Forms.NumericUpDown numCsUtilInitValue;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkCsUtilFilter;
        private System.Windows.Forms.Button btnCsutilSearchBosch;
        private System.Windows.Forms.Button btnCsUtilFix;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtCSAddr;
        private System.Windows.Forms.Label label18;
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
        private System.Windows.Forms.NumericUpDown numCsUtilMinRangeLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveResultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadCalculationRangeFromResultsToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
    }
}