using System;

namespace FastCSV.Internal
{
    internal interface IBufferedReader<T> : IDisposable
    {
        ReadOnlySpan<T> FillBuffer();
        void Consume(int count);
    }
}
