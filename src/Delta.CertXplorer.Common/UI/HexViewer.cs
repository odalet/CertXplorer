﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Delta.CertXplorer.Internals;

// I did not write the HexViewer control, though I can't remember where it's from...
namespace Delta.CertXplorer.UI
{
    public partial class HexViewer : Control
    {
        private readonly struct BytePositionInfo
        {
            public BytePositionInfo(long characterIndex, int characterLocation)
            {
                Index = characterIndex;
                Location = characterLocation;
            }

            public int Location { get; }
            public long Index { get; }
        }

        private readonly StringFormat _stringFormat; // Contains string format information for text drawing
        private readonly VScrollBar vScrollBar; // Contains a vertical scroll
        private Rectangle _recContent; // Contains the hole content bounds of all text
        private Rectangle _recLineInfo; // Contains the line info bounds
        private Rectangle _recHex; // Contains the hex data bounds
        private Rectangle _recStringView; // Contains the string view bounds
        private SizeF _charSize; // Contains the width and height of a single char
        private int hexMaxHBytes; // Contains the maximum of visible horizontal bytes
        private int hexMaxVBytes; // Contains the maximum of visible vertical bytes
        private int hexMaxBytes; // Contains the maximum of visible bytes.
        private long scrollVmin; // Contains the scroll bars minimum value
        private long scrollVmax; // Contains the scroll bars maximum value
        private long scrollVpos; // Contains the scroll bars current position        
        private int recBorderLeft = SystemInformation.Border3DSize.Width; // Contains the border´s left shift
        private int recBorderRight = SystemInformation.Border3DSize.Width; // Contains the border´s right shift
        private int recBorderTop = SystemInformation.Border3DSize.Height; // Contains the border´s top shift
        private int recBorderBottom = SystemInformation.Border3DSize.Height; // Contains the border bottom shift
        private long startByte; // Contains the index of the first visible byte
        private long endByte; // Contains the index of the last visible byte
        private long bytePosition = -1; // Contains the current byte position
        private int byteCharacterPosition; // Contains the current char position in one byte
        private string hexStringFormat = "X"; // Contains string format information for hex values
        private IKeyInterpreter currentKeyInterpreter; // Contains the current key interpreter
        private EmptyKeyInterpreter emptyKeyInterpreter; // Contains an empty key interpreter without functionality
        private KeyInterpreter keyInterpreter; // Contains the default key interpreter
        private StringKeyInterpreter stringKeyInterpreter; // Contains the string key interpreter
        private bool caretVisible; // Contains True if caret is visible
        private bool abortFind; // Contains true, if the find (Find method) should be aborted.
        private long findingPos; // Contains a value of the current finding position.
        private bool _insertActive; // Contains a state value about Insert or Write mode. When this value is true and the ByteProvider SupportsInsert is true bytes are inserted instead of overridden.
        private int _currentPositionInLine;
        private long _currentLine;
        private Color _shadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
        private bool _shadowSelectionVisible = true;
        private Color _selectionForeColor = Color.White;
        private Color _selectionBackColor = Color.Blue;
        private long _selectionLength;
        private bool _stringViewVisible;
        private BorderStyle _borderStyle = BorderStyle.Fixed3D;
        private bool _lineInfoVisible;
        bool _vScrollBarVisible;
        bool _useFixedBytesPerLine;
        int _bytesPerLine = 16;
        private bool _readOnly;
        private byte[] data;
        private IByteProvider _byteProvider;

