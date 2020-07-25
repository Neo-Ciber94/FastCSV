#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FastCSV.Csv
{
    public class CsvReader 
    {
        public CsvReader(StreamReader reader, CsvFormat format) { }

        public CsvHeader Header { get; }

        public CsvFormat Format { get; }

        public char Delimiter { get; }

        public char Quote { get; }

        public bool Done { get; }

        public CsvRecord? Read() => throw null;

        public IEnumerable<CsvRecord>? ReadAll() => throw null;

        public struct RecordEnumerable { }
    }

    public class CsvWriter 
    { 
        public CsvWriter(StreamWriter writer, CsvFormat format) { }

        public CsvFormat Format { get; }

        public char Delimiter { get; }

        public char Quote { get; }

        public bool IsFlexible { get; }

        public void Write(params string[] record) => throw null;

        public void WriteValue<T>(T value) => throw null;
    }

    public class CsvFormat
    {
        public static readonly CsvFormat Default = new CsvFormat(',', '"');

        public CsvFormat(char delimiter, char quote)
        {
            Delimiter = delimiter;
            Quote = quote;
        }

        public char Delimiter { get; }

        public char Quote { get; }

        public CsvFormat WithDelimiter(char delimiter) => throw null;

        public CsvFormat WithQuote(char quote) => throw null;
    }

    public class CsvDocument : IEnumerable<CsvRecord>
    {
        public CsvDocument(string data) { }

        public CsvDocument(CsvHeader header, IEnumerable<CsvRecord> records) { }

        public static CsvDocument FromPath(string path) => throw null;

        public static CsvDocument FromEnumerable<T>(IEnumerable<T> values) => throw null;

        public CsvHeader Header { get; }

        public int Count { get; }

        public CsvRecord this[int index] => throw null;

        public void Write(params string[] record) => throw null;

        public void WriteValue<T>(T value) => throw null;

        public IEnumerator<CsvRecord> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class CsvHeader : IEnumerable<string>
    {
        internal CsvHeader(IEnumerable<string> values, CsvFormat format) {  }

        public IEnumerable<string> Values { get; }

        public CsvFormat Format { get; }

        public char Delimiter { get; }

        public char Quote { get; }

        public int Length { get; }

        public string this[int index] => throw null;

        public CsvHeader WithDelimiter(char delimiter) => throw null;

        public CsvHeader WithQuote(char quote) => throw null;

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class CsvRecord : IEnumerable<string>
    {
        internal CsvRecord(IEnumerable<string> values, CsvFormat format) { }

        public IEnumerable<string> Values { get; }

        public CsvFormat Format { get; }

        public CsvHeader Header { get; }

        public char Delimiter { get; }

        public char Quote { get; }

        public int Length { get; }

        public string this[int index] => throw null;

        public string this[string key] => throw null;

        public CsvRecord WithDelimiter(char delimiter) => throw null;

        public CsvRecord WithQuote(char quote) => throw null;

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
