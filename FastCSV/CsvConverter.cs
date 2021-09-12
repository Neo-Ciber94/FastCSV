using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FastCSV.Converters;
using FastCSV.Utils;

namespace FastCSV
{
    // Determine if a field/property will be a getter, setter of both.
    internal enum Permission
    { /// <summary>
      /// Defines the Read.
      /// </summary>
        Read,
        /// <summary>
        /// Defines the Write.
        /// </summary>
        Write,
        /// <summary>
        /// Defines the ReadWrite.
        /// </summary>
        ReadWrite
    }

    /// <summary>
    /// Provides a set of utilities for serialize and deserialize csv.
    /// </summary>
    public static partial class CsvConverter
    {
        private const string BuiltInTypeHeaderName = "value";

        /// <summary>
        /// Serialize the given value to a csv.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The value to convert to csv.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>A csv string of the value.</returns>
        public static string Serialize<T>(T value, CsvConverterOptions? options = null)
        {
            return Serialize(value!, typeof(T), options);
        }

        /// <summary>
        /// Serialize the given value to a csv.
        /// </summary>
        /// <param name="value">The value to convert to csv.</param>
        /// <param name="type">Type of the value.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>A csv string of the value.</returns>
        public static string Serialize(object? value, Type type, CsvConverterOptions? options = null)
        {
            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            options ??= CsvConverterOptions.Default;

            if (IsBuiltInType(type))
            {
                if (options.IncludeHeader)
                {
                    string s = ValueToString(value, type, null);
                    return $"{BuiltInTypeHeaderName}\n{s}";
                }
                else
                {
                    return ValueToString(value, type, null);
                }
            }

            List<CsvField> fields = GetFields(type, options, Permission.Read, value);
            string[] csvValues = fields.Where(f => !f.Ignore)
                .Select((f) => ValueToString(f.Value, f.Type, f.Converter))
                .ToArray();

            string values = CsvUtility.ToCsvString(csvValues, options.Format);

            if (options.IncludeHeader)
            {
                string[] headerArray = GetHeader(type, options);
                string header = CsvUtility.ToCsvString(headerArray, options.Format);
                return CsvUtility.JoinLines(new string[] { header, values });
            }

            return values;
        }

        /// <summary>
        /// Deserialize the given csv string into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the resulting value.</typeparam>
        /// <param name="csv">Csv to convert to a value.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>The resulting value using the csv.</returns>
        public static T Deserialize<T>(string csv, CsvConverterOptions? options = null)
        {
            return (T)Deserialize(csv, typeof(T), options)!;
        }

        /// <summary>
        /// Deserialize the given csv string into a object of the given type.
        /// </summary>
        /// <param name="csv">Csv to convert to a value.</param>
        /// <param name="type">Type of the resulting value.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>The resulting value using the csv.</returns>
        public static object? Deserialize(string csv, Type type, CsvConverterOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("csv cannot be empty", nameof(csv));
            }

            options ??= CsvConverterOptions.Default;

            if (IsBuiltInType(type))
            {
                if (options.IncludeHeader)
                {
                    using MemoryStream stream2 = StreamHelper.ToMemoryStream(csv);
                    using CsvReader reader2 = CsvReader.FromStream(stream2, options.Format, options.IncludeHeader);
                    CsvRecord? singleRecord = reader2.Read();
                    
                    if (singleRecord == null)
                    {
                        return ParseString(null, type);
                    }

                    if (singleRecord.Length != 1)
                    {
                        throw new InvalidOperationException($"Expected 1 single field");
                    }

                    string s = singleRecord[0];
                    return ParseString(s, type);
                }
                else
                {
                    return ParseString(csv, type);
                }
            }

            using MemoryStream stream = StreamHelper.ToMemoryStream(csv);
            using CsvReader reader = CsvReader.FromStream(stream, options.Format, options.IncludeHeader);

            // SAFETY: should be at least 1 record
            CsvRecord record = reader.Read()!;

            List<CsvField> csvFields = GetFields(type, options, Permission.Write, null);
            object obj = FormatterServices.GetUninitializedObject(type);

