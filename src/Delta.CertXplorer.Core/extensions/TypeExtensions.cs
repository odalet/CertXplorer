using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Delta.CertXplorer
{
    /// <summary>
    /// Extends the <see cref="System.Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is a descendant of <paramref name="parentType"/>.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <param name="parentType">Type of the supposed parent.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is a descendant of <paramref name="parentType"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsA(this Type type, Type parentType) => parentType == null ? type == null : parentType.IsAssignableFrom(type);

        /// <summary>
        /// Determines whether the specified type is a descendant of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the supposed parent.</typeparam>
        /// <param name="type">The type to test.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is a descendant of <typeparamref name="T"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsA<T>(this Type type) => IsA(type, typeof(T));


        /// <summary>
        /// Creates an instance of the given <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// Type <paramref name="type"/> must provide a parameterless constructor.
        /// The constructor needn't be public.
        /// </remarks>
        /// <param name="type">The type for which to create an instance.</param>
        /// <returns>An instance of <paramref name="type"/>.</returns>
        [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "By design")]
        public static object CreateInstance(this Type type)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var ci = type.GetConstructor(flags, null, Type.EmptyTypes, null);
            return ci == null ?
                throw new ApplicationException($"Type {type} must define a parameterless constructor.") :
                ci.Invoke(null);
        }

        /// <summary>
        /// Determines whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}
