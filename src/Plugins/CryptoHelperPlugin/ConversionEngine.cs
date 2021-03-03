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
                Plugin.LogService.Error($"Could not open or process file {filename ?? "<NULL>"}: {ex.Message}", ex);
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
                Plugin.LogService.Error($"Could not process or save file {fname}: {ex.Message}", ex);
                _ = ErrorBox.Show($"Could not save data to file {fname}");
            }

        }

        public static byte[] GetBytes(string data, DataFormat format) => format switch
        {
            DataFormat.Text => Encoding.UTF8.GetBytes(data),
            DataFormat.Hexa => HexaConverter.GetBytes(data),
            DataFormat.Base64 => Convert.FromBase64String(data),
            DataFormat.UrlEncoded => HttpUtility.UrlDecodeToBytes(data),
            DataFormat.UrlEncodedBase64 => Convert.FromBase64String(HttpUtility.UrlDecode(data)),
            _ => new byte[0],
        };

        private static byte[] Process(byte[] data, Operation operation) => operation switch
        {
            Operation.Convert => data,
            Operation.Sha1 => ComputeSha1(data),
            _ => new byte[0]
        };

        private static byte[] ComputeSha1(byte[] data)
        {
            var prov = new SHA1CryptoServiceProvider();
            prov.Initialize();
            return prov.ComputeHash(data);
        }

        private static string GetString(byte[] data, DataFormat format) => format switch
        {
            DataFormat.Text => Encoding.UTF8.GetString(data),
            DataFormat.Hexa => HexaConverter.GetString(data),
            DataFormat.Base64 => Convert.ToBase64String(data),
            DataFormat.UrlEncoded => HttpUtility.UrlEncode(data),
            DataFormat.UrlEncodedBase64 => HttpUtility.UrlEncode(Convert.ToBase64String(data)),
            _ => string.Empty,
        };
    }
}