            for (int i = 0; i < csvFields.Count; i++)
            {
                CsvField csvField = csvFields[i];

                if (csvField.Ignore)
                {
                    continue;
                }

                Either<FieldInfo, PropertyInfo> source = csvField.Source;
                string csvValue = GetCsvValue(record, csvField, i);
                object? value = ParseString(csvValue, csvField.Type, csvField.Converter);

                if (source.IsLeft)
                {
                    FieldInfo field = source.Left;
                    field.SetValue(obj, value);
                }
                else
                {
                    PropertyInfo prop = source.Right;
                    prop.SetValue(obj, value);
                }
            }

            return obj;

            // Helper

            static string GetCsvValue(CsvRecord record, CsvField field, int index)
            {
                if((uint)index > (uint)record.Length)
                {
                    throw new InvalidOperationException($"Record value out of range, index was {index} but length was {record.Length}");
                }

                if (record.Header != null && !record.Header.Contains(field.Name))
                {
                    throw new InvalidOperationException($"Cannot find \"{field.Name}\" value in the record");
                }

                return record.Header != null? record[field.Name]: record[index];
            }
        }

        /// <summary>
        /// Converts the given value to a <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>A dictionary with the fields of the value.</returns>
        public static IDictionary<string, object?> SerializeToDictionary<T>(T value, CsvConverterOptions? options = null)
        {
            return SerializeToDictionary(value!, typeof(T), options);
        }

        /// <summary>
        /// Converts the given value to a <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>A dictionary with the fields of the value.</returns>
        public static IDictionary<string, object?> SerializeToDictionary(object value, Type type, CsvConverterOptions? options = null)
        {
            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            options ??= CsvConverterOptions.Default;

            if (!options.IncludeHeader)
            {
                throw new ArgumentException("Invalid options, header should always be include");
            }

            if (IsBuiltInType(type))
            {
                return new Dictionary<string, object?> { { BuiltInTypeHeaderName, value } };
            }

            Dictionary<string, object?> result = new Dictionary<string, object?>();
            List<CsvField> fields = GetFields(type, options, Permission.Read, value);

            foreach(CsvField csvField in fields)
            {
                if (csvField.Ignore)
                {
                    continue;
                }

                string key = csvField.Name;
                object? obj = csvField.Value;
                result.Add(key, obj);
            }

            return result;
        }

        /// <summary>
        /// Converts the data in the given dictionary to an object of type <c>T</c>.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="data">The dictionary containing the data of the instance.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>An object of the given type using the values from the dictionary.</returns>
        public static T DeserializeFromDictionary<T>(IReadOnlyDictionary<string, string> data, CsvConverterOptions? options = null)
        {
            return (T)DeserializeFromDictionary(data, typeof(T), options)!;
        }

        /// <summary>
        /// Converts the data in the given dictionary to an object of the given type.
        /// </summary>
        /// <param name="data">The dictionary containing the data of the instance.</param>
        /// <param name="type">Type of the object to construct.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>An object of the given type using the values from the dictionary.</returns>
        public static object? DeserializeFromDictionary(IReadOnlyDictionary<string, string> data, Type type, CsvConverterOptions? options = null)
        {
            if (IsBuiltInType(type))
            {
                if (!data.TryGetValue(BuiltInTypeHeaderName, out string? v) || data.Count != 1)
                {
                    throw new ArgumentException($"For builtin type '{nameof(data)}' should contains a single field named '{BuiltInTypeHeaderName}'");
                }

                return ParseString(v, type)!;
            }

            options ??= CsvConverterOptions.Default;
            List<CsvField> csvFields = GetFields(type, options, Permission.Write, instance: null);
            object result = FormatterServices.GetUninitializedObject(type);

