using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Delta.CertXplorer.UI
{
    partial class ListViewEx
    {
        private const int WM_PAINT = 0x000F;

        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETHEADER = LVM_FIRST + 31;

        private const int HDM_FIRST = 0x1200;
        private const int HDM_SETIMAGELIST = HDM_FIRST + 8;
        private const int HDM_GETIMAGELIST = HDM_FIRST + 9;
        private const int HDM_GETITEM = HDM_FIRST + 11;
        private const int HDM_SETITEM = HDM_FIRST + 12;

        private const int HDI_WIDTH = 0x0001;
        private const int HDI_HEIGHT = HDI_WIDTH;
        private const int HDI_TEXT = 0x0002;
        private const int HDI_FORMAT = 0x0004;
        private const int HDI_LPARAM = 0x0008;
        private const int HDI_BITMAP = 0x0010;
        private const int HDI_IMAGE = 0x0020;
        private const int HDI_DI_SETITEM = 0x0040;
        private const int HDI_ORDER = 0x0080;
        private const int HDI_FILTER = 0x0100;

        private const int HDF_LEFT = 0x0000;
        private const int HDF_RIGHT = 0x0001;
        private const int HDF_CENTER = 0x0002;
        private const int HDF_JUSTIFYMASK = 0x0003;
        private const int HDF_RTLREADING = 0x0004;
        private const int HDF_OWNERDRAW = 0x8000;
        private const int HDF_STRING = 0x4000;
        private const int HDF_BITMAP = 0x2000;
        private const int HDF_BITMAP_ON_RIGHT = 0x1000;
        private const int HDF_IMAGE = 0x0800;
        private const int HDF_SORTUP = 0x0400;
        private const int HDF_SORTDOWN = 0x0200;

        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Win32 Interop")]
        [StructLayout(LayoutKind.Sequential)]
        private struct LVCOLUMN
        {
            public int mask;
            public int cx;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public int fmt;
            public int iSubItem;
            public int iImage;
            public int iOrder;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessageLVCOLUMN(IntPtr hwnd, int msg, IntPtr wparam, ref LVCOLUMN lpLVCOLUMN);
    }
}
