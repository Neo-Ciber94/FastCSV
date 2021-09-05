using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FastCSV.Utils;

namespace FastCSV.Converters
{
    public class EnumerableValueConverter<T> : IValueConverter<IEnumerable<T>>
    {
        private const string DefaultItemSuffix = "item";

        public string ItemSuffix { get; }

        public CsvFormat Format { get; }

        public IValueConverter Converter { get; }

        public EnumerableValueConverter(CsvFormat format, IValueConverter? converter = null, string? itemSuffix = null)
        {
            converter ??= ValueConverters.GetConverter(typeof(T));

            if (converter == null)
            {
                throw new InvalidOperationException($"No value converter for type {typeof(T)}");
            }

            Format = format;
            Converter = converter;
            ItemSuffix = itemSuffix ?? DefaultItemSuffix;
        }

        public virtual string[] GetHeader(IEnumerable<T> values)
        {
            List<string> items = new List<string>();

            int i = 0;

            foreach(var e in values)
            {
                string name = $"{ItemSuffix}{++i}";
                items.Add(name);
            }

            return items.ToArray();
        }

        public virtual string? ToStringValue(IEnumerable<T> values)
        {
            int count = values.Count();
            
            if (count == 0)
            {
                return null;
            }

            string[] stringValues = new string[count];
            int i = 0;

            foreach(T e in values)
            {
                stringValues[i++] = Converter.ToStringValue(e) ?? string.Empty;
            }
            

            return CsvUtility.ToCsvString(stringValues, Format);
        }

        public virtual bool TryParse(string? s, out IEnumerable<T> values)
        {
            values = default!;
            IValueConverter? converter = ValueConverters.GetConverter(typeof(T));

            if (s == null || converter == null)
            {
                return false;
            }


            using MemoryStream memoryStream = CsvUtility.ToStream(s);
            using StreamReader reader = new StreamReader(memoryStream);

            List<string>? csvValues = CsvUtility.ReadRecord(reader, Format);

            if (csvValues == null)
            {
                return false;
            }

            List<T> items = new List<T>(csvValues.Count);

            foreach(string e in csvValues)
            {
                if(!converter.TryParse(s, out object? obj))
                {
                    return false;
                }

                items.Add((T)obj!);
            }

            values = items;
            return true;
        }
    }
}
