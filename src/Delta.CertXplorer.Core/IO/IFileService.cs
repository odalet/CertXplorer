namespace Delta.CertXplorer.IO
{
    public delegate void FileOperationDelegate(string filename, FileType type);

    public interface IFileService
    {
        bool FileExists(string path);
        bool DirectoryExists(string path);
        OperationResult SafeOpen(FileOperationDelegate operation, FileType[] types, int defaultTypeIndex, string title);
        OperationResult SafeSave(FileOperationDelegate operation, string filename, FileType[] types, int defaultTypeIndex, string title);
    }

    public static class FileServiceExtensions
    {
        public static OperationResult SafeOpen(this IFileService service, FileOperationDelegate operation) =>
            service.SafeOpen(operation, new[] { FileType.All });
        public static OperationResult SafeOpen(this IFileService service, FileOperationDelegate operation, FileType[] types) =>
            service.SafeOpen(operation, types, 0);
        public static OperationResult SafeOpen(this IFileService service, FileOperationDelegate operation, FileType[] types, int defaultTypeIndex) =>
            service.SafeOpen(operation, types, defaultTypeIndex, "Open...");

        public static OperationResult SafeSave(this IFileService service, FileOperationDelegate operation, string filename) =>
            service.SafeSave(operation, filename, new[] { FileType.All });
        public static OperationResult SafeSave(this IFileService service, FileOperationDelegate operation, string filename, FileType[] types) =>
            service.SafeSave(operation, filename, types, 0);
        public static OperationResult SafeSave(this IFileService service, FileOperationDelegate operation, string filename, FileType[] types, int defaultTypeIndex) =>
            service.SafeSave(operation, filename, types, defaultTypeIndex, "Save As...");

    }
}
