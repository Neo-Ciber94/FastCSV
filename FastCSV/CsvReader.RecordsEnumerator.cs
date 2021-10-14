using System.Collections;
using System.Collections.Generic;

namespace FastCSV
{
    public partial class CsvReader
    {
        /// <summary>
        /// Represents an iterator over the records of a csv.
        /// <para>
        /// The iterator consume each value after advance and don't creates a new enumerator if <see cref="RecordsEnumerator.GetEnumerator()"/> is called
        /// just returns itself. To reset the enumerator use <see cref="RecordsEnumerator.Reset()"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IEnumerator{FastCSV.CsvRecord}" />
        /// <seealso cref="System.Collections.Generic.IEnumerable{FastCSV.CsvRecord}" />
        public struct RecordsEnumerator : IEnumerator<CsvRecord>, IEnumerable<CsvRecord>
        {
            private readonly CsvReader _reader;
            private CsvRecord? _record;

            internal RecordsEnumerator(CsvReader reader)
            {
                _reader = reader;
                _record = null;
            }

            public CsvRecord Current => _record!;

            object? IEnumerator.Current => _record;

            public bool IsDone => _reader.IsDone;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_reader.IsDone)
                {
                    return false;
                }

                if ((_record = _reader.Read()) == null)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _reader.Reset();
                //_record = null;
            }

            public RecordsEnumerator GetEnumerator() => this;

            IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;
        }

    }
}
