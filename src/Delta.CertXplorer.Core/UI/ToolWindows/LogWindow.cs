using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.UI.ToolWindows
{
    public partial class LogWindow : ToolWindow
    {
        private readonly LogWindowControl logControl;

        public LogWindow()
        {
            InitializeComponent();

            logControl = new LogWindowControl
            {
                Dock = DockStyle.Fill,
                Name = "logControl",
                TabIndex = 0
            };

            Controls.Add(logControl);

            Icon = Properties.Resources.LogIcon;
            TabText = base.Text = SR.Log;
            ToolTipText = SR.LogWindow;

            logControl.LinkClicked += (s, e) => HandleUrl(e.LinkText);
        }

        public override Guid Guid => new Guid("{2A823517-2135-4417-9B82-3A43EDBE4532}");

        protected override DockState DefaultDockState => DockState.DockBottom;

        private void HandleUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            if (url.StartsWith("file://")) // Check existence
            {
                var path = url.Substring(7);
                if (string.IsNullOrEmpty(path)) return;
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    _ = ErrorBox.Show(this, string.Format(SR.PathNotFound, path));
                    return;
                }
            }

            try 
            {
                _ = Process.Start(url); 
            }
            catch (Exception ex)
            {
                _ = ErrorBox.Show(this, string.Format(SR.CantHandleUrl, url, ex.ToFormattedString()));
            }
        }
    }
}
