using FastCSV.Collections;
using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FastCSV
{
    public partial class CsvReader
    {
        /// <summary>
        /// Reads the next record as a value of type T.
        /// </summary>
        /// <typeparam name="T">Type to cast the record to.</typeparam>
        /// <param name="options">The options used for deserialize.</param>
        /// <returns>An optional with the value or none is there is no more records to read.</returns>
        public Optional<T> ReadAs<T>(CsvConverterOptions? options = null) where T : notnull
        {
            CsvRecord? record = Read(options?.Format);

            if (record == null)
            {
                return Optional.None<T>();
            }

            return record.ConvertTo<T>(options);
        }

        /// <summary>
        /// Gets an enumerator over the records of this reader csv and parser them to the type T,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <param name="options">The options used for deserialize.</param>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsEnumeratorOfT<T> ReadAllAs<T>(CsvConverterOptions? options = null) where T: notnull
        {
            return new RecordsEnumeratorOfT<T>(this, options);
        }

        /// <summary>
        /// An enumerator over the typed values of a csv document.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        public struct RecordsEnumeratorOfT<T> : IEnumerable<T>, IEnumerator<T> where T: notnull
        {
            private readonly CsvReader _reader;
            private readonly CsvConverterOptions? _options;
            private Optional<T> _current;

            public RecordsEnumeratorOfT(CsvReader reader, CsvConverterOptions? options = null)
            {
                _reader = reader;
                _options = options;
                _current = default;
            }

            public T Current
            {
                get
                {
                    if (!_current.HasValue)
                    {
                        throw new InvalidOperationException("No values available");
                    }

                    return _current.Value;
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _current = _reader.ReadAs<T>(_options);
                return _current.HasValue;
            }

            public void Reset()
            {
                _reader.Reset();
            }

            public RecordsEnumeratorOfT<T> GetEnumerator()
            {
                return new RecordsEnumeratorOfT<T>(_reader, _options);
            }

            void IDisposable.Dispose(){ }

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
