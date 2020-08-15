using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCSV.Utils
{
    public static class NullableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? ToNullable<T>(this T value) where T: struct
        {
            return new Nullable<T>(value);
        }
    }
}
