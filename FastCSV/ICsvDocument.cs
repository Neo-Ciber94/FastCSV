using System.Collections.Generic;

namespace FastCSV
{
    public interface ICsvDocument : IEnumerable<CsvRecord>
    {
        public CsvHeader Header { get; }

        public CsvFormat Format { get; }

        public int Count { get; }

        public bool IsEmpty { get; }

        public CsvRecord this[int index] { get; }
    }
}
