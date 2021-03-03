namespace Delta.Icao
{
    public static class BirthDateExtensions
    {
        public static string ToIcaoString(this BirthDate birthdate)
        {
            var y = "XXXX";
            var m = "XX";
            var d = "XX";

            if (birthdate.Year != 0) // if year is unknwon, so must be month and day
            {
                y = birthdate.Year.ToString("D4");
                if (birthdate.Month != 0) // if month is unknown, so must be day
                {
                    m = birthdate.Month.ToString("D2");
                    if (birthdate.Day != 0)
                        d = birthdate.Day.ToString("D2");
                }
            }

            return $"{d} {m} {y}";
        }

        /// <summary>
        /// Determines whether the specified nullable <see cref="Siti.BirthDate" /> is null or empty.
        /// </summary>
        /// <param name="value">The nullable birth date value.</param>
        /// <returns><c>true</c> if the birth date is null or has no value or is equal to <see cref="Siti.BirthDate.Empty"/>.</returns>
        public static bool IsNullOrEmpty(this BirthDate? value)
        {
            if (value == null) return true;
            return value.Value == BirthDate.Empty;
        }

        /// <summary>
        /// Serializes the the specified nullable <see cref="Siti.BirthDate" /> to its string representation.
        /// </summary>
        /// <param name="value">The nullable birth date value.</param>
        /// <param name="returnEmptyStringWhenNoValue">
        /// if set to <c>true</c> a <see cref="System.String.Empty"/> value is returned if the birth date is null or has no value.<br />
        /// By default, this setting is set to <c>false</c>.</param>
        /// <returns>The string representation of the specified <see cref="Siti.BirthDate" />.</returns>
        /// <seealso cref="Siti.BirthDate.SerializeToString"/>
        public static string SerializeToString(this BirthDate? value, bool returnEmptyStringWhenNoValue = false)
        {
            if (value == null) return returnEmptyStringWhenNoValue ? string.Empty : null;
            return value.Value.SerializeToString();
        }
    }
}
