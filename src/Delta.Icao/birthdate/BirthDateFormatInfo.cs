using System;
using System.Globalization;

namespace Delta.Icao
{
    public sealed class BirthDateFormatInfo : ICloneable, IFormatProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BirthDateFormatInfo" /> class.
        /// </summary>
        /// <param name="representMissingComponentsWithX">
        /// If set to <c>true</c> represent missing components with X; otherwise, use 0.</param>
        /// <param name="culture">The culture.</param>
        public BirthDateFormatInfo(CultureInfo culture = null, bool representMissingComponentsWithX = true)
        {
            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            RepresentMissingComponentsWithX = representMissingComponentsWithX;
            Culture = culture;
            DateTimeFormatInfo = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
        }

        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets a value indicating whether to represent missing components with X or 0.
        /// </summary>
        public bool RepresentMissingComponentsWithX { get; }

        internal DateTimeFormatInfo DateTimeFormatInfo { get; }

        public string Pattern => DateTimeFormatInfo.ShortDatePattern;

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone() => new BirthDateFormatInfo(Culture, RepresentMissingComponentsWithX);

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">An object that specifies the type of format object to return.</param>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType" />, if the <see cref="T:System.IFormatProvider" /> implementation can supply that type of object; otherwise, null.
        /// </returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(BirthDateFormatInfo))
                return this;

            if (formatType == typeof(DateTimeFormatInfo))
                return DateTimeFormatInfo;

            return null;
        }
    }
}
