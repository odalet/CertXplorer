using System;
using System.IO;
using System.Security;

namespace Delta.CertXplorer.ThisImplementation
{
    /// <summary>
    /// Provides properties and methods for accessing the local file system.
    /// </summary>
    /// <remarks>
    /// This is a basically a C# translation of <c>Microsoft.VisualBasic.FileIO.FileSystem</c>.
    /// </remarks>
    public class FileSystemProxy
    {
        public SpecialDirectoriesProxy SpecialDirectories { get; } = new SpecialDirectoriesProxy();

        public string GetTempFileName() => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        public string CurrentDirectory
        {
            get => NormalizePath(Directory.GetCurrentDirectory());
            set => Directory.SetCurrentDirectory(value);
        }

        internal static string NormalizePath(string path) => GetLongPath(RemoveEndingSeparator(Path.GetFullPath(path)));

        private static string RemoveEndingSeparator(string path) => 
            Path.IsPathRooted(path) && path.Equals(Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase) ?
            path : 
            path.TrimEnd(new char[]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            });

        private static bool IsRoot(string path)
        {
            if (!Path.IsPathRooted(path)) return false;

            path = path.TrimEnd(new char[]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            });

            return string.Compare(path, Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static string GetLongPath(string fullPath)
        {
            try
            {
                if (!IsRoot(fullPath))
                {
                    var info = new DirectoryInfo(GetParentPath(fullPath));
                    if (File.Exists(fullPath))
                        return info.GetFiles(Path.GetFileName(fullPath))[0].FullName;

                    if (Directory.Exists(fullPath))
                        return info.GetDirectories(Path.GetFileName(fullPath))[0].FullName;
                }

                return fullPath;
            }
            catch (Exception exception)
            {
                if (exception is not ArgumentException &&
                    exception is not ArgumentNullException &&
                    exception is not PathTooLongException &&
                    exception is not NotSupportedException &&
                    exception is not DirectoryNotFoundException &&
                    exception is not SecurityException &&
                    exception is not UnauthorizedAccessException) throw;

                return fullPath;
            }
        }

        private static string GetParentPath(string path) => IsRoot(path) ?
            throw new ArgumentException($"Cannot get parent path for {path}: it is a root path") :
            Path.GetDirectoryName(path.TrimEnd(new char[]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            }));
    }
}
