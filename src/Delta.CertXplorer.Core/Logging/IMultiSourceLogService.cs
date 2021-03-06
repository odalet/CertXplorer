namespace Delta.CertXplorer.Logging
{
    public interface IMultiSourceLogService : ILogService
    {
        ILogService this[string sourceName] { get; }
        ILogService this[LogSource source] { get; }
        LogSource CurrentSource { get; }
    }
}
