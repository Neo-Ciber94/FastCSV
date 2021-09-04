using System.Collections.Generic;
using System.IO;
using FastCSV.Utils;

namespace FastCSV.Converters
{
    public class EnumerableValueConverter<T> : IValueConverter<IEnumerable<T>>
    {
        private const string DefaultItemSuffix = "item";

        public string ItemSuffix { get; }

        public CsvFormat Format { get; }

        public EnumerableValueConverter(CsvFormat format, string? itemSuffix = null)
        {
            Format = format;
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

        public virtual string? ToValue(IEnumerable<T> value)
        {
            string? result = null;

            foreach(var e in value)
            {
                if (result == null)
                {
                    result = string.Empty;
                }
            }

            return result;
        }

        public virtual bool TryParse(string? s, out IEnumerable<T> value)
        {
            value = default!;
            IValueConverter? converter = ValueConverters.GetConverter(typeof(T));

            if (s == null || converter == null)
            {
                return false;
            }


            using MemoryStream memoryStream = CsvUtility.ToStream(s);
            using StreamReader reader = new StreamReader(memoryStream);

            List<string>? values = CsvUtility.ReadRecord(reader, Format);

            if (values == null)
            {
                return false;
            }

            List<T> items = new List<T>(values.Count);

            foreach(string e in values)
            {
                if(!converter.TryParse(s, out object? obj))
                {
                    return false;
                }

                items.Add((T)obj!);
            }

            value = items;
            return true;
        }
    }
}
