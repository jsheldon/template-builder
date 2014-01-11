using System;
using System.Collections.Generic;

namespace Forumz.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static void ForEach<T, U>(this IDictionary<T, U> items, Action<T, U> action)
        {
            foreach (var item in items)
            {
                action(item.Key, item.Value);
            }
        }
    }
}