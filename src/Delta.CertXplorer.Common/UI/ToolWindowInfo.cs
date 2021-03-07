using System;
using System.Drawing;
using Delta.CertXplorer.UI.ToolWindows;

namespace Delta.CertXplorer.UI
{
    public interface IToolWindowInfo
    {
        event EventHandler EnabledChanged;
        ToolWindow ToolWindow { get; }
        bool IsEnabled { get; set; }
        string MenuText { get; }
        Image MenuImage { get; }
    }

    public sealed class ToolWindowInfo<T> : IToolWindowInfo where T : ToolWindow
    {
        public ToolWindowInfo(T toolWindow)
        {
            ToolWindow = toolWindow;
            IsEnabled = true;
        }

        public event EventHandler EnabledChanged;

        public ToolWindow ToolWindow { get; }

        private bool enabled;
        public bool IsEnabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                OnEnabledChanged();
            }
        }

        public string MenuText => ToolWindow == null ? string.Empty : ToolWindow.ToolTipText;

        public Image MenuImage
        {
            get
            {
                if (ToolWindow == null) return null;

                var icon = ToolWindow.Icon;
                return icon == null ? null : (Image)icon.ToBitmap();
            }
        }

        private void OnEnabledChanged() => EnabledChanged?.Invoke(this, EventArgs.Empty);
    }
}
