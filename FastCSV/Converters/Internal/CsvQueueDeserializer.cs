using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FastCSV.Converters.Internal
{
    internal class CsvQueueDeserializer : CsvCollectionDeserializer
    {
        private MethodInfo? pushMethod;
        private Type? queueType;

        private readonly bool isGeneric;

        public CsvQueueDeserializer(bool isGeneric)
        {
            this.isGeneric = isGeneric;
        }

        protected override void AddItem(object collection, int _, object? item)
        {
            if (pushMethod == null)
            {
                Debug.Assert(queueType != null, $"{nameof(AddItem)} was called before {nameof(CreateCollection)}");
                pushMethod = queueType.GetMethod(nameof(Queue.Enqueue));
            }

            pushMethod!.Invoke(collection, new[] { item });
        }

        protected override object CreateCollection(Type elementType, int length)
        {
            if (queueType == null)
            {
                queueType = isGeneric ? typeof(Queue<>).MakeGenericType(elementType) : typeof(Queue);
            }

            return Activator.CreateInstance(queueType, length)!;
        }
    }
}
