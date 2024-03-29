﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastCSV.Collections;
using FastCSV.Internal;
using FastCSV.Utils;

namespace FastCSV
{
    /// <summary>
    /// Represents a typed in-memory csv document.
    /// </summary>
    /// <typeparam name="T">Type of the csv values.</typeparam>
    /// <seealso cref="FastCSV.ICsvDocument" />
    public partial class CsvDocument<T> : ICsvDocument, IValueEnumerable<CsvRecord, CsvDocument<T>.Enumerator>
    {
        internal readonly struct TypedRecord
        {
            internal readonly T _value;
            private readonly CsvRecord _record;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TypedRecord(T value, CsvConverterOptions options)
            {
                _record = CsvRecord.From(value, options);
                _value = value;
            }

            public CsvRecord Record
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _record;
            }

            public T Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Deconstruct(out CsvRecord record, out T value)
            {
                record = Record;
                value = _value;
            }

            public override string ToString()
            {
                return _value?.ToString() ?? string.Empty;
            }
        }

        private static readonly TypedRecord[] s_EmptyArray = Array.Empty<TypedRecord>();

        private TypedRecord[] _records;
        private int _count = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        public CsvDocument() : this(CsvConverterOptions.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public CsvDocument(CsvConverterOptions? options = null)
            : this(s_EmptyArray, CsvHeader.FromType<T>(options), options) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDocument{T}"/> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="format">The format.</param>
        public CsvDocument(IEnumerable<T> elements, CsvConverterOptions? options = null)
        {
            _records = s_EmptyArray;
            options ??= CsvConverterOptions.Default;

            Header = new CsvHeader(CsvConverter.GetHeader<T>(), options.Format);
            Options = options;

            foreach (var e in elements)
            {
                Write(e);
            }
        }

        internal CsvDocument(TypedRecord[] records, CsvHeader header, CsvConverterOptions? options)
        {
            options ??= CsvConverterOptions.Default;

            if (header.Format != options.Format)
            {
                throw new ArgumentException("Header format differs from the given format");
            }

            _records = records;
            _count = records.Length;
            Header = header;
            Options = options;
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
        public CsvConverterOptions Options { get; }

        /// <summary>
        /// Gets the number of records in this csv.
        /// </summary>
        /// <value>
        /// The number of records in the csv.
        /// </value>
        public int Count => _count;

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
        public ValueCollection Values => new ValueCollection(this);

        public CsvFormat Format => Options.Format;

        /// <summary>
        /// Writes the specified value as a record.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(T value)
        {
            if (_count == _records.Length)
            {
                Resize(1);
            }

            _records[_count++] = new TypedRecord(value, Options);
        }

        /// <summary>
        /// Writes the specified value as a record in the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void WriteAt(int index, T value)
        {
            if (index < 0 || index >= _count)
            {
                throw ThrowHelper.ArgumentOutOfRange(nameof(index), index, _count);
            }

            if (_count == _records.Length)
            {
                Resize(1);
            }

            Array.Copy(_records, index, _records, index + 1, _count - index);
            _records[index] = new TypedRecord(value, Options);
            _count += 1;
        }

        /// <summary>
        /// Updates the record at the specified index with the given value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Update(int index, T value)
        {
            if (index < 0 || index >= _count)
            {
                throw ThrowHelper.ArgumentOutOfRange(nameof(index), index, _count);
            }

            _records[index] = new TypedRecord(value, Options);
        }

        /// <summary>
        /// Removes the value from this document.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns><c>true</c> if the item was removed, otherwise false.</returns>
        public bool Remove(T value)
        {
            int index = IndexOf(value);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the record at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw ThrowHelper.ArgumentOutOfRange(nameof(index), index, _count);
            }

            _count -= 1;

            if (index < _count)
            {
                Array.Copy(_records, index + 1, _records, index, _count - index);
            }
        }

        /// <summary>
        /// Removes all the records that match the predicate.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The number of removed records</returns>
        public int RemoveAll(Predicate<T> match)
        {
            int index = 0;

            while (index < _count)
            {
                if (match(_records[index].Value))
                {
                    break;
                }

                index += 1;
            }

            int current = index + 1;

            while (current < _count)
            {
                if (!match(_records[current].Value))
                {
                    if (index < current)
                    {
                        _records[index++] = _records[current];
                    }
                }

                current += 1;
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_records, index, _count - index);
            }

            int removed = _count - index;
            _count -= removed;
            return removed;
        }

