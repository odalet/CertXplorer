using System;
using System.ComponentModel;
using System.IO;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin.DataHandlers
{
    internal class BinaryAsTextHandler : IDataHandler
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        private class DataInfo
        {
            public DataInfo(string filename, DataFormat format)
            {
                FileName = filename;
                Format = format;
            }

            public string FileName { get; private set; }
            public DataFormat Format { get; private set; }
        }

        private readonly BaseDataHandlerPlugin plugin;
        private readonly DataFormat format;        
        private byte[] decoded = null;
        private string fileName = null;

        public BinaryAsTextHandler(BaseDataHandlerPlugin parentPlugin, DataFormat dataFormat)
        {
            if (parentPlugin == null) throw new ArgumentNullException(nameof(parentPlugin));
            plugin = parentPlugin;
            format = dataFormat;
        }

        public bool CanHandleFile(string filename)
        {
            if (!File.Exists(filename)) throw new FileNotFoundException(string.Format(
                "File {0} could not be found;", filename));

            fileName = filename;
            var fileContent = File.ReadAllText(filename);
            
            try
            {
                decoded = ConversionEngine.GetBytes(fileContent, format);
                return true;
            }
            catch (Exception ex)
            {
                plugin.Log.Verbose(string.Format("Could not decode file {0} as a {1} file", fileName, format), ex);
                return false;
            }
        }

        public IData ProcessFile()
        {
            if (decoded == null) throw new InvalidOperationException("Invalid input data: null");            
            return new SimpleData()
            {
                MainData = decoded,
                AdditionalData = new DataInfo(fileName, format)
            };
        }
    }
}
