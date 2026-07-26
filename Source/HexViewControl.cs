using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace UniversalPatcher
{
    /// <summary>
    /// Nopea GDI+-pohjainen hex-näyttö, korvaa DataGridView'n.
    /// Piirtää suoraan OnPaint-metodissa — ei solukohtaisia kontrolliinstansseja.
    /// </summary>
    public class HexViewControl : UserControl
    {
        // ─── Julkiset tapahtumat ──────────────────────────────────────────────────
        public event EventHandler<HexSelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<HexScrollEventArgs>           Scrolled;

        // ─── Kirjasin / mitat ────────────────────────────────────────────────────
        private Font   _font      = new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point);
        private int    _cellW     = 35;   // Solun leveys pikseleinä
        private int    _cellH     = 18;   // Solun korkeus pikseleinä
        private int    _headerW   = 80;   // Riviotsikon leveys

        // Kirjaimen koon mukaan laskettuna päivitetään EnsureMeasurements()-kutsussa
        private int    _charW, _charH;

        // ─── Data ────────────────────────────────────────────────────────────────
        /// <summary>DGROW-rivit (sama rakenne kuin alkuperäisessä koodissa).</summary>
        private List<frmTableVisDouble2.DGROW> _rows = new List<frmTableVisDouble2.DGROW>();
        /// <summary>HexData-taulukko, indeksoitu (address - buffOffset).</summary>
        private frmTableVisDouble2.HexData[] _hexDatas;
        private int _buffOffset;
        /// <summary>PCM-puskuri tavuille.</summary>
        private byte[] _pcmBuf;

        // ─── Vieritys ────────────────────────────────────────────────────────────
        private VScrollBar _vscroll;
        private int _firstVisibleRow  = 0;
        private int _visibleRowCount  = 0;

        // ─── Valinta ─────────────────────────────────────────────────────────────
        // Valitut osoitteet tallennetaan HashSetiin nopeaa tarkistusta varten
        private HashSet<uint>   _selectedAddresses = new HashSet<uint>();
        private Point           _mouseDownCell     = new Point(-1, -1); // (col, row)
        private bool            _dragging          = false;

        // Hakutulokset
        private HashSet<uint>   _foundBytes        = new HashSet<uint>();

        // ─── Muut visuaaliset tilat ───────────────────────────────────────────────
        private int  _bytesPerRow = 16;
        public int BytesPerRow
        {
            get => _bytesPerRow;
            set { _bytesPerRow = value; Invalidate(); }
        }

        private int _headerWidth = 80;
        public int RowHeaderWidth
        {
            get => _headerWidth;
            set { _headerWidth = value; RecalcLayout(); Invalidate(); }
        }

        // ─── Rakentaja ───────────────────────────────────────────────────────────
        public HexViewControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.UserPaint, true);

            BackColor = Color.White;
            BorderStyle = BorderStyle.Fixed3D;

            _vscroll = new VScrollBar
            {
                Dock    = DockStyle.Right,
                Minimum = 0,
                Maximum = 0,
                SmallChange = 1,
                LargeChange = 10
            };
            _vscroll.ValueChanged += (s, e) =>
            {
                _firstVisibleRow = _vscroll.Value;
                Invalidate();
                Scrolled?.Invoke(this, new HexScrollEventArgs(_firstVisibleRow));
            };
            Controls.Add(_vscroll);
            EnsureMeasurements();
        }

        // ─── Julkiset metodit ────────────────────────────────────────────────────

        /// <summary>Asettaa piirrettävän datan. Kutsu tätä aina kun data muuttuu.</summary>
        public void SetData(
            List<frmTableVisDouble2.DGROW>    rows,
            frmTableVisDouble2.HexData[]      hexDatas,
            int                              buffOffset,
            byte[]                           pcmBuf)
        {
            _rows       = rows       ?? new List<frmTableVisDouble2.DGROW>();
            _hexDatas   = hexDatas;
            _buffOffset = buffOffset;
            _pcmBuf     = pcmBuf;
            _firstVisibleRow = 0;
            UpdateScrollbar();
            Invalidate();
        }

        public int RowCount => _rows?.Count ?? 0;

        /// <summary>Ensimmäinen näkyvä rivi (vierityksen tila).</summary>
        public int FirstDisplayedScrollingRowIndex
        {
            get => _firstVisibleRow;
            set
            {
                if (_rows == null) return;
                int max = Math.Max(0, _rows.Count - 1);
                _firstVisibleRow = Math.Max(0, Math.Min(value, max));
                _vscroll.Value   = Math.Max(_vscroll.Minimum,
                                   Math.Min(_vscroll.Maximum, _firstVisibleRow));
                Invalidate();
            }
        }

        /// <summary>Haku-korostukset (osoitejoukkona).</summary>
        public void SetFoundBytes(IEnumerable<uint> addresses)
        {
            _foundBytes = new HashSet<uint>(addresses);
            Invalidate();
        }
        public Font TextFont
        {
            get { return _font; }
            set 
            { 
                _font = value;
                Invalidate();
            }
        }

        // ─── Valintarajapinta ─────────────────────────────────────────────────────

        public List<uint> GetSelectedAddresses()
        {
            var list = new List<uint>(_selectedAddresses);
            list.Sort();
            return list;
        }

        public void ClearSelection()
        {
            _selectedAddresses.Clear();
            Invalidate();
            SelectionChanged?.Invoke(this, new HexSelectionChangedEventArgs(_selectedAddresses));
        }

        /// <summary>Palauttaa valittujen solujen lukumäärän.</summary>
        public int SelectedCount => _selectedAddresses.Count;

        /// <summary>Valitsee tietyn osoitteen mukaisen solun.</summary>
        public void SelectAddress(uint address)
        {
            _selectedAddresses.Clear();
            _selectedAddresses.Add(address);
            Invalidate();
            SelectionChanged?.Invoke(this, new HexSelectionChangedEventArgs(_selectedAddresses));
        }

        /// <summary>Korvaa valinnan annetulla osoitejoukolla (synkronointi).</summary>
        public void SetSelectedAddresses(IEnumerable<uint> addresses)
        {
            _selectedAddresses = new HashSet<uint>(addresses);
            Invalidate();
            // Älä nosta tapahtumaa — kutsutu synkronoinnin yhteydessä, ei käyttäjän toimesta
        }

        // ─── Piirto ──────────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_rows == null || _rows.Count == 0) return;

            Graphics g      = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            EnsureMeasurements();
            RecalcLayout();

            int drawX = _headerWidth;
            int drawY = 0;

            Brush whiteBrush   = Brushes.White;
            Brush yellowBrush  = Brushes.Yellow;
            Brush selectedBrush= new SolidBrush(Color.FromArgb(150, 100, 149, 237)); // Cornflower, puoliläpinäkyvä
            Pen   gridPen      = new Pen(Color.FromArgb(230, 230, 230));

            for (int rowIdx = _firstVisibleRow;
                 rowIdx < _rows.Count && drawY < ClientHeight;
                 rowIdx++)
            {
                frmTableVisDouble2.DGROW dgrow = _rows[rowIdx];

                // Riviotsikko
                DrawRowHeader(g, dgrow.HeaderTxt, rowIdx, drawY);

                // Solut
                for (int colIdx = 0; colIdx < dgrow.Cols.Count; colIdx++)
                {
                    int   x    = drawX + colIdx * _cellW;
                    int   y    = drawY;
                    uint  addr = dgrow.Addresses[colIdx];

                    // Taustaväri
                    bool isFound    = _foundBytes.Contains(addr);
                    bool isSelected = _selectedAddresses.Contains(addr);

                    Brush bgBrush = isFound ? yellowBrush : whiteBrush;
                    g.FillRectangle(bgBrush, x, y, _cellW, _cellH);

                    if (isSelected)
                        g.FillRectangle(selectedBrush, x, y, _cellW, _cellH);

                    // Ruudukko
                    //g.DrawRectangle(gridPen, x, y, _cellW - 1, _cellH - 1);

                    // Teksti ja väri
                    Color  fgColor   = Color.Black;
                    string cellText  = "??";
                    bool   isBold    = false;

                    if (_hexDatas != null && _pcmBuf != null)
                    {
                        int bufIdx = (int)addr - _buffOffset;
                        if (bufIdx >= 0 && bufIdx < _hexDatas.Length)
                        {
                            frmTableVisDouble2.HexData hxd = _hexDatas[bufIdx];
                            fgColor  = hxd.Color != Color.Empty ? hxd.Color : Color.Black;
                            isBold   = hxd.SelectedTD;
                            char pre = hxd.Prefix != '\0' ? hxd.Prefix : ' ';
                            char suf = hxd.Suffix != '\0' ? hxd.Suffix : ' ';
                            cellText = pre + _pcmBuf[addr].ToString("X2") + suf;
                        }
                        else if (addr < _pcmBuf.Length)
                        {
                            cellText = " " + _pcmBuf[addr].ToString("X2") + " ";
                        }
                    }
                    else if (colIdx < dgrow.Cols.Count)
                    {
                        cellText = " " + dgrow.Cols[colIdx] + " ";
                    }

                    Font drawFont = isBold ? new Font(_font, FontStyle.Bold) : _font;
                    using (Brush textBrush = new SolidBrush(fgColor))
                    {
                        g.DrawString(cellText, drawFont, textBrush,
                            x + 1, y + (_cellH - _charH) / 2);
                    }
                    if (isBold) drawFont.Dispose();
                }

                drawY += _cellH;
            }

            // Oikea reuna (tyhjä alue)
            if (drawY < ClientHeight)
                g.FillRectangle(SystemBrushes.Control,
                    0, drawY, ClientWidth, ClientHeight - drawY);

            gridPen.Dispose();
            ((IDisposable)selectedBrush).Dispose();
        }

        private void DrawRowHeader(Graphics g, string text, int rowIdx, int y)
        {
            Rectangle r    = new Rectangle(0, y, _headerWidth - 1, _cellH);
            bool isSelected = false;
            if (_rows[rowIdx].Addresses.Count > 0)
                isSelected = _selectedAddresses.Contains(_rows[rowIdx].Addresses[0]);

            Brush bg = isSelected
                ? SystemBrushes.Highlight
                : SystemBrushes.Control;

            g.FillRectangle(bg, r);
            g.DrawRectangle(SystemPens.ControlDark, r);

            Color fg = isSelected ? Color.White : Color.Black;
            using (Brush tb = new SolidBrush(fg))
            {
                string display = text ?? "";
                //if (display.Length * _charW > _headerWidth - 4)
                  //  display = display.Substring(0,Math.Max(0,display.Length - (_headerWidth - 4) / Math.Max(1, _charW)));
                //display = display.Substring(Math.Max(0, display.Length - (_headerWidth - 4) / Math.Max(1,_charW)));
                g.DrawString(display, _font, tb, 2, y + (_cellH - _charH) / 2);
            }
        }

        // ─── Layout ──────────────────────────────────────────────────────────────

        private void RecalcLayout()
        {
            _visibleRowCount = ClientHeight / Math.Max(1, _cellH);
        }

        private void EnsureMeasurements()
        {
            using (Graphics g = CreateGraphics())
            {
                SizeF sz = g.MeasureString("XX", _font);
                _charW = (int)Math.Ceiling(sz.Width / 2);
                _charH = (int)Math.Ceiling(sz.Height);
            }
            // Solun leveys: 3 merkkiä (prefix + 2 hex) + pieni reunus
            _cellW = _charW * 3;
            _cellH = _charH + 3;
        }

        private void UpdateScrollbar()
        {
            if (_rows == null || _rows.Count == 0)
            {
                _vscroll.Maximum = 0;
                return;
            }
            RecalcLayout();
            int max = Math.Max(0, _rows.Count - _visibleRowCount);
            _vscroll.Maximum     = max + _vscroll.LargeChange;
            _vscroll.LargeChange = Math.Max(1, _visibleRowCount);
            _firstVisibleRow     = Math.Min(_firstVisibleRow, max);
        }

        // ─── Hiiri ───────────────────────────────────────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            Point cell = HitTest(e.X, e.Y);
            if (cell.X < 0) return;

            _mouseDownCell = cell;
            _dragging      = true;

            if ((ModifierKeys & Keys.Control) == 0 &&
                (ModifierKeys & Keys.Shift)   == 0)
            {
                _selectedAddresses.Clear();
            }

            SelectRange(_mouseDownCell, cell);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging || e.Button != MouseButtons.Left) return;

            Point cell = HitTest(e.X, e.Y);
            if (cell.X < 0) return;

            if ((ModifierKeys & Keys.Control) == 0)
                _selectedAddresses.Clear();

            SelectRange(_mouseDownCell, cell);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;

            Point cell = HitTest(e.X, e.Y);
            if (cell.X >= 0)
            {
                if ((ModifierKeys & Keys.Control) == 0 &&
                    (ModifierKeys & Keys.Shift)   == 0)
                    _selectedAddresses.Clear();

                SelectRange(_mouseDownCell, cell);
                SelectionChanged?.Invoke(this,
                    new HexSelectionChangedEventArgs(_selectedAddresses));
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int delta = -(e.Delta / 120) * 3;
            int newVal = Math.Max(_vscroll.Minimum,
                         Math.Min(_vscroll.Maximum - _vscroll.LargeChange + 1,
                                  _vscroll.Value + delta));
            _vscroll.Value   = newVal;
            _firstVisibleRow = newVal;
            Invalidate();
            Scrolled?.Invoke(this, new HexScrollEventArgs(_firstVisibleRow));
        }

        // ─── Näppäimistö ─────────────────────────────────────────────────────────

        protected override bool IsInputKey(Keys keyData) => true;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Control && e.KeyCode == Keys.C)
            {
                // Kopioi valitut tavut leikepöydälle
                var addrs = GetSelectedAddresses();
                if (_pcmBuf != null && addrs.Count > 0)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (uint a in addrs)
                        if (a < _pcmBuf.Length)
                            sb.Append(_pcmBuf[a].ToString("X2") + " ");
                    Clipboard.SetDataObject(sb.ToString().Trim());
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract)
            {
                // Välitetään ylöspäin lomakkeelle
                OnKeyDown(e); // nousee tapahtumakäsittelijöihin
            }
        }

        // ─── Koko / Layout-muutos ────────────────────────────────────────────────

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalcLayout();
            UpdateScrollbar();
            Invalidate();
        }

        // ─── Apumetodit ──────────────────────────────────────────────────────────

        /// <summary>Muuntaa pikselikoordinaatit (col, row) -pariksi. Palauttaa (-1,-1) jos osuu otsikkoon tai tyhjään.</summary>
        private Point HitTest(int px, int py)
        {
            if (px < _headerWidth) return new Point(-1, -1);

            int col = (px - _headerWidth) / _cellW;
            int row = _firstVisibleRow + py / _cellH;

            if (row >= _rows.Count) return new Point(-1, -1);
            if (col >= _rows[row].Cols.Count) return new Point(-1, -1);

            return new Point(col, row);
        }

        /// <summary>Valitsee solut kahden (col,row)-pisteen väliltä lineaarisesti (kuten hex-editorissa).</summary>
        private void SelectRange(Point fromCell, Point toCell)
        {
            if (fromCell.X < 0 || toCell.X < 0) return;
            if (_rows == null) return;

            // Muunna lineaariseksi indeksiksi
            long LinearIndex(Point c)
                => (long)c.Y * _bytesPerRow + c.X;

            long idxA = LinearIndex(fromCell);
            long idxB = LinearIndex(toCell);
            long start = Math.Min(idxA, idxB);
            long end   = Math.Max(idxA, idxB);

            // Iteroi rivit/sarakkeet lineaarisesti
            for (long li = start; li <= end; li++)
            {
                int r = (int)(li / _bytesPerRow);
                int c = (int)(li % _bytesPerRow);
                if (r < _rows.Count && c < _rows[r].Addresses.Count)
                {
                    _selectedAddresses.Add(_rows[r].Addresses[c]);
                }
            }

            Invalidate();
        }

        // ─── ToolTip tuki ────────────────────────────────────────────────────────

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
        }

        /// <summary>Palauttaa tooltip-tekstin annetulle pikseli-koordinaatille.</summary>
        public string GetTooltipAt(int px, int py)
        {
            Point cell = HitTest(px, py);
            if (cell.X < 0 || _hexDatas == null) return "";
            var row = _rows[cell.Y];
            if (cell.X >= row.Addresses.Count) return "";
            uint addr   = row.Addresses[cell.X];
            int  bufIdx = (int)addr - _buffOffset;
            if (bufIdx < 0 || bufIdx >= _hexDatas.Length) return "";
            return _hexDatas[bufIdx].TableName ?? "";
        }

        // ─── Sisäinen apuominaisuus ───────────────────────────────────────────────
        private int ClientHeight => ClientSize.Height;
        private int ClientWidth  => ClientSize.Width - _vscroll.Width;
    }

    // ─── Tapahtuma-argumentit ─────────────────────────────────────────────────────

    public class HexSelectionChangedEventArgs : EventArgs
    {
        public ReadOnlyCollection<uint> SelectedAddresses { get; }
        public HexSelectionChangedEventArgs(IEnumerable<uint> addresses)
            => SelectedAddresses = new List<uint>(addresses).AsReadOnly();
    }

    public class HexScrollEventArgs : EventArgs
    {
        public int FirstVisibleRow { get; }
        public HexScrollEventArgs(int row) => FirstVisibleRow = row;
    }
}
