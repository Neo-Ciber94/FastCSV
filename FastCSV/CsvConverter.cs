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
using FastCSV.Extensions;
using FastCSV.Internal;
using FastCSV.Utils;

namespace FastCSV
{
    // Determine if a field/property will be a getter, setter of both.
    internal enum PropertyAccesor
    {
        /// <summary>
        /// Defines the getter.
        /// </summary>
        Getter = 1,
        /// <summary>
        /// Defines the setter.
        /// </summary>
        Setter = 2,
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

            options ??= CsvConverterOptions.Default;

            if (value != null && !EqualTypes(value.GetType(), type, options))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

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
                        int count = state.Count;
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

            using ValueList<DataToSerialize> props = GetSerializeData(value, type, options);
            var buffer = new List<string>(props.Length);
            int index = 0;

            while (index < props.Length)
            {
                DataToSerialize p = props[index];
                CsvNode node = p.Node;
                object? obj = node.Value;
                Type elementType = node.Info.Type;
                ICsvValueConverter? converter = GetConverter(elementType, options, node.Info.Converter);
                CsvSerializeState state = new(options, value, buffer, converter);

                if (converter == null || !converter.CanConvert(elementType) || !converter.TrySerialize(obj, elementType, ref state))
                {
                    throw ThrowHelper.CannotSerializeToType(obj, elementType);
                }

                int serializedCount = state.Count;

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
        public static object? Deserialize(ReadOnlySpan<char> csv, Type type, CsvConverterOptions? options = null)
        {
            if (csv.IsEmpty || csv.IsWhiteSpace())
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
                    using CsvReader reader = new CsvReader(stream, options.Format, options.IncludeHeader);
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

                    ReadOnlySpan<string> recordValues = record.AsSpan();
                    state = new CsvDeserializeState(options, type, recordValues);
                    return ParseString(type, ref state);
                }
                else
                {
                    state = new CsvDeserializeState(options, type, csv);
                    return ParseString(type, ref state);
                }
            }
            else
            {
                using Stream stream = StreamHelper.CreateStreamFromString(csv);
                using CsvReader reader = new CsvReader(stream, options.Format, options.IncludeHeader);
                CsvRecord? record = reader.Read(options.Format);

                if (record == null)
                {
                    return null;
                }

                return DeserializeFromRecord(record, type, options);
            }
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

            options ??= CsvConverterOptions.Default;
            Type type = value!.GetType();

            if (HasConverter(type, options))
            {
                return new string[] { ConvertObject(type, value, options) };
            }
            else if (type.IsEnumerableType())
            {
                if (typeof(ITuple).IsAssignableFrom(type))
                {
                    ITuple tuple = (ITuple)value;
                    string[] result = new string[tuple.Length];

                    for (int i = 0; i < tuple.Length; i++)
                    {
                        result[i] = ConvertObject(tuple.GetType(), tuple[i], options);
                    }

                    return result;
                }
                else
                {
                    IEnumerable<object> enumerable = ((IEnumerable)value).Cast<object>();
                    List<string> result = new(enumerable.Count());

                    foreach (object? item in enumerable)
                    {
                        if (item == null)
                        {
                            result.Add(CsvConverter.Null);
                        }
                        else
                        {
                            result.Add(ConvertObject(item.GetType(), item, options));
                        }
                    }

                    return result.ToArray();
                }
            }
            else
            {
                List<string> result = new List<string>();
                IEnumerable<CsvNode> nodes = GetCsvNodes(type, options, PropertyAccesor.Getter, value).Where(e => !e.Info.Ignore);

                foreach(CsvNode node in nodes)
                {
                    result.Add(ConvertObject(node.Info.Type, node.Value, options));
                }

                return result.ToArray();
            }

            static string ConvertObject(Type type, object? obj, CsvConverterOptions options)
            {
                if (obj == null)
                {
                    return CsvConverter.Null;
                }

                CsvSerializeState state = new(options, obj, new List<string>());
                ValueToString(type, ref state);
                return state.GetSerializedValue(0);
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
                return GetCsvNodes(type, options ?? CsvConverterOptions.Default, PropertyAccesor.Getter, instance: null)
                    .Where(f => !f.Info.Ignore)
                    .Select(f => f.Name)
                    .ToArray();
            }

            IReadOnlyList<CsvNode> csvNodes = GetCsvNodes(type, options ?? CsvConverterOptions.Default, PropertyAccesor.Getter, instance: null);
            List<string> values = new List<string>(csvNodes.Count);
            Stack<CsvNode> stack = new Stack<CsvNode>(csvNodes.Count);
            stack.PushRangeReverse(csvNodes);

