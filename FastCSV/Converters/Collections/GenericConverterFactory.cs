using FastCSV.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters.Collections
{
    internal static class GenericConverterFactory
    {
        public static ICsvValueConverter Create(Type converterGenericDefinition, Type elementType)
        {
            Debug.Assert(converterGenericDefinition.IsGenericTypeDefinition);
            Debug.Assert(!elementType.IsGenericType);

            var converterType = converterGenericDefinition.MakeGenericType(elementType);
            var constructor = converterType.GetConstructor(Type.EmptyTypes);

            if (!converterGenericDefinition.IsAssignableToClass(typeof(CsvCollectionConverter<,>)))
            {
                throw new InvalidOperationException($"{converterType} don't implement {typeof(CsvCollectionConverter<,>)}");
            }

            if (constructor == null)
            {
                throw new InvalidOperationException($"{converterGenericDefinition} don't contains a parameless constructor");
            }

            return (ICsvValueConverter)constructor.Invoke(null);
        }
    }
}
