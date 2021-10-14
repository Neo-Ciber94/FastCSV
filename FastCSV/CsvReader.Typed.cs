using FastCSV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
            Dictionary<string, string>? data = Read()?.ToDictionary();

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
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public IEnumerable<T> ReadAllAs<T>(CsvConverterOptions? options = null)
        {
            List<T> result = new List<T>();

            foreach (CsvRecord record in ReadAll())
            {
                Dictionary<string, string> data = record.ToDictionary()!;
                T value = CsvConverter.DeserializeFromDictionary<T>(data, options);
                result.Add(value);
            }

            return result;
        }

        /// <summary>
        /// Gets an enumerator over the records of this reader csv and parser them to the type T,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <param name="options">The options used for deserialize.</param>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsEnumeratorTyped<T> ReadAllAsWithEnumerator<T>(CsvConverterOptions? options = null) where T: notnull
        {
            return new RecordsEnumeratorTyped<T>(this, options);
        }

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
            Dictionary<string, string>? data = record?.ToDictionary();

            if (data == null)
            {
                return Optional.None<T>();
            }

            var result = CsvConverter.DeserializeFromDictionary<T>(data, options);
            return Optional.Some(result);
        }

        public async IAsyncEnumerable<T> ReadAllAsAsync<T>(CsvConverterOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default) where T : notnull
        {
            while (true)
            {
                Optional<T> value = await ReadAsAsync<T>(options, cancellationToken);

                if (!value.HasValue)
                {
                    break;  
                }

                yield return value.Value;
            }
        }

        /// <summary>
        /// Gets an enumerator over the records of this reader csv and parser them to the type T,
        /// this enumerable will read the records using this reader, so when the iteration
        /// end the reader will be at the end of the file.
        /// </summary>
        /// <param name="options">The options used for deserialize.</param>
        /// <returns>An enumerable over the records of this reader csv.</returns>
        public RecordsEnumeratorAsyncTyped<T> ReadAllAsAsyncWithAsyncEnumerator<T>(CsvConverterOptions? options = null, CancellationToken cancellationToken = default) where T: notnull
        {
            return new RecordsEnumeratorAsyncTyped<T>(this, options, cancellationToken);
        }

        public struct RecordsEnumeratorTyped<T> : IEnumerable<T>, IEnumerator<T> where T: notnull
        {
            private readonly CsvReader _reader;
            private readonly CsvConverterOptions? _options;
            private Optional<T> _current;

            public RecordsEnumeratorTyped(CsvReader reader, CsvConverterOptions? options = null)
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

            public bool MoveNext()
            {
                _current = _reader.ReadAs<T>(_options);
                return _current.HasValue;
            }

            public void Reset()
            {
                _reader.Reset();
            }

            public RecordsEnumeratorTyped<T> GetEnumerator() => new RecordsEnumeratorTyped<T>(_reader, _options);

            void IDisposable.Dispose() { }

            object IEnumerator.Current => Current;

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct RecordsEnumeratorAsyncTyped<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T> where T: notnull
        {
            private readonly CsvReader _reader;
            private readonly CsvConverterOptions? _options;
            private readonly CancellationToken _cancellationToken;
            private Optional<T> _current;

            public RecordsEnumeratorAsyncTyped(CsvReader reader, CsvConverterOptions? options = null, CancellationToken cancellationToken = default)
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

            public RecordsEnumeratorAsyncTyped<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new RecordsEnumeratorAsyncTyped<T>(_reader, _options, cancellationToken);
            }

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) => GetAsyncEnumerator(cancellationToken);

            public ValueTask<bool> MoveNextAsync()
            {
                _cancellationToken.ThrowIfCancellationRequested();
                _current = _reader.ReadAs<T>(_options);
                return ValueTask.FromResult(_current.HasValue);
            }

            public void Reset()
            {
                _reader.Reset();
            }

            ValueTask IAsyncDisposable.DisposeAsync() => default;
        }
    }
}
