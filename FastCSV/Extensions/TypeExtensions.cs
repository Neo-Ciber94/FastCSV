using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastCSV.Extensions
{
    internal static class TypeExtensions
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
        public static bool IsEnumerableType(this Type type)
        {
            // Special case
            if (type == typeof(string))
            {
                return false;
            }

            return type.IsArray || typeof(ITuple).IsAssignableFrom(type) || typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the element type of the given type if is an array or enumerable.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns></returns>
        public static Type? GetEnumerableElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType()!;
            }

            // Special case
            if (typeof(ITuple).IsAssignableFrom(type))
            {
                return typeof(object);
            }

            if (!typeof(IEnumerable).IsAssignableFrom(type) || type.IsGenericTypeDefinition)
            {
                return null;
            }

            // Special case
            if (type == typeof(BitArray))
            {
                return typeof(int);
            }

            // We look for the type in T in IEnumerable<T>

            Type? genericEnumerableInterface = null;

            if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                genericEnumerableInterface = type;
            }

            if (genericEnumerableInterface == null)
            {
                genericEnumerableInterface = type.GetInterfaces().FirstOrDefault(e =>
                {
                    return e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                });
            }

            if (genericEnumerableInterface != null)
            {
                return genericEnumerableInterface.GetGenericArguments()[0];
            }

            return typeof(object);
        }
    }
}
