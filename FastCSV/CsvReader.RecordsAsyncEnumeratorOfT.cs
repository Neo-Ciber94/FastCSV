using FastCSV.Collections;
using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastCSV
{
    public partial class CsvReader
    {
        /// <summary>
        /// Reads the next record as a value of type T asyncronously.
        /// </summary>
        /// <typeparam name="T">Type to cast the record to.</typeparam>
        /// <param name="options">The options used for deserializing.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An optional with the value or none is there is no more records to read.</returns>
        public async ValueTask<Optional<T>> ReadAsAsync<T>(CsvConverterOptions? options = null, CancellationToken cancellationToken = default) where T : notnull
        {
            CsvRecord? record = await ReadAsync(cancellationToken);
            Dictionary<string, SingleOrList<string>>? data = record?.ToDictionary();

            if (data == null)
            {
                return Optional.None<T>();
            }

            var result = CsvConverter.DeserializeFromDictionary<T>(data, options);
            return Optional.Some(result);
        }

        /// <summary>
        /// Gets an enumerator over the records of this reader csv and parser them to the type T,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <param name="options">The options used for deserialize.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsAsyncEnumeratorOfT<T> ReadAllAsAsync<T>(CsvConverterOptions? options = null, CancellationToken cancellationToken = default) where T: notnull
        {
            return new RecordsAsyncEnumeratorOfT<T>(this, options, cancellationToken);
        }

        /// <summary>
        /// An asynchronous enumerator over the typed values of a csv document.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        public struct RecordsAsyncEnumeratorOfT<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T> where T: notnull
        {
            private readonly CsvReader _reader;
            private readonly CsvConverterOptions? _options;
            private readonly CancellationToken _cancellationToken;
            private Optional<T> _current;

            public RecordsAsyncEnumeratorOfT(CsvReader reader, CsvConverterOptions? options = null, CancellationToken cancellationToken = default)
            {
                _reader = reader;
                _options = options;
                _current = default;
                _cancellationToken = cancellationToken;
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

            public ValueTask<bool> MoveNextAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();

                /*
                 * We could not use async-await with 'ReadAsAsync<T>' to mutate the inner state of this struct
                 * due the state machine generated copies this struct which invalidades the state.
                 */
                _current = _reader.ReadAs<T>(_options);
                return ValueTask.FromResult(_current.HasValue);
            }

            public void Reset()
            {
                _reader.Reset();
            }

            public RecordsAsyncEnumeratorOfT<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new RecordsAsyncEnumeratorOfT<T>(_reader, _options, cancellationToken);
            }

            ValueTask IAsyncDisposable.DisposeAsync() => default;

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) => GetAsyncEnumerator();
        }
    }
}
