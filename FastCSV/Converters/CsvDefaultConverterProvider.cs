using FastCSV.Utils;
using System;
using System.Collections.Generic;

namespace FastCSV.Converters
{
    internal partial class CsvDefaultConverterProvider : CsvConverterProvider
    {
        private readonly Dictionary<Type, ICsvValueConverter> _converters = new Dictionary<Type, ICsvValueConverter>();
        private readonly ObjectMap Converters = new ObjectMap();

        public CsvDefaultConverterProvider()
        {
            Initialize();
        }

        private partial void Initialize();

        public override ICsvValueConverter? GetConverter(Type type)
        {
            if (type.IsEnum)
            {
                if (!Converters.TryGet(type, out object? enumConverter))
                {
                    enumConverter = new EnumObjectValueConverter(type);
                    Converters.Add(type, enumConverter);
                }

                return (EnumObjectValueConverter)enumConverter!;
            }

            if (_converters.TryGetValue(type, out ICsvValueConverter? converter))
            {
                return converter;
            }

            return null;
        }
    }
}
