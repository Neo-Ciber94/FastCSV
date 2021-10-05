using FastCSV.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    internal class ObjectValueConverter : ICsvValueConverter
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(object);
        }

        public bool TryDeserialize(out object? result, Type elementType, ref CsvDeserializeState state)
        {
            string stringValue = state.Read();
            Type? actualType = elementType == typeof(object) ? TypeHelper.GetTypeFromString(stringValue) : elementType;
            result = default!;

            if (actualType != null && actualType != typeof(object))
            {
                var defaultConverter = state.Property?.Converter;
                var converter = CsvConverter.GetConverter(actualType, state.Options, defaultConverter);

                if (converter != null)
                {
                    return converter.TryDeserialize(out result, actualType, ref state);
                }
            }

            return false;
        }

        public bool TrySerialize(object? value, Type elementType, ref CsvSerializeState state)
        {
            if (value == null)
            {
                state.WriteNull();
                return true;
            }

            Type actualType = elementType == typeof(object)? value.GetType(): elementType;
            var converter = CsvConverter.GetConverter(actualType, state.Options);

            if (converter != null && actualType != typeof(object))
            {
                return converter.TrySerialize(value, actualType, ref state);
            }

            return false;
        }
    }
}
