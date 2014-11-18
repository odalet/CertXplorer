using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Delta.CapiNet.Pem
{
    // See http://www.ietf.org/rfc/rfc4716.txt for reference.
    public class PemDecoder
    {
        private const string headerBegin = "-----BEGIN ";
        private const string headerEnd = "-----";
        private const string footerBegin = "-----END ";
        private const string footerEnd = "-----";

        private const string headerBeginAlt = "---- BEGIN ";
        private const string headerEndAlt = " ----";
        private const string footerBeginAlt = "---- END ";
        private const string footerEndAlt = " ----";

        private static readonly Encoding pemEncoding = Encoding.ASCII;

        private List<String> errors = new List<string>();
        private List<String> warnings = new List<string>();

        private bool dirty = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PemDecoder"/> class.
        /// </summary>
        public PemDecoder() { }

        internal string TextData { get; private set; }
        internal byte[] Workload { get; private set; }
        internal byte[] PgpChecksum { get; private set; }
        internal PemKind Kind { get; private set; }
        internal string FullHeader { get; private set; }
        internal string FullFooter { get; private set; }
        internal Dictionary<string, string> AdditionalHeaders { get; private set; }
        internal string AdditionalText { get; private set; }

        public string[] Errors { get { return errors.ToArray(); } }
        public string[] Warnings { get { return warnings.ToArray(); } }

        public static bool IsPemFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename");
            if (!File.Exists(filename)) throw new ArgumentException(string.Format(
                "File {0} does not exist", filename), "filename");

            return IsPemData(File.ReadAllBytes(filename));
        }

        public static bool IsPemData(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            return IsPemString(pemEncoding.GetString(data));
        }

        public static bool IsPemString(string data)
        {
            return data.StartsWith(headerBegin) || data.StartsWith(headerBeginAlt);
        }

        public PemInfo ReadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException("filename");
            if (!File.Exists(filename)) throw new ArgumentException(string.Format(
                "File {0} does not exist", filename), "filename");

            TextData = File.ReadAllText(filename);
            return Decode();
        }

        public PemInfo ReadData(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            TextData = pemEncoding.GetString(data);
            return Decode();
        }

        public PemInfo ReadString(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("data");
            TextData = data;
            return Decode();
        }

        private PemInfo Decode()
        {
            if (dirty) throw new InvalidOperationException("A PemDecoder instance should only be used once.");
            dirty = true;

            var strings = ReadAllLines(TextData);
            if (strings == null || strings.Length == 0)
            {
                AddError("No data.");
                return null;
            }

            var altHeader = false;
            FullHeader = strings[0];
            if (!IsHeader(FullHeader, out altHeader))
            {
                AddError("First line does not contain a PEM Header.");
                return null;
            }

            var altFooter = false;
            var index = 1;
            var workloadLines = new List<string>();
            var additionalDataBuilder = new StringBuilder();
            while (index < strings.Length)
            {
                var current = strings[index];
                var tag = string.Empty;
                var content = string.Empty;
                if (IsAdditionalHeader(current, out tag, out content))
                    AddAdditionalHeader(tag, content);
                else if (string.IsNullOrWhiteSpace(current))
                {
                    // ignore white lines
                }
                else if (!IsFooter(current, out altFooter))
                {
                    if (string.IsNullOrEmpty(FullFooter))
                        workloadLines.Add(current);
                    else additionalDataBuilder.AppendLine(current);
                }
                else FullFooter = current;
                index++;
            }

            // All data was read, now go check consistency.

            if (altHeader != altFooter) AddWarning(altHeader ?
                "PEM inconsistency: header uses the alternate form ('---- BEGIN') whereas footer does not ('-----END')." :
                "PEM inconsistency: header uses the normal form ('-----BEGIN') whereas footer does not ('---- END').");

            var header = ExtractHeader(FullHeader, altHeader);
            var footer = ExtractFooter(FullFooter, altFooter);

            if (header != footer) AddWarning(string.Format(
                "PEM inconsistency: header ({0}) does not match footer ({1}).", header, footer));

            Kind = PemKind.Find(header);
            if (Kind == null)
            {
                AddWarning(string.Format("PEM header {0} is not a well-known PEM header.", header));
                Kind = PemKind.GetCustom(header, "Not a well-known PEM header.");
            }

            // convert the lines list to a string builder.            
            IEnumerable<string> lines = Kind.HasPgpChecksum ? // if PGP-like PEM, last line is a checksum, not part of the real workload
                Enumerable.Range(0, workloadLines.Count - 1).Select(i => workloadLines[i]) :
                workloadLines;

            var base64Builder = new StringBuilder();
            foreach (var line in lines)
                base64Builder.Append(line);

            if (Kind.HasPgpChecksum)
            {
                try
                {
                    // PGP Checksums starts with a '=' character
                    var checksum = workloadLines[workloadLines.Count - 1];
                    checksum = checksum.TrimStart('=');
                    PgpChecksum = Convert.FromBase64String(checksum);
                }
                catch (Exception ex)
                {
                    AddWarning(string.Format("Could not decode PGP Checksum from input Base64 data: {0}", ex.Message));
                }
            }

            try
            {
                Workload = Convert.FromBase64String(base64Builder.ToString());
            }
            catch (Exception ex)
            {
                AddError(string.Format("Could not decode input Base64 data: {0}", ex.Message));
            }

            // TODO: validates PGP checksums

            AdditionalText = additionalDataBuilder.ToString();

            return new PemInfo(this);
        }

        private bool IsHeader(string input, out bool isAlternateHeader)
        {
            isAlternateHeader = false;
            if (input.StartsWith(headerBegin))
                return true;
            else if (input.StartsWith(headerBeginAlt))
            {
                isAlternateHeader = false;
                return true;
            }

            return false;
        }

        private bool IsFooter(string input, out bool isAlternateFooter)
        {
            isAlternateFooter = false;
            if (input.StartsWith(footerBegin))
                return true;
            else if (input.StartsWith(footerBeginAlt))
            {
                isAlternateFooter = false;
                return true;
            }

            return false;
        }

        private bool IsAdditionalHeader(string input, out string tag, out string content)
        {
            tag = string.Empty;
            content = string.Empty;

            const char separator = ':';
            var splitIndex = input.IndexOf(separator);
            if (splitIndex < 0) return false;

            // extract tag and value
            tag = input.Substring(0, splitIndex);
            content = input.Substring(splitIndex + 1);

            return true;
        }

        private void AddAdditionalHeader(string tag, string content)
        {
            if (AdditionalHeaders == null)
                AdditionalHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // headers are case-insensitive

            if (AdditionalHeaders.ContainsKey(tag))
                AdditionalHeaders[tag] += "\r\n" + content;
            else AdditionalHeaders.Add(tag, content);
        }

        private string ExtractHeader(string input, bool alt)
        {
            var prefix = alt ? headerBegin : headerBeginAlt;
            var suffix = alt ? headerEnd : headerEndAlt;

            return input.Substring(prefix.Length, input.Length - prefix.Length - suffix.Length).Trim();
        }

        private string ExtractFooter(string input, bool alt)
        {
            var prefix = alt ? footerBegin : footerBeginAlt;
            var suffix = alt ? footerEnd : footerEndAlt;

            return input.Substring(prefix.Length, input.Length - prefix.Length - suffix.Length).Trim();
        }

        private void AddError(string error)
        {
            errors.Add(error);
        }

        private void AddWarning(string warning)
        {
            warnings.Add(warning);
        }

        private void ClearAll()
        {
            errors = new List<string>();
            warnings = new List<string>();

            TextData = null;
            Workload = null;
            Kind = null;
            FullHeader = null;
            FullFooter = null;
            AdditionalHeaders = null;
        }

        private static string[] ReadAllLines(string data)
        {
            const char continuationCharacter = '\\';
            var result = new List<string>();

            using (var reader = new StringReader(data))
            {
                string buffer = string.Empty;
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    // Let's handle the continuation character here (simpler than in the AdditionalHeaders code)
                    var trimmed = str.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && trimmed[trimmed.Length - 1] == continuationCharacter)
                        buffer += trimmed.Substring(0, trimmed.Length - 1);
                    else
                    {
                        result.Add(buffer + str);
                        buffer = string.Empty;
                    }
                }

                reader.Close();
            }

            return result.ToArray();
        }
    }
}
