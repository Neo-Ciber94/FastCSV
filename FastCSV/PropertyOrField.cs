using System;
using System.Reflection;

namespace FastCSV
{
    /// <summary>
    /// Convenient wrapper around <see cref="PropertyInfo"/> and <see cref="FieldInfo"/> to reduce code duplication.
    /// </summary>
    internal readonly struct PropertyOrField
    {
        private const string NeitherTypeExceptionMessage = "Value is neither a property or field";

        private readonly object _value;

        public PropertyOrField(PropertyInfo prop)
        {
            _value = prop;
        }

        public PropertyOrField(FieldInfo field)
        {
            _value = field;
        }

        public bool IsProperty => _value is PropertyInfo;

        public bool IsField => _value is FieldInfo;

        public PropertyInfo? Property => _value as PropertyInfo;

        public FieldInfo? Field => _value as FieldInfo;

        public string Name => Property?.Name ?? Field?.Name ?? throw new InvalidOperationException(NeitherTypeExceptionMessage);

        public Type Type => Property?.PropertyType ?? Field?.FieldType ?? throw new InvalidOperationException(NeitherTypeExceptionMessage);

        public TAttribute? GetCustomAttribute<TAttribute>() where TAttribute : Attribute
        {
            if (IsProperty)
            {
                return Property!.GetCustomAttribute<TAttribute>();
            }

            if (IsField)
            {
                return Field!.GetCustomAttribute<TAttribute>();
            }

            throw new InvalidOperationException(NeitherTypeExceptionMessage);
        }

        public object? GetValue(object obj)
        {
            if (IsField)
            {
                return Field!.GetValue(obj);
            }

            if (IsProperty)
            {
                return Property!.GetValue(obj);
            }

            throw new InvalidOperationException(NeitherTypeExceptionMessage);
        }

        public void SetValue(object? obj, object? value)
        {
            if (IsField)
            {
                Field!.SetValue(obj, value);
                return;
            }

            if (IsProperty)
            {
                Property!.SetValue(obj, value);
                return;
            }

            throw new InvalidOperationException(NeitherTypeExceptionMessage);
        }
    }
}
