using System.Windows.Forms;
using Delta.CertXplorer.ApplicationModel.Services;
using Delta.CertXplorer.Configuration;
using Delta.CertXplorer.Diagnostics;
using Delta.CertXplorer.Logging;
using Delta.CertXplorer.Logging.log4net;
using Delta.CertXplorer.UI;
using Delta.CertXplorer.UI.Theming;

namespace Delta.CertXplorer.ApplicationModel
{
    public abstract class BaseWindowsFormsApplication : BaseApplication
    {
        private sealed class ThemingSettings
        {
            public string Theme { get; set; }
        }

        private const string defaultLoggingSettingsFileName = "log4net.config";
        private const string defaultLayoutSettingsFileName = "app.layout.xml";

        private string loggingSettingsFileName = defaultLoggingSettingsFileName;
        private string layoutSettingsFileName = defaultLayoutSettingsFileName;

        protected BaseWindowsFormsApplication() => IsSingleInstance = false;

        protected bool IsSingleInstance { get; set; }

        protected string LayoutSettingsFileName
        {
            get => BuildPathRootedFileName(layoutSettingsFileName, defaultLayoutSettingsFileName);
            set => layoutSettingsFileName = value;
        }

        protected string LoggingSettingsFileName
        {
            get => BuildPathRootedFileName(loggingSettingsFileName, defaultLoggingSettingsFileName);
            set => loggingSettingsFileName = value;
        }

        protected string[] CommandLineArguments { get; private set; }

        protected virtual void Run(string[] arguments)
        {
            CommandLineArguments = arguments;
            InitializeThisApplication();

            _ = AddService(CreateLogService());
            _ = AddService(CreateExceptionHandlerService());

            var settingsService = CreateSettingsService();
            _ = AddService(settingsService);
            LoadApplicationSettings(settingsService);

            _ = AddService(CreateLayoutService());
            
            AddThemingService();
            AddOtherServices();
            if (OnBeforeCreateMainForm()) ShowMainForm();
        }

        protected override void InitializeThisApplication() => InitializeThisApplication(ApplicationCulture, IsSingleInstance);

        protected virtual void InitializeThisApplication(string culture, bool singleInstance)
        {
            This.InitializeWindowsFormsApplication(culture, singleInstance);
            ToolStripManager.Renderer = new BaseToolStripRenderer();
        }

        protected override IExceptionHandlerService CreateExceptionHandlerService() => new ExceptionHandlerService();

        protected virtual ILayoutService CreateLayoutService() => new LayoutService(LayoutSettingsFileName);

        protected override ILogService CreateLogService() => Log4NetServiceFactory.CreateService(LoggingSettingsFileName);

        protected override ISettingsService CreateSettingsService() => new SettingsService();

        protected virtual IThemingService CreateThemingService() => new ThemingService();

        protected virtual void AddThemingService()
        {
            var service = CreateThemingService();
            if (service != null)
            {
                _ = AddService(service);

                // Try to read the default theme from the application settings file.
                var settingsService = This.GetService<ISettingsService>(true);
                var theme = settingsService.GetApplicationSettingsStore<ThemingSettings>().Theme;
                if (!string.IsNullOrEmpty(theme) && service.ContainsTheme(theme))
                    service.ApplyTheme(theme);
            }
        }

        protected abstract Form CreateMainForm();

        protected virtual void ShowMainForm()
        {
            var form = CreateMainForm();
            OnBeforeShowMainForm(form);
            This.Application.Run(form);
        }

        protected virtual bool OnBeforeCreateMainForm() => true; // True: continue loading

        protected virtual void OnBeforeShowMainForm(Form form) { }
    }
}
