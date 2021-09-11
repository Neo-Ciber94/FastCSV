using System;
using System.Reflection;

namespace FastCSV.Converters
{
    /// <summary>
    /// Provides an access to the values of a <see cref="Nullable{T}"/>.
    /// </summary>
    internal readonly struct NullableObject
    {
        private const string HasValuePropertyName = nameof(Nullable<bool>.HasValue);
        private const string ValuePropertyName = nameof(Nullable<bool>.Value);

        private readonly object? value;
        private readonly bool hasValue;

        public object Value => hasValue ? value! : throw new InvalidOperationException("no value");

        public bool HasValue => hasValue;

        public NullableObject(object? value)
        {
            this.hasValue = value != null;
            this.value = value;
        }

        public static NullableObject FromNullable(object nullableValue)
        {
            if (nullableValue == null)
            {
                throw new ArgumentNullException(nameof(nullableValue));
            }

            Type type = nullableValue.GetType();

            if (!IsNullableType(type))
            {
                throw new ArgumentException($"{nullableValue} is no a nullable type");
            }

            PropertyInfo hasValueProperty = type.GetProperty(HasValuePropertyName)!;
            bool hasValue = (bool)hasValueProperty.GetValue(nullableValue)!;
            object? value = null;

            if (hasValue)
            {
                PropertyInfo valueProperty = type.GetProperty(ValuePropertyName)!;
                value = valueProperty.GetValue(nullableValue);

                return new NullableObject(value!);
            }
            else
            {
                return default;
            }
        }

        public static bool IsNullableType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
