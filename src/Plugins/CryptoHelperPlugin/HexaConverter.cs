using System;
using System.Linq;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin
{
    internal static class HexaConverter
    {
        public class Options
        {
            public bool PrefixWithZeroX { get; set; }
            public bool LowerCase { get; set; }
            public string Separator { get; set; }
        }

        private static readonly char[] hexLetters = { 'A', 'B', 'C', 'D', 'E', 'F' };
        private static readonly Options converterOptions = new Options();

        public static Options ConverterOptions { get { return converterOptions; } }

        public static string GetString(byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            // Adapted from http://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
            var format = converterOptions.LowerCase ? "x2" : "X2";
            return string.Join(ConverterOptions.Separator,
                data.Select(b => (converterOptions.PrefixWithZeroX ? "0x" : string.Empty) + b.ToString(format)));
        }

        public static byte[] GetBytes(string data)
        {
            if (string.IsNullOrEmpty(data))
                return new byte[0];

            var temp = data.ToUpperInvariant();
            temp = temp.Replace("0X", string.Empty);
            temp = new string(temp.Where(c => char.IsDigit(c) || hexLetters.Contains(c)).ToArray());

            Plugin.LogService.Debug(string.Format("Original string was transformed to: ", FirstCharacters(temp)));

            return Enumerable.Range(0, temp.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(temp.Substring(x, 2), 16))
                .ToArray();
        }

        private static string FirstCharacters(string input, int max = 20)
        {
            if (max < 20) max = 5;
            if (input.Length <= max) return input;

            return input.Substring(0, max - 3) + "...";
        }
    }
}
