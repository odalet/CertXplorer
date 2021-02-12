using System.Linq;
using System.Text;
using System.Globalization;

namespace Delta.Icao
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Removes the diacritics from the input string.
        /// </summary>
        /// <remarks>
        /// For example, the following string:
        /// <b><![CDATA["aàâä-eéèêë-iïî-oöô-uùüû-yÿ-ç"]]></b>
        /// would be replaced by
        /// <b><![CDATA["aaaa-eeeee-iii-ooo-uuuu-yy-c"]]></b>
        /// </remarks>
        /// <param name="input">The input string.</param>
        /// <returns>The cleaned-up string.</returns>
        public static string RemoveDiacritics(this string input)
        {
            var builder = new StringBuilder();
            var characters = input
                .Normalize(NormalizationForm.FormD)
                .Where(c =>
                {
                    var category = CharUnicodeInfo.GetUnicodeCategory(c);
                    return category != UnicodeCategory.NonSpacingMark;
                });

            return builder
                .Append(characters.ToArray())
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Returns a copy of the specified string converted to uppercase using the casing rules of 
        /// the invariant culture and after having removed all the diacritics (<see cref="RemoveDiacritics"/>).
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The uppercase equivalent of the specified string.</returns>
        public static string ToUpperInvariantNoDiacritics(this string input) => input.RemoveDiacritics().ToUpperInvariant();
    }
}
