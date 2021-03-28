using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

namespace Delta.CertXplorer.UI
{
    partial class HexViewer
    {
        /// <summary>
        /// Defines a user input handler such as for mouse and keyboard input
        /// </summary>
        private interface IKeyInterpreter
        {
            void ActivateMouseEvents();
            void DeactivateMouseEvents();
            bool PreProcessWmKeyUp(ref Message m);
            bool PreProcessWmChar(ref Message m);
            bool PreProcessWmKeyDown(ref Message m);
            PointF GetCaretPointF(long byteIndex);
        }

        private class KeyInterpreter : IKeyInterpreter
        {
            private bool shiftDown;
            private bool mouseDown;
            private BytePositionInfo startPositionInfo;

            public KeyInterpreter(HexViewer owner) => HexViewer = owner ?? throw new ArgumentNullException(nameof(owner));

            protected HexViewer HexViewer { get; }

            public virtual void ActivateMouseEvents()
            {
                HexViewer.MouseDown += new MouseEventHandler(BeginMouseSelection);
                HexViewer.MouseMove += new MouseEventHandler(UpdateMouseSelection);
                HexViewer.MouseUp += new MouseEventHandler(EndMouseSelection);
            }

            public virtual void DeactivateMouseEvents()
            {
                HexViewer.MouseDown -= new MouseEventHandler(BeginMouseSelection);
                HexViewer.MouseMove -= new MouseEventHandler(UpdateMouseSelection);
                HexViewer.MouseUp -= new MouseEventHandler(EndMouseSelection);
            }

            public virtual bool PreProcessWmKeyDown(ref Message m)
            {
                Debug.WriteLine("PreProcessWmKeyDown(ref Message m)", "KeyInterpreter");

                var vc = (Keys)m.WParam.ToInt32();
                var keyData = vc | Control.ModifierKeys;

                switch (keyData)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Right:
                    case Keys.Down:
                    case Keys.PageUp:
                    case Keys.PageDown:
                    case Keys.Left | Keys.Shift:
                    case Keys.Up | Keys.Shift:
                    case Keys.Right | Keys.Shift:
                    case Keys.Down | Keys.Shift:
                    case Keys.Tab:
                    case Keys.Back:
                    case Keys.Delete:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.ShiftKey | Keys.Shift:
                    case Keys.C | Keys.Control:
                    case Keys.X | Keys.Control:
                    case Keys.V | Keys.Control:
                        if (RaiseKeyDown(keyData))
                            return true;
                        break;
                }

                switch (keyData)
                {
                    case Keys.Left:						// move left
                        return PreProcessWmKeyDown_Left(ref m);
                    case Keys.Up:						// move up
                        return PreProcessWmKeyDown_Up(ref m);
                    case Keys.Right:					// move right
                        return PreProcessWmKeyDown_Right(ref m);
                    case Keys.Down:						// move down
                        return PreProcessWmKeyDown_Down(ref m);
                    case Keys.PageUp:					// move pageup
                        return PreProcessWmKeyDown_PageUp(ref m);
                    case Keys.PageDown:					// move pagedown
                        return PreProcessWmKeyDown_PageDown(ref m);
                    case Keys.Left | Keys.Shift:		// move left with selection
                        return PreProcessWmKeyDown_ShiftLeft(ref m);
                    case Keys.Up | Keys.Shift:			// move up with selection
                        return PreProcessWmKeyDown_ShiftUp(ref m);
                    case Keys.Right | Keys.Shift:		// move right with selection
                        return PreProcessWmKeyDown_ShiftRight(ref m);
                    case Keys.Down | Keys.Shift:		// move down with selection
                        return PreProcessWmKeyDown_ShiftDown(ref m);
                    case Keys.Tab:						// switch focus to string view
                        return PreProcessWmKeyDown_Tab(ref m);
                    case Keys.Back:						// back
                        return PreProcessWmKeyDown_Back(ref m);
                    case Keys.Delete:					// delete
                        return PreProcessWmKeyDown_Delete(ref m);
                    case Keys.Home:						// move to home
                        return PreProcessWmKeyDown_Home(ref m);
                    case Keys.End:						// move to end
                        return PreProcessWmKeyDown_End(ref m);
                    case Keys.ShiftKey | Keys.Shift:	// begin selection process
                        return PreProcessWmKeyDown_ShiftShiftKey(ref m);
                    case Keys.C | Keys.Control:			// copy
                        return PreProcessWmKeyDown_ControlC(ref m);
                    case Keys.X | Keys.Control:			// cut
                        return PreProcessWmKeyDown_ControlX(ref m);
                    case Keys.V | Keys.Control:			// paste
                        return PreProcessWmKeyDown_ControlV(ref m);
                    default:
                        HexViewer.ScrollByteIntoView();
                        return HexViewer.BasePreProcessMessage(ref m);
                }
            }

