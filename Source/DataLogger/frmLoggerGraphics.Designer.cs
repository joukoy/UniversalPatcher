
namespace UniversalPatcher
{
    partial class frmLoggerGraphics
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLoggerGraphics));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupPlayback = new System.Windows.Forms.GroupBox();
            this.label39 = new System.Windows.Forms.Label();
            this.numPlaybackSpeed = new System.Windows.Forms.NumericUpDown();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.dataGridSettings = new System.Windows.Forms.DataGridView();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLogSeparator = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabPointValues = new System.Windows.Forms.TabPage();
            this.dataGridPointValues = new System.Windows.Forms.DataGridView();
            this.labelZoom = new System.Windows.Forms.Label();
            this.ScrollPointsPerScreen = new System.Windows.Forms.HScrollBar();
            this.groupLiveSeconds = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numShowMax = new System.Windows.Forms.NumericUpDown();
            this.numDisplayInterval = new System.Windows.Forms.NumericUpDown();
            this.labelShowMax = new System.Windows.Forms.Label();
            this.ScrollStartPoint = new System.Windows.Forms.HScrollBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLastLogfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.loadProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProfileAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoscaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseFunctionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cursorXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cursorYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cursorXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noCursorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseWheelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wheelZoomXToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wheelZoomYToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.wheelZoomXYToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.noWheelZoomToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.resetZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerDisplayData = new System.Windows.Forms.Timer(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.labelDataValues = new System.Windows.Forms.Label();
            this.chkGetLiveData = new System.Windows.Forms.CheckBox();
            this.timerPlayback = new System.Windows.Forms.Timer(this.components);
            this.disableResampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verticalProgressBar1 = new UniversalPatcher.VerticalProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupPlayback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlaybackSpeed)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSettings)).BeginInit();
            this.tabPointValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPointValues)).BeginInit();
            this.groupLiveSeconds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numShowMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDisplayInterval)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Area3DStyle.Enable3D = true;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.Name = "ChartArea1";
            chartArea1.ShadowColor = System.Drawing.Color.White;
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(3, 0);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 2;
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(793, 528);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupPlayback);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1.Controls.Add(this.labelZoom);
            this.splitContainer1.Panel1.Controls.Add(this.ScrollPointsPerScreen);
            this.splitContainer1.Panel1.Controls.Add(this.groupLiveSeconds);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ScrollStartPoint);
            this.splitContainer1.Panel2.Controls.Add(this.chart1);
            this.splitContainer1.Size = new System.Drawing.Size(1199, 551);
            this.splitContainer1.SplitterDistance = 396;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupPlayback
            // 
            this.groupPlayback.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPlayback.Controls.Add(this.label39);
            this.groupPlayback.Controls.Add(this.numPlaybackSpeed);
            this.groupPlayback.Controls.Add(this.btnPlay);
            this.groupPlayback.Controls.Add(this.btnPause);
            this.groupPlayback.Location = new System.Drawing.Point(4, 453);
            this.groupPlayback.Name = "groupPlayback";
            this.groupPlayback.Size = new System.Drawing.Size(389, 53);
            this.groupPlayback.TabIndex = 32;
            this.groupPlayback.TabStop = false;
            this.groupPlayback.Text = "Playback";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(108, 26);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(41, 13);
            this.label39.TabIndex = 4;
            this.label39.Text = "Speed:";
            // 
            // numPlaybackSpeed
            // 
            this.numPlaybackSpeed.DecimalPlaces = 2;
            this.numPlaybackSpeed.Location = new System.Drawing.Point(171, 24);
            this.numPlaybackSpeed.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPlaybackSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numPlaybackSpeed.Name = "numPlaybackSpeed";
            this.numPlaybackSpeed.Size = new System.Drawing.Size(55, 20);
            this.numPlaybackSpeed.TabIndex = 3;
            this.numPlaybackSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPlaybackSpeed.ValueChanged += new System.EventHandler(this.numPlaybackSpeed_ValueChanged);
            // 
            // btnPlay
            // 
            this.btnPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlay.Location = new System.Drawing.Point(54, 21);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(40, 26);
            this.btnPlay.TabIndex = 1;
            this.btnPlay.Text = ">";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(8, 21);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(40, 26);
            this.btnPause.TabIndex = 0;
            this.btnPause.Text = "| |";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabPointValues);
            this.tabControl1.Location = new System.Drawing.Point(3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(390, 385);
            this.tabControl1.TabIndex = 14;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.chkSelectAll);
            this.tabSettings.Controls.Add(this.dataGridSettings);
            this.tabSettings.Controls.Add(this.comboBox1);
            this.tabSettings.Controls.Add(this.label1);
            this.tabSettings.Controls.Add(this.txtLogSeparator);
            this.tabSettings.Controls.Add(this.btnApply);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(382, 359);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(144, 335);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 14;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // dataGridSettings
            // 
            this.dataGridSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridSettings.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridSettings.Location = new System.Drawing.Point(3, 33);
            this.dataGridSettings.Name = "dataGridSettings";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridSettings.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridSettings.RowHeadersWidth = 5;
            this.dataGridSettings.Size = new System.Drawing.Size(379, 294);
            this.dataGridSettings.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(376, 21);
            this.comboBox1.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 339);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Log separator:";
            // 
            // txtLogSeparator
            // 
            this.txtLogSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogSeparator.Location = new System.Drawing.Point(87, 333);
            this.txtLogSeparator.Name = "txtLogSeparator";
            this.txtLogSeparator.Size = new System.Drawing.Size(39, 20);
            this.txtLogSeparator.TabIndex = 1;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(296, 332);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnShowData_Click);
            // 
            // tabPointValues
            // 
            this.tabPointValues.Controls.Add(this.dataGridPointValues);
            this.tabPointValues.Location = new System.Drawing.Point(4, 22);
            this.tabPointValues.Name = "tabPointValues";
            this.tabPointValues.Padding = new System.Windows.Forms.Padding(3);
            this.tabPointValues.Size = new System.Drawing.Size(382, 359);
            this.tabPointValues.TabIndex = 1;
            this.tabPointValues.Text = "Values";
            this.tabPointValues.UseVisualStyleBackColor = true;
            // 
            // dataGridPointValues
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridPointValues.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridPointValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridPointValues.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridPointValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridPointValues.Location = new System.Drawing.Point(3, 3);
            this.dataGridPointValues.Name = "dataGridPointValues";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridPointValues.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridPointValues.Size = new System.Drawing.Size(376, 353);
            this.dataGridPointValues.TabIndex = 0;
            // 
            // labelZoom
            // 
            this.labelZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(13, 511);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(34, 13);
            this.labelZoom.TabIndex = 12;
            this.labelZoom.Text = "Zoom";
            // 
            // ScrollPointsPerScreen
            // 
            this.ScrollPointsPerScreen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScrollPointsPerScreen.Location = new System.Drawing.Point(3, 528);
            this.ScrollPointsPerScreen.Maximum = 1000;
            this.ScrollPointsPerScreen.Minimum = 10;
            this.ScrollPointsPerScreen.Name = "ScrollPointsPerScreen";
            this.ScrollPointsPerScreen.Size = new System.Drawing.Size(390, 20);
            this.ScrollPointsPerScreen.TabIndex = 0;
            this.ScrollPointsPerScreen.Value = 100;
            this.ScrollPointsPerScreen.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollPointsPerScreen_Scroll);
            // 
            // groupLiveSeconds
            // 
            this.groupLiveSeconds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupLiveSeconds.Controls.Add(this.verticalProgressBar1);
            this.groupLiveSeconds.Controls.Add(this.label2);
            this.groupLiveSeconds.Controls.Add(this.numShowMax);
            this.groupLiveSeconds.Controls.Add(this.numDisplayInterval);
            this.groupLiveSeconds.Controls.Add(this.labelShowMax);
            this.groupLiveSeconds.Location = new System.Drawing.Point(3, 387);
            this.groupLiveSeconds.Name = "groupLiveSeconds";
            this.groupLiveSeconds.Size = new System.Drawing.Size(389, 66);
            this.groupLiveSeconds.TabIndex = 8;
            this.groupLiveSeconds.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Display interval seconds";
            // 
            // numShowMax
            // 
            this.numShowMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numShowMax.Location = new System.Drawing.Point(188, 13);
            this.numShowMax.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numShowMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numShowMax.Name = "numShowMax";
            this.numShowMax.Size = new System.Drawing.Size(40, 20);
            this.numShowMax.TabIndex = 5;
            this.numShowMax.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numDisplayInterval
            // 
            this.numDisplayInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numDisplayInterval.DecimalPlaces = 1;
            this.numDisplayInterval.Location = new System.Drawing.Point(188, 40);
            this.numDisplayInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numDisplayInterval.Name = "numDisplayInterval";
            this.numDisplayInterval.Size = new System.Drawing.Size(39, 20);
            this.numDisplayInterval.TabIndex = 7;
            this.numDisplayInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDisplayInterval.ValueChanged += new System.EventHandler(this.numDisplayInterval_ValueChanged);
            // 
            // labelShowMax
            // 
            this.labelShowMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelShowMax.AutoSize = true;
            this.labelShowMax.Location = new System.Drawing.Point(6, 15);
            this.labelShowMax.Name = "labelShowMax";
            this.labelShowMax.Size = new System.Drawing.Size(99, 13);
            this.labelShowMax.TabIndex = 4;
            this.labelShowMax.Text = "Show last seconds:";
            // 
            // ScrollStartPoint
            // 
            this.ScrollStartPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScrollStartPoint.Location = new System.Drawing.Point(0, 529);
            this.ScrollStartPoint.Maximum = 1000;
            this.ScrollStartPoint.Name = "ScrollStartPoint";
            this.ScrollStartPoint.Size = new System.Drawing.Size(790, 20);
            this.ScrollStartPoint.TabIndex = 3;
            this.ScrollStartPoint.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollStartPoint_Scroll);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1199, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadLogfileToolStripMenuItem,
            this.loadLastLogfileToolStripMenuItem,
            this.toolStripSeparator1,
            this.loadProfileToolStripMenuItem,
            this.saveProfileToolStripMenuItem,
            this.saveProfileAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadLogfileToolStripMenuItem
            // 
            this.loadLogfileToolStripMenuItem.Name = "loadLogfileToolStripMenuItem";
            this.loadLogfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.loadLogfileToolStripMenuItem.Text = "Load logfile";
            this.loadLogfileToolStripMenuItem.Click += new System.EventHandler(this.loadLogfileToolStripMenuItem_Click);
            // 
            // loadLastLogfileToolStripMenuItem
            // 
            this.loadLastLogfileToolStripMenuItem.Name = "loadLastLogfileToolStripMenuItem";
            this.loadLastLogfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.loadLastLogfileToolStripMenuItem.Text = "Load last logfile";
            this.loadLastLogfileToolStripMenuItem.Click += new System.EventHandler(this.loadLastLogfileToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // loadProfileToolStripMenuItem
            // 
            this.loadProfileToolStripMenuItem.Name = "loadProfileToolStripMenuItem";
            this.loadProfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.loadProfileToolStripMenuItem.Text = "Load profile";
            this.loadProfileToolStripMenuItem.Click += new System.EventHandler(this.loadProfileToolStripMenuItem_Click);
            // 
            // saveProfileToolStripMenuItem
            // 
            this.saveProfileToolStripMenuItem.Name = "saveProfileToolStripMenuItem";
            this.saveProfileToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveProfileToolStripMenuItem.Text = "Save profile";
            this.saveProfileToolStripMenuItem.Click += new System.EventHandler(this.saveProfileToolStripMenuItem_Click);
            // 
            // saveProfileAsToolStripMenuItem
            // 
            this.saveProfileAsToolStripMenuItem.Name = "saveProfileAsToolStripMenuItem";
            this.saveProfileAsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveProfileAsToolStripMenuItem.Text = "Save profile as...";
            this.saveProfileAsToolStripMenuItem.Click += new System.EventHandler(this.saveProfileAsToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showPointsToolStripMenuItem,
            this.autoscaleToolStripMenuItem,
            this.mouseFunctionToolStripMenuItem,
            this.mouseWheelToolStripMenuItem,
            this.resetZoomToolStripMenuItem,
            this.disableResampleToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // showPointsToolStripMenuItem
            // 
            this.showPointsToolStripMenuItem.Name = "showPointsToolStripMenuItem";
            this.showPointsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showPointsToolStripMenuItem.Text = "Show points";
            this.showPointsToolStripMenuItem.Click += new System.EventHandler(this.showPointsToolStripMenuItem_Click);
            // 
            // autoscaleToolStripMenuItem
            // 
            this.autoscaleToolStripMenuItem.Name = "autoscaleToolStripMenuItem";
            this.autoscaleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.autoscaleToolStripMenuItem.Text = "Autoscale";
            this.autoscaleToolStripMenuItem.Click += new System.EventHandler(this.autoscaleToolStripMenuItem_Click);
            // 
            // mouseFunctionToolStripMenuItem
            // 
            this.mouseFunctionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomXToolStripMenuItem,
            this.zoomYToolStripMenuItem,
            this.zoomXYToolStripMenuItem,
            this.noZoomToolStripMenuItem,
            this.toolStripSeparator2,
            this.cursorXToolStripMenuItem,
            this.cursorYToolStripMenuItem,
            this.cursorXYToolStripMenuItem,
            this.noCursorToolStripMenuItem});
            this.mouseFunctionToolStripMenuItem.Name = "mouseFunctionToolStripMenuItem";
            this.mouseFunctionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mouseFunctionToolStripMenuItem.Text = "Mouse function";
            // 
            // zoomXToolStripMenuItem
            // 
            this.zoomXToolStripMenuItem.Name = "zoomXToolStripMenuItem";
            this.zoomXToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.zoomXToolStripMenuItem.Text = "Zoom X";
            this.zoomXToolStripMenuItem.Click += new System.EventHandler(this.zoomXToolStripMenuItem_Click);
            // 
            // zoomYToolStripMenuItem
            // 
            this.zoomYToolStripMenuItem.Name = "zoomYToolStripMenuItem";
            this.zoomYToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.zoomYToolStripMenuItem.Text = "Zoom Y";
            this.zoomYToolStripMenuItem.Click += new System.EventHandler(this.zoomYToolStripMenuItem_Click);
            // 
            // zoomXYToolStripMenuItem
            // 
            this.zoomXYToolStripMenuItem.Name = "zoomXYToolStripMenuItem";
            this.zoomXYToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.zoomXYToolStripMenuItem.Text = "Zoom XY";
            this.zoomXYToolStripMenuItem.Click += new System.EventHandler(this.zoomXYToolStripMenuItem_Click);
            // 
            // noZoomToolStripMenuItem
            // 
            this.noZoomToolStripMenuItem.Name = "noZoomToolStripMenuItem";
            this.noZoomToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.noZoomToolStripMenuItem.Text = "No Zoom";
            this.noZoomToolStripMenuItem.Click += new System.EventHandler(this.noZoomToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(125, 6);
            // 
            // cursorXToolStripMenuItem
            // 
            this.cursorXToolStripMenuItem.Name = "cursorXToolStripMenuItem";
            this.cursorXToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.cursorXToolStripMenuItem.Text = "Cursor X";
            this.cursorXToolStripMenuItem.Click += new System.EventHandler(this.cusrorXToolStripMenuItem_Click);
            // 
            // cursorYToolStripMenuItem
            // 
            this.cursorYToolStripMenuItem.Name = "cursorYToolStripMenuItem";
            this.cursorYToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.cursorYToolStripMenuItem.Text = "Cursor Y";
            this.cursorYToolStripMenuItem.Click += new System.EventHandler(this.cursorYToolStripMenuItem_Click);
            // 
            // cursorXYToolStripMenuItem
            // 
            this.cursorXYToolStripMenuItem.Name = "cursorXYToolStripMenuItem";
            this.cursorXYToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.cursorXYToolStripMenuItem.Text = "Cursor XY";
            this.cursorXYToolStripMenuItem.Click += new System.EventHandler(this.cursorXYToolStripMenuItem_Click);
            // 
            // noCursorToolStripMenuItem
            // 
            this.noCursorToolStripMenuItem.Name = "noCursorToolStripMenuItem";
            this.noCursorToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.noCursorToolStripMenuItem.Text = "No Cursor";
            this.noCursorToolStripMenuItem.Click += new System.EventHandler(this.noCursorToolStripMenuItem_Click);
            // 
            // mouseWheelToolStripMenuItem
            // 
            this.mouseWheelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wheelZoomXToolStripMenuItem1,
            this.wheelZoomYToolStripMenuItem1,
            this.wheelZoomXYToolStripMenuItem1,
            this.noWheelZoomToolStripMenuItem1});
            this.mouseWheelToolStripMenuItem.Name = "mouseWheelToolStripMenuItem";
            this.mouseWheelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mouseWheelToolStripMenuItem.Text = "Mouse wheel";
            // 
            // wheelZoomXToolStripMenuItem1
            // 
            this.wheelZoomXToolStripMenuItem1.Name = "wheelZoomXToolStripMenuItem1";
            this.wheelZoomXToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.wheelZoomXToolStripMenuItem1.Text = "Zoom X";
            this.wheelZoomXToolStripMenuItem1.Click += new System.EventHandler(this.zoomXToolStripMenuItem1_Click);
            // 
            // wheelZoomYToolStripMenuItem1
            // 
            this.wheelZoomYToolStripMenuItem1.Name = "wheelZoomYToolStripMenuItem1";
            this.wheelZoomYToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.wheelZoomYToolStripMenuItem1.Text = "Zoom Y";
            this.wheelZoomYToolStripMenuItem1.Click += new System.EventHandler(this.zoomYToolStripMenuItem1_Click);
            // 
            // wheelZoomXYToolStripMenuItem1
            // 
            this.wheelZoomXYToolStripMenuItem1.Name = "wheelZoomXYToolStripMenuItem1";
            this.wheelZoomXYToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.wheelZoomXYToolStripMenuItem1.Text = "Zoom XY";
            this.wheelZoomXYToolStripMenuItem1.Click += new System.EventHandler(this.zoomXYToolStripMenuItem1_Click);
            // 
            // noWheelZoomToolStripMenuItem1
            // 
            this.noWheelZoomToolStripMenuItem1.Name = "noWheelZoomToolStripMenuItem1";
            this.noWheelZoomToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
            this.noWheelZoomToolStripMenuItem1.Text = "No zoom";
            this.noWheelZoomToolStripMenuItem1.Click += new System.EventHandler(this.noZoomToolStripMenuItem1_Click);
            // 
            // resetZoomToolStripMenuItem
            // 
            this.resetZoomToolStripMenuItem.Name = "resetZoomToolStripMenuItem";
            this.resetZoomToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetZoomToolStripMenuItem.Text = "Reset zoom";
            this.resetZoomToolStripMenuItem.Click += new System.EventHandler(this.resetZoomToolStripMenuItem_Click);
            // 
            // timerDisplayData
            // 
            this.timerDisplayData.Interval = 3000;
            this.timerDisplayData.Tick += new System.EventHandler(this.timerDisplayData_Tick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 27);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(1199, 640);
            this.splitContainer2.SplitterDistance = 551;
            this.splitContainer2.TabIndex = 4;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtResult);
            this.splitContainer3.Size = new System.Drawing.Size(1199, 89);
            this.splitContainer3.SplitterDistance = 157;
            this.splitContainer3.TabIndex = 1;
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.HideSelection = false;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(1038, 89);
            this.txtResult.TabIndex = 0;
            this.txtResult.Text = "";
            // 
            // labelDataValues
            // 
            this.labelDataValues.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDataValues.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelDataValues.Location = new System.Drawing.Point(0, 674);
            this.labelDataValues.Name = "labelDataValues";
            this.labelDataValues.Size = new System.Drawing.Size(1199, 15);
            this.labelDataValues.TabIndex = 5;
            this.labelDataValues.Text = "Click data point to show values";
            // 
            // chkGetLiveData
            // 
            this.chkGetLiveData.AutoSize = true;
            this.chkGetLiveData.Location = new System.Drawing.Point(252, 4);
            this.chkGetLiveData.Name = "chkGetLiveData";
            this.chkGetLiveData.Size = new System.Drawing.Size(149, 17);
            this.chkGetLiveData.TabIndex = 18;
            this.chkGetLiveData.Text = "Receive data from Logger";
            this.chkGetLiveData.UseVisualStyleBackColor = true;
            this.chkGetLiveData.CheckedChanged += new System.EventHandler(this.chkGetLiveData_CheckedChanged);
            // 
            // timerPlayback
            // 
            this.timerPlayback.Tick += new System.EventHandler(this.timerPlayback_Tick);
            // 
            // disableResampleToolStripMenuItem
            // 
            this.disableResampleToolStripMenuItem.Name = "disableResampleToolStripMenuItem";
            this.disableResampleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disableResampleToolStripMenuItem.Text = "Disable resample";
            this.disableResampleToolStripMenuItem.Click += new System.EventHandler(this.disableResampleToolStripMenuItem_Click);
            // 
            // verticalProgressBar1
            // 
            this.verticalProgressBar1.Location = new System.Drawing.Point(290, 25);
            this.verticalProgressBar1.Name = "verticalProgressBar1";
            this.verticalProgressBar1.Size = new System.Drawing.Size(100, 23);
            this.verticalProgressBar1.TabIndex = 8;
            // 
            // frmLoggerGraphics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 689);
            this.Controls.Add(this.labelDataValues);
            this.Controls.Add(this.chkGetLiveData);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmLoggerGraphics";
            this.Text = "frmLoggerGraphics";
            this.Load += new System.EventHandler(this.frmLoggerGraphics_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupPlayback.ResumeLayout(false);
            this.groupPlayback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPlaybackSpeed)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSettings)).EndInit();
            this.tabPointValues.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPointValues)).EndInit();
            this.groupLiveSeconds.ResumeLayout(false);
            this.groupLiveSeconds.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numShowMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDisplayInterval)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLogfileToolStripMenuItem;
        private System.Windows.Forms.TextBox txtLogSeparator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridSettings;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.NumericUpDown numShowMax;
        private System.Windows.Forms.Label labelShowMax;
        private System.Windows.Forms.Timer timerDisplayData;
        private System.Windows.Forms.NumericUpDown numDisplayInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem loadProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProfileAsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupLiveSeconds;
        private System.Windows.Forms.ToolStripMenuItem loadLastLogfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Label labelDataValues;
        private System.Windows.Forms.CheckBox chkGetLiveData;
        private System.Windows.Forms.HScrollBar ScrollPointsPerScreen;
        private System.Windows.Forms.Label labelZoom;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabPointValues;
        private System.Windows.Forms.DataGridView dataGridPointValues;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoscaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mouseFunctionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomXYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cursorXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cursorYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cursorXYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noZoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noCursorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.HScrollBar ScrollStartPoint;
        private System.Windows.Forms.Timer timerPlayback;
        private System.Windows.Forms.GroupBox groupPlayback;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.NumericUpDown numPlaybackSpeed;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.ToolStripMenuItem mouseWheelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wheelZoomXToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem wheelZoomYToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem wheelZoomXYToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem noWheelZoomToolStripMenuItem1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private VerticalProgressBar verticalProgressBar1;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.ToolStripMenuItem disableResampleToolStripMenuItem;
    }
}