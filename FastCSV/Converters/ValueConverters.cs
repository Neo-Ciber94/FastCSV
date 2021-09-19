using System;
using System.Collections;
using FastCSV.Utils;

namespace FastCSV.Converters
{
    /// <summary>
    /// Helper for <see cref="IValueConverter"/>.
    /// </summary>
    public static partial class ValueConverters
    {
        private static readonly ObjectMap Converters = new ObjectMap();

        /// <summary>
        /// Gets a <see cref="IValueConverter"/> for the given type or null if not found.
        /// </summary>
        /// <param name="type">Type to get the converter</param>
        /// <returns>A value converter from the given type or null</returns>
        public static IValueConverter? GetConverter(Type type)
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

            if (BuiltInConverters.TryGetValue(type, out var converter))
            {
                return converter;
            }

            return null;
        }
    }
}
