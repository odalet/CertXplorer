using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Delta.CapiNet.Logging;
using Delta.CertXplorer.About;
using Delta.CertXplorer.ApplicationModel;
using Delta.CertXplorer.Config;
using Delta.CertXplorer.Configuration;
using Delta.CertXplorer.DocumentModel;
using Delta.CertXplorer.Logging;
using Delta.CertXplorer.PluginsManagement;
using Delta.CertXplorer.UI.Theming;

namespace Delta.CertXplorer
{
    /// <summary>
    /// The Application entry point.
    /// </summary>
    internal sealed class Program : BaseWindowsFormsApplication
    {
        private sealed class CapiNetLogServiceWrapper : CapiNetLogger.ILogService
        {
            private readonly ILogService originalLogService;

            public CapiNetLogServiceWrapper(Type type, ILogService logService)
            {
                Type = type;
                originalLogService = logService;
            }

            public Type Type { get; }

            public void Log(string level, string message, Exception exception)
            {
                try
                {
                    var levels = Enums<LogLevel>.ValuesByName;                    
                    var found = levels.FirstOrDefault(kvp => string.Compare(kvp.Key, level, true) == 0);
                    var l = found.Key == null ? LogLevel.Info : found.Value;
                    // multi-sources does not work well... Don't play with it
                    originalLogService.Log(l, message, exception /*, Type == null ? null : Type.ToString()*/);
                }
                catch 
                {
                    // Nothing to do here
                }
            }
        }

        public static readonly string ApplicationName = "CertXplorer";

        [STAThread]
        private static void Main(string[] arguments)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Resolve Configuration files
            Globals.ApplicationSettingsFileName = ResolveConfigFile(Properties.Settings.Default.AppSettingsFileName);
            Globals.LayoutSettingsFileName = ResolveConfigFile(Properties.Settings.Default.LayoutSettingsFileName);
            Globals.LoggingSettingsFileName = ResolveConfigFile(Properties.Settings.Default.LoggingSettingsFileName);

            new Program
            {
                ApplicationSettingsFileName = Globals.ApplicationSettingsFileName,
                LayoutSettingsFileName = Globals.LayoutSettingsFileName,
                LoggingSettingsFileName = Globals.LoggingSettingsFileName
            }.Run(arguments);
        }

        protected override void AddOtherServices()
        {
            base.AddOtherServices();
            This.AddService<IAboutService>(new AboutCertXplorerService());            
            This.AddService(DocumentFactory.CreateDocumentHandlerRegistryService());
        }

        protected override ILogService CreateLogService()
        {
            var logService = base.CreateLogService();

            // Let's bind this to CapiNet and dependant libraries
            CapiNetLogger.LogServiceBuilder = t => new CapiNetLogServiceWrapper(t, logService);

            return logService;
        }

        protected override Form CreateMainForm()
        {
            var chrome = new MainForm();
            Globals.MainForm = chrome;

            const bool doApplyTheme = false; // Testing VS2015 theming

            // Set the theme
            if (doApplyTheme)
            {
                var themingService = This.GetService<IThemingService>();
                if (themingService != null)
                {
                    const string themeId = "Delta.CertXplorer";
                    if (themingService.ContainsTheme(themeId))
                        themingService.ApplyTheme(themeId);
                }
            }

            This.AddService(DocumentFactory.CreateDocumentManagerService(chrome));

            // Load document builders & view builders
            var registry = This.GetService<IDocumentHandlerRegistryService>(true);
            registry.Register(() => new Asn1DocumentHandler());
            This.Logger.Verbose("Registered ASN.1 Document Handler");

            foreach (var plugin in Globals.PluginsManager.DataHandlerPlugins)
            {
                if (!Globals.PluginsManager.Initialize(plugin))
                {
                    This.Logger.Error("Plugin initialization failed. Disabling it.");
                    continue;
                }

                var documentHandler = new PluginBasedDocumentHandler(plugin);
                registry.RegisterHandlerPlugin(documentHandler);
                This.Logger.Verbose(string.Format("Registered {0} Document Handler", plugin.PluginInfo.Name));
            }

            // Now tell the form what files it should open when launched.            
            if (CommandLineArguments != null && CommandLineArguments.Length > 0)
                chrome.FilesToOpenAtStartup.AddRange(CommandLineArguments); 

            return chrome;
        }

        protected override bool OnBeforeCreateMainForm()
        {
            // We plug the mef composition here.
            var pluginsManager = new PluginsManager(
                Properties.Settings.Default.PluginsDirectories.Cast<string>());
            Globals.PluginsManager = pluginsManager;
            pluginsManager.Compose();

            return base.OnBeforeCreateMainForm();
        }

        protected override void LoadApplicationSettings(ISettingsService settingsService)
        {
            base.LoadApplicationSettings(settingsService);

            // The app config file may define the application culture
            var store = settingsService.GetApplicationSettingsStore();
            if (store == null || !store.ContainsKey("culture")) return;
            var cultureName = store["culture"];
            if (!string.IsNullOrEmpty(cultureName))
            {
                try
                {
                    ApplicationCulture = new CultureInfo(cultureName);
                }
                catch 
                {
                    // Do nothing...
                }
            }
        }

        private static string ResolveConfigFile(string file, bool forceFileInitialization = false)
        {
            if (string.IsNullOrEmpty(file)) return string.Empty;

            var filename = Path.GetFileName(file);
            var userRoot = PathHelper.UserConfigDirectory;
            var userFile = Path.Combine(userRoot, filename);

            if (!File.Exists(userFile) || forceFileInitialization)
            {
                // Let's see if we don't have a template file in our resources
                var bytes = ConfigResources.Read(filename);
                if (bytes != null && bytes.Length > 0) 
                    File.WriteAllBytes(userFile, bytes);
            }

            return userFile;
        }
    }
}
