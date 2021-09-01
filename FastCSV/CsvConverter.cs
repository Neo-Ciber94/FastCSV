﻿namespace FastCSV
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Reflection;
    using System.Runtime.Serialization;
    using FastCSV.Utils;

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
    public static class CsvConverter
    {
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
        public static string Serialize(object value, Type type, CsvConverterOptions? options = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.GetType() != type)
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            options ??= CsvConverterOptions.Default;

            List<CsvField> fields = GetFields(type, options, Permission.Read, value);
            string[] csvValues = fields.Where(e => !e.Ignore)
                .Select(e => e.Value?.ToString() ?? string.Empty)
                .ToArray();

            string values = CsvUtility.ToCsvString(csvValues, options.Format);

            if (options.IncludeHeader)
            {
                string[] headerArray = GetHeader(type, options);
                string header = CsvUtility.ToCsvString(headerArray, options.Format);
                return CsvUtility.Join(new string[] { header, values });
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
            return (T)Deserialize(csv, typeof(T), options);
        }

        /// <summary>
        /// Deserialize the given csv string into a object of the given type.
        /// </summary>
        /// <param name="csv">Csv to convert to a value.</param>
        /// <param name="type">Type of the resulting value.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>The resulting value using the csv.</returns>
        public static object Deserialize(string csv, Type type, CsvConverterOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("csv cannot be empty", nameof(csv));
            }

            if (IsBuiltInType(type))
            {
                if (!CsvUtility.TryParse(csv, type, out object? result))
                {
                    throw new InvalidCastException($"Cannot convert '{csv}' to {type}");
                }

                return result!;
            }

            options ??= CsvConverterOptions.Default;

            using MemoryStream stream = CsvUtility.ToStream(csv);
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

                if (source.IsLeft)
                {
                    FieldInfo field = source.Left;
                    string csvValue = GetCsvValue(record, csvField, i);

                    if (!CsvUtility.TryParse(csvValue, csvField.Type, out var value))
                    {
                        throw new InvalidOperationException($"Cannot convert '{csvValue}' to '{ csvField.Type}'");
                    }

                    field.SetValue(obj, value);
                }
                else
                {
                    PropertyInfo prop = source.Right;
                    string csvValue = GetCsvValue(record, csvField, i);

                    if (!CsvUtility.TryParse(csvValue, csvField.Type, out var value))
                    {
                        throw new InvalidOperationException($"Cannot convert '{csvValue}' to '{ csvField.Type}'");
                    }

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
        /// Returns an array of the field values of the given object.
        /// </summary>
        /// <param name="value">The object to get the values.</param>
        /// <param name="options">Options used, if null will use the default options.</param>
        /// <returns>An array of the values of the given object.</returns>
        public static string[] GetValues(object value, CsvConverterOptions? options = null)
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

        /// <summary>
        /// The GetBuiltInTypeName.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
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
                _ => type.Name.ToLower()
            };
        }

        /// <summary>
        /// The IsBuiltInType.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        internal static bool IsBuiltInType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(decimal)
                || type == typeof(string)
                || type == typeof(BigInteger)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(IPAddress)
                || type == typeof(Version)
                || type == typeof(Guid);
        }

        /// <summary>
        /// The GetFields.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/>.</param>
        /// <param name="options">The options<see cref="CsvConverterOptions"/>.</param>
        /// <param name="permission">The permission<see cref="Permission"/>.</param>
        /// <param name="instance">The instance<see cref="object?"/>.</param>
        /// <returns>The <see cref="List{CsvField}"/>.</returns>
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
                    string originalName = field.Name;
                    string name = field.GetCustomAttribute<CsvFieldAttribute>()?.Name ?? originalName;
                    Type fieldType = field.FieldType;
                    object? fieldValue = instance != null ? field.GetValue(instance) : null;
                    bool ignore = field.GetCustomAttribute<CsvIgnoreAttribute>() != null || field.GetCustomAttribute<NonSerializedAttribute>() != null;

                    CsvField csvField = new(originalName, name, fieldValue, fieldType, Either.FromLeft(field), ignore);
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
                string originalName = prop.Name;
                string name = prop.GetCustomAttribute<CsvFieldAttribute>()?.Name ?? originalName;
                Type propType = prop.PropertyType;
                object? propValue = instance != null ? prop.GetValue(instance) : null;
                bool ignore = prop.GetCustomAttribute<CsvIgnoreAttribute>() != null || prop.GetCustomAttribute<NonSerializedAttribute>() != null;

                CsvField csvField = new(originalName, name, propValue, propType, Either.FromRight(prop), ignore);
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
    }
}