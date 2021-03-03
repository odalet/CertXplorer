using System.Globalization;
using System.Text;

namespace Delta.Icao
{
    partial struct BirthDate
    {
        private static class Formatter
        {
            public static string Format(BirthDate bdate, BirthDateFormatInfo info)
            {
                if (info == null) info = new BirthDateFormatInfo();                
                return Format(bdate, info.Pattern, info.RepresentMissingComponentsWithX);
            }

            private static string Format(BirthDate bdate, string pattern, bool useX)
            {
                int step;
                var builder = new StringBuilder();                
                for (var i = 0; i < pattern.Length; i += step)
                {
                    var patternChar = pattern[i];
                    switch (patternChar)
                    {
                        case 'y':
                            var year = bdate.Year;
                            step = ParseRepeatPattern(pattern, i, patternChar);
                            FormatDigits(builder, year, step <= 2 ? 2 : 4, useX, forceSubstring: true); // either 4 or 2!
                            break;
                        case 'M':
                            var month = bdate.Month;
                            step = ParseRepeatPattern(pattern, i, patternChar);
                            if (step > 2) // We do not support (yet) string-based month representations
                                step = 2;
                            FormatDigits(builder, month, step, useX);
                            break;
                        case 'd':
                            var day = bdate.Day;
                            step = ParseRepeatPattern(pattern, i, patternChar);
                            if (step > 2) // We do not support (yet) string-based day representations
                                step = 2;
                            FormatDigits(builder, day, step, useX);
                            break;
                        default: // Other characters are separators; they are simply copied to the output buffer
                            _ =builder.Append(patternChar);
                            step = 1;
                            break;
                    }
                }

                return builder.ToString();
            }

            private static int ParseRepeatPattern(string pattern, int position, char patternChar)
            {
                var index = position + 1;
                while (index < pattern.Length && pattern[index] == patternChar)
                    index++;

                return index - position;
            }

            private static void FormatDigits(StringBuilder builder, int value, int length, bool useX, bool forceSubstring = false)
            {
                if (useX && value == 0)
                {
                    _ = builder.Append(new string('X', length));
                    return;
                }

                var result = value.ToString($"D{length}");
                if (result.Length > length && forceSubstring)
                    result = result.Substring(result.Length - length);

                _ = builder.Append(result);
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => ToString((CultureInfo)null);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance formatted 
        /// according to the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">The culture used for formatting.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(CultureInfo culture) => ToString(new BirthDateFormatInfo(culture, true));

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance formatted 
        /// according to the specified <see cref="BirthDateFormatInfo"/> object.
        /// </summary>
        /// <param name="info">An object containing formatting and parsing information.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(BirthDateFormatInfo info) => Formatter.Format(this, info ?? new BirthDateFormatInfo());
    }
}
