using System;
using System.Collections.Generic;
using System.Linq;

namespace Delta.CertXplorer
{
    /// <summary>
    /// This class doesn't contain extension methods, but is a helper aimed
    /// at providing generic helpers for enumerations.
    /// </summary>
    /// <typeparam name="T"><c>T</c> must represent an enumeration type.</typeparam>
    public static class Enums<T>
    {
        public static IEnumerable<T> Values => Enum.GetValues(typeof(T)).Cast<T>();

        public static IEnumerable<string> Names => Enum.GetNames(typeof(T)).AsEnumerable();

        public static IEnumerable<KeyValuePair<string, T>> ValuesByName => Enum
            .GetValues(typeof(T))
            .Cast<T>()
            .Select(value => new KeyValuePair<string, T>(Enum.GetName(typeof(T), value), value));

        public static IEnumerable<KeyValuePair<T, string>> NamesByValue => Enum
            .GetValues(typeof(T))
            .Cast<T>()
            .Select(value => new KeyValuePair<T, string>(value, Enum.GetName(typeof(T), value)));
    }
}
