using System;
using System.Collections.Generic;
using System.Linq;

namespace FastCSV.Collections
{
    /// <summary>
    /// Extension methods for <see cref="Stack{T}"/>.
    /// </summary>
    public static class StackExtensions
    {
        /// <summary>
        /// Push a collection of items into a <see cref="Stack{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="items">The items to push.</param>
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            foreach(var e in items)
            {
                stack.Push(e);
            }
        }

        /// <summary>
        /// Push a collection of items into a <see cref="Stack{T}"/> in reverse order.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="items">The items to pusems"></param>
        public static void PushRangeReverse<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            if (items is IList<T> list)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    stack.Push(list[i]);
                }
            }
            else
            {
                PushRange(stack, items.Reverse());
            }
        }
    }
}
