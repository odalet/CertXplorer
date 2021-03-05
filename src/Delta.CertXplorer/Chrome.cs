using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Delta.CertXplorer.ApplicationModel;
using Delta.CertXplorer.Asn1Decoder;
using Delta.CertXplorer.CertManager;
using Delta.CertXplorer.Commanding;
using Delta.CertXplorer.UI;
using Delta.CertXplorer.UI.ToolWindows;

namespace Delta.CertXplorer
{
    public partial class Chrome : BaseChrome 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Chrome"/> class.
        /// </summary>
        public Chrome()
        {
            InitializeComponent();
            
            FilesToOpenAtStartup = new List<string>();
            Text = Program.ApplicationName;
            MenuStrip.MdiWindowListItem = windowMenuItem;
            ViewMenuItem = viewMenuItem;

            CreatePluginsMenu();
        }

        internal List<string> FilesToOpenAtStartup { get; }
        protected override string FormId => "Delta.CertXplorer.Chrome";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
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

        protected override void CreateToolWindows()
        {
            void register<T>() where T : ToolWindow, new() => 
                base.RegisterToolWindow(new ToolWindowInfo<T> { ToolWindow = new T() });

            register<LogWindow>();
            register<CertificateStoreWindow>();
            register<DocumentManagerWindow>();
            register<PropertiesWindow>();
            register<CertificateListWindow>();
        }

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
        }

        private void CreatePluginsMenu()
        {
            if (!Globals.PluginsManager.GlobalPlugins.Any()) return;

            // Create the menu
            var pluginsMenuItem = new ToolStripMenuItem("&Plugins");
            MenuStrip.Items.Insert(3, pluginsMenuItem);

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
    }
}

