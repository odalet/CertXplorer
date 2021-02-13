using Delta.CapiNet.Asn1;
using Delta.Icao.Logging;

namespace Delta.Icao.Asn1
{
    public sealed class Asn1Mrz : BaseAsn1String
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(Asn1Mrz));
        private string[] decoded = null;

        public Asn1Mrz(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        public string[] Mrz
        {
            get
            {
                if (decoded == null) decoded = DecodeMrz(Value);
                return decoded;
            }
        }

        public override string ToString() => $"MRZ: \r\n{string.Join("\r\n", Mrz)}";

        private string[] DecodeMrz(string input)
        {
            var mrz = new string[0]; // default

            if (string.IsNullOrEmpty(input))
                return mrz;

            var mrzFormat = MrzFormat.FindByTotalLength(input);
            if (mrzFormat == null)
            {
                log.Warning($"Invalid MRZ: {input}");
                return mrz;
            }

            var parser = MrzParser.Create(input);
            if (!parser.Parse())
            {
                log.Warning($"MRZ ({mrzFormat}) could not be decoded.");
                return mrz;
            }

            mrz = parser.MrzArray;
            log.Verbose($"MRZ ({mrzFormat}) was successfully decoded: \r\n{string.Join("\r\n", mrz)}");

            return mrz;
        }
    }
}
