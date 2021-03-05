using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Delta.CertXplorer.ComponentModel;
using Delta.CertXplorer.Diagnostics;
using Delta.CertXplorer.Logging;

namespace Delta.CertXplorer
{
    public static class This
    {
        private static readonly ObservableServiceContainer parentContainer;
        private static readonly ObservableServiceContainer childContainer;

        /// <summary>
        /// Initializes the <see cref="This"/> class.
        /// </summary>
        static This()
        {
            parentContainer = new ObservableServiceContainer();
            childContainer = new ObservableServiceContainer(parentContainer);

            AddDefaultServices();

            WireEvents(); // Child service container events.

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnCurrentDomainUnhandledException);
        }

        public static event ServiceNotificationEventHandler ServiceRequested;
        public static event ServiceNotificationEventHandler ServiceAdded;
        public static event ServiceNotificationEventHandler ServiceRemoved;

        public static IServiceContainer Services => childContainer;
        public static ILogService Logger => childContainer.GetService<ILogService>();

        public static T GetService<T>() where T : class => Services.GetService<T>();
        public static T GetService<T>(bool mandatory) where T : class => Services.GetService<T>(mandatory);
        public static void AddService<T>(T instance) where T : class => Services.AddService(instance);
        public static IList<Type> GetServicesList() => childContainer.GetServicesList(true);

        private static void AddDefaultServices()
        {
            var logManager = new LogManagerService(new SimpleLogService());

            parentContainer.AddService<ILogService>(logManager);

            // We don't want the log manager to be masked by another implementation.
            childContainer.AddService<ILogManagerService>(logManager);

            // Remark: we pass the child container to the IExceptionHandlerService instance
            // constructor so that it uses the services that may be added by a client class,
            // and not always the default ones (especially for the logging service)
            parentContainer.AddService<IExceptionHandlerService>(
                new BaseExceptionHandlerService(childContainer));
        }

        private static void WireEvents()
        {
            childContainer.ServiceRequested += (s, e) => ServiceRequested?.Invoke(null, e);
            childContainer.ServiceAdded += (s, e) => ServiceAdded?.Invoke(null, e);
            childContainer.ServiceRemoved += (s, e) => ServiceRemoved?.Invoke(null, e);
        }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) => 
            HandleException(e.ExceptionObject, e.IsTerminating);

        private static void HandleException(object exceptionObject, bool isTerminating)
        {
            try
            {
                var service = Services.GetService<IExceptionHandlerService>(true);
                service.HandleException(exceptionObject, isTerminating);
            }
            catch (ServiceNotFoundException)
            {
                Console.WriteLine($"FATAL ERROR: {exceptionObject}");
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine($"An exception occured while processing an unhandled exception: {ex}");
                }
                catch
                {
                    // Here, we are allowed to eat the exception ;-)
                }
            }
        }
    }
}