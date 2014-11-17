using System;

namespace Delta.Icao
{
    partial class MrzParser
    {
        private class Id3Parser : MrzParser // 2x44: Passports
        {
            private string[] array = null;

            public Id3Parser(string mrz) : base(mrz) { }

            protected override MrzFormat Format
            {
                get { return MrzFormat.Id3; }
            }

            public override string[] MrzArray
            {
                get { return array; }
            }

            public override bool Parse()
            {
                try
                {
                    var l1 = Mrz.Substring(0, 44);
                    var l2 = Mrz.Substring(44);

                    array = new string[] { l1, l2 };

                    AddComponent(MrzComponentCode.DocumentCode, l1.Substring(0, 2));
                    AddComponent(MrzComponentCode.IssuingState, l1.Substring(2, 3));
                    AddComponent(MrzComponentCode.Name, l1.Substring(5, 39));

                    AddComponent(MrzComponentCode.DocumentNumber, l2.Substring(0, 9));
                    AddComponent(MrzComponentCode.DocumentNumberCheckDigit, l2.Substring(9, 1));
                    AddComponent(MrzComponentCode.Nationality, l2.Substring(10, 3));
                    AddComponent(MrzComponentCode.DateOfBirth, l2.Substring(13, 6));
                    AddComponent(MrzComponentCode.DateOfBirthCheckDigit, l2.Substring(19, 1));
                    AddComponent(MrzComponentCode.Sex, l2.Substring(20, 1));
                    AddComponent(MrzComponentCode.DateOfExpiry, l2.Substring(21, 6));
                    AddComponent(MrzComponentCode.DateOfExpiryCheckDigit, l2.Substring(27, 1));
                    AddComponent(MrzComponentCode.OptionalData, l2.Substring(28, 14));
                    AddComponent(MrzComponentCode.OptionalDataCheckDigit, l2.Substring(42, 1));
                    AddComponent(MrzComponentCode.CompositeCheckDigit, l2.Substring(43, 1));
                }
                catch (Exception ex)
                {
                    var debugException = ex;
                    return false;
                }

                return true;
            }
        }
    }
}
