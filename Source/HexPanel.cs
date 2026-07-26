using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UniversalPatcher
{
    /// <summary>
    /// Lightweight virtual HEX editor panel for .NET 4.0.
    /// Only visible rows are rendered regardless of data size.
    ///
    /// Layout: [Offset] | [Hex bytes...] | [ASCII]
    ///
    /// Configurable:
    ///   <see cref="BytesPerRow"/>  — how many bytes per row (default 16)
    ///   <see cref="ShowHeaders"/>  — show/hide the column-header row
    ///   <see cref="ShowOffsets"/>  — show/hide the offset column
    ///   <see cref="ShowAscii"/>    — show/hide the ASCII column
    ///
    /// Selection feedback:
    ///   <see cref="SelectionChanged"/> event
    ///   <see cref="SelectedStart"/> / <see cref="SelectedLength"/> properties
    /// </summary>
    public class HexPanel : Control
    {
        // ── Layout constants ──────────────────────────────────────────────────────
        private const int OFFSET_CHARS         = 8;   // characters in "00000000"
        private const int COL_GAP              = 14;  // px gap between the three columns
        private const int LEFT_MARGIN          = 6;   // left margin when offsets are shown
        private const int LEFT_MARGIN_NOOFFSET = 16;  // wider left margin when offsets are hidden

        // ── Data ──────────────────────────────────────────────────────────────────
        private byte[] _data = new byte[0];

        // ── Highlight ranges ──────────────────────────────────────────────────────
        private readonly List<HighlightRange> _highlights = new List<HighlightRange>();

        // ── Scrollbar ─────────────────────────────────────────────────────────────
        private readonly VScrollBar _scrollBar;
        private int _firstVisibleRow;

        // ── Cached font metrics ───────────────────────────────────────────────────
        private Font _font;
        private int  _lineHeight;
        private int  _charWidth;

        // ── Column X positions (recalculated in RecalcMetrics) ────────────────────
        private int _offsetX;   // left edge of offset column (-1 when hidden)
        private int _hexX;      // left edge of hex-byte area
        private int _asciiX;    // left edge of ASCII area
        private int _totalRows;

        // ── Selection (inclusive byte indices) ────────────────────────────────────
        private int _selAnchor = -1;   // byte index where mouse was pressed
        private int _selFocus  = -1;   // byte index where mouse currently is

        // ── Bracket markers ───────────────────────────────────────────────────────────
        private int _bracketStart = -1;   // byte index where [ is drawn, -1 = hidden
        private int _bracketEnd = -1;   // byte index where ] is drawn, -1 = hidden

        // ── Modified byte tracking ────────────────────────────────────────────────────
        private readonly HashSet<int> _modifiedOffsets = new HashSet<int>();
        public Color ColorModified { get; set; } = Color.FromArgb(255, 160, 50); // amber

        // ── Keyboard hex input ────────────────────────────────────────────────────────
        private int _cursorOffset = -1;   // byte currently being edited, -1 = no cursor
        private bool _highNibble = true; // true = waiting for first hex char (high nibble)
        private char _pendingNibble = '\0'; // first hex char typed, waiting for second

        // ── Multi-selection (Ctrl+click individual bytes) ─────────────────────────────
        private readonly HashSet<int> _multiSelected = new HashSet<int>();

        // ── App-supplied external selection (replaces manual red highlights) ──────────
        private readonly HashSet<int> _appSelectedBytes = new HashSet<int>();


        // =========================================================================
        // Configuration properties
        // =========================================================================

        private int  _bytesPerRow = 16;
        private bool _showHeaders = false;
        private bool _showOffsets = false;
        private bool _showAscii   = true;

        /// <summary>
        /// When true, highlighted bytes get a faint background fill.
        /// When false, only the text color changes. Default: false.
        /// </summary>
        public bool HighlightBackground { get; set; } = false;
        /// <summary>
        /// Number of bytes displayed per row.
        /// Typical values: 4, 8, 16, 32. Default: 16.
        /// Triggers a full relayout and repaint when changed.
        /// </summary>
        public int BytesPerRow
        {
            get { return _bytesPerRow; }
            set
            {
                if (value < 1) value = 1;
                if (_bytesPerRow == value) return;
                _bytesPerRow = value;
                RecalcMetrics();
                Invalidate();
            }
        }

        /// <summary>
        /// Show the header row that labels each column index and the "Offset" caption.
        /// Default: false (pure data view).
        /// </summary>
        public bool ShowHeaders
        {
            get { return _showHeaders; }
            set
            {
                if (_showHeaders == value) return;
                _showHeaders = value;
                RecalcMetrics();
                Invalidate();
            }
        }

        /// <summary>
        /// Show the leftmost offset column ("00000000", "00000010", …).
        /// Default: false (pure data view).
        /// </summary>
        public bool ShowOffsets
        {
            get { return _showOffsets; }
            set
            {
                if (_showOffsets == value) return;
                _showOffsets = value;
                RecalcMetrics();
                Invalidate();
            }
        }

        /// <summary>
        /// Show the rightmost ASCII column.
        /// Default: true.
        /// </summary>
        public bool ShowAscii
        {
            get { return _showAscii; }
            set
            {
                if (_showAscii == value) return;
                _showAscii = value;
                RecalcMetrics();
                Invalidate();
            }
        }

        public Font TextFont
        {
            get { return _font; }
            set { _font = value; }
        }

        // =========================================================================
        // Colors — all public so the host application can theme the control
        // =========================================================================

        public Color ColorBackground { get; set; } = Color.FromArgb(28,  28,  28);
        public Color ColorOffset     { get; set; } = Color.FromArgb(90,  170,  90);
        public Color ColorHex        { get; set; } = Color.FromArgb(220, 220, 220);
        public Color ColorAscii      { get; set; } = Color.FromArgb(170, 195, 255);
        public Color ColorSeparator  { get; set; } = Color.FromArgb(60,  60,  60);
        public Color ColorHeader     { get; set; } = Color.FromArgb(110, 110, 110);
        public Color ColorSelection  { get; set; } = Color.FromArgb(0,   110, 210);
        public Color ColorSelText    { get; set; } = Color.White;

        public Color ColorCursor { get; set; } = Color.FromArgb(220, 60, 60); // red outline

        /// <summary>Fill color for bytes supplied via <see cref="SetExternalSelection"/>.</summary>
        //public Color ColorExternalSelection  { get; set; } = Color.FromArgb(200, 60, 60);
        /// <summary>Text color for bytes supplied via <see cref="SetExternalSelection"/>.</summary>
        public Color ColorExternalSelText    { get; set; } = Color.Red;

        // =========================================================================
        // Selection event + read-only properties
        // =========================================================================

        /// <summary>
        /// Raised whenever the byte selection changes (including programmatic changes).
        /// <see cref="HexSelectionEventArgs.Start"/>  — first selected byte (-1 = empty).
        /// <see cref="HexSelectionEventArgs.Length"/> — byte count (0 = empty).
        /// </summary>
        public event EventHandler<HexSelectionEventArgs> SelectionChanged;

        /// <summary>First selected byte index, or -1 when nothing is selected.</summary>
        public int SelectedStart  => _selAnchor < 0 ? -1 : Math.Min(_selAnchor, _selFocus);

        /// <summary>Number of selected bytes (0 when nothing is selected).</summary>
        public int SelectedLength => _selAnchor < 0 ?  0 : Math.Abs(_selFocus - _selAnchor) + 1;

        // =========================================================================
        // Constructor
        // =========================================================================

        public HexPanel()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint  |
                ControlStyles.UserPaint,
                true);

            _font = new Font("Consolas", 9f, FontStyle.Regular, GraphicsUnit.Point);

            _scrollBar = new VScrollBar
            {
                Dock        = DockStyle.Right,
                SmallChange = 1,
                LargeChange = 1
            };
            _scrollBar.Scroll += (s, e) => { _firstVisibleRow = e.NewValue; Invalidate(); };
            Controls.Add(_scrollBar);

            BackColor = ColorBackground;
        }

        // =========================================================================
        // Public API
        // =========================================================================

        /// <summary>
        /// Replace the displayed data. Clears all highlights and the selection.
        /// </summary>
        public void SetData(byte[] data)
        {
            //_data = data ?? new byte[0];
            _data = (byte[])data.Clone();
            _highlights.Clear();
            _modifiedOffsets.Clear();
            _appSelectedBytes.Clear();
            ClearSelectionCore();
            _firstVisibleRow = 0;
            RecalcMetrics();
            Invalidate();
        }
        /// <summary>
        /// Update a single byte in the displayed data without reloading the entire array.
        /// Only the affected row is repainted.
        /// </summary>
        /// <param name="offset">Byte index to update.</param>
        /// <param name="value">New byte value.</param>
        public void SetByte(int offset, byte value)
        {
            if (offset < 0 || offset >= _data.Length) return;
            _data[offset] = value;
            _modifiedOffsets.Add(offset);
            InvalidateRow(offset / _bytesPerRow);
        }

        /// <summary>
        /// Update a contiguous block of bytes without reloading the entire array.
        /// Only the affected rows are repainted.
        /// </summary>
        /// <param name="offset">First byte index to update.</param>
        /// <param name="values">New byte values.</param>
        public void SetBytes(int offset, byte[] values)
        {
            if (values == null || offset < 0) return;
            int count = Math.Min(values.Length, _data.Length - offset);
            if (count <= 0) return;

            Buffer.BlockCopy(values, 0, _data, offset, count);

            // Repaint only the rows that actually changed
            int firstRow = offset / _bytesPerRow;
            int lastRow = (offset + count - 1) / _bytesPerRow;
            for (int row = firstRow; row <= lastRow; row++)
                InvalidateRow(row);
        }
        /// <summary>
        /// Clears the modified-byte tracking without changing the data.
        /// Call this after saving so the amber color disappears.
        /// </summary>
        public void ClearModified()
        {
            _modifiedOffsets.Clear();
            Invalidate();
        }

        /// <summary>Returns true if the byte at offset has been modified since last load/clear.</summary>
        public bool IsModified(int offset) => _modifiedOffsets.Contains(offset);

        /// <summary>Returns all modified byte offsets in ascending order.</summary>
        public int[] GetModifiedOffsets()
        {
            var list = new List<int>(_modifiedOffsets);
            list.Sort();
            return list.ToArray();
        }
        /// <summary>
        /// Read a single byte from the displayed data.
        /// Returns -1 when the offset is out of range.
        /// </summary>
        public int GetByte(int offset)
        {
            if (offset < 0 || offset >= _data.Length) return -1;
            return _data[offset];
        }

        /// <summary>
        /// Invalidate (repaint) only one row in the visible area.
        /// Has no effect if the row is currently scrolled out of view.
        /// </summary>
        private void InvalidateRow(int row)
        {
            int visibleRow = row - _firstVisibleRow;
            if (visibleRow < 0 || visibleRow >= VisibleRows) return;

            int y = DataAreaTop + visibleRow * _lineHeight;
            Invalidate(new Rectangle(0, y, _scrollBar.Left, _lineHeight));
        }

        public Color ColorBracket { get; set; } = Color.FromArgb(255, 200, 50); // bright amber

        /// <summary>
        /// Byte offset where [ is drawn. Set to -1 to hide.
        /// </summary>
        public int BracketStart
        {
            get { return _bracketStart; }
            set { _bracketStart = value; Invalidate(); }
        }

        /// <summary>
        /// Byte offset where ] is drawn. Set to -1 to hide.
        /// </summary>
        public int BracketEnd
        {
            get { return _bracketEnd; }
            set { _bracketEnd = value; Invalidate(); }
        }

        /// <summary>Set both bracket markers at once and scroll [ into view.</summary>
        public void SetBrackets(int start, int end)
        {
            _bracketStart = start;
            _bracketEnd = end;
            if (start >= 0) ScrollToOffset(start);
            else Invalidate();
        }

        /// <summary>Hide both bracket markers.</summary>
        public void ClearBrackets()
        {
            _bracketStart = -1;
            _bracketEnd = -1;
            Invalidate();
        }
        /// <summary>
        /// <summary>
        /// Returns current data.
        /// </summary>
        public byte[] GetData()
        {
            return _data;
        }

        /// <summary>
        /// <summary>
        /// Returns a copy of the bytes between the bracket markers (inclusive).
        /// Returns null when either bracket is not set or the range is invalid.
        /// </summary>
        public byte[] GetBracketedData()
        {
            if (_bracketStart < 0 || _bracketEnd < 0) return null;

            int start = Math.Min(_bracketStart, _bracketEnd);
            int end = Math.Max(_bracketStart, _bracketEnd);
            int length = end - start + 1;

            if (start >= _data.Length) return null;
            length = Math.Min(length, _data.Length - start);

            byte[] result = new byte[length];
            Buffer.BlockCopy(_data, start, result, 0, length);
            return result;
        }
        /// <summary>
        /// Add a colored highlight over a contiguous byte range.
        /// </summary>
        /// <param name="start">First byte index of the range.</param>
        /// <param name="length">Number of bytes to cover.</param>
        /// <param name="color">Highlight color used for text tint or inverse fill.</param>
        /// <param name="label">Optional identifier stored with the range (not rendered).</param>
        /// <param name="priority">Higher values win when highlights overlap.</param>
        /// <param name="inverseColors">
        /// When <c>true</c> the cell is filled with <paramref name="color"/> and the
        /// text is drawn in the contrasting (inverted) color, making the highlight
        /// stand out like a solid tag.  When <c>false</c> (default) the existing
        /// text-tint / faint-background behaviour is used.
        /// </param>
        public void AddHighlight(int start, int length, Color color, string label = null, int priority = 0, bool inverseColors = false)
        {
            if (start < 0 || length <= 0) return;
            _highlights.Add(new HighlightRange(start, length, color, label, priority, inverseColors));
            Invalidate();
        }
        /// <summary>
        /// Returns the minimum size needed to display only the bracketed byte range
        /// without scrolling. Takes current column visibility and font into account.
        /// Returns Size.Empty when brackets are not set.
        /// </summary>
        public Size RequiredSizeForBrackets()
        {
            if (_bracketStart < 0 || _bracketEnd < 0) return Size.Empty;
            if (_charWidth == 0) RecalcMetrics();

            int start = Math.Min(_bracketStart, _bracketEnd);
            int end = Math.Max(_bracketStart, _bracketEnd);
            int byteCount = end - start + 1;

            // Number of rows the bracketed range spans
            int firstRow = start / _bytesPerRow;
            int lastRow = end / _bytesPerRow;
            int rowCount = lastRow - firstRow + 1;

            // ── Width — same as RequiredSize ─────────────────────────────────────────
            int width = _showOffsets ? LEFT_MARGIN : LEFT_MARGIN_NOOFFSET;

            if (_showOffsets)
                width += (OFFSET_CHARS + 1) * _charWidth + COL_GAP;

            width += _bytesPerRow * 3 * _charWidth + COL_GAP;

            if (_showAscii)
                width += _bytesPerRow * _charWidth + COL_GAP;

            width += _scrollBar.Width;

            // ── Height — only the rows that contain bracketed bytes ──────────────────
            int height = rowCount * _lineHeight;

            if (_showHeaders)
                height += _lineHeight;

            return new Size(width, height);
        }
        /// <summary>
        /// Highlights an arbitrary set of byte offsets using the external-selection
        /// colors (<see cref="ColorExternalSelection"/> / <see cref="ColorExternalSelText"/>).
        /// This replaces the previous approach of adding and removing red highlights
        /// from the host application — the control manages the state internally.
        /// Passing <c>null</c> or an empty array clears the external selection.
        /// </summary>
        public void SetExternalSelection(int[] selectedBytes)
        {
            _appSelectedBytes.Clear();
            if (selectedBytes != null)
                foreach (int b in selectedBytes)
                    _appSelectedBytes.Add(b);
            Invalidate();
        }

        /// <summary>Clears the external selection without affecting highlights or drag-selection.</summary>
        public void ClearExternalSelection()
        {
            _appSelectedBytes.Clear();
            Invalidate();
        }

        /// <summary>Remove all highlights without changing the selection.</summary>
        public void ClearHighlights()
        {
            _highlights.Clear();
            Invalidate();
        }

        /// <summary>Remove all highlights that have <paramref name="color"/> without changing the selection.</summary>
        public void ClearSingleColorHighlights(Color color, int priority)
        {
            int count = _highlights.RemoveAll(X => X.Color == color && X.Priority == priority);
            if (count > 1)
            {
                Invalidate();
            }
        }

        /// <summary>
        /// Replaces all highlights that have <paramref name="oldColor"/> with
        /// <paramref name="newColor"/>. Other properties (start, length, label)
        /// are preserved.
        /// </summary>
        public void ReplaceHighlightColor(Color oldColor, Color newColor)
        {
            bool changed = false;
            for (int i = 0; i < _highlights.Count; i++)
            {
                if (_highlights[i].Color.ToArgb() == oldColor.ToArgb())
                {
                    _highlights[i] = new HighlightRange(
                        _highlights[i].Start,
                        _highlights[i].Length,
                        newColor,
                        _highlights[i].Label,
                        _highlights[i].Priority,
                        _highlights[i].InverseColors);  
                    changed = true;
                }
            }
            if (changed) Invalidate();
        }

        /// <summary>
        /// Returns all individually Ctrl+clicked byte offsets in ascending order.
        /// Empty when no multi-selection exists.
        /// </summary>
        public int[] MultiSelectedOffsets
        {
            get
            {
                var list = new List<int>(_multiSelected);
                list.Sort();
                return list.ToArray();
            }
        }

        /// <summary>
        /// Returns all selected byte offsets in ascending order,
        /// combining both drag-selection and Ctrl+click multi-selection.
        /// </summary>
        public int[] GetSelectedOffsets()
        {
            var result = new HashSet<int>(_multiSelected);

            // Add drag-selection range
            if (_selAnchor >= 0)
            {
                int lo = Math.Min(_selAnchor, _selFocus);
                int hi = Math.Max(_selAnchor, _selFocus);
                for (int i = lo; i <= hi; i++)
                    result.Add(i);
            }

            var list = new List<int>(result);
            list.Sort();
            return list.ToArray();
        }
        /// <summary>
        /// Returns true when the byte at offset is part of either
        /// the drag selection or the Ctrl+click multi-selection.
        /// </summary>
        public bool IsByteSelected(int offset)
        {
            if (_multiSelected.Contains(offset)) return true;
            if (_selAnchor < 0) return false;
            int lo = Math.Min(_selAnchor, _selFocus);
            int hi = Math.Max(_selAnchor, _selFocus);
            return offset >= lo && offset <= hi;
        }
        /// <summary>
        /// Programmatically select a byte range and scroll it into view.
        /// Fires <see cref="SelectionChanged"/>.
        /// </summary>
        /// <param name="start">First byte index.</param>
        /// <param name="length">Number of bytes (minimum 1).</param>
        public void SetSelection(int start, int length)
        {
            if (start < 0 || length < 1 || start >= _data.Length) return;
            length     = Math.Min(length, _data.Length - start);
            _selAnchor = start;
            _selFocus  = start + length - 1;
            ScrollToOffset(start);
            FireSelectionChanged();
            Invalidate();
        }

        /// <summary>Clear the selection programmatically.</summary>
        public void ClearSelection()
        {
            ClearSelectionCore();
            FireSelectionChanged();
            Invalidate();
        }

        /// <summary>Scroll the view so that <paramref name="byteOffset"/> is visible.</summary>
        public void ScrollToOffset(int byteOffset)
        {
            int row     = byteOffset / _bytesPerRow;
            row         = Math.Max(0, Math.Min(row, _totalRows - 1));
            int visible = VisibleRows;

            if (row < _firstVisibleRow)
                _firstVisibleRow = row;
            else if (row >= _firstVisibleRow + visible)
                _firstVisibleRow = row - visible + 1;

            _scrollBar.Value = Math.Min(_firstVisibleRow, _scrollBar.Maximum);
            Invalidate();
        }

        /// <summary>
        /// Scrolls the view so that the bracketed data is centered vertically.
        /// Falls back to showing bracket start at the top when the bracket range
        /// is taller than the visible area.
        /// </summary>
        public void ScrollToBrackets()
        {
            if (_bracketStart < 0 || _bracketEnd < 0) return;

            int start = Math.Min(_bracketStart, _bracketEnd);
            int end = Math.Max(_bracketStart, _bracketEnd);

            int firstRow = start / _bytesPerRow;
            int lastRow = end / _bytesPerRow;
            int rangeRows = lastRow - firstRow + 1;

            int visible = VisibleRows;

            if (rangeRows >= visible)
            {
                // Bracket range fills or exceeds the visible area — show from top
                // with one empty row as margin if possible
                _firstVisibleRow = Math.Max(0, firstRow - 1);
            }
            else
            {
                // Center the bracket range vertically
                int padding = (visible - rangeRows) / 2;
                _firstVisibleRow = Math.Max(0, firstRow - padding);
            }

            // Clamp to valid range
            _firstVisibleRow = Math.Min(_firstVisibleRow,
                                        Math.Max(0, _totalRows - visible));

            _scrollBar.Value = Math.Min(_firstVisibleRow, _scrollBar.Maximum);
            Invalidate();
        }

        // =========================================================================
        // Layout metrics
        // =========================================================================

        private void RecalcMetrics()
        {
            if (!IsHandleCreated) return;

            using (var g = CreateGraphics())
            {
                var sz     = g.MeasureString("W", _font, PointF.Empty,
                                             StringFormat.GenericTypographic);
                _charWidth  = (int)Math.Ceiling(sz.Width);
                _lineHeight = (int)Math.Ceiling(sz.Height) + 3;
            }

            int x = _showOffsets ? LEFT_MARGIN : LEFT_MARGIN_NOOFFSET; // left margin

            // Offset column (optional)
            if (_showOffsets)
            {
                _offsetX = x;
                x       += (OFFSET_CHARS + 1) * _charWidth + COL_GAP;
            }
            else
            {
                _offsetX = -1; // hidden
            }

            _hexX = x;

            // Hex column width: BytesPerRow * "XX " plus one extra gap after the mid-group
            int hexWidth = _bytesPerRow * 3 * _charWidth + COL_GAP;
            _asciiX = _hexX + hexWidth;

            _totalRows = (_data.Length + _bytesPerRow - 1) / _bytesPerRow;
            if (_totalRows == 0) _totalRows = 1;

            int visible            = VisibleRows;
            _scrollBar.Maximum     = Math.Max(0, _totalRows - 1);
            _scrollBar.LargeChange = Math.Max(1, visible);
            _firstVisibleRow       = Math.Min(_firstVisibleRow,
                                              Math.Max(0, _totalRows - visible));
        }

        /// <summary>Number of data rows that fit in the current client height.</summary>
        private int VisibleRows
        {
            get
            {
                int reserved = _showHeaders ? _lineHeight : 0;
                return Math.Max(1, (Height - reserved) / _lineHeight);
            }
        }
        /// <summary>
        /// Returns the minimum client width in pixels needed to display all columns
        /// without clipping, based on current settings (BytesPerRow, ShowOffsets,
        /// ShowAscii, font size).
        /// Call after SetData() and any layout changes.
        /// </summary>
        public int RequiredWidth
        {
            get
            {
                if (_charWidth == 0) RecalcMetrics();

                int width = _showOffsets ? LEFT_MARGIN : LEFT_MARGIN_NOOFFSET; // left margin

                // Offset column
                if (_showOffsets)
                    width += (OFFSET_CHARS + 1) * _charWidth + COL_GAP;

                // Hex column: BytesPerRow * "XX " (no mid-gap anymore)
                width += _bytesPerRow * 3 * _charWidth + COL_GAP;

                // ASCII column
                if (_showAscii)
                    width += _bytesPerRow * _charWidth + COL_GAP;

                // Scrollbar
                width += _scrollBar.Width;

                return width;
            }
        }

        /// <summary>
        /// Returns the minimum size needed to display all data without scrolling,
        /// based on current settings (BytesPerRow, ShowOffsets, ShowAscii,
        /// ShowHeaders, font size, data length).
        /// </summary>
        public Size RequiredSize
        {
            get
            {
                if (_charWidth == 0) RecalcMetrics();

                // ── Width ─────────────────────────────────────────────────────────────
                int width = _showOffsets ? LEFT_MARGIN : LEFT_MARGIN_NOOFFSET; // left margin

                if (_showOffsets)
                    width += (OFFSET_CHARS + 1) * _charWidth + COL_GAP;

                width += _bytesPerRow * 3 * _charWidth + COL_GAP;

                if (_showAscii)
                    width += _bytesPerRow * _charWidth + COL_GAP;

                width += _scrollBar.Width;

                // ── Height ────────────────────────────────────────────────────────────
                int height = _totalRows * _lineHeight;

                if (_showHeaders)
                    height += _lineHeight;

                return new Size(width, height);
            }
        }
        /// <summary>Y coordinate of the first data row in client pixels.</summary>
        private int DataAreaTop => _showHeaders ? _lineHeight : 0;

        // =========================================================================
        // Painting
        // =========================================================================

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(ColorBackground);

            if (_charWidth == 0) RecalcMetrics();

            if (_showHeaders)
            {
                DrawHeader(g);
                DrawSeparators(g);
            }

            int lastRow = Math.Min(_firstVisibleRow + VisibleRows, _totalRows);
            for (int row = _firstVisibleRow; row < lastRow; row++)
            {
                int rowOffset  = row * _bytesPerRow;
                int bytesInRow = Math.Min(_bytesPerRow, _data.Length - rowOffset);
                int y          = DataAreaTop + (row - _firstVisibleRow) * _lineHeight;
                DrawRow(g, rowOffset, bytesInRow, y);
            }
        }

        private void DrawHeader(Graphics g)
        {
            using (var brush = new SolidBrush(ColorHeader))
            {
                if (_showOffsets)
                    DrawText(g, brush, "Offset  ", _offsetX, 1);

                for (int i = 0; i < _bytesPerRow; i++)
                    DrawText(g, brush, i.ToString("X2"), HexByteX(i), 1);

                if (_showAscii)
                {
                    var sb = new System.Text.StringBuilder(_bytesPerRow);
                    for (int i = 0; i < _bytesPerRow; i++)
                        sb.Append((i % 16).ToString("X1"));
                    DrawText(g, brush, sb.ToString(), _asciiX, 1);
                }
            }
        }

        private void DrawSeparators(Graphics g)
        {
            using (var pen = new Pen(ColorSeparator))
            {
                if (_showOffsets)
                    g.DrawLine(pen, _hexX - COL_GAP / 2, 0, _hexX - COL_GAP / 2, Height);

                if (_showAscii)
                    g.DrawLine(pen, _asciiX - COL_GAP / 2, 0, _asciiX - COL_GAP / 2, Height);

                g.DrawLine(pen, 0, _lineHeight, _scrollBar.Left, _lineHeight);
            }
        }

        private void DrawRow(Graphics g, int rowOffset, int bytesInRow, int y)
        {
            // Offset label
            if (_showOffsets)
            {
                using (var brush = new SolidBrush(ColorOffset))
                    DrawText(g, brush, rowOffset.ToString("X8"), _offsetX, y);
            }

            if (bytesInRow <= 0) return;

            int selLo = _selAnchor < 0 ? -1 : Math.Min(_selAnchor, _selFocus);
            int selHi = _selAnchor < 0 ? -1 : Math.Max(_selAnchor, _selFocus);

            for (int i = 0; i < bytesInRow; i++)
            {
                int byteIdx = rowOffset + i;
                byte b = _data[byteIdx];
                bool selected     = IsByteSelected(byteIdx);
                bool extSelected  = _appSelectedBytes.Contains(byteIdx);
                bool isCursor     = byteIdx == _cursorOffset;
                bool pendingNibble = isCursor && !_highNibble; // waiting for 2nd hex char

                Color hlColor;
                int hlPriority;
                bool hlInverse;
                bool hasHl = ResolveHighlight(byteIdx, out hlColor, out hlPriority, out hlInverse);

                // Modified color overrides highlight unless byte is selected
                bool isModified = _modifiedOffsets.Contains(byteIdx);
                if (isModified && !selected && !extSelected)
                {
                    if (!hasHl || hlPriority < 1)
                    {
                        hlColor    = ColorModified;
                        hlPriority = 0;
                        hlInverse  = false;
                        hasHl      = true;
                    }
                }

                int hexColX   = HexByteX(i);
                int asciiColX = _asciiX + i * _charWidth;

                // ── Backgrounds (priority: drag-selection > external selection > inverse hl > faint hl) ──
                if (selected)
                {
                    using (var sb = new SolidBrush(ColorSelection))
                    {
                        g.FillRectangle(sb, hexColX, y, _charWidth * 2, _lineHeight);
                        if (_showAscii)
                            g.FillRectangle(sb, asciiColX, y, _charWidth, _lineHeight);
                    }
                }
                //else if (extSelected)
                else if (extSelected && hasHl)
                {
                    // If the byte also carries a highlight, use that color as the fill
                    // so the highlight identity is preserved while still showing selection.
                    // Otherwise fall back to the generic external-selection color.
                    //Color extFill = hasHl ? hlColor : ColorExternalSelection;
                    Color extFill = hasHl ? hlColor : ColorHex; // ColorExternalSelection;
                    using (var sb = new SolidBrush(extFill))
                    {
                        g.FillRectangle(sb, hexColX, y, _charWidth * 2, _lineHeight);
                        if (_showAscii)
                            g.FillRectangle(sb, asciiColX, y, _charWidth, _lineHeight);
                    }
                }
                else if (hasHl && hlInverse)
                {
                    using (var sb = new SolidBrush(hlColor))
                    {
                        g.FillRectangle(sb, hexColX, y, _charWidth * 2, _lineHeight);
                        if (_showAscii)
                            g.FillRectangle(sb, asciiColX, y, _charWidth, _lineHeight);
                    }
                }
                else if (hasHl && HighlightBackground)
                {
                    using (var sb = new SolidBrush(Color.FromArgb(55, hlColor)))
                        g.FillRectangle(sb, hexColX, y, _charWidth * 2, _lineHeight);
                }

                // ── Pending-nibble indicator: dim background over low nibble cell ────
                if (pendingNibble)
                {
                    // Dim overlay on the second character position to show where the
                    // next keystroke will land.
                    using (var sb = new SolidBrush(Color.FromArgb(80, ColorCursor)))
                        g.FillRectangle(sb, hexColX + _charWidth, y, _charWidth, _lineHeight);
                }

                // ── Compute inverse-highlight text color ─────────────────────────────
                Color hlTextColor = Color.Empty;
                if (hasHl && hlInverse)
                {
                    double lum = (0.299 * hlColor.R + 0.587 * hlColor.G + 0.114 * hlColor.B) / 255.0;
                    hlTextColor = lum > 0.5 ? Color.Black : Color.White;
                }

                // ── Hex text ─────────────────────────────────────────────────────────
                Color hexColor = selected    ? ColorSelText
                               : extSelected ? (hasHl ? ColorBackground : ColorExternalSelText)
                               : (hasHl && hlInverse) ? hlTextColor
                               : (hasHl ? hlColor : ColorHex);
                if (selected && extSelected) hexColor = ColorExternalSelText;


                if (pendingNibble)
                {
                    // Draw the already-typed high nibble in bright cursor color,
                    // then an underscore hint for the missing low nibble.
                    using (var brush = new SolidBrush(ColorCursor))
                    {
                        DrawText(g, brush, _pendingNibble.ToString(), hexColX, y);
                        DrawText(g, brush, "_", hexColX + _charWidth, y);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(hexColor))
                        DrawText(g, brush, b.ToString("X2"), hexColX, y);
                }

                // ── ASCII text ───────────────────────────────────────────────────────
                if (_showAscii)
                {
                    Color ac = selected    ? ColorSelText
                             : extSelected ? (hasHl ? ColorBackground : ColorExternalSelText)
                             : (hasHl && hlInverse) ? hlTextColor
                             : (hasHl ? hlColor : ColorAscii);
                    char ch = (b >= 32 && b < 127) ? (char)b : '.';
                    using (var brush = new SolidBrush(ac))
                        DrawText(g, brush, ch.ToString(), asciiColX, y);
                }

                // ── Cursor outline ───────────────────────────────────────────────────
                if (isCursor)
                {
                    using (var pen = new Pen(ColorCursor, 1))
                        g.DrawRectangle(pen, hexColX, y, _charWidth * 2 - 1, _lineHeight - 1);
                }

                // ── Bracket markers ───────────────────────────────────────────────────
                DrawBracketIfNeeded(g, byteIdx, hexColX, asciiColX, y);
            }
        }


        /// <summary>
        /// Draws [ or ] just to the left/right of the hex cell (and ASCII cell)
        /// when byteIdx matches a bracket marker.
        /// </summary>
        private void DrawBracketIfNeeded(Graphics g, int byteIdx,
                                          int hexColX, int asciiColX, int y)
        {
            bool isStart = byteIdx == _bracketStart;
            bool isEnd = byteIdx == _bracketEnd;
            if (!isStart && !isEnd) return;

            using (var brush = new SolidBrush(ColorBracket))
            using (var font = new Font(_font.FontFamily, _font.SizeInPoints + 1f,
                                        FontStyle.Bold, GraphicsUnit.Point))
            {
                if (isStart)
                {
                    // [ sits one pixel to the left of the hex cell
                    DrawText(g, brush, "[", hexColX - _charWidth, y);
                    if (_showAscii)
                        DrawText(g, brush, "[", asciiColX - _charWidth, y);
                }

                if (isEnd)
                {
                    // ] sits right after the two hex chars ("XX")
                    DrawText(g, brush, "]", hexColX + _charWidth * 2, y);
                    if (_showAscii)
                        DrawText(g, brush, "]", asciiColX + _charWidth, y);
                }
            }
        }
        // =========================================================================
        // Helpers
        // =========================================================================

        /// <summary>
        /// X pixel position of hex byte column <paramref name="col"/> (0-based).
        /// Inserts one extra char-width gap after the mid-group (BytesPerRow / 2)
        /// to visually separate the two halves.
        /// </summary>
        private int HexByteX(int col)
        {
            return _hexX + col * 3 * _charWidth;
        }

        private void DrawText(Graphics g, Brush brush, string text, int x, int y)
            => g.DrawString(text, _font, brush, x, y, StringFormat.GenericTypographic);

        /// <summary>
        /// Returns the color and flags of the highest-priority highlight covering
        /// <paramref name="byteIdx"/>.  Returns false (and Color.Empty) when no
        /// highlight applies.
        /// </summary>
        private bool ResolveHighlight(int byteIdx, out Color color, out int priority, out bool inverseColors)
        {
            color = Color.Empty;
            priority = int.MinValue;
            inverseColors = false;
            bool found = false;

            foreach (var hl in _highlights)
            {
                if (byteIdx >= hl.Start && byteIdx < hl.Start + hl.Length)
                {
                    if (!found || hl.Priority > priority)
                    {
                        color         = hl.Color;
                        priority      = hl.Priority;
                        inverseColors = hl.InverseColors;
                        found         = true;
                    }
                }
            }
            return found;
        }
        // =========================================================================
        // Mouse — click + drag selection
        // =========================================================================

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left) return;
            Focus();
            int idx = HitTest(e.X, e.Y);
            if (idx < 0) return;

            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                // Ctrl+click: toggle this byte in multi-selection
                // Does not move the drag-selection anchor
                if (_multiSelected.Contains(idx))
                    _multiSelected.Remove(idx);
                else
                    _multiSelected.Add(idx);

                _cursorOffset = idx;
                _highNibble = true;
                _pendingNibble = '\0';
                FireSelectionChanged();
                Invalidate();
            }
            else
            {
                // Normal click: clear multi-selection, start drag
                _multiSelected.Clear();
                _selAnchor = idx;
                _selFocus = idx;
                _cursorOffset = idx;
                _highNibble = true;
                _pendingNibble = '\0';
                FireSelectionChanged();
                Invalidate();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.Left || _selAnchor < 0) return;
            int idx = HitTest(e.X, e.Y);
            if (idx >= 0 && idx != _selFocus)
            {
                _selFocus = idx;
                FireSelectionChanged();
                Invalidate();
            }
        }

        /// <summary>
        /// Maps a mouse position to a byte index.
        /// Returns -1 when outside the data area.
        /// </summary>
        private int HitTest(int mouseX, int mouseY)
        {
            int top = DataAreaTop;
            if (mouseY < top) return -1;

            int row = (mouseY - top) / _lineHeight + _firstVisibleRow;
            if (row < 0 || row >= _totalRows) return -1;

            int col = -1;

            // Hex area
            int hexAreaRight = _showAscii ? (_asciiX - COL_GAP / 2) : _scrollBar.Left;
            if (mouseX >= _hexX && mouseX < hexAreaRight)
            {
                int relX = mouseX - _hexX;
                int byteW = _charWidth * 3;
                col = relX / byteW;
            }
            // ASCII area
            else if (_showAscii && mouseX >= _asciiX)
            {
                col = (mouseX - _asciiX) / _charWidth;
                col = Math.Max(0, Math.Min(col, _bytesPerRow - 1));
            }

            if (col < 0) return -1;
            int idx = row * _bytesPerRow + col;
            return idx < _data.Length ? idx : -1;
        }

        // =========================================================================
        // Keyboard scrolling
        // =========================================================================

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up: case Keys.Down:
                case Keys.PageUp: case Keys.PageDown:
                case Keys.Home:   case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                // Scrolling (existing)
                int v = VisibleRows;
                switch (e.KeyCode)
                {
                    case Keys.Up: ScrollRows(-1); break;
                    case Keys.Down: ScrollRows(+1); break;
                    case Keys.PageUp: ScrollRows(-v); break;
                    case Keys.PageDown: ScrollRows(+v); break;
                    case Keys.Home: ScrollRows(-_totalRows); break;
                    case Keys.End: ScrollRows(+_totalRows); break;

                    // Cursor movement
                    case Keys.Left:
                        MoveCursor(-1);
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        MoveCursor(+1);
                        e.Handled = true;
                        break;
                    case Keys.Escape:
                        // Cancel pending nibble
                        _highNibble = true;
                        _pendingNibble = '\0';
                        Invalidate();
                        e.Handled = true;
                        break;
                }
                if (e.Control && e.KeyCode == Keys.C)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int byteIdx = 0; byteIdx < _data.Length; byteIdx++)
                    {
                        byte b = _data[byteIdx];
                        if (IsByteSelected(byteIdx))
                        {
                            sb.Append(_data[byteIdx].ToString("X2") + " ");
                        }
                    }
                    Clipboard.SetText(sb.ToString().Trim(' '));
                    e.Handled = true;
                }
                if (e.Control && e.KeyCode == Keys.V)
                {
                    if (_cursorOffset < 0 || _cursorOffset >= _data.Length) return;
                    if (!Clipboard.ContainsText())
                    {
                        return;
                    }
                    byte[] bytes = Clipboard.GetText().Replace(" ", "").ToBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        int byteIdx = i + _cursorOffset;
                        if (byteIdx >= _data.Length) return;
                        _data[byteIdx] = bytes[i];
                    }
                    Invalidate();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, HexPanel, line " + line + ": " + ex.Message);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (_cursorOffset < 0 || _cursorOffset >= _data.Length) return;

            char c = char.ToUpper(e.KeyChar);
            if (!IsHexChar(c)) return;

            e.Handled = true;

            if (_highNibble)
            {
                // First hex char typed — store it, wait for second
                _pendingNibble = c;
                _highNibble = false;
                Invalidate();   // show pending nibble on cursor cell
            }
            else
            {
                // Second hex char — compose the full byte and write it
                byte newValue = (byte)((HexCharToInt(_pendingNibble) << 4)
                                       | HexCharToInt(c));
                SetByte(_cursorOffset, newValue);   // marks as modified, repaints row

                _highNibble = true;
                _pendingNibble = '\0';

                // Advance cursor to next byte
                MoveCursor(+1);
            }
        }

        private void MoveCursor(int delta)
        {
            if (_cursorOffset < 0) return;
            int next = _cursorOffset + delta;
            if (next < 0 || next >= _data.Length) return;

            _cursorOffset = next;
            _highNibble = true;
            _pendingNibble = '\0';

            // Keep cursor visible
            ScrollToOffset(_cursorOffset);

            // Sync selection to cursor
            _selAnchor = _cursorOffset;
            _selFocus = _cursorOffset;
            FireSelectionChanged();
            Invalidate();
        }

        private static bool IsHexChar(char c)
            => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F');

        private static int HexCharToInt(char c)
            => c >= 'A' ? c - 'A' + 10 : c - '0';
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            ScrollRows(e.Delta > 0 ? -3 : 3);
        }

        private void ScrollRows(int delta)
        {
            _firstVisibleRow = Math.Max(0,
                Math.Min(_firstVisibleRow + delta,
                         Math.Max(0, _totalRows - VisibleRows)));
            _scrollBar.Value = Math.Min(_firstVisibleRow, _scrollBar.Maximum);
            Invalidate();
        }

        // =========================================================================
        // Resize / handle lifecycle
        // =========================================================================

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalcMetrics();
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RecalcMetrics();
        }

        // =========================================================================
        // Internal helpers
        // =========================================================================

        private void ClearSelectionCore()
        {
            _selAnchor = -1;
            _selFocus = -1;
            _cursorOffset = -1;
            _highNibble = true;
            _pendingNibble = '\0';
            _multiSelected.Clear();
            // Note: _appSelectedBytes is NOT cleared here — external selection
            // is managed independently via SetExternalSelection / ClearExternalSelection.
        }

        private void FireSelectionChanged()
        {
            var h = SelectionChanged;
            if (h != null)
                h(this, new HexSelectionEventArgs(SelectedStart, SelectedLength));
        }

        // =========================================================================
        // Nested: highlight storage
        // =========================================================================

        private sealed class HighlightRange
        {
            public int    Start         { get; }
            public int    Length        { get; }
            public Color  Color         { get; }
            public string Label         { get; }
            public int    Priority      { get; }
            public bool   InverseColors { get; }
            public HighlightRange(int start, int length, Color color, string label, int priority = 0, bool inverseColors = false)
            {
                Start         = start;
                Length        = length;
                Color         = color;
                Label         = label;
                Priority      = priority;
                InverseColors = inverseColors;
            }
        }
    }

    // =========================================================================
    // Event args — returned to the host on every selection change
    // =========================================================================

    /// <summary>
    /// Carries the current byte selection back to the host application.
    /// </summary>
    public sealed class HexSelectionEventArgs : EventArgs
    {
        /// <summary>
        /// Zero-based index of the first selected byte.
        /// -1 when the selection is empty.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Number of selected bytes.
        /// 0 when the selection is empty.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// One past the last selected byte — handy for slice / span operations.
        /// -1 when the selection is empty.
        /// </summary>
        public int End => Start < 0 ? -1 : Start + Length;

        public HexSelectionEventArgs(int start, int length)
        {
            Start  = start;
            Length = length;
        }

        public override string ToString()
            => Start < 0
               ? "No selection"
               : string.Format("0x{0:X8}  len={1}  (0x{0:X8}–0x{2:X8})",
                                Start, Length, End - 1);
    }
}
