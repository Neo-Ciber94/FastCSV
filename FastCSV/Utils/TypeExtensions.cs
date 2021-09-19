using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsArrayOrEnumerable(this Type type)
        {
            return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type GetEnumerableElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType()!;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type} don't implement {typeof(IEnumerable)}");
            }

            if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return typeof(object);
        }
    }
}
