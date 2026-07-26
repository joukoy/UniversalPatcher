using System;
using System.Drawing;
using System.Windows.Forms;

namespace UniversalPatcher
{
    /// <summary>
    /// Teema joka muuttaa FrmTuner:n PID Setup -tyyliseksi.
    ///
    /// KÄYTTÖ — lisää FrmTuner.cs molempiin konstruktoreihin InitializeComponent()-kutsun JÄLKEEN:
    ///     FrmTunerTheme.Apply(this);
    /// </summary>
    public static class FrmTunerTheme
    {
        // ── Väripaletti ─────────────────────────────────────────────────────────
        static readonly Color BgMain = Color.FromArgb(240, 240, 240);
        static readonly Color BgToolbar = Color.FromArgb(232, 232, 232);
        static readonly Color BgGrid = Color.White;
        static readonly Color BgGridHeader = Color.FromArgb(218, 218, 218);
        static readonly Color BgAltRow = Color.FromArgb(247, 247, 250);
        static readonly Color BgSelected = Color.FromArgb(0, 120, 215);
        static readonly Color BgSelectedAlt = Color.FromArgb(204, 228, 247);
        static readonly Color BgTabActive = Color.White;
        static readonly Color BgTabInactive = Color.FromArgb(225, 225, 225);
        static readonly Color BorderColor = Color.FromArgb(185, 185, 185);
        static readonly Color AccentGreen = Color.FromArgb(0, 153, 118);
        static readonly Color AccentHover = Color.FromArgb(0, 178, 138);
        static readonly Color TextMain = Color.FromArgb(25, 25, 25);
        static readonly Color TextMuted = Color.FromArgb(100, 100, 100);

        static readonly Font FontMain = new Font("Segoe UI", 8.5f);
        static readonly Font FontBold = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        static readonly Font FontMono = new Font("Consolas", 8.25f);

        // ── Julkinen sisäänkäynti ────────────────────────────────────────────────
        public static void Apply(Form form)
        {
            form.BackColor = BgMain;
            form.Font = FontMain;
            ApplyControls(form.Controls, form);
        }

        // ── Rekursiivinen kulku ──────────────────────────────────────────────────
        static void ApplyControls(Control.ControlCollection controls, Control parent)
        {
            foreach (Control c in controls)
            {
                switch (c)
                {
                    case MenuStrip ms:
                        StyleMenuStrip(ms);
                        break;

                    case DataGridView dgv:
                        StyleDataGrid(dgv);
                        break;

                    case SplitContainer sc:
                        // Splitter itse toimii erottajana
                        sc.BackColor = BorderColor;
                        sc.Panel1.BackColor = BgMain;
                        sc.Panel2.BackColor = BgMain;
                        ApplyControls(sc.Panel1.Controls, sc.Panel1);
                        ApplyControls(sc.Panel2.Controls, sc.Panel2);
                        break;

                    case TreeView tv:
                        StyleTreeView(tv);
                        break;

                    // TabControl: EI ownerdraw — säilytetään kuvakkeet,
                    // mutta maalataan tausta ja valitun välilehden korostus
                    case TabControl tc:
                        StyleTabControl(tc);
                        foreach (TabPage tp in tc.TabPages)
                        {
                            tp.BackColor = BgMain;
                            ApplyControls(tp.Controls, tp);
                        }
                        break;

                    case RichTextBox rtb:
                        rtb.BackColor = BgGrid;
                        rtb.ForeColor = TextMain;
                        rtb.BorderStyle = BorderStyle.FixedSingle;
                        rtb.Font = rtb.Name.IndexOf("Result",
                            StringComparison.OrdinalIgnoreCase) >= 0 ? FontMono : FontMain;
                        break;

                    case TextBox tb:
                        tb.BackColor = BgGrid;
                        tb.ForeColor = TextMain;
                        tb.BorderStyle = BorderStyle.FixedSingle;
                        tb.Font = FontMain;
                        break;

                    case ComboBox cb:
                        cb.BackColor = BgGrid;
                        cb.ForeColor = TextMain;
                        cb.FlatStyle = FlatStyle.System;
                        cb.Font = FontMain;
                        break;

                    case Button btn:
                        StyleButton(btn);
                        break;

                    case Label lbl:
                        lbl.ForeColor = TextMain;
                        lbl.Font = FontMain;
                        lbl.BackColor = Color.Transparent;
                        break;

                    case RadioButton rb:
                        rb.ForeColor = TextMain;
                        rb.Font = FontMain;
                        rb.BackColor = Color.Transparent;
                        break;

                    case GroupBox grp:
                        grp.BackColor = BgMain;
                        grp.ForeColor = TextMuted;
                        grp.Font = FontMain;
                        ApplyControls(grp.Controls, grp);
                        break;

                    case NumericUpDown nud:
                        nud.BackColor = BgGrid;
                        nud.ForeColor = TextMain;
                        nud.Font = FontMain;
                        break;

                    case Panel pnl:
                        pnl.BackColor = BgMain;
                        ApplyControls(pnl.Controls, pnl);
                        break;

                    default:
                        if (c.Controls.Count > 0)
                            ApplyControls(c.Controls, c);
                        break;
                }
            }
        }

        // ── MenuStrip ────────────────────────────────────────────────────────────
        static void StyleMenuStrip(MenuStrip ms)
        {
            ms.BackColor = BgToolbar;
            ms.ForeColor = TextMain;
            ms.Font = FontMain;
            ms.Renderer = new PidMenuRenderer();
            StyleMenuItems(ms.Items);
        }

