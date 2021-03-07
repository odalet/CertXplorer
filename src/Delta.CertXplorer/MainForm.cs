using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Delta.CertXplorer.ApplicationModel;
using Delta.CertXplorer.Commanding;
using Delta.CertXplorer.DocumentModel;
using Delta.CertXplorer.UI;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer
{
    public partial class MainForm : Form, IDocumentBasedUI
    {
        private readonly Dictionary<Guid, ToolWindowInfo> toolWindowInfos = new Dictionary<Guid, ToolWindowInfo>();
        private IDocumentView activeDocumentView;
        private bool toolstripPanelsFixed = false;

        public MainForm()
        {
            InitializeComponent();

            FilesToOpenAtStartup = new List<string>();
            Text = Program.ApplicationName;

            CreatePluginsMenu();

            ApplyVS2015Theme();
        }

        public event EventHandler ActiveDocumentChanged;

        public IDocumentView ActiveDocumentView
        {
            get => activeDocumentView;
            private set
            {
                if (activeDocumentView == value) return;
                activeDocumentView = value;
                ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal List<string> FilesToOpenAtStartup { get; }
        private string FormId => "Delta.CertXplorer.Chrome";
        private ILayoutService LayoutService { get; set; }

        public void ShowView(IDocumentView view)
        {
            if (view is not DockContent dockContent)
                throw new InvalidCastException($"Provided view must inherit {nameof(DockContent)}");
            dockContent.Show(workspace);
            ActiveDocumentView = view;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (!toolstripPanelsFixed)
            {
                FixToolStripPanelsChildIndex();
                toolstripPanelsFixed = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (DesignMode) return;

            LayoutService = This.GetService<ILayoutService>();
            workspace.ActiveDocumentChanged += (s, ev) => OnActiveDocumentChanged();

            // registers this form for layout serialization
            if (LayoutService != null)
                LayoutService.RegisterForm(FormId, this, workspace);

            base.OnLoad(e);

            InitializeStatusStrip();
            CreateToolWindows();

            // It is very important the DockPanel be added to its parent before adding tool windows,
            // otherwise, floating windows might be hidden by the parent form!
            workspace.ShowDocumentIcon = true;
            workspace.ActiveAutoHideContent = null;
            workspace.Dock = DockStyle.Fill;
            workspace.Name = "workspace";

            InitializeDocking();
            CreateToolWindowMenus();

            InitializeActions();

            // debug
            This.Logger.Verbose($"Application root directory is: {AppDomain.CurrentDomain.BaseDirectory}");

            var layoutService = This.GetService<ILayoutService>();
            if (layoutService != null)
            {
                This.Logger.Verbose("Layout service was found.");
                var layoutConfigFile = Globals.LayoutSettingsFileName;
                This.Logger.Verbose($"Layout service configuration file is: {layoutConfigFile}");
                if (!File.Exists(layoutConfigFile)) This.Logger.Warning("Layout service configuration does not exist.");
            }

            // Now open files that were passed on the command lin
            if (FilesToOpenAtStartup.Count == 0) return;

            foreach (var filename in FilesToOpenAtStartup)
            {
                var fname = Path.IsPathRooted(filename) ? filename : Path.Combine(Environment.CurrentDirectory, filename);
                Commands.RunVerb(Verbs.OpenFile, fname);
            }
        }

        private void OnActiveDocumentChanged() { /* Placeholder */ }
        private void InitializeStatusStrip() { /* Placeholder */ }
        private void OnToolWindowInfoRegistered(ToolWindowInfo windowInfo) { /* Placeholder */ }

        private void InitializeActions()
        {
            exitAction.Run += (_, _) => Close();
            aboutAction.Run += (_, _) =>
            {
                var service = This.GetService<IAboutService>();
                _ = service == null ?
                    InformationBox.Show(this, $"About Delta.CertXplorer Version {ThisAssembly.Version}") :
                    service.ShowAboutDialog(this);
            };

            openFileDocumentAction.Run += (_, _) => Commands.RunVerb(Verbs.OpenFile);
            openCertificateDocumentAction.Run += (_, _) => Commands.RunVerb(Verbs.OpenCertificate);

            actionsManager.SetAction(openFileToolStripButton, openFileDocumentAction);
            actionsManager.SetAction(openFileToolStripMenuItem, openFileDocumentAction);
            actionsManager.SetAction(openCertificateToolStripButton, openCertificateDocumentAction);
            actionsManager.SetAction(openCertificateToolStripMenuItem, openCertificateDocumentAction);
            actionsManager.SetAction(aboutToolStripButton, aboutAction);
            actionsManager.SetAction(aboutToolStripMenuItem, aboutAction);
            actionsManager.SetAction(exitToolStripMenuItem, exitAction);
        }

        private void CreatePluginsMenu()
        {
            if (!Globals.PluginsManager.GlobalPlugins.Any()) return;

            // Create the menu
            var pluginsMenuItem = new ToolStripMenuItem("&Plugins");
            menuStrip.Items.Insert(2, pluginsMenuItem);

            foreach (var plugin in Globals.PluginsManager.GlobalPlugins)
            {
                var menuItem = new ToolStripMenuItem(plugin.PluginInfo.Name);
                var icon = plugin.GetIcon();
                if (icon != null) menuItem.Image = icon;
                var pluginToRun = plugin;
                menuItem.Click += (_, _) =>
                {
                    Globals.PluginsManager.Run(pluginToRun, this, out var shouldDisablePlugin);
                    if (shouldDisablePlugin)
                    {
                        This.Logger.Error("This plugin is failing. It has been deactivated.");
                        menuItem.Enabled = false;
                    }
                };

                _ = pluginsMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void FixToolStripPanelsChildIndex()
        {
            /*
             * This fixes the appearance of the vertical toolstrip panels:
             * If we don't do run the code below, the toolstrip panels may appear
             * layed out like the figure on the left, and we want them like 
             * the figure on the right:
             *
             * |-------------|            ---------------
             * |             |            |             |
             * |             |            |             |
             * |             |            |             |
             * ---------------            ---------------
             * 
             * To achieve this, we reorder the child index so that the vertical panels
             * are created first. This ensures that the latter (the horizontal panels) 
             * will expand to their full width.
             * 
             */

            var index = 0;
            var controls = new Control[]
            {
                workspace,
                rightToolStripPanel,
                leftToolStripPanel,
                topToolStripPanel,
                bottomToolStripPanel,
            };

            while (index < controls.Length)
            {
                Controls.SetChildIndex(controls[index], index);
                index++;
            }
        }

        private void ApplyVS2015Theme()
        {
            var vs2015ToolStripBackground = Color.FromArgb(214, 219, 233);
            topToolStripPanel.BackColor = vs2015ToolStripBackground;
            bottomToolStripPanel.BackColor = vs2015ToolStripBackground;
            leftToolStripPanel.BackColor = vs2015ToolStripBackground;
            rightToolStripPanel.BackColor = vs2015ToolStripBackground;

            vsThemeToolSTripExtender.SetStyle(menuStrip, VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015BlueTheme);
            vsThemeToolSTripExtender.SetStyle(toolStrip, VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015BlueTheme);
            vsThemeToolSTripExtender.SetStyle(statusStrip, VisualStudioToolStripExtender.VsVersion.Vs2015, vS2015BlueTheme);
        }
    }
}
