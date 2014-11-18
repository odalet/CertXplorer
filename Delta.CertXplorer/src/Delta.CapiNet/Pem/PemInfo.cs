using System;
using System.Collections.Generic;

namespace Delta.CapiNet.Pem
{
    public class PemInfo
    {
        internal PemInfo(PemDecoder decoder)
        {
            if (decoder == null) throw new ArgumentNullException("reader");

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

        public string TextData { get; private set; }
        public byte[] Workload { get; private set; }
        public byte[] PgpChecksum { get; private set; }
        public PemKind Kind { get; private set; }
        public string FullHeader { get; private set; }
        public string FullFooter { get; private set; }
        public string AdditionalText { get; private set; }
        public string[] Warnings { get; private set; }
        public IDictionary<string, string> AdditionalHeaders { get; private set; }
    }
}
