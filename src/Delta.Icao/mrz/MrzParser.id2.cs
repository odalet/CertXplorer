using System;

namespace Delta.Icao
{
    partial class MrzParser
    {
        private class Id2Parser : MrzParser // 2x36: French ID Card For instance
        {
            private string[] array = null;

            public Id2Parser(string mrz) : base(mrz) { }

            protected override MrzFormat Format
            {
                get { return MrzFormat.Id2; }
            }

            public override string[] MrzArray
            {
                get { return array; }
            }

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
                    var debugException = ex;
                    return false;
                }

                // TODO: parsing is not complete.
                return false;
            }
        }
    }
}
