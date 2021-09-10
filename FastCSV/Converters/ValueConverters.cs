using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using FastCSV.Utils;

namespace FastCSV.Converters
{
    /// <summary>
    /// Helper for <see cref="IValueConverter"/>.
    /// </summary>
    public static class ValueConverters
    {
        private static readonly ObjectDictionary Converters = new ObjectDictionary();

        private static readonly IReadOnlyDictionary<Type, IValueConverter> BuiltInConverters = new Dictionary<Type, IValueConverter>
        {
            { typeof(bool),             new BoolValueConverter() },
            { typeof(char),             new CharValueConverter() },
            { typeof(byte),             new ByteValueConverter() },
            { typeof(short),            new ShortValueConverter() },
            { typeof(int),              new IntValueConverter() },
            { typeof(long),             new LongValueConverter() },
            { typeof(float),            new FloatValueConverter() },
            { typeof(double),           new DoubleValueConverter() },
            { typeof(sbyte),            new SByteValueConverter()},
            { typeof(ushort),           new UShortValueConverter() },
            { typeof(uint),             new UIntValueConverter() },
            { typeof(ulong),            new ULongValueConverter() },
            { typeof(decimal),          new DecimalValueConverter() },
            { typeof(Half),             new HalfValueConverter() },
            { typeof(DateTime),         new DateTimeValueConverter() },
            { typeof(DateTimeOffset),   new DateTimeOffsetValueConverter()},
            { typeof(BigInteger),       new BigIntegerValueConverter() },
            { typeof(Guid),             new GuidValueConverter()},
            { typeof(Version),          new VersionValueConverter() },
            { typeof(TimeSpan),         new TimeSpanValueConverter() },
            { typeof(IPAddress),        new IPAddressValueConverter()},
            { typeof(IntPtr),           new IntPtrValueConverter()},
            { typeof(UIntPtr),          new UIntPtrValueConverter()},
            { typeof(string),           new StringValueConverter() }
        };

        /// <summary>
        /// Gets a <see cref="IValueConverter"/> for the given type or null if not found.
        /// </summary>
        /// <param name="type">Type to get the converter</param>
        /// <returns>A value converter from the given type or null</returns>
        public static IValueConverter? GetConverter(Type type)
        {
            if (type.IsEnum)
            {
                if (!Converters.TryGetValue(type, out object? enumConverter))
                {
                    enumConverter = new EnumObjectValueConverter(type);
                    Converters.Add(type, enumConverter);
                }

                return (EnumObjectValueConverter)enumConverter!;
            }

            if (BuiltInConverters.TryGetValue(type, out var converter))
            {
                return converter;
            }

            return null;
        }
    }
}
