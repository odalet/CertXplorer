using System;
using System.Globalization;

namespace Delta.Icao
{
    /// <summary>
    /// Represents a date of birth.
    /// </summary>
    /// <remarks>
    /// Compared to a regular date, a birth date can have:
    /// <list type="bullet">
    /// <item>its day missing (equal to 0).</item>
    /// <item>both its day and month missing.</item>
    /// <item>The whole date unknown (day, month and year are equal to 0).</item>
    /// </list>
    /// </remarks>
    public partial struct BirthDate : IEquatable<BirthDate>, IComparable<BirthDate>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDate"/> struct.
        /// </summary>
        /// <param name="date">The date.</param>
        public BirthDate(DateTime date) : this(date.Year, date.Month, date.Day) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDate" /> struct.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the month (<paramref name="m"/>) is unknown (ie. equal to 0), then the day (<paramref name="d"/>) must also
        /// be unknown (ie. equal to 0).
        /// </para>
        /// <para>
        /// If the year (<paramref name="y"/>) is unknown (ie. equal to 0), then both the month and the day must also be unknown 
        /// (ie. equal o 0).
        /// </para>
        /// </remarks>
        /// <param name="y">The year (0 if unknown).</param>
        /// <param name="m">The month (0 if unknown).</param>
        /// <param name="d">The day (0 if unknown).</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public BirthDate(int y, int m, int d)
        {
            var exception = EnsureAreValid(y, m, d);
            if (exception != null) throw exception;

            Year = y;
            Month = m;
            Day = d;
        }

        /// <summary>
        /// Gets a <see cref="BirthDate"/> object set to the current local date on this computer.
        /// </summary>
        public static BirthDate Now => new BirthDate(DateTime.Now);

        public static readonly BirthDate Empty = new BirthDate(0, 0, 0);

        /// <summary>
        /// The smallest possible value for a <see cref="BirthDate"/> object.
        /// </summary>
        public static readonly BirthDate MinValue = new BirthDate(DateTime.MinValue);

        /// <summary>
        /// The largest possible value for a <see cref="BirthDate"/> object.
        /// </summary>
        public static readonly BirthDate MaxValue = new BirthDate(DateTime.MaxValue);

        /// <summary>
        /// Gets this BirthDate's Year.
        /// </summary>
        public int Year { get; }

        /// <summary>
        /// Gets this BirthDate's Month.
        /// </summary>
        public int Month { get; }

        /// <summary>
        /// Gets this BirthDate's Day.
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// Returns a new <see cref="BirthDate" /> that adds the specified number of years to
        /// the value of this instance.
        /// </summary>
        /// <param name="count">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>
        /// An object whose value is the sum of the date and time represented by this
        /// instance and the number of years represented by <paramref name="count" />.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Can't add a number of years when Year is unknown.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// count;value of the resulting System.DateTime is less than BirthDate.MinValue or greater than BirthDate.MaxValue.
        /// </exception>
        public BirthDate AddYears(int count)
        {
            if (Year == 0) throw new InvalidOperationException("Can't add a number of years when Year is unknown.");

            var y = Year + count;
            return y < MinValue.Year || y > MaxValue.Year ?
                throw new ArgumentOutOfRangeException(nameof(count), "value of the resulting BirthDate is less than BirthDate.MinValue or greater than BirthDate.MaxValue.")
                : new BirthDate(y, Month, Day);
        }

        /// <summary>
        /// Returns a new <see cref="BirthDate" /> that adds the specified number of months to
        /// the value of this instance.
        /// </summary>
        /// <param name="count">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>
        /// An object whose value is the sum of the date and time represented by this
        /// instance and the number of months represented by <paramref name="count" />.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Can't add a number of month when Month is unknown.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">count;value of the resulting System.DateTime is less than BirthDate.MinValue or greater than BirthDate.MaxValue.</exception>
        public BirthDate AddMonths(int count)
        {
            if (Month == 0) throw new InvalidOperationException("Can't add a number of months when Month is unknown.");

            var d = Day;
            var m = Month;
            var y = Year;

            var toAdd = Month - 1 + count;
            if (toAdd >= 0)
            {
                m = 1 + toAdd % 12;
                y += toAdd / 12;
            }
            else
            {
                m = 12 + (toAdd + 1) % 12;
                y += (toAdd - 11) / 12;
            }

            if (y < MinValue.Year || y > MaxValue.Year) throw new ArgumentOutOfRangeException(nameof(count),
               "value of the resulting BirthDate is less than BirthDate.MinValue or greater than BirthDate.MaxValue.");

            // Make sure the value for day is not too high
            var max = DateTime.DaysInMonth(y, m);
            if (d > max) d = max;

            return new BirthDate(y, m, d);
        }

