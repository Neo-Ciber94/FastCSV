using FastCSV.Collections;
using FastCSV.Converters;
using FastCSV.Converters.Internal;
using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace FastCSV
{
    // Determine if a field/property will be a getter, setter of both.
    [Flags]
    internal enum Permission // FIXME: PropertyAccesor
    { 
        /// <summary>
        /// Defines the getter.
        /// </summary>
        Getter = 1,
        /// <summary>
        /// Defines the setter.
        /// </summary>
        Setter = 2,
        /// <summary>
        /// Defines the getter and setter.
        /// </summary>
        GetterAndGetter = Permission.Getter | Permission.Setter
    }

    /// <summary>
    /// Provides a set of utilities for serialize and deserialize csv.
    /// </summary>
    public static partial class CsvConverter
    {
        /// <summary>
        /// Represents a null value in a csv.
        /// </summary>
        public static string Null { get; } = string.Empty;

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

            using ValueList<CsvSerializedProperty> fields = SerializeInternal(value, type, options);
            List<string> csvValues = new List<string>(fields.Length);

            foreach (var f in fields)
            {
                string s = ValueToString(f.Value, f.ElementType, f.Field.Converter);
                csvValues.Add(s);
            }

            string values = CsvUtility.ToCsvString(csvValues, options.Format);

            if (options.IncludeHeader)
            {
                string[] headerArray = new string[fields.Length];

                for (int i = 0; i < fields.Length; i++)
                {
                    headerArray[i] = fields[i].Key;
                }

                string header = CsvUtility.ToCsvString(headerArray, options.Format);
                return CsvUtility.JoinLines(new string[] { header, values });

                //string[] headerArray = GetHeader(type, options);
                //string header = CsvUtility.ToCsvString(headerArray, options.Format);
                //return CsvUtility.JoinLines(new string[] { header, values });
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
                    using MemoryStream stream = StreamHelper.ToMemoryStream(csv);
                    using CsvReader reader = CsvReader.FromStream(stream, options.Format, options.IncludeHeader);
                    CsvRecord? singleRecord = reader.Read();

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

            using ValueList<KeyValuePair<CsvPropertyInfo, object?>> fields = DeserializeInternal(csv, type, options);
            object obj = FormatterServices.GetUninitializedObject(type);

            foreach (var (p, value) in fields)
            {
                p.SetValue(obj, value);
            }

            return obj;
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

            using ValueList<CsvSerializedProperty> csvProps = SerializeInternal(value, type, options);
            Dictionary<string, object?> result = new Dictionary<string, object?>();

            foreach (var p in csvProps)
            {
                result.Add(p.Key, p.Value);
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

            List<CsvPropertyInfo> csvProps = GetCsvProperties(type, options, Permission.Setter, instance: null);
            object result = FormatterServices.GetUninitializedObject(type);

            foreach (var (key, value) in data)
            {
                CsvPropertyInfo? csvProperty = csvProps.FirstOrDefault(f => f.Name == key);

                if (csvProperty == null)
                {
                    throw new InvalidOperationException($"Field '{key}' not found");
                }

                if (csvProperty.Ignore)
                {
                    continue;
                }

                MemberInfo member = csvProperty.Member;
                object? obj = ParseString(value, csvProperty.Type, csvProperty.Converter);
                member.SetValue(result, obj);
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
                return GetCsvProperties(type, options ?? CsvConverterOptions.Default, Permission.Getter, instance: value)
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
                return new string[] { BuiltInTypeHeaderName };
            }

            options ??= CsvConverterOptions.Default;

            if (options.NestedObjectHandling == null)
            {
                return GetCsvProperties(type, options ?? CsvConverterOptions.Default, Permission.Getter, instance: null)
                    .Where(f => !f.Ignore)
                    .Select(f => f.Name)
                    .ToArray();
            }

            List<CsvPropertyInfo> csvProps = GetCsvProperties(type, options ?? CsvConverterOptions.Default, Permission.Getter, instance: null);
            List<string> values = new List<string>(csvProps.Count);
            Stack<CsvPropertyInfo> stack = new Stack<CsvPropertyInfo>(csvProps.Count);
            stack.PushRangeReverse(csvProps);

            while (stack.Count > 0)
            {
                CsvPropertyInfo p = stack.Pop();

                if (p.Ignore)
                {
                    continue;
                }

                if (p.Children.Count > 0)
                {
                    stack.PushRangeReverse(p.Children);
                }
                else
                {
                    values.Add(p.Name);
                }
            }

            return values.ToArray();
        }

        internal static ValueList<CsvSerializedProperty> SerializeInternal(object? value, Type type, CsvConverterOptions options)
        {
            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            if (IsBuiltInType(type))
            {
                throw new ArgumentException($"Cannot serialize the builtin type {type}");
            }

            List<CsvPropertyInfo> csvProps = GetCsvProperties(type, options, Permission.Getter, value);
            bool handleNestedObjects = options.NestedObjectHandling != null;
            bool handleCollections = options.CollectionHandling != null;

            ValueList<CsvSerializedProperty> items = new(csvProps.Count);

            if (handleNestedObjects)
            {
                List<CsvPropertyInfo> temp = new List<CsvPropertyInfo>(csvProps.Count);
                Stack<CsvPropertyInfo> stack = new Stack<CsvPropertyInfo>();

                foreach (var p in csvProps)
                {
                    if (p.Children.Count > 0)
                    {
                        stack.PushRangeReverse(p.Children);
                    }
                    else
                    {
                        temp.Add(p);
                    }

                    while (stack.Count > 0)
                    {
                        CsvPropertyInfo c = stack.Pop();

                        if (c.Children.Count > 0)
                        {
                            stack.PushRangeReverse(c.Children);
                        }
                        else
                        {
                            temp.Add(c);
                        }
                    }
                }

                // Assign the new fields
                csvProps = temp;
            }

            for (int i = 0; i < csvProps.Count; i++)
            {
                CsvPropertyInfo p = csvProps[i];

                if (p.Ignore)
                {
                    continue;
                }

                if (handleCollections && p.Value is IEnumerable enumerable)
                {
                    string itemName = options.CollectionHandling!.ItemName;
                    Type elementType = p.Type.GetCollectionElementType()!;
                    int itemIndex = 0;

                    foreach (var item in enumerable)
                    {
                        items.Add(new CsvSerializedProperty(p, $"{itemName}{++itemIndex}", item, elementType));
                    }
                }
                else
                {
                    items.Add(new CsvSerializedProperty(p, p.Name, p.Value, p.Type));
                }
            }

            return items;
        }

        internal static ValueList<KeyValuePair<CsvPropertyInfo, object?>> DeserializeInternal(string csv, Type type, CsvConverterOptions options)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                throw new ArgumentException("csv cannot be empty", nameof(csv));
            }

            if (IsBuiltInType(type))
            {
                throw new ArgumentException($"Cannot deserialize the builtin type {type}");
            }

            if (!options.IncludeHeader && options.CollectionHandling != null)
            {
                throw new InvalidOperationException($"IncludeHeader must be true when deserializing arrays");
            }

            using MemoryStream stream = StreamHelper.ToMemoryStream(csv);
            using CsvReader reader = CsvReader.FromStream(stream, options.Format, options.IncludeHeader);
            bool handleNestedObjects = options.NestedObjectHandling != null;
            bool handleCollections = options.CollectionHandling != null;

            // SAFETY: should be at least 1 record
            CsvRecord record = reader.Read()!;

            List<CsvPropertyInfo> csvProps = GetCsvProperties(type, options, Permission.Setter, null);
            Stack<object> objs = new Stack<object>();
            Stack<CsvPropertyInfo> props = new Stack<CsvPropertyInfo>();
            Stack<CsvPropertyInfo> parents = new Stack<CsvPropertyInfo>();
            props.PushRangeReverse(csvProps);
            int index = 0;

            ValueList<KeyValuePair<CsvPropertyInfo, object?>> items = new(csvProps.Count);

            while (props.Count > 0)
            {
                CsvPropertyInfo p = props.Pop();

                // Check if the current 'CsvField' is the parent of the last fields
                bool isParent = parents.Count > 0 && object.ReferenceEquals(parents.Peek(), p);

                if (p.Ignore)
                {
                    continue;
                }

                MemberInfo member = p.Member;
                IReadOnlyList<CsvPropertyInfo> children = p.Children;

                if (!isParent && children.Count > 0)
                {
                    // Adds the parent field
                    props.Push(p);
                    parents.Push(p);

                    // Adds all the children
                    props.PushRangeReverse(children);
                    objs.Push(FormatterServices.GetUninitializedObject(p.Type));
                }
                else
                {
                    object? value;

                    if (isParent)
                    {
                        // The object at the top of 'objs' is a field/property of the other object in the stack
                        value = objs.Pop();
                        parents.Pop();
                    }
                    else
                    {
                        if (p.Type.IsCollectionOfElements() && handleCollections)
                        {
                            var collectionDeserializer = CsvCollectionDeserializer.GetConverterForType(p.Type);
                            if (collectionDeserializer == null)
                            {
                                throw new InvalidOperationException($"No deserializer for type {p.Type}");
                            }

                            var deserializedCollection = collectionDeserializer.Convert(record, p, options.CollectionHandling!, index);
                            value = deserializedCollection.Collection;
                            index += deserializedCollection.Length;
                        }
                        else
                        {
                            string csvValue = GetCsvValue(record, p, index++);
                            value = ParseString(csvValue, p.Type, p.Converter);
                        }
                    }

                    if (objs.Count > 0)
                    {
                        object result = objs.Peek();
                        member.SetValue(result, value);
                    }
                    else
                    {
                        items.Add(new KeyValuePair<CsvPropertyInfo, object?>(p, value));
                    }
                }
            }

            return items;

            // Helper

            static string GetCsvValue(CsvRecord record, CsvPropertyInfo field, int index)
            {
                if ((uint)index > (uint)record.Length)
                {
                    throw new InvalidOperationException($"Record value out of range, index was {index} but length was {record.Length}");
                }

                if (record.Header != null && !record.Header.Contains(field.Name))
                {
                    throw new InvalidOperationException($"Cannot find \"{field.Name}\" value in the record");
                }

                return record.Header != null ? record[field.Name] : record[index];
            }
        }

        internal static List<CsvPropertyInfo> GetCsvProperties(Type type, CsvConverterOptions options, Permission permission, object? instance)
        {
            int maxDepth = options.NestedObjectHandling?.MaxDepth ?? 0;
            return GetFieldsInternal(type, options, permission, instance, 0, maxDepth);
        }

        internal static List<CsvPropertyInfo> GetFieldsInternal(Type type, CsvConverterOptions options, Permission permission, object? instance, int depth, int maxDepth)
        {
            // Determines if will handle nested objects
            bool handleNestedObjects = options.NestedObjectHandling != null;

            if (handleNestedObjects && depth > maxDepth)
            {
                throw new InvalidOperationException($"Reference depth exceeded, depth is {depth} but max was {maxDepth}");
            }

            List<CsvPropertyInfo> csvProps;

            var propertyFlags = GetFlagsFromPermission(permission);
            var properties = type.GetProperties(propertyFlags);

            if (options.IncludeFields)
            {
                var fieldFlags = GetFlagsFromPermission(permission);
                var fields = type.GetFields(fieldFlags);

                // Exact size to avoid reallocations
                csvProps = new List<CsvPropertyInfo>(fields.Length + properties.Length);

                if (!fields.Any() && !properties.Any())
                {
                    throw new ArgumentException($"No public fields or properties available for type {type}");
                }

                foreach (var field in fields)
                {
                    CsvPropertyInfo csvProp = CreateCsvProperty(field, options, instance);
                    csvProps.Add(csvProp);

                    if (handleNestedObjects && !IsBuiltInType(field.FieldType) && csvProp.Converter == null)
                    {
                        csvProp.Children = GetFieldsInternal(field.FieldType, options, permission, csvProp.Value, depth + 1, maxDepth);
                    }
                }
            }
            else
            {
                // Exact size to avoid reallocations
                csvProps = new List<CsvPropertyInfo>(properties.Length);

                if (!properties.Any())
                {
                    throw new ArgumentException($"No public properties available for type {type}");
                }
            }

            foreach (var prop in properties)
            {
                CsvPropertyInfo csvProp = CreateCsvProperty(prop, options, instance);
                csvProps.Add(csvProp);

                if (handleNestedObjects && !IsBuiltInType(prop.PropertyType) && csvProp.Converter == null)
                {
                    csvProp.Children = GetFieldsInternal(prop.PropertyType, options, permission, csvProp.Value, depth + 1, maxDepth);
                }
            }

            return csvProps;

            /// Helpers

            static BindingFlags GetFlagsFromPermission(Permission permission)
            {
                var flags = BindingFlags.Public | BindingFlags.Instance;
                switch (permission)
                {
                    case Permission.Getter:
                        flags |= BindingFlags.GetField;
                        break;
                    case Permission.Setter:
                        flags |= BindingFlags.SetField;
                        break;
                    case Permission.GetterAndGetter:
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

            return converter.Read(value) ?? string.Empty;
        }

        internal static IValueConverter? GetValueConverterFromAttribute(CsvValueConverterAttribute? attribute)
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

        internal static CsvPropertyInfo CreateCsvProperty(MemberInfo member, CsvConverterOptions options, object? instance)
        {
            CsvFieldAttribute? fieldAttribute = member.GetCustomAttribute<CsvFieldAttribute>();
            CsvValueConverterAttribute? converterAttribute = member.GetCustomAttribute<CsvValueConverterAttribute>();
            CsvNamingConvention? namingConvention = options.NamingConvention;

            string originalName = member.Name;
            string name = fieldAttribute?.Name ?? namingConvention?.Convert(originalName) ?? originalName;
            Type type = member.GetMemberType();
            object? value = instance != null ? member.GetValue(instance) : null;
            bool ignore = member.GetCustomAttribute<CsvIgnoreAttribute>() != null || member.GetCustomAttribute<NonSerializedAttribute>() != null;
            IValueConverter? converter = GetValueConverterFromAttribute(converterAttribute);

            return new(originalName, name, value, type, member, ignore, converter);
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
