using System;
using System.Linq;
using System.Collections.Generic;
using Delta.Icao.Logging;

namespace Delta.Icao
{
    public static class MrzHelper
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(MrzHelper));
        
        private static readonly Dictionary<char, string> nationalToMrz;        
        // List of caractères that are removed from the MRZ
        private static readonly char[] eatenCharacters = new char[] { '\'', '.', ',', ';' };
        // List of characters that are replaced by a filler in the MRZ
        private static readonly char[] whiteCharacters = new char[] { '-', ',', ' ' };
        
        /// <summary>
        /// The MRZ filler character: '&lt;'
        /// </summary>
        public static readonly char FillerCharacter = '<';

        /// <summary>
        /// The MRZ filler character as a string: &quot;&lt;&quot;
        /// </summary>
        public static readonly string FillerString = FillerCharacter.ToString();

        /// <summary>
        /// Two MRZ filler characters: &quot;&lt;&lt;&quot;
        /// </summary>
        public static readonly string DoubleFillerString = new string(FillerCharacter, 2);

        /// <summary>
        /// List of the characters a MRZ can contain, and only those ones.
        /// </summary>
        public static readonly char[] MrzAuthorizedCharacters;

        /// <summary>
        /// Initializes the <see cref="MrzHelper"/> class.
        /// </summary>
        static MrzHelper()
        {
            // List of the only authorized characters in MRZ.
            MrzAuthorizedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ<".ToCharArray();

            // Initialize the transliteration dictionary based on ICAO Doc9303
            // TODO: this should be in a parameters table or config file...
            nationalToMrz = new Dictionary<char, string>();

            nationalToMrz.Add('Á', "A");
            nationalToMrz.Add('À', "A");
            nationalToMrz.Add('Â', "A");
            nationalToMrz.Add('Ä', "AE");
            nationalToMrz.Add('Ã', "A");
            nationalToMrz.Add('Ă', "A");
            nationalToMrz.Add('Å', "AA");
            nationalToMrz.Add('Ā', "A");
            nationalToMrz.Add('Ą', "A");
            nationalToMrz.Add('Ć', "C");
            nationalToMrz.Add('Ĉ', "C");
            nationalToMrz.Add('Č', "C");
            nationalToMrz.Add('Ċ', "C");
            nationalToMrz.Add('Ç', "C");
            nationalToMrz.Add('Đ', "D");
            nationalToMrz.Add('Ď', "D");
            nationalToMrz.Add('É', "E");
            nationalToMrz.Add('È', "E");
            nationalToMrz.Add('Ê', "E");
            nationalToMrz.Add('Ë', "E");
            nationalToMrz.Add('Ě', "E");
            nationalToMrz.Add('Ė', "E");
            nationalToMrz.Add('Ē', "E");
            nationalToMrz.Add('Ę', "E");
            nationalToMrz.Add('Ĕ', "E");
            nationalToMrz.Add('Ĝ', "G");
            nationalToMrz.Add('Ğ', "G");
            nationalToMrz.Add('Ġ', "G");
            nationalToMrz.Add('Ģ', "G");
            nationalToMrz.Add('Ħ', "H");
            nationalToMrz.Add('Ĥ', "H");
            // I without a dot: no specific representation; it is the latin capital i (therefore, no dot), 
            // but still it is in this list to accomodate the turkish capital i-without-a-dot.
            nationalToMrz.Add('I', "I"); 
            nationalToMrz.Add('Í', "I");
            nationalToMrz.Add('Ì', "I");
            nationalToMrz.Add('Î', "I");
            nationalToMrz.Add('Ï', "I");
            nationalToMrz.Add('Ĩ', "I");
            nationalToMrz.Add('İ', "I");
            nationalToMrz.Add('Ī', "I");
            nationalToMrz.Add('Į', "I");
            nationalToMrz.Add('Ĭ', "I");
            nationalToMrz.Add('Ĵ', "J");
            nationalToMrz.Add('Ķ', "K");
            nationalToMrz.Add('Ł', "L");
            nationalToMrz.Add('Ĺ', "L");
            nationalToMrz.Add('Ľ', "L ");
            nationalToMrz.Add('Ļ', "L");
            nationalToMrz.Add('Ŀ', "L");
            nationalToMrz.Add('Ń', "N");
            nationalToMrz.Add('Ñ', "N");
            nationalToMrz.Add('Ň', "N");
            nationalToMrz.Add('Ņ', "N");
            nationalToMrz.Add('Ŋ', "N");
            nationalToMrz.Add('Ø', "OE");
            nationalToMrz.Add('Ó', "O");
            nationalToMrz.Add('Ò', "O");
            nationalToMrz.Add('Ô', "O");
            nationalToMrz.Add('Ö', "OE");
            nationalToMrz.Add('Õ', "O");
            nationalToMrz.Add('Ő', "O");
            nationalToMrz.Add('Ō', "O");
            nationalToMrz.Add('Ŏ', "O");
            nationalToMrz.Add('Ŕ', "R");
            nationalToMrz.Add('Ř', "R");
            nationalToMrz.Add('Ŗ', "R");
            nationalToMrz.Add('Ś', "S");
            nationalToMrz.Add('Ŝ', "S");
            nationalToMrz.Add('Š', "S");
            nationalToMrz.Add('Ş', "S");
            nationalToMrz.Add('Ŧ', "T");
            nationalToMrz.Add('Ť', "T");
            nationalToMrz.Add('Ţ', "T");
            nationalToMrz.Add('Ú', "U");
            nationalToMrz.Add('Ù', "U");
            nationalToMrz.Add('Û', "U");
            nationalToMrz.Add('Ü', "UE");
            nationalToMrz.Add('Ũ', "U");
            nationalToMrz.Add('Ŭ', "U");
            nationalToMrz.Add('Ű', "U");
            nationalToMrz.Add('Ů', "U");
            nationalToMrz.Add('Ū', "U");
            nationalToMrz.Add('Ų', "U");
            nationalToMrz.Add('Ŵ', "W");
            nationalToMrz.Add('Ý', "Y");
            nationalToMrz.Add('Ŷ', "Y");
            nationalToMrz.Add('Ÿ', "Y");
            nationalToMrz.Add('Ź', "Z");
            nationalToMrz.Add('Ž', "Z");
            nationalToMrz.Add('Ż', "Z");
            nationalToMrz.Add('Þ', "TH");
            nationalToMrz.Add('Æ', "AE");
            nationalToMrz.Add('Ĳ', "IJ");
            nationalToMrz.Add('Œ', "OE");
            nationalToMrz.Add('ß', "SS");
        }
        
        public static string ToMrzString(this string input)
        {
            return MrzHelper.Transliterate(input);
        }

        public static string Transliterate(string input)
        {
            var result = string.Empty;
            foreach (var ch in input)
                result += TranslateChar(ch);

            return result;
        }
        
        #region DateTime & BirthDate Formatting

        public static string GetFieldWithControl(this DateTime? date)
        {
            return date.HasValue ?
                GetFieldWithControl(date.Value) : 
                GetFieldWithControl(string.Empty, 6);
        }

        public static string GetFieldWithControl(this BirthDate? bdate)
        {
            return bdate.HasValue ? 
                GetFieldWithControl(bdate.Value) : 
                GetFieldWithControl(string.Empty, 6);
        }

        public static string GetFieldWithControl(this DateTime date)
        {
            return GetFieldWithControl(new BirthDate(date));
        }

        public static string GetFieldWithControl(this BirthDate bdate)
        {
            var without = GetFieldWithoutControl(bdate);
            return GetFieldWithControl(without, 6);
        }

        public static string GetFieldWithoutControl(this DateTime? date)
        {
            return date.HasValue ?
                GetFieldWithoutControl(date.Value) :
                GetFieldWithoutControl(string.Empty, 6);
        }

        public static string GetFieldWithoutControl(this BirthDate? bdate)
        {
            return bdate.HasValue ?
                GetFieldWithoutControl(bdate.Value) : 
                GetFieldWithoutControl(string.Empty, 6);
        }

        public static string GetFieldWithoutControl(this DateTime date)
        {
            return GetFieldWithoutControl(new BirthDate(date));
        }

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

        #endregion

        #region Text Formatting

        public static string GetFieldWithControl(this string text, int max)
        {
            string without = GetFieldWithoutControl(text, max);
            without += GetControlNumber(without);
            return without;
        }

        public static string GetFieldWithoutControl(this string text, int max)
        {
            if (text == null) text = string.Empty;
            return text.Trim().ToMrzString().PadRight(max, FillerCharacter);
        }

        #endregion

        public static string GetControlNumber(this string input)
        {
            var ponderations = new int[] { 7, 3, 1 };
            var checksum = 0;

            Func<char, int> getCharacterValue = ch =>
            {
                if (ch == FillerCharacter) return 0;
                if (ch >= '0' && ch <= '9') return int.Parse(ch.ToString());
                return 10 + ch.ToString().ToUpperInvariant().ToCharArray()[0] - 'A';			
            };

            var characters = input.ToCharArray();
            for (int i = 0; i<characters.Length; i++)
                checksum += getCharacterValue(characters[i]) * ponderations[i % 3];

            return (checksum % 10).ToString();
        }

        public static bool IsValidLength(string mrzLine)
        {
            if (string.IsNullOrEmpty(mrzLine)) return false;
            return MrzFormat.FindByLength(mrzLine) != null;
        }

        public static bool IsValid(string mrzLine)
        {
            return IsValidLength(mrzLine) && !ContainsInvalidCharacters(mrzLine);
        }

        public static bool ContainsInvalidCharacters(string mrzLine)
        {
            return mrzLine.ToCharArray().Count(c => !MrzAuthorizedCharacters.Contains(c)) > 0;
        }
        
        private static string TranslateChar(char ch)
        {
            if (eatenCharacters.Contains(ch)) return string.Empty;
            if (whiteCharacters.Contains(ch)) return FillerString;

            var result = string.Empty;
            if (nationalToMrz.ContainsKey(ch))
                result = nationalToMrz[ch];
            else result = ch.ToString().ToUpperInvariant();

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
