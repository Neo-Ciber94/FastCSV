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
        /// <summary>
        /// Creates a <see cref="ICsvValueConverter"/> for collections from the given types.
        /// 
        /// <para>
        /// The <paramref name="converterGenericDefinition"/> must be a generic type that takes as generic
        /// the type of the collection elements.
        /// </para>
        /// </summary>
        /// <param name="converterGenericDefinition">Type of the converter to create.</param>
        /// <param name="elementType">Type of the element the collection store.</param>
        /// <returns></returns>
        public static ICsvValueConverter CreateCollectionConverter(Type converterGenericDefinition, Type elementType)
        {
            Debug.Assert(elementType.IsGenericType && converterGenericDefinition.IsGenericTypeDefinition);
            Debug.Assert(!elementType.IsGenericType);

            if (converterGenericDefinition.GetGenericArguments().Length != 1)
            {
                throw new InvalidOperationException($"Type {converterGenericDefinition} must only take 1 generic argument.");
            }

            var converterType = converterGenericDefinition.MakeGenericType(elementType);
            var constructor = converterType.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException($"{converterGenericDefinition} don't contains a parameless constructor");
            }

            return (ICsvValueConverter)constructor.Invoke(null);
        }
    }
}
