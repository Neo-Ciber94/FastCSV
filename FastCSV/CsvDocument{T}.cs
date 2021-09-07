using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Represents a typed in-memory csv document.
    /// </summary>
    /// <typeparam name="T">Type of the csv values.</typeparam>
    /// <seealso cref="FastCSV.ICsvDocument" />
    public partial class CsvDocument<T> : ICsvDocument
    {
        private readonly List<TypedCsvRecord<T>> _records = new List<TypedCsvRecord<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        public CsvDocument() : this(CsvFormat.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        public CsvDocument(CsvFormat format)
        {
            Header = new CsvHeader(CsvConverter.GetHeader<T>(), format);
            Format = format;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public CsvDocument(IEnumerable<T> elements) : this(elements, CsvFormat.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="format">The format.</param>
        public CsvDocument(IEnumerable<T> elements, CsvFormat format)
        {
            Header = new CsvHeader(CsvConverter.GetHeader<T>(), format);
            Format = format;

            foreach(var e in elements)
            {
                Write(e);
            }
        }

        internal CsvDocument(List<TypedCsvRecord<T>> records, CsvHeader header, CsvFormat format)
        {
            if(header.Format != format)
            {
                throw new ArgumentException("Header format differs from the given format");
            }

            _records = records;
            Header = header;
            Format = format;
        }

        /// <summary>
        /// Gets the <see cref="CsvRecord"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="CsvRecord"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public CsvRecord this[int index] => _records[index].Record;

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public CsvHeader Header { get; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public CsvFormat Format { get; }

        /// <summary>
        /// Gets the number of records in this csv.
        /// </summary>
        /// <value>
        /// The number of records in the csv.
        /// </value>
        public int Count => _records.Count;

        /// <summary>
        /// Gets a value indicating whether this csv is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this csv is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// Gets a collection of the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public ValueCollection Values => new ValueCollection(_records);

        /// <summary>
        /// Writes the specified value as a record.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(T value)
        {
            _records.Add(new TypedCsvRecord<T>(value, Format));
        }

        /// <summary>
        /// Writes the specified value as a record in the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void WriteAt(int index, T value)
        {
            _records.Insert(index, new TypedCsvRecord<T>(value, Format));
        }

        /// <summary>
        /// Updates the record at the specified index with the given value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Update(int index, T value)
        {
            _records[index] = new TypedCsvRecord<T>(value, Format);
        }

        /// <summary>
        /// Removes the record at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The removed record an the value associate to it.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public (CsvRecord, T) RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(index.ToString());
            }

            TypedCsvRecord<T> typedRecord = _records[index];
            _records.RemoveAt(index);
            return typedRecord.ToTuple();
        }

        /// <summary>
        /// Removes all the records that match the predicate.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The number of removed records</returns>
        public int RemoveAll(Predicate<T> match)
        {
            return _records.RemoveAll((n) => match(n.Value));
        }

        /// <summary>
        /// Clears the contents of this csv.
        /// </summary>
        public void Clear()
        {
            _records.Clear();
        }

        /// <summary>
        /// Determines whether this csv contains the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if contains the value; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            foreach (TypedCsvRecord<T> e in _records)
            {
                if (comparer.Equals(e.Value, value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the index of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            int length = _records.Count;

            for(int i = 0; i < length; i++)
            {
                T current = _records[i].Value;

                if(comparer.Equals(current, value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the last index of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int LastIndexOf(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            int length = _records.Count;

            for (int i = length - 1; i >= 0; i--)
            {
                T current = _records[i].Value;

                if (comparer.Equals(current, value))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T GetValue(int index)
        {
            if(index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException(index.ToString());
            }

            return _records[index].Value;
        }

        /// <summary>
        /// Gets a copy of this csv with the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public CsvDocument<T> WithFormat(CsvFormat format)
        {
            return new CsvDocument<T>(_records.ToList(), Header.WithFormat(format), format);
        }

        /// <summary>
        /// Writes the content of this instance to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void WriteContentsToFile(string path)
        {
            CsvWriter.WriteToFile(Header, this, path);
        }

        /// <summary>
        /// Gets a string representation of this csv with the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(CsvFormat format)
        {
            StringBuilder sb = StringBuilderCache.Acquire();

            sb.AppendLine(Header.ToString(format));

            foreach (TypedCsvRecord<T> typedRecord in _records)
            {
                sb.AppendLine(typedRecord.Record.ToString(format));
            }

            return StringBuilderCache.ToStringAndRelease(ref sb!);
        }

        /// <summary>
        /// Gets a string representation of this csv with the specified format.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(Format);
        }

        /// <summary>
        /// Gets a pretty formated string of the records of this document.
        /// </summary>
        /// <returns></returns>
        public string ToPrettyString()
        {
            return CsvUtility.ToPrettyString(_records.Select(e => e.Record));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator() => new Enumerator(_records);

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<CsvRecord>
        {
            private readonly List<TypedCsvRecord<T>> _records;
            private int _index;

            internal Enumerator(List<TypedCsvRecord<T>> records)
            {
                _records = records;
                _index = -1;
            }

            public CsvRecord Current => _records[_index].Record;

            object? IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                int next = _index + 1;

                if(next < _records.Count)
                {
                    _index = next;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        public struct ValueCollection : IReadOnlyList<T>
        {
            private readonly List<TypedCsvRecord<T>> _records;

            internal ValueCollection(List<TypedCsvRecord<T>> records)
            {
                _records = records;
            }

            public int Count => _records.Count;

            public T this[int index] => _records[index].Value;

            public IEnumerator<T> GetEnumerator() => _records.Select(e => e.Value).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
