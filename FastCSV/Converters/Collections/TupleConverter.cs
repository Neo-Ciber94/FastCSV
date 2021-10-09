using FastCSV.Collections;
using FastCSV.Internal;
using FastCSV.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastCSV.Converters.Collections
{
    internal class TupleConverter : CsvCollectionConverter<ITuple, object?>
    {
        const int TupleMaxLength = 8;

        private readonly Type tupleType;
        private readonly Type[] tupleGenericTypes;

        public TupleConverter(Type tupleType)
        {
            if (tupleType.IsValueType)
            {
                AssertIsValueTupleType(tupleType);
            }
            else
            {
                AssertIsReferenceTupleType(tupleType);
            }

            this.tupleType = tupleType;
            this.tupleGenericTypes = GetTupleElementTypes(tupleType);
        }

        public override bool CanConvert(Type type)
        {
            return typeof(ITuple).IsAssignableFrom(type);
        }

        public override ITuple CreateCollection(Type elementType, int length)
        {
            return new TupleBuilder(tupleGenericTypes);
        }

        public override void AddItem(ITuple collection, int index, Type elementType, object? item)
        {
            var builder = (TupleBuilder)collection;
            builder[index] = item;
        }

        public override bool TrySerialize(ITuple value, ref CsvSerializeState state)
        {
            for (int i = 0; i < value.Length; i++)
            {
                object? obj = value[i];

                if (obj == null)
                {
                    state.WriteNull();
                }
                else
                {
                    Type elementType = obj.GetType();
                    ICsvValueConverter? converter = state.GetConverter(elementType);

                    if (converter == null || !converter.TrySerialize(obj, elementType, ref state))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected override ITuple PrepareCollection(ITuple collection)
        {
            var builder = (TupleBuilder)collection;
            
            if (tupleType.IsValueType)
            {
                return builder.Build(TupleKind.ValueType);
            }
            else
            {
                return builder.Build(TupleKind.Reference);
            }
        }

        protected override Type GetElementTypeAt(int index, ref CsvDeserializeState state)
        {
            return tupleGenericTypes[index];
        }

        public override bool TryDeserialize(out ITuple value, ref CsvDeserializeState state)
        {
            value = default!;

            ITuple collection = CreateCollection(state.ElementType, state.Count);
            CsvProperty? property = state.Property;
            CsvConverterOptions options = state.Options;

            for (int i = 0; i < state.Count; i++)
            {
                Type elementType = GetElementTypeAt(i, ref state);
                string stringValue = state.Read(i);

                CsvDeserializeState elementState = new CsvDeserializeState(options, elementType, stringValue);
                ICsvValueConverter converter = GetElementConverter(options, elementType, property?.Converter);

                if (!converter.TryDeserialize(out object? result, elementType, ref elementState))
                {
                    return false;
                }

                if (result is ITuple tuple)
                {
                    i += (tuple.Length - 1);
                }

                AddItem(collection, i, elementType, result!);
            }

            value = PrepareCollection(collection);
            return true;
        }

        private static void AssertIsValueTupleType(Type type)
        {
            Requires.True(typeof(ITuple).IsAssignableFrom(type));
            Requires.True(TupleUtils.IsValueTupleGenericDefinition(type), $"expected value tuple type but was {type}");
        }

        private static void AssertIsReferenceTupleType(Type type)
        {
            Requires.True(typeof(ITuple).IsAssignableFrom(type));
            Requires.True(TupleUtils.IsReferenceTupleGenericDefinition(type), $"expected tuple type but was {type}");
        }

        private static Type[] GetTupleElementTypes(Type tupleType)
        {
            Requires.True(TupleUtils.IsBuiltinTupleType(tupleType));

            Type[] types = tupleType.GetGenericArguments();

            if (types.Length == TupleMaxLength)
            {
                List<Type> typeList = new(8);

                while(true)
                {
                    int length = Math.Min(7, types.Length);

                    for (int i = 0; i < length; i++)
                    {
                        typeList.Add(types[i]);
                    }

                    if (types.Length != TupleMaxLength)
                    {
                        break;
                    }

                    Type lastType = types[TupleMaxLength - 1];

                    if (typeof(ITuple).IsAssignableFrom(lastType))
                    {
                        types = types[TupleMaxLength - 1].GetGenericArguments();
                    }
                    else
                    {
                        typeList.Add(lastType);
                        break;
                    }
                }

                return typeList.ToArray();
            }
            else
            {
                return types;
            }
        }

        readonly struct TupleElement
        {
            public object? Value { get; }
            public Type Type { get; }

            public TupleElement(Type type, object? value)
            {
                Value = value;
                Type = type;
            }
        }

        class TupleBuilder : ITuple
        {
            private readonly TupleElement[] _items;

            public TupleBuilder(Type[] types)
            {
                _items = types.Select(t => new TupleElement(t, null)).ToArray();
            }

            public int Length => _items.Length;

            public object? this[int index]
            {
                get => _items[index].Value;
                set
                {
                    TupleElement element = _items[index];

                    if (value != null && value.GetType() != element.Type)
                    {
                        throw ThrowHelper.InvalidType(value.GetType(), element.Type);
                    }

                    _items[index] = new TupleElement(element.Type, value);
                }
            }

            public ITuple Build(TupleKind kind)
            {
                return CreateTupleInternal(_items, kind);
            }

            private static ITuple CreateTupleInternal(TupleElement[] items, TupleKind tupleKind)
            {
                Requires.True(items.Length > 0);

                const int MaxTupleElementsCount = 8;

                ITuple? current = null;
                int length = items.Length;

                while(length > 0)
                {
                    int modResult = length % MaxTupleElementsCount;
                    int lastElementsCount = 0;
                    
                    if (current == null) 
                    {
                        lastElementsCount = modResult + 1;
                    } 
                    else
                    {
                        lastElementsCount = modResult;
                    }

                    int startIndex = length - lastElementsCount;
                    IEnumerable<TupleElement> tupleElements = items.Skip(startIndex).Take(lastElementsCount);

                    if (current == null)
                    {
                        Type[] types = tupleElements.Select(t => t.Type).ToArray();
                        object?[] values = tupleElements.Select(e => e.Value).ToArray();
                        current = TupleUtils.CreateTuple(types, values, tupleKind);
                    }
                    else
                    {
                        Type[] types = new Type[8];
                        object?[] values = new object[8];

                        for (int i = 0; i < MaxTupleElementsCount; i++)
                        {
                            types[i] = items[i + startIndex].Type;
                            values[i] = items[i + startIndex].Value;
                        }

                        types[MaxTupleElementsCount - 1] = current.GetType();
                        values[MaxTupleElementsCount - 1] = current;

                        current = TupleUtils.CreateTuple(types, values, tupleKind);
                    }

                    length -= lastElementsCount;
                }             

                return current!;
            }
        }
    }
}
