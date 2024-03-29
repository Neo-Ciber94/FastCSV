﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace FastCSV
{
    public partial class CsvReader
    {
        /// <summary>
        /// Represents an asynchronous iterator over the records of a csv.
        /// <para>
        /// The iterator consume each value after advance and don't creates a new enumerator if <see cref="RecordsEnumerator.GetAsyncEnumerator()"/> is called
        /// just returns itself. To reset the enumerator use <see cref="RecordsEnumerator.Reset()"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IAsyncEnumerator{FastCSV.CsvRecord}" />
        /// <seealso cref="System.Collections.Generic.IAsyncEnumerable{FastCSV.CsvRecord}" />
        public struct RecordsAsyncEnumerator : IAsyncEnumerator<CsvRecord>, IAsyncEnumerable<CsvRecord>
        {
            private readonly CsvReader _reader;
            private readonly CancellationToken _cancellationToken;
            private readonly CsvFormat _format;
            private CsvRecord? _record;

            internal RecordsAsyncEnumerator(CsvReader reader, CsvFormat? format = null, CancellationToken cancellationToken = default)
            {
                _reader = reader;
                _record = null;
                _format = format?? reader.Format;
                _cancellationToken = cancellationToken;
            }

            public CsvRecord Current => _record!;

            public bool IsDone => _reader.IsDone;

            public RecordsAsyncEnumerator WithFormat(CsvFormat format) => new RecordsAsyncEnumerator(_reader, format, _cancellationToken);

            public CancellationToken CancellationToken => _cancellationToken;

            public ValueTask<bool> MoveNextAsync()
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return ValueTask.FromCanceled<bool>(_cancellationToken);
                }

                if (_reader.IsDone)
                {
                    return new ValueTask<bool>(false);
                }

                if ((_record = _reader.Read(_format)) == null)
                {
                    return new ValueTask<bool>(false);
                }

                return new ValueTask<bool>(true);
            }

            public ValueTask DisposeAsync() => default;

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _reader.Reset();
            }

            public RecordsAsyncEnumerator GetEnumerator(CancellationToken cancellationToken = default)
            {
                return new RecordsAsyncEnumerator(_reader, _format, cancellationToken);
            }

            public IAsyncEnumerator<CsvRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new RecordsAsyncEnumerator(_reader, _format, cancellationToken);
            }
        }
    }
}
