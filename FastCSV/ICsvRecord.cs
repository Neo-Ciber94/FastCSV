using System;
using System.Collections.Generic;

namespace FastCSV
{
    public interface ICsvRecord : IEnumerable<string>
    {
        public int Length { get; }

        public CsvFormat Format { get; }

        public CsvHeader? Header { get; }

        public string this[int index] { get; }

        public string this[string key] { get; }

        public Span<string> this[Range range] { get; }
    }
}
