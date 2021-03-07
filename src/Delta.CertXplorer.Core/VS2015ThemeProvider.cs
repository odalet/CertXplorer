using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer
{
    public static class VS2015ThemeProvider
    {
        private sealed class ThemeProxy : VS2015BlueTheme
        {
            public ToolStripRenderer ToolStripRendererProxy => ToolStripRenderer;
        }

        private static readonly ThemeProxy proxy = new ThemeProxy();

        public static ToolStripRenderer Renderer => proxy.ToolStripRendererProxy;
    }
}
