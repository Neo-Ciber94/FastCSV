using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV
{
    public partial class CsvDocument<T>
    {
        /// <summary>
        /// Sorts the elements of this document.
        /// </summary>
        public void Sort()
        {
            SortInternal(TypedRecordComparer.Default);
        }

        /// <summary>
        /// Sorts the elements of this document using the given <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The comparison to use.</param>
        public void Sort(Comparison<T> comparison)
        {
            // FIXME: This creates a new allocation
            SortInternal(new TypedRecordComparer(Comparer<T>.Create(comparison)));
        }

        /// <summary>
        /// Sorts the elements of this document using the given <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public void Sort(IComparer<T> comparer)
        {
            // FIXME: This creates a new allocation
            SortInternal(new TypedRecordComparer(comparer));
        }

        /// <summary>
        /// Sorts the elements in the document by the specified key.
        /// </summary>
        /// <typeparam name="TKey">The key to sort by.</typeparam>
        /// <param name="keySelector">Selects the key to sort.</param>
        public void SortBy<TKey>(Func<T, TKey> keySelector)
        {
            SortBy(Comparer<TKey>.Default, keySelector);
        }

        /// <summary>
        /// Sorts the elements in the document by the specified key with the given <see cref="IComparer{T}"/>.
        /// </summary>
        /// <typeparam name="TKey">The key to sort by.</typeparam>
        /// <param name="keyComparer">The comparer for the keys</param>
        /// <param name="keySelector">Selects the key to sort.</param>
        public void SortBy<TKey>(IComparer<TKey> keyComparer, Func<T, TKey> keySelector)
        {
            // FIXME: Extra allocation due capture of locals
            Comparer<T> comparer = Comparer<T>.Create((x, y) =>
            {
                var xKey = keySelector(x);
                var yKey = keySelector(y);
                return keyComparer.Compare(xKey, yKey);
            });

            SortInternal(new TypedRecordComparer(comparer));
        }

        private void SortInternal(IComparer<TypedRecord> comparer)
        {
            Array.Sort(_records, 0, _count, comparer);
        }

        class TypedRecordComparer : IComparer<TypedRecord>
        {
            public static readonly TypedRecordComparer Default = new(Comparer<T>.Default);

            private readonly IComparer<T> _comparer;

            public TypedRecordComparer(IComparer<T> comparer) => _comparer = comparer;

            public int Compare(TypedRecord x, TypedRecord y) => _comparer.Compare(x.Value, y.Value);
        }
    }
}
