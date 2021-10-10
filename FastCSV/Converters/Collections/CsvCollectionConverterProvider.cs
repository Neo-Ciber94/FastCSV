using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
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

                // Special case for tuples
                if (typeof(ITuple).IsAssignableFrom(type))
                {
                    var tupleConverter = new TupleConverter(type);
                    _converters.Add(type, tupleConverter);
                    return tupleConverter;
                }

                // Generic collections
                if (TryGetGenericCollectionConverter(type, genericDefinition, out collectionConverter))
                {
                    return collectionConverter;
                }

                // Concurrent collections converters
                if (TryGetConcurrentCollectionConverter(type, genericDefinition, out collectionConverter))
                {
                    return collectionConverter;
                }

                // Fallback for most of collection types
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

            // Non generic collections
            if (TryGetNonGenericCollectionConverter(type, out collectionConverter))
            {
                return collectionConverter;
            }

            return collectionConverter;
        }

        private bool TryGetGenericCollectionConverter(Type type, Type genericDefinition, out ICsvValueConverter? converter)
        {
            converter = null;

            if (genericDefinition == typeof(Stack<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(StackOfTConverter<>));
                return true;
            }

            if (genericDefinition == typeof(Queue<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(QueueOfTConverter<>));
                return true;
            }

            switch (type)
            {
                case Type _ when genericDefinition == typeof(HashSet<>):
                case Type _ when genericDefinition == typeof(ISet<>):
                case Type _ when genericDefinition == typeof(IReadOnlySet<>):
                    converter = GetOrCreateCollectionConverter(type, typeof(HashSetOfTConverter<>));
                    return true;
                default:
                    break;
            }

            if (genericDefinition == typeof(SortedSet<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(SortedSetOfTConverter<>));
                return true;
            }

            if (genericDefinition == typeof(LinkedList<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(LinkedListOfTConverter<>));
                return true;
            }

            return false;
        }

        private bool TryGetConcurrentCollectionConverter(Type type, Type genericDefinition, out ICsvValueConverter? converter)
        {
            converter = null;

            if (genericDefinition == typeof(BlockingCollection<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(BlockingCollectionOfTConverter<>));
                return true;
            }

            if (genericDefinition == typeof(ConcurrentBag<>) || genericDefinition == typeof(IProducerConsumerCollection<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(ConcurrentBagOfTConverter<>));
                return true;
            }

            if (genericDefinition == typeof(ConcurrentStack<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(ConcurrentStackOfTConverter<>));
                return true;
            }

            if (genericDefinition == typeof(ConcurrentQueue<>))
            {
                converter = GetOrCreateCollectionConverter(type, typeof(ConcurrentQueueOfTConverter<>));
                return true;
            }

            return false;
        }

        private bool TryGetNonGenericCollectionConverter(Type type, out ICsvValueConverter? converter)
        {
            converter = null;

            switch (type)
            {
                case Type _ when type == typeof(ArrayList):
                case Type _ when type == typeof(IList):
                case Type _ when type == typeof(ICollection):
                case Type _ when type == typeof(IEnumerable):
                    converter = GetOrCreateArrayListConverter();
                    return true;
                default:
                    break;
            }

            if (type == typeof(Stack))
            {
                converter = GetOrCreateStackConverter();
                return true;
            }

            if (type == typeof(Queue))
            {
                converter = GetOrCreateQueueConverter();
                return true;
            }

            if (type == typeof(BitArray))
            {
                converter = new BitArrayConverter<int>();
                return true;
            }

            return false;
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
