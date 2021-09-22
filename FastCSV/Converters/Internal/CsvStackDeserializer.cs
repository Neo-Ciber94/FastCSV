using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FastCSV.Converters.Internal
{
    internal class CsvStackDeserializer : CsvCollectionDeserializer
    {
        private MethodInfo? pushMethod;
        private Type? stackType;

        private readonly bool isGeneric;

        public CsvStackDeserializer(bool isGeneric)
        {
            this.isGeneric = isGeneric;
        }

        protected override void AddItem(object collection, int _, object? item)
        {
            if (pushMethod == null)
            {
                Debug.Assert(stackType != null, $"{nameof(AddItem)} was called before {nameof(CreateCollection)}");
                pushMethod = stackType.GetMethod(nameof(Stack.Push));
            }

            pushMethod!.Invoke(collection, new[] { item });
        }

        protected override object CreateCollection(Type elementType, int length)
        {
            if (stackType == null)
            {
                stackType = isGeneric ? typeof(Stack<>).MakeGenericType(elementType) : typeof(Stack);
            }

            return Activator.CreateInstance(stackType, length)!;
        }
    }
}
