using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCSV.Extensions
{
    internal static class StreamExtensions
    {
        public static Stream? Clone(this Stream self)
        {
            if (!self.CanSeek && !self.CanWrite)
            {
                return null;
            }

            long oldPosition = self.Position;
            MemoryStream memoryStream = new MemoryStream();
            self.Position = 0;
            self.CopyTo(memoryStream);

            memoryStream.Position = 0;
            self.Position = oldPosition;
            return memoryStream;
        }
    }
}
