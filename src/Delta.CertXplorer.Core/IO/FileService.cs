using System;
using System.IO;
using System.Windows.Forms;
using Delta.CertXplorer.Logging;

namespace Delta.CertXplorer.IO
{
    public sealed class FileService : IFileService
    {
        private readonly IServiceProvider services;

        public FileService(IServiceProvider serviceProvider) =>
            services = serviceProvider ?? throw new ArgumentNullException("serviceProvider");

        public bool FileExists(string path) => File.Exists(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public OperationResult SafeOpen(FileOperationDelegate operation, FileType[] types, int defaultTypeIndex, string title)
        {
            var filter = FileType.CombineFilters(out var fileTypes, types);
            if (fileTypes == null || fileTypes.Length == 0)
            {
                fileTypes = new FileType[] { FileType.All };
                filter = FileType.All.Filter;
            }

            if (defaultTypeIndex >= fileTypes.Length) defaultTypeIndex = 0;

            using var dialog = new OpenFileDialog
            {
                Filter = filter,
                FilterIndex = defaultTypeIndex + 1,
                Title = title,
                SupportMultiDottedExtensions = true,
                RestoreDirectory = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var typeIndex = dialog.FilterIndex - 1;
                var selectedFileType = typeIndex < fileTypes.Length ? types[typeIndex] : FileType.Unknown;

                try
                {
                    operation(dialog.FileName, selectedFileType);
                    return OperationResult.OK;
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return OperationResult.Failed;
                }
            }
            
            return OperationResult.Cancelled;
        }

        public OperationResult SafeSave(FileOperationDelegate operation, string filename, FileType[] types, int defaultTypeIndex, string title)
        {
            var filter = FileType.CombineFilters(out var fileTypes, types);
            if (fileTypes == null || fileTypes.Length == 0)
            {
                fileTypes = new FileType[] { FileType.All };
                filter = FileType.All.Filter;
            }

            if (defaultTypeIndex >= fileTypes.Length) defaultTypeIndex = 0;

            using var dialog = new SaveFileDialog
            {
                Filter = filter,
                FilterIndex = defaultTypeIndex + 1,
                Title = title,
                SupportMultiDottedExtensions = true,
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var typeIndex = dialog.FilterIndex - 1;
                var selectedFileType = typeIndex < fileTypes.Length ? types[typeIndex] : FileType.Unknown;

                try
                {
                    operation(dialog.FileName, selectedFileType);
                    return OperationResult.OK;
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return OperationResult.Failed;
                }
            }
            
            return OperationResult.Cancelled;
        }

        private void LogError(Exception ex)
        {
            var log = services.GetService<ILogService>();
            if (log == null)
                Console.Error.WriteLine(ex == null ? "Unknown Error\r\n": $"ERROR - Exception: {ex}\r\n");
            else 
                log.Error(ex == null ? "Unknown Error" : $"Exception: {ex}");
        }
    }
}
