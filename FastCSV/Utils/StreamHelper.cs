using System.IO;

namespace FastCSV.Utils
{
    public static class StreamHelper
    {
        /// <summary>
        /// Gets a <see cref="MemoryStream"/> containing the specified <see cref="string"/> data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A stream containing the specified data.</returns>
        public static MemoryStream ToMemoryStream(string data)
        {
            MemoryStream memory = new MemoryStream(data.Length);
            using (var writer = new StreamWriter(memory, leaveOpen: true))
            {
                writer.Write(data);
                writer.Flush();
                memory.Position = 0;
            }

            return memory;
        }

    }
}
