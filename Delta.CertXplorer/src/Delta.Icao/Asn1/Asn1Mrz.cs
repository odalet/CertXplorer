using Delta.CapiNet.Asn1;
using Delta.Icao.Logging;

namespace Delta.Icao.Asn1
{
    public class Asn1Mrz : Asn1Utf8String
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(MrzHelper));

        private string[] mrz = null;

        public Asn1Mrz(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        public string[] Mrz
        {
            get
            {
                if (mrz == null)
                {
                    mrz = new string[0]; // default

                    if (string.IsNullOrEmpty(base.Value))
                        return mrz;

                    var mrzFormat = MrzFormat.FindByTotalLength(base.Value);
                    if (mrzFormat == null)
                    {
                        log.Warning(string.Format("Invalid MRZ: {0}", base.Value));
                        return mrz;
                    }

                    var parser = MrzParser.Create(base.Value);
                    if (!parser.Parse())
                    {
                        log.Warning(string.Format("MRZ ({0}) could not be decoded.", mrzFormat));
                        return mrz;
                    }

                    mrz = parser.MrzArray;
                    log.Verbose(string.Format("MRZ ({0}) was successfully decoded: \r\n{1}", mrzFormat, string.Join("\r\n", Mrz)));
                }

                return mrz;
            }
        }

        public override string ToString()
        {
            return string.Format("MRZ: \r\n{0}", string.Join("\r\n", Mrz));
        }
    }
}