            public virtual PointF GetCaretPointF(long byteIndex)
            {
                Debug.WriteLine("GetCaretPointF()", "KeyInterpreter");
                return HexViewer.GetBytePointF(byteIndex);
            }

            public virtual bool PreProcessWmChar(ref Message m)
            {
                if (ModifierKeys == Keys.Control)
                    return HexViewer.BasePreProcessMessage(ref m);

                var sw = HexViewer._byteProvider.SupportsWriteByte;
                var si = HexViewer._byteProvider.SupportsInsertBytes;
                var sd = HexViewer._byteProvider.SupportsDeleteBytes;

                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;
                var cp = HexViewer.byteCharacterPosition;

                if (!sw && pos != HexViewer._byteProvider.Length ||
                    !si && pos == HexViewer._byteProvider.Length)
                    return HexViewer.BasePreProcessMessage(ref m);

                var c = (char)m.WParam.ToInt32();
                if (Uri.IsHexDigit(c))
                {
                    if (RaiseKeyPress(c)) return true;

                    if (HexViewer.ReadOnly) return true;

                    var isInsertMode = pos == HexViewer._byteProvider.Length;

                    // do insert when insertActive = true
                    if (!isInsertMode && si && HexViewer._insertActive && cp == 0)
                        isInsertMode = true;

                    if (sd && si && sel > 0)
                    {
                        HexViewer._byteProvider.DeleteBytes(pos, sel);
                        isInsertMode = true;
                        cp = 0;
                        HexViewer.SetPosition(pos, cp);
                    }

                    HexViewer.ReleaseSelection();

                    byte currentByte = 0;
                    if (!isInsertMode) currentByte = HexViewer._byteProvider.ReadByte(pos);

                    var sCb = currentByte.ToString("X", System.Threading.Thread.CurrentThread.CurrentCulture);
                    if (sCb.Length == 1) sCb = "0" + sCb;

                    var sNewCb = c.ToString();
                    if (cp == 0) sNewCb += sCb.Substring(1, 1);
                    else sNewCb = sCb.Substring(0, 1) + sNewCb;

                    var newcb = byte.Parse(sNewCb,
                        NumberStyles.AllowHexSpecifier, Thread.CurrentThread.CurrentCulture);

                    if (isInsertMode)
                        HexViewer._byteProvider.InsertBytes(pos, new byte[] { newcb });
                    else HexViewer._byteProvider.WriteByte(pos, newcb);

                    _ = PerformPosMoveRight();

                    HexViewer.Invalidate();
                    return true;
                }
                
                return HexViewer.BasePreProcessMessage(ref m);
            }

            public virtual bool PreProcessWmKeyUp(ref Message m)
            {
                Debug.WriteLine("PreProcessWmKeyUp(ref Message m)", "KeyInterpreter");

                var vc = (Keys)m.WParam.ToInt32();
                var keyData = vc | Control.ModifierKeys;

                switch (keyData)
                {
                    case Keys.ShiftKey:
                    case Keys.Insert:
                        if (RaiseKeyUp(keyData))
                            return true;
                        break;
                }

                switch (keyData)
                {
                    case Keys.ShiftKey:
                        shiftDown = false;
                        return true;
                    case Keys.Insert:
                        return PreProcessWmKeyUp_Insert(ref m);
                    default:
                        return HexViewer.BasePreProcessMessage(ref m);
                }
            }
            
            protected virtual bool PreProcessWmKeyDown_Left(ref Message m) => PerformPosMoveLeft();

