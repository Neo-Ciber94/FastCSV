using System;
using System.Threading.Tasks;

namespace FastCSV.Internal
{
    internal interface IAsyncBufferedReader<T>
    {
        ValueTask<Memory<T>> FillBufferAsync();

        void Consume(int count);
    }
}
