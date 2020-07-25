using System.Collections;
using System.Collections.Generic;

namespace FastCSV.Utils
{
    /// <summary>
    /// Provides an iterator over elements of type T, and allows to check for the next element, if any.
    /// </summary>
    /// <typeparam name="T">Type of the elements to iterate</typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerator{T}" />
    public interface IIterator<T> : IEnumerator<T>
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

    public static class IteratorExtensions
    {
        /// <summary>
        /// Converts this enumerator into an <see cref="IIterator{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>An iterator over this enumerator</returns>
        public static IIterator<T> AsIterator<T>(this IEnumerator<T> enumerator)
        {
            return new Iterator<T>(enumerator);
        }
    }

    internal class Iterator<T> : IIterator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private Optional<T> _next;
        private T _current;

        public Iterator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
            _current = default!;
        }

        public T Current => _current;

        object IEnumerator.Current => Current!;

        public Optional<T> Peek
        {
            get
            {
                HasNext();
                return _next;
            }
        }

        public bool HasNext()
        {
            return MoveNext(moving: false);
        }

        public bool MoveNext()
        {
            return MoveNext(moving: true);
        }

        protected bool MoveNext(bool moving)
        {
            if (moving)
            {
                if (_next.HasValue)
                {
                    _current = _next.Value;
                    _next = default;
                    return true;
                }
                else
                {
                    if (_enumerator.MoveNext())
                    {
                        _current = _enumerator.Current;
                        return true;
                    }

                    return false;
                }
            }
            else
            {
                if (_next.HasValue)
                {
                    return true;
                }
                else
                {
                    if (_enumerator.MoveNext())
                    {
                        var temp = _enumerator.Current;
                        _next = new Optional<T>(temp);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public void Reset()
        {
            _current = default!;
            _next = default;
            _enumerator.Reset();
        }
    }
}