            protected virtual bool PreProcessWmKeyDown_Up(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var cp = HexViewer.byteCharacterPosition;

                if (!(pos == 0 && cp == 0))
                {
                    pos = Math.Max(-1, pos - HexViewer.hexMaxHBytes);
                    if (pos == -1)
                        return true;

                    HexViewer.SetPosition(pos);

                    if (pos < HexViewer.startByte)
                    {
                        HexViewer.PerformScrollLineUp();
                    }

                    HexViewer.UpdateCaret();
                    HexViewer.Invalidate();
                }

                HexViewer.ScrollByteIntoView();
                HexViewer.ReleaseSelection();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Right(ref Message m) => PerformPosMoveRight();

            protected virtual bool PreProcessWmKeyDown_Down(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var cp = HexViewer.byteCharacterPosition;

                if (pos == HexViewer._byteProvider.Length && cp == 0)
                    return true;

                pos = Math.Min(HexViewer._byteProvider.Length, pos + HexViewer.hexMaxHBytes);

                if (pos == HexViewer._byteProvider.Length)
                    cp = 0;

                HexViewer.SetPosition(pos, cp);

                if (pos > HexViewer.endByte - 1)
                {
                    HexViewer.PerformScrollLineDown();
                }

                HexViewer.UpdateCaret();
                HexViewer.ScrollByteIntoView();
                HexViewer.ReleaseSelection();
                HexViewer.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_PageUp(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var cp = HexViewer.byteCharacterPosition;

                if (pos == 0 && cp == 0)
                    return true;

                pos = Math.Max(0, pos - HexViewer.hexMaxBytes);
                if (pos == 0)
                    return true;

                HexViewer.SetPosition(pos);

                if (pos < HexViewer.startByte)
                {
                    HexViewer.PerformScrollPageUp();
                }

                HexViewer.ReleaseSelection();
                HexViewer.UpdateCaret();
                HexViewer.Invalidate();
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_PageDown(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var cp = HexViewer.byteCharacterPosition;

                if (pos == HexViewer._byteProvider.Length && cp == 0)
                    return true;

                pos = Math.Min(HexViewer._byteProvider.Length, pos + HexViewer.hexMaxBytes);

                if (pos == HexViewer._byteProvider.Length)
                    cp = 0;

                HexViewer.SetPosition(pos, cp);

                if (pos > HexViewer.endByte - 1)
                {
                    HexViewer.PerformScrollPageDown();
                }

                HexViewer.ReleaseSelection();
                HexViewer.UpdateCaret();
                HexViewer.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftLeft(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;

                if (pos + sel < 1)
                    return true;

                if (pos + sel <= startPositionInfo.Index)
                {
                    if (pos == 0)
                        return true;

                    pos--;
                    sel++;
                }
                else
                {
                    sel = Math.Max(0, sel - 1);
                }

                HexViewer.ScrollByteIntoView();
                HexViewer.InternalSelect(pos, sel);

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftUp(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;

                if (pos - HexViewer.hexMaxHBytes < 0 && pos <= startPositionInfo.Index)
                    return true;

                if (startPositionInfo.Index >= pos + sel)
                {
                    pos -= HexViewer.hexMaxHBytes;
                    sel += HexViewer.hexMaxHBytes;
                    HexViewer.InternalSelect(pos, sel);
                    HexViewer.ScrollByteIntoView();
                }
                else
                {
                    sel -= HexViewer.hexMaxHBytes;
                    if (sel < 0)
                    {
                        pos = startPositionInfo.Index + sel;
                        sel = -sel;
                        HexViewer.InternalSelect(pos, sel);
                        HexViewer.ScrollByteIntoView();
                    }
                    else
                    {
                        sel -= HexViewer.hexMaxHBytes;
                        HexViewer.InternalSelect(pos, sel);
                        HexViewer.ScrollByteIntoView(pos + sel);
                    }
                }

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftRight(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;

                if (pos + sel >= HexViewer._byteProvider.Length)
                    return true;

                if (startPositionInfo.Index <= pos)
                {
                    sel++;
                    HexViewer.InternalSelect(pos, sel);
                    HexViewer.ScrollByteIntoView(pos + sel);
                }
                else
                {
                    pos++;
                    sel = Math.Max(0, sel - 1);
                    HexViewer.InternalSelect(pos, sel);
                    HexViewer.ScrollByteIntoView();
                }

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftDown(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;
                var max = HexViewer._byteProvider.Length;

                if (pos + sel + HexViewer.hexMaxHBytes > max)
                    return true;

                if (startPositionInfo.Index <= pos)
                {
                    sel += HexViewer.hexMaxHBytes;
                    HexViewer.InternalSelect(pos, sel);
                    HexViewer.ScrollByteIntoView(pos + sel);
                }
                else
                {
                    sel -= HexViewer.hexMaxHBytes;
                    if (sel < 0)
                    {
                        pos = startPositionInfo.Index;
                        sel = -sel;
                    }
                    else
                    {
                        pos += HexViewer.hexMaxHBytes;
                        sel -= HexViewer.hexMaxHBytes;
                    }

                    HexViewer.InternalSelect(pos, sel);
                    HexViewer.ScrollByteIntoView();
                }

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Tab(ref Message m)
            {
                if (HexViewer._stringViewVisible && HexViewer.currentKeyInterpreter.GetType() == typeof(KeyInterpreter))
                {
                    HexViewer.ActivateStringKeyInterpreter();
                    HexViewer.ScrollByteIntoView();
                    HexViewer.ReleaseSelection();
                    HexViewer.UpdateCaret();
                    HexViewer.Invalidate();
                    return true;
                }

                if (HexViewer.Parent == null) return true;
                _ = HexViewer.Parent.SelectNextControl(HexViewer, true, true, true, true);
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftTab(ref Message m)
            {
                if (HexViewer.currentKeyInterpreter is StringKeyInterpreter)
                {
                    shiftDown = false;
                    HexViewer.ActivateKeyInterpreter();
                    HexViewer.ScrollByteIntoView();
                    HexViewer.ReleaseSelection();
                    HexViewer.UpdateCaret();
                    HexViewer.Invalidate();
                    return true;
                }

                if (HexViewer.Parent == null) return true;
                _ = HexViewer.Parent.SelectNextControl(HexViewer, false, true, true, true);
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Back(ref Message m)
            {
                if (!HexViewer._byteProvider.SupportsDeleteBytes)
                    return true;

                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;
                var cp = HexViewer.byteCharacterPosition;

                var startDelete = (cp == 0 && sel == 0) ? pos - 1 : pos;
                if (startDelete < 0 && sel < 1)
                    return true;

                var bytesToDelete = (sel > 0) ? sel : 1;
                HexViewer._byteProvider.DeleteBytes(Math.Max(0, startDelete), bytesToDelete);
                HexViewer.UpdateScrollSize();

                if (sel == 0)
                    _ = PerformPosMoveLeftByte();

                HexViewer.ReleaseSelection();
                HexViewer.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Delete(ref Message m)
            {
                if (!HexViewer._byteProvider.SupportsDeleteBytes)
                    return true;

                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;

                if (pos >= HexViewer._byteProvider.Length)
                    return true;

                var bytesToDelete = sel > 0 ? sel : 1;
                HexViewer._byteProvider.DeleteBytes(pos, bytesToDelete);

                HexViewer.UpdateScrollSize();
                HexViewer.ReleaseSelection();
                HexViewer.Invalidate();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_Home(ref Message m)
            {
                var pos = HexViewer.bytePosition;

                if (pos < 1)
                    return true;

                pos = 0;
                HexViewer.SetPosition(pos, 0);

                HexViewer.ScrollByteIntoView();
                HexViewer.UpdateCaret();
                HexViewer.ReleaseSelection();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_End(ref Message m)
            {
                var pos = HexViewer.bytePosition;
                if (pos >= HexViewer._byteProvider.Length - 1)
                    return true;

                pos = HexViewer._byteProvider.Length;
                HexViewer.SetPosition(pos, 0);

                HexViewer.ScrollByteIntoView();
                HexViewer.UpdateCaret();
                HexViewer.ReleaseSelection();

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ShiftShiftKey(ref Message m)
            {
                if (mouseDown)
                    return true;
                if (shiftDown)
                    return true;

                shiftDown = true;

                if (HexViewer._selectionLength > 0)
                    return true;

                startPositionInfo = new BytePositionInfo(HexViewer.bytePosition, HexViewer.byteCharacterPosition);

                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ControlC(ref Message m)
            {
                HexViewer.Copy();
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ControlX(ref Message m)
            {
                HexViewer.Cut();
                return true;
            }

            protected virtual bool PreProcessWmKeyDown_ControlV(ref Message m)
            {
                HexViewer.Paste();
                return true;
            }

            protected virtual bool PreProcessWmKeyUp_Insert(ref Message m)
            {
                HexViewer._insertActive = !HexViewer._insertActive;
                return true;
            }

            protected virtual bool PerformPosMoveLeft()
            {
                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;
                var cp = HexViewer.byteCharacterPosition;

                if (sel != 0)
                {
                    cp = 0;
                    HexViewer.SetPosition(pos, cp);
                    HexViewer.ReleaseSelection();
                }
                else
                {
                    if (pos == 0 && cp == 0)
                        return true;

                    if (cp > 0) cp--;
                    else
                    {
                        pos = Math.Max(0, pos - 1);
                        cp++;
                    }

                    HexViewer.SetPosition(pos, cp);

                    if (pos < HexViewer.startByte)
                        HexViewer.PerformScrollLineUp();

                    HexViewer.UpdateCaret();
                    HexViewer.Invalidate();
                }

                HexViewer.ScrollByteIntoView();
                return true;
            }

            protected virtual bool PerformPosMoveRight()
            {
                var pos = HexViewer.bytePosition;
                var cp = HexViewer.byteCharacterPosition;
                var sel = HexViewer._selectionLength;

                if (sel != 0)
                {
                    pos += sel;
                    cp = 0;
                    HexViewer.SetPosition(pos, cp);
                    HexViewer.ReleaseSelection();
                }
                else
                {
                    if (!(pos == HexViewer._byteProvider.Length && cp == 0))
                    {

                        if (cp > 0)
                        {
                            pos = Math.Min(HexViewer._byteProvider.Length, pos + 1);
                            cp = 0;
                        }
                        else cp++;

                        HexViewer.SetPosition(pos, cp);

                        if (pos > HexViewer.endByte - 1)
                            HexViewer.PerformScrollLineDown();

                        HexViewer.UpdateCaret();
                        HexViewer.Invalidate();
                    }
                }

                HexViewer.ScrollByteIntoView();
                return true;
            }

            protected virtual bool PerformPosMoveLeftByte()
            {
                var pos = HexViewer.bytePosition;
                if (pos == 0) return true;

                pos = Math.Max(0, pos - 1);
                HexViewer.SetPosition(pos, 0);

                if (pos < HexViewer.startByte)
                    HexViewer.PerformScrollLineUp();

                HexViewer.UpdateCaret();
                HexViewer.ScrollByteIntoView();
                HexViewer.Invalidate();

                return true;
            }

            protected virtual bool PerformPosMoveRightByte()
            {
                var pos = HexViewer.bytePosition;
                if (pos == HexViewer._byteProvider.Length) return true;

                pos = Math.Min(HexViewer._byteProvider.Length, pos + 1);
                HexViewer.SetPosition(pos, 0);

                if (pos > HexViewer.endByte - 1)
                    HexViewer.PerformScrollLineDown();

                HexViewer.UpdateCaret();
                HexViewer.ScrollByteIntoView();
                HexViewer.Invalidate();

                return true;
            }

            protected virtual BytePositionInfo GetBytePositionInfo(Point p) => HexViewer.GetHexBytePositionInfo(p);

            protected bool RaiseKeyDown(Keys keyData)
            {
                var e = new KeyEventArgs(keyData);
                HexViewer.OnKeyDown(e);
                return e.Handled;
            }

            protected bool RaiseKeyUp(Keys keyData)
            {
                var e = new KeyEventArgs(keyData);
                HexViewer.OnKeyUp(e);
                return e.Handled;
            }

            protected bool RaiseKeyPress(char keyChar)
            {
                var e = new KeyPressEventArgs(keyChar);
                HexViewer.OnKeyPress(e);
                return e.Handled;
            }

            private void BeginMouseSelection(object sender, MouseEventArgs e)
            {
                Debug.WriteLine("BeginMouseSelection()", "KeyInterpreter");
                mouseDown = true;

                if (!shiftDown)
                {
                    startPositionInfo = new BytePositionInfo(HexViewer.bytePosition, HexViewer.byteCharacterPosition);
                    HexViewer.ReleaseSelection();
                }
                else UpdateMouseSelection(this, e);
            }

            private void UpdateMouseSelection(object sender, MouseEventArgs e)
            {
                if (!mouseDown)
                    return;

                var currentPositionInfo = GetBytePositionInfo(new Point(e.X, e.Y));
                var selEnd = currentPositionInfo.Index;

                long realselStart;
                long realselLength;

                if (selEnd < startPositionInfo.Index)
                {
                    realselStart = selEnd;
                    realselLength = startPositionInfo.Index - selEnd;
                }
                else if (selEnd > startPositionInfo.Index)
                {
                    realselStart = startPositionInfo.Index;
                    realselLength = selEnd - realselStart;
                }
                else
                {
                    realselStart = HexViewer.bytePosition;
                    realselLength = 0;
                }

                if (realselStart != HexViewer.bytePosition || realselLength != HexViewer._selectionLength)
                    HexViewer.InternalSelect(realselStart, realselLength);
            }

            private void EndMouseSelection(object sender, MouseEventArgs e) => mouseDown = false;
        }

        /// <summary>
        /// Represents an empty input handler without any functionality. 
        /// If the current ByteProvider to null, then this interpreter is used.
        /// </summary>
        private sealed class EmptyKeyInterpreter : IKeyInterpreter
        {
            private readonly HexViewer hexViewer;

            public EmptyKeyInterpreter(HexViewer owner) =>
                hexViewer = owner ?? throw new ArgumentNullException(nameof(owner));

            public void ActivateMouseEvents() { /* Nothing to do in this implementation */ }
            public void DeactivateMouseEvents() { /* Nothing to do in this implementation */ }

            public bool PreProcessWmKeyUp(ref Message m) => hexViewer.BasePreProcessMessage(ref m);
            public bool PreProcessWmChar(ref Message m) => hexViewer.BasePreProcessMessage(ref m);
            public bool PreProcessWmKeyDown(ref Message m) => hexViewer.BasePreProcessMessage(ref m);
            public PointF GetCaretPointF(long byteIndex) => new();
        }

        /// <summary>
        /// Handles user input such as mouse and keyboard input during string view edit
        /// </summary>
        private sealed class StringKeyInterpreter : KeyInterpreter
        {
            public StringKeyInterpreter(HexViewer owner) : base(owner) => HexViewer.byteCharacterPosition = 0;

            public override bool PreProcessWmKeyDown(ref Message m)
            {
                var vc = (Keys)m.WParam.ToInt32();
                var keyData = vc | ModifierKeys;

                switch (keyData)
                {
                    case Keys.Tab | Keys.Shift:
                    case Keys.Tab:
                        if (RaiseKeyDown(keyData))
                            return true;
                        break;
                }

                return keyData switch
                {
                    Keys.Tab | Keys.Shift => PreProcessWmKeyDown_ShiftTab(ref m),
                    Keys.Tab => PreProcessWmKeyDown_Tab(ref m),
                    _ => base.PreProcessWmKeyDown(ref m),
                };
            }

            public override bool PreProcessWmChar(ref Message m)
            {
                if (ModifierKeys == Keys.Control)
                    return HexViewer.BasePreProcessMessage(ref m);

                var sw = HexViewer._byteProvider.SupportsWriteByte;
                var si = HexViewer._byteProvider.SupportsInsertBytes;
                var sd = HexViewer._byteProvider.SupportsDeleteBytes;

                var pos = HexViewer.bytePosition;
                var sel = HexViewer._selectionLength;

                if (!sw && pos != HexViewer._byteProvider.Length ||
                    !si && pos == HexViewer._byteProvider.Length)
                    return HexViewer.BasePreProcessMessage(ref m);

                var c = (char)m.WParam.ToInt32();

                if (RaiseKeyPress(c)) return true;
                if (HexViewer.ReadOnly) return true;

                var isInsertMode = pos == HexViewer._byteProvider.Length;

                // do insert when insertActive = true
                if (!isInsertMode && si && HexViewer._insertActive)
                    isInsertMode = true;

                if (sd && si && sel > 0)
                {
                    HexViewer._byteProvider.DeleteBytes(pos, sel);
                    isInsertMode = true;
                    HexViewer.SetPosition(pos, 0);
                }

                HexViewer.ReleaseSelection();

                if (isInsertMode)
                    HexViewer._byteProvider.InsertBytes(pos, new byte[] { (byte)c });
                else HexViewer._byteProvider.WriteByte(pos, (byte)c);

                _ = PerformPosMoveRightByte();
                HexViewer.Invalidate();

                return true;
            }

            public override PointF GetCaretPointF(long byteIndex)
            {
                Debug.WriteLine("GetCaretPointF()", "StringKeyInterpreter");

                var gp = HexViewer.GetGridBytePoint(byteIndex);
                return HexViewer.GetByteStringPointF(gp);
            }

            protected override bool PreProcessWmKeyDown_Left(ref Message m) => PerformPosMoveLeftByte();

            protected override bool PreProcessWmKeyDown_Right(ref Message m) => PerformPosMoveRightByte();

            protected override BytePositionInfo GetBytePositionInfo(Point p) => HexViewer.GetStringBytePositionInfo(p);
        }
    }
}
