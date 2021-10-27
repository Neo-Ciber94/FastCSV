using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;

namespace FastCSV.Internal
{
    internal static class BufferedReaderExtensions
    {
        //public static Optional<T> ReadNext<TSelf, T>(this TSelf self) 
        //    where T: notnull 
        //    where TSelf : IBufferedReader<T>
        //{
        //    Span<T> buffer = self.FillBuffer();

        //    if (buffer.IsEmpty)
        //    {
        //        return default;
        //    }

        //    T value = buffer[0];
        //    self.Consume(1);
        //    return value;
        //}
    }
}
