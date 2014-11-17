using System;
using System.Linq;
using System.Collections.Generic;

namespace Delta.Icao
{
    public abstract partial class MrzParser
    {
        public class MrzComponentCode
        {
            public const string DocumentCode = "DocumentCode";
            public const string IssuingState = "IssuingState";
            public const string Name = "Name";
            public const string DocumentNumber = "DocumentNumber";
            public const string DocumentNumberCheckDigit = "DocumentNumberCheckDigit";
            public const string Nationality = "Nationality";
            public const string DateOfBirth = "DateOfBirth";
            public const string DateOfBirthCheckDigit = "DateOfBirthCheckDigit";
            public const string Sex = "Sex";
            public const string DateOfExpiry = "DateOfExpiry";
            public const string DateOfExpiryCheckDigit = "DateOfExpiryCheckDigit";
            public const string OptionalData = "OptionalData";
            public const string OptionalDataCheckDigit = "OptionalDataCheckDigit";
            public const string CompositeCheckDigit = "CompositeCheckDigit";
        }

        public static MrzParser Create(string[] mrz)
        {
            if (mrz == null) throw new ArgumentNullException("mrz");
            // Check lengths
            if (mrz.Select(m => m.Length).Distinct().Count() != 1)
                throw new ArgumentException("All lines of a Mrz must have the same length.", "mrz");

            return Create(string.Join("", mrz));
        }

        public static MrzParser Create(string mrz)
        {
            if (string.IsNullOrEmpty(mrz)) throw new ArgumentNullException("mrz");
            var format = MrzFormat.FindByTotalLength(mrz);
            if (format == null) throw new ArgumentException("Could not determine Mrz Format given the specified mrz value.", "mrz");
            return Create(mrz, format);
        }

        private static MrzParser Create(string mrz, MrzFormat format)
        {
            switch (format.Name)
            {
                case MrzFormat.Id1FormatName: return new Id1Parser(mrz);
                case MrzFormat.Id2FormatName: return new Id2Parser(mrz);
                case MrzFormat.Id3FormatName: return new Id3Parser(mrz);
            }

            throw new ArgumentException(string.Format("Mrz Format '{0}' is unknown", format), "format");
        }

        private Dictionary<string, string> mrzComponents = new Dictionary<string, string>();

        protected MrzParser(string mrz)
        {
            Mrz = mrz;
        }

        public string Mrz { get; private set; }

        public abstract string[] MrzArray { get; }

        protected abstract MrzFormat Format { get; }

        public abstract bool Parse();

        public bool Contains(string code)
        {
            return mrzComponents.ContainsKey(code);
        }

        public string Get(string code)
        {
            if (mrzComponents.ContainsKey(code))
                return mrzComponents[code];
            return string.Empty;
        }

        protected void AddComponent(string componentCode, string componentValue)
        {
            mrzComponents.Add(componentCode, componentValue);
        }
    }
}
