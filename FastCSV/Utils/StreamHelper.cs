using System;
using System.IO;
using System.Text;

namespace FastCSV.Utils
{
    public static class StreamHelper
    {
        /// <summary>
        /// Gets a <see cref="MemoryStream"/> containing the specified <see cref="string"/> data.
        /// </summary>
        /// <param name="s">The data.</param>
        /// <returns>A stream containing the specified data.</returns>
        public static Stream CreateStreamFromString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return MemoryStream.Null;
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            return new MemoryStream(byteArray);
        }
    }
}
