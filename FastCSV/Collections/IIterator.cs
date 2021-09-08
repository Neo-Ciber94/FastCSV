using System.Collections.Generic;
using FastCSV.Utils;

namespace FastCSV.Collections
{
    /// <summary>
    /// Provides an iterator over elements of type T, and allows to check for the next element, if any.
    /// </summary>
    /// <typeparam name="T">Type of the elements to iterate</typeparam>
    /// <seealso cref="IEnumerator{T}" />
    public interface IIterator<T> : IEnumerator<T> where T : notnull
    {
        /// <summary>
        /// Determines whether there is a next element.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this iterator has a next value; otherwise, <c>false</c>.
        /// </returns>
        public bool HasNext();

        /// <summary>
        /// Gets the next element (if any) without move the iterator.
        /// </summary>
        /// <value>
        /// An <see cref="Optional{T}"/> containing the next element or none.
        /// </value>
        public Optional<T> Peek { get; }
    }
}
