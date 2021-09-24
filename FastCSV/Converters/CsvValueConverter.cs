using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    /// <summary>
    /// State of the current serialization operation.
    /// </summary>
    public struct CsvSerializeState
    {
        private readonly List<string> _serialized;

        /// <summary>
        /// Options used for serialization.
        /// </summary>
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// The source property or field of the serialize member.
        /// </summary>
        public MemberInfo Member { get; }

        /// <summary>
        /// The type of the member being serialized.
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// The actual value being serialized.
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// Gets a list of the current serialized values.
        /// </summary>
        public IReadOnlyList<string> Serialized => _serialized;

        /// <summary>
        /// Gets an span view to the serialized values.
        /// </summary>
        /// <returns>An span of the serialized values.</returns>
        public ReadOnlySpan<string> GetDeserializedAsSpan()
        {
            return CollectionsMarshal.AsSpan(_serialized);
        }

        /// <summary>
        /// Constructs a new <see cref="CsvSerializeState"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="member">The source member of the value.</param>
        /// <param name="elementType">Type of the member.</param>
        /// <param name="value">The value to serialize.</param>
        public CsvSerializeState(CsvConverterOptions options, MemberInfo member, Type elementType, object? value, List<string> serialized)
        {
            Options = options;
            Member = member;
            ElementType = elementType;
            Value = value;
            _serialized = serialized;
        }

        /// <summary>
        /// Write the serialized value to this state.
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            _serialized.Add(value);
        }
    }

    /// <summary>
    /// State of the current deserialization operation.
    /// </summary>
    public struct CsvDeserializeState
    {
        public CsvConverterOptions Options { get; }

        public CsvRecord Record { get; }

        public int ColumnIndex { get; }

        public MemberInfo Member { get; }

        public Type ElementType { get; }

        public object Instance { get; }

        public CsvDeserializeState(CsvConverterOptions options, CsvRecord record, int columnIndex, MemberInfo member, Type elementType, object instance)
        {
            Options = options;
            Record = record;
            ColumnIndex = columnIndex;
            Member = member;
            ElementType = elementType;
            Instance = instance;
        }

        public void Write(object? value)
        {
            switch (Member)
            {
                case PropertyInfo property:
                    property.SetValue(Instance, value);
                    break;
                case FieldInfo field:
                    field.SetValue(Instance, value);
                    break;
                default:
                    throw new Exception("Unreachable");
            }
        }
    }

    public abstract class CsvValueConverter
    {
    }
}
