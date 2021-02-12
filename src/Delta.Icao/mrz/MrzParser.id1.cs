using System;
using Delta.Icao.Logging;

namespace Delta.Icao
{
    partial class MrzParser
    {
        private sealed class Id1Parser : MrzParser // 3x30: Cards (Bank format)
        {
            private static readonly ILogService log = LogManager.GetLogger<Id1Parser>();

            private string[] array = null;

            public Id1Parser(string mrz) : base(mrz) { }

            public override string[] MrzArray => array;
            protected override MrzFormat Format => MrzFormat.Id1;

            public override bool Parse()
            {
                try
                {
                    var l1 = Mrz.Substring(0, 30);
                    var l2 = Mrz.Substring(30, 30);
                    var l3 = Mrz.Substring(60);

                    array = new string[] { l1, l2, l3 };

                    AddComponent(MrzComponentCode.DocumentCode, l1.Substring(0, 2));
                    AddComponent(MrzComponentCode.IssuingState, l1.Substring(2, 3));
                    AddComponent(MrzComponentCode.DocumentNumber, l1.Substring(5, 9));
                    AddComponent(MrzComponentCode.DocumentNumberCheckDigit, l1.Substring(14, 1));

                    AddComponent(MrzComponentCode.DateOfBirth, l2.Substring(0, 6));
                    AddComponent(MrzComponentCode.DateOfBirthCheckDigit, l2.Substring(6, 1));
                    AddComponent(MrzComponentCode.Sex, l2.Substring(7, 1));
                    AddComponent(MrzComponentCode.DateOfExpiry, l2.Substring(8, 6));
                    AddComponent(MrzComponentCode.DateOfExpiryCheckDigit, l2.Substring(14, 1));
                    AddComponent(MrzComponentCode.Nationality, l2.Substring(15, 3));

                    AddComponent(MrzComponentCode.Name, l3);
                }
                catch (Exception ex)
                {
                    log.Warning($"{Format} MRZ Parsing failed: {ex.Message}", ex);
                    return false;
                }

                return true;
            }
        }
    }
}
