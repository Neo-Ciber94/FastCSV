using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace FastCSV
{
    public partial class CsvReader
    {
        /// <summary>
        /// Represents an asynchronous iterator over the records of a csv.
        /// <para>
        /// The iterator consume each value after advance and don't creates a new enumerator if <see cref="Records.GetAsyncEnumerator()"/> is called
        /// just returns itself. To reset the enumerator use <see cref="Records.Reset()"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IAsyncEnumerator{FastCSV.CsvRecord}" />
        /// <seealso cref="System.Collections.Generic.IAsyncEnumerable{FastCSV.CsvRecord}" />
        public struct RecordsAsync : IAsyncEnumerator<CsvRecord>, IAsyncEnumerable<CsvRecord>
        {
            private readonly CsvReader _reader;
            private CsvRecord? _record;

            internal RecordsAsync(CsvReader reader)
            {
                _reader = reader;
                _record = null;
            }

            public CsvRecord Current => _record!;

            public bool IsDone => _reader.IsDone;

            public ValueTask<bool> MoveNextAsync()
            {
                if (_reader.IsDone)
                {
                    return new ValueTask<bool>(false);
                }

                if ((_record = _reader.Read()) == null)
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
                //_record = null;
            }

#pragma warning disable IDE0060 // Quitar el parámetro no utilizado
            public RecordsAsync GetEnumerator(CancellationToken cancellationToken = default) => this;
#pragma warning restore IDE0060 // Quitar el parámetro no utilizado

            public IAsyncEnumerator<CsvRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default) => this;
        }
    }
}
