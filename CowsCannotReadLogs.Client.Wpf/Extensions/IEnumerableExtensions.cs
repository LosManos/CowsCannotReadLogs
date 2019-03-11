using System.Collections.Generic;
using System.Linq;

namespace CowsCannotReadLogs.Client.Wpf.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal struct ItemStruct<T>
        {
            internal readonly T Item;
            internal readonly int Index;
            internal ItemStruct(T item, int index)
            {
                Item = item;
                Index = index;
            }
        }
        internal static IEnumerable<ItemStruct<T>> SelectWithIndex<T>(this IEnumerable<T> me)
        {
            return me.Select((x, i) => new ItemStruct<T>(x, i));
        }
    }
}
