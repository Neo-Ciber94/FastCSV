using System;

namespace FastCSV.Converters
{
    /// <summary>
    /// A value converter for an enum of type <c>T</c>.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumValueConverter<TEnum> : IValueConverter<TEnum> where TEnum : struct, Enum
    {
        public string? Read(TEnum value)
        {
            return Enum.GetName(value);
        }

        public bool TryParse(ReadOnlySpan<char> s, out TEnum value)
        {
            return Enum.TryParse(s.ToString(), true, out value);
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

        public string? Read(object? value)
        {
            Type? enumType = value?.GetType();

            if (enumType == null || !CanConvert(enumType))
            {
                return null;
            }

            return Enum.GetName(enumType, value!);
        }

        public bool TryParse(ReadOnlySpan<char> s, out object? value)
        {
            value = null;

            if (s == null)
            {
                return false;
            }

            if (!Enum.TryParse(EnumType, s.ToString(), out value))
            {
                return false;
            }

            return true;
        }

        public bool CanConvert(Type type)
        {
            return EnumType == type;
        }
    }
}