            foreach(var (key, value) in data)
            {
                CsvField? csvField = csvFields.FirstOrDefault(f => f.Name == key);

                if (csvField == null)
                {
                    throw new InvalidOperationException($"Field '{key}' not found");
                }

                if (csvField.Ignore)
                {
                    continue;
                }

                Either<FieldInfo, PropertyInfo> source = csvField.Source;
                object? obj = ParseString(value, csvField.Type, csvField.Converter);

                if (source.IsLeft)
                {
                    FieldInfo field = source.Left;
                    field.SetValue(result, obj);
                }
                else
                {
                    PropertyInfo prop = source.Right;
                    prop.SetValue(result, obj);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an array of the field values of the given object.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The object to get the values.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>An array of the values of the given object.</returns>
        public static string[] GetValues<T>(T value, CsvConverterOptions? options = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Type type = value!.GetType();

            if (IsBuiltInType(type))
            {
                return new string[] { value?.ToString() ?? string.Empty };
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return ((IEnumerable)value).Cast<object>()
                    .Select(e => e.ToString() ?? string.Empty)
                    .ToArray();
            }
            else
            {
                return GetFields(type, options ?? CsvConverterOptions.Default, Permission.Read, instance: value)
                    .Where(e => !e.Ignore)
                    .Select(e => e.Value?.ToString() ?? string.Empty)
                    .ToArray();
            }
        }

        /// <summary>
        /// Returns an array of the header names of the given type. 
        /// </summary>
        /// <typeparam name="T">The type to get the header values from.</typeparam>
        /// <param name="type">Type to extract the header fields.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>An array of the header values of the given type.</return
        public static string[] GetHeader<T>(CsvConverterOptions? options = null)
        {
            return GetHeader(typeof(T), options);
        }
 
        /// <summary>
        /// Returns an array of the header names of the given type.
        /// </summary>
        /// <param name="type">Type to extract the header fields.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>An array of the header values of the given type.</returns>
        public static string[] GetHeader(Type type, CsvConverterOptions? options = null)
        {
            if (IsBuiltInType(type))
            {
                return new string[] { GetBuiltInTypeName(type) };
            }

            return GetFields(type, options ?? CsvConverterOptions.Default, Permission.Read, instance: null)
                .Where(e => !e.Ignore)
                .Select(e => e.Name)
                .ToArray();
        }

        internal static string GetBuiltInTypeName(Type type)
        {
            return type switch
            {
                Type t when t == typeof(bool) => "bool",
                Type t when t == typeof(char) => "char",
                Type t when t == typeof(byte) => "byte",
                Type t when t == typeof(short) => "short",
                Type t when t == typeof(int) => "int",
                Type t when t == typeof(long) => "long",
                Type t when t == typeof(float) => "float",
                Type t when t == typeof(double) => "double",
                Type t when t == typeof(decimal) => "decimal",
                Type t when t == typeof(sbyte) => "sbyte",
                Type t when t == typeof(ushort) => "ushort",
                Type t when t == typeof(uint) => "uint",
                Type t when t == typeof(ulong) => "ulong",
                Type t when t == typeof(string) => "string",
                _ => type.Name
            };
        }

        internal static List<CsvField> GetFields(Type type, CsvConverterOptions options, Permission permission, object? instance)
        {
            List<CsvField> csvFields;

            var propertyFlags = GetFlagsFromPermission(permission);
            var properties = type.GetProperties(propertyFlags);

            if (options.IncludeFields)
            {
                var fieldFlags = GetFlagsFromPermission(permission);
                var fields = type.GetFields(fieldFlags);

                // Exact size to avoid reallocations
                csvFields = new List<CsvField>(fields.Length + properties.Length);

                if (!fields.Any() && !properties.Any())
                {
                    throw new ArgumentException($"No public fields or properties available for type {type}");
                }

                foreach (var field in fields)
                {
                    CsvField csvField = CreateCsvField(new PropertyOrField(field), options, instance);
                    csvFields.Add(csvField);
                }
            }
            else
            {
                // Exact size to avoid reallocations
                csvFields = new List<CsvField>(properties.Length);

                if (!properties.Any())
                {
                    throw new ArgumentException($"No public properties available for type {type}");
                }
            }

            foreach (var prop in properties)
            {                
                CsvField csvField = CreateCsvField(new PropertyOrField(prop), options, instance);
                csvFields.Add(csvField);
            }

            return csvFields;

            /// Helpers

            static BindingFlags GetFlagsFromPermission(Permission permission)
            {
                var flags = BindingFlags.Public | BindingFlags.Instance;
                switch (permission)
                {
                    case Permission.Read:
                        flags |= BindingFlags.GetField;
                        break;
                    case Permission.Write:
                        flags |= BindingFlags.SetField;
                        break;
                    case Permission.ReadWrite:
                        flags |= BindingFlags.GetField | BindingFlags.SetField;
                        break;
                }

                return flags;
            }
        }

        internal static object? ParseString(string? s, Type type, IValueConverter? converter = null)
        {
            if (NullableObject.IsNullableType(type))
            {
                if (string.IsNullOrEmpty(s))
                {
                    return null;
                }

                type = Nullable.GetUnderlyingType(type)!;
            }

            converter ??= ValueConverters.GetConverter(type);

            if (converter == null || !converter.CanConvert(type) || !converter.TryParse(s, out object? value))
            {
                throw new InvalidOperationException($"Cannot convert '{s}' to '{type}'");
            }

            return value;
        }

        internal static string ValueToString(object? value, Type type, IValueConverter? converter = null)
        {
            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Type missmatch, expected {type} but was {value.GetType()}");
            }

            if (NullableObject.IsNullableType(type))
            {
                var nullable = new NullableObject(value);

                if (nullable.HasValue)
                {
                    value = nullable.Value;
                    type = value.GetType();
                }
                else
                {
                    return string.Empty;
                }
            }

            converter ??= ValueConverters.GetConverter(type);

            if (converter == null || !converter.CanConvert(type))
            {
                throw new InvalidOperationException($"No converter found for type {type}");
            }

            return converter.ToStringValue(value) ?? string.Empty;
        }

        internal static IValueConverter? GetValueConverter(CsvValueConverterAttribute? attribute)
        {
            if (attribute == null || attribute.ConverterType == null)
            {
                return null;
            }

            Type converterType = attribute.ConverterType;
            
            if (!typeof(IValueConverter).IsAssignableFrom(converterType))
            {
                throw new ArgumentException($"Type {converterType} does not implements {typeof(IValueConverter)}");
            }

            ConstructorInfo? constructor = converterType.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException($"No parameless public constructor available for converter {converterType}");
            }

            object converter = constructor.Invoke(Array.Empty<object>());
            return (IValueConverter)converter;
        }

