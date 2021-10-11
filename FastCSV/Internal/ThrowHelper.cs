using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Internal
{
    internal static class ThrowHelper
    {
        public static Exception InvalidType(Type type, Type expected)
        {
            return new InvalidOperationException($"Expected type {expected} but was {type}");
        }

        public static Exception CollectionHandlingRequired()
        {
            return new InvalidOperationException("'CollectionHandling' option is required for collection types");
        }
    }
}