        /// <summary>
        /// Returns a new <see cref="BirthDate"/> that adds the specified number of days to 
        /// the value of this instance.
        /// </summary>
        /// <param name="count">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>
        /// An object whose value is the sum of the date and time represented by this
        /// instance and the number of days represented by <paramref name="count"/>.
        /// </returns>
        public BirthDate AddDays(int count)
        {
            if (Day == 0) throw new InvalidOperationException("Can't add a number of days when Day is unknown.");
            var dt = ToDateTime();
            return new BirthDate(dt.AddDays(count));
        }

        /// <summary>
        /// Converts this instance to a <see cref="DateTime" /> object.
        /// </summary>
        /// <returns>
        /// A <see cref="DateTime" /> object if the conversion succeeded.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">A BirthDate with an unknown year can't be converted to a DateTime object.</exception>
        /// <exception cref="System.InvalidOperationException">A BirthDate with an unknown month can't be converted to a DateTime object.</exception>
        /// <exception cref="System.InvalidOperationException">A BirthDate with an unknown day can't be converted to a DateTime object.</exception>
        public DateTime ToDateTime()
        {
            if (Year == 0) throw new InvalidOperationException("A BirthDate with an unknown year can't be converted to a DateTime object.");
            if (Month == 0) throw new InvalidOperationException("A BirthDate with an unknown month can't be converted to a DateTime object.");
            if (Day == 0) throw new InvalidOperationException("A BirthDate with an unknown day can't be converted to a DateTime object.");
            return new DateTime(Year, Month, Day);
        }

        /// <summary>
        /// Deserializes a BirthDate from a string; the expected format is YYYYMMDD or YYYY*MM*DD where * is a separator.
        /// with unknown values replaced either by 00 - or 0000 for the year - or XX - XXXX for a year.
        /// </summary>
        /// <param name="serialized">The serialized form of the birth date.</param>
        /// <returns>
        /// An instance of <see cref="BirthDate" />
        /// </returns>
        /// <exception cref="System.ArgumentNullException">serialized</exception>
        /// <exception cref="System.FormatException"></exception>
        public static BirthDate DeserializeFromString(string serialized)
        {
            if (string.IsNullOrEmpty(serialized))
                throw new ArgumentNullException("serialized");

            int[] indices;
            if (serialized.Length == 8)
                indices = new int[] { 0, 4, 6 };
            else if (serialized.Length == 10) // We have 2 separators; whatever they are doesn't matter.
                indices = new int[] { 0, 5, 8 };
            else throw new FormatException($"Unable to parse input string '{serialized}' into a BirthDate type.");
            
            var ys = serialized.Substring(indices[0], 4);
            var y = ys.ToUpperInvariant() == "XXXX" ? 0 : int.Parse(ys, CultureInfo.InvariantCulture);
            var ms = serialized.Substring(indices[1], 2);
            var m = ms.ToUpperInvariant() == "XX" ? 0 : int.Parse(ms, CultureInfo.InvariantCulture);
            var ds = serialized.Substring(indices[2], 2);
            var d = ds.ToUpperInvariant() == "XX" ? 0 : int.Parse(ds, CultureInfo.InvariantCulture);

            return new BirthDate(y, m, d);
        }

