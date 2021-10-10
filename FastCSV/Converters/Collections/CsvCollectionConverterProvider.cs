﻿using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FastCSV.Converters.Collections
{
    internal class CsvCollectionConverterProvider : ICsvConverterProvider
    {
        public static readonly CsvCollectionConverterProvider Default = new();

        private readonly IDictionary<Type, ICsvValueConverter> _converters = new Dictionary<Type, ICsvValueConverter>();

        // Array
        private ArrayConverter? _arrayConverter = null;
        private ArrayConverter GetOrCreateArrayConverter()
        {
            if (_arrayConverter == null)
            {
                _arrayConverter = new ArrayConverter();
            }

            return _arrayConverter;
        }

        // ArrayList
        private ArrayListConverter? _listConverter = null;
        private ArrayListConverter GetOrCreateArrayListConverter()
        {
            if (_listConverter == null)
            {
                _listConverter = new ArrayListConverter();
            }

            return _listConverter;
        }

        // Stack
        private StackConverter? _stackConverter = null;
        private StackConverter GetOrCreateStackConverter()
        {
            if (_stackConverter == null)
            {
                _stackConverter = new StackConverter();
            }

            return _stackConverter;
        }

        // Queue
        private QueueConverter? _queueConverter = null;
        private QueueConverter GetOrCreateQueueConverter()
        {
            if (_queueConverter == null)
            {
                _queueConverter = new QueueConverter();
            }

            return _queueConverter;
        }

        public ICsvValueConverter? GetConverter(Type type)
        {
            if (type.IsArray)
            {
                return GetOrCreateArrayConverter();
            }

            if (_converters.TryGetValue(type, out ICsvValueConverter? collectionConverter))
            {
                return collectionConverter;
            }

            if (type.IsGenericType)
            {
                Type genericDefinition = type.GetGenericTypeDefinition();

                if (genericDefinition == typeof(Stack<>))
                {
                    return GetOrCreateCollectionConverter(type, typeof(StackOfTConverter<>));
                }

                if (genericDefinition == typeof(Queue<>))
                {
                    return GetOrCreateCollectionConverter(type, typeof(QueueOfTConverter<>));
                }

                switch (type)
                {
                    case Type _ when genericDefinition == typeof(HashSet<>):
                    case Type _ when genericDefinition == typeof(ISet<>):
                    case Type _ when genericDefinition == typeof(IReadOnlySet<>):
                        return GetOrCreateCollectionConverter(type, typeof(HashSetOfTConverter<>));
                    default:
                        break;
                }

                if (genericDefinition == typeof(SortedSet<>))
                {
                    return GetOrCreateCollectionConverter(type, typeof(SortedSetOfTConverter<>));
                }

                if (genericDefinition == typeof(LinkedList<>))
                {
                    return GetOrCreateCollectionConverter(type, typeof(LinkedListOfTConverter<>));
                }

                if (typeof(ITuple).IsAssignableFrom(type))
                {
                    var tupleConverter = new TupleConverter(type);
                    _converters.Add(type, tupleConverter);
                    return tupleConverter;
                }

                switch (type)
                {
                    case Type _ when genericDefinition == typeof(List<>):
                    case Type _ when genericDefinition == typeof(IList<>):
                    case Type _ when genericDefinition == typeof(IReadOnlyList<>):
                    case Type _ when genericDefinition == typeof(ICollection<>):
                    case Type _ when genericDefinition == typeof(IReadOnlyCollection<>):
                    case Type _ when genericDefinition == typeof(IEnumerable<>):
                        return GetOrCreateCollectionConverter(type, typeof(ListOfTConverter<>));
                    default:
                        break;
                }
            }

            // Fallback
            return GetNonGenericCollectionConverter(type);
        }

        private ICsvValueConverter? GetNonGenericCollectionConverter(Type type)
        {
            switch (type)
            {
                case Type _ when type == typeof(ArrayList):
                case Type _ when type == typeof(IList):
                case Type _ when type == typeof(ICollection):
                case Type _ when type == typeof(IEnumerable):
                    return GetOrCreateArrayListConverter();
                default:
                    break;
            }

            if (type == typeof(Stack))
            {
                return GetOrCreateStackConverter();
            }

            if (type == typeof(Queue))
            {
                return GetOrCreateQueueConverter();
            }

            if (type == typeof(BitArray))
            {
                return new BitArrayConverter<int>();
            }

            return null;
        }

        private ICsvValueConverter GetOrCreateCollectionConverter(Type enumerableType, Type genericDefinition)
        {
            Debug.Assert(enumerableType.IsEnumerableType());
            Debug.Assert(genericDefinition.IsGenericTypeDefinition);

            var elementType = enumerableType.GetEnumerableElementType()!;
            var converter = GenericConverterFactory.CreateCollectionConverter(genericDefinition, elementType);
            _converters.Add(enumerableType, converter);
            return converter;
        }
    }
}
