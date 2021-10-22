using System.Diagnostics;

namespace FastCSV.Collections
{
    internal sealed class ValueListDebugView<T>
    {
        private readonly T[] _array;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _array;

        public ValueListDebugView(ValueList<T> list)
        {
            _array = list.ToArray();
        }
    }
}
