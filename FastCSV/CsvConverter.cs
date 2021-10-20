using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FastCSV.Collections;
using FastCSV.Converters;
using FastCSV.Converters.Collections;
using FastCSV.Internal;
using FastCSV.Utils;
using FastCSV.Extensions;

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

        /// Header for any type that have a converter
        private const string PlainTypeHeaderName = "value";

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
            if (value != null && type == typeof(object))
            {
                type = value.GetType();
            }

            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            options ??= CsvConverterOptions.Default;
            string values;

            if (HasConverter(type, options))
            {
                CollectionHandling? collectionHandling = options.CollectionHandling;
                CsvSerializeState state = new(options, value, buffer: new List<string>(1));
                ValueToString(type, ref state, null);

                values = CsvUtility.ToCsvString(state.Serialized, options.Format);

                if (options.IncludeHeader)
                {
                    if (type.IsEnumerableType())
                    {
                        if (collectionHandling == null)
                        {
                            throw ThrowHelper.CollectionHandlingRequired();
                        }
     
                        string itemName = collectionHandling.Tag;
                        int count = state.Serialized.Count;
                        string[] headerArray = new string[count];

                        for (int i = 0; i < headerArray.Length; i++)
                        {
                            headerArray[i] = $"{itemName}{i + 1}";
                        }

                        string header = CsvUtility.ToCsvString(headerArray, options.Format);
                        return CsvUtility.JoinLines(new string[] { header, values });
                    }
                    else
                    {
                        return $"{PlainTypeHeaderName}{Environment.NewLine}{values}";
                    }
                }

                return values;
            }

            using ValueList<DataToSerialize> props = SerializeInternal(value, type, options);
            var buffer = new List<string>(props.Length);
            int index = 0;

            while(index < props.Length)
            {
                DataToSerialize p = props[index];
                CsvProperty prop = p.Property;
                object? obj = prop.Value;
                Type elementType = prop.Type;
                ICsvValueConverter? converter = GetConverter(elementType, options, prop.Converter);
                CsvSerializeState state = new(options, value, buffer, converter);

                if (converter == null || !converter.CanConvert(elementType) || !converter.TrySerialize(obj, elementType, ref state))
                {
                    throw ThrowHelper.CannotSerializeToType(obj, elementType);
                }

                int serializedCount = state.Serialized.Count;

                // If nothing was serialized move to the next value to avoid overflow
                index = index == serializedCount ? index + 1 : serializedCount;
            }

            values = CsvUtility.ToCsvString(buffer, options.Format);

            if (options.IncludeHeader)
            {
                string[] headerArray = new string[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    headerArray[i] = props[i].ColumnName;
                }

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

            if (HasConverter(type, options))
            {
                CsvDeserializeState state;

                if (options.IncludeHeader)
                {
                    using Stream stream = StreamHelper.CreateStreamFromString(csv);
                    using CsvReader reader = CsvReader.FromStream(stream, options.Format, options.IncludeHeader);
                    CsvRecord? record = reader.Read();

                    if (record == null)
                    {
                        state = new CsvDeserializeState(options, type, CsvConverter.Null);
                        return ParseString(type, ref state);
                    }

                    if (!type.IsEnumerableType() && record.Length != 1)
                    {
                        throw new InvalidOperationException($"Expected 1 single field");
                    }

                    ReadOnlyMemory<string> recordValues = record.AsMemory();
                    state = new CsvDeserializeState(options, type, recordValues);
                    return ParseString(type, ref state);
                }
                else
                {
                    state = new CsvDeserializeState(options, type, csv);
                    return ParseString(type, ref state);
                }
            }

            using ValueList<DataToDeserialize> props = DeserializeInternal(csv, type, options);
            object obj = FormatterServices.GetUninitializedObject(type);

            foreach (var p in props)
            {
                CsvProperty csvProperty = p.Property;

                if (!csvProperty.IsReadOnly)
                {
                    var value = p.Value;
                    csvProperty.SetValue(obj, value);
                }
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

            if (HasConverter(type, options))
            {
                return new Dictionary<string, object?> { { PlainTypeHeaderName, value } };
            }

            using ValueList<DataToSerialize> csvProps = SerializeInternal(value, type, options);
            Dictionary<string, object?> result = new();

            foreach (var p in csvProps)
            {
                result.Add(p.ColumnName, p.Value);
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
        public static T DeserializeFromDictionary<T>(IReadOnlyDictionary<string, SingleOrList<string>> data, CsvConverterOptions? options = null)
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
        public static object? DeserializeFromDictionary(IReadOnlyDictionary<string, SingleOrList<string>> data, Type type, CsvConverterOptions? options = null)
        {
            options ??= CsvConverterOptions.Default;

            if (HasConverter(type, options))
            {
                if (!data.TryGetValue(PlainTypeHeaderName, out SingleOrList<string> v) || data.Count != 1)
                {
                    throw new ArgumentException($"Type '{nameof(data)}' should contains a header named '{PlainTypeHeaderName}'");
                }

                if (v.Count == 0)
                {
                    return null;
                }

                if (v.Count == 1)
                {
                    CsvDeserializeState state = new CsvDeserializeState(options, type, v[0]);
                    return ParseString(type, ref state)!;
                }
                else
                {
                    CsvDeserializeState state = new CsvDeserializeState(options, type, v.AsMemory());
                    return ParseString(type, ref state)!;
                }
            }

            List<CsvProperty> csvProps = GetCsvProperties(type, options, Permission.Setter, instance: null);
            object result = FormatterServices.GetUninitializedObject(type);

            foreach ((string key, SingleOrList<string> value) in data)
            {
                CsvProperty? property = csvProps.FirstOrDefault(f => f.Name == key);

                if (property == null)
                {
                    if (options.MatchExact)
                    {
                        throw new InvalidOperationException($"Field '{key}' not found");
                    }
                    else
                    {
                        continue;
                    }
                }

                if (property.Ignore)
                {
                    continue;
                }

                MemberInfo member = property.Member;
                Type propertyType = property.Type;
                object? obj = null;

                if (value.Count > 0)
                {
                    if (value.Count == 1)
                    {
                        CsvDeserializeState state = new(options, propertyType, value[0]);
                        obj = ParseString(propertyType, ref state);
                    }
                    else
                    {
                        CsvDeserializeState state = new(options, propertyType, value.AsMemory());
                        obj = ParseString(propertyType, ref state);
                    }
                }

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
        /// <returns>An array of the header values of the given type.</returns>
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
                return new string[] { PlainTypeHeaderName };
            }

            options ??= CsvConverterOptions.Default;

            if (options.NestedObjectHandling == null)
            {
                return GetCsvProperties(type, options ?? CsvConverterOptions.Default, Permission.Getter, instance: null)
                    .Where(f => !f.Ignore)
                    .Select(f => f.Name)
                    .ToArray();
            }

            List<CsvProperty> csvProps = GetCsvProperties(type, options ?? CsvConverterOptions.Default, Permission.Getter, instance: null);
            List<string> values = new List<string>(csvProps.Count);
            Stack<CsvProperty> stack = new Stack<CsvProperty>(csvProps.Count);
            stack.PushRangeReverse(csvProps);

            while (stack.Count > 0)
            {
                CsvProperty p = stack.Pop();

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

        private static ValueList<DataToSerialize> SerializeInternal(object? value, Type type, CsvConverterOptions options)
        {
            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            if (IsBuiltInType(type))
            {
                throw new ArgumentException($"Cannot serialize the builtin type {type}");
            }

            List<CsvProperty> csvProps = GetCsvProperties(type, options, Permission.Getter, value);
            bool handleNestedObjects = options.NestedObjectHandling != null;
            bool handleCollections = options.CollectionHandling != null;

            ValueList<DataToSerialize> items = new(csvProps.Count);

            if (handleNestedObjects)
            {
                List<CsvProperty> temp = new List<CsvProperty>(csvProps.Count);
                Stack<CsvProperty> stack = new Stack<CsvProperty>();

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
                        CsvProperty c = stack.Pop();

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
                CsvProperty property = csvProps[i];

                if (property.Ignore)
                {
                    continue;
                }

                if (handleCollections && property.Value is IEnumerable enumerable)
                {
                    string itemName = options.CollectionHandling!.Tag;
                    int itemIndex = 0;

                    foreach (object? item in enumerable)
                    {
                        items.Add(new DataToSerialize(property, $"{itemName}{++itemIndex}", item));
                    }
                }
                else if (handleCollections && property.Value is ITuple tuple)
                {
                    string itemName = options.CollectionHandling!.Tag;
                    int itemIndex = 0;

                    while (itemIndex < tuple.Length)
                    {
                        var item = tuple[itemIndex];
                        items.Add(new DataToSerialize(property, $"{itemName}{++itemIndex}", item));
                    }
                }
                else
                {
                    items.Add(new DataToSerialize(property, property.Name, property.Value));
                }
            }

            return items;
        }

        private static ValueList<DataToDeserialize> DeserializeInternal(string csv, Type type, CsvConverterOptions options)
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
                throw new InvalidOperationException("IncludeHeader must be true when deserializing arrays");
            }

            using Stream stream = StreamHelper.CreateStreamFromString(csv);
            using CsvReader reader = CsvReader.FromStream(stream, options.Format, options.IncludeHeader);
            bool handleNestedObjects = options.NestedObjectHandling != null;
            bool handleCollections = options.CollectionHandling != null;

            CsvRecord? record = reader.Read();

            if (record == null)
            {
                return default;
            }

            List<CsvProperty> csvProps = GetCsvProperties(type, options, Permission.Setter, null);

            if (options.MatchExact && record.Header != null)
            {
                foreach (string headerElement in record.Header)
                {
                    bool anyMatch = false;

                    foreach (var prop in csvProps)
                    {
                        if (prop.Name == headerElement)
                        {
                            anyMatch = true;
                            break;
                        }
                    }

                    if (!anyMatch)
                    {
                        throw new InvalidOperationException($"Field '{headerElement}' not found");
                    }
                }
            }

            Stack<object> objs = new Stack<object>();
            Stack<CsvProperty> props = new Stack<CsvProperty>();
            Stack<CsvProperty> parents = new Stack<CsvProperty>();
            props.PushRangeReverse(csvProps);

            ValueList<DataToDeserialize> items = new(csvProps.Count);
            int index = 0;

            while (props.Count > 0)
            {
                CsvProperty property = props.Pop();

                // Check if the current 'CsvField' is the parent of the last fields
                bool isParent = parents.Count > 0 && object.ReferenceEquals(parents.Peek(), property);

                if (property.Ignore)
                {
                    continue;
                }

                MemberInfo member = property.Member;
                IReadOnlyList<CsvProperty> children = property.Children;

                if (!isParent && children.Count > 0)
                {
                    // Adds the parent field
                    props.Push(property);
                    parents.Push(property);

                    // Adds all the children
                    props.PushRangeReverse(children);
                    objs.Push(FormatterServices.GetUninitializedObject(property.Type));
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
                        if (property.Type.IsEnumerableType() && handleCollections)
                        {
                            ReadOnlyMemory<string> recordValues = ReadCollectionFromRecord(record, index, options.CollectionHandling!);
                            ICsvValueConverter? collectionConverter = GetConverter(property.Type, options, property.Converter);

                            if (collectionConverter == null)
                            {
                                throw new InvalidOperationException($"No found deserializer for type {property.Type}");
                            }

                            var state = new CsvDeserializeState(options, property, recordValues);
                            if (!collectionConverter.TryDeserialize(out object? collection, state.ElementType, ref state))
                            {
                                var s = CsvUtility.ToCsvString(record[index..].ToArray(), options.Format);
                                throw new InvalidOperationException($"Can not convert '{s}' collection to {property.Type}");
                            }

                            value = collection;
                            index += recordValues.Length;
                        }
                        else
                        {
                            string csvValue = GetCsvValue(record, property, index++);
                            var state = new CsvDeserializeState(options, property, csvValue);
                            value = ParseString(property.Type, ref state, property.Converter);
                        }
                    }

                    if (objs.Count > 0)
                    {
                        object result = objs.Peek();
                        member.SetValue(result, value);
                    }
                    else
                    {
                        items.Add(new DataToDeserialize(property, value));
                    }
                }
            }

            return items;

            // Helper

            static string GetCsvValue(CsvRecord record, CsvProperty property, int index)
            {
                if ((uint)index > (uint)record.Length)
                {
                    throw new InvalidOperationException($"Record value out of range, index was {index} but length was {record.Length}");
                }

                if (record.Header != null && !record.Header.Contains(property.Name))
                {
                    throw new InvalidOperationException($"Cannot find \"{property.Name}\" value in the record");
                }

                return record.Header != null ? record[property.Name] : record[index];
            }
        }

        private static ReadOnlyMemory<string> ReadCollectionFromRecord(CsvRecord record, int startIndex, CollectionHandling collectionHandling)
        {
            var header = record.Header;

            if (header == null)
            {
                throw new InvalidOperationException("Cannot read a collection without a header");
            }

            if (collectionHandling == null)
            {
                throw ThrowHelper.CollectionHandlingRequired();
            }

            string itemName = collectionHandling.Tag;
            int count = 0;
            int index = startIndex;

            while(index < record.Length)
            {
                string headerItem = header[index];

                if (!headerItem.StartsWith(itemName))
                {
                    break;
                }

                ReadOnlySpan<char> itemString = headerItem.AsSpan(itemName.Length);

                if (!int.TryParse(itemString, out int itemIndex))
                {
                    break;
                }

                if (itemIndex != (count + 1) && itemIndex != 1)
                {
                    throw new InvalidOperationException($"Invalid item, expected {itemName}{count + 1} but was {itemName}{index}");
                }

                // This means the next item is the start of a new array
                if (count > 0 && itemIndex == 1)
                {
                    break;
                }

                count += 1;
                index += 1;
            }

            return record.AsMemory().Slice(startIndex, count);
        }

        private static List<CsvProperty> GetCsvProperties(Type type, CsvConverterOptions options, Permission permission, object? instance)
        {
            int maxDepth = options.NestedObjectHandling?.MaxDepth ?? 0;
            return GetCsvPropertiesInternal(type, options, permission, instance, 0, maxDepth);
        }

        private static List<CsvProperty> GetCsvPropertiesInternal(Type type, CsvConverterOptions options, Permission permission, object? instance, int depth, int maxDepth)
        {
            // Determines if will handle nested objects
            bool handleNestedObjects = options.NestedObjectHandling != null;

            if (handleNestedObjects && depth > maxDepth)
            {
                throw new InvalidOperationException($"Reference depth exceeded, depth is {depth} but max was {maxDepth}");
            }

            List<CsvProperty> csvProps;

            var propertyFlags = GetFlagsFromPermission(permission);
            var properties = type.GetProperties(propertyFlags);

            if (options.IncludeFields)
            {
                var fieldFlags = GetFlagsFromPermission(permission);
                var fields = type.GetFields(fieldFlags);

                // Exact size to avoid reallocations
                csvProps = new List<CsvProperty>(fields.Length + properties.Length);

                if (!fields.Any() && !properties.Any())
                {
                    throw new ArgumentException($"No public fields or properties available for type {type}");
                }

                foreach (var field in fields)
                {
                    CsvProperty csvProp = CreateCsvProperty(field, options, instance);
                    csvProps.Add(csvProp);

                    if (handleNestedObjects && !IsBuiltInType(field.FieldType) && csvProp.Converter == null)
                    {
                        csvProp.Children = GetCsvPropertiesInternal(field.FieldType, options, permission, csvProp.Value, depth + 1, maxDepth);
                    }
                }
            }
            else
            {
                // Exact size to avoid reallocations
                csvProps = new List<CsvProperty>(properties.Length);

                if (!properties.Any())
                {
                    throw new ArgumentException($"No public properties available for type {type}");
                }
            }

            foreach (var prop in properties)
            {
                CsvProperty csvProp = CreateCsvProperty(prop, options, instance);
                csvProps.Add(csvProp);

                if (handleNestedObjects && !IsBuiltInType(prop.PropertyType) && csvProp.Converter == null)
                {
                    csvProp.Children = GetCsvPropertiesInternal(prop.PropertyType, options, permission, csvProp.Value, depth + 1, maxDepth);
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

        private static object? ParseString(Type type, ref CsvDeserializeState state, ICsvValueConverter? converter = null)
        {
            if (state.Count == 0)
            {
                return null;
            }

            if (type.IsNullable())
            {
                if (state.Count == 1) 
                {
                    string s = state.Read();

                    if (string.IsNullOrEmpty(s))
                    {
                        return null;
                    }
                }

                type = Nullable.GetUnderlyingType(type)!;
            }

            converter = GetConverter(type, state.Options, converter);

            if (converter == null || !converter.CanConvert(type) || !converter.TryDeserialize(out object? value, type, ref state))
            {
                throw ThrowHelper.CannotDeserializeToType(state.Values.ToArray(), type);
            }

            return value;
        }

        private static void ValueToString(Type type, ref CsvSerializeState state, ICsvValueConverter? converter = null)
        {
            object? value = state.Value;

            if (value != null && !EqualTypes(value.GetType(), type))
            {
                throw new ArgumentException($"Type missmatch, expected {type} but was {value.GetType()}");
            }

            if (type.IsNullable())
            {
                if (value == null)
                {
                    state.WriteNull();
                    return;
                }

                type = Nullable.GetUnderlyingType(type)!;
            }

            converter = GetConverter(type, state.Options, converter);

            if (converter == null || !converter.CanConvert(type) || !converter.TrySerialize(value, type, ref state))
            {
                throw new InvalidOperationException($"No converter found for type {type}");
            }
        }

        /// Convenient method to try get a converter for the given type
        internal static ICsvValueConverter? GetConverter(Type elementType, CsvConverterOptions options, ICsvValueConverter? defaultConverter = null)
        {
            if (defaultConverter != null && defaultConverter.CanConvert(elementType))
            {
                return defaultConverter;
            }

            // Prioritize custom converters
            if (options.Converters.Any())
            {
                ICsvValueConverter? customConverter = options.Converters.FirstOrDefault(e => e.CanConvert(elementType));
                if (customConverter != null)
                {
                    return customConverter;
                }
            }
            
            if (options.CollectionHandling != null && elementType.IsEnumerableType())
            {
                var collectionConverter = CsvCollectionConverterProvider.Default.GetConverter(elementType);
                if (collectionConverter != null)
                {
                    return collectionConverter;
                }
            }

            return options.ConverterProvider.GetConverter(elementType);
        }

        private static ICsvCustomConverter? GetValueConverterFromAttribute(CsvValueConverterAttribute? attribute)
        {
            if (attribute == null || attribute.ConverterType == null)
            {
                return null;
            }

            Type converterType = attribute.ConverterType;

            if (!typeof(ICsvCustomConverter).IsAssignableFrom(converterType))
            {
                throw new ArgumentException($"Type {converterType} does not implements {typeof(ICsvCustomConverter)}");
            }

            ConstructorInfo? constructor = converterType.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException($"No parameless public constructor available for converter {converterType}");
            }

            object converter = constructor.Invoke(Array.Empty<object>());
            return (ICsvCustomConverter)converter;
        }

        private static CsvProperty CreateCsvProperty(MemberInfo member, CsvConverterOptions options, object? instance)
        {
            CsvFieldAttribute? fieldAttribute = member.GetCustomAttribute<CsvFieldAttribute>();
            CsvValueConverterAttribute? converterAttribute = member.GetCustomAttribute<CsvValueConverterAttribute>();
            CsvNamingConvention? namingConvention = options.NamingConvention;

            string originalName = member.Name;
            string name = fieldAttribute?.Name ?? namingConvention?.Convert(originalName) ?? originalName;
            Type type = member.GetMemberType();
            object? value = instance != null ? member.GetValue(instance) : null;
            bool ignore = member.GetCustomAttribute<CsvIgnoreAttribute>() != null || member.GetCustomAttribute<NonSerializedAttribute>() != null;
            ICsvCustomConverter? converter = GetValueConverterFromAttribute(converterAttribute);

            return new(originalName, name, value, type, member, ignore, converter);
        }

        private static bool HasConverter(Type type, CsvConverterOptions options)
        {
            if (type.IsGenericType && type.IsNullable())
            {
                type = Nullable.GetUnderlyingType(type)!;
            }

            return GetConverter(type, options) != null;
        }

        /*
         * Check if both types are equals. Nullable types are considered equals to its non-nullable variant:
         * 
         * EqualTypes(typeof(Nullable<int>), typeof(int)) == true
         */
        private static bool EqualTypes(Type leftType, Type rightType)
        {
            if (leftType.IsGenericType && leftType.IsNullable())
            {
                leftType = Nullable.GetUnderlyingType(leftType)!;
            }

            if (rightType.IsGenericType && rightType.IsNullable())
            {
                rightType = Nullable.GetUnderlyingType(rightType)!;
            }

            return leftType == rightType;
        }
    }
}
