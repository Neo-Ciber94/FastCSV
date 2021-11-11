using System;
using System.Collections.Generic;

namespace FastCSV.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> enumerable, out int count)
        {
            if (enumerable is ICollection<T> collection)
            {
                count = collection.Count;
                return true;
            }

            if (enumerable is IReadOnlyCollection<T> readOnlyCollection)
            {
                count = readOnlyCollection.Count;
                return true;
            }

            count = 0;
            return false;
        }

        public static bool ContainsSequence<T>(this IEnumerable<T> source, IEnumerable<T> sequence, EqualityComparer<T>? comparer = null)
        {
            if (source.TryGetNonEnumeratedCount(out int sourceCount) && sequence.TryGetNonEnumeratedCount(out int sequenceCount))
            {
                if (sourceCount < sequenceCount)
                {
                    return false;
                }

                if (sequenceCount == 0)
                {
                    return false;
                }
            }

            comparer ??= EqualityComparer<T>.Default;
            int skipCount = 0;

            while (true)
            {
                var sourceEnumerator = source.GetEnumerator();
                var sequenceEnumerator = sequence.GetEnumerator();

                if (!sourceEnumerator.MoveNext() || !sequenceEnumerator.MoveNext())
                {
                    break;
                }

                if (skipCount > 0)
                {
                    for (int i = 0; i < skipCount; i++)
                    {
                        if (!sourceEnumerator.MoveNext())
                        {
                            return false;
                        }
                    }
                }

                while(true)
                {
                    T x = sourceEnumerator.Current;
                    T y = sequenceEnumerator.Current;

                    if (!comparer.Equals(x, y))
                    {
                        skipCount += 1;
                        break;
                    }

                    if (!sequenceEnumerator.MoveNext())
                    {
                        return true;
                    }

                    if (!sourceEnumerator.MoveNext())
                    {
                        break;
                    }
                }
            }

            return false;
        }
    }
}