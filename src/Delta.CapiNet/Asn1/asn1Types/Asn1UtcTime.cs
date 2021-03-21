using System;
using System.Globalization;
using System.Text;

namespace Delta.CapiNet.Asn1
{
    public class Asn1UtcTime : Asn1Object
    {
        private static readonly string[] UtcFormats = new string[]
        {
            "yyMMddHHmmssZ",
            "yyMMddHHmmZ",
            "yyMMddHHmmz",
            "yyMMddHHmmssz"
        };

        public Asn1UtcTime(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject)
        {
            var ascii = Encoding.ASCII.GetString(content.Workload);
            Value = DateTimeOffset.ParseExact(ascii,
                UtcFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        public DateTimeOffset Value { get; }

        public override string ToString() => Value == DateTimeOffset.MinValue ? string.Empty : Value.ToString("u");
    }
}
