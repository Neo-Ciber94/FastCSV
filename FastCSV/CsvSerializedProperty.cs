﻿using System;
using System.Collections;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Resulting data of a serialization.
    /// 
    /// <para/>
    /// This is required for special cases for example serializing an array or enumerable,
    /// the actual <see cref="CsvPropertyInfo"/> type will be the array but the actual serialized value will be one element of the container.
    /// </summary>
    internal readonly struct CsvSerializedProperty
    {
        /// <summary>
        /// Source field.
        /// </summary>
        public CsvPropertyInfo Property { get; }

        /// <summary>
        /// Name of the serialized field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Actual value of the field.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Actual type of the serialized field.
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// Whether is this field come from an <see cref="Array"/> or <see cref="IEnumerable"/>.
        /// </summary>
        public bool IsFromCollection => Property.Type.IsCollectionType();

        public CsvSerializedProperty(CsvPropertyInfo field, string name, object? value, Type elementType)
        {
            Property = field;
            Name = name;
            Value = value;
            ElementType = elementType;
        }
    }
}