        public HexViewer()
        {
            InitializeComponent();

            vScrollBar = new VScrollBar();
            vScrollBar.Scroll += (s, e) =>
            {
                switch (e.Type)
                {
                    case ScrollEventType.Last:
                        break;
                    case ScrollEventType.EndScroll:
                        break;
                    case ScrollEventType.SmallIncrement:
                        PerformScrollLineDown();
                        break;
                    case ScrollEventType.SmallDecrement:
                        PerformScrollLineUp();
                        break;
                    case ScrollEventType.LargeIncrement:
                        PerformScrollPageDown();
                        break;
                    case ScrollEventType.LargeDecrement:
                        PerformScrollPageUp();
                        break;
                    case ScrollEventType.ThumbPosition:
                        var lPos = FromScrollPos(e.NewValue);
                        PerformScrollThumpPosition(lPos);
                        break;
                    case ScrollEventType.ThumbTrack:
                        break;
                    case ScrollEventType.First:
                        break;
                    default:
                        break;
                }

                e.NewValue = ToScrollPos(scrollVpos);
            };

            BackColor = Color.White;
            Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            _stringFormat = new StringFormat(StringFormat.GenericTypographic)
            {
                FormatFlags = StringFormatFlags.MeasureTrailingSpaces
            };

            ActivateEmptyKeyInterpreter();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public event EventHandler ReadOnlyChanged;
        public event EventHandler ByteProviderChanged;
        public event EventHandler SelectionStartChanged;
        public event EventHandler SelectionLengthChanged;
        public event EventHandler LineInfoVisibleChanged;
        public event EventHandler StringViewVisibleChanged;
        public event EventHandler BorderStyleChanged;
        public event EventHandler BytesPerLineChanged;
        public event EventHandler UseFixedBytesPerLineChanged;
        public event EventHandler VScrollBarVisibleChanged;
        public event EventHandler CasingChanged;
        public event EventHandler HorizontalByteCountChanged;
        public event EventHandler VerticalByteCountChanged;
        public event EventHandler CurrentLineChanged;
        public event EventHandler CurrentPositionInLineChanged;

        #region Properties

        /// <summary>
        /// Gets a value that indicates the current position during Find method execution.
        /// </summary>
        [DefaultValue(0), Browsable(false)]
        public long CurrentFindingPosition => findingPos;

        /// <summary>
        /// Gets or sets if the count of bytes in one line is fix.
        /// </summary>
        /// <remarks>
        /// When set to True, BytesPerLine property determine the maximum count of bytes in one line.
        /// </remarks>
        [DefaultValue(false), Category("Hex"), Description("Gets or sets if the count of bytes in one line is fix.")]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (_readOnly == value)
                    return;

                _readOnly = value;
                OnReadOnlyChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the maximum count of bytes in one line.
        /// </summary>
        /// <remarks>
        /// UsedFixedBytesPerLine property must set to true
        /// </remarks>
        [DefaultValue(16), Category("Hex"), Description("Gets or sets the maximum count of bytes in one line.")]
        public int BytesPerLine
        {
            get => _bytesPerLine;
            set
            {
                if (_bytesPerLine == value)
                    return;

                _bytesPerLine = value;
                OnByteProviderChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets if the count of bytes in one line is fix.
        /// </summary>
        /// <remarks>
        /// When set to True, BytesPerLine property determine the maximum count of bytes in one line.
        /// </remarks>
        [DefaultValue(false), Category("Hex"), Description("Gets or sets if the count of bytes in one line is fix.")]
        public bool UseFixedBytesPerLine
        {
            get => _useFixedBytesPerLine;
            set
            {
                if (_useFixedBytesPerLine == value)
                    return;

                _useFixedBytesPerLine = value;
                OnUseFixedBytesPerLineChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of a vertical scroll bar.
        /// </summary>
        [DefaultValue(false), Category("Hex"), Description("Gets or sets the visibility of a vertical scroll bar.")]
        public bool VScrollBarVisible
        {
            get => _vScrollBarVisible;
            set
            {
                if (_vScrollBarVisible == value)
                    return;

                _vScrollBarVisible = value;

                if (_vScrollBarVisible)
                    Controls.Add(vScrollBar);
                else
                    Controls.Remove(vScrollBar);

                UpdateRectanglePositioning();
                UpdateScrollSize();

                OnVScrollBarVisibleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of a line info.
        /// </summary>
        [DefaultValue(false), Category("Hex"), Description("Gets or sets the visibility of a line info.")]
        public bool LineInfoVisible
        {
            get => _lineInfoVisible;
            set
            {
                if (_lineInfoVisible == value)
                    return;

                _lineInfoVisible = value;
                OnLineInfoVisibleChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the hex box´s border style.
        /// </summary>
        [DefaultValue(typeof(BorderStyle), "Fixed3D"), Category("Hex"), Description("Gets or sets the hex box´s border style.")]
        public BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                if (_borderStyle == value)
                    return;

                _borderStyle = value;
                switch (_borderStyle)
                {
                    case BorderStyle.None:
                        recBorderLeft = recBorderTop = recBorderRight = recBorderBottom = 0;
                        break;
                    case BorderStyle.Fixed3D:
                        recBorderLeft = recBorderRight = SystemInformation.Border3DSize.Width;
                        recBorderTop = recBorderBottom = SystemInformation.Border3DSize.Height;
                        break;
                    case BorderStyle.FixedSingle:
                        recBorderLeft = recBorderTop = recBorderRight = recBorderBottom = 1;
                        break;
                }

                UpdateRectanglePositioning();
                OnBorderStyleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the string view.
        /// </summary>
        [DefaultValue(false), Category("Hex"), Description("Gets or sets the visibility of the string view.")]
        public bool StringViewVisible
        {
            get => _stringViewVisible;
            set
            {
                if (_stringViewVisible == value)
                    return;

                _stringViewVisible = value;
                OnStringViewVisibleChanged(EventArgs.Empty);

                UpdateRectanglePositioning();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether the HexBox control displays the hex characters in upper or lower case.
        /// </summary>
        [DefaultValue(typeof(CharacterCasing), "Upper"), Category("Hex"), Description("Gets or sets whether the HexBox control displays the hex characters in upper or lower case.")]
        public CharacterCasing Casing
        {
            get => hexStringFormat == "X" ? CharacterCasing.Upper : CharacterCasing.Lower;
            set
            {
                var format = value == CharacterCasing.Upper ? "X" : "x";

                if (hexStringFormat == format)
                    return;

                hexStringFormat = format;
                OnCasingChanged(EventArgs.Empty);

                Invalidate();
            }
        }

        /// <summary>
        /// Gets and sets the starting point of the bytes selected in the hex box.
        /// </summary>
        [Browsable(false), DefaultValue(0)]
        public long SelectionStart
        {
            get => bytePosition;
            set
            {
                SetPosition(value, 0);
                ScrollByteIntoView();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets and sets the number of bytes selected in the hex box.
        /// </summary>
        [DefaultValue(0)]
        public long SelectionLength
        {
            get => _selectionLength;
            set
            {
                SetSelectionLength(value);
                ScrollByteIntoView();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background color for the selected bytes.
        /// </summary>
        [DefaultValue(typeof(Color), "Blue"), Category("Hex"), Description("Gets or sets the background color for the selected bytes.")]
        public Color SelectionBackColor
        {
            get => _selectionBackColor;
            set
            {
                _selectionBackColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color for the selected bytes.
        /// </summary>
        [DefaultValue(typeof(Color), "White"), Category("Hex"), Description("Gets or sets the foreground color for the selected bytes.")]
        public Color SelectionForeColor
        {
            get => _selectionForeColor;
            set
            {
                _selectionForeColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of a shadow selection.
        /// </summary>
        [DefaultValue(true), Category("Hex"), Description("Gets or sets the visibility of a shadow selection.")]
        public bool ShadowSelectionVisible
        {
            get => _shadowSelectionVisible;
            set
            {
                if (_shadowSelectionVisible == value)
                    return;
                _shadowSelectionVisible = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the color of the shadow selection. 
        /// </summary>
        /// <remarks>
        /// A alpha component must be given! 
        /// Default alpha = 100
        /// </remarks>
        [Category("Hex"), Description("Gets or sets the color of the shadow selection.")]
        public Color ShadowSelectionColor
        {
            get => _shadowSelectionColor;
            set
            {
                _shadowSelectionColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the number bytes drawn horizontally.
        /// </summary>
        [DefaultValue(true), Browsable(false)]
        public int HorizontalByteCount => hexMaxHBytes;

        /// <summary>
        /// Gets the number bytes drawn vertically.
        /// </summary>
        [DefaultValue(true), Browsable(false)]
        public int VerticalByteCount => hexMaxVBytes;

        /// <summary>
        /// Gets the current line
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long CurrentLine => _currentLine;


        /// <summary>
        /// Gets the current position in the current line
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long CurrentPositionInLine => _currentPositionInLine;


        /// <summary>
        /// Gets the a value if insertion mode is active or not.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InsertActive => _insertActive;

        #endregion

        public void Load(byte[] bytes)
        {
            data = bytes;
            var provider = new ByteArrayProvider(data);

            if (_byteProvider == provider)
                return;

            ActivateKeyInterpreter();

            if (_byteProvider != null)
                _byteProvider.LengthChanged -= new EventHandler(OnByteProviderLengthChanged);

            _byteProvider = provider;
            _byteProvider.LengthChanged += new EventHandler(OnByteProviderLengthChanged);

            OnByteProviderChanged(EventArgs.Empty);

            SetPosition(0, 0);
            SetSelectionLength(0);

            if (caretVisible && Focused)
                UpdateCaret();
            else
                CreateCaret();

            CheckCurrentLineChanged();
            CheckCurrentPositionInLineChanged();

            scrollVpos = 0;

            UpdateVisibilityBytes();
            UpdateRectanglePositioning();

            Invalidate();
        }

        #region Paint methods

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);

            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    if (VisualStylesEnabled())
                    {
                        var element = Enabled ? VisualStyleElement.TextBox.TextEdit.Normal :
                            VisualStyleElement.TextBox.TextEdit.Disabled;
                        var renderer = new VisualStyleRenderer(element);
                        renderer.DrawBackground(pevent.Graphics, ClientRectangle);
                    } // draw default border
                    else ControlPaint.DrawBorder3D(pevent.Graphics, ClientRectangle, Border3DStyle.Sunken);
                    break;
                case BorderStyle.FixedSingle:
                    // draw fixed single border
                    ControlPaint.DrawBorder(pevent.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
                    break;
            }
        }

        /// <summary>
        /// Paints the hex box.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_byteProvider == null) return;

            // draw only in the content rectangle, so exclude the border and the scrollbar.
            Region r = new Region(ClientRectangle);
            r.Exclude(_recContent);
            e.Graphics.ExcludeClip(r);

            UpdateVisibilityBytes();

            if (_lineInfoVisible)
                PaintLineInfo(e.Graphics, startByte, endByte);

            if (!_stringViewVisible)
                PaintHex(e.Graphics, startByte, endByte);
            else
            {
                PaintHexAndStringView(e.Graphics, startByte, endByte);
                if (_shadowSelectionVisible)
                    PaintCurrentBytesSign(e.Graphics);
            }
        }

        private void PaintLineInfo(Graphics g, long startByte, long endByte)
        {
            Brush brush = new SolidBrush(GetDefaultForeColor());
            int maxLine = GetGridBytePoint(endByte - startByte).Y + 1;

            for (int i = 0; i < maxLine; i++)
            {
                long lastLineByte = startByte + (hexMaxHBytes) * i + hexMaxHBytes;

                PointF bytePointF = GetBytePointF(new Point(0, 0 + i));
                string info = lastLineByte.ToString(hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
                int nulls = 8 - info.Length;

                string formattedInfo;
                if (nulls > -1) formattedInfo = new string('0', 8 - info.Length) + info;
                else formattedInfo = new string('~', 8);

                g.DrawString(formattedInfo, Font, brush, new PointF(_recLineInfo.X, bytePointF.Y), _stringFormat);
            }
        }

        private void PaintHex(Graphics g, long startByte, long endByte)
        {
            Brush brush = new SolidBrush(GetDefaultForeColor());
            Brush selBrush = new SolidBrush(_selectionForeColor);
            Brush selBrushBack = new SolidBrush(_selectionBackColor);

            int counter = -1;
            long intern_endByte = Math.Min(_byteProvider.Length - 1, endByte + hexMaxHBytes);

            bool isKeyInterpreterActive = currentKeyInterpreter == null || currentKeyInterpreter.GetType() == typeof(KeyInterpreter);

            for (long i = startByte; i < intern_endByte + 1; i++)
            {
                counter++;
                Point gridPoint = GetGridBytePoint(counter);
                byte b = _byteProvider.ReadByte(i);

                bool isSelectedByte = i >= bytePosition && i <= (bytePosition + _selectionLength - 1) && _selectionLength != 0;

                if (isSelectedByte && isKeyInterpreterActive)
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint);
                else PaintHexString(g, b, brush, gridPoint);
            }
        }

        private void PaintHexString(Graphics g, byte b, Brush brush, Point gridPoint)
        {
            PointF bytePointF = GetBytePointF(gridPoint);

            string sB = b.ToString(hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (sB.Length == 1) sB = "0" + sB;

            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
            bytePointF.X += _charSize.Width;
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
        }

        private void PaintHexStringSelected(Graphics g, byte b, Brush brush, Brush brushBack, Point gridPoint)
        {
            string sB = b.ToString(hexStringFormat, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (sB.Length == 1) sB = "0" + sB;

            PointF bytePointF = GetBytePointF(gridPoint);

            bool isLastLineChar = (gridPoint.X + 1 == hexMaxHBytes);
            float bcWidth = (isLastLineChar) ? _charSize.Width * 2 : _charSize.Width * 3;

            g.FillRectangle(brushBack, bytePointF.X, bytePointF.Y, bcWidth, _charSize.Height);
            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
            bytePointF.X += _charSize.Width;
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
        }

        private void PaintHexAndStringView(Graphics g, long startByte, long endByte)
        {
            Brush brush = new SolidBrush(GetDefaultForeColor());
            Brush selBrush = new SolidBrush(_selectionForeColor);
            Brush selBrushBack = new SolidBrush(_selectionBackColor);

            var counter = -1;
            var intern_endByte = Math.Min(_byteProvider.Length - 1, endByte + hexMaxHBytes);

            var isKeyInterpreterActive = currentKeyInterpreter == null || currentKeyInterpreter.GetType() == typeof(KeyInterpreter);

            for (var i = startByte; i < intern_endByte + 1; i++)
            {
                counter++;
                var gridPoint = GetGridBytePoint(counter);
                var byteStringPointF = GetByteStringPointF(gridPoint);
                var b = _byteProvider.ReadByte(i);

                var isSelectedByte =
                    i >= bytePosition &&
                    i <= bytePosition + _selectionLength - 1 &&
                    _selectionLength != 0;

                if (isSelectedByte && isKeyInterpreterActive)
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint);
                else PaintHexString(g, b, brush, gridPoint);

                var s = b > 0x1F && !(b > 0x7E && b < 0xA0) ? ((char)b).ToString() : ".";
                if (isSelectedByte && currentKeyInterpreter is StringKeyInterpreter)
                {
                    g.FillRectangle(selBrushBack, byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height);
                    g.DrawString(s, Font, selBrush, byteStringPointF, _stringFormat);
                }
                else g.DrawString(s, Font, brush, byteStringPointF, _stringFormat);
            }
        }

        private void PaintCurrentBytesSign(Graphics g)
        {
            if (currentKeyInterpreter != null && Focused && bytePosition != -1 && Enabled)
            {
                if (currentKeyInterpreter.GetType() == typeof(KeyInterpreter))
                {
                    if (_selectionLength == 0)
                    {
                        Point gp = GetGridBytePoint(bytePosition - startByte);
                        PointF pf = GetByteStringPointF(gp);
                        Size s = new Size((int)_charSize.Width, (int)_charSize.Height);
                        Rectangle r = new Rectangle((int)pf.X, (int)pf.Y, s.Width, s.Height);
                        if (r.IntersectsWith(_recStringView))
                        {
                            r.Intersect(_recStringView);
                            PaintCurrentByteSign(g, r);
                        }
                    }
                    else
                    {
                        int lineWidth = (int)(_recStringView.Width - _charSize.Width);

                        Point startSelGridPoint = GetGridBytePoint(bytePosition - startByte);
                        PointF startSelPointF = GetByteStringPointF(startSelGridPoint);

                        Point endSelGridPoint = GetGridBytePoint(bytePosition - startByte + _selectionLength - 1);
                        PointF endSelPointF = GetByteStringPointF(endSelGridPoint);

                        int multiLine = endSelGridPoint.Y - startSelGridPoint.Y;
                        if (multiLine == 0)
                        {
                            Rectangle singleLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(endSelPointF.X - startSelPointF.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (singleLine.IntersectsWith(_recStringView))
                            {
                                singleLine.Intersect(_recStringView);
                                PaintCurrentByteSign(g, singleLine);
                            }
                        }
                        else
                        {
                            Rectangle firstLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(_recStringView.X + lineWidth - startSelPointF.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (firstLine.IntersectsWith(_recStringView))
                            {
                                firstLine.Intersect(_recStringView);
                                PaintCurrentByteSign(g, firstLine);
                            }

                            if (multiLine > 1)
                            {
                                Rectangle betweenLines = new Rectangle(
                                    _recStringView.X,
                                    (int)(startSelPointF.Y + _charSize.Height),
                                    _recStringView.Width,
                                    (int)(_charSize.Height * (multiLine - 1)));
                                if (betweenLines.IntersectsWith(_recStringView))
                                {
                                    betweenLines.Intersect(_recStringView);
                                    PaintCurrentByteSign(g, betweenLines);
                                }

                            }

                            Rectangle lastLine = new Rectangle(
                                _recStringView.X,
                                (int)endSelPointF.Y,
                                (int)(endSelPointF.X - _recStringView.X + _charSize.Width),
                                (int)_charSize.Height);
                            if (lastLine.IntersectsWith(_recStringView))
                            {
                                lastLine.Intersect(_recStringView);
                                PaintCurrentByteSign(g, lastLine);
                            }
                        }
                    }
                }
                else
                {
                    if (_selectionLength == 0)
                    {
                        Point gp = GetGridBytePoint(bytePosition - startByte);
                        PointF pf = GetBytePointF(gp);
                        Size s = new Size((int)_charSize.Width * 2, (int)_charSize.Height);
                        Rectangle r = new Rectangle((int)pf.X, (int)pf.Y, s.Width, s.Height);
                        PaintCurrentByteSign(g, r);
                    }
                    else
                    {
                        int lineWidth = (int)(_recHex.Width - _charSize.Width * 5);

                        Point startSelGridPoint = GetGridBytePoint(bytePosition - startByte);
                        PointF startSelPointF = GetBytePointF(startSelGridPoint);

                        Point endSelGridPoint = GetGridBytePoint(bytePosition - startByte + _selectionLength - 1);
                        PointF endSelPointF = GetBytePointF(endSelGridPoint);

                        int multiLine = endSelGridPoint.Y - startSelGridPoint.Y;
                        if (multiLine == 0)
                        {
                            Rectangle singleLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(endSelPointF.X - startSelPointF.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (singleLine.IntersectsWith(_recHex))
                            {
                                singleLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, singleLine);
                            }
                        }
                        else
                        {
                            Rectangle firstLine = new Rectangle(
                                (int)startSelPointF.X,
                                (int)startSelPointF.Y,
                                (int)(_recHex.X + lineWidth - startSelPointF.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (firstLine.IntersectsWith(_recHex))
                            {
                                firstLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, firstLine);
                            }

                            if (multiLine > 1)
                            {
                                Rectangle betweenLines = new Rectangle(
                                    _recHex.X,
                                    (int)(startSelPointF.Y + _charSize.Height),
                                    (int)(lineWidth + _charSize.Width * 2),
                                    (int)(_charSize.Height * (multiLine - 1)));
                                if (betweenLines.IntersectsWith(_recHex))
                                {
                                    betweenLines.Intersect(_recHex);
                                    PaintCurrentByteSign(g, betweenLines);
                                }

                            }

                            Rectangle lastLine = new Rectangle(
                                _recHex.X,
                                (int)endSelPointF.Y,
                                (int)(endSelPointF.X - _recHex.X + _charSize.Width * 2),
                                (int)_charSize.Height);
                            if (lastLine.IntersectsWith(_recHex))
                            {
                                lastLine.Intersect(_recHex);
                                PaintCurrentByteSign(g, lastLine);
                            }
                        }
                    }
                }
            }
        }

        private void PaintCurrentByteSign(Graphics g, Rectangle rec)
        {
            Bitmap myBitmap = new Bitmap(rec.Width, rec.Height);
            Graphics bitmapGraphics = Graphics.FromImage(myBitmap);

            SolidBrush greenBrush = new SolidBrush(_shadowSelectionColor);

            bitmapGraphics.FillRectangle(greenBrush, 0,
                0, rec.Width, rec.Height);

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;

            g.DrawImage(myBitmap, rec.Left, rec.Top);
        }

        private Color GetDefaultForeColor()
        {
            if (Enabled) return ForeColor;
            else return Color.Gray;
        }

        private void UpdateVisibilityBytes()
        {
            if (_byteProvider == null || _byteProvider.Length == 0)
                return;

            startByte = (scrollVpos + 1) * hexMaxHBytes - hexMaxHBytes;
            endByte = Math.Min(_byteProvider.Length - 1, startByte + hexMaxBytes);
        }

        #endregion

        #region Scroll methods

        private void UpdateScrollSize()
        {
            System.Diagnostics.Debug.WriteLine("UpdateScrollSize()", "HexBox");

            // calc scroll bar info
            if (VScrollBarVisible && _byteProvider != null && _byteProvider.Length > 0 && hexMaxHBytes != 0)
            {
                long scrollmax = (long)Math.Ceiling((double)_byteProvider.Length / (double)hexMaxHBytes - (double)hexMaxVBytes);
                scrollmax = Math.Max(0, scrollmax);

                long scrollpos = startByte / hexMaxHBytes;

                if (scrollmax == scrollVmax && scrollpos == scrollVpos)
                    return;

                scrollVmin = 0;
                scrollVmax = scrollmax;
                scrollVpos = Math.Min(scrollpos, scrollmax);
                UpdateVScroll();
            }
            else if (VScrollBarVisible)
            {
                // disable scroll bar
                scrollVmin = 0;
                scrollVmax = 0;
                scrollVpos = 0;
                UpdateVScroll();
            }
        }

        private void UpdateVScroll()
        {
            System.Diagnostics.Debug.WriteLine("UpdateVScroll()", "HexBox");

            int max = ToScrollMax(scrollVmax);

            if (max > 0)
            {
                vScrollBar.Minimum = 0;
                vScrollBar.Maximum = max;
                vScrollBar.Value = ToScrollPos(scrollVpos);
                vScrollBar.Enabled = true;
            }
            else vScrollBar.Enabled = false;
        }

        private int ToScrollPos(long value)
        {
            int max = 65535;

            if (scrollVmax < max) return (int)value;

            double valperc = (double)value / (double)scrollVmax * (double)100;
            int res = (int)Math.Floor((double)max / (double)100 * valperc);
            res = (int)Math.Max(scrollVmin, res);
            res = (int)Math.Min(scrollVmax, res);
            return res;
        }

        private long FromScrollPos(int value)
        {
            int max = 65535;
            if (scrollVmax < max) return (long)value;

            double valperc = (double)value / (double)max * (double)100;
            long res = (int)Math.Floor((double)scrollVmax / (double)100 * valperc);
            return res;
        }

        private int ToScrollMax(long value)
        {
            long max = 65535;
            if (value > max) return (int)max;
            else return (int)value;
        }

        private void PerformScrollToLine(long pos)
        {
            if (pos < scrollVmin || pos > scrollVmax || pos == scrollVpos)
                return;

            scrollVpos = pos;

            UpdateVScroll();
            UpdateVisibilityBytes();
            UpdateCaret();
            Invalidate();
        }

        private void PerformScrollLines(int lines)
        {
            long pos = 0L;
            if (lines > 0) pos = Math.Min(scrollVmax, scrollVpos + lines);
            else if (lines < 0) pos = Math.Max(scrollVmin, scrollVpos + lines);
            else return;

            PerformScrollToLine(pos);
        }

        private void PerformScrollLineDown()
        {
            PerformScrollLines(1);
        }

        private void PerformScrollLineUp()
        {
            PerformScrollLines(-1);
        }

        private void PerformScrollPageDown()
        {
            PerformScrollLines(hexMaxVBytes);
        }

        private void PerformScrollPageUp()
        {
            PerformScrollLines(-hexMaxVBytes);
        }

        private void PerformScrollThumpPosition(long pos)
        {
            PerformScrollToLine(pos);
        }

        /// <summary>
        /// Scrolls the selection start byte into view
        /// </summary>
        public void ScrollByteIntoView()
        {
            System.Diagnostics.Debug.WriteLine("ScrollByteIntoView()", "HexBox");

            ScrollByteIntoView(bytePosition);
        }

        /// <summary>
        /// Scrolls the specific byte into view
        /// </summary>
        /// <param name="index">the index of the byte</param>
        public void ScrollByteIntoView(long index)
        {
            System.Diagnostics.Debug.WriteLine("ScrollByteIntoView(long index)", "HexBox");

            if (_byteProvider == null || currentKeyInterpreter == null)
                return;

            if (index < startByte)
            {
                long line = (long)Math.Floor((double)index / (double)hexMaxHBytes);
                PerformScrollThumpPosition(line);
            }
            else if (index > endByte)
            {
                long line = (long)Math.Floor((double)index / (double)hexMaxHBytes);
                line -= hexMaxVBytes - 1;
                PerformScrollThumpPosition(line);
            }
        }

        #endregion

        #region Selection methods

        /// <summary>
        /// Selects the hex box.
        /// </summary>
        /// <param name="start">the start index of the selection</param>
        /// <param name="length">the length of the selection</param>
        public void Select(long start, long length)
        {
            InternalSelect(start, length);
            ScrollByteIntoView();
        }

        private void ReleaseSelection()
        {
            System.Diagnostics.Debug.WriteLine("ReleaseSelection()", "HexBox");

            if (_selectionLength == 0)
                return;
            _selectionLength = 0;
            OnSelectionLengthChanged(EventArgs.Empty);

            if (!caretVisible)
                CreateCaret();
            else
                UpdateCaret();

            Invalidate();
        }

        private void InternalSelect(long start, long length)
        {
            long pos = start;
            long sel = length;
            int cp = 0;

            if (sel > 0 && caretVisible) DestroyCaret();
            else if (sel == 0 && !caretVisible) CreateCaret();

            SetPosition(pos, cp);
            SetSelectionLength(sel);

            UpdateCaret();
            Invalidate();
        }

        #endregion

        #region Positioning methods

        private void UpdateRectanglePositioning()
        {
            // calc char size
            SizeF charSize = this.CreateGraphics().MeasureString("A", Font, 100, _stringFormat);
            _charSize = new SizeF((float)Math.Ceiling(charSize.Width), (float)Math.Ceiling(charSize.Height));

            // calc content bounds
            _recContent = ClientRectangle;
            _recContent.X += recBorderLeft;
            _recContent.Y += recBorderTop;
            _recContent.Width -= recBorderRight + recBorderLeft;
            _recContent.Height -= recBorderBottom + recBorderTop;

            if (_vScrollBarVisible)
            {
                _recContent.Width -= vScrollBar.Width;
                vScrollBar.Left = _recContent.X + _recContent.Width;
                vScrollBar.Top = _recContent.Y;
                vScrollBar.Height = _recContent.Height;
            }

            int marginLeft = 4;

            // calc line info bounds
            if (_lineInfoVisible)
            {
                _recLineInfo = new Rectangle(_recContent.X + marginLeft,
                    _recContent.Y,
                    (int)(_charSize.Width * 10),
                    _recContent.Height);
            }
            else
            {
                _recLineInfo = Rectangle.Empty;
                _recLineInfo.X = marginLeft;
            }

            // calc hex bounds and grid
            _recHex = new Rectangle(_recLineInfo.X + _recLineInfo.Width,
                _recLineInfo.Y,
                _recContent.Width - _recLineInfo.Width,
                _recContent.Height);

            if (UseFixedBytesPerLine)
            {
                SetHorizontalByteCount(_bytesPerLine);
                _recHex.Width = (int)Math.Floor(((double)hexMaxHBytes) * _charSize.Width * 3 + (2 * _charSize.Width));
            }
            else
            {
                int hmax = (int)Math.Floor((double)_recHex.Width / (double)_charSize.Width);
                if (hmax > 1)
                    SetHorizontalByteCount((int)Math.Floor((double)hmax / 3));
                else SetHorizontalByteCount(hmax);
            }

            if (_stringViewVisible)
            {
                _recStringView = new Rectangle(_recHex.X + _recHex.Width,
                    _recHex.Y,
                    (int)(_charSize.Width * hexMaxHBytes),
                    _recHex.Height);
            }
            else _recStringView = Rectangle.Empty;

            int vmax = (int)Math.Floor((double)_recHex.Height / (double)_charSize.Height);
            SetVerticalByteCount(vmax);

            hexMaxBytes = hexMaxHBytes * hexMaxVBytes;

            UpdateScrollSize();
        }

        private PointF GetBytePointF(long byteIndex)
        {
            Point gp = GetGridBytePoint(byteIndex);
            return GetBytePointF(gp);
        }

        private PointF GetBytePointF(Point gp)
        {
            float x = (3 * _charSize.Width) * gp.X + _recHex.X;
            float y = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recHex.Y;

            return new PointF(x, y);
        }

        private PointF GetByteStringPointF(Point gp)
        {
            float x = (_charSize.Width) * gp.X + _recStringView.X;
            float y = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recStringView.Y;

            return new PointF(x, y);
        }

        private Point GetGridBytePoint(long byteIndex)
        {
            int row = (int)Math.Floor((double)byteIndex / (double)hexMaxHBytes);
            int column = (int)(byteIndex + hexMaxHBytes - hexMaxHBytes * (row + 1));

            Point res = new Point(column, row);
            return res;
        }

        #endregion

        #region Key interpreter methods

        private void ActivateEmptyKeyInterpreter()
        {
            if (emptyKeyInterpreter == null)
                emptyKeyInterpreter = new EmptyKeyInterpreter(this);

            if (emptyKeyInterpreter == currentKeyInterpreter)
                return;

            if (currentKeyInterpreter != null)
                currentKeyInterpreter.DeactivateMouseEvents();

            currentKeyInterpreter = emptyKeyInterpreter;
            currentKeyInterpreter.ActivateMouseEvents();
        }

        private void ActivateKeyInterpreter()
        {
            if (keyInterpreter == null)
                keyInterpreter = new KeyInterpreter(this);

            if (keyInterpreter == currentKeyInterpreter)
                return;

            if (currentKeyInterpreter != null)
                currentKeyInterpreter.DeactivateMouseEvents();

            currentKeyInterpreter = keyInterpreter;
            currentKeyInterpreter.ActivateMouseEvents();
        }

        private void ActivateStringKeyInterpreter()
        {
            if (stringKeyInterpreter == null)
                stringKeyInterpreter = new StringKeyInterpreter(this);

            if (stringKeyInterpreter == currentKeyInterpreter)
                return;

            if (currentKeyInterpreter != null)
                currentKeyInterpreter.DeactivateMouseEvents();

            currentKeyInterpreter = stringKeyInterpreter;
            currentKeyInterpreter.ActivateMouseEvents();
        }

        #endregion

        #region Caret methods

        private void CreateCaret()
        {
            if (_byteProvider == null || currentKeyInterpreter == null || caretVisible || !this.Focused)
                return;

            System.Diagnostics.Debug.WriteLine("CreateCaret()", "HexBox");

            NativeMethods.CreateCaret(Handle, IntPtr.Zero, 1, (int)_charSize.Height);

            UpdateCaret();

            NativeMethods.ShowCaret(Handle);

            caretVisible = true;
        }

        private void UpdateCaret()
        {
            if (_byteProvider == null || currentKeyInterpreter == null)
                return;

            System.Diagnostics.Debug.WriteLine("UpdateCaret()", "HexBox");

            long byteIndex = bytePosition - startByte;
            PointF p = currentKeyInterpreter.GetCaretPointF(byteIndex);
            p.X += byteCharacterPosition * _charSize.Width;
            NativeMethods.SetCaretPos((int)p.X, (int)p.Y);
        }

        private void DestroyCaret()
        {
            if (!caretVisible)
                return;

            System.Diagnostics.Debug.WriteLine("DestroyCaret()", "HexBox");

            NativeMethods.DestroyCaret();
            caretVisible = false;
        }

        private void SetCaretPosition(Point p)
        {
            System.Diagnostics.Debug.WriteLine("SetCaretPosition()", "HexBox");

            if (_byteProvider == null || currentKeyInterpreter == null)
                return;

            if (_recHex.Contains(p))
            {
                var bpi = GetHexBytePositionInfo(p);
                SetPosition(bpi.Index, bpi.Location);

                ActivateKeyInterpreter();
                UpdateCaret();
                Invalidate();
            }
            else if (_recStringView.Contains(p))
            {
                var bpi = GetStringBytePositionInfo(p);
                SetPosition(bpi.Index, bpi.Location);

                ActivateStringKeyInterpreter();
                UpdateCaret();
                Invalidate();
            }
        }

        private BytePositionInfo GetHexBytePositionInfo(Point p)
        {
            System.Diagnostics.Debug.WriteLine("GetHexBytePositionInfo()", "HexBox");

            long bytePos;
            int byteCharaterPos;

            float x = ((float)(p.X - _recHex.X) / _charSize.Width);
            float y = ((float)(p.Y - _recHex.Y) / _charSize.Height);
            int iX = (int)x;
            int iY = (int)y;

            int hPos = (iX / 3 + 1);

            bytePos = Math.Min(_byteProvider.Length,
                startByte + (hexMaxHBytes * (iY + 1) - hexMaxHBytes) + hPos - 1);
            byteCharaterPos = (iX % 3);
            if (byteCharaterPos > 1)
                byteCharaterPos = 1;

            if (bytePos == _byteProvider.Length)
                byteCharaterPos = 0;

            if (bytePos < 0)
                return new BytePositionInfo(0, 0);
            return new BytePositionInfo(bytePos, byteCharaterPos);
        }

        private BytePositionInfo GetStringBytePositionInfo(Point p)
        {
            System.Diagnostics.Debug.WriteLine("GetStringBytePositionInfo()", "HexBox");

            long bytePos;
            int byteCharacterPos;

            float x = ((float)(p.X - _recStringView.X) / _charSize.Width);
            float y = ((float)(p.Y - _recStringView.Y) / _charSize.Height);
            int iX = (int)x;
            int iY = (int)y;

            int hPos = iX + 1;

            bytePos = Math.Min(_byteProvider.Length,
                startByte + (hexMaxHBytes * (iY + 1) - hexMaxHBytes) + hPos - 1);
            byteCharacterPos = 0;

            if (bytePos < 0)
                return new BytePositionInfo(0, 0);
            return new BytePositionInfo(bytePos, byteCharacterPos);
        }

        #endregion

        #region PreProcessMessage methods

        /// <summary>
        /// Preprocesses windows messages.
        /// </summary>
        /// <param name="msg">the message to process.</param>
        /// <returns>true, if the message was processed</returns>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true),
        SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        public override bool PreProcessMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case NativeMethods.WM_KEYDOWN:
                    return currentKeyInterpreter.PreProcessWmKeyDown(ref msg);
                case NativeMethods.WM_CHAR:
                    return currentKeyInterpreter.PreProcessWmChar(ref msg);
                case NativeMethods.WM_KEYUP:
                    return currentKeyInterpreter.PreProcessWmKeyUp(ref msg);
                default: return base.PreProcessMessage(ref msg);
            }
        }

        /// <summary>
        /// Bases the pre process message.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private bool BasePreProcessMessage(ref Message m)
        {
            return base.PreProcessMessage(ref m);
        }

        #endregion

        #region Find methods

        /// <summary>
        /// Searches the current ByteProvider
        /// </summary>
        /// <param name="bytes">the array of bytes to find</param>
        /// <param name="startIndex">the start index</param>
        /// <returns>the SelectionStart property value if find was successfull or
        /// -1 if there is no match
        /// -2 if Find was aborted.</returns>
        public long Find(byte[] bytes, long startIndex)
        {
            int match = 0;
            int bytesLength = bytes.Length;

            for (var pos = startIndex; pos < _byteProvider.Length; pos++)
            {
                if (pos % 1000 == 0) // for performance reasons: DoEvents only 1 times per 1000 loops
                    Application.DoEvents();

                if (_byteProvider.ReadByte(pos) != bytes[match])
                {
                    pos -= match;
                    match = 0;
                    findingPos = pos;
                    continue;
                }

                match++;

                if (match == bytesLength)
                {
                    long bytePos = pos - bytesLength + 1;
                    Select(bytePos, bytesLength);
                    ScrollByteIntoView(bytePosition + _selectionLength);
                    ScrollByteIntoView(bytePosition);

                    return bytePos;
                }
            }

            return -1;
        }

        #endregion

        #region Copy, Cut and Paste methods

        /// <summary>
        /// Copies the current selection in the hex box to the Clipboard.
        /// </summary>
        public void Copy()
        {
            if (!CanCopy()) return;

            // put bytes into buffer
            byte[] buffer = new byte[_selectionLength];
            int id = -1;

            for (long i = bytePosition; i < bytePosition + _selectionLength; i++)
            {
                id++;
                buffer[id] = _byteProvider.ReadByte(i);
            }

            DataObject da = new DataObject();

            // set string buffer clipbard data
            string sBuffer = System.Text.Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            da.SetData(typeof(string), sBuffer);

            //set memorystream (BinaryData) clipboard data
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, true);
            da.SetData("BinaryData", ms);

            Clipboard.SetDataObject(da, true);
            UpdateCaret();
            ScrollByteIntoView();
            Invalidate();
        }

        /// <summary>
        /// Return true if Copy method could be invoked.
        /// </summary>
        public bool CanCopy()
        {
            if (_selectionLength < 1 || _byteProvider == null) return false;
            return true;
        }

        /// <summary>
        /// Moves the current selection in the hex box to the Clipboard.
        /// </summary>
        public void Cut()
        {
            if (!CanCut()) return;

            Copy();

            _byteProvider.DeleteBytes(bytePosition, _selectionLength);
            byteCharacterPosition = 0;
            UpdateCaret();
            ScrollByteIntoView();
            ReleaseSelection();
            Invalidate();
            Refresh();
        }

        /// <summary>
        /// Return true if Cut method could be invoked.
        /// </summary>
        public bool CanCut()
        {
            if (_byteProvider == null) return false;
            if (_selectionLength < 1 || !_byteProvider.SupportsDeleteBytes) return false;
            return true;
        }

        /// <summary>
        /// Replaces the current selection in the hex box with the contents of the Clipboard.
        /// </summary>
        public void Paste()
        {
            if (!CanPaste()) return;

            if (_selectionLength > 0)
                _byteProvider.DeleteBytes(bytePosition, _selectionLength);

            byte[] buffer = null;
            IDataObject da = Clipboard.GetDataObject();

            if (da.GetDataPresent("BinaryData"))
            {
                System.IO.MemoryStream ms = (System.IO.MemoryStream)da.GetData("BinaryData");
                buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);
            }
            else if (da.GetDataPresent(typeof(string)))
            {
                string sBuffer = (string)da.GetData(typeof(string));
                buffer = System.Text.Encoding.ASCII.GetBytes(sBuffer);
            }
            else return;

            _byteProvider.InsertBytes(bytePosition, buffer);

            SetPosition(bytePosition + buffer.Length, 0);

            ReleaseSelection();
            ScrollByteIntoView();
            UpdateCaret();
            Invalidate();
        }

        /// <summary>
        /// Return true if Paste method could be invoked.
        /// </summary>
        public bool CanPaste()
        {
            if (_byteProvider == null || !_byteProvider.SupportsInsertBytes) return false;
            if (!_byteProvider.SupportsDeleteBytes && _selectionLength > 0) return false;

            IDataObject da = Clipboard.GetDataObject();
            if (da.GetDataPresent("BinaryData")) return true;
            else if (da.GetDataPresent(typeof(string))) return true;
            else return false;
        }

        #endregion

        #region Misc

        void SetPosition(long bytePos) => SetPosition(bytePos, byteCharacterPosition);

        void SetPosition(long bytePos, int characterPos)
        {
            byteCharacterPosition = characterPos;
            if (bytePos != bytePosition)
            {
                bytePosition = bytePos;
                CheckCurrentLineChanged();
                CheckCurrentPositionInLineChanged();

                OnSelectionStartChanged(EventArgs.Empty);
            }
        }

        void SetSelectionLength(long selectionLength)
        {
            if (selectionLength != _selectionLength)
            {
                _selectionLength = selectionLength;
                OnSelectionLengthChanged(EventArgs.Empty);
            }
        }

        void SetHorizontalByteCount(int value)
        {
            if (hexMaxHBytes == value)
                return;

            hexMaxHBytes = value;
            OnHorizontalByteCountChanged(EventArgs.Empty);
        }

        void SetVerticalByteCount(int value)
        {
            if (hexMaxVBytes == value)
                return;

            hexMaxVBytes = value;
            OnVerticalByteCountChanged(EventArgs.Empty);
        }

        void CheckCurrentLineChanged()
        {
            long currentLine = (long)Math.Floor((double)bytePosition / (double)hexMaxHBytes) + 1;

            if (_byteProvider == null && _currentLine != 0)
            {
                _currentLine = 0;
                OnCurrentLineChanged(EventArgs.Empty);
            }
            else if (currentLine != _currentLine)
            {
                _currentLine = currentLine;
                OnCurrentLineChanged(EventArgs.Empty);
            }
        }

        void CheckCurrentPositionInLineChanged()
        {
            Point gb = GetGridBytePoint(bytePosition);
            int currentPositionInLine = gb.X + 1;

            if (_byteProvider == null && _currentPositionInLine != 0)
            {
                _currentPositionInLine = 0;
                OnCurrentPositionInLineChanged(EventArgs.Empty);
            }
            else if (currentPositionInLine != _currentPositionInLine)
            {
                _currentPositionInLine = currentPositionInLine;
                OnCurrentPositionInLineChanged(EventArgs.Empty);
            }
        }

        private bool VisualStylesEnabled() =>
            VisualStyleInformation.IsSupportedByOS &&
            VisualStyleInformation.IsEnabledByUser &&
            (Application.VisualStyleState == VisualStyleState.ClientAreaEnabled ||
            Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled);

        /// <summary>
        /// Raises the ReadOnlyChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnReadOnlyChanged(EventArgs e)
        {
            if (ReadOnlyChanged != null)
                ReadOnlyChanged(this, e);
        }

        /// <summary>
        /// Raises the ByteProviderChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnByteProviderChanged(EventArgs e)
        {
            if (ByteProviderChanged != null)
                ByteProviderChanged(this, e);
        }

        /// <summary>
        /// Raises the SelectionStartChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnSelectionStartChanged(EventArgs e)
        {
            if (SelectionStartChanged != null)
                SelectionStartChanged(this, e);
        }

        /// <summary>
        /// Raises the SelectionLengthChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnSelectionLengthChanged(EventArgs e)
        {
            if (SelectionLengthChanged != null)
                SelectionLengthChanged(this, e);
        }

        /// <summary>
        /// Raises the LineInfoVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnLineInfoVisibleChanged(EventArgs e)
        {
            if (LineInfoVisibleChanged != null)
                LineInfoVisibleChanged(this, e);
        }

        /// <summary>
        /// Raises the StringViewVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnStringViewVisibleChanged(EventArgs e)
        {
            if (StringViewVisibleChanged != null)
                StringViewVisibleChanged(this, e);
        }

        /// <summary>
        /// Raises the BorderStyleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            if (BorderStyleChanged != null)
                BorderStyleChanged(this, e);
        }

        /// <summary>
        /// Raises the UseFixedBytesPerLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnUseFixedBytesPerLineChanged(EventArgs e)
        {
            if (UseFixedBytesPerLineChanged != null)
                UseFixedBytesPerLineChanged(this, e);
        }

        /// <summary>
        /// Raises the BytesPerLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnBytesPerLineChanged(EventArgs e)
        {
            if (BytesPerLineChanged != null)
                BytesPerLineChanged(this, e);
        }

        /// <summary>
        /// Raises the VScrollBarVisibleChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnVScrollBarVisibleChanged(EventArgs e)
        {
            if (VScrollBarVisibleChanged != null)
                VScrollBarVisibleChanged(this, e);
        }

        /// <summary>
        /// Raises the HexCasingChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCasingChanged(EventArgs e)
        {
            if (CasingChanged != null)
                CasingChanged(this, e);
        }

        /// <summary>
        /// Raises the HorizontalByteCountChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnHorizontalByteCountChanged(EventArgs e)
        {
            if (HorizontalByteCountChanged != null)
                HorizontalByteCountChanged(this, e);
        }

        /// <summary>
        /// Raises the VerticalByteCountChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnVerticalByteCountChanged(EventArgs e)
        {
            if (VerticalByteCountChanged != null)
                VerticalByteCountChanged(this, e);
        }

        /// <summary>
        /// Raises the CurrentLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCurrentLineChanged(EventArgs e)
        {
            if (CurrentLineChanged != null)
                CurrentLineChanged(this, e);
        }

        /// <summary>
        /// Raises the CurrentPositionInLineChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCurrentPositionInLineChanged(EventArgs e)
        {
            if (CurrentPositionInLineChanged != null)
                CurrentPositionInLineChanged(this, e);
        }

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnMouseDown()", "HexBox");

            if (!Focused) Focus();
            SetCaretPosition(new Point(e.X, e.Y));
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the MouseWhell event
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int linesToScroll = -(e.Delta * SystemInformation.MouseWheelScrollLines / 120);
            PerformScrollLines(linesToScroll);

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Raises the Resize event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRectanglePositioning();
        }

        /// <summary>
        /// Raises the GotFocus event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnGotFocus()", "HexBox");

            base.OnGotFocus(e);
            CreateCaret();
        }

        /// <summary>
        /// Raises the LostFocus event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnLostFocus()", "HexBox");

            base.OnLostFocus(e);
            DestroyCaret();
        }

        private void OnByteProviderLengthChanged(object sender, EventArgs e)
        {
            UpdateScrollSize();
        }

        #endregion
    }
}