        static void StyleMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.BackColor = BgToolbar;
                item.ForeColor = TextMain;
                item.Font = FontMain;
                if (item is ToolStripMenuItem mi)
                    StyleMenuItems(mi.DropDownItems);
            }
        }

        // ── DataGridView ─────────────────────────────────────────────────────────
        static void StyleDataGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = BgMain;
            dgv.GridColor = Color.FromArgb(210, 210, 210);
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.Font = FontMain;

            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BgGridHeader,
                ForeColor = TextMain,
                Font = FontBold,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0),
                SelectionBackColor = BgGridHeader,
                SelectionForeColor = TextMain
            };
            dgv.ColumnHeadersHeight = 26;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BgGrid,
                ForeColor = TextMain,
                SelectionBackColor = BgSelected,
                SelectionForeColor = Color.White,
                Font = FontMain,
                Padding = new Padding(4, 0, 0, 0)
            };

            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = BgAltRow,
                ForeColor = TextMain,
                SelectionBackColor = BgSelected,
                SelectionForeColor = Color.White,
                Font = FontMain,
                Padding = new Padding(4, 0, 0, 0)
            };

            dgv.RowTemplate.Height = 22;

            // Poista vanhat kuuntelijat ensin (Apply voidaan kutsua useasti)
            dgv.CellPainting -= DataGridView_CellPainting;
            dgv.CellPainting += DataGridView_CellPainting;
        }

        // Punainen vasen reunaviiva "modified"-riveille (row.Tag = "modified")
        static void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0) return;
            var row = ((DataGridView)sender).Rows[e.RowIndex];
            if (row.Tag?.ToString() != "modified") return;

            e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
            using (var pen = new Pen(Color.FromArgb(200, 30, 30), 3))
                e.Graphics.DrawLine(pen,
                    e.CellBounds.Left, e.CellBounds.Top,
                    e.CellBounds.Left, e.CellBounds.Bottom - 1);
            e.Handled = true;
        }

        // ── TreeView ─────────────────────────────────────────────────────────────
        static void StyleTreeView(TreeView tv)
        {
            tv.BackColor = BgGrid;
            tv.ForeColor = TextMain;
            tv.BorderStyle = BorderStyle.FixedSingle;
            tv.Font = FontMain;
            tv.ItemHeight = 20;
            // Ei OwnerDraw — säilytetään kuvakkeet (imageList1)
            // Käytetään vain värimuutosta
        }

        // ── TabControl — EI OwnerDraw, jotta kuvakkeet säilyvät ─────────────────
        static void StyleTabControl(TabControl tc)
        {
            // Jätetään DrawMode = Normal niin ImageList toimii normaalisti.
            // Tausta asetetaan vain TabPage-tasolla (yllä).
            tc.Font = FontMain;
            tc.BackColor = BgMain;

            // Väri-illuusio: kuunnellaan Selected-muutos ja päivitetään sivujen taustat
            tc.Selected -= TabControl_Selected;
            tc.Selected += TabControl_Selected;
        }

        static void TabControl_Selected(object sender, TabControlEventArgs e)
        {
            // Ei tarvita toimintoa — BackColor asetettu jo TabPage-tasolla
        }

        // ── Button ────────────────────────────────────────────────────────────────
        static void StyleButton(Button btn)
        {
            // Vihreitä: Execu(te), Flash, Appl(y)
            bool isGreen = btn.Name == "btnFlash"
                        || btn.Name == "btnExecute"
                        || btn.Name == "btnExtraOffsetTestApply";

            // Pieni ikoninappi (Collapse, Expand, nuolet jne.)
            bool isIcon = btn.Width <= 28;

            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = isGreen ? FontBold : FontMain;
            btn.ForeColor = isGreen ? Color.White : TextMain;
            btn.BackColor = isGreen ? AccentGreen
                          : isIcon ? Color.FromArgb(215, 215, 215)
                                    : Color.FromArgb(208, 208, 208);

            btn.FlatAppearance.BorderColor = isGreen ? AccentHover : BorderColor;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = isGreen ? AccentHover
                                                            : Color.FromArgb(195, 195, 200);
            btn.FlatAppearance.MouseDownBackColor = isGreen ? Color.FromArgb(0, 125, 95)
                                                            : Color.FromArgb(178, 178, 182);
            btn.Cursor = Cursors.Hand;
        }

        // ────────────────────────────────────────────────────────────────────────
        // Mukautettu MenuStrip-renderer
        // ────────────────────────────────────────────────────────────────────────
        class PidMenuRenderer : ToolStripProfessionalRenderer
        {
            public PidMenuRenderer() : base(new PidColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected && !e.Item.Pressed) return;
                var r = new Rectangle(Point.Empty, e.Item.Size);
                using (var b = new SolidBrush(BgSelectedAlt))
                    e.Graphics.FillRectangle(b, r);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
        }

        class PidColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => BgToolbar;
            public override Color MenuStripGradientEnd => BgToolbar;
            public override Color MenuItemSelected => BgSelectedAlt;
            public override Color MenuItemBorder => BorderColor;
            public override Color MenuItemSelectedGradientBegin => BgSelectedAlt;
            public override Color MenuItemSelectedGradientEnd => BgSelectedAlt;
            public override Color MenuItemPressedGradientBegin => BgToolbar;
            public override Color MenuItemPressedGradientEnd => BgToolbar;
            public override Color ToolStripDropDownBackground => BgGrid;
            public override Color ImageMarginGradientBegin => BgGrid;
            public override Color ImageMarginGradientMiddle => BgGrid;
            public override Color ImageMarginGradientEnd => BgGrid;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Merkitse rivi punaisella viivalla (muutettu arvo):
    //   dataGridView1.Rows[i].Tag = "modified";
    //   dataGridView1.InvalidateRow(i);
    // ──────────────────────────────────────────────────────────────────────────
}