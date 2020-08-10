using System;
using System.Collections.Generic;

namespace DominatorHouseCore.Utility
{
    public static class MoreEnumerable
    {
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            comparer = comparer ?? Comparer<TKey>.Default;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");
                TSource source1 = enumerator.Current;
                TKey y = selector(source1);
                while (enumerator.MoveNext())
                {
                    TSource current = enumerator.Current;
                    TKey x = selector(current);
                    if (comparer.Compare(x, y) < 0)
                    {
                        source1 = current;
                        y = x;
                    }
                }
                return source1;
            }
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return source.ToHashSet(null);
        }
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new HashSet<TSource>(source, comparer);
        }


    }
}
