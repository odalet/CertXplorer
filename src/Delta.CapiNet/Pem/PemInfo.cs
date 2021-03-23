using System;
using System.Collections.Generic;

namespace Delta.CapiNet.Pem
{
    public sealed class PemInfo
    {
        internal PemInfo(PemDecoder decoder)
        {
            if (decoder == null) throw new ArgumentNullException(nameof(decoder));

            TextData = decoder.TextData;
            Workload = decoder.Workload;
            PgpChecksum = decoder.PgpChecksum;
            Kind = decoder.Kind;
            FullHeader = decoder.FullHeader;
            FullFooter = decoder.FullFooter;
            AdditionalHeaders = decoder.AdditionalHeaders;
            AdditionalText = decoder.AdditionalText;
            Warnings = decoder.Warnings ?? new string[0];
        }

        public string TextData { get; }
        public byte[] Workload { get; }
        public byte[] PgpChecksum { get; }
        public PemKind Kind { get; }
        public string FullHeader { get; }
        public string FullFooter { get; }
        public string AdditionalText { get; }
        public string[] Warnings { get; }
        public IReadOnlyDictionary<string, string> AdditionalHeaders { get; }
    }
}
