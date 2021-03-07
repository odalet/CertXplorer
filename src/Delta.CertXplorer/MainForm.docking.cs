using System;
using System.Linq;
using System.Windows.Forms;
using Delta.CertXplorer.Asn1Decoder;
using Delta.CertXplorer.CertManager;
using Delta.CertXplorer.UI;
using Delta.CertXplorer.UI.ToolWindows;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer
{
    partial class MainForm
    {
        private void CreateToolWindows()
        {
            void register<T>() where T : ToolWindow, new() =>
                RegisterToolWindow(new ToolWindowInfo<T> { ToolWindow = new T() });

            register<LogWindow>();
            register<CertificateStoreWindow>();
            register<DocumentManagerWindow>();
            register<PropertiesWindow>();
            register<CertificateListWindow>();
        }

        private void RegisterToolWindow(ToolWindowInfo windowInfo)
        {
            CheckToolWindow(windowInfo);

            if (LayoutService != null)
                LayoutService.RegisterToolWindow(FormId, windowInfo.ToolWindow);
            toolWindowInfos.Add(windowInfo.ToolWindow.Guid, windowInfo);

            OnToolWindowInfoRegistered(windowInfo);
        }

        private void CreateToolWindowMenus()
        {
            foreach (var info in toolWindowInfos.Values)
                CreateToolWindowMenu(info);
        }

        private void CreateToolWindowMenu(ToolWindowInfo windowInfo)
        {
            if (windowInfo.ToolWindow == null) return;

            var menu = new ToolStripMenuItem(
                windowInfo.MenuText,
                windowInfo.MenuImage);

            windowInfo.ToolWindow.DockStateChanged += (s, e) =>
                menu.Checked = windowInfo.ToolWindow.DockState != DockState.Hidden;
            menu.Checked = windowInfo.ToolWindow.DockState != DockState.Hidden;

            menu.Visible = windowInfo.IsEnabled;
            menu.Tag = windowInfo;

            windowInfo.EnabledChanged += (_, _) =>
            {
                menu.Visible = windowInfo.IsEnabled;
                UpdateViewMenu();
            };

            menu.Click += (_, _) => ShowToolWindow(windowInfo);

            _ = viewToolStripMenuItem.DropDownItems.Add(menu);
            UpdateViewMenu();
        }

        private void UpdateViewMenu() => viewToolStripMenuItem.Visible = viewToolStripMenuItem
            .DropDownItems
            .Cast<ToolStripMenuItem>()
            .Any(item => item.Tag is ToolWindowInfo windowInfo && windowInfo.IsEnabled);

        private void InitializeDocking()
        {
            var restoreSucceeded = RestoreDockingState();
            foreach (var info in toolWindowInfos.Values)
            {
                if (IsToolWindowEnabled(info))
                    ShowToolWindow(info, !restoreSucceeded);
                else
                    HideToolWindow(info);
            }
        }

        private bool RestoreDockingState() => LayoutService != null && LayoutService.RestoreDockingState(FormId);

        private bool IsToolWindowEnabled(ToolWindowInfo windowInfo) => windowInfo != null && windowInfo.IsEnabled;

        private void ShowToolWindow(ToolWindowInfo windowInfo, bool dockDefault = false)
        {
            CheckToolWindow(windowInfo);
            windowInfo.ToolWindow.Show(workspace);
            if (dockDefault) windowInfo.ToolWindow.DockDefault();
        }

        private void HideToolWindow(ToolWindowInfo windowInfo)
        {
            CheckToolWindow(windowInfo);
            windowInfo.ToolWindow.Hide();
        }

        private void CheckToolWindow(ToolWindowInfo windowInfo)
        {
            if (windowInfo == null) throw new ArgumentNullException(nameof(windowInfo));
            if (windowInfo.ToolWindow == null) throw new ArgumentException(
                $"{nameof(windowInfo)}.{nameof(windowInfo.ToolWindow)} cannot be null", nameof(windowInfo));
        }
    }
}
