using System;
using System.Collections.Generic;
using System.Text;

namespace FastCSV.Utils
{
    public delegate bool TryParseDelegate(string value, string key, out object? result);

    public class ValueParser : IValueParser
    {
        private readonly TryParseDelegate _parser;

        public ValueParser(TryParseDelegate parser)
        {
            _parser = parser;
        }

        public bool TryParse(string value, string key, out object? result)
        {
            return _parser(value, key, out result);
        }
    }
}
