using System;
using System.Runtime.CompilerServices;

namespace FastCSV.Extensions
{
    internal static class NullableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? ToNullable<T>(this T value) where T: struct
        {
            return new Nullable<T>(value);
        }
    }
}
