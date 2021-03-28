using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Delta.CertXplorer.Internals
{
    internal static class NativeMethods
    {
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_NCACTIVATE = 0x0086;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;

        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_INTERNALPAINT = 0x0002;
        public const int RDW_ERASE = 0x0004;
        public const int RDW_VALIDATE = 0x0008;
        public const int RDW_NOINTERNALPAINT = 0x0010;
        public const int RDW_NOERASE = 0x0020;
        public const int RDW_NOCHILDREN = 0x0040;
        public const int RDW_ALLCHILDREN = 0x0080;
        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_ERASENOW = 0x0200;
        public const int RDW_FRAME = 0x0400;
        public const int RDW_NOFRAME = 0x0800;

        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Windows API Interop")]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        // Similar to RECT, but because this is a class, we can pass null to a method using it.
        [StructLayout(LayoutKind.Sequential)]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Windows API Interop")]
        public class COMRECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public COMRECT() { }
            public COMRECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        // Caret definitions
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyCaret();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool RedrawWindow(IntPtr hwnd, COMRECT rcUpdate, IntPtr hrgnUpdate, int flags);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);
    }
}
