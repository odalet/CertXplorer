using System.Linq;
using System.Collections.Generic;

namespace Delta.Icao
{
    public class MrzFormat
    {
        public const string Id1FormatName = "ID1"; // Cards (Bank format)
        public const string Id2FormatName = "ID2"; // French ID Card
        public const string Id3FormatName = "ID3"; // Passports

        private static readonly List<MrzFormat> formats;

        public static readonly MrzFormat Id1;
        public static readonly MrzFormat Id2;
        public static readonly MrzFormat Id3;

        /// <summary>
        /// Initializes the <see cref="MrzFormat"/> class.
        /// </summary>
        static MrzFormat()
        {
            Id1 = new MrzFormat(Id1FormatName, 3, 30);
            Id2 = new MrzFormat(Id2FormatName, 2, 36);
            Id3 = new MrzFormat(Id3FormatName, 2, 44);
            formats = new List<MrzFormat>();
            formats.AddRange(new[] { Id1, Id2, Id3 });
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MrzFormat"/> class from being created.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="count">The count.</param>
        /// <param name="length">The length.</param>
        private MrzFormat(string name, int count, int length)
        {
            Name = name;
            LineCount = count;
            LineLength = length;
        }

        public string Name { get; private set; }

        public int LineLength { get; private set; }

        public int LineCount { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        public static MrzFormat[] Formats
        {
            get { return formats.ToArray(); }
        }

        public static MrzFormat FindByLength(string line)
        {
            var length = line == null ? 0 : line.Length;
            return formats.SingleOrDefault(f => f.LineLength == length);
        }

        public static MrzFormat FindByTotalLength(string line)
        {
            var length = line == null ? 0 : line.Length;
            return formats.SingleOrDefault(f => f.LineLength * f.LineCount == length);
        }

        public static MrzFormat FindByName(string name)
        {
            return formats.SingleOrDefault(f => string.Compare(f.Name, name, true) == 0);
        }
    }
}
