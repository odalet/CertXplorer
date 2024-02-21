using System.IO;

namespace Delta.CertXplorer.Logging.log4net;

public static class Log4NetServiceFactory
{
    public static ILogService CreateService(string configurationFile)
    {
        var fileInfo = File.Exists(configurationFile)
            ? new FileInfo(configurationFile)
            : throw new FileNotFoundException("Log4Net configuration file was not found", configurationFile);
        return new Log4NetService(fileInfo);
    }
}
