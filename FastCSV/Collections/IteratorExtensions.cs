using System.Collections.Generic;

namespace FastCSV.Collections
{
    public static class IteratorExtensions
    {
        /// <summary>
        /// Converts this enumerator into an <see cref="Iterator{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>An iterator over this enumerator</returns>
        public static Iterator<T> AsIterator<T>(this IEnumerator<T> enumerator) where T: notnull
        {
            return new Iterator<T>(enumerator);
        }
    }
}
