using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    public static class ValueConverters
    {
        private static readonly IReadOnlyDictionary<Type, IValueConverter> Converters = new Dictionary<Type, IValueConverter>
        {
            { typeof(bool), new BoolValueConverter() },
            { typeof(char), new CharValueConverter() },
            { typeof(byte), new ByteValueConverter() },
            { typeof(short), new ShortValueConverter() },
            { typeof(int), new IntValueConverter() },
            { typeof(long), new LongValueConverter() },
            { typeof(float), new FloatValueConverter() },
            { typeof(double), new DoubleValueConverter() },
            { typeof(sbyte), new SByteValueConverter()},
            { typeof(ushort), new UShortValueConverter() },
            { typeof(uint), new UIntValueConverter() },
            { typeof(ulong), new ULongValueConverter() },
            { typeof(decimal), new DecimalValueConverter() },
            { typeof(Half), new HalfValueConverter() },
            { typeof(DateTime),new DateTimeValueConverter() },
            { typeof(DateTimeOffset), new DateTimeOffsetValueConverter()},
            { typeof(BigInteger), new BigIntegerValueConverter() },
            { typeof(Guid), new GuidValueConverter()},
            { typeof(Version), new VersionValueConverter() },
            { typeof(TimeSpan), new TimeSpanValueConverter() },
            { typeof(IPAddress), new IPAddressValueConverter()},
            { typeof(string), new StringValueConverter() }
        };

        public static IValueConverter? GetConverter(Type type)
        {
            if (Converters.TryGetValue(type, out var converter))
            {
                return converter;
            }

            return null;
        }
    }
}
