using System;
using System.Linq;
using System.Collections.Generic;

namespace Delta.Icao
{
    public abstract partial class MrzParser
    {
        public static class MrzComponentCode
        {
            public static readonly string DocumentCode = nameof(DocumentCode);
            public static readonly string IssuingState = nameof(IssuingState);
            public static readonly string Name = nameof(Name);
            public static readonly string DocumentNumber = nameof(DocumentNumber);
            public static readonly string DocumentNumberCheckDigit = nameof(DocumentNumberCheckDigit);
            public static readonly string Nationality = nameof(Nationality);
            public static readonly string DateOfBirth = nameof(DateOfBirth);
            public static readonly string DateOfBirthCheckDigit = nameof(DateOfBirthCheckDigit);
            public static readonly string Sex = nameof(Sex);
            public static readonly string DateOfExpiry = nameof(DateOfExpiry);
            public static readonly string DateOfExpiryCheckDigit = nameof(DateOfExpiryCheckDigit);
            public static readonly string OptionalData = nameof(OptionalData);
            public static readonly string OptionalDataCheckDigit = nameof(OptionalDataCheckDigit);
            public static readonly string CompositeCheckDigit = nameof(CompositeCheckDigit);
        }

        private readonly Dictionary<string, string> mrzComponents = new Dictionary<string, string>();

        public static MrzParser Create(string[] mrz)
        {
            if (mrz == null) throw new ArgumentNullException(nameof(mrz));
            
            if (mrz.Select(m => m.Length).Distinct().Count() != 1) // Check lengths
                throw new ArgumentException("All lines of a MRZ must have the same length.", "mrz");

            return Create(string.Join("", mrz));
        }

        public static MrzParser Create(string mrz)
        {
            if (string.IsNullOrEmpty(mrz)) throw new ArgumentNullException(nameof(mrz));

            var format = MrzFormat.FindByTotalLength(mrz);
            return format == null
                ? throw new ArgumentException("Could not determine MRZ Format given the specified mrz value.", "mrz")
                : Create(mrz, format);
        }

        private static MrzParser Create(string mrz, MrzFormat format)
        {
            switch (format.Name)
            {
                case MrzFormat.Id1FormatName: return new Id1Parser(mrz);
                case MrzFormat.Id2FormatName: return new Id2Parser(mrz);
                case MrzFormat.Id3FormatName: return new Id3Parser(mrz);
            }

            throw new ArgumentException($"MRZ Format '{format}' is unknown");
        }

        protected MrzParser(string mrz) => Mrz = mrz;

        public string Mrz { get; }
        public abstract string[] MrzArray { get; }
        protected abstract MrzFormat Format { get; }

        public abstract bool Parse();

        public bool Contains(string code) => mrzComponents.ContainsKey(code);

        public string Get(string code) => mrzComponents.ContainsKey(code) ? mrzComponents[code] : string.Empty;

        protected void AddComponent(string componentCode, string componentValue) => mrzComponents.Add(componentCode, componentValue);
    }
}
