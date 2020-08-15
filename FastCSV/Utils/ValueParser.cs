using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastCSV.Utils
{
    public delegate bool TryParseDelegate(string key, string value, out object? result);

    public class ValueParser : IValueParser
    {
        private readonly TryParseDelegate _parser;
        private readonly IEnumerable<string> _keys;

        public ValueParser(TryParseDelegate parser, params string[] keys)
        {
            _parser = parser;
            _keys = keys;
        }

        public bool TryParse(string key, string value, out object? result)
        {
            if(_keys.Any())
            {
                if (_keys.Contains(key))
                {
                    return _parser(key, value, out result);
                }

                result = null;
                return false;
            }

            return _parser(key, value, out result);
        }
    }
}
