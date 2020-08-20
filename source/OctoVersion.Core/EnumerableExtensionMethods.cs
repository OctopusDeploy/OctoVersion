using System;
using System.Collections.Generic;

namespace OctoVersion.Core
{
    public static class EnumerableExtensionMethods
    {
        public static IEnumerable<T> DepthFirst<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> getChildren)
        {
            foreach (var item in items)
            {
                yield return item;

                var children = getChildren(item);
                foreach (var descendant in children.DepthFirst(getChildren)) yield return descendant;
            }
        }
    }
}