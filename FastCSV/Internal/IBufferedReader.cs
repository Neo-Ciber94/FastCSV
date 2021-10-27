using System;

namespace FastCSV.Internal
{
    internal interface IBufferedReader<T> : IDisposable
    {
        Span<T> FillBuffer();
        void Consume(int count);
    }
}
