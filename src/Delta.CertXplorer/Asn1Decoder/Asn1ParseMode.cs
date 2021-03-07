namespace Delta.CertXplorer.Asn1Decoder
{
    public enum Asn1ParseMode
    {
        /// <summary>Only parses universal ASN.1 tags</summary>
        Standard,
        /// <summary>Parses universal ASN.1 tags and ICAO LDS specific ones</summary>
        Icao,
        /// <summary>Parses universal ASN.1 tags and Card Verifiable object specific ones</summary>
        CardVerifiable
    }
}
