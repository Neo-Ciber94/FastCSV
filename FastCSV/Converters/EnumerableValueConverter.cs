using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FastCSV.Utils;

namespace FastCSV.Converters
{
    public class EnumerableValueConverter : IValueConverter<IEnumerable>
    {
        private const string DefaultItemName = "item";

        public string ItemName { get; }

        public CsvFormat Format { get; }

        public IValueConverter Converter { get; }

        public Type ElementType { get; }

        public EnumerableValueConverter(CsvFormat format, Type elementType, IValueConverter? converter = null, string? itemName = null)
        {
            IValueConverter? c = converter?? ValueConverters.GetConverter(elementType);

            if (c == null)
            {
                throw new InvalidOperationException($"No value converter for type {elementType}");
            }

            Format = format;
            Converter = c;
            ElementType = elementType;
            ItemName = itemName ?? DefaultItemName;
        }

        public bool CanConvert(Type type)
        {
            return true;
        }

        public virtual string[] GetHeader(IEnumerable values)
        {
            // Returns: item1,item2,item3,item4, ...
            return values
                .Cast<object>()
                .Select((value, index) => $"{ItemName}{index + 1}")
                .ToArray();
        }

        public virtual string? Read(IEnumerable values)
        {
            int count = values.Cast<object>().Count();
            
            if (count == 0)
            {
                return null;
            }

            string[] stringValues = new string[count];
            int i = 0;

            foreach(var e in values)
            {
                stringValues[i++] = Converter.Read(e) ?? string.Empty;
            }
            

            return CsvUtility.ToCsvString(stringValues, Format);
        }

        public virtual bool TryParse(string? s, out IEnumerable values)
        {
            values = default!;

            if (s == null)
            {
                return false;
            }

            using MemoryStream memoryStream = StreamHelper.ToMemoryStream(s);
            using StreamReader reader = new StreamReader(memoryStream);

            List<string>? csvValues = CsvUtility.ReadRecord(reader, Format);

            if (csvValues == null)
            {
                return false;
            }

            List<object> items = new List<object>(csvValues.Count);

            foreach(string e in csvValues)
            {
                if(!Converter.TryParse(s, out object? obj))
                {
                    return false;
                }

                items.Add(obj!);
            }

            values = items;
            return true;
        }
    }
}