        internal static CsvField CreateCsvField(PropertyOrField propertyOrField, CsvConverterOptions options, object? instance)
        {
            CsvFieldAttribute? fieldAttribute = propertyOrField.GetCustomAttribute<CsvFieldAttribute>();
            CsvValueConverterAttribute? converterAttribute = propertyOrField.GetCustomAttribute<CsvValueConverterAttribute>();
            CsvNamingConvention? namingConvention = options.NamingConvention;

            string originalName = propertyOrField.Name;
            string name = fieldAttribute?.Name ?? namingConvention?.Convert(originalName) ?? originalName;
            Type fieldType = propertyOrField.Type;
            object? fieldValue = instance != null ? propertyOrField.GetValue(instance) : null;
            bool ignore = propertyOrField.GetCustomAttribute<CsvIgnoreAttribute>() != null || propertyOrField.GetCustomAttribute<NonSerializedAttribute>() != null;
            IValueConverter? converter = GetValueConverter(converterAttribute);
            
            Either<FieldInfo, PropertyInfo> source = propertyOrField switch
            {
                var p when p.IsProperty => Either.FromRight(propertyOrField.Property!),
                var f when f.IsField => Either.FromLeft(propertyOrField.Field!),
                _ => throw new InvalidOperationException()
            };

            return new(originalName, name, fieldValue, fieldType, source, ignore, converter);
        }

        internal static bool EqualTypes(Type leftType, Type rightType)
        {
            if (NullableObject.IsNullableType(leftType))
            {
                leftType = Nullable.GetUnderlyingType(leftType)!;
            }

            if (NullableObject.IsNullableType(rightType))
            {
                rightType = Nullable.GetUnderlyingType(rightType)!;
            }

            return leftType == rightType;
        }
    }
}
