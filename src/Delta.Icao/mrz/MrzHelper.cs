using System;
using System.Linq;
using System.Collections.Generic;
using Delta.Icao.Logging;
using System.Text;

namespace Delta.Icao
{
    public static class MrzHelper
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(MrzHelper));
                
        private static readonly char[] eatenCharacters = new char[] { '\'', '.', ',', ';' }; // These characters are removed from the MRZ
        private static readonly char[] whiteCharacters = new char[] { '-', ',', ' ' }; // These characters are replaced by a filler in the MRZ
        private static readonly Dictionary<char, string> nationalToMrz;
        private static readonly string FillerString = FillerCharacter.ToString();
        private static readonly string DoubleFillerString = new string(FillerCharacter, 2);

        // Initialize the transliteration dictionary based on ICAO Doc9303
        static MrzHelper() => nationalToMrz = new Dictionary<char, string>
        {
            { 'Á', "A" },
            { 'À', "A" },
            { 'Â', "A" },
            { 'Ä', "AE" },
            { 'Ã', "A" },
            { 'Ă', "A" },
            { 'Å', "AA" },
            { 'Ā', "A" },
            { 'Ą', "A" },
            { 'Ć', "C" },
            { 'Ĉ', "C" },
            { 'Č', "C" },
            { 'Ċ', "C" },
            { 'Ç', "C" },
            { 'Đ', "D" },
            { 'Ď', "D" },
            { 'É', "E" },
            { 'È', "E" },
            { 'Ê', "E" },
            { 'Ë', "E" },
            { 'Ě', "E" },
            { 'Ė', "E" },
            { 'Ē', "E" },
            { 'Ę', "E" },
            { 'Ĕ', "E" },
            { 'Ĝ', "G" },
            { 'Ğ', "G" },
            { 'Ġ', "G" },
            { 'Ģ', "G" },
            { 'Ħ', "H" },
            { 'Ĥ', "H" },
            // I without a dot: no specific representation; it is the latin capital i (therefore, no dot), 
            // but still it is in this list to accomodate the turkish capital i-without-a-dot.
            { 'I', "I" },
            { 'Í', "I" },
            { 'Ì', "I" },
            { 'Î', "I" },
            { 'Ï', "I" },
            { 'Ĩ', "I" },
            { 'İ', "I" },
            { 'Ī', "I" },
            { 'Į', "I" },
            { 'Ĭ', "I" },
            { 'Ĵ', "J" },
            { 'Ķ', "K" },
            { 'Ł', "L" },
            { 'Ĺ', "L" },
            { 'Ľ', "L " },
            { 'Ļ', "L" },
            { 'Ŀ', "L" },
            { 'Ń', "N" },
            { 'Ñ', "N" },
            { 'Ň', "N" },
            { 'Ņ', "N" },
            { 'Ŋ', "N" },
            { 'Ø', "OE" },
            { 'Ó', "O" },
            { 'Ò', "O" },
            { 'Ô', "O" },
            { 'Ö', "OE" },
            { 'Õ', "O" },
            { 'Ő', "O" },
            { 'Ō', "O" },
            { 'Ŏ', "O" },
            { 'Ŕ', "R" },
            { 'Ř', "R" },
            { 'Ŗ', "R" },
            { 'Ś', "S" },
            { 'Ŝ', "S" },
            { 'Š', "S" },
            { 'Ş', "S" },
            { 'Ŧ', "T" },
            { 'Ť', "T" },
            { 'Ţ', "T" },
            { 'Ú', "U" },
            { 'Ù', "U" },
            { 'Û', "U" },
            { 'Ü', "UE" },
            { 'Ũ', "U" },
            { 'Ŭ', "U" },
            { 'Ű', "U" },
            { 'Ů', "U" },
            { 'Ū', "U" },
            { 'Ų', "U" },
            { 'Ŵ', "W" },
            { 'Ý', "Y" },
            { 'Ŷ', "Y" },
            { 'Ÿ', "Y" },
            { 'Ź', "Z" },
            { 'Ž', "Z" },
            { 'Ż', "Z" },
            { 'Þ', "TH" },
            { 'Æ', "AE" },
            { 'Ĳ', "IJ" },
            { 'Œ', "OE" },
            { 'ß', "SS" }
        };

        /// <summary>
        /// Gets the list of characters a MRZ can contain, and only those ones.
        /// </summary>
        public static char[] MrzAuthorizedCharacters { get; } = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ<".ToCharArray();

        /// <summary>
        /// Gets the MRZ filler character: '&lt;'
        /// </summary>
        public static char FillerCharacter { get; } = '<';

        public static string ToMrzString(this string input) => MrzHelper.Transliterate(input);

        public static string Transliterate(string input)
        {
            var builder = new StringBuilder();
            foreach (var ch in input)
                _ = builder.Append(TranslateChar(ch));
            return builder.ToString();
        }

        public static string GetFieldWithControl(this DateTime? date) => 
            date.HasValue ? GetFieldWithControl(date.Value) : GetFieldWithControl(string.Empty, 6);

        public static string GetFieldWithControl(this BirthDate? bdate) => 
            bdate.HasValue ? GetFieldWithControl(bdate.Value) : GetFieldWithControl(string.Empty, 6);

        public static string GetFieldWithControl(this DateTime date) => GetFieldWithControl(new BirthDate(date));

        public static string GetFieldWithControl(this BirthDate bdate) => GetFieldWithControl(GetFieldWithoutControl(bdate), 6);

        public static string GetFieldWithControl(this string text, int max)
        {
            var without = GetFieldWithoutControl(text, max);
            without += GetControlNumber(without);
            return without;
        }

        public static string GetFieldWithoutControl(this DateTime? date) => 
            date.HasValue ? GetFieldWithoutControl(date.Value) : GetFieldWithoutControl(string.Empty, 6);

        public static string GetFieldWithoutControl(this BirthDate? bdate) =>
            bdate.HasValue ? GetFieldWithoutControl(bdate.Value) : GetFieldWithoutControl(string.Empty, 6);

        public static string GetFieldWithoutControl(this DateTime date) => GetFieldWithoutControl(new BirthDate(date));

        public static string GetFieldWithoutControl(this BirthDate bdate)
        {
            const int max = 6;

            var y = bdate.Year == 0 ?
                DoubleFillerString : bdate.Year.ToString("0000").Substring(2, 2);
            var m = bdate.Month.ToString("00").Replace("00", DoubleFillerString);
            var d = bdate.Day.ToString("00").Replace("00", DoubleFillerString);

            var text = y + m + d;
            return GetFieldWithoutControl(text, max);
        }        

        public static string GetFieldWithoutControl(this string text, int max) => 
            text.Trim().ToMrzString().PadRight(max, FillerCharacter);

        public static string GetControlNumber(this string input)
        {
            var ponderations = new int[] { 7, 3, 1 };
            var checksum = 0;

            Func<char, int> getCharacterValue = ch =>
            {
                if (ch == FillerCharacter) return 0;
                if (ch >= '0' && ch <= '9') return int.Parse(ch.ToString());
                return 10 + ch.ToString().ToUpperInvariant()[0] - 'A';			
            };

            var characters = input.ToCharArray();
            for (var i = 0; i<characters.Length; i++)
                checksum += getCharacterValue(characters[i]) * ponderations[i % 3];

            return (checksum % 10).ToString();
        }

        public static bool IsValidLength(string mrzLine) => !string.IsNullOrEmpty(mrzLine) && MrzFormat.FindByLength(mrzLine) != null;

        public static bool IsValid(string mrzLine) => IsValidLength(mrzLine) && !ContainsInvalidCharacters(mrzLine);

        public static bool ContainsInvalidCharacters(string mrzLine) => mrzLine.Any(c => !MrzAuthorizedCharacters.Contains(c));

        private static string TranslateChar(char ch)
        {
            if (eatenCharacters.Contains(ch)) return string.Empty;
            if (whiteCharacters.Contains(ch)) return FillerString;

            var result = nationalToMrz.ContainsKey(ch) ? 
                nationalToMrz[ch] : 
                ch.ToString().ToUpperInvariant();

            if (ContainsInvalidCharacters(result))
            {
                // Let's try with a more rude conversion of the input character
                var newChar = ch.ToString().ToUpperInvariantNoDiacritics()[0];
                if (newChar != ch) return TranslateChar(newChar);
                else
                {
                    log.Warning(string.Format("Could not transliterate character '{0}'. It won't appear in the MRZ.", ch));
                    return string.Empty; // Can't convert: eat the character, but log a warning.
                }
            }

            return result;
        }
    }
}