        /// <summary>
        /// Clears the contents of this csv.
        /// </summary>
        public void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_records, 0, _count);
            }

            _count = 0;
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
            return IndexOf(value) >= 0;
        }

        /// <summary>
        /// Gets the index of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            for (int i = 0; i < _count; i++)
            {
                T recordValue = _records[i].Value;

                if (comparer.Equals(recordValue, value))
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

            for (int i = _count - 1; i >= 0; i--)
            {
                T recordValue = _records[i].Value;

                if (comparer.Equals(recordValue, value))
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
            if (index < 0 || index >= _count)
            {
                throw ThrowHelper.ArgumentOutOfRange(nameof(index), index, _count);
            }

            return _records[index].Value;
        }

        /// <summary>
        /// Gets a readonly reference to the element at the given index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>A readonly reference to the index.</returns>
        public ref readonly T GetValueRef(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw ThrowHelper.ArgumentOutOfRange(nameof(index), index, _count);
            }

            return ref _records[index]._value;
        }

        /// <summary>
        /// Reverse the order of the elements of this <see cref="CsvDocument{T}"/>.
        /// </summary>
        public void Reverse()
        {
            Array.Reverse(_records, 0, _count);
        }

        /// <summary>
        /// Gets a copy of this csv with the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public CsvDocument<T> WithFormat(CsvFormat format)
        {
            CsvConverterOptions options = Options with { Format = format };

            TypedRecord[] array = _records.AsSpan(0, _count).ToArray();
            return new CsvDocument<T>(array, Header.WithFormat(format), options);
        }

        /// <summary>
        /// Gets the column with the specify index.
        /// </summary>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>The column with the given index.</returns>
        public CsvColumn<CsvDocument<T>, Enumerator> GetColumn(int columnIndex)
        {
            return new CsvColumn<CsvDocument<T>, Enumerator>(this, columnIndex);
        }

        /// <summary>
        /// Gets the column with the specify name.
        /// </summary>
        /// <param name="columName">The name of the column.</param>
        /// <returns>The column with the given name.</returns>
        public CsvColumn<CsvDocument<T>, Enumerator> GetColumn(string columName)
        {
            return new CsvColumn<CsvDocument<T>, Enumerator>(this, columName);
        }

        private void Resize(int required)
        {
            int minCapacity = _count + required;

            if (minCapacity > _records.Length)
            {
                int size = _count == 0 ? 1 : _count;
                int newCapacity = Math.Max(minCapacity, size * 2);

                TypedRecord[] newArray = new TypedRecord[newCapacity];
                Array.Copy(_records, newArray, _count);
                _records = newArray;
            }
        }

        public void CopyTo(Stream destination)
        {
            CsvWriter.WriteToStream(this, Header, destination);
        }

        public Task CopyToAsync(Stream destination, CancellationToken cancellationToken = default)
        {
            return CsvWriter.WriteToStreamAsync(this, Header, destination, cancellationToken: cancellationToken);
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

            foreach (TypedRecord typedRecord in _records.AsSpan(0, _count))
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
            return CsvUtility.ToPrettyString(this.Select(record => record));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator() => new(_records, _count);

        IEnumerator<CsvRecord> IEnumerable<CsvRecord>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<CsvRecord>
        {
            private readonly TypedRecord[]? items;
            private readonly int count;
            private int index;

            internal Enumerator(TypedRecord[] items, int count)
            {
                this.items = items;
                this.count = count;
                this.index = -1;
            }

            public CsvRecord Current
            {
                get
                {
                    if (index == -1 || items == null)
                    {
                        throw new InvalidOperationException("enumerator is not initialized");
                    }

                    return items[index].Record;
                }
            }

            object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                int next = index + 1;

                if (next < count)
                {
                    index = next;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                index = -1;
            }

            void IDisposable.Dispose() { }
        }
    }
}