        /// <summary>
        /// Similar to <see cref="DeserializeFromString"/> but accepts <c>null</c> and empty strings 
        /// and returns a <see cref="Nullable{Siti.BirthDate}"/> object.
        /// </summary>
        /// <param name="serialized">The serialized form of the birth date.</param>
        /// <returns>An instance of <see cref="Nullable{Siti.BirthDate}"/></returns>
        public static BirthDate? DeserializeFromStringToNullable(string serialized)
        {
            if (string.IsNullOrEmpty(serialized)) return new BirthDate?();

            var bdate = DeserializeFromString(serialized);
            return new BirthDate?(bdate);
        }

        /// <summary>
        /// Serializes this instance to a string representation;
        /// the format is YYYYMMDD, with unknown values replaced by 00 or 0000.
        /// </summary>
        /// <returns>Serialized form of this birth date instance.</returns>
        public string SerializeToString() => string.Format("{0:D4}{1:D2}{2:D2}", Year, Month, Day);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => Year ^ (Month << 13 | Month >> 19) ^ (Day << 21 | Day >> 11);

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is BirthDate date && Equals(date);

        /// <summary>
        /// Determines whether the specified <see cref="BirthDate" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BirthDate" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="BirthDate" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BirthDate other) => other.Year == Year && other.Month == Month && other.Day == Day;

        /// <summary>
        /// Ensures the specified birth dates are equal.
        /// </summary>
        /// <param name="d1">The first date to test for equality.</param>
        /// <param name="d2">The second date to test for equality.</param>
        /// <returns><c>true</c> if the specified dates are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BirthDate d1, BirthDate d2) => d1.Equals(d2);

        /// <summary>
        /// Ensures the specified birth dates are different.
        /// </summary>
        /// <param name="d1">The first date to test for inequality.</param>
        /// <param name="d2">The second date to test for inequality.</param>
        /// <returns><c>true</c> if the specified dates are different; otherwise, <c>false</c>.</returns>
        public static bool operator !=(BirthDate d1, BirthDate d2) => !d1.Equals(d2);

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings: 
        /// <list type="bullet">
        /// <item>Less than zero: This object is less than the other parameter.</item>
        /// <item>Zero: This object is equal to other.</item>
        /// <item>Greater than zero: This object is greater than other.</item>
        /// </list>
        /// </returns>
        public int CompareTo(BirthDate other)
        {
            var yc = Year.CompareTo(other.Year);
            if (yc != 0) return yc;

            var mc = Month.CompareTo(other.Month);
            if (mc != 0) return mc;

            return Day.CompareTo(other.Day);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns 
        /// an integer that indicates whether the current instance precedes, follows, or 
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: 
        /// <list type="bullet">
        /// <item>Less than zero: This instance precedes <paramref name="obj" /> in the sort order.</item>
        /// <item>Zero: This instance occurs in the same position in the sort order as <paramref name="obj" />.</item>
        /// <item>Greater than zero: This instance follows <paramref name="obj" /> in the sort order.</item>
        /// </list>
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is BirthDate date) return CompareTo(date);
            return 0;
        }

        private static Exception EnsureAreValid(int y, int m, int d)
        {
            if (y < 0 || y > 9999) return new ArgumentOutOfRangeException("y", "Year must be in the [0;9999]  interval.");
            if (m < 0 || m > 12) return new ArgumentOutOfRangeException("m", "Month must be in the [0;12]  interval.");
            if (d < 0 || d > 31) return new ArgumentOutOfRangeException("d", "Day must be in the [0;31]  interval.");

            // Testing the input represents a valid birth date
            var y2 = y == 0 ? 1 : y; // Year = 0 results in an invalid date!
            var m2 = m == 0 ? 1 : m; // Month = 0 results in an invalid date!
            var d2 = d == 0 ? 1 : d; // Day = 0 results in an invalid date!
            try
            {
                _ = new DateTime(y2, m2, d2);
            }
            catch (Exception ex)
            {
                return ex;
            }

            // Ensure day = 0 when month = 0 and month = 0 when year = 0
            if (m == 0 && d != 0) return new FormatException("Day must be 0 when Month is 0.");
            if (y == 0 && m != 0) return new FormatException("Month must be 0 when Year is 0.");

            return null; // Everything is ok.
        }
    }
}
