using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Delta.Icao.Logging;

namespace Delta.Icao
{
    partial struct BirthDate
    {
        private static class Parser
        {
            private const int defaultY2kPivot = 29;
            private static readonly char[] patternCharacters = new char[] { 'y', 'M', 'd' };
            private static readonly char[] numberCharacters = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'x', 'X' };
            private static readonly char[] separatorCharacters = new char[] { '/', '-', '.' };
            private static readonly char[] xCharacters = new char[] { 'x', 'X' };
            private static readonly char[] zeroCharacters = new char[] { '0' };

            public static BirthDate Parse(string s, BirthDateFormatInfo info)
            {
                if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("s");
                if (info == null) info = new BirthDateFormatInfo();

                // First let's try to parse this as a regular date                
                try
                {
                    var dt = DateTime.Parse(s, info.DateTimeFormatInfo);
                    return new BirthDate(dt);
                }
                catch (Exception ex)
                {
                    log.Warning($"Could not parse a {nameof(BirthDate)} from '{s}': {ex.Message}");
                    // Well, this means we probably have 'missing' components
                }

                // This did not work let's parse the string by hand
                return Parse(s, info.Pattern, GetY2kPivot(info));
            }

            private static BirthDate Parse(string s, string p, int y2kPivot)
            {
                var pattern = NormalizePattern(p);

                var ylist = new List<char>();
                var mlist = new List<char>();
                var dlist = new List<char>();

                List<char> getList(char pc)
                {
                    switch (pc)
                    {
                        case 'y': return ylist;
                        case 'M': return mlist;
                        case 'd': return dlist;
                    }

                    return null; // problem!
                }

                var patternIndex = 0;
                var list = getList(pattern[patternIndex]);
                var patternComponentsCount = pattern.Length;
                foreach (var c in s)
                {
                    if (numberCharacters.Contains(c)) // add the character to the corresponding queue (y/m/d)
                        list.Add(c);
                    else if (separatorCharacters.Contains(c)) // separator
                    {
                        patternIndex++;
                        if (patternIndex >= patternComponentsCount) throw new InvalidOperationException(
                            $"This {nameof(BirthDate)} representation is made of too many components (> {patternComponentsCount}).");

                        list = getList(pattern[patternIndex]);
                    }
                    else throw new InvalidOperationException(
                        $"Character '{c}' can not be part of a {nameof(BirthDate)} representation.");
                }

                // Now translate the lists into numbers
                var y = GetNumber(ylist);

                // Don't convert the year if it is unknown
                y = IsUnknown(ylist) ? 0 : HandleY2k(y, y2kPivot);

                var m = GetNumber(mlist);
                var d = GetNumber(dlist);

                return new BirthDate(y, m, d);
            }

            private static int GetY2kPivot(BirthDateFormatInfo info) =>
                info?.DateTimeFormatInfo?.Calendar == null ?
                defaultY2kPivot :
                info.DateTimeFormatInfo.Calendar.TwoDigitYearMax - 2000;

            private static int HandleY2k(int y, int y2kPivot)
            {
                if (y >= 100) return y;
                return y + (y <= y2kPivot ? 2000 : 1900);
            }

            private static bool IsUnknown(List<char> list)
            {
                foreach (var unc in xCharacters)
                {
                    var test = unc;
                    if (list.Any(c => c == test)) return true;
                }

                foreach (var unc in zeroCharacters)
                {
                    var test = unc;
                    if (list.All(c => c == test)) return true;
                }

                return false;
            }

            private static int GetNumber(List<char> list)
            {
                var s = string.Join("", list.ToArray());
                _ = int.TryParse(s, out var n);
                return n;
            }

            private static string NormalizePattern(string pattern)
            {
                var normalized = new StringBuilder();
                var lastPatternCharacter = '\0';

                foreach (var c in pattern)
                    if (patternCharacters.Contains(c) && c != lastPatternCharacter)
                    {
                        _ = normalized.Append(c);
                        lastPatternCharacter = c;
                    }

                return normalized.ToString();
            }
        }

        /// <summary>
        /// Converts the string representation of a birth date to a <see cref="BirthDate" /> object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="result">When this method returns, contains <see cref="BirthDate" /> object equivalent
        /// to the birth date contained in <paramref name="text" /> if the conversion succeeded;
        /// or <see cref="BirthDate.Empty" /> if the conversion failed.
        /// The conversion fails if <paramref name="text" /> is null or empty or is not
        /// in the correct format. This parameter is passed uninitialized.</param>
        /// <param name="culture">The culture used to parse.</param>
        /// <returns>
        ///   <c>True</c> if <paramref name="text" /> was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string text, out BirthDate result, CultureInfo culture = null)
        {
            result = Empty;
            try
            {
                result = Parse(text, culture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the string representation of a birth date to a <see cref="BirthDate" /> object.
        /// The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="result">When this method returns, contains <see cref="BirthDate" /> object equivalent
        /// to the birth date contained in <paramref name="text" /> if the conversion succeeded;
        /// or <see cref="BirthDate.Empty" /> if the conversion failed.
        /// The conversion fails if <paramref name="text" /> is null or empty or is not
        /// in the correct format. This parameter is passed uninitialized.</param>
        /// <param name="info">An object containing formatting and parsing information.</param>
        /// <returns>
        ///   <c>True</c> if <paramref name="text" /> was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string text, out BirthDate result, BirthDateFormatInfo info)
        {
            result = Empty;
            try
            {
                result = Parse(text, info);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the string representation of a birth date to a <see cref="BirthDate" /> object.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="culture">The culture used to parse.</param>
        /// <returns>A <see cref="BirthDate"/> object equivalent to the birth date specified in <paramref name="text"/>.</returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static BirthDate Parse(string text, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException(
                $"{nameof(text)} cannot be null or empty", nameof(text));

            var formatInfo = new BirthDateFormatInfo(culture, true);
            return Parse(text, formatInfo);
        }

        /// <summary>
        /// Converts the string representation of a birth date to a <see cref="BirthDate" /> object.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="info">An object containing formatting and parsing information.</param>
        /// <returns>A <see cref="BirthDate"/> object equivalent to the birth date specified in <paramref name="text"/>.</returns>
        public static BirthDate Parse(string text, BirthDateFormatInfo info)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException(
                $"{nameof(text)} cannot be null or empty", nameof(text));

            return Parser.Parse(text, info ?? new BirthDateFormatInfo());
        }

        /// <summary>
        /// Similar to <see cref="Parse" /> but accepts <c>null</c> and empty strings
        /// and returns a <see cref="System.Nullable{Siti.BirthDate}" /> object.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="culture">The culture used to parse.</param>
        /// <returns>
        /// An instance of <see cref="System.Nullable{Siti.BirthDate}" />
        /// </returns>
        public static BirthDate? ParseToNullable(string text, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(text)) return new BirthDate?();

            var bdate = Parse(text, culture);
            return new BirthDate?(bdate);
        }

        /// <summary>
        /// Similar to <see cref="Parse" /> but accepts <c>null</c> and empty strings
        /// and returns a <see cref="System.Nullable{Siti.BirthDate}" /> object.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="info">An object containing formatting and parsing information.</param>
        /// <returns>
        /// An instance of <see cref="System.Nullable{Siti.BirthDate}" />
        /// </returns>
        public static BirthDate? ParseToNullable(string text, BirthDateFormatInfo info)
        {
            if (string.IsNullOrEmpty(text)) return new BirthDate?();

            var bdate = Parse(text, info);
            return new BirthDate?(bdate);
        }
    }
}
