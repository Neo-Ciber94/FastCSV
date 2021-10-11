using System;
using System.Reflection;

namespace FastCSV.Extensions
{
    internal static class ReflectionExtensions
    {
        public static object? GetPropertyValue<T>(this T obj, string propertyName, BindingFlags bindingFlags = BindingFlags.GetProperty | BindingFlags.Instance)
        {
            Type type = typeof(T);
            PropertyInfo? property = type.GetProperty(propertyName, bindingFlags);

            if (property == null)
            {
                throw new InvalidOperationException($"Cannot find getter property: {propertyName}");
            }

            return property.GetValue(obj);
        }

        public static void SetPropertyValue<T>(this T obj, string propertyName, object? value, BindingFlags bindingFlags = BindingFlags.SetProperty | BindingFlags.Instance)
        {
            Type type = typeof(T);
            PropertyInfo? property = type.GetProperty(propertyName, bindingFlags);

            if (property == null)
            {
                throw new InvalidOperationException($"Cannot find setter property: {propertyName}");
            }

            property.SetValue(obj, value);
        }

        public static object? GetFieldValue<T>(this T obj, string fieldName, BindingFlags bindingFlags = BindingFlags.GetField | BindingFlags.Instance)
        {
            Type type = typeof(T);
            FieldInfo? field = type.GetField(fieldName, bindingFlags);

            if (field == null)
            {
                throw new InvalidOperationException($"Cannot find field: {fieldName}");
            }

            return field.GetValue(obj);
        }

        public static void SetFieldValue<T>(this T obj, string fieldName, object? value, BindingFlags bindingFlags = BindingFlags.SetField | BindingFlags.Instance)
        {
            Type type = typeof(T);
            FieldInfo? field = type.GetField(fieldName, bindingFlags);

            if (field == null)
            {
                throw new InvalidOperationException($"Cannot find field: {fieldName}");
            }

            field.SetValue(obj, value);
        }
    }
}
