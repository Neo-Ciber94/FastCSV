using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for an enum of type <c>T</c>.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumValueConverter<TEnum> : IValueConverter<TEnum> where TEnum : struct, Enum
    {
        public string? ToValue(TEnum value)
        {
            return Enum.GetName(value);
        }

        public bool TryParse(string? s, out TEnum value)
        {
            return Enum.TryParse(s, true, out value);
        }
    }

    /// <summary>
    /// A value converter for an enum of any type.
    /// </summary>
    public class EnumObjectValueConverter : IValueConverter
    {
        /// <summary>
        /// Type of the enum.
        /// </summary>
        public Type EnumType { get; }

        /// <summary>
        /// Constructs a new <see cref="EnumObjectValueConverter"/>
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        public EnumObjectValueConverter(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{enumType} is not an enum");
            }

            EnumType = enumType;
        }

        public string? ToValue(object? value)
        {
            if (value == null)
            {
                return null;
            }

            Type type = value.GetType();

            if (!type.IsEnum)
            {
                return null;
            }

            return Enum.GetName(type, value);
        }

        public bool TryParse(string? s, out object? value)
        {
            value = null;

            if (s == null || EnumType.IsEnum)
            {
                return false;
            }

            if (!Enum.TryParse(EnumType, s, out value))
            {
                return false;
            }

            return true;
        }
    }
}
