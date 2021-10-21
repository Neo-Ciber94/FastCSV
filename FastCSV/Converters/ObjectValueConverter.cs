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
            result = default!;
            var stringValue = state.Read();
            CsvConverterOptions options = state.Options;

            Type? actualType = GuessTypeOrNull(stringValue, options.TypeGuessers);

            if (actualType == null)
            {
                if (elementType == typeof(object))
                {
                    // Fallback to string if not type was found
                    actualType = TypeHelper.GetTypeFromString(stringValue) ?? typeof(string);
                }
                else
                {
                    actualType = elementType;
                }
            }

            if (actualType != null && actualType != typeof(object))
            {
                ICsvValueConverter? defaultConverter = state.Property?.Converter;
                ICsvValueConverter? converter = CsvConverter.GetConverter(actualType, options, defaultConverter);

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

        private static Type? GuessTypeOrNull(ReadOnlySpan<char> s, IReadOnlyList<ITypeGuesser> typeGuessers)
        {
            if (typeGuessers.Count > 0)
            {
                foreach (var guesser in typeGuessers)
                {
                    Type? type = guesser.GetTypeFromString(s);

                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
