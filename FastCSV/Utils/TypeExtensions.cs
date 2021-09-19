using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Whether if the given type is a nullable type.
        /// </summary>
        /// <param name="type">Type yo check.</param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Whether the given type is an array or enumerable.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArrayOrEnumerable(this Type type)
        {
            return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the element type of the given type if is an array or enumerable.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static Type? GetArrayOrEnumerableElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType()!;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return typeof(object);
        }
    }
}
