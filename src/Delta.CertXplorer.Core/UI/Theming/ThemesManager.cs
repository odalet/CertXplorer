using System;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI.Theming
{
    public static class ThemesManager
    {
        public static ProfessionalColorTable DefaultColorTable { get; } = new ProfessionalColorTable() { UseSystemColors = true };

        public static void RegisterThemeAwareControl(Control control, Action<ToolStripRenderer> onThemeChanged)
        {
            if (control == null || control.IsDisposed)
                return;

            ToolStripManager.RendererChanged += (s, e) => UpdateRenderer(control, onThemeChanged);
            UpdateRenderer(control, onThemeChanged);
        }

        public static ToolStripRenderer CloneCurrentRenderer()
        {
            ToolStripRenderer renderer = null;

            var service = This.GetService<IThemingService>();
            if (service != null) renderer = service.CreateThemeRenderer(service.Current);

            if (renderer == null) renderer = 
                    ToolStripManager.Renderer is ToolStripProfessionalRenderer tspRenderer ?
                    new ToolStripProfessionalRenderer(tspRenderer.ColorTable) :
                    new ToolStripProfessionalRenderer();

            return renderer;
        }

        public static ProfessionalColorTable GetCurrentColorTable()
        {
            ToolStripRenderer renderer = null;

            var service = This.GetService<IThemingService>();
            if (service != null) renderer = service.CreateThemeRenderer(service.Current);

            if (renderer == null) renderer = ToolStripManager.Renderer;

            return renderer is ToolStripProfessionalRenderer tspRenderer ?
                tspRenderer.ColorTable : 
                DefaultColorTable;
        }

        private static void UpdateRenderer(Control control, Action<ToolStripRenderer> action)
        {
            if (control == null || control.IsDisposed)
                return;

            var clone = CloneCurrentRenderer();
            action?.Invoke(clone);
        }
    }
}
