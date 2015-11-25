using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Delta.CertXplorer.Extensibility;
using Delta.CertXplorer.Extensibility.UI;

namespace CryptoHelperPlugin
{
    internal static class ConversionEngine
    {
        public static string Run(string data, DataFormat inputFormat, DataFormat outputFormat, Operation operation)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            var bytes = GetBytes(data, inputFormat);
            var result = Process(bytes, operation);
            return GetString(result, outputFormat);
        }

        public static string Load(string filename, DataFormat format)
        {
            try
            {
                var bytes = File.ReadAllBytes(filename);
                return GetString(bytes, format);
            }
            catch (Exception ex)
            {
                Plugin.LogService.Error(string.Format("Could not open or process file {0}: {1}", filename ?? "<NULL>", ex.Message), ex);
                return ex.Message;
            }
        }

        public static void Save(string data, string filename, DataFormat format)
        {
            try
            {
                var bytes = GetBytes(data, format);
                File.WriteAllBytes(filename, bytes);
            }
            catch (Exception ex)
            {
                var fname = filename ?? "<NULL>";
                Plugin.LogService.Error(string.Format("Could not process or save file {0}: {1}", fname, ex.Message), ex);
                ErrorBox.Show(string.Format("Could not save data to file", fname));
            }

        }

        private static byte[] Process(byte[] data, Operation operation)
        {
            switch (operation)
            {
                case Operation.Convert:
                    return data; 
                case Operation.Sha1:
                    var prov = new SHA1CryptoServiceProvider();
                    prov.Initialize();
                    return prov.ComputeHash(data);
            }

            return null;
        }

        private static byte[] GetBytes(string data, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Text:
                    return Encoding.UTF8.GetBytes(data);
                case DataFormat.Hexa:
                    return HexaConverter.GetBytes(data);
                case DataFormat.Base64:
                    return Convert.FromBase64String(data);
                case DataFormat.UrlEncoded:
                    return HttpUtility.UrlDecodeToBytes(data);
                case DataFormat.UrlEncodedBase64:
                    return Convert.FromBase64String(HttpUtility.UrlDecode(data));
            }

            return null;
        }

        private static string GetString(byte[] data, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Text:
                    return Encoding.UTF8.GetString(data);
                case DataFormat.Hexa:
                    return HexaConverter.GetString(data);
                case DataFormat.Base64:
                    return Convert.ToBase64String(data);
                case DataFormat.UrlEncoded:
                    return HttpUtility.UrlEncode(data);
                case DataFormat.UrlEncodedBase64:
                    return HttpUtility.UrlEncode(Convert.ToBase64String(data));
            }

            return null;
        }
    }
}
