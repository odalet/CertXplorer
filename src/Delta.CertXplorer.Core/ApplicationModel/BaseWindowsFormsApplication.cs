using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Delta.CertXplorer.ApplicationModel.Services;
using Delta.CertXplorer.Configuration;
using Delta.CertXplorer.Diagnostics;
using Delta.CertXplorer.Logging;
using Delta.CertXplorer.Logging.log4net;
using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.ApplicationModel
{
    public abstract class BaseWindowsFormsApplication : BaseApplication
    {
        private const string defaultLoggingSettingsFileName = "log4net.config";
        private const string defaultLayoutSettingsFileName = "app.layout.xml";

        private string loggingSettingsFileName = defaultLoggingSettingsFileName;
        private string layoutSettingsFileName = defaultLayoutSettingsFileName;

        protected BaseWindowsFormsApplication() { }

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
            
            AddOtherServices();
            if (OnBeforeCreateMainForm()) ShowMainForm();
        }

        protected override void InitializeThisApplication() => InitializeThisApplication(ApplicationCulture);

        protected virtual void InitializeThisApplication(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ToolStripManager.Renderer = new BaseToolStripRenderer();
        }

        protected override IExceptionHandlerService CreateExceptionHandlerService() => new ExceptionHandlerService();

        protected virtual ILayoutService CreateLayoutService() => new LayoutService(LayoutSettingsFileName);

        protected override ILogService CreateLogService() => Log4NetServiceFactory.CreateService(LoggingSettingsFileName);

        protected override ISettingsService CreateSettingsService() => new SettingsService();

        protected abstract Form CreateMainForm();

        protected virtual void ShowMainForm()
        {
            var form = CreateMainForm();
            OnBeforeShowMainForm(form);
            Application.Run(form);
        }

        protected virtual bool OnBeforeCreateMainForm() => true; // True: continue loading

        protected virtual void OnBeforeShowMainForm(Form form) { }
    }
}