            while (stack.Count > 0)
            {
                CsvNode node = stack.Pop();

                if (node.Info.Ignore)
                {
                    continue;
                }

                if (node.Children.Count > 0)
                {
                    stack.PushRangeReverse(node.Children);
                }
                else
                {
                    values.Add(node.Name);
                }
            }

            return values.ToArray();
        }

        internal static T DeserializeFromRecord<T>(CsvRecord record, CsvConverterOptions? options = null)
        {
            return (T)DeserializeFromRecord(record, typeof(T), options)!;
        }

        internal static object? DeserializeFromRecord(CsvRecord record, Type type, CsvConverterOptions? options = null)
        {
            options ??= CsvConverterOptions.Default;

            using ValueList<DataToDeserialize> dataToDeserialize = GetDeserializeData(record, type, options);
            object obj = FormatterServices.GetUninitializedObject(type);

            foreach (var data in dataToDeserialize)
            {
                CsvPropertyInfo csvProperty = data.Property;

                if (!csvProperty.IsReadOnly)
                {
                    var value = data.Value;
                    csvProperty.SetValue(obj, value);
                }
            }

            return obj;
        }

        private static object? ParseString(Type type, ref CsvDeserializeState state, ICsvValueConverter? converter = null)
        {
            if (state.Count == 0)
            {
                return null;
            }

            var reflector = state.Options.ReflectionProvider;

            if (reflector.IsNullableType(type))
            {
                if (state.Count == 1)
                {
                    ReadOnlySpan<char> s = state.Read();

                    if (s.IsEmpty || s.IsWhiteSpace())
                    {
                        return null;
                    }
                }

                type = reflector.GetNullableType(type)!;
            }

            converter = GetConverter(type, state.Options, converter);

            if (converter == null || !converter.CanConvert(type) || !converter.TryDeserialize(out object? value, type, ref state))
            {
                throw ThrowHelper.CannotDeserializeToType(state.Values.ToArray(), type);
            }

            return value;
        }

        private static IEnumerable<string> ValueToString(Type type, ref CsvSerializeState state, ICsvValueConverter? converter = null)
        {
            object? value = state.Value;

            if (value != null && !EqualTypes(value.GetType(), type, state.Options))
            {
                throw new ArgumentException($"Type missmatch, expected {type} but was {value.GetType()}");
            }

            var reflector = state.Options.ReflectionProvider;
            if (reflector.IsNullableType(type))
            {
                if (value == null)
                {
                    state.WriteNull();
                    return state.Serialized;
                }

                type = reflector.GetNullableType(type)!;
            }

            converter = GetConverter(type, state.Options, converter);

            if (converter == null || !converter.CanConvert(type) || !converter.TrySerialize(value, type, ref state))
            {
                throw new InvalidOperationException($"No converter found for type {type}");
            }

            return state.Serialized;
        }

