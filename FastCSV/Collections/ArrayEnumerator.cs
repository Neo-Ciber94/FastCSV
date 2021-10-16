using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Collections
{
    public struct ArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[]? items;
        private readonly int count;
        private int index;

        public ArrayEnumerator(T[] items, int count)
        {
            this.items = items;
            this.count = count;
            this.index = -1;
        }

        public T Current
        {
            get
            {
                if (index == -1 || items == null)
                {
                    throw new InvalidOperationException("enumerator is not initialized");
                }

                return items[index];
            }
        }

        object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            int next = index + 1;

            if (next < count)
            {
                index = next;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            index = -1;
        }

        void IDisposable.Dispose() { }
    }
}
