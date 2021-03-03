using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Delta.CertXplorer.Internals
{
    /// <summary>
    /// Classes d'utilitaires permettant la manipulation de fenêtres Windows natives.
    /// </summary>
    internal static partial class NativeMethods
    {
        public const uint WM_WINDOWPOSCHANGING = 0x0046;

        public const uint WM_NCACTIVATE = 0x0086;

        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_RBUTTONDOWN = 0x0204;
        public const uint WM_PARENTNOTIFY = 0x0210;

        public const uint WM_USER = 0x400;
        public const uint EM_FORMATRANGE = WM_USER + 57;

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

        public const uint SRCCOPY = 0x00CC0020;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -16;

        public const int PBS_VERTICAL = 4;
        public const int PBM_SETPOS = 0x0402;

        public const int NULL_STYLE = 0;

        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
 
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr DeleteObject(IntPtr hObject);
        
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, uint msg, UIntPtr wparam, IntPtr lparam);

        [DllImport("User32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short VkKeyScan(char key);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool RedrawWindow(IntPtr hwnd, COMRECT rcUpdate, IntPtr hrgnUpdate, int flags);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern void RtlMoveMemory([In, MarshalAs(UnmanagedType.I4)]int hpvDest, [In, Out]byte[] hpvSource, int cbCopy);


        public static IWin32Window ActiveWindow => new Win32Window(GetActiveWindow());

        public static int GetWindowStyle(IntPtr hwnd) => GetWindowLong(hwnd, GWL_STYLE);
        public static int GetWindowStyleEx(IntPtr hwnd) => GetWindowLong(hwnd, GWL_EXSTYLE);
        public static void SetWindowStyle(IntPtr hwnd, int styles) => SetWindowLong(hwnd, GWL_STYLE, styles);
        public static void SetWindowStyleEx(IntPtr hwnd, int styles) => SetWindowLong(hwnd, GWL_EXSTYLE, styles);

        public static void ModifyWindowStyle(IntPtr hwnd, int stylesToAdd, int stylesToRemove) 
        {
            var newStyle = GetWindowStyle(hwnd);
            newStyle &= ~stylesToRemove;
            newStyle |= stylesToAdd;            
            
            SetWindowStyle(hwnd, newStyle); 
        }

        public static void ModifyWindowStyleEx(IntPtr hwnd, int stylesToAdd, int stylesToRemove)
        {
            var newStyle = GetWindowStyleEx(hwnd);
            newStyle &= ~stylesToRemove;
            newStyle |= stylesToAdd;

            SetWindowStyleEx(hwnd, newStyle);
        }
    }
}
