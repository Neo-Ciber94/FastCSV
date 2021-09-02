using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Converters
{
    public class EnumerableConverter<T> : IValueConverter<IEnumerable<T>>
    {
        public string ItemSuffix { get; } = "Item";

        public EnumerableConverter(string suffix)
        {
            ItemSuffix = suffix;
        }

        public string? ToValue(IEnumerable<T> value)
        {
            string? result = null;



            return result;
        }

        public bool TryParse(string? s, out IEnumerable<T> value)
        {
            throw new NotImplementedException();
        }
    }
}

// item1,item2,item3,item4,item4
