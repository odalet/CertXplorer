using System;
using Delta.Icao.Logging;

namespace Delta.Icao
{
    partial class MrzParser
    {
        private sealed class Id2Parser : MrzParser // 2x36: French ID Card For instance
        {
            private static readonly ILogService log = LogManager.GetLogger<Id2Parser>();

            private string[] array = null;

            public Id2Parser(string mrz) : base(mrz) { }

            public override string[] MrzArray => array;
            protected override MrzFormat Format => MrzFormat.Id2;

            public override bool Parse()
            {
                try
                {
                    var l1 = Mrz.Substring(0, 36);
                    var l2 = Mrz.Substring(36);

                    array = new string[] { l1, l2 };
                }
                catch (Exception ex)
                {
                    log.Warning($"{Format} MRZ Parsing failed: {ex.Message}", ex);
                    return false;
                }

                // TODO: parsing is not complete.
                return false;
            }
        }
    }
}
