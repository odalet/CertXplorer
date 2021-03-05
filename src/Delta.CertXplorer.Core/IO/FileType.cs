using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Delta.CertXplorer.IO
{
    /// <summary>
    /// Helps supporting file types by associating extensions and descriptions.
    /// </summary>
    /// <remarks>
    /// The only supported patterns are of the followinf form: *.extension
    /// </remarks>
    public sealed class FileType
    {
        private static readonly Dictionary<string, FileType> types = new Dictionary<string, FileType>();

        public static readonly FileType Unknown = new FileType("UNKNOWN", new string[] { "*.*" }, "Unknown Files");
        public static readonly FileType All = new FileType("ALL", new string[] { "*.*" }, "All Files");
        public static readonly FileType Text = new FileType("TXT", new string[] { "*.txt" }, "Text Files");
        public static readonly FileType Log = new FileType("LOG", new string[] { "*.log", "*.txt" }, "Log Files");
        public static readonly FileType Rtf = new FileType("RTF", new string[] { "*.rtf" }, "Rich Text Files");

        private FileType(string id, string[] patterns, string filterText)
        {
            if (patterns == null || patterns.Length == 0) throw new ArgumentException($"{nameof(patterns)} cannot be null or empty", nameof(patterns));
            if (string.IsNullOrEmpty(id)) throw new ArgumentException($"{nameof(id)} cannot be null or empty", nameof(id));
            if (types.ContainsKey(id)) throw new ArgumentException($"This id ({id}) is already registred", nameof(id));

            if (string.IsNullOrEmpty(filterText)) filterText = patterns[0] + " Files";

            for (var i = 0; i < patterns.Length; i++)
            {
                if (!patterns[i].StartsWith("*.")) 
                    patterns[i] = "*." + patterns[i];
            }

            TypeId = id;
            Patterns = patterns;
            Filter = BuildFilter(filterText);

            types.Add(TypeId, this);
        }

        internal static IDictionary<string, FileType> Types => types;

        public string[] Patterns { get; }
        public string Filter { get; }
        public string FilterWithAll => TypeId == All.TypeId ? Filter : $"{Filter}|{All.Filter}";

        internal string TypeId { get; }

        internal static string CombineFilters(params FileType[] types) => CombineFilters(out _, types);

        [SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "Not that many strings")]
        internal static string CombineFilters(out FileType[] combinedTypes, params FileType[] types)
        {
            if (types == null || types.Length == 0)
            {
                combinedTypes = new FileType[] { All };
                return All.Filter;
            }

            var alreadyContainsStarFilter = types.Any(ft => ft.Filter == All.Filter);

            var combinedTypesCount = types.Length;
            if (!alreadyContainsStarFilter) combinedTypesCount++;

            combinedTypes = new FileType[combinedTypesCount];
            types.CopyTo(combinedTypes, 0);
            if (!alreadyContainsStarFilter) 
                combinedTypes[types.Length] = All;

            var combinedFilter = string.Empty;
            foreach (var type in combinedTypes) combinedFilter += type.Filter + "|";
            return combinedFilter.Substring(0, combinedFilter.Length - 1);
        }

        public bool Matches(string filename) => Patterns.Any(pattern => 
            MatchesCaseSensitive(filename.ToUpperInvariant(), pattern.ToUpperInvariant()));

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is FileType ft ? ft.TypeId == TypeId : obj.ToString() == TypeId;
        }

        public override int GetHashCode() => TypeId.GetHashCode();

        [SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "Not that many strings")]
        private string BuildFilter(string filterText)
        {
            var patternsList = string.Empty;
            foreach (var pattern in Patterns)
                patternsList += pattern + ";";

            patternsList = patternsList.Substring(0, patternsList.Length - 1);

            return new StringBuilder()   
                .Append(filterText)
                .Append(" (")
                .Append(patternsList)
                .Append(" )|")
                .Append(patternsList)
                .ToString();
        }

        private bool MatchesCaseSensitive(string filename, string pattern)
        {
            if (pattern == "*.*") return true;
            if (pattern[0] == '*')
            {
                var flength = filename.Length;
                var plength = pattern.Length;

                while (--plength > 0)
                {
                    if (pattern[plength] == '*') return MatchesCaseSensitive(filename, pattern, 0, 0);
                    if (flength-- == 0) return false;
                    if (pattern[plength] != filename[flength] && pattern[plength] != '?') return false;
                }
                return true;
            }
            else return MatchesCaseSensitive(filename, pattern, 0, 0);
        }

        private bool MatchesCaseSensitive(string filename, string pattern, int findex, int pindex)
        {
            var flength = filename.Length;
            var plength = pattern.Length;
            char next;
            while(true)
            {
                if (pindex == plength) return findex == flength;
                next = pattern[pindex++];
                if (next == '?')
                {
                    if (findex == flength) return false;
                    findex++;
                }
                else if (next == '*')
                {
                    if (pindex == plength) return true;
                    while (findex < flength)
                    {
                        if (MatchesCaseSensitive(filename, pattern, findex, pindex)) return true;
                        findex++;
                    }
                }
                else
                {
                    if (findex == flength || filename[findex] != next) return false;
                    findex++;
                }
            }
        }
    }
}
