using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FastCSV.Converters.Collections
{
    internal class TupleConverter : CsvCollectionConverter<ITuple, object?>
    {
        private readonly Type tupleType;

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
        }

        public override bool CanConvert(Type type)
        {
            return typeof(ITuple).IsAssignableFrom(type);
        }

        public override ITuple CreateCollection(Type elementType, int length)
        {
            return new TupleBuilder(GetTupleElementTypes(tupleType));
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

        public override ITuple PrepareCollection(ITuple collection)
        {
            var builder = (TupleBuilder)collection;
            
            if (tupleType.IsValueType)
            {
                return builder.Build(TupleBuildType.ValueType);
            }
            else
            {
                return builder.Build(TupleBuildType.Class);
            }
        }

        private static void AssertIsValueTupleType(Type type)
        {
            Debug.Assert(typeof(ITuple).IsAssignableFrom(type));

            Type t = type.GetGenericTypeDefinition();

            switch (t)
            {
                case Type _ when t == typeof(ValueTuple<>):
                case Type _ when t == typeof(ValueTuple<,>):
                case Type _ when t == typeof(ValueTuple<,,>):
                case Type _ when t == typeof(ValueTuple<,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,,,>):
                case Type _ when t == typeof(ValueTuple<,,,,,,,>):
                    break;
                default:
                    throw new InvalidOperationException($"Expected value tuple type but was {type}");
            }
        }

        private static void AssertIsReferenceTupleType(Type type)
        {
            Debug.Assert(typeof(ITuple).IsAssignableFrom(type));

            Type t = type.GetGenericTypeDefinition();

            switch (t)
            {
                case Type _ when t == typeof(Tuple<>):
                case Type _ when t == typeof(Tuple<,>):
                case Type _ when t == typeof(Tuple<,,>):
                case Type _ when t == typeof(Tuple<,,,>):
                case Type _ when t == typeof(Tuple<,,,,>):
                case Type _ when t == typeof(Tuple<,,,,,>):
                case Type _ when t == typeof(Tuple<,,,,,,>):
                case Type _ when t == typeof(Tuple<,,,,,,,>):
                    break;
                default:
                    throw new InvalidOperationException($"Expected value tuple type but was {type}");
            }
        }

        private static Type[] GetTupleElementTypes(Type tupleType)
        {
            Debug.Assert(typeof(ITuple).IsAssignableFrom(tupleType));
            return tupleType.GetGenericArguments();
        }

        enum TupleBuildType
        {
            ValueType, Class
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

            public object? this[int index]
            {
                get => _items[index].Value;
                set
                {
                    TupleElement element = _items[index];

                    if (value != null)
                    {
                        Debug.Assert(value.GetType() == element.Type);
                    }

                    _items[index] = new TupleElement(element.Type, value);
                }
            }

            public int Length => _items.Length;

            public ITuple Build(TupleBuildType buildType)
            {
                if (buildType == TupleBuildType.ValueType)
                {
                    return Length switch
                    {
                        1 => CreateTupleInstance(typeof(ValueTuple<>)),
                        2 => CreateTupleInstance(typeof(ValueTuple<,>)),
                        3 => CreateTupleInstance(typeof(ValueTuple<,,>)),
                        4 => CreateTupleInstance(typeof(ValueTuple<,,,>)),
                        5 => CreateTupleInstance(typeof(ValueTuple<,,,,>)),
                        6 => CreateTupleInstance(typeof(ValueTuple<,,,,,>)),
                        7 => CreateTupleInstance(typeof(ValueTuple<,,,,,,>)),
                        8 => CreateTupleInstance(typeof(ValueTuple<,,,,,,,>)),
                        _ => throw new Exception($"Invalid value type length: {Length}")
                    };
                }

                if (buildType == TupleBuildType.Class)
                {
                    return Length switch
                    {
                        1 => CreateTupleInstance(typeof(Tuple<>)),
                        2 => CreateTupleInstance(typeof(Tuple<,>)),
                        3 => CreateTupleInstance(typeof(Tuple<,,>)),
                        4 => CreateTupleInstance(typeof(Tuple<,,,>)),
                        5 => CreateTupleInstance(typeof(Tuple<,,,,>)),
                        6 => CreateTupleInstance(typeof(Tuple<,,,,,>)),
                        7 => CreateTupleInstance(typeof(Tuple<,,,,,,>)),
                        8 => CreateTupleInstance(typeof(Tuple<,,,,,,,>)),
                        _ => throw new Exception($"Invalid value type length: {Length}")
                    };
                }

                throw new Exception("Unreachable");
            }

            private ITuple CreateTupleInstance(Type tupleGenericDefinition)
            {
                Type[] types = _items.Select(t => t.Type).ToArray();
                object?[] values = _items.Select(e => e.Value).ToArray();

                var genericTupleType = tupleGenericDefinition.MakeGenericType(types);
                return (ITuple)Activator.CreateInstance(genericTupleType, values)!;
            }
        }
    }
}
