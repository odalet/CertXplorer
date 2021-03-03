using System;
using System.IO;
using System.Windows.Forms;

namespace Delta.CertXplorer.ThisImplementation
{
    public sealed class SpecialDirectoriesProxy
    {
        internal SpecialDirectoriesProxy() { }

        public string AllUsersApplicationData => GetDirectoryPath(Application.CommonAppDataPath, "AllUsersApplicationData");
        public string CurrentUserApplicationData => GetDirectoryPath(Application.UserAppDataPath, "CurrentUserApplicationData");
        public string Desktop => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Desktop");
        public string MyDocuments => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MyDocuments");
        public string MyMusic => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "MyMusic");
        public string MyPictures => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "MyPictures");
        public string ProgramFiles => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ProgramFiles");
        public string Programs => GetDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Programs");
        public string Temp => GetDirectoryPath(Path.GetTempPath(), "Temp");

        private static string GetDirectoryPath(string directory, string directoryName) => string.IsNullOrEmpty(directory) ?
            throw new DirectoryNotFoundException($"Special Directory {directory} ({directoryName}) was not found") :
            FileSystemProxy.NormalizePath(directory);
    }
}
