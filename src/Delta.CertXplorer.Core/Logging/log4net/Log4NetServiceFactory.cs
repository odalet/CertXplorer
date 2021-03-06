using System;
using System.IO;

namespace Delta.CertXplorer.Logging.log4net
{
    public static class Log4NetServiceFactory
    {
        public static ILogService CreateService() => new Log4NetService();

        public static ILogService CreateService(string configurationFile)
        {
            FileInfo fileInfo;
            if (File.Exists(configurationFile)) fileInfo = new FileInfo(configurationFile);
            else
            {
                // If we couldn't find the file, we try to search for it in the application's configuration
                // file directory.
                var appConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                var path = Path.GetDirectoryName(appConfig);
                var configFile = Path.Combine(path, configurationFile);
                fileInfo = File.Exists(configFile) ?
                    new FileInfo(configFile) : 
                    throw new FileNotFoundException("Log4Net configuration file was not found", configurationFile);
            }

            return new Log4NetService(fileInfo);
        }
    }
}
