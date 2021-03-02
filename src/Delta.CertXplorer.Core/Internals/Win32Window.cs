using System;
using System.Windows.Forms;

namespace Delta.CertXplorer.Internals
{
    internal sealed class Win32Window : IWin32Window
    {
        public Win32Window(IntPtr hwnd) => Handle = hwnd;

        public IntPtr Handle { get; }
    }
}