        private static ValueList<DataToSerialize> GetSerializeData(object? value, Type type, CsvConverterOptions options)
        {
            if (value != null && !EqualTypes(value.GetType(), type, options))
            {
                throw new ArgumentException($"Expected {type} but value type was {value.GetType()}");
            }

            if (IsBuiltInType(type))
            {
                throw new ArgumentException($"Cannot serialize the builtin type {type}");
            }

            IReadOnlyList<CsvNode> csvNodes = GetCsvNodes(type, options, PropertyAccesor.Getter, value);
            bool handleNestedObjects = options.NestedObjectHandling != null;
            bool handleCollections = options.CollectionHandling != null;

            ValueList<DataToSerialize> items = new(csvNodes.Count);

            if (handleNestedObjects)
            {
                List<CsvNode> temp = new List<CsvNode>(csvNodes.Count);
                Stack<CsvNode> stack = new Stack<CsvNode>();

                foreach (CsvNode node in csvNodes)
                {
                    if (node.Children.Count > 0)
                    {
                        stack.PushRangeReverse(node.Children);
                    }
                    else
                    {
                        temp.Add(node);
                    }

                    while (stack.Count > 0)
                    {
                        CsvNode c = stack.Pop();

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
                csvNodes = temp;
            }

            for (int i = 0; i < csvNodes.Count; i++)
            {
                CsvNode node = csvNodes[i];

                if (node.Info.Ignore)
                {
                    continue;
                }

                if (handleCollections && node.Value is IEnumerable enumerable)
                {
                    string itemName = options.CollectionHandling!.Tag;
                    int itemIndex = 0;

                    foreach (object? item in enumerable)
                    {
                        items.Add(new DataToSerialize(node, $"{itemName}{++itemIndex}", item));
                    }
                }
                else if (handleCollections && node.Value is ITuple tuple)
                {
                    string itemName = options.CollectionHandling!.Tag;
                    int itemIndex = 0;

                    while (itemIndex < tuple.Length)
                    {
                        object? item = tuple[itemIndex];
                        items.Add(new DataToSerialize(node, $"{itemName}{++itemIndex}", item));
                    }
                }
                else
                {
                    items.Add(new DataToSerialize(node, node.Name, node.Value));
                }
            }

            return items;
        }

        private static ValueList<DataToDeserialize> GetDeserializeData(CsvRecord record, Type type, CsvConverterOptions options)
        {
            if (IsBuiltInType(type))
            {
                throw new ArgumentException($"Cannot deserialize the builtin type {type}");
            }

            if (!options.IncludeHeader && options.CollectionHandling != null)
            {
                throw new InvalidOperationException("IncludeHeader must be true when deserializing arrays");
            }

            bool handleCollections = options.CollectionHandling != null;

            if (record == null)
            {
                return default;
            }

            IReadOnlyList<CsvNode> csvNodes = GetCsvNodes(type, options, PropertyAccesor.Setter, null);

            if (options.MatchExact && record.Header != null)
            {
                foreach (string headerElement in record.Header)
                {
                    bool anyMatch = false;

                    foreach (var prop in csvNodes)
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
            Stack<CsvNode> props = new Stack<CsvNode>();
            Stack<CsvNode> parents = new Stack<CsvNode>();
            props.PushRangeReverse(csvNodes);

            string?[] toDeserialize = record.Header?.ToArray() ?? Array.Empty<string?>();
            ValueList<DataToDeserialize> items = new(csvNodes.Count);
            int index = 0;

            while (props.Count > 0)
            {
                CsvNode node = props.Pop();

                // Check if the current 'CsvField' is the parent of the last fields
                bool isParent = parents.Count > 0 && object.ReferenceEquals(parents.Peek(), node);

                if (node.Info.Ignore)
                {
                    continue;
                }

                MemberInfo member = node.Info.Member;
                IReadOnlyList<CsvNode> children = node.Children;

                if (!isParent && children.Count > 0)
                {
                    // Adds the parent field
                    props.Push(node);
                    parents.Push(node);

                    // Adds all the children
                    props.PushRangeReverse(children);
                    objs.Push(FormatterServices.GetUninitializedObject(node.Info.Type));
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
                        if (node.Info.Type.IsEnumerableType() && handleCollections)
                        {
                            ReadOnlySpan<string> recordValues = ReadCollectionFromRecord(record, index, options.CollectionHandling!);
                            ICsvValueConverter? collectionConverter = GetConverter(node.Info.Type, options, node.Info.Converter);

                            if (collectionConverter == null)
                            {
                                throw new InvalidOperationException($"No found deserializer for type {node.Info.Type}");
                            }

                            var state = new CsvDeserializeState(options, node.Info, recordValues);
                            if (!collectionConverter.TryDeserialize(out object? collection, state.ElementType, ref state))
                            {
                                var s = CsvUtility.ToCsvString(record[index..].ToArray(), options.Format);
                                throw new InvalidOperationException($"Can not convert '{s}' collection to {node.Info.Type}");
                            }

                            value = collection;
                            index += recordValues.Length;
                        }
                        else
                        {
                            string csvValue = GetCsvValue(record, node, toDeserialize, index++);
                            var state = new CsvDeserializeState(options, node.Info, csvValue);
                            value = ParseString(node.Info.Type, ref state, node.Info.Converter);
                        }
                    }

                    if (objs.Count > 0)
                    {
                        object result = objs.Peek();
                        member.SetValue(result, value);
                    }
                    else
                    {
                        items.Add(new DataToDeserialize(node.Info, value));
                    }
                }
            }


            return items;

            // Helper

            static string GetCsvValue(CsvRecord record, CsvNode property, string?[] toDeserialize, int index)
            {
                if ((uint)index > (uint)record.Length)
                {
                    throw new InvalidOperationException($"Record value out of range, index was {index} but length was {record.Length}");
                }

                if (record.Header != null && !record.Header.Contains(property.Name))
                {
                    throw new InvalidOperationException($"Cannot find \"{property.Name}\" value in the record");
                }

                if (record.Header != null)
                {
                    // The property was already deserialized
                    if (!toDeserialize.Contains(property.Name))
                    {
                        throw new InvalidOperationException($"Cannot find \"{property.Name}\" value in the record");
                    }

                    int valueIndex = Array.IndexOf(toDeserialize, property.Name);
                    toDeserialize[valueIndex] = null;
                    return record[valueIndex];
                }
                else
                {
                    return record[index];
                }
            }
        }

        private static ReadOnlySpan<string> ReadCollectionFromRecord(CsvRecord record, int startIndex, CollectionHandling collectionHandling)
        {
            CsvHeader? header = record.Header;

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

            while (index < record.Length)
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

            return record.AsSpan().Slice(startIndex, count);
        }

        private static IReadOnlyList<CsvNode> GetCsvNodes(Type type, CsvConverterOptions options, PropertyAccesor accesor, object? instance, CsvNode? parent = null)
        {
            int maxDepth = options.NestedObjectHandling?.MaxDepth ?? 0;
            return GetCsvNodesInternal(type, options, accesor, instance, 0, maxDepth, parent);
        }

        private static IReadOnlyList<CsvNode> GetCsvNodesInternal(Type type, CsvConverterOptions options, PropertyAccesor accesor, object? instance, int depth, int maxDepth, CsvNode? parent)
        {
            // Check depth
            if (options.NestedObjectHandling != null && depth > maxDepth)
            {
                throw new InvalidOperationException($"Reference depth exceeded, depth is {depth} but max was {maxDepth}");
            }

            List<CsvNode> nodes;
            IReflector reflector = options.ReflectionProvider;
            BindingFlags propertyFlags = GetFlagsFromPropertyAccesor(accesor);
            IReadOnlyCollection<PropertyInfo> properties = reflector.GetProperties(type, propertyFlags);

            if (options.IncludeFields)
            {
                BindingFlags fieldFlags = GetFlagsFromPropertyAccesor(accesor);
                IReadOnlyCollection<FieldInfo> fields = reflector.GetFields(type, fieldFlags);

                // Exact size to avoid reallocations
                nodes = new List<CsvNode>(fields.Count + properties.Count);

                if (!fields.Any() && !properties.Any())
                {
                    throw new ArgumentException($"No public fields or properties available for type {type}");
                }

                // Set the nodes from the type fields
                SetNodes(options, accesor, instance, depth, maxDepth, parent, nodes, fields);
            }
            else
            {
                // Exact size to avoid reallocations
                nodes = new List<CsvNode>(properties.Count);

                if (!properties.Any())
                {
                    throw new ArgumentException($"No public properties available for type {type}");
                }
            }

            // Set the nodes from the type properties
            SetNodes(options, accesor, instance, depth, maxDepth, parent, nodes, properties);

            return nodes;

            /// Helpers

            static void SetNodes<TMember>(CsvConverterOptions options, PropertyAccesor accesor, object? instance, int depth, int maxDepth, CsvNode? parent, List<CsvNode> nodes, IReadOnlyCollection<TMember> members) where TMember : MemberInfo
            {
                NestedObjectHandling? nestedObjectHandling = options.NestedObjectHandling;
                bool handleNestedObjects = nestedObjectHandling != null;

                foreach (TMember member in members)
                {
                    Type memberType = member.GetMemberType();
                    CsvNode csvNode = CreateCsvNode(member, options, instance);

                    if (handleNestedObjects && !IsBuiltInType(memberType) && csvNode.Info.Converter == null)
                    {
                        csvNode.Parent = parent;

                        if (DetectReferenceLoop(csvNode))
                        {
                            /*
                             * Currently we are not able to deserialize types with multiple reference loops, 
                             * because is not possible to know how deep the loop goes.
                             * 
                             * For example:
                             * 
                             * Given the type:
                             *      record Node(int Value, Node? Left, Node? Right)
                             *      
                             * And the csv: 
                             *      Value,Value,Value,Value
                             *      1,    3,    20,   5
                             *      
                             * We can't decide when the 'Value' represents the Left or Right node.
                             */
                            if (accesor == PropertyAccesor.Setter)
                            {
                                throw new NotSupportedException($"Cannot set value to member '{member.Name}' due reference loop");
                            }

                            switch (nestedObjectHandling!.ReferenceLoopHandling)
                            {
                                case ReferenceLoopHandling.Error:
                                    throw new InvalidOperationException($"Reference loop detected in member '{csvNode.Info.OriginalName}'");
                                case ReferenceLoopHandling.Ignore:
                                    continue;
                                case ReferenceLoopHandling.Serialize:
                                    {
                                        if (csvNode.Value == null)
                                        {
                                            continue;
                                        }
                                    }
                                    break;
                            }
                        }

                        csvNode.Children = GetCsvNodesInternal(memberType, options, accesor, csvNode.Value, depth + 1, maxDepth, csvNode);
                    }

                    // Adds the node
                    nodes.Add(csvNode);
                }
            }

            static BindingFlags GetFlagsFromPropertyAccesor(PropertyAccesor permission)
            {
                var flags = BindingFlags.Public | BindingFlags.Instance;
                switch (permission)
                {
                    case PropertyAccesor.Getter:
                        flags |= BindingFlags.GetField;
                        break;
                    case PropertyAccesor.Setter:
                        flags |= BindingFlags.SetField;
                        break;
                }

                return flags;
            }

            static bool DetectReferenceLoop(CsvNode node)
            {
                if (node.Parent == null)
                {
                    return false;
                }

                Type type = node.Type;
                CsvNode? current = node;

                while (current != null)
                {
                    Type? parentDeclaringType = current.Info.Member.DeclaringType;

                    if (parentDeclaringType == null)
                    {
                        return false;
                    }

                    if (parentDeclaringType == type)
                    {
                        return true;
                    }

                    current = current.Parent;
                }

                return false;
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

        private static CsvNode CreateCsvNode(MemberInfo member, CsvConverterOptions options, object? instance)
        {
            IReflector reflector = options.ReflectionProvider;
            CsvFieldAttribute? fieldAttribute = reflector.GetMemberCustomAttribute<CsvFieldAttribute>(member);
            CsvNamingConvention? namingConvention = options.NamingConvention;

            CsvPropertyInfo property = reflector.GetCsvProperty(member, options);
            string originalName = property.OriginalName;
            string name = fieldAttribute?.Name ?? namingConvention?.Convert(originalName) ?? originalName;
            object? value = instance != null ? member.GetValue(instance) : null;
            return new CsvNode(property, name, value);
        }

        internal static CsvPropertyInfo CreateCsvPropertyInfo(MemberInfo member, CsvConverterOptions options)
        {
            IReflector reflector = options.ReflectionProvider;
            CsvValueConverterAttribute? converterAttribute = reflector.GetMemberCustomAttribute<CsvValueConverterAttribute>(member);

            string originalName = member.Name;
            Type type = member.GetMemberType();
            bool ignore = reflector.GetMemberCustomAttribute<CsvIgnoreAttribute>(member) != null || reflector.GetMemberCustomAttribute<NonSerializedAttribute>(member) != null;
            ICsvCustomConverter? converter = GetValueConverterFromAttribute(converterAttribute, options);

            return new(originalName, type, member, ignore, converter);
        }

        private static ICsvCustomConverter? GetValueConverterFromAttribute(CsvValueConverterAttribute? attribute, CsvConverterOptions options)
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

            ConstructorInfo? constructor = options.ReflectionProvider.GetConstructor(converterType, Type.EmptyTypes);

            if (constructor == null)
            {
                throw new InvalidOperationException($"No parameless public constructor available for converter {converterType}");
            }

            object converter = constructor.Invoke(Array.Empty<object>());
            return (ICsvCustomConverter)converter;
        }

        private static bool HasConverter(Type type, CsvConverterOptions options)
        {
            var reflector = options.ReflectionProvider;

            if (type.IsGenericType && reflector.IsNullableType(type))
            {
                type = reflector.GetNullableType(type)!;
            }

            return GetConverter(type, options) != null;
        }

        /*
         * Check if both types are equals. Nullable types are considered equals to its non-nullable variant:
         * 
         * EqualTypes(typeof(Nullable<int>), typeof(int)) == true
         */
        private static bool EqualTypes(Type leftType, Type rightType, CsvConverterOptions options)
        {
            var reflector = options.ReflectionProvider;
            if (leftType.IsGenericType && reflector.IsNullableType(leftType))
            {
                leftType = reflector.GetNullableType(leftType)!;
            }

            if (rightType.IsGenericType && reflector.IsNullableType(rightType))
            {
                rightType = reflector.GetNullableType(rightType)!;
            }

            return leftType == rightType;
        }
    }
}
