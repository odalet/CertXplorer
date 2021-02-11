using System;
using System.Text;
using System.Globalization;

namespace Delta.Icao
{
    partial struct BirthDate
    {
        private static class Formatter
        {
            public static string Format(BirthDate bdate, BirthDateFormatInfo info)
            {
                if (bdate == null) throw new ArgumentNullException("bdate");
                if (info == null) info = new BirthDateFormatInfo();
                
                return Format(bdate, info.Pattern, info.RepresentMissingComponentsWithX);
            }

            private static string Format(BirthDate bdate, string pattern, bool useX)
            {
                int step = 0;
                var builder = new StringBuilder();
                for (int i = 0; i < pattern.Length; i += step)
                {
                    char patternChar = pattern[i];
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
                            builder.Append(patternChar);
                            step = 1;
                            break;
                    }
                }

                return builder.ToString();
            }

            private static int ParseRepeatPattern(string pattern, int position, char patternChar)
            {
                int length = pattern.Length;
                int index = position + 1;
                while (index < length && pattern[index] == patternChar)
                    index++;
                return index - position;
            }

            private static void FormatDigits(StringBuilder builder, int value, int length, bool useX, bool forceSubstring = false)
            {
                if (useX && value == 0)
                {
                    builder.Append(new string('X', length));
                    return;
                }

                var format = "D" + length.ToString();
                string result = value.ToString(format);
                if (result.Length > length && forceSubstring)
                    result = result.Substring(result.Length - length);

                builder.Append(result);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString((CultureInfo)null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance formatted 
        /// according to the specified <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">The culture used for formatting.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(CultureInfo culture)
        {
            var formatInfo = new BirthDateFormatInfo(culture, true);
            return ToString(formatInfo);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance formatted 
        /// according to the specified <see cref="BirthDateFormatInfo"/> object.
        /// </summary>
        /// <param name="info">An object containing formatting and parsing information.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(BirthDateFormatInfo info)
        {
            return Formatter.Format(this, info ?? new BirthDateFormatInfo());
        }
    }
}
