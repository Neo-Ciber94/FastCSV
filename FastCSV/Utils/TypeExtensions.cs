using System;
using System.Collections.Generic;
using System.Text;

namespace FastCSV.Utils
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
