
using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Collections
{
    /// <summary>
    /// A stack-base enumerator.
    /// </summary>
    /// <typeparam name="T">Type fo the elements to enumerate.</typeparam>
    /// <typeparam name="TEnumerator">The type of the enumerator.</typeparam>
    public interface IValueEnumerable<T, TEnumerator> : IEnumerable<T> where TEnumerator : IEnumerator<T>
    {
        /// <summary>
        /// Gets an enumerator over the elements of this collection.
        /// </summary>
        /// <returns>An enumerator over the elements.</returns>
        new TEnumerator GetEnumerator();

        /// <inheritdoc />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}